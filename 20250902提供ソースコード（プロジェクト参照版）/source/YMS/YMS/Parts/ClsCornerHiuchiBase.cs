using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitUtil;

namespace YMS.Parts
{
    public enum CornerHiuchiId : int
    {
        review,
        reset,
        OK,
        cancel
    }

    public class ClsCornerHiuchiBase
    {

        #region 定数
        public const string baseName = "隅火打ベース";


        /// <summary>
        /// 処理方法
        /// </summary>
        public enum ShoriType
        {
            Auto,
            Manual,
            Replace,
            Change
        }

        /// <summary>
        /// 構成
        /// </summary>
        public enum Kousei
        {
            Single,
            Double
        }

        /// <summary>
        /// 火打ち角度
        /// </summary>
        public enum HiuchiAngle
        {
            Toubun,
            Nini
        }
        #endregion

        #region プロパティ
        /// <summary>
        /// 処理方法
        /// </summary>
        public ShoriType m_ShoriType { get; set; }

        /// <summary>
        /// 構成
        /// </summary>
        public Kousei m_Kousei { get; set; }

        /// <summary>
        /// 火打角度
        /// </summary>
        public HiuchiAngle m_HiuchiAngle { get; set; }

        /// <summary>
        /// 任意配置角度
        /// </summary>
        public double m_angle { get; set; }

        /// <summary>
        /// 鋼材タイプ
        /// </summary>
        public string m_SteelType { get; set; }

        /// <summary>
        /// 鋼材サイズ
        /// </summary>
        public string m_SteelSize { get; set; }

        /// <summary>
        /// 端部部品１
        /// </summary>
        public string m_TanbuParts1 { get; set; }

        /// <summary>
        /// 火打受ピースサイズ1
        /// </summary>
        public string m_HiuchiUkePieceSize1 { get; set; }

        /// <summary>
        /// 端部部品2
        /// </summary>
        public string m_TanbuParts2 { get; set; }

        /// <summary>
        /// 火打受ピースサイズ2
        /// </summary>
        public string m_HiuchiUkePieceSize2 { get; set; }

        /// <summary>
        /// 火打ち全長
        /// </summary>
        public double m_HiuchiTotalLength { get; set; }

        /// <summary>
        /// 火打ち長さ(シングル)
        /// </summary>
        public double m_HiuchiLengthSingleL { get; set; }

        /// <summary>
        /// ジャッキ(シングル)
        /// </summary>
        public string m_JackTypeSingle { get; set; }

        /// <summary>
        /// 火打ち長さ（ダブル上）
        /// </summary>
        public double m_HiuchiLengthDoubleUpperL1 { get; set; }

        /// <summary>
        /// ジャッキ(ダブル上)
        /// </summary>
        public string m_JackTypeDoubleUpper { get; set; }

        /// <summary>
        /// 火打ち長さ（ダブル下）
        /// </summary>
        public double m_HiuchiLengthDoubleUnderL2 { get; set; }

        /// <summary>
        /// ジャッキ(ダブル下)
        /// </summary>
        public string m_JackTypeDoubleLower { get; set; }

        /// <summary>
        /// 上下火打ずれ量a
        /// </summary>
        public double m_HiuchiZureryo { get; set; }


        /// <summary>
        /// レビュー線
        /// </summary>
        public List<ElementId> m_testLineList { get; set; }
        /// <summary>
        /// レビュー寸法
        /// </summary>
        public List<ElementId> m_testNoteList { get; set; }
        /// <summary>
        /// 腹起ベースのID　一つ目
        /// </summary>
        public ElementId m_HaraokoshiBaseID1 { get; set; }
        /// <summary>
        /// 腹起ベースのID　二つ目
        /// </summary>
        public ElementId m_HaraokoshiBaseID2 { get; set; }
        /// <summary>
        /// 腹起ベース変更用のID　一つ目
        /// </summary>
        public ElementId m_HaraokoshiBaseChangeID1 { get; set; }
        /// <summary>
        /// 腹起ベース変更用のID　二つ目
        /// </summary>
        public ElementId m_HaraokoshiBaseChangeID2 { get; set; }
        /// <summary>
        /// コーナー火打処理判定
        /// </summary>
        public CornerHiuchiId m_cornerHiuchiId { get; set; }
        /// <summary>
        /// コーナー火打ち
        /// </summary>
        public static ClsCornerHiuchiBase m_cornerHiuchi { get; set; }
        /// <summary>
        /// 変更するコーナー火打
        /// </summary>
        public ElementId m_cornerHiuchiID { get; set; }

        /// <summary>
        /// 編集用：フロア
        /// </summary>
        public string m_Floor { get; set; }

        /// <summary>
        /// 編集用：段
        /// </summary>
        public string m_Dan { get; set; }

        /// <summary>
        /// 編集用：段
        /// </summary>
        public bool m_FlgEdit { get; set; }


        /// <summary>
        /// 腹起ベースの角度
        /// </summary>
        public double m_HaraAngle { get; set; }
        #endregion

        #region コンストラクタ
        public ClsCornerHiuchiBase()
        {
            //初期化
            Init();
        }
        #endregion

        #region メソッド
        public void Init()
        {
            m_ShoriType = ShoriType.Auto;
            m_Kousei = Kousei.Single;
            m_HiuchiAngle = HiuchiAngle.Nini;
            m_angle = 45.0;
            m_SteelType = string.Empty;
            m_SteelSize = string.Empty;
            m_TanbuParts1 = string.Empty;
            m_HiuchiUkePieceSize1 = string.Empty;
            m_TanbuParts2 = string.Empty;
            m_HiuchiUkePieceSize2 = string.Empty;
            m_HiuchiTotalLength = 0.0;
            m_HiuchiLengthSingleL = 0.0;
            m_HiuchiLengthDoubleUpperL1 = 0.0;
            m_HiuchiLengthDoubleUnderL2 = 0.0;
            m_HiuchiZureryo = 0.0;
            m_testLineList = new List<ElementId>();
            m_testNoteList = new List<ElementId>();
            m_HaraokoshiBaseID1 = null;
            m_HaraokoshiBaseID2 = null;
            m_HaraokoshiBaseChangeID1 = null;
            m_HaraokoshiBaseChangeID2 = null;
            m_Floor = string.Empty;
            m_Dan = string.Empty;
            m_FlgEdit = false;
            m_HaraAngle = 0;
        }

        /// <summary>
        /// 腹起ベースが二本選ばれている・隅火打ベース長さが確定していることを前提に、隅火打ベースの始終点を返す
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="startPnt"></param>
        /// <param name="endPnt"></param>
        /// <returns></returns>
        public bool GetBaseStartEndPoint(Document doc, ref XYZ startPnt, ref XYZ endPnt)
        {
            double angle = m_angle;
            double length = m_HiuchiTotalLength;//構成に関わらず全長を入れるだけでよさそう
            //if (m_Kousei == Kousei.Double)
            //{
            //    if (m_HiuchiLengthDoubleUpperL1 < m_HiuchiLengthDoubleUnderL1)
            //    {
            //        length = m_HiuchiLengthDoubleUnderL1 + m_HiuchiZureryo;
            //    }
            //    else
            //    {
            //        length = m_HiuchiLengthDoubleUpperL1 + m_HiuchiZureryo;
            //    }
            //}
            //else
            //{
            //    length = m_HiuchiLengthSingleL;
            //}

            Element inst = doc.GetElement(m_HaraokoshiBaseID1);
            FamilyInstance ShinInstLevel = doc.GetElement(m_HaraokoshiBaseID1) as FamilyInstance;
            LocationCurve lCurve = inst.Location as LocationCurve;
            if (lCurve == null)
            {
                return false;
            }

            XYZ tmpStPoint = lCurve.Curve.GetEndPoint(0);
            XYZ tmpEdPoint = lCurve.Curve.GetEndPoint(1);

            Element inst2 = doc.GetElement(m_HaraokoshiBaseID2);
            FamilyInstance ShinInstLevel2 = doc.GetElement(m_HaraokoshiBaseID2) as FamilyInstance;
            LocationCurve lCurve2 = inst2.Location as LocationCurve;
            if (lCurve2 == null)
            {
                return false;
            }

            XYZ tmpStPoint2 = lCurve2.Curve.GetEndPoint(0);
            XYZ tmpEdPoint2 = lCurve2.Curve.GetEndPoint(1);

            //腹起同士の交点取得
            XYZ intersectPnt = ClsRevitUtil.GetIntersection(lCurve.Curve, lCurve2.Curve);//.CalculateIntersectionPoint(tmpStPoint, tmpEdPoint, tmpStPoint2, tmpEdPoint2);

            //交点が存在する場合入隅か判定
            if (intersectPnt != null)
            {
                //入隅か判定
                bool bIrizumi = false;
                if (!ClsHaraokoshiBase.CheckIrizumi(lCurve.Curve, lCurve2.Curve, ref bIrizumi))
                {
                    return false;
                }
                if (!bIrizumi)
                {
                    return false;
                }
            }

            if (intersectPnt == null)
            {
                (double dist1, double dist2) = ClsRevitUtil.GetDistanceToIntersection(tmpStPoint, tmpEdPoint, tmpStPoint2, tmpEdPoint2);
                Curve InfiniteCurve = ClsRevitUtil.CreateInfiniteCurve(tmpStPoint, tmpEdPoint, dist1);//延長したCurveをこの後の処理にも使用した方がよいかも
                Curve InfiniteCurve2 = ClsRevitUtil.CreateInfiniteCurve(tmpStPoint2, tmpEdPoint2, dist2);
                intersectPnt = ClsRevitUtil.GetIntersection(InfiniteCurve, InfiniteCurve2);
                if(intersectPnt == null)
                    return false;
            }

            //angleA = 入力された角度（一本目の腹起ベースと入力された長さの線分とのなす角）
            double angleA = angle;
            //angleC = 一本目の腹起ベースと二本目の腹起ベースのなす角
            double angleC = ClsRevitUtil.CalculateAngleBetweenLines(tmpStPoint, tmpEdPoint, tmpStPoint2, tmpEdPoint2);
            if (m_HiuchiAngle == HiuchiAngle.Toubun)
            {
                angleA = ClsGeo.FloorAtDigitAdjust(1, (180 - angleC) / 2);
                m_angle = angleA;
            }
            //angleB = 二本目の腹起ベースと入力された長さの線分とのなす角
            double angleB = 180 - angleA - angleC;

            m_HaraAngle = angleC; //腹起間の角度を保持

            //double abLength = length;
            //double abLength = dCalculateOffsetsAndLength(length, angleA, angleB);
            double abLength = GetTotalLength(angleC);
            double bcLength = 0.0;
            double caLength = 0.0;

            ClsRevitUtil.CalculateTriangleSides(abLength, angleA, angleB, angleC, ref caLength, ref bcLength);

            XYZ pntTmp = new XYZ();
            if (intersectPnt.DistanceTo(tmpStPoint) > (intersectPnt.DistanceTo(tmpEdPoint)))
            {
                pntTmp = tmpStPoint;
            }
            else
            {
                pntTmp = tmpEdPoint;
            }

            XYZ pntS = ClsRevitUtil.MoveCoordinates(intersectPnt, pntTmp, ClsRevitUtil.ConvertDoubleGeo2Revit(caLength));

            pntTmp = new XYZ();
            if (intersectPnt.DistanceTo(tmpStPoint2) > (intersectPnt.DistanceTo(tmpEdPoint2)))
            {
                pntTmp = tmpStPoint2;
            }
            else
            {
                pntTmp = tmpEdPoint2;
            }

            XYZ pntE = ClsRevitUtil.MoveCoordinates(intersectPnt, pntTmp, ClsRevitUtil.ConvertDoubleGeo2Revit(bcLength));


            startPnt = pntS;
            endPnt = pntE;


            return true;
        }

