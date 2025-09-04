using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;

namespace YMS_gantry.Master
{
  class ClsHBeamCsv : ClsMasterCsv
  {
    #region メンバ変数

    /// <summary>
    /// CSV名称
    /// </summary>
    private const string CsvFileName = "HBeam.csv" ;

    /// <summary>
    /// メンバ変数
    /// </summary>
    private static List<ClsHBeamCsv> m_Data = new List<ClsHBeamCsv>() ;

    /// <summary>
    /// マスタ名称
    /// </summary>
    public const string MasterName = "H鋼" ;

    /// <summary>
    /// 種類
    /// </summary>
    public const string TypeHiro = "H形鋼 広幅" ;

    public const string TypeNaka = "H形鋼 中幅" ;
    public const string TypeHoso = "H形鋼 細幅" ;

    #endregion

    #region プロパティ

    /// <summary>
    /// ファミリパス(杭)
    /// </summary>
    public string PileFamilyPath ;

    public double H { get ; set ; }
    public double B { get ; set ; }
    public double T1 { get ; set ; }
    public double T2 { get ; set ; }
    public double R { get ; set ; }

    public double HFt => ClsRevitUtil.CovertToAPI( H ) ;
    public double BFt => ClsRevitUtil.CovertToAPI( B ) ;
    public double T1Ft => ClsRevitUtil.CovertToAPI( T1 ) ;
    public double T2Ft => ClsRevitUtil.CovertToAPI( T2 ) ;
    public double RFt => ClsRevitUtil.CovertToAPI( R ) ;

    /// <summary>
    /// ファミリパス(継足し杭)
    /// </summary>
    public string ExPileFamilyPath ;

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
      List<ClsHBeamCsv> lstCls = new List<ClsHBeamCsv>() ;
      foreach ( List<string> lstStr in lstlstStr ) {
        if ( bHeader ) {
          bHeader = false ;
          continue ;
        }

        var index = 0 ;
        ClsHBeamCsv cls = new ClsHBeamCsv
        {
          Type = lstStr[ index++ ],
          Size = lstStr[ index++ ],
          FamilyPath = lstStr[ index++ ],
          PileFamilyPath = lstStr[ index++ ],
          H = Atof( lstStr.ElementAtOrDefault( index++ ) ),
          B = Atof( lstStr.ElementAtOrDefault( index++ ) ),
          T1 = Atof( lstStr.ElementAtOrDefault( index++ ) ),
          T2 = Atof( lstStr.ElementAtOrDefault( index++ ) ),
          R = Atof( lstStr.ElementAtOrDefault( index++ ) ),
          HrzJoint = lstStr.ElementAtOrDefault( index++ ),
          ExPileFamilyPath = lstStr.ElementAtOrDefault( index++ )
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
    public static List<ClsHBeamCsv> GetCsvData()
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
    public static List<string> GetSizeList( string type, bool bPile = false )
    {
      GetCsv() ;
      List<string> lst = new List<string>() ;
      if ( ( from data in m_Data where data.Type == type select data.Size ).Any() ) {
        lst = ( from data in m_Data where data.Type == type select data.Size ).ToList() ;
      }

      //ファミリ存在チェック
      lst = CheckExist( lst, bPile: bPile ) ;

      ////ファミリ一点配置対応 //※この処理は11月末インストーラ以降は削除します
      //if (!bPile)
      //{
      //    List<string> lstNew = new List<string>();
      //    foreach (string str in lst)
      //    {
      //        if (str == "H-250X250X9X14" || str == "H-300X300X10X15" || str == "H-350X350X12X19")
      //            lstNew.Add(str);
      //    }
      //    lst = lstNew;
      //}

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
    /// サイズをキーにファミリ(杭)のパスを取得
    /// </summary>
    /// <returns></returns>
    public static string GetPileFamilyPath( string size )
    {
      GetCsv() ;
      string str = string.Empty ;
      if ( ( from data in m_Data where data.Size == size select data.PileFamilyPath ).Any() ) {
        str = ( from data in m_Data where data.Size == size select data.PileFamilyPath ).ToList().First() ;
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
    /// サイズをキーにファミリ(継足し杭)のパスを取得
    /// </summary>
    /// <returns></returns>
    public static string GetExPileFamilyPath( string size )
    {
      GetCsv() ;
      string str = string.Empty ;
      if ( ( from data in m_Data where data.Size == size select data.ExPileFamilyPath ).Any() ) {
        str = ( from data in m_Data where data.Size == size select data.ExPileFamilyPath ).ToList().First() ;
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
    /// 継ぎ足し杭の存在するサイズか
    /// </summary>
    /// <returns></returns>
    public static bool IsExPileFamily( string size )
    {
      return ! string.IsNullOrEmpty( GetExPileFamilyPath( size ) ) ;
    }

    /// <summary>
    /// CSV情報からファミリ名を全て取得
    /// </summary>
    /// <returns></returns>
    public static List<string> GetAllHBeamName()
    {
      List<string> allHBeamNameList = new List<string>() ;
      List<ClsHBeamCsv> clsHBeamList = GetCsvData() ;
      foreach ( ClsHBeamCsv clsHBeam in clsHBeamList ) {
        string familyNameKui = RevitUtil.ClsRevitUtil.GetFamilyName( clsHBeam.PileFamilyPath ) ;
        allHBeamNameList.Add( familyNameKui ) ;
      }

      return allHBeamNameList ;
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
            where data.Size.IndexOf( size, StringComparison.OrdinalIgnoreCase ) >= 0
            select data.Size ).Any() ) {
        result = ( from data in m_Data
          where data.Size.IndexOf( size, StringComparison.OrdinalIgnoreCase ) >= 0
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