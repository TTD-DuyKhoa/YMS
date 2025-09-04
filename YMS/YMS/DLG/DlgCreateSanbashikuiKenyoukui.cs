using Autodesk.Revit.DB ;
using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.Drawing ;
using System.Linq ;
using System.Reflection.Emit ;
using System.Windows.Forms ;
using YMS.Parts ;
using static YMS.Parts.ClsSanbashiKui ;

namespace YMS.DLG
{
  public partial class DlgCreateSanbashikuiKenyoukui : System.Windows.Forms.Form
  {
    private Bitmap originalImage ;
    private bool m_ChangeDGVKuiLength = false ;

    #region 定数

    /// <summary>
    /// Iniファイルのパス
    /// </summary>
    const string iniPath = "ini\\DlgCreateSanbashikuiKenyoukui.ini" ;

    /// <summary>
    /// Iniファイルのセクション名
    /// </summary>
    const string sec = "DlgCreateSanbashikuiKenyoukui" ;

    #endregion

    #region メンバー変数

    public Document m_doc ;
    public ClsSanbashiKui m_ClsKui ;
    public string m_Level { get ; set ; }

    #endregion

    public DlgCreateSanbashikuiKenyoukui( Document doc )
    {
      InitializeComponent() ;
      originalImage = new Bitmap( pictureBox1.Image ) ;

      m_doc = doc ;
      m_ClsKui = new ClsSanbashiKui() ;
      m_Level = string.Empty ;
      InitControl() ;
      GetIniData() ;
    }

    public DlgCreateSanbashikuiKenyoukui( Document doc, ClsSanbashiKui clsSanbashiKui )
    {
      InitializeComponent() ;
      originalImage = new Bitmap( pictureBox1.Image ) ;

      m_doc = doc ;
      m_ClsKui = clsSanbashiKui ;
      m_Level = string.Empty ;
      InitControl() ;
      SetControl() ;
    }

    private void InitControl()
    {
      //GLレベル
      cmbLevel.Items.Clear() ;
      cmbLevel.Items.AddRange( ClsYMSUtil.GetLevelNames( m_doc ).ToArray() ) ;
      cmbLevel.SelectedIndex = 0 ;

      // 作成方法
      rbnKuiTenba.Checked = true ;

      // 杭タイプ
      rbnPileTypeSanbashi.Checked = true ;

      // 配置方法
      cmbShoriType.Items.Add( "1点" ) ;
      cmbShoriType.Items.Add( "交点" ) ;
      cmbShoriType.SelectedIndex = 0 ;

      // 杭全長
      txtPileTotalLength.Text = "10000" ;
      txtPileTotalLength_Top.Text = "0" ;
      txtPileTotalLength_Bottom.Text = "0" ;

      // 配置角度
      txtHaichiAngle.Text = "90" ;

      // 残置
      cmbZanchi.Items.AddRange( ClsGlobal.m_zanti.ToArray() ) ;
      cmbZanchi.SelectedIndex = 0 ;
      txtZantiLength.Text = "0" ;

      // トッププレートの取付
      rbnTopPlateOff.Checked = true ;

      // 継手箇所数
      txtJointCount.Text = "2" ;
      txtJointCount2.Text = "2" ;

      // 継手固定方法
      rbnKoteiTypeBolt.Checked = true ;
      rbnKoteiTypeBolt2.Checked = true ;

      // 杭の分割長さ
      if ( rbnPileTypeDanmenHenka.Checked ) {
        int.TryParse( txtJointCount.Text, out int nJointCount ) ;
        InitDataGridView( dgvPileLength, nJointCount ) ;
        int.TryParse( txtJointCount2.Text, out int nJointCount2 ) ;
        InitDataGridView( dgvPileLength2, nJointCount2 ) ;
      }
      else {
        int.TryParse( txtJointCount.Text, out int nJointCount ) ;
        InitDataGridView( dgvPileLength, nJointCount ) ;
      }

      // 継手プレート
      InitTsugitePlate() ;

      InitComboBox() ;
    }

    private void InitComboBox()
    {
      // 鋼材タイプ
      InitComboBoxSteelType() ;

      // 鋼材サイズ
      if ( rbnPileTypeDanmenHenka.Checked ) {
        InitComboBoxSteelSize_KoudaiKui() ;
        InitTxtBoxSteelSize_DanmenHenkaKui() ;
      }
      else {
        InitComboBoxSteelSize() ;
      }

      // トッププレートのサイズ
      InitComboBoxTopPlateSize() ;

      // エンドプレート厚
      InitComboBoxEndPlateThickness() ;
    }

    private void InitComboBoxSteelType()
    {
      // 鋼材タイプ
      string selectedText = cmbSteelType.Text ;
      cmbSteelType.Items.Clear() ;
      List<string> lstStr = new List<string>() ;
      lstStr = new List<string>()
      {
        Master.ClsHBeamCsv.TypeHiro, Master.ClsHBeamCsv.TypeNaka, Master.ClsHBeamCsv.TypeHoso,
      } ;
      foreach ( string str in lstStr ) {
        cmbSteelType.Items.Add( str ) ;
      }

      if ( cmbSteelType.Items.Count > 0 ) {
        if ( lstStr.Contains( selectedText ) ) {
          cmbSteelType.Text = selectedText ;
        }
        else {
          cmbSteelType.SelectedIndex = 0 ;
        }
      }
    }

    private void InitComboBoxSteelSize()
    {
      // 鋼材サイズ
      if ( cmbSteelSize != null ) {
        string selectedText = cmbSteelSize.Text ;
        cmbSteelSize.Items.Clear() ;
        List<string> lstStr = new List<string>() ;
        switch ( cmbSteelType.Text ) {
          case Master.ClsHBeamCsv.TypeHiro :
            lstStr = Master.ClsHBeamCsv.GetSizeList( Master.ClsHBeamCsv.TypeHiro ) ;
            break ;
          case Master.ClsHBeamCsv.TypeNaka :
            lstStr = Master.ClsHBeamCsv.GetSizeList( Master.ClsHBeamCsv.TypeNaka ) ;
            break ;
          case Master.ClsHBeamCsv.TypeHoso :
            lstStr = Master.ClsHBeamCsv.GetSizeList( Master.ClsHBeamCsv.TypeHoso ) ;
            break ;
          default :
            return ;
        }

        foreach ( string str in lstStr ) {
          cmbSteelSize.Items.Add( str ) ;
        }

        if ( cmbSteelSize.Items.Count > 0 ) {
          if ( lstStr.Contains( selectedText ) ) {
            cmbSteelSize.Text = selectedText ;
          }
          else {
            cmbSteelSize.SelectedIndex = 0 ;
          }
        }
      }
    }

