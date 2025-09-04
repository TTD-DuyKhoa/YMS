using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;

#region Namespaces

using System.Diagnostics ;
using System.Reflection ;
using WinForms = System.Windows.Forms ;
using Autodesk.Revit.ApplicationServices ;
using Autodesk.Revit.Attributes ;
using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using Autodesk.Revit.UI.Selection ;
using Autodesk.Revit.Creation ;
using System.Collections ;
using System.IO ;
using Autodesk.Revit.DB.Events ;
using YMS_gantry.UI ;
using static YMS_gantry.DefineUtil ;
using YMS_gantry.Data ;
using RevitUtil ;
using YMS_gantry.Material ;
using Autodesk.Revit.DB.Structure ;
using Document = Autodesk.Revit.DB.Document ;

//using System.Xml.Linq;

#endregion // Namespaces

namespace YMS_gantry
{
  public class GantryUtil
  {
    static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger() ;

    /// <summary>
    /// タイプ名作成
    /// </summary>
    /// <param name="name"></param>
    /// <param name="paraList"></param>
    /// <returns></returns>
    public static string CreateTypeName( string name, Dictionary<string, string> paraList )
    {
      string typeName = name ;
      foreach ( KeyValuePair<string, string> kv in paraList ) {
        typeName += $"{kv.Key}X{kv.Value}" ;
        if ( kv.Key != paraList.ElementAt( paraList.Count - 1 ).Key ) {
          typeName += "_" ;
        }
      }

      typeName = typeName.Replace( "<", "" ) ;
      typeName = typeName.Replace( ">", "" ) ;

      return typeName ;
    }

    /// <summary>
    /// 取付タイプのタイプ名を生成
    /// </summary>
    /// <param name="eJoinType"></param>
    /// <param name="familyName"></param>
    /// <returns></returns>
    public static string GetJointTypeName( DefineUtil.eJoinType eJoinType, string familyName )
    {
      string retSt = "" ;
      int ub = familyName.IndexOf( '_' ) ;
      retSt = familyName ;
      if ( ub != -1 ) {
        retSt = familyName.Remove( 0, ub + 1 ) ;
      }

      if ( retSt.Contains( "-" ) ) {
        retSt = retSt.Replace( "-", "" ) ;
      }

      retSt = retSt.Replace( "X", "x" ) ;
      if ( eJoinType == DefineUtil.eJoinType.Bolt ) {
        retSt += "ﾎﾞﾙﾄ継手" ;
      }
      else if ( eJoinType == DefineUtil.eJoinType.Welding ) {
        retSt += "溶接継手" ;
      }

      return retSt ;
    }

    /// <summary>
    /// 取付タイプのタイプ名を生成
    /// </summary>
    /// <param name="eJoinType"></param>
    /// <param name="familyName"></param>
    /// <returns></returns>
    public static string GetJointTypeName( DefineUtil.eJoinType eJoinType, string familyName, bool isSichu = false )
    {
      string retSt = "" ;
      int ub = familyName.IndexOf( '_' ) ;
      retSt = familyName ;
      if ( ub != -1 ) {
        retSt = familyName.Remove( 0, ub + 1 ) ;
      }

      if ( retSt.Contains( "-" ) ) {
        retSt = retSt.Replace( "-", "" ) ;
      }

      retSt = retSt.Replace( "X", "x" ) ;
      if ( isSichu ) {
        string[] p = retSt.Split( 'x' ) ;
        if ( p.Length >= 2 ) {
          retSt = $"{p[ 0 ]}x{p[ 1 ]}" ;
        }
      }

      if ( eJoinType == DefineUtil.eJoinType.Bolt ) {
        retSt += "ﾎﾞﾙﾄ継手" ;
      }
      else if ( eJoinType == DefineUtil.eJoinType.Welding ) {
        retSt += "溶接継手" ;
      }

      return retSt ;
    }

    /// <summary>
    /// すべてのレベル名を取得
    /// </summary>
    /// <param name="doc"></param>
    /// <returns></returns>
    public static List<string> GetAllLevelName( Document doc )
    {
      List<string> nameList = new List<string>() ;
      FilteredElementCollector collector = new FilteredElementCollector( doc ) ;
      ICollection<Element> collection = collector.OfClass( typeof( Level ) ).ToElements() ;

      foreach ( Element elem in collection ) {
        nameList.Add( elem.Name ) ;
      }

      return nameList ;
    }

    /// <summary>
    /// 鋼材の縦横厚みを取得する
    /// </summary>
    /// <param name="kouzaiName"></param>
    /// <returns></returns>
    public static MaterialSize GetKouzaiSize( string kouzaiName )
    {
      try {
        string retSt = "" ;
        //頭に[部材種類]_～がついている場合は除く
        int ub = kouzaiName.IndexOf( '_' ) ;
        retSt = kouzaiName ;
        if ( ub != -1 ) {
          retSt = kouzaiName.Remove( 0, ub + 1 ) ;
        }

        if ( retSt.Contains( "-" ) ) {
          retSt = retSt.Replace( "-", "" ) ;
        }

        var stringList = GetKouzaiSizeSunpouList( retSt ) ;

        //鋼材毎に判定分ける
        //桁材関連(不等辺はないものと想定）
        if ( retSt.StartsWith( "H" ) ) {
          return new MaterialSize
          {
            Shape = MaterialShape.H,
            Height = Atof( stringList.ElementAtOrDefault( 1 ) ?? "" ),
            Width = Atof( stringList.ElementAtOrDefault( 2 ) ?? "" ),
            Thick = Atof( stringList.ElementAtOrDefault( 3 ) ?? "" )
          } ;
        }
        else if ( retSt.StartsWith( "C" ) || retSt.StartsWith( "[" ) ) {
          return new MaterialSize
          {
            Shape = MaterialShape.C,
            Height = Atof( stringList.ElementAtOrDefault( 1 ) ?? "" ),
            Width = Atof( stringList.ElementAtOrDefault( 2 ) ?? "" ),
            Thick = Atof( stringList.ElementAtOrDefault( 3 ) ?? "" )
          } ;
        }
        else if ( retSt.StartsWith( "L" ) ) {
          return new MaterialSize
          {
            Shape = MaterialShape.L,
            Height = Atof( stringList.ElementAtOrDefault( 1 ) ?? "" ),
            Width = Atof( stringList.ElementAtOrDefault( 2 ) ?? "" ),
            Thick = Atof( stringList.ElementAtOrDefault( 3 ) ?? "" )
          } ;
        }
        //PL
        else if ( retSt.StartsWith( "PL" ) ) {
          return new MaterialSize
          {
            Shape = MaterialShape.PL,
            Height = Atof( stringList.ElementAtOrDefault( 2 ) ?? "" ),
            Width = Atof( stringList.ElementAtOrDefault( 3 ) ?? "" ),
            Thick = Atof( stringList.ElementAtOrDefault( 1 ) ?? "" )
          } ;
        }
        //山留材
        else if ( retSt.Contains( "HA" ) ) {
          int ind = retSt.IndexOf( "HA" ) ;
          string type = retSt.Substring( ind, retSt.Length - ind ) ;
          double size = Atof( retSt.Replace( type, "" ) ) ;
          return new MaterialSize { Shape = MaterialShape.HA, Height = size * 10, Width = size * 10, Thick = 0 } ;
        }
        //高強度山留材
        else if ( retSt.Contains( "SMH" ) ) {
          int ind = retSt.IndexOf( "SMH" ) ;
          string type = retSt.Substring( ind, retSt.Length - ind ) ;
          double size = Atof( retSt.Replace( type, "" ) ) ;
          return new MaterialSize { Shape = MaterialShape.SMH, Height = size * 10, Width = size * 10, Thick = 0 } ;
        }
        //覆工板
        else if ( retSt.StartsWith( "MD" ) ) {
          return new MaterialSize
          {
            Shape = MaterialShape.MD,
            Height = 1000,
            Width = retSt.EndsWith( "2000" ) ? 2000 : 3000,
            Thick = DefineUtil.FukkouBAN_THICK
          } ;
        }
        else {
          return null ;
        }
      }
      catch ( Exception ) {
        return null ;
      }
    }

    /// <summary>
    /// 形鋼_HorL〇X〇X〇X〇から指定の寸法値を取得する
    /// </summary>
    /// <param name="kouzaiName">鋼材名</param>
    /// <param name="num">0:形鋼の種類,1:高さ,2:ﾌﾗﾝｼﾞ幅,3:腹板厚,4:ﾌﾗﾝｼﾞ厚</param>
    /// <returns>寸法値</returns>
    public static List<string> GetKouzaiSizeSunpouList( string kouzaiName )
    {
      List<string> nameDataList = new List<string>() ;
      string retSt = "" ;
      int ub = kouzaiName.IndexOf( '_' ) ;
      retSt = kouzaiName ;
      if ( ub != -1 ) {
        retSt = kouzaiName.Remove( 0, ub + 1 ) ;
      }

      if ( retSt.StartsWith( "-" ) ) {
        retSt = kouzaiName.Remove( 0, 1 ) ;
      }

      if ( retSt.StartsWith( "H" ) || retSt.StartsWith( "L" ) || retSt.StartsWith( "C" ) || retSt.StartsWith( "[" ) ) {
        nameDataList.Add( retSt.Substring( 0, 1 ) ) ;
        retSt = retSt.Remove( 0, 1 ) ;
      }
      else if ( retSt.StartsWith( "PL" ) ) {
        nameDataList.Add( retSt.Substring( 0, 2 ) ) ;
        retSt = retSt.Remove( 0, 2 ) ;
      }


      if ( retSt.Contains( "X" ) ) {
        retSt = retSt.Replace( "X", "x" ) ;
      }

      string[] sunpou = retSt.Split( 'x' ) ;
      foreach ( string snp in sunpou ) {
        nameDataList.Add( snp ) ;
      }

      return nameDataList ;
    }

