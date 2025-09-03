#region Namespaces
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using YMS.Command;
using YMS.DLG;
using YMS.Parts;
//using static Autodesk.Revit.DB.SpecTypeId;
using Autodesk.Revit.UI.Events;
#endregion

namespace YMS
{
    [Transaction(TransactionMode.Manual)]
    public class ClsCommand : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // Access current selection

            Selection sel = uidoc.Selection;

            // Retrieve elements from database

            FilteredElementCollector col
              = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .OfCategory(BuiltInCategory.INVALID)
                .OfClass(typeof(Wall));

            // Filtered element collector is iterable

            foreach (Element e in col)
            {
                Debug.Print(e.Name);
            }

            // Modify document within a transaction

            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Transaction Name");
                tx.Commit();
            }

            return Result.Succeeded;
        }
    }
    /// <summary>
    /// React to elevation view creation subscribing to DocumentChanged event
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    class CmdElevationWatcher : IExternalCommand
    {
        internal static Guid SchemaId { get; } = new Guid("720080CB-DA99-40DC-9415-E53F28099990");
        internal static string SchemaName { get; } = "ModelLineSchemaName";
        internal static string FieldName { get; } = "ModelLineeId";
        internal static string VendorId { get; } = "fresco.co.jp";

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var app = uiapp.Application;
            var doc = uidoc.Document;

            while (true)
            {
                Wall pickedWall = null;
                Element picLine = null;
                do
                {
                    try
                    {
                        //Reference reference = uidoc.Selection.PickObject(ObjectType.Element);
                        //pickedWall = doc.GetElement(reference) as Wall;
                        (pickedWall, picLine) = ClsTest.CreateWallSelectLine(uidoc);
                    }
                    catch
                    {
                        return Result.Succeeded;
                    }
                    if (pickedWall == null)
                    {
                        return Result.Succeeded;
                    }
                } while (pickedWall == null);

                using (var transaction = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                {
                    transaction.Start();

                    // �ǂ̒��_�𒆐S�AZ����@���Ƃ���~�����f�������ō쐬���A
                    // Id ��ǂ̊g���X�g���[�W�ɕۑ�����
                    //var (plane, line) = CreateLine(pickedWall);
                    //var sketchPlane = SketchPlane.Create(doc, plane);
                    ////var modelLine = doc.Create.NewModelCurve(line, sketchPlane);

                    //Curve cvShin = (picLine.Location as LocationCurve).Curve;
                    //var modelLine = doc.Create.NewModelCurve(cvShin, sketchPlane);

                    // �g���X�g���[�W�ɕR�Â�����ۑ�
                    // https://help.autodesk.com/view/RVT/2022/ENU/?guid=Revit_API_Revit_API_Developers_Guide_Advanced_Topics_Storing_Data_in_the_Revit_model_Extensible_Storage_html
                    // https://qiita.com/fallaf/items/0422cee4d38fd9993816
                    using (var schema = CreateSchema())
                    {
                        using (var entity = new Entity(schema))
                        {
                            // set the value for this entity
                            entity.Set<ElementId>(FieldName, picLine.Id);
                            pickedWall.SetEntity(entity); // store the entity in the element
                            //���c�ɂ�����ێ�������
                            entity.Set<ElementId>(FieldName, pickedWall.Id);
                            picLine.SetEntity(entity);
                        }
                    }
                    transaction.Commit();
                }
            }
        }

        //static double mm2feet(double mm) => mm * 0.00328084;

        internal static (Plane, Curve) CreateLine(Wall pickedWall)
        {
            var locationCurve = pickedWall.Location as LocationCurve;
            if (locationCurve == null) { return default; }
            var start = locationCurve.Curve.GetEndPoint(0);//.Reference.GlobalPoint;
            var end = locationCurve.Curve.GetEndPoint(1);
            var mid = 0.5 * (start + end);

            var plane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, mid);
            //var circle = Arc.Create(plane, mm2feet(700.0), 0, 2.0 * Math.PI);

            Curve cv = Line.CreateBound(start, end);

            return (plane, cv);

        }

        internal static (Plane, XYZ) CreateWall(ModelLine pickedLine)
        {
            var locationCurve = pickedLine.Location as LocationCurve;
            if (locationCurve == null) { return default; }
            var start = locationCurve.Curve.GetEndPoint(0);//.Reference.GlobalPoint;
            var end = locationCurve.Curve.GetEndPoint(1);
            var mid = 0.5 * (start + end);
            //var mp = (end - start) * 0.5;

            var plane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, mid);
            //var circle = Arc.Create(plane, mm2feet(700.0), 0, 2.0 * Math.PI);

            Curve cv = Line.CreateBound(start, end);
            //var wall = Wall.Create()
            return (plane, mid);
        }

        static Schema CreateSchema()
        {
            var existedSchema = Schema.Lookup(SchemaId);
            if (existedSchema != null) { return existedSchema; }

            using (var schemaBuilder = new SchemaBuilder(SchemaId))
            {
                //Public�ɂ��Ȃ���Autodesk.Revit.Exceptions InvalidOperationException����
                schemaBuilder.SetReadAccessLevel(AccessLevel.Public); // allow anyone to read the object
                schemaBuilder.SetWriteAccessLevel(AccessLevel.Public); // restrict writing to this vendor only
                schemaBuilder.SetVendorId(VendorId); // required because of restricted write-access
                schemaBuilder.SetSchemaName(SchemaName);
                schemaBuilder.AddSimpleField(FieldName, typeof(ElementId));

                return schemaBuilder.Finish(); // return the Schema object
            }
        }
    }//CmdElevationWatcher

    [Transaction(TransactionMode.Manual)]
    public class Rubberband : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uiapp.ActiveUIDocument.Document;


            Reference r = uidoc.Selection.PickObject(ObjectType.Element);
            ElementId targetId = r.ElementId;
            List<ElementId> addIds = new List<ElementId>();
            ClsRevitUtil.CheckCollision2(doc, targetId, ref addIds);


            //List<ElementId> addIds = new List<ElementId>();

            //foreach (ElementId id in ClsRevitUtil.GetAllCreatedFamilyInstanceList(doc))
            //{
            //    if (id == targetId)
            //    {
            //        continue;
            //    }

            //    if (ClsYMSUtil.CheckCollision(doc, targetId, id) > 0)
            //    {
            //        addIds.Add(id);
            //    }
            //}

            ClsRevitUtil.SelectElement(uidoc, addIds);

            //foreach (ElementId id in addIds)
            //{
            //     ClsRevitUtil.SelectElement(uidoc, addIds);
            //   // ClsRevitUtil.ChangeLineColor(doc, id, ClsGlobal.m_redColor);
            //}


            return Result.Succeeded;
        }
    }
    [Transaction(TransactionMode.Manual)]
    public class CommandTest : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                YMS.DLG.spForm sf = new DLG.spForm();
                sf.Show();
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Info Message1", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }
            return Result.Succeeded;
        }
    }
    //TESTCommand
    [Transaction(TransactionMode.Manual)]
    public class TEST1 : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;

                ElementId id = uidoc.Selection.PickObject(ObjectType.Element, "�x�[�X��I�����Ă�������").ElementId;
                ClsYMSUtil.CreateBaseElevationView(doc, id);
                ClsYMSUtil.ChangeView(uidoc, "TEST");
            }
            catch (Exception ex)
            {
                TaskDialog.Show("TEST1", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }
            return Result.Succeeded;
        }
    }
    //TESTCommand
    [Transaction(TransactionMode.Manual)]
    public class TEST2 : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;

                ClsJosn.CreateBase(doc);
                //ClsJosn.CreateKabe(doc);
                ClsJosn.CreateTanakui(doc);

                //TaskDialog.Show("�ʔz�u�}�X�^�쐬�J�n", "");

                //ClsTest stt = new ClsTest();

                //stt.CommandTest_Kurane2(uiapp);

                //TEST1
                //ElementId id = uidoc.Selection.PickObject(ObjectType.Element, "�x�[�X��I�����Ă�������").ElementId;
                //List<ElementId> ids = new List<ElementId>();
                //ids.Add(id);
                //uidoc.Selection.SetElementIds(ids);
                //// Revit�̃��[�N�Z�b�g�_�C�A���O��\�����邽�߂̃R�}���hID���擾
                //RevitCommandId cmdId = RevitCommandId.LookupPostableCommandId(PostableCommand.SetWorkPlane);//PostableCommand.SetWorkPlane����͐}�ʏ�̍�ƃ��x����ݒ肷��̂ł����ăt�@�~���̍�ƖʕҏW�ɂ͑Ή����Ă��Ȃ�
                ////RevitCommandId cmdId = RevitCommandId.LookupCommandId("��Ɩʂ�ҏW");
                //if (cmdId == null)
                //{
                //    return Result.Failed;
                //}
                //// �R�}���h�����s���ă_�C�A���O��\��
                //if (uiapp.CanPostCommand(cmdId))
                //{
                //    uiapp.PostCommand(cmdId);
                //}
                //TEST1
            }
            catch (Exception ex)
            {
                TaskDialog.Show("TEST2", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }
            return Result.Succeeded;
        }
    }
    //TESTCommand
    [Transaction(TransactionMode.Manual)]
    public class TEST3 : IExternalCommand
    {
        public static Result CreateBase(UIApplication uiapp, UIDocument uidoc, Document doc)
        {

            try
            {

                var keyCenter1 = "��]��1";
                var keyCenter2 = "��]��2";
                var keyEdge1 = "���|�ޒ[�_";
                var keyEdge2 = "���|�ޒ[�_2";
                var keyTheta0 = "��0";
                var keyTheta1 = "��1";
                var keyTheta2 = "��2";
                var keyTheta3 = "�ݒu�p�x";

                var num = 2;
                var pieceInstances = new FamilyInstance[num];
                var origin = new XYZ[num];//�t�@�~����_
                var basisX = new XYZ[num];//�t�@�~�� X���
                var basisY = new XYZ[num];//�t�@�~�� Y���
                var basisZ = new XYZ[num];//�t�@�~�� Z���
                var center1Set = new XYZ[num];// ��]��1 �̍��W
                var center2Set = new XYZ[num];// ��]��2 �̍��W
                var edge1Set = new XYZ[num];// ���|�ޒ[�_ �̍��W
                var edge2Set = new XYZ[num];// ���|�ޒ[�_2 �̍��W
                var l3 = new double[num];// �t�@�~����_���� V3 �ւ̋���
                var l2 = new double[num];// V3 �����]��2�ւ̋���
                var l1Set = new double[num];// ��]��2�����]��1�ւ̋���
                var l0Set = new double[num];// ��]��1���牼�|�ޒ[�_�ւ̋���
                var v3 = new XYZ[num];//V3 : V2��ZX���ʂ֎ˉe�����_
                var v2 = new XYZ[num];
                var v1 = new XYZ[num];
                var v0 = new XYZ[num];
                var e0 = new XYZ[num];
                //var v3ToOrigin = new XYZ[num];

                for (int i = 0; i < num; i++)
                {
                    var pickedElem = doc.GetElement(uidoc.Selection.PickObject(ObjectType.Element, "�Η���s�[�X��I�����Ă�������"));
                    var pickedInstance = pickedElem as FamilyInstance;
                    pieceInstances[i] = pickedInstance;

                    var instanceCoodinate = pickedInstance.GetTotalTransform();
                    origin[i] = instanceCoodinate.Origin;
                    basisY[i] = instanceCoodinate.BasisY;

                    var hasTheta3 = ClsRevitUtil.FindParameter(doc, pickedInstance.Id, keyTheta3);
                    var theta3 = hasTheta3 ? ClsRevitUtil.GetParameterDouble(doc, pickedInstance.Id, keyTheta3) : 0.0;
                    var basisRotation = SMatrix3d.Rotation(-basisY[i], XYZ.Zero, theta3);

                    basisX[i] = basisRotation * instanceCoodinate.BasisX;
                    basisZ[i] = basisRotation * instanceCoodinate.BasisZ;

                    center1Set[i] = ClsRevitUtil.GetPointSyabariTanten(doc, pickedInstance, keyCenter1);
                    center2Set[i] = ClsRevitUtil.GetPointSyabariTanten(doc, pickedInstance, keyCenter2);

                    var toZXplane = SMatrix3d.BasisTransform(Plane.CreateByOriginAndBasis(origin[i], basisZ[i], basisX[i]));
                    var fromZXplane = toZXplane.Inverse();
                    var proj = fromZXplane * SMatrix3d.ProjXY * toZXplane;
                    v3[i] = proj * center2Set[i];

                    var aaaa = center2Set[i] - origin[i];

                    edge1Set[i] = ClsRevitUtil.GetPointSyabariTanten(doc, pickedInstance, keyEdge1);
                    edge2Set[i] = ClsRevitUtil.GetPointSyabariTanten(doc, pickedInstance, keyEdge2);

#if DEBUG
                    var length3 = ClsRevitUtil.CovertFromAPI(origin[i].DistanceTo(v3[i]));
                    var length2 = ClsRevitUtil.CovertFromAPI(v3[i].DistanceTo(center2Set[i]));
                    var length1 = ClsRevitUtil.CovertFromAPI(center2Set[i].DistanceTo(center1Set[i]));
                    var length0 = ClsRevitUtil.CovertFromAPI(center1Set[i].DistanceTo(edge1Set[i]));
                    var w = ClsRevitUtil.CovertFromAPI(edge1Set[i].DistanceTo(edge2Set[i]));
#endif
                    l3[i] = origin[i].DistanceTo(v3[i]);
                    l2[i] = v3[i].DistanceTo(center2Set[i]);
                    l1Set[i] = center2Set[i].DistanceTo(center1Set[i]);
                    l0Set[i] = center1Set[i].DistanceTo(edge1Set[i]);

                    //v3[i] = v3;
                    v2[i] = v3[i] + l2[i] * basisY[i];
                    v1[i] = v2[i] + l1Set[i] * basisY[i];
                    v0[i] = v1[i] + l0Set[i] * basisY[i];

                    var edgeDist = edge1Set[i].DistanceTo(edge2Set[i]);
                    e0[i] = v0[i] + /*l3Set[i] * */(origin[i] - v3[i]);// - edgeDist * basisX[i];
                }

                // theta2 �̎Z�o
                var theta2Set = new double[num];
                var rotateTheta2 = new SMatrix3d[num];
                var v1prime = new XYZ[num];
                var basisYprime = new XYZ[num];
                var basisZprime = new XYZ[num];
                for (int i = 0; i < num; i++)
                {
                    var buddy = (i + 1) % num;

                    var pt = v2[i];// originSet[i] + l2Set[i] * basisYSet[i];
                    var buddyPt = v2[buddy];// originSet[buddy] + l2Set[buddy] * basisYSet[buddy];

                    var yzPlane = Plane.CreateByOriginAndBasis(XYZ.Zero, basisY[i], basisZ[i]);
                    //var yzPlane = Plane.CreateByNormalAndOrigin(-basisX[i], XYZ.Zero);
                    var projection = SMatrix3d.ProjXY * SMatrix3d.BasisTransform(yzPlane);

                    var p1 = projection * pt;
                    var p2 = projection * buddyPt;

                    var originToV3_1 = projection * (v3[i] - origin[i]);
                    var originToV3_2 = projection * (v3[buddy] - origin[buddy]);

                    var h = Math.Abs(p1.Y - p2.Y);
                    var l = Math.Abs(p1.X - p2.X);
                    var w1 = -originToV3_1.Y;// l3[i];
                    var w2 = originToV3_2.Y;//l3[buddy];
                    var a = w1 + w2;

                    //var ans = UtilAlgebra.SolveQuadraticEquation(
                    //    h * h + l * l,
                    //    2.0 * a * h,
                    //    a * a - l * l);

                    var cos = UtilAlgebra.SolveQuadraticEquation(
                        h * h + l * l,
                        2.0 * a * h,
                        a * a - l * l)
                        .Where(x => ClsGeo.GEO_EQ(x.Imaginary, 0.0))
                        .Select(x => x.Real)
                        .FirstOrDefault(x => ClsGeo.GEO_LE(0, x) || ClsGeo.GEO_LE(x, 1.0));

                    var theta_ = Math.Acos(cos);
                    var theta0 = p2.Y < p1.Y ? theta_ : -theta_;

#if DEBUG
                    var vec2 = projection * (buddyPt - pt);
                    var vec1 = projection * basisY[i];

                    var theta1 = vec1.AngleOnPlaneTo(vec2, XYZ.BasisZ);
                    var diff = SLibRevitReo.PrincipalRadian(theta1) - SLibRevitReo.PrincipalRadian(theta0);
#endif

                    var theta = theta0;
                    theta2Set[i] = SLibRevitReo.PrincipalRadian(theta);
                    rotateTheta2[i] = SMatrix3d.Rotation(-basisX[i], v2[i], theta2Set[i]);
                    v1prime[i] = rotateTheta2[i] * v1[i];
                    var rotateBasis = SMatrix3d.Rotation(-basisX[i], XYZ.Zero, theta2Set[i]);
                    basisYprime[i] = rotateBasis * basisY[i];
                    basisZprime[i] = rotateBasis * basisZ[i];
                }

                // theta1 �̎Z�o
                var theta1Set = new double[num];
                var rotateTheta1 = new SMatrix3d[num];
                var v0prime = new XYZ[num];
                var e0prime = new XYZ[num];
                var basisXprime = new XYZ[num];
                var basisYprime2 = new XYZ[num];
                for (int i = 0; i < num; i++)
                {
                    var buddy = (i + 1) % num;

                    var pt = v1prime[i];
                    var buddyPt = v1prime[buddy];

                    var xyPrimePlane = Plane.CreateByNormalAndOrigin(-basisZprime[i], XYZ.Zero);
                    var projection = SMatrix3d.ProjXY * SMatrix3d.BasisTransform(xyPrimePlane);

                    var vec2 = projection * (buddyPt - pt);
                    var vec1 = projection * basisYprime[i];
                    var theta = vec1.AngleOnPlaneTo(vec2, XYZ.BasisZ);
                    theta1Set[i] = SLibRevitReo.PrincipalRadian(theta);
                    rotateTheta1[i] = SMatrix3d.Rotation(-basisZprime[i], pt, theta1Set[i]) * rotateTheta2[i];
                    v0prime[i] = rotateTheta1[i] * v0[i];
                    e0prime[i] = rotateTheta1[i] * e0[i];
                    var rotateBasis = SMatrix3d.Rotation(-basisZprime[i], XYZ.Zero, theta1Set[i]);
                    basisYprime2[i] = rotateBasis * basisYprime[i];
                    basisXprime[i] = rotateBasis * basisX[i];
                }

                // theta0 �̎Z�o
                var theta0Set = new double[num];
                var rotateTheta0 = new SMatrix3d[num];
                var basisXprime2 = new XYZ[num];
                for (int i = 0; i < num; i++)
                {
                    var a = basisZprime[i];
                    var b = basisZ[i];
                    var radian = SLibRevitReo.PrincipalRadian(a.AngleOnPlaneTo(b, -basisYprime2[i]));

                    // theta0 �p�����[�^�����݂��Ȃ��Ƃ��� 0 �Ƃ��Čv�Z��i�߂�
                    var existsTheta0 = ClsRevitUtil.FindParameter(doc, pieceInstances[i].Id, keyTheta0);
                    theta0Set[i] = existsTheta0 ? radian : 0.0;

                    rotateTheta0[i] = SMatrix3d.Rotation(-basisYprime2[i], e0prime[i], theta0Set[i]) * rotateTheta1[i];
                    basisXprime2[i] = SMatrix3d.Rotation(-basisYprime2[i], XYZ.Zero, theta0Set[i]) * basisXprime[i];
                }

                // �Η��x�[�X�t�@�~���̓Ǎ�
                var symbolFolpath = ClsZumenInfo.GetYMSFolder();
                var shinfamily = System.IO.Path.Combine(symbolFolpath, "�x�[�X�֌W", "�Η��x�[�X" + ".rfa");
                if (!ClsRevitUtil.LoadFamilySymbolData(doc, shinfamily, "�Η��x�[�X", out FamilySymbol sym))
                {
                    return Result.Failed;
                }

                using (var transaction = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                {
                    transaction.Start();
                    for (int i = 0; i < num; i++)
                    {
                        ClsRevitUtil.SetParameter(doc, pieceInstances[i].Id, keyTheta2, ClsYMSUtil.CheckSyabariAngle(theta2Set[i]));
                        ClsRevitUtil.SetParameter(doc, pieceInstances[i].Id, keyTheta1, ClsYMSUtil.CheckSyabariAngle(theta1Set[i]));
                        ClsRevitUtil.SetParameter(doc, pieceInstances[i].Id, keyTheta0, ClsYMSUtil.CheckSyabariAngle(theta0Set[i]));

                    }

                    var tmpStPoint = e0prime[0];
                    var tmpEdPoint = e0prime[1];
                    var tmpThirdPoint = e0prime[0] - basisXprime2[0];

                    var slopePlane = ClsRevitUtil.CreateReferencePlane(doc, tmpStPoint, tmpEdPoint, tmpThirdPoint);

                    if (!sym.IsActive)
                    {
                        sym.Activate();
                    }

                    try
                    {
                        var family = sym.Family;
                        //���̃^�C�v�łȂ��ƎQ�Ɩʂɔz�u�ł��Ȃ�
                        if (family.FamilyPlacementType == FamilyPlacementType.WorkPlaneBased)
                        {
                            var reference = slopePlane.GetReference();
                            var dir = (tmpEdPoint - tmpStPoint).Normalize();
                            var instance = doc.Create.NewFamilyInstance(reference, tmpStPoint, dir, sym);//tmpEdPoint - tmpStPoint

                            ClsRevitUtil.SetParameter(doc, instance.Id, "����", tmpStPoint.DistanceTo(tmpEdPoint));
                            ClsRevitUtil.SetParameter(doc, instance.Id, "�|�ރT�C�Y", "35HA");// ClsCommonUtils.GetOnlyNumberInStringReturnString(name1) + "HA");//�T�C�Y�͎Η���s�[�X������擾���邪�����x�Ȃǂ��g�p���邩�͕s��

                            var id = instance.Id;
                            var element = doc.GetElement(reference);
                            // �����ň��ړ��Ȃǂ����Ȃ��� Location �����_�̂܂ܕԂ��Ă���B
                            ElementTransformUtils.MoveElement(doc, id, XYZ.Zero);
                            slopePlane.Name = "�Η�" + id.IntegerValue.ToString();
                        }
                        else
                        {
                            //�쐬�o���Ȃ�
                        }
                    }
                    catch (Exception ex)
                    {
                        TaskDialog.Show("TEST4", ex.Message);
                    }

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                //#if DEBUG
                TaskDialog taskDialog = new TaskDialog("TEST3")
                {
                    TitleAutoPrefix = true,
                    MainIcon = TaskDialogIcon.TaskDialogIconError,
                    MainInstruction = ex.GetType().ToString().Split('.').LastOrDefault(),
                    MainContent = ex.Message,
                    ExpandedContent = ex.StackTrace,
                    CommonButtons = TaskDialogCommonButtons.Close,
                };
                taskDialog.Show();
                //#endif
                return Result.Failed;
            }
            return Result.Succeeded;
        }


        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            return CreateBase(uiapp, uidoc, doc);
        }
    }
    //TESTCommand
    [Transaction(TransactionMode.Manual)]
    public class TEST4 : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                //var tmpStPoint = uidoc.Selection.PickPoint("�n�_��I�����Ă�������");
                //var tmpEdPoint = uidoc.Selection.PickPoint("�I�_��I�����Ă�������");

                ElementId id1 = null;
                if (!ClsHaraokoshiBase.PickBaseObject(uidoc, ref id1, "�n�_���̕��N�x�[�X��I�����Ă�������"))
                    return Result.Failed;
                var inst1 = doc.GetElement(id1) as FamilyInstance;
                var cv1 = (inst1.Location as LocationCurve).Curve;
                var levelId1 = inst1.Host.Id;

                ElementId id2 = null;
                if (!ClsHaraokoshiBase.PickBaseObject(uidoc, ref id2, "�I�_���̕��N�x�[�X��I�����Ă�������"))
                    return Result.Failed;
                var inst2 = doc.GetElement(id2) as FamilyInstance;
                var cv2 = (inst2.Location as LocationCurve).Curve;
                var levelId2 = inst2.Host.Id;

                var tmpStPoint = (cv1.GetEndPoint(0) + cv1.GetEndPoint(1)) / 2;
                var tmpEdPoint = (cv2.GetEndPoint(0) + cv2.GetEndPoint(1)) / 2;

                if (false)
                {
                    ElementId lineId = null;
                    if (!ClsRevitUtil.PickObject(uidoc, "���f������", "���f������", ref lineId))
                        return Result.Failed;
                    var line = (doc.GetElement(lineId).Location as LocationCurve).Curve;
                }
                else
                {
                    Reference rfHara1 = null;
                    if (!ClsHaraokoshiBase.PickBaseObject(uidoc, ref rfHara1, "���N�x�[�X��Ŏn�_��I�����Ă�������"))
                    {
                        //return;
                    }
                    tmpStPoint = ClsRevitUtil.CorrectedPitchPoint(rfHara1.GlobalPoint, cv1.GetEndPoint(0), 100.0);

                    Reference rfHara2 = null;
                    if (!ClsHaraokoshiBase.PickBaseObject(uidoc, ref rfHara2, "���N�x�[�X��ŏI�_��I�����Ă�������"))
                    {
                        //return;
                    }
                    tmpEdPoint = ClsRevitUtil.CorrectedPitchPoint(rfHara2.GlobalPoint, cv2.GetEndPoint(0), 100.0);
                }

                DLG.DlgTEST dlg = new DLG.DlgTEST();
                dlg.ShowDialog();
                //�_�C�A���O�ŉ����ʂ̌���
                double fall = ClsRevitUtil.CovertToAPI(dlg.fall);

                Line cv = Line.CreateBound(tmpStPoint, tmpEdPoint);
                var dir = tmpEdPoint - tmpStPoint;// cv.Direction;

                //�Q�Ɩʂ̍쐬
                ReferencePlane slopePlane = null;
                using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                {
                    t.Start();
                    FailureHandlingOptions failOpt = t.GetFailureHandlingOptions();
                    failOpt.SetFailuresPreprocessor(new WarningSwallower());
                    t.SetFailureHandlingOptions(failOpt);
                    tmpEdPoint = new XYZ(tmpEdPoint.X, tmpEdPoint.Y, tmpEdPoint.Z - fall);
                    slopePlane = ClsRevitUtil.CreateReferencePlaneW(doc, tmpStPoint, tmpEdPoint, fall);
                    t.Commit();
                }
                //�Β��̓Ǎ�
                string symbolFolpath = ClsZumenInfo.GetYMSFolder();
                string shinfamily = System.IO.Path.Combine(symbolFolpath, "�x�[�X�֌W\\" + "�Η��x�[�X" + ".rfa");
                if (!ClsRevitUtil.LoadFamilySymbolData(doc, shinfamily, "�Η��x�[�X", out FamilySymbol sym))
                {
                    return Result.Failed;
                }

                //�쐬�����Q�ƖʂɃx�[�X��z�u
                using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                {
                    t.Start();
                    FailureHandlingOptions failOpt = t.GetFailureHandlingOptions();
                    failOpt.SetFailuresPreprocessor(new WarningSwallower());
                    t.SetFailureHandlingOptions(failOpt);
                    if (sym == null)
                    {
                        return Result.Failed;
                    }

                    if (!sym.IsActive)
                    {
                        sym.Activate();
                    }

                    try
                    {
                        //tmpEdPoint = new XYZ(tmpEdPoint.X, tmpEdPoint.Y, tmpEdPoint.Z - fall);

                        var family = sym.Family;
                        //���̃^�C�v�łȂ��ƎQ�Ɩʂɔz�u�ł��Ȃ�
                        if (family.FamilyPlacementType == FamilyPlacementType.WorkPlaneBased)
                        {
                            var reference = slopePlane.GetReference();
                            var instance = doc.Create.NewFamilyInstance(reference, tmpStPoint, tmpEdPoint - tmpStPoint, sym);

                            ClsRevitUtil.SetParameter(doc, instance.Id, "����", tmpStPoint.DistanceTo(tmpEdPoint));
                            ClsRevitUtil.SetParameter(doc, instance.Id, "�|�ރT�C�Y", dlg.size);

                            var id = instance.Id;
                            var element = doc.GetElement(reference);
                            // �����ň��ړ��Ȃǂ����Ȃ��� Location �����_�̂܂ܕԂ��Ă���B
                            ElementTransformUtils.MoveElement(doc, id, XYZ.Zero);
                            slopePlane.Name = "�Η�" + id.IntegerValue.ToString();
                        }
                        else
                        {
                            //�쐬�o���Ȃ�
                        }
                    }
                    catch (Exception ex)
                    {
                        TaskDialog.Show("TEST4", ex.Message);
                    }
                    ClsRevitUtil.SetParameter(doc, id2, "�z�X�g����̃I�t�Z�b�g", -fall);
                    t.Commit();
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("TEST4", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }
            return Result.Succeeded;
        }
    }
    //TESTCommand
    [Transaction(TransactionMode.Manual)]
    public class TEST5 : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;

                var id = uidoc.Selection.PickObject(ObjectType.Element, "�Η��x�[�X��I�����Ă�������").ElementId;
                var name = "�Β�" + id.IntegerValue.ToString();
                ClsYMSUtil.CreateBaseElevationView(doc, id, name);
                ClsYMSUtil.ChangeView(uidoc, name);

                var inst = doc.GetElement(id) as FamilyInstance;
                var lPoint = inst.Location as LocationPoint;
                var tmpStPoint = lPoint.Point;
                var dist = ClsRevitUtil.GetParameterDouble(doc, id, "����");
                var tmpEdPoint = tmpStPoint + dist * inst.HandOrientation;

                var scaleIds = ClsWarituke.CreateScale(uidoc, tmpStPoint, tmpEdPoint, WaritukeDist.warituke100mm, cross: true);
                //�폜�t���O����

            }
            catch (Exception ex)
            {
                TaskDialog.Show("TEST5", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }
            return Result.Succeeded;
        }
    }
    //TESTCommand
    //TESTCommand
    [Transaction(TransactionMode.Manual)]
    public class TEST6 : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                ClsJson.ReadJOSN(doc);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("TEST5", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }
            return Result.Succeeded;
        }
    }
    //TESTCommand
    //TESTCommand
    [Transaction(TransactionMode.Manual)]
    public class TEST7 : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;

                var id = uidoc.Selection.PickObject(ObjectType.Element, "�Η��x�[�X��I�����Ă�������").ElementId;
                var name = "�Β�" + id.IntegerValue.ToString();
                ClsYMSUtil.CreateBaseElevationView(doc, id, name);
                ClsYMSUtil.ChangeView(uidoc, name);

                var inst = doc.GetElement(id) as FamilyInstance;
                var lPoint = inst.Location as LocationPoint;
                var tmpStPoint = lPoint.Point;
                var dist = ClsRevitUtil.GetParameterDouble(doc, id, "����");
                var tmpEdPoint = tmpStPoint + dist * inst.HandOrientation;

                var scaleIds = ClsWarituke.CreateScale(uidoc, tmpStPoint, tmpEdPoint, WaritukeDist.warituke100mm, cross: true);
                //�폜�t���O����

            }
            catch (Exception ex)
            {
                TaskDialog.Show("TEST5", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }
            return Result.Succeeded;
        }
    }
    //TESTCommand
    //TESTCommand
    [Transaction(TransactionMode.Manual)]
    public class TEST8 : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                //�_�C�A���O��\��
                Test_Form frm = new Test_Form(doc);
                DialogResult res = frm.ShowDialog();
                if (res != DialogResult.OK)
                    return Result.Cancelled;
                string strLevel = frm.m_level;

                //���[�U�[�ɖʂ�I�����Ă��炤
                Reference pickedRef = uidoc.Selection.PickObject(ObjectType.Face, "�ʂ�I�����Ă�������");
                Element element = doc.GetElement(pickedRef);
                GeometryObject geoObj = element.GetGeometryObjectFromReference(pickedRef);
                Face face = geoObj as Face;

                if (face == null)
                {
                    TaskDialog.Show("�G���[", "�I�����ꂽ�I�u�W�F�N�g�͗L���Ȗʂł͂���܂���B");
                    return Result.Failed;
                }

                //Transform ���擾�i�K�v�Ȃ�j
                Transform transform = GetGeometryTransform(doc, element);

                //���x��Z�擾
                ElementId targetLevelId = ClsRevitUtil.GetLevelID(doc, strLevel);
                Level level = doc.GetElement(targetLevelId) as Level;
                double levelZ = level.ProjectElevation;

                //��_�擾�iTransform �K�p�ς݁j
                List<XYZ> intersectionPoints = GetTransformedFaceIntersectionPoints(face, transform, levelZ);
                if (intersectionPoints.Count < 2)
                {
                    TaskDialog.Show("���", "�����_��2�_�����̂��ߐ���`�悵�܂���B");
                    return Result.Succeeded;
                }

                //�ŉ��_�y�A�擾
                (XYZ pt1, XYZ pt2) = GetFarthestPoints(intersectionPoints);

                //���f���������쐬
                RevitUtil.ClsRevitUtil.CreateNewModelCurve(doc,pt1, pt2);

                ////���[�U�[���������̐��ƍ��W���w��
                //ElementId id = null;
                //if (!ClsRevitUtil.PickObject(uidoc, "�n�[���̃��f������", "���f������", ref id))
                //{
                //    return Result.Cancelled;
                //}
                //Element inst = doc.GetElement(id);
                //LocationCurve lCurve = inst.Location as LocationCurve;
                //if (lCurve == null)
                //{
                //    return Result.Cancelled;
                //}

                ////���[�U�[���ʂ̐������W���w��
                //Selection selection = uidoc.Selection;
                //XYZ startPoint = selection.PickPoint("�n�_�i���f���������j���w�肵�Ă�������");
                //XYZ checkPoint = new XYZ(startPoint.X, startPoint.Y, 0);
                //Curve checkCv = Line.CreateBound(new XYZ(lCurve.Curve.GetEndPoint(0).X, lCurve.Curve.GetEndPoint(0).Y, 0), new XYZ(lCurve.Curve.GetEndPoint(1).X, lCurve.Curve.GetEndPoint(1).Y, 0));
                //if (!ClsRevitUtil.IsPointOnCurve(checkCv, checkPoint))
                //{
                //    MessageBox.Show("�I���������f��������̓_���w�肵�Ă�������");
                //    return Result.Cancelled;
                //}

                ////�I�_���̐������W���w��
                //XYZ�@endPoint = selection.PickPoint("�I�_�i�ʑ��j���w�肵�Ă�������");
                //Line line2 = Line.CreateBound(pt1, pt2);
                //IntersectionResult result = line2.Project(endPoint);
                //endPoint = result.XYZPoint;
                //RevitUtil.ClsRevitUtil.CreateNewModelCurveKeisha(doc, startPoint, endPoint);

                return Result.Succeeded;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }

        public void DrawDebugPoint(XYZ point, Document doc)
        {
            using (Transaction tx = new Transaction(doc, "�f�o�b�O�_�`��"))
            {
                tx.Start();

                // SketchPlane�iXY���ʁj
                Plane plane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, point);
                SketchPlane sketchPlane = SketchPlane.Create(doc, plane);

                // �Z�������\���ɕ`��
                double len = 2; // 10cm���x�̒Z����
                XYZ p1 = point + new XYZ(-len / 2, 0, 0);
                XYZ p2 = point + new XYZ(len / 2, 0, 0);
                XYZ p3 = point + new XYZ(0, -len / 2, 0);
                XYZ p4 = point + new XYZ(0, len / 2, 0);

                Line line1 = Line.CreateBound(p1, p2);
                Line line2 = Line.CreateBound(p3, p4);

                doc.Create.NewModelCurve(line1, sketchPlane);
                doc.Create.NewModelCurve(line2, sketchPlane);

                tx.Commit();
            }
        }

        /// <summary>
        /// �v�f�������� Transform�i�t�@�~���C���X�^���X�̏ꍇ�͕K�{�j
        /// </summary>
        private Transform GetGeometryTransform(Document doc, Element element)
        {
            Options opt = new Options
            {
                ComputeReferences = true,
                IncludeNonVisibleObjects = true,
                DetailLevel = ViewDetailLevel.Fine
            };

            GeometryElement geoElem = element.get_Geometry(opt);
            foreach (GeometryObject obj in geoElem)
            {
                if (obj is GeometryInstance geoInst)
                {
                    return geoInst.Transform;
                }
            }

            return Transform.Identity; // Transform �s�v�ȏꍇ�͒P�ʍs��
        }

        /// <summary>
        /// �ʂƐ�����Z�Ƃ̌����_���擾�iTransform�l���j
        /// </summary>
        private List<XYZ> GetTransformedFaceIntersectionPoints(Face face, Transform transform, double levelZ)
        {
            List<XYZ> points = new List<XYZ>();

            foreach (EdgeArray edgeArray in face.EdgeLoops)
            {
                foreach (Edge edge in edgeArray)
                {
                    Curve curve = edge.AsCurve();

                    XYZ p1 = transform.OfPoint(curve.GetEndPoint(0));
                    XYZ p2 = transform.OfPoint(curve.GetEndPoint(1));

                    double z1 = p1.Z;
                    double z2 = p2.Z;

                    if ((z1 < levelZ && z2 > levelZ) || (z1 > levelZ && z2 < levelZ))
                    {
                        double t = (levelZ - z1) / (z2 - z1);
                        XYZ intersection = p1 + t * (p2 - p1);
                        points.Add(intersection);
                    }
                }
            }

            return points;
        }

        /// <summary>
        /// ��_�̂����ł�����������2�_��Ԃ�
        /// </summary>
        private (XYZ, XYZ) GetFarthestPoints(List<XYZ> points)
        {
            double maxDist = 0;
            XYZ pt1 = null, pt2 = null;

            for (int i = 0; i < points.Count; i++)
            {
                for (int j = i + 1; j < points.Count; j++)
                {
                    double dist = points[i].DistanceTo(points[j]);
                    if (dist > maxDist)
                    {
                        maxDist = dist;
                        pt1 = points[i];
                        pt2 = points[j];
                    }
                }
            }

            return (pt1, pt2);
        }

        /// <summary>
        /// �v�f�ɕR�Â��Ă��� Level ���擾
        /// </summary>
        private Level GetElementLevel(Document doc, Element element)
        {
            //    BuiltInParameter[] levelParams = new[]
            //    {
            //    BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM
            //};

            BuiltInParameter[] levelParams = new[]
{
            BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM
        };

            foreach (var bip in levelParams)
            {
                Parameter p = element.get_Parameter(bip);
                if (p != null && p.HasValue)
                {
                    return doc.GetElement(p.AsElementId()) as Level;
                }
            }

            return null;
        }
    }


    [Transaction(TransactionMode.Manual)]
    public class TEST9 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                var uiapp = commandData.Application;
                var uidoc = uiapp.ActiveUIDocument;
                var doc = uidoc.Document;

                var pick1 = uidoc.Selection.PickObject(ObjectType.Element, "1 �{�ڂ̎Η��x�[�X��I��");
                var pick2 = uidoc.Selection.PickObject(ObjectType.Element, "2 �{�ڂ̎Η��x�[�X��I��");
                var pick3 = uidoc.Selection.PickObject(ObjectType.Element, "�Η��Ȃ��x�[�X (���f������) ��I��");

                var testDialog = new Autodesk.Revit.UI.TaskDialog(GetType().Name)
                {
                    TitleAutoPrefix = true,
                    MainIcon = Autodesk.Revit.UI.TaskDialogIcon.TaskDialogIconInformation,
                    MainInstruction = "��[�Ɖ��[�̂ǂ�����Z�o���܂���",
                    MainContent = "",
                    CommonButtons = Autodesk.Revit.UI.TaskDialogCommonButtons.Close,

                };
                testDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "�㑤���Z�o", "�Η��Ȃ���z��");
                testDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, "�������Z�o", "�Η��󂯂�z��");
                var testDialogResult = testDialog.Show();

                Plane sectionPlane;
                // pick���ꂽ�Η��Ȃ��x�[�X�̃��f���������牔�������̒f�ʕ��ʂ��쐬
                {
                    var tsunagiModelLine = doc.GetElement(pick3) as ModelLine;
                    var tsunagiLine = tsunagiModelLine.GeometryCurve as Line;
                    var a = tsunagiLine.GetEndPoint(0);
                    var b = tsunagiLine.GetEndPoint(1);
                    sectionPlane = Plane.CreateByOriginAndBasis(a, (b - a).Normalize(), XYZ.BasisZ);
                }

                if (testDialogResult == TaskDialogResult.CommandLink1)
                {

                    var pickResultSet = new Reference[] { pick1, pick2 };
                    var topPoints = new List<XYZ>();
                    var sectionBBoxes = new List<XYZ[]>();
                    foreach (var pickResult in pickResultSet)
                    {
                        var shabariBase = doc.GetElement(pickResult) as FamilyInstance;

                        // �Η��x�[�X�̒������Η��x�[�X�t�@�~���� X ���Ƃ��Čv�Z
                        var shabariBaseTrans = shabariBase.GetTotalTransform();
                        var shabariBaseLine = Line.CreateUnbound(shabariBaseTrans.Origin, shabariBaseTrans.BasisX);

                        // �Η��x�[�X�̕��ʂ� Host ����擾
                        var shabariBasePlane = (shabariBase.Host as ReferencePlane)?.GetPlane();

                        var shabariW = 350.0; // �Η��|�ރT�C�Y�����ɌŒ�l�Ŏw��
                        var shabariH = 350.0; // �Η��|�ރT�C�Y�����ɌŒ�l�Ŏw��

                        // �Η��|�ނ̒f�ʂ̒[�_���Z�o
                        var points = ClsYMSUtil.CalcShabariSection(shabariBaseLine, shabariBasePlane, shabariW, shabariH, sectionPlane);
                        sectionBBoxes.Add(points);

                        // �Η��f�ʂ̒[�_�̒��� Z �������ő�̓_���擾
                        var sectionTop = ClsYMSUtil.CalcShabariSectionTop(shabariBaseLine, shabariBasePlane, shabariW, shabariH, sectionPlane);
                        if (sectionTop != null)
                        {
                            topPoints.Add(sectionTop);
                        }
                        else
                        {
                            var dialog = new TaskDialog(nameof(YMS.TEST9))
                            {
                                MainInstruction = "�Η��f�ʂ̒[�_���Z�o�ł��܂���",
                                MainContent = "�Η��x�[�X�ƎΗ��x�[�X�Ȃ������s��������܂���",
                                MainIcon = TaskDialogIcon.TaskDialogIconError,
                            };
                            dialog.Show();
                        }
                    }

                    using (var transaction = new Transaction(doc, Guid.NewGuid().ToString()))
                    {
                        transaction.Start();

                        // �Η��f�ʂ����f�������ō쐬
                        foreach (var points in sectionBBoxes)
                        {
                            var p = SketchPlane.Create(doc, sectionPlane);
                            for (int i = 0; i < points.Length; i++)
                            {
                                for (int j = i + 1; j < points.Length; j++)
                                {
                                    var a = points[i];
                                    var b = points[j];
                                    var l = Line.CreateBound(a, b);
                                    var mline = uidoc.Document.Create.NewModelCurve(l, p);
                                }
                            }
                        }

                        // �Η��f�ʂ̍ō��_�����ԃ��f���������쐬
                        if (topPoints.Count == 2)
                        {
                            var l = Line.CreateBound(topPoints.FirstOrDefault(), topPoints.LastOrDefault());
                            var p = SketchPlane.Create(doc, sectionPlane);
                            uidoc.Document.Create.NewModelCurve(l, p);
                        }
                        transaction.Commit();
                    }
                }
                else if (testDialogResult == TaskDialogResult.CommandLink2)
                {
                    var pickResultSet = new Reference[] { pick1, pick2 };
                    var topPoints = new List<XYZ>();
                    var sectionBBoxes = new List<XYZ[]>();
                    foreach (var pickResult in pickResultSet)
                    {
                        var shabariBase = doc.GetElement(pickResult) as FamilyInstance;

                        // �Η��x�[�X�̒������Η��x�[�X�t�@�~���� X ���Ƃ��Čv�Z
                        var shabariBaseTrans = shabariBase.GetTotalTransform();
                        var shabariBaseLine = Line.CreateUnbound(shabariBaseTrans.Origin, shabariBaseTrans.BasisX);

                        // �Η��x�[�X�̕��ʂ� Host ����擾
                        var shabariBasePlane = (shabariBase.Host as ReferencePlane)?.GetPlane();

                        var shabariW = 350.0; // �Η��|�ރT�C�Y�����ɌŒ�l�Ŏw��
                        var shabariH = 350.0; // �Η��|�ރT�C�Y�����ɌŒ�l�Ŏw��
                        var ukezaiW = 200.0; // �Η��󂯍ލ|�ރT�C�Y�����ɌŒ�l

                        // �Η��|�ނ̒f�ʂ̒[�_���Z�o
                        var ukeWhalf = 0.5 * ClsRevitUtil.CovertToAPI(ukezaiW);
                        var points = Enumerable.Empty<double>()
                            .Append(ukeWhalf)
                            .Append(-ukeWhalf)
                            .Select(x => Plane.CreateByNormalAndOrigin(sectionPlane.Normal, sectionPlane.Origin + x * sectionPlane.Normal.Normalize()))
                            .Select(x => ClsYMSUtil.CalcShabariSection(shabariBaseLine, shabariBasePlane, shabariW, shabariH, x));
                        //var points = ClsYMSUtil.CalcShabariSection(shabariBaseLine, shabariBasePlane, shabariW, shabariH, sectionPlane);
                        sectionBBoxes.AddRange(points);

                        // �Η��f�ʂ̒[�_�̒��� Z �������ŏ��̓_���擾
                        var sectionTop = ClsYMSUtil.CalcShabariSectionBottom(shabariBaseLine, shabariBasePlane, shabariW, shabariH, sectionPlane, ukezaiW);
                        if (sectionTop != null)
                        {
                            topPoints.Add(sectionTop);
                        }
                        else
                        {
                            var dialog = new TaskDialog(nameof(YMS.TEST9))
                            {
                                MainInstruction = "�Η��f�ʂ̒[�_���Z�o�ł��܂���",
                                MainContent = "�Η��x�[�X�ƎΗ��x�[�X�󂯂����s��������܂���",
                                MainIcon = TaskDialogIcon.TaskDialogIconError,
                            };
                            dialog.Show();
                        }
                    }

                    using (var transaction = new Transaction(doc, Guid.NewGuid().ToString()))
                    {
                        transaction.Start();

                        // �Η��f�ʂ����f�������ō쐬
                        foreach (var points in sectionBBoxes)
                        {
                            var p = SketchPlane.Create(doc, sectionPlane);
                            for (int i = 0; i < points.Length; i++)
                            {
                                for (int j = i + 1; j < points.Length; j++)
                                {
                                    var a = points[i];
                                    var b = points[j];
                                    var l = Line.CreateBound(a, b);
                                    var linePlane = TempPlane(l);
                                    var sketchP = SketchPlane.Create(doc, linePlane);
                                    var mline = uidoc.Document.Create.NewModelCurve(l, sketchP);
                                }
                            }
                        }

                        // �Η��f�ʂ̍ō��_�����ԃ��f���������쐬
                        if (topPoints.Count == 2)
                        {
                            var p1 = topPoints.FirstOrDefault();
                            var p2 = topPoints.LastOrDefault();
                            var l = Line.CreateBound(p1, p2);
                            var linePlane = TempPlane(l);
                            var p = SketchPlane.Create(doc, linePlane);
                            uidoc.Document.Create.NewModelCurve(l, p);
                        }
                        transaction.Commit();
                    }
                }

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                //#if DEBUG
                TaskDialog taskDialog = new TaskDialog("Error!!!")
                {
                    TitleAutoPrefix = true,
                    MainIcon = TaskDialogIcon.TaskDialogIconError,
                    MainInstruction = ex.GetType().ToString().Split('.').LastOrDefault(),
                    MainContent = ex.Message,
                    ExpandedContent = ex.StackTrace,
                    CommonButtons = TaskDialogCommonButtons.Close,
                };
                taskDialog.Show();
                //#endif
                return Result.Failed;
            }
        }

        private static Plane TempPlane(Line line)
        {
            var p1 = line.GetEndPoint(0);
            var p2 = line.GetEndPoint(1);
            var a = (p2 - p1).Normalize();
            var crossProd = a.CrossProduct(XYZ.BasisZ);
            if (!ClsGeo.GEO_EQ0(crossProd.GetLength()))
            {
                return Plane.CreateByOriginAndBasis(p1, a, crossProd.Normalize());
            }
            else
            {
                return Plane.CreateByNormalAndOrigin(XYZ.BasisX, p1);
            }
        }
    }
    [Transaction(TransactionMode.Manual)]
    public class TEST10 : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;

                //�h�L�������g���擾
                Document doc = uidoc.Document;

                ElementId id = null;
                if (!ClsHaraokoshiBase.PickBaseObject(uidoc, ref id))
                    return Result.Failed;

                ElementId lineId = null;
                if (!ClsRevitUtil.PickObject(uidoc, "���f������", "���f������", ref lineId))
                    return Result.Failed;

                var inst = doc.GetElement(id) as FamilyInstance;
                var cv = (inst.Location as LocationCurve).Curve;
                var levelId = inst.Host.Id;

                var line = (doc.GetElement(lineId).Location as LocationCurve).Curve;

                var insec = ClsRevitUtil.GetIntersectionZ0(cv, line);
                if (insec != null)
                {
                    //���̒l
                    var name = "35VP";
                    var typeName = "���Α�";
                    var path = Master.ClsHiuchiCsv.GetFamilyPath(name);

                    //�V���{���Ǎ�
                    var familyName = RevitUtil.ClsRevitUtil.GetFamilyName(path);
                    //if (!ClsRevitUtil.LoadFamilyData(doc, path, out Family tanbuFam))
                    //    return Result.Failed;
                    //var sym = (ClsRevitUtil.GetFamilySymbol(doc, familyName, typeName));
                    //�V���{���Ǎ�
                    if (!ClsRevitUtil.LoadFamilySymbolData(doc, path, familyName, out var sym))
                        return Result.Failed;
                    if (sym != null)
                    {
                        using (Transaction t = new Transaction(doc, "Create Family"))
                        {
                            t.Start();
                            ElementId CreatedId = ClsRevitUtil.Create(doc, insec, levelId, sym);
                            t.Commit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("TEST10", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }
            return Result.Succeeded;
        }
    }
    [Transaction(TransactionMode.Manual)]
    public class TEST11 : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                //UIDocument uidoc = uiapp.ActiveUIDocument;
                ClsTest clsTest = new ClsTest();
                clsTest.CommandTest_Kurane2(uiapp, false);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("TEST11", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }
            return Result.Succeeded;
        }
    }
    [Transaction(TransactionMode.Manual)]
    public class TEST12 : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                //UIDocument uidoc = uiapp.ActiveUIDocument;
                ClsTest clsTest = new ClsTest();
                clsTest.CommandTest_Kurane2(uiapp, true);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("TEST12", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }

            TaskDialog.Show("TEST12","END");
            return Result.Succeeded;
        }
    }
    [Transaction(TransactionMode.Manual)]
    public class TEST13 : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
				UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;

                List<ElementId> elementIds = new List<ElementId>();
                
                //�����I��
                if (ClsRevitUtil.PickObjectsPartSymbol(uidoc, "�{���g����ݒ肷�镔�ނ�I�����Ă��������B", ref elementIds))
				{
                    //�{���g���ƃ{���g���͉��̒l
                    string boltName = "���{���g";
                    int boltNum = 10;
                    using (Transaction t = new Transaction(doc, "Set Bolt Data"))
					{
                        t.Start();
                        foreach (ElementId id in elementIds)
                        {
                            ClsYMSUtil.SetBolt(doc, id, boltName, boltNum);
                        }
                        t.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("TEST13", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }

            //TaskDialog.Show("TEST13", "END");
            return Result.Succeeded;
        }
    }
    [Transaction(TransactionMode.Manual)]
    public class TEST14 : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;

                ElementId id = null;

                //���ޑI��
                if (ClsRevitUtil.PickObjectPartSymbol(uidoc, "�{���g����\�����镔�ނ�I�����Ă��������B", ref id))
                {
                    if (id != null)
                    {
                        var boltNameAndNum = ClsYMSUtil.GetBolt(doc, id);
                        string boltName = boltNameAndNum.Item1 == null ? "�{���g���Ȃ�" : boltNameAndNum.Item1;
                        int boltNum = boltNameAndNum.Item2;

                        string boltInfoMessage = $"�{���g��ށF{boltName}\r\n�{���g���F{boltNum}";
                        TaskDialog.Show("TEST14", boltInfoMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("TEST14", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }

            //TaskDialog.Show("TEST14", "END");
            return Result.Succeeded;
        }
    }
    [Transaction(TransactionMode.Manual)]
    public class TEST15 : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;

                DLG.DlgCreateKobetsuHaichi dlg = new DLG.DlgCreateKobetsuHaichi(true);
                DialogResult result = dlg.ShowDialog();

                if (result != DialogResult.OK)
                {
                    return Result.Cancelled;
                }

                string familyPath = dlg.GetFamilyPath();
                string familyType = dlg.GetFamilyType();
                if (string.IsNullOrEmpty(familyType))
                {
                    //�t�@�~���^�C�v�����݂��Ȃ��ꍇ�A�^�C�v�����V���{�����ɕύX
                    familyType = ClsRevitUtil.GetFamilyName(familyPath);
                }

                if (!ClsRevitUtil.LoadFamilySymbolData(doc, familyPath, familyType, out FamilySymbol sym))
                {
                    MessageBox.Show("�t�@�~���̎擾�Ɏ��s���܂���");
                    return Result.Cancelled;
                }
                if (sym == null)
                {
                    MessageBox.Show("�t�@�~���̎擾�Ɏ��s���܂���");

                    return Result.Cancelled;
                }

                ////�z�u����Ă���I�u�W�F�N�g��ێ�
                FilteredElementCollector collector = new FilteredElementCollector(doc);�@//#34229
                ICollection<ElementId> preElementIds = collector.WhereElementIsNotElementType().ToElementIds();//#34229
                preElementIds2 = preElementIds;//#34229

                ElementType et = doc.GetElement(sym.Id) as ElementType;
                uidoc.PostRequestForElementTypePlacement(et);

                uiapp.Idling += OnIdling;//#34229

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("TEST14", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }
        }

        private static ICollection<ElementId> preElementIds2;
        /// <summary>
        /// �ʔz�u���̒ǉ�ID�ɑ΂��鏈��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnIdling(object sender, IdlingEventArgs e)
        {
            UIApplication uiApp = sender as UIApplication;
            UIDocument uidoc = uiApp.ActiveUIDocument;
            Document doc = uidoc.Document;

            // �C�x���g����
            uiApp.Idling -= OnIdling;

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ICollection<ElementId> currentIds = collector.WhereElementIsNotElementType().ToElementIds();

            // �V�K�v�f�̌��o
            var newIds = currentIds.Except(preElementIds2).ToList();
            if (newIds.Count > 0)
            {
                // �z�u���ꂽ�v�f�ɑ΂��ď������s��
                foreach (ElementId id in newIds)
                {
                    Element newElement = doc.GetElement(id);
                    using (Transaction tx = new Transaction(doc, "Update Element"))
                    {
                        tx.Start();
                        ClsRevitUtil.SetParameter(doc, id, "�R�����g", "�ʔz�u�Őݒ肵���R�����g�ł�");
                        tx.Commit();
                    }
                }

                ////�C�x���g�I��������
                //ElementId idPick = null;
                //if (!ClsRevitUtil.PickObject(uidoc, "1�߂̕��N�x�[�X", "���N�x�[�X", ref idPick))
                //{
                //    return;
                //}

                //�I����Ԃɂ���
                ClsRevitUtil.SelectElement(uidoc, newIds);
            }
        }
    }
    //TESTCommand
    /// <summary>
    /// �}�ʏ��ݒ�
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CommandZumenInfo : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            try
            {
                if (!ClsProtect.bCheckProtect())
                {
                    return Result.Failed;
                }

                //DLG.DlgZumenInfo zumenInfo = new DLG.DlgZumenInfo();
                //ClsZumenInfo.GetProjectInfo(uiapp, zumenInfo);
                //DialogResult result = zumenInfo.ShowDialog();
                //if (result == DialogResult.OK)
                //{
                //    ClsZumenInfo.SaveProjectInfo(uiapp, zumenInfo);
                //}
                ClsCommandZumenInfo.CommandZumenInfo(uidoc);

            }
            catch (Exception ex)
            {
                TaskDialog.Show("Info Message2", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }
            return Result.Succeeded;
        }
    }
    [Transaction(TransactionMode.Manual)]
    public class CommandReadJson : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                if (!ClsProtect.bCheckProtect())
                {
                    return Result.Failed;
                }
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                ClsJson.ReadJOSN(doc);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("JSON�Ǎ�", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }
            return Result.Succeeded;
        }
    }
    [Transaction(TransactionMode.Manual)]
    public class ReplaceModelLineToShin : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            //Document doc = uidoc.Document;

            ClsTest.TestCreateBase(uidoc);


            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class CreateKariKiribari : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            //Document doc = uidoc.Document;



            //ClsTest.testCommand3(uidoc);


            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class testtest : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            Document doc = uidoc.Document;

            ClsTest.testCommand4(uidoc);

            return Result.Succeeded;
        }
    }
    [Transaction(TransactionMode.Manual)]
    public class FormTest : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            //UIApplication uiapp = commandData.Application;
            //UIDocument uidoc = uiapp.ActiveUIDocument;
            //Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            ////Document doc = uidoc.Document;

            //ClsTest.testCommand5(uidoc);
            //ClsTest.testCommand4(uidoc);

            //return Result.Succeeded;
            try
            {
                Application.thisApp.ShowForm(commandData.Application);

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }

        }
    }
    [Transaction(TransactionMode.Manual)]
    public class FormDlgCreateVoidFamily : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            //UIApplication uiapp = commandData.Application;
            //UIDocument uidoc = uiapp.ActiveUIDocument;
            //Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            ////Document doc = uidoc.Document;

            //ClsTest.testCommand5(uidoc);
            //ClsTest.testCommand4(uidoc);

            //return Result.Succeeded;
            try
            {
                Application.thisApp.ShowFormdlgCreateVoidFamily(commandData.Application);

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }

        }
    }
    [Transaction(TransactionMode.Manual)]
    public class CreateIntersectionFamilyCommand : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            Document doc = uidoc.Document;

            ClsTest.CreateIntersectionFamily(uidoc);

            return Result.Succeeded;
        }
    }
    [Transaction(TransactionMode.Manual)]
    public class FormDlgCreateCASETest : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                Application.thisApp.ShowForm_dlgCASETest(commandData.Application);

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }

        }
    }

    [Transaction(TransactionMode.Manual)]
    public class FormDlgTest_Kurane : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                ClsTest clsTest = new ClsTest();
                clsTest.CommandTest_Kurane(uiapp);
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class FormDlgTest_Kurane2 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                ClsTest clsTest = new ClsTest();
                ClsTest.testCommand3();
                //clsTest.CommandTest_Kurane2(uiapp);
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }

            return Result.Succeeded;
        }
    }


    [Transaction(TransactionMode.Manual)]
    public class CheckHaraokoshiIrizumi : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            Document doc = uidoc.Document;

            ClsTest.CheckIrizumi(uidoc);
            //ClsTest.ObjectReverse(uidoc);


            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class CreateSymbolCommand : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            Document doc = uidoc.Document;

            ClsTest.GetBaseCount(uidoc);
            ClsTest.ChangeKariKouzai(uidoc);
            //ClsTest.CreateSymbol(uidoc,);

            return Result.Succeeded;
        }
    }

    /// <summary>
    /// ���ʋL���ύX
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ChangeShikibetuSym : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;

            MessageBox.Show("���ʋL���ύX");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �ǐc�쐬
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateKabeshin : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            if (!ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            ClsKabeShin.CreateKabeShin(uidoc);
            //MessageBox.Show("�ǐc�쐬");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �|��쐬
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateKouyaita : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                if (!ClsProtect.bCheckProtect())
                {
                    return Result.Failed;
                }

                UIApplication uiapp = commandData.Application;

                ClsCommandKouyaita.CommandKouyaita(uiapp);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Info Message20", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �|��ύX
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ChangeKouyaita : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            MessageBox.Show("�|��ύX");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �|����]
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ReverseKouyaita : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                if (!ClsProtect.bCheckProtect())
                {
                    return Result.Failed;
                }

                UIApplication uiapp = commandData.Application;

                ClsCommandKouyaita.CommandFlipKouyaita(uiapp);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Info Message", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// ����T�R�[�i�[�ϊ�
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class MergeKoyaita : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            MessageBox.Show("����T�R�[�i�[�ϊ�");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �e�Y�쐬
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateOyakui : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                if (!ClsProtect.bCheckProtect())
                {
                    return Result.Failed;
                }

                UIApplication uiapp = commandData.Application;

                CommandCls.ClsCommandOyakui.CommandOyakui(uiapp);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Info Message3", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }
            //MessageBox.Show("�e�Y�쐬");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �e�Y�ύX
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ChangeOyakui : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            MessageBox.Show("�e�Y�ύX");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// ����쐬�i�e�Y��I�����č쐬�j
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateYokoyaita : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                if (!ClsProtect.bCheckProtect())
                {
                    return Result.Failed;
                }

                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;

                ClsCommandYokoyaita.CommandYokoyaita(uidoc);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Info Message4", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }
            //MessageBox.Show("����쐬�i�e�Y��I�����č쐬�j");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// ����ύX
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ChangeYokoyaita : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                if (!ClsProtect.bCheckProtect())
                {
                    return Result.Failed;
                }

                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;

                ClsCommandYokoyaita.CommandChangeYokoyaita(uidoc);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Info Message5", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }
            //MessageBox.Show("����ύX");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �A���Ǎ쐬
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateRenzokuKabe : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                if (!ClsProtect.bCheckProtect())
                {
                    return Result.Failed;
                }

                UIApplication uiapp = commandData.Application;

                Command.ClsCommandRenzokukabe.CommandRenzokukabe(uiapp);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Info Message6", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }
            //MessageBox.Show("�A���Ǎ쐬");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �A���ǕύX
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ChangeRenzokuKabe : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            MessageBox.Show("�A���ǕύX");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// SMW�쐬
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateSMW : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                if (!ClsProtect.bCheckProtect())
                {
                    return Result.Failed;
                }

                //YMS.DLG.DlgCreateSMW SMW = new DLG.DlgCreateSMW();
                //ClsGlobal.m_SMW = SMW;
                //DialogResult result = SMW.ShowDialog();
                //if (result == DialogResult.OK)
                //{
                //    UIApplication uiapp = commandData.Application;
                //    UIDocument uidoc = uiapp.ActiveUIDocument;
                //    ClsTest.CreateSymbol(uidoc, SMW);
                //}

                UIApplication uiapp = commandData.Application;

                CommandCls.ClsCommandSMW.CommandSMW(uiapp);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Info Message7", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }
            //MessageBox.Show("SMW�쐬");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// SMW�ύX
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ChangeSMW : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            MessageBox.Show("SMW�ύX");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �ǂ���ʂ�
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateVoidFamily : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            if (!ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            ClsCommandVoid.CommandVoid(uidoc);
            //MessageBox.Show("�ǂ���ʂ�");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// CASE�Ǘ�
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ManegeCASE : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            //clsCommandCASESetting.CommandCASESetting(uiapp);
            clsCommandCASESetting.CommandMoodLessCASESetting(uiapp);
            //DLG.DlgCASE dl = new DLG.DlgCASE();
            //dl.Show();

            return Result.Succeeded;
        }
    }
    /// <summary>
    /// TC�Y�E�\��Y�E���p�Y�E�f�ʕω��Y�쐬
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateSanbashiKui : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            if (!ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ClsCommandSanbashiKui.CommandCreateKui(uidoc);
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// TC�Y�E�\��Y�E���p�Y�E�f�ʕω��Y�ύX
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ChangeKui : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            if (!ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ClsCommandSanbashiKui.CommandChangeKui(uidoc);
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// ���N�x�[�X�쐬
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateHaraokoshiBase : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                if (!ClsProtect.bCheckProtect())
                {
                    return Result.Failed;
                }

                UIApplication uiapp = commandData.Application;
                ClsCommandHaraokoshiBase.CommandHaraokoshiBase(uiapp);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Info Message8", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }
            //MessageBox.Show("���N�x�[�X�쐬");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// ���N�x�[�X�ύX
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ChangeHaraokoshiBase : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                if (!ClsProtect.bCheckProtect())
                {
                    return Result.Failed;
                }

                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                ClsCommandHaraokoshiBase.CommandChangeHaraokoshiBase(uidoc);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Info Message9", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }
            //MessageBox.Show("���N�x�[�X�ύX");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �ؗ��x�[�X�쐬
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateKiribariBase : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                if (!ClsProtect.bCheckProtect())
                {
                    return Result.Failed;
                }

                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                ClsCommandKiribariBase.CreateKiribariBase(uidoc);
                //YMS.DLG.DlgCreateKiribari kiribari = new DLG.DlgCreateKiribari();
                //DialogResult result = kiribari.ShowDialog();
                //if (result == DialogResult.OK)
                //{
                //    UIApplication uiapp = commandData.Application;
                //    UIDocument uidoc = uiapp.ActiveUIDocument;
                //    ClsTest.CreateKiribariBase(uidoc, kiribari);
                //}
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Info Message10", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }
            //MessageBox.Show("�ؗ��x�[�X�쐬");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �ؗ��x�[�X�ύX
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ChangeKiribariBase : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                if (!ClsProtect.bCheckProtect())
                {
                    return Result.Failed;
                }

                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                ClsCommandKiribariBase.CommandChangeKiribariBase(uidoc);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Info Message11", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }
            //MessageBox.Show("�ؗ��x�[�X�ύX");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �O���s�[�X�z�u
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class PutSanjikuPiece : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            if (!ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            ClsCommandSanjikuPiece.CommandSanjikuPiece(uidoc);

            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �O���s�[�X�ʒu����
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class PositionSanjikuPiece : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            if (!ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            //ClsSanjikuPeace.MoveSanjikuPeace(uiapp);
            try
            {
                ClsCommandPositionSanjikuPiece.CommandPositionSanjikuPiece(uidoc);
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
            //MessageBox.Show("�O���s�[�X�ʒu����");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// ���ΑŃx�[�X�쐬
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateCornerHiuchiBase : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            if (!ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            ClsCommandCornerHiuchiBase.CommandCreateCornerHiuchiBase(uidoc);
            // MessageBox.Show("���ΑŃx�[�X�쐬");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// ���ΑŃx�[�X�ύX
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ChangeCornerHiuchiBase : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            if (!ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            ClsCommandCornerHiuchiBase.CommandChangeCornerHiuchiBase(uidoc);

            //MessageBox.Show("���ΑŃx�[�X�ύX");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �ؗ��ΑŃx�[�X�쐬
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateKiribariHiuchiBase : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            if (!ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ClsCommandKiribariHiuchiBase.CommandKiribariHiuchiBase(uidoc);
            //MessageBox.Show("�ؗ��ΑŃx�[�X�쐬");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �ؗ��ΑŃx�[�X�ύX
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ChangeKiribariHiuchiBase : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            if (!ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ClsCommandKiribariHiuchiBase.CommandChangeKiribariHiuchiBase(uidoc);
            //MessageBox.Show("�ؗ��ΑŃx�[�X�ύX");
            return Result.Succeeded;
        }
    }

    /// <summary>
    /// �x�[�X�R�s�[
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CopyBase : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            if (!ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ClsCommandCopyBase.CommandCopyBase(uidoc);
            return Result.Succeeded;
        }
    }

    /// <summary>
    /// �I�Y�E���ԍY�쐬
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateTanakui : IExternalCommand
    {
        public Result Execute(
               ExternalCommandData commandData,
               ref string message,
               ElementSet elements)
        {
            if (!ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ClsCommandTanakui.CommandCreateTanakui(uidoc);
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �I�Y�E���ԍY�ύX
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ChangeTanakui : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ClsCommandTanakui.CommandChangeTanakui(uidoc);
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �W���b�L�쐬
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateJack : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            if (!ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            UIApplication uiapp = commandData.Application;
            ClsCommandJack.CreateJack(uiapp);
            //MessageBox.Show("�W���b�L�쐬");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �W���b�L�ύX
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Changejack : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            if (!ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            UIApplication uiapp = commandData.Application;
            ClsCommandJack.ChangeJack(uiapp);
            //MessageBox.Show("�W���b�L�ύX");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �ؗ��󂯃x�[�X�쐬
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateKiribariUkeBase : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            if (!ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ClsCommandKiribariUkeBase.CommandKiribariUkeBase(uidoc);
            //MessageBox.Show("�ؗ��󂯃x�[�X�쐬");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �ؗ��󂯃x�[�X�ύX
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ChangeKiribariUkeBase : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            if (!ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ClsCommandKiribariUkeBase.CommandChangeKiribariUkeBase(uidoc);
            //MessageBox.Show("�ؗ��󂯃x�[�X�ύX");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �ؗ��c�i�M�ރx�[�X�쐬
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateKiribariTsunagizaiBase : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            if (!ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ClsCommandKiribariTsunagizaiBase.CommandKiribariTsunagizaiBase(uidoc);
            //MessageBox.Show("�ؗ��c�i�M�ރx�[�X�쐬");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �ؗ��c�i�M�ރx�[�X�ύX
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ChangeKiribariTsunagizaiBase : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            if (!ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ClsCommandKiribariTsunagizaiBase.CommandChangeKiribariTsunagizaiBase(uidoc);
            //MessageBox.Show("�ؗ��c�i�M�ރx�[�X�ύX");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �ΑŃc�i�M�ރx�[�X�쐬
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateHiuchiTsunagizaiBase : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            if (!ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            //MessageBox.Show("�ΑŃc�i�M�ރx�[�X�쐬");
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ClsCommandHiuchiTsunagizaiBase.CommandHiuchiTsunagizaiBase(uidoc);
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �ΑŃc�i�M�ރx�[�X�ύX
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ChangeHIuchiTsunagizaiBase : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            if (!ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ClsCommandHiuchiTsunagizaiBase.CommandChangeHiuchiTsunagizaiBase(uidoc);
            //MessageBox.Show("�ΑŃc�i�M�ރx�[�X�ύX");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �ؗ��p���x�[�X�ύX
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ChangeKiribariTsunagiBase : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            if (!ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ClsCommandKiribariTsugiBase.CommandChangeKiribariTsugiBase(uidoc);
            //MessageBox.Show("�ؗ��p���x�[�X�ύX");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �ؗ��p���x�[�X�쐬
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateKiribariTsunagiBase : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            if (!ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ClsCommandKiribariTsugiBase.CommandKiribariTsugiBase(uidoc);
            //MessageBox.Show("�ؗ��p���x�[�X�쐬");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �Η��̋�̗p�����z�u
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class PutSyabariStructureLine : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                if (!ClsProtect.bCheckProtect())
                {
                    return Result.Failed;
                }

                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;

                ClsCommandSyabariBase.CommandPutSyabariStructureLine(uidoc);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Info Message10", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }
            MessageBox.Show("�Η��̋�̗p�����z�u");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �Η��̒[�����i�z�u�i�ʁj
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class PutSyabariPieceIndividual : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                if (!ClsProtect.bCheckProtect())
                {
                    return Result.Failed;
                }

                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;

                ClsCommandSyabariBase.CommandPutSyabariPieceIndividual(uidoc);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Info Message10", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }
            //MessageBox.Show("�Η��̒[�����i�z�u�i�ʁj");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �Η��̒[�����i�z�u�i��_�j
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class PutSyabariPieceIntersectionPoint : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                if (!ClsProtect.bCheckProtect())
                {
                    return Result.Failed;
                }

                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;


                ClsCommandSyabariBase.CommandPutSyabariPieceIntersectionPoint(uidoc);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Info Message10", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }
            //MessageBox.Show("�Η��̒[�����i�z�u�i��_�j");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �Η��x�[�X�쐬
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateSyabariBase : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                if (!ClsProtect.bCheckProtect())
                {
                    return Result.Failed;
                }

                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                ClsCommandSyabariBase.CommandCreateSyabariBase(uidoc);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Info Message11", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }
            //MessageBox.Show("�Η��x�[�X�쐬");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �Η��x�[�X�ύX
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ChangeSyabariBase : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                if (!ClsProtect.bCheckProtect())
                {
                    return Result.Failed;
                }

                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                ClsCommandSyabariBase.CommandChangeSyabariBase(uidoc);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Info Message11", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }
            //MessageBox.Show("�Η��x�[�X�ύX");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �Η��c�i�M�ރx�[�X�쐬
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateSyabariTsunagizaiBase : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                if (!ClsProtect.bCheckProtect())
                {
                    return Result.Failed;
                }

                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                ClsCommandSyabariTsunagizaiBase.CommandCreateSyabariTsunagizaiBase(uidoc);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Info Message11", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }
            //MessageBox.Show("�Η��c�i�M�ރx�[�X�쐬");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �Η��c�i�M�ރx�[�X�ύX
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ChangeSyabariTsunagizaiBase : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                if (!ClsProtect.bCheckProtect())
                {
                    return Result.Failed;
                }

                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                ClsCommandSyabariTsunagizaiBase.CommandChangeSyabariTsunagizaiBase(uidoc);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Info Message11", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }
            //MessageBox.Show("�Η��c�i�M�ރx�[�X�ύX");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �Η��󂯃x�[�X�쐬
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateSyabariUkeBase : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                if (!ClsProtect.bCheckProtect())
                {
                    return Result.Failed;
                }

                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                ClsCommandSyabariUkeBase.CommandCreateSyabariUkeBase(uidoc);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Info Message11", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }
            //MessageBox.Show("�Η��󂯃x�[�X�쐬");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �Η��󂯃x�[�X�ύX
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ChangeSyabariUkeBase : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                if (!ClsProtect.bCheckProtect())
                {
                    return Result.Failed;
                }

                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                ClsCommandSyabariUkeBase.CommandChangeSyabariUkeBase(uidoc);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Info Message11", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }
            //MessageBox.Show("�Η��󂯃x�[�X�ύX");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �Η��Αł̒[�����i�z�u
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class PutSyabariHiuchiPiece : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                if (!ClsProtect.bCheckProtect())
                {
                    return Result.Failed;
                }

                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                //ClsCommandSyabariBase.CommandPutSyabariHiuchiPiece(uidoc);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Info Message10", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }
            MessageBox.Show("�Η��Αł̒[�����i�z�u");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �Η��ΑŃx�[�X�쐬
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateSyabariHiuchiBase : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                if (!ClsProtect.bCheckProtect())
                {
                    return Result.Failed;
                }

                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                ClsCommandSyabariHiuchiBase.CommandCreateSyabariHiuchiBase(uidoc);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Info Message11", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }
            //MessageBox.Show("�Η��ΑŃx�[�X�쐬");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �Η��ΑŃx�[�X�ύX
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ChangeSyabariHiuchiBase : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                if (!ClsProtect.bCheckProtect())
                {
                    return Result.Failed;
                }

                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                ClsCommandSyabariHiuchiBase.CommandChangeSyabariHiuchiBase(uidoc);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Info Message11", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }
            //MessageBox.Show("�Η��ΑŃx�[�X�ύX");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// ���c�i�M�ލ쐬
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateAtamaTsunagiZai : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            if (!ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ClsCommandAtamaTsunagi.CommandCreateAtamaTsunagiZai(uidoc);
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// ���c�i�M�⏕�ލ쐬
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateAtamaTsunagiHojoZai : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            if (!ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ClsCommandAtamaTsunagi.CommandCreateAtamaTsunagiHojoZai(uidoc);
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// ���c�i�M�폜
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class DeleteAtamaTsunagi : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            if (!ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ClsCommandAtamaTsunagi.CommandDeleteAtamaTsunagi(uidoc);
            //MessageBox.Show("���c�i�M�폜");
            return Result.Succeeded;
        }
    }

    /// <summary>
    /// �o�����⋭�ލ쐬
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateDesumibuHokyouzai : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                if (!ClsProtect.bCheckProtect())
                {
                    return Result.Failed;
                }

                UIApplication uiapp = commandData.Application;

                ClsCommandDesumibuHokyouzai.CommandCreateDesumibuHokyouzai(uiapp);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Info Message", ex.Message);
                message = ex.Message;
                return Result.Failed;
            }
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �o�����⋭�ޕύX
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ChangeDesumibuHokyouzai : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            MessageBox.Show("�o�����⋭�ޕύX");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// ���x�[�X�쐬
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateKariBase : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            MessageBox.Show("���x�[�X�쐬");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �x�[�X�ꗗ�E�ҏW
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class EditBaseList : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            if (!ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            UIApplication uiapp = commandData.Application;
            ClsCommandBaseList.CommandBaseList(uiapp);

            return Result.Succeeded;
        }
    }
    /// <summary>
    /// ���|�ޔz�u
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class PutKariKouzai : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            if (!ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            //ClsTest.CreataKariKouzai(uidoc);
            //MessageBox.Show("���|�ޔz�u");
            ClsCommandKariKozai.CommandCreateKariKouzai(uidoc);

            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �����s�[�X�쐬
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateSumibuPiace : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            if (!ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            ClsCommandSumibuPiece.CommandSumibuPiece(uidoc);

            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �����s�[�X�ύX
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ChangeSumibuPiace : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            if (!ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            ClsCommandSumibuPiece.CommandChangeSumibuPiece(uidoc);

            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �u���P�b�g�i�P�Ɓj�쐬
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateBracket : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            if (!ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ClsCommandBracket.CommandCreateBracket(uidoc);
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// ����f�{���g�⋭�ލ쐬
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateSendanBoltHokyouzai : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            if (!ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ClsCommandSendanBoltHokyouzai.CommandCreateSendanBoltHokyouzai(uidoc);
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// ����f�{���g�⋭�ޕύX
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ChangeSendanBoltHokyouzai : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            MessageBox.Show("����f�{���g�⋭�ޕύX");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �㉺���N�c�i�M�ލ쐬
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateJyougeHaraokoshiTsunagi : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            MessageBox.Show("�㉺���N�c�i�M�ލ쐬");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �㉺���N�c�i�M�ޕύX
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ChangeJyougeHaraokoshiTsunagi : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            MessageBox.Show("�㉺���N�c�i�M�ޕύX");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// ���N�X�x���~�ߍ쐬
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateHaraokoshiSuberidome : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            if (!ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            UIApplication uiapp = commandData.Application;
            ClsCommandHaraokoshiSuberidome.CreateHaraokoshiSuberidome(uiapp);
            //MessageBox.Show("���N�X�x���~�ߍ쐬");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// ���N�X�x���~�ߕύX
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ChangeHaraokoshiSuberidome : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            if (!ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            UIApplication uiapp = commandData.Application;
            ClsCommandHaraokoshiSuberidome.ChangeHaraokoshiSuberidome(uiapp);
            //MessageBox.Show("���N�X�x���~�ߕύX");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// ���������莩���쐬
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateKousyabuMawari : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            if (!ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            UIApplication uiapp = commandData.Application;
            ClsCommandKousyabuMawari.CreateKousyabuMawari(uiapp);
            //ClsCommandKousyabuMawari.CreateKousyabuMawariSelectBase(uiapp);
            //MessageBox.Show("���������莩���쐬");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// ���t�p�s�[�X�쐬
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateShimetukePiece : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            if (!ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            UIApplication uiapp = commandData.Application;
            ClsCommandShimetukePiece.CommandCreateShimetukePiece(uiapp);
            //MessageBox.Show("���t�p�s�[�X�쐬");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �X�`�t�i�[�쐬
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateStiffener : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            if (!ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            UIApplication uiapp = commandData.Application;
            ClsCommandStiffener.CreateStiffener(uiapp);
            //MessageBox.Show("�X�`�t�i�[�쐬");
            return Result.Succeeded;
        }
    }
    /// <summary>
    /// �蓮���t
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class WaritukeSyudou : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                if (!ClsProtect.bCheckProtect())
                {
                    return Result.Failed;
                }

                UIApplication uiapp = commandData.Application;

                ClsCommandWarituke.CommandWarituke(uiapp);
            }
            catch (Exception ex)
            {
                //TaskDialog.Show("Info Message12", ex.Message);
                //message = ex.Message;
                return Result.Failed;
            }
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class WaritukeJidou : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                if (!ClsProtect.bCheckProtect())
                {
                    return Result.Failed;
                }

                UIApplication uiapp = commandData.Application;

                ClsCommandWarituke.CommandJidouWarituke(uiapp);
            }
            catch (Exception ex)
            {
                //TaskDialog.Show("Info Message12", ex.Message);
                //message = ex.Message;
                return Result.Failed;
            }
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class DeleteWarituke : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                if (!ClsProtect.bCheckProtect())
                {
                    return Result.Failed;
                }

                UIApplication uiapp = commandData.Application;
                ClsCommandWarituke.CommandDeleteWarituke(uiapp);
            }
            catch (Exception ex)
            {
                //TaskDialog.Show("Info Message12", ex.Message);
                //message = ex.Message;
                return Result.Failed;
            }
            return Result.Succeeded;
        }
    }

    /// <summary>
    /// �|�ޒ����ύX
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ChangeLengthKouzai : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            ClsCommandChangeKouzaiLength.CommandChangeKouzaiLength(uidoc);

            return Result.Succeeded;
        }
    }
    /// <summary>
    /// ���N�u���P�b�g�쐬
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateHaraokoshiBracket : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            MessageBox.Show("���N�u���P�b�g�쐬");
            return Result.Succeeded;
        }
    }

    /// <summary>
    ///�@���[�N�Z�b�g�쐬
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateWorkset : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            //ClsWorkset ws = new ClsWorkset();
            //ws.CreateWorkset(doc);

            return Result.Succeeded;
        }
    }

    /// <summary>
    /// �ʔz�u
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class KobetsuHaichi : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;

            if (!ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            ClsCommandKobetsuHaichi.CommandKobetsuHaichi(uiapp);


            return Result.Succeeded;
        }
    }

    //KiribariCopy
    [Transaction(TransactionMode.Manual)]
    public class KiribariCopy : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;

            if (!ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            ClsCommandCopyKiribari.CommandCopyKiribari(uidoc);


            return Result.Succeeded;
        }
    }

    /// <summary>
    /// �x���\���̐���(�����ł���x�����\������Ȃ��Ȃ�)
    /// </summary>
    /// <remarks>���L�̂悤�Ɏg��
    /// t.Start();
    ///FailureHandlingOptions failOpt = t.GetFailureHandlingOptions();
    ///failOpt.SetFailuresPreprocessor(new WarningSwallower());
    ///t.SetFailureHandlingOptions(failOpt);</remarks>
    public class WarningSwallower : IFailuresPreprocessor
    {
        public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
        {
            IList<FailureMessageAccessor> failList = new List<FailureMessageAccessor>();
            failList = failuresAccessor.GetFailureMessages();
            foreach (FailureMessageAccessor failure in failList)
            {
                FailureDefinitionId failID = failure.GetFailureDefinitionId();
                failuresAccessor.DeleteWarning(failure);
            }

            return FailureProcessingResult.Continue;
        }
    }
}