    private void InitComboBoxSteelSize_KoudaiKui()
    {
      if ( cmbSteelSize != null ) {
        string selectedText = cmbSteelSize.Text ;
        cmbSteelSize.Items.Clear() ;
        List<string> lstStr = new List<string>() ;
        lstStr = Master.ClsDanmenHenkaKuiCsv.GetSizeList_Koudai() ;

        foreach ( string str in lstStr ) {
          cmbSteelSize.Items.Add( str ) ;
        }

        if ( cmbSteelSize.Items.Count > 0 ) {
          if ( lstStr.Contains( selectedText ) ) {
            cmbSteelSize.Text = selectedText ;
          }
          else {
            cmbSteelSize.SelectedIndex = 0 ;
          }
        }
      }
    }

    private void InitTxtBoxSteelSize_DanmenHenkaKui()
    {
      string selectedText = txtSteelSize.Text ;
      txtSteelSize.Clear() ;
      txtSteelSize.Text = Master.ClsDanmenHenkaKuiCsv.GetSize2( cmbSteelSize.Text ) ;
    }

    private void InitComboBoxTopPlateSize()
    {
      if ( cmbTopPlateSizeTeigata != null ) {
        string selectedText = cmbTopPlateSizeTeigata.Text ;
        cmbTopPlateSizeTeigata.Items.Clear() ;
        List<string> lstStr = new List<string>() ;
        lstStr = Master.ClsTopPlateCSV.GetTypeNameList() ;

        foreach ( string str in lstStr ) {
          if ( str == "" ) continue ;
          cmbTopPlateSizeTeigata.Items.Add( str ) ;
        }

        if ( cmbTopPlateSizeTeigata.Items.Count > 0 ) {
          if ( lstStr.Contains( selectedText ) && selectedText != "" ) {
            cmbTopPlateSizeTeigata.Text = selectedText ;
          }
          else {
            cmbTopPlateSizeTeigata.SelectedIndex = 0 ;
          }
        }
      }
    }

    private void InitComboBoxEndPlateThickness()
    {
      if ( cmbEndPlateThickness != null ) {
        string selectedText = cmbEndPlateThickness.Text ;
        cmbEndPlateThickness.Items.Clear() ;
        List<string> lstStr = new List<string>() ;
        lstStr = Master.ClsEndPlateCSV.GetSizeList() ;

        foreach ( string str in lstStr ) {
          if ( str == "" ) continue ;
          cmbEndPlateThickness.Items.Add( str ) ;
        }

        if ( cmbEndPlateThickness.Items.Count > 0 ) {
          if ( lstStr.Contains( selectedText ) && selectedText != "" ) {
            cmbEndPlateThickness.Text = selectedText ;
          }
          else {
            cmbEndPlateThickness.SelectedIndex = 0 ;
          }
        }
      }

      if ( rbnPileTypeDanmenHenka.Checked == true ) {
        cmbEndPlateThickness.Enabled = true ;
      }
      else {
        cmbEndPlateThickness.Enabled = false ;
      }
    }

    private void InitTabControl()
    {
      if ( rbnPileTypeDanmenHenka.Checked ) {
        tabControl1.TabPages[ 0 ].Text = "上杭" ;
        tabControl1.TabPages[ 1 ].Text = "下杭" ;
        foreach ( System.Windows.Forms.Control control in tabControl1.TabPages[ 1 ].Controls ) {
          control.Enabled = true ;
        }

        tabControl1.TabPages[ 1 ].Show() ;

        lblSteelType.Enabled = false ;
        cmbSteelType.Enabled = false ;

        lblKouzaiSize.Text = "鋼材ｻｲｽﾞ(構台杭)" ;
        lblKouzaiSize2.Visible = true ;
        lblKouzaiSize2.Text = "鋼材ｻｲｽﾞ(断面変化杭)" ;
        lblKouzaiSize2.Enabled = true ;
        txtSteelSize.Enabled = true ;
        txtSteelSize.Visible = true ;

        cmbEndPlateThickness.Enabled = true ;

        lblTotalLength_Top.Visible = true ;
        txtPileTotalLength_Top.Visible = true ;
        lblTotalLength_Bottom.Visible = true ;
        txtPileTotalLength_Bottom.Visible = true ;

        txtPileTotalLength.Enabled = false ;
        double pileHalfLengt = double.Parse( txtPileTotalLength.Text ) / 2 ;
        txtPileTotalLength_Top.Text = pileHalfLengt.ToString() ;
        txtPileTotalLength_Bottom.Text = pileHalfLengt.ToString() ;

        InitComboBoxSteelSize_KoudaiKui() ;
        InitTxtBoxSteelSize_DanmenHenkaKui() ;

        if ( ! int.TryParse( txtJointCount.Text, out int nKashoNum ) ) {
          return ;
        }

        if ( ! int.TryParse( txtJointCount2.Text, out int nKashoNum2 ) ) {
          return ;
        }

        InitDataGridView( dgvPileLength, nKashoNum ) ;
        InitDataGridView( dgvPileLength2, nKashoNum2 ) ;
      }
      else {
        tabControl1.TabPages[ 0 ].Text = " " ;
        tabControl1.TabPages[ 1 ].Text = " " ;
        foreach ( System.Windows.Forms.Control control in tabControl1.TabPages[ 1 ].Controls ) {
          control.Enabled = false ;
        }

        tabControl1.SelectedTab = tabPage1 ;

        lblSteelType.Enabled = true ;
        cmbSteelType.Enabled = true ;

        lblKouzaiSize.Text = "鋼材サイズ" ;
        lblKouzaiSize2.Visible = false ;
        lblKouzaiSize2.Enabled = false ;
        lblKouzaiSize2.Text = "鋼材サイズ" ;
        txtSteelSize.Visible = false ;
        txtSteelSize.Enabled = false ;

        cmbEndPlateThickness.Enabled = false ;

        lblTotalLength_Top.Visible = false ;
        txtPileTotalLength_Top.Visible = false ;
        lblTotalLength_Bottom.Visible = false ;
        txtPileTotalLength_Bottom.Visible = false ;

        txtPileTotalLength.Enabled = true ;
        txtPileTotalLength.Text = "10000" ;

        InitComboBoxSteelSize() ;
        txtSteelSize.Clear() ;

        if ( ! int.TryParse( txtJointCount.Text, out int nKashoNum ) ) {
          return ;
        }

        InitDataGridView( dgvPileLength, nKashoNum ) ;
      }
    }

    private void InitDataGridView( DataGridView dgv, int nColmnCount, bool isTop = true )
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

      int remainingLength = 0 ;
      if ( rbnPileTypeDanmenHenka.Checked ) {
        if ( isTop ) {
          remainingLength = ClsCommonUtils.ChangeStrToInt( txtPileTotalLength_Top.Text ) ;
        }
        else {
          remainingLength = ClsCommonUtils.ChangeStrToInt( txtPileTotalLength_Bottom.Text ) ;
        }
      }
      else {
        remainingLength = ClsCommonUtils.ChangeStrToInt( txtPileTotalLength.Text ) ;
      }

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