        //private double dCalculateOffsetsAndLength(double dLength, double dAngleS, double dAngleE)
        //{
        //    //火打部品の厚みを取得する
        //    double dStartPartsThickness = 0.0;
        //    double dEndPartsThickness = 0.0;
        //    dStartPartsThickness = 16.0;//仮の値
        //    dEndPartsThickness = 16.0;//仮の値

        //    double dKouzaiWidth = 0.0;
        //    dKouzaiWidth = 400.0;    //仮の値

        //    double offsetStart = dKouzaiWidth / (2 * Math.Tan(dAngleS));
        //    double offsetEnd = dKouzaiWidth / (2 * Math.Tan(dAngleE));

        //    double dOffsetStartThickness = 0.0;
        //    double dOffsetEndThickness = 0.0;

        //    //作図に必要な長さを取得
        //   offsetStart += dOffsetStartThickness / Math.Sin(dAngleS);
        //    offsetEnd += dOffsetEndThickness / Math.Sin(dAngleE);

        //    if (dStartPartsThickness != 0.0)
        //    {
        //        offsetStart += dStartPartsThickness / Math.Sin(dAngleS);
        //    }
        //    if (dEndPartsThickness != 0.0)
        //    {
        //        offsetEnd += dEndPartsThickness / Math.Sin(dAngleE);
        //    }

        //    double dTotalLength = dLength + offsetStart + offsetEnd;

        //    return dTotalLength;
        //}


        /// <summary>
        /// 隅火打ちの全長取得処理
        /// </summary>
        /// <param name="dHaraAngle">腹起間角度</param>
        /// <returns></returns>
        public double GetTotalLength(double dHaraAngle)
        {
            double dMain = 0.0;
            double dA = 0.0;
            double dB = 0.0;
            return GetTotalLength(dHaraAngle, ref dMain, ref dA, ref dB);
        }

