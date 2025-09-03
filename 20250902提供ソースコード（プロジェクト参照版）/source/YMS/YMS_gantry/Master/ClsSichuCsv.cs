using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YMS_gantry.Master
{
    class ClsSichuCsv:ClsMasterCsv
    {
        #region メンバ変数
        /// <summary>
        /// CSV名称
        /// </summary>
        private const string CsvFileName = "Sichu.csv";

        /// <summary>
        /// メンバ変数
        /// </summary>
        private static List<ClsSichuCsv> m_Data = new List<ClsSichuCsv>();

        /// <summary>
        /// マスタ名称
        /// </summary>
        public const string MasterName = "支柱";

        public const string HKou = "H形鋼 広幅";
        public const string Yamadome = "山留材";
        public const string Koukyoudo = "高強度山留材";

        public string BPLSaikyou { get; set; }
        public string BPLHyoujun { get; set; }
        public string BPLKani { get; set; }
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
            List<ClsSichuCsv> lstCls = new List<ClsSichuCsv>();
            foreach (List<string> lstStr in lstlstStr)
            {
                if (bHeader)
                {
                    bHeader = false;
                    continue;
                }

                var k = 0;
                ClsSichuCsv cls = new ClsSichuCsv
                {
                    Type = lstStr[k++],
                    Size = lstStr[k++],
                    FamilyPath = lstStr[k++],
                    BPLSaikyou= lstStr[k++],
                    BPLHyoujun= lstStr[k++],
                    BPLKani= lstStr[k++]
                };
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
        public static List<ClsSichuCsv> GetCsvData()
        {
            GetCsv();
            return m_Data;
        }

        /// <summary>
        /// 種別をキーにサイズのリストを取得
        /// </summary>
        /// <returns></returns>
        public static List<string> GetSizeList(string type)
        {
            GetCsv();
            List<string> lst = new List<string>();
            if ((from data in m_Data where data.Type==type select data.Size).Any())
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
        public static string GetBPLData(string size,string BPLType)
        {
            GetCsv();
            string str = string.Empty;
            if(BPLType=="最強型")
            {
                if ((from data in m_Data where data.Size == size select data.BPLSaikyou).Any())
                {
                    str = (from data in m_Data where data.Size == size select data.BPLSaikyou).ToList().First();
                }

            }
            else if(BPLType=="標準型")
            {
                if ((from data in m_Data where data.Size == size select data.BPLHyoujun).Any())
                {
                    str = (from data in m_Data where data.Size == size select data.BPLHyoujun).ToList().First();
                }
            }
            else
            {
                if ((from data in m_Data where data.Size == size select data.BPLKani).Any())
                {
                    str = (from data in m_Data where data.Size == size select data.BPLKani).ToList().First();
                }
            }
           
            return str;
        }
        #endregion
    }
}
