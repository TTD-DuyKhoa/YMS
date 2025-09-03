using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YMS.gantry;
using YMS_gantry.Material;

namespace YMS_gantry.UI
{
    public partial class FrmEditLocationChange : System.Windows.Forms.Form
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private const string FORM_NAME = "位置変更";

        private RequestHandler m_Handler;
        private ExternalEvent m_ExEvent;

        private UIDocument _uiDoc;
        public ElementId _targetElementId;
        private List<string> _koudaiNameList;
        private List<Bunrui> _bunruiList;

        private List<ElementId> _selectedElemetIdList;

        // 対象部材の初期位置
        public XYZ _originalLocation;

        public FrmEditLocationChange(ExternalEvent exEvent, RequestHandler handler, UIApplication uiApp, ElementId targetElementId)
        {
            InitializeComponent();

            _uiDoc = uiApp.ActiveUIDocument;
            _targetElementId = targetElementId;

            _koudaiNameList = new List<string>();
            _bunruiList = new List<Bunrui>();

            _selectedElemetIdList = new List<ElementId>();

            m_Handler = handler;
            m_ExEvent = exEvent;
        }

        private void FrmEditLocationChange_Load(object sender, EventArgs e)
        {
            try
            {
                // Revit上でのファミリ選択状態を解除
                _uiDoc.Selection.SetElementIds(new List<ElementId>());

                var targetFi = GantryUtil.GetFamilyInstance(_uiDoc.Document, _targetElementId);

                if (targetFi == null)
                {
                    MessageUtil.Warning("対象にできない部材が選択されています。\n画面を閉じてください。", FORM_NAME, this);
                    this.BtnMoveStart.Enabled = false;
                    this.BtnMoveEnd.Enabled = false;
                    this.BtnTargetReSelect.Enabled = false;
                    this.BtnSelectedAll.Enabled = false;
                    this.BtnUnSelectedAll.Enabled = false;
                    this.groupBox1.Enabled = false;
                    this.DgvEditLocationChangeList.Enabled = false;
                }
                else
                {
                    InitComponent(targetFi);
                    InitData();
                }
            }
            catch (Exception ex)
            {
                MessageUtil.Error("画面のロード処理に失敗しました。", FORM_NAME, this);
                logger.Error(ex, ex.Message);
            }
        }

