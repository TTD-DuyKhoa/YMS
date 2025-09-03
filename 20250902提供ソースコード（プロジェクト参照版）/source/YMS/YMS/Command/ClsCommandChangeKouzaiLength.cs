using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YMS.Parts;
using RevitUtil;
using Autodesk.Revit.UI.Selection;


namespace YMS.Command
{
    /// <summary>
    /// 鋼材長さ変更コマンド
    /// </summary>
    class ClsCommandChangeKouzaiLength
    {
        public static bool CommandChangeKouzaiLength(UIDocument uidoc)
        {
            Document doc = uidoc.Document;

            //ベース選択
            //ClsGlobal.m_baseShinList

            //ElementId baseId = null;

            //if (!ClsRevitUtil.PickObjectSymbolFilters(uidoc, "基準となるベースを選択してください。", ClsGlobal.m_baseShinList, ref baseId))
            //{
            //    return false;
            //}

            //複数選択　
            List<ElementId> selectedIds = new List<ElementId>();
            if (!ClsWarituke.PickWaritukeShuzaiObjectsWithoutMegabeam(uidoc, ref selectedIds))
            {
                return false;
            }

            if (selectedIds.Count < 1)
            {
                return false;
            }

            //複数選択の統合で、選択された主材同士が真直で接続していなければ処理を処理を終了する
            var targetParamsList = new List<(XYZ startPoint, XYZ vec, double length)>();
            foreach (ElementId id in selectedIds)
            {
                FamilyInstance instance = doc.GetElement(id) as FamilyInstance;
                LocationPoint locationpoint = instance.Location as LocationPoint;
                FamilySymbol symtmp = instance.Symbol;
                double lengthTmp = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(symtmp, "長さ"));

                XYZ startPoint = new XYZ(locationpoint.Point.X, locationpoint.Point.Y, locationpoint.Point.Z);
                XYZ vec = instance.HandOrientation;
                double len = ClsRevitUtil.CovertToAPI(lengthTmp);
                (XYZ startPoint, XYZ vec, double length) targetParams = (startPoint, vec, len);
                targetParamsList.Add(targetParams);
            }
            int ConnectStraightCount = 0;
            for (int i = 0; i < targetParamsList.Count(); i++)
            {
                for (int j = i + 1; j < targetParamsList.Count(); j++)
                {
                    if (ClsRevitUtil.IsConnectStraight(targetParamsList[i].startPoint, targetParamsList[i].vec, targetParamsList[i].length,
                        targetParamsList[j].startPoint, targetParamsList[j].vec, targetParamsList[j].length))
                    {
                        ConnectStraightCount++;
                    }
                }
            }
            if(ConnectStraightCount != selectedIds.Count() - 1)
            {
                MessageBox.Show("選択された複数の主材が真直に接続していません。");
                return false;
            }

            FamilyInstance inst = doc.GetElement(selectedIds[0]) as FamilyInstance;
            if (inst == null)
            {
                return false;
            }

            ElementId levelid = inst.LevelId;

            FamilySymbol sym = inst.Symbol;
            Element elem = doc.GetElement(selectedIds[0]);
            string targetTypeName = ClsRevitUtil.GetTypeName(elem);

            if (string.IsNullOrWhiteSpace(targetTypeName))
            {
                MessageBox.Show("タイプ名の取得に失敗しました。");
                return false;
            }

            string targetShoubunrui = ClsRevitUtil.GetTypeParameterString(sym, "小分類");

            if (string.IsNullOrWhiteSpace(targetShoubunrui))
            {
                MessageBox.Show("小分類の取得に失敗しました。");
                return false;
            }

            string targetKigou = ClsRevitUtil.GetTypeParameterString(sym, "記号");

            if (string.IsNullOrWhiteSpace(targetShoubunrui))
            {
                MessageBox.Show("記号の取得に失敗しました。");
                return false;
            }

            List<ElementId> plateIds = ClsCoverPlate.GetAllCoverplate(doc);

