using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Fresco.Geometory;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YMS_gantry.Data;
using YMS_gantry.Master;
using YMS_gantry.Material;
using YMS_gantry.UI;
using YMS_gantry.UI.FrnCreateSlopeControls;
using static YMS_gantry.UI.FrmPutHorizontalBraceData;

namespace YMS_gantry.Command
{
    class CmdPutHorizontalBrace
    {
        static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        Document _doc { get; set; }
        UIDocument _uiDoc { get; set; }


        public CmdPutHorizontalBrace(UIDocument uiDoc)
        {
            _uiDoc = uiDoc;
            _doc = _uiDoc.Document;
        }

        /// <summary>
        /// 実行
        /// </summary>
        /// <returns></returns>
        public bool Excute()
        {
            Selection selection = _uiDoc.Selection;
            try
            {
                FrmPutHorizontalBrace f = Application.thisApp.frmPutHorizontalBrace;

                FrmPutHorizontalBrace.PutHorizontalBraceId id = f.putHorizontalBraceId;
                MaterialSuper[] unionMaterials = MaterialSuper.Collect(_doc);
                List<ElementId> elementIds = new List<ElementId>();

                FrmPutHorizontalBraceData fData = f.data;
                switch (id)
                {
                    case FrmPutHorizontalBrace.PutHorizontalBraceId.Cancel:
                        f.Close();
                        break;
                    case FrmPutHorizontalBrace.PutHorizontalBraceId.OK:


                        //■■■■■■■■■■■■■■ここからOKの処理■■■■■■■■■

                        using (var transaction = new Transaction(_doc))
                        {
                            transaction.Start(Guid.NewGuid().ToString());

                            ModelData md = ModelData.GetModelData(_doc);
                            AllKoudaiFlatFrmData kodaiData = md.GetKoudaiData(fData.SelectedKodai).AllKoudaiFlatData;
                            var baseLevel = new FilteredElementCollector(_doc)
                            .OfClass(typeof(Level))
                            .Cast<Level>()
                            .FirstOrDefault(x => x.Name == kodaiData.SelectedLevel);
                            if (baseLevel == null)
                            {
                                MessageUtil.Error("構台の基準レベルが取得できません。", "error");
                                return false;
                            }
                            var positivePlane = PositivePlane(_doc, baseLevel);
                            var negativePlane = NegativePlane(_doc, baseLevel);
                            ReferencePlane plane = null;

                            var ohbikiSizeValue = kodaiData.ohbikiData.OhbikiSize;
                            if (ohbikiSizeValue.Contains("HA") || ohbikiSizeValue.Contains("SMH"))
                            {
                                var obikiSizeTrim = ohbikiSizeValue.Contains("_") ? ohbikiSizeValue.Substring(0, ohbikiSizeValue.IndexOf('_')) : ohbikiSizeValue;
                                ohbikiSizeValue = ClsKouzaiSpecify.GetKouzaiSize(obikiSizeTrim);
                            }

                            //大引きサイズ
                            var ohbikiSize = ClsMasterCsv.Shared
                                    .Select(x => x as ClsHBeamCsv)
                                    .Where(x => x != null)
                                    .FirstOrDefault(x => x.Size == ohbikiSizeValue.Replace("-", ""));

                            // 最高大引下端の取得
                            var highestOhbikiBottomFt = MaterialSuper
                                .Collect(_doc)
                                .Where(x => x.m_KodaiName == fData.SelectedKodai)
                                .Select(x => x as Ohbiki)
                                .Where(x => x != null)
                                .Select(x => ClsRevitUtil.GetParameterDouble(_doc, x.m_ElementId, DefineUtil.PARAM_BASE_OFFSET) - 0.5 * (x.SteelSize?.HeightFeet ?? 0.0))
                                .Max();

                            var levelOffsetSet = 0.0;
                            if (fData.IsTopPlate && (kodaiData.pilePillerData.HasTopPlate || kodaiData.pilePillerData.ExtensionPile))
                            {
                                // トッププレート
                                var plateSizeText = kodaiData.pilePillerData.ExtensionPile ?
                                    kodaiData.pilePillerData.extensionPileData.topPlateData.PlateSize :
                                    kodaiData.pilePillerData.topPlateData.PlateSize;
                                var topplateSize = ClsMasterCsv.Shared.FirstOrDefault(x => x.Size == plateSizeText) as ClsTopPlateCsv;
                                var plateThick = topplateSize?.T.ToFt() ?? 0.0;
                                //levelOffsetSet = highestOhbikiBottomFt;
                                levelOffsetSet = highestOhbikiBottomFt - plateThick;
                                plane = positivePlane;
                            }
                            else if (fData.SetObikiFace == FrmPutHorizontalBraceData.SetFace.UpperB)
                            {
                                // 大引下部
                                var ohbikiFrangeThick = ohbikiSize?.T2Ft ?? 0.0;
                                //var ohbikiB = ohbikiSize?.BFt ?? 0.0;高さだからHでは？
                                var ohbikiB = ohbikiSize?.HFt ?? 0.0;
                                levelOffsetSet = highestOhbikiBottomFt +ohbikiB - ohbikiFrangeThick;

                                plane = positivePlane;
                            }
                            else if (fData.SetObikiFace == FrmPutHorizontalBraceData.SetFace.LowerT)
                            {
                                // 大引下部上
                                var ohbikiFrangeThick = ohbikiSize?.T2Ft ?? 0.0;
                                levelOffsetSet = -highestOhbikiBottomFt - ohbikiFrangeThick;

                                plane = negativePlane;
                            }
                            else
                            {
                                // 大引下部下
                                var ohbikiFrangeThick = ohbikiSize?.T2Ft ?? 0.0;
                                levelOffsetSet = highestOhbikiBottomFt;

                                plane = positivePlane;
                            }
                            transaction.Commit();

                            if (fData.PlaceIsSingle) //単純
                            {
                                if(fData.IsTurnBackle)
                                {
                                    CreateTurnBackle(fData);
                                }
                                else
                                {
                                    //transaction.Start(Guid.NewGuid().ToString());
                                    //XYZ p1 = new XYZ();
                                    //if (!ClsRevitUtil.PickPoint(_uiDoc, "始点をピックしてください", ref p1))
                                    //{
                                    //    break;
                                    //}

                                    //XYZ p2 = new XYZ();
                                    //if (!ClsRevitUtil.PickPoint(_uiDoc, "終点をピックしてください", ref p2))
                                    //{
                                    //    break;
                                    //}
                                    ////ブレースの作図
                                    //if (!CreateBrace(p1, p2, levelOffsetSet, plane.GetReference(), fData))
                                    //{
                                    //    break;
                                    //}
                                    //transaction.Commit();

                                    //単体配置処理
                                    var pickedRef = selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, new FilterRefPlane(), "水平ブレースを配置する参照面を選択");
                                    var refPlaneElem = _doc.GetElement(pickedRef.ElementId) as ReferencePlane;
                                    var refPlane = refPlaneElem.GetPlane();
                                    var view3d = new FilteredElementCollector(_doc)
                                        .OfClass(typeof(View3D))
                                        .Cast<View3D>()
                                        .FirstOrDefault(x => !x.Name.Contains("CASE"));

                                    // 3D ビューがあれば 3D ビューを上から見た状態にする
                                    var viewOrientation = view3d.GetOrientation();
                                    var toBottomOrientation = new ViewOrientation3D(XYZ.Zero, XYZ.BasisY, -XYZ.BasisZ);
                                    view3d.SetOrientation(toBottomOrientation);
                                    view3d.OrientTo(-XYZ.BasisZ);
                                    //view3d.get_Parameter(BuiltInParameter.MODEL_GRAPHICS_STYLE).Set(1);
                                    _uiDoc.ActiveView = view3d;

                                    transaction.Start(Guid.NewGuid().ToString());

                                    var originalGraphicStyle = view3d.get_Parameter(BuiltInParameter.MODEL_GRAPHICS_STYLE).AsInteger();
                                    using (var defer = new Defer(
                                        () => { view3d.get_Parameter(BuiltInParameter.MODEL_GRAPHICS_STYLE).Set(1); },
                                        () => { view3d.get_Parameter(BuiltInParameter.MODEL_GRAPHICS_STYLE).Set(originalGraphicStyle); }))
                                    {
                                        XYZ p1 = new XYZ();
                                        if (!ClsRevitUtil.PickPoint(_uiDoc, "始点をピックしてください", ref p1))
                                        {
                                            break;
                                        }
                                        p1 = CalcPerp(refPlane, p1, view3d.GetOrientation().ForwardDirection);


                                        XYZ p2 = new XYZ();
                                        if (!ClsRevitUtil.PickPoint(_uiDoc, "終点をピックしてください", ref p2))
                                        {
                                            break;
                                        }
                                        p2 = CalcPerp(refPlane, p2, view3d.GetOrientation().ForwardDirection);

                                        //ブレースの作図
                                        if (!CreateBrace(p1, p2, 0, refPlaneElem.GetReference(), fData))
                                        {
                                            break;
                                        }
                                    }

                                      
                                    transaction.Commit();
                                }
                            }
                            else if (fData.PlaceIsMultiple)//連続
                            {
                                while(true)
                                {
                                    transaction.Start(Guid.NewGuid().ToString());
                                    //杭を指定
                                    ElementId kuiId = null;
                                    if (!ClsRevitUtil.PickObjectPartFilter(_uiDoc, "杭を選択してください", "杭", ref kuiId))
                                    {
                                        break;
                                    }
                                    //杭の座標を取得
                                    FamilyInstance instKui = _doc.GetElement(kuiId) as FamilyInstance;

                                    XYZ p1 = new XYZ();
                                    Location location = instKui.Location;
                                    if (location != null && location is LocationPoint)
                                    {
                                        LocationPoint locationPoint = location as LocationPoint;
                                        p1 = locationPoint.Point; // 挿入基点の座標を取得
                                    }

                                    //大引きを指定
                                    ElementId ohbikiId = null;
                                    if (!ClsRevitUtil.PickObjectPartFilter(_uiDoc, "大引を選択してください", "大引", ref ohbikiId))
                                    {
                                        break;
                                    }
                                    FamilyInstance instOhbiki = _doc.GetElement(ohbikiId) as FamilyInstance;
                                    XYZ vecOhbiki1 = instOhbiki.HandOrientation;


                                    //杭を指定
                                    ElementId kuiId2 = null;
                                    if (!ClsRevitUtil.PickObjectPartFilter(_uiDoc, "杭を選択してください", "杭", ref kuiId2))
                                    {
                                        break;
                                    }
                                    //杭の座標を取得
                                    FamilyInstance instKui2 = _doc.GetElement(kuiId2) as FamilyInstance;
                                    XYZ vecOhbiki2 = instOhbiki.HandOrientation;

                                    XYZ p2 = new XYZ();
                                    Location location2 = instKui2.Location;
                                    if (location2 != null && location2 is LocationPoint)
                                    {
                                        LocationPoint locationPoint = location2 as LocationPoint;
                                        p2 = locationPoint.Point; // 挿入基点の座標を取得
                                    }

                                    //大引きを指定
                                    if (!ClsRevitUtil.PickObjectPartFilter(_uiDoc, "大引を選択してください", "大引", ref kuiId))
                                    {
                                        break;
                                    }

                                    //計算処理
                                    double dAngle = vecOhbiki1.AngleTo(p1 - p2);
                                    dAngle = (((Math.PI)/2) > dAngle ? dAngle:(Math.PI) - dAngle);
                                    double dLength = (p1 - p2).GetLength();
                                    double dSin = Math.Sin(dAngle);
                                    double dCos = Math.Cos(dAngle);
                                    double dLengthH = dCos * dLength;
                                    double dLengthK = dSin * dLength;
                                    List<ClsAngleCsv> lst = ClsAngleCsv.GetCsvData();
                                    double dWidth = (from data in lst where data.Size == fData.Size select data.AFt).FirstOrDefault();
                                    List<XYZ> lstCalc = GetCalcPoints(p1, dLengthH, dLengthK, dWidth, fData.VrtBrace1UpperXFt, fData.VrtBrace1UpperYFt, vecOhbiki1);

                                    //4種に分類
                                    XYZ targetVector = new XYZ(0, 0, 1);
                                    XYZ vecK = -vecOhbiki1.CrossProduct(targetVector);
                                    bool bHLeft = false;
                                    if (ClsGeo.IsLeft(vecOhbiki1, p2 - p1))
                                    {
                                        bHLeft = true;
                                    }
                                    bool bKLeft = false;
                                    if (ClsGeo.IsLeft(vecK, p2 - p1))
                                    {
                                        bKLeft = true;
                                    }
                                    List<XYZ> lstP = new List<XYZ>();
                                    if(bHLeft && bKLeft)　// 右上
                                    {
                                        XYZ mirrorP1 = GetSymmetricalPoint(lstCalc[0], p1,vecK);
                                        XYZ mirrorP2 = GetSymmetricalPoint(lstCalc[1], p1,vecK);
                                        lstP = new List<XYZ>() { mirrorP2, mirrorP1 };
                                    }
                                    else if (!bHLeft && bKLeft)　//左上
                                    {
                                        XYZ mirrorP1 = GetSymmetricalPoint(lstCalc[0], p1, vecK);
                                        mirrorP1 = GetSymmetricalPoint(mirrorP1, p1, vecOhbiki1);
                                        XYZ mirrorP2 = GetSymmetricalPoint(lstCalc[1], p1, vecK);
                                        mirrorP2 = GetSymmetricalPoint(mirrorP2, p1, vecOhbiki1);
                                        lstP = new List<XYZ>() { mirrorP1, mirrorP2 };
                                    }
                                    else if (!bHLeft && !bKLeft) //左下
                                    {
                                        XYZ mirrorP1 = GetSymmetricalPoint(lstCalc[0], p1, vecOhbiki1);
                                        XYZ mirrorP2 = GetSymmetricalPoint(lstCalc[1], p1, vecOhbiki1);
                                        lstP = new List<XYZ>() { mirrorP2, mirrorP1 };
                                    }
                                    else if (bHLeft && !bKLeft)　//右下
                                    {
                                        lstP = lstCalc;
                                    }

                                    ////4点を取得
                                    //List <XYZ> lst1 = new List<XYZ>();
                                    //if (!GetNgAreaPoints(p1, vecOhbiki1, fData.VrtBrace1UpperXFt, fData.VrtBrace1UpperYFt, ref lst1))
                                    //{
                                    //    break;
                                    //}
                                    //List<XYZ> lst2 = new List<XYZ>();
                                    //if (!GetNgAreaPoints(p2, vecOhbiki2, fData.VrtBrace1UpperXFt, fData.VrtBrace1UpperYFt, ref lst2))
                                    //{
                                    //    break;
                                    //}
                                    ////4点を取得
                                    //List<XYZ> lst1 = new List<XYZ>();
                                    //if (!GetNgAreaPoints2(p1, lstCalc[1], vecOhbiki1, ref lst1))
                                    //{
                                    //    break;
                                    //}
                                    //List<XYZ> lst2 = new List<XYZ>();
                                    //if (!GetNgAreaPoints2(p2, p2 + (p1-lstCalc[1]), vecOhbiki2, ref lst2))
                                    //{
                                    //    break;
                                    //}

                                    ////一番近い2点を取得
                                    //List<XYZ> lstP = MostNearDistancePoint(lst1, lst2);
                                    //List<XYZ> lstP = lstCalc;

                                    ////幾何計算でYの位置を調整
                                    //List<ClsAngleCsv> lst = ClsAngleCsv.GetCsvData();
                                    //double dWidth = (from data in lst where data.Size == fData.Size select data.AFt).FirstOrDefault();
                                    //double dYplus = GetXplus(lstP[0], lstP[1], vecOhbiki1, dWidth);

                                    ////再度4点を取得
                                    //lst1 = new List<XYZ>();
                                    //if (!GetNgAreaPoints(p1, vecOhbiki1, fData.VrtBrace1UpperXFt, fData.VrtBrace1UpperYFt + dYplus, ref lst1))
                                    //{
                                    //    break;
                                    //}
                                    //lst2 = new List<XYZ>();
                                    //if (!GetNgAreaPoints(p2, vecOhbiki2, fData.VrtBrace1UpperXFt, fData.VrtBrace1UpperYFt + dYplus, ref lst2))
                                    //{
                                    //    break;
                                    //}

                                    ////一番近い2点を取得
                                    //lstP = MostNearDistancePoint(lst1, lst2);

                                    //上部につける場合は、始点終点を反転する
                                    if(fData.SetObikiFace == FrmPutHorizontalBraceData.SetFace.LowerT)
                                    {
                                        lstP = new List<XYZ>() { lstP[1], lstP[0] };
                                    }

                                    //ブレースの作図
                                    if (!CreateBrace(lstP[1], lstP[0], levelOffsetSet, plane.GetReference(), fData,false))
                                    {
                                        break;
                                    }
                                    transaction.Commit();
                                }
                            }
                        }
                        //■■■■■■■■■■■■■■ここまでOKの処理■■■■■■■■■


                        break;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return false;
            }
        }
        public bool GetNgAreaPoints(XYZ p, XYZ vecOhbiki, double dX, double dY, ref List<XYZ> lst)
        {
            try
            {
                XYZ targetVector = new XYZ(0, 0, 1); 
                XYZ perpendicularVector = vecOhbiki.CrossProduct(targetVector);

                XYZ p1  = p + (perpendicularVector * dX) + (vecOhbiki * dY);
                XYZ p2 = p + (-perpendicularVector * dX) + (vecOhbiki * dY);
                XYZ p3 = p + (perpendicularVector * dX) + (-vecOhbiki * dY);
                XYZ p4 = p + (-perpendicularVector * dX) + (-vecOhbiki * dY);

                lst = new List<XYZ>() { p1,p2,p3,p4};

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return false;
            }
        }

