using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using YMS.DLG;
using YMS.Parts;
using static Autodesk.Revit.DB.SpecTypeId;

namespace YMS.Command
{
    class ClsCommandJack
    {
        /// <summary>
        /// ジャッキの作図
        /// </summary>
        /// <param name="uidoc"></param>
        public static bool CreateJack(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            try
            {
                for (; ; )
                {
                    // 対象となるベースを選択
                    ElementId baseId = null;
                    if (!ClsRevitUtil.PickObjectFilters(uidoc, "切梁ベース or 切梁火打ベース or 隅火打ベースを指定してください。（Escキーで機能終了）", ClsGlobal.m_baseKiriOrKHiuchiOrCHiuchiOrSyabariList, ref baseId))
                    {
                        break;
                    }

                    FamilyInstance instBase = doc.GetElement(baseId) as FamilyInstance;

                    string syuzaiSize = string.Empty;
                    switch (instBase.Name)
                    {
                        case ClsKiribariBase.baseName:
                            ClsKiribariBase clsKiri = new ClsKiribariBase();
                            clsKiri.SetParameter(doc, baseId);
                            syuzaiSize = clsKiri.m_kouzaiSize;
                            break;
                        case ClsKiribariHiuchiBase.baseName:
                            ClsKiribariHiuchiBase clsKiriHiuchi = new ClsKiribariHiuchiBase();
                            clsKiriHiuchi.SetParameter(doc, baseId);
                            syuzaiSize = clsKiriHiuchi.m_SteelSizeSingle;
                            break;
                        case ClsCornerHiuchiBase.baseName:
                            ClsCornerHiuchiBase clsCHiuchi = new ClsCornerHiuchiBase();
                            clsCHiuchi.SetParameter(doc, baseId);
                            syuzaiSize = clsCHiuchi.m_SteelSize;
                            break;
                        case "斜梁ベース"://仮でべた書き
                            syuzaiSize = ClsRevitUtil.GetInstMojiParameter(doc, baseId, "鋼材サイズ");
                            break;
                        default:
                            break;
                    }

                    if (string.IsNullOrWhiteSpace(syuzaiSize))
                    {
                        MessageBox.Show("ベースの内部情報取得に失敗しました。");
                        continue;
                    }

                    //#31640
                    ClsKiribariBase templateBase = new ClsKiribariBase();
                    templateBase.SetParameter(doc, baseId);
                    string jackType = ClsRevitUtil.CompareValues(ClsRevitUtil.GetParameter(doc, baseId, "ジャッキタイプ(1)"), templateBase.m_jack1);

                    // ジャッキ作成ダイアログを表示
                    DLG.DlgCreateJack dlgCreateJack = new DLG.DlgCreateJack(doc, syuzaiSize, jackType);
                    DialogResult result = dlgCreateJack.ShowDialog();
                    if (result != DialogResult.OK)
                    {
                        continue;
                    }

                    ClsJack clsJack = dlgCreateJack.m_ClsJack;

                    XYZ sp = new XYZ();
                    XYZ ep = new XYZ();
                    Curve cv = null;
                    if (instBase.Location is LocationPoint lPoint)
                    {
                        sp = lPoint.Point;
                        var dist = ClsRevitUtil.GetParameterDouble(doc, baseId, "長さ");
                        ep = sp + dist * instBase.HandOrientation;
                        cv = Line.CreateBound(sp, ep);
                    }
                    else
                    {
                        LocationCurve lCurve = instBase.Location as LocationCurve;
                        cv = lCurve.Curve;
                        sp = cv.GetEndPoint(0);
                        ep = cv.GetEndPoint(1);
                    }

                    XYZ vec3d = (ep - sp).Normalize();

                    // ジャッキの挿入点
                    XYZ pickPoint;
                    Selection selection = uidoc.Selection;
                    try
                    {
                        pickPoint = selection.PickPoint("ベースの始点方向を指定してください");
                    }
                    catch (Exception e)
                    {
                        continue;
                    }

                    double distanceToSt = pickPoint.DistanceTo(sp);
                    double distanceToEd = pickPoint.DistanceTo(ep);

                    // ピック点に近い方を採用
                    if (distanceToSt > distanceToEd)
                    {
                        sp = cv.GetEndPoint(1);
                        ep = cv.GetEndPoint(0);
                    }
                    vec3d = (ep - sp).Normalize();


                    XYZ insertPoint;
                    if (clsJack.m_UseOffset)
                    {
                        double dHanare = ClsRevitUtil.ConvertDoubleGeo2Revit(clsJack.m_Offset);
                        insertPoint = sp + vec3d * dHanare;
                        clsJack.CreateJack(doc, baseId, sp, ep, insertPoint);
                    }
                    else
                    {
                        for (; ; )
                        {
                            try
                            {
                                insertPoint = selection.PickPoint("ジャッキの配置箇所を指定してください");
                                XYZ checkPoint = new XYZ(insertPoint.X, insertPoint.Y, 0);
                                Curve checkCv = Line.CreateBound(new XYZ(sp.X, sp.Y, 0), new XYZ(ep.X, ep.Y, 0));
                                if (ClsRevitUtil.IsPointOnCurve(checkCv, checkPoint))
                                {
                                    clsJack.CreateJack(doc, baseId, sp, ep, insertPoint);
                                }
                                else
                                {
                                    MessageBox.Show("選択したベース上の点を指定してください");
                                }
                            }
                            catch (Exception e)
                            {
                                break;
                            }
                        }
                    }

                   
                }
            }
            catch (Exception e)
            {

                return false;
            }
            

            return true;
        }

