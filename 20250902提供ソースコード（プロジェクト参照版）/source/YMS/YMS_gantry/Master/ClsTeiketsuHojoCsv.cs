using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YMS_gantry.Master
{
    class ClsTeiketsuHojoCsv:ClsMasterCsv
    {
        #region メンバ変数
        /// <summary>
        /// CSV名称
        /// </summary>
        private const string CsvFileName = "TeiketsuHojo.csv";

        /// <summary>
        /// メンバ変数
        /// </summary>
        private static List<ClsTeiketsuHojoCsv> m_Data = new List<ClsTeiketsuHojoCsv>();

        /// <summary>
        /// マスタ名称
        /// </summary>
        public const string MasterName = "締結補助材";

        /// <summary>
        /// 種類
        /// </summary>
        public const string TypeBulman = "ﾌﾞﾙﾏﾝ";
        public const string TypeRikiman = "ﾘｷﾏﾝ";
        public const string TypeNeko = "取付補助材";

        public const string TeiketsuC = "C";
        public const string TeiketsuG = "G";
        public const string TeiketsuLA = "LA";
        public const string TeiketsuNT = "NT";

        public const string HojoType_T = "ツナギ";
        public const string HojoType_B = "ブレス";

        /// <summary>
        /// 締結タイプ
        /// </summary>
        public string TeiketsuType { get; set; }
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
            List<ClsTeiketsuHojoCsv> lstCls = new List<ClsTeiketsuHojoCsv>();
            foreach (List<string> lstStr in lstlstStr)
            {
                if (bHeader)
                {
                    bHeader = false;
                    continue;
                }

                ClsTeiketsuHojoCsv cls = new ClsTeiketsuHojoCsv();
                cls.Type = lstStr[0];
                cls.Size = lstStr[1];
                cls.FamilyPath = lstStr[2];
                cls.TeiketsuType = lstStr[3];
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
        public static List<ClsTeiketsuHojoCsv> GetCsvData()
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

        /// <summary>
        /// サイズをキーにファミリのパスを取得
        /// </summary>
        /// <returns></returns>
        public static string GetFamilyPath(string type,string size)
        {
            GetCsv();
            string str = string.Empty;
            if ((from data in m_Data where data.Type==type&& data.Size == size select data.FamilyPath).Any())
            {
                str = (from data in m_Data where data.Type == type && data.Size == size select data.FamilyPath).ToList().First();
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
        /// サイズをキーにファミリのパスを取得
        /// </summary>
        /// <returns></returns>
        public static string GetTeiketsuType( string type,string size)
        {
            GetCsv();
            string retD ="";
            if ((from data in m_Data where data.Type == type && data.Size == size select data.TeiketsuType).Any())
            {
                retD = (from data in m_Data where data.Type == type && data.Size == size select data.TeiketsuType).ToList().First();
            }

            return retD;
        }
        #endregion

    }
}
