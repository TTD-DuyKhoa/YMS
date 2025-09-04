using System ;
using System.Windows.Forms ;

namespace YMS.DLG
{
  public partial class dlgKariHaichi : Form
  {
    public dlgKariHaichi()
    {
      InitializeComponent() ;
    }

    public string m_buzai ;

    private void btnOK_Click( object sender, EventArgs e )
    {
      DialogResult = DialogResult.OK ;
      if ( rdoHaraokoshi.Checked ) {
        m_buzai = "腹起" ;
      }
      else if ( rdoHiuchi.Checked ) {
        m_buzai = "火打" ;
      }
      else if ( rdoKabe.Checked ) {
        m_buzai = "外壁-レンガ、メタル スタッド" ;
      }
      else if ( rdoKiribari.Checked ) {
        m_buzai = "切梁" ;
      }
    }

    private void btnCancel_Click( object sender, EventArgs e )
    {
      DialogResult = DialogResult.Cancel ;
    }
  }
}