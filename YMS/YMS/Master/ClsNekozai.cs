using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;

namespace YMS.Master
{
  class ClsNekozai
  {
    /// <summary>
    /// CSV名称
    /// </summary>
    private const string CsvFileName = "Nekozai.csv" ;

    /// <summary>
    /// メンバ変数
    /// </summary>
    private static List<ClsNekozai> m_Data = new List<ClsNekozai>() ;

    /// <summary>
    /// サイズ
    /// </summary>
    public string Size ;

    /// <summary>
    /// 名称
    /// </summary>
    public string Name ;

    /// <summary>
    /// 頭繋ぎフラグ
    /// </summary>
    public bool AtamaTsunagi ;

    /// <summary>
    /// ファミリパス
    /// </summary>
    public string FamilyPath ;

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
      List<ClsNekozai> lstCls = new List<ClsNekozai>() ;
      foreach ( List<string> lstStr in lstlstStr ) {
        if ( bHeader ) {
          bHeader = false ;
          continue ;
        }

        ClsNekozai cls = new ClsNekozai() ;
        cls.Size = lstStr[ 0 ] ;
        cls.Name = lstStr[ 1 ] ;
        cls.AtamaTsunagi = RevitUtil.ClsCommonUtils.ChangeStrToBool( lstStr[ 2 ] ) ;
        cls.FamilyPath = lstStr[ 3 ] ;
        lstCls.Add( cls ) ;
      }

      m_Data = lstCls ;

      return true ;
    }

    /// <summary>
    /// サイズをキーにファミリのパスを取得
    /// </summary>
    /// <returns></returns>
    public static string GetFamilyPath( string size, string name )
    {
      GetCsv() ;
      string str = string.Empty ;
      if ( ( from data in m_Data where data.Size == size && data.Name == name select data.FamilyPath ).Any() ) {
        str = ( from data in m_Data where data.Size == size && data.Name == name select data.FamilyPath ).ToList()
          .First() ;
      }

      string symbolFolpath = Parts.ClsZumenInfo.GetYMSFolder() ;
      string filePath = System.IO.Path.Combine( symbolFolpath, str ) ;
      return filePath ;
    }
  }
}