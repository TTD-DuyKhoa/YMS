using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YMS.Master
{
    class ClsBracketCsv
    {
        #region メンバ変数
        /// <summary>
        /// CSV名称
        /// </summary>
        private const string CsvFileName = "Bracket.csv";
        #endregion

        #region メンバ変数
        /// <summary>
        /// メンバ変数
        /// </summary>
        private static List<ClsBracketCsv> m_Data = new List<ClsBracketCsv>();
        #endregion

        #region プロパティ
        /// <summary>
        /// 種類
        /// </summary>
        public string type;

        /// <summary>
        /// サイズ
        /// </summary>
        public string size;

        /// <summary>
        /// 幅
        /// </summary>
        public string width;

        /// <summary>
        /// 長さ
        /// </summary>
        public string length;

        /// <summary>
        /// ファミリパス
        /// </summary>
        public string familyPath;
        #endregion

        #region メソッド
        /// <summary>
        /// CSVから情報を取得
        /// </summary>
        /// <returns></returns>
        public static bool GetCsv()
        {
            if (m_Data != null && m_Data.Count != 0)
            {
                return true;
            }

            string symbolFolpath = Parts.ClsZumenInfo.GetYMSFolder();
            string fileName = System.IO.Path.Combine(symbolFolpath, Parts.ClsZumenInfo.master, CsvFileName);
            List<List<string>> lstlstStr = new List<List<string>>();
            if (!RevitUtil.ClsCommonUtils.ReadCsv(fileName, ref lstlstStr))
            {
                MessageBox.Show("CSVファイルの取得に失敗しました。：" + fileName);
                return false;
            }

            bool bHeader = true;
            List<ClsBracketCsv> lstCls = new List<ClsBracketCsv>();
            foreach (List<string> lstStr in lstlstStr)
            {
                if (bHeader)
                {
                    bHeader = false;
                    continue;
                }

                ClsBracketCsv cls = new ClsBracketCsv();
                cls.type = lstStr[0];
                cls.size = lstStr[1];
                cls.width = lstStr[2];
                cls.length = lstStr[3];
                cls.familyPath = lstStr[4];
                lstCls.Add(cls);
            }
            m_Data = lstCls;

            return true;
        }

        /// <summary>
        /// CSV情報を取得
        /// </summary>
        /// <returns></returns>
        public static List<ClsBracketCsv> GetCsvData()
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
            if ((from data in m_Data select data.type).Any())
            {
                lst = (from data in m_Data select data.type).ToList();
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
            if ((from data in m_Data where data.type == type select data.size).Any())
            {
                lst = (from data in m_Data where data.type == type select data.size).ToList();
            }
            return lst;
        }

        /// <summary>
        /// サイズをキーに幅を取得
        /// </summary>
        /// <returns></returns>
        public static double GetWidth(string size)
        {
            GetCsv();
            double width = double.NaN;
            string str = string.Empty;
            if ((from data in m_Data where data.size == size select data.width).Any())
            {
                str = (from data in m_Data where data.size == size select data.width).ToList().First();
            }
            double.TryParse(str, out width);

            return width;
        }

        /// <summary>
        /// サイズをキーに長さを取得
        /// </summary>
        /// <returns></returns>
        public static double GetLength(string size)
        {
            GetCsv();
            double length = double.NaN;
            string str = string.Empty;
            if ((from data in m_Data where data.size == size select data.length).Any())
            {
                str = (from data in m_Data where data.size == size select data.length).ToList().First();
            }
            double.TryParse(str, out length);

            return length;
        }

        /// <summary>
        /// 種別とサイズから長さを取得
        /// </summary>
        /// <returns></returns>
        public static double GetLength(string type, string size)
        {
            GetCsv();
            double length = double.NaN;
            string str = string.Empty;

            var query = from data in m_Data
                        where data.type == type && data.size == size
                        select data.length;

            if (query.Any())
            {
                str = query.First();
            }

            double.TryParse(str, out length);
            return length;
        }


        /// <summary>
        /// サイズをキーにファミリのパスを取得
        /// </summary>
        /// <returns></returns>
        public static string GetFamilyPath(string size)
        {
            GetCsv();
            string str = string.Empty;
            if ((from data in m_Data where data.size == size select data.familyPath).Any())
            {
                str = (from data in m_Data where data.size == size select data.familyPath).ToList().First();
            }

            string symbolFolpath = Parts.ClsZumenInfo.GetYMSFolder();
            string filePath = System.IO.Path.Combine(symbolFolpath, str);
            return filePath;
        }
        #endregion
    }
}
