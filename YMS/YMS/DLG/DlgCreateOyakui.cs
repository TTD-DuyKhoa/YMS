using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.ComponentModel ;
using System.Data ;
using System.Drawing ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;
using YMS.Parts ;
using static YMS.ClsCASE ;

namespace YMS.DLG
{
  public partial class DlgCreateOyakui : Form
  {
    #region 定数

    /// <summary>
    /// Iniファイルのパス
    /// </summary>
    const string IniPath = "ini\\DlgCreateOyakui.ini" ;

    /// <summary>
    /// Iniファイルのセクション名
    /// </summary>
    const string sec = "DlgOyakui" ;

    #endregion

    #region メンバー

    public ClsOyakui m_ClsOyakui ;
    private bool m_ChangeDGVKuiLength = false ;
    private ClsCASE m_caseInfo ;

    #endregion

    public DlgCreateOyakui( Autodesk.Revit.DB.Document doc, ClsOyakui clsOyakui )
    {
      InitializeComponent() ;

      m_ClsOyakui = clsOyakui ;
      m_caseInfo = new ClsCASE( ClsCASE.enKabeshu.oyagui, doc ) ;
      initComboBox() ;
      SetControl() ;
    }

    public DlgCreateOyakui( Autodesk.Revit.DB.Document doc )
    {
      InitializeComponent() ;

      m_ClsOyakui = new ClsOyakui() ;
      m_caseInfo = new ClsCASE( ClsCASE.enKabeshu.oyagui, doc ) ;
      initComboBox() ;
      GetIniData() ;
    }

    public void initComboBox()
    {
      string bk = string.Empty ;
      List<string> lstStr = new List<string>() ;

      //処理タイプ・残置
      txtPlateCountFlangeInSide.Text = 2.ToString() ;
      cmbShoriType.Items.AddRange( ClsGlobal.m_syorihouhou5.ToArray() ) ;
      cmbZanti.Items.AddRange( ClsGlobal.m_zanti.ToArray() ) ;
      cmbBoltSizeFlangeSide.Items.AddRange( ClsGlobal.m_boltsize.ToArray() ) ;
      cmbBoltSizeWebSide.Items.AddRange( ClsGlobal.m_boltsize.ToArray() ) ;
      cmbYokoyaitaType.Items.AddRange( ClsGlobal.m_yokoyaitaytype.ToArray() ) ;

      initComboBoxBoltType() ;

      //鋼材タイプ
      bk = cmbKouzaiType.Text ;
      cmbKouzaiType.Items.Clear() ;
      lstStr = Master.ClsHBeamCsv.GetTypeList() ;
      foreach ( string str in lstStr ) {
        cmbKouzaiType.Items.Add( str ) ;
      }

      if ( lstStr.Contains( bk ) ) cmbKouzaiType.Text = bk ;
      else cmbKouzaiType.SelectedIndex = 0 ;
    }

    public void initComboBoxSteelSize()
    {
      string bk = string.Empty ;
      List<string> lstStr = new List<string>() ;

      //鋼材サイズ
      bk = cmbKouzaiSize.Text ;
      cmbKouzaiSize.Items.Clear() ;
      lstStr = Master.ClsHBeamCsv.GetSizeList( cmbKouzaiType.Text ) ;
      foreach ( string str in lstStr ) {
        cmbKouzaiSize.Items.Add( str ) ;
      }

      if ( lstStr.Contains( bk ) ) cmbKouzaiSize.Text = bk ;
      else cmbKouzaiSize.SelectedIndex = 0 ;

      //横矢板タイプ
      bk = cmbYokoyaitaType.Text ;
      cmbYokoyaitaType.Items.Clear() ;
      lstStr = new List<string>() { Master.ClsKouyaitaCsv.TypeKiyaita } ;
      foreach ( string str in lstStr ) {
        cmbYokoyaitaType.Items.Add( str ) ;
      }

      if ( lstStr.Contains( bk ) ) cmbYokoyaitaType.Text = bk ;
      else cmbYokoyaitaType.SelectedIndex = 0 ;

      //横矢板サイズ
      bk = cmbYokoyaitaSize.Text ;
      cmbYokoyaitaSize.Items.Clear() ;
      lstStr = Master.ClsYokoyaitaCSV.GetSizeList( cmbYokoyaitaType.Text ) ;
      foreach ( string str in lstStr ) {
        cmbYokoyaitaSize.Items.Add( str ) ;
      }

      if ( lstStr.Contains( bk ) ) cmbYokoyaitaSize.Text = bk ;
      else cmbYokoyaitaSize.SelectedIndex = 0 ;

      initOtherControl() ;
    }

