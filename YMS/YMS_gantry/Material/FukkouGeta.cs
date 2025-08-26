using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMS_gantry.Material
{
    [MaterialCategory("覆工桁")]
    public sealed class FukkouGeta :MaterialSuper
    {
        public static new string Name = "Fukkouban";
        public static new string typeName = "覆工桁";

        public FukkouGeta():base()
        {

        }

        public FukkouGeta(ElementId id ,string koudaiName,string material, string size):base(id,koudaiName,material,size)
        {

        }

        public static ElementId CreateOnFace(Document doc ,FukkouGeta fg, XYZ point1,XYZ point2,Reference refer,double rotate,string koudainame)
        {
            ElementId id = ElementId.InvalidElementId;
            FamilySymbol sym;
            string familyPath = Master.ClsFukkouGetaCsv.GetFamilyPath(fg.m_Size);
            if (!GantryUtil.GetFamilySymbol(doc,familyPath,FukkouGeta.typeName, out sym, true))
            {
                return ElementId.InvalidElementId;
            }
            //パラメータ
            //パラメータの組み合わせ作成
            Dictionary<string, string> paramList = new Dictionary<string, string>();
            paramList.Add(DefineUtil.PARAM_MATERIAL, fg.m_Material);
            //paramList.Add("サイズ", fg.m_Size);
            paramList.Add(DefineUtil.PARAM_ROTATE, $"{rotate / (180 / Math.PI)}");
            sym = GantryUtil.DuplicateTypeWithNameRule(doc, koudainame, sym, FukkouGeta.typeName);
            //2点配置
            id = MaterialSuper.PlaceWithTwoPoints(sym, refer, point1, point2, 0,true);
            //sym = GantryUtil.ChangeInstanceTypeID(doc, sym, id, GantryUtil.CreateTypeName(MaterialSuper.typeName, paramList));

            //スロープ時にズレが出るので修正
            FamilyInstance ins = doc.GetElement(id) as FamilyInstance;
            Parameter param = ins.LookupParameter(DefineUtil.PARAM_HOST_OFFSET);
            MaterialSize size = GantryUtil.GetKouzaiSize(fg.m_Size);

            double dist = param.AsDouble();
            double real = RevitUtil.ClsRevitUtil.CovertFromAPI(dist);
            Transform ts = ins.GetTransform();
            double rad = XYZ.BasisZ.AngleTo(ts.BasisZ);
            if (ts.BasisZ.Z>0&&XYZ.BasisZ.Z != ts.BasisZ.Z && rad != 0)
            {
                //平面図から指定した時の点に正しく置かれるように計算しなおして移動
                XYZ or = point1 + ts.BasisZ * 100;
                XYZ oz = or - XYZ.BasisZ * 100;
                XYZ intPnt = GantryUtil.FindPerpendicularIntersection(Line.CreateBound(point1, or), oz);
                XYZ vec = (intPnt - oz).Normalize();

                double height = size.Height;
                if(size.Shape==MaterialShape.C&&rotate!=0&&rotate!=180)
                {
                    height = size.Width;
                }
                double dist2 = (Math.Abs(real)/* + height*/) * Math.Sin(rad);
                ElementTransformUtils.MoveElement(doc, id, vec * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(dist2));
            }

            //タイプパラメータ設定
            foreach (KeyValuePair<string, string> kv in paramList)
            {
                GantryUtil.SetParameterValueByParameterName(doc.GetElement(id) as FamilyInstance, kv.Key, kv.Value);
            }
            //RevitUtil.ClsRevitUtil.SetParameter(doc, id, DefineUtil.PARAM_ROTATE, rotate / (180 / Math.PI));

            double offset = size.Height/2;
            if(size.Shape==MaterialShape.C)
            {
                offset = rotate == 0 ? size.Width : (rotate == 90 ? size.Height / 2 : size.Thick);
            }

            //高さ
            RevitUtil.ClsRevitUtil.SetParameter(doc, id, DefineUtil.PARAM_HOST_OFFSET, RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(offset));
            fg.m_ElementId = id;
            FukkouGeta.WriteToElement(fg, doc);

            return id;
        }

    }
}
