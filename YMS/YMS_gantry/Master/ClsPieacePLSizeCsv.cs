using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;

namespace YMS_gantry.Master
{
  class ClsPieacePLSizeCsv : ClsMasterCsv
  {
    #region メンバ変数

    /// <summary>
    /// CSV名称
    /// </summary>
    private const string CsvFileName = "PieaceEndPLSize.csv" ;

    /// <summary>
    /// メンバ変数
    /// </summary>
    private static List<ClsPieacePLSizeCsv> m_Data = new List<ClsPieacePLSizeCsv>() ;

    /// <summary>
    /// マスタ名称
    /// </summary>
    public const string MasterName = "ピースエンドプレート" ;

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
      List<ClsPieacePLSizeCsv> lstCls = new List<ClsPieacePLSizeCsv>() ;
      foreach ( List<string> lstStr in lstlstStr ) {
        if ( bHeader ) {
          bHeader = false ;
          continue ;
        }

        ClsPieacePLSizeCsv cls = new ClsPieacePLSizeCsv() ;
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
    [YmsMasterLoad]
    public static List<ClsPieacePLSizeCsv> GetCsvData()
    {
      GetCsv() ;
      return m_Data ;
    }

    /// <summary>
    /// 種別のリストを取得
    /// </summary>
    /// <returns></returns>
    public static List<string> GetTypeList()
    {
      GetCsv() ;
      List<string> lst = new List<string>() ;
      if ( ( from data in m_Data select data.Type ).Any() ) {
        lst = ( from data in m_Data select data.Type ).ToList() ;
      }

      return lst ;
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

      return lst ;
    }

    #endregion
  }
}