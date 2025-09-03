using Autodesk.Revit.DB;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static YMS.Parts.ClsSanbashiKui;
using System.Windows.Forms;
using Autodesk.Revit.UI;
using System.Net;
using Autodesk.Revit.UI.Selection;

namespace YMS.Parts
{
    public class ClsTanakui
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

        public enum CreatePosition
        {
            LeftTop,    // 左上
            RightTop,   // 右上
            LeftBottom, // 左下
            RightBottom // 右下
        }

        /// <summary>
        /// 固定方法
        /// </summary>
        public enum FixingType
        {
            Welding,    //溶接
            Bolt,       //ボルト
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
        /// 配置方法
        /// </summary>
        public CreateType m_CreateType { get; set; }

        /// <summary>
        /// 作成位置
        /// </summary>
        public CreatePosition m_CreatePosition { get; set; }

        /// <summary>
        /// ずれ量(X)
        /// </summary>
        public double m_ZureryouX { get; set; }

        /// <summary>
        /// ずれ量(Y)
        /// </summary>
        public double m_ZureryouY { get; set; }

        /// <summary>
        /// 鋼材タイプ
        /// </summary>
        public string m_KouzaiType { get; set; }

        /// <summary>
        /// 鋼材サイズ
        /// </summary>
        public string m_KouzaiSize { get; set; }

        /// <summary>
        /// 杭の全長
        /// </summary>
        public double m_PileTotalLength { get; set; }

        /// <summary>
        /// 杭の配置角度
        /// </summary>
        public double m_PileHaichiAngle { get; set; }

        /// <summary>
        /// 残置
        /// </summary>
        public string m_Zanti { get; set; }

        /// <summary>
        /// 残置長さ
        /// </summary>
        public double m_ZantiLength { get; set; }

        /// <summary>
        /// 切梁ブラケットも同時に作成するか？
        /// </summary>
        public bool m_CreateKiribariBracket { get; set; }

        /// <summary>
        /// 切梁ブラケットサイズを自動で選択するか？
        /// </summary>
        public bool m_KiribariBracketSizeIsAuto { get; set; }

        /// <summary>
        /// 切梁ブラケットサイズ
        /// </summary>
        public string m_KiribariBracketSize { get; set; }

        /// <summary>
        /// プレロードガイド材も同時に作成するか？
        /// </summary>
        public bool m_CreatePreloadGuide { get; set; }

        /// <summary>
        /// 切梁押えブラケットも同時に作成するか？
        /// </summary>
        public bool m_CreateKiribariOsaeBracket { get; set; }

        /// <summary>
        /// 切梁押えブラケットサイズを自動で選択するか？
        /// </summary>
        public bool m_KiribariOsaeBracketSizeIsAuto { get; set; }

        /// <summary>
        /// 切梁押えブラケットサイズ
        /// </summary>
        public string m_KiribariOsaeBracketSize { get; set; }

        /// <summary>
        /// ガイド材も同時に作成するか？
        /// </summary>
        public bool m_CreateGuide { get; set; }

        // ---継手関連---

        /// <summary>
        /// 継手箇所数
        /// </summary>
        public int m_TsugiteCount { get; set; }

        /// <summary>
        /// 継手固定方法
        /// </summary>
        public FixingType m_FixingType { get; set; }

        /// <summary>
        /// 杭長さ
        /// </summary>
        public List<int> m_PileLengthList { get; set; }

        /// <summary>
        /// 継手プレートサイズ フランジ 外側
        /// </summary>
        public string m_TsugitePlateSize_Out { get; set; }

        /// <summary>
        /// 継手プレート枚数 フランジ 外側
        /// </summary>
        public string m_TsugitePlateQuantity_Out { get; set; }

        /// <summary>
        /// 継手プレートサイズ フランジ 内側
        /// </summary>
        public string m_TsugitePlateSize_In { get; set; }

        /// <summary>
        /// 継手プレート枚数 フランジ 内側
        /// </summary>
        public string m_TsugitePlateQuantity_In { get; set; }

