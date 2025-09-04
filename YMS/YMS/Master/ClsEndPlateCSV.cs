using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;

namespace YMS.Master
{
  class ClsEndPlateCSV
  {
    #region メンバ変数

    /// <summary>
    /// CSV名称
    /// </summary>
    private const string CsvFileName = "EndPlate.csv" ;

    /// <summary>
    /// メンバ変数
    /// </summary>
    private static List<ClsEndPlateCSV> m_Data = new List<ClsEndPlateCSV>() ;

    #endregion

    #region プロパティ

    /// <summary>
    /// サイズ
    /// </summary>
    public string Size ;

    #endregion

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
      List<ClsEndPlateCSV> lstCls = new List<ClsEndPlateCSV>() ;
      foreach ( List<string> lstStr in lstlstStr ) {
        if ( bHeader ) {
          bHeader = false ;
          continue ;
        }

        ClsEndPlateCSV cls = new ClsEndPlateCSV() ;
        cls.Size = lstStr[ 0 ] ;
        lstCls.Add( cls ) ;
      }

      m_Data = lstCls ;

      return true ;
    }

    /// <summary>
    /// CSV情報を取得
    /// </summary>
    /// <returns></returns>
    public static List<ClsEndPlateCSV> GetCsvData()
    {
      GetCsv() ;
      return m_Data ;
    }

    /// <summary>
    /// サイズ名のリストを取得
    /// </summary>
    /// <returns></returns>
    public static List<string> GetSizeList()
    {
      GetCsv() ;
      List<string> lst = new List<string>() ;
      if ( ( from data in m_Data select data.Size ).Any() ) {
        lst = ( from data in m_Data select data.Size ).Distinct().ToList() ;
      }

      return lst ;
    }
  }
}