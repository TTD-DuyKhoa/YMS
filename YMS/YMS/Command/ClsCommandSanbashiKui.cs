using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using YMS.DLG;
using YMS.Parts;
using static Autodesk.Revit.DB.SpecTypeId;

namespace YMS.Command
{
    class ClsCommandSanbashiKui
    {
        /// <summary>
        /// 杭作成
        /// </summary>
        /// <param name="uidoc"></param>
        /// <remarks>TC杭・構台杭・兼用杭・断面変化杭</remarks>
        public static void CommandCreateKui(UIDocument uidoc)
        {
            Document doc = uidoc.Document;

            DlgCreateSanbashikuiKenyoukui dlgCreateSanbashikuiKenyoukui = new DlgCreateSanbashikuiKenyoukui(doc);
            DialogResult result = dlgCreateSanbashikuiKenyoukui.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            ClsSanbashiKui clsKui = dlgCreateSanbashikuiKenyoukui.m_ClsKui;

            string levelName = dlgCreateSanbashikuiKenyoukui.m_Level;
            ElementId levelID = ClsRevitUtil.GetLevelID(doc, levelName);

            while (true)
            {
                try
                {
                    string uniqueName = string.Empty;

                    List<XYZ> insertPoints = new List<XYZ>();
                    switch (clsKui.m_CreateType)
                    {
                        case ClsSanbashiKui.CreateType.Intersection:
                            List<ElementId> ids = new List<ElementId>();
                            ClsRevitUtil.PickObjectsPartFilter(uidoc, "下書き線を選択してください", "モデル線分", ref ids);
                            GetIntersectionPoints(uidoc, insertPoints, ids);
                            break;
                        case ClsSanbashiKui.CreateType.OnePoint:
                            Selection selection = uidoc.Selection;
                            XYZ selectedPoint = selection.PickPoint("挿入点を指定してください");
                            insertPoints.Add(selectedPoint);
                            break;
                        default:
                            return;
                    }

                    if (insertPoints.Count == 0)
                    {
                        break;
                    }

                    bool renzoku = false;
                    int cnt = 1;
                    foreach (XYZ point in insertPoints)
                    {
                        if (cnt > 1)
                        {
                            renzoku = true;
                        }
                        clsKui.CreateSanbashiKui(doc, point, levelID, renzoku, ref uniqueName);
                        cnt++;
                    }

                }
                catch (Exception)
                {
                    break;
                }
            }

        }

