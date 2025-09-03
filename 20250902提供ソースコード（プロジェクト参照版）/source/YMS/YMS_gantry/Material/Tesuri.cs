using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMS_gantry.Material
{
    [MaterialCategory("手摺")]
    public sealed class Tesuri : MaterialSuper
    {

        public static new string typeName = "手摺";
        public Tesuri() : base()
        {

        }

        public Tesuri(ElementId id, string koudaname, string size, string material) : base(id, koudaname, size, material)
        {

        }

        public ElementId CreateTesuri(Document doc,string koudaiName, Curve curve, string size, string material,Reference refer,double offset,double rotate,FamilySymbol orSym=null)
        {
            ElementId retId = ElementId.InvalidElementId;
            try
            {
                FamilySymbol sym=orSym;
                if(orSym==null)
                {
                    string familyPath = Master.ClsTesuriCsv.GetFamilyPath(size, Master.ClsTesuriCsv.TypeTesuri);
                    if (!GantryUtil.GetFamilySymbol(doc, familyPath, Master.ClsTesuriCsv.TypeTesuri, out sym, true))
                    {
                        return ElementId.InvalidElementId;
                    }
                    sym = GantryUtil.DuplicateTypeWithNameRule(doc, koudaiName, sym, Master.ClsTesuriCsv.TypeTesuri);
                }
                

                //パラメータの組み合わせ作成
                Dictionary<string, string> paramList = new Dictionary<string, string>();
                paramList.Add(DefineUtil.PARAM_MATERIAL, material);
                paramList.Add(DefineUtil.PARAM_LENGTH, $"{curve.GetEndPoint(0).DistanceTo(curve.GetEndPoint(1))}");
                using (SubTransaction tr = new SubTransaction(doc))
                {
                    tr.Start();
                    ElementId CreatedID = MaterialSuper.PlaceWithTwoPoints(sym,refer,curve.GetEndPoint(0),curve.GetEndPoint(1),offset);
                    //sym = RevitUtil.ClsRevitUtil.ChangeTypeID(doc, sym, CreatedID, GantryUtil.CreateTypeName(size, paramList));
                    RevitUtil.ClsRevitUtil.SetParameter(doc, CreatedID,"回転", rotate / (180 / Math.PI));
                    //タイプパラメータ設定
                    foreach (KeyValuePair<string, string> kv in paramList)
                    {
                        GantryUtil.SetParameterValueByParameterName(doc.GetElement(CreatedID) as FamilyInstance, kv.Key, kv.Value);
                    }
                    retId = CreatedID;
                    tr.Commit();
                }
                return retId;
            }
            catch (Exception ex)
            {
                retId = ElementId.InvalidElementId;
            }
            return retId;
        }
    }
}
