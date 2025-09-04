using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;

namespace YMS.Master
{
  public class ClsKouyaitaTsugiteCsv
  {
    #region メンバ変数

    /// <summary>
    /// CSV名称
    /// </summary>
    private const string CsvFileName = "KouyaitaTsugite.csv" ;

    #endregion

    #region メンバ変数

    /// <summary>
    /// メンバ変数
    /// </summary>
    private static List<ClsKouyaitaTsugiteCsv> m_Data = new List<ClsKouyaitaTsugiteCsv>() ;

    #endregion

    #region プロパティ

    /// <summary>
    /// 固定方法
    /// </summary>
    public string KoteiHoho ;

    /// <summary>
    /// タイプ
    /// </summary>
    public string Type ;

    /// <summary>
    /// サイズ
    /// </summary>
    public string Size ;

    /// <summary>
    /// プレートサイズ（フランジ側）
    /// </summary>
    public string PlateSizeF ;

    /// <summary>
    /// プレート枚数（フランジ側）
    /// </summary>
    public int PlateNumF ;

    /// <summary>
    /// ボルトサイズ（フランジ側）
    /// </summary>
    public string BoltSizeF ;

    /// <summary>
    /// ボルト個数（フランジ側）
    /// </summary>
    public int BoltNumF ;

    /// <summary>
    /// プレートサイズ（WEB側）
    /// </summary>
    public string PlateSizeW ;

    /// <summary>
    /// プレート枚数（WEB側）
    /// </summary>
    public int PlateNumW ;

    /// <summary>
    /// プレートサイズ（WEB側2）
    /// </summary>
    public string PlateSizeW2 ;

    /// <summary>
    /// プレート枚数（WEB側2）
    /// </summary>
    public int PlateNumW2 ;

    /// <summary>
    /// ボルトサイズ（WEB側）
    /// </summary>
    public string BoltSizeW ;

    /// <summary>
    /// ボルト個数（WEB側）
    /// </summary>
    public int BoltNumW ;

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
      List<ClsKouyaitaTsugiteCsv> lstCls = new List<ClsKouyaitaTsugiteCsv>() ;
      foreach ( List<string> lstStr in lstlstStr ) {
        if ( bHeader ) {
          bHeader = false ;
          continue ;
        }

        ClsKouyaitaTsugiteCsv cls = new ClsKouyaitaTsugiteCsv() ;
        cls.KoteiHoho = lstStr[ 0 ] ;
        cls.Type = lstStr[ 1 ] ;
        cls.Size = lstStr[ 2 ] ;
        cls.PlateSizeF = lstStr[ 3 ] ;
        cls.PlateNumF = RevitUtil.ClsCommonUtils.ChangeStrToInt( lstStr[ 4 ] ) ;
        cls.BoltSizeF = lstStr[ 5 ] ;
        cls.BoltNumF = RevitUtil.ClsCommonUtils.ChangeStrToInt( lstStr[ 6 ] ) ;
        cls.PlateSizeW = lstStr[ 7 ] ;
        cls.PlateNumW = RevitUtil.ClsCommonUtils.ChangeStrToInt( lstStr[ 8 ] ) ;
        cls.PlateSizeW2 = lstStr[ 9 ] ;
        cls.PlateNumW2 = RevitUtil.ClsCommonUtils.ChangeStrToInt( lstStr[ 10 ] ) ;
        cls.BoltSizeW = lstStr[ 11 ] ;
        cls.BoltNumW = RevitUtil.ClsCommonUtils.ChangeStrToInt( lstStr[ 12 ] ) ;
        lstCls.Add( cls ) ;
      }

      m_Data = lstCls ;

      return true ;
    }

    /// <summary>
    /// CSV情報を取得
    /// </summary>
    /// <returns></returns>
    public static List<ClsKouyaitaTsugiteCsv> GetCsvData()
    {
      GetCsv() ;
      return m_Data ;
    }

    /// <summary>
    /// 固定方法のリストを取得
    /// </summary>
    /// <returns></returns>
    public static List<string> GetTypeList()
    {
      GetCsv() ;
      List<string> lst = new List<string>() ;
      if ( ( from data in m_Data select data.KoteiHoho ).Any() ) {
        lst = ( from data in m_Data select data.KoteiHoho ).ToList() ;
      }

      return lst ;
    }

    /// <summary>
    /// 固定方法をキーにサイズのリストを取得
    /// </summary>
    /// <returns></returns>
    public static List<string> GetSizeList( string koteihoho )
    {
      GetCsv() ;
      List<string> lst = new List<string>() ;
      if ( ( from data in m_Data where data.KoteiHoho == koteihoho select data.Size ).Any() ) {
        lst = ( from data in m_Data where data.KoteiHoho == koteihoho select data.Size ).ToList() ;
      }

      return lst ;
    }

    /// <summary>
    /// サイズをキーに鋼矢板継ぎCSVクラスを取得
    /// </summary>
    /// <returns></returns>
    public static ClsKouyaitaTsugiteCsv GetCls( string koteihoho, string size )
    {
      GetCsv() ;
      ClsKouyaitaTsugiteCsv cls = new ClsKouyaitaTsugiteCsv() ;
      if ( ( from data in m_Data where data.KoteiHoho == koteihoho && data.Size == size select data ).Any() ) {
        cls = ( from data in m_Data where data.KoteiHoho == koteihoho && data.Size == size select data ).ToList()
          .First() ;
      }

      return cls ;
    }

    #endregion
  }
}