    public static SteelSize GetKouzaiSizeSunpou( string kouzaiName )
    {
      try {
        string retSt = "" ;
        //頭に[部材種類]_～がついている場合は除く
        int ub = kouzaiName.IndexOf( '_' ) ;
        retSt = kouzaiName ;
        if ( ub != -1 ) {
          retSt = kouzaiName.Remove( 0, ub + 1 ) ;
        }

        if ( retSt.Contains( "-" ) ) {
          retSt = retSt.Replace( "-", "" ) ;
        }

        var stringList = GetKouzaiSizeSunpouList( retSt ) ;

        //鋼材毎に判定分ける
        //桁材関連(不等辺はないものと想定）
        if ( retSt.StartsWith( "H" ) ) {
          return new SteelSize
          {
            Shape = SteelShape.H,
            Height = Atof( stringList.ElementAtOrDefault( 1 ) ?? "" ),
            FrangeWidth = Atof( stringList.ElementAtOrDefault( 2 ) ?? "" ),
            WebThick = Atof( stringList.ElementAtOrDefault( 3 ) ?? "" ),
            FrangeThick = Atof( stringList.ElementAtOrDefault( 4 ) ?? "" )
          } ;
        }
        else if ( retSt.StartsWith( "C" ) || retSt.StartsWith( "[" ) ) {
          return new SteelSize
          {
            Shape = SteelShape.C,
            Height = Atof( stringList.ElementAtOrDefault( 1 ) ?? "" ),
            FrangeWidth = Atof( stringList.ElementAtOrDefault( 2 ) ?? "" ),
            WebThick = Atof( stringList.ElementAtOrDefault( 3 ) ?? "" ),
            FrangeThick = Atof( stringList.ElementAtOrDefault( 4 ) ?? "" )
          } ;
        }
        else if ( retSt.StartsWith( "L" ) ) {
          return new SteelSize
          {
            Shape = SteelShape.L,
            Height = Atof( stringList.ElementAtOrDefault( 1 ) ?? "" ),
            FrangeWidth = Atof( stringList.ElementAtOrDefault( 2 ) ?? "" ),
            WebThick = Atof( stringList.ElementAtOrDefault( 3 ) ?? "" ),
            FrangeThick = Atof( stringList.ElementAtOrDefault( 4 ) ?? "" )
          } ;
        }
        //PL
        else if ( retSt.StartsWith( "PL" ) ) {
          return new SteelSize
          {
            Shape = SteelShape.PL,
            Height = Atof( stringList.ElementAtOrDefault( 2 ) ?? "" ),
            FrangeWidth = Atof( stringList.ElementAtOrDefault( 3 ) ?? "" ),
            WebThick = Atof( stringList.ElementAtOrDefault( 1 ) ?? "" ),
            FrangeThick = 0
          } ;
        }
        //山留材
        else if ( retSt.Contains( "HA" ) ) {
          var kouzaiSize = Master.ClsKouzaiSpecify.GetKouzaiSize( retSt ) ;
          stringList = GetKouzaiSizeSunpouList( kouzaiSize ) ;

          return new SteelSize
          {
            Shape = SteelShape.HA,
            Height = Atof( stringList.ElementAtOrDefault( 1 ) ?? "" ),
            FrangeWidth = Atof( stringList.ElementAtOrDefault( 2 ) ?? "" ),
            WebThick = Atof( stringList.ElementAtOrDefault( 3 ) ?? "" ),
            FrangeThick = Atof( stringList.ElementAtOrDefault( 4 ) ?? "" )
          } ;
        }
        //高強度山留材
        else if ( retSt.Contains( "SMH" ) ) {
          var kouzaiSize = Master.ClsKouzaiSpecify.GetKouzaiSize( retSt ) ;
          stringList = GetKouzaiSizeSunpouList( kouzaiSize ) ;

          return new SteelSize
          {
            Shape = SteelShape.SMH,
            Height = Atof( stringList.ElementAtOrDefault( 1 ) ?? "" ),
            FrangeWidth = Atof( stringList.ElementAtOrDefault( 2 ) ?? "" ),
            WebThick = Atof( stringList.ElementAtOrDefault( 3 ) ?? "" ),
            FrangeThick = Atof( stringList.ElementAtOrDefault( 4 ) ?? "" )
          } ;
        }
        //覆工板
        else if ( retSt.StartsWith( "MD" ) ) {
          return new SteelSize
          {
            Shape = SteelShape.MD,
            Height = 1000,
            FrangeWidth = retSt.EndsWith( "2000" ) ? 2000 : 3000,
            WebThick = DefineUtil.FukkouBAN_THICK,
            FrangeThick = 0
          } ;
        }
        else {
          return new SteelSize() ;
          ;
        }
      }
      catch ( Exception ) {
        return new SteelSize() ;
        ;
      }
    }

    private static double Atof( string numString )
    {
      if ( ! double.TryParse( numString, out double value ) ) {
        return 0.0 ;
      }

      return value ;
    }

    /// <summary>
    /// パラメータ名に合わせた型に変換してファミリインスタンスのシンボルパラメータ設定
    /// </summary>
    /// <param name="familyInstance"></param>
    /// <param name="parameterName"></param>
    /// <param name="parameterValue"></param>
    public static void SetParameterValueByParameterName( FamilyInstance familyInstance, string parameterName,
      string parameterValue )
    {
      FamilySymbol familySymbol = familyInstance.Symbol ;
      Family family = familySymbol.Family ;

      Parameter parm = familySymbol.LookupParameter( parameterName ) ;
      if ( parm == null ) return ;

      // パラメータの値を設定
      switch ( parm.StorageType ) {
        case StorageType.Integer :
          int intValue ;
          if ( int.TryParse( parameterValue, out intValue ) ) {
            parm.Set( intValue ) ;
          }

          break ;

        case StorageType.Double :
          double doubleValue ;
          if ( double.TryParse( parameterValue, out doubleValue ) ) {
            // Revitバージョンの取得
            #if BUILD_REVIT2022
                        doubleValue =
 (parm.Definition.ParameterType == ParameterType.Angle) ? doubleValue : RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(doubleValue);
            #else
            doubleValue = ( parm.Definition.GetDataType() == SpecTypeId.Angle )
              ? doubleValue
              : RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( doubleValue ) ;
            #endif
            //int version = RevitVersionHelper.GetRevitVersion();

            //if (version <= 2022)
            //{
            //    doubleValue = (parm.Definition.ParameterType == ParameterType.Angle) ? doubleValue : RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(doubleValue);
            //}
            //else
            //{
            //     doubleValue = (parm.Definition.GetDataType() == SpecTypeId.Angle) ? doubleValue : RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(doubleValue);
            //}
            parm.Set( doubleValue ) ;
          }

          break ;

        case StorageType.String :
          parm.Set( parameterValue ) ;
          break ;
        case StorageType.ElementId :
          int id = int.MinValue ;
          if ( int.TryParse( parameterValue, out id ) ) {
            parm.Set( new ElementId( id ) ) ;
          }

          break ;
      }
    }

    /// <summary>
    /// パラメータ名に合わせた型に変換してシンボルのパラメータ設定
    /// </summary>
    /// <param name="familySymbol"></param>
    /// <param name="parameterName"></param>
    /// <param name="parameterValue"></param>
    public static void SetSymbolParameterValueByParameterName( FamilySymbol familySymbol, string parameterName,
      string parameterValue )
    {
      Parameter parm = familySymbol.LookupParameter( parameterName ) ;
      if ( parm == null ) return ;

      // パラメータの値を設定
      switch ( parm.StorageType ) {
        case StorageType.Integer :
          int intValue ;
          if ( int.TryParse( parameterValue, out intValue ) ) {
            parm.Set( intValue ) ;
          }

          break ;

        case StorageType.Double :
          double doubleValue ;
          if ( double.TryParse( parameterValue, out doubleValue ) ) {
            #if BUILD_REVIT2022
                        doubleValue =
 (parm.Definition.ParameterType == ParameterType.Angle) ? doubleValue : RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(doubleValue);
            #else
            doubleValue = ( parm.Definition.GetDataType() == SpecTypeId.Angle )
              ? doubleValue
              : RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( doubleValue ) ;
            #endif
            //int version = RevitVersionHelper.GetRevitVersion();

            //if (version <= 2022)
            //{
            //    doubleValue = (parm.Definition.ParameterType == ParameterType.Angle) ? doubleValue : RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(doubleValue);
            //}
            //else
            //{
            //     doubleValue = (parm.Definition.GetDataType() == SpecTypeId.Angle) ? doubleValue : RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(doubleValue);
            //}
            parm.Set( doubleValue ) ;
          }

          break ;

        case StorageType.String :
          parm.Set( parameterValue ) ;
          break ;
        case StorageType.ElementId :
          int id = int.MinValue ;
          if ( int.TryParse( parameterValue, out id ) ) {
            parm.Set( new ElementId( id ) ) ;
          }

          break ;
      }
    }

    /// <summary>
    /// 1点指定のファミリを配置する
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="point"></param>
    /// <param name="levelId"></param>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static FamilyInstance CreateInstanceWith1point( Document doc, XYZ point, Reference refer,
      FamilySymbol symbol, XYZ normal = null, bool noHost = false, bool followWithHostType = true )
    {
      if ( symbol == null || ! noHost && refer == null ) {
        return null ;
      }

      if ( ! symbol.IsActive ) {
        symbol.Activate() ;
      }

      XYZ norm = ( normal == null ) ? new XYZ() : normal ;
      FamilyInstance newInstance = null ;
      try {
        if ( noHost ) {
          newInstance = doc.Create.NewFamilyInstance( point, symbol, StructuralType.NonStructural ) ;
        }
        else {
          var element = doc.GetElement( refer ) ;
          if ( followWithHostType && element is Level level ) {
            newInstance = doc.Create.NewFamilyInstance( point, symbol, level, StructuralType.NonStructural ) ;
          }
          else if ( followWithHostType && element is ReferencePlane plane ) {
            newInstance = doc.Create.NewFamilyInstance( point, symbol, (Level) doc.GetElement( element.LevelId ),
              StructuralType.NonStructural ) ;
          }
          else {
            newInstance = doc.Create.NewFamilyInstance( refer, point, norm, symbol ) ;
          }
        }
      }
      catch ( Exception ex ) {
        _logger.Error( ex.StackTrace ) ;
        return null ;
      }

      return newInstance ;
    }

    /// <summary>
    /// インスタンスのホストを取得　ホストでなくても作業面があれば作業面を取得する
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="instBase"></param>
    /// <returns></returns>
    public static ElementId GetInstanceHostId( Document doc, FamilyInstance instBase )
    {
      ElementId id = ElementId.InvalidElementId ;
      if ( instBase.Host != null ) {
        id = instBase.Host.Id ;
      }
      else {
        Parameter hostlevId = instBase.LookupParameter( "作業面" ) ;
        string levelName = hostlevId.AsValueString() ;
        if ( hostlevId == null || hostlevId.Element == null ) {
          id = ElementId.InvalidElementId ;
        }
        else if ( hostlevId.Element.GetType() == typeof( FamilyInstance ) || hostlevId.GetType() == typeof( Level ) ) {
          id = hostlevId.Element.Id ;
        }
        else {
          hostlevId = instBase.LookupParameter( "集計レベル" ) ;
          levelName = hostlevId.AsValueString() ;
          if ( hostlevId == null || hostlevId.Element == null ) {
            id = ElementId.InvalidElementId ;
          }
          else if ( hostlevId.Element.GetType() == typeof( FamilyInstance ) ||
                    hostlevId.GetType() == typeof( Level ) ) {
            id = hostlevId.Element.Id ;
          }
        }
      }

      return id ;
    }


    /// <summary>
    /// 同時に複数のファミリを配置する
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="dataList"></param>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static List<ElementId> CreateInstancesWith1point( Document doc,
      List<(XYZ point, ElementId levelId)> dataList, FamilySymbol symbol )
    {
      if ( symbol == null ) {
        return null ;
      }

      List<ElementId> ids = new List<ElementId>() ;

      if ( ! symbol.IsActive ) {
        symbol.Activate() ;
      }

      try {
        List<FamilyInstanceCreationData> creatinDataList = new List<FamilyInstanceCreationData>() ;
        foreach ( ( XYZ p, ElementId levelId ) in dataList ) {
          Level lv = doc.GetElement( levelId ) as Level ;
          Reference refe = lv.GetPlaneReference() ;
          FamilyInstanceCreationData data =
            new FamilyInstanceCreationData( p, symbol, lv, StructuralType.NonStructural ) ;
          creatinDataList.Add( data ) ;
        }

        if ( creatinDataList.Count < 1 ) {
          return ids ;
        }

        ids = doc.Create.NewFamilyInstances2( creatinDataList ).ToList() ;
      }
      catch {
        return new List<ElementId>() ;
      }

      return ids ;
    }

