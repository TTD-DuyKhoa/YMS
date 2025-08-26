using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YMS_gantry.UI;
using static YMS_gantry.DefineUtil;

namespace YMS_gantry.Command
{
    class CmdCreatePile
    {
        static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        Document _doc { get; set; }
        UIDocument _uiDoc { get; set; }

        public CmdCreatePile(UIDocument uiDoc)
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
            try
            {
                //FrmPutKui frm = new FrmPutKui(_uiDoc.Application);
                //if (frm.ShowDialog() != DialogResult.OK) { return false; }
                FrmPutKui frm = Application.thisApp.frmPutKui;

                //杭配置のために情報を作成-----------------------------------------------------------------------
                Level level = RevitUtil.ClsRevitUtil.GetLevel(_doc, frm.CmbLevel.Text) as Level;
                AllKoudaiFlatFrmData kData = GantryUtil.GetKoudaiData(_doc, frm.CmbKoudaiName.Text).AllKoudaiFlatData;
                MaterialSize ohbikiSize = GantryUtil.GetKouzaiSize(kData.ohbikiData.OhbikiSize);
                MaterialSize nedaSize = GantryUtil.GetKouzaiSize(kData.nedaData.NedaSize);
                string plateSize = frm.CmbTopPlateSize.Text;/* (frm.CmbTopPlateSize.Text != "任意サイズ") ?  : $"PL-{frm.NmcPLH.Value}x{frm.NmcPLW.Value}x{frm.NmcPLD}";*/
                PilePillerData pData = new PilePillerData();
                pData.exPillarHeadLeng = (double)frm.NmcKuiHeadLng.Value;
                pData.ExtensionPile = frm.ChkExPile.Checked;
                pData.extensionPileData = new ExtensionPileData()
                {
                    attachType =  eJoinType.Welding,
                    Length = (double)frm.NmcKuiExLng.Value,
                    topPlateData = new TopPlateData { PlateSize = plateSize }
                };
                JointPlateData jpd = new JointPlateData();
                jpd.PlateSize = Master.ClsPileTsugiteCsv.CreatePlateName(frm.txtExJointPlSizeFO1.Text, frm.txtExJointPlSizeFO2.Text, frm.txtExJointPlSizeFO3.Text);
                jpd.PlateCount = ClsCommonUtils.ChangeStrToInt(frm.txtExJointPlSizeFO4.Text);
                pData.extensionPileData.PlateFlangeOutSide = jpd;
                jpd = new JointPlateData();
                jpd.PlateSize = Master.ClsPileTsugiteCsv.CreatePlateName(frm.txtExJointPlSizeFI1.Text, frm.txtExJointPlSizeFI2.Text, frm.txtExJointPlSizeFI3.Text);
                jpd.PlateCount = ClsCommonUtils.ChangeStrToInt(frm.txtExJointPlSizeFI4.Text);
                pData.extensionPileData.PlateFlangeInSide = jpd;
                jpd = new JointPlateData();
                jpd.PlateSize = Master.ClsPileTsugiteCsv.CreatePlateName(frm.txtExJointPlSizeW1.Text, frm.txtExJointPlSizeW2.Text, frm.txtExJointPlSizeW3.Text);
                jpd.PlateCount = ClsCommonUtils.ChangeStrToInt(frm.txtExJointPlSizeW4.Text);
                pData.extensionPileData.PlateWeb = jpd;
                JointBoltData jbd = new JointBoltData();
                //jbd.BoltSize = frm.CmbExFlangeBolt.Text;
                //jbd.BoltCount = ClsCommonUtils.ChangeStrToInt(frm.txtExJointBoltF.Text);
                //pData.extensionPileData.FlangeBolt = jbd;
                //jbd = new JointBoltData();
                //jbd.BoltSize = frm.CmbExWebBolt.Text;
                //jbd.BoltCount = ClsCommonUtils.ChangeStrToInt(frm.txtExJointBoltW.Text);
                //pData.extensionPileData.WebBolt = jbd;
                pData.HasTopPlate = frm.ChkTopPlate.Checked;
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
                jbd.BoltSize = frm.CmbJointFlangeBolt.Text;
                jbd.BoltCount = ClsCommonUtils.ChangeStrToInt(frm.txtJointBoltF.Text);
                pData.jointDetailData.FlangeBolt = jbd;
                jbd = new JointBoltData();
                jbd.BoltSize = frm.CmbJointWebBolt.Text;
                jbd.BoltCount = ClsCommonUtils.ChangeStrToInt(frm.txtJointBoltW.Text);
                pData.jointDetailData.WebBolt = jbd;
                pData.PillarLength = (double)frm.NmcKuiLng.Value;
                pData.PillarMaterial = frm.CmbMaterial.Text;
                pData.PillarSize = frm.CmbSize.Text;
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

                Selection selection =_uiDoc.Selection;

                bool cancel = false;
                while (!cancel)
                {
                    using (Transaction tr = new Transaction(_doc))
                    {
                        tr.Start("Pile Palcement");
                        try
                        {
                            //杭情報
                            XYZ point = selection.PickPoint("杭の配置座標を指定してください");

                            //杭配置
                            double pOff = PilePiller.CalcLevelOffsetLevel(level, kData, ohbikiSize.Height, nedaSize.Height);
                            ElementId eId = PilePillerSuper.CreatePile(_doc, point, 0, pData,frm.CmbKoudaiName.Text, XYZ.Zero, level, pOff);
                            //個別情報追加
                            PilePiller p= new PilePiller(eId, frm.CmbKoudaiName.Text, pData);
                            if (pData.pJointCount > 0&& pData.jointDetailData.JointType==eJoinType.Bolt)
                            {
                                p.m_BoltInfo1 = pData.jointDetailData.WebBolt.BoltSize;
                                p.m_Bolt1Cnt = pData.jointDetailData.WebBolt.BoltCount * pData.pJointCount;
                                p.m_BoltInfo2 = pData.jointDetailData.FlangeBolt.BoltSize;
                                p.m_Bolt2Cnt = pData.jointDetailData.FlangeBolt.BoltCount * pData.pJointCount;
                            }
                            PilePiller.WriteToElement(p, _doc);
                           
                            if (frm.ChkExPile.Checked)
                            {
                                //継足し杭配置
                                eId = PilePillerSuper.CreateExtraPile(_doc, point, 0, pData, frm.CmbKoudaiName.Text, XYZ.Zero, level, PilePillerSuper.CalcLevelOffsetExtensionPileLevel(level, kData.BaseLevel, (double)frm.NmcOffset.Value, ohbikiSize.Height, nedaSize.Height));
                                ExtensionPile ex = new ExtensionPile(eId, frm.CmbKoudaiName.Text, pData);
                                ExtensionPile.WriteToElement(ex, _doc);
                            }
                        }
                        catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                        {
                            cancel = true;
                        }
                        tr.Commit();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageUtil.Error("杭の個別配置に失敗しました", "杭個別配置");
                _logger.Error(ex.Message);
                return false;
            }
        }
    }
}