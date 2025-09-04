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

namespace YMS.DLG
{
  public partial class DlgCreateKiribariTsunagizaiBase : Form
  {
    #region 定数

    /// <summary>
    /// Iniファイルのパス
    /// </summary>
    const string IniPath = "ini\\DlgCreateKiribariTsunagizaiBase.ini" ;

    /// <summary>
    /// Iniファイルのセクション名
    /// </summary>
    const string sec = "DlgCreateKiribariTsunagizaiBase" ;

    #endregion

    #region メンバー

    /// <summary>
    /// 切梁受けベースクラス
    /// </summary>
    public ClsKiribariTsunagizaiBase m_KiribariTsunagizaiBase ;

    #endregion

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="kiriHiuchiBase"></param>
    public DlgCreateKiribariTsunagizaiBase( ClsKiribariTsunagizaiBase KiribariTsunagizaiBase )
    {
      InitializeComponent() ;
      m_KiribariTsunagizaiBase = KiribariTsunagizaiBase ;

      InitComboBox() ;
      SetControl() ;
      groupBox3.Enabled = false ;
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="kiriHiuchiBase"></param>
    public DlgCreateKiribariTsunagizaiBase()
    {
      InitializeComponent() ;
      m_KiribariTsunagizaiBase = new ClsKiribariTsunagizaiBase() ;

      InitComboBox() ;
      GetIniData() ;
      groupBox3.Enabled = true ;
    }

    public void InitComboBox()
    {
      if ( cmbSteelType == null || cmbBoltType1 == null ) {
        // ヌル チェック
        return ;
      }

      string selectedSteelType = cmbSteelType.Text ;
      string selectedBoltType = cmbBoltType1.Text ;

      cmbSteelType.Items.Clear() ;
      cmbBoltType1.Items.Clear() ;

      List<string> steelTypeList = new List<string>()
      {
        Master.ClsYamadomeCsv.TypeShuzai,
        Master.ClsHBeamCsv.TypeHiro,
        Master.ClsHBeamCsv.TypeNaka,
        Master.ClsHBeamCsv.TypeHoso,
        Master.ClsAngleCsv.TypeAngle,
        Master.ClsChannelCsv.TypeCannel
      } ;

      // 鋼材タイプの設定
      cmbSteelType.Items.AddRange( steelTypeList.ToArray() ) ;
      if ( steelTypeList.Contains( selectedSteelType ) ) {
        cmbSteelType.Text = selectedSteelType ;
      }
      else if ( cmbSteelType.Items.Count > 0 ) {
        cmbSteelType.SelectedIndex = 0 ;
      }

      // ボルト種類の設定
      List<string> boltTypeList = Master.ClsBoltCsv.GetTypeList() ;
      cmbBoltType1.Items.AddRange( boltTypeList.ToArray() ) ;
      if ( boltTypeList.Contains( selectedBoltType ) ) {
        cmbBoltType1.Text = selectedBoltType ;
      }
      else if ( cmbBoltType1.Items.Count > 0 ) {
        cmbBoltType1.SelectedIndex = 0 ;
      }
    }

    public void InitComboBoxSteelSize()
    {
      if ( cmbSteelSize == null || cmbSteelType == null ) {
        // ヌル チェック
        return ;
      }

      string selectedSteelSize = cmbSteelSize.Text ;
      cmbSteelSize.Items.Clear() ;
      List<string> lstStr = new List<string>() ;

      string steelType = cmbSteelType.Text ;

      switch ( steelType ) {
        case Master.ClsYamadomeCsv.TypeShuzai :
          lstStr = Master.ClsYamadomeCsv.GetSizeList( steelType ) ;
          break ;
        case Master.ClsHBeamCsv.TypeHiro :
        case Master.ClsHBeamCsv.TypeNaka :
        case Master.ClsHBeamCsv.TypeHoso :
          lstStr = Master.ClsHBeamCsv.GetSizeList( steelType ) ;
          break ;
        case Master.ClsAngleCsv.TypeAngle :
        case Master.ClsAngleCsv.TypeFutohenAngle :
          lstStr = Master.ClsAngleCsv.GetSizeList( steelType ) ;
          break ;
        default :
          lstStr = Master.ClsChannelCsv.GetSizeList() ;
          break ;
      }

      cmbSteelSize.Items.AddRange( lstStr.ToArray() ) ;

      if ( lstStr.Contains( selectedSteelSize ) ) {
        cmbSteelSize.Text = selectedSteelSize ;
      }
      else if ( cmbSteelSize.Items.Count > 0 ) {
        cmbSteelSize.SelectedIndex = 0 ;
      }
    }

    public void InitComboBoxBoltType()
    {
      if ( cmbBoltType1 == null || cmbBoltType2 == null ) {
        // ヌル チェック
        return ;
      }

      string selectedBoltType = cmbBoltType2.Text ;

      try {
        // データ取得前にクリアする
        cmbBoltType2.Items.Clear() ;

        // ボルトタイプのデータ取得
        List<string> sizeList = Master.ClsBoltCsv.GetSizeList( cmbBoltType1.Text ) ;

        // 新しいデータをセット
        cmbBoltType2.Items.AddRange( sizeList.ToArray() ) ;

        // 選択されたアイテムの処理
        if ( sizeList.Contains( selectedBoltType ) ) {
          cmbBoltType2.Text = selectedBoltType ;
        }
        else if ( cmbBoltType2.Items.Count > 0 ) {
          cmbBoltType2.SelectedIndex = 0 ;
        }
      }
      catch ( Exception ex ) {
        // 例外処理を追加するか、ログに出力するなどの対応を行う
        Console.WriteLine( "Error in InitComboBoxBoltType: " + ex.Message ) ;
      }
    }

    /// <summary>
    /// OKボタン押下処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnOK_Click( object sender, EventArgs e )
    {
      if ( ! CheckInput() )
        return ;

      SetData() ;
      SetIniData() ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    /// <summary>
    /// キャンセルボタン押下処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnCancel_Click( object sender, EventArgs e )
    {
      this.DialogResult = DialogResult.Cancel ;
      this.Close() ;
    }


    /// <summary>
    /// コントロールの値をメンバクラスに保持
    /// </summary>
    private void SetData()
    {
      ClsKiribariTsunagizaiBase clsKTB = m_KiribariTsunagizaiBase ;
      clsKTB.m_SteelType = cmbSteelType.Text ;
      clsKTB.m_SteelSize = cmbSteelSize.Text ;
      clsKTB.m_SplitFlg = chkSplit.Checked ;

      if ( rbnToritsukeHohoBolt.Checked )
        clsKTB.m_ToritsukeHoho = ClsKiribariTsunagizaiBase.ToritsukeHoho.Bolt ;
      else if ( rbnToritsukeHohoBuruman.Checked )
        clsKTB.m_ToritsukeHoho = ClsKiribariTsunagizaiBase.ToritsukeHoho.Buruman ;
      else
        clsKTB.m_ToritsukeHoho = ClsKiribariTsunagizaiBase.ToritsukeHoho.Rikiman ;

      if ( rbnShoriTypeReplace.Checked )
        clsKTB.m_ShoriType = ClsKiribariTsunagizaiBase.ShoriType.Replace ;
      else
        clsKTB.m_ShoriType = ClsKiribariTsunagizaiBase.ShoriType.Manual ;

      clsKTB.m_BoltType1 = cmbBoltType1.Text ;
      clsKTB.m_BoltType2 = cmbBoltType2.Text ;
      clsKTB.m_BoltNum = ClsCommonUtils.ChangeStrToInt( txtBoltNum.Text ) ;
    }

    /// <summary>
    /// コントロールの値をメンバクラスに保持
    /// </summary>
    private void SetControl()
    {
      ClsKiribariTsunagizaiBase clsKHB = m_KiribariTsunagizaiBase ;
      cmbSteelType.Text = clsKHB.m_SteelType ;
      cmbSteelSize.Text = clsKHB.m_SteelSize ;
      chkSplit.Checked = clsKHB.m_SplitFlg ;

      if ( clsKHB.m_ToritsukeHoho == ClsKiribariTsunagizaiBase.ToritsukeHoho.Bolt )
        rbnToritsukeHohoBolt.Checked = true ;
      else if ( clsKHB.m_ToritsukeHoho == ClsKiribariTsunagizaiBase.ToritsukeHoho.Buruman )
        rbnToritsukeHohoBuruman.Checked = true ;
      else
        rbnToritsukeHohoRikiman.Checked = true ;

      if ( clsKHB.m_ShoriType == ClsKiribariTsunagizaiBase.ShoriType.Replace )
        rbnShoriTypeReplace.Checked = true ;
      else
        rbnShoriTypeManual.Checked = true ;

      cmbBoltType1.Text = clsKHB.m_BoltType1 ;
      cmbBoltType2.Text = clsKHB.m_BoltType2 ;
      txtBoltNum.Text = clsKHB.m_BoltNum.ToString() ;
    }

    /// <summary>
    /// Iniの情報をコントロールにセット
    /// </summary>
    private void GetIniData()
    {
      try {
        string iniPath = ClsIni.GetIniFilePath( IniPath ) ;
        cmbSteelType.Text = ClsIni.GetIniFile( sec, cmbSteelType.Name, iniPath ) ;
        cmbSteelSize.Text = ClsIni.GetIniFile( sec, cmbSteelSize.Name, iniPath ) ;
        chkSplit.Checked = bool.Parse( ClsIni.GetIniFile( sec, chkSplit.Name, iniPath ) ) ;
        rbnToritsukeHohoBolt.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnToritsukeHohoBolt.Name, iniPath ) ) ;
        rbnToritsukeHohoBuruman.Checked =
          bool.Parse( ClsIni.GetIniFile( sec, rbnToritsukeHohoBuruman.Name, iniPath ) ) ;
        rbnToritsukeHohoRikiman.Checked =
          bool.Parse( ClsIni.GetIniFile( sec, rbnToritsukeHohoRikiman.Name, iniPath ) ) ;
        rbnShoriTypeReplace.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnShoriTypeReplace.Name, iniPath ) ) ;
        rbnShoriTypeManual.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnShoriTypeManual.Name, iniPath ) ) ;
        cmbBoltType1.Text = ClsIni.GetIniFile( sec, cmbBoltType1.Name, iniPath ) ;
        cmbBoltType2.Text = ClsIni.GetIniFile( sec, cmbBoltType2.Name, iniPath ) ;
        txtBoltNum.Text = ClsIni.GetIniFile( sec, txtBoltNum.Name, iniPath ) ;
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
      ClsIni.WritePrivateProfileString( sec, cmbSteelType.Name, cmbSteelType.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, cmbSteelSize.Name, cmbSteelSize.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, chkSplit.Name, chkSplit.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, rbnToritsukeHohoBolt.Name, rbnToritsukeHohoBolt.Checked.ToString(),
        iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, rbnToritsukeHohoBuruman.Name, rbnToritsukeHohoBuruman.Checked.ToString(),
        iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, rbnToritsukeHohoRikiman.Name, rbnToritsukeHohoRikiman.Checked.ToString(),
        iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, rbnShoriTypeReplace.Name, rbnShoriTypeReplace.Checked.ToString(),
        iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, rbnShoriTypeManual.Name, rbnShoriTypeManual.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, cmbBoltType1.Name, cmbBoltType1.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, cmbBoltType2.Name, cmbBoltType2.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, txtBoltNum.Name, txtBoltNum.Text, iniPath ) ;
    }

    private void rbnToritsukeHohoBolt_CheckedChanged( object sender, EventArgs e )
    {
      chkToritsukeHouhou() ;
    }

    private void rbnToritsukeHohoBuruman_CheckedChanged( object sender, EventArgs e )
    {
      chkToritsukeHouhou() ;
    }

    private void rbnToritsukeHohoRikiman_CheckedChanged( object sender, EventArgs e )
    {
      chkToritsukeHouhou() ;
    }

    private void chkToritsukeHouhou()
    {
      if ( rbnToritsukeHohoBolt.Checked == true ) {
        cmbBoltType1.Enabled = true ;
        cmbBoltType2.Enabled = true ;
        txtBoltNum.Enabled = true ;
      }
      else {
        cmbBoltType1.Enabled = false ;
        cmbBoltType2.Enabled = false ;
        txtBoltNum.Enabled = false ;
      }
    }

    /// <summary>
    /// 入力チェック
    /// </summary>
    /// <returns></returns>
    public bool CheckInput()
    {
      if ( ! ClsCommonUtils.ChkContorolTextDouble( txtBoltNum, "ボルト本数", bZero: true, bMinus: true, bComma: true ) )
        return false ;

      return true ;
    }

    private void cmbSteelType_SelectedIndexChanged( object sender, EventArgs e )
    {
      InitComboBoxSteelSize() ;
    }

    private void cmbBoltType1_SelectedIndexChanged( object sender, EventArgs e )
    {
      InitComboBoxBoltType() ;
    }
  }
}