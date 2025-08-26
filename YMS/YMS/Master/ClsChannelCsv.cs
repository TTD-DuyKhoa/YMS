using RevitUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YMS.Master
{
    class ClsChannelCsv
    {
        #region メンバ変数
        /// <summary>
        /// CSV名称
        /// </summary>
        private const string CsvFileName = "Channel.csv";

        /// <summary>
        /// メンバ変数
        /// </summary>
        private static List<ClsChannelCsv>  m_Data = new List<ClsChannelCsv>();

        /// <summary>
        /// 種類
        /// </summary>
        public const string TypeCannel = "チャンネル";
        #endregion

        #region プロパティ
        /// <summary>
        /// サイズ
        /// </summary>
        public string Size;

        /// <summary>
        /// ファミリパス
        /// </summary>
        public string FamilyPath;

        /// <summary>
        /// 頭繋ぎフラグ
        /// </summary>
        public bool bAtamaTsunagi;
        #endregion

        #region メソッド
        /// <summary>
        /// CSVから情報を取得
        /// </summary>
        /// <returns></returns>
        public static bool GetCsv()
        {
            if(m_Data != null && m_Data.Count != 0)
            {
                return true;
            }

            string symbolFolpath = Parts.ClsZumenInfo.GetYMSFolder();
            string fileName = System.IO.Path.Combine(symbolFolpath, Parts.ClsZumenInfo.master, CsvFileName);
            List<List<string>> lstlstStr = new List<List<string>>();
            if(!RevitUtil.ClsCommonUtils.ReadCsv(fileName, ref lstlstStr))
            {
                MessageBox.Show("CSVファイルの取得に失敗しました。：" + fileName);
                return false;
            }

            bool bHeader = true;
            List<ClsChannelCsv> lstCls = new List<ClsChannelCsv>();
            foreach(List<string> lstStr in lstlstStr)
            {
                if(bHeader) 
                {
                    bHeader = false;
                    continue;
                }

                ClsChannelCsv cls = new ClsChannelCsv();
                cls.Size = lstStr[0];
                cls.bAtamaTsunagi = RevitUtil.ClsCommonUtils.ChangeStrToBool(lstStr[1]);
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
        public static List<ClsChannelCsv> GetCsvData()
        {
            GetCsv();
            return m_Data;
        }

        /// <summary>
        /// 種別をキーにサイズのリストを取得
        /// </summary>
        /// <returns></returns>
        public static List<string> GetSizeList(bool bAtamaTsunagi = false)
        {
            GetCsv();
            List<string> lst = new List<string>();
            if(bAtamaTsunagi)
            {
                if ((from data in m_Data where data.bAtamaTsunagi == true select data.Size).Any())
                {
                    lst = (from data in m_Data where data.bAtamaTsunagi == true select data.Size).ToList();
                }
            }
            else
            {
                if ((from data in m_Data select data.Size).Any())
                {
                    lst = (from data in m_Data select data.Size).ToList();
                }
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
            string  str = string.Empty;
            if ((from data in m_Data where data.Size == size select data.FamilyPath).Any())
            {
                str = (from data in m_Data where data.Size == size select data.FamilyPath).ToList().First();
            }

            string symbolFolpath = Parts.ClsZumenInfo.GetYMSFolder();
            string filePath = System.IO.Path.Combine(symbolFolpath, str);
            return filePath;
        }
        /// <summary>
        /// ファミリの名前Listを取得
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFamilyNameList()
        {
            GetCsv();
            List<string> kariNameList = new List<string>();
            foreach (var data in m_Data)
            {
                string filePath = data.FamilyPath;
                string familyName = ClsRevitUtil.GetFamilyName(filePath);
                kariNameList.Add(familyName);
            }

            return kariNameList;
        }
        #endregion
    }
}
