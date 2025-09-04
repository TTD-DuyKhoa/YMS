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
  class ClsCommandCornerHiuchiBase
  {
    public static bool CommandCreateCornerHiuchiBase( UIDocument uidoc )
    {
      Application.thisApp.ShowForm_dlgCreateCornerHiuchi( uidoc ) ;
      return true ;
    }

    public static bool CommandChangeCornerHiuchiBase( UIDocument uidoc )
    {
      Document doc = uidoc.Document ;

      // 対象となるベースを複数選択
      List<ElementId> idList = null ;
      if ( ! ClsCornerHiuchiBase.PickBaseObjects( uidoc, ref idList ) ) {
        return false ;
      }

      ClsCornerHiuchiBase templateBase = new ClsCornerHiuchiBase() ;
      for ( int i = 0 ; i < idList.Count() ; i++ ) {
        ElementId id = idList[ i ] ;

        if ( i == 0 ) {
          templateBase.SetParameter( doc, id ) ;
        }
        else {
          FamilyInstance shinInstLevel = doc.GetElement( id ) as FamilyInstance ;
          ElementId levelID = shinInstLevel.Host.Id ;
          LocationCurve lCurve = shinInstLevel.Location as LocationCurve ;
          if ( lCurve == null ) {
            return false ;
          }

          var tmpStPoint = lCurve.Curve.GetEndPoint( 0 ) ;

          //元データを取得
          templateBase.m_Kousei = ClsRevitUtil.CompareValues(
            ClsRevitUtil.GetParameter( doc, id, "構成" ) == "シングル"
              ? ClsCornerHiuchiBase.Kousei.Single
              : ClsCornerHiuchiBase.Kousei.Double, templateBase.m_Kousei ) ;
          if ( templateBase.m_Kousei == ClsCornerHiuchiBase.Kousei.Double ) {
            templateBase.m_HiuchiLengthDoubleUpperL1 = ClsRevitUtil.CompareValues(
              ClsRevitUtil.CovertFromAPI( ClsRevitUtil.GetParameterDouble( doc, id, "火打長さ(ダブル上)L1" ) ),
              templateBase.m_HiuchiLengthDoubleUpperL1 ) ;
            templateBase.m_HiuchiLengthDoubleUnderL2 = ClsRevitUtil.CompareValues(
              ClsRevitUtil.CovertFromAPI( ClsRevitUtil.GetParameterDouble( doc, id, "火打長さ(ダブル下)L2" ) ),
              templateBase.m_HiuchiLengthDoubleUnderL2 ) ;
            templateBase.m_HiuchiZureryo = ClsRevitUtil.CompareValues(
              ClsRevitUtil.CovertFromAPI( ClsRevitUtil.GetParameterDouble( doc, id, "上下火打ずれ量a" ) ),
              templateBase.m_HiuchiZureryo ) ;
          }
          else {
            templateBase.m_HiuchiLengthSingleL =
              ClsRevitUtil.CompareValues( ClsRevitUtil.CovertFromAPI( lCurve.Curve.Length ),
                templateBase.m_HiuchiLengthSingleL ) ;
          }

          templateBase.m_SteelType = ClsRevitUtil.CompareValues( ClsRevitUtil.GetParameter( doc, id, "鋼材タイプ" ),
            templateBase.m_SteelType ) ;
          templateBase.m_SteelSize = ClsRevitUtil.CompareValues( ClsRevitUtil.GetParameter( doc, id, "鋼材サイズ" ),
            templateBase.m_SteelSize ) ;
          templateBase.m_TanbuParts1 = ClsRevitUtil.CompareValues( ClsRevitUtil.GetParameter( doc, id, "端部部品(1)" ),
            templateBase.m_TanbuParts1 ) ;
          templateBase.m_HiuchiUkePieceSize1 =
            ClsRevitUtil.CompareValues( ClsRevitUtil.GetParameter( doc, id, "火打受ピースサイズ(1)" ),
              templateBase.m_HiuchiUkePieceSize1 ) ;
          templateBase.m_TanbuParts2 = ClsRevitUtil.CompareValues( ClsRevitUtil.GetParameter( doc, id, "端部部品(2)" ),
            templateBase.m_TanbuParts2 ) ;
          templateBase.m_HiuchiUkePieceSize2 =
            ClsRevitUtil.CompareValues( ClsRevitUtil.GetParameter( doc, id, "火打受ピースサイズ(2)" ),
              templateBase.m_HiuchiUkePieceSize2 ) ;

          //交差する腹起ベースを見つける
          List<ElementId> haraIdList = ClsHaraokoshiBase.GetAllHaraokoshiBaseList( doc ) ;
          List<ElementId> insecList = new List<ElementId>() ;
          foreach ( ElementId haraId in haraIdList ) {
            FamilyInstance haraInst = doc.GetElement( haraId ) as FamilyInstance ;
            LocationCurve lCurveHara = haraInst.Location as LocationCurve ;
            if ( lCurveHara != null ) {
              XYZ insec = ClsRevitUtil.GetIntersection( lCurve.Curve, lCurveHara.Curve ) ; //交点が2つ見つかる
              if ( insec != null ) {
                if ( ClsGeo.GEO_EQ( insec, tmpStPoint ) ) {
                  templateBase.m_HaraokoshiBaseChangeID1 = haraId ;
                }
                else {
                  templateBase.m_HaraokoshiBaseChangeID2 = haraId ;
                }
                //insecList.Add(haraId);
                //if (insecList.Count == 2)
                //{
                //    break;
                //}
              }
            }
          }
          //templateBase.m_HaraokoshiBaseChangeID1 = ClsRevitUtil.CompareValues(insecList[0], templateBase.m_HaraokoshiBaseChangeID1);
          //templateBase.m_HaraokoshiBaseChangeID2 = ClsRevitUtil.CompareValues(insecList[1], templateBase.m_HaraokoshiBaseChangeID2);
        }
      }

      DLG.DlgCreateCornerHiuchiBase cornerHiuchiBase = new DLG.DlgCreateCornerHiuchiBase( templateBase ) ;
      DialogResult result = cornerHiuchiBase.ShowDialog() ;
      if ( result != DialogResult.OK ) {
        return false ;
      }

      foreach ( var id in idList ) {
        ClsCornerHiuchiBase clsCHB = cornerHiuchiBase.m_ClsCornerHiuchiBase ;

        //// 始点と終点を取得
        FamilyInstance shinInstLevel = doc.GetElement( id ) as FamilyInstance ;
        ElementId levelID = shinInstLevel.Host.Id ;
        LocationCurve lCurve = shinInstLevel.Location as LocationCurve ;
        if ( lCurve == null ) {
          continue ;
        }

        XYZ tmpStPoint = lCurve.Curve.GetEndPoint( 0 ) ;
        //XYZ tmpEdPoint = lCurve.Curve.GetEndPoint(1);

        //// 元のベースから一部の情報を引き継ぐ
        //ClsCornerHiuchiBase tmp = new ClsCornerHiuchiBase();
        //tmp.SetParameter(doc, id);
        //clsCornerHiuchiBase.m_ShoriType = tmp.m_ShoriType;
        //clsCornerHiuchiBase.m_HiuchiAngle = tmp.m_HiuchiAngle;
        //clsCornerHiuchiBase.m_angle = tmp.m_angle;
        //clsCornerHiuchiBase.m_HiuchiTotalLength = tmp.m_HiuchiTotalLength;

        //string dan = ClsRevitUtil.GetParameter(doc, id, "段");
        //// 元のベースを削除
        //using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
        //{
        //    t.Start();
        //    ClsRevitUtil.Delete(doc, id);
        //    t.Commit();
        //}

        //// ベースを再作成
        //clsCornerHiuchiBase.CreateCornerHiuchi(doc, tmpStPoint, tmpEdPoint, levelID, dan);

        clsCHB.m_cornerHiuchiID = id ;
        //変更する隅火打ベースに接している腹起ベースを2本取得する
        //交差する腹起ベースを見つける
        List<ElementId> haraIdList = ClsHaraokoshiBase.GetAllHaraokoshiBaseList( doc ) ;
        foreach ( ElementId haraId in haraIdList ) {
          FamilyInstance haraInst = doc.GetElement( haraId ) as FamilyInstance ;
          LocationCurve lCurveHara = haraInst.Location as LocationCurve ;
          if ( lCurveHara != null ) {
            XYZ insec = ClsRevitUtil.GetIntersection( lCurve.Curve, lCurveHara.Curve ) ; //交点が2つ見つかる
            if ( insec != null ) {
              if ( ClsGeo.GEO_EQ( insec, tmpStPoint ) ) {
                clsCHB.m_HaraokoshiBaseID1 = haraId ;
              }
              else {
                clsCHB.m_HaraokoshiBaseID2 = haraId ;
              }
            }
          }
        }

        //(clsCHB.m_HaraokoshiBaseID1, clsCHB.m_HaraokoshiBaseID2) = clsCHB.GetInsecHaraokoshiBase(doc, id);
        if ( clsCHB.m_HaraokoshiBaseID1 == null || clsCHB.m_HaraokoshiBaseID2 == null ) {
          continue ;
        }

        //FamilyInstance shinInstLevel = doc.GetElement(id) as FamilyInstance;
        //ElementId levelID = shinInstLevel.Host.Id;
        string dan = ClsRevitUtil.GetParameter( doc, id, "段" ) ; //腹起ベースの変更に伴う段の確認は保留clsCHB.m_HaraokoshiBaseID1
        //元のベースを削除
        using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
          t.Start() ;
          ClsRevitUtil.Delete( doc, id ) ;
          t.Commit() ;
        }

        //ベース作成
        XYZ pntS = new XYZ() ;
        XYZ pntE = new XYZ() ;
        if ( clsCHB.GetBaseStartEndPoint( doc, ref pntS, ref pntE ) ) {
          clsCHB.CreateCornerHiuchi( doc, pntS, pntE, levelID, dan ) ;
        }
      }

      return true ;
    }

    public static bool CreateCornerHiuchiBase( UIDocument uidoc )
    {
      Document doc = uidoc.Document ;
      ClsCornerHiuchiBase cornerHiuchi = ClsCornerHiuchiBase.m_cornerHiuchi ;

      switch ( cornerHiuchi.m_cornerHiuchiId ) {
        case CornerHiuchiId.OK :
        {
          cornerHiuchi.CreateCornerHiuchi( uidoc ) ;
          break ;
        }
        case CornerHiuchiId.review :
        {
          cornerHiuchi.CreateCornerHiuchiCheck( uidoc ) ;
          cornerHiuchi.UpdateTotalLength() ;
          break ;
        }
        case CornerHiuchiId.reset :
        {
          cornerHiuchi.DeleteCornerHiuchiBaseReview( doc ) ;
          break ;
        }
        case CornerHiuchiId.cancel :
        {
          cornerHiuchi.DeleteCornerHiuchiBaseReview( doc ) ;
          cornerHiuchi.CloseDlg() ;
          break ;
        }
      }

      return true ;
    }

    public static ClsCornerHiuchiBase ChangeCornerHiuchiBase( UIDocument uidoc )
    {
      Document doc = uidoc.Document ;
      ClsCornerHiuchiBase clsCHB = new ClsCornerHiuchiBase() ;

      ElementId id = null ;
      if ( ! ClsCornerHiuchiBase.PickBaseObject( uidoc, ref id ) ) {
        return clsCHB ;
      }

      FamilyInstance shinInstLevel = doc.GetElement( id ) as FamilyInstance ;
      LocationCurve lCurve = shinInstLevel.Location as LocationCurve ;
      if ( lCurve == null ) {
        return clsCHB ;
      }

      var tmpStPoint = lCurve.Curve.GetEndPoint( 0 ) ;

      //元データを取得
      clsCHB.m_Kousei = ClsRevitUtil.GetParameter( doc, id, "構成" ) == "シングル"
        ? ClsCornerHiuchiBase.Kousei.Single
        : ClsCornerHiuchiBase.Kousei.Double ;
      if ( clsCHB.m_Kousei == ClsCornerHiuchiBase.Kousei.Double ) {
        clsCHB.m_HiuchiLengthDoubleUpperL1 =
          ClsRevitUtil.CovertFromAPI( ClsRevitUtil.GetParameterDouble( doc, id, "火打長さ(ダブル上)L1" ) ) ;
        clsCHB.m_HiuchiLengthDoubleUnderL2 =
          ClsRevitUtil.CovertFromAPI( ClsRevitUtil.GetParameterDouble( doc, id, "火打長さ(ダブル下)L2" ) ) ;
        clsCHB.m_HiuchiZureryo = ClsRevitUtil.CovertFromAPI( ClsRevitUtil.GetParameterDouble( doc, id, "上下火打ずれ量a" ) ) ;
      }
      else {
        clsCHB.m_HiuchiLengthSingleL =
          ClsRevitUtil.CovertFromAPI( ClsRevitUtil.GetParameterDouble( doc, id, "火打長さ(シングル)L" ) ) ;
      }

      clsCHB.m_angle = ClsRevitUtil.GetParameterDouble( doc, id, "配置角度" ) / Math.PI * 180 ;
      clsCHB.m_SteelType = ClsRevitUtil.GetParameter( doc, id, "鋼材タイプ" ) ;
      clsCHB.m_SteelSize = ClsRevitUtil.GetParameter( doc, id, "鋼材サイズ" ) ;
      clsCHB.m_TanbuParts1 = ClsRevitUtil.GetParameter( doc, id, "端部部品(1)" ) ;
      clsCHB.m_HiuchiUkePieceSize1 = ClsRevitUtil.GetParameter( doc, id, "火打受ピースサイズ(1)" ) ;
      clsCHB.m_TanbuParts2 = ClsRevitUtil.GetParameter( doc, id, "端部部品(2)" ) ;
      clsCHB.m_HiuchiUkePieceSize2 = ClsRevitUtil.GetParameter( doc, id, "火打受ピースサイズ(2)" ) ;

      clsCHB.m_ShoriType = ClsCornerHiuchiBase.ShoriType.Change ;
      clsCHB.m_cornerHiuchiID = id ;

      //交差する腹起ベースを見つける
      List<ElementId> haraIdList = ClsHaraokoshiBase.GetAllHaraokoshiBaseList( doc ) ;
      List<ElementId> insecList = new List<ElementId>() ;
      foreach ( ElementId haraId in haraIdList ) {
        FamilyInstance haraInst = doc.GetElement( haraId ) as FamilyInstance ;
        LocationCurve lCurveHara = haraInst.Location as LocationCurve ;
        if ( lCurveHara != null ) {
          XYZ insec = ClsRevitUtil.GetIntersection( lCurve.Curve, lCurveHara.Curve ) ; //交点が2つ見つかる
          if ( insec != null ) {
            if ( ClsGeo.GEO_EQ( insec, tmpStPoint ) ) {
              clsCHB.m_HaraokoshiBaseChangeID1 = haraId ;
            }
            else {
              clsCHB.m_HaraokoshiBaseChangeID2 = haraId ;
            }
            //insecList.Add(haraId);
            //if (insecList.Count == 2)
            //{
            //    break;
            //}
          }
        }
      }
      //clsCHB.m_HaraokoshiBaseChangeID1 = insecList[0];
      //clsCHB.m_HaraokoshiBaseChangeID2 = insecList[1];


      return clsCHB ;
    }
  }
}