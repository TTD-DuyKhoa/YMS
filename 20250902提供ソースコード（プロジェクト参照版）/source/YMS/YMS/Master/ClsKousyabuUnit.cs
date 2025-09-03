using RevitUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YMS.Master
{
    class ClsKousyabuUnit
    {

        #region 定数
        /// <summary>
        /// CSV名称
        /// </summary>
        private const string CsvFileName = "KousyabuMawari.csv";
        #endregion

        #region メンバ変数
        /// <summary>
        /// メンバ変数
        /// </summary>
        private static List<ClsKousyabuUnit> m_Data = new List<ClsKousyabuUnit>();
        #endregion

        #region プロパティ
        /// <summary>
        /// タイプ
        /// </summary>
        public string Type;

        /// <summary>
        /// 腹起サイズ
        /// </summary>
        public string HaraSize;

        /// <summary>
        /// 切梁サイズ
        /// </summary>
        public string KiriSize;

        /// <summary>
        /// ファミリパス
        /// </summary>
        public string FamilyPath;
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
            List<ClsKousyabuUnit> lstCls = new List<ClsKousyabuUnit>();
            foreach (List<string> lstStr in lstlstStr)
            {
                if (bHeader)
                {
                    bHeader = false;
                    continue;
                }

                ClsKousyabuUnit cls = new ClsKousyabuUnit();
                cls.Type = lstStr[0];
                cls.HaraSize = lstStr[1];
                cls.KiriSize = lstStr[2];
                cls.FamilyPath = lstStr[3];
                lstCls.Add(cls);
            }
            m_Data = lstCls;

            return true;
        }

        /// <summary>
        /// CSV情報を取得
        /// </summary>
        /// <returns></returns>
        public static List<ClsKousyabuUnit> GetCsvData()
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
                lst = (from data in m_Data select data.Type).Distinct().ToList();
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
            if ((from data in m_Data where data.Type == type select data.HaraSize).Any())
            {
                lst = (from data in m_Data where data.Type == type select data.HaraSize).ToList();
            }
            return lst;
        }

        /// <summary>
        /// 対象主材名称をキーにファミリのパスを取得
        /// </summary>
        /// <param name="type">交叉部or締結部</param>
        /// <param name="size">主材サイズ</param>
        /// <returns></returns>
        public static string GetFamilyPath(string type, string haraSize, string kiriSize)
        {
            GetCsv();
            string str = string.Empty;
            if ((from data in m_Data where data.HaraSize == haraSize && data.KiriSize == kiriSize && data.Type == type select data.FamilyPath).Any())
            {
                str = (from data in m_Data where data.HaraSize == haraSize && data.KiriSize == kiriSize && data.Type == type select data.FamilyPath).ToList().First();
            }

            string symbolFolpath = Parts.ClsZumenInfo.GetYMSFolder();
            string filePath = System.IO.Path.Combine(symbolFolpath, str);
            return filePath;
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
