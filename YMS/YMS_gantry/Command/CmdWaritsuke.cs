using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YMS_gantry.Material;
using YMS_gantry.UI;

namespace YMS_gantry.Command
{
    class CmdWaritsuke
    {
        static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public enum WaritukeDist : int
        {
            warituke1mm = 1,
            warituke10mm = 10,
            warituke100mm = 100
        }

        public enum WaritsukeResult : int
        {
            End=0,
            Continue=1,
            Cancel=2,
            Error=3
        }


        Document _doc { get; set; }
        UIDocument _uiDoc { get; set; }
        FrmWaritsukeElement f { get; set; }

        /// <summary>
        /// 割付対象のID
        /// </summary>
        ElementId m_targetId { get; set; }

        /// <summary>
        /// 鋼材のサイズ(H〇〇x～)
        /// </summary>
        string originalKouzaiSize { get; set; }
        /// <summary>
        /// 選択されたサイズ
        /// </summary>
        string selectedSize { get; set; }
        public static XYZ m_InsertPoint { get; set; }
        public static XYZ m_EndPoint { get; set; }

        public static List<ElementId> m_createdIds { get; set; }

        public static List<XYZ> m_createdXYZs { get; set; }

        public static List<(ElementId parent,List<ElementId> coverPl)> m_coverPLId { get; set; }
        public static XYZ m_OriginalStartPnt { get; set; }
        private bool didWaritsuke { get; set; }

        public CmdWaritsuke(UIDocument uiDoc)
        {
            _uiDoc = uiDoc;
            _doc = _uiDoc.Document;
            f = Application.thisApp.frmWaritsuke;
            m_targetId = null;
        }

        /// <summary>
        /// 実行
        /// </summary>
        /// <returns></returns>
        public Result Excute()
        {
            Result rs = Result.Succeeded;
            WaritsukeResult wRs = WaritsukeResult.Continue;
            while(wRs==WaritsukeResult.Continue)
            {
                try
                {
                    // 対象の部材を取得
                    ElementId buzaiId = null;
                    if (!GetTargetElement(ref buzaiId))
                    {
                        return Result.Cancelled;
                    }
                    m_targetId = buzaiId;

                    m_createdIds = new List<ElementId>();
                    m_createdXYZs = new List<XYZ>();
                    m_coverPLId = new List<(ElementId parent, List<ElementId> coverPl)>();

                    ///コマンド単位でトランザクションまとめるため、ここでTRGP開く
                    using (TransactionGroup tg = new TransactionGroup(_doc, Guid.NewGuid().GetHashCode().ToString()))
                    {
                        tg.Start();
                       wRs= WaritukeMain(_uiDoc.Application);

                        //割付したら消す
                        if (m_createdIds.Count > 0)
                        {
                            using (Transaction tr = new Transaction(_doc, Guid.NewGuid().GetHashCode().ToString()))
                            {
                                tr.Start();
                                _doc.Delete(m_targetId);
                                tr.Commit();
                            }
                        }
                        else
                        {
                            ChangeElementVisible(_doc, m_targetId, false);
                        }
                        tg.Assimilate();
                    }
                    rs = Result.Succeeded;
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                {
                    rs = Result.Cancelled;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.StackTrace);
                    rs = Result.Failed;
                }
                finally
                {
                    if (Application.thisApp.frmWaritsuke != null && Application.thisApp.frmWaritsuke.Visible)
                    {
                        Application.thisApp.frmWaritsuke.Close();
                        Application.thisApp.frmWaritsuke.Dispose();
                    }
                }
            }
            return rs;
        }

