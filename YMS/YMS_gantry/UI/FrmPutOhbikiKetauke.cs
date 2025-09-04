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
  public partial class FrmPutOhbikiKetauke : System.Windows.Forms.Form
  {
    #region 定数

    /// <summary>
    /// Iniファイルのパス
    /// </summary>
    const string IniPath = "ini\\FrmPutOhbikiKetauke.ini" ;

    /// <summary>
    /// Iniファイルのセクション名
    /// </summary>
    const string sec = "FrmPutOhbikiKetauke" ;

    #endregion

    private RequestHandler m_Handler ;
    private ExternalEvent m_ExEvent ;

    private UIApplication _uiApp { get ; set ; }
    private Document doc { get ; set ; }

    public FrmPutOhbikiKetauke( UIApplication uiApp )
    {
      InitializeComponent() ;

      _uiApp = uiApp ;
      doc = uiApp.ActiveUIDocument.Document ;
    }

    public FrmPutOhbikiKetauke( ExternalEvent exEvent, RequestHandler handler, UIApplication uiApp )
    {
      InitializeComponent() ;

      _uiApp = uiApp ;
      doc = uiApp.ActiveUIDocument.Document ;
      m_Handler = handler ;
      m_ExEvent = exEvent ;
    }

    private void comboBox3_SelectedIndexChanged( object sender, EventArgs e )
    {
      //bool isHkou = string.IsNullOrEmpty(this.comboBox3.Text)&&this.comboBox3.SelectedItem.ToString().Equals("H鋼");

      bool isHkou = ! string.IsNullOrEmpty( this.CmbSizeType.Text ) &&
                    ( ! this.CmbSizeType.SelectedItem.ToString().Equals( "チャンネル" ) &&
                      ! this.CmbSizeType.SelectedItem.ToString().Equals( "アングル" ) ) ;
      //H鋼で不要な設定は触らせない
      panel1.Enabled = ! isHkou ;
      panel2.Enabled = ! isHkou ;
      panel3.Enabled = ! isHkou ;
      NmcOhbikiBoltCnt.Enabled = ! isHkou ;
      RbtOhbikiDouble.Enabled = ! isHkou ;
      RbtOhbikiTriple.Enabled = ! isHkou ;
      if ( isHkou ) {
        RbtOhbikiSingle.Checked = true ;
      }

      this.Refresh() ;
    }

    private void button1_Click( object sender, EventArgs e )
    {
      if ( ! CheckInpurValues() ) {
        return ;
      }

      MakeRequest( RequestId.Ohbiki ) ;
      SetIniData() ;
      this.DialogResult = DialogResult.OK ;
    }

    private void comboBox12_SelectedIndexChanged( object sender, EventArgs e )
    {
      KoudaiChange() ;
    }

    private void KoudaiChange()
    {
      string koudaiName = this.CmbKoudaiName.Text ;
      KoudaiData data = GantryUtil.GetKoudaiData( doc, koudaiName ) ;
      List<string> lstStr = new List<string>() ;

      if ( data.AllKoudaiFlatData != null && data.AllKoudaiFlatData.KoudaiName != "" ) {
        AllKoudaiFlatFrmData kdata = data.AllKoudaiFlatData ;
        // 大引⇔桁受け　と異なる
        if ( kdata.KoujiType == DefineUtil.eKoujiType.Kenchiku ) {
          this.Text = "[個別] 大引配置" ;
          groupBox2.Enabled = false ;
          groupBox4.Enabled = false ;
          RbtOhbikiSingle.Checked = true ;
          //大引き
          lstStr = new List<string>()
          {
            Master.ClsHBeamCsv.TypeHiro,
            Master.ClsHBeamCsv.TypeNaka,
            Master.ClsHBeamCsv.TypeHoso,
            Master.ClsYamadomeCsv.TypeShuzai,
            Master.ClsYamadomeCsv.TypeHighShuzai
          } ;
        }
        else {
          this.Text = "[個別] 桁受配置" ;
          groupBox2.Enabled = true ;
          groupBox4.Enabled = true ;
          if ( ! kdata.ohbikiData.isHkou ) {
            this.RbtOhbikiBolt.Checked = kdata.ohbikiData.OhbikiAttachType == DefineUtil.eJoinType.Bolt ? true : false ;
            this.RbtOhbikiWeld.Checked =
              kdata.ohbikiData.OhbikiAttachType == DefineUtil.eJoinType.Welding ? true : false ;

            this.RbtOhbikiNormalB.Checked =
              kdata.ohbikiData.OhbikiBoltType == DefineUtil.eBoltType.Normal ? true : false ;
            this.RbtOhbikiBHTB.Checked = kdata.ohbikiData.OhbikiBoltType == DefineUtil.eBoltType.HTB ? true : false ;

            this.NmcOhbikiBoltCnt.Value = kdata.ohbikiData.OhbikiBoltCount ;
          }

          if ( kdata.ohbikiData.OhbikiDan.Equals( 3 ) ) {
            this.RbtOhbikiTriple.Checked = true ;
          }
          else if ( kdata.ohbikiData.OhbikiDan.Equals( 2 ) ) {
            this.RbtOhbikiDouble.Checked = true ;
          }
          else {
            this.RbtOhbikiSingle.Checked = true ;
          }

          lstStr = new List<string>()
          {
            Master.ClsHBeamCsv.TypeHiro,
            Master.ClsHBeamCsv.TypeNaka,
            Master.ClsHBeamCsv.TypeHoso,
            Master.ClsAngleCsv.TypeAngle,
            Master.ClsYamadomeCsv.TypeShuzai,
            Master.ClsYamadomeCsv.TypeHighShuzai,
            Master.ClsChannelCsv.TypeCannel
          } ;
        }

        this.CmbSize.Text = kdata.ohbikiData.OhbikiSize ;
        this.CmbMaterial.Text = kdata.ohbikiData.OhbikiMaterial ;

        this.CmbLevel.Text = kdata.SelectedLevel ;
        this.NmcOffset.Value = (decimal) kdata.LevelOffset ;

        this.NmcSLng.Value = (decimal) kdata.ohbikiData.exOhbikiStartLeng ;
        this.NmcELng.Value = (decimal) kdata.ohbikiData.exOhbikiEndLeng ;
      }

      ControlUtils.SetComboBoxItems( CmbSizeType, lstStr ) ;
    }

    private void FrmPutOhbikiKetauke_Load( object sender, EventArgs e )
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
      KoudaiChange() ;
      GetIniData() ;
    }


    /// <summary>
    /// コンボボックス初期化
    /// </summary>
    private void InitComboBox()
    {
      List<string> lstStr = new List<string>() ;

      //イベント
      CmbSizeType.SelectedIndexChanged += InitComboBoxOhbiki ;

      //構台名をコンボボックスに追加
      ControlUtils.SetComboBoxItems( CmbKoudaiName, GantryUtil.GetAllKoudaiName( doc ).ToList() ) ;

      //レベル追加
      ControlUtils.SetComboBoxItems( CmbLevel, GantryUtil.GetAllLevelName( doc ).ToList() ) ;

      //大引き
      lstStr = new List<string>()
      {
        Master.ClsHBeamCsv.TypeHiro,
        Master.ClsHBeamCsv.TypeNaka,
        Master.ClsHBeamCsv.TypeHoso,
        Master.ClsYamadomeCsv.TypeShuzai,
        Master.ClsYamadomeCsv.TypeHighShuzai,
        Master.ClsAngleCsv.TypeAngle,
        Master.ClsChannelCsv.TypeCannel
      } ;
      ControlUtils.SetComboBoxItems( CmbSizeType, lstStr ) ;

      //材質
      lstStr = DefineUtil.ListMaterial ;
      ControlUtils.SetComboBoxItems( CmbMaterial, lstStr ) ;
    }

    /// <summary>
    /// コンボボックス初期化（大引）
    /// </summary>
    private void InitComboBoxOhbiki( object sender, EventArgs e )
    {
      List<string> lstStr = new List<string>() ;

      //大引
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
      else if ( type == Master.ClsChannelCsv.TypeCannel ) //C材
      {
        lstStr = Master.ClsChannelCsv.GetSizeList() ;
      }
      else if ( type == Master.ClsAngleCsv.TypeAngle ) //L材
      {
        lstStr = Master.ClsAngleCsv.GetSizeList( type ) ;
      }

      ControlUtils.SetComboBoxItems( CmbSize, lstStr ) ;
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
      ClsIni.WritePrivateProfileString( sec, RbtOhbikiSingle.Name, RbtOhbikiSingle.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtOhbikiDouble.Name, RbtOhbikiDouble.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtOhbikiTriple.Name, RbtOhbikiTriple.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtKui.Name, RbtKui.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtFree.Name, RbtFree.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, CmbLevel.Name, CmbLevel.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, NmcOffset.Name, NmcOffset.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, NmcSLng.Name, NmcSLng.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, NmcELng.Name, NmcELng.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtOneSide.Name, RbtOneSide.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtBothSide.Name, RbtBothSide.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtOhbikiBolt.Name, RbtOhbikiBolt.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtOhbikiWeld.Name, RbtOhbikiWeld.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtOhbikiNormalB.Name, RbtOhbikiNormalB.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtOhbikiBHTB.Name, RbtOhbikiBHTB.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, NmcOhbikiBoltCnt.Name, NmcOhbikiBoltCnt.Text, iniPath ) ;
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
        RbtOhbikiSingle.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtOhbikiSingle.Name, iniPath ) ) ;
        RbtOhbikiDouble.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtOhbikiDouble.Name, iniPath ) ) ;
        RbtOhbikiTriple.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtOhbikiTriple.Name, iniPath ) ) ;
        RbtKui.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtKui.Name, iniPath ) ) ;
        RbtFree.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtFree.Name, iniPath ) ) ;
        CmbLevel.Text = ClsIni.GetIniFile( sec, CmbLevel.Name, iniPath ) ;
        NmcOffset.Text = ClsIni.GetIniFile( sec, NmcOffset.Name, iniPath ) ;
        NmcSLng.Text = ClsIni.GetIniFile( sec, NmcSLng.Name, iniPath ) ;
        NmcELng.Text = ClsIni.GetIniFile( sec, NmcELng.Name, iniPath ) ;
        RbtOneSide.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtOneSide.Name, iniPath ) ) ;
        RbtBothSide.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtBothSide.Name, iniPath ) ) ;
        RbtOhbikiBolt.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtOhbikiBolt.Name, iniPath ) ) ;
        RbtOhbikiWeld.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtOhbikiWeld.Name, iniPath ) ) ;
        RbtOhbikiNormalB.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtOhbikiNormalB.Name, iniPath ) ) ;
        RbtOhbikiBHTB.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtOhbikiBHTB.Name, iniPath ) ) ;
        NmcOhbikiBoltCnt.Text = ClsIni.GetIniFile( sec, NmcOhbikiBoltCnt.Name, iniPath ) ;
      }
    }

    private void radioButton1_CheckedChanged( object sender, EventArgs e )
    {
      ChechkPalcementType() ;
    }

    private void ChechkPalcementType()
    {
      if ( this.RbtFree.Checked ) {
        this.RbtOneSide.Checked = true ;
        this.RbtBothSide.Enabled = false ;
        if ( CmbSizeType.Text == Master.ClsAngleCsv.TypeL || CmbSizeType.Text == Master.ClsChannelCsv.TypeC ) {
          RbtOhbikiSingle.Checked = true ;
          RbtOhbikiDouble.Enabled = false ;
          RbtOhbikiTriple.Enabled = false ;
        }
      }
      else {
        this.RbtBothSide.Enabled = true ;
        if ( CmbSizeType.Text == Master.ClsAngleCsv.TypeL || CmbSizeType.Text == Master.ClsChannelCsv.TypeC ) {
          RbtOhbikiDouble.Enabled = true ;
          RbtOhbikiTriple.Enabled = true ;
        }
      }

      if ( this.RbtKui.Checked ) {
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

      if ( CmbSizeType.Text == Master.ClsChannelCsv.TypeC || CmbSizeType.Text == Master.ClsAngleCsv.TypeL ) {
        if ( RbtOhbikiBolt.Checked && NmcOhbikiBoltCnt.Value <= 0 ) {
          errMsg.Add( "ボルト本数を指定してください" ) ;
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