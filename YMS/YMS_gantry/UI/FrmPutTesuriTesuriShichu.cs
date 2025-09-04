using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
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
  public partial class FrmPutTesuriTesuriShichu : System.Windows.Forms.Form
  {
    #region 定数

    /// <summary>
    /// Iniファイルのパス
    /// </summary>
    const string IniPath = "ini\\FrmPutTesuriTesuriShichu.ini" ;

    /// <summary>
    /// Iniファイルのセクション名
    /// </summary>
    const string sec = "FrmPutTesuriTesuriShichu" ;

    #endregion

    private RequestHandler m_Handler ;
    private ExternalEvent m_ExEvent ;

    private UIApplication _uiApp { get ; set ; }
    private Document doc { get ; set ; }

    public FrmPutTesuriTesuriShichu( UIApplication uiApp )
    {
      InitializeComponent() ;
      _uiApp = uiApp ;
      doc = uiApp.ActiveUIDocument.Document ;
    }

    public FrmPutTesuriTesuriShichu( ExternalEvent exEvent, RequestHandler handler, UIApplication uiApp )
    {
      InitializeComponent() ;
      _uiApp = uiApp ;
      doc = uiApp.ActiveUIDocument.Document ;

      m_Handler = handler ;
      m_ExEvent = exEvent ;
    }

    public FrmPutTesuriTesuriShichu()
    {
      InitializeComponent() ;
    }

    private void FrmPutTesuriTesuriShichu_Load( object sender, EventArgs e )
    {
      InitComboBox() ;
      GetIniData() ;
      changeEnabler() ;
    }

    private void InitComboBox()
    {
      //構台名をコンボボックスに追加
      ControlUtils.SetComboBoxItems( CmbKoudaiName, GantryUtil.GetAllKoudaiName( doc ).ToList() ) ;

      //レベル追加
      ControlUtils.SetComboBoxItems( CmbLevel, GantryUtil.GetAllLevelName( doc ).ToList() ) ;

      List<string> lstStr = new List<string>() ;

      //手摺支柱
      lstStr = Master.ClsTesuriCsv.GetSizeList( Master.ClsTesuriCsv.TypeTesuriShichu ) ;
      ControlUtils.SetComboBoxItems( CmbTesurishichuSize, lstStr ) ;

      //手摺
      lstStr = Master.ClsTesuriCsv.GetSizeList( Master.ClsTesuriCsv.TypeTesuri ) ;
      ControlUtils.SetComboBoxItems( CmbTesuriSize, lstStr ) ;

      //材質
      lstStr = DefineUtil.ListMaterial ;
      ControlUtils.SetComboBoxItems( CmbTesuriMaterial, lstStr ) ;
      ControlUtils.SetComboBoxItems( CmbTesurishichuMaterial, lstStr ) ;
    }

    /// <summary>
    /// ダイアログデータをini二セット
    /// </summary>
    public void SetIniData()
    {
      string iniPath = ClsIni.GetIniFilePath( IniPath ) ;
      ClsIni.WritePrivateProfileString( sec, CmbTesurishichuSize.Name, CmbTesurishichuSize.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, CmbTesurishichuMaterial.Name, CmbTesurishichuMaterial.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, NmcBasePitch.Name, NmcBasePitch.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, NmcHight.Name, NmcHight.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, ChkTsukidashiKakuho.Name, ChkTsukidashiKakuho.Checked.ToString(),
        iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, CmbTesuriSize.Name, CmbTesuriSize.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, CmbTesuriMaterial.Name, CmbTesuriMaterial.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, NmcTesuriLng.Name, NmcTesuriLng.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, NmcDanPitch.Name, NmcDanPitch.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, NmcDanCount.Name, NmcDanCount.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtFace.Name, RbtFace.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtFree.Name, RbtFree.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, CmbLevel.Name, CmbLevel.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, NmcOffset.Name, NmcOffset.Text, iniPath ) ;
    }

    /// <summary>
    /// iniデータをダイアログにセット
    /// </summary>
    public void GetIniData()
    {
      string iniPath = ClsIni.GetIniFilePath( IniPath ) ;
      if ( File.Exists( iniPath ) ) {
        CmbTesurishichuSize.Text = ClsIni.GetIniFile( sec, CmbTesurishichuSize.Name, iniPath ) ;
        CmbTesurishichuMaterial.Text = ClsIni.GetIniFile( sec, CmbTesurishichuMaterial.Name, iniPath ) ;
        NmcBasePitch.Text = ClsIni.GetIniFile( sec, NmcBasePitch.Name, iniPath ) ;
        NmcHight.Text = ClsIni.GetIniFile( sec, NmcHight.Name, iniPath ) ;
        ChkTsukidashiKakuho.Checked = bool.Parse( ClsIni.GetIniFile( sec, ChkTsukidashiKakuho.Name, iniPath ) ) ;
        CmbTesuriSize.Text = ClsIni.GetIniFile( sec, CmbTesuriSize.Name, iniPath ) ;
        CmbTesuriMaterial.Text = ClsIni.GetIniFile( sec, CmbTesuriMaterial.Name, iniPath ) ;
        NmcTesuriLng.Text = ClsIni.GetIniFile( sec, NmcTesuriLng.Name, iniPath ) ;
        NmcDanPitch.Text = ClsIni.GetIniFile( sec, NmcDanPitch.Name, iniPath ) ;
        NmcDanCount.Text = ClsIni.GetIniFile( sec, NmcDanCount.Name, iniPath ) ;
        RbtFace.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtFace.Name, iniPath ) ) ;
        RbtFree.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtFree.Name, iniPath ) ) ;
        CmbLevel.Text = ClsIni.GetIniFile( sec, CmbLevel.Name, iniPath ) ;
        NmcOffset.Text = ClsIni.GetIniFile( sec, NmcOffset.Name, iniPath ) ;
      }
    }

    private void button1_Click( object sender, EventArgs e )
    {
      if ( ! CheckInpurValues() ) {
        return ;
      }

      MakeRequest( RequestId.Tesuri ) ;
      SetIniData() ;
      this.DialogResult = DialogResult.OK ;
    }

    private bool CheckInpurValues()
    {
      List<string> errMsg = new List<string>() ;

      if ( string.IsNullOrEmpty( CmbKoudaiName.Text ) ) {
        errMsg.Add( "構台名を指定してください" ) ;
      }

      //手摺支柱
      if ( string.IsNullOrEmpty( CmbTesurishichuSize.Text ) ) {
        errMsg.Add( "手摺支柱のサイズを指定してください" ) ;
      }

      if ( string.IsNullOrEmpty( CmbTesurishichuMaterial.Text ) ) {
        errMsg.Add( "手摺支柱の材質を指定してください" ) ;
      }

      if ( NmcBasePitch.Value <= 0 ) {
        errMsg.Add( "手摺支柱の基準ピッチを指定してください" ) ;
      }

      if ( NmcHight.Value <= 0 ) {
        errMsg.Add( "手摺支柱の高さを指定してください" ) ;
      }

      //手摺
      if ( string.IsNullOrEmpty( CmbTesuriSize.Text ) ) {
        errMsg.Add( "手摺のサイズを指定してください" ) ;
      }

      if ( string.IsNullOrEmpty( CmbTesuriMaterial.Text ) ) {
        errMsg.Add( "手摺の材質を指定してください" ) ;
      }

      if ( NmcDanCount.Value <= 0 ) {
        errMsg.Add( "手摺の段間隔を指定してください" ) ;
      }

      if ( NmcDanCount.Value > 0 && NmcDanPitch.Value <= 0 ) {
        errMsg.Add( "手摺の段ピッチを指定してください" ) ;
      }

      if ( RbtFree.Checked && string.IsNullOrEmpty( CmbLevel.Text ) ) {
        errMsg.Add( "配置するレベルを指定してください" ) ;
      }

      if ( errMsg.Count > 0 ) {
        FrmErrorInformation frm = new FrmErrorInformation( errMsg, this ) ;
        return false ;
      }
      else {
        return true ;
      }
    }

    private void RbtFace_CheckedChanged( object sender, EventArgs e )
    {
      changeEnabler() ;
    }

    private void changeEnabler()
    {
      if ( ! this.RbtFree.Checked ) {
        this.CmbLevel.Enabled = false ;
        this.NmcOffset.Enabled = false ;
        if ( CmbLevel.Items.Contains( "部材選択" ) ) {
          CmbLevel.Items.Remove( "部材選択" ) ;
        }
      }
      else {
        this.CmbLevel.Enabled = true ;
        this.NmcOffset.Enabled = true ;
        if ( ! CmbLevel.Items.Contains( "部材選択" ) ) {
          CmbLevel.Items.Add( "部材選択" ) ;
        }
      }
    }

    private void button3_Click( object sender, EventArgs e )
    {
      //初期化処理
      MakeRequest( RequestId.End ) ;
      m_ExEvent.Dispose() ;
      m_ExEvent = null ;
      m_Handler = null ;

      this.DialogResult = DialogResult.Cancel ;
      this.Close() ;
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
  }
}