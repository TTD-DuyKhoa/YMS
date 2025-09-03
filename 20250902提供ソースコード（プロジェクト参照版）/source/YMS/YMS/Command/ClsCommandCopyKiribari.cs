using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YMS.DLG;
using YMS.Parts;

namespace YMS.Command
{
    class ClsCommandCopyKiribari
    {
        public static void CommandCopyKiribari(UIDocument uidoc) 
        {

            //ドキュメントを取得
            Document doc = uidoc.Document;
            List<ElementId> copyTargetBuzaiIds = new List<ElementId>();
            //List<ElementId> copyTargetOppBuzaiIds = new List<ElementId>();
            List<ElementId> targetKiribariBaseIds = new List<ElementId>();

            //切梁ベース指定
            ElementId idKiri = null;
            if (!ClsKiribariBase.PickBaseObject(uidoc, ref idKiri, "コピー元の切梁ベースを選択してください。"))
            {
                return;
            }
            FamilyInstance inst = doc.GetElement(idKiri) as FamilyInstance;
            ElementId levelID = inst.Host.Id;

            FamilyInstance kiribarBaseInst = doc.GetElement(idKiri) as FamilyInstance;
            Curve cvBase = (kiribarBaseInst.Location as LocationCurve).Curve;
            XYZ tmpStPoint = cvBase.GetEndPoint(0);
            XYZ tmpEdPoint = cvBase.GetEndPoint(1);
            XYZ dir = (tmpEdPoint - tmpStPoint).Normalize();



            string steelSizeOrign = string.Empty;
            double baseOffset = 0.0;
            steelSizeOrign = ClsRevitUtil.GetInstMojiParameter(doc, idKiri, "鋼材サイズ");
            double kouzaiSize = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(steelSizeOrign.Replace("HA", "")) * 10 / 2);
            if (steelSizeOrign.Contains("SMH"))
            {
                if (steelSizeOrign.Contains("80SMH"))
                {
                    kouzaiSize = ClsRevitUtil.CovertToAPI(400.0 / 2);
                }
                else if (steelSizeOrign.Contains("60SMH"))
                {
                    kouzaiSize = ClsRevitUtil.CovertToAPI(400.0 / 2);
                }
                else
                {
                    kouzaiSize = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(steelSizeOrign.Replace("SMH", "")) * 10 / 2);
                }
            }

            switch (ClsRevitUtil.GetParameter(doc, idKiri, "段"))
            {
                case "上段":
                    baseOffset = kouzaiSize;//+(kouzaiSize / 2);
                    break;
                case "下段":
                    baseOffset = -kouzaiSize;// - (kouzaiSize / 2);
                    break;
                case "同段":
                    baseOffset = 0.0;
                    break;
                default:
                    break;
            }
            //反対からも部材の確認
            //ClsYMSUtil.GetObjectOnBaseLine(doc, tmpStPoint, tmpEdPoint, ref copyTargetOppBuzaiIds);

            //仮鋼材の上のファミリが乗っていないポイントList
            //一旦オフセット
            tmpStPoint = new XYZ(tmpStPoint.X, tmpStPoint.Y, tmpStPoint.Z + baseOffset);
            tmpEdPoint = new XYZ(tmpEdPoint.X, tmpEdPoint.Y, tmpEdPoint.Z + baseOffset);
            
            List<List<XYZ>> notPojntLists = ClsYMSUtil.GetObjectOnBaseLine(doc, tmpStPoint, tmpEdPoint, ref copyTargetBuzaiIds);
            
            //foreach (ElementId id in copyTargetOppBuzaiIds)
            //{
            //    if (!copyTargetBuzaiIds.Contains(id))
            //    {
            //        copyTargetBuzaiIds.Add(id);
            //    }
            //}

            XYZ first = tmpStPoint, last = tmpEdPoint;
            for (int i = 0; i < notPojntLists.Count; i++)
            {
                if (i == 0)
                    first = notPojntLists[i][0];

                if (i == notPojntLists.Count - 1)
                    last = notPojntLists[i][1];
            }

            double stPntDist = tmpStPoint.DistanceTo(first);

            tmpStPoint = first;
            tmpEdPoint = last;

            if (copyTargetBuzaiIds.Count < 1)
            {
                MessageBox.Show("選択した部材にコピー可能な部材が存在していません。");
                return;
            }
            ClsRevitUtil.SelectElement(uidoc, copyTargetBuzaiIds);

            List<ElementId> idList = null;
            if (!ClsKiribariBase.PickBaseObjects(uidoc, ref idList,"コピー先の切梁ベース"))
            {
                return;
            }

            if (idList.Contains(idKiri))
            {
                MessageBox.Show("コピー元と同一の切梁ベースが選択されています。");
                return;
            }

            List<string> hojyoPeaceList = (Master.ClsSupportPieceCsv.GetFamilyNameList().ToList());
            List<string> jackList = (Master.ClsJackCsv.GetFamilyNameList().ToList());

