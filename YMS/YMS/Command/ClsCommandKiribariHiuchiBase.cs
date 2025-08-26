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
    class ClsCommandKiribariHiuchiBase
    {
        public static void CommandKiribariHiuchiBase(UIDocument uidoc)
        {
            //ドキュメントを取得
            Document doc = uidoc.Document;


            ////ワークセット
            //ClsWorkset clsWS = new ClsWorkset();
            //clsWS.SetWorkSetTest1(doc);

            //ダイアログを表示
            YMS.DLG.DlgCreateKiribariHiuchiBase KHB = new DLG.DlgCreateKiribariHiuchiBase();
            DialogResult result = KHB.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                return;
            }
            //切梁火打ベースクラスを生成
            ClsKiribariHiuchiBase clsKHB = KHB.m_KiribariHiuchiBase;

            ElementId levelID = null;
            string dan;
            if (clsKHB.m_ShoriType == ClsKiribariHiuchiBase.ShoriType.KiriKiri)
            {
                for (; ; )
                {
                    try
                    {
                        //切梁ベース指定
                        Reference rfKiri1 = null;
                        if (!ClsKiribariBase.PickBaseObject(uidoc, ref rfKiri1))
                        {
                            return;
                        }
                        ElementId idKiri1 = rfKiri1.ElementId;
                        //切梁ベース指定
                        Reference rfKiri2 = null;
                        if (!ClsKiribariBase.PickBaseObject(uidoc, ref rfKiri2))
                        {
                            return;
                        }
                        ElementId idKiri2 = rfKiri2.ElementId;

                        if (!clsKHB.CheckDan(doc, idKiri1, idKiri2)) return;

                        FamilyInstance inst1 = doc.GetElement(idKiri1) as FamilyInstance;
                        levelID = inst1.Host.Id;
                        dan = ClsRevitUtil.GetParameter(doc, idKiri1, "段");


                        clsKHB.CreateKiribariHiuchiBase(uidoc, idKiri1, idKiri2, levelID, dan, rfKiri1.GlobalPoint, rfKiri2.GlobalPoint);
                    }
                    catch
                    {
                        break;
                    }
                }
            }
            else if (clsKHB.m_ShoriType == ClsKiribariHiuchiBase.ShoriType.KiriHara)
            {
                //作図に必要な作図箇所の座標を取得
                if (clsKHB.m_ShoriHoho == ClsKiribariHiuchiBase.ShoriHoho.Auto)
                {

                    //腹起ベース指定
                    ElementId idHara = null;
                    if (!ClsHaraokoshiBase.PickBaseObject(uidoc, ref idHara))
                    {
                        return;
                    }
                    FamilyInstance inst1 = doc.GetElement(idHara) as FamilyInstance;
                    levelID = inst1.Host.Id;

                    //腹起ベース
                    List<ElementId> haraList = ClsHaraokoshiBase.GetAllHaraokoshiBaseList(doc);
                    List<ElementId> kiriList = ClsKiribariBase.GetAllKiribariBaseList(doc);
                    foreach (ElementId haraId in haraList)
                    {
                        FamilyInstance instHara = doc.GetElement(haraId) as FamilyInstance;
                        //同じレベルの腹起ベースのみで作成レベル比較できない
                        if (levelID != instHara.Host.Id) continue;
                        Curve cvInstHara = (instHara.Location as LocationCurve).Curve;
                        if (cvInstHara == null) continue;
                        dan = ClsRevitUtil.GetParameter(doc, haraId, "段");
                        foreach (ElementId kiriId in kiriList)
                        {
                            FamilyInstance instKiri = doc.GetElement(kiriId) as FamilyInstance;
                            Curve cvInstKiri = (instKiri.Location as LocationCurve).Curve;
                            if (cvInstKiri == null) continue;
                            XYZ insec = ClsRevitUtil.GetIntersection(cvInstHara, cvInstKiri);
                            if (insec != null)
                            {
                                clsKHB.CreateKiribariHiuchiBase(uidoc, kiriId, haraId, levelID, dan);
                            }
                        }
                    }

                    //自動の処理をここに記述
                    //MessageBox.Show("自動の処理は現在実装中です。");
                }
                else if (clsKHB.m_ShoriHoho == ClsKiribariHiuchiBase.ShoriHoho.Manual)
                {
                    for (; ; )
                    {
                        try
                        {
                            //切梁ベース指定
                            ElementId idKiri = null;
                            if (!ClsKiribariBase.PickBaseObject(uidoc, ref idKiri))
                            {
                                return;
                            }

                            //腹起ベース指定
                            Reference rfHara = null;
                            if (!ClsHaraokoshiBase.PickBaseObject(uidoc, ref rfHara))
                            {
                                return;
                            }
                            ElementId idHara = rfHara.ElementId;

                            if (!clsKHB.CheckDan(doc, idKiri, idHara)) return;

                            FamilyInstance inst1 = doc.GetElement(idKiri) as FamilyInstance;
                            levelID = inst1.Host.Id;
                            dan = ClsRevitUtil.GetParameter(doc, idKiri, "段");

                            clsKHB.CreateKiribariHiuchiBase(uidoc, idKiri, idHara, levelID, dan, null, rfHara.GlobalPoint);
                            //手動の処理をここに記述
                            //MessageBox.Show("手動の処理は現在実装中です。");
                        }
                        catch
                        {
                            break;
                        }
                    }
                }
                else
                {
                    //腹起ベース指定
                    ElementId idHara = null;
                    if (!ClsHaraokoshiBase.PickBaseObject(uidoc, ref idHara))
                    {
                        return;
                    }
                    FamilyInstance inst1 = doc.GetElement(idHara) as FamilyInstance;
                    levelID = inst1.Host.Id;
                    dan = ClsRevitUtil.GetParameter(doc, idHara, "段");

                    List<ElementId> ids = new List<ElementId>();
                    if (!ClsRevitUtil.PickObjects(uidoc, "切梁火打ベースに置換するモデル線分", "モデル線分", ref ids))
                    {
                        return;
                    }
                    if (ids.Count < 1)
                    {
                        MessageBox.Show("モデル線分が選択されていません。");
                        return;
                    }
                    foreach (ElementId id in ids)
                    {
                        ModelLine modelLine = doc.GetElement(id) as ModelLine;
                        LocationCurve lCurve = modelLine.Location as LocationCurve;
                        if (lCurve == null)
                        {
                            continue;
                        }
                        Curve cv = lCurve.Curve;
                        clsKHB.CreateKiribariHiuchiBase(doc, cv.GetEndPoint(0), cv.GetEndPoint(1), levelID, dan);
                    }
                    //置換の処理をここに記述
                    //MessageBox.Show("置換の処理は現在実装中です。");
                }
            }
            else//三軸
            {
                for (; ; )
                {
                    try
                    {
                        ElementId idSanjiku = null;
                        if (!ClsRevitUtil.PickObjectFilters(uidoc, "三軸ピースを選択してください", ClsGlobal.m_sanjikuPeaceList, ref idSanjiku))
                        {
                            return;
                        }
                        Element selSanjiku = doc.GetElement(idSanjiku);

                        //選択された三軸ピース
                        FamilyInstance inst1 = selSanjiku as FamilyInstance;

                        XYZ picPoint = (inst1.Location as LocationPoint).Point;
                        XYZ dirSanjiku = inst1.FacingOrientation;//直進の向き
                        XYZ facOriKiri = inst1.HandOrientation;
                        double distSanjiku = ClsSanjikuPeace.GetSanjikuDistanceToCenter(doc, idSanjiku);
                        //三軸ピースの点を中心に移動
                        picPoint = new XYZ(picPoint.X + (ClsRevitUtil.CovertToAPI(distSanjiku) * dirSanjiku.X),
                                               picPoint.Y + (ClsRevitUtil.CovertToAPI(distSanjiku) * dirSanjiku.Y),
                                               picPoint.Z);
                        ElementId idKiri = null;

                        //図面上の切梁ベースを全て取得
                        List<ElementId> baseIdList = ClsKiribariBase.GetAllKiribariBaseList(doc);
                        //三軸ピースと交点のある切梁ベースを探す//段を取得するため
                        foreach (ElementId baseId in baseIdList)
                        {
                            string danKiri = ClsRevitUtil.GetParameter(doc, baseId, "段");
                            string size = ClsRevitUtil.GetParameter(doc, baseId, "鋼材サイズ");
                            double kouzaiSize = ClsRevitUtil.CovertToAPI(Master.ClsYamadomeCsv.GetWidth(size) / 2);
                            FamilyInstance shinInst = doc.GetElement(baseId) as FamilyInstance;
                            LocationCurve lCurveKiri = shinInst.Location as LocationCurve;
                            if (lCurveKiri != null)
                            {
                                XYZ tmpStPoint = lCurveKiri.Curve.GetEndPoint(0);
                                XYZ tmpEdPoint = lCurveKiri.Curve.GetEndPoint(1);
                                //選択された芯の段によって調整
                                if (danKiri == "上段")
                                {
                                    tmpStPoint = new XYZ(tmpStPoint.X, tmpStPoint.Y, tmpStPoint.Z + kouzaiSize);
                                    tmpEdPoint = new XYZ(tmpEdPoint.X, tmpEdPoint.Y, tmpEdPoint.Z + kouzaiSize);
                                }
                                else if (danKiri == "下段")
                                {
                                    tmpStPoint = new XYZ(tmpStPoint.X, tmpStPoint.Y, tmpStPoint.Z - kouzaiSize);
                                    tmpEdPoint = new XYZ(tmpEdPoint.X, tmpEdPoint.Y, tmpEdPoint.Z - kouzaiSize);
                                }
                                Curve cvBaseKiri = Line.CreateBound(tmpStPoint, tmpEdPoint);
                                if (ClsRevitUtil.IsPointOnCurve(cvBaseKiri, picPoint))
                                {
                                    idKiri = baseId;
                                    if (danKiri == "上段")
                                    {
                                        picPoint = new XYZ(picPoint.X, picPoint.Y, picPoint.Z - kouzaiSize);
                                    }
                                    else if (danKiri == "下段")
                                    {
                                        picPoint = new XYZ(picPoint.X, picPoint.Y, picPoint.Z + kouzaiSize);
                                    }
                                    break;
                                }
                            }
                        }
                        if (idKiri == null) return;

                        //腹起ベース指定
                        Reference rfHara = null;
                        if (!ClsHaraokoshiBase.PickBaseObject(uidoc, ref rfHara))
                        {
                            return;
                        }
                        ElementId idHara = rfHara.ElementId;

                        if (!clsKHB.CheckDan(doc, idKiri, idHara)) return;

                        FamilyInstance instKiri = doc.GetElement(idKiri) as FamilyInstance;
                        Curve cvKiri = (instKiri.Location as LocationCurve).Curve;
                        FamilyInstance instHara = doc.GetElement(idHara) as FamilyInstance;
                        Curve cvHara = (instHara.Location as LocationCurve).Curve;
                        XYZ dirHara = Line.CreateBound(cvHara.GetEndPoint(0), cvHara.GetEndPoint(1)).Direction;

                        dan = ClsRevitUtil.GetParameter(doc, idKiri, "段");
                        levelID = instKiri.Host.Id;

                        //交点がない場合があるため無限に伸ばす
                        var cvInfiKiri = ClsKiribariHiuchiBase.CreateInfiniteCurve(cvKiri);
                        var cvInfiHara = ClsKiribariHiuchiBase.CreateInfiniteCurve(cvHara);
                        XYZ insec = ClsRevitUtil.GetIntersection(cvInfiKiri, cvInfiHara);
                        if (insec == null) return;

                        double crossDist = insec.DistanceTo(picPoint);
                        double angle = dirSanjiku.AngleTo(dirHara);
                        XYZ dirPic = Line.CreateBound(insec, rfHara.GlobalPoint).Direction;
                        double shitenkankyori;

                        if (clsKHB.m_CreateType == ClsKiribariHiuchiBase.CreateType.Both)
                        {
                            shitenkankyori = crossDist / Math.Sin(Math.PI - Math.PI / 6 - angle) * Math.Sin(Math.PI / 6);//右
                            XYZ newPoint = ClsRevitUtil.GetDistanceNewPoint(insec, dirHara, ClsRevitUtil.CovertFromAPI(shitenkankyori));

                            clsKHB.CreateKiribariHiuchiBase(doc, picPoint, newPoint, levelID, dan);

                            shitenkankyori = crossDist / Math.Sin(Math.PI - Math.PI / 6 - (Math.PI - angle)) * Math.Sin(Math.PI / 6);//左
                            newPoint = ClsRevitUtil.GetDistanceNewPoint(insec, -dirHara, ClsRevitUtil.CovertFromAPI(shitenkankyori));

                            clsKHB.CreateKiribariHiuchiBase(doc, picPoint, newPoint, levelID, dan);
                        }
                        else if (clsKHB.m_CreateType == ClsKiribariHiuchiBase.CreateType.AnyLengthManual)
                        {
                            if (ClsGeo.IsLeft(dirSanjiku, dirPic))
                            {
                                shitenkankyori = crossDist / Math.Sin(Math.PI - Math.PI / 6 - (Math.PI - angle)) * Math.Sin(Math.PI / 6);//左
                                dirHara = -dirHara;
                            }
                            else
                            {
                                shitenkankyori = crossDist / Math.Sin(Math.PI - Math.PI / 6 - angle) * Math.Sin(Math.PI / 6);//右
                            }
                            XYZ newPoint = ClsRevitUtil.GetDistanceNewPoint(insec, dirHara, ClsRevitUtil.CovertFromAPI(shitenkankyori));

                            clsKHB.CreateKiribariHiuchiBase(doc, picPoint, newPoint, levelID, dan);
                        }
                        else//自動不明
                        {
                            //切梁火打ベース指定
                            ElementId idKiriHiuch = null;
                            if (!ClsKiribariHiuchiBase.PickBaseObject(uidoc, ref idKiriHiuch))
                            {
                                return;
                            }
                            FamilyInstance instKiriHiuchi = doc.GetElement(idKiriHiuch) as FamilyInstance;
                            Curve cvKiriHiuch = (instKiriHiuchi.Location as LocationCurve).Curve;

                            //通常作成
                            Line kiriBLine = Line.CreateBound(insec, cvKiriHiuch.GetEndPoint(1));
                            XYZ dirKiriHiuch = kiriBLine.Direction;
                            XYZ newStPoint = cvKiriHiuch.GetEndPoint(0);
                            XYZ newEndPoint = ClsRevitUtil.GetDistanceNewPoint(insec, -dirKiriHiuch, ClsRevitUtil.CovertFromAPI(kiriBLine.Length));

                            clsKHB.CreateKiribariHiuchiBase(doc, newStPoint, newEndPoint, levelID, dan);
                        }
                    }
                    catch
                    {
                        break;
                    }
                }

            }
            //切梁火打ベースを作図
            //if (!clsKHB.CreateKiribariHiuchiBase())
            //{
            //    //作図に失敗した時のコメント
            //}

            return;
        }

        public static bool CommandChangeKiribariHiuchiBase(UIDocument uidoc)
        {
            Document doc = uidoc.Document;

            // 対象となるベースを複数選択
            List<ElementId> idList = null;
            if (!ClsKiribariHiuchiBase.PickBaseObjects(uidoc, ref idList))
            {
                return false;
            }

            ClsKiribariHiuchiBase templateBase = new ClsKiribariHiuchiBase();
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
                    templateBase.m_CreateHoho = ClsRevitUtil.CompareValues(ClsRevitUtil.GetParameter(doc, id, "構成") == "シングル" ? ClsKiribariHiuchiBase.CreateHoho.Single : ClsKiribariHiuchiBase.CreateHoho.Double, templateBase.m_CreateHoho);

                    templateBase.m_SteelType = ClsRevitUtil.CompareValues(ClsRevitUtil.GetParameter(doc, id, "鋼材タイプ"), templateBase.m_SteelType);
                    templateBase.m_SteelSizeSingle = ClsRevitUtil.CompareValues(ClsRevitUtil.GetParameter(doc, id, "鋼材サイズ(シングル)"), templateBase.m_SteelSizeSingle);
                    templateBase.m_SteelSizeDoubleUpper = ClsRevitUtil.CompareValues(ClsRevitUtil.GetParameter(doc, id, "鋼材サイズ(ダブル上)"), templateBase.m_SteelSizeDoubleUpper);
                    templateBase.m_SteelSizeDoubleUnder = ClsRevitUtil.CompareValues(ClsRevitUtil.GetParameter(doc, id, "鋼材サイズ(ダブル下)"), templateBase.m_SteelSizeDoubleUnder);
                    templateBase.m_HiuchiZureRyo = ClsRevitUtil.CompareValues((int)ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetParameterDouble(doc, id, "上下火打ずれ量a")), templateBase.m_HiuchiZureRyo);

                    templateBase.m_PartsTypeKiriSide = ClsRevitUtil.CompareValues(ClsRevitUtil.GetParameter(doc, id, "部品タイプ(切梁側)"), templateBase.m_PartsTypeKiriSide);
                    templateBase.m_PartsSizeKiriSide = ClsRevitUtil.CompareValues(ClsRevitUtil.GetParameter(doc, id, "部品サイズ(切梁側)"), templateBase.m_PartsSizeKiriSide);
                    templateBase.m_PartsTypeHaraSide = ClsRevitUtil.CompareValues(ClsRevitUtil.GetParameter(doc, id, "部品タイプ(腹起側)"), templateBase.m_PartsTypeHaraSide);
                    templateBase.m_PartsSizeHaraSide = ClsRevitUtil.CompareValues(ClsRevitUtil.GetParameter(doc, id, "部品サイズ(腹起側)"), templateBase.m_PartsSizeHaraSide);
                }
            }

            //ダイアログを表示
            DLG.DlgCreateKiribariHiuchiBase kiribariHiuchiBase = new DLG.DlgCreateKiribariHiuchiBase(templateBase);
            DialogResult result = kiribariHiuchiBase.ShowDialog();
            if (result != DialogResult.OK)
            {
                return false;
            }

            ClsKiribariHiuchiBase clsKiribariHiuchiBase = kiribariHiuchiBase.m_KiribariHiuchiBase;

            foreach (var id in idList)
            {
                FamilyInstance shinInstLevel = doc.GetElement(id) as FamilyInstance;
                LocationCurve lCurve = shinInstLevel.Location as LocationCurve;
                if (lCurve == null)
                {
                    continue;
                }
                XYZ tmpStPoint = lCurve.Curve.GetEndPoint(0);
                XYZ tmpEdPoint = lCurve.Curve.GetEndPoint(1);

                //元データを取得
                ElementId levelID = shinInstLevel.Host.Id;
                string dan = ClsRevitUtil.GetParameter(doc, id, "段");

                //切梁-切梁or切梁-腹起のid
                (ElementId id1, ElementId id2) = clsKiribariHiuchiBase.GetInsecHaraokoshi_KiribariBase(doc, id);

                if (id1 == null || id2 == null)
                {
                    continue;
                }

                XYZ pntS = new XYZ();
                XYZ pntE = new XYZ();
                XYZ insec = new XYZ();

                FamilyInstance inst1 = doc.GetElement(id1) as FamilyInstance;
                LocationCurve lCurve1 = inst1.Location as LocationCurve;
                //選択した切梁火打の始点と取得したベースが同一線上にあるかの確認
                if (!ClsRevitUtil.IsPointOnCurve(lCurve1.Curve, tmpStPoint))
                {
                    ElementId c = id1;
                    id1 = id2;
                    id2 = c;
                }

                if (clsKiribariHiuchiBase.GetBaseStartEndPoint(doc, id1, id2, ref pntS, ref pntE, ref insec))
                {
                    XYZ newPoint = pntS;
                    XYZ newEndPoint = pntE;
                    if (!ClsRevitUtil.JudgeOnLine(insec, pntS, tmpStPoint))
                    {
                        Line kiriLine = Line.CreateBound(insec, pntS);
                        XYZ dirKiriLine = kiriLine.Direction;
                        newPoint = ClsRevitUtil.GetDistanceNewPoint(insec, -dirKiriLine, ClsRevitUtil.CovertFromAPI(kiriLine.Length));
                    }

                    if (!ClsRevitUtil.JudgeOnLine(insec, pntE, tmpEdPoint))
                    {
                        Line kiriLine = Line.CreateBound(insec, pntE);
                        XYZ dirKiriLine = kiriLine.Direction;
                        newEndPoint = ClsRevitUtil.GetDistanceNewPoint(insec, -dirKiriLine, ClsRevitUtil.CovertFromAPI(kiriLine.Length));
                    }

                    //元のベースを削除
                    using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                    {
                        t.Start();
                        ClsRevitUtil.Delete(doc, id);
                        t.Commit();
                    }
                    clsKiribariHiuchiBase.CreateKiribariHiuchiBase(doc, newPoint, newEndPoint, levelID, dan);
                }
            }

            return true;
        }
    }
}
