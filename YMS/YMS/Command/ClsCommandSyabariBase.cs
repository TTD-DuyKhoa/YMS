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
  class ClsCommandSyabariBase
  {
    /// <summary>
    /// 斜梁躯体用線分配置
    /// </summary>
    /// <param name="uidoc"></param>
    /// <returns></returns>
    public static bool CommandPutSyabariStructureLine( UIDocument uidoc )
    {
      Document doc = uidoc.Document ;

      return true ;
    }

    /// <summary>
    /// 斜梁端部部品個別配置
    /// </summary>
    /// <param name="uidoc"></param>
    /// <returns></returns>
    public static bool CommandPutSyabariPieceIndividual( UIDocument uidoc )
    {
      Document doc = uidoc.Document ;
      //斜張コマンドクラスが出来たら移植予定
      var dlg = new DLG.DlgCreateSyabariPiece() ;
      DialogResult result = dlg.ShowDialog() ;

      if ( result != DialogResult.OK ) {
        return false ;
      }

      string familyPath = dlg.GetFamilyPath1() ;
      string familyType = ClsRevitUtil.GetFamilyName( familyPath ) ;

      if ( ! ClsRevitUtil.LoadFamilySymbolData( doc, familyPath, familyType, out FamilySymbol sym ) ) {
        MessageBox.Show( "ファミリの取得に失敗しました" ) ;
        return false ;
      }

      if ( sym == null ) {
        MessageBox.Show( "ファミリの取得に失敗しました" ) ;
        return false ;
      }

      ElementType et = doc.GetElement( sym.Id ) as ElementType ;
      uidoc.PostRequestForElementTypePlacement( et ) ;

      return true ;
    }

    public static bool CommandPutSyabariPieceIntersectionPoint( UIDocument uidoc )
    {
      Document doc = uidoc.Document ;
      //斜張コマンドクラスが出来たら移植予定
      var dlg = new DLG.DlgCreateSyabariPiece( false ) ;
      DialogResult result = dlg.ShowDialog() ;

      if ( result != DialogResult.OK ) {
        return false ;
      }

      string familyPath1 = dlg.GetFamilyPath1() ;
      string familyType1 = ClsRevitUtil.GetFamilyName( familyPath1 ) ;
      if ( ! ClsRevitUtil.LoadFamilySymbolData( doc, familyPath1, familyType1, out FamilySymbol sym1 ) ) {
        MessageBox.Show( "ファミリの取得に失敗しました" ) ;
        return false ;
      }

      if ( sym1 == null ) {
        MessageBox.Show( "ファミリの取得に失敗しました" ) ;
        return false ;
      }

      string familyPath2 = dlg.GetFamilyPath2() ;
      string familyType2 = ClsRevitUtil.GetFamilyName( familyPath2 ) ;
      if ( ! ClsRevitUtil.LoadFamilySymbolData( doc, familyPath2, familyType2, out FamilySymbol sym2 ) ) {
        MessageBox.Show( "ファミリの取得に失敗しました" ) ;
        return false ;
      }

      if ( sym2 == null ) {
        MessageBox.Show( "ファミリの取得に失敗しました" ) ;
        return false ;
      }

      ElementId id1 = null ;
      if ( ! ClsHaraokoshiBase.PickBaseObject( uidoc, ref id1 ) )
        return false ;
      var inst1 = doc.GetElement( id1 ) as FamilyInstance ;
      var cv1 = ( inst1.Location as LocationCurve ).Curve ;
      var levelId1 = inst1.Host.Id ;

      ElementId id2 = null ;
      if ( ! ClsHaraokoshiBase.PickBaseObject( uidoc, ref id2 ) )
        return false ;
      var inst2 = doc.GetElement( id2 ) as FamilyInstance ;
      var cv2 = ( inst2.Location as LocationCurve ).Curve ;
      var levelId2 = inst2.Host.Id ;

      ElementId lineId = null ;
      if ( ! ClsRevitUtil.PickObject( uidoc, "モデル線分", "モデル線分", ref lineId ) )
        return false ;
      var line = ( doc.GetElement( lineId ).Location as LocationCurve ).Curve ;

      //Z軸が高い方を始点とする
      if ( cv1.GetEndPoint( 0 ).Z < cv2.GetEndPoint( 0 ).Z ) {
        var change = cv1 ;
        cv1 = cv2 ;
        cv2 = change ;
      }

      var insec1 = ClsRevitUtil.GetIntersectionZ0( cv1, line ) ;
      if ( insec1 != null ) {
        if ( sym1 != null ) {
          using ( Transaction t = new Transaction( doc, "Create Family" ) ) {
            t.Start() ;
            ElementId CreatedId = ClsRevitUtil.Create( doc, insec1, levelId1, sym1 ) ;
            t.Commit() ;
          }
        }
      }

      var insec2 = ClsRevitUtil.GetIntersectionZ0( cv2, line ) ;
      if ( insec2 != null ) {
        if ( sym2 != null ) {
          using ( Transaction t = new Transaction( doc, "Create Family" ) ) {
            t.Start() ;
            ElementId CreatedId = ClsRevitUtil.Create( doc, insec2, levelId2, sym2 ) ;
            t.Commit() ;
          }
        }
      }

      return true ;
    }

    public static bool CommandCreateSyabariBase( UIDocument uidoc )
    {
      Document doc = uidoc.Document ;
      List<string> firterList = new List<string> { ClsHaraokoshiBase.baseName, "モデル線分" } ;
      var mess = "腹起ベースorモデル線分を選択してください" ;

      var dlg = new DLG.DlgCreateSyabariBase() ;
      DialogResult result = dlg.ShowDialog() ;

      if ( result != DialogResult.OK ) {
        return false ;
      }

      var clsSyabari = dlg.m_ClsSyabariBase ;

      while ( true ) {
        try {
          Reference rfHara1 = null ;
          if ( ! ClsRevitUtil.PickObjectFilters( uidoc, "始点" + mess, firterList, ref rfHara1 ) )
            return false ;
          var id1 = rfHara1.ElementId ;
          var inst1 = doc.GetElement( id1 ) ;
          var cv1 = ( inst1.Location as LocationCurve ).Curve ;
          ElementId levelId = null ;
          if ( inst1 is CurveElement cvElement ) {
            //モデル線分の作業面はレベルではなく参照面になっているため名前からレベルに変更する
            levelId = ClsRevitUtil.GetLevelID( doc, cvElement.SketchPlane.Name ) ;
          }
          else {
            levelId = ( inst1 as FamilyInstance ).Host.Id ;
          }

          Reference rfHara2 = null ;
          if ( ! ClsRevitUtil.PickObjectFilters( uidoc, "終点" + mess, firterList, ref rfHara2 ) )
            return false ;
          var id2 = rfHara2.ElementId ;
          var inst2 = doc.GetElement( id2 ) ;
          var cv2 = ( inst2.Location as LocationCurve ).Curve ;

          if ( inst1.Name == ClsHaraokoshiBase.baseName && inst2.Name == ClsHaraokoshiBase.baseName ) {
            if ( levelId != ( inst2 as FamilyInstance ).Host.Id ) {
              //レベルの違う腹起ベースを選択した場合再度選択させる
              MessageBox.Show( "選択した腹起ベース同士のレベルが違います。" ) ;
              continue ;
            }
          }

          XYZ tmpStPoint ;
          XYZ tmpEdPoint ;
          bool create = false ;
          if ( clsSyabari.m_ShoriType == ClsSyabariBase.ShoriType.BaseLine ) {
            List<ElementId> lineIds = null ;
            if ( ! ClsRevitUtil.PickObjects( uidoc, "モデル線分", "モデル線分", ref lineIds ) )
              return false ;
            foreach ( var lineId in lineIds ) {
              var line = ( doc.GetElement( lineId ).Location as LocationCurve ).Curve ;

              tmpStPoint = ClsRevitUtil.GetIntersectionZ0( cv1, line ) ;
              tmpEdPoint = ClsRevitUtil.GetIntersectionZ0( cv2, line ) ;
              if ( tmpStPoint == null || tmpEdPoint == null )
                continue ;

              if ( ! clsSyabari.CheckKaitenPiece( cv1, cv2, tmpStPoint, tmpEdPoint ) )
                continue ;

              using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
                t.Start() ;
                ClsRevitUtil.Delete( doc, lineId ) ;
                t.Commit() ;
              }

              if ( ! clsSyabari.CreateSyabariBase( doc, tmpStPoint, tmpEdPoint, levelId ) )
                continue ;
              create = true ;
            }
          }
          else {
            //腹起ベースを選択している場合は選択した点を始点から100ピッチに補正する
            if ( inst1.Name == ClsHaraokoshiBase.baseName )
              tmpStPoint = ClsRevitUtil.CorrectedPitchPoint( rfHara1.GlobalPoint, cv1.GetEndPoint( 0 ), 100.0 ) ;
            else
              tmpStPoint = rfHara1.GlobalPoint ;
            if ( inst2.Name == ClsHaraokoshiBase.baseName )
              tmpEdPoint = ClsRevitUtil.CorrectedPitchPoint( rfHara2.GlobalPoint, cv2.GetEndPoint( 0 ), 100.0 ) ;
            else
              tmpEdPoint = rfHara2.GlobalPoint ;

            if ( ! clsSyabari.CheckKaitenPiece( cv1, cv2, tmpStPoint, tmpEdPoint ) )
              return false ;
            if ( ! clsSyabari.CreateSyabariBase( doc, tmpStPoint, tmpEdPoint, levelId ) )
              return false ;
            create = true ;
          }

          if ( inst2.Name.Contains( ClsHaraokoshiBase.baseName ) && create ) {
            using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
              t.Start() ;
              ClsRevitUtil.SetParameter( doc, id2, "ホストからのオフセット",
                -ClsRevitUtil.CovertToAPI( clsSyabari.m_endOffset1 ) ) ;
              t.Commit() ;
            }
          }
          else if ( create ) {
            //終点側モデル線分は作成した斜梁ベースの終点に接続するように位置を下げたものを作成する
            using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
              t.Start() ;
              //var createId = clsSyabari.m_ElementId;
              //var referencePlane = (doc.GetElement(createId) as FamilyInstance).Host as ReferencePlane;
              var p2S = cv2.GetEndPoint( 0 ) ;
              var p2E = cv2.GetEndPoint( 1 ) ;
              var offset = -ClsRevitUtil.CovertToAPI( clsSyabari.m_endOffset1 ) ;
              p2S = p2S + offset * XYZ.BasisZ ;
              p2E = p2E + offset * XYZ.BasisZ ;
              //掘削向きの変換は必要がないと思われる必要なパターンが出た場合は要調整
              var l = Line.CreateBound( p2S, p2E ) ;
              var linePlane = TempPlane( l ) ;
              var p = SketchPlane.Create( doc, linePlane ) ;
              doc.Create.NewModelCurve( l, p ) ;
              ClsRevitUtil.Delete( doc, id2 ) ;
              t.Commit() ;
            }
          }
        }
        catch {
          break ;
        }
      }

      return true ;
    }

    private static Plane TempPlane( Line line )
    {
      var p1 = line.GetEndPoint( 0 ) ;
      var p2 = line.GetEndPoint( 1 ) ;
      var a = ( p2 - p1 ).Normalize() ;
      var crossProd = a.CrossProduct( XYZ.BasisZ ) ;
      if ( ! ClsGeo.GEO_EQ0( crossProd.GetLength() ) ) {
        return Plane.CreateByOriginAndBasis( p1, a, crossProd.Normalize() ) ;
      }
      else {
        return Plane.CreateByNormalAndOrigin( XYZ.BasisX, p1 ) ;
      }
    }

    public static bool CommandChangeSyabariBase( UIDocument uidoc )
    {
      Document doc = uidoc.Document ;
      List<ElementId> ids = null ;
      if ( ! ClsSyabariBase.PickBaseObjects( uidoc, ref ids ) )
        return false ;
      if ( ids.Count < 1 )
        return false ;

      var clsSyabari = new ClsSyabariBase() ;
      //基本同じものを選択している想定。違う場合でも最終的には同じパラメータに直すので1つ目のデータを採用
      clsSyabari.SetClassParameter( doc, ids[ 0 ] ) ;

      var dlg = new DLG.DlgCreateSyabariBase( clsSyabari ) ;
      DialogResult result = dlg.ShowDialog() ;

      if ( result != DialogResult.OK ) {
        return false ;
      }

      clsSyabari = dlg.m_ClsSyabariBase ;

      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        t.Start() ;
        foreach ( var id in ids ) {
          clsSyabari.SetParameter( doc, id ) ;
        }

        t.Commit() ;
      }

      return true ;
    }
  }
}