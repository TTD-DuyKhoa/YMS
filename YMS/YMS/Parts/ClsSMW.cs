using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;

namespace YMS.Parts
{
  public class ClsSMW
  {
    #region 定数

    /// <summary>
    /// 最大ジョイント数
    /// </summary>
    public const int PileJointMax = 10 ;

    public const string SMW = "SMW" ;
    public const string SOIL = "ソイル_単軸" ;

    #endregion


    #region プロパティ

    /// <summary>
    /// CASE番号
    /// </summary>
    public int m_case { get ; set ; }

    /// <summary>
    /// 枝番号
    /// </summary>
    public string m_edaNum { get ; set ; }

    /// <summary>
    /// 枝番号2
    /// </summary>
    public string m_edaNum2 { get ; set ; }

    /// <summary>
    /// 処理方法
    /// </summary>
    public int m_way { get ; set ; }

    /// <summary>
    /// 掘削深さ
    /// </summary>
    public int m_void { get ; set ; }

    /// <summary>
    /// 基準点からの距離
    /// </summary>
    public int m_refPDist { get ; set ; }

    /// <summary>
    /// 残置
    /// </summary>
    public string m_zanti { get ; set ; }

    /// <summary>
    /// 材質
    /// </summary>
    public string m_zaishitu { get ; set ; }

    /// <summary>
    /// 一部埋め殺し長さ
    /// </summary>
    public string m_zantiLength { get ; set ; }

    /// <summary>
    /// くり抜きフラグ
    /// </summary>
    public bool m_bVoid { get ; set ; }

    /// <summary>
    /// 径
    /// </summary>
    public int m_dia { get ; set ; }

    /// <summary>
    /// ソイルピッチ
    /// </summary>
    public int m_soil { get ; set ; }

    /// <summary>
    /// 天端
    /// </summary>
    public int m_soilTop { get ; set ; }

    /// <summary>
    /// 全長
    /// </summary>
    public int m_soilLen { get ; set ; }

    /// <summary>
    /// 支点側端部のソイルを作図
    /// </summary>
    public bool m_bTanbuS { get ; set ; }

    /// <summary>
    /// 終点側端部のソイルを作図
    /// </summary>
    public bool m_bTanbuE { get ; set ; }

    /// <summary>
    /// 配置の位置：タイプ
    /// </summary>
    public string m_type { get ; set ; }

    /// <summary>
    /// 配置の位置：サイズ
    /// </summary>
    public string m_size { get ; set ; }

    /// <summary>
    /// 配置の位置：天端
    /// </summary>
    public int m_HTop { get ; set ; }

    /// <summary>
    /// 配置の位置：全長
    /// </summary>
    public int m_HLen { get ; set ; }

    /// <summary>
    /// 入隅コーナーに芯材を配置フラグ
    /// </summary>
    public bool m_bCorner { get ; set ; }

    /// <summary>
    /// 配置パターン
    /// </summary>
    public int m_putPtnFlag { get ; set ; }

    /// <summary>
    /// 交互配置フラグ
    /// </summary>
    public bool m_KougoFlg { get ; set ; }

    /// <summary>
    /// 箇所数１
    /// </summary>
    public int m_Kasho1 { get ; set ; }

    /// <summary>
    /// 箇所数２
    /// </summary>
    public int m_Kasho2 { get ; set ; }

    /// <summary>
    /// 固定方法1
    /// </summary>
    public Master.Kotei m_Kotei1 { get ; set ; }

    /// <summary>
    /// 固定方法2
    /// </summary>
    public Master.Kotei m_Kotei2 { get ; set ; }

    /// <summary>
    /// 杭長さ１
    /// </summary>
    public List<int> m_ListPileLength1 { get ; set ; }

    /// <summary>
    /// 杭長さ２
    /// </summary>
    public List<int> m_ListPileLength2 { get ; set ; }

    /// <summary>
    /// ボルト1(フランジ)
    /// </summary>
    public string m_BoltF1 { get ; set ; }

    /// <summary>
    /// ボルト1(フランジ数)
    /// </summary>
    public string m_BoltFNum1 { get ; set ; }

    /// <summary>
    /// ボルト1(ウェブ)
    /// </summary>
    public string m_BoltW1 { get ; set ; }

    /// <summary>
    /// ボルト1(ウェブ数)
    /// </summary>
    public string m_BoltWNum1 { get ; set ; }

    /// <summary>
    /// ボルト2(フランジ)
    /// </summary>
    public string m_BoltF2 { get ; set ; }

    /// <summary>
    /// ボルト2(フランジ数)
    /// </summary>
    public string m_BoltFNum2 { get ; set ; }

    /// <summary>
    /// ボルト2(ウェブ)
    /// </summary>
    public string m_BoltW2 { get ; set ; }

    /// <summary>
    /// ボルト2(ウェブ数)
    /// </summary>
    public string m_BoltWNum2 { get ; set ; }

    /// <summary>
    /// 杭のファミリ名称
    /// </summary>
    public string FamilyNameKui
    {
      get
      {
        return "杭_" + m_size ; // 外部から取得される
      }
    }

    /// <summary>
    /// 配置順
    /// </summary>
    public int m_oreder { get ; set ; }

    #endregion

    #region コンストラクタ

    public ClsSMW()
    {
      //初期化
      Init() ;
    }

    #endregion

    #region メソッド

    public void Init()
    {
      m_case = 0 ;
      m_edaNum = string.Empty ;
      m_edaNum2 = string.Empty ;
      m_way = 0 ;
      m_void = 0 ;
      m_refPDist = 0 ;
      m_zanti = string.Empty ;
      m_zantiLength = string.Empty ;
      m_bVoid = false ;
      m_dia = 0 ;
      m_soil = 0 ;
      m_soilTop = 0 ;
      m_soilLen = 0 ;
      m_bTanbuS = false ;
      m_bTanbuE = false ;
      m_size = string.Empty ;
      m_HTop = 0 ;
      m_HLen = 0 ;
      m_bCorner = false ;
      m_putPtnFlag = 0 ;
      m_KougoFlg = false ;
      m_Kasho1 = 0 ;
      m_Kasho2 = 0 ;
      m_Kotei1 = Master.Kotei.Bolt ;
      m_Kotei2 = Master.Kotei.Bolt ;
      m_ListPileLength1 = new List<int>() ;
      m_ListPileLength2 = new List<int>() ;
      m_BoltF1 = string.Empty ;
      m_BoltFNum1 = string.Empty ;
      m_BoltW1 = string.Empty ;
      m_BoltWNum1 = string.Empty ;
      m_BoltF2 = string.Empty ;
      m_BoltFNum2 = string.Empty ;
      m_BoltW2 = string.Empty ;
      m_BoltWNum2 = string.Empty ;

      m_oreder = -1 ;

      return ;
    }