        private void InitComponent(FamilyInstance targetFi)
        {
            var doc = _uiDoc.Document;
            var intersectElementIdList = ClsRevitUtil.GetIntersectFamilys(doc, _targetElementId, ignorelist:new List<string>() { targetFi.Name });

            foreach (var intersectElementId in intersectElementIdList)
            {
                var intersectFi = GantryUtil.GetFamilyInstance(doc, intersectElementId);

                if (intersectFi == null) continue;

                // ネストされたファミリである場合はスキップ
                if (intersectFi.SuperComponent != null) continue;
                // 対象をホストに持つ場合はスキップ
                if (intersectFi.Host != null && intersectFi.Host.Id.IntegerValue == _targetElementId.IntegerValue) continue;

                // エレメント取得
                var element = doc.GetElement(intersectFi.Id);
                if (element == null) continue;

                var material = MaterialSuper.ReadFromElement(intersectFi.Id, doc);
                if (material == null) continue;

                // ファミリ名取得
                var familyName = GantryUtil.GetFamilyName(element);

                // 「構台基点ファミリ」の場合はスキップ
                if (familyName == KoudaiReference.familyName) continue;

                // タイプ名取得
                var typeName = GantryUtil.GetTypeName(element);
                // ファミリシンボル取得
                var familySymbol = ClsRevitUtil.GetFamilySymbol(doc, familyName, typeName);

                if (familySymbol == null) continue;

                var id = element.UniqueId;
                var elemId = element.Id.IntegerValue;
                var koudaiName = material?.m_KodaiName;
                var daiBunrui = GantryUtil.GetTypeParameter(familySymbol, DefineUtil.PARAM_DAI_BUNRUI);
                var chuuBunrui = GantryUtil.GetTypeParameter(familySymbol, DefineUtil.PARAM_CHUU_BUNRUI);
                var shouBunrui = GantryUtil.GetTypeParameter(familySymbol, DefineUtil.PARAM_SHOU_BUNRUI);

                var existsKoudaiName = _koudaiNameList.Exists(x => x == koudaiName);
                if (!existsKoudaiName)
                {
                    // 配置ファミリの構台名を抽出
                    _koudaiNameList.Add(koudaiName);
                }

                var existsBunrui = _bunruiList.Exists(x => x.Dai == daiBunrui && x.Chuu == chuuBunrui && x.Shou == shouBunrui);
                if (!existsBunrui)
                {
                    // 全ての分類が空の対象は抽出しない
                    if (string.IsNullOrWhiteSpace(daiBunrui) && string.IsNullOrWhiteSpace(chuuBunrui) && string.IsNullOrWhiteSpace(shouBunrui)) return;

                    // 図面内に配置されたファミリの分類情報を抽出
                    _bunruiList.Add(new Bunrui { Dai = daiBunrui, Chuu = chuuBunrui, Shou = shouBunrui });
                }
            }

            // 空の選択肢を追加
            this.CmbDaiBunrui.Items.Add(string.Empty);
            this.CmbChuuBunrui.Items.Add(string.Empty);
            this.CmbShouBunrui.Items.Add(string.Empty);

            foreach (var bunrui in _bunruiList)
            {
                if (!this.CmbDaiBunrui.Items.Contains(bunrui.Dai))
                {
                    // 大分類コンボに値をセット
                    this.CmbDaiBunrui.Items.Add(bunrui.Dai);
                }

                if (!this.CmbChuuBunrui.Items.Contains(bunrui.Chuu))
                {
                    // 中分類コンボに値をセット
                    this.CmbChuuBunrui.Items.Add(bunrui.Chuu);
                }

                if (!this.CmbShouBunrui.Items.Contains(bunrui.Shou))
                {
                    // 小分類コンボに値をセット
                    this.CmbShouBunrui.Items.Add(bunrui.Shou);
                }
            }

            foreach (var _koudaiName in _koudaiNameList)
            {
                // 構台名リストに値をセット
                this.LstKoudai.Items.Add(_koudaiName);
            }
        }

        public void InitData()
        {
            this.DgvEditLocationChangeList.ClearSelection();

            var rowDataList = CreateRowDataList(_targetElementId);

            // 位置変更データグリッドに値をセット
            foreach (var rowData in rowDataList)
            {
                this.DgvEditLocationChangeList.Rows.Add(new object[] { rowData.Id, rowData.ElementId, false, rowData.KoudaiName, rowData.DaiBunrui, rowData.ChuuBunrui, rowData.ShouBunrui, rowData.FamilyName });
            }
        }

