using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Reflection ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;

namespace YMS_gantry.Master
{
  public class ClsChannelCsv : ClsMasterCsv
  {
    public double H { get ; set ; }
    public double B { get ; set ; }
    public double T1 { get ; set ; }
    public double T2 { get ; set ; }
    public double R1 { get ; set ; }
    public double R2 { get ; set ; }

    public double HFt => ClsRevitUtil.CovertToAPI( H ) ;
    public double BFt => ClsRevitUtil.CovertToAPI( B ) ;
    public double T1Ft => ClsRevitUtil.CovertToAPI( T1 ) ;
    public double T2Ft => ClsRevitUtil.CovertToAPI( T2 ) ;
    public double R1Ft => ClsRevitUtil.CovertToAPI( R1 ) ;
    public double R2Ft => ClsRevitUtil.CovertToAPI( R2 ) ;

    #region メンバ変数

    /// <summary>
    /// CSV名称
    /// </summary>
    private const string CsvFileName = "Channel.csv" ;

    /// <summary>
    /// メンバ変数
    /// </summary>
    private static List<ClsChannelCsv> m_Data = new List<ClsChannelCsv>() ;

    /// <summary>
    /// マスタ名称
    /// </summary>
    public const string MasterName = "C材" ;

    /// <summary>
    /// 種類
    /// </summary>
    public const string TypeCannel = "チャンネル" ;

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
      List<ClsChannelCsv> lstCls = new List<ClsChannelCsv>() ;
      foreach ( List<string> lstStr in lstlstStr ) {
        if ( bHeader ) {
          bHeader = false ;
          continue ;
        }

        var index = 0 ;
        ClsChannelCsv cls = new ClsChannelCsv
        {
          Size = lstStr[ index++ ],
          FamilyPath = lstStr[ index++ ],
          H = Atof( lstStr.ElementAtOrDefault( index++ ) ),
          B = Atof( lstStr.ElementAtOrDefault( index++ ) ),
          T1 = Atof( lstStr.ElementAtOrDefault( index++ ) ),
          T2 = Atof( lstStr.ElementAtOrDefault( index++ ) ),
          R1 = Atof( lstStr.ElementAtOrDefault( index++ ) ),
          R2 = Atof( lstStr.ElementAtOrDefault( index++ ) ),
          HrzJoint = lstStr.ElementAtOrDefault( index++ ),
        } ;
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
    public static List<ClsChannelCsv> GetCsvData()
    {
      GetCsv() ;
      return m_Data ;
    }

    /// <summary>
    /// 種別をキーにサイズのリストを取得
    /// </summary>
    /// <returns></returns>
    public static List<string> GetSizeList()
    {
      GetCsv() ;
      List<string> lst = new List<string>() ;
      if ( ( from data in m_Data select data.Size ).Any() ) {
        lst = ( from data in m_Data select data.Size ).ToList() ;
      }

      //ファミリ存在チェック
      lst = CheckExist( lst ) ;
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

      string symbolFolpath = PathUtil.GetYMSFolder() ;
      string filePath = System.IO.Path.Combine( symbolFolpath, str ) ;

      if ( ! System.IO.File.Exists( filePath ) ) {
        //MessageBox.Show("ファミリが存在しません。\n" + filePath);
        filePath = string.Empty ;
      }

      return filePath ;
    }

    /// <summary>
    /// サイズの中に指定した値が存在するか確認
    /// </summary>
    /// <returns>サイズを返却(存在しない場合は空文字を返却)</returns>
    public static string ExistSize( string size )
    {
      if ( string.IsNullOrWhiteSpace( size ) ) return string.Empty ;

      GetCsv() ;
      string result = string.Empty ;
      if ( ( from data in m_Data
            where size.IndexOf( data.Size, StringComparison.OrdinalIgnoreCase ) >= 0
            select data.Size ).Any() ) {
        result = ( from data in m_Data
          where size.IndexOf( data.Size, StringComparison.OrdinalIgnoreCase ) >= 0
          select data.Size ).ToList().First() ;
      }

      return result ;
    }

    /// <summary>
    /// サイズから種別を特定する
    /// </summary>
    /// <param name="size">サイズ</param>
    /// <returns>種別</returns>
    public static string GetTypeBySize( string size )
    {
      if ( string.IsNullOrWhiteSpace( size ) ) return string.Empty ;

      GetCsv() ;
      string result = string.Empty ;
      if ( ( from data in m_Data where data.Size == size select data.Type ).Any() ) {
        result = TypeCannel ;
      }

      return result ;
    }

    #endregion
  }
}