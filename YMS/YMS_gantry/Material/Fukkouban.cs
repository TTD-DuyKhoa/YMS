using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YMS_gantry.UI;
using static YMS_gantry.DefineUtil;

namespace YMS_gantry
{
    [MaterialCategory("覆工板")]
    public sealed class Fukkouban : MaterialSuper
    {
        public static new string Name = "Fukkouban";

        [MaterialProperty("橋長No")]
        public int m_kyoutyouNum { get; set; } = int.MinValue;
        [MaterialProperty("幅員No")]
        public int m_hukuinNum { get; set; } = int.MinValue;

        public Fukkouban():base()
        {

        }

        public Fukkouban(ElementId _id, string koudaname,string size,FukkoubanData data, int kyoutyouNum = int.MinValue, int hukuinNum = int.MinValue):base(_id,koudaname,data.FukkouMaterial,size)
        {
            this.m_kyoutyouNum = kyoutyouNum;
            this.m_hukuinNum = hukuinNum;
        }

        /// <summary>
        /// 配置位置算出
        /// </summary>
        /// <returns></returns>
        public static Dictionary<XYZ, string> CalcArrangementPoint(double LengKyoutyou, double LenHukuin, List<double> hukuinPitch, XYZ basePnt, XYZ vecKyoutyou, XYZ vecHukuin)
        {
            Dictionary<XYZ, string> retList = new Dictionary<XYZ, string>();
            int kyoutyouCnt = (int)(Math.Floor(LengKyoutyou) / DefineUtil.ONE_M_AS_MM);
            XYZ baseP = basePnt;
            for (int kC = 0; kC < kyoutyouCnt; kC++)
            {
                baseP = basePnt + vecKyoutyou.Normalize() * (RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(kC * 1000));
                XYZ newP = baseP;
                for (int hC = 0; hC < hukuinPitch.Count; hC++)
                {
                    newP = (hC == 0) ? newP : newP + vecHukuin.Normalize() * (RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(hukuinPitch[hC - 1]));
                    retList.Add(newP, $"{kC},{hC}");
                }
            }
            return retList;
        }

        /// <summary>
        /// 覆工板作図
        /// </summary>
        /// <returns></returns>
        public static ElementId CreateFukkouban(Document doc, string material, XYZ point, eFukkoubanType hType, string size, double rotate, XYZ adjustVec = null, Level level = null, double levelOffset = 0)
        {
            ElementId retId = ElementId.InvalidElementId;
            FamilySymbol sym;

            string familyPath = Master.ClsFukkoubanCsv.GetFamilyPath(size);
            string familyName = Path.GetFileNameWithoutExtension(familyPath);
            if (!GantryUtil.GetFamilySymbol(doc, familyPath, familyName, out sym, true))
            {
                return ElementId.InvalidElementId;
            }

            //パラメータの組み合わせ作成
            Dictionary<string, string> paramList = new Dictionary<string, string>();
            paramList.Add(DefineUtil.PARAM_MATERIAL, material);
            paramList.Add("覆工板サイズ", $"{hType}");

            using (SubTransaction tr = new SubTransaction(doc))
            {
                tr.Start();
                ElementId CreatedID = /*doc.Create.NewFamilyInstance(point, sym, level, StructuralType.NonStructural).Id; */GantryUtil.CreateInstanceWith1point(doc, point, level.GetPlaneReference(), sym).Id;
                RevitUtil.ClsRevitUtil.RotateElement(doc, CreatedID, Line.CreateBound(point, point + XYZ.BasisZ), rotate);
                //sym = GantryUtil.ChangeInstanceTypeID(doc, sym, CreatedID, GantryUtil.CreateTypeName(typeName, paramList));

                if (adjustVec != null && adjustVec != XYZ.Zero)
                {
                    ElementTransformUtils.MoveElement(doc, CreatedID, adjustVec);
                }
                RevitUtil.ClsRevitUtil.SetParameter(doc, CreatedID, DefineUtil.PARAM_BASE_OFFSET, RevitUtil.ClsRevitUtil.CovertToAPI(levelOffset));

                retId = CreatedID;
                tr.Commit();
            }
            return retId;
        }

