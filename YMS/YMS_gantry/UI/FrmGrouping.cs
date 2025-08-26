using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
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
using YMS_gantry.Data;

namespace YMS_gantry.UI
{
    public partial class FrmGrouping : System.Windows.Forms.Form
    {
        private RequestHandler m_Handler;
        private ExternalEvent m_ExEvent;

        private UIApplication _uiApp { get; set; }
        private UIDocument _uiDoc { get; set; }
        private Document _doc { get; set; }
        private ModelData _md { get; set; }
        private List<string> _groupNameList { get; set; }
        public GroupingId groupingId { get; set; }
        public string selectKoudaiName { get; set; }
        public ElementId selectBuzaiId { get; set; }
        public GroupData selectGroupData { get; set; }

        public enum GroupingId : int
        {
            HunchSelect,
            GroupView,
            Delete,
            KoudaiView,
            BuzaiView,
            OK,
            Cancel
        }

        #region コンストラクタ
        public FrmGrouping(UIApplication uiApp)
        {
            InitializeComponent();
            _uiApp = uiApp;
            _uiDoc = uiApp.ActiveUIDocument;
            _doc = uiApp.ActiveUIDocument.Document;
            _md = ModelData.GetModelData(_doc);
        }
        public FrmGrouping(ExternalEvent exEvent, RequestHandler handler, UIApplication uiApp)
        {
            InitializeComponent();
            _uiApp = uiApp;
            _uiDoc = uiApp.ActiveUIDocument;
            _doc = uiApp.ActiveUIDocument.Document;
            _md = ModelData.GetModelData(_doc);

            m_Handler = handler;
            m_ExEvent = exEvent;
        }
        #endregion

        #region イベント
        /// <summary>
        /// ロード処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmGrouping_Load(object sender, EventArgs e)
        {
            SetDataGridViewGroup();
            SetDataGridViewKoudai();
            SetDataGridViewBuzai();
            UpdateGroupNameList();
        }

        /// <summary>
        /// 追加ボタン押下処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtGroupName.Text))
            {
                MessageUtil.Information("グループ名を入力して下さい。", "グルーピング", this);
                return;
            }

            if (_groupNameList.Contains(txtGroupName.Text))
            {
                MessageUtil.Information("既に同一名称のグループが存在しています。", "グルーピング", this);
                return;
            }

            var row = new DataGridViewRow();
            row.CreateCells(this.dgvGroup);
            row.Cells[1].Value = this.txtGroupName.Text;
            row.Cells[2].Value = 0;
            row.Cells[0].Value = true;

            GroupData gd = new GroupData();
            gd.GroupName = this.txtGroupName.Text;
            row.Tag = gd;

            this.dgvGroup.Rows.Add(row);
            UpdateGroupNameList();
        }
        #endregion

        #region メソッド
        /// <summary>
        /// グループグリッドをメンバ変数で更新
        /// </summary>
        public void SetDataGridViewGroup()
        {
            this.dgvGroup.Rows.Clear();

            ModelData md = _md;

            List<GroupData> lstGd = md.ListGroupData;

            for (int i = 0; i < lstGd.Count; i++)
            {
                GroupData gd = lstGd[i];
                var row = new DataGridViewRow();
                row.CreateCells(this.dgvGroup);
                row.Cells[0].Value = gd.FlgLineView;
                row.Cells[1].Value = gd.GroupName;
                row.Cells[2].Value = gd.ListHunchId.Count.ToString();

                row.Tag = gd;

                this.dgvGroup.Rows.Add(row);
            }
        }

        /// <summary>
        /// 構台グリッドをメンバ変数で更新
        /// </summary>
        private void SetDataGridViewKoudai()
        {
            ModelData md = _md;

            List<KoudaiData> lstKd = md.ListKoudaiData;

            for (int i = 0; i < lstKd.Count; i++)
            {
                KoudaiData kd = lstKd[i];
                var row = new DataGridViewRow();
                row.CreateCells(this.dgvKoudai);
                row.Cells[0].Value = i + 1;
                row.Cells[1].Value = kd.AllKoudaiFlatData.KoudaiType;
                row.Cells[2].Value = kd.AllKoudaiFlatData.KoudaiName;
                UpdateGroupNameList();//コンボボックスの内容を更新
                row.Cells[3].Value = kd.GroupName;

                this.dgvKoudai.Rows.Add(row);
            }
        }


