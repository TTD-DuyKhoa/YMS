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
  public partial class DlgJougenNum : Form
  {
    public double m_jougen { get ; set ; }

    public DlgJougenNum()
    {
      InitializeComponent() ;
    }

    private void btnOK_Click( object sender, EventArgs e )
    {
      m_jougen = (double) numericUpDown1.Value ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btnCancel_Click( object sender, EventArgs e )
    {
      this.DialogResult = DialogResult.Cancel ;
      this.Close() ;
    }
  }
}