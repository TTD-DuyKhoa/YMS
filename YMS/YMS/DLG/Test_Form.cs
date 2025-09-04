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
  public partial class Test_Form : System.Windows.Forms.Form
  {
    public Document m_doc ;
    public string m_level ;

    public Test_Form( ExternalEvent exEvent, RequestHandler handler )
    {
      InitializeComponent() ;
    }

    public void InitControl()
    {
      cmbHaichiLevel.Items.AddRange( ClsYMSUtil.GetLevelNames( m_doc ).ToArray() ) ;
    }

    private void Test_Form_Load( object sender, EventArgs e )
    {
    }

    public Test_Form( Document doc )
    {
      InitializeComponent() ;
      m_doc = doc ;

      InitControl() ;
    }

    private void btnOK_Click( object sender, EventArgs e )
    {
      m_level = cmbHaichiLevel.Text ;

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