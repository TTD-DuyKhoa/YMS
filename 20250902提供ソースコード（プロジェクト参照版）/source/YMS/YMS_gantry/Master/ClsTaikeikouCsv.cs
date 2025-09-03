using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YMS_gantry.Master
{
    class ClsTaikeikouCsv:ClsMasterCsv
    {
        #region メンバ変数
        /// <summary>
        /// CSV名称
        /// </summary>
        private const string CsvFileName = "Taikeikou.csv";

        /// <summary>
        /// メンバ変数
        /// </summary>
        private static List<ClsTaikeikouCsv> m_Data = new List<ClsTaikeikouCsv>();

        /// <summary>
        /// マスタ名称
        /// </summary>
        public const string MasterName = "対傾構";

        /// <summary>
        /// 定型サイズ
        /// </summary>
        public string TeikeiSize { get; set; }

        /// <summary>
        /// 定型サイズ
        /// </summary>
        public string KouzaiSize { get; set; }

        /// <summary>
        /// 種類
        /// </summary>
        public const string PL = "プレート";
        public const string Jack = "ジャッキ";

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
            List<ClsTaikeikouCsv> lstCls = new List<ClsTaikeikouCsv>();
            foreach (List<string> lstStr in lstlstStr)
            {
                if (bHeader)
                {
                    bHeader = false;
                    continue;
                }

                ClsTaikeikouCsv cls = new ClsTaikeikouCsv();
                cls.TeikeiSize = lstStr[0];
                cls.Size = lstStr[1];
                cls.FamilyPath = lstStr[2];
                cls.KouzaiSize = lstStr[3];
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
        public static List<ClsTaikeikouCsv> GetCsvData()
        {
            GetCsv();
            return m_Data;
        }

        /// <summary>
        /// 種別をキーにサイズのリストを取得
        /// </summary>
        /// <returns></returns>
        public static List<string> GetSizeList(string teikeiSize)
        {
            GetCsv();
            List<string> lst = new List<string>();
            if ((from data in m_Data where data.TeikeiSize == teikeiSize select data.Size).Any())
            {
                lst = (from data in m_Data where data.TeikeiSize == teikeiSize select data.Size).ToList();
            }
            //ファミリ存在チェック
            lst = CheckExist(lst);
            return lst;
        }


        /// <summary>
        /// すべてのサイズのリストを取得
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllSizeList()
        {
            GetCsv();
            List<string> lst = new List<string>();
            if ((from data in m_Data  select data.Size).Any())
            {
                lst = (from data in m_Data  select data.Size).ToList();
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

        /// <summary>
        /// サイズをキーにファミリのパスを取得
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
        #endregion


    }

    class ClsTaikeikouPLCsv : ClsMasterCsv
    {
        #region メンバ変数
        /// <summary>
        /// CSV名称
        /// </summary>
        private const string CsvFileName = "TaikeikouPL.csv";

        /// <summary>
        /// メンバ変数
        /// </summary>
        private static List<ClsTaikeikouPLCsv> m_Data = new List<ClsTaikeikouPLCsv>();

        /// <summary>
        /// マスタ名称
        /// </summary>
        public const string MasterName = "対傾構取付PL";

        /// <summary>
        /// タイプ名
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 厚み
        /// </summary>
        public double Thick { get; set; }
        /// <summary>
        /// プレートサイズ
        /// </summary>
        public string PlSize { get; set; }
        /// <summary>
        /// スチフナーパス
        /// </summary>
        public string StiffnerPath { get; set; }
        /// <summary>
        /// スチフナータイプ
        /// </summary>
        public string StiffnerTypeName { get; set; }
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
            List<ClsTaikeikouPLCsv> lstCls = new List<ClsTaikeikouPLCsv>();
            foreach (List<string> lstStr in lstlstStr)
            {
                if (bHeader)
                {
                    bHeader = false;
                    continue;
                }

                ClsTaikeikouPLCsv cls = new ClsTaikeikouPLCsv();
                cls.Size = lstStr[0];
                cls.FamilyPath = lstStr[1];
                cls.TypeName = lstStr[2];
                cls.StiffnerPath = lstStr[3];
                cls.StiffnerTypeName =lstStr[4];
                cls.Thick = RevitUtil.ClsCommonUtils.ChangeStrToDbl(lstStr[5]);
                cls.PlSize = lstStr[6];
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
        public static List<ClsTaikeikouPLCsv> GetCsvData()
        {
            GetCsv();
            return m_Data;
        }

        /// <summary>
        /// すべてのサイズのリストを取得
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllSizeList()
        {
            GetCsv();
            List<string> lst = new List<string>();
            if ((from data in m_Data select data.Size).Any())
            {
                lst = (from data in m_Data select data.Size).ToList();
            }
            //ファミリ存在チェック
            lst = CheckExist(lst);
            return lst;
        }

        /// <summary>
        /// すべてのサイズ(PL)のリストを取得
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllPLSizeList()
        {
            GetCsv();
            List<string> lst = new List<string>();
            if ((from data in m_Data select data.PlSize).Any())
            {
                lst = (from data in m_Data select data.PlSize).ToList();
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
            if ((from data in m_Data where (data.Size == size || data.PlSize == size) select data.FamilyPath).Any())
            {
                str = (from data in m_Data where (data.Size == size || data.PlSize == size) select data.FamilyPath).ToList().First();
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
        public static string GetFamilyTypeName(string size)
        {
            GetCsv();
            string str = string.Empty;
            if ((from data in m_Data where data.Size == size select data.TypeName).Any())
            {
                str = (from data in m_Data where data.Size == size select data.TypeName).ToList().First();
            }

            return str;
        }

        /// <summary>
        /// サイズをキーにファミリのパスを取得
        /// </summary>
        /// <returns></returns>
        public static double GetThick(string size)
        {
            GetCsv();
            double thick = 12;
            if ((from data in m_Data where data.Size == size select data.Thick).Any())
            {
               thick = (from data in m_Data where data.Size == size select data.Thick).ToList().First();
            }

            return thick;
        }



        /// <summary>
        /// サイズをキーにファミリのパスを取得
        /// </summary>
        /// <returns></returns>
        public static string GetStiffnerFamilyPath(string size)
        {
            GetCsv();
            string str = string.Empty;
            if ((from data in m_Data where (data.Size == size || data.PlSize == size) select data.StiffnerPath).Any())
            {
                str = (from data in m_Data where (data.Size == size || data.PlSize == size) select data.StiffnerPath).ToList().First();
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
        public static string GetStiffnerFamilyTypeName(string size)
        {
            GetCsv();
            string str = string.Empty;
            if ((from data in m_Data where data.Size == size select data.StiffnerTypeName).Any())
            {
                str = (from data in m_Data where data.Size == size select data.StiffnerTypeName).ToList().First();
            }

            return str;
        }
        #endregion


    }
}
