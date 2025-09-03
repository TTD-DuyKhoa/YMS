using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YMS_gantry.Master
{
    class ClsTakasaChouseiCsv : ClsMasterCsv
    {
        #region メンバ変数
        /// <summary>
        /// CSV名称
        /// </summary>
        private const string CsvFileName = "TakasaChousei.csv";

        /// <summary>
        /// メンバ変数
        /// </summary>
        private static List<ClsTakasaChouseiCsv> m_Data = new List<ClsTakasaChouseiCsv>();

        /// <summary>
        /// マスタ名称
        /// </summary>
        public const string MasterName = "高さ調整";

        /// <summary>
        /// 種類
        /// </summary>
        public const string TypePlate = "プレート";
        public const string TypePieace = "ピース";
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
            List<ClsTakasaChouseiCsv> lstCls = new List<ClsTakasaChouseiCsv>();
            foreach (List<string> lstStr in lstlstStr)
            {
                if (bHeader)
                {
                    bHeader = false;
                    continue;
                }

                ClsTakasaChouseiCsv　cls = new ClsTakasaChouseiCsv();
                cls.Type = lstStr[0];
                cls.Size = lstStr[1];
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
        [YmsMasterLoad]
        public static List<ClsTakasaChouseiCsv> GetCsvData()
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
        #endregion

    }
}
