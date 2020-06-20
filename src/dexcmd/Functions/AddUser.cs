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
   internal class AddUser : IKustoFunction
   {
      public async Task Execute(KustoFunctionsState functionsState)
      {
         try
         {
            var databasesQuery = await functionsState.GetDataAdminReader(functionsState._options.DatabaseName, 
               $".add table {functionsState._options.TableName} admins('aaduser={functionsState._options.UserName}')", true);

            var allTables = databasesQuery.FromDataReader(functionsState._options.DatabaseName);

            var principals = (from DataRow row in allTables.Tables[0].Rows select new KustoPrincipal(row)).ToList();

            var headerThickness = new LineThickness(LineWidth.Single, LineWidth.Single);

            var doc = new Document(
               new Grid
               {
                  Color = Gray,
                  Columns = { GridLength.Auto, GridLength.Char(20), GridLength.Char(20), GridLength.Auto },
                  Children = {
                     new Cell("Role") { Stroke = headerThickness},
                     new Cell("Principal Type") { Stroke = headerThickness },
                     new Cell("Display Name") { Stroke = headerThickness },
                     new Cell("PrincipalFQN") { Stroke = headerThickness },
                     principals.Select(item =>
                     {
                        return new[]
                        {
                           new Cell(item.Role) {Color = Yellow},
                           new Cell(item.PrincipalType),
                           new Cell(item.PrincipalDisplayName),
                           new Cell(item.PrincipalFQN) {Color = Yellow},
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

      public KustoFunctionsEnum KustoCommandType => KustoFunctionsEnum.AddUser;

      public static IKustoFunction Create()
      {
         return new AddUser();
      }
   }
}
