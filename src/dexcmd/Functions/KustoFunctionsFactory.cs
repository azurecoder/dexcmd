using System.Threading.Tasks;

namespace dexcmd.Functions
{
   public class KustoFunctionsFactory
   {
      public async Task ProcessCommands(KustoFunctionsState functionsState)
      {
         IKustoFunction[] functions = {ListDatabases.Create(), ListTables.Create(), AddUser.Create()};
         KustoFunctionsEnum kustoEnum = KustoFunctionsEnum.None;
         if (functionsState._options.ListDatabases) kustoEnum |= KustoFunctionsEnum.ListDatabases;
         if (functionsState._options.ListTables) kustoEnum |= KustoFunctionsEnum.ListTables;
         // TODO: Need to ensure that there is error handling within the contained classes for things like database and table names - should be a validate method on the interface
         foreach (var function in functions)
         {
            if ((function.KustoCommandType & kustoEnum) == function.KustoCommandType)
            {
               await function.Execute(functionsState);
            }
         }
      }
   }
}