        private List<RowData> CreateRowDataList(ElementId targetElementId)
        {
            var rowDataList = new List<RowData>();

            var doc = _uiDoc.Document;

            var targetFi = GantryUtil.GetFamilyInstance(doc, targetElementId);
            var intersectElementIdList = ClsRevitUtil.GetIntersectFamilys(doc, targetElementId, ignorelist:new List<string>() { targetFi.Name });

            foreach (var intersectElementId in intersectElementIdList)
            {
                var intersectFi = GantryUtil.GetFamilyInstance(doc, intersectElementId);

                if (intersectFi == null) continue;

                //// ネストされたファミリである場合はスキップ
                //if (intersectFi.SuperComponent != null) continue;
                //// 対象をホストに持つ場合はスキップ
                //if (intersectFi.Host != null && intersectFi.Host.Id.IntegerValue == targetElementId.IntegerValue) continue;

                //// エレメント取得
                //var element = doc.GetElement(intersectFi.Id);
                //if (element == null) continue;

                //var material = MaterialSuper.ReadFromElement(intersectFi.Id, doc);
                //if (material == null) continue;

                //// ファミリ名取得
                //var familyName = GantryUtil.GetFamilyName(element);

                //// 「構台基点ファミリ」の場合はスキップ
                //if (familyName == KoudaiReference.familyName) continue;

                //// タイプ名取得
                //var typeName = GantryUtil.GetTypeName(element);
                //// ファミリシンボル取得
                //var familySymbol = ClsRevitUtil.GetFamilySymbol(doc, familyName, typeName);

                //if (familySymbol == null) continue;

                //var id = element.UniqueId;
                //var elemId = element.Id.IntegerValue;
                //var koudaiName = material?.m_KodaiName;
                //var daiBunrui = GantryUtil.GetTypeParameter(familySymbol, DefineUtil.PARAM_DAI_BUNRUI);
                //var chuuBunrui = GantryUtil.GetTypeParameter(familySymbol, DefineUtil.PARAM_CHUU_BUNRUI);
                //var shouBunrui = GantryUtil.GetTypeParameter(familySymbol, DefineUtil.PARAM_SHOU_BUNRUI);

                //var rowData = new RowData { Id = id, ElementId = elemId, KoudaiName = koudaiName, DaiBunrui = daiBunrui, ChuuBunrui = chuuBunrui, ShouBunrui = shouBunrui, FamilyName = familyName };

                var rowData = CreateRowData(doc, intersectElementId, targetElementId);
                if(rowData!=null)
                {
                    rowDataList.Add(rowData);
                }

                //継足し杭だったらその下の杭も表示する
                if(intersectFi.Symbol.Name.StartsWith("継ぎ足し杭"))
                {
                    var pileData = GetPileData(doc, intersectFi.Id);
                    if(pileData!=null)
                    {
                        rowDataList.Add(pileData);
                    }
                }
            }

            return rowDataList;
        }

        /// <summary>
        /// データ作成
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="targetId"></param>
        /// <param name="hostId"></param>
        /// <returns></returns>
        private RowData CreateRowData(Document doc,ElementId targetId,ElementId hostId)
        {
            RowData retData = null;
            try
            {
                var intersectFi = GantryUtil.GetFamilyInstance(doc, targetId);

                if (intersectFi == null) return null;

                // ネストされたファミリである場合はスキップ
                if (intersectFi.SuperComponent != null) return null;
                // 対象をホストに持つ場合はスキップ
                if (intersectFi.Host != null && intersectFi.Host.Id.IntegerValue == hostId.IntegerValue) return null;

                // エレメント取得
                var element = doc.GetElement(intersectFi.Id);
                if (element == null) return null;

                var material = MaterialSuper.ReadFromElement(intersectFi.Id, doc);
                if (material == null) return null;

                // ファミリ名取得
                var familyName = GantryUtil.GetFamilyName(element);

                // 「構台基点ファミリ」の場合はスキップ
                if (familyName == KoudaiReference.familyName) return null;

                // タイプ名取得
                var typeName = GantryUtil.GetTypeName(element);
                // ファミリシンボル取得
                var familySymbol = ClsRevitUtil.GetFamilySymbol(doc, familyName, typeName);

                if (familySymbol == null) return null;

                var id = element.UniqueId;
                var elemId = element.Id.IntegerValue;
                var koudaiName = material?.m_KodaiName;
                var daiBunrui = GantryUtil.GetTypeParameter(familySymbol, DefineUtil.PARAM_DAI_BUNRUI);
                var chuuBunrui = GantryUtil.GetTypeParameter(familySymbol, DefineUtil.PARAM_CHUU_BUNRUI);
                var shouBunrui = GantryUtil.GetTypeParameter(familySymbol, DefineUtil.PARAM_SHOU_BUNRUI);

                retData = new RowData { Id = id, ElementId = elemId, KoudaiName = koudaiName, DaiBunrui = daiBunrui, ChuuBunrui = chuuBunrui, ShouBunrui = shouBunrui, FamilyName = familyName };
            }
            catch (Exception)
            {
                retData = null;
            }
            return retData;
        }