    /// <summary>
    /// パラメータの型に併せて自動でセット
    /// </summary>
    /// <param name="familySymbol"></param>
    /// <param name="parameterName"></param>
    /// <param name="parameterValue"></param>
    public static void SetParameterValueByParameterName( FamilySymbol familySymbol, string parameterName,
      string parameterValue )
    {
      Family family = familySymbol.Family ;

      Parameter parm = familySymbol.LookupParameter( parameterName ) ;
      if ( parm == null ) return ;

      // パラメータの値を設定
      switch ( parm.StorageType ) {
        case StorageType.Integer :
          int intValue ;
          if ( int.TryParse( parameterValue, out intValue ) ) {
            parm.Set( intValue ) ;
          }

          break ;

        case StorageType.Double :
          double doubleValue ;
          if ( double.TryParse( parameterValue, out doubleValue ) ) {
            #if BUILD_REVIT2022
                        doubleValue =
 (parm.Definition.ParameterType == ParameterType.Angle) ? doubleValue : RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(doubleValue);
            #else
            doubleValue = ( parm.Definition.GetDataType() == SpecTypeId.Angle )
              ? doubleValue
              : RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( doubleValue ) ;
            #endif
            //int version = RevitVersionHelper.GetRevitVersion();

            //if (version <= 2022)
            //{
            //    doubleValue = (parm.Definition.ParameterType == ParameterType.Angle) ? doubleValue : RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(doubleValue);
            //}
            //else
            //{
            //     doubleValue = (parm.Definition.GetDataType() == SpecTypeId.Angle) ? doubleValue : RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(doubleValue);
            //}
            parm.Set( doubleValue ) ;
          }

          break ;

        case StorageType.String :
          parm.Set( parameterValue ) ;
          break ;
        case StorageType.ElementId :
          int id = int.MinValue ;
          if ( int.TryParse( parameterValue, out id ) ) {
            parm.Set( new ElementId( id ) ) ;
          }

          break ;
      }
    }

    /// <summary>
    /// 指定したファミリシンボルを図面にロードする
    /// </summary>
    /// <param name="doc">ドキュメント</param>
    /// <param name="rfaFilePath">ファミリのパス</param>
    /// <param name="typeName">タイプ名</param>
    /// <param name="name">ファミリネーム</param>
    /// <param name="sym">ファミリシンボル</param>
    /// <param name="isSubTr">サブトランザクションでロードするか</param>
    /// <returns>true:成功,false:失敗</returns>
    public static bool GetFamilySymbol( Document doc, string rfaFilePath, string typeName, out FamilySymbol sym,
      bool isSubTr = false, string name = "" )
    {
      sym = null ;
      string familyName = ( name == "" ) ? Path.GetFileNameWithoutExtension( rfaFilePath ) : name ;

      // 既に読み込まれているかどうかをチェックする
      sym = GetFamilySymbol( doc, typeName, familyName, isSubTr ) ;
      if ( sym != null ) {
        return true ;
      }

      Family loadedFamily = null ;
      try {
        // まだ読み込まれていない場合は読み込む
        LoadOpts lp = new LoadOpts() ;
        if ( ! isSubTr ) {
          using ( Transaction tx = new Transaction( doc, "Load Family" ) ) {
            tx.Start() ;
            //doc.LoadFamilySymbol(rfaFilePath, typeName, lp,  out sym);
            doc.LoadFamilySymbol( rfaFilePath, typeName, out sym ) ;
            tx.Commit() ;
          }
        }
        else {
          using ( SubTransaction tx = new SubTransaction( doc ) ) {
            tx.Start() ;
            //doc.LoadFamilySymbol(rfaFilePath, typeName, lp, out sym);
            doc.LoadFamilySymbol( rfaFilePath, typeName, out sym ) ;
            tx.Commit() ;
          }
        }

        if ( sym != null ) {
          return true ;
        }

        if ( loadedFamily != null ) {
          FilteredElementCollector collector = new FilteredElementCollector( doc ) ;
          collector.OfCategory( BuiltInCategory.OST_GenericModel ) ;
          // ファミリが正常にロードされた場合、指定のタイプを探す
          foreach ( FamilySymbol fs in collector.OfClass( typeof( FamilySymbol ) ).Cast<FamilySymbol>() ) {
            if ( fs.Family.Name == familyName && fs.Name == typeName ) {
              sym = fs ;
              break ;
            }
          }
        }

        if ( sym == null ) {
          return false ;
        }

        return true ;
      }
      catch ( Exception ex ) {
        sym = null ;
        return false ;
      }
    }

    /// <summary>
    /// 既に図面にあるファミリシンボルを取得する
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="typeName"></param>
    /// <param name="familyName"></param>
    /// <param name="isSubTr"></param>
    /// <returns></returns>
    public static FamilySymbol GetFamilySymbol( Document doc, string typeName, string familyName, bool isSubTr = false )
    {
      //一般カテゴリの中から探す
      FilteredElementCollector collector = new FilteredElementCollector( doc ) ;
      collector.OfCategory( BuiltInCategory.OST_GenericModel ) ; // 必要に応じて適切なカテゴリを選択

      foreach ( FamilySymbol fs in collector.OfClass( typeof( FamilySymbol ) ).Cast<FamilySymbol>() ) {
        if ( fs == null || fs.Family == null ) continue ;
        if ( fs.Family.Name.Replace( "X", "x" ) == familyName.Replace( "X", "x" ) &&
             fs.Name.Replace( "X", "x" ) == typeName.Replace( "X", "x" ) ) {
          // 指定のファミリ名とタイプ名が存在する場合はシンボルを返す
          return fs ;
        }
      }

      return null ;
    }

    /// <summary>
    /// 既に図面にある全てのファミリシンボルを取得する
    /// </summary>
    /// <param name="doc">図面情報</param>
    /// <returns>ファミリシンボルリストを返却</returns>
    public static List<FamilySymbol> GetAllFamilySymbol( Document doc )
    {
      var resultList = new List<FamilySymbol>() ;

      FilteredElementCollector collector = new FilteredElementCollector( doc ) ;
      collector.OfCategory( BuiltInCategory.OST_GenericModel ) ;

      foreach ( FamilySymbol fs in collector.OfClass( typeof( FamilySymbol ) ).Cast<FamilySymbol>() ) {
        resultList.Add( fs ) ;
        continue ;
      }

      return resultList ;
    }

    /// <summary>
    /// 指定したファミリシンボルに該当するファミリインスタンスを取得
    /// </summary>
    /// <param name="doc">図面情報</param>
    /// <param name="fs">ファミリシンボル</param>
    /// <returns>ファミリインスタンスのリストを返却</returns>
    public static List<FamilyInstance> GetFamilyInstanceList( Document doc, FamilySymbol fs )
    {
      var resultList = new List<FamilyInstance>() ;

      FilteredElementCollector collector = new FilteredElementCollector( doc ) ;
      collector.OfCategory( BuiltInCategory.OST_GenericModel ) ;

      return collector.OfClass( typeof( FamilyInstance ) ).Select( x => x as FamilyInstance )
        .Where( x => x != null && x.Symbol.Name == fs.Name ).ToList() ;
    }

    /// <summary>
    /// 指定したエレメントIDに該当するファミリインスタンスを取得
    /// </summary>
    /// <param name="doc">図面情報</param>
    /// <param name="fs">ファミリシンボル</param>
    /// <returns>ファミリインスタンスのリストを返却</returns>
    public static FamilyInstance GetFamilyInstance( Document doc, ElementId id )
    {
      var resultList = new List<FamilyInstance>() ;

      FilteredElementCollector collector = new FilteredElementCollector( doc ) ;
      collector.OfCategory( BuiltInCategory.OST_GenericModel ) ;

      return collector.OfClass( typeof( FamilyInstance ) ).Select( x => x as FamilyInstance )
        .Where( x => x != null && x.Id.IntegerValue == id.IntegerValue ).FirstOrDefault() ;
    }

    ///// <summary>
    ///// タイプを複製する
    ///// </summary>
    ///// <param name="doc"></param>
    ///// <param name="familySymbol"></param>
    ///// <param name="targetId"></param>
    ///// <param name="strNewType"></param>
    ///// <returns></returns>
    //public static FamilySymbol ChangeInstanceTypeID(Document doc, FamilySymbol familySymbol, ElementId targetId, string strNewType)
    //{
    //    FamilySymbol elementType = ChangeTypeID(doc, familySymbol, strNewType);
    //    Element instance = doc.GetElement(targetId);
    //    instance.ChangeTypeId(elementType.Id);

    //    return (FamilySymbol)elementType;
    //}

    //public static FamilySymbol ChangeTypeID(Document doc, FamilySymbol familySymbol, string strNewType)
    //{
    //    // 複製元のタイプを取得
    //    FamilySymbol sourceType = familySymbol; // 複製元のタイプ

    //    //タイプが存在しない場合追加
    //    List<string> typeNamses = GetTypeNames(doc, sourceType);
    //    ElementType elementType = null;
    //    if (!typeNamses.Contains(strNewType))
    //    {
    //        // タイプを複製
    //        elementType = sourceType.Duplicate(strNewType);
    //    }
    //    else
    //    {
    //        // タイプを取得
    //        elementType = GetElementType(doc, familySymbol, strNewType);
    //    }

    //    return (FamilySymbol)elementType;
    //}

    /// <summary>
    /// タイプの一覧を取得する
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="familySymbol"></param>
    /// <returns></returns>
    public static List<string> GetTypeNames( Document doc, FamilySymbol familySymbol )
    {
      // 複製元のタイプを取得
      FamilySymbol sourceType = familySymbol ; // 複製元のタイプ

      //タイプの存在チェック
      List<string> typeNames = new List<string>() ;
      List<ElementId> typeIds = familySymbol.GetSimilarTypes().ToList() ;
      foreach ( ElementId id in typeIds ) {
        FamilySymbol instance = doc.GetElement( id ) as FamilySymbol ;
        if ( familySymbol.FamilyName.Equals( instance.FamilyName ) ) {
          string typeName = instance.Name ;
          typeNames.Add( typeName ) ;
        }
      }

      return typeNames ;
    }

    /// <summary>
    /// すべてのファミリタイプを取得する
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="targetType">集めたいファミリのタイプ名</param>
    /// <returns></returns>
    public static List<string> GetAllTypeName( Document doc, string targetType )
    {
      List<string> typeNames = new List<string>() ;

      // 一般モデルファミリのカテゴリを取得
      FilteredElementCollector collector = new FilteredElementCollector( doc ) ;
      ICollection<Element> elements = collector.OfCategory( BuiltInCategory.OST_GenericModel )
        .WhereElementIsElementType().ToElements() ;

      // 一般モデルファミリのタイプ名を取得
      foreach ( Element element in elements ) {
        ElementType elementType = element as ElementType ;
        if ( elementType != null ) {
          if ( elementType.Name.StartsWith( targetType ) ) {
            typeNames.Add( elementType.Name ) ;
          }
        }
      }

      return typeNames ;
    }


    /// <summary>
    /// エレメントタイプを取得
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="familySymbol"></param>
    /// <param name="strTypeName"></param>
    /// <returns></returns>
    public static ElementType GetElementType( Document doc, FamilySymbol familySymbol, string strTypeName )
    {
      // 複製元のタイプを取得
      FamilySymbol sourceType = familySymbol ; // 複製元のタイプ

      //タイプの存在チェック
      List<ElementId> typeIds = familySymbol.GetSimilarTypes().ToList() ;
      ElementType elementType = null ;
      foreach ( ElementId id in typeIds ) {
        FamilySymbol instance = doc.GetElement( id ) as FamilySymbol ;
        string typeName = instance.Name ;
        if ( familySymbol.FamilyName == instance.FamilyName && typeName == strTypeName ) {
          elementType = doc.GetElement( id ) as ElementType ;
          break ;
        }
      }

      return elementType ;
    }

    public List<ElementId> _added_element_ids = new List<ElementId>() ;

