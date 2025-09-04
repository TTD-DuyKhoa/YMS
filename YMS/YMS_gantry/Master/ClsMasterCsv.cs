using System ;
using System.Collections.Generic ;
using System.IO ;
using System.Linq ;
using System.Reflection ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;
using YMS_gantry.UI.FrnCreateSlopeControls ;

namespace YMS_gantry.Master
{
  public class ClsMasterCsv
  {
    #region メンバ変数

    /// <summary>
    /// CSV名称
    /// </summary>
    private const string CsvFileName = "csvName" ;


    /// <summary>
    /// 種類
    /// </summary>
    public const string TypeH = "H鋼" ;

    public const string TypeC = "チャンネル" ;
    public const string TypeL = "アングル" ;
    public const string TypeYamadome = "山留主材" ;
    public const string TypeHighTenYamadome = "高強度山留主材" ;

    /// <summary>
    /// 火打ち受けピースの中心部長
    /// </summary>
    public double HiuchiLengh { get ; set ; }

    #endregion

    #region プロパティ

    /// <summary>
    /// 種類
    /// </summary>
    public string Type { get ; set ; }

    /// <summary>
    /// サイズ
    /// </summary>
    public string Size { get ; set ; }

    /// <summary>
    /// ファミリパス
    /// </summary>
    public string FamilyPath { get ; set ; }

    public string FamilyName
    {
      get
      {
        try {
          return System.IO.Path.GetFileNameWithoutExtension( FamilyPath ) ;
        }
        catch {
          return "" ;
        }
      }
    }

    /// <summary>
    /// 水平つなぎ対応可否
    /// </summary>
    public string HrzJoint { get ; set ; }

    public bool IsHrzJoint => ! string.IsNullOrWhiteSpace( HrzJoint ) ;

    public bool IsBrace { get ; set ; } = false ;

    public string MadumeSymbol { get ; set ; }

    public static ClsMasterCsv[] Shared { get ; }

    #endregion

    #region　メソッド

    /// <summary>
    /// ファミリのパスを取得
    /// </summary>
    /// <param name="size">ファミリ名</param>
    /// <param name="dLen">長さパラメータ</param>
    /// <returns></returns>
    public static string GetFamilyPath( string size, double dLen = 0.0, bool bPile = false, bool bPilePiller = false,
      bool bExPile = false )
    {
      string strPath = string.Empty ;

      if ( bPile ) {
        strPath = ClsHBeamCsv.GetPileFamilyPath( size ) ;
        if ( ! string.IsNullOrEmpty( strPath ) ) return strPath ;
      }
      else if ( bExPile ) {
        strPath = ClsHBeamCsv.GetExPileFamilyPath( size ) ;
        if ( ! string.IsNullOrEmpty( strPath ) ) return strPath ;
      }
      else if ( bPilePiller ) {
        strPath = ClsSichuCsv.GetFamilyPath( size ) ;
        if ( ! string.IsNullOrEmpty( strPath ) ) return strPath ;
      }
      else {
        strPath = ClsAngleCsv.GetFamilyPath( size ) ;
        if ( ! string.IsNullOrEmpty( strPath ) ) return strPath ;
        strPath = ClsChannelCsv.GetFamilyPath( size ) ;
        if ( ! string.IsNullOrEmpty( strPath ) ) return strPath ;
        strPath = ClsHBeamCsv.GetFamilyPath( size ) ;
        if ( ! string.IsNullOrEmpty( strPath ) ) return strPath ;

        strPath = ClsTopPlateCsv.GetFamilyPath( size ) ;
        if ( ! string.IsNullOrEmpty( strPath ) ) return strPath ;
        strPath = ClsYamadomeCsv.GetFamilyPath( size, dLen ) ;
        if ( ! string.IsNullOrEmpty( strPath ) ) return strPath ;
        strPath = ClsFukkoubanCsv.GetFamilyPath( size ) ;
        if ( ! string.IsNullOrEmpty( strPath ) ) return strPath ;
        strPath = ClsKoubanzaiCsv.GetFamilyPath( size ) ;
        if ( ! string.IsNullOrEmpty( strPath ) ) return strPath ;
        strPath = ClsTakasaChouseiCsv.GetFamilyPath( size ) ;
        if ( ! string.IsNullOrEmpty( strPath ) ) return strPath ;
        strPath = ClsFukkouGetaCsv.GetFamilyPath( size ) ;
        if ( ! string.IsNullOrEmpty( strPath ) ) return strPath ;
        strPath = ClsTaikeikouCsv.GetFamilyPath( size ) ;
        if ( ! string.IsNullOrEmpty( strPath ) ) return strPath ;
        strPath = ClsTaikeikouPLCsv.GetFamilyPath( size ) ;
        if ( ! string.IsNullOrEmpty( strPath ) ) return strPath ;
        strPath = ClsHoudueCsv.GetFamilyPath( size ) ;
        if ( ! string.IsNullOrEmpty( strPath ) ) return strPath ;
        strPath = ClsTeiketsuHojoCsv.GetFamilyPath( size ) ;
        if ( ! string.IsNullOrEmpty( strPath ) ) return strPath ;
        strPath = ClsTurnBackleCsv.GetFamilyPath( size ) ;
        if ( ! string.IsNullOrEmpty( strPath ) ) return strPath ;
      }

      return strPath ;
    }

