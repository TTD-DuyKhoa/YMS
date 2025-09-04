//using Autodesk.Revit.DB;

using Autodesk.Revit.UI ;
using System ;
using System.Windows.Forms ;
using YMS.Parts ;

namespace YMS.DLG
{
  public partial class spForm : Form //IDockablePaneProvider//Form, System.Windows.Controls.Page,
  {
    public spForm()
    {
      InitializeComponent() ;
    }

    private void button2_Click( object sender, EventArgs e )
    {
      //DlgCreateHaraokoshiBase dl = new DlgCreateHaraokoshiBase();
      //dl.Show();
    }

    private void button1_Click( object sender, EventArgs e )
    {
    }

    private void button3_Click( object sender, EventArgs e )
    {
    }

    private void button4_Click( object sender, EventArgs e )
    {
      //DlgCreateKiribari dl = new DlgCreateKiribari();
      //dl.Show();
    }

    private void button5_Click( object sender, EventArgs e )
    {
    }

    private void button6_Click( object sender, EventArgs e )
    {
    }

    private void button7_Click( object sender, EventArgs e )
    {
    }

    private void button8_Click( object sender, EventArgs e )
    {
      //DlgCreateKouyaita dl = new DlgCreateKouyaita();
      //dl.Show();
    }

    private void button9_Click( object sender, EventArgs e )
    {
      //DlgCreateRenzokukabe dl = new DlgCreateRenzokukabe();
      //dl.Show();
    }

    private void button10_Click( object sender, EventArgs e )
    {
      //ClsSMW clsSMW = new ClsSMW();
      //DlgCreateSMW dl = new DlgCreateSMW(clsSMW);
      //dl.Show();
    }

    private void button11_Click( object sender, EventArgs e )
    {
      //DlgCreateOyakui dl = new DlgCreateOyakui();
      //dl.Show();
    }

    private void button12_Click( object sender, EventArgs e )
    {
    }

    private void button13_Click( object sender, EventArgs e )
    {
      //DlgCASE dl = new DlgCASE();
      //dl.Show();
    }

    private void button14_Click( object sender, EventArgs e )
    {
      DlgCreateYokoyaita dl = new DlgCreateYokoyaita() ;
      dl.Show() ;
    }

    private void button15_Click( object sender, EventArgs e )
    {
    }

    private void button16_Click( object sender, EventArgs e )
    {
    }

    private void button17_Click( object sender, EventArgs e )
    {
      //DlgCreateJack dl = new DlgCreateJack();
      //dl.Show();
    }

    private void button18_Click( object sender, EventArgs e )
    {
      //DlgCreateCornerHiuchiBase dl = new DlgCreateCornerHiuchiBase();
      //dl.Show();
    }

    private void button19_Click( object sender, EventArgs e )
    {
      ClsKiribariHiuchiBase khb = new ClsKiribariHiuchiBase() ;
      DlgCreateKiribariHiuchiBase dl = new DlgCreateKiribariHiuchiBase( khb ) ;
      dl.Show() ;
    }

    private void button20_Click( object sender, EventArgs e )
    {
      ClsKiribariUkeBase kub = new ClsKiribariUkeBase() ;
      DlgCreateKiribariUkeBase dl = new DlgCreateKiribariUkeBase( kub ) ;
      dl.Show() ;
    }

    private void button21_Click( object sender, EventArgs e )
    {
      ClsKiribariTsunagizaiBase ktb = new ClsKiribariTsunagizaiBase() ;
      DlgCreateKiribariTsunagizaiBase dl = new DlgCreateKiribariTsunagizaiBase( ktb ) ;
      dl.Show() ;
    }

    private void button22_Click( object sender, EventArgs e )
    {
      ClsHiuchiTsunagizaiBase htb = new ClsHiuchiTsunagizaiBase() ;
      DlgCreateHiuchiTsunagizaiBase dl = new DlgCreateHiuchiTsunagizaiBase( htb ) ;
      dl.Show() ;
    }

    private void button23_Click( object sender, EventArgs e )
    {
      ClsKiribariTsugiBase KT = new ClsKiribariTsugiBase() ;
      DlgCreateKiribariTsugiBase dl = new DlgCreateKiribariTsugiBase( KT ) ;
      dl.Show() ;
    }

    private void button24_Click( object sender, EventArgs e )
    {
    }

    private void button25_Click( object sender, EventArgs e )
    {
      //DlgChangeLength dl = new DlgChangeLength();
      //dl.Show();
    }

    private void button26_Click( object sender, EventArgs e )
    {
    }

    private void button27_Click( object sender, EventArgs e )
    {
      //DlgCreateStiffener dl = new DlgCreateStiffener();
      //dl.Show();
    }

    private void button28_Click( object sender, EventArgs e )
    {
    }

    private void button29_Click( object sender, EventArgs e )
    {
      DlgCreateSumibuPiece dl = new DlgCreateSumibuPiece() ;
      dl.Show() ;
    }

    private void button30_Click( object sender, EventArgs e )
    {
      DlgCreateJyougeHaraokoshiTsunagizai dl = new DlgCreateJyougeHaraokoshiTsunagizai() ;
      dl.Show() ;
    }

    private void button31_Click( object sender, EventArgs e )
    {
    }
  }
}