using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YMS_gantry.Material;
using YMS_gantry.UI;

namespace YMS_gantry.Command
{
    class CmdCreateHoudue
    {
        static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        Document _doc { get; set; }
        UIDocument _uiDoc { get; set; }

        public CmdCreateHoudue(UIDocument uiDoc)
        {
            _uiDoc = uiDoc;
            _doc = _uiDoc.Document;
        }

        public void Excute()
        {
            //FrmPutHoudue frm = new FrmPutHoudue(_uiDoc.Application);
            try
            {
                //if (frm.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                //{
                //    return;
                //}
                FrmPutHoudue frm = Application.thisApp.frmPutHoudue;

                using(TransactionGroup tr= new TransactionGroup(_doc))
                {
                    tr.Start("Houdue Placement");
                    FamilyInstance pillerFam = null;
                    SelectionFilterUtil pickFilter = new SelectionFilterUtil(_uiDoc, new List<string> { PilePiller.typeName, PilePiller.sichuTypeName });
                    if (!pickFilter.Select("配置する杭、または支柱を選択してください", out pillerFam)) { return; }
                    Curve pC = GantryUtil.GetPileCurve(pillerFam,_doc);
                    MaterialSuper pS = MaterialSuper.ReadFromElement(pillerFam.Id, _doc);

                    FamilyInstance ketaFam = null;
                    pickFilter = new SelectionFilterUtil(_uiDoc, new List<string> { "根太", "桁受", "大引", "主桁" });
                    if (!pickFilter.Select("配置する桁材を選択してください", out ketaFam)) { return; }
                    Curve kC = GantryUtil.GetCurve(_doc, ketaFam.Id);
                    if (!RevitUtil.ClsGeo.GEO_EQ(kC.GetEndPoint(0).Z,kC.GetEndPoint(1).Z))
                    {
                        MessageUtil.Warning("水平な桁材のみが対象です。", "方杖配置");
                        return;
                    }
                    MaterialSuper kS = MaterialSuper.ReadFromElement(ketaFam.Id, _doc);
                    if (kC.GetEndPoint(0).DistanceTo(pC.GetEndPoint(0)) >= kC.GetEndPoint(1).DistanceTo(pC.GetEndPoint(0)))
                    {
                        kC = Line.CreateBound(kC.GetEndPoint(1), kC.GetEndPoint(0));
                    }


                    XYZ vec = kC.GetEndPoint(1) - kC.GetEndPoint(0);
                    double angle = Math.Abs(vec.AngleTo(pillerFam.HandOrientation));
                    bool isPrl = angle< Math.PI / 4||angle>Math.PI-(Math.PI/4);
                    XYZ pillerNomr = pillerFam.HandOrientation.Normalize();
                    XYZ crossNorm = pillerNomr.CrossProduct(XYZ.BasisZ).Normalize();
                    List<(XYZ,XYZ)> intPnt = GetIntersectionPoint(pC, kC, pS, kS, isPrl,frm.RbtHiuchi.Checked);
                    string hSize = frm.CmbSize.Text;
                    string famPath = Master.ClsHoudueCsv.GetFamilyPath(hSize);
                    string type =(frm.RbtEdge.Checked)?Master.ClsHoudueCsv.GetFamilyTypeBySize(hSize): (frm.RbtMaterial.Checked) ?Path.GetFileNameWithoutExtension(famPath):"方杖";
                    FamilySymbol sym;
                    if (!GantryUtil.GetFamilySymbol(_doc, famPath,type, out sym, false))
                    {
                        return;
                    }

                    if (frm.RbtHiuchi.Checked)
                    {
                        double length = (double)frm.NmcLength.Value;
                        //杭側火打
                        string kuiHi = frm.CmbKuiSize.Text;
                        string kuiHiPath = Master.ClsHoudueCsv.GetFamilyPath(kuiHi);
                        double kuiHiL = Master.ClsHoudueCsv.GetHiuchiLehg(kuiHi);

                        string ketaHi = frm.CmbKuiSize.Text;
                        string ketaHiPath = Master.ClsHoudueCsv.GetFamilyPath(ketaHi);
                        double ketaHiL = Master.ClsHoudueCsv.GetHiuchiLehg(ketaHi);

                        double wholeLeng = length + kuiHiL + ketaHiL /*+ ((isPrl) ? paraDiffL + paraDiffS : nonParaDif * 2)*/;
                        double shortL = wholeLeng / Math.Sqrt(2);

                        FamilySymbol kuiSym; FamilySymbol ketaSym;

                        using (Transaction loTr = new Transaction(_doc))
                        {
                            loTr.Start("hiuchi load");
                            type = Path.GetFileNameWithoutExtension(kuiHiPath);
                            if (!GantryUtil.GetFamilySymbol(_doc, kuiHiPath, type, out kuiSym, true))
                            {
                                return;
                            }

                            type = Path.GetFileNameWithoutExtension(ketaHiPath);
                            if (!GantryUtil.GetFamilySymbol(_doc, ketaHiPath, type, out ketaSym, true))
                            {
                                return;
                            }
                            loTr.Commit();
                        }


                        Face btmFace = GantryUtil.GetSpecifyFaceOfFamilyInstance(ketaFam, XYZ.BasisZ.Negate());/* GetBtmFaceOfFamilyInstance(ketaFam);*/

                        foreach ((XYZ, XYZ) val in intPnt)
                        {
                            XYZ kuiHiuchiP;
                            XYZ ketaHiuchiP;
                            Face kuiHiF = null;
                            FamilyInstance kuiHiIns;
                            FamilyInstance ketaHiIns;
                            using (Transaction hiTra = new Transaction(_doc))
                            {
                                hiTra.Start("hiuchi placement");
                                //火打ち受けピースを配置
                                kuiHiuchiP = val.Item1 - XYZ.BasisZ * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(shortL);
                                ketaHiuchiP = val.Item1 + val.Item2.Normalize() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(shortL);

                                XYZ norm = val.Item2.Normalize();
                                if(isPrl)
                                {
                                    norm = GantryUtil.AreComponentsSameSign(pillerNomr, val.Item2) ? pillerNomr : pillerNomr.Negate();
                                }
                                else
                                {
                                    norm = GantryUtil.AreComponentsSameSign(crossNorm, val.Item2) ? crossNorm : crossNorm.Negate();
                                }

                                Face piFace = GantryUtil.GetSpecifyFaceOfFamilyInstance(pillerFam,norm);
                                kuiHiIns = GantryUtil.CreateInstanceWith1point(_doc, kuiHiuchiP, piFace.Reference, kuiSym, normal:XYZ.BasisZ);
                                Houdue h = new Houdue();
                                h.m_KodaiName = frm.CmbKoudaiName.Text;
                                h.m_Size = kuiHi;
                                h.m_ElementId = kuiHiIns.Id;
                                Houdue.WriteToElement(h, _doc);

                                ketaHiIns = GantryUtil.CreateInstanceWith1point(_doc, ketaHiuchiP, btmFace.Reference, ketaSym, normal: val.Item2.Negate().Normalize());
                                h.m_Size = ketaHi;
                                h.m_ElementId = ketaHiIns.Id;
                                Houdue.WriteToElement(h, _doc);
                                hiTra.Commit();
                            }
                            
                            kuiHiF = GantryUtil.GetSpecifyFaceOfFamilyInstance(kuiHiIns,kuiHiIns.GetTransform().BasisY);

                            double up = ClsRevitUtil.ConvertDoubleGeo2Revit(kuiHiL /*+ ((isPrl) ? paraDiffL : nonParaDif)*/);
                            XYZ vY = (ketaHiuchiP - kuiHiuchiP).Normalize();
                            using (Transaction hiTra = new Transaction(_doc))
                            {
                                hiTra.Start("hiuchi placement");
                                sym = GantryUtil.DuplicateTypeWithNameRule(_doc, frm.CmbKoudaiName.Text, sym, "方杖");

                                //そのトップに仮鋼材を配置
                                ElementId id = GantryUtil.CreateInstanceWith1point(_doc, kuiHiuchiP + vY * up, kuiHiF.Reference, sym).Id;
                                Houdue h = new Houdue();
                                h.m_ElementId = id;
                                h.m_KodaiName = frm.CmbKoudaiName.Text;
                                h.m_Size = hSize;
                                h.m_Syuzai = true;
                                h.IsSyuzai = true;
                                Houdue.WriteToElement(h, _doc);
                                ClsRevitUtil.SetParameter(_doc, id, DefineUtil.PARAM_LENGTH,ClsRevitUtil.CovertToAPI((double)frm.NmcLength.Value));
                                Line li = Line.CreateBound(kuiHiuchiP + vY * up, kuiHiuchiP + vY * up + kuiHiIns.GetTransform().BasisY);
                                RevitUtil.ClsRevitUtil.RotateElement(_doc, id,li, 45 * (Math.PI / 180));

                                hiTra.Commit();
                            }
                        }
                    }
                    else if(frm.RbtMaterial.Checked)
                    {
                        Face top = GantryUtil.GetTopFaceOfFamilyInstance(pillerFam);

                        //仮想交点に配置する
                        using (Transaction hiTra = new Transaction(_doc))
                        {
                            hiTra.Start("hiuchi placement");
                            foreach ((XYZ, XYZ) val in intPnt)
                            {
                                ElementId id = GantryUtil.CreateInstanceWith1point(_doc, val.Item1, top.Reference, sym, val.Item2).Id;
                                Houdue h = new Houdue();
                                h.m_ElementId = id;
                                h.m_KodaiName = frm.CmbKoudaiName.Text;
                                h.m_Size = hSize;
                                Houdue.WriteToElement(h, _doc);

                                string cate = GantryUtil.GetOriginalTypeName(_doc, pillerFam);
                                PilePiller p = PilePiller.ReadFromElement(pillerFam.Id,_doc) as PilePiller;
                                //対象の支柱が25HAまたは30HAだった場合でウェブ側に付く場合はネコ材が必要
                                if(p!=null&&cate.Equals(PilePiller.sichuTypeName))
                                {
                                    string size = p.m_Size;
                                    if((size.Contains("25HA")&&hSize.Contains("CH25"))||(size.Contains("30HA") && hSize.Contains("CH30")))
                                    {
                                        FamilySymbol nekoSym;
                                        string nekoSize = size.Contains("25HA") ? HoudueNeko.neko25 : HoudueNeko.neko30;
                                        famPath = Master.ClsHoudueCsv.GetFamilyPath(nekoSize);
                                        string nekoType = Path.GetFileNameWithoutExtension(famPath);
                                        if (!GantryUtil.GetFamilySymbol(_doc, famPath,nekoType, out nekoSym,true))
                                        {
                                            continue;
                                        }
                                        double wide = size.Contains("25HA") ? 250 : 300;
                                        XYZ cross = val.Item2.CrossProduct(XYZ.BasisZ).Normalize();
                                        if(!RevitUtil.ClsGeo.IsLeft(val.Item2.Normalize(),cross))
                                        {
                                            cross = cross.Negate().Normalize();
                                        }

                                        //2個配置
                                        HoudueNeko hN = new HoudueNeko();
                                        hN.m_KodaiName = frm.CmbKoudaiName.Text;
                                        hN.m_Size = nekoSize;
                                        XYZ nPnt = val.Item1-XYZ.BasisZ*RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(1000)+
                                            cross* RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(wide/2);
                                        hN.m_ElementId = GantryUtil.CreateInstanceWith1point(_doc, nPnt, top.Reference, nekoSym, val.Item2).Id;
                                        Houdue.WriteToElement(hN, _doc);
                                        nPnt+= cross.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(wide);
                                        hN.m_ElementId = GantryUtil.CreateInstanceWith1point(_doc, nPnt, top.Reference, nekoSym, cross.Negate()).Id;
                                        Houdue.WriteToElement(hN, _doc);
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                            }
                            hiTra.Commit();
                        }
                    }
                    else if(frm.RbtEdge.Checked)
                    {
                        //仮想交点に配置する
                        using (Transaction hiTra = new Transaction(_doc))
                        {
                            hiTra.Start("hiuchi placement");
                            foreach ((XYZ, XYZ) val in intPnt)
                            {
                                Face piFace = GantryUtil.GetSpecifyFaceOfFamilyInstance(pillerFam, val.Item2.Normalize());
                                ElementId id = GantryUtil.CreateInstanceWith1point(_doc, val.Item1,piFace.Reference, sym, XYZ.BasisZ).Id;
                                Houdue h = new Houdue();
                                h.m_ElementId = id;
                                h.m_KodaiName = frm.CmbKoudaiName.Text;
                                h.m_Size = hSize;
                                Houdue.WriteToElement(h, _doc);
                            }
                            hiTra.Commit();
                        }
                    }
                    tr.Assimilate();
                }
            }
            catch(Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return;
            }
        }
        
  

        /// <summary>
        /// 挿入点とその向きを返す
        /// </summary>
        /// <param name="pillerC"></param>
        /// <param name="ketaC"></param>
        /// <param name="piller"></param>
        /// <param name="keta"></param>
        /// <returns></returns>
        private List<(XYZ,XYZ)> GetIntersectionPoint(Curve pillerC,Curve ketaC,MaterialSuper piller,MaterialSuper keta, bool isPrl,bool isHiuchi)
        {
            List<(XYZ, XYZ)> retList = new List<(XYZ, XYZ)>();
            XYZ retXYZ = pillerC.GetEndPoint(0);
            XYZ intP = new XYZ(retXYZ.X, retXYZ.Y, ketaC.GetEndPoint(0).Z);

            XYZ vecKeta = (ketaC.GetEndPoint(1) - ketaC.GetEndPoint(0)).Normalize();
            string pillerSize = piller.m_Size;
            if(pillerSize.Contains("_"))
            {
                int i = pillerSize.IndexOf("_");
                pillerSize = pillerSize.Substring(0, i);
            }
            string ketaSize = keta.m_Size;
            if (pillerSize.Contains("_"))
            {
                int i =ketaSize.IndexOf("_");
                ketaSize = ketaSize.Substring(0, i);
            }

            if (pillerSize.Contains("HA")||pillerSize.Contains("SMH"))
            {
                pillerSize = Master.ClsKouzaiSpecify.GetKouzaiSize(pillerSize);
            }
            if (ketaSize.Contains("HA") || ketaSize.Contains("SMH"))
            {
                ketaSize = Master.ClsKouzaiSpecify.GetKouzaiSize(ketaSize);
            }

            SteelSize pS = GantryUtil.GetKouzaiSizeSunpou(pillerSize);
            MaterialSize kS = keta.MaterialSize();

            double pThick = (isPrl) ? pS.FrangeWidth: pS.Height;
            if (isHiuchi)
            {
                pThick = (isPrl) ? pS.WebThick : pS.Height;
            }

            retXYZ = intP - XYZ.BasisZ * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(kS.Height/2);
            retXYZ = retXYZ + vecKeta * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(pThick / 2);
            retList.Add((retXYZ, vecKeta));

            //桁材端部が柱の直上に来ていない時は両端部に方杖を付ける
            if (intP.DistanceTo(ketaC.GetEndPoint(0))> RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(pS.Height))
            {
                vecKeta = vecKeta.Negate();
                retXYZ = intP - XYZ.BasisZ * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(kS.Height / 2);
                retXYZ = retXYZ + vecKeta * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(pThick / 2);
                retList.Add((retXYZ, vecKeta));
            }
            return retList;
        }

        /// <summary>
        /// 指定長さを底辺とした時の他2辺長さを求める
        /// </summary>
        /// <param name="baseLength"></param>
        /// <returns></returns>
        private double CalculateIsoscelesSides(double baseLength)
        {
            double height = baseLength/ Math.Sqrt(2);
            double side = Math.Sqrt(baseLength * baseLength + height * height);

            return height;
        }

        private bool AreParallel(XYZ vector1, XYZ vector2)
        {
            XYZ crossProduct = vector1.CrossProduct(vector2);
            return crossProduct.IsAlmostEqualTo(XYZ.Zero);
        }
    }
}
