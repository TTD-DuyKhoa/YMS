using Autodesk.Revit.DB ;
using System ;
using System.Collections.Generic ;
using System.IO ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using YMS_gantry.UI ;

namespace YMS_gantry.Material
{
  [MaterialCategory( "対傾構" )]
  public sealed class Taikeikou : MaterialSuper
  {
    public static new string typeName = "対傾構" ;

    [MaterialProperty( "jointType" )]
    public DefineUtil.eJoinType m_JointType { get ; set ; }

    [MaterialProperty( "BoltType" )]
    public DefineUtil.eBoltType m_BoltType { get ; set ; }

    [MaterialProperty( "BoltCount" )]
    public int m_BoltCount { get ; set ; }

    public Taikeikou() : base()
    {
    }

    public Taikeikou( ElementId id, string koudaname, string size, string material, DefineUtil.eJoinType joint,
      DefineUtil.eBoltType boltType, int boltC ) : base( id, koudaname, size, material )
    {
      m_JointType = joint ;
      m_BoltType = boltType ;
      m_BoltCount = boltC ;
    }

    /// <summary>
    /// 対傾構作成
    /// </summary>
    /// <returns></returns>
    public static string CreateTaikeikou( Document doc, Neda neda1, Neda neda2, Taikeikou taikeikou, XYZ point1,
      XYZ point2, double margin, bool mustDefault, bool hasEndStif )
    {
      string retSt = "" ;
      ElementId id = ElementId.InvalidElementId ;
      FamilySymbol sym = null ;
      double length = RevitUtil.ClsRevitUtil.CovertFromAPI( point1.DistanceTo( point2 ) ) ;
      string size = taikeikou.m_Size ;
      double teikeiSize = 90 ;

      try {
        SteelSize nedaSize = neda1.SteelSize ;
        SteelSize nedaSize2 = neda2.SteelSize ;
        FamilyInstance nedaIns1 = doc.GetElement( neda1.m_ElementId ) as FamilyInstance ;
        FamilyInstance nedaIns2 = doc.GetElement( neda2.m_ElementId ) as FamilyInstance ;
        MaterialSize kouzaiSize = GantryUtil.GetKouzaiSize( Master.ClsTaikeikouCsv.GetKouzaiSize( size ) ) ;

        Reference refer = GantryUtil.GetReference( nedaIns1 ) ;
        if ( refer != null ) {
          size = ( mustDefault && RevitUtil.ClsGeo.GEO_EQ( length, 2000 )
            ?
            Master.ClsTaikeikouCsv.GetSizeList( "2" ).First()
            : ( mustDefault && RevitUtil.ClsGeo.GEO_EQ( length, 3000 ) )
              ? Master.ClsTaikeikouCsv.GetSizeList( "3" ).First()
              : taikeikou.m_Size ) ;
          string familyPath = Master.ClsTaikeikouCsv.GetFamilyPath( size ) ;
          string type = Path.GetFileNameWithoutExtension( familyPath ) ;

          XYZ p1 = point1, p2 = point2 ;
          double offset = 0 ;
          GantryUtil.GetInstanceLevelAndOffset( doc, nedaIns1, ref offset ) ;

          XYZ vec = ( p2 - p1 ).Normalize() ;
          p1 = p1 + vec * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( margin ) ;
          p2 = p2 + vec.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( margin ) ;
          XYZ stfVec = vec.CrossProduct( nedaIns1.GetTransform().BasisZ ).Normalize() ;
          if ( RevitUtil.ClsGeo.IsLeft( stfVec, vec ) ) {
            stfVec = stfVec.Negate() ;
          }

          double taikeikouRo = nedaIns1.HandOrientation.Normalize().AngleTo( vec.Normalize() ) ;
          double rad = ( taikeikouRo > Math.PI / 2 ) ? Math.PI - taikeikouRo : taikeikouRo ;
          Curve nedaC = GantryUtil.GetCurve( doc, neda1.m_ElementId ) ;

          XYZ dotNeda = nedaIns1.HandOrientation.Normalize().CrossProduct( nedaIns1.GetTransform().BasisZ ) ;
          if ( GantryUtil.AreComponentsSameSign( vec, dotNeda ) ) {
            dotNeda = dotNeda.Negate() ;
          }

          XYZ nedaVec = nedaIns1.HandOrientation.Normalize() ;

          XYZ place1 = point1 ;
          XYZ place2 = point2 ;
          bool isNearP = false ;
          //直交する場合はオフセット計算無で配置
          if ( Math.Abs( Math.Abs( taikeikouRo ) - Math.PI / 2 ) < 0.1 ) {
            place1 = p1 ;
            place2 = p2 ;
            isNearP = true ;

            if ( ! GantryUtil.GetFamilySymbol( doc, familyPath, type, out sym, true ) ) {
              return $"ファミリのロードに失敗しました:{familyPath}" ;
            }
          }
          else {
            //最初は定型サイズのオフセット値で定型長さに該当するか確認
            if ( ! GantryUtil.AreComponentsSameSign( nedaVec, vec ) ) {
              nedaVec = nedaVec.Negate() ;
            }

            XYZ ep1 = p1 ;
            XYZ ep2 = p1 + stfVec.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( teikeiSize ) ;
            XYZ pp1 = ProjectPointToCurve( ep1, nedaC ) ;
            XYZ pp2 = ProjectPointToCurve( ep2, nedaC ) ;

            if ( pp1.DistanceTo( ep1 ) <= pp2.DistanceTo( ep2 ) ) {
              double mDist = margin / Math.Sin( rad ) ;
              place1 = point1 + vec * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( mDist ) ;
              mDist = mDist + ( teikeiSize / Math.Tan( rad ) ) ;
              place2 = point2 + vec.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( mDist ) ;
              isNearP = true ;
            }
            else {
              double mDist = margin / Math.Sin( rad ) ;
              place2 = point2 + vec.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( mDist ) ;
              mDist = mDist + ( teikeiSize / Math.Tan( rad ) ) ;
              place1 = point1 + vec * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( mDist ) ;
              isNearP = false ;
            }

            length = place1.DistanceTo( place2 ) ;
            if ( mustDefault && RevitUtil.ClsGeo.GEO_EQ( length, 2000 ) ) {
              size = Master.ClsTaikeikouCsv.GetSizeList( "2" ).First() ;
              familyPath = Master.ClsTaikeikouCsv.GetFamilyPath( size ) ;
              type = Path.GetFileNameWithoutExtension( familyPath ) ;
              if ( ! GantryUtil.GetFamilySymbol( doc, familyPath, type, out sym, true ) ) {
                return $"ファミリのロードに失敗しました:{familyPath}" ;
              }
            }
            else if ( mustDefault && RevitUtil.ClsGeo.GEO_EQ( length, 3000 ) ) {
              size = Master.ClsTaikeikouCsv.GetSizeList( "3" ).First() ;
              familyPath = Master.ClsTaikeikouCsv.GetFamilyPath( size ) ;
              type = Path.GetFileNameWithoutExtension( familyPath ) ;
              if ( ! GantryUtil.GetFamilySymbol( doc, familyPath, type, out sym, true ) ) {
                return $"ファミリのロードに失敗しました:{familyPath}" ;
              }
            }
            else {
              ep1 = p1 ;
              ep2 = p1 + stfVec.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( kouzaiSize.Width ) ;
              pp1 = ProjectPointToCurve( ep1, nedaC ) ;
              pp2 = ProjectPointToCurve( ep2, nedaC ) ;
              if ( pp1.DistanceTo( ep1 ) <= pp2.DistanceTo( ep2 ) ) {
                double mDist = margin / Math.Sin( rad ) ;
                place1 = point1 + vec * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( mDist ) ;
                mDist = mDist + ( kouzaiSize.Width / Math.Tan( rad ) ) ;
                place2 = point2 + vec.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( mDist ) ;
                isNearP = true ;
              }
              else {
                double mDist = margin / Math.Sin( rad ) ;
                place2 = point2 + vec.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( mDist ) ;
                mDist = mDist + ( kouzaiSize.Width / Math.Tan( rad ) ) ;
                place1 = point1 + vec * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( mDist ) ;
                isNearP = false ;
              }

              familyPath = Master.ClsTaikeikouCsv.GetFamilyPath( taikeikou.m_Size ) ;
              type = Path.GetFileNameWithoutExtension( familyPath ) ;
              if ( ! GantryUtil.GetFamilySymbol( doc, familyPath, type, out sym, true ) ) {
                return $"ファミリのロードに失敗しました:{familyPath}" ;
              }
            }
          }

          if ( sym != null ) {
            if ( ! sym.FamilyName.Contains( "TC" ) ) {
              sym = GantryUtil.DuplicateTypeWithNameRule( doc, taikeikou.m_KodaiName, sym, "対傾構" ) ;
              RevitUtil.ClsRevitUtil.SetTypeParameter( sym, DefineUtil.PARAM_LENGTH, place1.DistanceTo( place2 ) ) ;
            }

            id = MaterialSuper.PlaceWithTwoPoints( sym, refer, place1, place2,
              RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( offset ) ) ;
          }

          if ( id != ElementId.InvalidElementId ) {
            taikeikou.m_ElementId = id ;
            Taikeikou.WriteToElement( taikeikou, doc ) ;

            //構台の最大根太Index
            AllKoudaiFlatFrmData kData = GantryUtil.GetKoudaiData( doc, neda1.m_KodaiName ).AllKoudaiFlatData ;
            int maxHInd = kData.NedaPitch.Count - 1 ;

            //取付PL作図
            string kouzaiSizeS = neda1.m_Size ;
            MaterialSize mtSize = neda1.MaterialSize() ;
            if ( mtSize.Shape == MaterialShape.HA || mtSize.Shape == MaterialShape.SMH ) {
              kouzaiSizeS = Master.ClsKouzaiSpecify.GetKouzaiSize( kouzaiSizeS ) ;
            }

            //ウェブの厚み
            SteelSize stSize = GantryUtil.GetKouzaiSizeSunpou( kouzaiSizeS ) ;

            string kouzaiSizeS2 = neda2.m_Size ;
            MaterialSize mtSize2 = neda2.MaterialSize() ;
            if ( mtSize2.Shape == MaterialShape.HA || mtSize2.Shape == MaterialShape.SMH ) {
              kouzaiSizeS2 = Master.ClsKouzaiSpecify.GetKouzaiSize( kouzaiSizeS2 ) ;
            }

            //ウェブの厚み
            SteelSize stSize2 = GantryUtil.GetKouzaiSizeSunpou( kouzaiSizeS2 ) ;

            double thick = Master.ClsTaikeikouPLCsv.GetThick( kouzaiSizeS ) ;
            XYZ moveVec1, moveVec2, moveVevN1, moveVecN2 ;

            if ( ! isNearP ) {
              double mDist = ( ( stSize.WebThick / 2 ) / Math.Sin( rad ) ) + ( 1 / Math.Sin( rad ) ) ;
              moveVec1 = vec * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( mDist ) ;
              mDist = ( ( stSize.WebThick / 2 ) / Math.Sin( rad ) ) + ( 1 / Math.Sin( rad ) ) +
                      ( ( thick ) / Math.Tan( rad ) ) ;
              moveVevN1 = vec.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( mDist ) ;

              mDist = ( ( stSize.WebThick / 2 ) / Math.Sin( rad ) ) + ( 1 / Math.Sin( rad ) ) +
                      ( ( thick ) / Math.Tan( rad ) ) ;
              moveVec2 = vec.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( mDist ) ;
              mDist = ( ( stSize.WebThick / 2 ) / Math.Sin( rad ) ) + ( 1 / Math.Sin( rad ) ) ;
              moveVecN2 = vec * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( mDist ) ;
            }
            else {
              double mDist = ( ( thick / 2 ) * Math.Tan( Math.PI - ( rad + Math.PI / 2 ) ) ) +
                             ( ( stSize.WebThick / 2 ) / Math.Sin( rad ) ) + ( 1 / Math.Sin( rad ) ) +
                             ( ( thick / 2 ) / Math.Tan( rad ) ) ;
              moveVec1 = vec * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( mDist ) ;
              mDist = ( ( stSize.WebThick / 2 ) / Math.Sin( rad ) ) + ( 1 / Math.Sin( rad ) ) ;
              moveVevN1 = vec.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( mDist ) ;

              mDist = ( ( stSize2.WebThick / 2 ) / Math.Sin( rad ) ) + ( 1 / Math.Sin( rad ) ) ;
              moveVec2 = vec.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( mDist ) ;
              mDist = ( ( thick / 2 ) * Math.Tan( Math.PI - ( rad + Math.PI / 2 ) ) ) +
                      ( ( stSize2.WebThick / 2 ) / Math.Sin( rad ) ) + ( 1 / Math.Sin( rad ) ) +
                      ( ( thick / 2 ) / Math.Tan( rad ) ) ;
              moveVecN2 = vec * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( mDist ) ;
            }

            //始点側
            TaikeikouPL.CreateTaikeikouPL( doc, neda1, point1 + moveVec1, vec, stfVec, stfVec, refer, kouzaiSize,
              offset ) ;
            if ( hasEndStif && ( neda1.m_H == 0 || neda1.m_H == maxHInd ) ) {
              TaikeikouPL.CreateTaikeikouPL( doc, neda1, point1 + moveVevN1, vec.Negate(), stfVec.Negate(), stfVec,
                refer, kouzaiSize, offset, true ) ;
            }

            //終点側
            TaikeikouPL.CreateTaikeikouPL( doc, neda2, point2 + moveVec2, vec.Negate(), stfVec.Negate(), stfVec, refer,
              kouzaiSize, offset ) ;
            if ( hasEndStif && ( neda2.m_H == 0 || neda2.m_H == maxHInd ) ) {
              TaikeikouPL.CreateTaikeikouPL( doc, neda2, point2 + moveVecN2, vec, stfVec, stfVec, refer, kouzaiSize,
                offset, true ) ;
            }
          }
          else {
            return $"対傾構配置に失敗しました:{sym.FamilyName}" ;
          }
        }

        retSt = "完了" ;
      }
      catch ( Exception ex ) {
        retSt = $"対傾構配置に失敗しました{ex.Message}" ;
      }

      return retSt ;
    }

