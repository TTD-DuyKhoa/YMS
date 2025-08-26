using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using YMS.gantry;
using YMS_gantry.Material;

namespace YMS_gantry.UI
{
    public partial class FrmEditLengthChange : System.Windows.Forms.Form
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private const string FORM_NAME = "長さ変更";

        private RequestHandler m_Handler;
        private ExternalEvent m_ExEvent;

        private UIDocument _uiDoc;
        private List<ElementId> _targetElementIdList;
        public List<string> _targetElementUniqueIdList;
        private List<string> _koudaiNameList;
        private List<Bunrui> _bunruiList;

        private List<ElementId> _selectedElemetIdList;

        public FrmEditLengthChange(ExternalEvent exEvent, RequestHandler handler, UIApplication uiApp, List<ElementId> targetElementIdList)
        {
            InitializeComponent();

            _uiDoc = uiApp.ActiveUIDocument;
            _targetElementIdList = targetElementIdList;
            _targetElementUniqueIdList = new List<string>();

            // ElementId は値が変更されるケースがあるらしく怪しいので UniqueId を利用する
            foreach (var elementId in targetElementIdList)
            {
                var element = _uiDoc.Document.GetElement(elementId);
                if (element == null) continue;
                _targetElementUniqueIdList.Add(element.UniqueId);
            }

            _koudaiNameList = new List<string>();
            _bunruiList = new List<Bunrui>();

            _selectedElemetIdList = new List<ElementId>();

            m_Handler = handler;
            m_ExEvent = exEvent;
        }

        private void FrmEditLength_Load(object sender, EventArgs e)
        {
            try
            {
                // Revit上でのファミリ選択状態を解除
                _uiDoc.Selection.SetElementIds(new List<ElementId>());

                InitComponent();
                InitData();
            }
            catch (Exception ex)
            {
                MessageUtil.Error("画面のロード処理に失敗しました。", FORM_NAME, this);
                logger.Error(ex, ex.Message);
            }
        }

        private void InitComponent()
        {
            foreach (var elementUniqueId in _targetElementUniqueIdList)
            {
                // エレメント取得
                var element = _uiDoc.Document.GetElement(elementUniqueId);
                if (element == null) continue;

                var elementId = element.Id;

                // ファミリ名取得
                var familyName = GantryUtil.GetFamilyName(_uiDoc.Document.GetElement(elementId));
                // タイプ名取得
                var typeName = GantryUtil.GetTypeName(_uiDoc.Document.GetElement(elementId));
                // ファミリインスタンス取得
                var familyInstance = GantryUtil.GetFamilyInstance(_uiDoc.Document, elementId);
                // ファミリシンボル取得
                var familySymbol = ClsRevitUtil.GetFamilySymbol(_uiDoc.Document, familyName, typeName);

                if (familyInstance == null || familySymbol == null) continue;

                var material = MaterialSuper.ReadFromElement(familyInstance.Id, _uiDoc.Document);
                if (material == null) continue;

                // ファミリパラメータから長さを取得
                var lengthByParam = ClsRevitUtil.GetParameter(_uiDoc.Document, elementId, DefineUtil.PARAM_LENGTH);
                // タイプパラメータから長さを取得
                var lengthByTypeParam = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(familySymbol, DefineUtil.PARAM_LENGTH)).ToString();

                // 「構台基点ファミリ」の場合はレコード追加をスキップ
                if (familyName == KoudaiReference.familyName) continue;

                // 長さパラメータを取得できないファミリの場合はレコード追加をスキップ
                if (string.IsNullOrEmpty(lengthByParam) && (string.IsNullOrEmpty(lengthByTypeParam) || double.Parse(lengthByTypeParam) == 0.0)) continue;
                var length = string.IsNullOrEmpty(lengthByParam) ? lengthByTypeParam : lengthByParam;

                var id = element.UniqueId;
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
                    if (string.IsNullOrWhiteSpace(daiBunrui) && string.IsNullOrWhiteSpace(chuuBunrui) && string.IsNullOrWhiteSpace(shouBunrui)) continue;

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

            foreach (var koudaiName in _koudaiNameList)
            {
                // 構台名リストに値をセット
                this.LstKoudai.Items.Add(koudaiName);
            }
        }

        public void InitData()
        {
            this.DgvEditLengthChangeList.ClearSelection();

            var rowDataList = CreateRowDataList(_targetElementUniqueIdList);

            // 長さ変更データグリッドに値をセット
            foreach (var rowData in rowDataList)
            {
                this.DgvEditLengthChangeList.Rows.Add(new object[] { rowData.Id, rowData.ElementId, false, rowData.KoudaiName, rowData.DaiBunrui, rowData.ChuuBunrui, rowData.ShouBunrui, rowData.FamilyName, rowData.Length, });
            }
        }

