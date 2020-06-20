using System;
using System.Threading.Tasks;

namespace dexcmd.Functions
{
   /// <summary>
   /// Interface used to define key functionsState for a Factory method for each dexcmd
   /// </summary>
   public interface IKustoFunction
   {
      /// <summary>
      /// Executes the command for dexcmd with the desired state
      /// </summary>
      /// <param name="functionsState">The current state of the kusto commands</param>
      public Task Execute(KustoFunctionsState functionsState);
      /// <summary>
      /// Returns the type of command
      /// </summary>
      public KustoFunctionsEnum KustoCommandType { get; }

   }
}