        private RowData GetPileData(Document doc, ElementId exPileId)
        {
            RowData retData = null;
            try
            {
                var targetFi = GantryUtil.GetFamilyInstance(doc, exPileId);
                var intersectElementIdList = ClsRevitUtil.GetIntersectFamilys(doc, exPileId, ignorelist: new List<string>() { targetFi.Name });
                foreach (var intersectElementId in intersectElementIdList)
                {
                    var intersectFi = GantryUtil.GetFamilyInstance(doc, intersectElementId);
                    if (intersectFi == null) continue;
                    if (!intersectFi.Symbol.Name.StartsWith("支持杭")) continue;

                    var tmp = CreateRowData(doc,intersectElementId, exPileId);
                    if (tmp != null)
                    {
                        retData=tmp;
                        break;
                    }
                }
            }
            catch (Exception)
            {
                retData= null;
            }
            return retData;
        }

        private class Bunrui
        {
            public string Dai { get; set; }
            public string Chuu { get; set; }
            public string Shou { get; set; }
        }

        private class RowData
        {
            public string Id { get; set; }
            public int ElementId { get; set; }
            public string KoudaiName { get; set; }
            public string DaiBunrui { get; set; }
            public string ChuuBunrui { get; set; }
            public string ShouBunrui { get; set; }
            public string FamilyName { get; set; }
        }

        private void DgvEditLocationChangeList_SelectionChanged(object sender, EventArgs e)
        {
            _selectedElemetIdList.Clear();

            var selectedRowList = new List<DataGridViewRow>();

            foreach (DataGridViewRow row in this.DgvEditLocationChangeList.SelectedRows)
            {
                // 選択されたRowを取得
                selectedRowList.Add(row);
            }

            // 選択されたRowよりID列の値を取得
            var selectedUniqueIdsList = selectedRowList.Select(x => x.Cells["ColLocationChangeListID"].Value.ToString());

            foreach (var uniqueId in selectedUniqueIdsList)
            {
                // ID列よりElementIDのリストを生成
                var element = _uiDoc.Document.GetElement(uniqueId);
                if (element == null) continue;

                _selectedElemetIdList.Add(element.Id);
            }

            _uiDoc.Selection.SetElementIds(_selectedElemetIdList);
        }

        private void BtnMoveStart_Click(object sender, EventArgs e)
        {
            MessageUtil.Information("選択部材の移動を開始してください。\r\n移動完了後は「移動終了」ボタンを押下して位置を確定してください。","移動",this);
            MaterialSuper ms = MaterialSuper.ReadFromElement(_targetElementId, _uiDoc.Document);
            _originalLocation = ms.Position;
            _uiDoc.Selection.SetElementIds(new List<ElementId>() {_targetElementId});
        }

        private void BtnMoveEnd_Click(object sender, EventArgs e)
        {
            MakeRequest(RequestId.EditLocation);
        }

        private void BtnTargetReSelect_Click(object sender, EventArgs e)
        {
            try
            {
                // 対象部材を再指定処理
                var selectedElement = _uiDoc.Selection.PickObject(ObjectType.Element, "対象部材を指定してください。");
                var element = _uiDoc.Document.GetElement(selectedElement.ElementId);

                // 対象部材を更新
                _targetElementId = element.Id;
            }
            catch (Exception ex)
            {
                MessageUtil.Error("選択できない部材が指定されている可能性があります。", FORM_NAME, this);
                logger.Error(ex, ex.Message);
            }

            Search();
        }

        private void BtmCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmEditLocationChange_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Revit上でのファミリ選択状態を解除
            _uiDoc.Selection.SetElementIds(new List<ElementId>());

