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
using System.Collections.Generic;
using System.Linq;

namespace YMS.Parts
{
    public enum WaritukeDist : int
    {
        warituke1mm = 1,
        warituke10mm = 10,
        warituke100mm = 100
    }
    public  class ClsWarituke
    {
        public const string WARITUKE = "割付";
        public static ElementId GetConnectionBase(Document doc, ElementId buzaiId)
        {
            ElementId baseId = null;
            baseId = ClsKariKouzai.GetBaseId(doc, buzaiId);
            return baseId;
        }

        public static List<ElementId> GetConnectionBuzai(Document doc, ElementId baseId)
        {
            List<ElementId> insecList = new List<ElementId>();

            List<ElementId> kariIdList = ClsKariKouzai.GetAllIdList(doc);
            foreach (ElementId kariId in kariIdList)
            {
                //仮鋼材に紐図いているベースIdを取得
                ElementId kariBaseId = ClsKariKouzai.GetBaseId(doc, kariId);
                if (kariBaseId == baseId)
                {
                    insecList.Add(kariId);
                }
            }

            return insecList;
        }
        public static List<ElementId> GetConnectionBuzaiDouble(Document doc, ElementId baseId, string dan)
        {
            List<ElementId> insecList = new List<ElementId>();

            List<ElementId> kariIdList = ClsKariKouzai.GetAllIdList(doc);
            foreach (ElementId kariId in kariIdList)
            {
                //仮鋼材に紐図いているベースIdを取得
                ElementId kariBaseId = ClsKariKouzai.GetBaseIdDouble(doc, kariId, dan);
                if (kariBaseId == baseId)
                {
                    insecList.Add(kariId);
                }
            }

            return insecList;
        }
        /// <summary>
        /// 割付ダイアログを閉じる
        /// </summary>
        public static void CloseDlg()
        {
            DLG.DlgWaritsuke dlgWaritsuke = Application.thisApp.GetForm_dlgWaritsuke();
            if (dlgWaritsuke != null && dlgWaritsuke.Visible)
            {
                dlgWaritsuke.Close();
            }
        }
        public static XYZ CreateWaritukeScale(UIDocument uidoc, XYZ tmpStPoint, XYZ tmpEdPoint, WaritukeDist eDist, out double picPointLength, out bool max, double maxLength = 7000.0, double minLength = 1000.0)
        {
            Document doc = uidoc.Document;

            Autodesk.Revit.DB.View vActv = doc.ActiveView;
            int dist = ((int)eDist);
            double convetDist = ClsRevitUtil.CovertToAPI(dist);
            XYZ pickPoint = null;
            //double vSize = 1.0;
            Line baseLine = Line.CreateBound(tmpStPoint, tmpEdPoint);
            XYZ dir = baseLine.Direction;
            double baseLength = ClsRevitUtil.CovertFromAPI(baseLine.Length);
            picPointLength = 0;
            max = false;

            if (baseLength < dist)
                return pickPoint;

            //目盛り上限
            double scaleMax = baseLength;
            if (maxLength < scaleMax)
                scaleMax = maxLength;
            else if (scaleMax < minLength)
                return pickPoint;
            else
                max = true;

            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                Selection selection = uidoc.Selection;
                List<ElementId> scaleIds = new List<ElementId>();

                try
                {
                    // スケールの作成
                    t.Start();
                    for (int i = 0; i <= scaleMax / dist; i++)
                    {
                        XYZ newPoint = new XYZ(tmpStPoint.X + (ClsRevitUtil.CovertToAPI(i * dist) * dir.X),
                                               tmpStPoint.Y + (ClsRevitUtil.CovertToAPI(i * dist) * dir.Y),
                                               tmpStPoint.Z);
                        Curve cv;
                        if (i % 10 == 0)
                        {
                            cv = CreateCurveToPoint(newPoint, dir, convetDist * 5);
                            ElementId defaultTypeId = doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);

                            TextNoteOptions opts = new TextNoteOptions(defaultTypeId);
                            double maxWidth = TextNote.GetMaximumAllowedWidth(doc, defaultTypeId);
                            maxWidth = ClsRevitUtil.CovertToAPI(maxWidth);
                            TextNote tx = TextNote.Create(doc, vActv.Id, new XYZ(newPoint.X - 0.56, newPoint.Y + 0.56, newPoint.Z),
                                                          maxWidth, (i * dist).ToString() + "mm", opts);
                            TextElementType textType = tx.Symbol;
                            BuiltInParameter paraIndex = BuiltInParameter.TEXT_SIZE;
                            Parameter textSize = textType.get_Parameter(paraIndex);

                            textSize.Set(maxWidth / 10);

                            paraIndex = BuiltInParameter.TEXT_BACKGROUND;
                            Parameter textBack = textType.get_Parameter(paraIndex);
                            textBack.Set(1);// 0 = Opaque（不透明） :: 1 = Transparent（透過）
                            scaleIds.Add(tx.Id);
                        }
                        else if (i % 5 == 0)
                        {
                            cv = CreateCurveToPoint(newPoint, dir, convetDist * 2.5);
                        }
                        else
                        {
                            cv = CreateCurveToPoint(newPoint, dir, convetDist);
                        }

                        XYZ mid = 0.5 * (cv.GetEndPoint(0) + cv.GetEndPoint(1));
                        Plane plane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, mid);
                        SketchPlane sketchPlane = SketchPlane.Create(doc, plane);
                        ModelLine modelLine = doc.Create.NewModelCurve(cv as Line, sketchPlane) as ModelLine;
                        scaleIds.Add(modelLine.Id);

                        Line axis = Line.CreateBound(newPoint, newPoint + XYZ.BasisZ);
                        ClsRevitUtil.RotateElement(doc, modelLine.Id, axis, Math.PI / 2);
                    }
                    t.Commit();