        /// <summary>
        /// 隅火打ちの全長取得処理
        /// </summary>
        /// <param name="dHaraAngle">腹起間角度</param>
        /// <returns></returns>
        public double GetTotalLength(double dHaraAngle, ref double dMain, ref double dOffsetA, ref double dOffsetB)
        {
            double dTotalLength = 0;
            double d = 0.0;
            double d2 = 0.0;
            if (m_Kousei == Kousei.Single)
            {
                dTotalLength += m_HiuchiLengthSingleL;
            }
            else
            {
                //ダブルの場合は長い方の値で計算
                d = m_HiuchiLengthDoubleUpperL1;
                d2 = m_HiuchiLengthDoubleUnderL2;
                d = (d >= d2 ? d : d2);
                d2 = m_HiuchiZureryo;
                dTotalLength += d + d2;
            }

            double dAngleA = 0;
            double dAngleB = 0;
            double dKouzaiWidthA = 0;
            double dKouzaiWidthB = 0;
            double dPartsThicknessA = 0;
            double dPartsThicknessB = 0;

            //角度
            if (m_HiuchiAngle == HiuchiAngle.Nini)
            {
                dAngleA = m_angle;
                dAngleB = 180 - dHaraAngle - m_angle;
            }
            else
            {
                dAngleA = ClsGeo.FloorAtDigitAdjust(1, (180 - dHaraAngle) / 2);
                dAngleB = dAngleA;
            }

            //鋼材幅
            if (m_TanbuParts1 != Master.ClsHiuchiCsv.TypeNameFVP)
            {
                string targetShuzai = Master.ClsHiuchiCsv.GetTargetShuzai(m_HiuchiUkePieceSize1);
                dKouzaiWidthA = Master.ClsYamadomeCsv.GetWidth(targetShuzai);
            }
            else
            {
                dKouzaiWidthA = Master.ClsYamadomeCsv.GetWidth(m_SteelSize);
            }

            if (m_TanbuParts2 != Master.ClsHiuchiCsv.TypeNameFVP)
            {
                string targetShuzai = Master.ClsHiuchiCsv.GetTargetShuzai(m_HiuchiUkePieceSize2);
                dKouzaiWidthB = Master.ClsYamadomeCsv.GetWidth(targetShuzai);
            }
            else
            {
                dKouzaiWidthB = Master.ClsYamadomeCsv.GetWidth(m_SteelSize);
            }

            //端部部品厚
            dPartsThicknessA = Master.ClsHiuchiCsv.GetThickness(m_HiuchiUkePieceSize1);
            dPartsThicknessB = Master.ClsHiuchiCsv.GetThickness(m_HiuchiUkePieceSize2);

            double dA = CalculateOffsets(dAngleA, dKouzaiWidthA, dPartsThicknessA);
            double dB = CalculateOffsets(dAngleB, dKouzaiWidthB, dPartsThicknessB);

            dMain = dTotalLength;
            dOffsetA = dA;
            dOffsetB = dB;

            return ClsGeo.FloorAtDigitAdjust(0, dTotalLength + dA + dB); ;
        }
        public bool CreateHojyoPeace(Document doc, XYZ tmpStPoint, XYZ tmpEdPoint, double dTotalLength, string dan)
        {
            int hundredsPlace = ClsGeo.GetNthDigitNum((int)dTotalLength, 3);
            if (hundredsPlace == 0 || hundredsPlace == 5)
                return false;

            ElementId baseID = m_cornerHiuchiID;
            if (baseID == null)
            {
                return false;
            }

            //ベース
            FamilyInstance ShinInstLevel = doc.GetElement(baseID) as FamilyInstance;
            ElementId levelID = ShinInstLevel.Host.Id;
            double size = ClsRevitUtil.CovertToAPI(Master.ClsYamadomeCsv.GetWidth(m_SteelSize) / 2);
            ClsYMSUtil.GetDifferenceWithAllBase(doc, baseID, out double diff, out double diff2);
            //size += diff;
            //Element inst = doc.GetElement(baseID);
            //LocationCurve lCurve = inst.Location as LocationCurve;
            //if (lCurve == null)
            //{
            //    return false;
            //}

            //XYZ tmpStPoint = lCurve.Curve.GetEndPoint(0);
            //XYZ tmpEdPoint = lCurve.Curve.GetEndPoint(1);
            XYZ dir = Line.CreateBound(tmpStPoint, tmpEdPoint).Direction;

            string hojyo2 = Master.ClsSupportPieceCsv.GetSize(m_SteelSize, 2);
            string path2 = Master.ClsSupportPieceCsv.GetFamilyPath(hojyo2);
            string familyName2 = RevitUtil.ClsRevitUtil.GetFamilyName(path2);

            string hojyo3 = Master.ClsSupportPieceCsv.GetSize(m_SteelSize, 3);
            string path3 = Master.ClsSupportPieceCsv.GetFamilyPath(hojyo3);
            string familyName3 = RevitUtil.ClsRevitUtil.GetFamilyName(path3);

            if (!ClsRevitUtil.LoadFamilyData(doc, path2, out Family fam2))
            {
                return false;
            }
            FamilySymbol sym2 = (ClsRevitUtil.GetFamilySymbol(doc, familyName2, "隅火打"));

            if (!ClsRevitUtil.LoadFamilyData(doc, path3, out Family fam3))
            {
                return false;
            }
            FamilySymbol sym3 = (ClsRevitUtil.GetFamilySymbol(doc, familyName3, "隅火打"));

            using (Transaction tx = new Transaction(doc, "Load Family"))
            {
                tx.Start();
                //■■■始点側処理■■■
                if (sym3 != null)
                {
                    ElementId CreatedID = ClsRevitUtil.Create(doc, tmpStPoint, levelID, sym3);
                    FamilyInstance ch = doc.GetElement(CreatedID) as FamilyInstance;

                    XYZ dirV = ch.HandOrientation;
                    double dVectAngle = dirV.AngleOnPlaneTo(dir, XYZ.BasisZ);
                    Line axis = Line.CreateBound(tmpStPoint, tmpStPoint + XYZ.BasisZ);

                    ClsRevitUtil.RotateElement(doc, CreatedID, axis, dVectAngle);
                    if (dan == "上段")
                        ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", size+ diff);
                    else if (dan == "下段")
                        ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", -size - diff);
                    else
                        ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", 0.0);
                    if (dan != "同段" && m_Kousei == Kousei.Double)
                        ClsKariKouzai.SetBaseIdDouble(doc, CreatedID, baseID, dan);
                    ClsKariKouzai.SetBaseId(doc, CreatedID, baseID);
                    ClsKariKouzai.SetKariKouzaiFlag(doc, CreatedID);
                }
                //■■■終点側処理■■■
                if (sym2 != null)
                {
                    ElementId CreatedID = ClsRevitUtil.Create(doc, tmpEdPoint, levelID, sym2);
                    FamilyInstance ch = doc.GetElement(CreatedID) as FamilyInstance;

                    XYZ dirV = ch.HandOrientation;
                    double dVectAngle = dirV.AngleOnPlaneTo(dir, XYZ.BasisZ) + Math.PI;
                    Line axis = Line.CreateBound(tmpEdPoint, tmpEdPoint + XYZ.BasisZ);

                    ClsRevitUtil.RotateElement(doc, CreatedID, axis, dVectAngle);

                    if (dan == "上段")
                        ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", size + diff2);
                    else if (dan == "下段")
                        ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", -size - diff2);
                    else
                        ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", 0.0);
                    if (dan != "同段" && m_Kousei == Kousei.Double)
                        ClsKariKouzai.SetBaseIdDouble(doc, CreatedID, baseID, dan);
                    ClsKariKouzai.SetBaseId(doc, CreatedID, baseID);
                    ClsKariKouzai.SetKariKouzaiFlag(doc, CreatedID);
                }
                tx.Commit();
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
            ElementId baseID = m_cornerHiuchiID;
            if (baseID == null)
            {
                return false;
            }

            //隅火打ちベース
            FamilyInstance ShinInstLevel = doc.GetElement(baseID) as FamilyInstance;
            FamilyInstance lv = doc.GetElement(baseID) as FamilyInstance;
            ElementId levelID = lv.Host.Id;
            Element inst = doc.GetElement(baseID);
            LocationCurve lCurve = inst.Location as LocationCurve;
            if (lCurve == null)
            {
                return false;
            }

            XYZ tmpStPoint = lCurve.Curve.GetEndPoint(0);
            XYZ tmpEdPoint = lCurve.Curve.GetEndPoint(1);
            XYZ vecSE = (tmpEdPoint - tmpStPoint).Normalize();

            //始端側の梁と終点側の梁を選別
            List<ElementId> lstHari = ClsHaraokoshiBase.GetIntersectionBase(doc, baseID, levelID);

            if (lstHari.Count < 2)
                return false;

            ElementId idA = null;
            ElementId idB = null;
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

            XYZ insec = ClsRevitUtil.GetIntersection(lCurveA.Curve, lCurveB.Curve);
            bool clockwise = false;// !ClsRevitUtil.CheckNearGetEndPoint(lCurveB.Curve, insec);
            if(insec != null)
            {
                clockwise = !ClsRevitUtil.CheckNearGetEndPoint(lCurveB.Curve, insec);
            }
            else
            {
                //腹起が交叉していない時の判定
                clockwise = !ClsRevitUtil.CheckNearGetEndPoint(lCurveB.Curve, tmpEdPointA);
            }
            //時計回りのとき自在火打受の移動ベクトルを反転させる
            var fvpVec = -1;
            if (clockwise)
            {
                fvpVec = 1;
            }
            //角度
            double dHaraAngle = ClsRevitUtil.CalculateAngleBetweenLines(tmpStPointA, tmpEdPointA, tmpStPointB, tmpEdPointB);//, tmpEdPointB, tmpStPointB);
            double dAngleA = ClsRevitUtil.CalculateAngleBetweenLines(tmpStPoint, tmpEdPoint, tmpEdPointA, tmpStPointA);
            double dAngleB = ClsRevitUtil.CalculateAngleBetweenLines(tmpStPoint, tmpEdPoint, tmpEdPointB, tmpStPointB);


            //挿入位置を取得
            double dA = 0;
            double dB = 0;
            double dMain = 0;
            GetTotalLength(dHaraAngle, ref dMain, ref dA, ref dB);
            pS = tmpStPoint + (vecSE * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(dA));//自在火打ち対応
            pE = tmpEdPoint + (-vecSE * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(dB));//自在火打ち対応


            //■■■共通処理■■■
            
            ClsYMSUtil.GetDifferenceWithAllBase(doc, baseID, out double diff, out double diff2);

            FamilySymbol sym, sym2, sym3 = null;//自在火打ち対応
            string shinfamily = Master.ClsHiuchiCsv.GetFamilyPath(m_HiuchiUkePieceSize1);
            string shinfamilyName = RevitUtil.ClsRevitUtil.GetFamilyName(shinfamily);
            double sizeA = ClsRevitUtil.CovertToAPI(Master.ClsYamadomeCsv.GetWidth(m_SteelSize) / 2) + diff;
            //シンボル読込※自在とその他はタイプを持っている
            if (m_TanbuParts1 == Master.ClsHiuchiCsv.TypeNameFVP || m_TanbuParts1 == Master.ClsHiuchiCsv.TypeNameOtherVP)
            {
                if (!ClsRevitUtil.LoadFamilyData(doc, shinfamily, out Family tanbuFam))
                {
                    //return false; ;
                }
                sym = (ClsRevitUtil.GetFamilySymbol(doc, shinfamilyName, "隅火打"));

                //自在火打ち対応 -start
                string buruFamPath = Master.ClsBurumanCSV.GetFamilyPath("C-50");
                string buruFamName = RevitUtil.ClsRevitUtil.GetFamilyName(buruFamPath);
                if (!ClsRevitUtil.LoadFamilyData(doc, buruFamPath, out Family buruFam))
                {
                    //return false; ;
                }
                sym3 = (ClsRevitUtil.GetFamilySymbol(doc, buruFamName, "自在受けピース(FVP)"));
                //自在火打ち対応 -end
            }
            else
            {
                if (!ClsRevitUtil.LoadFamilySymbolData(doc, shinfamily, shinfamilyName, out sym))
                {
                    //return false;
                }
            }

            string shinfamily2 = Master.ClsHiuchiCsv.GetFamilyPath(m_HiuchiUkePieceSize2);
            string shinfamilyName2 = RevitUtil.ClsRevitUtil.GetFamilyName(shinfamily2);
            double sizeB = ClsRevitUtil.CovertToAPI(Master.ClsYamadomeCsv.GetWidth(m_SteelSize) / 2) + diff2;
            if (m_TanbuParts2 == Master.ClsHiuchiCsv.TypeNameFVP || m_TanbuParts2 == Master.ClsHiuchiCsv.TypeNameOtherVP)
            {
                if (!ClsRevitUtil.LoadFamilyData(doc, shinfamily2, out Family tanbuFam))
                {
                    //return false; ;
                }
                sym2 = (ClsRevitUtil.GetFamilySymbol(doc, shinfamilyName2, "隅火打"));

                //自在火打ち対応 -start
                string buruFamPath = Master.ClsBurumanCSV.GetFamilyPath("C-50");
                string buruFamName = RevitUtil.ClsRevitUtil.GetFamilyName(buruFamPath);
                if (!ClsRevitUtil.LoadFamilyData(doc, buruFamPath, out Family buruFam))
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
                //■■■始点側処理■■■
                if (sym != null)
                {
                    if (m_TanbuParts1 == Master.ClsHiuchiCsv.TypeNameFVP)
                    {
                        //自在火打ち対応 -start
                        //自在火打の挿入・位置調整・回転
                        tx.Start();
                        ElementId CreatedID = ClsRevitUtil.Create(doc, tmpStPoint, levelID, sym);
                        ClsKariKouzai.SetKariKouzaiFlag(doc, CreatedID);
                        FamilyInstance ch = doc.GetElement(CreatedID) as FamilyInstance;
                        tx.Commit();
                        tx.Start();

                        XYZ dirV = new XYZ(1, 0, 0);//ファミリインスタンスの向き
                        double dVectAngle = dirV.AngleTo(vecSEA);
                        Line axis = Line.CreateBound(tmpStPoint, tmpStPoint + XYZ.BasisZ);
                        ch.flipFacing();

                        if (!ClsGeo.IsLeft(dirV, vecSEA))
                        {
                            dVectAngle = -dVectAngle;
                        }

                        //RevitUtil.ClsRevitUtil.MirrorElement(doc, CreatedID, plane);
                        //ElementTransformUtils.MoveElement(doc, CreatedID, tmpStPoint - ((LocationPoint)ch.Location).Point);
                        //double dW = RevitUtil.ClsRevitUtil.CovertToAPI(Master.ClsYamadomeCsv.GetWidth(m_SteelSize));
                        double dW = RevitUtil.ClsRevitUtil.CovertToAPI(ClsYMSUtil.GetKouzaiHabaFromBase(doc, baseID));
                        XYZ dirMove = new XYZ(fvpVec * dW / 2 * -Math.Cos(ClsGeo.Deg2Rad(dAngleA * 2)), 0, 0);//移動長さ
                        ElementTransformUtils.MoveElement(doc, CreatedID, dirMove);
                        ClsRevitUtil.RotateElement(doc, CreatedID, axis, dVectAngle);

                        XYZ direction = vecSE.Normalize(); // 方向ベクトルを正規化
                        XYZ direction2 = vecSEA.Normalize(); // 方向ベクトルを正規化
                        //時計回り要
                        if (clockwise)
                        {
                            ch.flipHand();
                            direction = -direction;
                            dVectAngle -= Math.PI / 2;
                        }
                        //ブルマンの挿入位置を算出

                        XYZ leftDirection = new XYZ(-direction.Y, direction.X, 0); // 左側方向に90度回転
                        XYZ moveVector = leftDirection * (dW / 2) * 0.8;
                        Curve cvBase = Line.CreateBound(tmpStPoint + moveVector, tmpEdPoint + moveVector);


                        XYZ leftDirection2 = new XYZ(-direction2.Y, direction2.X, 0); // 左側方向に90度回転
                        string target = Master.ClsHiuchiCsv.GetTargetShuzai(m_HiuchiUkePieceSize1);
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
                            ClsRevitUtil.SetParameter(doc, CreatedID_B, "基準レベルからの高さ", -sizeA * 2 + diff);
                        }
                        else
                        {
                            ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", 0.0);
                            ClsRevitUtil.SetParameter(doc, CreatedID_B, "基準レベルからの高さ", -sizeA + diff);
                        }

                        //自在火打ち対応 -end
                        tx.Commit();
                    }
                    else
                    {
                        tx.Start();
                        ElementId CreatedID = ClsRevitUtil.Create(doc, pS, levelID, sym);
                        ClsKariKouzai.SetKariKouzaiFlag(doc, CreatedID);
                        FamilyInstance ch = doc.GetElement(CreatedID) as FamilyInstance;
                        tx.Commit();
                        //XYZ dirV = new XYZ(1, 0, 0);//ファミリインスタンスの向き
                        //double dVectAngle = dirV.AngleTo(vecSEA);
                        //if (!ClsGeo.IsLeft(vecSEA, pS))
                        //{
                        //    dVectAngle = -dVectAngle;
                        //}
                        tx.Start();
                        double dVectAngle = -vecSEA.AngleOnPlaneTo(XYZ.BasisX, XYZ.BasisZ);

                        Line axis = Line.CreateBound(pS, pS + XYZ.BasisZ);
                        ClsRevitUtil.RotateElement(doc, CreatedID, axis, dVectAngle);
                        if (clockwise)
                        {
                            ch.flipHand();
                        }
                        if (danA == "上段")
                            ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", sizeA);
                        else if (danA == "下段")
                            ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", -sizeA);
                        else
                            ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", 0.0);
                        tx.Commit();
                    }
                }
                //■■■終点側処理■■■
                if (sym2 != null)
                {

                    if (m_TanbuParts2 == Master.ClsHiuchiCsv.TypeNameFVP)
                    {
                        tx.Start();
                        ElementId CreatedID = ClsRevitUtil.Create(doc, tmpEdPoint, levelID, sym2);
                        ClsKariKouzai.SetKariKouzaiFlag(doc, CreatedID);
                        FamilyInstance ch = doc.GetElement(CreatedID) as FamilyInstance;
                        tx.Commit();

                        tx.Start();
                        XYZ dirV = new XYZ(-1, 0, 0);//ファミリインスタンスの向き
                        double dVectAngle = dirV.AngleTo(vecSEB);
                        Line axis = Line.CreateBound(tmpEdPoint, tmpEdPoint + XYZ.BasisZ);

                        if (!ClsGeo.IsLeft(dirV, vecSEB))
                        {
                            dVectAngle = -dVectAngle;
                        }

                        //RevitUtil.ClsRevitUtil.MirrorElement(doc, CreatedID, plane);
                        //ElementTransformUtils.MoveElement(doc, CreatedID, tmpEdPoint - ((LocationPoint)ch.Location).Point);
                        //double dW = RevitUtil.ClsRevitUtil.CovertToAPI(Master.ClsYamadomeCsv.GetWidth(m_SteelSize));
                        double dW = RevitUtil.ClsRevitUtil.CovertToAPI(ClsYMSUtil.GetKouzaiHabaFromBase(doc, baseID));
                        XYZ dirMove = new XYZ(fvpVec * dW / 2 * -Math.Cos(ClsGeo.Deg2Rad(dAngleB * 2)), 0, 0);//移動長さ
                        ElementTransformUtils.MoveElement(doc, CreatedID, dirMove);
                        ClsRevitUtil.RotateElement(doc, CreatedID, axis, dVectAngle);

                        XYZ direction = vecSE.Normalize(); // 方向ベクトルを正規化
                        XYZ direction2 = vecSEB.Normalize(); // 方向ベクトルを正規化

                        //時計回り要
                        if (clockwise)
                        {
                            ch.flipHand();
                            direction = -direction;
                            dVectAngle += Math.PI / 2;
                        }
                        //ブルマンの挿入位置を算出

                        XYZ leftDirection = new XYZ(-direction.Y, direction.X, 0); // 左側方向に90度回転
                        XYZ moveVector = leftDirection * (dW / 2) * 0.8;
                        Curve cvBase = Line.CreateBound(tmpStPoint + moveVector, tmpEdPoint + moveVector);

                        XYZ leftDirection2 = new XYZ(-direction2.Y, direction2.X, 0); // 左側方向に90度回転
                        string target = Master.ClsHiuchiCsv.GetTargetShuzai(m_HiuchiUkePieceSize2);
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
                            ClsRevitUtil.SetParameter(doc, CreatedID_B, "基準レベルからの高さ", diff2);
                        }
                        else if (danB == "下段")
                        {
                            ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", -sizeB);
                            ClsRevitUtil.SetParameter(doc, CreatedID_B, "基準レベルからの高さ", -sizeB * 2 + diff2);
                        }
                        else
                        {
                            ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", 0.0);
                            ClsRevitUtil.SetParameter(doc, CreatedID_B, "基準レベルからの高さ", -sizeB + diff2);
                        }

                        tx.Commit();
                    }
                    else
                    {
                        tx.Start();
                        ElementId CreatedID = ClsRevitUtil.Create(doc, pE, levelID, sym2);
                        ClsKariKouzai.SetKariKouzaiFlag(doc, CreatedID);
                        FamilyInstance ch = doc.GetElement(CreatedID) as FamilyInstance;
                        //ch.flipFacing();
                        tx.Commit();
                        //XYZ dirV = new XYZ(-1, 0, 0);//ファミリインスタンスの向き
                        //double dVectAngle = dirV.AngleTo(vecSEB);


                        //if (!ClsGeo.IsLeft(vecSEB, pE))
                        //{
                        //    dVectAngle = -dVectAngle;
                        //}
                        tx.Start();
                        double dVectAngle = -vecSEB.AngleOnPlaneTo(XYZ.BasisX, XYZ.BasisZ);

                        Line axis = Line.CreateBound(pE, pE + XYZ.BasisZ);
                        ClsRevitUtil.RotateElement(doc, CreatedID, axis, dVectAngle);
                        if (!clockwise)
                        {
                            ch.flipHand();
                        }


                        if (danB == "上段")
                            ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", sizeB);
                        else if (danB == "下段")
                            ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", -sizeB);
                        else
                            ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", 0.0);
                        tx.Commit();
                    }
                }


            }


