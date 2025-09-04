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
  public class ClsHiuchiTsunagizaiBase
  {
    #region 定数

    public const string baseName = "火打繋ぎ材ベース" ;

    #endregion

    #region Enum

    /// <summary>
    /// 取り付け方法
    /// </summary>
    public enum ToritsukeHoho
    {
      Bolt,
      Buruman,
      Rikiman
    }

    /// <summary>
    /// 処理方法
    /// </summary>
    public enum ShoriType
    {
      Replace,
      Manual
    }

    #endregion

    #region プロパティ

    /// <summary>
    /// 鋼材情報：分類
    /// </summary>
    public string m_SteelType { get ; set ; }

    /// <summary>
    ///鋼材情報：サイズ
    /// </summary>
    public string m_SteelSize { get ; set ; }

    /// <summary>
    /// 分割フラグ
    /// </summary>
    public bool m_SplitFlg { get ; set ; }

    /// <summary>
    /// 取り付け方法
    /// </summary>
    public ToritsukeHoho m_ToritsukeHoho { get ; set ; }

    /// <summary>
    /// 処理方法
    /// </summary>
    public ShoriType m_ShoriType { get ; set ; }

    /// <summary>
    /// ボルト種類１
    /// </summary>
    public string m_BoltType1 { get ; set ; }

    /// <summary>
    /// ボルト種類２
    /// </summary>
    public string m_BoltType2 { get ; set ; }

    /// <summary>
    /// ボルト本数
    /// </summary>
    public int m_BoltNum { get ; set ; }

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

    public ClsHiuchiTsunagizaiBase()
    {
      //初期化
      Init() ;
    }

    #endregion

    #region メソッド

    /// <summary>
    /// 初期化
    /// </summary>
    public void Init()
    {
      m_SteelType = string.Empty ;
      m_SteelSize = string.Empty ;
      m_ShoriType = ShoriType.Replace ;
      m_SplitFlg = false ;
      m_ToritsukeHoho = ToritsukeHoho.Bolt ;
      m_ShoriType = ShoriType.Manual ;
      m_BoltType1 = string.Empty ;
      m_BoltType2 = string.Empty ;
      m_BoltNum = 0 ;
      m_Floor = string.Empty ;
      m_Dan = string.Empty ;
      m_FlgEdit = false ;
    }

    public bool CreateHiuchiTsunagizaiBase( Document doc, List<ElementId> modelLineIdList, ElementId levelID )
    {
      string symbolFolpath = ClsZumenInfo.GetYMSFolder() ;
      string shinfamily = System.IO.Path.Combine( symbolFolpath, "ベース関係\\" + baseName + ".rfa" ) ;
      //シンボル読込
      if ( ! ClsRevitUtil.LoadFamilySymbolData( doc, shinfamily, baseName, out FamilySymbol sym ) ) {
        return false ;
      }

      foreach ( ElementId modelLineId in modelLineIdList ) {
        ModelLine modelLine = doc.GetElement( modelLineId ) as ModelLine ;
        LocationCurve lCurve = modelLine.Location as LocationCurve ;
        if ( lCurve == null ) {
          continue ;
        }

        Curve cv = lCurve.Curve ;
        string dan = "上段" ;

        using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
          t.Start() ;
          ElementId CreatedID = ClsRevitUtil.Create( doc, cv, levelID, sym ) ;
          ClsRevitUtil.SetMojiParameter( doc, CreatedID, "段", dan ) ;
          ClsRevitUtil.SetMojiParameter( doc, CreatedID, "分類", m_SteelType ) ;
          ClsRevitUtil.SetMojiParameter( doc, CreatedID, "サイズ", m_SteelSize ) ;
          ClsRevitUtil.SetMojiParameter( doc, CreatedID, "取付方法", GetToritsukeHoho( m_ToritsukeHoho ) ) ;
          if ( m_ToritsukeHoho == ToritsukeHoho.Bolt ) {
            ClsRevitUtil.SetMojiParameter( doc, CreatedID, "ボルトタイプ1", m_BoltType1 ) ;
            ClsRevitUtil.SetMojiParameter( doc, CreatedID, "ボルトタイプ2", m_BoltType2 ) ;
            ClsRevitUtil.SetParameter( doc, CreatedID, "ボルト本数", m_BoltNum ) ;

            //ボルト情報をカスタムデータとして設定する
            ClsYMSUtil.SetBolt( doc, CreatedID, m_BoltType2, m_BoltNum ) ;
          }

          t.Commit() ;
        }
      }

      return true ;
    }

    public bool ChangeHiuchiTsunagizaiBase( Document doc, XYZ tmpStPoint, XYZ tmpEdPoint, string dan,
      ElementId levelID )
    {
      string symbolFolpath = ClsZumenInfo.GetYMSFolder() ;
      string shinfamily = System.IO.Path.Combine( symbolFolpath, "ベース関係\\" + baseName + ".rfa" ) ;
      //シンボル読込
      if ( ! ClsRevitUtil.LoadFamilySymbolData( doc, shinfamily, baseName, out FamilySymbol sym ) ) {
        return false ;
      }

      Curve cv = Line.CreateBound( tmpStPoint, tmpEdPoint ) ;
      dan = "上段" ;

      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        t.Start() ;
        ElementId CreatedID = ClsRevitUtil.Create( doc, cv, levelID, sym ) ;
        ClsRevitUtil.SetMojiParameter( doc, CreatedID, "段", dan ) ;
        ClsRevitUtil.SetMojiParameter( doc, CreatedID, "分類", m_SteelType ) ;
        ClsRevitUtil.SetMojiParameter( doc, CreatedID, "サイズ", m_SteelSize ) ;
        ClsRevitUtil.SetMojiParameter( doc, CreatedID, "取付方法", GetToritsukeHoho( m_ToritsukeHoho ) ) ;
        if ( m_ToritsukeHoho == ToritsukeHoho.Bolt ) {
          ClsRevitUtil.SetMojiParameter( doc, CreatedID, "ボルトタイプ1", m_BoltType1 ) ;
          ClsRevitUtil.SetMojiParameter( doc, CreatedID, "ボルトタイプ2", m_BoltType2 ) ;
          ClsRevitUtil.SetParameter( doc, CreatedID, "ボルト本数", m_BoltNum ) ;

          //ボルト情報をカスタムデータとして設定する
          ClsYMSUtil.SetBolt( doc, CreatedID, m_BoltType2, m_BoltNum ) ;
        }
        else {
          //ボルト情報のカスタムデータを削除する
          ClsYMSUtil.DeleteBolt( doc, CreatedID ) ;
        }

        t.Commit() ;
      }

      return true ;
    }

    public bool ChangeHiuchiTsunagizaiBase( Document doc, XYZ tmpStPoint, XYZ tmpEdPoint )
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
      m_Dan = "上段" ;

      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        t.Start() ;
        FailureHandlingOptions failOpt = t.GetFailureHandlingOptions() ;
        failOpt.SetFailuresPreprocessor( new WarningSwallower() ) ;
        t.SetFailureHandlingOptions( failOpt ) ;
        ElementId CreatedID = ClsRevitUtil.Create( doc, cv, levelID, sym ) ;
        ClsRevitUtil.SetMojiParameter( doc, CreatedID, "段", m_Dan ) ;
        ClsRevitUtil.SetMojiParameter( doc, CreatedID, "分類", m_SteelType ) ;
        ClsRevitUtil.SetMojiParameter( doc, CreatedID, "サイズ", m_SteelSize ) ;
        ClsRevitUtil.SetMojiParameter( doc, CreatedID, "取付方法", GetToritsukeHoho( m_ToritsukeHoho ) ) ;
        if ( m_ToritsukeHoho == ToritsukeHoho.Bolt ) {
          ClsRevitUtil.SetMojiParameter( doc, CreatedID, "ボルトタイプ1", m_BoltType1 ) ;
          ClsRevitUtil.SetMojiParameter( doc, CreatedID, "ボルトタイプ2", m_BoltType2 ) ;
          ClsRevitUtil.SetParameter( doc, CreatedID, "ボルト本数", m_BoltNum ) ;

          //ボルト情報をカスタムデータとして設定する
          ClsYMSUtil.SetBolt( doc, CreatedID, m_BoltType2, m_BoltNum ) ;
        }
        else {
          //ボルト情報のカスタムデータを削除する
          ClsYMSUtil.DeleteBolt( doc, CreatedID ) ;
        }

        t.Commit() ;
      }

      return true ;
    }

    public (List<XYZ>, List<XYZ>) ChangeSplitHiuchiTsunagizaiBase( Document doc, ElementId id )
    {
      List<XYZ> stList = new List<XYZ>() ;
      List<XYZ> edList = new List<XYZ>() ;
      List<XYZ> insecList = new List<XYZ>() ;

      FamilyInstance baseInst = doc.GetElement( id ) as FamilyInstance ;
      LocationCurve lCurve = baseInst.Location as LocationCurve ;
      if ( lCurve == null ) {
        return ( stList, edList ) ;
      }

      Curve baseCv = lCurve.Curve ;
      XYZ baseStPnt = baseCv.GetEndPoint( 0 ) ;
      XYZ baseEdptn = baseCv.GetEndPoint( 1 ) ;

      if ( m_SplitFlg ) {
        List<ElementId> hiuchiList = new List<ElementId>() ;
        foreach ( string baseName in ClsGlobal.m_baseHiuchiList ) {
          hiuchiList.AddRange( ClsRevitUtil.GetSelectCreatedFamilyInstanceList( doc, baseName ) ) ;
        }

        foreach ( ElementId hiuchiId in hiuchiList ) {
          FamilyInstance inst = doc.GetElement( hiuchiId ) as FamilyInstance ;
          Curve cv = ( inst.Location as LocationCurve ).Curve ;
          XYZ insec = ClsRevitUtil.GetIntersection( baseCv, cv ) ;
          if ( insec != null ) {
            insecList.Add( insec ) ;
          }
        }

        List<ClsSortedPoint> sortedPointList = ClsGeo.PointsSort( insecList, baseStPnt ) ;

        stList.Add( baseStPnt ) ;
        //ソートリストの最初と最後は使用しない
        for ( int i = 1 ; i < sortedPointList.Count - 1 ; i++ ) {
          edList.Add( sortedPointList[ i ].Point ) ;
          stList.Add( sortedPointList[ i ].Point ) ;
        }

        edList.Add( baseEdptn ) ;
      }
      else {
        stList.Add( baseStPnt ) ;
        edList.Add( baseEdptn ) ;
      }

      return ( stList, edList ) ;
    }

    public ToritsukeHoho GetToritsukeHoho( string toritsuke )
    {
      switch ( toritsuke ) {
        case "ボルト" :
        {
          return ToritsukeHoho.Bolt ;
        }
        case "ブルマン" :
        {
          return ToritsukeHoho.Buruman ;
        }
        case "リキマン" :
        {
          return ToritsukeHoho.Rikiman ;
        }
        default :
        {
          return 0 ;
        }
      }
    }

    public string GetToritsukeHoho( ToritsukeHoho toritsuke )
    {
      switch ( toritsuke ) {
        case ToritsukeHoho.Bolt :
        {
          return "ボルト" ;
        }
        case ToritsukeHoho.Buruman :
        {
          return "ブルマン" ;
        }
        case ToritsukeHoho.Rikiman :
        {
          return "リキマン" ;
        }
        default :
        {
          return "ボルト" ;
        }
      }
    }

    /// <summary>
    /// 火打ツナギ材ベース のみを単独選択
    /// </summary>
    /// <param name="uidoc"></param>
    /// <param name="id">選択した 火打ツナギ材ベース のID</param>
    /// <param name="message">指示</param>
    /// <returns></returns>
    public static bool PickBaseObject( UIDocument uidoc, ref ElementId id, string message = baseName )
    {
      return ClsRevitUtil.PickObject( uidoc, message, baseName, ref id ) ;
    }


    /// <summary>
    /// 図面上の火打ツナギ材ベースを全て取得する
    /// </summary>
    /// <param name="doc"></param>
    /// <returns></returns>
    public static List<ElementId> GetAllHiuchiTsunagizaiBaseList( Document doc )
    {
      //図面上の火打ツナギ材ベースを全て取得
      List<ElementId> htIdList = ClsRevitUtil.GetSelectCreatedFamilyInstanceList( doc, baseName ) ;
      return htIdList ;
    }

    /// <summary>
    ///  図面上の火打ツナギ材ベースを全てクラスで取得する
    /// </summary>
    /// <param name="doc"></param>
    /// <returns></returns>
    public static List<ClsHiuchiTsunagizaiBase> GetAllClsHiuchiTsunagizaiBaseList( Document doc )
    {
      List<ClsHiuchiTsunagizaiBase> lstBase = new List<ClsHiuchiTsunagizaiBase>() ;

      List<ElementId> lstId = GetAllHiuchiTsunagizaiBaseList( doc ) ;
      foreach ( ElementId id in lstId ) {
        ClsHiuchiTsunagizaiBase clsHT = new ClsHiuchiTsunagizaiBase() ;
        clsHT.SetParameter( doc, id ) ;
        lstBase.Add( clsHT ) ;
      }

      return lstBase ;
    }

    /// <summary>
    /// 指定したIDから火打ツナギ材ベースクラスを取得する
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
      m_SteelType = ClsRevitUtil.GetParameter( doc, id, "分類" ) ;
      m_SteelSize = ClsRevitUtil.GetParameter( doc, id, "サイズ" ) ;
      string toritsuke = ClsRevitUtil.GetParameter( doc, id, "取付方法" ) ;
      m_ToritsukeHoho = GetToritsukeHoho( toritsuke ) ;
      if ( m_ToritsukeHoho == ClsHiuchiTsunagizaiBase.ToritsukeHoho.Bolt ) {
        m_BoltType1 = ClsRevitUtil.GetParameter( doc, id, "ボルトタイプ1" ) ;
        m_BoltType2 = ClsRevitUtil.GetParameter( doc, id, "ボルトタイプ2" ) ;
        m_BoltNum = ClsRevitUtil.GetParameterInteger( doc, id, "ボルト本数" ) ;
      }

      return ;
    }

    /// <summary>
    /// ﾌﾞﾙﾏﾝの作図（ベースのエレメントIDが必須）
    /// </summary>
    /// <param name="doc"></param>
    /// <returns></returns>
    public bool CreateTanbuParts( Document doc )
    {
      ElementId baseID = m_ElementId ;
      if ( baseID == null ) {
        return false ;
      }

      FamilySymbol sym = null ;

      double dVectAngle = 0.0 ;

      if ( m_ToritsukeHoho == ToritsukeHoho.Buruman ) {
        if ( m_SteelType != "チャンネル" ) {
          string buruFamPath = Master.ClsBurumanCSV.GetFamilyPath( "C-50" ) ;
          string buruFamName = RevitUtil.ClsRevitUtil.GetFamilyName( buruFamPath ) ;
          if ( ! ClsRevitUtil.LoadFamilyData( doc, buruFamPath, out Family buruFam ) ) {
            //return false; ;
          }

          sym = ( ClsRevitUtil.GetFamilySymbol( doc, buruFamName, "火打繋ぎ" ) ) ;
        }
        else {
          string buruFamPath = Master.ClsBurumanCSV.GetFamilyPath( "G" ) ;
          string buruFamName = RevitUtil.ClsRevitUtil.GetFamilyName( buruFamPath ) ;
          if ( ! ClsRevitUtil.LoadFamilyData( doc, buruFamPath, out Family buruFam ) ) {
            //return false; ;
          }

          sym = ( ClsRevitUtil.GetFamilySymbol( doc, buruFamName, "火打繋ぎ" ) ) ;
          dVectAngle += Math.PI ;
        }
      }
      else if ( m_ToritsukeHoho == ToritsukeHoho.Rikiman ) {
        string rikiFamPath = Master.ClsRikimanCSV.GetFamilyPath( "G" ) ;
        string rikiFamName = RevitUtil.ClsRevitUtil.GetFamilyName( rikiFamPath ) ;
        if ( ! ClsRevitUtil.LoadFamilyData( doc, rikiFamPath, out Family buruFam ) ) {
          //return false; ;
        }

        sym = ( ClsRevitUtil.GetFamilySymbol( doc, rikiFamName, "火打繋ぎ" ) ) ;
      }
      else
        return true ;

      double size ;
      switch ( m_SteelType ) {
        case "チャンネル" :
        case "アングル" :
        case "H形鋼 広幅" :
        case "H形鋼 中幅" :
        case "H形鋼 細幅" :
        {
          double sunpou2 =
            ClsCommonUtils.ChangeStrToDbl( Master.ClsYamadomeCsv.GetKouzaiSizeSunpou( m_SteelSize, 1 ) ) ;
          size = ClsRevitUtil.CovertToAPI( sunpou2 / 2 ) ; /// 2;
          break ;
        }
        default :
        {
          double sunpou2 = Master.ClsYamadomeCsv.GetWidth( m_SteelSize ) ;
          size = ClsRevitUtil.CovertToAPI( sunpou2 ) ; // / 2;
          break ;
        }
      }

      Element inst = doc.GetElement( baseID ) ;
      LocationCurve lCurve = inst.Location as LocationCurve ;
      if ( lCurve == null ) {
        return false ;
      }

      Curve cv = lCurve.Curve ;
      XYZ tmpStPoint = lCurve.Curve.GetEndPoint( 0 ) ;
      XYZ tmpEdPoint = lCurve.Curve.GetEndPoint( 1 ) ;
      XYZ dir = Line.CreateBound( tmpStPoint, tmpEdPoint ).Direction ;

      ClsYMSUtil.GetDifferenceWithAllBase( doc, baseID, out double diff, out double diff2 ) ;
      //始端側の梁と終点側の梁を選別
      List<ElementId> lstHari = YMS.ClsYMSUtil.GetIntersectionBase( doc, baseID ) ;

      foreach ( ElementId id in lstHari ) {
        FamilyInstance el = doc.GetElement( id ) as FamilyInstance ;
        ElementId levelID = el.Host.Id ;
        ;
        LocationCurve lCurve2 = el.Location as LocationCurve ;
        XYZ insec = ClsRevitUtil.GetIntersection( lCurve.Curve, lCurve2.Curve ) ;

        if ( insec != null ) {
          string danA = ClsRevitUtil.GetParameter( doc, baseID, "段" ) ;
          string kouzaiSizeA = ClsRevitUtil.GetParameter( doc, id, "鋼材サイズ" ) ;
          Curve cvA = lCurve2.Curve ;
          XYZ tmpStPointA = lCurve2.Curve.GetEndPoint( 0 ) ;
          XYZ tmpEdPointA = lCurve2.Curve.GetEndPoint( 1 ) ;
          XYZ direction2 = Line.CreateBound( tmpStPointA, tmpEdPointA ).Direction ;
          double sizeA = ClsRevitUtil.CovertToAPI( Master.ClsYamadomeCsv.GetWidth( kouzaiSizeA ) / 2 ) + diff ;
          using ( Transaction tx = new Transaction( doc, "Load Family" ) ) {
            tx.Start() ;

            if ( sym != null ) {
              double angle =
                dVectAngle -
                ( dir.AngleOnPlaneTo( XYZ.BasisX, XYZ.BasisZ ) +
                  ClsGeo.Deg2Rad( 150 ) ) ; // * dir.X + 30 * dir.Y));// dir.AngleOnPlaneTo(direction2, XYZ.BasisZ) + 

              double dW = size ; ///繋ぎ材サイズ
              //ブルマンの挿入位置を算出 // 方向ベクトルを正規化
              XYZ leftDirection = ClsRevitUtil.RotationDirection( dir, Math.PI / 2 ) ; // dVectAngle);//vecSEA;// 
              XYZ moveVector = leftDirection * ( dW * 0.7 ) ; // / 2) * 0.8;
              Curve cvBase = Line.CreateBound( tmpStPoint + moveVector, tmpEdPoint + moveVector ) ;

              //切梁ベース側
              XYZ leftDirection2 = dir ; //ClsRevitUtil.RotationDirection(direction2, Math.PI / 2);//  
              double dFVPWidth = sizeA ; //
              XYZ moveVector2 = leftDirection2 * dFVPWidth ;
              Curve cvHariS = Line.CreateBound( tmpStPointA + moveVector2, tmpEdPointA + moveVector2 ) ;

              XYZ pBuru = RevitUtil.ClsRevitUtil.GetIntersection( cvBase, cvHariS ) ;

              if ( pBuru != null ) {
                //ブルマンの挿入
                ElementId CreatedId = ClsRevitUtil.Create( doc, pBuru, levelID, sym ) ;
                Line axis = Line.CreateBound( pBuru, pBuru + XYZ.BasisZ ) ;
                ClsRevitUtil.RotateElement( doc, CreatedId, axis, angle ) ;
                ClsKariKouzai.SetKariKouzaiFlag( doc, CreatedId ) ;
                //ClsRevitUtil.RotateElement(doc, CreatedId, axis, ClsGeo.Deg2Rad(-30));
                //上下の位置調整
                if ( danA == "上段" ) {
                  ClsRevitUtil.SetParameter( doc, CreatedId, "基準レベルからの高さ", sizeA * 2 - diff ) ;
                }
                else if ( danA == "下段" ) {
                  ClsRevitUtil.SetParameter( doc, CreatedId, "基準レベルからの高さ", diff ) ;
                }
                else {
                  ClsRevitUtil.SetParameter( doc, CreatedId, "基準レベルからの高さ", sizeA ) ;
                }
              }
            }

            tx.Commit() ;
          }
        }
      }

      return true ;
    }

    #endregion
  }
}