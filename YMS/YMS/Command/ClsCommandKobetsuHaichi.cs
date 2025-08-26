using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YMS.Parts;
using RevitUtil;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI.Events;

namespace YMS.Command
{
    public class ClsCommandKobetsuHaichi
    {
        public static bool CommandKobetsuHaichi(UIApplication uiApp, bool yamadome = true)
        {
            UIDocument uidoc = uiApp.ActiveUIDocument;
            Document doc = uidoc.Document;

            DLG.DlgCreateKobetsuHaichi dlg = new DLG.DlgCreateKobetsuHaichi(yamadome);
            DialogResult result = dlg.ShowDialog();

            if (result != DialogResult.OK)
            {
                return false ;
            }

            string familyPath = dlg.GetFamilyPath();
            string familyType = dlg.GetFamilyType();
            if (string.IsNullOrEmpty(familyType))
            {
                //ファミリタイプが存在しない場合、タイプ名をシンボル名に変更
                familyType = ClsRevitUtil.GetFamilyName(familyPath);
            }

            //↓#31788
            //if (!ClsRevitUtil.LoadFamilyData(doc, familyPath,  out Family family))
            //{
            //    MessageBox.Show("ファミリの取得に失敗しました");
            //    return false;
            //}

            //string familyName = RevitUtil.ClsRevitUtil.GetFamilyName(familyPath);

            //FamilySymbol sym = (ClsRevitUtil.GetFamilySymbol(doc, familyName, familyType));
            //↑#31788

            if (!ClsRevitUtil.LoadFamilySymbolData(doc, familyPath, familyType, out FamilySymbol sym))
            {
                MessageBox.Show("ファミリの取得に失敗しました");
                return false;
            }
            if (sym == null)
            {
                MessageBox.Show("ファミリの取得に失敗しました");
                
                return false;
            }

            ////配置されているオブジェクトを保持
            //FilteredElementCollector collector = new FilteredElementCollector(doc);　//#34229
            //ICollection<ElementId> preElementIds = collector.WhereElementIsNotElementType().ToElementIds();//#34229
            //preElementIds2 = preElementIds;//#34229

            ElementType et = doc.GetElement(sym.Id) as ElementType;
            uidoc.PostRequestForElementTypePlacement(et);

            //uiApp.Idling += OnIdling;//#34229

            return true;
        }

        private static ICollection<ElementId> preElementIds2;

        /// <summary>
        /// 個別配置時の追加IDに対する処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnIdling(object sender, IdlingEventArgs e)
        {
            UIApplication uiApp = sender as UIApplication;
            UIDocument uidoc = uiApp.ActiveUIDocument;
            Document doc = uidoc.Document;

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ICollection<ElementId> currentIds = collector.WhereElementIsNotElementType().ToElementIds();

            // 新規要素の検出
            var newIds = currentIds.Except(preElementIds2).ToList();
            if (newIds.Count > 0)
            {
                // 配置された要素に対して処理を行う
                foreach (ElementId id in newIds)
                {
                    Element newElement = doc.GetElement(id);
                    using (Transaction tx = new Transaction(doc, "Update Element"))
                    {
                        tx.Start();

                        MessageBox.Show("ここでパラメータセット");

                        tx.Commit();
                    }
                }

                // イベント解除
                uiApp.Idling -= OnIdling;
            }
        }
    }
}
