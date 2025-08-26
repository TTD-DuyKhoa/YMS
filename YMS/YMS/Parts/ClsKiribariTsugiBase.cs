using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YMS.Parts
{
    public class ClsKiribariTsugiBase
    {
        #region 定数
        public const string baseName = "切梁継ぎベース";
        #endregion

        #region Enum
        /// <summary>
        /// 部材タイプ
        /// </summary>
        public enum PartsType
        {
            KiriKiri,
            KiriHara
        }

        /// <summary>
        /// 処理方法
        /// </summary>
        public enum ShoriType
        {
            Point,
            SanjikuPiece,
            Replace
        }

        /// <summary>
        /// 構成
        /// </summary>
        public enum Kousei
        {
            Single,
            Double
        }
        #endregion

        #region プロパティ
        /// <summary>
        /// 部材タイプ
        /// </summary>
        public PartsType m_PartsType { get; set; }

        /// <summary>
        ///鋼材タイプ
        /// </summary>
        public string m_SteelType { get; set; }

        /// <summary>
        /// 処理方法
        /// </summary>
        public ShoriType m_ShoriType { get; set; }

        /// <summary>
        /// 構成
        /// </summary>
        public Kousei m_Kousei { get; set; }

        /// <summary>
        /// 鋼材サイズ（ｼﾝｸﾞﾙ）
        /// </summary>
        public string m_SteelSizeSingle { get; set; }

        /// <summary>
        /// 切梁側：鋼材サイズ（ダブル）
        /// </summary>
        public string m_KiriSideSteelSizeDouble{ get; set; }

        /// <summary>
        /// 切梁側：切梁繋ぎ長さ
        /// </summary>
        public int m_KiriSideTsunagiLength { get; set; }

        /// <summary>
        /// 切梁側：切梁側部品
        /// </summary>
        public string m_KiriSideParts { get; set; }

        /// <summary>
        /// 腹起側：鋼材サイズ（ダブル）
        /// </summary>
        public string m_HaraSideSteelSizeDouble { get; set; }

        /// <summary>
        /// 腹起側：切梁繋ぎ長さ
        /// </summary>
        public int m_HaraSideTsunagiLength { get; set; }

        /// <summary>
        /// 腹起側：腹起側部品
        /// </summary>
        public string m_HaraSideParts { get; set; }

        /// <summary>
        /// 編集用：フロア
        /// </summary>
        public string m_Floor { get; set; }

        /// <summary>
        /// 編集用：エレメントID
        /// </summary>
        public ElementId m_ElementId { get; set; }

        /// <summary>
        /// 編集用：段
        /// </summary>
        public string m_Dan { get; set; }

        /// <summary>
        /// 編集用：編集フラグ
        /// </summary>
        public bool m_FlgEdit { get; set; }
        #endregion

        #region コンストラクタ
        public ClsKiribariTsugiBase()
        {
            //初期化
            Init();
        }
        #endregion

        #region メソッド
        /// <summary>
        /// 初期化
        /// </summary>
        public void Init()
        {
            m_PartsType = PartsType.KiriHara;
            m_SteelType = String.Empty;
            m_ShoriType = ShoriType.Replace;
            m_Kousei = Kousei.Single;
            m_SteelSizeSingle = string.Empty;
            m_ShoriType = ShoriType.Point;
            m_KiriSideSteelSizeDouble = string.Empty;
            m_KiriSideTsunagiLength = 0;
            m_KiriSideParts = String.Empty;
            m_HaraSideSteelSizeDouble = String.Empty;
            m_HaraSideTsunagiLength = 0;
            m_HaraSideParts = String.Empty;
            m_Floor = string.Empty;
            m_Dan = string.Empty;
            m_FlgEdit = false;
        }

        public bool CreateKiribariTsugiBase(Document doc, List<ElementId> modelLineIdList, List<string> danList, ElementId levelID)
        {
            string symbolFolpath = ClsZumenInfo.GetYMSFolder();
            string shinfamily = System.IO.Path.Combine(symbolFolpath, "ベース関係\\" + baseName + ".rfa");
            //シンボル読込
            if (!ClsRevitUtil.LoadFamilySymbolData(doc, shinfamily, baseName, out FamilySymbol sym))
            {
                return false;
            }

            int n = 0;
            foreach (ElementId modelLineId in modelLineIdList)
            {
                ModelLine modelLine = doc.GetElement(modelLineId) as ModelLine;
                LocationCurve lCurve = modelLine.Location as LocationCurve;
                if (lCurve == null)
                {
                    continue;
                }
                Curve cv = lCurve.Curve;

                using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                {
                    t.Start();
                    ElementId CreatedID = ClsRevitUtil.Create(doc, cv, levelID, sym);
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "段", danList[n]);
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "鋼材タイプ", m_SteelType);
                    if (m_Kousei == Kousei.Double)
                    {
                        ClsRevitUtil.SetMojiParameter(doc, CreatedID, "構成", "ダブル");
                        ClsRevitUtil.ChangeLineColor(doc, CreatedID, ClsGlobal.m_redColor);
                        ClsRevitUtil.SetMojiParameter(doc, CreatedID, "切梁側/鋼材サイズ(ダブル)", m_KiriSideSteelSizeDouble);
                        ClsRevitUtil.SetParameter(doc, CreatedID, "切梁側/切梁繋ぎ長さ", ClsRevitUtil.CovertToAPI(m_KiriSideTsunagiLength));
                        ClsRevitUtil.SetMojiParameter(doc, CreatedID, "切梁側/部品", m_KiriSideParts);
                        ClsRevitUtil.SetMojiParameter(doc, CreatedID, "腹起側/鋼材サイズ(ダブル)", m_HaraSideSteelSizeDouble);
                        ClsRevitUtil.SetParameter(doc, CreatedID, "腹起側/切梁繋ぎ長さ", ClsRevitUtil.CovertToAPI(m_HaraSideTsunagiLength));
                        ClsRevitUtil.SetMojiParameter(doc, CreatedID, "腹起側/部品", m_HaraSideParts);
                    }
                    else
                    {
                        ClsRevitUtil.SetMojiParameter(doc, CreatedID, "構成", "シングル");
                        ClsRevitUtil.SetMojiParameter(doc, CreatedID, "鋼材サイズ(シングル)", m_SteelSizeSingle);
                        ClsRevitUtil.SetMojiParameter(doc, CreatedID, "切梁側/部品", m_KiriSideParts);
                        ClsRevitUtil.SetMojiParameter(doc, CreatedID, "腹起側/部品", m_HaraSideParts);
                    }
                    n++;
                    t.Commit();
                }
            }
            return true;
        }
        public bool CreateKiribariTsugiBase(Document doc, XYZ tmpStPoint, XYZ tmpEdPoint, string dan, ElementId levelID)
        {
            string symbolFolpath = ClsZumenInfo.GetYMSFolder();
            string shinfamily = System.IO.Path.Combine(symbolFolpath, "ベース関係\\" + baseName + ".rfa");
            //シンボル読込
            if (!ClsRevitUtil.LoadFamilySymbolData(doc, shinfamily, baseName, out FamilySymbol sym))
            {
                return false;
            }

            Curve cv = Line.CreateBound(tmpStPoint, tmpEdPoint);

            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                t.Start();
                ElementId CreatedID = ClsRevitUtil.Create(doc, cv, levelID, sym);
                ClsRevitUtil.SetMojiParameter(doc, CreatedID, "段", dan);
                ClsRevitUtil.SetMojiParameter(doc, CreatedID, "鋼材タイプ", m_SteelType);
                if (m_Kousei == Kousei.Double)
                {
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "構成", "ダブル");
                    ClsRevitUtil.ChangeLineColor(doc, CreatedID, ClsGlobal.m_redColor);
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "切梁側/鋼材サイズ(ダブル)", m_KiriSideSteelSizeDouble);
                    ClsRevitUtil.SetParameter(doc, CreatedID, "切梁側/切梁繋ぎ長さ", ClsRevitUtil.CovertToAPI(m_KiriSideTsunagiLength));
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "切梁側/部品", m_KiriSideParts);
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "腹起側/鋼材サイズ(ダブル)", m_HaraSideSteelSizeDouble);
                    ClsRevitUtil.SetParameter(doc, CreatedID, "腹起側/切梁繋ぎ長さ", ClsRevitUtil.CovertToAPI(m_HaraSideTsunagiLength));
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "腹起側/部品", m_HaraSideParts);
                }
                else
                {
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "構成", "シングル");
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "鋼材サイズ(シングル)", m_SteelSizeSingle);
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "切梁側/部品", m_KiriSideParts);
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "腹起側/部品", m_HaraSideParts);
                }
                t.Commit();
            }
            return true;
        }

        public bool CreateKiribariTsugiBase(Document doc, XYZ tmpStPoint, XYZ tmpEdPoint)
        {
            ElementId levelID = ClsRevitUtil.GetLevelID(doc, m_Floor);
            if (levelID == null)
            {
                return false;
            }

            string symbolFolpath = ClsZumenInfo.GetYMSFolder();
            string shinfamily = System.IO.Path.Combine(symbolFolpath, "ベース関係\\" + baseName + ".rfa");
            //シンボル読込
            if (!ClsRevitUtil.LoadFamilySymbolData(doc, shinfamily, baseName, out FamilySymbol sym))
            {
                return false;
            }

            Curve cv = Line.CreateBound(tmpStPoint, tmpEdPoint);

            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                t.Start();
                FailureHandlingOptions failOpt = t.GetFailureHandlingOptions();
                failOpt.SetFailuresPreprocessor(new WarningSwallower());
                t.SetFailureHandlingOptions(failOpt);
                ElementId CreatedID = ClsRevitUtil.Create(doc, cv, levelID, sym);
                ClsRevitUtil.SetMojiParameter(doc, CreatedID, "段", m_Dan);
                ClsRevitUtil.SetMojiParameter(doc, CreatedID, "鋼材タイプ", m_SteelType);
                if (m_Kousei == Kousei.Double)
                {
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "構成", "ダブル");
                    ClsRevitUtil.ChangeLineColor(doc, CreatedID, ClsGlobal.m_redColor);
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "切梁側/鋼材サイズ(ダブル)", m_KiriSideSteelSizeDouble);
                    ClsRevitUtil.SetParameter(doc, CreatedID, "切梁側/切梁繋ぎ長さ", ClsRevitUtil.CovertToAPI(m_KiriSideTsunagiLength));
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "切梁側/部品", m_KiriSideParts);
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "腹起側/鋼材サイズ(ダブル)", m_HaraSideSteelSizeDouble);
                    ClsRevitUtil.SetParameter(doc, CreatedID, "腹起側/切梁繋ぎ長さ", ClsRevitUtil.CovertToAPI(m_HaraSideTsunagiLength));
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "腹起側/部品", m_HaraSideParts);
                }
                else
                {
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "構成", "シングル");
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "鋼材サイズ(シングル)", m_SteelSizeSingle);
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "切梁側/部品", m_KiriSideParts);
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "腹起側/部品", m_HaraSideParts);
                }
                t.Commit();
            }
            return true;
        }
        /// <summary>
        /// 端部部品の作図（ベースのエレメントIDが必須）
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public bool CreateTanbuParts(Document doc, ref XYZ pS, ref XYZ pE)//自在火打ち対応
        {
            ElementId baseID = m_ElementId;
            if (baseID == null)
            {
                return false;
            }

            bool BaseEndIsKiribaribase = false;

            string steelSizeS, steelSizeE;
            if (m_Kousei == Kousei.Double)
            {
                steelSizeS = m_KiriSideSteelSizeDouble;
                steelSizeE = m_HaraSideSteelSizeDouble;
            }
            else
            {
                steelSizeS = m_SteelSizeSingle;
                steelSizeE = m_SteelSizeSingle;
            }
            //ベース
            FamilyInstance ShinInstLevel = doc.GetElement(baseID) as FamilyInstance;

            Element inst = doc.GetElement(baseID);
            LocationCurve lCurve = inst.Location as LocationCurve;
            if (lCurve == null)
            {
                return false;
            }

            XYZ tmpStPoint = lCurve.Curve.GetEndPoint(0);
            XYZ tmpEdPoint = lCurve.Curve.GetEndPoint(1);
            XYZ vecSE = (tmpEdPoint - tmpStPoint).Normalize();
            XYZ vecES = (tmpStPoint - tmpEdPoint).Normalize();

            //始端側の梁と終点側の梁を選別
            List<ElementId> lstHari = YMS.ClsYMSUtil.GetIntersectionHaraKiriBase(doc, baseID);
            if (lstHari.Count < 2)
                return false;

            ElementId idA = null;//始点側に交差する切梁ベース
            ElementId idB = null;//終点側に交差する腹起しベースor切梁ベース
            double dSMin = 0;
            double dEMin = 0;
            foreach (ElementId id in lstHari)
            {
                Element el = doc.GetElement(id);
                LocationCurve lCurve2 = el.Location as LocationCurve;
                XYZ p = ClsRevitUtil.GetIntersection(lCurve.Curve, lCurve2.Curve);

                double dS = tmpStPoint.DistanceTo(p);
                double dE = tmpEdPoint.DistanceTo(p);
                if (idA == null || dSMin > dS)
                {
                    idA = id;
                    dSMin = dS;
                }

                if (idB == null || dEMin > dE)
                {
                    idB = id;
                    dEMin = dE;
                }
            }

            if (idB != null)
            {
                //#31698
                Element elem = doc.GetElement(idB) as Element;
                if (elem.Name == "切梁ベース")
                {
                    BaseEndIsKiribaribase = true;
                }
            }

            //梁（始点側）
            Element elA = doc.GetElement(idA);
            LocationCurve lCurveA = elA.Location as LocationCurve;
            if (lCurveA == null)
            {
                return false;
            }

            string danA = ClsRevitUtil.GetParameter(doc, idA, "段");
            XYZ tmpStPointA = lCurveA.Curve.GetEndPoint(0);
            XYZ tmpEdPointA = lCurveA.Curve.GetEndPoint(1);
            XYZ vecSEA = (tmpEdPointA - tmpStPointA).Normalize();


            //梁（終点側）
            Element elB = doc.GetElement(idB);
            LocationCurve lCurveB = elB.Location as LocationCurve;
            if (lCurveB == null)
            {
                return false;
            }

            string danB = ClsRevitUtil.GetParameter(doc, idB, "段");
            XYZ tmpStPointB = lCurveB.Curve.GetEndPoint(0);
            XYZ tmpEdPointB = lCurveB.Curve.GetEndPoint(1);
            XYZ vecSEB = (tmpEdPointB - tmpStPointB).Normalize();

            //角度
            double dHaraAngle = ClsRevitUtil.CalculateAngleBetweenLines(tmpStPointA, tmpEdPointA, tmpEdPointB, tmpStPointB);
            double dAngleA = ClsRevitUtil.CalculateAngleBetweenLines(tmpStPoint, tmpEdPoint, tmpEdPointA, tmpStPointA);
            double dAngleB = ClsRevitUtil.CalculateAngleBetweenLines(tmpStPoint, tmpEdPoint, tmpEdPointB, tmpStPointB);
            //if (dAngleB < 90.0)
            //    dAngleB = 180.0 - dAngleB;


            //挿入位置を取得
            double dA = 0;
            double dB = 0;
            double dMain = 0;
            GetTotalLength(dAngleA, dAngleB, BaseEndIsKiribaribase, ref dMain, ref dA, ref dB);
            pS = tmpStPoint + (vecSE * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(dA));
            pE = tmpEdPoint + (-vecSE * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(dB));


            //■■■共通処理■■■
            FamilyInstance lv = doc.GetElement(baseID) as FamilyInstance;
            ElementId levelID = lv.Host.Id;
            ClsYMSUtil.GetDifferenceWithAllBase(doc, baseID, out double diff, out double diff2);

            FamilySymbol sym, sym2, sym3 = null;//自在火打ち対応
            string shinfamily = Master.ClsHiuchiCsv.GetFamilyPath(m_KiriSideParts, steelSizeS);
            string shinfamilyName = RevitUtil.ClsRevitUtil.GetFamilyName(shinfamily);
            string tanbuPieceSize = Master.ClsHiuchiCsv.GetSize(m_KiriSideParts, steelSizeS);
            double sizeA = ClsRevitUtil.CovertToAPI(Master.ClsHiuchiCsv.GetWidth(m_KiriSideParts, tanbuPieceSize) / 2) + diff;
            double atumiSHG = -ClsRevitUtil.CovertToAPI(12.0);
            //シンボル読込※タイプを持っている
            if (m_KiriSideParts == Master.ClsHiuchiCsv.TypeNameFVP || m_KiriSideParts == Master.ClsHiuchiCsv.TypeNameKaitenVP)
            {
                //↑ここには入らないのでは？
                if (!ClsRevitUtil.LoadFamilyData(doc, shinfamily, out Family tanbuFam))
                {
                    //return false; ;
                }
                sym = (ClsRevitUtil.GetFamilySymbol(doc, shinfamilyName, "隅火打"));
            }
            else
            {
                if (!ClsRevitUtil.LoadFamilySymbolData(doc, shinfamily, shinfamilyName, out sym))
                {
                    //return false;
                }
            }

            string shinfamily2 = Master.ClsHiuchiCsv.GetFamilyPath(m_HaraSideParts, steelSizeE);
            string shinfamilyName2 = RevitUtil.ClsRevitUtil.GetFamilyName(shinfamily2);
            string tanbuPieceSize2 = Master.ClsHiuchiCsv.GetSize(m_HaraSideParts, steelSizeE);
            double sizeB = ClsRevitUtil.CovertToAPI(Master.ClsHiuchiCsv.GetWidth(m_HaraSideParts, tanbuPieceSize2) / 2) + diff2;
            if (m_HaraSideParts == Master.ClsHiuchiCsv.TypeNameFVP || m_HaraSideParts == Master.ClsHiuchiCsv.TypeNameKaitenVP)
            {
                //自在火打ちの処理は保留
                if (!ClsRevitUtil.LoadFamilyData(doc, shinfamily2,  out Family tanbuFam))
                {
                    //return false; ;
                }
                sym2 = (ClsRevitUtil.GetFamilySymbol(doc, shinfamilyName2, "隅火打"));//←　これは仕様

                //自在火打ち対応 -start
                string buruFamPath = Master.ClsBurumanCSV.GetFamilyPath("C-50");
                string buruFamName = RevitUtil.ClsRevitUtil.GetFamilyName(buruFamPath);
                if (!ClsRevitUtil.LoadFamilyData(doc, buruFamPath,out Family buruFam))
                {
                    //return false; ;
                }
                sym3 = (ClsRevitUtil.GetFamilySymbol(doc, buruFamName, "自在受けピース(FVP)"));
                //自在火打ち対応 -end
            }
            else
            {
                //シンボル読込
                if (!ClsRevitUtil.LoadFamilySymbolData(doc, shinfamily2, shinfamilyName2, out sym2))
                {
                    //return false;
                }
            }
            using (Transaction tx = new Transaction(doc, "Load Family"))
            {
                tx.Start();


                //■■■始点側処理■■■
                if (sym != null)
                {
                    if (m_KiriSideParts == Master.ClsHiuchiCsv.TypeNameFVP)
                    {
                        //自在火打ち対応 -start
                        //自在火打の挿入・位置調整・回転
                        ElementId CreatedID = ClsRevitUtil.Create(doc, tmpStPoint, levelID, sym);
                        ClsKariKouzai.SetKariKouzaiFlag(doc, CreatedID);
                        FamilyInstance ch = doc.GetElement(CreatedID) as FamilyInstance;
                        XYZ dirV = new XYZ(1, 0, 0);//ファミリインスタンスの向き
                        double dVectAngle = dirV.AngleTo(vecSEA);
                        Line axis = Line.CreateBound(tmpStPoint, tmpStPoint + XYZ.BasisZ);
                        ch.flipFacing();

                        if (!ClsGeo.IsLeft(dirV, vecSEA))
                        {
                            dVectAngle = -dVectAngle;
                        }

                        XYZ axisDirection = ClsRevitUtil.RotateVector(dirV, -Math.PI);
                        Plane plane = Plane.CreateByThreePoints(tmpStPoint + new XYZ(0, 1, 0), tmpStPoint + new XYZ(0, 0, 1), tmpStPoint);

                        //RevitUtil.ClsRevitUtil.MirrorElement(doc, CreatedID, plane);
                        //ElementTransformUtils.MoveElement(doc, CreatedID, tmpStPoint - ((LocationPoint)ch.Location).Point);
                        //double dW = RevitUtil.ClsRevitUtil.CovertToAPI(Master.ClsYamadomeCsv.GetWidth(m_SteelSize));
                        double dW = RevitUtil.ClsRevitUtil.CovertToAPI(ClsYMSUtil.GetKouzaiHabaFromBase(doc, baseID));
                        XYZ dirMove = new XYZ(-dW / 2, 0, 0);//移動長さ
                        ElementTransformUtils.MoveElement(doc, CreatedID, dirMove);
                        ClsRevitUtil.RotateElement(doc, CreatedID, axis, dVectAngle);

                        //ブルマンの挿入位置を算出
                        XYZ direction = vecSE.Normalize(); // 方向ベクトルを正規化
                        XYZ leftDirection = new XYZ(-direction.Y, direction.X, 0); // 左側方向に90度回転
                        XYZ moveVector = leftDirection * (dW / 2) * 0.8;
                        Curve cvBase = Line.CreateBound(tmpStPoint + moveVector, tmpEdPoint + moveVector);
                        XYZ direction2 = vecSEA.Normalize(); // 方向ベクトルを正規化
                        XYZ leftDirection2 = new XYZ(-direction2.Y, direction2.X, 0); // 左側方向に90度回転
                        string target = Master.ClsHiuchiCsv.GetTargetShuzai(tanbuPieceSize);
                        double dFVPWidth = RevitUtil.ClsRevitUtil.CovertToAPI(Master.ClsYamadomeCsv.GetWidth(target));
                        XYZ moveVector2 = leftDirection2 * dFVPWidth;
                        Curve cvHariS = Line.CreateBound(tmpStPointA + moveVector2, tmpEdPointA + moveVector2);
                        XYZ pBuru = RevitUtil.ClsRevitUtil.GetIntersection(cvBase, cvHariS);

                        //ブルマンの挿入
                        ElementId CreatedID_B = ClsRevitUtil.Create(doc, pBuru, levelID, sym3);
                        ClsKariKouzai.SetKariKouzaiFlag(doc, CreatedID_B);
                        Line axis2 = Line.CreateBound(pBuru, pBuru + XYZ.BasisZ);
                        ClsRevitUtil.RotateElement(doc, CreatedID_B, axis2, dVectAngle + ClsGeo.Deg2Rad(-30));

                        //上下の位置調整
                        if (danA == "上段")
                        {
                            ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", sizeA);
                            ClsRevitUtil.SetParameter(doc, CreatedID_B, "基準レベルからの高さ", diff);
                        }
                        else if (danA == "下段")
                        {
                            ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", -sizeA);
                            ClsRevitUtil.SetParameter(doc, CreatedID_B, "基準レベルからの高さ", -sizeA * 2);
                        }
                        else
                        {
                            ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", diff);
                            ClsRevitUtil.SetParameter(doc, CreatedID_B, "基準レベルからの高さ", -sizeA);
                        }
                        //自在火打ち対応 -end
                    }
                    else
                    {
                        double dVectAngle = XYZ.BasisX.AngleOnPlaneTo(vecSE, XYZ.BasisZ);
                        Line axis = Line.CreateBound(tmpStPoint, tmpStPoint + XYZ.BasisZ);

                        //上
                        ElementId CreatedID = ClsRevitUtil.Create(doc, tmpStPoint, levelID, sym);
                        ClsRevitUtil.RotateElement(doc, CreatedID, axis, dVectAngle);
                        if (danA == "上段")
                            ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", sizeA * 2 - diff);
                        else if (danA == "下段")
                            ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", diff);
                        else
                            ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", sizeA);

                        //下
                        CreatedID = ClsRevitUtil.Create(doc, tmpStPoint, levelID, sym);
                        ClsRevitUtil.RotateElement(doc, CreatedID, axis, dVectAngle);
                        if (danA == "上段")
                            ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", atumiSHG + diff);
                        else if (danA == "下段")
                            ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", -sizeA * 2 + atumiSHG);
                        else
                            ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", -sizeA + atumiSHG);
                    }
                }

                //■■■終点側処理■■■

                if (BaseEndIsKiribaribase)
                {
                    if (sym != null)
                    {
                        //#31698
                        double dVectAngle = XYZ.BasisX.AngleOnPlaneTo(vecES, XYZ.BasisZ);
                        Line axis = Line.CreateBound(tmpEdPoint, tmpEdPoint + XYZ.BasisZ);

                        //上
                        ElementId CreatedID = ClsRevitUtil.Create(doc, tmpEdPoint, levelID, sym);
                        ClsKariKouzai.SetKariKouzaiFlag(doc, CreatedID);
                        ClsRevitUtil.RotateElement(doc, CreatedID, axis, dVectAngle);
                        if (danA == "上段")
                            ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", sizeA * 2 - diff);
                        else if (danA == "下段")
                            ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", diff);
                        else
                            ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", sizeA);

                        //下
                        CreatedID = ClsRevitUtil.Create(doc, tmpEdPoint, levelID, sym);
                        ClsKariKouzai.SetKariKouzaiFlag(doc, CreatedID);
                        ClsRevitUtil.RotateElement(doc, CreatedID, axis, dVectAngle);
                        if (danA == "上段")
                            ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", atumiSHG + diff);
                        else if (danA == "下段")
                            ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", -sizeA * 2 + atumiSHG);
                        else
                            ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", -sizeA + atumiSHG);
                    }
                }
                else
                {
                    if (sym2 != null)
                    {
                        if (m_HaraSideParts == Master.ClsHiuchiCsv.TypeNameFVP)
                        {
                            ElementId CreatedID = ClsRevitUtil.Create(doc, tmpEdPoint, levelID, sym2);
                            ClsKariKouzai.SetKariKouzaiFlag(doc, CreatedID);
                            FamilyInstance ch = doc.GetElement(CreatedID) as FamilyInstance;

                            XYZ dirV = new XYZ(-1, 0, 0);//ファミリインスタンスの向き
                            double dVectAngle = dirV.AngleTo(vecSEB);
                            Line axis = Line.CreateBound(tmpEdPoint, tmpEdPoint + XYZ.BasisZ);

                            if (!ClsGeo.IsLeft(dirV, vecSEB))
                            {
                                dVectAngle = -dVectAngle;
                            }

                            Plane plane = Plane.CreateByThreePoints(tmpEdPoint + new XYZ(0, 1, 0), tmpEdPoint + new XYZ(0, 0, 1), tmpEdPoint);

                            //RevitUtil.ClsRevitUtil.MirrorElement(doc, CreatedID, plane);
                            //ElementTransformUtils.MoveElement(doc, CreatedID, tmpEdPoint - ((LocationPoint)ch.Location).Point);
                            //double dW = RevitUtil.ClsRevitUtil.CovertToAPI(Master.ClsYamadomeCsv.GetWidth(m_SteelSize));
                            double dW = RevitUtil.ClsRevitUtil.CovertToAPI(ClsYMSUtil.GetKouzaiHabaFromBase(doc, baseID));
                            if (m_Kousei == Kousei.Double) dW = RevitUtil.ClsRevitUtil.CovertToAPI(ClsYMSUtil.GetKouzaiHabaFromBase(doc, baseID, bDoubleUNorHara: true));
                            XYZ dirMove = new XYZ(-dW / 2, 0, 0);//移動長さ
                            ElementTransformUtils.MoveElement(doc, CreatedID, dirMove);

                            ClsRevitUtil.RotateElement(doc, CreatedID, axis, dVectAngle);

                            //ブルマンの挿入位置を算出
                            XYZ direction = vecSE.Normalize(); // 方向ベクトルを正規化
                            XYZ leftDirection = new XYZ(-direction.Y, direction.X, 0); // 左側方向に90度回転
                            XYZ moveVector = leftDirection * (dW / 2) * 0.8;
                            Curve cvBase = Line.CreateBound(tmpStPoint + moveVector, tmpEdPoint + moveVector);
                            XYZ direction2 = vecSEB.Normalize(); // 方向ベクトルを正規化
                            XYZ leftDirection2 = new XYZ(-direction2.Y, direction2.X, 0); // 左側方向に90度回転
                            string target = Master.ClsHiuchiCsv.GetTargetShuzai(tanbuPieceSize2);
                            double dFVPWidth = RevitUtil.ClsRevitUtil.CovertToAPI(Master.ClsYamadomeCsv.GetWidth(target));
                            XYZ moveVector2 = leftDirection2 * dFVPWidth;
                            Curve cvHariS = Line.CreateBound(tmpStPointB + moveVector2, tmpEdPointB + moveVector2);
                            XYZ pBuru = RevitUtil.ClsRevitUtil.GetIntersection(cvBase, cvHariS);

                            //ブルマンの挿入
                            ElementId CreatedID_B = ClsRevitUtil.Create(doc, pBuru, levelID, sym3);
                            ClsKariKouzai.SetKariKouzaiFlag(doc, CreatedID_B);
                            Line axis2 = Line.CreateBound(pBuru, pBuru + XYZ.BasisZ);
                            ClsRevitUtil.RotateElement(doc, CreatedID_B, axis2, dVectAngle + ClsGeo.Deg2Rad(30));

                            if (danB == "上段")
                            {
                                ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", sizeB);
                                ClsRevitUtil.SetParameter(doc, CreatedID_B, "基準レベルからの高さ", diff);
                            }
                            else if (danB == "下段")
                            {
                                ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", -sizeB);
                                ClsRevitUtil.SetParameter(doc, CreatedID_B, "基準レベルからの高さ", -sizeB * 2);
                            }
                            else
                            {
                                ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", diff);
                                ClsRevitUtil.SetParameter(doc, CreatedID_B, "基準レベルからの高さ", -sizeB);
                            }
                        }
                        else
                        {
                            ElementId CreatedID = ClsRevitUtil.Create(doc, pE, levelID, sym2);
                            ClsKariKouzai.SetKariKouzaiFlag(doc, CreatedID);
                            FamilyInstance ch = doc.GetElement(CreatedID) as FamilyInstance;
                            double filpF = 0.0;

                            XYZ dirV = ch.FacingOrientation;// new XYZ(-1, 0, 0);//ファミリインスタンスの向き
                            XYZ dirF = ch.HandOrientation;
                            double dVectAngle = 0;// dirV.AngleTo(vecSE);// + filpF;
                            double dFacAngle = dirF.AngleTo(vecSEB);
                            Line axis = Line.CreateBound(pE, pE + XYZ.BasisZ);

                            if (!ClsGeo.IsLeft(vecSEB, pE))
                            {
                                dFacAngle = -dFacAngle;
                            }
                            ClsRevitUtil.RotateElement(doc, CreatedID, axis, -dVectAngle - dFacAngle - filpF);
                            if (dAngleB < 90.0)
                            {
                                //ch.flipFacing();
                                ch.flipHand();
                                //filpF = Math.PI;
                            }
                            if (danB == "上段")
                                ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", sizeB);
                            else if (danB == "下段")
                                ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", -sizeB);
                            else
                                ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", diff);
                        }
                    }
                }


                tx.Commit();
            }

            return true;
        }
        /// <summary>
        /// 切梁継ぎの全長取得処理
        /// </summary>
        /// <param name="dHaraAngle">腹起間角度</param>
        /// <returns></returns>
        public double GetTotalLength(double dStartAngle, double dEndAngle, bool BaseEndIsKiribaribase, ref double dMain, ref double dOffsetA, ref double dOffsetB)
        {
            //切梁-切梁の場合は端部部品の厚みが必ず0の為終了
            if (BaseEndIsKiribaribase)
            {
                return 0.0;
            }
            double dTotalLength = 0;//切梁の長さ
            double d = 0.0;
            double d2 = 0.0;

            double dAngleA = 0;
            double dAngleB = 0;
            double dKouzaiWidthA = 0;
            double dKouzaiWidthB = 0;
            double dPartsThicknessA = 0;
            double dPartsThicknessB = 0;

            string steelSizeS, steelSizeE;
            if (m_Kousei == Kousei.Double)
            {
                steelSizeS = m_KiriSideSteelSizeDouble;
                steelSizeE = m_HaraSideSteelSizeDouble;
            }
            else
            {
                steelSizeS = m_SteelSizeSingle;
                steelSizeE = m_SteelSizeSingle;
            }
            //角度
            //if (m_HiuchiAngle == HiuchiAngle.Nini)
            //{
            //    dAngleA = m_angle;
            //    dAngleB = 180 - dHaraAngle - m_angle;
            //}
            //else
            //{
            //    dAngleA = ClsGeo.FloorAtDigitAdjust(1, (180 - dHaraAngle) / 2);
            //    dAngleB = dAngleA;
            //}
            dAngleA = dStartAngle;
            dAngleB = dEndAngle;
            //鋼材幅
            //if (m_KiriSideParts != Master.ClsHiuchiCsv.TypeNameFVP)
            //{
            //    //string targetShuzai = Master.ClsHiuchiCsv.GetTargetShuzai(m_HiuchiUkePieceSize1);
            //    dKouzaiWidthA = Master.ClsYamadomeCsv.GetWidth(steelSizeS);
            //}
            //else
            //{
            //    dKouzaiWidthA = Master.ClsYamadomeCsv.GetWidth(steelSizeS);

            //}
            //dPartsThicknessA = Master.ClsHiuchiCsv.GetThickness(m_KiriSideParts, steelSizeS);

            if (BaseEndIsKiribaribase)
            {
                //dKouzaiWidthB = Master.ClsYamadomeCsv.GetWidth(steelSizeE);
                //dPartsThicknessB = Master.ClsHiuchiCsv.GetThickness(m_KiriSideParts, steelSizeE);
            }
            else
            {
                if (m_HaraSideParts != Master.ClsHiuchiCsv.TypeNameFVP)
                {
                    //string targetShuzai = Master.ClsHiuchiCsv.GetTargetShuzai(m_HiuchiUkePieceSize2);
                    dKouzaiWidthB = Master.ClsYamadomeCsv.GetWidth(steelSizeE);
                }
                else
                {
                    dKouzaiWidthB = Master.ClsYamadomeCsv.GetWidth(steelSizeE);

                }
                dPartsThicknessB = Master.ClsHiuchiCsv.GetThickness(m_HaraSideParts, steelSizeE);
            }
            
            //double dA = ClsYMSUtil.CalculateOffsets(dAngleA, dKouzaiWidthA, dPartsThicknessA);
            double dB = ClsYMSUtil.CalculateOffsets(dAngleB, dKouzaiWidthB, dPartsThicknessB);

            dMain = dTotalLength;
            //dOffsetA = dA;
            dOffsetB = dB;

            return ClsGeo.FloorAtDigitAdjust(0, dTotalLength + dOffsetA + dOffsetB);
        }
        /// <summary>
        /// 指定のElementと接するベースを探す
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="cv">指定のElement</param>
        /// <returns>指定のElementと接するもの</returns>
        public List<ElementId> FindBaseToIntersection(Document doc, Curve cv)
        {
            List<ElementId> insecList = new List<ElementId>();

            List<ElementId> haraIdList = ClsHaraokoshiBase.GetAllHaraokoshiBaseList(doc);
            foreach (ElementId haraId in haraIdList)
            {
                FamilyInstance haraokoshiShinInst = doc.GetElement(haraId) as FamilyInstance;
                LocationCurve lCurveHara = haraokoshiShinInst.Location as LocationCurve;
                if (lCurveHara != null)
                {
                    Curve cvHara = lCurveHara.Curve;
                    XYZ insec = ClsRevitUtil.GetIntersection(cv, cvHara);
                    if (insec != null)
                    {
                        insecList.Add(haraId);
                    }
                }
            }

            return insecList;
        }

        /// <summary>
        /// 選択された切梁ベースor三軸ピース-腹起ベースor切梁ベースの段設定と構成が一致するか
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="element">切梁ベースor三軸ピース</param>
        /// <param name="partner">腹起ベースor切梁ベース</param>
        /// <returns></returns>
        public bool CheckDan(Document doc, ElementId element, ElementId partner)
        {
            string danElement = ClsRevitUtil.GetParameter(doc, element, "段");
            string danHara = ClsRevitUtil.GetParameter(doc, partner, "段");
            if(danElement == danHara)
            {
                if(m_Kousei == Kousei.Double)
                {
                    MessageBox.Show("同段において構成ダブルが選択されています。");
                    return false;
                }
            }
            else
            {
                if (m_Kousei == Kousei.Single)
                {
                    MessageBox.Show("異段において構成シングルが選択されています。");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// ダブルで設定した切梁継ぎ長さが実寸を超えていないかのチェック
        /// </summary>
        /// <param name="exSize">実寸</param>
        /// <returns></returns>
        public bool CheckLength(double exSize)
        {
            if(exSize < m_KiriSideTsunagiLength)
            {
                MessageBox.Show("切梁側の切梁継ぎ長さが" + exSize.ToString() + "を超えています。");
                return false;
            }
            if(exSize < m_HaraSideTsunagiLength)
            {
                MessageBox.Show("腹起側の切梁継ぎ長さが" + exSize.ToString() + "を超えています。");
                return false;
            }
            return true;
        }
        /// <summary>
        /// ダブルで設定した切梁継ぎ、腹起側の切梁継ぎ長さが交差するかのチェック
        /// </summary>
        /// <param name="exSize">実寸</param>
        /// <returns></returns>
        public bool CheckIntersectionLength(double exSize)
        {
            if (m_KiriSideTsunagiLength + m_HaraSideTsunagiLength < exSize)
            {
                MessageBox.Show("切梁側の切梁継ぎと腹起側の切梁継ぎが交差しません。合計" + exSize.ToString() + "を超えて下さい");
                return false;
            }
            return true;
        }
        /// <summary>
        /// 切梁継ぎベース のみを単独選択
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="id">選択した 切梁継ぎベース のID</param>
        /// <param name="message">指示</param>
        /// <returns></returns>
        public static bool PickBaseObject(UIDocument uidoc, ref ElementId id, string message = baseName)
        {
            return ClsRevitUtil.PickObject(uidoc, message, baseName, ref id);
        }

        public static bool PickBaseObjects(UIDocument uidoc, ref List<ElementId> ids, string message = baseName)
        {
            return ClsRevitUtil.PickObjectsPartFilter(uidoc, message + "を選択してください", baseName, ref ids);
        }

        /// <summary>
        /// 図面上の切梁継ぎベースを全て取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<ElementId> GetAllKiribariTsugiBaseList(Document doc)
        {
            //図面上の切梁継ぎベースを全て取得
            List<ElementId> htIdList = ClsRevitUtil.GetSelectCreatedFamilyInstanceList(doc, baseName);
            return htIdList;
        }

        /// <summary>
        ///  図面上の切梁継ぎベースを全てクラスで取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<ClsKiribariTsugiBase> GetAllClsKiribariTsugiBaseList(Document doc)
        {
            List<ClsKiribariTsugiBase> lstBase = new List<ClsKiribariTsugiBase>();

            List<ElementId> lstId = GetAllKiribariTsugiBaseList(doc);
            foreach (ElementId id in lstId)
            {
                ClsKiribariTsugiBase clsKT = new ClsKiribariTsugiBase();
                clsKT.SetParameter(doc, id);
                lstBase.Add(clsKT);
            }

            return lstBase;
        }

        /// <summary>
        /// 指定したIDから切梁継ぎベースクラスを取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public void SetParameter(Document doc, ElementId id)
        {
            FamilyInstance shinInstLevel = doc.GetElement(id) as FamilyInstance;
            m_Floor = shinInstLevel.Host.Name;
            m_ElementId = id;
            m_Dan = ClsRevitUtil.GetParameter(doc, id, "段");
            m_SteelType = ClsRevitUtil.GetParameter(doc, id, "鋼材タイプ");
            m_Kousei = ClsRevitUtil.GetParameter(doc, id, "構成") == "シングル" ? ClsKiribariTsugiBase.Kousei.Single : ClsKiribariTsugiBase.Kousei.Double;
            if (m_Kousei == ClsKiribariTsugiBase.Kousei.Double)
            {
                m_KiriSideSteelSizeDouble = ClsRevitUtil.GetParameter(doc, id, "切梁側/鋼材サイズ(ダブル)");
                m_KiriSideTsunagiLength = (int)ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetParameterDouble(doc, id, "切梁側/切梁繋ぎ長さ"));
                m_HaraSideSteelSizeDouble = ClsRevitUtil.GetParameter(doc, id, "腹起側/鋼材サイズ(ダブル)");
                m_HaraSideTsunagiLength = (int)ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetParameterDouble(doc, id, "腹起側/切梁繋ぎ長さ"));
            }
            else
            {
                m_SteelSizeSingle = ClsRevitUtil.GetParameter(doc, id, "鋼材サイズ(シングル)");
            }
            m_KiriSideParts = ClsRevitUtil.GetParameter(doc, id, "切梁側/部品");
            m_HaraSideParts = ClsRevitUtil.GetParameter(doc, id, "腹起側/部品");

            return;
        }
        #endregion
    }
}