        private WaritsukeResult WaritukeMain(UIApplication uiapp)
        {
            //ドキュメントを取得
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            // 情報を取得
            FamilyInstance instBase = doc.GetElement(m_targetId) as FamilyInstance;

            string typeName = GantryUtil.GetOriginalTypeName(_doc, instBase);
            Curve cvBase = GantryUtil.GetCurve(_doc, m_targetId);
            if (typeName == PilePiller.sichuTypeName)
            {
                cvBase = GantryUtil.GetPileCurve(instBase, _doc);
            }
            XYZ tmpStPoint = cvBase.GetEndPoint(0);
            XYZ tmpEdPoint = cvBase.GetEndPoint(1);

            MaterialSuper ms = MaterialSuper.ReadFromElement(m_targetId, _doc);
            if (ms == null)
            {
                MessageUtil.Warning("システム外で配置された部材のため割付できません。", "部材割付");
                return WaritsukeResult.Continue;
            }

            string steelSizeOrign = string.Empty;
            bool isSichu = false;
            double rotate = 0;
            switch (typeName)
            {
                case "大引":
                case "根太":
                case "主桁":
                case "敷桁":
                case "方杖":
                case "地覆":
                case "手摺":
                case "覆工桁":
                case "ﾂﾅｷﾞ":
                case "水平ﾂﾅｷﾞ":
                    steelSizeOrign = ms.m_Size;
                    break;
                case "支柱":
                    isSichu = true;
                    steelSizeOrign = ms.m_Size;
                    break;
                case "桁受":
                    steelSizeOrign = ms.m_Size;
                    if(ms.m_Size.StartsWith("[")||ms.m_Size.StartsWith("L"))
                    {
                       rotate= RevitUtil.ClsRevitUtil.GetTypeParameter(instBase.Symbol, DefineUtil.PARAM_ROTATE);
                    }
                    break;
                default:
                    MessageUtil.Warning("割付対象外の部材が選択されました。", "部材割付");
                    return WaritsukeResult.Continue;
            }

            bool hasHost = true;

            ElementId hostId = GantryUtil.GetInstanceHostId(_doc, instBase);
            if (hostId == ElementId.InvalidElementId) { hasHost = false; }
            if (!hasHost)
            {
                MessageUtil.Warning("指定された部材に作業面またはホストがないため、割付部材を配置できません", "部材割付");
                return WaritsukeResult.Continue;
            }

            // 割付範囲の選択
            Autodesk.Revit.DB.View v = uidoc.ActiveView;
            if (!isSichu)
            {
                (tmpStPoint, tmpEdPoint) = GetPickPoint(uidoc, tmpStPoint, tmpEdPoint);
            }
            else
            {
                //柱の上下端どちらを基点にするか選ぶ
                FrmWaritsukeYesNo yN = new FrmWaritsukeYesNo();
                DialogResult rs = yN.ShowDialog();
                if (rs==DialogResult.Yes)
                {
                    tmpStPoint = (cvBase.GetEndPoint(0).Z > cvBase.GetEndPoint(1).Z) ? cvBase.GetEndPoint(0) : cvBase.GetEndPoint(1);
                    tmpEdPoint = (cvBase.GetEndPoint(0).Z > cvBase.GetEndPoint(1).Z) ? cvBase.GetEndPoint(1) : cvBase.GetEndPoint(0);
                }
                else if(rs==DialogResult.No)
                {
                    tmpStPoint = (cvBase.GetEndPoint(0).Z < cvBase.GetEndPoint(1).Z) ? cvBase.GetEndPoint(0) : cvBase.GetEndPoint(1);
                    tmpEdPoint = (cvBase.GetEndPoint(0).Z < cvBase.GetEndPoint(1).Z) ? cvBase.GetEndPoint(1) : cvBase.GetEndPoint(0);
                }
                else
                {
                    return WaritsukeResult.Cancel;
                }
            }

            if (ClsRevitUtil.CovertFromAPI(tmpStPoint.DistanceTo(tmpEdPoint)) < 0.1)
            {
                MessageUtil.Warning("割付距離が短いため、割付部材を配置できません", "部材割付");
                return WaritsukeResult.Continue;
            }

            if (!instBase.Invisible)
            {
                //元の部材は非表示
                ChangeElementVisible(_doc, m_targetId, true);
            }

            XYZ dir = (tmpEdPoint - tmpStPoint).Normalize();
            m_InsertPoint = tmpStPoint;
            m_EndPoint = tmpEdPoint;
            m_OriginalStartPnt = tmpStPoint;
            // 割付処理
            double originalLength = ClsRevitUtil.CovertFromAPI(Math.Abs((m_InsertPoint.DistanceTo(m_EndPoint))));
            bool Continue = true;
            WaritsukeResult res = WaritsukeResult.Continue;
            while (Continue)
            {
                // スケールの表示
                double maxLength = double.MaxValue;
                double minLength = double.MinValue;
                double buzaiLength = 0.0;

                List<ElementId> scaleIds = new List<ElementId>();

                // 割付ダイアログの表示
                Application.thisApp.ShowForm_Waritsuke(_uiDoc.Application);
                f = Application.thisApp.frmWaritsuke;
                f.Targetwaritsuke(typeName,steelSizeOrign);
                double remainingL = ClsRevitUtil.CovertFromAPI(m_InsertPoint.DistanceTo(m_EndPoint));
                f.UpdateLength(originalLength, remainingL);
                //Del 2024/03/04 メモリサイズは固定でOK
                //WaritukeDist d = f.CmbUnit.Text == "1" ? WaritukeDist.warituke1mm : f.CmbUnit.Text == "10" ? WaritukeDist.warituke10mm : WaritukeDist.warituke100mm;

                scaleIds = CreateScale(uidoc, m_InsertPoint, m_EndPoint, instBase, WaritukeDist.warituke100mm, maxLength, minLength);

                // ダイアログが閉じられるまで待機
                while (f.Visible)
                {
                    uidoc.RefreshActiveView();
                    System.Windows.Forms.Application.DoEvents();
                }

                // ダイアログが閉じられた後でリザルトを取得
                DialogResult result = f.DialogResult;
                if (result == DialogResult.Cancel)
                {
                    DeleteScale(doc, scaleIds);
                    Continue = false;
                    res = WaritsukeResult.End;
                    continue;
                }

                // 各種割付処理
                string familyPath = string.Empty;
                try
                {
                    FamilySymbol symbol = null;
                    switch (f.command)
                    {
                        case FrmWaritsukeElement.WaritsukeCommand.CreateOhbiki:
                        case FrmWaritsukeElement.WaritsukeCommand.CreateJifuku:
                        case FrmWaritsukeElement.WaritsukeCommand.CreateKetauke:
                        case FrmWaritsukeElement.WaritsukeCommand.CreateTesuri:
                        case FrmWaritsukeElement.WaritsukeCommand.CreateShikiketa:
                        case FrmWaritsukeElement.WaritsukeCommand.CreateHoudue:
                        case FrmWaritsukeElement.WaritsukeCommand.CreateNeda:
                        case FrmWaritsukeElement.WaritsukeCommand.CreateShuketa:
                        case FrmWaritsukeElement.WaritsukeCommand.CreateJoint:
                        case FrmWaritsukeElement.WaritsukeCommand.CreateHukkouketa:
                            familyPath = Master.ClsWaritsukeCsv.GetFamilyPath(f.SelectedSize, f.familyType);
                            if (!GantryUtil.GetFamilySymbol(_doc, familyPath, f.typeName, out symbol))
                            {
                                MessageUtil.Warning("指定したファミリまたはタイプを取得できませんでした。\r\n" +
                                    $"パス:{familyPath}\r\nタイプ:{f.typeName}", "割付");
                                break;
                            }
                            if (f.waritsukeType.Equals(FrmWaritsukeElement.WaritsukeType.Normal))
                            {
                                double length = (double)(f.NmcWaritsukeLeng.Value);
                                if (f.mode != FrmWaritsukeElement.WaritsukeMode.Normal)
                                {
                                    string syuD = f.SelectedSize.Substring(f.SelectedSize.IndexOf("_") + 1, 3);
                                    length = ClsCommonUtils.ChangeStrToDbl(syuD) * 1000;
                                }

                                if (ClsGeo.GEO_EQ0(length))
                                {
                                    continue;
                                }
                                if (WaritsukeKetaZai(_doc, instBase, symbol, f.SelectedSize, length,rotate))
                                {
                                    buzaiLength = length;
                                    if (f.ChkNeedCoverPL.Checked&&m_createdIds.Count>1)
                                    {
                                        familyPath = Master.ClsWaritsukeCsv.GetFamilyPath(f.CmbCoverPL.Text, "カバープレート");
                                        if (!GantryUtil.GetFamilySymbol(_doc, familyPath, "ｶﾊﾞｰﾌﾟﾚｰﾄ(構台)", out symbol))
                                        {
                                            MessageUtil.Warning("指定したファミリまたはタイプを取得できませんでした。\r\n" +
                                                $"パス:{familyPath}\r\nタイプ:{f.typeName}", "割付");
                                            break;
                                        }

                                        WaritsukeCoverPL(_doc, instBase,symbol, f.SelectedSize, f.CmbCoverPL.Text, false);
                                    }
                                }
                                else
                                {
                                    buzaiLength = 0.0;
                                }
                            }
                            else if (f.waritsukeType.Equals(FrmWaritsukeElement.WaritsukeType.Pieace))
                            {
                                if (WaritsukePieace(_doc, instBase, symbol, f.SelectedSize))
                                {
                                    (double, double) leng = GetPieaceLength(f.SelectedSize);
                                    buzaiLength = leng.Item1;
                                }
                                else
                                {
                                    buzaiLength = 0.0;
                                }
                            }
                            else if (f.waritsukeType.Equals(FrmWaritsukeElement.WaritsukeType.Plate))
                            {
                                if (WaritsukePlate(_doc, instBase, symbol, f.SelectedSize, f.PlateSize))
                                {
                                    buzaiLength = f.PlateSize.W;
                                }
                                else
                                {
                                    buzaiLength = 0.0;
                                }
                            }
                            else if (f.waritsukeType.Equals(FrmWaritsukeElement.WaritsukeType.Jack))
                            {
                                //桁材にジャッキは割り付けない
                                //double length = ClsCommonUtils.ChangeStrToDbl(f.NmcJackLng.Value.ToString());
                                //if (WaritsukeJack(_doc, instBase, symbol, f.SelectedSize, length))
                                //{
                                //    buzaiLength = length;
                                //}
                                //else
                                //{
                                //    buzaiLength = 0.0;
                                //}
                            }
                            break;
                        case FrmWaritsukeElement.WaritsukeCommand.CreateSichu:
                            familyPath = Master.ClsWaritsukeCsv.GetShicuFamilyPath(f.SelectedSize, f.familyType);
                            if (!GantryUtil.GetFamilySymbol(_doc, familyPath, f.typeName, out symbol))
                            {
                                MessageUtil.Warning("指定したファミリまたはタイプを取得できませんでした。\r\n" +
                                    $"パス:{familyPath}\r\nタイプ:{f.typeName}", "割付");
                                break;
                            }
                            if (f.waritsukeType.Equals(FrmWaritsukeElement.WaritsukeType.Normal))
                            {
                                double length = (double)(f.NmcWaritsukeLeng.Value);
                                if (f.mode != FrmWaritsukeElement.WaritsukeMode.Normal)
                                {
                                    string syuD = f.SelectedSize.Substring(f.SelectedSize.IndexOf("_") + 1, 3);
                                    length = ClsCommonUtils.ChangeStrToDbl(syuD) * 1000;
                                }
                                if (ClsGeo.GEO_EQ0(length))
                                {
                                    continue;
                                }
                                if (WaritsukeShichu(_doc, instBase, symbol, f.SelectedSize, length))
                                {
                                    buzaiLength = length;
                                    if (f.ChkNeedCoverPL.Checked && m_createdIds.Count > 1)
                                    {
                                        familyPath = Master.ClsWaritsukeCsv.GetFamilyPath(f.CmbCoverPL.Text, "カバープレート");
                                        if (!GantryUtil.GetFamilySymbol(_doc, familyPath, "ｶﾊﾞｰﾌﾟﾚｰﾄ(構台)", out symbol))
                                        {
                                            MessageUtil.Warning("指定したファミリまたはタイプを取得できませんでした。\r\n" +
                                                $"パス:{familyPath}\r\nタイプ:{f.typeName}", "割付");
                                            break;
                                        }
                                        WaritsukeCoverPL(_doc, instBase, symbol, f.SelectedSize, f.CmbCoverPL.Text, true);
                                    }
                                }
                                else
                                {
                                    buzaiLength = 0.0;
                                }
                            }
                            else if (f.waritsukeType.Equals(FrmWaritsukeElement.WaritsukeType.Pieace))
                            {
                                if (WaritsukePieaceToShicu(_doc, instBase, symbol, f.SelectedSize))
                                {
                                    (double, double) leng = GetPieaceLength(f.SelectedSize);
                                    buzaiLength = leng.Item1;
                                }
                                else
                                {
                                    buzaiLength = 0.0;
                                }
                            }
                            else if (f.waritsukeType.Equals(FrmWaritsukeElement.WaritsukeType.Plate))
                            {
                                if (WaritsukePlateToShicu(_doc, instBase, symbol, f.SelectedSize, f.PlateSize))
                                {
                                    buzaiLength = f.PlateSize.H;
                                }
                                else
                                {
                                    buzaiLength = 0.0;
                                }
                            }
                            else if (f.waritsukeType.Equals(FrmWaritsukeElement.WaritsukeType.Jack))
                            {
                                double length = ClsCommonUtils.ChangeStrToDbl(f.NmcJackLng.Value.ToString());
                                if (WaritsukeJackToSichu(_doc, instBase, symbol, f.SelectedSize, length))
                                {
                                    length += ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(symbol, "調整材長さ_左"));
                                    length += ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(symbol, "調整材長さ_右"));
                                    buzaiLength = length;
                                }
                                else
                                {
                                    buzaiLength = 0.0;
                                }
                            }
                            break;
                        case FrmWaritsukeElement.WaritsukeCommand.Switch:
                            break;
                        case FrmWaritsukeElement.WaritsukeCommand.CommandUndo:
                            WaritukeUndo(doc);
                            buzaiLength = 0.0;
                            break;
                        case FrmWaritsukeElement.WaritsukeCommand.CommandEnd:
                            break;
                    }

                    var remainingLength = m_InsertPoint.DistanceTo(m_EndPoint);
                    if (ClsGeo.GEO_GT(ClsRevitUtil.CovertToAPI(buzaiLength),remainingLength))
                    {
                        if(MessageUtil.YesNo("割付の最大長を超えました。最大長を超えて割付を行いますか？\r\n はい : 割付して終了します \r\n いいえ : 最大長を超える前に戻ります", "割付", f)==DialogResult.Yes)
                        {
                            Continue = false;
                            continue;
                        }
                        else
                        {
                            var tmpP = m_InsertPoint;
                            WaritukeUndo(doc);
                            m_InsertPoint = tmpP;
                            continue;
                        }
                    }

                    m_InsertPoint += dir * ClsRevitUtil.CovertToAPI(buzaiLength);
                    // 最終点まで到達したかの判定
                    if (ClsGeo.GEO_EQ0(m_InsertPoint.DistanceTo(m_EndPoint)))
                    {
                        MessageUtil.Information("割付の最大長まで到達しました","割付",f);
                        DeleteScale(doc, scaleIds);
                        Continue = false;
                        continue;
                    }
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                {
                    return WaritsukeResult.Cancel;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.StackTrace);
                    return WaritsukeResult.Error;
                }
                finally
                {
                      // ダイアログを閉じる
                    f.Dispose();
                    // スケールの削除
                    DeleteScale(doc, scaleIds);
                }
            }