    public static string GetFamilyTypeBySize( string size )
    {
      string strType = string.Empty ;
      strType = ClsHoudueCsv.GetFamilyType( size ) ;
      if ( ! string.IsNullOrEmpty( strType ) ) return strType ;
      strType = ClsStiffenerCsv.GetFamilyType( size ) ;
      if ( ! string.IsNullOrEmpty( strType ) ) return strType ;

      return strType ;
    }

    public static string GetFamilyPathBySize( string size, string type )
    {
      string strPath = string.Empty ;
      strPath = ClsFukkoubanCsv.GetFamilyPath( size ) ;
      if ( ! string.IsNullOrEmpty( strPath ) ) return strPath ;
      strPath = ClsHoudueCsv.GetFamilyPath( size, type ) ;
      if ( ! string.IsNullOrEmpty( strPath ) ) return strPath ;
      strPath = ClsTesuriCsv.GetFamilyPath( size, type ) ;
      if ( ! string.IsNullOrEmpty( strPath ) ) return strPath ;

      return strPath ;
    }


    public static List<string> CheckExist( List<string> lst, double dLen = 0.0, bool bPile = false )
    {
      List<string> lstNew = new List<string>() ;
      foreach ( string str in lst ) {
        string strPath = GetFamilyPath( str, dLen, bPile ) ;

        if ( System.IO.File.Exists( strPath ) ) {
          lstNew.Add( str ) ;
        }
      }

      return lstNew ;
    }

    protected static double Atof( string str )
    {
      if ( double.TryParse( str, out double d ) ) return d ;
      return 0.0 ;
    }

    protected static bool Atob( string str )
    {
      return str?.ToLower() == "true" ;
    }

    static ClsMasterCsv()
    {
      #if DEBUG
      #else
            try
      #endif
      {
        using ( var defer = new Defer(
                 () => System.Windows.Forms.Application.EnterThreadModal += Application_EnterThreadModal,
                 () => System.Windows.Forms.Application.EnterThreadModal -= Application_EnterThreadModal ) ) {
          var sharedValue = new List<ClsMasterCsv>() ;

          var recordClasses = Assembly.GetExecutingAssembly().GetTypes()
            .Where( x => x != null && x.IsClass && x.IsSubclassOf( typeof( ClsMasterCsv ) ) ).ToArray() ;

          foreach ( var recordClass in recordClasses ) {
            var loadMethod = recordClass.GetMethods()
              .FirstOrDefault( x => x?.GetCustomAttribute<YmsMasterLoadAttribute>() != null ) ;
            var records = loadMethod?.Invoke( null, null ) as IEnumerable<ClsMasterCsv> ;
            if ( records == null ) {
              continue ;
            }

            sharedValue.AddRange( records ) ;
          }

          Shared = sharedValue.ToArray() ;
        }
      }
      #if DEBUG
      #else
            catch (Exception ex)
            {
                Shared = new ClsMasterCsv[0];
            }
      #endif
      //.Select(x => new { Type = x, Attributes = x.GetCustomAttributes<AttributeType>() })
      //.Where(x => x.Attributes.Any())
      //.SelectMany(x => x.Attributes.Select(y => new ClassNamePair { Name = y?.Name, ClassType = x.Type }))
      //.Where(x => !string.IsNullOrEmpty(x.Name))
      //.ToArray();
    }

    private static void Application_EnterThreadModal( object sender, EventArgs e )
    {
    }

    #endregion
  }
}