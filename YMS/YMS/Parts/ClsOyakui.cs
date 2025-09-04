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
  public class ClsOyakui
  {
    public const string oyakui = "親杭" ;

    /// <summary>
    /// 最大ジョイント数
    /// </summary>
    public const int PileJointMax = 10 ;

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
    /// 配置ピッチ
    /// </summary>
    public int m_putPitch { get ; set ; }

    /// <summary>
    /// 鋼材：タイプ
    /// </summary>
    public string m_type { get ; set ; }

    /// <summary>
    /// 鋼材：サイズ
    /// </summary>
    public string m_size { get ; set ; }

    /// <summary>
    /// 杭：天端
    /// </summary>
    public int m_HTop { get ; set ; }

    /// <summary>
    /// 杭：全長
    /// </summary>
    public int m_HLen { get ; set ; }

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
    /// 配置位置
    /// </summary>
    public int m_putPosFlag { get ; set ; }

    /// <summary>
    /// 横矢板
    /// </summary>
    public bool m_bYokoyaita { get ; set ; }

    /// <summary>
    /// 横矢板：タイプ
    /// </summary>
    public string m_typeYokoyaita { get ; set ; }

    /// <summary>
    /// 横矢板：サイズ
    /// </summary>
    public string m_sizeYokoyaita { get ; set ; }

    /// <summary>
    /// 横矢板配置位置
    /// </summary>
    public int m_putPosYokoyaitaFlag { get ; set ; }

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
    /// 出隅に1点作成された杭ID
    /// </summary>
    public ElementId m_DesumiKui { get ; set ; }

    /// <summary>
    /// 配置順
    /// </summary>
    public int m_oreder { get ; set ; }

    /// <summary>
    /// 最初に作成された杭ID
    /// </summary>
    public ElementId m_FirstKui { get ; set ; }

    /// <summary>
    /// 最後に1点作成された杭ID
    /// </summary>
    public ElementId m_LastKui { get ; set ; }

    #endregion

    #region コンストラクタ

    public ClsOyakui()
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
      m_putPitch = 0 ;
      m_type = string.Empty ;
      m_size = string.Empty ;
      m_HTop = 0 ;
      m_HLen = 0 ;
      m_zanti = string.Empty ;
      m_zantiLength = string.Empty ;
      m_putPosFlag = 0 ;
      m_bYokoyaita = false ;
      m_typeYokoyaita = string.Empty ;
      m_sizeYokoyaita = string.Empty ;
      m_putPosYokoyaitaFlag = 1 ;

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

      m_DesumiKui = null ;
      m_oreder = -1 ;

      m_FirstKui = null ;
      m_LastKui = null ;
      return ;
    }

    public bool CreateOyakui( Document doc, XYZ picPointS, XYZ picPointE, ElementId levelId, bool bIrizumi = true,
      ElementId kabeShinId = null, bool defVoid = true )
    {
      Line line = Line.CreateBound( picPointS, picPointE ) ;

      XYZ direction = line.Direction ; //単位ベクトル
      double length = ClsRevitUtil.CovertFromAPI( line.Length ) ; //芯の長さ
      try {
        string symbolFolpath = ClsZumenInfo.GetYMSFolder() ;

        string familyPathKui = Master.ClsHBeamCsv.GetPileFamilyPath( m_size ) ;
        string familyNameKui = ClsRevitUtil.GetFamilyName( familyPathKui ) ;

        ClsYokoyaita clsYokoyaita = new ClsYokoyaita() ;
        clsYokoyaita.m_putPosFlag = m_putPosYokoyaitaFlag ;

        //親杭配置

        //↓#31788

        //if (!ClsRevitUtil.RoadFamilyData(doc, familyPathKui, familyNameKui, out Family oyaFam))
        //{
        //    return false;
        //}
        //FamilySymbol oya = ClsRevitUtil.GetFamilySymbol(doc, familyNameKui, "親杭");

        if ( ! ClsRevitUtil.LoadFamilySymbolData( doc, familyPathKui, "親杭", out FamilySymbol oya ) ) {
          return false ;
        }

        //↑#31788

        string stB = ClsYMSUtil.GetKouzaiSizeSunpou1( familyNameKui, 2 ) ;
        int posFlag = 1 ;
        if ( m_putPosFlag == 0 ) //杭面
        {
          posFlag = 2 ;
        }

        int dFlangeWidth = ClsCommonUtils.ChangeStrToInt( stB ) / posFlag ;

        //ダイアログ指定の値をSET
        int refPDist = m_refPDist + dFlangeWidth ;
        int pitch = m_putPitch ;
        //基準点から指定の数値離す
        picPointS = new XYZ( picPointS.X + ( ClsRevitUtil.CovertToAPI( refPDist ) * direction.X ),
          picPointS.Y + ( ClsRevitUtil.CovertToAPI( refPDist ) * direction.Y ), picPointS.Z ) ;
        length -= refPDist ;

        //#31611 
        using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
          t.Start() ;
          FailureHandlingOptions failOpt = t.GetFailureHandlingOptions() ;
          failOpt.SetFailuresPreprocessor( new WarningSwallower() ) ;
          t.SetFailureHandlingOptions( failOpt ) ;

          //H形鋼
          ClsRevitUtil.SetTypeParameter( oya, ClsGlobal.m_length, ClsRevitUtil.CovertToAPI( m_HLen ) ) ;

          t.Commit() ;
        }


        //グループ化するIDを保持する変数の宣言(nullだとAdd出来ないため)
        ICollection<ElementId> selectionIds = new List<ElementId>() ;
        List<ElementId> oyaIdList = new List<ElementId>() ;

        string stH = ClsYMSUtil.GetKouzaiSizeSunpou1( familyNameKui, 1 ) ;
        double dEx = ClsCommonUtils.ChangeStrToDbl( stH ) / 2 ;

        XYZ picPoint = null ;
        double dCount = length / pitch ; //ダイアログソイルピッチずつ親杭、ソイルを配置するため何個置けるか
        double dAngle = direction.AngleOnPlaneTo( XYZ.BasisX, XYZ.BasisZ ) ;
        if ( ! defVoid )
          dAngle += Math.PI ;

        using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
          t.Start() ;
          FailureHandlingOptions failOpt = t.GetFailureHandlingOptions() ;
          failOpt.SetFailuresPreprocessor( new WarningSwallower() ) ;
          t.SetFailureHandlingOptions( failOpt ) ;
          if ( kabeShinId != null )
            ClsRevitUtil.SetMojiParameter( doc, kabeShinId, "壁", oyakui ) ;
          int putPtn = 0 ;
          for ( int i = 0 ; i <= dCount ; i++ ) {
            if ( ! bIrizumi && i == 0 ) {
              if ( m_DesumiKui != null )
                oyaIdList.Add( m_DesumiKui ) ;
              continue ;
            } //出隅の時は別で出隅分作成するため始点を飛ばす

            picPoint = new XYZ( picPointS.X + ( ClsRevitUtil.CovertToAPI( pitch * i ) * direction.X ),
              picPointS.Y + ( ClsRevitUtil.CovertToAPI( pitch * i ) * direction.Y ),
              picPointS.Z + ( ClsRevitUtil.CovertToAPI( pitch * i ) * direction.Z ) ) ;

            ElementId CreatedOyaID = ClsRevitUtil.Create( doc, picPoint, levelId, oya ) ; //親杭作成
            ClsRevitUtil.SetParameter( doc, CreatedOyaID, ClsGlobal.m_refLvTop, ClsRevitUtil.CovertToAPI( m_HTop ) ) ;
            ClsVoid.SetVoidDep( doc, CreatedOyaID, m_void ) ;
            ClsKabeShin.SetKabeShinId( doc, CreatedOyaID, kabeShinId ) ;
            ClsRevitUtil.CustomDataSet( doc, CreatedOyaID, "ピッチ", m_putPitch ) ;

            ClsKabeShin.SetPutOrder( doc, CreatedOyaID, putPtn ) ;
            if ( putPtn == 0 )
              putPtn = 1 ;
            else
              putPtn = 0 ;

            Line axis = Line.CreateBound( picPoint, picPoint + XYZ.BasisZ ) ;
            double angle = dAngle ;

            ClsRevitUtil.RotateElement( doc, CreatedOyaID, axis, -angle ) ;
            selectionIds.Add( CreatedOyaID ) ;
            oyaIdList.Add( CreatedOyaID ) ;

            FamilyInstance inst = doc.GetElement( CreatedOyaID ) as FamilyInstance ;
            if ( m_putPosFlag == 0 ) //杭面
            {
              ClsRevitUtil.MoveFamilyInstance( inst, -dEx, inst.FacingOrientation ) ;
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


            oya = ClsRevitUtil.ChangeTypeID( doc, oya, CreatedOyaID, "親杭_" + m_case + "_" + edaban ) ;
            ClsRevitUtil.SetTypeParameter( oya, "CASE", m_case.ToString() ) ;
            ClsRevitUtil.SetTypeParameter( oya, "枝番", edaban ) ;

            ClsRevitUtil.SetTypeParameter( oya, "ジョイント数", nKasho ) ;
            ClsRevitUtil.SetTypeParameter( oya, "ピッチ", m_putPitch.ToString() ) ;
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

            if ( m_FirstKui == null )
              m_FirstKui = CreatedOyaID ;
            m_LastKui = CreatedOyaID ;
          }

          t.Commit() ;
        }

        //横矢板の作成
        if ( m_bYokoyaita ) {
          for ( int i = 0 ; i < oyaIdList.Count - 1 ; i++ ) {
            clsYokoyaita.m_type = m_typeYokoyaita ;
            clsYokoyaita.m_size = m_sizeYokoyaita ;
            if ( m_void <= 0 ) continue ;
            clsYokoyaita.CreateMokuyaita( doc, oyaIdList[ i ], oyaIdList[ i + 1 ], m_putPitch,
              ClsRevitUtil.CovertToAPI( m_void ), levelId, ClsRevitUtil.CovertToAPI( m_HTop ) ) ;
            //clsYokoyaita.CreateYokoyaita(doc, oyaIdList[i], m_putPitch);
          }
        }
      }
      catch ( Exception ex ) {
        TaskDialog.Show( "Info Message17", ex.Message ) ;
        string message = ex.Message ;
        MessageBox.Show( message ) ;
      }

      return true ;
    }

    public bool CreateSingleOyakui( Document doc, XYZ picPointS, XYZ direction, ElementId levelId, bool bIrizumi = true,
      bool bCorner = false, ElementId kabeShinId = null, double apexAngle = 0.0 )
    {
      try {
        string symbolFolpath = ClsZumenInfo.GetYMSFolder() ;

        string familyPathKui = Master.ClsHBeamCsv.GetPileFamilyPath( m_size ) ;
        string familyNameKui = ClsRevitUtil.GetFamilyName( familyPathKui ) ;

        //親杭配置
        //if (!ClsRevitUtil.LoadFamilyData(doc, familyPathKui, out Family oyaFam))
        //{
        //    return false;
        //}
        //FamilySymbol oya = ClsRevitUtil.GetFamilySymbol(doc, familyNameKui, "親杭");

        if ( ! ClsRevitUtil.LoadFamilySymbolData( doc, familyPathKui, "親杭", out FamilySymbol oya ) ) {
          return false ;
        }

        //↑#31788

        string stB = ClsYMSUtil.GetKouzaiSizeSunpou1( familyNameKui, 2 ) ;
        int dFlangeWidth = ClsCommonUtils.ChangeStrToInt( stB ) / 2 ;

        string stH = ClsYMSUtil.GetKouzaiSizeSunpou1( familyNameKui, 1 ) ;
        double dEx = ClsCommonUtils.ChangeStrToDbl( stH ) / 2 ;

        double dAngle = direction.AngleOnPlaneTo( XYZ.BasisX, XYZ.BasisZ ) ;
        Line axis = Line.CreateBound( picPointS, picPointS + XYZ.BasisZ ) ;

        using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
          t.Start() ;
          FailureHandlingOptions failOpt = t.GetFailureHandlingOptions() ;
          failOpt.SetFailuresPreprocessor( new WarningSwallower() ) ;
          t.SetFailureHandlingOptions( failOpt ) ;

          ElementId CreatedOyaID = ClsRevitUtil.Create( doc, picPointS, levelId, oya ) ; //親杭作成
          ClsRevitUtil.SetParameter( doc, CreatedOyaID, ClsGlobal.m_refLvTop, ClsRevitUtil.CovertToAPI( m_HTop ) ) ;
          ClsVoid.SetVoidDep( doc, CreatedOyaID, m_void ) ;
          ClsKabeShin.SetKabeShinId( doc, CreatedOyaID, kabeShinId ) ;

          ClsRevitUtil.RotateElement( doc, CreatedOyaID, axis, -dAngle ) ;

          ClsKabeShin.SetPutOrder( doc, CreatedOyaID, m_oreder ) ;

          //コーナー入隅出隅杭面杭芯対応
          FamilyInstance inst = doc.GetElement( CreatedOyaID ) as FamilyInstance ;
          if ( bCorner ) {
            if ( bIrizumi && m_putPosFlag == 0 ) //コーナーかつ入隅かつ杭面
            {
              ClsRevitUtil.MoveFamilyInstance( inst, -dEx, inst.FacingOrientation ) ;
            }
            else if ( ! bIrizumi && m_putPosFlag == 0 ) //出隅かつ杭面
            {
              double oneSide =
                ClsRevitUtil.CalculateIsoscelesTriangleOneSide( dFlangeWidth * 2, apexAngle, out double baseAngle ) ;
              oneSide = ClsRevitUtil.CalculateTriangleOneSide( oneSide, Math.PI / 2, baseAngle ) ;
              ClsRevitUtil.MoveFamilyInstance( inst, -( oneSide + dEx ),
                inst.FacingOrientation ) ; // - dEx * 2, inst.FacingOrientation);
            }
            else if ( ! bIrizumi ) //出隅かつ杭芯
            {
              double oneSide =
                ClsRevitUtil.CalculateIsoscelesTriangleOneSide( dFlangeWidth * 2, apexAngle, out double baseAngle ) ;
              oneSide = ClsRevitUtil.CalculateTriangleOneSide( oneSide, Math.PI / 2, baseAngle ) ;
              ClsRevitUtil.MoveFamilyInstance( inst, -( oneSide + dEx ), inst.FacingOrientation ) ;
              //ClsRevitUtil.MoveFamilyInstance(inst, -dEx, inst.FacingOrientation);
            }
            else //入隅かつ杭芯
            {
              ClsRevitUtil.MoveFamilyInstance( inst, dEx, inst.FacingOrientation ) ;
            }

            SetCornerFlag( doc, CreatedOyaID, bCorner ) ;
          }

          //H形鋼
          ClsRevitUtil.SetTypeParameter( oya, ClsGlobal.m_length, ClsRevitUtil.CovertToAPI( m_HLen ) ) ;

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

          oya = ClsRevitUtil.ChangeTypeID( doc, oya, CreatedOyaID, "親杭_" + m_case + "_" + edaban ) ;
          ClsRevitUtil.SetTypeParameter( oya, "CASE", m_case.ToString() ) ;
          ClsRevitUtil.SetTypeParameter( oya, "枝番", edaban ) ;

          ClsRevitUtil.SetTypeParameter( oya, "ジョイント数", nKasho ) ;
          ClsRevitUtil.SetTypeParameter( oya, "ピッチ", m_putPitch.ToString() ) ;
          if ( m_zanti == "一部埋め殺し" )
            ClsRevitUtil.SetTypeParameter( oya, "残置/引抜", m_zanti + "(" + m_zantiLength + ")" ) ;
          else
            ClsRevitUtil.SetTypeParameter( oya, "残置/引抜", m_zanti ) ;

          for ( int j = 0 ; j < PileJointMax ; j++ ) {
            int nPileLength = 0 ;
            if ( j < pileLengths.Count ) {
              nPileLength = pileLengths[ j ] ;
            }

            ClsRevitUtil.SetTypeParameter( oya, "杭" + ( j + 1 ).ToString(), ClsRevitUtil.CovertToAPI( nPileLength ) ) ;
          }

          m_DesumiKui = CreatedOyaID ;
          t.Commit() ;
        }
      }
      catch ( Exception ex ) {
        TaskDialog.Show( "Info Message18", ex.Message ) ;
        string message = ex.Message ;
        MessageBox.Show( message ) ;
      }

      return true ;
    }

    public bool CreateCornerOyakui( Document doc, XYZ picPointS, XYZ direction, ElementId levelId, bool bIrizumi = true,
      bool bCorner = false, ElementId kabeShinId = null )
    {
      try {
        string symbolFolpath = ClsZumenInfo.GetYMSFolder() ;

        string familyPathKui = Master.ClsHBeamCsv.GetPileFamilyPath( m_size ) ;
        string familyNameKui = ClsRevitUtil.GetFamilyName( familyPathKui ) ;

        //親杭配置
        if ( ! ClsRevitUtil.LoadFamilyData( doc, familyPathKui, out Family oyaFam ) ) {
          return false ;
        }

        FamilySymbol oya = ClsRevitUtil.GetFamilySymbol( doc, familyNameKui, "親杭" ) ;


        string stH = ClsYMSUtil.GetKouzaiSizeSunpou1( familyNameKui, 1 ) ;
        double dEx = ClsCommonUtils.ChangeStrToDbl( stH ) / 2 ;

        double dAngle = direction.AngleOnPlaneTo( XYZ.BasisX, XYZ.BasisZ ) ;
        Line axis = Line.CreateBound( picPointS, picPointS + XYZ.BasisZ ) ;

        using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
          t.Start() ;
          FailureHandlingOptions failOpt = t.GetFailureHandlingOptions() ;
          failOpt.SetFailuresPreprocessor( new WarningSwallower() ) ;
          t.SetFailureHandlingOptions( failOpt ) ;

          ElementId CreatedOyaID = ClsRevitUtil.Create( doc, picPointS, levelId, oya ) ; //親杭作成
          ClsRevitUtil.SetParameter( doc, CreatedOyaID, ClsGlobal.m_refLvTop, ClsRevitUtil.CovertToAPI( m_HTop ) ) ;
          ClsVoid.SetVoidDep( doc, CreatedOyaID, m_void ) ;
          ClsKabeShin.SetKabeShinId( doc, CreatedOyaID, kabeShinId ) ;

          ClsRevitUtil.RotateElement( doc, CreatedOyaID, axis, -dAngle ) ;

          ClsKabeShin.SetPutOrder( doc, CreatedOyaID, m_oreder ) ;

          //コーナー入隅出隅杭面杭芯対応
          FamilyInstance inst = doc.GetElement( CreatedOyaID ) as FamilyInstance ;
          if ( bCorner ) {
            if ( bIrizumi && m_putPosFlag == 0 ) //コーナーかつ入隅かつ杭面
            {
              ClsRevitUtil.MoveFamilyInstance( inst, -dEx, inst.FacingOrientation ) ;
            }
            else if ( ! bIrizumi && m_putPosFlag == 0 ) //出隅かつ杭面
            {
              ClsRevitUtil.MoveFamilyInstance( inst, -dEx * 2, inst.FacingOrientation ) ;
            }
            else if ( ! bIrizumi ) //出隅かつ杭芯
            {
              ClsRevitUtil.MoveFamilyInstance( inst, -dEx, inst.FacingOrientation ) ;
            }
            else //入隅かつ杭芯
            {
              ClsRevitUtil.MoveFamilyInstance( inst, dEx, inst.FacingOrientation ) ;
            }
          }

          //H形鋼
          ClsRevitUtil.SetTypeParameter( oya, ClsGlobal.m_length, ClsRevitUtil.CovertToAPI( m_HLen ) ) ;

          ////交互対応
          string edaban = m_edaNum ;
          //ジョイント数
          int nKasho = m_Kasho1 ;
          //杭長さ
          List<int> pileLengths = m_ListPileLength1 ;

          //if (m_KougoFlg && (i % 2) == 1)//交互時の枝番タブ２
          //{
          //    int numeric = ClsCommonUtils.ConvertToNumeric(m_edaNum);
          //    numeric += 1;
          //    edaban = ClsCommonUtils.ConvertToAlphabet(numeric);
          //    nKasho = m_Kasho2;
          //    pileLengths = m_ListPileLength2;
          //}

          oya = ClsRevitUtil.ChangeTypeID( doc, oya, CreatedOyaID, "親杭_" + m_case + "_" + edaban ) ;
          ClsRevitUtil.SetTypeParameter( oya, "CASE", m_case.ToString() ) ;
          ClsRevitUtil.SetTypeParameter( oya, "枝番", edaban ) ;

          ClsRevitUtil.SetTypeParameter( oya, "ジョイント数", nKasho ) ;
          if ( m_zanti == "一部埋め殺し" )
            ClsRevitUtil.SetTypeParameter( oya, "残置/引抜", m_zanti + "(" + m_zantiLength + ")" ) ;
          else
            ClsRevitUtil.SetTypeParameter( oya, "残置/引抜", m_zanti ) ;

          for ( int j = 0 ; j < PileJointMax ; j++ ) {
            int nPileLength = 0 ;
            if ( j < pileLengths.Count ) {
              nPileLength = pileLengths[ j ] ;
            }

            ClsRevitUtil.SetTypeParameter( oya, "杭" + ( j + 1 ).ToString(), ClsRevitUtil.CovertToAPI( nPileLength ) ) ;
          }

          m_DesumiKui = CreatedOyaID ;
          t.Commit() ;
        }
      }
      catch ( Exception ex ) {
        TaskDialog.Show( "Info Message18", ex.Message ) ;
        string message = ex.Message ;
        MessageBox.Show( message ) ;
      }

      return true ;
    }

    /// <summary>
    /// 杭(H鋼) のみを単独選択
    /// </summary>
    /// <param name="uidoc"></param>
    /// <param name="id">選択した 杭(H鋼) のID</param>
    /// <param name="message">指示</param>
    /// <returns></returns>
    public static bool PickBaseObject( UIDocument uidoc, ref ElementId id, string message = "杭" )
    {
      List<string> oyakuiList = ClsHBeamCsv.GetAllHBeamName() ;
      return ClsRevitUtil.PickObjectSymbolFilters( uidoc, message, oyakuiList, ref id ) ;
    }

    /// <summary>
    /// 杭(H鋼) のみを複数選択
    /// </summary>
    /// <param name="uidoc"></param>
    /// <param name="id">選択した 杭(H鋼) のID</param>
    /// <param name="message">指示</param>
    /// <returns></returns>
    public static bool PickBaseObjects( UIDocument uidoc, ref List<ElementId> id, string message = "杭" )
    {
      List<string> oyakuiList = ClsHBeamCsv.GetAllHBeamName() ;
      return ClsRevitUtil.PickObjectsSymbolFilters( uidoc, message, oyakuiList, ref id ) ;
    }

    /// <summary>
    /// 親杭 のみを単独選択
    /// </summary>
    /// <param name="uidoc"></param>
    /// <param name="id">選択した 親杭 のID</param>
    /// <param name="message">指示</param>
    /// <returns></returns>
    public static bool PickObject( UIDocument uidoc, ref ElementId id, string message = oyakui )
    {
      return ClsRevitUtil.PickObjectPartFilter( uidoc, message + "を選択してください", oyakui, ref id ) ;
    }

    /// <summary>
    /// 親杭 のみを複数選択
    /// </summary>
    /// <param name="uidoc"></param>
    /// <param name="id">選択した 親杭 のID</param>
    /// <param name="message">指示</param>
    /// <returns></returns>
    public static bool PickObjects( UIDocument uidoc, ref List<ElementId> ids, string message = oyakui )
    {
      return ClsRevitUtil.PickObjectsPartFilter( uidoc, message + "を選択してください", oyakui, ref ids ) ;
    }

    /// <summary>
    /// 図面上の親杭を全て取得
    /// </summary>
    /// <param name="doc"></param>
    /// <returns></returns>
    public static List<ElementId> GetAllOyakuiList( Document doc )
    {
      List<ElementId> oyakuiIdList = ClsRevitUtil.GetSelectCreatedFamilyInstanceList( doc, "親杭", true ) ;
      return oyakuiIdList ;
    }

    /// <summary>
    /// 対象の親杭にcornerで作図されたか否かをカスタムデータとして持たせる
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="id">ファミリ</param>
    /// <returns></returns>
    public static bool SetCornerFlag( Document doc, ElementId id, bool corner )
    {
      return ClsRevitUtil.CustomDataSet<bool>( doc, id, "corner", corner ) ;
    }

    /// <summary>
    /// 対象の親杭からcornerで作図されたか否かのカスタムデータを取得する
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="id">ファミリ</param>
    /// <returns></returns>
    public static bool GetCornerFlag( Document doc, ElementId id )
    {
      ClsRevitUtil.CustomDataGet<bool>( doc, id, "corner", out bool corner ) ;
      return corner ;
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

      m_putPitch = ClsCommonUtils.ChangeStrToInt( ClsRevitUtil.GetTypeParameterString( sym, "ピッチ" ) ) ;
      m_zaishitu = ClsRevitUtil.GetTypeParameterString( sym, "材質" ) ;
      //m_putPosFlag = 0;
      //m_bYokoyaita = false;
      //m_typeYokoyaita = string.Empty;
      //m_sizeYokoyaita = string.Empty;
      //m_putPosYokoyaitaFlag = 0;

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

    #endregion
  }
}