    public void initOtherControl()
    {
      string bk = string.Empty ;
      List<string> lstStr = new List<string>() ;
      List<string> lstStr2 = new List<string>() ;
      List<string> lstStrW = new List<string>() ;
      List<string> lstStrW2 = new List<string>() ;

      if ( rbnKoteiTypeBolt.Checked ) {
        lstStr = Master.ClsBoltCsv.GetSizeList( cmbBoltTypeF.Text ) ;
        lstStrW = Master.ClsBoltCsv.GetSizeList( cmbBoltTypeW.Text ) ;
      }
      else {
        lstStr = new List<string>() { string.Empty } ;
        lstStrW = new List<string>() { string.Empty } ;
      }

      cmbBoltTypeF.Enabled = rbnKoteiTypeBolt.Checked ;
      cmbBoltSizeFlangeSide.Enabled = rbnKoteiTypeBolt.Checked ;
      txtBoltCountFlangeSide.Enabled = rbnKoteiTypeBolt.Checked ;
      cmbBoltTypeW.Enabled = rbnKoteiTypeBolt.Checked ;
      cmbBoltSizeWebSide.Enabled = rbnKoteiTypeBolt.Checked ;
      txtBoltCountWebSide.Enabled = rbnKoteiTypeBolt.Checked ;
      if ( rbnKoteiTypeBolt2.Checked ) {
        lstStr2 = Master.ClsBoltCsv.GetSizeList( cmbBoltTypeF2.Text ) ;
        lstStrW2 = Master.ClsBoltCsv.GetSizeList( cmbBoltTypeW2.Text ) ;
      }
      else {
        lstStr2 = new List<string>() { string.Empty } ;
        lstStrW2 = new List<string>() { string.Empty } ;
      }

      cmbBoltTypeF2.Enabled = rbnKoteiTypeBolt2.Checked ;
      cmbBoltSizeFlangeSide2.Enabled = rbnKoteiTypeBolt2.Checked ;
      txtBoltCountFlangeSide2.Enabled = rbnKoteiTypeBolt2.Checked ;
      cmbBoltTypeW2.Enabled = rbnKoteiTypeBolt2.Checked ;
      cmbBoltSizeWebSide2.Enabled = rbnKoteiTypeBolt2.Checked ;
      txtBoltCountWebSide2.Enabled = rbnKoteiTypeBolt2.Checked ;

      //ボルト1-1
      bk = cmbBoltSizeFlangeSide.Text ;
      cmbBoltSizeFlangeSide.Items.Clear() ;
      foreach ( string str in lstStr ) {
        cmbBoltSizeFlangeSide.Items.Add( str ) ;
      }

      if ( ! string.IsNullOrWhiteSpace( cmbBoltTypeF.Text ) ) {
        if ( lstStr.Contains( bk ) ) cmbBoltSizeFlangeSide.Text = bk ;
        else cmbBoltSizeFlangeSide.SelectedIndex = 0 ;
      }

      //ボルト1-2
      bk = cmbBoltSizeWebSide.Text ;
      cmbBoltSizeWebSide.Items.Clear() ;
      foreach ( string str in lstStrW ) {
        cmbBoltSizeWebSide.Items.Add( str ) ;
      }

      if ( ! string.IsNullOrWhiteSpace( cmbBoltTypeW.Text ) ) {
        if ( lstStrW.Contains( bk ) ) cmbBoltSizeWebSide.Text = bk ;
        else cmbBoltSizeWebSide.SelectedIndex = 0 ;
      }

      //ボルト2-1
      bk = cmbBoltSizeFlangeSide2.Text ;
      cmbBoltSizeFlangeSide2.Items.Clear() ;
      foreach ( string str in lstStr2 ) {
        cmbBoltSizeFlangeSide2.Items.Add( str ) ;
      }

      if ( ! string.IsNullOrWhiteSpace( cmbBoltTypeF2.Text ) ) {
        if ( lstStr2.Contains( bk ) ) cmbBoltSizeFlangeSide2.Text = bk ;
        else cmbBoltSizeFlangeSide2.SelectedIndex = 0 ;
      }

      //ボルト2-2
      bk = cmbBoltSizeWebSide2.Text ;
      cmbBoltSizeWebSide2.Items.Clear() ;
      foreach ( string str in lstStrW2 ) {
        cmbBoltSizeWebSide2.Items.Add( str ) ;
      }

      if ( ! string.IsNullOrWhiteSpace( cmbBoltTypeW2.Text ) ) {
        if ( lstStrW2.Contains( bk ) ) cmbBoltSizeWebSide2.Text = bk ;
        else cmbBoltSizeWebSide2.SelectedIndex = 0 ;
      }

      //CSVから継手情報を取得
      string strKotei = ( rbnKoteiTypeBolt.Checked
        ? Master.ClsPileTsugiteCsv.KoteiHohoBolt
        : Master.ClsPileTsugiteCsv.KoteiHohoYousetsu ) ;
      Master.ClsPileTsugiteCsv pileCsv = Master.ClsPileTsugiteCsv.GetCls( strKotei, cmbKouzaiSize.Text ) ;

      string strKotei2 = ( rbnKoteiTypeBolt2.Checked
        ? Master.ClsPileTsugiteCsv.KoteiHohoBolt
        : Master.ClsPileTsugiteCsv.KoteiHohoYousetsu ) ;
      Master.ClsPileTsugiteCsv pileCsv2 = Master.ClsPileTsugiteCsv.GetCls( strKotei2, cmbKouzaiSize.Text ) ;

      //各コントロールに値を設定
      txtPlateSizeFlangeOutSide.Text = pileCsv.PlateSizeFOut ;
      txtPlateCountFlangeOutSide.Text = pileCsv.PlateNumFOut.ToString() ;
      txtPlateSizeFlangeInSide.Text = pileCsv.PlateSizeFIn ;
      txtPlateCountFlangeInSide.Text = pileCsv.PlateNumFIn.ToString() ;
      txtPlateSizeWebSide1.Text = pileCsv.PlateSizeW ;
      txtPlateCountWebSide1.Text = pileCsv.PlateNumW.ToString() ;
      txtPlateSizeWebSide2.Text = pileCsv.PlateSizeW2 ;
      txtPlateCountWebSide2.Text = pileCsv.PlateNumW2.ToString() ;
      cmbBoltSizeFlangeSide.Text = pileCsv.BoltSizeF ;
      txtBoltCountFlangeSide.Text = pileCsv.BoltNumF.ToString() ;
      cmbBoltSizeWebSide.Text = pileCsv.BoltSizeW ;
      txtBoltCountWebSide.Text = pileCsv.BoltNumW.ToString() ;

      txtPlateSizeFlangeOutSide2.Text = pileCsv2.PlateSizeFOut ;
      txtPlateCountFlangeOutSide2.Text = pileCsv2.PlateNumFOut.ToString() ;
      txtPlateSizeFlangeInSide2.Text = pileCsv2.PlateSizeFIn ;
      txtPlateCountFlangeInSide2.Text = pileCsv2.PlateNumFIn.ToString() ;
      txtPlateSizeWebSide12.Text = pileCsv2.PlateSizeW ;
      txtPlateCountWebSide12.Text = pileCsv2.PlateNumW.ToString() ;
      txtPlateSizeWebSide22.Text = pileCsv2.PlateSizeW2 ;
      txtPlateCountWebSide22.Text = pileCsv2.PlateNumW2.ToString() ;
      cmbBoltSizeFlangeSide2.Text = pileCsv2.BoltSizeF ;
      txtBoltCountFlangeSide2.Text = pileCsv2.BoltNumF.ToString() ;
      cmbBoltSizeWebSide2.Text = pileCsv2.BoltSizeW ;
      txtBoltCountWebSide2.Text = pileCsv2.BoltNumW.ToString() ;
    }