            if (MessageUtil.YesNo("続けて割付を行いますか？", "部材割付") == DialogResult.Yes)
            {
                res = WaritsukeResult.Continue;
            }
            else
            {
                res = WaritsukeResult.End;
            }
            return res;
        }

        /// <summary>
        /// Undoコマンド
        /// </summary>
        /// <param name="doc"></param>
        private static void WaritukeUndo(Document doc)
        {
            if (m_createdIds.Count > 0 && m_createdXYZs.Count > 0)
            {
                using (Transaction transaction = new Transaction(doc, "Delete Family"))
                {
                    transaction.Start();
                    // ファミリの削除処理
                    ElementId targetId = m_createdIds.Last();
                    //カバーPLついてたら消す
                    foreach ((ElementId parent, List<ElementId> coverPl) coverPLItem in m_coverPLId)
                    {
                        if (coverPLItem.parent.Equals(targetId))
                        {
                            foreach(ElementId id in coverPLItem.coverPl)
                            {
                                doc.Delete(id);
                            }
                        }
                    }
                    doc.Delete(targetId);
                    transaction.Commit();

                    m_createdIds.RemoveAt(m_createdIds.Count - 1);
                    m_createdXYZs.RemoveAt(m_createdXYZs.Count - 1);
                    if(m_createdXYZs.Count>0)
                    {
                        m_InsertPoint = m_createdXYZs.Last();
                    }
                    else
                    {
                        m_InsertPoint = m_OriginalStartPnt;
                    }
                }
            }
        }

        #region 素材/山留主材/高強山留材 割付
        /// <summary>
        /// 桁材を割り付ける
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="instBase"></param>
        /// <param name="kouzaiSym"></param>
        /// <param name="size"></param>
        /// <param name="length"></param>
        private static bool WaritsukeKetaZai(Document doc, FamilyInstance instBase, FamilySymbol kouzaiSym, string size, double length,double rotate=0)
        {
            bool retBool = true;
            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                try
                {
                    t.Start();
                    Reference refer = GantryUtil.GetReference(instBase);
                    MaterialSuper ms = MaterialSuper.ReadFromElement(instBase.Id, doc);
                    if (ms == null) { return false; }

                    //タイプを複製
                    FamilySymbol creatSym =(kouzaiSym.Name=="ﾂﾅｷﾞ") ?kouzaiSym:GantryUtil.DuplicateTypeWithNameRule(doc, ms.m_KodaiName, kouzaiSym, kouzaiSym.Name);
                    
                    var element = doc.GetElement(refer);
                    double offset = 0;
                    if (element is Level level)
                    {
                        offset = ClsRevitUtil.GetParameterDouble(doc, instBase.Id, DefineUtil.PARAM_BASE_OFFSET);
                    }
                    else
                    {
                        offset = ClsRevitUtil.GetParameterDouble(doc, instBase.Id, DefineUtil.PARAM_HOST_OFFSET);
                    }
                    XYZ vec = (m_EndPoint - m_InsertPoint).Normalize();
                    XYZ start = m_InsertPoint;
                    XYZ end = m_InsertPoint + vec * ClsRevitUtil.ConvertDoubleGeo2Revit(length);
                    var ts = instBase.GetTransform();
                    if (ms.MaterialSize().Shape == MaterialShape.C || ms.MaterialSize().Shape == MaterialShape.L)
                    {
                        if (RevitUtil.ClsGeo.IsLeft(instBase.FacingOrientation.Normalize(),vec))
                        {
                            start = end;
                            end = m_InsertPoint;
                        }
                    }
                    

                    ElementId newId = MaterialSuper.PlaceWithTwoPoints(creatSym, refer, start, end, offset);

                    if (newId != null && newId != ElementId.InvalidElementId)
                    {
                        AttachMaterialSuper(doc, newId, ms.m_KodaiName, ms, size, FrmWaritsukeElement.WaritsukeType.Normal);
                        if(rotate!=0)
                        {
                            RevitUtil.ClsRevitUtil.SetTypeParameter(creatSym, DefineUtil.PARAM_ROTATE, rotate);
                        }
                        t.Commit();
                        m_createdIds.Add(newId);
                        m_createdXYZs.Add(end);
                        retBool = true;
                    }
                    else
                    {
                        retBool = false;
                    }
                }
                catch (Exception ex)
                {
                    retBool = false;
                    _logger.Error(ex.StackTrace);
                }
                finally
                {
                    t.Dispose();
                }
            }
            return retBool;
        }
        /// <summary>
        /// 支柱を割り付ける
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="instBase"></param>
        /// <param name="kouzaiSym"></param>
        /// <param name="size"></param>
        /// <param name="length"></param>
        private static bool WaritsukeShichu(Document doc, FamilyInstance instBase, FamilySymbol kouzaiSym, string size, double length)
        {
            bool retBool = true;
            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                try
                {
                    t.Start();
                    Reference refer = GantryUtil.GetReference(instBase);
                    MaterialSuper ms = MaterialSuper.ReadFromElement(instBase.Id, doc);
                    if (ms == null) { return false; }

                    //タイプを複製
                    FamilySymbol creatSym = kouzaiSym;
                    if (!size.Contains("HA") && !size.Contains("SMH"))
                    {
                        creatSym = GantryUtil.DuplicateTypeWithNameRule(doc, ms.m_KodaiName, kouzaiSym, kouzaiSym.Name);
                        ClsRevitUtil.SetTypeParameter(creatSym, DefineUtil.PARAM_LENGTH, ClsRevitUtil.CovertToAPI(length));
                        ClsRevitUtil.SetTypeParameter(creatSym, DefineUtil.PARAM_PILE_CUT_LENG, 0);
                    }
                    XYZ vec = (m_EndPoint - m_InsertPoint).Normalize();
                    XYZ end = m_InsertPoint + vec * ClsRevitUtil.ConvertDoubleGeo2Revit(length);
                    if (vec.Z < 0) { end = m_InsertPoint; }
                    ElementId newId = ElementId.InvalidElementId;
                    FamilyInstance ins = GantryUtil.CreateInstanceWith1point(doc, end, refer, creatSym, instBase.HandOrientation);
                    if (ins != null) { newId = ins.Id; }
                    if (newId != null && newId != ElementId.InvalidElementId)
                    {
                        AttachMaterialSuper(doc, newId, ms.m_KodaiName, ms, size, FrmWaritsukeElement.WaritsukeType.Normal);
                        t.Commit();
                        m_createdIds.Add(newId);
                        m_createdXYZs.Add(end);
                        retBool = true;
                    }
                    else
                    {
                        retBool = false;
                    }
                }
                catch (Exception ex)
                {
                    retBool = false;
                    _logger.Error(ex.StackTrace);
                }
                finally
                {
                    t.Dispose();
                }
            }
            return retBool;
        }

        /// <summary>
        /// カバープレート配置
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="instBase"></param>
        /// <param name="coverPLSym"></param>
        /// <param name="parentSize"></param>
        /// <param name="size"></param>
        /// <param name="isShichu"></param>
        /// <returns></returns>
        private static bool WaritsukeCoverPL(Document doc, FamilyInstance instBase, FamilySymbol coverPLSym,string parentSize, string size,bool isShichu)
        {
            bool retBool = true;
            if(!parentSize.Contains("HA")&&!parentSize.Contains("SMH"))
            {
                return true;
            }

            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                try
                {
                    t.Start();
                    XYZ vec = instBase.GetTransform().BasisZ.Normalize();
                    if (isShichu)
                    {
                        vec = instBase.HandOrientation.CrossProduct(XYZ.BasisZ).Normalize();
                    }
                    XYZ norm = instBase.HandOrientation;
                    if (isShichu)
                    {
                        norm = XYZ.BasisZ.Negate();
                    }

                    Reference refer = GantryUtil.GetReference(instBase);
                    if(isShichu)
                    {
                        FamilyInstance paretIns = doc.GetElement(m_createdIds.Last()) as FamilyInstance;
                        Face f = GantryUtil.GetSpecifyFaceOfFamilyInstance(paretIns, vec);
                        if (f != null) { refer = f.Reference; }
                    }
                    MaterialSuper ms = MaterialSuper.ReadFromElement(instBase.Id, doc);
                    if (ms == null) { return false; }

                    FamilyInstance parent = doc.GetElement(m_createdIds.Last()) as FamilyInstance;
                    if (parent == null) { return true; }
                    MaterialSuper pMs = MaterialSuper.ReadFromElement(parent.Id, doc);
                    string pSize = parentSize;
                    if (pSize.Contains("HA") || pSize.Contains("SMH"))
                    {
                        int ind = pSize.IndexOf('_');
                        pSize = pSize.Substring(0, ind);
                    }
                    MaterialSize materialSize = GantryUtil.GetKouzaiSize(pSize);


                    string thickSt = Master.ClsWaritsukeCsv.GetLength(size, "カバープレート");
                    double thick=ClsCommonUtils.ChangeStrToDbl(thickSt);
                    ElementId newId1 = ElementId.InvalidElementId;
                    XYZ insertP=m_InsertPoint+vec* ClsRevitUtil.CovertToAPI(materialSize.Height/2);
                    FamilyInstance ins = GantryUtil.CreateInstanceWith1point(doc, insertP, refer, coverPLSym, norm);
                    if (ins != null) { newId1 = ins.Id; }
                    if (newId1 != null && newId1 != ElementId.InvalidElementId)
                    {
                        CoverPL cPl = new CoverPL();
                        cPl.m_ElementId = newId1;
                        cPl.m_KodaiName = ms.m_KodaiName;
                        cPl.m_Size = size;
                        CoverPL.WriteToElement(cPl, doc);
                        retBool = true;
                    }
                    else
                    {
                        retBool = false;
                    }

                    ElementId newId2 = ElementId.InvalidElementId;
                    insertP = m_InsertPoint + vec.Negate() * ClsRevitUtil.CovertToAPI((materialSize.Height / 2) + thick);
                    ins = GantryUtil.CreateInstanceWith1point(doc,insertP, refer, coverPLSym, norm);
                    if (ins != null) { newId2 = ins.Id; }
                    if (newId2 != null && newId2 != ElementId.InvalidElementId)
                    {
                        CoverPL cPl = new CoverPL();
                        cPl.m_ElementId = newId2;
                        cPl.m_KodaiName = ms.m_KodaiName;
                        cPl.m_Size = size;
                        CoverPL.WriteToElement(cPl, doc);
                        retBool = true;
                    }
                    else
                    {
                        retBool = false;
                    }

                    m_coverPLId.Add((m_createdIds.Last(), new List<ElementId>() { newId1, newId2 }));
                    t.Commit();
                }
                catch (Exception ex)
                {
                    retBool = false;
                    _logger.Error(ex.StackTrace);
                }
                finally
                {
                    t.Dispose();
                }
            }
            return retBool;

        }

        #endregion

        #region ピース割付
        /// <summary>
        /// 補助ピースを割り付ける
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="instBase"></param>
        /// <param name="PieaceSym"></param>
        /// <param name="size"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private static bool WaritsukePieace(Document doc, FamilyInstance instBase, FamilySymbol PieaceSym, string size)
        {
            bool retBool = true;
            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                try
                {
                    t.Start();
                    Reference refer = GantryUtil.GetReference(instBase);
                    MaterialSuper ms = MaterialSuper.ReadFromElement(instBase.Id, doc);
                    if (ms == null) { return false; }

                    var element = doc.GetElement(refer);
                    double offset = 0;
                    if (element is Level level)
                    {
                        offset = ClsRevitUtil.GetParameterDouble(doc, instBase.Id, DefineUtil.PARAM_BASE_OFFSET);
                    }
                    else
                    {
                        offset = ClsRevitUtil.GetParameterDouble(doc, instBase.Id, DefineUtil.PARAM_HOST_OFFSET);
                    }
                    XYZ vec = (m_EndPoint - m_InsertPoint).Normalize();
                    FamilyInstance ins = GantryUtil.CreateInstanceWith1point(doc, m_InsertPoint, refer, PieaceSym, vec.Normalize(),followWithHostType:false);

                    if (ins != null)
                    {
                        AttachMaterialSuper(doc, ins.Id, ms.m_KodaiName, ms, size, FrmWaritsukeElement.WaritsukeType.Pieace);
                        t.Commit();
                        m_createdIds.Add(ins.Id);
                        m_createdXYZs.Add(m_InsertPoint);
                        retBool = true;
                    }
                    else
                    {
                        retBool = false;
                    }
                }
                catch (Exception ex)
                {
                    retBool = false;
                    _logger.Error(ex.StackTrace);
                }
                finally
                {
                    t.Dispose();
                }
            }
            return retBool;
        }
        /// <summary>
        /// 補助ピースを割り付ける
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="instBase"></param>
        /// <param name="PieaceSym"></param>
        /// <param name="size"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private static bool WaritsukePieaceToShicu(Document doc, FamilyInstance instBase, FamilySymbol PieaceSym, string size)
        {
            bool retBool = true;
            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                try
                {
                    t.Start();
                    Reference refer = GantryUtil.GetReference(instBase);
                    MaterialSuper ms = MaterialSuper.ReadFromElement(instBase.Id, doc);
                    if (ms == null) { return false; }

                    XYZ vec = (m_EndPoint - m_InsertPoint).Normalize();
                    var pSize = GetPieaceLength(size);
                    XYZ newP = m_InsertPoint + vec * ClsRevitUtil.CovertToAPI((pSize.Item1));
                    if (vec.Z > 0) { newP = m_InsertPoint; }
                    FamilyInstance ins = GantryUtil.CreateInstanceWith1point(doc, newP, refer, PieaceSym, instBase.HandOrientation, followWithHostType: true);
                    if (ins != null)
                    {
                        AttachMaterialSuper(doc, ins.Id, ms.m_KodaiName, ms, size, FrmWaritsukeElement.WaritsukeType.Pieace);
                        t.Commit();
                        m_createdIds.Add(ins.Id);
                        m_createdXYZs.Add(m_InsertPoint);
                        retBool = true;
                    }
                    else
                    {
                        retBool = false;
                    }
                }
                catch (Exception ex)
                {
                    retBool = false;
                    _logger.Error(ex.StackTrace);
                }
                finally
                {
                    t.Dispose();
                }
            }
            return retBool;
        }
        /// <summary>
        /// 選択されたピースサイズから長さを取る
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        private static (double, double) GetPieaceLength(string size)
        {
            string oriSize = size.Replace("SM-", "");
            int ind = oriSize.IndexOf("D-");
            string subs = oriSize.Substring(ind + 2, 1);
            double height = ClsCommonUtils.ChangeStrToDbl(subs) * 100;
            double width = ClsCommonUtils.ChangeStrToDbl(oriSize.Substring(0, 2)) * 10;
            return (height, width);
        }
        #endregion

        #region プレート割付
        /// <summary>
        /// プレート割付
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="instBase"></param>
        /// <param name="plateSym"></param>
        /// <param name="size"></param>
        /// <param name="plateSize"></param>
        /// <returns></returns>
        private static bool WaritsukePlate(Document doc, FamilyInstance instBase, FamilySymbol plateSym, string size, (double h, double d, double w) plateSize)
        {
            bool retBool = true;
            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                try
                {
                    t.Start();
                    Reference refer = GantryUtil.GetReference(instBase);
                    MaterialSuper ms = MaterialSuper.ReadFromElement(instBase.Id, doc);
                    if (ms == null) { return false; }

                    //タイプを複製
                    FamilySymbol creatSym = GantryUtil.DuplicateTypeWithNameRule(doc, ms.m_KodaiName, plateSym, plateSym.Name);
                    var element = doc.GetElement(refer);
                    double offset = 0;
                    if (element is Level level)
                    {
                        offset = ClsRevitUtil.GetParameterDouble(doc, instBase.Id, DefineUtil.PARAM_BASE_OFFSET);
                    }
                    else
                    {
                        offset = ClsRevitUtil.GetParameterDouble(doc, instBase.Id, DefineUtil.PARAM_HOST_OFFSET);
                    }
                    XYZ vec = (m_EndPoint - m_InsertPoint).Normalize();
                    ClsRevitUtil.SetTypeParameter(creatSym, "W", ClsRevitUtil.CovertToAPI(plateSize.w));
                    ClsRevitUtil.SetTypeParameter(creatSym, "D", ClsRevitUtil.CovertToAPI(plateSize.d));
                    ClsRevitUtil.SetTypeParameter(creatSym, "H1", ClsRevitUtil.CovertToAPI(plateSize.h));
                    ClsRevitUtil.SetTypeParameter(creatSym, "H2", ClsRevitUtil.CovertToAPI(plateSize.h));

                    FamilyInstance ins = GantryUtil.CreateInstanceWith1point(doc, m_InsertPoint, refer, creatSym, vec.Normalize(), followWithHostType: false);
                    if (ins != null)
                    {
                        AttachMaterialSuper(doc, ins.Id, ms.m_KodaiName, ms, size, FrmWaritsukeElement.WaritsukeType.Plate);
                        t.Commit();
                        m_createdIds.Add(ins.Id);
                        m_createdXYZs.Add(m_InsertPoint);
                        retBool = true;
                    }
                    else
                    {
                        retBool = false;
                    }
                }
                catch (Exception ex)
                {
                    retBool = false;
                    _logger.Error(ex.StackTrace);
                }
                finally
                {
                    t.Dispose();
                }
            }
            return retBool;
        }
        /// <summary>
        /// プレート割付
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="instBase"></param>
        /// <param name="plateSym"></param>
        /// <param name="size"></param>
        /// <param name="plateSize"></param>
        /// <returns></returns>
        private static bool WaritsukePlateToShicu(Document doc, FamilyInstance instBase, FamilySymbol plateSym, string size, (double h, double d, double w) plateSize)
        {
            bool retBool = true;
            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                try
                {
                    t.Start();
                    Reference refer = GantryUtil.GetReference(instBase);
                    MaterialSuper ms = MaterialSuper.ReadFromElement(instBase.Id, doc);
                    if (ms == null) { return false; }

                    //タイプを複製
                    FamilySymbol creatSym = GantryUtil.DuplicateTypeWithNameRule(doc, ms.m_KodaiName, plateSym, plateSym.Name);
                    var element = doc.GetElement(refer);
                    double offset = 0;
                    if (element is Level level)
                    {
                        offset = ClsRevitUtil.GetParameterDouble(doc, instBase.Id, DefineUtil.PARAM_BASE_OFFSET);
                    }
                    else
                    {
                        offset = ClsRevitUtil.GetParameterDouble(doc, instBase.Id, DefineUtil.PARAM_HOST_OFFSET);
                    }
                    XYZ vec = (m_EndPoint - m_InsertPoint).Normalize();
                    ClsRevitUtil.SetTypeParameter(creatSym, "W", ClsRevitUtil.CovertToAPI(plateSize.w));
                    ClsRevitUtil.SetTypeParameter(creatSym, "D", ClsRevitUtil.CovertToAPI(plateSize.d));
                    ClsRevitUtil.SetTypeParameter(creatSym, "H1", ClsRevitUtil.CovertToAPI(plateSize.h));
                    ClsRevitUtil.SetTypeParameter(creatSym, "H2", ClsRevitUtil.CovertToAPI(plateSize.h));

                    XYZ newP = m_InsertPoint + vec * ClsRevitUtil.CovertToAPI(plateSize.h);
                    if (vec.Z > 0) { newP = m_InsertPoint; }

                    FamilyInstance ins = GantryUtil.CreateInstanceWith1point(doc, newP, refer, creatSym, instBase.HandOrientation, followWithHostType: true);
                    if (ins != null)
                    {
                        AttachMaterialSuper(doc, ins.Id, ms.m_KodaiName, ms, size, FrmWaritsukeElement.WaritsukeType.Plate);
                        t.Commit();
                        m_createdIds.Add(ins.Id);
                        m_createdXYZs.Add(m_InsertPoint);
                        retBool = true;
                    }
                    else
                    {
                        retBool = false;
                    }
                }
                catch (Exception ex)
                {
                    retBool = false;
                    _logger.Error(ex.StackTrace);
                }
                finally
                {
                    t.Dispose();
                }
            }
            return retBool;
        }
        #endregion

        #region ジャッキ割付
        /// <summary>
        /// ジャッキ割付
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="instBase"></param>
        /// <param name="jackSym"></param>
        /// <param name="size"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private static bool WaritsukeJack(Document doc, FamilyInstance instBase, FamilySymbol jackSym, string size, double length)
        {
            bool retBool = true;
            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                try
                {
                    t.Start();
                    Reference refer = GantryUtil.GetReference(instBase);
                    MaterialSuper ms = MaterialSuper.ReadFromElement(instBase.Id, doc);
                    if (ms == null) { return false; }

                    //タイプを複製
                    double offset = ClsRevitUtil.GetParameterDouble(doc, instBase.Id, DefineUtil.PARAM_HOST_OFFSET);
                    XYZ vec = (m_EndPoint - m_InsertPoint).Normalize();

                    FamilyInstance ins = GantryUtil.CreateInstanceWith1point(doc, m_InsertPoint, refer, jackSym, vec.Normalize());
                    if (ins != null)
                    {
                        ClsRevitUtil.SetParameter(doc, ins.Id, DefineUtil.PARAM_LENGTH, length);
                        AttachMaterialSuper(doc, ins.Id, ms.m_KodaiName, ms, size, FrmWaritsukeElement.WaritsukeType.Jack);
                        t.Commit();
                        m_createdIds.Add(ins.Id);
                        m_createdXYZs.Add(m_InsertPoint);
                        retBool = true;
                    }
                    else
                    {
                        retBool = false;
                    }
                }
                catch (Exception ex)
                {
                    retBool = false;
                    _logger.Error(ex.StackTrace);
                }
                finally
                {
                    t.Dispose();
                }
            }
            return retBool;
        }
        /// <summary>
        /// ジャッキ割付
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="instBase"></param>
        /// <param name="jackSym"></param>
        /// <param name="size"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private static bool WaritsukeJackToSichu(Document doc, FamilyInstance instBase, FamilySymbol jackSym, string size, double length)
        {
            bool retBool = true;
            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                try
                {
                    t.Start();
                    Reference refer = GantryUtil.GetReference(instBase);
                    MaterialSuper ms = MaterialSuper.ReadFromElement(instBase.Id, doc);
                    if (ms == null) { return false; }
                    XYZ vec = (m_EndPoint - m_InsertPoint).Normalize();
                    double leng = 0;
                    leng+=ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(jackSym,  "調整材長さ_左"));
                    leng+=ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(jackSym, "調整材長さ_右"));

                    XYZ newP = m_InsertPoint + vec * ClsRevitUtil.CovertToAPI(length+leng);
                    if (vec.Z > 0) { newP = m_InsertPoint; }
                    var element = doc.GetElement(refer);
                    double offset = 0;
                    if (element is Level level)
                    {
                        offset = ClsRevitUtil.GetParameterDouble(doc, instBase.Id, DefineUtil.PARAM_BASE_OFFSET);
                    }
                    else
                    {
                        offset = ClsRevitUtil.GetParameterDouble(doc, instBase.Id, DefineUtil.PARAM_HOST_OFFSET);
                    }
                    FamilyInstance ins = GantryUtil.CreateInstanceWith1point(doc, newP, refer, jackSym, instBase.HandOrientation, followWithHostType:false);
                    if (ins != null)
                    {
                        ClsRevitUtil.SetParameter(doc, ins.Id, DefineUtil.PARAM_LENGTH, ClsRevitUtil.CovertToAPI(length));
                        AttachMaterialSuper(doc, ins.Id, ms.m_KodaiName, ms, size, FrmWaritsukeElement.WaritsukeType.Jack);
                        t.Commit();
                        m_createdIds.Add(ins.Id);
                        m_createdXYZs.Add(m_InsertPoint);
                        retBool = true;
                    }
                    else
                    {
                        retBool = false;
                    }
                }
                catch (Exception ex)
                {
                    retBool = false;
                    _logger.Error(ex.StackTrace);
                }
                finally
                {
                    t.Dispose();
                }
            }
            return retBool;
        }
        #endregion

        /// <summary>
        /// 配置部材に部材情報を付与する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id"></param>
        /// <param name="koudaiName"></param>
        /// <param name="materialSuper"></param>
        /// <param name="size"></param>
        private static void AttachMaterialSuper(Document doc, ElementId id, string koudaiName, MaterialSuper materialSuper, string size, FrmWaritsukeElement.WaritsukeType wType)
        {
            if (wType == FrmWaritsukeElement.WaritsukeType.Normal)
            {
                if (materialSuper.GetType() == typeof(Ohbiki))
                {
                    Ohbiki newMs = new Ohbiki();
                    newMs.m_ElementId = id;
                    newMs.m_KodaiName = koudaiName;
                    newMs.m_Size = size;
                    Ohbiki.WriteToElement(newMs, doc);
                }
                else if (materialSuper.GetType() == typeof(Shikigeta))
                {
                    Shikigeta newMs = new Shikigeta();
                    newMs.m_ElementId = id;
                    newMs.m_KodaiName = koudaiName;
                    newMs.m_Size = size;
                    Shikigeta.WriteToElement(newMs, doc);
                }
                else if (materialSuper.GetType() == typeof(Neda))
                {
                    Neda newMs = new Neda();
                    newMs.m_ElementId = id;
                    newMs.m_KodaiName = koudaiName;
                    newMs.m_Size = size;
                    Neda.WriteToElement(newMs, doc);
                }
                else if (materialSuper.GetType() == typeof(FukkouGeta))
                {
                    FukkouGeta newMs = new FukkouGeta();
                    newMs.m_ElementId = id;
                    newMs.m_KodaiName = koudaiName;
                    newMs.m_Size = size;
                    FukkouGeta.WriteToElement(newMs, doc);
                }
                else if (materialSuper.GetType() == typeof(JifukuZuredomezai))
                {
                    JifukuZuredomezai newMs = new JifukuZuredomezai();
                    newMs.m_ElementId = id;
                    newMs.m_KodaiName = koudaiName;
                    newMs.m_Size = size;
                    JifukuZuredomezai.WriteToElement(newMs, doc);
                }
                else if (materialSuper.GetType() == typeof(Tesuri))
                {
                    Tesuri newMs = new Tesuri();
                    newMs.m_ElementId = id;
                    newMs.m_KodaiName = koudaiName;
                    newMs.m_Size = size;
                    Tesuri.WriteToElement(newMs, doc);
                }
                else if (materialSuper.GetType() == typeof(HorizontalJoint))
                {
                    HorizontalJoint newMs = new HorizontalJoint();
                    newMs.m_ElementId = id;
                    newMs.m_KodaiName = koudaiName;
                    newMs.m_Size = size;
                    HorizontalJoint.WriteToElement(newMs, doc);
                }
                else if (materialSuper.GetType() == typeof(Houdue))
                {
                    Houdue newMs = new Houdue();
                    newMs.m_ElementId = id;
                    newMs.m_KodaiName = koudaiName;
                    newMs.m_Size = size;
                    Houdue.WriteToElement(newMs, doc);
                }
                else if (materialSuper.GetType() == typeof(PilePiller))
                {
                    PilePiller newMs = new PilePiller();
                    newMs.m_ElementId = id;
                    newMs.m_KodaiName = koudaiName;
                    newMs.m_Size = size;
                    PilePiller.WriteToElement(newMs, doc);
                }
            }
            else if (wType == FrmWaritsukeElement.WaritsukeType.Pieace)
            {
                HojoPieace tc = new HojoPieace();
                tc.m_ElementId = id;
                tc.m_KodaiName = koudaiName;
                tc.m_Size = size;
                HojoPieace.WriteToElement(tc, doc);
            }
            else if (wType == FrmWaritsukeElement.WaritsukeType.Plate)
            {
                TakasaChouseizai tc = new TakasaChouseizai();
                tc.m_ElementId = id;
                tc.m_KodaiName = koudaiName;
                tc.m_Size = size;
                TakasaChouseizai.WriteToElement(tc, doc);
            }
            else if (wType == FrmWaritsukeElement.WaritsukeType.Jack)
            {
                Jack tc = new Jack();
                tc.m_ElementId = id;
                tc.m_KodaiName = koudaiName;
                tc.m_Size = size;
                Jack.WriteToElement(tc, doc);
            }
        }

        /// <summary>
        /// スケール作成
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="tmpStPoint"></param>
        /// <param name="tmpEdPoint"></param>
        /// <param name="ins"></param>
        /// <param name="eDist"></param>
        /// <param name="maxLength"></param>
        /// <param name="minLength"></param>
        /// <returns></returns>
        public static List<ElementId> CreateScale(UIDocument uidoc, XYZ tmpStPoint, XYZ tmpEdPoint, FamilyInstance ins, WaritukeDist eDist, double maxLength = 7000.0, double minLength = 1000.0)
        {
            List<ElementId> scaleIds = new List<ElementId>();

            Document doc = uidoc.Document;

            int dist = ((int)eDist);
            double convetDist = ClsRevitUtil.CovertToAPI(dist);
            Autodesk.Revit.DB.View vActv = doc.ActiveView;

            Line baseLine = Line.CreateBound(tmpStPoint, tmpEdPoint);
            XYZ dir = baseLine.Direction;
            double baseLength = ClsRevitUtil.CovertFromAPI(baseLine.Length);
            if (baseLength < dist)
                return null;

            //目盛り上限
            double scaleMax = baseLength;
            if (maxLength < scaleMax)
                scaleMax = maxLength;

            //Z方向成分
            XYZ norm = XYZ.BasisZ;
            XYZ base1 = baseLine.GetEndPoint(0);
            XYZ base2 = baseLine.GetEndPoint(1);
            XYZ baseMid = base1 + dir * (ClsRevitUtil.CovertToAPI(baseLength) * 0.5);

            if (ClsGeo.GEO_EQ(dir.Normalize().Z, -1) || ClsGeo.GEO_EQ(dir.Normalize().Z, 1))
            {
                if(vActv.Name.Contains("3D"))
                {
                    norm = dir.CrossProduct(XYZ.BasisX);
                }
                else
                {
                    norm = vActv.ViewDirection;
                }
            }
            else
            {
                XYZ pRd = baseMid - XYZ.BasisZ * ClsRevitUtil.CovertToAPI(10);
                XYZ pPo = GantryUtil.ProjectPointToCurve(pRd, baseLine as Curve);
                norm = (pPo - pRd).Normalize();
            }
          

            //if (norm.Z < 0) { norm = norm.Negate(); }
            //if (dir.Normalize().Z.Equals(0))
            //{
            //    norm = XYZ.BasisZ;
            //}

            //if (!vActv.Name.Contains("3D") && (ClsGeo.GEO_EQ0(dir.Z) || ClsGeo.GEO_EQ(dir.Normalize().Z, -1))|| norm.DistanceTo(new XYZ(-1, 0, 0)) < 0.1)
            //{
            //    norm = vActv.ViewDirection;
            //}

            //norm = XYZ.BasisZ;

            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                try
                {
                    // スケールの作成
                    t.Start();
                    for (int i = 0; i <= scaleMax / dist; i++)
                    {
                        XYZ newPoint = tmpStPoint + dir.Normalize() * ClsRevitUtil.CovertToAPI(i * dist);
                        Curve cv;
                        if (i % 10 == 0)
                        {
                            cv = CreateCurveToPoint(newPoint, dir, convetDist * 5);

                        }
                        else if (i % 5 == 0)
                        {
                            cv = CreateCurveToPoint(newPoint, dir, convetDist * 2.5);
                        }
                        else
                        {
                            cv = CreateCurveToPoint(newPoint, dir, convetDist);
                        }

                        XYZ mid = 0.5 * (cv.GetEndPoint(0) + cv.GetEndPoint(1));
                        Plane plane = Plane.CreateByNormalAndOrigin(norm, mid);
                        SketchPlane sketchPlane = SketchPlane.Create(doc, plane);
                        
                        ModelLine modelLine = doc.Create.NewModelCurve(cv as Line, sketchPlane) as ModelLine;
                        scaleIds.Add(modelLine.Id);

                        Line axis = Line.CreateBound(newPoint, newPoint + norm);
                        ClsRevitUtil.RotateElement(doc, modelLine.Id, axis, Math.PI / 2);

                        if (i % 10 == 0)
                        {
                            cv = (doc.GetElement(modelLine.Id).Location as LocationCurve).Curve;
                            ElementId defaultTypeId = doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);

                            TextNoteOptions opts = new TextNoteOptions(defaultTypeId);
                            double maxWidth = TextNote.GetMaximumAllowedWidth(doc, defaultTypeId);
                            maxWidth = ClsRevitUtil.CovertToAPI(maxWidth);
                            XYZ txtDir = (cv as Line).Direction;
                            XYZ txtPoint = cv.GetEndPoint(1) + dir.Negate() * ClsRevitUtil.CovertToAPI(100) + txtDir * ClsRevitUtil.CovertToAPI(200);
                            TextNote tx = TextNote.Create(doc, vActv.Id, txtPoint, maxWidth, (i * dist / 1000.0).ToString() + ".0", opts);

                            TextElementType textType = tx.Symbol;
                            BuiltInParameter paraIndex = BuiltInParameter.TEXT_SIZE;
                            Parameter textSize = textType.get_Parameter(paraIndex);

                            textSize.Set(maxWidth / 5);

                            paraIndex = BuiltInParameter.TEXT_BACKGROUND;
                            Parameter textBack = textType.get_Parameter(paraIndex);
                            textBack.Set(1);// 0 = Opaque（不透明） :: 1 = Transparent（透過）
                            scaleIds.Add(tx.Id);

                        }
                    }
                    t.Commit();
                }
                catch (OperationCanceledException e)
                {
                }
                finally
                {
                    // 必ずトランザクションを終了させる
                    t.Dispose();
                }
            }
            return scaleIds;
        }

        /// <summary>
        /// スケール削除
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="scaleIds"></param>
        private static void DeleteScale(Document doc, List<ElementId> scaleIds)
        {
            // スケールの削除
            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                t.Start();
                ClsRevitUtil.Delete(doc, scaleIds);
                t.Commit();
            }
        }

        /// <summary>
        /// 指定の点からdirの向きに指定距離離したCurve
        /// </summary>
        /// <param name="point">指定点</param>
        /// <param name="dir">向き</param>
        /// <param name="dist">距離</param>
        /// <returns></returns>
        private static Curve CreateCurveToPoint(XYZ point, XYZ dir, double dist)
        {
            point = point + dir.Negate() * dist / 2;
            XYZ endP = point + dir * dist;
            Curve cv = Line.CreateBound(point, endP);
            return cv;
        }

        /// <summary>
        /// 割付部材指定
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool GetTargetElement(ref ElementId id)
        {
            bool retB = false;
            try
            {
                Selection selection = _uiDoc.Selection;
                YMSWaritsukeFilter filter = new YMSWaritsukeFilter(_doc);
                ElementId selId = selection.PickObject(ObjectType.Element, filter).ElementId;
                if (selId == null || selId == ElementId.InvalidElementId)
                {
                    return false;
                }
                id = selId;
                return true;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// 割付の始点側を指定させる
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="tmpStPoint"></param>
        /// <param name="tmpEdPoint"></param>
        /// <returns></returns>
        public static (XYZ, XYZ) GetPickPoint(UIDocument uidoc, XYZ tmpStPoint, XYZ tmpEdPoint)
        {
            Selection selection = uidoc.Selection;
            Curve cvBase = Line.CreateBound(tmpStPoint, tmpEdPoint);
            XYZ dir = Line.CreateBound(tmpStPoint, tmpEdPoint).Direction;

            XYZ pickPoint = null;
            try
            {
                pickPoint = selection.PickPoint("割付の始点側を指定してください");

            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException ex)
            {
                //中断
                throw ex;
            }
            catch (Exception ex)
            {
                MessageUtil.Information("アクティブなビューに作業面がないため、割付開始点を選択できません。\r\n端点間で自動的に割付方向を指定します", "割付");
                return (tmpStPoint, tmpEdPoint);
            }

            if (pickPoint.DistanceTo(tmpStPoint) > pickPoint.DistanceTo(tmpEdPoint))
            {
                return (tmpEdPoint, tmpStPoint);
            }
            else
            {
                return (tmpStPoint, tmpEdPoint);
            }
        }

        /// <summary>
        /// エレメントの表示状態を切り替える
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id"></param>
        /// <param name="hide"></param>
        private static void ChangeElementVisible(Document doc, ElementId id, bool hide)
        {
            FamilyInstance ins = doc.GetElement(id) as FamilyInstance;

            if (ins == null) { return; }
            List<ElementId> ids = new List<ElementId>() { id };
            var subs = ins.GetSubComponentIds();
            if (subs.Count > 0)
            {
                foreach (ElementId idsub in subs)
                {
                    Element s = doc.GetElement(idsub);
                    if (s is FamilyInstance)
                    {
                        ids.Add(idsub);
                    }
                }
            }
            if (hide)
            {
                RevitUtil.ClsRevitUtil.HideElement(doc, ids);
            }
            else
            {
                RevitUtil.ClsRevitUtil.UnhideElement(doc, ids);

            }
        }
    }
}