    /// <summary>
    /// 任意位置にシンボル配置
    /// </summary>
    /// <param name="symbol">配置するシンボル</param>
    /// <param name="isSketch">作業面に配置か false=面に配置</param>
    /// <returns></returns>
    public List<ElementId> PlaceFamilyInstance( UIApplication uiapp, FamilySymbol symbol, bool isSketch )
    {
      _added_element_ids = new List<ElementId>() ;
      UIDocument uidoc = uiapp.ActiveUIDocument ;
      var app = uiapp.Application ;

      _added_element_ids.Clear() ;
      PromptForFamilyInstancePlacementOptions opt = new PromptForFamilyInstancePlacementOptions() ;
      //配置されたインスタンスのIdを取得するためにイベントハンドラに登録
      app.DocumentChanged += new EventHandler<DocumentChangedEventArgs>( OnDocumentChanged ) ;
      try {
        opt.FaceBasedPlacementType =
          ( isSketch ) ? FaceBasedPlacementType.Default : FaceBasedPlacementType.PlaceOnFace ;
        uidoc.PromptForFamilyInstancePlacement( symbol, opt ) ;
      }
      catch ( Autodesk.Revit.Exceptions.OperationCanceledException ) {
        //配置終了（キャンセル）時は特にスローせず終了
      }
      catch ( Exception ex ) {
      }
      finally {
        //呼ぶ毎に追加、削除する
        app.DocumentChanged -= new EventHandler<DocumentChangedEventArgs>( OnDocumentChanged ) ;
      }

      return _added_element_ids ;
    }

    /// <summary>
    /// イベントハンドラ登録
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void OnDocumentChanged( object sender, DocumentChangedEventArgs e )
    {
      List<ElementId> ids = e.GetAddedElementIds().ToList() ;
      if ( ids.Count > 0 ) {
        _added_element_ids.AddRange( ids ) ;
        //if (mate != null)
        //{
        //    using (SubTransaction sub = new SubTransaction(mate.m_Document))
        //    {
        //        sub.Start();
        //        foreach (ElementId id in e.GetAddedElementIds())
        //        {
        //            mate.m_ElementId = id;
        //            MaterialSuper.WriteToElement(mate, mate.m_Document);
        //        }
        //        sub.Commit();
        //    }
        //}
      }
    }

    public static Result CreateRubberLine( UIDocument uidoc )
    {
      Document doc = uidoc.Document ;
      Selection sel1 = uidoc.Selection ;
      List<XYZ> tempXYZ = new List<XYZ>( 1 ) ;
      XYZ p1 = sel1.PickPoint() ;
      XYZ p2 = null ;


      //tempXYZ.Add(p3);

      ModelCurve visualLine = null ;
      using ( TransactionGroup tGroup = new TransactionGroup( doc ) ) {
        tGroup.Start() ;

        Redraw:
        using ( Transaction t = new Transaction( doc ) ) {
          t.Start( "Step 1" ) ;

          Line line = Line.CreateBound( p1, getP3( uidoc ) ) ;

          Plane geomPlane = Plane.CreateByNormalAndOrigin( doc.ActiveView.ViewDirection, doc.ActiveView.Origin ) ;
          SketchPlane sketch = SketchPlane.Create( doc, geomPlane ) ;
          visualLine = doc.Create.NewModelCurve( line, sketch ) as ModelCurve ;
          doc.Regenerate() ;
          uidoc.RefreshActiveView() ;
          goto Redraw ;

          t.Commit() ;
        }

        tGroup.Commit() ;
      }

      return Result.Succeeded ;
    }

    private static XYZ getP3( UIDocument uidoc )
    {
      UIView uiview = GetActiveUiView( uidoc ) ;
      Rectangle rect = uiview.GetWindowRectangle() ;
      System.Drawing.Point p = System.Windows.Forms.Cursor.Position ;
      System.Windows.Forms.Cursor.Position = new System.Drawing.Point( p.X, p.Y ) ;
      double dx = (double) ( p.X - rect.Left ) / ( rect.Right - rect.Left ) ;
      double dy = (double) ( p.Y - rect.Bottom ) / ( rect.Top - rect.Bottom ) ;
      IList<XYZ> corners = uiview.GetZoomCorners() ;
      XYZ a = corners[ 0 ] ;
      XYZ b = corners[ 1 ] ;
      XYZ v = b - a ;
      XYZ p3 = a + dx * v.X * XYZ.BasisX + dy * v.Y * XYZ.BasisY ;
      return p3 ;
    }

    //Convert Document hiện hành thành UIView
    private static UIView GetActiveUiView( UIDocument uidoc )
    {
      Document doc = uidoc.Document ;
      View view = doc.ActiveView ;
      IList<UIView> uiviews = uidoc.GetOpenUIViews() ;
      UIView uiview = null ;

      foreach ( UIView uv in uiviews ) {
        if ( uv.ViewId.Equals( view.Id ) ) {
          uiview = uv ;
          break ;
        }
      }

      return uiview ;
    }


    /// <summary>
    /// ファミリシンボルのタイプから指定名のタイプを取得
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="familySymbol"></param>
    /// <param name="strNewType"></param>
    /// <returns></returns>
    public static FamilySymbol GetFamilySymbol( Document doc, FamilySymbol familySymbol, string strNewType )
    {
      // 複製元のタイプを取得
      FamilySymbol sourceType = familySymbol ; // 複製元のタイプ

      //タイプが存在しない場合追加
      List<string> typeNamses = RevitUtil.ClsRevitUtil.GetTypeNames( doc, sourceType ) ;
      ElementType elementType = null ;
      if ( ! typeNamses.Contains( strNewType ) ) {
        // タイプを複製
        elementType = sourceType.Duplicate( strNewType ) ;
      }
      else {
        // タイプを取得
        elementType = RevitUtil.ClsRevitUtil.GetElementType( doc, familySymbol, strNewType ) ;
      }

      return (FamilySymbol) elementType ;
    }

    public static XYZ GetClosestPointOnVector( XYZ V1, XYZ VP1, XYZ P1 )
    {
      if ( V1.Z != 0 ) {
        XYZ flatV1 = new XYZ( V1.X, V1.Y, 0 ) ;
        XYZ flatP1 = new XYZ( P1.X, P1.Y, 0 ) ;
        XYZ flatVP1 = new XYZ( VP1.X, VP1.Y, 0 ) ;
        XYZ vecflat = ( flatP1 - flatVP1 ) ;
        double flatDotProduct = vecflat.DotProduct( flatV1.Normalize() ) ;
        XYZ flatcross = flatVP1 + flatV1.Normalize() * flatDotProduct ;

        XYZ direction = V1.Normalize() ;
        XYZ vectorToP1 = P1 - VP1 ;
        double dotProduct = vectorToP1.DotProduct( direction ) ;
        XYZ closestPoint = VP1 + direction * dotProduct ;

        XYZ flatcpp = new XYZ( closestPoint.X, closestPoint.Y, 0 ) ;
        XYZ flatV = ( flatcross - flatcpp ) ;
        XYZ realCross = closestPoint + new XYZ( flatV.X, flatV.Y, direction.Z ) ;
        XYZ v = realCross - closestPoint ;
        if ( direction.Z > 0 && v.Z < 0 || direction.Z < 0 && v.Z > 0 ) {
          realCross = closestPoint + new XYZ( flatV.X, flatV.Y, -direction.Z ) ;
        }

        return realCross ;
      }
      else {
        XYZ direction = V1.Normalize() ;
        XYZ vectorToP1 = P1 - VP1 ;
        double dotProduct = vectorToP1.DotProduct( direction ) ;
        return VP1 + direction * dotProduct ;
      }
    }

    public static XYZ GetClosestPointOnLine( Curve c, XYZ P1 )
    {
      XYZ vec = c.GetEndPoint( 1 ) - c.GetEndPoint( 0 ) ;
      return GetClosestPointOnVector( vec, c.GetEndPoint( 0 ), P1 ) ;
    }

    public static List<XYZ> GetCurvePoints( Document doc, ElementId id )
    {
      List<XYZ> retList = new List<XYZ>() ;

      try {
        Curve crv = GetCurve( doc, id ) ;
        retList.Add( crv.GetEndPoint( 0 ) ) ;
        retList.Add( crv.GetEndPoint( 1 ) ) ;
      }
      catch ( Exception ) {
        throw ;
      }

      return retList ;
    }

    /// <summary>
    /// ファミリインスタンスが持つホスト成分を返す
    /// Hostがいればそれを、作業面、集計レベルに置かれているだけならそのRefereceを返す
    /// </summary>
    /// <param name="ins"></param>
    /// <returns></returns>
    public static Reference GetReference( FamilyInstance ins )
    {
      Reference refer = null ;
      if ( ins.Host != null ) {
        if ( typeof( Level ) == ins.Host.GetType() ) {
          refer = ( ( ins.Host ) as Level ).GetPlaneReference() ;
        }
        else if ( typeof( ReferencePlane ) == ins.Host.GetType() ) {
          refer = ( ( ins.Host ) as ReferencePlane ).GetReference() ;
        }
        else if ( typeof( FamilyInstance ) == ins.Host.GetType() ) {
          if ( ins.HostFace != null ) {
            refer = ins.HostFace ;
          }
        }
      }
      else {
        Parameter hostlevId = ins.LookupParameter( "集計レベル" ) ;
        string levelName = hostlevId.AsValueString() ;
        ElementId lId = hostlevId.AsElementId() ;
        Element i = ins.Document.GetElement( lId ) ;
        if ( hostlevId == null || hostlevId.Element == null ) {
          refer = null ;
        }
        else if ( i != null && i.GetType() == typeof( Level ) ) {
          refer = ( ( i ) as Level ).GetPlaneReference() ;
        }
        else if ( i != null && i.GetType() == typeof( FamilyInstance ) ) {
          refer = new Reference( hostlevId.Element ) ;
        }
        else {
          hostlevId = ins.LookupParameter( "作業面" ) ;
          levelName = hostlevId.AsValueString() ;
          lId = hostlevId.AsElementId() ;
          i = ins.Document.GetElement( lId ) ;
          if ( hostlevId == null || hostlevId.Element == null ) {
            refer = null ;
          }
          else if ( i != null && i.GetType() == typeof( Level ) ) {
            refer = ( ( i ) as Level ).GetPlaneReference() ;
          }
          else if ( i != null && i.GetType() == typeof( FamilyInstance ) ) {
            refer = new Reference( hostlevId.Element ) ;
          }
        }
      }

      return refer ;
    }


    /// <summary>
    /// ファミリの端点を取得する
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public static Curve GetCurve( Document doc, ElementId id )
    {
      FamilyInstance instance = doc.GetElement( id ) as FamilyInstance ;
      Curve retCurve = null ;
      if ( instance == null ) {
        return null ;
      }

      if ( instance.Symbol.Family.FamilyPlacementType == FamilyPlacementType.CurveBased ) {
        LocationCurve lCurve = instance.Location as LocationCurve ;
        retCurve = lCurve.Curve ;
      }
      else {
        LocationPoint lPoint = instance.Location as LocationPoint ;
        XYZ pnt = lPoint.Point ;
        double length = ClsRevitUtil.GetParameterDouble( doc, id, DefineUtil.PARAM_LENGTH ) ;
        if ( length == null ) {
          return null ;
        }

        if ( length == 0 ) {
          var typeLength = ClsRevitUtil.GetTypeParameter( instance.Symbol, DefineUtil.PARAM_LENGTH ) ;
          if ( typeLength != 0 ) {
            length = typeLength ;
          }
        }

        XYZ pnt2 = pnt + instance.HandOrientation * length ;
        retCurve = Line.CreateBound( pnt, pnt2 ) ;
      }

      return retCurve ;
    }

    /// <summary>
    /// 柱の長さを求める
    /// </summary>
    /// <param name="instance"></param>
    /// <returns></returns>
    public static Curve GetPileCurve( FamilyInstance instance, Document doc )
    {
      LocationPoint lPoint = instance.Location as LocationPoint ;
      XYZ pnt = lPoint.Point ;
      double length = ClsRevitUtil.GetParameterDouble( doc, instance.Symbol.Id, DefineUtil.PARAM_LENGTH ) ;
      if ( length == null ) {
        return null ;
      }

      XYZ pnt2 = pnt - XYZ.BasisZ * length ;
      return Line.CreateBound( pnt, pnt2 ) ;
    }

