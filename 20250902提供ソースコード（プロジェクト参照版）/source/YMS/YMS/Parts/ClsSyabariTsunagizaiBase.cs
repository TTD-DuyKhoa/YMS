using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMS.Parts
{
    public class ClsSyabariTsunagizaiBase
    {
        #region 定数
        public const string baseName = "斜梁繋ぎ材ベース";
        public const string NAME = "斜梁繋ぎ材";
        #endregion

        #region Enum
        /// <summary>
        /// 取り付け方法
        /// </summary>
        public enum ToritsukeHoho
        {
            Bolt,
            Buruman,
            Rikiman
        }

        /// <summary>
        /// 処理方法
        /// </summary>
        public enum ShoriType
        {
            Replace,
            Manual
        }
        #endregion

        #region プロパティ
        /// <summary>
        /// 鋼材情報：分類
        /// </summary>
        public string m_SteelType { get; set; }


        /// <summary>
        ///鋼材情報：サイズ
        /// </summary>
        public string m_SteelSize { get; set; }

        /// <summary>
        /// 分割フラグ
        /// </summary>
        public bool m_SplitFlg { get; set; }

        /// <summary>
        /// 取り付け方法
        /// </summary>
        public ToritsukeHoho m_ToritsukeHoho { get; set; }

        /// <summary>
        /// 処理方法
        /// </summary>
        public ShoriType m_ShoriType { get; set; }

        /// <summary>
        /// ボルト種類１
        /// </summary>
        public string m_BoltType1 { get; set; }

        /// <summary>
        /// ボルト種類２
        /// </summary>
        public string m_BoltType2 { get; set; }

        /// <summary>
        /// ボルト本数
        /// </summary>
        public int m_BoltNum { get; set; }

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
        public ClsSyabariTsunagizaiBase()
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
            m_SteelType = string.Empty;
            m_SteelSize = string.Empty;
            m_ShoriType = ShoriType.Replace;
            m_SplitFlg = false;
            m_ToritsukeHoho = ToritsukeHoho.Bolt;
            m_ShoriType = ShoriType.Manual;
            m_BoltType1 = string.Empty;
            m_BoltType2 = string.Empty;
            m_BoltNum = 0;
            m_Floor = string.Empty;
            m_Dan = string.Empty;
            m_FlgEdit = false;
            m_ElementId = null;
        }
        public bool CreateSyabariTsunagizaiBase(Document doc, List<ElementId> modelLineIdList, ElementId levelID)
        {
            string symbolFolpath = ClsZumenInfo.GetYMSFolder();
            string shinfamily = System.IO.Path.Combine(symbolFolpath, "ベース関係\\" + baseName + ".rfa");
            //シンボル読込
            if (!ClsRevitUtil.LoadFamilySymbolData(doc, shinfamily, baseName, out FamilySymbol sym))
            {
                return false;
            }

            foreach (ElementId modelLineId in modelLineIdList)
            {
                ModelLine modelLine = doc.GetElement(modelLineId) as ModelLine;
                LocationCurve lCurve = modelLine.Location as LocationCurve;
                if (lCurve == null)
                {
                    continue;
                }
                Curve cv = lCurve.Curve;
                var tmpStPoint = cv.GetEndPoint(0);
                var tmpEdPoint = cv.GetEndPoint(1);
                m_Dan = "上段";

                using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                {
                    t.Start();
                    //CreateSyabariTsunagizaiBase(doc, tmpStPoint, tmpEdPoint, levelID);
                    t.Commit();
                }
            }
            return true;
        }
        public bool CreateSyabariTsunagizaiBase(Document doc, XYZ tmpStPoint, XYZ tmpEdPoint, XYZ tmpThirdPoint, ElementId levelId)
        {
            //参照面の作成
            ReferencePlane slopePlane = null;
            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                t.Start();
                FailureHandlingOptions failOpt = t.GetFailureHandlingOptions();
                failOpt.SetFailuresPreprocessor(new WarningSwallower());
                t.SetFailureHandlingOptions(failOpt);
                //tmpEdPoint = new XYZ(tmpEdPoint.X, tmpEdPoint.Y, tmpEdPoint.Z - ClsRevitUtil.CovertToAPI(0));
                //slopePlane = ClsRevitUtil.CreateReferencePlaneW(doc, tmpStPoint, tmpEdPoint, ClsRevitUtil.CovertToAPI(0));
                slopePlane = ClsRevitUtil.CreateReferencePlane(doc, tmpStPoint, tmpEdPoint, tmpThirdPoint);
                t.Commit();
            }

            //斜張の読込
            string symbolFolpath = ClsZumenInfo.GetYMSFolder();
            string shinfamily = System.IO.Path.Combine(symbolFolpath, "ベース関係\\" + baseName + ".rfa");
            if (!ClsRevitUtil.LoadFamilySymbolData(doc, shinfamily, baseName, out FamilySymbol sym))
            {
                return false;
            }

            //作成した参照面にベースを配置
            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                t.Start();
                FailureHandlingOptions failOpt = t.GetFailureHandlingOptions();
                failOpt.SetFailuresPreprocessor(new WarningSwallower());
                t.SetFailureHandlingOptions(failOpt);
                if (sym == null)
                {
                    return false;
                }

                if (!sym.IsActive)
                {
                    sym.Activate();
                }

                try
                {
                    var family = sym.Family;
                    //このタイプでないと参照面に配置できない
                    if (family.FamilyPlacementType == FamilyPlacementType.WorkPlaneBased)
                    {
                        var reference = slopePlane.GetReference();
                        var instance = doc.Create.NewFamilyInstance(reference, tmpStPoint, tmpEdPoint - tmpStPoint, sym);
                        var createId = instance.Id;
                        m_ElementId = createId;
                        ClsRevitUtil.SetParameter(doc, createId, "長さ", tmpStPoint.DistanceTo(tmpEdPoint));
                        ClsRevitUtil.SetParameter(doc, createId, "集計レベル", levelId);
                        SetParameter(doc, createId);

                        var id = instance.Id;
                        var element = doc.GetElement(reference);
                        // ここで一回移動などをしないと Location が原点のまま返ってくる。
                        ElementTransformUtils.MoveElement(doc, id, XYZ.Zero);
                        slopePlane.Name = baseName + id.IntegerValue.ToString();

                        //#31575
                        ClsRevitUtil.SetMojiParameter(doc, createId, "取付方法", GetToritsukeHoho(m_ToritsukeHoho));
                        if (m_ToritsukeHoho == ToritsukeHoho.Bolt)
                        {
                            ClsRevitUtil.SetMojiParameter(doc, createId, "ボルトタイプ1", m_BoltType1);
                            ClsRevitUtil.SetMojiParameter(doc, createId, "ボルトタイプ2", m_BoltType2);
                            ClsRevitUtil.SetParameter(doc, createId, "ボルト本数", m_BoltNum);

                            //ボルト情報をカスタムデータとして設定する
                            ClsYMSUtil.SetBolt(doc, createId, m_BoltType2, m_BoltNum);
                        }
                        else
                        {
                            ////ボルト情報のカスタムデータを削除する
                            //ClsYMSUtil.DeleteBolt(doc, createId);
                        }
                    }
                    else
                    {
                        //作成出来ない
                    }
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("TEST4", ex.Message);
                }

                t.Commit();
            }
            return true;
        }

        public bool CreateSyabariTsunagizai(Document doc, ElementId id1, ElementId id2, ElementId id3)
        {
            var kouzaiType = m_SteelType;
            var kouzaiSize = m_SteelSize;
            var dSize = 0.0;
            string familyPath;
            
            switch (kouzaiType)
            {
                case "チャンネル":
                    {
                        familyPath = Master.ClsSyabariKouzaiCSV.GetFamilyPath(kouzaiSize);
                        double sunpou2 = ClsCommonUtils.ChangeStrToDbl(Master.ClsYamadomeCsv.GetKouzaiSizeSunpou(kouzaiSize, 1));
                        dSize += ClsRevitUtil.CovertToAPI(sunpou2 / 2);
                        break;
                    }
                case "アングル":
                    {
                        familyPath = Master.ClsAngleCsv.GetFamilyPath(kouzaiSize);
                        double sunpou2 = ClsCommonUtils.ChangeStrToDbl(Master.ClsYamadomeCsv.GetKouzaiSizeSunpou(kouzaiSize, 1));
                        dSize += ClsRevitUtil.CovertToAPI(sunpou2 / 2);
                        break;
                    }
                case "H形鋼 広幅":
                case "H形鋼 中幅":
                case "H形鋼 細幅":
                    {
                        familyPath = Master.ClsSyabariKouzaiCSV.GetFamilyPath(kouzaiSize);
                        double sunpou2 = ClsCommonUtils.ChangeStrToDbl(Master.ClsYamadomeCsv.GetKouzaiSizeSunpou(kouzaiSize, 1));
                        dSize += ClsRevitUtil.CovertToAPI(sunpou2 / 2);
                        break;
                    }
                default:
                    {
                        familyPath = Master.ClsYamadomeCsv.GetKariFamilyPath(kouzaiSize, true);
                        double sunpou2 = Master.ClsYamadomeCsv.GetWidth(kouzaiSize);
                        dSize += ClsRevitUtil.CovertToAPI(sunpou2 / 2);
                        break;
                    }
            }

            string symbolName = ClsRevitUtil.GetFamilyName(familyPath);
            if (!ClsRevitUtil.LoadFamilyData(doc, familyPath, out Family kouFam))
            {
                return false;
            }
            FamilySymbol sym = ClsRevitUtil.GetFamilySymbol(doc, symbolName, NAME);

            if (sym == null)
                return false;

            Plane sectionPlane;
            // pickされた斜梁つなぎベースのモデル線分から鉛直方向の断面平面を作成
            {
                var tsunagiLine = ClsYMSUtil.GetBaseLine(doc, id3);
                var a = tsunagiLine.GetEndPoint(0);
                var b = tsunagiLine.GetEndPoint(1);
                //Z軸成分を0にする
                a = new XYZ(a.X, a.Y, 0);
                b = new XYZ(b.X, b.Y, 0);
                //鉛直方向のベクトルを求める必要があるかもXYZ.BasicZでは鉛直にならないパターンがあるため
                sectionPlane = Plane.CreateByOriginAndBasis(a, (b - a).Normalize(), XYZ.BasisZ);//Z軸が同じでなければいけない
            }
            var inst = doc.GetElement(id3) as FamilyInstance;
            var referencePlane = inst.Host as ReferencePlane;

            var pickResultSet = new ElementId[] { id1, id2 };
            var topPoints = new List<XYZ>();
            var sectionBBoxes = new List<XYZ[]>();
            foreach (var pickResult in pickResultSet)
            {
                var shabariBase = doc.GetElement(pickResult) as FamilyInstance;

                // 斜梁ベースの直線を斜梁ベースファミリの X 軸として計算
                var shabariBaseTrans = shabariBase.GetTotalTransform();
                var shabariBaseLine = Line.CreateUnbound(shabariBaseTrans.Origin, shabariBaseTrans.BasisX);

                // 斜梁ベースの平面は Host から取得
                var shabariBasePlane = (shabariBase.Host as ReferencePlane)?.GetPlane();

                var shabariSize = Master.ClsYamadomeCsv.GetWidth(ClsRevitUtil.GetParameter(doc, pickResult, "鋼材サイズ"));
                var shabariW = shabariSize;
                var shabariH = shabariSize;

                // 斜梁鋼材の断面の端点を算出
                //var points = ClsYMSUtil.CalcShabariSection(shabariBaseLine, shabariBasePlane, shabariW, shabariH, sectionPlane);
                //sectionBBoxes.Add(points);

                // 斜梁断面の端点の中で Z 成分が最大の点を取得
                var sectionTop = ClsYMSUtil.CalcShabariSectionTop(shabariBaseLine, shabariBasePlane, shabariW, shabariH, sectionPlane);
                if (sectionTop != null)
                {
                    topPoints.Add(sectionTop);
                }
                else
                {
                    var dialog = new TaskDialog(baseName)
                    {
                        MainInstruction = "斜梁断面の端点が算出できません",
                        MainContent = "斜梁ベースと斜梁ベースつなぎが平行かもしれません",
                        MainIcon = TaskDialogIcon.TaskDialogIconError,
                    };
                    dialog.Show();
                }
            }

            using (var transaction = new Transaction(doc, Guid.NewGuid().ToString()))
            {
                transaction.Start();

                // 斜梁断面の最高点を結ぶモデル線分を作成
                if (topPoints.Count == 2)
                {
                    var l = Line.CreateBound(topPoints.FirstOrDefault(), topPoints.LastOrDefault());
                    //var p = SketchPlane.Create(doc, sectionPlane);
                    //doc.Create.NewModelCurve(l, p);
                    var tmpStPoint = l.GetEndPoint(0);
                    var tmpEdPoint = l.GetEndPoint(1);
                    //var line1 = ClsYMSUtil.GetBaseLine(doc, id1);
                    //var line2 = ClsYMSUtil.GetBaseLine(doc, id2);
                    var line3 = ClsYMSUtil.GetBaseLine(doc, id3);
                    var top13Line = Line.CreateBound(tmpStPoint, tmpStPoint - ClsRevitUtil.CovertToAPI(10000) * XYZ.BasisZ);//鋼材サイズが500より大きいものが無く交点が見つかればよいため500
                    var insec13 = ClsRevitUtil.GetIntersection(line3, top13Line);
                    var top23Line = Line.CreateBound(tmpEdPoint, tmpEdPoint - ClsRevitUtil.CovertToAPI(10000) * XYZ.BasisZ);
                    var insec23 = ClsRevitUtil.GetIntersection(line3, top23Line);

                    if (insec13 != null && insec23 != null)
                    {
                        var top13Z = tmpStPoint.Z - insec13.Z;
                        var top23Z = tmpEdPoint.Z - insec23.Z;

                        //ベースの始端終端は選択した斜梁の順番に合わせる
                        if(line3.GetEndPoint(0).DistanceTo(tmpStPoint) < line3.GetEndPoint(1).DistanceTo(tmpStPoint))
                        {
                            tmpStPoint = line3.GetEndPoint(0) + top13Z * XYZ.BasisZ;
                            tmpEdPoint = line3.GetEndPoint(1) + top23Z * XYZ.BasisZ;
                        }
                        else
                        {
                            tmpStPoint = line3.GetEndPoint(1) + top13Z * XYZ.BasisZ;
                            tmpEdPoint = line3.GetEndPoint(0) + top23Z * XYZ.BasisZ;
                        }
                    }

                    ElementId createId = ClsRevitUtil.Create(doc, tmpStPoint, (tmpEdPoint - tmpStPoint).Normalize(), referencePlane, sym);
                    ClsKariKouzai.SetBaseId(doc, createId, id3);
                    ClsKariKouzai.SetKariKouzaiFlag(doc, createId);
                    ClsRevitUtil.SetParameter(doc, createId, "長さ", tmpStPoint.DistanceTo(tmpEdPoint));
                    //オフセットに合わせた足し引きにする参照面の向きによる
                    var offset = ClsRevitUtil.GetParameterDouble(doc, createId, "ホストからのオフセット");
                    if (0 < offset)
                    {
                        dSize += ClsRevitUtil.GetParameterDouble(doc, createId, "ホストからのオフセット");
                    }
                    else
                    {
                        dSize += ClsRevitUtil.GetParameterDouble(doc, createId, "ホストからのオフセット");
                        //dSize = -dSize + ClsRevitUtil.GetParameterDouble(doc, createId, "ホストからのオフセット");
                    }
                    ClsRevitUtil.SetParameter(doc, createId, "ホストからのオフセット", dSize);
                    //ClsRevitUtil.SetParameter(doc, createId, "集計レベル", levelId);
                    ElementId levelId = ClsRevitUtil.GetParameterElementId(doc, id3, "集計レベル");
                    if (levelId != null)
                    {
                        ClsRevitUtil.SetParameter(doc, createId, "集計レベル", levelId);
                    }
                    
                }
                transaction.Commit();
            }
            if (!CreateBruman(doc, id1, id2, id3))
            {
                //ブルマンが作図されない
            }
            return true;
        }

        /// <summary>
        /// 2本の線分の無限延長線が3D空間で交差するかを判定し、交点があれば返す。
        /// </summary>
        /// <param name="lineA">線分A</param>
        /// <param name="lineB">線分B</param>
        /// <param name="intersectionPoint">交点（存在する場合）</param>
        /// <returns>交差する場合 true、しない場合 false</returns>
        public static bool TryGetIntersection(Line lineA, Line lineB, out XYZ intersectionPoint)
        {
            intersectionPoint = null;

            if (lineA == null || lineB == null || !lineA.IsBound || !lineB.IsBound)
                return false;

            XYZ p1 = lineA.GetEndPoint(0);
            XYZ v1 = (lineA.GetEndPoint(1) - p1).Normalize();

            XYZ p2 = lineB.GetEndPoint(0);
            XYZ v2 = (lineB.GetEndPoint(1) - p2).Normalize();

            XYZ cross = v1.CrossProduct(v2);

            if (IsZeroLength(cross))
                return false; // 平行または同一直線

            // 直線同士の最短距離点を求める
            XYZ w0 = p1 - p2;
            double a = v1.DotProduct(v1); // = 1 (Normalize済み)
            double b = v1.DotProduct(v2);
            double c = v2.DotProduct(v2); // = 1 (Normalize済み)
            double d = v1.DotProduct(w0);
            double e = v2.DotProduct(w0);

            double denom = a * c - b * b;
            if (Math.Abs(denom) < 1e-9)
                return false; // 数学的にほぼ平行と判定

            double s = (b * e - c * d) / denom;
            double t = (a * e - b * d) / denom;

            XYZ pointA = p1 + v1 * s;
            XYZ pointB = p2 + v2 * t;

            // 2つの直線が交差していれば、交点は一致する（または誤差内）
            if ((pointA - pointB).GetLength() < 1e-6)
            {
                intersectionPoint = pointA;
                return true;
            }

            return false; // 交点なし（ねじれ線）
        }

        /// <summary>
        /// ベクトルがゼロ長かどうかをチェック（許容誤差あり）
        /// </summary>
        private static bool IsZeroLength(XYZ vector, double tolerance = 1e-9)
        {
            return vector.GetLength() < tolerance;
        }

        public static void RevercePick(Document doc, ref ElementId id1, ref ElementId id2)
        {
            try
            {
                ElementId temp = null;
                var cv = ClsYMSUtil.GetBaseLine(doc, id1);
                var pId1s = cv.GetEndPoint(0);
                var pId1e = cv.GetEndPoint(1);
                var cv2 = ClsYMSUtil.GetBaseLine(doc, id2);

                if (ClsGeo.IsLeft((pId1e - pId1s).Normalize(), (pId1e - cv2.GetEndPoint(0)).Normalize()))
                {
                    temp = id1;
                    id1 = id2;
                    id2 = temp;
                }
            }
            catch
            {

            }
        }

        public static void ReverceTsunagiBaseVec(Document doc, ElementId id1, ref XYZ pStart, ref XYZ pEnd)
        {
            try
            {
                XYZ temp = null;
                var cv1 = ClsYMSUtil.GetBaseLine(doc, id1);
                var pId1s = cv1.GetEndPoint(0);
                var pId1e = cv1.GetEndPoint(1);

                XYZ pHigh = (pId1s.Z < pId1e.Z ? pId1e : pId1s);

                if (!ClsGeo.IsLeft((pEnd - pStart).Normalize(), (pHigh - pStart).Normalize()))
                {
                    temp = pStart;
                    pStart = pEnd;
                    pEnd = temp;
                }

                //XYZ direction = (pStart - pEnd).Normalize();
                //if (direction.Z < 0)
                //{
                //    temp = pStart;
                //    pStart = pEnd;
                //    pEnd = temp;
                //}
            }
            catch
            {

            }
        }

        public static void EnsureReferencePlaneRightHanded(
    ref XYZ bubbleEnd,
    ref XYZ freeEnd,
    ref XYZ thirdPoint)
        {
            XYZ xDir = (freeEnd - bubbleEnd).Normalize();
            XYZ yTemp = (thirdPoint - bubbleEnd).Normalize();
            XYZ zDir = xDir.CrossProduct(yTemp).Normalize();

            // Revit のファミリは、Z軸が「上」、Yが「正面」を仮定している
            // → Zが下向きにならないよう補正
            if (zDir.Z < 0)
            {
                zDir = -zDir;
            }

            // 正しい右手系の Y軸を再計算（Z × X）
            XYZ yDir = zDir.CrossProduct(xDir).Normalize();

            // ThirdPoint を BubbleEnd からの Y方向に補正
            thirdPoint = bubbleEnd + yDir;
        }

        public static (XYZ, XYZ) GetSyabariTsunagizaiPoint(Document doc, ElementId id1, ElementId id2, ElementId id3)
        {
            var line3 = ClsYMSUtil.GetBaseLine(doc, id3);
            var tmpStPoint = line3.GetEndPoint(0);
            var tmpEdPoint = line3.GetEndPoint(1);

            Plane sectionPlane;
            // pickされた斜梁つなぎベースのモデル線分から鉛直方向の断面平面を作成
            {
                var tsunagiLine = ClsYMSUtil.GetBaseLine(doc, id3);
                var a = tsunagiLine.GetEndPoint(0);
                var b = tsunagiLine.GetEndPoint(1);
                //Z軸成分を0にする
                a = new XYZ(a.X, a.Y, 0);
                b = new XYZ(b.X, b.Y, 0);
                //鉛直方向のベクトルを求める必要があるかもXYZ.BasicZでは鉛直にならないパターンがあるため
                sectionPlane = Plane.CreateByOriginAndBasis(a, (b - a).Normalize(), XYZ.BasisZ);//Z軸が同じでなければいけない
            }
            var inst = doc.GetElement(id3) as FamilyInstance;
            var referencePlane = inst.Host as ReferencePlane;

            var pickResultSet = new ElementId[] { id1, id2 };
            var topPoints = new List<XYZ>();
            foreach (var pickResult in pickResultSet)
            {
                var shabariBase = doc.GetElement(pickResult) as FamilyInstance;

                // 斜梁ベースの直線を斜梁ベースファミリの X 軸として計算
                var shabariBaseTrans = shabariBase.GetTotalTransform();
                var shabariBaseLine = Line.CreateUnbound(shabariBaseTrans.Origin, shabariBaseTrans.BasisX);

                // 斜梁ベースの平面は Host から取得
                var shabariBasePlane = (shabariBase.Host as ReferencePlane)?.GetPlane();

                var shabariSize = Master.ClsYamadomeCsv.GetWidth(ClsRevitUtil.GetParameter(doc, pickResult, "鋼材サイズ"));
                var shabariW = shabariSize;
                var shabariH = shabariSize;

                // 斜梁断面の端点の中で Z 成分が最大の点を取得
                var sectionTop = ClsYMSUtil.CalcShabariSectionTop(shabariBaseLine, shabariBasePlane, shabariW, shabariH, sectionPlane);
                if (sectionTop != null)
                {
                    topPoints.Add(sectionTop);
                }
                else
                {
                    var dialog = new TaskDialog(baseName)
                    {
                        MainInstruction = "斜梁断面の端点が算出できません",
                        MainContent = "斜梁ベースと斜梁ベースつなぎが平行かもしれません",
                        MainIcon = TaskDialogIcon.TaskDialogIconError,
                    };
                    //dialog.Show();
                }
            }

            // 斜梁断面の最高点を結ぶモデル線分を作成
            if (topPoints.Count == 2)
            {
                var l = Line.CreateBound(topPoints.FirstOrDefault(), topPoints.LastOrDefault());
                tmpStPoint = l.GetEndPoint(0);
                tmpEdPoint = l.GetEndPoint(1);
                
                var top13Line = Line.CreateBound(tmpStPoint, tmpStPoint - ClsRevitUtil.CovertToAPI(500.0) * XYZ.BasisZ);//鋼材サイズが500より大きいものが無く交点が見つかればよいため500
                var insec13 = ClsRevitUtil.GetIntersection(line3, top13Line);
                var top23Line = Line.CreateBound(tmpEdPoint, tmpEdPoint - ClsRevitUtil.CovertToAPI(500.0) * XYZ.BasisZ);
                var insec23 = ClsRevitUtil.GetIntersection(line3, top23Line);
                if (insec13 != null && insec23 != null)
                {
                    var top13Z = tmpStPoint.Z - insec13.Z;
                    var top23Z = tmpEdPoint.Z - insec23.Z;

                    tmpStPoint = line3.GetEndPoint(0) + top13Z * XYZ.BasisZ;
                    tmpEdPoint = line3.GetEndPoint(1) + top23Z * XYZ.BasisZ;
                }
            }
            return (tmpStPoint, tmpEdPoint);
        }

        /// <summary>
        /// ﾌﾞﾙﾏﾝの作図（ベースのエレメントIDが必須）
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public bool CreateBruman(Document doc, ElementId id1, ElementId id2, ElementId id3)
        {
            ElementId baseID = m_ElementId;
            if (baseID == null)
            {
                return false;
            }
            FamilySymbol sym = null;

            var vec = 1;
            if (m_ToritsukeHoho == ToritsukeHoho.Buruman)
            {
                if (m_SteelType != "チャンネル")
                {
                    string buruFamPath = Master.ClsBurumanCSV.GetFamilyPath("C-50");
                    string buruFamName = RevitUtil.ClsRevitUtil.GetFamilyName(buruFamPath);
                    if (!ClsRevitUtil.LoadFamilyData(doc, buruFamPath, out Family buruFam))
                    {
                        //return false; ;
                    }
                    sym = (ClsRevitUtil.GetFamilySymbol(doc, buruFamName, "切梁繋ぎ"));//タイプが追加され次第　斜梁繋ぎ
                    //ベクトルが他と違うため
                    vec = -1;
                }
                else
                {
                    string buruFamPath = Master.ClsBurumanCSV.GetFamilyPath("G");
                    string buruFamName = RevitUtil.ClsRevitUtil.GetFamilyName(buruFamPath);
                    if (!ClsRevitUtil.LoadFamilyData(doc, buruFamPath, out Family buruFam))
                    {
                        //return false; ;
                    }
                    sym = (ClsRevitUtil.GetFamilySymbol(doc, buruFamName, "切梁繋ぎ"));//タイプが追加され次第　斜梁繋ぎ
                    ////ベクトルが他と違うため
                    //vec = -1;
                }

            }
            else if (m_ToritsukeHoho == ToritsukeHoho.Rikiman)
            {
                string rikiFamPath = Master.ClsRikimanCSV.GetFamilyPath("G");
                string rikiFamName = RevitUtil.ClsRevitUtil.GetFamilyName(rikiFamPath);
                if (!ClsRevitUtil.LoadFamilyData(doc, rikiFamPath, out Family buruFam))
                {
                    //return false; ;
                }
                sym = (ClsRevitUtil.GetFamilySymbol(doc, rikiFamName, "切梁繋ぎ"));
            }
            else
                return true;

            double sizeH, sizeB;
            switch (m_SteelType)
            {
                case "チャンネル":
                    {
                        var h = ClsCommonUtils.ChangeStrToDbl(Master.ClsYamadomeCsv.GetKouzaiSizeSunpou(m_SteelSize, 1));
                        sizeH = ClsRevitUtil.CovertToAPI(h);
                        var b = ClsCommonUtils.ChangeStrToDbl(Master.ClsYamadomeCsv.GetKouzaiSizeSunpou(m_SteelSize, 2));
                        sizeB = -ClsRevitUtil.CovertToAPI(b);
                        break;
                    }
                case "アングル":
                case "H形鋼 広幅":
                case "H形鋼 中幅":
                case "H形鋼 細幅":
                    {
                        var h = ClsCommonUtils.ChangeStrToDbl(Master.ClsYamadomeCsv.GetKouzaiSizeSunpou(m_SteelSize, 1));
                        sizeH = ClsRevitUtil.CovertToAPI(h / 2);
                        var b = ClsCommonUtils.ChangeStrToDbl(Master.ClsYamadomeCsv.GetKouzaiSizeSunpou(m_SteelSize, 2));
                        sizeB = -ClsRevitUtil.CovertToAPI(b / 2);
                        break;
                    }
                default:
                    {
                        var sunpou = Master.ClsYamadomeCsv.GetWidth(m_SteelSize);
                        sizeH = ClsRevitUtil.CovertToAPI(sunpou / 2);
                        sizeB = -ClsRevitUtil.CovertToAPI(sunpou / 2);
                        break;
                    }
            }

            var inst = doc.GetElement(id3) as FamilyInstance;
            var referencePlane = inst.Host as ReferencePlane;
            var cv = ClsYMSUtil.GetBaseLine(doc, baseID);

            Plane sectionPlane;
            // 斜梁つなぎベースを鋼材分ずらした位置から鉛直方向の断面平面を作成
            {
                var tsunagiLine = ClsYMSUtil.GetBaseLine(doc, id3);
                
                var tmpStPoint = cv.GetEndPoint(0);
                var tmpEdPoint = cv.GetEndPoint(1);
                var dir = Line.CreateBound(tmpStPoint, tmpEdPoint).Direction;

                //斜梁繋ぎ材ベースの位置を鋼材サイズ分ベクトルの向きに対して右横にずらす
                tmpStPoint = new XYZ(tmpStPoint.X + (sizeB * dir.Y),
                                     tmpStPoint.Y - (sizeB * dir.X),
                                     tmpStPoint.Z);
                tmpEdPoint = new XYZ(tmpEdPoint.X + (sizeB * dir.Y),
                                     tmpEdPoint.Y - (sizeB * dir.X),
                                     tmpEdPoint.Z);
                var a = tmpStPoint;// tsunagiLine.GetEndPoint(0);
                var b = tmpEdPoint;// tsunagiLine.GetEndPoint(1);
                //Z軸成分を0にする
                a = new XYZ(a.X, a.Y, 0);
                b = new XYZ(b.X, b.Y, 0);
                //鉛直方向のベクトルを求める必要があるかもXYZ.BasicZでは鉛直にならないパターンがあるため
                sectionPlane = Plane.CreateByOriginAndBasis(a, (b - a).Normalize(), XYZ.BasisZ);//Z軸が同じでなければいけない
            }

            var pickResultSet = new ElementId[] { id1, id2 };
            var topPoints = new List<XYZ>();
            var sectionBBoxes = new List<XYZ[]>();
            foreach (var pickResult in pickResultSet)
            {
                var shabariBase = doc.GetElement(pickResult) as FamilyInstance;

                // 斜梁ベースの直線を斜梁ベースファミリの X 軸として計算
                var shabariBaseTrans = shabariBase.GetTotalTransform();
                var shabariBaseLine = Line.CreateUnbound(shabariBaseTrans.Origin, shabariBaseTrans.BasisX);

                // 斜梁ベースの平面は Host から取得
                var shabariBasePlane = (shabariBase.Host as ReferencePlane)?.GetPlane();

                var shabariSize = Master.ClsYamadomeCsv.GetWidth(ClsRevitUtil.GetParameter(doc, pickResult, "鋼材サイズ"));
                var shabariW = shabariSize;
                var shabariH = shabariSize;

                // 斜梁断面の端点の中で Z 成分が最大の点を取得
                var sectionTop = ClsYMSUtil.CalcShabariSectionTop(shabariBaseLine, shabariBasePlane, shabariW, shabariH, sectionPlane, true);
                if (sectionTop != null)
                {
                    topPoints.Add(sectionTop);
                }
                else
                {
                    var dialog = new TaskDialog(baseName)
                    {
                        MainInstruction = "斜梁断面の端点が算出できません",
                        MainContent = "斜梁ベースと斜梁ベースつなぎが平行かもしれません",
                        MainIcon = TaskDialogIcon.TaskDialogIconError,
                    };
                    //dialog.Show();
                }
            }

            using (var t = new Transaction(doc, Guid.NewGuid().ToString()))
            {
                t.Start();

                // 斜梁断面の最高点を結ぶモデル線分を作成
                if (topPoints.Count == 2)
                {
                    var l = Line.CreateBound(topPoints.FirstOrDefault(), topPoints.LastOrDefault());
                    var tmpStPoint = l.GetEndPoint(0);
                    var tmpEdPoint = l.GetEndPoint(1);
                    var dir = l.Direction;

                    var shabariBase1 = doc.GetElement(pickResultSet[0]) as FamilyInstance;
                    var shabariBaseLine1 = ClsYMSUtil.GetBaseLine(doc, pickResultSet[0]);
                    var vec1 = new XYZ();
                    if (ClsGeo.IsLeft(topPoints[0] - shabariBaseLine1.GetEndPoint(0), shabariBaseLine1.GetEndPoint(1) - shabariBaseLine1.GetEndPoint(0)))
                    {
                        vec1 = ((topPoints[0] - topPoints[1]).Normalize() + (shabariBaseLine1.GetEndPoint(0) - shabariBaseLine1.GetEndPoint(1)).Normalize()).Normalize();
                    }
                    else
                    {
                        vec1 = ((topPoints[1] - topPoints[0]).Normalize() + (shabariBaseLine1.GetEndPoint(0) - shabariBaseLine1.GetEndPoint(1)).Normalize()).Normalize();
                    }
                    ElementId createId = ClsRevitUtil.Create(doc, topPoints[0], vec1 * vec, referencePlane, sym);
                    ClsKariKouzai.SetKariKouzaiFlag(doc, createId);

                    var shabariBase2 = doc.GetElement(pickResultSet[1]) as FamilyInstance;
                    var shabariBaseLine2 = ClsYMSUtil.GetBaseLine(doc, pickResultSet[1]);
                    var vec2 = new XYZ();
                    if (ClsGeo.IsLeft(topPoints[1] - shabariBaseLine2.GetEndPoint(0), shabariBaseLine2.GetEndPoint(1) - shabariBaseLine2.GetEndPoint(0)))
                    {
                        vec2 = ((topPoints[0] - topPoints[1]).Normalize() + (shabariBaseLine2.GetEndPoint(0) - shabariBaseLine2.GetEndPoint(1)).Normalize()).Normalize();
                    }
                    else
                    {
                        vec2 = ((topPoints[1] - topPoints[0]).Normalize() + (shabariBaseLine2.GetEndPoint(0) - shabariBaseLine2.GetEndPoint(1)).Normalize()).Normalize();
                    }
                    ElementId createId2 = ClsRevitUtil.Create(doc, topPoints[1], vec2 * vec, referencePlane, sym);
                    ClsKariKouzai.SetKariKouzaiFlag(doc, createId2);

                    ElementId levelId = ClsRevitUtil.GetParameterElementId(doc, id3, "集計レベル");
                    if (levelId != null)
                    {
                        ClsRevitUtil.SetParameter(doc, createId, "集計レベル", levelId);
                    }

                    //var point = tmpStPoint;
                    //foreach (var pickResult in pickResultSet)
                    //{
                    //    var shabariBase = doc.GetElement(pickResult) as FamilyInstance;
                    //    var shabariBaseLine = ClsYMSUtil.GetBaseLine(doc, pickResult);
                    //    var insec = ClsRevitUtil.GetIntersection(cv, shabariBaseLine);

                    //    //交点が無ければブルマンは配置されない
                    //    //されるパターンがあるのであれば角度算出に使っている交点がないパターンでは代用するものを検討
                    //    if (insec != null)
                    //    {
                    //        ElementId createId = ClsRevitUtil.Create(doc, point, vec * (insec - point).Normalize(), referencePlane, sym);
                    //        //ClsKariKouzai.SetBaseId(doc, createId, id3);
                    //        //40位オフセットしたほうがよいかも
                    //        //dSize += ClsRevitUtil.GetParameterDouble(doc, createId, "ホストからのオフセット");
                    //        //ClsRevitUtil.SetParameter(doc, createId, "ホストからのオフセット", dSize);
                    //        //ClsRevitUtil.SetParameter(doc, createId, "集計レベル", levelId);
                    //        //var angle = -(dir.AngleOnPlaneTo(XYZ.BasisX, XYZ.BasisZ) + ClsGeo.Deg2Rad(150));
                    //        //var axis = Line.CreateBound(tmpStPoint, tmpStPoint + XYZ.BasisZ);
                    //        //ClsRevitUtil.RotateElement(doc, createId, axis, angle);
                    //        ElementId levelId = ClsRevitUtil.GetParameterElementId(doc, id3, "集計レベル");
                    //        if (levelId != null)
                    //        {
                    //            ClsRevitUtil.SetParameter(doc, createId, "集計レベル", levelId);
                    //        }
                    //    }
                    //    point = tmpEdPoint;
                    //}
                }
                t.Commit();
            }
            return true;
        }

        private static bool IsPointOnLine(XYZ point, Line line)
        {

            return true;
        }

        private static bool IsPointOnLine2(XYZ a, XYZ b, XYZ p, double tolerance = 1e-9)
        {
            // 2Dベクトルとして扱うため、Zを無視
            var ax = a.X;
            var ay = a.Y;
            var bx = b.X;
            var by = b.Y;
            var px = p.X;
            var py = p.Y;

            // ベクトル AB, AP
            double abX = bx - ax;
            double abY = by - ay;
            double apX = px - ax;
            double apY = py - ay;

            // 外積（2D）で一直線上かをチェック
            double cross = abX * apY - abY * apX;
            if (Math.Abs(cross) > tolerance)
                return false;

            // 点 p が AB の内側にあるか（スカラー積でチェック）
            double dot = abX * apX + abY * apY;
            if (dot < -tolerance)
                return false;

            double abLenSq = abX * abX + abY * abY;
            if (dot - abLenSq > tolerance)
                return false;

            return true;
        }

        public void SetParameter(Document doc, ElementId id)
        {
            //ClsRevitUtil.SetMojiParameter(doc, id, "段", m_Dan);
            ClsRevitUtil.SetMojiParameter(doc, id, "分類", m_SteelType);
            ClsRevitUtil.SetMojiParameter(doc, id, "サイズ", m_SteelSize);
            ClsRevitUtil.SetMojiParameter(doc, id, "取付方法", GetToritsukeHoho(m_ToritsukeHoho));
            if (m_ToritsukeHoho == ToritsukeHoho.Bolt)
            {
                ClsRevitUtil.SetMojiParameter(doc, id, "ボルトタイプ1", m_BoltType1);
                ClsRevitUtil.SetMojiParameter(doc, id, "ボルトタイプ2", m_BoltType2);
                ClsRevitUtil.SetParameter(doc, id, "ボルト本数", m_BoltNum);
                //ボルト情報をカスタムデータとして設定する
                ClsYMSUtil.SetBolt(doc, id, m_BoltType2, m_BoltNum);
            }
            else
            {
                //ボルト情報のカスタムデータを削除する
                ClsYMSUtil.DeleteBolt(doc, id);
            }
        }
        /// <summary>
        /// 指定したIDから斜梁繋ぎ材ベースクラスを取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public void SetClassParameter(Document doc, ElementId id)
        {
            m_ElementId = id;
            m_Floor = doc.GetElement(ClsRevitUtil.GetParameterElementId(doc, id, "集計レベル")).Name;
            m_SteelType = ClsRevitUtil.GetParameter(doc, id, "分類");
            m_SteelSize = ClsRevitUtil.GetParameter(doc, id, "サイズ");
            m_ToritsukeHoho = GetToritsukeHoho(ClsRevitUtil.GetParameter(doc, id, "取付方法"));
            if (m_ToritsukeHoho == ToritsukeHoho.Bolt)
            {
                m_BoltType1 = ClsRevitUtil.GetParameter(doc, id, "ボルトタイプ1");
                m_BoltType2 = ClsRevitUtil.GetParameter(doc, id, "ボルトタイプ2");
                m_BoltNum = ClsRevitUtil.GetParameterInteger(doc, id, "ボルト本数");
            }
        }
        /// <summary>
        /// 斜梁繋ぎ材ベース のみを複数選択
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
        /// 図面上の 斜梁繋ぎ材ベース を全て取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<ElementId> GetAllBaseList(Document doc, ElementId levelId = null)
        {
            if (levelId == null)
                return ClsRevitUtil.GetSelectCreatedFamilyInstanceList(doc, baseName);
            else
                return ClsRevitUtil.GetSelectLevelCreatedFamilyInstanceList(doc, baseName, levelId);
        }
        /// <summary>
        ///  図面上の 斜梁繋ぎ材ベース を全てクラスで取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<ClsSyabariTsunagizaiBase> GetAllClsBaseList(Document doc)
        {
            var lstBase = new List<ClsSyabariTsunagizaiBase>();

            var lstId = GetAllBaseList(doc);
            foreach (var id in lstId)
            {
                var cls = new ClsSyabariTsunagizaiBase();
                cls.SetClassParameter(doc, id);
                lstBase.Add(cls);
            }

            return lstBase;
        }
        private string GetToritsukeHoho(ToritsukeHoho toritsuke)
        {
            switch (toritsuke)
            {
                case ToritsukeHoho.Bolt:
                    {
                        return "ボルト";
                    }
                case ToritsukeHoho.Buruman:
                    {
                        return "ブルマン";
                    }
                case ToritsukeHoho.Rikiman:
                    {
                        return "リキマン";
                    }
                default:
                    {
                        return "ボルト";
                    }
            }
        }
        private ToritsukeHoho GetToritsukeHoho(string toritsuke)
        {
            switch (toritsuke)
            {
                case "ボルト":
                    {
                        return ToritsukeHoho.Bolt;
                    }
                case "ブルマン":
                    {
                        return ToritsukeHoho.Buruman;
                    }
                case "リキマン":
                    {
                        return ToritsukeHoho.Rikiman;
                    }
                default:
                    {
                        return 0;
                    }
            }
        }

        /// <summary>
        /// 斜梁 のみを単独選択
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="id">選択した斜梁 のID</param>
        /// <param name="message">指示</param>
        /// <returns></returns>
        public static bool PickObject(UIDocument uidoc, ref ElementId id, string message = "斜梁")
        {
            return ClsRevitUtil.PickObjectPartFilter(uidoc, message + "を選択してください", "斜梁", ref id);
        }


        /// <summary>
        /// 斜梁のみを単独選択
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="id">選択した 斜梁のID</param>
        /// <param name="message">指示</param>
        /// <returns></returns>
        public static bool PickObject(UIDocument uidoc, ref Reference rf, string message = baseName)
        {
            return ClsRevitUtil.PickObject(uidoc, message, "斜梁", ref rf);
        }
        #endregion
    }
}
