using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Autodesk.Revit.DB.SpecTypeId;

namespace YMS.Parts
{
    public class ClsSyabariHiuchiBase
    {
        #region 定数
        public const string baseName = "斜梁火打ベース";
        public const string BASEID端部 = "ベースID端";
        #endregion

        #region Enum
        /// <summary>
        /// 配置方法：作成方法
        /// </summary>
        public enum CreateType
        {
            Both,
            AnyLengthManual,
            AnyLengthAuto
        }
        #endregion

        #region プロパティ
        /// <summary>
        /// 配置方法：作成方法
        /// </summary>
        public CreateType m_CreateType { get; set; }

        /// <summary>
        /// 作成方法：鋼材タイプ
        /// </summary>
        public string m_SteelType { get; set; }

        /// <summary>
        /// 作成方法：鋼材サイズ
        /// </summary>
        public string m_SteelSize { get; set; }

        /// <summary>
        /// 作成方法：火打ち長さL
        /// </summary>
        public int m_HiuchiLengthL { get; set; }

        /// <summary>
        /// 部品：部品タイプ（斜梁側）
        /// </summary>
        public string m_PartsTypeSyabariSide { get; set; }

        /// <summary>
        /// 部品：部品サイズ（斜梁側）
        /// </summary>
        public string m_PartsSizeSyabariSide { get; set; }

        /// <summary>
        /// 部品：部品タイプ（腹起側）
        /// </summary>
        public string m_PartsTypeHaraSide { get; set; }

        /// <summary>
        /// 部品：部品サイズ（腹起側）
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
        public ClsSyabariHiuchiBase()
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
            m_CreateType = CreateType.Both;
            m_SteelType = string.Empty;
            m_SteelSize = string.Empty;
            m_HiuchiLengthL = 0;
            m_PartsTypeSyabariSide = string.Empty;
            m_PartsSizeSyabariSide = string.Empty;
            m_PartsTypeHaraSide = string.Empty;
            m_PartsSizeHaraSide = string.Empty;
            m_Angle = 0;
            m_Floor = string.Empty;
            m_Dan = string.Empty;
            m_FlgEdit = false;
        }

        public void SetParameter(Document doc, ElementId id)
        {
            ClsRevitUtil.CustomDataSet<string>(doc, id, "kType", m_SteelType);//"鋼材タイプ"
            ClsRevitUtil.SetMojiParameter(doc, id, "鋼材サイズ", m_SteelSize);//下と同じで不要ではあるが念のため。
            ClsRevitUtil.CustomDataSet<string>(doc, id, "kSize", m_SteelSize);//鋼材サイズ
            ClsRevitUtil.CustomDataSet<int>(doc, id, "kLen", m_HiuchiLengthL);//長さ
            ClsRevitUtil.CustomDataSet<string>(doc, id, "sType", m_PartsTypeSyabariSide);//部品タイプ　斜梁側
            ClsRevitUtil.CustomDataSet<string>(doc, id, "sSize", m_PartsSizeSyabariSide);//部品サイズ　斜梁側
            ClsRevitUtil.CustomDataSet<string>(doc, id, "hType", m_PartsTypeHaraSide);//部品タイプ　腹起し側
            ClsRevitUtil.CustomDataSet<string>(doc, id, "hSize", m_PartsSizeHaraSide);//部品サイズ　腹起し側
            ClsRevitUtil.CustomDataSet<string>(doc, id, "angle", m_Angle.ToString());//長さ
        }

