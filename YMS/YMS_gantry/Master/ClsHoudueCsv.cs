using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YMS_gantry.Master
{
    class ClsHoudueCsv:ClsMasterCsv
    {
        #region メンバ変数
        /// <summary>
        /// CSV名称
        /// </summary>
        private const string CsvFileName = "Houdue.csv";

        /// <summary>
        /// メンバ変数
        /// </summary>
        private static List<ClsHoudueCsv> m_Data = new List<ClsHoudueCsv>();

        /// <summary>
        /// マスタ名称
        /// </summary>
        public const string MasterName = "方杖";

        /// <summary>
        /// 種類
        /// </summary>
        public const string TypeHiuchiUke = "火打受ピース";
        public const string TypeSumibu = "隅部ピース";
        public const string TypeSozai = "素材";
        public const string TypeNeko = "取付補助材";

        /// <summary>
        /// 火打ち受けピースの中心部長
        /// </summary>
        public double HiuchiLengh { get; set; }
        /// <summary>
        /// タイプ名
        /// </summary>
        public string FamilyType { get; set; }
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

            string symbolFolpath = PathUtil.GetYMS_GantryMasterPath();
            string fileName = System.IO.Path.Combine(symbolFolpath, CsvFileName);
            List<List<string>> lstlstStr = new List<List<string>>();
            if (!RevitUtil.ClsCommonUtils.ReadCsv(fileName, ref lstlstStr))
            {
                MessageBox.Show("CSVファイルの取得に失敗しました。：" + fileName);
                return false;
            }

            bool bHeader = true;
            List<ClsHoudueCsv> lstCls = new List<ClsHoudueCsv>();
            foreach (List<string> lstStr in lstlstStr)
            {
                if (bHeader)
                {
                    bHeader = false;
                    continue;
                }

                ClsHoudueCsv cls = new ClsHoudueCsv();
                cls.Type = lstStr[0];
                cls.Size = lstStr[1];
                cls.FamilyPath = lstStr[2];
                cls.HiuchiLengh = RevitUtil.ClsCommonUtils.ChangeStrToDbl(lstStr[3]);
                cls.FamilyType = lstStr[4];
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
        public static List<ClsHoudueCsv> GetCsvData()
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
            string str = string.Empty;
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

        public static string GetFamilyPath(string size, string type)
        {
            GetCsv();
            string retSt = "";
            if ((from data in m_Data where data.Type == type && data.Size == size select data.FamilyPath).Any())
            {
                retSt = (from data in m_Data where data.Type == type && data.Size == size select data.FamilyPath).ToList().First();
            }

            string symbolFolpath = PathUtil.GetYMSFolder();
            string filePath = System.IO.Path.Combine(symbolFolpath, retSt);

            if (!System.IO.File.Exists(filePath))
            {
                //MessageBox.Show("ファミリが存在しません。\n" + filePath);
                filePath = string.Empty;
            }

            return filePath;
        }


        /// <summary>
        /// サイズをキーにファミリのパスを取得
        /// </summary>
        /// <returns></returns>
        public static double GetHiuchiLehg(string size)
        {
            GetCsv();
            double retD = 0;
            if ((from data in m_Data where data.Size == size select data.HiuchiLengh).Any())
            {
                retD = (from data in m_Data where data.Size == size select data.HiuchiLengh).ToList().First();
            }

            return retD;
        }

        /// <summary>
        /// サイズをキーにファミリのタイプを取得
        /// </summary>
        /// <returns></returns>
        public static string GetFamilyType(string size)
        {
            GetCsv();
            string retSt = "";
            if ((from data in m_Data where data.Size == size select data.FamilyType).Any())
            {
                retSt = (from data in m_Data where data.Size == size select data.FamilyType).ToList().First();
            }

            return retSt;
        }
        #endregion

    }
}
