using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Architecture;
using System.IO;
using Autodesk.Revit.UI.Selection;
using RevitUtil;
using YMS.Parts;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using static Autodesk.Revit.DB.SpecTypeId;
using Reference = Autodesk.Revit.DB.Reference;
using System.Diagnostics;
using System.Runtime.InteropServices;

//using ParkingAutoModeling.Common;
namespace YMS
{
    public class ClsYMSUtil
    {
        #region CommonUtil

        /// <summary>
        /// DLL直下YMSフォルダ取得
        /// </summary>
        /// <param name="iniPath"></param>
        /// <returns></returns>
        public static string GetExecutingAssemblyYMSPath()
        {
            string apppath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string appFol = System.IO.Path.GetDirectoryName(apppath);
            string symbolFolpath = System.IO.Path.Combine(appFol, "");

            return symbolFolpath;
        }

        /// <summary>
        /// 斜梁受ピースに設定するθの角度が0.18未満だとファミリエラーを起こすため0に変換
        /// -> 2025/4/8 正確な配置ができないので、そのまま返すように変更 
        /// </summary>
        /// <param name="angle">角度</param>
        /// <returns></returns>
        public static double CheckSyabariAngle(double angle)
        {
            //double deg = ClsGeo.Rad2Deg(angle);
            //if (-0.18 < deg && deg < 0.18)
            //{
            //    return 0.0;
            //}
            return angle;
        }

        /// <summary>
        /// 形鋼_HorL〇X〇X〇X〇から指定の寸法値を取得する
        /// </summary>
        /// <param name="kouzaiName">鋼材名</param>
        /// <param name="num">0:形鋼の種類,1:高さ,2:ﾌﾗﾝｼﾞ幅,3:腹板厚,4:ﾌﾗﾝｼﾞ厚</param>
        /// <returns>寸法値</returns>
        public static string GetKouzaiSizeSunpou(string kouzaiName, int num)
        {
            if (4 < num || num < 0) return string.Empty;

            List<string> nameDataList = new List<string>();
            //HorL〇X〇X〇X〇の形に直す
            kouzaiName = kouzaiName.Remove(0, 3);

            //ハイフンを取り除く
            kouzaiName = kouzaiName.Replace("-", "");

            nameDataList.Add(kouzaiName.Substring(0, 1));
            //string[] sunpou = kouzaiName.Substring(1).Split('X');
            string[] sunpou = kouzaiName.Substring(1).Split(new char[] { 'X', 'x' });
            foreach (string snp in sunpou)
            {
                nameDataList.Add(snp);
            }
            return nameDataList[num];
        }

        /// <summary>
        /// 形or杭_HorL〇x〇x〇x〇から指定の寸法値を取得する
        /// </summary>
        /// <param name="kouzaiName">鋼材名</param>
        /// <param name="num">0:形鋼の種類,1:高さ,2:ﾌﾗﾝｼﾞ幅,3:腹板厚,4:ﾌﾗﾝｼﾞ厚</param>
        /// <returns>寸法値</returns>
        public static string GetKouzaiSizeSunpou1(string kouzaiName, int num)
        {
            if (4 < num || num < 0) return string.Empty;

            List<string> nameDataList = new List<string>();
            //HorL〇x〇x〇x〇の形に直す
            kouzaiName = kouzaiName.Remove(0, 2);

            nameDataList.Add(kouzaiName.Substring(0, 1));
            string[] sunpou = kouzaiName.Substring(1).Split('x');
            foreach (string snp in sunpou)
            {
                nameDataList.Add(snp);
            }
            return nameDataList[num];
        }

        /// <summary>
        /// HorL-〇x〇x〇x〇から指定の寸法値を取得する
        /// </summary>
        /// <param name="kouzaiName">鋼材名</param>
        /// <param name="num">0:形鋼の種類,1:高さ,2:ﾌﾗﾝｼﾞ幅,3:腹板厚,4:ﾌﾗﾝｼﾞ厚</param>
        /// <returns>寸法値</returns>
        public static string GetKouzaiSizeSunpou2(string kouzaiName, int num)
        {
            if (4 < num || num < 0) return string.Empty;

            List<string> nameDataList = new List<string>();
            //HorL〇x〇x〇x〇の形に直す
            kouzaiName = RemoveLeftSide(kouzaiName, '-', '‐');

            nameDataList.Add(kouzaiName.Substring(0, 1));
            string[] sunpou = kouzaiName.Substring(1).Split('x');
            foreach (string snp in sunpou)
            {
                nameDataList.Add(snp);
            }
            return nameDataList[num];
        }

        public static string RemoveLeftSide(string input, params char[] separators)
        {
            int index = -1;
            foreach (char separator in separators)
            {
                int tempIndex = input.LastIndexOf(separator);
                if (tempIndex > index)
                {
                    index = tempIndex;
                }
            }

            if (index >= 0)
            {
                return input.Substring(index + 1);
            }
            else
            {
                return input;
            }
        }

        ///// <summary>
        ///// 主材のサイズ(〇〇HA)から幅(フランジ幅)を取得する
        ///// </summary>
        ///// <param name="syuzaiName"></param>
        ///// <returns></returns>
        //public static double GetSyuzaiHaba(string syuzaiName)
        //{
        //    //if (clsHaraBase.m_kouzaiType == "主材")
        //    //{
        //    //    strKouzaiHaba = strKouzaiHaba.Replace("HA", "");
        //    //}
        //    //else if (clsHaraBase.m_kouzaiType == "高強度主材")
        //    //{
        //    //    strKouzaiHaba = strKouzaiHaba.Replace("SMH", "");
        //    //}
        //    //else if (clsHaraBase.m_kouzaiType == "メガビーム")
        //    //{
        //    //    strKouzaiHaba = strKouzaiHaba.Replace("SMH", "");
        //    //    return double.Parse(strKouzaiHaba) * 10 / 2;
        //    //}
        //    //else
        //    //{
        //    //    return 0.0;
        //    //}

        //    //return double.Parse(strKouzaiHaba) * 10;

        //    return 0.0;
        //}

        /// <summary>
        /// 主材のファミリ名から長さを取得する
        /// </summary>
        /// <param name="syuzaiName"></param>
        /// <returns></returns>
        /// <remarks>山留主材_30HA_1.0 → 1.0</remarks>
        public static double GetSyuzaiLength(string syuzaiName)
        {
            // SMHが含まれる場合
            if (syuzaiName.Contains("80SMH"))
            {
                return 9000.0;
            }

            // 文字列をアンダースコアで分割
            string[] parts = syuzaiName.Split('_');

            // 最後の要素を取得
            string lastPart = parts[parts.Length - 1];

            double length = ClsCommonUtils.ChangeStrToDbl(lastPart) * 1000;

            return length;
        }

        /// <summary>
        /// ベースから幅(フランジ幅)を取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="idPartner">腹起 切梁 切梁火打 隅火打 に対応</param>
        /// <returns></returns>
        public static double GetKouzaiHabaFromBase(Document doc, ElementId idPartner, bool bDoubleUPorKiri = false, bool bDoubleUNorHara = false)
        {
            string strKouzaiType = "";
            string strKouzaiHaba = "";

            Element selPartner = doc.GetElement(idPartner);
            if (selPartner is FamilyInstance)
            {
                FamilyInstance inst = selPartner as FamilyInstance;
                switch (inst.Name)
                {
                    case "腹起ベース":
                        ClsHaraokoshiBase clsHaraBase = new ClsHaraokoshiBase();
                        clsHaraBase.SetParameter(doc, idPartner);
                        strKouzaiHaba = clsHaraBase.m_kouzaiSize;
                        strKouzaiType = clsHaraBase.m_kouzaiType;
                        break;
                    case "切梁ベース":
                        ClsKiribariBase clsKiri = new ClsKiribariBase();
                        clsKiri.SetParameter(doc, idPartner);
                        strKouzaiHaba = clsKiri.m_kouzaiSize;
                        break;
                    case "切梁火打ベース":
                        ClsKiribariHiuchiBase clsKiriHiuchi = new ClsKiribariHiuchiBase();
                        clsKiriHiuchi.SetParameter(doc, idPartner);

                        if (bDoubleUPorKiri) strKouzaiHaba = clsKiriHiuchi.m_SteelSizeDoubleUpper;
                        else if (bDoubleUNorHara) strKouzaiHaba = clsKiriHiuchi.m_SteelSizeDoubleUnder;
                        else strKouzaiHaba = clsKiriHiuchi.m_SteelSizeSingle;

                        break;
                    case "隅火打ベース":
                        ClsCornerHiuchiBase clsCHiuchi = new ClsCornerHiuchiBase();
                        clsCHiuchi.SetParameter(doc, idPartner);
                        strKouzaiHaba = clsCHiuchi.m_SteelSize;
                        break;
                    case "切梁継ぎベース":
                        ClsKiribariTsugiBase clsKiriTsugi = new ClsKiribariTsugiBase();
                        clsKiriTsugi.SetParameter(doc, idPartner);
                        if (bDoubleUPorKiri) strKouzaiHaba = clsKiriTsugi.m_KiriSideSteelSizeDouble;
                        else if (bDoubleUNorHara) strKouzaiHaba = clsKiriTsugi.m_HaraSideSteelSizeDouble;
                        else strKouzaiHaba = clsKiriTsugi.m_SteelSizeSingle;
                        break;
                    default:
                        return 0.0;
                }
            }

            if (strKouzaiType == "主材" || strKouzaiHaba.Contains("HA"))
            {
                strKouzaiHaba = strKouzaiHaba.Replace("HA", "");
            }
            else if (strKouzaiType == "高強度主材" || strKouzaiHaba.Contains("SMH"))
            {
                strKouzaiHaba = strKouzaiHaba.Replace("SMH", "");
                if (strKouzaiHaba == "60")
                    strKouzaiHaba = "40";
                if (strKouzaiType == "メガビーム")
                {
                    return double.Parse(strKouzaiHaba) * 10 / 2;
                }
            }
            else
            {
                return 0.0;
            }

            return double.Parse(strKouzaiHaba) * 10;
        }

        /// <summary>
        /// ベースから鋼材サイズを取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="idPartner">腹起 切梁 切梁火打 隅火打 に対応</param>
        /// <returns></returns>
        public static string GetKouzaiSizeFromBase(Document doc, ElementId idPartner, bool bDoubleUPorKiri = false, bool bDoubleUNorHara = false)
        {
            string strKouzaiType = "";
            string strKouzaiSize = "";

            Element selPartner = doc.GetElement(idPartner);
            if (selPartner is FamilyInstance)
            {
                FamilyInstance inst = selPartner as FamilyInstance;
                switch (inst.Name)
                {
                    case "腹起ベース":
                        ClsHaraokoshiBase clsHaraBase = new ClsHaraokoshiBase();
                        clsHaraBase.SetParameter(doc, idPartner);
                        strKouzaiSize = clsHaraBase.m_kouzaiSize;
                        strKouzaiType = clsHaraBase.m_kouzaiType;
                        break;
                    case "切梁ベース":
                        ClsKiribariBase clsKiri = new ClsKiribariBase();
                        clsKiri.SetParameter(doc, idPartner);
                        strKouzaiSize = clsKiri.m_kouzaiSize;
                        break;
                    case "切梁火打ベース":
                        ClsKiribariHiuchiBase clsKiriHiuchi = new ClsKiribariHiuchiBase();
                        clsKiriHiuchi.SetParameter(doc, idPartner);

                        if (bDoubleUPorKiri) strKouzaiSize = clsKiriHiuchi.m_SteelSizeDoubleUpper;
                        else if (bDoubleUNorHara) strKouzaiSize = clsKiriHiuchi.m_SteelSizeDoubleUnder;
                        else strKouzaiSize = clsKiriHiuchi.m_SteelSizeSingle;

                        break;
                    case "隅火打ベース":
                        ClsCornerHiuchiBase clsCHiuchi = new ClsCornerHiuchiBase();
                        clsCHiuchi.SetParameter(doc, idPartner);
                        strKouzaiSize = clsCHiuchi.m_SteelSize;
                        break;
                    case "切梁継ぎベース":
                        ClsKiribariTsugiBase clsKiriTsugi = new ClsKiribariTsugiBase();
                        clsKiriTsugi.SetParameter(doc, idPartner);
                        if (bDoubleUPorKiri) strKouzaiSize = clsKiriTsugi.m_KiriSideSteelSizeDouble;
                        else if (bDoubleUNorHara) strKouzaiSize = clsKiriTsugi.m_HaraSideSteelSizeDouble;
                        else strKouzaiSize = clsKiriTsugi.m_SteelSizeSingle;
                        break;
                    default:
                        return "";
                }
            }


            return strKouzaiSize;
        }

        /// <summary>
        /// 対象のファミリにボルト種類と数をカスタムデータとして持たせる(フランジ)
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id">ファミリ</param>
        /// <param name="bolt">ボルト種類</param>
        /// <param name="num">数</param>
        /// <returns></returns>
        public static bool SetBoltFlange(Document doc, ElementId id, string bolt, int num)
        {
            if (!ClsRevitUtil.CustomDataSet(doc, id, "ボルトF", bolt))
                return false;
            if (!ClsRevitUtil.CustomDataSet(doc, id, "ボルトF数", num))
                return false;
            return true;
        }
        /// <summary>
        /// 対象のファミリからボルト種類と数のカスタムデータを取得する(フランジ)
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id">ファミリ</param>
        /// <returns></returns>
        public static (string, int) GetBoltFlange(Document doc, ElementId id)
        {
            ClsRevitUtil.CustomDataGet(doc, id, "ボルトF", out string bolt);
            ClsRevitUtil.CustomDataGet(doc, id, "ボルトF数", out int num);
            return (bolt, num);
        }
        /// <summary>
        /// 対象のファミリにボルト種類と数をカスタムデータとして持たせる(ウェブ)
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id">ファミリ</param>
        /// <param name="bolt">ボルト種類</param>
        /// <param name="num">数</param>
        /// <returns></returns>
        public static bool SetBoltWeb(Document doc, ElementId id, string bolt, int num)
        {
            if (!ClsRevitUtil.CustomDataSet(doc, id, "ボルトW", bolt))
                return false;
            if (!ClsRevitUtil.CustomDataSet(doc, id, "ボルトW数", num))
                return false;
            return true;
        }
        /// <summary>
        /// 対象のファミリからボルト種類と数のカスタムデータを取得する(ウェブ)
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id">ファミリ</param>
        /// <returns></returns>
        public static (string, int) GetBoltWeb(Document doc, ElementId id)
        {
            ClsRevitUtil.CustomDataGet(doc, id, "ボルトW", out string bolt);
            ClsRevitUtil.CustomDataGet(doc, id, "ボルトW数", out int num);
            return (bolt, num);
        }
        /// <summary>
        /// 対象のファミリにボルト種類と数をカスタムデータとして持たせる
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id">ファミリ</param>
        /// <param name="bolt">ボルト種類</param>
        /// <param name="num">数</param>
        /// <returns></returns>
        public static bool SetBolt(Document doc, ElementId id, string bolt, int num)
        {
            if (!ClsRevitUtil.CustomDataSet(doc, id, "ボルト", bolt))
                return false;
            if (!ClsRevitUtil.CustomDataSet(doc, id, "ボルト数", num))
                return false;
            return true;
        }
        /// <summary>
        /// 対象のファミリからボルト種類と数のカスタムデータを取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id">ファミリ</param>
        /// <returns></returns>
        public static (string, int) GetBolt(Document doc, ElementId id)
        {
            ClsRevitUtil.CustomDataGet(doc, id, "ボルト", out string bolt);
            ClsRevitUtil.CustomDataGet(doc, id, "ボルト数", out int num);
            return (bolt, num);
        }

        public static void DeleteBolt(Document doc, ElementId id)
        {
            ClsRevitUtil.CustomDataDelete(doc, id, "ボルト");
            ClsRevitUtil.CustomDataDelete(doc, id, "ボルト数");
            return;
        }

        /// <summary>
        /// 対象のファミリに固定方法をカスタムデータとして持たせる
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id">ファミリ</param>
        /// <param name="kotei">固定方法</param>
        /// <returns></returns>
        public static bool SetKotei(Document doc, ElementId id, Master.Kotei kotei)
        {
            string strKotei = (kotei == Master.Kotei.Bolt ? Master.ClsPileTsugiteCsv.KoteiHohoBolt : Master.ClsPileTsugiteCsv.KoteiHohoYousetsu);
            return ClsRevitUtil.CustomDataSet(doc, id, "固定", strKotei);
        }
        /// <summary>
        /// 対象のファミリから固定方法のカスタムデータを取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id">ファミリ</param>
        /// <returns></returns>
        public static string GetKotei(Document doc, ElementId id)
        {
            ClsRevitUtil.CustomDataGet(doc, id, "固定", out string kotei);
            return kotei;
        }

