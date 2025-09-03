using Autodesk.Revit.DB;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace YMS.Parts
{
    public class ClsHojoPiece
    {
        public ElementId m_CreatedId;

        public bool CreateHojoPiece(Document doc, ElementId baseId, XYZ ptSt, XYZ ptEd, ElementId levelId, string size, string baseName, out double buzaiLength, double baseoffset = 0.0)
        {
            buzaiLength = 0.0;

            if (size == null)
            {
                return false;
            }

            // ファミリの取得処理
            string familyPath = string.Empty;
            familyPath = Master.ClsSupportPieceCsv.GetFamilyPath(size);
            string symbolName = ClsRevitUtil.GetFamilyName(familyPath);
            if (!ClsRevitUtil.LoadFamilyData(doc, familyPath,  out Family family))
            {
                return false;
            }
            FamilySymbol kouzaiSym = ClsRevitUtil.GetFamilySymbol(doc, symbolName, baseName);

            // ファミリの配置処理
            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                t.Start();

                ElementId createdId = null;
                XYZ vec = (ptEd - ptSt).Normalize();
                if (doc.GetElement(levelId) is ReferencePlane referencePlane)
                {
                    createdId = ClsRevitUtil.Create(doc, ptSt, vec, referencePlane, kouzaiSym);
                    ClsRevitUtil.SetParameter(doc, createdId, "集計レベル", ClsRevitUtil.GetParameterElementId(doc, baseId, "集計レベル"));
                    //baseoffset += ClsRevitUtil.GetParameterDouble(doc, createdId, "ホストからのオフセット");
                    //ClsRevitUtil.SetParameter(doc, createdId, "ホストからのオフセット", baseoffset);
                }
                else
                {
                    createdId = ClsRevitUtil.Create(doc, ptSt, levelId, kouzaiSym);
                    
                    // シンボルを回転
                    Line rotationAxis = Line.CreateBound(ptSt, ptSt + XYZ.BasisZ);    // Z軸周りに回転
                    double radians = XYZ.BasisX.AngleOnPlaneTo(vec, XYZ.BasisZ);
                    ElementTransformUtils.RotateElement(doc, createdId, rotationAxis, radians);
                    if (!ClsRevitUtil.SetParameter(doc, createdId, "基準レベルからの高さ", baseoffset))
                        ClsRevitUtil.SetParameter(doc, createdId, "ホストからのオフセット", baseoffset);
                }
                

                m_CreatedId = createdId;

                t.Commit();
            }

            // ハイフンを使って文字列を分割
            string[] parts = size.Split('-');
            string lastPart = parts[parts.Length - 1]; // 最後の部分を取得

            // 長さの単位を取り除いて数値に変換し、100 倍して数値化
            double length = double.Parse(lastPart) * 100;

            buzaiLength = length;

            return true;
        }

    }
}
