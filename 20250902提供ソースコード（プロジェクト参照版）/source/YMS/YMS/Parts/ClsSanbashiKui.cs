using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static YMS.Parts.ClsAtamaTsunagi;
using static YMS.Parts.ClsSanbashiKui;
using Document = Autodesk.Revit.DB.Document;

namespace YMS.Parts
{
    public class ClsSanbashiKui
    {
        #region Enum

        /// <summary>
        /// 配置方法
        /// </summary>
        public enum CreateType
        {
            Intersection,   // 交点
            OnePoint        // 1点
        }

        /// <summary>
        /// 杭タイプ
        /// </summary>
        public enum PileType
        {
            Sanbashi,       // 桟橋杭
            Kenyou,         // 兼用杭
            DanmenHenka,    // 断面変化杭
            TC              // TC杭
        }

        /// <summary>
        /// 固定方法
        /// </summary>
        public enum FixingType
        {
            Welding,    //溶接
            Bolt,       //ボルト
        }

        /// <summary>
        /// トッププレートのサイズ
        /// </summary>
        public enum SizeOption
        {
            Custom,    // 任意
            Standard   // 定型
        }


        #endregion

        #region 変数

        /// <summary>
        /// 作成方法(高さ位置)
        /// </summary>
        public bool m_IsFromGL { get; set; }

        /// <summary>
        /// GLからの高さ
        /// </summary>
        public double m_HightFromGL { get; set; }

        /// <summary>
        /// 杭タイプ
        /// </summary>
        public PileType m_PileType { get; set; }

        /// <summary>
        /// 配置方法
        /// </summary>
        public CreateType m_CreateType { get; set; }

        /// <summary>
        /// 鋼材タイプ
        /// </summary>
        public string m_KouzaiType { get; set; }

        /// <summary>
        /// 鋼材サイズ
        /// </summary>
        public string m_KouzaiSize { get; set; }

        /// <summary>
        /// 鋼材サイズ
        /// </summary>
        public string m_KouzaiSizeKoudaikui { get; set; }

        /// <summary>
        /// 鋼材サイズ
        /// </summary>
        public string m_KouzaiSizeDanmenHenkaKui { get; set; }

        /// <summary>
        /// 杭の全長
        /// </summary>
        public double m_PileTotalLength { get; set; }

        /// <summary>
        /// 断面変化杭の全長 上段
        /// </summary>
        public double m_PileTotalLength_Top { get; set; }

        /// <summary>
        /// 断面変化杭の全長 下段
        /// </summary>
        public double m_PileTotalLength_Bottom { get; set; }

        /// <summary>
        /// 杭の配置角度
        /// </summary>
        public double m_PileAngle { get; set; }

        /// <summary>
        /// 残置
        /// </summary>
        public string m_Zanti { get; set; }

        /// <summary>
        /// 残置長さ
        /// </summary>
        public double m_ZantiLength { get; set; }

        /// <summary>
        /// トッププレートの取付有無
        /// </summary>
        public bool m_UseTopPlate { get; set; }

        /// <summary>
        /// トッププレートのサイズ(任意 or 定型)
        /// </summary>
        public SizeOption m_TopPlateSizeOption { get; set; }

        /// <summary>
        /// トッププレートのサイズ
        /// </summary>
        public string m_TopPlateSize { get; set; }

        /// <summary>
        /// エンドプレートの厚み
        /// </summary>
        public double m_EndPlateThickness { get; set; }

        // ---継手関連---

        /// <summary>
        /// 継手箇所数
        /// </summary>
        public int m_TsugiteCount { get; set; }

        /// <summary>
        /// 継手箇所数
        /// </summary>
        public int m_TsugiteCount2 { get; set; }

        /// <summary>
        /// 継手固定方法
        /// </summary>
        public FixingType m_FixingType { get; set; }

        /// <summary>
        /// 継手固定方法
        /// </summary>
        public FixingType m_FixingType2 { get; set; }

        /// <summary>
        /// 杭長さ
        /// </summary>
        public List<int> m_PileLengthList { get; set; }

        /// <summary>
        /// 杭長さ
        /// </summary>
        public List<int> m_PileLengthList2 { get; set; }

        /// <summary>
        /// 継手プレートサイズ フランジ 外側
        /// </summary>
        public string m_TsugitePlateSize_Out { get; set; }

