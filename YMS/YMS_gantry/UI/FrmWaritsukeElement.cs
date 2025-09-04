using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.ComponentModel ;
using System.Data ;
using System.Drawing ;
using System.IO ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;
using YMS.gantry ;
using YMS_gantry.GantryUtils ;

namespace YMS_gantry.UI
{
  public partial class FrmWaritsukeElement : System.Windows.Forms.Form
  {
    #region 定数

    /// <summary>
    /// Iniファイルのパス
    /// </summary>
    const string IniPath = "ini\\FrmWaritsukeElement.ini" ;

    /// <summary>
    /// Iniファイルのセクション名
    /// </summary>
    const string sec = "FrmWaritsukeElement" ;

    #endregion

    public enum WaritsukeCommand : int
    {
      None,
      CreateSichu, //支柱
      CreateJifuku, //地覆
      CreateOhbiki, //大引
      CreateKetauke, //桁受
      CreateTesuri, //手摺
      CreateShikiketa, //敷桁
      CreateHoudue, //方杖
      CreateNeda, //根太
      CreateShuketa, //主桁
      CreateJoint, //継材
      CreateHukkouketa, //覆工桁
      Switch, // 通常モード
      SwitchSMH, // 高強度モード
      CommandUndo, // 戻る
      CommandEnd, // 終了
    }

    public enum WaritsukeMode
    {
      Normal,
      HA,
      SMH
    }

    public enum WaritsukeType
    {
      Normal,
      Plate,
      Jack,
      Pieace
    }

    private UIDocument _uiDoc ;
    private Element _element ;

    private RequestHandler m_Handler ;
    private ExternalEvent m_ExEvent ;
    private UIApplication _uiApp { get ; set ; }
    private Document doc { get ; set ; }

    string waritukeTarget { get ; set ; }
    string waritsukeSize { get ; set ; }
    public WaritsukeType waritsukeType { get ; set ; }
    public WaritsukeCommand command { get ; set ; }
    public WaritsukeMode mode { get ; set ; }

    public string familyType { get ; set ; }

    /// <summary>
    /// 選択されたサイズ
    /// </summary>
    public string SelectedSize { get ; set ; }

    public string typeName { get ; set ; }

    /// <summary>
    /// プレートのサイズ
    /// </summary>
    public (double H, double D, double W) PlateSize { get ; set ; }

    /// <summary>
    /// ジャッキの長さ
    /// </summary>
    public double JackLength { get ; set ; }

    public FrmWaritsukeElement( UIDocument uidoc, Element element )
    {
      InitializeComponent() ;

      _uiDoc = uidoc ;
      _element = element ;
    }