    /// <summary>
    /// 構台の基準点オブジェクトを挿入する
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="point"></param>
    /// <param name="level"></param>
    /// <param name="isSubTr"></param>
    /// <returns></returns>
    public static ElementId InsertReferenceFamily( Document doc, XYZ point, Level level, bool isSubTr = false )
    {
      ElementId retId ;
      FamilySymbol sym ;
      if ( ! GantryUtil.GetFamilySymbol( doc,
            Path.Combine( PathUtil.GetGantyrUtilFamilyFolderPath(), $"{KoudaiReference.familyName}.rfa" ),
            KoudaiReference.familyName, out sym, true ) ) {
        return ElementId.InvalidElementId ;
      }

      if ( isSubTr ) {
        using ( SubTransaction tr = new SubTransaction( doc ) ) {
          tr.Start() ;
          ElementId CreatedID = RevitUtil.ClsRevitUtil.Create( doc, point, level.Id, sym ) ;
          tr.Commit() ;
          retId = CreatedID ;
        }
      }
      else {
        using ( Transaction tr = new Transaction( doc ) ) {
          tr.Start( "ReferenceFamilyload" ) ;
          ElementId CreatedID = RevitUtil.ClsRevitUtil.Create( doc, point, level.Id, sym ) ;
          tr.Commit() ;
          retId = CreatedID ;
        }
      }

      return retId ;
    }

    public static ElementId InsertPointFamily( Document doc, XYZ point, Level level, bool isSubTr = false )
    {
      ElementId retId ;
      FamilySymbol sym ;
      if ( ! GantryUtil.GetFamilySymbol( doc, PathUtil.GetGantyrUtilFamilyFolderPath() + "\\Point.rfa", "Point",
            out sym, true ) ) {
        return ElementId.InvalidElementId ;
      }


      if ( isSubTr ) {
        using ( SubTransaction tr = new SubTransaction( doc ) ) {
          tr.Start() ;
          ElementId CreatedID = RevitUtil.ClsRevitUtil.Create( doc, point, level.Id, sym ) ;
          ClsRevitUtil.SetParameter( doc, CreatedID, DefineUtil.PARAM_BASE_OFFSET, 0 ) ;
          tr.Commit() ;
          retId = CreatedID ;
        }
      }
      else {
        using ( Transaction tr = new Transaction( doc ) ) {
          tr.Start( "PointFamilyload" ) ;
          ElementId CreatedID = RevitUtil.ClsRevitUtil.Create( doc, point, level.Id, sym ) ;
          tr.Commit() ;
          retId = CreatedID ;
        }
      }

      return retId ;
    }

    public static ElementId InsertPointFamily( Document doc, XYZ point, Reference face, bool isSubTr = false )
    {
      ElementId retId ;
      FamilySymbol sym ;
      if ( ! GantryUtil.GetFamilySymbol( doc, PathUtil.GetGantyrUtilFamilyFolderPath() + "\\Point.rfa", "Point",
            out sym, true ) ) {
        return ElementId.InvalidElementId ;
      }


      if ( isSubTr ) {
        using ( SubTransaction tr = new SubTransaction( doc ) ) {
          tr.Start() ;
          ElementId CreatedID = CreateInstanceWith1point( doc, point, face, sym ).Id ;
          tr.Commit() ;
          retId = CreatedID ;
        }
      }
      else {
        using ( Transaction tr = new Transaction( doc ) ) {
          tr.Start( "PointFamilyload" ) ;
          ElementId CreatedID = CreateInstanceWith1point( doc, point, face, sym ).Id ;
          tr.Commit() ;
          retId = CreatedID ;
        }
      }

      return retId ;
    }

    public static ElementId InsertLineFamily( Document doc, XYZ start, XYZ end, Level level, bool isSubTr = false )
    {
      ElementId retId ;
      FamilySymbol sym ;
      if ( ! GantryUtil.GetFamilySymbol( doc, PathUtil.GetGantyrUtilFamilyFolderPath() + "\\GaRubberLine.rfa",
            "GaRubberLine", out sym, true ) ) {
        return ElementId.InvalidElementId ;
      }

      Curve curve = Line.CreateBound( start, end ) ;
      if ( isSubTr ) {
        using ( SubTransaction tr = new SubTransaction( doc ) ) {
          tr.Start() ;
          ElementId CreatedID = RevitUtil.ClsRevitUtil.Create( doc, curve, level.Id, sym ) ;
          tr.Commit() ;
          retId = CreatedID ;
        }
      }
      else {
        using ( Transaction tr = new Transaction( doc ) ) {
          tr.Start( "LineFamlyLoad" ) ;
          ElementId CreatedID = RevitUtil.ClsRevitUtil.Create( doc, curve, level.Id, sym ) ;
          tr.Commit() ;
          retId = CreatedID ;
        }
      }

      return retId ;
    }

    public static ElementId InsertLineFamily( Document doc, XYZ start, XYZ end, Reference face, bool isSubTr = false )
    {
      ElementId retId ;
      FamilySymbol sym ;
      if ( ! GantryUtil.GetFamilySymbol( doc, PathUtil.GetGantyrUtilFamilyFolderPath() + "\\GaRubberLine.rfa",
            "GaRubberLine", out sym, true ) ) {
        return ElementId.InvalidElementId ;
      }

      Curve curve = Line.CreateBound( start, end ) ;
      if ( isSubTr ) {
        using ( SubTransaction tr = new SubTransaction( doc ) ) {
          tr.Start() ;
          ElementId CreatedID = MaterialSuper.PlaceWithTwoPoints( sym, face, start, end, 0 ) ;
          tr.Commit() ;
          retId = CreatedID ;
        }
      }
      else {
        using ( Transaction tr = new Transaction( doc ) ) {
          tr.Start( "LineFamlyLoad" ) ;
          ElementId CreatedID = MaterialSuper.PlaceWithTwoPoints( sym, face, start, end, 0 ) ;
          tr.Commit() ;
          retId = CreatedID ;
        }
      }

      return retId ;
    }


    /// <summary>
    /// 指定部材の上にファミリシンボルを配置する
    /// </summary>
    /// <param name="uiDoc">ドキュメント</param>
    /// <param name="sel">セレクション</param>
    /// <param name="sym">ファミリシンボル</param>
    /// <param name="lev">指定部材からレベルが取得できなかった時の基準レベル</param>
    /// <param name="offset">指定レベルからオフセット値が取得できなかった時のオフセット値</param>
    /// <param name="isCurve">２点指定のファミリか</param>
    /// <param name="exStart">始点側突出長さ</param>
    /// <param name="exEnd">終点側突出長さ</param>
    /// <returns></returns>
    public static ElementId PlaceSymbolOverTheSelectedElm( UIDocument uiDoc, FamilySymbol sym, string koudaiName,
      Level lev = null, double offset = 0, double exStart = 0, double exEnd = 0, string functionName = "",
      Dictionary<string, string> paramList = null )
    {
      ElementId id = ElementId.InvalidElementId ;
      Selection sel = uiDoc.Selection ;

      //部材を指定
      ElementId selId = sel.PickObject( ObjectType.Element, "配置基準となる部材を選択してください" ).ElementId ;
      FamilyInstance ins = uiDoc.Document.GetElement( selId ) as FamilyInstance ;
      if ( ins == null ) {
        return id ;
      }

      double insOff = 0 ;
      Level level = GetInstanceLevelAndOffset( uiDoc.Document, ins, ref insOff ) ;
      if ( level == null ) {
        MessageUtil.Warning( "指定した部材のホストに設定されたレベルが取得できません\r\nホストがレベルの部材を選択してください", functionName ) ;
        return ElementId.InvalidElementId ;
      }

      XYZ p1, p2 ;
      ( p1, p2 ) = RevitUtil.ClsRevitUtil.GetSelect2Point( uiDoc ) ;
      if ( p1 == null || p2 == null ) {
        return id ;
      }

      double Z = level.Elevation ;
      p1 = new XYZ( p1.X, p1.Y, Z ) ;
      p2 = new XYZ( p2.X, p2.Y, Z ) ;
      if ( exStart != 0 || exEnd != 0 ) {
        XYZ vec = ( p2 - p1 ).Normalize() ;
        p1 += vec.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( exStart ) ;
        p2 += vec * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( exEnd ) ;
      }

      insOff += offset ;
      id = MaterialSuper.PlaceWithTwoPoints( sym, level.GetPlaneReference(), p1, p2,
        ClsRevitUtil.ConvertDoubleGeo2Revit( insOff ) ) ;
      if ( paramList != null ) {
        //FamilySymbol newSym = GantryUtil.ChangeInstanceTypeID(uiDoc.Document, sym, id, GantryUtil.CreateTypeName(MaterialSuper.typeName, paramList));
        FamilySymbol newSym = GantryUtil.DuplicateTypeWithNameRule( uiDoc.Document, koudaiName, sym, sym.Name ) ;

        //タイプパラメータ設定
        foreach ( KeyValuePair<string, string> kv in paramList ) {
          GantryUtil.SetParameterValueByParameterName( uiDoc.Document.GetElement( id ) as FamilyInstance, kv.Key,
            kv.Value ) ;
        }
      }

      return id ;
    }

    /// <summary>
    /// 指定点にシンボルを配置する
    /// </summary>
    /// <param name="uiDoc"></param>
    /// <param name="sel"></param>
    /// <param name="sym"></param>
    /// <param name="level"></param>
    /// <param name="offset"></param>
    /// <param name="isCurve"></param>
    /// <returns></returns>
    public static ElementId PlaceSymbolToSelectedPnt( UIDocument uiDoc, FamilySymbol sym, Level level,
      double offset = 0, double exStart = 0, double exEnd = 0 )
    {
      ElementId id = ElementId.InvalidElementId ;
      try {
        if ( sym.Family.FamilyPlacementType == FamilyPlacementType.CurveBased ) {
          XYZ p1, p2 ;
          ( p1, p2 ) = RevitUtil.ClsRevitUtil.GetSelect2Point( uiDoc ) ;
          if ( p1 == null || p1 == null ) {
            return id ;
          }

          double Z = level.Elevation ;
          p1 = new XYZ( p1.X, p1.Y, Z ) ;
          p2 = new XYZ( p2.X, p2.Y, Z ) ;
          if ( exStart != 0 || exEnd != 0 ) {
            XYZ vec = ( p2 - p1 ).Normalize() ;
            p1 += vec.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( exStart ) ;
            p2 += vec * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( exEnd ) ;
          }

          Curve c = Line.CreateBound( p1, p2 ) ;
          id = MaterialSuper.PlaceWithTwoPoints( sym, level.GetPlaneReference(), p1, p2,
            ClsRevitUtil.ConvertDoubleGeo2Revit( offset ) ) ;
        }
        else if ( sym.Family.FamilyPlacementType == FamilyPlacementType.WorkPlaneBased ) {
          XYZ point = uiDoc.Selection.PickPoint( ObjectSnapTypes.None, "挿入点を指定してください。" ) ;
          id = RevitUtil.ClsRevitUtil.Create( uiDoc.Document, point, level.Id, sym ) ;
          if ( offset != 0 ) {
            RevitUtil.ClsRevitUtil.SetParameter( uiDoc.Document, id, DefineUtil.PARAM_BASE_OFFSET, offset ) ;
          }
        }
      }
      catch ( Exception ) {
        id = ElementId.InvalidElementId ;
      }

      return id ;
    }

