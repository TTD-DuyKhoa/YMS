using System ;
using System.Collections.Generic ;
using System.IO ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Xml ;
using System.Xml.Serialization ;
using Autodesk.Revit.DB ;
using Autodesk.Revit.DB.ExtensibleStorage ;

namespace YMS_gantry
{
  /// <summary>
  /// Extensible StrageのUtil的クラス
  /// </summary>
  public class ExStrageUtils
  {
    static Document _doc { get ; set ; }
    static private string venderName = "Fresco" ;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="doc"></param>
    public ExStrageUtils( Document doc )
    {
      _doc = doc ;
    }

    /// <summary>
    /// ストレージ作成しIdのエレメントに付与する
    /// </summary>
    /// <param name="data"></param>
    /// <param name="attachedId"></param>
    public static void CreateAndStoreData( GantryData data, ElementId attachedId )
    {
      if ( attachedId == ElementId.InvalidElementId || data == null ) {
        return ;
      }

      using ( Transaction tr = new Transaction( _doc, "CreateExStrageAndStore" ) ) {
        tr.Start() ;

        Element el = _doc.GetElement( attachedId ) ;
        if ( el == null ) {
          return ;
        }

        SchemaWrapper wrapper = SchemaWrapper.NewSchema( Guid.NewGuid(), AccessLevel.Public, AccessLevel.Vendor,
          venderName, "", data.SchemaName, "" ) ;

        //Entity ent = CreateEntityFromDict(data.DataSet,wrapper);
        Entity ent = new Entity() ;
        el.SetEntity( ent ) ; // store the entity in the element
        tr.Commit() ;
      }
    }

    /// <summary>
    /// DictionaryからEntityに変換
    /// </summary>
    /// <param name="dicParam"></param>
    /// <returns></returns>
    public static Entity CreateEntityFromDict( Dictionary<string, object> dicParam, SchemaWrapper mSchemaWrapper )
    {
      foreach ( KeyValuePair<string, object> kv in dicParam ) {
        AddFieldToWrapper( kv.Key, kv.Value, ref mSchemaWrapper ) ;
      }

      //mSchemaWrapper.Data.DataList

      //return new Entity(new Guid(mSchemaWrapper.Data.SchemaId));
      //return new Entity();
      Schema s = Schema.ListSchemas()[ 0 ] ;

      mSchemaWrapper.SetSchema( s ) ;
      return new Entity( mSchemaWrapper.GetSchema() ) ;
    }

    /// <summary>
    /// SchemaWrapperにフィールドを追加する
    /// </summary>
    /// <param name="name">フィールド名</param>
    /// <param name="val">値</param>
    /// <param name="wrapper">追加したいSchemaWrapper</param>
    public static void AddFieldToWrapper( string name, object val, ref SchemaWrapper wrapper )
    {
      Type t = val.GetType() ;
      switch ( val ) {
        case int i :
          wrapper.AddField<int>( name, new ForgeTypeId(), null ) ;
          break ;
        case short s :
          wrapper.AddField<short>( name, new ForgeTypeId(), null ) ;
          break ;
        case byte b :
          wrapper.AddField<byte>( name, new ForgeTypeId(), null ) ;
          break ;
        case double d :
          wrapper.AddField<double>( name, SpecTypeId.Length, null ) ;
          break ;
        case float f :
          wrapper.AddField<float>( name, SpecTypeId.Length, null ) ;
          break ;
        case bool bo :
          wrapper.AddField<bool>( name, new ForgeTypeId(), null ) ;
          break ;
        case string st :
          wrapper.AddField<string>( name, new ForgeTypeId(), null ) ;
          break ;
        case Guid guid :
          wrapper.AddField<Guid>( name, new ForgeTypeId(), null ) ;
          break ;
        case ElementId eId :
          wrapper.AddField<ElementId>( name, new ForgeTypeId(), null ) ;
          break ;
        case XYZ xyz :
          wrapper.AddField<XYZ>( name, SpecTypeId.Length, null ) ;
          break ;
        case UV uv :
          wrapper.AddField<UV>( name, SpecTypeId.Length, null ) ;
          break ;
        case List<Type> list :
          wrapper.AddField<IList<Type>>( name, SpecTypeId.Length, null ) ;
          break ;
        case IDictionary<Type, Type> dic :
          wrapper.AddField<IDictionary<string, string>>( name, new ForgeTypeId(), null ) ;
          break ;
        case Schema s :
          SchemaWrapper subSchema = SchemaWrapper.NewSchema( s.GUID, s.ReadAccessLevel, s.WriteAccessLevel, s.VendorId,
            "", s.SchemaName, s.Documentation ) ;
          wrapper.AddField<Entity>( name, new ForgeTypeId(), subSchema ) ;
          break ;
        default :
          break ;
      }

      return ;
    }

    /// <summary>
    /// ドキュメントに拡張可能なストレージが存在する場合は true を返し、それ以外の場合は false を返します。
    /// </summary>
    public static bool DoesAnyStorageExist()
    {
      IList<Schema> schemas = Schema.ListSchemas() ;
      if ( schemas.Count == 0 )
        return false ;
      else {
        foreach ( Schema schema in schemas ) {
          List<ElementId> ids = ElementsWithStorage( schema ) ;
          if ( ids.Count > 0 )
            return true ;
        }

        return false ;
      }
    }