    public FrmWaritsukeElement( ExternalEvent exEvent, RequestHandler handler, UIApplication uiApp )
    {
      InitializeComponent() ;
      _uiApp = uiApp ;
      doc = uiApp.ActiveUIDocument.Document ;
      _uiDoc = _uiApp.ActiveUIDocument ;
      m_Handler = handler ;
      m_ExEvent = exEvent ;
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
      //if (!status)
      //{
      //    this.button3.Enabled = true;
      //}
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

    public void Targetwaritsuke( string type, string size )
    {
      waritukeTarget = type ;
      if ( ( size.Contains( "HA" ) || size.Contains( "SMH" ) ) && size.Contains( "_" ) ) {
        size = size.Substring( 0, size.IndexOf( "_" ) ) ;
      }

      waritsukeSize = size ;
      UpDateMainSizeList() ;
      UpdateJackSize() ;
      UpdatePieaceSize() ;
      List<string> list = new List<string>() ;
      if ( ! string.IsNullOrEmpty( waritsukeSize ) ) {
        list = Master.ClsWaritsukeCsv.GetSizeList( "カバープレート", waritsukeSize ) ;
      }

      ControlUtils.SetComboBoxItems( this.CmbCoverPL, list ) ;
    }

    public void UpdateLength( double originalL, double remainL )
    {
      this.LblOriginalL.Text = ( (int) originalL ).ToString() ;
      this.LblRemainL.Text = ( (int) remainL ).ToString() ;
    }

    private void FrmWaritsuke_Load( object sender, EventArgs e )
    {
      InitComboBox() ;
      GetIniData() ;
      UpdateJackLimited() ;
    }

    private void InitComboBox()
    {
      //構台名をコンボボックスに追加
      ControlUtils.SetComboBoxItems( this.CmbKoudaiName, GantryUtil.GetAllKoudaiName( doc ).ToList() ) ;
      //メイン
      if ( ! string.IsNullOrEmpty( waritsukeSize ) ) {
        UpDateMainSizeList() ;
      }

      if ( ! string.IsNullOrEmpty( waritsukeSize ) ) {
        //ジャッキ
        UpdateJackSize() ;
      }

      if ( ! string.IsNullOrEmpty( waritsukeSize ) ) {
        //補助ピース
        UpdatePieaceSize() ;
      }

      //カバープレート
      List<string> list = new List<string>() ;
      if ( ! string.IsNullOrEmpty( waritsukeSize ) ) {
        list = Master.ClsWaritsukeCsv.GetSizeList( "カバープレート", waritsukeSize ) ;
      }

      ControlUtils.SetComboBoxItems( this.CmbCoverPL, list ) ;

      if ( this.NmcWaritsukeLeng.Value < 0 ) {
        this.NmcWaritsukeLeng.Value = 0 ;
      }

      if ( CmbUnit.Items.Count > 0 ) {
        CmbUnit.SelectedIndex = 0 ;
      }

      if ( CmbCoverPL.Items.Count > 0 ) {
        CmbCoverPL.SelectedIndex = 0 ;
      }
    }

    private void UpDateMainSizeList()
    {
      string type = this.RbtMainMaterial.Checked ? "素材" : this.RbtMainNormal.Checked ? "山留主材" : "高強度山留主材" ;

      List<string> list = Master.ClsWaritsukeCsv.GetSizeList( type, waritsukeSize ) ;
      ControlUtils.SetListBoxItems( this.LstMainSize, list ) ;
    }

    private void FrmWaritsuke_FormClosed( object sender, FormClosedEventArgs e )
    {
    }

    private void button4_Click( object sender, EventArgs e )
    {
      //初期化処理
      MakeRequest( RequestId.End ) ;
      m_ExEvent.Dispose() ;
      m_ExEvent = null ;
      m_Handler = null ;

      this.DialogResult = DialogResult.Cancel ;
      this.Close() ;
    }

    private void RbtMainMaterial_CheckedChanged( object sender, EventArgs e )
    {
      UpDateMainSizeList() ;
      if ( this.RbtMainMaterial.Checked ) {
        mode = WaritsukeMode.Normal ;
        this.ChkNeedCoverPL.Checked = false ;
        this.ChkNeedCoverPL.Enabled = false ;
        this.CmbCoverPL.Enabled = false ;
        this.NmcWaritsukeLeng.Enabled = true ;
      }
      else if ( this.RbtMainNormal.Checked ) {
        mode = WaritsukeMode.HA ;
        this.ChkNeedCoverPL.Enabled = true ;
        this.CmbCoverPL.Enabled = true ;
        this.NmcWaritsukeLeng.Value = 0 ;
        this.NmcWaritsukeLeng.Enabled = false ;
      }
      else if ( this.RbtMainHigh.Checked ) {
        mode = WaritsukeMode.SMH ;
        this.ChkNeedCoverPL.Enabled = true ;
        this.CmbCoverPL.Enabled = true ;
        this.NmcWaritsukeLeng.Value = 0 ;
        this.NmcWaritsukeLeng.Enabled = false ;
      }
    }

    /// <summary>
    /// 桁材配置
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnCreateMain_Click( object sender, EventArgs e )
    {
      if ( ! CheckInpurValues( WaritsukeType.Normal ) ) {
        return ;
      }

      waritsukeType = WaritsukeType.Normal ;
      SelectedSize = this.LstMainSize.SelectedItem.ToString() ;
      this.command = getCommand() ;
      GetFamilyType() ;
      SetIniData() ;
      this.DialogResult = DialogResult.OK ;
      this.Visible = false ;
    }

    /// <summary>
    /// 補助ピース配置
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnCreatePieace_Click( object sender, EventArgs e )
    {
      if ( ! CheckInpurValues( WaritsukeType.Pieace ) ) {
        return ;
      }

      waritsukeType = WaritsukeType.Pieace ;
      SelectedSize = this.LstPieace.SelectedItem.ToString() ;
      this.command = getCommand() ;
      GetFamilyType() ;
      SetIniData() ;
      this.DialogResult = DialogResult.OK ;
      this.Visible = false ;
    }

    /// <summary>
    /// プレート配置
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnCreatePL_Click( object sender, EventArgs e )
    {
      if ( ! CheckInpurValues( WaritsukeType.Plate ) ) {
        return ;
      }

      waritsukeType = WaritsukeType.Plate ;
      PlateSize = ( (double) this.NmcPLH.Value, (double) this.NmcPLD.Value, (double) this.NmcPLW.Value ) ;
      this.command = getCommand() ;
      SelectedSize = "高さ調整プレート" ;
      GetFamilyType() ;
      SetIniData() ;
      this.DialogResult = DialogResult.OK ;
      this.Visible = false ;
    }

    /// <summary>
    /// ジャッキ配置
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnCreateJack_Click( object sender, EventArgs e )
    {
      if ( ! CheckInpurValues( WaritsukeType.Jack ) ) {
        return ;
      }

      waritsukeType = WaritsukeType.Jack ;
      SelectedSize = this.LstJackSize.SelectedItem.ToString() ;
      JackLength = (double) this.NmcJackLng.Value ;
      this.command = getCommand() ;
      GetFamilyType() ;
      SetIniData() ;
      this.DialogResult = DialogResult.OK ;
      this.Visible = false ;
    }

    /// <summary>
    /// 割付する対象
    /// </summary>
    /// <returns></returns>
    private WaritsukeCommand getCommand()
    {
      switch ( this.waritukeTarget ) {
        case "大引" :
          return WaritsukeCommand.CreateOhbiki ;
        case "桁受" :
          return WaritsukeCommand.CreateKetauke ;
        case "根太" :
          return WaritsukeCommand.CreateNeda ;
        case "主桁" :
          return WaritsukeCommand.CreateShuketa ;
        case "敷桁" :
          return WaritsukeCommand.CreateShikiketa ;
        case "方杖" :
          return WaritsukeCommand.CreateHoudue ;
        case "地覆" :
          return WaritsukeCommand.CreateJifuku ;
        case "手摺" :
          return WaritsukeCommand.CreateTesuri ;
        case "支柱" :
          return WaritsukeCommand.CreateSichu ;
        case "覆工桁" :
          return WaritsukeCommand.CreateHukkouketa ;
        case "ﾂﾅｷﾞ" :
          return WaritsukeCommand.CreateJoint ;
        default :
          return WaritsukeCommand.None ;
      }
    }

    private bool CheckInpurValues( WaritsukeType waritsukeType )
    {
      List<string> errMsg = new List<string>() ;

      if ( string.IsNullOrEmpty( CmbKoudaiName.Text ) ) {
        errMsg.Add( "構台名を指定してください" ) ;
      }

      if ( waritsukeType == WaritsukeType.Normal ) {
        if ( LstMainSize.SelectedIndex < 0 ) {
          errMsg.Add( "メイン部材を選択して下さい" ) ;
        }

        if ( RbtMainMaterial.Checked && NmcWaritsukeLeng.Value <= 0 ) {
          errMsg.Add( "割付長さを指定して下さい" ) ;
        }

        if ( CmbCoverPL.Enabled && ChkNeedCoverPL.Checked && string.IsNullOrEmpty( CmbCoverPL.Text ) ) {
          errMsg.Add( "カバープレートを指定して下さい" ) ;
        }
      }
      else if ( waritsukeType == WaritsukeType.Plate ) {
        if ( NmcPLH.Value <= 0 ) {
          errMsg.Add( "プレートHを指定して下さい" ) ;
        }

        if ( NmcPLW.Value <= 0 ) {
          errMsg.Add( "プレートWを指定して下さい" ) ;
        }

        if ( NmcPLD.Value <= 0 ) {
          errMsg.Add( "プレートDを指定して下さい" ) ;
        }
      }
      else if ( waritsukeType == WaritsukeType.Jack ) {
        if ( LstJackSize.SelectedIndex < 0 ) {
          errMsg.Add( "ジャッキを選択して下さい" ) ;
        }
      }
      else if ( waritsukeType == WaritsukeType.Pieace ) {
        if ( LstPieace.SelectedIndex < 0 ) {
          errMsg.Add( "補助ピースを選択して下さい" ) ;
        }
      }

      if ( errMsg.Count > 0 ) {
        FrmErrorInformation frm = new FrmErrorInformation( errMsg, this ) ;
        return false ;
      }
      else {
        return true ;
      }
    }

    private void RbtHojo_CheckedChanged( object sender, EventArgs e )
    {
      UpdatePieaceSize() ;
    }

    /// <summary>
    /// 補助ピースリスト更新
    /// </summary>
    private void UpdatePieaceSize()
    {
      List<string> list = new List<string>() ;
      if ( this.RbtHojo.Checked ) {
        list = Master.ClsWaritsukeCsv.GetSizeList( "補助ピース", waritsukeSize ) ;
      }
      else {
        list = Master.ClsWaritsukeCsv.GetSizeList( "高強度補助ピース", waritsukeSize ) ;
      }

      ControlUtils.SetListBoxItems( this.LstPieace, list ) ;
    }

    private void UpdateJackSize()
    {
      List<string> list = new List<string>() ;
      if ( waritukeTarget == PilePiller.sichuTypeName ) {
        if ( this.RbtJackYuatsu.Checked ) {
          list = Master.ClsWaritsukeCsv.GetSizeList( "油圧ジャッキ", waritsukeSize ) ;
        }
        else {
          list = Master.ClsWaritsukeCsv.GetSizeList( "キリンジャッキ", waritsukeSize ) ;
        }
      }

      ControlUtils.SetListBoxItems( this.LstJackSize, list ) ;
    }

    /// <summary>
    /// Numeric抜け防止
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NmcPLD_Leave( object sender, EventArgs e )
    {
      NumericUpDown nmc = (NumericUpDown) sender ;
      if ( nmc.Text == "" ) {
        nmc.Value = 0 ;
        nmc.Text = "0" ;
      }

      base.OnLostFocus( e ) ;
    }

    private void GetFamilyType()
    {
      if ( this.waritsukeType.Equals( WaritsukeType.Normal ) ) {
        familyType = mode == FrmWaritsukeElement.WaritsukeMode.Normal ? "素材" :
          mode == FrmWaritsukeElement.WaritsukeMode.HA ? "山留主材" : "高強度山留主材" ;
        typeName = waritukeTarget ;
      }
      else if ( this.waritsukeType.Equals( WaritsukeType.Pieace ) ) {
        familyType = RbtHighHojo.Checked ? "高強度補助ピース" : "補助ピース" ;
        typeName = RbtHighHojo.Checked ? "高強度補助ﾋﾟｰｽ" : "補助ﾋﾟｰｽ" ;
      }
      else if ( this.waritsukeType.Equals( WaritsukeType.Plate ) ) {
        familyType = "プレート" ;
        typeName = "高さ調整ﾌﾟﾚｰﾄ" ;
      }
      else if ( this.waritsukeType.Equals( WaritsukeType.Jack ) ) {
        familyType = RbtJackYuatsu.Checked ? "油圧ジャッキ" : "キリンジャッキ" ;
        if ( waritukeTarget == PilePiller.sichuTypeName ) {
          int ind = SelectedSize.LastIndexOf( "_" ) ;
          string tensho = SelectedSize.Substring( ind + 1, SelectedSize.Length - ind - 1 ) ;
          typeName = "支柱" + ( ( ind > -1 ) ? "_" + tensho : "" ) ;
        }
        else {
          int ind = SelectedSize.LastIndexOf( "_" ) ;
          string tensho = SelectedSize.Substring( ind + 1, SelectedSize.Length - ind - 1 ) ;
          typeName = "支柱" + ( ( ind > -1 ) ? "_" + tensho : "" ) ;
        }
      }
    }

    private void btnUndo_Click( object sender, EventArgs e )
    {
      this.command = WaritsukeCommand.CommandUndo ;
      this.DialogResult = DialogResult.OK ;
      this.Visible = false ;
    }

    private void RbtJackKirin_CheckedChanged( object sender, EventArgs e )
    {
      UpdateJackSize() ;
    }

    private void LstJackSize_SelectedIndexChanged( object sender, EventArgs e )
    {
      UpdateJackLimited() ;
    }

    private void UpdateJackLimited()
    {
      if ( LstJackSize.SelectedIndex < 0 ) {
        this.NmcJackLng.Minimum = 0 ;
        this.NmcJackLng.Value = 0 ;
        this.NmcJackLng.Maximum = 500 ;
      }
      else {
        string type = RbtJackYuatsu.Checked ? "油圧ジャッキ" : "キリンジャッキ" ;
        string jackLimits = Master.ClsWaritsukeCsv.GetLength( LstJackSize.SelectedItem.ToString(), type ) ;
        if ( string.IsNullOrEmpty( jackLimits ) || jackLimits.Split( '_' ).Length != 2 ) {
          this.NmcJackLng.Minimum = 0 ;
          this.NmcJackLng.Value = 0 ;
          this.NmcJackLng.Maximum = 500 ;
        }
        else {
          string[] lengths = jackLimits.Split( '_' ) ;
          double min = ClsCommonUtils.ChangeStrToDbl( lengths[ 0 ] ) ;
          double max = ClsCommonUtils.ChangeStrToDbl( lengths[ 1 ] ) ;
          this.NmcJackLng.Minimum = (decimal) min ;
          this.NmcJackLng.Value = (decimal) min ;
          this.NmcJackLng.Maximum = (decimal) max ;
        }
      }
    }

    /// <summary>
    /// ダイアログデータをini二セット
    /// </summary>
    public void SetIniData()
    {
      string iniPath = ClsIni.GetIniFilePath( IniPath ) ;
      ClsIni.WritePrivateProfileString( sec, tabControl1.Name, tabControl1.SelectedIndex.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtMainMaterial.Name, RbtMainMaterial.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtMainNormal.Name, RbtMainNormal.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtMainHigh.Name, RbtMainHigh.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, NmcWaritsukeLeng.Name, NmcWaritsukeLeng.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, NmcPLH.Name, NmcPLH.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, NmcPLW.Name, NmcPLW.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, NmcPLD.Name, NmcPLD.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtJackKirin.Name, RbtJackKirin.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtJackYuatsu.Name, RbtJackYuatsu.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, NmcJackLng.Name, NmcJackLng.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtHojo.Name, RbtHojo.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtHighHojo.Name, RbtHighHojo.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, ChkNeedCoverPL.Name, ChkNeedCoverPL.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, CmbCoverPL.Name, CmbCoverPL.Text, iniPath ) ;
    }

    /// <summary>
    /// iniデータをダイアログにセット
    /// </summary>
    public void GetIniData()
    {
      string iniPath = ClsIni.GetIniFilePath( IniPath ) ;
      if ( File.Exists( iniPath ) ) {
        tabControl1.SelectedIndex = int.Parse( ClsIni.GetIniFile( sec, tabControl1.Name, iniPath ) ) ;
        RbtMainMaterial.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtMainMaterial.Name, iniPath ) ) ;
        RbtMainNormal.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtMainNormal.Name, iniPath ) ) ;
        RbtMainHigh.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtMainHigh.Name, iniPath ) ) ;
        NmcWaritsukeLeng.Text = ClsIni.GetIniFile( sec, NmcWaritsukeLeng.Name, iniPath ) ;
        NmcPLH.Text = ClsIni.GetIniFile( sec, NmcPLH.Name, iniPath ) ;
        NmcPLW.Text = ClsIni.GetIniFile( sec, NmcPLW.Name, iniPath ) ;
        NmcPLD.Text = ClsIni.GetIniFile( sec, NmcPLD.Name, iniPath ) ;
        RbtJackKirin.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtJackKirin.Name, iniPath ) ) ;
        RbtJackYuatsu.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtJackYuatsu.Name, iniPath ) ) ;
        NmcJackLng.Text = ClsIni.GetIniFile( sec, NmcJackLng.Name, iniPath ) ;
        RbtHojo.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtHojo.Name, iniPath ) ) ;
        RbtHighHojo.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtHighHojo.Name, iniPath ) ) ;
        ChkNeedCoverPL.Checked = bool.Parse( ClsIni.GetIniFile( sec, ChkNeedCoverPL.Name, iniPath ) ) ;
        CmbCoverPL.Text = ClsIni.GetIniFile( sec, CmbCoverPL.Name, iniPath ) ;
      }
    }
  }
}