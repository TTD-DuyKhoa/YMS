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
    public class ClsKiribariHiuchiBase
    {
        #region 定数
        public const string baseName = "切梁火打ベース";
        public const string BASEID端部 = "ベースID端";
        #endregion

        #region Enum
        /// <summary>
        /// 配置方法：処理タイプ
        /// </summary>
        public enum ShoriType
        {
            KiriKiri,
            KiriHara,
            SanjikuHara
        }

        /// <summary>
        /// 配置方法：処理方法
        /// </summary>
        public enum ShoriHoho
        {
            Auto,
            Manual,
            Replace
        }

        /// <summary>
        /// 配置方法：作成方法
        /// </summary>
        public enum CreateType
        {
            Both,
            AnyLengthManual,
            AnyLengthAuto
        }

        /// <summary>
        /// 作成方法：作成方法
        /// </summary>
        public enum CreateHoho
        {
            Single,
            Double,
        }
        #endregion

        #region プロパティ
        /// <summary>
        /// 配置方法：処理タイプ
        /// </summary>
        public ShoriType m_ShoriType { get; set; }

        /// <summary>
        /// 配置方法：処理方法
        /// </summary>
        public ShoriHoho m_ShoriHoho { get; set; }

        /// <summary>
        /// 配置方法：作成方法
        /// </summary>
        public CreateType m_CreateType { get; set; }

        /// <summary>
        /// 作成方法：ｼﾝｸﾞﾙ/ﾀﾞﾌﾞﾙ
        /// </summary>
        public CreateHoho m_CreateHoho { get; set; }

        /// <summary>
        /// 作成方法：鋼材タイプ
        /// </summary>
        public string m_SteelType { get; set; }

        /// <summary>
        /// 作成方法：鋼材サイズ（シングル）
        /// </summary>
        public string m_SteelSizeSingle { get; set; }

        /// <summary>
        /// 作成方法：鋼材サイズ（ダブル上）
        /// </summary>
        public string m_SteelSizeDoubleUpper { get; set; }

        /// <summary>
        /// 作成方法：鋼材サイズ（ダブル下）
        /// </summary>
        public string m_SteelSizeDoubleUnder { get; set; }

        /// <summary>
        /// 作成方法：火打ち長さ（シングル）L
        /// </summary>
        public int m_HiuchiLengthSingleL { get; set; }

        /// <summary>
        /// 作成方法：火打ち長さ（ダブル上）L１
        /// </summary>
        public int m_HiuchiLengthDoubleUpperL1 { get; set; }

        /// <summary>
        /// 作成方法：火打ち長さ（ダブル下）L２
        /// </summary>
        public int m_HiuchiLengthDoubleUnderL2 { get; set; }

        /// <summary>
        /// 作成方法：上下火打ずれ量a
        /// </summary>
        public int m_HiuchiZureRyo { get; set; }

        /// <summary>
        /// 部品：部品タイプ（切梁側）
        /// </summary>
        public string m_PartsTypeKiriSide { get; set; }

        /// <summary>
        /// 部品：部品サイズ（切梁側）
        /// </summary>
        public string m_PartsSizeKiriSide { get; set; }

        /// <summary>
        /// 部品：部品タイプ（腹起側）
        /// </summary>
        public string m_PartsTypeHaraSide { get; set; }

        /// <summary>
        /// 部品：部品サイズ（切梁側）
        /// </summary>
        public string m_PartsSizeHaraSide { get; set; }

        /// <summary>
        /// 角度
        /// </summary>
        public double m_Angle { get; set; }

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
        public ClsKiribariHiuchiBase()
        {
            Init();
        }
        #endregion

        #region メソッド

        /// <summary>
        /// 初期化
        /// </summary>
        public void Init()
        {
            m_ShoriType = ShoriType.KiriHara;
            m_ShoriHoho = ShoriHoho.Auto;
            m_CreateType = CreateType.Both;
            m_CreateHoho = CreateHoho.Single;
            m_SteelType = string.Empty;
            m_SteelSizeSingle = string.Empty;
            m_SteelSizeDoubleUpper = string.Empty;
            m_SteelSizeDoubleUnder = string.Empty;
            m_HiuchiLengthSingleL = 0;
            m_HiuchiLengthDoubleUpperL1 = 0;
            m_HiuchiLengthDoubleUnderL2 = 0;
            m_HiuchiZureRyo = 0;
            m_PartsTypeKiriSide = string.Empty;
            m_PartsSizeKiriSide = string.Empty;
            m_PartsTypeHaraSide = string.Empty;
            m_PartsSizeHaraSide = string.Empty;
            m_Angle = 0;
            m_Floor = string.Empty;
            m_Dan = string.Empty;
            m_FlgEdit = false;
        }

        public bool CreateKiribariHiuchiBase(UIDocument uidoc, ElementId id1, ElementId id2, ElementId levelID, string dan, XYZ picPoint1 = null, XYZ picPoint2 = null)
        {
            //ドキュメントを取得
            Document doc = uidoc.Document;
            XYZ pntS = new XYZ();
            XYZ pntE = new XYZ();
            XYZ insec = new XYZ();
            if (m_CreateType == CreateType.Both)
            {
                //2回とか呼ぶ
                //通常作成
                if (GetBaseStartEndPoint(doc, id1, id2, ref pntS, ref pntE, ref insec))
                {
                    XYZ newPoint = pntS;
                    if (picPoint1 != null)
                    {
                        if (!ClsRevitUtil.JudgeOnLine(insec, pntS, picPoint1))
                        {
                            Line kiriLine = Line.CreateBound(insec, pntS);
                            XYZ dirKiriLine = kiriLine.Direction;
                            newPoint = ClsRevitUtil.GetDistanceNewPoint(insec, -dirKiriLine, ClsRevitUtil.CovertFromAPI(kiriLine.Length));
                        }
                    }
                    CreateKiribariHiuchiBase(doc, newPoint, pntE, levelID, dan);
                    //角度を変えて再度呼ぶ
                    m_Angle = -m_Angle;
                    if (GetBaseStartEndPoint(doc, id1, id2, ref pntS, ref pntE, ref insec, inner: true))
                    {
                        newPoint = pntS;
                        CreateKiribariHiuchiBase(doc, newPoint, pntE, levelID, dan);
                    }

                }


            }
            else if (m_CreateType == CreateType.AnyLengthManual)
            {
                //通常作成
                if (GetBaseStartEndPoint(doc, id1, id2, ref pntS, ref pntE, ref insec))
                {
                    XYZ newPoint = pntS;
                    XYZ newEndPoint = pntE;
                    bool judge1 = false;
                    if (picPoint1 != null)//自動の時、切梁-腹起の時用に分岐
                    {
                        if (!ClsRevitUtil.JudgeOnLine(insec, pntS, picPoint1))
                        {
                            Line kiriLine = Line.CreateBound(insec, pntS);
                            XYZ dirKiriLine = kiriLine.Direction;
                            newPoint = ClsRevitUtil.GetDistanceNewPoint(insec, -dirKiriLine, ClsRevitUtil.CovertFromAPI(kiriLine.Length));
                            judge1 = true;//始点を反転させた場合反転させたままにするため
                        }
                    }

                    if (picPoint2 != null)
                    {
                        if (!ClsRevitUtil.JudgeOnLine(insec, pntE, picPoint2))
                        {
                            //指定した点が内分の為角度を変えて再度位置を指定
                            m_Angle = -m_Angle;
                            if (GetBaseStartEndPoint(doc, id1, id2, ref pntS, ref pntE, ref insec, inner:true))
                            {
                                if (!judge1)
                                    newPoint = pntS;
                                newEndPoint = pntE;
                            }
                        }
                    }

                    CreateKiribariHiuchiBase(doc, newPoint, newEndPoint, levelID, dan);
                }
            }
            else//自動不明
            {
                //切梁火打ベース指定
                ElementId idKiriHiuch = null;
                if (!PickBaseObject(uidoc, ref idKiriHiuch))
                {
                    return false;
                }
                FamilyInstance instKiriHiuchi = doc.GetElement(idKiriHiuch) as FamilyInstance;
                Curve cvKiriHiuch = (instKiriHiuchi.Location as LocationCurve).Curve;

                //通常作成
                if (GetBaseStartEndPoint(doc, id1, id2, ref pntS, ref pntE, ref insec))
                {
                    XYZ newStPoint = cvKiriHiuch.GetEndPoint(0);

                    Line kiriLine = Line.CreateBound(insec, cvKiriHiuch.GetEndPoint(1));
                    XYZ dirKiriLine = kiriLine.Direction;
                    XYZ newEndPoint = ClsRevitUtil.GetDistanceNewPoint(insec, -dirKiriLine, ClsRevitUtil.CovertFromAPI(kiriLine.Length));

                    CreateKiribariHiuchiBase(doc, newStPoint, newEndPoint, levelID, dan);
                }
            }
            return false;
        }
        public bool CreateKiribariHiuchiBase(Document doc, XYZ pntS, XYZ pntE, ElementId levelID, string dan)
        {
            string symbolFolpath = ClsZumenInfo.GetYMSFolder();
            string shinfamily = System.IO.Path.Combine(symbolFolpath, "ベース関係\\" + baseName + ".rfa");

            //シンボル読込
            if (!ClsRevitUtil.LoadFamilySymbolData(doc, shinfamily, baseName, out FamilySymbol sym))
            {
                return false;
            }
            if (dan == "下段" && m_ShoriType == ClsKiribariHiuchiBase.ShoriType.KiriKiri)
            {
                XYZ p = pntS;
                pntS = pntE;
                pntE = p;
            }
            Curve cv = Line.CreateBound(pntS, pntE);

            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                t.Start();
                ElementId CreatedID = ClsRevitUtil.Create(doc, cv, levelID, sym);

                ClsRevitUtil.SetMojiParameter(doc, CreatedID, "鋼材タイプ", m_SteelType);
                SetShoriType(doc, CreatedID, m_ShoriType);

                if (m_CreateHoho == CreateHoho.Double)
                {
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "構成", "ダブル");
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "鋼材サイズ(ダブル上)", m_SteelSizeDoubleUpper);
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "鋼材サイズ(ダブル下)", m_SteelSizeDoubleUnder);
                    ClsRevitUtil.ChangeLineColor(doc, CreatedID, ClsGlobal.m_redColor);
                    ClsRevitUtil.SetParameter(doc, CreatedID, "火打長さ(ダブル上)L1", ClsRevitUtil.CovertToAPI(m_HiuchiLengthDoubleUpperL1));
                    ClsRevitUtil.SetParameter(doc, CreatedID, "火打長さ(ダブル下)L2", ClsRevitUtil.CovertToAPI(m_HiuchiLengthDoubleUnderL2));
                    ClsRevitUtil.SetParameter(doc, CreatedID, "上下火打ずれ量a", ClsRevitUtil.CovertToAPI(m_HiuchiZureRyo));
                }
                else
                {
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "構成", "シングル");
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "鋼材サイズ(シングル)", m_SteelSizeSingle);
                    ClsRevitUtil.SetParameter(doc, CreatedID, "火打長さ(シングル)L", ClsRevitUtil.CovertToAPI(m_HiuchiLengthSingleL));
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "段", dan);

                }
                ClsRevitUtil.SetMojiParameter(doc, CreatedID, "部品タイプ(切梁側)", m_PartsTypeKiriSide);
                ClsRevitUtil.SetMojiParameter(doc, CreatedID, "部品サイズ(切梁側)", m_PartsSizeKiriSide);
                ClsRevitUtil.SetMojiParameter(doc, CreatedID, "部品タイプ(腹起側)", m_PartsTypeHaraSide);
                ClsRevitUtil.SetMojiParameter(doc, CreatedID, "部品サイズ(腹起側)", m_PartsSizeHaraSide);
                m_Angle = m_Angle < 0 ? -m_Angle : m_Angle;
                ClsRevitUtil.SetParameter(doc, CreatedID, "角度", m_Angle / 180 * Math.PI);
                t.Commit();
            }
            return true;
        }

        public bool CreateKiribariHiuchiBase(Document doc, XYZ pntS, XYZ pntE)
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
            if (m_Dan == "下段" && m_ShoriType == ClsKiribariHiuchiBase.ShoriType.KiriKiri)
            {
                XYZ p = pntS;
                pntS = pntE;
                pntE = p;
            }
            Curve cv = Line.CreateBound(pntS, pntE);

            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                t.Start();
                FailureHandlingOptions failOpt = t.GetFailureHandlingOptions();
                failOpt.SetFailuresPreprocessor(new WarningSwallower());
                t.SetFailureHandlingOptions(failOpt);
                ElementId CreatedID = ClsRevitUtil.Create(doc, cv, levelID, sym);

                ClsRevitUtil.SetMojiParameter(doc, CreatedID, "鋼材タイプ", m_SteelType);
                SetShoriType(doc, CreatedID, m_ShoriType);

                if (m_CreateHoho == CreateHoho.Double)
                {
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "構成", "ダブル");
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "鋼材サイズ(ダブル上)", m_SteelSizeDoubleUpper);
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "鋼材サイズ(ダブル下)", m_SteelSizeDoubleUnder);
                    ClsRevitUtil.ChangeLineColor(doc, CreatedID, ClsGlobal.m_redColor);
                    ClsRevitUtil.SetParameter(doc, CreatedID, "火打長さ(ダブル上)L1", ClsRevitUtil.CovertToAPI(m_HiuchiLengthDoubleUpperL1));
                    ClsRevitUtil.SetParameter(doc, CreatedID, "火打長さ(ダブル下)L2", ClsRevitUtil.CovertToAPI(m_HiuchiLengthDoubleUnderL2));
                    ClsRevitUtil.SetParameter(doc, CreatedID, "上下火打ずれ量a", ClsRevitUtil.CovertToAPI(m_HiuchiZureRyo));
                }
                else
                {
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "構成", "シングル");
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "鋼材サイズ(シングル)", m_SteelSizeSingle);
                    ClsRevitUtil.SetParameter(doc, CreatedID, "火打長さ(シングル)L", ClsRevitUtil.CovertToAPI(m_HiuchiLengthSingleL));
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "段", m_Dan);

                }
                ClsRevitUtil.SetMojiParameter(doc, CreatedID, "部品タイプ(切梁側)", m_PartsTypeKiriSide);
                ClsRevitUtil.SetMojiParameter(doc, CreatedID, "部品サイズ(切梁側)", m_PartsSizeKiriSide);
                ClsRevitUtil.SetMojiParameter(doc, CreatedID, "部品タイプ(腹起側)", m_PartsTypeHaraSide);
                ClsRevitUtil.SetMojiParameter(doc, CreatedID, "部品サイズ(腹起側)", m_PartsSizeHaraSide);
                m_Angle = m_Angle < 0 ? -m_Angle : m_Angle;
                ClsRevitUtil.SetParameter(doc, CreatedID, "角度", m_Angle / 180 * Math.PI);
                t.Commit();
            }
            return true;
        }

        /// <summary>
        /// 腹起ベースが二本選ばれている・切梁火打ベース長さが確定していることを前提に、切梁火打ベースの始終点を返す
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="startPnt"></param>
        /// <param name="endPnt"></param>
        /// <returns></returns>
        public bool GetBaseStartEndPoint(Document doc, ElementId id1, ElementId id2, ref XYZ startPnt, ref XYZ endPnt, ref XYZ intersectPoint, bool inner = false)
        {
            double angle = m_Angle;
            double length;
            if (m_CreateHoho == CreateHoho.Double)
            {
                if (m_HiuchiLengthDoubleUpperL1 < m_HiuchiLengthDoubleUnderL2)
                {
                    length = m_HiuchiLengthDoubleUnderL2 + m_HiuchiZureRyo;
                }
                else
                {
                    length = m_HiuchiLengthDoubleUpperL1 + m_HiuchiZureRyo;
                }
            }
            else
            {
                length = m_HiuchiLengthSingleL;
            }


            FamilyInstance inst1 = doc.GetElement(id1) as FamilyInstance;
            LocationCurve lCurve = inst1.Location as LocationCurve;
            if (lCurve == null)
            {
                return false;
            }
            Curve cv = lCurve.Curve;
            XYZ tmpStPoint = lCurve.Curve.GetEndPoint(0);
            XYZ tmpEdPoint = lCurve.Curve.GetEndPoint(1);

            FamilyInstance inst2 = doc.GetElement(id2) as FamilyInstance;
            LocationCurve lCurve2 = inst2.Location as LocationCurve;
            if (lCurve2 == null)
            {
                return false;
            }
            Curve cv2 = lCurve2.Curve;
            XYZ tmpStPoint2 = lCurve2.Curve.GetEndPoint(0);
            XYZ tmpEdPoint2 = lCurve2.Curve.GetEndPoint(1);

            //ベース同士の交点取得
            XYZ intersectPnt = ClsRevitUtil.GetIntersection(cv, cv2);//.CalculateIntersectionPoint(tmpStPoint, tmpEdPoint, tmpStPoint2, tmpEdPoint2);

            if (intersectPnt == null)
            {
                cv = CreateInfiniteCurve(cv);
                cv2 = CreateInfiniteCurve(cv2);
                intersectPnt = ClsRevitUtil.GetIntersection(cv, cv2);
                if (intersectPnt == null)
                    return false;
            }

            //angleA = 入力された角度（一本目の腹起ベースと入力された長さの線分とのなす角）
            double angleA = angle;
            //angleC = 一本目の腹起ベースと二本目の腹起ベースのなす角
            var st = tmpStPoint;
            var ed = tmpEdPoint;
            if (inner)
            {
                st = tmpEdPoint;
                ed = tmpStPoint;
            }
            if (!ClsGeo.GEO_EQ(st, intersectPnt))
            {
                if (!inner)
                {
                    st = tmpStPoint;
                    ed = tmpEdPoint;
                }
            }
            
            double angleC = ClsRevitUtil.CalculateAngleBetweenLines(st, ed, tmpStPoint2, tmpEdPoint2);
            //double angleC = ClsRevitUtil.CalculateAngleBetweenLines(tmpStPoint, tmpEdPoint, tmpStPoint2, tmpEdPoint2);
            ////入力角度を-に変更していない場合はCのなす角を反転させる
            if (angleA > 0 && inner)
            {
                angleC = 180 - angleC;//
            }
            //angleB = 二本目の腹起ベースと入力された長さの線分とのなす角
            double angleB = 180 - (angleA < 0 ? -angleA : angleA) - angleC;// (angleC < 0 ? -angleC : angleC);//Cの判定 - angleC

            string kouzaiSizeA = ClsRevitUtil.GetParameter(doc, id1, "鋼材サイズ");
            string kouzaiSizeB = ClsRevitUtil.GetParameter(doc, id2, "鋼材サイズ");
            double abLength = GetTotalLength(angleC, kouzaiSizeA, kouzaiSizeB); //length;
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
            intersectPoint = intersectPnt;

            return true;
        }

        /// <summary>
        /// 腹起ベースが二本選ばれている・隅火打ベース長さが確定していることを前提に、隅火打ベースの始終点を返す
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="startPnt"></param>
        /// <param name="endPnt"></param>
        /// <returns></returns>
        public bool GetBaseStartEndPoint(Document doc, XYZ base1startPnt, XYZ base1endPnt,
            XYZ base2startPnt, XYZ base2endPnt, ref XYZ startPnt, ref XYZ endPnt)

        {
            double angle = m_Angle;
            double length = m_HiuchiLengthSingleL;//長さシングル、ダブルあるダブルは全長で良いかもL1＞L2+ずれ量a



            XYZ tmpStPoint = base1startPnt;
            XYZ tmpEdPoint = base1endPnt;

            Curve cvInst = Line.CreateBound(tmpStPoint, tmpEdPoint);

            XYZ tmpStPoint2 = base2startPnt;
            XYZ tmpEdPoint2 = base2endPnt;

            Curve cvInst2 = Line.CreateBound(tmpStPoint2, tmpEdPoint2);

            //ベース同士の交点取得
            XYZ intersectPnt = ClsRevitUtil.GetIntersection(cvInst, cvInst2);//.CalculateIntersectionPoint(tmpStPoint, tmpEdPoint, tmpStPoint2, tmpEdPoint2);

            if (intersectPnt == null)
            {
                return false;
            }

            //angleA = 入力された角度（一本目の腹起ベースと入力された長さの線分とのなす角）
            double angleA = angle;
            //angleC = 一本目の腹起ベースと二本目の腹起ベースのなす角
            double angleC = ClsRevitUtil.CalculateAngleBetweenLines(tmpStPoint, tmpEdPoint, tmpStPoint2, tmpEdPoint2);
            //angleB = 二本目の腹起ベースと入力された長さの線分とのなす角
            double angleB = 180 - angleA - angleC;

            double abLength = length;
            double bcLength = 0.0;
            double caLength = 0.0;


            ClsRevitUtil.CalculateTriangleSides(abLength, angleA, angleB, angleC, ref caLength, ref bcLength);


            XYZ pntTmp = new XYZ();
            if (intersectPnt.DistanceTo(tmpStPoint) > intersectPnt.DistanceTo(tmpEdPoint))
            {
                pntTmp = tmpStPoint;
            }
            else
            {
                pntTmp = tmpEdPoint;
            }

            XYZ pntS = ClsRevitUtil.MoveCoordinates(intersectPnt, pntTmp, ClsRevitUtil.ConvertDoubleGeo2Revit(caLength));

            pntTmp = new XYZ();
            if (intersectPnt.DistanceTo(tmpStPoint2) > intersectPnt.DistanceTo(tmpEdPoint2))
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
            if (danElement == danHara)
            {
                if (m_CreateHoho == CreateHoho.Double)
                {
                    MessageBox.Show("同段において構成ダブルが選択されています。");
                    return false;
                }
            }
            else
            {
                if (m_CreateHoho == CreateHoho.Single)
                {
                    MessageBox.Show("異段において構成シングルが選択されています。");
                    return false;
                }
            }
            return true;
        }

        public (ElementId, ElementId) GetInsecHaraokoshi_KiribariBase(Document doc, ElementId id)
        {
            List<ElementId> insecList = new List<ElementId>();

            FamilyInstance shinInstLevel = doc.GetElement(id) as FamilyInstance;
            LocationCurve lCurve = shinInstLevel.Location as LocationCurve;
            if (lCurve == null)
            {
                return (null, null);
            }

            //交差する切梁ベースを見つける
            List<ElementId> kiriIdList = ClsKiribariBase.GetAllKiribariBaseList(doc);
            foreach (ElementId kiriId in kiriIdList)
            {
                FamilyInstance kiriInst = doc.GetElement(kiriId) as FamilyInstance;
                LocationCurve lCurveKiri = kiriInst.Location as LocationCurve;
                if (lCurveKiri != null)
                {
                    XYZ insec = ClsRevitUtil.GetIntersection(lCurve.Curve, lCurveKiri.Curve);
                    if (insec != null)
                    {
                        insecList.Add(kiriId);
                        if (insecList.Count == 2)
                        {
                            break;
                        }
                    }
                }
            }
            //交差する腹起ベースを見つける
            List<ElementId> haraIdList = ClsHaraokoshiBase.GetAllHaraokoshiBaseList(doc);
            foreach (ElementId haraId in haraIdList)
            {
                FamilyInstance haraInst = doc.GetElement(haraId) as FamilyInstance;
                LocationCurve lCurveHara = haraInst.Location as LocationCurve;
                if (lCurveHara != null)
                {
                    XYZ insec = ClsRevitUtil.GetIntersection(lCurve.Curve, lCurveHara.Curve);
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
        /// 端部部品の作図（ベースのエレメントIDが必須）
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public bool CreateTanbuParts(Document doc)
        {
            ElementId baseID = m_ElementId;
            if (baseID == null)
            {
                return false;
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

            //始端側の梁と終点側の梁を選別
            List<ElementId> lstHari = YMS.ClsYMSUtil.GetIntersectionHaraKiriBase(doc, baseID);
            if (lstHari.Count < 2)
                return false;

            ElementId idA = null;
            ElementId idB = null;
            double dSMin = 0;
            double dEMin = 0;
            XYZ sInsec = new XYZ();

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
                    sInsec = p;
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

            XYZ insec = ClsRevitUtil.GetIntersection(lCurveA.Curve, lCurveB.Curve); ;//対象のベースと接しているベース同士の交点
            XYZ vecSEAInsec = (tmpEdPointA - insec).Normalize();
            XYZ vecSEInsec = (tmpStPoint - insec).Normalize();

            //角度
            double dHaraAngle = ClsRevitUtil.CalculateAngleBetweenLines(tmpStPointA, tmpEdPointA, tmpEdPointB, tmpStPointB);
            double dAngleA = ClsRevitUtil.CalculateAngleBetweenLines(tmpStPoint, tmpEdPoint, tmpEdPointA, tmpStPointA);
            double dAngleB = ClsRevitUtil.CalculateAngleBetweenLines(tmpStPoint, tmpEdPoint, tmpEdPointB, tmpStPointB);


            //挿入位置を取得
            double dA = 0;
            double dB = 0;
            double dMain = 0;
            //GetTotalLength(dHaraAngle, ref dMain, ref dA, ref dB);
            XYZ pS = tmpStPoint + (vecSE * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(dA));
            XYZ pE = tmpEdPoint + (-vecSE * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(dB));


            //■■■共通処理■■■
            FamilyInstance lv = doc.GetElement(baseID) as FamilyInstance;
            ElementId levelID = lv.Host.Id;

            XYZ VecBS2SI = Line.CreateBound(tmpStPointB, sInsec).Direction;
            XYZ insecVecE = Line.CreateBound(insec, pE).Direction;

            FamilySymbol sym, sym2;
            string shinfamily = Master.ClsHiuchiCsv.GetFamilyPath(m_PartsSizeKiriSide);
            string shinfamilyName = RevitUtil.ClsRevitUtil.GetFamilyName(shinfamily);
            //string tanbuPieceSize = Master.ClsHiuchiCsv.GetSize(m_tanbuStart, m_kouzaiSize);
            double sizeA = ClsRevitUtil.CovertToAPI(Master.ClsHiuchiCsv.GetWidth(m_PartsSizeKiriSide) / 2);
            //シンボル読込※タイプを持っている
            if (m_PartsTypeKiriSide == Master.ClsHiuchiCsv.TypeNameFVP || m_PartsTypeKiriSide == Master.ClsHiuchiCsv.TypeNameKaitenVP)
            {
                if (!ClsRevitUtil.LoadFamilyData(doc, shinfamily, out Family tanbuFam))
                {
                    //return false;
                }
                sym = (ClsRevitUtil.GetFamilySymbol(doc, shinfamilyName, "切梁火打"));
            }
            else
            {
                if (!ClsRevitUtil.LoadFamilySymbolData(doc, shinfamily, shinfamilyName, out sym))
                {
                    //return false;
                }
            }

            string shinfamily2 = Master.ClsHiuchiCsv.GetFamilyPath(m_PartsSizeHaraSide);
            string shinfamilyName2 = RevitUtil.ClsRevitUtil.GetFamilyName(shinfamily2);
            //string tanbuPieceSize2 = Master.ClsHiuchiCsv.GetSize(m_tanbuEnd, m_kouzaiSize);
            double sizeB = ClsRevitUtil.CovertToAPI(Master.ClsHiuchiCsv.GetWidth(m_PartsSizeHaraSide) / 2);
            if (m_PartsTypeHaraSide == Master.ClsHiuchiCsv.TypeNameFVP || m_PartsTypeHaraSide == Master.ClsHiuchiCsv.TypeNameKaitenVP)
            {
                //自在火打ちの処理は保留
                if (!ClsRevitUtil.LoadFamilyData(doc, shinfamily2, out Family tanbuFam))
                {
                    //return false; ;
                }
                sym2 = (ClsRevitUtil.GetFamilySymbol(doc, shinfamilyName2, "切梁火打"));
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
                    //if(m_TanbuFirst)//一番最初に作成される端部部品が正常に作図されないため
                    //{
                    //    ElementId first = ClsRevitUtil.Create(doc, pS, levelID, sym);
                    //    ClsRevitUtil.Delete(doc, first);
                    //    tx.Commit();
                    //    tx.Start();
                    //    m_TanbuFirst = false;
                    //}

                    if (m_PartsTypeKiriSide == Master.ClsHiuchiCsv.TypeNameFVP)
                    {
                        //自在火打ちの処理は保留
                    }
                    else
                    {
                        ElementId CreatedID = ClsRevitUtil.Create(doc, pS, levelID, sym);
                        FamilyInstance ch = doc.GetElement(CreatedID) as FamilyInstance;

                        XYZ dirV = ch.HandOrientation;//ch.FacingOrientation;//new XYZ(1, 0, 0);//ファミリインスタンスの向き
                        XYZ dirF = ch.FacingOrientation;

                        double dVectAngle = dirV.AngleTo(vecSEA);
                        double dFacAngle = dirF.AngleTo(vecSEA);
                        Line axis = Line.CreateBound(pS, pS + XYZ.BasisZ);

                        if (ClsGeo.IsLeft(vecSEA, insecVecE))
                        {
                            ch.flipFacing(); //ch.flipHand();
                            //dVectAngle = -dVectAngle;
                            dFacAngle = -dFacAngle;
                        }

                        //切梁ベースの終点に配置する場合、反転させる
                        if (ClsGeo.GEO_EQ(tmpEdPointA, insec) || !ClsGeo.GEO_EQ(vecSEInsec, vecSEAInsec))
                        {
                            ch.flipHand();
                            //dVectAngle = Math.PI/2 + dVectAngle;
                        }
                        ClsRevitUtil.RotateElement(doc, CreatedID, axis, -dVectAngle + dFacAngle * 2);

                        if (danA == "上段")
                            ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", sizeA);
                        else if (danA == "下段")
                            ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", -sizeA);
                        else
                            ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", 0.0);
                    }
                }
                //■■■終点側処理■■■
                if (sym2 != null)
                {
                    if (m_PartsTypeHaraSide == Master.ClsHiuchiCsv.TypeNameFVP)
                    {
                        //自在火打ちの処理は保留
                    }
                    else
                    {
                        ElementId CreatedID = ClsRevitUtil.Create(doc, pE, levelID, sym2);
                        FamilyInstance ch = doc.GetElement(CreatedID) as FamilyInstance;


                        XYZ dirV = -ch.HandOrientation; //ch.FacingOrientation;// new XYZ(-1, 0, 0);//ファミリインスタンスの向き
                        XYZ dirF = ch.FacingOrientation;
                        double dVectAngle = dirV.AngleTo(vecSEB);// + Math.PI / 2;//3.14
                        double dFacAngle = dirF.AngleTo(vecSEA);//0
                        Line axis = Line.CreateBound(pE, pE + XYZ.BasisZ);

                        if (!ClsGeo.IsLeft(vecSEB, VecBS2SI))
                        {

                            vecSEB = (tmpStPointB - tmpEdPointB).Normalize();
                            dVectAngle = dirV.AngleTo(vecSEB);
                        }

                        if (ClsGeo.IsLeft(vecSEA, insecVecE))
                        {
                            //ch.flipFacing();
                            ch.flipHand();
                            //dVectAngle = -dVectAngle;
                            dFacAngle = -dFacAngle;
                        }
                        ch.flipFacing();
                        //切梁ベースの終点に配置する場合、反転させる
                        if (ClsGeo.GEO_EQ(tmpEdPointA, insec) || !ClsGeo.GEO_EQ(vecSEInsec, vecSEAInsec))
                        {
                            //ch.flipFacing();
                            ch.flipHand();
                            //dVectAngle = Math.PI/2 + dVectAngle;
                            dFacAngle = 0.0;
                        }
                        ClsRevitUtil.RotateElement(doc, CreatedID, axis, -dVectAngle + dFacAngle * 2);
                        if (danB == "上段")
                            ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", sizeB);
                        else if (danB == "下段")
                            ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", -sizeB);
                        else
                            ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", 0.0);
                    }
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
        public bool CreateTanbuParts1(Document doc, ref XYZ pS, ref XYZ pE)//自在火打ち対応
        {
            ElementId baseID = m_ElementId;
            if (baseID == null)
            {
                return false;
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


            //始端側の梁と終点側の梁を選別
            List<ElementId> lstHari = YMS.ClsYMSUtil.GetIntersectionHaraKiriBase(doc, baseID);

            if (lstHari.Count < 2)
                return false;

            ElementId idA = null;
            ElementId idB = null;
            double dSMin = 0;
            double dEMin = 0;
            XYZ sInsec = new XYZ();
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
                    sInsec = p;
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
            Curve cvA = lCurveA.Curve;
            string danA = ClsRevitUtil.GetParameter(doc, idA, "段");
            //始点は必ず切梁
            ClsYMSUtil.GetDifferenceWithAllBase(doc, baseID, out double diff, out double diff2);

            //梁（終点側）
            Element elB = doc.GetElement(idB);
            LocationCurve lCurveB = elB.Location as LocationCurve;
            if (lCurveB == null)
            {
                return false;
            }
            Curve cvB = lCurveB.Curve;
            string danB = ClsRevitUtil.GetParameter(doc, idB, "段");


            //if (m_ShoriType == ShoriType.KiriKiri)
            //{
            //    cvA = CreateInfiniteCurve(cvA);
            //    cvB = CreateInfiniteCurve(cvB);
            //}
            var cvInfiA = cvA;// CreateInfiniteCurve(cvA);
            var cvInfiB = cvB;// CreateInfiniteCurve(cvB);

            XYZ insec = ClsRevitUtil.GetIntersection(cvInfiA, cvInfiB); //対象のベースと接しているベース同士の交点
            if (insec == null)
            {
                cvInfiA = CreateInfiniteCurve(cvA);
                cvInfiB = CreateInfiniteCurve(cvB);
                insec = ClsRevitUtil.GetIntersection(cvInfiA, cvInfiB);
                if (insec == null)
                    return false;
            }
                

            XYZ tmpStPointA = cvA.GetEndPoint(0);
            XYZ tmpEdPointA = cvA.GetEndPoint(1);
            XYZ vecSEA = (tmpEdPointA - tmpStPointA).Normalize();

            XYZ tmpStPointB = cvB.GetEndPoint(0);
            XYZ tmpEdPointB = cvB.GetEndPoint(1);
            XYZ vecSEB = (tmpEdPointB - tmpStPointB).Normalize();

            XYZ vecSEAInsec = (tmpEdPointA - insec).Normalize();
            XYZ vecSEInsec = (tmpStPoint - insec).Normalize();

            //角度
            double dHaraAngle = ClsRevitUtil.CalculateAngleBetweenLines(tmpStPointA, tmpEdPointA, tmpEdPointB, tmpStPointB);
            double dAngleA = ClsRevitUtil.CalculateAngleBetweenLines(tmpStPoint, tmpEdPoint, tmpEdPointA, tmpStPointA);
            double dAngleB = ClsRevitUtil.CalculateAngleBetweenLines(tmpStPoint, tmpEdPoint, tmpEdPointB, tmpStPointB);


            //挿入位置を取得
            string kouzaiSizeA = ClsRevitUtil.GetParameter(doc, idA, "鋼材サイズ");
            string kouzaiSizeB = ClsRevitUtil.GetParameter(doc, idB, "鋼材サイズ");
            double dA = 0;
            double dB = 0;
            double dMain = 0;
            GetTotalLength(dHaraAngle, kouzaiSizeA, kouzaiSizeB, ref dMain, ref dA, ref dB, m_ShoriType == ShoriType.KiriKiri);
            pS = tmpStPoint + (vecSE * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(dA));
            pE = tmpEdPoint + (-vecSE * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(dB));


            //■■■共通処理■■■
            FamilyInstance lv = doc.GetElement(baseID) as FamilyInstance;
            ElementId levelID = lv.Host.Id;

            XYZ VecBS2SI = Line.CreateBound(tmpStPointB, sInsec).Direction;
            XYZ insecVecE = Line.CreateBound(insec, pE).Direction;

            string steelSizeA = m_SteelSizeSingle;
            string steelSizeB = m_SteelSizeSingle;
            if(m_CreateHoho == CreateHoho.Double)
            {
                if(danA == "上段")
                {
                    steelSizeA = m_SteelSizeDoubleUpper;
                    steelSizeB = m_SteelSizeDoubleUnder;
                }
                else if(danA == "下段")
                {
                    steelSizeA = m_SteelSizeDoubleUnder;
                    steelSizeB = m_SteelSizeDoubleUpper;
                }
            }

            FamilySymbol sym, sym2, sym3 = null;//自在火打ち対応
            string shinfamily = Master.ClsHiuchiCsv.GetFamilyPath(m_PartsSizeKiriSide);
            string shinfamilyName = RevitUtil.ClsRevitUtil.GetFamilyName(shinfamily);
            double sizeA = ClsRevitUtil.CovertToAPI(Master.ClsYamadomeCsv.GetWidth(steelSizeA) / 2) + diff;


            //シンボル読込※タイプを持っている
            if (m_PartsTypeKiriSide == Master.ClsHiuchiCsv.TypeNameFVP || m_PartsTypeKiriSide == Master.ClsHiuchiCsv.TypeNameKaitenVP)
            {
                if (!ClsRevitUtil.LoadFamilyData(doc, shinfamily, out Family tanbuFam))
                {
                    //return false;
                }
                sym = (ClsRevitUtil.GetFamilySymbol(doc, shinfamilyName, "切梁火打"));
            }
            else
            {
                if (!ClsRevitUtil.LoadFamilySymbolData(doc, shinfamily, shinfamilyName, out sym))
                {
                    //return false;
                }
            }

            string shinfamily2 = Master.ClsHiuchiCsv.GetFamilyPath(m_PartsSizeHaraSide);
            string shinfamilyName2 = RevitUtil.ClsRevitUtil.GetFamilyName(shinfamily2);
            double sizeB = ClsRevitUtil.CovertToAPI(Master.ClsYamadomeCsv.GetWidth(steelSizeB) / 2) + diff2;
            if (m_PartsTypeHaraSide == Master.ClsHiuchiCsv.TypeNameFVP || m_PartsTypeHaraSide == Master.ClsHiuchiCsv.TypeNameKaitenVP)
            {
                if (!ClsRevitUtil.LoadFamilyData(doc, shinfamily2, out Family tanbuFam))
                {
                    //return false; ;
                }
                sym2 = (ClsRevitUtil.GetFamilySymbol(doc, shinfamilyName2, "切梁火打"));

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
                    //切梁火打の始点から接続するベース(切梁-切梁or切梁-腹起)同士の交点までのベクトル
                    var baseS2Insec = (insec - tmpStPoint).Normalize();
                    //切梁火打と接続するベース(切梁-切梁or切梁-腹起)同士の交点から切梁火打の始点までのベクトル
                    var insec2BaseS = (tmpStPoint - insec).Normalize();
                    if (m_PartsTypeKiriSide == Master.ClsHiuchiCsv.TypeNameFVP)//始点側に自在火打受は付かない
                    {
                        string steelSize = string.Empty;
                        if (m_CreateHoho == CreateHoho.Double && danA == "上段") steelSize = m_SteelSizeDoubleUpper;
                        else if (m_CreateHoho == CreateHoho.Double && danA == "下段") steelSize = m_SteelSizeDoubleUnder;
                        else steelSize = m_SteelSizeSingle;

                        tx.Start();
                        //自在火打ち対応 -start
                        //自在火打の挿入・位置調整・回転
                        ElementId CreatedID = ClsRevitUtil.Create(doc, tmpStPoint, levelID, sym);
                        FamilyInstance ch = doc.GetElement(CreatedID) as FamilyInstance;
                        ClsKariKouzai.SetBaseId(doc, CreatedID, baseID, BASEID端部);
                        ClsKariKouzai.SetKariKouzaiFlag(doc, CreatedID);
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

                        XYZ axisDirection = ClsRevitUtil.RotateVector(dirV, -Math.PI);
                        //Plane plane = Plane.CreateByThreePoints(tmpStPoint + new XYZ(0, 1, 0), tmpStPoint + new XYZ(0, 0, 1), tmpStPoint);

                        //RevitUtil.ClsRevitUtil.MirrorElement(doc, CreatedID, plane);
                        //ElementTransformUtils.MoveElement(doc, CreatedID, tmpStPoint - ((LocationPoint)ch.Location).Point);
                        //double dW = RevitUtil.ClsRevitUtil.CovertToAPI(Master.ClsYamadomeCsv.GetWidth(m_SteelSize));
                        double dW = RevitUtil.ClsRevitUtil.CovertToAPI(ClsYMSUtil.GetKouzaiHabaFromBase(doc, baseID));
                        XYZ dirMove = new XYZ(-dW / 2, 0, 0);//移動長さ// * -Math.Cos(ClsGeo.Deg2Rad(dAngleA * 2))
                        ElementTransformUtils.MoveElement(doc, CreatedID, dirMove);
                        ClsRevitUtil.RotateElement(doc, CreatedID, axis, dVectAngle);

                        //ブルマンの挿入位置を算出
                        XYZ direction = vecSE.Normalize(); // 方向ベクトルを正規化
                        XYZ leftDirection = new XYZ(-direction.Y, direction.X, 0); // 左側方向に90度回転
                        XYZ moveVector = leftDirection * (dW / 2) * 0.8;
                        Curve cvBase = Line.CreateBound(tmpStPoint + moveVector, tmpEdPoint + moveVector);
                        XYZ direction2 = vecSEA.Normalize(); // 方向ベクトルを正規化
                        XYZ leftDirection2 = new XYZ(-direction2.Y, direction2.X, 0); // 左側方向に90度回転
                        string target = Master.ClsHiuchiCsv.GetTargetShuzai(m_PartsTypeKiriSide);
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
                        ClsKariKouzai.SetBaseId(doc, CreatedID, baseID, BASEID端部);
                        tx.Commit();

                        tx.Start();
                        XYZ dirV = ch.HandOrientation;//ch.FacingOrientation;//new XYZ(1, 0, 0);//ファミリインスタンスの向き
                        XYZ dirF = ch.FacingOrientation;

                        //
                        var a = dirV.AngleOnPlaneTo(insec2BaseS, XYZ.BasisZ);
                        var b = ClsGeo.Rad2Deg(a);
                        if (!ClsGeo.IsLeft(dirV, insec2BaseS))
                        {
                            a = dirV.AngleOnPlaneTo(baseS2Insec, XYZ.BasisZ);
                            if (!ClsGeo.IsLeft(baseS2Insec, vecSE))
                            {
                                ch.flipFacing();
                            }
                        }
                        else
                        {
                            ch.flipHand();
                            if (ClsGeo.IsLeft(baseS2Insec, vecSE))
                            {
                                ch.flipFacing();
                            }
                        }
                        //

                        double dVectAngle = dirV.AngleTo(vecSEA);
                        double dFacAngle = dirF.AngleTo(vecSEA);
                        Line axis = Line.CreateBound(pS, pS + XYZ.BasisZ);

                        //
                        //var VecSB2SI = (tmpEdPoint - tmpStPointA).Normalize();
                        //if (!ClsGeo.IsLeft(vecSEA, VecSB2SI))
                        //{
                        //    var vecSEA2 = (tmpStPointA - tmpEdPointA).Normalize();
                        //    dVectAngle = dirV.AngleTo(vecSEA2);
                        //}
                        //
                        //
                        if (ClsGeo.IsLeft(baseS2Insec, vecSE))
                        {
                            //ch.flipFacing();
                        }
                        if (!ClsGeo.GEO_EQ(insec2BaseS, vecSEAInsec))
                        {
                            //ch.flipHand();
                        }
                        //
                        //ch.flipHand();
                        //if (ClsGeo.IsLeft(vecSEA, insecVecE))
                        //{
                        //    ch.flipFacing();
                        //    dFacAngle = -dFacAngle;
                        //}

                        //切梁ベースの終点に配置する場合、反転させる
                        //if (ClsGeo.GEO_EQ(tmpEdPointA, insec) || !ClsGeo.GEO_EQ(vecSEInsec, vecSEAInsec))
                        //{
                        //    ch.flipHand();
                        //}

                        ClsRevitUtil.RotateElement(doc, CreatedID, axis, a);// - dVectAngle + dFacAngle * 2);

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
                    //切梁火打の終点から接続するベース(切梁-切梁or切梁-腹起)同士の交点までのベクトル
                    var baseE2Insec = (insec - tmpEdPoint).Normalize();
                    //切梁火打と接続するベース(切梁-切梁or切梁-腹起)同士の交点から切梁火打の終点までのベクトル
                    var insec2BaseE = (tmpEdPoint - insec).Normalize();
                    if (m_PartsTypeHaraSide == Master.ClsHiuchiCsv.TypeNameFVP)
                    {
                        string steelSize = string.Empty;
                        if (m_CreateHoho == CreateHoho.Double && danB == "上段") steelSize = m_SteelSizeDoubleUpper;
                        else if (m_CreateHoho == CreateHoho.Double && danB == "下段") steelSize = m_SteelSizeDoubleUnder;
                        else steelSize = m_SteelSizeSingle;

                        tx.Start();
                        ElementId CreatedID = ClsRevitUtil.Create(doc, tmpEdPoint, levelID, sym2);
                        ClsKariKouzai.SetKariKouzaiFlag(doc, CreatedID);
                        FamilyInstance ch = doc.GetElement(CreatedID) as FamilyInstance;
                        ClsKariKouzai.SetBaseId(doc, CreatedID, baseID, BASEID端部);
                        tx.Commit();

                        tx.Start();
                        XYZ bCenter = (tmpEdPoint + tmpStPoint) / 2;
                        XYZ bCorner = ClsRevitUtil.GetIntersection(cvInfiA, cvInfiB);//lCurveA.Curve, lCurveB.Curve);
                        bool bMirror = ClsGeo.IsLeft(bCenter - bCorner, tmpEdPoint - bCorner);

                        int nMirror = 1;
                        if (bMirror)
                        {
                            nMirror = -1;
                            ch.flipFacing();
                        }

                        XYZ dirV = new XYZ(-1, 0, 0) * nMirror;//ファミリインスタンスの向き
                        double dVectAngle = dirV.AngleTo(vecSEB);
                        Line axis = Line.CreateBound(tmpEdPoint, tmpEdPoint + XYZ.BasisZ);

                        if (!ClsGeo.IsLeft(dirV, vecSEB))
                        {
                            dVectAngle = -dVectAngle;
                        }

                        //Plane plane = Plane.CreateByThreePoints(tmpEdPoint + new XYZ(0, 1, 0), tmpEdPoint + new XYZ(0, 0, 1), tmpEdPoint);

                        //RevitUtil.ClsRevitUtil.MirrorElement(doc, CreatedID, plane);
                        //ElementTransformUtils.MoveElement(doc, CreatedID, tmpEdPoint - ((LocationPoint)ch.Location).Point);
                        //double dW = RevitUtil.ClsRevitUtil.CovertToAPI(Master.ClsYamadomeCsv.GetWidth(m_SteelSize));
                        double dW = RevitUtil.ClsRevitUtil.CovertToAPI(ClsYMSUtil.GetKouzaiHabaFromBase(doc, baseID) / 2);
                        var insec2BaseS = (tmpStPoint - insec).Normalize();//vecSEBと計算する
                        //var insecBaseAngle = insec2BaseS.AngleOnPlaneTo(vecSEB, XYZ.BasisZ);
                        var insecBaseAngle = vecSEB.AngleOnPlaneTo(insec2BaseS, XYZ.BasisZ);
                        var baseS2Insec = (insec - tmpStPoint).Normalize();
                        if (!ClsGeo.IsLeft(vecSE, baseS2Insec))
                        {
                            insecBaseAngle = Math.PI - insecBaseAngle;
                        }
                        var insecBaseAngleCos = Math.Cos(insecBaseAngle);

                        XYZ dirMove = new XYZ(-dW * (1 - insecBaseAngleCos), 0, 0);//移動長さ// * -Math.Cos(ClsGeo.Deg2Rad(dAngleB * 2))
                        ElementTransformUtils.MoveElement(doc, CreatedID, dirMove);

                        ClsRevitUtil.RotateElement(doc, CreatedID, axis, dVectAngle);

                        //ブルマンの挿入位置を算出
                        XYZ direction = vecSE.Normalize(); // 方向ベクトルを正規化
                        XYZ leftDirection = new XYZ(-direction.Y, direction.X, 0) * nMirror; // 左側方向に90度回転
                        XYZ moveVector = leftDirection * (dW) * 0.8;
                        Curve cvBase = Line.CreateBound(tmpStPoint + moveVector, tmpEdPoint + moveVector);
                        XYZ direction2 = vecSEB.Normalize(); // 方向ベクトルを正規化
                        XYZ leftDirection2 = new XYZ(-direction2.Y, direction2.X, 0); // 左側方向に90度回転
                        string target = Master.ClsHiuchiCsv.GetTargetShuzai(m_PartsSizeHaraSide);

                        double dFVPWidth = RevitUtil.ClsRevitUtil.CovertToAPI(Master.ClsYamadomeCsv.GetWidth(target));
                        XYZ moveVector2 = leftDirection2 * dFVPWidth;
                        Curve cvHariS = Line.CreateBound(tmpStPointB + moveVector2, tmpEdPointB + moveVector2);
                        XYZ pBuru = RevitUtil.ClsRevitUtil.GetIntersection(cvBase, cvHariS);

                        //ブルマンの挿入
                        ElementId CreatedID_B = ClsRevitUtil.Create(doc, pBuru, levelID, sym3);
                        ClsKariKouzai.SetKariKouzaiFlag(doc, CreatedID_B);
                        Line axis2 = Line.CreateBound(pBuru, pBuru + XYZ.BasisZ);
                        ClsRevitUtil.RotateElement(doc, CreatedID_B, axis2, dVectAngle + ClsGeo.Deg2Rad(30 * nMirror));

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
                    else if (m_PartsTypeHaraSide == Master.ClsHiuchiCsv.TypeNameKaitenVP)
                    {
                        tx.Start();
                        ElementId CreatedID = ClsRevitUtil.Create(doc, tmpEdPoint, levelID, sym2);
                        ClsKariKouzai.SetKariKouzaiFlag(doc, CreatedID);
                        FamilyInstance ch = doc.GetElement(CreatedID) as FamilyInstance;

                        //腹起ベースに対して全面接地するよう回転
                        double dVectAngle = vecSEB.AngleOnPlaneTo(XYZ.BasisX, XYZ.BasisZ);
                        Line axis = Line.CreateBound(tmpEdPoint, tmpEdPoint + XYZ.BasisZ);
                        ClsRevitUtil.RotateElement(doc, CreatedID, axis, -dVectAngle);
                        //回転後に回転ピース回転軸が切梁上に乗るように移動させる
                        var peiceAngle = (-vecSE).AngleOnPlaneTo(ch.FacingOrientation, XYZ.BasisZ);
                        var movement = 191.0 * Math.Sin(peiceAngle);
                        ClsRevitUtil.MoveFamilyInstance(ch, movement, vecSEB);
                        //回転ピースの面を回転
                        if (!ClsGeo.IsLeft(-vecSE, ch.FacingOrientation))
                            peiceAngle = -ch.FacingOrientation.AngleOnPlaneTo(-vecSE, XYZ.BasisZ);
                        ClsRevitUtil.SetParameter(doc, CreatedID, "θ_角度", (Math.PI / 2 + peiceAngle));

                        //回転ピースの回転向きに制限があるため反転させる必要があるケースも存在する
                        //ケース判別方法要検討
                        //ファミリが回転しない
                        if (Math.PI * 7 / 12 < Math.PI / 2 + peiceAngle)
                        {
                            if (ch.flipFacing())
                                ClsRevitUtil.SetParameter(doc, CreatedID, "θ_角度", Math.PI / 2 - peiceAngle);
                        }

                        var test1 = ClsGeo.Rad2Deg(dVectAngle);
                        var test2 = ClsGeo.Rad2Deg(peiceAngle);

                        if (danB == "上段")
                            ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", sizeB + diff);
                        else if (danB == "下段")
                            ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", -sizeB - diff);
                        else
                            ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", 0.0);
                        //火打長さ分位置をずらす
                        //double dPartsThicknessE = Master.ClsHiuchiCsv.GetThickness(m_tanbuEnd, m_kouzaiSize);
                        //pE += (-vecSE * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(dPartsThicknessE));
                        tx.Commit();
                    }
                    else
                    {
                        tx.Start();
                        ElementId CreatedID = ClsRevitUtil.Create(doc, pE, levelID, sym2);
                        ClsKariKouzai.SetKariKouzaiFlag(doc, CreatedID);
                        FamilyInstance ch = doc.GetElement(CreatedID) as FamilyInstance;
                        ClsKariKouzai.SetBaseId(doc, CreatedID, baseID, BASEID端部);
                        tx.Commit();

                        tx.Start();
                        XYZ dirV = ch.HandOrientation; //ch.FacingOrientation;// new XYZ(-1, 0, 0);//ファミリインスタンスの向き
                        XYZ dirF = ch.FacingOrientation;
                        double dVectAngle = dirV.AngleTo(vecSEB);// + Math.PI / 2;//3.14
                        double dFacAngle = dirF.AngleTo(vecSEA);//0
                        Line axis = Line.CreateBound(pE, pE + XYZ.BasisZ);

                        //if (!ClsGeo.IsLeft(vecSEB, VecBS2SI))
                        //{
                        //    vecSEB = (tmpStPointB - tmpEdPointB).Normalize();
                        //    dVectAngle = dirV.AngleTo(vecSEB);
                        //}

                        //if (ClsGeo.IsLeft(vecSEA, insecVecE))
                        //{
                        //    ch.flipHand();
                        //    dFacAngle = -dFacAngle;
                        //}

                        //ch.flipFacing();

                        ////切梁ベースの終点に配置する場合、反転させる
                        //if (ClsGeo.GEO_EQ(tmpEdPointA, insec) || !ClsGeo.GEO_EQ(vecSEInsec, vecSEAInsec))
                        //{
                        //    ch.flipHand();
                        //    dFacAngle = 0.0;
                        //}

                        //
                        var a = dirV.AngleOnPlaneTo(insec2BaseE, XYZ.BasisZ);
                        var b = ClsGeo.Rad2Deg(a);
                        if (!ClsGeo.IsLeft(dirV, insec2BaseE))
                        {
                            a = dirV.AngleOnPlaneTo(baseE2Insec, XYZ.BasisZ);
                            if (!ClsGeo.IsLeft(baseE2Insec, -vecSE))
                            {
                                ch.flipFacing();
                            }
                        }
                        else
                        {
                            ch.flipHand();
                            if (ClsGeo.IsLeft(baseE2Insec, -vecSE))
                            {
                                ch.flipFacing();
                            }
                        }
                        //

                        ClsRevitUtil.RotateElement(doc, CreatedID, axis, a);// -dVectAngle + dFacAngle * 2);
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
        public bool GetTanbuPartsPoints(Document doc, XYZ tmpStPoint, XYZ tmpEdPoint, ref XYZ pS, ref XYZ pE, string sanjikuSize = null)
        {
            var cv = Line.CreateBound(tmpStPoint, tmpEdPoint);
            XYZ vecSE = (tmpEdPoint - tmpStPoint).Normalize();

            //始端側の梁と終点側の梁を選別
            List<ElementId> lstHari = new List<ElementId>();
            lstHari.AddRange(ClsHaraokoshiBase.GetAllHaraokoshiBaseList(doc).ToList());
            lstHari.AddRange(ClsKiribariBase.GetAllKiribariBaseList(doc).ToList());

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
            Curve cvA = lCurveA.Curve;
            string danA = ClsRevitUtil.GetParameter(doc, idA, "段");

            //梁（終点側）
            Element elB = doc.GetElement(idB);
            LocationCurve lCurveB = elB.Location as LocationCurve;
            if (lCurveB == null)
            {
                return false;
            }
            Curve cvB = lCurveB.Curve;
            string danB = ClsRevitUtil.GetParameter(doc, idB, "段");


            if (m_ShoriType == ShoriType.KiriKiri)
            {
                cvA = CreateInfiniteCurve(cvA);
                cvB = CreateInfiniteCurve(cvB);
            }
            XYZ insec = ClsRevitUtil.GetIntersection(cvA, cvB); //対象のベースと接しているベース同士の交点
            if (insec == null)
                return false;

            XYZ tmpStPointA = cvA.GetEndPoint(0);
            XYZ tmpEdPointA = cvA.GetEndPoint(1);

            XYZ tmpStPointB = cvB.GetEndPoint(0);
            XYZ tmpEdPointB = cvB.GetEndPoint(1);

            //角度
            double dHaraAngle = ClsRevitUtil.CalculateAngleBetweenLines(tmpStPointA, tmpEdPointA, tmpEdPointB, tmpStPointB);

            //挿入位置を取得
            string kouzaiSizeA = ClsRevitUtil.GetParameter(doc, idA, "鋼材サイズ");
            string kouzaiSizeB = ClsRevitUtil.GetParameter(doc, idB, "鋼材サイズ");
            double dA = 0;
            double dB = 0;
            double dMain = 0;
            GetTotalLength(dHaraAngle, kouzaiSizeA, kouzaiSizeB, ref dMain, ref dA, ref dB, m_ShoriType == ShoriType.KiriKiri);
            if(m_ShoriType == ClsKiribariHiuchiBase.ShoriType.SanjikuHara && sanjikuSize != null)
            {
                dA += Master.ClsSanjikuPieceCSV.GetSideLength(sanjikuSize);
            }
            pS = tmpStPoint + (vecSE * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(dA));
            pE = tmpEdPoint + (-vecSE * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(dB));

            return true;
        }
        /// <summary>
        /// 切梁火打の全長取得処理
        /// </summary>
        /// <param name="dHaraAngle">切梁-腹起or切梁角度</param>
        /// <returns></returns>
        public double GetTotalLength(double dHaraAngle, string kouzaiSizeA, string kouzaiSizeB)
        {
            double dMain = 0.0;
            double dA = 0.0;
            double dB = 0.0;
            return GetTotalLength(dHaraAngle, kouzaiSizeA, kouzaiSizeB, ref dMain, ref dA, ref dB);
        }

        /// <summary>
        /// 切梁火打の全長取得処理
        /// </summary>
        /// <param name="dHaraAngle">腹起間角度</param>
        /// <param name="kouzaiSizeA">Aの鋼材サイズ</param>
        /// <param name="kouzaiSizeB">Bの鋼材サイズ</param>
        /// <returns></returns>
        public double GetTotalLength(double dHaraAngle, string kouzaiSizeA, string kouzaiSizeB, ref double dMain, ref double dOffsetA, ref double dOffsetB, bool bKiri = false)
        {
            double dTotalLength = 0;//切梁の長さ
            double d = 0.0;
            double d2 = 0.0;
            if (m_CreateHoho == CreateHoho.Single)
            {
                dTotalLength += m_HiuchiLengthSingleL;
            }
            else
            {
                //ダブルの場合は長い方の値で計算
                d = m_HiuchiLengthDoubleUpperL1;
                d2 = m_HiuchiLengthDoubleUnderL2;
                d = (d >= d2 ? d : d2);
                d2 = m_HiuchiZureRyo;
                dTotalLength += d + d2;
            }
            double dAngleA = 0;
            double dAngleB = 0;
            double dKouzaiWidthA = 0;
            double dKouzaiWidthB = 0;
            double dPartsThicknessA = 0;
            double dPartsThicknessB = 0;

            //角度
            if (m_Angle < 0)//両方作成時に角度をマイナスにする処理があるため
            {
                dAngleA = -m_Angle;
                dAngleB = 180 - 90 + m_Angle;//dHaraAngleではなく90
            }
            else
            {
                dAngleA = m_Angle;
                dAngleB = 180 - 90 - m_Angle;
            }

            //鋼材幅
            if (m_PartsTypeKiriSide != Master.ClsHiuchiCsv.TypeNameFVP)
            {
                //string targetShuzai = Master.ClsHiuchiCsv.GetTargetShuzai(m_HiuchiUkePieceSize1);
                dKouzaiWidthA = Master.ClsHiuchiCsv.GetWidth(m_PartsSizeKiriSide);
            }
            else
            {
                dKouzaiWidthA = Master.ClsHiuchiCsv.GetWidth(m_PartsSizeKiriSide);
            }

            if (m_PartsTypeHaraSide != Master.ClsHiuchiCsv.TypeNameFVP)
            {
                //string targetShuzai = Master.ClsHiuchiCsv.GetTargetShuzai(m_HiuchiUkePieceSize2);
                dKouzaiWidthB = Master.ClsHiuchiCsv.GetWidth(m_PartsSizeHaraSide);
            }
            else
            {
                dKouzaiWidthB = Master.ClsHiuchiCsv.GetWidth(m_PartsSizeHaraSide);
            }

            
            // 端部部品厚
            dPartsThicknessA = Master.ClsHiuchiCsv.GetThickness(m_PartsSizeKiriSide);
            dPartsThicknessB = Master.ClsHiuchiCsv.GetThickness(m_PartsSizeHaraSide);

            //ベース長さを求めるために長さを求める必要がある
            if (m_PartsTypeHaraSide == Master.ClsHiuchiCsv.TypeNameKaitenVP)
            {
                dKouzaiWidthB = 0;//一旦0
                dPartsThicknessB = 0;//
            }
            //string kouzaiSizeA = string.Empty;
            //string kouzaiSizeB = string.Empty;
            //if (m_CreateHoho == CreateHoho.Double)
            //{
            //    kouzaiSizeA = m_SteelSizeDoubleUpper;
            //    kouzaiSizeB = m_SteelSizeDoubleUnder;
            //}
            //else
            //{
            //    kouzaiSizeA = m_SteelSizeSingle;
            //    kouzaiSizeB = m_SteelSizeSingle;
            //}

            double web = Master.ClsYamadomeCsv.GetWeb(kouzaiSizeA);
            double webB = Master.ClsYamadomeCsv.GetWeb(kouzaiSizeB);
            //double flange = Master.ClsYamadomeCsv.GetFlange(kouzaiSizeB);

            //変更
            double dA = ClsYMSUtil.CalculateOffsets(dAngleA, dKouzaiWidthA, dPartsThicknessA + (web / 2));
            double dB;
            if (bKiri)
                dB = ClsYMSUtil.CalculateOffsets(dAngleB, dKouzaiWidthB, dPartsThicknessB + (webB / 2));
            else
                dB = ClsYMSUtil.CalculateOffsets(dAngleB, dKouzaiWidthB, dPartsThicknessB);
           

            dMain = dTotalLength;
            dOffsetA = dA;
            dOffsetB = dB;

            return ClsGeo.FloorAtDigitAdjust(0, dTotalLength + dOffsetA + dOffsetB);
        }

        public bool CreateHojyoPeace(Document doc, XYZ tmpStPoint, XYZ tmpEdPoint, string steelSize, double dTotalLength, string dan)
        {
            int hundredsPlace = ClsGeo.GetNthDigitNum((int)dTotalLength, 3);
            if (hundredsPlace == 0 || hundredsPlace == 5)
                return false;

            ElementId baseID = m_ElementId;
            if (baseID == null)
            {
                return false;
            }

            //ベース
            FamilyInstance ShinInstLevel = doc.GetElement(baseID) as FamilyInstance;
            ElementId levelID = ShinInstLevel.Host.Id;
            //string dan = ClsRevitUtil.GetParameter(doc, baseID, "段");
            double size = ClsRevitUtil.CovertToAPI(Master.ClsYamadomeCsv.GetWidth(steelSize) / 2);
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

            string hojyo2 = Master.ClsSupportPieceCsv.GetSize(steelSize, 2);
            string path2 = Master.ClsSupportPieceCsv.GetFamilyPath(hojyo2);
            string familyName2 = RevitUtil.ClsRevitUtil.GetFamilyName(path2);

            string hojyo3 = Master.ClsSupportPieceCsv.GetSize(steelSize, 3);
            string path3 = Master.ClsSupportPieceCsv.GetFamilyPath(hojyo3);
            string familyName3 = RevitUtil.ClsRevitUtil.GetFamilyName(path3);

            if (!ClsRevitUtil.LoadFamilyData(doc, path2, out Family fam2))
            {
                return false;
            }
            FamilySymbol sym2 = (ClsRevitUtil.GetFamilySymbol(doc, familyName2, "切梁火打"));

            if (!ClsRevitUtil.LoadFamilyData(doc, path3, out Family fam3))
            {
                return false;
            }
            FamilySymbol sym3 = (ClsRevitUtil.GetFamilySymbol(doc, familyName3, "切梁火打"));

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
                        ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", size + diff);
                    else if (dan == "下段")
                        ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", -size - diff);
                    else
                        ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", 0.0);
                    if (dan != "同段" && m_CreateHoho == CreateHoho.Double)
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
                    if (dan != "同段" && m_CreateHoho == CreateHoho.Double)
                        ClsKariKouzai.SetBaseIdDouble(doc, CreatedID, baseID, dan);
                    ClsKariKouzai.SetBaseId(doc, CreatedID, baseID);
                    ClsKariKouzai.SetKariKouzaiFlag(doc, CreatedID);
                }
                tx.Commit();
            }
            return true;
        }

        /// <summary>
        /// 切梁火打ベース のみを単独選択
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="id">選択した 切梁火打ベース のID</param>
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
        /// 図面上の切梁火打ベースを全て取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<ElementId> GetAllKiribariHiuchiBaseList(Document doc)
        {
            //図面上の切梁火打ベースを全て取得
            List<ElementId> htIdList = ClsRevitUtil.GetSelectCreatedFamilyInstanceList(doc, baseName);
            return htIdList;
        }

        /// <summary>
        ///  図面上の切梁火打ベースを全てクラスで取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<ClsKiribariHiuchiBase> GetAllClsKiribariHiuchiBaseList(Document doc)
        {
            List<ClsKiribariHiuchiBase> lstBase = new List<ClsKiribariHiuchiBase>();

            List<ElementId> lstId = GetAllKiribariHiuchiBaseList(doc);
            foreach (ElementId id in lstId)
            {
                ClsKiribariHiuchiBase clsKH = new ClsKiribariHiuchiBase();
                clsKH.SetParameter(doc, id);
                lstBase.Add(clsKH);
            }

            return lstBase;
        }

        /// <summary>
        /// 指定したIDから切切梁火打ベースクラスを取得する
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
            XYZ tmpStPoint = lCurve.Curve.GetEndPoint(0);
            XYZ tmpEdPoint = lCurve.Curve.GetEndPoint(1);

            m_Floor = shinInstLevel.Host.Name;
            m_ElementId = id;
            m_ShoriType = GetShoriType(doc, id);
            m_Dan = ClsRevitUtil.GetParameter(doc, id, "段");
            m_CreateHoho = ClsRevitUtil.GetParameter(doc, id, "構成") == "シングル" ? ClsKiribariHiuchiBase.CreateHoho.Single : ClsKiribariHiuchiBase.CreateHoho.Double;
            m_SteelType = ClsRevitUtil.GetParameter(doc, id, "鋼材タイプ");

            if (m_CreateHoho == ClsKiribariHiuchiBase.CreateHoho.Double)
            {
                m_SteelSizeDoubleUpper = ClsRevitUtil.GetParameter(doc, id, "鋼材サイズ(ダブル上)");
                m_SteelSizeDoubleUnder = ClsRevitUtil.GetParameter(doc, id, "鋼材サイズ(ダブル下)");
                m_HiuchiLengthDoubleUpperL1 = (int)ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetParameterDouble(doc, id, "火打長さ(ダブル上)L1"));
                m_HiuchiLengthDoubleUnderL2 = (int)ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetParameterDouble(doc, id, "火打長さ(ダブル下)L2"));
                m_HiuchiZureRyo = (int)ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetParameterDouble(doc, id, "上下火打ずれ量a"));
            }
            else
            {
                m_SteelSizeSingle = ClsRevitUtil.GetParameter(doc, id, "鋼材サイズ(シングル)");
                m_HiuchiLengthSingleL = (int)ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetParameterDouble(doc, id, "火打長さ(シングル)L"));
            }
            m_PartsTypeKiriSide = ClsRevitUtil.GetParameter(doc, id, "部品タイプ(切梁側)");
            m_PartsSizeKiriSide = ClsRevitUtil.GetParameter(doc, id, "部品サイズ(切梁側)");
            m_PartsTypeHaraSide = ClsRevitUtil.GetParameter(doc, id, "部品タイプ(腹起側)");
            m_PartsSizeHaraSide = ClsRevitUtil.GetParameter(doc, id, "部品サイズ(腹起側)");
            m_Angle = ClsRevitUtil.GetParameterDouble(doc, id, "角度") / Math.PI * 180;

            return;
        }
        /// <summary>
        /// 対象のファミリに配置順をカスタムデータとして持たせる
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id">ファミリ</param>
        /// <param name="order">0or1or-1(単独で作成されたもの)</param>
        /// <returns></returns>
        public static bool SetShoriType(Document doc, ElementId id, ShoriType shoriType)
        {
            int type = ((int)shoriType);
            return ClsRevitUtil.CustomDataSet(doc, id, "処理タイプ", type);
        }
        /// <summary>
        /// 対象のファミリから配置順のカスタムデータを取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id">ファミリ</param>
        /// <returns>0or1or-1(単独で作成されたもの)</returns>
        public static ShoriType GetShoriType(Document doc, ElementId id)
        {
            ClsRevitUtil.CustomDataGet(doc, id, "処理タイプ", out int type);
            ShoriType shoriType = (ShoriType)type;
            return shoriType;
        }

        /// <summary>
        /// Curveの長さを無限に伸ばす
        /// </summary>
        /// <param name="Curve">ベース</param>
        /// <returns></returns>
        public static Curve CreateInfiniteCurve(Curve cvBase)
        {
            XYZ tmpStPoint = cvBase.GetEndPoint(0);
            XYZ tmpEdPoint = cvBase.GetEndPoint(1);
            XYZ dir = (cvBase as Line).Direction;
            double dist = ClsRevitUtil.CovertToAPI(int.MaxValue);
            Curve cv = Line.CreateBound(new XYZ(tmpStPoint.X - (dist * dir.X), tmpStPoint.Y - (dist * dir.Y), tmpStPoint.Z),
                                                  new XYZ(tmpEdPoint.X + (dist * dir.X), tmpEdPoint.Y + (dist * dir.Y), tmpEdPoint.Z));
            tmpStPoint = cv.GetEndPoint(0);
            tmpEdPoint = cv.GetEndPoint(1);
            return cv;
        }
        #endregion
    }
}
