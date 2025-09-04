using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Runtime.InteropServices ;
using System.Text ;
using System.Threading.Tasks ;

namespace YMS
{
  class ClsMiscWin
  {
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
  }
}