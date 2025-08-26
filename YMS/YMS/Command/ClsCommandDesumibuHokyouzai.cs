using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YMS.Parts;

namespace YMS.Command
{
    public class ClsCommandDesumibuHokyouzai
    {
        public static void CommandCreateDesumibuHokyouzai(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            //ドキュメントを取得
            Document doc = uidoc.Document;

            //クラスを作成
            ClsDesumibuHokyouzai clsDesumibuHokyouzai = new ClsDesumibuHokyouzai();
            ElementId id = null, id1 = null, id2 = null;

            if (!ClsOyakui.PickBaseObject(uidoc, ref id, "出隅コーナーの杭を指定"))
            {
                return;
            }
            FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
            XYZ dir = inst.HandOrientation;
            (XYZ point, double dEx, double dFx) = GetPointAndFlange(doc, id);
            XYZ pointS = new XYZ(point.X - (dir.X * dFx) + (dir.Y * dEx),
                                 point.Y - (dir.Y * dFx) - (dir.X * dEx), 
                                 point.Z);
            XYZ pointE = new XYZ(point.X + (dir.X * dFx) + (dir.Y * dEx), 
                                 point.Y + (dir.Y * dFx) - (dir.X * dEx),
                                 point.Z);
            double sumikiriLength = dFx * 2;
            Curve cv = Line.CreateBound(pointS, pointE);

            if (!ClsOyakui.PickBaseObject(uidoc, ref id1, "配置点の杭1つ目を選択"))
            {
                return;
            }
            FamilyInstance inst1 = doc.GetElement(id1) as FamilyInstance;
            XYZ dir1 = -inst1.HandOrientation;
            (XYZ point1, double dEx1, double dFx1) = GetPointAndFlange(doc, id1);
            XYZ pointS1 = new XYZ(point1.X - (dir1.X * dFx1) + (dir1.Y * dEx1), 
                                  point1.Y - (dir1.Y * dFx1) - (dir1.X * dEx1), 
                                  point1.Z);
            XYZ pointE1 = new XYZ(point1.X + (dir1.X * dFx1) + (dir1.Y * dEx1),
                                  point1.Y + (dir1.Y * dFx1) - (dir1.X * dEx1),
                                  point1.Z);
            
            if (!ClsOyakui.PickBaseObject(uidoc, ref id2, "配置点の杭2つ目を選択"))
            {
                return;
            }
            FamilyInstance inst2 = doc.GetElement(id2) as FamilyInstance;
            XYZ dir2 = -inst2.HandOrientation;
            (XYZ point2, double dEx2, double dFx2) = GetPointAndFlange(doc, id2);
            XYZ pointS2 = new XYZ(point2.X - (dir2.X * dFx2) + (dir2.Y * dEx2),
                                 point2.Y - (dir2.Y * dFx2) - (dir2.X * dEx2),
                                 point2.Z);
            XYZ pointE2 = new XYZ(point2.X + (dir2.X * dFx2) + (dir2.Y * dEx2),
                                 point2.Y + (dir2.Y * dFx2) - (dir2.X * dEx2),
                                 point2.Z);

            Line line1, line2;
            if (!ClsRevitUtil.CheckNearGetEndPoint(cv, pointS1) && ClsRevitUtil.CheckNearGetEndPoint(cv, pointE2))
            {
                line1 = Line.CreateBound(pointS1, pointS);
                line2 = Line.CreateBound(pointE2, pointE);
                
            }
            else
            {
                line2 = Line.CreateBound(pointE1, pointE);
                line1 = Line.CreateBound(pointS2, pointS);
            }
            double length1 = line1.Length;
            double length2 = line2.Length;
            
            //dir2 = line2.Direction;

            //ダイアログを表示
            YMS.DLG.DlgCreateDesumibuHokyouzai DesumibuHokyouzai = new DLG.DlgCreateDesumibuHokyouzai();
            DialogResult result = DesumibuHokyouzai.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                return;
            }
            clsDesumibuHokyouzai = DesumibuHokyouzai.m_clsDesumibuHokyouzai;