            //ClsRevitUtil.SelectElement(uidoc, plateIds);

            double length = 0.0;
            foreach (ElementId id in selectedIds)
            {
                FamilyInstance insttmp = doc.GetElement(id) as FamilyInstance;
                if (insttmp == null)
                {
                    return false;
                }
                FamilySymbol symtmp = insttmp.Symbol;
                string shoubunrui = ClsRevitUtil.GetTypeParameterString(symtmp, "小分類");
                string kigou = ClsRevitUtil.GetTypeParameterString(symtmp, "記号");

                if (targetShoubunrui != shoubunrui)
                {
                    MessageBox.Show("異なる種類の鋼材が選択されています。");
                    return false;
                }

                if (targetKigou != kigou)
                {
                    MessageBox.Show("異なる種類の鋼材が選択されています。");
                    return false;
                }

                double selectedKouzaiLength =ClsRevitUtil.CovertFromAPI (ClsRevitUtil.GetTypeParameter(symtmp, "長さ"));
                if (ClsGeo.GEO_GE0(selectedKouzaiLength))
                {
                    length += selectedKouzaiLength;
                }
            }

            DLG.DlgChangeLength dlg = new DLG.DlgChangeLength(selectedIds.Count,length);
            DialogResult result = dlg.ShowDialog();
            if (result != DialogResult.OK)
            {
                return false;
            }