        /// <summary>
        /// 部材グリッドをメンバ変数で更新
        /// </summary>
        public void SetDataGridViewBuzai()
        {
            this.dgvBuzai.Rows.Clear();

            //未割当の部材を取得
            List <ElementId> lstId = MaterialSuper.CollectOtherMaterial(_doc);
            for (int i = 0; i < lstId.Count; i++)
            {
                ElementId elementId = lstId[i];

                //構台名
                //var kodaiName = lstMs[i].m_KodaiName;
                // ファミリ名取得
                var familyName = GetFamilyName(_uiDoc.Document.GetElement(elementId));
                // タイプ名取得
                var typeName = GetTypeName(_uiDoc.Document.GetElement(elementId));
                // ファミリシンボル取得
                var familySymbol = ClsRevitUtil.GetFamilySymbol(_uiDoc.Document, familyName, typeName);

                var daibunrui = GetTypeParameter(familySymbol, "大分類");
                var chuubunrui = GetTypeParameter(familySymbol, "中分類");
                var shoubunrui = GetTypeParameter(familySymbol, "小分類");

                // ファミリインスタンス取得
                var familyInstance = GantryUtil.GetFamilyInstance(_uiDoc.Document, elementId);
                if (familyInstance == null) continue;

                // ネストされたファミリである場合はレコードの追加をスキップ
                if (familyInstance.SuperComponent != null) continue;

                var row = new DataGridViewRow();
                row.CreateCells(this.dgvBuzai);
                row.Cells[0].Value = daibunrui;
                row.Cells[1].Value = chuubunrui;
                row.Cells[2].Value = shoubunrui;
                row.Cells[3].Value = familyName;
                //row.Cells[4].Value = kodaiName;
                row.Tag = elementId;

                this.dgvBuzai.Rows.Add(row);
            }
        }

        
        /// <summary>
        /// 編集中のモデルデータを取得
        /// </summary>
        public ModelData GetEditModelData()
        {
            return _md;
        }

        /// <summary>
        /// 編集中のモデルデータを取得
        /// </summary>
        public  void SetEditModelData(ModelData md)
        {
            _md = md;
            return;
        }


        /// <summary>
        /// グループ名リストを更新
        /// </summary>
        private void UpdateGroupNameList()
        {
            List<string> lstGN = new List<string>();
            foreach(DataGridViewRow row in dgvGroup.Rows)
            {
                lstGN.Add(row.Cells[1].Value.ToString());
            }

            _groupNameList = lstGN;
            UpdateCellDropList();
        }

        /// <summary>
        /// グループのリストを更新
        /// </summary>
        private void UpdateCellDropList()
        {
            foreach (DataGridViewRow row in dgvKoudai.Rows)
            {
                string bk = Convert.ToString(row.Cells[3].Value);
                DataGridViewComboBoxCell cell = ((DataGridViewComboBoxCell)row.Cells[3]);

                cell.Items.Clear();
                cell.Items.Add(string.Empty);
                foreach (string groupName in _groupNameList)
                {
                    cell.Items.Add(groupName);
                }
                if(cell.Items.Contains(bk))
                {
                    cell.Value = bk;
                }
                else
                {
                    cell.Value = string.Empty;
                }
            }

            List<string> lstKoudai = (from data in _md.ListKoudaiData select data.AllKoudaiFlatData.KoudaiName).ToList();
            foreach (DataGridViewRow row in dgvBuzai.Rows)
            {
                string bk = Convert.ToString(row.Cells[4].Value);
                DataGridViewComboBoxCell cell = ((DataGridViewComboBoxCell)row.Cells[4]);

                cell.Items.Clear();
                cell.Items.Add(string.Empty);
                foreach (string koudaiName in lstKoudai)
                {
                    cell.Items.Add(koudaiName);
                }
                if (cell.Items.Contains(bk))
                {
                    cell.Value = bk;
                }
                else
                {
                    cell.Value = string.Empty;
                }
            }
        }

        /// <summary>
        /// ハンチを追加
        /// </summary>
        /// <param name="lstAddId"></param>
        public void AddHunchId(List<ElementId> lstAddId)
        {
            List<string> lstGN = new List<string>();

            GroupData gd = (GroupData)dgvGroup.SelectedRows[0].Tag;
            List<ElementId> lstId = gd.GetListHunchElementId();

            if (lstId == null)
                lstId = new List<ElementId>();

            foreach(ElementId addId in lstAddId)
            {
                if(!(from data in lstId where data.ToString() == addId.ToString() select data).Any())
                {
                    lstId.Add(addId);
                }
            }
            gd.SetListHunchElementId(lstId);
            dgvGroup.SelectedRows[0].Tag = gd;
            dgvGroup.SelectedRows[0].Cells[2].Value = lstId.Count;
        }


        private string GetFamilyName(Element e)
        {
            var eId = e?.GetTypeId();
            if (eId == null)
                return "";
            var elementType = e.Document.GetElement(eId) as ElementType;
            return elementType?.FamilyName ?? "";
        }

        private string GetTypeName(Element e)
        {
            var eId = e?.GetTypeId();
            if (eId == null)
                return "";
            var elementType = e.Document.GetElement(eId) as ElementType;
            return elementType?.Name ?? "";
        }

