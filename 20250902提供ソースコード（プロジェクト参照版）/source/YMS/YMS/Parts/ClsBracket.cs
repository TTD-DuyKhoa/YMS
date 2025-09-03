using Autodesk.Revit.DB;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using YMS.Command;
using YMS.Master;
using static YMS.Parts.ClsTanakui;
using Document = Autodesk.Revit.DB.Document;

namespace YMS.Parts
{
    public class ClsBracket
    {
        #region Enum

        public enum CreateType
        {
            Kiribari,       // 切梁ブラケット
            kiribariOsae,   // 切梁押えブラケット
            Haraokoshi,     // 腹起ブラケット
            HaraokoshiOsae, // 腹起押えブラケット
        }
        public enum CreateMode
        {
            Auto,    // 自動
            Manual,  // 手動
            Optional,// 任意
        }

        #endregion

        #region 変数

        public CreateType m_CreateType { get; set; }

        public CreateMode m_CreateMode { get; set; }

        public double m_ProhibitionLength { get; set; }

        public string m_BracketSize { get; set; }

        /// <summary>
        /// 作成したブラケットのId
        /// </summary>
        public List<ElementId> m_CreateIds { get; set; } = new List<ElementId>();

        /// <summary>
        /// 対象の杭の角度
        /// </summary>
        public double m_TargetKuiAngle { get; set; }

        /// <summary>
        /// 対象の杭の位置
        /// </summary>
        public ClsTanakui.CreatePosition m_TargetKuiCreatePosition { get; set; }

        /// <summary>
        /// 対象の杭の作成方法
        /// </summary>
        public ClsTanakui.CreateType m_TargetKuiCreateType { get; set; }

        /// <summary>
        /// ブラケット作成時の切梁ベースID
        /// </summary>
        public List<ElementId> m_TargetKiribariBaseIds { get; set; } = new List<ElementId>();

        #endregion

        public bool CreateKiribariBracket(Document doc, ElementId kuiId, bool sizeIsAuto = false)
        {
            if (kuiId == null)
            {
                return false;
            }

            if (!GetTargetKiribariBaseId_Web(doc, kuiId, out List<ElementId> kiribariIds, out List<bool> isFront, out List<XYZ> crossPoints))
            {
                return false;
            }

            for (int i = 0; i < kiribariIds.Count; i++)
            {
                // ファミリの取得処理
                if (sizeIsAuto)
                {
                    m_BracketSize = GetKiribariBracketSizeWithAuto(ClsRevitUtil.GetInstMojiParameter(doc, kiribariIds[i], "鋼材サイズ"));
                }
                string bracketFamilyPath = Master.ClsBracketCsv.GetFamilyPath(m_BracketSize);
                string bracketFamilyName = ClsRevitUtil.GetFamilyName(bracketFamilyPath);
                if (!ClsRevitUtil.LoadFamilyData(doc, bracketFamilyPath, out Family family))
                {
                    return false;
                }
                string bracketFamilyTypeName = "切梁BL";
                FamilySymbol sym = (ClsRevitUtil.GetFamilySymbol(doc, bracketFamilyName, bracketFamilyTypeName));

                FamilyInstance instKui = doc.GetElement(kuiId) as FamilyInstance;
                XYZ pkui = (instKui.Location as LocationPoint).Point;
                double kuiSizeHalf = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou1(instKui.Symbol.FamilyName, 1)) / 2);

                XYZ insertPointSt = XYZ.Zero;
                if (isFront[i])
                {
                    insertPointSt = new XYZ(pkui.X + ((kuiSizeHalf) * instKui.FacingOrientation.X), pkui.Y + ((kuiSizeHalf) * instKui.FacingOrientation.Y), pkui.Z);
                }
                else
                {
                    insertPointSt = new XYZ(pkui.X - ((kuiSizeHalf) * instKui.FacingOrientation.X), pkui.Y - ((kuiSizeHalf) * instKui.FacingOrientation.Y), pkui.Z);
                }

                // 高さの取得
                string levelName = ClsRevitUtil.GetInstMojiParameter(doc, kiribariIds[i], "集計レベル");
                if (levelName == null || levelName == "")
                {
                    levelName = ClsRevitUtil.GetInstMojiParameter(doc, kiribariIds[i], "作業面");
                    levelName = levelName != null ? levelName.Replace("レベル : ", "") : null;
                }
                double elevation = levelName != null ? ClsRevitUtil.GetLevelElevation(doc, levelName) : 0.0;

                double kiribariSize = double.NaN;
                string kiribariSteelSize = ClsRevitUtil.GetInstMojiParameter(doc, kiribariIds[i], "鋼材サイズ");//.Replace("HA", "");
                //kiribariSteelSize = kiribariSteelSize.Replace("SMH", "");
                kiribariSize = ClsRevitUtil.CovertToAPI(Master.ClsYamadomeCsv.GetWidth(kiribariSteelSize));// ClsCommonUtils.ChangeStrToDbl(kiribariSteelSize) * 10);
                if (kiribariSize == 0)
                {
                    FamilyInstance instKiribari = doc.GetElement(kiribariIds[i]) as FamilyInstance;
                    string skiribariSize = ClsYMSUtil.GetSyuzaiSize(instKiribari.Symbol.FamilyName);
                    string numericValue = Regex.Match(skiribariSize, @"\d+").Value;
                    kiribariSize = ClsRevitUtil.CovertToAPI((double.Parse(numericValue) * 10));
                }

                // レベルの取得
                FamilyInstance instHari = doc.GetElement(kiribariIds[i]) as FamilyInstance;
                ElementId levelIdKui = instHari.Host.Id;

                //ブラケット重複チェック
                if (!CheckDuplicateBracket(doc, levelIdKui, insertPointSt, CreateType.Kiribari))
                    continue;

                using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                {
                    t.Start();
                    FailureHandlingOptions failOpt = t.GetFailureHandlingOptions();
                    failOpt.SetFailuresPreprocessor(new WarningSwallower());
                    t.SetFailureHandlingOptions(failOpt);

                    ElementId createdId = ClsRevitUtil.Create(doc, insertPointSt, levelIdKui, sym);
                    m_CreateIds.Add(createdId);
                    m_TargetKiribariBaseIds.Add(kiribariIds[i]);
                    FamilyInstance instBracket = doc.GetElement(createdId) as FamilyInstance;

                    // Z軸を回転軸としてFamilyInstanceを回転させる
                    double kuiAngle = Math.PI * m_TargetKuiAngle / 180.0;
                    ElementTransformUtils.RotateElement(doc, createdId, Line.CreateBound(insertPointSt, insertPointSt + XYZ.BasisZ), kuiAngle);
                    double angle = GetRotateAngle(pkui, insertPointSt, instBracket);
                    ElementTransformUtils.RotateElement(doc, createdId, Line.CreateBound(insertPointSt, insertPointSt + XYZ.BasisZ), angle);

                    ClsYMSUtil.GetDifferenceWithAllBase(doc, kiribariIds[i], out double syuzaiDiff, out double diff2);

                    if (ClsRevitUtil.GetInstMojiParameter(doc, kiribariIds[i], "段") == "上段")
                    {
                        ClsRevitUtil.SetParameter(doc, createdId, "基準レベルからの高さ", (+syuzaiDiff));
                    }
                    else if (ClsRevitUtil.GetInstMojiParameter(doc, kiribariIds[i], "段") == "下段")
                    {
                        ClsRevitUtil.SetParameter(doc, createdId, "基準レベルからの高さ", (-kiribariSize - syuzaiDiff));
                    }
                    else if (ClsRevitUtil.GetInstMojiParameter(doc, kiribariIds[i], "段") == "同段")
                    {
                        ClsRevitUtil.SetParameter(doc, createdId, "基準レベルからの高さ", (-kiribariSize / 2));
                    }
                    else
                    {
                        double offset = ClsRevitUtil.GetParameterDouble(doc, kiribariIds[i], "基準レベルからの高さ");
                        ClsRevitUtil.SetParameter(doc, createdId, "基準レベルからの高さ", (offset + kiribariSize / 2));
                    }

                    t.Commit();
                }
            }

