using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
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
  class ClsCommandSyabariHiuchiBase
  {
    public static bool CommandCreateSyabariHiuchiBase( UIDocument uidoc )
    {
      Document doc = uidoc.Document ;
      List<string> firterList = new List<string> { ClsHaraokoshiBase.baseName, "モデル線分" } ;
      var mess = "腹起ベースorモデル線分を選択してください" ;

      while ( true ) {
        try {
          Reference rfHara = null ;
          if ( ! ClsRevitUtil.PickObjectFilters( uidoc, mess, firterList, ref rfHara ) )
            return false ;
          var idHara = rfHara.ElementId ;
          var picPoint = rfHara.GlobalPoint ;
          var instHara = doc.GetElement( idHara ) ;
          var cvHara = ClsYMSUtil.GetBaseLine( doc, idHara ) ;
          //var cvHara = (instHara.Location as LocationCurve).Curve;

          ElementId idSyabari = null ;
          if ( ! ClsSyabariBase.PickBaseOrKariKouzaiObject( uidoc, ref idSyabari ) )
            return false ;
          //var instSyabari = doc.GetElement(idSyabari);
          var cvSyabari = ClsYMSUtil.GetBaseLine( doc, idSyabari ) ;

          //仮鋼材を配置すると接続しなくなるため
          //var insec = ClsRevitUtil.GetIntersection(cvHara, cvSyabari);
          //if (insec == null)
          //{
          //    //MessageBox.Show("腹起ベースまたはモデル線分と斜梁ベースが接続していません。");
          //    //return false;
          //}

          if ( ! Check90( cvHara, cvSyabari ) ) {
            MessageBox.Show( "腹起ベースまたはモデル線分と斜梁ベースが90度ではありません。" ) ;
            return false ;
          }

          string tanbuType, tanbuSize ;
          var csh = new ClsSyabariBase() ;
          csh.SetClassParameter( doc, idSyabari ) ;
          if ( ClsRevitUtil.CheckNearGetEndPoint( cvSyabari, picPoint ) ) {
            tanbuType = csh.m_buhinTypeStart ;
            tanbuSize = csh.m_buhinSizeStart ;
          }
          else {
            tanbuType = csh.m_buhinTypeEnd ;
            tanbuSize = csh.m_buhinSizeEnd ;
          }

          var dlg = new DLG.DlgCreateSyabariHiuchiBase( tanbuType, tanbuSize ) ;
          var result = dlg.ShowDialog() ;

          if ( result != DialogResult.OK ) {
            return false ;
          }

          var clsSyabariHiuchi = dlg.m_SyabariHiuchiBase ;

          clsSyabariHiuchi.CreateSyabariHiuchiBase( doc, rfHara, idSyabari ) ;
        }
        catch ( Exception ex ) {
          //#if DEBUG
          TaskDialog taskDialog = new TaskDialog( $"[{typeof( ClsCommandSyabariHiuchiBase ).Name}] Error!!" )
          {
            TitleAutoPrefix = true,
            MainIcon = TaskDialogIcon.TaskDialogIconError,
            MainInstruction = ex.GetType().ToString().Split( '.' ).LastOrDefault(),
            MainContent = ex.Message,
            ExpandedContent = ex.StackTrace,
            CommonButtons = TaskDialogCommonButtons.Close,
          } ;
          taskDialog.Show() ;
          //#endif
          break ;
        }
      }

      return true ;
    }

    private static bool Check90( Line cv1, Line cv2 )
    {
      // M.Sakuraba XY平面上での直交判定へ変更
      var projXY = SMatrix3d.ProjXY ;
      var dir1XY = projXY.TransformVector( cv1.Direction ) ;
      var dir2XY = projXY.TransformVector( cv2.Direction ) ;
      var dotProd = dir1XY.DotProduct( dir2XY ) ;
      return ClsGeo.GEO_EQ( dotProd, 0.0 ) ;
      //var dir1 = (cv1 as Line).Direction;
      //var dir2 = (cv2 as Line).Direction;
      //return Check90(dir1, dir2);
    }

    //public static bool Check90(XYZ dir1, XYZ dir2)
    //{
    //    var pi2 = ClsGeo.FloorAtDigitAdjust(5, Math.PI / 2);
    //    var pi32 = ClsGeo.FloorAtDigitAdjust(5, Math.PI * 3 / 2);
    //    var angle = dir1.AngleOnPlaneTo(dir2, XYZ.BasisZ);
    //    angle = ClsGeo.FloorAtDigitAdjust(5, angle);
    //    if (angle == pi2 || angle == -pi2 || angle == pi32 || angle == -pi32)
    //        return true;//90度pass
    //    return false;
    //}
    public static bool CommandChangeSyabariHiuchiBase( UIDocument uidoc )
    {
      Document doc = uidoc.Document ;

      List<ElementId> ids = null ;
      if ( ! ClsSyabariHiuchiBase.PickBaseObjects( uidoc, ref ids ) )
        return false ;
      if ( ids.Count < 1 )
        return false ;

      var clsSyabariHiuchi = new ClsSyabariHiuchiBase() ;
      //基本同じものを選択している想定。違う場合でも最終的には同じパラメータに直すので1つ目のデータを採用
      clsSyabariHiuchi.SetClassParameter( doc, ids[ 0 ] ) ;
      //DlgCreateSyabariHiuchiBase
      var dlg = new DLG.DlgCreateSyabariHiuchiBase( clsSyabariHiuchi ) ;
      DialogResult result = dlg.ShowDialog() ;

      if ( result != DialogResult.OK ) {
        return false ;
      }

      clsSyabariHiuchi = dlg.m_SyabariHiuchiBase ;

      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        t.Start() ;
        foreach ( var id in ids ) {
          clsSyabariHiuchi.SetParameter( doc, id ) ;
        }

        t.Commit() ;
      }

      return true ;
    }
  }
}