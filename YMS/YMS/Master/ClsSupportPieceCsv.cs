using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;

namespace YMS.Master
{
  class ClsSupportPieceCsv
  {
    #region メンバ変数

    /// <summary>
    /// CSV名称
    /// </summary>
    private const string CsvFileName = "SupportPiece.csv" ;

    #endregion

    #region メンバ変数

    /// <summary>
    /// メンバ変数
    /// </summary>
    private static List<ClsSupportPieceCsv> m_Data = new List<ClsSupportPieceCsv>() ;

    #endregion

    #region プロパティ

    /// <summary>
    /// サイズ
    /// </summary>
    public string Size ;

    /// <summary>
    /// ファミリパス
    /// </summary>
    public string FamilyPath ;

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
      List<ClsSupportPieceCsv> lstCls = new List<ClsSupportPieceCsv>() ;
      foreach ( List<string> lstStr in lstlstStr ) {
        if ( bHeader ) {
          bHeader = false ;
          continue ;
        }

        ClsSupportPieceCsv cls = new ClsSupportPieceCsv() ;
        cls.Size = lstStr[ 0 ] ;
        cls.FamilyPath = lstStr[ 1 ] ;
        lstCls.Add( cls ) ;
      }

      m_Data = lstCls ;

      return true ;
    }

    /// <summary>
    /// CSV情報を取得
    /// </summary>
    /// <returns></returns>
    public static List<ClsSupportPieceCsv> GetCsvData()
    {
      GetCsv() ;
      return m_Data ;
    }

    /// <summary>
    /// サイズのリストを取得
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
    /// ベースサイズと一致するサイズのリストを取得
    /// </summary>
    /// <param name="baseSize"></param>
    /// <param name="isSMH"></param>
    /// <returns></returns>
    public static List<string> GetSizeList( string baseSize, bool isSMH = false, bool isTwinBeam = false )
    {
      GetCsv() ;
      List<string> lst = new List<string>() ;

      string type ;
      if ( isSMH ) {
        type = "SM-" + baseSize.Replace( "HA", "D" ) + "-" ;
      }
      else if ( isTwinBeam ) {
        type = "SM-60D-" ;
      }
      else {
        type = baseSize.Replace( "HA", "D" ) + "-" ;
      }

      // baseSize に一致し、かつ指定された baseSize で始まる部材のみを抽出
      lst = ( from data in m_Data where data.Size.StartsWith( type ) select data.Size ).ToList() ;

      return lst ;
    }


    /// <summary>
    /// ファミリの名前Listを取得
    /// </summary>
    /// <returns></returns>
    public static List<string> GetFamilyNameList()
    {
      GetCsv() ;
      List<string> kariNameList = new List<string>() ;
      foreach ( var data in m_Data ) {
        string filePath = data.FamilyPath ;
        string familyName = ClsRevitUtil.GetFamilyName( filePath ) ;
        kariNameList.Add( familyName ) ;
      }

      return kariNameList ;
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

      string symbolFolpath = Parts.ClsZumenInfo.GetYMSFolder() ;
      string filePath = System.IO.Path.Combine( symbolFolpath, str ) ;
      return filePath ;
    }

    /// <summary>
    /// 通常補助ピースサイズを返す
    /// </summary>
    /// <param name="syuzai">対象主材</param>
    /// <param name="length">長さ</param>
    /// <returns></returns>
    public static string GetSize( string syuzai, int length )
    {
      if ( syuzai.Contains( "SMH" ) ) {
        string syuzaiSize = syuzai.Replace( "SMH", "" ) ;
        if ( syuzaiSize == "80" )
          return "40" + "D" + "-" + length.ToString() ;
        else
          return "SM-" + syuzaiSize + "D" + "-" + length.ToString() ;
      }
      else {
        string syuzaiSize = syuzai.Replace( "HA", "" ) ;
        return syuzaiSize + "D" + "-" + length.ToString() ;
      }
    }

    #endregion
  }
}