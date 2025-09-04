using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Windows.Forms ;
using YMS.gantry ;
using YMS_gantry.Data ;
using YMS_gantry.GantryUtils ;
using YMS_gantry.Material ;
using static YMS_gantry.DefineUtil ;

namespace YMS_gantry.UI
{
  public partial class FrmEditSizeList : System.Windows.Forms.Form
  {
    private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger() ;

    private const string FORM_NAME = "サイズ一覧" ;

    #region 分類名(大中小)定数

    public const string CONST_DAIBUNRUI_KOUDAI_1 = "構台(架台)" ;
    public const string CONST_DAIBUNRUI_KOUDAI_2 = "構台（架台）" ;
    public const string CONST_DAIBUNRUI_KOUDAIKUI = "構台杭" ;
    public const string CONST_DAIBUNRUI_YAMADOMEHEKI = "山留壁" ;

    public const string CONST_CHUUBUNRUI_FUKKOUBAN = "覆工板" ;
    public const string CONST_CHUUBUNRUI_KETAZAI = "桁材" ;
    public const string CONST_CHUUBUNRUI_SONOTA = "その他" ;
    public const string CONST_CHUUBUNRUI_PL = "ﾌﾟﾚｰﾄ" ;
    public const string CONST_CHUUBUNRUI_BRACE = "ﾌﾞﾚｰｽ" ;
    public const string CONST_CHUUBUNRUI_TEIKETSUKANAGU = "締結金具" ;
    public const string CONST_CHUUBUNRUI_SHUZAI = "主材" ;
    public const string CONST_CHUUBUNRUI_TSUGITASHIKUI = "継ぎ足し杭" ;
    public const string CONST_CHUUBUNRUI_SHIJIKUI = "支持杭" ;
    public const string CONST_CHUUBUNRUI_TAKASACHOUSEIPIEACE = "高さ調整ﾋﾟｰｽ" ;

    public const string CONST_SHOUBUNRUI_FUKKOUBAN = "覆工板" ;
    public const string CONST_SHOUBUNRUI_OHBIKI = "大引" ;
    public const string CONST_SHOUBUNRUI_KETAUKE = "桁受" ;
    public const string CONST_SHOUBUNRUI_NEDA = "根太" ;
    public const string CONST_SHOUBUNRUI_SHUGETA = "主桁" ;
    public const string CONST_SHOUBUNRUI_SHIKIGETA = "敷桁" ;
    public const string CONST_SHOUBUNRUI_FUKKOUGETA = "覆工桁" ;
    public const string CONST_SHOUBUNRUI_TAIKEIKOU = "対傾構" ;
    public const string CONST_SHOUBUNRUI_SLOPEMAZUMEZAI = "ｽﾛｰﾌﾟ間詰め材" ;
    public const string CONST_SHOUBUNRUI_FUKKOUBANZUREDOMEZAI = "覆工板ｽﾞﾚ止め" ;
    public const string CONST_SHOUBUNRUI_SUCHIFUNAJACK = "ｽﾁﾌﾅｰｼﾞｬｯｷ" ;
    public const string CONST_SHOUBUNRUI_TORITSUKEHOJYOZAI = "取付用補助材" ;
    public const string CONST_SHOUBUNRUI_TAIKEIKOUPL = "対傾構取付ﾌﾟﾚｰﾄ" ;
    public const string CONST_SHOUBUNRUI_SUCHIFUNAPL = "ｽﾁﾌﾅｰPL" ;
    public const string CONST_SHOUBUNRUI_TAKASACHOUSEIPIEACE = "高さ調整ﾋﾟｰｽ" ;
    public const string CONST_SHOUBUNRUI_TAKASACHOUSEIPL = "高さ調整ﾌﾟﾚｰﾄ" ;
    public const string CONST_SHOUBUNRUI_SHIMAKOUBAN = "縞鋼板" ;
    public const string CONST_SHOUBUNRUI_SHIKITEPPAN = "敷鉄板" ;
    public const string CONST_SHOUBUNRUI_JIFUKU = "地覆" ;
    public const string CONST_SHOUBUNRUI_TESURI = "手摺" ;
    public const string CONST_SHOUBUNRUI_TESURISHICHU = "手摺支柱" ;
    public const string CONST_SHOUBUNRUI_BRACE = "ﾌﾞﾚｰｽ" ;
    public const string CONST_SHOUBUNRUI_TURNBUCKLEBRACE = "ﾀｰﾝﾊﾞｯｸﾙﾌﾞﾚｰｽ" ;
    public const string CONST_SHOUBUNRUI_BULMAN = "ﾌﾞﾙﾏﾝ" ;
    public const string CONST_SHOUBUNRUI_RIKIMAN = "ﾘｷﾏﾝ" ;
    public const string CONST_SHOUBUNRUI_TSUNAGI = "水平ﾂﾅｷﾞ" ;
    public const string CONST_SHOUBUNRUI_HOUDUE = "方杖" ;
    public const string CONST_SHOUBUNRUI_HIUCHIUKE = "火打受ﾋﾟｰｽ" ;
    public const string CONST_SHOUBUNRUI_SUMIBUPIEACE = "隅部ﾋﾟｰｽ" ;
    public const string CONST_SHOUBUNRUI_TSUGITASHIKUI = "継ぎ足し杭" ;
    public const string CONST_SHOUBUNRUI_SHICHU_SOZAI = "素材" ;
    public const string CONST_SHOUBUNRUI_SHICHU_YAMADOME = "山留主材" ;
    public const string CONST_SHOUBUNRUI_SHICHU_KOUKYOUDOYAMADOME = "高強度山留主材" ;
    public const string CONST_SHOUBUNRUI_KUI = "杭" ;
    public const string CONST_SHOUBUNRUI_KAKOUHIN = "加工品" ;

    private const string CONST_CATEGORY_SHICHU = "支柱" ;
    private const string CONST_CATEGORY_OHBIKI = "大引" ;
    private const string CONST_CATEGORY_KETAUKE = "桁受" ;
    private const string CONST_CATEGORY_SHIKIGETA = "敷桁" ;
    private const string CONST_CATEGORY_NEDA = "根太" ;
    private const string CONST_CATEGORY_SHUGETA = "主桁" ;
    private const string CONST_CATEGORY_JIFUKU = "地覆" ;
    private const string CONST_CATEGORY_FUKKOUBANZUREDOMEZAI = "覆工板ｽﾞﾚ止め" ;
    private const string CONST_CATEGORY_SHIJIKUI = "支持杭" ;

    #endregion

    private RequestHandler m_Handler ;
    private ExternalEvent m_ExEvent ;

    private UIDocument _uiDoc ;
    public List<ElementId> _targetElementIdList ;
    public List<string> _targetElementUniqueIdList ;
    private List<string> _koudaiNameList ;
    private List<Bunrui> _bunruiList ;
    private List<ChoicesSize> _choicesSizeList ;
    private List<ChoicesSizeOfCategory> _choicesSizeOfCategoryList ;

    private List<ElementId> _selectedElemetIdList ;
    private List<ElementId> _selectedElemetIdListOfCategory ;

    private List<string> _buzaiCategoryList ;

    public string DAIBUNRUI_KOUDAI_1 = CONST_DAIBUNRUI_KOUDAI_1 ;
    public string DAIBUNRUI_KOUDAI_2 = CONST_DAIBUNRUI_KOUDAI_2 ;
    public string DAIBUNRUI_YAMADOMEHEKI = CONST_DAIBUNRUI_YAMADOMEHEKI ;
    public string DAIBUNRUI_KOUDAIKUI = CONST_DAIBUNRUI_KOUDAIKUI ;
    public string CHUUBUNRUI_SHUZAI = CONST_CHUUBUNRUI_SHUZAI ;
    public string CHUUBUNRUI_SHIJIKUI = CONST_CHUUBUNRUI_SHIJIKUI ;
    public string CHUUBUNRUI_TSUGITASHIKUI = CONST_CHUUBUNRUI_TSUGITASHIKUI ;
    public string SHOUBUNRUI_SHICHU_SOZAI = CONST_SHOUBUNRUI_SHICHU_SOZAI ;
    public string SHOUBUNRUI_SHICHU_YAMADOME = CONST_SHOUBUNRUI_SHICHU_YAMADOME ;
    public string SHOUBUNRUI_SHICHU_KOUKYOUDOYAMADOME = CONST_SHOUBUNRUI_SHICHU_KOUKYOUDOYAMADOME ;
    public string SHOUBUNRUI_KUI = CONST_SHOUBUNRUI_KUI ;
    public string SHOUBUNRUI_TSUGITASHIKUI = CONST_SHOUBUNRUI_TSUGITASHIKUI ;

    public string CATEGORY_SHICHU = CONST_CATEGORY_SHICHU ;
    public string CATEGORY_OHBIKI = CONST_CATEGORY_OHBIKI ;
    public string CATEGORY_KETAUKE = CONST_CATEGORY_KETAUKE ;
    public string CATEGORY_SHIKIGETA = CONST_CATEGORY_SHIKIGETA ;
    public string CATEGORY_NEDA = CONST_CATEGORY_NEDA ;
    public string CATEGORY_SHUGETA = CONST_CATEGORY_SHUGETA ;
    public string CATEGORY_JIFUKU = CONST_CATEGORY_JIFUKU ;
    public string CATEGORY_FUKKOUBANZUREDOMEZAI = CONST_CATEGORY_FUKKOUBANZUREDOMEZAI ;
    public string CATEGORY_SHIJIKUI = CONST_CATEGORY_SHIJIKUI ;

    public FrmEditSizeList( ExternalEvent exEvent, RequestHandler handler, UIApplication uiApp,
      List<ElementId> targetElementIdList )
    {
      InitializeComponent() ;

      _uiDoc = uiApp.ActiveUIDocument ;
      _targetElementIdList = targetElementIdList ;
      _targetElementUniqueIdList = new List<string>() ;

      // ElementId は値が変更されるケースがあるらしく怪しいので UniqueId を利用する
      foreach ( var elementId in targetElementIdList ) {
        var element = _uiDoc.Document.GetElement( elementId ) ;
        if ( element == null ) continue ;
        _targetElementUniqueIdList.Add( element.UniqueId ) ;
      }

      _koudaiNameList = new List<string>() ;
      _bunruiList = new List<Bunrui>() ;
      _choicesSizeList = new List<ChoicesSize>() ;
      _choicesSizeOfCategoryList = new List<ChoicesSizeOfCategory>() ;

      _selectedElemetIdList = new List<ElementId>() ;
      _selectedElemetIdListOfCategory = new List<ElementId>() ;

      _buzaiCategoryList = new List<string>() { string.Empty } ;

      m_Handler = handler ;
      m_ExEvent = exEvent ;
    }

    private void FrmEditSizeList_Load( object sender, EventArgs e )
    {
      try {
        // Revit上でのファミリ選択状態を解除
        _uiDoc.Selection.SetElementIds( new List<ElementId>() ) ;

        InitComponent() ;
        InitData() ;
      }
      catch ( Exception ex ) {
        MessageUtil.Error( "画面のロード処理に失敗しました。", FORM_NAME, this ) ;
        logger.Error( ex, ex.Message ) ;
      }
    }

