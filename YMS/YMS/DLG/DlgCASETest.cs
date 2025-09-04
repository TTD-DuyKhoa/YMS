using Autodesk.Revit.UI ;
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
  public partial class DlgCASETest : System.Windows.Forms.Form
  {
    private RequestHandler m_Handler ;
    private ExternalEvent m_ExEvent ;

    public DlgCASETest( ExternalEvent exEvent, RequestHandler handler )
    {
      InitializeComponent() ;
      m_Handler = handler ;
      m_ExEvent = exEvent ;
    }

    private void DlgCASETest_Load( object sender, EventArgs e )
    {
      //図面のCASEデータを取得し更新する処理
      MakeRequest( RequestId.UpdateCASE ) ;
    }

    /// <summary>
    ///   WakeUp -> enable all controls
    /// </summary>
    /// 
    public void WakeUp()
    {
      EnableCommands( true ) ;
    }

    /// <summary>
    ///   Control enabler / disabler 
    /// </summary>
    ///
    private void EnableCommands( bool status )
    {
      foreach ( Control ctrl in this.Controls ) {
        ctrl.Enabled = status ;
      }

      if ( ! status ) {
        //this.btnCancel.Enabled = true;
      }
    }

    private void DlgCASETest_FormClosed( object sender, FormClosedEventArgs e )
    {
      //初期化処理
      MakeRequest( RequestId.End ) ;
      m_ExEvent.Dispose() ;
      m_ExEvent = null ;
      m_Handler = null ;
    }

    private void MakeRequest( RequestId request )
    {
      m_Handler.Request.Make( request ) ;
      m_ExEvent.Raise() ;
      DozeOff() ;
    }

    /// <summary>
    ///   DozeOff -> disable all controls (but the Exit button)
    /// </summary>
    /// 
    private void DozeOff()
    {
      EnableCommands( false ) ;
    }

    public bool SetDataGridViewUp( List<string> allCASEList )
    {
      if ( allCASEList != null ) {
        var duplicates = allCASEList.GroupBy( x => x ).Where( x => x.Count() > 1 )
          .Select( x => new { Item = x.Key, Count = x.Count().ToString() } ).ToList() ;
        var duplicates1 = allCASEList.GroupBy( x => x ).Where( x => x.Count() == 1 )
          .Select( x => new { Item = x.Key, Count = x.Count().ToString() } ).ToList() ;
        int num = 0 ;
        for ( int i = 0 ; i < duplicates.Count ; i++ ) {
          dataGridView1.Rows.Add() ;
          dataGridView1.Rows[ i ].Cells[ 0 ].Value = duplicates[ i ].Item ;
          dataGridView1.Rows[ i ].Cells[ 1 ].Value = duplicates[ i ].Count ;
          num++ ;
        }

        for ( int i = 0 ; i < duplicates1.Count ; i++ ) {
          dataGridView1.Rows.Add() ;
          dataGridView1.Rows[ num ].Cells[ 0 ].Value = duplicates1[ i ].Item ;
          dataGridView1.Rows[ num ].Cells[ 1 ].Value = duplicates1[ i ].Count ;
          num++ ;
        }
      }

      return true ;
    }

    public bool SetDataGridViewLo( List<string> allCASEList )
    {
      if ( allCASEList != null ) {
        //初期化
        dataGridView2.Rows.Clear() ;
        var duplicates = allCASEList.GroupBy( x => x ).Where( x => x.Count() > 1 )
          .Select( x => new { Item = x.Key, Count = x.Count().ToString() } ).ToList() ;
        var duplicates1 = allCASEList.GroupBy( x => x ).Where( x => x.Count() == 1 )
          .Select( x => new { Item = x.Key, Count = x.Count().ToString() } ).ToList() ;
        int num = 0 ;
        for ( int i = 0 ; i < duplicates.Count ; i++ ) {
          dataGridView2.Rows.Add() ;
          dataGridView2.Rows[ i ].Cells[ 0 ].Value = duplicates[ i ].Item ;
          dataGridView2.Rows[ i ].Cells[ 1 ].Value = duplicates[ i ].Count ;
          num++ ;
        }

        for ( int i = 0 ; i < duplicates1.Count ; i++ ) {
          if ( duplicates1[ i ].Item != "" ) {
            dataGridView2.Rows.Add() ;
            dataGridView2.Rows[ num ].Cells[ 0 ].Value = duplicates1[ i ].Item ;
            dataGridView2.Rows[ num ].Cells[ 1 ].Value = duplicates1[ i ].Count ;
            num++ ;
          }
        }
      }

      return true ;
    }

    private void button1_Click( object sender, EventArgs e )
    {
      MakeRequest( RequestId.UpdateSelectCASE ) ;
    }
  }
}