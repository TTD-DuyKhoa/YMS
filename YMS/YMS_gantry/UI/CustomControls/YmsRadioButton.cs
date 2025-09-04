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
  public partial class YmsRadioButton : RadioButton
  {
    public YmsRadioButton()
    {
      InitializeComponent() ;
    }

    protected override void OnPaint( PaintEventArgs pe )
    {
      base.OnPaint( pe ) ;
      foreach ( var binding in this.DataBindings.Cast<Binding>()
                 .Where( x => x.PropertyName == nameof( RadioButton.Checked ) ) ) {
        binding.ControlUpdateMode = ControlUpdateMode.Never ;
      }
    }
  }
}