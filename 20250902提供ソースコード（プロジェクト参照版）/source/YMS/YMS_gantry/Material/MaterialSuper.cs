using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YMS_gantry;

namespace YMS_gantry
{
    public abstract class MaterialSuper
    {
        public static string Name = "MaterialSuper";
        public static string typeName = "";

        [MaterialProperty("構台名")]
        public string m_KodaiName { get; set; }

        [MaterialProperty("材質")]
        public string m_Material { get; set; }

        [MaterialProperty("サイズ")]
        public string m_Size { get; set; }

        [MaterialProperty("締結タイプ")]
        public DefineUtil.eJoinType m_TeiketsuType { get; set; } = DefineUtil.eJoinType.None;

        [MaterialProperty("ボルト情報1")]
        public string m_BoltInfo1 { get; set; } = "";

        [MaterialProperty("ボルト情報2")]
        public string m_BoltInfo2 { get; set; } = "";

        [MaterialProperty("ボルト1本数")]
        public int m_Bolt1Cnt { get; set; } = 0;

        [MaterialProperty("ボルト2本数")]
        public int m_Bolt2Cnt { get; set; } = 0;

        public ElementId m_ElementId { get; set; } = ElementId.InvalidElementId;

        public Document m_Document { get; set; }

        /// <summary>
        /// ファミリインスタンスの基点を返す
        /// </summary>
        /// <returns></returns>
        public XYZ Position
        {
            get
            {
                var elem = m_Document.GetElement(m_ElementId);
                if (elem is FamilyInstance familyInstance)
                {
                    if (familyInstance.Location is LocationCurve locationCurve)
                    {
                        return locationCurve.Curve?.GetEndPoint(0);
                    }
                    else if (familyInstance.Location is LocationPoint locationPoint)
                    {
                        return locationPoint.Point;
                    }
                }
                return XYZ.Zero;
            }
        }

        public XYZ Center
        {
            get
            {
                var elem = m_Document.GetElement(m_ElementId);
                if (elem is FamilyInstance familyInstance)
                {
                    var bbox = elem.get_BoundingBox(null);
                    if (bbox != null)
                    {
                        return 0.5 * bbox.Min + 0.5 * bbox.Max;
                    }
                }
                return null;
            }
        }

        public Line CenterLine
        {
            get
            {
                var elem = m_Document.GetElement(m_ElementId);
                if (elem is FamilyInstance familyInstance)
                {
                    if (familyInstance.Location is LocationCurve locationCurve)
                    {
                        return locationCurve.Curve as Line;
                    }
                    else if (familyInstance.Location is LocationPoint locationPoint)
                    {
                        var transform = familyInstance.GetTransform();
                        var startPt = locationPoint.Point;
                        var lengthParam = familyInstance.Parameters.Cast<Parameter>().FirstOrDefault(x => x.Definition.Name == DefineUtil.PARAM_LENGTH);
                        var lengthFeet = lengthParam.AsDouble();
                        var directionFeet = transform.BasisX;
                        var endPt = startPt + lengthFeet * directionFeet;

                        return Line.CreateBound(startPt, endPt);
                    }
                }
                return null;
            }
        }

        public SteelSize SteelSize
        {
            get
            {
                var familyInstance = m_Document.GetElement(m_ElementId) as FamilyInstance;
                var familyName = familyInstance?.Symbol?.Family?.Name;
                if (familyName == null) { return new SteelSize(); }
                return GantryUtil.GetKouzaiSizeSunpou(familyName);
            }
        }

        public MaterialSize MaterialSize()
        {
            var familyInstance = m_Document.GetElement(m_ElementId) as FamilyInstance;
            var familyName = familyInstance?.Symbol?.Family?.Name;
            if (familyName == null) { return null; }
            return GantryUtil.GetKouzaiSize(familyName);
        }