    /// <summary>
    /// 渡したファミリインスタンスの基準レベルおよびオフセット値を返す
    /// ファミリのつくりによっては返さない場合もあり
    /// 正しく取得できない場合はnull返
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="ins">ファミリインスタンス</param>
    /// <param name="offset"></param>
    /// <returns></returns>
    public static Level GetInstanceLevelAndOffset( Document doc, FamilyInstance ins, ref double offset )
    {
      Level level = null ;
      try {
        if ( ins.Host == null ) {
          Parameter hostlevId = ins.LookupParameter( "集計レベル" ) ;
          if ( hostlevId == null ) {
            hostlevId = ins.LookupParameter( "作業面" ) ;
          }

          string levelName = hostlevId.AsValueString() ;
          if ( hostlevId == null ) {
            return null ;
          }
          else if ( hostlevId.GetType() == typeof( FamilyInstance ) ) {
            FamilyInstance fam = doc.GetElement( hostlevId.AsElementId() ) as FamilyInstance ;
            level = GetInstanceLevelAndOffset( doc, fam, ref offset ) ;
          }
          else if ( hostlevId.GetType() == typeof( Level ) ) {
            level = doc.GetElement( hostlevId.AsElementId() ) as Level ;
          }
          else if ( GetAllLevelName( doc ).Contains( levelName ) ) {
            level = RevitUtil.ClsRevitUtil.GetLevel( doc, levelName ) as Level ;
          }
          else {
            ElementId id = hostlevId.AsElementId() ;
            Element el = doc.GetElement( id ) ;
            if ( el.GetType() == typeof( Level ) ) {
              level = el as Level ;
            }
          }
        }
        else {
          level = doc.GetElement( ins.Host.Id ) as Level ;
        }

        Parameter prm = null ;
        if ( level != null ) {
          prm = ins.LookupParameter( DefineUtil.PARAM_BASE_OFFSET ) ;
        }
        else {
          prm = ins.LookupParameter( DefineUtil.PARAM_HOST_OFFSET ) ;
        }


        if ( prm != null ) {
          offset = Math.Round( RevitUtil.ClsRevitUtil.CovertFromAPI( prm.AsDouble() ), 1 ) ;
        }
        else {
          offset = 0 ;
        }
      }
      catch ( Exception ) {
        return null ;
      }

      return level ;
    }

    /// <summary>
    /// selectionから部材のレベルとオフセット値を取得する
    /// </summary>
    /// <param name="uidoc"></param>
    /// <returns></returns>
    public static (Level, double) GetLevelAndOffsetWithSelect( UIDocument uidoc )
    {
      ElementId id = uidoc.Selection.PickObject( ObjectType.Element, "基準となる部材を選択してください" ).ElementId ;
      FamilyInstance ins = uidoc.Document.GetElement( id ) as FamilyInstance ;
      double offset = 0 ;
      Level level = GantryUtil.GetInstanceLevelAndOffset( uidoc.Document, ins, ref offset ) ;
      return ( level, offset ) ;
    }

    /// <summary>
    /// 直交する点を取得
    /// </summary>
    /// <param name="line"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    public static XYZ FindPerpendicularIntersection( Line line, XYZ point )
    {
      XYZ vectorToStart = line.GetEndPoint( 0 ) - point ;
      XYZ vectorToEnd = line.GetEndPoint( 1 ) - point ;
      XYZ vectorLine = line.GetEndPoint( 1 ) - line.GetEndPoint( 0 ) ;
      double dist = vectorLine.Normalize().DotProduct( vectorToStart ) ;
      return line.GetEndPoint( 0 ) + vectorLine.Normalize() * dist ;
    }

    /// <summary>
    /// Project a point on a face
    /// </summary>
    /// <param name="xyzArray">the face points, them fix a face </param>
    /// <param name="point">the point</param>
    /// <returns>the projected point on this face</returns>
    public static XYZ ProjectPoint( List<XYZ> xyzArray, Autodesk.Revit.DB.XYZ point )
    {
      XYZ a = xyzArray[ 0 ] - xyzArray[ 1 ] ;
      XYZ b = xyzArray[ 0 ] - xyzArray[ 2 ] ;
      XYZ c = point - xyzArray[ 0 ] ;

      XYZ normal = ( a.CrossProduct( b ) ) ;

      try {
        normal = normal.Normalize() ;
      }
      catch ( Exception ) {
        normal = XYZ.Zero ;
      }

      XYZ retProjectedPoint = point - ( normal.DotProduct( c ) ) * normal ;
      return retProjectedPoint ;
    }