            for (int i = 0; i < idList.Count; i++)
            {

                List<ElementId> deleteBuzaiIdList = ClsWarituke.GetConnectionBuzai(doc, idList[i]);

                //ジャッキカバーが削除対象に含まれていないため削除は個別で行う※ツインビームの場合は仮鋼材にジャッキカバーが含まれるため
                List<ElementId> jackCoverList = new List<ElementId>();
                foreach (var deleteId in deleteBuzaiIdList)
                {
                    ElementId jcid = null;
                    if (ClsJack.GetConnectedJackCover(doc, deleteId, ref jcid))
                    {
                        jackCoverList.Add(jcid);
                    }
                }

                using (Transaction t = new Transaction(doc, "仮鋼材削除"))
                {
                    t.Start();
                    ClsRevitUtil.Delete(doc, deleteBuzaiIdList);
                    ClsRevitUtil.Delete(doc, jackCoverList);
                    t.Commit();
                }


                ElementId id = idList[i];
                FamilyInstance targetBaseInst = doc.GetElement(id) as FamilyInstance;
                Curve cb = (targetBaseInst.Location as LocationCurve).Curve;
                XYZ ts = cb.GetEndPoint(0);
                XYZ te = cb.GetEndPoint(1);

                switch (ClsRevitUtil.GetParameter(doc, id, "段"))
                {
                    case "上段":
                        baseOffset = kouzaiSize;// + (kouzaiSize / 2);
                        break;
                    case "下段":
                        baseOffset = -kouzaiSize; //- (kouzaiSize / 2);
                        break;
                    case "同段":
                        baseOffset = 0.0;
                        break;
                    default:
                        break;
                }

                //仮鋼材の上のファミリが乗っていないポイントList
                //一旦オフセット
                ts = new XYZ(ts.X, ts.Y, ts.Z + baseOffset);
                te = new XYZ(te.X, te.Y, te.Z + baseOffset);

                XYZ newSp = ClsRevitUtil.MoveCoordinates(ts, te, stPntDist);
                XYZ moveVec = newSp - tmpStPoint;
                XYZ copyvec = (te - newSp).Normalize();
                double rad = dir.AngleOnPlaneTo(copyvec,XYZ.BasisZ);
                Line axis = Line.CreateBound(newSp, newSp + XYZ.BasisZ);


                string targetlevelName = targetBaseInst.Host.Name;
                ElementId targetlevelID = targetBaseInst.Host.Id;
                if (targetlevelID != null)
                {
                    Level lv = doc.GetElement(targetlevelID) as Level;
                    targetlevelName = lv.Name;
                }

                // コピー前の Level 一覧
                var beforeLevelIds = new FilteredElementCollector(doc)
                    .OfClass(typeof(Level)).ToElementIds().ToHashSet();

                using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                {
                    t.Start();
                    List<ElementId> newIds =  ElementTransformUtils.CopyElements(doc, copyTargetBuzaiIds, moveVec).ToList();
                    ElementTransformUtils.RotateElements(doc, newIds, axis, rad);

                    t.Commit();

                    t.Start();

                    
                    foreach (ElementId idt in newIds)
                    {
                        if (!string.IsNullOrWhiteSpace(targetlevelName) )
                        {
                            foreach (string name in hojyoPeaceList)
                            {
                                FamilyInstance newInst = doc.GetElement(idt) as FamilyInstance;
                                if (newInst != null && newInst.Symbol.FamilyName == name)//補助ピースのみフラグを付加
                                {
                                    ClsWarituke.SetWarituke(doc, idt);
                                }
                            }
                            foreach (string name in jackList)
                            {
                                FamilyInstance newInst = doc.GetElement(idt) as FamilyInstance;
                                if (newInst != null && newInst.Symbol.FamilyName == name)//ジャッキにフラグを付加
                                {
                                    // 基準IDを設定
                                    ClsKariKouzai.SetBaseId(doc, idt, id);
                                    ClsJack.SetBaseId(doc, idt, "割付");
                                }
                            }
                            ClsRevitUtil.SetParameter(doc, idt, "集計レベル", targetlevelID);
                            ClsRevitUtil.ModifyLevel(doc, idt, targetlevelID);
                        }
                    }


                    t.Commit();
                }

                // コピー後の Level 一覧と差分を検出
                var afterLevelIds = new FilteredElementCollector(doc)
                    .OfClass(typeof(Level)).ToElementIds().ToHashSet();
                var newLevelIds = afterLevelIds.Except(beforeLevelIds).ToList();
                //foreach (ElementId nid in newLevelIds)
                //{
                //    string name = RevitUtil.ClsRevitUtil.GetLevelName(doc, nid);
                //    int bbb = 0;
                //}

                
                if (newLevelIds.Count > 0)
                {
                    using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                    {
                        t.Start();
                        ClsRevitUtil.Delete(doc, newLevelIds);
                        t.Commit();
                    }
                }

            }
        }
    }
}
