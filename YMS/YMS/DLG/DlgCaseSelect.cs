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
  public partial class DlgCaseSelect : Form
  {
    public DlgCaseSelect()
    {
      InitializeComponent() ;
    }

    private void button1_Click( object sender, EventArgs e )
    {
      this.DialogResult = DialogResult.Yes ;
      this.Close() ;
    }

    private void button2_Click( object sender, EventArgs e )
    {
      this.DialogResult = DialogResult.No ;
      this.Close() ;
    }
  }
}