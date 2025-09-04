using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;
using YMS_gantry.UI ;

namespace YMS_gantry.Command
{
  class CmdKanshouCheck
  {
    static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger() ;
    Document _doc { get ; set ; }
    UIDocument _uiDoc { get ; set ; }

    public CmdKanshouCheck( UIDocument uiDoc )
    {
      _uiDoc = uiDoc ;
      _doc = _uiDoc.Document ;
    }
  }
}