        /// <summary>
        /// 面に覆工板を配置する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="material"></param>
        /// <param name="point"></param>
        /// <param name="hType"></param>
        /// <param name="size"></param>
        /// <param name="rotate"></param>
        /// <param name="refer"></param>
        /// <param name="adjustVec"></param>
        /// <returns></returns>
        public static ElementId CreateFukkoubanOnFace(Document doc, string material, XYZ point, eFukkoubanType hType, string size, double rotate, Reference refer, XYZ norm = null, XYZ adjustVec = null)
        {
            ElementId retId = ElementId.InvalidElementId;
            FamilySymbol sym;
            string familyPath = Master.ClsFukkoubanCsv.GetFamilyPath(size);
            string familyName = Path.GetFileNameWithoutExtension(familyPath);
            if (!GantryUtil.GetFamilySymbol(doc ,familyPath, familyName, out sym, true))
            {
                return ElementId.InvalidElementId;
            }

            //パラメータの組み合わせ作成
            Dictionary<string, string> paramList = new Dictionary<string, string>();
            paramList.Add(DefineUtil.PARAM_MATERIAL, material);
            paramList.Add("覆工板サイズ", $"{hType}");

            using (SubTransaction tr = new SubTransaction(doc))
            {
                tr.Start();
                FamilyInstance ins= GantryUtil.CreateInstanceWith1point(doc, point,refer, sym,norm);
                ElementId CreatedID = ins.Id;             
                //sym = GantryUtil.ChangeInstanceTypeID(doc, sym, CreatedID, GantryUtil.CreateTypeName(typeName, paramList));
                if (adjustVec != null && adjustVec != XYZ.Zero)
                {
                    ElementTransformUtils.MoveElement(doc, CreatedID, adjustVec);
                }

                //スロープ時にズレが出るので修正
                Parameter param = ins.LookupParameter(DefineUtil.PARAM_HOST_OFFSET);
                double dist = param.AsDouble();
                double real = RevitUtil.ClsRevitUtil.CovertFromAPI(dist);
                Transform ts = ins.GetTransform();
                double rad =XYZ.BasisZ.AngleTo(ts.BasisZ);
                if (XYZ.BasisZ.Z!=ts.BasisZ.Z&&rad!=0)
                {
                    //平面図から指定した時の点に正しく置かれるように計算しなおして移動
                    XYZ or = point+ts.BasisZ*100;
                    XYZ oz = or -XYZ.BasisZ * 100;
                    XYZ intPnt = GantryUtil.FindPerpendicularIntersection(Line.CreateBound(point, or), oz);
                    XYZ vec = (intPnt - oz).Normalize();

                    double dist2 = (Math.Abs(real)+DefineUtil.FukkouBAN_THICK) * Math.Sin(rad);
                    ElementTransformUtils.MoveElement(doc, CreatedID, vec * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(dist2));
                }

                //高さは常に0
                RevitUtil.ClsRevitUtil.SetParameter(doc, CreatedID, DefineUtil.PARAM_HOST_OFFSET, 0);
                retId = CreatedID;
                tr.Commit();
            }
            return retId;
        }
       
        /// <summary>
        /// 基準レベルを指定した際の覆工板のオフセット値
        /// </summary>
        /// <param name="baseLevel"></param>
        /// <param name="bLevel"></param>
        /// <param name="offset"></param>
        /// <param name="OhbikiH"></param>
        /// <param name="NedaH"></param>
        /// <returns></returns>
        public static double CalcLevelOffsetLevel(Level baseLevel, eBaseLevel bLevel, double offset, double OhbikiH, double NedaH, int ohbikiC, bool isHOhbiki)
        {
            double retOffset = 0;
            if (isHOhbiki)
            {
                if (bLevel == eBaseLevel.FukkouTop)
                {
                    retOffset = offset - DefineUtil.FukkouBAN_THICK;
                }
                else
                {
                    retOffset = offset + OhbikiH + NedaH;
                }
            }
            else
            {
                if (bLevel == eBaseLevel.FukkouTop)
                {
                    retOffset = offset - DefineUtil.FukkouBAN_THICK;
                }
                else
                {
                    retOffset = offset + (OhbikiH * ohbikiC) + NedaH;
                }
            }
            return retOffset;
        }
    }

    [Serializable]
    [System.Xml.Serialization.XmlRoot("FukkoubanData")]
    public class FukkoubanData
    {
        /// <summary>
        /// 材質
        /// </summary>
        [System.Xml.Serialization.XmlElement("FukkouMaterial")]
        public string FukkouMaterial { get; set; }

        /// <summary>
        /// 覆工板サイズ 2m 3m 可変
        /// </summary>
        [System.Xml.Serialization.XmlElement("FukkoubanSize")]
        public string FukkoubanSize { get; set; }

        public FukkoubanData()
        {
            FukkouMaterial = "SS400";
            FukkoubanSize ="";
        }

    }

}
