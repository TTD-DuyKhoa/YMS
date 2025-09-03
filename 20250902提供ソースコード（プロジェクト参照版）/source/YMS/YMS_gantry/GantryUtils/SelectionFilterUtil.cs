using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YMS_gantry.Material;
using YMS_gantry.UI;
using static YMS_gantry.DefineUtil;

namespace YMS_gantry
{
    class SelectionFilterUtil
    {
        UIDocument uiDoc { get; set; }
        List<string> CategoryList { get; set; }

        public SelectionFilterUtil(UIDocument _uiDoc, List<string> categories)
        {
            uiDoc = _uiDoc;
            CategoryList = categories;
        }

        /// <summary>
        /// 複数選択
        /// </summary>
        /// <param name="prmpt"></param>
        /// <param name="retList"></param>
        /// <returns></returns>
        public bool Selection(string prmpt, out List<FamilyInstance> retList)
        {
            retList = new List<FamilyInstance>();
            bool flag = true;

            while (flag)
            {
                if (retList.Count > 0)
                {
                    List<ElementId> selList = retList.Select(x => x.Id).ToList();
                    RevitUtil.ClsRevitUtil.SelectElement(uiDoc, selList);
                }

                try
                {
                    YMSPickFilter pickFilter = new YMSPickFilter(uiDoc.Document, CategoryList);
                    Reference reference = uiDoc.Selection.PickObject(ObjectType.Element, pickFilter, "部材を選択して下さい　選択終了はEscキーを押下してください");
                    retList.Add(uiDoc.Document.GetElement(reference.ElementId) as FamilyInstance);
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                {
                    flag = false;
                }
                catch(Exception ex)
                {
                    flag = false;
                }
            }

            return true;
        }

        /// <summary>
        /// 単一選択
        /// </summary>
        public bool Select(string prmpt, out FamilyInstance retIns,string fmailyHeadSt= "")
        {
            try
            {
                Selection selection = uiDoc.Selection;
                YMSPickFilter pickFilter = new YMSPickFilter(uiDoc.Document, CategoryList);
                Reference select= selection.PickObject(ObjectType.Element, pickFilter, prmpt);
                retIns = uiDoc.Document.GetElement(select.ElementId) as FamilyInstance;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                retIns = null;
                return false;
            }
            return true;
        }
    }

    class YMSPickFilter : ISelectionFilter
    {
        Document doc { get; set; }

        /// <summary>
        /// 中分類のリスト
        /// </summary>
        List<string> CategoryList { get; set; }

        /// <summary>
        /// 面指定か
        /// </summary>
        bool IsFace { get; set; }

        public YMSPickFilter(Document _doc, List<string> categories,bool isFace=false)
        {
            doc = _doc;
            CategoryList = categories;
            IsFace = isFace;
        }

        public bool AllowElement(Element elem)
        {
            if (elem.Category == null) { return false; }
            if (elem.Category.CategoryType != CategoryType.Model) { return false; }
            try
            {
                FamilyInstance instance = doc.GetElement(elem.Id) as FamilyInstance;
                if (instance == null) { return false; }
                FamilySymbol sym = instance.Symbol;
                //Parameter prm = sym.LookupParameter("小分類");
                //if (prm == null) { return false; }
                string cate=GantryUtil.GetOriginalTypeName(doc, instance);

                //string cate = prm.AsString();
                if (string.IsNullOrEmpty(cate) || cate == null) { return false; }
                if (CategoryList.Contains(cate)) { return true; }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            if(IsFace)
            {
                Element elem = doc.GetElement(reference.ElementId);

                if (elem.Category == null) { return false; }
                if (elem.Category.CategoryType != CategoryType.Model) { return false; }
                try
                {
                    FamilyInstance instance = doc.GetElement(elem.Id) as FamilyInstance;
                    if (instance == null) { return false; }
                    FamilySymbol sym = instance.Symbol;
                    //Parameter prm = sym.LookupParameter("小分類");
                    //if (prm == null) { return false; }
                    string cate = GantryUtil.GetOriginalTypeName(doc, instance);

                    //string cate = prm.AsString();
                    if (string.IsNullOrEmpty(cate) || cate == null) { return false; }
                    if (CategoryList.Contains(cate)) { return true; }
                    else
                    {
                        return false;
                    }


                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            throw new NotImplementedException();
        }
    }

    class YMSWaritsukeFilter : ISelectionFilter
    {
        Document doc { get; set; }

        public YMSWaritsukeFilter(Document _doc)
        {
            doc = _doc;
        }

        public bool AllowElement(Element elem)
        {
            if (elem.Category == null) { return false; }
            if (elem.Category.CategoryType != CategoryType.Model) { return false; }
            try
            {
                FamilyInstance instance = doc.GetElement(elem.Id) as FamilyInstance;
                if (instance == null) { return false; }
                MaterialSuper ms = MaterialSuper.ReadFromElement(elem.Id, doc);

                if (ms == null) { return false; }
                string originaiType = GantryUtil.GetOriginalTypeName(doc, instance);
                if (ms.GetType() == typeof(Ohbiki) || ms.GetType() == typeof(Ohbiki) || ms.GetType() == typeof(Shikigeta) || ms.GetType() == typeof(PutFukkougeta) ||
                    ms.GetType() == typeof(HorizontalJoint)|| ms.GetType() == typeof(Tesuri)||ms.GetType()==typeof(Neda))
                {
                    return true;
                }
                else if (ms.GetType() == typeof(JifukuZuredomezai))
                {
                    return originaiType == Master.ClsJifukuZuredomeCsv.TypeJifuku;
                }
                else if(ms.GetType() == typeof(Houdue))
                {
                    if(ms.m_Size.Contains("HA_") || ms.m_Size.Contains("SMH_"))
                    {
                        return true;
                    }
                    return ((Houdue)ms).m_Syuzai;
                }
                else if(ms.GetType() == typeof(PilePiller))
                {
                    return originaiType == PilePiller.sichuTypeName;
                }else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            throw new NotImplementedException();
        }

    }

    class YMSReckonKiribariUkeAsHolJoinFilter : ISelectionFilter
    {
        Document doc { get; set; }

        public YMSReckonKiribariUkeAsHolJoinFilter(Document _doc)
        {
            doc = _doc;
        }

        public bool AllowElement(Element elem)
        {
            if (elem.Category == null) { return false; }
            if (elem.Category.CategoryType != CategoryType.Model) { return false; }
            try
            {
                FamilyInstance instance = doc.GetElement(elem.Id) as FamilyInstance;
                if (instance == null) { return false; }

                FamilySymbol symbol = instance.Symbol;
                if(symbol==null) { return false; }
                Family f = symbol.Family;
                if (f == null) { return false; }

                if(symbol.Name.StartsWith("切梁受"))
                {
                    if (f.Name.StartsWith("形鋼_C"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            throw new NotImplementedException();
        }

    }
}
