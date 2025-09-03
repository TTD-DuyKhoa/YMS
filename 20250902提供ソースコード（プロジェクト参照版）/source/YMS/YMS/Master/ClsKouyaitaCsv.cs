using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YMS.Master
{
    public class ClsKouyaitaCsv
    {
        #region メンバ変数
        /// <summary>
        /// CSV名称
        /// </summary>
        private const string CsvFileName = "Kouyaita.csv";

        // 抽出する文字列のパターン
        private const string Pattern = @"(SP-[23456]L?|SP-(10|25|45|50)H|LSP-[23]B-[56]mm|LSP-3C-5mm|SP-[234WJ]|SP-C[34])";

        /// <summary>
        /// 種別
        /// </summary>
        public const string TypeKouyaita = "鋼矢板";
        public const string TypeKeiryoKouyaita = "軽量鋼矢板";
        public const string TypeHirohabaKouyaita = "広幅鋼矢板";
        public const string TypeSMJ = "SM-J";
        public const string TypeKiyaita = "木矢板";

        /// <summary>
        /// メンバ変数
        /// </summary>
        private static List<ClsKouyaitaCsv> m_Data = new List<ClsKouyaitaCsv>();
        #endregion

        #region プロパティ
        /// <summary>
        /// 種類
        /// </summary>
        public string Type;

        /// <summary>
        /// サイズ
        /// </summary>
        public string Size;

        /// <summary>
        /// コーナー材フラグ
        /// </summary>
        public bool CornerFlg;

        /// <summary>
        /// コーナー種類1
        /// </summary>
        public string CornerType1;

        /// <summary>
        /// コーナー種類2
        /// </summary>
        public string CornerType2;

        /// <summary>
        /// 高さ
        /// </summary>
        public string Height;

        /// <summary>
        /// 幅
        /// </summary>
        public string Width;

        /// <summary>
        /// 横長さ
        /// </summary>
        public string Length;

        /// <summary>
        /// ファミリパス
        /// </summary>
        public string FamilyPath;
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
            List<ClsKouyaitaCsv> lstCls = new List<ClsKouyaitaCsv>();
            foreach (List<string> lstStr in lstlstStr)
            {
                if (bHeader)
                {
                    bHeader = false;
                    continue;
                }

                ClsKouyaitaCsv cls = new ClsKouyaitaCsv();
                cls.Type = lstStr[0];
                cls.Size = lstStr[1];
                cls.CornerFlg = RevitUtil.ClsCommonUtils.ChangeStrToBool(lstStr[2]);
                cls.CornerType1 = lstStr[3];
                cls.CornerType2 = lstStr[4];
                cls.Height = lstStr[5];
                cls.Width = lstStr[6];
                cls.Length = lstStr[7];
                cls.FamilyPath = lstStr[8];
                lstCls.Add(cls);
            }
            m_Data = lstCls;

            return true;
        }

        /// <summary>
        /// CSV情報を取得
        /// </summary>
        /// <returns></returns>
        public static List<ClsKouyaitaCsv> GetCsvData()
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
        /// 種別のリストを取得（コーナー材の判別）
        /// </summary>
        /// <returns></returns>
        public static List<string> GetTypeList(bool bCorner)
        {
            GetCsv();
            List<string> lst = new List<string>();
            if ((from data in m_Data where data.CornerFlg == bCorner select data.Type).Any())
            {
                lst = (from data in m_Data where data.CornerFlg == bCorner select data.Type).Distinct().ToList();
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

        /// <summary>
        /// 種別をキーにサイズのリストを取得（コーナー材の判別）
        /// </summary>
        /// <returns></returns>
        public static List<string> GetSizeList(string type, bool bCorner)
        {
            GetCsv();
            List<string> lst = new List<string>();
            if ((from data in m_Data where data.Type == type && data.CornerFlg == bCorner select data.Size).Any())
            {
                lst = (from data in m_Data where data.Type == type && data.CornerFlg == bCorner select data.Size).ToList();
            }
            return lst;
        }

        /// <summary>
        /// サイズをキーにコーナー矢板を取得
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static List<string> GetCornerList(string type, string size)
        {
            GetCsv();
            string str1 = string.Empty;
            if ((from data in m_Data where data.Size == size select data.CornerType1).Any())
            {
                str1 = (from data in m_Data where data.Size == size select data.CornerType1).ToList().First();
            }
            string str2 = string.Empty;
            if ((from data in m_Data where data.Size == size select data.CornerType2).Any())
            {
                str2 = (from data in m_Data where data.Size == size select data.CornerType2).ToList().First();
            }

            List<string> lst = new List<string>();
            if ((from data in m_Data where data.Type == type && data.CornerFlg == true && data.CornerType1 == str1 select data.Size).Any())
            {
                lst.Add((from data in m_Data where data.Type == type && data.CornerFlg == true && data.CornerType1 == str1 select data.Size).ToList().First());
            }
            if((from data in m_Data where data.Type == type && data.CornerFlg == true && data.CornerType1 == str2 select data.Size).Any())
            {
                lst.Add((from data in m_Data where data.Type == type && data.CornerFlg == true && data.CornerType1 == str2 select data.Size).ToList().First());
            }

            return lst;
        }

        /// <summary>
        /// サイズをキーに高さを取得
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string GetHeight(String size)
        {
            Match match = Regex.Match(size, Pattern);
            if (!match.Success)
            {
                return string.Empty;
            }
            size = match.Value;

            GetCsv();
            string str = string.Empty;
            if ((from data in m_Data where data.Size == size select data.Height).Any())
            {
                str = (from data in m_Data where data.Size == size select data.Height).ToList().First();
            }
            return str;
        }

        /// <summary>
        /// サイズをキーに幅を取得
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string GetWidth(String size)
        {
            Match match = Regex.Match(size, Pattern);
            if (!match.Success)
            {
                return string.Empty;
            }
            size = match.Value;

            GetCsv();
            string str = string.Empty;
            if ((from data in m_Data where data.Size == size select data.Width).Any())
            {
                str = (from data in m_Data where data.Size == size select data.Width).ToList().First();
            }
            return str;
        }

        /// <summary>
        /// サイズをキーに長さを取得
        /// </summary>
        /// <returns></returns>
        public static string GetLength(string size)
        {
            GetCsv();
            string str = string.Empty;
            if ((from data in m_Data where data.Size == size select data.Length).Any())
            {
                str = (from data in m_Data where data.Size == size select data.Length).ToList().First();
            }
            return str;
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

        /// <summary>
        /// ファミリのパスに含まれる名前をキーにサイズを取得
        /// </summary>
        /// <returns></returns>
        public static string GetSizePileFamilyPathInName(string familyName)
        {
            GetCsv();
            string str = string.Empty;
            if ((from data in m_Data where data.FamilyPath.Contains(familyName) select data.Size).Any())
            {
                str = (from data in m_Data where data.FamilyPath.Contains(familyName) select data.Size).ToList().First();
            }
            return str;
        }

        /// <summary>
        /// ファミリのパスに含まれる名前をキーにタイプを取得
        /// </summary>
        /// <returns></returns>
        public static string GetTypePileFamilyPathInName(string familyName)
        {
            GetCsv();
            string str = string.Empty;
            if ((from data in m_Data where data.FamilyPath.Contains(familyName) select data.Type).Any())
            {
                str = (from data in m_Data where data.FamilyPath.Contains(familyName) select data.Type).ToList().First();
            }
            return str;
        }

        #endregion
    }
}
