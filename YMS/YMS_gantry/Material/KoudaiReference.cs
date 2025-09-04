using Autodesk.Revit.DB ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;

namespace YMS_gantry.Material
{
  [MaterialCategory( "構台基準点" )]
  public sealed class KoudaiReference : MaterialSuper
  {
    public static new string Name = "KoudaiReference" ;
    public static string familyName = "構台基点ファミリ" ;

    public KoudaiReference() : base()
    {
    }

    public KoudaiReference( Document doc, string koudaiName, ElementId id ) : base()
    {
      this.m_Document = doc ;
      this.m_KodaiName = koudaiName ;
      this.m_ElementId = id ;
    }

    public double GetRotate()
    {
      double rotate = 0 ;
      if ( this.m_ElementId == ElementId.InvalidElementId ) {
        return 0 ;
      }

      FamilyInstance inst = m_Document.GetElement( m_ElementId ) as FamilyInstance ;
      if ( inst == null ) {
        return 0 ;
      }

      LocationPoint l = inst.Location as LocationPoint ;
      return l.Rotation ;
    }
  }
}