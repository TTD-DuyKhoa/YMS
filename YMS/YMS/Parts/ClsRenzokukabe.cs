using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;
using YMS.Master ;

namespace YMS.Parts
{
  public class ClsRenzokukabe
  {
    #region 定数

    /// <summary>
    /// 最大ジョイント数
    /// </summary>
    public const int PileJointMax = 10 ;

    public const string RENZOKUKABE = "連続壁" ;
    public const string RENZOKUKUI = "連続杭" ;

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
    /// 連続壁厚み
    /// </summary>
    public int m_kabeAtumi { get ; set ; }

    /// <summary>
    /// 全長
    /// </summary>
    public int m_kabeLen { get ; set ; }

    /// <summary>
    /// くり抜きフラグ
    /// </summary>
    public bool m_bVoid { get ; set ; }

    /// <summary>
    /// 芯材：タイプ
    /// </summary>
    public string m_type { get ; set ; }

    /// <summary>
    /// 芯材：サイズ
    /// </summary>
    public string m_size { get ; set ; }

    /// <summary>
    /// 芯材：天端
    /// </summary>
    public int m_HTop { get ; set ; }

    /// <summary>
    /// 芯材：全長
    /// </summary>
    public int m_HLen { get ; set ; }

    /// <summary>
    /// 芯材：ピッチ
    /// </summary>
    public int m_pitch { get ; set ; }

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