        /// <summary>
        /// ジャッキの変更
        /// </summary>
        /// <param name="uidoc"></param>
        public static bool ChangeJack(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            List<string> jackNameList = Master.ClsJackCsv.GetFamilyNameList().ToList();
            // 対象となるジャッキを選択
            ElementId id = null;
            if (!ClsRevitUtil.PickObjectSymbolFilters(uidoc, "ジャッキを指定してください", jackNameList, ref id))
            {
                return false;
            }
            Element selPartner = doc.GetElement(id);
            FamilyInstance inst = selPartner as FamilyInstance;
            ElementId levelID = inst.LevelId;//ジャッキはホストはnullそのためこちら
            string strSymbolName = inst.Name;
            LocationPoint lPoint = inst.Location as LocationPoint;
            double offset = ClsRevitUtil.GetParameterDouble(doc, id, "基準レベルからの高さ");

            //ジャッキの挿入角度
            double angle = lPoint.Rotation;
            // ジャッキの挿入点
            XYZ insertPoint = lPoint.Point;

            string strJackSize = Master.ClsJackCsv.GetSize(inst.Symbol.FamilyName);
            string strJackType = Master.ClsJackCsv.GetType(strJackSize);
            string strSyuzaiSize = Master.ClsJackCsv.GetWidth(strJackSize);

            // ジャッキ作成ダイアログを表示
            DLG.DlgCreateJack dlgCreateJack = new DLG.DlgCreateJack(doc, strSyuzaiSize, strJackType);
            DialogResult result = dlgCreateJack.ShowDialog();
            if (result != DialogResult.OK)
            {
                return false;
            }

            ElementId baseId = ClsKariKouzai.GetBaseId(doc, id);
            FamilyInstance instBase = doc.GetElement(baseId) as FamilyInstance;
            LocationCurve lCurve = instBase.Location as LocationCurve;
            XYZ sp = lCurve.Curve.GetEndPoint(0);
            XYZ ep = lCurve.Curve.GetEndPoint(1);

            ClsJack cjk = dlgCreateJack.m_ClsJack;
            strSymbolName = cjk.ChangeTypeNameToBaseName(strSymbolName);

            //変更するジャッキを削除
            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                t.Start();
                ClsRevitUtil.Delete(doc, id);
                t.Commit();
            }

            cjk.CreateJack(doc, baseId, sp, ep, insertPoint, true, angle);

            return true;
        }
    }

}
