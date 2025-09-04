using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using YMS.Parts ;

namespace YMS.Command
{
  public class ClsCommandKousyabuMawari
  {
    public static bool CreateKousyabuMawari( UIApplication uiapp )
    {
      UIDocument uidoc = uiapp.ActiveUIDocument ;
      Document doc = uidoc.Document ;
      // 対象の切梁を選択ジャッキ補助ピースが選択されてしまう
      List<ElementId> idList = null ;
      if ( ! ClsRevitUtil.PickObjects( uidoc, "交叉部ユニットを配置する切梁", "切梁", ref idList ) ) {
        return false ;
      }

      for ( int i = 0 ; i < idList.Count ; i++ ) {
        ElementId id = idList[ i ] ;
        double offset = 0.0 ;
        double kijyun = ClsRevitUtil.GetParameterDouble( doc, id, "基準レベルからの高さ" ) ;
        if ( kijyun != 0.0 )
          offset = kijyun ;
        else
          offset = ClsRevitUtil.GetParameterDouble( doc, id, "ホストからのオフセット" ) ;

        if ( offset <= 0.0 ) //上段配置切梁のみ使用
          continue ;

        FamilyInstance inst = doc.GetElement( id ) as FamilyInstance ;
        LocationCurve lCurve = inst.Location as LocationCurve ;
        Curve cv ;
        ElementId levelId = null ;
        if ( lCurve != null ) //仮鋼材
        {
          XYZ tmpStPoint = lCurve.Curve.GetEndPoint( 0 ) ;
          XYZ tmpEdPoint = lCurve.Curve.GetEndPoint( 1 ) ;
          tmpStPoint = new XYZ( tmpStPoint.X, tmpStPoint.Y, 0.0 ) ;
          tmpEdPoint = new XYZ( tmpEdPoint.X, tmpEdPoint.Y, 0.0 ) ;
          cv = Line.CreateBound( tmpStPoint, tmpEdPoint ) ;
          levelId = inst.Host.Id ;
        }
        else //割付主材
        {
          LocationPoint lPoint = inst.Location as LocationPoint ;
          if ( lPoint == null )
            continue ;
          else {
            XYZ point = new XYZ( lPoint.Point.X, lPoint.Point.Y, 0.0 ) ;
            string length = string.Empty ;
            string[] kouzaiNameUnder = inst.Symbol.FamilyName.Split( '_' ) ;
            if ( kouzaiNameUnder.Count() != 3 )
              continue ;
            else {
              length = kouzaiNameUnder[ 2 ] ;
            }

            double dLength = ClsCommonUtils.ChangeStrToDbl( length ) * 1000.0 ;
            XYZ endPoint = point + ClsRevitUtil.CovertToAPI( dLength ) * inst.HandOrientation ;
            cv = Line.CreateBound( point, endPoint ) ;
            levelId = ClsRevitUtil.GetParameterElementId( doc, id, "集計レベル" ) ;
          }
        }

        //ElementId levelId = inst.Host.Id;
        string kiriSize = ClsYMSUtil.GetSyuzaiSize( inst.Symbol.FamilyName ) ;
        double angle = inst.HandOrientation.AngleOnPlaneTo( XYZ.BasisY, XYZ.BasisZ ) ;

        for ( int j = 0 ; j < idList.Count ; j++ ) {
          if ( i == j )
            continue ;
          ElementId id2 = idList[ j ] ;
          //double offset2 = ClsRevitUtil.GetParameterDouble(doc, id2, "ホストからのオフセット");
          //if (offset2 >= 0.0)//下段段配置切梁のみ使用
          //    continue;
          kijyun = ClsRevitUtil.GetParameterDouble( doc, id2, "基準レベルからの高さ" ) ;
          if ( kijyun != 0.0 )
            offset = kijyun ;
          else
            offset = ClsRevitUtil.GetParameterDouble( doc, id2, "ホストからのオフセット" ) ;

          if ( offset >= 0.0 ) //下段段配置切梁のみ使用
            continue ;

          FamilyInstance inst2 = doc.GetElement( id2 ) as FamilyInstance ;
          LocationCurve lCurve2 = inst2.Location as LocationCurve ;
          Curve cv2 ;
          ElementId levelId2 = null ;
          if ( lCurve2 != null ) //仮鋼材
          {
            XYZ tmpStPoint = lCurve2.Curve.GetEndPoint( 0 ) ;
            XYZ tmpEdPoint = lCurve2.Curve.GetEndPoint( 1 ) ;
            tmpStPoint = new XYZ( tmpStPoint.X, tmpStPoint.Y, 0.0 ) ;
            tmpEdPoint = new XYZ( tmpEdPoint.X, tmpEdPoint.Y, 0.0 ) ;
            cv2 = Line.CreateBound( tmpStPoint, tmpEdPoint ) ;
            levelId2 = inst2.Host.Id ;
          }
          else //割付主材
          {
            LocationPoint lPoint = inst2.Location as LocationPoint ;
            if ( lPoint == null )
              continue ;
            else {
              XYZ point = new XYZ( lPoint.Point.X, lPoint.Point.Y, 0.0 ) ;
              string length = string.Empty ;
              string[] kouzaiNameUnder = inst2.Symbol.FamilyName.Split( '_' ) ;
              if ( kouzaiNameUnder.Count() != 3 )
                continue ;
              else {
                length = kouzaiNameUnder[ 2 ] ;
              }

              double dLength = ClsCommonUtils.ChangeStrToDbl( length ) * 1000.0 ;
              XYZ endPoint = point + ClsRevitUtil.CovertToAPI( dLength ) * inst2.HandOrientation ;
              cv2 = Line.CreateBound( point, endPoint ) ;
              levelId2 = levelId = ClsRevitUtil.GetParameterElementId( doc, id2, "集計レベル" ) ;
            }
          }

          string syuzaiSize2 = ClsYMSUtil.GetSyuzaiSize( inst2.Symbol.FamilyName ) ;

          XYZ insec = ClsRevitUtil.GetIntersection( cv, cv2 ) ;
          if ( insec != null && levelId == levelId2 ) {
            if ( ClsKousyabuMawari.CheckSyuzaiSize( doc, insec, levelId, out string haraSize ) ) {
              ClsKousyabuMawari.CreateKousyabuMawari( doc, levelId, haraSize, kiriSize, insec, angle ) ;
            }
          }
        }
      }

      return true ;
    }

