using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;

namespace YMS.Master
{
  class ClsTopPlateCSV
  {
    #region メンバ変数

    /// <summary>
    /// CSV名称
    /// </summary>
    private const string CsvFileName = "TopPlate.csv" ;

    /// <summary>
    /// メンバ変数
    /// </summary>
    private static List<ClsTopPlateCSV> m_Data = new List<ClsTopPlateCSV>() ;

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

    /// <summary>
    /// タイプ名
    /// </summary>
    public string TypeName ;

    /// <summary>
    /// サイズ(W)
    /// </summary>
    public string Size_W ;

    /// <summary>
    /// サイズ(D)
    /// </summary>
    public string Size_D ;

    /// <summary>
    /// サイズ(t)
    /// </summary>
    public string Size_t ;

    /// <summary>
    /// ファミリパス
    /// </summary>
    public string FamilyPath ; // 現状ではトッププレートのファミリは単体で存在しない

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
      List<ClsTopPlateCSV> lstCls = new List<ClsTopPlateCSV>() ;
      foreach ( List<string> lstStr in lstlstStr ) {
        if ( bHeader ) {
          bHeader = false ;
          continue ;
        }

        ClsTopPlateCSV cls = new ClsTopPlateCSV() ;
        cls.Type = lstStr[ 0 ] ;
        cls.Size = lstStr[ 1 ] ;
        cls.TypeName = lstStr[ 2 ] ;
        cls.Size_W = lstStr[ 3 ] ;
        cls.Size_D = lstStr[ 4 ] ;
        cls.Size_t = lstStr[ 5 ] ;
        cls.FamilyPath = lstStr[ 6 ] ;
        lstCls.Add( cls ) ;
      }

      m_Data = lstCls ;

      return true ;
    }

    /// <summary>
    /// CSV情報を取得
    /// </summary>
    /// <returns></returns>
    public static List<ClsTopPlateCSV> GetCsvData()
    {
      GetCsv() ;
      return m_Data ;
    }

    /// <summary>
    /// タイプ名のリストを取得
    /// </summary>
    /// <returns></returns>
    public static List<string> GetTypeNameList()
    {
      GetCsv() ;
      List<string> lst = new List<string>() ;
      if ( ( from data in m_Data select data.TypeName ).Any() ) {
        lst = ( from data in m_Data select data.TypeName ).Distinct().ToList() ;
      }

      return lst ;
    }
  }
}