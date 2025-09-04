using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using Autodesk.Revit.UI.Selection ;
using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;
using YMS.Parts ;

namespace YMS.Command
{
  class ClsCommandPositionSanjikuPiece
  {
    /// <summary>
    /// 三軸ピース作図
    /// </summary>
    /// <param name="uidoc"></param>
    public static void CommandPositionSanjikuPiece( UIDocument uidoc )
    {
      if ( ClsSanjikuPeace.SetSanjikuPeace( uidoc ) ) {
        ClsSanjikuPeace.MovedSanjikuPeace( uidoc ) ;
        Application.thisApp.ShowForm_dlgCreateSanjiku() ;
      }

      return ;
    }
  }
}