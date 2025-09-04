using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Runtime.InteropServices ;
using System.Text ;
using System.Threading.Tasks ;

namespace YMS
{
  public static class ClsIni
  {
    [DllImport( "kernel32.dll", EntryPoint = "GetPrivateProfileStringW", CharSet = CharSet.Unicode,
      SetLastError = true )]
    static extern uint GetPrivateProfileString( string lpAppName, string lpKeyName, string lpDefault,
      StringBuilder lpReturnedString, uint nSize, string lpFileName ) ;

    [DllImport( "kernel32.dll", EntryPoint = "WritePrivateProfileStringW", CharSet = CharSet.Unicode,
      SetLastError = true )]
    [return: MarshalAs( UnmanagedType.Bool )]
    public static extern bool WritePrivateProfileString( string lpAppName, string lpKeyName, string lpString,
      string lpFileName ) ;

    /// <summary>
    /// iniファイルから値を取ってくる
    /// </summary>
    /// <param name="sec">セクション名</param>
    /// <param name="key">キー名</param>
    /// <param name="path">iniファイルパス名</param>
    /// <returns>Value</returns>
    public static string GetIniFile( string sec, string key, string path )
    {
      int capacitySize = 256 ;
      StringBuilder sb = new StringBuilder( capacitySize ) ;
      uint ret = GetPrivateProfileString( sec, key, "none", sb, Convert.ToUInt32( sb.Capacity ), path ) ;
      if ( 0 < ret ) {
        return sb.ToString() ;
      }

      return string.Empty ;
    }

    /// <summary>
    /// iniファイルのパスを取得
    /// </summary>
    /// <param name="iniPath">ini\\ファイル名.ini</param>
    /// <returns></returns>
    public static string GetIniFilePath( string iniPath )
    {
      string apppath = System.Reflection.Assembly.GetExecutingAssembly().Location ;
      string appFol = System.IO.Path.GetDirectoryName( apppath ) ;
      string symbolFolpath = System.IO.Path.Combine( appFol, "YMS" ) ;

      return System.IO.Path.Combine( symbolFolpath, iniPath ) ;
    }

    /// <summary>
    /// 構台側のiniファイルのパスを取得
    /// </summary>
    /// <param name="iniPath">ini\\ファイル名.ini</param>
    /// <returns></returns>
    public static string GetKoudaiIniFilePath( string iniPath )
    {
      string apppath = System.Reflection.Assembly.GetExecutingAssembly().Location ;
      string appFol = System.IO.Path.GetDirectoryName( apppath ) ;
      string symbolFolpath = System.IO.Path.Combine( appFol, "YMS_gantry" ) ;

      return System.IO.Path.Combine( symbolFolpath, iniPath ) ;
    }
  }
}