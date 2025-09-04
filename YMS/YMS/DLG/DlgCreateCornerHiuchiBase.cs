using System ;
using System.Collections.Generic ;
using System.ComponentModel ;
using System.Data ;
using System.Drawing ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;
using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using RevitUtil ;
using YMS.Parts ;

namespace YMS.DLG
{
  public partial class DlgCreateCornerHiuchiBase : System.Windows.Forms.Form
  {
    #region 定数

    /// <summary>
    /// Iniファイルのパス
    /// </summary>
    const string IniPath = "ini\\DlgCreateCornerHiuchiBase.ini" ;

    /// <summary>
    /// Iniファイルのセクション名
    /// </summary>
    const string sec = "DlgCreateCornerHiuchiBase" ;

    #endregion

    private bool m_FlgModal = false ;
    private RequestHandler m_Handler ;
    private ExternalEvent m_ExEvent ;
    public ClsCornerHiuchiBase m_ClsCornerHiuchiBase ;

    public DlgCreateCornerHiuchiBase( ClsCornerHiuchiBase cornerHiuchi, ExternalEvent exEvent, RequestHandler handler )
    {
      InitializeComponent() ;
      m_Handler = handler ;
      m_ExEvent = exEvent ;
      m_ClsCornerHiuchiBase = cornerHiuchi ;
      initControl() ;
      SetControl() ;

      chkSyorihouho( false ) ;
    }

    public DlgCreateCornerHiuchiBase( ClsCornerHiuchiBase cornerHiuchi )
    {
      InitializeComponent() ;
      m_FlgModal = true ;
      m_ClsCornerHiuchiBase = cornerHiuchi ;
      initControl() ;
      SetControl() ;
      chkBtn( false ) ;

      chkSyorihouho( false ) ;
    }

    public DlgCreateCornerHiuchiBase( ExternalEvent exEvent, RequestHandler handler )
    {
      InitializeComponent() ;
      m_Handler = handler ;
      m_ExEvent = exEvent ;
      m_ClsCornerHiuchiBase = new ClsCornerHiuchiBase() ;
      initControl() ;
      GetIniData() ;
      chkSyorihouho() ;
    }

    private void DlgCreateCornerHiuchi_Load( object sender, EventArgs e )
    {
    }

    /// <summary>
    /// 全長を更新
    /// </summary>
    public void UpdateTotalLength()
    {
      if ( m_ClsCornerHiuchiBase.m_HaraAngle > 0 ) {
        SetData() ;
        txtTotalLength.Text = m_ClsCornerHiuchiBase.GetTotalLength( m_ClsCornerHiuchiBase.m_HaraAngle ).ToString() ;
      }
      else {
        txtTotalLength.Text = string.Empty ;
      }
    }

    public void initControl()
    {
      initComboBox() ;
      chkKose() ;
      chkHiuchiDouble() ;

      // Revitウィンドウを親にしてアクティブ制御（Revitにキーを奪われにくくする）
      ClsYMSUtil.SetRevitAsOwner( this ) ;
    }

    public void initComboBox()
    {
      // 鋼材タイプの処理
      InitComboBoxHelper( cmbSteelType,
        () => Master.ClsYamadomeCsv.GetTypeList( bHigh: true, bMega: false, bTwin: false ) ) ;

      // 端部部品(1)の処理
      InitComboBoxHelper( cmbTanbuParts1, () => Master.ClsHiuchiCsv.GetTypeListCornerHiuchi() ) ;

      // 端部部品(2)の処理
      InitComboBoxHelper( cmbTanbuParts2, () => Master.ClsHiuchiCsv.GetTypeListCornerHiuchi() ) ;

      initComboBoxJackSizeSingle() ;
      initComboBoxJackSizeDouble_U() ;
      initComboBoxJackSizeDouble_D() ;
    }

    /// <summary>
    ///  ComboBoxを初期化するためのヘルパーメソッド
    /// </summary>
    /// <param name="comboBox"></param>
    /// <param name="getListFunc"></param>
    private void InitComboBoxHelper( System.Windows.Forms.ComboBox comboBox, Func<List<string>> getListFunc )
    {
      if ( comboBox == null ) {
        // comboBox が null の場合は処理を中止するか、適切な例外処理を行う
        return ;
      }

      string selectedText = comboBox.Text ;
      comboBox.Items.Clear() ;

      List<string> lstStr = getListFunc() ;

      if ( lstStr != null ) {
        foreach ( string str in lstStr ) {
          comboBox.Items.Add( str ) ;
        }

        if ( lstStr.Contains( selectedText ) ) {
          comboBox.Text = selectedText ;
        }
        else if ( comboBox.Items.Count > 0 ) {
          comboBox.SelectedIndex = 0 ;
        }
        else {
          comboBox.Text = string.Empty ;
        }
      }
      else {
        comboBox.Text = string.Empty ;
      }
    }

