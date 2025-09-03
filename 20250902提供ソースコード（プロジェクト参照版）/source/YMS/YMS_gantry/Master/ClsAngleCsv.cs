using RevitUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YMS_gantry.Master
{
    public class ClsAngleCsv : ClsMasterCsv
    {
        #region メンバ変数
        /// <summary>
        /// CSV名称
        /// </summary>
        private const string CsvFileName = "Angle.csv";

        /// <summary>
        /// メンバ変数
        /// </summary>
        private static List<ClsAngleCsv> m_Data = new List<ClsAngleCsv>();

        /// <summary>
        /// マスタ名称
        /// </summary>
        public const string MasterName = "L材";

        /// <summary>
        /// 種類
        /// </summary>
        public const string TypeAngle = "アングル";
        public const string TypeFutohenAngle = "不等辺アングル";
        #endregion

        public double A { get; set; }
        public double B { get; set; }
        public double T { get; set; }

        public double AFt => ClsRevitUtil.CovertToAPI(A);
        public double BFt => ClsRevitUtil.CovertToAPI(B);
        public double TFt => ClsRevitUtil.CovertToAPI(T);

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
            List<ClsAngleCsv> lstCls = new List<ClsAngleCsv>();
            foreach (List<string> lstStr in lstlstStr)
            {
                if (bHeader)
                {
                    bHeader = false;
                    continue;
                }

                var index = 0;
                ClsAngleCsv cls = new ClsAngleCsv
                {
                    Type = lstStr[index++],
                    Size = lstStr[index++],
                    FamilyPath = lstStr[index++],
                    HrzJoint = lstStr.ElementAtOrDefault(index++),
                    IsBrace = Atob(lstStr.ElementAtOrDefault(index++)),
                    A = Atof(lstStr.ElementAtOrDefault(index++)),
                    B = Atof(lstStr.ElementAtOrDefault(index++)),
                    T = Atof(lstStr.ElementAtOrDefault(index++)),
                    MadumeSymbol = lstStr.ElementAtOrDefault(index++),
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
        public static List<ClsAngleCsv> GetCsvData()
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
        /// サイズの中に指定した値が存在するか確認
        /// </summary>
        /// <returns>サイズを返却(存在しない場合は空文字を返却)</returns>
        public static string ExistSize(string size)
        {
            if (string.IsNullOrWhiteSpace(size)) return string.Empty;

            GetCsv();
            string result = string.Empty;
            if ((from data in m_Data where data.Size.IndexOf(size, StringComparison.OrdinalIgnoreCase) >= 0 select data.Size).Any())
            {
                result = (from data in m_Data where data.Size.IndexOf(size, StringComparison.OrdinalIgnoreCase) >= 0 select data.Size).ToList().First();
            }

            return result;
        }

        /// <summary>
        /// サイズから種別を特定する
        /// </summary>
        /// <param name="size">サイズ</param>
        /// <returns>種別</returns>
        public static string GetTypeBySize(string size)
        {
            if (string.IsNullOrWhiteSpace(size)) return string.Empty;

            GetCsv();
            string result = string.Empty;
            if ((from data in m_Data where data.Size == size select data.Type).Any())
            {
                result = (from data in m_Data where data.Size == size select data.Type).ToList().First();
            }

            return result;
        }
        #endregion
    }
}
