using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Text.RegularExpressions ;
using System.Threading.Tasks ;
using YMS.Master ;
using static Autodesk.Revit.DB.SpecTypeId ;
using static YMS.Parts.ClsHaraokoshiSuberidome ;

namespace YMS.Parts
{
  public class ClsShimetukePiece
  {
    /// <summary>
    /// 部材サイズ
    /// </summary>
    public string m_buzaiSize { get ; set ; }

    public bool CreateShimetukePiece( Document doc, ElementId bracketId, ElementId kiribariId )
    {
      // 切梁のサイズ取得処理
      FamilyInstance instKiribari = doc.GetElement( kiribariId ) as FamilyInstance ;
      string skiribariSize = ClsYMSUtil.GetSyuzaiSize( instKiribari.Symbol.FamilyName ) ;
      double dkiribariSize = ClsRevitUtil.CovertToAPI( Master.ClsYamadomeCsv.GetWidth( skiribariSize ) / 2 ) ;
      //仮鋼材or割付主材でない場合終了
      if ( skiribariSize == string.Empty )
        return false ;

      // ファミリの取得処理
      string symbolFolpath = ClsZumenInfo.GetYMSFolder() ;
      string familyPath = Master.ClsShimetukePieceCsv.GetFamilyPath( skiribariSize ) ;
      string tmp = familyPath.Replace( ".rfa", "" ) ;
      string[] parts = tmp.Split( '\\' ) ;
      string extractedName = parts[ parts.Length - 1 ] ;
      string typeName = extractedName ;
      string familyName = ClsRevitUtil.GetFamilyName( familyPath ) ;

      if ( ! ClsRevitUtil.LoadFamilyData( doc, familyPath, out Family family ) ) {
        return false ;
      }

      FamilySymbol sym = ( ClsRevitUtil.GetFamilySymbol( doc, familyName, typeName ) ) ;

      // 作図位置の算出処理
      XYZ ptSt = XYZ.Zero ;
      XYZ ptEd = XYZ.Zero ;
      XYZ ptInsert = XYZ.Zero ;
      FamilyInstance instTarget = doc.GetElement( bracketId ) as FamilyInstance ;
      if ( instTarget.Symbol.Name == "切梁BL" ) {
        // 切梁ブラケットの線分
        ptSt = ( instTarget.Location as LocationPoint ).Point ;
        XYZ defaultFacing = -instTarget.FacingOrientation ;
        string bracketSize = instTarget.Symbol.Family.Name.Replace( "ﾌﾞﾗｹｯﾄ_", "" ) ;
        double bracketLength = ClsRevitUtil.CovertToAPI( ClsBracketCsv.GetLength( "切梁ブラケット", bracketSize ) ) ;
        ptEd = ptSt + ( defaultFacing * bracketLength ) ;
      }
      else if ( instTarget.Symbol.Name == "切梁受け材" ) {
        // 切梁受け材の線分
        LocationCurve lCurve = instTarget.Location as LocationCurve ;
        ptSt = lCurve.Curve.GetEndPoint( 0 ) ;
        ptEd = lCurve.Curve.GetEndPoint( 1 ) ;
      }

      ptSt = new XYZ( ptSt.X, ptSt.Y, 0 ) ;
      ptEd = new XYZ( ptEd.X, ptEd.Y, 0 ) ;

      // 切梁の線分
      XYZ ptKiribariSt = XYZ.Zero ;
      XYZ ptKiribariEd = XYZ.Zero ;
      LocationPoint location = instKiribari.Location as LocationPoint ;
      if ( location != null ) {
        XYZ position = location.Point ;
        XYZ facingOrientation = instKiribari.FacingOrientation ;
        double kiribariLength =
          ClsRevitUtil.CovertToAPI( ClsYMSUtil.GetSyuzaiLength( instKiribari.Symbol.Family.Name ) ) ;
        if ( kiribariLength == 0 ) {
          return false ;
        }

        ptKiribariSt = position ;
        // 90度の回転行列を生成します
        Transform rotation = Transform.CreateRotationAtPoint( XYZ.BasisZ, -Math.PI / 2, position ) ;
        facingOrientation = rotation.OfVector( facingOrientation ) ;
        ptKiribariEd = position + ( kiribariLength * facingOrientation ) ;
        ptKiribariSt = new XYZ( ptKiribariSt.X, ptKiribariSt.Y, 0 ) ;
        ptKiribariEd = new XYZ( ptKiribariEd.X, ptKiribariEd.Y, 0 ) ;
      }
      else {
        Curve cvBase = ( instKiribari.Location as LocationCurve ).Curve ;
        XYZ startPoint = cvBase.GetEndPoint( 0 ) ;
        XYZ endPoint = cvBase.GetEndPoint( 1 ) ;

        // Z軸を0に設定
        ptKiribariSt = new XYZ( startPoint.X, startPoint.Y, 0 ) ;
        ptKiribariEd = new XYZ( endPoint.X, endPoint.Y, 0 ) ;
      }

      // 直線を作成
      Line lineKiribari = Line.CreateBound( ptKiribariSt, ptKiribariEd ) ;
      Curve curveKiribari = lineKiribari as Curve ;
      Line lineKiribariUke = Line.CreateBound( ptSt, ptEd ) ;
      Curve curveKiribariUke = lineKiribariUke as Curve ;

      //CreateDebugLine(doc, ptInsert, curveKiribari);
      //CreateDebugLine(doc, ptInsert, curveKiribariUke);

      // 直線と直線の交点を計算
      IntersectionResultArray results ;
      curveKiribariUke.Intersect( curveKiribari, out results ) ;

      // 交点があるか確認
      if ( results != null && results.Size > 0 ) {
        IntersectionResult intersectionResult = results.get_Item( 0 ) ;
        ptInsert = intersectionResult.XYZPoint ;
      }
      else {
        System.Windows.Forms.MessageBox.Show( "交点が見つかりません" ) ;
        return false ;
      }

      double offset = 0.0 ;
      double kijyun = ClsRevitUtil.GetParameterDouble( doc, kiribariId, "基準レベルからの高さ" ) ;
      if ( kijyun != 0.0 )
        offset = kijyun ;
      else
        offset = ClsRevitUtil.GetParameterDouble( doc, kiribariId, "ホストからのオフセット" ) ;

      // 作図処理
      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        t.Start() ;

        ElementId createdId = ClsRevitUtil.Create( doc, ptInsert, instTarget.LevelId, sym ) ;
        FamilyInstance instPiece = doc.GetElement( createdId ) as FamilyInstance ;

        double angle = GetRotateAngle( ptKiribariSt, ptKiribariEd, instPiece ) ;
        ElementTransformUtils.RotateElement( doc, createdId, Line.CreateBound( ptInsert, ptInsert + XYZ.BasisZ ),
          angle ) ;

        ClsRevitUtil.SetParameter( doc, createdId, "基準レベルからの高さ", offset + dkiribariSize ) ;

        t.Commit() ;
      }

