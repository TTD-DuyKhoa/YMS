using Autodesk.Revit.DB ;
using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.Data ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using static Autodesk.Internal.Windows.SwfMediaPlayer ;

namespace YMS.Master
{
  public class ClsSyabariPieceCSV
  {
    public int Id { get ; set ; }
    public string TypeName { get ; set ; }
    public string SizeName { get ; set ; }
    public bool EnableShabari { get ; set ; }
    public bool EnableHiuchi { get ; set ; }
    public string FamilyPath { get ; set ; }
    public string SymbolL { get ; set ; }
    public string SymbolR { get ; set ; }

    public string FamilyFullPath => System.IO.Path.Combine( Parts.ClsZumenInfo.GetYMSFolder(), FamilyPath ) ;

    private static string CsvFileName => "SyabariPiece.csv" ;

    private static Lazy<ClsSyabariPieceCSV[]> _Shared { get ; } = new Lazy<ClsSyabariPieceCSV[]>( () =>
    {
      var result = new List<ClsSyabariPieceCSV>() ;
      string symbolFolpath = Parts.ClsZumenInfo.GetYMSFolder() ;
      string fileName = System.IO.Path.Combine( symbolFolpath, Parts.ClsZumenInfo.master, CsvFileName ) ;

      var table = SLibCsvSerializer.Deserialize( fileName, Encoding.GetEncoding( "Shift_JIS" ) ) ;

      foreach ( DataRow row in table.Rows ) {
        if ( int.TryParse( row[ "Id" ].ToString(), out var id ) ) {
          result.Add( new ClsSyabariPieceCSV
          {
            Id = id,
            TypeName = row[ "TypeName" ].ToString(),
            SizeName = row[ "SizeName" ].ToString(),
            EnableShabari = SLibCsvSerializer.ParseBool( row[ "Syabari" ].ToString() ),
            EnableHiuchi = SLibCsvSerializer.ParseBool( row[ "Hiuchi" ].ToString() ),
            FamilyPath = row[ "FamilyPath" ].ToString(),
            SymbolL = row[ "SymbolL" ].ToString(),
            SymbolR = row[ "SymbolR" ].ToString(),
          } ) ;
        }
      }

      return result.ToArray() ;
    } ) ;

    public static ClsSyabariPieceCSV[] Shared => _Shared.Value ;
    public static ClsSyabariPieceCSV[] SharedShabari => Shared.Where( x => x.EnableShabari ).ToArray() ;
    public static ClsSyabariPieceCSV[] SharedHiuchi => Shared.Where( x => x.EnableHiuchi ).ToArray() ;

    public static List<string> GetFamilyNameList()
    {
      return Shared.Select( x => ClsRevitUtil.GetFamilyName( x.FamilyPath ) ).ToList() ;
    }

    [Obsolete( "type と size から引き当てるメソッドを使用してください" )]
    public static string GetFamilyPath( string aa )
    {
      return null ;
    }

    public static string GetFamilyPath( string type, string size )
    {
      return Shared.FirstOrDefault( x => x.TypeName == type && x.SizeName == size )?.FamilyPath ?? "" ;
    }

    [Obsolete( "GetSizeListSyabari か GetSizeListHiuchi を使用してください" )]
    public static List<string> GetSizeList( string typeName )
    {
      return Shared.Where( x => x.TypeName == typeName ).Select( x => x.SizeName ).ToList() ;
    }

    public static List<string> GetSizeListSyabari( string typeName )
    {
      return SharedShabari.Where( x => x.TypeName == typeName ).Select( x => x.SizeName ).ToList() ;
    }

    public static List<string> GetSizeListHiuchi( string typeName )
    {
      return SharedHiuchi.Where( x => x.TypeName == typeName ).Select( x => x.SizeName ).ToList() ;
    }

    [Obsolete( "GetTypeListSyabari か GetTypeListHiuchi を使用してください" )]
    public static List<string> GetTypeList()
    {
      return Shared.GroupBy( x => x.TypeName ).Select( x => x.Key ).ToList() ;
    }

    public static List<string> GetTypeListSyabari()
    {
      return SharedShabari.GroupBy( x => x.TypeName ).Select( x => x.Key ).ToList() ;
    }

    public static List<string> GetTypeListHiuchi()
    {
      return SharedHiuchi.GroupBy( x => x.TypeName ).Select( x => x.Key ).ToList() ;
    }
  }
}