                    // 割付位置の選択
                    pickPoint = selection.PickPoint("位置を選択してください");

                    //Z軸の統一
                    pickPoint = new XYZ(pickPoint.X, pickPoint.Y, tmpStPoint.Z);

                    //基準線からどちら周りで90度回転させるかの
                    double harfPI = Math.PI / 2;
                    XYZ picDir = Line.CreateBound(tmpStPoint, pickPoint).Direction;
                    if (ClsGeo.IsLeft(dir, picDir))
                        harfPI = -harfPI;

                    //指定点から基準線への垂線ベクトルの作成
                    picDir = new XYZ(dir.X * Math.Cos(harfPI) - dir.Y * Math.Sin(harfPI), dir.X * Math.Sin(harfPI) + dir.Y * Math.Cos(harfPI), dir.Z);

                    //指定点から無限に伸ばした直線
                    XYZ perpendicPoint = new XYZ(pickPoint.X + (ClsRevitUtil.CovertToAPI(int.MaxValue) * picDir.X),
                                               pickPoint.Y + (ClsRevitUtil.CovertToAPI(int.MaxValue) * picDir.Y),
                                               pickPoint.Z);
                    Curve perpendicCv = Line.CreateBound(pickPoint, perpendicPoint);

                    //始点から無限に伸ばした直線
                    XYZ extStPoint = new XYZ(tmpStPoint.X + (ClsRevitUtil.CovertToAPI(int.MaxValue) * dir.X),
                                               tmpStPoint.Y + (ClsRevitUtil.CovertToAPI(int.MaxValue) * dir.Y),
                                               tmpStPoint.Z);
                    Curve baseCV = Line.CreateBound(tmpStPoint, extStPoint);

                    //指定点から基準線に下ろした垂線との交点を取得
                    XYZ insecPoint = ClsRevitUtil.GetIntersection(baseCV, perpendicCv);

                    double nearPointLength = minLength;
                    if (insecPoint != null)
                    {
                        Line putLength = Line.CreateBound(tmpStPoint, insecPoint);
                        nearPointLength = ClsRevitUtil.CovertFromAPI(putLength.Length);
                    }

                    if (scaleMax <= nearPointLength)
                    {
                        nearPointLength = scaleMax;
                    }
                    else
                        max = false;

                    //丸め量
                    int round = 500;//500mm単位で丸める
                    double roundLength = ClsGeo.RoundOff(nearPointLength, 5);
                    double remainder = roundLength % round;
                    if (remainder < round && remainder != 0.0)
                        roundLength = roundLength - remainder;

