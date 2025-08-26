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

namespace YMS.Command
{
    class ClsCommandRenzokukabe
    {
        public static void CommandRenzokukabe(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            //ドキュメントを取得
            Document doc = uidoc.Document;

            //Renzokukabeクラスを生成
            ClsRenzokukabe clsRenzokukabe = new ClsRenzokukabe();

            ////ワークセット
            //ClsWorkset clsWS = new ClsWorkset();
            //clsWS.SetWorkSetTest2(doc);

            //ダイアログを表示
            YMS.DLG.DlgCreateRenzokukabe Renzokukabe = new DLG.DlgCreateRenzokukabe(doc);
            //ClsGlobal.m_Renzokukabe = Renzokukabe;
            for (; ; )
            {
                DialogResult result = Renzokukabe.ShowDialog();
                if (result == DialogResult.Cancel)
                {
                    return;
                }
                else if (result == DialogResult.Retry)
                {
                    ElementId id = null;
                    if (!ClsRenzokukabe.PickObject(uidoc, ref id))
                        return;
                    clsRenzokukabe.SetParameter(doc, id);
                    Renzokukabe = new DLG.DlgCreateRenzokukabe(doc,clsRenzokukabe);
                }
                else
                    break;
            }
            clsRenzokukabe = Renzokukabe.m_ClsRenzokukabe;

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
                    if (clsRenzokukabe.m_way == 0)
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
                                clsRenzokukabe.CreateSingleRenzokukabe(doc, picPointS, dir, levelId, bIrizumi);
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
                            //Renzokukabeを作図
                            if (!clsRenzokukabe.CreateRenzokukabe(doc, picPointS, picPointE, levelId, bIrizumi, selShinId))
                            {
                                //作図に失敗した時のコメント
                            }
                        }
                        break;
                    }
                    if (clsRenzokukabe.m_way == 1)
                    {
                        if (!ClsKabeShin.PickBaseObject(uidoc, ref selShinId))//ClsRevitUtil.PickObject(uidoc, "壁芯", "壁芯", ref selShinId))
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
                    if (clsRenzokukabe.m_way == 2)
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

                        picPointS = selection.PickPoint(clsRenzokukabe.FamilyNameKui + "を配置する開始位置を選択してください");
                        if (picPointS == null)
                        {
                            return;
                        }
                        picPointE = selection.PickPoint(clsRenzokukabe.FamilyNameKui + "を配置する終了位置を選択してください");
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
                    if (clsRenzokukabe.m_way == 3)
                    {
                        ElementId id1 = null;
                        if (!ClsKabeShin.PickBaseObject(uidoc, ref id1))
                        {
                            return;
                        }
                        FamilyInstance inst1 = doc.GetElement(id1) as FamilyInstance;
                        Curve cv1 = (inst1.Location as LocationCurve).Curve;
                        levelId = inst1.Host.Id;
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

                        XYZ dir = dir1 + dir2;

                        ////入隅か判定
                        bool bIrizumi = false;
                        if (!ClsHaraokoshiBase.CheckIrizumi(cv1, cv2, ref bIrizumi))
                        {
                            continue;
                        }
                        //Oyakuiを作図
                        if (!clsRenzokukabe.CreateSingleRenzokukabe(doc, picPointS, dir, levelId, bIrizumi, true, id1))
                        {
                            //作図に失敗した時のコメント
                        }
                        continue;
                    }
                    //Renzokukabeを作図
                    if (!clsRenzokukabe.CreateRenzokukabe(doc, picPointS, picPointE, levelId, true, selShinId, defVoid))
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