        private string GetTypeParameter(FamilySymbol sym, string paramName)
        {
            try
            {
                Parameter parm = sym.LookupParameter(paramName);
                if (parm == null)
                {
                    return string.Empty;
                }
                return parm.AsValueString();
            }
            catch
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// メンバ変数のモデルデータを更新
        /// </summary>
        private void UpdateModelData()
        {
            //モデルデータ更新
            List<KoudaiData> lstKd = _md.ListKoudaiData;
            foreach (DataGridViewRow row in dgvKoudai.Rows)
            {
                foreach (KoudaiData kd in lstKd)
                {
                    if (kd.AllKoudaiFlatData.KoudaiName == row.Cells[2].Value.ToString())
                    {
                        kd.GroupName = Convert.ToString(row.Cells[3].Value);
                    }
                }
            }
            foreach (DataGridViewRow row in dgvBuzai.Rows)
            {
                foreach (KoudaiData kd in lstKd)
                {
                    if (kd.AllKoudaiFlatData.KoudaiName == Convert.ToString( row.Cells[4].Value))
                    {
                        kd.ListWariateElementId.Add(((ElementId)row.Tag).IntegerValue);
                    }
                }
            }
            _md.ListKoudaiData = lstKd;

            List<GroupData> lstGd = new List<GroupData>();
            foreach (DataGridViewRow row in dgvGroup.Rows)
            {
                GroupData gd = (GroupData)row.Tag;

                string groupname = row.Cells[1].Value.ToString();
                List<string> lstKoudaiName = new List<string>();
                if ((from data in lstKd where data.GroupName == groupname select data.AllKoudaiFlatData.KoudaiName).Any())
                {
                    lstKoudaiName = (from data in lstKd where data.GroupName == groupname select data.AllKoudaiFlatData.KoudaiName).ToList();
                }
                gd.GroupName = groupname;
                gd.ListKoudaiName = lstKoudaiName;
                //gd.ListHunchId = ConvIdListToIntList((List<ElementId>)row.Tag);
                gd.FlgLineView = (bool)row.Cells[0].Value;
                lstGd.Add(gd);
            }
            _md.ListGroupData = lstGd;
        }

        private List<int> ConvIdListToIntList(List<ElementId> lstId)
        {
            List<int> lstInt = new List<int>();
            if (lstId == null) return lstInt;

            foreach(ElementId id in lstId)
            {
                lstInt.Add(id.IntegerValue);
            }
            return lstInt;
        }
        #endregion
        /// <summary>
        /// Groupグリッドボタンセル押下処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var dgv = (DataGridView)sender;

            int clmnIndex = e.ColumnIndex;
            int rowIndex = e.RowIndex;
            switch (clmnIndex)
            {
                case 3: //ハンチ選択
                    {
                        groupingId = GroupingId.HunchSelect;
                        MakeRequest(RequestId.Grouping);
                        break;
                    }
                case 4: //表示
                    {
                        UpdateModelData();
                        selectGroupData = _md.GetGroupData(dgv.Rows[rowIndex].Cells[1].Value.ToString());
                        groupingId = GroupingId.GroupView;
                        MakeRequest(RequestId.Grouping);
                        break;
                    }
                case 5: //削除
                    {
                        selectGroupData = _md.GetGroupData(dgv.Rows[rowIndex].Cells[1].Value.ToString());
                        dgvGroup.Rows.Remove(dgvGroup.Rows[rowIndex]);
                        UpdateGroupNameList();
                        groupingId = GroupingId.Delete;
                        MakeRequest(RequestId.Grouping);
                        break;
                    }
            }
        }

        /// <summary>
        /// 構台グリッドボタンセル押下処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvKoudai_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var dgv = (DataGridView)sender;

            int clmnIndex = e.ColumnIndex;
            int rowIndex = e.RowIndex;
            switch (clmnIndex)
            {
                case 4: //表示
                    {
                        UpdateModelData();
                        selectKoudaiName = dgv.Rows[rowIndex].Cells[2].Value.ToString();
                        groupingId = GroupingId.KoudaiView;
                        MakeRequest(RequestId.Grouping);
                        break;
                    }
            }

        }
        /// <summary>
        /// Groupグリッドボタンセル押下処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvBuzai_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var dgv = (DataGridView)sender;

            int clmnIndex = e.ColumnIndex;
            int rowIndex = e.RowIndex;
            switch (clmnIndex)
            {
                case 5: //表示
                    {
                        UpdateModelData();
                        selectBuzaiId = (ElementId)dgv.Rows[rowIndex].Tag;
                        groupingId = GroupingId.BuzaiView;
                        MakeRequest(RequestId.Grouping);
                        break;
                    }
            }
        }

        /// <summary>
        /// キャンセルボタン押下処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            //初期化処理
            this.DialogResult = DialogResult.Cancel;
            groupingId = GroupingId.Cancel;
            MakeRequest(RequestId.Grouping);
            m_ExEvent.Dispose();
            m_ExEvent = null;
            m_Handler = null;

            this.Close();
        }

        /// <summary>
        /// OKボタン押下処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            UpdateModelData();

            //ModelData.SetModelData(_doc, _md);

            groupingId = GroupingId.OK;
            MakeRequest(RequestId.Grouping);
            this.DialogResult = DialogResult.OK;
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
