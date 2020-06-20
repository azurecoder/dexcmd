using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alba.CsConsoleFormat;
using dexcmd.Model;
using Kusto.Cloud.Platform.Data;
using Newtonsoft.Json;
using static System.ConsoleColor;

namespace dexcmd.Functions
{
   internal class ListTables : IKustoFunction
   {
      public async Task Execute(KustoFunctionsState functionsState)
      {
         try
         {
            List<KustoTableDetail> tableDetails = new List<KustoTableDetail>();
            var databasesQuery = await functionsState.GetDataAdminReader(functionsState._options.DatabaseName, ".show tables details");
            var allTables = databasesQuery.FromDataReader(functionsState._options.DatabaseName);

            foreach (DataRow row in allTables.Tables[0].Rows)
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

      public KustoFunctionsEnum KustoCommandType => KustoFunctionsEnum.ListTables;

      public static IKustoFunction Create()
      {
         return new ListTables();
      }
   }
}
