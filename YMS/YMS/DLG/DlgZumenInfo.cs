//using Autodesk.Revit.DB;

using Autodesk.Revit.UI ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Windows.Forms ;
using YMS.Parts ;

namespace YMS.DLG
{
  public partial class DlgZumenInfo : Form
  {
    #region 定数

    const string inisec = "ClsZumenInfo" ;
    const string inikey = "FOLDER" ;
    const string iniName = "ini\\ClsZumenInfo.ini" ;

    #endregion

    #region メンバー

    public ClsZumenInfo m_ClsZumenInfo ;

    #endregion

    public DlgZumenInfo( ClsZumenInfo clsZumenInfo )
    {
      InitializeComponent() ;
      m_ClsZumenInfo = clsZumenInfo ;
    }

    private void btnOK_Click( object sender, EventArgs e )
    {
      SetData() ;
      DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btnCancel_Click( object sender, EventArgs e )
    {
      DialogResult = DialogResult.Cancel ;
      this.Close() ;
    }

    private void DlgZumenInfo_FormClosed( object sender, FormClosedEventArgs e )
    {
    }


    public string GetSekkeishishin()
    {
      if ( rdoKenchiku.Checked ) {
        return "建築" ;
      }
      else {
        return "土木" ;
      }
    }

    public void SetSekkeishishin( string shishin )
    {
      if ( shishin == "建築" ) {
        rdoKenchiku.Checked = true ;
      }
      else if ( shishin == "土木" ) {
        rdoDoboku.Checked = true ;
      }
      else {
        foreach ( RadioButton radioButton in groupBox1.Controls.OfType<RadioButton>() ) {
          radioButton.Enabled = true ;
        }

        return ;
      }

      foreach ( RadioButton radioButton in groupBox1.Controls.OfType<RadioButton>() ) {
        radioButton.Enabled = false ;
      }

      return ;
    }


    private void DlgZumenInfo_Load( object sender, EventArgs e )
    {
      ClsZumenInfo clsZumenInfo = m_ClsZumenInfo ;

      SetSekkeishishin( clsZumenInfo.m_sekkeishishin ) ;
      txtTokuisaki.Text = clsZumenInfo.m_tokuisaki ;
      txtGenba.Text = clsZumenInfo.m_genbaName ;
      txtSekkeiNum.Text = clsZumenInfo.m_sekkeiNum ;
    }

    public void SetData()
    {
      ClsZumenInfo clsZumenInfo = m_ClsZumenInfo ;
      clsZumenInfo.m_sekkeishishin = GetSekkeishishin() ;
      clsZumenInfo.m_tokuisaki = txtTokuisaki.Text ;
      clsZumenInfo.m_genbaName = txtGenba.Text ;
      clsZumenInfo.m_sekkeiNum = txtSekkeiNum.Text ;
    }
  }
}