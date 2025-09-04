using Autodesk.Revit.DB ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using YMS_gantry.UI ;

namespace YMS_gantry
{
  [MaterialCategory( "敷桁" )]
  public sealed class Shikigeta : OhbikiSuper
  {
    public Shikigeta() : base()
    {
    }

    public Shikigeta( ElementId id, string koudaname, ShikigetaData data, int k = int.MinValue ) : base()
    {
      this.m_ElementId = id ;
      this.m_KodaiName = koudaname ;
      this.m_Size = data.ShikigetaSize ;
      this.m_Material = data.ShikigetaMaterial ;
      this.m_K = k ;
      this.m_ExStartLen = data.exShikigetaStartLeng ;
      this.m_ExEndLen = data.exShikigetaEndLeng ;
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
          if ( kData.shikigetaData.ShikigetaSize.StartsWith( "H" ) ||
               kData.shikigetaData.ShikigetaSize.Contains( "HA" ) ||
               kData.shikigetaData.ShikigetaSize.Contains( "SMH" ) ) {
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
      }

      return retList ;
    }

    /// <summary>
    /// 敷桁配置
    /// </summary>
    /// <returns></returns>
    public static List<ElementId> CreateShikiGeta( Document doc, OhbikiData oData, ShikigetaData data, XYZ start,
      XYZ end, Level level, string koudainame, double rotate = 0, double leveloffset = 0, FamilySymbol oriSym = null )
    {
      List<ElementId> retId = new List<ElementId>() ;
      FamilySymbol sym = oriSym ;
      if ( oriSym == null ) {
        string familyPath = Master.ClsMasterCsv.GetFamilyPath( data.ShikigetaSize ) ;
        if ( ! GantryUtil.GetFamilySymbol( doc, familyPath, shikiGetaName, out sym, true ) ) {
          return new List<ElementId>() ;
        }

        sym = GantryUtil.DuplicateTypeWithNameRule( doc, koudainame, sym, shikiGetaName ) ;
      }

      //パラメータの組み合わせ作成
      Dictionary<string, string> paramList = new Dictionary<string, string>() ;
      paramList.Add( DefineUtil.PARAM_MATERIAL, data.ShikigetaMaterial ) ;
      XYZ vec = ( end - start ).Normalize() ;
      start = start - vec * ( RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( data.exShikigetaStartLeng ) ) ;
      end = end + vec * ( RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( data.exShikigetaEndLeng ) ) ;
      double length = ( end - start ).GetLength() ;
      paramList.Add( DefineUtil.PARAM_LENGTH, $"{length}" ) ;

      using ( SubTransaction tr = new SubTransaction( doc ) ) {
        tr.Start() ;
        ElementId CreatedID = ElementId.InvalidElementId ;
        if ( data.ShikigetaSize.StartsWith( "H" ) ) {
          CreatedID = MaterialSuper.PlaceWithTwoPoints( sym, level.GetPlaneReference(), start, end,
            RevitUtil.ClsRevitUtil.CovertToAPI( leveloffset ) ) ;
          //sym = GantryUtil.ChangeInstanceTypeID(doc, sym, CreatedID, GantryUtil.CreateTypeName(shikiGetaName, paramList));

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
          MaterialSize kouzai = GantryUtil.GetKouzaiSize( data.ShikigetaSize ) ;

          for ( int i = 0 ; i < oData.OhbikiDan ; i++ ) {
            XYZ nstart = start + XYZ.BasisZ * ( kouzai.Height * i ) ;
            XYZ nend = end + XYZ.BasisZ * ( kouzai.Height * i ) ;
            double off = RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( leveloffset + ( kouzai.Height * i ) ) ;

            CreatedID = MaterialSuper.PlaceWithTwoPoints( sym, level.GetPlaneReference(), nstart, nend, off ) ;
            //sym = GantryUtil.ChangeInstanceTypeID(doc, sym, CreatedID, GantryUtil.CreateTypeName(shikiGetaName, paramList));

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