        public static void CommandChangeKui(UIDocument uidoc)
        {
            Document doc = uidoc.Document;

            DlgChangeSanbashikuiKenyoukui dlgChangeSanbashikuiKenyoukui = new DlgChangeSanbashikuiKenyoukui();
            DialogResult result1 = dlgChangeSanbashikuiKenyoukui.ShowDialog();
            if (result1 != DialogResult.OK)
            {
                return;
            }

            List<ElementId> ids = null;
            if (!ClsSanbashiKui.PickObjects(uidoc, dlgChangeSanbashikuiKenyoukui.m_PileType, ref ids))
            {
                return;
            }
            if (ids == null || ids.Count() == 0)
            {
                return;
            }

            ClsSanbashiKui template = new ClsSanbashiKui();
            for (int i = 0; i < ids.Count(); i++)
            {
                ElementId id = ids[i];
                if (i == 0)
                {
                    template.SetParameter(doc, id);
                }
                else
                {
                    FamilyInstance instKui = doc.GetElement(id) as FamilyInstance;
                    if (instKui == null)
                    {
                        return;
                    }

                    ClsSanbashiKui trgKui = new ClsSanbashiKui();
                    trgKui.SetParameter(doc, id);

                    template.m_IsFromGL = ClsRevitUtil.CompareValues(trgKui.m_IsFromGL, template.m_IsFromGL);
                    template.m_HightFromGL = ClsRevitUtil.CompareValues(trgKui.m_HightFromGL, template.m_HightFromGL);
                    template.m_PileType = ClsRevitUtil.CompareValues(trgKui.m_PileType, template.m_PileType);
                    template.m_CreateType = ClsRevitUtil.CompareValues(trgKui.m_CreateType, template.m_CreateType);
                    template.m_KouzaiType = ClsRevitUtil.CompareValues(trgKui.m_KouzaiType, template.m_KouzaiType);
                    template.m_KouzaiSize = ClsRevitUtil.CompareValues(trgKui.m_KouzaiSize, template.m_KouzaiSize);
                    template.m_KouzaiSizeKoudaikui = ClsRevitUtil.CompareValues(trgKui.m_KouzaiSizeKoudaikui, template.m_KouzaiSizeKoudaikui);
                    template.m_KouzaiSizeDanmenHenkaKui = ClsRevitUtil.CompareValues(trgKui.m_KouzaiSizeDanmenHenkaKui, template.m_KouzaiSizeDanmenHenkaKui);
                    template.m_PileTotalLength = ClsRevitUtil.CompareValues(trgKui.m_PileTotalLength, template.m_PileTotalLength);
                    template.m_PileTotalLength_Top = ClsRevitUtil.CompareValues(trgKui.m_PileTotalLength_Top, template.m_PileTotalLength_Top);
                    template.m_PileTotalLength_Bottom = ClsRevitUtil.CompareValues(trgKui.m_PileTotalLength_Bottom, template.m_PileTotalLength_Bottom);
                    template.m_PileAngle = ClsRevitUtil.CompareValues(trgKui.m_PileAngle, template.m_PileAngle);
                    template.m_Zanti = ClsRevitUtil.CompareValues(trgKui.m_Zanti, template.m_Zanti);
                    template.m_ZantiLength = ClsRevitUtil.CompareValues(trgKui.m_ZantiLength, template.m_ZantiLength);
                    template.m_UseTopPlate = ClsRevitUtil.CompareValues(trgKui.m_UseTopPlate, template.m_UseTopPlate);
                    template.m_TopPlateSize = ClsRevitUtil.CompareValues(trgKui.m_TopPlateSize, template.m_TopPlateSize);
                    template.m_EndPlateThickness = ClsRevitUtil.CompareValues(trgKui.m_EndPlateThickness, template.m_EndPlateThickness);

                    template.m_TsugiteCount = ClsRevitUtil.CompareValues(trgKui.m_TsugiteCount, template.m_TsugiteCount);
                    template.m_TsugiteCount2 = ClsRevitUtil.CompareValues(trgKui.m_TsugiteCount2, template.m_TsugiteCount2);
                    template.m_FixingType = ClsRevitUtil.CompareValues(trgKui.m_FixingType, template.m_FixingType);
                    template.m_FixingType2 = ClsRevitUtil.CompareValues(trgKui.m_FixingType2, template.m_FixingType2);
                    //template.m_PileLengthList              = ClsRevitUtil.CompareValues(trgKui.m_PileLengthList, template.m_PileLengthList);
                    //template.m_PileLengthList2             = ClsRevitUtil.CompareValues(trgKui.m_PileLengthList2, template.m_PileLengthList2);
                    template.m_TsugitePlateSize_Out = ClsRevitUtil.CompareValues(trgKui.m_TsugitePlateSize_Out, template.m_TsugitePlateSize_Out);
                    template.m_TsugitePlateSize_Out2 = ClsRevitUtil.CompareValues(trgKui.m_TsugitePlateSize_Out2, template.m_TsugitePlateSize_Out2);
                    template.m_TsugitePlateQuantity_Out = ClsRevitUtil.CompareValues(trgKui.m_TsugitePlateQuantity_Out, template.m_TsugitePlateQuantity_Out);
                    template.m_TsugitePlateQuantity_Out2 = ClsRevitUtil.CompareValues(trgKui.m_TsugitePlateQuantity_Out2, template.m_TsugitePlateQuantity_Out2);
                    template.m_TsugitePlateSize_In = ClsRevitUtil.CompareValues(trgKui.m_TsugitePlateSize_In, template.m_TsugitePlateSize_In);
                    template.m_TsugitePlateSize_In2 = ClsRevitUtil.CompareValues(trgKui.m_TsugitePlateSize_In2, template.m_TsugitePlateSize_In2);
                    template.m_TsugitePlateQuantity_In = ClsRevitUtil.CompareValues(trgKui.m_TsugitePlateQuantity_In, template.m_TsugitePlateQuantity_In);
                    template.m_TsugitePlateQuantity_In2 = ClsRevitUtil.CompareValues(trgKui.m_TsugitePlateQuantity_In2, template.m_TsugitePlateQuantity_In2);
                    template.m_TsugitePlateSize_Web1 = ClsRevitUtil.CompareValues(trgKui.m_TsugitePlateSize_Web1, template.m_TsugitePlateSize_Web1);
                    template.m_TsugitePlateSize_Web1_2 = ClsRevitUtil.CompareValues(trgKui.m_TsugitePlateSize_Web1_2, template.m_TsugitePlateSize_Web1_2);
                    template.m_TsugitePlateQuantity_Web1 = ClsRevitUtil.CompareValues(trgKui.m_TsugitePlateQuantity_Web1, template.m_TsugitePlateQuantity_Web1);
                    template.m_TsugitePlateQuantity_Web1_2 = ClsRevitUtil.CompareValues(trgKui.m_TsugitePlateQuantity_Web1_2, template.m_TsugitePlateQuantity_Web1_2);
                    template.m_TsugitePlateSize_Web2 = ClsRevitUtil.CompareValues(trgKui.m_TsugitePlateSize_Web2, template.m_TsugitePlateSize_Web2);
                    template.m_TsugitePlateSize_Web2_2 = ClsRevitUtil.CompareValues(trgKui.m_TsugitePlateSize_Web2_2, template.m_TsugitePlateSize_Web2_2);
                    template.m_TsugitePlateQuantity_Web2 = ClsRevitUtil.CompareValues(trgKui.m_TsugitePlateQuantity_Web2, template.m_TsugitePlateQuantity_Web2);
                    template.m_TsugitePlateQuantity_Web2_2 = ClsRevitUtil.CompareValues(trgKui.m_TsugitePlateQuantity_Web2_2, template.m_TsugitePlateQuantity_Web2_2);
                    template.m_TsugiteBoltSize_Flange = ClsRevitUtil.CompareValues(trgKui.m_TsugiteBoltSize_Flange, template.m_TsugiteBoltSize_Flange);
                    template.m_TsugiteBoltSize_Flange2 = ClsRevitUtil.CompareValues(trgKui.m_TsugiteBoltSize_Flange2, template.m_TsugiteBoltSize_Flange2);
                    template.m_TsugiteBoltQuantity_Flange = ClsRevitUtil.CompareValues(trgKui.m_TsugiteBoltQuantity_Flange, template.m_TsugiteBoltQuantity_Flange);
                    template.m_TsugiteBoltQuantity_Flange2 = ClsRevitUtil.CompareValues(trgKui.m_TsugiteBoltQuantity_Flange2, template.m_TsugiteBoltQuantity_Flange2);
                    template.m_TsugiteBoltSize_Web = ClsRevitUtil.CompareValues(trgKui.m_TsugiteBoltSize_Web, template.m_TsugiteBoltSize_Web);
                    template.m_TsugiteBoltSize_Web2 = ClsRevitUtil.CompareValues(trgKui.m_TsugiteBoltSize_Web2, template.m_TsugiteBoltSize_Web2);
                    template.m_TsugiteBoltQuantity_Web = ClsRevitUtil.CompareValues(trgKui.m_TsugiteBoltQuantity_Web, template.m_TsugiteBoltQuantity_Web);
                    template.m_TsugiteBoltQuantity_Web2 = ClsRevitUtil.CompareValues(trgKui.m_TsugiteBoltQuantity_Web2, template.m_TsugiteBoltQuantity_Web2);

                }

            }

            DlgCreateSanbashikuiKenyoukui dlgCreateSanbashikuiKenyoukui = new DlgCreateSanbashikuiKenyoukui(doc, template);
            DialogResult result2 = dlgCreateSanbashikuiKenyoukui.ShowDialog();
            if (result2 != DialogResult.OK)
            {
                return;
            }

            string uniqueName = string.Empty;

            bool renzoku = false;
            int cnt = 1;
            foreach (var id in ids)
            {
                if (cnt > 1)
                {
                    renzoku = true;
                }
                ClsSanbashiKui clsSanbashiKui = dlgCreateSanbashikuiKenyoukui.m_ClsKui;
                FamilyInstance instKui = doc.GetElement(id) as FamilyInstance;
                XYZ point = (instKui.Location as LocationPoint).Point;
                string levelName = dlgCreateSanbashikuiKenyoukui.m_Level;
                ElementId levelId = ClsRevitUtil.GetLevelID(doc, levelName);

                using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                {
                    t.Start();
                    ClsRevitUtil.Delete(doc, id);
                    t.Commit();
                }


                clsSanbashiKui.CreateSanbashiKui(doc, point, levelId, renzoku, ref uniqueName);
                cnt++;
            }

            return;
        }