        public void SetClassParameter(Document doc, ElementId id)
        {
            string temps = string.Empty;
            int tempn = 0;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "kType", out temps);
            m_SteelType = temps;
            ClsRevitUtil.CustomDataGet<string>(doc, id, "kSize", out temps);
            m_SteelSize = temps;
            ClsRevitUtil.CustomDataGet<string>(doc, id, "sType", out temps);
            m_PartsTypeSyabariSide = temps;
            ClsRevitUtil.CustomDataGet<string>(doc, id, "sSize", out temps);
            m_PartsSizeSyabariSide = temps;
            ClsRevitUtil.CustomDataGet<string>(doc, id, "hType", out temps);
            m_PartsTypeHaraSide = temps;
            ClsRevitUtil.CustomDataGet<string>(doc, id, "hSize", out temps);
            m_PartsSizeHaraSide = temps;
            ClsRevitUtil.CustomDataGet<int>(doc, id, "kLen", out tempn);
            m_HiuchiLengthL = tempn;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "angle", out temps);
            double tempD = 0.0;
            if(double.TryParse(temps,out tempD))
            {
                m_Angle = tempD;
            }

        }

        //public bool CreateSyabariHiuchiBase(UIDocument uidoc, ElementId id1, ElementId id2, ElementId levelID, string dan, XYZ picPoint1 = null, XYZ picPoint2 = null)
        //{
        //    //ドキュメントを取得
        //    Document doc = uidoc.Document;
        //    return false;
        //}
        //public bool CreateSyabariHiuchiBase(Document doc, XYZ pntS, XYZ pntE, ElementId levelID, string dan)
        //{
        //    return true;
        //}

        //public bool CreateSyabariHiuchiBase(Document doc, XYZ pntS, XYZ pntE)
        //{
        //    return true;
        //}

        public bool CreateSyabariHiuchiBase(Document doc, Autodesk.Revit.DB.Reference haraBaseRef, ElementId shaBaseId)
        {
            var rec = Master.ClsSyabariPieceCSV.Shared.FirstOrDefault(x => x.TypeName == m_PartsTypeHaraSide && x.SizeName == m_PartsSizeHaraSide);
            var haraPieceSymbolL = ClsSyabariBase.LoadSymbol(doc, rec.FamilyFullPath, rec.SymbolL);
            var haraPieceSymbolR = ClsSyabariBase.LoadSymbol(doc, rec.FamilyFullPath, rec.SymbolR);
            var shaPieceSymbol = ClsSyabariBase.LoadSymbol(doc, Master.ClsHiuchiCsv.GetFamilyPath(m_PartsSizeSyabariSide));
            // 斜梁火打ベースファミリの読込
            var symbolFolpath = ClsZumenInfo.GetYMSFolder();
            var shinfamily = System.IO.Path.Combine(symbolFolpath, "ベース関係\\" + baseName + ".rfa");
            if (!ClsRevitUtil.LoadFamilySymbolData(doc, shinfamily, baseName, out var hiuchiBaseSym))
            {
                return false;
            }

            var shaBaseLine = ClsYMSUtil.GetBaseLine(doc, shaBaseId);
            var shaBaseMid = SLibRevitReo.MidPoint(shaBaseLine);
            var shaBaseElem = doc.GetElement(shaBaseId) as FamilyInstance;
            var shaBasePlane = (shaBaseElem?.Host as ReferencePlane)?.GetPlane();
            var shaBaseLevelId = shaBaseElem.GetParameter(ParameterTypeId.InstanceScheduleOnlyLevelParam)?.AsElementId();

            var haraLine = ClsYMSUtil.GetBaseLine(doc, haraBaseRef.ElementId);
            var haraSectionPlane = Plane.CreateByNormalAndOrigin(haraLine.Direction.CrossProduct(XYZ.BasisZ), haraLine.Origin);
            var haraBaseLine = SLibRevitReo.IntersectPlaneAndPlane(shaBasePlane, haraSectionPlane);

            var p0 = SLibRevitReo.IntersectPlaneAndLine(haraSectionPlane, shaBaseLine);
            var p0toMid = (shaBaseMid - p0).Normalize();

            Plane shaSystem;
            {
                var v = p0toMid.CrossProduct(shaBasePlane.Normal);
                var dotProd = v.DotProduct(haraBaseLine.Direction);
                var bX = (dotProd * haraBaseLine.Direction).Normalize();
                var bY = shaBasePlane.Normal.CrossProduct(bX);

                shaSystem = Plane.CreateByOriginAndBasis(p0, bX, bY);
            }

            var ssTransform = SMatrix3d.BasisTransform(shaSystem);
            var ssInv = ssTransform.Inverse();
            var shaDirSS = ssTransform.TransformVector(p0toMid).Normalize();
            var alpha1 = ClsGeo.Deg2Rad(m_Angle);
            var hiuchiKozaiLength = ClsRevitUtil.CovertToAPI(m_HiuchiLengthL);

            //ここで斜梁側の火打受ピースの長さを求めておきたい
            double hiuchiPieceLength;
            {
                var shaBase = new ClsSyabariBase();
                shaBase.SetClassParameter(doc, shaBaseId);
                var shaWebThickMM = Master.ClsYamadomeCsv.GetWeb(shaBase.m_kouzaiSize);
                var shaWebThick = ClsRevitUtil.CovertToAPI(shaWebThickMM);
                var a3 = 0.5 * shaWebThick;
                //var a3_ = ClsRevitUtil.CovertFromAPI(a3);

                var pieceHiuchiSize = Master.ClsHiuchiCsv.GetTargetShuzai(m_PartsSizeSyabariSide);
                var pieceHiuchiWidthMM = Master.ClsYamadomeCsv.GetWidth(pieceHiuchiSize);
                var pieceHiuchiWidth = ClsRevitUtil.CovertToAPI(pieceHiuchiWidthMM);
                var a1 = 0.5 * pieceHiuchiWidth * Math.Cos(alpha1);
                //var a1_ = ClsRevitUtil.CovertFromAPI(a1);

                var pieceThickMM = Master.ClsHiuchiCsv.GetThickness(m_PartsSizeSyabariSide);
                var pieceThick = ClsRevitUtil.CovertToAPI(pieceThickMM);
                var a2 = pieceThick;
                //var a2_ = ClsRevitUtil.CovertFromAPI(a2);

                var a = a1 + a2 + a3;
                hiuchiPieceLength = a / Math.Sin(alpha1);
            }

            XYZ[] targetDirections;
            if (m_CreateType == CreateType.AnyLengthManual)
            {
                targetDirections = new XYZ[]
                {
                    haraBaseRef.GlobalPoint - p0
                };
            }
            else
            {
                targetDirections = new XYZ[]
                {
                    haraBaseLine.Direction,
                    -haraBaseLine.Direction
                };
            }

            var haraPiecedataL = ClsSyabariBase.GetPieceData(haraPieceSymbolL);
            var haraPiecedataR = ClsSyabariBase.GetPieceData(haraPieceSymbolR);
            var hiuchiBaseIds = new List<ElementId>();

            using (var transaction = new Transaction(doc, Guid.NewGuid().ToString()))
            {
                transaction.Start();

                foreach (var targetDirection in targetDirections)
                {
                    var targetDirSS = (SMatrix3d.ProjX * ssTransform).TransformVector(targetDirection).Normalize();
                    var axisZ = targetDirSS.CrossProduct(shaDirSS).Normalize();
                    var alpha0 = targetDirSS.AngleOnPlaneTo(shaDirSS, axisZ); // always positive
                    var alpha2 = Math.PI - alpha0 - alpha1; // always positive
                    var p2toP1SS = SMatrix3d.Rotation(-axisZ, XYZ.Zero, alpha2) * (-targetDirSS);
                    var p2toP1 = ssInv.TransformVector(p2toP1SS);

                    var isL = targetDirSS.DotProduct(XYZ.BasisX) > 0 ? false : true;

                    var pieceBasisX = SMatrix3d.ProjXY.TransformVector(shaSystem.XVec).Normalize();
                    var pieceBasisZ = XYZ.BasisZ;
                    var pieceBasisY = pieceBasisZ.CrossProduct(pieceBasisX);

                    //ここで斜梁ピースの theta2 を求めておきたい
                    var theta2 = SLibRevitReo.PrincipalRadian(pieceBasisY.AngleOnPlaneTo(p2toP1, -pieceBasisX));

                    var rotateTheta2 = SMatrix3d.Rotation(-pieceBasisX, XYZ.Zero, theta2);
                    var pieceBasisX2 = rotateTheta2.TransformVector(pieceBasisX);
                    var pieceBasisZ2 = rotateTheta2.TransformVector(pieceBasisZ);
                    var pieceBasisY2 = rotateTheta2.TransformVector(pieceBasisY);

                    // theta1 の算出
                    var theta1 = SLibRevitReo.PrincipalRadian(pieceBasisY2.AngleOnPlaneTo(p2toP1, -pieceBasisZ2));

                    var rotateTheta1 = SMatrix3d.Rotation(-pieceBasisZ2, XYZ.Zero, theta1);
                    var pieceBasisX1 = rotateTheta2.TransformVector(pieceBasisX2);
                    var pieceBasisZ1 = rotateTheta2.TransformVector(pieceBasisZ2);
                    var pieceBasisY1 = rotateTheta2.TransformVector(pieceBasisY2);

                    // theta0 の算出 
                    // 斜梁と腹起は直交しているなら常に 0.0 なので計算しない
                    var theta0 = 0.0;
                    //var theta0 = SLibRevitReo.PrincipalRadian(pieceBasisZ1.AngleOnPlaneTo(shaSystem.Normal, pieceBasisY1));

                    // ここで回転後の斜梁ピースの長さを求めておきたい
                    var shaWidthMM = Master.ClsYamadomeCsv.GetWidth(m_SteelSize);
                    var hiuchiBaseLine = Line.CreateUnbound(XYZ.Zero, ssInv.TransformVector(p2toP1SS));
                    var mortarLength = ClsSyabariBase.CalcMortarLength(haraSectionPlane, hiuchiBaseLine, shaBasePlane, shaWidthMM, shaWidthMM);
                    var haraPiecedata = isL ? haraPiecedataL : haraPiecedataR;
                    var pieceVertices = ClsSyabariBase.CalcPieceVertices(haraPiecedata, 0.0, theta2, theta1, 0.0, mortarLength);
                    double haraPieceLength;
                    XYZ e3;
                    {
                        var line0 = Line.CreateUnbound(pieceVertices.E0, pieceVertices.V1 - pieceVertices.V0);
                        var plane = Plane.CreateByNormalAndOrigin(pieceVertices.BasisY, pieceVertices.Origin);
                        e3 = SLibRevitReo.IntersectPlaneAndLine(plane, line0);
                        haraPieceLength = pieceVertices.E0.DistanceTo(e3);
                    }

                    var h0 = haraPieceLength + hiuchiKozaiLength + hiuchiPieceLength;
                    var sinAlpha0 = Math.Sin(alpha0);
                    var h1 = (h0 * Math.Sin(alpha1)) / sinAlpha0;
                    var h2 = (h0 * Math.Sin(alpha2)) / sinAlpha0;

                    var p2SS = h1 * targetDirSS;
                    var p1SS = h2 * shaDirSS;
                    var p0SS = XYZ.Zero;

                    var p2 = ssInv * p2SS;
                    var p1 = ssInv * p1SS;

                    var e3toP2 = p2 - e3;
                    var pieceRotate = SMatrix3d.Rotation(XYZ.BasisZ, p2, pieceVertices.BasisY.AngleOnPlaneTo(shaSystem.YVec, XYZ.BasisZ));
                    var pieceTransform = pieceRotate * SMatrix3d.Displacement(e3toP2);

                    var pieceOrigin = pieceTransform * pieceVertices.Origin;
                    var e0 = pieceTransform * pieceVertices.E0;

                    // 腹起側の斜梁ピースの配置
                    var pt = pieceOrigin;
                    var haraPiecePlane = ClsRevitUtil.CreateReferencePlane(doc, pt, pt + XYZ.BasisX, pt + XYZ.BasisY);
                    var haraPiecePlaneRef = haraPiecePlane.GetReference();
                    //var haraPieceDir = (SMatrix3d.ProjXY * shaSystem.XVec).Normalize();
                    var haraPieceSymbol = isL ? haraPieceSymbolL : haraPieceSymbolR;
                    var haraPieceInstance = doc.Create.NewFamilyInstance(haraPiecePlaneRef, pt, pieceBasisX, haraPieceSymbol);
                    haraPieceInstance.GetParameter(ParameterTypeId.InstanceScheduleOnlyLevelParam)?.Set(shaBaseLevelId); // 集計レベルの設定
                    ClsRevitUtil.SetParameter(doc, haraPieceInstance.Id, ClsSyabariBase.KeyTheta2, ClsYMSUtil.CheckSyabariAngle(theta2));
                    ClsRevitUtil.SetParameter(doc, haraPieceInstance.Id, ClsSyabariBase.KeyTheta1, ClsYMSUtil.CheckSyabariAngle(theta1));
                    ClsRevitUtil.SetParameter(doc, haraPieceInstance.Id, ClsSyabariBase.KeyTheta0, ClsYMSUtil.CheckSyabariAngle(theta0));
                    if (ClsRevitUtil.FindParameter(doc, haraPieceInstance.Id, ClsSyabariBase.KeyMortarLength))
                    {
                        ClsRevitUtil.SetParameter(doc, haraPieceInstance.Id, ClsSyabariBase.KeyMortarLength, mortarLength);
                    }
                    ClsKariKouzai.SetKariKouzaiFlag(doc, haraPieceInstance.Id);

                    // 斜梁側の火打受ピースの配置
                    var p3 = e0 + hiuchiKozaiLength * p2toP1;
                    var p1toP0SS = (p0SS - p1SS).Normalize();
                    var p1toP0 = ssInv.TransformVector(p1toP0SS);
                    var shaPiecePlane = ClsRevitUtil.CreateReferencePlane(doc, p3, p3 + targetDirection, p3 + shaSystem.YVec);
                    var shaPiecePlaneRef = shaPiecePlane.GetReference();
                    if (!shaPieceSymbol.IsActive) { shaPieceSymbol.Activate(); }
                    var shaPieceInstance = doc.Create.NewFamilyInstance(shaPiecePlaneRef, p3, p1toP0, shaPieceSymbol);
                    shaPieceInstance.GetParameter(ParameterTypeId.InstanceScheduleOnlyLevelParam)?.Set(shaBaseLevelId); // 集計レベルの設定
                    ClsKariKouzai.SetKariKouzaiFlag(doc, shaPieceInstance.Id);

                    // 斜梁火打ベースの配置
                    var hiuchiBasePlane = ClsRevitUtil.CreateReferencePlane(doc, shaSystem);
                    if (!hiuchiBaseSym.IsActive) { hiuchiBaseSym.Activate(); }
                    var hiuchiBaseInstance = doc.Create.NewFamilyInstance(hiuchiBasePlane.GetReference(), e0, (p3 - e0).Normalize(), hiuchiBaseSym);
                    ClsRevitUtil.SetParameter(doc, hiuchiBaseInstance.Id, ClsGlobal.m_length, e0.DistanceTo(p3));
                    var idHara = haraBaseRef.ElementId;
                    if (idHara == null)
                    {
                        continue;
                    }
                    SetHaraokoshiBaseShabariBaseIdString(doc, hiuchiBaseInstance.Id, idHara.ToString(), shaBaseId.ToString());

                    this.SetParameter(doc, hiuchiBaseInstance.Id);
                    hiuchiBaseInstance.GetParameter(ParameterTypeId.InstanceScheduleOnlyLevelParam)?.Set(shaBaseLevelId); // 集計レベルの設定
                    hiuchiBaseIds.Add(hiuchiBaseInstance.Id);
                }
                transaction.Commit();
            }

            //斜梁仮鋼材作成
            foreach (var hiuchiBaseId in hiuchiBaseIds)
            {
                ClsKariKouzai.CreateSingleKarikouzaiSyabari(doc, hiuchiBaseId, "斜梁", m_SteelSize);
            }

            return true;
        }

        /// <summary>
        /// 斜梁火打ち作成関数　仮鋼材オフ後に再度斜梁火打ちを作成する際に使用する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="haraBaseRef"></param>
        /// <param name="shaBaseId"></param>
        /// <returns></returns>
        public bool CreateSyabariHiuchiBaseForKarikouzai(Document doc, ElementId haraokoshiId,XYZ haraPnt, ElementId shaBaseId)
        {
            var rec = Master.ClsSyabariPieceCSV.Shared.FirstOrDefault(x => x.TypeName == m_PartsTypeHaraSide && x.SizeName == m_PartsSizeHaraSide);
            var haraPieceSymbolL = ClsSyabariBase.LoadSymbol(doc, rec.FamilyFullPath, rec.SymbolL);
            var haraPieceSymbolR = ClsSyabariBase.LoadSymbol(doc, rec.FamilyFullPath, rec.SymbolR);
            var shaPieceSymbol = ClsSyabariBase.LoadSymbol(doc, Master.ClsHiuchiCsv.GetFamilyPath(m_PartsSizeSyabariSide));
            // 斜梁火打ベースファミリの読込
            var symbolFolpath = ClsZumenInfo.GetYMSFolder();
            var shinfamily = System.IO.Path.Combine(symbolFolpath, "ベース関係\\" + baseName + ".rfa");
            if (!ClsRevitUtil.LoadFamilySymbolData(doc, shinfamily, baseName, out var hiuchiBaseSym))
            {
                return false;
            }

            var shaBaseLine = ClsYMSUtil.GetBaseLine(doc, shaBaseId);
            var shaBaseMid = SLibRevitReo.MidPoint(shaBaseLine);
            var shaBaseElem = doc.GetElement(shaBaseId) as FamilyInstance;
            var shaBasePlane = (shaBaseElem?.Host as ReferencePlane)?.GetPlane();
            var shaBaseLevelId = shaBaseElem.GetParameter(ParameterTypeId.InstanceScheduleOnlyLevelParam)?.AsElementId();

            var haraLine = ClsYMSUtil.GetBaseLine(doc, haraokoshiId);
            var haraSectionPlane = Plane.CreateByNormalAndOrigin(haraLine.Direction.CrossProduct(XYZ.BasisZ), haraLine.Origin);
            var haraBaseLine = SLibRevitReo.IntersectPlaneAndPlane(shaBasePlane, haraSectionPlane);

            var p0 = SLibRevitReo.IntersectPlaneAndLine(haraSectionPlane, shaBaseLine);
            var p0toMid = (shaBaseMid - p0).Normalize();

            Plane shaSystem;
            {
                var v = p0toMid.CrossProduct(shaBasePlane.Normal);
                var dotProd = v.DotProduct(haraBaseLine.Direction);
                var bX = (dotProd * haraBaseLine.Direction).Normalize();
                var bY = shaBasePlane.Normal.CrossProduct(bX);

                shaSystem = Plane.CreateByOriginAndBasis(p0, bX, bY);
            }

            var ssTransform = SMatrix3d.BasisTransform(shaSystem);
            var ssInv = ssTransform.Inverse();
            var shaDirSS = ssTransform.TransformVector(p0toMid).Normalize();
            var alpha1 = ClsGeo.Deg2Rad(m_Angle);
            var hiuchiKozaiLength = ClsRevitUtil.CovertToAPI(m_HiuchiLengthL);

            //ここで斜梁側の火打受ピースの長さを求めておきたい
            double hiuchiPieceLength;
            {
                var shaBase = new ClsSyabariBase();
                shaBase.SetClassParameter(doc, shaBaseId);
                var shaWebThickMM = Master.ClsYamadomeCsv.GetWeb(shaBase.m_kouzaiSize);
                var shaWebThick = ClsRevitUtil.CovertToAPI(shaWebThickMM);
                var a3 = 0.5 * shaWebThick;
                //var a3_ = ClsRevitUtil.CovertFromAPI(a3);

                var pieceHiuchiSize = Master.ClsHiuchiCsv.GetTargetShuzai(m_PartsSizeSyabariSide);
                var pieceHiuchiWidthMM = Master.ClsYamadomeCsv.GetWidth(pieceHiuchiSize);
                var pieceHiuchiWidth = ClsRevitUtil.CovertToAPI(pieceHiuchiWidthMM);
                var a1 = 0.5 * pieceHiuchiWidth * Math.Cos(alpha1);
                //var a1_ = ClsRevitUtil.CovertFromAPI(a1);

                var pieceThickMM = Master.ClsHiuchiCsv.GetThickness(m_PartsSizeSyabariSide);
                var pieceThick = ClsRevitUtil.CovertToAPI(pieceThickMM);
                var a2 = pieceThick;
                //var a2_ = ClsRevitUtil.CovertFromAPI(a2);

                var a = a1 + a2 + a3;
                hiuchiPieceLength = a / Math.Sin(alpha1);
            }

            XYZ[] targetDirections;
            //if (m_CreateType == CreateType.AnyLengthManual)
            //{
            //    targetDirections = new XYZ[]
            //    {
            //        haraPnt - p0
            //    };
            //}
            //else
            //{
            //    targetDirections = new XYZ[]
            //    {
            //        haraBaseLine.Direction,
            //        -haraBaseLine.Direction
            //    };
            //}
            targetDirections = new XYZ[]
                {
                    haraPnt - p0
                };

            var haraPiecedataL = ClsSyabariBase.GetPieceData(haraPieceSymbolL);
            var haraPiecedataR = ClsSyabariBase.GetPieceData(haraPieceSymbolR);
            var hiuchiBaseIds = new List<ElementId>();

            using (var transaction = new Transaction(doc, Guid.NewGuid().ToString()))
            {
                transaction.Start();
                FailureHandlingOptions failOpt = transaction.GetFailureHandlingOptions();
                failOpt.SetFailuresPreprocessor(new WarningSwallower());
                transaction.SetFailureHandlingOptions(failOpt);

                foreach (var targetDirection in targetDirections)
                {
                    var targetDirSS = (SMatrix3d.ProjX * ssTransform).TransformVector(targetDirection).Normalize();
                    var axisZ = targetDirSS.CrossProduct(shaDirSS).Normalize();
                    var alpha0 = targetDirSS.AngleOnPlaneTo(shaDirSS, axisZ); // always positive
                    var alpha2 = Math.PI - alpha0 - alpha1; // always positive
                    var p2toP1SS = SMatrix3d.Rotation(-axisZ, XYZ.Zero, alpha2) * (-targetDirSS);
                    var p2toP1 = ssInv.TransformVector(p2toP1SS);

                    var isL = targetDirSS.DotProduct(XYZ.BasisX) > 0 ? false : true;

                    var pieceBasisX = SMatrix3d.ProjXY.TransformVector(shaSystem.XVec).Normalize();
                    var pieceBasisZ = XYZ.BasisZ;
                    var pieceBasisY = pieceBasisZ.CrossProduct(pieceBasisX);

                    //ここで斜梁ピースの theta2 を求めておきたい
                    var theta2 = SLibRevitReo.PrincipalRadian(pieceBasisY.AngleOnPlaneTo(p2toP1, -pieceBasisX));

                    var rotateTheta2 = SMatrix3d.Rotation(-pieceBasisX, XYZ.Zero, theta2);
                    var pieceBasisX2 = rotateTheta2.TransformVector(pieceBasisX);
                    var pieceBasisZ2 = rotateTheta2.TransformVector(pieceBasisZ);
                    var pieceBasisY2 = rotateTheta2.TransformVector(pieceBasisY);

                    // theta1 の算出
                    var theta1 = SLibRevitReo.PrincipalRadian(pieceBasisY2.AngleOnPlaneTo(p2toP1, -pieceBasisZ2));

                    var rotateTheta1 = SMatrix3d.Rotation(-pieceBasisZ2, XYZ.Zero, theta1);
                    var pieceBasisX1 = rotateTheta2.TransformVector(pieceBasisX2);
                    var pieceBasisZ1 = rotateTheta2.TransformVector(pieceBasisZ2);
                    var pieceBasisY1 = rotateTheta2.TransformVector(pieceBasisY2);

                    // theta0 の算出 
                    // 斜梁と腹起は直交しているなら常に 0.0 なので計算しない
                    var theta0 = 0.0;
                    //var theta0 = SLibRevitReo.PrincipalRadian(pieceBasisZ1.AngleOnPlaneTo(shaSystem.Normal, pieceBasisY1));

                    // ここで回転後の斜梁ピースの長さを求めておきたい
                    var shaWidthMM = Master.ClsYamadomeCsv.GetWidth(m_SteelSize);
                    var hiuchiBaseLine = Line.CreateUnbound(XYZ.Zero, ssInv.TransformVector(p2toP1SS));
                    var mortarLength = ClsSyabariBase.CalcMortarLength(haraSectionPlane, hiuchiBaseLine, shaBasePlane, shaWidthMM, shaWidthMM);
                    var haraPiecedata = isL ? haraPiecedataL : haraPiecedataR;
                    var pieceVertices = ClsSyabariBase.CalcPieceVertices(haraPiecedata, 0.0, theta2, theta1, 0.0, mortarLength);
                    double haraPieceLength;
                    XYZ e3;
                    {
                        var line0 = Line.CreateUnbound(pieceVertices.E0, pieceVertices.V1 - pieceVertices.V0);
                        var plane = Plane.CreateByNormalAndOrigin(pieceVertices.BasisY, pieceVertices.Origin);
                        e3 = SLibRevitReo.IntersectPlaneAndLine(plane, line0);
                        haraPieceLength = pieceVertices.E0.DistanceTo(e3);
                    }

                    var h0 = haraPieceLength + hiuchiKozaiLength + hiuchiPieceLength;
                    var sinAlpha0 = Math.Sin(alpha0);
                    var h1 = (h0 * Math.Sin(alpha1)) / sinAlpha0;
                    var h2 = (h0 * Math.Sin(alpha2)) / sinAlpha0;

                    var p2SS = h1 * targetDirSS;
                    var p1SS = h2 * shaDirSS;
                    var p0SS = XYZ.Zero;

                    var p2 = ssInv * p2SS;
                    var p1 = ssInv * p1SS;

                    var e3toP2 = p2 - e3;
                    var pieceRotate = SMatrix3d.Rotation(XYZ.BasisZ, p2, pieceVertices.BasisY.AngleOnPlaneTo(shaSystem.YVec, XYZ.BasisZ));
                    var pieceTransform = pieceRotate * SMatrix3d.Displacement(e3toP2);

                    var pieceOrigin = pieceTransform * pieceVertices.Origin;
                    var e0 = pieceTransform * pieceVertices.E0;

                    // 腹起側の斜梁ピースの配置
                    var pt = pieceOrigin;
                    var haraPiecePlane = ClsRevitUtil.CreateReferencePlane(doc, pt, pt + XYZ.BasisX, pt + XYZ.BasisY);
                    var haraPiecePlaneRef = haraPiecePlane.GetReference();
                    //var haraPieceDir = (SMatrix3d.ProjXY * shaSystem.XVec).Normalize();
                    var haraPieceSymbol = isL ? haraPieceSymbolL : haraPieceSymbolR;
                    var haraPieceInstance = doc.Create.NewFamilyInstance(haraPiecePlaneRef, pt, pieceBasisX, haraPieceSymbol);
                    haraPieceInstance.GetParameter(ParameterTypeId.InstanceScheduleOnlyLevelParam)?.Set(shaBaseLevelId); // 集計レベルの設定
                    ClsRevitUtil.SetParameter(doc, haraPieceInstance.Id, ClsSyabariBase.KeyTheta2, ClsYMSUtil.CheckSyabariAngle(theta2));
                    ClsRevitUtil.SetParameter(doc, haraPieceInstance.Id, ClsSyabariBase.KeyTheta1, ClsYMSUtil.CheckSyabariAngle(theta1));
                    ClsRevitUtil.SetParameter(doc, haraPieceInstance.Id, ClsSyabariBase.KeyTheta0, ClsYMSUtil.CheckSyabariAngle(theta0));
                    if (ClsRevitUtil.FindParameter(doc, haraPieceInstance.Id, ClsSyabariBase.KeyMortarLength))
                    {
                        ClsRevitUtil.SetParameter(doc, haraPieceInstance.Id, ClsSyabariBase.KeyMortarLength, mortarLength);
                    }
                    ClsKariKouzai.SetKariKouzaiFlag(doc, haraPieceInstance.Id);

                    // 斜梁側の火打受ピースの配置
                    var p3 = e0 + hiuchiKozaiLength * p2toP1;
                    var p1toP0SS = (p0SS - p1SS).Normalize();
                    var p1toP0 = ssInv.TransformVector(p1toP0SS);
                    var shaPiecePlane = ClsRevitUtil.CreateReferencePlane(doc, p3, p3 + targetDirection, p3 + shaSystem.YVec);
                    var shaPiecePlaneRef = shaPiecePlane.GetReference();
                    if (!shaPieceSymbol.IsActive) { shaPieceSymbol.Activate(); }
                    var shaPieceInstance = doc.Create.NewFamilyInstance(shaPiecePlaneRef, p3, p1toP0, shaPieceSymbol);
                    shaPieceInstance.GetParameter(ParameterTypeId.InstanceScheduleOnlyLevelParam)?.Set(shaBaseLevelId); // 集計レベルの設定
                    ClsKariKouzai.SetKariKouzaiFlag(doc, shaPieceInstance.Id);

                    // 斜梁火打ベースの配置
                    var hiuchiBasePlane = ClsRevitUtil.CreateReferencePlane(doc, shaSystem);
                    if (!hiuchiBaseSym.IsActive) { hiuchiBaseSym.Activate(); }
                    var hiuchiBaseInstance = doc.Create.NewFamilyInstance(hiuchiBasePlane.GetReference(), e0, (p3 - e0).Normalize(), hiuchiBaseSym);
                    ClsRevitUtil.SetParameter(doc, hiuchiBaseInstance.Id, ClsGlobal.m_length, e0.DistanceTo(p3));
                    var idHara = haraokoshiId;
                    if (idHara == null)
                    {
                        continue;
                    }
                    SetHaraokoshiBaseShabariBaseIdString(doc, hiuchiBaseInstance.Id, idHara.ToString(), shaBaseId.ToString());

                    this.SetParameter(doc, hiuchiBaseInstance.Id);
                    hiuchiBaseInstance.GetParameter(ParameterTypeId.InstanceScheduleOnlyLevelParam)?.Set(shaBaseLevelId); // 集計レベルの設定
                    hiuchiBaseIds.Add(hiuchiBaseInstance.Id);
                }
                transaction.Commit();
            }

            //斜梁仮鋼材作成
            foreach (var hiuchiBaseId in hiuchiBaseIds)
            {
                ClsKariKouzai.CreateSingleKarikouzaiSyabari(doc, hiuchiBaseId, "斜梁", m_SteelSize);
            }

            return true;
        }

        public static bool SetHaraokoshiBaseShabariBaseIdString(Document doc, ElementId hiuchiId, string haraokoshiId, string shabariId)
        {
            try
            {
             bool res =   ClsRevitUtil.CustomDataSet<string>(doc, hiuchiId ,"hB", haraokoshiId);
             bool res2 =   ClsRevitUtil.CustomDataSet<string>(doc, hiuchiId, "sB", shabariId);
            }
            catch (Exception)
            {

                return false;
            }
            return true;
        }

        public static bool GetHaraokoshiBaseIdString(Document doc, ElementId hiuchiId,ref string haraIdstring)
        {
            try
            {
                if (!ClsRevitUtil.CustomDataGet<string>(doc, hiuchiId, "hB", out haraIdstring))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static bool GetShabariBaseIdString(Document doc, ElementId hiuchiId, ref string shabariIdstring)
        {
            try
            {
                if (!ClsRevitUtil.CustomDataGet<string>(doc, hiuchiId, "sB", out shabariIdstring))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 図面上の斜梁火打ベースを全て取得する
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
        /// 斜梁ベース のみを複数選択
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="ids"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool PickBaseObjects(UIDocument uidoc, ref List<ElementId> ids, string message = baseName)
        {
            return ClsRevitUtil.PickObjectsPartFilter(uidoc, message + "を選択してください", baseName, ref ids);
        }

        #endregion
    }
}