        private List<RowData> CreateRowDataList(List<string> elementUniqueIds)
        {
            var rowDataList = new List<RowData>();

            foreach (var elementUniqueId in elementUniqueIds)
            {
                // エレメント取得
                var element = _uiDoc.Document.GetElement(elementUniqueId);
                if (element == null) continue;

                var elementId = element.Id;

                // ファミリ名取得
                var familyName = GantryUtil.GetFamilyName(element);
                // タイプ名取得
                var typeName = GantryUtil.GetTypeName(element);
                // ファミリインスタンス取得
                var familyInstance = GantryUtil.GetFamilyInstance(_uiDoc.Document, elementId);
                // ファミリシンボル取得
                var familySymbol = ClsRevitUtil.GetFamilySymbol(_uiDoc.Document, familyName, typeName);

                if (familyInstance == null || familySymbol == null) continue;

                var material = MaterialSuper.ReadFromElement(familyInstance.Id, _uiDoc.Document);

                // ファミリパラメータから長さを取得
                var lengthByParam = ClsRevitUtil.GetParameter(_uiDoc.Document, elementId, DefineUtil.PARAM_LENGTH);
                // タイプパラメータから長さを取得
                var lengthByTypeParam = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(familySymbol, DefineUtil.PARAM_LENGTH)).ToString();

                // 「構台基点ファミリ」の場合はレコード追加をスキップ
                if (familyName == KoudaiReference.familyName) continue;

                // 長さパラメータを取得できないファミリの場合はレコード追加をスキップ
                if (string.IsNullOrEmpty(lengthByParam) && (string.IsNullOrEmpty(lengthByTypeParam) || double.Parse(lengthByTypeParam) == 0.0)) continue;
                var length = string.IsNullOrEmpty(lengthByParam) ? lengthByTypeParam : lengthByParam;

                var id = element.UniqueId;
                var elemId = element.Id.IntegerValue;
                var koudaiName = material?.m_KodaiName;
                var daiBunrui = GantryUtil.GetTypeParameter(familySymbol, DefineUtil.PARAM_DAI_BUNRUI);
                var chuuBunrui = GantryUtil.GetTypeParameter(familySymbol, DefineUtil.PARAM_CHUU_BUNRUI);
                var shouBunrui = GantryUtil.GetTypeParameter(familySymbol, DefineUtil.PARAM_SHOU_BUNRUI);

                var rowData = new RowData { Id = id, ElementId = elemId, KoudaiName = koudaiName, DaiBunrui = daiBunrui, ChuuBunrui = chuuBunrui, ShouBunrui = shouBunrui, FamilyName = familyName, Length = length };

                rowDataList.Add(rowData);
            }