        public static ElementId PlaceWithTwoPoints(FamilySymbol symbol, Reference reference, XYZ startPt, XYZ endPt, double levelOffsetFeet,bool noOffset=false)
        {
            var doc = symbol.Document;
            var family = symbol.Family;
            if (!symbol.IsActive)
            {
                symbol.Activate();
            }
            if (startPt == null || endPt == null) { return ElementId.InvalidElementId; }
            if (family.FamilyPlacementType == FamilyPlacementType.WorkPlaneBased)
            {
                var instance = doc.Create.NewFamilyInstance(reference, startPt, endPt - startPt, symbol);

                ClsRevitUtil.SetParameter(doc, instance.Id, DefineUtil.PARAM_LENGTH, startPt.DistanceTo(endPt));
               
                var id = instance.Id;
                var element = doc.GetElement(reference);
                if (element is Level level)
                {
                    if (!noOffset)
                    {
                        ClsRevitUtil.SetParameter(doc, instance.Id, DefineUtil.PARAM_BASE_OFFSET, levelOffsetFeet);
                    }
                }
                else
                {
                    if (!noOffset)
                    {
                        ClsRevitUtil.SetParameter(doc, instance.Id, DefineUtil.PARAM_HOST_OFFSET, levelOffsetFeet);
                    }
                }
                    // ここで一回移動などをしないと Location が原点のまま返ってくる。
                    ElementTransformUtils.MoveElement(doc, id, XYZ.Zero);
                //var qqq = doc.GetElement(id) as FamilyInstance;
                //if (qqq.Location is LocationPoint a)
                //{
                //    var aa = a.Point;
                //    var bb = a.Rotation;
                //}
                return id;
            }
            else if (family.FamilyPlacementType == FamilyPlacementType.CurveBased)
            {
                var element = doc.GetElement(reference);
                if (element is Level level)
                {
                    var lineSeg = Line.CreateBound(startPt, endPt);
                    var instance = doc.Create.NewFamilyInstance(lineSeg, symbol, level, StructuralType.NonStructural);
                    if (!noOffset)
                    {
                        ClsRevitUtil.SetParameter(doc, instance.Id, DefineUtil.PARAM_HOST_OFFSET, levelOffsetFeet);
                    }
                    var id = instance.Id;
                    ElementTransformUtils.MoveElement(doc, id, XYZ.Zero);
                    return id;
                }
                else if (element is ReferencePlane plane)
                {
                    var lineSeg = Line.CreateBound(startPt, endPt);
                    var instance = doc.Create.NewFamilyInstance(lineSeg, symbol, (Level)doc.GetElement(element.LevelId), StructuralType.NonStructural);
                    if (!noOffset)
                    {
                        ClsRevitUtil.SetParameter(doc, instance.Id, DefineUtil.PARAM_HOST_OFFSET, levelOffsetFeet);
                    }
                    var id = instance.Id;
                    ElementTransformUtils.MoveElement(doc, id, XYZ.Zero);
                    return id;
                }
            }
            return ElementId.InvalidElementId;
        }

        public MaterialSuper()
        {

        }

        public MaterialSuper(ElementId _id, string _koudaiName, string _material, string _size)
        {
            this.m_ElementId = _id;
            this.m_KodaiName = _koudaiName;
            this.m_Size = _size;
            this.m_Material = _material;
        }

        //public (Level level, double levelOffsetMm) GetLevelAndOffset()
        //{
        //    var familyInstance = m_Document.GetElement(m_ElementId) as FamilyInstance;
        //    var levelOffset = 0.0;
        //    var level = GantoryUtil.GetInstanceLevelAndOffset(m_Document, familyInstance, ref levelOffset);
        //    return (level, levelOffset);
        //}

        public double LevelOffsetFeet
        {
            get
            {
                var familyInstance = m_Document.GetElement(m_ElementId) as FamilyInstance;
                var a = 0.0;
                GantryUtil.GetInstanceLevelAndOffset(m_Document, familyInstance, ref a);
                //var value = familyInstance.Parameters
                //    .Cast<Parameter>()
                //    .FirstOrDefault(x => x.Definition.Name == DefineUtil.PARAM_HOST_OFFSET)
                //    .AsDouble();
                return ClsRevitUtil.CovertToAPI(a);
            }
        }

        public Level HostLevel
        {
            get
            {
                var familyInstance = m_Document.GetElement(m_ElementId) as FamilyInstance;
                var a = 0.0;
                return GantryUtil.GetInstanceLevelAndOffset(m_Document, familyInstance, ref a);
            }
        }

