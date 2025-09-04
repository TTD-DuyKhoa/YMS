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

namespace YMS
{
  class ClsVoid
  {
    #region 定数

    const string Void = "掘削用ボイド" ;
    public const string InVoidLine = "掘削側補助線" ;
    public const string OutVoidLine = "壁側補助線" ;

    #endregion

    /// <summary>
    /// 選択された壁を補助線を基準にボイドする
    /// </summary>
    /// <param name="doc">ドキュメント</param>
    /// <param name="selId">選択された壁</param>
    /// <param name="line">補助線</param>
    /// <param name="dep">掘削深さ</param>
    /// <param name="haba">ボイド幅</param>
    /// <returns></returns>
    public static bool DoVoid( Document doc, List<ElementId> selId, Element line, double dep, double haba,
      ElementId levelId )
    {
      //ソイル位置作成
      Curve cv = ( line.Location as LocationCurve ).Curve ;
      XYZ lineOrigin = cv.GetEndPoint( 0 ) ;
      XYZ lineEndPoint = cv.GetEndPoint( 1 ) ;
      XYZ Direction = Line.CreateBound( lineOrigin, lineEndPoint ).Direction ;
      XYZ soilDirection = lineEndPoint - lineOrigin ;
      double wallLength = soilDirection.GetLength() ;
      XYZ p = new XYZ( ( lineOrigin.X + lineEndPoint.X ) / 2.0, ( lineOrigin.Y + lineEndPoint.Y ) / 2.0,
        ( lineOrigin.Z + lineEndPoint.Z ) / 2.0 ) ;

      //掘削シンボル作成
      string symbolFolpath = ClsZumenInfo.GetYMSFolder() ;
      string voidfamily = System.IO.Path.Combine( symbolFolpath, "ベース関係\\" + Void + ".rfa" ) ;
      if ( ! ClsRevitUtil.LoadFamilySymbolData( doc, voidfamily, Void, out FamilySymbol sym ) ) {
        return false ;
      }

      //選択した1つ目を使用する（全て同一の壁が選択されているものとする）
      Element kabeElem = doc.GetElement( selId[ 0 ] ) ;
      //選択したシンボル名からどの壁か判別し深さを取得する
      double voidDip = dep ;
      if ( voidDip <= 0.0 ) return false ;

      //ビューの向き//ビューがどの向きを向いていようが作成方向は同じであるため固定値
      XYZ vecView = new XYZ( 0, 1, 0 ) ;

      //角度//X軸がプラスの時回転しすぎてしまうので分岐が必要
      double dAngle = Direction.AngleTo( vecView ) ;
      if ( lineOrigin.X < lineEndPoint.X ) {
        dAngle = -dAngle ;
      }

      XYZ vec = new XYZ( 0, 0, 1 ) ; //部材の法線ベクトルはZ軸を基準にしている
      Line axis = Line.CreateBound( p, p + vec ) ;

      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        try {
          t.Start() ;
          ElementId CreatedID = ClsRevitUtil.Create( doc, p, levelId, sym ) ;

          //ボイドを回転
          ClsRevitUtil.RotateElement( doc, CreatedID, axis, -Math.PI / 2 + dAngle ) ;
          //パラメーターの設定処理
          ClsRevitUtil.SetParameter( doc, CreatedID, ClsGlobal.m_length, wallLength ) ;
          ClsRevitUtil.SetParameter( doc, CreatedID, "幅", ClsRevitUtil.CovertToAPI( haba ) ) ;
          ClsRevitUtil.SetParameter( doc, CreatedID, "掘削深さ1", ClsRevitUtil.CovertToAPI( voidDip ) ) ;
          ClsRevitUtil.SetParameter( doc, CreatedID, "掘削深さ2", ClsRevitUtil.CovertToAPI( voidDip ) ) ;

          //切り取り処理
          for ( int i = 0 ; i < selId.Count ; i++ ) {
            Element hostElem = doc.GetElement( selId[ i ] ) ;
            Element cutElem = doc.GetElement( CreatedID ) ;

            InstanceVoidCutUtils.AddInstanceVoidCut( doc, hostElem, cutElem ) ;
          }

          doc.Delete( line.Id ) ;
          t.Commit() ;
        }
        catch ( OperationCanceledException e ) {
        }
        catch ( Exception e ) {
          MessageBox.Show( e.Message ) ;
        }
      }

      return true ;
    }

