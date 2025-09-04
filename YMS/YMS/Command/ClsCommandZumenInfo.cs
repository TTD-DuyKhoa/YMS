using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using Autodesk.Revit.UI.Selection ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;
using YMS.Parts ;

namespace YMS.Command
{
  class ClsCommandZumenInfo
  {
    public static bool CommandZumenInfo( UIDocument uidoc )
    {
      Document doc = uidoc.Document ;
      ClsZumenInfo clsZumenInfo = new ClsZumenInfo() ;
      DLG.DlgZumenInfo zumenInfo = new DLG.DlgZumenInfo( clsZumenInfo ) ;
      //図面から情報を取得する
      clsZumenInfo.GetProjectInfo( doc ) ;

      DialogResult result = zumenInfo.ShowDialog() ;
      if ( result != DialogResult.OK ) {
        return false ;
      }

      clsZumenInfo = zumenInfo.m_ClsZumenInfo ;

      clsZumenInfo.SaveProjectInfo( doc ) ;

      //ClsKabeShin.GetCsvLineKabe(doc, out List<string> kabeList);
      //ClsKabeShin.GetCsvLineKui(doc, out List<string> kuiList);

      //ElementId id = uidoc.Selection.PickObject(ObjectType.Element).ElementId;
      //ClsYMSUtil.CreateBaseElevationView(doc, id);
      //ClsYMSUtil.ChangeView(uidoc, "TEST");
      return true ;
    }
  }
}