using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using Document = Autodesk.Revit.DB.Document;

namespace YMS.Parts
{
    public class ClsHaraokoshiSuberidome
    {

        public enum BuzaiType
        {
            HojoPiece,
            Channel
        }

        #region 変数

        /// <summary>
        /// 部材タイプ
        /// </summary>
        public BuzaiType m_buzaiType { get; set; }

        /// <summary>
        /// 部材サイズ
        /// </summary>
        public string m_buzaiSize { get; set; }

        /// <summary>
        /// すべり方向は右向きか？
        /// </summary>
        public bool m_isRight { get; set; }

        /// <summary>
        /// ボルト種類
        /// </summary>
        public string m_BoltType { get; set; }

        /// <summary>
        /// ボルトサイズ
        /// </summary>
        public string m_BoltSize { get; set; }

        /// <summary>
        /// ボルト本数
        /// </summary>
        public int m_BoltNum { get; set; }

        #endregion

        //コンストラクタ
        public ClsHaraokoshiSuberidome()
        {
            Init();
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Init()
        {
            m_buzaiType = BuzaiType.HojoPiece;
            m_buzaiSize = string.Empty;
            m_isRight = false;
            m_BoltType = string.Empty;
            m_BoltSize = string.Empty;
            m_BoltNum = 0;
        }

        public bool CreateHaraokoshiSuberidome(Document doc, ElementId oyakuiId, ref string uniqueName)
        {
            // ファミリの取得処理
            string symbolFolpath = ClsZumenInfo.GetYMSFolder();
            string familyPath = string.Empty;
            string typeName = string.Empty;
            switch (m_buzaiType)
            {
                case BuzaiType.HojoPiece:
                    familyPath = Master.ClsSupportPieceCsv.GetFamilyPath(m_buzaiSize);
                    typeName = "切梁";
                    break;
                case BuzaiType.Channel:
                    familyPath = Master.ClsHaraokoshiSuberidomeCsv.GetFamilyPath(m_buzaiSize);
                    string tmp = familyPath.Replace(".rfa", "");
                    string[] parts = tmp.Split('\\');
                    string extractedName = parts[parts.Length - 1];
                    typeName = extractedName;
                    break;
                default:
                    return false;
            }
            string familyName = ClsRevitUtil.GetFamilyName(familyPath);

            if (!ClsRevitUtil.LoadFamilyData(doc, familyPath, out Family family))
            {
                return false;
            }
            FamilySymbol sym = (ClsRevitUtil.GetFamilySymbol(doc, familyName, typeName));

            // 作図位置の算出処理
            FamilyInstance instKui = doc.GetElement(oyakuiId) as FamilyInstance;

            if (instKui.Symbol.Family.Name.Contains("鋼矢板"))
            {
                var enumValueKussaku = VoidVec.Kussaku;
                var nKussaku = (int)enumValueKussaku;
                var enumValueKabe = VoidVec.Kabe;
                var nKabe = (int)enumValueKabe;

                if (m_buzaiType == BuzaiType.HojoPiece)
                {
                    if (ClsKouyaita.GetVoidvec(doc, oyakuiId) == nKussaku)
                    {
                        return false;
                    }
                }
                else if (m_buzaiType == BuzaiType.Channel)
                {
                    if (ClsKouyaita.GetVoidvec(doc, oyakuiId) == nKabe)
                    {
                        return false;
                    }
                }
            }

            XYZ ptOyakui = instKui.GetTotalTransform().Origin;

            FamilyInstance instHaraokoshiBase = null;
            XYZ ptCrossPoint = null;
            XYZ ptKuiFrontPoint = null;
            if (!GetTargetHaraokosiBase(doc, instKui, ref ptOyakui, out instHaraokoshiBase, out ptCrossPoint, out ptKuiFrontPoint))
            {
                return false;
            }

            XYZ ptInsertPoint = ptCrossPoint;

            // 鋼材の幅の分だけオフセット
            string steelSizeOrign = ClsRevitUtil.GetInstMojiParameter(doc, instHaraokoshiBase.Id, "鋼材サイズ");
            double kouzaiSize = ClsRevitUtil.CovertToAPI(Master.ClsYamadomeCsv.GetWidth(steelSizeOrign));

            XYZ vecKouzaiOffset = (new XYZ(ptOyakui.X,ptOyakui.Y,0) - new XYZ(ptCrossPoint.X, ptCrossPoint.Y, 0)).Normalize();//#34342
            //XYZ vecKouzaiOffset = (ptOyakui - ptCrossPoint).Normalize();
            ptInsertPoint = ptInsertPoint + (kouzaiSize * vecKouzaiOffset);

            // 腹起の横ダブルの分だけオフセット
            if (ClsRevitUtil.GetInstMojiParameter(doc, instHaraokoshiBase.Id, "横本数") == "ダブル")
            {
                ptInsertPoint = ptInsertPoint + (kouzaiSize * vecKouzaiOffset);
            }

            // 腹起の上下段の分だけオフセット
            double offsetZ = double.NaN;
            switch (ClsRevitUtil.GetParameter(doc, instHaraokoshiBase.Id, "段"))
            {
                case "上段":
                    offsetZ = +(kouzaiSize / 2);
                    break;
                case "下段":
                    offsetZ = -(kouzaiSize / 2);
                    break;
                case "同段":
                    offsetZ = 0.0;
                    break;
                default:
                    break;
            }

            // 腹起の縦ダブルの分だけオフセット
            if (ClsRevitUtil.GetInstMojiParameter(doc, instHaraokoshiBase.Id, "縦本数") == "ダブル")
            {
                // 今後の改修で縦ダブルの上下を選択できるようになった場合、ここを改修する
                string sVerticalGap = ClsRevitUtil.GetInstMojiParameter(doc, instHaraokoshiBase.Id, "縦方向の隙間");
                double dVerticalGap = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(sVerticalGap));

                offsetZ = +((kouzaiSize / 2) + (dVerticalGap / 2));
            }

            bool isFristTime = false;

            // 作図処理
            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                t.Start();

                // 一意の名前を生成
                if (uniqueName == string.Empty)
                {
                    uniqueName = GetUniqueSymbolName(doc, familyName, typeName);
                    isFristTime = true;
                }

                // ファミリシンボルの複製
                FamilySymbol duplicatedSymbol = sym;

                ElementId createdId = null;
                if (m_buzaiType == BuzaiType.HojoPiece)
                {
                    createdId = ClsRevitUtil.Create(doc, ptInsertPoint, instHaraokoshiBase.Host.Id, duplicatedSymbol);
                }
                else if (m_buzaiType == BuzaiType.Channel)
                {
                    createdId = ClsRevitUtil.Create(doc, ptKuiFrontPoint, instHaraokoshiBase.Host.Id, duplicatedSymbol);
                }

                //タイプ名を変更
                duplicatedSymbol = ClsRevitUtil.ChangeTypeID(doc, duplicatedSymbol, createdId, uniqueName);

                // 部材の高さを設定
                ClsRevitUtil.SetParameter(doc, createdId, "基準レベルからの高さ", offsetZ);

                if (m_buzaiType == BuzaiType.Channel)
                {
                    // スベリ方向の設定
                    if (m_isRight)
                    {
                        ClsRevitUtil.SetTypeParameter(duplicatedSymbol, "右側鋼材", 0);
                        ClsRevitUtil.SetTypeParameter(duplicatedSymbol, "左側鋼材", 1);
                    }
                    else
                    {
                        ClsRevitUtil.SetTypeParameter(duplicatedSymbol, "右側鋼材", 1);
                        ClsRevitUtil.SetTypeParameter(duplicatedSymbol, "左側鋼材", 0);
                    }

                    // 杭側鋼材の位置 = 杭の半分のサイズ + 本体の高さ？
                    double height = ClsRevitUtil.CovertToAPI(300.0);
                    ClsRevitUtil.SetTypeParameter(duplicatedSymbol, "杭側鋼材の位置", height);

                    // 杭側鋼材の長さ = 杭側鋼材の長さ + 杭の表面位置から挿入点までの距離
                    double channelLength = ClsRevitUtil.GetTypeParameter(duplicatedSymbol, "杭側鋼材の長さ");
                    //XYZ vector = ptInsertPoint - ptKuiFrontPoint;
                    XYZ vector = (new XYZ(ptInsertPoint.X, ptInsertPoint.Y, 0) - new XYZ(ptKuiFrontPoint.X, ptKuiFrontPoint.Y, 0));//#34342
                    channelLength = channelLength + vector.GetLength();
                    if (isFristTime)
                    {
                        ClsRevitUtil.SetTypeParameter(duplicatedSymbol, "杭側鋼材の長さ", channelLength);
                    }

                    // 杭面からの距離 = フランジの厚さ + 杭の表面位置から挿入点までの距離
                    if (steelSizeOrign == "80SMH") { steelSizeOrign = "40HA"; };
                    double flangeT = Master.ClsYamadomeCsv.GetFlange(steelSizeOrign);
                    flangeT = ClsRevitUtil.CovertToAPI(flangeT);

                    ClsRevitUtil.SetTypeParameter(duplicatedSymbol, "杭面からの距離", 0);
                    ClsRevitUtil.SetTypeParameter(duplicatedSymbol, "杭面からの距離", flangeT + vector.GetLength());

                    // 腹起芯から穴位置までの高さ = 主材の寸法詳細から取得するしかない

                }

                // シンボルを回転
                if (m_buzaiType == BuzaiType.HojoPiece)
                {
                    Line rotationAxis = Line.CreateBound(ptInsertPoint, ptInsertPoint + XYZ.BasisZ);
                    XYZ vec = (ptOyakui - ptCrossPoint).Normalize();
                    double radians = XYZ.BasisX.AngleOnPlaneTo(vec, XYZ.BasisZ);
                    ElementTransformUtils.RotateElement(doc, createdId, rotationAxis, radians);
                }
                else if (m_buzaiType == BuzaiType.Channel)
                {
                    FamilyInstance instTrg = doc.GetElement(createdId) as FamilyInstance;
                    double angle = GetRotateAngle(ptOyakui, ptInsertPoint, instTrg);
                    ElementTransformUtils.RotateElement(doc, createdId, Line.CreateBound(ptKuiFrontPoint, ptKuiFrontPoint + XYZ.BasisZ), angle);
                }

                //#31575
                //ClsRevitUtil.SetMojiParameter(doc, createdId, "ボルトタイプ1", m_BoltType1);
                //ClsRevitUtil.SetMojiParameter(doc, createdId, "ボルトタイプ2", m_BoltType2);
                //ClsRevitUtil.SetParameter(doc, createdId, "ボルト本数", m_BoltNum);

                //ボルト情報をカスタムデータとして設定する #31575
                ClsYMSUtil.SetBolt(doc, createdId, m_BoltSize, m_BoltNum);

                t.Commit();
            }

