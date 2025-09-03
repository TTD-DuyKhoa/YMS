using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YMS_gantry.Master
{
    class ClsWaritsukeCsv:ClsMasterCsv
    {
        #region メンバ変数
        /// <summary>
        /// CSV名称
        /// </summary>
        private const string CsvFileName = "Waritsuke.csv";

        /// <summary>
        /// メンバ変数
        /// </summary>
        private static List<ClsWaritsukeCsv> m_Data = new List<ClsWaritsukeCsv>();

        /// <summary>
        /// マスタ名称
        /// </summary>
        public const string MasterName = "割付";

        /// <summary>
        /// 対象となる部材名
        /// </summary>
      　public string targetMaterials { get; set; }

        /// <summary>
        /// 支柱パス
        /// </summary>
        public string shichuFamilyPath { get; set; }

        /// <summary>
        /// 長さ
        /// 
        /// </summary>
        public string length { get; set; }

        /// <summary>
        /// 種類
        /// </summary>
        //public const string Neda = "根太";
        //public const string Ohbiki = "大引";
        //public const string Ketauke = "桁受";
        //public const string Shikiketa = "敷桁";
        //public const string Fukouketa = "覆工桁";
        //public const string Jifuku = "地覆";
        //public const string Tsunagi = "ﾂﾅｷﾞ";
        //public const string Tesuri = "手摺";
        //public const string Houdue = "方杖";
        //public const string Sichu = "支柱";
        //public const string Plate = "プレート";
        //public const string Jack = "ジャッキ";
        //public const string Pieace = "補助ピース";
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
            List<ClsWaritsukeCsv> lstCls = new List<ClsWaritsukeCsv>();
            foreach (List<string> lstStr in lstlstStr)
            {
                if (bHeader)
                {
                    bHeader = false;
                    continue;
                }

                ClsWaritsukeCsv cls = new ClsWaritsukeCsv();
                cls.Type = lstStr[0];
                cls.Size = lstStr[1];
                cls.targetMaterials = lstStr[2];
                cls.FamilyPath = lstStr[3];
                cls.shichuFamilyPath = lstStr[4];
                cls.length = lstStr[5];
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
        public static List<ClsWaritsukeCsv> GetCsvData()
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
        public static List<string> GetSizeList(string type,string materialSize="")
        {
            GetCsv();
            List<string> lst = new List<string>();
            if(materialSize!=""&&materialSize!=null)
            {
                if ((from data in m_Data where data.Type == type && data.targetMaterials.Contains(materialSize) select data.Size).Any())
                {
                    lst = (from data in m_Data where data.Type == type && data.targetMaterials.Contains(materialSize) select data.Size).ToList();
                }
            }
            else
            {
                if ((from data in m_Data where data.Type == type select data.Size).Any())
                {
                    lst = (from data in m_Data where data.Type == type select data.Size).ToList();
                }
            }
            return lst;
        }

        public static string GetFamilyPath(string size, string type)
        {
            GetCsv();
            string retSt = "";
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

        public static string GetShicuFamilyPath(string size, string type)
        {
            GetCsv();
            string retSt = "";
            if ((from data in m_Data where data.Size == size && data.Type == type select data.shichuFamilyPath).Any())
            {
                retSt = (from data in m_Data where data.Size == size && data.Type == type select data.shichuFamilyPath).First();
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

        public static string GetLength(string size, string type)
        {
            GetCsv();
            string retSt = "";
            if ((from data in m_Data where data.Size == size && data.Type == type select data.length).Any())
            {
                retSt = (from data in m_Data where data.Size == size && data.Type == type select data.length).First();
            }

            return retSt;
        }

        #endregion

    }
}
