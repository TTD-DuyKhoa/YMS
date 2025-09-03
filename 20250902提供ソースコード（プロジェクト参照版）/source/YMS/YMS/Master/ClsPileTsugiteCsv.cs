using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YMS.Master
{
    public enum Kotei
    {
        Bolt,
        Yousetsu
    }

    public class ClsPileTsugiteCsv
    {
        #region メンバ変数
        /// <summary>
        /// CSV名称
        /// </summary>
        private const string CsvFileName = "PileTsugite.csv";

        /// <summary>
        /// メンバ変数
        /// </summary>
        private static List<ClsPileTsugiteCsv> m_Data = new List<ClsPileTsugiteCsv>();

        /// <summary>
        /// 固定方法
        /// </summary>
        public const string KoteiHohoBolt = "ボルト";
        public const string KoteiHohoYousetsu = "溶接";
        #endregion

        #region プロパティ
        /// <summary>
        /// 固定方法
        /// </summary>
        public string KoteiHoho;

        /// <summary>
        /// タイプ
        /// </summary>
        public string Type;

        /// <summary>
        /// サイズ
        /// </summary>
        public string Size;

        /// <summary>
        /// プレートサイズ（フランジ側：外側）
        /// </summary>
        public string PlateSizeFOut;

        /// <summary>
        /// プレート枚数（フランジ側：外側）
        /// </summary>
        public int PlateNumFOut;

        /// <summary>
        /// プレートサイズ（フランジ側：内側）
        /// </summary>
        public string PlateSizeFIn;

        /// <summary>
        /// プレート枚数（フランジ側：内側）
        /// </summary>
        public int PlateNumFIn;

        /// <summary>
        /// ボルトサイズ（フランジ側）
        /// </summary>
        public string BoltSizeF;

        /// <summary>
        /// ボルト個数（フランジ側）
        /// </summary>
        public int BoltNumF;

        /// <summary>
        /// プレートサイズ（WEB側）
        /// </summary>
        public string PlateSizeW;

        /// <summary>
        /// プレート枚数（WEB側）
        /// </summary>
        public int PlateNumW;

        /// <summary>
        /// プレートサイズ（WEB側2）
        /// </summary>
        public string PlateSizeW2;

        /// <summary>
        /// プレート枚数（WEB側2）
        /// </summary>
        public int PlateNumW2;

        /// <summary>
        /// ボルトサイズ（WEB側）
        /// </summary>
        public string BoltSizeW;

        /// <summary>
        /// ボルト個数（WEB側）
        /// </summary>
        public int BoltNumW;
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
            List<ClsPileTsugiteCsv> lstCls = new List<ClsPileTsugiteCsv>();
            foreach (List<string> lstStr in lstlstStr)
            {
                if (bHeader)
                {
                    bHeader = false;
                    continue;
                }

                ClsPileTsugiteCsv cls = new ClsPileTsugiteCsv();
                cls.KoteiHoho = lstStr[0];
                cls.Type = lstStr[1];
                cls.Size = lstStr[2];
                cls.PlateSizeFOut = lstStr[3];
                cls.PlateNumFOut = RevitUtil.ClsCommonUtils.ChangeStrToInt(lstStr[4]);
                cls.PlateSizeFIn = lstStr[5];
                cls.PlateNumFIn = RevitUtil.ClsCommonUtils.ChangeStrToInt(lstStr[6]);
                cls.BoltSizeF = lstStr[7];
                cls.BoltNumF = RevitUtil.ClsCommonUtils.ChangeStrToInt(lstStr[8]);
                cls.PlateSizeW = lstStr[9];
                cls.PlateNumW = RevitUtil.ClsCommonUtils.ChangeStrToInt(lstStr[10]);
                cls.PlateSizeW2 = lstStr[11];
                cls.PlateNumW2 = RevitUtil.ClsCommonUtils.ChangeStrToInt(lstStr[12]);
                cls.BoltSizeW = lstStr[13];
                cls.BoltNumW = RevitUtil.ClsCommonUtils.ChangeStrToInt(lstStr[14]);
                lstCls.Add(cls);
            }
            m_Data = lstCls;

            return true;
        }

        /// <summary>
        /// CSV情報を取得
        /// </summary>
        /// <returns></returns>
        public static List<ClsPileTsugiteCsv> GetCsvData()
        {
            GetCsv();
            return m_Data;
        }

        /// <summary>
        /// 固定方法のリストを取得
        /// </summary>
        /// <returns></returns>
        public static List<string> GetTypeList()
        {
            GetCsv();
            List<string> lst = new List<string>();
            if ((from data in m_Data select data.KoteiHoho).Any())
            {
                lst = (from data in m_Data select data.KoteiHoho).ToList();
            }
            return lst;
        }

        /// <summary>
        /// 固定方法をキーにサイズのリストを取得
        /// </summary>
        /// <returns></returns>
        public static List<string> GetSizeList(string koteihoho)
        {
            GetCsv();
            List<string> lst = new List<string>();
            if ((from data in m_Data where data.KoteiHoho == koteihoho select data.Size).Any())
            {
                lst = (from data in m_Data where data.KoteiHoho == koteihoho select data.Size).ToList();
            }
            return lst;
        }

        /// <summary>
        /// サイズをキーに鋼矢板継ぎCSVクラスを取得
        /// </summary>
        /// <returns></returns>
        public static ClsPileTsugiteCsv GetCls(string koteiHoho, string size)
        {
            GetCsv();
            ClsPileTsugiteCsv cls = new ClsPileTsugiteCsv();
            if ((from data in m_Data where data.KoteiHoho == koteiHoho && data.Size == size select data).Any())
            {
                cls = (from data in m_Data where data.KoteiHoho == koteiHoho && data.Size == size select data).ToList().First();
            }
            return cls;
        }
        #endregion
    }
}
