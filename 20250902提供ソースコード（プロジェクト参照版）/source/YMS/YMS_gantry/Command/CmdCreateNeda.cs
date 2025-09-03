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
    class CmdCreateNeda
    {
        static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        Document _doc { get; set; }
        UIDocument _uiDoc { get; set; }

        public CmdCreateNeda(UIDocument uiDoc)
        {
            _uiDoc = uiDoc;
            _doc = _uiDoc.Document;
        }

        public void Excute()
        {
            //ダイアログ表示
            //FrmPutNedaShugeta frm = new FrmPutNedaShugeta(_uiDoc.Application);
            //if (frm.ShowDialog() != DialogResult.OK) { return; }
            FrmPutNedaShugeta frm = Application.thisApp.frmPutNedaShugeta;
            Selection selection = _uiDoc.Selection;

            //根太用データ作成-------------------------------------------------------------------------------
            AllKoudaiFlatFrmData allData = GantryUtil.GetKoudaiData(_doc, frm.CmbKoudaiName.Text).AllKoudaiFlatData;
            NedaData nData = new NedaData();
            nData.NedaType = frm.CmbSizeType.Text;
            nData.NedaSize = frm.CmbSize.Text;
            nData.NedaMaterial = frm.CmbMaterial.Text;
            nData.exNedaStartLeng = (double)frm.NmcSLng.Value;
            nData.exNedaEndLeng = (double)frm.NmcELng.Value;

            double offset = (double)frm.NmcOffset.Value;
            Level level = ClsRevitUtil.GetLevel(_doc, frm.CmbLevel.Text) as Level;
            //根太用データ作成-------------------------------------------------------------------------------

            try
            {
                while(true)
                {
                    using (Transaction tr = new Transaction(_doc))
                    {
                        tr.Start("Placement neda");
                        //大引選択
                        if (frm.RbtOhbikiKetauke.Checked)
                        {
                            //２本の大引を選択
                            SelectionFilterUtil pickFilter = new SelectionFilterUtil(_uiDoc, new List<string> { "大引", "敷桁" });
                            FamilyInstance ohbiki1 = null, ohbiki2 = null;
                            XYZ on1, on2;
                            if (!pickFilter.Select("１本目の大引を選択してください", out ohbiki1))
                            {
                                return;
                            }
                            //それぞれのどの位置から根太を乗せるか選択
                            on1 = selection.PickPoint("根太の始点位置を選択");
                            if (!pickFilter.Select("２本目の大引を選択してください", out ohbiki2))
                            {
                                return;
                            }
                            on2 = selection.PickPoint("根太の終点位置を選択");

                            //各点をそれぞれの大引の線上に変換
                            List<XYZ> o1Pnts = GantryUtil.GetCurvePoints(_doc, ohbiki1.Id);
                            on1 = GantryUtil.GetClosestPointOnVector(o1Pnts[1] - o1Pnts[0], o1Pnts[0], on1);
                            List<XYZ> o2Pnts = GantryUtil.GetCurvePoints(_doc, ohbiki2.Id);
                            on2 = GantryUtil.GetClosestPointOnVector(o2Pnts[1] - o2Pnts[0], o2Pnts[0], on2);

                            double ohOff = 0;
                            level = GantryUtil.GetInstanceLevelAndOffset(_doc,ohbiki1,ref ohOff);

                            //選択された大引のサイズを見る
                            FamilyInstance ins = _doc.GetElement(ohbiki1.Id) as FamilyInstance;
                            MaterialSize ohSize = GantryUtil.GetKouzaiSize(ins.Symbol.FamilyName);
                            MaterialSize nedaSize= GantryUtil.GetKouzaiSize(nData.NedaSize);
                            offset = ohOff+(ohSize.Height/2) + (nedaSize.Height / 2);

                            //根太の線分作成
                            XYZ nedaVec = (on2 - on1).Normalize();
                            //on1 += nedaVec.Negate() * (ClsRevitUtil.ConvertDoubleGeo2Revit(nData.exNedaStartLeng));
                            //on2 += nedaVec * (ClsRevitUtil.ConvertDoubleGeo2Revit(nData.exNedaEndLeng));

                            //線分
                            ElementId newId = Neda.CreateNeda(_doc, nData, on1, on2, level,allData.KoujiType==DefineUtil.eKoujiType.Doboku,frm.CmbKoudaiName.Text, offset);
                            ClsRevitUtil.SetParameter(_doc, newId, DefineUtil.PARAM_BASE_OFFSET, ClsRevitUtil.ConvertDoubleGeo2Revit(offset));

                            Neda dNd = new Neda(newId,frm.CmbKoudaiName.Text,nData);
                            Neda.WriteToElement(dNd, _doc);
                        }
                        else
                        {
                            FamilySymbol sym;
                            string familyPath = Master.ClsMasterCsv.GetFamilyPath(nData.NedaSize);
                            string type = allData.KoujiType == eKoujiType.Doboku ? "主桁" : Neda.typeName;
                            if (!GantryUtil.GetFamilySymbol(_doc, familyPath, type, out sym, true))
                            {
                                return ;
                            }
                            ElementId newId = ElementId.InvalidElementId;
                            Dictionary<string, string> paramList = new Dictionary<string, string>();
                            paramList.Add(DefineUtil.PARAM_MATERIAL, nData.NedaMaterial);
                            //sym = GantryUtil.ChangeTypeID(_doc, sym, GantryUtil.CreateTypeName(Neda.typeName, paramList));
                            sym = GantryUtil.DuplicateTypeWithNameRule(_doc, frm.CmbKoudaiName.Text, sym, type);
                            //タイプパラメータ設定
                            foreach (KeyValuePair<string, string> kv in paramList)
                            {
                                GantryUtil.SetParameterValueByParameterName(sym, kv.Key, kv.Value);
                            }

                            try
                            {
                                if (frm.CmbLevel.Text == "部材選択")
                                {
                                    newId = GantryUtil.PlaceSymbolOverTheSelectedElm(_uiDoc, sym, frm.CmbKoudaiName.Text, level, offset, nData.exNedaStartLeng, nData.exNedaEndLeng);
                                }
                                else
                                {
                                    XYZ p1=selection.PickPoint("1点目を指定してください");
                                    XYZ p2=selection.PickPoint("2点目を指定してください");
                                    p1 =new XYZ(p1.X,p1.Y, level.Elevation); p2 = new XYZ(p2.X, p2.Y, level.Elevation);
                                    XYZ vec = (p2 - p1).Normalize();
                                    if (frm.NmcSLng.Value > 0)
                                    {
                                        p1 = p1 + vec.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(nData.exNedaStartLeng);
                                    }
                                    if (frm.NmcELng.Value > 0)
                                    {
                                        p2 = p2 + vec * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(nData.exNedaEndLeng);
                                    }
                                    newId = MaterialSuper.PlaceWithTwoPoints(sym, level.GetPlaneReference(), p1, p2, RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( offset));
                                }
                                if (newId != ElementId.InvalidElementId || newId != null)
                                {
                                    Neda dNd = new Neda(newId, frm.CmbKoudaiName.Text, nData);
                                    Neda.WriteToElement(dNd, _doc);
                                }
                            }
                            catch(Autodesk.Revit.Exceptions.OperationCanceledException)
                            {
                                return;
                            }
                        }
                        tr.Commit();
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
