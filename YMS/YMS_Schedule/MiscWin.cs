using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Runtime.InteropServices ;
using System.IO ;
using System.Windows.Media.Imaging ;

namespace YMS_Schedule
{
  public static class MiscWin
  {
    #region MyRegion

    private static Encoding DetectEncoding( string path )
    {
      var fs = File.OpenRead( path ) ;
      var bom = new byte[ Math.Min( 4, fs.Length ) ] ;
      fs.Read( bom, 0, bom.Length ) ;
      if ( bom.Length >= 3 && bom[ 0 ] == 0xEF && bom[ 1 ] == 0xBB && bom[ 2 ] == 0xBF )
        return new UTF8Encoding( true ) ;
      if ( bom.Length >= 2 && bom[ 0 ] == 0xFF && bom[ 1 ] == 0xFE ) return Encoding.Unicode ;
      if ( bom.Length >= 2 && bom[ 0 ] == 0xFE && bom[ 1 ] == 0xFF ) return Encoding.BigEndianUnicode ;
      return Encoding.GetEncoding( 932 ) ; // Shift-JIS mặc định
    }

    #endregion

    [DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
    private static extern int GetPrivateProfileString( string lpApplicationName, string lpKeyName, string lpDefault,
      StringBuilder lpReturnedstring, int nSize, string lpFileName ) ;

    [DllImport( "kernel32.dll" )]
    static extern int GetPrivateProfileSectionNames( IntPtr lpszReturnBuffer, uint nSize, string lpFileName ) ;

    [DllImport( "KERNEL32.DLL", EntryPoint = "GetPrivateProfileStringA" )]
    public static extern uint GetPrivateProfileStringByByteArray( string lpAppName, string lpKeyName, string lpDefault,
      byte[] lpReturnedString, uint nSize, string lpFileName ) ;

    public static string GetIniValue( string path, string section, string key, string dflt = "" )
    {
      var enc = DetectEncoding( path ) ;
      var text = File.ReadAllText( path, enc ) ;
      var tmp = Path.Combine( Path.GetTempPath(), $"ini_utf16_{Path.GetFileName( path )}_{Guid.NewGuid():N}.ini" ) ;
      File.WriteAllText( tmp, text, Encoding.Unicode ) ;
      StringBuilder sb = new StringBuilder( 4096 ) ;
      GetPrivateProfileString( section, key, dflt, sb, sb.Capacity, path ) ;
      string res = sb.ToString() ;
      return string.IsNullOrEmpty( res ) ? dflt : res ;
    }

    public static List<string> GetSectionNames( string path )
    {
      List<string> res = new List<string>() ;

      if ( File.Exists( path ) ) {
        IntPtr ptr = Marshal.StringToHGlobalAnsi( new String( '\0', 1024 ) ) ;
        int length = GetPrivateProfileSectionNames( ptr, 1024, path ) ;

        if ( 0 < length ) {
          String result = Marshal.PtrToStringAnsi( ptr, length ) ;

          Array.ForEach<String>( result.Split( '\0' ), s => res.Add( s ) ) ;
        }

        Marshal.FreeHGlobal( ptr ) ;

        if ( res.Contains( "" ) ) {
          res.Remove( "" ) ;
        }
      }

      return res ;
    }

    public static List<string> GetKeys( string sectionName, string path )
    {
      byte[] ar1 = new byte[ 4096 ] ;
      uint sz = GetPrivateProfileStringByByteArray( sectionName, null, "", ar1, (uint) ar1.Length, path ) ;
      string results = Encoding.Default.GetString( ar1, 0, (int) sz - 1 ) ;
      return results.Split( '\0' ).ToList() ;
    }

    [DllImport( "User32.dll", EntryPoint = "SendMessage" )]
    public static extern int SendMessage( int hWnd, int Msg, int wParam, int lParam ) ;

    [DllImport( "User32.dll", EntryPoint = "PostMessage" )]
    public static extern int PostMessage( int hWnd, int Msg, int wParam, int lParam ) ;

    public const int WM_KEYDOWN = 0x0100 ;

    public static int SendWindowsMessage( int hWnd, int Msg, int wParam, int lParam )
    {
      int result = 0 ;
      if ( hWnd > 0 ) {
        result = SendMessage( hWnd, Msg, wParam, lParam ) ;
      }

      return result ;
    }

    public static int PostWindowsMessage( int hWnd, int Msg, int wParam, int lParam )
    {
      int result = 0 ;
      if ( hWnd > 0 ) {
        result = PostMessage( hWnd, Msg, wParam, lParam ) ;
      }

      return result ;
    }

    public static System.Windows.Media.ImageSource BmpImageSource( string embeddedPath )
    {
      return new BitmapImage( new Uri( embeddedPath ) ) ;
    }

    public static string MakeValidFileName( string name )
    {
      string invalidChars =
        System.Text.RegularExpressions.Regex.Escape( new string( System.IO.Path.GetInvalidFileNameChars() ) ) ;
      string invalidReStr = string.Format( @"([{0}]*\.+$)|([{0}]+)", invalidChars ) ;
      return System.Text.RegularExpressions.Regex.Replace( name, invalidReStr, "_" ) ;
    }
  }
}