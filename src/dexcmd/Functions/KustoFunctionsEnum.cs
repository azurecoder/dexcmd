using System;
using System.Collections.Generic;
using System.Text;

namespace dexcmd.Functions
{
   /// <summary>
   /// An enum which contains the various functions as flags 
   /// </summary>
   [Flags]
   public enum KustoFunctionsEnum
   {
      None = 0,
      ListDatabases = 1,
      ListTables = 2,
      AddUser = 4
   }
}
