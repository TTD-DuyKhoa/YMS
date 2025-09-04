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
using YMS.Parts ;

namespace YMS.DLG
{
  public partial class DlgCreateAtamatsunagiSyuzai : System.Windows.Forms.Form
  {
    public double m_Length { get ; set ; }

    public DlgCreateAtamatsunagiSyuzai()
    {
      InitializeComponent() ;
      m_Length = 0 ;
    }

    private void btn10_Click( object sender, EventArgs e )
    {
      m_Length = 1.0 ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btn15_Click( object sender, EventArgs e )
    {
      m_Length = 1.5 ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btn20_Click( object sender, EventArgs e )
    {
      m_Length = 2.0 ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btn25_Click( object sender, EventArgs e )
    {
      m_Length = 2.5 ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btn30_Click( object sender, EventArgs e )
    {
      m_Length = 3.0 ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btn35_Click( object sender, EventArgs e )
    {
      m_Length = 3.5 ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btn40_Click( object sender, EventArgs e )
    {
      m_Length = 4.0 ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btn45_Click( object sender, EventArgs e )
    {
      m_Length = 4.5 ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btn50_Click( object sender, EventArgs e )
    {
      m_Length = 5.0 ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btn55_Click( object sender, EventArgs e )
    {
      m_Length = 5.5 ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btn60_Click( object sender, EventArgs e )
    {
      m_Length = 6.0 ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btn65_Click( object sender, EventArgs e )
    {
      m_Length = 6.5 ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btn70_Click( object sender, EventArgs e )
    {
      m_Length = 7.0 ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btnUndo_Click( object sender, EventArgs e )
    {
      m_Length = 0.0 ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btnEnd_Click( object sender, EventArgs e )
    {
      this.DialogResult = DialogResult.Cancel ;
      this.Close() ;
    }
  }
}