        /// <summary>
        /// 継手プレートサイズ ウェブ 1
        /// </summary>
        public string m_TsugitePlateSize_Web1 { get; set; }

        /// <summary>
        /// 継手プレート枚数 ウェブ 1
        /// </summary>
        public string m_TsugitePlateQuantity_Web1 { get; set; }

        /// <summary>
        /// 継手プレートサイズ ウェブ 2
        /// </summary>
        public string m_TsugitePlateSize_Web2 { get; set; }

        /// <summary>
        /// 継手プレート枚数 ウェブ 2
        /// </summary>
        public string m_TsugitePlateQuantity_Web2 { get; set; }

        /// <summary>
        /// 継手ボルトサイズ フランジ
        /// </summary>
        public string m_TsugiteBoltSize_Flange { get; set; }

        /// <summary>
        /// 継手ボルト本数 フランジ
        /// </summary>
        public string m_TsugiteBoltQuantity_Flange { get; set; }

        /// <summary>
        /// 継手ボルトサイズ ウェブ
        /// </summary>
        public string m_TsugiteBoltSize_Web { get; set; }

        /// <summary>
        /// 継手ボルト本数 ウェブ
        /// </summary>
        public string m_TsugiteBoltQuantity_Web { get; set; }

        // 連動作成用

        /// <summary>
        /// 作成した杭のId
        /// </summary>
        public ElementId m_CreateId { get; set; }

        #endregion

        public bool CreateTanaKui(Document doc, XYZ insertPoint, ElementId levelId, bool renzokushori, ref string uniqueName)
        {
            // ファミリの取得処理
            string symbolFolpath = ClsZumenInfo.GetYMSFolder();
            string familyPath = Master.ClsHBeamCsv.GetPileFamilyPath(m_KouzaiSize);
            string familyName = ClsRevitUtil.GetFamilyName(familyPath);
            string typeName = "棚杭・中間杭";

            //#31788

            //if (!ClsRevitUtil.RoadFamilyData(doc, familyPath, familyName, out Family family))
            //{
            //    return false;
            //}

            //FamilySymbol sym = (ClsRevitUtil.GetFamilySymbol(doc, familyName, categoryName));
            //if (sym == null)
            //{
            //    MessageBox.Show("ファミリシンボルの取得に失敗しました");
            //    return false;
            //}

            if (!ClsRevitUtil.LoadFamilySymbolData(doc, familyPath, typeName, out FamilySymbol sym))
            {
                return false;
            }

            if (m_CreateType == CreateType.Intersection)
            {
                // 作成位置の算出
                XYZ offsetPoint = XYZ.Zero;
                double zureX = ClsRevitUtil.CovertToAPI(m_ZureryouX);
                double zureY = ClsRevitUtil.CovertToAPI(m_ZureryouY);
                switch (m_CreatePosition)
                {
                    case CreatePosition.LeftTop:
                        offsetPoint = new XYZ(insertPoint.X + -zureX, insertPoint.Y + zureY, insertPoint.Z);
                        break;
                    case CreatePosition.RightTop:
                        offsetPoint = new XYZ(insertPoint.X + zureX, insertPoint.Y + zureY, insertPoint.Z);
                        break;
                    case CreatePosition.LeftBottom:
                        offsetPoint = new XYZ(insertPoint.X + -zureX, insertPoint.Y + -zureY, insertPoint.Z);
                        break;
                    case CreatePosition.RightBottom:
                        offsetPoint = new XYZ(insertPoint.X + zureX, insertPoint.Y + -zureY, insertPoint.Z);
                        break;
                    default:
                        break;
                }
                insertPoint = offsetPoint;
            }

            // インスタンスの作成処理
            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                t.Start();

                // 一意の名前を生成
                if (uniqueName == string.Empty)
                    uniqueName = GetUniqueSymbolName(doc, familyName, typeName);

                FamilySymbol duplicatedSymbol = sym;

                // インスタンスの作成
                ElementId createdId = ClsRevitUtil.Create(doc, insertPoint, levelId, duplicatedSymbol);
                m_CreateId = createdId;

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
                double radians = m_PileHaichiAngle * (Math.PI / 180.0);
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

                ClsRevitUtil.SetTypeParameter(duplicatedSymbol, "ジョイント数", m_TsugiteCount);
                for (int i = 0; i < m_TsugiteCount + 1; i++)
                {
                    ClsRevitUtil.SetTypeParameter(duplicatedSymbol, "杭" + (i + 1), ClsRevitUtil.CovertToAPI(m_PileLengthList[i]));
                }

                // インスタンスに拡張データを付与
                SetCustomData(doc, createdId);

                t.Commit();
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

            ClsRevitUtil.CustomDataGet<string>(doc, id, "作成位置", out tmp);
            switch (tmp)
            {
                case "LeftTop":
                    m_CreatePosition = CreatePosition.LeftTop;
                    break;
                case "LeftBottom":
                    m_CreatePosition = CreatePosition.LeftBottom;
                    break;
                case "RightTop":
                    m_CreatePosition = CreatePosition.RightTop;
                    break;
                case "RightBottom":
                    m_CreatePosition = CreatePosition.RightBottom;
                    break;

                default:
                    return;
            }

            ClsRevitUtil.CustomDataGet<string>(doc, id, "ずれ量X", out tmp);
            m_ZureryouX = double.Parse(tmp);

            ClsRevitUtil.CustomDataGet<string>(doc, id, "ずれ量Y", out tmp);
            m_ZureryouY = double.Parse(tmp);

            ClsRevitUtil.CustomDataGet<string>(doc, id, "鋼材タイプ", out tmp);
            m_KouzaiType = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "鋼材サイズ", out tmp);
            m_KouzaiSize = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "杭全長", out tmp);
            m_PileTotalLength = double.Parse(tmp);