        /// <summary>
        /// 継手プレートサイズ フランジ 外側
        /// </summary>
        public string m_TsugitePlateSize_Out2 { get; set; }

        /// <summary>
        /// 継手プレート枚数 フランジ 外側
        /// </summary>
        public string m_TsugitePlateQuantity_Out { get; set; }

        /// <summary>
        /// 継手プレート枚数 フランジ 外側
        /// </summary>
        public string m_TsugitePlateQuantity_Out2 { get; set; }

        /// <summary>
        /// 継手プレートサイズ フランジ 内側
        /// </summary>
        public string m_TsugitePlateSize_In { get; set; }

        /// <summary>
        /// 継手プレートサイズ フランジ 内側
        /// </summary>
        public string m_TsugitePlateSize_In2 { get; set; }

        /// <summary>
        /// 継手プレート枚数 フランジ 内側
        /// </summary>
        public string m_TsugitePlateQuantity_In { get; set; }

        /// <summary>
        /// 継手プレート枚数 フランジ 内側
        /// </summary>
        public string m_TsugitePlateQuantity_In2 { get; set; }

        /// <summary>
        /// 継手プレートサイズ ウェブ 1
        /// </summary>
        public string m_TsugitePlateSize_Web1 { get; set; }

        /// <summary>
        /// 継手プレートサイズ ウェブ 1
        /// </summary>
        public string m_TsugitePlateSize_Web1_2 { get; set; }

        /// <summary>
        /// 継手プレート枚数 ウェブ 1
        /// </summary>
        public string m_TsugitePlateQuantity_Web1 { get; set; }

        /// <summary>
        /// 継手プレート枚数 ウェブ 1
        /// </summary>
        public string m_TsugitePlateQuantity_Web1_2 { get; set; }

        /// <summary>
        /// 継手プレートサイズ ウェブ 2
        /// </summary>
        public string m_TsugitePlateSize_Web2 { get; set; }

        /// <summary>
        /// 継手プレートサイズ ウェブ 2
        /// </summary>
        public string m_TsugitePlateSize_Web2_2 { get; set; }

        /// <summary>
        /// 継手プレート枚数 ウェブ 2
        /// </summary>
        public string m_TsugitePlateQuantity_Web2 { get; set; }

        /// <summary>
        /// 継手プレート枚数 ウェブ 2
        /// </summary>
        public string m_TsugitePlateQuantity_Web2_2 { get; set; }

        /// <summary>
        /// 継手ボルトサイズ フランジ
        /// </summary>
        public string m_TsugiteBoltSize_Flange { get; set; }

        /// <summary>
        /// 継手ボルトサイズ フランジ
        /// </summary>
        public string m_TsugiteBoltSize_Flange2 { get; set; }

        /// <summary>
        /// 継手ボルト本数 フランジ
        /// </summary>
        public string m_TsugiteBoltQuantity_Flange { get; set; }

        /// <summary>
        /// 継手ボルト本数 フランジ
        /// </summary>
        public string m_TsugiteBoltQuantity_Flange2 { get; set; }

        /// <summary>
        /// 継手ボルトサイズ ウェブ
        /// </summary>
        public string m_TsugiteBoltSize_Web { get; set; }

        /// <summary>
        /// 継手ボルトサイズ ウェブ
        /// </summary>
        public string m_TsugiteBoltSize_Web2 { get; set; }

        /// <summary>
        /// 継手ボルト本数 ウェブ
        /// </summary>
        public string m_TsugiteBoltQuantity_Web { get; set; }

        /// <summary>
        /// 継手ボルト本数 ウェブ
        /// </summary>
        public string m_TsugiteBoltQuantity_Web2 { get; set; }

        #endregion

