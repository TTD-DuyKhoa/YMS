using Autodesk.Revit.DB ;
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
  public partial class DlgCreateHaraokoshisuberidome : System.Windows.Forms.Form
  {
    #region 定数

    /// <summary>
    /// Iniファイルのパス
    /// </summary>
    const string iniPath = "ini\\DlgCreateHaraokoshisuberidome.ini" ;

    /// <summary>
    /// Iniファイルのセクション名
    /// </summary>
    const string sec = "DlgCreateHaraokoshisuberidome" ;

    #endregion

    #region メンバー変数

    public Document m_doc ;
    public ClsHaraokoshiSuberidome m_ClsHaraokoshiSuberidome ;

    #endregion

    public DlgCreateHaraokoshisuberidome( Document doc, ClsHaraokoshiSuberidome haraokoshiSuberidome )
    {
      InitializeComponent() ;
      m_doc = doc ;
      m_ClsHaraokoshiSuberidome = haraokoshiSuberidome ;
      InitControl( true ) ;
      SetControl() ;
    }

    public DlgCreateHaraokoshisuberidome( Document doc )
    {
      InitializeComponent() ;
      m_doc = doc ;
      m_ClsHaraokoshiSuberidome = new ClsHaraokoshiSuberidome() ;
      InitControl() ;
      GetIniData() ;
    }

    private void InitControl( bool isChange = false )
    {
      grpSuberiDirection.Enabled = rbnTypeChannel.Checked ;
      InitComboBoxBuzaiSize() ;
      InitComboBoxBoltType() ;
      InitComboBoxBoltSize() ;
      if ( isChange ) {
        rbnTypeHojoPiece.Enabled = false ;
      }
    }

    private void InitComboBoxBuzaiSize()
    {
      string selectedText = cmbBuzaiSize.Text ;
      cmbBuzaiSize.Items.Clear() ;
      List<string> lstStr = new List<string>() ;

      if ( rbnTypeHojoPiece.Checked ) {
        lstStr = Master.ClsSupportPieceCsv.GetSizeList() ;
        foreach ( string str in lstStr ) {
          if ( str.Contains( "30D-2" ) ) {
            cmbBuzaiSize.Items.Add( str ) ;
          }
        }
      }
      else if ( rbnTypeChannel.Checked ) {
        lstStr = Master.ClsHaraokoshiSuberidomeCsv.GetSizeList() ;
        foreach ( string str in lstStr ) {
          cmbBuzaiSize.Items.Add( str ) ;
        }
      }
      else {
        return ;
      }

      if ( lstStr.Contains( selectedText ) ) {
        cmbBuzaiSize.Text = selectedText ;
      }
      else {
        cmbBuzaiSize.SelectedIndex = 0 ;
      }
    }

    private void InitComboBoxBoltType()
    {
      string selectedText = cmbBoltType.Text ;
      cmbBoltType.Items.Clear() ;
      List<string> lstStr = new List<string>() ;
      lstStr = Master.ClsBoltCsv.BoltTypes ;
      foreach ( string str in lstStr ) {
        cmbBoltType.Items.Add( str ) ;
      }

      if ( cmbBoltType.Items.Count > 0 ) {
        if ( lstStr.Contains( selectedText ) ) {
          cmbBoltType.Text = selectedText ;
        }
        else {
          cmbBoltType.SelectedIndex = 0 ;
        }
      }
    }

    private void InitComboBoxBoltSize()
    {
      if ( cmbBoltSize != null ) {
        string selectedText = cmbBoltSize.Text ;
        cmbBoltSize.Items.Clear() ;
        List<string> lstStr = new List<string>() ;
        lstStr = Master.ClsBoltCsv.GetSizeList( cmbBoltType.Text ) ;

        foreach ( string str in lstStr ) {
          cmbBoltSize.Items.Add( str ) ;
        }

        if ( cmbBoltSize.Items.Count > 0 ) {
          if ( lstStr.Contains( selectedText ) ) {
            cmbBoltSize.Text = selectedText ;
          }
          else {
            cmbBoltSize.SelectedIndex = 0 ;
          }
        }
      }
    }

    /// <summary>
    /// メンバーの値をコントロールにセット
    /// </summary>
    private void SetControl()
    {
      ClsHaraokoshiSuberidome clsHKS = m_ClsHaraokoshiSuberidome ;

      //部材タイプ
      if ( clsHKS.m_buzaiType == ClsHaraokoshiSuberidome.BuzaiType.HojoPiece )
        rbnTypeHojoPiece.Checked = true ;
      else if ( clsHKS.m_buzaiType == ClsHaraokoshiSuberidome.BuzaiType.Channel )
        rbnTypeChannel.Checked = true ;

      //部材サイズ
      cmbBuzaiSize.Text = clsHKS.m_buzaiSize ;

      //すべり方向
      if ( clsHKS.m_isRight )
        rbnSuberiDirectionR.Checked = true ;
      else
        rbnSuberiDirectionL.Checked = true ;

      //ボルト種類
      cmbBoltType.Text = clsHKS.m_BoltType ;

      //ボルトサイズ
      cmbBoltSize.Text = clsHKS.m_BoltSize ;

      //ボルト本数
      txtBoltNum.Text = clsHKS.m_BoltNum.ToString() ;
    }

    private void GetIniData()
    {
      try {
        string path = ClsIni.GetIniFilePath( iniPath ) ;

        rbnTypeHojoPiece.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnTypeHojoPiece.Name, path ) ) ;
        rbnTypeChannel.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnTypeChannel.Name, path ) ) ;

        cmbBuzaiSize.Text = ClsIni.GetIniFile( sec, cmbBuzaiSize.Name, path ) ;

        rbnSuberiDirectionR.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnSuberiDirectionR.Name, path ) ) ;
        rbnSuberiDirectionL.Checked = bool.Parse( ClsIni.GetIniFile( sec, rbnSuberiDirectionL.Name, path ) ) ;

        cmbBoltType.Text = ClsIni.GetIniFile( sec, cmbBoltType.Name, path ) ;
        cmbBoltSize.Text = ClsIni.GetIniFile( sec, cmbBoltSize.Name, path ) ;
        txtBoltNum.Text = ClsIni.GetIniFile( sec, txtBoltNum.Name, path ) ;
      }
      catch {
        //MessageBox.Show("前回値の取得に失敗しました");
      }
    }

    private void SetIniData()
    {
      string path = ClsIni.GetIniFilePath( iniPath ) ;

      ClsIni.WritePrivateProfileString( sec, rbnTypeHojoPiece.Name, rbnTypeHojoPiece.Checked.ToString(), path ) ;
      ClsIni.WritePrivateProfileString( sec, rbnTypeChannel.Name, rbnTypeChannel.Checked.ToString(), path ) ;

      ClsIni.WritePrivateProfileString( sec, cmbBuzaiSize.Name, cmbBuzaiSize.Text, path ) ;

      ClsIni.WritePrivateProfileString( sec, rbnSuberiDirectionR.Name, rbnSuberiDirectionR.Checked.ToString(), path ) ;
      ClsIni.WritePrivateProfileString( sec, rbnSuberiDirectionL.Name, rbnSuberiDirectionL.Checked.ToString(), path ) ;

      ClsIni.WritePrivateProfileString( sec, cmbBoltType.Name, cmbBoltType.Text, path ) ;
      ClsIni.WritePrivateProfileString( sec, cmbBoltSize.Name, cmbBoltSize.Text, path ) ;
      ClsIni.WritePrivateProfileString( sec, txtBoltNum.Name, txtBoltNum.Text, path ) ;
    }

    private bool CheckInput()
    {
      if ( ! ClsCommonUtils.ChkContorolTextDouble( txtBoltNum, "ボルト本数", bMinus: true, bZero: true, bComma: true ) ) {
        return false ;
      }

      return true ;
    }

    private void SetData()
    {
      ClsHaraokoshiSuberidome clsHaraokoshiSuberidome = m_ClsHaraokoshiSuberidome ;

      if ( rbnTypeHojoPiece.Checked ) {
        clsHaraokoshiSuberidome.m_buzaiType = ClsHaraokoshiSuberidome.BuzaiType.HojoPiece ;
      }
      else {
        clsHaraokoshiSuberidome.m_buzaiType = ClsHaraokoshiSuberidome.BuzaiType.Channel ;
      }

      clsHaraokoshiSuberidome.m_buzaiSize = cmbBuzaiSize.Text ;

      clsHaraokoshiSuberidome.m_isRight = rbnSuberiDirectionR.Checked ;

      clsHaraokoshiSuberidome.m_BoltType = cmbBoltType.Text ;
      clsHaraokoshiSuberidome.m_BoltSize = cmbBoltSize.Text ;
      clsHaraokoshiSuberidome.m_BoltNum = int.Parse( txtBoltNum.Text ) ;
    }

    #region コントロールイベント

    private void btnOK_Click( object sender, EventArgs e )
    {
      if ( ! CheckInput() ) {
        return ;
      }

      SetData() ;
      SetIniData() ;

      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btnCancel_Click( object sender, EventArgs e )
    {
      this.DialogResult = DialogResult.Cancel ;
      this.Close() ;
    }

    private void rbnPartsTypeHojoPeace_CheckedChanged( object sender, EventArgs e )
    {
      grpSuberiDirection.Enabled = false ;
      InitComboBoxBuzaiSize() ;
    }

    private void rbnPartsTypeChannel_CheckedChanged( object sender, EventArgs e )
    {
      grpSuberiDirection.Enabled = true ;
      InitComboBoxBuzaiSize() ;
    }

    private void picSuberiDirectionR_Click( object sender, EventArgs e )
    {
      if ( grpSuberiDirection.Enabled ) {
        rbnSuberiDirectionR.Checked = true ;
      }
    }

    private void picSuberiDirectionL_Click( object sender, EventArgs e )
    {
      if ( grpSuberiDirection.Enabled ) {
        rbnSuberiDirectionL.Checked = true ;
      }
    }

    private void cmbBoltType_SelectedIndexChanged( object sender, EventArgs e )
    {
      InitComboBoxBoltSize() ;
    }

    #endregion
  }
}