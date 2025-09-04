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
  public partial class FrmPutTaikeikou : System.Windows.Forms.Form
  {
    #region 定数

    /// <summary>
    /// Iniファイルのパス
    /// </summary>
    const string IniPath = "ini\\FrmPutTaikeikou.ini" ;

    /// <summary>
    /// Iniファイルのセクション名
    /// </summary>
    const string sec = "FrmPutTaikeikou" ;

    #endregion

    private RequestHandler m_Handler ;
    private ExternalEvent m_ExEvent ;
    private UIApplication _uiApp { get ; set ; }
    private Document doc { get ; set ; }

    public FrmPutTaikeikou( UIApplication uiApp )
    {
      _uiApp = uiApp ;
      doc = uiApp.ActiveUIDocument.Document ;
      InitializeComponent() ;
    }

    public FrmPutTaikeikou( ExternalEvent exEvent, RequestHandler handler, UIApplication uiApp )
    {
      _uiApp = uiApp ;
      doc = uiApp.ActiveUIDocument.Document ;
      InitializeComponent() ;
      m_Handler = handler ;
      m_ExEvent = exEvent ;
    }

    private void radioButton4_CheckedChanged( object sender, EventArgs e )
    {
      if ( this.RbtAttachWelding.Checked ) {
        this.groupBox2.Enabled = false ;
        return ;
      }

      this.groupBox2.Enabled = true ;
    }

    private void FrmPutTaikeikou_Load( object sender, EventArgs e )
    {
      InitComboBox() ;
      GetIniData() ;
    }

    private void InitComboBox()
    {
      //構台名をコンボボックスに追加
      ControlUtils.SetComboBoxItems( this.CmbKoudainame, GantryUtil.GetAllKoudaiName( doc ).ToList() ) ;

      //サイズ
      List<string> list = Master.ClsTaikeikouCsv.GetAllSizeList() ;
      ControlUtils.SetComboBoxItems( this.CmbSize, list ) ;

      //材質
      ControlUtils.SetComboBoxItems( this.CmbMaterial, DefineUtil.ListMaterial ) ;
    }

    private void NmcClearlance_Leave( object sender, EventArgs e )
    {
      NumericUpDown nmc = (NumericUpDown) sender ;
      if ( nmc.Text == "" ) {
        nmc.Value = 0 ;
        nmc.Text = "0" ;
      }

      base.OnLostFocus( e ) ;
    }

    private void button2_Click( object sender, EventArgs e )
    {
      //初期化処理
      MakeRequest( RequestId.End ) ;
      m_ExEvent.Dispose() ;
      m_ExEvent = null ;
      m_Handler = null ;

      this.DialogResult = DialogResult.Cancel ;
      this.Close() ;
    }

    private void button1_Click( object sender, EventArgs e )
    {
      if ( ! CheckInpurValues() ) {
        return ;
      }

      MakeRequest( RequestId.Taikeikou ) ;
      SetIniData() ;
      this.DialogResult = DialogResult.OK ;
    }

    /// <summary>
    /// ダイアログデータをini二セット
    /// </summary>
    public void SetIniData()
    {
      string iniPath = ClsIni.GetIniFilePath( IniPath ) ;
      ClsIni.WritePrivateProfileString( sec, CmbSize.Name, CmbSize.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, ChkNormalSize.Name, ChkNormalSize.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, CmbMaterial.Name, CmbMaterial.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, NmcClearlance.Name, NmcClearlance.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtAttatchBolt.Name, RbtAttatchBolt.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtAttachWelding.Name, RbtAttachWelding.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtStiffenerY.Name, RbtStiffenerY.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtStiffenerN.Name, RbtStiffenerN.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtBolt.Name, RbtBolt.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtHTB.Name, RbtHTB.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, NmcBoltCnt.Name, NmcBoltCnt.Text, iniPath ) ;
    }

    /// <summary>
    /// iniデータをダイアログにセット
    /// </summary>
    public void GetIniData()
    {
      string iniPath = ClsIni.GetIniFilePath( IniPath ) ;
      if ( File.Exists( iniPath ) ) {
        CmbSize.Text = ClsIni.GetIniFile( sec, CmbSize.Name, iniPath ) ;
        ChkNormalSize.Checked = bool.Parse( ClsIni.GetIniFile( sec, ChkNormalSize.Name, iniPath ) ) ;
        CmbMaterial.Text = ClsIni.GetIniFile( sec, CmbMaterial.Name, iniPath ) ;
        NmcClearlance.Text = ClsIni.GetIniFile( sec, NmcClearlance.Name, iniPath ) ;
        RbtAttatchBolt.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtAttatchBolt.Name, iniPath ) ) ;
        RbtAttachWelding.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtAttachWelding.Name, iniPath ) ) ;
        RbtStiffenerY.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtStiffenerY.Name, iniPath ) ) ;
        RbtStiffenerN.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtStiffenerN.Name, iniPath ) ) ;
        RbtBolt.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtBolt.Name, iniPath ) ) ;
        RbtHTB.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtHTB.Name, iniPath ) ) ;
        NmcBoltCnt.Text = ClsIni.GetIniFile( sec, NmcBoltCnt.Name, iniPath ) ;
      }
    }

    private bool CheckInpurValues()
    {
      List<string> errMsg = new List<string>() ;

      if ( string.IsNullOrEmpty( CmbKoudainame.Text ) ) {
        errMsg.Add( "構台名を指定してください" ) ;
      }

      if ( string.IsNullOrEmpty( CmbMaterial.Text ) ) {
        errMsg.Add( "材質を設定してください" ) ;
      }

      if ( string.IsNullOrEmpty( CmbSize.Text ) ) {
        errMsg.Add( "サイズを設定してください" ) ;
      }

      if ( NmcClearlance.Value < 0 ) {
        errMsg.Add( "クリアランスの値が正しくありません" ) ;
      }

      if ( RbtAttatchBolt.Checked && NmcBoltCnt.Value <= 0 ) {
        errMsg.Add( "ボルト本数を指定してください" ) ;
      }

      if ( errMsg.Count > 0 ) {
        FrmErrorInformation frm = new FrmErrorInformation( errMsg, this ) ;
        return false ;
      }
      else {
        return true ;
      }
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