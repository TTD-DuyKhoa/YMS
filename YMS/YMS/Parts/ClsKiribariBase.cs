using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;

namespace YMS.Parts
{
  public class ClsKiribariBase
  {
    #region 定数

    public const string baseName = "切梁ベース" ;

    /// <summary>
    /// 処理タイプ
    /// </summary>
    public enum ShoriType
    {
      BaseLine,
      PtoP
    }

    #endregion

    #region プロパティ

    /// <summary>
    /// 鋼材タイプ
    /// </summary>
    public string m_kouzaiType { get ; set ; }

    /// <summary>
    /// 鋼材サイズ
    /// </summary>
    public string m_kouzaiSize { get ; set ; }

    /// <summary>
    /// 処理方法
    /// </summary>
    public ShoriType m_ShoriType { get ; set ; }

    /// <summary>
    /// 端部部品（始点側）
    /// </summary>
    public string m_tanbuStart { get ; set ; }

    /// <summary>
    /// 端部部品（終点側）
    /// </summary>
    public string m_tanbuEnd { get ; set ; }

    /// <summary>
    /// ジャッキタイプ1
    /// </summary>
    public string m_jack1 { get ; set ; }

    /// <summary>
    /// ジャッキタイプ2
    /// </summary>
    public string m_jack2 { get ; set ; }

    /// <summary>
    /// 編集用：フロア
    /// </summary>
    public string m_Floor { get ; set ; }

    /// <summary>
    /// 編集用：エレメントID
    /// </summary>
    public ElementId m_ElementId { get ; set ; }

    /// <summary>
    /// 編集用：段
    /// </summary>
    public string m_Dan { get ; set ; }

    /// <summary>
    /// 編集用：編集フラグ
    /// </summary>
    public bool m_FlgEdit { get ; set ; }

    #endregion

    #region コンストラクタ

    public ClsKiribariBase()
    {
      //初期化
      Init() ;
    }

    #endregion

    #region メソッド

    public void Init()
    {
      m_kouzaiType = string.Empty ;
      m_kouzaiSize = string.Empty ;
      //m_bTwin = false;
      //m_bSMH = false;
      m_ShoriType = ShoriType.BaseLine ;
      m_tanbuStart = string.Empty ;
      m_tanbuEnd = string.Empty ;
      m_jack1 = string.Empty ;
      m_jack2 = string.Empty ;
      m_Floor = string.Empty ;
      m_Dan = string.Empty ;
      m_FlgEdit = false ;
    }

    /// <summary>
    /// 選択したモデル線分に切梁ベースを作成する
    /// </summary>
    /// <param name="doc">ドキュメント</param>
    /// <param name="ids">選択された芯</param>
    /// <param name="haraokoshiShin">基準となる段の腹起ベース</param>
    /// <param name="sameDan">配置レベルの段</param>
    /// <returns></returns>
    public bool CreateKiribariBase( Document doc, List<ElementId> ids, ElementId haraokoshiShin, string sameDan )
    {
      string symbolFolpath = ClsZumenInfo.GetYMSFolder() ;
      string shinfamily = System.IO.Path.Combine( symbolFolpath, "ベース関係\\" + baseName + ".rfa" ) ;

      //シンボル読込
      if ( ! ClsRevitUtil.LoadFamilySymbolData( doc, shinfamily, baseName, out FamilySymbol sym ) ) {
        return false ;
      }

      //図面上の腹起ベースを全て取得
      List<ElementId> haraIdList = ClsHaraokoshiBase.GetAllHaraokoshiBaseList( doc ) ;

      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        foreach ( ElementId id in ids ) {
          Element inst = doc.GetElement( id ) ;
          FamilyInstance haraokoshiShinInstLevel = doc.GetElement( haraokoshiShin ) as FamilyInstance ;
          LocationCurve lCurve = inst.Location as LocationCurve ;
          if ( lCurve == null ) {
            continue ;
          }

          XYZ tmpStPoint = lCurve.Curve.GetEndPoint( 0 ) ;
          XYZ tmpEdPoint = lCurve.Curve.GetEndPoint( 1 ) ;

          ElementId levelID = haraokoshiShinInstLevel.Host.Id ; //GetLevelID(doc, "レベル 1");
          Curve cv = Line.CreateBound( tmpStPoint, tmpEdPoint ) ;

          //腹起ベースとの交点を見つける
          //Z軸を0で統一すると腹起ベースが複数あった場合に交点が複数できてしまう
          //Z軸を選択した腹起ベースと同じ高さに設定
          LocationCurve lCurveZ = haraokoshiShinInstLevel.Location as LocationCurve ;
          if ( lCurveZ == null ) {
            continue ;
          }

          XYZ tmpStPointZ = lCurveZ.Curve.GetEndPoint( 0 ) ;
          XYZ tmpEdPointZ = lCurveZ.Curve.GetEndPoint( 1 ) ;
          //X,Y:選択したモデル線分, Z:選択した腹起ベース//モデル線分の長さを伸ばすならここ
          tmpStPointZ = new XYZ( tmpStPoint.X, tmpStPoint.Y, tmpStPointZ.Z ) ;
          tmpEdPointZ = new XYZ( tmpEdPoint.X, tmpEdPoint.Y, tmpEdPointZ.Z ) ;
          Curve cvZ = Line.CreateBound( tmpStPointZ, tmpEdPointZ ) ;

          List<XYZ> insecList = new List<XYZ>() ;
          foreach ( ElementId haraId in haraIdList ) {
            FamilyInstance haraokoshiShinInst = doc.GetElement( haraId ) as FamilyInstance ;
            LocationCurve lCurveHara = haraokoshiShinInst.Location as LocationCurve ;
            if ( lCurveHara != null ) {
              XYZ tmpStPointHara = lCurveHara.Curve.GetEndPoint( 0 ) ;
              XYZ tmpEdPointHara = lCurveHara.Curve.GetEndPoint( 1 ) ;
              Curve cvHara = Line.CreateBound( tmpStPointHara, tmpEdPointHara ) ;
              XYZ insec = ClsRevitUtil.GetIntersection( cvZ, cvHara ) ;
              if ( insec != null ) {
                insecList.Add( insec ) ;
              }
            }
          }

          //交点が１つもない場合は選択したモデル線分の端点に近い腹起ベースまで交点を伸ばす
          if ( insecList.Count == 0 ) {
            XYZ insec1 = ClsYMSUtil.GetLineNearFamilyInstancePoint( doc, tmpStPointZ, tmpEdPointZ, haraIdList ) ;
            XYZ insec2 = ClsYMSUtil.GetLineNearFamilyInstancePoint( doc, tmpEdPointZ, tmpStPointZ, haraIdList ) ;
            cv = Line.CreateBound( insec1, insec2 ) ;
          }

          //交点が１つしかない場合は選択したモデル線分のどちらの端点に近いかを判定する
          if ( insecList.Count == 1 ) {
            double distanceToStart = insecList[ 0 ].DistanceTo( tmpStPointZ ) ;
            double distanceToEnd = insecList[ 0 ].DistanceTo( tmpEdPointZ ) ;
            XYZ insec ;
            if ( distanceToStart < distanceToEnd ) {
              // 判定したい点は始点に近いです
              insec = ClsYMSUtil.GetLineNearFamilyInstancePoint( doc, tmpEdPointZ, tmpStPointZ, haraIdList ) ;
              if ( ! ClsGeo.GEO_EQ( insec, insecList[ 0 ] ) )
                cv = Line.CreateBound( insecList[ 0 ], insec ) ;
              else
                cv = Line.CreateBound( insecList[ 0 ], tmpEdPointZ ) ;
            }
            else {
              // 判定したい点は終点に近いです
              insec = ClsYMSUtil.GetLineNearFamilyInstancePoint( doc, tmpStPointZ, tmpEdPointZ, haraIdList ) ;
              if ( ! ClsGeo.GEO_EQ( insec, insecList[ 0 ] ) )
                cv = Line.CreateBound( insec, insecList[ 0 ] ) ;
              else
                cv = Line.CreateBound( tmpStPointZ, insecList[ 0 ] ) ;
            }
          }

          //交点が2つある場合は交点で切梁ベースを作成する
          if ( insecList.Count == 2 ) {
            cv = Line.CreateBound( insecList[ 0 ], insecList[ 1 ] ) ;
          }

          //切梁ベースの向きを統一する
          cv = ClsRevitUtil.ChangDirection( cv.GetEndPoint( 0 ), cv.GetEndPoint( 1 ) ) ;

          t.Start() ;
          ElementId CreatedID = ClsRevitUtil.Create( doc, cv, levelID, sym ) ;
          ClsRevitUtil.SetMojiParameter( doc, CreatedID, "段", sameDan ) ;
          ClsRevitUtil.SetMojiParameter( doc, CreatedID, "鋼材タイプ", m_kouzaiType ) ;
          ClsRevitUtil.SetMojiParameter( doc, CreatedID, "鋼材サイズ", m_kouzaiSize ) ;
          ClsRevitUtil.SetMojiParameter( doc, CreatedID, "端部部品(始点側)", m_tanbuStart ) ;
          ClsRevitUtil.SetMojiParameter( doc, CreatedID, "端部部品(終点側)", m_tanbuEnd ) ;
          ClsRevitUtil.SetMojiParameter( doc, CreatedID, "ジャッキタイプ(1)", m_jack1 ) ;
          ClsRevitUtil.SetMojiParameter( doc, CreatedID, "ジャッキタイプ(2)", m_jack2 ) ;
          t.Commit() ;
        }
      }

