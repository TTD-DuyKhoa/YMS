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
    public class ClsSyabariUkeBase
    {
        #region 定数
        public const string baseName = "斜梁受け材ベース";
        public const string NAME = "斜梁受け材";
        #endregion

        #region Enum
        /// <summary>
        /// 処理方法
        /// </summary>
        public enum ShoriType
        {
            Replace,
            PileSelect,
            PtoP
        }
        #endregion

        #region プロパティ
        /// <summary>
        /// 処理タイプ
        /// </summary>
        public ShoriType m_ShoriType { get; set; }

        /// <summary>
        /// 作成方法：鋼材タイプ
        /// </summary>
        public string m_SteelType { get; set; }

        /// <summary>
        /// 作成方法：鋼材サイズ
        /// </summary>
        public string m_SteelSize { get; set; }

        /// <summary>
        /// 突き出し量-始点
        /// </summary>
        public int m_TsukidashiRyoS { get; set; }

        /// <summary>
        /// 突き出し量-終点
        /// </summary>
        public int m_TsukidashiRyoE { get; set; }

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
        public ClsSyabariUkeBase()
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
            m_ShoriType = ShoriType.Replace;
            m_SteelType = string.Empty;
            m_SteelSize = string.Empty;
            m_TsukidashiRyoS = 0;
            m_TsukidashiRyoE = 0;
            m_Floor = string.Empty;
            m_Dan = string.Empty;
            m_FlgEdit = false;
        }
        public bool CreateSyabariUkeBase(Document doc, XYZ tmpStPoint, XYZ tmpEdPoint, XYZ tmpThirdPoint, ElementId levelId)
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
        public bool CreateSyabariUke(Document doc, ElementId id1, ElementId id3)
        {
            var kouzaiType = m_SteelType;
            var kouzaiSize = m_SteelSize;//主材のみ
            var dSize = 0.0;
            var dWidth = 0.0;
            string familyPath;

            switch (kouzaiType)
            {
                case "チャンネル":
                    {
                        familyPath = Master.ClsSyabariKouzaiCSV.GetFamilyPath(kouzaiSize);
                        double sunpou2 = ClsCommonUtils.ChangeStrToDbl(Master.ClsYamadomeCsv.GetKouzaiSizeSunpou(kouzaiSize, 1));
                        dWidth = ClsCommonUtils.ChangeStrToDbl(Master.ClsYamadomeCsv.GetKouzaiSizeSunpou(kouzaiSize, 2));
                        dSize += ClsRevitUtil.CovertToAPI(sunpou2 / 2);
                        break;
                    }
                case "アングル":
                    {
                        familyPath = Master.ClsAngleCsv.GetFamilyPath(kouzaiSize);
                        double sunpou2 = ClsCommonUtils.ChangeStrToDbl(Master.ClsYamadomeCsv.GetKouzaiSizeSunpou(kouzaiSize, 1));
                        dWidth = ClsCommonUtils.ChangeStrToDbl(Master.ClsYamadomeCsv.GetKouzaiSizeSunpou(kouzaiSize, 2));
                        dSize += ClsRevitUtil.CovertToAPI(sunpou2 / 2);
                        break;
                    }
                case "H形鋼 広幅":
                case "H形鋼 中幅":
                case "H形鋼 細幅":
                    {
                        familyPath = Master.ClsSyabariKouzaiCSV.GetFamilyPath(kouzaiSize);
                        double sunpou2 = ClsCommonUtils.ChangeStrToDbl(Master.ClsYamadomeCsv.GetKouzaiSizeSunpou(kouzaiSize, 1));
                        dWidth = ClsCommonUtils.ChangeStrToDbl(Master.ClsYamadomeCsv.GetKouzaiSizeSunpou(kouzaiSize, 2));
                        dSize += ClsRevitUtil.CovertToAPI(sunpou2 / 2);
                        break;
                    }
                default:
                    {
                        familyPath = Master.ClsYamadomeCsv.GetKariFamilyPath(kouzaiSize, true);
                        double sunpou2 = Master.ClsYamadomeCsv.GetWidth(kouzaiSize);
                        dWidth = Master.ClsYamadomeCsv.GetWidth(kouzaiSize);
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

            var pickResultSet = new ElementId[] { id1};
            var sectionBottom = new XYZ();
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

                // 斜梁断面の端点の中で Z 成分が最小の点を取得
                sectionBottom = ClsYMSUtil.CalcShabariSectionBottom(shabariBaseLine, shabariBasePlane, shabariW, shabariH, sectionPlane, dWidth);
                if (sectionBottom == null)
                {
                    var dialog = new TaskDialog(baseName)
                    {
                        MainInstruction = "斜梁断面の端点が算出できません",
                        MainContent = "斜梁ベースと斜梁ベース受けが平行かもしれません",
                        MainIcon = TaskDialogIcon.TaskDialogIconError,
                    };
                    dialog.Show();
                }
            }

            using (var transaction = new Transaction(doc, Guid.NewGuid().ToString()))
            {
                transaction.Start();

                // 斜梁断面の最高点を結ぶモデル線分を作成
                var line3 = ClsYMSUtil.GetBaseLine(doc, id3);
                var tmpStPoint = line3.GetEndPoint(0);
                var tmpEdPoint = line3.GetEndPoint(1);

                tmpStPoint = new XYZ(tmpStPoint.X, tmpStPoint.Y, sectionBottom.Z);
                tmpEdPoint = new XYZ(tmpEdPoint.X, tmpEdPoint.Y, sectionBottom.Z);

                ElementId createId = ClsRevitUtil.Create(doc, tmpStPoint, (tmpEdPoint - tmpStPoint).Normalize(), referencePlane, sym);
                ClsKariKouzai.SetBaseId(doc, createId, id3);
                ClsKariKouzai.SetKariKouzaiFlag(doc, createId);
                ClsRevitUtil.SetParameter(doc, createId, "長さ", tmpStPoint.DistanceTo(tmpEdPoint));
                dSize = dSize * -1 + ClsRevitUtil.GetParameterDouble(doc, createId, "ホストからのオフセット");
                ClsRevitUtil.SetParameter(doc, createId, "ホストからのオフセット", dSize);
                //ClsRevitUtil.SetParameter(doc, createId, "集計レベル", levelId);
                ElementId levelId = ClsRevitUtil.GetParameterElementId(doc, id3, "集計レベル");
                if (levelId != null)
                {
                    ClsRevitUtil.SetParameter(doc, createId, "集計レベル", levelId);
                }
                transaction.Commit();
            }
            return true;
        }
        public static (XYZ, XYZ) GetSyabariUkePoint(Document doc, ElementId id1, ElementId id3, double dWidth)
        {
            var cv = ClsYMSUtil.GetBaseLine(doc, id3);
            var tmpStPoint = cv.GetEndPoint(0);
            var tmpEdPoint = cv.GetEndPoint(1);

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

            var pickResultSet = new ElementId[] { id1 };
            var sectionBottom = new XYZ();
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
                // 斜梁断面の端点の中で Z 成分が最小の点を取得
                sectionBottom = ClsYMSUtil.CalcShabariSectionBottom(shabariBaseLine, shabariBasePlane, shabariW, shabariH, sectionPlane, dWidth);
                if (sectionBottom == null)
                {
                    var dialog = new TaskDialog(baseName)
                    {
                        MainInstruction = "斜梁断面の端点が算出できません",
                        MainContent = "斜梁ベースと斜梁ベース受けが平行かもしれません",
                        MainIcon = TaskDialogIcon.TaskDialogIconError,
                    };
                   // dialog.Show();
                }
            }

            using (var transaction = new Transaction(doc, Guid.NewGuid().ToString()))
            {
                transaction.Start();

                // 斜梁断面の最高点を結ぶモデル線分を作成
                var line3 = ClsYMSUtil.GetBaseLine(doc, id3);
                tmpStPoint = line3.GetEndPoint(0);
                tmpEdPoint = line3.GetEndPoint(1);

                tmpStPoint = new XYZ(tmpStPoint.X, tmpStPoint.Y, sectionBottom.Z);
                tmpEdPoint = new XYZ(tmpEdPoint.X, tmpEdPoint.Y, sectionBottom.Z);

            }
            return (tmpStPoint, tmpEdPoint);
        }
        public double GetSyabariUkeWidth()
        {
            var kouzaiType = m_SteelType;
            var kouzaiSize = m_SteelSize;//主材のみ
            var dWidth = 0.0;

            switch (kouzaiType)
            {
                case "チャンネル":
                    {
                        dWidth = ClsCommonUtils.ChangeStrToDbl(Master.ClsYamadomeCsv.GetKouzaiSizeSunpou(kouzaiSize, 2));
                        break;
                    }
                case "アングル":
                    {
                        dWidth = ClsCommonUtils.ChangeStrToDbl(Master.ClsYamadomeCsv.GetKouzaiSizeSunpou(kouzaiSize, 2));
                        break;
                    }
                case "H形鋼 広幅":
                case "H形鋼 中幅":
                case "H形鋼 細幅":
                    {
                        dWidth = ClsCommonUtils.ChangeStrToDbl(Master.ClsYamadomeCsv.GetKouzaiSizeSunpou(kouzaiSize, 2));
                        break;
                    }
                default:
                    {
                        dWidth = Master.ClsYamadomeCsv.GetWidth(kouzaiSize);
                        break;
                    }
            }

            
            return dWidth;
        }
        /// <summary>
        /// 斜梁受け材ベース のみを単独選択
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="id">選択した 斜梁受け材ベース のID</param>
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
        /// 図面上の 斜梁受け材ベース を全て取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<ElementId> GetAllSyabariUkeBaseList(Document doc)
        {
            List<ElementId> htIdList = ClsRevitUtil.GetSelectCreatedFamilyInstanceList(doc, baseName);
            return htIdList;
        }
        /// <summary>
        /// 図面上の 斜梁受け材ベース を全て取得する
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
        ///  図面上の 斜梁受け材ベース を全てクラスで取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<ClsSyabariUkeBase> GetAllClsBaseList(Document doc)
        {
            var lstBase = new List<ClsSyabariUkeBase>();

            var lstId = GetAllBaseList(doc);
            foreach (var id in lstId)
            {
                var cls = new ClsSyabariUkeBase();
                cls.SetClassParameter(doc, id);
                lstBase.Add(cls);
            }

            return lstBase;
        }
        /// <summary>
        /// 斜梁受け材ベース　にパラメータを追加する※長さは追加しない
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id"></param>
        public void SetParameter(Document doc, ElementId id)
        {
            ClsRevitUtil.SetMojiParameter(doc, id, "段", m_Dan);
            ClsRevitUtil.SetMojiParameter(doc, id, "鋼材サイズ", m_SteelSize);
            ClsRevitUtil.SetMojiParameter(doc, id, "鋼材タイプ", m_SteelType);
            ClsRevitUtil.SetParameter(doc, id, "突き出し量始点", m_TsukidashiRyoS);
            ClsRevitUtil.SetParameter(doc, id, "突き出し量終点", m_TsukidashiRyoE);
        }
        /// <summary>
        /// 指定したIDから斜梁受け材ベースクラスを取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public void SetClassParameter(Document doc, ElementId id)
        {
            FamilyInstance shinInstLevel = doc.GetElement(id) as FamilyInstance;

            m_Floor = doc.GetElement(ClsRevitUtil.GetParameterElementId(doc, id, "集計レベル")).Name;
            m_ElementId = id;
            string dan = ClsRevitUtil.GetParameter(doc, id, "段");
            m_SteelSize = ClsRevitUtil.GetParameter(doc, id, "鋼材サイズ");
            m_SteelType = ClsRevitUtil.GetParameter(doc, id, "鋼材タイプ");
            m_TsukidashiRyoS = (int)ClsRevitUtil.GetParameterDouble(doc, id, "突き出し量始点");//ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetParameterDouble(doc, id, "突き出し量始点"));
            m_TsukidashiRyoE = (int)ClsRevitUtil.GetParameterDouble(doc, id, "突き出し量終点");// ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetParameterDouble(doc, id, "突き出し量終点"));

            return;
        }
        #endregion
    }
}
