using Autodesk.Revit.DB ;
using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.Drawing ;
using System.Linq ;
using System.Windows.Forms ;
using YMS.Parts ;
using static YMS.Parts.ClsSanbashiKui ;

namespace YMS.DLG
{
  public partial class DlgCreateTanakui : System.Windows.Forms.Form
  {
    private Bitmap originalImage ;
    private bool m_ChangeDGVKuiLength = false ;

    #region 定数

    /// <summary>
    /// Iniファイルのパス
    /// </summary>
    const string iniPath = "ini\\DlgCreateTanakui.ini" ;

    /// <summary>
    /// Iniファイルのセクション名
    /// </summary>
    const string sec = "DlgCreateTanakui" ;

    #endregion

    #region メンバー変数

    public Document m_doc ;
    public ClsTanakui m_ClsKui ;
    public string m_Level { get ; set ; }

    #endregion

    public DlgCreateTanakui( Document doc, ClsTanakui tanakui, string levelName )
    {
      InitializeComponent() ;
      originalImage = new Bitmap( pictureBox1.Image ) ;

      m_doc = doc ;
      m_ClsKui = tanakui ;
      m_Level = levelName ;
      InitControl( true ) ;
      SetControl() ;
    }

    public DlgCreateTanakui( Document doc )
    {
      InitializeComponent() ;
      originalImage = new Bitmap( pictureBox1.Image ) ;

      m_doc = doc ;
      m_ClsKui = new ClsTanakui() ;
      m_Level = string.Empty ;
      InitControl() ;
      GetIniData() ;
    }

    private void InitControl( bool change = false )
    {
      //GLレベル
      cmbLevel.Items.Clear() ;
      cmbLevel.Items.AddRange( ClsYMSUtil.GetLevelNames( m_doc ).ToArray() ) ;
      cmbLevel.SelectedIndex = 0 ;

      // 作成方法
      rbnKuiTenba.Checked = true ;

      // 作成位置
      rbnCreatePossitionLeftTop.Checked = true ;

      // 配置方法
      cmbShoriType.Items.Add( "1点" ) ;
      cmbShoriType.Items.Add( "交点" ) ;
      cmbShoriType.SelectedIndex = 0 ;

      // ずれ量
      txtCreatePossitionZureX.Text = "0" ;
      txtCreatePossitionZureY.Text = "0" ;

      // 杭全長
      txtPileTotalLength.Text = "10000" ;

      // 配置角度
      txtPileHaichiAngle.Text = "90" ;

      // 残置
      cmbZanchi.Items.AddRange( ClsGlobal.m_zanti.ToArray() ) ;
      cmbZanchi.SelectedIndex = 0 ;
      txtZantiLength.Text = "0" ;

      // 切梁ブラケット
      InitBracket() ;

      // 継手箇所数
      txtJointCount.Text = "2" ;

      // 継手固定方法
      rbnKoteiTypeBolt.Checked = true ;

      // 杭の分割長さ
      int.TryParse( txtJointCount.Text, out int nJointCount ) ;
      InitDataGridView( dgvPileLength, nJointCount ) ;

      // 継手プレート
      InitTsugitePlate() ;

      InitComboBox() ;

      if ( change ) {
        cmbShoriType.Enabled = false ;
        rbnCreatePossitionLeftTop.Enabled = false ;
        rbnCreatePossitionRightTop.Enabled = false ;
        rbnCreatePossitionLeftBottom.Enabled = false ;
        rbnCreatePossitionRightBottom.Enabled = false ;
        txtCreatePossitionZureX.Enabled = false ;
        txtCreatePossitionZureY.Enabled = false ;
        chkBracket1.Enabled = false ;
        chkBracket2.Enabled = false ;
        chkBracketOsae1.Enabled = false ;
        chkBracketOsae2.Enabled = false ;
      }
    }

    private void InitComboBox()
    {
      // 鋼材タイプ
      InitComboBoxSteelType() ;

      // 鋼材サイズ
      InitComboBoxSteelSize() ;

      // 切梁ブラケットサイズ
      InitComboBoxKiribariBracketSize() ;

      // 切梁押えブラケットサイズ
      InitComboBoxKiribariBracketOsaeSize() ;
    }

