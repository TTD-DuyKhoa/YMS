using System ;
using System.Collections.Generic ;
using System.Diagnostics ;
using System.IO ;
using System.Linq ;
using System.Reflection ;
using System.Text ;
using System.Threading.Tasks ;

namespace YMS
{
  public class ClsProtect
  {
    public static bool bCheckProtect()
    {
      #if DEBUG
      return true ;

      #else
            try  
              {
                  // 外部の exe ファイルのパスを指定して起動
                  string assemblyPath = Assembly.GetExecutingAssembly().Location;
                  string dllPath = Path.Combine(Path.GetDirectoryName(assemblyPath), "YMS.dll");
                  string externalExePath = dllPath.Replace("YMS.dll", "YMS_Protect.exe");

                  Process process = new Process();
                  process.StartInfo.FileName = externalExePath;
                  process.StartInfo.UseShellExecute = false;
                  process.StartInfo.RedirectStandardOutput = true;

                  process.Start();
                  process.WaitForExit(); // 外部プロセスの終了を待つ
                  int exitCode = process.ExitCode;
                  switch (exitCode)
                  {
                      case 0:     //正常終了
                          return true;
                      case 1:     // キャンセル終了
                          return false;
                      case 2:     // その他の終了コード
                          return false;
                      default:
                          return false;
                  }

              }
              catch (Exception ex)
              {
                  return false;
              }

            //return true;
      #endif
    }
  }
}