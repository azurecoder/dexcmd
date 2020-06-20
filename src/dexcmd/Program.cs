using System;
using System.Threading.Tasks;
using CommandLine;
using dexcmd.Functions;
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
                  var functions = new KustoFunctionsState(kustoArgs);
                  KustoFunctionsFactory factory = new KustoFunctionsFactory();
                  await factory.ProcessCommands(functions);
                  Console.WriteLine("Press any key to exit ..");
                  Console.Read();
               });
      }
   }
}