            DLG.DlgChangeLength.EMode mode = dlg.GetMode();
            if (mode == DLG.DlgChangeLength.EMode.Edit)
            {
                //変更
                double newLength = dlg.GetLength();  //注意 長さはmm
                XYZ newStartPoint = new XYZ();
                XYZ newEndPoint = new XYZ();

                if (!ClsWarituke.GetStartEndPoint(doc, selectedIds, ref newStartPoint, ref newEndPoint))
                {
                    MessageBox.Show("座標位置の取得に失敗しました。");
                    return false;
                }

                newEndPoint = ClsRevitUtil.MoveCoordinates(newStartPoint, newEndPoint, ClsRevitUtil.ConvertDoubleGeo2Revit(newLength));
                double lengthM = ClsGeo.mm2m(newLength);
                string strLen = lengthM.ToString("F1");

                string familyPath = Master.ClsYamadomeCsv.GetFamilyPath(targetKigou, lengthM);
                string symbolName = ClsRevitUtil.GetFamilyName(familyPath);
                if (!ClsRevitUtil.LoadFamilyData(doc, familyPath, out Family family))
                {
                    MessageBox.Show("ファミリの取得に失敗しました");
                    return false;
                }

                FamilySymbol kouzaiSym = ClsRevitUtil.GetFamilySymbol(doc, symbolName, targetTypeName);


                double offset = ClsRevitUtil.GetParameterDouble(doc, selectedIds[0], "ホストからのオフセット");

                using (Transaction t = new Transaction(doc, "選択鋼材削除"))
                {
                    t.Start();
                    ClsRevitUtil.Delete(doc, selectedIds);
                    t.Commit();
                }

                using (Transaction t = new Transaction(doc, "鋼材変更"))
                {
                    t.Start();
                    ElementId createdId = ClsRevitUtil.Create(doc, newStartPoint, levelid, kouzaiSym);
                    ClsRevitUtil.SetParameter(doc, createdId, "ホストからのオフセット", offset);

                    // シンボルを回転
                    Line rotationAxis = Line.CreateBound(newStartPoint, newStartPoint + XYZ.BasisZ);    // Z軸周りに回転
                    XYZ vec = (newEndPoint - newStartPoint).Normalize();
                    double radians = XYZ.BasisX.AngleOnPlaneTo(vec, XYZ.BasisZ);
                    ElementTransformUtils.RotateElement(doc, createdId, rotationAxis, radians);
                    t.Commit();

                }

            }
            else if (mode == DLG.DlgChangeLength.EMode.Partition)
            {
                //分割
                double div1 = dlg.GetDiv1Length(); //m
                double div2 = dlg.GetDiv2Length(); //m


                XYZ newStartPoint = new XYZ();//一本目の始点
                XYZ newEndPoint2 = new XYZ();//一本目の終点　二点目の始点
                XYZ newEndPoint = new XYZ();//二本目の終点


                if (!ClsWarituke.GetStartEndPoint(doc, selectedIds, ref newStartPoint, ref newEndPoint))
                {
                    MessageBox.Show("座標位置の取得に失敗しました。");
                    return false;
                }

                newEndPoint2 = ClsRevitUtil.MoveCoordinates(newStartPoint, newEndPoint, ClsRevitUtil.ConvertDoubleGeo2Revit(ClsGeo.m2mm(div1)));
                newEndPoint = ClsRevitUtil.MoveCoordinates(newEndPoint2, newEndPoint, ClsRevitUtil.ConvertDoubleGeo2Revit(ClsGeo.m2mm(div2)));

                string familyPath1 = Master.ClsYamadomeCsv.GetFamilyPath(targetKigou, div1);
                string symbolName1 = ClsRevitUtil.GetFamilyName(familyPath1);
                if (!ClsRevitUtil.LoadFamilyData(doc, familyPath1, out Family family1))
                {
                    MessageBox.Show("ファミリの取得に失敗しました");
                    return false;
                }

                FamilySymbol kouzaiSym1 = ClsRevitUtil.GetFamilySymbol(doc, symbolName1, targetTypeName);

                string familyPath2 = Master.ClsYamadomeCsv.GetFamilyPath(targetKigou, div2);
                string symbolName2= ClsRevitUtil.GetFamilyName(familyPath2);
                if (!ClsRevitUtil.LoadFamilyData(doc, familyPath2,  out Family family2))
                {
                    MessageBox.Show("ファミリの取得に失敗しました");
                    return false;
                }

                FamilySymbol kouzaiSym2 = ClsRevitUtil.GetFamilySymbol(doc, symbolName2, targetTypeName);

                double offset = ClsRevitUtil.GetParameterDouble(doc, selectedIds[0], "ホストからのオフセット");

                using (Transaction t = new Transaction(doc, "選択鋼材削除"))
                {
                    t.Start();
                    ClsRevitUtil.Delete(doc, selectedIds);
                    t.Commit();
                }

                using (Transaction t = new Transaction(doc, "鋼材分割作成"))
                {
                    t.Start();
                    ElementId createdId1 = ClsRevitUtil.Create(doc, newStartPoint, levelid, kouzaiSym1);
                    ClsRevitUtil.SetParameter(doc, createdId1, "ホストからのオフセット", offset);

                    // シンボルを回転
                    Line rotationAxis = Line.CreateBound(newStartPoint, newStartPoint + XYZ.BasisZ);    // Z軸周りに回転
                    XYZ vec1 = (newEndPoint2 - newStartPoint).Normalize();
                    double radians1 = XYZ.BasisX.AngleOnPlaneTo(vec1, XYZ.BasisZ);
                    ElementTransformUtils.RotateElement(doc, createdId1, rotationAxis, radians1);
                    t.Commit();

                    t.Start();
                    ElementId createdId2 = ClsRevitUtil.Create(doc, newEndPoint2, levelid, kouzaiSym2);
                    ClsRevitUtil.SetParameter(doc, createdId2, "ホストからのオフセット", offset);

                    // シンボルを回転
                    Line rotationAxis2 = Line.CreateBound(newEndPoint2, newEndPoint2 + XYZ.BasisZ);    // Z軸周りに回転
                    XYZ vec2 = (newEndPoint - newEndPoint2).Normalize();
                    double radians2 = XYZ.BasisX.AngleOnPlaneTo(vec2, XYZ.BasisZ);
                    ElementTransformUtils.RotateElement(doc, createdId2, rotationAxis2, radians2);
                    t.Commit();

                }

                //カバープレートの処理
                double kigouSize = 400.0;
                string sizeStr = "40";
                string kigouSizeStr = string.Empty;
                bool coverPlateJikkou = true;
                kigouSizeStr = targetKigou;
                if (kigouSizeStr.Contains("80SMH"))
                {
                    kigouSize = 400.0;
                    sizeStr = "40";
                }
                else if (kigouSizeStr.Contains("60SMH"))
                {
                    kigouSize = 400.0;
                    sizeStr = "40";
                }
                else if (kigouSizeStr.Contains("SMH"))
                {
                    kigouSize = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(kigouSizeStr.Replace("SMH", "")) * 10);
                    sizeStr = kigouSizeStr.Replace("SMH", "");
                }
                else if (kigouSizeStr.Contains("HA"))
                {
                    kigouSize = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(kigouSizeStr.Replace("HA", "")) * 10);
                    sizeStr = kigouSizeStr.Replace("HA", "");
                }
                else
                {
                    coverPlateJikkou = false;
                }

