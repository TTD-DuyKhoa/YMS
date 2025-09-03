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
using static YMS_gantry.UI.FrmPutVerticalBrace;
using static YMS_gantry.UI.FrmPutVerticalBrace.FrmPutVerticalBraceData;

namespace YMS_gantry.Command
{
    class CmdPutVerticalBrace
    {
        static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        Document _doc { get; set; }
        UIDocument _uiDoc { get; set; }


        public CmdPutVerticalBrace(UIDocument uiDoc)
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
                FrmPutVerticalBrace f = Application.thisApp.frmPutVerticalBrace;

                FrmPutVerticalBrace.PutVerticalBraceId vbId = f.putVerticalBraceId;
                MaterialSuper[] unionMaterials = MaterialSuper.Collect(_doc);
                List<ElementId> elementIds = new List<ElementId>();

                FrmPutVerticalBraceData fData = f.data;
                switch (vbId)
                {
                    case FrmPutVerticalBrace.PutVerticalBraceId.Cancel:
                        f.Close();
                        break;
                    case FrmPutVerticalBrace.PutVerticalBraceId.OK:


                        //■■■■■■■■■■■■■■ここからOKの処理■■■■■■■■■

                        using (Transaction transaction = new Transaction(_doc, Guid.NewGuid().GetHashCode().ToString()))
                        {
                            if (fData.PlaceIsSingle) //単純
                            {
                                try
                                {
                                    if(fData.IsTurnBackle)
                                    {
                                        CreateTurnBackle(fData);
                                    }
                                    else
                                    {
                                        Document doc = _doc;
                                        UIDocument uidoc = _uiDoc;
                                        //単体配置処理
                                        var pickedRef = selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, new FilterRefPlane(), "垂直ブレースを配置する参照面を選択");
                                        var refPlaneElem = doc.GetElement(pickedRef.ElementId) as ReferencePlane;
                                        var refPlane = refPlaneElem.GetPlane();
                                        var view3d = new FilteredElementCollector(doc)
                                            .OfClass(typeof(View3D))
                                            .Cast<View3D>()
                                            .FirstOrDefault(x => !x.Name.Contains("CASE"));

                                        // 3D ビューがあれば 3D ビューを上から見た状態にする
                                        var viewOrientation = view3d.GetOrientation();
                                        var toBottomOrientation = new ViewOrientation3D(XYZ.Zero, XYZ.BasisY, -XYZ.BasisZ);
                                        view3d.SetOrientation(toBottomOrientation);
                                        view3d.OrientTo(refPlane.Normal);
                                        //view3d.OrientTo(-XYZ.BasisZ);
                                        //view3d.get_Parameter(BuiltInParameter.MODEL_GRAPHICS_STYLE).Set(1);
                                        uidoc.ActiveView = view3d;

                                        //XYZ perpendicularVector = refPlane.Normal.CrossProduct(targetVector);


                                        // 端点指定の時だけ Wireframe 表示に切り替え
                                        var originalGraphicStyle = view3d.get_Parameter(BuiltInParameter.MODEL_GRAPHICS_STYLE).AsInteger();
                                        transaction.Start();
                                        using (var defer = new Defer(
                                            () => { view3d.get_Parameter(BuiltInParameter.MODEL_GRAPHICS_STYLE).Set(1); },
                                            () => { view3d.get_Parameter(BuiltInParameter.MODEL_GRAPHICS_STYLE).Set(originalGraphicStyle); }))
                                        {
                                            while (true)
                                            {
                                                try
                                                {
                                                    // 端点.1 を選択
                                                    ISelectionFilter selFilters = new FamilyPartSelectionFilter("杭");
                                                    var picked1 = selection.PickObject(ObjectType.Element, selFilters,
                                                        statusPrompt: "1 点目の端点を選択"
                                                        ).GlobalPoint;

                                                    picked1 = CalcPerp(refPlane, picked1, view3d.GetOrientation().ForwardDirection);

                                                    // 端点.2 を選択
                                                    XYZ picked2;
                                                    do
                                                    {
                                                        picked2 = selection.PickObject(ObjectType.Element, selFilters,
                                                            statusPrompt: "2 点目の端点を選択"
                                                            ).GlobalPoint;
                                                        picked2 = CalcPerp(refPlane, picked2, view3d.GetOrientation().ForwardDirection);
                                                    } while (UtilAlgebra.Eq(picked1.DistanceTo(picked2), 0.0));
                                                    var picked1to2 = picked2 - picked1;

                                                    // ファミリロード
                                                    string strfPath = string.Empty;
                                                    var familyPath = string.Empty;
                                                    var familyTypeName = string.Empty;
                                                    var familyName = string.Empty;
                                                    if (!string.IsNullOrEmpty(ClsAngleCsv.ExistSize(fData.Size)))
                                                    {
                                                        strfPath = Master.ClsAngleCsv.GetFamilyPath(fData.Size);
                                                        familyPath = Directory.EnumerateFiles(PathUtil.GetYMSFolder(), Path.GetFileName(strfPath), SearchOption.AllDirectories).FirstOrDefault();
                                                        familyName = Path.GetFileNameWithoutExtension(familyPath);
                                                        familyTypeName = "ﾌﾞﾚｰｽ";
                                                    }
                                                    else
                                                    {
                                                        strfPath = Master.ClsTurnBackleCsv.GetFamilyPath(fData.Size);
                                                        familyPath = Directory.EnumerateFiles(PathUtil.GetYMSFolder(), Path.GetFileName(strfPath), SearchOption.AllDirectories).FirstOrDefault();
                                                        familyName = Path.GetFileNameWithoutExtension(familyPath);
                                                        familyTypeName = "垂直ﾌﾞﾚｰｽ背面";
                                                    }

                                                    if (GantryUtil.GetFamilySymbol(doc, familyPath, familyTypeName, out var symbol, true))
                                                    {
                                                        XYZ start, end;
                                                        if (fData.VrtBraceHasRound)
                                                        {
                                                            var originalLength = picked1.DistanceTo(picked2);
                                                            var roundedLength = (Math.Floor(originalLength / fData.RoundNumFt) + 1.0) * fData.RoundNumFt;

                                                            var diff = 0.5 * (roundedLength - originalLength);
                                                            var startToEnd = (picked2 - picked1).Normalize();

                                                            start = picked1 - diff * startToEnd;
                                                            end = picked2 + diff * startToEnd;
                                                        }
                                                        else
                                                        {
                                                            start = picked1;
                                                            end = picked2;
                                                        }

                                                        var id = MaterialSuper.PlaceWithTwoPoints(symbol, refPlaneElem.GetReference(), start, end, 0.0, noOffset: true);
                                                        if (id == ElementId.InvalidElementId)
                                                        {
                                                            _logger.Error("ブレースファミリインスタンスの配置に失敗");
                                                            continue;
                                                        }
                                                        var material = new VerticalBrace
                                                        {
                                                            m_KodaiName = fData.SelectedKodai,
                                                            m_Document = _doc,
                                                            m_ElementId = id,
                                                            m_Size = fData.Size,
                                                            m_TeiketsuType = fData.VrtBraceConnectingType == ConnectingType.Welding ? DefineUtil.eJoinType.Welding : fData.VrtBraceConnectingType == ConnectingType.Bolt ? DefineUtil.eJoinType.Bolt : DefineUtil.eJoinType.Support
                                                        };
                                                        MaterialSuper.WriteToElement(material, _doc);
                                                    }
                                                    else
                                                    {
                                                        _logger.Error($"family:\"{familyName}\", type:\"{familyTypeName}\" cannot load");
                                                    }
                                                }
                                                catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                        transaction.Commit();
                                    }
                                }
                                catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                                {

                                }

                            }
                            else if (fData.PlaceIsMultiple)//連続
                            {
                                while (true)
                                {
                                    transaction.Start(Guid.NewGuid().ToString());
                                    ////杭を指定
                                    XYZ p1 = new XYZ();
                                    if (!ClsRevitUtil.PickPoint(_uiDoc, "始点をピックしてください", ref p1))
                                    {
                                        break;
                                    }

                                    XYZ p2 = new XYZ();
                                    if (!ClsRevitUtil.PickPoint(_uiDoc, "終点をピックしてください", ref p2))
                                    {
                                        break;
                                    }

                                    if (!CreateBrace(p1, p2, fData))
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

        public bool CreateBrace(XYZ p1, XYZ p2, FrmPutVerticalBraceData viewModel)
        {

            ModelData md = ModelData.GetModelData(_doc);
            AllKoudaiFlatFrmData kodaiData = md.GetKoudaiData(viewModel.SelectedKodai).AllKoudaiFlatData;

            var baseLevel = new FilteredElementCollector(_doc)
                       .OfClass(typeof(Level))
                       .Cast<Level>()
                       .FirstOrDefault(x => x.Name == kodaiData.SelectedLevel);
            if (baseLevel == null)
            {
                MessageUtil.Error("構台の基準レベルが取得できません。", "error");
                return false;
            }

            var (origin, axisH, axisK, axisZ) = kodaiData.GetBasis(_doc);

            var pillarSize = ClsMasterCsv.Shared
    .Select(x => x as ClsHBeamCsv)
    .FirstOrDefault(x => x?.Size == kodaiData.pilePillerData.PillarSize.Replace("-", ""));
            var pillarSizeH = pillarSize?.HFt ?? 0.0;
            var pillarSizeB = pillarSize?.BFt ?? 0.0;

            string strfPath = string.Empty;
            var familyPath = string.Empty;
            var familyTypeName = string.Empty;
            var familyName = string.Empty;
            double dWidth = 0;
            if (!string.IsNullOrEmpty(ClsAngleCsv.ExistSize(viewModel.Size)))
            {
                strfPath = Master.ClsAngleCsv.GetFamilyPath(viewModel.Size);
                familyPath = Directory.EnumerateFiles(PathUtil.GetYMSFolder(), Path.GetFileName(strfPath), SearchOption.AllDirectories).FirstOrDefault();
                familyName = Path.GetFileNameWithoutExtension(familyPath);
                familyTypeName = "ﾌﾞﾚｰｽ";

                List<ClsAngleCsv> lst = ClsAngleCsv.GetCsvData();
                dWidth = (from data in lst where data.Size == viewModel.Size select data.AFt).FirstOrDefault();
            }
            else
            {
                strfPath = Master.ClsTurnBackleCsv.GetFamilyPath(viewModel.Size);
                familyPath = Directory.EnumerateFiles(PathUtil.GetYMSFolder(), Path.GetFileName(strfPath), SearchOption.AllDirectories).FirstOrDefault();
                familyName = Path.GetFileNameWithoutExtension(familyPath);
                familyTypeName = "垂直ﾌﾞﾚｰｽ背面";
            }

            var danData = viewModel.DanRowSet;

            // 最高大引下端の取得
            var highestOhbikiBottomFt = MaterialSuper
                .Collect(_doc)
                .Where(x => x.m_KodaiName == viewModel.SelectedKodai)
                .Select(x => x as Ohbiki)
                .Where(x => x != null)
                .Select(x => ClsRevitUtil.GetParameterDouble(_doc, x.m_ElementId, DefineUtil.PARAM_BASE_OFFSET) - 0.5 * (x.SteelSize?.HeightFeet ?? 0.0))
                .Max();
            var danTopSet = Enumerable
                .Range(0, danData.Count)
                .Select(x => danData.Take(x).Select(y => y.IntervalFt).Sum())
                .ToArray();

            var jointSizeH = 0.0;
            //水平つなぎのサイズ不明
            //if (viewModel.HrzJointSize is ClsChannelCsv channelRec)
            //{
            //    jointSizeH = channelRec.HFt;
            //}
            //else if (viewModel.HrzJointSize is ClsHBeamCsv hbeamRec)
            //{
            //    jointSizeH = hbeamRec.BFt;
            //}

            if (GantryUtil.GetFamilySymbol(_doc, familyPath, familyTypeName, out var symbol, true))
            {
                var insets = new List<BraceInsets>();

                var flags = new bool[]
                {
                                //viewModel.VrtBracePriorH,
                                //!viewModel.VrtBracePriorH,
                                true
                };
                for (int indexFlag = 0; indexFlag < flags.Length; indexFlag++)
                {
                    var preferH = flags.ElementAtOrDefault(indexFlag);
                    //var preferH = viewModel.VrtBracePriorH;
                    var preferred = indexFlag == 0;

                    var braceOffset = 0.0;
                    //var braceOffset = -0.5 * (preferH ? pillarSizeB : pillarSizeH);
                    var pillarHalfWidth = 0.5 * (preferH ? pillarSizeH : pillarSizeB);

                    // 勝ち側の杭座標
                    //var pillarsH = preferH ? pillarHSet : pillarKSet;
                    //var pillarsK = preferH ? pillarKSet : pillarHSet;
                    //var dirH = preferH ? axisH : axisK;
                    //var dirK = preferH ? axisK : axisH;
                    var dirH = (p2 - p1).Normalize();
                    // 
                    //for (int indexH = 0; indexH < pillarsH.Length - 1; indexH++)
                    //{

                    //}

                    //var startH = pillarsH.ElementAtOrDefault(indexH);
                    //var endH = pillarsH.ElementAtOrDefault(indexH + 1);

                    //var lengthH = Math.Abs(endH - startH);
                    var lengthH = p1.DistanceTo(p2);

                    //for (int indexK = 0; indexK < pillarsK.Length; indexK++)
                    //{

                    //}

                    //var pillarK = pillarsK.ElementAtOrDefault(indexK);

                    //var areaOriginKH = origin + startH * dirH + pillarK * dirK;

                    var areaOriginKH = p1;

                    var referenceSet = new Reference[]
                    {
                                        _doc.Create.NewReferencePlane(areaOriginKH, areaOriginKH + dirH, axisZ, _doc.ActiveView)?.GetReference(),
                                        _doc.Create.NewReferencePlane(areaOriginKH, areaOriginKH - dirH, axisZ, _doc.ActiveView)?.GetReference(),
                    };

                    for (int indexDan = 0; indexDan < danData.Count(); indexDan++)
                    {
                        var dan = danData.ElementAtOrDefault(indexDan);
                        if (dan == null) { continue; }

                        var isFirst = indexDan == 0;
                        var isLast = indexDan == danData.Count() - 1;

                        var upperX = isFirst ? viewModel.VrtBrace1UpperXFt : viewModel.VrtBrace2UpperXFt;
                        var upperY = isFirst ? viewModel.VrtBrace1UpperYFt : viewModel.VrtBrace2UpperYFt;
                        var lowerX = isFirst ? viewModel.VrtBrace1LowerXFt : viewModel.VrtBrace2LowerXFt;
                        var lowerY = isFirst ? viewModel.VrtBrace1LowerYFt : viewModel.VrtBrace2LowerYFt;

                        //if (!preferred)
                        //{
                        //    var targetInsets = insets.Where(x => x.K == indexH && x.H == indexK && x.D == indexDan).ToArray();
                        //    if (targetInsets.Any())
                        //    {
                        //        upperY += targetInsets.Select(x => x.Upper).Max();
                        //        lowerY += targetInsets.Select(x => x.Lower).Max();
                        //    }
                        //}

                        var elevation = baseLevel.ProjectElevation + highestOhbikiBottomFt - danTopSet.ElementAtOrDefault(indexDan);

                        var areaOrigin = new XYZ(areaOriginKH.X, areaOriginKH.Y, elevation);

                        var lengthZ = dan.IntervalFt - (isFirst || isLast ? 1.0 : 2.0) * jointSizeH;

                        var a = lengthH + 2.0 * upperX;
                        var b = lengthH + 2.0 * lowerX;
                        var d = dWidth;
                        var aplusb = a + b;

                        var c = lengthZ - upperY - lowerY;

                        var a2 = 0.25 * aplusb * aplusb + c * c;
                        var a1 = aplusb * d;
                        var a0 = d * d - c * c;

                        var roots = UtilAlgebra.SolveQuadraticEquation(a2, a1, a0);
                        var sinTheta = roots.FirstOrDefault(x => UtilAlgebra.IsReal(x) && x.Real > 0.0).Real;
                        var cosTheta = Math.Sqrt(1 - sinTheta * sinTheta);
                        var tanTheta = sinTheta / cosTheta;

                        var upperInset = (upperX + pillarHalfWidth) * tanTheta + d / cosTheta;
                        var lowerInset = (lowerX + pillarHalfWidth) * tanTheta + d / cosTheta;
                        insets.Add(new BraceInsets
                        {
                            //H = indexH,
                            //K = indexK,
                            D = indexDan,
                            Upper = upperInset,
                            Lower = lowerInset,
                        });
                        insets.Add(new BraceInsets
                        {
                            //H = indexH + 1,
                            //K = indexK,
                            D = indexDan,
                            Upper = upperInset,
                            Lower = lowerInset,
                        });

                        //var aa = areaOrigin + (upperX + a + 0.5 * d * sinTheta) * dirH - (upperY + 0.5 * d * cosTheta) * axisZ;
                        var braceOrigin = areaOrigin - (isFirst ? 0.0 : jointSizeH) * axisZ;
                        var braceStartSet = new XYZ[]
                        {
                                            braceOrigin + (-upperX + a) * dirH - (upperY) * axisZ,
                                            braceOrigin + (-upperX) * dirH - (upperY) * axisZ,
                        };
                        var braceEndSet = new XYZ[]
                        {
                                            braceOrigin + (-lowerX - d * sinTheta) * dirH - (upperY + c - d * cosTheta) * axisZ,
                                            braceOrigin + (-lowerX + b + d * sinTheta) * dirH - (upperY + c - d * cosTheta) * axisZ,
                        };

                        if (viewModel.DanRowSet.FirstOrDefault(x => x.Index == indexDan + 1)?.HasBrace != true) { continue; }

                        // 最下段に配置するか確認
                        //if (isLast && !viewModel.VrtBraceHasLowest) { continue; }

                        //if (!viewModel.VrtBraceHasH && preferH) { continue; }

                        //if (!viewModel.VrtBraceHasK && !preferH) { continue; }

                        for (int k = 0; k < 1; k++)
                        {
                            var braceStart = braceStartSet.ElementAtOrDefault(k);
                            var braceEnd = braceEndSet.ElementAtOrDefault(k);

                            var reference = referenceSet.ElementAtOrDefault(k);

                            XYZ start, end;
                            if (viewModel.VrtBraceHasRound)
                            {
                                var originalLength = braceStart.DistanceTo(braceEnd);
                                var roundedLength = (Math.Floor(originalLength / viewModel.RoundNumFt) + 1.0) * viewModel.RoundNumFt;

                                var diff = 0.5 * (roundedLength - originalLength);
                                var startToEnd = (braceEnd - braceStart).Normalize();

                                start = braceStart - diff * startToEnd;
                                end = braceEnd + diff * startToEnd;
                            }
                            else
                            {
                                start = braceStart;
                                end = braceEnd;
                            }

                            var id = MaterialSuper.PlaceWithTwoPoints(symbol, reference, start, end, braceOffset);
                            if (id == ElementId.InvalidElementId)
                            {
                                _logger.Error("ブレースファミリインスタンスの配置に失敗");
                                continue;
                            }
                            var material = new VerticalBrace
                            {
                                m_KodaiName = viewModel.SelectedKodai,
                                m_Document = _doc,
                                m_ElementId = id,
                                m_Size = viewModel.Size,
                                m_TeiketsuType = viewModel.VrtBraceConnectingType == ConnectingType.Welding ? DefineUtil.eJoinType.Welding : viewModel.VrtBraceConnectingType == ConnectingType.Bolt ? DefineUtil.eJoinType.Bolt : DefineUtil.eJoinType.Support
                            };
                            MaterialSuper.WriteToElement(material, _doc);
                        }
                    }

                }
            }

            return true;
        }

        public bool CreateTurnBackle(FrmPutVerticalBraceData fData)
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
                    string familyTypeName = "垂直ﾌﾞﾚｰｽ背面";

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



        //    public bool CreateBrace(XYZ p1, XYZ p2,double dOffset, Reference reference, FrmPutVerticalBraceData fData)
        //{
        //    try
        //    {

        //        //ブレースは配置点を部材中央部とする
        //        List<ClsAngleCsv> lst = ClsAngleCsv.GetCsvData();
        //        double dWidth = (from data in lst where data.Size == fData.Size select data.AFt).FirstOrDefault();
        //        XYZ vec1to2 = p2 - p1;
        //        XYZ targetVector = new XYZ(0, 0, 1);
        //        XYZ perpendicularVector = vec1to2.CrossProduct(targetVector).Normalize();
        //        p1 = p1 + (-perpendicularVector * dWidth * 0.5);
        //        p2 = p2 + (-perpendicularVector * dWidth * 0.5);

        //        var braceStart = p1;
        //        var braceEnd = p2;
        //        var levelOffset = dOffset;
        //        //var reference = reference;// baseLevel.GetPlaneReference();

        //        string strfPath = Master.ClsAngleCsv.GetFamilyPath(fData.Size);
        //        var familyPath = Directory.EnumerateFiles(PathUtil.GetYMSFolder(), Path.GetFileName(strfPath), SearchOption.AllDirectories).FirstOrDefault();
        //        var familyName = Path.GetFileNameWithoutExtension(familyPath);
        //        var familyTypeName = "ﾌﾞﾚｰｽ";

        //        if (!GantryUtil.GetFamilySymbol(_doc, familyPath, familyTypeName, out var symbol, true))
        //        {
        //            return false;
        //        }

        //            var id = MaterialSuper.PlaceWithTwoPoints(symbol, reference, braceStart, braceEnd, levelOffset);
        //        if (id == ElementId.InvalidElementId)
        //        {
        //            _logger.Error("ツナギ材ファミリインスタンスの配置に失敗");
        //            return false;
        //        }
        //        var material = new VerticalBrace
        //        {
        //            m_KodaiName = fData.SelectedKodai,
        //            m_Document = _doc,
        //            m_ElementId = id,
        //            m_Size = fData.Size
        //        };
        //        MaterialSuper.WriteToElement(material, _doc);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error(ex.Message);
        //        return false;
        //    }
        //}
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
        static ReferencePlane NegativePlane(Document doc, Level baseLevel)
        {
            var elevation = baseLevel.Elevation;
            var origin = XYZ.Zero + elevation * XYZ.BasisZ;
            return doc.Create.NewReferencePlane(origin, origin + XYZ.BasisY, -XYZ.BasisX, doc.ActiveView);
        }

        static ReferencePlane PositivePlane(Document doc, Level baseLevel)
        {
            var elevation = baseLevel.Elevation;
            var origin = XYZ.Zero + elevation * XYZ.BasisZ;
            return doc.Create.NewReferencePlane(origin, origin + XYZ.BasisY, XYZ.BasisX, doc.ActiveView);
        }
    }
    class BraceInsets
    {
        public int K { get; set; }
        public int H { get; set; }
        public int D { get; set; }
        public double Upper { get; set; }
        public double Lower { get; set; }
    }

    class FilterRefPlane : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is ReferencePlane) { return true; }
            //else if (elem is Level) { return true; }
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
    }
}