            return rowDataList;
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
            public string Length { get; set; }
        }

        private void DgvEditLengthList_SelectionChanged(object sender, EventArgs e)
        {
            // 選択レコードに該当する部材を選択(ハイライト)するサンプル実装。
            if (_targetElementUniqueIdList.Count() != 0)
            {
                _selectedElemetIdList.Clear();

                var selectedRowList = new List<DataGridViewRow>();

                foreach (DataGridViewRow row in this.DgvEditLengthChangeList.SelectedRows)
                {
                    // 選択されたRowを取得
                    selectedRowList.Add(row);
                }

                // 選択されたRowよりID列の値を取得
                var selectedUniqueIdsList = selectedRowList.Select(x => x.Cells["ColLengthChangeListID"].Value.ToString());

                foreach (var uniqueId in selectedUniqueIdsList)
                {
                    // ID列よりElementIDのリストを生成
                    var element = _uiDoc.Document.GetElement(uniqueId);
                    if (element == null) continue;

                    _selectedElemetIdList.Add(element.Id);
                }

                _uiDoc.Selection.SetElementIds(_selectedElemetIdList);
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in this.DgvEditLengthChangeList.Rows)
            {
                // 入力値検証
                var afterLength = row.Cells["ColLengthChangeListAfterLength"].Value?.ToString();
                if (!string.IsNullOrEmpty(afterLength) && !double.TryParse(afterLength, out double afterLengthTemp))
                {
                    // 入力値がdoubleに変換できない場合はエラー
                    MessageUtil.Warning("入力値に不正な値が含まれています。", FORM_NAME, this);
                    return;
                }

                if (string.IsNullOrEmpty(afterLength)) continue;

                var familyName = row.Cells["ColLengthChangeListFamilyName"].Value?.ToString();
                if (!string.IsNullOrEmpty(familyName) && familyName.Contains("HA_"))
                {
                    var afterLengthValue = double.Parse(afterLength);
                    if (afterLengthValue < 1000 || afterLengthValue > 7000 || afterLengthValue % 500 != 0)
                    {
                        // 入力値が山留主材の定形長さ外の場合はエラー
                        MessageUtil.Warning("入力値に引きあたる山留主材がありません。", FORM_NAME, this);
                        return;
                    }
                }

                if (!string.IsNullOrEmpty(familyName) && familyName.Contains("SMH_"))
                {
                    var afterLengthValue = double.Parse(afterLength);
                    if (afterLengthValue < 1000 || afterLengthValue > 9000 || afterLengthValue % 500 != 0)
                    {
                        // 入力値が高強度山留主材の定形長さ外の場合はエラー
                        MessageUtil.Warning("入力値に引きあたる高強度山留主材がありません。", FORM_NAME, this);
                        return;
                    }
                }
            }

            MakeRequest(RequestId.EditLengthChange);
        }

        private void FrmEditLength_FormClosed(object sender, FormClosedEventArgs e)
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
            for (int i = 0; i < this.DgvEditLengthChangeList.RowCount; i++)
            {
                this.DgvEditLengthChangeList["ColLengthChangeListSelected", i].Value = true;
            }
        }

        private void BtnUnSelectedAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.DgvEditLengthChangeList.RowCount; i++)
            {
                this.DgvEditLengthChangeList["ColLengthChangeListSelected", i].Value = false;
            }
        }

        private void 選択ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in this.DgvEditLengthChangeList.SelectedRows)
            {
                row.Cells["ColLengthChangeListSelected"].Value = true;
            }
        }

        private void 解除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in this.DgvEditLengthChangeList.SelectedRows)
            {
                row.Cells["ColLengthChangeListSelected"].Value = false;
            }
        }

        private void DgvEditLengthChangeList_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            //編集中のセル
            var cellEditing = this.DgvEditLengthChangeList.Rows[e.RowIndex].Cells[e.ColumnIndex];
            // 選択中セルの変更対象が「変更後長さ」でない場合は処理スキップ
            if (cellEditing.OwningColumn.Name != "ColLengthChangeListAfterLength") return;

            var cellEditingCheck = this.DgvEditLengthChangeList.Rows[e.RowIndex].Cells["ColLengthChangeListSelected"] as DataGridViewCheckBoxCell;
            // 選択中セルのチェック状態が有効でない場合は処理スキップ
            if (!(bool)cellEditingCheck.Value) return;

            //編集中の列が「変更後長さ」の場合
            for (var i = 0; i < this.DgvEditLengthChangeList.RowCount; i++)
            {
                var cellCheck = this.DgvEditLengthChangeList.Rows[i].Cells["ColLengthChangeListSelected"] as DataGridViewCheckBoxCell;

                // 編集中のセルを除くかつ、チェックが有効な場合
                if ((cellEditing.RowIndex != cellCheck.RowIndex) && (bool)cellCheck.Value)
                {
                    var cellEditLength = this.DgvEditLengthChangeList.Rows[i].Cells["ColLengthChangeListAfterLength"] as DataGridViewTextBoxCell;
                    var cellEditingLength = cellEditing as DataGridViewTextBoxCell;

                    // 編集中のセルの「変更後長さ」の値をチェックが有効な同列の値に設定
                    cellEditLength.Value = cellEditingLength.Value;
                }
            }
        }

        private void DgvEditLengthChangeList_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            var editLengthChangeList = sender as DataGridView;

            //コミットされていない内容がある
            if (editLengthChangeList.IsCurrentCellDirty)
            {
                editLengthChangeList.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        /// <summary>
        /// フィルタ検索
        /// </summary>
        public void Search()
        {
            try
            {
                // 長さ変更データグリッドに値をセット
                var rowDataList = CreateRowDataList(_targetElementUniqueIdList);

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

                // 長さ変更データグリッドを初期化
                this.DgvEditLengthChangeList.Rows.Clear();

                foreach (var rowData in filterList)
                {
                    this.DgvEditLengthChangeList.Rows.Add(new object[] { rowData.Id, rowData.ElementId, false, rowData.KoudaiName, rowData.DaiBunrui, rowData.ChuuBunrui, rowData.ShouBunrui, rowData.FamilyName, rowData.Length, });
                }
            }
            catch (Exception ex)
            {
                MessageUtil.Error("長さ変更リストのフィルター処理に失敗しました。", FORM_NAME, this);
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

        private void BtmCancel_Click(object sender, EventArgs e)
        {
            this.Close();
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
