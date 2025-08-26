using Autodesk.Revit.DB;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMS.Parts
{
    public class ClsKousyabuMawari
    {
        public static bool CreateKousyabuMawari(Document doc, ElementId levelID, string haraSize, string kiriSize ,XYZ point, double angle)
        {
            //double widthKousya = ClsRevitUtil.CovertToAPI(Master.ClsYamadomeCsv.GetWidth(syuzai));
            //if (syuzai == "なし")
            //    widthKousya = ClsRevitUtil.CovertToAPI(400.0);//異なる場合のファミリの高さによって修正する

            if (kiriSize.Contains("SMH"))
            {
                kiriSize = kiriSize.Replace("SMH", "");
                if (kiriSize.Contains("60"))
                    kiriSize = "40HA";
                else
                    kiriSize += "HA";
            }
            string pathKousya = Master.ClsKousyabuUnit.GetFamilyPath("交叉部", haraSize, kiriSize);
            string familyNameKousya = RevitUtil.ClsRevitUtil.GetFamilyName(pathKousya);

            if (!ClsRevitUtil.LoadFamilySymbolData(doc, pathKousya, familyNameKousya, out FamilySymbol symKousya))
            {
                return false;
            }

            double offset = ClsRevitUtil.CovertToAPI(Master.ClsYamadomeCsv.GetWidth(kiriSize) + (Master.ClsYamadomeCsv.GetWidth(haraSize) - Master.ClsYamadomeCsv.GetWidth(kiriSize)) / 2);
            if (symKousya != null)
            {
                using (Transaction tr = new Transaction(doc, "交叉部の作成"))
                {
                    tr.Start();
                    ElementId CreatedID = ClsRevitUtil.Create(doc, point, levelID, symKousya);
                    ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", offset);
                    Line axis = Line.CreateBound(point, point + XYZ.BasisZ);
                    ClsRevitUtil.RotateElement(doc, CreatedID, axis, angle);

                    tr.Commit();
                }
            }

            //double widthTeiketsu = Master.ClsYamadomeCsv.GetWidth("25HA");
            //string pathTeiketsu = Master.ClsKousyabuUnit.GetFamilyPath("締結部", "25HA");
            //string familyNameTeiketsu = RevitUtil.ClsRevitUtil.GetFamilyName(pathTeiketsu);

            //if (!ClsRevitUtil.RoadFamilySymbolData(doc, pathTeiketsu, familyNameTeiketsu, out FamilySymbol symTeiketsu))
            //{
            //    return false;
            //}
            //if (symTeiketsu != null)
            //{
            //    using (Transaction tr = new Transaction(doc, "締結部の作成"))
            //    {
            //        tr.Start();
            //        ElementId CreatedID = ClsRevitUtil.Create(doc, point, levelID, symTeiketsu);
            //        ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", widthTeiketsu);
            //        Line axis = Line.CreateBound(point, point + XYZ.BasisZ);
            //        //ClsRevitUtil.RotateElement(doc, CreatedID, axis, dAngle);

            //        tr.Commit();
            //    }
            //}

            return true;
        }
        /// <summary>
        /// 配置点ある切梁ベースと接続する腹起ベースの主材サイズを取得
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="insec">配置点</param>
        /// <param name="haraSyuzai">腹起ベースに設定されている主材サイズ</param>
        /// <param name="targetDan">指定の段</param>
        /// <returns>true:判定に必要な主材サイズあり</returns>
        public static bool CheckSyuzaiSize(Document doc, XYZ insec, ElementId levelId ,out string haraSyuzai,string targetDan = "上段")
        {
            haraSyuzai = string.Empty;

            List<ElementId> kiriIdList = ClsKiribariBase.GetAllKiribariBaseList(doc);

            ElementId onKiriId = null;
            foreach(ElementId kiriId in kiriIdList)
            {
                FamilyInstance inst = doc.GetElement(kiriId) as FamilyInstance;
                LocationCurve lCurve = inst.Location as LocationCurve;
                XYZ tmpStPoint = lCurve.Curve.GetEndPoint(0);
                XYZ tmpEdPoint = lCurve.Curve.GetEndPoint(1);
                tmpStPoint = new XYZ(tmpStPoint.X, tmpStPoint.Y, 0.0);
                tmpEdPoint = new XYZ(tmpEdPoint.X, tmpEdPoint.Y, 0.0);
                Curve cv = Line.CreateBound(tmpStPoint, tmpEdPoint);
                ElementId kiriLevel = inst.Host.Id;
                if (ClsRevitUtil.IsPointOnCurve(cv, insec) && kiriLevel == levelId)
                {
                    string dan = ClsRevitUtil.GetParameter(doc, kiriId, "段");
                    if(dan == targetDan)
                    {
                        onKiriId = kiriId;
                        break;
                    }
                }
            }

            if (onKiriId == null)
                return false;

            FamilyInstance onKiriInst = doc.GetElement(onKiriId) as FamilyInstance;
            levelId = onKiriInst.Host.Id;

            List<ElementId> insecHaraIdList = ClsHaraokoshiBase.GetIntersectionBase(doc, onKiriId);
            if (insecHaraIdList.Count == 0)
                return false;

            haraSyuzai = ClsRevitUtil.GetParameter(doc, insecHaraIdList[0], "鋼材サイズ");//主材サイズを持ってくる腹起ベースは1本目にしてある
            if (haraSyuzai == string.Empty)
                return false;
            if(haraSyuzai.Contains("SMH"))
            {
                haraSyuzai = haraSyuzai.Replace("SMH", "");
                if (haraSyuzai.Contains("80"))
                    haraSyuzai = "40HA";
                else
                    haraSyuzai += "HA";
            }

            return true;
        }
    }
}
