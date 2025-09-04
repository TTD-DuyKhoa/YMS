namespace YMS
{
  class ClsSystemData
  {
    public const string DLL_NAME = "YMS.dll" ;
    public const string FILE_FOLDER_NAME = "YMS" ;
    public const string CSV_FILE_NAME = "YMS.csv" ;

    /// <summary>
    /// システムが使用するiniファイル等の各ファイルの置き場所を取得
    /// </summary>
    /// <returns></returns>
    /// <remarks>レジストリにiniファイルが書かれている前提</remarks>
    public static string GetSystemFileFolder()
    {
      string path = string.Empty ;

      #if DEBUG
      path = System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location ) ;
      #else
           // path = ClsRegistry.GetInstallPath();
      #endif

      return path ;
    }

    /// <summary>
    /// 実行ファイルのパスを取得
    /// </summary>
    /// <returns></returns>
    public static string GetDLLPath()
    {
      string fol = System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location ) ;
      return System.IO.Path.Combine( fol, DLL_NAME ) ;
    }
  }
}