    public void initComboBoxBoltType()
    {
      // ボルトタイプ
      string selectedTextF = cmbBoltTypeF.Text ;
      cmbBoltTypeF.Items.Clear() ;
      string selectedTextF2 = cmbBoltTypeF2.Text ;
      cmbBoltTypeF2.Items.Clear() ;
      string selectedTextW = cmbBoltTypeW.Text ;
      cmbBoltTypeW.Items.Clear() ;
      string selectedTextW2 = cmbBoltTypeW2.Text ;
      cmbBoltTypeW2.Items.Clear() ;

      List<string> lstStr = new List<string>()
      {
        Master.ClsBoltCsv.BoltTypeT, Master.ClsBoltCsv.BoltTypeN, Master.ClsBoltCsv.BoltTypeH,
      } ;

      if ( rbnKoteiTypeBolt.Checked ) {
        foreach ( string str in lstStr ) {
          cmbBoltTypeF.Items.Add( str ) ;
          cmbBoltTypeW.Items.Add( str ) ;
        }

        if ( lstStr.Contains( selectedTextF ) ) cmbBoltTypeF.Text = selectedTextF ;
        else cmbBoltTypeF.SelectedIndex = 0 ;
        if ( lstStr.Contains( selectedTextW ) ) cmbBoltTypeW.Text = selectedTextW ;
        else cmbBoltTypeW.SelectedIndex = 0 ;
      }

      if ( rbnKoteiTypeBolt2.Checked ) {
        foreach ( string str in lstStr ) {
          cmbBoltTypeF2.Items.Add( str ) ;
          cmbBoltTypeW2.Items.Add( str ) ;
        }

        if ( lstStr.Contains( selectedTextF2 ) ) cmbBoltTypeF2.Text = selectedTextF2 ;
        else cmbBoltTypeF2.SelectedIndex = 0 ;
        if ( lstStr.Contains( selectedTextW2 ) ) cmbBoltTypeW2.Text = selectedTextW2 ;
        else cmbBoltTypeW2.SelectedIndex = 0 ;
      }
    }

