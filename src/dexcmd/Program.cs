using System;
using System.Threading.Tasks;
using CommandLine;

namespace KustoIngest
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
