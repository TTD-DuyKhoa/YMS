using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YMS_gantry.UI;
using static YMS_gantry.DefineUtil;
using YMS_gantry.Material;

namespace YMS_gantry.Command
{
    class CmdCreateFukkouban
    {
        static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        Document _doc { get; set; }
        UIDocument _uiDoc { get; set; }

        public CmdCreateFukkouban(UIDocument uiDoc)
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
            bool retBool = true;
            try
            {
                FrmPutFukkouban f = Application.thisApp.frmPutFukkouban;
                Selection selection = _uiDoc.Selection;
                //面選択
                if (f.RbtFace.Checked)
                {
                    //配置基準面選択
                    Reference refer = selection.PickObject(ObjectType.Face, "配置基準とする面を選択してください)");
                    Element element = _doc.GetElement(refer);
                    Face face = element.GetGeometryObjectFromReference(refer) as Face;
                    face.ComputeNormal(UV.Zero);
                    if (face == null)
                    {
                        return Result.Cancelled;
                    }

                    using (Transaction tr = new Transaction(_doc))
                    {
                        tr.Start("Fukkouban placement");
                        XYZ origin, hukuinPnt, vecHukuin, kyoutyouPnt, orthogolanVec;

                        ElementId pointId = ElementId.InvalidElementId, LineId = ElementId.InvalidElementId;
                        double offset = (double)f.NmcOffset.Value;
                        try
                        {   //配置基準＋幅員、橋長方向取得
                            origin = selection.PickPoint(ObjectSnapTypes.Nearest, "挿入の基点を指定");
                            pointId = GantryUtil.InsertPointFamily(_doc, origin, refer, true);
                            _doc.Regenerate();

                            //幅員方向ベクトル
                            hukuinPnt = selection.PickPoint(ObjectSnapTypes.Nearest, "幅員方向を指定");
                            LineId = GantryUtil.InsertLineFamily(_doc, origin, hukuinPnt, refer, true);
                            _doc.Regenerate();

                            vecHukuin = (hukuinPnt - origin).Normalize();
                            //橋軸方向ベクトル
                            kyoutyouPnt = selection.PickPoint(ObjectSnapTypes.Nearest, "橋長方向を指定");
                            orthogolanVec = vecHukuin.CrossProduct(XYZ.BasisZ);

                            if (pointId != null && pointId != ElementId.InvalidElementId)
                            {
                                FamilyInstance ins = _doc.GetElement(pointId) as FamilyInstance;
                                if (ins != null)
                                {
                                    _doc.Delete(pointId);
                                }
                            }
                            if (LineId != null && LineId != ElementId.InvalidElementId)
                            {
                                FamilyInstance ins = _doc.GetElement(LineId) as FamilyInstance;
                                if (ins != null)
                                {
                                    _doc.Delete(LineId);
                                }
                            }

                            XYZ vecKyoutyou = (kyoutyouPnt - origin).Normalize();
                            if (RevitUtil.ClsGeo.GEO_GE0(orthogolanVec.DotProduct(vecKyoutyou)))
                            {
                                vecKyoutyou = orthogolanVec.Normalize();
                            }
                            else
                            {
                                vecKyoutyou = orthogolanVec.Negate().Normalize();
                            }

                            bool isLeft = RevitUtil.ClsGeo.IsLeft(vecHukuin, vecKyoutyou);

                            double adjustAngle = XYZ.BasisX.AngleTo(vecHukuin) * (RevitUtil.ClsGeo.GEO_LT0(vecHukuin.Y) ? -1 : 1);
                            int hukuinCnt = (int)f.NmcHukuinCnt.Value; int kyoutyouCnt = (int)f.NmcKyoutyouCnt.Value;
                            double w = RevitUtil.ClsCommonUtils.ChangeStrToDbl(f.CmbSize.Text.Substring(f.CmbSize.Text.LastIndexOf("X") + 1));
                            double hukuinSize = w!=0?w:(f.CmbSize.Text.EndsWith("2000")) ? 2000 : 3000;
                            List<XYZ> FukkouList = calcFukkoubanPlacement(origin, vecHukuin, vecKyoutyou, hukuinCnt, kyoutyouCnt, hukuinSize);
                            XYZ adVec = (!isLeft) ? vecKyoutyou.Normalize() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(1000) : XYZ.Zero;
                            foreach (XYZ pnt in FukkouList)
                            {
                                DefineUtil.eFukkoubanType hType = (hukuinSize==2000) ? eFukkoubanType.TwoM : eFukkoubanType.Three;
                                ElementId idnew = Fukkouban.CreateFukkoubanOnFace(_doc, f.CmbMaterial.Text, pnt, hType, f.CmbSize.Text, adjustAngle, refer, vecHukuin, adVec);
                                //個別情報追加
                                Fukkouban fukkouban = new Fukkouban();
                                fukkouban.m_ElementId = idnew;
                                fukkouban.m_KodaiName = f.CmbKoudaiName.Text;
                                fukkouban.m_Size = f.CmbSize.Text;
                                fukkouban.m_Material = f.CmbMaterial.Text;
                                Fukkouban.WriteToElement(fukkouban, _doc);
                            }
                            tr.Commit();

                        }
                        catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                        {
                            return Result.Cancelled;
                        }
                        catch (Exception ex)
                        {
                            MessageUtil.Error("覆工板個別配置に失敗しました", "覆工板個別配置");
                            _logger.Error(ex.Message);
                            return Result.Failed;
                        }
                    }
                }
                else
                {
                    //自由位置
                    //基準レベル
                    Level baselevel = null;
                    double offset = (double)f.NmcOffset.Value;
                    if (f.CmbLevel.Text == "部材選択")
                    {
                        ElementId id = selection.PickObject(ObjectType.Element, "基準となる部材を選択してください").ElementId;
                        FamilyInstance ins = _doc.GetElement(id) as FamilyInstance;
                        baselevel = GantryUtil.GetInstanceLevelAndOffset(_doc, ins, ref offset);
                        //double topZ = GantryUtil.GetTopElevationOfFamilyInstance(ins);
                        //offset = topZ - baselevel.Elevation;
                        if (baselevel == null) {
                            MessageUtil.Warning("指定した部材のホストに設定されたレベルが取得できません\r\nホストがレベルの部材を選択してください", "覆工板配置");
                            return Result.Cancelled; 
                        }
                        if(f.NmcOffset.Value>0)
                        {
                            offset += (double)f.NmcOffset.Value;
                        }
                    }
                    else
                    {
                        baselevel = RevitUtil.ClsRevitUtil.GetLevel(_doc, f.CmbLevel.Text) as Level;
                    }
                    offset -= DefineUtil.FukkouBAN_THICK;

                    XYZ origin, hukuinPnt, vecHukuin, kyoutyouPnt, orthogolanVec;
                    ElementId pointId = ElementId.InvalidElementId, LineId = ElementId.InvalidElementId;
                    try
                    {
                        //配置基準＋幅員、橋長方向取得
                        origin = selection.PickPoint(ObjectSnapTypes.Nearest, "挿入の基点を指定");
                        pointId = GantryUtil.InsertPointFamily(_doc, origin, baselevel);

                        //幅員方向ベクトル
                        hukuinPnt = selection.PickPoint(ObjectSnapTypes.Nearest, "幅員方向を指定");
                        vecHukuin = (hukuinPnt - origin).Normalize();
                        LineId = GantryUtil.InsertLineFamily(_doc, origin, hukuinPnt, baselevel);

                        //橋軸方向ベクトル
                        kyoutyouPnt = selection.PickPoint(ObjectSnapTypes.Nearest, "橋長方向を指定");
                        orthogolanVec = vecHukuin.CrossProduct(XYZ.BasisZ);

                        using (Transaction tr = new Transaction(_doc))
                        {
                            tr.Start("FlatSettings");
                            if (pointId != null && pointId != ElementId.InvalidElementId)
                            {
                                FamilyInstance ins = _doc.GetElement(pointId) as FamilyInstance;
                                if (ins != null)
                                {
                                    _doc.Delete(pointId);
                                }
                            }
                            if (LineId != null && LineId != ElementId.InvalidElementId)
                            {
                                FamilyInstance ins = _doc.GetElement(LineId) as FamilyInstance;
                                if (ins != null)
                                {
                                    _doc.Delete(LineId);
                                }
                            }
                            XYZ vecKyoutyou = (kyoutyouPnt - origin).Normalize();
                            if (RevitUtil.ClsGeo.GEO_GE0(orthogolanVec.DotProduct(vecKyoutyou)))
                            {
                                vecKyoutyou = orthogolanVec.Normalize();
                            }
                            else
                            {
                                vecKyoutyou = orthogolanVec.Negate().Normalize();
                            }

                            bool isLeft = RevitUtil.ClsGeo.IsLeft(vecHukuin, vecKyoutyou);

                            double adjustAngle = XYZ.BasisX.AngleTo(vecHukuin) * (RevitUtil.ClsGeo.GEO_LT0(vecHukuin.Y) ? -1 : 1);
                            int hukuinCnt = (int)f.NmcHukuinCnt.Value; int kyoutyouCnt = (int)f.NmcKyoutyouCnt.Value;
                            double w = RevitUtil.ClsCommonUtils.ChangeStrToDbl(f.CmbSize.Text.Substring(f.CmbSize.Text.LastIndexOf("X") + 1));
                            double hukuinSize = w != 0 ? w : (f.CmbSize.Text.EndsWith("2000")) ? 2000 : 3000;
                            List<XYZ> FukkouList = calcFukkoubanPlacement(origin, vecHukuin, vecKyoutyou, hukuinCnt, kyoutyouCnt, hukuinSize);
                            XYZ adVec = (!isLeft) ? vecKyoutyou.Normalize() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(1000) : XYZ.Zero;
                            foreach (XYZ pnt in FukkouList)
                            {
                                DefineUtil.eFukkoubanType hType = (hukuinSize==2000) ? eFukkoubanType.TwoM : eFukkoubanType.Three;
                                ElementId idnew = Fukkouban.CreateFukkouban(_doc, f.CmbMaterial.Text, pnt, hType, f.CmbSize.Text, adjustAngle, adVec, level: baselevel, levelOffset: offset);
                                //個別情報追加
                                Fukkouban fukkouban = new Fukkouban();
                                fukkouban.m_ElementId = idnew;
                                fukkouban.m_KodaiName = f.CmbKoudaiName.Text;
                                fukkouban.m_Size = f.CmbSize.Text;
                                fukkouban.m_Material = f.CmbMaterial.Text;
                                Fukkouban.WriteToElement(fukkouban, _doc);
                            }
                            tr.Commit();
                        }
                    }
                    catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                    {
                        return Result.Cancelled;
                    }
                }
            }
            catch (Exception ex)
            {
                retBool = false;
                _logger.Error(ex.Message);
                return Result.Failed;
            }
            return Result.Succeeded;
        }

        private List<XYZ> calcFukkoubanPlacement(XYZ origin,XYZ vecHukuin,XYZ vecKyoutyou,int hukuinCnt,int kyoutyouCnt,double hukuinSize)
        {
            List<XYZ> retList = new List<XYZ>();
            XYZ baseP = origin;
            double hukuinPitch = RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(hukuinSize);
            for (int kC = 0; kC < kyoutyouCnt; kC++)
            {
                baseP = origin + vecKyoutyou.Normalize() * (RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(kC * 1000));
                XYZ newP = baseP;
                for (int hC = 0; hC < hukuinCnt; hC++)
                {
                    newP = (hC == 0) ? newP : newP + vecHukuin.Normalize() * hukuinPitch;
                    retList.Add(newP);
                }
            }

            return retList;
        }
    }
}
