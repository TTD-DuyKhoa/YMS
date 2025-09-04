using Autodesk.Revit.DB ;
using System ;
using System.Collections.Generic ;
using System.IO ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;

namespace YMS_gantry.Material
{
  [MaterialCategory( "手摺支柱" )]
  public sealed class TesuriShichu : MaterialSuper
  {
    public static new string typeName = "手摺支柱" ;

    public TesuriShichu() : base()
    {
    }

    public TesuriShichu( ElementId id, string koudaname, string size, string material ) : base( id, koudaname, size,
      material )
    {
    }

    public ElementId CreateTesuriShichu( Document doc, string koudainame, XYZ pnt, string size, string material,
      Reference refer, double offset, double height, XYZ normal, FamilySymbol orSym = null )
    {
      ElementId retId = ElementId.InvalidElementId ;
      try {
        FamilySymbol sym = orSym ;

        if ( orSym == null ) {
          string familyPath = Master.ClsTesuriCsv.GetFamilyPath( size, Master.ClsTesuriCsv.TypeTesuriShichu ) ;
          string familyName = Path.GetFileNameWithoutExtension( familyPath ) ;
          if ( ! GantryUtil.GetFamilySymbol( doc, familyPath, familyName, out sym, true ) ) {
            return ElementId.InvalidElementId ;
          }

          sym = GantryUtil.DuplicateTypeWithNameRule( doc, koudainame, sym, familyName ) ;
        }

        //パラメータの組み合わせ作成
        Dictionary<string, string> paramList = new Dictionary<string, string>() ;
        paramList.Add( DefineUtil.PARAM_MATERIAL, material ) ;
        paramList.Add( "サイズ", $"{size}" ) ;
        paramList.Add( DefineUtil.PARAM_LENGTH, $"{height}" ) ;

        using ( SubTransaction tr = new SubTransaction( doc ) ) {
          tr.Start() ;
          ElementId CreatedID = GantryUtil.CreateInstanceWith1point( doc, pnt, refer, sym, normal ).Id ;
          //sym = RevitUtil.ClsRevitUtil.ChangeTypeID(doc, sym, CreatedID, GantryUtil.CreateTypeName(size, paramList));
          var elem = doc.GetElement( refer ) ;
          if ( elem is Level ) {
            RevitUtil.ClsRevitUtil.SetParameter( doc, CreatedID, DefineUtil.PARAM_BASE_OFFSET,
              RevitUtil.ClsRevitUtil.CovertToAPI( offset ) ) ;
          }
          else {
            RevitUtil.ClsRevitUtil.SetParameter( doc, CreatedID, DefineUtil.PARAM_HOST_OFFSET,
              RevitUtil.ClsRevitUtil.CovertToAPI( offset ) ) ;
          }

          //タイプパラメータ設定
          foreach ( KeyValuePair<string, string> kv in paramList ) {
            GantryUtil.SetParameterValueByParameterName( doc.GetElement( CreatedID ) as FamilyInstance, kv.Key,
              kv.Value ) ;
          }

          retId = CreatedID ;
          tr.Commit() ;
        }

        return retId ;
      }
      catch ( Exception ex ) {
        retId = ElementId.InvalidElementId ;
      }

      return retId ;
    }
  }
}