    public void initComboBoxSize()
    {
      if ( cmbSteelSize == null || cmbSteelType == null ) {
        return ;
      }

      string selectedText = cmbSteelSize.Text ;
      cmbSteelSize.Items.Clear() ;

      List<string> lstStr = Master.ClsYamadomeCsv.GetSizeList( cmbSteelType.Text ) ;

      if ( lstStr != null ) {
        foreach ( string str in lstStr ) {
          cmbSteelSize.Items.Add( str ) ;
        }

        if ( lstStr.Contains( selectedText ) ) {
          cmbSteelSize.Text = selectedText ;
        }
        else if ( cmbSteelSize.Items.Count > 0 ) {
          cmbSteelSize.SelectedIndex = 0 ;
        }
        else {
          cmbSteelSize.Text = string.Empty ;
        }
      }
      else {
        cmbSteelSize.Text = string.Empty ;
      }
    }

    public void initComboBoxSize1()
    {
      if ( cmbHiuchiUkePeaceSize1 == null || cmbTanbuParts1 == null ) {
        return ;
      }

      string selectedText = cmbHiuchiUkePeaceSize1.Text ;
      cmbHiuchiUkePeaceSize1.Items.Clear() ;

      List<string> lstStr = Master.ClsHiuchiCsv.GetSizeListCornerHiuchi( cmbTanbuParts1.Text ) ;

      if ( lstStr != null ) {
        foreach ( string str in lstStr ) {
          cmbHiuchiUkePeaceSize1.Items.Add( str ) ;
        }

        if ( lstStr.Contains( selectedText ) ) {
          cmbHiuchiUkePeaceSize1.Text = selectedText ;
        }
        else if ( cmbHiuchiUkePeaceSize1.Items.Count > 0 ) {
          cmbHiuchiUkePeaceSize1.SelectedIndex = 0 ;
        }
        else {
          cmbHiuchiUkePeaceSize1.Text = string.Empty ;
        }
      }
      else {
        cmbHiuchiUkePeaceSize1.Text = string.Empty ;
      }
    }

    public void initComboBoxSize2()
    {
      if ( cmbHiuchiUkePeaceSize2 == null || cmbTanbuParts2 == null ) {
        return ;
      }

      string selectedText = cmbHiuchiUkePeaceSize2.Text ;
      cmbHiuchiUkePeaceSize2.Items.Clear() ;

      List<string> lstStr = Master.ClsHiuchiCsv.GetSizeListCornerHiuchi( cmbTanbuParts2.Text ) ;

      if ( lstStr != null ) {
        foreach ( string str in lstStr ) {
          cmbHiuchiUkePeaceSize2.Items.Add( str ) ;
        }

        if ( lstStr.Contains( selectedText ) ) {
          cmbHiuchiUkePeaceSize2.Text = selectedText ;
        }
        else if ( cmbHiuchiUkePeaceSize2.Items.Count > 0 ) {
          cmbHiuchiUkePeaceSize2.SelectedIndex = 0 ;
        }
        else {
          cmbHiuchiUkePeaceSize2.Text = string.Empty ;
        }
      }
      else {
        cmbHiuchiUkePeaceSize2.Text = string.Empty ;
      }
    }

    private void initComboBoxJackSizeSingle()
    {
      //string bk = cmbJackType_Single.Text;
      //var lstStr = Master.ClsJackCsv.GetTypeList();

      //cmbJackType_Single.Items.Clear();
      //cmbJackType_Single.Items.AddRange(lstStr.ToArray());

      //if (lstStr.Contains(bk))
      //{
      //    cmbJackType_Single.Text = bk;
      //}
      //else
      //{
      //    if (lstStr.Count > 0)
      //    {
      //        cmbJackType_Single.SelectedIndex = 0;
      //    }
      //}
    }

