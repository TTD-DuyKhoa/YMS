using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;
using System.Windows.Media ;
using YMS.DLG ;
using YMS.Parts ;

namespace YMS.Command
{
  internal class ClsCommandShimetukePiece
  {
    public static void CommandCreateShimetukePiece( UIApplication uiapp )
    {
      UIDocument uidoc = uiapp.ActiveUIDocument ;
      Document doc = uidoc.Document ;

      while ( true ) {
        try {
          // 対象は切梁ブラケットまたは切梁受材
          ElementId trgBracketId = null ;
          if ( ! ClsShimetukePiece.PickObject( uidoc, ref trgBracketId, "" ) ) {
            return ;
          }

          //切梁ベースは選択させない
          ElementId trgKiribariId = null ;
          if ( ! ClsRevitUtil.PickObject( uidoc, "対象の切梁を選択してください", "切梁", ref trgKiribariId ) ) {
            return ;
          }

          ClsShimetukePiece clsShimetukePiece = new ClsShimetukePiece() ;
          clsShimetukePiece.CreateShimetukePiece( doc, trgBracketId, trgKiribariId ) ;
        }
        catch ( Exception e ) {
          break ;
        }
      }
    }
  }
}