            return true;
        }
        /// <summary>
        /// 端部部品が配置される際の位置を取得
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public bool GetTanbuPartsPoints(Document doc, XYZ tmpStPoint, XYZ tmpEdPoint, ref XYZ pS, ref XYZ pE)
        {
            var cv = Line.CreateBound(tmpStPoint, tmpEdPoint);
            XYZ vecSE = (tmpEdPoint - tmpStPoint).Normalize();

            //始端側の梁と終点側の梁を選別
            List<ElementId> lstHari = ClsHaraokoshiBase.GetAllHaraokoshiBaseList(doc);

            ElementId idA = null;
            ElementId idB = null;
            double dSMin = 0;
            double dEMin = 0;
            foreach (ElementId id in lstHari)
            {
                Element el = doc.GetElement(id);
                LocationCurve lCurve2 = el.Location as LocationCurve;
                XYZ p = ClsRevitUtil.GetIntersection(cv, lCurve2.Curve);
                if (p != null)
                {
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
            }

            //梁（始点側）
            Element elA = doc.GetElement(idA);
            LocationCurve lCurveA = elA.Location as LocationCurve;
            if (lCurveA == null)
            {
                return false;
            }

            XYZ tmpStPointA = lCurveA.Curve.GetEndPoint(0);
            XYZ tmpEdPointA = lCurveA.Curve.GetEndPoint(1);


            //梁（終点側）
            Element elB = doc.GetElement(idB);
            LocationCurve lCurveB = elB.Location as LocationCurve;
            if (lCurveB == null)
            {
                return false;
            }

            XYZ tmpStPointB = lCurveB.Curve.GetEndPoint(0);
            XYZ tmpEdPointB = lCurveB.Curve.GetEndPoint(1);

            //角度
            double dHaraAngle = ClsRevitUtil.CalculateAngleBetweenLines(tmpStPointA, tmpEdPointA, tmpStPointB, tmpEdPointB);//, tmpEdPointB, tmpStPointB);
           
            //挿入位置を取得
            double dA = 0;
            double dB = 0;
            double dMain = 0;
            GetTotalLength(dHaraAngle, ref dMain, ref dA, ref dB);
            pS = tmpStPoint + (vecSE * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(dA));//自在火打ち対応
            pE = tmpEdPoint + (-vecSE * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(dB));//自在火打ち対応

            return true;
        }
        public static bool ObjectReverse(UIDocument uidoc)
        {
            Document doc = uidoc.Document;

            ClsCornerHiuchiBase c = new ClsCornerHiuchiBase();
            DLG.DlgCreateCornerHiuchiBase f = new DLG.DlgCreateCornerHiuchiBase(c);
            f.Show();
            c = f.m_ClsCornerHiuchiBase;


            ElementId id1 = null;
            if (!ClsHaraokoshiBase.PickBaseObject(uidoc, ref id1, "隅火打ベースを配置する腹起ベース　1本目"))
            {
                return false;
            }

            FamilyInstance ShinInstLevel = doc.GetElement(id1) as FamilyInstance;

            Element inst = doc.GetElement(id1);
            LocationCurve lCurve = inst.Location as LocationCurve;
            if (lCurve == null)
            {
                return false;
            }


            XYZ tmpStPoint = lCurve.Curve.GetEndPoint(0);
            XYZ tmpEdPoint = lCurve.Curve.GetEndPoint(1);

            string shinfamily = Master.ClsHiuchiCsv.GetFamilyPath(c.m_HiuchiUkePieceSize1);
            string shinfamilyName = RevitUtil.ClsRevitUtil.GetFamilyName(shinfamily);


            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                //シンボル読込
                if (!ClsRevitUtil.LoadFamilySymbolData(doc, shinfamily, shinfamilyName, out FamilySymbol sym))
                {
                    return false;
                }

                t.Start();

                FamilyInstance lv = doc.GetElement(id1) as FamilyInstance;
                ElementId levelID = lv.Host.Id;

                ElementId CreatedID = ClsRevitUtil.Create(doc, tmpStPoint, levelID, sym);


                FamilyInstance cr = doc.GetElement(CreatedID) as FamilyInstance;
                cr.flipFacing();


                //FamilyInstance sanjikuInst = doc.GetElement(CreatedID) as FamilyInstance;
                ////XYZ sanjikuPoint = (sanjikuInst.Location as LocationPoint).Point;
                //XYZ dirV = sanjikuInst.FacingOrientation;//ファミリインスタンスの向き
                //XYZ dirB = sanjikuInst.HandOrientation;//ファミリインスタンスの側面

                //XYZ directionKiri = Line.CreateBound(cvKiri.GetEndPoint(0), cvKiri.GetEndPoint(1)).Direction;
                //XYZ directionHara = Line.CreateBound(cvHara.GetEndPoint(0), cvHara.GetEndPoint(1)).Direction;

                //double dBaseAngle = dirB.AngleTo(directionHara);
                //double dVectAngle = dirV.AngleTo(directionKiri);
                //double dAngle = -dVectAngle;//時計回りになるように-をかける

                Line axis = Line.CreateBound(tmpStPoint, tmpStPoint + XYZ.BasisZ);
                //ClsRevitUtil.RotateElement(doc, CreatedID, axis, dAngle);

                t.Commit();
            }
            return true;
        }