        public XYZ GetSymmetricalPoint(XYZ pointP, XYZ pointA, XYZ vectorB)
        {
            // 線分ABのベクトルを計算
            XYZ vectorAB = vectorB;

            // ベクトルABを正規化
            XYZ normalizedVectorAB = vectorAB.Normalize();

            // 点Pから線分ABに垂直なベクトルを計算
            XYZ perpendicularVector = pointP - pointA;
            double dotProduct = perpendicularVector.DotProduct(normalizedVectorAB);
            XYZ projection = normalizedVectorAB * dotProduct;

            // 線分ABに対して点Pの線対称の点を計算
            XYZ symmetricalPoint = pointP + 2 * (projection - perpendicularVector);

            return symmetricalPoint;

        }

        public List<XYZ> GetCalcPoints(XYZ origin,double lengthH, double lengthK, double angleWidth, double dXFt, double dYFt, XYZ axisH)
        {
            List<XYZ> lstRes = new List<XYZ>();

            XYZ targetVector = new XYZ(0, 0, 1);
            XYZ axisK = -axisH.CrossProduct(targetVector).Normalize();
            try
            {

                //var startK = dSK;
                //var endK = pillarKSet[(indexK + 1) % pillarKSet.Length];
                //var lengthK = Math.Abs(endK - startK);
                var targetWidthK = lengthK - 2.0 * dXFt;

                //var startH = pillarHSet[indexH];
                //var endH = pillarHSet[(indexH + 1) % pillarHSet.Length];
                //var lengthH = Math.Abs(endH - startH);
                var targetWidthH = lengthH - 2.0 * dYFt;

                var a = targetWidthK;
                var b = targetWidthH;
                var c = angleWidth;

                var coefSet = new double[]
                {
                                        c*c,
                                        -2*b*c,
                                        a*a+b*b,
                                        0.0,
                                        -a*a,
                };

                var k = 0;
                var roots = UtilAlgebra.SolveQuarticEquation(coefSet[k++], coefSet[k++], coefSet[k++], coefSet[k++], coefSet[k++]);

                var cosTheta = roots.FirstOrDefault(x => UtilAlgebra.Eq(x.Imaginary, 0.0) && x.Real > 0).Real;
                var sinTheta = Math.Sqrt(1.0 - cosTheta * cosTheta);

                var braceOrigin = origin + (dYFt) * axisH + (dXFt) * axisK;

                //var aaaa = braceOrigin + c * cosTheta * axisH + (a + 0.5 * sinTheta) * axisK;
                var braceEndSet = new XYZ[]
                {
                                        braceOrigin + 0.5 * c * sinTheta * axisK,
                                        braceOrigin + b * axisH + 0.5 * c * sinTheta * axisK,
                };
                var braceStartSet = new XYZ[]
                {
                                        braceOrigin + (a + 0.5 * c * sinTheta) * axisK + (b - c * cosTheta) * axisH,
                                        braceOrigin + c * cosTheta * axisH + (a + 0.5 * c * sinTheta) * axisK,
                };

                lstRes = new List<XYZ>() { braceStartSet[0], braceEndSet[0] };


                return lstRes;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return lstRes;
            }
        }

