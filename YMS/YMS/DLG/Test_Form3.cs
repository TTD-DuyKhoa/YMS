using Autodesk.Revit.DB ;
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
  public partial class Test_Form3 : System.Windows.Forms.Form
  {
    public string levelName ;
    private Document m_doc ;

    public Test_Form3( Document doc )
    {
      InitializeComponent() ;
      m_doc = doc ;
    }

    private void Test_Form3_Load( object sender, EventArgs e )
    {
      cmbLevel.Items.AddRange( ClsYMSUtil.GetLevelNames( m_doc ).ToArray() ) ;
    }

    private void btnOK_Click( object sender, EventArgs e )
    {
      levelName = cmbLevel.Text ;
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