    /// <summary>
    /// 拡張可能なストレージを含むすべての要素のスキーマ GUID と要素情報を含むフォーマットされた文字列を返します。
    /// </summary>
    public static string GetElementsWithAllSchemas()
    {
      StringBuilder sBuilder = new StringBuilder() ;
      IList<Schema> schemas = Schema.ListSchemas() ;
      if ( schemas.Count == 0 )
        return "No schemas or storage." ;
      else {
        foreach ( Schema schema in schemas ) {
          sBuilder.Append( GetElementsWithSchema( schema ) ) ;
        }

        return sBuilder.ToString() ;
      }
    }

    /// <summary>
    /// 特定のスキーマの拡張可能なストレージを含むすべての要素のスキーマ GUID と
    /// 要素情報を含むフォーマットされた文字列を返します。
    /// </summary>
    private static string GetElementsWithSchema( Schema schema )
    {
      StringBuilder sBuilder = new StringBuilder() ;
      sBuilder.AppendLine( "Schema: " + schema.GUID.ToString() + ", " + schema.SchemaName ) ;
      List<ElementId> elementsofSchema = ElementsWithStorage( schema ) ;
      if ( elementsofSchema.Count == 0 )
        sBuilder.AppendLine( "No elements." ) ;
      else {
        foreach ( ElementId id in elementsofSchema ) {
          sBuilder.AppendLine( GetElementInfo( id ) ) ;
        }
      }

      return sBuilder.ToString() ;
    }

    /// <summary>
    /// ExtensibleStorageFilter ElementQuickFilter を使用し
    /// 特定のスキーマの拡張可能なストレージを含む ElementId のリストを返します。
    /// </summary>
    private static List<ElementId> ElementsWithStorage( Schema schema )
    {
      List<ElementId> ids = new List<ElementId>() ;
      FilteredElementCollector collector = new FilteredElementCollector( _doc ) ;
      collector.WherePasses( new ExtensibleStorageFilter( schema.GUID ) ) ;
      ids.AddRange( collector.ToElementIds() ) ;
      return ids ;
    }

    /// <summary>
    /// ElementInfoを文字列にして返します
    /// </summary>
    private static string GetElementInfo( ElementId id )
    {
      Element element = _doc.GetElement( id ) ;
      string retval = ( element.Id.ToString() + ", " + element.Name + ", " + element.GetType().FullName ) ;
      return retval ;
    }

    #region xml

    /// <summary>
    /// XMLのシリアライズメソッド
    /// </summary>
    /// <typeparam name="Type">型情報</typeparam>
    /// <param name="strFilePath">ファイルパス</param>
    /// <param name="serializeTarget">シリアライズ対象のインスタンス</param>
    /// <returns>True:正常/False:エラー</returns>
    public static bool WriteXml<Type>( string strFilePath, Type serializeTarget )
    {
      try {
        XmlSerializer s = new XmlSerializer( serializeTarget.GetType() ) ;
        using ( StreamWriter sw = new StreamWriter( strFilePath, false, new UTF8Encoding( false ) ) ) {
          s.Serialize( sw, serializeTarget ) ;
          sw.Close() ;
        }
      }
      catch ( System.Exception ) {
        return false ;
      }

      return true ;
    }

    /// <summary>
    /// XMLのデシリアライズメソッド
    /// </summary>
    /// <typeparam name="Type">型情報</typeparam>
    /// <param name="strFilePath">ファイルパス</param>
    /// <param name="data">デシリアライズで取得したデータをセットするインスタンス</param>
    /// <returns>True:正常/False:エラー</returns>
    public static bool ReadXml<Type>( string strFilePath, ref Type data )
    {
      bool bRet = false ;
      try {
        using ( XmlReader xr = XmlReader.Create( strFilePath ) ) {
          XmlSerializer s = new XmlSerializer( typeof( Type ) ) ;
          bRet = s.CanDeserialize( xr ) ;
          if ( bRet ) {
            data = (Type) s.Deserialize( xr ) ;
          }

          xr.Close() ;
        }
      }
      catch ( System.Exception ) {
        return false ;
      }

      return bRet ;
    }

    #endregion

    #region text

    /// <summary>
    /// テキストの書き込み
    /// </summary>
    /// <param name="strFilePath">ファイルパス</param>
    /// <param name="str">書き込み文字列</param>
    /// <returns></returns>
    public static bool WriteText( string strFilePath, string str )
    {
      try {
        StreamWriter sw = new StreamWriter( strFilePath, false ) ;
        sw.WriteLine( str ) ;
        sw.Close() ;
      }
      catch ( System.Exception ) {
        return false ;
      }

      return true ;
    }

    /// <summary>
    /// テキストの読み込み
    /// </summary>
    /// <param name="strFilePath">ファイルパス</param>
    /// <param name="str">取得文字列</param>
    /// <returns></returns>
    public static bool ReadText( string strFilePath, ref string str )
    {
      bool bRet = false ;
      try {
        string line ;
        StreamReader sr = new StreamReader( strFilePath ) ;
        line = sr.ReadLine() ;
        while ( line != null ) {
          if ( string.IsNullOrEmpty( str ) ) {
            str = line ;
          }
          else {
            str = str + "\n" + line ;
          }

          line = sr.ReadLine() ;
        }

        //close the file
        sr.Close() ;
        bRet = true ;
      }
      catch ( System.Exception ) {
        return false ;
      }

      return bRet ;
    }

    #endregion
  }
}