        public double CalculateOffsets(double dAngle, double dKouzaiWidth, double dPartsThickness)
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

        /// <summary>
        /// 腹起ベースが二本選ばれている・隅火打ベース長さが確定していることを前提に、隅火打ベースの始終点を返す
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="startPnt"></param>
        /// <param name="endPnt"></param>
        /// <returns></returns>
        public bool GetBaseStartEndPoint(Document doc, XYZ haraBase1startPnt, XYZ haraBase1endPnt,
            XYZ haraBase2startPnt, XYZ haraBase2endPnt, ref XYZ startPnt, ref XYZ endPnt)

        {
            double angle = m_angle;
            double length = m_HiuchiTotalLength;//構成に関わらず全長を入れるだけでよさそう
            //if (m_Kousei == Kousei.Double)
            //{
            //    if (m_HiuchiLengthDoubleUpperL1 < m_HiuchiLengthDoubleUnderL1)
            //    {
            //        length = m_HiuchiLengthDoubleUnderL1 + m_HiuchiZureryo;
            //    }
            //    else
            //    {
            //        length = m_HiuchiLengthDoubleUpperL1 + m_HiuchiZureryo;
            //    }
            //}
            //else
            //{
            //    length = m_HiuchiLengthSingleL;
            //}


            XYZ tmpStPoint = haraBase1startPnt;
            XYZ tmpEdPoint = haraBase1endPnt;

            Curve cvInst = Line.CreateBound(tmpStPoint, tmpEdPoint);

            XYZ tmpStPoint2 = haraBase2startPnt;
            XYZ tmpEdPoint2 = haraBase2endPnt;

            Curve cvInst2 = Line.CreateBound(tmpStPoint2, tmpEdPoint2);

            //入隅か判定
            bool bIrizumi = false;
            if (!ClsHaraokoshiBase.CheckIrizumi(cvInst, cvInst2, ref bIrizumi))
            {
                return false;
            }
            if (!bIrizumi)
            {
                return false;
            }

            //腹起同士の交点取得
            XYZ intersectPnt = ClsRevitUtil.GetIntersection(cvInst, cvInst2);//.CalculateIntersectionPoint(tmpStPoint, tmpEdPoint, tmpStPoint2, tmpEdPoint2);

            if (intersectPnt == null)
            {
                return false;
            }

            //angleA = 入力された角度（一本目の腹起ベースと入力された長さの線分とのなす角）
            double angleA = angle;
            //angleC = 一本目の腹起ベースと二本目の腹起ベースのなす角
            double angleC = ClsRevitUtil.CalculateAngleBetweenLines(tmpStPoint, tmpEdPoint, tmpStPoint2, tmpEdPoint2);
            if (m_HiuchiAngle == HiuchiAngle.Toubun)
            {
                angleA = ClsGeo.FloorAtDigitAdjust(1, (180 - angleC) / 2);
                m_angle = angleA;
            }
            //angleB = 二本目の腹起ベースと入力された長さの線分とのなす角
            double angleB = 180 - angleA - angleC;

            m_HaraAngle = angleC; //腹起間の角度を保持

            //double abLength = length;
            double abLength = GetTotalLength(angleC);
            double bcLength = 0.0;
            double caLength = 0.0;


            ClsRevitUtil.CalculateTriangleSides(abLength, angleA, angleB, angleC, ref caLength, ref bcLength);


            XYZ pntTmp = new XYZ();
            if (intersectPnt.DistanceTo(tmpStPoint) > (intersectPnt.DistanceTo(tmpEdPoint)))
            {
                pntTmp = tmpStPoint;
            }
            else
            {
                pntTmp = tmpEdPoint;
            }

            XYZ pntS = ClsRevitUtil.MoveCoordinates(intersectPnt, pntTmp, ClsRevitUtil.ConvertDoubleGeo2Revit(caLength));

            pntTmp = new XYZ();
            if (intersectPnt.DistanceTo(tmpStPoint2) > (intersectPnt.DistanceTo(tmpEdPoint2)))
            {
                pntTmp = tmpStPoint2;
            }
            else
            {
                pntTmp = tmpEdPoint2;
            }

            XYZ pntE = ClsRevitUtil.MoveCoordinates(intersectPnt, pntTmp, ClsRevitUtil.ConvertDoubleGeo2Revit(bcLength));


            startPnt = pntS;
            endPnt = pntE;


            return true;
        }


