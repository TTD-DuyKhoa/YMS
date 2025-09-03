using RevitUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YMS.Master
{
    class ClsStiffenerCSV
    {
        #region 定数
        /// <summary>
        /// CSV名称
        /// </summary>
        private const string CsvFileName = "Stiffener.csv";
        #endregion

        #region メンバ変数
        /// <summary>
        /// メンバ変数
        /// </summary>
        private static List<ClsStiffenerCSV> m_Data = new List<ClsStiffenerCSV>();
        #endregion

        #region プロパティ

        /// <summary>
        /// 腹起サイズ
        /// </summary>
        public string HaraSize;

        /// <summary>
        /// タイプ(プレートorジャッキ)
        /// </summary>
        public string Type;

        /// <summary>
        /// タイプ(プレートタイプorジャッキタイプ)
        /// </summary>
        public string Type2;

        /// <summary>
        /// 記号
        /// </summary>
        public string Mark;

        /// <summary>
        /// サイズ
        /// </summary>
        public string Size;

        /// <summary>
        /// ファミリパス
        /// </summary>
        public string FamilyPath;

        /// <summary>
        /// アタッチメントパス
        /// </summary>
        public string AttachmentPath;
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
            List<ClsStiffenerCSV> lstCls = new List<ClsStiffenerCSV>();
            foreach (List<string> lstStr in lstlstStr)
            {
                if (bHeader)
                {
                    bHeader = false;
                    continue;
                }

                ClsStiffenerCSV cls = new ClsStiffenerCSV();
                cls.HaraSize = lstStr[0];
                cls.Type = lstStr[1];
                cls.Type2 = lstStr[2];
                cls.Mark = lstStr[3];
                cls.Size = lstStr[4];
                cls.FamilyPath = lstStr[5];
                cls.AttachmentPath = lstStr[6];
                lstCls.Add(cls);
            }
            m_Data = lstCls;

            return true;
        }

        /// <summary>
        /// CSV情報を取得
        /// </summary>
        /// <returns></returns>
        public static List<ClsStiffenerCSV> GetCsvData()
        {
            GetCsv();
            return m_Data;
        }


        /// <summary>
        /// 腹起サイズ、タイプ2をキーにファミリのパスを取得
        /// </summary>
        /// <returns></returns>
        public static string GetFamilyPath(string haraSize, string type2)
        {
            GetCsv();
            string str = string.Empty;
            if ((from data in m_Data where data.HaraSize == haraSize && data.Type2 == type2 select data.FamilyPath).Any())
            {
                str = (from data in m_Data where data.HaraSize == haraSize && data.Type2 == type2 select data.FamilyPath).ToList().First();
            }

            string symbolFolpath = Parts.ClsZumenInfo.GetYMSFolder();
            string filePath = System.IO.Path.Combine(symbolFolpath, str);
            return filePath;
        }
        /// <summary>
        /// 腹起サイズ、タイプ2をキーに記号を取得
        /// </summary>
        /// <returns></returns>
        public static string GetMark(string haraSize, string type2)
        {
            GetCsv();
            string str = string.Empty;
            if ((from data in m_Data where data.HaraSize == haraSize && data.Type2 == type2 select data.Mark).Any())
            {
                str = (from data in m_Data where data.HaraSize == haraSize && data.Type2 == type2 select data.Mark).ToList().First();
            }

            return str;
        }
        /// <summary>
        /// 腹起サイズ、タイプをキーに記号を取得
        /// </summary>
        /// <returns></returns>
        public static List<string> GetTypeCount(string haraSize, string type)
        {
            GetCsv();
            List<string> list = new List<string>();
            if ((from data in m_Data where data.HaraSize == haraSize && data.Type == type select data.Mark).Any())
            {
                list = (from data in m_Data where data.HaraSize == haraSize && data.Type == type select data.Mark).ToList();
            }

            return list;
        }
        /// <summary>
        /// ファミリの名前Listを取得
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFamilyNameList()
        {
            GetCsv();
            List<string> kariNameList = new List<string>();
            foreach (var data in m_Data)
            {
                string filePath = data.FamilyPath;
                string familyName = ClsRevitUtil.GetFamilyName(filePath);
                kariNameList.Add(familyName);
            }

            return kariNameList;
        }
        #endregion
    }
}
