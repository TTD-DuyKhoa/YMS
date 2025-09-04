using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;

namespace YMS_gantry.Master
{
  class ClsTopPlateCsv : ClsMasterCsv
  {
    #region メンバ変数

    /// <summary>
    /// CSV名称
    /// </summary>
    private const string CsvFileName = "TopPlate.csv" ;

    /// <summary>
    /// メンバ変数
    /// </summary>
    private static List<ClsTopPlateCsv> m_Data = new List<ClsTopPlateCsv>() ;

    /// <summary>
    /// マスタ名称
    /// </summary>
    public const string MasterName = "トッププレート" ;

    /// <summary>
    /// フリーサイズ
    /// </summary>
    public const string FreeSize = "任意" ;

    #endregion

    public double T { get ; set ; }
    public double A { get ; set ; }
    public double B { get ; set ; }

    #region メソッド

    /// <summary>
    /// CSVから情報を取得
    /// </summary>
    /// <returns></returns>
    public static bool GetCsv()
    {
      if ( m_Data != null && m_Data.Count != 0 ) {
        return true ;
      }

      string symbolFolpath = PathUtil.GetYMS_GantryMasterPath() ;
      string fileName = System.IO.Path.Combine( symbolFolpath, CsvFileName ) ;
      List<List<string>> lstlstStr = new List<List<string>>() ;
      if ( ! RevitUtil.ClsCommonUtils.ReadCsv( fileName, ref lstlstStr ) ) {
        MessageBox.Show( "CSVファイルの取得に失敗しました。：" + fileName ) ;
        return false ;
      }

      bool bHeader = true ;
      List<ClsTopPlateCsv> lstCls = new List<ClsTopPlateCsv>() ;
      foreach ( List<string> lstStr in lstlstStr ) {
        if ( bHeader ) {
          bHeader = false ;
          continue ;
        }

        var k = 0 ;
        ClsTopPlateCsv cls = new ClsTopPlateCsv
        {
          Type = lstStr[ k++ ],
          Size = lstStr[ k++ ],
          FamilyPath = lstStr[ k++ ],
          T = Atof( lstStr.ElementAtOrDefault( k++ ) ),
          A = Atof( lstStr.ElementAtOrDefault( k++ ) ),
          B = Atof( lstStr.ElementAtOrDefault( k++ ) ),
        } ;
        lstCls.Add( cls ) ;
      }

      m_Data = lstCls ;

      return true ;
    }

    /// <summary>
    /// CSV情報を取得
    /// </summary>
    /// <returns></returns>
    [YmsMasterLoad]
    public static List<ClsTopPlateCsv> GetCsvData()
    {
      GetCsv() ;
      return m_Data ;
    }

    /// <summary>
    /// 種別をキーにサイズのリストを取得
    /// </summary>
    /// <returns></returns>
    public static List<string> GetSizeList()
    {
      GetCsv() ;
      List<string> lst = new List<string>() ;
      if ( ( from data in m_Data select data.Size ).Any() ) {
        lst = ( from data in m_Data select data.Size ).ToList() ;
      }

      return lst ;
    }

    /// <summary>
    /// サイズをキーにファミリのパスを取得
    /// </summary>
    /// <returns></returns>
    public static string GetFamilyPath( string size )
    {
      GetCsv() ;
      string str = string.Empty ;
      if ( ( from data in m_Data where data.Size == size select data.FamilyPath ).Any() ) {
        str = ( from data in m_Data where data.Size == size select data.FamilyPath ).ToList().First() ;
      }

      string symbolFolpath = PathUtil.GetYMSFolder() ;
      string filePath = System.IO.Path.Combine( symbolFolpath, str ) ;

      if ( ! System.IO.File.Exists( filePath ) ) {
        //MessageBox.Show("ファミリが存在しません。\n" + filePath);
        filePath = string.Empty ;
      }

      return filePath ;
    }

    #endregion
  }
}