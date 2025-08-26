using Autodesk.Revit.DB;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static YMS.DLG.DlgWaritsuke;
using YMS.DLG;

namespace YMS.Parts
{
    public class ClsMegaBeam
    {

        public ElementId m_CreatedId;

        public bool CreateMegaBeam(Document doc, XYZ ptSt, XYZ ptEd, ElementId levelId, XYZ vecMegabeam, double baseoffset = 0.0)
        {
            // ファミリの取得処理
            string familyPath = string.Empty;
            familyPath = Master.ClsYamadomeCsv.GetFamilyPath("80SMH", (double)9);
            string symbolName = ClsRevitUtil.GetFamilyName(familyPath);
            if (!ClsRevitUtil.LoadFamilyData(doc, familyPath,  out Family family))
            {
                return false;
            }
            FamilySymbol kouzaiSym = ClsRevitUtil.GetFamilySymbol(doc, symbolName, "腹起");

            // ファミリの配置処理
            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                t.Start();

                ElementId createdId = ClsRevitUtil.Create(doc, ptSt, levelId, kouzaiSym);
                m_CreatedId = createdId;
                ClsRevitUtil.SetParameter(doc, createdId, "基準レベルからの高さ", baseoffset);

                // シンボルを回転
                Line rotationAxis = Line.CreateBound(ptSt, ptSt + XYZ.BasisZ);    // Z軸周りに回転
                XYZ vec = (ptEd - ptSt).Normalize();
                double radians = XYZ.BasisX.AngleOnPlaneTo(vec, XYZ.BasisZ);
                ElementTransformUtils.RotateElement(doc, createdId, rotationAxis, radians);

                // 位置を移動
                ElementTransformUtils.MoveElement(doc, createdId, (ClsRevitUtil.CovertToAPI(200)) * vecMegabeam);

                t.Commit();
            }

            return true;
        }
    }
}