    public static bool CreateKousyabuMawariSelectBase( UIApplication uiapp )
    {
      //UIDocument uidoc = uiapp.ActiveUIDocument;
      //Document doc = uidoc.Document;
      //List<ElementId> idList = null;
      //if (!ClsKiribariBase.PickBaseObjects(uidoc, ref idList))
      //{
      //    return false;
      //}

      //for (int i = 0; i < idList.Count; i++)
      //{
      //    ElementId id = idList[i];
      //    string dan = ClsRevitUtil.GetParameter(doc, id, "段");
      //    if (dan != "上段")
      //        continue;
      //    string syuzaiSize = ClsRevitUtil.GetParameter(doc, id, "鋼材サイズ");

      //    double offset = ClsRevitUtil.CovertToAPI(Master.ClsYamadomeCsv.GetWidth(syuzaiSize) / 2);

      //    FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
      //    LocationCurve lCurve = inst.Location as LocationCurve;
      //    if (lCurve == null)//ジャッキなどPointのものを弾く
      //        continue;
      //    Curve cv = lCurve.Curve;
      //    //ElementId levelId = inst.Host.Id;

      //    double angle = inst.HandOrientation.AngleOnPlaneTo(XYZ.BasisY, XYZ.BasisZ);

      //    for (int j = 0; j < idList.Count; j++)
      //    {
      //        if (i == j)
      //            continue;
      //        ElementId id2 = idList[j];
      //        string dan2 = ClsRevitUtil.GetParameter(doc, id2, "段");
      //        if (dan2 != "下段")
      //            continue;
      //        string syuzaiSize2 = ClsRevitUtil.GetParameter(doc, id2, "鋼材サイズ");

      //        double offset2 = Master.ClsYamadomeCsv.GetWidth(syuzaiSize2) / 2;

      //        FamilyInstance inst2 = doc.GetElement(id2) as FamilyInstance;
      //        LocationCurve lCurve2 = inst2.Location as LocationCurve;
      //        if (lCurve2 == null)//ジャッキなどPointのものを弾く
      //            continue;
      //        Curve cv2 = lCurve2.Curve;

      //        XYZ insec = ClsRevitUtil.GetIntersection(cv, cv2);
      //        if (insec != null)
      //        {
      //            if (ClsKousyabuMawari.CheckSyuzaiSize(doc, insec, out ElementId levelId, out string haraSyuzai))
      //            {
      //                if (syuzaiSize != haraSyuzai)
      //                {
      //                    syuzaiSize = "なし";
      //                }
      //                ClsKousyabuMawari.CreateKousyabuMawari(doc, levelId, syuzaiSize, offset, insec, angle);
      //            }
      //        }
      //    }
      //}
      return true ;
    }
  }
}