            //初期化処理
            MakeRequest(RequestId.End);
            m_ExEvent.Dispose();
            m_ExEvent = null;
            m_Handler = null;
        }

        private void BtnSelectedAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.DgvEditLocationChangeList.RowCount; i++)
            {
                this.DgvEditLocationChangeList["ColLocationChangeListSelected", i].Value = true;
            }
        }

        private void BtnUnSelectedAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.DgvEditLocationChangeList.RowCount; i++)
            {
                this.DgvEditLocationChangeList["ColLocationChangeListSelected", i].Value = false;
            }
        }

        private void 選択ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in this.DgvEditLocationChangeList.SelectedRows)
            {
                row.Cells["ColLocationChangeListSelected"].Value = true;
            }
        }

        private void 解除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in this.DgvEditLocationChangeList.SelectedRows)
            {
                row.Cells["ColLocationChangeListSelected"].Value = false;
            }
        }

        /// <summary>
        /// フィルタ検索
        /// </summary>
        public void Search()
        {
            try
            {
                // 位置変更データグリッドに値をセット
                var rowDataList = CreateRowDataList(_targetElementId);

                var selectedKoudaiNameList = new List<string>();
                foreach (string koudaiName in this.LstKoudai.SelectedItems)
                {
                    selectedKoudaiNameList.Add(koudaiName);
                }

                var filterList = rowDataList.AsEnumerable();
                if (selectedKoudaiNameList.Count > 0)
                {
                    filterList = rowDataList.Where(x => selectedKoudaiNameList.Exists(t => !string.IsNullOrEmpty(x.KoudaiName) && t.Contains(x.KoudaiName)));
                }

                var selectedDaiBunrui = this.CmbDaiBunrui.SelectedItem?.ToString();
                if (!string.IsNullOrEmpty(selectedDaiBunrui))
                {
                    filterList = filterList.Where(x => x.DaiBunrui == selectedDaiBunrui);
                }

                var selectedChuuBunrui = this.CmbChuuBunrui.SelectedItem?.ToString();
                if (!string.IsNullOrEmpty(selectedChuuBunrui))
                {
                    filterList = filterList.Where(x => x.ChuuBunrui == selectedChuuBunrui);
                }

                var selectedShouBunrui = this.CmbShouBunrui.SelectedItem?.ToString();
                if (!string.IsNullOrEmpty(selectedShouBunrui))
                {
                    filterList = filterList.Where(x => x.ShouBunrui == selectedShouBunrui);
                }

                // 位置変更データグリッドを初期化
                this.DgvEditLocationChangeList.Rows.Clear();

                foreach (var rowData in filterList)
                {
                    this.DgvEditLocationChangeList.Rows.Add(new object[] { rowData.Id, rowData.ElementId, false, rowData.KoudaiName, rowData.DaiBunrui, rowData.ChuuBunrui, rowData.ShouBunrui, rowData.FamilyName });
                }
            }
            catch (Exception ex)
            {
                MessageUtil.Error("位置変更リストのフィルター処理に失敗しました。", FORM_NAME, this);
                logger.Error(ex, ex.Message);
            }
        }

        private void LstKoudai_SelectedIndexChanged(object sender, EventArgs e)
        {
            Search();
        }

        private void CmbDaiBunrui_SelectedIndexChanged(object sender, EventArgs e)
        {
            Search();
        }

        private void CmbChuuBunrui_SelectedIndexChanged(object sender, EventArgs e)
        {
            Search();
        }

        private void CmbShouBunrui_SelectedIndexChanged(object sender, EventArgs e)
        {
            Search();
        }

        #region "モードレス対応"
        /// <summary>
        ///   WakeUp -> enable all controls
        /// </summary>
        /// 
        public void WakeUp()
        {
            EnableCommands(true);
        }
        /// <summary>
        ///   Control enabler / disabler 
        /// </summary>
        ///
        private void EnableCommands(bool status)
        {
            foreach (System.Windows.Forms.Control ctrl in this.Controls)
            {
                ctrl.Enabled = status;
            }
            //if (!status)
            //{
            //    this.button3.Enabled = true;
            //}
        }
        private void MakeRequest(RequestId request)
        {
            m_Handler.Request.Make(request);
            m_ExEvent.Raise();
            DozeOff();
        }
        /// <summary>
        ///   DozeOff -> disable all controls (but the Exit button)
        /// </summary>
        /// 
        private void DozeOff()
        {
            EnableCommands(false);
        }
        #endregion
    }
}
