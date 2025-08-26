using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YMS_gantry.UI;
using static YMS_gantry.DefineUtil;

namespace YMS_gantry.Command
{
    class CmdCreatePiller
    {

        static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        Document _doc { get; set; }
        UIDocument _uiDoc { get; set; }

        public CmdCreatePiller(UIDocument uiDoc)
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
            bool retB = true;
            try
            {
                //FrmPutShichu frm = new FrmPutShichu(_uiDoc.Application);
                //if (frm.ShowDialog() != DialogResult.OK) { return false; }
                FrmPutShichu frm = Application.thisApp.frmPutShichu;

                //配置のために杭情報収集--------------------------------------------------------------------
                //杭配置のために情報を作成-----------------------------------------------------------------------
                AllKoudaiFlatFrmData kData = GantryUtil.GetKoudaiData(_doc, frm.CmbKoudaiName.Text).AllKoudaiFlatData;
                MaterialSize ohbikiSize = GantryUtil.GetKouzaiSize(kData.ohbikiData.OhbikiSize);
                MaterialSize nedaSize = GantryUtil.GetKouzaiSize(kData.nedaData.NedaSize);
                string plateSize = frm.CmbTopPlateSize.Text;/* (frm.CmbTopPlateSize.Text != "任意サイズ") ?  : $"PL-{frm.NmcPLH.Value}x{frm.NmcPLW.Value}x{frm.NmcPLD}";*/
                PilePillerData pData = new PilePillerData();
                pData.exPillarHeadLeng = (double)frm.NmcKuiHeadLng.Value;
                pData.ExtensionPile =false;
                JointPlateData jpd = new JointPlateData();
                JointBoltData jbd = new JointBoltData();
                pData.HasTopPlate = frm.ChkHasTopPlate.Checked;
                pData.IsCut = frm.ChckKuiHasCut.Checked;
                pData.jointDetailData = new JointDetailData
                {
                    JointType = frm.CmbJointAttach.Text == "ボルト" ? eJoinType.Bolt : eJoinType.Welding
                };
                jpd = new JointPlateData();
                jpd.PlateSize = Master.ClsPileTsugiteCsv.CreatePlateName(frm.txtJointPlSizeFO1.Text, frm.txtJointPlSizeFO2.Text, frm.txtJointPlSizeFO3.Text);
                jpd.PlateCount = ClsCommonUtils.ChangeStrToInt(frm.txtJointPlSizeFO4.Text);
                pData.jointDetailData.PlateFlangeOutSide = jpd;
                jpd = new JointPlateData();
                jpd.PlateSize = Master.ClsPileTsugiteCsv.CreatePlateName(frm.txtJointPlSizeFI1.Text, frm.txtJointPlSizeFI2.Text, frm.txtJointPlSizeFI3.Text);
                jpd.PlateCount = ClsCommonUtils.ChangeStrToInt(frm.txtJointPlSizeFI4.Text);
                pData.jointDetailData.PlateFlangeInSide = jpd;
                jpd = new JointPlateData();
                jpd.PlateSize = Master.ClsPileTsugiteCsv.CreatePlateName(frm.txtJointPlSizeW1.Text, frm.txtJointPlSizeW2.Text, frm.txtJointPlSizeW3.Text);
                jpd.PlateCount = ClsCommonUtils.ChangeStrToInt(frm.txtJointPlSizeW4.Text);
                pData.jointDetailData.PlateWeb = jpd;
                jbd = new JointBoltData();
                jbd.BoltSize = frm.CmbKuiJointBoltSizeF.Text;
                jbd.BoltCount = ClsCommonUtils.ChangeStrToInt(frm.txtJointBoltF.Text);
                pData.jointDetailData.FlangeBolt = jbd;
                jbd = new JointBoltData();
                jbd.BoltSize = frm.CmbKuiJointBoltSizeW.Text;
                jbd.BoltCount = ClsCommonUtils.ChangeStrToInt(frm.txtJointBoltW.Text);
                pData.jointDetailData.WebBolt = jbd;
                pData.PillarLength = (double)frm.NmcKuiLng.Value;
                pData.PillarMaterial = frm.CmbMaterial.Text;
                pData.PillarSize = frm.CmbShichuSize.Text;
                pData.PillarWholeLength = (double)frm.NmcKuiWholeLng.Value;
                pData.pJointCount = (int)frm.NmcJointCnt.Value;
                foreach (DataGridViewRow row in frm.DgvJointSpan.Rows)
                {
                    if (row.Cells[1].Equals(0)) { continue; }
                    pData.pJointPitch.Add(double.Parse(row.Cells[1].Value.ToString()));
                }
                pData.topPlateData = new TopPlateData
                {
                    PlateSize = plateSize
                };

                double cutLeng = PilePillerSuper.CalcPileCutLength(_doc, kData,pData);
                pData.PileCutLength = cutLeng;

                //杭配置のために情報を作成-----------------------------------------------------------------------

                //面指定
                if (frm.RbtElement.Checked)
                {
                    using (TransactionGroup tr = new TransactionGroup(_doc))
                    {
                        FamilySymbol sym = null;

                        try
                        {
                            tr.Start("Piller Palcement");
                            using (Transaction loadTrans = new Transaction(_doc))
                            {
                                loadTrans.Start("Piller Palcement");

                                string path = Master.ClsSichuCsv.GetFamilyPath(frm.CmbShichuSize.Text);
                                if (!GantryUtil.GetFamilySymbol(_doc, path, PilePiller.sichuTypeName, out sym, true))
                                {
                                    MessageUtil.Error("ファミリシンボルを取得できませんでした", "支柱配置");
                                    return false;
                                }

                                Dictionary<string, string> paramList = new Dictionary<string, string>();
                                paramList.Add(DefineUtil.PARAM_LENGTH, pData.PillarWholeLength.ToString());
                                if (frm.CmbShichuSizeType.Text == Master.ClsSichuCsv.HKou)
                                {
                                    string jointF = GantryUtil.GetJointTypeName(pData.jointDetailData.JointType, pData.PillarSize);
                                    FamilySymbol jSym= RevitUtil.ClsRevitUtil.GetFamilySymbol(_doc, jointF, jointF);
                                    if(jSym!=null)
                                    {
                                        paramList.Add(DefineUtil.PARAM_JOINT_TYPE, jSym.Id.ToString());
                                    }
                                    paramList.Add(DefineUtil.PARAM_PILE_CUT_LENG, (pData.IsCut) ? $"{pData.PileCutLength}" : "0");
                                    paramList.Add(DefineUtil.PARAM_MATERIAL, pData.PillarMaterial);

                                    //トッププレート
                                    paramList.Add(DefineUtil.PARAM_TOP_PLATE, (pData.HasTopPlate) ? ((int)DefineUtil.PramYesNo.Yes).ToString() : ((int)DefineUtil.PramYesNo.No).ToString());
                                    MaterialSize plsize = GantryUtil.GetKouzaiSize(pData.topPlateData.PlateSize);

                                    string plateName = "ﾄｯﾌﾟﾌﾟﾚｰﾄ_定形";
                                    string plateType = (pData.extensionPileData.topPlateData.PlateSize == "任意") ? "ﾄｯﾌﾟﾌﾟﾚｰﾄ_任意" : pData.topPlateData.PlateSize;
                                    FamilySymbol pltSym = RevitUtil.ClsRevitUtil.GetFamilySymbolWithFuzzy(_doc, plateName, plateType);
                                    if (pltSym == null)
                                    {
                                        pltSym = RevitUtil.ClsRevitUtil.GetFamilySymbolWithFuzzy(_doc, "ﾄｯﾌﾟﾌﾟﾚｰﾄ_任意", "ﾄｯﾌﾟﾌﾟﾚｰﾄ_任意");
                                    }
                                    if (pltSym != null)
                                    {
                                        paramList.Add(DefineUtil.PARAM_TOP_PLATE_SIZE, pltSym.Id.ToString());
                                    }

                                    //ベースプレート
                                    string[] bpl = Master.ClsSichuCsv.GetBPLData(frm.CmbShichuSize.Text, frm.CmbBasePlateType.Text).Split('_');
                                    paramList.Add(DefineUtil.PARAM_BASE_PLATE, (frm.ChkHasBasePlate.Checked ? ((int)DefineUtil.PramYesNo.Yes).ToString() : ((int)DefineUtil.PramYesNo.No).ToString()));
                                    if (bpl.Length == 2)
                                    {
                                        pltSym = RevitUtil.ClsRevitUtil.GetFamilySymbolWithFuzzy(_doc, bpl[0], bpl[1]);
                                        if (pltSym != null)
                                        {
                                            paramList.Add(DefineUtil.PARAM_BASE_PLATE_SIZE, pltSym.Id.ToString());
                                        }
                                    }

                                    //継ぎ手
                                    paramList.Add(DefineUtil.PARAM_JOINT_COUNT, pData.pJointCount.ToString()); ;
                                    for (int i = 0; i < pData.pJointPitch.Count; i++)
                                    {
                                        if (pData.pJointPitch[i].Equals(0)) { continue; }
                                        paramList.Add($"{i + 1}段目長さ", pData.pJointPitch[i].ToString());
                                    }
                                }
                                string typeName = GantryUtil.CreateTypeName(PilePiller.sichuTypeName, paramList);
                                //sym = GantryUtil.ChangeTypeID(_doc, sym, typeName);
                                sym = GantryUtil.DuplicateTypeWithNameRule(_doc, frm.CmbKoudaiName.Text, sym,PilePillerSuper.sichuTypeName);


                                //タイプパラメータ設定
                                foreach (KeyValuePair<string, string> kv in paramList)
                                {
                                    GantryUtil.SetParameterValueByParameterName(_doc.GetElement(sym.Id) as FamilySymbol, kv.Key, kv.Value);
                                }
                                loadTrans.Commit();
                            }
                            GantryUtil util = new GantryUtil();
                            //配置
                            List<ElementId> ids = util.PlaceFamilyInstance(_uiDoc.Application, sym, frm.RbtElement.Checked);
                            using (Transaction placeTr = new Transaction(_doc))
                            {
                                
                                placeTr.Start("piller set");
                                //インスタンスParam設定
                                foreach (ElementId id in ids)
                                {
                                    FamilyInstance ins = _doc.GetElement(id) as FamilyInstance;
                                    if (ins==null||ins.SuperComponent != null)
                                    {
                                        continue;
                                    }

                                    PilePiller p = new PilePiller(id, frm.CmbKoudaiName.Text, pData);
                                    if (pData.pJointCount > 0 && pData.jointDetailData.JointType == eJoinType.Bolt)
                                    {
                                        p.m_BoltInfo1 = pData.jointDetailData.WebBolt.BoltSize;
                                        p.m_Bolt1Cnt = pData.jointDetailData.WebBolt.BoltCount * pData.pJointCount;
                                        p.m_BoltInfo2 = pData.jointDetailData.FlangeBolt.BoltSize;
                                        p.m_Bolt2Cnt = pData.jointDetailData.FlangeBolt.BoltCount * pData.pJointCount;
                                    }
                                    PilePiller.WriteToElement(p, _doc);
                                    PilePiller.WriteToElement(p, _doc);
                                }

                                placeTr.Commit();
                            }
                            tr.Commit();
                        }
                        catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                        {

                        }
                        catch (Exception ex)
                        {
                            tr.RollBack();
                            _logger.Error(ex.Message);
                        }
                    }
                }
                else　/*自由配置*/
                {
                    Selection selection = _uiDoc.Selection;
                    bool cancel = false;
                    Level level = RevitUtil.ClsRevitUtil.GetLevel(_doc, frm.CmbLevel.Text) as Level;
                    while (!cancel)
                    {
                        double insOff = 0;
                        using (Transaction tr = new Transaction(_doc))
                        {
                            tr.Start("Piller Palcement");
                            try
                            {
                                ElementId eId = ElementId.InvalidElementId;
                                if (frm.CmbLevel.Text == "部材選択")
                                {
                                    ElementId selId = selection.PickObject(ObjectType.Element, "配置基準となる部材を選択してください").ElementId;
                                    FamilyInstance ins = _doc.GetElement(selId) as FamilyInstance;
                                    if (ins == null) { return false; }

                                    level = GantryUtil.GetInstanceLevelAndOffset(_uiDoc.Document, ins, ref insOff);
                                    if (level == null)
                                    {
                                        MessageUtil.Warning("指定した部材のホストに設定されたレベルが取得できません\r\nホストがレベルの部材を選択してください", "支柱配置");
                                        return false;
                                    }
                                }

                                insOff += (double)frm.NmcOffset.Value;
                                //杭情報
                                XYZ point = selection.PickPoint("支柱の配置座標を指定してください");

                                string BplData=Master.ClsSichuCsv.GetBPLData(frm.CmbShichuSize.Text, frm.CmbBasePlateType.Text);
                                //杭配置
                                eId = PilePillerSuper.CreatePiller(_doc, point, 0,frm.CmbShichuSizeType.Text, pData,frm.CmbKoudaiName.Text, BplData,frm.ChkHasBasePlate.Checked, level, insOff);
                                //個別情報追加
                                PilePiller p = new PilePiller(eId, frm.CmbKoudaiName.Text, pData);
                                if (pData.pJointCount > 0 && pData.jointDetailData.JointType == eJoinType.Bolt)
                                {
                                    p.m_BoltInfo1 = pData.jointDetailData.WebBolt.BoltSize;
                                    p.m_Bolt1Cnt = pData.jointDetailData.WebBolt.BoltCount * pData.pJointCount;
                                    p.m_BoltInfo2 = pData.jointDetailData.FlangeBolt.BoltSize;
                                    p.m_Bolt2Cnt = pData.jointDetailData.FlangeBolt.BoltCount * pData.pJointCount;
                                }
                                PilePiller.WriteToElement(p, _doc);
                            }
                            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                            {
                                cancel = true;
                            }
                            tr.Commit();
                        }
                    }
                }

                retB = true;
            }
            catch (Exception)
            {
                retB = false;
            }
            return retB;
        }
    }
}