        /// <summary>
        ///
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="insertPoint"></param>
        /// <param name="levelId"></param>
        /// <param name="renzokushori">同一コマンド内で同タイプ名のファミリをまとめて配置するときはTRUE　</param>
        /// <param name="uniqueName"></param>
        /// <returns></returns>
        public bool CreateSanbashiKui(Document doc, XYZ insertPoint, ElementId levelId, bool renzokushori, ref string uniqueName)
        {
            // ファミリの取得処理
            string symbolFolpath = ClsZumenInfo.GetYMSFolder();
            string familyPath = "";
            string familyName = "";
            string typeName = "";

            switch (m_PileType)
            {
                case PileType.Sanbashi:
                    familyPath = Master.ClsHBeamCsv.GetPileFamilyPath(m_KouzaiSize);
                    familyName = ClsRevitUtil.GetFamilyName(familyPath);
                    typeName = "支持杭";
                    break;
                case PileType.Kenyou:
                    familyPath = Master.ClsHBeamCsv.GetPileFamilyPath(m_KouzaiSize);
                    familyName = ClsRevitUtil.GetFamilyName(familyPath);
                    typeName = "兼用杭_棚杭、支持杭";
                    break;
                case PileType.DanmenHenka:
                    familyPath = Master.ClsDanmenHenkaKuiCsv.GetFamilyPath(m_KouzaiSize);
                    familyName = ClsRevitUtil.GetFamilyName(familyPath);
                    typeName = "1-A";
                    break;
                case PileType.TC:
                    familyPath = Master.ClsHBeamCsv.GetPileFamilyPath(m_KouzaiSize);
                    familyName = ClsRevitUtil.GetFamilyName(familyPath);
                    typeName = "TC杭";
                    break;
                default:
                    return false;
            }

            if (!ClsRevitUtil.LoadFamilySymbolData(doc, familyPath, typeName, out FamilySymbol sym))
            {
                return false;
            }

            FamilySymbol sym2 = null;
            if (m_TopPlateSizeOption == SizeOption.Custom)
            {
                string folderPath = Parts.ClsZumenInfo.GetYMSFolder();
                string searchPattern = "*ﾄｯﾌﾟﾌﾟﾚｰﾄ_任意.rfa";

                // 一致するファイルの完全パスを取得
                string[] foundFiles = FindFiles(folderPath, searchPattern);
                if (foundFiles.Length == 1)
                {
                    ClsRevitUtil.LoadFamilySymbolData(doc, foundFiles.First(), "ﾄｯﾌﾟﾌﾟﾚｰﾄ_任意", out sym2);
                }
            }

            FamilySymbol tmpsym = null;
            ElementId tmpcreatedId = null;

            // インスタンスの作成処理
            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                t.Start();

                // 一意の名前を生成
                if (uniqueName == string.Empty)
                {
                    if (familyName.Contains("断面変化杭"))
                    {
                        typeName = "断面変化杭";
                    }
                    uniqueName = GetUniqueSymbolName(doc, familyName, typeName);
                }

                FamilySymbol duplicatedSymbol = sym;

                // インスタンスの作成
                ElementId createdId = ClsRevitUtil.Create(doc, insertPoint, levelId, duplicatedSymbol);

                //タイプ名を変更
                duplicatedSymbol = ClsRevitUtil.ChangeTypeID2(doc, duplicatedSymbol, createdId, uniqueName, renzokushori);

                // 部材の高さを設定
                if (m_IsFromGL)
                {
                    // GLからの高さ
                    ClsRevitUtil.SetParameter(doc, createdId, "基準レベルからの高さ", ClsRevitUtil.CovertToAPI(m_HightFromGL));
                }

                // 部材の回転処理
                Line rotationAxis = Line.CreateBound(insertPoint, insertPoint + XYZ.BasisZ);
                double radians = m_PileAngle * (Math.PI / 180.0);
                ElementTransformUtils.RotateElement(doc, createdId, rotationAxis, radians);

                // インスタンスパラメータの設定
                ClsRevitUtil.SetTypeParameter(duplicatedSymbol, "長さ", ClsRevitUtil.CovertToAPI(m_PileTotalLength));

                if (m_Zanti == "一部埋め殺し")
                {
                    ClsRevitUtil.SetTypeParameter(duplicatedSymbol, "残置/引抜", m_Zanti + "(" + (m_ZantiLength.ToString()) + ")");
                }
                else
                {
                    ClsRevitUtil.SetTypeParameter(duplicatedSymbol, "残置/引抜", m_Zanti);
                }

                if (m_PileType == PileType.DanmenHenka)
                {
                    ClsRevitUtil.SetTypeParameter(duplicatedSymbol, "上杭ジョイント数", m_TsugiteCount);
                    for (int i = 0; i < m_TsugiteCount; i++)
                    {
                        ClsRevitUtil.SetTypeParameter(duplicatedSymbol, "上杭" + (i + 1), ClsRevitUtil.CovertToAPI(m_PileLengthList[i]));
                    }
                    ClsRevitUtil.SetTypeParameter(duplicatedSymbol, "上杭長さ", ClsRevitUtil.CovertToAPI(m_PileTotalLength_Top));

                    ClsRevitUtil.SetTypeParameter(duplicatedSymbol, "下杭ジョイント数", m_TsugiteCount2);
                    for (int i = 0; i < m_TsugiteCount2; i++)
                    {
                        ClsRevitUtil.SetTypeParameter(duplicatedSymbol, "下杭" + (i + 1), ClsRevitUtil.CovertToAPI(m_PileLengthList2[i]));
                    }
                    ClsRevitUtil.SetTypeParameter(duplicatedSymbol, "下杭長さ", ClsRevitUtil.CovertToAPI(m_PileTotalLength_Bottom));

                    ClsRevitUtil.SetTypeParameter(duplicatedSymbol, "エンドプレート厚さ", ClsRevitUtil.CovertToAPI(m_EndPlateThickness));
                }
                else
                {
                    ClsRevitUtil.SetTypeParameter(duplicatedSymbol, "ジョイント数", m_TsugiteCount);

                    for (int i = 0; i < m_TsugiteCount + 1; i++)
                    {
                        ClsRevitUtil.SetTypeParameter(duplicatedSymbol, "杭" + (i + 1), ClsRevitUtil.CovertToAPI(m_PileLengthList[i]));
                    }
                    ClsRevitUtil.SetTypeParameter(duplicatedSymbol, "長さ", ClsRevitUtil.CovertToAPI(m_PileTotalLength));
                }

                if (m_UseTopPlate)
                {
                    string koteiWay = "ﾎﾞﾙﾄ";
                    if (m_FixingType == FixingType.Welding)
                    {
                        koteiWay = "溶接";
                    }

                    string tugitename = familyName.Replace("杭_", "") + koteiWay + "継手";
                    FamilySymbol tugiteSym = ClsRevitUtil.GetFamilySymbol(doc, tugitename, tugitename);
                    if (tugiteSym != null)
                    {
                        ClsRevitUtil.SetTypeParameter(duplicatedSymbol, "継手", tugiteSym.Id);
                    }

                    if (m_TopPlateSizeOption != SizeOption.Custom)
                    {
                        FamilySymbol topPlateSym = ClsRevitUtil.GetFamilySymbol(doc, "ﾄｯﾌﾟﾌﾟﾚｰﾄ_定形", m_TopPlateSize);
                        if (topPlateSym != null)
                        {
                            ClsRevitUtil.SetTypeParameter(duplicatedSymbol, "トッププレート種類", topPlateSym.Id);
                        }
                    }
                    else
                    {
                        FamilySymbol topPlateSym = ClsRevitUtil.GetFamilySymbol(doc, "ﾄｯﾌﾟﾌﾟﾚｰﾄ_任意", "ﾄｯﾌﾟﾌﾟﾚｰﾄ_任意");
                        if (topPlateSym != null)
                        {
                            ClsRevitUtil.SetTypeParameter(duplicatedSymbol, "トッププレート種類", topPlateSym.Id);
                        }
                    }

                    SplitString(m_TopPlateSize, out double topPlateT, out double topPlateW, out double topPlateD);
                    ClsRevitUtil.SetTypeParameter(duplicatedSymbol, "トッププレート", 1);
                    ClsRevitUtil.SetTypeParameter(duplicatedSymbol, "トッププレートW", ClsRevitUtil.CovertToAPI(topPlateW));
                    ClsRevitUtil.SetTypeParameter(duplicatedSymbol, "トッププレートD", ClsRevitUtil.CovertToAPI(topPlateD));
                    ClsRevitUtil.SetTypeParameter(duplicatedSymbol, "トッププレート厚さ", ClsRevitUtil.CovertToAPI(topPlateT));

                    tmpsym = duplicatedSymbol;
                    tmpcreatedId = createdId;
                }

                // インスタンスに拡張データを付与
                SetCustomData(doc, createdId);

                t.Commit();
            }

