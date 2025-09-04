using System ;
using System.Collections.Generic ;
using System.ComponentModel ;
using System.Data ;
using System.Drawing ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows ;
using System.Windows.Forms ;
using static YMS.Parts.ClsSanbashiKui ;

namespace YMS.DLG
{
  public partial class DlgChangeSanbashikuiKenyoukui : Form
  {
    public PileType m_PileType { get ; set ; }

    public DlgChangeSanbashikuiKenyoukui()
    {
      InitializeComponent() ;
    }

    private void SetData()
    {
      // 指定したグループ内でチェックされているラジオボタンを取得する
      var RadioButtonChecked_InGroup =
        grpPileType.Controls.OfType<RadioButton>().SingleOrDefault( rb => rb.Checked == true ) ;
      switch ( RadioButtonChecked_InGroup.Text ) {
        case "桟橋杭(支持杭)" :
          m_PileType = PileType.Sanbashi ;
          break ;
        case "兼用杭" :
          m_PileType = PileType.Kenyou ;
          break ;
        case "断面変化杭" :
          m_PileType = PileType.DanmenHenka ;
          break ;
        case "TC杭" :
          m_PileType = PileType.TC ;
          break ;
        default :
          break ;
      }
    }

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
  }
}