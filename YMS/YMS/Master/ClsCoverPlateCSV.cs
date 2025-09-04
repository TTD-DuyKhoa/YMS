using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;

namespace YMS.Master
{
  class ClsCoverPlateCSV
  {
    /// <summary>
    /// CSV名称
    /// </summary>
    private const string CsvFileName = "CoverPlate.csv" ;

    /// <summary>
    /// メンバ変数
    /// </summary>
    private static List<ClsCoverPlateCSV> m_Data = new List<ClsCoverPlateCSV>() ;

    /// <summary>
    /// 種類
    /// </summary>
    public string Type ;

    /// <summary>
    /// サイズ
    /// </summary>
    public string Size ;

    /// <summary>
    /// 厚さ
    /// </summary>
    public string Thickness ;

    /// <summary>
    /// ファミリパス
    /// </summary>
    public string FamilyPath ;

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
      List<ClsCoverPlateCSV> lstCls = new List<ClsCoverPlateCSV>() ;
      foreach ( List<string> lstStr in lstlstStr ) {
        if ( bHeader ) {
          bHeader = false ;
          continue ;
        }

        ClsCoverPlateCSV cls = new ClsCoverPlateCSV() ;
        cls.Type = lstStr[ 0 ] ;
        cls.Size = lstStr[ 1 ] ;
        cls.Thickness = lstStr[ 2 ] ;
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
    public static List<ClsCoverPlateCSV> GetCsvData()
    {
      GetCsv() ;
      return m_Data ;
    }

    /// <summary>
    /// サイズからファミリのパスを取得
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public static string GetFamilyPath( string size )
    {
      // CSVデータを取得
      GetCsv() ;

      // サイズが見つからない場合は空のパスを返す
      string filePath = string.Empty ;

      // サイズをキーにしてファミリのパスを取得
      var query = from data in m_Data where data.Size == size select data.FamilyPath ;

      // クエリの結果があればパスを取得
      if ( query.Any() ) {
        string str = query.First() ;
        string symbolFolpath = Parts.ClsZumenInfo.GetYMSFolder() ;
        filePath = System.IO.Path.Combine( symbolFolpath, str ) ;
      }

      return filePath ;
    }

    /// <summary>
    /// サイズから厚さを取得
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public static string GetThickness( string size )
    {
      GetCsv() ;
      string str = string.Empty ;
      if ( ( from data in m_Data where data.Size == size select data.Thickness ).Any() ) {
        str = ( from data in m_Data where data.Size == size select data.Thickness ).FirstOrDefault() ;
      }

      return str ;
    }

    /// <summary>
    /// ファイル名リストを取得
    /// </summary>
    /// <returns></returns>
    public static List<string> GetFamilyNameList()
    {
      GetCsv() ;
      List<string> nameList = new List<string>() ;
      foreach ( var data in m_Data ) {
        string filePath = data.FamilyPath ;
        string familyName = RevitUtil.ClsRevitUtil.GetFamilyName( filePath ) ;
        nameList.Add( familyName ) ;
      }

      return nameList ;
    }
  }
}