                    picPointLength = roundLength / 1000.0;// Math.Min, maxLength);

                    pickPoint = new XYZ(tmpStPoint.X + (ClsRevitUtil.CovertToAPI(roundLength) * dir.X),
                                tmpStPoint.Y + (ClsRevitUtil.CovertToAPI(roundLength) * dir.Y),
                                tmpStPoint.Z);
                }
                catch (OperationCanceledException e)
                {
                }
                finally
                {
                    // スケールの削除
                    t.Start();
                    ClsRevitUtil.Delete(doc, scaleIds);
                    t.Commit();

                    // 必ずトランザクションを終了させる
                    t.Dispose();
                }
            }

            return pickPoint;
        }
        public static List<ElementId> CreateScale(UIDocument uidoc, XYZ tmpStPoint, XYZ tmpEdPoint, WaritukeDist eDist, double maxLength = 7000.0, double minLength = 1000.0, bool cross = false)
        {
            List<ElementId> scaleIds = new List<ElementId>();

            Document doc = uidoc.Document;

            Autodesk.Revit.DB.View vActv = doc.ActiveView;
            int dist = ((int)eDist);
            double convetDist = ClsRevitUtil.CovertToAPI(dist);

            Line baseLine = Line.CreateBound(tmpStPoint, tmpEdPoint);
            XYZ dir = baseLine.Direction;
            double baseLength = ClsRevitUtil.CovertFromAPI(baseLine.Length);

            if (baseLength < dist)
                return null;

            //目盛り上限
            double scaleMax = baseLength;
            if (maxLength < scaleMax)
                scaleMax = maxLength;

            bool exception = false;
            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                try
                {
                    // スケールの作成
                    t.Start();
                    for (int i = 0; i <= scaleMax / dist; i++)
                    {
                        XYZ newPoint = new XYZ(tmpStPoint.X + (ClsRevitUtil.CovertToAPI(i * dist) * dir.X),
                                               tmpStPoint.Y + (ClsRevitUtil.CovertToAPI(i * dist) * dir.Y),
                                               tmpStPoint.Z + (ClsRevitUtil.CovertToAPI((i) * dist) * dir.Z));
                        Curve cv;
                        if (i % 10 == 0)
                        {
                            cv = CreateCurveToPoint(newPoint, dir, convetDist * 5);

                        }
                        else if (i % 5 == 0)
                        {
                            cv = CreateCurveToPoint(newPoint, dir, convetDist * 2.5);
                        }
                        else
                        {
                            cv = CreateCurveToPoint(newPoint, dir, convetDist);
                        }

                        XYZ mid = 0.5 * (cv.GetEndPoint(0) + cv.GetEndPoint(1));
                        XYZ viewDirection = vActv.ViewDirection;
                        if (exception)
                            viewDirection = XYZ.BasisZ;

                        Plane plane = Plane.CreateByNormalAndOrigin(viewDirection, mid);
                        SketchPlane sketchPlane = SketchPlane.Create(doc,  plane);
                        ModelLine modelLine = null;
                        //3D斜め上視点で作成すると例外になるため真上からのものも作成する
                        try
                        {
                            modelLine = doc.Create.NewModelCurve(cv as Line, sketchPlane) as ModelLine;
                        }
                        catch
                        {
                            plane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, mid);
                            sketchPlane = SketchPlane.Create(doc, plane);
                            modelLine = doc.Create.NewModelCurve(cv as Line, sketchPlane) as ModelLine;
                            cross = false;
                            exception = true;
                        }
                        if (modelLine == null)
                            continue;

                        scaleIds.Add(modelLine.Id);

                        Line axis = Line.CreateBound(mid, mid + viewDirection);
                        double angle = Math.PI / 2 - dir.AngleOnPlaneTo((cv as Line).Direction, viewDirection);
                        ClsRevitUtil.RotateElement(doc, modelLine.Id, axis, angle);
                        if(cross)
                        {
                            Plane crossPlane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, mid);
                            SketchPlane crossSketchPlane = SketchPlane.Create(doc, crossPlane);
                            ModelLine crossModelLine = doc.Create.NewModelCurve(cv as Line, crossSketchPlane) as ModelLine;
                            scaleIds.Add(crossModelLine.Id);
                            Line crossAxis = Line.CreateBound(mid, mid + XYZ.BasisZ);
                            ClsRevitUtil.RotateElement(doc, crossModelLine.Id, crossAxis, Math.PI / 2);
                        }

                        if (i % 10 == 0)
                        {
                            cv = (doc.GetElement(modelLine.Id).Location as LocationCurve).Curve;

                            //#34157
                            TextNoteType tnt = ClsRevitUtil.GetTextNoteTypeByName(doc, "WaritukeType");
                            if (tnt == null)
                            {
                                ElementId defaultTypeId = doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);

                                //デフォルトをこのまま使うのはまずい。コピーする　#34157
                                Element ele = doc.GetElement(defaultTypeId) as Element;
                                TextNoteType noteType = ele as TextNoteType;
                                Element newTypeEle = noteType.Duplicate("WaritukeType");
                                ele = doc.GetElement(newTypeEle.Id) as Element;
                                tnt = ele as TextNoteType;
                            }

                            TextNoteOptions opts = new TextNoteOptions(tnt.Id);
                            double maxWidth = TextNote.GetMaximumAllowedWidth(doc, tnt.Id);
                            maxWidth = ClsRevitUtil.CovertToAPI(maxWidth);
                            XYZ txtPoint = cv.GetEndPoint(1);
                            XYZ txtDir = (cv as Line).Direction;
                            TextNote tx = TextNote.Create(doc, vActv.Id,
                                    new XYZ(txtPoint.X - ClsRevitUtil.CovertToAPI(150) * txtDir.Y + ClsRevitUtil.CovertToAPI(200) * txtDir.X
                                , txtPoint.Y + ClsRevitUtil.CovertToAPI(200) * txtDir.Y - ClsRevitUtil.CovertToAPI(150) * txtDir.X
                                , txtPoint.Z),
                                                          maxWidth, (i * dist / 1000.0).ToString() + ".0", opts);
                            //tx.HorizontalAlignment = HorizontalTextAlignment.Center;
                            TextElementType textType = tx.Symbol;
                            BuiltInParameter paraIndex = BuiltInParameter.TEXT_SIZE;
                            Parameter textSize = textType.get_Parameter(paraIndex);

                            textSize.Set(maxWidth / 5);

                            paraIndex = BuiltInParameter.TEXT_BACKGROUND;
                            Parameter textBack = textType.get_Parameter(paraIndex);
                            textBack.Set(1);// 0 = Opaque（不透明） :: 1 = Transparent（透過）
                            scaleIds.Add(tx.Id);
                        }
                    }
                    t.Commit();
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("Info Message12", ex.Message);
                    //message = ex.Message;
                }
                //catch (OperationCanceledException e)
                //{
                //}
                finally
                {
                    // 必ずトランザクションを終了させる
                    t.Dispose();
                }
            }

            return scaleIds;
        }
        public static XYZ GetWaritukePoint(UIDocument uidoc, XYZ tmpStPoint, XYZ tmpEdPoint, out double picPointLength, out bool max, double maxLength = 7000.0, double minLength = 1000.0)
        {
            Document doc = uidoc.Document;

            XYZ pickPoint = null;

            Line baseLine = Line.CreateBound(tmpStPoint, tmpEdPoint);
            XYZ dir = baseLine.Direction;
            double baseLength = ClsRevitUtil.CovertFromAPI(baseLine.Length);
            picPointLength = 0;
            max = false;
            bool min = false;
            //目盛り上限
            double scaleMax = baseLength;
            if (maxLength < scaleMax)
            {
                scaleMax = maxLength;
            }
            else if (scaleMax < minLength)
            {
                min = true;
            }
            else
            {
                max = true;
            }

            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                Selection selection = uidoc.Selection;
                try
                {
                    // 割付位置の選択
                    pickPoint = selection.PickPoint("位置を選択してください");

                    while (min)
                    {
                        MessageBox.Show("割付長さを超えています");
                        pickPoint = selection.PickPoint("部材を選択してください");
                    }
                    //Z軸の統一
                    pickPoint = new XYZ(pickPoint.X, pickPoint.Y, tmpStPoint.Z);

                    //基準線からどちら周りで90度回転させるかの
                    double harfPI = Math.PI / 2;
                    XYZ picDir = Line.CreateBound(tmpStPoint, pickPoint).Direction;
                    if (ClsGeo.IsLeft(dir, picDir))
                        harfPI = -harfPI;

                    //指定点から基準線への垂線ベクトルの作成
                    picDir = new XYZ(dir.X * Math.Cos(harfPI) - dir.Y * Math.Sin(harfPI), dir.X * Math.Sin(harfPI) + dir.Y * Math.Cos(harfPI), dir.Z);

                    //指定点から無限に伸ばした直線
                    XYZ perpendicPoint = new XYZ(pickPoint.X + (ClsRevitUtil.CovertToAPI(int.MaxValue) * picDir.X),
                                               pickPoint.Y + (ClsRevitUtil.CovertToAPI(int.MaxValue) * picDir.Y),
                                               pickPoint.Z);
                    Curve perpendicCv = Line.CreateBound(pickPoint, perpendicPoint);

                    //始点から無限に伸ばした直線
                    XYZ extStPoint = new XYZ(tmpStPoint.X + (ClsRevitUtil.CovertToAPI(int.MaxValue) * dir.X),
                                               tmpStPoint.Y + (ClsRevitUtil.CovertToAPI(int.MaxValue) * dir.Y),
                                               tmpStPoint.Z);
                    Curve baseCV = Line.CreateBound(tmpStPoint, extStPoint);

                    //指定点から基準線に下ろした垂線との交点を取得
                    XYZ insecPoint = ClsRevitUtil.GetIntersection(baseCV, perpendicCv);

                    double nearPointLength = minLength;
                    if (insecPoint != null)
                    {
                        Line putLength = Line.CreateBound(tmpStPoint, insecPoint);
                        nearPointLength = ClsRevitUtil.CovertFromAPI(putLength.Length);
                    }

                    if (scaleMax <= nearPointLength)
                    {
                        nearPointLength = scaleMax;
                    }
                    else
                        max = false;

                    //丸め量
                    int round = 500;//500mm単位で丸める
                    double roundLength = ClsGeo.RoundOff(nearPointLength, 5);
                    double remainder = roundLength % round;
                    if (remainder < round && remainder != 0.0)
                        roundLength = roundLength - remainder;

                    picPointLength = roundLength / 1000.0;// Math.Min, maxLength);

                    if (max && roundLength != scaleMax)
                        max = false;

                    pickPoint = new XYZ(tmpStPoint.X + (ClsRevitUtil.CovertToAPI(roundLength) * dir.X),
                                tmpStPoint.Y + (ClsRevitUtil.CovertToAPI(roundLength) * dir.Y),
                                tmpStPoint.Z);
                }
                catch (OperationCanceledException e)
                {
                }
                finally
                {
                    // 必ずトランザクションを終了させる
                    t.Dispose();
                }
            }

            return pickPoint;
        }

        /// <summary>
        /// 連続して配置されていることを前提とした主材のリストから全体の両端点を取得する
        /// </summary>
        /// <param name="ids">主材　ツインビーム　高強度主材のid群　特にチェックは入れてないので変なものを入れないこと！</param>
        /// <param name="startPoint">算出された始点</param>
        /// <param name="endPoint">算出された終点</param>
        public static　bool GetStartEndPoint(Document doc,List<ElementId> ids, ref XYZ startPoint, ref XYZ endPoint)
        {

            List<XYZ> sPntList = new List<XYZ>();
            List<XYZ> ePntList = new List<XYZ>();

            List<XYZ> sRemainPntList = new List<XYZ>();
            List<XYZ> eRemainPntList = new List<XYZ>();

            //主材群の始点と終点のリストをそれぞれ作成
            foreach (ElementId id in ids)
            {
                FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                if (inst == null)
                {
                    return false;
                }

                LocationPoint lPoint = inst.Location as LocationPoint;
                if (lPoint == null)
                {
                    return false;
                }

                XYZ sp = new XYZ(lPoint.Point.X, lPoint.Point.Y, lPoint.Point.Z);

                FamilySymbol symtmp = inst.Symbol;
                double lengthTmp = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(symtmp, "長さ"));
                XYZ ep = sp + ClsRevitUtil.CovertToAPI(lengthTmp) * inst.HandOrientation;

                sPntList.Add(ClsRevitUtil.ConvertPointRevit2Geo(sp));
                ePntList.Add(ClsRevitUtil.ConvertPointRevit2Geo(ep));
            }

            foreach(XYZ sptmp in sPntList)
            {
                bool hit = false;
                //全終点リストと同座標チェックを行い、終点と位置被りがないものだけをスタックする
                foreach (XYZ eptmp in ePntList)
                {
                    if (ClsGeo.GEO_EQ(sptmp.X, eptmp.X) &&
                        ClsGeo.GEO_EQ(sptmp.Y, eptmp.Y) &&
                        ClsGeo.GEO_EQ(sptmp.Z, eptmp.Z))
                    {
                        hit = true;
                        break;
                    }
                }

                if (!hit)
                {
                    //重複なし
                    sRemainPntList.Add(sptmp);
                }
            }



            foreach (XYZ eptmp in ePntList)
            {
                bool hit = false;
                //全始点リストと同座標チェックを行い、始点と位置被りがないものだけをスタックする
                foreach (XYZ sptmp in sPntList)
                {
                    if (ClsGeo.GEO_EQ(sptmp.X, eptmp.X) &&
                        ClsGeo.GEO_EQ(sptmp.Y, eptmp.Y) &&
                        ClsGeo.GEO_EQ(sptmp.Z, eptmp.Z))
                    {
                        hit = true;
                        break;
                    }
                }

                if (!hit)
                {
                    //重複なし
                    eRemainPntList.Add(eptmp);
                }
            }

            //始点-終点 + 始点-終点 +始点-終点 +始点-終点 …　の並びになっていれば、
            //sRemainPntList eRemainPntList はそれぞれ1個ずつのはず
            if (eRemainPntList.Count == 1 && sRemainPntList.Count == 1)
            {
                startPoint = sRemainPntList[0];
                endPoint = eRemainPntList[0];
            }
            else
            {
                //主材　始点-終点 +　終点-始点 + 終点-始点　のようにがイレギュラーな並び方をしている
                List<XYZ> pntList = new List<XYZ>();
                pntList.AddRange(sPntList);
                pntList.AddRange(ePntList);

                List<XYZ> remainPntList = new List<XYZ>();

                bool chouhuku = false;
                for (int i = 0; i < pntList.Count(); i++)
                {
                    for (int j = 0; j < pntList.Count(); j++)
                    {
                        if (i == j)
                        {
                            continue;
                        }

                        if (ClsGeo.GEO_EQ(pntList[i].X, pntList[j].X) &&
                        ClsGeo.GEO_EQ(pntList[i].Y, pntList[j].Y) &&
                        ClsGeo.GEO_EQ(pntList[i].Z, pntList[j].Z))
                        {
                            chouhuku = true;
                            break;
                        }
                    }

                    if (!chouhuku)
                    {
                        remainPntList.Add(pntList[i]);
                    }
                }

                if (remainPntList.Count == 2)
                {
                    //両端が取れているのでOK
                    startPoint = remainPntList[0];
                    endPoint = remainPntList[1];
                }
                else
                {
                    //座標取得失敗
                    return false;
                }
            }

            startPoint = ClsRevitUtil.ConvertPointGeo2Revit(startPoint);
            endPoint = ClsRevitUtil.ConvertPointGeo2Revit(endPoint);

            return true;
        }

        /// 仮鋼材orベース のみを単独選択
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="id">選択した 仮鋼材orベース のID</param>
        /// <param name="message">指示</param>
        /// <returns></returns>
        public static bool PickBaseObject(UIDocument uidoc, ref ElementId id, string message = "仮鋼材orベース")
        {
            //仮鋼材部品名orベースを取得
            List<string> kariKouzaiList = new List<string>();
            kariKouzaiList.AddRange(Master.ClsYamadomeCsv.GetKariFamilyNameList().ToList());
            kariKouzaiList.AddRange(Master.ClsYamadomeCsv.GetKariSyabariFamilyNameList().ToList());
            kariKouzaiList.AddRange(Master.ClsChannelCsv.GetFamilyNameList().ToList());
            kariKouzaiList.AddRange(Master.ClsHiuchiCsv.GetFamilyNameList().ToList());
            kariKouzaiList.AddRange(Master.ClsJackCsv.GetFamilyNameList().ToList());
            kariKouzaiList.AddRange(Master.ClsSupportPieceCsv.GetFamilyNameList().ToList());
            kariKouzaiList.AddRange(Master.ClsBurumanCSV.GetFamilyNameList().ToList());
            kariKouzaiList.AddRange(ClsGlobal.m_baseShinList.ToList());

            return ClsRevitUtil.PickObjectSymbolFilters(uidoc, message, kariKouzaiList, ref id);
        }

        public static bool PickWaritukeShuzaiObject(UIDocument uidoc, ref ElementId id, string message = "割付主材")
        {
            List<string> kouzaiList = new List<string>();
            kouzaiList.AddRange(Master.ClsYamadomeCsv.GetYamadomeFamilynameList().ToList());

            return ClsRevitUtil.PickObjectSymbolFilters(uidoc, message, kouzaiList, ref id);
        }

        public static bool PickWaritukeShuzaiObjects(UIDocument uidoc, ref List<ElementId> ids, string message = "割付主材")
        {
            List<string> kouzaiList = new List<string>();
            kouzaiList.AddRange(Master.ClsYamadomeCsv.GetYamadomeFamilynameList().ToList());

            return ClsRevitUtil.PickObjectsSymbolFilters(uidoc, message, kouzaiList,ref ids);
        }

        /// <summary>
        /// メガビームを除いた割付部材（主材　ツインビーム　高強度主材）を選択させる
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="ids"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool PickWaritukeShuzaiObjectsWithoutMegabeam(UIDocument uidoc, ref List<ElementId> ids, string message = "対象の割付主材を選択してください（複数選択可）")
        {
            List<string> kouzaiList = new List<string>();
            List<string> kouzaiList2 = new List<string>();
            kouzaiList.AddRange(Master.ClsYamadomeCsv.GetYamadomeFamilynameList().ToList());

            foreach (string str in kouzaiList)
            {
                if(!str.Contains("高強度腹起"))
                {
                    kouzaiList2.Add(str);
                }
            }

            return ClsRevitUtil.PickObjectsSymbolFilters(uidoc, message, kouzaiList2, ref ids);
        }

        /// <summary>
        /// 指定の点からdirの向きに指定距離離したCurve
        /// </summary>
        /// <param name="point">指定点</param>
        /// <param name="dir">向き</param>
        /// <param name="dist">距離</param>
        /// <returns></returns>
        private static Curve CreateCurveToPoint(XYZ point, XYZ dir, double dist)
        {
            Curve cv = Line.CreateBound(new XYZ(point.X - (dist * dir.X), point.Y - (dist * dir.Y), point.Z),// - (dist * dir.Z)),
                                                  new XYZ(point.X + (dist * dir.X), point.Y + (dist * dir.Y), point.Z));// + (dist * dir.Z)));
            return cv;
        }
        /// <summary>
        /// 対象のファミリに割付で作成したフラグをカスタムデータとして持たせる
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id">ファミリ</param>
        /// <returns></returns>
        public static bool SetWarituke(Document doc, ElementId id)
        {
            return ClsRevitUtil.CustomDataSet<string>(doc, id, WARITUKE, WARITUKE);
        }
        /// <summary>
        /// 対象のファミリから割付で作成したフラグのカスタムデータを取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id">ファミリ</param>
        /// <returns>フラグ</returns>
        public static string GetWarituke(Document doc, ElementId id)
        {
            ClsRevitUtil.CustomDataGet(doc, id, WARITUKE, out string waritukeFlag);
            return waritukeFlag;
        }
    }
}
