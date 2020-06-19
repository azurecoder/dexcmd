using System.Threading.Tasks;

namespace dexcmd.Functions
{
   public interface IKustoFunction
   {
      public Task Execute(KustoFunctions functions);
   }
}