      return true ;
    }

    public bool ChangeKiribariBase( Document doc, ElementId id, string dan, ElementId levelID )
    {
      string symbolFolpath = ClsZumenInfo.GetYMSFolder() ;
      string shinfamily = System.IO.Path.Combine( symbolFolpath, "ベース関係\\" + baseName + ".rfa" ) ;

      //シンボル読込
      if ( ! ClsRevitUtil.LoadFamilySymbolData( doc, shinfamily, baseName, out FamilySymbol sym ) ) {
        return false ;
      }

      //Curve cv = Line.CreateBound(tmpStPoint, tmpEdPoint);

      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        t.Start() ;
        ElementId CreatedID = id ; // ClsRevitUtil.Create(doc, cv, levelID, sym);
        ClsRevitUtil.SetMojiParameter( doc, CreatedID, "段", dan ) ;
        ClsRevitUtil.SetMojiParameter( doc, CreatedID, "鋼材タイプ", m_kouzaiType ) ;
        ClsRevitUtil.SetMojiParameter( doc, CreatedID, "鋼材サイズ", m_kouzaiSize ) ;
        ClsRevitUtil.SetMojiParameter( doc, CreatedID, "端部部品(始点側)", m_tanbuStart ) ;
        ClsRevitUtil.SetMojiParameter( doc, CreatedID, "端部部品(終点側)", m_tanbuEnd ) ;
        ClsRevitUtil.SetMojiParameter( doc, CreatedID, "ジャッキタイプ(1)", m_jack1 ) ;
        ClsRevitUtil.SetMojiParameter( doc, CreatedID, "ジャッキタイプ(2)", m_jack2 ) ;
        t.Commit() ;
      }

