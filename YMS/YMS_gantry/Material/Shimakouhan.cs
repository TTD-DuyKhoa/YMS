using Autodesk.Revit.DB ;
using System ;
using System.Collections.Generic ;
using System.IO ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;

namespace YMS_gantry.Material
{
  [MaterialCategory( "縞鋼板" )]
  public sealed class Shimakouhan : KouhanzaiSuper
  {
    public static new string typeName = "縞鋼板" ;

    public Shimakouhan() : base()
    {
    }

    public Shimakouhan( ElementId id, string koudaname, string size ) : base( id, koudaname, size )
    {
    }


    public static ElementId CreateShimakouhan( Document doc, string size, string thick, XYZ point, double rotate,
      XYZ adjustVec = null, Level level = null, double levelOffset = 0 )
    {
      ElementId id = ElementId.InvalidElementId ;
      try {
        ElementId retId = ElementId.InvalidElementId ;
        FamilySymbol sym ;
        string familyPath = Master.ClsKoubanzaiCsv.GetFamilyPath( size ) ;

        if ( ! GantryUtil.GetFamilySymbol( doc, familyPath, thick, out sym, true ) ) {
          return ElementId.InvalidElementId ;
        }

        //パラメータの組み合わせ作成
        Dictionary<string, string> paramList = new Dictionary<string, string>() ;
        //paramList.Add(DefineUtil.PARAM_MATERIAL, material);
        //paramList.Add("サイズ", $"{size}");

        using ( SubTransaction tr = new SubTransaction( doc ) ) {
          tr.Start() ;
          ElementId CreatedID = GantryUtil.CreateInstanceWith1point( doc, point, level.GetPlaneReference(), sym ).Id ;
          RevitUtil.ClsRevitUtil.RotateElement( doc, CreatedID, Line.CreateBound( point, point + XYZ.BasisZ ),
            rotate ) ;
          //sym = RevitUtil.ClsRevitUtil.ChangeTypeID(doc, sym, CreatedID, GantoryUtil.CreateTypeName(typeName, paramList));

          if ( adjustVec != null && adjustVec != XYZ.Zero ) {
            ElementTransformUtils.MoveElement( doc, CreatedID, adjustVec ) ;
          }

          RevitUtil.ClsRevitUtil.SetParameter( doc, CreatedID, DefineUtil.PARAM_BASE_OFFSET,
            RevitUtil.ClsRevitUtil.CovertToAPI( levelOffset ) ) ;

          retId = CreatedID ;
          tr.Commit() ;
        }

        return retId ;
      }
      catch ( Exception ex ) {
        id = ElementId.InvalidElementId ;
      }

      return id ;
    }

    public static ElementId CreateShimakouhanOnFace( Document doc, string size, string thick, XYZ point, double rotate,
      Reference refer, XYZ normal = null, XYZ adjustVec = null )
    {
      ElementId id = ElementId.InvalidElementId ;
      try {
        ElementId retId = ElementId.InvalidElementId ;
        FamilySymbol sym ;
        //if (!GantryUtil.GetFamilySymbol(doc, Path.Combine(PathUtil.GetFamilyMainDir(), PathUtil.GetFamilyFolder(typeof(KouhanzaiSuper))) + $"\\{typeName}\\{size}.rfa", thick, out sym, true))
        string familyPath = Master.ClsKoubanzaiCsv.GetFamilyPath( size ) ;
        if ( ! GantryUtil.GetFamilySymbol( doc, familyPath, thick, out sym, true ) ) {
          return ElementId.InvalidElementId ;
        }

        using ( SubTransaction tr = new SubTransaction( doc ) ) {
          tr.Start() ;

          FamilyInstance ins = GantryUtil.CreateInstanceWith1point( doc, point, refer, sym, normal ) ;
          ElementId CreatedID = ins.Id ;
          XYZ norm = ( doc.GetElement( refer.ElementId ) as FamilyInstance ).GetTransform().BasisZ ;

          if ( adjustVec != null && adjustVec != XYZ.Zero ) {
            ElementTransformUtils.MoveElement( doc, CreatedID, adjustVec ) ;
          }

          //スロープ時にズレが出るので修正
          Parameter param = ins.LookupParameter( DefineUtil.PARAM_HOST_OFFSET ) ;
          double dist = param.AsDouble() ;
          double real = RevitUtil.ClsRevitUtil.CovertFromAPI( dist ) ;
          Transform ts = ins.GetTransform() ;
          double rad = XYZ.BasisZ.AngleTo( ts.BasisZ ) ;
          if ( XYZ.BasisZ.Z != ts.BasisZ.Z && rad != 0 ) {
            XYZ or = point + ts.BasisZ * 100 ;
            XYZ oz = or - XYZ.BasisZ * 100 ;
            XYZ intPnt = GantryUtil.FindPerpendicularIntersection( Line.CreateBound( point, or ), oz ) ;
            XYZ vec = ( intPnt - oz ).Normalize() ;

            double dthick = RevitUtil.ClsCommonUtils.ChangeStrToDbl( thick.Replace( "PL", "" ) ) ;
            double dist2 = ( Math.Abs( real ) + dthick ) * Math.Sin( rad ) ;
            ElementTransformUtils.MoveElement( doc, CreatedID,
              vec * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( dist2 ) ) ;
          }

          RevitUtil.ClsRevitUtil.SetParameter( doc, CreatedID, DefineUtil.PARAM_HOST_OFFSET, 0 ) ;
          retId = CreatedID ;
          tr.Commit() ;
        }

        return retId ;
      }
      catch ( Exception ex ) {
        id = ElementId.InvalidElementId ;
      }

      return id ;
    }
  }
}