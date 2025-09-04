using System ;
using System.Collections.Generic ;
using System.ComponentModel ;
using System.Data ;
using System.Drawing ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;

namespace YMS.DLG
{
  public partial class DlgCreateSyabariPiece : Form
  {
    #region 定数

    /// <summary>
    /// Iniファイルのパス
    /// </summary>
    const string IniPath = "ini\\DlgCreateSyabariPiece.ini" ;

    /// <summary>
    /// Iniファイルのセクション名
    /// </summary>
    const string sec = "DlgCreateSyabariPiece" ;

    #endregion

    private string m_familyPath1 = "" ; //ファミリパス
    private string m_familyPath2 = "" ; //ファミリパス

    public DlgCreateSyabariPiece( bool kobetsu = true )
    {
      InitializeComponent() ;

      InitControl( kobetsu ) ;
      GetIniData() ;
    }

    public void InitControl( bool kobetsu )
    {
      if ( kobetsu ) {
        lblBuhinType2.Visible = ! kobetsu ;
        lblBuhinSize2.Visible = ! kobetsu ;
        cmbBuhinType2.Visible = ! kobetsu ;
        cmbBuhinSize2.Visible = ! kobetsu ;
      }
      else {
        lblBuhinType1.Text += "(始点側)" ;
        lblBuhinSize1.Text += "(始点側)" ;
      }

      initComboBoxType() ;
    }

    public void initComboBoxType()
    {
      initComboBoxTypeSub( cmbBuhinType1, () => Master.ClsSyabariPieceCSV.GetTypeListSyabari() ) ;
      initComboBoxTypeSub( cmbBuhinType2, () => Master.ClsSyabariPieceCSV.GetTypeListSyabari() ) ;
    }

    private void initComboBoxTypeSub( ComboBox comboBox, Func<List<string>> getListFunc )
    {
      if ( comboBox == null || getListFunc == null ) {
        return ;
      }

      string selectedText = comboBox.Text ;
      comboBox.Items.Clear() ;

      List<string> lstStr = getListFunc.Invoke() ;

      if ( lstStr != null ) {
        comboBox.Items.AddRange( lstStr.ToArray() ) ;

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
      initComboBoxSizeSub( cmbBuhinSize1, cmbBuhinType1.Text ) ;
      initComboBoxSizeSub( cmbBuhinSize2, cmbBuhinType2.Text ) ;
    }

    private void initComboBoxSizeSub( ComboBox comboBox, string selectedSteelType )
    {
      if ( comboBox == null ) {
        return ;
      }

      string selectedText = comboBox.Text ;
      comboBox.Items.Clear() ;

      List<string> lstStr = Master.ClsSyabariPieceCSV.GetSizeListSyabari( selectedSteelType ) ;

      if ( lstStr != null ) {
        comboBox.Items.AddRange( lstStr.ToArray() ) ;

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

      if ( string.IsNullOrWhiteSpace( comboBox.Text ) )
        comboBox.Enabled = false ;
      else
        comboBox.Enabled = true ;
    }

    private void cmbBuhinType1_SelectedIndexChanged( object sender, EventArgs e )
    {
      initComboBoxSizeSub( cmbBuhinSize1, cmbBuhinType1.Text ) ;
    }

    private void cmbBuhinType2_SelectedIndexChanged( object sender, EventArgs e )
    {
      initComboBoxSizeSub( cmbBuhinSize2, cmbBuhinType2.Text ) ;
    }

    /// <summary>
    /// Iniの情報をコントロールにセット
    /// </summary>
    private void GetIniData()
    {
      try {
        string iniPath = ClsIni.GetIniFilePath( IniPath ) ;
        cmbBuhinType1.Text = ClsIni.GetIniFile( sec, cmbBuhinType1.Name, iniPath ) ;
        cmbBuhinSize1.Text = ClsIni.GetIniFile( sec, cmbBuhinSize1.Name, iniPath ) ;
        cmbBuhinType2.Text = ClsIni.GetIniFile( sec, cmbBuhinType2.Name, iniPath ) ;
        cmbBuhinSize2.Text = ClsIni.GetIniFile( sec, cmbBuhinSize2.Name, iniPath ) ;
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
      ClsIni.WritePrivateProfileString( sec, cmbBuhinType1.Name, cmbBuhinType1.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, cmbBuhinSize1.Name, cmbBuhinSize1.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, cmbBuhinType2.Name, cmbBuhinType2.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, cmbBuhinSize2.Name, cmbBuhinSize2.Text, iniPath ) ;
    }

    private void btnOK_Click( object sender, EventArgs e )
    {
      m_familyPath1 = Master.ClsSyabariPieceCSV.GetFamilyPath( cmbBuhinType1.Text, cmbBuhinSize1.Text ) ;
      m_familyPath2 = Master.ClsSyabariPieceCSV.GetFamilyPath( cmbBuhinType2.Text, cmbBuhinSize2.Text ) ;
      SetIniData() ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btnCancel_Click( object sender, EventArgs e )
    {
      this.DialogResult = DialogResult.Cancel ;
      this.Close() ;
    }

    public string GetFamilyPath1()
    {
      return m_familyPath1 ;
    }

    public string GetFamilyPath2()
    {
      return m_familyPath2 ;
    }
  }
}