    /// <summary>
    /// SMWを作図する
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="picPointS"></param>
    /// <param name="picPointE"></param>
    /// <param name="soilDirection"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public bool CreateSMW( Document doc, XYZ picPointS, XYZ picPointE, ElementId levelId, bool bIrizumi = true,
      bool defVoid = true, ElementId kabeShinId = null )
    {
      try {
        Line soilLine = Line.CreateBound( picPointS, picPointE ) ;

        XYZ soilDirection = soilLine.Direction ; //単位ベクトル
        double length = ClsGeo.RoundOff( ClsRevitUtil.CovertFromAPI( soilLine.Length ), 1 ) ; //芯の長さ

        string symbolFolpath = ClsZumenInfo.GetYMSFolder() ;

        string familyPathSoil = System.IO.Path.Combine( symbolFolpath, "山留壁関係\\05_ｿｲﾙ\\ｿｲﾙ_単軸.rfa" ) ;
        string familyNameSoil = "ｿｲﾙ_単軸" ;

        string familyPathKui = Master.ClsHBeamCsv.GetPileFamilyPath( m_size ) ;
        string familyNameKui = ClsRevitUtil.GetFamilyName( familyPathKui ) ;

        //シンボル配置
        if ( ! ClsRevitUtil.LoadFamilySymbolData( doc, familyPathSoil, familyNameSoil, out FamilySymbol sym ) ) {
          return false ;
        }

        //親杭配置

        //#31788

        //if (!ClsRevitUtil.RoadFamilyData(doc, familyPathKui, familyNameKui, out Family oyaFam))
        //{
        //    return false;
        //}
        //FamilySymbol oya = ClsRevitUtil.GetFamilySymbol(doc, familyNameKui, "SMW");

        if ( ! ClsRevitUtil.LoadFamilySymbolData( doc, familyPathKui, "SMW", out FamilySymbol oya ) ) {
          return false ;
        }

        string stB = ClsYMSUtil.GetKouzaiSizeSunpou1( familyNameKui, 2 ) ;
        int dFlangeWidth = ClsCommonUtils.ChangeStrToInt( stB ) / 2 ;
        //ソイル
        //ClsRevitUtil.SetTypeParameter(sym, ClsGlobal.m_length, ClsRevitUtil.CovertToAPI(m_soilLen));
        //ClsRevitUtil.SetTypeParameter(sym, ClsGlobal.m_dia, ClsRevitUtil.CovertToAPI(m_dia));
        int refPDist = m_refPDist ; // + dFlangeWidth;
        int soil = m_soil ;
        XYZ originalPointS = picPointS ;
        //基準点から指定の数値離す
        picPointS = new XYZ( picPointS.X + ( ClsRevitUtil.CovertToAPI( refPDist ) * soilDirection.X ),
          picPointS.Y + ( ClsRevitUtil.CovertToAPI( refPDist ) * soilDirection.Y ), picPointS.Z ) ;
        length -= refPDist ;

        //始点に作図する杭なしソイルの数
        int kuiNoSoilCountS = refPDist / soil ;

        //H形鋼
        //ClsRevitUtil.SetTypeParameter(oya, ClsGlobal.m_length, ClsRevitUtil.CovertToAPI(m_HLen));
        string stH = ClsYMSUtil.GetKouzaiSizeSunpou1( familyNameKui, 1 ) ;
        double dEx = ClsCommonUtils.ChangeStrToDbl( stH ) / 2 ;

        //角度//X軸がプラスの時回転しすぎてしまうので分岐が必要
        double dAngle = soilDirection.AngleOnPlaneTo( XYZ.BasisX, XYZ.BasisZ ) ;
        if ( ! defVoid )
          dAngle += Math.PI ;

        XYZ picPoint = null ;
        //グループ化するIDを保持する変数の宣言(nullだとAdd出来ないため)
        ICollection<ElementId> selectionIds = new List<ElementId>() ;
        List<ElementId> selId = new List<ElementId>() ; //ボイド用

        //始点側から終点側に向かって、ソイル円の中心点が「ソイル配置区間-(ソイルピッチ×2)」を超えないように
        double dCount = ( length / soil ) - 2 ; // - 1;//ダイアログソイルピッチずつ杭、ソイルを配置するため何個置けるか
        int x = 0 ;
        ModelLine outLine = null, inLine = null ;

        using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
          t.Start() ;
          FailureHandlingOptions failOpt = t.GetFailureHandlingOptions() ;
          failOpt.SetFailuresPreprocessor( new WarningSwallower() ) ;
          t.SetFailureHandlingOptions( failOpt ) ;

          if ( kabeShinId != null )
            ClsRevitUtil.SetMojiParameter( doc, kabeShinId, "壁", SMW ) ;

          //ダイアログ指定の値をSET
          ClsRevitUtil.SetTypeParameter( oya, "切断長さ", 0.0 ) ;
          ClsRevitUtil.SetTypeParameter( oya, "トッププレート", 0 ) ;

          int putPtn = 0 ;

          for ( int i = 0 ; i <= dCount ; i++ ) {
            if ( ( ! bIrizumi && i == 0 ) ||
                 ( ! m_bTanbuS && i == 0 && kuiNoSoilCountS == 0 ) ) // || (!m_bTanbuE && dCount - 1 <= i)) 
            {
              continue ;
            } //出隅の時は別で出隅分作成するため始点を飛ばす

            picPoint = new XYZ( picPointS.X + ( ClsRevitUtil.CovertToAPI( soil * i ) * soilDirection.X ),
              picPointS.Y + ( ClsRevitUtil.CovertToAPI( soil * i ) * soilDirection.Y ),
              picPointS.Z + ( ClsRevitUtil.CovertToAPI( soil * i ) * soilDirection.Z ) ) ;

            //******ソイル作成*******//
            ElementId CreatedID = ClsRevitUtil.Create( doc, picPoint, levelId, sym ) ; //ソイル作成

            ClsRevitUtil.SetParameter( doc, CreatedID, ClsGlobal.m_refLvTop, ClsRevitUtil.CovertToAPI( m_soilTop ) ) ;
            ClsRevitUtil.SetTypeParameter( sym, ClsGlobal.m_length, ClsRevitUtil.CovertToAPI( m_soilLen ) ) ;
            ClsRevitUtil.SetTypeParameter( sym, ClsGlobal.m_dia, ClsRevitUtil.CovertToAPI( m_dia ) ) ;
            ClsVoid.SetVoidDep( doc, CreatedID, m_void ) ;
            selectionIds.Add( CreatedID ) ;
            selId.Add( CreatedID ) ;
            //******ソイル作成*******//

            //配置パターン
            x++ ;
            if ( x == 4 ) x = 0 ;
            if ( x == m_putPtnFlag ) {
              sym = ClsRevitUtil.ChangeTypeID( doc, sym, CreatedID, "Soil_" + m_case + "_" + m_edaNum ) ;
              ClsRevitUtil.SetTypeParameter( sym, "CASE", m_case.ToString() ) ;
              ClsRevitUtil.SetTypeParameter( sym, "枝番", m_edaNum ) ;
              x = 0 ;
              continue ;
            }
            //自動かつ入隅に芯材を配置しないかつラスト
            //if (m_way == 0 && bIrizumi && !m_bCorner && dCount - 1 <= i)
            //{
            //    sym = ClsRevitUtil.ChangeTypeID(doc, sym, CreatedID, "Soil_" + m_case + "_" + m_edaNum);
            //    ClsRevitUtil.SetTypeParameter(sym, "CASE", m_case.ToString());
            //    ClsRevitUtil.SetTypeParameter(sym, "枝番", m_edaNum);
            //    continue;
            //}

            ElementId CreatedOyaID = ClsRevitUtil.Create( doc, picPoint, levelId, oya ) ; //親杭作成
            ClsRevitUtil.SetParameter( doc, CreatedOyaID, ClsGlobal.m_refLvTop, ClsRevitUtil.CovertToAPI( m_HTop ) ) ;
            ClsVoid.SetVoidDep( doc, CreatedOyaID, m_void ) ;
            ClsKabeShin.SetKabeShinId( doc, CreatedOyaID, kabeShinId ) ;
            ClsRevitUtil.CustomDataSet( doc, CreatedOyaID, "ピッチ", m_soil ) ;

            ClsKabeShin.SetPutOrder( doc, CreatedOyaID, putPtn ) ;
            if ( putPtn == 0 )
              putPtn = 1 ;
            else
              putPtn = 0 ;

            Line axis = Line.CreateBound( picPoint, picPoint + XYZ.BasisZ ) ;
            ClsRevitUtil.RotateElement( doc, CreatedOyaID, axis, -dAngle ) ;
            selectionIds.Add( CreatedOyaID ) ;

            ////交互対応
            string edaban = m_edaNum ;
            //ジョイント数
            int nKasho = m_Kasho1 ;
            //杭長さ
            List<int> pileLengths = m_ListPileLength1 ;
            Master.Kotei kotei = m_Kotei1 ;
            //ボルト
            string boltF = m_BoltF1 ;
            string boltFNum = m_BoltFNum1 ;
            string boltW = m_BoltW1 ;
            string boltWNum = m_BoltWNum1 ;

            if ( m_KougoFlg && putPtn == 0 ) //交互時の枝番タブ２
            {
              edaban = m_edaNum2 ;
              nKasho = m_Kasho2 ;
              kotei = m_Kotei2 ;
              pileLengths = m_ListPileLength2 ;
              boltF = m_BoltF2 ;
              boltFNum = m_BoltFNum2 ;
              boltW = m_BoltW2 ;
              boltWNum = m_BoltWNum2 ;
            }

            ClsYMSUtil.SetKotei( doc, CreatedOyaID, kotei ) ;
            string koteiWay = "ﾎﾞﾙﾄ" ;
            if ( m_Kotei1 == Master.Kotei.Yousetsu )
              koteiWay = "溶接" ;
            string tugitename = familyNameKui.Replace( "杭_", "" ) + koteiWay + "継手" ;
            FamilySymbol tugiteSym = ClsRevitUtil.GetFamilySymbol( doc, tugitename, tugitename ) ;
            if ( tugiteSym != null ) {
              ClsRevitUtil.SetTypeParameter( oya, "継手", tugiteSym.Id ) ;
            }

            ClsYMSUtil.SetBoltFlange( doc, CreatedOyaID, boltF, ClsCommonUtils.ChangeStrToInt( boltFNum ) ) ;
            ClsYMSUtil.SetBoltWeb( doc, CreatedOyaID, boltW, ClsCommonUtils.ChangeStrToInt( boltWNum ) ) ;

            sym = ClsRevitUtil.ChangeTypeID( doc, sym, CreatedID, "Soil_" + m_case + "_" + edaban ) ;
            ClsRevitUtil.SetTypeParameter( sym, "CASE", m_case.ToString() ) ;
            ClsRevitUtil.SetTypeParameter( sym, "枝番", edaban ) ;
            oya = ClsRevitUtil.ChangeTypeID( doc, oya, CreatedOyaID, "SMW_" + m_case + "_" + edaban ) ;
            ClsRevitUtil.SetTypeParameter( oya, "CASE", m_case.ToString() ) ;
            ClsRevitUtil.SetTypeParameter( oya, "枝番", edaban ) ;

            ClsRevitUtil.SetTypeParameter( oya, "長さ", ClsRevitUtil.CovertToAPI( m_HLen ) ) ;
            ClsRevitUtil.SetTypeParameter( oya, "ジョイント数", nKasho ) ;
            ClsRevitUtil.SetTypeParameter( oya, "ピッチ", m_soil.ToString() ) ;
            if ( m_zanti == "一部埋め殺し" )
              ClsRevitUtil.SetTypeParameter( oya, "残置/引抜", m_zanti + "(" + m_zantiLength + ")" ) ;
            else
              ClsRevitUtil.SetTypeParameter( oya, "残置/引抜", m_zanti ) ;


            for ( int j = 0 ; j < PileJointMax ; j++ ) {
              int nPileLength = 0 ;
              if ( j < pileLengths.Count ) {
                nPileLength = pileLengths[ j ] ;
              }

              ClsRevitUtil.SetTypeParameter( oya, "杭" + ( j + 1 ).ToString(),
                ClsRevitUtil.CovertToAPI( nPileLength ) ) ;
            }
          }

          XYZ lastPoint = picPoint ;
          //ソイルピッチで描き、終点側からソイルピッチで選択された軸数分描く
          XYZ oopDir = Line.CreateBound( picPointE, picPointS ).Direction ;
          //putPtn = 0;
          for ( int i = 0 ; i < 3 ; i++ ) {
            if ( ! m_bTanbuE && i == 0 ) {
              continue ;
            }

            picPoint = new XYZ( picPointE.X + ( ClsRevitUtil.CovertToAPI( soil * i ) * oopDir.X ),
              picPointE.Y + ( ClsRevitUtil.CovertToAPI( soil * i ) * oopDir.Y ), picPointE.Z ) ;

            if ( ClsGeo.GEO_EQ( picPoint, lastPoint ) && i == 2 ) {
              continue ;
            }

            //******ソイル作成*******//
            ElementId CreatedID = ClsRevitUtil.Create( doc, picPoint, levelId, sym ) ; //ソイル作成

            ClsRevitUtil.SetParameter( doc, CreatedID, ClsGlobal.m_refLvTop, ClsRevitUtil.CovertToAPI( m_soilTop ) ) ;
            ClsRevitUtil.SetTypeParameter( sym, ClsGlobal.m_length, ClsRevitUtil.CovertToAPI( m_soilLen ) ) ;
            ClsRevitUtil.SetTypeParameter( sym, ClsGlobal.m_dia, ClsRevitUtil.CovertToAPI( m_dia ) ) ;
            ClsVoid.SetVoidDep( doc, CreatedID, m_void ) ;
            selectionIds.Add( CreatedID ) ;
            selId.Add( CreatedID ) ;
            //******ソイル作成*******//

            //配置パターン
            if ( ( x == 0 && m_putPtnFlag == 3 && i == 2 ) || i == 1 ||
                 ( m_way == 0 && bIrizumi && m_bCorner && i == 0 ) ) {
              if ( x == 0 && m_putPtnFlag == 3 && i == 1 && ! ClsGeo.GEO_EQ(
                    new XYZ( picPointE.X + ( ClsRevitUtil.CovertToAPI( soil * 2 ) * oopDir.X ),
                      picPointE.Y + ( ClsRevitUtil.CovertToAPI( soil * 2 ) * oopDir.Y ), picPointE.Z ), lastPoint ) ) {
                if ( putPtn == 0 )
                  putPtn = 1 ;
                else
                  putPtn = 0 ;
              }

              //杭作成
              ElementId CreatedOyaID = ClsRevitUtil.Create( doc, picPoint, levelId, oya ) ; //親杭作成
              ClsRevitUtil.SetParameter( doc, CreatedOyaID, ClsGlobal.m_refLvTop, ClsRevitUtil.CovertToAPI( m_HTop ) ) ;
              ClsVoid.SetVoidDep( doc, CreatedOyaID, m_void ) ;
              ClsKabeShin.SetKabeShinId( doc, CreatedOyaID, kabeShinId ) ;
              ClsRevitUtil.CustomDataSet( doc, CreatedOyaID, "ピッチ", m_soil ) ;

              ClsKabeShin.SetPutOrder( doc, CreatedOyaID, putPtn ) ;

              if ( putPtn == 0 )
                putPtn = 1 ;
              else
                putPtn = 0 ;

              Line axis = Line.CreateBound( picPoint, picPoint + XYZ.BasisZ ) ;
              ClsRevitUtil.RotateElement( doc, CreatedOyaID, axis, -dAngle ) ;
              selectionIds.Add( CreatedOyaID ) ;

              ////交互対応
              string edaban = m_edaNum ;
              //ジョイント数
              int nKasho = m_Kasho1 ;
              //杭長さ
              List<int> pileLengths = m_ListPileLength1 ;
              Master.Kotei kotei = m_Kotei1 ;
              //ボルト
              string boltF = m_BoltF1 ;
              string boltFNum = m_BoltFNum1 ;
              string boltW = m_BoltW1 ;
              string boltWNum = m_BoltWNum1 ;

              if ( m_KougoFlg && putPtn == 0 ) //交互時の枝番タブ２
              {
                edaban = m_edaNum2 ;
                nKasho = m_Kasho2 ;
                kotei = m_Kotei2 ;
                pileLengths = m_ListPileLength2 ;
                boltF = m_BoltF2 ;
                boltFNum = m_BoltFNum2 ;
                boltW = m_BoltW2 ;
                boltWNum = m_BoltWNum2 ;
              }

              ClsYMSUtil.SetKotei( doc, CreatedOyaID, kotei ) ;
              string koteiWay = "ﾎﾞﾙﾄ" ;
              if ( m_Kotei1 == Master.Kotei.Yousetsu )
                koteiWay = "溶接" ;
              string tugitename = familyNameKui.Replace( "杭_", "" ) + koteiWay + "継手" ;
              FamilySymbol tugiteSym = ClsRevitUtil.GetFamilySymbol( doc, tugitename, tugitename ) ;
              if ( tugiteSym != null ) {
                ClsRevitUtil.SetTypeParameter( oya, "継手", tugiteSym.Id ) ;
              }

              ClsYMSUtil.SetBoltFlange( doc, CreatedOyaID, boltF, ClsCommonUtils.ChangeStrToInt( boltFNum ) ) ;
              ClsYMSUtil.SetBoltWeb( doc, CreatedOyaID, boltW, ClsCommonUtils.ChangeStrToInt( boltWNum ) ) ;

              sym = ClsRevitUtil.ChangeTypeID( doc, sym, CreatedID, "Soil_" + m_case + "_" + edaban ) ;
              ClsRevitUtil.SetTypeParameter( sym, "CASE", m_case.ToString() ) ;
              ClsRevitUtil.SetTypeParameter( sym, "枝番", edaban ) ;
              oya = ClsRevitUtil.ChangeTypeID( doc, oya, CreatedOyaID, "SMW_" + m_case + "_" + edaban ) ;
              ClsRevitUtil.SetTypeParameter( oya, "CASE", m_case.ToString() ) ;
              ClsRevitUtil.SetTypeParameter( oya, "枝番", edaban ) ;

              ClsRevitUtil.SetTypeParameter( oya, "長さ", ClsRevitUtil.CovertToAPI( m_HLen ) ) ;
              ClsRevitUtil.SetTypeParameter( oya, "ジョイント数", nKasho ) ;
              ClsRevitUtil.SetTypeParameter( oya, "ピッチ", m_soil.ToString() ) ;
              if ( m_zanti == "一部埋め殺し" )
                ClsRevitUtil.SetTypeParameter( oya, "残置/引抜", m_zanti + "(" + m_zantiLength + ")" ) ;
              else
                ClsRevitUtil.SetTypeParameter( oya, "残置/引抜", m_zanti ) ;


              for ( int j = 0 ; j < PileJointMax ; j++ ) {
                int nPileLength = 0 ;
                if ( j < pileLengths.Count ) {
                  nPileLength = pileLengths[ j ] ;
                }

                ClsRevitUtil.SetTypeParameter( oya, "杭" + ( j + 1 ).ToString(),
                  ClsRevitUtil.CovertToAPI( nPileLength ) ) ;
              }
            }
            else {
              //if (putPtn == 0)
              //    putPtn = 1;
              //else
              //    putPtn = 0;
              ////交互対応
              string edaban = m_edaNum ;

              if ( m_KougoFlg && putPtn == 0 ) //交互時の枝番タブ２
              {
                edaban = m_edaNum2 ;
              }

              sym = ClsRevitUtil.ChangeTypeID( doc, sym, CreatedID, "Soil_" + m_case + "_" + edaban ) ;
              ClsRevitUtil.SetTypeParameter( sym, "CASE", m_case.ToString() ) ;
              ClsRevitUtil.SetTypeParameter( sym, "枝番", edaban ) ;
            }
          }
          //始端に杭なしソイルを配置する

          for ( int i = 0 ; i < kuiNoSoilCountS ; i++ ) {
            if ( ! m_bTanbuS && i == 0 ) {
              continue ;
            }

            picPoint = new XYZ( originalPointS.X + ( ClsRevitUtil.CovertToAPI( soil * i ) * soilDirection.X ),
              originalPointS.Y + ( ClsRevitUtil.CovertToAPI( soil * i ) * soilDirection.Y ),
              originalPointS.Z + ( ClsRevitUtil.CovertToAPI( soil * i ) * soilDirection.Z ) ) ;

            //******ソイル作成*******//
            ElementId CreatedID = ClsRevitUtil.Create( doc, picPoint, levelId, sym ) ; //ソイル作成

            ClsRevitUtil.SetParameter( doc, CreatedID, ClsGlobal.m_refLvTop, ClsRevitUtil.CovertToAPI( m_soilTop ) ) ;
            ClsRevitUtil.SetTypeParameter( sym, ClsGlobal.m_length, ClsRevitUtil.CovertToAPI( m_soilLen ) ) ;
            ClsRevitUtil.SetTypeParameter( sym, ClsGlobal.m_dia, ClsRevitUtil.CovertToAPI( m_dia ) ) ;
            ClsVoid.SetVoidDep( doc, CreatedID, m_void ) ;
            selectionIds.Add( CreatedID ) ;
            selId.Add( CreatedID ) ;
            //******ソイル作成*******//

            if ( putPtn == 0 )
              putPtn = 1 ;
            else
              putPtn = 0 ;
            ////交互対応
            string edaban = m_edaNum ;

            if ( m_KougoFlg && putPtn == 0 ) //交互時の枝番タブ２
            {
              edaban = m_edaNum2 ;
            }

            sym = ClsRevitUtil.ChangeTypeID( doc, sym, CreatedID, "Soil_" + m_case + "_" + edaban ) ;
            ClsRevitUtil.SetTypeParameter( sym, "CASE", m_case.ToString() ) ;
            ClsRevitUtil.SetTypeParameter( sym, "枝番", edaban ) ;
          }

          //補助線の作成
          double dH = ClsRevitUtil.CovertToAPI( dEx ) ;
          try {
            outLine = CreateKabeHojyoLine( doc, originalPointS, picPointE, dH,
              ClsRevitUtil.CovertToAPI( m_dia ) ) ; // / 2));//外側
            inLine = CreateKabeHojyoLine( doc, originalPointS, picPointE, -dH,
              ClsRevitUtil.CovertToAPI( m_dia ) ) ; // / 2));//内側

            ClsVoid.SetVoidHojyoLine( doc, outLine.Id, ClsVoid.OutVoidLine ) ;
            ClsVoid.SetVoidHojyoLine( doc, inLine.Id, ClsVoid.InVoidLine ) ;
          }
          catch {
          }

          //変更が加わるメッセージ原因不明
          t.Commit() ;
        }

        if ( outLine != null && inLine != null ) {
          outLine = ClsVoid.CreateAdjustVoidLine( doc, outLine.Id, ClsVoid.OutVoidLine ) ;
          inLine = ClsVoid.CreateAdjustVoidLine( doc, inLine.Id, ClsVoid.InVoidLine ) ;
          ModelLine line = inLine ;
          if ( ! defVoid )
            line = outLine ;
          //壁くり抜き処理
          if ( m_bVoid ) ClsVoid.DoVoid( doc, selId, line, m_void, m_dia / 2, levelId ) ;
        }
      }
      catch ( Exception ex ) {
        TaskDialog.Show( "Info Message21", ex.Message ) ;
        string message = ex.Message ;
        MessageBox.Show( message ) ;
      }