                if (coverPlateJikkou)
                {
                    // ファミリの取得処理
                    string familyPath = string.Empty;
                    familyPath = Master.ClsCoverPlateCSV.GetFamilyPath(kigouSizeStr);
                    string symbolName = ClsRevitUtil.GetFamilyName(familyPath);
                    if (!ClsRevitUtil.LoadFamilyData(doc, familyPath, out Family family))
                    {
                        return false;
                    }
                    FamilySymbol coverSym = ClsRevitUtil.GetFamilySymbol(doc, symbolName, targetTypeName);

                    //#31590
                    //using (Transaction t = new Transaction(doc, "Create First Cover Plate"))
                    //{
                    //    t.Start();

                    //    ElementId createdId = ClsRevitUtil.Create(doc, newEndPoint2, levelid, coverSym);

                    //    // シンボルを回転
                    //    Line rotationAxis = Line.CreateBound(newEndPoint2, newEndPoint2 + XYZ.BasisZ);    // Z軸周りに回転
                    //    XYZ vec = (newEndPoint - newEndPoint2).Normalize();
                    //    double radians = XYZ.BasisX.AngleOnPlaneTo(vec, XYZ.BasisZ);
                    //    ElementTransformUtils.RotateElement(doc, createdId, rotationAxis, radians);

                    //    //if (true)
                    //    //{
                    //    //    //掘削側
                    //    //    if (targetTypeName.Contains("切梁") || targetTypeName.Contains("隅火打"))
                    //    //    {
                    //    //        ClsRevitUtil.SetParameter(doc, createdId, "基準レベルからの高さ", baseoffset - kouzaiSizeHalf - ClsRevitUtil.CovertToAPI((coverPlateThickness / 2)));
                    //    //    }
                    //    //    else if (targetTypeName.Contains("腹起"))
                    //    //    {
                    //    //        ElementTransformUtils.MoveElement(doc, createdId, ((kouzaiSizeHalf) + ClsRevitUtil.CovertToAPI(coverPlateThickness / 2)) * -vecOffset);
                    //    //        ClsRevitUtil.SetParameter(doc, createdId, "基準レベルからの高さ", baseoffset);
                    //    //    }
                    //    //}
                    //    //if (true)
                    //    //{
                    //    //    //壁側
                    //    //    if (targetTypeName.Contains("切梁") || targetTypeName.Contains("隅火打"))
                    //    //    {
                    //    //        ClsRevitUtil.SetParameter(doc, createdId, "基準レベルからの高さ", baseoffset + kouzaiSizeHalf + ClsRevitUtil.CovertToAPI((coverPlateThickness / 2)));
                    //    //    }
                    //    //    else if (targetTypeName.Contains("腹起"))
                    //    //    {
                    //    //        ElementTransformUtils.MoveElement(doc, createdId, (kouzaiSizeHalf + ClsRevitUtil.CovertToAPI(coverPlateThickness / 2)) * vecOffset);
                    //    //        ClsRevitUtil.SetParameter(doc, createdId, "基準レベルからの高さ", baseoffset);
                    //    //    }
                    //    //}
                        
                        

                    //    t.Commit();
                    //}

                }