      return true ;
    }

    public static bool PickObject( UIDocument uidoc, ref ElementId id, string message = "" )
    {
      message = "切梁ブラケットまたは切梁受材" ;
      List<string> filterList = new List<string>() ;
      filterList.Add( "切梁BL" ) ;
      filterList.Add( "切梁受け材" ) ;
      return ClsRevitUtil.PickObjectPartListFilter( uidoc, message + "を選択してください", filterList, ref id ) ;
    }

    private static double GetRotateAngle( XYZ pkui, XYZ insertPointSt, FamilyInstance inst, bool isReverse = false )
    {
      // 基準点と挿入点
      XYZ basePoint = pkui ; // 基準点の座標

      // 基準点から挿入点へのベクトルを取得
      XYZ direction = basePoint - insertPointSt ;
      if ( isReverse ) {
        direction = insertPointSt - basePoint ;
      }

      // ベクトルをZ軸に対応する方向に変換（プロジェクト平面への変換）
      XYZ projectedDirection = new XYZ( direction.X, direction.Y, 0 ).Normalize() ;

      // デフォルトの向きを取得
      XYZ defaultFacing = inst.FacingOrientation ;

      // Z軸と計算した方向ベクトルのなす角を求める
      double angle = defaultFacing.AngleTo( projectedDirection ) ;
      if ( ClsGeo.IsLeft( basePoint, insertPointSt ) ) {
        angle = -angle ;
      }

      return angle ;
    }


    private static void CreateDebugLine( Document doc, XYZ pt, Curve line )
    {
      // デバッグ用の線分を表示
      using ( Transaction transaction = new Transaction( doc, "Create Model Line" ) ) {
        transaction.Start() ;

        // モデル線分を作成
        ElementId levelID = ClsRevitUtil.GetLevelID( doc, ClsKabeShin.GL ) ;
        Plane plane = Plane.CreateByNormalAndOrigin( XYZ.BasisZ, pt ) ;
        SketchPlane sketchPlane = SketchPlane.Create( doc, plane ) ;
        ModelCurve modelLineF = doc.Create.NewModelCurve( line, sketchPlane ) ;

        transaction.Commit() ;
      }
    }
  }
}