using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YMS.Parts;

namespace YMS
{
    public class ClsSanjikuPeace
    {
        public static ElementId m_idSanjiku = null;
        public static XYZ m_insec = null;
        public static double m_dAngle = 0.0;

        public static void Reset()
        {
            m_idSanjiku = null;
            m_insec = null;
            m_dAngle = 0.0;
        }

        public static bool CreateSanjikuPeace(UIDocument uidoc)
        {
            Document doc = uidoc.Document;
            ElementId idHara = null;
            if (!ClsRevitUtil.PickObject(uidoc, "腹起ベース", "腹起ベース", ref idHara))
            {
                return false;
            }
            ElementId idKiri = null;
            if (!ClsRevitUtil.PickObject(uidoc, "切梁ベース", "切梁ベース", ref idKiri))
            {
                return false;
            }
            CreateSanjikuPeace(doc, idHara, idKiri);
            return true;
        }
        public static bool CreateSanjikuPeace(Document doc, ElementId idHara, ElementId idKiri)
        {
            string symbolFolpath = ClsZumenInfo.GetYMSFolder();

            ClsKiribariBase clsKiri = new ClsKiribariBase();
            clsKiri.SetParameter(doc, idKiri);
            if (clsKiri.m_kouzaiSize == "20HA" || clsKiri.m_kouzaiSize == "25HA")
            {
                MessageBox.Show("切梁の鋼材サイズでは三軸ピースは作成出来ません。");
                return false;
            }
            string shinfamily = Master.ClsSanjikuPieceCSV.GetFamilyPath(clsKiri.m_kouzaiSize);
            string shinfamilyName = RevitUtil.ClsRevitUtil.GetFamilyName(shinfamily);

            //シンボル読込
            if (!ClsRevitUtil.LoadFamilySymbolData(doc, shinfamily, shinfamilyName, out FamilySymbol sym))
            {
                return false;
            }

            FamilyInstance kiriShinInstLevel = doc.GetElement(idKiri) as FamilyInstance;
            LocationCurve lCurve = kiriShinInstLevel.Location as LocationCurve;
            if (lCurve == null)
            {
                return false;
            }
            Curve cvKiri = lCurve.Curve;

            ElementId levelID = kiriShinInstLevel.Host.Id;

            FamilyInstance haraokoshiShinInst = doc.GetElement(idHara) as FamilyInstance;
            LocationCurve lCurveHara = haraokoshiShinInst.Location as LocationCurve;
            if (lCurveHara != null)
            {
                Curve cvHara = lCurveHara.Curve;
                XYZ insec = GetIntersection(cvKiri, cvHara);
                if (insec != null)
                {
                    using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                    {
                        t.Start();
                        ElementId CreatedID = ClsRevitUtil.Create(doc, insec, levelID, sym);

                        //三軸ピース
                        FamilyInstance sanjikuInst = doc.GetElement(CreatedID) as FamilyInstance;
                        //XYZ sanjikuPoint = (sanjikuInst.Location as LocationPoint).Point;
                        XYZ dirV = sanjikuInst.FacingOrientation;//ファミリインスタンスの向き
                        XYZ dirB = sanjikuInst.HandOrientation;//ファミリインスタンスの側面

                        XYZ directionKiri = Line.CreateBound(cvKiri.GetEndPoint(0), cvKiri.GetEndPoint(1)).Direction;
                        XYZ directionHara = Line.CreateBound(cvHara.GetEndPoint(0), cvHara.GetEndPoint(1)).Direction;

                        double dBaseAngle = dirB.AngleTo(directionHara);
                        double dVectAngle = dirV.AngleTo(directionKiri);
                        double dAngle = -dVectAngle;//時計回りになるように-をかける

                        //切梁ベースの終点に配置する場合、始点と条件を合わせるためにπを足す
                        if (ClsGeo.GEO_EQ(cvKiri.GetEndPoint(1), insec))
                        {
                            dAngle = Math.PI + dAngle;
                        }

                        Line axis = Line.CreateBound(insec, insec + XYZ.BasisZ);
                        ClsRevitUtil.RotateElement(doc, CreatedID, axis, dAngle);
                        string dan = ClsRevitUtil.GetParameter(doc, idKiri, "段");
                        string size = ClsRevitUtil.GetParameter(doc, idKiri, "鋼材サイズ");
                        double kouzaiSize = ClsRevitUtil.CovertToAPI(Master.ClsYamadomeCsv.GetWidth(size) / 2);
                        ClsYMSUtil.GetDifferenceWithAllBase(doc, idKiri, out double diff, out double diff2);
                        //選択された芯の段によって調整
                        //集計レベルの場合基準レベルを指定
                        if (dan == "上段")
                        {
                            ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", kouzaiSize + diff);
                        }
                        else if (dan == "下段")
                        {
                            ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", -kouzaiSize - diff);
                        }
                        else//同段
                        {
                            ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", 0.0);
                        }
                        m_idSanjiku = CreatedID;
                        ClsKariKouzai.SetBaseId(doc, CreatedID, idKiri);
                        t.Commit();
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 三軸ピース作成
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="idKiri">対象の切梁</param>
        /// <param name="point">配置点</param>
        /// <param name="dir">配置向き</param>
        /// <returns></returns>
        public static bool CreateSanjikuPeace(Document doc, ElementId idKiri, XYZ point, XYZ dir)
        {
            ClsKiribariBase clsKiri = new ClsKiribariBase();
            clsKiri.SetParameter(doc, idKiri);
            if (clsKiri.m_kouzaiSize == "20HA" || clsKiri.m_kouzaiSize == "25HA")
            {
                MessageBox.Show("切梁の鋼材サイズでは三軸ピースは作成出来ません。");
                return false;
            }
            string shinfamily = Master.ClsSanjikuPieceCSV.GetFamilyPath(clsKiri.m_kouzaiSize);
            string shinfamilyName = RevitUtil.ClsRevitUtil.GetFamilyName(shinfamily);

            //シンボル読込
            if (!ClsRevitUtil.LoadFamilySymbolData(doc, shinfamily, shinfamilyName, out FamilySymbol sym))
            {
                return false;
            }

            FamilyInstance kiriShinInstLevel = doc.GetElement(idKiri) as FamilyInstance;
            ElementId levelID = kiriShinInstLevel.Host.Id;

            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                t.Start();
                FailureHandlingOptions failOpt = t.GetFailureHandlingOptions();
                failOpt.SetFailuresPreprocessor(new WarningSwallower());
                t.SetFailureHandlingOptions(failOpt);
                ElementId CreatedID = ClsRevitUtil.Create(doc, point, levelID, sym);

                //三軸ピース
                FamilyInstance sanjikuInst = doc.GetElement(CreatedID) as FamilyInstance;
                double dAngle = -sanjikuInst.FacingOrientation.AngleOnPlaneTo(dir, XYZ.BasisZ);

                Line axis = Line.CreateBound(point, point + XYZ.BasisZ);
                ClsRevitUtil.RotateElement(doc, CreatedID, axis, dAngle);
                string dan = ClsRevitUtil.GetParameter(doc, idKiri, "段");
                string size = ClsRevitUtil.GetParameter(doc, idKiri, "鋼材サイズ");
                double kouzaiSize = ClsRevitUtil.CovertToAPI(Master.ClsYamadomeCsv.GetWidth(size) / 2);
                ClsYMSUtil.GetDifferenceWithAllBase(doc, idKiri, out double diff, out double diff2);
                //選択された芯の段によって調整
                //集計レベルの場合基準レベルを指定
                if (dan == "上段")
                {
                    ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", kouzaiSize + diff);
                }
                else if (dan == "下段")
                {
                    ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", -kouzaiSize - diff);
                }
                else//同段
                {
                    ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", 0.0);
                }
                m_idSanjiku = CreatedID;
                ClsKariKouzai.SetBaseId(doc, CreatedID, idKiri);
                t.Commit();
            }
            return true;
        }
        public static bool SetSanjikuPeace(UIDocument uidoc)
        {
            m_idSanjiku = null;
            if (!ClsRevitUtil.PickObjectFilters(uidoc, "三軸ピースを選択してください", ClsGlobal.m_sanjikuPeaceList, ref m_idSanjiku))
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 三軸ピースを+-指定値で動かせる
        /// </summary>
        /// <param name="uiapp"></param>
        /// <returns></returns>
        public static bool MoveConstSanjikuPeace(UIDocument uidoc)
        {
            Document doc = uidoc.Document;


            try
            {
                if (m_idSanjiku == null)
                {
                    return false;
                }
                FamilyInstance sanjikuInst = doc.GetElement(m_idSanjiku) as FamilyInstance;
                XYZ sanjikuPoint = (sanjikuInst.Location as LocationPoint).Point;
                XYZ dir = sanjikuInst.FacingOrientation;//ファミリインスタンスの向き
                XYZ dirB = sanjikuInst.HandOrientation;//ファミリインスタンスの側面

                double dist = ClsRevitUtil.CovertToAPI(DLG.DlgCreateSanjiku.m_constDist);
                XYZ moveVector = new XYZ(dir.X * dist,
                                         dir.Y * dist,
                                         dir.Z);
                using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                {
                    t.Start();
                    sanjikuInst.Location.Move(moveVector);
                    t.Commit();
                }
            }
            catch (OperationCanceledException e)
            {
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// 三軸ピースの支点間距離を求める
        /// </summary>
        /// <param name="uiapp"></param>
        /// <returns></returns>
        public static bool MovedSanjikuPeace(UIDocument uidoc)
        {
            Document doc = uidoc.Document;

            try
            {
                if (m_idSanjiku == null)
                {
                    return false;
                }
                FamilyInstance sanjikuInst = doc.GetElement(m_idSanjiku) as FamilyInstance;
                XYZ sanjikuPoint = (sanjikuInst.Location as LocationPoint).Point;

                double kouzaiSize = ClsRevitUtil.GetParameterDouble(doc, m_idSanjiku, "基準レベルからの高さ");

                if (m_insec == null)
                {
                    XYZ dicA = sanjikuInst.FacingOrientation;//ファミリインスタンスの向き
                    XYZ dicB = sanjikuInst.HandOrientation;//ファミリインスタンスの0度

                    //三軸ピースが配置されている切梁を見つける
                    Curve cvBaseKiri = null;
                    XYZ dirKiri = null;

                    //図面上の切梁ベースを全て取得
                    List<ElementId> baseIdList = ClsKiribariBase.GetAllKiribariBaseList(doc);
                    //三軸ピースと交点のある切梁ベースを探す
                    foreach (ElementId baseId in baseIdList)
                    {
                        FamilyInstance shinInst = doc.GetElement(baseId) as FamilyInstance;
                        LocationCurve lCurveKiri = shinInst.Location as LocationCurve;
                        if (lCurveKiri != null)
                        {
                            XYZ tmpStPoint = lCurveKiri.Curve.GetEndPoint(0);
                            XYZ tmpEdPoint = lCurveKiri.Curve.GetEndPoint(1);
                            tmpStPoint = new XYZ(tmpStPoint.X, tmpStPoint.Y, tmpStPoint.Z + kouzaiSize);
                            tmpEdPoint = new XYZ(tmpEdPoint.X, tmpEdPoint.Y, tmpEdPoint.Z + kouzaiSize);

                            cvBaseKiri = Line.CreateBound(tmpStPoint, tmpEdPoint);
                            if (IsPointOnCurve(cvBaseKiri, sanjikuPoint))
                            {
                                dirKiri = Line.CreateBound(tmpStPoint, tmpEdPoint).Direction;
                                break;
                            }
                        }
                    }
                    if (dirKiri == null) return false;//切梁上に無いため返却

                    //三軸ピースが乗っている切梁ベースと交差する腹起ベースを探す
                    //腹起ベースは内外判定のために使用
                    XYZ dirHara = null;
                    //図面上の腹起ベースを全て取得
                    baseIdList = ClsHaraokoshiBase.GetAllHaraokoshiBaseList(doc);
                    //三軸ピースと交点のある腹起ベースを探す
                    foreach (ElementId baseId in baseIdList)
                    {
                        //string dan = ClsRevitUtil.GetParameter(doc, baseId, "段");
                        FamilyInstance shinInst = doc.GetElement(baseId) as FamilyInstance;
                        LocationCurve lCurve = shinInst.Location as LocationCurve;
                        if (lCurve != null)
                        {
                            XYZ tmpStPoint = lCurve.Curve.GetEndPoint(0);
                            XYZ tmpEdPoint = lCurve.Curve.GetEndPoint(1);
                            tmpStPoint = new XYZ(tmpStPoint.X, tmpStPoint.Y, tmpStPoint.Z + kouzaiSize);
                            tmpEdPoint = new XYZ(tmpEdPoint.X, tmpEdPoint.Y, tmpEdPoint.Z + kouzaiSize);

                            Curve cvBase = Line.CreateBound(tmpStPoint, tmpEdPoint);
                            m_insec = GetIntersection(cvBaseKiri, cvBase);//交点が2つ見つかる
                            if (m_insec != null)
                            {
                                dirHara = Line.CreateBound(tmpStPoint, tmpEdPoint).Direction;
                                XYZ dir = dirKiri;
                                //切梁ベースの終点との交点の場合、始点とは反対に出来ているので条件を合わせるために向きを反対にする
                                if (ClsGeo.GEO_EQ(cvBaseKiri.GetEndPoint(1), m_insec))
                                {
                                    dir = -dir;
                                }
                                if (ClsGeo.GEO_EQ(dir, dicA))
                                {
                                    m_dAngle = dicA.AngleTo(dirHara);
                                    break;
                                }
                            }
                        }
                    }
                    if (dirHara == null) return false;//切梁と接する腹起ベースが無いため返却
                }
                if (m_insec == null) return false;

                //ダイアログ表示前の初期値を格納する必要がある
                double d = m_insec.DistanceTo(sanjikuPoint);
                d = ClsRevitUtil.CovertFromAPI(d);
                d = ClsGeo.FloorAtDigitAdjust(1, d);//誤差があるため調整
                DLG.DlgCreateSanjiku.m_sanjikuDist = d;

                //切梁 + 腹起交点と、三軸ピースの斜め部分(30度) + 腹起との交点間の距離
                double crossDist = d + GetSanjikuDistanceToCenter(doc, m_idSanjiku);//三軸ピースの基点から複数部材がクロスしている部分までの長さが1500で50は1350
                                            //角度a,b,cの対面にある辺をA、B、Cとする三角形
                                            //A/sin(a) = B/sin(b) = C/sin(c)
                double shitenkankyoriR = crossDist / Math.Sin(Math.PI - Math.PI / 6 - m_dAngle) * Math.Sin(Math.PI / 6);//右
                double shitenkankyoriL = crossDist / Math.Sin(Math.PI - Math.PI / 6 - (Math.PI - m_dAngle)) * Math.Sin(Math.PI / 6);//左
                shitenkankyoriR = ClsGeo.FloorAtDigitAdjust(3, shitenkankyoriR);//誤差があるため調整
                shitenkankyoriL = ClsGeo.FloorAtDigitAdjust(3, shitenkankyoriL);//誤差があるため調整
                DLG.DlgCreateSanjiku.m_distR = shitenkankyoriR;
                DLG.DlgCreateSanjiku.m_distL = shitenkankyoriL;

                //ダイアログに支点間距離を表示する
                DLG.DlgCreateSanjiku m_FormCreateSanjiku = Application.thisApp.GetForm_dlgCreateSanjiku();
                if (m_FormCreateSanjiku != null && m_FormCreateSanjiku.Visible)
                {
                    m_FormCreateSanjiku.SetShitenkanKyori(DLG.DlgCreateSanjiku.m_distR, DLG.DlgCreateSanjiku.m_distL);
                    m_FormCreateSanjiku.SetSanjikuDist(DLG.DlgCreateSanjiku.m_sanjikuDist);
                }
            }
            catch (OperationCanceledException e)
            {
            }
            catch (Exception e)
            {
            }

            return true;
        }
        /// <summary>
        /// 図面上の三軸を全て取得
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<ElementId> GetAllSanjikuPeaceList(Document doc)
        {
            List<ElementId> sanjikuIdList = ClsRevitUtil.GetSelectCreatedFamilyInstanceList(doc, "三軸", true);
            return sanjikuIdList;
        }

        public static bool ChangeSanjikuMode(Document doc, bool mode)
        {
            int show = 0;
            if (!mode)
                show = 1;
            List<ElementId> sanjikuIdList = GetAllSanjikuPeaceList(doc);
            foreach(ElementId id in sanjikuIdList)
            {
                FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                FamilySymbol sym = inst.Symbol;
                using (Transaction t = new Transaction(doc, "三軸表示"))
                {
                    t.Start();
                    FailureHandlingOptions failOpt = t.GetFailureHandlingOptions();
                    failOpt.SetFailuresPreprocessor(new WarningSwallower());
                    t.SetFailureHandlingOptions(failOpt);
                    try
                    {
                        ClsRevitUtil.SetTypeParameter(sym, "単線表示", show);
                    }
                    catch
                    { t.RollBack(); }

                    t.Commit();
                }
            }
            return true;
        }
        public static double GetSanjikuDistanceToCenter(Document doc, ElementId sanjikuId)
        {
            double distance = 1500.0;
            FamilyInstance inst = doc.GetElement(sanjikuId) as FamilyInstance;
            if (inst.Name.Contains("50"))
                distance = 1350.0;
            return distance;
        }
        #region メソッド
        /// <summary>
        /// ベクトルの象限を取得する
        /// </summary>
        /// <param name="direction">ベクトル</param>
        /// <returns>0:原点,1:第一象限,2:第二象限,3:第三象限,4:第四象限</returns>
        public static int GetMatrix4Quadrants(XYZ direction)
        {
            //第一象限
            //if (direction.X > 0.0 && direction.Y >= 0.0)//(1,0,0)から右上向き
            if (ClsGeo.GEO_GT0(direction.X) && ClsGeo.GEO_GE0(direction.Y))
            {
                return 1;
            }
            //第二象限
            //if (direction.X <= 0.0 && direction.Y > 0.0)//(0,1,0)から左上向き
            if (ClsGeo.GEO_LE0(direction.X) && ClsGeo.GEO_GT0(direction.Y))
            {
                return 2;
            }
            //第三象限
            //if (direction.X < 0.0 && direction.Y <= 0.0)//(-1,0,0)から左下向き
            if (ClsGeo.GEO_LT0(direction.X) && ClsGeo.GEO_LE0(direction.Y))
            {
                return 2;
            }
            //第四象限
            //if (direction.X >= 0.0 && direction.Y < 0.0)//(0,-1,0)から右下向き
            if (ClsGeo.GEO_GE0(direction.X) && ClsGeo.GEO_LT0(direction.Y))
            {
                return 1;
            }
            return 0;
        }
        
        /// <summary>
        /// 線の交点を取得
        /// </summary>
        /// <param name="cv1">線1</param>
        /// <param name="cv2">線2</param>
        /// <returns>交点</returns>
        public static XYZ GetIntersection(Curve cv1, Curve cv2)
        {
            XYZ intersectionPoint = null;
            IntersectionResultArray results;
            SetComparisonResult result = cv1.Intersect(cv2, out results);
            if (result != SetComparisonResult.Overlap)
            {
                return intersectionPoint;
            }

            if (results == null || results.Size != 1)
            {
                return intersectionPoint;
            }

            IntersectionResult iResult = results.get_Item(0);
            intersectionPoint = iResult.XYZPoint;
            return intersectionPoint;
        }
        /// <summary>
        /// 指定の点がCurveが通るか判定する
        /// </summary>
        /// <param name="curve">線分</param>
        /// <param name="point">指定点</param>
        /// <returns></returns>
        public static bool IsPointOnCurve(Curve curve, XYZ point)
        {
            double dist = curve.Distance(point);
            // パラメータがパラメータ範囲内にあるかどうかを判定
            if (ClsGeo.GEO_EQ(dist, 0.0))
            {
                return true; // 点は曲線上に存在します
            }
            else
            {
                return false; // 点は曲線上に存在しません
            }
        }
        #endregion
    }//ClsSanjikuPeace
}
