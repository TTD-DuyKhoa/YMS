using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YMS_gantry.Master
{
    class ClsKouzaiSpecify:ClsMasterCsv
    {
        #region メンバ変数
        /// <summary>
        /// CSV名称
        /// </summary>
        private const string CsvFileName = "YamadomeZaiSize.csv";

        /// <summary>
        /// メンバ変数
        /// </summary>
        private static List<ClsKouzaiSpecify> m_Data = new List<ClsKouzaiSpecify>();

        /// <summary>
        /// マスタ名称
        /// </summary>
        public const string MasterName = "主材";

        /// <summary>
        /// 種類
        /// </summary>
        public const string TypeShuzai = "山留主材";
        public const string TypeHighShuzai = "高強度山留主材";
        public const string TypePieace = "補助ピース";
        public const string TypeHighPieace = "高強度補助ピース";
        #endregion

        #region プロパティ
        /// <summary>
        /// 長さ
        /// </summary>
        public string KouzaiSize;

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
            List< ClsKouzaiSpecify> lstCls = new List<ClsKouzaiSpecify>();
            foreach (List<string> lstStr in lstlstStr)
            {
                if (bHeader)
                {
                    bHeader = false;
                    continue;
                }

                ClsKouzaiSpecify cls = new ClsKouzaiSpecify();
                cls.Type = lstStr[0];
                cls.Size = lstStr[1];
                cls.KouzaiSize = lstStr[2];
                lstCls.Add(cls);
            }
            m_Data = lstCls;

            return true;
        }

        /// <summary>
        /// CSV情報を取得
        /// </summary>
        /// <returns></returns>
        public static List<ClsKouzaiSpecify> GetCsvData()
        {
            GetCsv();
            return m_Data;
        }

        /// <summary>
        /// 種別のリストを取得
        /// </summary>
        /// <returns></returns>
        public static List<string> GetTypeList(bool bHigh = true, bool pieace = true, bool highPieace = true)
        {
            GetCsv();
            List<string> lst = new List<string>();
            if ((from data in m_Data select data.Type).Any())
            {
                lst = (from data in m_Data select data.Type).Distinct().ToList();

                if (bHigh == false)
                {
                    lst = (from data in lst where data != TypeHighShuzai select data).Distinct().ToList();
                }
                if (pieace == false)
                {
                    lst = (from data in lst where data != TypePieace select data).Distinct().ToList();
                }
                if (highPieace == false)
                {
                    lst = (from data in lst where data != TypeHighPieace select data).Distinct().ToList();
                }
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
                lst = (from data in m_Data where data.Type == type select data.Size).Distinct().ToList();
            }

            //ファミリ存在チェック
            lst = CheckExist(lst, 4.0); //4mがあれば存在しているとする

            return lst;
        }

        /// <summary>
        /// サイズをキーに鋼材名を取得
        /// </summary>
        /// <returns></returns>
        public static string GetKouzaiSize(string size)
        {
            GetCsv();
            string str = string.Empty;
            if ((from data in m_Data where data.Size == size select data.KouzaiSize).Any())
            {
                str = (from data in m_Data where data.Size == size select data.KouzaiSize).ToList().First();
            }

            return str;
        }

        /// <summary>
        /// 鋼材サイズをキーに山留仮鋼材名を取得
        /// </summary>
        /// <returns></returns>
        public static string GetYamadomeKarikouzai(string kouzaiSize)
        {
            string result = string.Empty;
            if (string.IsNullOrWhiteSpace(kouzaiSize)) return result;

            GetCsv();
            if ((from data in m_Data where data.KouzaiSize.IndexOf(kouzaiSize, StringComparison.OrdinalIgnoreCase) >= 0 select data.Size).Any())
            {
                result = (from data in m_Data where data.KouzaiSize.IndexOf(kouzaiSize, StringComparison.OrdinalIgnoreCase) >= 0 select data.Size).ToList().First();
            }

            return result;
        }
        #endregion
    }
}
