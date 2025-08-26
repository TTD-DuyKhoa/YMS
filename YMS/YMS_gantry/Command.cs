#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using YMS_gantry.Command;
using YMS_gantry.UI;

#endregion

namespace YMS_gantry
{
    /// <summary>
    /// [一括配置] 構台(フラット)配置
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class AllPutKoudaiFlat : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
          ref string message, ElementSet elements)
        {
            //プロテクトキット用処理
            if (!YMS.ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;

            //using (var f = new FrmAllPutKoudaiFlat())
            //{
            //    f.ShowDialog();
            //}
            CmdCreateKoudaiFlat cmd = new CmdCreateKoudaiFlat(uidoc);
            cmd.Excute();

            return Result.Succeeded;
        }
    }

    ///// <summary>
    ///// [一括配置] 構台(特殊形状)配置
    ///// </summary>
    //[Transaction(TransactionMode.Manual)]
    //public class AllPutKoudaiUnique : IExternalCommand
    //{
    //    public Result Execute(ExternalCommandData commandData,
    //      ref string message, ElementSet elements)
    //    {
    //        UIApplication uiapp = commandData.Application;
    //        UIDocument uidoc = uiapp.ActiveUIDocument;

    //        var result = MessageBox.Show("構台の新規一括配置をおこないますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
    //        if (result == DialogResult.Yes)
    //        {
    //            IList<Element> elementList = uidoc.Selection.PickElementsByRectangle("配置対象となる外枠線を範囲指定してください。");
    //            if (elementList.Count == 0) return Result.Succeeded;

    //            var basePoint = uidoc.Selection.PickObject(ObjectType.PointOnElement, "配置基点を指定してください。");
    //            var lengthVector = uidoc.Selection.PickObject(ObjectType.PointOnElement, "橋軸方向のベクトルを指定してください。");
    //            var widthVector = uidoc.Selection.PickObject(ObjectType.PointOnElement, "幅員方向のベクトルを指定してください。");
    //        }

    //        using (var f = new FrmAllPutKoudaiUnique())
    //        {
    //            f.ShowDialog();
    //        }

    //        return Result.Succeeded;
    //    }
    //}

    /// <summary>
    /// [一括配置] スロープ作成
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateSlope : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
          ref string message, ElementSet elements)
        {
            //プロテクトキット用処理
            if (!YMS.ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;

            CmdCreateSlope cmd = new CmdCreateSlope(uidoc.Document);
            cmd.Excute(uidoc);

            return Result.Succeeded;
        }
    }

    /// <summary>
    /// [一括配置] ブレース・ツナギ配置
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class AllPutBraceTsunagi : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
          ref string message, ElementSet elements)
        {
            //プロテクトキット用処理
            if (!YMS.ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;

            var cmd = new CmdAllPutBraceTsunagi();
            cmd.Execute(uidoc);

            return Result.Succeeded;
        }
    }

    /// <summary>
    /// [一括配置] グルーピング
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Grouping : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
          ref string message, ElementSet elements)
        {
            //プロテクトキット用処理
            if (!YMS.ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;

            //CmdCreateGrouping cmd = new CmdCreateGrouping(uidoc);
            //cmd.Excute();

            Application.thisApp.ShowForm_FrmGrouping(uiapp);

            //using (var f = new FrmGrouping(uidoc))
            //{
            //    f.ShowDialog();
            //}

            return Result.Succeeded;
        }
    }

    ///// <summary>
    ///// [個別配置] テスト用 ※実装テスト用のコマンド
    ///// </summary>
    //[Transaction(TransactionMode.Manual)]
    //public class PutTest : IExternalCommand
    //{
    //    //public Result Execute(ExternalCommandData commandData,
    //    //  ref string message, ElementSet elements)
    //    //{
    //    //    UIApplication uiapp = commandData.Application;
    //    //    UIDocument uidoc = uiapp.ActiveUIDocument;
    //    //    MessageBox.Show("テスト配置");
    //    //    return Result.Succeeded;
    //    //}
    //    public Result Execute(
    //     ExternalCommandData commandData,
    //     ref string message,
    //     ElementSet elements)
    //    {
    //        UIApplication uiapp = commandData.Application;
    //        UIDocument uidoc = uiapp.ActiveUIDocument;
    //        Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
    //        Document doc = uidoc.Document;

    //        return GantryUtil.CreateRubberLine(uidoc);
    //    }
    //}

    /// <summary>
    /// [個別配置] 覆工板配置
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class PutFukkouban : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
          ref string message, ElementSet elements)
        {
            ////プロテクトキット用処理
            //if (!YMS.ClsProtect.bCheckProtect())
            //{
            //    return Result.Failed;
            //}
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;

            ////CmdCreateFukkouban cmd = new CmdCreateFukkouban(uidoc.Document);
            ////cmd.Excute(uidoc);
            //////using (var f = new FrmPutFukkouban(uiapp))
            //////{
            //////    if(f.ShowDialog()==DialogResult.OK)
            //////    {
            //////        Fukkouban.CreateFukkoudan(uiapp, uiapp.ActiveUIDocument.Document,"SS400", DefineUtil.eFukkoubanType.TwoM, true);
            //////    }
            //////}
            /////

            Application.thisApp.ShowForm_FrmPutFukkouban(uiapp);

            return Result.Succeeded;
        }
    }

    /// <summary>
    /// [個別配置] 大引(桁受)配置
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class PutOobikiKetauke : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
          ref string message, ElementSet elements)
        {
            //プロテクトキット用処理
            if (!YMS.ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            //CmdCreateOhbiki cmd = new CmdCreateOhbiki(uidoc);
            //cmd.Excute();
            Application.thisApp.ShowForm_FrmPutOhbikiKetauke(uiapp);

            return Result.Succeeded;
        }
    }

    /// <summary>
    /// [個別配置] 根太(主桁)配置
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class PutNedaShugeta : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
          ref string message, ElementSet elements)
        {
            //プロテクトキット用処理
            if (!YMS.ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;

            //CmdCreateNeda cmd = new CmdCreateNeda(uidoc);
            //cmd.Excute();
            Application.thisApp.ShowForm_FrmPutNedaShugeta(uiapp);

            return Result.Succeeded;
        }
    }

    /// <summary>
    /// [個別配置] 覆工桁配置
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class PutFukkougeta : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
          ref string message, ElementSet elements)
        {
            //プロテクトキット用処理
            if (!YMS.ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;

            //CmdCreateFukkougeta cmd = new CmdCreateFukkougeta(uidoc);
            //if (!cmd.Excute())
            //{
            //    return Result.Failed;
            //}
            Application.thisApp.ShowForm_FrmPutFukkougeta(uiapp);

            return Result.Succeeded;
        }
    }

    /// <summary>
    /// [個別配置] 対傾構・対傾構取付プレート配置
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class PutTaikeikou : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
          ref string message, ElementSet elements)
        {
            //プロテクトキット用処理
            if (!YMS.ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;

            //CmdCreateTaikeikou cmd = new CmdCreateTaikeikou(uidoc);
            //return cmd.Excute();
            Application.thisApp.ShowForm_FrmPutTaikeikou(uiapp);

            return Result.Succeeded;
        }
    }

    /// <summary>
    /// [個別配置] 敷桁配置
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class PutShikigeta : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
          ref string message, ElementSet elements)
        {
            //プロテクトキット用処理
            if (!YMS.ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            //CmdCreateShikigeta cmd = new CmdCreateShikigeta(uidoc);
            //cmd.Excute();
            Application.thisApp.ShowForm_FrmPutShikigeta(uiapp);

            return Result.Succeeded;
        }
    }

    /// <summary>
    /// [個別配置] スチフナープレート・スチフナージャッキ配置
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class PutSuchifuna : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
          ref string message, ElementSet elements)
        {
            //プロテクトキット用処理
            if (!YMS.ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;

            //CmdCreateStiffener cmd = new CmdCreateStiffener(uidoc);
            //if (cmd.Excute() != Result.Succeeded)
            //{
            //    return Result.Failed;
            //}
            Application.thisApp.ShowForm_FrmPutSuchifuna(uiapp);

            return Result.Succeeded;
        }
    }

    /// <summary>
    /// [個別配置] 高さ調整プレート・調整材配置
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class PutTakasaChouseiPlate : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
          ref string message, ElementSet elements)
        {
            //プロテクトキット用処理
            if (!YMS.ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;

            //CmdCreateTakasaChousei cmd = new CmdCreateTakasaChousei(uidoc);
            //if (cmd.Excute())
            //{
            //    return Result.Succeeded;
            //}
            //else
            //{
            //    return Result.Failed;
            //}
            Application.thisApp.ShowForm_FrmPutTakasaChouseiPlateChouseizai(uiapp);
            return Result.Succeeded;
        }
    }

    /// <summary>
    /// [個別配置] 鋼板材配置
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class PutKoubanzai : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
          ref string message, ElementSet elements)
        {
            //プロテクトキット用処理
            if (!YMS.ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;

            //CmdCreateKouhan cmd = new CmdCreateKouhan(uidoc);
            //cmd.Excute();

            Application.thisApp.ShowForm_FrmPutKoubanzai(uiapp);

            return Result.Succeeded;
        }
    }

    /// <summary>
    /// [個別配置] 地覆・覆工板ズレ止め材配置
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class PutJifukuFukkoubanZuredome : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
          ref string message, ElementSet elements)
        {
            //プロテクトキット用処理
            if (!YMS.ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            //CmdCreateJifukuZuredome cmd = new CmdCreateJifukuZuredome(uidoc);
            //cmd.Excute();
            Application.thisApp.ShowForm_FrmPutJifukuFukkoubanZuredomezai(uiapp);

            return Result.Succeeded;
        }
    }

    /// <summary>
    /// [個別配置] 手摺・手摺支柱配置
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class PutTesuriTesurishichu : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
          ref string message, ElementSet elements)
        {
            //プロテクトキット用処理
            if (!YMS.ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            //CmdCreateTesuri cmd = new CmdCreateTesuri(uidoc);
            //cmd.Excute();
            Application.thisApp.ShowForm_FrmPutTesuriTesuriShichu(uiapp);

            return Result.Succeeded;
        }
    }

    /// <summary>
    /// [個別配置] 水平ブレース配置
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class PutHorizontalBrace : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
          ref string message, ElementSet elements)
        {
            //プロテクトキット用処理
            if (!YMS.ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;

            //using (var f = new FrmPutHorizontalBrace())
            //{
            //    f.ShowDialog();
            //}
            Application.thisApp.ShowForm_FrmPutHorizontalBrace(uiapp);

            return Result.Succeeded;
        }
    }



    /// <summary>
    /// [個別配置] 垂直ブレース配置
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class PutVerticalBrace : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
          ref string message, ElementSet elements)
        {
            //プロテクトキット用処理
            if (!YMS.ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;

            //using (var f = new FrmPutVerticalBrace())
            //{
            //    f.ShowDialog();
            //}
            Application.thisApp.ShowForm_FrmPutVerticalBrace(uiapp);

            //Reference ref= uidoc.Selection.PickObject(ObjectType.Face, "面を選択");

            //GantoryUtil.CreateReferencePlaneFromFace();
            return Result.Succeeded;
        }
    }

    /// <summary>
    /// [個別配置] 締結補助材配置
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class PutTeiketsuHojyozai : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
          ref string message, ElementSet elements)
        {
            //プロテクトキット用処理
            if (!YMS.ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;

            //CmdCreateTeiketsuHojo cmd = new CmdCreateTeiketsuHojo(uidoc);
            //cmd.Excute();
            Application.thisApp.ShowForm_FrmPutTeiketsuHojyozai(uiapp);

            return Result.Succeeded;
        }
    }

    /// <summary>
    /// [個別配置] 水平つなぎ配置
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class PutHorizontalTsunagi : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
          ref string message, ElementSet elements)
        {
            //プロテクトキット用処理
            if (!YMS.ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }
            UIApplication uiapp = commandData.Application;
            Application.thisApp.ShowForm_FrmPutHorizontalTsunagi(uiapp);

            return Result.Succeeded;
        }
    }

    /// <summary>
    /// [個別配置] 方杖配置
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class PutHoudue : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
          ref string message, ElementSet elements)
        {
            //プロテクトキット用処理
            if (!YMS.ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;

            //CmdCreateHoudue cmd = new CmdCreateHoudue(uidoc);
            //cmd.Excute();
            Application.thisApp.ShowForm_FrmPutHoudue(uiapp);

            return Result.Succeeded;
        }
    }

    /// <summary>
    /// [個別配置] 支柱配置
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class PutShichu : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
          ref string message, ElementSet elements)
        {
            //プロテクトキット用処理
            if (!YMS.ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;

            //CmdCreatePiller cmd = new CmdCreatePiller(uidoc);
            //cmd.Excute();
            Application.thisApp.ShowForm_FrmPutShichu(uiapp);

            return Result.Succeeded;
        }
    }

    /// <summary>
    /// [個別配置] 構台杭(支持杭)配置
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class PutKui : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
          ref string message, ElementSet elements)
        {
            //プロテクトキット用処理
            if (!YMS.ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;

            //CmdCreatePile cmd = new CmdCreatePile(uidoc);
            //cmd.Excute();
            Application.thisApp.ShowForm_FrmPutKui(uiapp);

            return Result.Succeeded;
        }
    }

    /// <summary>
    /// [部材修正] サイズ一覧
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class EditSizeList : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
          ref string message, ElementSet elements)
        {
            //プロテクトキット用処理
            if (!YMS.ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;

            try
            {
                IList<Element> getElements = uidoc.Selection.PickElementsByRectangle("対象部材を指定してください。");
                List<ElementId> selectedElementIdList = new List<ElementId>();
                foreach (var element in getElements)
                {
                    selectedElementIdList.Add(element.Id);
                }

                Application.thisApp.ShowForm_FrmEditSizeList(uiapp, selectedElementIdList);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException ex)
            {
                // ESCキー等によるキャンセル処理が走った場合は何もせずに処理を終える。
            }

            return Result.Succeeded;
        }
    }

    /// <summary>
    /// [部材修正] 位置変更
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class EditLocationChange : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
          ref string message, ElementSet elements)
        {
            //プロテクトキット用処理
            if (!YMS.ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;

            try
            {
                var selectedElement = uidoc.Selection.PickObject(ObjectType.Element, "対象部材を指定してください。");
                var element = uidoc.Document.GetElement(selectedElement.ElementId);
                Application.thisApp.ShowForm_FrmEditLocationChange(uiapp, element.Id);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException ex)
            {
                // ESCキー等によるキャンセル処理が走った場合は何もせずに処理を終える。
            }

            return Result.Succeeded;
        }
    }

    /// <summary>
    /// [部材修正] 長さ変更
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class EditLengthChange : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
          ref string message, ElementSet elements)
        {
            //プロテクトキット用処理
            if (!YMS.ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;

            try
            {
                IList<Element> getElements = uidoc.Selection.PickElementsByRectangle("対象部材を指定してください。");
                List<ElementId> selectedElementIdList = new List<ElementId>();
                foreach (var element in getElements)
                {
                    selectedElementIdList.Add(element.Id);
                }

                Application.thisApp.ShowForm_FrmEditLengthChange(uiapp, selectedElementIdList);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException ex)
            {
                // ESCキー等によるキャンセル処理が走った場合は何もせずに処理を終える。
            }

            return Result.Succeeded;
        }
    }

    /// <summary>
    /// [割付] 部材割付
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class WaritsukeElement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
          ref string message, ElementSet elements)
        {
            //プロテクトキット用処理
            if (!YMS.ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;

            CmdWaritsuke cmd = new CmdWaritsuke(uidoc);
            cmd.Excute();

            return Result.Succeeded;
        }
    }

    /// <summary>
    /// [その他] 干渉チェック
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class SonotaKanshouCheck : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
          ref string message, ElementSet elements)
        {
            //プロテクトキット用処理
            if (!YMS.ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }
            UIApplication uiapp = commandData.Application;
            Application.thisApp.ShowForm_FrmSonotaKanshouCheck(uiapp);

            return Result.Succeeded;
        }
    }
    /// <summary>
    /// [その他] 個別配置
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

            if (!YMS.ClsProtect.bCheckProtect())
            {
                return Result.Failed;
            }

            YMS.Command.ClsCommandKobetsuHaichi.CommandKobetsuHaichi(uiapp, false);


            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class CmdPlaceFamilyInstance : IExternalCommand
    {
        List<ElementId> _added_element_ids = new List<ElementId>();
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            var app = uiapp.Application;
            Document doc = uidoc.Document;
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfCategory(BuiltInCategory.OST_Doors);
            collector.OfClass(typeof(FamilySymbol));
            FamilySymbol symbol = collector.FirstElement() as FamilySymbol;
            _added_element_ids.Clear();
            app.DocumentChanged += new EventHandler<DocumentChangedEventArgs>(OnDocumentChanged);
            uidoc.PromptForFamilyInstancePlacement(symbol);
            app.DocumentChanged -= new EventHandler<DocumentChangedEventArgs>(OnDocumentChanged);
            int n = _added_element_ids.Count;
            TaskDialog.Show("Place Family Instance", string.Format("{0} element{1} added.", n, ((1 == n) ? "" : "s")));
            return Result.Succeeded;
        }
        void OnDocumentChanged(object sender, DocumentChangedEventArgs e)
        {
            _added_element_ids.AddRange(e.GetAddedElementIds());
        }
    }

}