        /// <summary>
        /// コーナー火打ちレビュー線を作成する
        /// </summary>
        /// <param name="uidoc"></param>
        /// <returns></returns>
        public bool CreateCornerHiuchiCheck(UIDocument uidoc)
        {
            Document doc = uidoc.Document;

            if (m_ShoriType == ShoriType.Manual)
            {
                if (m_HaraokoshiBaseID1 == null && m_HaraokoshiBaseID2 == null)
                {
                    ElementId id1 = null;
                    if (!ClsHaraokoshiBase.PickBaseObject(uidoc, ref id1, "隅火打ベースを配置する腹起ベース　1本目"))//ClsRevitUtil.PickObject(uidoc, "隅火打ベースを配置する腹起ベース　1本目", "腹起ベース", ref id1))
                    {
                        return false;
                    }
                    m_HaraokoshiBaseID1 = id1;

                    ElementId id2 = null;
                    if (!ClsHaraokoshiBase.PickBaseObject(uidoc, ref id2, "隅火打ベースを配置する腹起ベース　2本目")) //ClsRevitUtil.PickObject(uidoc, "隅火打ベースを配置する腹起ベース　2本目", "腹起ベース", ref id2))
                    {
                        return false;
                    }
                    m_HaraokoshiBaseID2 = id2;
                }

                //腹起ベースのレベルを取得
                FamilyInstance ShinInstLevel = doc.GetElement(m_HaraokoshiBaseID1) as FamilyInstance;
                ElementId levelID = ShinInstLevel.Host.Id;
                //事前にレビューが残っていれば削除
                DeleteCornerHiuchiBaseReview(doc);

                XYZ pntS = new XYZ();
                XYZ pntE = new XYZ();
                if (GetBaseStartEndPoint(doc, ref pntS, ref pntE))
                {
                    using (Transaction CreateModelLines = new Transaction(doc, "Create Model Lines"))
                    {
                        try
                        {
                            CreateModelLines.Start();

                            TextNoteOptions options = new TextNoteOptions();
                            options.KeepRotatedTextReadable = false;
                            options.TypeId = doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);

                            XYZ centerPnt = new XYZ((pntS.X + pntE.X) / 2, (pntS.Y + pntE.Y) / 2, (pntS.Z + pntE.Z) / 2);

                            double distRevit = pntS.DistanceTo(pntE);
                            double distVisual = ClsRevitUtil.CovertFromAPI(distRevit);

                            Autodesk.Revit.DB.View activeView = doc.ActiveView;
                            TextNote note = TextNote.Create(doc, activeView.Id, centerPnt, distVisual.ToString(), options);
                            m_testNoteList.Add(note.Id);

                            Curve Curve = Line.CreateBound(pntS, pntE);

                            SketchPlane sketchPlane = SketchPlane.Create(doc, levelID);
                            ModelCurve c = doc.Create.NewModelCurve(Curve, sketchPlane);

                            if (m_Kousei == Kousei.Double) ClsRevitUtil.ChangeLineColor(doc, c.Id, ClsGlobal.m_redColor);

                            m_testLineList.Add(c.Id);
                            CreateModelLines.Commit();
                        }
                        catch (Exception ex)
                        {
                            CreateModelLines.RollBack();
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
            else if (m_ShoriType == ShoriType.Auto)
            {
                if (m_HaraokoshiBaseID1 == null && m_HaraokoshiBaseID2 == null)
                {
                    ElementId id1 = null;
                    if (!ClsHaraokoshiBase.PickBaseObject(uidoc, ref id1, "隅火打ベースを配置する腹起ベース"))//ClsRevitUtil.PickObject(uidoc, "隅火打ベースを配置する腹起ベース　1本目", "腹起ベース", ref id1))
                    {
                        return false;
                    }
                    m_HaraokoshiBaseID1 = id1;
                }

                //腹起ベースのレベルを取得
                FamilyInstance ShinInstLevel = doc.GetElement(m_HaraokoshiBaseID1) as FamilyInstance;
                ElementId levelID = ShinInstLevel.Host.Id;
                //事前にレビューが残っていれば削除
                DeleteCornerHiuchiBaseReview(doc);

                FamilyInstance instance = doc.GetElement(m_HaraokoshiBaseID1) as FamilyInstance;
                LocationCurve lCurve = instance.Location as LocationCurve;

                List<List<XYZ>> spList = ClsHaraokoshiBase.GetALLHaraokoshiBaseStartEndPointList(doc);
                List<List<XYZ>> resList = new List<List<XYZ>>();

                //List<XYZ> tmpXYZ = new List<XYZ>();
                //tmpXYZ.Add(lCurve.Curve.GetEndPoint(0));
                //tmpXYZ.Add(lCurve.Curve.GetEndPoint(1));
                //resList.Add(tmpXYZ);

                ClsYMSUtil.FindConnectedPoints(lCurve.Curve.GetEndPoint(0), lCurve.Curve.GetEndPoint(1), spList, ref resList);

                for (int i = 0; i < resList.Count; i++)
                {
                    List<XYZ> list1 = new List<XYZ>(); ;// resList[i];
                    List<XYZ> list2 = new List<XYZ>(); ;// resList[i+1];

                    if (i == resList.Count - 1)
                    {
                        list1 = resList[i];
                        list2 = resList[0];
                    }
                    else
                    {
                        list1 = resList[i];
                        list2 = resList[i + 1];
                    }

                    XYZ pntS = new XYZ();
                    XYZ pntE = new XYZ();
                    if (GetBaseStartEndPoint(doc, list1[0], list1[1], list2[0], list2[1], ref pntS, ref pntE))
                    {
                        using (Transaction CreateModelLines = new Transaction(doc, "Create Model Lines"))
                        {
                            try
                            {
                                CreateModelLines.Start();

                                TextNoteOptions options = new TextNoteOptions();
                                options.KeepRotatedTextReadable = false;
                                options.TypeId = doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);

                                XYZ centerPnt = new XYZ((pntS.X + pntE.X) / 2, (pntS.Y + pntE.Y) / 2, (pntS.Z + pntE.Z) / 2);

                                double distRevit = pntS.DistanceTo(pntE);
                                double distVisual = ClsRevitUtil.CovertFromAPI(distRevit);

                                Autodesk.Revit.DB.View activeView = doc.ActiveView;
                                TextNote note = TextNote.Create(doc, activeView.Id, centerPnt, distVisual.ToString(), options);
                                m_testNoteList.Add(note.Id);

                                Curve Curve = Line.CreateBound(pntS, pntE);

                                SketchPlane sketchPlane = SketchPlane.Create(doc, levelID);
                                ModelCurve c = doc.Create.NewModelCurve(Curve, sketchPlane);

                                if (m_Kousei == Kousei.Double) ClsRevitUtil.ChangeLineColor(doc, c.Id, ClsGlobal.m_redColor);

                                m_testLineList.Add(c.Id);
                                CreateModelLines.Commit();
                            }
                            catch (Exception ex)
                            {
                                CreateModelLines.RollBack();
                                MessageBox.Show(ex.Message);
                            }
                        }
                    }
                }
            }
            else if (m_ShoriType == ShoriType.Change)
            {
                m_HaraokoshiBaseID1 = m_HaraokoshiBaseChangeID1;
                m_HaraokoshiBaseID2 = m_HaraokoshiBaseChangeID2;
                if (m_HaraokoshiBaseID1 == null || m_HaraokoshiBaseID2 == null)
                {
                    MessageBox.Show("選択した隅火打ベースは腹起ベースと交差していません");
                }

                //腹起ベースのレベルを取得
                FamilyInstance ShinInstLevel = doc.GetElement(m_HaraokoshiBaseID1) as FamilyInstance;
                ElementId levelID = ShinInstLevel.Host.Id;
                //事前にレビューが残っていれば削除
                DeleteCornerHiuchiBaseReview(doc);

                XYZ pntS = new XYZ();
                XYZ pntE = new XYZ();
                if (GetBaseStartEndPoint(doc, ref pntS, ref pntE))
                {
                    using (Transaction CreateModelLines = new Transaction(doc, "Create Model Lines"))
                    {
                        try
                        {
                            CreateModelLines.Start();

                            TextNoteOptions options = new TextNoteOptions();
                            options.KeepRotatedTextReadable = false;
                            options.TypeId = doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);

                            XYZ centerPnt = new XYZ((pntS.X + pntE.X) / 2, (pntS.Y + pntE.Y) / 2, (pntS.Z + pntE.Z) / 2);

                            double distRevit = pntS.DistanceTo(pntE);
                            double distVisual = ClsRevitUtil.CovertFromAPI(distRevit);

                            Autodesk.Revit.DB.View activeView = doc.ActiveView;
                            TextNote note = TextNote.Create(doc, activeView.Id, centerPnt, distVisual.ToString(), options);
                            m_testNoteList.Add(note.Id);

                            Curve Curve = Line.CreateBound(pntS, pntE);

                            SketchPlane sketchPlane = SketchPlane.Create(doc, levelID);
                            ModelCurve c = doc.Create.NewModelCurve(Curve, sketchPlane);

                            if (m_Kousei == Kousei.Double) ClsRevitUtil.ChangeLineColor(doc, c.Id, ClsGlobal.m_redColor);

                            m_testLineList.Add(c.Id);
                            CreateModelLines.Commit();
                        }
                        catch (Exception ex)
                        {
                            CreateModelLines.RollBack();
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }



            return true;
        }
        /// <summary>
        /// コーナー火打を作図する
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public bool CreateCornerHiuchi(UIDocument uidoc)
        {
            Document doc = uidoc.Document;
            ////ワークセット
            //ClsWorkset clsWS = new ClsWorkset();
            //clsWS.SetWorkSetTest1(doc);

            //事前にレビューが残っていれば削除
            DeleteCornerHiuchiBaseReview(doc);

            //実際にベースを置く処理
            if (m_ShoriType == ShoriType.Manual)
            {
                for (; ; )
                {
                    try
                    {
                        //手動配置
                        if (m_HaraokoshiBaseID1 == null || m_HaraokoshiBaseID2 == null)
                        {
                            ElementId id1 = null;
                            if (!ClsHaraokoshiBase.PickBaseObject(uidoc, ref id1, "隅火打ベースを配置する腹起ベース　1本目"))
                            {
                                return false;
                            }
                            m_HaraokoshiBaseID1 = id1;

                            ElementId id2 = null;
                            if (!ClsHaraokoshiBase.PickBaseObject(uidoc, ref id2, "隅火打ベースを配置する腹起ベース　2本目"))
                            {
                                return false;
                            }
                            m_HaraokoshiBaseID2 = id2;
                        }

                        //腹起ベースのレベルを取得
                        FamilyInstance ShinInstLevel = doc.GetElement(m_HaraokoshiBaseID1) as FamilyInstance;
                        ElementId levelID = ShinInstLevel.Host.Id;


                        XYZ pntS = new XYZ();
                        XYZ pntE = new XYZ();
                        if (GetBaseStartEndPoint(doc, ref pntS, ref pntE))
                        {
                            CreateCornerHiuchi(doc, pntS, pntE, levelID, ClsRevitUtil.GetParameter(doc, m_HaraokoshiBaseID1, "段"));
                        }
                        m_HaraokoshiBaseID1 = null;
                        m_HaraokoshiBaseID2 = null;
                    }
                    catch (OperationCanceledException e)
                    {
                        break;
                    }
                }
            }
            else if (m_ShoriType == ShoriType.Auto)
            {

                //自動配置
                if (m_HaraokoshiBaseID1 == null)
                {
                    ElementId id1 = null;
                    if (!ClsHaraokoshiBase.PickBaseObject(uidoc, ref id1, "隅火打ベースを配置する基準となる腹起ベースを選択"))
                    {
                        return false;
                    }
                    m_HaraokoshiBaseID1 = id1;
                }

                FamilyInstance instance = doc.GetElement(m_HaraokoshiBaseID1) as FamilyInstance;
                LocationCurve lCurve = instance.Location as LocationCurve;

                List<List<XYZ>> spList = ClsHaraokoshiBase.GetALLHaraokoshiBaseStartEndPointList(doc);
                List<List<XYZ>> resList = new List<List<XYZ>>();

                //List<XYZ> tmpXYZ = new List<XYZ>();
                //tmpXYZ.Add(lCurve.Curve.GetEndPoint(0));
                //tmpXYZ.Add(lCurve.Curve.GetEndPoint(1));
                //resList.Add(tmpXYZ);

                ClsYMSUtil.FindConnectedPoints(lCurve.Curve.GetEndPoint(0), lCurve.Curve.GetEndPoint(1), spList, ref resList);
                //腹起ベースのレベルを取得
                FamilyInstance ShinInstLevel = doc.GetElement(m_HaraokoshiBaseID1) as FamilyInstance;
                ElementId levelID = ShinInstLevel.Host.Id;

                for (int i = 0; i < resList.Count; i++)
                {
                    List<XYZ> list1 = new List<XYZ>(); ;// resList[i];
                    List<XYZ> list2 = new List<XYZ>(); ;// resList[i+1];

                    if (i == resList.Count - 1)
                    {
                        list1 = resList[i];
                        list2 = resList[0];
                    }
                    else
                    {
                        list1 = resList[i];
                        list2 = resList[i + 1];
                    }

                    XYZ pntS = new XYZ();
                    XYZ pntE = new XYZ();
                    if (GetBaseStartEndPoint(doc, list1[0], list1[1], list2[0], list2[1], ref pntS, ref pntE))
                    {
                        CreateCornerHiuchi(doc, pntS, pntE, levelID, ClsRevitUtil.GetParameter(doc, m_HaraokoshiBaseID1, "段"));
                    }
                }

                //List<ElementId> haraList = ClsHaraokoshiBase.GetAllHaraokoshiBaseList(doc);
                //for (; ; )
                //{
                //    m_HaraokoshiBaseID2 = ClsRevitUtil.GetEndPointElment(doc, m_HaraokoshiBaseID1, haraList);

                //    if (m_HaraokoshiBaseID2 == null) break;//交点が2つ以上存在しない腹起ベースがあると処理を終了

                //    //腹起ベースのレベルを取得
                //    FamilyInstance ShinInstLevel = doc.GetElement(m_HaraokoshiBaseID1) as FamilyInstance;
                //    ElementId levelID = ShinInstLevel.Host.Id;

                //    XYZ pntS = new XYZ();
                //    XYZ pntE = new XYZ();
                //    if (GetBaseStartEndPoint(doc, ref pntS, ref pntE))
                //    {
                //        CreateCornerHiuchi(doc, pntS, pntE, levelID);
                //    }

                //    m_HaraokoshiBaseID1 = m_HaraokoshiBaseID2;

                //    //基準ベースと同じだったら抜ける
                //    if (id1 == m_HaraokoshiBaseID1) break;
                //}
            }
            else if (m_ShoriType == ShoriType.Replace)
            {
                //置換
                ElementId id1 = null;
                List<ElementId> modelLineIdList = new List<ElementId>();
                if (!ClsHaraokoshiBase.PickBaseObject(uidoc, ref id1, "隅火打ベースを配置する基準となる腹起ベースを選択"))
                {
                    return false;
                }
                m_HaraokoshiBaseID1 = id1;

                if (!ClsRevitUtil.PickObjects(uidoc, "隅火打ベースに置換するモデル線分", "モデル線分", ref modelLineIdList))
                {
                    return false;
                }
                if (modelLineIdList.Count < 1)
                {
                    MessageBox.Show("芯が選択されていません。");
                    return false;
                }

                //腹起ベースのレベルを取得
                FamilyInstance ShinInstLevel = doc.GetElement(m_HaraokoshiBaseID1) as FamilyInstance;
                ElementId levelID = ShinInstLevel.Host.Id;

                foreach (ElementId modelLineId in modelLineIdList)
                {
                    ModelLine modelLine = doc.GetElement(modelLineId) as ModelLine;
                    LocationCurve lCurve = modelLine.Location as LocationCurve;
                    if (lCurve == null)
                    {
                        return false;
                    }

                    XYZ tmpStPoint = lCurve.Curve.GetEndPoint(0);
                    XYZ tmpEdPoint = lCurve.Curve.GetEndPoint(1);

                    CreateCornerHiuchi(doc, tmpStPoint, tmpEdPoint, levelID, ClsRevitUtil.GetParameter(doc, m_HaraokoshiBaseID1, "段"));
                    using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                    {
                        t.Start();
                        ClsRevitUtil.Delete(doc, modelLineId);
                        t.Commit();
                    }
                }
            }
            else
            {
                m_HaraokoshiBaseID1 = m_HaraokoshiBaseChangeID1;
                m_HaraokoshiBaseID2 = m_HaraokoshiBaseChangeID2;
                if (m_HaraokoshiBaseID1 == null || m_HaraokoshiBaseID2 == null)
                {
                    MessageBox.Show("選択した隅火打ベースは腹起ベースと交差していません");
                }
                if (m_cornerHiuchiID == null) return false;
                ElementId id = m_cornerHiuchiID;
                FamilyInstance shinInstLevel = doc.GetElement(id) as FamilyInstance;
                //LocationCurve lCurve = shinInstLevel.Location as LocationCurve;
                //if (lCurve == null)
                //{
                //    return false;
                //}
                //XYZ tmpStPoint = lCurve.Curve.GetEndPoint(0);
                //XYZ tmpEdPoint = lCurve.Curve.GetEndPoint(1);
                ElementId levelID = shinInstLevel.Host.Id;
                string dan = ClsRevitUtil.GetParameter(doc, id, "段");
                //元のベースを削除
                using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                {
                    t.Start();
                    ClsRevitUtil.Delete(doc, id);
                    t.Commit();
                }
                m_cornerHiuchiID = null;
                //ベース作成
                XYZ pntS = new XYZ();
                XYZ pntE = new XYZ();
                if (GetBaseStartEndPoint(doc, ref pntS, ref pntE))
                {
                    CreateCornerHiuchi(doc, pntS, pntE, levelID, dan);
                }
                //CreateCornerHiuchi(doc, tmpStPoint, tmpEdPoint, levelID);
            }


            //ダイアログを閉じる処理
            CloseDlg();

            return true;
        }
        public bool CreateCornerHiuchi(Document doc, XYZ pntS, XYZ pntE, ElementId levelID, string dan = "同段")
        {
            string symbolFolpath = ClsZumenInfo.GetYMSFolder();
            string shinfamily = System.IO.Path.Combine(symbolFolpath, "ベース関係\\" + baseName + ".rfa");

            //シンボル読込
            if (!ClsRevitUtil.LoadFamilySymbolData(doc, shinfamily, baseName, out FamilySymbol sym))
            {
                return false;
            }
            Curve cv = Line.CreateBound(pntS, pntE);

            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                t.Start();
                ElementId CreatedID = ClsRevitUtil.Create(doc, cv, levelID, sym);
                ClsRevitUtil.SetParameter(doc, CreatedID, "配置角度", m_angle / 180 * Math.PI);
                ClsRevitUtil.SetMojiParameter(doc, CreatedID, "鋼材タイプ", m_SteelType);
                ClsRevitUtil.SetMojiParameter(doc, CreatedID, "鋼材サイズ", m_SteelSize);
                ClsRevitUtil.SetMojiParameter(doc, CreatedID, "端部部品(1)", m_TanbuParts1);
                ClsRevitUtil.SetMojiParameter(doc, CreatedID, "火打受ピースサイズ(1)", m_HiuchiUkePieceSize1);
                ClsRevitUtil.SetMojiParameter(doc, CreatedID, "端部部品(2)", m_TanbuParts2);
                ClsRevitUtil.SetMojiParameter(doc, CreatedID, "火打受ピースサイズ(2)", m_HiuchiUkePieceSize2);

                if (m_Kousei == Kousei.Double)
                {
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "構成", "ダブル");
                    ClsRevitUtil.ChangeLineColor(doc, CreatedID, ClsGlobal.m_redColor);
                    ClsRevitUtil.SetParameter(doc, CreatedID, "火打長さ(ダブル上)L1", ClsRevitUtil.CovertToAPI(m_HiuchiLengthDoubleUpperL1));
                    ClsRevitUtil.SetParameter(doc, CreatedID, "火打長さ(ダブル下)L2", ClsRevitUtil.CovertToAPI(m_HiuchiLengthDoubleUnderL2));
                    ClsRevitUtil.SetParameter(doc, CreatedID, "上下火打ずれ量a", ClsRevitUtil.CovertToAPI(m_HiuchiZureryo));
                }
                else
                {
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "段", dan);
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "構成", "シングル");
                    ClsRevitUtil.SetParameter(doc, CreatedID, "火打長さ(シングル)L", ClsRevitUtil.CovertToAPI(m_HiuchiLengthSingleL));
                }
                t.Commit();
            }
            return true;
        }

