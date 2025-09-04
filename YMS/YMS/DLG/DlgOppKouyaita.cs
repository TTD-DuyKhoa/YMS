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
  public partial class DlgOppKouyaita : Form
  {
    public DialogResult dialogResult { get ; set ; }

    public DlgOppKouyaita()
    {
      InitializeComponent() ;
    }

    private void btnOK_Click( object sender, EventArgs e )
    {
      dialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btnOpp_Click( object sender, EventArgs e )
    {
      dialogResult = DialogResult.Retry ;
      this.Close() ;
    }
  }
}