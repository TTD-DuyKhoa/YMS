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
  public partial class FrmPutFukkouban : System.Windows.Forms.Form
  {
    #region 定数

    /// <summary>
    /// Iniファイルのパス
    /// </summary>
    const string IniPath = "ini\\FrmPutFukkouban.ini" ;

    /// <summary>
    /// Iniファイルのセクション名
    /// </summary>
    const string sec = "FrmPutFukkouban" ;

    #endregion

    private RequestHandler m_Handler ;
    private ExternalEvent m_ExEvent ;

    private UIApplication _uiApp { get ; set ; }
    private Document doc { get ; set ; }

    public FrmPutFukkouban( UIApplication uiApp )
    {
      InitializeComponent() ;
      _uiApp = uiApp ;
      doc = uiApp.ActiveUIDocument.Document ;
    }

    public FrmPutFukkouban( ExternalEvent exEvent, RequestHandler handler, UIApplication uiApp )
    {
      InitializeComponent() ;
      _uiApp = uiApp ;
      doc = uiApp.ActiveUIDocument.Document ;

      m_Handler = handler ;
      m_ExEvent = exEvent ;
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

    private void button1_Click( object sender, EventArgs e )
    {
      if ( ! CheckInpurValues() ) {
        return ;
      }

      MakeRequest( RequestId.Fukkouban ) ;
      SetIniData() ;
      this.DialogResult = DialogResult.OK ;
    }

    private void radioButton4_CheckedChanged( object sender, EventArgs e )
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

    private void FrmPutFukkouban_Load( object sender, EventArgs e )
    {
      ////構台名をコンボボックスに追加
      //comboBox12.Items.Clear();
      //comboBox12.Items.AddRange(GantryUtil.GetAllKoudaiName(doc).ToArray());

      ////レベル追加
      //comboBox18.Items.Clear();
      //comboBox18.Items.AddRange(GantryUtil.GetAllLevelName(doc).ToArray());

      InitComboBox() ;
      GetIniData() ;
      changeEnabler() ;
      UpdateSize() ;
    }

    /// <summary>
    /// コンボボックス初期化
    /// </summary>
    private void InitComboBox()
    {
      List<string> lstStr = new List<string>() ;

      //構台名をコンボボックスに追加
      ControlUtils.SetComboBoxItems( CmbKoudaiName, GantryUtil.GetAllKoudaiName( doc ).ToList() ) ;

      //レベル追加
      ControlUtils.SetComboBoxItems( CmbLevel, GantryUtil.GetAllLevelName( doc ).ToList() ) ;

      //覆工板名
      lstStr = Master.ClsFukkoubanCsv.GetSizeList( Master.ClsFukkoubanCsv.TypeM ) ;
      lstStr.AddRange( Master.ClsFukkoubanCsv.GetSizeList( Master.ClsFukkoubanCsv.TypeNormal ) ) ;
      ControlUtils.SetComboBoxItems( CmbSize, lstStr ) ;

      //材質
      lstStr = DefineUtil.ListMaterial ;
      ControlUtils.SetComboBoxItems( CmbMaterial, lstStr ) ;
    }

    /// <summary>
    /// ダイアログデータをini二セット
    /// </summary>
    public void SetIniData()
    {
      string iniPath = ClsIni.GetIniFilePath( IniPath ) ;
      ClsIni.WritePrivateProfileString( sec, CmbSize.Name, CmbSize.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, CmbMaterial.Name, CmbMaterial.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtFace.Name, RbtFace.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtFree.Name, RbtFree.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, CmbLevel.Name, CmbLevel.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, NmcOffset.Name, NmcOffset.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, NmcHukuinCnt.Name, NmcHukuinCnt.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, NmcKyoutyouCnt.Name, NmcKyoutyouCnt.Text, iniPath ) ;
    }

    /// <summary>
    /// iniデータをダイアログにセット
    /// </summary>
    public void GetIniData()
    {
      string iniPath = ClsIni.GetIniFilePath( IniPath ) ;
      if ( File.Exists( iniPath ) ) {
        CmbSize.Text = ClsIni.GetIniFile( sec, CmbSize.Name, iniPath ) ;
        CmbMaterial.Text = ClsIni.GetIniFile( sec, CmbMaterial.Name, iniPath ) ;
        RbtFace.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtFace.Name, iniPath ) ) ;
        RbtFree.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtFree.Name, iniPath ) ) ;
        CmbLevel.Text = ClsIni.GetIniFile( sec, CmbLevel.Name, iniPath ) ;
        NmcOffset.Text = ClsIni.GetIniFile( sec, NmcOffset.Name, iniPath ) ;
        NmcHukuinCnt.Text = ClsIni.GetIniFile( sec, NmcHukuinCnt.Name, iniPath ) ;
        NmcKyoutyouCnt.Text = ClsIni.GetIniFile( sec, NmcKyoutyouCnt.Name, iniPath ) ;
      }
    }

    /// <summary>
    /// NumericupDownの空回避用処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void numericUpDown_Leave( object sender, EventArgs e )
    {
      NumericUpDown nmc = (NumericUpDown) sender ;
      if ( nmc.Text == "" ) {
        nmc.Value = 0 ;
        nmc.Text = "0" ;
      }

      base.OnLostFocus( e ) ;
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

    private void radioButton3_CheckedChanged( object sender, EventArgs e )
    {
    }

    private void CmbMaterial_SelectedIndexChanged( object sender, EventArgs e )
    {
      UpdateSize() ;
    }

    private void UpdateSize()
    {
      //材質によってサイズを更新
      List<string> sizeList = new List<string>() ;
      if ( this.CmbMaterial.Text == "SS400" ) {
        sizeList = Master.ClsFukkoubanCsv.GetSizeList( Master.ClsFukkoubanCsv.TypeNormal ) ;
      }
      else if ( this.CmbMaterial.Text == "SM490" ) {
        sizeList = Master.ClsFukkoubanCsv.GetSizeList( Master.ClsFukkoubanCsv.TypeM ) ;
      }
      else {
        sizeList = Master.ClsFukkoubanCsv.GetSizeList( Master.ClsFukkoubanCsv.TypeNormal ) ;
        sizeList.AddRange( Master.ClsFukkoubanCsv.GetSizeList( Master.ClsFukkoubanCsv.TypeM ) ) ;
      }

      ControlUtils.SetComboBoxItems( CmbSize, sizeList ) ;
    }

    private bool CheckInpurValues()
    {
      List<string> errMsg = new List<string>() ;

      if ( string.IsNullOrEmpty( CmbKoudaiName.Text ) ) {
        errMsg.Add( "構台名を指定してください" ) ;
      }

      if ( string.IsNullOrEmpty( CmbMaterial.Text ) ) {
        errMsg.Add( "材質を設定してください" ) ;
      }

      if ( string.IsNullOrEmpty( CmbSize.Text ) ) {
        errMsg.Add( "サイズを設定してください" ) ;
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
  }
}