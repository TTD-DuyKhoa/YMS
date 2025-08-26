using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using YMS.gantry;

namespace YMS_gantry.UI
{
    public partial class FrmSonotaKanshouCheck : System.Windows.Forms.Form
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private const string FORM_NAME = "干渉チェック";

        private RequestHandler m_Handler;
        private ExternalEvent m_ExEvent;

        private UIDocument _uiDoc;
        private List<FamilySymbol> _familySymbolList;
        private List<ElementId> _resultList;
        public List<ElementId> _rowCheckedElementIdList;

        public FrmSonotaKanshouCheck(ExternalEvent exEvent, RequestHandler handler, UIApplication uiApp)
        {
            InitializeComponent();

            _uiDoc = uiApp.ActiveUIDocument;
            _familySymbolList = new List<FamilySymbol>();
            _resultList = new List<ElementId>();

            m_Handler = handler;
            m_ExEvent = exEvent;
        }

        private void FrmSonotaKanshouCheck_Load(object sender, EventArgs e)
        {
            try
            {
                // Revit上でのファミリ選択状態を解除
                _uiDoc.Selection.SetElementIds(new List<ElementId>());

                var doc = _uiDoc.Document;

                // 図面内に配置された全ての構台名を取得
                var koudaiNameList = GantryUtil.GetAllKoudaiName(doc);

                var bunruiList = new List<Bunrui>();
                foreach (var koudaiName in koudaiNameList)
                {
                    // 図面内に配置されたファミリシンボルを取得
                    _familySymbolList = GantryUtil.GetAllFamilySymbol(doc);
                    foreach (var fs in _familySymbolList)
                    {
                        var daiBunrui = GantryUtil.GetTypeParameter(fs, DefineUtil.PARAM_DAI_BUNRUI);
                        var chuuBunrui = GantryUtil.GetTypeParameter(fs, DefineUtil.PARAM_CHUU_BUNRUI);
                        var shouBunrui = GantryUtil.GetTypeParameter(fs, DefineUtil.PARAM_SHOU_BUNRUI);

                        var exists = bunruiList.Exists(x => x.Dai == daiBunrui && x.Chuu == chuuBunrui && x.Shou == shouBunrui);
                        if (!exists)
                        {
                            // 全ての分類が空の対象は抽出しない
                            if (string.IsNullOrWhiteSpace(daiBunrui) && string.IsNullOrWhiteSpace(chuuBunrui) && string.IsNullOrWhiteSpace(shouBunrui)) continue;

                            // 図面内に配置されたファミリの分類情報を抽出
                            bunruiList.Add(new Bunrui { Dai = daiBunrui, Chuu = chuuBunrui, Shou = shouBunrui });
                        }
                    }
                }

                // 空の選択肢を追加
                this.CmbDaiBunrui.Items.Add(string.Empty);
                this.CmbChuuBunrui.Items.Add(string.Empty);
                this.CmbShouBunrui.Items.Add(string.Empty);

                foreach (var bunrui in bunruiList)
                {
                    // 分類データグリッドに値をセット
                    this.DgvBunruiList.Rows.Add(new object[] { bunrui.Dai, bunrui.Chuu, bunrui.Shou });

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

                foreach (var koudaiName in koudaiNameList)
                {
                    // 構台名リストに値をセット
                    this.LstKoudai.Items.Add(koudaiName);
                    var index = this.LstKoudai.Items.IndexOf(koudaiName);
                }
            }
            catch (Exception ex)
            {
                MessageUtil.Error("画面のロード処理に失敗しました。", FORM_NAME, this);
                logger.Error(ex, ex.Message);
            }
        }

        private void 選択ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in this.DgvKanshouCheckList.SelectedRows)
            {
                row.Cells["ColKenshouCheckListSelected"].Value = true;
            }
        }

        private void 解除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in this.DgvKanshouCheckList.SelectedRows)
            {
                row.Cells["ColKenshouCheckListSelected"].Value = false;
            }
        }

        private void BtnKanshouCheck_Click(object sender, EventArgs e)
        {
            try
            {
                // 干渉ファミリデータグリッドを初期化
                this.DgvKanshouCheckList.Rows.Clear();

                // Revit上でのファミリ選択状態を解除
                _uiDoc.Selection.SetElementIds(new List<ElementId>());

                var doc = _uiDoc.Document;

                // 干渉チェックの対象とする分類を取得
                var bunruiList = new List<Bunrui>();
                foreach (DataGridViewRow row in this.DgvBunruiList.SelectedRows)
                {
                    var daiBunrui = row.Cells["ColDaiBunrui"].Value.ToString();
                    var chuuBunrui = row.Cells["ColChuuBunrui"].Value.ToString();
                    var shouBunrui = row.Cells["ColShouBunrui"].Value.ToString();

                    bunruiList.Add(new Bunrui { Dai = daiBunrui, Chuu = chuuBunrui, Shou = shouBunrui });
                }

                // 干渉チェックの対象とするファミリシンボルを取得
                var targetList = new List<FamilySymbol>();
                foreach (var fs in _familySymbolList)
                {
                    var daiBunrui = GantryUtil.GetTypeParameter(fs, DefineUtil.PARAM_DAI_BUNRUI);
                    var chuuBunrui = GantryUtil.GetTypeParameter(fs, DefineUtil.PARAM_CHUU_BUNRUI);
                    var shouBunrui = GantryUtil.GetTypeParameter(fs, DefineUtil.PARAM_SHOU_BUNRUI);

                    var exists = bunruiList.Exists(x => x.Dai == daiBunrui && x.Chuu == chuuBunrui && x.Shou == shouBunrui);
                    if (exists)
                    {
                        targetList.Add(fs);
                    }
                }

                // 干渉データを取得
                var resultList = new List<ElementId>();
                foreach (var fs in targetList)
                {
                    var kanshouElementIds = new List<ElementId>();
                    var fiList = GantryUtil.GetFamilyInstanceList(doc, fs);

                    foreach (var fi in fiList)
                    {
                        var resultIds = new List<ElementId>();
                        ClsRevitUtil.CheckCollision(doc, fi.Id, ref resultIds);

                        foreach (var resultId in resultIds)
                        {
                            var exists = kanshouElementIds.Exists(x => x.IntegerValue == resultId.IntegerValue);
                            if (!exists)
                            {
                                kanshouElementIds.Add(resultId);
                            }
                        }
                    }

                    foreach (var elementId in kanshouElementIds)
                    {
                        // エレメント取得
                        var element = _uiDoc.Document.GetElement(elementId);

                        var exists = resultList.Exists(x => x.IntegerValue == elementId.IntegerValue);
                        if (!exists)
                        {
                            resultList.Add(elementId);
                        }
                    }
                }

                // 干渉ファミリデータグリッドに値をセット
                var rowDataList = CreateRowDataList(doc, resultList);
                foreach (var rowData in rowDataList)
                {
                    this.DgvKanshouCheckList.Rows.Add(new object[] { rowData.Id, false, rowData.KoudaiName, rowData.DaiBunrui, rowData.ChuuBunrui, rowData.ShouBunrui, rowData.FamilyName });
                }

                _resultList = resultList;

                MessageUtil.Information("干渉チェックが完了しました。", FORM_NAME, this);
            }
            catch (Exception ex)
            {
                MessageUtil.Error("干渉チェック処理に失敗しました。", FORM_NAME, this);
                logger.Error(ex, ex.Message);
            }
        }

        private List<RowData> CreateRowDataList(Document doc,  List<ElementId> resultList)
        {
            var rowDataList = new List<RowData>();

            foreach (var elementId in resultList)
            {
                // ファミリ名取得
                var familyName = GantryUtil.GetFamilyName(_uiDoc.Document.GetElement(elementId));
                // タイプ名取得
                var typeName = GantryUtil.GetTypeName(_uiDoc.Document.GetElement(elementId));
                // ファミリインスタンス取得
                var familyInstance = GantryUtil.GetFamilyInstance(_uiDoc.Document, elementId);
                // ファミリシンボル取得
                var familySymbol = GantryUtil.GetFamilySymbol(_uiDoc.Document, typeName, familyName);

                if (familyInstance == null || familySymbol == null) continue;

                var koudaiName = string.Empty;

                var material = MaterialSuper.ReadFromElement(familyInstance.Id, doc);
                if (material == null)
                {
                    // YMS構台の機能で配置したファミリではないが、共通パラメータに構台名だけ存在する場合を考慮
                    var isYmsParam = MaterialSuper.IsFromOtherElement(elementId, _uiDoc.Document);
                    if (!isYmsParam)
                    {
                        // 共通パラメータが空でない場合、その値を取得
                        var lineString = ClsRevitUtil.GetInstMojiParameter(_uiDoc.Document, elementId, DefineUtil.PARAM_MATERIAL_DETAIL);
                        var dictionary = GantryUtil.CreateDictionaryFromString(lineString);
                        const string KOUDAI_NAME = "構台名";
                        if (dictionary.ContainsKey(KOUDAI_NAME))
                        {
                            // 共通パラメータ内に構台名が存在する場合、値を取得
                            koudaiName = dictionary[KOUDAI_NAME];
                        }
                    }
                    
                }
                else
                {
                    koudaiName = material?.m_KodaiName;
                }

                var element = _uiDoc.Document.GetElement(elementId);

                var id = element.UniqueId;

                var daiBunrui = GantryUtil.GetTypeParameter(familySymbol, DefineUtil.PARAM_DAI_BUNRUI);
                var chuuBunrui = GantryUtil.GetTypeParameter(familySymbol, DefineUtil.PARAM_CHUU_BUNRUI);
                var shouBunrui = GantryUtil.GetTypeParameter(familySymbol, DefineUtil.PARAM_SHOU_BUNRUI);

                var rowData = new RowData { Id = id, KoudaiName = koudaiName, DaiBunrui = daiBunrui, ChuuBunrui = chuuBunrui, ShouBunrui = shouBunrui, FamilyName = familyName };

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
            public string KoudaiName { get; set; }
            public string DaiBunrui { get; set; }
            public string ChuuBunrui { get; set; }
            public string ShouBunrui { get; set; }
            public string FamilyName { get; set; }
        }

        private void DgvKanshouCheckList_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (this.DgvKanshouCheckList.CurrentCellAddress.X == 1 && this.DgvKanshouCheckList.IsCurrentCellDirty)
            {
                //コミットする
                this.DgvKanshouCheckList.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void DgvKanshouCheckList_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (m_Handler == null || m_ExEvent == null) return;

            var checkBoxColumnIndex = 1;

            //列のインデックスを確認する
            if (e.ColumnIndex == checkBoxColumnIndex)
            {
                var rowCheckedElementIdList = new List<ElementId>();
                foreach (DataGridViewRow row in this.DgvKanshouCheckList.Rows)
                {
                    var isChecked = (bool)row.Cells["ColKenshouCheckListSelected"].Value;
                    if (isChecked)
                    {
                        // 行のユニークIDを取得
                        var rowCheckedUniqueId = row.Cells["ColKenshouCheckListID"].Value.ToString();
                        // ID列よりElementを生成
                        var element = _uiDoc.Document.GetElement(rowCheckedUniqueId);

                        rowCheckedElementIdList.Add(element.Id);
                    }
                }

                _rowCheckedElementIdList = rowCheckedElementIdList;
            }

            _uiDoc.Selection.SetElementIds(_rowCheckedElementIdList);
        }

        private void FrmSonotaKanshouCheck_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Revit上でのファミリ選択状態を解除
            _uiDoc.Selection.SetElementIds(new List<ElementId>());

            //初期化処理
            MakeRequest(RequestId.End);
            m_ExEvent.Dispose();
            m_ExEvent = null;
            m_Handler = null;
        }

        /// <summary>
        /// フィルタ検索
        /// </summary>
        private void Search()
        {
            try
            {
                var doc = _uiDoc.Document;

                // 干渉ファミリデータグリッドに値をセット
                var rowDataList = CreateRowDataList(doc, _resultList);

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

                // 干渉ファミリデータグリッドを初期化
                this.DgvKanshouCheckList.Rows.Clear();

                foreach (var rowData in filterList)
                {
                    this.DgvKanshouCheckList.Rows.Add(new object[] { rowData.Id, false, rowData.KoudaiName, rowData.DaiBunrui, rowData.ChuuBunrui, rowData.ShouBunrui, rowData.FamilyName });
                }
            }
            catch (Exception ex)
            {
                MessageUtil.Error("干渉チェック結果のフィルター処理に失敗しました。", FORM_NAME, this);
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

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnSelectedAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.DgvKanshouCheckList.RowCount; i++)
            {
                this.DgvKanshouCheckList["ColKenshouCheckListSelected", i].Value = true;
            }
        }

        private void BtnUnSelectedAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.DgvKanshouCheckList.RowCount; i++)
            {
                this.DgvKanshouCheckList["ColKenshouCheckListSelected", i].Value = false;
            }
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