        public double GetXplus(XYZ p1, XYZ p2, XYZ vecOhbiki, double dWidth)
        {
            try
            {
                XYZ targetVector = (p2 - p1).Normalize();
                double dAngle1 = targetVector.AngleTo(vecOhbiki);
                double dAngle2 = targetVector.AngleTo(-vecOhbiki);
                double dAngle = (dAngle1 < dAngle2 ? dAngle1 : dAngle2);

                //double dAngleEikaku = (Math.PI / 2) - dAngle;
                double dSin = Math.Sin(dAngle);

                return dSin * (dWidth /2);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return 0;
            }
        }

        public List<XYZ> MostNearDistancePoint(List<XYZ> lst1, List<XYZ> lst2)
        {
            List<XYZ> res = new List<XYZ>();
            try
            {
               double dMin = double.MaxValue;
                foreach(XYZ p1 in lst1)
                {
                    foreach (XYZ p2 in lst2)
                    {
                        if(p1.DistanceTo(p2) < dMin)
                        {
                            dMin = p1.DistanceTo(p2);
                            res = new List<XYZ>() { p1, p2 };
                        }
                    }
                }

                return res;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return res;
            }
        }

        public bool CreateTurnBackle(FrmPutHorizontalBraceData fData)
        {
            try
            {
                using (Transaction tr = new Transaction(_doc))
                {

                    GantryUtil util = new GantryUtil();
                    //配置
                    string strfPath = Master.ClsTurnBackleCsv.GetFamilyPath(fData.Size);
                    string familyPath = Directory.EnumerateFiles(PathUtil.GetYMSFolder(), Path.GetFileName(strfPath), SearchOption.AllDirectories).FirstOrDefault();
                    string familyName = Path.GetFileNameWithoutExtension(familyPath);
                    string familyTypeName = "水平ﾌﾞﾚｰｽ上部";

                    if (!GantryUtil.GetFamilySymbol(_doc, familyPath, familyTypeName, out var sym, false))
                    {
                        return false;
                    }

                    List<ElementId> ids = util.PlaceFamilyInstance(_uiDoc.Application, sym, false);
                    tr.Start("TurnBackle");
                    foreach (ElementId id in ids)
                    {
                        var material = new HorizontalBrace
                        {
                            m_KodaiName = fData.SelectedKodai,
                            m_Document = _doc,
                            m_ElementId = id,
                            m_Size = fData.Size,
                            m_TeiketsuType = fData.VrtBraceConnectingType == ConnectingType.Welding ? DefineUtil.eJoinType.Welding : fData.VrtBraceConnectingType == ConnectingType.Bolt ? DefineUtil.eJoinType.Bolt : DefineUtil.eJoinType.Support
                        };
                        MaterialSuper.WriteToElement(material, _doc);
                    }
                    tr.Commit();
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return false;
            }
        }


        public bool CreateBrace(XYZ p1, XYZ p2, double dOffset, Reference reference, FrmPutHorizontalBraceData fData,bool bCenter = true)
        {
            try
            {

                //ブレースは配置点を部材中央部とする
                double dWidth = 0;

                List<ClsAngleCsv> lst = ClsAngleCsv.GetCsvData();
                dWidth = (from data in lst where data.Size == fData.Size select data.AFt).FirstOrDefault();

                XYZ vec1to2 = p2 - p1;
                XYZ targetVector = new XYZ(0, 0, 1);
                XYZ perpendicularVector = vec1to2.CrossProduct(targetVector).Normalize();
                if(bCenter)
                {
                    p1 = p1 + (-perpendicularVector * dWidth * 0.5);
                    p2 = p2 + (-perpendicularVector * dWidth * 0.5);
                }

                var braceStart = p1;
                var braceEnd = p2;
                var levelOffset = dOffset;
                //var reference = reference;// baseLevel.GetPlaneReference();

                string strfPath = string.Empty;
                string familyPath = string.Empty;
                string familyName = string.Empty;
                string familyTypeName = string.Empty;

                strfPath = Master.ClsAngleCsv.GetFamilyPath(fData.Size);
                familyPath = Directory.EnumerateFiles(PathUtil.GetYMSFolder(), Path.GetFileName(strfPath), SearchOption.AllDirectories).FirstOrDefault();
                familyName = Path.GetFileNameWithoutExtension(familyPath);
                familyTypeName = "ﾌﾞﾚｰｽ";

                if (!GantryUtil.GetFamilySymbol(_doc, familyPath, familyTypeName, out var symbol, true))
                {
                    return false;
                }

                var id = MaterialSuper.PlaceWithTwoPoints(symbol, reference, braceStart, braceEnd, levelOffset);
                if (id == ElementId.InvalidElementId)
                {
                    _logger.Error("ブレースファミリインスタンスの配置に失敗");
                    return false;
                }
                var material = new HorizontalBrace
                {
                    m_KodaiName = fData.SelectedKodai,
                    m_Document = _doc,
                    m_ElementId = id,
                    m_Size = fData.Size,
                    m_TeiketsuType = fData.VrtBraceConnectingType == ConnectingType.Welding ? DefineUtil.eJoinType.Welding : fData.VrtBraceConnectingType == ConnectingType.Bolt ? DefineUtil.eJoinType.Bolt : DefineUtil.eJoinType.Support
                };
                MaterialSuper.WriteToElement(material, _doc);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return false;
            }
        }


        static ReferencePlane NegativePlane(Document doc, Level baseLevel)
        {
            var elevation = baseLevel.ProjectElevation;
            var origin = XYZ.Zero + elevation * XYZ.BasisZ;
            return doc.Create.NewReferencePlane(origin, origin + XYZ.BasisY, -XYZ.BasisX, doc.ActiveView);
        }

        static ReferencePlane PositivePlane(Document doc, Level baseLevel)
        {
            var elevation = baseLevel.ProjectElevation;
            var origin = XYZ.Zero + elevation * XYZ.BasisZ;
            return doc.Create.NewReferencePlane(origin, origin + XYZ.BasisY, XYZ.BasisX, doc.ActiveView);
        }


        /// <summary>
        /// pt を direction で plane へ射影した結果を求める。
        /// 不定の場合は null を返す
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="pt"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        static XYZ CalcPerp(Plane plane, XYZ pt, XYZ direction)
        {
            var x = plane.XVec;
            var y = plane.YVec;
            var p = pt;
            var z = plane.Origin;
            var d = direction;
            var matrix = new Matrix2d(new double[] {
                x.X, y.X, -d.X,
                x.Y, y.Y, -d.Y,
                x.Z, y.Z, -d.Z
            });
            var inv = matrix.Inverse();
            if (inv == null) { return null; }
            var c = p - z;
            var gamma = inv[2, 0] * c.X + inv[2, 1] * c.Y + inv[2, 2] * c.Z;
            return p + gamma * d;
        }
    }
}
