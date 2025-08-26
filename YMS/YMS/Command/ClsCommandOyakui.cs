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
using YMS.Parts;

namespace YMS.CommandCls
{
    class ClsCommandOyakui
    {
        public static void CommandOyakui(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            //ドキュメントを取得
            Document doc = uidoc.Document;

            //Oyakuiクラスを生成
            ClsOyakui clsOyakui = new ClsOyakui();

            ////ワークセット
            //ClsWorkset clsWS = new ClsWorkset();
            //clsWS.SetWorkSetTest2(doc);

            //ダイアログを表示
            DLG.DlgCreateOyakui Oyakui = new DLG.DlgCreateOyakui(doc);
            //ClsGlobal.m_Oyakui = Oyakui;
            for (; ; )
            {
                DialogResult result = Oyakui.ShowDialog();
                if (result == DialogResult.Cancel)
                {
                    return;
                }
                else if (result == DialogResult.Retry)
                {
                    ElementId id = null;
                    if (!ClsOyakui.PickObject(uidoc, ref id))
                        return;
                    clsOyakui.SetParameter(doc, id);
                    Oyakui = new DLG.DlgCreateOyakui(doc,clsOyakui);
                }
                else
                    break;
            }
            clsOyakui = Oyakui.m_ClsOyakui;

            //作図に必要な作図箇所の座標を取得
            Selection selection = uidoc.Selection;
            ElementId selShinId = null;
            XYZ picPointS = null;
            XYZ picPointE = null;
            ElementId levelId = null;
            for (; ; )
            {
                bool defVoid = true;
                try
                {
                    if (clsOyakui.m_way == 0)
                    {
                        
                        if (!ClsKabeShin.PickBaseObject(uidoc, ref selShinId))
                        {
                            return;
                        }

                        FamilyInstance instance = doc.GetElement(selShinId) as FamilyInstance;
                        LocationCurve lCurve = instance.Location as LocationCurve;
                        levelId = instance.Host.Id;

                        List<List<XYZ>> spList = ClsKabeShin.GetALLKabeShinStartEndPointList(doc);
                        List<List<XYZ>> resList = new List<List<XYZ>>();

                        ClsYMSUtil.FindConnectedPoints(lCurve.Curve.GetEndPoint(0), lCurve.Curve.GetEndPoint(1), spList, ref resList);
                        List<ElementId> kabeShinList = ClsKabeShin.GetAllKabeShinList(doc);
                        for (int i = 0; i < resList.Count; i++)
                        {
                            if (resList[i].Count != 2)
                                continue;
                            List<XYZ> list1 = new List<XYZ>();
                            List<XYZ> list2 = new List<XYZ>();

                            if (i == 0)
                            {
                                list1 = resList[i];
                                list2 = resList[resList.Count - 1];
                            }
                            else
                            {
                                list1 = resList[i];
                                list2 = resList[i - 1];
                            }
                            Curve cv1 = Line.CreateBound(list1[0], list1[1]);
                            Curve cv2 = Line.CreateBound(list2[0], list2[1]);

                            //入隅か判定
                            bool bIrizumi = false;
                            if (!ClsHaraokoshiBase.CheckIrizumi(cv1, cv2, ref bIrizumi))
                            {
                                continue;
                            }

                            picPointS = list1[0];
                            picPointE = list1[1];

                            if (!bIrizumi)//出隅のときのみ別で親杭を作成する
                            {
                                XYZ dir1 = Line.CreateBound(list1[0], list1[1]).Direction;
                                XYZ dir2 = Line.CreateBound(list2[0], list2[1]).Direction;
                                XYZ dir = dir1 + dir2;
                                double apexAngle = Math.PI - dir1.AngleOnPlaneTo(dir2, XYZ.BasisZ);
                                XYZ desumipoint = picPointS;
                                if(clsOyakui.m_putPosFlag != 0)
                                {
                                    double dEx = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(Master.ClsYamadomeCsv.GetKouzaiSizeSunpou(clsOyakui.m_size, 1)) / 2);
                                    desumipoint = desumipoint + dEx * (dir2 - dir1);
                                }
                                clsOyakui.CreateSingleOyakui(doc, desumipoint, dir, levelId, bIrizumi, true, selShinId, apexAngle);
                            }
                            foreach (ElementId id in kabeShinList)
                            {
                                FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                                Curve cv = (inst.Location as LocationCurve).Curve;
                                if (ClsGeo.GEO_EQ(picPointS, cv.GetEndPoint(0)) && ClsGeo.GEO_EQ(picPointE, cv.GetEndPoint(1)))
                                {
                                    selShinId = id;
                                    break;
                                }
                            }
                            //Oyakuiを作図
                            if (!clsOyakui.CreateOyakui(doc, picPointS, picPointE, levelId, bIrizumi, selShinId))
                            {
                                //作図に失敗した時のコメント
                            }
                        }
                        break;
                    }
                    if (clsOyakui.m_way == 1)
                    {
                        if (!ClsKabeShin.PickBaseObject(uidoc, ref selShinId))
                        {
                            return;
                        }
                        FamilyInstance instance = doc.GetElement(selShinId) as FamilyInstance;
                        LocationCurve lCurve = instance.Location as LocationCurve;
                        levelId = instance.Host.Id;

                        Curve cvShin = lCurve.Curve;
                        picPointS = cvShin.GetEndPoint(0);
                        picPointE = cvShin.GetEndPoint(1);

                        XYZ selPoint = uidoc.Selection.PickPoint("始点方向を指定してください");
                        if (!ClsRevitUtil.CheckNearGetEndPoint(cvShin, selPoint))
                        {
                            XYZ change = picPointE;
                            picPointE = picPointS;
                            picPointS = change;
                            defVoid = false;
                        }

                    }
                    if (clsOyakui.m_way == 2)
                    {
                        //if (levelId == null)
                        //{
                        //    DLG.DlgGL dlgGL = new DLG.DlgGL(doc);
                        //    DialogResult result = dlgGL.ShowDialog();
                        //    if (result == DialogResult.Cancel)
                        //    {
                        //        return;
                        //    }
                        //    string levelName = dlgGL.m_Level;
                        //    levelId = ClsRevitUtil.GetLevelID(doc, levelName);
                        //}
                        if (!ClsKabeShin.PickBaseObject(uidoc, ref selShinId))
                        {
                            return;
                        }
                        FamilyInstance instance = doc.GetElement(selShinId) as FamilyInstance;
                        LocationCurve lCurve = instance.Location as LocationCurve;
                        levelId = instance.Host.Id;

                        picPointS = selection.PickPoint(clsOyakui.FamilyNameKui + "を配置する開始位置を選択してください");
                        if (picPointS == null)
                        {
                            return;
                        }
                        picPointE = selection.PickPoint(clsOyakui.FamilyNameKui + "を配置する終了位置を選択してください");
                        if (picPointE == null)
                        {
                            return;
                        }
                        //ClsCInput.Instance.Get2Point(uiapp, out picPointS, out picPointE);
                        //掘削側を指定
                        XYZ Direction = Line.CreateBound(picPointS, picPointE).Direction;
                        XYZ kussaku = selection.PickPoint("掘削側を指定してください");
                        XYZ kussakuDir = Line.CreateBound(picPointS, kussaku).Direction;
                        if (!ClsGeo.IsLeft(Direction, kussakuDir))
                        {
                            defVoid = false;
                        }
                    }
                    if (clsOyakui.m_way == 3)
                    {
                        Reference rf = null;
                        if (!ClsKabeShin.PickBaseObject(uidoc, ref rf))
                        {
                            return;
                        }
                        picPointS = rf.GlobalPoint;
                        if (picPointS == null)
                        {
                            return;
                        }
                        FamilyInstance inst = doc.GetElement(rf) as FamilyInstance;
                        levelId = inst.Host.Id;
                        Curve cv = (inst.Location as LocationCurve).Curve;
                        XYZ dir = Line.CreateBound(cv.GetEndPoint(0), cv.GetEndPoint(1)).Direction;
                        //Oyakuiを作図
                        if (!clsOyakui.CreateSingleOyakui(doc, picPointS, dir, levelId))
                        {
                            //作図に失敗した時のコメント
                        }
                        continue;
                    }
                    if (clsOyakui.m_way == 4)
                    {
                        ElementId id1 = null;
                        if (!ClsKabeShin.PickBaseObject(uidoc, ref id1))
                        {
                            return;
                        }
                        FamilyInstance inst1 = doc.GetElement(id1) as FamilyInstance;
                        levelId = inst1.Host.Id;
                        Curve cv1 = (inst1.Location as LocationCurve).Curve;
                        XYZ dir1 = Line.CreateBound(cv1.GetEndPoint(0), cv1.GetEndPoint(1)).Direction;

                        ElementId id2 = null;
                        if (!ClsKabeShin.PickBaseObject(uidoc, ref id2))
                        {
                            return;
                        }
                        FamilyInstance inst2 = doc.GetElement(id2) as FamilyInstance;
                        Curve cv2 = (inst2.Location as LocationCurve).Curve;
                        XYZ dir2 = Line.CreateBound(cv2.GetEndPoint(0), cv2.GetEndPoint(1)).Direction;

                        picPointS = ClsRevitUtil.GetIntersection(cv1, cv2);
                        if (picPointS == null)
                        {
                            return;//交点が存在しない
                        }
                        //交点が始点に近い壁芯を1番目に設定
                        if(!ClsRevitUtil.CheckNearGetEndPoint(cv1, picPointS))
                        {
                            XYZ change = dir1;
                            dir1 = dir2;
                            dir2 = change;
                        }


                        XYZ dir = dir1 + dir2;

                        ////入隅か判定
                        bool bIrizumi = false;
                        if (!ClsHaraokoshiBase.CheckIrizumi(cv1, cv2, ref bIrizumi))
                        {
                            continue;
                        }
                        XYZ desumipoint = picPointS;
                        if (clsOyakui.m_putPosFlag != 0)
                        {
                            ElementId oya1 = null;
                            if (!ClsOyakui.PickObject(uidoc, ref oya1))
                                return;
                            ElementId oya2 = null;
                            if (!ClsOyakui.PickObject(uidoc, ref oya2))
                                return;
                            FamilyInstance instH1 = doc.GetElement(oya1) as FamilyInstance;
                            string stE1 = ClsYMSUtil.GetKouzaiSizeSunpou1(instH1.Symbol.FamilyName, 1);
                            double dEx1 = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(stE1) / 2);

                            FamilyInstance instH2 = doc.GetElement(oya2) as FamilyInstance;
                            string stE2 = ClsYMSUtil.GetKouzaiSizeSunpou1(instH2.Symbol.FamilyName, 1);
                            double dEx2 = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(stE2) / 2);

                            if(!ClsGeo.GEO_EQ(dir1, instH1.HandOrientation))
                            {
                                double dChange = dEx1;
                                dEx1 = dEx2;
                                dEx2 = dChange;
                            }

                            desumipoint += (dEx1 * dir2 - dEx2 * dir1);
                        }
                        double apexAngle = Math.PI - dir1.AngleOnPlaneTo(dir2, XYZ.BasisZ);
                        //Oyakuiを作図
                        if (!clsOyakui.CreateSingleOyakui(doc, desumipoint, dir, levelId, bIrizumi, true, id1, apexAngle))
                        {
                            //作図に失敗した時のコメント
                        }
                        continue;
                    }
                    //Oyakuiを作図
                    if (!clsOyakui.CreateOyakui(doc, picPointS, picPointE, levelId, true, selShinId, defVoid))
                    {
                        //作図に失敗した時のコメント
                    }
                }
                catch
                {
                    break;
                }
            }
            return;
        }
    }
}