            ClsRevitUtil.CustomDataGet<string>(doc, id, "配置角度", out tmp);
            m_PileHaichiAngle = double.Parse(tmp);

            ClsRevitUtil.CustomDataGet<string>(doc, id, "残置", out tmp);
            m_Zanti = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "残置長さ", out tmp);
            m_ZantiLength = double.Parse(tmp);

            ClsRevitUtil.CustomDataGet<string>(doc, id, "切梁ブラ", out tmp);
            m_CreateKiribariBracket = tmp == "True" ? true : false;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "プレロ", out tmp);
            m_CreatePreloadGuide = tmp == "True" ? true : false;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "切梁押ブラ", out tmp);
            m_CreateKiribariOsaeBracket = tmp == "True" ? true : false;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "ガイド", out tmp);
            m_CreateGuide = tmp == "True" ? true : false;

            // 継手
            ClsRevitUtil.CustomDataGet<string>(doc, id, "箇所数", out tmp);
            m_TsugiteCount = int.Parse(tmp);

            ClsRevitUtil.CustomDataGet<string>(doc, id, "固定方法", out tmp);
            switch (tmp)
            {
                case "Bolt":
                    m_FixingType = FixingType.Bolt;
                    break;
                case "Welding":
                    m_FixingType = FixingType.Welding;
                    break;
                default:
                    return;
            }

            // 継手プレート
            ClsRevitUtil.CustomDataGet<string>(doc, id, "寸法FO1", out tmp);
            m_TsugitePlateSize_Out = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "枚数FO1", out tmp);
            m_TsugitePlateQuantity_Out = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "寸法FI1", out tmp);
            m_TsugitePlateSize_In = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "枚数FI1", out tmp);
            m_TsugitePlateQuantity_In = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "寸法W1_1", out tmp);
            m_TsugitePlateSize_Web1 = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "枚数W1_1", out tmp);
            m_TsugitePlateQuantity_Web1 = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "寸法W2_1", out tmp);
            m_TsugitePlateSize_Web2 = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "枚数W2_1", out tmp);
            m_TsugitePlateQuantity_Web2 = tmp;

            // 継手ボルト
            ClsRevitUtil.CustomDataGet<string>(doc, id, "ボサF1", out tmp);
            m_TsugiteBoltSize_Flange = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "ボ数F1", out tmp);
            m_TsugiteBoltQuantity_Flange = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "ボサW1", out tmp);
            m_TsugiteBoltSize_Web = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "ボ数W1", out tmp);
            m_TsugiteBoltQuantity_Web = tmp;
        }

        private void SetCustomData(Document doc, ElementId id)
        {
            // 杭
            ClsRevitUtil.CustomDataSet<string>(doc, id, "作成方法", m_IsFromGL.ToString());
            ClsRevitUtil.CustomDataSet<string>(doc, id, "GLからの高", m_HightFromGL.ToString());
            ClsRevitUtil.CustomDataSet<string>(doc, id, "配置方法", m_CreateType.ToString());
            ClsRevitUtil.CustomDataSet<string>(doc, id, "作成位置", m_CreatePosition.ToString());
            ClsRevitUtil.CustomDataSet<string>(doc, id, "ずれ量X", m_ZureryouX.ToString());
            ClsRevitUtil.CustomDataSet<string>(doc, id, "ずれ量Y", m_ZureryouY.ToString());
            ClsRevitUtil.CustomDataSet<string>(doc, id, "鋼材タイプ", m_KouzaiType);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "鋼材サイズ", m_KouzaiSize);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "杭全長", m_PileTotalLength.ToString());
            ClsRevitUtil.CustomDataSet<string>(doc, id, "配置角度", m_PileHaichiAngle.ToString());
            ClsRevitUtil.CustomDataSet<string>(doc, id, "残置", m_Zanti);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "残置長さ", m_ZantiLength.ToString());
            ClsRevitUtil.CustomDataSet<string>(doc, id, "切梁ブラ", m_CreateKiribariBracket.ToString());
            ClsRevitUtil.CustomDataSet<string>(doc, id, "プレロ", m_CreatePreloadGuide.ToString());
            ClsRevitUtil.CustomDataSet<string>(doc, id, "切梁押ブラ", m_CreateKiribariOsaeBracket.ToString());
            ClsRevitUtil.CustomDataSet<string>(doc, id, "ガイド", m_CreateGuide.ToString());


            // 継手
            ClsRevitUtil.CustomDataSet<string>(doc, id, "箇所数", m_TsugiteCount.ToString());
            ClsRevitUtil.CustomDataSet<string>(doc, id, "固定方法", m_FixingType.ToString());
            ClsRevitUtil.CustomDataSet<string>(doc, id, "寸法FO1", m_TsugitePlateSize_Out);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "枚数FO1", m_TsugitePlateQuantity_Out);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "寸法FI1", m_TsugitePlateSize_In);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "枚数FI1", m_TsugitePlateQuantity_In);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "寸法W1_1", m_TsugitePlateSize_Web1);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "枚数W1_1", m_TsugitePlateQuantity_Web1);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "寸法W2_1", m_TsugitePlateSize_Web2);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "枚数W2_1", m_TsugitePlateQuantity_Web2);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "ボサF1", m_TsugiteBoltSize_Flange);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "ボ数F1", m_TsugiteBoltQuantity_Flange);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "ボサW1", m_TsugiteBoltSize_Web);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "ボ数W1", m_TsugiteBoltQuantity_Web);

            ClsYMSUtil.SetKotei(doc, id, m_FixingType == FixingType.Bolt ? Master.Kotei.Bolt : Master.Kotei.Yousetsu);
            ClsYMSUtil.SetBoltFlange(doc, id, m_TsugiteBoltSize_Flange, ClsCommonUtils.ChangeStrToInt(m_TsugiteBoltQuantity_Flange));
            ClsYMSUtil.SetBoltWeb(doc, id, m_TsugiteBoltSize_Web, ClsCommonUtils.ChangeStrToInt(m_TsugiteBoltQuantity_Web));
        }

        public void SetParameter(Document doc, ElementId id)
        {
            FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
            if (inst == null)
            {
                return;
            }

            GetCustomData(doc, id);

            //杭長さ
            m_PileLengthList = new List<int>();
            for (int i = 0; i < m_TsugiteCount + 1; i++)
            {
                m_PileLengthList.Add((int)ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(inst.Symbol, "杭" + (i + 1))));
            }
        }

        public static bool PickObjects(UIDocument uidoc, ref List<ElementId> ids, string message = "棚杭")
        {
            return ClsRevitUtil.PickObjectsPartFilter(uidoc, message + "を選択してください", "棚杭", ref ids);
        }

        public static List<ElementId> GetAllKuiList(Document doc)
        {
            List<ElementId> kuiIdList = new List<ElementId>();

            kuiIdList.AddRange(ClsRevitUtil.GetSelectCreatedFamilyInstanceList(doc, "棚杭・中間杭", true));

            return kuiIdList;
        }

        /// <summary>
        ///杭のサイズからフランジを伸ばした判定線を作成するメソッド
        /// </summary>
        /// <param name="pileInstance">杭のインスタンス</param>
        /// <param name="offset">mm単位</param>
        /// <returns></returns>
        public static Curve[] CreateBoundaryLinesFlange(FamilyInstance pileInstance, double offset, double angle = 0.0)
        {
            // インスタンスの向きを取得
            XYZ facingDirection = pileInstance.FacingOrientation;

            // サイズデータを抽出
            string selectedSize = pileInstance.Symbol.FamilyName;
            SizeData sizeData = ExtractSize(selectedSize);
            if (sizeData == null)
            {
                // サイズデータが不正な場合はnullを返す
                return null;
            }

            // 杭の中心座標を取得
            XYZ centerPoint = pileInstance.GetTotalTransform().Origin;

            // H型の1画目と3画目の座標を計算
            XYZ startPoint1 = new XYZ(centerPoint.X - ClsRevitUtil.CovertToAPI(sizeData.W) / 2, centerPoint.Y + ClsRevitUtil.CovertToAPI(sizeData.H) / 2, 0);
            XYZ endPoint1 = new XYZ(centerPoint.X - ClsRevitUtil.CovertToAPI(sizeData.W) / 2, centerPoint.Y - ClsRevitUtil.CovertToAPI(sizeData.H) / 2, 0);
            XYZ startPoint3 = new XYZ(centerPoint.X + ClsRevitUtil.CovertToAPI(sizeData.W) / 2, centerPoint.Y + ClsRevitUtil.CovertToAPI(sizeData.H) / 2, 0);
            XYZ endPoint3 = new XYZ(centerPoint.X + ClsRevitUtil.CovertToAPI(sizeData.W) / 2, centerPoint.Y - ClsRevitUtil.CovertToAPI(sizeData.H) / 2, 0);

            // 方向ベクトルを取得
            XYZ direction1 = (endPoint1 - startPoint1).Normalize();
            XYZ direction3 = (endPoint3 - startPoint3).Normalize();

            // 方向ベクトルにオフセットを適用して延長
            offset = ClsRevitUtil.CovertToAPI(offset);
            startPoint1 = startPoint1 - direction1 * offset;
            endPoint1 = endPoint1 + direction1 * offset;
            startPoint3 = startPoint3 - direction3 * offset;
            endPoint3 = endPoint3 + direction3 * offset;

            // 杭の向きに合わせて回転
            double angle1 = (Math.PI / 2) + angle;
            double angle3 = (Math.PI / 2) + angle;
            startPoint1 = RotatePointAroundZAxis(startPoint1, angle1, centerPoint);
            endPoint1 = RotatePointAroundZAxis(endPoint1, angle1, centerPoint);
            startPoint3 = RotatePointAroundZAxis(startPoint3, angle3, centerPoint);
            endPoint3 = RotatePointAroundZAxis(endPoint3, angle3, centerPoint);

            // 1画目と3画目の座標から線分を作成
            Curve[] boundaryLines = new Curve[2];
            boundaryLines[0] = Line.CreateBound(startPoint1, endPoint1);
            boundaryLines[1] = Line.CreateBound(startPoint3, endPoint3);

            return boundaryLines;
        }

        /// <summary>
        /// 杭のサイズからウェブを伸ばした判定線を作成するメソッド
        /// </summary>
        /// <param name="pileInstance"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Curve[] CreateBoundaryLinesWeb(FamilyInstance pileInstance, double offset)
        {
            // インスタンスの向きを取得
            XYZ facingDirection = pileInstance.FacingOrientation;

            // サイズデータを抽出
            string selectedSize = pileInstance.Symbol.FamilyName;
            SizeData sizeData = ExtractSize(selectedSize);
            if (sizeData == null)
            {
                // サイズデータが不正な場合はnullを返す
                return null;
            }

            // 杭の中心座標を取得
            XYZ centerPoint = pileInstance.GetTotalTransform().Origin;
            XYZ ptCenter = new XYZ(centerPoint.X, centerPoint.Y, XYZ.Zero.Z);

            // H型の2画目の座標を計算
            XYZ ptEnd_Front = new XYZ(centerPoint.X - (facingDirection.X * ClsRevitUtil.CovertToAPI(sizeData.W) / 2), centerPoint.Y - (facingDirection.Y * ClsRevitUtil.CovertToAPI(sizeData.W) / 2), XYZ.Zero.Z);
            XYZ ptEnd_Back = new XYZ(centerPoint.X + (facingDirection.X * ClsRevitUtil.CovertToAPI(sizeData.W) / 2), centerPoint.Y + (facingDirection.Y * ClsRevitUtil.CovertToAPI(sizeData.W) / 2), XYZ.Zero.Z);

            // 方向ベクトルを取得
            XYZ direction = (ptEnd_Front - ptCenter).Normalize();

            // 方向ベクトルにオフセットを適用して延長
            offset = ClsRevitUtil.CovertToAPI(offset);
            ptEnd_Front = ptEnd_Front + direction * offset;
            ptEnd_Back = ptEnd_Back - direction * offset;

            // 杭の向きに合わせて回転
            double angle = facingDirection.AngleTo(direction);
            ptEnd_Front = RotatePointAroundZAxis(ptEnd_Front, angle, centerPoint);
            ptEnd_Back = RotatePointAroundZAxis(ptEnd_Back, -angle, centerPoint);

            // 座標から線分を作成
            Curve[] boundaryLines = new Curve[2];
            boundaryLines[0] = Line.CreateBound(ptCenter, ptEnd_Front);
            boundaryLines[1] = Line.CreateBound(ptCenter, ptEnd_Back);

            return boundaryLines;
        }

        private static XYZ RotatePointAroundZAxis(XYZ point, double angle, XYZ center)
        {
            // センターを原点にシフト
            double x1 = point.X - center.X;
            double y1 = point.Y - center.Y;

            // 回転
            double x2 = x1 * Math.Cos(angle) - y1 * Math.Sin(angle);
            double y2 = x1 * Math.Sin(angle) + y1 * Math.Cos(angle);

            // シフトを戻す
            double rotatedX = x2 + center.X;
            double rotatedY = y2 + center.Y;

            return new XYZ(rotatedX, rotatedY, point.Z);
        }

        public static bool GetKuiPosition(XYZ intersectionPoint, XYZ kuiPoint, out CreatePosition position)
        {
            double tolerance = 0.0001; // 許容誤差

            double deltaX = kuiPoint.X - intersectionPoint.X;
            double deltaY = kuiPoint.Y - intersectionPoint.Y;

            if (Math.Abs(deltaX) < tolerance && Math.Abs(deltaY) < tolerance)
            {
                position = CreatePosition.LeftTop; // 同じ座標（左上として扱う）
                return false;
            }
            else if (deltaX > tolerance)
            {
                if (deltaY > tolerance)
                {
                    position = CreatePosition.RightTop; // 右上
                    return true;
                }
                else if (deltaY < -tolerance)
                {
                    position = CreatePosition.RightBottom; // 右下
                    return true;
                }
            }
            else if (deltaX < -tolerance)
            {
                if (deltaY > tolerance)
                {
                    position = CreatePosition.LeftTop; // 左上
                    return true;
                }
                else if (deltaY < -tolerance)
                {
                    position = CreatePosition.LeftBottom; // 左下
                    return true;
                }
            }

            // 判定失敗時はデフォルト値を設定し、false を返す
            position = CreatePosition.LeftTop;
            return false;
        }

        /// <summary>
        /// ユーザーが選択したH形鋼からサイズデータを抽出するメソッド
        /// </summary>
        /// <param name="selectedSize"></param>
        /// <returns></returns>
        private static SizeData ExtractSize(string selectedSize)
        {
            // 正規表現を使用してサイズからデータを抽出
            selectedSize = selectedSize.Replace("杭_", "");
            Regex regex = new Regex(@"H(\d+)x(\d+)x(\d+(\.\d+)?)x(\d+(\.\d+)?)");
            Match match = regex.Match(selectedSize);

            // マッチしなかった場合はnullを返す
            if (!match.Success)
            {
                return null;
            }

            // 抽出したデータをSizeDataオブジェクトに格納して返す
            SizeData sizeData = new SizeData();
            sizeData.H = int.Parse(match.Groups[1].Value);
            sizeData.W = int.Parse(match.Groups[2].Value);
            sizeData.T1 = double.Parse(match.Groups[3].Value);
            sizeData.T2 = double.Parse(match.Groups[5].Value);

            return sizeData;
        }

        // サイズデータを格納するクラス
        private class SizeData
        {
            /// <summary>
            /// 高さ
            /// </summary>
            public int H { get; set; }

            /// <summary>
            /// 幅
            /// </summary>
            public int W { get; set; }

            /// <summary>
            /// 下側の厚さ
            /// </summary>
            public double T1 { get; set; }

            /// <summary>
            /// 上側の厚さ
            /// </summary>
            public double T2 { get; set; }
        }

        public static CreatePosition GetCreatePoition(string position)
        {
            switch(position)
            {
                case "左上":
                    return CreatePosition.LeftTop;
                case "右上":
                    return CreatePosition.RightTop;
                case "左下":
                    return CreatePosition.LeftBottom;
                case "右下":
                    return CreatePosition.RightBottom;
                default:
                    return CreatePosition.LeftTop;
            }
        }


        public struct TanakuiData
        {
            public string Classification { get; set; }     // 分類
            public string Usage { get; set; }              // 用途
            public string SteelType { get; set; }          // 鋼材タイプ
            public string Size { get; set; }               // サイズ
            public string CaseName { get; set; }           // CASE名
            public double Length { get; set; }             // 長さ
            public double Pitch { get; set; }              // ピッチ
            public double EmbedmentLength { get; set; }    // 埋め殺し長さ
            public int JointCount { get; set; }            // ジョイント数
            public int Quantity { get; set; }              // 数量
            public double UnitWeight { get; set; }         // 単位質量
            public double SoilDiameter { get; set; }       // ソイル径
            public double WallLength { get; set; }         // 壁長
            public double WallExtension { get; set; }      // 壁延長
            public string JointSpecification { get; set; } // 継手仕様
            public string Plate1 { get; set; }             // プレート1(F)
            public int Plate1Count { get; set; }           // 枚数１
            public string Plate2 { get; set; }             // プレート2(F)
            public int Plate2Count { get; set; }           // 枚数２
            public string BoltTypeF { get; set; }          // ボルトタイプ(F)
            public int BoltCountF { get; set; }            // ボルト本数(F)
            public string PlateW { get; set; }             // プレート(W)
            public int PlateCountW { get; set; }           // 枚数
            public string BoltTypeW { get; set; }          // ボルトタイプ(W)
            public int BoltCountW { get; set; }            // ボルト本数(W)
            public double BoltDiameter { get; set; }       // ボルト孔径
            public string Remarks { get; set; }            // 備考欄
        }

    }
}
