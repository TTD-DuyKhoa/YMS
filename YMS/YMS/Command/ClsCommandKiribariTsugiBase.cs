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
    class ClsCommandKiribariTsugiBase
    {
        public static void CommandKiribariTsugiBase(UIDocument uidoc)
        {
            //ドキュメントを取得
            Document doc = uidoc.Document;

            ////ワークセット
            //ClsWorkset clsWS = new ClsWorkset();
            //clsWS.SetWorkSetTest1(doc);

            //ダイアログを表示
            YMS.DLG.DlgCreateKiribariTsugiBase KT = new DLG.DlgCreateKiribariTsugiBase();
            DialogResult result = KT.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                return;
            }
            ClsKiribariTsugiBase clsKT = KT.m_KiribariTsugiBase;

            bool other = false;
            for (; ; )
            {
                //作図に必要な作図箇所の座標を取得
                List<ElementId> ids = new List<ElementId>();
                List<string> danList = new List<string>();
                string danBase;
                ElementId levelID = null;
                try
                {
                    if (clsKT.m_ShoriType == ClsKiribariTsugiBase.ShoriType.Point)
                    {
                        //切梁ベース指定
                        Reference rfPick = null;
                        if (!ClsKiribariBase.PickBaseObject(uidoc, ref rfPick))
                        {
                            return;
                        }
                        ElementId idKiri = rfPick.ElementId;
                        danList.Add(ClsRevitUtil.GetParameter(doc, idKiri, "段"));
                        danBase = ClsRevitUtil.GetParameter(doc, idKiri, "段");
                        //腹起ベース指定or切梁ベース
                        ElementId idPartner = null;
                        if (!ClsRevitUtil.PickObjectFilters(uidoc, "腹起ベースor切梁ベースを指定してください", ClsGlobal.m_baseKiriOrHaraList, ref idPartner))
                        {
                            return;
                        }
                        danList.Add(ClsRevitUtil.GetParameter(doc, idPartner, "段"));
                        if (!clsKT.CheckDan(doc, idKiri, idPartner)) return;

                        XYZ picPoint = rfPick.GlobalPoint;

                        Element selKiri = doc.GetElement(idKiri);
                        Element selPartner = doc.GetElement(idPartner);

                        //選択された切梁
                        FamilyInstance inst1 = selKiri as FamilyInstance;
                        levelID = inst1.Host.Id;
                        Curve cvKiri = (inst1.Location as LocationCurve).Curve;
                        XYZ dirKiri = Line.CreateBound(cvKiri.GetEndPoint(0), cvKiri.GetEndPoint(1)).Direction;
                        XYZ facOriKiri = inst1.FacingOrientation;

                        //選択されなかった火打
                        FamilyInstance inst2 = selPartner as FamilyInstance;
                        Curve cvPartner = (inst2.Location as LocationCurve).Curve;

                        //選択されなかった火打の中点を取得
                        XYZ midPartner = (cvPartner.GetEndPoint(0) + cvPartner.GetEndPoint(1)) / 2;

                        //選択した火打から見て選択していない火打の中点は左にあるかで向きを決める
                        XYZ dir1toMid = Line.CreateBound(cvKiri.GetEndPoint(0), midPartner).Direction;

                        if (ClsGeo.IsLeft(dirKiri, dir1toMid))
                        {
                            //左に90度
                            dirKiri = new XYZ(facOriKiri.X, facOriKiri.Y, 0);
                        }
                        else
                        {
                            //右に90度
                            dirKiri = new XYZ(-facOriKiri.X, -facOriKiri.Y, 0);
                        }



                        List<ElementId> filterList = clsKT.FindBaseToIntersection(doc, cvKiri);
                        //ピッチを決める腹起ベース指定
                        ElementId idhara = null;
                        if (!ClsRevitUtil.PickObjectElementFilter(uidoc, "ピッチを決める腹起ベースを選択してください", filterList, ref idhara))
                        {
                            return;
                        }
                        FamilyInstance inst3 = doc.GetElement(idhara) as FamilyInstance;
                        Curve cvHara = (inst3.Location as LocationCurve).Curve;
                        XYZ pitch = ClsRevitUtil.GetIntersection(cvHara, cvKiri);
                        Line pitchLine = Line.CreateBound(pitch, picPoint);
                        XYZ dirPitch = pitchLine.Direction;
                        double dPitchLine = ClsRevitUtil.CovertFromAPI(pitchLine.Length);

                        double d = dPitchLine % 100.0;
                        if (d < 50.0)
                        {
                            picPoint = new XYZ(picPoint.X - (ClsRevitUtil.CovertToAPI(d) * dirPitch.X),
                                               picPoint.Y - (ClsRevitUtil.CovertToAPI(d) * dirPitch.Y),
                                               picPoint.Z);
                            //insec = new XYZ(insec.X - (ClsRevitUtil.CovertToAPI(d) * dirPitch.X),
                            //                insec.Y - (ClsRevitUtil.CovertToAPI(d) * dirPitch.Y),
                            //                insec.Z);
                        }
                        else
                        {
                            picPoint = new XYZ(picPoint.X + (ClsRevitUtil.CovertToAPI(100.0 - d) * dirPitch.X),
                                               picPoint.Y + (ClsRevitUtil.CovertToAPI(100.0 - d) * dirPitch.Y),
                                               picPoint.Z);
                            //insec = new XYZ(insec.X + (ClsRevitUtil.CovertToAPI(100.0 - d) * dirPitch.X),
                            //                insec.Y + (ClsRevitUtil.CovertToAPI(100.0 - d) * dirPitch.Y),
                            //                insec.Z);
                        }

                        XYZ insec = null;
                        XYZ newPoint = new XYZ(picPoint.X + (ClsRevitUtil.CovertToAPI(int.MaxValue) * dirKiri.X),
                                               picPoint.Y + (ClsRevitUtil.CovertToAPI(int.MaxValue) * dirKiri.Y),
                                               picPoint.Z + (ClsRevitUtil.CovertToAPI(int.MaxValue) * dirKiri.Z));

                        Curve cvBase = Line.CreateBound(picPoint, newPoint);
                        insec = ClsRevitUtil.GetIntersection(cvPartner, cvBase);
                        if (insec == null)
                        {
                            MessageBox.Show("交点が見つからないため作成出来ません");
                            return;
                        }

                        Line kariLine = Line.CreateBound(picPoint, insec);
                        double dKariLine = ClsRevitUtil.CovertFromAPI(kariLine.Length);

                        if (clsKT.m_Kousei == ClsKiribariTsugiBase.Kousei.Double)
                        {
                            if (!clsKT.CheckLength(dKariLine)) return;
                            if (!clsKT.CheckIntersectionLength(dKariLine)) return;
                        }
                        ////腹起ベースが斜めの場合があるため交点が存在するかのさピッチをずらした後に交点が存在するかの確認//
                        //cvBase = Line.CreateBound(picPoint, insec);
                        //XYZ insecCheck = ClsRevitUtil.GetIntersection(cvPartner, cvBase);
                        //if (insecCheck == null)
                        //{
                        //    newPoint = new XYZ(picPoint.X + (ClsRevitUtil.CovertToAPI(int.MaxValue) * dirKiri.X),
                        //                           picPoint.Y + (ClsRevitUtil.CovertToAPI(int.MaxValue) * dirKiri.Y),
                        //                           picPoint.Z + (ClsRevitUtil.CovertToAPI(int.MaxValue) * dirKiri.Z));

                        //    cvBase = Line.CreateBound(picPoint, newPoint);
                        //    insec = ClsRevitUtil.GetIntersection(cvPartner, cvBase);
                        //}


                        clsKT.CreateKiribariTsugiBase(doc, picPoint, insec, danBase, levelID);

                        //if (clsKT.m_Kousei == ClsKiribariTsugiBase.Kousei.Double)
                        //{
                        //    XYZ newPoint = ClsRevitUtil.GetDistanceNewPoint(picPoint, dirKiri, clsKT.m_KiriSideTsunagiLength);
                        //    //切梁ベース側からの補助線作成
                        //    using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                        //    {
                        //        t.Start();
                        //        ModelLine line = ClsRevitUtil.CreateKabeHojyoLine(doc, picPoint, newPoint, 0, 0);
                        //        t.Commit();
                        //        ids.Add(line.Id);
                        //    }
                        //    newPoint = ClsRevitUtil.GetDistanceNewPoint(insec, -dirKiri, clsKT.m_HaraSideTsunagiLength);
                        //    //腹起ベース側からの補助線作成
                        //    using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                        //    {
                        //        t.Start();
                        //        ModelLine line = ClsRevitUtil.CreateKabeHojyoLine(doc, insec, newPoint, 0, 0);
                        //        t.Commit();
                        //        ids.Add(line.Id);
                        //    }
                        //}
                        //else
                        //{
                        //    using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                        //    {
                        //        t.Start();
                        //        ModelLine line = ClsRevitUtil.CreateKabeHojyoLine(doc, picPoint, insec, 0, 0);
                        //        t.Commit();
                        //        ids.Add(line.Id);
                        //    }
                        //}

                        //座標指定の処理をここに記述
                        //MessageBox.Show("座標指定の処理は現在実装中です。");
                    }
                    else if (clsKT.m_ShoriType == ClsKiribariTsugiBase.ShoriType.SanjikuPiece)
                    {
                        //三軸指定
                        ElementId idSanjiku = null;
                        if (!ClsRevitUtil.PickObjectFilters(uidoc, "三軸ピースを選択してください", ClsGlobal.m_sanjikuPeaceList, ref idSanjiku))
                        {
                            return;
                        }
                        Element selSanjiku = doc.GetElement(idSanjiku);
                        danBase = ClsRevitUtil.GetParameter(doc, idSanjiku, "段");
                        //選択された三軸ピース
                        FamilyInstance inst1 = selSanjiku as FamilyInstance;

                        XYZ picPoint = (inst1.Location as LocationPoint).Point;
                        XYZ dirSanjiku = inst1.FacingOrientation;//直進の向き
                        XYZ facOriKiri = inst1.HandOrientation;

                        //三軸ピースの点を中心に移動
                        picPoint = new XYZ(picPoint.X + (ClsRevitUtil.CovertToAPI(1500) * dirSanjiku.X),
                                               picPoint.Y + (ClsRevitUtil.CovertToAPI(1500) * dirSanjiku.Y),
                                               picPoint.Z);

                        ElementId idKiri = null;

                        //図面上の切梁ベースを全て取得
                        List<ElementId> baseIdList = ClsKiribariBase.GetAllKiribariBaseList(doc);
                        //三軸ピースと交点のある切梁ベースを探す//段を取得するため
                        foreach (ElementId baseId in baseIdList)
                        {
                            string dan = ClsRevitUtil.GetParameter(doc, baseId, "段");
                            string size = ClsRevitUtil.GetParameter(doc, baseId, "鋼材サイズ");
                            double kouzaiSize = ClsRevitUtil.CovertToAPI(Master.ClsYamadomeCsv.GetWidth(size) / 2);
                            FamilyInstance shinInst = doc.GetElement(baseId) as FamilyInstance;
                            LocationCurve lCurveKiri = shinInst.Location as LocationCurve;
                            if (lCurveKiri != null)
                            {
                                XYZ tmpStPoint = lCurveKiri.Curve.GetEndPoint(0);
                                XYZ tmpEdPoint = lCurveKiri.Curve.GetEndPoint(1);
                                //選択された芯の段によって調整
                                if (dan == "上段")
                                {
                                    tmpStPoint = new XYZ(tmpStPoint.X, tmpStPoint.Y, tmpStPoint.Z + kouzaiSize);
                                    tmpEdPoint = new XYZ(tmpEdPoint.X, tmpEdPoint.Y, tmpEdPoint.Z + kouzaiSize);
                                }
                                else if (dan == "下段")
                                {
                                    tmpStPoint = new XYZ(tmpStPoint.X, tmpStPoint.Y, tmpStPoint.Z - kouzaiSize);
                                    tmpEdPoint = new XYZ(tmpEdPoint.X, tmpEdPoint.Y, tmpEdPoint.Z - kouzaiSize);
                                }
                                Curve cvBaseKiri = Line.CreateBound(tmpStPoint, tmpEdPoint);
                                if (ClsRevitUtil.IsPointOnCurve(cvBaseKiri, picPoint))
                                {
                                    idKiri = baseId;
                                    danList.Add(dan);//
                                    if (dan == "上段")
                                    {
                                        picPoint = new XYZ(picPoint.X, picPoint.Y, picPoint.Z - kouzaiSize);
                                    }
                                    else if (dan == "下段")
                                    {
                                        picPoint = new XYZ(picPoint.X, picPoint.Y, picPoint.Z + kouzaiSize);
                                    }
                                    break;
                                }
                            }
                        }
                        if (idKiri == null) return;


                        //腹起ベース指定or切梁ベース
                        ElementId idPartner = null;
                        if (!ClsRevitUtil.PickObjectFilters(uidoc, "腹起ベースor切梁ベースを指定してください", ClsGlobal.m_baseKiriOrHaraList, ref idPartner))
                        {
                            return;
                        }
                        danList.Add(ClsRevitUtil.GetParameter(doc, idPartner, "段"));

                        Element selPartner = doc.GetElement(idPartner);
                        if (!clsKT.CheckDan(doc, idKiri, idPartner)) return;

                        //腹起ベース指定or切梁ベース
                        FamilyInstance inst2 = selPartner as FamilyInstance;
                        Curve cvPartner = (inst2.Location as LocationCurve).Curve;
                        levelID = inst2.Host.Id;

                        //選択されなかった火打の中点を取得
                        XYZ midPartner = (cvPartner.GetEndPoint(0) + cvPartner.GetEndPoint(1)) / 2;

                        //選択した火打から見て選択していない火打の中点は左にあるかで向きを決める
                        XYZ dir1toMid = Line.CreateBound(picPoint, midPartner).Direction;

                        if (ClsGeo.IsLeft(dirSanjiku, dir1toMid))
                        {
                            //左に90度
                            dirSanjiku = ClsRevitUtil.RotationDirection(dirSanjiku, Math.PI / 2);
                        }
                        else
                        {
                            //右に90度
                            dirSanjiku = ClsRevitUtil.RotationDirection(dirSanjiku, -Math.PI / 2);
                        }

                        XYZ insec = null;
                        //for (int i = 1; i < int.MaxValue; i++)
                        //{
                        //100増やすかは未定
                        XYZ newPoint = new XYZ(picPoint.X + (ClsRevitUtil.CovertToAPI(int.MaxValue) * dirSanjiku.X),
                                               picPoint.Y + (ClsRevitUtil.CovertToAPI(int.MaxValue) * dirSanjiku.Y),
                                               picPoint.Z);

                        Curve cvBase = Line.CreateBound(picPoint, newPoint);
                        insec = ClsRevitUtil.GetIntersection(cvPartner, cvBase);
                        //    if (insec != null)
                        //    {
                        //        break;
                        //    }
                        //}
                        if (insec == null)
                        {
                            MessageBox.Show("交点が見つからないため作成出来ません");
                            return;
                        }

                        Line kariLine = Line.CreateBound(picPoint, insec);
                        double dKariLine = ClsRevitUtil.CovertFromAPI(kariLine.Length);

                        if (clsKT.m_Kousei == ClsKiribariTsugiBase.Kousei.Double)
                        {
                            if (!clsKT.CheckLength(dKariLine)) return;
                            if (!clsKT.CheckIntersectionLength(dKariLine)) return;
                        }

                        //三軸ピースの場合は端部部品（切梁側が付かない
                        clsKT.m_KiriSideParts = "なし";

                        clsKT.CreateKiribariTsugiBase(doc, picPoint, insec, danBase, levelID);

                        //if (clsKT.m_Kousei == ClsKiribariTsugiBase.Kousei.Double)
                        //{
                        //    XYZ newPoint = ClsRevitUtil.GetDistanceNewPoint(picPoint, dirSanjiku, clsKT.m_KiriSideTsunagiLength);
                        //    //切梁ベース側からの補助線作成
                        //    using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                        //    {
                        //        t.Start();
                        //        ModelLine line = ClsRevitUtil.CreateKabeHojyoLine(doc, picPoint, newPoint, 0, 0);
                        //        t.Commit();
                        //        ids.Add(line.Id);
                        //    }
                        //    newPoint = ClsRevitUtil.GetDistanceNewPoint(insec, -dirSanjiku, clsKT.m_HaraSideTsunagiLength);
                        //    //腹起ベース側からの補助線作成
                        //    using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                        //    {
                        //        t.Start();
                        //        ModelLine line = ClsRevitUtil.CreateKabeHojyoLine(doc, insec, newPoint, 0, 0);
                        //        t.Commit();
                        //        ids.Add(line.Id);
                        //    }
                        //}
                        //else
                        //{
                        //    using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                        //    {
                        //        t.Start();
                        //        ModelLine line = ClsRevitUtil.CreateKabeHojyoLine(doc, picPoint, insec, 0, 0);
                        //        t.Commit();
                        //        ids.Add(line.Id);
                        //    }
                        //}
                        //三軸ピースの処理をここに記述
                        //MessageBox.Show("三軸ピースの処理は現在実装中です。");
                    }
                    else
                    {
                        if (!ClsRevitUtil.PickObjects(uidoc, "切梁継ぎベースに置換するモデル線分", "モデル線分", ref ids))
                        {
                            return;
                        }
                        if (ids.Count < 1)
                        {
                            MessageBox.Show("モデル線分が選択されていません。");
                            return;
                        }

                        //レベルを合わせる腹起ベースを選択
                        ElementId idHiuchi = null;
                        if (!ClsHaraokoshiBase.PickBaseObject(uidoc, ref idHiuchi, "レベル段を合わせる腹起ベースを指定してください"))
                        {
                            return;
                        }
                        FamilyInstance inst = doc.GetElement(idHiuchi) as FamilyInstance;
                        levelID = inst.Host.Id;
                        for (int i = 0; i < ids.Count; i++)
                        {
                            danList.Add(ClsRevitUtil.GetParameter(doc, idHiuchi, "段"));
                        }
                        //置換の処理をここに記述
                        //MessageBox.Show("置換の処理は現在実装中です。");
                        //切梁繋ぎを作図
                        if (!clsKT.CreateKiribariTsugiBase(doc, ids, danList, levelID))
                        {
                            //作図に失敗した時のコメント
                        }
                        other = true;
                    }

                    ////切梁繋ぎを作図
                    //if (!clsKT.CreateKiribariTsugiBase(doc, ids, danList, levelID))
                    //{
                    //    //作図に失敗した時のコメント
                    //}

                    //補助線を削除
                    using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                    {
                        t.Start();
                        ClsRevitUtil.Delete(doc, ids);
                        t.Commit();
                    }
                    if (other)
                        break;
                }
                catch
                {
                    break;
                }
            }
            return;
        }

        public static bool CommandChangeKiribariTsugiBase(UIDocument uidoc)
        {
            Document doc = uidoc.Document;

            // 対象となるベースを複数選択
            List<ElementId> idList = null;
            if (!ClsKiribariTsugiBase.PickBaseObjects(uidoc, ref idList))
            {
                return false;
            }

            ClsKiribariTsugiBase templateBase = new ClsKiribariTsugiBase();
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
                    templateBase.m_SteelType = ClsRevitUtil.CompareValues(ClsRevitUtil.GetParameter(doc, id, "鋼材タイプ"), templateBase.m_SteelType);
                    templateBase.m_Kousei = ClsRevitUtil.CompareValues(ClsRevitUtil.GetParameter(doc, id, "構成") == "シングル" ? ClsKiribariTsugiBase.Kousei.Single : ClsKiribariTsugiBase.Kousei.Double, templateBase.m_Kousei);
                    templateBase.m_SteelSizeSingle = ClsRevitUtil.CompareValues(ClsRevitUtil.GetParameter(doc, id, "鋼材サイズ(シングル)"), templateBase.m_SteelSizeSingle);
                    templateBase.m_KiriSideSteelSizeDouble = ClsRevitUtil.CompareValues(ClsRevitUtil.GetParameter(doc, id, "切梁側/鋼材サイズ(ダブル)"), templateBase.m_KiriSideSteelSizeDouble);
                    templateBase.m_KiriSideTsunagiLength = ClsRevitUtil.CompareValues((int)ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetParameterDouble(doc, id, "切梁側/切梁繋ぎ長さ")), templateBase.m_KiriSideTsunagiLength);
                    templateBase.m_KiriSideParts = ClsRevitUtil.CompareValues(ClsRevitUtil.GetParameter(doc, id, "切梁側/部品"), templateBase.m_KiriSideParts);
                    templateBase.m_HaraSideSteelSizeDouble = ClsRevitUtil.CompareValues(ClsRevitUtil.GetParameter(doc, id, "腹起側/鋼材サイズ(ダブル)"), templateBase.m_HaraSideSteelSizeDouble);
                    templateBase.m_HaraSideTsunagiLength = ClsRevitUtil.CompareValues((int)ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetParameterDouble(doc, id, "腹起側/切梁繋ぎ長さ")), templateBase.m_HaraSideTsunagiLength);
                    templateBase.m_HaraSideParts = ClsRevitUtil.CompareValues(ClsRevitUtil.GetParameter(doc, id, "腹起側/部品"), templateBase.m_HaraSideParts);
                }
            }

            //ダイアログを表示
            DLG.DlgCreateKiribariTsugiBase kiribariTsugiBase = new DLG.DlgCreateKiribariTsugiBase(templateBase);
            DialogResult result = kiribariTsugiBase.ShowDialog();
            if (result != DialogResult.OK)
            {
                return false;
            }

            foreach (var id in idList)
            {
                ClsKiribariTsugiBase clsKiribariTsugiBase = kiribariTsugiBase.m_KiribariTsugiBase;

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

                clsKiribariTsugiBase.CreateKiribariTsugiBase(doc, tmpStPoint, tmpEdPoint, dan, levelID);
            }
            
            return true;

        }
    }
}
