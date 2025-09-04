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
  public partial class DlgCreateShimetukePiece : System.Windows.Forms.Form
  {
    #region メンバー変数

    public Document m_doc ;
    public ClsShimetukePiece m_ClsShimetukePiece ;

    #endregion

    public DlgCreateShimetukePiece()
    {
      InitializeComponent() ;
      m_ClsShimetukePiece = new ClsShimetukePiece() ;
      InitComboBoxBuzaiSize() ;
    }

    private void InitComboBoxBuzaiSize()
    {
      string selectedText = cmbBuzaiSize.Text ;
      cmbBuzaiSize.Items.Clear() ;
      List<string> lstStr = new List<string>() ;
      lstStr = Master.ClsShimetukePieceCsv.GetSizeList() ;
      foreach ( string str in lstStr ) {
        cmbBuzaiSize.Items.Add( str ) ;
      }

      if ( lstStr.Contains( selectedText ) ) {
        cmbBuzaiSize.Text = selectedText ;
      }
      else {
        cmbBuzaiSize.SelectedIndex = 0 ;
      }
    }

    private void SetData()
    {
      ClsShimetukePiece clsShimetukePiece = m_ClsShimetukePiece ;

      clsShimetukePiece.m_buzaiSize = cmbBuzaiSize.Text ;
    }

    #region コントロールイベント

    private void btnOK_Click( object sender, EventArgs e )
    {
      SetData() ;

      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btnCancel_Click( object sender, EventArgs e )
    {
      this.DialogResult = DialogResult.Cancel ;
      this.Close() ;
    }

    #endregion
  }
}