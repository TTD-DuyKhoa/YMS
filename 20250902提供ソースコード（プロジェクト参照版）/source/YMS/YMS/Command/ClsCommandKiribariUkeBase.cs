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
    class ClsCommandKiribariUkeBase
    {
        public static void CommandKiribariUkeBase(UIDocument uidoc)
        {
            //ドキュメントを取得
            Document doc = uidoc.Document;

            ////ワークセット
            //ClsWorkset clsWS = new ClsWorkset();
            //clsWS.SetWorkSetTest1(doc);

            //ダイアログを表示
            YMS.DLG.DlgCreateKiribariUkeBase KUB = new DLG.DlgCreateKiribariUkeBase();
            DialogResult result = KUB.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                return;
            }
            ClsKiribariUkeBase clsKUB = KUB.m_KiribariUkeBase;

            for (; ; )
            {
                try
                {
                    ElementId idKiri = null;
                    if (!ClsKiribariBase.PickBaseObject(uidoc, ref idKiri))
                    {
                        return;
                    }
                    FamilyInstance inst = doc.GetElement(idKiri) as FamilyInstance;
                    ElementId levelID = inst.Host.Id;
                    Curve cvKiri = (inst.Location as LocationCurve).Curve;
                    XYZ midKiri = (cvKiri.GetEndPoint(0) + cvKiri.GetEndPoint(1)) / 2;

                    //作図に必要な作図箇所の座標を取得
                    List<ElementId> ids = new List<ElementId>();
                    XYZ tmpStPoint, tmpEdPoint;
                    if (clsKUB.m_ShoriType == ClsKiribariUkeBase.ShoriType.Replace)
                    {
                        if (!ClsRevitUtil.PickObjects(uidoc, "切梁受けベースに置換するモデル線分", "モデル線分", ref ids))
                        {
                            return;
                        }
                        if (ids.Count < 1)
                        {
                            MessageBox.Show("モデル線分が選択されていません。");
                            return;
                        }

                        //置換の処理をここに記述
                        //MessageBox.Show("置換の処理は現在実装中です。");
                    }
                    else if (clsKUB.m_ShoriType == ClsKiribariUkeBase.ShoriType.PtoP)
                    {
                        (tmpStPoint, tmpEdPoint) = ClsRevitUtil.GetSelect2Point(uidoc);
                        if (tmpStPoint == null || tmpEdPoint == null) return;

                        //選択した切梁の中点を掘削側とし向きに対して左側に来るように変更
                        XYZ Direction = Line.CreateBound(tmpStPoint, tmpEdPoint).Direction;
                        XYZ kussakuDir = Line.CreateBound(tmpStPoint, midKiri).Direction;
                        if (!ClsGeo.IsLeft(Direction, kussakuDir))
                        {
                            //SとE入れ替え
                            XYZ p = tmpStPoint;
                            tmpStPoint = tmpEdPoint;
                            tmpEdPoint = p;
                        }

                        using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                        {
                            t.Start();
                            ModelLine line = ClsYMSUtil.CreateKabeHojyoLine(doc, tmpStPoint, tmpEdPoint, 0, 0);
                            t.Commit();
                            ids.Add(line.Id);
                        }

                        //2点間の処理をここに記述
                        //MessageBox.Show("2点間の処理は現在実装中です。");
                    }
                    else
                    {
                        ElementId id1 = null, id2 = null;

                        if (!ClsOyakui.PickBaseObject(uidoc, ref id1))
                        {
                            return;
                        }
                        if (!ClsOyakui.PickBaseObject(uidoc, ref id2))
                        {
                            return;
                        }

                        FamilyInstance inst1 = doc.GetElement(id1) as FamilyInstance;
                        LocationPoint lPoint1 = inst1.Location as LocationPoint;
                        if (lPoint1 == null)
                        {
                            return;
                        }
                        FamilyInstance inst2 = doc.GetElement(id2) as FamilyInstance;
                        LocationPoint lPoint2 = inst2.Location as LocationPoint;
                        if (lPoint2 == null)
                        {
                            return;
                        }

                        tmpStPoint = lPoint1.Point;
                        tmpEdPoint = lPoint2.Point;
                        var dir = Line.CreateBound(tmpStPoint, tmpEdPoint).Direction;

                        string kui1 = inst1.Symbol.FamilyName;
                        string H1 = ClsYMSUtil.GetKouzaiSizeSunpou1(kui1, 1);
                        string B1 = ClsYMSUtil.GetKouzaiSizeSunpou1(kui1, 2);

                        double dH1 = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(H1) / 2);
                        double dB1 = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(B1) / 2);

                        string kui2 = inst2.Symbol.FamilyName;
                        string H2 = ClsYMSUtil.GetKouzaiSizeSunpou1(kui2, 1);
                        string B2 = ClsYMSUtil.GetKouzaiSizeSunpou1(kui2, 2);

                        double dH2 = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(H2) / 2);
                        double dB2 = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(B2) / 2);

                        var wid1 = dH1; var wid2 = dH2; var len1 = dB1; var len2 = dB2;

                        //フランジ側作図かウェブ作図か判定
                        if (ClsGeo.GEO_EQ(dir, inst1.FacingOrientation) || ClsGeo.GEO_EQ(-dir, inst1.FacingOrientation))
                        {
                            //web側作成用サイズ代入
                            wid1 = dB1; len1 = dH1; wid2 = dB2; len2 = dH2;
                        }

                        using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                        {
                            t.Start();
                            //杭の芯から離れ量を指定するためここでは伸ばさない
                            ModelLine lineLeft = ClsYMSUtil.CreateHojyoLine(doc, tmpStPoint, tmpEdPoint, -wid1, -wid2, 0, 0);// len1, len2);
                            ModelLine lineRight = ClsYMSUtil.CreateHojyoLine(doc, tmpEdPoint, tmpStPoint, -wid1, -wid2, 0, 0);//len1, len2);
                            t.Commit();

                            var cLeft = (lineLeft.Location as LocationCurve).Curve;
                            var midLeft = (cLeft.GetEndPoint(0) + cLeft.GetEndPoint(1)) / 2;
                            midLeft = new XYZ(midLeft.X, midLeft.Y, 0);
                            var cRight = (lineRight.Location as LocationCurve).Curve;
                            var midRight = (cRight.GetEndPoint(0) + cRight.GetEndPoint(1)) / 2;
                            midRight = new XYZ(midRight.X, midRight.Y, 0);

                            XYZ picPoint = uidoc.Selection.PickPoint("作図側を指定してください");
                            picPoint = new XYZ(picPoint.X, picPoint.Y, 0);

                            var distLeft = picPoint.DistanceTo(midLeft);
                            var distRight = picPoint.DistanceTo(midRight);
                            //作図指定側に遠い方を削除する
                            if (distLeft < distRight)
                            {
                                ids.Add(lineLeft.Id);
                                t.Start();
                                ClsRevitUtil.Delete(doc, lineRight.Id);
                                t.Commit();
                            }
                            else
                            {
                                ids.Add(lineRight.Id);
                                t.Start();
                                ClsRevitUtil.Delete(doc, lineLeft.Id);
                                t.Commit();
                            }
                        }
                        //杭選択の処理をここに記述
                        //MessageBox.Show("杭選択の処理は現在実装中です。");
                    }

                    //切梁受けベースを作図
                    if (!clsKUB.CreateKiribariUkeBase(doc, ids, levelID))
                    {
                        //作図に失敗した時のコメント
                    }

                    //補助線を削除
                    using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                    {
                        t.Start();
                        ClsRevitUtil.Delete(doc, ids);
                        t.Commit();
                    }
                    if (clsKUB.m_ShoriType == ClsKiribariUkeBase.ShoriType.Replace)
                        break;
                }
                catch
                {
                    break;
                }
            }
            return;
        }

        public static bool CommandChangeKiribariUkeBase(UIDocument uidoc)
        {
            Document doc = uidoc.Document;

            // 対象となるベースを複数選択
            List<ElementId> idList = null;
            if (!ClsKiribariUkeBase.PickBaseObjects(uidoc, ref idList))
            {
                return false;
            }

            ClsKiribariUkeBase templateBase = new ClsKiribariUkeBase();
            for (int i = 0; i < idList.Count(); i++)
            {
                ElementId id = idList[i];
                if (i == 0)
                {
                    templateBase.SetParameter(doc, id);
                }
                else
                {
                    FamilyInstance shinInstLevel = doc.GetElement(id) as FamilyInstance;
                    ElementId levelID = shinInstLevel.Host.Id;
                    LocationCurve lCurve = shinInstLevel.Location as LocationCurve;
                    if (lCurve == null)
                    {
                        return false;
                    }

                    //元データを取得
                    string dan = ClsRevitUtil.GetParameter(doc, id, "段");
                    templateBase.m_SteelSize = ClsRevitUtil.CompareValues(ClsRevitUtil.GetParameter(doc, id, "鋼材サイズ"), templateBase.m_SteelSize);
                    templateBase.m_SteelType = ClsRevitUtil.CompareValues(ClsRevitUtil.GetParameter(doc, id, "鋼材タイプ"), templateBase.m_SteelType);
                    templateBase.m_TsukidashiRyoS = ClsRevitUtil.CompareValues((int)ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetParameterDouble(doc, id, "突き出し量始点")), templateBase.m_TsukidashiRyoS);
                    templateBase.m_TsukidashiRyoE = ClsRevitUtil.CompareValues((int)ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetParameterDouble(doc, id, "突き出し量終点")), templateBase.m_TsukidashiRyoE);
                }
            }

            //ダイアログを表示
            DLG.DlgCreateKiribariUkeBase kiribariUkeBase = new DLG.DlgCreateKiribariUkeBase(templateBase);
            DialogResult result = kiribariUkeBase.ShowDialog();
            if (result != DialogResult.OK)
            {
                return false;
            }

            foreach (var id in idList)
            {
                ClsKiribariUkeBase clsKiribariUkeBase = kiribariUkeBase.m_KiribariUkeBase;

                // 始点と終点を取得
                FamilyInstance shinInstLevel = doc.GetElement(id) as FamilyInstance;
                ElementId levelID = shinInstLevel.Host.Id;
                LocationCurve lCurve = shinInstLevel.Location as LocationCurve;
                if (lCurve == null)
                {
                    return false;
                }
                XYZ tmpStPoint = lCurve.Curve.GetEndPoint(0);
                XYZ tmpEdPoint = lCurve.Curve.GetEndPoint(1);

                string dan = ClsRevitUtil.GetParameter(doc, id, "段");

                //元のベースを削除
                using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                {
                    t.Start();
                    ClsRevitUtil.Delete(doc, id);
                    t.Commit();
                }

                clsKiribariUkeBase.ChangeKiribariUkeBase(doc, tmpStPoint, tmpEdPoint, dan, levelID);
            }

            return true;
        }
    }
}
