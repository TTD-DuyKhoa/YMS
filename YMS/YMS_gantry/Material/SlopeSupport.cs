using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;

namespace YMS_gantry
{
  /// <summary>
  /// スロープ補助部材
  /// </summary>
  public abstract class SlopeSupport : MaterialSuper
  {
    [MaterialProperty( "K" )]
    public int m_K { get ; set ; } = 0 ;
  }
}