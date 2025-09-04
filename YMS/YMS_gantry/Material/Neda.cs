using Autodesk.Revit.DB ;
using System ;
using System.Collections.Generic ;
using System.IO ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using static YMS_gantry.DefineUtil ;

namespace YMS_gantry
{
  [MaterialCategory( "根太" )]
  public sealed class Neda : MaterialSuper
  {
    public static new string Name = "Neda" ;
    public static new string typeName = "根太" ;

    [MaterialProperty( "H" )]
    public int m_H { get ; set ; }

    [MaterialProperty( "MinK" )]
    public int m_MinK { get ; set ; }

    [MaterialProperty( "MaxK" )]
    public int m_MaxK { get ; set ; }

    [MaterialProperty( "始点側突出" )]
    public double m_ExStartLen { get ; set ; }

    [MaterialProperty( "終点側突出" )]
    public double m_ExEndLen { get ; set ; }

    public Neda() : base()
    {
    }

    public Neda( ElementId id, string koudaname, NedaData data, int mink = int.MinValue, int maxk = int.MinValue,
      int h = int.MinValue ) : base( id, koudaname, data.NedaMaterial, data.NedaSize )
    {
      this.m_H = h ;
      this.m_MinK = mink ;
      this.m_MaxK = maxk ;
      this.m_ExEndLen = data.exNedaEndLeng ;
      this.m_ExStartLen = data.exNedaStartLeng ;
    }

    /// <summary>
    /// 配置位置算出
    /// </summary>
    /// <returns></returns>
    public static List<XYZ> CalcArrangementPoint( double LengKyoutyou, double LenHukuin, List<double> NedaPitch,
      XYZ basePnt, XYZ vecKyoutyou, XYZ vecHukuin )
    {
      List<XYZ> retList = new List<XYZ>() ;
      XYZ baseP = basePnt ;
      XYZ newP = baseP ;
      for ( int hC = 0 ; hC < NedaPitch.Count ; hC++ ) {
        newP = newP + vecHukuin.Normalize() * ( RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( NedaPitch[ hC ] ) ) ;
        retList.Add( newP ) ;
      }

      return retList ;
    }

    /// <summary>
    /// 根太配置
    /// </summary>
    /// <returns></returns>
    public static ElementId CreateNeda( Document doc, NedaData data, XYZ start, XYZ end, Level level, bool isDoboku,
      string koudaName, double rotate = 0, double leveloffset = 0, FamilySymbol oriSym = null )
    {
      ElementId retId = ElementId.InvalidElementId ;
      //シンボル取得
      FamilySymbol sym = oriSym ;
      if ( oriSym == null ) {
        string familyPath = Master.ClsMasterCsv.GetFamilyPath( data.NedaSize ) ;
        string type = /*isDoboku ? "主桁" :*/ typeName ;
        if ( ! GantryUtil.GetFamilySymbol( doc, familyPath, type, out sym, true ) ) {
          return ElementId.InvalidElementId ;
        }

        sym = GantryUtil.DuplicateTypeWithNameRule( doc, koudaName, sym, type ) ;
      }

      //パラメータの組み合わせ作成
      Dictionary<string, string> paramList = new Dictionary<string, string>() ;
      paramList.Add( DefineUtil.PARAM_MATERIAL, data.NedaMaterial ) ;
      XYZ vec = ( end - start ).Normalize() ;
      start = start - vec * ( RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( data.exNedaStartLeng ) ) ;
      end = end + vec * ( RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( data.exNedaEndLeng ) ) ;
      double length = ( end - start ).GetLength() ;
      paramList.Add( DefineUtil.PARAM_LENGTH, $"{length}" ) ;

      using ( SubTransaction tr = new SubTransaction( doc ) ) {
        tr.Start() ;
        ElementId CreatedID = MaterialSuper.PlaceWithTwoPoints( sym, level.GetPlaneReference(), start, end,
          RevitUtil.ClsRevitUtil.CovertToAPI( leveloffset ) ) ;
        //sym = GantryUtil.ChangeInstanceTypeID(doc, sym, CreatedID, GantryUtil.CreateTypeName(typeName, paramList));
        FamilyInstance ins = doc.GetElement( CreatedID ) as FamilyInstance ;

        //タイプパラメータ設定
        foreach ( KeyValuePair<string, string> kv in paramList ) {
          GantryUtil.SetParameterValueByParameterName( doc.GetElement( CreatedID ) as FamilyInstance, kv.Key,
            kv.Value ) ;
        }

        //エンドプレート有無
        RevitUtil.ClsRevitUtil.SetTypeParameter( sym, DefineUtil.PARAM_END_PLATE, (double) DefineUtil.PramYesNo.No ) ;

        RevitUtil.ClsRevitUtil.ModifyLevel( doc, CreatedID, level.Id ) ;

        retId = CreatedID ;
        tr.Commit() ;
      }

      return retId ;
    }

    public static double CalcLevelOffsetLevel( Level baseLevel, eBaseLevel bLevel, bool hasFukkou, double offset,
      double OhbikiH, double NedaH, int ohbikiC, bool isHOhbiki )
    {
      double retOffset = 0 ;
      if ( isHOhbiki ) {
        if ( bLevel == eBaseLevel.FukkouTop ) {
          retOffset = offset - ( NedaH / 2 ) - ( hasFukkou ? DefineUtil.FukkouBAN_THICK : 0 ) ;
        }
        else {
          retOffset = offset + OhbikiH + ( NedaH / 2 ) ;
        }
      }
      else {
        if ( bLevel == eBaseLevel.FukkouTop ) {
          retOffset = offset - ( NedaH / 2 ) - ( hasFukkou ? DefineUtil.FukkouBAN_THICK : 0 ) ;
        }
        else {
          retOffset = offset + ( OhbikiH * ohbikiC ) + ( NedaH / 2 ) ;
        }
      }

      return retOffset ;
    }
  }

  [Serializable]
  [System.Xml.Serialization.XmlRoot( "nedaData" )]
  public class NedaData
  {
    /// <summary>
    /// 材質
    /// </summary>
    [System.Xml.Serialization.XmlElement( "NedaMaterial" )]
    public string NedaMaterial { get ; set ; }

    /// <summary>                                           
    /// 根太タイプ                                          
    /// </summary>
    [System.Xml.Serialization.XmlElement( "NedaType" )]
    public string NedaType { get ; set ; }

    /// <summary>
    /// 根太サイズ
    /// </summary>
    [System.Xml.Serialization.XmlElement( "NedaSize" )]
    public string NedaSize { get ; set ; }

    /// <summary>
    /// 基点側突出量
    /// </summary>
    [System.Xml.Serialization.XmlElement( "exNedaStartLeng" )]
    public double exNedaStartLeng { get ; set ; }

    /// <summary>
    /// 終点側突出量
    /// </summary>
    [System.Xml.Serialization.XmlElement( "exNedaEndLeng" )]
    public double exNedaEndLeng { get ; set ; }

    public NedaData()
    {
      NedaMaterial = "SS400" ;
      NedaType = "" ;
      NedaSize = "" ;
      exNedaStartLeng = 0 ;
      exNedaEndLeng = 0 ;
    }
  }
}