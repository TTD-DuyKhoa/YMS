using Autodesk.Revit.DB ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using YMS_gantry.UI ;

namespace YMS_gantry
{
  [MaterialCategory( "大引" )]
  public sealed class Ohbiki : OhbikiSuper
  {
    public Ohbiki() : base()
    {
    }

    public Ohbiki( ElementId id, string koudaname, OhbikiData data, int k = int.MinValue ) : base( id, koudaname, data,
      k )
    {
    }

    /// <summary>
    /// 配置位置算出
    /// </summary>
    /// <returns></returns>
    public static Dictionary<XYZ, string> CalcArrangementPoint( AllKoudaiFlatFrmData kData, XYZ basePnt,
      XYZ vecKyoutyou )
    {
      Dictionary<XYZ, string> retList = new Dictionary<XYZ, string>() ;
      XYZ newP = basePnt ;
      for ( int hC = 0 ; hC < kData.KyoutyouPillarPitch.Count ; hC++ ) {
        newP = newP + vecKyoutyou.Normalize() *
          ( RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( kData.KyoutyouPillarPitch[ hC ] ) ) ;
        if ( ( hC == 0 && kData.IsFirstShikigeta ) ||
             ( hC == kData.KyoutyouPillarPitch.Count - 1 && kData.IsLastShikigeta ) ) {
          continue ;
        }

        if ( kData.ohbikiData.isHkou ) {
          retList.Add( newP, $"{hC}" ) ;
        }
        else {
          MaterialSize pS = GantryUtil.GetKouzaiSize( kData.pilePillerData.PillarSize ) ;
          retList.Add(
            newP + vecKyoutyou.Normalize() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( ( pS.Height / 2 ) ),
            $"F-{hC}" ) ; //橋長方向側
          retList.Add(
            newP - vecKyoutyou.Normalize() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( ( pS.Height / 2 ) ),
            $"B-{hC}" ) ; //原点側
        }
      }

      return retList ;
    }


    /// <summary>
    /// 大引配置
    /// </summary>
    /// <returns></returns>
    public static List<ElementId> CreateOhbiki( Document doc, OhbikiData data, XYZ start, XYZ end, Level level,
      bool isDoboku, string koudainame, double leveloffset = 0, double rotate = 0, FamilySymbol oriSym = null,
      bool reverse = false )
    {
      List<ElementId> retId = new List<ElementId>() ;

      FamilySymbol sym = oriSym ;
      if ( oriSym == null ) {
        string familyPath = Master.ClsMasterCsv.GetFamilyPath( data.OhbikiSize ) ;
        string type = isDoboku ? "桁受" : typeName ;
        if ( ! GantryUtil.GetFamilySymbol( doc, familyPath, type, out sym, true ) ) {
          return new List<ElementId>() ;
        }

        sym = GantryUtil.DuplicateTypeWithNameRule( doc, koudainame, sym, type ) ;
      }

      MaterialSize kouzai = GantryUtil.GetKouzaiSize( data.OhbikiSize ) ;
      //パラメータの組み合わせ作成
      Dictionary<string, string> paramList = new Dictionary<string, string>() ;
      paramList.Add( DefineUtil.PARAM_MATERIAL, data.OhbikiMaterial ) ;
      XYZ vec = ( end - start ).Normalize() ;
      if ( reverse ) {
        start = start - vec * ( RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( data.exOhbikiEndLeng ) ) ;
        end = end + vec * ( RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( data.exOhbikiStartLeng ) ) ;
      }
      else {
        start = start - vec * ( RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( data.exOhbikiStartLeng ) ) ;
        end = end + vec * ( RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( data.exOhbikiEndLeng ) ) ;
      }

      double length = ( end - start ).GetLength() ;
      paramList.Add( DefineUtil.PARAM_LENGTH, $"{length}" ) ;

      using ( SubTransaction tr = new SubTransaction( doc ) ) {
        tr.Start() ;
        ElementId CreatedID = ElementId.InvalidElementId ;
        if ( data.isHkou ) {
          CreatedID = MaterialSuper.PlaceWithTwoPoints( sym, level.GetPlaneReference(), start, end,
            RevitUtil.ClsRevitUtil.CovertToAPI( leveloffset ) ) ;
          //sym = GantryUtil.ChangeInstanceTypeID(doc, sym, CreatedID, GantryUtil.CreateTypeName(typeName, paramList));

          //タイプパラメータ設定
          foreach ( KeyValuePair<string, string> kv in paramList ) {
            GantryUtil.SetParameterValueByParameterName( doc.GetElement( CreatedID ) as FamilyInstance, kv.Key,
              kv.Value ) ;
          }

          //エンドプレート
          RevitUtil.ClsRevitUtil.SetTypeParameter( sym, DefineUtil.PARAM_END_PLATE, (double) DefineUtil.PramYesNo.No ) ;

          //インスタンス変更
          RevitUtil.ClsRevitUtil.SetParameter( doc, CreatedID, DefineUtil.PARAM_LENGTH, ( start - end ).GetLength() ) ;

          retId.Add( CreatedID ) ;
        }
        else {
          paramList.Add( DefineUtil.PARAM_ROTATE, $"{rotate / ( 180 / Math.PI )}" ) ;
          for ( int i = 0 ; i < data.OhbikiDan ; i++ ) {
            XYZ nstart = start + XYZ.BasisZ * ( kouzai.Height * i ) ;
            XYZ nend = end + XYZ.BasisZ * ( kouzai.Height * i ) ;
            double off = RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( leveloffset + ( kouzai.Height * i ) ) ;

            CreatedID = MaterialSuper.PlaceWithTwoPoints( sym, level.GetPlaneReference(), nstart, nend, off ) ;
            //sym = GantryUtil.ChangeInstanceTypeID(doc, sym, CreatedID, GantryUtil.CreateTypeName(typeName, paramList));

            //タイプパラメータ設定
            foreach ( KeyValuePair<string, string> kv in paramList ) {
              GantryUtil.SetParameterValueByParameterName( doc.GetElement( CreatedID ) as FamilyInstance, kv.Key,
                kv.Value ) ;
            }

            //エンドプレート
            RevitUtil.ClsRevitUtil.SetTypeParameter( sym, DefineUtil.PARAM_END_PLATE,
              (double) DefineUtil.PramYesNo.No ) ;

            //インスタンス変更
            RevitUtil.ClsRevitUtil.SetParameter( doc, CreatedID, DefineUtil.PARAM_LENGTH,
              ( start - end ).GetLength() ) ;

            retId.Add( CreatedID ) ;
          }
        }

        tr.Commit() ;
      }

      return retId ;
    }
  }
}