            //Curve cv = Line.CreateBound(ptInsertPoint, ptKuiFrontPoint);
            //CreateDebugLine(doc, ptInsertPoint, cv);

            return true;
        }

        public bool PickObjectsKabe(UIDocument uidoc, ref List<ElementId> ids, string message = "杭")
        {
            List<string> filterList = new List<string>();
            filterList.Add(ClsKouyaita.KOUYAITA);
            filterList.Add(ClsKouyaita.CORNERYAITA);
            filterList.Add(ClsKouyaita.IKEIYAITA);
            if (m_buzaiType == BuzaiType.Channel)
            {
                filterList.Add(ClsOyakui.oyakui);
                filterList.Add(ClsRenzokukabe.RENZOKUKABE);
                filterList.Add(ClsSMW.SMW);
            }
            return ClsRevitUtil.PickObjectsPartListFilter(uidoc, message + "を選択してください", filterList, ref ids);
        }

        private bool GetTargetHaraokosiBase(Document doc, FamilyInstance instKui, ref XYZ ptOyakui, out FamilyInstance instHaraokoshiBase, out XYZ ptCrossPoint, out XYZ ptKuiFrontPoint)
        {
            instHaraokoshiBase = null;
            ptCrossPoint = null;
            ptKuiFrontPoint = null;

            // 杭からの判定線を作成 -H- のように伸びる
            double searchRange = ClsRevitUtil.ConvertDoubleGeo2Revit(1000.0);   // 杭からの探査範囲
            double kuiSizeHalf = double.NaN;

            XYZ direction = new XYZ(instKui.FacingOrientation.X, instKui.FacingOrientation.Y, 0.0);
            //XYZ direction2 = new XYZ(instKui.FacingOrientation.X, instKui.FacingOrientation.Y, 0.0);

            XYZ kuiPtSF = new XYZ(), kuiPtSB = new XYZ();
            double yaitaHigh = 0.0;

            if (instKui.Symbol.Family.Name.Contains("杭"))
            {
                kuiSizeHalf = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou1(instKui.Symbol.FamilyName, 1)) / 2);
                kuiPtSF = instKui.GetTotalTransform().Origin + kuiSizeHalf * direction;
                kuiPtSB = instKui.GetTotalTransform().Origin - kuiSizeHalf * direction;

            }
            else if (instKui.Symbol.Family.Name.Contains("鋼矢板"))
            {
                //direction = new XYZ(instKui.HandOrientation.X, instKui.HandOrientation.Y, 0.0);
                kuiSizeHalf = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(Master.ClsKouyaitaCsv.GetWidth(instKui.Symbol.FamilyName)) / 2);

                if (m_buzaiType == BuzaiType.Channel)
                    yaitaHigh = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(Master.ClsKouyaitaCsv.GetHeight(instKui.Symbol.FamilyName)));

                kuiPtSF = instKui.GetTotalTransform().Origin + kuiSizeHalf * instKui.HandOrientation + yaitaHigh * direction;
                kuiPtSB = instKui.GetTotalTransform().Origin + kuiSizeHalf * instKui.HandOrientation - yaitaHigh * direction;
                ptOyakui = kuiPtSF;
            }

            //kuiPtSF = instKui.GetTotalTransform().Origin + kuiSizeHalf * direction + yaitaHigh * direction2;
            //kuiPtSB = instKui.GetTotalTransform().Origin - kuiSizeHalf * direction - yaitaHigh * direction2;
            kuiPtSF = new XYZ(kuiPtSF.X, kuiPtSF.Y, XYZ.Zero.Z);
            kuiPtSB = new XYZ(kuiPtSB.X, kuiPtSB.Y, XYZ.Zero.Z);

            XYZ kuiPtEF = kuiPtSF + searchRange * direction;
            XYZ kuiPtEB = kuiPtSB - searchRange * direction;
            kuiPtEF = new XYZ(kuiPtEF.X, kuiPtEF.Y, XYZ.Zero.Z);
            kuiPtEB = new XYZ(kuiPtEB.X, kuiPtEB.Y, XYZ.Zero.Z);

            Curve cvHanteiLineFront = Line.CreateBound(kuiPtSF, kuiPtEF);
            Curve cvHanteiLineBack = Line.CreateBound(kuiPtSB, kuiPtEB);

            // 図面上の腹起ベースを取得
            List<ElementId> baseInstancesIds = new List<ElementId>();
            baseInstancesIds = ClsRevitUtil.GetSelectCreatedFamilyInstanceList(doc, "腹起ベース", true);
            if (baseInstancesIds.Count == 0)
            {
                return false;
            }

            foreach (var id in baseInstancesIds)
            {
                instHaraokoshiBase = doc.GetElement(id) as FamilyInstance;

                // 腹起の始点と終点を取得し、Curveを作成
                LocationCurve locationCurve = instHaraokoshiBase.Location as LocationCurve;
                if (locationCurve != null)
                {
                    XYZ ptBaseSt = locationCurve.Curve.GetEndPoint(0);
                    XYZ ptBaseEd = locationCurve.Curve.GetEndPoint(1);
                    Curve cvBase = Line.CreateBound(ptBaseSt, ptBaseEd);

                    // CurveのZ座標を0に設定
                    cvBase = Line.CreateBound(new XYZ(cvBase.GetEndPoint(0).X, cvBase.GetEndPoint(0).Y, XYZ.Zero.Z),
                                              new XYZ(cvBase.GetEndPoint(1).X, cvBase.GetEndPoint(1).Y, XYZ.Zero.Z));

                    // 判定
                    SetComparisonResult resultF = cvBase.Intersect(cvHanteiLineFront);
                    if (resultF == SetComparisonResult.Overlap)
                    {
                        ptCrossPoint = ClsRevitUtil.GetIntersection(cvHanteiLineFront, cvBase);
                        ptKuiFrontPoint = kuiPtSF;
                        break;
                    }
                    SetComparisonResult resultB = cvBase.Intersect(cvHanteiLineBack);
                    if (resultB == SetComparisonResult.Overlap)
                    {
                        ptCrossPoint = ClsRevitUtil.GetIntersection(cvHanteiLineBack, cvBase);
                        ptKuiFrontPoint = kuiPtEF;
                        break;
                    }
                }
            }

            if (instHaraokoshiBase == null)
            {
                return false;
            }
            if (ptCrossPoint == null)
            {
                return false;
            }

            return true;
        }

        private static double GetRotateAngle(XYZ pKui, XYZ insertPointSt, FamilyInstance inst)
        {
            // 基準点と挿入点
            XYZ basePoint = pKui; // 基準点の座標

            // 基準点から挿入点へのベクトルを取得
            XYZ direction = basePoint - insertPointSt;

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

        /// <summary>
        /// ファミリシンボルをコピーする際に使用する一意の名前を取得する
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

        public static bool PickHaraokoshiSuberidomeObjects(UIDocument uidoc, ref List<ElementId> ids, string message = "腹起スベリ止メ")
        {
            return ClsRevitUtil.PickObjectsPartFilter(uidoc, message + "を選択してください", "腹起ｽﾍﾞﾘ止ﾒ", ref ids);
        }

        public void SetParameter(Document doc, ElementId id)
        {
            FamilyInstance inst = doc.GetElement(id) as FamilyInstance;

            string buzaiSize = string.Empty; GetBuzaiSizeFromFileName(inst.Symbol.FamilyName, ref buzaiSize);

            // 部材タイプ
            string SupportPiecePath = Master.ClsSupportPieceCsv.GetFamilyPath(buzaiSize);
            string HaraokoshiSuberidomPath = Master.ClsHaraokoshiSuberidomeCsv.GetFamilyPath(buzaiSize);
            if (File.Exists(SupportPiecePath))
            {
                m_buzaiType = BuzaiType.HojoPiece;
            }
            else if (File.Exists(HaraokoshiSuberidomPath))
            {
                m_buzaiType = BuzaiType.Channel;
            }

            // 部材サイズ
            m_buzaiSize = buzaiSize;

            // スベリ方向
            if (ClsRevitUtil.GetTypeParameterInt(inst.Symbol, "右側鋼材") == 1)
            {
                m_isRight = false;
            }
            else if (ClsRevitUtil.GetTypeParameterInt(inst.Symbol, "左側鋼材") == 1)
            {
                m_isRight = true;
            }

            //デフォルト値を与える #31575 
            // ボルトタイプ
            //m_BoltType;
            var boltNameAndNum = ClsYMSUtil.GetBolt(doc, id);
            string boltName = boltNameAndNum.Item1 == null ? "ボルト情報なし" : boltNameAndNum.Item1;
            int boltNum = boltNameAndNum.Item2;

            m_BoltType = Master.ClsBoltCsv.GetType(boltName);//Master.ClsBoltCsv.BoltTypes.FirstOrDefault();

            // ボルトサイズ
            //m_BoltSize;
            m_BoltSize = boltName;//Master.ClsBoltCsv.GetSizeList(m_BoltType).FirstOrDefault();

            // ボルト本数
            //m_BoltNum;
            m_BoltNum = boltNum;
        }

        private static bool GetBuzaiSizeFromFileName(string fileName, ref string name)
        {
            List<string> list = Master.ClsHaraokoshiSuberidomeCsv.GetSizeList();
            foreach (string sn in list)
            {
                if (fileName.Contains(sn))
                {
                    name = sn;
                    return true;
                }
            }
            return false;
        }

        public void SetDefaultParameter()
        {
            this.m_buzaiType = BuzaiType.Channel;
            this.m_buzaiSize = Master.ClsHaraokoshiSuberidomeCsv.GetSizeList().FirstOrDefault();
            this.m_isRight = true;
            this.m_BoltType = Master.ClsBoltCsv.BoltTypes.FirstOrDefault();
            this.m_BoltSize = Master.ClsBoltCsv.GetSizeList(m_BoltType).FirstOrDefault();
            this.m_BoltNum = 0;
        }

        private static void CreateDebugLine(Document doc, XYZ pt, Curve line)
        {
            // デバッグ用の線分を表示
            using (Transaction transaction = new Transaction(doc, "Create Model Line"))
            {
                transaction.Start();

                // モデル線分を作成
                ElementId levelID = ClsRevitUtil.GetLevelID(doc, ClsKabeShin.GL);
                Plane plane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, pt);
                SketchPlane sketchPlane = SketchPlane.Create(doc, plane);
                ModelCurve modelLineF = doc.Create.NewModelCurve(line, sketchPlane);

                transaction.Commit();
            }
        }

        public static bool IsHaraokoshiSuberidomeChannel(Document doc, List<ElementId> ids)
        {
            foreach (ElementId id in ids)
            {
                FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                string buzaiSize = string.Empty; GetBuzaiSizeFromFileName(inst.Symbol.FamilyName, ref buzaiSize);

                string HaraokoshiSuberidomPath = Master.ClsHaraokoshiSuberidomeCsv.GetFamilyPath(buzaiSize);
                if (!File.Exists(HaraokoshiSuberidomPath))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool GetKuiConHaraokoshiSuberidome(Document doc, FamilyInstance targetHaraokoshiSuberidome, out FamilyInstance oyaKui)
        {
            oyaKui = null;
            XYZ targetPoint = targetHaraokoshiSuberidome.GetTotalTransform().Origin;

            List<ElementId> oyaKuiInstancesIds = ClsRevitUtil.GetSelectCreatedFamilyInstanceList(doc, "親杭", true);
            foreach (var id in oyaKuiInstancesIds)
            {
                FamilyInstance instOyaKui = doc.GetElement(id) as FamilyInstance;

                XYZ direction = new XYZ(instOyaKui.FacingOrientation.X, instOyaKui.FacingOrientation.Y, 0.0);
                double kuiSizeHalf = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou1(instOyaKui.Symbol.FamilyName, 1)) / 2);
                XYZ oriP = instOyaKui.GetTotalTransform().Origin;
                XYZ kuiPtSF = oriP + kuiSizeHalf * direction;
                XYZ kuiPtSB = oriP - kuiSizeHalf * direction;

                double disSFX = targetPoint.X - kuiPtSF.X;
                double disSFY = targetPoint.Y - kuiPtSF.Y;
                double disSBX = targetPoint.X - kuiPtSB.X;
                double disSBY = targetPoint.X - kuiPtSB.X;

                double oyaKuiLengthTmp = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(instOyaKui.Symbol, "長さ"));
                double oyaKuiLength = ClsRevitUtil.CovertToAPI(oyaKuiLengthTmp);
                double pStZ = oriP.Z;
                double pEdZ = oriP.Z - oyaKuiLength;

                if (((ClsGeo.GEO_EQ0(disSFX) && ClsGeo.GEO_EQ0(disSFY)) || (ClsGeo.GEO_EQ0(disSBX) && ClsGeo.GEO_EQ0(disSBY))) &&
                    (ClsGeo.GEO_LE(pEdZ, targetPoint.Z) && ClsGeo.GEO_LE(targetPoint.Z, pStZ)))
                {
                    oyaKui = instOyaKui;
                    return true;
                }
            }

            List<ElementId> yaitaInstancesIds = ClsRevitUtil.GetSelectCreatedFamilyInstanceList(doc, "鋼矢板", true);
            foreach (var id in yaitaInstancesIds)
            {
                FamilyInstance instaita = doc.GetElement(id) as FamilyInstance;

                XYZ direction = new XYZ(instaita.FacingOrientation.X, instaita.FacingOrientation.Y, 0.0);
                double kuiSizeHalf = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(Master.ClsKouyaitaCsv.GetWidth(instaita.Symbol.FamilyName)) / 2);
                double yaitaHigh = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(Master.ClsKouyaitaCsv.GetHeight(instaita.Symbol.FamilyName)));
                XYZ oriP = instaita.GetTotalTransform().Origin;
                XYZ kuiPtSF = oriP + kuiSizeHalf * instaita.HandOrientation + yaitaHigh * direction;
                XYZ kuiPtSB = oriP + kuiSizeHalf * instaita.HandOrientation - yaitaHigh * direction;

                double disSFX = targetPoint.X - kuiPtSF.X;
                double disSFY = targetPoint.Y - kuiPtSF.Y;
                double disSBX = targetPoint.X - kuiPtSB.X;
                double disSBY = targetPoint.X - kuiPtSB.X;

                double yaitaLengthTmp = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(instaita.Symbol, "長さ"));
                double yaitaLength = ClsRevitUtil.CovertToAPI(yaitaLengthTmp);
                double pStZ = oriP.Z;
                double pEdZ = oriP.Z - yaitaLength;

                if (((ClsGeo.GEO_EQ0(disSFX) && ClsGeo.GEO_EQ0(disSFY)) || (ClsGeo.GEO_EQ0(disSBX) && ClsGeo.GEO_EQ0(disSBY))) &&
                    (ClsGeo.GEO_LE(pEdZ, targetPoint.Z) && ClsGeo.GEO_LE(targetPoint.Z, pStZ)))
                {
                    oyaKui = instaita;
                    return true;
                }
            }

            return false;
        }
    }
}
