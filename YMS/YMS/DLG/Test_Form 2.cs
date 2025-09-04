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
using Autodesk.Revit.DB ;
using RevitUtil ;
using Autodesk.Revit.UI ;

namespace YMS.DLG
{
  public partial class Test_Form2 : System.Windows.Forms.Form
  {
    public Document m_doc ;
    public int m_length ;

    public Test_Form2( ExternalEvent exEvent, RequestHandler handler )
    {
      InitializeComponent() ;
    }

    public void InitControl()
    {
    }

    private void Test_Form_Load( object sender, EventArgs e )
    {
    }

    public Test_Form2( Document doc )
    {
      InitializeComponent() ;
      m_doc = doc ;

      InitControl() ;
    }

    private void btnOK_Click( object sender, EventArgs e )
    {
      if ( ! int.TryParse( textBox1.Text, out m_length ) ) {
        MessageBox.Show( "半角数字を入力してください。" ) ;
      }

      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btnCancel_Click( object sender, EventArgs e )
    {
      this.DialogResult = DialogResult.Cancel ;
      this.Close() ;
    }

    private void textBox1_KeyPress( object sender, KeyPressEventArgs e )
    {
      // 入力された文字が半角数字でない場合は入力をキャンセルする
      if ( ! char.IsDigit( e.KeyChar ) && ! char.IsControl( e.KeyChar ) ) {
        e.Handled = true ;
      }
    }
  }
}