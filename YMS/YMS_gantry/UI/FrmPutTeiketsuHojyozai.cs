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
  public partial class FrmPutTeiketsuHojyozai : System.Windows.Forms.Form
  {
    #region 定数

    /// <summary>
    /// Iniファイルのパス
    /// </summary>
    const string IniPath = "ini\\FrmPutTeiketsuHojyozai.ini" ;

    /// <summary>
    /// Iniファイルのセクション名
    /// </summary>
    const string sec = "FrmPutTeiketsuHojyozai" ;

    #endregion

    private RequestHandler m_Handler ;
    private ExternalEvent m_ExEvent ;

    private UIApplication _uiApp { get ; set ; }
    private Document doc { get ; set ; }

    public FrmPutTeiketsuHojyozai( UIApplication uiApp )
    {
      InitializeComponent() ;
      _uiApp = uiApp ;
      doc = uiApp.ActiveUIDocument.Document ;
    }

    public FrmPutTeiketsuHojyozai( ExternalEvent exEvent, RequestHandler handler, UIApplication uiApp )
    {
      InitializeComponent() ;
      _uiApp = uiApp ;
      doc = uiApp.ActiveUIDocument.Document ;

      m_Handler = handler ;
      m_ExEvent = exEvent ;
    }

    private void radioButton2_CheckedChanged( object sender, EventArgs e )
    {
      if ( this.RbtAuto.Checked ) {
        this.groupBox1.Enabled = true ;
        this.groupBox3.Enabled = false ;
        return ;
      }

      this.groupBox1.Enabled = false ;
      this.groupBox3.Enabled = true ;
    }

    private void button1_Click( object sender, EventArgs e )
    {
      if ( ! CheckInpurValues() ) {
        return ;
      }

      MakeRequest( RequestId.TeiketsuHojozai ) ;
      SetIniData() ;
      this.DialogResult = DialogResult.OK ;
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

    private void FrmPutTeiketsuHojyozai_Load( object sender, EventArgs e )
    {
      InitComboBox() ;
      GetIniData() ;
    }

    private void InitComboBox()
    {
      //構台名をコンボボックスに追加
      ControlUtils.SetComboBoxItems( CmbKoudaiName, GantryUtil.GetAllKoudaiName( doc ).ToList() ) ;

      List<string> lstStr = new List<string>() ;
      //ネコ材
      lstStr = Master.ClsTeiketsuHojoCsv.GetSizeList( Master.ClsTeiketsuHojoCsv.TypeNeko ) ;
      ControlUtils.SetComboBoxItems( this.CmbHolBrace, lstStr ) ;
      ControlUtils.SetComboBoxItems( this.CmbVertBrace_Web, lstStr ) ;
      ControlUtils.SetComboBoxItems( this.CmbVertBrace_Frange, lstStr ) ;
      ControlUtils.SetComboBoxItems( this.CmbHolJoint_Web, lstStr ) ;
      ControlUtils.SetComboBoxItems( this.CmbHolJoint_Frange, lstStr ) ;
      ControlUtils.SetComboBoxItems( this.CmbNekoFree, lstStr ) ;

      //ブルマン
      lstStr = Master.ClsTeiketsuHojoCsv.GetSizeList( Master.ClsTeiketsuHojoCsv.TypeBulman ) ;
      ControlUtils.SetComboBoxItems( this.CmbBulFree, lstStr ) ;

      //リキマン
      lstStr = Master.ClsTeiketsuHojoCsv.GetSizeList( Master.ClsTeiketsuHojoCsv.TypeRikiman ) ;
      ControlUtils.SetComboBoxItems( this.CmbRikiFree, lstStr ) ;

      UpdateUpCmbSize() ;
      UpdateBtmCmbSize() ;
    }


    private void UpdateUpCmbSize()
    {
      string type = ( this.RbtBulmanUp.Checked )
        ? Master.ClsTeiketsuHojoCsv.TypeBulman
        : Master.ClsTeiketsuHojoCsv.TypeRikiman ;
      List<string> lstStr = new List<string>() ;
      lstStr = Master.ClsTeiketsuHojoCsv.GetSizeList( type ) ;
      ControlUtils.SetComboBoxItems( this.CmbUpSize, lstStr ) ;
    }

    private void UpdateBtmCmbSize()
    {
      string type = ( this.RbtBulmanBtm.Checked )
        ? Master.ClsTeiketsuHojoCsv.TypeBulman
        : Master.ClsTeiketsuHojoCsv.TypeRikiman ;
      List<string> lstStr = new List<string>() ;
      lstStr = Master.ClsTeiketsuHojoCsv.GetSizeList( type ) ;
      ControlUtils.SetComboBoxItems( this.CmbBtmSize, lstStr ) ;
    }

    private void RbtBulmanUp_CheckedChanged( object sender, EventArgs e )
    {
      UpdateUpCmbSize() ;
    }

    private void RbtBulmanBtm_CheckedChanged( object sender, EventArgs e )
    {
      UpdateBtmCmbSize() ;
    }

    /// <summary>
    /// ダイアログデータをini二セット
    /// </summary>
    public void SetIniData()
    {
      string iniPath = ClsIni.GetIniFilePath( IniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtAuto.Name, RbtAuto.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, ChkNeko.Name, ChkNeko.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, CmbHolBrace.Name, CmbHolBrace.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, CmbVertBrace_Web.Name, CmbVertBrace_Web.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, CmbVertBrace_Frange.Name, CmbVertBrace_Frange.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, CmbHolJoint_Web.Name, CmbHolJoint_Web.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, CmbHolJoint_Frange.Name, CmbHolJoint_Frange.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, ChkUp.Name, ChkUp.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtBulmanUp.Name, RbtBulmanUp.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtRikimanUp.Name, RbtRikimanUp.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, CmbUpSize.Name, CmbUpSize.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, ChkBtm.Name, ChkBtm.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtBulmanBtm.Name, RbtBulmanBtm.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtRikimanBtm.Name, RbtRikimanBtm.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, CmbBtmSize.Name, CmbBtmSize.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtFree.Name, RbtFree.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtNekoFree.Name, RbtNekoFree.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtBulFree.Name, RbtBulFree.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtRikiFree.Name, RbtRikiFree.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, CmbNekoFree.Name, CmbNekoFree.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, CmbBulFree.Name, CmbBulFree.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, CmbRikiFree.Name, CmbRikiFree.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, ChkKiribariAsJoint.Name, ChkKiribariAsJoint.Checked.ToString(), iniPath ) ;
    }

    /// <summary>
    /// iniデータをダイアログにセット
    /// </summary>
    public void GetIniData()
    {
      string iniPath = ClsIni.GetIniFilePath( IniPath ) ;
      if ( File.Exists( iniPath ) ) {
        RbtAuto.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtAuto.Name, iniPath ) ) ;
        ChkNeko.Checked = bool.Parse( ClsIni.GetIniFile( sec, ChkNeko.Name, iniPath ) ) ;
        CmbHolBrace.Text = ClsIni.GetIniFile( sec, CmbHolBrace.Name, iniPath ) ;
        CmbVertBrace_Web.Text = ClsIni.GetIniFile( sec, CmbVertBrace_Web.Name, iniPath ) ;
        CmbVertBrace_Frange.Text = ClsIni.GetIniFile( sec, CmbVertBrace_Frange.Name, iniPath ) ;
        CmbHolJoint_Web.Text = ClsIni.GetIniFile( sec, CmbHolJoint_Web.Name, iniPath ) ;
        CmbHolJoint_Frange.Text = ClsIni.GetIniFile( sec, CmbHolJoint_Frange.Name, iniPath ) ;
        ChkUp.Checked = bool.Parse( ClsIni.GetIniFile( sec, ChkUp.Name, iniPath ) ) ;
        RbtBulmanUp.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtBulmanUp.Name, iniPath ) ) ;
        RbtRikimanUp.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtRikimanUp.Name, iniPath ) ) ;
        CmbUpSize.Text = ClsIni.GetIniFile( sec, CmbUpSize.Name, iniPath ) ;
        ChkBtm.Checked = bool.Parse( ClsIni.GetIniFile( sec, ChkBtm.Name, iniPath ) ) ;
        RbtBulmanBtm.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtBulmanBtm.Name, iniPath ) ) ;
        RbtRikimanBtm.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtRikimanBtm.Name, iniPath ) ) ;
        CmbBtmSize.Text = ClsIni.GetIniFile( sec, CmbBtmSize.Name, iniPath ) ;
        RbtFree.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtFree.Name, iniPath ) ) ;
        RbtNekoFree.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtNekoFree.Name, iniPath ) ) ;
        RbtBulFree.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtBulFree.Name, iniPath ) ) ;
        RbtRikiFree.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtRikiFree.Name, iniPath ) ) ;
        CmbNekoFree.Text = ClsIni.GetIniFile( sec, CmbNekoFree.Name, iniPath ) ;
        CmbBulFree.Text = ClsIni.GetIniFile( sec, CmbBulFree.Name, iniPath ) ;
        CmbRikiFree.Text = ClsIni.GetIniFile( sec, CmbRikiFree.Name, iniPath ) ;
        ChkKiribariAsJoint.Checked = bool.Parse( ClsIni.GetIniFile( sec, ChkKiribariAsJoint.Name, iniPath ) ) ;
      }
    }

    private bool CheckInpurValues()
    {
      List<string> errMsg = new List<string>() ;

      if ( string.IsNullOrEmpty( CmbKoudaiName.Text ) ) {
        errMsg.Add( "構台名を指定してください" ) ;
      }

      if ( RbtAuto.Checked ) {
        if ( ChkNeko.Checked ) {
          if ( string.IsNullOrEmpty( CmbHolBrace.Text ) ) {
            errMsg.Add( "水平ﾌﾞﾚｰｽ用ネコ材を指定してください" ) ;
          }

          if ( string.IsNullOrEmpty( CmbVertBrace_Web.Text ) ) {
            errMsg.Add( "垂直ﾌﾞﾚｰｽ(ｳｪﾌﾞ用)ネコ材を指定してください" ) ;
          }

          if ( string.IsNullOrEmpty( CmbVertBrace_Frange.Text ) ) {
            errMsg.Add( "垂直ﾌﾞﾚｰｽ(フランジ用)ネコ材を指定してください" ) ;
          }

          if ( string.IsNullOrEmpty( CmbHolJoint_Web.Text ) ) {
            errMsg.Add( "水平ﾂﾅｷﾞ(ｳｪﾌﾞ用)用ネコ材を指定してください" ) ;
          }

          if ( string.IsNullOrEmpty( CmbHolJoint_Frange.Text ) ) {
            errMsg.Add( "水平ﾂﾅｷﾞ(フランジ用)用ネコ材を指定してください" ) ;
          }
        }

        if ( ChkUp.Checked ) {
          if ( string.IsNullOrEmpty( CmbUpSize.Text ) ) {
            errMsg.Add( "上部工用の締結補助材を指定してください" ) ;
          }
        }

        if ( ChkBtm.Checked ) {
          if ( string.IsNullOrEmpty( CmbBtmSize.Text ) ) {
            errMsg.Add( "下部工用の締結補助材を指定してください" ) ;
          }
        }
      }
      else {
        if ( RbtNekoFree.Checked && string.IsNullOrEmpty( CmbNekoFree.Text ) ) {
          errMsg.Add( "ネコ材サイズを指定してください" ) ;
        }

        if ( RbtBulFree.Checked && string.IsNullOrEmpty( CmbBulFree.Text ) ) {
          errMsg.Add( "ﾌﾞﾙﾏﾝのタイプﾟを指定してください" ) ;
        }

        if ( RbtRikiFree.Checked && string.IsNullOrEmpty( CmbRikiFree.Text ) ) {
          errMsg.Add( "ﾘｷﾏﾝのタイプﾟを指定してください" ) ;
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