    private void initComboBoxJackSizeDouble_U()
    {
      //string bk = cmbJackType_Double_U.Text;
      //var lstStr = Master.ClsJackCsv.GetTypeList();

      //cmbJackType_Double_U.Items.Clear();
      //cmbJackType_Double_U.Items.AddRange(lstStr.ToArray());

      //if (lstStr.Contains(bk))
      //{
      //    cmbJackType_Double_U.Text = bk;
      //}
      //else
      //{
      //    if (lstStr.Count > 0)
      //    {
      //        cmbJackType_Double_U.SelectedIndex = 0;
      //    }
      //}
    }

    private void initComboBoxJackSizeDouble_D()
    {
      //string bk = cmbJackType_Double_D.Text;
      //var lstStr = Master.ClsJackCsv.GetTypeList();

      //cmbJackType_Double_D.Items.Clear();
      //cmbJackType_Double_D.Items.AddRange(lstStr.ToArray());

      //if (lstStr.Contains(bk))
      //{
      //    cmbJackType_Double_D.Text = bk;
      //}
      //else
      //{
      //    if (lstStr.Count > 0)
      //    {
      //        cmbJackType_Double_D.SelectedIndex = 0;
      //    }
      //}
    }

    private void btnOK_Click( object sender, EventArgs e )
    {
      if ( ! CheckInput() )
        return ;

      SetData() ;
      SetIniData() ;

      ClsCornerHiuchiBase cornerHiuchi = m_ClsCornerHiuchiBase ;
      cornerHiuchi.m_cornerHiuchiId = CornerHiuchiId.OK ;
      m_ClsCornerHiuchiBase = cornerHiuchi ;
      ClsCornerHiuchiBase.m_cornerHiuchi = m_ClsCornerHiuchiBase ;

      if ( m_FlgModal ) {
        DialogResult = DialogResult.OK ;
        Close() ;
      }
      else {
        MakeRequest( RequestId.CreateCornerHiuchi ) ;
      }
    }

    private void btnCancel_Click( object sender, EventArgs e )
    {
      ClsCornerHiuchiBase cornerHiuchi = m_ClsCornerHiuchiBase ;
      cornerHiuchi.m_cornerHiuchiId = CornerHiuchiId.cancel ;
      m_ClsCornerHiuchiBase = cornerHiuchi ;
      ClsCornerHiuchiBase.m_cornerHiuchi = m_ClsCornerHiuchiBase ;

      if ( m_FlgModal ) {
        DialogResult = DialogResult.Cancel ;
        Close() ;
      }
      else {
        MakeRequest( RequestId.CreateCornerHiuchi ) ;
      }
    }

    private void btnCheck_Click( object sender, EventArgs e )
    {
      SetData() ;

      ClsCornerHiuchiBase cornerHiuchi = m_ClsCornerHiuchiBase ;
      cornerHiuchi.m_cornerHiuchiId = CornerHiuchiId.review ;
      m_ClsCornerHiuchiBase = cornerHiuchi ;
      ClsCornerHiuchiBase.m_cornerHiuchi = m_ClsCornerHiuchiBase ;


      MakeRequest( RequestId.CreateCornerHiuchi ) ;
    }

    private void btnCheckReset_Click( object sender, EventArgs e )
    {
      ClsCornerHiuchiBase cornerHiuchi = m_ClsCornerHiuchiBase ;
      cornerHiuchi.m_HaraokoshiBaseID1 = null ;
      cornerHiuchi.m_HaraokoshiBaseID2 = null ;
      cornerHiuchi.m_cornerHiuchiId = CornerHiuchiId.reset ;
      m_ClsCornerHiuchiBase = cornerHiuchi ;
      ClsCornerHiuchiBase.m_cornerHiuchi = m_ClsCornerHiuchiBase ;

      MakeRequest( RequestId.CreateCornerHiuchi ) ;
    }

    private void DlgCreateCornerHiuchiBase_FormClosed( object sender, FormClosedEventArgs e )
    {
      if ( ! m_FlgModal ) {
        //初期化処理
        MakeRequest( RequestId.End ) ;
        m_ExEvent.Dispose() ;
        m_ExEvent = null ;
        m_Handler = null ;
      }
    }

    private void MakeRequest( RequestId request )
    {
      if ( m_FlgModal ) {
        return ;
      }

      m_Handler.Request.Make( request ) ;
      m_ExEvent.Raise() ;
      DozeOff() ;
    }

