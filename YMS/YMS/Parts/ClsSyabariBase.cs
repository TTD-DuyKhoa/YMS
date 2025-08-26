using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.UI;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace YMS.Parts
{
    public class ClsSyabariBase
    {
        #region 定数
        public const string baseName = "斜梁ベース";
        const string FirstPointS = "最初の始点";
        const string FirstPointE = "最初の終点";
        const string FirstReferencePlane = "最初の参照面";

        /// <summary>
        /// 処理タイプ
        /// </summary>
        public enum ShoriType
        {
            BaseLine,
            PtoP
        }
        #endregion
        #region プロパティ
        /// <summary>
        /// 鋼材タイプ
        /// </summary>
        public string m_kouzaiType { get; set; }
        /// <summary>
        /// 鋼材サイズ
        /// </summary>
        public string m_kouzaiSize { get; set; }
        /// <summary>
        /// 処理方法
        /// </summary>
        public ShoriType m_ShoriType { get; set; }
        /// <summary>
        /// 終端側オフセット（－）
        /// </summary>
        public double m_endOffset1 { get; set; }
        /// <summary>
        /// 部品タイプ(始点側)
        /// </summary>
        public string m_buhinTypeStart { get; set; }
        /// <summary>
        /// 部品サイズ(始点側)
        /// </summary>
        public string m_buhinSizeStart { get; set; }
        /// <summary>
        /// 部品タイプ(終点側)
        /// </summary>
        public string m_buhinTypeEnd { get; set; }
        /// <summary>
        /// 部品サイズ(終点側)
        /// </summary>
        public string m_buhinSizeEnd { get; set; }
        /// <summary>
        /// ジャッキタイプ1
        /// </summary>
        public string m_jack1 { get; set; }
        /// <summary>
        /// ジャッキタイプ2
        /// </summary>
        public string m_jack2 { get; set; }
        /// <summary>
        /// 編集用：フロア
        /// </summary>
        public string m_Floor { get; set; }
        /// <summary>
        /// 編集用：レベルID
        /// </summary>
        public ElementId m_LevelId { get; set; }

        /// <summary>
        /// 編集用：エレメントID
        /// </summary>
        public ElementId m_ElementId { get; set; }
        /// <summary>
        /// 編集用：参照面
        /// </summary>
        public ReferencePlane m_ReferencePlanee { get; set; }

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
        public ClsSyabariBase()
        {
            //初期化
            Init();
        }
        #endregion
        #region メソッド
        public void Init()
        {
            m_kouzaiType = string.Empty;
            m_kouzaiSize = string.Empty;
            //m_bTwin = false;
            //m_bSMH = false;
            m_ShoriType = ShoriType.BaseLine;
            m_endOffset1 = 0.0;
            m_buhinTypeStart = string.Empty;
            m_buhinSizeStart = string.Empty;
            m_buhinTypeEnd = string.Empty;
            m_buhinSizeEnd = string.Empty;
            m_jack1 = string.Empty;
            m_jack2 = string.Empty;
            m_Floor = string.Empty;
            m_Dan = string.Empty;
            m_FlgEdit = false;
            m_LevelId = null;
            m_ElementId = null;
            m_ReferencePlanee = null;
        }

        public bool CreateSyabariBase(Document doc, XYZ tmpStPoint, XYZ tmpEdPoint, ElementId levelId)
        {
            //参照面の作成
            ReferencePlane slopePlane = null;
            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                t.Start();
                FailureHandlingOptions failOpt = t.GetFailureHandlingOptions();
                failOpt.SetFailuresPreprocessor(new WarningSwallower());
                t.SetFailureHandlingOptions(failOpt);
                tmpEdPoint = new XYZ(tmpEdPoint.X, tmpEdPoint.Y, tmpEdPoint.Z - ClsRevitUtil.CovertToAPI(m_endOffset1));
                slopePlane = ClsRevitUtil.CreateReferencePlaneW(doc, tmpStPoint, tmpEdPoint, ClsRevitUtil.CovertToAPI(m_endOffset1));
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
                        ClsRevitUtil.SetParameter(doc, createId, "長さ", tmpStPoint.DistanceTo(tmpEdPoint));
                        ClsRevitUtil.SetParameter(doc, createId, "集計レベル", levelId);
                        ClsRevitUtil.SetParameter(doc, createId, "基準レベルからの高さ", 0.0);//Z軸を求めるかもオフセット0にするかは暫定的
                        SetParameter(doc, createId);

                        var id = instance.Id;
                        var element = doc.GetElement(reference);
                        // ここで一回移動などをしないと Location が原点のまま返ってくる。
                        ElementTransformUtils.MoveElement(doc, id, XYZ.Zero);
                        slopePlane.Name = "斜梁" + id.IntegerValue.ToString();
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
        public bool CreateSyabariBase(Document doc, XYZ tmpStPoint, XYZ tmpEdPoint, ReferencePlane referencePlane, ElementId levelId)
        {
            //参照面の作成
            ReferencePlane slopePlane = referencePlane;
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
                        if (levelId != null)
                            ClsRevitUtil.SetParameter(doc, createId, "集計レベル", levelId);
                        ClsRevitUtil.SetParameter(doc, createId, "基準レベルからの高さ", 0.0);//Z軸を求めるかもオフセット0にするかは暫定的
                        SetParameter(doc, createId);

                        var id = instance.Id;
                        var element = doc.GetElement(reference);
                        // ここで一回移動などをしないと Location が原点のまま返ってくる。
                        ElementTransformUtils.MoveElement(doc, id, XYZ.Zero);
                        //slopePlane.Name = "斜梁" + id.IntegerValue.ToString();
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

        public static string KeyCenter1 => "回転軸1";
        public static string KeyCenter2 => "回転軸2";
        public static string KeyEdge1 => "仮鋼材端点";
        public static string KeyEdge2 => "仮鋼材端点2";
        public static string KeyTheta0 => "θ0";
        public static string KeyTheta1 => "θ1";
        public static string KeyTheta2 => "θ2";
        public static string KeyTheta3 => "設置角度";
        public static string KeyMortarLength => "MortarLength";

        public class PieceVertices
        {
            public XYZ Origin { get; set; }
            public XYZ BasisX { get; set; }
            public XYZ BasisY { get; set; }
            public XYZ V3 { get; set; }
            public XYZ V2 { get; set; }
            public XYZ V1 { get; set; }
            public XYZ V0 { get; set; }
            public XYZ E0 { get; set; }
        }

        public static PieceVertices CalcPieceVertices(PieceData data, double theta3, double theta2, double theta1, double theta0, double? mortarLength = null)
        {
            //var doc = symbol.Document;
            //var data = ClsSyabariBase.GetPieceData(symbol);
            //var hasMortarLength = ClsRevitUtil.FindParameter(symbol, KeyMortarLength);
            var hasMortarLength = data.HasMortarLength;


            //PieceData data;
            ////FamilyInstance instance;
            //bool hasMortarLength;
            //using (var transaction = new Transaction(doc, Guid.NewGuid().ToString()))
            //{
            //    transaction.Start();

            //    if (!symbol.IsActive) { symbol.Activate(); }
            //    var plane = ClsRevitUtil.CreateReferencePlane(doc, XYZ.Zero, XYZ.BasisX, XYZ.BasisY);
            //    instance = doc.Create.NewFamilyInstance(plane.GetReference(), XYZ.Zero, XYZ.BasisX, symbol);

            //    hasMortarLength = ClsRevitUtil.FindParameter(doc, instance.Id, KeyMortarLength);

            //    transaction.Commit();
            //}

            //data = ClsSyabariBase.GetPieceData(instance);
            //using (var transaction = new Transaction(doc, Guid.NewGuid().ToString()))
            //{
            //    transaction.Start();
            //    ClsRevitUtil.Delete(doc, instance.Id);
            //    transaction.Commit();
            //}

            var l0 = hasMortarLength && mortarLength.HasValue ? mortarLength.Value : data.L0;

            var v3 = data.Origin - data.V3toO;
            var v2 = v3 + data.L2 * data.BasisY;
            var v1 = v2 + data.L1 * data.BasisY;
            var v0 = v1 + l0 * data.BasisY;
            var e0 = v0 + data.V3toO;

            var theta3T = SMatrix3d.Rotation(data.BasisY, data.Origin, theta3);// 回転の向き

            var v3_3 = theta3T * v3;
            var v2_3 = theta3T * v2;
            var v1_3 = theta3T * v1;
            var v0_3 = theta3T * v0;
            var e0_3 = theta3T * e0;
            var basisX_3 = theta3T.TransformVector(data.BasisX);
            var basisY_3 = theta3T.TransformVector(data.BasisY);
            var basisZ_3 = theta3T.TransformVector(data.BasisZ);

            var theta2T = SMatrix3d.Rotation(-basisX_3, v2_3, theta2);// 回転の向き

            var v1_2 = theta2T * v1_3;
            var v0_2 = theta2T * v0_3;
            var e0_2 = theta2T * e0_3;
            var basisX_2 = theta2T.TransformVector(basisX_3);
            var basisY_2 = theta2T.TransformVector(basisY_3);
            var basisZ_2 = theta2T.TransformVector(basisZ_3);

            var theta1T = SMatrix3d.Rotation(-basisZ_2, v1_2, theta1);// 回転の向き

            var v0_1 = theta1T * v0_2;
            var e0_1 = theta1T * e0_2;
            var basisX_1 = theta1T.TransformVector(basisX_2);
            var basisY_1 = theta1T.TransformVector(basisY_2);
            var basisZ_1 = theta1T.TransformVector(basisZ_2);

            return new PieceVertices
            {
                Origin = data.Origin,
                BasisX = data.BasisX,
                BasisY = data.BasisY,
                V3 = v3_3,
                V2 = v2_3,
                V1 = v1_2,
                V0 = v0_1,
                E0 = e0_1,
            };
        }

        public class PieceData
        {
            public double L3 { get; set; }
            public double L2 { get; set; }
            public double L1 { get; set; }
            public double L0 { get; set; }
            public XYZ V3toO { get; set; }
            public XYZ Origin { get; set; }
            public XYZ BasisX { get; set; }
            public XYZ BasisY { get; set; }
            public XYZ BasisZ { get; set; }
            public bool HasMortarLength { get; set; }
        }

        public static PieceData GetPieceData(FamilySymbol symbol)
        {
            var doc = symbol.Document;
            FamilyInstance instance;
            bool hasMortarLength;
            using (var transaction = new Transaction(doc, Guid.NewGuid().ToString()))
            {
                transaction.Start();

                if (!symbol.IsActive) { symbol.Activate(); }
                var plane = ClsRevitUtil.CreateReferencePlane(doc, XYZ.Zero, XYZ.BasisX, XYZ.BasisY);
                instance = doc.Create.NewFamilyInstance(plane.GetReference(), XYZ.Zero, XYZ.BasisX, symbol);

                hasMortarLength = ClsRevitUtil.FindParameter(doc, instance.Id, KeyMortarLength);

                transaction.Commit();
            }

            var data = ClsSyabariBase.GetPieceData(instance);
            using (var transaction = new Transaction(doc, Guid.NewGuid().ToString()))
            {
                transaction.Start();
                ClsRevitUtil.Delete(doc, instance.Id);
                transaction.Commit();
            }
            return data;
        }

        public static PieceData GetPieceData(FamilyInstance pickedInstance)
        {
            var doc = pickedInstance.Document;
            var sym = pickedInstance.Symbol;
            var instanceCoodinate = pickedInstance.GetTotalTransform();
            var origin = instanceCoodinate.Origin;
            var basisY = instanceCoodinate.BasisY;

            var hasTheta3 = ClsRevitUtil.FindParameter(doc, pickedInstance.Id, KeyTheta3);
            var theta3 = hasTheta3 ? ClsRevitUtil.GetParameterDouble(doc, pickedInstance.Id, KeyTheta3) : 0.0;
            var basisRotation = SMatrix3d.Rotation(-basisY, XYZ.Zero, theta3);

            var basisX = basisRotation * instanceCoodinate.BasisX;
            var basisZ = basisRotation * instanceCoodinate.BasisZ;

            var center1Set = ClsRevitUtil.GetPointSyabariTanten(doc, pickedInstance, KeyCenter1);
            var center2Set = ClsRevitUtil.GetPointSyabariTanten(doc, pickedInstance, KeyCenter2);

            var toZXplane = SMatrix3d.BasisTransform(Plane.CreateByOriginAndBasis(origin, basisZ, basisX));
            var fromZXplane = toZXplane.Inverse();
            var proj = fromZXplane * SMatrix3d.ProjXY * toZXplane;
            var v3 = proj * center2Set;

            var aaaa = center2Set - origin;

            var edge1Set = ClsRevitUtil.GetPointSyabariTanten(doc, pickedInstance, KeyEdge1);
            var edge2Set = ClsRevitUtil.GetPointSyabariTanten(doc, pickedInstance, KeyEdge2);

#if DEBUG
            var length3 = ClsRevitUtil.CovertFromAPI(origin.DistanceTo(v3));
            var length2 = ClsRevitUtil.CovertFromAPI(v3.DistanceTo(center2Set));
            var length1 = ClsRevitUtil.CovertFromAPI(center2Set.DistanceTo(center1Set));
            var length0 = ClsRevitUtil.CovertFromAPI(center1Set.DistanceTo(edge1Set));
            var w = ClsRevitUtil.CovertFromAPI(edge1Set.DistanceTo(edge2Set));
#endif
            var l3 = origin.DistanceTo(v3);
            var l2 = v3.DistanceTo(center2Set);
            var l1Set = center2Set.DistanceTo(center1Set);
            var l0Set = center1Set.DistanceTo(edge1Set);

            var hasMortarLength = ClsRevitUtil.FindParameter(doc, pickedInstance.Id, KeyMortarLength);

            return new PieceData
            {
                L3 = l3,
                L2 = l2,
                L1 = l1Set,
                L0 = l0Set,
                V3toO = origin - v3,
                Origin = origin,
                BasisX = basisX,
                BasisY = basisY,
                BasisZ = basisZ,
                HasMortarLength = hasMortarLength,
            };

            //v3 = v3;
            //v2 = v3 + l2 * basisY;
            //v1 = v2 + l1Set * basisY;
            //v0 = v1 + l0Set * basisY;
        }
        //        private static PieceData GetPieceData(FamilySymbol pickedInstance)
        //        {
        //            var doc = pickedInstance.Document;
        //            //var instanceCoodinate = pickedInstance.GetTotalTransform();
        //            var origin = XYZ.Zero;// instanceCoodinate.Origin;
        //            var basisY = XYZ.BasisY;// instanceCoodinate.BasisY;

        //            var hasTheta3 = ClsRevitUtil.FindParameter(doc, pickedInstance.Id, KeyTheta3);
        //            var theta3 = hasTheta3 ? ClsRevitUtil.GetParameterDouble(doc, pickedInstance.Id, KeyTheta3) : 0.0;
        //            var basisRotation = SMatrix3d.Rotation(-basisY, XYZ.Zero, theta3);

        //            var basisX = basisRotation * XYZ.BasisX;// instanceCoodinate.BasisX;
        //            var basisZ = basisRotation * XYZ.BasisZ;// instanceCoodinate.BasisZ;

        //            var center1Set = ClsRevitUtil.GetPointSyabariSymbolTanten(doc, pickedInstance, KeyCenter1);
        //            var center2Set = ClsRevitUtil.GetPointSyabariSymbolTanten(doc, pickedInstance, KeyCenter2);

        //            var toZXplane = SMatrix3d.BasisTransform(Plane.CreateByOriginAndBasis(origin, basisZ, basisX));
        //            var fromZXplane = toZXplane.Inverse();
        //            var proj = fromZXplane * SMatrix3d.ProjXY * toZXplane;
        //            var v3 = proj * center2Set;

        //            var aaaa = center2Set - origin;

        //            var edge1Set = ClsRevitUtil.GetPointSyabariSymbolTanten(doc, pickedInstance, KeyEdge1);
        //            var edge2Set = ClsRevitUtil.GetPointSyabariSymbolTanten(doc, pickedInstance, KeyEdge2);

        //#if DEBUG
        //            var length3 = ClsRevitUtil.CovertFromAPI(origin.DistanceTo(v3));
        //            var length2 = ClsRevitUtil.CovertFromAPI(v3.DistanceTo(center2Set));
        //            var length1 = ClsRevitUtil.CovertFromAPI(center2Set.DistanceTo(center1Set));
        //            var length0 = ClsRevitUtil.CovertFromAPI(center1Set.DistanceTo(edge1Set));
        //            var w = ClsRevitUtil.CovertFromAPI(edge1Set.DistanceTo(edge2Set));
        //#endif
        //            var l3 = origin.DistanceTo(v3);
        //            var l2 = v3.DistanceTo(center2Set);
        //            var l1Set = center2Set.DistanceTo(center1Set);
        //            var l0Set = center1Set.DistanceTo(edge1Set);

        //            return new PieceData
        //            {
        //                L3 = l3,
        //                L2 = l2,
        //                L1 = l1Set,
        //                L0 = l0Set,
        //                V3toO = origin - v3,
        //                Origin = origin,
        //                BasisX = basisX,
        //                BasisY = basisY,
        //                BasisZ = basisZ,
        //            };

        //            //v3 = v3;
        //            //v2 = v3 + l2 * basisY;
        //            //v1 = v2 + l1Set * basisY;
        //            //v0 = v1 + l0Set * basisY;
        //        }

        public static double CalcMortarLength(Plane sectionPlane, Line shaBaseLine, /*XYZ kozaiPt, XYZ kozaiOppositePt,*/ Plane shaBasePlane, double shaW, double shaH)
        {
            //var sectionPlane = Plane.CreateByNormalAndOrigin(basisY[i], origin[i]);
            var shaBaseZ = shaBaseLine.Direction.Normalize();// (kozaiOppositePt - kozaiPt).Normalize();
            //var shaBaseLine = Line.CreateUnbound(kozaiPt, shaBaseZ);
            var ptsOnHaraWall = ClsYMSUtil.CalcShabariSection(shaBaseLine, shaBasePlane, shaW, shaH, sectionPlane);

            var shaBaseOrigin = SLibRevitReo.IntersectPlaneAndLine(sectionPlane, shaBaseLine);
            var shaBaseSystem = Plane.CreateByNormalAndOrigin(shaBaseZ, shaBaseOrigin);
            var toShaBaseSystem = SMatrix3d.BasisTransform(shaBaseSystem);
            var toZ = SMatrix3d.ProjZ;
            var transform = toZ * toShaBaseSystem;

            var aa = ptsOnHaraWall.Select(x => transform * x).ToArray();
            //var touchPt = ptsOnHaraWall.Select(x => transform * x).OrderByDescending(x => x.Z).FirstOrDefault();
            //var length = touchPt.Z;

            var length = ptsOnHaraWall.Select(x => transform * x).Select(x => Math.Abs(x.Z)).Max();//.FirstOrDefault();

            return length;
        }

        public ElementId CreateShabariBase(Document doc, ElementId piece1, ElementId piece2, ClsSyabariBase original)
        {

            ElementId resultId = null;
            var keyCenter1 = KeyCenter1;
            var keyCenter2 = KeyCenter2;
            var keyEdge1 = KeyEdge1;
            var keyEdge2 = KeyEdge2;
            var keyTheta0 = KeyTheta0;
            var keyTheta1 = KeyTheta1;
            var keyTheta2 = KeyTheta2;
            var keyTheta3 = KeyTheta3;
            var keyMortarLength = KeyMortarLength;

            var num = 2;
            var pieceIds = new ElementId[]
            {
                    piece1, piece2
            };
            var pieceInstances = new FamilyInstance[num];
            var origin = new XYZ[num];//ファミリ基点
            var basisX = new XYZ[num];//ファミリ X基底
            var basisY = new XYZ[num];//ファミリ Y基底
            var basisZ = new XYZ[num];//ファミリ Z基底
            var center1Set = new XYZ[num];// 回転軸1 の座標
            var center2Set = new XYZ[num];// 回転軸2 の座標
            var edge1Set = new XYZ[num];// 仮鋼材端点 の座標
            var edge2Set = new XYZ[num];// 仮鋼材端点2 の座標
            var l3 = new double[num];// ファミリ基点から V3 への距離
            var l2 = new double[num];// V3 から回転軸2への距離
            var l1Set = new double[num];// 回転軸2から回転軸1への距離
            var l0Set = new double[num];// 回転軸1から仮鋼材端点への距離
            var v3 = new XYZ[num];//V3 : V2をZX平面へ射影した点
            var v2 = new XYZ[num];
            var v1 = new XYZ[num];
            var v0 = new XYZ[num];
            var e0 = new XYZ[num];
            //var v3ToOrigin = new XYZ[num];

            for (int i = 0; i < num; i++)
            {
                var pickedElem = doc.GetElement(pieceIds[i]);
                var pickedInstance = pickedElem as FamilyInstance;
                pieceInstances[i] = pickedInstance;

                var instanceCoodinate = pickedInstance.GetTotalTransform();
                origin[i] = instanceCoodinate.Origin;
                basisY[i] = instanceCoodinate.BasisY;

                var hasTheta3 = ClsRevitUtil.FindParameter(doc, pickedInstance.Id, keyTheta3);
                var theta3 = hasTheta3 ? ClsRevitUtil.GetParameterDouble(doc, pickedInstance.Id, keyTheta3) : 0.0;
                var basisRotation = SMatrix3d.Rotation(-basisY[i], XYZ.Zero, theta3);

                basisX[i] = basisRotation * instanceCoodinate.BasisX;
                basisZ[i] = basisRotation * instanceCoodinate.BasisZ;

                center1Set[i] = ClsRevitUtil.GetPointSyabariTanten(doc, pickedInstance, keyCenter1);
                center2Set[i] = ClsRevitUtil.GetPointSyabariTanten(doc, pickedInstance, keyCenter2);

                var toZXplane = SMatrix3d.BasisTransform(Plane.CreateByOriginAndBasis(origin[i], basisZ[i], basisX[i]));
                var fromZXplane = toZXplane.Inverse();
                var proj = fromZXplane * SMatrix3d.ProjXY * toZXplane;
                v3[i] = proj * center2Set[i];

                var aaaa = center2Set[i] - origin[i];

                edge1Set[i] = ClsRevitUtil.GetPointSyabariTanten(doc, pickedInstance, keyEdge1);
                edge2Set[i] = ClsRevitUtil.GetPointSyabariTanten(doc, pickedInstance, keyEdge2);

#if DEBUG
                var length3 = ClsRevitUtil.CovertFromAPI(origin[i].DistanceTo(v3[i]));
                var length2 = ClsRevitUtil.CovertFromAPI(v3[i].DistanceTo(center2Set[i]));
                var length1 = ClsRevitUtil.CovertFromAPI(center2Set[i].DistanceTo(center1Set[i]));
                var length0 = ClsRevitUtil.CovertFromAPI(center1Set[i].DistanceTo(edge1Set[i]));
                var w = ClsRevitUtil.CovertFromAPI(edge1Set[i].DistanceTo(edge2Set[i]));
#endif
                l3[i] = origin[i].DistanceTo(v3[i]);
                l2[i] = v3[i].DistanceTo(center2Set[i]);
                l1Set[i] = center2Set[i].DistanceTo(center1Set[i]);
                l0Set[i] = center1Set[i].DistanceTo(edge1Set[i]);

                //v3[i] = v3;
                v2[i] = v3[i] + l2[i] * basisY[i];
                v1[i] = v2[i] + l1Set[i] * basisY[i];
                v0[i] = v1[i] + l0Set[i] * basisY[i];

                var edgeDist = edge1Set[i].DistanceTo(edge2Set[i]);
                e0[i] = v0[i] + /*l3Set[i] * */(origin[i] - v3[i]);// - edgeDist * basisX[i];
            }

            // theta2 の算出
            var theta2Set = new double[num];
            var rotateTheta2 = new SMatrix3d[num];
            var v1prime = new XYZ[num];
            var basisYprime = new XYZ[num];
            var basisZprime = new XYZ[num];
            for (int i = 0; i < num; i++)
            {
                var buddy = (i + 1) % num;

                var pt = v2[i];// originSet[i] + l2Set[i] * basisYSet[i];
                var buddyPt = v2[buddy];// originSet[buddy] + l2Set[buddy] * basisYSet[buddy];

                var yzPlane = Plane.CreateByOriginAndBasis(XYZ.Zero, basisY[i], basisZ[i]);
                //var yzPlane = Plane.CreateByNormalAndOrigin(-basisX[i], XYZ.Zero);
                var projection = SMatrix3d.ProjXY * SMatrix3d.BasisTransform(yzPlane);

                var p1 = projection * pt;
                var p2 = projection * buddyPt;

                var originToV3_1 = projection * (v3[i] - origin[i]);
                var originToV3_2 = projection * (v3[buddy] - origin[buddy]);

                var h = Math.Abs(p1.Y - p2.Y);
                var l = Math.Abs(p1.X - p2.X);
                var w1 = -originToV3_1.Y;// l3[i];
                var w2 = originToV3_2.Y;//l3[buddy];
                var a = w1 + w2;

                //var ans = UtilAlgebra.SolveQuadraticEquation(
                //    h * h + l * l,
                //    2.0 * a * h,
                //    a * a - l * l);

                var cos = UtilAlgebra.SolveQuadraticEquation(
                    h * h + l * l,
                    2.0 * a * h,
                    a * a - l * l)
                    .Where(x => ClsGeo.GEO_EQ(x.Imaginary, 0.0))
                    .Select(x => x.Real)
                    .FirstOrDefault(x => ClsGeo.GEO_LE(0, x) || ClsGeo.GEO_LE(x, 1.0));

                var theta_ = Math.Acos(cos);
                var theta0 = p2.Y < p1.Y ? theta_ : -theta_;

#if DEBUG
                var vec2 = projection * (buddyPt - pt);
                var vec1 = projection * basisY[i];

                var theta1 = vec1.AngleOnPlaneTo(vec2, XYZ.BasisZ);
                var diff = SLibRevitReo.PrincipalRadian(theta1) - SLibRevitReo.PrincipalRadian(theta0);
#endif

                var theta = theta0;
                theta2Set[i] = SLibRevitReo.PrincipalRadian(theta);
                rotateTheta2[i] = SMatrix3d.Rotation(-basisX[i], v2[i], theta2Set[i]);
                v1prime[i] = rotateTheta2[i] * v1[i];
                var rotateBasis = SMatrix3d.Rotation(-basisX[i], XYZ.Zero, theta2Set[i]);
                basisYprime[i] = rotateBasis * basisY[i];
                basisZprime[i] = rotateBasis * basisZ[i];
            }

            // theta1 の算出
            var theta1Set = new double[num];
            var rotateTheta1 = new SMatrix3d[num];
            var v0prime = new XYZ[num];
            var e0prime = new XYZ[num];
            var basisXprime = new XYZ[num];
            var basisYprime2 = new XYZ[num];
            for (int i = 0; i < num; i++)
            {
                var buddy = (i + 1) % num;

                var pt = v1prime[i];
                var buddyPt = v1prime[buddy];

                var xyPrimePlane = Plane.CreateByNormalAndOrigin(-basisZprime[i], XYZ.Zero);
                var projection = SMatrix3d.ProjXY * SMatrix3d.BasisTransform(xyPrimePlane);

                var vec2 = projection * (buddyPt - pt);
                var vec1 = projection * basisYprime[i];
                var theta = vec1.AngleOnPlaneTo(vec2, XYZ.BasisZ);
                theta1Set[i] = SLibRevitReo.PrincipalRadian(theta);
                rotateTheta1[i] = SMatrix3d.Rotation(-basisZprime[i], pt, theta1Set[i]) * rotateTheta2[i];
                v0prime[i] = rotateTheta1[i] * v0[i];
                e0prime[i] = rotateTheta1[i] * e0[i];
                var rotateBasis = SMatrix3d.Rotation(-basisZprime[i], XYZ.Zero, theta1Set[i]);
                basisYprime2[i] = rotateBasis * basisYprime[i];
                basisXprime[i] = rotateBasis * basisX[i];
            }

            // theta0 の算出
            var theta0Set = new double[num];
            var rotateTheta0 = new SMatrix3d[num];
            var basisXprime2 = new XYZ[num];
            for (int i = 0; i < num; i++)
            {
                var a = basisZprime[i];
                var b = basisZ[i];
                var radian = SLibRevitReo.PrincipalRadian(a.AngleOnPlaneTo(b, -basisYprime2[i]));

                // theta0 パラメータが存在しないときは 0 として計算を進める
                var existsTheta0 = ClsRevitUtil.FindParameter(doc, pieceInstances[i].Id, keyTheta0);
                theta0Set[i] = existsTheta0 ? radian : 0.0;

                rotateTheta0[i] = SMatrix3d.Rotation(-basisYprime2[i], e0prime[i], theta0Set[i]) * rotateTheta1[i];
                basisXprime2[i] = SMatrix3d.Rotation(-basisYprime2[i], XYZ.Zero, theta0Set[i]) * basisXprime[i];
            }

            // MortarLength の算出
            var mortarLength = new double?[num];
            var shaBaseLine = Line.CreateBound(e0prime[0], e0prime[1]);
            var shaBasePlane = Plane.CreateByThreePoints(e0prime[0], e0prime[1], e0prime[0] - basisXprime2[0]);
            var shaW = Master.ClsYamadomeCsv.GetWidth(original.m_kouzaiSize);
            var shaH = shaW;
            for (int i = 0; i < num; i++)
            {
                var buddy = (i + 1) % num;
                var existsMortar = ClsRevitUtil.FindParameter(doc, pieceInstances[i].Id, keyMortarLength);
                if (existsMortar)
                {
                    // MortarLength が存在するファミリのみ算出

                    var sectionPlane = Plane.CreateByNormalAndOrigin(basisY[i], origin[i]);
                    var ptsOnHaraWall = ClsYMSUtil.CalcShabariSection(shaBaseLine, shaBasePlane, shaW, shaH, sectionPlane);

                    var shaBaseOrigin = SLibRevitReo.IntersectPlaneAndLine(sectionPlane, shaBaseLine);
                    var shaBaseZ = (e0prime[buddy] - e0prime[i]).Normalize();
                    var shaBaseSystem = Plane.CreateByNormalAndOrigin(shaBaseZ, shaBaseOrigin);
                    var toShaBaseSystem = SMatrix3d.BasisTransform(shaBaseSystem);
                    var toZ = SMatrix3d.ProjZ;
                    var transform = toZ * toShaBaseSystem;

                    var aa = ptsOnHaraWall.Select(x => transform * x).ToArray();
                    var touchPt = ptsOnHaraWall.Select(x => transform * x).OrderByDescending(x => x.Z).FirstOrDefault();
                    var length = touchPt.Z;

                    mortarLength[i] = length;

                    // 以下はモルタル長さを加味した斜梁ベースの始終端の算出
                    var originalPt = e0prime[i];
                    var dist = shaBaseOrigin.DistanceTo(originalPt);
                    e0prime[i] = originalPt + (-dist + length) * shaBaseZ;
                }
            }

            // 斜梁ベースファミリの読込
            var symbolFolpath = ClsZumenInfo.GetYMSFolder();
            var shinfamily = System.IO.Path.Combine(symbolFolpath, "ベース関係\\" + baseName + ".rfa");
            if (!ClsRevitUtil.LoadFamilySymbolData(doc, shinfamily, baseName, out FamilySymbol sym))
            {
                return default;
            }

            using (var transaction = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                transaction.Start();
                for (int i = 0; i < num; i++)
                {
                    ClsRevitUtil.SetParameter(doc, pieceInstances[i].Id, keyTheta2, ClsYMSUtil.CheckSyabariAngle(theta2Set[i]));
                    ClsRevitUtil.SetParameter(doc, pieceInstances[i].Id, keyTheta1, ClsYMSUtil.CheckSyabariAngle(theta1Set[i]));
                    ClsRevitUtil.SetParameter(doc, pieceInstances[i].Id, keyTheta0, ClsYMSUtil.CheckSyabariAngle(theta0Set[i]));

                    if (mortarLength[i] != null)
                    {
                        ClsRevitUtil.SetParameter(doc, pieceInstances[i].Id, keyMortarLength, mortarLength[i].Value);
                    }
                }

                var tmpStPoint = e0prime[0];
                var tmpEdPoint = e0prime[1];
                var tmpThirdPoint = e0prime[0] - basisXprime2[0];
                var slopePlane = ClsRevitUtil.CreateReferencePlane(doc, tmpStPoint, tmpEdPoint, tmpThirdPoint);

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
                        var dir = (tmpEdPoint - tmpStPoint).Normalize();
                        var instance = doc.Create.NewFamilyInstance(reference, tmpStPoint, dir, sym);//tmpEdPoint - tmpStPoint
                        var createId = instance.Id;
                        m_ElementId = createId;
                        ClsRevitUtil.SetParameter(doc, createId, "長さ", tmpStPoint.DistanceTo(tmpEdPoint));
                        if (m_LevelId != null)
                            ClsRevitUtil.SetParameter(doc, createId, "集計レベル", m_LevelId);
                        //ClsRevitUtil.SetParameter(doc, createId, "基準レベルからの高さ", 0.0);//Z軸を求めるかもオフセット0にするかは暫定的
                        SetParameter(doc, createId);

                        var id = instance.Id;
                        var element = doc.GetElement(reference);
                        // ここで一回移動などをしないと Location が原点のまま返ってくる。
                        ElementTransformUtils.MoveElement(doc, id, XYZ.Zero);
                        slopePlane.Name = "斜梁" + id.IntegerValue.ToString();
                        resultId = id;
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

                transaction.Commit();
            }
            return resultId;
        }

        public static FamilySymbol LoadSymbol(Document doc, string path, string symbolName = null)
        {
            var shinfamily = path;
            var symbolName0 = !string.IsNullOrEmpty(symbolName) ? symbolName : System.IO.Path.GetFileNameWithoutExtension(shinfamily);
            if (!ClsRevitUtil.LoadFamilySymbolData(doc, shinfamily, symbolName0, out FamilySymbol sym1))
            {
                return null;
            }
            return sym1;
        }

        public (ElementId baseId, ElementId piece1, ElementId piece2) ReplaceShabariBase(Document doc, ElementId originalBaseId)
        {
            //斜梁ベースを選択
            var shaBase = doc.GetElement(originalBaseId) as FamilyInstance;
            var shaBaseLine = ClsYMSUtil.GetBaseLine(doc, shaBase.Id);
            var shaBaseMid = SLibRevitReo.MidPoint(shaBaseLine.GetEndPoint(0), shaBaseLine.GetEndPoint(1));
            var shaBaseInstance = new ClsSyabariBase();
            shaBaseInstance.SetClassParameter(doc, originalBaseId);

            var shabariClass = new ClsSyabariBase();
            shabariClass.SetClassParameter(doc, shaBase.Id);
            var num = 2;
            var buhinType = new string[]
            {
                    shabariClass.m_buhinTypeStart,
                    shabariClass.m_buhinTypeEnd,
            };
            var buhinSize = new string[]
            {
                    shabariClass.m_buhinSizeStart,
                    shabariClass.m_buhinSizeEnd,
            };
            var sym = new FamilySymbol[num];

            // 斜梁ピースファミリの読込
            var symbolFolpath = ClsZumenInfo.GetYMSFolder();
            for (int i = 0; i < num; i++)
            {
                var rec = Master.ClsSyabariPieceCSV.Shared.FirstOrDefault(x => x.TypeName == buhinType[i] && x.SizeName == buhinSize[i]);
                var sym1 = LoadSymbol(doc, rec.FamilyFullPath, rec.SymbolL); // 斜梁端部で LR を分けるピースは無いのでシンボル名は L 固定
                if (sym1 == null) { return default; }
                sym[i] = sym1;

                //var shinfamily = Master.ClsSyabariPieceCSV.GetFamilyPath(buhinSize[i]);
                ////var shinfamily = System.IO.Path.Combine(symbolFolpath, "支保工関係", buhinType[i], $"{buhinSize[i]}.rfa");
                //var symbolName = System.IO.Path.GetFileNameWithoutExtension(shinfamily);// string.IsNullOrWhiteSpace(buhinSize[i]) ? buhinType[i] : buhinSize[i];
                //if (!ClsRevitUtil.LoadFamilySymbolData(doc, shinfamily, symbolName, out FamilySymbol sym1))
                //{
                //    return default;
                //}
                //sym[i] = sym1;
            }

            var haraPtSet = ClsHaraokoshiBase.GetIntersectionBase2ModelLine(doc, shaBase.Id); //ClsHaraokoshiBase.GetIntersectionBase2(doc, shaBase.Id);
            //var modelList = ClsRevitUtil.GetIn
            if (haraPtSet.Count != 2)
            {
                return default;
            }

            //var aaa = ClsHaraokoshiBase.GetIntersectionBase2(doc, shaBase.Id);

            // 両方の端部に斜梁ピースを配置
            var pieceIds = new ElementId[num];
            using (var transaction = new Transaction(doc, Guid.NewGuid().ToString()))
            {
                transaction.Start();
                for (int i = 0; i < num; i++)
                {
                    var buddy = (i + 1) % num;
                    var startPt = shaBaseLine.GetEndPoint(i);
                    var endPt = shaBaseLine.GetEndPoint(buddy);

                    var dan = "同段";
                    var haraH = 0.0;
                    ElementId id = null;
                    XYZ point = new XYZ();
                    if (haraPtSet.Count == 0)
                    {
                        continue;
                    }
                    else
                    {
                        var haraPt = haraPtSet.FirstOrDefault(x => ClsGeo.GEO_EQ0(x.Point.DistanceTo(startPt)));
                        if (haraPt == null) { continue; }

                        id = haraPt.Id;
                        point = haraPt.Point;
                        if (doc.GetElement(id).Name == ClsHaraokoshiBase.baseName)
                        {
                            dan = ClsRevitUtil.GetParameter(doc, id, "段");
                            var size = ClsRevitUtil.GetParameter(doc, id, "鋼材サイズ");
                            haraH = ClsRevitUtil.CovertToAPI(Master.ClsYamadomeCsv.GetWidth(size));
                        }
                    }


                    XYZ pt;
                    if (dan == "上段")
                    {
                        pt = startPt + 0.5 * haraH * XYZ.BasisZ;
                    }
                    else if (dan == "下段")
                    {
                        pt = startPt - 0.5 * haraH * XYZ.BasisZ;
                    }
                    else
                    {
                        pt = startPt;
                    }
                    //haraBase.m_kouzaiSize

                    if (!sym[i].IsActive) { sym[i].Activate(); }
                    //var pt = haraPt.Point;
                    var toMid = shaBaseMid - point;
                    var haraLine = ClsYMSUtil.GetBaseLine(doc, id);
                    var haraOrthSystem = Plane.CreateByOriginAndBasis(XYZ.Zero, XYZ.BasisZ, haraLine.Direction.Normalize());
                    var m = SMatrix3d.BasisTransform(haraOrthSystem);
                    var a = m.Inverse() * SMatrix3d.ProjZ * m;
                    var haraOrthDir = a * toMid;
                    var tanbuDir = SMatrix3d.Rotation(-XYZ.BasisZ, XYZ.Zero, 0.5 * Math.PI) * haraOrthDir;

                    var tanbuPlane = ClsRevitUtil.CreateReferencePlane(doc, pt, pt + XYZ.BasisX, pt + XYZ.BasisY);
                    //tanbuPlane.Name = $"斜梁端部{tanbuPlane.Id}";
                    //tanbuPlane.Maximize3DExtents();
                    var reference = tanbuPlane.GetReference();
                    var instance = doc.Create.NewFamilyInstance(reference, pt, tanbuDir.Normalize(), sym[i]);
                    if (ClsGeo.GEO_LT(pt.Z, endPt.Z))
                    {
                        ClsRevitUtil.SetParameter(doc, instance.Id, "設置角度", Math.PI);
                    }

                    ElementId levelId = ClsRevitUtil.GetParameterElementId(doc, originalBaseId, "集計レベル");
                    if (levelId != null)
                    {
                        ClsRevitUtil.SetParameter(doc, instance.Id, "集計レベル", levelId);
                    }
                    ClsKariKouzai.SetKariKouzaiFlag(doc, instance.Id);
                    pieceIds[i] = instance.Id;

                }
                transaction.Commit();
            }

            var baseId = CreateShabariBase(doc, pieceIds.ElementAtOrDefault(0), pieceIds.ElementAtOrDefault(1), shaBaseInstance);
            return (baseId, pieceIds[0], pieceIds[1]);
        }
        /// <summary>
        /// 回転ピースを配置する斜梁ベースとその始終点の腹起ベースがなす角はXY平面上で90度か判定
        /// </summary>
        /// <param name="cvS">斜梁ベース始点側に付く線</param>
        /// <param name="cvE">斜梁ベース終点側に付く線</param>
        /// <param name="tmpStPoint">斜梁ベース始点</param>
        /// <param name="tmpEdPoint">斜梁ベース終点</param>
        /// <returns></returns>
        public bool CheckKaitenPiece(Curve cvS, Curve cvE, XYZ tmpStPoint, XYZ tmpEdPoint)
        {
            bool check = false;
            var dir = (tmpEdPoint - tmpStPoint).Normalize();
            var pi2 = ClsGeo.FloorAtDigitAdjust(5, Math.PI / 2);
            var pi32 = ClsGeo.FloorAtDigitAdjust(5, Math.PI * 3 / 2);
            //始点側の部品チェック
            if (m_buhinTypeStart == "回転ピース")
            {
                var dirS = (cvS as Line).Direction;
                var angle = dir.AngleOnPlaneTo(dirS, XYZ.BasisZ);
                angle = ClsGeo.FloorAtDigitAdjust(5, angle);
                if (angle == pi2 || angle == -pi2 || angle == pi32 || angle == -pi32)
                    check = true;//90度pass
            }
            else
                check = true;//始点側は回転でなければ終点のCheck
            //終点側の端部部品と始点側をpassしているか
            if (m_buhinTypeEnd == "回転ピース" && check)
            {
                var dirE = (cvE as Line).Direction;
                var angle = -dir.AngleOnPlaneTo(dirE, XYZ.BasisZ);
                angle = ClsGeo.FloorAtDigitAdjust(5, angle);
                //終点側が90度pass出来るか
                if (angle == pi2 || angle == -pi2 || angle == pi32 || angle == -pi32)
                    check = true;
                else
                    check = false;
            }
            if (!check)
            {
                MessageBox.Show("斜梁ベースと腹起ベースのなす角が90度ではないため回転ピースを配置できません");
            }
            return check;
        }
        /// <summary>
        /// 斜梁ベース のみを単独選択
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="id">選択した 斜梁ベース のID</param>
        /// <param name="message">指示</param>
        /// <returns></returns>
        public static bool PickBaseObject(UIDocument uidoc, ref ElementId id, string message = baseName)
        {
            return ClsRevitUtil.PickObject(uidoc, message, baseName, ref id);
        }
        /// <summary>
        /// 斜梁ベースor仮鋼材 のみを単独選択
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="id">選択した 斜梁ベース のID</param>
        /// <param name="message">指示</param>
        /// <returns></returns>
        public static bool PickBaseOrKariKouzaiObject(UIDocument uidoc, ref ElementId id, string message = baseName + "仮鋼材")
        {
            var doc = uidoc.Document;
            var firterList = new List<string> { baseName };
            firterList.AddRange(Master.ClsYamadomeCsv.GetKariSyabariFamilyNameList().ToList());
            if (!ClsRevitUtil.PickObjectSymbolFilters(uidoc, message, firterList, ref id))
                return false;
            if (doc.GetElement(id).Name != baseName)
                id = ClsKariKouzai.GetBaseId(doc, id);
            return true;
        }
        /// <summary>
        /// 斜梁ベース のみを単独選択
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="id">選択した 斜梁ベース のID</param>
        /// <param name="message">指示</param>
        /// <returns></returns>
        public static bool PickBaseObject(UIDocument uidoc, ref Reference rf, string message = baseName)
        {
            return ClsRevitUtil.PickObject(uidoc, message, baseName, ref rf);
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
        /// <summary>
        /// 図面上の斜梁ベースを全て取得する
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
        ///  図面上の 斜梁ベース を全てクラスで取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<ClsSyabariBase> GetAllClsBaseList(Document doc)
        {
            var lstBase = new List<ClsSyabariBase>();

            var lstId = GetAllBaseList(doc);
            foreach (var id in lstId)
            {
                var cls = new ClsSyabariBase();
                cls.SetClassParameter(doc, id);
                lstBase.Add(cls);
            }

            return lstBase;
        }
        /// <summary>
        /// 対象のベースと接続する斜梁ベースを取得
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id">対象のベースID</param>
        /// <returns></returns>
        public static List<ElementId> GetIntersectionBase(Document doc, ElementId id, ElementId levelId = null)
        {
            List<ElementId> insecIdList = new List<ElementId>();
            List<ElementId> targetFamilies = GetAllBaseList(doc, levelId);

            FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
            Curve cvBase;
            if (inst.Location is LocationCurve lCurve)
            {
                cvBase = lCurve.Curve;
            }
            else
            {
                cvBase = ClsYMSUtil.GetBaseLine(doc, id);
            }

            foreach (ElementId tgId in targetFamilies)
            {
                FamilyInstance tgInst = doc.GetElement(tgId) as FamilyInstance;
                if (inst == tgInst)
                    continue;
                Curve tgCv = ClsYMSUtil.GetBaseLine(doc, tgId);
                XYZ insec = ClsRevitUtil.GetIntersection(cvBase, tgCv);
                if (insec != null)
                {
                    insecIdList.Add(tgId);
                }
            }
            return insecIdList;
        }
        /// <summary>
        /// 斜梁ベースにパラメータを追加する※長さは追加しない
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id"></param>
        public void SetParameter(Document doc, ElementId id)
        {
            ClsRevitUtil.SetMojiParameter(doc, id, "鋼材タイプ", m_kouzaiType);
            ClsRevitUtil.SetMojiParameter(doc, id, "鋼材サイズ", m_kouzaiSize);
            ClsRevitUtil.SetMojiParameter(doc, id, "端部部品タイプ(始点側)", m_buhinTypeStart);
            ClsRevitUtil.SetMojiParameter(doc, id, "端部部品サイズ(始点側)", m_buhinSizeStart);
            ClsRevitUtil.SetMojiParameter(doc, id, "端部部品タイプ(終点側)", m_buhinTypeEnd);
            ClsRevitUtil.SetMojiParameter(doc, id, "端部部品サイズ(終点側)", m_buhinSizeEnd);
            ClsRevitUtil.SetMojiParameter(doc, id, "ジャッキタイプ(1)", m_jack1);
            ClsRevitUtil.SetMojiParameter(doc, id, "ジャッキタイプ(2)", m_jack2);
        }
        /// <summary>
        /// 指定したIDから斜梁ベースクラスを取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public void SetClassParameter(Document doc, ElementId id)
        {
            m_ElementId = id;

            m_LevelId = ClsRevitUtil.GetParameterElementId(doc, id, "集計レベル");
            m_Floor = doc.GetElement(ClsRevitUtil.GetParameterElementId(doc, id, "集計レベル")).Name;
            //m_ReferencePlanee = (doc.GetElement(id) as FamilyInstance).Host as ReferencePlane;
            m_kouzaiType = ClsRevitUtil.GetParameter(doc, id, "鋼材タイプ");
            m_kouzaiSize = ClsRevitUtil.GetParameter(doc, id, "鋼材サイズ");
            //m_ShoriType = ShoriType.BaseLine;
            //m_endOffset1 = 0.0;
            m_buhinTypeStart = ClsRevitUtil.GetParameter(doc, id, "端部部品タイプ(始点側)");
            m_buhinSizeStart = ClsRevitUtil.GetParameter(doc, id, "端部部品サイズ(始点側)");
            m_buhinTypeEnd = ClsRevitUtil.GetParameter(doc, id, "端部部品タイプ(終点側)");
            m_buhinSizeEnd = ClsRevitUtil.GetParameter(doc, id, "端部部品サイズ(終点側)");
            m_jack1 = ClsRevitUtil.GetParameter(doc, id, "ジャッキタイプ(1)");
            m_jack2 = ClsRevitUtil.GetParameter(doc, id, "ジャッキタイプ(2)");
        }
        /// <summary>
        /// 対象のファミリに始終点を持たせるをカスタムデータとして持たせる
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id"></param>
        /// <param name="pointS"></param>
        /// <param name="pointE"></param>
        /// <returns></returns>
        public static bool SetFirstPoint(Document doc, ElementId id, XYZ pointS, XYZ pointE)
        {
            if (!ClsRevitUtil.CustomDataSet<string>(doc, id, FirstPointS, pointS.ToString()))
                return false;
            if (!ClsRevitUtil.CustomDataSet<string>(doc, id, FirstPointE, pointE.ToString()))
                return false;

            return true;
        }
        /// <summary>
        /// 対象のファミリに始終点を持たせるカスタムデータを取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id">ファミリ</param>
        /// <returns>ベースのID</returns>
        public static bool GetFirstPoint(Document doc, ElementId id, out XYZ pointS, out XYZ pointE)
        {
            pointS = new XYZ();
            pointE = new XYZ();
            if (!ClsRevitUtil.CustomDataGet<string>(doc, id, FirstPointS, out string strPointS))
                return false;
            if (!ClsRevitUtil.CustomDataGet<string>(doc, id, FirstPointE, out string strPointE))
                return false;

            pointS = ClsRevitUtil.StringToXYZ(strPointS);
            pointE = ClsRevitUtil.StringToXYZ(strPointE);
            return true;
        }
        /// <summary>
        /// 対象のファミリに始終点を持たせるをカスタムデータとして持たせる
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id"></param>
        /// <param name="pointS"></param>
        /// <param name="pointE"></param>
        /// <returns></returns>
        public static bool SetCustomDataReferencePlane(Document doc, ElementId id, ReferencePlane referencePlane)
        {
            return ClsRevitUtil.CustomDataSet<ElementId>(doc, id, FirstReferencePlane, referencePlane.Id);
        }
        /// <summary>
        /// 対象のファミリに始終点を持たせるカスタムデータを取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id">ファミリ</param>
        /// <returns>ベースのID</returns>
        public static bool GetCustomDataReferencePlane(Document doc, ElementId id, out ReferencePlane reference)
        {
            reference = null;
            if (!ClsRevitUtil.CustomDataGet<ElementId>(doc, id, FirstReferencePlane, out ElementId referenceId))
                return false;
            reference = doc.GetElement(referenceId) as ReferencePlane;
            return true;
        }


        /// <summary>
        /// 斜梁のみを単独選択
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="ids">選択した 斜梁のID</param>
        /// <param name="message">指示</param>
        /// <returns></returns>
        public static bool PickObjects(UIDocument uidoc, ref List<ElementId> ids, string message = baseName)
        {
            return ClsRevitUtil.PickObjects(uidoc, message, "斜梁", ref ids);
        }
        #endregion
    }
}
