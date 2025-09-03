using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YMS.Master
{
    public class ClsSendanBoltHokyouzaiCsv
    {
        /// <summary>
        /// CSV名称
        /// </summary>
        private const string CsvFileName = "SendanBoltHokyouzai.csv";

        /// <summary>
        /// メンバ変数
        /// </summary>
        private static List<ClsSendanBoltHokyouzaiCsv> m_Data = new List<ClsSendanBoltHokyouzaiCsv>();

        /// <summary>
        /// サイズ
        /// </summary>
        public string Size;

        /// <summary>
        /// ファミリパス
        /// </summary>
        public string FamilyPath;

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
            List<ClsSendanBoltHokyouzaiCsv> lstCls = new List<ClsSendanBoltHokyouzaiCsv>();
            foreach (List<string> lstStr in lstlstStr)
            {
                if (bHeader)
                {
                    bHeader = false;
                    continue;
                }

                ClsSendanBoltHokyouzaiCsv cls = new ClsSendanBoltHokyouzaiCsv();
                cls.Size = lstStr[0];
                cls.FamilyPath = lstStr[1];
                lstCls.Add(cls);
            }
            m_Data = lstCls;

            return true;
        }

        /// <summary>
        /// CSV情報を取得
        /// </summary>
        /// <returns></returns>
        public static List<ClsSendanBoltHokyouzaiCsv> GetCsvData()
        {
            GetCsv();
            return m_Data;
        }

        /// <summary>
        /// サイズのリストを取得
        /// </summary>
        /// <returns></returns>
        public static List<string> GetSizeList()
        {
            GetCsv();
            List<string> lst = new List<string>();
            if ((from data in m_Data select data.Size).Any())
            {
                lst = (from data in m_Data select data.Size).ToList();
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

            string symbolFolpath = Parts.ClsZumenInfo.GetYMSFolder();
            string filePath = System.IO.Path.Combine(symbolFolpath, str);
            return filePath;
        }

    }
}