    /// <summary>
    /// 対象のファミリに掘削深さをカスタムデータとして持たせる
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="id">ファミリ</param>
    /// <param name="dep">掘削深さ</param>
    /// <returns></returns>
    public static bool SetVoidDep( Document doc, ElementId id, double dep )
    {
      string sDep = dep.ToString() ;
      return ClsRevitUtil.CustomDataSet<string>( doc, id, Void, sDep ) ;
    }

    /// <summary>
    /// 対象のファミリから掘削深さのカスタムデータを取得する
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="id">ファミリ</param>
    /// <returns>掘削深さ</returns>
    public static double GetVoidDep( Document doc, ElementId id )
    {
      ClsRevitUtil.CustomDataGet<string>( doc, id, Void, out string sDep ) ;

      double dep = ClsCommonUtils.ChangeStrToInt( sDep ) ;
      return dep ;
    }

    /// <summary>
    /// 対象のファミリに掘削補助線かどうかのフラグを持たす
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="id">ファミリ</param>
    /// <returns></returns>
    public static bool SetVoidHojyoLine( Document doc, ElementId id, string line )
    {
      return ClsRevitUtil.CustomDataSet<bool>( doc, id, line, true ) ;
    }

    /// <summary>
    /// 対象のファミリが掘削補助線かどうかのフラグを取得する
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="id">ファミリ</param>
    /// <returns>フラグを持っているか</returns>
    public static bool GetVoidHojyoLine( Document doc, ElementId id, string line )
    {
      bool flag = false ;
      ClsRevitUtil.CustomDataGet<bool>( doc, id, line, out flag ) ;

      return flag ;
    }

    /// <summary>
    /// 掘削補助線の長さを調整する
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="targetId">基準となる掘削補助線</param>
    /// <returns>調整した掘削補助線</returns>
    public static ModelLine CreateAdjustVoidLine( Document doc, ElementId targetId, string line )
    {
      ModelLine modelLine = doc.GetElement( targetId ) as ModelLine ;
      Curve cv = ( modelLine.Location as LocationCurve ).Curve ;
      XYZ tmpStPoint = cv.GetEndPoint( 0 ) ;
      XYZ tmpEdPoint = cv.GetEndPoint( 1 ) ;
      List<ElementId> deleteList = new List<ElementId>() ;
      deleteList.Add( targetId ) ;
      List<XYZ> createStList = new List<XYZ>() ;
      List<XYZ> createEdList = new List<XYZ>() ;

      ModelLine createTargetLine = modelLine ;

      //図面のモデル線分を全て取得;
      List<ElementId> modelLineList = ClsRevitUtil.GetALLModelLine( doc ) ;
      foreach ( ElementId modelLineId in modelLineList ) {
        if ( targetId == modelLineId )
          continue ;

        if ( GetVoidHojyoLine( doc, modelLineId, line ) ) {
          ModelLine mlModelLine = doc.GetElement( modelLineId ) as ModelLine ;
          Curve mlCv = ( mlModelLine.Location as LocationCurve ).Curve ;
          XYZ insec = ClsRevitUtil.GetIntersection( mlCv, cv ) ;
          if ( insec != null ) {
            if ( ClsRevitUtil.CheckNearGetEndPoint( cv, insec ) )
              tmpStPoint = insec ;
            else
              tmpEdPoint = insec ;

            XYZ mlStPoint = mlCv.GetEndPoint( 0 ) ;
            XYZ mlEdPoint = mlCv.GetEndPoint( 1 ) ;
            if ( ClsRevitUtil.CheckNearGetEndPoint( mlCv, insec ) )
              mlStPoint = insec ;
            else
              mlEdPoint = insec ;
            createStList.Add( mlStPoint ) ;
            createEdList.Add( mlEdPoint ) ;

            deleteList.Add( modelLineId ) ;
          }
        }
      }

      //交叉する補助線が無いため終了
      if ( deleteList.Count < 2 )
        return createTargetLine ;

      //壁側の掘削補助線は出隅部のみ掘削補助線の調整を行う
      if ( line == OutVoidLine ) {
        bool bIrizumi = false ;
        //壁側掘削補助線は壁芯と向きが逆の為反転
        Curve cv1 = Line.CreateBound( tmpEdPoint, tmpStPoint ) ;
        for ( int i = 0 ; i < createStList.Count ; i++ ) {
          //壁側掘削補助線は向きが壁芯と逆の為反転
          Curve cv2 = Line.CreateBound( createEdList[ i ], createStList[ i ] ) ;
          bIrizumi = false ;
          if ( ! ClsHaraokoshiBase.CheckIrizumi( cv1, cv2, ref bIrizumi ) ) {
            return createTargetLine ;
          }

          if ( ! bIrizumi ) //出隅かの判断
          {
            createStList.Clear() ;
            createEdList.Clear() ;
            createStList.Add( cv2.GetEndPoint( 1 ) ) ;
            createEdList.Add( cv2.GetEndPoint( 0 ) ) ;
            break ;
          }
        }

        if ( bIrizumi ) //入隅の場合、削除、作成を行わない
        {
          return createTargetLine ;
        }
      }

      //掘削用補助線既存のものを削除し調整した補助線を作図
      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        t.Start() ;
        ClsRevitUtil.Delete( doc, deleteList ) ;
        t.Commit() ;

        t.Start() ;
        ModelLine targetLine = ClsYMSUtil.CreateKabeHojyoLine( doc, tmpStPoint, tmpEdPoint, 0, 0 ) ;
        SetVoidHojyoLine( doc, targetLine.Id, line ) ;
        createTargetLine = targetLine ;

        for ( int i = 0 ; i < createStList.Count ; i++ ) {
          ModelLine insecLine = ClsYMSUtil.CreateKabeHojyoLine( doc, createStList[ i ], createEdList[ i ], 0, 0 ) ;
          SetVoidHojyoLine( doc, insecLine.Id, line ) ;
        }

        t.Commit() ;
      }