            if (m_UseTopPlate && m_TopPlateSizeOption == SizeOption.Custom)
            {
                FamilySymbol tmptopPlateSym = null;
                using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                {
                    t.Start();

                    // 一意の名前を生成
                    string uniqueName_T = GetUniqueSymbolName(doc, "ﾄｯﾌﾟﾌﾟﾚｰﾄ_任意", "ﾄｯﾌﾟﾌﾟﾚｰﾄ_任意");

                    FamilySymbol duplicatedSymbol = sym2.Duplicate(uniqueName_T) as FamilySymbol;

                    SplitString(m_TopPlateSize, out double topPlateT, out double topPlateW, out double topPlateD);
                    ClsRevitUtil.SetTypeParameter(duplicatedSymbol, "W", ClsRevitUtil.CovertToAPI(topPlateW));
                    ClsRevitUtil.SetTypeParameter(duplicatedSymbol, "D", ClsRevitUtil.CovertToAPI(topPlateD));
                    ClsRevitUtil.SetTypeParameter(duplicatedSymbol, "H", ClsRevitUtil.CovertToAPI(topPlateT));

                    t.Commit();

                    tmptopPlateSym = duplicatedSymbol;
                }

                using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                {
                    t.Start();

                    ClsRevitUtil.SetTypeParameter(tmpsym, "トッププレート種類", tmptopPlateSym.Id);

                    t.Commit();
                }
            }

