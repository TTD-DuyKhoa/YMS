using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YMS_gantry.Master
{
    class ClsStiffenerCsv:ClsMasterCsv
    {
        #region メンバ変数
        /// <summary>
        /// CSV名称
        /// </summary>
        private const string CsvFileName = "Stiffener.csv";

        /// <summary>
        /// メンバ変数
        /// </summary>
        private static List<ClsStiffenerCsv> m_Data = new List<ClsStiffenerCsv>();

        /// <summary>
        /// マスタ名称
        /// </summary>
        public const string MasterName = "スチフナー";

        /// <summary>
        /// 鋼材サイズ
        /// </summary>
        public string KouzaiSize { get; set; }

        /// <summary>
        /// タイプ名
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 種類
        /// </summary>
        public const string PL = "プレート";
        public const string SHJack = "SHジャッキ";
        public const string DWJJack = "DWJジャッキ";
        public const string Tensetsuban = "添接板";

        /// <summary>
        /// スチフナージャッキ長さ
        /// </summary>
        public double JackPLL { get; set; }
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
            List<ClsStiffenerCsv> lstCls = new List<ClsStiffenerCsv>();
            foreach (List<string> lstStr in lstlstStr)
            {
                if (bHeader)
                {
                    bHeader = false;
                    continue;
                }

                ClsStiffenerCsv cls = new ClsStiffenerCsv();
                cls.Type = lstStr[0];
                cls.Size = lstStr[1];
                cls.KouzaiSize = lstStr[2];
                cls.FamilyPath = lstStr[3];
                cls.TypeName = lstStr[4];
                cls.JackPLL = RevitUtil.ClsCommonUtils.ChangeStrToDbl(lstStr[5]);
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
        public static List<ClsStiffenerCsv> GetCsvData()
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

        public static string GetSize(string kouzaiSize,string type="プレート")
        {
            kouzaiSize = kouzaiSize.Replace("-", "");
            GetCsv();
            string retSt = "";
            if ((from data in m_Data where data.KouzaiSize == kouzaiSize&& data.Type==type select data.Size).Any())
            {
                retSt = (from data in m_Data where data.KouzaiSize == kouzaiSize && data.Type == type select data.Size).First();
            }

            return retSt;
        }

        public static string GetType(string kouzaiSize, string type = "プレート")
        {
            kouzaiSize = kouzaiSize.Replace("-", "");
            GetCsv();
            string retSt = "";
            if ((from data in m_Data where data.KouzaiSize == kouzaiSize && data.Type == type select data.Size).Any())
            {
                retSt = (from data in m_Data where data.KouzaiSize == kouzaiSize && data.Type == type select data.TypeName).First();
            }

            return retSt;
        }

        public static string GetTypeWithSize(string size, string type = "プレート")
        {
            GetCsv();
            string retSt = "";
            if ((from data in m_Data where data.Size == size && data.Type == type select data.Size).Any())
            {
                retSt = (from data in m_Data where data.Size == size && data.Type == type select data.TypeName).First();
            }

            return retSt;
        }


        public static string GetFamilyPath(string size, string type = "プレート")
        {
            GetCsv();
            string retSt = "";
            size = GetSize(size, type);
            if ((from data in m_Data where data.Size == size && data.Type == type select data.FamilyPath).Any())
            {
                retSt = (from data in m_Data where data.Size == size && data.Type == type select data.FamilyPath).First();
            }

            string symbolFolpath = PathUtil.GetYMSFolder();
            retSt = System.IO.Path.Combine(symbolFolpath, retSt);

            if (!System.IO.File.Exists(retSt))
            {
                //MessageBox.Show("ファミリが存在しません。\n" + filePath);
                retSt = string.Empty;
            }

            return retSt;
        }
        public static string GetFamilyPath(string size)
        {
            GetCsv();
            string retSt = "";
            if ((from data in m_Data where data.Size == size  select data.FamilyPath).Any())
            {
                retSt = (from data in m_Data where data.Size == size  select data.FamilyPath).First();
            }

            string symbolFolpath = PathUtil.GetYMSFolder();
            retSt = System.IO.Path.Combine(symbolFolpath, retSt);

            if (!System.IO.File.Exists(retSt))
            {
                //MessageBox.Show("ファミリが存在しません。\n" + filePath);
                retSt = string.Empty;
            }

            return retSt;
        }

        public static string GetFamilyType(string size)
        {
            GetCsv();
            string retSt = "";
            if ((from data in m_Data where data.Size == size select data.TypeName).Any())
            {
                retSt = (from data in m_Data where data.Size == size select data.TypeName).ToList().First();
            }

            return retSt;
        }
        #endregion

    }
}