      return createTargetLine ;
    }

    /// <summary>
    /// 掘削側指定
    /// </summary>
    /// <param name="doc">ドキュメント</param>
    /// <param name="id">選択された芯</param>
    /// <returns>(始点, 終点)</returns>
    public static (XYZ, XYZ) SelectVoidSide( UIDocument uidoc, ElementId id )
    {
      XYZ tmpStPoint = null ;
      XYZ tmpEdPoint = null ;
      Document doc = uidoc.Document ;
      Selection selection = uidoc.Selection ;
      Element inst = doc.GetElement( id ) ;
      LocationCurve lCurve = inst.Location as LocationCurve ;
      if ( lCurve == null ) {
        return ( tmpStPoint, tmpEdPoint ) ;
      }

      tmpStPoint = lCurve.Curve.GetEndPoint( 0 ) ;
      tmpEdPoint = lCurve.Curve.GetEndPoint( 1 ) ;
      XYZ Direction = Line.CreateBound( tmpStPoint, tmpEdPoint ).Direction ;

      //掘削側を指定
      XYZ kussaku = selection.PickPoint( "掘削側を指定してください" ) ;
      XYZ kussakuDir = Line.CreateBound( tmpStPoint, kussaku ).Direction ;
      if ( ! ClsGeo.IsLeft( Direction, kussakuDir ) ) {
        //SとE入れ替え
        XYZ p = tmpStPoint ;
        tmpStPoint = tmpEdPoint ;
        tmpEdPoint = p ;
      }

      return ( tmpStPoint, tmpEdPoint ) ;
    }

    /// <summary>
    /// 掘削側指定
    /// </summary>
    /// <param name="uidoc"></param>
    /// <param name="sPtn">始点</param>
    /// <param name="ePtn">終点</param>
    /// <returns>(始点, 終点)</returns>
    public static (XYZ, XYZ) SelectVoidSide( UIDocument uidoc, XYZ sPtn, XYZ ePtn )
    {
      XYZ tmpStPoint = sPtn ;
      XYZ tmpEdPoint = ePtn ;
      Selection selection = uidoc.Selection ;

      XYZ Direction = Line.CreateBound( tmpStPoint, tmpEdPoint ).Direction ;

      //掘削側を指定
      XYZ kussaku = selection.PickPoint( "掘削側を指定してください" ) ;
      XYZ kussakuDir = Line.CreateBound( tmpStPoint, kussaku ).Direction ;
      if ( ! ClsGeo.IsLeft( Direction, kussakuDir ) ) {
        //SとE入れ替え
        XYZ p = tmpStPoint ;
        tmpStPoint = tmpEdPoint ;
        tmpEdPoint = p ;
      }

      return ( tmpStPoint, tmpEdPoint ) ;
    }
  }
}