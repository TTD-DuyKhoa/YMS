using System ;
using System.Collections.Generic ;
using System.ComponentModel ;
using System.Data ;
using System.Drawing ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;

namespace YMS_gantry.UI
{
  public partial class YmsNumericInteger : NumericUpDown
  {
    public int DoubleValue
    {
      get => (int) Value ;
      set { Value = (decimal) value ; }
    }

    public YmsNumericInteger()
    {
      InitializeComponent() ;
      Maximum = int.MaxValue ;
      Minimum = 0 ;
      TextAlign = HorizontalAlignment.Right ;
    }

    protected override void OnPaint( PaintEventArgs pe )
    {
      base.OnPaint( pe ) ;
    }

    protected override void OnValueChanged( EventArgs e )
    {
      DoubleValue = (int) Value ;
      base.OnValueChanged( e ) ;
    }
  }
}