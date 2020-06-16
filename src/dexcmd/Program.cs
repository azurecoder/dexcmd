using System;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.IdentityModel.Clients.ActiveDirectory.Extensibility;

namespace dexcmd
{
   class Program
   {
      static async Task Main(string[] args)
      {
         var options = await Parser.Default.ParseArguments<Options>(args)
            .WithParsedAsync<Options>(async (opt) =>
            {
            var kustoArgs = Options.GetConfigArguments(opt);
            if (opt.ListDatabases)
            {
               var functions = new KustoFunctions(kustoArgs);
               await functions.ListDatabases();
            }
            else if (opt.ListTables)
            {
               if (String.IsNullOrEmpty(opt.DatabaseName))
               {
                  throw new ApplicationException("Database name should be supplied to list all tables");
               }
               var functions = new KustoFunctions(kustoArgs);
               await functions.ListTables(opt.DatabaseName);
            }
            else
            {
               Console.WriteLine("No functions requested ... ");
            }
            });
         Console.WriteLine("Press any key to exit ..");
         Console.Read();
      }
   }
}