    private void InitTsugitePlate()
    {
      string strKotei = ( rbnKoteiTypeBolt.Checked
        ? Master.ClsPileTsugiteCsv.KoteiHohoBolt
        : Master.ClsPileTsugiteCsv.KoteiHohoYousetsu ) ;
      Master.ClsPileTsugiteCsv pileCsv = Master.ClsPileTsugiteCsv.GetCls( strKotei, cmbSteelSize.Text ) ;

      // 継手プレート
      txtPlateSizeFlangeOutSide.Text = pileCsv.PlateSizeFOut ;
      txtPlateCountFlangeOutSide.Text = pileCsv.PlateNumFOut.ToString() ;
      txtPlateSizeFlangeInSide.Text = pileCsv.PlateSizeFIn ;
      txtPlateCountFlangeInSide.Text = pileCsv.PlateNumFIn.ToString() ;
      txtPlateSizeWebSide1.Text = pileCsv.PlateSizeW ;
      txtPlateCountWebSide1.Text = pileCsv.PlateNumW.ToString() ;
      txtPlateSizeWebSide2.Text = pileCsv.PlateSizeW2 ;
      txtPlateCountWebSide2.Text = pileCsv.PlateNumW2.ToString() ;

      // 継手ボルト
      InitComboBoxBoltTypeF() ;
      cmbBoltTypeF.Text = Master.ClsBoltCsv.GetType( pileCsv.BoltSizeF ) ;
      InitComboBoxBoltFlangeSize() ;
      cmbBoltSizeFlangeSide.Text = pileCsv.BoltSizeF ;
      txtBoltCountFlangeSide.Text = pileCsv.BoltNumF.ToString() ;
      InitComboBoxBoltTypeW() ;
      cmbBoltTypeW.Text = Master.ClsBoltCsv.GetType( pileCsv.BoltSizeW ) ;
      InitComboBoxBoltWebSize() ;
      cmbBoltSizeWebSide.Text = pileCsv.BoltSizeW ;
      txtBoltCountWebSide.Text = pileCsv.BoltNumW.ToString() ;

      cmbBoltSizeFlangeSide.Enabled = rbnKoteiTypeBolt.Checked ;
      txtBoltCountFlangeSide.Enabled = rbnKoteiTypeBolt.Checked ;
      cmbBoltSizeWebSide.Enabled = rbnKoteiTypeBolt.Checked ;
      txtBoltCountWebSide.Enabled = rbnKoteiTypeBolt.Checked ;
    }

    public void InitComboBoxBoltTypeF()
    {
      // ボルトタイプ
      string selectedText = cmbBoltTypeF.Text ;
      cmbBoltTypeF.Items.Clear() ;
      List<string> lstStr = new List<string>() ;
      lstStr = new List<string>()
      {
        Master.ClsBoltCsv.BoltTypeN, Master.ClsBoltCsv.BoltTypeH, Master.ClsBoltCsv.BoltTypeT,
      } ;
      cmbBoltTypeF.Items.Add( "" ) ;
      foreach ( string str in lstStr ) {
        cmbBoltTypeF.Items.Add( str ) ;
      }

      if ( cmbBoltTypeF.Items.Count > 0 ) {
        if ( lstStr.Contains( selectedText ) ) {
          cmbBoltTypeF.Text = selectedText ;
        }
        else {
          cmbBoltTypeF.SelectedIndex = 0 ;
        }
      }

      if ( rbnKoteiTypeBolt.Enabled == true && rbnKoteiTypeBolt.Checked == true ) {
        cmbBoltTypeF.Enabled = true ;
        cmbBoltTypeF.Enabled = true ;
      }
      else {
        cmbBoltTypeF.Enabled = false ;
        cmbBoltTypeF.Enabled = false ;
      }
    }

    private void InitComboBoxBoltFlangeSize()
    {
      if ( cmbBoltSizeFlangeSide != null ) {
        string selectedText = cmbBoltSizeFlangeSide.Text ;
        cmbBoltSizeFlangeSide.Items.Clear() ;
        List<string> lstStr = new List<string>() ;
        switch ( cmbBoltTypeF.Text ) {
          case Master.ClsBoltCsv.BoltTypeN :
          case Master.ClsBoltCsv.BoltTypeH :
          case Master.ClsBoltCsv.BoltTypeT :
            lstStr = Master.ClsBoltCsv.GetSizeList( cmbBoltTypeF.Text ) ;
            break ;
          default :
            return ;
        }

        cmbBoltSizeFlangeSide.Items.Add( "" ) ;
        foreach ( string str in lstStr ) {
          cmbBoltSizeFlangeSide.Items.Add( str ) ;
        }

        if ( cmbBoltSizeFlangeSide.Items.Count > 0 ) {
          if ( lstStr.Contains( selectedText ) ) {
            cmbBoltSizeFlangeSide.Text = selectedText ;
          }
          else {
            cmbBoltSizeFlangeSide.SelectedIndex = 0 ;
          }
        }
      }
    }

    public void InitComboBoxBoltTypeW()
    {
      // ボルトタイプ
      string selectedText = cmbBoltTypeW.Text ;
      cmbBoltTypeW.Items.Clear() ;
      List<string> lstStr = new List<string>() ;
      lstStr = new List<string>()
      {
        Master.ClsBoltCsv.BoltTypeN, Master.ClsBoltCsv.BoltTypeH, Master.ClsBoltCsv.BoltTypeT,
      } ;
      cmbBoltTypeW.Items.Add( "" ) ;
      foreach ( string str in lstStr ) {
        cmbBoltTypeW.Items.Add( str ) ;
      }

      if ( cmbBoltTypeW.Items.Count > 0 ) {
        if ( lstStr.Contains( selectedText ) ) {
          cmbBoltTypeW.Text = selectedText ;
        }
        else {
          cmbBoltTypeW.SelectedIndex = 0 ;
        }
      }

      if ( rbnKoteiTypeBolt.Enabled == true && rbnKoteiTypeBolt.Checked == true ) {
        cmbBoltTypeW.Enabled = true ;
        cmbBoltTypeW.Enabled = true ;
      }
      else {
        cmbBoltTypeW.Enabled = false ;
        cmbBoltTypeW.Enabled = false ;
      }
    }

    private void InitComboBoxBoltWebSize()
    {
      if ( cmbBoltSizeWebSide != null ) {
        string selectedText = cmbBoltSizeWebSide.Text ;
        cmbBoltSizeWebSide.Items.Clear() ;
        List<string> lstStr = new List<string>() ;
        switch ( cmbBoltTypeW.Text ) {
          case Master.ClsBoltCsv.BoltTypeN :
          case Master.ClsBoltCsv.BoltTypeH :
          case Master.ClsBoltCsv.BoltTypeT :
            lstStr = Master.ClsBoltCsv.GetSizeList( cmbBoltTypeW.Text ) ;
            break ;
          default :
            return ;
        }

        cmbBoltSizeWebSide.Items.Add( "" ) ;
        foreach ( string str in lstStr ) {
          cmbBoltSizeWebSide.Items.Add( str ) ;
        }

        if ( cmbBoltSizeWebSide.Items.Count > 0 ) {
          if ( lstStr.Contains( selectedText ) ) {
            cmbBoltSizeWebSide.Text = selectedText ;
          }
          else {
            cmbBoltSizeWebSide.SelectedIndex = 0 ;
          }
        }
      }
    }