        private static string CategoryKey => "部材カテゴリ";
        private static PropertyInfo[] GetPropertiesRecursive(Type type, BindingFlags bindingFlags)
        {
            var result = new List<PropertyInfo>();
            result.AddRange(type.GetProperties());
            //var parentType = type.BaseType;
            //if (parentType != null)
            //{
            //    result.AddRange(GetPropertiesRecursive(parentType, bindingFlags));
            //}
            return result.ToArray();
        }
        private static MaterialSuper FromDictionary(Dictionary<string, string> map)
        {
            try
            {
                if (!map.TryGetValue(CategoryKey, out var materialKey))
                {
                    return null;
                }
                var result = UtilAttribute<MaterialCategoryAttribute>.CreateFrom<MaterialSuper>(materialKey);
                if (result == null) { return null; }

                var resultType = result.GetType();
                foreach (var property in GetPropertiesRecursive(resultType, BindingFlags.Instance))
                {
                    var attribute = property.GetCustomAttributes<MaterialPropertyAttribute>()?.FirstOrDefault();
                    if (attribute == null) { continue; }
                    var key = attribute.Name;
                    if (!map.TryGetValue(key, out var valueString)) { continue; }
                    var propertyType = property.GetMethod.ReturnType;
                    object value = null;
                    if (propertyType == typeof(string))
                    {
                        value = valueString;
                    }
                    else if (propertyType.BaseType == typeof(Enum))
                    {
                        value = Enum.Parse(propertyType, valueString);
                    }
                    else if (propertyType == typeof(int))
                    {
                        if (int.TryParse(valueString, out var valueInt))
                        {
                            value = valueInt;
                        }
                    }
                    else if (propertyType == typeof(double))
                    {
                        if (double.TryParse(valueString, out var valueDouble))
                        {
                            value = valueDouble;
                        }
                    }
                    else if (propertyType == typeof(bool))
                    {
                        if (bool.TryParse(valueString, out var valueBool))
                        {
                            value = valueBool;
                        }
                    }

                    if (value == null) { continue; }
                    property.SetValue(result, value);
                }
                return result;
            }
            catch
            {
                return null;
            }
        }
        private static Dictionary<string, string> ToDictionary(MaterialSuper material)
        {
            var defaultResult = new Dictionary<string, string>();
            try
            {
                var result = new Dictionary<string, string>();

                var materialType = material?.GetType();
                var materialAttribute = materialType?.GetCustomAttribute<MaterialCategoryAttribute>(false);
                if (materialAttribute == null) { return defaultResult; }
                result.Add(CategoryKey, materialAttribute.Name);

                var properties = GetPropertiesRecursive(materialType, BindingFlags.Instance)
                    .ToArray();

                foreach (var property in properties)
                {
                    var attribute = property?.GetCustomAttribute<MaterialPropertyAttribute>();
                    if (attribute == null) continue;
                    var value = property.GetValue(material);
                    if (value == null) { continue; }
                    var key = attribute.Name;

                    string valueString = value.ToString();
                    result.Add(key, valueString);

                    //var propertyType = property.GetMethod.ReturnType;
                    //if (propertyType == typeof(string))
                    //{
                    //    valueString = value.ToString();
                    //}
                    //else if (propertyType.BaseType == typeof(Enum))
                    //{

                    //}
                    //else if (propertyType == typeof(int))
                    //{
                    //    valueString = value.ToString();
                    //}
                    //else if (propertyType == typeof(double))
                    //{
                    //    valueString = value.ToString();
                    //}
                    //else if (propertyType == typeof(bool))
                    //{
                    //    valueString = ((bool)value) ? "1" : "0";
                    //}
                }
                return result;
            }
            catch
            {
                return defaultResult;
            }
        }

        /// <summary>
        /// ファミリインスタンスパラメータから情報を読み込み MaterialSuper へ展開
        /// </summary>
        /// <param name="id"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static MaterialSuper ReadFromElement(ElementId id, Document doc)
        {
            var lineString = RevitUtil.ClsRevitUtil.GetInstMojiParameter(doc, id, DefineUtil.PARAM_MATERIAL_DETAIL);
            if (string.IsNullOrWhiteSpace(lineString)) { return null; }
            var dictionary = GantryUtil.CreateDictionaryFromString(lineString);
            var result = FromDictionary(dictionary);
            if (result == null) { return null; }
            result.m_ElementId = id;
            result.m_Document = doc;
            return result;
        }

        /// <summary>
        /// MaterialSuper の情報をファミリインスタンスのパラメータへ書き込み
        /// </summary>
        /// <param name="material"></param>
        /// <param name="doc"></param>
        public static void WriteToElement(MaterialSuper material, Document doc)
        {
            if (material == null || doc == null || material.m_ElementId == ElementId.InvalidElementId) return;
            var dictionary = ToDictionary(material);
            var lineString = GantryUtil.CombineDictionaryToString(dictionary);
            RevitUtil.ClsRevitUtil.SetMojiParameter(doc, material.m_ElementId, DefineUtil.PARAM_MATERIAL_DETAIL, lineString);
        }

        /// <summary>
        /// RVT 上の全ての Material を収集
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static MaterialSuper[] Collect(Document document)
        {
            return new FilteredElementCollector(document)
                .OfClass(typeof(FamilyInstance))
                .Select(x => ReadFromElement(x.Id, document))
                .Where(x => x != null)
                .ToArray();
        }

        /// <summary>
        /// RVT 上の全ての 構台に所属しない Material を収集
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static List<ElementId> CollectOtherMaterial(Document document)
        {
            return new FilteredElementCollector(document)
                .OfClass(typeof(FamilyInstance))
                .Select(x => x.Id)
                .Where(x => IsFromOtherElement(x,document))
                .ToList();
        }

        public static bool IsFromOtherElement(ElementId id, Document doc)
        {
            var lineString = RevitUtil.ClsRevitUtil.GetInstMojiParameter(doc, id, DefineUtil.PARAM_MATERIAL_DETAIL);
            return string.IsNullOrWhiteSpace(lineString);
        }
    }
}
