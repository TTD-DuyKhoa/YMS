using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;

namespace YMS_gantry.Master
{
  class ClsTesuriCsv : ClsMasterCsv
  {
    #region メンバ変数

    /// <summary>
    /// CSV名称
    /// </summary>
    private const string CsvFileName = "Tesuri.csv" ;

    /// <summary>
    /// メンバ変数
    /// </summary>
    private static List<ClsTesuriCsv> m_Data = new List<ClsTesuriCsv>() ;

    /// <summary>
    /// マスタ名称
    /// </summary>
    public const string MasterName = "手摺" ;

    /// <summary>
    /// 種類
    /// </summary>
    public const string TypeTesuri = "手摺" ;

    public const string TypeTesuriShichu = "手摺支柱" ;

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
      List<ClsTesuriCsv> lstCls = new List<ClsTesuriCsv>() ;
      foreach ( List<string> lstStr in lstlstStr ) {
        if ( bHeader ) {
          bHeader = false ;
          continue ;
        }

        ClsTesuriCsv cls = new ClsTesuriCsv() ;
        cls.Type = lstStr[ 0 ] ;
        cls.Size = lstStr[ 1 ] ;
        cls.FamilyPath = lstStr[ 2 ] ;
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
    public static List<ClsTesuriCsv> GetCsvData()
    {
      GetCsv() ;
      return m_Data ;
    }

    /// <summary>
    /// 種別をキーにサイズのリストを取得
    /// </summary>
    /// <returns></returns>
    public static List<string> GetSizeList( string type )
    {
      GetCsv() ;
      List<string> lst = new List<string>() ;
      if ( ( from data in m_Data where data.Type == type select data.Size ).Any() ) {
        lst = ( from data in m_Data where data.Type == type select data.Size ).ToList() ;
      }

      //ファミリ存在チェック
      lst = CheckExist( lst ) ;
      return lst ;
    }

    /// <summary>
    /// サイズをキーにファミリのパスを取得
    /// </summary>
    /// <returns></returns>
    public static string GetFamilyPath( string size, string type )
    {
      GetCsv() ;
      string str = string.Empty ;
      if ( ( from data in m_Data where data.Size == size && data.Type == type select data.FamilyPath ).Any() ) {
        str = ( from data in m_Data where data.Size == size && data.Type == type select data.FamilyPath ).ToList()
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