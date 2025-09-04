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
  public class ClsCommandStiffener
  {
    public static bool CreateStiffener( UIApplication uiapp )
    {
      UIDocument uidoc = uiapp.ActiveUIDocument ;
      Document doc = uidoc.Document ;

      // ジャッキ作成ダイアログを表示
      DLG.DlgCreateStiffener dlgCreateStiffener = new DLG.DlgCreateStiffener( doc ) ;
      DialogResult result = dlgCreateStiffener.ShowDialog() ;
      if ( result != DialogResult.OK ) {
        return false ;
      }

      ClsStiffener csf = dlgCreateStiffener.m_ClsStiffener ;

      //スチフナーを作成するレベルのID
      ElementId targetLevelId = ClsRevitUtil.GetLevelID( doc, csf.m_Level ) ;

      //指定レベルにある腹起ベースを全て取得
      List<ElementId> haraTargetLevelList = new List<ElementId>() ;
      List<ElementId> haraAllList = ClsHaraokoshiBase.GetAllHaraokoshiBaseList( doc ) ;
      foreach ( ElementId haraId in haraAllList ) {
        FamilyInstance haraInst = doc.GetElement( haraId ) as FamilyInstance ;
        if ( targetLevelId == haraInst.Host.Id )
          haraTargetLevelList.Add( haraId ) ;
      }

      if ( haraTargetLevelList.Count == 0 )
        return false ;

      //指定のレベルにある腹起（仮鋼材も）を全て拾い集め挿入点（Pointだと思われる）のListを作る
      //各ベースの鋼材サイズ幅の内側に上記点が入っているかの判定を行う


      foreach ( ElementId haraId in haraTargetLevelList ) {
        FamilyInstance haraInst = doc.GetElement( haraId ) as FamilyInstance ;
        Curve haraCv = ( haraInst.Location as LocationCurve ).Curve ;
        XYZ haraDir = Line.CreateBound( haraCv.GetEndPoint( 0 ), haraCv.GetEndPoint( 1 ) ).Direction ;
        string haraSize = ClsRevitUtil.GetParameter( doc, haraId, "鋼材サイズ" ) ;
        string dan = ClsRevitUtil.GetParameter( doc, haraId, "段" ) ;

        //腹起+切梁or火打ブロック
        csf.m_StiffenerType = csf.m_Kiribari_StiffenerType ;
        csf.m_StiffenerJackType = csf.m_Kiribari_StiffenerJackType ;
        csf.m_StiffenerPlateType = csf.m_Kiribari_StiffenerPlateType ;
        csf.m_Count = csf.m_Kiribari_Count ;
        List<ElementId> allList = ClsKiribariBase.GetAllKiribariBaseList( doc ) ;
        foreach ( ElementId id in allList ) {
          string kiriSize = ClsRevitUtil.GetParameter( doc, id, "鋼材サイズ" ) ;
          double tanbuHaba = Master.ClsYamadomeCsv.GetWidth( kiriSize ) ;
          FamilyInstance inst = doc.GetElement( id ) as FamilyInstance ;
          Curve cv = ( inst.Location as LocationCurve ).Curve ;
          XYZ kiriDir = Line.CreateBound( cv.GetEndPoint( 0 ), cv.GetEndPoint( 1 ) ).Direction ;

          string hiuchiName = "なし" ;
          //始点側
          XYZ point = null ;
          if ( ClsRevitUtil.IsPointOnCurve( haraCv, cv.GetEndPoint( 0 ) ) ) {
            point = cv.GetEndPoint( 0 ) ;
            hiuchiName = ClsRevitUtil.GetParameter( doc, id, "端部部品(始点側)" ) ;
          }

          //終点側
          if ( ClsRevitUtil.IsPointOnCurve( haraCv, cv.GetEndPoint( 1 ) ) ) {
            point = cv.GetEndPoint( 1 ) ;
            kiriDir = Line.CreateBound( cv.GetEndPoint( 1 ), cv.GetEndPoint( 0 ) ).Direction ;
            hiuchiName = ClsRevitUtil.GetParameter( doc, id, "端部部品(終点側)" ) ;
          }

          if ( point == null )
            continue ;

          if ( hiuchiName != "なし" && hiuchiName.Contains( "火打ブロック" ) ) {
            //腹起のジョイントに作成位置が来ていないかの判定
            double dist = ClsRevitUtil.CovertToAPI( tanbuHaba ) ;
            XYZ moveHalfSizePoint = new XYZ( point.X - dist / 2 * haraDir.X, point.Y - dist / 2 * haraDir.Y, point.Z ) ;
            List<XYZ> vertexList = ClsRevitUtil.GetQuadrilateralForOnePoint( moveHalfSizePoint, haraDir, dist, 4 ) ;
            List<XYZ> haraList = ClsHaraokoshiBase.GetALLHaraokoshInsertPointList( doc ) ;
            bool create = true ;
            foreach ( XYZ insertPoint in haraList ) {
              if ( ClsRevitUtil.IsInArea( vertexList, insertPoint, XYZ.BasisZ ) ) {
                create = false ;
                break ;
              }
            }

            if ( create ) {
              csf.m_Count = csf.m_HiuchiBlock_Kiribari_Count ;
              csf.CreateStiffener( doc, haraSize, dan, point, haraDir, targetLevelId, tanbuHaba ) ;
            }

            //火打ブロック側
            csf.m_Count = csf.m_HiuchiBlock_Hiuchi_Count ;
            string hiuchiSize = Master.ClsHiuchiCsv.GetSize( hiuchiName, kiriSize ) ;
            if ( ! hiuchiName.Contains( "小" ) ) {
              double block =
                ClsRevitUtil.CovertToAPI( 3000.0 / 2 -
                                          tanbuHaba / 2 ) ; // Master.ClsHiuchiCsv.GetThickness(hiuchiSize));
              XYZ pointR = new XYZ( point.X + block * haraDir.X, point.Y + block * haraDir.Y, point.Z ) ;
              //kiriDir = ClsRevitUtil.RotationVector(kiriDir);
              csf.CreateStiffener( doc, haraSize, dan, pointR, haraDir, targetLevelId, tanbuHaba ) ;

              XYZ pointL = new XYZ( point.X - block * haraDir.X, point.Y - block * haraDir.Y, point.Z ) ;
              csf.CreateStiffener( doc, haraSize, dan, pointL, haraDir, targetLevelId, tanbuHaba ) ;
            }
            else {
              double block = ClsRevitUtil.CovertToAPI( 1560.0 / 2 ) ; // Master.ClsHiuchiCsv.GetThickness(hiuchiSize));
              XYZ pointR = new XYZ( point.X + block * haraDir.X, point.Y + block * haraDir.Y, point.Z ) ;
              //kiriDir = ClsRevitUtil.RotationVector(kiriDir);
              csf.CreateStiffener( doc, haraSize, dan, pointR, haraDir, targetLevelId, tanbuHaba ) ;

              XYZ pointL = new XYZ( point.X - block * haraDir.X, point.Y - block * haraDir.Y, point.Z ) ;
              csf.CreateStiffener( doc, haraSize, dan, pointL, haraDir, targetLevelId, tanbuHaba ) ;
            }
          }
          else //切梁端部部品なし
          {
            csf.m_Count = csf.m_Kiribari_Count ;
            csf.CreateStiffener( doc, haraSize, dan, point, haraDir, targetLevelId, tanbuHaba ) ;
          }
        }

        //腹起＋切梁火打ち
        csf.m_StiffenerType = csf.m_KiribariHiuchi_StiffenerType ;
        csf.m_StiffenerJackType = csf.m_KiribariHiuchi_StiffenerJackType ;
        csf.m_StiffenerPlateType = csf.m_KiribariHiuchi_StiffenerPlateType ;
        csf.m_Count = csf.m_KiribariHiuchi_Count ;
        allList = ClsKiribariHiuchiBase.GetAllKiribariHiuchiBaseList( doc ) ;
        foreach ( ElementId id in allList ) {
          FamilyInstance inst = doc.GetElement( id ) as FamilyInstance ;
          Curve cv = ( inst.Location as LocationCurve ).Curve ;
          XYZ point = cv.GetEndPoint( 1 ) ;
          if ( ClsRevitUtil.IsPointOnCurve( haraCv, point ) ) //腹起側は終点側にしか付かない
          {
            string hiuchiSize = ClsRevitUtil.GetParameter( doc, id, "部品サイズ(腹起側)" ) ;
            double tanbuHaba = Master.ClsHiuchiCsv.GetHaraGroundLength( hiuchiSize ) ;
            csf.CreateStiffener( doc, haraSize, dan, point, haraDir, targetLevelId, tanbuHaba ) ;
          }
        }

        //腹起+隅火打
        csf.m_StiffenerType = csf.m_CornerHiuchi_StiffenerType ;
        csf.m_StiffenerJackType = csf.m_CornerHiuchi_StiffenerJackType ;
        csf.m_StiffenerPlateType = csf.m_CornerHiuchi_StiffenerPlateType ;
        csf.m_Count = csf.m_CornerHiuchi_Count ;
        allList = ClsCornerHiuchiBase.GetAllCornerHiuchiBaseList( doc ) ;
        foreach ( ElementId id in allList ) {
          FamilyInstance inst = doc.GetElement( id ) as FamilyInstance ;
          Curve cv = ( inst.Location as LocationCurve ).Curve ;

          for ( int i = 0 ; i < 2 ; i++ ) {
            XYZ point = cv.GetEndPoint( i ) ;
            if ( ClsRevitUtil.IsPointOnCurve( haraCv, point ) ) {
              string hiuchiSize = ClsRevitUtil.GetParameter( doc, id, "火打受ピースサイズ(" + ( i + 1 ).ToString() + ")" ) ;
              double tanbuHaba = Master.ClsHiuchiCsv.GetHaraGroundLength( hiuchiSize ) ;
              csf.CreateStiffener( doc, haraSize, dan, point, haraDir, targetLevelId, tanbuHaba ) ;
            }
          }
        }
      }

      return true ;
    }
  }
}