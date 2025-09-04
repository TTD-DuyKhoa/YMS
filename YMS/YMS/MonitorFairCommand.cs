using Autodesk.Revit.Attributes ;
using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;

namespace YMS
{
  [Transaction( TransactionMode.Manual )]
  public class MonitorFairCommand : IExternalCommand
  {
    #region Methods (SC)

    public Result Execute( ExternalCommandData revit, ref string message, ElementSet elements )
    {
      //Messaging.DebugMessage("FairDummyCommand.Execute");

      return Result.Succeeded ;
    }

    #endregion
  }
}