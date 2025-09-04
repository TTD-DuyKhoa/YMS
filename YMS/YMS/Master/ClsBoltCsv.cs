using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;

namespace YMS.Master
{
  class ClsBoltCsv
  {
    #region メンバ変数

    /// <summary>
    /// CSV名称
    /// </summary>
    private const string CsvFileName = "Bolt.csv" ;

    /// <summary>
    /// メンバ変数
    /// </summary>
    private static List<ClsBoltCsv> m_Data = new List<ClsBoltCsv>() ;

    /// <summary>
    /// ボルトタイプ
    /// </summary>
    public const string BoltTypeN = "普通ボルト" ;

    public const string BoltTypeH = "ハイテンションボルト" ;
    public const string BoltTypeT = "トルシア型ボルト" ;
    public static List<string> BoltTypes = new List<string>() { BoltTypeN, BoltTypeH, BoltTypeT } ;

    #endregion

    #region プロパティ

    /// <summary>
    /// 種類
    /// </summary>
    public string Type ;

    /// <summary>
    /// サイズ
    /// </summary>
    public string Size ;

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

      string symbolFolpath = Parts.ClsZumenInfo.GetYMSFolder() ;
      string fileName = System.IO.Path.Combine( symbolFolpath, Parts.ClsZumenInfo.master, CsvFileName ) ;
      List<List<string>> lstlstStr = new List<List<string>>() ;
      if ( ! RevitUtil.ClsCommonUtils.ReadCsv( fileName, ref lstlstStr ) ) {
        MessageBox.Show( "CSVファイルの取得に失敗しました。：" + fileName ) ;
        return false ;
      }

      bool bHeader = true ;
      List<ClsBoltCsv> lstCls = new List<ClsBoltCsv>() ;
      foreach ( List<string> lstStr in lstlstStr ) {
        if ( bHeader ) {
          bHeader = false ;
          continue ;
        }

        ClsBoltCsv cls = new ClsBoltCsv() ;
        cls.Type = lstStr[ 0 ] ;
        cls.Size = lstStr[ 1 ] ;
        lstCls.Add( cls ) ;
      }

      m_Data = lstCls ;

      return true ;
    }

    /// <summary>
    /// CSV情報を取得
    /// </summary>
    /// <returns></returns>
    public static List<ClsBoltCsv> GetCsvData()
    {
      GetCsv() ;
      return m_Data ;
    }

    /// <summary>
    /// 種類のリストを取得
    /// </summary>
    /// <returns></returns>
    public static List<string> GetTypeList()
    {
      GetCsv() ;
      List<string> lst = new List<string>() ;
      if ( ( from data in m_Data select data.Type ).Any() ) {
        lst = ( from data in m_Data select data.Type ).Distinct().ToList() ;
      }

      return lst ;
    }

    /// <summary>
    /// 種類をキーに名称のリストを取得
    /// </summary>
    /// <returns></returns>
    public static List<string> GetSizeList( string type )
    {
      GetCsv() ;
      List<string> lst = new List<string>() ;
      if ( ( from data in m_Data where data.Type == type select data.Size ).Any() ) {
        lst = ( from data in m_Data where data.Type == type select data.Size ).ToList() ;
      }

      return lst ;
    }

    /// <summary>
    /// 名称をキーに種類を取得
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public static string GetType( string size )
    {
      if ( size == null || size == "" ) {
        return "" ;
      }

      GetCsv() ;
      List<string> lst = new List<string>() ;
      if ( ( from data in m_Data where data.Size == size select data.Type ).Any() ) {
        lst = ( from data in m_Data where data.Size == size select data.Type ).ToList() ;
      }

      if ( lst.Count == 0 ) {
        return "" ;
      }

      return lst.First() ;
    }

    #endregion
  }
}