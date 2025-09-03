using RevitUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YMS.Master
{
    class ClsYamadomeCsv
    {
        #region メンバ変数

        /// <summary>
        /// CSV名称
        /// </summary>
        private const string CsvFileName = "Yamadome.csv";

        /// <summary>
        /// メンバ変数
        /// </summary>
        private static List<ClsYamadomeCsv> m_Data = new List<ClsYamadomeCsv>();

        /// <summary>
        /// 種類
        /// </summary>
        public const string TypeShuzai = "主材";
        public const string TypeHighShuzai = "高強度主材";
        public const string TypeTwinBeam = "ツインビーム";
        public const string TypeMegaBeam = "メガビーム";

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
        /// 頭繋ぎフラグ
        /// </summary>
        public bool bAtamaTsunagi;

        /// <summary>
        /// 長さ
        /// </summary>
        public double Length;

        /// <summary>
        /// ウェブ
        /// </summary>
        public double Web;

        /// <summary>
        /// フランジ
        /// </summary>
        public double Flange;

        /// <summary>
        /// ファミリパス
        /// </summary>
        public string FamilyPath;

        /// <summary>
        /// 仮鋼材ファミリパス
        /// </summary>
        public string KariFamilyPath;

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
            List<ClsYamadomeCsv> lstCls = new List<ClsYamadomeCsv>();
            foreach (List<string> lstStr in lstlstStr)
            {
                if (bHeader)
                {
                    bHeader = false;
                    continue;
                }

                ClsYamadomeCsv cls = new ClsYamadomeCsv();
                cls.Type = lstStr[0];
                cls.Size = lstStr[1];
                cls.bAtamaTsunagi = RevitUtil.ClsCommonUtils.ChangeStrToBool(lstStr[2]);
                cls.Length = RevitUtil.ClsCommonUtils.ChangeStrToDbl(lstStr[3]);
                cls.Web = RevitUtil.ClsCommonUtils.ChangeStrToDbl(lstStr[4]);
                cls.Flange = RevitUtil.ClsCommonUtils.ChangeStrToDbl(lstStr[5]);
                cls.FamilyPath = lstStr[6];
                cls.KariFamilyPath = lstStr[7];
                lstCls.Add(cls);
            }
            m_Data = lstCls;

            return true;
        }

        /// <summary>
        /// CSV情報を取得
        /// </summary>
        /// <returns></returns>
        public static List<ClsYamadomeCsv> GetCsvData()
        {
            GetCsv();
            return m_Data;
        }

        /// <summary>
        /// 種別のリストを取得
        /// </summary>
        /// <returns></returns>
        public static List<string> GetTypeList(bool bHigh = true, bool bMega = true, bool bTwin = true)
        {
            GetCsv();
            List<string> lst = new List<string>();
            if ((from data in m_Data select data.Type).Any())
            {
                lst = (from data in m_Data select data.Type).Distinct().ToList();


                if (bHigh == false)
                {
                    lst = (from data in lst where data != TypeHighShuzai select data).Distinct().ToList();
                }
                if (bMega == false)
                {
                    lst = (from data in lst where data != TypeMegaBeam select data).Distinct().ToList();
                }
                if (bTwin == false)
                {
                    lst = (from data in lst where data != TypeTwinBeam select data).Distinct().ToList();
                }
            }

            return lst;
        }

        /// <summary>
        /// 種別をキーにサイズのリストを取得
        /// </summary>
        /// <returns></returns>
        public static List<string> GetSizeList(string type, bool bAtamaTsunagi = false)
        {
            GetCsv();
            List<string> lst = new List<string>();
            if (bAtamaTsunagi)
            {
                if ((from data in m_Data where data.Type == type && data.bAtamaTsunagi == true select data.Size).Any())
                {
                    lst = (from data in m_Data where data.Type == type && data.bAtamaTsunagi == true select data.Size).Distinct().ToList();
                }
            }
            else
            {
                if ((from data in m_Data where data.Type == type select data.Size).Any())
                {
                    lst = (from data in m_Data where data.Type == type select data.Size).Distinct().ToList();
                }
            }

            return lst;
        }

        /// <summary>
        /// サイズをキーにウェブ厚を取得
        /// </summary>
        /// <returns></returns>
        public static double GetWeb(string size)
        {
            GetCsv();
            double web = 0.0;
            if ((from data in m_Data where data.Size == size select data.Web).Any())
            {
                web = (from data in m_Data where data.Size == size select data.Web).ToList().First();
            }
            return web;
        }

        /// <summary>
        /// サイズをキーにフランジ厚を取得
        /// </summary>
        /// <returns></returns>
        public static double GetFlange(string size)
        {
            GetCsv();
            double flange = 0.0;
            if ((from data in m_Data where data.Size == size select data.Flange).Any())
            {
                flange = (from data in m_Data where data.Size == size select data.Flange).ToList().First();
            }
            return flange;
        }

        /// <summary>
        /// サイズをキーにファミリのパスを取得
        /// </summary>
        /// <returns></returns>
        public static string GetFamilyPath(string size, double dLength)
        {
            GetCsv();
            string str = string.Empty;
            if ((from data in m_Data where data.Size == size && data.Length == dLength select data.FamilyPath).Any())
            {
                str = (from data in m_Data where data.Size == size && data.Length == dLength select data.FamilyPath).ToList().First();
            }

            string symbolFolpath = Parts.ClsZumenInfo.GetYMSFolder();
            string filePath = System.IO.Path.Combine(symbolFolpath, str);
            return filePath;
        }

        /// <summary>
        /// サイズをキーに仮鋼材のファミリのパスを取得
        /// </summary>
        /// <returns></returns>
        public static string GetKariFamilyPath(string size, double dLength)
        {
            GetCsv();
            string str = string.Empty;
            if ((from data in m_Data where data.Size == size && data.Length == dLength select data.KariFamilyPath).Any())
            {
                str = (from data in m_Data where data.Size == size && data.Length == dLength select data.KariFamilyPath).ToList().First();
            }

            string symbolFolpath = Parts.ClsZumenInfo.GetYMSFolder();
            string filePath = System.IO.Path.Combine(symbolFolpath, str);
            return filePath;
        }

        /// <summary>
        /// サイズをキーに仮鋼材のファミリのパスを取得
        /// </summary>
        /// <returns></returns>
        public static string GetKariFamilyPath(string size)
        {
            GetCsv();
            string str = string.Empty;
            if ((from data in m_Data where data.Size == size select data.KariFamilyPath).Any())
            {
                str = (from data in m_Data where data.Size == size select data.KariFamilyPath).ToList().First();
            }

            string symbolFolpath = Parts.ClsZumenInfo.GetYMSFolder();
            string filePath = System.IO.Path.Combine(symbolFolpath, str);
            return filePath;
        }
        /// <summary>
        /// サイズをキーに仮鋼材(1点配置)のファミリのパスを取得
        /// </summary>
        /// <returns></returns>
        public static string GetKariFamilyPath(string size, bool one)
        {
            GetCsv();
            string str = string.Empty;
            if ((from data in m_Data where data.Size == size select data.KariFamilyPath).Any())
            {
                str = (from data in m_Data where data.Size == size select data.KariFamilyPath).ToList().First();
            }

            if (one)
            {
                if(str.Contains("高強度"))
                    str = str.Replace("仮鋼材_高強度山留主材", "仮鋼材_斜梁");
                else
                    str = str.Replace("仮鋼材_主材", "仮鋼材_斜梁");
            }

            string symbolFolpath = Parts.ClsZumenInfo.GetYMSFolder();
            string filePath = System.IO.Path.Combine(symbolFolpath, str);
            return filePath;
        }
        /// <summary>
        /// 仮鋼材のファミリのファミリ名Listを取得
        /// </summary>
        /// <returns></returns>
        public static List<string> GetKariFamilyNameList()
        {
            GetCsv();
            List<string> kariNameList = new List<string>();
            foreach (var data in m_Data)
            {
                string filePath = data.KariFamilyPath;
                string familyName = ClsRevitUtil.GetFamilyName(filePath);
                kariNameList.Add(familyName);
            }

            return kariNameList;
        }
        /// <summary>
        /// 仮鋼材斜梁のファミリのファミリ名Listを取得
        /// </summary>
        /// <returns></returns>
        public static List<string> GetKariSyabariFamilyNameList()
        {
            GetCsv();
            List<string> kariNameList = new List<string>();
            foreach (var data in m_Data)
            {
                string filePath = data.KariFamilyPath;
                string familyName = ClsRevitUtil.GetFamilyName(filePath);
                if (familyName.Contains("高強度"))
                    familyName = familyName.Replace("仮鋼材_高強度山留主材", "仮鋼材_斜梁");
                else
                    familyName = familyName.Replace("仮鋼材_主材", "仮鋼材_斜梁");
                kariNameList.Add(familyName);
            }

            return kariNameList;
        }
        /// <summary>
        /// 仮鋼材の名称から割付部材のファミリパスを取得
        /// </summary>
        /// <param name="karikouzai"></param>
        /// <param name="lengthM"></param>
        /// <returns></returns>
        public static string GetYamadomeFamilyFromKarikouzai(string karikouzai, double lengthM)
        {
            string res = string.Empty;

            GetCsv();
            foreach (var data in m_Data)
            {
                string kariPath = data.KariFamilyPath;
                string kariName = System.IO.Path.GetFileNameWithoutExtension(kariPath);
                if (kariName == karikouzai && data.Length == lengthM)
                {
                    string filePath = data.FamilyPath;
                    string symbolFolpath = Parts.ClsZumenInfo.GetYMSFolder();
                    res = System.IO.Path.Combine(symbolFolpath, filePath);
                    break;
                }
            }

            return res;
        }

        /// <summary>
        /// 山留主材のファイル名リストを取得
        /// </summary>
        /// <returns></returns>
        public static List<string> GetYamadomeFamilynameList()
        {
            GetCsv();
            List<string> nameList = new List<string>();
            foreach (var data in m_Data)
            {
                string filePath = data.FamilyPath;
                string familyName = ClsRevitUtil.GetFamilyName(filePath);
                nameList.Add(familyName);
            }

            return nameList;
        }

        /// <summary>
        /// サイズから幅を取得
        /// </summary>
        /// <returns></returns>
        public static double GetWidth(string size)
        {
            //本来はファミリの中身を見に行って、プロパティから幅の値を取得するが、一旦仮
            double dWidth = 0;
            switch (size)
            {
                case "20HA":
                    {
                        dWidth = 200;
                        break;
                    }
                case "25HA":
                    {
                        dWidth = 250;
                        break;
                    }
                case "30HA":
                    {
                        dWidth = 300;
                        break;
                    }
                case "35HA":
                    {
                        dWidth = 350;
                        break;
                    }
                case "40HA":
                    {
                        dWidth = 400;
                        break;
                    }
                case "50HA":
                    {
                        dWidth = 500;
                        break;
                    }
                case "35SMH":
                    {
                        dWidth = 350;
                        break;
                    }
                case "40SMH":
                    {
                        dWidth = 400;
                        break;
                    }
                case "60SMH":
                    {
                        dWidth = 400;
                        break;
                    }
                case "80SMH":
                    {
                        dWidth = 400;
                        break;
                    }
                default:
                    {
                        dWidth = 0;
                        break;
                    }
            }
            return dWidth;
        }

        /// <summary>
        /// CorH-〇X〇X〇X〇から指定の寸法値を取得する
        /// </summary>
        /// <param name="kouzaiSize">鋼材名</param>
        /// <param name="num">0:形鋼の種類,1:高さ,2:ﾌﾗﾝｼﾞ幅,3:腹板厚,4:ﾌﾗﾝｼﾞ厚</param>
        /// <returns>寸法値</returns>
        public static string GetKouzaiSizeSunpou(string kouzaiSize, int num)
        {
            if (4 < num || num < 0) return string.Empty;

            List<string> nameDataList = new List<string>();

            nameDataList.Add(kouzaiSize.Substring(0, 1));
            string[] sunpou = kouzaiSize.Substring(2).Split('X');
            foreach (string snp in sunpou)
            {
                nameDataList.Add(snp);
            }
            return nameDataList[num];
        }

        #endregion
    }
}