    private void InitComboBoxSteelType()
    {
      // 鋼材タイプ
      string selectedText = cmbSteelType.Text ;
      cmbSteelType.Items.Clear() ;
      List<string> lstStr = new List<string>() ;
      lstStr = Master.ClsHBeamCsv.TypeList ;
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

    private void InitComboBoxKiribariBracketSize()
    {
      if ( cmbKiribariBracketSize != null ) {
        string selectedText = cmbKiribariBracketSize.Text ;
        cmbKiribariBracketSize.Items.Clear() ;

        List<string> lstStr = new List<string>() ;
        lstStr = Master.ClsBracketCsv.GetSizeList( "切梁ブラケット" ) ;

        foreach ( string str in lstStr ) {
          cmbKiribariBracketSize.Items.Add( str ) ;
        }

        if ( cmbKiribariBracketSize.Items.Count > 0 ) {
          if ( lstStr.Contains( selectedText ) ) {
            cmbKiribariBracketSize.Text = selectedText ;
          }
          else {
            cmbKiribariBracketSize.SelectedIndex = 0 ;
          }
        }
      }
    }

    private void InitComboBoxKiribariBracketOsaeSize()
    {
      if ( cmbKiribariOsaeBracketSize != null ) {
        string selectedText = cmbKiribariOsaeBracketSize.Text ;
        cmbKiribariOsaeBracketSize.Items.Clear() ;

        List<string> lstStr = new List<string>() ;
        lstStr = Master.ClsBracketCsv.GetSizeList( "切梁押えブラケット" ) ;

        foreach ( string str in lstStr ) {
          cmbKiribariOsaeBracketSize.Items.Add( str ) ;
        }

        if ( cmbKiribariOsaeBracketSize.Items.Count > 0 ) {
          if ( lstStr.Contains( selectedText ) ) {
            cmbKiribariOsaeBracketSize.Text = selectedText ;
          }
          else {
            cmbKiribariOsaeBracketSize.SelectedIndex = 0 ;
          }
        }
      }
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
      if ( float.TryParse( txtPileHaichiAngle.Text, out angle ) ) {
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

    private void InitBracket()
    {
      if ( chkBracket1.Checked ) {
        lblKiribariBracketSize.Enabled = true ;
        rbnBracketSize_A.Enabled = true ;
        rbnBracketSize_M.Enabled = true ;
        cmbKiribariBracketSize.Enabled = rbnBracketSize_M.Checked ;
        chkBracket2.Enabled = true ;
      }
      else {
        lblKiribariBracketSize.Enabled = false ;
        rbnBracketSize_A.Enabled = false ;
        rbnBracketSize_M.Enabled = false ;
        cmbKiribariBracketSize.Enabled = false ;
        chkBracket2.Enabled = false ;
      }

      if ( chkBracketOsae1.Checked ) {
        lblKiribariOsaeBracketSize.Enabled = true ;
        rbnOsaeBracketSize_A.Enabled = true ;
        rbnOsaeBracketSize_M.Enabled = true ;
        cmbKiribariOsaeBracketSize.Enabled = rbnOsaeBracketSize_M.Checked ;
        chkBracketOsae2.Enabled = true ;
      }
      else {
        lblKiribariOsaeBracketSize.Enabled = false ;
        rbnOsaeBracketSize_A.Enabled = false ;
        rbnOsaeBracketSize_M.Enabled = false ;
        cmbKiribariOsaeBracketSize.Enabled = false ;
        chkBracketOsae2.Enabled = false ;
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

        rbnCreatePossitionLeftTop.Checked =
          bool.Parse( ClsIni.GetIniFile( sec, rbnCreatePossitionLeftTop.Name, path ) ) ;
        rbnCreatePossitionLeftBottom.Checked =
          bool.Parse( ClsIni.GetIniFile( sec, rbnCreatePossitionLeftBottom.Name, path ) ) ;
        rbnCreatePossitionRightTop.Checked =
          bool.Parse( ClsIni.GetIniFile( sec, rbnCreatePossitionRightTop.Name, path ) ) ;
        rbnCreatePossitionRightBottom.Checked =
          bool.Parse( ClsIni.GetIniFile( sec, rbnCreatePossitionRightBottom.Name, path ) ) ;

        cmbShoriType.Text = ClsIni.GetIniFile( sec, cmbShoriType.Name, path ) ;
        txtCreatePossitionZureX.Text = ClsIni.GetIniFile( sec, txtCreatePossitionZureX.Name, path ) ;
        txtCreatePossitionZureY.Text = ClsIni.GetIniFile( sec, txtCreatePossitionZureY.Name, path ) ;

        cmbSteelType.Text = ClsIni.GetIniFile( sec, cmbSteelType.Name, path ) ;
        cmbSteelSize.Text = ClsIni.GetIniFile( sec, cmbSteelSize.Name, path ) ;

        txtPileTotalLength.Text = ClsIni.GetIniFile( sec, txtPileTotalLength.Name, path ) ;

        txtPileHaichiAngle.Text = ClsIni.GetIniFile( sec, txtPileHaichiAngle.Name, path ) ;
        InitKuiAnglePicture() ;

        cmbZanchi.Text = ClsIni.GetIniFile( sec, cmbZanchi.Name, path ) ;
        txtZantiLength.Text = ClsIni.GetIniFile( sec, txtZantiLength.Name, path ) ;

        chkBracket1.Checked = bool.Parse( ClsIni.GetIniFile( sec, chkBracket1.Name, path ) ) ;
        cmbKiribariBracketSize.Text = ClsIni.GetIniFile( sec, cmbKiribariBracketSize.Name, path ) ;
        chkBracket2.Checked = bool.Parse( ClsIni.GetIniFile( sec, chkBracket2.Name, path ) ) ;
        chkBracketOsae1.Checked = bool.Parse( ClsIni.GetIniFile( sec, chkBracketOsae1.Name, path ) ) ;
        cmbKiribariOsaeBracketSize.Text = ClsIni.GetIniFile( sec, cmbKiribariOsaeBracketSize.Name, path ) ;
        chkBracketOsae2.Checked = bool.Parse( ClsIni.GetIniFile( sec, chkBracketOsae2.Name, path ) ) ;

        txtJointCount.Text = ClsIni.GetIniFile( sec, txtJointCount.Name, path ) ;
        rbnKoteiTypeBolt.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnKoteiTypeBolt.Name, path ) ) ;
        rbnKoteiTypeYosetsu.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnKoteiTypeYosetsu.Name, path ) ) ;

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

      ClsIni.WritePrivateProfileString( sec, rbnCreatePossitionLeftTop.Name,
        rbnCreatePossitionLeftTop.Checked.ToString(), path ) ;
      ClsIni.WritePrivateProfileString( sec, rbnCreatePossitionLeftBottom.Name,
        rbnCreatePossitionLeftBottom.Checked.ToString(), path ) ;
      ClsIni.WritePrivateProfileString( sec, rbnCreatePossitionRightTop.Name,
        rbnCreatePossitionRightTop.Checked.ToString(), path ) ;
      ClsIni.WritePrivateProfileString( sec, rbnCreatePossitionRightBottom.Name,
        rbnCreatePossitionRightBottom.Checked.ToString(), path ) ;

      ClsIni.WritePrivateProfileString( sec, cmbShoriType.Name, cmbShoriType.Text, path ) ;
      ClsIni.WritePrivateProfileString( sec, txtCreatePossitionZureX.Name, txtCreatePossitionZureX.Text, path ) ;
      ClsIni.WritePrivateProfileString( sec, txtCreatePossitionZureY.Name, txtCreatePossitionZureY.Text, path ) ;

      ClsIni.WritePrivateProfileString( sec, cmbSteelType.Name, cmbSteelType.Text, path ) ;
      ClsIni.WritePrivateProfileString( sec, cmbSteelSize.Name, cmbSteelSize.Text, path ) ;

      ClsIni.WritePrivateProfileString( sec, txtPileTotalLength.Name, txtPileTotalLength.Text, path ) ;
      ClsIni.WritePrivateProfileString( sec, txtPileHaichiAngle.Name, txtPileHaichiAngle.Text, path ) ;

      ClsIni.WritePrivateProfileString( sec, cmbZanchi.Name, cmbZanchi.Text, path ) ;
      ClsIni.WritePrivateProfileString( sec, txtZantiLength.Name, txtZantiLength.Text, path ) ;

      ClsIni.WritePrivateProfileString( sec, chkBracket1.Name, chkBracket1.Checked.ToString(), path ) ;
      ClsIni.WritePrivateProfileString( sec, cmbKiribariBracketSize.Name, cmbKiribariBracketSize.Text, path ) ;
      ClsIni.WritePrivateProfileString( sec, chkBracket2.Name, chkBracket2.Checked.ToString(), path ) ;
      ClsIni.WritePrivateProfileString( sec, chkBracketOsae1.Name, chkBracketOsae1.Checked.ToString(), path ) ;
      ClsIni.WritePrivateProfileString( sec, cmbKiribariOsaeBracketSize.Name, cmbKiribariOsaeBracketSize.Text, path ) ;
      ClsIni.WritePrivateProfileString( sec, chkBracketOsae2.Name, chkBracketOsae2.Checked.ToString(), path ) ;

      ClsIni.WritePrivateProfileString( sec, txtJointCount.Name, txtJointCount.Text, path ) ;
      ClsIni.WritePrivateProfileString( sec, rbnKoteiTypeBolt.Name, rbnKoteiTypeBolt.Checked.ToString(), path ) ;
      ClsIni.WritePrivateProfileString( sec, rbnKoteiTypeYosetsu.Name, rbnKoteiTypeYosetsu.Checked.ToString(), path ) ;

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

      if ( ! ClsCommonUtils.ChkContorolTextDouble( txtPileHaichiAngle, "配置角度", bMinus: true ) ) {
        return false ;
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
      for ( int i = 0 ; i < dgvPileLength.Columns.Count ; i++ ) {
        lstN1.Add( ClsCommonUtils.ChangeStrToInt( dgvPileLength[ i, 0 ].Value.ToString() ) ) ;
      }

      if ( lstN1.Sum() != ( ClsCommonUtils.ChangeStrToInt( txtPileTotalLength.Text ) ) ) {
        MessageBox.Show( "杭の長さの合計と全長が一致しません" ) ;
        return false ;
      }

      return true ;
    }

    private bool SetData()
    {
      ClsTanakui clsTanaKui = m_ClsKui ;

      clsTanaKui.m_IsFromGL = rbnFromToP.Checked ;

      if ( rbnFromToP.Checked == true ) {
        if ( txtFromToP.Text == null ) {
          return false ;
        }

        clsTanaKui.m_HightFromGL = double.Parse( txtFromToP.Text ) ;
      }

      // 指定したグループ内でチェックされているラジオボタンを取得する
      var RadioButtonChecked_InGroup =
        grpCreatePosition.Controls.OfType<RadioButton>().SingleOrDefault( rb => rb.Checked == true ) ;
      switch ( RadioButtonChecked_InGroup.Text ) {
        case "左上" :
          clsTanaKui.m_CreatePosition = ClsTanakui.CreatePosition.LeftTop ;
          break ;
        case "右上" :
          clsTanaKui.m_CreatePosition = ClsTanakui.CreatePosition.RightTop ;
          break ;
        case "左下" :
          clsTanaKui.m_CreatePosition = ClsTanakui.CreatePosition.LeftBottom ;
          break ;
        case "右下" :
          clsTanaKui.m_CreatePosition = ClsTanakui.CreatePosition.RightBottom ;
          break ;
        default :
          break ;
      }

      if ( cmbShoriType.SelectedItem == null ) {
        return false ;
      }

      switch ( cmbShoriType.SelectedItem.ToString() ) {
        case ( "交点" ) :
          clsTanaKui.m_CreateType = (ClsTanakui.CreateType) CreateType.Intersection ;
          break ;
        case ( "1点" ) :
          clsTanaKui.m_CreateType = (ClsTanakui.CreateType) CreateType.OnePoint ;
          break ;
        default :
          break ;
      }

      if ( txtCreatePossitionZureX.Text == null ) {
        return false ;
      }

      clsTanaKui.m_ZureryouX = double.Parse( txtCreatePossitionZureX.Text ) ;
      if ( txtCreatePossitionZureY.Text == null ) {
        return false ;
      }

      clsTanaKui.m_ZureryouY = double.Parse( txtCreatePossitionZureY.Text ) ;

      if ( cmbSteelType.SelectedItem == null ) {
        return false ;
      }

      clsTanaKui.m_KouzaiType = cmbSteelType.SelectedItem.ToString() ;

      if ( cmbSteelSize.SelectedItem == null ) {
        return false ;
      }

      clsTanaKui.m_KouzaiSize = cmbSteelSize.SelectedItem.ToString() ;

      if ( txtPileTotalLength.Text == null ) {
        return false ;
      }

      clsTanaKui.m_PileTotalLength = double.Parse( txtPileTotalLength.Text ) ;

      if ( txtPileHaichiAngle.Text == null ) {
        return false ;
      }

      clsTanaKui.m_PileHaichiAngle = double.Parse( txtPileHaichiAngle.Text ) ;

      if ( cmbZanchi.SelectedItem == null ) {
        return false ;
      }

      clsTanaKui.m_Zanti = cmbZanchi.SelectedItem.ToString() ;
      if ( txtZantiLength.Text == null ) {
        return false ;
      }

      clsTanaKui.m_ZantiLength = double.Parse( txtZantiLength.Text ) ;

      clsTanaKui.m_CreateKiribariBracket = chkBracket1.Checked ;
      if ( cmbKiribariBracketSize.SelectedItem == null ) {
        return false ;
      }

      clsTanaKui.m_KiribariBracketSizeIsAuto = rbnOsaeBracketSize_A.Checked ;
      clsTanaKui.m_KiribariBracketSize = cmbKiribariBracketSize.SelectedItem.ToString() ;
      clsTanaKui.m_CreatePreloadGuide = chkBracket2.Checked ;

      clsTanaKui.m_CreateKiribariOsaeBracket = chkBracketOsae1.Checked ;
      if ( cmbKiribariOsaeBracketSize.SelectedItem == null ) {
        return false ;
      }

      clsTanaKui.m_KiribariOsaeBracketSizeIsAuto = rbnOsaeBracketSize_A.Checked ;
      clsTanaKui.m_KiribariOsaeBracketSize = cmbKiribariOsaeBracketSize.SelectedItem.ToString() ;
      clsTanaKui.m_CreateGuide = chkBracketOsae2.Checked ;

      if ( txtJointCount.Text == null ) {
        return false ;
      }

      clsTanaKui.m_TsugiteCount = ClsCommonUtils.ChangeStrToInt( txtJointCount.Text ) ;
      clsTanaKui.m_FixingType =
        (ClsTanakui.FixingType) ( rbnKoteiTypeBolt.Checked ? FixingType.Bolt : FixingType.Welding ) ;

      List<int> lstN1 = new List<int>() ;
      for ( int i = 0 ; i < dgvPileLength.Columns.Count ; i++ ) {
        if ( dgvPileLength[ i, 0 ].Value == null ) {
          return false ;
        }

        lstN1.Add( ClsCommonUtils.ChangeStrToInt( dgvPileLength[ i, 0 ].Value.ToString() ) ) ;
      }

      clsTanaKui.m_PileLengthList = lstN1 ;

      clsTanaKui.m_TsugiteBoltSize_Flange = cmbBoltSizeFlangeSide.Text ;
      clsTanaKui.m_TsugiteBoltQuantity_Flange = txtBoltCountFlangeSide.Text ;
      clsTanaKui.m_TsugiteBoltSize_Web = cmbBoltSizeWebSide.Text ;
      clsTanaKui.m_TsugiteBoltQuantity_Web = txtBoltCountWebSide.Text ;

      return true ;
    }

    private void SetControl()
    {
      ClsTanakui clsTanaKui = m_ClsKui ;

      if ( clsTanaKui.m_IsFromGL ) {
        rbnFromToP.Checked = true ;
        txtFromToP.Text = clsTanaKui.m_HightFromGL.ToString() ;
      }
      else {
        rbnKuiTenba.Checked = true ;
      }

      switch ( clsTanaKui.m_CreateType ) {
        case ClsTanakui.CreateType.OnePoint :
          cmbShoriType.Text = "1点" ;
          break ;
        case ClsTanakui.CreateType.Intersection :
          cmbShoriType.Text = "交点" ;
          break ;
      }

      switch ( clsTanaKui.m_CreatePosition ) {
        case ClsTanakui.CreatePosition.LeftTop :
          rbnCreatePossitionLeftTop.Checked = true ;
          break ;
        case ClsTanakui.CreatePosition.LeftBottom :
          rbnCreatePossitionLeftBottom.Checked = true ;
          break ;
        case ClsTanakui.CreatePosition.RightTop :
          rbnCreatePossitionRightTop.Checked = true ;
          break ;
        case ClsTanakui.CreatePosition.RightBottom :
          rbnCreatePossitionRightBottom.Checked = true ;
          break ;
      }

      txtCreatePossitionZureX.Text = clsTanaKui.m_ZureryouX.ToString() ;
      txtCreatePossitionZureY.Text = clsTanaKui.m_ZureryouY.ToString() ;

      cmbSteelType.SelectedItem = clsTanaKui.m_KouzaiType ;
      cmbSteelSize.SelectedItem = clsTanaKui.m_KouzaiSize ;

      txtPileTotalLength.Text = clsTanaKui.m_PileTotalLength.ToString() ;

      txtPileHaichiAngle.Text = clsTanaKui.m_PileHaichiAngle.ToString() ;

      cmbZanchi.SelectedItem = clsTanaKui.m_Zanti ;
      txtZantiLength.Text = clsTanaKui.m_ZantiLength.ToString() ;

      chkBracket1.Checked = clsTanaKui.m_CreateKiribariBracket ;
      chkBracket2.Checked = clsTanaKui.m_CreatePreloadGuide ;
      chkBracketOsae1.Checked = clsTanaKui.m_CreateKiribariOsaeBracket ;
      chkBracketOsae2.Checked = clsTanaKui.m_CreateGuide ;

      // 継手
      txtJointCount.Text = clsTanaKui.m_TsugiteCount.ToString() ;

      if ( clsTanaKui.m_FixingType == ClsTanakui.FixingType.Bolt ) {
        rbnKoteiTypeBolt.Checked = true ;
      }
      else {
        rbnKoteiTypeYosetsu.Checked = true ;
      }

      if ( clsTanaKui.m_PileLengthList != null ) {
        for ( int i = 0 ; i < clsTanaKui.m_PileLengthList.Count && i < dgvPileLength.Columns.Count ; i++ ) {
          dgvPileLength[ i, 0 ].Value = clsTanaKui.m_PileLengthList[ i ].ToString() ;
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
        MessageBox.Show( "入力が必要な箇所で空欄があります" ) ;
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

    private void rbnKuiTenba_CheckedChanged( object sender, EventArgs e )
    {
      txtFromToP.Enabled = false ;
    }

    private void rbnFromToP_CheckedChanged( object sender, EventArgs e )
    {
      txtFromToP.Enabled = true ;
    }

    private void cmbShoriType_SelectedIndexChanged( object sender, EventArgs e )
    {
      switch ( cmbShoriType.SelectedItem.ToString() ) {
        case ( "交点" ) :
          grpCreatePosition.Enabled = true ;

          lblZureX.Enabled = true ;
          txtCreatePossitionZureX.Enabled = true ;
          lblZureXmm.Enabled = true ;

          lblZureY.Enabled = true ;
          txtCreatePossitionZureY.Enabled = true ;
          lblZureYmm.Enabled = true ;
          break ;
        case ( "1点" ) :
          grpCreatePosition.Enabled = false ;

          lblZureX.Enabled = false ;
          txtCreatePossitionZureX.Enabled = false ;
          lblZureXmm.Enabled = false ;

          lblZureY.Enabled = false ;
          txtCreatePossitionZureY.Enabled = false ;
          lblZureYmm.Enabled = false ;
          break ;
        default :
          break ;
      }
    }

    private void cmbSteelType_SelectedIndexChanged( object sender, EventArgs e )
    {
      InitComboBoxSteelSize() ;
    }

    private void cmbSteelSize_SelectedIndexChanged( object sender, EventArgs e )
    {
      InitTsugitePlate() ;
    }

    private void txtPileTotalLength_TextChanged( object sender, EventArgs e )
    {
      if ( ! int.TryParse( txtJointCount.Text, out int nKashoNum ) ) {
        return ;
      }

      InitDataGridView( dgvPileLength, nKashoNum ) ;
    }

    private void txtHaichiAngle_Leave( object sender, EventArgs e )
    {
      InitKuiAnglePicture() ;
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

      InitDataGridView( dgvPileLength, jointCount ) ;
    }

    private void rbnKoteiTypeBolt_TextChanged( object sender, EventArgs e )
    {
      InitTsugitePlate() ;
    }

    private void rbnKoteiTypeYosetsu_CheckedChanged( object sender, EventArgs e )
    {
      InitTsugitePlate() ;
    }

    private void cmbBoltTypeF_SelectedIndexChanged( object sender, EventArgs e )
    {
      InitComboBoxBoltFlangeSize() ;
    }

    private void cmbBoltTypeW_SelectedIndexChanged( object sender, EventArgs e )
    {
      InitComboBoxBoltWebSize() ;
    }

    private void chkBracket1_CheckedChanged( object sender, EventArgs e )
    {
      InitBracket() ;
    }

    private void chkBracketOsae1_CheckedChanged( object sender, EventArgs e )
    {
      InitBracket() ;
    }

    private void rbnBracketSize_CheckedChanged( object sender, EventArgs e )
    {
      cmbKiribariBracketSize.Enabled = rbnBracketSize_M.Checked ;
    }

    private void rbnOsaeBracketSize_CheckedChanged( object sender, EventArgs e )
    {
      cmbKiribariOsaeBracketSize.Enabled = rbnOsaeBracketSize_M.Checked ;
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