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
  public partial class DlgKobetsuHaichi_Jizai : Form
  {
    public enum EnumPtn
    {
      typeA = 0,
      typeB,
      typeC
    }

    public bool m_isLeft { get ; set ; }
    public EnumPtn m_ptn { get ; set ; }

    public DlgKobetsuHaichi_Jizai()
    {
      InitializeComponent() ;
    }

    /// <summary>
    /// Aパターン左
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void button3_Click( object sender, EventArgs e )
    {
      m_ptn = EnumPtn.typeA ;
      m_isLeft = true ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    /// <summary>
    /// Aパターン右
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void button4_Click( object sender, EventArgs e )
    {
      m_ptn = EnumPtn.typeA ;
      m_isLeft = false ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    /// <summary>
    /// Bパターン
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void button5_Click( object sender, EventArgs e )
    {
      m_ptn = EnumPtn.typeB ;
      //m_isLeft = true;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }


    /// <summary>
    /// Cパターン左
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void button1_Click( object sender, EventArgs e )
    {
      m_ptn = EnumPtn.typeC ;
      m_isLeft = true ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    /// <summary>
    /// Cパターン右
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void button2_Click( object sender, EventArgs e )
    {
      m_ptn = EnumPtn.typeC ;
      m_isLeft = false ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }
  }
}