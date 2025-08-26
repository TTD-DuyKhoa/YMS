using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YMS_gantry.Data;
using YMS_gantry.Material;
using YMS_gantry.UI;

namespace YMS_gantry.Command
{
    class CmdCreateJifukuZuredome
    {
        static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        Document _doc { get; set; }
        UIDocument _uiDoc { get; set; }

        public CmdCreateJifukuZuredome(UIDocument uiDoc)
        {
            _uiDoc = uiDoc;
            _doc = _uiDoc.Document;
        }

        /// <summary>
        /// 実行
        /// </summary>
        /// <returns></returns>
        public Result Excute()
        {
            try
            {
                //FrmPutJifukuFukkoubanZuredomezai frm = new FrmPutJifukuFukkoubanZuredomezai(_uiDoc.Application);
                //if (frm.ShowDialog() != System.Windows.Forms.DialogResult.OK) { return Result.Cancelled; }
                FrmPutJifukuFukkoubanZuredomezai frm = Application.thisApp.frmPutJifukuFukkoubanZuredomezai;
                Selection selection = _uiDoc.Selection;

                //構台の最大根太Index
                AllKoudaiFlatFrmData kData = GantryUtil.GetKoudaiData(_doc, frm.CmbKoudaiName.Text).AllKoudaiFlatData;
                string type = JifukuZuredomezai.typeJifukuName;
                if (kData.KoujiType == DefineUtil.eKoujiType.Kenchiku || frm.CmbSizeType.Text == Master.ClsJifukuZuredomeCsv.TypeL)
                {
                    type = JifukuZuredomezai.typeZuredomeName;
                }
                int maxHInd = kData.NedaPitch.Count - 1;
                int maxKInd = kData.KyoutyouPillarPitch.Count - 1;
                var koudaiCor = kData.GetCoordinate(_doc);
                bool isChannel = frm.CmbSizeType.Text.Equals(Master.ClsMasterCsv.TypeC);
                MaterialSize targetSize = GantryUtil.GetKouzaiSize(frm.CmbSize.Text);
                double cOffset = isChannel ? targetSize.Height / 2 : 0;
                double cRotate = isChannel ? 90 : 0;

                JifukuZuredomezai jifuku = new JifukuZuredomezai();
                jifuku.m_KodaiName = frm.CmbKoudaiName.Text;
                jifuku.m_Material = frm.CmbMaterial.Text;
                jifuku.m_Size = frm.CmbSize.Text;

                //自動配置(根太端部のみの自動配置)
                if (frm.RbtAuto.Checked)
                {
                    double length = (double)frm.NmcZuredomeLeng.Value;
                    if (frm.CmbSizeType.Text == Master.ClsJifukuZuredomeCsv.TypeL)
                    {
                        length = getNekoLength(frm.CmbSize.Text);
                    }

                    //範囲選択で根太を選択させる
                    YMSPickFilter pickFilter = new YMSPickFilter(_uiDoc.Document, new List<string> { "根太", "主桁" });
                    List<Element> nedaPicks = selection.PickElementsByRectangle(pickFilter, "対象とする根太を範囲選択してください").ToList();
                    List<ElementId> ids = nedaPicks.Select(x => x.Id).ToList();
                    MaterialSuper[] materials = MaterialSuper.Collect(_doc).Where(x => ids.Contains(x.m_ElementId) && x.m_KodaiName == jifuku.m_KodaiName).ToArray();
                    if (materials.Length == 0)
                    {
                        MessageUtil.Information("指定した構台の根太が選択されていません", "地覆・覆工板ズレ止メ");
                        return Result.Cancelled;
                    }

                    Neda[] startNeda = materials.Select(x => x as Neda).Where(x => x != null && x.m_MinK == 0).ToArray();
                    Neda[] endNeda = materials.Select(x => x as Neda).Where(x => x != null && x.m_MaxK == maxKInd).ToArray();
                    if (startNeda.Length == 0 && endNeda.Length == 0)
                    {
                        MessageUtil.Information("端部の根太が含まれていませんでした", "地覆・覆工板ズレ止メ");
                        return Result.Cancelled;
                    }

                    using (Transaction tr = new Transaction(_doc))
                    {
                        tr.Start("JifukuPlacement");
                        foreach (Neda neda in startNeda)
                        {
                            FamilyInstance ins = _doc.GetElement(neda.m_ElementId) as FamilyInstance;
                            Transform ts = ins.GetTransform();
                            Face f = GantryUtil.GetTopFaceOfFamilyInstance(ins);
                            var pnts = GantryUtil.GetCurvePoints(_doc, neda.m_ElementId);
                            pnts = pnts.OrderBy(x => x.DistanceTo(koudaiCor.Origin)).ToList();
                            XYZ vecLine = (pnts[1] - pnts[0]).Normalize();
                            XYZ vecCross = ts.BasisY.Normalize();
                            vecCross = (RevitUtil.ClsGeo.IsLeft(vecLine, vecCross)) ? vecCross.Negate().Normalize() : vecCross;
                            double height = neda.MaterialSize().Height;
                            XYZ point1 = pnts[0] + vecLine * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(neda.m_ExStartLen) +
                                               ts.BasisZ * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(cOffset + (height / 2)) +
                                               vecCross * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(length / 2);
                            XYZ point2 = point1 + vecCross.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(length);
                            jifuku.CreateJifuku(jifuku, _doc, type, f.Reference, point1, point2, cOffset, cRotate);
                        }

                        foreach (Neda neda in endNeda)
                        {
                            FamilyInstance ins = _doc.GetElement(neda.m_ElementId) as FamilyInstance;
                            Transform ts = ins.GetTransform();
                            Face f = GantryUtil.GetTopFaceOfFamilyInstance(ins);
                            var pnts = GantryUtil.GetCurvePoints(_doc, neda.m_ElementId);
                            pnts = pnts.OrderByDescending(x => x.DistanceTo(koudaiCor.Origin)).ToList();
                            XYZ vecLine = (pnts[1] - pnts[0]).Normalize();
                            XYZ vecCross = ts.BasisY.Normalize();
                            vecCross = (RevitUtil.ClsGeo.IsLeft(vecLine, vecCross)) ? vecCross.Negate().Normalize() : vecCross;
                            double height = neda.MaterialSize().Height;
                            XYZ point1 = pnts[0] + vecLine * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(neda.m_ExEndLen) +
                                               ts.BasisZ * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(cOffset + (height / 2)) +
                                               vecCross * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(length / 2);
                            XYZ point2 = point1 + vecCross.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(length);
                            jifuku.CreateJifuku(jifuku, _doc, type, f.Reference, point1, point2, cOffset, cRotate);
                        }
                        tr.Commit();
                    }

                }
                //全周配置
                else if (frm.RbtAutoAll.Checked)
                {

                    MaterialSuper[] materials = MaterialSuper.Collect(_doc).Where(x => x.m_KodaiName == jifuku.m_KodaiName).ToArray();

                    //始点側端部にある根太
                    Neda[] startNeda = materials.Select(x => x as Neda).Where(x => x != null && x.m_MinK == 0).OrderBy(x => x.m_H).ToArray();
                    //終点側端部にある根太
                    Neda[] endNeda = materials.Select(x => x as Neda).Where(x => x != null && x.m_MaxK == maxKInd).OrderBy(x => x.m_H).ToArray();
                    //構台左側にある根太
                    Neda[] leftNeda = materials.Select(x => x as Neda).Where(x => x != null && x.m_H == 0).OrderBy(x => x.m_MinK).ToArray();
                    //構台右側にある根太
                    Neda[] rightNeda = materials.Select(x => x as Neda).Where(x => x != null && x.m_H == maxHInd).OrderBy(x => x.m_MinK).ToArray();

                    using (Transaction tr = new Transaction(_doc))
                    {
                        tr.Start("JifukuPlacement");
                        //始点側
                        if (startNeda.Length > 1)
                        {
                            FamilyInstance ins = _doc.GetElement(startNeda.Last().m_ElementId) as FamilyInstance;
                            Transform ts = ins.GetTransform();
                            Face f = GantryUtil.GetTopFaceOfFamilyInstance(ins);
                            var pntsF = GantryUtil.GetCurvePoints(_doc, startNeda.Last().m_ElementId);
                            pntsF = pntsF.OrderBy(x => x.DistanceTo(koudaiCor.Origin)).ToList();
                            XYZ vecLine = (pntsF[1] - pntsF[0]).Normalize();
                            XYZ vecCross = ts.BasisY.Normalize();
                            vecCross = (RevitUtil.ClsGeo.IsLeft(vecLine, vecCross)) ? vecCross.Negate().Normalize() : vecCross;
                            double height = startNeda.Last().MaterialSize().Height;
                            var pntsE = GantryUtil.GetCurvePoints(_doc, startNeda.First().m_ElementId);
                            pntsE = pntsE.OrderBy(x => x.DistanceTo(koudaiCor.Origin)).ToList();
                            double length = targetSize.Width;

                            XYZ point1 = pntsF[0] + vecLine * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(startNeda.Last().m_ExStartLen) +
                                               ts.BasisZ * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(cOffset + (height / 2)) +
                                               vecCross * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(length);
                            XYZ point2 = pntsE[0] + vecLine * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(startNeda.Last().m_ExStartLen) +
                                             ts.BasisZ * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(cOffset + (height / 2)) +
                                             vecCross.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(length);
                            jifuku.CreateJifuku(jifuku, _doc, type, f.Reference, point1, point2, cOffset, cRotate);
                        }
                        //終点側
                        if (endNeda.Length > 1)
                        {
                            FamilyInstance ins = _doc.GetElement(endNeda.First().m_ElementId) as FamilyInstance;
                            Transform ts = ins.GetTransform();
                            Face f = GantryUtil.GetTopFaceOfFamilyInstance(ins);
                            var pntsF = GantryUtil.GetCurvePoints(_doc, endNeda.First().m_ElementId);
                            pntsF = pntsF.OrderByDescending(x => x.DistanceTo(koudaiCor.Origin)).ToList();
                            XYZ vecLine = (pntsF[1] - pntsF[0]).Normalize();
                            XYZ vecCross = ts.BasisY.Normalize();
                            vecCross = (RevitUtil.ClsGeo.IsLeft(vecLine, vecCross)) ? vecCross.Negate().Normalize() : vecCross;
                            double height = endNeda.Last().MaterialSize().Height;
                            var pntsE = GantryUtil.GetCurvePoints(_doc, endNeda.Last().m_ElementId);
                            pntsE = pntsE.OrderByDescending(x => x.DistanceTo(koudaiCor.Origin)).ToList();
                            double length = targetSize.Width;

                            XYZ point1 = pntsF[0] + vecLine * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(endNeda.Last().m_ExEndLen) +
                                               ts.BasisZ * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(cOffset + (height / 2)) +
                                               vecCross * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(length);
                            XYZ point2 = pntsE[0] + vecLine * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(endNeda.Last().m_ExEndLen) +
                                             ts.BasisZ * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(cOffset + (height / 2)) +
                                             vecCross.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(length);
                            jifuku.CreateJifuku(jifuku, _doc, type, f.Reference, point1, point2, cOffset, cRotate);
                        }
                        //左側
                        if (leftNeda.Length >= 1)
                        {
                            for (int i = 0; i < leftNeda.Length; i++)
                            {
                                FamilyInstance ins = _doc.GetElement(leftNeda[i].m_ElementId) as FamilyInstance;
                                Transform ts = ins.GetTransform();
                                bool same = true; int cnt = i + 1;
                                FamilyInstance nextins = ins;
                                while (same && cnt < leftNeda.Length)
                                {
                                    Transform nextts = (_doc.GetElement(leftNeda[cnt].m_ElementId) as FamilyInstance).GetTransform();
                                    if (nextts.BasisZ == ts.BasisZ)
                                    {
                                        nextins = _doc.GetElement(leftNeda[cnt].m_ElementId) as FamilyInstance;
                                        cnt++;
                                        i++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                Face f = GantryUtil.GetTopFaceOfFamilyInstance(ins);
                                var pntsF = GantryUtil.GetCurvePoints(_doc, ins.Id);
                                pntsF = pntsF.OrderBy(x => x.DistanceTo(koudaiCor.Origin)).ToList();
                                XYZ vecLine = (pntsF[1] - pntsF[0]).Normalize();
                                double height = leftNeda.Where(x => x.m_ElementId == ins.Id).First().MaterialSize().Height;
                                var pntsE = GantryUtil.GetCurvePoints(_doc, nextins.Id);
                                pntsE = pntsE.OrderBy(x => x.DistanceTo(koudaiCor.Origin)).ToList();
                                double length = targetSize.Width;
                                XYZ point1 = pntsF[0] + ts.BasisZ * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(cOffset + (height / 2)); ;
                                point1 = (ins.Id == leftNeda.First().m_ElementId) ? point1 + vecLine * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(leftNeda.First().m_ExStartLen) : point1;
                                XYZ point2 = pntsE[1] + ts.BasisZ * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(cOffset + (height / 2));
                                point2 = (nextins.Id == leftNeda.Last().m_ElementId) ? point2 + vecLine.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(leftNeda.Last().m_ExEndLen) : point2;
                                jifuku.CreateJifuku(jifuku, _doc, type, f.Reference, point1, point2, cOffset, cRotate);
                            }
                        }
                        //右側
                        if (rightNeda.Length >= 1)
                        {
                            for (int i = 0; i < rightNeda.Length; i++)
                            {
                                FamilyInstance ins = _doc.GetElement(rightNeda[i].m_ElementId) as FamilyInstance;
                                Transform ts = ins.GetTransform();
                                bool same = true; int cnt = i + 1;
                                FamilyInstance nextins = ins;
                                while (same && cnt < rightNeda.Length)
                                {
                                    Transform nextts = (_doc.GetElement(rightNeda[cnt].m_ElementId) as FamilyInstance).GetTransform();
                                    if (nextts.BasisZ == ts.BasisZ)
                                    {
                                        nextins = _doc.GetElement(rightNeda[cnt].m_ElementId) as FamilyInstance;
                                        cnt++;
                                        i++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                Face f = GantryUtil.GetTopFaceOfFamilyInstance(nextins);
                                var pntsF = GantryUtil.GetCurvePoints(_doc, nextins.Id);
                                pntsF = pntsF.OrderByDescending(x => x.DistanceTo(koudaiCor.Origin)).ToList();
                                XYZ vecLine = (pntsF[1] - pntsF[0]).Normalize();
                                double height = rightNeda.Where(x => x.m_ElementId == nextins.Id).First().MaterialSize().Height;
                                var pntsE = GantryUtil.GetCurvePoints(_doc, ins.Id);
                                pntsE = pntsE.OrderByDescending(x => x.DistanceTo(koudaiCor.Origin)).ToList();
                                double length = targetSize.Width;
                                XYZ point1 = pntsF[0] + ts.BasisZ * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(cOffset + (height / 2)); ;
                                point1 = (nextins.Id == rightNeda.Last().m_ElementId) ? point1 + vecLine * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(rightNeda.Last().m_ExEndLen) : point1;
                                XYZ point2 = pntsE[1] + ts.BasisZ * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(cOffset + (height / 2));
                                point2 = (ins.Id == rightNeda.First().m_ElementId) ? point2 + vecLine.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(rightNeda.First().m_ExStartLen) : point2;
                                jifuku.CreateJifuku(jifuku, _doc, type, f.Reference, point1, point2, cOffset, cRotate);
                            }
                        }

                        tr.Commit();
                    }

                }
                //面配置
                else if (frm.RbtFace.Checked)
                {
                    if (frm.CmbSizeType.Text == Master.ClsJifukuZuredomeCsv.TypeL)
                    {
                        type = Master.ClsJifukuZuredomeCsv.TypeZuredomezai;
                        string size = frm.CmbSize.Text;
                        using (TransactionGroup tG = new TransactionGroup(_doc))
                        {
                            FamilySymbol sym = null;
                            string path = Master.ClsJifukuZuredomeCsv.GetFamilyPath(type, size);
                            if (!GantryUtil.GetFamilySymbol(_doc, path, type, out sym, false))
                            {
                                _logger.Error($"ファミリロードに失敗しました。\r{path}");
                                return Result.Cancelled; ;
                            }


                            using (Transaction tr = new Transaction(_doc))
                            {
                                GantryUtil util = new GantryUtil();
                                //配置

                                List<ElementId> ids = util.PlaceFamilyInstance(_uiDoc.Application, sym, false);
                                tr.Start("TeiketsuRalated");
                                foreach (ElementId id in ids)
                                {
                                    jifuku.m_ElementId = id;
                                    JifukuZuredomezai.WriteToElement(jifuku, _doc);
                                }
                                tr.Commit();
                            }
                            tG.Assimilate();
                        }
                    }
                    else
                    {
                        //配置基準面選択
                        Reference refer = selection.PickObject(ObjectType.Face, "配置基準とする面を選択してください)");
                        Element element = _doc.GetElement(refer);
                        Face face = element.GetGeometryObjectFromReference(refer) as Face;
                        if (face == null)
                        {
                            return Result.Cancelled;
                        }

                        using (Transaction tr = new Transaction(_doc))
                        {
                            tr.Start("Fukkouban placement");
                            double offset = (double)frm.NmcOffset.Value;
                            try
                            {   //2点指定
                                XYZ p1 = selection.PickPoint("1点目を指定してください");
                                XYZ p2 = selection.PickPoint("2点目を指定してください");
                                jifuku.CreateJifuku(jifuku, _doc, type, refer, p1, p2, cOffset, cRotate, true);
                            }
                            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                            {
                                return Result.Cancelled;
                            }
                            tr.Commit();
                        }
                    }
                }
                //自由位置配置
                else if (frm.RbtFree.Checked)
                {
                    Level level = ClsRevitUtil.GetLevel(_doc, frm.CmbLevel.Text) as Level;
                    double offset = (double)frm.NmcOffset.Value;
                    string familyPath = Master.ClsJifukuZuredomeCsv.GetFamilyPath(jifuku.m_Size);
                    FamilySymbol sym;
                    using (TransactionGroup trg = new TransactionGroup(_doc))
                    {
                        trg.Start("Fukkouban placement");
                        using (Transaction tr = new Transaction(_doc))
                        {
                            tr.Start("Load Family");
                            if (!GantryUtil.GetFamilySymbol(_doc, familyPath, type, out sym, true))
                            {
                                return Result.Cancelled;
                            }
                            if (frm.CmbSize.Text.StartsWith("["))
                            {
                                sym = GantryUtil.DuplicateTypeWithNameRule(_doc, frm.CmbKoudaiName.Text, sym, type);
                            }
                            Dictionary<string, string> paramList = new Dictionary<string, string>();
                            paramList.Add(DefineUtil.PARAM_MATERIAL, jifuku.m_Material);
                            //paramList.Add("サイズ", fg.m_Size);
                            paramList.Add(DefineUtil.PARAM_ROTATE, $"{cRotate / (180 / Math.PI)}");
                            //sym = GantryUtil.ChangeTypeID(_doc, sym, GantryUtil.CreateTypeName(MaterialSuper.typeName, paramList));
                            //タイプパラメータ設定
                            foreach (KeyValuePair<string, string> kv in paramList)
                            {
                                GantryUtil.SetParameterValueByParameterName(sym, kv.Key, kv.Value);
                            }
                            tr.Commit();
                        }

                        ElementId newId = ElementId.InvalidElementId;
                        if (frm.CmbSizeType.Text == Master.ClsJifukuZuredomeCsv.TypeL)
                        {
                            try
                            {
                                if (frm.CmbLevel.Text == "部材選択")
                                {
                                    //部材を指定
                                    ElementId selId = selection.PickObject(ObjectType.Element, "配置基準となる部材を選択してください").ElementId;
                                    FamilyInstance ins = _uiDoc.Document.GetElement(selId) as FamilyInstance;
                                    if (ins == null)
                                    {
                                        MessageUtil.Warning("指定した部材のホストに設定されたレベルが取得できません\r\nホストがレベルの部材を選択してください", "地覆・ズレ止め材配置");
                                        return Result.Cancelled;
                                    }

                                    double insOff = 0;
                                    level = GantryUtil.GetInstanceLevelAndOffset(_uiDoc.Document, ins, ref insOff);
                                    if (level == null)
                                    {
                                        MessageUtil.Warning("指定した部材のホストに設定されたレベルが取得できません\r\nホストがレベルの部材を選択してください", "地覆・ズレ止め材配置");
                                        return Result.Cancelled;
                                    }
                                }
                                XYZ origin, sidePnt, vecSide, headPnt, orthogolanVec;
                                ElementId pointId = ElementId.InvalidElementId, LineId = ElementId.InvalidElementId;
                                //配置基準＋幅員、橋長方向取得
                                origin = selection.PickPoint(ObjectSnapTypes.Nearest, "挿入の基点を指定");
                                pointId = GantryUtil.InsertPointFamily(_doc, origin, level);

                                //幅員方向ベクトル
                                sidePnt = selection.PickPoint(ObjectSnapTypes.Nearest, "長辺方向を指定");
                                vecSide = (sidePnt - origin).Normalize();
                                LineId = GantryUtil.InsertLineFamily(_doc, origin, sidePnt, level);

                                //橋軸方向ベクトル
                                headPnt = selection.PickPoint(ObjectSnapTypes.Nearest, "アングルの向きを指定");

                                using (Transaction tr = new Transaction(_doc))
                                {
                                    tr.Start("FlatSettings");
                                    if (pointId != null && pointId != ElementId.InvalidElementId)
                                    {
                                        FamilyInstance inP = _doc.GetElement(pointId) as FamilyInstance;
                                        if (inP != null)
                                        {
                                            _doc.Delete(pointId);
                                        }
                                    }
                                    if (LineId != null && LineId != ElementId.InvalidElementId)
                                    {
                                        FamilyInstance inL = _doc.GetElement(LineId) as FamilyInstance;
                                        if (inL != null)
                                        {
                                            _doc.Delete(LineId);
                                        }
                                    }

                                    XYZ vecHead = (headPnt - origin).Normalize();
                                    orthogolanVec = vecHead.CrossProduct(XYZ.BasisZ);
                                    if (!RevitUtil.ClsGeo.IsLeft(vecHead,orthogolanVec))
                                    {
                                        orthogolanVec = orthogolanVec.Negate().Normalize();
                                    }


                                    var newIns = GantryUtil.CreateInstanceWith1point(_doc, origin, level.GetPlaneReference(), sym, orthogolanVec);

                                    if (newIns != null)
                                    {
                                        newId = newIns.Id;
                                        if (offset != 0)
                                        {
                                            RevitUtil.ClsRevitUtil.SetParameter(_uiDoc.Document, newId, DefineUtil.PARAM_BASE_OFFSET, offset);
                                        }
                                        if (newId != ElementId.InvalidElementId || newId != null)
                                        {
                                            jifuku.m_ElementId = newId;
                                            JifukuZuredomezai.WriteToElement(jifuku, _doc);
                                        }
                                    }
                                    tr.Commit();

                                }
                            }
                            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                            {
                            }

                        }
                        else
                        {
                            using (Transaction tr = new Transaction(_doc))
                            {
                                tr.Start("Load Family");
                                try
                                {
                                    if (frm.CmbLevel.Text == "部材選択")
                                    {
                                        newId = GantryUtil.PlaceSymbolOverTheSelectedElm(_uiDoc, sym, frm.CmbKoudaiName.Text, level, offset, functionName: "地覆・ズレ止め材配置");
                                    }
                                    else
                                    {
                                        XYZ p1 = selection.PickPoint("1点目を指定してください");
                                        XYZ p2 = selection.PickPoint("2点目を指定してください");
                                        p1 = new XYZ(p1.X, p1.Y, level.Elevation);
                                        p2 = new XYZ(p2.X, p2.Y, level.Elevation);
                                        newId = MaterialSuper.PlaceWithTwoPoints(sym, level.GetPlaneReference(), p1, p2, RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(offset));
                                    }
                                    if (newId != ElementId.InvalidElementId || newId != null)
                                    {
                                        jifuku.m_ElementId = newId;
                                        JifukuZuredomezai.WriteToElement(jifuku, _doc);
                                    }
                                    tr.Commit();
                                }
                                catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                                {
                                }
                            }
                        }
                        trg.Assimilate();
                    }
                }
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return Result.Failed;
            }
        }
        /// <summary>
        /// 文字列からネコ材の長さを返す
        /// </summary>
        /// <param name="nekoSize"></param>
        /// <returns></returns>
        private double getNekoLength(string nekoSize)
        {
            string leng = nekoSize.Substring(nekoSize.IndexOf('-') + 1, 2);
            return RevitUtil.ClsCommonUtils.ChangeStrToDbl(leng) * 10;
        }
    }
}
