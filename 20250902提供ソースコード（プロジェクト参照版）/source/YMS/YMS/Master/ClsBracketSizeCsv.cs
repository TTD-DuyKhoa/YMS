using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YMS.Master
{
    internal class ClsBracketSizeCsv
    {
        #region メンバ変数

        /// <summary>
        /// CSV名称
        /// </summary>
        private const string CsvFileName = "BracketSize.csv";

        /// <summary>
        /// メンバ変数
        /// </summary>
        private static List<ClsBracketSizeCsv> m_Data = new List<ClsBracketSizeCsv>();

        #endregion

        #region プロパティ

        /// <summary>
        /// 腹起サイズ
        /// </summary>
        public string Size;

        /// <summary>
        /// 縦本数
        /// </summary>
        public int NumV;

        /// <summary>
        /// 横本数
        /// </summary>
        public int NumH;

        /// <summary>
        /// 押えブラケットか？
        /// </summary>
        public bool isOsae;

        /// <summary>
        /// 腹起ブラケットサイズ
        /// </summary>
        public string SizeBracket;

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
            List<ClsBracketSizeCsv> lstCls = new List<ClsBracketSizeCsv>();
            foreach (List<string> lstStr in lstlstStr)
            {
                if (bHeader)
                {
                    bHeader = false;
                    continue;
                }

                ClsBracketSizeCsv cls = new ClsBracketSizeCsv();
                cls.Size = lstStr[0];
                cls.NumV = int.Parse(lstStr[1]);
                cls.NumH = int.Parse(lstStr[2]);
                cls.isOsae = bool.Parse(lstStr[3]);
                cls.SizeBracket = lstStr[4];
                lstCls.Add(cls);
            }
            m_Data = lstCls;

            return true;
        }

        /// <summary>
        /// CSV情報を取得
        /// </summary>
        /// <returns></returns>
        public static List<ClsBracketSizeCsv> GetCsvData()
        {
            GetCsv();
            return m_Data;
        }

        /// <summary>
        /// 腹起ブラケットサイズを取得
        /// </summary>
        /// <param name="size">腹起サイズ</param>
        /// <param name="numV">縦本数</param>
        /// <param name="numH">横本数</param>
        /// <param name="isOsae">押えブラケット？</param>
        /// <returns></returns>
        public static string GetBracketSize(string size, int numV, int numH, bool isOsae = false)
        {
            GetCsv();
            string str = string.Empty;
            if ((from data in m_Data where data.Size == size && data.NumV == numV && data.NumH == numH && data.isOsae == isOsae select data.SizeBracket).Any())
            {
                str = (from data in m_Data where data.Size == size && data.NumV == numV && data.NumH == numH && data.isOsae == isOsae select data.SizeBracket).FirstOrDefault();
            }
            //if ((from data in m_Data where data.Size == size && data.NumV == numV && data.NumH == numH select data.SizeBracket).Any())
            //{
            //    str = (from data in m_Data where data.Size == size && data.NumV == numV && data.NumH == numH select data.SizeBracket).FirstOrDefault();
            //}

            return str;
        }

        #endregion
    }
}