      return true ;
    }

    /// <summary>
    /// SMWを作図する
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="picPointS"></param>
    /// <param name="picPointE"></param>
    /// <param name="soilDirection"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public bool CreateSingleSMW( Document doc, XYZ picPointS, XYZ soilDirection, ElementId levelId,
      bool bIrizumi = true, bool bSoil = true, bool bH = true, ElementId kabeShinId = null, double apexAngle = 0.0 )
    {
      try {
        string symbolFolpath = ClsZumenInfo.GetYMSFolder() ;

        string familyPathSoil = System.IO.Path.Combine( symbolFolpath, "山留壁関係\\05_ｿｲﾙ\\ｿｲﾙ_単軸.rfa" ) ;
        string familyNameSoil = "ｿｲﾙ_単軸" ;

        string familyPathKui = Master.ClsHBeamCsv.GetPileFamilyPath( m_size ) ;
        string familyNameKui = ClsRevitUtil.GetFamilyName( familyPathKui ) ;

        //シンボル配置
        if ( ! ClsRevitUtil.LoadFamilySymbolData( doc, familyPathSoil, familyNameSoil, out FamilySymbol sym ) ) {
          return false ;
        }

        //親杭配置
        //if (!ClsRevitUtil.LoadFamilyData(doc, familyPathKui, out Family oyaFam))
        //{
        //    return false;
        //}
        //FamilySymbol oya = ClsRevitUtil.GetFamilySymbol(doc, familyNameKui, "SMW");
        if ( ! ClsRevitUtil.LoadFamilySymbolData( doc, familyPathKui, "SMW", out FamilySymbol oya ) ) {
          return false ;
        }

        //H形鋼
        //ClsRevitUtil.SetTypeParameter(oya, ClsGlobal.m_length, ClsRevitUtil.CovertToAPI(m_HLen));
        string stH = ClsYMSUtil.GetKouzaiSizeSunpou1( familyNameKui, 1 ) ;
        double dEx = ClsCommonUtils.ChangeStrToDbl( stH ) / 2 ;

        string stB = ClsYMSUtil.GetKouzaiSizeSunpou1( familyNameKui, 2 ) ;
        int dFlangeWidth = ClsCommonUtils.ChangeStrToInt( stB ) / 2 ;
        int flangeAtsu = ClsCommonUtils.ChangeStrToInt( ClsYMSUtil.GetKouzaiSizeSunpou1( familyNameKui, 4 ) ) ;

        double dAngle = soilDirection.AngleOnPlaneTo( XYZ.BasisX, XYZ.BasisZ ) ;

        ////交互対応
        string edaban = m_edaNum ;
        //ジョイント数
        int nKasho = m_Kasho1 ;
        //杭長さ
        List<int> pileLengths = m_ListPileLength1 ;

        //グループ化するIDを保持する変数の宣言(nullだとAdd出来ないため)
        ICollection<ElementId> selectionIds = new List<ElementId>() ;
        List<ElementId> selId = new List<ElementId>() ; //ボイド用
        ModelLine outLine = null, inLine = null ;

        using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
          t.Start() ;
          FailureHandlingOptions failOpt = t.GetFailureHandlingOptions() ;
          failOpt.SetFailuresPreprocessor( new WarningSwallower() ) ;
          t.SetFailureHandlingOptions( failOpt ) ;

          //ダイアログ指定の値をSET
          ClsRevitUtil.SetTypeParameter( oya, "切断長さ", 0.0 ) ;
          ClsRevitUtil.SetTypeParameter( oya, "トッププレート", 0 ) ;

          ElementId CreatedID = null ;
          if ( bSoil ) {
            //******ソイル作成*******//
            CreatedID = ClsRevitUtil.Create( doc, picPointS, levelId, sym ) ; //ソイル作成

            ClsRevitUtil.SetParameter( doc, CreatedID, ClsGlobal.m_refLvTop, ClsRevitUtil.CovertToAPI( m_soilTop ) ) ;
            ClsRevitUtil.SetTypeParameter( sym, ClsGlobal.m_length, ClsRevitUtil.CovertToAPI( m_soilLen ) ) ;
            ClsRevitUtil.SetTypeParameter( sym, ClsGlobal.m_dia, ClsRevitUtil.CovertToAPI( m_dia ) ) ;
            ClsVoid.SetVoidDep( doc, CreatedID, m_void ) ;
            selectionIds.Add( CreatedID ) ;
            selId.Add( CreatedID ) ;
            sym = ClsRevitUtil.ChangeTypeID( doc, sym, CreatedID, "Soil_" + m_case + "_" + m_edaNum ) ;
            ClsRevitUtil.SetTypeParameter( sym, "CASE", m_case.ToString() ) ;
            ClsRevitUtil.SetTypeParameter( sym, "枝番", edaban ) ;
            //******ソイル作成*******//
          }

          if ( bH ) {
            ElementId CreatedOyaID = ClsRevitUtil.Create( doc, picPointS, levelId, oya ) ; //親杭作成
            ClsRevitUtil.SetParameter( doc, CreatedOyaID, ClsGlobal.m_refLvTop, ClsRevitUtil.CovertToAPI( m_HTop ) ) ;
            ClsVoid.SetVoidDep( doc, CreatedOyaID, m_void ) ;
            ClsKabeShin.SetPutOrder( doc, CreatedOyaID, m_oreder ) ;
            ClsKabeShin.SetKabeShinId( doc, CreatedOyaID, kabeShinId ) ;

            Line axis = Line.CreateBound( picPointS, picPointS + XYZ.BasisZ ) ;
            ClsRevitUtil.RotateElement( doc, CreatedOyaID, axis, -dAngle ) ;
            selectionIds.Add( CreatedOyaID ) ;

            FamilyInstance inst = doc.GetElement( CreatedOyaID ) as FamilyInstance ;
            if ( ! bIrizumi ) //出隅杭
            {
              double oneSide =
                ClsRevitUtil.CalculateIsoscelesTriangleOneSide( dFlangeWidth * 2, apexAngle, out double baseAngle ) ;
              oneSide = ClsRevitUtil.CalculateTriangleOneSide( oneSide, Math.PI / 2, baseAngle ) ;
              ClsRevitUtil.MoveFamilyInstance( inst, -( oneSide + dEx ), inst.FacingOrientation ) ; // - flangeAtsu
              //ClsRevitUtil.MoveFamilyInstance(inst, -dEx, inst.FacingOrientation);
              picPointS = ( inst.Location as LocationPoint ).Point ;
            }

            if ( ! bIrizumi && bSoil && CreatedID != null ) //出隅ソイル
            {
              FamilyInstance instSoil = doc.GetElement( CreatedID ) as FamilyInstance ;
              double oneSide =
                ClsRevitUtil.CalculateIsoscelesTriangleOneSide( dFlangeWidth * 2, apexAngle, out double baseAngle ) ;
              oneSide = ClsRevitUtil.CalculateTriangleOneSide( oneSide, Math.PI / 2, baseAngle ) ;
              ClsRevitUtil.MoveFamilyInstance( instSoil, -( oneSide + dEx ), inst.FacingOrientation ) ; // -flangeAtsu
              ////ClsRevitUtil.MoveFamilyInstance(instSoil, -dEx, inst.FacingOrientation);
            }

            //ボルト
            string boltF = m_BoltF1 ;
            string boltFNum = m_BoltFNum1 ;
            string boltW = m_BoltW1 ;
            string boltWNum = m_BoltWNum1 ;
            Master.Kotei kotei = m_Kotei1 ;

            ClsYMSUtil.SetKotei( doc, CreatedOyaID, kotei ) ;
            string koteiWay = "ﾎﾞﾙﾄ" ;
            if ( m_Kotei1 == Master.Kotei.Yousetsu )
              koteiWay = "溶接" ;
            string tugitename = familyNameKui.Replace( "杭_", "" ) + koteiWay + "継手" ;
            FamilySymbol tugiteSym = ClsRevitUtil.GetFamilySymbol( doc, tugitename, tugitename ) ;
            if ( tugiteSym != null ) {
              ClsRevitUtil.SetTypeParameter( oya, "継手", tugiteSym.Id ) ;
            }

            ClsYMSUtil.SetBoltFlange( doc, CreatedOyaID, boltF, ClsCommonUtils.ChangeStrToInt( boltFNum ) ) ;
            ClsYMSUtil.SetBoltWeb( doc, CreatedOyaID, boltW, ClsCommonUtils.ChangeStrToInt( boltWNum ) ) ;

            oya = ClsRevitUtil.ChangeTypeID( doc, oya, CreatedOyaID, "SMW_" + m_case + "_" + edaban ) ;
            ClsRevitUtil.SetTypeParameter( oya, "CASE", m_case.ToString() ) ;
            ClsRevitUtil.SetTypeParameter( oya, "枝番", edaban ) ;

            ClsRevitUtil.SetTypeParameter( oya, "長さ", ClsRevitUtil.CovertToAPI( m_HLen ) ) ;
            ClsRevitUtil.SetTypeParameter( oya, "ジョイント数", nKasho ) ;
            ClsRevitUtil.SetTypeParameter( oya, "ピッチ", m_soil.ToString() ) ;
            if ( m_zanti == "一部埋め殺し" )
              ClsRevitUtil.SetTypeParameter( oya, "残置/引抜", m_zanti + "(" + m_zantiLength + ")" ) ;
            else
              ClsRevitUtil.SetTypeParameter( oya, "残置/引抜", m_zanti ) ;


            for ( int j = 0 ; j < PileJointMax ; j++ ) {
              int nPileLength = 0 ;
              if ( j < pileLengths.Count ) {
                nPileLength = pileLengths[ j ] ;
              }

              ClsRevitUtil.SetTypeParameter( oya, "杭" + ( j + 1 ).ToString(),
                ClsRevitUtil.CovertToAPI( nPileLength ) ) ;
            }

            ////補助線の作成
            double dH = ClsRevitUtil.CovertToAPI( dEx ) ;
            outLine = CreatePointHojyoLine( doc, picPointS, -inst.FacingOrientation, soilDirection, dH,
              ClsRevitUtil.CovertToAPI( m_dia / 2 ) ) ; //外側
            inLine = CreatePointHojyoLine( doc, picPointS, -inst.FacingOrientation, soilDirection, -dH,
              ClsRevitUtil.CovertToAPI( m_dia / 2 ) ) ; //内側
          }

          //変更が加わるメッセージ原因不明
          t.Commit() ;
        }

        if ( outLine != null && inLine != null )
          ////壁くり抜き処理
          if ( m_bVoid )
            ClsVoid.DoVoid( doc, selId, inLine, m_void, m_dia / 2, levelId ) ;
      }
      catch ( Exception ex ) {
        TaskDialog.Show( "Info Message22", ex.Message ) ;
        string message = ex.Message ;
        MessageBox.Show( message ) ;
      }

      return true ;
    }

    /// <summary>
    /// 壁に補助線を作成
    /// </summary>
    /// <param name="doc">ドキュメント</param>
    /// <param name="pointS">開始位置</param>
    /// <param name="pointE">終了位置</param>
    /// <param name="dH">H形鋼サイズ/2(+:外側, -:内側)</param>
    /// <param name="dEx">補助線の延長</param>
    /// <returns>補助線</returns>
    public static ModelLine CreateKabeHojyoLine( Document doc, XYZ pointS, XYZ pointE, double dH, double dEx )
    {
      XYZ Direction = Line.CreateBound( pointS, pointE ).Direction ;
      //補助線の作成
      XYZ hojyoS = new XYZ( pointS.X + ( dH * Direction.Y ) - ( dEx * Direction.X ),
        pointS.Y - ( dH * Direction.X ) - ( dEx * Direction.Y ), pointS.Z ) ;
      XYZ hojyoE = new XYZ( pointE.X + ( dH * Direction.Y ) + ( dEx * Direction.X ),
        pointE.Y - ( dH * Direction.X ) + ( dEx * Direction.Y ), pointE.Z ) ;
      Line hojyo = Line.CreateBound( hojyoS, hojyoE ) ;
      if ( dH > 0 ) //補助線の内外を判別するためにDirectionを反対にする
      {
        hojyo = Line.CreateBound( hojyoE, hojyoS ) ;
      }

      XYZ mid = 0.5 * ( hojyoS + hojyoE ) ;
      //作成した壁に補助線を引く
      Plane plane = Plane.CreateByNormalAndOrigin( XYZ.BasisZ, mid ) ;
      SketchPlane sketchPlane = SketchPlane.Create( doc, plane ) ;
      ModelLine modelLine = doc.Create.NewModelCurve( hojyo, sketchPlane ) as ModelLine ;
      return modelLine ;
    }

    /// <summary>
    /// 壁に補助線を作成
    /// </summary>
    /// <param name="doc">ドキュメント</param>
    /// <param name="pointS">開始位置</param>
    /// <param name="pointE">終了位置</param>
    /// <param name="dH">H形鋼サイズ/2(+:外側, -:内側)</param>
    /// <param name="dEx">補助線の延長</param>
    /// <returns>補助線</returns>
    public static ModelLine CreatePointHojyoLine( Document doc, XYZ pointS, XYZ dirH, XYZ Direction, double dH,
      double dEx )
    {
      //補助線の作成
      XYZ hojyoS = new XYZ( pointS.X + ( dH * dirH.X ) - ( dEx * Direction.X ),
        pointS.Y + ( dH * dirH.Y ) - ( dEx * Direction.Y ), pointS.Z ) ;
      XYZ hojyoE = new XYZ( pointS.X + ( dH * dirH.X ) + ( dEx * Direction.X ),
        pointS.Y + ( dH * dirH.Y ) + ( dEx * Direction.Y ), pointS.Z ) ;
      Line hojyo = Line.CreateBound( hojyoS, hojyoE ) ;
      if ( dH > 0 ) //補助線の内外を判別するためにDirectionを反対にする
      {
        hojyo = Line.CreateBound( hojyoE, hojyoS ) ;
      }

      XYZ mid = 0.5 * ( hojyoS + hojyoE ) ;
      //作成した壁に補助線を引く
      Plane plane = Plane.CreateByNormalAndOrigin( XYZ.BasisZ, mid ) ;
      SketchPlane sketchPlane = SketchPlane.Create( doc, plane ) ;
      ModelLine modelLine = doc.Create.NewModelCurve( hojyo, sketchPlane ) as ModelLine ;
      return modelLine ;
    }

    /// <summary>
    /// 指定したIDからクラスデータを取得する
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public void SetParameter( Document doc, ElementId id )
    {
      FamilyInstance inst = doc.GetElement( id ) as FamilyInstance ;
      FamilySymbol sym = inst.Symbol ;

      m_case = ClsCommonUtils.ChangeStrToInt( ClsRevitUtil.GetTypeParameterString( sym, "CASE" ) ) ;
      m_edaNum = ClsRevitUtil.GetTypeParameterString( sym, "枝番" ) ;

      //m_way = 0;
      m_void = (int) ClsVoid.GetVoidDep( doc, id ) ;
      //m_refPDist = 0;
      //m_putPitch = 0;
      m_type = Master.ClsHBeamCsv.GetTypePileFamilyPathInName( sym.FamilyName ) ;
      m_size = Master.ClsHBeamCsv.GetSizePileFamilyPathInName( sym.FamilyName ) ;
      m_HTop = (int) ClsRevitUtil.CovertFromAPI( ClsRevitUtil.GetParameterDouble( doc, id, ClsGlobal.m_refLvTop ) ) ;
      m_HLen = (int) ClsRevitUtil.CovertFromAPI( ClsRevitUtil.GetTypeParameter( sym, ClsGlobal.m_length ) ) ;
      m_zanti = ClsRevitUtil.GetTypeParameterString( sym, "残置/引抜" ) ;
      if ( m_zanti.Contains( "一部埋め殺し" ) ) {
        string[] split = m_zanti.Split( '(' ) ;
        m_zanti = split[ 0 ] ;
        string[] split1 = split[ 1 ].Split( ')' ) ;
        m_zantiLength = split1[ 0 ] ;
      }

      m_soil = ClsCommonUtils.ChangeStrToInt( ClsRevitUtil.GetTypeParameterString( sym, "ピッチ" ) ) ;
      m_zaishitu = ClsRevitUtil.GetTypeParameterString( sym, "材質" ) ;
      //m_KougoFlg = false;
      //ソイル
      List<ElementId> soilList = GetAllSoilList( doc ) ;
      foreach ( ElementId soilId in soilList ) {
        FamilyInstance soilInst = doc.GetElement( soilId ) as FamilyInstance ;
        FamilySymbol soilSym = soilInst.Symbol ;
        int CASE = ClsCommonUtils.ChangeStrToInt( ClsRevitUtil.GetTypeParameterString( soilSym, "CASE" ) ) ;
        string edaNum = ClsRevitUtil.GetTypeParameterString( soilSym, "枝番" ) ;
        //ソイルのCASEと枝番号が一致するものからデータを取得する
        if ( m_case == CASE && m_edaNum == edaNum ) {
          m_dia = (int) ClsRevitUtil.CovertFromAPI( ClsRevitUtil.GetTypeParameter( soilSym, ClsGlobal.m_dia ) ) ;
          //m_soil = 0;
          m_soilTop = (int) ClsRevitUtil.CovertFromAPI(
            ClsRevitUtil.GetParameterDouble( doc, soilId, ClsGlobal.m_refLvTop ) ) ;
          m_soilLen = (int) ClsRevitUtil.CovertFromAPI( ClsRevitUtil.GetTypeParameter( soilSym, ClsGlobal.m_length ) ) ;
          break ;
        }
      }

      //m_Kasho2 = 0;
      m_ListPileLength1 = new List<int>() ;
      for ( int i = 0 ; i < PileJointMax ; i++ ) {
        int kuiLength =
          (int) ClsRevitUtil.CovertFromAPI( ClsRevitUtil.GetTypeParameter( sym, "杭" + ( i + 1 ).ToString() ) ) ;
        if ( kuiLength <= 0 )
          break ;
        m_ListPileLength1.Add( kuiLength ) ;
      }

      m_Kasho1 = m_ListPileLength1.Count() - 1 ;
      //m_ListPileLength2 = new List<int>();

      m_oreder = ClsKabeShin.GetPutOrder( doc, id ) ;

      //固定方法
      var kotei = ClsYMSUtil.GetKotei( doc, id ) ;
      m_Kotei1 = ( kotei == Master.ClsPileTsugiteCsv.KoteiHohoBolt ? Master.Kotei.Bolt : Master.Kotei.Yousetsu ) ;
      ( var BoltF1, int BoltFNum1 ) = ClsYMSUtil.GetBoltFlange( doc, id ) ;
      m_BoltF1 = BoltF1 ;
      m_BoltFNum1 = BoltFNum1.ToString() ;
      ( var BoltW1, int BoltWNum1 ) = ClsYMSUtil.GetBoltWeb( doc, id ) ;
      m_BoltW1 = BoltW1 ;
      m_BoltWNum1 = BoltWNum1.ToString() ;

      return ;
    }

    public static void ChangeSoil( Document doc, ElementId id, string CASE, string edaNum, int soilLen, int dia )
    {
      FamilyInstance inst = doc.GetElement( id ) as FamilyInstance ;
      FamilySymbol sym = inst.Symbol ;
      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        t.Start() ;
        FailureHandlingOptions failOpt = t.GetFailureHandlingOptions() ;
        failOpt.SetFailuresPreprocessor( new WarningSwallower() ) ;
        t.SetFailureHandlingOptions( failOpt ) ;
        sym = ClsRevitUtil.ChangeTypeID( doc, sym, id, "Soil_" + CASE + "_" + edaNum ) ;

        ClsRevitUtil.SetTypeParameter( sym, "CASE", CASE ) ;
        ClsRevitUtil.SetTypeParameter( sym, "枝番", edaNum ) ;

        ClsRevitUtil.SetTypeParameter( sym, ClsGlobal.m_length, ClsRevitUtil.CovertToAPI( soilLen ) ) ;
        ClsRevitUtil.SetTypeParameter( sym, ClsGlobal.m_dia, ClsRevitUtil.CovertToAPI( dia ) ) ;
        t.Commit() ;
      }
    }

    /// <summary>
    /// SMW のみを単独選択
    /// </summary>
    /// <param name="uidoc"></param>
    /// <param name="id">選択した SMW のID</param>
    /// <param name="message">指示</param>
    /// <returns></returns>
    public static bool PickObject( UIDocument uidoc, ref ElementId id, string message = SMW )
    {
      return ClsRevitUtil.PickObjectPartFilter( uidoc, message + "を選択してください", SMW, ref id ) ;
    }

    /// <summary>
    /// SMW のみを複数選択
    /// </summary>
    /// <param name="uidoc"></param>
    /// <param name="id">選択した SMW のID</param>
    /// <param name="message">指示</param>
    /// <returns></returns>
    public static bool PickObjects( UIDocument uidoc, ref List<ElementId> ids, string message = SMW )
    {
      return ClsRevitUtil.PickObjectsPartFilter( uidoc, message + "を選択してください", SMW, ref ids ) ;
    }

    /// <summary>
    /// 図面上のソイルを全て取得する
    /// </summary>
    /// <param name="doc"></param>
    /// <returns></returns>
    public static List<ElementId> GetAllSoilList( Document doc )
    {
      //図面上のソイルを全て取得
      List<ElementId> soilIdList = ClsRevitUtil.GetSelectCreatedFamilyInstanceList( doc, "Soil", true ) ;
      return soilIdList ;
    }

    /// <summary>
    /// 図面上のSMWを全て取得
    /// </summary>
    /// <param name="doc"></param>
    /// <returns></returns>
    public static List<ElementId> GetAllSMWList( Document doc )
    {
      List<ElementId> smwIdList = ClsRevitUtil.GetSelectCreatedFamilyInstanceList( doc, "SMW", true ) ;
      return smwIdList ;
    }

    #endregion
  }
}