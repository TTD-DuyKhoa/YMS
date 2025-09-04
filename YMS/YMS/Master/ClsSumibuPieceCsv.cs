using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;

namespace YMS.Master
{
  public class ClsSumibuPieceCsv
  {
    #region メンバ変数

    /// <summary>
    /// CSV名称
    /// </summary>
    private const string CsvFileName = "CornerPiece.csv" ;

    #endregion

    #region メンバ変数

    /// <summary>
    /// メンバ変数
    /// </summary>
    private static List<ClsSumibuPieceCsv> m_Data = new List<ClsSumibuPieceCsv>() ;

    #endregion

    #region プロパティ

    /// <summary>
    /// タイプ
    /// </summary>
    public string Type ;

    /// <summary>
    /// サイズ
    /// </summary>
    public string Size ;

    /// <summary>
    /// 対象主材サイズ
    /// </summary>
    public string TargetShuzaiSize ;

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
      List<ClsSumibuPieceCsv> lstCls = new List<ClsSumibuPieceCsv>() ;
      foreach ( List<string> lstStr in lstlstStr ) {
        if ( bHeader ) {
          bHeader = false ;
          continue ;
        }

        ClsSumibuPieceCsv cls = new ClsSumibuPieceCsv() ;
        cls.Type = lstStr[ 0 ] ;
        cls.Size = lstStr[ 1 ] ;
        cls.TargetShuzaiSize = lstStr[ 2 ] ;
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
    public static List<ClsSumibuPieceCsv> GetCsvData()
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
        lst = ( from data in m_Data select data.Type ).Distinct().ToList() ;
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
        lst = ( from data in m_Data where data.Type == type select data.Size ).Distinct().ToList() ;
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

      string symbolFolpath = Parts.ClsZumenInfo.GetYMSFolder() ;
      string filePath = System.IO.Path.Combine( symbolFolpath, str ) ;
      return filePath ;
    }

    /// <summary>
    /// タイプと主材サイズをキーにサイズを取得
    /// </summary>
    /// <returns></returns>
    public static string GetSizeName( string type, string shuzaiSize )
    {
      GetCsv() ;
      string str = string.Empty ;
      if ( ( from data in m_Data where data.Type == type && data.TargetShuzaiSize == shuzaiSize select data.Size )
          .Any() ) {
        str = ( from data in m_Data where data.Type == type && data.TargetShuzaiSize == shuzaiSize select data.Size )
          .ToList().First() ;
      }

      return str ;
    }

    /// <summary>
    /// タイプと主材サイズをキーにファミリのパスを取得
    /// </summary>
    /// <returns></returns>
    public static string GetFamilyPath( string type, string shuzaiSize )
    {
      GetCsv() ;
      string str = string.Empty ;
      if ( ( from data in m_Data where data.Type == type && data.TargetShuzaiSize == shuzaiSize select data.FamilyPath )
          .Any() ) {
        str = ( from data in m_Data
          where data.Type == type && data.TargetShuzaiSize == shuzaiSize
          select data.FamilyPath ).ToList().First() ;
      }

      string symbolFolpath = Parts.ClsZumenInfo.GetYMSFolder() ;
      string filePath = System.IO.Path.Combine( symbolFolpath, str ) ;
      return filePath ;
    }

    /// <summary>
    /// コーナーピースのサイズから主材のサイズを取得
    /// </summary>
    /// <param name="pieceSize"></param>
    /// <returns></returns>
    public static string GetShuzaiSize( string pieceSize )
    {
      string res = string.Empty ;
      GetCsv() ;
      foreach ( var data in m_Data ) {
        if ( data.Size == pieceSize ) {
          res = data.TargetShuzaiSize ;
        }
      }

      return res ;
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

    #endregion
  }
}