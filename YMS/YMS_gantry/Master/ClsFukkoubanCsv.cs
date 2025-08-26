using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YMS_gantry.Master
{
    class ClsFukkoubanCsv : ClsMasterCsv
    {
        #region メンバ変数
        /// <summary>
        /// CSV名称
        /// </summary>
        private const string CsvFileName = "Fukkouban.csv";

        /// <summary>
        /// メンバ変数
        /// </summary>
        private static List<ClsFukkoubanCsv>  m_Data = new List<ClsFukkoubanCsv>();

        /// <summary>
        /// 種類
        /// </summary>
        public const string TypeNormal = "覆工板";
        public const string TypeM = "覆工板(M)";
        public const string TypeSp = "覆工板(特殊)";
        #endregion

        #region メソッド
        /// <summary>
        /// CSVから情報を取得
        /// </summary>
        /// <returns></returns>
        public static bool GetCsv()
        {
            if(m_Data != null && m_Data.Count != 0)
            {
                return true;
            }

            string symbolFolpath = PathUtil.GetYMS_GantryMasterPath();
            string fileName = System.IO.Path.Combine(symbolFolpath,  CsvFileName);
            List<List<string>> lstlstStr = new List<List<string>>();
            if(!RevitUtil.ClsCommonUtils.ReadCsv(fileName, ref lstlstStr))
            {
                MessageBox.Show("CSVファイルの取得に失敗しました。：" + fileName);
                return false;
            }

            bool bHeader = true;
            List<ClsFukkoubanCsv> lstCls = new List<ClsFukkoubanCsv>();
            foreach(List<string> lstStr in lstlstStr)
            {
                if(bHeader) 
                {
                    bHeader = false;
                    continue;
                }

                ClsFukkoubanCsv cls = new ClsFukkoubanCsv();
                cls.Type = lstStr[0];
                cls.Size = lstStr[1];
                cls.FamilyPath = lstStr[2];
                lstCls.Add(cls);
            }
            m_Data = lstCls;

            return true;
        }

        /// <summary>
        /// CSV情報を取得
        /// </summary>
        /// <returns></returns>
        [YmsMasterLoad]
        public static List<ClsFukkoubanCsv> GetCsvData()
        {
            GetCsv();
            return m_Data;
        }

        /// <summary>
        /// 種別のリストを取得
        /// </summary>
        /// <returns></returns>
        public static List<string> GetTypeList()
        {
            GetCsv();
            List<string> lst = new List<string>();
            if ((from data in m_Data select data.Type).Any())
            {
                lst = (from data in m_Data select data.Type).ToList();
            }
            return lst;
        }

        /// <summary>
        /// 種別をキーにサイズのリストを取得
        /// </summary>
        /// <returns></returns>
        public static List<string> GetSizeList(string type)
        {
            GetCsv();
            List<string> lst = new List<string>();
            if ((from data in m_Data where data.Type == type select data.Size).Any())
            {
                lst = (from data in m_Data where data.Type == type select data.Size).ToList();
            }
            //ファミリ存在チェック
            lst = CheckExist(lst);
            return lst;
        }

        /// <summary>
        /// サイズをキーにファミリのパスを取得
        /// </summary>
        /// <returns></returns>
        public static string GetFamilyPath(string size)
        {
            GetCsv();
            string  str = string.Empty;
            if ((from data in m_Data where data.Size == size select data.FamilyPath).Any())
            {
                str = (from data in m_Data where data.Size == size select data.FamilyPath).ToList().First();
            }

            string symbolFolpath = PathUtil.GetYMSFolder();
            string filePath = System.IO.Path.Combine(symbolFolpath, str);

            if (!System.IO.File.Exists(filePath))
            {
                //MessageBox.Show("ファミリが存在しません。\n" + filePath);
                filePath = string.Empty;
            }

            return filePath;
        }

        /// <summary>
        /// サイズの中に指定した値が存在するか確認
        /// </summary>
        /// <returns>サイズを返却(存在しない場合は空文字を返却)</returns>
        public static string ExistSize(string size)
        {
            if (string.IsNullOrWhiteSpace(size)) return string.Empty;

            GetCsv();
            string result = string.Empty;
            if ((from data in m_Data where size.IndexOf(data.Size, StringComparison.OrdinalIgnoreCase) >= 0 select data.Size).Any())
            {
                result = (from data in m_Data where size.IndexOf(data.Size, StringComparison.OrdinalIgnoreCase) >= 0 select data.Size).ToList().First();
            }

            return result;
        }

        /// <summary>
        /// サイズの中に指定した値が存在するか確認（計算書取込用）
        /// </summary>
        /// <returns>サイズを返却(存在しない場合は空文字を返却)</returns>
        public static string ExistSizeForCalc(string size, string material)
        {
            if (string.IsNullOrWhiteSpace(size)) return string.Empty;

            if (material == "SM490")
            {
                size = size.Replace("MD", "MD(M)");
            }

            GetCsv();
            string result = string.Empty;
            if ((from data in m_Data where size.IndexOf(data.Size, StringComparison.OrdinalIgnoreCase) >= 0 select data.Size).Any())
            {
                result = (from data in m_Data where size.IndexOf(data.Size, StringComparison.OrdinalIgnoreCase) >= 0 select data.Size).ToList().First();
            }

            return result;
        }
        #endregion
    }
}