    private void InitComponent()
    {
      foreach ( var elementUniqueId in _targetElementUniqueIdList ) {
        // エレメント取得
        var element = _uiDoc.Document.GetElement( elementUniqueId ) ;
        if ( element == null ) continue ;

        var elementId = element.Id ;

        // ファミリ名取得
        var familyName = GantryUtil.GetFamilyName( _uiDoc.Document.GetElement( elementId ) ) ;
        // タイプ名取得
        var typeName = GantryUtil.GetTypeName( _uiDoc.Document.GetElement( elementId ) ) ;
        // ファミリインスタンス取得
        var familyInstance = GantryUtil.GetFamilyInstance( _uiDoc.Document, elementId ) ;
        // ファミリシンボル取得
        var familySymbol = ClsRevitUtil.GetFamilySymbol( _uiDoc.Document, familyName, typeName ) ;

        if ( familyInstance == null || familySymbol == null ) continue ;

        // ネストされたファミリである場合はレコードの追加をスキップ
        if ( familyInstance.SuperComponent != null ) continue ;

        var material = MaterialSuper.ReadFromElement( familyInstance.Id, _uiDoc.Document ) ;

        // 「構台基点ファミリ」の場合はレコード追加をスキップ
        if ( familyName == KoudaiReference.familyName ) continue ;

        // Revit標準機能で配置された(部材情報のElementId有無で判断)ファミリの場合はスキップ
        // システムで配置した部材については部材情報にElementId持ってるはず。
        // 未割当部材への構台名付与機能で構台名のみをセットされている場合があるがそれをスキップしたいという意図。
        if ( material == null || material.m_ElementId == null ) continue ;

        var id = element.UniqueId ;
        var koudaiName = material?.m_KodaiName ;
        var daiBunrui = GantryUtil.GetTypeParameter( familySymbol, DefineUtil.PARAM_DAI_BUNRUI ) ;
        var chuuBunrui = GantryUtil.GetTypeParameter( familySymbol, DefineUtil.PARAM_CHUU_BUNRUI ) ;
        var shouBunrui = GantryUtil.GetTypeParameter( familySymbol, DefineUtil.PARAM_SHOU_BUNRUI ) ;

        var existsKoudaiName = _koudaiNameList.Exists( x => x == koudaiName ) ;
        if ( ! existsKoudaiName ) {
          // 配置ファミリの構台名を抽出
          _koudaiNameList.Add( koudaiName ) ;
        }

        var existsBunrui =
          _bunruiList.Exists( x => x.Dai == daiBunrui && x.Chuu == chuuBunrui && x.Shou == shouBunrui ) ;
        if ( ! existsBunrui ) {
          // 全ての分類が空の対象は抽出しない
          if ( string.IsNullOrWhiteSpace( daiBunrui ) && string.IsNullOrWhiteSpace( chuuBunrui ) &&
               string.IsNullOrWhiteSpace( shouBunrui ) ) continue ;

          if ( ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                 chuuBunrui == CONST_CHUUBUNRUI_SONOTA && shouBunrui == CONST_SHOUBUNRUI_SUCHIFUNAJACK ) ||
               ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                 chuuBunrui == CONST_CHUUBUNRUI_PL && shouBunrui == CONST_SHOUBUNRUI_TAIKEIKOUPL ) ||
               ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                 chuuBunrui == CONST_CHUUBUNRUI_PL && shouBunrui == CONST_SHOUBUNRUI_SUCHIFUNAPL ) ) {
            // サイズ変更時に対象としない(桁材のサイズに依存するため)
            continue ;
          }

          // 図面内に配置されたファミリの分類情報を抽出
          _bunruiList.Add( new Bunrui { Dai = daiBunrui, Chuu = chuuBunrui, Shou = shouBunrui } ) ;
        }

