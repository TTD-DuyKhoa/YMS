using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;

namespace YMS.Master
{
  class ClsDanmenHenkaKuiCsv
  {
    #region メンバ変数

    /// <summary>
    /// CSV名称
    /// </summary>
    private const string CsvFileName = "DanmenHenkaKui.csv" ;

    /// <summary>
    /// メンバ変数
    /// </summary>
    private static List<ClsDanmenHenkaKuiCsv> m_Data = new List<ClsDanmenHenkaKuiCsv>() ;

    #endregion

    #region プロパティ

    /// <summary>
    /// サイズ
    /// </summary>
    public string Size ;

    /// <summary>
    /// サイズ(構台杭)
    /// </summary>
    public string Size_Koudai ;

    /// <summary>
    /// サイズ(断面変化杭)
    /// </summary>
    public string Size_DanmenHenka ;

    /// <summary>
    /// ファミリパス
    /// </summary>
    public string FamilyPath ;

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
      List<ClsDanmenHenkaKuiCsv> lstCls = new List<ClsDanmenHenkaKuiCsv>() ;
      foreach ( List<string> lstStr in lstlstStr ) {
        if ( bHeader ) {
          bHeader = false ;
          continue ;
        }

        ClsDanmenHenkaKuiCsv cls = new ClsDanmenHenkaKuiCsv() ;
        cls.Size = lstStr[ 0 ] ;
        cls.Size_Koudai = lstStr[ 1 ] ;
        cls.Size_DanmenHenka = lstStr[ 2 ] ;
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
    public static List<ClsDanmenHenkaKuiCsv> GetCsvData()
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
        lst = ( from data in m_Data select data.Size ).Distinct().ToList() ;
      }

      return lst ;
    }

    /// <summary>
    /// サイズ1(構台杭)のリストを取得
    /// </summary>
    /// <returns></returns>
    public static List<string> GetSizeList_Koudai()
    {
      GetCsv() ;
      List<string> lst = new List<string>() ;
      if ( ( from data in m_Data select data.Size_Koudai ).Any() ) {
        lst = ( from data in m_Data select data.Size_Koudai ).Distinct().ToList() ;
      }

      return lst ;
    }

    /// <summary>
    /// サイズ2(断面変化杭)のリストを取得
    /// </summary>
    /// <returns></returns>
    public static List<string> GetSizeList_DanmenHenka()
    {
      GetCsv() ;
      List<string> lst = new List<string>() ;
      if ( ( from data in m_Data select data.Size_DanmenHenka ).Any() ) {
        lst = ( from data in m_Data select data.Size_DanmenHenka ).Distinct().ToList() ;
      }

      return lst ;
    }

    /// <summary>
    /// サイズ1(構台杭)をキーにサイズ2(断面変化杭)を取得
    /// </summary>
    /// <returns></returns>
    public static string GetSize2( string size )
    {
      GetCsv() ;
      string str = string.Empty ;
      if ( ( from data in m_Data where data.Size_Koudai == size select data.Size_DanmenHenka ).Any() ) {
        str = ( from data in m_Data where data.Size_Koudai == size select data.Size_DanmenHenka ).ToList().First() ;
      }

      return str ;
    }

    /// <summary>
    /// サイズ1(構台杭)をキーにファミリのパスを取得
    /// </summary>
    /// <returns></returns>
    public static string GetFamilyPath( string size )
    {
      GetCsv() ;
      string str = string.Empty ;
      if ( ( from data in m_Data where data.Size_Koudai == size select data.FamilyPath ).Any() ) {
        str = ( from data in m_Data where data.Size_Koudai == size select data.FamilyPath ).ToList().First() ;
      }

      string symbolFolpath = Parts.ClsZumenInfo.GetYMSFolder() ;
      string filePath = System.IO.Path.Combine( symbolFolpath, str ) ;
      return filePath ;
    }
  }
}