    public ClsRenzokukabe()
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
      m_kabeAtumi = 0 ;
      m_kabeLen = 0 ;
      m_bVoid = false ;
      m_size = string.Empty ;
      m_HTop = 0 ;
      m_HLen = 0 ;
      m_pitch = 0 ;
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
    /// Renzokukabeを作図する
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="picPointS"></param>
    /// <param name="picPointE"></param>
    /// <param name="soilDirection"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public bool CreateRenzokukabe( Document doc, XYZ picPointS, XYZ picPointE, ElementId levelId, bool bIrizumi = true,
      ElementId kabeShinId = null, bool defVoid = true )
    {
      try {
        Line soilLine = Line.CreateBound( picPointS, picPointE ) ;

        XYZ soilDirection = soilLine.Direction ; //単位ベクトル
        double length = ClsRevitUtil.CovertFromAPI( soilLine.Length ) ; //芯の長さ

        string symbolFolpath = ClsZumenInfo.GetYMSFolder() ;

        string familyPathKabe =
          System.IO.Path.Combine( symbolFolpath, "山留壁関係\\08_" + RENZOKUKABE + "\\" + RENZOKUKABE + ".rfa" ) ;
        string familyNameKabe = RENZOKUKABE ;

        //シンボル配置
        //if (!ClsRevitUtil.RoadFamilySymbolData(doc, familyPathKabe, familyNameKabe, out FamilySymbol sym))
        //{
        //    return false;
        //}
        if ( ! ClsRevitUtil.LoadFamilyData( doc, familyPathKabe, out Family symFam ) ) {
          return false ;
        }

        FamilySymbol sym = ClsRevitUtil.GetFamilySymbol( doc, familyNameKabe, RENZOKUKABE ) ;

        string familyPathKui = Master.ClsHBeamCsv.GetPileFamilyPath( m_size ) ;
        string familyNameKui = ClsRevitUtil.GetFamilyName( familyPathKui ) ;

        //親杭配置

        //#31788

        //if (!ClsRevitUtil.RoadFamilyData(doc, familyPathKui, familyNameKui, out Family oyaFam))
        //{
        //    return false;
        //}
        //FamilySymbol oya = ClsRevitUtil.GetFamilySymbol(doc, familyNameKui, RENZOKUKABE);

        if ( ! ClsRevitUtil.LoadFamilySymbolData( doc, familyPathKui, RENZOKUKABE, out FamilySymbol oya ) ) {
          return false ;
        }

        string stB = ClsYMSUtil.GetKouzaiSizeSunpou1( familyNameKui, 2 ) ;
        int dFlangeWidth = ClsCommonUtils.ChangeStrToInt( stB ) / 2 ;

        int refPDist = m_refPDist ; // + dFlangeWidth;
        int pitch = m_pitch ;
        //基準点から指定の数値離す
        picPointS = new XYZ( picPointS.X + ( ClsRevitUtil.CovertToAPI( refPDist ) * soilDirection.X ),
          picPointS.Y + ( ClsRevitUtil.CovertToAPI( refPDist ) * soilDirection.Y ), picPointS.Z ) ;
        length -= refPDist ;

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

        double dCount = length / pitch ; //ダイアログソイルピッチずつ親杭、ソイルを配置するため何個置けるか
        ModelLine outLine = null, inLine = null ;

        using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
          t.Start() ;
          FailureHandlingOptions failOpt = t.GetFailureHandlingOptions() ;
          failOpt.SetFailuresPreprocessor( new WarningSwallower() ) ;
          t.SetFailureHandlingOptions( failOpt ) ;
          if ( kabeShinId != null )
            ClsRevitUtil.SetMojiParameter( doc, kabeShinId, "壁", RENZOKUKABE ) ;
          //ダイアログ指定の値をSET
          ClsRevitUtil.SetTypeParameter( oya, "切断長さ", 0.0 ) ;
          ClsRevitUtil.SetTypeParameter( oya, "トッププレート", 0 ) ;

          int putPtn = 0 ;
          for ( int i = 0 ; i < dCount ; i++ ) {
            if ( ! bIrizumi && i == 0 ) {
              continue ;
            } //出隅の時は別で出隅分作成するため始点を飛ばす

            picPoint = new XYZ( picPointS.X + ( ClsRevitUtil.CovertToAPI( pitch * i ) * soilDirection.X ),
              picPointS.Y + ( ClsRevitUtil.CovertToAPI( pitch * i ) * soilDirection.Y ),
              picPointS.Z + ( ClsRevitUtil.CovertToAPI( pitch * i ) * soilDirection.Z ) ) ;


            ElementId CreatedOyaID = ClsRevitUtil.Create( doc, picPoint, levelId, oya ) ; //親杭作成
            ClsRevitUtil.SetParameter( doc, CreatedOyaID, ClsGlobal.m_refLvTop, ClsRevitUtil.CovertToAPI( m_HTop ) ) ;
            ClsVoid.SetVoidDep( doc, CreatedOyaID, m_void ) ;
            ClsKabeShin.SetKabeShinId( doc, CreatedOyaID, kabeShinId ) ;
            ClsRevitUtil.CustomDataSet( doc, CreatedOyaID, "ピッチ", m_pitch ) ;

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
            Kotei kotei = m_Kotei1 ;
            //杭長さ
            List<int> pileLengths = m_ListPileLength1 ;
            //ボルト
            string boltF = m_BoltF1 ;
            string boltFNum = m_BoltFNum1 ;
            string boltW = m_BoltW1 ;
            string boltWNum = m_BoltWNum1 ;

            if ( m_KougoFlg && ( i % 2 ) == 1 ) //交互時の枝番タブ２
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
              ClsRevitUtil.SetTypeParameter( oya, "トッププレート種類", tugiteSym.Id ) ;
            }

            ClsYMSUtil.SetBoltFlange( doc, CreatedOyaID, boltF, ClsCommonUtils.ChangeStrToInt( boltFNum ) ) ;
            ClsYMSUtil.SetBoltWeb( doc, CreatedOyaID, boltW, ClsCommonUtils.ChangeStrToInt( boltWNum ) ) ;

            oya = ClsRevitUtil.ChangeTypeID( doc, oya, CreatedOyaID, RENZOKUKUI + "_" + m_case + "_" + edaban ) ;
            ClsRevitUtil.SetTypeParameter( oya, "CASE", m_case.ToString() ) ;
            ClsRevitUtil.SetTypeParameter( oya, "枝番", edaban ) ;

            ClsRevitUtil.SetTypeParameter( oya, "ジョイント数", nKasho ) ;
            ClsRevitUtil.SetTypeParameter( oya, "ピッチ", m_pitch.ToString() ) ;
            if ( m_zanti == "一部埋め殺し" )
              ClsRevitUtil.SetTypeParameter( oya, "残置/引抜", m_zanti + "(" + m_zantiLength + ")" ) ;
            else
              ClsRevitUtil.SetTypeParameter( oya, "残置/引抜", m_zanti ) ;

            //杭長さ
            ClsRevitUtil.SetTypeParameter( oya, "長さ", ClsRevitUtil.CovertToAPI( m_HLen ) ) ;

            for ( int j = 0 ; j < PileJointMax ; j++ ) {
              int nPileLength = 0 ;
              if ( j < pileLengths.Count ) {
                nPileLength = pileLengths[ j ] ;
              }

              ClsRevitUtil.SetTypeParameter( oya, "杭" + ( j + 1 ).ToString(),
                ClsRevitUtil.CovertToAPI( nPileLength ) ) ;
            }
          }

          //*******壁作成*******//
          Curve wallCV = Line.CreateBound( picPointS, picPointE ) ;
          ElementId CreatedID = ClsRevitUtil.Create( doc, wallCV, levelId, sym ) ; //連続壁
          sym = ClsRevitUtil.ChangeTypeID( doc, sym, CreatedID, RENZOKUKABE + "_" + m_case ) ; // + "_" + m_edaNum);

          ClsRevitUtil.SetTypeParameter( sym, "CASE", m_case.ToString() ) ;
          ClsRevitUtil.SetTypeParameter( sym, "W", ClsRevitUtil.CovertToAPI( m_kabeAtumi ) ) ;
          ClsRevitUtil.SetTypeParameter( sym, "H", ClsRevitUtil.CovertToAPI( m_kabeLen ) ) ;
          ClsVoid.SetVoidDep( doc, CreatedID, m_void ) ;
          selId.Add( CreatedID ) ;
          //ClsRevitUtil.SetParameter(doc, CreatedID, "指定高さ", ClsRevitUtil.CovertToAPI(m_kabeLen));
          //ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベル オフセット", ClsRevitUtil.CovertToAPI(-m_kabeLen));
          //*******壁作成*******//

          //補助線の作成
          double dH = ClsRevitUtil.CovertToAPI( dEx ) ;
          outLine = CreateKabeHojyoLine( doc, picPointS, picPointE, dH, ClsRevitUtil.CovertToAPI( dEx / 2 ) ) ; //外側
          inLine = CreateKabeHojyoLine( doc, picPointS, picPointE, -dH, ClsRevitUtil.CovertToAPI( dEx / 2 ) ) ; //内側

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
          if ( m_bVoid ) ClsVoid.DoVoid( doc, selId, line, m_void, m_kabeAtumi / 2, levelId ) ;
        }
      }
      catch ( Exception ex ) {
        TaskDialog.Show( "Info Message19", ex.Message ) ;
        string message = ex.Message ;
        MessageBox.Show( message ) ;
      }

      return true ;
    }

    /// <summary>
    /// Renzokukabeを作図する
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="picPointS"></param>
    /// <param name="picPointE"></param>
    /// <param name="soilDirection"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public bool CreateSingleRenzokukabe( Document doc, XYZ picPointS, XYZ soilDirection, ElementId levelId,
      bool bIrizumi = true, bool kabeCreate = true, ElementId kabeShinId = null )
    {
      try {
        string symbolFolpath = ClsZumenInfo.GetYMSFolder() ;

        string familyPathKabe =
          System.IO.Path.Combine( symbolFolpath, "山留壁関係\\08_" + RENZOKUKABE + "\\" + RENZOKUKABE + ".rfa" ) ;
        string familyNameKabe = RENZOKUKABE ;

        //シンボル配置
        if ( ! ClsRevitUtil.LoadFamilySymbolData( doc, familyPathKabe, familyNameKabe, out FamilySymbol sym ) ) {
          return false ;
        }

        string familyPathKui = Master.ClsHBeamCsv.GetPileFamilyPath( m_size ) ;
        string familyNameKui = ClsRevitUtil.GetFamilyName( familyPathKui ) ;

        //親杭配置
        if ( ! ClsRevitUtil.LoadFamilyData( doc, familyPathKui, out Family oyaFam ) ) {
          return false ;
        }

        FamilySymbol oya = ClsRevitUtil.GetFamilySymbol( doc, familyNameKui, RENZOKUKABE ) ;

        //H形鋼
        //ClsRevitUtil.SetTypeParameter(oya, ClsGlobal.m_length, ClsRevitUtil.CovertToAPI(m_HLen));
        string stH = ClsYMSUtil.GetKouzaiSizeSunpou1( familyNameKui, 1 ) ;
        double dEx = ClsCommonUtils.ChangeStrToDbl( stH ) / 2 ;

        double dAngle = soilDirection.AngleOnPlaneTo( XYZ.BasisX, XYZ.BasisZ ) ;

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
          if ( kabeCreate ) {
            //*******壁作成*******//
            ( XYZ pS, XYZ pE ) = GetStEdToMidPoint( picPointS, soilDirection, ClsRevitUtil.CovertToAPI( dEx ) ) ;
            Curve wallCV = Line.CreateBound( pS, pE ) ;
            CreatedID = ClsRevitUtil.Create( doc, wallCV, levelId, sym ) ; //連続壁
            sym = ClsRevitUtil.ChangeTypeID( doc, sym, CreatedID, RENZOKUKABE + "_" + m_case ) ; // + "_" + m_edaNum);

            ClsRevitUtil.SetTypeParameter( sym, "CASE", m_case.ToString() ) ;
            ClsRevitUtil.SetTypeParameter( sym, "W", ClsRevitUtil.CovertToAPI( m_kabeAtumi ) ) ;
            ClsRevitUtil.SetTypeParameter( sym, "H", ClsRevitUtil.CovertToAPI( m_kabeLen ) ) ;
            ClsVoid.SetVoidDep( doc, CreatedID, m_void ) ;
            selId.Add( CreatedID ) ;
            //ClsRevitUtil.SetParameter(doc, CreatedID, "指定高さ", ClsRevitUtil.CovertToAPI(m_kabeLen));
            //ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベル オフセット", ClsRevitUtil.CovertToAPI(-m_kabeLen));
            //*******壁作成*******//
          }

          ElementId CreatedOyaID = ClsRevitUtil.Create( doc, picPointS, levelId, oya ) ; //親杭作成
          ClsRevitUtil.SetParameter( doc, CreatedOyaID, ClsGlobal.m_refLvTop, ClsRevitUtil.CovertToAPI( m_HTop ) ) ;
          ClsVoid.SetVoidDep( doc, CreatedOyaID, m_void ) ;
          ClsKabeShin.SetPutOrder( doc, CreatedOyaID, m_oreder ) ;
          ClsKabeShin.SetKabeShinId( doc, CreatedOyaID, kabeShinId ) ;
          ClsRevitUtil.CustomDataSet( doc, CreatedOyaID, "ピッチ", m_pitch ) ;

          Line axis = Line.CreateBound( picPointS, picPointS + XYZ.BasisZ ) ;
          ClsRevitUtil.RotateElement( doc, CreatedOyaID, axis, -dAngle ) ;
          selectionIds.Add( CreatedOyaID ) ;

          FamilyInstance inst = doc.GetElement( CreatedOyaID ) as FamilyInstance ;
          if ( ! bIrizumi ) //出隅
          {
            ClsRevitUtil.MoveFamilyInstance( inst, -dEx, inst.FacingOrientation ) ;
            picPointS = ( inst.Location as LocationPoint ).Point ;

            if ( CreatedID != null ) {
              FamilyInstance instSoil = doc.GetElement( CreatedID ) as FamilyInstance ;
              ClsRevitUtil.MoveFamilyInstance( instSoil, -dEx, inst.FacingOrientation ) ;
            }
          }

          ////交互対応
          string edaban = m_edaNum ;
          //ジョイント数
          int nKasho = m_Kasho1 ;
          Kotei kotei = m_Kotei1 ;
          //杭長さ
          List<int> pileLengths = m_ListPileLength1 ;

          //ボルト
          string boltF = m_BoltF1 ;
          string boltFNum = m_BoltFNum1 ;
          string boltW = m_BoltW1 ;
          string boltWNum = m_BoltWNum1 ;

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

          oya = ClsRevitUtil.ChangeTypeID( doc, oya, CreatedOyaID, RENZOKUKUI + "_" + m_case + "_" + edaban ) ;
          ClsRevitUtil.SetTypeParameter( oya, "CASE", m_case.ToString() ) ;
          ClsRevitUtil.SetTypeParameter( oya, "枝番", edaban ) ;

          ClsRevitUtil.SetTypeParameter( oya, "ジョイント数", nKasho ) ;
          ClsRevitUtil.SetTypeParameter( oya, "ピッチ", m_pitch.ToString() ) ;
          if ( m_zanti == "一部埋め殺し" )
            ClsRevitUtil.SetTypeParameter( oya, "残置/引抜", m_zanti + "(" + m_zantiLength + ")" ) ;
          else
            ClsRevitUtil.SetTypeParameter( oya, "残置/引抜", m_zanti ) ;

          //杭長さ
          ClsRevitUtil.SetTypeParameter( oya, "長さ", ClsRevitUtil.CovertToAPI( m_HLen ) ) ;

          for ( int j = 0 ; j < PileJointMax ; j++ ) {
            int nPileLength = 0 ;
            if ( j < pileLengths.Count ) {
              nPileLength = pileLengths[ j ] ;
            }

            ClsRevitUtil.SetTypeParameter( oya, "杭" + ( j + 1 ).ToString(), ClsRevitUtil.CovertToAPI( nPileLength ) ) ;
          }

          ////補助線の作成
          double dH = ClsRevitUtil.CovertToAPI( dEx ) ;
          outLine = CreatePointHojyoLine( doc, picPointS, -inst.FacingOrientation, soilDirection, dH,
            ClsRevitUtil.CovertToAPI( dEx / 2 ) ) ; //外側
          inLine = CreatePointHojyoLine( doc, picPointS, -inst.FacingOrientation, soilDirection, -dH,
            ClsRevitUtil.CovertToAPI( dEx / 2 ) ) ; //内側

          //変更が加わるメッセージ原因不明
          t.Commit() ;
        }

        if ( outLine != null && inLine != null ) {
          outLine = ClsVoid.CreateAdjustVoidLine( doc, outLine.Id, ClsVoid.OutVoidLine ) ;
          inLine = ClsVoid.CreateAdjustVoidLine( doc, inLine.Id, ClsVoid.InVoidLine ) ;
          ////壁くり抜き処理
          if ( m_bVoid ) ClsVoid.DoVoid( doc, selId, inLine, m_void, m_kabeAtumi / 2, levelId ) ;
        }
      }
      catch ( Exception ex ) {
        TaskDialog.Show( "Info Message20", ex.Message ) ;
        string message = ex.Message ;
        MessageBox.Show( message ) ;
      }

      return true ;
    }

    public static void ChangeKabeParameter( Document doc, ElementId id, string CASE, int kabeAtumi, int kabeLen )
    {
      FamilyInstance inst = doc.GetElement( id ) as FamilyInstance ;
      FamilySymbol sym = inst.Symbol ;
      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        t.Start() ;
        FailureHandlingOptions failOpt = t.GetFailureHandlingOptions() ;
        failOpt.SetFailuresPreprocessor( new WarningSwallower() ) ;
        t.SetFailureHandlingOptions( failOpt ) ;
        sym = ClsRevitUtil.ChangeTypeID( doc, sym, id, RENZOKUKABE + "_" + CASE ) ;

        ClsRevitUtil.SetTypeParameter( sym, "CASE", CASE.ToString() ) ;
        ClsRevitUtil.SetTypeParameter( sym, "W", ClsRevitUtil.CovertToAPI( kabeAtumi ) ) ;
        ClsRevitUtil.SetTypeParameter( sym, "H", ClsRevitUtil.CovertToAPI( kabeLen ) ) ;
        t.Commit() ;
      }
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
      m_zanti = ClsRevitUtil.GetTypeParameterString( sym, "残置/引抜" ) ;
      if ( m_zanti.Contains( "一部埋め殺し" ) ) {
        string[] split = m_zanti.Split( '(' ) ;
        m_zanti = split[ 0 ] ;
        string[] split1 = split[ 1 ].Split( ')' ) ;
        m_zantiLength = split1[ 0 ] ;
      }

      m_pitch = ClsCommonUtils.ChangeStrToInt( ClsRevitUtil.GetTypeParameterString( sym, "ピッチ" ) ) ;
      m_zaishitu = ClsRevitUtil.GetTypeParameterString( sym, "材質" ) ;

      List<ElementId> kabeList = GetAllKabeList( doc ) ;
      foreach ( ElementId kabeId in kabeList ) {
        FamilyInstance kabeInst = doc.GetElement( kabeId ) as FamilyInstance ;
        FamilySymbol kabeSym = kabeInst.Symbol ;
        int CASE = ClsCommonUtils.ChangeStrToInt( ClsRevitUtil.GetTypeParameterString( kabeSym, "CASE" ) ) ;
        //連続壁ののCASEが一致するものからデータを取得する
        if ( m_case == CASE ) {
          m_kabeAtumi = (int) ClsRevitUtil.CovertFromAPI( ClsRevitUtil.GetTypeParameter( kabeSym, "W" ) ) ;
          m_kabeLen = (int) ClsRevitUtil.CovertFromAPI( ClsRevitUtil.GetTypeParameter( kabeSym, "H" ) ) ;
          break ;
        }
      }

      m_type = Master.ClsHBeamCsv.GetTypePileFamilyPathInName( sym.FamilyName ) ;
      m_size = Master.ClsHBeamCsv.GetSizePileFamilyPathInName( sym.FamilyName ) ;
      m_HTop = (int) ClsRevitUtil.CovertFromAPI( ClsRevitUtil.GetParameterDouble( doc, id, ClsGlobal.m_refLvTop ) ) ;
      m_HLen = (int) ClsRevitUtil.CovertFromAPI( ClsRevitUtil.GetTypeParameter( sym, ClsGlobal.m_length ) ) ;


      //m_KougoFlg = false;

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
      m_Kotei1 = ( kotei == ClsPileTsugiteCsv.KoteiHohoBolt ? Kotei.Bolt : Kotei.Yousetsu ) ;
      ( var BoltF1, int BoltFNum1 ) = ClsYMSUtil.GetBoltFlange( doc, id ) ;
      m_BoltF1 = BoltF1 ;
      m_BoltFNum1 = BoltFNum1.ToString() ;
      ( var BoltW1, int BoltWNum1 ) = ClsYMSUtil.GetBoltWeb( doc, id ) ;
      m_BoltW1 = BoltW1 ;
      m_BoltWNum1 = BoltWNum1.ToString() ;

      return ;
    }

    /// <summary>
    /// 連続壁　の杭 のみを単独選択
    /// </summary>
    /// <param name="uidoc"></param>
    /// <param name="id">選択した 連続壁 の杭 のID</param>
    /// <param name="message">指示</param>
    /// <returns></returns>
    public static bool PickObject( UIDocument uidoc, ref ElementId id, string message = RENZOKUKUI )
    {
      return ClsRevitUtil.PickObjectPartFilter( uidoc, message + "を選択してください", RENZOKUKUI, ref id ) ;
    }

    /// <summary>
    /// 連続壁 の杭 のみを複数選択
    /// </summary>
    /// <param name="uidoc"></param>
    /// <param name="id">選択した 連続壁 の杭 のID</param>
    /// <param name="message">指示</param>
    /// <returns></returns>
    public static bool PickObjects( UIDocument uidoc, ref List<ElementId> ids, string message = RENZOKUKUI )
    {
      return ClsRevitUtil.PickObjectsPartFilter( uidoc, message + "を選択してください", RENZOKUKUI, ref ids ) ;
    }

    /// <summary>
    /// 図面上の連続杭を全て取得する
    /// </summary>
    /// <param name="doc"></param>
    /// <returns></returns>
    public static List<ElementId> GetAllRenzokuKabeList( Document doc )
    {
      //図面上の連続壁を全て取得
      List<ElementId> idList = ClsRevitUtil.GetSelectCreatedFamilyInstanceList( doc, RENZOKUKUI, true ) ;
      return idList ;
    }

    /// <summary>
    /// 図面上の連続壁を全て取得する
    /// </summary>
    /// <param name="doc"></param>
    /// <returns></returns>
    public static List<ElementId> GetAllKabeList( Document doc )
    {
      //図面上の連続壁を全て取得
      List<ElementId> idList = ClsRevitUtil.GetSelectCreatedFamilyInstanceList( doc, RENZOKUKABE, true ) ;
      return idList ;
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
    /// 中点から指定量離れた位置の始終点を返す
    /// </summary>
    /// <param name="pointMid">中点</param>
    /// <param name="Direction"></param>
    /// <param name="dEx"></param>
    /// <returns></returns>
    public static (XYZ, XYZ) GetStEdToMidPoint( XYZ pointMid, XYZ Direction, double dEx )
    {
      //補助線の作成
      XYZ hojyoS = new XYZ( pointMid.X - ( dEx * Direction.X ), pointMid.Y - ( dEx * Direction.Y ), pointMid.Z ) ;
      XYZ hojyoE = new XYZ( pointMid.X + ( dEx * Direction.X ), pointMid.Y + ( dEx * Direction.Y ), pointMid.Z ) ;
      return ( hojyoS, hojyoE ) ;
    }

    #endregion
  }
}