        var buzaiCategory = string.Empty ;
        if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
             chuuBunrui == CONST_CHUUBUNRUI_KETAZAI && shouBunrui == CONST_SHOUBUNRUI_NEDA ) {
          buzaiCategory = CONST_CATEGORY_NEDA ;
        }
        else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                  chuuBunrui == CONST_CHUUBUNRUI_KETAZAI && shouBunrui == CONST_SHOUBUNRUI_SHUGETA ) {
          buzaiCategory = CONST_CATEGORY_SHUGETA ;
        }
        else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                  chuuBunrui == CONST_CHUUBUNRUI_KETAZAI && shouBunrui == CONST_SHOUBUNRUI_OHBIKI ) {
          buzaiCategory = CONST_CATEGORY_OHBIKI ;
        }
        else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                  chuuBunrui == CONST_CHUUBUNRUI_KETAZAI && shouBunrui == CONST_SHOUBUNRUI_KETAUKE ) {
          buzaiCategory = CONST_CATEGORY_KETAUKE ;
        }
        else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                  chuuBunrui == CONST_CHUUBUNRUI_KETAZAI && shouBunrui == CONST_SHOUBUNRUI_SHIKIGETA ) {
          buzaiCategory = CONST_CATEGORY_SHIKIGETA ;
        }
        else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                  chuuBunrui == CONST_CHUUBUNRUI_KETAZAI && shouBunrui == CONST_SHOUBUNRUI_JIFUKU ) {
          buzaiCategory = CONST_CATEGORY_JIFUKU ;
        }
        else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                  chuuBunrui == CONST_CHUUBUNRUI_KETAZAI && shouBunrui == CONST_SHOUBUNRUI_FUKKOUBANZUREDOMEZAI ) {
          buzaiCategory = CONST_CATEGORY_FUKKOUBANZUREDOMEZAI ;
        }
        else if ( ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                    chuuBunrui == CONST_CHUUBUNRUI_SHUZAI && shouBunrui == CONST_SHOUBUNRUI_SHICHU_SOZAI ) ||
                  ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                    chuuBunrui == CONST_CHUUBUNRUI_SHUZAI && shouBunrui == CONST_SHOUBUNRUI_SHICHU_YAMADOME ) ||
                  ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                    chuuBunrui == CONST_CHUUBUNRUI_SHUZAI &&
                    shouBunrui == CONST_SHOUBUNRUI_SHICHU_KOUKYOUDOYAMADOME ) ) {
          buzaiCategory = CONST_CATEGORY_SHICHU ;
        }
        else if ( daiBunrui == CONST_DAIBUNRUI_YAMADOMEHEKI && chuuBunrui == CONST_CHUUBUNRUI_SHIJIKUI &&
                  shouBunrui == CONST_SHOUBUNRUI_KUI ) {
          buzaiCategory = CONST_CATEGORY_SHIJIKUI ;
        }

        if ( ! string.IsNullOrEmpty( buzaiCategory ) ) {
          if ( ! _buzaiCategoryList.Exists( x => x == buzaiCategory ) ) {
            _buzaiCategoryList.Add( buzaiCategory ) ;
          }
        }
      }

      // 空の選択肢を追加
      this.CmbDaiBunrui.Items.Add( string.Empty ) ;
      this.CmbChuuBunrui.Items.Add( string.Empty ) ;
      this.CmbShouBunrui.Items.Add( string.Empty ) ;

      foreach ( var bunrui in _bunruiList ) {
        if ( ! this.CmbDaiBunrui.Items.Contains( bunrui.Dai ) ) {
          // 大分類コンボに値をセット
          this.CmbDaiBunrui.Items.Add( bunrui.Dai ) ;
        }

        if ( ! this.CmbChuuBunrui.Items.Contains( bunrui.Chuu ) ) {
          // 中分類コンボに値をセット
          this.CmbChuuBunrui.Items.Add( bunrui.Chuu ) ;
        }

        if ( ! this.CmbShouBunrui.Items.Contains( bunrui.Shou ) ) {
          // 小分類コンボに値をセット
          this.CmbShouBunrui.Items.Add( bunrui.Shou ) ;
        }
      }

      foreach ( var koudaiName in _koudaiNameList ) {
        // 構台名リストに値をセット
        this.LstKoudaiEditSize.Items.Add( koudaiName ) ;
        this.LstKoudaiEditSizeCategory.Items.Add( koudaiName ) ;
      }

      //var allKoudaiNameList = GantryUtil.GetAllKoudaiName(_uiDoc.Document).ToList();
      //foreach (var koudaiName in allKoudaiNameList)
      //{
      //    this.LstKoudaiEditSizeCategory.Items.Add(koudaiName);
      //}

      this.CmbCategory.Items.AddRange( _buzaiCategoryList.ToArray() ) ;
    }

    public void InitData()
    {
      var rowDataOfSizeList = CreateRowDataOfSizeList( _targetElementUniqueIdList ) ;

      // 個別一覧データグリッドに値をセット
      foreach ( var rowData in rowDataOfSizeList ) {
        var rowIndex = this.DgvEditSizeList.Rows.Add( new object[]
        {
          rowData.Id, rowData.ElementId, false, rowData.KoudaiName, rowData.DaiBunrui, rowData.ChuuBunrui,
          rowData.ShouBunrui, rowData.BeforeSize,
        } ) ;

        // 各行の変更後サイズ選択肢を作成
        CreateComboBoxSizeList( rowIndex ) ;
      }

      var rowDataOfSizeCategoryList = CreateRowDataOfSizeCategoryList( _targetElementUniqueIdList ) ;

      // 種別一覧データグリッドに値をセット
      foreach ( var rowData in rowDataOfSizeCategoryList ) {
        var rowIndex =
          this.DgvEditSizeCategoryList.Rows.Add( new object[] { false, rowData.KoudaiName, rowData.Category, } ) ;

        // 各行の変更後サイズ選択肢を作成
        CreateComboBoxSizeListOfCategory( rowIndex ) ;
      }

      this.DgvEditSizeList.ClearSelection() ;
      this.DgvEditSizeCategoryList.ClearSelection() ;
    }

    private void CreateComboBoxSizeList( int rowIndex )
    {
      var comboBoxCellOfAfterSize =
        (DataGridViewComboBoxCell) this.DgvEditSizeList.Rows[ rowIndex ].Cells[ "ColSizeListAfterSize" ] ;
      var daiBunrui = this.DgvEditSizeList.Rows[ rowIndex ].Cells[ "ColSizeListDaiBunrui" ].Value.ToString() ;
      var chuuBunrui = this.DgvEditSizeList.Rows[ rowIndex ].Cells[ "ColSizeListChuuBunrui" ].Value.ToString() ;
      var shouBunrui = this.DgvEditSizeList.Rows[ rowIndex ].Cells[ "ColSizeListShouBunrui" ].Value.ToString() ;

      var choicesSize = _choicesSizeList.FirstOrDefault( x =>
        x.BunruiOfChoicesSize.Dai == daiBunrui && x.BunruiOfChoicesSize.Chuu == chuuBunrui &&
        x.BunruiOfChoicesSize.Shou == shouBunrui ) ;
      if ( choicesSize != null ) {
        // 既に該当するサイズ選択肢が作成されている場合はそれを設定
        comboBoxCellOfAfterSize.Items.AddRange( choicesSize.ValueOfChoicesSize.ToArray() ) ;
        return ;
      }

      var masterTypes = new List<string>() ;
      var comboBoxSizeList = new List<string>() ;
      if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
           chuuBunrui == CONST_CHUUBUNRUI_FUKKOUBAN && shouBunrui == CONST_SHOUBUNRUI_FUKKOUBAN ) {
        masterTypes = new List<string>() { Master.ClsFukkoubanCsv.TypeM, Master.ClsFukkoubanCsv.TypeNormal } ;

        comboBoxSizeList = CreateComboBoxFukkoubanSizeList( masterTypes ) ;
      }
      else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                chuuBunrui == CONST_CHUUBUNRUI_KETAZAI && shouBunrui == CONST_SHOUBUNRUI_OHBIKI ) {
        masterTypes = new List<string>()
        {
          Master.ClsHBeamCsv.TypeHiro,
          Master.ClsHBeamCsv.TypeNaka,
          Master.ClsHBeamCsv.TypeHoso,
          Master.ClsYamadomeCsv.TypeShuzai,
          Master.ClsYamadomeCsv.TypeHighShuzai
        } ;

        comboBoxSizeList = CreateComboBoxKetazaiSizeList( masterTypes ) ;
      }
      else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                chuuBunrui == CONST_CHUUBUNRUI_KETAZAI && shouBunrui == CONST_SHOUBUNRUI_KETAUKE ) {
        masterTypes = new List<string>()
        {
          Master.ClsHBeamCsv.TypeHiro,
          Master.ClsHBeamCsv.TypeNaka,
          Master.ClsHBeamCsv.TypeHoso,
          Master.ClsAngleCsv.TypeAngle,
          Master.ClsChannelCsv.TypeCannel
        } ;

        comboBoxSizeList = CreateComboBoxKetazaiSizeList( masterTypes ) ;
      }
      else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                chuuBunrui == CONST_CHUUBUNRUI_KETAZAI && shouBunrui == CONST_SHOUBUNRUI_NEDA ) {
        masterTypes = new List<string>()
        {
          Master.ClsHBeamCsv.TypeHiro,
          Master.ClsHBeamCsv.TypeNaka,
          Master.ClsHBeamCsv.TypeHoso,
          Master.ClsYamadomeCsv.TypeShuzai,
          Master.ClsYamadomeCsv.TypeHighShuzai
        } ;

        comboBoxSizeList = CreateComboBoxKetazaiSizeList( masterTypes ) ;
      }
      else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                chuuBunrui == CONST_CHUUBUNRUI_KETAZAI && shouBunrui == CONST_SHOUBUNRUI_SHUGETA ) {
        masterTypes = new List<string>()
        {
          Master.ClsHBeamCsv.TypeHiro,
          Master.ClsHBeamCsv.TypeNaka,
          Master.ClsHBeamCsv.TypeHoso,
          Master.ClsYamadomeCsv.TypeShuzai,
          Master.ClsYamadomeCsv.TypeHighShuzai
        } ;

        comboBoxSizeList = CreateComboBoxKetazaiSizeList( masterTypes ) ;
      }
      else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                chuuBunrui == CONST_CHUUBUNRUI_KETAZAI && shouBunrui == CONST_SHOUBUNRUI_SHIKIGETA ) {
        masterTypes = new List<string>()
        {
          Master.ClsHBeamCsv.TypeHiro,
          Master.ClsHBeamCsv.TypeNaka,
          Master.ClsHBeamCsv.TypeHoso,
          Master.ClsYamadomeCsv.TypeShuzai,
          Master.ClsYamadomeCsv.TypeHighShuzai
        } ;

        comboBoxSizeList = CreateComboBoxKetazaiSizeList( masterTypes ) ;
      }
      else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                chuuBunrui == CONST_CHUUBUNRUI_KETAZAI && shouBunrui == CONST_SHOUBUNRUI_FUKKOUGETA ) {
        masterTypes = new List<string>()
        {
          Master.ClsFukkouGetaCsv.TypeH, Master.ClsFukkouGetaCsv.TypeC, Master.ClsFukkouGetaCsv.TypeYamadome
        } ;

        comboBoxSizeList = CreateComboBoxFukkouGetaSizeList( masterTypes ) ;
      }
      else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                chuuBunrui == CONST_CHUUBUNRUI_KETAZAI && shouBunrui == CONST_SHOUBUNRUI_TAIKEIKOU ) {
        comboBoxSizeList = CreateComboBoxTaikeikouSizeList() ;
      }
      else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                chuuBunrui == CONST_CHUUBUNRUI_SONOTA && shouBunrui == CONST_SHOUBUNRUI_SLOPEMAZUMEZAI ) {
        comboBoxSizeList = CreateComboBoxSlopeMazumezaiSizeList() ;
      }
      else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                chuuBunrui == CONST_CHUUBUNRUI_SONOTA && shouBunrui == CONST_SHOUBUNRUI_FUKKOUBANZUREDOMEZAI ) {
        //var elementIdValue = (int)this.DgvEditSizeList.Rows[rowIndex].Cells["ColSizeListElementID"].Value;
        //var elementId = new ElementId(elementIdValue);
        //var familyInstance = GantryUtil.GetFamilyInstance(_uiDoc.Document, elementId);
        //var material = JifukuZuredomezai.ReadFromElement(familyInstance.Id, _uiDoc.Document);
        //var isTypeC = material.m_Size.StartsWith("[");

        comboBoxSizeList = CreateComboBoxZuredomeSizeList( false ) ;
      }
      else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                chuuBunrui == CONST_CHUUBUNRUI_KETAZAI && shouBunrui == CONST_SHOUBUNRUI_FUKKOUBANZUREDOMEZAI ) {
        //var elementIdValue = (int)this.DgvEditSizeList.Rows[rowIndex].Cells["ColSizeListElementID"].Value;
        //var elementId = new ElementId(elementIdValue);
        //var familyInstance = GantryUtil.GetFamilyInstance(_uiDoc.Document, elementId);
        //var material = JifukuZuredomezai.ReadFromElement(familyInstance.Id, _uiDoc.Document);
        //var isTypeC = material.m_Size.StartsWith("[");

        comboBoxSizeList = CreateComboBoxZuredomeSizeList( true ) ;
      }
      else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                chuuBunrui == CONST_CHUUBUNRUI_SONOTA && shouBunrui == CONST_SHOUBUNRUI_SUCHIFUNAJACK ) {
        comboBoxSizeList = CreateComboBoxSuchifunaJackSizeList() ;
      }
      else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                chuuBunrui == CONST_CHUUBUNRUI_SONOTA && shouBunrui == CONST_SHOUBUNRUI_TORITSUKEHOJYOZAI ) {
        comboBoxSizeList = CreateComboBoxToritsukehojyozaiSizeList() ;
      }
      else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                chuuBunrui == CONST_CHUUBUNRUI_PL && shouBunrui == CONST_SHOUBUNRUI_TAIKEIKOUPL ) {
        comboBoxSizeList = CreateComboBoxTaikeikouPLSizeList() ;
      }
      else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                chuuBunrui == CONST_CHUUBUNRUI_PL && shouBunrui == CONST_SHOUBUNRUI_SUCHIFUNAPL ) {
        comboBoxSizeList = CreateComboBoxSuchifunaPLSizeList() ;
      }
      else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                chuuBunrui == CONST_CHUUBUNRUI_SONOTA && shouBunrui == CONST_SHOUBUNRUI_TAKASACHOUSEIPIEACE ) {
        comboBoxSizeList = CreateComboBoxTakasaChouseiPieaceSizeList() ;
      }
      else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                chuuBunrui == CONST_CHUUBUNRUI_TAKASACHOUSEIPIEACE && shouBunrui == CONST_SHOUBUNRUI_KAKOUHIN ) {
        comboBoxSizeList = CreateComboBoxTakasaChouseiPieaceHBeamSizeList() ;
      }
      else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                chuuBunrui == CONST_CHUUBUNRUI_PL && shouBunrui == CONST_SHOUBUNRUI_TAKASACHOUSEIPL ) {
        comboBoxSizeList = CreateComboBoxTakasaChouseiPLSizeList() ;
      }
      else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                chuuBunrui == CONST_CHUUBUNRUI_FUKKOUBAN && shouBunrui == CONST_SHOUBUNRUI_SHIMAKOUBAN ) {
        comboBoxSizeList = CreateComboBoxShimaKoubanSizeList() ;
      }
      else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                chuuBunrui == CONST_CHUUBUNRUI_FUKKOUBAN && shouBunrui == CONST_SHOUBUNRUI_SHIKITEPPAN ) {
        comboBoxSizeList = CreateComboBoxShikiteppanSizeList() ;
      }
      else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                chuuBunrui == CONST_CHUUBUNRUI_KETAZAI && shouBunrui == CONST_SHOUBUNRUI_JIFUKU ) {
        comboBoxSizeList = CreateComboBoxJifukuSizeList() ;
      }
      else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                chuuBunrui == CONST_CHUUBUNRUI_KETAZAI && shouBunrui == CONST_SHOUBUNRUI_TESURI ) {
        comboBoxSizeList = CreateComboBoxTesuriSizeList() ;
      }
      else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                chuuBunrui == CONST_CHUUBUNRUI_SONOTA && shouBunrui == CONST_SHOUBUNRUI_TESURISHICHU ) {
        comboBoxSizeList = CreateComboBoxTesuriShichuSizeList() ;
      }
      else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                chuuBunrui == CONST_CHUUBUNRUI_KETAZAI && shouBunrui == CONST_SHOUBUNRUI_BRACE ) {
        comboBoxSizeList = CreateComboBoxBraceSizeList() ;
      }
      else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                chuuBunrui == CONST_CHUUBUNRUI_BRACE && shouBunrui == CONST_SHOUBUNRUI_TURNBUCKLEBRACE ) {
        comboBoxSizeList = CreateComboBoxTurnBuckleBraceSizeList() ;
      }
      else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                chuuBunrui == CONST_CHUUBUNRUI_TEIKETSUKANAGU && shouBunrui == CONST_SHOUBUNRUI_BULMAN ) {
        comboBoxSizeList = CreateComboBoxBulmanSizeList() ;
      }
      else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                chuuBunrui == CONST_CHUUBUNRUI_TEIKETSUKANAGU && shouBunrui == CONST_SHOUBUNRUI_RIKIMAN ) {
        comboBoxSizeList = CreateComboBoxRikimanSizeList() ;
      }
      else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                chuuBunrui == CONST_CHUUBUNRUI_KETAZAI && shouBunrui == CONST_SHOUBUNRUI_TSUNAGI ) {
        comboBoxSizeList = CreateComboBoxTsunagiSizeList() ;
      }
      else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                chuuBunrui == CONST_CHUUBUNRUI_KETAZAI && shouBunrui == CONST_SHOUBUNRUI_HOUDUE ) {
        comboBoxSizeList = CreateComboBoxHouzueYamadomeSizeList() ;
      }
      else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                chuuBunrui == CONST_CHUUBUNRUI_SHUZAI && shouBunrui == CONST_SHOUBUNRUI_HIUCHIUKE ) {
        comboBoxSizeList = CreateComboBoxHouzueHiuchiUkeSizeList() ;
      }
      else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                chuuBunrui == CONST_CHUUBUNRUI_SONOTA && shouBunrui == CONST_SHOUBUNRUI_HOUDUE ) {
        comboBoxSizeList = CreateComboBoxHouzueSozaiSizeList() ;
      }
      else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                chuuBunrui == CONST_CHUUBUNRUI_SHUZAI && shouBunrui == CONST_SHOUBUNRUI_SUMIBUPIEACE ) {
        comboBoxSizeList = CreateComboBoxHouzueSumibuPieaceSizeList() ;
      }
      else if ( daiBunrui == CONST_DAIBUNRUI_KOUDAIKUI && chuuBunrui == CONST_CHUUBUNRUI_TSUGITASHIKUI &&
                shouBunrui == CONST_SHOUBUNRUI_TSUGITASHIKUI ) {
        comboBoxSizeList = CreateComboBoxTsugitashiKuiSizeList() ;
      }

      if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
           chuuBunrui == CONST_CHUUBUNRUI_SHUZAI && shouBunrui == CONST_SHOUBUNRUI_SHICHU_SOZAI ) {
        comboBoxSizeList = CreateComboBoxShichuSozaiSizeList() ;
      }

      if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
           chuuBunrui == CONST_CHUUBUNRUI_SHUZAI && shouBunrui == CONST_SHOUBUNRUI_SHICHU_YAMADOME ) {
        comboBoxSizeList = CreateComboBoxShichuYamadomeSizeList() ;
      }

      if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
           chuuBunrui == CONST_CHUUBUNRUI_SHUZAI && shouBunrui == CONST_SHOUBUNRUI_SHICHU_KOUKYOUDOYAMADOME ) {
        comboBoxSizeList = CreateComboBoxShichuKoukyoudoYamadomeSizeList() ;
      }

      if ( daiBunrui == CONST_DAIBUNRUI_YAMADOMEHEKI && chuuBunrui == CONST_CHUUBUNRUI_SHIJIKUI &&
           shouBunrui == CONST_SHOUBUNRUI_KUI ) {
        masterTypes = new List<string>()
        {
          Master.ClsHBeamCsv.TypeHiro, Master.ClsHBeamCsv.TypeNaka, Master.ClsHBeamCsv.TypeHoso
        } ;

        comboBoxSizeList = CreateComboBoxKuiSizeList( masterTypes ) ;
      }

      if ( comboBoxSizeList.Count() == 0 ) {
        // サイズ選択肢を取得できないものは編集不可
        comboBoxCellOfAfterSize.ReadOnly = true ;
      }

      comboBoxCellOfAfterSize.Items.AddRange( comboBoxSizeList.ToArray() ) ;

      // 一度作成したカテゴリのサイズ選択肢を保存
      choicesSize = new ChoicesSize
      {
        BunruiOfChoicesSize = new Bunrui { Dai = daiBunrui, Chuu = chuuBunrui, Shou = shouBunrui },
        ValueOfChoicesSize = comboBoxSizeList
      } ;
      _choicesSizeList.Add( choicesSize ) ;
    }

    private void CreateComboBoxSizeListOfCategory( int rowIndex )
    {
      var comboBoxCellOfAfterSize =
        (DataGridViewComboBoxCell) this.DgvEditSizeCategoryList.Rows[ rowIndex ]
          .Cells[ "ColSizeCategoryListAfterSize" ] ;
      var category = this.DgvEditSizeCategoryList.Rows[ rowIndex ].Cells[ "ColSizeCategoryListCategory" ].Value
        .ToString() ;

      var choicesSizeCategory = _choicesSizeOfCategoryList.FirstOrDefault( x => x.CategoryOfChoicesSize == category ) ;
      if ( choicesSizeCategory != null ) {
        // 既に該当するサイズ選択肢が作成されている場合はそれを設定
        comboBoxCellOfAfterSize.Items.AddRange( choicesSizeCategory.ValueOfChoicesSize.ToArray() ) ;
        return ;
      }

      var masterTypes = new List<string>() ;
      var comboBoxSizeList = new List<string>() ;

      if ( category == CONST_CATEGORY_SHICHU ) {
        comboBoxSizeList = CreateComboBoxShichuSozaiSizeList() ;
        comboBoxSizeList.AddRange( CreateComboBoxShichuYamadomeSizeList().Where( x => ! string.IsNullOrEmpty( x ) )
          .ToList() ) ;
        comboBoxSizeList.AddRange( CreateComboBoxShichuKoukyoudoYamadomeSizeList()
          .Where( x => ! string.IsNullOrEmpty( x ) ).ToList() ) ;
      }
      else if ( category == CONST_CATEGORY_OHBIKI ) {
        masterTypes = new List<string>()
        {
          Master.ClsHBeamCsv.TypeHiro,
          Master.ClsHBeamCsv.TypeNaka,
          Master.ClsHBeamCsv.TypeHoso,
          Master.ClsYamadomeCsv.TypeShuzai,
          Master.ClsYamadomeCsv.TypeHighShuzai
        } ;

        comboBoxSizeList = CreateComboBoxKetazaiSizeList( masterTypes ) ;
      }
      else if ( category == CONST_CATEGORY_KETAUKE ) {
        masterTypes = new List<string>()
        {
          Master.ClsHBeamCsv.TypeHiro,
          Master.ClsHBeamCsv.TypeNaka,
          Master.ClsHBeamCsv.TypeHoso,
          Master.ClsAngleCsv.TypeAngle,
          Master.ClsChannelCsv.TypeCannel
        } ;

        comboBoxSizeList = CreateComboBoxKetazaiSizeList( masterTypes ) ;
      }
      else if ( category == CONST_CATEGORY_SHIKIGETA ) {
        masterTypes = new List<string>()
        {
          Master.ClsHBeamCsv.TypeHiro,
          Master.ClsHBeamCsv.TypeNaka,
          Master.ClsHBeamCsv.TypeHoso,
          Master.ClsYamadomeCsv.TypeShuzai,
          Master.ClsYamadomeCsv.TypeHighShuzai
        } ;

        comboBoxSizeList = CreateComboBoxKetazaiSizeList( masterTypes ) ;
      }
      else if ( category == CONST_CATEGORY_NEDA ) {
        masterTypes = new List<string>()
        {
          Master.ClsHBeamCsv.TypeHiro,
          Master.ClsHBeamCsv.TypeNaka,
          Master.ClsHBeamCsv.TypeHoso,
          Master.ClsYamadomeCsv.TypeShuzai,
          Master.ClsYamadomeCsv.TypeHighShuzai
        } ;

        comboBoxSizeList = CreateComboBoxKetazaiSizeList( masterTypes ) ;
      }
      else if ( category == CONST_CATEGORY_SHUGETA ) {
        masterTypes = new List<string>()
        {
          Master.ClsHBeamCsv.TypeHiro,
          Master.ClsHBeamCsv.TypeNaka,
          Master.ClsHBeamCsv.TypeHoso,
          Master.ClsYamadomeCsv.TypeShuzai,
          Master.ClsYamadomeCsv.TypeHighShuzai
        } ;

        comboBoxSizeList = CreateComboBoxKetazaiSizeList( masterTypes ) ;
      }
      else if ( category == CONST_CATEGORY_JIFUKU ) {
        comboBoxSizeList = CreateComboBoxJifukuSizeList() ;
      }
      else if ( category == CONST_CATEGORY_FUKKOUBANZUREDOMEZAI ) {
        comboBoxSizeList = CreateComboBoxZuredomeSizeList() ;
      }
      else if ( category == CONST_CATEGORY_SHIJIKUI ) {
        masterTypes = new List<string>()
        {
          Master.ClsHBeamCsv.TypeHiro, Master.ClsHBeamCsv.TypeNaka, Master.ClsHBeamCsv.TypeHoso
        } ;

        comboBoxSizeList = CreateComboBoxKuiSizeList( masterTypes ) ;
      }

      comboBoxCellOfAfterSize.Items.AddRange( comboBoxSizeList.ToArray() ) ;

      // 一度作成したカテゴリのサイズ選択肢を保存
      choicesSizeCategory =
        new ChoicesSizeOfCategory { CategoryOfChoicesSize = category, ValueOfChoicesSize = comboBoxSizeList } ;
      _choicesSizeOfCategoryList.Add( choicesSizeCategory ) ;
    }

    #region 部材分類毎のComboBox選択肢を作成

    private List<string> CreateComboBoxFukkoubanSizeList( List<string> masterTypes )
    {
      List<string> lstStr = new List<string>() ;
      lstStr.Add( string.Empty ) ;

      foreach ( var masterType in masterTypes ) {
        lstStr.AddRange( Master.ClsFukkoubanCsv.GetSizeList( masterType ) ) ;
      }

      return lstStr ;
    }

    private List<string> CreateComboBoxKetazaiSizeList( List<string> masterTypes )
    {
      List<string> lstStr = new List<string>() ;
      lstStr.Add( string.Empty ) ;

      foreach ( var masterType in masterTypes ) {
        if ( masterType == Master.ClsYamadomeCsv.TypeShuzai || masterType == Master.ClsYamadomeCsv.TypeHighShuzai ) //主材
        {
          lstStr.AddRange( Master.ClsYamadomeCsv.GetSizeList( masterType ) ) ;
        }
        else if ( masterType == Master.ClsHBeamCsv.TypeHoso || masterType == Master.ClsHBeamCsv.TypeNaka ||
                  masterType == Master.ClsHBeamCsv.TypeHiro ) //H鋼
        {
          lstStr.AddRange( Master.ClsHBeamCsv.GetSizeList( masterType ) ) ;
        }
        else if ( masterType == Master.ClsChannelCsv.TypeCannel ) //C材
        {
          lstStr.AddRange( Master.ClsChannelCsv.GetSizeList() ) ;
        }
        else if ( masterType == Master.ClsAngleCsv.TypeAngle ) //L材
        {
          lstStr.AddRange( Master.ClsAngleCsv.GetSizeList( masterType ) ) ;
        }
      }

      return lstStr ;
    }

    private List<string> CreateComboBoxFukkouGetaSizeList( List<string> masterTypes )
    {
      List<string> lstStr = new List<string>() ;
      lstStr.Add( string.Empty ) ;

      foreach ( var masterType in masterTypes ) {
        lstStr.AddRange( Master.ClsFukkouGetaCsv.GetSizeList( masterType ) ) ;
      }

      return lstStr ;
    }

    private List<string> CreateComboBoxTaikeikouSizeList()
    {
      List<string> lstStr = new List<string>() ;
      lstStr.Add( string.Empty ) ;
      lstStr.AddRange( Master.ClsTaikeikouCsv.GetAllSizeList() ) ;

      return lstStr ;
    }

    private List<string> CreateComboBoxSlopeMazumezaiSizeList()
    {
      List<string> lstStr = new List<string>() ;
      lstStr.Add( string.Empty ) ;
      lstStr.AddRange( Master.ClsMasterCsv.Shared.Where( x => ! string.IsNullOrEmpty( x.MadumeSymbol ) )
        .Select( x => x.Size ).ToList() ) ;

      return lstStr ;
    }

    private List<string> CreateComboBoxZuredomeSizeList( bool isTypeC = true )
    {
      List<string> lstStr = new List<string>() ;
      lstStr.Add( string.Empty ) ;

      if ( isTypeC ) {
        lstStr.AddRange( Master.ClsJifukuZuredomeCsv.GetSizeList( Master.ClsJifukuZuredomeCsv.TypeZuredomezai,
          Master.ClsJifukuZuredomeCsv.TypeC ) ) ;
        return lstStr ;
      }

      lstStr.AddRange( Master.ClsJifukuZuredomeCsv.GetSizeList( Master.ClsJifukuZuredomeCsv.TypeZuredomezai,
        Master.ClsJifukuZuredomeCsv.TypeL ) ) ;

      return lstStr ;
    }

    private List<string> CreateComboBoxJifukuSizeList()
    {
      List<string> lstStr = new List<string>() ;
      lstStr.Add( string.Empty ) ;
      lstStr.AddRange( Master.ClsJifukuZuredomeCsv.GetSizeList( Master.ClsJifukuZuredomeCsv.TypeJifuku,
        Master.ClsJifukuZuredomeCsv.TypeC ) ) ;

      return lstStr ;
    }

    private List<string> CreateComboBoxSuchifunaJackSizeList()
    {
      List<string> lstStr = new List<string>() ;
      lstStr.Add( string.Empty ) ;
      lstStr.AddRange( Master.ClsStiffenerCsv.GetSizeList( Master.ClsStiffenerCsv.SHJack ) ) ;
      lstStr.AddRange( Master.ClsStiffenerCsv.GetSizeList( Master.ClsStiffenerCsv.DWJJack ) ) ;


      return lstStr ;
    }

    private List<string> CreateComboBoxToritsukehojyozaiSizeList()
    {
      List<string> lstStr = new List<string>() ;
      lstStr.Add( string.Empty ) ;
      lstStr.AddRange( Master.ClsTeiketsuHojoCsv.GetSizeList( Master.ClsTeiketsuHojoCsv.TypeNeko ) ) ;

      return lstStr ;
    }

    private List<string> CreateComboBoxTaikeikouPLSizeList()
    {
      List<string> lstStr = new List<string>() ;
      lstStr.Add( string.Empty ) ;
      lstStr.AddRange( Master.ClsTaikeikouPLCsv.GetAllPLSizeList() ) ;

      return lstStr ;
    }

    private List<string> CreateComboBoxSuchifunaPLSizeList()
    {
      List<string> lstStr = new List<string>() ;
      lstStr.Add( string.Empty ) ;
      lstStr.AddRange( Master.ClsStiffenerCsv.GetSizeList( Master.ClsStiffenerCsv.PL ) ) ;

      return lstStr ;
    }

    private List<string> CreateComboBoxTakasaChouseiPieaceSizeList()
    {
      List<string> lstStr = new List<string>() ;
      lstStr.Add( string.Empty ) ;
      var sizeList = Master.ClsTakasaChouseiCsv.GetSizeList( Master.ClsTakasaChouseiCsv.TypePieace ) ;
      // H鋼(可変)のサイズでない対象を追加
      lstStr.AddRange( sizeList.Where( x => ! x.StartsWith( "H" ) ) ) ;

      return lstStr ;
    }

    private List<string> CreateComboBoxTakasaChouseiPieaceHBeamSizeList()
    {
      List<string> lstStr = new List<string>() ;
      lstStr.Add( string.Empty ) ;
      var sizeList = Master.ClsTakasaChouseiCsv.GetSizeList( Master.ClsTakasaChouseiCsv.TypePieace ) ;
      // H鋼(可変)のサイズである対象を追加
      lstStr.AddRange( sizeList.Where( x => x.StartsWith( "H" ) ) ) ;

      return lstStr ;
    }

    private List<string> CreateComboBoxTakasaChouseiPLSizeList()
    {
      List<string> lstStr = new List<string>() ;
      lstStr.Add( string.Empty ) ;
      lstStr.AddRange( Master.ClsTakasaChouseiCsv.GetSizeList( Master.ClsTakasaChouseiCsv.TypePlate ) ) ;

      return lstStr ;
    }

    private List<string> CreateComboBoxShimaKoubanSizeList()
    {
      List<string> lstStr = new List<string>() ;
      lstStr.Add( string.Empty ) ;
      lstStr.AddRange( Master.ClsKoubanzaiCsv.GetSizeList( Master.ClsKoubanzaiCsv.TypeShimaKouban ) ) ;

      return lstStr ;
    }

    private List<string> CreateComboBoxShikiteppanSizeList()
    {
      List<string> lstStr = new List<string>() ;
      lstStr.Add( string.Empty ) ;
      lstStr.AddRange( Master.ClsKoubanzaiCsv.GetSizeList( Master.ClsKoubanzaiCsv.TypeShikiteppan ) ) ;

      return lstStr ;
    }

    private List<string> CreateComboBoxTesuriSizeList()
    {
      List<string> lstStr = new List<string>() ;
      lstStr.Add( string.Empty ) ;
      lstStr.AddRange( Master.ClsTesuriCsv.GetSizeList( Master.ClsTesuriCsv.TypeTesuri ) ) ;

      return lstStr ;
    }

    private List<string> CreateComboBoxTesuriShichuSizeList()
    {
      List<string> lstStr = new List<string>() ;
      lstStr.Add( string.Empty ) ;
      lstStr.AddRange( Master.ClsTesuriCsv.GetSizeList( Master.ClsTesuriCsv.TypeTesuriShichu ) ) ;

      return lstStr ;
    }

    private List<string> CreateComboBoxBraceSizeList()
    {
      List<string> lstStr = new List<string>() ;
      lstStr.Add( string.Empty ) ;
      lstStr.AddRange( Master.ClsMasterCsv.Shared.Where( x => x.IsBrace ).Select( x => x.Size ).ToList() ) ;

      return lstStr ;
    }

    private List<string> CreateComboBoxTurnBuckleBraceSizeList()
    {
      List<string> lstStr = new List<string>() ;
      lstStr.Add( string.Empty ) ;
      lstStr.AddRange( Master.ClsTurnBackleCsv.GetSizeList( Master.ClsTurnBackleCsv.TurnBackle ) ) ;

      return lstStr ;
    }

    private List<string> CreateComboBoxBulmanSizeList()
    {
      List<string> lstStr = new List<string>() ;
      lstStr.Add( string.Empty ) ;
      lstStr.AddRange( Master.ClsTeiketsuHojoCsv.GetSizeList( Master.ClsTeiketsuHojoCsv.TypeBulman ) ) ;

      return lstStr ;
    }

    private List<string> CreateComboBoxRikimanSizeList()
    {
      List<string> lstStr = new List<string>() ;
      lstStr.Add( string.Empty ) ;
      lstStr.AddRange( Master.ClsTeiketsuHojoCsv.GetSizeList( Master.ClsTeiketsuHojoCsv.TypeRikiman ) ) ;

      return lstStr ;
    }

    private List<string> CreateComboBoxTsunagiSizeList()
    {
      List<string> lstStr = new List<string>() ;
      lstStr.Add( string.Empty ) ;
      lstStr.AddRange( Master.ClsMasterCsv.Shared.Where( x => x.IsHrzJoint ).Select( x => x.Size ).ToList() ) ;

      return lstStr ;
    }

    private List<string> CreateComboBoxHouzueYamadomeSizeList()
    {
      List<string> lstStr = new List<string>() ;
      lstStr.Add( string.Empty ) ;
      lstStr.AddRange( Master.ClsHoudueCsv.GetSizeList( Master.ClsHoudueCsv.TypeYamadome ) ) ;

      return lstStr ;
    }

    private List<string> CreateComboBoxHouzueHiuchiUkeSizeList()
    {
      List<string> lstStr = new List<string>() ;
      lstStr.Add( string.Empty ) ;
      lstStr.AddRange( Master.ClsHoudueCsv.GetSizeList( Master.ClsHoudueCsv.TypeHiuchiUke ) ) ;

      return lstStr ;
    }

    private List<string> CreateComboBoxHouzueSozaiSizeList()
    {
      List<string> lstStr = new List<string>() ;
      lstStr.Add( string.Empty ) ;
      lstStr.AddRange( Master.ClsHoudueCsv.GetSizeList( Master.ClsHoudueCsv.TypeSozai ) ) ;

      return lstStr ;
    }

    private List<string> CreateComboBoxHouzueSumibuPieaceSizeList()
    {
      List<string> lstStr = new List<string>() ;
      lstStr.Add( string.Empty ) ;
      lstStr.AddRange( Master.ClsHoudueCsv.GetSizeList( Master.ClsHoudueCsv.TypeSumibu ) ) ;

      return lstStr ;
    }

    private List<string> CreateComboBoxTsugitashiKuiSizeList()
    {
      List<string> lstStr = new List<string>() ;
      lstStr.Add( string.Empty ) ;

      List<string> tmpSizeList = new List<string>() ;
      tmpSizeList.AddRange( Master.ClsYamadomeCsv.GetSizeList( Master.ClsYamadomeCsv.TypeShuzai ) ) ;
      tmpSizeList.AddRange( Master.ClsYamadomeCsv.GetSizeList( Master.ClsYamadomeCsv.TypeHighShuzai ) ) ;
      tmpSizeList.AddRange( Master.ClsHBeamCsv.GetSizeList( Master.ClsHBeamCsv.TypeHoso, true ) ) ;
      tmpSizeList.AddRange( Master.ClsHBeamCsv.GetSizeList( Master.ClsHBeamCsv.TypeNaka, true ) ) ;
      tmpSizeList.AddRange( Master.ClsHBeamCsv.GetSizeList( Master.ClsHBeamCsv.TypeHiro, true ) ) ;

      foreach ( var size in tmpSizeList ) {
        if ( Master.ClsHBeamCsv.IsExPileFamily( size ) ) {
          // 該当サイズの継ぎ足し杭のパスが存在するか確認してあれば追加。
          lstStr.Add( size ) ;
        }
      }

      return lstStr ;
    }

    private List<string> CreateComboBoxShichuSozaiSizeList()
    {
      List<string> lstStr = new List<string>() ;
      lstStr.Add( string.Empty ) ;
      lstStr.AddRange( Master.ClsSichuCsv.GetSizeList( Master.ClsSichuCsv.HKou ) ) ;

      return lstStr ;
    }

    private List<string> CreateComboBoxShichuYamadomeSizeList()
    {
      List<string> lstStr = new List<string>() ;
      lstStr.Add( string.Empty ) ;
      lstStr.AddRange( Master.ClsSichuCsv.GetSizeList( Master.ClsSichuCsv.Yamadome ) ) ;

      return lstStr ;
    }

    private List<string> CreateComboBoxShichuKoukyoudoYamadomeSizeList()
    {
      List<string> lstStr = new List<string>() ;
      lstStr.Add( string.Empty ) ;
      lstStr.AddRange( Master.ClsSichuCsv.GetSizeList( Master.ClsSichuCsv.Koukyoudo ) ) ;

      return lstStr ;
    }

    private List<string> CreateComboBoxKuiSizeList( List<string> masterTypes )
    {
      List<string> lstStr = new List<string>() ;
      lstStr.Add( string.Empty ) ;

      foreach ( var masterType in masterTypes ) {
        if ( masterType == Master.ClsYamadomeCsv.TypeShuzai || masterType == Master.ClsYamadomeCsv.TypeHighShuzai ) //主材
        {
          lstStr.AddRange( Master.ClsYamadomeCsv.GetSizeList( masterType ) ) ;
        }
        else if ( masterType == Master.ClsHBeamCsv.TypeHoso || masterType == Master.ClsHBeamCsv.TypeNaka ||
                  masterType == Master.ClsHBeamCsv.TypeHiro ) //H鋼
        {
          lstStr.AddRange( Master.ClsHBeamCsv.GetSizeList( masterType, true ) ) ;
        }
      }

      return lstStr ;
    }

    #endregion

    private List<RowDataOfSizeList> CreateRowDataOfSizeList( List<string> elementUniqueIds )
    {
      var rowDataList = new List<RowDataOfSizeList>() ;

      foreach ( var elementUniqueId in elementUniqueIds ) {
        // エレメント取得
        var element = _uiDoc.Document.GetElement( elementUniqueId ) ;
        if ( element == null ) continue ;

        var elementId = element.Id ;

        // ファミリ名取得
        var familyName = GantryUtil.GetFamilyName( element ) ;
        // タイプ名取得
        var typeName = GantryUtil.GetTypeName( element ) ;
        // ファミリインスタンス取得
        var familyInstance = GantryUtil.GetFamilyInstance( _uiDoc.Document, elementId ) ;
        // ファミリシンボル取得
        var familySymbol = ClsRevitUtil.GetFamilySymbol( _uiDoc.Document, familyName, typeName ) ;

        if ( familyInstance == null || familySymbol == null ) continue ;

        // ネストされたファミリである場合はレコードの追加をスキップ
        if ( familyInstance.SuperComponent != null ) continue ;

        var material = MaterialSuper.ReadFromElement( familyInstance.Id, _uiDoc.Document ) ;
        // Revit標準機能で配置された(部材情報のElementId有無で判断)ファミリの場合はスキップ
        // システムで配置した部材については部材情報にElementId持ってるはず。
        // 未割当部材への構台名付与機能で構台名のみをセットされている場合があるがそれをスキップしたいという意図。
        if ( material == null || material.m_ElementId == null ) continue ;

        // 「構台基点ファミリ」の場合はレコード追加をスキップ
        if ( familyName == KoudaiReference.familyName ) continue ;

        var id = element.UniqueId ;
        var elemId = element.Id.IntegerValue ;
        var koudaiName = material?.m_KodaiName ;
        var daiBunrui = GantryUtil.GetTypeParameter( familySymbol, DefineUtil.PARAM_DAI_BUNRUI ) ;
        var chuuBunrui = GantryUtil.GetTypeParameter( familySymbol, DefineUtil.PARAM_CHUU_BUNRUI ) ;
        var shouBunrui = GantryUtil.GetTypeParameter( familySymbol, DefineUtil.PARAM_SHOU_BUNRUI ) ;
        //var size = GantryUtil.GetTypeParameter(familySymbol, DefineUtil.PARAM_SIZE);

        //// ファミリのサイズパラメータを取得できないものはスキップ
        //if (string.IsNullOrEmpty(size)) continue;

        if ( ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
               chuuBunrui == CONST_CHUUBUNRUI_SONOTA && shouBunrui == CONST_SHOUBUNRUI_SUCHIFUNAJACK ) ||
             ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
               chuuBunrui == CONST_CHUUBUNRUI_PL && shouBunrui == CONST_SHOUBUNRUI_TAIKEIKOUPL ) ||
             ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
               chuuBunrui == CONST_CHUUBUNRUI_PL && shouBunrui == CONST_SHOUBUNRUI_SUCHIFUNAPL ) ) {
          // サイズ変更時に対象としない(桁材のサイズに依存するため)
          continue ;
        }

        var rowData = new RowDataOfSizeList
        {
          Id = id,
          ElementId = elemId,
          KoudaiName = koudaiName,
          DaiBunrui = daiBunrui,
          ChuuBunrui = chuuBunrui,
          ShouBunrui = shouBunrui,
          BeforeSize = familyName,
        } ;

        rowDataList.Add( rowData ) ;
      }

      return rowDataList ;
    }

    private List<RowDataOfSizeCategoryList> CreateRowDataOfSizeCategoryList( List<string> elementUniqueIds )
    {
      var rowDataList = new List<RowDataOfSizeCategoryList>() ;

      foreach ( var elementUniqueId in elementUniqueIds ) {
        // エレメント取得
        var element = _uiDoc.Document.GetElement( elementUniqueId ) ;
        if ( element == null ) continue ;

        var elementId = element.Id ;

        // ファミリ名取得
        var familyName = GantryUtil.GetFamilyName( element ) ;
        // タイプ名取得
        var typeName = GantryUtil.GetTypeName( element ) ;
        // ファミリインスタンス取得
        var familyInstance = GantryUtil.GetFamilyInstance( _uiDoc.Document, elementId ) ;
        // ファミリシンボル取得
        var familySymbol = ClsRevitUtil.GetFamilySymbol( _uiDoc.Document, familyName, typeName ) ;

        if ( familyInstance == null || familySymbol == null ) continue ;

        // ネストされたファミリである場合はレコードの追加をスキップ
        if ( familyInstance.SuperComponent != null ) continue ;

        var material = MaterialSuper.ReadFromElement( familyInstance.Id, _uiDoc.Document ) ;
        // Revit標準機能で配置された(部材情報のElementId有無で判断)ファミリの場合はスキップ
        // システムで配置した部材については部材情報にElementId持ってるはず。
        // 未割当部材への構台名付与機能で構台名のみをセットされている場合があるがそれをスキップしたいという意図。
        if ( material == null || material.m_ElementId == null ) continue ;

        // 「構台基点ファミリ」の場合はレコード追加をスキップ
        if ( familyName == KoudaiReference.familyName ) continue ;

        var id = element.UniqueId ;
        var elemId = element.Id.IntegerValue ;
        var koudaiName = material?.m_KodaiName ;
        var daiBunrui = GantryUtil.GetTypeParameter( familySymbol, DefineUtil.PARAM_DAI_BUNRUI ) ;
        var chuuBunrui = GantryUtil.GetTypeParameter( familySymbol, DefineUtil.PARAM_CHUU_BUNRUI ) ;
        var shouBunrui = GantryUtil.GetTypeParameter( familySymbol, DefineUtil.PARAM_SHOU_BUNRUI ) ;

        var buzaiCategory = string.Empty ;
        if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
             chuuBunrui == CONST_CHUUBUNRUI_KETAZAI && shouBunrui == CONST_SHOUBUNRUI_NEDA ) {
          buzaiCategory = CONST_CATEGORY_NEDA ;
        }
        else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                  chuuBunrui == CONST_CHUUBUNRUI_KETAZAI && shouBunrui == CONST_SHOUBUNRUI_SHUGETA ) {
          buzaiCategory = CONST_CATEGORY_SHUGETA ;
        }
        else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                  chuuBunrui == CONST_CHUUBUNRUI_KETAZAI && shouBunrui == CONST_SHOUBUNRUI_OHBIKI ) {
          buzaiCategory = CONST_CATEGORY_OHBIKI ;
        }
        else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                  chuuBunrui == CONST_CHUUBUNRUI_KETAZAI && shouBunrui == CONST_SHOUBUNRUI_KETAUKE ) {
          buzaiCategory = CONST_CATEGORY_KETAUKE ;
        }
        else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                  chuuBunrui == CONST_CHUUBUNRUI_KETAZAI && shouBunrui == CONST_SHOUBUNRUI_SHIKIGETA ) {
          buzaiCategory = CONST_CATEGORY_SHIKIGETA ;
        }
        else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                  chuuBunrui == CONST_CHUUBUNRUI_KETAZAI && shouBunrui == CONST_SHOUBUNRUI_JIFUKU ) {
          buzaiCategory = CONST_CATEGORY_JIFUKU ;
        }
        else if ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                  chuuBunrui == CONST_CHUUBUNRUI_KETAZAI && shouBunrui == CONST_SHOUBUNRUI_FUKKOUBANZUREDOMEZAI ) {
          buzaiCategory = CONST_CATEGORY_FUKKOUBANZUREDOMEZAI ;
        }
        else if ( ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                    chuuBunrui == CONST_CHUUBUNRUI_SHUZAI && shouBunrui == CONST_SHOUBUNRUI_SHICHU_SOZAI ) ||
                  ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                    chuuBunrui == CONST_CHUUBUNRUI_SHUZAI && shouBunrui == CONST_SHOUBUNRUI_SHICHU_YAMADOME ) ||
                  ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                    chuuBunrui == CONST_CHUUBUNRUI_SHUZAI &&
                    shouBunrui == CONST_SHOUBUNRUI_SHICHU_KOUKYOUDOYAMADOME ) ) {
          buzaiCategory = CONST_CATEGORY_SHICHU ;
        }
        else if ( daiBunrui == CONST_DAIBUNRUI_YAMADOMEHEKI && chuuBunrui == CONST_CHUUBUNRUI_SHIJIKUI &&
                  shouBunrui == CONST_SHOUBUNRUI_KUI ) {
          buzaiCategory = CONST_CATEGORY_SHIJIKUI ;
        }

        if ( string.IsNullOrEmpty( buzaiCategory ) ) continue ;
        if ( rowDataList.Exists( x => x.KoudaiName == koudaiName && x.Category == buzaiCategory ) ) continue ;

        var rowData = new RowDataOfSizeCategoryList { KoudaiName = koudaiName, Category = buzaiCategory, } ;

        rowDataList.Add( rowData ) ;
      }

      return rowDataList ;
    }

    private class Bunrui
    {
      public string Dai { get ; set ; }
      public string Chuu { get ; set ; }
      public string Shou { get ; set ; }
    }

    private class RowDataOfSizeList
    {
      public string Id { get ; set ; }
      public int ElementId { get ; set ; }
      public string KoudaiName { get ; set ; }
      public string DaiBunrui { get ; set ; }
      public string ChuuBunrui { get ; set ; }
      public string ShouBunrui { get ; set ; }
      public string BeforeSize { get ; set ; }
      public string AfterSize { get ; set ; }
    }

    private class RowDataOfSizeCategoryList
    {
      public string KoudaiName { get ; set ; }
      public string Category { get ; set ; }
      public string AfterSize { get ; set ; }
    }

    private class ChoicesSize
    {
      public Bunrui BunruiOfChoicesSize { get ; set ; }
      public List<string> ValueOfChoicesSize { get ; set ; }
    }

    private class ChoicesSizeOfCategory
    {
      public string CategoryOfChoicesSize { get ; set ; }
      public List<string> ValueOfChoicesSize { get ; set ; }
    }

    private void DgvEditSizeList_SelectionChanged( object sender, EventArgs e )
    {
      // 選択レコードに該当する部材を選択(ハイライト)
      if ( _targetElementUniqueIdList.Count() != 0 ) {
        _selectedElemetIdList.Clear() ;

        var selectedRowList = new List<DataGridViewRow>() ;

        foreach ( DataGridViewRow row in this.DgvEditSizeList.SelectedRows ) {
          // 選択されたRowを取得
          selectedRowList.Add( row ) ;
        }

        // 選択されたRowよりID列の値を取得
        var selectedUniqueIdsList = selectedRowList.Select( x => x.Cells[ "ColSizeListID" ].Value.ToString() ) ;

        foreach ( var uniqueId in selectedUniqueIdsList ) {
          // ID列よりElementIDのリストを生成
          var element = _uiDoc.Document.GetElement( uniqueId ) ;
          if ( element == null ) continue ;

          _selectedElemetIdList.Add( element.Id ) ;
        }

        _uiDoc.Selection.SetElementIds( _selectedElemetIdList ) ;
      }
    }

    private void DgvEditSizeCategoryList_SelectionChanged( object sender, EventArgs e )
    {
      // 選択レコードに該当する部材を選択(ハイライト)
      if ( _targetElementUniqueIdList.Count() != 0 ) {
        _selectedElemetIdListOfCategory.Clear() ;

        var selectedRowList = new List<DataGridViewRow>() ;

        foreach ( DataGridViewRow row in this.DgvEditSizeCategoryList.SelectedRows ) {
          // 選択されたRowを取得
          selectedRowList.Add( row ) ;
        }

        // 選択されたRowより種別列の値を取得
        var selectedCategoryList = selectedRowList.Select( x => new CategoryRow()
        {
          KoudaiName = x.Cells[ "ColSizeCategoryListKoudai" ].Value.ToString(),
          Category = x.Cells[ "ColSizeCategoryListCategory" ].Value.ToString()
        } ) ;

        foreach ( var categoryRow in selectedCategoryList ) {
          foreach ( var elementUniqueId in _targetElementUniqueIdList ) {
            // エレメント取得
            var _element = _uiDoc.Document.GetElement( elementUniqueId ) ;
            if ( _element == null ) continue ;

            var elementId = _element.Id ;

            // ファミリ名取得
            var familyName = GantryUtil.GetFamilyName( _element ) ;
            // タイプ名取得
            var typeName = GantryUtil.GetTypeName( _element ) ;
            // ファミリインスタンス取得
            var familyInstance = GantryUtil.GetFamilyInstance( _uiDoc.Document, elementId ) ;
            // ファミリシンボル取得
            var familySymbol = ClsRevitUtil.GetFamilySymbol( _uiDoc.Document, familyName, typeName ) ;

            // ネストされたファミリである場合はレコードの追加をスキップ
            if ( familyInstance == null || familyInstance.SuperComponent != null ) continue ;

            var material = MaterialSuper.ReadFromElement( familyInstance.Id, _uiDoc.Document ) ;
            // Revit標準機能で配置された(部材情報のElementId有無で判断)ファミリの場合はスキップ
            // システムで配置した部材については部材情報にElementId持ってるはず。
            // 未割当部材への構台名付与機能で構台名のみをセットされている場合があるがそれをスキップしたいという意図。
            if ( material == null || material.m_ElementId == null ) continue ;

            // 「構台基点ファミリ」の場合はレコード追加をスキップ
            if ( familyName == KoudaiReference.familyName ) continue ;

            var koudaiName = material?.m_KodaiName ;
            var daiBunrui = GantryUtil.GetTypeParameter( familySymbol, DefineUtil.PARAM_DAI_BUNRUI ) ;
            var chuuBunrui = GantryUtil.GetTypeParameter( familySymbol, DefineUtil.PARAM_CHUU_BUNRUI ) ;
            var shouBunrui = GantryUtil.GetTypeParameter( familySymbol, DefineUtil.PARAM_SHOU_BUNRUI ) ;

            if ( categoryRow.KoudaiName != koudaiName ) continue ;

            if ( categoryRow.Category == CONST_CATEGORY_NEDA ) {
              if ( ! ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                       chuuBunrui == CONST_CHUUBUNRUI_KETAZAI && shouBunrui == CONST_SHOUBUNRUI_NEDA ) ) continue ;
            }
            else if ( categoryRow.Category == CONST_CATEGORY_SHUGETA ) {
              if ( ! ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                       chuuBunrui == CONST_CHUUBUNRUI_KETAZAI && shouBunrui == CONST_SHOUBUNRUI_SHUGETA ) ) continue ;
            }
            else if ( categoryRow.Category == CONST_CATEGORY_OHBIKI ) {
              if ( ! ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                       chuuBunrui == CONST_CHUUBUNRUI_KETAZAI && shouBunrui == CONST_SHOUBUNRUI_OHBIKI ) ) continue ;
            }
            else if ( categoryRow.Category == CONST_CATEGORY_KETAUKE ) {
              if ( ! ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                       chuuBunrui == CONST_CHUUBUNRUI_KETAZAI && shouBunrui == CONST_SHOUBUNRUI_KETAUKE ) ) continue ;
            }
            else if ( categoryRow.Category == CONST_CATEGORY_SHIKIGETA ) {
              if ( ! ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                       chuuBunrui == CONST_CHUUBUNRUI_KETAZAI && shouBunrui == CONST_SHOUBUNRUI_SHIKIGETA ) ) continue ;
            }
            else if ( categoryRow.Category == CONST_CATEGORY_JIFUKU ) {
              if ( ! ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                       chuuBunrui == CONST_CHUUBUNRUI_KETAZAI && shouBunrui == CONST_SHOUBUNRUI_JIFUKU ) ) continue ;
            }
            else if ( categoryRow.Category == CONST_CATEGORY_FUKKOUBANZUREDOMEZAI ) {
              if ( ! ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                       chuuBunrui == CONST_CHUUBUNRUI_SONOTA &&
                       shouBunrui == CONST_SHOUBUNRUI_FUKKOUBANZUREDOMEZAI ) ) continue ;
            }
            else if ( categoryRow.Category == CONST_CATEGORY_SHICHU ) {
              if ( ! ( ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                         chuuBunrui == CONST_CHUUBUNRUI_SHUZAI && shouBunrui == CONST_SHOUBUNRUI_SHICHU_SOZAI ) ||
                       ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                         chuuBunrui == CONST_CHUUBUNRUI_SHUZAI && shouBunrui == CONST_SHOUBUNRUI_SHICHU_YAMADOME ) ||
                       ( ( daiBunrui == CONST_DAIBUNRUI_KOUDAI_1 || daiBunrui == CONST_DAIBUNRUI_KOUDAI_2 ) &&
                         chuuBunrui == CONST_CHUUBUNRUI_SHUZAI &&
                         shouBunrui == CONST_SHOUBUNRUI_SHICHU_KOUKYOUDOYAMADOME ) ) ) continue ;
            }
            else if ( categoryRow.Category == CONST_CATEGORY_SHIJIKUI ) {
              if ( ! ( daiBunrui == CONST_DAIBUNRUI_YAMADOMEHEKI && chuuBunrui == CONST_CHUUBUNRUI_SHIJIKUI &&
                       shouBunrui == CONST_SHOUBUNRUI_KUI ) ) continue ;
            }
            else {
              continue ;
            }

            _selectedElemetIdListOfCategory.Add( _element.Id ) ;
          }
        }

        _uiDoc.Selection.SetElementIds( _selectedElemetIdListOfCategory ) ;
      }
    }

    private class CategoryRow
    {
      public string KoudaiName ;
      public string Category ;
    }

    private void FrmEditSizeList_FormClosed( object sender, FormClosedEventArgs e )
    {
      // Revit上でのファミリ選択状態を解除
      _uiDoc.Selection.SetElementIds( new List<ElementId>() ) ;

      //初期化処理
      MakeRequest( RequestId.End ) ;
      m_ExEvent.Dispose() ;
      m_ExEvent = null ;
      m_Handler = null ;
    }

    private void BtnSelectedAll_Click( object sender, EventArgs e )
    {
      for ( int i = 0 ; i < this.DgvEditSizeList.RowCount ; i++ ) {
        this.DgvEditSizeList[ "ColSizeListSelected", i ].Value = true ;
      }
    }

    private void BtnUnSelectedAll_Click( object sender, EventArgs e )
    {
      for ( int i = 0 ; i < this.DgvEditSizeList.RowCount ; i++ ) {
        this.DgvEditSizeList[ "ColSizeListSelected", i ].Value = false ;
      }
    }

    private void 選択ToolStripMenuItem_Click( object sender, EventArgs e )
    {
      foreach ( DataGridViewRow row in this.DgvEditSizeList.SelectedRows ) {
        row.Cells[ "ColSizeListSelected" ].Value = true ;
      }
    }

    private void 解除ToolStripMenuItem_Click( object sender, EventArgs e )
    {
      foreach ( DataGridViewRow row in this.DgvEditSizeList.SelectedRows ) {
        row.Cells[ "ColSizeListSelected" ].Value = false ;
      }
    }

    private void BtnSelectedAllOfCategory_Click( object sender, EventArgs e )
    {
      for ( int i = 0 ; i < this.DgvEditSizeCategoryList.RowCount ; i++ ) {
        this.DgvEditSizeCategoryList[ "ColSizeCategoryListSelected", i ].Value = true ;
      }
    }

    private void BtnUnSelectedAllOfCategory_Click( object sender, EventArgs e )
    {
      for ( int i = 0 ; i < this.DgvEditSizeCategoryList.RowCount ; i++ ) {
        this.DgvEditSizeCategoryList[ "ColSizeCategoryListSelected", i ].Value = false ;
      }
    }

    private void 選択ToolStripMenuItemOfCategory_Click( object sender, EventArgs e )
    {
      foreach ( DataGridViewRow row in this.DgvEditSizeCategoryList.SelectedRows ) {
        row.Cells[ "ColSizeCategoryListSelected" ].Value = true ;
      }
    }

    private void 解除ToolStripMenuItemOfCategory_Click( object sender, EventArgs e )
    {
      foreach ( DataGridViewRow row in this.DgvEditSizeCategoryList.SelectedRows ) {
        row.Cells[ "ColSizeCategoryListSelected" ].Value = false ;
      }
    }

    private void BtnOK_Click( object sender, EventArgs e )
    {
      var selectedTab = this.tabControl1.SelectedTab ;

      if ( selectedTab.Name == "TbpSizeList" ) {
        MakeRequest( RequestId.EditSize ) ;
        return ;
      }

      if ( selectedTab.Name == "TbpSizeCategoryList" ) {
        MakeRequest( RequestId.EditSizeCategory ) ;
        return ;
      }
    }

    private void BtnCancel_Click( object sender, EventArgs e )
    {
      this.Close() ;
    }

    private void DgvEditSizeList_CellValueChanged( object sender, DataGridViewCellEventArgs e )
    {
      if ( e.RowIndex < 0 ) return ;

      //編集中のセル
      var cellEditing = this.DgvEditSizeList.Rows[ e.RowIndex ].Cells[ e.ColumnIndex ] ;
      // 選択中セルの変更対象が「変更後サイズ」でない場合は処理スキップ
      if ( cellEditing.OwningColumn.Name != "ColSizeListAfterSize" ) return ;

      var cellEditingCheck =
        this.DgvEditSizeList.Rows[ e.RowIndex ].Cells[ "ColSizeListSelected" ] as DataGridViewCheckBoxCell ;
      // 選択中セルのチェック状態が有効でない場合は処理スキップ
      if ( ! (bool) cellEditingCheck.Value ) return ;

      //編集中の列が「変更後サイズ」の場合
      for ( var i = 0 ; i < this.DgvEditSizeList.RowCount ; i++ ) {
        var cellCheck = this.DgvEditSizeList.Rows[ i ].Cells[ "ColSizeListSelected" ] as DataGridViewCheckBoxCell ;

        // 編集中のセルを除くかつ、チェックが有効な場合
        if ( ( cellEditing.RowIndex != cellCheck.RowIndex ) && (bool) cellCheck.Value ) {
          var cellEditSize =
            this.DgvEditSizeList.Rows[ i ].Cells[ "ColSizeListAfterSize" ] as DataGridViewComboBoxCell ;
          var cellEditingSize = cellEditing as DataGridViewComboBoxCell ;

          // 選択肢の中に同一の値が存在する場合
          if ( cellEditSize.Items.Contains( cellEditingSize.Value.ToString() ) ) {
            // 編集中のセルの「変更サイズ」の値をチェックが有効な同列の値に設定
            cellEditSize.Value = cellEditingSize.Value ;
          }
        }
      }
    }

    private void DgvEditSizeList_CurrentCellDirtyStateChanged( object sender, EventArgs e )
    {
      var editSizeChangeList = sender as DataGridView ;

      //コミットされていない内容がある
      if ( editSizeChangeList.IsCurrentCellDirty ) {
        editSizeChangeList.CommitEdit( DataGridViewDataErrorContexts.Commit ) ;
      }
    }

    private void DgvEditSizeCategoryList_CellValueChanged( object sender, DataGridViewCellEventArgs e )
    {
      if ( e.RowIndex < 0 ) return ;

      //編集中のセル
      var cellEditing = this.DgvEditSizeCategoryList.Rows[ e.RowIndex ].Cells[ e.ColumnIndex ] ;
      // 選択中セルの変更対象が「変更後サイズ」でない場合は処理スキップ
      if ( cellEditing.OwningColumn.Name != "ColSizeCategoryListAfterSize" ) return ;

      var cellEditingCheck =
        this.DgvEditSizeCategoryList.Rows[ e.RowIndex ].Cells[ "ColSizeCategoryListSelected" ] as
          DataGridViewCheckBoxCell ;
      // 選択中セルのチェック状態が有効でない場合は処理スキップ
      if ( ! (bool) cellEditingCheck.Value ) return ;

      //編集中の列が「変更後サイズ」の場合
      for ( var i = 0 ; i < this.DgvEditSizeCategoryList.RowCount ; i++ ) {
        var cellCheck =
          this.DgvEditSizeCategoryList.Rows[ i ].Cells[ "ColSizeCategoryListSelected" ] as DataGridViewCheckBoxCell ;

        // 編集中のセルを除くかつ、チェックが有効な場合
        if ( ( cellEditing.RowIndex != cellCheck.RowIndex ) && (bool) cellCheck.Value ) {
          var cellEditSize =
            this.DgvEditSizeCategoryList.Rows[ i ].Cells[ "ColSizeCategoryListAfterSize" ] as DataGridViewComboBoxCell ;
          var cellEditingSize = cellEditing as DataGridViewComboBoxCell ;

          // 選択肢の中に同一の値が存在する場合
          if ( cellEditSize.Items.Contains( cellEditingSize.Value.ToString() ) ) {
            // 編集中のセルの「変更後サイズ」の値をチェックが有効な同列の値に設定
            cellEditSize.Value = cellEditingSize.Value ;
          }
        }
      }
    }

    private void DgvEditSizeCategoryList_CurrentCellDirtyStateChanged( object sender, EventArgs e )
    {
      var editSizeCategoryChangeList = sender as DataGridView ;

      //コミットされていない内容がある
      if ( editSizeCategoryChangeList.IsCurrentCellDirty ) {
        editSizeCategoryChangeList.CommitEdit( DataGridViewDataErrorContexts.Commit ) ;
      }
    }

    /// <summary>
    /// フィルタ検索
    /// </summary>
    public void SearchSizeList()
    {
      try {
        // 個別一覧データグリッドに値をセット
        var rowDataList = CreateRowDataOfSizeList( _targetElementUniqueIdList ) ;

        var selectedKoudaiNameList = new List<string>() ;
        foreach ( string koudaiName in this.LstKoudaiEditSize.SelectedItems ) {
          selectedKoudaiNameList.Add( koudaiName ) ;
        }

        var filterList = rowDataList.AsEnumerable() ;
        if ( selectedKoudaiNameList.Count > 0 ) {
          filterList = rowDataList.Where( x =>
            selectedKoudaiNameList.Exists( t =>
              ! string.IsNullOrEmpty( x.KoudaiName ) && t.Contains( x.KoudaiName ) ) ) ;
        }

        var selectedDaiBunrui = this.CmbDaiBunrui.SelectedItem?.ToString() ;
        if ( ! string.IsNullOrEmpty( selectedDaiBunrui ) ) {
          filterList = filterList.Where( x => x.DaiBunrui == selectedDaiBunrui ) ;
        }

        var selectedChuuBunrui = this.CmbChuuBunrui.SelectedItem?.ToString() ;
        if ( ! string.IsNullOrEmpty( selectedChuuBunrui ) ) {
          filterList = filterList.Where( x => x.ChuuBunrui == selectedChuuBunrui ) ;
        }

        var selectedShouBunrui = this.CmbShouBunrui.SelectedItem?.ToString() ;
        if ( ! string.IsNullOrEmpty( selectedShouBunrui ) ) {
          filterList = filterList.Where( x => x.ShouBunrui == selectedShouBunrui ) ;
        }

        // 個別一覧データグリッドを初期化
        this.DgvEditSizeList.Rows.Clear() ;

        foreach ( var rowData in filterList ) {
          var rowIndex = this.DgvEditSizeList.Rows.Add( new object[]
          {
            rowData.Id, rowData.ElementId, false, rowData.KoudaiName, rowData.DaiBunrui, rowData.ChuuBunrui,
            rowData.ShouBunrui, rowData.BeforeSize,
          } ) ;

          // 各行の変更後サイズ選択肢を作成
          CreateComboBoxSizeList( rowIndex ) ;
        }

        this.DgvEditSizeList.ClearSelection() ;
      }
      catch ( Exception ex ) {
        MessageUtil.Error( "個別一覧のフィルター処理に失敗しました。", FORM_NAME, this ) ;
        logger.Error( ex, ex.Message ) ;
      }
    }

    private void LstKoudaiEditSize_SelectedIndexChanged( object sender, EventArgs e )
    {
      SearchSizeList() ;
    }

    private void CmbDaiBunrui_SelectedIndexChanged( object sender, EventArgs e )
    {
      SearchSizeList() ;
    }

    private void CmbChuuBunrui_SelectedIndexChanged( object sender, EventArgs e )
    {
      SearchSizeList() ;
    }

    private void CmbShouBunrui_SelectedIndexChanged( object sender, EventArgs e )
    {
      SearchSizeList() ;
    }


    /// <summary>
    /// フィルタ検索
    /// </summary>
    public void SearchSizeCategoryList()
    {
      try {
        // 種別一覧データグリッドに値をセット
        var rowDataList = CreateRowDataOfSizeCategoryList( _targetElementUniqueIdList ) ;

        var selectedKoudaiNameList = new List<string>() ;
        foreach ( string koudaiName in this.LstKoudaiEditSizeCategory.SelectedItems ) {
          selectedKoudaiNameList.Add( koudaiName ) ;
        }

        var filterList = rowDataList.AsEnumerable() ;
        if ( selectedKoudaiNameList.Count > 0 ) {
          filterList = rowDataList.Where( x =>
            selectedKoudaiNameList.Exists( t =>
              ! string.IsNullOrEmpty( x.KoudaiName ) && t.Contains( x.KoudaiName ) ) ) ;
        }

        var selectedCategory = this.CmbCategory.SelectedItem?.ToString() ;
        if ( ! string.IsNullOrEmpty( selectedCategory ) ) {
          filterList = filterList.Where( x => x.Category == selectedCategory ) ;
        }

        // 種別一覧データグリッドを初期化
        this.DgvEditSizeCategoryList.Rows.Clear() ;

        foreach ( var rowData in filterList ) {
          var rowIndex =
            this.DgvEditSizeCategoryList.Rows.Add( new object[] { false, rowData.KoudaiName, rowData.Category, } ) ;

          // 各行の変更後サイズ選択肢を作成
          CreateComboBoxSizeListOfCategory( rowIndex ) ;
        }

        this.DgvEditSizeCategoryList.ClearSelection() ;
      }
      catch ( Exception ex ) {
        MessageUtil.Error( "種別一覧のフィルター処理に失敗しました。", FORM_NAME, this ) ;
        logger.Error( ex, ex.Message ) ;
      }
    }

    private void LstKoudaiEditSizeCategory_SelectedIndexChanged( object sender, EventArgs e )
    {
      SearchSizeCategoryList() ;
    }

    private void CmbCategory_SelectedIndexChanged( object sender, EventArgs e )
    {
      SearchSizeCategoryList() ;
    }

    #region "モードレス対応"

    /// <summary>
    ///   WakeUp -> enable all controls
    /// </summary>
    /// 
    public void WakeUp()
    {
      EnableCommands( true ) ;
    }

    /// <summary>
    ///   Control enabler / disabler 
    /// </summary>
    ///
    private void EnableCommands( bool status )
    {
      foreach ( System.Windows.Forms.Control ctrl in this.Controls ) {
        ctrl.Enabled = status ;
      }
    }

    private void MakeRequest( RequestId request )
    {
      m_Handler.Request.Make( request ) ;
      m_ExEvent.Raise() ;
      DozeOff() ;
    }

    /// <summary>
    ///   DozeOff -> disable all controls (but the Exit button)
    /// </summary>
    /// 
    private void DozeOff()
    {
      EnableCommands( false ) ;
    }

    #endregion
  }
}