    /// <summary>
    /// Curve上にpointを投影する
    /// </summary>
    /// <param name="point"></param>
    /// <param name="curve"></param>
    /// <returns></returns>
    private static XYZ ProjectPointToCurve( XYZ point, Curve curve )
    {
      try {
        double parameter = curve.Project( point ).Parameter ;
        XYZ projectedPoint = curve.Evaluate( parameter, false ) ;
        return projectedPoint ;
      }
      catch ( Exception ex ) {
        return null ;
      }
    }
  }

  [MaterialCategory( "対傾構取付ﾌﾟﾚｰﾄ" )]
  public sealed class TaikeikouPL : MaterialSuper
  {
    public static new string typeName = "対傾構取付PL" ;

    public TaikeikouPL() : base()
    {
    }

    public TaikeikouPL( ElementId id, string koudaname, string size, string material ) : base( id, koudaname, size,
      material )
    {
    }

    /// <summary>
    /// 対傾構取付PL作成
    /// </summary>
    /// <returns></returns>
    public static bool CreateTaikeikouPL( Document doc, Neda neda, XYZ point, XYZ vec, XYZ normal, XYZ adjustVec,
      Reference refer, MaterialSize taikeikouSize, double offset, bool isEnd = false )
    {
      bool retBool = true ;
      try {
        //始点側のPL
        string kouzaiSize = neda.m_Size ;
        MaterialSize mtSize = neda.MaterialSize() ;
        if ( mtSize.Shape == MaterialShape.HA || mtSize.Shape == MaterialShape.SMH ) {
          kouzaiSize = Master.ClsKouzaiSpecify.GetKouzaiSize( kouzaiSize ) ;
        }

        string familyPath = Master.ClsTaikeikouPLCsv.GetFamilyPath( kouzaiSize ) ;
        string type = Master.ClsTaikeikouPLCsv.GetFamilyTypeName( kouzaiSize ) ;

        if ( isEnd ) {
          familyPath = Master.ClsTaikeikouPLCsv.GetStiffnerFamilyPath( kouzaiSize ) ;
          type = Master.ClsTaikeikouPLCsv.GetStiffnerFamilyTypeName( kouzaiSize ) ;
        }

        FamilySymbol sym ;
        if ( ! GantryUtil.GetFamilySymbol( doc, familyPath, type, out sym, true ) ) {
          return false ;
        }

        if ( ! sym.IsActive ) {
          sym.Activate() ;
        }

        SteelSize st = neda.SteelSize ;
        double thick = Master.ClsTaikeikouPLCsv.GetThick( kouzaiSize ) ;
        if ( thick == 0 ) {
          thick = 12 ;
        }

        XYZ setP = point + adjustVec * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( thick / 2 ) ;
        FamilyInstance ins = doc.Create.NewFamilyInstance( refer, setP, normal, sym ) ;
        if ( ins == null ) {
          return false ;
        }

        var elem = doc.GetElement( refer ) ;
        if ( elem is Level ) {
          RevitUtil.ClsRevitUtil.SetParameter( doc, ins.Id, DefineUtil.PARAM_BASE_OFFSET,
            RevitUtil.ClsRevitUtil.CovertToAPI( offset ) ) ;
        }
        else {
          RevitUtil.ClsRevitUtil.SetParameter( doc, ins.Id, DefineUtil.PARAM_HOST_OFFSET,
            RevitUtil.ClsRevitUtil.CovertToAPI( offset ) ) ;
        }
        //ElementTransformUtils.RotateElement(doc, elem.Id, Line.CreateBound(setP,setP+XYZ.BasisZ),0);

        var data = new TaikeikouPL
        {
          m_Document = doc, m_ElementId = ins.Id, m_Size = sym.FamilyName, m_KodaiName = neda.m_KodaiName,
        } ;
        TaikeikouPL.WriteToElement( data, doc ) ;
      }
      catch ( Exception ex ) {
        retBool = false ;
      }

      return retBool ;
    }
  }
}