        /// <summary>
        /// 腹起ベースと接続するベースのサイズ差を取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="baseId"></param>
        /// <param name="diff"></param>
        /// <returns></returns>
        public static bool GetDifferenceWithBase(Document doc, ElementId baseId, out double diff, string syuzaiSize = "")
        {
            diff = 0.0;

            // 対象のベースの始点を取得
            FamilyInstance inst = doc.GetElement(baseId) as FamilyInstance;
            if (inst == null)
            {
                return false;
            }

            LocationCurve lCurve = inst.Location as LocationCurve;
            if (lCurve == null)
            {
                return false;
            }

            Curve cv = lCurve.Curve;
            XYZ baseSp = cv.GetEndPoint(0);

            // 接続先の腹起ベースを取得
            List<ElementId> elements = ClsRevitUtil.GetAllCreatedFamilyInstanceList(doc);
            List<ElementId> haraokoshiBaseIds = new List<ElementId>();
            foreach (ElementId elem in elements)
            {
                if (elem != null && doc.GetElement(elem).Name == "腹起ベース")
                {
                    haraokoshiBaseIds.Add(elem);
                }
            }

            if (haraokoshiBaseIds.Count() == 0)
            {
                return false;
            }

            foreach (var id in haraokoshiBaseIds)
            {
                FamilyInstance instHaraokoshiBase = doc.GetElement(baseId) as FamilyInstance;
                LocationCurve lcvHaraokoshiBase = instHaraokoshiBase.Location as LocationCurve;
                Curve cvHaraokosiBase = lCurve.Curve;
                if (ClsRevitUtil.IsPointOnCurve(cvHaraokosiBase, baseSp))
                {
                    try
                    {
                        string haraokoshiSize = ClsRevitUtil.GetParameter(doc, id, "鋼材サイズ");
                        if (haraokoshiSize == "80SMH") { haraokoshiSize = "40HA"; };
                        string haraValue = Regex.Match(haraokoshiSize, @"\d+").Value;
                        string kiribariSize = ClsRevitUtil.GetParameter(doc, baseId, "鋼材サイズ");
                        if (syuzaiSize != "") { kiribariSize = syuzaiSize; };
                        if (kiribariSize == "60SMH") { kiribariSize = "40HA"; };
                        string kiriValue = Regex.Match(kiribariSize, @"\d+").Value;
                        double tmp = double.Parse(haraValue) - double.Parse(kiriValue);
                        diff = ClsRevitUtil.CovertToAPI((tmp * 10) / 2);
                        break;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool GetDifferenceWithAllBase(Document doc, ElementId baseId, out double diff, out double diff2)
        {
            diff = 0.0;
            diff2 = 0.0;


            // 対象のベースの始点を取得
            FamilyInstance inst = doc.GetElement(baseId) as FamilyInstance;
            if (inst == null)
            {
                return false;
            }
            string baseName = inst.Name;
            ElementId id = baseId;

            //ダブルの場合
            if (ClsRevitUtil.GetParameter(doc, baseId, "構成") == "ダブル")
            {
                return GetDifferenceWithBaseDouble(doc, id, out diff, out diff2);
            }
            else
            {
                switch (baseName)
                {
                    case ClsKiribariBase.baseName:
                        {
                            return GetDifferenceWithBase(doc, id, out diff, out diff2);
                        }
                    case ClsCornerHiuchiBase.baseName:
                        {
                            return GetDifferenceWithBase(doc, id, out diff, out diff2);
                        }
                    case ClsKiribariHiuchiBase.baseName:
                        {
                            return GetDifferenceWithBase(doc, id, out diff, out diff2);
                        }
                    case ClsKiribariUkeBase.baseName:
                        {
                            //交叉してる切梁ベースを探す
                            List<ElementId> lstHari = YMS.ClsYMSUtil.GetIntersectionKiribariBase(doc, baseId);
                            if (0 < lstHari.Count)
                                id = lstHari[0];
                            return GetDifferenceWithBase(doc, id, out diff, out diff2);
                        }
                    case ClsKiribariTsunagizaiBase.baseName:
                        {
                            //交叉してる切梁ベースを探す
                            List<ElementId> lstHari = YMS.ClsYMSUtil.GetIntersectionKiribariBase(doc, baseId);
                            if (0 < lstHari.Count)
                                id = lstHari[0];
                            return GetDifferenceWithBase(doc, id, out diff, out diff2);
                        }
                    case ClsHiuchiTsunagizaiBase.baseName:
                        {
                            //交叉してる火打ベースを探す
                            List<ElementId> lstHari = YMS.ClsYMSUtil.GetIntersectionHiuchiBase(doc, baseId);
                            if (0 < lstHari.Count)
                                id = lstHari[0];
                            return GetDifferenceWithBase(doc, id, out diff, out diff2);
                        }
                    case ClsKiribariTsugiBase.baseName:
                        {
                            //交叉してる切梁ベースを探す
                            List<ElementId> lstHari = YMS.ClsYMSUtil.GetIntersectionKiribariBase(doc, baseId);
                            if (0 < lstHari.Count)
                                id = lstHari[0];
                            return GetDifferenceWithBase(doc, id, out diff, out diff2);
                        }
                    default:
                        return false;
                }
            }
        }
        /// <summary>
        /// 切梁or隅火打ベースと接続する腹起ベースのサイズ差を取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="baseId">切梁or隅火打or切梁火打のみ</param>
        /// <param name="diff"></param>
        /// <returns></returns>
        public static bool GetDifferenceWithBase(Document doc, ElementId baseId, out double diff, out double diff2)
        {
            diff = 0.0;
            diff2 = 0.0;

            //ダブルの場合はサイズ差を無視する
            if (ClsRevitUtil.GetParameter(doc, baseId, "構成") == "ダブル")
                return false;

            // 対象のベースの始点を取得
            FamilyInstance inst = doc.GetElement(baseId) as FamilyInstance;
            if (inst == null)
            {
                return false;
            }
            ElementId levelId = inst.Host.Id;

            //切梁or隅火打or切梁火打以外を元に差分を見ない
            if (inst.Name != ClsKiribariBase.baseName && inst.Name != ClsCornerHiuchiBase.baseName && inst.Name != ClsKiribariHiuchiBase.baseName)
                return false;

            LocationCurve lCurve = inst.Location as LocationCurve;
            if (lCurve == null)
            {
                return false;
            }
            Curve cv = lCurve.Curve;

            // 図面上で接している腹起ベースを取得
            List<ElementId> insecBaseIds = ClsHaraokoshiBase.GetIntersectionBase(doc, baseId, levelId);
            if (insecBaseIds.Count() == 0)
            {
                return false;
            }

            string kouzaiSize = ClsRevitUtil.GetParameter(doc, baseId, "鋼材サイズ");
            double baseSize = Master.ClsYamadomeCsv.GetWidth(kouzaiSize);
            if (inst.Name == ClsKiribariHiuchiBase.baseName)
            {
                kouzaiSize = ClsRevitUtil.GetParameter(doc, baseId, "鋼材サイズ(シングル)");
                baseSize = Master.ClsYamadomeCsv.GetWidth(kouzaiSize);
            }
            foreach (var id in insecBaseIds)
            {
                FamilyInstance instInsec = doc.GetElement(id) as FamilyInstance;
                LocationCurve lCurveInsec = instInsec.Location as LocationCurve;
                Curve cvInsec = lCurveInsec.Curve;
                XYZ insec = ClsRevitUtil.GetIntersection(cv, cvInsec);
                if (insec != null)
                {
                    try
                    {
                        string haraokoshiSize = ClsRevitUtil.GetParameter(doc, id, "鋼材サイズ");
                        double harasize = Master.ClsYamadomeCsv.GetWidth(haraokoshiSize);
                        diff = ClsRevitUtil.CovertToAPI((harasize - baseSize) / 2);
                        diff2 = diff;
                        break;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 切梁or隅火打ベースと接続する腹起ベースのサイズ差を取得する（ダブル専用）
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="baseId">切梁or隅火打のみ</param>
        /// <param name="diff1">始点側の鋼材差</param>
        /// <param name="diff2">終点側の鋼材差</param>
        /// <returns></returns>
        public static bool GetDifferenceWithBaseDouble(Document doc, ElementId baseId, out double diff1, out double diff2)
        {
            diff1 = 0.0;
            diff2 = 0.0;

            //ダブル専用処理
            if (ClsRevitUtil.GetParameter(doc, baseId, "構成") != "ダブル")
                return false;

            // 対象のベースの始点を取得
            FamilyInstance inst = doc.GetElement(baseId) as FamilyInstance;
            if (inst == null)
            {
                return false;
            }
            ElementId levelId = inst.Host.Id;
            string baseName = inst.Name;

            //切梁火打or隅火打or切梁継ぎ以外を元に差分を見ない
            if (baseName != ClsKiribariHiuchiBase.baseName && baseName != ClsCornerHiuchiBase.baseName && baseName != ClsKiribariTsugiBase.baseName)
                return false;

            LocationCurve lCurve = inst.Location as LocationCurve;
            if (lCurve == null)
            {
                return false;
            }
            Curve cv = lCurve.Curve;

            double baseSize1 = Master.ClsYamadomeCsv.GetWidth(ClsRevitUtil.GetParameter(doc, baseId, "鋼材サイズ"));
            double baseSize2 = Master.ClsYamadomeCsv.GetWidth(ClsRevitUtil.GetParameter(doc, baseId, "鋼材サイズ"));
            switch (baseName)
            {
                case ClsCornerHiuchiBase.baseName:
                    {
                        break;
                    }
                case ClsKiribariHiuchiBase.baseName:
                    {
                        baseSize1 = Master.ClsYamadomeCsv.GetWidth(ClsRevitUtil.GetParameter(doc, baseId, "鋼材サイズ(ダブル上)"));
                        baseSize2 = Master.ClsYamadomeCsv.GetWidth(ClsRevitUtil.GetParameter(doc, baseId, "鋼材サイズ(ダブル下)"));
                        break;
                    }
                case ClsKiribariTsugiBase.baseName:
                    {
                        baseSize1 = Master.ClsYamadomeCsv.GetWidth(ClsRevitUtil.GetParameter(doc, baseId, "切梁側/鋼材サイズ(ダブル)"));
                        baseSize2 = Master.ClsYamadomeCsv.GetWidth(ClsRevitUtil.GetParameter(doc, baseId, "腹起側/鋼材サイズ(ダブル)"));
                        break;
                    }
                default:
                    return false;
            }

            // 図面上で接している腹起ベースと切梁ベースを取得
            List<ElementId> insecBaseIds = GetIntersectionHaraKiriBase(doc, baseId);
            if (insecBaseIds.Count() == 0)
            {
                return false;
            }
            foreach (var id in insecBaseIds)
            {
                FamilyInstance instInsec = doc.GetElement(id) as FamilyInstance;
                LocationCurve lCurveInsec = instInsec.Location as LocationCurve;
                Curve cvInsec = lCurveInsec.Curve;
                XYZ insec = ClsRevitUtil.GetIntersection(cv, cvInsec);
                if (insec != null)
                {
                    try
                    {
                        string dan = ClsRevitUtil.GetParameter(doc, id, "段");
                        string insecKouzaiSize = ClsRevitUtil.GetParameter(doc, id, "鋼材サイズ");
                        double insecBaseSize = Master.ClsYamadomeCsv.GetWidth(insecKouzaiSize);
                        if (ClsRevitUtil.CheckNearGetEndPoint(cv, insec))
                        {
                            //if(dan == "上段")
                            //{
                            //    diff1 = ClsRevitUtil.CovertToAPI((insecBaseSize - baseSize1) / 2);
                            //}
                            //else
                            //{
                            //    diff2 = ClsRevitUtil.CovertToAPI((insecBaseSize - baseSize2) / 2);
                            //}
                            diff1 = ClsRevitUtil.CovertToAPI((insecBaseSize - baseSize1) / 2);
                        }
                        else
                        {
                            //if (dan == "上段")
                            //{
                            //    diff1 = ClsRevitUtil.CovertToAPI((insecBaseSize - baseSize1) / 2);
                            //}
                            //else
                            //{
                            //    diff2 = ClsRevitUtil.CovertToAPI((insecBaseSize - baseSize2) / 2);
                            //}
                            diff2 = ClsRevitUtil.CovertToAPI((insecBaseSize - baseSize2) / 2);
                        }
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static void SetRevitAsOwner(System.Windows.Forms.Form form)
        {
            var revitHandle = Process.GetCurrentProcess().MainWindowHandle;
            NativeMethods.SetWindowLong(form.Handle, -8 /* GWL_HWNDPARENT */, revitHandle);
        }

        public static class NativeMethods
        {
            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
        }
        #endregion

        #region RevitUtil

        /// <summary>
        /// JSONファイル選択ダイアログ
        /// </summary>
        /// <param name="selectedPath"></param>
        /// <returns></returns>
        public static bool SelectJSONFile(ref string selectedPath)
        {
            System.Windows.Forms.OpenFileDialog sfd = new System.Windows.Forms.OpenFileDialog();
            //sfd.FileName = ;
            //sfd.InitialDirectory = ;
            sfd.Filter = "JSONファイル(*.json)|*.json|すべてのファイル(*.*)|*.*";
            sfd.Title = "ファイルを選択してください";
            sfd.RestoreDirectory = true;
            if (System.Windows.Forms.DialogResult.OK != sfd.ShowDialog())
            {
                return false;
            }

            selectedPath = sfd.FileName;
            return true;
        }


        #region オブジェクトが乗っていない箇所の線分座標群を取得

        /// <summary>
        /// オブジェクトが乗っていない箇所の線分座標群を取得
        /// </summary>
        /// <param name="doc">ドキュメント</param>
        /// <param name="id">対象の線分のエレメントID</param>
        /// <returns></returns>
        public static List<List<XYZ>> GetNonObjectBaseLinePoints(Document doc, ElementId id)
        {
            List<List<XYZ>> res = new List<List<XYZ>>();
            try
            {

                View3D view3D = Get3DView(doc);

                FamilyInstance shinInstLevel = doc.GetElement(id) as FamilyInstance;
                LocationCurve lCurve = shinInstLevel.Location as LocationCurve;
                if (lCurve == null)
                {
                    return res;
                }
                XYZ tmpStPoint = lCurve.Curve.GetEndPoint(0);
                XYZ tmpEdPoint = lCurve.Curve.GetEndPoint(1);

                XYZ center = tmpStPoint;
                XYZ rayDirection = tmpEdPoint - tmpStPoint;

                //線分方向の衝突エレメントを取得
                ReferenceIntersector refIntersector2 = new ReferenceIntersector(view3D);
                IList<ReferenceWithContext> referenceWithContext2 = refIntersector2.Find(center, rayDirection);
                List<Reference> refs = new List<Reference>();
                foreach (var x in referenceWithContext2)
                {
                    Reference r = x.GetReference();
                    refs.Add(r);
                }

                //線分上のエレメントIDリストの取得
                List<ElementId> lstElementId = new List<ElementId>();
                lstElementId = (from data in refs select data.ElementId).Distinct().ToList();
                if (lstElementId == null || lstElementId.Count == 0)
                {
                    List<XYZ> lstXYZ = new List<XYZ>() { tmpStPoint, tmpEdPoint };
                    res.Add(lstXYZ);
                    return res;
                }

                //エレメントID毎に近い座標と遠い座標を取得
                List<KeyValuePair<ElementId, List<XYZ>>> lstPair = new List<KeyValuePair<ElementId, List<XYZ>>>();
                foreach (ElementId el in lstElementId)
                {
                    List<XYZ> lstXYZ = (from data in refs where data.ElementId == el select data.GlobalPoint).ToList();

                    XYZ max = lstXYZ.OrderByDescending(item => item.DistanceTo(tmpStPoint)).First();
                    XYZ min = lstXYZ.OrderByDescending(item => item.DistanceTo(tmpStPoint)).Last();

                    if (max.DistanceTo(min) < 1 ||//ClsRevitUtil.CovertToAPI(1) ||
                        tmpStPoint.DistanceTo(min) > tmpStPoint.DistanceTo(tmpEdPoint))
                    {
                        continue;
                    }

                    List<XYZ> pairXYZ = new List<XYZ>() { min, max };
                    KeyValuePair<ElementId, List<XYZ>> pair = new KeyValuePair<ElementId, List<XYZ>>(el, pairXYZ);
                    lstPair.Add(pair);
                }
                lstPair = lstPair.OrderBy(item => item.Value[0].DistanceTo(tmpStPoint)).ToList();

                //線分しか関連していない場合
                if (lstPair.Count == 0)
                {
                    List<XYZ> lstXYZ = new List<XYZ>() { tmpStPoint, tmpEdPoint };
                    res.Add(lstXYZ);
                    return res;
                }

                XYZ max2 = new XYZ();
                XYZ min2 = tmpStPoint;
                for (int i = 0; i < lstPair.Count(); i++)
                {
                    KeyValuePair<ElementId, List<XYZ>> p = lstPair[i];

                    if (i == 0) //始点の場合、上に部材が載っているか判定
                    {
                        if (CheckOntheElement(doc, min2, p.Key))
                        {
                            min2 = p.Value[1];
                            continue;
                        }
                    }

                    max2 = p.Value[0];
                    if (min2.DistanceTo(max2) <= 0)
                    {
                        continue;
                    }

                    res.Add(new List<XYZ>() { min2, max2 });

                    min2 = p.Value[1];
                }

                //終点上に部材が載っているか判定
                if (!CheckOntheElement(doc, tmpEdPoint, lstPair.Last().Key))
                {
                    res.Add(new List<XYZ>() { min2, tmpEdPoint });
                }

                ////テスト用コード※通常はコメントアウト
                //foreach (List<XYZ> lst in res)
                //{
                //    using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                //    {
                //        ElementId levelID = ClsRevitUtil.GetLevelID(doc, "3段目");
                //        Curve cv = Line.CreateBound(lst[0], lst[1]);
                //        t.Start();

                //        string symbolFolpath = ClsZumenInfo.GetYMSFolder();
                //        string shinfamily = System.IO.Path.Combine(symbolFolpath, "山留壁ファミリ\\" + "隅火打ベース" + ".rfa");
                //        //シンボル読込
                //        if (!ClsRevitUtil.RoadFamilySymbolData(doc, shinfamily, "隅火打ベース", out FamilySymbol sym))
                //        {
                //            return res;
                //        }
                //        ElementId CreatedID = ClsRevitUtil.Create(doc, cv, levelID, sym);
                //        t.Commit();
                //    }
                //}

                return res;
            }
            catch
            {
                return res;
            }
        }
        /// <summary>
        /// ダブル用オブジェクトが乗っていない箇所の線分座標群を取得
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="tmpStPoint">始点(Z軸を上下段でずらす)</param>
        /// <param name="tmpEdPoint">終点(Z軸を上下段でずらす)</param>
        /// <param name="Nop">特定のファミリを検索対象から外す　（杭　ブルマン　リキマン）</param>
        /// <returns></returns>
        public static List<List<XYZ>> GetNonObjectBaseLinePoints(Document doc, XYZ tmpStPoint, XYZ tmpEdPoint, bool Nop = true)
        {
            List<List<XYZ>> res = new List<List<XYZ>>();
            try
            {
                View3D view3D = Get3DView(doc);

                XYZ center = tmpStPoint;
                XYZ rayDirection = tmpEdPoint - tmpStPoint;

                //線分方向の衝突エレメントを取得
                ReferenceIntersector refIntersector2 = new ReferenceIntersector(view3D);
                IList<ReferenceWithContext> referenceWithContext2 = refIntersector2.Find(center, rayDirection);
                List<Reference> refs = new List<Reference>();
                foreach (var x in referenceWithContext2)
                {
                    Reference r = x.GetReference();

                    ImportInstance importInst = doc.GetElement(r) as ImportInstance;
                    if (importInst != null)
                    {
                        //#32407
                        continue;
                    }

                    if (Nop)
                    {
                        //#31697
                        FamilyInstance inst = doc.GetElement(r) as FamilyInstance;
                        if (inst != null)
                        {
                            if (inst.Name.Contains("杭"))
                            {
                                continue;
                            }
                            if (inst.Name.Contains("鋼矢板"))
                            {
                                continue;
                            }
                            if (inst.Symbol.FamilyName.Contains("ﾌﾞﾙﾏﾝ") || inst.Symbol.FamilyName.Contains("ﾘｷﾏﾝ"))
                            {
                                continue;
                            }
                            if (inst.Symbol.FamilyName.Contains("ﾌﾞﾗｹｯﾄ"))
                            {
                                continue;
                            }
                            if (inst.Symbol.FamilyName.Contains("ｽﾁﾌﾅｰ"))
                            {
                                continue;
                            }
                            if (inst.Symbol.FamilyName.Contains("カバープレート"))
                            {
                                continue;
                            }
                            if (inst.Symbol.FamilyName.Contains("隅部ピース"))
                            {
                                continue;
                            }
                        }

                    }
                    refs.Add(r);
                }

                //線分上のエレメントIDリストの取得
                List<ElementId> lstElementId = new List<ElementId>();
                lstElementId = (from data in refs select data.ElementId).Distinct().ToList();
                if (lstElementId == null || lstElementId.Count == 0)
                {
                    List<XYZ> lstXYZ = new List<XYZ>() { tmpStPoint, tmpEdPoint };
                    res.Add(lstXYZ);
                    return res;
                }

                //エレメントID毎に近い座標と遠い座標を取得
                List<KeyValuePair<ElementId, List<XYZ>>> lstPair = new List<KeyValuePair<ElementId, List<XYZ>>>();
                foreach (ElementId el in lstElementId)
                {
                    FamilyInstance inst = doc.GetElement(el) as FamilyInstance;
                    if (inst != null)//ファミリ名でフィルター
                    {
                        string name = inst.Symbol.FamilyName;
                    }
                    List<XYZ> lstXYZ = (from data in refs where data.ElementId == el select data.GlobalPoint).ToList();

                    XYZ max = lstXYZ.OrderByDescending(item => item.DistanceTo(tmpStPoint)).First();
                    XYZ min = lstXYZ.OrderByDescending(item => item.DistanceTo(tmpStPoint)).Last();

                    if (max.DistanceTo(min) < ClsRevitUtil.CovertToAPI(15) || //99) ||//#31638対応で変えてみたが問題あるかは今後検証
                        tmpStPoint.DistanceTo(min) > tmpStPoint.DistanceTo(tmpEdPoint))
                    {
                        continue;
                    }

                    List<XYZ> pairXYZ = new List<XYZ>() { min, max };
                    KeyValuePair<ElementId, List<XYZ>> pair = new KeyValuePair<ElementId, List<XYZ>>(el, pairXYZ);
                    lstPair.Add(pair);
                }
                lstPair = lstPair.OrderBy(item => item.Value[0].DistanceTo(tmpStPoint)).ToList();

                //線分しか関連していない場合
                if (lstPair.Count == 0)
                {
                    List<XYZ> lstXYZ = new List<XYZ>() { tmpStPoint, tmpEdPoint };
                    res.Add(lstXYZ);
                    return res;
                }

                XYZ max2 = new XYZ();
                XYZ min2 = tmpStPoint;
                for (int i = 0; i < lstPair.Count(); i++)
                {
                    KeyValuePair<ElementId, List<XYZ>> p = lstPair[i];

                    if (i == 0) //始点の場合、上に部材が載っているか判定
                    {
                        if (CheckOntheElement(doc, min2, p.Key))
                        {
                            min2 = p.Value[1];
                            continue;
                        }
                    }

                    max2 = p.Value[0];
                    if (min2.DistanceTo(max2) < 0)
                    {
                        continue;
                    }

                    if (!ClsGeo.GEO_EQ(min2, max2))//同一の点上になっていないか判定
                    {
                        res.Add(new List<XYZ>() { min2, max2 });
                    }

                    min2 = p.Value[1];
                }

                //終点上に部材が載っているか判定
                if (!CheckOntheElement(doc, tmpEdPoint, lstPair.Last().Key))
                {
                    res.Add(new List<XYZ>() { min2, tmpEdPoint });
                }
                return res;
            }
            catch
            {
                return res;
            }
        }
        /// <summary>
        /// オブジェクトが乗っているかの判定
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="tmpStPoint">始点(Z軸を上下段でずらす)</param>
        /// <param name="tmpEdPoint">終点(Z軸を上下段でずらす)</param>
        /// <param name="Nop">特定のファミリを検索対象から外す　（杭　ブルマン　リキマン）</param>
        /// <returns></returns>
        public static bool GetNonObjectFlagBaseLinePoints(Document doc, XYZ tmpStPoint, XYZ tmpEdPoint, double size, bool reverse = false, bool Nop = true)
        {
            tmpStPoint = new XYZ(tmpStPoint.X, tmpStPoint.Y, tmpStPoint.Z + size);
            tmpEdPoint = new XYZ(tmpEdPoint.X, tmpEdPoint.Y, tmpEdPoint.Z + size);
            List<List<XYZ>> res = new List<List<XYZ>>();
            try
            {
                View3D view3D = Get3DView(doc);

                XYZ center = tmpStPoint;
                XYZ rayDirection = tmpEdPoint - tmpStPoint;

                if (reverse)
                {
                    center = tmpEdPoint;
                    rayDirection = tmpStPoint - tmpEdPoint;
                }

                //線分方向の衝突エレメントを取得
                ReferenceIntersector refIntersector2 = new ReferenceIntersector(view3D);
                IList<ReferenceWithContext> referenceWithContext2 = refIntersector2.Find(center, rayDirection);
                List<Reference> refs = new List<Reference>();
                foreach (var x in referenceWithContext2)
                {
                    Reference r = x.GetReference();

                    ImportInstance importInst = doc.GetElement(r) as ImportInstance;
                    if (importInst != null)
                    {
                        //#32407
                        continue;
                    }

                    if (Nop)
                    {
                        //#31697
                        FamilyInstance inst = doc.GetElement(r) as FamilyInstance;
                        if (inst != null)
                        {
                            if (inst.Name.Contains("杭"))
                            {
                                continue;
                            }
                            if (inst.Name.Contains("鋼矢板"))
                            {
                                continue;
                            }
                            if (inst.Symbol.FamilyName.Contains("ﾌﾞﾙﾏﾝ") || inst.Symbol.FamilyName.Contains("ﾘｷﾏﾝ"))
                            {
                                continue;
                            }
                            if (inst.Symbol.FamilyName.Contains("ﾌﾞﾗｹｯﾄ"))
                            {
                                continue;
                            }
                            if (inst.Symbol.FamilyName.Contains("ｽﾁﾌﾅｰ"))
                            {
                                continue;
                            }
                            if (inst.Symbol.FamilyName.Contains("カバープレート"))
                            {
                                continue;
                            }
                            if (inst.Symbol.FamilyName.Contains("隅部ピース"))
                            {
                                continue;
                            }
                        }

                    }
                    refs.Add(r);
                }

                //線分上のエレメントIDリストの取得
                List<ElementId> lstElementId = new List<ElementId>();
                lstElementId = (from data in refs select data.ElementId).Distinct().ToList();
                if (lstElementId == null || lstElementId.Count == 0)
                {
                    List<XYZ> lstXYZ = new List<XYZ>() { tmpStPoint, tmpEdPoint };
                    res.Add(lstXYZ);
                    if (res.Count == 0)
                        return false;
                    else
                        return true;
                }

                //エレメントID毎に近い座標と遠い座標を取得
                List<KeyValuePair<ElementId, List<XYZ>>> lstPair = new List<KeyValuePair<ElementId, List<XYZ>>>();
                foreach (ElementId el in lstElementId)
                {
                    FamilyInstance inst = doc.GetElement(el) as FamilyInstance;
                    if (inst != null)//ファミリ名でフィルター
                    {
                        string name = inst.Symbol.FamilyName;
                    }
                    List<XYZ> lstXYZ = (from data in refs where data.ElementId == el select data.GlobalPoint).ToList();

                    XYZ max = lstXYZ.OrderByDescending(item => item.DistanceTo(tmpStPoint)).First();
                    XYZ min = lstXYZ.OrderByDescending(item => item.DistanceTo(tmpStPoint)).Last();

                    if (max.DistanceTo(min) < ClsRevitUtil.CovertToAPI(15) || //99) ||//#31638対応で変えてみたが問題あるかは今後検証
                        tmpStPoint.DistanceTo(min) > tmpStPoint.DistanceTo(tmpEdPoint))
                    {
                        continue;
                    }

                    List<XYZ> pairXYZ = new List<XYZ>() { min, max };
                    KeyValuePair<ElementId, List<XYZ>> pair = new KeyValuePair<ElementId, List<XYZ>>(el, pairXYZ);
                    lstPair.Add(pair);
                }
                lstPair = lstPair.OrderBy(item => item.Value[0].DistanceTo(tmpStPoint)).ToList();

                //線分しか関連していない場合
                if (lstPair.Count == 0)
                {
                    List<XYZ> lstXYZ = new List<XYZ>() { tmpStPoint, tmpEdPoint };
                    res.Add(lstXYZ);
                    if (res.Count == 0)
                        return false;
                    else
                        return true;
                }

                XYZ max2 = new XYZ();
                XYZ min2 = tmpStPoint;
                for (int i = 0; i < lstPair.Count(); i++)
                {
                    KeyValuePair<ElementId, List<XYZ>> p = lstPair[i];

                    if (i == 0) //始点の場合、上に部材が載っているか判定
                    {
                        if (CheckOntheElement(doc, min2, p.Key))
                        {
                            min2 = p.Value[1];
                            continue;
                        }
                    }

                    max2 = p.Value[0];
                    if (min2.DistanceTo(max2) < 0)
                    {
                        continue;
                    }

                    if (!ClsGeo.GEO_EQ(min2, max2))//同一の点上になっていないか判定
                    {
                        res.Add(new List<XYZ>() { min2, max2 });
                    }

                    min2 = p.Value[1];
                }

                //終点上に部材が載っているか判定
                if (!CheckOntheElement(doc, tmpEdPoint, lstPair.Last().Key))
                {
                    res.Add(new List<XYZ>() { min2, tmpEdPoint });
                }
                if (res.Count == 0)
                    return false;
                else
                    return true;
            }
            catch
            {
                if (res.Count == 0)
                    return false;
                else
                    return true;
            }
        }
        /// <summary>
        /// 始終点間に衝突した部材の一覧を取得する　（火打ち系は全て無視）（山留材は切梁のみ対象とする）（取得部材に接触しているカバープレートとジャッキカバーも対象に含める）
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="tmpStPoint"></param>
        /// <param name="tmpEdPoint"></param>
        /// <param name="hitIdList"></param>
        /// <returns></returns>
        public static List<List<XYZ>> GetObjectOnBaseLine(Document doc, XYZ tmpStPoint, XYZ tmpEdPoint, ref List<ElementId> hitIdList)
        {
            List<List<XYZ>> res = new List<List<XYZ>>();
            try
            {
                View3D view3D = Get3DView(doc);

                XYZ center = tmpStPoint;
                XYZ rayDirection = tmpEdPoint - tmpStPoint;

                //線分方向の衝突エレメントを取得
                ReferenceIntersector refIntersector2 = new ReferenceIntersector(view3D);
                IList<ReferenceWithContext> referenceWithContext2 = refIntersector2.Find(center, rayDirection);
                List<Reference> refs = new List<Reference>();
                foreach (var x in referenceWithContext2)
                {
                    Reference r = x.GetReference();
                    ImportInstance importInst = doc.GetElement(r) as ImportInstance;
                    if (importInst != null)
                    {
                        //#32407
                        continue;
                    }

                    FamilyInstance inst = doc.GetElement(r) as FamilyInstance;
                    if (inst != null)
                    {
                        if (Master.ClsHiuchiCsv.GetFamilyNameList().ToList().Contains(inst.Symbol.FamilyName))
                        {
                            continue;
                        }

                        if (Master.ClsYamadomeCsv.GetYamadomeFamilynameList().ToList().Contains(inst.Symbol.FamilyName) ||
                            Master.ClsYamadomeCsv.GetKariFamilyNameList().ToList().Contains(inst.Symbol.FamilyName))
                        {
                            if (!inst.Symbol.Name.Contains("切梁"))
                            {
                                continue;
                            }
                        }

                        List<string> kouzaiList = new List<string>();
                        kouzaiList.AddRange(Master.ClsYamadomeCsv.GetYamadomeFamilynameList().ToList());
                        kouzaiList.AddRange(Master.ClsSupportPieceCsv.GetFamilyNameList().ToList());
                        kouzaiList.AddRange(Master.ClsCoverPlateCSV.GetFamilyNameList().ToList());
                        kouzaiList.AddRange(Master.ClsJackCsv.GetFamilyNameList().ToList());
                        kouzaiList.AddRange(Master.ClsJackCoverCSV.GetFamilyNameList().ToList());
                        kouzaiList.AddRange(Master.ClsCoverPlateCSV.GetFamilyNameList().ToList());
                        kouzaiList.AddRange(Master.ClsSanjikuPieceCSV.GetFamilyNameList().ToList());
                        if (!kouzaiList.Contains(inst.Symbol.FamilyName))
                        {
                            continue;
                        }

                    }

                    refs.Add(r);
                }

                //線分上のエレメントIDリストの取得
                List<ElementId> lstElementId = new List<ElementId>();
                lstElementId = (from data in refs select data.ElementId).Distinct().ToList();
                if (lstElementId == null || lstElementId.Count == 0)
                {
                    List<XYZ> lstXYZ = new List<XYZ>() { tmpStPoint, tmpEdPoint };
                    res.Add(lstXYZ);
                    return res;
                }

                hitIdList.AddRange(lstElementId);

                List<ElementId> kensakuIds = new List<ElementId>();
                List<ElementId> plusBuzaiIds = new List<ElementId>();
                //図面上のカバープレートを全て取得
                List<ElementId> plateIds = ClsCoverPlate.GetAllCoverplate(doc);
                List<ElementId> kiribariPlateIds2 = new List<ElementId>();
                foreach (ElementId pid in plateIds)
                {
                    FamilyInstance inst = doc.GetElement(pid) as FamilyInstance;
                    if (inst == null)
                    {
                        continue;
                    }
                    if (inst.Symbol.Name == "切梁")
                    {
                        kiribariPlateIds2.Add(pid);
                    }
                }

                if (kiribariPlateIds2.Count > 0)
                {
                    kensakuIds.AddRange(kiribariPlateIds2);
                }

                //図面上のジャッキカバーを全て取得
                List<ElementId> jackCoverList = ClsRevitUtil.GetSelectCreatedFamilyInstanceFamilySymbolList(doc, "ジャッキカバー", true);
                if (jackCoverList.Count > 0)
                {
                    kensakuIds.AddRange(jackCoverList);
                }

                //エレメントID毎に近い座標と遠い座標を取得
                List<KeyValuePair<ElementId, List<XYZ>>> lstPair = new List<KeyValuePair<ElementId, List<XYZ>>>();
                foreach (ElementId el in lstElementId)
                {
                    FamilyInstance inst = doc.GetElement(el) as FamilyInstance;
                    if (inst != null)//ファミリ名でフィルター
                    {
                        string name = inst.Symbol.FamilyName;
                    }

                    //ｶﾊﾞｰﾌﾟﾚｰﾄ　ジャッキカバーは上記検索にかからないのでここで捕まえる
                    if (kensakuIds.Count > 0)
                    {
                        //ジャッキカバーの仕様が変わったため
                        List<ElementId> Ids = ClsRevitUtil.GetIntersectFamilysToDependentElements(doc, inst.Id, 0.1, null, kensakuIds);
                        if (Ids.Count > 0)
                        {
                            foreach (ElementId getId in Ids)
                            {

                                FamilyInstance instSetuzoku = doc.GetElement(getId) as FamilyInstance;
                                if (instSetuzoku != null)//ファミリ名でフィルター
                                {
                                    if (Master.ClsJackCoverCSV.GetFamilyNameList().ToList().Contains(instSetuzoku.Symbol.FamilyName) ||
                                        Master.ClsCoverPlateCSV.GetFamilyNameList().ToList().Contains(instSetuzoku.Symbol.FamilyName))
                                    {
                                        if (!plusBuzaiIds.Contains(getId))
                                        {
                                            plusBuzaiIds.Add(getId);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    List<XYZ> lstXYZ = (from data in refs where data.ElementId == el select data.GlobalPoint).ToList();

                    XYZ max = lstXYZ.OrderByDescending(item => item.DistanceTo(tmpStPoint)).First();
                    XYZ min = lstXYZ.OrderByDescending(item => item.DistanceTo(tmpStPoint)).Last();

                    if (max.DistanceTo(min) < ClsRevitUtil.CovertToAPI(15) || //99) ||//#31638対応で変えてみたが問題あるかは今後検証
                        tmpStPoint.DistanceTo(min) > tmpStPoint.DistanceTo(tmpEdPoint))
                    {
                        continue;
                    }

                    List<XYZ> pairXYZ = new List<XYZ>() { min, max };
                    KeyValuePair<ElementId, List<XYZ>> pair = new KeyValuePair<ElementId, List<XYZ>>(el, pairXYZ);
                    lstPair.Add(pair);
                }
                lstPair = lstPair.OrderBy(item => item.Value[0].DistanceTo(tmpStPoint)).ToList();

                //線分しか関連していない場合
                if (lstPair.Count == 0)
                {
                    List<XYZ> lstXYZ = new List<XYZ>() { tmpStPoint, tmpEdPoint };
                    res.Add(lstXYZ);
                    return res;
                }

                XYZ max2 = new XYZ();
                XYZ min2 = tmpStPoint;
                for (int i = 0; i < lstPair.Count(); i++)
                {
                    KeyValuePair<ElementId, List<XYZ>> p = lstPair[i];
                    res.Add(new List<XYZ>() { p.Value[0], p.Value[1] });
                }

                if (plusBuzaiIds.Count > 0)
                {
                    hitIdList.AddRange(plusBuzaiIds);
                }

                return res;
            }
            catch
            {
                return res;
            }
        }

        /// <summary>
        /// 始終点間に衝突した部材の一覧を取得する　（火打ち系は全て無視）（山留材は切梁のみ対象とする）（取得部材に接触しているカバープレートとジャッキカバーも対象に含める）
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="tmpStPoint"></param>
        /// <param name="tmpEdPoint"></param>
        /// <param name="hitIdList"></param>
        /// <returns></returns>
        public static List<ElementId> GetObjectOnBaseLine2(Document doc, XYZ tmpStPoint, XYZ tmpEdPoint)
        {
            List<ElementId> ids = new List<ElementId>();

            try
            {
                View3D view3D = Get3DView(doc);

                XYZ center = tmpStPoint;
                XYZ rayDirection = tmpEdPoint - tmpStPoint;

                //線分方向の衝突エレメントを取得
                ReferenceIntersector refIntersector2 = new ReferenceIntersector(view3D);
                IList<ReferenceWithContext> referenceWithContext2 = refIntersector2.Find(center, rayDirection);
                List<Reference> refs = new List<Reference>();
                foreach (var x in referenceWithContext2)
                {
                    Reference r = x.GetReference();

                    refs.Add(r);
                }

                //線分上のエレメントIDリストの取得
                List<ElementId> lstElementId = new List<ElementId>();
                lstElementId = (from data in refs select data.ElementId).Distinct().ToList();
                if (lstElementId != null && lstElementId.Count > 0)
                {
                    ids.AddRange(lstElementId);
                }

                return ids;
            }
            catch
            {
                ids.Clear();
                return ids;
            }
        }


        /// <summary>
        /// ダブル用オブジェクトが乗っていない箇所の線分座標群を取得
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="tmpStPoint">始点(Z軸を上下段でずらす)</param>
        /// <param name="tmpEdPoint">終点(Z軸を上下段でずらす)</param>
        /// <returns></returns>
        public static List<List<XYZ>> GetNonObjectBaseLinePointsFilterObject(Document doc, XYZ tmpStPoint, XYZ tmpEdPoint, List<string> filterList)
        {
            List<List<XYZ>> res = new List<List<XYZ>>();
            try
            {
                View3D view3D = Get3DView(doc);

                XYZ center = tmpStPoint;
                XYZ rayDirection = tmpEdPoint - tmpStPoint;

                //線分方向の衝突エレメントを取得
                ReferenceIntersector refIntersector2 = new ReferenceIntersector(view3D);
                IList<ReferenceWithContext> referenceWithContext2 = refIntersector2.Find(center, rayDirection);
                List<Reference> refs = new List<Reference>();
                foreach (var x in referenceWithContext2)
                {
                    Reference r = x.GetReference();
                    refs.Add(r);
                }

                //線分上のエレメントIDリストの取得
                List<ElementId> lstElementId = new List<ElementId>();
                lstElementId = (from data in refs select data.ElementId).Distinct().ToList();
                if (lstElementId == null || lstElementId.Count == 0)
                {
                    List<XYZ> lstXYZ = new List<XYZ>() { tmpStPoint, tmpEdPoint };
                    res.Add(lstXYZ);
                    return res;
                }

                //エレメントID毎に近い座標と遠い座標を取得
                List<KeyValuePair<ElementId, List<XYZ>>> lstPair = new List<KeyValuePair<ElementId, List<XYZ>>>();
                foreach (ElementId el in lstElementId)
                {
                    FamilyInstance inst = doc.GetElement(el) as FamilyInstance;
                    if (inst != null)//ファミリ名でフィルター
                    {
                        bool match = false;
                        string name = inst.Symbol.FamilyName;
                        foreach (string filterName in filterList)
                        {
                            if (name == filterName)
                            {
                                match = true;
                                break;
                            }
                        }
                        if (match)
                            continue;
                    }
                    List<XYZ> lstXYZ = (from data in refs where data.ElementId == el select data.GlobalPoint).ToList();

                    XYZ max = lstXYZ.OrderByDescending(item => item.DistanceTo(tmpStPoint)).First();
                    XYZ min = lstXYZ.OrderByDescending(item => item.DistanceTo(tmpStPoint)).Last();

                    if (max.DistanceTo(min) < ClsRevitUtil.CovertToAPI(99) ||
                        tmpStPoint.DistanceTo(min) > tmpStPoint.DistanceTo(tmpEdPoint))
                    {
                        continue;
                    }

                    List<XYZ> pairXYZ = new List<XYZ>() { min, max };
                    KeyValuePair<ElementId, List<XYZ>> pair = new KeyValuePair<ElementId, List<XYZ>>(el, pairXYZ);
                    lstPair.Add(pair);
                }
                lstPair = lstPair.OrderBy(item => item.Value[0].DistanceTo(tmpStPoint)).ToList();

                //線分しか関連していない場合
                if (lstPair.Count == 0)
                {
                    List<XYZ> lstXYZ = new List<XYZ>() { tmpStPoint, tmpEdPoint };
                    res.Add(lstXYZ);
                    return res;
                }

                XYZ max2 = new XYZ();
                XYZ min2 = tmpStPoint;
                for (int i = 0; i < lstPair.Count(); i++)
                {
                    KeyValuePair<ElementId, List<XYZ>> p = lstPair[i];

                    if (i == 0) //始点の場合、上に部材が載っているか判定
                    {
                        if (CheckOntheElement(doc, min2, p.Key))
                        {
                            min2 = p.Value[1];
                            continue;
                        }
                    }

                    max2 = p.Value[0];
                    if (min2.DistanceTo(max2) < 0)
                    {
                        continue;
                    }

                    if (!ClsGeo.GEO_EQ(min2, max2))//同一の点上になっていないか判定
                    {
                        res.Add(new List<XYZ>() { min2, max2 });
                    }

                    min2 = p.Value[1];
                }

                //終点上に部材が載っているか判定
                if (!CheckOntheElement(doc, tmpEdPoint, lstPair.Last().Key))
                {
                    res.Add(new List<XYZ>() { min2, tmpEdPoint });
                }
                return res;
            }
            catch
            {
                return res;
            }
        }
        /// <summary>
        /// ダブル用オブジェクトが乗っていない箇所の線分座標群を取得
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="tmpStPoint">始点(Z軸を上下段でずらす)</param>
        /// <param name="tmpEdPoint">終点(Z軸を上下段でずらす)</param>
        /// <param name="filterList">ファミリ名を指定するとそのファミリのみオブジェクトが乗っている</param>
        /// <returns></returns>
        public static List<List<XYZ>> GetNonObjectBaseLinePoints(Document doc, XYZ tmpStPoint, XYZ tmpEdPoint, List<string> filterList)
        {
            List<List<XYZ>> res = new List<List<XYZ>>();
            try
            {
                View3D view3D = Get3DView(doc);

                XYZ center = tmpStPoint;
                XYZ rayDirection = tmpEdPoint - tmpStPoint;

                //線分方向の衝突エレメントを取得
                ReferenceIntersector refIntersector2 = new ReferenceIntersector(view3D);
                IList<ReferenceWithContext> referenceWithContext2 = refIntersector2.Find(center, rayDirection);
                List<Reference> refs = new List<Reference>();
                foreach (var x in referenceWithContext2)
                {
                    Reference r = x.GetReference();
                    ImportInstance importInst = doc.GetElement(r) as ImportInstance;
                    if (importInst != null)
                    {
                        //#32407
                        continue;
                    }
                    refs.Add(r);
                }

                //線分上のエレメントIDリストの取得
                List<ElementId> lstElementId = new List<ElementId>();
                lstElementId = (from data in refs select data.ElementId).Distinct().ToList();
                if (lstElementId == null || lstElementId.Count == 0)
                {
                    List<XYZ> lstXYZ = new List<XYZ>() { tmpStPoint, tmpEdPoint };
                    res.Add(lstXYZ);
                    return res;
                }

                //エレメントID毎に近い座標と遠い座標を取得
                List<KeyValuePair<ElementId, List<XYZ>>> lstPair = new List<KeyValuePair<ElementId, List<XYZ>>>();
                foreach (ElementId el in lstElementId)
                {
                    FamilyInstance inst = doc.GetElement(el) as FamilyInstance;
                    if (inst != null)//ファミリ名でフィルター
                    {
                        bool match = false;
                        string name = inst.Symbol.FamilyName;
                        foreach (string filterName in filterList)
                        {
                            if (name == filterName)
                            {
                                match = true;
                                break;
                            }
                        }
                        if (!match)
                            continue;

                    }
                    List<XYZ> lstXYZ = (from data in refs where data.ElementId == el select data.GlobalPoint).ToList();

                    XYZ max = lstXYZ.OrderByDescending(item => item.DistanceTo(tmpStPoint)).First();
                    XYZ min = lstXYZ.OrderByDescending(item => item.DistanceTo(tmpStPoint)).Last();

                    if (max.DistanceTo(min) < ClsRevitUtil.CovertToAPI(99) ||
                        tmpStPoint.DistanceTo(min) > tmpStPoint.DistanceTo(tmpEdPoint))
                    {
                        continue;
                    }

                    List<XYZ> pairXYZ = new List<XYZ>() { min, max };
                    KeyValuePair<ElementId, List<XYZ>> pair = new KeyValuePair<ElementId, List<XYZ>>(el, pairXYZ);
                    lstPair.Add(pair);
                }
                lstPair = lstPair.OrderBy(item => item.Value[0].DistanceTo(tmpStPoint)).ToList();

                //線分しか関連していない場合
                if (lstPair.Count == 0)
                {
                    List<XYZ> lstXYZ = new List<XYZ>() { tmpStPoint, tmpEdPoint };
                    res.Add(lstXYZ);
                    return res;
                }

                XYZ max2 = new XYZ();
                XYZ min2 = tmpStPoint;
                for (int i = 0; i < lstPair.Count(); i++)
                {
                    KeyValuePair<ElementId, List<XYZ>> p = lstPair[i];

                    if (i == 0) //始点の場合、上に部材が載っているか判定
                    {
                        if (CheckOntheElement(doc, min2, p.Key))
                        {
                            min2 = p.Value[1];
                            continue;
                        }
                    }

                    max2 = p.Value[0];
                    if (min2.DistanceTo(max2) < 0)
                    {
                        continue;
                    }

                    if (!ClsGeo.GEO_EQ(min2, max2))//同一の点上になっていないか判定
                    {
                        res.Add(new List<XYZ>() { min2, max2 });
                    }

                    min2 = p.Value[1];
                }

                //終点上に部材が載っているか判定
                if (!CheckOntheElement(doc, tmpEdPoint, lstPair.Last().Key))
                {
                    res.Add(new List<XYZ>() { min2, tmpEdPoint });
                }
                return res;
            }
            catch
            {
                return res;
            }
        }
        /// <summary>
        /// 線分座標上に乗っている指定のオブジェクトを削除する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="tmpStPoint">始点(Z軸を上下段でずらす)</param>
        /// <param name="tmpEdPoint">終点(Z軸を上下段でずらす)</param>
        /// <param name="filterList">削除するオブジェクトリスト</param>
        /// <returns></returns>
        public static List<ElementId> DeleteObjectBaseLinePoints(Document doc, XYZ tmpStPoint, XYZ tmpEdPoint, List<string> filterList)
        {
            List<List<XYZ>> res = new List<List<XYZ>>();
            List<ElementId> ids = new List<ElementId>();
            try
            {
                View3D view3D = Get3DView(doc);

                XYZ center = tmpStPoint;
                XYZ rayDirection = tmpEdPoint - tmpStPoint;

                //線分方向の衝突エレメントを取得
                ReferenceIntersector refIntersector2 = new ReferenceIntersector(view3D);
                IList<ReferenceWithContext> referenceWithContext2 = refIntersector2.Find(center, rayDirection);
                List<Reference> refs = new List<Reference>();
                foreach (var x in referenceWithContext2)
                {
                    Reference r = x.GetReference();
                    refs.Add(r);
                }

                //線分上のエレメントIDリストの取得
                List<ElementId> lstElementId = new List<ElementId>();
                lstElementId = (from data in refs select data.ElementId).Distinct().ToList();
                if (lstElementId == null || lstElementId.Count == 0)
                {
                    return ids;
                }


                //エレメントID毎に近い座標と遠い座標を取得
                List<KeyValuePair<ElementId, List<XYZ>>> lstPair = new List<KeyValuePair<ElementId, List<XYZ>>>();
                foreach (ElementId el in lstElementId)
                {
                    FamilyInstance inst = doc.GetElement(el) as FamilyInstance;
                    if (inst != null)//ファミリ名でフィルター
                    {
                        bool match = false;
                        string name = inst.Symbol.FamilyName;
                        foreach (string filterName in filterList)
                        {
                            if (name == filterName)
                            {
                                match = true;
                                break;
                            }
                        }
                        if (!match)
                            continue;
                        ids.Add(el);
                    }

                }


                return ids;
            }
            catch
            {
                return ids;
            }
        }
        /// <summary>
        /// ダブル用始点から端部部品が乗っていない箇所の座標始点を取得
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="tmpStPoint">始点(Z軸を上下段でずらす)</param>
        /// <param name="tmpEdPoint">終点(Z軸を上下段でずらす)</param>
        /// <returns></returns>
        public static XYZ GetNonObjectBaseLinePoint(Document doc, XYZ tmpStPoint, XYZ tmpEdPoint)
        {
            XYZ res = new XYZ();
            try
            {
                View3D view3D = Get3DView(doc);

                XYZ center = tmpStPoint;
                XYZ rayDirection = tmpEdPoint - tmpStPoint;

                //線分方向の衝突エレメントを取得
                ReferenceIntersector refIntersector2 = new ReferenceIntersector(view3D);
                IList<ReferenceWithContext> referenceWithContext2 = refIntersector2.Find(center, rayDirection);
                List<Reference> refs = new List<Reference>();
                foreach (var x in referenceWithContext2)
                {
                    Reference r = x.GetReference();
                    ImportInstance importInst = doc.GetElement(r) as ImportInstance;
                    if (importInst != null)
                    {
                        //#32407
                        continue;
                    }
                    refs.Add(r);
                }

                //線分上のエレメントIDリストの取得
                List<ElementId> lstElementId = new List<ElementId>();
                lstElementId = (from data in refs select data.ElementId).Distinct().ToList();
                if (lstElementId == null || lstElementId.Count == 0)
                {
                    //List<XYZ> lstXYZ = new List<XYZ>() { tmpStPoint, tmpEdPoint };
                    res = tmpStPoint;// lstXYZ.ToList();
                    return res;
                }

                //エレメントID毎に近い座標と遠い座標を取得
                List<KeyValuePair<ElementId, List<XYZ>>> lstPair = new List<KeyValuePair<ElementId, List<XYZ>>>();
                foreach (ElementId el in lstElementId)
                {
                    List<XYZ> lstXYZ = (from data in refs where data.ElementId == el select data.GlobalPoint).ToList();

                    XYZ max = lstXYZ.OrderByDescending(item => item.DistanceTo(tmpStPoint)).First();
                    XYZ min = lstXYZ.OrderByDescending(item => item.DistanceTo(tmpStPoint)).Last();

                    if (max.DistanceTo(min) < ClsRevitUtil.CovertToAPI(15) ||
                        tmpStPoint.DistanceTo(min) > tmpStPoint.DistanceTo(tmpEdPoint))
                    {
                        continue;
                    }

                    List<XYZ> pairXYZ = new List<XYZ>() { min, max };
                    KeyValuePair<ElementId, List<XYZ>> pair = new KeyValuePair<ElementId, List<XYZ>>(el, pairXYZ);
                    lstPair.Add(pair);
                }
                lstPair = lstPair.OrderBy(item => item.Value[0].DistanceTo(tmpStPoint)).ToList();

                //線分しか関連していない場合
                if (lstPair.Count == 0)
                {
                    //List<XYZ> lstXYZ = new List<XYZ>() { tmpStPoint, tmpEdPoint };
                    res = tmpStPoint;// lstXYZ.ToList();
                    return res;
                }

                XYZ max2 = new XYZ();
                XYZ min2 = tmpStPoint;
                //for (int i = 0; i < lstPair.Count(); i++)
                //{
                KeyValuePair<ElementId, List<XYZ>> p = lstPair[0];

                //if (i == 0) //始点の場合、上に部材が載っているか判定
                //{
                if (CheckOntheElement(doc, min2, p.Key))
                {
                    min2 = p.Value[1];
                    //continue;
                }
                res = min2;
                //res.Add(min2);
                //res.Add(tmpEdPoint);
                //}

                //max2 = p.Value[0];
                //if (min2.DistanceTo(max2) <= 0)
                //{
                //    continue;
                //}

                //res.Add(new List<XYZ>() { min2, max2 });

                //min2 = p.Value[1];
                //}

                //終点上に部材が載っているか判定
                //if (!CheckOntheElement(doc, tmpEdPoint, lstPair.Last().Key))
                //{
                //    //res.Add(new List<XYZ>() { min2, tmpEdPoint });
                //    res.Add(tmpEdPoint);
                //}
                return res;
            }
            catch
            {
                return res;
            }
        }

        public static List<List<XYZ>> GetNonObjectBaseLinePoints2D(Document doc, XYZ tmpStPoint, XYZ tmpEdPoint, bool Nop = true)
        {
            List<List<XYZ>> res = new List<List<XYZ>>();
            try
            {
                View activeView = doc.ActiveView;
                //BoundingBoxXYZ viewBoundingBox = activeView.CropBox;

                Outline baseOutLine = new Outline(tmpStPoint, tmpEdPoint);

                FilteredElementCollector collector = new FilteredElementCollector(doc, activeView.Id);
                IList<Element> elementsInView = collector.ToElements();

                //線分上のエレメントIDリストの取得
                List<ElementId> lstElementId = new List<ElementId>();
                List<BoundingBoxXYZ> boundingBoxList = new List<BoundingBoxXYZ>();
                List<KeyValuePair<ElementId, BoundingBoxXYZ>> idBoundingBoxPairList = new List<KeyValuePair<ElementId, BoundingBoxXYZ>>();

                bool insecCount = true;
                while (insecCount)
                {
                    foreach (Element element in elementsInView)
                    {
                        //逆方向になると途端に取得できなくなる
                        BoundingBoxXYZ boundingBox = element.get_BoundingBox(activeView);
                        if (boundingBox != null)
                        {
                            Outline outLine = new Outline(boundingBox.Min, boundingBox.Max);
                            if (outLine.Intersects(baseOutLine, 0.001))
                            {
                                if (element is ImportInstance)
                                {
                                    //#32407
                                    continue;
                                }

                                if (element is FamilyInstance)
                                {
                                    if (Nop)
                                    {
                                        //#31697
                                        FamilyInstance inst = element as FamilyInstance;
                                        if (inst != null)
                                        {
                                            if (inst.Name.Contains("杭"))
                                            {
                                                continue;
                                            }
                                            if (inst.Name.Contains("鋼矢板"))
                                            {
                                                continue;
                                            }
                                            if (inst.Symbol.FamilyName.Contains("ﾌﾞﾙﾏﾝ") || inst.Symbol.FamilyName.Contains("ﾘｷﾏﾝ"))
                                            {
                                                continue;
                                            }
                                            if (inst.Symbol.FamilyName.Contains("ﾌﾞﾗｹｯﾄ"))
                                            {
                                                continue;
                                            }
                                            if (inst.Symbol.FamilyName.Contains("ｽﾁﾌﾅｰ"))
                                            {
                                                continue;
                                            }
                                            if (inst.Symbol.FamilyName.Contains("カバープレート"))
                                            {
                                                continue;
                                            }
                                            if (inst.Symbol.FamilyName.Contains("隅部ピース"))
                                            {
                                                continue;
                                            }
                                        }

                                    }
                                    lstElementId.Add(element.Id);
                                    boundingBoxList.Add(boundingBox);//referenceからGlobalPointが取れない
                                    KeyValuePair<ElementId, BoundingBoxXYZ> idBoundingBoxPair = new KeyValuePair<ElementId, BoundingBoxXYZ>(element.Id, boundingBox);
                                    idBoundingBoxPairList.Add(idBoundingBoxPair);
                                    insecCount = false;
                                }
                            }
                        }
                    }
                    baseOutLine = new Outline(tmpEdPoint, tmpStPoint);
                }

                if (lstElementId == null || lstElementId.Count == 0)
                {
                    List<XYZ> lstXYZ = new List<XYZ>() { tmpStPoint, tmpEdPoint };
                    res.Add(lstXYZ);
                    return res;
                }

                //エレメントID毎に近い座標と遠い座標を取得
                List<KeyValuePair<ElementId, List<XYZ>>> lstPair = new List<KeyValuePair<ElementId, List<XYZ>>>();

                foreach (KeyValuePair<ElementId, BoundingBoxXYZ> idBoundingBoxPair in idBoundingBoxPairList)
                {
                    ElementId el = idBoundingBoxPair.Key;
                    BoundingBoxXYZ boundingBox = idBoundingBoxPair.Value;
                    FamilyInstance inst = doc.GetElement(el) as FamilyInstance;
                    if (inst != null)//ファミリ名でフィルター
                    {
                        string name = inst.Symbol.FamilyName;
                    }
                    //List<XYZ> lstXYZ = (from data in boundingBoxList where data.ElementId == el select data.GlobalPoint).ToList();
                    //BoundingBoxXYZ a = (from data in idBoundingBoxPairList where data.Key == el select data.Value);

                    //BoundingBoxから取得のパターン
                    //三軸を間に挟んだほうがよいかも
                    //ファミリインスタンスから直接計算した方がよいかも
                    //XYZ max = new XYZ(Math.Max(tmpStPoint.X, boundingBox.Min.X),
                    //                  Math.Max(tmpStPoint.Y, boundingBox.Min.Y),
                    //                  Math.Max(tmpStPoint.Z, boundingBox.Min.Z)); //boundingBox.Max;// lstXYZ.OrderByDescending(item => item.DistanceTo(tmpStPoint)).First();
                    //XYZ min = new XYZ(Math.Min(tmpEdPoint.X, boundingBox.Max.X),
                    //                  Math.Min(tmpEdPoint.Y, boundingBox.Max.Y),
                    //                  Math.Min(tmpEdPoint.Z, boundingBox.Max.Z));//boundingBox.Min;// lstXYZ.OrderByDescending(item => item.DistanceTo(tmpStPoint)).Last();

                    //if (tmpStPoint.DistanceTo(boundingBox.Max) < tmpStPoint.DistanceTo(boundingBox.Min))
                    //{
                    //    max = new XYZ(Math.Max(tmpEdPoint.X, boundingBox.Max.X),
                    //                  Math.Max(tmpEdPoint.Y, boundingBox.Max.Y),
                    //                  Math.Max(tmpEdPoint.Z, boundingBox.Max.Z)); 
                    //    min = new XYZ(Math.Min(tmpStPoint.X, boundingBox.Min.X),
                    //                  Math.Min(tmpStPoint.Y, boundingBox.Min.Y),
                    //                  Math.Min(tmpStPoint.Z, boundingBox.Min.Z));
                    //}
                    //BoundingBoxから取得のパターン

                    //Referenceから取得のパターン
                    //List<Reference> refs = GetReferencesFromFamilyInstance(doc, inst).ToList();

                    //
                    List<XYZ> checkList = GetReferencesPointsFromFamilyInstance(inst);
                    List<XYZ> lstXYZ = GetCurveOnPointList(tmpStPoint, tmpEdPoint, checkList);//(from data in refs where data.ElementId == el select data.GlobalPoint).ToList();

                    if (lstXYZ.Count < 2)
                    {
                        continue;
                    }

                    XYZ max = lstXYZ.OrderByDescending(item => item.DistanceTo(tmpStPoint)).First();
                    XYZ min = lstXYZ.OrderByDescending(item => item.DistanceTo(tmpStPoint)).Last();//lstXYZ.OrderByDescending(item => item.DistanceTo(tmpEdPoint)).First(); //

                    if (max.DistanceTo(min) < ClsRevitUtil.CovertToAPI(15) || //99) ||//#31638対応で変えてみたが問題あるかは今後検証
                        tmpStPoint.DistanceTo(min) > tmpStPoint.DistanceTo(tmpEdPoint))
                    {
                        continue;
                    }
                    //Referenceから取得のパターン

                    List<XYZ> pairXYZ = new List<XYZ>() { min, max };//{ max, min };// min, max };
                    KeyValuePair<ElementId, List<XYZ>> pair = new KeyValuePair<ElementId, List<XYZ>>(el, pairXYZ);
                    lstPair.Add(pair);
                }

                lstPair = lstPair.OrderBy(item => item.Value[0].DistanceTo(tmpStPoint)).ToList();

                //線分しか関連していない場合
                if (lstPair.Count == 0)
                {
                    List<XYZ> lstXYZ = new List<XYZ>() { tmpStPoint, tmpEdPoint };
                    res.Add(lstXYZ);
                    return res;
                }

                XYZ max2 = new XYZ();
                XYZ min2 = tmpStPoint;
                for (int i = 0; i < lstPair.Count(); i++)
                {
                    KeyValuePair<ElementId, List<XYZ>> p = lstPair[i];

                    if (i == 0) //始点の場合、上に部材が載っているか判定
                    {
                        if (CheckOntheElement(doc, min2, p.Key))
                        {
                            min2 = p.Value[1];
                            continue;
                        }
                    }

                    max2 = p.Value[0];
                    if (min2.DistanceTo(max2) < 0)
                    {
                        continue;
                    }

                    if (!ClsGeo.GEO_EQ(min2, max2))//同一の点上になっていないか判定
                    {
                        res.Add(new List<XYZ>() { min2, max2 });
                    }

                    min2 = p.Value[1];
                }

                //終点上に部材が載っているか判定
                if (!CheckOntheElement(doc, tmpEdPoint, lstPair.Last().Key))
                {
                    res.Add(new List<XYZ>() { min2, tmpEdPoint });
                }
                return res;
            }
            catch
            {
                return res;
            }
        }
        public static List<Reference> GetReferencesFromFamilyInstance(Document doc, FamilyInstance familyInstance)
        {
            List<Reference> references = new List<Reference>();

            // FamilyInstanceのジオメトリを取得
            Options geomOptions = new Options();
            geomOptions.ComputeReferences = true;
            geomOptions.DetailLevel = ViewDetailLevel.Fine;
            GeometryElement geomElement = familyInstance.get_Geometry(geomOptions);

            foreach (GeometryObject geomObj in geomElement)
            {
                if (geomObj is GeometryInstance)
                {
                    GeometryInstance geomInstance = geomObj as GeometryInstance;
                    GeometryElement instanceGeometry = geomInstance.GetInstanceGeometry();
                    foreach (GeometryObject instanceObj in instanceGeometry)
                    {
                        if (instanceObj is Solid)
                        {
                            Solid solid = instanceObj as Solid;
                            foreach (Face face in solid.Faces)
                            {
                                Reference reference = face.Reference;
                                if (reference != null && reference.ElementId == familyInstance.Id)
                                    references.Add(reference);

                                if (reference.GlobalPoint != null)
                                {
                                    string a = "test";
                                }
                            }
                        }
                    }
                }
            }

            return references;
        }
        public static List<XYZ> GetReferencesPointsFromFamilyInstance(FamilyInstance familyInstance)
        {
            List<XYZ> referencesPoint = new List<XYZ>();

            // FamilyInstanceのジオメトリを取得
            Options geomOptions = new Options();
            //geomOptions.ComputeReferences = true;
            //geomOptions.DetailLevel = ViewDetailLevel.Fine;
            GeometryElement geomElement = familyInstance.get_Geometry(geomOptions);

            foreach (GeometryObject geomObj in geomElement)
            {
                if (geomObj is GeometryInstance)
                {
                    GeometryInstance geomInstance = geomObj as GeometryInstance;
                    GeometryElement instanceGeometry = geomInstance.GetInstanceGeometry();
                    foreach (GeometryObject instanceObj in instanceGeometry)
                    {
                        if (instanceObj is Solid)
                        {
                            Solid solid = instanceObj as Solid;
                            foreach (Face face in solid.Faces)
                            {
                                if (face is PlanarFace)
                                {
                                    PlanarFace planarFace = face as PlanarFace;
                                    if (planarFace != null)
                                    {
                                        referencesPoint.Add(planarFace.Origin);//.Evaluate(new UV(0.5, 0.5)));planarFace.Originとnew UV(0, 0)は取得位置が一緒
                                        //referencesPoint.Add(planarFace.Evaluate(new UV(-0.656167979, 0.656167979)));//new UV(0.5, 0.5)だと幅がかなり広がってしまう//UV(X, Y)X,Yは-も許容していて面によって違くこれらを正確に取れればもしかしたら
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return referencesPoint;
        }
        public static List<XYZ> GetCurveOnPointList(XYZ tmpStPoint, XYZ tmpEdPoint, List<XYZ> checkList)
        {
            List<XYZ> onPointList = new List<XYZ>();
            //Z軸の判定は後ほど行うため0に合わせる
            Curve cv = Line.CreateBound(tmpStPoint, tmpEdPoint);//new XYZ(tmpStPoint.X, tmpStPoint.Y, 0), new XYZ(tmpEdPoint.X, tmpEdPoint.Y, 0));

            XYZ max = checkList.OrderByDescending(item => item.Z).First();
            XYZ min = checkList.OrderByDescending(item => item.Z).Last();

            //点群のZ軸内に指定点は収まっているか
            if (min.Z < tmpStPoint.Z && tmpStPoint.Z < max.Z)
            {
                foreach (XYZ check in checkList)
                {
                    XYZ checkZ0 = new XYZ(check.X, check.Y, tmpStPoint.Z);
                    foreach (XYZ check2 in checkList)
                    {
                        XYZ check2Z0 = new XYZ(check2.X, check2.Y, tmpStPoint.Z);
                        if (!ClsGeo.GEO_EQ(check, check2))
                        {
                            Curve checkLine = Line.CreateBound(checkZ0, check2Z0);
                            XYZ point = ClsRevitUtil.GetIntersection(cv, checkLine);
                            if (point != null)
                            {
                                onPointList.Add(point);
                            }
                        }
                    }
                }
            }
            return onPointList;
        }
        /// <summary>
        /// 鋼材名にある主材サイズを取得する
        /// </summary>
        /// <param name="kouzaiName">鋼材名(仮鋼材_主材_20HA)など</param>
        /// <returns></returns>
        public static string GetSyuzaiSize(string kouzaiName)
        {
            string syuzaiSize = string.Empty;
            string[] kouzaiNameUnder = kouzaiName.Split('_');
            foreach (string syuzai in kouzaiNameUnder)
            {
                if (syuzai.Contains("HA") || syuzai.Contains("SMH"))
                {
                    syuzaiSize = syuzai;
                    break;
                }
            }
            return syuzaiSize;
        }

        /// <summary>
        /// 鋼材に設定されているサイズを取得する
        /// </summary>
        /// <param name="sym">鋼材シンボル</param>
        /// <returns></returns>
        public static string GetSyuzaiSize(FamilySymbol sym)
        {
            return ClsRevitUtil.GetTypeParameterString(sym, "記号");
        }
        /// <summary>
        /// ポイントの上にエレメントが存在するか確認
        /// </summary>
        /// <param name="doc">ドキュメント</param>
        /// <param name="p">座標</param>
        /// <param name="id">確認するエレメントID</param>
        /// <returns></returns>
        public static bool CheckOntheElement(Document doc, XYZ p, ElementId id)
        {
            XYZ center = p;
            XYZ rayDirection = new XYZ(0, 0, 1);

            View3D view3D = Get3DView(doc);

            ReferenceIntersector refIntersector = new ReferenceIntersector(view3D);
            IList<ReferenceWithContext> referenceWithContext = refIntersector.Find(center, rayDirection);
            List<Reference> refs = new List<Reference>();
            foreach (var x in referenceWithContext)
            {
                Reference r = x.GetReference();

                if (r.ElementId == id)
                {
                    return true;
                }
            }

            return false;
        }
        /// <summary>
        /// 3Dビューを取得
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static View3D Get3DView(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(View3D));
            foreach (View3D v in collector)
            {
                // skip view templates here because they are invisible in project browsers:
                if (v != null && !v.IsTemplate && v.Name == ClsGlobal.m_3DView)
                {
                    return v;
                }
            }
            return null;
        }
        /// <summary>
        /// 図面上のビューを切り替える
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="name">ビューの名前</param>
        /// <param name="viewType">ビューのタイプ(3D,平面図,立面図など)</param>
        /// <returns></returns>
        public static bool ChangeView(UIDocument uidoc, string name, ViewType viewType = ViewType.Elevation)
        {
            Document doc = uidoc.Document;
            var collector = new FilteredElementCollector(doc).OfClass(typeof(View)).Cast<View>().Where(x => x.ViewType == viewType);
            foreach (var view in collector)
            {
                if (view != null && view.Name == name)
                {
                    uidoc.ActiveView = view;
                    //return true;
                }
            }
            return false;
        }

        public static void CreateElevationView(Document doc, XYZ point, XYZ dir, string viewName = "TEST")
        {
            //既存の場合は作成しない
            if (new FilteredElementCollector(doc)
                .OfClass(typeof(ViewSection))
                .Cast<ViewSection>()
                .FirstOrDefault(x => x.Name == viewName)
                != null)
                return;

            //図面上の平面図を取得※どれでもよい
            var viewPlan = new FilteredElementCollector(doc)
                .OfClass(typeof(View))
                .Cast<View>()
                .FirstOrDefault(x => x.ViewType == ViewType.FloorPlan);
            //立面図の設定？を図面から取得
            var viewFamilyType = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewFamilyType))
                .Cast<ViewFamilyType>()
                .FirstOrDefault(x => x.ViewFamily == ViewFamily.Elevation);
            //図面上の立面図を取得※どれでもよい
            var viewSection = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewSection))
                .Cast<ViewSection>()
                .FirstOrDefault(x => x.ViewType == ViewType.Elevation);

            // 新しい立面図ビューの作成
            using (Transaction t = new Transaction(doc, "Create New Elevation View"))
            {
                t.Start();
                //ElevationMarkerは平面図で見るところの目ん玉みたいなやつ
                ElevationMarker marker = ElevationMarker.CreateElevationMarker(doc, viewFamilyType.Id, point, 100);
                //viewPlanは平面図でないといけない
                //ViewSectionはmakerの先がどれくらいなのかなど
                ViewSection newElevationView = marker.CreateElevation(doc, viewPlan.Id, 0);//作った立面図の初期データを図面に既存である立面図に合わせる処理が必要
                newElevationView.Name = viewName;
                //図面上にある立面図と同じ表示範囲に設定する
                newElevationView.CropBox = viewSection.CropBox;
                string parm = "前方クリップ オフセット";
                ClsRevitUtil.SetParameter(doc, newElevationView.Id, parm, ClsRevitUtil.GetParameterDouble(doc, viewSection.Id, parm));

                //ElevationMarkerを回転すればおのずとVieweSectionも回転する
                double angle = dir.AngleOnPlaneTo(XYZ.BasisY, XYZ.BasisZ);
                Line axis = Line.CreateBound(point, point + XYZ.BasisZ);
                ClsRevitUtil.RotateElement(doc, marker.Id, axis, -angle);
                t.Commit();
            }
        }

        public static void CreateBaseElevationView(Document doc, ElementId id, string viewName = "TEST")
        {
            FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
            if (!inst.Symbol.Name.Contains("ベース"))
            {
                return;
            }

            XYZ point = new XYZ();
            XYZ dir = new XYZ();
            if (inst.Location is LocationCurve)
            {
                LocationCurve lCurve = inst.Location as LocationCurve;

                XYZ tmpStPoint = lCurve.Curve.GetEndPoint(0);
                XYZ tmpEdPoint = lCurve.Curve.GetEndPoint(1);
                point = (tmpStPoint + tmpEdPoint) * 0.5;
                dir = Line.CreateBound(tmpStPoint, tmpEdPoint).Direction;
            }
            else if (inst.Location is LocationPoint)
            {
                var lLocation = inst.Location as LocationPoint;
                point = lLocation.Point;
                dir = inst.HandOrientation;
            }
            else
                return;

            ClsYMSUtil.CreateElevationView(doc, point, dir, viewName);
        }

        #endregion

        /// <summary>
        ///
        /// </summary>
        /// <param name="dTotalLength"></param>
        /// <param name="dHaraAngle"></param>
        /// <param name="dOffsetA">始点端部部品から鋼材始点までの長さ</param>
        /// <param name="dOffsetB">始点端部部品から終点端部部品までの長さ（鋼材長さ）</param>
        /// <param name="dOffsetC">鋼材終点から終点端部部品まで</param>
        /// <returns></returns>
        public static double GetTotalLength(double dTotalLength, string steelSize, string tanbuParts1, string tanbuParts2, string tanbuSize1, string tanbusize2,
            double angle, double dHaraAngle, ref double dOffsetA, ref double dOffsetB, ref double dOffsetC)
        {
            double d = 0.0;
            double d2 = 0.0;

            double dAngleA = 0;
            double dAngleB = 0;
            double dKouzaiWidthA = 0;
            double dKouzaiWidthB = 0;
            double dPartsThicknessA = 0;
            double dPartsThicknessB = 0;

            dAngleA = angle;
            dAngleB = 180 - dHaraAngle - angle;


            //鋼材幅
            if (tanbuParts1 != Master.ClsHiuchiCsv.TypeNameFVP)
            {
                string targetShuzai = Master.ClsHiuchiCsv.GetTargetShuzai(tanbuSize1);
                dKouzaiWidthA = Master.ClsYamadomeCsv.GetWidth(targetShuzai);
            }
            else
            {
                dKouzaiWidthA = Master.ClsYamadomeCsv.GetWidth(steelSize);
            }

            if (tanbuParts2 != Master.ClsHiuchiCsv.TypeNameFVP)
            {
                string targetShuzai = Master.ClsHiuchiCsv.GetTargetShuzai(tanbusize2);
                dKouzaiWidthB = Master.ClsYamadomeCsv.GetWidth(targetShuzai);
            }
            else
            {
                dKouzaiWidthB = Master.ClsYamadomeCsv.GetWidth(steelSize);
            }

            //端部部品厚
            dPartsThicknessA = Master.ClsHiuchiCsv.GetThickness(tanbuSize1);
            dPartsThicknessB = Master.ClsHiuchiCsv.GetThickness(tanbusize2);

            double dA = CalculateOffsets(dAngleA, dKouzaiWidthA, dPartsThicknessA);
            double dC = CalculateOffsets(dAngleB, dKouzaiWidthB, dPartsThicknessB);

            dOffsetA = dA;
            dOffsetB = dTotalLength - dA - dC;
            dOffsetC = dC;

            return ClsGeo.FloorAtDigitAdjust(0, dTotalLength + dA + dC); ;
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="dTotalLength"></param>
        /// <param name="dHaraAngle"></param>
        /// <param name="dOffsetA">始点端部部品から鋼材始点までの長さ</param>
        /// <param name="dOffsetB">始点端部部品から終点端部部品までの長さ（鋼材長さ）</param>
        /// <param name="dOffsetC">鋼材終点から終点端部部品まで</param>
        /// <returns></returns>
        public static double GetTotalLength(double dTotalLength, ClsTanbuParts clsTanbuParts,
            double angle, out double dOffsetA, out double dOffsetB)
        {
            double dAngle = angle;
            double dKouzaiWidth = 0;
            double dPartsThickness = 0;

            //鋼材幅
            if (clsTanbuParts.TanbuParts != Master.ClsHiuchiCsv.TypeNameFVP)
            {
                string targetShuzai = Master.ClsHiuchiCsv.GetTargetShuzai(clsTanbuParts.TanbuSize);
                dKouzaiWidth = Master.ClsYamadomeCsv.GetWidth(targetShuzai);
            }
            else
            {
                dKouzaiWidth = Master.ClsYamadomeCsv.GetWidth(clsTanbuParts.SteelSize);
            }

            //端部部品厚
            dPartsThickness = Master.ClsHiuchiCsv.GetThickness(clsTanbuParts.TanbuSize);

            double dA = CalculateOffsets(dAngle, dKouzaiWidth, dPartsThickness);

            dOffsetA = dA;
            dOffsetB = dTotalLength - dA;

            return ClsGeo.FloorAtDigitAdjust(0, dTotalLength + dA); ;
        }
        public static double CalculateOffsets(double dAngle, double dKouzaiWidth, double dPartsThickness)
        {
            double dAngleA = dAngle;
            double dAngleB = 180 - 90 - dAngle;

            double offsetStart = (dKouzaiWidth / 2) * Math.Tan(ClsGeo.Deg2Rad(dAngleB));

            if (dPartsThickness != 0.0)
            {
                offsetStart += dPartsThickness / Math.Sin(ClsGeo.Deg2Rad(dAngleA));
            }

            return offsetStart;
        }
        public static double CalculateOffsets(double dAngleA, double dAngleB, double dKouzaiWidth, double dPartsThickness)
        {
            double offsetStart = (dKouzaiWidth / 2) * Math.Tan(ClsGeo.Deg2Rad(dAngleB));

            if (dPartsThickness != 0.0)
            {
                offsetStart += dPartsThickness / Math.Sin(ClsGeo.Deg2Rad(dAngleA));
            }

            return offsetStart;
        }
        #region Levelメソッド
        /// <summary>
        /// 図面に存在するレベル名称を全て取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<string> GetLevelNames(Document doc)
        {
            List<string> levelNames = new List<string>();

            // レベルのコレクションを取得
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ICollection<Element> levels = collector.OfClass(typeof(Level)).ToElements();

            // レベル名称を取得
            foreach (Element level in levels)
            {
                Parameter nameParam = level.get_Parameter(BuiltInParameter.DATUM_TEXT);
                if (nameParam != null)
                {
                    string levelName = nameParam.AsString();
                    levelNames.Add(levelName);
                }
            }

            // レベル名称をソート
            levelNames.Sort((x, y) => CompareLevelNames(doc, x, y));

            return levelNames;
        }
        //private static int CompareLevelNames(string x, string y)
        //{
        //    if (x.StartsWith(ClsKabeShin.GL) && y.StartsWith(ClsKabeShin.GL))
        //    {
        //        int xNumber = GetLevelNumber(x);
        //        int yNumber = GetLevelNumber(y);
        //        return xNumber.CompareTo(yNumber);
        //    }
        //    else if (x.StartsWith(ClsKabeShin.GL))
        //    {
        //        return -1;
        //    }
        //    else if (y.StartsWith(ClsKabeShin.GL))
        //    {
        //        return 1;
        //    }
        //    else
        //    {
        //        int xPosition = x.LastIndexOf("段目");
        //        int yPosition = y.LastIndexOf("段目");
        //        string xNumberString = x.Substring(0, xPosition);
        //        string yNumberString = y.Substring(0, yPosition);
        //        int xNumber = GetLevelNumber(xNumberString);
        //        int yNumber = GetLevelNumber(yNumberString);
        //        return xNumber.CompareTo(yNumber);
        //    }
        //}
        private static int GetLevelNumber(string levelName)
        {
            int number;
            if (int.TryParse(levelName, out number))
            {
                return number;
            }
            return 0;
        }
        private static int CompareLevelNames(Document doc, string x, string y)
        {
            double xNumber = GetLevelHight(doc, x);
            double yNumber = GetLevelHight(doc, y);

            return yNumber.CompareTo(xNumber);
        }
        private static double GetLevelHight(Document doc, string levelName)
        {
            ElementId levelID = ClsRevitUtil.GetLevelID(doc, levelName);
            double hight = ClsRevitUtil.GetParameterDouble(doc, levelID, "高さ");
            return hight;
        }
        /// <summary>
        /// 新しいLevelを作成する※存在する場合は高さを変更する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="newLevelName">レベル名</param>
        /// <param name="elevation">高さ</param>
        /// <returns></returns>
        public static bool CreateNewLevel(Document doc, string newLevelName, double elevation)
        {
            if (doc == null || string.IsNullOrEmpty(newLevelName))
                return false;

            var levalId = ClsRevitUtil.GetLevelID(doc, newLevelName);
            if (levalId == null)
            {
                //存在しない
                Level newLevel = Level.Create(doc, elevation);
                newLevel.Name = newLevelName;
                //newLevel.Elevation = elevation;
            }
            else
            {
                //存在する
                Level l = doc.GetElement(levalId) as Level;
                l.Elevation = elevation;
            }
            return true;
        }
        /// <summary>
        /// レベル作成
        /// </summary>
        /// <param name="doc">ドキュメント</param>
        /// <param name="elevation">高さ</param>
        /// <param name="name">レベル名</param>
        /// <remarks>指定したレベル名が既に存在していた場合、レベルの作成は行わない</remarks>
        /// <returns></returns>
        public ElementId Create(Document doc, double elevation, string name)
        {
            if (doc == null)
            {
                return null;
            }

            ElementId id = null;

            try
            {
                Level newLevel = Level.Create(doc, elevation);
                id = newLevel.Id;

            }
            catch
            {
                return null;
            }
            return id;
        }

        #endregion

        /// <summary>
        /// 壁に補助線を作成
        /// </summary>
        /// <param name="doc">ドキュメント</param>
        /// <param name="pointS">開始位置</param>
        /// <param name="pointE">終了位置</param>
        /// <param name="dH">H形鋼サイズ/2(+:外側, -:内側)</param>
        /// <param name="dEx">補助線の延長</param>
        /// <returns>補助線</returns>
        public static ModelLine CreateKabeHojyoLine(Document doc, XYZ pointS, XYZ pointE, double dH, double dEx)
        {
            XYZ Direction = Line.CreateBound(pointS, pointE).Direction;
            //補助線の作成
            XYZ hojyoS = new XYZ(pointS.X + (dH * Direction.Y) - (dEx * Direction.X),
                                 pointS.Y - (dH * Direction.X) - (dEx * Direction.Y),
                                 pointS.Z);
            XYZ hojyoE = new XYZ(pointE.X + (dH * Direction.Y) + (dEx * Direction.X),
                                 pointE.Y - (dH * Direction.X) + (dEx * Direction.Y),
                                 pointE.Z);
            Line hojyo = Line.CreateBound(hojyoS, hojyoE);
            if (dH > 0)//補助線の内外を判別するためにDirectionを反対にする
            {
                hojyo = Line.CreateBound(hojyoE, hojyoS);
            }
            XYZ mid = 0.5 * (hojyoS + hojyoE);
            //作成した壁に補助線を引く
            Plane plane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, mid);
            SketchPlane sketchPlane = SketchPlane.Create(doc, plane);
            ModelLine modelLine = doc.Create.NewModelCurve(hojyo, sketchPlane) as ModelLine;
            return modelLine;
        }
        /// <summary>
        /// 補助線を作成
        /// </summary>
        /// <param name="doc">ドキュメント</param>
        /// <param name="pointS">始点</param>
        /// <param name="pointE">終点</param>
        /// <param name="dH1">始点ずれ量</param>
        /// <param name="dH2">終点ずれ量</param>
        /// <param name="dEx1">始点延長</param>
        /// <param name="dEx2">終点延長</param>
        /// <param name="binout">true:外側,false:内側</param>
        /// <returns>補助線</returns>
        public static ModelLine CreateHojyoLine(Document doc, XYZ pointS, XYZ pointE, double dH1, double dH2, double dEx1, double dEx2, bool binout = false)
        {
            XYZ Direction = Line.CreateBound(pointS, pointE).Direction;
            //補助線の作成
            XYZ hojyoS = new XYZ(pointS.X + (dH1 * Direction.Y) - (dEx1 * Direction.X),
                                 pointS.Y - (dH1 * Direction.X) - (dEx1 * Direction.Y),
                                 pointS.Z);
            XYZ hojyoE = new XYZ(pointE.X + (dH2 * Direction.Y) + (dEx2 * Direction.X),
                                 pointE.Y - (dH2 * Direction.X) + (dEx2 * Direction.Y),
                                 pointE.Z);
            Line hojyo = Line.CreateBound(hojyoS, hojyoE);
            if (binout)//補助線の内外を判別するためにDirectionを反対にする
            {
                hojyo = Line.CreateBound(hojyoE, hojyoS);
            }
            XYZ mid = 0.5 * (hojyoS + hojyoE);
            //作成した壁に補助線を引く
            Plane plane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, mid);
            SketchPlane sketchPlane = SketchPlane.Create(doc, plane);
            ModelLine modelLine = doc.Create.NewModelCurve(hojyo, sketchPlane) as ModelLine;
            return modelLine;
        }
        public static Line CreateLine(Document doc, XYZ pointS, XYZ pointE, double dH1, double dH2, double dEx1, double dEx2, bool binout = false)
        {
            XYZ Direction = Line.CreateBound(pointS, pointE).Direction;
            //補助線の作成
            XYZ hojyoS = new XYZ(pointS.X + (dH1 * Direction.Y) - (dEx1 * Direction.X),
                                 pointS.Y - (dH1 * Direction.X) - (dEx1 * Direction.Y),
                                 pointS.Z);
            XYZ hojyoE = new XYZ(pointE.X + (dH2 * Direction.Y) + (dEx2 * Direction.X),
                                 pointE.Y - (dH2 * Direction.X) + (dEx2 * Direction.Y),
                                 pointE.Z);
            Line hojyo = Line.CreateBound(hojyoS, hojyoE);
            if (binout)//補助線の内外を判別するためにDirectionを反対にする
            {
                hojyo = Line.CreateBound(hojyoE, hojyoS);
            }
            return hojyo;
        }
        public static (XYZ, XYZ) GetMovePoint(Document doc, ElementId id, List<ElementId> targetFamilies, double dist)
        {
            var inst = doc.GetElement(id) as FamilyInstance;
            var lCurve = inst.Location as LocationCurve;
            var cv = lCurve.Curve;
            var tmpStPoint = cv.GetEndPoint(0);
            var tmpEdPoint = cv.GetEndPoint(1);
            var sDir = new XYZ();
            var eDir = new XYZ();

            // 図面上で接している腹起ベースを取得
            //var targetFamilies = ClsHaraokoshiBase.GetAllHaraokoshiBaseList(doc);
            foreach (var tgId in targetFamilies)
            {
                var tgInst = doc.GetElement(tgId) as FamilyInstance;
                if (inst == tgInst)
                    continue;
                var tgCv = (tgInst.Location as LocationCurve).Curve;
                var insec = ClsRevitUtil.GetIntersection(cv, tgCv);
                if (insec != null)
                {
                    if (ClsRevitUtil.CheckNearGetEndPoint(cv, insec))
                    {
                        sDir = tgInst.FacingOrientation;
                    }
                    else
                    {
                        eDir = tgInst.FacingOrientation;
                    }
                }
            }

            return GetMovePoint(tmpStPoint, tmpEdPoint, sDir, eDir, dist);
        }
        /// <summary>
        /// 2点を指定の向きにそれぞれ指定値分移動させる
        /// </summary>
        /// <param name="tmpStPoint">始点</param>
        /// <param name="tmpEdPoint">終点</param>
        /// <param name="sDir">始点からの向き</param>
        /// <param name="eDir">終点からの向き</param>
        /// <param name="dist">移動量</param>
        /// <returns></returns>
        public static (XYZ, XYZ) GetMovePoint(XYZ tmpStPoint, XYZ tmpEdPoint, XYZ sDir, XYZ eDir, double dist)
        {
            XYZ sPoint = tmpStPoint + dist * sDir;
            XYZ ePoint = tmpEdPoint + dist * eDir;

            return (sPoint, ePoint);
        }

        public static Curve GetInsecCurve(Document doc, Curve cvBase, List<ElementId> idList)
        {
            List<XYZ> insecList = new List<XYZ>();
            Curve cv = null;
            XYZ tmpStPoint = cvBase.GetEndPoint(0);
            XYZ tmpEdPoint = cvBase.GetEndPoint(1);
            foreach (ElementId id in idList)
            {
                FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                LocationCurve lCurve = inst.Location as LocationCurve;
                if (lCurve != null)
                {
                    XYZ sInst = lCurve.Curve.GetEndPoint(0);
                    XYZ eInst = lCurve.Curve.GetEndPoint(1);
                    //tmpStPointHara = new XYZ(tmpStPointHara.X, tmpStPointHara.Y, 0.0);
                    //tmpEdPointHara = new XYZ(tmpEdPointHara.X, tmpEdPointHara.Y, 0.0);
                    Curve cvInst = Line.CreateBound(sInst, eInst);
                    XYZ insec = RevitUtil.ClsRevitUtil.GetIntersection(cvBase, cvInst);
                    if (insec != null)
                    {
                        insecList.Add(insec);
                    }
                }
            }
            //交点が１つもない場合は選択したモデル線分の端点に近い腹起ベースまで交点を伸ばす
            if (insecList.Count == 0)
            {
                XYZ insec1 = ClsYMSUtil.GetLineNearFamilyInstancePoint(doc, tmpStPoint, tmpEdPoint, idList);
                XYZ insec2 = ClsYMSUtil.GetLineNearFamilyInstancePoint(doc, tmpEdPoint, tmpStPoint, idList);
                cv = Line.CreateBound(insec1, insec2);
            }
            //交点が１つしかない場合は選択したモデル線分のどちらの端点に近いかを判定する
            if (insecList.Count == 1)
            {
                double distanceToStart = insecList[0].DistanceTo(tmpStPoint);
                double distanceToEnd = insecList[0].DistanceTo(tmpEdPoint);
                XYZ insec;
                if (distanceToStart < distanceToEnd)
                {
                    // 判定したい点は始点に近いです
                    insec = ClsYMSUtil.GetLineNearFamilyInstancePoint(doc, tmpEdPoint, tmpStPoint, idList);
                    cv = Line.CreateBound(insecList[0], insec);
                }
                else
                {
                    // 判定したい点は終点に近いです
                    insec = ClsYMSUtil.GetLineNearFamilyInstancePoint(doc, tmpStPoint, tmpEdPoint, idList);
                    cv = Line.CreateBound(insec, insecList[0]);
                }
            }
            //交点が2つある場合は交点で切梁ベースを作成する
            if (insecList.Count == 2)
            {
                cv = Line.CreateBound(insecList[0], insecList[1]);
            }
            return cv;
        }


        /// <summary>
        /// 掘削指定側の単位ベクトルを取得
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id">対象のベース</param>
        /// <param name="kussaku">true:掘削側,false:反対側</param>
        /// <returns>単位ベクトル</returns>
        public static XYZ GetKussakuDirecyion(Document doc, ElementId id, bool kussaku = true)
        {
            XYZ dir = new XYZ();
            Element el = doc.GetElement(id);
            LocationCurve lCurve = el.Location as LocationCurve;
            if (lCurve == null)
            {
                return dir;
            }
            XYZ pntS = lCurve.Curve.GetEndPoint(0);
            XYZ pntE = lCurve.Curve.GetEndPoint(1);
            if (kussaku)
            {
                dir = Line.CreateBound(pntS, pntE).Direction;
            }
            else
            {
                dir = Line.CreateBound(pntE, pntS).Direction;
            }
            return dir;
        }


        public static int GetDan(string dan)
        {
            switch (dan)
            {
                case "上段":
                    {
                        return 1;
                    }
                case "下段":
                    {
                        return 2;
                    }
                default:
                    {
                        return 0;
                    }
            }
        }
        /// <summary>
        /// H形鋼のファミリパラメータの持つ大分類をダイアログ表示の分類名に変換する
        /// </summary>
        /// <param name="type">大分類</param>
        /// <returns></returns>
        public static string ChangeHBeamTypeName(string type)
        {
            switch (type)
            {
                case "広幅H形鋼":
                    {
                        return "H形鋼 広幅";
                    }
                case "中幅H形鋼":
                    {
                        return "H形鋼 中幅";
                    }
                case "細幅H形鋼":
                    {
                        return "H形鋼 細幅";
                    }
                default:
                    {
                        return string.Empty;
                    }
            }
        }

        /// <summary>
        /// startPoint,endPointに交差する線分をlinePointsから検索（四角形の一辺から残り三本を取得するイメージ）
        /// </summary>
        /// <param name="startPoint">始点</param>
        /// <param name="endPoint">終点</param>
        /// <param name="linePoints">始終点のリスト　この中から探す</param>
        /// <param name="resultList">見つかった線分の始終点をこの中に入れて返す</param>
        /// <returns>複数ある場合は終点に近い方を採用</returns>
        public static bool FindConnectedPoints(XYZ startPoint, XYZ endPoint, List<List<XYZ>> linePoints, ref List<List<XYZ>> resultList)
        {
            List<List<XYZ>> result = new List<List<XYZ>>();

            List<XYZ> tmpXYZ = new List<XYZ>();
            tmpXYZ.Add(startPoint);
            tmpXYZ.Add(endPoint);

            List<List<XYZ>> listStartSide = new List<List<XYZ>>();
            List<List<XYZ>> listEndSide = new List<List<XYZ>>();

            //選択された線分の交点を取得
            List<XYZ> lstConection = new List<XYZ>();
            Curve cvTarget = Line.CreateBound(startPoint, endPoint);
            foreach (List<XYZ> points in linePoints)
            {
                Curve cvInst = Line.CreateBound(points[0], points[1]);
                XYZ insec = RevitUtil.ClsRevitUtil.GetIntersection(cvTarget, cvInst);
                if (insec != null)
                {
                    lstConection.Add(insec);
                }
            }

            //交点が1か所の場合（連なる線分の端の場合）
            if (lstConection.Count == 1)
            {
                if (RevitUtil.ClsGeo.GEO_LE(lstConection[0].DistanceTo(endPoint),
                                  lstConection[0].DistanceTo(startPoint)))
                {
                    //終点までの距離が始点までの距離より小さい場合

                    //終点側に検索
                    if (!FindConnectedPoints(startPoint, endPoint, false, ref linePoints, ref listEndSide))
                    {
                        return false;
                    }
                    result.Add(tmpXYZ);
                    result.AddRange(listEndSide);
                }
                else
                {
                    //始点までの距離が終点までの距離より小さい場合
                    //始点側に検索
                    if (!FindConnectedPoints(startPoint, endPoint, true, ref linePoints, ref listStartSide))
                    {
                        return false;
                    }
                    result.AddRange(listStartSide.Reverse<List<XYZ>>());
                    result.Add(tmpXYZ);
                }
            }
            else
            {

                //終点側に検索
                if (!FindConnectedPoints(startPoint, endPoint, false, ref linePoints, ref listEndSide))
                {
                    return false;
                }

                //余りから支点側に検索
                if (!FindConnectedPoints(startPoint, endPoint, true, ref linePoints, ref listStartSide))
                {
                    return false;
                }

                if (listStartSide.Count > 0)
                {
                    result.AddRange(listStartSide.Reverse<List<XYZ>>());
                }
                result.Add(tmpXYZ);
                result.AddRange(listEndSide);
            }

            resultList = result;

            return true;
        }


        /// <summary>
        /// startPoint,endPointに交差する線分をlinePointsから検索（四角形の一辺から残り三本を取得するイメージ）
        /// </summary>
        /// <param name="startPoint">始点</param>
        /// <param name="endPoint">終点</param>
        /// <param name="bStartSide">支点側に近い方を採用する場合</param>
        /// <param name="linePoints">始終点のリスト　この中から探す</param>
        /// <param name="resultList">見つかった線分の始終点をこの中に入れて返す</param>
        /// <returns>複数ある場合は終点に近い方を採用</returns>
        public static bool FindConnectedPoints(XYZ startPoint, XYZ endPoint, bool bStartSide, ref List<List<XYZ>> linePoints, ref List<List<XYZ>> resultList)
        {
            try
            {
                Curve cvTarget = Line.CreateBound(startPoint, endPoint);


                List<List<XYZ>> hitPoints = new List<List<XYZ>>();
                List<XYZ> hit = new List<XYZ>();
                double dist = double.MaxValue;
                int hitindex = int.MinValue;
                int index = 0;
                foreach (List<XYZ> pointList in linePoints)
                {
                    if (pointList[0] == startPoint && pointList[1] == endPoint)
                    {
                        continue;
                    }

                    if (pointList[1] == startPoint && pointList[0] == endPoint)
                    {
                        continue;
                    }

                    Curve cvInst = Line.CreateBound(pointList[0], pointList[1]);
                    XYZ insec = RevitUtil.ClsRevitUtil.GetIntersection(cvTarget, cvInst);
                    if (insec != null)
                    {
                        if (bStartSide)
                        {
                            if (RevitUtil.ClsGeo.GEO_LE(insec.DistanceTo(startPoint), dist))
                            {
                                hit = new List<XYZ>();
                                hit.Add(pointList[0]);
                                hit.Add(pointList[1]);
                                dist = insec.DistanceTo(startPoint);
                                hitindex = index;
                            }
                        }
                        else
                        {
                            if (RevitUtil.ClsGeo.GEO_LE(insec.DistanceTo(endPoint), dist))
                            {
                                hit = new List<XYZ>();
                                hit.Add(pointList[0]);
                                hit.Add(pointList[1]);
                                dist = insec.DistanceTo(endPoint);
                                hitindex = index;
                            }
                        }
                    }
                    index++;
                }

                if (hitindex >= 0)
                {
                    linePoints.RemoveAt(hitindex);
                    resultList.Add(hit);
                    FindConnectedPoints(hit[0], hit[1], bStartSide, ref linePoints, ref resultList);
                    hitindex = int.MinValue;
                    hit = new List<XYZ>();
                }
                else
                {
                    return true;
                }

            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// 選択したモデル線分を伸ばしていきベースとの交点を取得する
        /// </summary>
        /// <param name="doc">ドキュメント</param>
        /// <param name="point">ベースと接していなかったため、伸ばしていく点</param>
        /// <param name="constPoint">選択したモデル線分の基点となる点</param>
        /// <param name="elementIds">交点を見つけるベースととなるファミリインスタンスList</param>
        /// <returns>ベースと選択したモデル線分の交点</returns>
        public static XYZ GetLineNearFamilyInstancePoint(Document doc, XYZ point, XYZ constPoint, List<ElementId> elementIds)
        {
            XYZ insec = point;
            FamilyInstance closestInstance = null;
            double closestDistance = double.MaxValue;

            foreach (ElementId id in elementIds)
            {
                FamilyInstance instance = doc.GetElement(id) as FamilyInstance;
                //LocationPoint location = instance.Location as LocationPoint;//LocationCurveだった腹起ベースのどこかしらのポイントを指定
                LocationCurve lCurve = instance.Location as LocationCurve;
                XYZ midPoint = (lCurve.Curve.GetEndPoint(0) + lCurve.Curve.GetEndPoint(1)) / 2;//とりあえず中点//中点だけでは不十分な可能性がある

                // ファミリインスタンスの位置情報を取得して、判定したい点との距離を計算する
                double distance = midPoint.DistanceTo(point);

                if (distance < closestDistance)
                {
                    closestInstance = instance;
                    closestDistance = distance;//pointとベースの中点との距離
                }
            }

            // 最も近いファミリインスタンスが見つかった場合の処理
            if (closestInstance != null)
            {
                // closestInstanceが最も近いファミリインスタンスです
                LocationCurve lCurve = closestInstance.Location as LocationCurve;
                Curve cv = lCurve.Curve;

                closestDistance = RevitUtil.ClsRevitUtil.GetLineNearPointDistance(cv, point);

                double dEx = RevitUtil.ClsRevitUtil.CovertToAPI(100);
                XYZ Direction = Line.CreateBound(constPoint, point).Direction;
                //100xXで探して見つかったインスタンスの中点までの距離X2に交点がない場合とりあえずと終了する//いくら伸ばしても交点が一生現れない（平行など）の判定が必要かもしれない
                //上記終了条件だと交わることが出来るのに届かない可能性がある
                //伸ばしていく端点から最も遠い端点までの距離を上限とする
                for (int ext = 1; dEx * ext <= closestDistance; ext++)
                {
                    //切梁ベースを配置するために選択した線分のみ伸ばしているためモデル線分の進行方向に腹起ベースの1点が見えていないといけない
                    XYZ hojyoS = new XYZ(point.X + (dEx * ext * Direction.X),
                                         point.Y + (dEx * ext * Direction.Y),
                                         point.Z);
                    Curve cvHojyo = Line.CreateBound(constPoint, hojyoS);
                    XYZ insecP = RevitUtil.ClsRevitUtil.GetIntersection(cvHojyo, cv);
                    if (insecP != null)
                    {
                        insec = insecP;
                        break;
                    }
                }
            }

            return insec;
        }

        public static List<ElementId> GetIntersectionBase(Document doc, ElementId id)
        {
            List<ElementId> insecIdList = new List<ElementId>();
            FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
            Curve cvBase = (inst.Location as LocationCurve).Curve;
            ElementId levelId = inst.Host.Id;
            //図面上のベースをレベルを指定して全て取得※ファミリ名「ベース」の部分一致だとエラーが起きる図面があるため各ベースから取得
            List<ElementId> targetFamilies = new List<ElementId>();// ClsRevitUtil.GetSelectLevelCreatedFamilyInstanceList(doc, "ベース", levelId, true);
            //腹起と切梁のみレベルを指定して全て取得
            targetFamilies.AddRange(ClsHaraokoshiBase.GetAllHaraokoshiBaseList(doc, levelId));
            targetFamilies.AddRange(ClsKiribariBase.GetAllKiribariBaseList(doc, levelId));
            targetFamilies.AddRange(ClsCornerHiuchiBase.GetAllCornerHiuchiBaseList(doc));
            targetFamilies.AddRange(ClsKiribariHiuchiBase.GetAllKiribariHiuchiBaseList(doc));
            targetFamilies.AddRange(ClsKiribariUkeBase.GetAllKiribariUkeBaseList(doc));
            targetFamilies.AddRange(ClsKiribariTsunagizaiBase.GetAllKiribariTsunagizaiBaseList(doc));
            targetFamilies.AddRange(ClsHiuchiTsunagizaiBase.GetAllHiuchiTsunagizaiBaseList(doc));
            targetFamilies.AddRange(ClsKiribariTsugiBase.GetAllKiribariTsugiBaseList(doc));
            //List<ElementId> targetFamilies = new List<ElementId>();
            //foreach (string baseName in ClsGlobal.m_baseShinList)
            //{
            //    foreach (ElementId elem in elements)
            //    {
            //        if (elem != null && doc.GetElement(elem).Name == baseName)
            //        {
            //            targetFamilies.Add(elem);
            //        }
            //    }
            //}

            foreach (ElementId tgId in targetFamilies)
            {
                FamilyInstance tgInst = doc.GetElement(tgId) as FamilyInstance;
                if (inst == tgInst)
                    continue;
                Curve tgCv = (tgInst.Location as LocationCurve).Curve;
                XYZ insec = ClsRevitUtil.GetIntersection(cvBase, tgCv);
                if (insec != null)//ClsRevitUtil.IsPointOnCurve(tgCv, sPnt))
                {
                    insecIdList.Add(tgId);
                }
            }
            return insecIdList;
        }
        public static List<ElementId> GetIntersectionHaraKiriBase(Document doc, ElementId id)
        {
            List<ElementId> insecIdList = new List<ElementId>();
            FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
            Curve cvBase = (inst.Location as LocationCurve).Curve;
            ElementId levelId = inst.Host.Id;
            //図面上のベースをレベルを指定して全て取得
            List<ElementId> targetFamilies = new List<ElementId>();

            //List<ElementId> elements = ClsRevitUtil.GetSelectLevelCreatedFamilyInstanceList(doc, "ベース", levelId, true);
            //foreach (string baseName in ClsGlobal.m_baseKiriOrHaraList)
            //{
            //    foreach (ElementId elem in elements)
            //    {
            //        if (elem != null && doc.GetElement(elem).Name == baseName)
            //        {
            //            targetFamilies.Add(elem);
            //        }
            //    }
            //}

            //腹起と切梁のみレベルを指定して全て取得
            targetFamilies.AddRange(ClsHaraokoshiBase.GetAllHaraokoshiBaseList(doc, levelId));
            targetFamilies.AddRange(ClsKiribariBase.GetAllKiribariBaseList(doc, levelId));

            foreach (ElementId tgId in targetFamilies)
            {
                FamilyInstance tgInst = doc.GetElement(tgId) as FamilyInstance;
                if (inst == tgInst)
                    continue;
                Curve tgCv = (tgInst.Location as LocationCurve).Curve;
                XYZ insec = ClsRevitUtil.GetIntersection(cvBase, tgCv);
                if (insec != null)//ClsRevitUtil.IsPointOnCurve(tgCv, sPnt))
                {
                    insecIdList.Add(tgId);
                }
            }
            return insecIdList;
        }

        /// <summary>
        /// 全ベースを取得
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<ElementId> GetAllBase(Document doc)
        {
            //図面上のベースを全て取得
            List<ElementId> elements = ClsRevitUtil.GetAllCreatedFamilyInstanceList(doc);
            List<ElementId> targetFamilies = new List<ElementId>();
            foreach (string baseName in ClsGlobal.m_baseKiriOrHaraList)
            {
                foreach (ElementId elem in elements)
                {
                    if (elem != null && doc.GetElement(elem).Name == baseName)
                    {
                        targetFamilies.Add(elem);
                    }
                }
            }

            return targetFamilies;
        }

        public static List<ElementId> GetIntersectionHaraokoshiBase(Document doc, ElementId id)
        {
            List<ElementId> insecIdList = new List<ElementId>();
            //図面上のベースを全て取得
            List<ElementId> elements = ClsRevitUtil.GetAllCreatedFamilyInstanceList(doc);
            List<ElementId> targetFamilies = new List<ElementId>();
            foreach (ElementId elem in elements)
            {
                if (elem != null && doc.GetElement(elem).Name == "腹起ベース")
                {
                    targetFamilies.Add(elem);
                }
            }

            FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
            Curve cvBase = (inst.Location as LocationCurve).Curve;


            foreach (ElementId tgId in targetFamilies)
            {
                FamilyInstance tgInst = doc.GetElement(tgId) as FamilyInstance;
                if (inst == tgInst)
                    continue;
                Curve tgCv = (tgInst.Location as LocationCurve).Curve;
                XYZ insec = ClsRevitUtil.GetIntersection(cvBase, tgCv);
                if (insec != null)//ClsRevitUtil.IsPointOnCurve(tgCv, sPnt))
                {
                    insecIdList.Add(tgId);
                }
            }
            return insecIdList;
        }

        public static List<ElementId> GetIntersectionKiribariBase(Document doc, ElementId id)
        {
            List<ElementId> insecIdList = new List<ElementId>();
            FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
            Curve cvBase = (inst.Location as LocationCurve).Curve;
            ElementId levelId = inst.Host.Id;

            //図面上の切梁ベースをレベルを指定して全て取得
            List<ElementId> targetFamilies = ClsRevitUtil.GetSelectLevelCreatedFamilyInstanceList(doc, ClsKiribariBase.baseName, levelId);

            foreach (ElementId tgId in targetFamilies)
            {
                FamilyInstance tgInst = doc.GetElement(tgId) as FamilyInstance;
                if (inst == tgInst)
                    continue;
                Curve tgCv = (tgInst.Location as LocationCurve).Curve;
                XYZ insec = ClsRevitUtil.GetIntersection(cvBase, tgCv);
                if (insec != null)//ClsRevitUtil.IsPointOnCurve(tgCv, sPnt))
                {
                    insecIdList.Add(tgId);
                }
            }
            return insecIdList;
        }
        public static List<ElementId> GetIntersectionHiuchiBase(Document doc, ElementId id)
        {
            List<ElementId> insecIdList = new List<ElementId>();
            FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
            Curve cvBase = (inst.Location as LocationCurve).Curve;
            ElementId levelId = inst.Host.Id;

            //図面上の火打ベースと付くベースのレベルを指定して全て取得
            List<ElementId> targetFamilies = ClsRevitUtil.GetSelectLevelCreatedFamilyInstanceList(doc, "火打ベース", levelId, true);

            foreach (ElementId tgId in targetFamilies)
            {
                FamilyInstance tgInst = doc.GetElement(tgId) as FamilyInstance;
                if (inst == tgInst)
                    continue;
                Curve tgCv = (tgInst.Location as LocationCurve).Curve;
                XYZ insec = ClsRevitUtil.GetIntersection(cvBase, tgCv);
                if (insec != null)//ClsRevitUtil.IsPointOnCurve(tgCv, sPnt))
                {
                    insecIdList.Add(tgId);
                }
            }
            return insecIdList;
        }
        /// <summary>
        /// ファミリが存在しない場合にロードする
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="familyName"></param>
        /// <param name="familyPath"></param>
        /// <returns></returns>
        public static bool LoadFamilyIfNotExists(Document doc, string familyName, string familyPath)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            var existingFamilies = collector.OfClass(typeof(Family))
                .Cast<Family>()
                .Where(family => family.Name == familyName);

            if (!existingFamilies.Any())
            {
                Transaction trans = new Transaction(doc, "Load and Place Family");
                trans.Start();
                try
                {
                    // ファミリをロード
                    doc.LoadFamily(familyPath, out Family family);
                    if (family != null)
                    {
                        // ファミリが正常に読み込まれた場合
                        trans.Commit();
                        return true;
                    }
                    else
                    {
                        TaskDialog.Show("Error", "ファミリのロードに失敗しました。");
                        trans.RollBack();
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("Error", ex.Message);
                    trans.RollBack();
                    return false;
                }
                finally
                {
                    trans.Dispose();
                }
            }

            return true;
        }

        /// <summary>
        /// 対象のシンボルをアクティブにする
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="familyName"></param>
        /// <param name="symbolName"></param>
        public static void ActivateSymbolInFamily(Document doc, string familyName, string symbolName)
        {
            // ファミリを取得
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            var families = collector.OfClass(typeof(Family))
                .Cast<Family>()
                .Where(family => family.Name == familyName);

            foreach (var family in families)
            {
                // ファミリ内のシンボルを取得
                var familySymbols = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_GenericModel)
                    .OfClass(typeof(FamilySymbol))
                    .Cast<FamilySymbol>()
                    .Where(symbol => symbol.Family.Id == family.Id && symbol.Name == symbolName);

                foreach (var symbol in familySymbols)
                {
                    // シンボルをアクティブに設定
                    if (!symbol.IsActive)
                    {
                        using (Transaction transaction = new Transaction(doc))
                        {
                            transaction.Start("Activate Symbol");
                            symbol.Activate();
                            transaction.Commit();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 主材のファミリからCurveを取得
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static Curve GetCurveFromSyuzaiFamilyInstance(FamilyInstance instance, bool flattenZ = false)
        {
            if (!instance.Symbol.Family.Name.Contains("山留主材"))
            {
                return null;
            }

            LocationPoint location = instance.Location as LocationPoint;
            if (location == null)
            {
                return null;
            }

            XYZ position = location.Point;
            XYZ facingOrientation = instance.FacingOrientation;

            // 長さを取得
            double syuzaiLength = ClsRevitUtil.CovertToAPI(ClsYMSUtil.GetSyuzaiLength(instance.Symbol.Family.Name));
            if (syuzaiLength == 0)
            {
                return null;
            }

            // 回転行列を生成してファミリの向きを調整
            Transform rotation = Transform.CreateRotationAtPoint(XYZ.BasisZ, -Math.PI / 2, position);
            facingOrientation = rotation.OfVector(facingOrientation);

            XYZ ptSt = position;
            XYZ ptEd = position + (syuzaiLength * facingOrientation);

            // Z 座標を 0 にする
            if (flattenZ)
            {
                ptSt = new XYZ(ptSt.X, ptSt.Y, 0);
                ptEd = new XYZ(ptEd.X, ptEd.Y, 0);
            }

            // Curve を作成
            return Line.CreateBound(ptSt, ptEd);
        }

        #endregion

        #region BK
        //public static bool GetInstallDir(ref string dir)
        //{
        //    dir = null;
        //    string keyName = null;

        //    keyName = "Software\\hirose\\YMS";

        //    string name = "InstallDir";

        //    try
        //    {
        //        Microsoft.Win32.RegistryKey wkReg;
        //        wkReg = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(keyName);
        //        dir = wkReg.GetValue(name).ToString();

        //        wkReg.Close();
        //    }
        //    catch
        //    {
        //        return false;
        //    }

        //    if (String.IsNullOrEmpty(dir))
        //    {
        //        return false;
        //    }

        //    return true;
        //}


        //public static List<string> GetKouzaiSizeName(string folderPath)
        //{
        //    List<string> nameList = new List<string>();
        //    string ext = "rfa";
        //    // 指定したフォルダを表すDirectoryInfoオブジェクトを作成
        //    DirectoryInfo directory = new DirectoryInfo(folderPath);

        //    // 指定した拡張子を持つファイルを取得
        //    FileInfo[] files = directory.GetFiles("*." + ext);

        //    foreach (FileInfo file in files)
        //    {
        //        nameList.Add(file.Name);
        //    }

        //    return nameList;
        //}
        #endregion

        #region 斜梁
        public static XYZ[] CalcShabariSection(Line shaBaseLine, Plane shaBasePlane, double shaWidthMm, double shaHeightMm, Plane sectionPlane)
        {
            // 20250217時点 動作未確認
            var w = ClsRevitUtil.CovertToAPI(shaWidthMm);
            var h = ClsRevitUtil.CovertToAPI(shaHeightMm);

            var dirH = 0.5 * h * shaBasePlane.Normal.Normalize();
            var dirW = 0.5 * w * shaBaseLine.Direction.CrossProduct(shaBasePlane.Normal).Normalize();

            return Enumerable.Empty<XYZ>()
                .Append(dirH + dirW)
                .Append(dirH - dirW)
                .Append(-dirH - dirW)
                .Append(-dirH + dirW)
                .Select(x => shaBaseLine.Origin + x)
                .Select(x => Line.CreateUnbound(x, shaBaseLine.Direction))
                .Select(x => SLibRevitReo.IntersectPlaneAndLine(sectionPlane, x))
                .Where(x => x != null)
                .ToArray();
        }

        /// <summary>
        /// 斜梁鋼材の断面における最高点座標を算出
        /// 求められない時は null を返す
        /// </summary>
        /// <param name="shaBaseLine">斜梁ベース直線</param>
        /// <param name="shaBasePlane">斜梁ベース平面</param>
        /// <param name="shaWidthMm">斜梁鋼材の横幅 (mm) = 斜梁ベース平面方向の幅</param>
        /// <param name="shaHeightMm">斜梁鋼材の縦長さ (mm) = 斜梁ベース平面の法線方向の幅</param>
        /// <param name="sectionPlane">断面平面</param>
        /// <returns></returns>
        public static XYZ CalcShabariSectionTop(Line shaBaseLine, Plane shaBasePlane, double shaWidthMm, double shaHeightMm, Plane sectionPlane, bool bFirst = false)
        {
            var sectionPoints = CalcShabariSection(shaBaseLine, shaBasePlane, shaWidthMm, shaHeightMm, sectionPlane);

            if (sectionPoints.Length == 0)
            {
                return null;
            }

            XYZ maxPnt = new XYZ();
            if (TryGetHighestPoint(sectionPoints, out maxPnt, bFirst))
            {
                return sectionPoints.OrderBy(x => -x.Z).FirstOrDefault();
            }
            else
            {
                return maxPnt;
            }

        }

        /// <summary>
        /// ①：XYZの配列を引数に与える
        ///②：Z値の高さを比較し一番高い座標値を探す
        ///③：②で探した最高座標値が一個の場合はTrue,複数ある場合はFalseを返す
        ///④：Falseの場合、複数の最高座標値の中点を引数で返す
        /// </summary>
        /// <param name="points"></param>
        /// <param name="midpoint"></param>
        /// <param name="bFirst"></param>
        /// <returns></returns>
        public static bool TryGetHighestPoint(XYZ[] points, out XYZ midpoint, bool bFirst = false)
        {
            midpoint = null;

            if (points == null || points.Length == 0)
                throw new ArgumentException("points array is null or empty.");

            // 最大Z値を取得
            double maxZ = points.Max(p => p.Z);

            // 最大Z値を持つ点を取得（誤差考慮）
            var highestPoints = points.Where(p => Math.Abs(p.Z - maxZ) < 1e-6).ToArray();

            if (highestPoints.Length == 1)
            {
                return true;
            }
            if(bFirst)
            {
                midpoint =  highestPoints[0];
                return true;
            }
            else
            {
                // 中点を計算
                double avgX = highestPoints.Average(p => p.X);
                double avgY = highestPoints.Average(p => p.Y);
                double avgZ = highestPoints.Average(p => p.Z);

                midpoint = new XYZ(avgX, avgY, avgZ);
                return false;
            }
        }

        /// <summary>
        /// 斜梁鋼材の断面における最下点座標を算出
        /// 受け材の幅を考慮
        /// </summary>
        /// <param name="shaBaseLine"></param>
        /// <param name="shaBasePlane"></param>
        /// <param name="shaWidthMm"></param>
        /// <param name="shaHeightMm"></param>
        /// <param name="sectionPlane"></param>
        /// <param name="ukeWidthMn">受け材の幅 (mm)</param>
        /// <returns></returns>
        public static XYZ CalcShabariSectionBottom(Line shaBaseLine, Plane shaBasePlane, double shaWidthMm, double shaHeightMm, Plane sectionPlane, double ukeWidthMn)
        {
            var ukeWhalf = ClsRevitUtil.CovertToAPI(0.5 * ukeWidthMn);
            var sectionPlanes = Enumerable.Empty<double>()
                .Append(ukeWhalf)
                .Append(-ukeWhalf)
                .Select(x => Plane.CreateByNormalAndOrigin(sectionPlane.Normal, sectionPlane.Origin + x * sectionPlane.Normal.Normalize()));
            var sectionPointSet = sectionPlanes.Select(x => CalcShabariSection(shaBaseLine, shaBasePlane, shaWidthMm, shaHeightMm, x)).ToArray();
            //var sectionPoints = CalcShabariSection(shaBaseLine, shaBasePlane, shaWidthMm, shaHeightMm, sectionPlane);
            var sectionPoints = sectionPointSet.SelectMany(x => x).ToArray();

            if (sectionPoints.Length == 0)
            {
                return null;
            }

            return sectionPoints.OrderBy(x => x.Z).FirstOrDefault();
        }

        /// <summary>
        /// ベースまたはモデル線分 などから幾何オブジェクトの線分を返す
        /// 想定外の引数だったら null
        /// M.Sakuraba
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Line GetBaseLine(Document doc, ElementId id)
        {
            var elem = doc.GetElement(id);

            if (elem is FamilyInstance baseInstance)
            {
                var family = baseInstance.Symbol.Family;
                //if (family.FamilyPlacementType == FamilyPlacementType.WorkPlaneBased)

                if (true)
                {
                    // 作業面上に配置するベースファミリ (斜梁ベースを想定)
                    var coordinate = baseInstance.GetTotalTransform();
                    var start = coordinate.Origin;
                    var dir = coordinate.BasisX;
                    var length = ClsRevitUtil.GetParameterDouble(doc, baseInstance.Id, "長さ");
                    var end = start + length * dir.Normalize();
                    return Line.CreateBound(start, end);
                }
                else if (family.FamilyPlacementType == FamilyPlacementType.CurveBased)
                {
                    // LocationCurve 上に配置するベースファミリ (腹起や切梁のベースなど)
                    var lCurve = baseInstance.Location as LocationCurve;
                    var tmpStPoint = lCurve.Curve.GetEndPoint(0);
                    var tmpEdPoint = lCurve.Curve.GetEndPoint(1);
                    return Line.CreateBound(tmpStPoint, tmpEdPoint);
                }
                else
                {
                    return null;
                }
            }
            else if (elem is ModelLine modelline)
            {
                return modelline.GeometryCurve as Line;
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
    public class ClsTanbuParts
    {
        #region コンストラクタ

        public ClsTanbuParts(string steelSize, string tanbuParts, string tanbuSize)
        {
            this.SteelSize = steelSize;
            this.TanbuParts = tanbuParts;
            this.TanbuSize = tanbuSize;
        }

        #endregion

        #region プロパティ

        /// <summary>鋼材サイズ</summary>
        public string SteelSize { get; set; }

        /// <summary>端部部品</summary>
        public string TanbuParts { get; set; }

        /// <summary>端部部品サイズ</summary>
        public string TanbuSize { get; set; }


        #endregion
    }
}
