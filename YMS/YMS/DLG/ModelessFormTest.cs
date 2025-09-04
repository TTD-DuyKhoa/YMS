using Autodesk.Revit.UI ;
using System ;
using System.Windows.Forms ;

namespace YMS.DLG
{
  public partial class ModelessFormTest : Form
  {
    private RequestHandler m_Handler ;
    private ExternalEvent m_ExEvent ;

    //private UIDocument uidocForm;
    public ModelessFormTest( ExternalEvent exEvent, RequestHandler handler )
    {
      InitializeComponent() ;
      m_Handler = handler ;
      m_ExEvent = exEvent ;
      //uidocForm = uidoc;
    }

    private void buttonOK_Click( object sender, EventArgs e )
    {
      MessageBox.Show( "Hello World" ) ;
      MakeRequest( RequestId.PutHari ) ;
      //ダイアログの再表示
    }

    private void ModelessFormTest_Shown( object sender, EventArgs e )
    {
      //ClsTest.testCommand4(uidocForm);
    }

    private void buttonCancel_Click( object sender, EventArgs e )
    {
      this.Close() ;
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
        this.buttonCancel.Enabled = true ;
      }
    }

    private void ModelessFormTest_FormClosed( object sender, FormClosedEventArgs e )
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

    private void buttonWarituke_Click( object sender, EventArgs e )
    {
      //割付処理
      MakeRequest( RequestId.PutHari ) ;
    }

    private void buttonTyouseizaiPut_Click( object sender, EventArgs e )
    {
      //調整材配置
      MakeRequest( RequestId.PutTyouseizai ) ;
    }
  }
}