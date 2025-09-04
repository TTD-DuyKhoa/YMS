using Autodesk.Revit.DB ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;

namespace YMS_gantry.Material
{
  [MaterialCategory( "高さ調整材" )]
  public sealed class TakasaChouseizai : MaterialSuper
  {
    [MaterialProperty( "H1" )]
    public int m_H1 { get ; set ; } = 0 ;

    [MaterialProperty( "H2" )]
    public int m_H2 { get ; set ; } = 0 ;

    [MaterialProperty( "W" )]
    public int m_W { get ; set ; } = 0 ;

    [MaterialProperty( "D" )]
    public int m_D { get ; set ; } = 0 ;

    public new string typeName = "高さ調整プレート" ;

    public TakasaChouseizai() : base()
    {
    }

    public TakasaChouseizai( ElementId id, string koudaiName, string material, string size, int W, int D, int H1,
      int H2 ) : base( id, koudaiName, material, size )
    {
      m_W = W ;
      m_D = D ;
      m_H1 = H1 ;
      m_H2 = H2 ;
    }
  }
}