                //double kouzaiSize = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(steelSizeOrign.Replace("HA", "")) * 10);



            }
            else if (mode == DLG.DlgChangeLength.EMode.Combine)
            {
                //結合
                double combineLength = dlg.GetLength(); //注意 長さはmm
                XYZ newStartPoint = new XYZ();
                XYZ newEndPoint = new XYZ();

                if (!ClsWarituke.GetStartEndPoint(doc, selectedIds, ref newStartPoint, ref newEndPoint))
                {
                    MessageBox.Show("座標位置の取得に失敗しました。");
                    return false;
                }


                double lengthM = ClsGeo.FloorAtDigitAdjust(1, ClsGeo.mm2m(combineLength));

                string  familyPath = Master.ClsYamadomeCsv.GetFamilyPath(targetKigou, lengthM);
                string symbolName = ClsRevitUtil.GetFamilyName(familyPath);
                if (!ClsRevitUtil.LoadFamilyData(doc, familyPath,  out Family family))
                {
                    MessageBox.Show("ファミリの取得に失敗しました");
                    return false;
                }

                FamilySymbol kouzaiSym = ClsRevitUtil.GetFamilySymbol(doc, symbolName, targetTypeName);


                //図面上のカバープレートを取得
                List<ElementId> cpList = ClsCoverPlate.GetAllCoverplate(doc);
                List<ElementId> intersectCplist = new List<ElementId>();
                foreach (ElementId selId in selectedIds)
                {
                    List<ElementId> ids = ClsRevitUtil.GetIntersectFamilys(doc, selId, 0.1, null, cpList);
                    foreach (ElementId id in ids)
                    {
                        if (!intersectCplist.Contains(id))
                        {
                            intersectCplist.Add(id);
                        }
                    }
                }

                List<ElementId> intersectCplist2 = new List<ElementId>();
                foreach (ElementId id in intersectCplist)
                {
                    Element ce = doc.GetElement(id);
                    int hitCnt = 0; 
                    foreach (ElementId selId in selectedIds)
                    {
                        Element se = doc.GetElement(selId);
                        if (ClsRevitUtil.OverlapCheck(doc, se, ce, 0.1))
                        {
                            hitCnt++;              
                        }
                    }
                    if (hitCnt == 2 )
                    {
                        //二個の鋼材と接触してる　= 結合対象の二つの鋼材を結んだカバープレートなので削除対象
                        intersectCplist2.Add(id);
                    }
                }

                double offset = ClsRevitUtil.GetParameterDouble(doc, selectedIds[0], "ホストからのオフセット");

                using (Transaction t = new Transaction(doc, "選択鋼材削除"))
                {
                    t.Start();
                    ClsRevitUtil.Delete(doc, selectedIds);
                    ClsRevitUtil.Delete(doc, intersectCplist2);
                    t.Commit();
                }

                using (Transaction t = new Transaction(doc, "鋼材結合"))
                {
                    t.Start();
                    ElementId createdId = ClsRevitUtil.Create(doc, newStartPoint, levelid, kouzaiSym);
                    ClsRevitUtil.SetParameter(doc, createdId, "ホストからのオフセット", offset);

                    // シンボルを回転
                    Line rotationAxis = Line.CreateBound(newStartPoint, newStartPoint + XYZ.BasisZ);    // Z軸周りに回転
                    XYZ vec = (newEndPoint - newStartPoint).Normalize();
                    double radians = XYZ.BasisX.AngleOnPlaneTo(vec, XYZ.BasisZ);
                    ElementTransformUtils.RotateElement(doc, createdId, rotationAxis, radians);
                    t.Commit();

                }

            }


            return true;
        }
    }
}
