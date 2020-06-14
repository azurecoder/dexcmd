using System;
using System.Collections.Generic;
using System.Text;

namespace dexcmd.Model
{
   public class KustoAuthorisedPrincipals
   {
      public string Type { get; set; }
      public string DisplayName { get; set; }
      public string ObjectId { get; set; }
      public string FQN { get; set; }
      public string Notes { get; set; }
      public string RoleAssignmentIdentifier { get; set; }
   }
}