            return true;
        }

        /// <summary>
        /// 継手プレートの PL-◯◯◯X◯◯◯X◯◯◯ から寸法値を抽出する
        /// </summary>
        /// <param name="input"></param>
        /// <param name="part1"></param>
        /// <param name="part2"></param>
        /// <param name="part3"></param>
        /// <remarks></remarks>
        static void SplitString(string input, out double part1, out double part2, out double part3)
        {
            input = input.ToLower();
            // 正規表現を使用してサイズ部分を抽出
            Match match = Regex.Match(input, @"pl-(\d+)x(\d+)x(\d+)");
            if (match.Success)
            {
                double.TryParse(match.Groups[1].Value, out part1);
                double.TryParse(match.Groups[2].Value, out part2);
                double.TryParse(match.Groups[3].Value, out part3);
            }
            else
            {
                part1 = part2 = part3 = 0.0;
            }
        }

        /// <summary>
        /// 杭のファミリシンボルをコピーする際に使用する一意の名前を取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="familyName"></param>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        private string GetUniqueSymbolName(Document doc, string familyName, string categoryName)
        {
            int suffix = 1;
            string baseName = categoryName + "_";

            // すでに存在している同じ名前のファミリシンボルを検索
            var existingSymbols = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>()
                .Where(s => s.Family.Name == familyName && s.Name.StartsWith(baseName));

            // 重複している名前があれば、最大のサフィックスを取得
            if (existingSymbols.Any())
            {
                suffix = existingSymbols
                    .Select(s => int.TryParse(s.Name.Substring(baseName.Length), out int num) ? num : 0)
                    .Max() + 1;
            }

            //000を付ける
            string hundredsSuffix = "000";
            if (suffix < 10)
                hundredsSuffix = "00" + suffix.ToString();
            else if (10 <= suffix || suffix < 100)
                hundredsSuffix = "0" + suffix.ToString();
            else if (100 <= suffix || suffix < 1000)
                hundredsSuffix = suffix.ToString();
            else
                hundredsSuffix = "上限";

            // 一意の名前を生成して返す
            return baseName + hundredsSuffix;
        }

