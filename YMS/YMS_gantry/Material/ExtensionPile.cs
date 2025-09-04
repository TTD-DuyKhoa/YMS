using Autodesk.Revit.DB ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;

namespace YMS_gantry
{
  [MaterialCategory( "継足し杭" )]
  public sealed class ExtensionPile : PilePillerSuper
  {
    public ExtensionPile() : base()
    {
    }

    public ExtensionPile( ElementId _id, string koudaname, PilePillerData data, int kyoutyouNum = int.MinValue,
      int hukuinNum = int.MinValue ) : base( _id, koudaname, data, kyoutyouNum, hukuinNum )
    {
    }
  }
}