using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;

namespace YMS_gantry.Master
{
  class ClsJifukuZuredomeCsv : ClsMasterCsv
  {
    #region メンバ変数

    /// <summary>
    /// CSV名称
    /// </summary>
    private const string CsvFileName = "JifukuZuredome.csv" ;

    /// <summary>
    /// メンバ変数
    /// </summary>
    private static List<ClsJifukuZuredomeCsv> m_Data = new List<ClsJifukuZuredomeCsv>() ;

    /// <summary>
    /// マスタ名称
    /// </summary>
    public const string MasterName = "地覆覆工板ｽﾞﾚ止ﾒ材" ;

    /// <summary>
    /// 配置種類
    /// </summary>
    public const string TypeJifuku = "地覆" ;

    public const string TypeZuredomezai = "覆工板ｽﾞﾚ止め" ;

    /// <summary>
    /// 配置種類
    /// </summary>
    public string PutType { get ; set ; }

    #endregion

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
      List<ClsJifukuZuredomeCsv> lstCls = new List<ClsJifukuZuredomeCsv>() ;
      foreach ( List<string> lstStr in lstlstStr ) {
        if ( bHeader ) {
          bHeader = false ;
          continue ;
        }

        ClsJifukuZuredomeCsv cls = new ClsJifukuZuredomeCsv() ;
        cls.PutType = lstStr[ 0 ] ;
        cls.Type = lstStr[ 1 ] ;
        cls.Size = lstStr[ 2 ] ;
        cls.FamilyPath = lstStr[ 3 ] ;
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
    public static List<ClsJifukuZuredomeCsv> GetCsvData()
    {
      GetCsv() ;
      return m_Data ;
    }

    /// <summary>
    /// 配置種別、鋼材種類をキーにサイズのリストを取得
    /// </summary>
    /// <returns></returns>
    public static List<string> GetSizeList( string putType, string type )
    {
      GetCsv() ;
      List<string> lst = new List<string>() ;
      if ( ( from data in m_Data where data.PutType == putType && data.Type == type select data.Size ).Any() ) {
        lst = ( from data in m_Data where data.PutType == putType && data.Type == type select data.Size ).ToList() ;
      }

      //ファミリ存在チェック
      List<string> retList = new List<string>() ;
      foreach ( string s in lst ) {
        string p = GetFamilyPath( putType, s ) ;
        if ( System.IO.File.Exists( p ) ) {
          retList.Add( s ) ;
        }
      }

      return retList ;
    }

    /// <summary>
    /// サイズをキーにファミリのパスを取得
    /// </summary>
    /// <returns></returns>
    public static string GetFamilyPath( string putType, string size )
    {
      GetCsv() ;
      string str = string.Empty ;
      if ( ( from data in m_Data where data.PutType == putType && data.Size == size select data.FamilyPath ).Any() ) {
        str = ( from data in m_Data where data.PutType == putType && data.Size == size select data.FamilyPath ).ToList()
          .First() ;
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