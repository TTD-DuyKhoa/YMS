using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using System ;
using System.Collections.Generic ;
using System.ComponentModel ;
using System.Data ;
using System.Drawing ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;
using YMS_gantry.Data ;
using YMS.gantry ;
using YMS_gantry.GantryUtils ;
using System.IO ;

namespace YMS_gantry.UI
{
  public partial class FrmPutShikigeta : System.Windows.Forms.Form
  {
    #region 定数

    /// <summary>
    /// Iniファイルのパス
    /// </summary>
    const string IniPath = "ini\\FrmPutShikigeta.ini" ;

    /// <summary>
    /// Iniファイルのセクション名
    /// </summary>
    const string sec = "FrmPutShikigeta" ;

    #endregion

    private RequestHandler m_Handler ;
    private ExternalEvent m_ExEvent ;

    private UIApplication _uiApp { get ; set ; }
    private Document doc { get ; set ; }

    public FrmPutShikigeta( UIApplication uiApp )
    {
      InitializeComponent() ;

      _uiApp = uiApp ;
      doc = uiApp.ActiveUIDocument.Document ;
    }

    public FrmPutShikigeta( ExternalEvent exEvent, RequestHandler handler, UIApplication uiApp )
    {
      InitializeComponent() ;

      _uiApp = uiApp ;
      doc = uiApp.ActiveUIDocument.Document ;

      m_Handler = handler ;
      m_ExEvent = exEvent ;
    }

    private void FrmPutShikigeta_Load( object sender, EventArgs e )
    {
      ////構台名をコンボボックスに追加
      //comboBox12.Items.Clear();
      //comboBox12.Items.AddRange(GantryUtil.GetAllKoudaiName(doc).ToArray());

      ////レベル追加
      //comboBox18.Items.Clear();
      //comboBox18.Items.AddRange(GantryUtil.GetAllLevelName(doc).ToArray());
      //comboBox18.Text = comboBox18.Items[0].ToString();

      InitComboBox() ;
      ChechkPalcementType() ;
      GetIniData() ;
      changeEnabler() ;
    }

    /// <summary>
    /// コンボボックス初期化
    /// </summary>
    private void InitComboBox()
    {
      List<string> lstStr = new List<string>() ;

      //イベント
      CmbSizeType.SelectedIndexChanged += InitComboBoxShikigeta ;

      //構台名をコンボボックスに追加
      ControlUtils.SetComboBoxItems( CmbKoudaiName, GantryUtil.GetAllKoudaiName( doc ).ToList() ) ;

      //レベル追加
      ControlUtils.SetComboBoxItems( CmbLevel, GantryUtil.GetAllLevelName( doc ).ToList() ) ;
      CmbLevel.Items.Add( "部材選択" ) ;

      //敷桁
      lstStr = new List<string>()
      {
        Master.ClsHBeamCsv.TypeHiro,
        Master.ClsHBeamCsv.TypeNaka,
        Master.ClsHBeamCsv.TypeHoso,
        Master.ClsYamadomeCsv.TypeShuzai,
        Master.ClsYamadomeCsv.TypeHighShuzai
      } ;
      ControlUtils.SetComboBoxItems( CmbSizeType, lstStr ) ;

      //材質
      lstStr = DefineUtil.ListMaterial ;
      ControlUtils.SetComboBoxItems( CmbMaterial, lstStr ) ;
    }

    /// <summary>
    /// コンボボックス初期化（敷桁）
    /// </summary>
    private void InitComboBoxShikigeta( object sender, EventArgs e )
    {
      List<string> lstStr = new List<string>() ;

      //敷桁
      string type = CmbSizeType.Text ;

      if ( type == Master.ClsYamadomeCsv.TypeShuzai || type == Master.ClsYamadomeCsv.TypeHighShuzai ) //主材
      {
        lstStr = Master.ClsYamadomeCsv.GetSizeList( type ) ;
      }
      else if ( type == Master.ClsHBeamCsv.TypeHoso || type == Master.ClsHBeamCsv.TypeNaka ||
                type == Master.ClsHBeamCsv.TypeHiro ) //H鋼
      {
        lstStr = Master.ClsHBeamCsv.GetSizeList( type ) ;
      }

      ControlUtils.SetComboBoxItems( CmbSize, lstStr ) ;
    }

