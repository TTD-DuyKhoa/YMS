using Autodesk.Revit.DB ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;

namespace YMS_gantry.Material
{
  [MaterialCategory( "補助ピース" )]
  public sealed class HojoPieace : MaterialSuper
  {
    [MaterialProperty( "H1" )]
    public int m_H1 { get ; set ; } = 0 ;

    [MaterialProperty( "H2" )]
    public int m_H2 { get ; set ; } = 0 ;

    [MaterialProperty( "エンドプレート上W" )]
    public int m_TopW { get ; set ; } = 0 ;

    [MaterialProperty( "エンドプレート上D" )]
    public int m_TopD { get ; set ; } = 0 ;

    [MaterialProperty( "エンドプレート上H" )]
    public int m_TopH { get ; set ; } = 0 ;

    [MaterialProperty( "エンドプレート上種類" )]
    public string m_TopType { get ; set ; } = "ｴﾝﾄﾞﾌﾟﾚｰﾄ任意" ;


    [MaterialProperty( "エンドプレート下W" )]
    public int m_EndW { get ; set ; } = 0 ;

    [MaterialProperty( "エンドプレート下D" )]
    public int m_EndD { get ; set ; } = 0 ;

    [MaterialProperty( "エンドプレート下H" )]
    public int m_EndH { get ; set ; } = 0 ;

    [MaterialProperty( "エンドプレート下種類" )]
    public string m_EndType { get ; set ; } = "ｴﾝﾄﾞﾌﾟﾚｰﾄ任意" ;


    public new string typeName = "高さ調整プレート" ;

    public HojoPieace() : base()
    {
    }

    public HojoPieace( ElementId id, string koudaiName, string material, string size, int topW, int topD, int topH,
      int endW, int endD, int endH, int H1, int H2 ) : base( id, koudaiName, material, size )
    {
      m_TopW = topW ;
      m_TopD = topD ;
      m_TopH = topH ;
      m_EndW = endW ;
      m_EndD = endD ;
      m_EndH = endH ;
      m_H1 = H1 ;
      m_H2 = H2 ;
    }
  }
}