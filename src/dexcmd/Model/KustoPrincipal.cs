using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace dexcmd.Model
{
   public class KustoPrincipal
   {
      private readonly DataRow _row;

      public KustoPrincipal(DataRow row)
      {
         _row = row;
      }

      public string Role => (string) _row.ItemArray[0];
      public string PrincipalType => (string) _row.ItemArray[1];
      public string PrincipalDisplayName => (string) _row.ItemArray[2];
      public string PrincipalObjectId => (string) _row.ItemArray[3];
      public string PrincipalFQN => (string) _row.ItemArray[4];
   }
}