            //指定点から無限に伸ばした直線1
            XYZ perpendicPoint1 = new XYZ(pointS1.X + (ClsRevitUtil.CovertToAPI(int.MaxValue) * dir1.X),
                                       pointS1.Y + (ClsRevitUtil.CovertToAPI(int.MaxValue) * dir1.Y),
                                       pointS1.Z);
            Curve perpendicCv1 = Line.CreateBound(pointS1, perpendicPoint1);
            //指定点から無限に伸ばした直線2
            XYZ perpendicPoint2 = new XYZ(pointE2.X + (ClsRevitUtil.CovertToAPI(int.MaxValue) * -dir2.X),
                                       pointE2.Y + (ClsRevitUtil.CovertToAPI(int.MaxValue) * -dir2.Y),
                                       pointE2.Z);
            Curve perpendicCv2 = Line.CreateBound(pointE2, perpendicPoint2);

            //ファミリの回転角
            double rotation = dir1.AngleOnPlaneTo(XYZ.BasisX, XYZ.BasisZ);

            double anglePi = Math.PI;
            //指定点から基準線に下ろした垂線との交点を取得
            XYZ insecPoint = ClsRevitUtil.GetIntersection(perpendicCv1, perpendicCv2);
            if (insecPoint == null)
            {
                rotation = dir2.AngleOnPlaneTo(XYZ.BasisX, XYZ.BasisZ);
                anglePi = 0.0;
                dir1 *= -1;
                //指定点から無限に伸ばした直線1
                perpendicPoint1 = new XYZ(pointS1.X + (ClsRevitUtil.CovertToAPI(int.MaxValue) * dir1.X),
                                           pointS1.Y + (ClsRevitUtil.CovertToAPI(int.MaxValue) * dir1.Y),
                                           pointS1.Z);
                perpendicCv1 = Line.CreateBound(pointS1, perpendicPoint1);
                //指定点から無限に伸ばした直線2
                perpendicPoint2 = new XYZ(pointE2.X + (ClsRevitUtil.CovertToAPI(int.MaxValue) * dir2.X),
                                           pointE2.Y + (ClsRevitUtil.CovertToAPI(int.MaxValue) * dir2.Y),
                                           pointE2.Z);
                perpendicCv2 = Line.CreateBound(pointE2, perpendicPoint2);
                insecPoint = ClsRevitUtil.GetIntersection(perpendicCv1, perpendicCv2);
                if (insecPoint == null)
                {
                    return;
                }
                //return;
            }

            double voidDep = ClsVoid.GetVoidDep(doc, id1);
            //パラメータに使用する角度
            double angle = dir1.AngleOnPlaneTo(dir2, XYZ.BasisZ);
            if (anglePi != 0.0)
            {
                angle = anglePi - angle;

            }
            
            ElementId levelID = ClsRevitUtil.GetParameterElementId(doc, id, "集計レベル");
            clsDesumibuHokyouzai.CreateDesumibuHokyouzai(doc, levelID, insecPoint, sumikiriLength, length1, length2, voidDep, angle, rotation);
        }

        private static (XYZ, double, double) GetPointAndFlange(Document doc, ElementId id)
        {
            FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
            XYZ point = (inst.Location as LocationPoint).Point;
            string size = Master.ClsHBeamCsv.GetSizePileFamilyPathInName(inst.Symbol.FamilyName);
            string familyPathKui = Master.ClsHBeamCsv.GetPileFamilyPath(size);
            string familyNameKui = ClsRevitUtil.GetFamilyName(familyPathKui);
            string stE = ClsYMSUtil.GetKouzaiSizeSunpou1(familyNameKui, 1);
            double dEx = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(stE) / 2);
            string stF = ClsYMSUtil.GetKouzaiSizeSunpou1(familyNameKui, 2);
            double dFx = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(stF) / 2);

            return (point, dEx, dFx);
        }
    }
}