    private void button1_Click( object sender, EventArgs e )
    {
      if ( ! CheckInput() )
        return ;

      SetData() ;
      SetIniData() ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void button2_Click( object sender, EventArgs e )
    {
      this.DialogResult = DialogResult.Cancel ;
      this.Close() ;
    }

    private void button3_Click( object sender, EventArgs e )
    {
      this.DialogResult = DialogResult.Retry ;
      this.Close() ;
    }

    private void textBox7_TextChanged( object sender, EventArgs e )
    {
      if ( ! string.IsNullOrWhiteSpace( txtPlateCountFlangeInSide.Text ) ) {
        dgvPileLength.RowCount = int.Parse( txtPlateCountFlangeInSide.Text ) + 1 ;
        if ( dgvPileLength.RowCount == 1 ) {
          dgvPileLength.RowCount = 0 ;
        }
      }
    }

    private void DlgCreateOyakui_Load( object sender, EventArgs e )
    {
      //txtPlateCountFlangeInSide.Text = 2.ToString();
      //cmbShoriType.Items.AddRange(ClsGlobal.m_syorihouhou5.ToArray());
      //cmbZanti.Items.AddRange(ClsGlobal.m_zanti1.ToArray());
      //cmbBoltSizeFlangeSide.Items.AddRange(ClsGlobal.m_boltsize.ToArray());
      //cmbBoltSizeWebSide.Items.AddRange(ClsGlobal.m_boltsize.ToArray());
      //comboBox8.Items.AddRange(ClsGlobal.m_yokoyaitaytype.ToArray());


      int nKashoNum = 0 ;
      int.TryParse( txtKashoNum.Text, out nKashoNum ) ;
      InitDataGridView( dgvPileLength, nKashoNum ) ;

      int.TryParse( txtKashoNum2.Text, out nKashoNum ) ;
      InitDataGridView( dgvPileLength2, nKashoNum ) ;
      dgvPileLength2.Enabled = false ;
    }


    public void SetData()
    {
      ClsOyakui clsOyakui = m_ClsOyakui ;
      clsOyakui.m_case = ClsCommonUtils.ChangeStrToInt( txtCase.Text ) ;
      clsOyakui.m_edaNum = txtEdaNumber.Text ;
      clsOyakui.m_edaNum2 = tabPage2.Text ;
      clsOyakui.m_way = cmbShoriType.SelectedIndex ;
      clsOyakui.m_void = ClsCommonUtils.ChangeStrToInt( txtKussakuFukasa.Text ) ;
      clsOyakui.m_refPDist = ClsCommonUtils.ChangeStrToInt( txtKijuntenKyori.Text ) ;
      clsOyakui.m_putPitch = ClsCommonUtils.ChangeStrToInt( txtPutPitch.Text ) ;
      clsOyakui.m_type = cmbKouzaiType.Text ;
      clsOyakui.m_size = cmbKouzaiSize.Text ;
      clsOyakui.m_HTop = ClsCommonUtils.ChangeStrToInt( txtHaichiIchiTop.Text ) ;
      clsOyakui.m_HLen = ClsCommonUtils.ChangeStrToInt( txtHaichiIchiTotallength.Text ) ;
      clsOyakui.m_zanti = cmbZanti.Text ;
      clsOyakui.m_zantiLength = txtZantiLength.Text ;
      var RadioButtonChecked_InGroup =
        groupBox9.Controls.OfType<RadioButton>().SingleOrDefault( rb => rb.Checked == true ) ;
      clsOyakui.m_putPosFlag = ClsCommonUtils.ChangeStrToInt( RadioButtonChecked_InGroup.Tag.ToString() ) ;

      clsOyakui.m_bYokoyaita = chkYokoyaita.Checked ;
      clsOyakui.m_typeYokoyaita = cmbYokoyaitaType.Text ;
      clsOyakui.m_sizeYokoyaita = cmbYokoyaitaSize.Text ;
      var RadioButtonChecked_InYOkoyaGroup =
        groupBox7.Controls.OfType<RadioButton>().SingleOrDefault( rb => rb.Checked == true ) ;
      clsOyakui.m_putPosYokoyaitaFlag =
        ClsCommonUtils.ChangeStrToInt( RadioButtonChecked_InYOkoyaGroup.Tag.ToString() ) ;
      ;


      //グリッドビュー

      clsOyakui.m_KougoFlg = chkKougoHaichi.Checked ;
      clsOyakui.m_Kasho1 = ClsCommonUtils.ChangeStrToInt( txtKashoNum.Text ) ;
      clsOyakui.m_Kasho2 = ClsCommonUtils.ChangeStrToInt( txtKashoNum2.Text ) ;
      clsOyakui.m_Kotei1 = rbnKoteiTypeBolt.Checked ? Master.Kotei.Bolt : Master.Kotei.Yousetsu ;
      clsOyakui.m_Kotei2 = rbnKoteiTypeBolt2.Checked ? Master.Kotei.Bolt : Master.Kotei.Yousetsu ;

      List<int> lstN1 = new List<int>() ;
      for ( int i = 0 ; i < dgvPileLength.Columns.Count ; i++ ) {
        lstN1.Add( ClsCommonUtils.ChangeStrToInt( dgvPileLength[ i, 0 ].Value.ToString() ) ) ;
      }

      clsOyakui.m_ListPileLength1 = lstN1 ;
      List<int> lstN2 = new List<int>() ;
      for ( int i = 0 ; i < dgvPileLength2.Columns.Count ; i++ ) {
        lstN2.Add( ClsCommonUtils.ChangeStrToInt( dgvPileLength2[ i, 0 ].Value.ToString() ) ) ;
      }

      clsOyakui.m_ListPileLength2 = lstN2 ;

      clsOyakui.m_BoltF1 = cmbBoltSizeFlangeSide.Text ;
      clsOyakui.m_BoltFNum1 = txtBoltCountFlangeSide.Text ;
      clsOyakui.m_BoltW1 = cmbBoltSizeWebSide.Text ;
      clsOyakui.m_BoltWNum1 = txtBoltCountWebSide.Text ;
      clsOyakui.m_BoltF2 = cmbBoltSizeFlangeSide2.Text ;
      clsOyakui.m_BoltFNum2 = txtBoltCountFlangeSide2.Text ;
      clsOyakui.m_BoltW2 = cmbBoltSizeWebSide2.Text ;
      clsOyakui.m_BoltWNum2 = txtBoltCountWebSide2.Text ;


      //メンバ変数に格納
      m_ClsOyakui = clsOyakui ;
    }

    /// <summary>
    /// メンバの値をコントロールにセット
    /// </summary>
    private void SetControl()
    {
      ClsOyakui clsOyakui = m_ClsOyakui ;
      txtCase.Text = clsOyakui.m_case.ToString() ;
      txtEdaNumber.Text = clsOyakui.m_edaNum ;

      //clsOyakui.m_way = cmbShoriType.SelectedIndex;
      txtKussakuFukasa.Text = clsOyakui.m_void.ToString() ;
      //clsOyakui.m_refPDist = ClsCommonUtils.ChangeStrToInt(txtKijuntenKyori.Text);
      //clsOyakui.m_putPitch = ClsCommonUtils.ChangeStrToInt(txtPutPitch.Text);

      cmbKouzaiType.Text = clsOyakui.m_type ;
      cmbKouzaiSize.Text = clsOyakui.m_size ;
      txtHaichiIchiTop.Text = clsOyakui.m_HTop.ToString() ;
      txtHaichiIchiTotallength.Text = clsOyakui.m_HLen.ToString() ;
      cmbZanti.Text = clsOyakui.m_zanti ;
      txtZantiLength.Text = clsOyakui.m_zantiLength ;

      //var RadioButtonChecked_InGroup = groupBox9.Controls.OfType<RadioButton>().SingleOrDefault(rb => rb.Checked == true);
      //clsOyakui.m_putPosFlag = ClsCommonUtils.ChangeStrToInt(RadioButtonChecked_InGroup.Tag.ToString());

      //clsOyakui.m_bYokoyaita = chkYokoyaita.Checked;
      //clsOyakui.m_typeYokoyaita = cmbYokoyaitaType.Text;
      //clsOyakui.m_sizeYokoyaita = cmbYokoyaitaSize.Text;
      //var RadioButtonChecked_InYOkoyaGroup = groupBox7.Controls.OfType<RadioButton>().SingleOrDefault(rb => rb.Checked == true);
      //clsOyakui.m_putPosYokoyaitaFlag = ClsCommonUtils.ChangeStrToInt(RadioButtonChecked_InYOkoyaGroup.Tag.ToString()); ;


      //グリッドビュー

      //clsOyakui.m_KougoFlg = chkKougoHaichi.Checked;
      txtKashoNum.Text = clsOyakui.m_Kasho1.ToString() ;
      //clsOyakui.m_Kasho2 = ClsCommonUtils.ChangeStrToInt(txtKashoNum2.Text);

      List<int> lstN1 = clsOyakui.m_ListPileLength1 ;
      for ( int i = 0 ; i < lstN1.Count ; i++ ) {
        dgvPileLength[ i, 0 ].Value = lstN1[ i ].ToString() ;
      }
      //List<int> lstN2 = new List<int>();
      //for (int i = 0; i < dgvPileLength2.Columns.Count; i++)
      //{
      //    lstN2.Add(ClsCommonUtils.ChangeStrToInt(dgvPileLength2[i, 0].Value.ToString()));
      //}
      //clsOyakui.m_ListPileLength2 = lstN2;
    }

    /// <summary>
    /// ダイアログデータをini二セット
    /// </summary>
    public void SetIniData()
    {
      string iniPath = ClsIni.GetIniFilePath( IniPath ) ;

      ClsIni.WritePrivateProfileString( sec, txtCase.Name, txtCase.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, txtEdaNumber.Name, txtEdaNumber.Text, iniPath ) ;

      ClsIni.WritePrivateProfileString( sec, cmbShoriType.Name, cmbShoriType.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, txtKussakuFukasa.Name, txtKussakuFukasa.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, txtKijuntenKyori.Name, txtKijuntenKyori.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, txtPutPitch.Name, txtPutPitch.Text, iniPath ) ;

      ClsIni.WritePrivateProfileString( sec, cmbKouzaiType.Name, cmbKouzaiType.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, cmbKouzaiSize.Name, cmbKouzaiSize.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, txtHaichiIchiTop.Name, txtHaichiIchiTop.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, txtHaichiIchiTotallength.Name, txtHaichiIchiTotallength.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, cmbZanti.Name, cmbZanti.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, txtZantiLength.Name, txtZantiLength.Text, iniPath ) ;

      ClsIni.WritePrivateProfileString( sec, rbnKuiFace.Name, rbnKuiFace.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, rbnKuiCore.Name, rbnKuiCore.Checked.ToString(), iniPath ) ;

      ClsIni.WritePrivateProfileString( sec, chkYokoyaita.Name, chkYokoyaita.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, cmbYokoyaitaType.Name, cmbYokoyaitaType.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, cmbYokoyaitaSize.Name, cmbYokoyaitaSize.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, rbnIn.Name, rbnIn.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, rbnOut.Name, rbnOut.Checked.ToString(), iniPath ) ;

      ClsIni.WritePrivateProfileString( sec, chkKougoHaichi.Name, chkKougoHaichi.Checked.ToString(), iniPath ) ;

      ClsIni.WritePrivateProfileString( sec, txtKashoNum.Name, txtKashoNum.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, rbnKoteiTypeBolt.Name, rbnKoteiTypeBolt.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, rbnKoteiTypeYosetsu.Name, rbnKoteiTypeYosetsu.Checked.ToString(),
        iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, txtKashoNum2.Name, txtKashoNum2.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, rbnKoteiTypeBolt2.Name, rbnKoteiTypeBolt2.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, rbnKoteiTypeYosetsu2.Name, rbnKoteiTypeYosetsu2.Checked.ToString(),
        iniPath ) ;
    }

    /// <summary>
    /// iniデータをダイアログにセット
    /// </summary>
    public void GetIniData()
    {
      string iniPath = ClsIni.GetIniFilePath( IniPath ) ;
      //txtCase.Text = ClsIni.GetIniFile(sec, txtCase.Name, iniPath);
      //txtEdaNumber.Text = ClsIni.GetIniFile(sec, txtEdaNumber.Name, iniPath);

      cmbShoriType.Text = ClsIni.GetIniFile( sec, cmbShoriType.Name, iniPath ) ;
      txtKussakuFukasa.Text = ClsIni.GetIniFile( sec, txtKussakuFukasa.Name, iniPath ) ;
      txtKijuntenKyori.Text = ClsIni.GetIniFile( sec, txtKijuntenKyori.Name, iniPath ) ;
      txtPutPitch.Text = ClsIni.GetIniFile( sec, txtPutPitch.Name, iniPath ) ;

      cmbKouzaiType.Text = ClsIni.GetIniFile( sec, cmbKouzaiType.Name, iniPath ) ;
      cmbKouzaiSize.Text = ClsIni.GetIniFile( sec, cmbKouzaiSize.Name, iniPath ) ;
      txtHaichiIchiTop.Text = ClsIni.GetIniFile( sec, txtHaichiIchiTop.Name, iniPath ) ;
      txtHaichiIchiTotallength.Text = ClsIni.GetIniFile( sec, txtHaichiIchiTotallength.Name, iniPath ) ;
      cmbZanti.Text = ClsIni.GetIniFile( sec, cmbZanti.Name, iniPath ) ;
      txtZantiLength.Text = ClsIni.GetIniFile( sec, txtZantiLength.Name, iniPath ) ;

      rbnKuiFace.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnKuiFace.Name, iniPath ) ) ;
      rbnKuiCore.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnKuiCore.Name, iniPath ) ) ;

      chkYokoyaita.Checked = bool.Parse( ClsIni.GetIniFile( sec, chkYokoyaita.Name, iniPath ) ) ;
      cmbYokoyaitaType.Text = ClsIni.GetIniFile( sec, cmbYokoyaitaType.Name, iniPath ) ;
      cmbYokoyaitaSize.Text = ClsIni.GetIniFile( sec, cmbYokoyaitaSize.Name, iniPath ) ;
      rbnIn.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnIn.Name, iniPath ) ) ;
      rbnOut.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnOut.Name, iniPath ) ) ;

      chkKougoHaichi.Checked = bool.Parse( ClsIni.GetIniFile( sec, chkKougoHaichi.Name, iniPath ) ) ;

      txtKashoNum.Text = ClsIni.GetIniFile( sec, txtKashoNum.Name, iniPath ) ;
      rbnKoteiTypeBolt.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnKoteiTypeBolt.Name, iniPath ) ) ;
      rbnKoteiTypeYosetsu.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnKoteiTypeYosetsu.Name, iniPath ) ) ;
      txtKashoNum2.Text = ClsIni.GetIniFile( sec, txtKashoNum2.Name, iniPath ) ;
      rbnKoteiTypeBolt2.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnKoteiTypeBolt2.Name, iniPath ) ) ;
      rbnKoteiTypeYosetsu2.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnKoteiTypeYosetsu2.Name, iniPath ) ) ;
    }

    /// <summary>
    /// 入力チェック
    /// </summary>
    /// <returns></returns>
    public bool CheckInput()
    {
      if ( ! ClsCommonUtils.ChkContorolText( txtCase, "CASE", bNull: true ) )
        return false ;
      if ( ! ClsCommonUtils.ChkContorolTextAlphabet( txtEdaNumber, "枝番号", bNull: true ) )
        return false ;
      if ( ! ClsCommonUtils.ChkContorolTextDouble( txtKussakuFukasa, "掘削深さ" ) )
        return false ;
      if ( ! ClsCommonUtils.ChkContorolTextDouble( txtKijuntenKyori, "基準点からの距離" ) )
        return false ;
      if ( ! ClsCommonUtils.ChkContorolTextDouble( txtPutPitch, "配置ピッチ", bMinus: true, bZero: true ) )
        return false ;
      if ( ! ClsCommonUtils.ChkContorolTextDouble( txtHaichiIchiTop, "芯材：天端", bMinus: true ) )
        return false ;
      if ( ! ClsCommonUtils.ChkContorolTextDouble( txtHaichiIchiTotallength, "芯材：全長", bMinus: true, bZero: true ) )
        return false ;
      if ( ! ClsCommonUtils.ChkContorolTextDouble( txtKashoNum, "箇所数(1)", bMinus: true, bComma: true ) )
        return false ;
      if ( ! ClsCommonUtils.ChkContorolTextDouble( txtKashoNum2, "箇所数(2)", bMinus: true, bComma: true ) )
        return false ;
      if ( ! ClsCommonUtils.ChkContorolTextDouble( txtBoltCountFlangeSide, "ボルト本数(1)（フランジ側）", bMinus: true,
            bComma: true ) )
        return false ;
      if ( ! ClsCommonUtils.ChkContorolTextDouble( txtBoltCountFlangeSide2, "ボルト本数(2)（フランジ側）", bMinus: true,
            bComma: true ) )
        return false ;
      if ( ! ClsCommonUtils.ChkContorolTextDouble( txtBoltCountWebSide, "ボルト本数(1)（Web側）", bMinus: true, bComma: true ) )
        return false ;
      if ( ! ClsCommonUtils.ChkContorolTextDouble( txtBoltCountWebSide2, "ボルト本数(2)（Web側）", bMinus: true,
            bComma: true ) )
        return false ;

      //杭の全長チェック
      List<int> lstN1 = new List<int>() ;
      for ( int i = 0 ; i < dgvPileLength.Columns.Count ; i++ ) {
        lstN1.Add( ClsCommonUtils.ChangeStrToInt( dgvPileLength[ i, 0 ].Value.ToString() ) ) ;
      }

      ;
      if ( lstN1.Sum() != ( ClsCommonUtils.ChangeStrToInt( txtHaichiIchiTotallength.Text ) ) ) {
        MessageBox.Show( "杭の長さの合計と全長が一致しません。[タブ1]" ) ;
        return false ;
      }

      if ( chkKougoHaichi.Checked ) {
        List<int> lstN2 = new List<int>() ;
        for ( int i = 0 ; i < dgvPileLength2.Columns.Count ; i++ ) {
          lstN2.Add( ClsCommonUtils.ChangeStrToInt( dgvPileLength2[ i, 0 ].Value.ToString() ) ) ;
        }

        if ( lstN2.Sum() != ( ClsCommonUtils.ChangeStrToInt( txtHaichiIchiTotallength.Text ) ) ) {
          MessageBox.Show( "杭の長さの合計と全長が一致しません。[タブ2]" ) ;
          return false ;
        }
      }

      return true ;
    }

    private void InitDataGridView( DataGridView dgv, int nColmnCount )
    {
      dgv.Columns.Clear() ;
      dgv.Rows.Clear() ;

      for ( int i = 0 ; i <= nColmnCount ; i++ ) {
        DataGridViewTextBoxColumn clm = new DataGridViewTextBoxColumn() ;
        clm.Name = "clm" + ( i + 1 ).ToString() ;
        clm.HeaderText = "杭" + ( i + 1 ).ToString() + "長さ" ;
        dgv.Columns.Add( clm ) ;
      }

      if ( nColmnCount >= 0 ) dgv.Rows.Add() ;

      int remainingLength = ClsCommonUtils.ChangeStrToInt( txtHaichiIchiTotallength.Text ) ;
      int kuiLength = remainingLength / ( nColmnCount + 1 ) ;
      for ( int i = 0 ; i <= nColmnCount ; i++ ) {
        if ( i != nColmnCount ) {
          dgv[ i, 0 ].Value = kuiLength ;
          remainingLength -= kuiLength ;
        }
        else
          dgv[ i, 0 ].Value = remainingLength ;
      }
    }

    private void rbnKoteiTypeBolt_CheckedChanged( object sender, EventArgs e )
    {
      initComboBoxBoltType() ;
      initOtherControl() ;
      SetKougoType() ;
    }

    private void radioButton4_CheckedChanged( object sender, EventArgs e )
    {
      initComboBoxBoltType() ;
      initOtherControl() ;
      SetKougoType() ;
    }

    private void rbnKoteiTypeBolt2_CheckedChanged( object sender, EventArgs e )
    {
      initComboBoxBoltType() ;
      initOtherControl() ;
    }

    private void radioButton3_CheckedChanged( object sender, EventArgs e )
    {
      initComboBoxBoltType() ;
      initOtherControl() ;
    }

    private void cmbKouzaiType_SelectedIndexChanged( object sender, EventArgs e )
    {
      initComboBoxSteelSize() ;
    }

    private void cmbKouzaiSize_SelectedIndexChanged( object sender, EventArgs e )
    {
      initComboBoxBoltType() ;
      initOtherControl() ;
    }

    private void cmbBoltTypeF_SelectedIndexChanged( object sender, EventArgs e )
    {
      initOtherControl() ;
    }

    private void cmbBoltTypeW_SelectedIndexChanged( object sender, EventArgs e )
    {
      initOtherControl() ;
    }

    private void cmbBoltTypeF2_SelectedIndexChanged( object sender, EventArgs e )
    {
      initOtherControl() ;
    }

    private void cmbBoltTypeW2_SelectedIndexChanged( object sender, EventArgs e )
    {
      initOtherControl() ;
    }

    private void txtKashoNum_TextChanged( object sender, EventArgs e )
    {
      int nKashoNum = 0 ;
      if ( ! int.TryParse( txtKashoNum.Text, out nKashoNum ) ) {
        return ;
      }

      if ( nKashoNum < 0 || nKashoNum > 10 ) {
        MessageBox.Show( "接続箇所数は0以上10以下の整数を入力してください。" ) ;
        return ;
      }

      InitDataGridView( dgvPileLength, nKashoNum ) ;

      txtKashoNum2.Text = txtKashoNum.Text ;
      InitDataGridView( dgvPileLength2, nKashoNum ) ;
    }

    private void txtKashoNum2_TextChanged( object sender, EventArgs e )
    {
      //int nKashoNum = 0;
      //if (!int.TryParse(txtKashoNum.Text, out nKashoNum))
      //{
      //    return;
      //}

      //if (nKashoNum < 0 || nKashoNum > 10)
      //{
      //    MessageBox.Show("接続箇所数は0以上10以下の整数を入力してください。");
      //    return;
      //}

      //InitDataGridView(dgvPileLength2, nKashoNum);
    }

    private void txtHaichiIchiTotallength_TextChanged( object sender, EventArgs e )
    {
      int nKashoNum = 0 ;
      if ( ! int.TryParse( txtKashoNum.Text, out nKashoNum ) ) {
        return ;
      }

      InitDataGridView( dgvPileLength, nKashoNum ) ;
    }

    private void dgvPileLength_CellValueChanged( object sender, DataGridViewCellEventArgs e )
    {
      dgvPileLengthCellValueChanged( dgvPileLength, e.ColumnIndex ) ;
    }

    private void dgvPileLength_CellClick( object sender, DataGridViewCellEventArgs e )
    {
      m_ChangeDGVKuiLength = true ;
    }

    private void dgvPileLength2_CellValueChanged( object sender, DataGridViewCellEventArgs e )
    {
      //dgvPileLengthCellValueChanged(dgvPileLength2, e.ColumnIndex);
    }

    private void dgvPileLength2_CellClick( object sender, DataGridViewCellEventArgs e )
    {
      m_ChangeDGVKuiLength = true ;
    }

    private void dgvPileLengthCellValueChanged( DataGridView dgv, int columnIndex )
    {
      if ( ! m_ChangeDGVKuiLength )
        return ;
      m_ChangeDGVKuiLength = false ;
      int nColmnCount = dgv.Columns.Count - 1 ;
      int kuiTotal = ClsCommonUtils.ChangeStrToInt( txtHaichiIchiTotallength.Text ) ;
      int total = 0 ;
      if ( columnIndex == nColmnCount ) //最終列変更時
      {
        for ( int i = 0 ; i < columnIndex + 1 ; i++ ) {
          total += ClsCommonUtils.GetCellValToInt( dgv[ i, 0 ] ) ;
        }
      }
      else {
        int remainingLength = 0 ; // kuiTotal - ClsCommonUtils.ChangeStrToInt(dgv[columnIndex, 0].Value.ToString());
        //int kuiLength = remainingLength / (nColmnCount - 1);
        for ( int i = 0 ; i < nColmnCount ; i++ ) {
          remainingLength += ClsCommonUtils.GetCellValToInt( dgv[ i, 0 ] ) ;
        }

        dgv[ nColmnCount, 0 ].Value = kuiTotal - remainingLength ;
        total = kuiTotal ;
      }

      if ( kuiTotal != total ) {
        //赤くする
        lblWarning.Text = "杭長さの合計と全長が一致しません。" ;
        ChangeCellColor( dgv, Color.Red ) ;
      }
      else {
        lblWarning.Text = "" ;
        ChangeCellColor( dgv, Color.Empty ) ;
      }

      if ( chkKougoHaichi.Checked )
        SetKougoHaichi() ;
    }

    private void ChangeCellColor( DataGridView dgv, Color color )
    {
      for ( int i = 0 ; i < dgv.Columns.Count ; i++ ) {
        dgv[ i, 0 ].Style.BackColor = color ;
      }
    }

    private void SetKougoHaichi()
    {
      int nColmnCount = dgvPileLength.Columns.Count - 1 ;
      int nColmnCount2 = dgvPileLength2.Columns.Count - 1 ;
      if ( nColmnCount != nColmnCount2 )
        return ;
      int nRowCount = dgvPileLength.Rows.Count ;
      int nRowCount2 = dgvPileLength2.Rows.Count ;
      if ( nRowCount <= 0 || nRowCount2 <= 0 )
        return ;

      int total = 0 ;
      for ( int i = 0 ; i <= nColmnCount ; i++ ) {
        dgvPileLength2[ i, 0 ].Value = dgvPileLength[ nColmnCount - i, 0 ].Value ;
        total += ClsCommonUtils.ChangeStrToInt( dgvPileLength[ i, 0 ].Value.ToString() ) ;
      }

      if ( ClsCommonUtils.ChangeStrToInt( txtHaichiIchiTotallength.Text ) != total )
        ChangeCellColor( dgvPileLength2, Color.Red ) ;
      else
        ChangeCellColor( dgvPileLength2, Color.Empty ) ;
    }

    private void SetKougoType()
    {
      if ( rbnKoteiTypeBolt.Checked )
        rbnKoteiTypeBolt2.Checked = true ;
      else
        rbnKoteiTypeYosetsu2.Checked = true ;
    }

    private void chkKougoHaichi_CheckedChanged( object sender, EventArgs e )
    {
      if ( chkKougoHaichi.Checked ) {
        SetKougoHaichi() ;
        SetNextEdaNum() ;
        SetKougoType() ;
      }
    }

    private void cmbZanti_SelectedIndexChanged( object sender, EventArgs e )
    {
      if ( cmbZanti.SelectedItem.ToString() == "一部埋め殺し" ) {
        lblZantiLength.Enabled = true ;
        txtZantiLength.Enabled = true ;
      }
      else {
        lblZantiLength.Enabled = false ;
        txtZantiLength.Enabled = false ;
      }
    }

    private void txtCase_TextChanged( object sender, EventArgs e )
    {
      ClsCASE clsCASE = m_caseInfo ;
      string nextCASE = string.Empty ;
      enCASEHit enCASEHit = clsCASE.ExistCase( enKabeshu.oyagui, txtCase.Text, ref nextCASE ) ;
      if ( enCASEHit == enCASEHit.sameKabeshu ) {
        foreach ( stCASEOyagui oyagui in clsCASE.m_oyaguiDataList ) {
          if ( oyagui.CASE == txtCase.Text ) {
            cmbKouzaiType.Text = oyagui.buzaiType ;
            cmbKouzaiSize.Text = oyagui.buzaiSize ;
            txtHaichiIchiTotallength.Text = oyagui.TotalLength.ToString() ;
            cmbZanti.Text = oyagui.zanchi ;
            txtZantiLength.Text = oyagui.zanchiLength ;
            break ;
          }
        }

        SetCASEControl( false ) ;
      }
      else if ( enCASEHit == enCASEHit.noHit )
        SetCASEControl( true ) ;
    }

    private void txtEdaNumber_TextChanged( object sender, EventArgs e )
    {
      ClsCASE clsCASE = m_caseInfo ;
      string nextCASE = string.Empty ;
      string nextEdaban = string.Empty ;
      enCASE_EdabanHit enCASE_EdabanHit = clsCASE.ExistCaseEdaban( enKabeshu.oyagui, txtCase.Text, txtEdaNumber.Text,
        ref nextCASE, ref nextEdaban ) ;
      if ( enCASE_EdabanHit == enCASE_EdabanHit.sameKabeshu ) {
        foreach ( stCASEOyagui oyagui in clsCASE.m_oyaguiDataList ) {
          if ( oyagui.CASE == txtCase.Text && oyagui.Edaban == txtEdaNumber.Text ) {
            txtKashoNum.Text = oyagui.JointNum.ToString() ;
            if ( dgvPileLength.Columns.Count == oyagui.KuiLengthList.Count && 0 < dgvPileLength.Rows.Count ) {
              for ( int i = 0 ; i < dgvPileLength.Columns.Count ; i++ ) {
                dgvPileLength[ i, 0 ].Value = oyagui.KuiLengthList[ i ] ;
              }
            }

            break ;
          }
        }

        SetEdaNum( false ) ;
      }
      else if ( enCASE_EdabanHit == enCASE_EdabanHit.noHit )
        SetEdaNum( true ) ;

      tabPage1.Text = txtEdaNumber.Text ;
      SetNextEdaNum() ;
    }

    private void SetCASEControl( bool set )
    {
      cmbKouzaiType.Enabled = set ;
      cmbKouzaiSize.Enabled = set ;
      txtHaichiIchiTotallength.Enabled = set ;
      cmbZanti.Enabled = set ;
      txtZantiLength.Enabled = set ;
    }

    private void SetEdaNum( bool set )
    {
      txtKashoNum.Enabled = set ;
      //dgvPileLength.Enabled = set;
      SetColorAndReadonyRow( dgvPileLength, set ) ;
      SetColorAndReadonyRow( dgvPileLength2, set ) ;
    }

    /// <summary>
    /// DGVの行をReadonyに変更
    /// </summary>
    private void SetColorAndReadonyRow( DataGridView dgv, bool set )
    {
      if ( dgv.Rows.Count < 0 )
        return ;

      set = set == false ? true : false ;
      DataGridViewRow row = dgv.Rows[ 0 ] ;
      foreach ( DataGridViewCell cell in row.Cells ) {
        if ( set )
          cell.Style.BackColor = ClsGlobal.m_GrayColor ;
        else
          cell.Style.BackColor = ClsGlobal.m_WhiteColor ;
        cell.ReadOnly = set ;
      }
    }

    private void SetNextEdaNum()
    {
      if ( chkKougoHaichi.Checked && ! string.IsNullOrWhiteSpace( txtEdaNumber.Text ) ) {
        ClsCASE clsCASE = m_caseInfo ;
        //ダイアログに入力された枝番の次のアルファベット
        int numeric = ClsCommonUtils.ConvertToNumeric( txtEdaNumber.Text ) ;
        numeric += 1 ;
        string edaban = ClsCommonUtils.ConvertToAlphabet( numeric ) ;

        string nextCASE = string.Empty ;
        string nextEdaban = string.Empty ;
        enCASE_EdabanHit enCASE_EdabanHit =
          clsCASE.ExistCaseEdaban( enKabeshu.oyagui, txtCase.Text, edaban, ref nextCASE, ref nextEdaban ) ;
        bool use = true ;
        if ( enCASE_EdabanHit == enCASE_EdabanHit.sameKabeshu ) {
          foreach ( stCASEOyagui oyagui in clsCASE.m_oyaguiDataList ) {
            if ( oyagui.CASE == txtCase.Text && oyagui.Edaban == edaban ) {
              if ( txtKashoNum.Text == oyagui.JointNum.ToString() ) {
                if ( dgvPileLength.Columns.Count == oyagui.KuiLengthList.Count && 0 < dgvPileLength.Rows.Count ) {
                  for ( int i = 0 ; i < dgvPileLength.Columns.Count ; i++ ) {
                    if ( (int) dgvPileLength[ i, 0 ].Value !=
                         oyagui.KuiLengthList[ oyagui.KuiLengthList.Count - 1 - i ] ) {
                      use = false ;
                      break ;
                    }
                  }
                }
              }

              if ( use ) {
                tabPage2.Text = edaban ;
                txtKashoNum2.Text = oyagui.JointNum.ToString() ;
                if ( dgvPileLength2.Columns.Count == oyagui.KuiLengthList.Count && 0 < dgvPileLength2.Rows.Count ) {
                  for ( int i = 0 ; i < dgvPileLength.Columns.Count ; i++ ) {
                    dgvPileLength2[ i, 0 ].Value = oyagui.KuiLengthList[ i ] ;
                  }
                }
              }
              else {
                string nextEdaNum = clsCASE.GetEdabanAlphabet( enKabeshu.smw, txtCase.Text, txtEdaNumber.Text ) ;
                tabPage2.Text = nextEdaNum ;
                if ( chkKougoHaichi.Checked ) {
                  SetKougoHaichi() ;
                }
              }

              break ;
            }
          }
        }
        else if ( enCASE_EdabanHit == enCASE_EdabanHit.noHit ) {
          string nextEdaNum = clsCASE.GetEdabanAlphabet( enKabeshu.oyagui, txtCase.Text, txtEdaNumber.Text ) ;
          tabPage2.Text = nextEdaNum ;
        }
      }
    }
  }
}