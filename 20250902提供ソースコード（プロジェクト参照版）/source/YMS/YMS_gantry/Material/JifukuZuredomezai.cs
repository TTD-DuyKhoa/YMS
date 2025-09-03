using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMS_gantry.Material
{
    [MaterialCategory("地覆・覆工板ズレ止め材")]
    public sealed class JifukuZuredomezai : MaterialSuper
    {
        public const string typeJifukuName = "地覆";
        public const string typeZuredomeName = "覆工板ｽﾞﾚ止め";

        public JifukuZuredomezai():base()
        {

        }

        public JifukuZuredomezai(string koudainame,string size, string material,ElementId id):base(id,koudainame,material,size)
        {

        }

        public ElementId CreateJifuku(JifukuZuredomezai jifuku,Document doc,string type,Reference refer,XYZ point1,XYZ point2,double offset=0, double rotate = 0, bool adjust=false)
        {
            ElementId retId = ElementId.InvalidElementId;
            try
            {
                string familyPath = Master.ClsJifukuZuredomeCsv.GetFamilyPath(type,jifuku.m_Size);
                FamilySymbol sym;
                if (!GantryUtil.GetFamilySymbol(doc, familyPath, type, out sym, true))
                {
                    return ElementId.InvalidElementId;
                }
                if(jifuku.m_Size.StartsWith("["))
                {
                    sym = GantryUtil.DuplicateTypeWithNameRule(doc, jifuku.m_KodaiName, sym, type);
                }
                Dictionary<string, string> paramList = new Dictionary<string, string>();
                paramList.Add(DefineUtil.PARAM_MATERIAL, jifuku.m_Material);
                paramList.Add(DefineUtil.PARAM_ROTATE, $"{rotate / (180 / Math.PI)}");

                if(jifuku.m_Size.StartsWith("["))
                {
                    retId = MaterialSuper.PlaceWithTwoPoints(sym, refer, point1, point2, offset, true);
                }
                else
                {
                    XYZ mid = point1 + (point2-point1) * 0.5;
                    var ins = GantryUtil.CreateInstanceWith1point(doc, mid, refer, sym, (point1 - point2).Normalize());
                    if (ins != null) { retId = ins.Id; }
                }

                if (retId!=ElementId.InvalidElementId)
                {
                    //sym = GantryUtil.ChangeInstanceTypeID(doc, sym, retId, GantryUtil.CreateTypeName(type, paramList));
                    if(adjust)
                    {
                        //スロープ時にズレが出るので修正
                        FamilyInstance ins = doc.GetElement(retId) as FamilyInstance;
                        Parameter param = ins.LookupParameter(DefineUtil.PARAM_HOST_OFFSET);
                        double dist = param.AsDouble();
                        double real = RevitUtil.ClsRevitUtil.CovertFromAPI(dist);
                        Transform ts = ins.GetTransform();
                        double rad = XYZ.BasisZ.AngleTo(ts.BasisZ);
                        if (ts.BasisZ.Z > 0 && XYZ.BasisZ.Z != ts.BasisZ.Z && rad != 0)
                        {
                            //平面図から指定した時の点に正しく置かれるように計算しなおして移動
                            XYZ or = point1 + ts.BasisZ * 100;
                            XYZ oz = or - XYZ.BasisZ * 100;
                            XYZ intPnt = GantryUtil.FindPerpendicularIntersection(Line.CreateBound(point1, or), oz);
                            XYZ vec = (intPnt - oz).Normalize();

                            MaterialSize size = GantryUtil.GetKouzaiSize(jifuku.m_Size);
                            double height = size.Height;
                            if (size.Shape == MaterialShape.C && rotate != 0 && rotate != 180)
                            {
                                height = size.Width;
                            }
                            double dist2 = (Math.Abs(real)/* + height*/) * Math.Sin(rad);
                            ElementTransformUtils.MoveElement(doc, retId, vec * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(dist2));
                        }
                    }
                    //高さ
                    RevitUtil.ClsRevitUtil.SetParameter(doc, retId, DefineUtil.PARAM_HOST_OFFSET, RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(offset));
                    //タイプパラメータ設定
                    foreach (KeyValuePair<string, string> kv in paramList)
                    {
                        GantryUtil.SetParameterValueByParameterName(doc.GetElement(retId) as FamilyInstance, kv.Key, kv.Value);
                    }
                    jifuku.m_ElementId = retId;
                    JifukuZuredomezai.WriteToElement(jifuku, doc);
                }
            }
            catch (Exception ex)
            {
                retId = ElementId.InvalidElementId;
            }
            return retId;
        }

    }
}