            return true;
        }

        public bool CreateKiribariOsaeBracket(Document doc, ElementId kuiId, bool sizeIsAuto = false)
        {
            if (kuiId == null)
            {
                return false;
            }

            // 対象となる切梁を取得
            if (!GetTargetKiribariBaseId_Web(doc, kuiId, out List<ElementId> kiribariIds_Web, out List<bool> isFront_Web, out List<XYZ> crossPoints_Web))
            {
                return false;
            }

            //if (ClsRevitUtil.GetParameter(doc, kiribariIds_Web, "段") != "下段")
            //{
            //    System.Windows.Forms.MessageBox.Show("対象の切梁(下段)が見つかりません");
            //    return false;
            //}

            if (!GetTargetKiribariBaseId_Flange(doc, kuiId, out List<ElementId> kiribariIds_Flange, out List<bool> isFront_Flange, out List<XYZ> crossPoints_Flange))
            {
                return false;
            }

            //if (ClsRevitUtil.GetParameter(doc, kiribariId_Flange, "段") != "上段")
            //{
            //    System.Windows.Forms.MessageBox.Show("対象の切梁(上段)が見つかりません");
            //    return false;
            //}

            for (int i = 0; i < kiribariIds_Flange.Count(); i++)
            {
                string danF = ClsRevitUtil.GetParameter(doc, kiribariIds_Flange[i], "段");
                for (int k = 0; k < kiribariIds_Web.Count(); k++)
                {
                    string danW = ClsRevitUtil.GetParameter(doc, kiribariIds_Web[k], "段");
                    // 切梁の交点を取得
                    FamilyInstance instKiribari_Web = doc.GetElement(kiribariIds_Web[k]) as FamilyInstance;
                    LocationCurve curveKiribari_web = instKiribari_Web.Location as LocationCurve;
                    FamilyInstance instKiribari_Flange = doc.GetElement(kiribariIds_Flange[i]) as FamilyInstance;
                    LocationCurve curveKiribari_Flange = instKiribari_Flange.Location as LocationCurve;

                    XYZ crossPointKiribari = ClsRevitUtil.GetIntersection(curveKiribari_web.Curve, curveKiribari_Flange.Curve);
                    if (crossPointKiribari == null)
                    {
                        continue;
                    }

                    // ファミリの取得処理
                    if (sizeIsAuto)
                    {
                        m_BracketSize = GetKiribariBracketSizeWithAuto(ClsRevitUtil.GetInstMojiParameter(doc, kiribariIds_Flange[i], "鋼材サイズ"));
                    }
                    string bracketFamilyPath = Master.ClsBracketCsv.GetFamilyPath(m_BracketSize);
                    string bracketFamilyName = ClsRevitUtil.GetFamilyName(bracketFamilyPath);
                    if (!ClsRevitUtil.LoadFamilyData(doc, bracketFamilyPath, out Family family))
                    {
                        return false;
                    }
                    string bracketFamilyTypeName = "切梁押えBL";
                    FamilySymbol sym = (ClsRevitUtil.GetFamilySymbol(doc, bracketFamilyName, bracketFamilyTypeName));

                    // 配置位置の算出
                    FamilyInstance instKui = doc.GetElement(kuiId) as FamilyInstance;
                    XYZ ptkui = instKui.GetTotalTransform().Origin;
                    ptkui = new XYZ(ptkui.X, ptkui.Y, XYZ.Zero.Z);//杭高さも0に統一する
                    double kuiSizeHalf = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou1(instKui.Symbol.FamilyName, 1)) / 2);
                    double bracketWidthHalf = ClsRevitUtil.CovertToAPI(Master.ClsBracketCsv.GetWidth(m_BracketSize) / 2);
                    double kuiAngle = Math.PI * m_TargetKuiAngle / 180.0;

                    XYZ insertPoint = ptkui;
                    XYZ flangePoint = ptkui;
                    XYZ crossPointW = new XYZ(crossPoints_Web[k].X, crossPoints_Web[k].Y, XYZ.Zero.Z);
                    XYZ vecWeb = (crossPointW - ptkui).Normalize();
                    if (m_TargetKuiCreateType != ClsTanakui.CreateType.Intersection)
                    {
                        CreatePosition tmp = new CreatePosition();
                        if (!ClsTanakui.GetKuiPosition(crossPointKiribari, ptkui, out tmp))
                        {
                            return false;
                        }
                        m_TargetKuiCreatePosition = tmp;
                    }

                    switch (m_TargetKuiCreatePosition)
                    {
                        case CreatePosition.LeftTop:
                            if (kuiAngle >= Math.PI / 2)
                            {
                                insertPoint = new XYZ(ptkui.X + ((kuiSizeHalf + bracketWidthHalf) * vecWeb.X), ptkui.Y + ((kuiSizeHalf + bracketWidthHalf) * vecWeb.Y), ptkui.Z);
                                flangePoint = ptkui + ((kuiSizeHalf) * vecWeb);
                            }
                            else
                            {
                                insertPoint = new XYZ(ptkui.X + ((kuiSizeHalf + bracketWidthHalf) * vecWeb.X), ptkui.Y - ((kuiSizeHalf + bracketWidthHalf) * vecWeb.Y), ptkui.Z);
                                flangePoint = ptkui + ((kuiSizeHalf) * vecWeb);
                            }
                            break;
                        case CreatePosition.RightTop:
                            if (kuiAngle >= Math.PI / 2)
                            {
                                insertPoint = new XYZ(ptkui.X - ((kuiSizeHalf + bracketWidthHalf) * vecWeb.X), ptkui.Y - ((kuiSizeHalf + bracketWidthHalf) * vecWeb.Y), ptkui.Z);
                                flangePoint = ptkui - ((kuiSizeHalf) * vecWeb);
                            }
                            else
                            {
                                insertPoint = new XYZ(ptkui.X - ((kuiSizeHalf + bracketWidthHalf) * vecWeb.X), ptkui.Y + ((kuiSizeHalf + bracketWidthHalf) * vecWeb.Y), ptkui.Z);
                                flangePoint = ptkui + ((kuiSizeHalf) * vecWeb);
                            }
                            break;
                        case CreatePosition.LeftBottom:
                            if (kuiAngle >= Math.PI / 2)
                            {
                                insertPoint = new XYZ(ptkui.X - ((kuiSizeHalf + bracketWidthHalf) * vecWeb.X), ptkui.Y + ((kuiSizeHalf + bracketWidthHalf) * vecWeb.Y), ptkui.Z);
                                flangePoint = ptkui + ((kuiSizeHalf) * vecWeb);
                            }
                            else
                            {
                                insertPoint = new XYZ(ptkui.X - ((kuiSizeHalf + bracketWidthHalf) * vecWeb.X), ptkui.Y + ((kuiSizeHalf + bracketWidthHalf) * vecWeb.Y), ptkui.Z);
                                flangePoint = ptkui - ((kuiSizeHalf) * vecWeb);
                            }
                            break;
                        case CreatePosition.RightBottom:
                            if (kuiAngle >= Math.PI / 2)
                            {
                                insertPoint = new XYZ(ptkui.X + ((kuiSizeHalf + bracketWidthHalf) * vecWeb.X), ptkui.Y - ((kuiSizeHalf + bracketWidthHalf) * vecWeb.Y), ptkui.Z);
                                flangePoint = ptkui - ((kuiSizeHalf) * vecWeb);
                            }
                            else
                            {
                                insertPoint = new XYZ(ptkui.X + ((kuiSizeHalf + bracketWidthHalf) * vecWeb.X), ptkui.Y - ((kuiSizeHalf + bracketWidthHalf) * vecWeb.Y), ptkui.Z);
                                flangePoint = ptkui - ((kuiSizeHalf) * vecWeb);
                            }
                            break;
                        default:

                            break;
                    }

                    // 高さの取得
                    string levelName = ClsRevitUtil.GetInstMojiParameter(doc, kiribariIds_Flange[i], "集計レベル");
                    if (levelName == null || levelName == "")
                    {
                        levelName = ClsRevitUtil.GetInstMojiParameter(doc, kiribariIds_Flange[i], "作業面");
                        levelName = levelName != null ? levelName.Replace("レベル : ", "") : null;
                    }
                    double elevation = levelName != null ? ClsRevitUtil.GetLevelElevation(doc, levelName) : 0.0;

                    string kiribariSteelSize = ClsRevitUtil.GetInstMojiParameter(doc, kiribariIds_Flange[i], "鋼材サイズ");//.Replace("HA", "");
                    double kiribariSize = ClsRevitUtil.CovertToAPI(Master.ClsYamadomeCsv.GetWidth(kiribariSteelSize));// ClsCommonUtils.ChangeStrToDbl(kiribariSteelSize) * 10);
                    if (kiribariSize == 0)
                    {
                        FamilyInstance instKiribari = doc.GetElement(kiribariIds_Flange[i]) as FamilyInstance;
                        string skiribariSize = ClsYMSUtil.GetSyuzaiSize(instKiribari.Symbol.FamilyName);
                        string numericValue = Regex.Match(skiribariSize, @"\d+").Value;
                        kiribariSize = ClsRevitUtil.CovertToAPI((double.Parse(numericValue) * 10));
                    }

                    ClsYMSUtil.GetDifferenceWithAllBase(doc, kiribariIds_Flange[i], out double syuzaiDiff, out double diff2);

                    // レベルの取得
                    FamilyInstance instHari = doc.GetElement(kiribariIds_Flange[i]) as FamilyInstance;
                    ElementId levelIdKui = instHari.Host.Id;

                    double offset = 0.0;
                    if (danF == "上段")
                    {
                        offset = kiribariSize + syuzaiDiff;
                    }
                    else if (danF == "下段")
                    {
                        offset = -syuzaiDiff;
                    }
                    else//同段
                    {
                        offset = kiribariSize / 2;
                    }

                    //ブラケット重複チェック
                    if (!CheckDuplicateBracket(doc, levelIdKui, insertPoint, CreateType.kiribariOsae))
                        continue;

                    using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                    {
                        t.Start();
                        //FailureHandlingOptions failOpt = t.GetFailureHandlingOptions();
                        //failOpt.SetFailuresPreprocessor(new WarningSwallower());
                        //t.SetFailureHandlingOptions(failOpt);

                        ElementId createdId = ClsRevitUtil.Create(doc, insertPoint, levelIdKui, sym);
                        m_CreateIds.Add(createdId);
                        m_TargetKiribariBaseIds.Add(kiribariIds_Flange[i]);
                        FamilyInstance instBracket = doc.GetElement(createdId) as FamilyInstance;

                        // 部材を杭の向きに合わせて回転
                        XYZ crossPoint = new XYZ(crossPoints_Flange[i].X, crossPoints_Flange[i].Y, XYZ.Zero.Z);
                        double angle = GetRotateAngle2(flangePoint, crossPoint, instBracket);
                        ElementTransformUtils.RotateElement(doc, createdId, Line.CreateBound(insertPoint, insertPoint + XYZ.BasisZ), angle);

                        ClsRevitUtil.SetParameter(doc, createdId, "ホストからのオフセット", offset);

                        t.Commit();
                    }

                }
            }

            return true;
        }

        public bool CreateHaraokoshiBracket(Document doc, List<ElementId> oyakuiIds, List<ElementId> haraokoshiIds, string bracketFamilyTypeName)
        {
            //List<ElementId> usedKuiIds = new List<ElementId>();
            var usedKuiIdsLevel = new List<KeyValuePair<ElementId, string>>();

            bool isOsae = bracketFamilyTypeName == "腹起押えBL";

            foreach (var haraokoshiId in haraokoshiIds)
            {
                if (ClsCommandWarituke.GetIsKussaku(doc, haraokoshiId))
                {
                    continue;
                }

                // 腹起の始点終点から判定線を作成
                FamilyInstance instHaraokoshi = doc.GetElement(haraokoshiId) as FamilyInstance;
                if (instHaraokoshi.Symbol.Name != "腹起" || !instHaraokoshi.Symbol.Family.Name.Contains("主材"))
                {
                    if (!instHaraokoshi.Symbol.Family.Name.Contains("高強度腹起"))
                    {
                        continue;
                    }
                }

                // 始点から長さとベクトルを用いて終点を取得
                XYZ originalDirection = new XYZ(instHaraokoshi.FacingOrientation.X, instHaraokoshi.FacingOrientation.Y, 0.0);
                XYZ rotatedDirection = new XYZ(originalDirection.Y, -originalDirection.X, originalDirection.Z);
                XYZ haraokoshiStartPoint = instHaraokoshi.GetTotalTransform().Origin;
                double syuzaiLength = ClsRevitUtil.CovertToAPI(ClsYMSUtil.GetSyuzaiLength(instHaraokoshi.Symbol.Family.Name));
                if (syuzaiLength == 0)
                {
                    continue;
                }
                XYZ haraokoshiEndPoint = haraokoshiStartPoint + syuzaiLength * rotatedDirection;

                XYZ vecSt = (haraokoshiEndPoint - haraokoshiStartPoint).Normalize();
                XYZ vecEd = (haraokoshiStartPoint - haraokoshiEndPoint).Normalize();

                if (!isOsae) //配置禁止区域は腹起押えは無視する
                {
                    haraokoshiStartPoint = haraokoshiStartPoint + (m_ProhibitionLength * vecSt);
                    haraokoshiEndPoint = haraokoshiEndPoint + (m_ProhibitionLength * vecEd);
                }

                haraokoshiStartPoint = new XYZ(haraokoshiStartPoint.X, haraokoshiStartPoint.Y, XYZ.Zero.Z);
                haraokoshiEndPoint = new XYZ(haraokoshiEndPoint.X, haraokoshiEndPoint.Y, XYZ.Zero.Z);

                Curve cvHaraokoshi = Line.CreateBound(haraokoshiStartPoint, haraokoshiEndPoint);
                //CreateDebugLine(doc, haraokoshiStartPoint, haraokoshiEndPoint);

                // 杭から判定線を作成
                double searchRange = ClsRevitUtil.ConvertDoubleGeo2Revit(1100.0);   // 杭からの探査範囲

                // ブラケットの配置点を取得
                List<Tuple<XYZ, FamilyInstance>> intersectionPoints = new List<Tuple<XYZ, FamilyInstance>>();
                List<Tuple<XYZ, FamilyInstance>> intersectionPointsTwin = new List<Tuple<XYZ, FamilyInstance>>();
                foreach (var id in oyakuiIds)
                {
                    FamilyInstance instTrgOyakui = doc.GetElement(id) as FamilyInstance;
                    Curve cvOyakui = GetOyakuiCurve(doc, id, haraokoshiStartPoint, haraokoshiEndPoint, searchRange);
                    if (cvOyakui == null)
                    {
                        continue;
                    }

                    // 線分同士の交差を判定
                    SetComparisonResult result = cvHaraokoshi.Intersect(cvOyakui, out IntersectionResultArray intersectionResults);
                    if (result == SetComparisonResult.Overlap)
                    {
                        foreach (IntersectionResult intersectionResult in intersectionResults)
                        {
                            intersectionPoints.Add(new Tuple<XYZ, FamilyInstance>(intersectionResult.XYZPoint, instTrgOyakui));
                            //CreateDebugLine(doc, cvOyakui.GetEndPoint(0), cvOyakui.GetEndPoint(1));
                        }
                    }

                    // 線分同士の交差を判定//ツインビーム用
                    Line extendedLine = Line.CreateUnbound(cvHaraokoshi.GetEndPoint(0), cvHaraokoshi.GetEndPoint(1) - cvHaraokoshi.GetEndPoint(0));
                    result = extendedLine.Intersect(cvOyakui, out IntersectionResultArray intersectionResultsTwin);
                    if (result == SetComparisonResult.Overlap)
                    {
                        foreach (IntersectionResult intersectionResult in intersectionResultsTwin)
                        {
                            intersectionPointsTwin.Add(new Tuple<XYZ, FamilyInstance>(intersectionResult.XYZPoint, instTrgOyakui));
                        }
                    }
                }

                if (intersectionPoints.Count == 0)
                {
                    continue;
                }

                // ツインビーム切梁が接続されている場合、接続箇所につき1つ押えブラケットを配置する
                List<Tuple<XYZ, FamilyInstance>> intersectionPointsTwinBeam = new List<Tuple<XYZ, FamilyInstance>>();
                List<ElementId> kiribariBaseIds = new List<ElementId>();

                // 図面上の切梁を取得
                List<ElementId> kiribariIds = new List<ElementId>();
                kiribariIds = ClsRevitUtil.GetSelectCreatedFamilyInstanceList(doc, "切梁", true);
                foreach (var kiribariId in kiribariIds)
                {
                    FamilyInstance instkiribari = doc.GetElement(kiribariId) as FamilyInstance;
                    if (!instkiribari.Symbol.Family.Name.Contains("高強度"))
                    {
                        continue;
                    }

                    ElementId baseId = ClsCommandWarituke.GetBaseId(doc, kiribariId);
                    if (!kiribariBaseIds.Contains(baseId))
                    {
                        kiribariBaseIds.Add(baseId);
                    }
                }

                // 腹起と直交する切梁ベースを取得する
                foreach (var kiribaribaseId in kiribariBaseIds)
                {
                    FamilyInstance instBase = doc.GetElement(kiribaribaseId) as FamilyInstance;
                    if (instBase == null)
                    {
                        continue;
                    }
                    Curve cvBase = (instBase.Location as LocationCurve).Curve;
                    XYZ startPoint = cvBase.GetEndPoint(0);
                    XYZ endPoint = cvBase.GetEndPoint(1);

                    XYZ direction = (endPoint - startPoint).Normalize();

                    // 始点と終点を延長
                    XYZ newStartPoint = startPoint - direction * searchRange;
                    XYZ newEndPoint = endPoint + direction * searchRange;

                    // Z座標をゼロに設定
                    newStartPoint = new XYZ(newStartPoint.X, newStartPoint.Y, 0);
                    newEndPoint = new XYZ(newEndPoint.X, newEndPoint.Y, 0);

                    cvBase = Line.CreateBound(newStartPoint, newEndPoint);
                    //CreateDebugLine(doc, newStartPoint, newEndPoint);

                    SetComparisonResult result = cvHaraokoshi.Intersect(cvBase, out IntersectionResultArray intersectionResults);
                    if (result == SetComparisonResult.Overlap)
                    {
                        foreach (IntersectionResult intersectionResult in intersectionResults)
                        {
                            intersectionPointsTwinBeam.Add(new Tuple<XYZ, FamilyInstance>(intersectionResult.XYZPoint, instBase));
                        }
                    }
                }

                // 腹起と切梁の交点に近い親杭を取得する
                XYZ closestPoint = null;
                FamilyInstance instKui1_Twin = null;
                double minDistance1_Twin = double.MaxValue;

                XYZ secondClosestPoint = null;
                FamilyInstance instKui2_Twin = null;
                double minDistance2_Twin = double.MaxValue;

                foreach (Tuple<XYZ, FamilyInstance> trg in intersectionPointsTwinBeam)
                {
                    //#34069 start
                    XYZ pointKiribari = trg.Item1;
                    //foreach (var id in oyakuiIds)
                    //{
                    //    FamilyInstance instOyakui = doc.GetElement(id) as FamilyInstance;
                    //    XYZ pointOyakui = instOyakui.GetTotalTransform().Origin;
                    //    double distance = pointKiribari.DistanceTo(pointOyakui);

                    //    if (distance < minDistance1_Twin)
                    //    {
                    //        // 1番目の点を更新する前に、現在の1番目の点を2番目の点にスライドする
                    //        secondClosestPoint = closestPoint;
                    //        instKui2_Twin = instKui1_Twin;
                    //        minDistance2_Twin = minDistance1_Twin;

                    //        // 1番目の点を更新
                    //        minDistance1_Twin = distance;
                    //        closestPoint = pointOyakui;
                    //        instKui1_Twin = instOyakui;
                    //    }
                    //    else if (distance < minDistance2_Twin)
                    //    {
                    //        // 2番目の点を更新
                    //        minDistance2_Twin = distance;
                    //        secondClosestPoint = pointOyakui;
                    //        instKui2_Twin = instOyakui;
                    //    }

                    //}

                    foreach (Tuple<XYZ, FamilyInstance> intersectionPoint in intersectionPointsTwin)
                    {
                        XYZ pointOyakui = intersectionPoint.Item1;
                        double distance = pointKiribari.DistanceTo(pointOyakui);

                        if (distance < minDistance1_Twin)
                        {
                            // 1番目の点を更新する前に、現在の1番目の点を2番目の点にスライドする
                            secondClosestPoint = closestPoint;
                            instKui2_Twin = instKui1_Twin;
                            minDistance2_Twin = minDistance1_Twin;

                            // 1番目の点を更新
                            minDistance1_Twin = distance;
                            closestPoint = pointOyakui;
                            instKui1_Twin = intersectionPoint.Item2;
                        }
                        else if (distance < minDistance2_Twin)
                        {
                            // 2番目の点を更新
                            minDistance2_Twin = distance;
                            secondClosestPoint = pointOyakui;
                            instKui2_Twin = intersectionPoint.Item2;
                        }
                        //#34069 end
                    }
                }

                // 腹起の始点側に近い杭を取得
                XYZ crossPoint1 = null;
                FamilyInstance instKuiS = null;
                double minDistance1 = double.MaxValue;
                foreach (Tuple<XYZ, FamilyInstance> trg in intersectionPoints)
                {
                    XYZ point = trg.Item1;
                    double distance = haraokoshiStartPoint.DistanceTo(point);
                    if (distance < minDistance1)
                    {
                        minDistance1 = distance;
                        crossPoint1 = point;
                        instKuiS = trg.Item2;
                    }
                }

                // 腹起の終点側に近い杭を取得
                XYZ crossPoint2 = null;
                FamilyInstance instKuiE = null;
                double minDistance2 = double.MaxValue;
                foreach (Tuple<XYZ, FamilyInstance> trg in intersectionPoints)
                {
                    XYZ point = trg.Item1;
                    double distance = haraokoshiEndPoint.DistanceTo(point);
                    if (distance < minDistance2)
                    {
                        minDistance2 = distance;
                        crossPoint2 = point;
                        instKuiE = trg.Item2;
                    }
                }

                // 腹起の中央に近い杭を取得
                XYZ midpoint = cvHaraokoshi.Evaluate(0.5, true);
                XYZ crossPointMid = null;
                FamilyInstance instKuiMid = null;
                double minDistanceMid = double.MaxValue;
                foreach (Tuple<XYZ, FamilyInstance> trg in intersectionPoints)
                {
                    XYZ point = trg.Item1;
                    double distance = midpoint.DistanceTo(point);
                    if (distance < minDistanceMid)
                    {
                        minDistanceMid = distance;
                        crossPointMid = point;
                        instKuiMid = trg.Item2;
                    }
                }

                // 腹起ベースから情報を取得
                string haraokoshiSteelSize = ClsYMSUtil.GetSyuzaiSize(instHaraokoshi.Symbol.Family.Name);
                if (!GetTargetHaraokoshiId(doc, instKuiS, out ElementId haraokoshiBaseId, out bool isFront))
                {
                    continue;
                }
                int numV = ClsRevitUtil.GetInstMojiParameter(doc, haraokoshiBaseId, "縦本数") == "シングル" ? 1 : 2;
                int numH = ClsRevitUtil.GetInstMojiParameter(doc, haraokoshiBaseId, "横本数") == "シングル" ? 1 : 2;
                //bool isOsae = instHaraokoshi.Symbol.Family.Name.Contains("高強度腹起") && bracketFamilyTypeName == "腹起押えBL";

                // ファミリを取得
                if (m_CreateMode == CreateMode.Manual)
                {
                    //#33972
                    //if (m_BracketSize == "")
                    //{
                    //    ClsBracketSizeCsv.GetBracketSize(haraokoshiSteelSize, numV, numH, isOsae);
                    //}
                }
                else
                {
                    m_BracketSize = ClsBracketSizeCsv.GetBracketSize(haraokoshiSteelSize, numV, numH, isOsae); //#33972
                }

                string bracketFamilyPath = Master.ClsBracketCsv.GetFamilyPath(m_BracketSize);
                string bracketFamilyName = ClsRevitUtil.GetFamilyName(bracketFamilyPath);
                if (!ClsRevitUtil.LoadFamilyData(doc, bracketFamilyPath, out Family family))
                {
                    continue;
                }
                FamilySymbol sym = (ClsRevitUtil.GetFamilySymbol(doc, bracketFamilyName, bracketFamilyTypeName));

                double haraokoshiSize = ClsRevitUtil.CovertToAPI(ClsYamadomeCsv.GetWidth(haraokoshiSteelSize));

                string levelName = ClsRevitUtil.GetInstMojiParameter(doc, haraokoshiId, "集計レベル");
                if (levelName == null || levelName == "")
                {
                    levelName = ClsRevitUtil.GetInstMojiParameter(doc, haraokoshiId, "作業面");
                    levelName = levelName != null ? levelName.Replace("レベル : ", "") : null;
                }
                double elevation = 0.0;

                if (!isOsae || haraokoshiSteelSize == "80SMH")
                {
                    if (!usedKuiIdsLevel.Contains(new KeyValuePair<ElementId, string>(instKuiS.Id, levelName)))//!usedKuiIds.Contains(instKuiS.Id) || 
                    {
                        CreateHaraokoshiBracket(doc, bracketFamilyTypeName, haraokoshiId, searchRange, haraokoshiBaseId, sym, haraokoshiSize, elevation, instKuiS);
                        var pair = new KeyValuePair<ElementId, string>(instKuiS.Id, levelName);
                        usedKuiIdsLevel.Add(pair);
                    }
                    if (!usedKuiIdsLevel.Contains(new KeyValuePair<ElementId, string>(instKuiE.Id, levelName)))//!usedKuiIds.Contains(instKuiE.Id) || 
                    {
                        CreateHaraokoshiBracket(doc, bracketFamilyTypeName, haraokoshiId, searchRange, haraokoshiBaseId, sym, haraokoshiSize, elevation, instKuiE);
                        var pair = new KeyValuePair<ElementId, string>(instKuiE.Id, levelName);
                        usedKuiIdsLevel.Add(pair);
                    }
                }
                if (haraokoshiSteelSize == "80SMH")
                {
                    if (!usedKuiIdsLevel.Contains(new KeyValuePair<ElementId, string>(instKuiMid.Id, levelName)))//!usedKuiIds.Contains(instKuiMid.Id) || 
                    {
                        CreateHaraokoshiBracket(doc, bracketFamilyTypeName, haraokoshiId, searchRange, haraokoshiBaseId, sym, haraokoshiSize, elevation, instKuiMid);
                        var pair = new KeyValuePair<ElementId, string>(instKuiMid.Id, levelName);
                        usedKuiIdsLevel.Add(pair);
                    }
                }

                if (bracketFamilyTypeName == "腹起押えBL" && intersectionPointsTwinBeam.Count() != 0)
                {
                    // ファミリを取得
                    bracketFamilyPath = Master.ClsBracketCsv.GetFamilyPath(ClsBracketSizeCsv.GetBracketSize("80SMH", 1, 2, true));
                    bracketFamilyName = ClsRevitUtil.GetFamilyName(bracketFamilyPath);
                    if (!ClsRevitUtil.LoadFamilyData(doc, bracketFamilyPath, out Family fam))
                    {
                        continue;
                    }
                    sym = (ClsRevitUtil.GetFamilySymbol(doc, bracketFamilyName, bracketFamilyTypeName));

                    if (!usedKuiIdsLevel.Contains(new KeyValuePair<ElementId, string>(instKui1_Twin.Id, levelName)))//!usedKuiIds.Contains(instKui1_Twin.Id) || 
                    {
                        CreateHaraokoshiBracket(doc, bracketFamilyTypeName, haraokoshiId, searchRange, haraokoshiBaseId, sym, haraokoshiSize, elevation, instKui1_Twin);
                        var pair = new KeyValuePair<ElementId, string>(instKui1_Twin.Id, levelName);
                        usedKuiIdsLevel.Add(pair);
                    }
                    if (!usedKuiIdsLevel.Contains(new KeyValuePair<ElementId, string>(instKui2_Twin.Id, levelName)))//!usedKuiIds.Contains(instKui2_Twin.Id) || 
                    {
                        CreateHaraokoshiBracket(doc, bracketFamilyTypeName, haraokoshiId, searchRange, haraokoshiBaseId, sym, haraokoshiSize, elevation, instKui2_Twin);
                        var pair = new KeyValuePair<ElementId, string>(instKui2_Twin.Id, levelName);
                        usedKuiIdsLevel.Add(pair);
                    }
                }
            }

            return true;
        }

        private static void CreateHaraokoshiBracket(Document doc, string bracketFamilyTypeName, ElementId haraokoshiId, double searchRange, ElementId haraokoshiBaseId, FamilySymbol sym, double haraokoshiSize, double elevation, FamilyInstance instTrgOyakui)
        {
            if (instTrgOyakui == null) return;

            double trgKuiSize = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou1(instTrgOyakui.Symbol.FamilyName, 1)) / 2);
            XYZ insertPointSt = XYZ.Zero;
            XYZ centerPoint = XYZ.Zero;
            if (instTrgOyakui.Symbol.FamilyName.Contains("杭"))
            {
                insertPointSt = GetInsertPoint_Oyakui(doc, instTrgOyakui, instTrgOyakui.Symbol.Family.Name, ref centerPoint);
            }
            else if (instTrgOyakui.Symbol.FamilyName.Contains("鋼矢板"))
            {
                insertPointSt = GetInsertPoint_Kouyaita(doc, instTrgOyakui, instTrgOyakui.Symbol.Family.Name, ref centerPoint);
            }

            XYZ insertPointEd = XYZ.Zero;
            insertPointEd = insertPointSt + (searchRange * instTrgOyakui.FacingOrientation.Normalize());

            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                t.Start();

                ElementId levelIdKui = ClsRevitUtil.GetParameterElementId(doc, haraokoshiId, "集計レベル");

                ElementId createdId = ClsRevitUtil.Create(doc, insertPointSt, levelIdKui, sym);
                FamilyInstance instBracket = doc.GetElement(createdId) as FamilyInstance;

                // Z軸を回転軸としてFamilyInstanceを回転させる
                double angle = GetRotateAngle2(centerPoint, insertPointSt, instBracket);
                ElementTransformUtils.RotateElement(doc, createdId, Line.CreateBound(insertPointSt, insertPointSt + XYZ.BasisZ), angle);

                // ファミリの高さを設定
                double offset = double.NaN;
                if (bracketFamilyTypeName == "腹起BL")
                {
                    if (ClsRevitUtil.GetInstMojiParameter(doc, haraokoshiBaseId, "段") == "上段")
                    {
                        offset = elevation;
                    }
                    else if (ClsRevitUtil.GetInstMojiParameter(doc, haraokoshiBaseId, "段") == "下段")
                    {
                        offset = elevation - haraokoshiSize;
                    }
                    else if (ClsRevitUtil.GetInstMojiParameter(doc, haraokoshiBaseId, "段") == "同段")
                    {
                        offset = elevation - haraokoshiSize / 2;
                    }

                    if (ClsRevitUtil.GetInstMojiParameter(doc, haraokoshiId, "縦本数") == "ダブル")
                    {
                        offset = offset - haraokoshiSize;
                    }
                }
                else if (bracketFamilyTypeName == "腹起押えBL")
                {
                    if (ClsRevitUtil.GetInstMojiParameter(doc, haraokoshiBaseId, "段") == "上段")
                    {
                        offset = elevation + haraokoshiSize;
                    }
                    else if (ClsRevitUtil.GetInstMojiParameter(doc, haraokoshiBaseId, "段") == "下段")
                    {
                        offset = elevation;
                    }
                    else if (ClsRevitUtil.GetInstMojiParameter(doc, haraokoshiBaseId, "段") == "同段")
                    {
                        offset = elevation + haraokoshiSize / 2;
                    }

                    if (ClsRevitUtil.GetInstMojiParameter(doc, haraokoshiId, "縦本数") == "ダブル")
                    {
                        offset = offset + haraokoshiSize;
                    }
                }

                ClsRevitUtil.SetParameter(doc, createdId, "ホストからのオフセット", (offset));

                t.Commit();

                ClsHaraokoshiBase chb = new ClsHaraokoshiBase();
                chb.SetParameter(doc, haraokoshiBaseId);
                chb.CreateGapTyousei(doc, insertPointSt);
            }
        }

        public bool CreateHaraokoshiOsaeBracket_Manual(Document doc, ElementId kuiId, ElementId haraokoshiId)
        {
            if (kuiId == null)
            {
                return false;
            }
            FamilyInstance instKui = doc.GetElement(kuiId) as FamilyInstance;
            bool isFront;
            if (!GetTargetHaraokoshiId(doc, instKui, out ElementId haraokoshiId_base, out isFront))
            {
                return false;
            }

            FamilyInstance instHaraokoshi = doc.GetElement(haraokoshiId) as FamilyInstance;

            string levelName = ClsRevitUtil.GetInstMojiParameter(doc, haraokoshiId, "集計レベル");
            if (levelName == null || levelName == "")
            {
                levelName = ClsRevitUtil.GetInstMojiParameter(doc, haraokoshiId, "作業面");
                levelName = levelName != null ? levelName.Replace("レベル : ", "") : null;
            }
            double elevation = levelName != null ? ClsRevitUtil.GetLevelElevation(doc, levelName) : 0.0;

            string bracketFamilyPath = Master.ClsBracketCsv.GetFamilyPath(m_BracketSize);
            string bracketFamilyName = ClsRevitUtil.GetFamilyName(bracketFamilyPath);
            if (!ClsRevitUtil.LoadFamilyData(doc, bracketFamilyPath, out Family family))
            {
                return false;
            }
            string bracketFamilyTypeName = "腹起押えBL";
            FamilySymbol sym = (ClsRevitUtil.GetFamilySymbol(doc, bracketFamilyName, bracketFamilyTypeName));

            XYZ pkui = (instKui.Location as LocationPoint).Point;
            double kuiSizeHalf = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou1(instKui.Symbol.FamilyName, 1)) / 2);

            XYZ insertPointSt = XYZ.Zero;
            if (isFront)
            {
                insertPointSt = new XYZ(pkui.X + ((kuiSizeHalf) * instKui.FacingOrientation.X), pkui.Y + ((kuiSizeHalf) * instKui.FacingOrientation.Y), pkui.Z);
            }
            else
            {
                insertPointSt = new XYZ(pkui.X - ((kuiSizeHalf) * instKui.FacingOrientation.X), pkui.Y - ((kuiSizeHalf) * instKui.FacingOrientation.Y), pkui.Z);
            }

            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                t.Start();

                ElementId levelIdKui = ClsRevitUtil.GetParameterElementId(doc, kuiId, "集計レベル");

                ElementId createdId = ClsRevitUtil.Create(doc, insertPointSt, levelIdKui, sym);
                FamilyInstance instBracket = doc.GetElement(createdId) as FamilyInstance;

                // Z軸を回転軸としてFamilyInstanceを回転させる
                double angle = GetRotateAngle(pkui, insertPointSt, instBracket);
                ElementTransformUtils.RotateElement(doc, createdId, Line.CreateBound(insertPointSt, insertPointSt + XYZ.BasisZ), angle);

                string kiribariSteelSize = ClsRevitUtil.GetInstMojiParameter(doc, haraokoshiId, "鋼材サイズ").Replace("HA", "");
                double kiribariSize = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(kiribariSteelSize) * 10);
                ClsRevitUtil.SetParameter(doc, createdId, "ホストからのオフセット", (elevation + kiribariSize));

                t.Commit();
            }

            return true;
        }

        public bool CreateHaraokoshiOsaeBracket_Auto(Document doc, ElementId kuiId, ElementId haraokoshiId)
        {
            if (kuiId == null)
            {
                return false;
            }

            if (ClsRevitUtil.GetInstMojiParameter(doc, haraokoshiId, "横本数") != "ダブル")
            {
                return false;
            }

            // 腹起の始点終点から判定線を作成
            FamilyInstance instHaraokoshi = doc.GetElement(haraokoshiId) as FamilyInstance;

            // 始点から長さとベクトルを用いて終点を取得
            XYZ originalDirection = new XYZ(instHaraokoshi.FacingOrientation.X, instHaraokoshi.FacingOrientation.Y, 0.0);
            XYZ rotatedDirection = new XYZ(originalDirection.Y, -originalDirection.X, originalDirection.Z);
            LocationPoint locationPoint = instHaraokoshi.Location as LocationPoint;
            XYZ haraokoshiStartPoint = locationPoint.Point;
            double syuzaiLength = ClsRevitUtil.CovertToAPI(ClsYMSUtil.GetSyuzaiLength(instHaraokoshi.Symbol.Family.Name));
            XYZ haraokoshiEndPoint = haraokoshiStartPoint + syuzaiLength * rotatedDirection;

            XYZ vecSt = (haraokoshiEndPoint - haraokoshiStartPoint).Normalize();
            XYZ vecEd = (haraokoshiStartPoint - haraokoshiEndPoint).Normalize();

            haraokoshiStartPoint = haraokoshiStartPoint + (m_ProhibitionLength * vecSt);
            haraokoshiEndPoint = haraokoshiEndPoint + (m_ProhibitionLength * vecEd);

            haraokoshiStartPoint = new XYZ(haraokoshiStartPoint.X, haraokoshiStartPoint.Y, XYZ.Zero.Z);
            haraokoshiEndPoint = new XYZ(haraokoshiEndPoint.X, haraokoshiEndPoint.Y, XYZ.Zero.Z);

            Curve cvHaraokoshi = Line.CreateBound(haraokoshiStartPoint, haraokoshiEndPoint);
            //CreateDebugLine(doc, haraokoshiStartPoint, cvHaraokoshi);

            // 杭から判定線を作成
            FamilyInstance instKui = doc.GetElement(kuiId) as FamilyInstance;
            double searchRange = ClsRevitUtil.ConvertDoubleGeo2Revit(1100.0);   // 杭からの探査範囲

            List<Tuple<XYZ, FamilyInstance>> intersectionPoints = new List<Tuple<XYZ, FamilyInstance>>();
            List<ElementId> bracketIds = GetAllHaraokoshiBracketList(doc);
            foreach (var id in bracketIds)
            {
                FamilyInstance instTrgBracket = doc.GetElement(id) as FamilyInstance;
                XYZ pSt = (instTrgBracket.Location as LocationPoint).Point;
                pSt = new XYZ(pSt.X, pSt.Y, XYZ.Zero.Z);
                XYZ pEd = pSt + (searchRange * instKui.FacingOrientation.Normalize());
                pEd = new XYZ(pEd.X, pEd.Y, XYZ.Zero.Z);
                Curve cvBracket = Line.CreateBound(pSt, pEd);

                //CreateDebugLine(doc, pSt, cvBracket);

                // 線分同士の交差を判定
                SetComparisonResult result = cvHaraokoshi.Intersect(cvBracket, out IntersectionResultArray intersectionResults);
                if (result == SetComparisonResult.Overlap)
                {
                    foreach (IntersectionResult intersectionResult in intersectionResults)
                    {
                        intersectionPoints.Add(new Tuple<XYZ, FamilyInstance>(intersectionResult.XYZPoint, instTrgBracket));
                    }
                }
            }

            string haraokoshiSteelSize = ClsYMSUtil.GetSyuzaiSize(instHaraokoshi.Symbol.Family.Name);
            if (!GetTargetHaraokoshiId(doc, instKui, out ElementId haraokoshiBaseId, out bool isFront))
            {
                return false;
            }
            int numV = ClsRevitUtil.GetInstMojiParameter(doc, haraokoshiBaseId, "縦本数") == "シングル" ? 1 : 2;
            int numH = ClsRevitUtil.GetInstMojiParameter(doc, haraokoshiBaseId, "横本数") == "シングル" ? 1 : 2;

            string bracketFamilyPath = Master.ClsBracketCsv.GetFamilyPath(ClsBracketSizeCsv.GetBracketSize(haraokoshiSteelSize, numV, numH));
            string bracketFamilyName = ClsRevitUtil.GetFamilyName(bracketFamilyPath);
            if (!ClsRevitUtil.LoadFamilyData(doc, bracketFamilyPath, out Family family))
            {
                return false;
            }
            string bracketFamilyTypeName = "腹起押えBL";
            FamilySymbol sym = (ClsRevitUtil.GetFamilySymbol(doc, bracketFamilyName, bracketFamilyTypeName));

            haraokoshiSteelSize = haraokoshiSteelSize.Replace("HA", "");
            if (haraokoshiSteelSize.Contains("SMH"))
            {
                haraokoshiSteelSize = "40";
            }
            double haraokoshiSize = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(haraokoshiSteelSize) * 10);

            string levelName = ClsRevitUtil.GetInstMojiParameter(doc, haraokoshiId, "集計レベル");
            if (levelName == null || levelName == "")
            {
                levelName = ClsRevitUtil.GetInstMojiParameter(doc, haraokoshiId, "作業面");
                levelName = levelName != null ? levelName.Replace("レベル : ", "") : null;
            }
            double elevation = levelName != null ? ClsRevitUtil.GetLevelElevation(doc, levelName) : 0.0;

            foreach (Tuple<XYZ, FamilyInstance> trg in intersectionPoints)
            {
                FamilyInstance instBracket = trg.Item2;
                XYZ insertPointSt = (instBracket.Location as LocationPoint).Point;

                using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                {
                    t.Start();

                    ElementId levelIdKui = ClsRevitUtil.GetParameterElementId(doc, kuiId, "集計レベル");

                    ElementId createdId = ClsRevitUtil.Create(doc, insertPointSt, levelIdKui, sym);
                    FamilyInstance instOsaeBracket = doc.GetElement(createdId) as FamilyInstance;

                    // Z軸を回転軸としてFamilyInstanceを回転させる
                    double angle = GetRotateAngle(insertPointSt, trg.Item1, instOsaeBracket);
                    ElementTransformUtils.RotateElement(doc, createdId, Line.CreateBound(insertPointSt, insertPointSt + XYZ.BasisZ), angle);

                    ClsRevitUtil.SetParameter(doc, createdId, "ホストからのオフセット", (elevation + haraokoshiSize));

                    t.Commit();
                }
            }

            return true;
        }

        private bool GetTargetHaraokoshiId(Document doc, FamilyInstance instKui, out ElementId HaraokoshiId, out bool isFront)
        {
            HaraokoshiId = null;
            isFront = true;

            if (instKui != null)
            {
                double searchRange = ClsRevitUtil.ConvertDoubleGeo2Revit(1500.0);   // 杭からの探査範囲
                double kuiSizeHalf = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou1(instKui.Symbol.FamilyName, 1)) / 2);
                XYZ direction = new XYZ(instKui.FacingOrientation.X, instKui.FacingOrientation.Y, 0.0);

                XYZ kuiPtSF = instKui.GetTotalTransform().Origin + kuiSizeHalf * direction;
                XYZ kuiPtSB = instKui.GetTotalTransform().Origin - kuiSizeHalf * direction;
                kuiPtSF = new XYZ(kuiPtSF.X, kuiPtSF.Y, XYZ.Zero.Z);
                kuiPtSB = new XYZ(kuiPtSB.X, kuiPtSB.Y, XYZ.Zero.Z);

                XYZ kuiPtEF = kuiPtSF + searchRange * direction;
                XYZ kuiPtEB = kuiPtSB - searchRange * direction;
                kuiPtEF = new XYZ(kuiPtEF.X, kuiPtEF.Y, XYZ.Zero.Z);
                kuiPtEB = new XYZ(kuiPtEB.X, kuiPtEB.Y, XYZ.Zero.Z);

                Curve cvHanteiLineFront = Line.CreateBound(kuiPtSF, kuiPtEF);
                Curve cvHanteiLineBack = Line.CreateBound(kuiPtSB, kuiPtEB);

                // 図面上の腹起を取得
                List<ElementId> haraokoshiFamilyInstancesIds = new List<ElementId>();
                haraokoshiFamilyInstancesIds = ClsRevitUtil.GetSelectCreatedFamilyInstanceList(doc, "腹起", true);
                if (haraokoshiFamilyInstancesIds.Count == 0)
                {
                    return false;
                }

                foreach (var id in haraokoshiFamilyInstancesIds)
                {
                    FamilyInstance haraokoshifamilyInstance = doc.GetElement(id) as FamilyInstance;
                    // 腹起の始点と終点を取得し、Curveを作成
                    LocationCurve locationCurve = haraokoshifamilyInstance.Location as LocationCurve;
                    if (locationCurve != null)
                    {
                        XYZ haraokoshiStartPoint = locationCurve.Curve.GetEndPoint(0);
                        XYZ haraokoshiEndPoint = locationCurve.Curve.GetEndPoint(1);
                        Curve cvHaraokoshi = Line.CreateBound(haraokoshiStartPoint, haraokoshiEndPoint);

                        // CurveのZ座標を0に設定
                        cvHaraokoshi = Line.CreateBound(new XYZ(cvHaraokoshi.GetEndPoint(0).X, cvHaraokoshi.GetEndPoint(0).Y, XYZ.Zero.Z),
                                                        new XYZ(cvHaraokoshi.GetEndPoint(1).X, cvHaraokoshi.GetEndPoint(1).Y, XYZ.Zero.Z));

                        // 判定
                        SetComparisonResult resultF = cvHaraokoshi.Intersect(cvHanteiLineFront);
                        if (resultF == SetComparisonResult.Overlap)
                        {
                            //// 交差している場合、曲線をハイライト表示
                            //using (Transaction tx = new Transaction(doc, "Change Line Color"))
                            //{
                            //    tx.Start();
                            //    ClsRevitUtil.ChangeLineColor(doc, kiribarifamilyInstance.Id, ClsGlobal.m_lightBlue);
                            //    tx.Commit();
                            //}

                            HaraokoshiId = id;
                            isFront = true;
                            return true;
                        }
                        SetComparisonResult resultB = cvHaraokoshi.Intersect(cvHanteiLineBack);
                        if (resultB == SetComparisonResult.Overlap)
                        {
                            //// 交差している場合、曲線をハイライト表示
                            //using (Transaction tx = new Transaction(doc, "Change Line Color"))
                            //{
                            //    tx.Start();
                            //    ClsRevitUtil.ChangeLineColor(doc, kiribarifamilyInstance.Id, ClsGlobal.m_lightBlue);
                            //    tx.Commit();
                            //}

                            HaraokoshiId = id;
                            isFront = false;
                            return true;
                        }
                    }
                }

                return false;
            }

            return true;
        }

        private bool GetTargetKiribariBaseId_Web(Document doc, ElementId kuiId, out List<ElementId> beamIds, out List<bool> isFront, out List<XYZ> crossPoints)
        {
            beamIds = new List<ElementId>();
            isFront = new List<bool>();
            crossPoints = new List<XYZ>();

            FamilyInstance kuiInstance = doc.GetElement(kuiId) as FamilyInstance;
            if (kuiInstance == null)
            {
                return false;
            }

            if (m_TargetKuiAngle == 0)
            {
                Transform transform = kuiInstance.GetTransform();
                double angle = transform.BasisY.AngleOnPlaneTo(XYZ.BasisY, XYZ.BasisZ);
                // 角度をπ（180度）で割って2π（360度）を超える場合は補正する
                if (angle > Math.PI)
                {
                    angle -= Math.PI;
                }
                m_TargetKuiAngle = angle * (180.0 / Math.PI); // ラジアンから度数に変換
            }

            // 杭の判定線を取得
            Curve[] kuiCurves = ClsTanakui.CreateBoundaryLinesWeb(kuiInstance, 1000);

            // 切梁を取得
            List<ElementId> baseFamilyInstanceIds = new List<ElementId>();
            baseFamilyInstanceIds = ClsRevitUtil.GetSelectCreatedFamilyInstanceList(doc, "切梁ベース", true);
            if (baseFamilyInstanceIds.Count == 0)
            {
                return false;
            }

            // 杭と切梁ベースの接触判定
            foreach (var baseId in baseFamilyInstanceIds)
            {
                FamilyInstance baseInstance = doc.GetElement(baseId) as FamilyInstance;
                Curve cvBase = (baseInstance.Location as LocationCurve).Curve;
                XYZ startPoint = cvBase.GetEndPoint(0);
                XYZ endPoint = cvBase.GetEndPoint(1);

                // Z軸を0に設定
                XYZ startPointWithZeroZ = new XYZ(startPoint.X, startPoint.Y, 0);
                XYZ endPointWithZeroZ = new XYZ(endPoint.X, endPoint.Y, 0);

                // 新しい Curve を作成
                Curve cvKiribariBase = Line.CreateBound(startPointWithZeroZ, endPointWithZeroZ);

                // 切梁との交差を検査
                IntersectionResultArray intersectionResults1 = new IntersectionResultArray();
                cvKiribariBase.Intersect(kuiCurves[0], out intersectionResults1);
                IntersectionResultArray intersectionResults2 = new IntersectionResultArray();
                cvKiribariBase.Intersect(kuiCurves[1], out intersectionResults2);

                // 交差点があるか確認
                if (intersectionResults1 != null && intersectionResults1.Size > 0)
                {
                    // 最初の交点を取得
                    IntersectionResult intersectionResult = intersectionResults1.get_Item(0);
                    XYZ intersectionPoint = intersectionResult.XYZPoint;
                    beamIds.Add(baseId);
                    isFront.Add(true);
                    crossPoints.Add(new XYZ(intersectionPoint.X, intersectionPoint.Y, startPoint.Z));
                    continue;
                }

                if (intersectionResults2 != null && intersectionResults2.Size > 0)
                {
                    // 最初の交点を取得
                    IntersectionResult intersectionResult = intersectionResults2.get_Item(0);
                    XYZ intersectionPoint = intersectionResult.XYZPoint;
                    beamIds.Add(baseId);
                    isFront.Add(false);
                    crossPoints.Add(new XYZ(intersectionPoint.X, intersectionPoint.Y, startPoint.Z));
                    continue;
                }

            }

            if (beamIds.Count() == 0)
            {
                return false;
            }

            return true;
        }

        private bool GetTargetKiribariBaseId_Flange(Document doc, ElementId kuiId, out List<ElementId> beamIds, out List<bool> isFront, out List<XYZ> crossPoints)
        {
            beamIds = new List<ElementId>();
            isFront = new List<bool>();
            crossPoints = new List<XYZ>();

            FamilyInstance kuiInstance = doc.GetElement(kuiId) as FamilyInstance;
            if (kuiInstance == null)
            {
                return false;
            }

            if (m_TargetKuiAngle == 0)
            {
                Transform transform = kuiInstance.GetTransform();
                double angle = transform.BasisY.AngleOnPlaneTo(XYZ.BasisY, XYZ.BasisZ);
                // 角度をπ（180度）で割って2π（360度）を超える場合は補正する
                if (angle > Math.PI)
                {
                    angle -= Math.PI;
                }
                m_TargetKuiAngle = angle * (180.0 / Math.PI); // ラジアンから度数に変換
            }

            double kuiAngle = Math.PI * m_TargetKuiAngle / 180.0;

            // 杭の判定線を取得
            Curve[] kuiCurves = ClsTanakui.CreateBoundaryLinesFlange(kuiInstance, 1000, kuiAngle);
            //CreateDebugLine(doc, kuiInstance.GetTotalTransform().Origin, kuiCurves[0]);
            //CreateDebugLine(doc, kuiInstance.GetTotalTransform().Origin, kuiCurves[1]);

            // 切梁を取得
            List<ElementId> baseFamilyInstanceIds = new List<ElementId>();
            baseFamilyInstanceIds = ClsRevitUtil.GetSelectCreatedFamilyInstanceList(doc, "切梁ベース", true);
            if (baseFamilyInstanceIds.Count == 0)
            {
                return false;
            }

            // 杭と切梁ベースの接触判定
            foreach (var baseId in baseFamilyInstanceIds)
            {
                FamilyInstance baseInstance = doc.GetElement(baseId) as FamilyInstance;
                Curve cvBase = (baseInstance.Location as LocationCurve).Curve;
                XYZ startPoint = cvBase.GetEndPoint(0);
                XYZ endPoint = cvBase.GetEndPoint(1);

                // Z軸を0に設定
                XYZ startPointWithZeroZ = new XYZ(startPoint.X, startPoint.Y, 0);
                XYZ endPointWithZeroZ = new XYZ(endPoint.X, endPoint.Y, 0);

                // 新しい Curve を作成
                Curve cvKiribariBase = Line.CreateBound(startPointWithZeroZ, endPointWithZeroZ);

                // 切梁との交差を検査
                IntersectionResultArray intersectionResults1 = new IntersectionResultArray();
                cvKiribariBase.Intersect(kuiCurves[0], out intersectionResults1);
                IntersectionResultArray intersectionResults2 = new IntersectionResultArray();
                cvKiribariBase.Intersect(kuiCurves[1], out intersectionResults2);

                // 交差点があるか確認
                if (intersectionResults1 != null && intersectionResults1.Size > 0)
                {
                    // 最初の交点を取得
                    IntersectionResult intersectionResult = intersectionResults1.get_Item(0);
                    XYZ intersectionPoint = intersectionResult.XYZPoint;
                    beamIds.Add(baseId);
                    isFront.Add(true);
                    crossPoints.Add(new XYZ(intersectionPoint.X, intersectionPoint.Y, startPoint.Z));
                    continue;
                }

                if (intersectionResults2 != null && intersectionResults2.Size > 0)
                {
                    // 最初の交点を取得
                    IntersectionResult intersectionResult = intersectionResults2.get_Item(0);
                    XYZ intersectionPoint = intersectionResult.XYZPoint;
                    beamIds.Add(baseId);
                    isFront.Add(false);
                    crossPoints.Add(new XYZ(intersectionPoint.X, intersectionPoint.Y, startPoint.Z));
                    continue;
                }

            }

            if (beamIds.Count() == 0)
            {
                return false;
            }

            return true;
        }

        public bool CreatePreloadGuideZai(Document doc, ElementId bracketId, ElementId kiribariId)
        {
            // 切梁のサイズ取得処理
            string kiribariSteelSize = ClsRevitUtil.GetInstMojiParameter(doc, kiribariId, "鋼材サイズ");
            double kiribariSize = ClsRevitUtil.CovertToAPI(Master.ClsYamadomeCsv.GetWidth(kiribariSteelSize));
            if (kiribariSize == 0)
            {
                FamilyInstance instKiribari = doc.GetElement(kiribariId) as FamilyInstance;
                string skiribariSize = ClsYMSUtil.GetSyuzaiSize(instKiribari.Symbol.FamilyName);
                string numericValue = Regex.Match(skiribariSize, @"\d+").Value;
                kiribariSize = ClsRevitUtil.CovertToAPI((double.Parse(numericValue) * 10));
            }

            // ファミリの取得処理
            string symbolFolpath = ClsZumenInfo.GetYMSFolder();
            string familyPath = Master.ClsNekozai.GetFamilyPath("L-75x75x6", "D-15");
            string typeName = "ﾌﾟﾚﾛｰﾄﾞｶﾞｲﾄﾞ材";
            string familyName = ClsRevitUtil.GetFamilyName(familyPath);

            if (!ClsRevitUtil.LoadFamilyData(doc, familyPath, out Family family))
            {
                return false;
            }
            FamilySymbol sym = (ClsRevitUtil.GetFamilySymbol(doc, familyName, typeName));

            // 作図位置の算出処理
            XYZ ptCross = XYZ.Zero;
            XYZ ptInsert1 = XYZ.Zero;
            XYZ ptInsert2 = XYZ.Zero;
            double angle1 = double.NaN;
            double angle2 = double.NaN;

            FamilyInstance instTarget = doc.GetElement(bracketId) as FamilyInstance;
            if (instTarget.Symbol.Name == "切梁BL")
            {
                // ブラケットの線分
                XYZ ptBracket = (instTarget.Location as LocationPoint).Point;
                XYZ defaultFacing = new XYZ(instTarget.FacingOrientation.X, instTarget.FacingOrientation.Y, instTarget.FacingOrientation.Z);
                string bracketSize = instTarget.Symbol.Family.Name.Replace("ﾌﾞﾗｹｯﾄ_", "");
                double bracketLength = ClsRevitUtil.CovertToAPI(ClsBracketCsv.GetLength("切梁ブラケット", bracketSize) / 2);
                XYZ ptBracketStart = ptBracket;
                XYZ ptBracketEnd = ptBracket - (defaultFacing * bracketLength);
                ptBracketStart = new XYZ(ptBracketStart.X, ptBracketStart.Y, 0);
                ptBracketEnd = new XYZ(ptBracketEnd.X, ptBracketEnd.Y, 0);
                Curve cvBracket = Line.CreateBound(ptBracketStart, ptBracketEnd);

                // 切梁ベースの線分
                FamilyInstance baseInstance = doc.GetElement(kiribariId) as FamilyInstance;
                Curve cvBase = (baseInstance.Location as LocationCurve).Curve;
                XYZ startPoint = cvBase.GetEndPoint(0);
                XYZ endPoint = cvBase.GetEndPoint(1);
                XYZ startPointWithZeroZ = new XYZ(startPoint.X, startPoint.Y, 0);
                XYZ endPointWithZeroZ = new XYZ(endPoint.X, endPoint.Y, 0);
                Curve cvKiribariBase = Line.CreateBound(startPointWithZeroZ, endPointWithZeroZ);

                // 交点を取得
                IntersectionResultArray results;
                cvKiribariBase.Intersect(cvBracket, out results);

                // 交点があるか確認
                if (results != null && results.Size > 0)
                {
                    IntersectionResult intersectionResult = results.get_Item(0);
                    ptCross = intersectionResult.XYZPoint;
                }
                else
                {
                    //System.Windows.Forms.MessageBox.Show("交点が見つかりません");
                    return false;
                }

                // プレロードガイド材の回転角度を取得
                GetPreloadGuideZaiAngle(ptBracket, ptCross, instTarget, ref angle1, ref angle2);

                // 切梁ベースとの交点を中心に切梁のサイズ分だけオフセットする
                ptInsert1 = ptCross + (defaultFacing * +kiribariSize / 2);
                ptInsert2 = ptCross + (defaultFacing * -kiribariSize / 2);

                // 杭に対してブラケットの右側に配置するためオフセットする
                double bracketWidth = ClsRevitUtil.CovertToAPI(Master.ClsBracketCsv.GetWidth(bracketSize));
                XYZ rightFacing = new XYZ(-defaultFacing.Y, defaultFacing.X, 0);
                ptInsert1 = ptInsert1 + (-rightFacing * +bracketWidth / 2);
                ptInsert2 = ptInsert2 + (rightFacing * -bracketWidth / 2);

            }

            // 作図処理
            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                t.Start();
                FailureHandlingOptions failOpt = t.GetFailureHandlingOptions();
                failOpt.SetFailuresPreprocessor(new WarningSwallower());
                t.SetFailureHandlingOptions(failOpt);

                ElementId createdId1 = ClsRevitUtil.Create(doc, ptInsert1, instTarget.LevelId, sym);
                ElementId createdId2 = ClsRevitUtil.Create(doc, ptInsert2, instTarget.LevelId, sym);

                // Z軸を回転軸としてFamilyInstanceを回転させる
                FamilyInstance instGuideZai1 = doc.GetElement(createdId1) as FamilyInstance;
                ElementTransformUtils.RotateElement(doc, createdId1, Line.CreateBound(ptInsert1, ptInsert1 + XYZ.BasisZ), angle1);
                FamilyInstance instGuideZai2 = doc.GetElement(createdId2) as FamilyInstance;
                ElementTransformUtils.RotateElement(doc, createdId2, Line.CreateBound(ptInsert2, ptInsert2 + XYZ.BasisZ), angle2);

                double offset = ClsRevitUtil.GetParameterDouble(doc, bracketId, "基準レベルからの高さ");
                ClsRevitUtil.SetParameter(doc, createdId1, "基準レベルからの高さ", (offset));
                ClsRevitUtil.SetParameter(doc, createdId2, "基準レベルからの高さ", (offset));

                t.Commit();
            }

            return true;
        }

        private void GetPreloadGuideZaiAngle(XYZ ptBracket, XYZ ptCross, FamilyInstance instTarget, ref double angle1, ref double angle2)
        {
            double bracketAngle = double.NaN;
            double kuiAngle = m_TargetKuiAngle * Math.PI / 180.0;
            switch (m_TargetKuiCreatePosition)
            {
                case ClsTanakui.CreatePosition.LeftTop:
                    bracketAngle = GetRotateAngle(ptBracket, ptCross, instTarget, false) * Math.PI / 180.0;
                    if (m_TargetKuiAngle >= 0 && m_TargetKuiAngle < 90)
                    {
                        angle1 = Math.PI - bracketAngle;
                        angle2 = (Math.PI / 2) - bracketAngle;
                    }
                    else
                    {
                        angle1 = (Math.PI) + kuiAngle;
                        angle2 = (Math.PI / 2) + kuiAngle;
                    }
                    break;
                case ClsTanakui.CreatePosition.RightTop:
                    bracketAngle = GetRotateAngle(ptBracket, ptCross, instTarget, false) * Math.PI / 180.0;
                    if (m_TargetKuiAngle >= 0 && m_TargetKuiAngle < 90)
                    {
                        angle1 = Math.PI - bracketAngle;
                        angle2 = (Math.PI / 2) - bracketAngle;
                    }
                    else
                    {
                        angle1 = (Math.PI) - kuiAngle;
                        angle2 = (Math.PI / 2) - kuiAngle;
                    }
                    break;
                case ClsTanakui.CreatePosition.LeftBottom:
                    bracketAngle = GetRotateAngle(ptBracket, ptCross, instTarget, true) * Math.PI / 180.0;
                    if (m_TargetKuiAngle >= 0 && m_TargetKuiAngle < 90)
                    {
                        angle1 = 0.0;
                        angle2 = -(Math.PI / 2);
                    }
                    else
                    {
                        angle1 = (Math.PI) + kuiAngle;
                        angle2 = (Math.PI / 2) + kuiAngle;
                    }
                    break;
                case ClsTanakui.CreatePosition.RightBottom:
                    if (m_TargetKuiAngle >= 0 && m_TargetKuiAngle < 90)
                    {
                        angle1 = 0.0;
                        angle2 = -(Math.PI / 2);
                    }
                    else
                    {
                        angle1 = (Math.PI) - kuiAngle;
                        angle2 = (Math.PI / 2) - kuiAngle;
                    }
                    break;
                default:
                    angle1 = 0.0;
                    angle2 = 0.0;
                    break;
            }
        }

        public bool CreateGuideZai(Document doc, ElementId bracketId, ElementId kiribariId)
        {
            // 切梁のサイズ取得処理
            string kiribariSteelSize = ClsRevitUtil.GetInstMojiParameter(doc, kiribariId, "鋼材サイズ");
            double kiribariSize = ClsRevitUtil.CovertToAPI(Master.ClsYamadomeCsv.GetWidth(kiribariSteelSize));
            if (kiribariSize == 0)
            {
                FamilyInstance instKiribari = doc.GetElement(kiribariId) as FamilyInstance;
                string skiribariSize = ClsYMSUtil.GetSyuzaiSize(instKiribari.Symbol.FamilyName);
                string numericValue = Regex.Match(skiribariSize, @"\d+").Value;
                kiribariSize = ClsRevitUtil.CovertToAPI((double.Parse(numericValue) * 10));
            }

            // ファミリの取得処理
            string symbolFolpath = ClsZumenInfo.GetYMSFolder();
            string familyPath = Master.ClsNekozai.GetFamilyPath("L-75x75x6", "D-10");
            string typeName = "ｶﾞｲﾄﾞ材";
            string familyName = ClsRevitUtil.GetFamilyName(familyPath);

            if (!ClsRevitUtil.LoadFamilyData(doc, familyPath, out Family family))
            {
                return false;
            }
            FamilySymbol sym = (ClsRevitUtil.GetFamilySymbol(doc, familyName, typeName));

            // 作図位置の算出処理
            XYZ ptCross = XYZ.Zero;
            XYZ ptInsert1 = XYZ.Zero;
            XYZ ptInsert2 = XYZ.Zero;

            FamilyInstance instTarget = doc.GetElement(bracketId) as FamilyInstance;
            if (instTarget.Symbol.Name == "切梁押えBL")
            {
                // ブラケットの線分
                XYZ ptBracket = (instTarget.Location as LocationPoint).Point;
                XYZ defaultFacing = new XYZ(instTarget.FacingOrientation.X, instTarget.FacingOrientation.Y, instTarget.FacingOrientation.Z);
                string bracketSize = instTarget.Symbol.Family.Name.Replace("ﾌﾞﾗｹｯﾄ_", "");
                double bracketLengthHalf = ClsRevitUtil.CovertToAPI(ClsBracketCsv.GetLength("切梁ブラケット", bracketSize) / 2);
                XYZ ptBracketStart = ptBracket;
                XYZ ptBracketEnd = ptBracket - (defaultFacing * (bracketLengthHalf * 2));
                ptBracketStart = new XYZ(ptBracketStart.X, ptBracketStart.Y, 0);
                ptBracketEnd = new XYZ(ptBracketEnd.X, ptBracketEnd.Y, 0);
                Curve cvBracket = Line.CreateBound(ptBracketStart, ptBracketEnd);
                //CreateDebugLine(doc, ptBracketStart, ptBracketEnd);

                // 切梁ベースの線分
                FamilyInstance baseInstance = doc.GetElement(kiribariId) as FamilyInstance;
                Curve cvBase = (baseInstance.Location as LocationCurve).Curve;
                XYZ startPoint = cvBase.GetEndPoint(0);
                XYZ endPoint = cvBase.GetEndPoint(1);
                XYZ startPointWithZeroZ = new XYZ(startPoint.X, startPoint.Y, 0);
                XYZ endPointWithZeroZ = new XYZ(endPoint.X, endPoint.Y, 0);
                Curve cvKiribariBase = Line.CreateBound(startPointWithZeroZ, endPointWithZeroZ);

                // 交点を取得
                IntersectionResultArray results;
                cvKiribariBase.Intersect(cvBracket, out results);

                // 交点があるか確認
                if (results != null && results.Size > 0)
                {
                    IntersectionResult intersectionResult = results.get_Item(0);
                    ptCross = intersectionResult.XYZPoint;
                }
                else
                {
                    //System.Windows.Forms.MessageBox.Show("交点が見つかりません");
                    //CreateDebugLine(doc, startPointWithZeroZ, endPointWithZeroZ);
                    return false;
                }

                // 切梁ベースとの交点を中心に切梁のサイズ分だけオフセットする
                ptInsert1 = ptCross + (defaultFacing * +kiribariSize / 2);
                ptInsert2 = ptCross + (defaultFacing * -kiribariSize / 2);
            }

            //// 高さ位置の算出処理
            //string levelName = ClsRevitUtil.GetInstMojiParameter(doc, kiribariId, "集計レベル");
            //if (levelName == null || levelName == "")
            //{
            //    levelName = ClsRevitUtil.GetInstMojiParameter(doc, kiribariId, "作業面");
            //    levelName = levelName != null ? levelName.Replace("レベル : ", "") : null;
            //}
            //double elevation = ClsRevitUtil.GetLevelElevation(doc, levelName);
            //elevation = elevation + kiribariSize;

            // 作図処理
            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                t.Start();

                ElementId createdId1 = ClsRevitUtil.Create(doc, ptInsert1, instTarget.LevelId, sym);
                FamilyInstance instGuideZai1 = doc.GetElement(createdId1) as FamilyInstance;
                ElementId createdId2 = ClsRevitUtil.Create(doc, ptInsert2, instTarget.LevelId, sym);
                FamilyInstance instGuideZai2 = doc.GetElement(createdId2) as FamilyInstance;

                // Z軸を回転軸としてFamilyInstanceを回転させる
                double angle1 = GetRotateAngle2(ptCross, ptInsert1, instGuideZai1);
                ElementTransformUtils.RotateElement(doc, createdId1, Line.CreateBound(ptInsert1, ptInsert1 + XYZ.BasisZ), angle1);
                double angle2 = GetRotateAngle2(ptCross, ptInsert2, instGuideZai2);
                ElementTransformUtils.RotateElement(doc, createdId2, Line.CreateBound(ptInsert2, ptInsert2 + XYZ.BasisZ), angle2);

                double offset = ClsRevitUtil.GetParameterDouble(doc, bracketId, "基準レベルからの高さ");
                ClsRevitUtil.SetParameter(doc, createdId1, "基準レベルからの高さ", (offset));
                ClsRevitUtil.SetParameter(doc, createdId2, "基準レベルからの高さ", (offset));

                t.Commit();
            }

            return true;
        }

        private static double GetRotateAngle(XYZ pkui, XYZ insertPointSt, FamilyInstance inst, bool isReverse = false)
        {
            // 基準点と挿入点
            XYZ basePoint = pkui; // 基準点の座標

            // 基準点から挿入点へのベクトルを取得
            XYZ direction = basePoint - insertPointSt;
            if (isReverse)
            {
                direction = insertPointSt - basePoint;
            }

            // ベクトルをZ軸に対応する方向に変換（プロジェクト平面への変換）
            XYZ projectedDirection = new XYZ(direction.X, direction.Y, 0).Normalize();

            // デフォルトの向きを取得
            XYZ defaultFacing = inst.FacingOrientation;

            // Z軸と計算した方向ベクトルのなす角を求める
            double angle = defaultFacing.AngleTo(projectedDirection);
            if (ClsGeo.IsLeft(basePoint, insertPointSt))
            {
                angle = -angle;
            }

            return angle;
        }

        private static double GetRotateAngle2(XYZ endpoint, XYZ startpoint, FamilyInstance inst)
        {
            // 基準点から挿入点へのベクトルを取得
            XYZ direction = endpoint - startpoint;

            // ベクトルをZ軸に対応する方向に変換（プロジェクト平面への変換）
            XYZ projectedDirection = new XYZ(direction.X, direction.Y, 0).Normalize();

            // デフォルトの向きを取得
            XYZ defaultFacing = inst.FacingOrientation;

            // Z軸と計算した方向ベクトルのなす角を求める
            double angle = defaultFacing.AngleOnPlaneTo(projectedDirection, XYZ.BasisZ);

            return angle;
        }

        private static XYZ GetInsertPoint_Oyakui(Document doc, FamilyInstance instOyakui, string oyakuiSize, ref XYZ centerPoint, bool isFront = true)
        {
            XYZ ptOyakui = instOyakui.GetTransform().Origin;
            XYZ vecOyakui = instOyakui.FacingOrientation;

            centerPoint = ptOyakui;

            double oyaHeight = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou1(oyakuiSize, 1)));
            XYZ ptOyakuiInsert = XYZ.Zero;
            if (isFront)
            {
                ptOyakuiInsert = ptOyakui + (vecOyakui * (oyaHeight / 2));
            }
            else
            {
                ptOyakuiInsert = ptOyakui - (vecOyakui * (oyaHeight / 2));
            }

            return ptOyakuiInsert;
        }

        public static XYZ GetInsertPoint_Kouyaita(Document doc, FamilyInstance instKouyaita, string kouyaitaSize, ref XYZ centerPoint)
        {
            // 鋼矢板の挿入点は中央ではなく端部

            XYZ ptKouyaita = instKouyaita.GetTransform().Origin;
            XYZ vecKouyaita = instKouyaita.FacingOrientation;
            XYZ vecOffset = XYZ.Zero;

            // 鋼矢板の向きを判定
            double tolerance = 0.0001; // 適切な許容範囲の値を設定してください
            int voidVec = ClsKouyaita.GetVoidvec(doc, instKouyaita.Id);
            if (voidVec == 1)
            {
                if (Math.Abs(vecKouyaita.X - 1) < tolerance && Math.Abs(vecKouyaita.Y) < tolerance && Math.Abs(vecKouyaita.Z) < tolerance)
                {
                    vecOffset = new XYZ(0, 1, 0);
                }
                else if (Math.Abs(vecKouyaita.X) < tolerance && Math.Abs(vecKouyaita.Y - 1) < tolerance && Math.Abs(vecKouyaita.Z) < tolerance)
                {
                    vecOffset = new XYZ(1, 0, 0);
                }
            }
            else
            {
                if (Math.Abs(vecKouyaita.X + 1) < tolerance && Math.Abs(vecKouyaita.Y) < tolerance && Math.Abs(vecKouyaita.Z) < tolerance)
                {
                    vecOffset = new XYZ(0, 1, 0);
                }
                else if (Math.Abs(vecKouyaita.X) < tolerance && Math.Abs(vecKouyaita.Y - 1) < tolerance && Math.Abs(vecKouyaita.Z) < tolerance)
                {
                    vecOffset = new XYZ(1, 0, 0);
                }
                else
                {
                    if (vecKouyaita.IsAlmostEqualTo(new XYZ(-1, 0, 0), tolerance))
                    {
                        vecOffset = new XYZ(0, 1, 0);
                    }
                    else if (vecKouyaita.IsAlmostEqualTo(new XYZ(0, -1, 0), tolerance))
                    {
                        vecOffset = new XYZ(-1, 0, 0);
                    }
                    else if (vecKouyaita.IsAlmostEqualTo(new XYZ(1, 0, 0), tolerance))
                    {
                        vecOffset = new XYZ(0, -1, 0);
                    }
                    else if (vecKouyaita.IsAlmostEqualTo(new XYZ(0, 1, 0), tolerance))
                    {
                        vecOffset = new XYZ(-1, 0, 0);
                    }
                }
            }

            // 鋼矢板は挿入点が端部のため、中央に補正する
            double oyaWidth = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(Master.ClsKouyaitaCsv.GetWidth(kouyaitaSize)));
            XYZ ptKouyaitaCenter = ptKouyaita + (vecOffset * (oyaWidth / 2));
            centerPoint = ptKouyaitaCenter;

            double oyaHeight = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(Master.ClsKouyaitaCsv.GetHeight(kouyaitaSize)));
            XYZ ptKouyaitaInsert = ptKouyaitaCenter + (vecKouyaita * oyaHeight);

            return ptKouyaitaInsert;
        }

        //public static XYZ GetInsertPoint_Kouyaita(Document doc, FamilyInstance instKouyaita, XYZ ptSt, XYZ ptEd, string kouyaitaSize)
        //{
        //    XYZ ptKouyaita = instKouyaita.GetTransform().Origin;
        //    XYZ vecKouyaita = instKouyaita.FacingOrientation;
        //    XYZ vec = XYZ.Zero;

        //    // 鋼矢板の向きを判定
        //    if (ClsKouyaita.GetVoidvec(doc, instKouyaita.Id) == 1)
        //    {
        //        // 壁側方向 (凸)
        //        vec = (ptEd - ptSt).Normalize();
        //    }
        //    else
        //    {
        //        // 穴側方向 (凹)
        //        vec = (ptSt - ptEd).Normalize();
        //    }

        //    // 鋼矢板は挿入点が端部のため、中央に補正する
        //    double oyaWidth = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(Master.ClsKouyaitaCsv.GetWidth(kouyaitaSize)));
        //    XYZ ptKouyaitaCenter = ptKouyaita + (vec * (oyaWidth / 2));

        //    double oyaHeight = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(Master.ClsKouyaitaCsv.GetHeight(kouyaitaSize)));
        //    XYZ ptKouyaitaInsert = ptKouyaitaCenter + (vecKouyaita * oyaHeight);

        //    return ptKouyaitaInsert;
        //}

        private static String GetKiribariBracketSizeWithAuto(string syuzaiSize)
        {
            switch (syuzaiSize)
            {
                case "20HA":
                case "25HA":
                case "30HA":
                case "35HA":
                    return "SBL";
                case "35SMH":
                    return "SBL";
                case "40HA":
                case "50HA":
                    return "HBL";
                case "40SMH":
                    return "HBL";
                case "60SMH":
                    return "TBL";
                default:
                    return string.Empty;
            }
        }


        public static string GetBracketSize(string haraokoshiSteelSize, int numV, int numH)
        {
            string res = string.Empty;

            res = ClsBracketSizeCsv.GetBracketSize(haraokoshiSteelSize, numV, numH);

            return res;
        }

        public static List<ElementId> GetAllHaraokoshiBracketList(Document doc)
        {
            List<ElementId> bracketIdList = ClsRevitUtil.GetSelectCreatedFamilyInstanceList(doc, "腹起BL", true);
            return bracketIdList;
        }

        /// <summary>
        /// 指定したブラケットを図面上から全て取得
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="levelId">レベル</param>
        /// <param name="type">ブラケット種類</param>
        /// <returns></returns>
        public static List<ElementId> GetSelectBracketList(Document doc, ElementId levelId, CreateType type)
        {
            string typeName;
            switch (type)
            {
                case CreateType.Kiribari:
                    typeName = "切梁BL";
                    break;
                case CreateType.kiribariOsae:
                    typeName = "切梁押えBL";
                    break;
                case CreateType.Haraokoshi:
                    typeName = "腹起BL";
                    break;
                case CreateType.HaraokoshiOsae:
                    typeName = "腹起押えBL";
                    break;
                default:
                    typeName = string.Empty;
                    break;
            }
            List<ElementId> bracketIdList = ClsRevitUtil.GetSelectCreatedFamilyInstanceList(doc, typeName);
            List<ElementId> bracketIdSelectLevelList = new List<ElementId>();
            //レベルを指定する
            foreach (ElementId id in bracketIdList)
            {
                Element el = doc.GetElement(id);
                ElementId level = el.LevelId;
                if (level == levelId)
                    bracketIdSelectLevelList.Add(id);
            }
            return bracketIdSelectLevelList;
        }

        /// <summary>
        /// 指定点に指定のブラケットが存在しているかのチェック
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="targetPoint">指定点</param>
        /// <param name="type">ブラケット種類</param>
        /// <returns></returns>
        public static bool CheckDuplicateBracket(Document doc, ElementId levelId, XYZ targetPoint, CreateType type)
        {
            //チェックするブラケットを図面上から全て取得する
            List<ElementId> targetBL = GetSelectBracketList(doc, levelId, type);
            foreach (ElementId id in targetBL)
            {
                FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                XYZ point = (inst.Location as LocationPoint).Point;
                if (ClsGeo.GEO_EQ(point, targetPoint))
                {
                    return false;
                }
            }
            return true;
        }
        public Curve GetOyakuiCurve(Document doc, ElementId oyakuiId, XYZ haraokoshiStartPoint, XYZ haraokoshiEndPoint, double searchRange)
        {
            // FamilyInstanceを取得
            FamilyInstance instTrgOyakui = doc.GetElement(oyakuiId) as FamilyInstance;
            if (instTrgOyakui == null)
            {
                throw new ArgumentException("The provided ElementId does not correspond to a FamilyInstance.");
            }

            string oyaSymName = instTrgOyakui.Symbol.FamilyName;

            // 開始点の初期化
            XYZ pSt = XYZ.Zero;
            if (oyaSymName.Contains("杭"))
            {
                pSt = instTrgOyakui.GetTotalTransform().Origin;
            }
            else if (oyaSymName.Contains("鋼矢板"))
            {
                if (ClsKouyaita.GetVoidvec(doc, instTrgOyakui.Id) == 1)
                {
                    // 壁側方向 (凸) の場合は処理を中断するか、適切な処理を追加します
                    // ここでは例としてnullを返します
                    return null;
                }
                //pSt = GetInsertPoint_Kouyaita(doc, instTrgOyakui, haraokoshiStartPoint, haraokoshiEndPoint, oyaSymName);
                XYZ centerPoint = XYZ.Zero;
                pSt = GetInsertPoint_Kouyaita(doc, instTrgOyakui, instTrgOyakui.Symbol.Family.Name, ref centerPoint);

            }

            // Z座標をゼロに設定
            pSt = new XYZ(pSt.X, pSt.Y, 0);
            XYZ pEd = pSt + (searchRange * instTrgOyakui.FacingOrientation.Normalize());
            pEd = new XYZ(pEd.X, pEd.Y, 0);

            // Curveを生成
            Curve cvOyakui = Line.CreateBound(pSt, pEd);
            return cvOyakui;
        }


        private static void CreateDebugLine(Document doc, XYZ sp, XYZ ep)
        {
            Curve line = Line.CreateBound(sp, ep);

            // デバッグ用の線分を表示
            using (Transaction transaction = new Transaction(doc, "Create Model Line"))
            {
                transaction.Start();

                // モデル線分を作成
                ElementId levelID = ClsRevitUtil.GetLevelID(doc, ClsKabeShin.GL);
                Plane plane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, sp);
                SketchPlane sketchPlane = SketchPlane.Create(doc, plane);
                ModelCurve modelLineF = doc.Create.NewModelCurve(line, sketchPlane);

                transaction.Commit();
            }
        }

    }
}