        private void GetCustomData(Document doc, ElementId id)
        {
            string tmp = string.Empty;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "作成方法", out tmp);
            m_IsFromGL = tmp == "True" ? true : false;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "GLからの高", out tmp);
            m_HightFromGL = double.Parse(tmp);

            ClsRevitUtil.CustomDataGet<string>(doc, id, "杭タイプ", out tmp);
            switch (tmp)
            {
                case "Sanbashi":
                    m_PileType = PileType.Sanbashi;
                    break;
                case "Kenyou":
                    m_PileType = PileType.Kenyou;
                    break;
                case "DanmenHenka":
                    m_PileType = PileType.DanmenHenka;
                    break;
                case "TC":
                    m_PileType = PileType.TC;
                    break;
                default:
                    return;
            }


            ClsRevitUtil.CustomDataGet<string>(doc, id, "配置方法", out tmp);
            switch (tmp)
            {
                case "OnePoint":
                    m_CreateType = CreateType.OnePoint;
                    break;
                case "Intersection":
                    m_CreateType = CreateType.Intersection;
                    break;
                default:
                    return;
            }

            ClsRevitUtil.CustomDataGet<string>(doc, id, "鋼材タイプ", out tmp);
            m_KouzaiType = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "サイズ1", out tmp);
            m_KouzaiSize = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "サイズ2", out tmp);
            m_KouzaiSizeKoudaikui = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "サイズ3", out tmp);
            m_KouzaiSizeDanmenHenkaKui = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "杭全長", out tmp);
            m_PileTotalLength = double.Parse(tmp);

            ClsRevitUtil.CustomDataGet<string>(doc, id, "杭全長上", out tmp);
            m_PileTotalLength_Top = double.Parse(tmp);

            ClsRevitUtil.CustomDataGet<string>(doc, id, "杭全長下", out tmp);
            m_PileTotalLength_Bottom = double.Parse(tmp);

            ClsRevitUtil.CustomDataGet<string>(doc, id, "配置角度", out tmp);
            m_PileAngle = double.Parse(tmp);

            ClsRevitUtil.CustomDataGet<string>(doc, id, "残置", out tmp);
            m_Zanti = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "残置長さ", out tmp);
            m_ZantiLength = double.Parse(tmp);

            ClsRevitUtil.CustomDataGet<string>(doc, id, "TP取付", out tmp);
            m_UseTopPlate = tmp == "True" ? true : false;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "TPサイズ", out tmp);
            m_TopPlateSize = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "EP厚さ", out tmp);
            m_EndPlateThickness = double.Parse(tmp);

            // 継手
            ClsRevitUtil.CustomDataGet<string>(doc, id, "箇所数1", out tmp);
            m_TsugiteCount = int.Parse(tmp);

            ClsRevitUtil.CustomDataGet<string>(doc, id, "箇所数2", out tmp);
            m_TsugiteCount2 = int.Parse(tmp);

            ClsRevitUtil.CustomDataGet<string>(doc, id, "固定方法1", out tmp);
            //m_FixingType = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "固定方法2", out tmp);
            //m_FixingType2 = tmp;

            // 継手プレート
            ClsRevitUtil.CustomDataGet<string>(doc, id, "寸法FO1", out tmp);
            m_TsugitePlateSize_Out = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "寸法FO2", out tmp);
            m_TsugitePlateSize_Out2 = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "枚数FO1", out tmp);
            m_TsugitePlateQuantity_Out = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "枚数FO2", out tmp);
            m_TsugitePlateQuantity_Out2 = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "寸法FI1", out tmp);
            m_TsugitePlateSize_In = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "寸法FI2", out tmp);
            m_TsugitePlateSize_In2 = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "枚数FI1", out tmp);
            m_TsugitePlateQuantity_In = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "枚数FI2", out tmp);
            m_TsugitePlateQuantity_In2 = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "寸法W1_1", out tmp);
            m_TsugitePlateSize_Web1 = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "寸法W1_2", out tmp);
            m_TsugitePlateSize_Web1_2 = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "枚数W1_1", out tmp);
            m_TsugitePlateQuantity_Web1 = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "枚数W1_2", out tmp);
            m_TsugitePlateQuantity_Web1_2 = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "寸法W2_1", out tmp);
            m_TsugitePlateSize_Web2 = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "寸法W2_2", out tmp);
            m_TsugitePlateSize_Web2_2 = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "枚数W2_1", out tmp);
            m_TsugitePlateQuantity_Web2 = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "枚数W2_2", out tmp);
            m_TsugitePlateQuantity_Web2_2 = tmp;

            // 継手ボルト
            ClsRevitUtil.CustomDataGet<string>(doc, id, "ボサF1", out tmp);
            m_TsugiteBoltSize_Flange = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "ボサF2", out tmp);
            m_TsugiteBoltSize_Flange2 = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "ボ数F1", out tmp);
            m_TsugiteBoltQuantity_Flange = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "ボ数F2", out tmp);
            m_TsugiteBoltQuantity_Flange2 = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "ボサW1", out tmp);
            m_TsugiteBoltSize_Web = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "ボサW2", out tmp);
            m_TsugiteBoltSize_Web2 = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "ボ数W1", out tmp);
            m_TsugiteBoltQuantity_Web = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "ボ数W2", out tmp);
            m_TsugiteBoltQuantity_Web2 = tmp;
        }

        private void SetCustomData(Document doc, ElementId id)
        {
            // 杭
            ClsRevitUtil.CustomDataSet<string>(doc, id, "作成方法", m_IsFromGL.ToString());
            ClsRevitUtil.CustomDataSet<string>(doc, id, "GLからの高", m_HightFromGL.ToString());
            ClsRevitUtil.CustomDataSet<string>(doc, id, "杭タイプ", m_PileType.ToString());
            ClsRevitUtil.CustomDataSet<string>(doc, id, "配置方法", m_CreateType.ToString());
            ClsRevitUtil.CustomDataSet<string>(doc, id, "鋼材タイプ", m_KouzaiType);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "サイズ1", m_KouzaiSize);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "サイズ2", m_KouzaiSizeKoudaikui);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "サイズ3", m_KouzaiSizeDanmenHenkaKui);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "杭全長", m_PileTotalLength.ToString());
            ClsRevitUtil.CustomDataSet<string>(doc, id, "杭全長上", m_PileTotalLength_Top.ToString());
            ClsRevitUtil.CustomDataSet<string>(doc, id, "杭全長下", m_PileTotalLength_Bottom.ToString());
            ClsRevitUtil.CustomDataSet<string>(doc, id, "配置角度", m_PileAngle.ToString());
            ClsRevitUtil.CustomDataSet<string>(doc, id, "残置", m_Zanti);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "残置長さ", m_ZantiLength.ToString());
            ClsRevitUtil.CustomDataSet<string>(doc, id, "TP取付", m_UseTopPlate.ToString());
            ClsRevitUtil.CustomDataSet<string>(doc, id, "TPサイズ", m_TopPlateSize);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "EP厚さ", m_EndPlateThickness.ToString());

            // 継手
            ClsRevitUtil.CustomDataSet<string>(doc, id, "箇所数1", m_TsugiteCount.ToString());
            ClsRevitUtil.CustomDataSet<string>(doc, id, "箇所数2", m_TsugiteCount2.ToString());
            ClsRevitUtil.CustomDataSet<string>(doc, id, "固定方法1", m_FixingType.ToString());
            ClsRevitUtil.CustomDataSet<string>(doc, id, "固定方法2", m_FixingType2.ToString());
            //ClsRevitUtil.CustomDataSet<string>(doc, id, "杭長さ1", m_PileLengthList.ToString());     // List型は不可 タイプパラメータが保持しているのでそちらから取得
            //ClsRevitUtil.CustomDataSet<string>(doc, id, "杭長さ2", m_PileLengthList2.ToString());    // List型は不可 タイプパラメータが保持しているのでそちらから取得
            ClsRevitUtil.CustomDataSet<string>(doc, id, "寸法FO1", m_TsugitePlateSize_Out);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "寸法FO2", m_TsugitePlateSize_Out2);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "枚数FO1", m_TsugitePlateQuantity_Out);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "枚数FO2", m_TsugitePlateQuantity_Out2);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "寸法FI1", m_TsugitePlateSize_In);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "寸法FI2", m_TsugitePlateSize_In2);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "枚数FI1", m_TsugitePlateQuantity_In);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "枚数FI2", m_TsugitePlateQuantity_In2);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "寸法W1_1", m_TsugitePlateSize_Web1);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "寸法W1_2", m_TsugitePlateSize_Web1_2);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "枚数W1_1", m_TsugitePlateQuantity_Web1);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "枚数W1_2", m_TsugitePlateQuantity_Web1_2);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "寸法W2_1", m_TsugitePlateSize_Web2);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "寸法W2_2", m_TsugitePlateSize_Web2_2);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "枚数W2_1", m_TsugitePlateQuantity_Web2);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "枚数W2_2", m_TsugitePlateQuantity_Web2_2);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "ボサF1", m_TsugiteBoltSize_Flange);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "ボサF2", m_TsugiteBoltSize_Flange2);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "ボ数F1", m_TsugiteBoltQuantity_Flange);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "ボ数F2", m_TsugiteBoltQuantity_Flange2);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "ボサW1", m_TsugiteBoltSize_Web);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "ボサW2", m_TsugiteBoltSize_Web2);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "ボ数W1", m_TsugiteBoltQuantity_Web);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "ボ数W2", m_TsugiteBoltQuantity_Web2);


            ClsYMSUtil.SetKotei(doc, id, m_FixingType == FixingType.Bolt ? Master.Kotei.Bolt : Master.Kotei.Yousetsu);
            ClsYMSUtil.SetKotei(doc, id, m_FixingType2 == FixingType.Bolt ? Master.Kotei.Bolt : Master.Kotei.Yousetsu);
            ClsYMSUtil.SetBoltFlange(doc, id, m_TsugiteBoltSize_Flange, ClsCommonUtils.ChangeStrToInt(m_TsugiteBoltQuantity_Flange));
            ClsYMSUtil.SetBoltFlange(doc, id, m_TsugiteBoltSize_Flange2, ClsCommonUtils.ChangeStrToInt(m_TsugiteBoltQuantity_Flange2));
            ClsYMSUtil.SetBoltWeb(doc, id, m_TsugiteBoltSize_Web, ClsCommonUtils.ChangeStrToInt(m_TsugiteBoltQuantity_Web));
            ClsYMSUtil.SetBoltWeb(doc, id, m_TsugiteBoltSize_Web2, ClsCommonUtils.ChangeStrToInt(m_TsugiteBoltQuantity_Web2));
        }

        public void SetParameter(Document doc, ElementId kuiId)
        {
            FamilyInstance inst = doc.GetElement(kuiId) as FamilyInstance;
            if (inst == null)
            {
                return;
            }

            GetCustomData(doc, kuiId);

            if (m_PileType == PileType.DanmenHenka)
            {
                m_PileLengthList = new List<int>();
                m_TsugiteCount = (int)ClsRevitUtil.GetTypeParameter(inst.Symbol, "上杭ジョイント数");
                for (int i = 0; i < m_TsugiteCount; i++)
                {
                    m_PileLengthList[i] = (int)ClsRevitUtil.GetTypeParameter(inst.Symbol, "上杭" + (i + 1));
                }
                m_PileTotalLength_Top = (int)ClsRevitUtil.GetTypeParameter(inst.Symbol, "上杭長さ");

                m_PileLengthList2 = new List<int>();
                m_TsugiteCount2 = (int)ClsRevitUtil.GetTypeParameter(inst.Symbol, "下杭ジョイント数");
                for (int i = 0; i < m_TsugiteCount2; i++)
                {
                    m_PileLengthList2[i] = (int)ClsRevitUtil.GetTypeParameter(inst.Symbol, "下杭" + (i + 1));
                }
                m_PileTotalLength_Bottom = (int)ClsRevitUtil.GetTypeParameter(inst.Symbol, "下杭長さ");

                m_EndPlateThickness = (int)ClsRevitUtil.GetTypeParameter(inst.Symbol, "エンドプレート厚さ");
            }
            else
            {
                m_PileLengthList = new List<int>();
                m_TsugiteCount = ClsRevitUtil.GetTypeParameterInt(inst.Symbol, "ジョイント数");
                for (int i = 0; i < m_TsugiteCount + 1; i++)
                {
                    m_PileLengthList.Add((int)ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(inst.Symbol, "杭" + (i + 1))));
                }
            }
        }

        public static bool PickObjects(UIDocument uidoc, PileType pileType, ref List<ElementId> ids, string message = "杭")
        {
            string type = string.Empty;
            switch (pileType)
            {
                case PileType.Sanbashi:
                    type = "支持杭";
                    break;
                case PileType.Kenyou:
                    type = "兼用杭";
                    break;
                case PileType.DanmenHenka:
                    type = "断面変化杭";
                    break;
                case PileType.TC:
                    type = "TC杭";
                    break;
                default:
                    break;
            }

            return ClsRevitUtil.PickObjectsPartFilter(uidoc, message + "を選択してください", type, ref ids);
        }

        public static List<ElementId> GetAllKuiList(Document doc)
        {
            List<ElementId> kuiIdList = new List<ElementId>();

            kuiIdList.AddRange(ClsRevitUtil.GetSelectCreatedFamilyInstanceList(doc, "支持杭", true));
            kuiIdList.AddRange(ClsRevitUtil.GetSelectCreatedFamilyInstanceList(doc, "兼用杭_棚杭、支持杭", true));
            kuiIdList.AddRange(ClsRevitUtil.GetSelectCreatedFamilyInstanceList(doc, "断面変化杭", true));
            kuiIdList.AddRange(ClsRevitUtil.GetSelectCreatedFamilyInstanceList(doc, "TC杭", true));

            return kuiIdList;
        }

        private static string[] FindFiles(string folderPath, string searchPattern)
        {
            try
            {
                // フォルダ内の指定されたパターンに一致するファイルを検索
                return Directory.GetFiles(folderPath, searchPattern, SearchOption.AllDirectories);
            }
            catch (Exception ex)
            {
                Console.WriteLine("エラーが発生しました: " + ex.Message);
                return new string[0]; // エラー時は空の配列を返す
            }
        }

    }
}