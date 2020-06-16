using Alba.CsConsoleFormat;
using Microsoft.Azure.Management.Kusto;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using dexcmd.Model;
using Kusto.Cloud.Platform.Data;
using Kusto.Data;
using Kusto.Data.Common;
using Microsoft.Identity.Client;
using kClient = Kusto.Data.Net.Client;
using Newtonsoft.Json;
using static System.ConsoleColor;
using AuthenticationResult = Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationResult;
using ClientCredential = Microsoft.IdentityModel.Clients.ActiveDirectory.ClientCredential;

namespace dexcmd
{
   public class KustoFunctions
   {
      private readonly Options _options;
      public const string AUTHORITY = "https://login.microsoftonline.com/";

      public KustoFunctions(Options options)
      {
         _options = options;
      }

      #region List Databases 
      /// <summary>
      /// Given an ADX cluster returns a list of databases in that cluster with a row count and on disk compressed volume
      /// </summary>
      public async Task ListDatabases()
      {
         try
         {
            var client = await GetManagementClient();
            var databases =
               await client.Databases.ListByClusterAsync(resourceGroupName: _options.ResourceGroup,
                  clusterName: $"{_options.KustoClusterName}");

            var headerThickness = new LineThickness(LineWidth.Single, LineWidth.Single);

            var doc = new Document(
               new Grid
               {
                  Color = Gray,
                  Columns = { GridLength.Auto, GridLength.Char(20), GridLength.Char(20) },
                  Children = {
                     new Cell("Name") { Stroke = headerThickness},
                     new Cell("Row Count") { Stroke = headerThickness },
                     new Cell("Cache Size (GB)") { Stroke = headerThickness },
                     databases.Select(item => 
                     {
                        var databasesQuery = GetDataAdminReader(item.Name.Split('/')[1], ".show database datastats").Result;
                        var dataStats = new KustoDatastats(databasesQuery);
                        return new[]
                        {
                           new Cell(dataStats.DatabaseName) {Color = Yellow},
                           new Cell(dataStats.HotRowCount),
                           new Cell(dataStats.HotCompressedSize.ToString("##,##0.00000000", CultureInfo.InvariantCulture)) {Align = Align.Right},
                        };
                     })
                  }
               }
            );
            
            ConsoleRenderer.RenderDocument(doc);
         }
         catch (Exception ex)
         {
            Console.WriteLine(ex);
         }
      }

      #endregion

      #region List Tables 

      public async Task ListTables(string databaseName)
      {
         try
         {
            List<KustoTableDetail> tableDetails = new List<KustoTableDetail>();
            var databasesQuery = await GetDataAdminReader(databaseName, ".show tables details");
            var allTables = databasesQuery.FromDataReader(databaseName);
            
            foreach(DataRow row in allTables.Tables[0].Rows)
            {
               tableDetails.Add(new KustoTableDetail(row));
            }

            var headerThickness = new LineThickness(LineWidth.Single, LineWidth.Single);

            var doc = new Document(
               new Grid
               {
                  Color = Gray,
                  Columns = { GridLength.Auto, GridLength.Char(20), GridLength.Char(20), GridLength.Auto },
                  Children = {
                     new Cell("Name") { Stroke = headerThickness},
                     new Cell("Row Count") { Stroke = headerThickness },
                     new Cell("Cache Size (GB)") { Stroke = headerThickness },
                     new Cell("Users/Groups") { Stroke = headerThickness },
                     tableDetails.Select(item =>
                     {
                        var kustoAuthorised = JsonConvert.DeserializeObject<List<KustoAuthorisedPrincipals>>(item.AuthorizedPrincipals);
                        var kaWithType = kustoAuthorised.Select(item => item.DisplayName + $" [{item.Type}]");
                        string principals = String.Join('\n', kaWithType);
                        double extentSize = item.TotalExtentSize / 1000000000;
                        return new[]
                        {
                           new Cell(item.TableName) {Color = Yellow},
                           new Cell(item.TotalRowCount),
                           new Cell(extentSize.ToString("##,##0.00000000", CultureInfo.InvariantCulture)) {Align = Align.Right},
                           new Cell(principals) {Color = Yellow}, 
                        };
                     })
                  }
               }
            );

            ConsoleRenderer.RenderDocument(doc);
         }
         catch (Exception ex)
         {
            Console.WriteLine(ex);
         }
      }

      #endregion

      #region Helpers

      public bool LoginInteractively => !String.IsNullOrEmpty(_options.InteractiveClientId);
      private async Task<string> GetAadToken(Options options, string resource = "https://management.core.windows.net/")
      {
         if (LoginInteractively)
         {
            var publicApp = PublicClientApplicationBuilder.Create(options.InteractiveClientId)
               .WithAuthority(AadAuthorityAudience.None)
               .WithTenantId(options.TenantId)
               .WithDefaultRedirectUri()
               .Build();
            var scopes = new[] { $"{resource}/user_impersonation" };
            var resultInteractive = publicApp.AcquireTokenInteractive(scopes);

            return (await resultInteractive.WithPrompt(Prompt.SelectAccount).ExecuteAsync()).AccessToken;
         }
         // get a token for the Graph without triggering any user interaction (from the cache, via multi-resource refresh token, etc)
         ClientCredential creds = new ClientCredential(options.ClientId, options.ClientSecret);
         // Get auth token from auth code
         var authContext = new AuthenticationContext(AUTHORITY + options.TenantId);
         var result = await authContext.AcquireTokenAsync(resource, creds);
         // Acquire user token for the interactive user for Kusto:
         return result.AccessToken;
      }

      private async Task<KustoManagementClient> GetManagementClient()
      {
         var token = await GetAadToken(_options);
         var client = new KustoManagementClient(credentials: new TokenCredentials(token))
         {
            SubscriptionId = _options.SubscriptionId
         };
         return client;
      }

      private async Task<IDataReader> GetDataAdminReader(string databaseName, string query)
      {
         string resource = $"https://{_options.KustoClusterName}.northeurope.kusto.windows.net";
         KustoConnectionStringBuilder kcsb;
         if (!LoginInteractively)
         {
            kcsb = new KustoConnectionStringBuilder(resource)
            {
               ApplicationClientId = _options.ClientId,
               ApplicationKey = _options.ClientSecret,
               Authority = _options.TenantId,
               FederatedSecurity = true,
               InitialCatalog = databaseName,
            };
         }
         else
         {
            kcsb = new KustoConnectionStringBuilder(resource)
            {
               ApplicationToken = await GetAadToken(_options, resource),
               Authority = _options.TenantId,
               FederatedSecurity = true,
               InitialCatalog = databaseName,
            };
         }
         
         var admin = kClient.KustoClientFactory.CreateCslQueryProvider(kcsb);
         return await admin.ExecuteQueryAsync(databaseName, query, new ClientRequestProperties());
      }
      #endregion
   }
}
