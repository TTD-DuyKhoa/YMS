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
  public partial class DlgCreateKiribariUkeBase : Form
  {
    #region 定数

    /// <summary>
    /// Iniファイルのパス
    /// </summary>
    const string IniPath = "ini\\DlgCreateKiribariUkeBase.ini" ;

    /// <summary>
    /// Iniファイルのセクション名
    /// </summary>
    const string sec = "DlgCreateKiribariUkeBase" ;

    #endregion

    #region メンバー

    /// <summary>
    /// 切梁受けベースクラス
    /// </summary>
    public ClsKiribariUkeBase m_KiribariUkeBase ;

    #endregion

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="kiriHiuchiBase"></param>
    public DlgCreateKiribariUkeBase( ClsKiribariUkeBase kiriUkeBase )
    {
      InitializeComponent() ;
      m_KiribariUkeBase = kiriUkeBase ;

      initComboBox() ;
      SetControl() ;
      DoHiddenWay( false ) ;
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="kiriHiuchiBase"></param>
    public DlgCreateKiribariUkeBase()
    {
      InitializeComponent() ;
      m_KiribariUkeBase = new ClsKiribariUkeBase() ;
      initComboBox() ;
      GetIniData() ;
      DoHiddenWay() ;
    }

    public void initComboBox()
    {
      string bk = string.Empty ;
      List<string> lstStr = new List<string>() ;

      //鋼材タイプ
      bk = cmbSteelType.Text ;
      cmbSteelType.Items.Clear() ;
      lstStr = new List<string>()
      {
        Master.ClsYamadomeCsv.TypeShuzai,
        Master.ClsHBeamCsv.TypeHiro,
        Master.ClsHBeamCsv.TypeNaka,
        Master.ClsHBeamCsv.TypeHoso,
        Master.ClsChannelCsv.TypeCannel
      } ;

      foreach ( string str in lstStr ) {
        cmbSteelType.Items.Add( str ) ;
      }

      if ( lstStr.Contains( bk ) ) cmbSteelType.Text = bk ;
      else cmbSteelType.SelectedIndex = 0 ;
    }

    public void initComboBoxSteelSize()
    {
      string bk = string.Empty ;
      List<string> lstStr = new List<string>() ;

      //鋼材サイズ
      bk = cmbSteelSize.Text ;
      cmbSteelSize.Items.Clear() ;
      if ( cmbSteelType.Text == Master.ClsYamadomeCsv.TypeShuzai ) {
        lstStr = Master.ClsYamadomeCsv.GetSizeList( cmbSteelType.Text ) ;
      }
      else if ( cmbSteelType.Text == Master.ClsHBeamCsv.TypeHiro || cmbSteelType.Text == Master.ClsHBeamCsv.TypeNaka ||
                cmbSteelType.Text == Master.ClsHBeamCsv.TypeHoso ) {
        lstStr = Master.ClsHBeamCsv.GetSizeList( cmbSteelType.Text ) ;
      }
      else {
        lstStr = Master.ClsChannelCsv.GetSizeList() ;
      }

      foreach ( string str in lstStr ) {
        cmbSteelSize.Items.Add( str ) ;
      }

      if ( lstStr.Contains( bk ) ) cmbSteelSize.Text = bk ;
      else cmbSteelSize.SelectedIndex = 0 ;
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
      ClsKiribariUkeBase clsKUB = m_KiribariUkeBase ;

      if ( rbnShoriTypeReplace.Checked )
        clsKUB.m_ShoriType = ClsKiribariUkeBase.ShoriType.Replace ;
      else if ( rbnShoriTypePileSelect.Checked )
        clsKUB.m_ShoriType = ClsKiribariUkeBase.ShoriType.PileSelect ;
      else
        clsKUB.m_ShoriType = ClsKiribariUkeBase.ShoriType.PtoP ;

      clsKUB.m_SteelType = cmbSteelType.Text ;
      clsKUB.m_SteelSize = cmbSteelSize.Text ;
      clsKUB.m_TsukidashiRyoS = int.Parse( txtTsukidashiRyoS.Text ) ;
      clsKUB.m_TsukidashiRyoE = int.Parse( txtTsukidashiRyoE.Text ) ;
    }

    /// <summary>
    /// コントロールの値をメンバクラスに保持
    /// </summary>
    private void SetControl()
    {
      ClsKiribariUkeBase clsKUB = m_KiribariUkeBase ;

      if ( clsKUB.m_ShoriType == ClsKiribariUkeBase.ShoriType.Replace )
        rbnShoriTypeReplace.Checked = true ;
      else if ( clsKUB.m_ShoriType == ClsKiribariUkeBase.ShoriType.PileSelect )
        rbnShoriTypePileSelect.Checked = true ;
      else
        rbnShoriTypePtoP.Checked = true ;

      cmbSteelType.Text = clsKUB.m_SteelType ;
      cmbSteelSize.Text = clsKUB.m_SteelSize ;
      txtTsukidashiRyoS.Text = clsKUB.m_TsukidashiRyoS.ToString() ;
      txtTsukidashiRyoE.Text = clsKUB.m_TsukidashiRyoE.ToString() ;
    }

    /// <summary>
    /// Iniの情報をコントロールにセット
    /// </summary>
    private void GetIniData()
    {
      try {
        string iniPath = ClsIni.GetIniFilePath( IniPath ) ;
        rbnShoriTypeReplace.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnShoriTypeReplace.Name, iniPath ) ) ;
        rbnShoriTypePileSelect.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnShoriTypePileSelect.Name, iniPath ) ) ;
        rbnShoriTypePtoP.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnShoriTypePtoP.Name, iniPath ) ) ;
        cmbSteelType.Text = ClsIni.GetIniFile( sec, cmbSteelType.Name, iniPath ) ;
        cmbSteelSize.Text = ClsIni.GetIniFile( sec, cmbSteelSize.Name, iniPath ) ;
        txtTsukidashiRyoS.Text = ClsIni.GetIniFile( sec, txtTsukidashiRyoS.Name, iniPath ) ;
        txtTsukidashiRyoE.Text = ClsIni.GetIniFile( sec, txtTsukidashiRyoE.Name, iniPath ) ;
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

      ClsIni.WritePrivateProfileString( sec, rbnShoriTypeReplace.Name, rbnShoriTypeReplace.Checked.ToString(),
        iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, rbnShoriTypePileSelect.Name, rbnShoriTypePileSelect.Checked.ToString(),
        iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, rbnShoriTypePtoP.Name, rbnShoriTypePtoP.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, cmbSteelType.Name, cmbSteelType.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, cmbSteelSize.Name, cmbSteelSize.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, txtTsukidashiRyoS.Name, txtTsukidashiRyoS.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, txtTsukidashiRyoE.Name, txtTsukidashiRyoE.Text, iniPath ) ;
    }

    /// <summary>
    /// 入力チェック
    /// </summary>
    /// <returns></returns>
    public bool CheckInput()
    {
      if ( ! ClsCommonUtils.ChkContorolTextDouble( txtTsukidashiRyoS, "突き出し量-始点" ) )
        return false ;
      if ( ! ClsCommonUtils.ChkContorolTextDouble( txtTsukidashiRyoE, "突き出し量-終点" ) )
        return false ;

      return true ;
    }

    private void cmbSteelType_SelectedIndexChanged( object sender, EventArgs e )
    {
      initComboBoxSteelSize() ;
    }

    private void DoHiddenWay( bool bFlag = true )
    {
      rbnShoriTypeReplace.Enabled = bFlag ;
      rbnShoriTypePileSelect.Enabled = bFlag ;
      rbnShoriTypePtoP.Enabled = bFlag ;
    }
  }
}