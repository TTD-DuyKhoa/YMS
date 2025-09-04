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
  public partial class FrmPutKoubanzai : System.Windows.Forms.Form
  {
    #region 定数

    /// <summary>
    /// Iniファイルのパス
    /// </summary>
    const string IniPath = "ini\\FrmPutKoubanzai.ini" ;

    /// <summary>
    /// Iniファイルのセクション名
    /// </summary>
    const string sec = "FrmPutKoubanzai" ;

    #endregion

    private RequestHandler m_Handler ;
    private ExternalEvent m_ExEvent ;

    private UIApplication _uiApp { get ; set ; }
    private Document doc { get ; set ; }

    public FrmPutKoubanzai( UIApplication uiApp )
    {
      InitializeComponent() ;

      _uiApp = uiApp ;
      doc = uiApp.ActiveUIDocument.Document ;
    }

    public FrmPutKoubanzai( ExternalEvent exEvent, RequestHandler handler, UIApplication uiApp )
    {
      InitializeComponent() ;
      _uiApp = uiApp ;
      doc = uiApp.ActiveUIDocument.Document ;

      m_Handler = handler ;
      m_ExEvent = exEvent ;
    }

    private void FrmPutKoubanzai_Load( object sender, EventArgs e )
    {
      ////構台名をコンボボックスに追加
      //comboBox12.Items.Clear();
      //comboBox12.Items.AddRange(GantryUtil.GetAllKoudaiName(doc).ToArray());

      ////レベル追加
      //comboBox18.Items.Clear();
      //comboBox18.Items.AddRange(GantryUtil.GetAllLevelName(doc).ToArray());
      //comboBox18.Text = comboBox18.Items[0].ToString();

      InitComboBox() ;
      //InitSize();
      changeEnabler() ;
      GetIniData() ;
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

      //鋼板材
      InitComboBoxKoubanzai() ;

      //鋼板材厚さ
      lstStr = Master.ClsKoubanzaiCsv.Thickness ;
      ControlUtils.SetComboBoxItems( CmbThick, lstStr ) ;
    }

    /// <summary>
    /// コンボボックス初期化（鋼板材）
    /// </summary>
    private void InitComboBoxKoubanzai()
    {
      List<string> lstStr = new List<string>() ;

      //鋼板材
      string type = string.Empty ;
      if ( RbtShimakouhan.Checked ) {
        type = Master.ClsKoubanzaiCsv.TypeShimaKouban ;
      }
      else {
        type = Master.ClsKoubanzaiCsv.TypeShikiteppan ;
      }

      lstStr = Master.ClsKoubanzaiCsv.GetSizeList( type ) ;

      ControlUtils.SetComboBoxItems( CmbSize, lstStr ) ;
    }

    /// <summary>
    /// ダイアログデータをini二セット
    /// </summary>
    public void SetIniData()
    {
      string iniPath = ClsIni.GetIniFilePath( IniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtShimakouhan.Name, RbtShimakouhan.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtShikiteppan.Name, RbtShikiteppan.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, CmbSize.Name, CmbSize.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, CmbThick.Name, CmbThick.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtFace.Name, RbtFace.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtFree.Name, RbtFree.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, CmbLevel.Name, CmbLevel.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, NmcOffset.Name, NmcOffset.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, NmcLongCnt.Name, NmcLongCnt.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, NmcShortCnt.Name, NmcShortCnt.Text, iniPath ) ;
    }

    /// <summary>
    /// iniデータをダイアログにセット
    /// </summary>
    public void GetIniData()
    {
      string iniPath = ClsIni.GetIniFilePath( IniPath ) ;
      if ( File.Exists( iniPath ) ) {
        RbtShimakouhan.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtShimakouhan.Name, iniPath ) ) ;
        RbtShikiteppan.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtShikiteppan.Name, iniPath ) ) ;
        CmbSize.Text = ClsIni.GetIniFile( sec, CmbSize.Name, iniPath ) ;
        CmbThick.Text = ClsIni.GetIniFile( sec, CmbThick.Name, iniPath ) ;
        RbtFace.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtFace.Name, iniPath ) ) ;
        RbtFree.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtFree.Name, iniPath ) ) ;
        CmbLevel.Text = ClsIni.GetIniFile( sec, CmbLevel.Name, iniPath ) ;
        NmcOffset.Text = ClsIni.GetIniFile( sec, NmcOffset.Name, iniPath ) ;
        NmcLongCnt.Text = ClsIni.GetIniFile( sec, NmcLongCnt.Name, iniPath ) ;
        NmcShortCnt.Text = ClsIni.GetIniFile( sec, NmcShortCnt.Name, iniPath ) ;
      }
    }

    //Okボタン
    private void button1_Click( object sender, EventArgs e )
    {
      if ( ! CheckInpurValues() ) {
        return ;
      }

      MakeRequest( RequestId.Koubanzai ) ;
      SetIniData() ;
      this.DialogResult = DialogResult.OK ;
    }

    private void comboBox12_SelectedIndexChanged( object sender, EventArgs e )
    {
    }

    //キャンセルボタン
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

    private void radioButton1_CheckedChanged( object sender, EventArgs e )
    {
      InitComboBoxKoubanzai() ;
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

    private void FrmPutKoubanzai_Leave( object sender, EventArgs e )
    {
      NumericUpDown nmc = (NumericUpDown) sender ;
      if ( nmc.Text == "" ) {
        nmc.Value = 0 ;
        nmc.Text = "0" ;
      }

      base.OnLostFocus( e ) ;
    }


    private bool CheckInpurValues()
    {
      List<string> errMsg = new List<string>() ;

      if ( string.IsNullOrEmpty( CmbKoudaiName.Text ) ) {
        errMsg.Add( "構台名を指定してください" ) ;
      }

      if ( string.IsNullOrEmpty( CmbSize.Text ) ) {
        errMsg.Add( "サイズを設定してください" ) ;
      }

      if ( string.IsNullOrEmpty( CmbThick.Text ) ) {
        errMsg.Add( "厚みを設定してください" ) ;
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

    private void FrmPutKoubanzai_Activated( object sender, EventArgs e )
    {
      //this.ProcessTabKey(true);
      //bool isf = this.CanFocus;
      //this.Focus();
      //bool fofo = this.Focused;

      //var owner = this.Owner;
      //var ownerForms = this.OwnedForms;            
    }
  }
}