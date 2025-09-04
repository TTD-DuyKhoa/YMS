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
  public partial class DlgCreateKouyaita : Form
  {
    #region 定数

    /// <summary>
    /// Iniファイルのパス
    /// </summary>
    const string IniPath = "ini\\DlgCreateKouyaita.ini" ;

    /// <summary>
    /// Iniファイルのセクション名
    /// </summary>
    const string sec = "DlgKouyaita" ;

    #endregion

    #region メンバー

    public ClsKouyaita m_ClsKouyaita ;
    private bool m_ChangeDGVKuiLength = false ;
    private ClsCASE m_caseInfo ;

    #endregion

    public DlgCreateKouyaita( Autodesk.Revit.DB.Document doc )
    {
      InitializeComponent() ;
      m_ClsKouyaita = new ClsKouyaita() ;
      m_caseInfo = new ClsCASE( ClsCASE.enKabeshu.koyauita, doc ) ;
      initComboBox() ;
      GetIniData() ;
    }

    public DlgCreateKouyaita( Autodesk.Revit.DB.Document doc, ClsKouyaita clsKouyaita )
    {
      InitializeComponent() ;
      m_ClsKouyaita = clsKouyaita ;
      m_caseInfo = new ClsCASE( ClsCASE.enKabeshu.koyauita, doc ) ;
      initComboBox() ;
      GetIniData() ;
      SetControl() ;
    }

    public void initComboBox()
    {
      string bk = string.Empty ;
      List<string> lstStr = new List<string>() ;

      //処理タイプ・残置
      cmbShoriType.Items.AddRange( ClsGlobal.m_syorihouhou3.ToArray() ) ;
      cmbZanchi.Items.AddRange( ClsGlobal.m_zanti.ToArray() ) ;
      cmbShoriType.SelectedIndex = 1 ;

      initComboBoxBoltType() ;


      //鋼材タイプ
      bk = cmbSteelType.Text ;
      cmbSteelType.Items.Clear() ;
      lstStr = Master.ClsKouyaitaCsv.GetTypeList( false ) ;
      foreach ( string str in lstStr ) {
        cmbSteelType.Items.Add( str ) ;
      }

      if ( lstStr.Contains( bk ) ) cmbSteelType.Text = bk ;
      else cmbSteelType.SelectedIndex = 0 ;

      //鋼材サイズ
      initComboBoxSteelSize() ;
    }

    public void initComboBoxSteelSize()
    {
      string bk = string.Empty ;
      List<string> lstStr = new List<string>() ;

      //鋼材サイズ
      bk = cmbSteelSize.Text ;
      cmbSteelSize.Items.Clear() ;
      lstStr = Master.ClsKouyaitaCsv.GetSizeList( cmbSteelType.Text, false ) ;
      foreach ( string str in lstStr ) {
        cmbSteelSize.Items.Add( str ) ;
      }

      if ( lstStr.Contains( bk ) ) cmbSteelSize.Text = bk ;
      else cmbSteelSize.SelectedIndex = 0 ;

      initComboBoxCornerSize() ;
    }

    public void initComboBoxCornerSize()
    {
      string bk = string.Empty ;
      List<string> lstStr = new List<string>() ;

      //コーナー矢板サイズ
      bk = cmbCornerYaitaSize.Text ;
      cmbCornerYaitaSize.Items.Clear() ;
      lstStr = Master.ClsKouyaitaCsv.GetCornerList( cmbSteelType.Text, cmbSteelSize.Text ) ;
      foreach ( string str in lstStr ) {
        cmbCornerYaitaSize.Items.Add( str ) ;
      }

      if ( lstStr.Contains( bk ) ) cmbCornerYaitaSize.Text = bk ;
      else cmbCornerYaitaSize.SelectedIndex = 0 ;

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

      //ボルト1B-1
      bk = cmbBoltSizeFlangeSideB.Text ;
      cmbBoltSizeFlangeSideB.Items.Clear() ;
      foreach ( string str in lstStr ) {
        cmbBoltSizeFlangeSideB.Items.Add( str ) ;
      }

      if ( ! string.IsNullOrWhiteSpace( cmbBoltTypeFB.Text ) ) {
        if ( lstStr.Contains( bk ) ) cmbBoltSizeFlangeSideB.Text = bk ;
        else cmbBoltSizeFlangeSideB.SelectedIndex = 0 ;
      }

      //ボルト1B-2
      bk = cmbBoltSizeWebSideB.Text ;
      cmbBoltSizeWebSideB.Items.Clear() ;
      foreach ( string str in lstStrW ) {
        cmbBoltSizeWebSideB.Items.Add( str ) ;
      }

      if ( ! string.IsNullOrWhiteSpace( cmbBoltTypeWB.Text ) ) {
        if ( lstStrW.Contains( bk ) ) cmbBoltSizeWebSideB.Text = bk ;
        else cmbBoltSizeWebSideB.SelectedIndex = 0 ;
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

      //ボルト2B-1
      bk = cmbBoltSizeFlangeSideB2.Text ;
      cmbBoltSizeFlangeSideB2.Items.Clear() ;
      foreach ( string str in lstStr2 ) {
        cmbBoltSizeFlangeSideB2.Items.Add( str ) ;
      }

      if ( ! string.IsNullOrWhiteSpace( cmbBoltTypeFB2.Text ) ) {
        if ( lstStr2.Contains( bk ) ) cmbBoltSizeFlangeSideB2.Text = bk ;
        else cmbBoltSizeFlangeSideB2.SelectedIndex = 0 ;
      }

      //ボルト2B-2
      bk = cmbBoltSizeWebSideB2.Text ;
      cmbBoltSizeWebSideB2.Items.Clear() ;
      foreach ( string str in lstStrW2 ) {
        cmbBoltSizeWebSideB2.Items.Add( str ) ;
      }

      if ( ! string.IsNullOrWhiteSpace( cmbBoltTypeWB2.Text ) ) {
        if ( lstStrW2.Contains( bk ) ) cmbBoltSizeWebSideB2.Text = bk ;
        else cmbBoltSizeWebSideB2.SelectedIndex = 0 ;
      }

      //CSVから継手情報を取得
      string strKotei = ( rbnKoteiTypeBolt.Checked
        ? Master.ClsPileTsugiteCsv.KoteiHohoBolt
        : Master.ClsPileTsugiteCsv.KoteiHohoYousetsu ) ;
      Master.ClsKouyaitaTsugiteCsv pileCsv = Master.ClsKouyaitaTsugiteCsv.GetCls( strKotei, cmbSteelSize.Text ) ;
      Master.ClsKouyaitaTsugiteCsv cornerPileCsv =
        Master.ClsKouyaitaTsugiteCsv.GetCls( strKotei, cmbCornerYaitaSize.Text ) ;

      string strKotei2 = ( rbnKoteiTypeBolt2.Checked
        ? Master.ClsPileTsugiteCsv.KoteiHohoBolt
        : Master.ClsPileTsugiteCsv.KoteiHohoYousetsu ) ;
      Master.ClsKouyaitaTsugiteCsv pileCsv2 = Master.ClsKouyaitaTsugiteCsv.GetCls( strKotei2, cmbSteelSize.Text ) ;
      Master.ClsKouyaitaTsugiteCsv cornerPileCsv2 =
        Master.ClsKouyaitaTsugiteCsv.GetCls( strKotei2, cmbCornerYaitaSize.Text ) ;

      //各コントロールに値を設定
      //コーナー以外
      txtPlateSizeFlangeSide.Text = pileCsv.PlateSizeF ;
      txtPlateCountFlangeSide.Text = pileCsv.PlateNumF.ToString() ;
      txtPlateSizeWebSide1.Text = pileCsv.PlateSizeW ;
      txtPlateCountWebSide1.Text = pileCsv.PlateNumW.ToString() ;
      txtPlateSizeWebSide2.Text = pileCsv.PlateSizeW2 ;
      txtPlateCountWebSide2.Text = pileCsv.PlateNumW2.ToString() ;
      cmbBoltSizeFlangeSide.Text = pileCsv.BoltSizeF ;
      txtBoltCountFlangeSide.Text = pileCsv.BoltNumF.ToString() ;
      cmbBoltSizeWebSide.Text = pileCsv.BoltSizeW ;
      txtBoltCountWebSide.Text = pileCsv.BoltNumW.ToString() ;
      //コーナー
      txtPlateSizeFlangeSideB.Text = cornerPileCsv.PlateSizeF ;
      txtPlateCountFlangeSideB.Text = cornerPileCsv.PlateNumF.ToString() ;
      txtPlateSizeWebSideB1.Text = cornerPileCsv.PlateSizeW ;
      txtPlateCountWebSideB1.Text = cornerPileCsv.PlateNumW.ToString() ;
      txtPlateSizeWebSideB2.Text = cornerPileCsv.PlateSizeW2 ;
      txtPlateCountWebSideB2.Text = cornerPileCsv.PlateNumW2.ToString() ;
      cmbBoltSizeFlangeSideB.Text = cornerPileCsv.BoltSizeF ;
      txtBoltCountFlangeSideB.Text = cornerPileCsv.BoltNumF.ToString() ;
      cmbBoltSizeWebSideB.Text = cornerPileCsv.BoltSizeW ;
      txtBoltCountWebSideB.Text = cornerPileCsv.BoltNumW.ToString() ;

      //コーナー以外2
      txtPlateSizeFlangeSide2.Text = pileCsv2.PlateSizeF ;
      txtPlateCountFlangeSide2.Text = pileCsv2.PlateNumF.ToString() ;
      txtPlateSizeWebSide12.Text = pileCsv2.PlateSizeW ;
      txtPlateCountWebSide12.Text = pileCsv2.PlateNumW.ToString() ;
      txtPlateSizeWebSide22.Text = pileCsv2.PlateSizeW2 ;
      txtPlateCountWebSide22.Text = pileCsv2.PlateNumW2.ToString() ;
      cmbBoltSizeFlangeSide2.Text = pileCsv2.BoltSizeF ;
      txtBoltCountFlangeSide2.Text = pileCsv2.BoltNumF.ToString() ;
      cmbBoltSizeWebSide2.Text = pileCsv2.BoltSizeW ;
      txtBoltCountWebSide2.Text = pileCsv2.BoltNumW.ToString() ;
      //コーナー2
      txtPlateSizeFlangeSideB2.Text = cornerPileCsv2.PlateSizeF ;
      txtPlateCountFlangeSideB2.Text = cornerPileCsv2.PlateNumF.ToString() ;
      txtPlateSizeWebSideB12.Text = cornerPileCsv2.PlateSizeW ;
      txtPlateCountWebSideB12.Text = cornerPileCsv2.PlateNumW.ToString() ;
      txtPlateSizeWebSideB22.Text = cornerPileCsv2.PlateSizeW2 ;
      txtPlateCountWebSideB22.Text = cornerPileCsv2.PlateNumW2.ToString() ;
      cmbBoltSizeFlangeSideB2.Text = cornerPileCsv2.BoltSizeF ;
      txtBoltCountFlangeSideB2.Text = cornerPileCsv2.BoltNumF.ToString() ;
      cmbBoltSizeWebSideB2.Text = cornerPileCsv2.BoltSizeW ;
      txtBoltCountWebSideB2.Text = cornerPileCsv2.BoltNumW.ToString() ;
    }

    public void initComboBoxBoltType()
    {
      // ボルトタイプ
      string selectedTextF = cmbBoltTypeF.Text ;
      cmbBoltTypeF.Items.Clear() ;
      string selectedTextW = cmbBoltTypeW.Text ;
      cmbBoltTypeW.Items.Clear() ;
      string selectedTextFB = cmbBoltTypeFB.Text ;
      cmbBoltTypeFB.Items.Clear() ;
      string selectedTextWB = cmbBoltTypeWB.Text ;
      cmbBoltTypeWB.Items.Clear() ;

      string selectedTextF2 = cmbBoltTypeF2.Text ;
      cmbBoltTypeF2.Items.Clear() ;
      string selectedTextW2 = cmbBoltTypeW2.Text ;
      cmbBoltTypeW2.Items.Clear() ;
      string selectedTextFB2 = cmbBoltTypeFB2.Text ;
      cmbBoltTypeFB2.Items.Clear() ;
      string selectedTextWB2 = cmbBoltTypeWB2.Text ;
      cmbBoltTypeWB2.Items.Clear() ;

      List<string> lstStr = new List<string>()
      {
        Master.ClsBoltCsv.BoltTypeT, Master.ClsBoltCsv.BoltTypeN, Master.ClsBoltCsv.BoltTypeH,
      } ;

      if ( rbnKoteiTypeBolt.Checked ) {
        foreach ( string str in lstStr ) {
          cmbBoltTypeF.Items.Add( str ) ;
          cmbBoltTypeW.Items.Add( str ) ;
          cmbBoltTypeFB.Items.Add( str ) ;
          cmbBoltTypeWB.Items.Add( str ) ;
        }

        if ( lstStr.Contains( selectedTextF ) ) cmbBoltTypeF.Text = selectedTextF ;
        else cmbBoltTypeF.SelectedIndex = 0 ;
        if ( lstStr.Contains( selectedTextW ) ) cmbBoltTypeW.Text = selectedTextW ;
        else cmbBoltTypeW.SelectedIndex = 0 ;
        if ( lstStr.Contains( selectedTextFB ) ) cmbBoltTypeFB.Text = selectedTextFB ;
        else cmbBoltTypeFB.SelectedIndex = 0 ;
        if ( lstStr.Contains( selectedTextWB ) ) cmbBoltTypeWB.Text = selectedTextWB ;
        else cmbBoltTypeWB.SelectedIndex = 0 ;
      }

      if ( rbnKoteiTypeBolt2.Checked ) {
        foreach ( string str in lstStr ) {
          cmbBoltTypeF2.Items.Add( str ) ;
          cmbBoltTypeW2.Items.Add( str ) ;
          cmbBoltTypeFB2.Items.Add( str ) ;
          cmbBoltTypeWB2.Items.Add( str ) ;
        }

        if ( lstStr.Contains( selectedTextF2 ) ) cmbBoltTypeF2.Text = selectedTextF2 ;
        else cmbBoltTypeF2.SelectedIndex = 0 ;
        if ( lstStr.Contains( selectedTextW2 ) ) cmbBoltTypeW2.Text = selectedTextW2 ;
        else cmbBoltTypeW2.SelectedIndex = 0 ;
        if ( lstStr.Contains( selectedTextFB2 ) ) cmbBoltTypeFB2.Text = selectedTextFB2 ;
        else cmbBoltTypeFB2.SelectedIndex = 0 ;
        if ( lstStr.Contains( selectedTextWB2 ) ) cmbBoltTypeWB2.Text = selectedTextWB2 ;
        else cmbBoltTypeWB2.SelectedIndex = 0 ;
      }
    }

    private void DlgCreateKouyaita_Load( object sender, EventArgs e )
    {
      //txtKasho.Text = 2.ToString();
      //cmbShoriType.Items.AddRange(ClsGlobal.m_syorihouhou3.ToArray());
      //cmbCornerYaitaSize.Items.AddRange(ClsGlobal.m_corneryaitasize.ToArray());
      //cmbZanchi.Items.AddRange(ClsGlobal.m_zanti.ToArray());
      //cmbBoltSizeFlange.Items.AddRange(ClsGlobal.m_boltsize.ToArray());
      //comboBox7.Items.AddRange(ClsGlobal.m_boltsize.ToArray());
      //cmbBoltSizeWebSideB.Items.AddRange(ClsGlobal.m_boltsize.ToArray());
      //cmbBoltSizeFlangeSideB.Items.AddRange(ClsGlobal.m_boltsize.ToArray());
      //cmbMaterial.Items.AddRange(ClsGlobal.m_zaishitu.ToArray());

      int nKashoNum = 0 ;
      int.TryParse( txtKashoNum.Text, out nKashoNum ) ;
      InitDataGridView( dgvPileLength, nKashoNum ) ;

      int.TryParse( txtKashoNum2.Text, out nKashoNum ) ;
      InitDataGridView( dgvPileLength2, nKashoNum ) ;
      dgvPileLength2.Enabled = false ;
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
      //if (!int.TryParse(txtKashoNum2.Text, out nKashoNum))
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

    private void txtPileTotalLength_TextChanged( object sender, EventArgs e )
    {
      int nKashoNum = 0 ;
      if ( ! int.TryParse( txtKashoNum.Text, out nKashoNum ) ) {
        return ;
      }

      InitDataGridView( dgvPileLength, nKashoNum ) ;

      if ( ! int.TryParse( txtKashoNum2.Text, out nKashoNum ) ) {
        return ;
      }

      InitDataGridView( dgvPileLength2, nKashoNum ) ;
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

      int remainingLength = ClsCommonUtils.ChangeStrToInt( txtPileTotalLength.Text ) ;
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

    private void cmbSteelType_SelectedIndexChanged( object sender, EventArgs e )
    {
      initComboBoxSteelSize() ;
    }

    private void cmbSteelSize_SelectedIndexChanged( object sender, EventArgs e )
    {
      initComboBoxCornerSize() ;
      initComboBoxBoltType() ;
      initOtherControl() ;
    }

    private void cmbCornerYaitaSize_SelectedIndexChanged( object sender, EventArgs e )
    {
      initComboBoxBoltType() ;
      initOtherControl() ;
    }

    private void rbnKoteiTypeBolt_CheckedChanged( object sender, EventArgs e )
    {
      initComboBoxBoltType() ;
      initOtherControl() ;
      SetKougoType() ;
    }

    private void rbnKoteiTypeYosetsu_CheckedChanged( object sender, EventArgs e )
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

    private void rbnKoteiTypeYosetsu2_CheckedChanged( object sender, EventArgs e )
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

    private void cmbBoltTypeFB_SelectedIndexChanged( object sender, EventArgs e )
    {
      initOtherControl() ;
    }

    private void cmbBoltTypeWB_SelectedIndexChanged( object sender, EventArgs e )
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

    private void cmbBoltTypeFB2_SelectedIndexChanged( object sender, EventArgs e )
    {
      initOtherControl() ;
    }

    private void cmbBoltTypeWB2_SelectedIndexChanged( object sender, EventArgs e )
    {
      initOtherControl() ;
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
      int kuiTotal = ClsCommonUtils.ChangeStrToInt( txtPileTotalLength.Text ) ;
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
      for ( int i = 0 ; i < dgvPileLength.Columns.Count ; i++ ) {
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

      if ( ClsCommonUtils.ChangeStrToInt( txtPileTotalLength.Text ) != total )
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


    public void SetData()
    {
      ClsKouyaita cki = m_ClsKouyaita ;
      cki.m_case = ClsCommonUtils.ChangeStrToInt( txtCase.Text ) ;
      cki.m_edaNum = txtEdaNumber.Text ;
      cki.m_edaNum2 = tabPage2.Text ;
      cki.m_way = cmbShoriType.SelectedIndex ;
      cki.m_void = ClsCommonUtils.ChangeStrToInt( txtKussakuFukasa.Text ) ;
      cki.m_putVec = rbnHaishiMukiKougo.Checked == true ? PutVec.Kougo : PutVec.Const ;
      cki.m_type = cmbSteelType.Text ;
      cki.m_size = cmbSteelSize.Text ;
      cki.m_HTop = ClsCommonUtils.ChangeStrToInt( txtPileTop.Text ) ;
      cki.m_HLen = ClsCommonUtils.ChangeStrToInt( txtPileTotalLength.Text ) ;
      cki.m_kouyaitaSize = cmbCornerYaitaSize.Text ;
      cki.m_zaishitu = cmbMaterial.Text ;
      cki.m_bIzyou = chkIkeiYaita.Checked ;
      cki.m_zanti = cmbZanchi.Text ;
      cki.m_zantiLength = txtZantiLength.Text ;

      cki.m_KougoFlg = chkKougoHaichi.Checked ;
      cki.m_Kasho1 = ClsCommonUtils.ChangeStrToInt( txtKashoNum.Text ) ;
      cki.m_Kasho2 = ClsCommonUtils.ChangeStrToInt( txtKashoNum2.Text ) ;
      cki.m_Kotei1 = rbnKoteiTypeBolt.Checked ? Master.Kotei.Bolt : Master.Kotei.Yousetsu ;
      cki.m_Kotei2 = rbnKoteiTypeBolt2.Checked ? Master.Kotei.Bolt : Master.Kotei.Yousetsu ;
      cki.TensetuVec1 = rbnTensetsuHaraSide.Checked ? TensetuVec.HaraSide : TensetuVec.SenakaSide ;
      cki.TensetuVec2 = rbnTensetsuHaraSide2.Checked ? TensetuVec.HaraSide : TensetuVec.SenakaSide ;

      List<int> lstN1 = new List<int>() ;
      for ( int i = 0 ; i < dgvPileLength.Columns.Count ; i++ ) {
        lstN1.Add( ClsCommonUtils.ChangeStrToInt( dgvPileLength[ i, 0 ].Value.ToString() ) ) ;
      }

      cki.m_ListPileLength1 = lstN1 ;
      List<int> lstN2 = new List<int>() ;
      for ( int i = 0 ; i < dgvPileLength2.Columns.Count ; i++ ) {
        lstN2.Add( ClsCommonUtils.ChangeStrToInt( dgvPileLength2[ i, 0 ].Value.ToString() ) ) ;
      }

      cki.m_ListPileLength2 = lstN2 ;

      cki.m_BoltF1 = cmbBoltSizeFlangeSide.Text ;
      cki.m_BoltFNum1 = txtBoltCountFlangeSide.Text ;
      cki.m_BoltW1 = cmbBoltSizeWebSide.Text ;
      cki.m_BoltWNum1 = txtBoltCountWebSide.Text ;

      cki.m_BoltCornerF1 = cmbBoltSizeFlangeSideB.Text ;
      cki.m_BoltCornerFNum1 = txtBoltCountFlangeSideB.Text ;
      cki.m_BoltCornerW1 = cmbBoltSizeWebSideB.Text ;
      cki.m_BoltCornerWNum1 = txtBoltCountWebSideB.Text ;

      cki.m_BoltF2 = cmbBoltSizeFlangeSide2.Text ;
      cki.m_BoltFNum2 = txtBoltCountFlangeSide2.Text ;
      cki.m_BoltW2 = cmbBoltSizeWebSide2.Text ;
      cki.m_BoltWNum2 = txtBoltCountWebSide2.Text ;

      cki.m_BoltCornerF2 = cmbBoltSizeFlangeSideB2.Text ;
      cki.m_BoltCornerFNum2 = txtBoltCountFlangeSideB2.Text ;
      cki.m_BoltCornerW2 = cmbBoltSizeWebSideB2.Text ;
      cki.m_BoltCornerWNum2 = txtBoltCountWebSideB2.Text ;
      //メンバ変数に格納
      m_ClsKouyaita = cki ;
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
      if ( ! ClsCommonUtils.ChkContorolTextDouble( txtPileTop, "天端", bMinus: true ) )
        return false ;
      if ( ! ClsCommonUtils.ChkContorolTextDouble( txtPileTotalLength, "全長", bMinus: true, bZero: true ) )
        return false ;

      if ( ! ClsCommonUtils.ChkContorolTextDouble( txtKashoNum, "箇所数(1)", bMinus: true, bComma: true ) )
        return false ;
      if ( ! ClsCommonUtils.ChkContorolTextDouble( txtKashoNum2, "箇所数(2)", bMinus: true, bComma: true ) )
        return false ;
      if ( ! ClsCommonUtils.ChkContorolTextDouble( txtBoltCountFlangeSide, "ボルト本数(1)（フランジ側）", bMinus: true,
            bComma: true ) )
        return false ;
      if ( ! ClsCommonUtils.ChkContorolTextDouble( txtBoltCountFlangeSideB, "ボルト本数B(1)（フランジ側）", bMinus: true,
            bComma: true ) )
        return false ;
      if ( ! ClsCommonUtils.ChkContorolTextDouble( txtBoltCountFlangeSide2, "ボルト本数(2)（フランジ側）", bMinus: true,
            bComma: true ) )
        return false ;
      if ( ! ClsCommonUtils.ChkContorolTextDouble( txtBoltCountFlangeSideB2, "ボルト本数B(2)（フランジ側）", bMinus: true,
            bComma: true ) )
        return false ;

      if ( ! ClsCommonUtils.ChkContorolTextDouble( txtBoltCountWebSide, "ボルト本数(1)（Web側）", bMinus: true, bComma: true ) )
        return false ;
      if ( ! ClsCommonUtils.ChkContorolTextDouble( txtBoltCountWebSideB, "ボルト本数B(1)（Web側）", bMinus: true,
            bComma: true ) )
        return false ;
      if ( ! ClsCommonUtils.ChkContorolTextDouble( txtBoltCountWebSide2, "ボルト本数(2)（Web側）", bMinus: true,
            bComma: true ) )
        return false ;
      if ( ! ClsCommonUtils.ChkContorolTextDouble( txtBoltCountWebSideB2, "ボルト本数B(2)（Web側）", bMinus: true,
            bComma: true ) )
        return false ;

      //杭の全長チェック
      List<int> lstN1 = new List<int>() ;
      for ( int i = 0 ; i < dgvPileLength.Columns.Count ; i++ ) {
        lstN1.Add( ClsCommonUtils.ChangeStrToInt( dgvPileLength[ i, 0 ].Value.ToString() ) ) ;
      }

      ;
      if ( lstN1.Sum() != ( ClsCommonUtils.ChangeStrToInt( txtPileTotalLength.Text ) ) ) {
        MessageBox.Show( "杭の長さの合計と全長が一致しません。[タブ1]" ) ;
        return false ;
      }

      if ( chkKougoHaichi.Checked ) {
        List<int> lstN2 = new List<int>() ;
        for ( int i = 0 ; i < dgvPileLength2.Columns.Count ; i++ ) {
          lstN2.Add( ClsCommonUtils.ChangeStrToInt( dgvPileLength2[ i, 0 ].Value.ToString() ) ) ;
        }

        if ( lstN2.Sum() != ( ClsCommonUtils.ChangeStrToInt( txtPileTotalLength.Text ) ) ) {
          MessageBox.Show( "杭の長さの合計と全長が一致しません。[タブ2]" ) ;
          return false ;
        }
      }

      return true ;
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
      ClsIni.WritePrivateProfileString( sec, rbnHaishiMukiIttei.Name, rbnHaishiMukiIttei.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, rbnHaishiMukiKougo.Name, rbnHaishiMukiKougo.Checked.ToString(), iniPath ) ;

      ClsIni.WritePrivateProfileString( sec, cmbSteelType.Name, cmbSteelType.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, cmbSteelSize.Name, cmbSteelSize.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, txtPileTop.Name, txtPileTop.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, txtPileTotalLength.Name, txtPileTotalLength.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, cmbCornerYaitaSize.Name, cmbCornerYaitaSize.Text, iniPath ) ;

      ClsIni.WritePrivateProfileString( sec, cmbMaterial.Name, cmbMaterial.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, chkIkeiYaita.Name, chkIkeiYaita.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, cmbZanchi.Name, cmbZanchi.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, txtZantiLength.Name, txtZantiLength.Text, iniPath ) ;

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
      //string nextCase = string.Empty;
      //txtEdaNumber.Text = ClsIni.GetIniFile(sec, txtEdaNumber.Name, iniPath);
      cmbShoriType.Text = ClsIni.GetIniFile( sec, cmbShoriType.Name, iniPath ) ;
      txtKussakuFukasa.Text = ClsIni.GetIniFile( sec, txtKussakuFukasa.Name, iniPath ) ;
      rbnHaishiMukiIttei.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnHaishiMukiIttei.Name, iniPath ) ) ;
      rbnHaishiMukiKougo.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnHaishiMukiKougo.Name, iniPath ) ) ;

      cmbSteelType.Text = ClsIni.GetIniFile( sec, cmbSteelType.Name, iniPath ) ;
      cmbSteelSize.Text = ClsIni.GetIniFile( sec, cmbSteelSize.Name, iniPath ) ;
      txtPileTop.Text = ClsIni.GetIniFile( sec, txtPileTop.Name, iniPath ) ;
      txtPileTotalLength.Text = ClsIni.GetIniFile( sec, txtPileTotalLength.Name, iniPath ) ;
      cmbCornerYaitaSize.Text = ClsIni.GetIniFile( sec, cmbCornerYaitaSize.Name, iniPath ) ;

      cmbMaterial.Text = ClsIni.GetIniFile( sec, cmbMaterial.Name, iniPath ) ;
      chkIkeiYaita.Checked = bool.Parse( ClsIni.GetIniFile( sec, chkIkeiYaita.Name, iniPath ) ) ;
      cmbZanchi.Text = ClsIni.GetIniFile( sec, cmbZanchi.Name, iniPath ) ;
      txtZantiLength.Text = ClsIni.GetIniFile( sec, txtZantiLength.Name, iniPath ) ;

      chkKougoHaichi.Checked = bool.Parse( ClsIni.GetIniFile( sec, chkKougoHaichi.Name, iniPath ) ) ;

      txtKashoNum.Text = ClsIni.GetIniFile( sec, txtKashoNum.Name, iniPath ) ;
      rbnKoteiTypeBolt.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnKoteiTypeBolt.Name, iniPath ) ) ;
      rbnKoteiTypeYosetsu.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnKoteiTypeYosetsu.Name, iniPath ) ) ;
      txtKashoNum2.Text = ClsIni.GetIniFile( sec, txtKashoNum2.Name, iniPath ) ;
      rbnKoteiTypeBolt2.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnKoteiTypeBolt2.Name, iniPath ) ) ;
      rbnKoteiTypeYosetsu2.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnKoteiTypeYosetsu2.Name, iniPath ) ) ;
    }

    /// <summary>
    /// メンバの値をコントロールにセット
    /// </summary>
    private void SetControl()
    {
      ClsKouyaita cki = m_ClsKouyaita ;
      txtCase.Text = cki.m_case.ToString() ;
      txtEdaNumber.Text = cki.m_edaNum ;

      //cki.m_way = cmbShoriType.SelectedIndex;
      txtKussakuFukasa.Text = cki.m_void.ToString() ;


      cmbZanchi.Text = cki.m_zanti ;
      txtZantiLength.Text = cki.m_zantiLength ;

      cmbSteelType.Text = cki.m_type ;
      cmbSteelSize.Text = cki.m_size ;
      txtPileTop.Text = cki.m_HTop.ToString() ;
      txtPileTotalLength.Text = cki.m_HLen.ToString() ;

      //グリッドビュー
      //cki.m_KougoFlg = chkKougoHaichi.Checked;
      txtKashoNum.Text = cki.m_Kasho1.ToString() ;
      //cki.m_Kasho2 = ClsCommonUtils.ChangeStrToInt(txtKashoNum2.Text);

      List<int> lstN1 = cki.m_ListPileLength1 ;
      for ( int i = 0 ; i < lstN1.Count ; i++ ) {
        dgvPileLength[ i, 0 ].Value = lstN1[ i ].ToString() ;
      }
      //List<int> lstN2 = new List<int>();
      //for (int i = 0; i < dgvPileLength2.Columns.Count; i++)
      //{
      //    lstN2.Add(ClsCommonUtils.ChangeStrToInt(dgvPileLength2[i, 0].Value.ToString()));
      //}
      //cki.m_ListPileLength2 = lstN2;
    }

    private void cmbZanchi_SelectedIndexChanged( object sender, EventArgs e )
    {
      if ( cmbZanchi.SelectedItem.ToString() == "一部埋め殺し" ) {
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
      enCASEHit enCASEHit = clsCASE.ExistCase( enKabeshu.koyauita, txtCase.Text, ref nextCASE ) ;
      if ( enCASEHit == enCASEHit.sameKabeshu ) {
        foreach ( stCASEKoyaita koyauita in clsCASE.m_koyaitaDataList ) {
          if ( koyauita.CASE == txtCase.Text ) {
            cmbSteelType.Text = koyauita.buzaiType ;
            cmbSteelSize.Text = koyauita.buzaiSize ;
            txtPileTotalLength.Text = koyauita.TotalLength.ToString() ;
            cmbMaterial.Text = koyauita.zaishitsu ;
            cmbZanchi.Text = koyauita.zanchi ;
            txtZantiLength.Text = koyauita.zanchiLength ;
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
      enCASE_EdabanHit enCASE_EdabanHit = clsCASE.ExistCaseEdaban( enKabeshu.koyauita, txtCase.Text, txtEdaNumber.Text,
        ref nextCASE, ref nextEdaban ) ;
      if ( enCASE_EdabanHit == enCASE_EdabanHit.sameKabeshu ) {
        foreach ( stCASEKoyaita koyauita in clsCASE.m_koyaitaDataList ) {
          if ( koyauita.CASE == txtCase.Text && koyauita.Edaban == txtEdaNumber.Text ) {
            txtKashoNum.Text = koyauita.JointNum.ToString() ;
            if ( dgvPileLength.Columns.Count == koyauita.KuiLengthList.Count && 0 < dgvPileLength.Rows.Count ) {
              for ( int i = 0 ; i < dgvPileLength.Columns.Count ; i++ ) {
                dgvPileLength[ i, 0 ].Value = koyauita.KuiLengthList[ i ] ;
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
      cmbSteelType.Enabled = set ;
      cmbSteelSize.Enabled = set ;
      txtPileTotalLength.Enabled = set ;
      cmbMaterial.Enabled = set ;
      cmbZanchi.Enabled = set ;
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
          clsCASE.ExistCaseEdaban( enKabeshu.koyauita, txtCase.Text, edaban, ref nextCASE, ref nextEdaban ) ;
        bool use = true ;
        if ( enCASE_EdabanHit == enCASE_EdabanHit.sameKabeshu ) {
          foreach ( stCASEKoyaita koyauita in clsCASE.m_koyaitaDataList ) {
            if ( koyauita.CASE == txtCase.Text && koyauita.Edaban == edaban ) {
              if ( txtKashoNum.Text == koyauita.JointNum.ToString() ) {
                if ( dgvPileLength.Columns.Count == koyauita.KuiLengthList.Count && 0 < dgvPileLength.Rows.Count ) {
                  for ( int i = 0 ; i < dgvPileLength.Columns.Count ; i++ ) {
                    if ( (int) dgvPileLength[ i, 0 ].Value !=
                         koyauita.KuiLengthList[ koyauita.KuiLengthList.Count - 1 - i ] ) {
                      use = false ;
                      break ;
                    }
                  }
                }
              }

              if ( use ) {
                tabPage2.Text = edaban ;
                txtKashoNum2.Text = koyauita.JointNum.ToString() ;
                if ( dgvPileLength2.Columns.Count == koyauita.KuiLengthList.Count && 0 < dgvPileLength2.Rows.Count ) {
                  for ( int i = 0 ; i < dgvPileLength.Columns.Count ; i++ ) {
                    dgvPileLength2[ i, 0 ].Value = koyauita.KuiLengthList[ i ] ;
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
          string nextEdaNum = clsCASE.GetEdabanAlphabet( enKabeshu.koyauita, txtCase.Text, txtEdaNumber.Text ) ;
          tabPage2.Text = nextEdaNum ;
        }
      }
    }
  }
}