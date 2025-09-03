using RevitUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YMS.Master
{
    class ClsHiuchiCsv
    {
        #region 定数
        /// <summary>
        /// CSV名称
        /// </summary>
        private const string CsvFileName = "Hiuchi.csv";

        /// <summary>
        /// 種別
        /// </summary>
        public const string TypeNameFVP = "自在火打受ピース";
        public const string TypeName30VP = "30度火打受ピース";
        public const string TypeNameVP = "火打受ピース";
        public const string TypeNameKaitenVP = "回転火打受けピース";
        public const string TypeNameOtherVP = "その他火打受けピース";
        public const string TypeNameHB = "火打ブロック";
        public const string TypeNameSHB = "小火打ブロック";
        public const string TypeName2RenSpPiece = "2連式補助ピース";
        public const string TypeNameSCG = "SCG";
        public const string TypeNameSHG = "SHG";
        public const string TypeNameSOG = "SOG";
        public const string TypeNameNone = "なし";

        #endregion

        #region メンバ変数
        /// <summary>
        /// メンバ変数
        /// </summary>
        private static List<ClsHiuchiCsv> m_Data = new List<ClsHiuchiCsv>();
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
        /// 対応主材
        /// </summary>
        public string TargetShuzai;

        /// <summary>
        /// 長さ
        /// </summary>
        public double Thickness;

        /// <summary>
        /// 腹起接地面長さ
        /// </summary>
        public double HaraGroundLength;

        /// <summary>
        /// 隅火打ちフラグ
        /// </summary>
        public bool CornerHiuchiFlg;

        /// <summary>
        /// 切梁フラグ
        /// </summary>
        public bool KiribariFlg;

        /// <summary>
        /// 切梁繋（切梁側）
        /// </summary>
        public bool KiribariTsunagiKiriSideFlg;

        /// <summary>
        /// 切梁繋（腹起側）
        /// </summary>
        public bool KiribariTsunagiHaraSideFlg;

        /// <summary>
        /// 切梁火打ち（上下段）
        /// </summary>
        public bool KiribariHiuchiJougeFlg;

        /// <summary>
        /// 切梁火打ち（切梁側）
        /// </summary>
        public bool KiribariHiuchiKiriSideFlg;

        /// <summary>
        /// 切梁火打ち（腹起側）
        /// </summary>
        public bool KiribariHiuchiHaraSideFlg;

        /// <summary>
        /// 切梁火打ち（三軸ピース側）
        /// </summary>
        public bool KiribariHiuchiSanjikuPieceSideFlg;

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
            List<ClsHiuchiCsv> lstCls = new List<ClsHiuchiCsv>();
            foreach (List<string> lstStr in lstlstStr)
            {
                if (bHeader)
                {
                    bHeader = false;
                    continue;
                }

                ClsHiuchiCsv cls = new ClsHiuchiCsv();
                cls.Type = lstStr[0];
                cls.Size = lstStr[1];
                cls.Thickness = RevitUtil.ClsCommonUtils.ChangeStrToDbl(lstStr[2]);
                cls.HaraGroundLength = RevitUtil.ClsCommonUtils.ChangeStrToDbl(lstStr[3]);
                cls.TargetShuzai = lstStr[4];
                cls.CornerHiuchiFlg = RevitUtil.ClsCommonUtils.ChangeStrToBool(lstStr[5]);
                cls.KiribariFlg = RevitUtil.ClsCommonUtils.ChangeStrToBool(lstStr[6]);
                cls.KiribariTsunagiKiriSideFlg = RevitUtil.ClsCommonUtils.ChangeStrToBool(lstStr[7]);
                cls.KiribariTsunagiHaraSideFlg = RevitUtil.ClsCommonUtils.ChangeStrToBool(lstStr[8]);
                cls.KiribariHiuchiJougeFlg = RevitUtil.ClsCommonUtils.ChangeStrToBool(lstStr[9]);
                cls.KiribariHiuchiKiriSideFlg = RevitUtil.ClsCommonUtils.ChangeStrToBool(lstStr[10]);
                cls.KiribariHiuchiHaraSideFlg = RevitUtil.ClsCommonUtils.ChangeStrToBool(lstStr[11]);
                cls.KiribariHiuchiSanjikuPieceSideFlg = RevitUtil.ClsCommonUtils.ChangeStrToBool(lstStr[12]);
                cls.FamilyPath = lstStr[13];
                lstCls.Add(cls);
            }
            m_Data = lstCls;

            return true;
        }

        /// <summary>
        /// CSV情報を取得
        /// </summary>
        /// <returns></returns>
        public static List<ClsHiuchiCsv> GetCsvData()
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
                lst = (from data in m_Data select data.Type).Distinct().ToList();
            }
            return lst;
        }

        /// <summary>
        /// 種別のリストを取得（隅火打ち）
        /// </summary>
        /// <returns></returns>
        public static List<string> GetTypeListCornerHiuchi()
        {
            GetCsv();
            List<string> lst = new List<string>();
            if ((from data in m_Data where data.CornerHiuchiFlg == true select data.Type).Any())
            {
                lst = (from data in m_Data where data.CornerHiuchiFlg == true select data.Type).Distinct().ToList();
            }
            return lst;
        }

        /// <summary>
        /// 種別のリストを取得（切梁）
        /// </summary>
        /// <returns></returns>
        public static List<string> GetTypeListKiribari()
        {
            GetCsv();
            List<string> lst = new List<string>();
            if ((from data in m_Data where data.KiribariFlg == true select data.Type).Any())
            {
                lst = (from data in m_Data where data.KiribariFlg == true select data.Type).Distinct().ToList();
            }
            return lst;
        }


        /// <summary>
        /// 種別のリストを取得（切梁繋ぎ-切梁側）
        /// </summary>
        /// <returns></returns>
        public static List<string> GetTypeListKiribariTsunagiKiriSide()
        {
            GetCsv();
            List<string> lst = new List<string>();
            if ((from data in m_Data where data.KiribariTsunagiKiriSideFlg == true select data.Type).Any())
            {
                lst = (from data in m_Data where data.KiribariTsunagiKiriSideFlg == true select data.Type).Distinct().ToList();
            }
            return lst;
        }

        /// <summary>
        /// 種別のリストを取得（切梁繋ぎ-腹起側）
        /// </summary>
        /// <returns></returns>
        public static List<string> GetTypeListKiribariTsunagiHaraSide()
        {
            GetCsv();
            List<string> lst = new List<string>();
            if ((from data in m_Data where data.KiribariTsunagiHaraSideFlg == true select data.Type).Any())
            {
                lst = (from data in m_Data where data.KiribariTsunagiHaraSideFlg == true select data.Type).Distinct().ToList();
            }
            return lst;
        }

        /// <summary>
        /// 種別のリストを取得（切梁火打ち-上下）
        /// </summary>
        /// <returns></returns>
        public static List<string> GetTypeListKiribariHiuchiJouge()
        {
            GetCsv();
            List<string> lst = new List<string>();
            if ((from data in m_Data where data.KiribariHiuchiJougeFlg == true select data.Type).Any())
            {
                lst = (from data in m_Data where data.KiribariHiuchiJougeFlg == true select data.Type).Distinct().ToList();
            }
            return lst;
        }

        /// <summary>
        /// 種別のリストを取得（切梁火打ち-切梁側）
        /// </summary>
        /// <returns></returns>
        public static List<string> GetTypeListKiribariHiuchiKiriSide()
        {
            GetCsv();
            List<string> lst = new List<string>();
            if ((from data in m_Data where data.KiribariHiuchiKiriSideFlg == true select data.Type).Any())
            {
                lst = (from data in m_Data where data.KiribariHiuchiKiriSideFlg == true select data.Type).Distinct().ToList();
            }
            return lst;
        }

        /// <summary>
        /// 種別のリストを取得（切梁火打ち-腹起側）
        /// </summary>
        /// <returns></returns>
        public static List<string> GetTypeListKiribariHiuchiHaraSide()
        {
            GetCsv();
            List<string> lst = new List<string>();
            if ((from data in m_Data where data.KiribariHiuchiHaraSideFlg == true select data.Type).Any())
            {
                lst = (from data in m_Data where data.KiribariHiuchiHaraSideFlg == true select data.Type).Distinct().ToList();
            }
            return lst;
        }

        /// <summary>
        /// 種別のリストを取得（切梁火打ち-三軸ピース側）
        /// </summary>
        /// <returns></returns>
        public static List<string> GetTypeListKiribariHiuchiSanjikuPieceSide()
        {
            GetCsv();
            List<string> lst = new List<string>();
            if ((from data in m_Data where data.KiribariHiuchiSanjikuPieceSideFlg == true select data.Type).Any())
            {
                lst = (from data in m_Data where data.KiribariHiuchiSanjikuPieceSideFlg == true select data.Type).Distinct().ToList();
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
        /// 種別をキーにサイズのリストを取得(隅火打ち)
        /// </summary>
        /// <returns></returns>
        public static List<string> GetSizeListCornerHiuchi(string type)
        {
            GetCsv();
            List<string> lst = new List<string>();
            if ((from data in m_Data where data.Type == type && data.CornerHiuchiFlg == true select data.Size).Any())
            {
                lst = (from data in m_Data where data.Type == type && data.CornerHiuchiFlg == true select data.Size).ToList();
            }
            return lst;
        }

        /// <summary>
        /// 種別をキーにサイズのリストを取得(切梁繋ぎ-切梁側）
        /// </summary>
        /// <returns></returns>
        public static List<string> GetSizeListKiribariTsunagiKiriSide(string type)
        {
            GetCsv();
            List<string> lst = new List<string>();
            if ((from data in m_Data where data.Type == type && data.KiribariTsunagiKiriSideFlg == true select data.Size).Any())
            {
                lst = (from data in m_Data where data.Type == type && data.KiribariTsunagiKiriSideFlg == true select data.Size).ToList();
            }
            return lst;
        }

        /// <summary>
        /// 種別をキーにサイズのリストを取得(切梁繋ぎ-腹起側）
        /// </summary>
        /// <returns></returns>
        public static List<string> GetSizeListKiribariTsunagiHaraSide(string type)
        {
            GetCsv();
            List<string> lst = new List<string>();
            if ((from data in m_Data where data.Type == type && data.KiribariTsunagiHaraSideFlg == true select data.Size).Any())
            {
                lst = (from data in m_Data where data.Type == type && data.KiribariTsunagiHaraSideFlg == true select data.Size).ToList();
            }
            return lst;
        }

        /// <summary>
        /// 種別をキーにサイズのリストを取得(切梁火打ち-上下）
        /// </summary>
        /// <returns></returns>
        public static List<string> GetSizeListKiribariHiuchiJouge(string type)
        {
            GetCsv();
            List<string> lst = new List<string>();
            if ((from data in m_Data where data.Type == type && data.KiribariHiuchiJougeFlg == true select data.Size).Any())
            {
                lst = (from data in m_Data where data.Type == type && data.KiribariHiuchiJougeFlg == true select data.Size).ToList();
            }
            return lst;
        }

        /// <summary>
        /// 種別をキーにサイズのリストを取得(切梁火打ち-切梁側）
        /// </summary>
        /// <returns></returns>
        public static List<string> GetSizeListKiribariHiuchiKiriSide(string type)
        {
            GetCsv();
            List<string> lst = new List<string>();
            if ((from data in m_Data where data.Type == type && data.KiribariHiuchiKiriSideFlg == true select data.Size).Any())
            {
                lst = (from data in m_Data where data.Type == type && data.KiribariHiuchiKiriSideFlg == true select data.Size).ToList();
            }
            return lst;
        }

        /// <summary>
        /// 種別をキーにサイズのリストを取得(切梁火打ち-腹起側）
        /// </summary>
        /// <returns></returns>
        public static List<string> GetSizeListKiribariHiuchiHaraSide(string type)
        {
            GetCsv();
            List<string> lst = new List<string>();
            if ((from data in m_Data where data.Type == type && data.KiribariHiuchiHaraSideFlg == true select data.Size).Any())
            {
                lst = (from data in m_Data where data.Type == type && data.KiribariHiuchiHaraSideFlg == true select data.Size).ToList();
            }
            return lst;
        }

        /// <summary>
        /// 種別をキーにサイズのリストを取得(切梁火打ち-三軸ピース側）
        /// </summary>
        /// <returns></returns>
        public static List<string> GetSizeListKiribariHiuchiSanjikuPieceSide(string type)
        {
            GetCsv();
            List<string> lst = new List<string>();
            if ((from data in m_Data where data.Type == type && data.KiribariHiuchiSanjikuPieceSideFlg == true select data.Size).Any())
            {
                lst = (from data in m_Data where data.Type == type && data.KiribariHiuchiSanjikuPieceSideFlg == true select data.Size).ToList();
            }
            return lst;
        }


        /// <summary>
        /// サイズをキーに厚さを取得
        /// </summary>
        /// <returns></returns>
        public static double GetThickness(string size)
        {
            GetCsv();
            double d = 0;
            if ((from data in m_Data where data.Size == size select data.Thickness).Any())
            {
                d = (from data in m_Data where data.Size == size select data.Thickness).ToList().First();
            }
            return d;
        }
        /// <summary>
        /// サイズをキーに厚さを取得
        /// </summary>
        /// <returns></returns>
        public static double GetHaraGroundLength(string size)
        {
            GetCsv();
            double d = 0;
            if ((from data in m_Data where data.Size == size select data.HaraGroundLength).Any())
            {
                d = (from data in m_Data where data.Size == size select data.HaraGroundLength).ToList().First();
            }
            return d;
        }
        /// <summary>
        /// サイズをキーに対応主材を取得
        /// </summary>
        /// <returns></returns>
        public static string GetTargetShuzai(string size)
        {
            GetCsv();
            string str = string.Empty;
            if ((from data in m_Data where data.Size == size select data.TargetShuzai).Any())
            {
                str = (from data in m_Data where data.Size == size select data.TargetShuzai).ToList().First();
            }
            return str;
        }
        /// <summary>
        /// サイズをキーに高さを取得
        /// </summary>
        /// <returns></returns>
        public static double GetWidth(string size)
        {
            GetCsv();
            string str = string.Empty;
            if ((from data in m_Data where data.Size == size select data.TargetShuzai).Any())
            {
                str = (from data in m_Data where data.Size == size select data.TargetShuzai).ToList().First();
            }
            return Master.ClsYamadomeCsv.GetWidth(str);
        }
        /// <summary>
        /// 種類とサイズをキーに高さを取得
        /// </summary>
        /// <returns></returns>
        public static double GetWidth(string type, string size)
        {
            GetCsv();
            string str = string.Empty;
            if ((from data in m_Data where data.Type == type && data.Size == size select data.TargetShuzai).Any())
            {
                str = (from data in m_Data where data.Type == type && data.Size == size select data.TargetShuzai).ToList().First();
            }
            return Master.ClsYamadomeCsv.GetWidth(str);
        }
        /// <summary>
        /// サイズをキーに種類を取得
        /// </summary>
        /// <returns></returns>
        public static string GetType(string size)
        {
            GetCsv();
            string str = string.Empty;
            if ((from data in m_Data where data.Size == size select data.TargetShuzai).Any())
            {
                str = (from data in m_Data where data.Size == size select data.Type).ToList().First();
            }
            return str;
        }
        /// <summary>
        /// 種類と対象主材をキーにサイズを取得
        /// </summary>
        /// <returns></returns>
        public static string GetSize(string type, string TargetShuzai)
        {
            GetCsv();
            string str = string.Empty;
            if ((from data in m_Data where data.Type == type && data.TargetShuzai == TargetShuzai select data.Size).Any())
            {
                str = (from data in m_Data where data.Type == type && data.TargetShuzai == TargetShuzai select data.Size).ToList().First();
            }
            return str;
        }
        /// <summary>
        /// 種類と対象主材をキーにサイズを取得
        /// </summary>
        /// <returns></returns>
        public static string GetSize2(string type, string TargetShuzai)
        {
            GetCsv();
            string str = string.Empty;
            if ((from data in m_Data where data.Type == type && data.TargetShuzai == TargetShuzai select data.Size).Any())
            {
                str = (from data in m_Data where data.Type == type && data.TargetShuzai == TargetShuzai select data.Size).ToList().First();
            }
            if(string.IsNullOrWhiteSpace(str))
            {
                if ((from data in m_Data where data.Type == type select data.Size).Any())
                {
                    str = (from data in m_Data where data.Type == type select data.Size).ToList().First();
                }
            }
            return str;
        }
        /// <summary>
        /// 種類と対象主材をキーにサイズを取得
        /// </summary>
        /// <returns></returns>
        public static double GetThickness(string type, string TargetShuzai)
        {
            GetCsv();
            double d = 0;
            if ((from data in m_Data where data.Type == type && data.TargetShuzai == TargetShuzai select data.Thickness).Any())
            {
                d = (from data in m_Data where data.Type == type && data.TargetShuzai == TargetShuzai select data.Thickness).ToList().First();
            }
            return d;
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
        /// 種類と対象主材をキーにファミリのパスを取得
        /// </summary>
        /// <returns></returns>
        public static string GetFamilyPath(string type, string TargetShuzai)
        {
            GetCsv();
            string str = string.Empty;
            if ((from data in m_Data where data.Type == type && data.TargetShuzai == TargetShuzai select data.FamilyPath).Any())
            {
                str = (from data in m_Data where data.Type == type && data.TargetShuzai == TargetShuzai select data.FamilyPath).ToList().First();
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
