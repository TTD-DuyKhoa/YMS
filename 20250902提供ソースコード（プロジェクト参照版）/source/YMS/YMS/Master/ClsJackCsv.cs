using RevitUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YMS.Master
{
    class ClsJackCsv
    {
        #region メンバ変数
        /// <summary>
        /// CSV名称
        /// </summary>
        private const string CsvFileName = "Jack.csv";
        #endregion

        #region メンバ変数
        /// <summary>
        /// メンバ変数
        /// </summary>
        private static List<ClsJackCsv>  m_Data = new List<ClsJackCsv>();
        #endregion

        #region プロパティ
        /// <summary>
        /// 種類
        /// </summary>
        public string Type;

        /// <summary>
        /// サイズ
        /// </summary>
        public string Size;

        /// <summary>
        /// 対応主材
        /// </summary>
        public string Shuzai;

        /// <summary>
        /// タイプ名の種類
        /// </summary>
        public string TypeNameType;

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
            if(m_Data != null && m_Data.Count != 0)
            {
                return true;
            }

            string symbolFolpath = Parts.ClsZumenInfo.GetYMSFolder();
            string fileName = System.IO.Path.Combine(symbolFolpath, Parts.ClsZumenInfo.master, CsvFileName);
            List<List<string>> lstlstStr = new List<List<string>>();
            if(!RevitUtil.ClsCommonUtils.ReadCsv(fileName, ref lstlstStr))
            {
                MessageBox.Show("CSVファイルの取得に失敗しました。：" + fileName);
                return false;
            }

            bool bHeader = true;
            List<ClsJackCsv> lstCls = new List<ClsJackCsv>();
            foreach(List<string> lstStr in lstlstStr)
            {
                if(bHeader) 
                {
                    bHeader = false;
                    continue;
                }

                ClsJackCsv cls = new ClsJackCsv();
                cls.Type = lstStr[0];
                cls.Size = lstStr[1];
                cls.Shuzai = lstStr[2];
                cls.TypeNameType = lstStr[3];
                cls.FamilyPath = lstStr[4];
                lstCls.Add(cls);
            }
            m_Data = lstCls;

            return true;
        }

        /// <summary>
        /// CSV情報を取得
        /// </summary>
        /// <returns></returns>
        public static List<ClsJackCsv> GetCsvData()
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
            if ((from data in m_Data where data.Type == type select data.Size).Any())
            {
                lst = (from data in m_Data where data.Type == type select data.Size).ToList();
            }
            return lst;
        }

        /// <summary>
        /// ファミリ名をキーにサイズを取得
        /// </summary>
        /// <returns></returns>
        public static string GetSize(string name)
        {
            GetCsv();
            string str = string.Empty;
            if ((from data in m_Data where data.FamilyPath.Contains(name) select data.Size).Any())
            {
                str = (from data in m_Data where data.FamilyPath.Contains(name) select data.Size).ToList().First();
            }
            return str;
        }
        /// <summary>
        /// サイズをキーに種別を取得
        /// </summary>
        /// <returns></returns>
        public static string GetType(string size)
        {
            GetCsv();
            string str = string.Empty;
            if ((from data in m_Data where data.Size == size select data.Type).Any())
            {
                str = (from data in m_Data where data.Size == size select data.Type).ToList().First();
            }
            return str;
        }
        /// <summary>
        /// サイズをキーにタイプ名の種類を取得
        /// </summary>
        /// <returns></returns>
        public static string GetTypeNameType(string size)
        {
            GetCsv();
            string str = string.Empty;
            if ((from data in m_Data where data.Size == size select data.TypeNameType).Any())
            {
                str = (from data in m_Data where data.Size == size select data.TypeNameType).ToList().First();
            }
            return str;
        }
        /// <summary>
        /// サイズをキーに対応主材を取得
        /// </summary>
        /// <returns></returns>
        public static string GetWidth(string size)
        {
            GetCsv();
            string str = string.Empty;
            if ((from data in m_Data where data.Size == size select data.Shuzai).Any())
            {
                str = (from data in m_Data where data.Size == size select data.Shuzai).ToList().First();
            }
            return str;
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

            string symbolFolpath = Parts.ClsZumenInfo.GetYMSFolder();
            string filePath = System.IO.Path.Combine(symbolFolpath, str);
            return filePath;
        }

        public static string GetJackSize(string type,string shuzai)
        {
            GetCsv();
            string str = string.Empty;
            var matchingData = m_Data.FirstOrDefault(data => data.Type == type && data.Shuzai == shuzai);
            if (matchingData != null)
            {
                return matchingData.Size;
            }
            else
            {
                return string.Empty; // マッチするデータが見つからない場合は空の文字列を返す
            }
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

    class ClsJackCoverCSV
    {
        /// <summary>
        /// CSV名称
        /// </summary>
        private const string CsvFileName = "JackCover.csv";

        /// <summary>
        /// 構造体のリスト
        /// </summary>
        private static List<ClsJackCoverCSV> m_Data = new List<ClsJackCoverCSV>();

        /// <summary>
        /// ジャッキカバーサイズ
        /// </summary>
        public string Size;

        /// <summary>
        /// 対応主材サイズ
        /// </summary>
        public string Shuzai;

        /// <summary>
        /// ファミリパス
        /// </summary>
        public string FamilyPath;

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
            List<ClsJackCoverCSV> lstCls = new List<ClsJackCoverCSV>();
            foreach (List<string> lstStr in lstlstStr)
            {
                if (bHeader)
                {
                    bHeader = false;
                    continue;
                }

                ClsJackCoverCSV cls = new ClsJackCoverCSV();
                cls.Size = lstStr[0];
                cls.Shuzai = lstStr[1];
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
        public static List<ClsJackCoverCSV> GetCsvData()
        {
            GetCsv();
            return m_Data;
        }

        /// <summary>
        /// ファミリのパスを取得する
        /// </summary>
        /// <param name="size">主材のサイズ</param>
        /// <returns></returns>
        public static string GetFamilyPath(string size)
        {
            GetCsv();
            string str = string.Empty;
            if ((from data in m_Data where data.Shuzai == size select data.FamilyPath).Any())
            {
                str = (from data in m_Data where data.Shuzai == size select data.FamilyPath).ToList().First();
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
    }
}