        public bool CreateCornerHiuchi(Document doc, XYZ pntS, XYZ pntE)
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
            Curve cv = Line.CreateBound(pntS, pntE);

            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                t.Start();
                FailureHandlingOptions failOpt = t.GetFailureHandlingOptions();
                failOpt.SetFailuresPreprocessor(new WarningSwallower());
                t.SetFailureHandlingOptions(failOpt);
                ElementId CreatedID = ClsRevitUtil.Create(doc, cv, levelID, sym);
                ClsRevitUtil.SetParameter(doc, CreatedID, "配置角度", m_angle / 180 * Math.PI);
                ClsRevitUtil.SetMojiParameter(doc, CreatedID, "鋼材タイプ", m_SteelType);
                ClsRevitUtil.SetMojiParameter(doc, CreatedID, "鋼材サイズ", m_SteelSize);
                ClsRevitUtil.SetMojiParameter(doc, CreatedID, "端部部品(1)", m_TanbuParts1);
                ClsRevitUtil.SetMojiParameter(doc, CreatedID, "火打受ピースサイズ(1)", m_HiuchiUkePieceSize1);
                ClsRevitUtil.SetMojiParameter(doc, CreatedID, "端部部品(2)", m_TanbuParts2);
                ClsRevitUtil.SetMojiParameter(doc, CreatedID, "火打受ピースサイズ(2)", m_HiuchiUkePieceSize2);

