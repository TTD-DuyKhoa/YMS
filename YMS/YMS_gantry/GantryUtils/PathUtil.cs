using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YMS_gantry.GantryUtils;

namespace YMS_gantry
{
    public class PathUtil
    {

        #region 定数
        const string inisec = "Path";
        const string inikey = "FOLDER_Gantry";
        const string iniName = "Path.ini";
        #endregion

        /// <summary>
        /// DLL直下YMS_gantryフォルダ取得
        /// </summary>
        /// <returns></returns>
        public static string GetExecutingAssemblyYMSGantryPath()
        {
            string apppath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string appFol = System.IO.Path.GetDirectoryName(apppath);
            string symbolFolpath = System.IO.Path.Combine(appFol, "");

            return symbolFolpath;
        }

        /// <summary>
        /// ファミリメインディレクトリ取得
        /// </summary>
        /// <returns></returns>
        public static string GetGantyrUtilFamilyFolderPath()
        {
            string retSt=GetYMSFolder();
            retSt = System.IO.Path.Combine(retSt, "構台汎用関係"); ;
            return retSt;
        }

        /// <summary>
        /// マスターディレクトリ取得
        /// </summary>
        /// <returns></returns>
        public static string GetYMSgantryDir()
        {
            string retSt = "";
            string dllPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            dllPath = dllPath.Replace(dllPath.Substring(dllPath.IndexOf("YMS_gantry")), "");
            dllPath += "YMS_gantry";
            retSt = dllPath;
            return retSt;
        }

        /// <summary>
        /// モデル情報（一時ファイル）の出力先ファイルパス取得
        /// </summary>
        /// <returns></returns>
        public static string GetTempModelFilePath()
        {
            string path = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            path = System.IO.Path.Combine(path, "YmsModelTemp.xml");
            return path;
        }

        /// <summary>
        /// YMSフォルダを取得（このフォルダの下にファミリが入っている）
        /// </summary>
        /// <returns>YMSフォルダパス</returns>
        public static string GetYMSFolder()
        {
            string selectedPath = string.Empty;

            string iniPath = ClsIni.GetIniFilePath(iniName);

            selectedPath = ClsIni.GetIniFile(inisec, inikey, iniPath);

            if(!System.IO.Directory.Exists(selectedPath) )
            {
                selectedPath = GetYMSgantryDir();
            }

            return selectedPath;
        }

        /// <summary>
        /// マスタフォルダのトップを取得
        /// </summary>
        /// <returns></returns>
        public static string GetYMS_GantryMasterPath()
        {
            string retPath = string.Empty;
            string iniPath = ClsIni.GetIniFilePath(iniName);

            retPath = ClsIni.GetIniFile(inisec, inikey, iniPath);

            if (!System.IO.Directory.Exists(retPath))
            {
                retPath = GetYMSgantryDir(); ;
            }

            retPath=  System.IO.Path.Combine(retPath, "master","Koudai");
            return retPath;
        }
    }
}
