using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMS.Parts
{
    public class ClsBaseList
    {
        public static System.Drawing.Point m_LastLocation { get; set; }

        public static bool m_Close { get; set; }

        public static string m_LastTabName { get; set; }
        /// <summary>
        /// ベースダイアログを閉じる
        /// </summary>
        public  static void CloseDlg(UIDocument uidoc, DLG.DlgBaseList dlgBaseList)
        {
            Document doc = uidoc.Document;

            BackOriginalColor(doc, dlgBaseList);

            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                t.Start();
                ClsRevitUtil.Delete(doc, dlgBaseList.m_NumberList);
                t.Commit();
            }
            m_LastLocation = dlgBaseList.Location;
            dlgBaseList.Close();
        }

        /// <summary>
        /// ベース一覧で選択されている行のベースの色を変更する
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="dlgBaseList"></param>
        public static void HighLightBase(UIDocument uidoc, DLG.DlgBaseList dlgBaseList)
        {
            Document doc = uidoc.Document; 
            
            BackOriginalColor(doc, dlgBaseList);

            ElementId id = dlgBaseList.m_HighLightId;
            if (id == null) return;

            //ハイライトにするベースと図面上で選択しているベースが違う場合、選択を解除
            if(ClsRevitUtil.GetSelectElement(uidoc) != id) ClsRevitUtil.UnSelectElement(uidoc);

            dlgBaseList.m_originalSettingsId = id;
            dlgBaseList.m_originalSettings = doc.ActiveView.GetElementOverrides(id);

            using (Transaction tx = new Transaction(doc, "Change Line Color"))
            {
                //#31454
                byte r = ClsZumenInfo.GetBaseListSelectedbaseColorR();
                byte g = ClsZumenInfo.GetBaseListSelectedbaseColorG();
                byte b = ClsZumenInfo.GetBaseListSelectedbaseColorB();

                tx.Start();
                ClsRevitUtil.ChangeLineColor(doc, id, new Color(r,g,b));
                tx.Commit();
            }

            
        }

        /// <summary>
        /// 編集されたベースを変更する
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static bool ChangeBase(UIDocument uidoc, DLG.DlgBaseList dlgBaseList)
        {
            Document doc = uidoc.Document;
            ElementId id;
            //腹起ベース
            List<ClsHaraokoshiBase> haraList = dlgBaseList.ListHaraokoshiBase;
            foreach (ClsHaraokoshiBase clsHaraBase in haraList)
            {
                if (clsHaraBase.m_FlgEdit)
                {
                    id = clsHaraBase.m_ElementId;
                    FamilyInstance shinInstLevel = doc.GetElement(id) as FamilyInstance;
                    ElementId levelID = ClsRevitUtil.GetLevelID(doc, clsHaraBase.m_level);
                    if (levelID != shinInstLevel.Host.Id)
                    {
                        LocationCurve lCurve = shinInstLevel.Location as LocationCurve;
                        if (lCurve == null)
                        {
                            continue;
                        }
                        XYZ tmpStPoint = lCurve.Curve.GetEndPoint(0);
                        XYZ tmpEdPoint = lCurve.Curve.GetEndPoint(1);
                        //元のベースを削除
                        using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                        {
                            t.Start();
                            ClsRevitUtil.Delete(doc, id);
                            t.Commit();
                        }

                        //ベース作成
                        clsHaraBase.CreateHaraokoshiBase(doc, tmpStPoint, tmpEdPoint, 0);
                    }
                    else
                    {
                        //変更された腹起ベースがシングル⇔ダブルで変更されていた場合に色を戻さない
                        var doubleFlag = false;
                        if (dlgBaseList.m_originalSettingsId == id)
                        {
                            var yoko = ClsRevitUtil.GetParameter(doc, id, "横本数") == "シングル" ? ClsHaraokoshiBase.SideNum.Single : ClsHaraokoshiBase.SideNum.Double;
                            var tate = ClsRevitUtil.GetParameter(doc, id, "縦本数") == "シングル" ? ClsHaraokoshiBase.VerticalNum.Single : ClsHaraokoshiBase.VerticalNum.Double;
                            if(yoko != clsHaraBase.m_yoko || tate != clsHaraBase.m_tate)
                            {
                                doubleFlag = true;
                            }
                        }
                        clsHaraBase.ChangeHaraokoshiBase(doc, id);
                        clsHaraBase.ChangeInterSectionHaraokoshiBase(doc, id);

                        if(doubleFlag)
                        {
                            //シングル⇔ダブルで変更されているため変更された色を取得する
                            dlgBaseList.m_originalSettings = doc.ActiveView.GetElementOverrides(id);
                        }
                    }
                }
            }
            //切梁ベース
            List<ClsKiribariBase> kiriList = dlgBaseList.ListKiribariBase;
            foreach (ClsKiribariBase clskiriBase in kiriList)
            {
                if (clskiriBase.m_FlgEdit)
                {
                    id = clskiriBase.m_ElementId;
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
                    string beforeSize = ClsRevitUtil.GetParameter(doc, id, "鋼材サイズ");
                    ////元のベースを削除
                    //using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                    //{
                    //    t.Start();
                    //    ClsRevitUtil.Delete(doc, id);
                    //    t.Commit();
                    //}

                    //切梁ベース変更
                    clskiriBase.ChangeKiribariBase(doc, id, dan, levelID);
                    clskiriBase.ChangeInterSectionKiribariBaseWithKiribariHiuchBase(doc, id, beforeSize);
                }
            }
            //隅火打ベース
            List<ClsCornerHiuchiBase> chbList = dlgBaseList.ListCornerHiuchiBase;
            foreach (ClsCornerHiuchiBase clsCHB in chbList)
            {
                if (clsCHB.m_FlgEdit)
                {
                    id = clsCHB.m_cornerHiuchiID;

                    FamilyInstance shinInstLevel = doc.GetElement(id) as FamilyInstance;
                    ElementId levelID = shinInstLevel.Host.Id;
                    LocationCurve lCurve = shinInstLevel.Location as LocationCurve;
                    if (lCurve == null)
                    {
                        continue;
                    }
                    XYZ tmpStPoint = lCurve.Curve.GetEndPoint(0);
                    List<ElementId> haraIdList = ClsHaraokoshiBase.GetAllHaraokoshiBaseList(doc);
                    foreach (ElementId haraId in haraIdList)
                    {
                        FamilyInstance haraInst = doc.GetElement(haraId) as FamilyInstance;
                        LocationCurve lCurveHara = haraInst.Location as LocationCurve;
                        if (lCurveHara != null)
                        {
                            XYZ insec = ClsRevitUtil.GetIntersection(lCurve.Curve, lCurveHara.Curve);//交点が2つ見つかる
                            if (insec != null)
                            {
                                if (ClsGeo.GEO_EQ(insec, tmpStPoint))
                                {
                                    clsCHB.m_HaraokoshiBaseID1 = haraId;
                                }
                                else
                                {
                                    clsCHB.m_HaraokoshiBaseID2 = haraId;
                                }
                            }
                        }
                    }
                    //変更する隅火打ベースに接している腹起ベースを2本取得する
                    //(clsCHB.m_HaraokoshiBaseID1, clsCHB.m_HaraokoshiBaseID2) = clsCHB.GetInsecHaraokoshiBase(doc, id);
                    if (clsCHB.m_HaraokoshiBaseID1 == null || clsCHB.m_HaraokoshiBaseID2 == null)
                    {
                        continue;
                    }
                    //FamilyInstance shinInstLevel = doc.GetElement(id) as FamilyInstance;
                    //ElementId levelID = shinInstLevel.Host.Id;
                    string dan = ClsRevitUtil.GetParameter(doc, id, "段");//腹起ベースの変更に伴う段の確認は保留clsCHB.m_HaraokoshiBaseID1
                    //元のベースを削除
                    using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                    {
                        t.Start();
                        ClsRevitUtil.Delete(doc, id);
                        t.Commit();
                    }

                    //ベース作成
                    XYZ pntS = new XYZ();
                    XYZ pntE = new XYZ();
                    if (clsCHB.GetBaseStartEndPoint(doc, ref pntS, ref pntE))
                    {
                        clsCHB.CreateCornerHiuchi(doc, pntS, pntE, levelID, dan);
                    }
                }
            }
            //切梁火打ベース
            List<ClsKiribariHiuchiBase> khbList = dlgBaseList.ListKiribariHiuchiBase;
            foreach (ClsKiribariHiuchiBase clsKHB in khbList)
            {
                if (clsKHB.m_FlgEdit)
                {
                    id = clsKHB.m_ElementId;
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
                    (ElementId id1, ElementId id2) = clsKHB.GetInsecHaraokoshi_KiribariBase(doc, id);

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

                    if (clsKHB.GetBaseStartEndPoint(doc, id1, id2, ref pntS, ref pntE, ref insec))
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
                        clsKHB.CreateKiribariHiuchiBase(doc, newPoint, newEndPoint, levelID, dan);
                    }
                }
            }
            //切梁受けベース
            List<ClsKiribariUkeBase> kubList = dlgBaseList.ListKiribariUkeBase;
            foreach (ClsKiribariUkeBase clsKUB in kubList)
            {
                if (clsKUB.m_FlgEdit)
                {
                    id = clsKUB.m_ElementId;
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

                    //元のベースを削除
                    using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                    {
                        t.Start();
                        ClsRevitUtil.Delete(doc, id);
                        t.Commit();
                    }
                    //ベース変更
                    clsKUB.ChangeKiribariUkeBase(doc, tmpStPoint, tmpEdPoint, dan, levelID);
                }
            }
            //切梁繋ぎ材ベース
            List<ClsKiribariTsunagizaiBase> ktbList = dlgBaseList.ListKiribariTsunagizaiBase;
            foreach (ClsKiribariTsunagizaiBase clsKTB in ktbList)
            {
                if (clsKTB.m_FlgEdit)
                {
                    id = clsKTB.m_ElementId;
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

                    //分割がある場合の処理
                    (List<XYZ> stList, List<XYZ> edList) = clsKTB.ChangeSplitKiribariTsunagizaiBase(doc, id);

                    //元のベースを削除
                    using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                    {
                        t.Start();
                        ClsRevitUtil.Delete(doc, id);
                        t.Commit();
                    }
                    //ベース変更
                    for (int i = 0; i < stList.Count; i++)
                    {
                        tmpStPoint = stList[i];
                        tmpEdPoint = edList[i];
                        clsKTB.ChangeKiribariTsunagizaiBase(doc, tmpStPoint, tmpEdPoint, dan, levelID);
                    }
                }
            }
            //火打繋ぎ材ベース
            List<ClsHiuchiTsunagizaiBase> htbList = dlgBaseList.ListHiuchiTsunagizaiBase;
            foreach (ClsHiuchiTsunagizaiBase clsHTB in htbList)
            {
                if (clsHTB.m_FlgEdit)
                {
                    id = clsHTB.m_ElementId;
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

                    //分割がある場合の処理
                    (List<XYZ> stList, List<XYZ> edList) = clsHTB.ChangeSplitHiuchiTsunagizaiBase(doc, id);

                    //元のベースを削除
                    using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                    {
                        t.Start();
                        ClsRevitUtil.Delete(doc, id);
                        t.Commit();
                    }
                    //ベース変更
                    for (int i = 0; i < stList.Count; i++)
                    {
                        tmpStPoint = stList[i];
                        tmpEdPoint = edList[i];
                        clsHTB.ChangeHiuchiTsunagizaiBase(doc, tmpStPoint, tmpEdPoint, dan, levelID);
                    }
                }
            }
            //切梁継ぎベース
            List<ClsKiribariTsugiBase> ktsbList = dlgBaseList.ListKiribariTsugiBase;
            foreach (ClsKiribariTsugiBase clsKTB in ktsbList)
            {
                if (clsKTB.m_FlgEdit)
                {
                    id = clsKTB.m_ElementId;
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

                    //元のベースを削除
                    using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                    {
                        t.Start();
                        ClsRevitUtil.Delete(doc, id);
                        t.Commit();
                    }
                    //ベース変更
                    clsKTB.CreateKiribariTsugiBase(doc, tmpStPoint, tmpEdPoint, dan, levelID);
                }
            }
            //斜梁ベース
            var sList = dlgBaseList.ListSyabariBase;
            foreach (var cls in sList)
            {
                if (cls.m_FlgEdit)
                {
                    id = cls.m_ElementId;
                    using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                    {
                        t.Start();
                        cls.SetParameter(doc, id);

                        t.Commit();
                    }
                }
            }
            //斜梁繋ぎ材ベース
            var sTList = dlgBaseList.ListSyabariTsunagizaiBase;
            foreach (var cls in sTList)
            {
                if (cls.m_FlgEdit)
                {
                    id = cls.m_ElementId;
                    using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                    {
                        t.Start();
                        cls.SetParameter(doc, id);
                        t.Commit();
                    }
                }
            }
            //斜梁受け材ベース
            var sUList = dlgBaseList.ListSyabariUkeBase;
            foreach (var cls in sUList)
            {
                if (cls.m_FlgEdit)
                {
                    id = cls.m_ElementId;
                    using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                    {
                        t.Start();
                        cls.SetParameter(doc, id);

                        t.Commit();
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// タブが変更されたときに各ベースに文字列を作成する
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="dlgBaseList"></param>
        public static void CreateNumber(UIDocument uidoc, DLG.DlgBaseList dlgBaseList)
        {
            Document doc = uidoc.Document;
            ElementId id;
            ElementId noteId;
            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                t.Start();
                ClsRevitUtil.Delete(doc, dlgBaseList.m_NumberList);
                t.Commit();
            }
            dlgBaseList.m_NumberList = new List<ElementId>();

            BackOriginalColor(doc, dlgBaseList);

            switch (dlgBaseList.m_TabName)
            {
                case "腹起":
                    {
                        List<ClsHaraokoshiBase> haraList = dlgBaseList.ListHaraokoshiBase;
                        for (int i = 0; i < haraList.Count; i++)
                        {
                            id = haraList[i].m_ElementId;
                            noteId = CreateNumber(doc, id, (i + 1).ToString(), dlgBaseList.m_NumberList);
                            dlgBaseList.m_NumberList.Add(noteId);
                        }
                        break;
                    }
                case "切梁":
                    {
                        List<ClsKiribariBase> kiriList = dlgBaseList.ListKiribariBase;
                        for (int i = 0; i < kiriList.Count; i++)
                        {
                            id = kiriList[i].m_ElementId;
                            noteId = CreateNumber(doc, id, (i + 1).ToString(), dlgBaseList.m_NumberList);
                            dlgBaseList.m_NumberList.Add(noteId);
                        }
                        break;
                    }
                case "隅火打":
                    {
                        List<ClsCornerHiuchiBase> chbList = dlgBaseList.ListCornerHiuchiBase;
                        for (int i = 0; i < chbList.Count; i++)
                        {
                            id = chbList[i].m_cornerHiuchiID;
                            noteId = CreateNumber(doc, id, (i + 1).ToString(), dlgBaseList.m_NumberList);
                            dlgBaseList.m_NumberList.Add(noteId);
                        }
                        break;
                    }
                case "切梁火打":
                    {
                        List<ClsKiribariHiuchiBase> khbList = dlgBaseList.ListKiribariHiuchiBase;
                        for (int i = 0; i < khbList.Count; i++)
                        {
                            id = khbList[i].m_ElementId;
                            noteId = CreateNumber(doc, id, (i + 1).ToString(), dlgBaseList.m_NumberList);
                            dlgBaseList.m_NumberList.Add(noteId);
                        }
                        break;
                    }
                case "切梁受け":
                    {
                        List<ClsKiribariUkeBase> kubList = dlgBaseList.ListKiribariUkeBase;
                        for (int i = 0; i < kubList.Count; i++)
                        {
                            id = kubList[i].m_ElementId;
                            noteId = CreateNumber(doc, id, (i + 1).ToString(), dlgBaseList.m_NumberList);
                            dlgBaseList.m_NumberList.Add(noteId);
                        }
                        break;
                    }
                case "切梁繋ぎ材":
                    {
                        List<ClsKiribariTsunagizaiBase> ktbList = dlgBaseList.ListKiribariTsunagizaiBase;
                        for (int i = 0; i < ktbList.Count; i++)
                        {
                            id = ktbList[i].m_ElementId;
                            noteId = CreateNumber(doc, id, (i + 1).ToString(), dlgBaseList.m_NumberList);
                            dlgBaseList.m_NumberList.Add(noteId);
                        }
                        break;
                    }
                case "火打繋ぎ材":
                    {
                        List<ClsHiuchiTsunagizaiBase> htbList = dlgBaseList.ListHiuchiTsunagizaiBase;
                        for (int i = 0; i < htbList.Count; i++)
                        {
                            id = htbList[i].m_ElementId;
                            noteId = CreateNumber(doc, id, (i + 1).ToString(), dlgBaseList.m_NumberList);
                            dlgBaseList.m_NumberList.Add(noteId);
                        }
                        break;
                    }
                case "切梁継ぎ":
                    {
                        List<ClsKiribariTsugiBase> ktsbList = dlgBaseList.ListKiribariTsugiBase;
                        for (int i = 0; i < ktsbList.Count; i++)
                        {
                            id = ktsbList[i].m_ElementId;
                            noteId = CreateNumber(doc, id, (i + 1).ToString(), dlgBaseList.m_NumberList);
                            dlgBaseList.m_NumberList.Add(noteId);
                        }
                        break;
                    }
                case "斜梁":
                    {
                        var lst = dlgBaseList.ListSyabariBase;
                        for (int i = 0; i < lst.Count; i++)
                        {
                            id = lst[i].m_ElementId;
                            noteId = CreateNumber(doc, id, (i + 1).ToString(), dlgBaseList.m_NumberList);
                            dlgBaseList.m_NumberList.Add(noteId);
                        }
                        break;
                    }
                case "斜梁繋ぎ材":
                    {
                        var lst = dlgBaseList.ListSyabariTsunagizaiBase;
                        for (int i = 0; i < lst.Count; i++)
                        {
                            id = lst[i].m_ElementId;
                            noteId = CreateNumber(doc, id, (i + 1).ToString(), dlgBaseList.m_NumberList);
                            dlgBaseList.m_NumberList.Add(noteId);
                        }
                        break;
                    }
                case "斜梁受け材":
                    {
                        var lst = dlgBaseList.ListSyabariUkeBase;
                        for (int i = 0; i < lst.Count; i++)
                        {
                            id = lst[i].m_ElementId;
                            noteId = CreateNumber(doc, id, (i + 1).ToString(), dlgBaseList.m_NumberList);
                            dlgBaseList.m_NumberList.Add(noteId);
                        }
                        break;
                    }
            }

            //タブを切り替えたタイミングで文字を作成するので同時にDGVで選択されている行をHL
            HighLightBase(uidoc, dlgBaseList);
        }

        /// <summary>
        /// ベースにNo文字を作成する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id">ベース</param>
        /// <param name="num">No文字</param>
        public static ElementId CreateNumber(Document doc, ElementId id, string num, List<ElementId> numberList)
        {
            using (Transaction CreateBaseNumber = new Transaction(doc, "Create Base Number"))
            {
                try
                {
                    FamilyInstance shinInstLevel = doc.GetElement(id) as FamilyInstance;

                    Curve cv;
                    if (shinInstLevel.Location is LocationCurve lCurve)
                    {
                        cv = lCurve.Curve;
                    }
                    else
                    {
                        cv = ClsYMSUtil.GetBaseLine(doc, id);
                    }
                    XYZ tmpStPoint = cv.GetEndPoint(0);
                    XYZ tmpEdPoint = cv.GetEndPoint(1);

                    CreateBaseNumber.Start();

                    TextNoteOptions options = new TextNoteOptions();
                    options.KeepRotatedTextReadable = false;
                    options.TypeId = doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);

                    XYZ tmpCtPoint = (tmpStPoint + tmpEdPoint) / 2;

                    XYZ dir = Line.CreateBound(tmpStPoint, tmpEdPoint).Direction;
                    tmpCtPoint = ShiftNumber(doc, dir, numberList, tmpCtPoint);
                    View activeView = doc.ActiveView;
                    TextNote note = TextNote.Create(doc, activeView.Id, tmpCtPoint, num, options);

                    //TextElementType textType = note.Symbol;
                    //BuiltInParameter paraIndex = BuiltInParameter.TEXT_SIZE;
                    //Parameter textSize = textType.get_Parameter(paraIndex);

                    //textSize.Set(ClsRevitUtil.CovertToAPI(7));

                    CreateBaseNumber.Commit();

                    return note.Id;
                }
                catch (Exception ex)
                {
                    CreateBaseNumber.RollBack();
                    //MessageBox.Show(ex.Message);
                }
                return null;
            }
        }

        /// <summary>
        /// 色が変更されているベースを元の色に戻す
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="dlgBaseList"></param>
        public static void BackOriginalColor(Document doc, DLG.DlgBaseList dlgBaseList)
        {
            if (dlgBaseList.m_originalSettingsId != null && dlgBaseList.m_originalSettings != null)
            {
                ClsRevitUtil.SetElementOriginalColor(doc, dlgBaseList.m_originalSettingsId, dlgBaseList.m_originalSettings);
            }
        }

        /// <summary>
        /// 図面上のベースを全て取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<ElementId> GetAllBaseList(Document doc)
        {
            //図面上のベースを全て取得
            List<ElementId> baseIdList = ClsRevitUtil.GetSelectCreatedFamilyInstanceList(doc, "ベース", true);
            return baseIdList;
        }

        public static XYZ ShiftNumber(Document doc, XYZ dir, List<ElementId> numberList, ElementId targetNumberId)
        {
            //対象の文字
            double dist = ClsRevitUtil.CovertToAPI(500);
            Element targetNumber = doc.GetElement(targetNumberId);
            TextElement txtTargetNumber = targetNumber as TextElement;
            XYZ pointTargetNumber = txtTargetNumber.Coord;

            //図面に既に存在している文字List
            foreach (ElementId numberId in numberList)
            {
                Element number = doc.GetElement(numberId);
                TextElement txtNumber = number as TextElement;
                XYZ pointNumber = txtNumber.Coord;
                XYZ moveHalfPointNumber = new XYZ(pointNumber.X - dist / 2 * dir.X,
                                                        pointNumber.Y - dist / 2 * dir.Y,
                                                        pointNumber.Z);
                List<XYZ> vertexList = ClsRevitUtil.GetQuadrilateralForOnePoint(moveHalfPointNumber, dir, dist, 4);
                if (ClsRevitUtil.IsInArea(vertexList, pointTargetNumber, XYZ.BasisZ))
                {
                    //using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                    //{
                    //    try
                    //    {
                    //        t.Start();
                    //        double d = ClsRevitUtil.CovertToAPI(5);
                    //        XYZ movePoint = new XYZ(pointTargetNumber.X + dist * dir.X,
                    //                                    pointTargetNumber.Y + dist * dir.Y,
                    //                                    pointTargetNumber.Z);
                    //        targetNumber.Location.Move(movePoint);
                    //        t.Commit();

                    //        //pointTargetNumber = movePoint;
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        t.RollBack();
                    //    }
                    //}
                    //break;
                    pointTargetNumber = new XYZ(pointTargetNumber.X + dist * dir.X,
                                                        pointTargetNumber.Y + dist * dir.Y,
                                                        pointTargetNumber.Z);
                }
            }

            return pointTargetNumber;
        }
        public static XYZ ShiftNumber(Document doc, XYZ dir, List<ElementId> numberList, XYZ pointTargetNumber)
        {
            //対象の文字
            double dist = ClsRevitUtil.CovertToAPI(500);

            //図面に既に存在している文字List
            foreach (ElementId numberId in numberList)
            {
                Element number = doc.GetElement(numberId);
                TextElement txtNumber = number as TextElement;
                XYZ pointNumber = txtNumber.Coord;
                XYZ moveHalfPointNumber = new XYZ(pointNumber.X - dist / 2 * dir.X,
                                                        pointNumber.Y - dist / 2 * dir.Y,
                                                        pointNumber.Z);
                List<XYZ> vertexList = ClsRevitUtil.GetQuadrilateralForOnePoint(moveHalfPointNumber, dir, dist, 4);
                if (ClsRevitUtil.IsInArea(vertexList, pointTargetNumber, XYZ.BasisZ))
                {
                    pointTargetNumber = new XYZ(pointTargetNumber.X + dist * dir.X,
                                                        pointTargetNumber.Y + dist * dir.Y,
                                                        pointTargetNumber.Z);
                }
            }

            return pointTargetNumber;
        }
    }
}
