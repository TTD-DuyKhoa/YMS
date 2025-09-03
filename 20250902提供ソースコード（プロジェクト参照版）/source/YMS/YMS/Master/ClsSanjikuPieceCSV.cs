using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YMS.Master
{
    class ClsSanjikuPieceCSV
    {
        #region 定数
        /// <summary>
        /// CSV名称
        /// </summary>
        private const string CsvFileName = "SanjikuPiece.csv";
        #endregion

        #region メンバ変数
        /// <summary>
        /// メンバ変数
        /// </summary>
        private static List<ClsSanjikuPieceCSV> m_Data = new List<ClsSanjikuPieceCSV>();
        #endregion

        #region プロパティ
        /// <summary>
        /// サイズ
        /// </summary>
        public string Size;

        /// <summary>
        /// 対象主材
        /// </summary>
        public string Target;

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
            List<ClsSanjikuPieceCSV> lstCls = new List<ClsSanjikuPieceCSV>();
            foreach (List<string> lstStr in lstlstStr)
            {
                if (bHeader)
                {
                    bHeader = false;
                    continue;
                }

                ClsSanjikuPieceCSV cls = new ClsSanjikuPieceCSV();
                cls.Size = lstStr[0];
                cls.Target = lstStr[1];
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
        public static List<ClsSanjikuPieceCSV> GetCsvData()
        {
            GetCsv();
            return m_Data;
        }

        /// <summary>
        /// サイズのリストを取得
        /// </summary>
        /// <returns></returns>
        public static List<string> GetSizeList()
        {
            GetCsv();
            List<string> lst = new List<string>();
            if ((from data in m_Data select data.Size).Any())
            {
                lst = (from data in m_Data select data.Size).ToList();
            }
            return lst;
        }
        /// <summary>
        /// 対象主材名称をキーにファミリのパスを取得
        /// </summary>
        /// <returns></returns>
        public static string GetSize(string targetSyuzaiName)
        {
            GetCsv();
            string str = string.Empty;
            if ((from data in m_Data where data.Target == targetSyuzaiName select data.Size).Any())
            {
                str = (from data in m_Data where data.Target == targetSyuzaiName select data.Size).ToList().First();
            }

            return str;
        }
        /// <summary>
        /// 対象主材名称をキーにファミリのパスを取得
        /// </summary>
        /// <returns></returns>
        public static string GetFamilyPath(string targetSyuzaiName)
        {
            GetCsv();
            string str = string.Empty;
            if ((from data in m_Data where data.Target == targetSyuzaiName select data.FamilyPath).Any())
            {
                str = (from data in m_Data where data.Target == targetSyuzaiName select data.FamilyPath).ToList().First();
            }

            string symbolFolpath = Parts.ClsZumenInfo.GetYMSFolder();
            string filePath = System.IO.Path.Combine(symbolFolpath, str);
            return filePath;
        }

        /// <summary>
        /// ファイル名リストを取得
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFamilyNameList()
        {
            GetCsv();
            List<string> nameList = new List<string>();
            foreach (var data in m_Data)
            {
                string filePath = data.FamilyPath;
                string familyName = RevitUtil.ClsRevitUtil.GetFamilyName(filePath);
                nameList.Add(familyName);
            }

            return nameList;
        }

        /// <summary>
        /// 三軸の横についている部品の中心からの長さ取得
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static double GetSideLength(string size)
        {
            switch(size)//マスタに定義した方が良いかは検討
            {
                case "30SHP":
                    return 800.0;
                case "35SHP":
                    return 850.0;
                case "40SHP":
                    return 1000.0;
                case "50SHP-N":
                    return 1100.0;
                default:
                    return 0.0;
            }
        }
        #endregion

    }
}
