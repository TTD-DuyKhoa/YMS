using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
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
    class ClsCommandSyabariTsunagizaiBase
    {
        public static void CommandCreateSyabariTsunagizaiBase(UIDocument uidoc)
        {
            //ドキュメントを取得
            Document doc = uidoc.Document;

            ////ワークセット
            //ClsWorkset clsWS = new ClsWorkset();
            //clsWS.SetWorkSetTest1(doc);

            //ダイアログを表示
            YMS.DLG.DlgCreateSyabariTsunagizaiBase STB = new DLG.DlgCreateSyabariTsunagizaiBase();
            var result = STB.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                return;
            }
            ClsSyabariTsunagizaiBase clsSTB = STB.m_SyabariTsunagizaiBase;

            for (; ; )
            {
                //作図に必要な作図箇所の座標を取得
                List<ElementId> ids = new List<ElementId>();
                ElementId levelID = null;
                try
                {
                    if (clsSTB.m_ShoriType == ClsSyabariTsunagizaiBase.ShoriType.Replace)
                    {
                        ////斜梁1本目
                        //ElementId id = null;
                        //if (!ClsSyabariBase.PickBaseObject(uidoc, ref id, "レベルを合わせる斜梁ベース"))
                        //{
                        //    return;
                        //}
                        //FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                        //var cv = (inst.Location as LocationCurve).Curve;
                        //levelID = ClsRevitUtil.GetParameterElementId(doc, id, "集計レベル");
                        //斜梁1本目
                        ElementId id1 = null;
                        ElementId id1_s = null;
                        //if (!ClsSyabariBase.PickBaseObject(uidoc, ref id1))
                        //{
                        //    return;
                        //}
                        if (!ClsSyabariTsunagizaiBase.PickObject(uidoc, ref id1_s))
                        {
                            return;
                        }
                        id1 = YMS.ClsKariKouzai.GetBaseId(doc, id1_s);
                        if (id1 == new ElementId(0)) id1 = id1_s;

                        //斜梁2本目
                        ElementId id2 = null;
                        ElementId id2_s = null;
                        //if (!ClsSyabariBase.PickBaseObject(uidoc, ref id2))
                        //{
                        //    return;
                        //}
                        if (!ClsSyabariTsunagizaiBase.PickObject(uidoc, ref id2_s))
                        {
                            return;
                        }
                        id2 = YMS.ClsKariKouzai.GetBaseId(doc, id2_s);
                        if (id2 == new ElementId(0)) id2 = id2_s;

                        ClsSyabariTsunagizaiBase.RevercePick(doc, ref id1, ref id2);

                        FamilyInstance inst1 = doc.GetElement(id1) as FamilyInstance;
                        var length1 = ClsRevitUtil.GetParameterDouble(doc, id1, "長さ");
                        var tmpStPoint1 = (inst1.Location as LocationPoint).Point;
                        var tmpEdPoint1 = tmpStPoint1 + length1 * inst1.HandOrientation;
                        Curve cv1 = Line.CreateBound(tmpStPoint1, tmpEdPoint1);
                        levelID = ClsRevitUtil.GetParameterElementId(doc, id1, "集計レベル");

                        FamilyInstance inst2 = doc.GetElement(id2) as FamilyInstance;
                        var length2 = ClsRevitUtil.GetParameterDouble(doc, id2, "長さ");
                        var tmpStPoint2 = (inst2.Location as LocationPoint).Point;
                        var tmpEdPoint2 = tmpStPoint2 + length2 * inst2.HandOrientation;
                        
                        Curve cv2 = Line.CreateBound(tmpStPoint2, tmpEdPoint2);

                        if (!ClsRevitUtil.PickObjects(uidoc, "斜梁繋ぎ材ベースに置換するモデル線分", "モデル線分", ref ids))
                        {
                            return;
                        }
                        if (ids.Count < 1)
                        {
                            MessageBox.Show("モデル線分が選択されていません。");
                            return;
                        }

                        foreach (var lineId in ids)
                        {
                            var line = (doc.GetElement(lineId).Location as LocationCurve).Curve;
                            var tmpStPointH = ClsRevitUtil.GetIntersectionZ0(cv1, line, true);
                            var tmpEdPointH = ClsRevitUtil.GetIntersectionZ0(cv2, line, true);
                            var tmpStPointU = line.GetEndPoint(0);
                            var tmpEdPointU = line.GetEndPoint(1);

                            var lh = Line.CreateBound(tmpStPointH, tmpEdPointH);
                            var ls = Line.CreateBound(tmpStPointU, tmpStPointU + XYZ.BasisZ);
                            var le = Line.CreateBound(tmpEdPointU, tmpEdPointU + XYZ.BasisZ);

                            var tmpStPoint = new XYZ();
                            ClsSyabariTsunagizaiBase.TryGetIntersection(lh, ls, out tmpStPoint);
                            var tmpEdPoint = new XYZ();
                            ClsSyabariTsunagizaiBase.TryGetIntersection(lh, le, out tmpEdPoint);

                            var tmpThirdPoint = tmpStPoint2;


                            ClsSyabariTsunagizaiBase.ReverceTsunagiBaseVec(doc, id1, ref tmpStPoint, ref tmpEdPoint);

                            //右にある場合は法線ベクトルが逆に出来るため終点を参照面作成の3点目にする
                            if (!ClsGeo.IsLeft((tmpEdPoint - tmpStPoint).Normalize(), (tmpThirdPoint - tmpStPoint).Normalize()))
                            {
                                tmpThirdPoint = tmpEdPoint2;
                            }
                            
                        //lsSyabariTsunagizaiBase.EnsureReferencePlaneRightHanded(ref tmpStPoint, ref tmpEdPoint, ref tmpThirdPoint);

                        //if (ClsSyabariTsunagizaiBase.IsReferencePlaneFacingDown(tmpStPoint, tmpEdPoint, tmpThirdPoint))
                        //{
                        //    tmpThirdPoint = tmpThirdPoint + length2 * inst2.HandOrientation; 
                        //}


                            if (!clsSTB.CreateSyabariTsunagizaiBase(doc, tmpStPoint, tmpEdPoint, tmpThirdPoint, levelID))
                                continue;
                            if(!clsSTB.CreateSyabariTsunagizai(doc, id1, id2, clsSTB.m_ElementId))
                                return;
                        }
                    }
                    else
                    {

                        List<ElementId> filterList = new List<ElementId>();
                        //斜梁1本目
                        Reference id1_s = null;
                        //if (!ClsSyabariBase.PickBaseObject(uidoc, ref id1))
                        //{
                        //    return;
                        //}
                        if (!ClsSyabariTsunagizaiBase.PickObject(uidoc, ref id1_s,"斜梁"))
                        {
                            return;
                        }
                        ElementId id1 = YMS.ClsKariKouzai.GetBaseId(doc, id1_s.ElementId);
                        //filterList.Add(id1);

                        levelID = ClsRevitUtil.GetParameterElementId(doc, id1, "集計レベル");
                        //斜梁2本目
                        Reference id2_s = null;
                        //if (!ClsSyabariBase.PickBaseObject(uidoc, ref id2))
                        //{
                        //    return;
                        //}
                        if (!ClsSyabariTsunagizaiBase.PickObject(uidoc, ref id2_s, "斜梁"))
                        {
                            return;
                        }
                        ElementId id2 =YMS.ClsKariKouzai.GetBaseId(doc, id2_s.ElementId);

                        ClsSyabariTsunagizaiBase.RevercePick(doc, ref id1, ref id2);

                        //filterList.Add(id2);
                        FamilyInstance inst2 = doc.GetElement(id2) as FamilyInstance;
                        var length2 = ClsRevitUtil.GetParameterDouble(doc, id2, "長さ");

                        var tmpStPoint = id1_s.GlobalPoint;
                        var tmpEdPoint = id2_s.GlobalPoint;

                        //分割の交点が見つからない
                        if (clsSTB.m_SplitFlg && false)
                        {
                            Curve cvKari = Line.CreateBound(tmpStPoint, tmpEdPoint);
                            List<ElementId> List = ClsSyabariBase.GetAllBaseList(doc);
                            XYZ p1 = tmpStPoint;
                            XYZ p2 = tmpStPoint;
                            foreach (ElementId Id in List)
                            {
                                if (Id == id1_s.ElementId || Id == id2_s.ElementId) continue;

                                FamilyInstance inst = doc.GetElement(Id) as FamilyInstance;
                                var cv = ClsYMSUtil.GetBaseLine(doc, Id);
                                p2 = ClsRevitUtil.GetIntersection(cvKari, cv);
                                if (p2 != null)
                                {
                                    if (!clsSTB.CreateSyabariTsunagizaiBase(doc, p1, p2, (inst2.Location as LocationPoint).Point, levelID))
                                        continue;
                                    p1 = p2;
                                }

                            }

                            tmpStPoint = p2;
                        }
                        var dSize1 = Master.ClsYamadomeCsv.GetWidth(ClsRevitUtil.GetParameter(doc, id1, "鋼材サイズ")) / 2;
                        var dSize2 = Master.ClsYamadomeCsv.GetWidth(ClsRevitUtil.GetParameter(doc, id2, "鋼材サイズ")) / 2;
                        var kariLine = Line.CreateBound(tmpStPoint, tmpEdPoint);
                        var dir = kariLine.Direction;
                        var dKariLine = ClsRevitUtil.CovertFromAPI(kariLine.Length);
                        for (int i = 0; i < int.MaxValue; i++)
                        {
                            double d = 500.0 * i - dKariLine;
                            if (0 < d)
                            {
                                if (d <= 100.0 + dSize1 + dSize2)
                                {
                                    i += 1;
                                }
                                d = 500.0 * i - dKariLine - dSize1 - dSize2;
                                tmpStPoint = new XYZ(tmpStPoint.X - (ClsRevitUtil.CovertToAPI(dSize1 + d / 2) * dir.X),
                                                   tmpStPoint.Y - (ClsRevitUtil.CovertToAPI(dSize1 + d / 2) * dir.Y),
                                                   tmpStPoint.Z - (ClsRevitUtil.CovertToAPI(dSize1 + d / 2) * dir.Z));
                                tmpEdPoint = new XYZ(tmpEdPoint.X + (ClsRevitUtil.CovertToAPI(dSize2 + d / 2) * dir.X),
                                                tmpEdPoint.Y + (ClsRevitUtil.CovertToAPI(dSize2 + d / 2) * dir.Y),
                                                tmpEdPoint.Z + (ClsRevitUtil.CovertToAPI(dSize2 + d / 2) * dir.Z));
                                break;
                            }
                        }

                        ClsSyabariTsunagizaiBase.ReverceTsunagiBaseVec(doc, id1, ref tmpStPoint, ref tmpEdPoint);

                        var tmpThirdPoint = (inst2.Location as LocationPoint).Point;
                        //右にある場合は法線ベクトルが逆に出来るため終点を参照面作成の3点目にする
                        if (!ClsGeo.IsLeft((tmpEdPoint - tmpStPoint).Normalize(), (tmpThirdPoint - tmpStPoint).Normalize()))
                        {
                            tmpThirdPoint = tmpThirdPoint + length2 * inst2.HandOrientation; ;
                        }
                        //XYZ direction = (tmpStPoint - tmpEdPoint).Normalize();
                        //if (direction.Z < 0)
                        //{
                        //    tmpThirdPoint = tmpThirdPoint + length2 * inst2.HandOrientation; ;
                        //}

                        //ClsSyabariTsunagizaiBase.EnsureReferencePlaneRightHanded(ref tmpStPoint, ref tmpEdPoint, ref tmpThirdPoint);

                        //if (ClsSyabariTsunagizaiBase.IsReferencePlaneFacingDown(tmpStPoint, tmpEdPoint, tmpThirdPoint))
                        //{
                        //    tmpThirdPoint = tmpThirdPoint + length2 * inst2.HandOrientation; 
                        //}

                        if (!clsSTB.CreateSyabariTsunagizaiBase(doc, tmpStPoint, tmpEdPoint, tmpThirdPoint, levelID))
                            return;
                        if (!clsSTB.CreateSyabariTsunagizai(doc, id1, id2, clsSTB.m_ElementId))
                            return;
                    }
                    //補助線を削除
                    using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                    {
                        t.Start();
                        ClsRevitUtil.Delete(doc, ids);
                        t.Commit();
                    }
                    if (clsSTB.m_ShoriType == ClsSyabariTsunagizaiBase.ShoriType.Replace)
                        break;
                }
                catch
                {
                    break;
                }
            }
            return;
        }


        public static void CommandChangeSyabariTsunagizaiBase(UIDocument uidoc)
        {
            //ドキュメントを取得
            Document doc = uidoc.Document;

            ////ワークセット
            //ClsWorkset clsWS = new ClsWorkset();
            //clsWS.SetWorkSetTest1(doc);

            List<ElementId> ids = null;
            if (!ClsSyabariTsunagizaiBase.PickBaseObjects(uidoc, ref ids))
                return;
            if (ids.Count < 1)
                return;

            ClsSyabariTsunagizaiBase clsSTB = new ClsSyabariTsunagizaiBase();
            //基本同じものを選択している想定。違う場合でも最終的には同じパラメータに直すので1つ目のデータを採用
            clsSTB.SetClassParameter(doc, ids[0]);

            //ダイアログを表示
            YMS.DLG.DlgCreateSyabariTsunagizaiBase STB = new DLG.DlgCreateSyabariTsunagizaiBase(clsSTB);
            var result = STB.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                return;
            }
            clsSTB = STB.m_SyabariTsunagizaiBase;

            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                t.Start();
                foreach (var id in ids)
                {
                    clsSTB.SetParameter(doc, id);
                }
                t.Commit();
            }
            return;
        }
    }
}