    private void InitKuiAnglePicture()
    {
      pictureBox1.Image = new Bitmap( pictureBox1.Width, pictureBox1.Height ) ;

      float angle ;
      if ( float.TryParse( txtHaichiAngle.Text, out angle ) ) {
        angle = -angle ; // 角度を負の値に変更

        using ( Graphics g = Graphics.FromImage( pictureBox1.Image ) ) {
          g.TranslateTransform( pictureBox1.Width / 2, pictureBox1.Height / 2 ) ;
          g.RotateTransform( angle ) ;
          g.TranslateTransform( -pictureBox1.Width / 2, -pictureBox1.Height / 2 ) ;

          g.DrawImage( originalImage, 0, 0 ) ;
        }

        pictureBox1.Invalidate() ;
      }
      else {
        MessageBox.Show( "有効な角度を入力してください。" ) ;
      }
    }

    private void GetIniData()
    {
      try {
        string path = ClsIni.GetIniFilePath( iniPath ) ;
        cmbLevel.Text = ClsIni.GetIniFile( sec, cmbLevel.Name, iniPath ) ;

        rbnFromToP.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnFromToP.Name, path ) ) ;
        rbnKuiTenba.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnKuiTenba.Name, path ) ) ;
        txtFromToP.Text = ClsIni.GetIniFile( sec, txtFromToP.Name, path ) ;

        rbnPileTypeSanbashi.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnPileTypeSanbashi.Name, path ) ) ;
        rbnPileTypeKenyou.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnPileTypeKenyou.Name, path ) ) ;
        rbnPileTypeDanmenHenka.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnPileTypeDanmenHenka.Name, path ) ) ;
        rbnPileTypeTC.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnPileTypeTC.Name, path ) ) ;
        InitTabControl() ;

        cmbShoriType.Text = ClsIni.GetIniFile( sec, cmbShoriType.Name, path ) ;

        cmbSteelType.Text = ClsIni.GetIniFile( sec, cmbSteelType.Name, path ) ;
        cmbSteelSize.Text = ClsIni.GetIniFile( sec, cmbSteelSize.Name, path ) ;
        txtSteelSize.Text = ClsIni.GetIniFile( sec, txtSteelSize.Name, path ) ;
        InitTxtBoxSteelSize_DanmenHenkaKui() ;

        txtPileTotalLength.Text = ClsIni.GetIniFile( sec, txtPileTotalLength.Name, path ) ;

        txtHaichiAngle.Text = ClsIni.GetIniFile( sec, txtHaichiAngle.Name, path ) ;
        InitKuiAnglePicture() ;

        cmbZanchi.Text = ClsIni.GetIniFile( sec, cmbZanchi.Name, path ) ;
        txtZantiLength.Text = ClsIni.GetIniFile( sec, txtZantiLength.Name, path ) ;

        rbnTopPlateOn.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnTopPlateOn.Name, path ) ) ;
        rbnTopPlateOff.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnTopPlateOff.Name, path ) ) ;

        rbnTopPlate_Nini.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnTopPlate_Nini.Name, path ) ) ;
        txtTopPlateT.Text = ClsIni.GetIniFile( sec, txtTopPlateT.Name, path ) ;
        txtTopPlateW.Text = ClsIni.GetIniFile( sec, txtTopPlateW.Name, path ) ;
        txtTopPlateD.Text = ClsIni.GetIniFile( sec, txtTopPlateD.Name, path ) ;

        rbnTopPlate_Teigata.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnTopPlate_Teigata.Name, path ) ) ;
        cmbTopPlateSizeTeigata.Text = ClsIni.GetIniFile( sec, cmbTopPlateSizeTeigata.Name, path ) ;

        cmbEndPlateThickness.Text = ClsIni.GetIniFile( sec, cmbEndPlateThickness.Name, path ) ;

        txtJointCount.Text = ClsIni.GetIniFile( sec, txtJointCount.Name, path ) ;
        txtPileTotalLength_Top.Text = ClsIni.GetIniFile( sec, txtPileTotalLength_Top.Name, path ) ;
        rbnKoteiTypeBolt.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnKoteiTypeBolt.Name, path ) ) ;
        rbnKoteiTypeYosetsu.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnKoteiTypeYosetsu.Name, path ) ) ;

        txtJointCount2.Text = ClsIni.GetIniFile( sec, txtJointCount2.Name, path ) ;
        txtPileTotalLength_Bottom.Text = ClsIni.GetIniFile( sec, txtPileTotalLength_Bottom.Name, path ) ;
        rbnKoteiTypeBolt2.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnKoteiTypeBolt2.Name, path ) ) ;
        rbnKoteiTypeYosetsu2.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnKoteiTypeYosetsu2.Name, path ) ) ;

        string lengthListString = ClsIni.GetIniFile( sec, "divLength", path ) ;
        if ( lengthListString != "none" ) {
          string[] div = lengthListString.Split( '_' ) ;
          if ( div.Length > 0 ) {
            int i = 0 ;
            foreach ( string n in div ) {
              double d = 0.0 ;
              if ( ! double.TryParse( n, out d ) ) {
                continue ;
              }

              dgvPileLength[ i, 0 ].Value = n ;
              i++ ;
            }
          }
        }
      }
      catch {
        //MessageBox.Show("前回値の取得に失敗しました");
      }
    }

    private void SetIniData()
    {
      string path = ClsIni.GetIniFilePath( iniPath ) ;

      ClsIni.WritePrivateProfileString( sec, cmbLevel.Name, cmbLevel.Text, iniPath ) ;

      ClsIni.WritePrivateProfileString( sec, rbnFromToP.Name, rbnFromToP.Checked.ToString(), path ) ;
      ClsIni.WritePrivateProfileString( sec, rbnKuiTenba.Name, rbnKuiTenba.Checked.ToString(), path ) ;
      ClsIni.WritePrivateProfileString( sec, txtFromToP.Name, txtFromToP.Text, path ) ;

      ClsIni.WritePrivateProfileString( sec, rbnPileTypeSanbashi.Name, rbnPileTypeSanbashi.Checked.ToString(), path ) ;
      ClsIni.WritePrivateProfileString( sec, rbnPileTypeKenyou.Name, rbnPileTypeKenyou.Checked.ToString(), path ) ;
      ClsIni.WritePrivateProfileString( sec, rbnPileTypeDanmenHenka.Name, rbnPileTypeDanmenHenka.Checked.ToString(),
        path ) ;
      ClsIni.WritePrivateProfileString( sec, rbnPileTypeTC.Name, rbnPileTypeTC.Checked.ToString(), path ) ;

      ClsIni.WritePrivateProfileString( sec, cmbShoriType.Name, cmbShoriType.Text, path ) ;

      ClsIni.WritePrivateProfileString( sec, cmbSteelType.Name, cmbSteelType.Text, path ) ;
      ClsIni.WritePrivateProfileString( sec, cmbSteelSize.Name, cmbSteelSize.Text, path ) ;
      ClsIni.WritePrivateProfileString( sec, txtSteelSize.Name, txtSteelSize.Text, path ) ;

      ClsIni.WritePrivateProfileString( sec, txtPileTotalLength.Name, txtPileTotalLength.Text, path ) ;

      ClsIni.WritePrivateProfileString( sec, txtHaichiAngle.Name, txtHaichiAngle.Text, path ) ;

      ClsIni.WritePrivateProfileString( sec, cmbZanchi.Name, cmbZanchi.Text, path ) ;
      ClsIni.WritePrivateProfileString( sec, txtZantiLength.Name, txtZantiLength.Text, path ) ;

      ClsIni.WritePrivateProfileString( sec, rbnTopPlateOn.Name, rbnTopPlateOn.Checked.ToString(), path ) ;
      ClsIni.WritePrivateProfileString( sec, rbnTopPlateOff.Name, rbnTopPlateOff.Checked.ToString(), path ) ;

      ClsIni.WritePrivateProfileString( sec, rbnTopPlate_Nini.Name, rbnTopPlate_Nini.Checked.ToString(), path ) ;
      ClsIni.WritePrivateProfileString( sec, txtTopPlateT.Name, txtTopPlateT.Text, path ) ;
      ClsIni.WritePrivateProfileString( sec, txtTopPlateW.Name, txtTopPlateW.Text, path ) ;
      ClsIni.WritePrivateProfileString( sec, txtTopPlateD.Name, txtTopPlateD.Text, path ) ;

      ClsIni.WritePrivateProfileString( sec, rbnTopPlate_Teigata.Name, rbnTopPlate_Teigata.Checked.ToString(), path ) ;
      ClsIni.WritePrivateProfileString( sec, cmbTopPlateSizeTeigata.Name, cmbTopPlateSizeTeigata.Text, path ) ;

      ClsIni.WritePrivateProfileString( sec, cmbEndPlateThickness.Name, cmbEndPlateThickness.Text, path ) ;

      ClsIni.WritePrivateProfileString( sec, txtJointCount.Name, txtJointCount.Text, path ) ;
      ClsIni.WritePrivateProfileString( sec, txtPileTotalLength_Top.Name, txtPileTotalLength_Top.Text, path ) ;
      ClsIni.WritePrivateProfileString( sec, rbnKoteiTypeBolt.Name, rbnKoteiTypeBolt.Checked.ToString(), path ) ;
      ClsIni.WritePrivateProfileString( sec, rbnKoteiTypeYosetsu.Name, rbnKoteiTypeYosetsu.Checked.ToString(), path ) ;

      ClsIni.WritePrivateProfileString( sec, txtJointCount2.Name, txtJointCount2.Text, path ) ;
      ClsIni.WritePrivateProfileString( sec, txtPileTotalLength_Bottom.Name, txtPileTotalLength_Bottom.Text, path ) ;
      ClsIni.WritePrivateProfileString( sec, rbnKoteiTypeBolt2.Name, rbnKoteiTypeBolt2.Checked.ToString(), path ) ;
      ClsIni.WritePrivateProfileString( sec, rbnKoteiTypeYosetsu2.Name, rbnKoteiTypeYosetsu2.Checked.ToString(),
        path ) ;

      string tmp = string.Empty ;
      for ( int i = 0 ; i < dgvPileLength.Columns.Count ; i++ ) {
        if ( i == dgvPileLength.Columns.Count - 1 ) {
          tmp += dgvPileLength[ i, 0 ].Value.ToString() ;
        }
        else {
          tmp += dgvPileLength[ i, 0 ].Value.ToString() + "_" ;
        }
      }

      ClsIni.WritePrivateProfileString( sec, "divLength", tmp, path ) ;
    }

    private bool CheckInput()
    {
      if ( ! ClsCommonUtils.ChkContorolCommbo( cmbLevel, "配置レベル", true ) )
        return false ;

      if ( rbnFromToP.Checked == true ) {
        if ( ! ClsCommonUtils.ChkContorolTextDouble( txtFromToP, "GLからの高さ" ) ) {
          return false ;
        }
      }

      if ( ! ClsCommonUtils.ChkContorolTextDouble( txtPileTotalLength, "杭全長", bMinus: true ) ) {
        return false ;
      }

      if ( ! ClsCommonUtils.ChkContorolTextDouble( txtHaichiAngle, "配置角度", bMinus: true ) ) {
        return false ;
      }

      if ( rbnTopPlateOn.Checked == true && rbnTopPlate_Nini.Checked == true ) {
        if ( ! ClsCommonUtils.ChkContorolTextDouble( txtTopPlateT, "トッププレート(t)", bMinus: true ) ) {
          return false ;
        }

        if ( ! ClsCommonUtils.ChkContorolTextDouble( txtTopPlateW, "トッププレート(W)", bMinus: true ) ) {
          return false ;
        }

        if ( ! ClsCommonUtils.ChkContorolTextDouble( txtTopPlateD, "トッププレート(D)", bMinus: true ) ) {
          return false ;
        }
      }

      if ( ! ClsCommonUtils.ChkContorolTextDouble( txtJointCount, "継手箇所数", bMinus: true, bComma: true ) ) {
        return false ;
      }

      if ( ! ClsCommonUtils.ChkContorolTextDouble( txtBoltCountFlangeSide, "ボルト本数(フランジ側)", bMinus: true,
            bComma: true ) ) {
        return false ;
      }

      if ( ! ClsCommonUtils.ChkContorolTextDouble( txtBoltCountWebSide, "ボルト本数(Web側)", bMinus: true, bComma: true ) ) {
        return false ;
      }

      //杭の全長チェック
      List<int> lstN1 = new List<int>() ;
      if ( rbnPileTypeDanmenHenka.Checked ) {
        for ( int i = 0 ; i < dgvPileLength.Columns.Count ; i++ ) {
          lstN1.Add( ClsCommonUtils.ChangeStrToInt( dgvPileLength[ i, 0 ].Value.ToString() ) ) ;
        }

        for ( int i = 0 ; i < dgvPileLength2.Columns.Count ; i++ ) {
          lstN1.Add( ClsCommonUtils.ChangeStrToInt( dgvPileLength2[ i, 0 ].Value.ToString() ) ) ;
        }

        if ( lstN1.Sum() != ( ClsCommonUtils.ChangeStrToInt( txtPileTotalLength.Text ) ) ) {
          MessageBox.Show( "杭の長さの合計と全長が一致しません" ) ;
          return false ;
        }
      }
      else {
        for ( int i = 0 ; i < dgvPileLength.Columns.Count ; i++ ) {
          lstN1.Add( ClsCommonUtils.ChangeStrToInt( dgvPileLength[ i, 0 ].Value.ToString() ) ) ;
        }

        if ( lstN1.Sum() != ( ClsCommonUtils.ChangeStrToInt( txtPileTotalLength.Text ) ) ) {
          MessageBox.Show( "杭の長さの合計と全長が一致しません" ) ;
          return false ;
        }
      }

      return true ;
    }

    private bool SetData()
    {
      ClsSanbashiKui clsKui = m_ClsKui ;

      clsKui.m_IsFromGL = rbnFromToP.Checked ;

      if ( rbnFromToP.Checked == true ) {
        if ( txtFromToP.Text == null ) {
          return false ;
        }

        clsKui.m_HightFromGL = double.Parse( txtFromToP.Text ) ;
      }

      // 指定したグループ内でチェックされているラジオボタンを取得する
      var RadioButtonChecked_InGroup =
        grpPileType.Controls.OfType<RadioButton>().SingleOrDefault( rb => rb.Checked == true ) ;
      switch ( RadioButtonChecked_InGroup.Text ) {
        case "桟橋杭(支持杭)" :
          clsKui.m_PileType = PileType.Sanbashi ;
          break ;
        case "兼用杭" :
          clsKui.m_PileType = PileType.Kenyou ;
          break ;
        case "断面変化杭" :
          clsKui.m_PileType = PileType.DanmenHenka ;
          break ;
        case "TC杭" :
          clsKui.m_PileType = PileType.TC ;
          break ;
        default :
          break ;
      }

      if ( cmbShoriType.SelectedItem == null ) {
        return false ;
      }

      switch ( cmbShoriType.SelectedItem.ToString() ) {
        case ( "交点" ) :
          clsKui.m_CreateType = CreateType.Intersection ;
          break ;
        case ( "1点" ) :
          clsKui.m_CreateType = CreateType.OnePoint ;
          break ;
        default :
          break ;
      }

      if ( cmbSteelType.SelectedItem == null ) {
        return false ;
      }

      clsKui.m_KouzaiType = cmbSteelType.SelectedItem.ToString() ;

      if ( cmbSteelSize.SelectedItem == null ) {
        return false ;
      }

      clsKui.m_KouzaiSize = cmbSteelSize.SelectedItem.ToString() ;

      if ( cmbSteelSize.SelectedItem == null ) {
        return false ;
      }

      clsKui.m_KouzaiSizeKoudaikui = cmbSteelSize.SelectedItem.ToString() ;

      clsKui.m_KouzaiSizeDanmenHenkaKui = txtSteelSize.Text ;

      if ( txtPileTotalLength.Text == null ) {
        return false ;
      }

      clsKui.m_PileTotalLength = double.Parse( txtPileTotalLength.Text ) ;

      if ( txtPileTotalLength_Top.Text == null ) {
        return false ;
      }

      clsKui.m_PileTotalLength_Top = double.Parse( txtPileTotalLength_Top.Text ) ;

      if ( txtPileTotalLength_Bottom.Text == null ) {
        return false ;
      }

      clsKui.m_PileTotalLength_Bottom = double.Parse( txtPileTotalLength_Bottom.Text ) ;

      if ( txtHaichiAngle.Text == null ) {
        return false ;
      }

      clsKui.m_PileAngle = double.Parse( txtHaichiAngle.Text ) ;

      if ( cmbZanchi.SelectedItem == null ) {
        return false ;
      }

      clsKui.m_Zanti = cmbZanchi.SelectedItem.ToString() ;
      if ( txtZantiLength.Text == null ) {
        return false ;
      }

      clsKui.m_ZantiLength = double.Parse( txtZantiLength.Text ) ;

      clsKui.m_UseTopPlate = rbnTopPlateOn.Checked ? true : false ;

      if ( rbnTopPlateOn.Checked ) {
        if ( rbnTopPlate_Nini.Checked ) {
          clsKui.m_TopPlateSizeOption = SizeOption.Custom ;
          clsKui.m_TopPlateSize = "PL-" + txtTopPlateT.Text + "x" + txtTopPlateW.Text + "x" + txtTopPlateD.Text ;
        }
        else {
          if ( cmbTopPlateSizeTeigata.SelectedItem == null ) {
            return false ;
          }

          clsKui.m_TopPlateSizeOption = SizeOption.Standard ;
          clsKui.m_TopPlateSize = cmbTopPlateSizeTeigata.SelectedItem.ToString() ;
        }
      }
      else {
        clsKui.m_TopPlateSize = "" ;
      }

      if ( cmbEndPlateThickness.SelectedItem == null ) {
        return false ;
      }

      clsKui.m_EndPlateThickness = double.Parse( cmbEndPlateThickness.SelectedItem.ToString() ) ;

      clsKui.m_TsugiteCount = ClsCommonUtils.ChangeStrToInt( txtJointCount.Text ) ;
      clsKui.m_FixingType = rbnKoteiTypeBolt.Checked ? FixingType.Bolt : FixingType.Welding ;

      clsKui.m_TsugiteCount2 = ClsCommonUtils.ChangeStrToInt( txtJointCount2.Text ) ;
      clsKui.m_FixingType2 = rbnKoteiTypeBolt2.Checked ? FixingType.Bolt : FixingType.Welding ;

      List<int> lstN1 = new List<int>() ;
      for ( int i = 0 ; i < dgvPileLength.Columns.Count ; i++ ) {
        lstN1.Add( ClsCommonUtils.ChangeStrToInt( dgvPileLength[ i, 0 ].Value.ToString() ) ) ;
      }

      clsKui.m_PileLengthList = lstN1 ;

      if ( rbnPileTypeDanmenHenka.Checked ) {
        List<int> lstN2 = new List<int>() ;
        for ( int i = 0 ; i < dgvPileLength2.Columns.Count ; i++ ) {
          lstN2.Add( ClsCommonUtils.ChangeStrToInt( dgvPileLength2[ i, 0 ].Value.ToString() ) ) ;
        }

        clsKui.m_PileLengthList2 = lstN2 ;
      }

      clsKui.m_TsugiteBoltSize_Flange = cmbBoltSizeFlangeSide.Text ;
      clsKui.m_TsugiteBoltQuantity_Flange = txtBoltCountFlangeSide.Text ;
      clsKui.m_TsugiteBoltSize_Web = cmbBoltSizeWebSide.Text ;
      clsKui.m_TsugiteBoltQuantity_Web = txtBoltCountWebSide.Text ;
      clsKui.m_TsugiteBoltSize_Flange2 = cmbBoltSizeFlangeSide2.Text ;
      clsKui.m_TsugiteBoltQuantity_Flange2 = txtBoltCountFlangeSide2.Text ;
      clsKui.m_TsugiteBoltSize_Web2 = cmbBoltSizeWebSide2.Text ;
      clsKui.m_TsugiteBoltQuantity_Web2 = txtBoltCountWebSide2.Text ;


      return true ;
    }

    private void SetControl()
    {
      ClsSanbashiKui clsSanbashiKui = m_ClsKui ;

      if ( clsSanbashiKui.m_IsFromGL ) {
        rbnFromToP.Checked = true ;
        txtFromToP.Text = clsSanbashiKui.m_HightFromGL.ToString() ;
      }
      else {
        rbnKuiTenba.Checked = true ;
      }

      bool changeFlag = false ;
      switch ( clsSanbashiKui.m_PileType ) {
        case PileType.Sanbashi :
          rbnPileTypeSanbashi.Checked = true ;
          changeFlag = true ;
          break ;
        case PileType.Kenyou :
          rbnPileTypeKenyou.Checked = true ;
          changeFlag = true ;
          break ;
        case PileType.DanmenHenka :
          rbnPileTypeDanmenHenka.Checked = true ;
          break ;
        case PileType.TC :
          rbnPileTypeTC.Checked = true ;
          break ;
        default :
          rbnPileTypeSanbashi.Checked = true ;
          break ;
      }

      InitTabControl() ;

      // 桟橋杭 ←→ 兼用杭 は変更可能とする
      if ( changeFlag ) {
        rbnPileTypeSanbashi.Enabled = true ;
        rbnPileTypeKenyou.Enabled = true ;
        rbnPileTypeDanmenHenka.Enabled = false ;
        rbnPileTypeTC.Enabled = false ;
      }
      else {
        grpPileType.Enabled = false ;
      }

      cmbShoriType.SelectedItem = clsSanbashiKui.m_CreateType.ToString() ;

      cmbSteelType.SelectedItem = clsSanbashiKui.m_KouzaiType ;
      cmbSteelSize.SelectedItem = clsSanbashiKui.m_KouzaiSize ;
      txtSteelSize.Text = clsSanbashiKui.m_KouzaiSizeDanmenHenkaKui ;
      InitTxtBoxSteelSize_DanmenHenkaKui() ;

      txtPileTotalLength.Text = clsSanbashiKui.m_PileTotalLength.ToString() ;

      txtPileTotalLength_Top.Text = clsSanbashiKui.m_PileTotalLength_Top.ToString() ;
      txtPileTotalLength_Bottom.Text = clsSanbashiKui.m_PileTotalLength_Bottom.ToString() ;

      txtHaichiAngle.Text = clsSanbashiKui.m_PileAngle.ToString() ;
      InitKuiAnglePicture() ;

      cmbZanchi.SelectedItem = clsSanbashiKui.m_Zanti ;
      txtZantiLength.Text = clsSanbashiKui.m_ZantiLength.ToString() ;

      if ( clsSanbashiKui.m_UseTopPlate ) {
        rbnTopPlateOn.Checked = true ;
      }
      else {
        rbnTopPlateOff.Checked = true ;
      }

      if ( clsSanbashiKui.m_UseTopPlate ) {
        bool foundMatch = false ;
        foreach ( var item in cmbTopPlateSizeTeigata.Items ) {
          if ( item.ToString() == clsSanbashiKui.m_TopPlateSize ) {
            rbnTopPlate_Teigata.Checked = true ;
            cmbTopPlateSizeTeigata.SelectedItem = item ;
            foundMatch = true ;
            break ;
          }
        }

        if ( ! foundMatch ) {
          rbnTopPlate_Nini.Checked = true ;
          string[] size = clsSanbashiKui.m_TopPlateSize.Substring( 3 ).Split( 'x' ) ;
          txtTopPlateT.Text = size[ 0 ] ;
          txtTopPlateW.Text = size[ 1 ] ;
          txtTopPlateD.Text = size[ 2 ] ;
        }
      }

      cmbEndPlateThickness.SelectedItem = clsSanbashiKui.m_EndPlateThickness.ToString() ;

      // 継手

      if ( clsSanbashiKui.m_FixingType == FixingType.Bolt ) {
        rbnKoteiTypeBolt.Checked = true ;
      }
      else {
        rbnKoteiTypeYosetsu.Checked = true ;
      }

      txtJointCount.Text = clsSanbashiKui.m_TsugiteCount.ToString() ;

      if ( clsSanbashiKui.m_FixingType2 == FixingType.Bolt ) {
        rbnKoteiTypeBolt2.Checked = true ;
      }
      else {
        rbnKoteiTypeYosetsu2.Checked = true ;
      }

      txtJointCount2.Text = clsSanbashiKui.m_TsugiteCount2.ToString() ;

      if ( clsSanbashiKui.m_PileLengthList != null ) {
        for ( int i = 0 ; i < clsSanbashiKui.m_PileLengthList.Count && i < dgvPileLength.Columns.Count ; i++ ) {
          dgvPileLength[ i, 0 ].Value = clsSanbashiKui.m_PileLengthList[ i ].ToString() ;
        }
      }

      if ( clsSanbashiKui.m_PileLengthList2 != null ) {
        for ( int i = 0 ; i < clsSanbashiKui.m_PileLengthList2.Count && i < dgvPileLength2.Columns.Count ; i++ ) {
          dgvPileLength2[ i, 0 ].Value = clsSanbashiKui.m_PileLengthList2[ i ].ToString() ;
        }
      }
    }


    #region コントロールイベント

    private void btnOK_Click( object sender, EventArgs e )
    {
      if ( ! CheckInput() ) {
        return ;
      }

      if ( ! SetData() ) {
        MessageBox.Show( "入力が必要な箇所で空欄があります。" ) ;
        return ;
      }

      SetIniData() ;
      m_Level = cmbLevel.Text ;

      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btnCancel_Click( object sender, EventArgs e )
    {
      this.DialogResult = DialogResult.Cancel ;
      this.Close() ;
    }

    private void rbnPileTypeDanmenHenka_CheckedChanged( object sender, EventArgs e )
    {
      InitTabControl() ;
    }

    private void cmbSteelType_SelectedIndexChanged( object sender, EventArgs e )
    {
      InitComboBoxSteelSize() ;
    }

    private void rbnTopPlateOn_CheckedChanged( object sender, EventArgs e )
    {
      grpTopPlateSize.Enabled = true ;
      rbnTopPlate_Teigata.Checked = true ;
    }

    private void rbnTopPlateOff_CheckedChanged( object sender, EventArgs e )
    {
      grpTopPlateSize.Enabled = false ;
    }

    private void btnTopPlate_Nini_CheckedChanged( object sender, EventArgs e )
    {
      txtTopPlateT.Enabled = true ;
      txtTopPlateW.Enabled = true ;
      txtTopPlateD.Enabled = true ;

      cmbTopPlateSizeTeigata.Enabled = false ;
    }

    private void btnTopPlate_Teigata_CheckedChanged( object sender, EventArgs e )
    {
      txtTopPlateT.Enabled = false ;
      txtTopPlateW.Enabled = false ;
      txtTopPlateD.Enabled = false ;

      cmbTopPlateSizeTeigata.Enabled = true ;
    }

    private void rbnKuiTenba_CheckedChanged( object sender, EventArgs e )
    {
      txtFromToP.Enabled = false ;
    }

    private void rbnFromToP_CheckedChanged( object sender, EventArgs e )
    {
      txtFromToP.Enabled = true ;
    }

    private void txtJointCount_TextChanged( object sender, EventArgs e )
    {
      if ( ! int.TryParse( txtJointCount.Text, out int jointCount ) ) {
        //MessageBox.Show("継手箇所数は整数を入力してください");
        txtJointCount.Clear() ;
        return ;
      }

      if ( jointCount < 0 || jointCount > 10 ) {
        MessageBox.Show( "継手箇所数は0以上10以下の整数を入力してください" ) ;
        txtJointCount.Clear() ;
        return ;
      }

      if ( ! txtJointCount.Text.All( char.IsDigit ) ) {
        MessageBox.Show( "継手箇所数には数値を入力してください" ) ;
        txtJointCount.Clear() ;
        return ;
      }

      InitDataGridView( dgvPileLength, jointCount, true ) ;
    }

    private void txtJointCount2_TextChanged( object sender, EventArgs e )
    {
      if ( ! int.TryParse( txtJointCount2.Text, out int jointCount ) ) {
        //MessageBox.Show("継手箇所数は整数を入力してください");
        txtJointCount2.Clear() ;
        return ;
      }

      if ( jointCount < 0 || jointCount > 10 ) {
        MessageBox.Show( "継手箇所数は0以上10以下の整数を入力してください" ) ;
        txtJointCount2.Clear() ;
        return ;
      }

      if ( ! txtJointCount2.Text.All( char.IsDigit ) ) {
        MessageBox.Show( "継手箇所数には数値を入力してください" ) ;
        txtJointCount2.Clear() ;
        return ;
      }

      InitDataGridView( dgvPileLength2, jointCount, false ) ;
    }

    private void rbnKoteiTypeBolt_CheckedChanged( object sender, EventArgs e )
    {
      InitTsugitePlate() ;
    }

    private void rbnKoteiTypeYosetsu_CheckedChanged( object sender, EventArgs e )
    {
      InitTsugitePlate() ;
    }

    private void txtPileTotalLength_TextChanged( object sender, EventArgs e )
    {
      if ( txtPileTotalLength.Enabled == true ) {
        if ( rbnPileTypeDanmenHenka.Checked ) {
          if ( ! int.TryParse( txtJointCount.Text, out int nKashoNum ) ) {
            return ;
          }

          if ( ! int.TryParse( txtJointCount2.Text, out int nKashoNum2 ) ) {
            return ;
          }

          InitDataGridView( dgvPileLength, nKashoNum ) ;
          InitDataGridView( dgvPileLength2, nKashoNum2 ) ;
        }
        else {
          if ( ! int.TryParse( txtJointCount.Text, out int nKashoNum ) ) {
            return ;
          }

          InitDataGridView( dgvPileLength, nKashoNum ) ;
        }
      }
    }

    private void cmbSteelSize_SelectedIndexChanged( object sender, EventArgs e )
    {
      InitTsugitePlate() ;
      if ( rbnPileTypeDanmenHenka.Checked ) {
        InitTxtBoxSteelSize_DanmenHenkaKui() ;
      }
    }

    private void txtHaichiAngle_Leave( object sender, EventArgs e )
    {
      InitKuiAnglePicture() ;
    }

    private void cmbBoltTypeF_SelectedIndexChanged( object sender, EventArgs e )
    {
      InitComboBoxBoltFlangeSize() ;
    }

    private void cmbBoltTypeW_SelectedIndexChanged( object sender, EventArgs e )
    {
      InitComboBoxBoltWebSize() ;
    }

    private void cmbBoltSizeFlangeSide_SelectedIndexChanged( object sender, EventArgs e )
    {
      InitComboBoxBoltTypeF() ;
    }

    private void cmbBoltSizeWebSide_SelectedIndexChanged( object sender, EventArgs e )
    {
      InitComboBoxBoltTypeW() ;
    }

    private void txtTotalLength_Top_TextChanged( object sender, EventArgs e )
    {
      if ( rbnPileTypeDanmenHenka.Checked ) {
        if ( ! int.TryParse( txtJointCount.Text, out int nKashoNum ) ) {
          return ;
        }

        InitDataGridView( dgvPileLength, nKashoNum, true ) ;

        if ( ! double.TryParse( txtPileTotalLength_Top.Text, out double lenTop ) ) {
          lenTop = 0 ;
        }

        if ( ! double.TryParse( txtPileTotalLength_Bottom.Text, out double lenBottom ) ) {
          lenBottom = 0 ;
        }

        txtPileTotalLength.Text = ( lenTop + lenBottom ).ToString() ;
      }
    }

    private void txtTotalLength_Bottom_TextChanged( object sender, EventArgs e )
    {
      if ( rbnPileTypeDanmenHenka.Checked ) {
        if ( ! int.TryParse( txtJointCount2.Text, out int nKashoNum ) ) {
          return ;
        }

        InitDataGridView( dgvPileLength2, nKashoNum, false ) ;

        if ( ! double.TryParse( txtPileTotalLength_Top.Text, out double lenTop ) ) {
          lenTop = 0 ;
        }

        if ( ! double.TryParse( txtPileTotalLength_Bottom.Text, out double lenBottom ) ) {
          lenBottom = 0 ;
        }

        txtPileTotalLength.Text = ( lenTop + lenBottom ).ToString() ;
      }
    }

    private void cmbZanchi_SelectedIndexChanged( object sender, EventArgs e )
    {
      if ( cmbZanchi.SelectedItem == null || cmbZanchi.SelectedItem.ToString() == "一部埋め殺し" ) {
        lblZantiLength.Enabled = true ;
        txtZantiLength.Enabled = true ;
      }
      else {
        lblZantiLength.Enabled = false ;
        txtZantiLength.Enabled = false ;
      }
    }

    #endregion

    private void dgvPileLength_CellClick( object sender, DataGridViewCellEventArgs e )
    {
      m_ChangeDGVKuiLength = true ;
    }

    private void dgvPileLength_CellValueChanged( object sender, DataGridViewCellEventArgs e )
    {
      dgvPileLengthCellValueChanged( dgvPileLength, e.ColumnIndex ) ;
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
        ChangeCellColor( dgv, System.Drawing.Color.Red ) ;
      }
      else {
        lblWarning.Text = "" ;
        ChangeCellColor( dgv, System.Drawing.Color.Empty ) ;
      }
    }

    private void ChangeCellColor( DataGridView dgv, System.Drawing.Color color )
    {
      for ( int i = 0 ; i < dgvPileLength.Columns.Count ; i++ ) {
        dgv[ i, 0 ].Style.BackColor = color ;
      }
    }
  }
}