    private void ChechkPalcementType()
    {
      // 24/02/26 del 自動配置不要
      //if (this.RbtAuto.Checked)
      //{
      //    this.CmbLevel.Enabled = false;
      //    this.NmcOffset.Enabled = false;
      //    if (CmbLevel.Items.Contains("部材選択"))
      //    {
      //        CmbLevel.Items.Remove("部材選択");
      //    }
      //}
      //else
      //{
      //    this.CmbLevel.Enabled = true;
      //    this.NmcOffset.Enabled = true;
      //    if (!CmbLevel.Items.Contains("部材選択"))
      //    {
      //        CmbLevel.Items.Add("部材選択");
      //    }
      //}
    }

    /// <summary>
    /// ダイアログデータをini二セット
    /// </summary>
    public void SetIniData()
    {
      string iniPath = ClsIni.GetIniFilePath( IniPath ) ;
      ClsIni.WritePrivateProfileString( sec, CmbSizeType.Name, CmbSizeType.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, CmbSize.Name, CmbSize.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, CmbMaterial.Name, CmbMaterial.Text, iniPath ) ;
      //ClsIni.WritePrivateProfileString(sec, RbtAuto.Name, RbtAuto.Checked.ToString(), iniPath);
      //ClsIni.WritePrivateProfileString(sec, RbtFree.Name, RbtFree.Checked.ToString(), iniPath);
      ClsIni.WritePrivateProfileString( sec, CmbLevel.Name, CmbLevel.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, NmcOffset.Name, NmcOffset.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, NmcSLng.Name, NmcSLng.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, NmcELng.Name, NmcELng.Text, iniPath ) ;
    }

    /// <summary>
    /// iniデータをダイアログにセット
    /// </summary>
    public void GetIniData()
    {
      string iniPath = ClsIni.GetIniFilePath( IniPath ) ;
      if ( File.Exists( iniPath ) ) {
        CmbSizeType.Text = ClsIni.GetIniFile( sec, CmbSizeType.Name, iniPath ) ;
        CmbSize.Text = ClsIni.GetIniFile( sec, CmbSize.Name, iniPath ) ;
        CmbMaterial.Text = ClsIni.GetIniFile( sec, CmbMaterial.Name, iniPath ) ;
        //RbtAuto.Checked = bool.Parse(ClsIni.GetIniFile(sec, RbtAuto.Name, iniPath));
        //RbtFree.Checked = bool.Parse(ClsIni.GetIniFile(sec, RbtFree.Name, iniPath));
        CmbLevel.Text = ClsIni.GetIniFile( sec, CmbLevel.Name, iniPath ) ;
        NmcOffset.Text = ClsIni.GetIniFile( sec, NmcOffset.Name, iniPath ) ;
        NmcSLng.Text = ClsIni.GetIniFile( sec, NmcSLng.Name, iniPath ) ;
        NmcELng.Text = ClsIni.GetIniFile( sec, NmcELng.Name, iniPath ) ;
      }
    }

    private void button1_Click( object sender, EventArgs e )
    {
      if ( ! CheckInpurValues() ) {
        return ;
      }

      MakeRequest( RequestId.Shikigeta ) ;
      SetIniData() ;
      this.DialogResult = DialogResult.OK ;
    }

    private void comboBox12_SelectedIndexChanged( object sender, EventArgs e )
    {
      string koudaiName = this.CmbKoudaiName.Text ;
      KoudaiData data = GantryUtil.GetKoudaiData( doc, koudaiName ) ;
      if ( data.AllKoudaiFlatData != null && data.AllKoudaiFlatData.KoudaiName != "" ) {
        ShikigetaData sData = data.AllKoudaiFlatData.shikigetaData ;
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

    private void RbtAuto_CheckedChanged( object sender, EventArgs e )
    {
      changeEnabler() ;
    }

    private void changeEnabler()
    {
      // 24/02/26 del 自動配置不要
      //if (!this.RbtFree.Checked)
      //{
      //    this.CmbLevel.Enabled = false;
      //    this.NmcOffset.Enabled = false;
      //    if (CmbLevel.Items.Contains("部材選択"))
      //    {
      //        CmbLevel.Items.Remove("部材選択");
      //    }
      //}
      //else
      //{
      //    this.CmbLevel.Enabled = true;
      //    this.NmcOffset.Enabled = true;
      //    if (!CmbLevel.Items.Contains("部材選択"))
      //    {
      //        CmbLevel.Items.Add("部材選択");
      //    }
      //}
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

      if ( string.IsNullOrEmpty( CmbLevel.Text ) ) {
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
  }
}