        private static void GetIntersectionPoints(UIDocument uidoc, List<XYZ> insertPoints, List<ElementId> ids)
        {
            FilteredElementCollector collector = new FilteredElementCollector(uidoc.Document);
            List<Curve> curves = new List<Curve>();
            foreach (ElementId id in ids)
            {
                Element element = collector.WhereElementIsNotElementType().Where(x => x.Id == id).FirstOrDefault();
                if (element is CurveElement curveElement)
                {
                    curves.Add(curveElement.GeometryCurve);
                }
            }

            // 交点の取得
            for (int i = 0; i < curves.Count - 1; i++)
            {
                for (int j = i + 1; j < curves.Count; j++)
                {
                    if (curves[i].Intersect(curves[j]) is SetComparisonResult res && res == SetComparisonResult.Overlap)
                    {
                        IntersectionResultArray intersectionPointsArray;
                        curves[i].Intersect(curves[j], out intersectionPointsArray);

                        foreach (IntersectionResult intersectionResult in intersectionPointsArray)
                        {
                            XYZ intersectionPoint = intersectionResult.XYZPoint;
                            insertPoints.Add(intersectionPoint);
                        }
                    }
                }
            }

            List<XYZ> insertPoints2 = new List<XYZ>();

            

            for (int i = 0; i < insertPoints.Count; i++)
            {
                if (i == insertPoints.Count - 1)
                {
                    bool hit = false;
                    foreach (XYZ p in insertPoints2)
                    {
                        if (ClsGeo.GEO_EQ(insertPoints[i].X, p.X, 0.1) &&
                          ClsGeo.GEO_EQ(insertPoints[i].Y, p.Y, 0.1) &&
                          ClsGeo.GEO_EQ(insertPoints[i].Z, p.Z, 0.1))
                        {
                            hit = true;
                            break;
                        }
                    }

                    if (!hit)
                    {
                        insertPoints2.Add(insertPoints[i]);
                    }
                }
                else
                {
                    bool hit = false;

                    for (int j = i + 1; j < insertPoints.Count; j++)
                    {
                        if (ClsGeo.GEO_EQ(insertPoints[i].X, insertPoints[j].X, 0.1) &&
                           ClsGeo.GEO_EQ(insertPoints[i].Y, insertPoints[j].Y, 0.1) &&
                           ClsGeo.GEO_EQ(insertPoints[i].Z, insertPoints[j].Z, 0.1))
                        {
                            hit = true;
                            break;
                        }
                    }
                    if (!hit)
                    {
                        insertPoints2.Add(insertPoints[i]);
                    }
                }

               
            }

            insertPoints.Clear();
            insertPoints.AddRange(insertPoints2);
        }
    }
}