    private void MakeRequest2( RequestId request )
    {
      m_Handler.Request.Make( request ) ;
      m_ExEvent.Raise() ;
      DozeOff( false ) ;
    }

    /// <summary>
    ///   DozeOff -> disable all controls (but the Exit button)
    /// </summary>
    ///
    private void DozeOff( bool flg = true )
    {
      EnableCommands( flg ) ;
    }

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

      if ( ! status ) {
      }
    }

    private void txtHiuchiLengthSingleL_TextChanged( object sender, EventArgs e )
    {
      UpdateTotalLength() ;
    }

    /// <summary>
    /// コントロールの値をメンバクラスに保持
    /// </summary>
    private void SetData()
    {
      ClsCornerHiuchiBase clsCHB = m_ClsCornerHiuchiBase ;

      // 処理方法
      if ( rbnShoriHohoAuto.Checked )
        clsCHB.m_ShoriType = ClsCornerHiuchiBase.ShoriType.Auto ;
      else if ( rbnShoriHohoManual.Checked )
        clsCHB.m_ShoriType = ClsCornerHiuchiBase.ShoriType.Manual ;
      else if ( rbnShoriHohoReplace.Checked )
        clsCHB.m_ShoriType = ClsCornerHiuchiBase.ShoriType.Replace ;
      else
        clsCHB.m_ShoriType = ClsCornerHiuchiBase.ShoriType.Change ;

      // 構成
      if ( rbnKoseDouble.Checked )
        clsCHB.m_Kousei = ClsCornerHiuchiBase.Kousei.Double ;
      else
        clsCHB.m_Kousei = ClsCornerHiuchiBase.Kousei.Single ;

      // 火打角度
      if ( rbnHiuchiAngleToubun.Checked )
        clsCHB.m_HiuchiAngle = ClsCornerHiuchiBase.HiuchiAngle.Toubun ;
      else if ( rbnHiuchiAngleNini.Checked )
        clsCHB.m_HiuchiAngle = ClsCornerHiuchiBase.HiuchiAngle.Nini ;
      else {
        rbnHiuchiAngleToubun.Enabled = false ;
        rbnHiuchiAngleNini.Enabled = false ;
      }

      clsCHB.m_angle = ClsCommonUtils.ChangeStrToDbl( txtHiuchiAngleNini.Text ) ;

      // 鋼材タイプや鋼材サイズ
      clsCHB.m_SteelType = cmbSteelType.Text ;
      clsCHB.m_SteelSize = cmbSteelSize.Text ;
      clsCHB.m_TanbuParts1 = cmbTanbuParts1.Text ;
      clsCHB.m_HiuchiUkePieceSize1 = cmbHiuchiUkePeaceSize1.Text ;
      clsCHB.m_TanbuParts2 = cmbTanbuParts2.Text ;
      clsCHB.m_HiuchiUkePieceSize2 = cmbHiuchiUkePeaceSize2.Text ;

      // 火打全長
      clsCHB.m_HiuchiTotalLength = ClsCommonUtils.ChangeStrToDbl( txtTotalLength.Text ) ;

      // 火打長さ(シングル)
      clsCHB.m_HiuchiLengthSingleL = ClsCommonUtils.ChangeStrToDbl( txtHiuchiLengthSingleL.Text ) ;
      //clsCHB.m_JackTypeSingle = cmbJackType_Single.Text;

      // 火打長さ(ダブル上)
      clsCHB.m_HiuchiLengthDoubleUpperL1 = ClsCommonUtils.ChangeStrToDbl( txtHiuchiLengthDoubleL1.Text ) ;
      //clsCHB.m_JackTypeDoubleUpper = cmbJackType_Double_U.Text;

      // 火打長さ(ダブル下)
      clsCHB.m_HiuchiLengthDoubleUnderL2 = ClsCommonUtils.ChangeStrToDbl( txtHiuchiLengthDoubleL2.Text ) ;
      //clsCHB.m_JackTypeDoubleLower = cmbJackType_Double_D.Text;

      // 上下火打ずれ量a
      clsCHB.m_HiuchiZureryo = ClsCommonUtils.ChangeStrToDbl( txtHiuchiZureryo.Text ) ;
    }

