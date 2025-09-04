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

namespace YMS.DLG
{
  public partial class DlgTEST : Form
  {
    public double fall { get ; set ; }
    public string size { get ; set ; }

    public DlgTEST()
    {
      InitializeComponent() ;
    }

    private void button1_Click( object sender, EventArgs e )
    {
      fall = ClsCommonUtils.ChangeStrToDbl( textBox1.Text ) ;
      size = comboBox1.Text ;
      this.Close() ;
    }
  }
}