    /// <summary>
    /// Faceの上に2点指定のファミリを配置する
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="line"></param>
    /// <param name="face"></param>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static FamilyInstance CreateFamilyInstanceOnFace( Document doc, Line line, Reference face,
      FamilySymbol symbol )
    {
      if ( symbol == null || face == null ) {
        return null ;
      }

      ElementId CreatedId = ElementId.InvalidElementId ;

      if ( ! symbol.IsActive ) {
        symbol.Activate() ;
      }

      CreatedId = doc.Create.NewFamilyInstance( face, line, symbol ).Id ;

      if ( CreatedId == ElementId.InvalidElementId ) {
        return null ;
      }
      else {
        return doc.GetElement( CreatedId ) as FamilyInstance ;
      }
    }

    /// <summary>
    /// FaceのMaterialElementIdを渡すと、親のレベルとオフセット値を返す
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="parentid"></param>
    /// <returns></returns>
    public static (Level, double) GetFaceLevelAndOffset( Document doc, ElementId parentid )
    {
      double retD = 0 ;
      FamilyInstance ins = doc.GetElement( parentid ) as FamilyInstance ;
      if ( ins == null ) {
        return ( null, retD ) ;
      }

      Level pLevel = GetInstanceLevelAndOffset( doc, ins, ref retD ) ;
      if ( pLevel == null && ins.Host != null ) {
        double ret = 0 ;
        ( pLevel, ret ) = GetFaceLevelAndOffset( doc, ins.Host.Id ) ;
        retD += ret ;
      }

      return ( pLevel, retD ) ;
    }

    /// <summary>
    /// ファミリの天端のZ値を取得する
    /// </summary>
    /// <param name="familyInstance"></param>
    /// <returns></returns>
    public static double GetTopElevationOfFamilyInstance( FamilyInstance familyInstance )
    {
      BoundingBoxXYZ boundingBox = familyInstance.get_BoundingBox( null ) ;

      if ( boundingBox != null ) {
        return boundingBox.Max.Z ;
      }

      return double.NaN ; // エラーの場合はNaN (Not a Number) を返す
    }

    /// <summary>
    /// ファミリ固有のインスタンスパラメータに値を保持するために必要な文字列を返す
    /// </summary>
    /// <param name="dictionary"></param>
    /// <returns></returns>
    public static string CombineDictionaryToString( Dictionary<string, string> dictionary )
    {
      return string.Join( ",", dictionary.Select( kv => $"{kv.Key}:{kv.Value}" ) ) ;
    }

    /// <summary>
    /// 文字列からディクショナリを作成する
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static Dictionary<string, string> CreateDictionaryFromString( string input )
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>() ;

      string[] keyValuePairs = input.Split( ',' ) ;

      foreach ( string pair in keyValuePairs ) {
        string[] keyValue = pair.Split( ':' ) ;
        if ( keyValue.Length == 2 ) {
          dictionary[ keyValue[ 0 ] ] = keyValue[ 1 ] ;
        }
      }

      return dictionary ;
    }

    /// <summary>
    /// ファミリインスタンスのトップにある面を取得する
    /// </summary>
    /// <param name="familyInstance"></param>
    /// <returns></returns>
    public static Face GetTopFaceOfFamilyInstance( FamilyInstance familyInstance )
    {
      if ( familyInstance == null ) {
        return null ;
      }

      Options options = new Options() ;
      options.ComputeReferences = true ;
      options.IncludeNonVisibleObjects = true ;

      GeometryElement geometryElement = familyInstance.get_Geometry( options ) ;
      if ( geometryElement == null ) {
        return null ;
      }

      Face retFace = null ;

      foreach ( GeometryInstance geometryObject in geometryElement ) {
        GeometryElement geom = geometryObject.SymbolGeometry ;
        foreach ( var obj in geom ) {
          if ( obj is Solid solid ) {
            foreach ( Face face in solid.Faces ) {
              if ( IsTopFace( face, familyInstance ) ) {
                if ( retFace != null && retFace.Area > face.Area ) {
                  continue ;
                }

                retFace = face ;
              }
            }
          }
        }
      }

      return retFace ;
    }

    public static Face IdentifyFaceOfFamilyInstance( FamilyInstance familyInstance, Reference refer )
    {
      Options options = new Options() ;
      options.ComputeReferences = true ;
      options.IncludeNonVisibleObjects = true ;

      GeometryElement geometryElement = familyInstance.get_Geometry( options ) ;
      Face retFace = null ;

      foreach ( GeometryInstance geometryObject in geometryElement ) {
        GeometryElement geom = geometryObject.SymbolGeometry ;
        foreach ( var obj in geom ) {
          if ( obj is Solid solid ) {
            foreach ( Face face in solid.Faces ) {
              if ( refer.ConvertToStableRepresentation( familyInstance.Document ) ==
                   face.Reference.ConvertToStableRepresentation( familyInstance.Document ) ) {
                retFace = face ;
                break ;
              }
            }
          }
        }
      }

      return retFace ;
    }

    private static bool IsTopFace( Face face, FamilyInstance familyInstance )
    {
      // ここにトップフェースを判定するための条件を追加
      // 例: Z軸方向の法線ベクトルが上向き（Zが正）の場合をトップフェースとする
      Transform ts = familyInstance.GetTotalTransform() ;
      XYZ transformedNormal = ts.OfVector( face.ComputeNormal( new UV( 0, 0 ) ) ) ;
      return RevitUtil.ClsGeo.GEO_GT0( transformedNormal.Z ) ;
    }


    /// <summary>
    /// ファミリインスタンスのトップにある面を取得する
    /// </summary>
    /// <param name="familyInstance"></param>
    /// <returns></returns>
    public static Face GetBtmFaceOfFamilyInstance( FamilyInstance familyInstance )
    {
      if ( familyInstance == null ) {
        return null ;
      }

      Options options = new Options() ;
      options.ComputeReferences = true ;
      options.IncludeNonVisibleObjects = true ;

      GeometryElement geometryElement = familyInstance.get_Geometry( options ) ;
      Face retFace = null ;

      foreach ( GeometryInstance geometryObject in geometryElement ) {
        GeometryElement geom = geometryObject.SymbolGeometry ;
        foreach ( var obj in geom ) {
          if ( obj is Solid solid ) {
            foreach ( Face face in solid.Faces ) {
              // 必要に応じて条件を追加
              if ( IsBtmFace( face, familyInstance ) ) {
                if ( retFace != null && retFace.Area > face.Area ) {
                  continue ;
                }

                retFace = face ;
              }
            }
          }
        }
      }

      return retFace ;
    }

    private static bool IsBtmFace( Face face, FamilyInstance familyInstance )
    {
      // ここにトップフェースを判定するための条件を追加
      // 例: Z軸方向の法線ベクトルが上向き（Zが正）の場合をトップフェースとする
      Transform tr = familyInstance.GetTransform() ;
      XYZ faceNormal = face.ComputeNormal( new UV( 0, 0 ) ) ;
      return RevitUtil.ClsGeo.GEO_LT0( faceNormal.Z ) ;
    }

    /// <summary>
    /// ファミリインスタンスの特定の方向を向くFaceを取得
    /// </summary>
    /// <param name="familyInstance"></param>
    /// <returns></returns>
    public static Face GetSpecifyFaceOfFamilyInstance( FamilyInstance familyInstance, XYZ faceVec )
    {
      if ( familyInstance == null ) {
        return null ;
      }

      Options options = new Options() ;
      options.ComputeReferences = true ;
      options.IncludeNonVisibleObjects = true ;

      GeometryElement geometryElement = familyInstance.get_Geometry( options ) ;
      if ( geometryElement == null ) {
        return null ;
      }

      Face retFace = null ;

      foreach ( GeometryInstance geometryObject in geometryElement ) {
        GeometryElement geom = geometryObject.SymbolGeometry ;
        foreach ( var obj in geom ) {
          if ( obj is Solid solid ) {
            foreach ( Face face in solid.Faces ) {
              Transform ts = familyInstance.GetTotalTransform() ;
              XYZ transformedNormal = ts.OfVector( face.ComputeNormal( new UV( 0, 0 ) ) ) ;
              if ( AreComponentsSameSign( transformedNormal, faceVec ) ) {
                if ( retFace != null && retFace.Area > face.Area ) {
                  continue ;
                }

                retFace = face ;
              }
            }
          }
        }
      }

      return retFace ;
    }

    /// <summary>
    /// 向いている方向が同じか
    /// </summary>
    /// <param name="vector1"></param>
    /// <param name="vector2"></param>
    /// <returns></returns>
    public static bool AreComponentsSameSign( XYZ vector1, XYZ vector2 )
    {
      //各XYZ成分の正負が一致していたらTrue
      if ( vector1.DistanceTo( vector2 ) < 0.1 ) {
        return true ;
      }

      double x1 = 0, y1 = 0, z1 = 0, x2 = 0, y2 = 0, z2 = 0 ;
      x1 = RevitUtil.ClsGeo.GEO_EQ0( vector1.X ) ? 0 : vector1.X ;
      y1 = RevitUtil.ClsGeo.GEO_EQ0( vector1.Y ) ? 0 : vector1.Y ;
      z1 = RevitUtil.ClsGeo.GEO_EQ0( vector1.Z ) ? 0 : vector1.Z ;

      x2 = RevitUtil.ClsGeo.GEO_EQ0( vector2.X ) ? 0 : vector2.X ;
      y2 = RevitUtil.ClsGeo.GEO_EQ0( vector2.Y ) ? 0 : vector2.Y ;
      z2 = RevitUtil.ClsGeo.GEO_EQ0( vector2.Z ) ? 0 : vector2.Z ;

      bool xSign = Math.Sign( x1 ) == Math.Sign( x2 ) ;
      bool ySign = Math.Sign( y1 ) == Math.Sign( y2 ) ;
      bool zSign = Math.Sign( z1 ) == Math.Sign( z2 ) ;

      return xSign && ySign && zSign ;
    }

    /// <summary>
    /// 図面内のすべての構台名を取得する
    /// </summary>
    /// <param name="doc"></param>
    /// <returns></returns>
    public static List<string> GetAllKoudaiName( Document doc )
    {
      List<string> retlist = new List<string>() ;
      try {
        ModelData mdata = ModelData.GetModelData( doc ) ;
        if ( mdata == null ) {
        }

        foreach ( KoudaiData kData in mdata.ListKoudaiData ) {
          retlist.Add( kData.AllKoudaiFlatData.KoudaiName ) ;
        }
      }
      catch {
        retlist = new List<string>() ;
      }

      return retlist ;
    }

    /// <summary>
    /// 指定した構台名の構台情報を取得
    /// </summary>
    /// <returns></returns>
    public static KoudaiData GetKoudaiData( Document doc, string koudaiName )
    {
      KoudaiData retData = new KoudaiData() ;
      ModelData mdata = ModelData.GetModelData( doc ) ;
      if ( mdata == null ) {
      }

      foreach ( KoudaiData kData in mdata.ListKoudaiData ) {
        if ( kData.AllKoudaiFlatData.KoudaiName.Equals( koudaiName ) ) {
          retData = kData ;
        }
      }

      return retData ;
    }

    /// <summary>
    /// ダイアログのString値が数値か確認する
    /// </summary>
    /// <param name="text">入力値</param>
    /// <param name="bMinus">0以下非許容か</param>
    /// <param name="bZero">0非許容か</param>
    /// <param name="bComma">小数点非許容か</param>
    /// <param name="bMax">最大値非許容か</param>
    /// <param name="dMax">最大値非許容の場合の最大値</param>
    /// <returns></returns>
    public static bool ChkDoubleValueAsString( string text, bool bMinus = false, bool bZero = false,
      bool bComma = false, bool bMax = false, double dMax = 0 )
    {
      try {
        double d = 0 ;
        //数値チェック
        if ( ! double.TryParse( text, out d ) ) {
          return false ;
        }

        //マイナスチェック
        if ( bMinus && d < 0 ) {
          return false ;
        }

        //ゼロチェック
        if ( bZero && d == 0 ) {
          return false ;
        }

        //小数点以下チェック
        double decimal_part = d % 1 ;
        if ( bComma && decimal_part != 0 ) {
          return false ;
        }

        //上限値チェック
        if ( bMax && d > dMax ) {
          return false ;
        }
      }
      catch ( System.Exception e ) {
        return false ;
      }

      return true ;
    }

    public static XYZ FindIntersectionPoint( Curve curve1, Curve curve2 )
    {
      // Curve1とCurve2の交点を求める
      IntersectionResultArray intersectionResults = new IntersectionResultArray() ;
      SetComparisonResult result = curve1.Intersect( curve2, out intersectionResults ) ;

      if ( result == SetComparisonResult.Overlap && intersectionResults.Size > 0 ) {
        // 交点を取得
        XYZ intersectionPoint = intersectionResults.get_Item( 0 ).XYZPoint ;

        return intersectionPoint ;
      }

      return null ;
    }

    public static List<XYZ> FindIntersectionPnts( Document doc, ElementId id1, ElementId id2 )
    {
      List<XYZ> retList = new List<XYZ>() ;
      Element targetElement = doc.GetElement( id1 ) ;

      BoundingBoxXYZ targetBoundingBox = targetElement.get_BoundingBox( null ) ;
      XYZ min = new XYZ( targetBoundingBox.Min.X - 0.1, targetBoundingBox.Min.Y - 0.1, targetBoundingBox.Min.Z - 10 ) ;
      XYZ max = new XYZ( targetBoundingBox.Max.X + 0.1, targetBoundingBox.Max.Y + 0.1, targetBoundingBox.Max.Z + 10 ) ;
      Outline targetOutline = new Outline( min, max ) ;

      Element elem = doc.GetElement( id2 ) ;
      if ( typeof( FamilyInstance ) != elem.GetType() ) {
        return retList ;
      }

      retList.AddRange( OverlapCheck( targetElement, elem ) ) ;

      return retList ;
    }

    public static List<XYZ> OverlapCheck( Element elm1, Element elm2, double distanceBorder = 0.1 )
    {
      //elm1
      EdgeArray elm1EdgeArrays = RevitUtil.ClsRevitUtil.GetEdgeArray( elm1 ) ;
      List<Curve> elm1CurveList = new List<Curve>() ;
      List<XYZ> retList = new List<XYZ>() ;

      foreach ( Edge edge in elm1EdgeArrays ) {
        Curve curve = edge.AsCurve() ;
        XYZ curveSP = curve.GetEndPoint( 0 ) ;
        XYZ curveEP = curve.GetEndPoint( 1 ) ;

        if ( curveSP.X == curveEP.X && curveSP.Y == curveEP.Y && curveSP.Z == curveEP.Z )
          continue ;

        //レベル面に作成したライン
        try {
          if ( ClsGeo.GEO_EQ( curveSP.X, curveEP.X ) && ClsGeo.GEO_EQ( curveSP.Y, curveEP.Y ) &&
               ClsGeo.GEO_EQ( curveSP.Z, curveEP.Z ) ) {
            continue ;
          }

          Curve newCurve = Line.CreateBound( curveSP, curveEP ) ;
          elm1CurveList.Add( newCurve ) ;
        }
        catch {
        }
      }

      //elm2のアウトラインを取得
      EdgeArray elm2EdgeArrays = ClsRevitUtil.GetEdgeArray( elm2 ) ;
      List<Curve> elm2CurveList = new List<Curve>() ;

      foreach ( Edge edge in elm2EdgeArrays ) {
        Curve curve = edge.AsCurve() ;
        XYZ curveSP = curve.GetEndPoint( 0 ) ;
        XYZ curveEP = curve.GetEndPoint( 1 ) ;

        if ( curveSP.X == curveEP.X && curveSP.Y == curveEP.Y && curveSP.Z == curveEP.Z )
          continue ;

        //レベル面に作成したライン
        try {
          if ( ClsGeo.GEO_EQ( curveSP.X, curveEP.X ) && ClsGeo.GEO_EQ( curveSP.Y, curveEP.Y ) &&
               ClsGeo.GEO_EQ( curveSP.Z, curveEP.Z ) ) {
            continue ;
          }

          Curve newCurve = Line.CreateBound( curveSP, curveEP ) ;
          elm2CurveList.Add( newCurve ) ;
        }
        catch {
        }
      }

      //elm1Polyとelm2Polyが干渉するとき
      foreach ( Curve elm1Curve in elm1CurveList ) {
        foreach ( Curve elm2Curve in elm2CurveList ) {
          List<XYZ> lst = GetLineDistance( elm1Curve, elm2Curve ) ;
          foreach ( XYZ p in lst ) {
            if ( ! IsXYZInList( p, retList ) ) {
              retList.Add( p ) ;
            }
          }
        }
      }

      return retList ;
    }

    static List<XYZ> GetLineDistance( Curve c1, Curve c2 )
    {
      double tole = 0.1 ;
      double shortestDistance ;
      List<XYZ> retList = new List<XYZ>() ;

      Line line1 = Line.CreateBound( c1.GetEndPoint( 0 ), c1.GetEndPoint( 1 ) ) ;
      Line line2 = Line.CreateBound( c2.GetEndPoint( 0 ), c2.GetEndPoint( 1 ) ) ;

      IntersectionResultArray results ;
      SetComparisonResult comparisonResult = line1.Intersect( line2, out results ) ; // 線分1と線分2の交差を確認

      if ( comparisonResult == SetComparisonResult.Overlap ) {
        foreach ( IntersectionResult res in results ) {
          retList.Add( res.XYZPoint ) ;
        }
      }
      else {
        XYZ p1 = line1.GetEndPoint( 0 ) ;
        XYZ q1 = line1.GetEndPoint( 1 ) ;
        XYZ p2 = line2.GetEndPoint( 0 ) ;
        XYZ q2 = line2.GetEndPoint( 1 ) ;

        XYZ v = q1 - p1 ;
        XYZ w = q2 - p2 ;

        XYZ u = p1 - p2 ;

        double a = v.DotProduct( v ) ; // vの長さの2乗
        double b = v.DotProduct( w ) ; // vとwの内積
        double c = w.DotProduct( w ) ; // wの長さの2乗
        double d = u.DotProduct( v ) ; // uとvの内積
        double e = u.DotProduct( w ) ; // uとwの内積

        double denominator = a * c - b * b ;

        double t1, t2 ;

        if ( denominator != 0 ) {
          t1 = ( b * e - c * d ) / denominator ;
          t2 = ( a * e - b * d ) / denominator ;
        }
        else {
          t1 = 0 ; // 分母がゼロの場合、0を代入
          t2 = ( d / b != 0 ) ? e / b : 0 ; // 分母がゼロの場合は分母も0にする
        }

        if ( t1 < 0 ) {
          shortestDistance = p1.DistanceTo( p2 ) ; // 線分1の範囲外で最も近い点はp1とp2の距離
          if ( shortestDistance < tole ) {
            retList.Add( ProjectPointToCurve( p2, c1 ) ) ;
          }
        }
        else if ( t1 > 1 ) {
          shortestDistance = q1.DistanceTo( q2 ) ; // 線分1の範囲外で最も近い点はq1とq2の距離
          if ( shortestDistance < tole ) {
            retList.Add( ProjectPointToCurve( q2, c1 ) ) ;
          }
        }
        else if ( t2 < 0 ) {
          shortestDistance = p1.DistanceTo( p2 ) ; // 線分2の範囲外で最も近い点はp1とp2の距離
          if ( shortestDistance < tole ) {
            retList.Add( ProjectPointToCurve( p2, c1 ) ) ;
          }
        }
        else if ( t2 > 1 ) {
          shortestDistance = q1.DistanceTo( q2 ) ; // 線分2の範囲外で最も近い点はq1とq2の距離
          if ( shortestDistance < tole ) {
            retList.Add( ProjectPointToCurve( q2, c1 ) ) ;
          }
        }
        else {
          XYZ closestPointOnLine1 = p1 + t1 * v ;
          XYZ closestPointOnLine2 = p2 + t2 * w ;

          shortestDistance = closestPointOnLine1.DistanceTo( closestPointOnLine2 ) ;
          if ( shortestDistance < tole ) {
            retList.Add( closestPointOnLine1 ) ;
          }
        }
      }

      return retList ;
    }

    public static XYZ ProjectPointToCurve( XYZ point, Curve curve )
    {
      // Curve上に点を射影
      try {
        double parameter = curve.Project( point ).Parameter ;
        XYZ projectedPoint = curve.Evaluate( parameter, false ) ;
        return projectedPoint ;
      }
      catch ( Exception ex ) {
        return null ;
      }
    }

    public static bool IsXYZInList( XYZ pointToCheck, List<XYZ> xyzList )
    {
      double tolerance = 0.0001 ; // 許容誤差

      foreach ( XYZ point in xyzList ) {
        if ( point.DistanceTo( pointToCheck ) < tolerance ) {
          return true ;
        }
      }

      return false ;
    }

    /// <summary>
    /// 指定ビュータイプを取得
    /// </summary>
    /// <param name="doc">Document</param>
    /// <param name="name">ビュー名</param>
    public static Autodesk.Revit.DB.ViewType getViewType( Document doc, string viewName )
    {
      Autodesk.Revit.DB.ViewType getV = ViewType.FloorPlan ;

      IList<Element> views = new FilteredElementCollector( doc ).OfClass( typeof( Autodesk.Revit.DB.View ) )
        .ToElements() ;
      foreach ( Autodesk.Revit.DB.View v in views ) {
        if ( v.Name == viewName ) {
          getV = v.ViewType ;
          break ;
        }
      }

      return getV ;
    }

    /// <summary>
    /// 指定したタイプパラメータの値を取得
    /// </summary>
    /// <param name="fs">ファミリシンボル</param>
    /// <param name="paramName">タイプパラメータ名</param>
    /// <returns>タイプパラメータの値を返却</returns>
    public static string GetTypeParameter( FamilySymbol fs, string paramName, bool isValue = false )
    {
      try {
        Parameter parm = fs.LookupParameter( paramName ) ;
        if ( parm == null ) {
          return string.Empty ;
        }

        if ( isValue ) {
          return parm.AsValueString() ;
        }

        return parm.AsString() ;
      }
      catch {
        return string.Empty ;
      }
    }

    public static ElementId GetTypeParameterElementId( FamilySymbol fs, string paramName )
    {
      try {
        Parameter parm = fs.LookupParameter( paramName ) ;
        if ( parm == null ) {
          return null ;
        }

        return parm.AsElementId() ;
      }
      catch {
        return null ;
      }
    }

    /// <summary>
    /// 指定のElementからファミリ名を取得
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public static string GetFamilyName( Element e )
    {
      var eId = e?.GetTypeId() ;
      if ( eId == null )
        return "" ;
      var elementType = e.Document.GetElement( eId ) as ElementType ;
      return elementType?.FamilyName ?? "" ;
    }

    /// <summary>
    /// 指定のElementからタイプ名を取得
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public static string GetTypeName( Element e )
    {
      var eId = e?.GetTypeId() ;
      if ( eId == null )
        return "" ;
      var elementType = e.Document.GetElement( eId ) as ElementType ;
      return elementType?.Name ?? "" ;
    }

    /// <summary>
    /// 長さ変更時、中心を基点に伸び縮みさせる
    /// </summary>
    /// <param name="doc">ドキュメント</param>
    /// <param name="targetId">移動させるElementId</param>
    /// <param name="originalLength">サイズ変更前の長さ</param>
    /// <param name="editedLength">サイズ変更後の長さ</param>
    /// <returns></returns>
    public static bool SizeEditMove( Document doc, ElementId targetId, double originalLength, double editedLength,
      bool isBasePointTop, bool isVertical = false )
    {
      //長さが変わらなければ何もしない
      if ( ClsGeo.GEO_EQ( originalLength, editedLength ) ) {
        return true ;
      }

      bool resBool = false ;
      try {
        //部材の終始点を取得
        Curve c = GetCurve( doc, targetId ) ;

        if ( isVertical ) {
          var instance = GetFamilyInstance( doc, targetId ) ;
          c = GetPileCurve( instance, doc ) ;
        }

        //終始点の取れない部材は対象外
        if ( c != null ) {
          //標準ベクトルを取得
          XYZ cVec = ( ( isBasePointTop ? c.GetEndPoint( 1 ) : c.GetEndPoint( 0 ) ) -
                       ( isBasePointTop ? c.GetEndPoint( 0 ) : c.GetEndPoint( 1 ) ) ).Normalize() ;
          //移動距離
          double dist = Math.Abs( originalLength - editedLength ) / 2 ;
          //移動方向 短くなる場合は 始⇒終 伸びる倍は逆
          XYZ moveVec = ( ( originalLength > editedLength ) ? cVec : cVec.Negate() ) *
                        RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( dist ) ;
          //移動
          ElementTransformUtils.MoveElement( doc, targetId, moveVec ) ;
          resBool = true ;
        }
      }
      catch ( Exception ) {
        resBool = false ;
      }

      return resBool ;
    }

    /// <summary>
    /// 命名側にしたがってタイプを複製する
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="koudaiName">構台名</param>
    /// <param name="original">複製元となるタイプ</param>
    /// <param name="originalTypeName">大元のタイプ名</param>
    /// <example>大元のタイプ名==大引、根太など。既にナンバリングされているタイプからの複製でも大元で判別する</example>
    /// <returns></returns>
    public static FamilySymbol DuplicateTypeWithNameRule( Document doc, string koudaiName, FamilySymbol original,
      string originalTypeName, string koudaiTypeName = "" )
    {
      if ( original == null ) {
        return null ;
      }

      FamilySymbol resSym = original ;
      try {
        //構台情報取得
        KoudaiData kData = GetKoudaiData( doc, koudaiName ) ;
        bool noData = false ;
        AllKoudaiFlatFrmData data = null ;
        if ( kData == null || kData.AllKoudaiFlatData == null ) {
          noData = true ;
        }
        else {
          data = kData.AllKoudaiFlatData ;
        }

        string koudaiType = ( data != null ) ? data.KoudaiType : koudaiTypeName ;

        int maxNum = 0 ;

        //タイプ名検索用
        string typeName = $"{originalTypeName}_{koudaiType}_{koudaiName}" ;
        List<string> types = GetAllTypeName( doc, originalTypeName ) ;

        if ( ! noData ) {
          Family family = original.Family ;
          //ナンバリング複製対象は下記の想定
          //ナンバリングインクリメント
          foreach ( string tName in types ) {
            if ( tName.StartsWith( typeName ) ) {
              string numSt = tName.Replace( $"{typeName}_", "" ) ;
              if ( numSt.Length != 3 ) {
                numSt = numSt.Substring( 0, numSt.Length - 3 ) ;
              }

              int num = RevitUtil.ClsCommonUtils.ChangeStrToInt( numSt ) ;
              if ( maxNum <= num ) {
                maxNum = num ;
              }
            }
          }
        }

        typeName = $"{typeName}_{( maxNum + 1 ).ToString( "000" )}" ;
        while ( types.Contains( typeName ) ) {
          maxNum += 1 ;
          typeName = $"{originalTypeName}_{koudaiType}_{koudaiName}_{maxNum.ToString( "000" )}" ;
        }


        ElementType duplicated = original.Duplicate( typeName ) ;
        resSym = (FamilySymbol) duplicated ;
      }
      //複製失敗
      catch ( Exception ex ) {
        resSym = original ;
      }

      return resSym ;
    }

    /// <summary>
    /// 命名側に従ったファミリタイプ名から元のタイプ名を取得
    /// </summary>
    /// <returns></returns>
    public static string GetOriginalTypeName( Document doc, FamilyInstance instance )
    {
      //システム外で置かれたファミリは無視
      MaterialSuper ms = MaterialSuper.ReadFromElement( instance.Id, doc ) ;
      if ( ms == null ) {
        return "" ;
      }

      string name = instance.Symbol.Name ;
      //システムで複製しているファミリか
      if ( name.Contains( "_" ) ) {
        int ind = name.IndexOf( '_' ) ;
        return name.Substring( 0, ind ) ;
      }
      else {
        return name ;
      }
    }


    public List<(string, int)> GetBoltInfo( Document doc, ElementId targetID )
    {
      List<(string, int)> retList = new List<(string, int)>() ;
      try {
        MaterialSuper ms = MaterialSuper.ReadFromElement( targetID, doc ) ;
        if ( ms == null ) {
          return retList ;
        }

        if ( ms.m_TeiketsuType == eJoinType.Bolt ) {
          string bolt1Info = ms.m_BoltInfo1 ;
          int bolt1Cnt = ms.m_Bolt1Cnt ;
          if ( bolt1Info != "" ) {
            retList.Add( ( bolt1Info, bolt1Cnt ) ) ;
          }

          string bolt2Info = ms.m_BoltInfo2 ;
          int bolt2Cnt = ms.m_Bolt2Cnt ;
          if ( bolt2Info != "" ) {
            retList.Add( ( bolt2Info, bolt2Cnt ) ) ;
          }
        }
      }
      catch ( Exception ) {
        retList = new List<(string, int)>() ;
      }

      return retList ;
    }
  }

  public enum SteelShape
  {
    H,
    L,
    C,
    MD,
    HA,
    SMH,
    PL
  }

  public class SteelSize
  {
    public SteelShape Shape { get ; set ; }
    public double Height { get ; set ; }
    public double FrangeWidth { get ; set ; }
    public double WebThick { get ; set ; }
    public double FrangeThick { get ; set ; }

    public double FrangeWidthFeet => ClsRevitUtil.CovertToAPI( FrangeWidth ) ;
    public double HeightFeet => ClsRevitUtil.CovertToAPI( Height ) ;
    public double WebThickFeet => ClsRevitUtil.CovertToAPI( WebThick ) ;
  }

  public enum MaterialShape
  {
    H,
    L,
    C,
    MD,
    HA,
    SMH,
    PL
  }

  public class MaterialSize
  {
    public MaterialShape Shape { get ; set ; }
    public double Height { get ; set ; }
    public double Width { get ; set ; }
    public double Thick { get ; set ; }
  }


  class LoadOpts : IFamilyLoadOptions
  {
    public bool OnFamilyFound( bool familyInUse, out bool overwriteParameterValues )
    {
      overwriteParameterValues = true ;
      return true ;
    }

    public bool OnSharedFamilyFound( Family sharedFamily, bool familyInUse, out FamilySource source,
      out bool overwriteParameterValues )
    {
      source = FamilySource.Family ;
      overwriteParameterValues = true ;
      return true ;
    }
  }
}