    /// <summary>
    /// コントロールの値をメンバクラスに保持
    /// </summary>
    private void SetControl()
    {
      ClsCornerHiuchiBase clsCHB = m_ClsCornerHiuchiBase ;

      if ( clsCHB.m_ShoriType == ClsCornerHiuchiBase.ShoriType.Auto )
        rbnShoriHohoAuto.Checked = true ;
      else if ( clsCHB.m_ShoriType == ClsCornerHiuchiBase.ShoriType.Manual )
        rbnShoriHohoManual.Checked = true ;
      else if ( clsCHB.m_ShoriType == ClsCornerHiuchiBase.ShoriType.Replace )
        rbnShoriHohoReplace.Checked = true ;
      else {
        rbnShoriHohoAuto.Checked = false ;
        rbnShoriHohoManual.Checked = false ;
        rbnShoriHohoReplace.Checked = false ;
      }

      if ( clsCHB.m_Kousei == ClsCornerHiuchiBase.Kousei.Double )
        rbnKoseDouble.Checked = true ;
      else
        rbnKoseSingle.Checked = true ;

      if ( clsCHB.m_HiuchiAngle == ClsCornerHiuchiBase.HiuchiAngle.Toubun )
        rbnHiuchiAngleToubun.Checked = true ;
      else if ( clsCHB.m_HiuchiAngle == ClsCornerHiuchiBase.HiuchiAngle.Nini )
        rbnHiuchiAngleNini.Checked = true ;
      else {
        rbnHiuchiAngleToubun.Checked = false ;
        rbnHiuchiAngleNini.Checked = false ;
      }

      txtHiuchiAngleNini.Text = clsCHB.m_angle.ToString() ;

      cmbSteelType.Text = clsCHB.m_SteelType ;

      cmbSteelSize.Text = clsCHB.m_SteelSize ;

      cmbTanbuParts1.Text = clsCHB.m_TanbuParts1 ;
      cmbHiuchiUkePeaceSize1.Text = clsCHB.m_HiuchiUkePieceSize1 ;

      cmbTanbuParts2.Text = clsCHB.m_TanbuParts2 ;
      cmbHiuchiUkePeaceSize2.Text = clsCHB.m_HiuchiUkePieceSize2 ;

      txtTotalLength.Text = clsCHB.m_HiuchiTotalLength.ToString() ;

      txtHiuchiLengthSingleL.Text = clsCHB.m_HiuchiLengthSingleL.ToString() ;
      txtHiuchiLengthDoubleL1.Text = clsCHB.m_HiuchiLengthDoubleUpperL1.ToString() ;
      txtHiuchiLengthDoubleL2.Text = clsCHB.m_HiuchiLengthDoubleUnderL2.ToString() ;
      txtHiuchiZureryo.Text = clsCHB.m_HiuchiZureryo.ToString() ;
    }

