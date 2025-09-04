using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;

namespace YMS_gantry.Material
{
  [MaterialCategory( "方杖" )]
  public sealed class Houdue : MaterialSuper
  {
    [MaterialProperty( "主材" )]
    public bool m_Syuzai { get ; set ; } = false ;

    //火打ち受けにつく主材か
    public bool IsSyuzai { get ; set ; } = false ;
  }

  [MaterialCategory( "取付補助材" )]
  public sealed class HoudueNeko : MaterialSuper
  {
    public const string neko25 = "CL25ﾈｺ材" ;
    public const string neko30 = "CL30ﾈｺ材" ;
  }
}