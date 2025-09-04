using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;

namespace YMS_gantry.Master
{
  class ClsYamadomeCsv : ClsMasterCsv
  {
    #region メンバ変数

    /// <summary>
    /// CSV名称
    /// </summary>
    private const string CsvFileName = "Yamadome.csv" ;

    /// <summary>
    /// メンバ変数
    /// </summary>
    private static List<ClsYamadomeCsv> m_Data = new List<ClsYamadomeCsv>() ;

    /// <summary>
    /// マスタ名称
    /// </summary>
    public const string MasterName = "主材" ;

    /// <summary>
    /// 種類
    /// </summary>
    public const string TypeShuzai = "主材" ;

    public const string TypeHighShuzai = "高強度主材" ;
    public const string TypeTwinBeam = "ツインビーム" ;
    public const string TypeMegaBeam = "メガビーム" ;

    #endregion

    #region プロパティ

    /// <summary>
    /// 長さ
    /// </summary>
    public double Length ;

    /// <summary>
    /// 仮鋼材ファミリパス
    /// </summary>
    public string KariFamilyPath ;

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
      List<ClsYamadomeCsv> lstCls = new List<ClsYamadomeCsv>() ;
      foreach ( List<string> lstStr in lstlstStr ) {
        if ( bHeader ) {
          bHeader = false ;
          continue ;
        }

        ClsYamadomeCsv cls = new ClsYamadomeCsv() ;
        cls.Type = lstStr[ 0 ] ;
        cls.Size = lstStr[ 1 ] ;
        cls.Length = RevitUtil.ClsCommonUtils.ChangeStrToDbl( lstStr[ 2 ] ) ;
        cls.FamilyPath = lstStr[ 3 ] ;
        cls.KariFamilyPath = lstStr[ 4 ] ;
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
    public static List<ClsYamadomeCsv> GetCsvData()
    {
      GetCsv() ;
      return m_Data ;
    }

    /// <summary>
    /// 種別のリストを取得
    /// </summary>
    /// <returns></returns>
    public static List<string> GetTypeList( bool bHigh = true, bool bMega = true, bool bTwin = true )
    {
      GetCsv() ;
      List<string> lst = new List<string>() ;
      if ( ( from data in m_Data select data.Type ).Any() ) {
        lst = ( from data in m_Data select data.Type ).Distinct().ToList() ;


        if ( bHigh == false ) {
          lst = ( from data in lst where data != TypeHighShuzai select data ).Distinct().ToList() ;
        }

        if ( bMega == false ) {
          lst = ( from data in lst where data != TypeMegaBeam select data ).Distinct().ToList() ;
        }

        if ( bTwin == false ) {
          lst = ( from data in lst where data != TypeTwinBeam select data ).Distinct().ToList() ;
        }
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

      //ファミリ存在チェック
      lst = CheckExist( lst, 4.0 ) ; //4mがあれば存在しているとする

      return lst ;
    }

    /// <summary>
    /// サイズをキーにファミリのパスを取得
    /// </summary>
    /// <returns></returns>
    public static string GetFamilyPath( string size, double dLength )
    {
      GetCsv() ;
      string str = string.Empty ;
      if ( ( from data in m_Data where data.Size == size && data.Length == dLength select data.FamilyPath ).Any() ) {
        str = ( from data in m_Data where data.Size == size && data.Length == dLength select data.FamilyPath ).ToList()
          .First() ;
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
    /// サイズをキーに仮鋼材のファミリのパスを取得
    /// </summary>
    /// <returns></returns>
    public static string GetKariFamilyPath( string size, double dLength )
    {
      GetCsv() ;
      string str = string.Empty ;
      if ( ( from data in m_Data where data.Size == size && data.Length == dLength select data.KariFamilyPath )
          .Any() ) {
        str = ( from data in m_Data where data.Size == size && data.Length == dLength select data.KariFamilyPath )
          .ToList().First() ;
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
    /// 仮鋼材のファミリのファミリ名Listを取得
    /// </summary>
    /// <returns></returns>
    public static List<string> GetKariFamilyNameList()
    {
      GetCsv() ;
      List<string> kariNameList = new List<string>() ;
      foreach ( var data in m_Data ) {
        string filePath = data.KariFamilyPath ;
        string familyName = ClsRevitUtil.GetFamilyName( filePath ) ;
        kariNameList.Add( familyName ) ;
      }

      return kariNameList ;
    }

    /// <summary>
    /// サイズから幅を取得
    /// </summary>
    /// <returns></returns>
    public static double GetWidth( string size )
    {
      //本来はファミリの中身を見に行って、プロパティから幅の値を取得するが、一旦仮
      double dWidth = 0 ;
      switch ( size ) {
        case "20HA" :
        {
          dWidth = 200 ;
          break ;
        }
        case "25HA" :
        {
          dWidth = 250 ;
          break ;
        }
        case "30HA" :
        {
          dWidth = 300 ;
          break ;
        }
        case "35HA" :
        {
          dWidth = 350 ;
          break ;
        }
        case "40HA" :
        {
          dWidth = 400 ;
          break ;
        }
        case "50HA" :
        {
          dWidth = 500 ;
          break ;
        }
        default :
        {
          dWidth = 0 ;
          break ;
        }
      }

      return dWidth ;
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
        result = ( from data in m_Data where data.Size == size select data.Type ).ToList().First() ;
      }

      return result ;
    }

    #endregion
  }
}