    /// <summary>
    /// Iniの情報をコントロールにセット
    /// </summary>
    private void GetIniData()
    {
      try {
        string iniPath = ClsIni.GetIniFilePath( IniPath ) ;
        rbnShoriHohoAuto.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnShoriHohoAuto.Name, iniPath ) ) ;
        rbnShoriHohoManual.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnShoriHohoManual.Name, iniPath ) ) ;
        rbnShoriHohoReplace.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnShoriHohoReplace.Name, iniPath ) ) ;

        rbnKoseSingle.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnKoseSingle.Name, iniPath ) ) ;
        rbnKoseDouble.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnKoseDouble.Name, iniPath ) ) ;
        rbnHiuchiAngleToubun.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnHiuchiAngleToubun.Name, iniPath ) ) ;
        rbnHiuchiAngleNini.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnHiuchiAngleNini.Name, iniPath ) ) ;
        txtHiuchiAngleNini.Text = ClsIni.GetIniFile( sec, txtHiuchiAngleNini.Name, iniPath ) ;
        cmbSteelType.Text = ClsIni.GetIniFile( sec, cmbSteelType.Name, iniPath ) ;
        cmbSteelSize.Text = ClsIni.GetIniFile( sec, cmbSteelSize.Name, iniPath ) ;
        cmbTanbuParts1.Text = ClsIni.GetIniFile( sec, cmbTanbuParts1.Name, iniPath ) ;
        cmbHiuchiUkePeaceSize1.Text = ClsIni.GetIniFile( sec, cmbHiuchiUkePeaceSize1.Name, iniPath ) ;
        cmbTanbuParts2.Text = ClsIni.GetIniFile( sec, cmbTanbuParts2.Name, iniPath ) ;
        cmbHiuchiUkePeaceSize2.Text = ClsIni.GetIniFile( sec, cmbHiuchiUkePeaceSize2.Name, iniPath ) ;

        txtHiuchiLengthSingleL.Text = ClsIni.GetIniFile( sec, txtHiuchiLengthSingleL.Name, iniPath ) ;
        //cmbJackType_Single.Text      = ClsIni.GetIniFile(sec, cmbJackType_Single.Name, iniPath);
        txtHiuchiLengthDoubleL1.Text = ClsIni.GetIniFile( sec, txtHiuchiLengthDoubleL1.Name, iniPath ) ;
        //cmbJackType_Double_U.Text    = ClsIni.GetIniFile(sec, cmbJackType_Double_U.Name, iniPath);
        txtHiuchiLengthDoubleL2.Text = ClsIni.GetIniFile( sec, txtHiuchiLengthDoubleL2.Name, iniPath ) ;
        //cmbJackType_Double_D.Text    = ClsIni.GetIniFile(sec, cmbJackType_Double_D.Name, iniPath);
        txtHiuchiZureryo.Text = ClsIni.GetIniFile( sec, txtHiuchiZureryo.Name, iniPath ) ;
      }
      catch {
        //MessageBox.Show("前回値の取得に失敗しました");
      }
    }

    /// <summary>
    /// コントロールの情報をIniにセット
    /// </summary>
    private void SetIniData()
    {
      string iniPath = ClsIni.GetIniFilePath( IniPath ) ;
      ClsCornerHiuchiBase cornerHiuchi = m_ClsCornerHiuchiBase ;

      if ( cornerHiuchi.m_ShoriType != ClsCornerHiuchiBase.ShoriType.Change ) {
        //変更でダイアログを起動した場合処理方法は保存しない
        ClsIni.WritePrivateProfileString( sec, rbnShoriHohoAuto.Name, rbnShoriHohoAuto.Checked.ToString(), iniPath ) ;
        ClsIni.WritePrivateProfileString( sec, rbnShoriHohoManual.Name, rbnShoriHohoManual.Checked.ToString(),
          iniPath ) ;
        ClsIni.WritePrivateProfileString( sec, rbnShoriHohoReplace.Name, rbnShoriHohoReplace.Checked.ToString(),
          iniPath ) ;
      }

      ClsIni.WritePrivateProfileString( sec, rbnKoseSingle.Name, rbnKoseSingle.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, rbnKoseDouble.Name, rbnKoseDouble.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, rbnHiuchiAngleToubun.Name, rbnHiuchiAngleToubun.Checked.ToString(),
        iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, rbnHiuchiAngleNini.Name, rbnHiuchiAngleNini.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, txtHiuchiAngleNini.Name, txtHiuchiAngleNini.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, cmbSteelType.Name, cmbSteelType.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, cmbSteelSize.Name, cmbSteelSize.Text, iniPath ) ;

      ClsIni.WritePrivateProfileString( sec, cmbTanbuParts1.Name, cmbTanbuParts1.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, cmbHiuchiUkePeaceSize1.Name, cmbHiuchiUkePeaceSize1.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, cmbTanbuParts2.Name, cmbTanbuParts2.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, cmbHiuchiUkePeaceSize2.Name, cmbHiuchiUkePeaceSize2.Text, iniPath ) ;

      ClsIni.WritePrivateProfileString( sec, txtHiuchiLengthSingleL.Name, txtHiuchiLengthSingleL.Text, iniPath ) ;
      //ClsIni.WritePrivateProfileString(sec, cmbJackType_Single.Name, cmbJackType_Single.Text, iniPath);
      ClsIni.WritePrivateProfileString( sec, txtHiuchiLengthDoubleL1.Name, txtHiuchiLengthDoubleL1.Text, iniPath ) ;
      //ClsIni.WritePrivateProfileString(sec, cmbJackType_Double_U.Name, cmbJackType_Double_U.Text, iniPath);
      ClsIni.WritePrivateProfileString( sec, txtHiuchiLengthDoubleL2.Name, txtHiuchiLengthDoubleL2.Text, iniPath ) ;
      //ClsIni.WritePrivateProfileString(sec, cmbJackType_Double_D.Name, cmbJackType_Double_D.Text, iniPath);
      ClsIni.WritePrivateProfileString( sec, txtHiuchiZureryo.Name, txtHiuchiZureryo.Text, iniPath ) ;
    }

    private void rbnShoriHohoAuto_CheckedChanged( object sender, EventArgs e )
    {
      if ( rbnShoriHohoAuto.Checked ) {
        if ( m_ClsCornerHiuchiBase != null ) {
          ClsCornerHiuchiBase cornerHiuchi = m_ClsCornerHiuchiBase ;
          cornerHiuchi.m_HaraokoshiBaseID1 = null ;
          cornerHiuchi.m_HaraokoshiBaseID2 = null ;
          cornerHiuchi.m_cornerHiuchiId = CornerHiuchiId.reset ;
          m_ClsCornerHiuchiBase = cornerHiuchi ;
          ClsCornerHiuchiBase.m_cornerHiuchi = m_ClsCornerHiuchiBase ;

          MakeRequest( RequestId.CreateCornerHiuchi ) ;
        }
      }
    }

    private void rbnShoriHohoManual_CheckedChanged( object sender, EventArgs e )
    {
      if ( rbnShoriHohoManual.Checked ) {
        if ( m_ClsCornerHiuchiBase != null ) {
          ClsCornerHiuchiBase cornerHiuchi = m_ClsCornerHiuchiBase ;
          cornerHiuchi.m_HaraokoshiBaseID1 = null ;
          cornerHiuchi.m_HaraokoshiBaseID2 = null ;
          cornerHiuchi.m_cornerHiuchiId = CornerHiuchiId.reset ;
          m_ClsCornerHiuchiBase = cornerHiuchi ;
          ClsCornerHiuchiBase.m_cornerHiuchi = m_ClsCornerHiuchiBase ;

          MakeRequest( RequestId.CreateCornerHiuchi ) ;
        }
      }
    }

    private void rbnShoriHohoReplace_CheckedChanged( object sender, EventArgs e )
    {
      if ( rbnShoriHohoReplace.Checked ) {
        if ( m_ClsCornerHiuchiBase != null ) {
          ClsCornerHiuchiBase cornerHiuchi = m_ClsCornerHiuchiBase ;
          cornerHiuchi.m_HaraokoshiBaseID1 = null ;
          cornerHiuchi.m_HaraokoshiBaseID2 = null ;
          cornerHiuchi.m_cornerHiuchiId = CornerHiuchiId.reset ;
          m_ClsCornerHiuchiBase = cornerHiuchi ;
          ClsCornerHiuchiBase.m_cornerHiuchi = m_ClsCornerHiuchiBase ;
        }

        MakeRequest( RequestId.CreateCornerHiuchi ) ;
      }
    }

    private void rbnKoseSingle_CheckedChanged( object sender, EventArgs e )
    {
      chkKose() ;
      UpdateTotalLength() ;
    }

    private void rbnKoseDouble_CheckedChanged( object sender, EventArgs e )
    {
      chkKose() ;
      UpdateTotalLength() ;
    }

    private void chkKose()
    {
      if ( rbnKoseDouble.Checked == true ) {
        txtHiuchiLengthSingleL.Enabled = false ; //火打長さ(シングル)
        //cmbJackType_Single.Enabled = false;     //ジャッキ(シングル)

        txtHiuchiLengthDoubleL1.Enabled = true ; //火打長さ(ダブル上)
        //cmbJackType_Double_U.Enabled = true;      //ジャッキ(ダブル上)
        txtHiuchiLengthDoubleL2.Enabled = true ; //火打長さ(ダブル下)
        //cmbJackType_Double_D.Enabled = true;      //ジャッキ(ダブル下)

        txtHiuchiZureryo.Enabled = true ; //上下火打ずれ量a
      }
      else {
        txtHiuchiLengthSingleL.Enabled = true ; //火打長さ(シングル)
        //cmbJackType_Single.Enabled = true;      //ジャッキ(シングル)

        txtHiuchiLengthDoubleL1.Enabled = false ; //火打長さ(ダブル上)
        //cmbJackType_Double_U.Enabled = false;     //ジャッキ(ダブル上)
        txtHiuchiLengthDoubleL2.Enabled = false ; //火打長さ(ダブル下)
        //cmbJackType_Double_D.Enabled = false;     //ジャッキ(ダブル下)

        txtHiuchiZureryo.Enabled = false ; //上下火打ずれ量a
      }
    }

    private void rbnHiuchiAngleToubun_CheckedChanged( object sender, EventArgs e )
    {
      chkHiuchiDouble() ;
      UpdateTotalLength() ;
    }

    private void rbnHiuchiAngleNini_CheckedChanged( object sender, EventArgs e )
    {
      chkHiuchiDouble() ;
      UpdateTotalLength() ;
    }

    private void chkHiuchiDouble()
    {
      if ( rbnHiuchiAngleNini.Checked == true ) {
        txtHiuchiAngleNini.Enabled = true ;
      }
      else {
        txtHiuchiAngleNini.Enabled = false ;
      }
    }

    private void cmbTanbuParts1_SelectedIndexChanged( object sender, EventArgs e )
    {
      string selectedItem = cmbTanbuParts1.SelectedItem.ToString() ;
      if ( selectedItem != "なし" && cmbTanbuParts1.Enabled == true ) {
        cmbHiuchiUkePeaceSize1.Enabled = true ;
      }
      else {
        cmbHiuchiUkePeaceSize1.Enabled = false ;
      }

      initComboBoxSize1() ;
    }

    private void cmbTanbuParts2_SelectedIndexChanged( object sender, EventArgs e )
    {
      string selectedItem = cmbTanbuParts2.SelectedItem.ToString() ;
      if ( selectedItem != "なし" && cmbTanbuParts2.Enabled == true ) {
        cmbHiuchiUkePeaceSize2.Enabled = true ;
      }
      else {
        cmbHiuchiUkePeaceSize2.Enabled = false ;
      }

      initComboBoxSize2() ;
    }

    private void cmbHiuchiUkePeaceSize1_SelectedIndexChanged( object sender, EventArgs e )
    {
      UpdateTotalLength() ;
    }

    private void cmbHiuchiUkePeaceSize2_SelectedIndexChanged( object sender, EventArgs e )
    {
      UpdateTotalLength() ;
    }

    /// <summary>
    /// 入力チェック
    /// </summary>
    /// <returns></returns>
    public bool CheckInput()
    {
      if ( ! ClsCommonUtils.ChkContorolTextDouble( txtHiuchiAngleNini, "任意配置角度" ) )
        return false ;
      if ( ! ClsCommonUtils.ChkContorolTextDouble( txtHiuchiLengthSingleL, "火打長さ(シングル)L", bMinus: true ) )
        return false ;
      if ( ! ClsCommonUtils.ChkContorolTextDouble( txtHiuchiLengthDoubleL1, "火打長さ(ダブル上)L1", bMinus: true ) )
        return false ;
      if ( ! ClsCommonUtils.ChkContorolTextDouble( txtHiuchiLengthDoubleL2, "火打長さ(ダブル下)L2", bMinus: true ) )
        return false ;
      if ( ! ClsCommonUtils.ChkContorolTextDouble( txtHiuchiZureryo, "上下火打ずれ量a" ) )
        return false ;

      return true ;
    }

    private void txtHiuchiLengthDoubleL1_TextChanged( object sender, EventArgs e )
    {
      UpdateTotalLength() ;
    }

    private void txtHiuchiLengthDoubleL2_TextChanged( object sender, EventArgs e )
    {
      UpdateTotalLength() ;
    }

    private void txtHiuchiZureryo_TextChanged( object sender, EventArgs e )
    {
      UpdateTotalLength() ;
    }

    private void chkSyorihouho( bool bFlag = true )
    {
      rbnShoriHohoAuto.Enabled = bFlag ;
      rbnShoriHohoManual.Enabled = bFlag ;
      rbnShoriHohoReplace.Enabled = bFlag ;
    }

    private void chkBtn( bool bFlag = true )
    {
      btnCheck.Enabled = bFlag ;
      btnCheckReset.Enabled = bFlag ;
    }

    private void txtHiuchiAngleNini_TextChanged( object sender, EventArgs e )
    {
      UpdateTotalLength() ;
    }

    private void cmbSteelSize_SelectedIndexChanged( object sender, EventArgs e )
    {
      UpdateTotalLength() ;
    }

    private void cmbSteelType_SelectedIndexChanged( object sender, EventArgs e )
    {
      initComboBoxSize() ;
    }
  }
}