                if (m_Kousei == Kousei.Double)
                {
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "構成", "ダブル");
                    ClsRevitUtil.ChangeLineColor(doc, CreatedID, ClsGlobal.m_redColor);
                    ClsRevitUtil.SetParameter(doc, CreatedID, "火打長さ(ダブル上)L1", ClsRevitUtil.CovertToAPI(m_HiuchiLengthDoubleUpperL1));
                    ClsRevitUtil.SetParameter(doc, CreatedID, "火打長さ(ダブル下)L2", ClsRevitUtil.CovertToAPI(m_HiuchiLengthDoubleUnderL2));
                    ClsRevitUtil.SetParameter(doc, CreatedID, "上下火打ずれ量a", ClsRevitUtil.CovertToAPI(m_HiuchiZureryo));
                }
                else
                {
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "構成", "シングル");
                    ClsRevitUtil.SetParameter(doc, CreatedID, "火打長さ(シングル)L", ClsRevitUtil.CovertToAPI(m_HiuchiLengthSingleL));
                }
                t.Commit();
            }
            return true;
        }

        /// <summary>
        /// 作成したレビューを削除する
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public bool DeleteCornerHiuchiBaseReview(Document doc)
        {
            using (Transaction tr = new Transaction(doc, "del"))
            {
                tr.Start();
                ClsRevitUtil.Delete(doc, m_testLineList);
                ClsRevitUtil.Delete(doc, m_testNoteList);
                tr.Commit();
                m_testLineList = new List<ElementId>();
                m_testNoteList = new List<ElementId>();
            }
            return true;
        }
        /// <summary>
        /// コーナー火打ダイアログを閉じる
        /// </summary>
        public void CloseDlg()
        {
            DLG.DlgCreateCornerHiuchiBase dlgCreateCornerHiuchiBase = Application.thisApp.GetForm_dlgCreateCornerHiuchi();
            if (dlgCreateCornerHiuchiBase != null && dlgCreateCornerHiuchiBase.Visible)
            {
                dlgCreateCornerHiuchiBase.Close();
            }
        }

        /// <summary>
        /// コーナー火打の全長を更新する
        /// </summary>
        public void UpdateTotalLength()
        {
            DLG.DlgCreateCornerHiuchiBase dlgCreateCornerHiuchiBase = Application.thisApp.GetForm_dlgCreateCornerHiuchi();
            dlgCreateCornerHiuchiBase.UpdateTotalLength();
        }

        public (ElementId, ElementId) GetInsecHaraokoshiBase(Document doc, ElementId id)
        {
            List<ElementId> insecList = new List<ElementId>();

            FamilyInstance shinInstLevel = doc.GetElement(id) as FamilyInstance;
            LocationCurve lCurve = shinInstLevel.Location as LocationCurve;
            if (lCurve == null)
            {
                return (null, null);
            }

            //交差する腹起ベースを見つける
            List<ElementId> haraIdList = ClsHaraokoshiBase.GetAllHaraokoshiBaseList(doc);
            foreach (ElementId haraId in haraIdList)
            {
                FamilyInstance haraInst = doc.GetElement(haraId) as FamilyInstance;
                LocationCurve lCurveHara = haraInst.Location as LocationCurve;
                if (lCurveHara != null)
                {
                    XYZ insec = ClsRevitUtil.GetIntersection(lCurve.Curve, lCurveHara.Curve);//交点が2つ見つかる
                    if (insec != null)
                    {
                        insecList.Add(haraId);
                        if (insecList.Count == 2)
                        {
                            break;
                        }
                    }
                }
            }

            if (insecList.Count != 2) return (null, null);

            return (insecList[0], insecList[1]);
        }

        /// <summary>
        /// 隅火打ベース のみを単独選択
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="id">選択した 隅火打ベース のID</param>
        /// <param name="message">指示</param>
        /// <returns></returns>
        public static bool PickBaseObject(UIDocument uidoc, ref ElementId id, string message = baseName)
        {
            return ClsRevitUtil.PickObject(uidoc, message, baseName, ref id);
        }

        /// <summary>
        /// 隅火打ベース のみを複数選択
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="ids"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool PickBaseObjects(UIDocument uidoc, ref List<ElementId> ids, string message = baseName)
        {
            return ClsRevitUtil.PickObjectsPartFilter(uidoc, message + "を選択してください", baseName, ref ids);
        }

        /// <summary>
        /// 図面上の隅火打ベースを全て取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<ElementId> GetAllCornerHiuchiBaseList(Document doc)
        {
            //図面上の隅火打ベースを全て取得
            List<ElementId> htIdList = ClsRevitUtil.GetSelectCreatedFamilyInstanceList(doc, baseName);
            return htIdList;
        }

        /// <summary>
        ///  図面上の隅火打ベースを全てクラスで取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<ClsCornerHiuchiBase> GetAllClsCornerHiuchiBaseList(Document doc)
        {
            List<ClsCornerHiuchiBase> lstBase = new List<ClsCornerHiuchiBase>();

            List<ElementId> lstId = GetAllCornerHiuchiBaseList(doc);
            foreach (ElementId id in lstId)
            {
                ClsCornerHiuchiBase clsCH = new ClsCornerHiuchiBase();
                clsCH.SetParameter(doc, id);
                lstBase.Add(clsCH);
            }

            return lstBase;
        }

        /// <summary>
        /// 指定したIDから隅火打ベースクラスを取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public void SetParameter(Document doc, ElementId id)
        {
            FamilyInstance shinInstLevel = doc.GetElement(id) as FamilyInstance;
            LocationCurve lCurve = shinInstLevel.Location as LocationCurve;
            if (lCurve == null)
            {
                return;
            }

            m_Floor = shinInstLevel.Host.Name;
            m_Kousei = ClsRevitUtil.GetParameter(doc, id, "構成") == "シングル" ? ClsCornerHiuchiBase.Kousei.Single : ClsCornerHiuchiBase.Kousei.Double;
            if (m_Kousei == ClsCornerHiuchiBase.Kousei.Double)
            {
                m_HiuchiLengthDoubleUpperL1 = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetParameterDouble(doc, id, "火打長さ(ダブル上)L1"));
                m_HiuchiLengthDoubleUnderL2 = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetParameterDouble(doc, id, "火打長さ(ダブル下)L2"));
                m_HiuchiZureryo = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetParameterDouble(doc, id, "上下火打ずれ量a"));
            }
            else
            {
                m_HiuchiLengthSingleL = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetParameterDouble(doc, id, "火打長さ(シングル)L"));
            }
            m_angle = ClsRevitUtil.GetParameterDouble(doc, id, "配置角度") / Math.PI * 180;
            m_SteelType = ClsRevitUtil.GetParameter(doc, id, "鋼材タイプ");
            m_SteelSize = ClsRevitUtil.GetParameter(doc, id, "鋼材サイズ");
            m_TanbuParts1 = ClsRevitUtil.GetParameter(doc, id, "端部部品(1)");
            m_HiuchiUkePieceSize1 = ClsRevitUtil.GetParameter(doc, id, "火打受ピースサイズ(1)");
            m_TanbuParts2 = ClsRevitUtil.GetParameter(doc, id, "端部部品(2)");
            m_HiuchiUkePieceSize2 = ClsRevitUtil.GetParameter(doc, id, "火打受ピースサイズ(2)");

            m_ShoriType = ClsCornerHiuchiBase.ShoriType.Change;
            m_cornerHiuchiID = id;

            return;
        }

        ///// <summary>
        ///// 入隅かどうか判定する
        ///// </summary>
        ///// <param name="cv1"></param>
        ///// <param name="cv2"></param>
        ///// <returns></returns>
        //public static bool CheckIrizumi(Curve cv1, Curve cv2, ref bool res)
        //{
        //    XYZ pIntersect = RevitUtil.ClsRevitUtil.GetIntersection(cv1, cv2);
        //    if (pIntersect == null)
        //    {
        //        return false;
        //    }

        //    bool bAFirst = false;
        //    if (pIntersect.DistanceTo(cv1.GetEndPoint(1)) <= pIntersect.DistanceTo(cv2.GetEndPoint(1)))
        //        bAFirst = true;

        //    XYZ vec1 = new XYZ();
        //    XYZ p = new XYZ();
        //    if (bAFirst)
        //    {
        //        vec1 = cv1.GetEndPoint(1) - cv1.GetEndPoint(0);
        //        p = cv2.GetEndPoint(1) - cv1.GetEndPoint(0);
        //    }
        //    else
        //    {
        //        vec1 = cv2.GetEndPoint(1) - cv2.GetEndPoint(0);
        //        p = cv1.GetEndPoint(1) - cv2.GetEndPoint(0);
        //    }

        //    XYZ crossProduct = vec1.CrossProduct(p);
        //    if (crossProduct.Z > 0)
        //    {
        //        res = true;
        //    }
        //    else if (crossProduct.Z < 0)
        //    {
        //        res = false;
        //    }
        //    else
        //    {
        //        res = false;
        //        return false;
        //    }

        //    return true;
        //}

        #endregion

    }
}