      return true ;
    }

    /// <summary>
    /// ベース作成（ベースコピー機能で使用　他でも使えるだろうけど未検証）
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="tmpStPoint"></param>
    /// <param name="tmpEdPoint"></param>
    /// <param name="levelID"></param>
    /// <returns></returns>
    public bool CreateKiribariBase( Document doc, XYZ tmpStPoint, XYZ tmpEdPoint )
    {
      ElementId levelID = ClsRevitUtil.GetLevelID( doc, m_Floor ) ;
      if ( levelID == null ) {
        return false ;
      }

      string symbolFolpath = ClsZumenInfo.GetYMSFolder() ;
      string shinfamily = System.IO.Path.Combine( symbolFolpath, "ベース関係\\" + baseName + ".rfa" ) ;

      //シンボル読込
      if ( ! ClsRevitUtil.LoadFamilySymbolData( doc, shinfamily, baseName, out FamilySymbol sym ) ) {
        return false ;
      }

      Curve cv = Line.CreateBound( tmpStPoint, tmpEdPoint ) ;

      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        t.Start() ;
        FailureHandlingOptions failOpt = t.GetFailureHandlingOptions() ;
        failOpt.SetFailuresPreprocessor( new WarningSwallower() ) ;
        t.SetFailureHandlingOptions( failOpt ) ;
        ElementId CreatedID = ClsRevitUtil.Create( doc, cv, levelID, sym ) ;
        ClsRevitUtil.SetMojiParameter( doc, CreatedID, "段", m_Dan ) ;
        ClsRevitUtil.SetMojiParameter( doc, CreatedID, "鋼材タイプ", m_kouzaiType ) ;
        ClsRevitUtil.SetMojiParameter( doc, CreatedID, "鋼材サイズ", m_kouzaiSize ) ;
        ClsRevitUtil.SetMojiParameter( doc, CreatedID, "端部部品(始点側)", m_tanbuStart ) ;
        ClsRevitUtil.SetMojiParameter( doc, CreatedID, "端部部品(終点側)", m_tanbuEnd ) ;
        ClsRevitUtil.SetMojiParameter( doc, CreatedID, "ジャッキタイプ(1)", m_jack1 ) ;
        ClsRevitUtil.SetMojiParameter( doc, CreatedID, "ジャッキタイプ(2)", m_jack2 ) ;
        t.Commit() ;
      }

      return true ;
    }

    /// <summary>
    /// 切梁変更に伴い切梁火打ベースの位置を修正する
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="kiriBaseId"></param>
    /// <param name="beforeSize"></param>
    /// <returns></returns>
    public bool ChangeInterSectionKiribariBaseWithKiribariHiuchBase( Document doc, ElementId kiriBaseId,
      string beforeSize )
    {
      FamilyInstance instKiriBase = doc.GetElement( kiriBaseId ) as FamilyInstance ;
      Curve cvBase = ( instKiriBase.Location as LocationCurve ).Curve ;
      string afterSize = ClsRevitUtil.GetParameter( doc, kiriBaseId, "鋼材サイズ" ) ;

      if ( beforeSize == afterSize )
        return false ;

      List<KeyValuePair<ElementId, XYZ>> insecIdPointlist = new List<KeyValuePair<ElementId, XYZ>>() ;

      List<ElementId> allKiribariHiuchBaseIdList = ClsKiribariHiuchiBase.GetAllKiribariHiuchiBaseList( doc ).ToList() ;

      foreach ( ElementId id in allKiribariHiuchBaseIdList ) {
        FamilyInstance inst = doc.GetElement( id ) as FamilyInstance ;
        if ( instKiriBase == inst )
          continue ;
        Curve cv = ( inst.Location as LocationCurve ).Curve ;
        XYZ insec = ClsRevitUtil.GetIntersection( cvBase, cv ) ;
        if ( insec != null ) {
          KeyValuePair<ElementId, XYZ> pair = new KeyValuePair<ElementId, XYZ>( id, insec ) ;
          insecIdPointlist.Add( pair ) ;
        }
      }

      //変更
      double web = Master.ClsYamadomeCsv.GetWeb( beforeSize ) ;
      double webB = Master.ClsYamadomeCsv.GetWeb( afterSize ) ;
      double offset = ( webB - web ) / 2 ; //切梁火打ちが接する切梁-腹起or切梁のなす角が90のときのoffset値/tan(θ)でθ=切梁火打設定角度
      XYZ dir = new XYZ() ;
      foreach ( KeyValuePair<ElementId, XYZ> p in insecIdPointlist ) {
        ElementId id = p.Key ;
        XYZ insec = p.Value ;
        FamilyInstance inst = doc.GetElement( id ) as FamilyInstance ;
        Curve cv = ( inst.Location as LocationCurve ).Curve ;
        XYZ endPoint = cv.GetEndPoint( 1 ) ;
        //切梁火打ベースの終点が切梁の始終点どちらに近いかを調べ移動向きを決定
        if ( ClsGeo.GEO_EQ( endPoint, insec ) ) {
          endPoint = cv.GetEndPoint( 0 ) ;
        }

        if ( ClsRevitUtil.CheckNearGetEndPoint( cvBase, endPoint ) ) {
          dir = Line.CreateBound( cvBase.GetEndPoint( 0 ), insec ).Direction ;
        }
        else {
          dir = Line.CreateBound( cvBase.GetEndPoint( 1 ), insec ).Direction ;
        }

        double angle = ClsRevitUtil.GetParameterDouble( doc, id, "角度" ) ;
        using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
          t.Start() ;
          ClsRevitUtil.MoveFamilyInstance( inst, offset / Math.Tan( angle ), dir ) ;
          t.Commit() ;
        }
      }

      return true ;
    }

    /// <summary>
    /// 端部部品の作図（ベースのエレメントIDが必須）
    /// </summary>
    /// <param name="doc"></param>
    /// <returns></returns>
    public bool CreateTanbuParts( Document doc, ref XYZ pS, ref XYZ pE ) //自在火打ち対応
    {
      ElementId baseID = m_ElementId ;
      if ( baseID == null ) {
        return false ;
      }

      //ベース
      FamilyInstance ShinInstLevel = doc.GetElement( baseID ) as FamilyInstance ;
      ElementId levelID = ShinInstLevel.Host.Id ;

      Element inst = doc.GetElement( baseID ) ;
      LocationCurve lCurve = inst.Location as LocationCurve ;
      if ( lCurve == null ) {
        return false ;
      }

      XYZ tmpStPoint = lCurve.Curve.GetEndPoint( 0 ) ;
      XYZ tmpEdPoint = lCurve.Curve.GetEndPoint( 1 ) ;
      XYZ vecSE = ( tmpEdPoint - tmpStPoint ).Normalize() ;

      //始端側の梁と終点側の梁を選別
      List<ElementId> lstHari = ClsHaraokoshiBase.GetIntersectionBase( doc, baseID, levelID ) ;
      if ( lstHari.Count < 2 )
        return false ;

      ElementId idA = null ;
      ElementId idB = null ;
      double dSMin = 0 ;
      double dEMin = 0 ;
      foreach ( ElementId id in lstHari ) {
        Element el = doc.GetElement( id ) ;
        LocationCurve lCurve2 = el.Location as LocationCurve ;
        XYZ p = ClsRevitUtil.GetIntersection( lCurve.Curve, lCurve2.Curve ) ;

        double dS = tmpStPoint.DistanceTo( p ) ;
        double dE = tmpEdPoint.DistanceTo( p ) ;
        if ( idA == null || dSMin > dS ) {
          idA = id ;
          dSMin = dS ;
        }

        if ( idB == null || dEMin > dE ) {
          idB = id ;
          dEMin = dE ;
        }
      }

      //梁（始点側）
      Element elA = doc.GetElement( idA ) ;
      LocationCurve lCurveA = elA.Location as LocationCurve ;
      if ( lCurveA == null ) {
        return false ;
      }

      string danA = ClsRevitUtil.GetParameter( doc, idA, "段" ) ;
      XYZ tmpStPointA = lCurveA.Curve.GetEndPoint( 0 ) ;
      XYZ tmpEdPointA = lCurveA.Curve.GetEndPoint( 1 ) ;
      XYZ vecSEA = ( tmpEdPointA - tmpStPointA ).Normalize() ;


      //梁（終点側）
      Element elB = doc.GetElement( idB ) ;
      LocationCurve lCurveB = elB.Location as LocationCurve ;
      if ( lCurveB == null ) {
        return false ;
      }

      string danB = ClsRevitUtil.GetParameter( doc, idB, "段" ) ;
      XYZ tmpStPointB = lCurveB.Curve.GetEndPoint( 0 ) ;
      XYZ tmpEdPointB = lCurveB.Curve.GetEndPoint( 1 ) ;
      XYZ vecSEB = ( tmpEdPointB - tmpStPointB ).Normalize() ;

      //角度
      double dHaraAngle =
        ClsRevitUtil.CalculateAngleBetweenLines( tmpStPointA, tmpEdPointA, tmpEdPointB, tmpStPointB ) ;
      double dAngleA = ClsRevitUtil.CalculateAngleBetweenLines( tmpStPoint, tmpEdPoint, tmpEdPointA, tmpStPointA ) ;
      double dAngleB = ClsRevitUtil.CalculateAngleBetweenLines( tmpStPoint, tmpEdPoint, tmpEdPointB, tmpStPointB ) ;


      //挿入位置を取得
      double dA = 0 ;
      double dB = 0 ;
      double dMain = 0 ;
      GetTotalLength( dAngleA, dAngleB, ref dMain, ref dA, ref dB ) ;
      pS = tmpStPoint + ( vecSE * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( dA ) ) ;
      pE = tmpEdPoint + ( -vecSE * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( dB ) ) ;


      //■■■共通処理■■■
      FamilyInstance lv = doc.GetElement( baseID ) as FamilyInstance ;


      FamilySymbol sym, sym2, sym3 = null ; //自在火打ち対応
      if ( m_kouzaiSize == "60SMH" || m_kouzaiSize == "40SMH" ) {
        m_kouzaiSize = "40HA" ; //端部に付ける部材は40HAの為置き換える
      }
      else if ( m_kouzaiSize == "35SMH" ) {
        m_kouzaiSize = "35HA" ; //端部に付ける部材は35HAの為置き換える
      }


      //接続する腹起と鋼材サイズが違う場合、腹起と中心が合うようにオフセット値を足す
      string haraokoshiSize = ClsRevitUtil.GetParameter( doc, idA, "鋼材サイズ" ) ;
      double harasize = Master.ClsYamadomeCsv.GetWidth( haraokoshiSize ) ;
      double baseSize = Master.ClsYamadomeCsv.GetWidth( m_kouzaiSize ) ;
      double diff = 0.0 ;
      if ( baseSize < harasize ) {
        diff = ClsRevitUtil.CovertToAPI( ( harasize - baseSize ) / 2 ) ;
      }


      string shinfamily = Master.ClsHiuchiCsv.GetFamilyPath( m_tanbuStart, m_kouzaiSize ) ;
      string shinfamilyName = RevitUtil.ClsRevitUtil.GetFamilyName( shinfamily ) ;
      string tanbuPieceSize = Master.ClsHiuchiCsv.GetSize( m_tanbuStart, m_kouzaiSize ) ;
      double sizeA = ClsRevitUtil.CovertToAPI( Master.ClsHiuchiCsv.GetWidth( m_tanbuStart, tanbuPieceSize ) / 2 ) ;

      //自在火打ち対応 -start
      string buruFamPath = Master.ClsBurumanCSV.GetFamilyPath( "C-50" ) ;
      string buruFamName = RevitUtil.ClsRevitUtil.GetFamilyName( buruFamPath ) ;
      if ( ! ClsRevitUtil.LoadFamilyData( doc, buruFamPath, out Family buruFam ) ) {
        //return false; ;
      }

      sym3 = ( ClsRevitUtil.GetFamilySymbol( doc, buruFamName, "自在受けピース(FVP)" ) ) ;
      //自在火打ち対応 -end

      //シンボル読込※タイプを持っている
      if ( m_tanbuStart == Master.ClsHiuchiCsv.TypeNameFVP || m_tanbuStart == Master.ClsHiuchiCsv.TypeNameKaitenVP ) {
        if ( ! ClsRevitUtil.LoadFamilyData( doc, shinfamily, out Family tanbuFam ) ) {
          //return false; ;
        }

        sym = ( ClsRevitUtil.GetFamilySymbol( doc, shinfamilyName, "切梁" ) ) ;
      }
      else {
        if ( ! ClsRevitUtil.LoadFamilySymbolData( doc, shinfamily, shinfamilyName, out sym ) ) {
          //return false;
        }
      }

      string shinfamily2 = Master.ClsHiuchiCsv.GetFamilyPath( m_tanbuEnd, m_kouzaiSize ) ;
      string shinfamilyName2 = RevitUtil.ClsRevitUtil.GetFamilyName( shinfamily2 ) ;
      string tanbuPieceSize2 = Master.ClsHiuchiCsv.GetSize( m_tanbuEnd, m_kouzaiSize ) ;
      double sizeB = ClsRevitUtil.CovertToAPI( Master.ClsHiuchiCsv.GetWidth( m_tanbuEnd, tanbuPieceSize2 ) / 2 ) ;
      if ( m_tanbuEnd == Master.ClsHiuchiCsv.TypeNameFVP || m_tanbuEnd == Master.ClsHiuchiCsv.TypeNameKaitenVP ) {
        //自在火打ちの処理は保留
        if ( ! ClsRevitUtil.LoadFamilyData( doc, shinfamily2, out Family tanbuFam ) ) {
          //return false; ;
        }

        sym2 = ( ClsRevitUtil.GetFamilySymbol( doc, shinfamilyName2, "切梁" ) ) ;
      }
      else {
        //シンボル読込
        if ( ! ClsRevitUtil.LoadFamilySymbolData( doc, shinfamily2, shinfamilyName2, out sym2 ) ) {
          //return false;
        }
      }

      using ( Transaction tx = new Transaction( doc, "Load Family" ) ) {
        tx.Start() ;


        //■■■始点側処理■■■
        if ( sym != null ) {
          if ( m_tanbuStart == Master.ClsHiuchiCsv.TypeNameFVP ) {
            //自在火打ち対応 -start
            //自在火打の挿入・位置調整・回転
            ElementId CreatedID = ClsRevitUtil.Create( doc, tmpStPoint, levelID, sym ) ;
            ClsKariKouzai.SetKariKouzaiFlag( doc, CreatedID ) ;
            FamilyInstance ch = doc.GetElement( CreatedID ) as FamilyInstance ;
            XYZ dirV = new XYZ( 1, 0, 0 ) ; //ファミリインスタンスの向き
            double dVectAngle = dirV.AngleTo( vecSEA ) ;
            Line axis = Line.CreateBound( tmpStPoint, tmpStPoint + XYZ.BasisZ ) ;
            ch.flipFacing() ;

            if ( ! ClsGeo.IsLeft( dirV, vecSEA ) ) {
              dVectAngle = -dVectAngle ;
            }

            XYZ axisDirection = ClsRevitUtil.RotateVector( dirV, -Math.PI ) ;
            Plane plane = Plane.CreateByThreePoints( tmpStPoint + new XYZ( 0, 1, 0 ), tmpStPoint + new XYZ( 0, 0, 1 ),
              tmpStPoint ) ;

            //RevitUtil.ClsRevitUtil.MirrorElement(doc, CreatedID, plane);
            //ElementTransformUtils.MoveElement(doc, CreatedID, tmpStPoint - ((LocationPoint)ch.Location).Point);
            //double dW = RevitUtil.ClsRevitUtil.CovertToAPI(Master.ClsYamadomeCsv.GetWidth(m_SteelSize));
            double dW = RevitUtil.ClsRevitUtil.CovertToAPI( ClsYMSUtil.GetKouzaiHabaFromBase( doc, baseID ) ) ;
            XYZ dirMove = new XYZ( -dW / 2, 0, 0 ) ; //移動長さ
            ElementTransformUtils.MoveElement( doc, CreatedID, dirMove ) ;
            ClsRevitUtil.RotateElement( doc, CreatedID, axis, dVectAngle ) ;

            //ブルマンの挿入位置を算出
            XYZ direction = vecSE.Normalize() ; // 方向ベクトルを正規化
            XYZ leftDirection = new XYZ( -direction.Y, direction.X, 0 ) ; // 左側方向に90度回転
            XYZ moveVector = leftDirection * ( dW / 2 ) * 0.8 ;
            Curve cvBase = Line.CreateBound( tmpStPoint + moveVector, tmpEdPoint + moveVector ) ;
            XYZ direction2 = vecSEA.Normalize() ; // 方向ベクトルを正規化
            XYZ leftDirection2 = new XYZ( -direction2.Y, direction2.X, 0 ) ; // 左側方向に90度回転
            string target = Master.ClsHiuchiCsv.GetTargetShuzai( tanbuPieceSize ) ;
            double dFVPWidth = RevitUtil.ClsRevitUtil.CovertToAPI( Master.ClsYamadomeCsv.GetWidth( target ) ) ;
            XYZ moveVector2 = leftDirection2 * dFVPWidth ;
            Curve cvHariS = Line.CreateBound( tmpStPointA + moveVector2, tmpEdPointA + moveVector2 ) ;
            XYZ pBuru = RevitUtil.ClsRevitUtil.GetIntersection( cvBase, cvHariS ) ;

            //ブルマンの挿入
            ElementId CreatedID_B = ClsRevitUtil.Create( doc, pBuru, levelID, sym3 ) ;
            ClsKariKouzai.SetKariKouzaiFlag( doc, CreatedID_B ) ;
            Line axis2 = Line.CreateBound( pBuru, pBuru + XYZ.BasisZ ) ;
            ClsRevitUtil.RotateElement( doc, CreatedID_B, axis2, dVectAngle + ClsGeo.Deg2Rad( -30 ) ) ;

            //上下の位置調整
            if ( danA == "上段" ) {
              ClsRevitUtil.SetParameter( doc, CreatedID, "基準レベルからの高さ", sizeA + diff ) ;
              ClsRevitUtil.SetParameter( doc, CreatedID_B, "基準レベルからの高さ", diff ) ;
            }
            else if ( danA == "下段" ) {
              ClsRevitUtil.SetParameter( doc, CreatedID, "基準レベルからの高さ", -sizeA - diff ) ;
              ClsRevitUtil.SetParameter( doc, CreatedID_B, "基準レベルからの高さ", -( sizeA - diff ) * 2 ) ;
            }
            else {
              ClsRevitUtil.SetParameter( doc, CreatedID, "基準レベルからの高さ", 0.0 ) ;
              ClsRevitUtil.SetParameter( doc, CreatedID_B, "基準レベルからの高さ", -sizeA - diff ) ;
            }
            //自在火打ち対応 -end
          }
          else if ( m_tanbuStart == Master.ClsHiuchiCsv.TypeNameKaitenVP ) {
            ElementId CreatedID = ClsRevitUtil.Create( doc, pS, levelID, sym ) ;
            ClsKariKouzai.SetKariKouzaiFlag( doc, CreatedID ) ;
            FamilyInstance ch = doc.GetElement( CreatedID ) as FamilyInstance ;
            //腹起ベースに対して全面接地するよう回転
            double dVectAngle = vecSEA.AngleOnPlaneTo( XYZ.BasisX, XYZ.BasisZ ) ;
            Line axis = Line.CreateBound( pS, pS + XYZ.BasisZ ) ;
            ClsRevitUtil.RotateElement( doc, CreatedID, axis, -dVectAngle ) ;
            //回転後に回転ピース回転軸が切梁上に乗るように移動させる
            var peiceAngle = vecSE.AngleOnPlaneTo( ch.FacingOrientation, XYZ.BasisZ ) ;
            var movement = 191.0 * Math.Sin( peiceAngle ) ;
            ClsRevitUtil.MoveFamilyInstance( ch, movement, vecSEA ) ;
            //回転ピースの面を回転
            if ( ! ClsGeo.IsLeft( vecSE, ch.FacingOrientation ) )
              peiceAngle = -ch.FacingOrientation.AngleOnPlaneTo( vecSE, XYZ.BasisZ ) ;
            ClsRevitUtil.SetParameter( doc, CreatedID, "θ_角度", Math.PI / 2 + peiceAngle ) ;

            //回転ピースの回転向きに制限があるため反転させる必要があるケースも存在する
            //ケース判別方法要検討
            //ファミリが回転しない
            if ( Math.PI * 7 / 12 < Math.PI / 2 + peiceAngle ) {
              if ( ch.flipFacing() )
                ClsRevitUtil.SetParameter( doc, CreatedID, "θ_角度", Math.PI / 2 - peiceAngle ) ;
            }

            var test1 = ClsGeo.Rad2Deg( dVectAngle ) ;
            var test2 = ClsGeo.Rad2Deg( peiceAngle ) ;


            if ( danA == "上段" )
              ClsRevitUtil.SetParameter( doc, CreatedID, "基準レベルからの高さ", sizeA + diff ) ;
            else if ( danA == "下段" )
              ClsRevitUtil.SetParameter( doc, CreatedID, "基準レベルからの高さ", -sizeA - diff ) ;
            else
              ClsRevitUtil.SetParameter( doc, CreatedID, "基準レベルからの高さ", 0.0 ) ;
            //火打長さ分位置をずらす
            double dPartsThicknessS = Master.ClsHiuchiCsv.GetThickness( m_tanbuStart, m_kouzaiSize ) ;
            pS += ( vecSE * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( dPartsThicknessS ) ) ;
          }
          else {
            ElementId CreatedID = ClsRevitUtil.Create( doc, pS, levelID, sym ) ;
            ClsKariKouzai.SetKariKouzaiFlag( doc, CreatedID ) ;
            FamilyInstance ch = doc.GetElement( CreatedID ) as FamilyInstance ;

            XYZ dirV = ch.FacingOrientation ; //new XYZ(1, 0, 0);//ファミリインスタンスの向き
            double dVectAngle = vecSE.AngleOnPlaneTo( XYZ.BasisY, XYZ.BasisZ ) ; //dirV.AngleTo(vecSE);
            Line axis = Line.CreateBound( pS, pS + XYZ.BasisZ ) ;

            //if (!ClsGeo.IsLeft(vecSEA, pS))
            //{
            //    dVectAngle = -dVectAngle;
            //}

            ClsRevitUtil.RotateElement( doc, CreatedID, axis, -dVectAngle ) ;

            if ( danA == "上段" )
              ClsRevitUtil.SetParameter( doc, CreatedID, "基準レベルからの高さ", sizeA + diff ) ;
            else if ( danA == "下段" )
              ClsRevitUtil.SetParameter( doc, CreatedID, "基準レベルからの高さ", -sizeA - diff ) ;
            else
              ClsRevitUtil.SetParameter( doc, CreatedID, "基準レベルからの高さ", 0.0 ) ;
            //火打長さ分位置をずらす
            double dPartsThicknessS = Master.ClsHiuchiCsv.GetThickness( m_tanbuStart, m_kouzaiSize ) ;
            pS += ( vecSE * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( dPartsThicknessS ) ) ;
          }
        }

        //■■■終点側処理■■■
        if ( sym2 != null ) {
          if ( m_tanbuEnd == Master.ClsHiuchiCsv.TypeNameFVP ) {
            ElementId CreatedID = ClsRevitUtil.Create( doc, tmpEdPoint, levelID, sym2 ) ;
            ClsKariKouzai.SetKariKouzaiFlag( doc, CreatedID ) ;
            FamilyInstance ch = doc.GetElement( CreatedID ) as FamilyInstance ;


            XYZ dirV = new XYZ( -1, 0, 0 ) ; //ファミリインスタンスの向き
            double dVectAngle = dirV.AngleTo( vecSEB ) ;
            Line axis = Line.CreateBound( tmpEdPoint, tmpEdPoint + XYZ.BasisZ ) ;

            if ( ! ClsGeo.IsLeft( dirV, vecSEB ) ) {
              dVectAngle = -dVectAngle ;
            }

            Plane plane = Plane.CreateByThreePoints( tmpEdPoint + new XYZ( 0, 1, 0 ), tmpEdPoint + new XYZ( 0, 0, 1 ),
              tmpEdPoint ) ;

            //RevitUtil.ClsRevitUtil.MirrorElement(doc, CreatedID, plane);
            //ElementTransformUtils.MoveElement(doc, CreatedID, tmpEdPoint - ((LocationPoint)ch.Location).Point);
            //double dW = RevitUtil.ClsRevitUtil.CovertToAPI(Master.ClsYamadomeCsv.GetWidth(m_kouzaiSize));
            double dW = RevitUtil.ClsRevitUtil.CovertToAPI( ClsYMSUtil.GetKouzaiHabaFromBase( doc, baseID ) ) ;
            XYZ dirMove = new XYZ( -dW / 2, 0, 0 ) ; //移動長さ
            ElementTransformUtils.MoveElement( doc, CreatedID, dirMove ) ;

            ClsRevitUtil.RotateElement( doc, CreatedID, axis, dVectAngle ) ;

            //ブルマンの挿入位置を算出
            XYZ direction = vecSE.Normalize() ; // 方向ベクトルを正規化
            XYZ leftDirection = new XYZ( -direction.Y, direction.X, 0 ) ; // 左側方向に90度回転
            XYZ moveVector = leftDirection * ( dW / 2 ) * 0.8 ;
            Curve cvBase = Line.CreateBound( tmpStPoint + moveVector, tmpEdPoint + moveVector ) ;
            XYZ direction2 = vecSEB.Normalize() ; // 方向ベクトルを正規化
            XYZ leftDirection2 = new XYZ( -direction2.Y, direction2.X, 0 ) ; // 左側方向に90度回転
            string target = Master.ClsHiuchiCsv.GetTargetShuzai( tanbuPieceSize2 ) ;
            double dFVPWidth = RevitUtil.ClsRevitUtil.CovertToAPI( Master.ClsYamadomeCsv.GetWidth( target ) ) ;
            XYZ moveVector2 = leftDirection2 * dFVPWidth ;
            Curve cvHariS = Line.CreateBound( tmpStPointB + moveVector2, tmpEdPointB + moveVector2 ) ;
            XYZ pBuru = RevitUtil.ClsRevitUtil.GetIntersection( cvBase, cvHariS ) ;

            //ブルマンの挿入
            ElementId CreatedID_B = ClsRevitUtil.Create( doc, pBuru, levelID, sym3 ) ;
            ClsKariKouzai.SetKariKouzaiFlag( doc, CreatedID_B ) ;
            Line axis2 = Line.CreateBound( pBuru, pBuru + XYZ.BasisZ ) ;
            ClsRevitUtil.RotateElement( doc, CreatedID_B, axis2, dVectAngle + ClsGeo.Deg2Rad( 30 ) ) ;

            if ( danB == "上段" ) {
              ClsRevitUtil.SetParameter( doc, CreatedID, "基準レベルからの高さ", sizeB + diff ) ;
              ClsRevitUtil.SetParameter( doc, CreatedID_B, "基準レベルからの高さ", diff ) ;
            }
            else if ( danB == "下段" ) {
              ClsRevitUtil.SetParameter( doc, CreatedID, "基準レベルからの高さ", -sizeB - diff ) ;
              ClsRevitUtil.SetParameter( doc, CreatedID_B, "基準レベルからの高さ", ( -sizeB - diff ) * 2 ) ;
            }
            else {
              ClsRevitUtil.SetParameter( doc, CreatedID, "基準レベルからの高さ", 0.0 ) ;
              ClsRevitUtil.SetParameter( doc, CreatedID_B, "基準レベルからの高さ", -sizeB - diff ) ;
            }
          }
          else if ( m_tanbuEnd == Master.ClsHiuchiCsv.TypeNameKaitenVP ) {
            ElementId CreatedID = ClsRevitUtil.Create( doc, pE, levelID, sym2 ) ;
            ClsKariKouzai.SetKariKouzaiFlag( doc, CreatedID ) ;
            FamilyInstance ch = doc.GetElement( CreatedID ) as FamilyInstance ;

            //腹起ベースに対して全面接地するよう回転
            double dVectAngle = vecSEB.AngleOnPlaneTo( XYZ.BasisX, XYZ.BasisZ ) ;
            Line axis = Line.CreateBound( pE, pE + XYZ.BasisZ ) ;
            ClsRevitUtil.RotateElement( doc, CreatedID, axis, -dVectAngle ) ;
            //回転後に回転ピース回転軸が切梁上に乗るように移動させる
            var peiceAngle = ( -vecSE ).AngleOnPlaneTo( ch.FacingOrientation, XYZ.BasisZ ) ;
            var movement = 191.0 * Math.Sin( peiceAngle ) ;
            ClsRevitUtil.MoveFamilyInstance( ch, movement, vecSEB ) ;
            //回転ピースの面を回転
            if ( ! ClsGeo.IsLeft( -vecSE, ch.FacingOrientation ) )
              peiceAngle = -ch.FacingOrientation.AngleOnPlaneTo( -vecSE, XYZ.BasisZ ) ;
            ClsRevitUtil.SetParameter( doc, CreatedID, "θ_角度", ( Math.PI / 2 + peiceAngle ) ) ;

            //回転ピースの回転向きに制限があるため反転させる必要があるケースも存在する
            //ケース判別方法要検討
            //ファミリが回転しない
            if ( Math.PI * 7 / 12 < Math.PI / 2 + peiceAngle ) {
              if ( ch.flipFacing() )
                ClsRevitUtil.SetParameter( doc, CreatedID, "θ_角度", Math.PI / 2 - peiceAngle ) ;
            }

            var test1 = ClsGeo.Rad2Deg( dVectAngle ) ;
            var test2 = ClsGeo.Rad2Deg( peiceAngle ) ;

            if ( danB == "上段" )
              ClsRevitUtil.SetParameter( doc, CreatedID, "基準レベルからの高さ", sizeB + diff ) ;
            else if ( danB == "下段" )
              ClsRevitUtil.SetParameter( doc, CreatedID, "基準レベルからの高さ", -sizeB - diff ) ;
            else
              ClsRevitUtil.SetParameter( doc, CreatedID, "基準レベルからの高さ", 0.0 ) ;
            //火打長さ分位置をずらす
            double dPartsThicknessE = Master.ClsHiuchiCsv.GetThickness( m_tanbuEnd, m_kouzaiSize ) ;
            pE += ( -vecSE * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( dPartsThicknessE ) ) ;
          }
          else {
            ElementId CreatedID = ClsRevitUtil.Create( doc, pE, levelID, sym2 ) ;
            ClsKariKouzai.SetKariKouzaiFlag( doc, CreatedID ) ;
            FamilyInstance ch = doc.GetElement( CreatedID ) as FamilyInstance ;
            //ch.flipFacing();//フリップするかベクトルにマイナスをかけるか

            XYZ dirV = ch.FacingOrientation ; // new XYZ(-1, 0, 0);//ファミリインスタンスの向き
            double dVectAngle = ( -vecSE ).AngleOnPlaneTo( XYZ.BasisY, XYZ.BasisZ ) ; //dirV.AngleTo(vecSE) + Math.PI;
            Line axis = Line.CreateBound( pE, pE + XYZ.BasisZ ) ;

            ClsRevitUtil.RotateElement( doc, CreatedID, axis, -dVectAngle ) ;
            if ( danB == "上段" )
              ClsRevitUtil.SetParameter( doc, CreatedID, "基準レベルからの高さ", sizeB + diff ) ;
            else if ( danB == "下段" )
              ClsRevitUtil.SetParameter( doc, CreatedID, "基準レベルからの高さ", -sizeB - diff ) ;
            else
              ClsRevitUtil.SetParameter( doc, CreatedID, "基準レベルからの高さ", 0.0 ) ;
            //火打長さ分位置をずらす
            double dPartsThicknessE = Master.ClsHiuchiCsv.GetThickness( m_tanbuEnd, m_kouzaiSize ) ;
            pE += ( -vecSE * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( dPartsThicknessE ) ) ;
          }
        }

        tx.Commit() ;
      }


      return true ;
    }

    public bool CreateTanbuParts_Test( Document doc )
    {
      ElementId baseID = m_ElementId ;
      if ( baseID == null ) {
        return false ;
      }

      //ベース
      FamilyInstance ShinInstLevel = doc.GetElement( baseID ) as FamilyInstance ;

      Element inst = doc.GetElement( baseID ) ;
      LocationCurve lCurve = inst.Location as LocationCurve ;
      if ( lCurve == null ) {
        return false ;
      }

      XYZ tmpStPoint = lCurve.Curve.GetEndPoint( 0 ) ;
      XYZ tmpEdPoint = lCurve.Curve.GetEndPoint( 1 ) ;
      XYZ vecSE = ( tmpEdPoint - tmpStPoint ).Normalize() ;

      //始端側の梁と終点側の梁を選別
      List<ElementId> lstHari = ClsHaraokoshiBase.GetIntersectionBase( doc, baseID ) ;
      if ( lstHari.Count < 2 )
        return false ;

      ElementId idA = null ;
      ElementId idB = null ;
      double dSMin = 0 ;
      double dEMin = 0 ;
      foreach ( ElementId id in lstHari ) {
        Element el = doc.GetElement( id ) ;
        LocationCurve lCurve2 = el.Location as LocationCurve ;
        XYZ p = ClsRevitUtil.GetIntersection( lCurve.Curve, lCurve2.Curve ) ;

        double dS = tmpStPoint.DistanceTo( p ) ;
        double dE = tmpEdPoint.DistanceTo( p ) ;
        if ( idA == null || dSMin > dS ) {
          idA = id ;
          dSMin = dS ;
        }

        if ( idB == null || dEMin > dE ) {
          idB = id ;
          dEMin = dE ;
        }
      }

      //梁（始点側）
      Element elA = doc.GetElement( idA ) ;
      LocationCurve lCurveA = elA.Location as LocationCurve ;
      if ( lCurveA == null ) {
        return false ;
      }

      string danA = ClsRevitUtil.GetParameter( doc, idA, "段" ) ;
      XYZ tmpStPointA = lCurveA.Curve.GetEndPoint( 0 ) ;
      XYZ tmpEdPointA = lCurveA.Curve.GetEndPoint( 1 ) ;
      XYZ vecSEA = ( tmpEdPointA - tmpStPointA ).Normalize() ;


      //梁（終点側）
      Element elB = doc.GetElement( idB ) ;
      LocationCurve lCurveB = elB.Location as LocationCurve ;
      if ( lCurveB == null ) {
        return false ;
      }

      string danB = ClsRevitUtil.GetParameter( doc, idB, "段" ) ;
      XYZ tmpStPointB = lCurveB.Curve.GetEndPoint( 0 ) ;
      XYZ tmpEdPointB = lCurveB.Curve.GetEndPoint( 1 ) ;
      XYZ vecSEB = ( tmpEdPointB - tmpStPointB ).Normalize() ;

      //角度
      double dHaraAngle =
        ClsRevitUtil.CalculateAngleBetweenLines( tmpStPointA, tmpEdPointA, tmpEdPointB, tmpStPointB ) ;
      double dAngleA = ClsRevitUtil.CalculateAngleBetweenLines( tmpStPoint, tmpEdPoint, tmpEdPointA, tmpStPointA ) ;
      double dAngleB = ClsRevitUtil.CalculateAngleBetweenLines( tmpStPoint, tmpEdPoint, tmpEdPointB, tmpStPointB ) ;


      //挿入位置を取得
      double dA = 0 ;
      double dB = 0 ;
      double dMain = 0 ;
      GetTotalLength( dAngleA, dAngleB, ref dMain, ref dA, ref dB ) ;
      XYZ pS = tmpStPoint + ( vecSE * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( dA ) ) ;
      XYZ pE = tmpEdPoint + ( -vecSE * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( dB ) ) ;


      //■■■共通処理■■■
      FamilyInstance lv = doc.GetElement( baseID ) as FamilyInstance ;
      ElementId levelId = lv.Host.Id ;

      FamilySymbol sym, sym2 ;
      string shinfamily = Master.ClsHiuchiCsv.GetFamilyPath( m_tanbuStart, m_kouzaiSize ) ;
      string shinfamilyName = RevitUtil.ClsRevitUtil.GetFamilyName( shinfamily ) ;
      string tanbuPieceSize = Master.ClsHiuchiCsv.GetSize( m_tanbuStart, m_kouzaiSize ) ;
      double sizeA = ClsRevitUtil.CovertToAPI( Master.ClsHiuchiCsv.GetWidth( m_tanbuStart, tanbuPieceSize ) / 2 ) ;
      //シンボル読込※タイプを持っている
      if ( m_tanbuStart == Master.ClsHiuchiCsv.TypeNameFVP || m_tanbuStart == Master.ClsHiuchiCsv.TypeNameKaitenVP ) {
        if ( ! ClsRevitUtil.LoadFamilyData( doc, shinfamily, out Family tanbuFam ) ) {
          //return false; ;
        }

        sym = ( ClsRevitUtil.GetFamilySymbol( doc, shinfamilyName, "切梁" ) ) ;
      }
      else {
        if ( ! ClsRevitUtil.LoadFamilySymbolData( doc, shinfamily, shinfamilyName, out sym ) ) {
          //return false;
        }
      }

      string shinfamily2 = Master.ClsHiuchiCsv.GetFamilyPath( m_tanbuEnd, m_kouzaiSize ) ;
      string shinfamilyName2 = RevitUtil.ClsRevitUtil.GetFamilyName( shinfamily2 ) ;
      string tanbuPieceSize2 = Master.ClsHiuchiCsv.GetSize( m_tanbuEnd, m_kouzaiSize ) ;
      double sizeB = ClsRevitUtil.CovertToAPI( Master.ClsHiuchiCsv.GetWidth( m_tanbuEnd, tanbuPieceSize2 ) / 2 ) ;
      if ( m_tanbuEnd == Master.ClsHiuchiCsv.TypeNameFVP || m_tanbuEnd == Master.ClsHiuchiCsv.TypeNameKaitenVP ) {
        //自在火打ちの処理は保留
        if ( ! ClsRevitUtil.LoadFamilyData( doc, shinfamily2, out Family tanbuFam ) ) {
          //return false; ;
        }

        sym2 = ( ClsRevitUtil.GetFamilySymbol( doc, shinfamilyName2, "切梁" ) ) ;
      }
      else {
        //シンボル読込
        if ( ! ClsRevitUtil.LoadFamilySymbolData( doc, shinfamily2, shinfamilyName2, out sym2 ) ) {
          //return false;
        }
      }

      using ( Transaction tx = new Transaction( doc, "Load Family" ) ) {
        tx.Start() ;

        //■■■始点側処理■■■
        if ( sym != null ) {
          // シンボルを配置
          ElementId createdId = ClsRevitUtil.Create( doc, pS, levelId, sym ) ;

          // ファミリインスタンスの向きを取得
          FamilyInstance ch = doc.GetElement( createdId ) as FamilyInstance ;
          XYZ dirV = ch.FacingOrientation ;

          // シンボルの12時から6時の方向を軸にする
          XYZ axisDirection = ClsRevitUtil.RotateVector( dirV, Math.PI ) ;
          Plane plane = Plane.CreateByNormalAndOrigin( axisDirection.CrossProduct( XYZ.BasisZ ), pS ) ;

          // ファミリインスタンスをミラーリング
          List<ElementId> elementsToMirrorList = new List<ElementId>() ;
          elementsToMirrorList.Add( createdId ) ;
          ElementTransformUtils.MirrorElements( doc, elementsToMirrorList, plane, false ) ;

          // ミラーリング後、元の位置に移動
          ElementTransformUtils.MoveElement( doc, createdId, pS - ch.GetTransform().Origin ) ;

          // ミラーリングの確認用に鏡像前のサンプルを配置
          ElementId createdId2 = ClsRevitUtil.Create( doc, pS, levelId, sym ) ;

          // プロパティに値をセット
          if ( danA == "上段" )
            ClsRevitUtil.SetParameter( doc, createdId, "基準レベルからの高さ", sizeA ) ;
          else if ( danA == "下段" )
            ClsRevitUtil.SetParameter( doc, createdId, "基準レベルからの高さ", -sizeA ) ;
          else
            ClsRevitUtil.SetParameter( doc, createdId, "基準レベルからの高さ", 0.0 ) ;
        }

        //■■■終点側処理■■■
        if ( sym2 != null ) {
          ElementId CreatedID = ClsRevitUtil.Create( doc, pE, levelId, sym2 ) ;
          FamilyInstance ch = doc.GetElement( CreatedID ) as FamilyInstance ;
          ch.flipFacing() ;

          XYZ dirV = ch.FacingOrientation ;
          double dVectAngle = dirV.AngleTo( vecSE ) + Math.PI ;
          Line axis = Line.CreateBound( pE, pE + XYZ.BasisZ ) ;
          ClsRevitUtil.RotateElement( doc, CreatedID, axis, -dVectAngle ) ;

          if ( danB == "上段" )
            ClsRevitUtil.SetParameter( doc, CreatedID, "基準レベルからの高さ", sizeB ) ;
          else if ( danB == "下段" )
            ClsRevitUtil.SetParameter( doc, CreatedID, "基準レベルからの高さ", -sizeB ) ;
          else
            ClsRevitUtil.SetParameter( doc, CreatedID, "基準レベルからの高さ", 0.0 ) ;
        }

        tx.Commit() ;
      }


      return true ;
    }

    /// <summary>
    /// 切梁の全長取得処理
    /// </summary>
    /// <param name="dHaraAngle">腹起間角度</param>
    /// <returns></returns>
    public double GetTotalLength( double dStartAngle, double dEndAngle, ref double dMain, ref double dOffsetA,
      ref double dOffsetB )
    {
      double dTotalLength = 0 ; //切梁の長さ
      double d = 0.0 ;
      double d2 = 0.0 ;

      double dAngleA = 0 ;
      double dAngleB = 0 ;
      double dKouzaiWidthA = 0 ;
      double dKouzaiWidthB = 0 ;
      double dPartsThicknessA = 0 ;
      double dPartsThicknessB = 0 ;

      //角度
      //if (m_HiuchiAngle == HiuchiAngle.Nini)
      //{
      //    dAngleA = m_angle;
      //    dAngleB = 180 - dHaraAngle - m_angle;
      //}
      //else
      //{
      //    dAngleA = ClsGeo.FloorAtDigitAdjust(1, (180 - dHaraAngle) / 2);
      //    dAngleB = dAngleA;
      //}
      dAngleA = dStartAngle ;
      dAngleB = dEndAngle ;
      //鋼材幅
      if ( m_tanbuStart != Master.ClsHiuchiCsv.TypeNameFVP ) {
        //string targetShuzai = Master.ClsHiuchiCsv.GetTargetShuzai(m_HiuchiUkePieceSize1);
        dKouzaiWidthA = Master.ClsYamadomeCsv.GetWidth( m_kouzaiSize ) ;
      }
      else {
        dKouzaiWidthA = Master.ClsYamadomeCsv.GetWidth( m_kouzaiSize ) ;
        dPartsThicknessA = Master.ClsHiuchiCsv.GetThickness( m_tanbuStart, m_kouzaiSize ) ;
        double dA = ClsYMSUtil.CalculateOffsets( dAngleA, dKouzaiWidthA, dPartsThicknessA ) ;
        dOffsetA = dA ;
      }

      if ( m_tanbuEnd != Master.ClsHiuchiCsv.TypeNameFVP ) {
        //string targetShuzai = Master.ClsHiuchiCsv.GetTargetShuzai(m_HiuchiUkePieceSize2);
        dKouzaiWidthB = Master.ClsYamadomeCsv.GetWidth( m_kouzaiSize ) ;
      }
      else {
        dKouzaiWidthB = Master.ClsYamadomeCsv.GetWidth( m_kouzaiSize ) ;
        dPartsThicknessB = Master.ClsHiuchiCsv.GetThickness( m_tanbuEnd, m_kouzaiSize ) ;
        double dB = ClsYMSUtil.CalculateOffsets( dAngleB, dKouzaiWidthB, dPartsThicknessB ) ;
        dOffsetB = dB ;
      }

      dMain = dTotalLength ;

      return ClsGeo.FloorAtDigitAdjust( 0, dTotalLength + dOffsetA + dOffsetB ) ;
    }


    public bool CreateHojyoPeace( Document doc, XYZ tmpStPoint, XYZ tmpEdPoint, bool changeHojyoPeace, bool bJack )
    {
      ElementId baseID = m_ElementId ;
      if ( baseID == null ) {
        return false ;
      }

      //ベース
      FamilyInstance ShinInstLevel = doc.GetElement( baseID ) as FamilyInstance ;
      ElementId levelID = ShinInstLevel.Host.Id ;
      string dan = ClsRevitUtil.GetParameter( doc, baseID, "段" ) ;
      double size = ClsRevitUtil.CovertToAPI( Master.ClsYamadomeCsv.GetWidth( m_kouzaiSize ) / 2 ) ;

      ClsYMSUtil.GetDifferenceWithAllBase( doc, baseID, out double diff, out double diff2 ) ;
      size += diff ;
      //Element inst = doc.GetElement(baseID);
      //LocationCurve lCurve = inst.Location as LocationCurve;
      //if (lCurve == null)
      //{
      //    return false;
      //}

      //XYZ tmpStPoint = lCurve.Curve.GetEndPoint(0);
      //XYZ tmpEdPoint = lCurve.Curve.GetEndPoint(1);
      XYZ dir = Line.CreateBound( tmpStPoint, tmpEdPoint ).Direction ;

      string hojyo2 = Master.ClsSupportPieceCsv.GetSize( m_kouzaiSize, 2 ) ;
      string path2 = Master.ClsSupportPieceCsv.GetFamilyPath( hojyo2 ) ;
      string familyName2 = RevitUtil.ClsRevitUtil.GetFamilyName( path2 ) ;

      string hojyo3 = Master.ClsSupportPieceCsv.GetSize( m_kouzaiSize, 3 ) ;
      string path3 = Master.ClsSupportPieceCsv.GetFamilyPath( hojyo3 ) ;
      string familyName3 = RevitUtil.ClsRevitUtil.GetFamilyName( path3 ) ;

      if ( ! ClsRevitUtil.LoadFamilyData( doc, path2, out Family fam2 ) ) {
        return false ;
      }

      FamilySymbol sym2 = ( ClsRevitUtil.GetFamilySymbol( doc, familyName2, "切梁" ) ) ;
      var eSym = sym2 ;

      if ( ! ClsRevitUtil.LoadFamilyData( doc, path3, out Family fam3 ) ) {
        return false ;
      }

      FamilySymbol sym3 = ( ClsRevitUtil.GetFamilySymbol( doc, familyName3, "切梁" ) ) ;
      var sSym = sym3 ;

      //端部離れ量が300未満のときに補助ピースの位置を修正する
      if ( changeHojyoPeace ) {
        sSym = sym2 ;
        eSym = sym3 ;
      }

      using ( Transaction tx = new Transaction( doc, "Load Family" ) ) {
        tx.Start() ;
        //■■■始点側処理■■■
        if ( sSym != null ) {
          ElementId CreatedID = ClsRevitUtil.Create( doc, tmpStPoint, levelID, sSym ) ;
          FamilyInstance ch = doc.GetElement( CreatedID ) as FamilyInstance ;

          XYZ dirV = ch.HandOrientation ; //new XYZ(1, 0, 0);//ファミリインスタンスの向き
          double dVectAngle = dirV.AngleOnPlaneTo( dir, XYZ.BasisZ ) ;
          Line axis = Line.CreateBound( tmpStPoint, tmpStPoint + XYZ.BasisZ ) ;

          ClsRevitUtil.RotateElement( doc, CreatedID, axis, dVectAngle ) ;
          if ( dan == "上段" )
            ClsRevitUtil.SetParameter( doc, CreatedID, "基準レベルからの高さ", size ) ;
          else if ( dan == "下段" )
            ClsRevitUtil.SetParameter( doc, CreatedID, "基準レベルからの高さ", -size ) ;
          else
            ClsRevitUtil.SetParameter( doc, CreatedID, "基準レベルからの高さ", 0.0 ) ;
          ClsKariKouzai.SetBaseId( doc, CreatedID, baseID ) ;
          ClsKariKouzai.SetKariKouzaiFlag( doc, CreatedID ) ;
        }

        //■■■終点側処理■■■
        if ( eSym != null ) {
          ElementId CreatedID = ClsRevitUtil.Create( doc, tmpEdPoint, levelID, eSym ) ;
          FamilyInstance ch = doc.GetElement( CreatedID ) as FamilyInstance ;

          XYZ dirV = ch.HandOrientation ; //new XYZ(1, 0, 0);//ファミリインスタンスの向き
          double dVectAngle = dirV.AngleOnPlaneTo( dir, XYZ.BasisZ ) + Math.PI ;
          Line axis = Line.CreateBound( tmpEdPoint, tmpEdPoint + XYZ.BasisZ ) ;

          if ( bJack )
            ClsRevitUtil.RotateElement( doc, CreatedID, axis, dVectAngle + Math.PI ) ;
          else
            ClsRevitUtil.RotateElement( doc, CreatedID, axis, dVectAngle ) ;

          if ( dan == "上段" )
            ClsRevitUtil.SetParameter( doc, CreatedID, "基準レベルからの高さ", size ) ;
          else if ( dan == "下段" )
            ClsRevitUtil.SetParameter( doc, CreatedID, "基準レベルからの高さ", -size ) ;
          else
            ClsRevitUtil.SetParameter( doc, CreatedID, "基準レベルからの高さ", 0.0 ) ;
          ClsKariKouzai.SetBaseId( doc, CreatedID, baseID ) ;
          ClsKariKouzai.SetKariKouzaiFlag( doc, CreatedID ) ;
        }

        tx.Commit() ;
      }

      return true ;
    }

    public bool CreateTwinHojyoPeace( Document doc, XYZ tmpStPoint, ref XYZ tmpEdPoint )
    {
      ElementId baseId = m_ElementId ;
      if ( baseId == null ) {
        return false ;
      }

      //ベース
      FamilyInstance ShinInstLevel = doc.GetElement( baseId ) as FamilyInstance ;
      ElementId levelID = ShinInstLevel.Host.Id ;
      string dan = ClsRevitUtil.GetParameter( doc, baseId, "段" ) ;
      double offset = ClsRevitUtil.CovertToAPI( Master.ClsYamadomeCsv.GetWidth( m_kouzaiSize ) / 2 ) ;
      ClsYMSUtil.GetDifferenceWithAllBase( doc, baseId, out double diff, out double diff2 ) ;
      if ( dan == "上段" )
        offset = offset + diff ;
      else if ( dan == "下段" )
        offset = -offset - diff ;
      else
        offset = 0.0 ;

      XYZ dir = Line.CreateBound( tmpStPoint, tmpEdPoint ).Direction ;

      //string kirikae = Master.ClsSupportPieceCsv.GetSize(m_kouzaiSize, 2);
      string pathSwitch = Master.ClsSupportPieceCsv.GetFamilyPath( "604CD" ) ;
      string familyNameSwitch = RevitUtil.ClsRevitUtil.GetFamilyName( pathSwitch ) ;

      string hojyo3 = Master.ClsSupportPieceCsv.GetSize( "40HA", 3 ) ;
      string path3 = Master.ClsSupportPieceCsv.GetFamilyPath( hojyo3 ) ;
      string familyName3 = RevitUtil.ClsRevitUtil.GetFamilyName( path3 ) ;

      string jack = Master.ClsJackCsv.GetJackSize( m_jack1, m_kouzaiSize ) ;
      //string pathJack = Master.ClsJackCsv.GetFamilyPath(jack);
      //string familyNameJack = RevitUtil.ClsRevitUtil.GetFamilyName(pathJack);

      if ( ! ClsRevitUtil.LoadFamilyData( doc, pathSwitch, out Family famSwitch ) ) {
        return false ;
      }

      FamilySymbol symSwitch = ( ClsRevitUtil.GetFamilySymbol( doc, familyNameSwitch, "切梁" ) ) ;

      if ( ! ClsRevitUtil.LoadFamilyData( doc, path3, out Family fam3 ) ) {
        return false ;
      }

      FamilySymbol sym3 = ( ClsRevitUtil.GetFamilySymbol( doc, familyName3, "切梁" ) ) ;

      //if (!ClsRevitUtil.RoadFamilyData(doc, pathJack, familyNameJack, out Family famJack))
      //{
      //    return false;
      //}
      //FamilySymbol symJack = (ClsRevitUtil.GetFamilySymbol(doc, familyNameJack, "切梁"));

      ClsJack cjk = new ClsJack() ;
      string strTypeNameType = Master.ClsJackCsv.GetTypeNameType( jack ) ; //Atypeなど
      if ( strTypeNameType != "Atype" )
        cjk.m_JackTensyo = "九州" ;
      cjk.m_JackType = m_jack1 ;
      cjk.m_SyuzaiSize = m_kouzaiSize ;

      using ( Transaction tx = new Transaction( doc, "Load Family" ) ) {
        double length = ClsRevitUtil.CovertToAPI( 500.0 ) ;
        tmpStPoint = new XYZ( tmpStPoint.X + length * dir.X, tmpStPoint.Y + length * dir.Y, tmpStPoint.Z ) ;

        //■■■始点側処理■■■
        if ( symSwitch != null ) {
          tx.Start() ;
          ElementId CreatedID = ClsRevitUtil.Create( doc, tmpStPoint, levelID, symSwitch ) ;
          FamilyInstance ch = doc.GetElement( CreatedID ) as FamilyInstance ;

          XYZ dirV = ch.HandOrientation ; //new XYZ(1, 0, 0);//ファミリインスタンスの向き
          double dVectAngle = dirV.AngleOnPlaneTo( dir, XYZ.BasisZ ) ;
          Line axis = Line.CreateBound( tmpStPoint, tmpStPoint + XYZ.BasisZ ) ;

          ClsRevitUtil.RotateElement( doc, CreatedID, axis, dVectAngle ) ;
          ClsRevitUtil.SetParameter( doc, CreatedID, "基準レベルからの高さ", offset ) ;
          ClsKariKouzai.SetBaseId( doc, CreatedID, baseId ) ;
          ClsKariKouzai.SetKariKouzaiFlag( doc, CreatedID ) ;
          tx.Commit() ;
        }

        //■■■終点側処理■■■
        if ( sym3 != null ) {
          tx.Start() ;
          ElementId CreatedID = ClsRevitUtil.Create( doc, tmpEdPoint, levelID, sym3 ) ;
          FamilyInstance ch = doc.GetElement( CreatedID ) as FamilyInstance ;

          XYZ dirV = ch.HandOrientation ; //new XYZ(1, 0, 0);//ファミリインスタンスの向き
          double dVectAngle = dirV.AngleOnPlaneTo( dir, XYZ.BasisZ ) + Math.PI ;
          Line axis = Line.CreateBound( tmpEdPoint, tmpEdPoint + XYZ.BasisZ ) ;

          ClsRevitUtil.RotateElement( doc, CreatedID, axis, dVectAngle ) ;
          ClsRevitUtil.SetParameter( doc, CreatedID, "基準レベルからの高さ", offset ) ;

          ClsKariKouzai.SetBaseId( doc, CreatedID, baseId ) ;
          ClsKariKouzai.SetKariKouzaiFlag( doc, CreatedID ) ;
          length = ClsRevitUtil.CovertToAPI( 300.0 ) ;
          tx.Commit() ;
        }

        dir = Line.CreateBound( tmpEdPoint, tmpStPoint ).Direction ;
        tmpEdPoint = new XYZ( tmpEdPoint.X + length * dir.X, tmpEdPoint.Y + length * dir.Y, tmpEdPoint.Z ) ;

        if ( jack != string.Empty ) {
          double dVectAngle = XYZ.BasisX.AngleOnPlaneTo( dir, XYZ.BasisZ ) ;
          Line axis = Line.CreateBound( tmpEdPoint, tmpEdPoint + XYZ.BasisZ ) ;
          cjk.m_UseJackCover = true ;
          cjk.CreateJack( doc, baseId, tmpStPoint, tmpEdPoint, tmpEdPoint, true, dVectAngle ) ;

          length = ClsRevitUtil.CovertToAPI( ClsJack.GetTotalJack( doc, cjk.m_CreatedJackId ) + 500.0 ) ;
        }

        tmpEdPoint = new XYZ( tmpEdPoint.X + length * dir.X, tmpEdPoint.Y + length * dir.Y, tmpEdPoint.Z ) ;

        if ( symSwitch != null ) {
          tx.Start() ;
          ElementId CreatedID = ClsRevitUtil.Create( doc, tmpEdPoint, levelID, symSwitch ) ;
          FamilyInstance ch = doc.GetElement( CreatedID ) as FamilyInstance ;

          XYZ dirV = ch.HandOrientation ; //new XYZ(1, 0, 0);//ファミリインスタンスの向き
          double dVectAngle = dirV.AngleOnPlaneTo( dir, XYZ.BasisZ ) ; // + Math.PI;
          Line axis = Line.CreateBound( tmpEdPoint, tmpEdPoint + XYZ.BasisZ ) ;

          ClsRevitUtil.RotateElement( doc, CreatedID, axis, dVectAngle ) ;
          ClsRevitUtil.SetParameter( doc, CreatedID, "基準レベルからの高さ", offset ) ;
          ClsKariKouzai.SetBaseId( doc, CreatedID, baseId ) ;
          ClsKariKouzai.SetKariKouzaiFlag( doc, CreatedID ) ;
          tx.Commit() ;
        }
      }

      return true ;
    }

    /// <summary>
    /// 図面上の切梁ベースを全て取得する
    /// </summary>
    /// <param name="doc"></param>
    /// <returns></returns>
    public static List<ElementId> GetAllKiribariBaseList( Document doc, ElementId levelId = null )
    {
      //図面上の切梁ベースを全て取得
      List<ElementId> kiriIdList = new List<ElementId>() ;
      if ( levelId == null )
        kiriIdList = ClsRevitUtil.GetSelectCreatedFamilyInstanceList( doc, baseName ) ;
      else
        kiriIdList = ClsRevitUtil.GetSelectLevelCreatedFamilyInstanceList( doc, baseName, levelId ) ;
      return kiriIdList ;
    }

    /// <summary>
    /// 切梁ベース のみを単独選択
    /// </summary>
    /// <param name="uidoc"></param>
    /// <param name="id">選択した 切梁ベース のID</param>
    /// <param name="message">指示</param>
    /// <returns></returns>
    public static bool PickBaseObject( UIDocument uidoc, ref ElementId id, string message = baseName )
    {
      return ClsRevitUtil.PickObject( uidoc, message, baseName, ref id ) ;
    }

    /// <summary>
    /// 切梁ベース のみを単独選択
    /// </summary>
    /// <param name="uidoc"></param>
    /// <param name="id">選択した 切梁ベース のID</param>
    /// <param name="message">指示</param>
    /// <returns></returns>
    public static bool PickBaseObject( UIDocument uidoc, ref Reference rf, string message = baseName )
    {
      return ClsRevitUtil.PickObject( uidoc, message, baseName, ref rf ) ;
    }

    /// <summary>
    /// 切梁ベース のみを複数選択
    /// </summary>
    /// <param name="uidoc"></param>
    /// <param name="ids"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static bool PickBaseObjects( UIDocument uidoc, ref List<ElementId> ids, string message = baseName )
    {
      return ClsRevitUtil.PickObjectsPartFilter( uidoc, message + "を選択してください", baseName, ref ids ) ;
    }

    /// <summary>
    ///  図面上の切梁ベースを全てクラスで取得する
    /// </summary>
    /// <param name="doc"></param>
    /// <returns></returns>
    public static List<ClsKiribariBase> GetAllClsKiribariBaseList( Document doc )
    {
      List<ClsKiribariBase> lstBase = new List<ClsKiribariBase>() ;

      List<ElementId> lstId = GetAllKiribariBaseList( doc ) ;
      foreach ( ElementId id in lstId ) {
        ClsKiribariBase clsK = new ClsKiribariBase() ;
        clsK.SetParameter( doc, id ) ;
        lstBase.Add( clsK ) ;
      }

      return lstBase ;
    }

    /// <summary>
    /// 指定したIDから切梁ベースクラスを取得する
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public void SetParameter( Document doc, ElementId id )
    {
      FamilyInstance shinInstLevel = doc.GetElement( id ) as FamilyInstance ;
      m_Floor = shinInstLevel.Host.Name ;
      m_ElementId = id ;
      m_Dan = ClsRevitUtil.GetParameter( doc, id, "段" ) ;
      m_kouzaiType = ClsRevitUtil.GetParameter( doc, id, "鋼材タイプ" ) ;
      m_kouzaiSize = ClsRevitUtil.GetParameter( doc, id, "鋼材サイズ" ) ;
      m_tanbuStart = ClsRevitUtil.GetParameter( doc, id, "端部部品(始点側)" ) ;
      m_tanbuEnd = ClsRevitUtil.GetParameter( doc, id, "端部部品(終点側)" ) ;
      m_jack1 = ClsRevitUtil.GetParameter( doc, id, "ジャッキタイプ(1)" ) ;
      m_jack2 = ClsRevitUtil.GetParameter( doc, id, "ジャッキタイプ(2)" ) ;

      return ;
    }

    #endregion
  }
}