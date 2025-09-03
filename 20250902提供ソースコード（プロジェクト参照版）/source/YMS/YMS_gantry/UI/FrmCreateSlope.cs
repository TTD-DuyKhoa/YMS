//using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YMS_gantry.Data;
using Form = System.Windows.Forms.Form;

namespace YMS_gantry.UI
{
    public partial class FrmCreateSlope : Form
    {
        public ModelData ModelData { get; set; }

        internal FrmCreateSlopeViewModel ViewModel { get; } = new FrmCreateSlopeViewModel();

        //プレビュー用
        public SlopeDataForPreview previewData { get; set; }
        public FrmPreviewSlope preview { get; set; }

        private Document _doc { get; set; }

        public FrmCreateSlope(Document doc)
        {
            InitializeComponent();
            InitializeBindings();
            _doc = doc;
            preview = new FrmPreviewSlope();
            this.button3.Enabled = true;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!InputValueChck())
            {
                return;
            }
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// 入力値検証
        /// </summary>
        /// <returns></returns>
        private bool InputValueChck()
        {
            bool retB = true;

            try
            {
                List<string> errMsg = new List<string>();
                if(comboBox1.Text=="")
                {
                    errMsg.Add("構台を選択してください");
                }
                else
                {
                    //var koudai = GantryUtil.GetKoudaiData(_doc, comboBox1.Text);
                    //if (koudai != null || koudai.AllKoudaiFlatData != null)
                    //{
                    //    if (!koudai.AllKoudaiFlatData.ohbikiData.isHkou)
                    //    {
                    //        errMsg.Add("スロープ対象外の大引が配置されている為、作成できません。");
                    //    }
                    //}
                }

                int currentHeigh = 0;
                bool isFNoriire = true;
                SlopeType cType = SlopeType.none;

                for (int i=0;i<gridSlopeStyle.RowCount;i++)
                {
                    DataGridViewRow row = gridSlopeStyle.Rows[i];
                    string type = row.Cells[1].EditedFormattedValue.ToString();
                    double height = RevitUtil.ClsCommonUtils.ChangeStrToDbl(row.Cells[3].Value.ToString());
                    double percent = RevitUtil.ClsCommonUtils.ChangeStrToDbl(row.Cells[2].Value.ToString());
                    SlopeType sType = (type == "スロープ無")?SlopeType.none:(type == "スロープ")?SlopeType.slope:SlopeType.noriire;

                    if (sType ==SlopeType.none)
                    {
                        height = RevitUtil.ClsCommonUtils.ChangeStrToDbl(row.Cells[3].Value.ToString());
                        if (isFNoriire) { isFNoriire = false; }
                        if(row.Index!=0&&cType==SlopeType.none&&currentHeigh!=height)
                        {
                            errMsg.Add("隣り合うフラット部分で高さが異なります");
                            break;
                        }
                        currentHeigh = (int)height;
                    }
                    else if (sType == SlopeType.slope)
                    {
                        if (row.Index == 0 || row.Index == gridSlopeStyle.RowCount - 1||isFNoriire)
                        {
                            errMsg.Add("乗り入れ部の隣または端部にスロープは設置できません");
                            break;
                        }
                        else if(!isFNoriire)
                        {
                            for (int j = i; j < gridSlopeStyle.RowCount - 1; j++)
                            {
                                string btype = row.Cells[1].EditedFormattedValue.ToString();
                                SlopeType bsType = (btype == "スロープ無") ? SlopeType.none : (btype == "スロープ") ? SlopeType.slope : SlopeType.noriire;
                                if (bsType == SlopeType.noriire)
                                {
                                    errMsg.Add("乗り入れ部の隣または端部にスロープは設置できません");
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        if(!isFNoriire)
                        {
                            for(int j=i;j<gridSlopeStyle.RowCount-1;j++)
                            {
                                string btype = row.Cells[1].EditedFormattedValue.ToString();
                                SlopeType bsType = (btype == "スロープ無") ? SlopeType.none : (btype == "スロープ") ? SlopeType.slope : SlopeType.noriire;
                                if(bsType!=SlopeType.noriire)
                                {
                                    errMsg.Add("構台の途中に乗り入れ部は指定できません");
                                    break;
                                }
                            }
                        }

                        if((row.Index==0&&percent==0)||(row.Index==gridSlopeStyle.RowCount-1&&percent==0))
                        {
                            errMsg.Add("乗り入れ部端部の勾配が0です");
                            break;
                        }
                    }
                    cType = sType;
                }

                if (errMsg.Count>0)
                {
                    FrmErrorInformation frm = new FrmErrorInformation(errMsg, this);
                    retB = false;
                }else
                {
                    retB = true;
                }

            }
            catch (Exception ex)
            {
                retB = false;
            }
            return retB;
        }


        private void FrnCreateSlope_Load(object sender, EventArgs e)
        {
            if (ModelData != null)
            {
                var data = ModelData;

                foreach (var x in data.ListKoudaiData)
                {
                    //x.SlopeData = x.SlopeData ?? FrnCreateSlopeViewModel.InitWith(x.AllKoudaiFlatData);
                    ViewModel.KodaiSet.Add(x.SlopeData);
                }
                var first = ViewModel.KodaiSet.FirstOrDefault();
                ViewModel.SelectedKodai = first;


                //var aaa = new string[]
                //{
                //    "aaa", "bbb", "ccc"
                //};
                //foreach (var x in aaa)
                //{
                //    var item = new FrnCreateSlopeViewModel.KodaiSlopeModel();
                //    item.Name = x;
                //    ViewModel.KodaiSet.Add(item);
                //}
                //for (int i = 0; i < 5; i++)
                //{
                //    var item = new FrnCreateSlopeViewModel.StyleRow
                //    {
                //        RowHeader = $"row{i}",
                //        SlopeType = FrnCreateSlopeViewModel.SlopeStyle.Nil,
                //        Percent = 0.0,
                //        Level = 0.0,
                //    };
                //    ViewModel.StyleRowSet.Add(item);

                //    ViewModel.SupportRowSet.Add(new FrnCreateSlopeViewModel.SupportRow
                //    {
                //        RowHeader = "aaa",
                //        HasTensetsuban = i % 2 == 0,
                //        Stiffener = FrnCreateSlopeViewModel.StiffenerType.Plate,
                //        Madumezai = FrnCreateSlopeViewModel.MadumezaiType.L75x6
                //    });
                //}
                //comboBox1.DataSource = data.ListKoudaiData.Select(x => x.AllKoudaiFlatData).ToArray();
                //comboBox1.DisplayMember = nameof(AllKoudaiFlatFrmData.KoudaiName);
            }


            ////入れ替えたほうが直感的な操作でできる気がする
            //int SpanC = 6;
            //dgvSlopeStyle.Rows.Add(SpanC);
            //dgvSupportParts.Rows.Add(SpanC + 1);

            //for (int i = 0; i < SpanC; i++)
            //{
            //    dgvSlopeStyle.Rows[i].HeaderCell.Value = $"スパン{i + 1}";
            //}

            //for (int i = 0; i < SpanC + 1; i++)
            //{
            //    if (i == 0)
            //    {
            //        dgvSupportParts.Rows[i].HeaderCell.Value = $"スパン1始点側";
            //    }
            //    else if (i == SpanC)
            //    {
            //        dgvSupportParts.Rows[i].HeaderCell.Value = $"スパン{i}終点側";
            //    }
            //    else
            //    {
            //        dgvSupportParts.Rows[i].HeaderCell.Value = $"スパン{i}～{i + 1}間";
            //    }
            //}

            //((DataGridViewComboBoxColumn)dgvSlopeStyle.Columns["cmbSlopeStyle"]).Items.AddRange("スロープ無", "乗り入れ", "スロープ");
            //((DataGridViewComboBoxColumn)dgvSlopeStyle.Columns["cmbSlopeStyle"]).DefaultCellStyle.NullValue = "スロープ無";

            //((DataGridViewComboBoxColumn)dgvSupportParts.Columns["cmbStiUmu"]).Items.AddRange("スチフナー無", "プレート", "ジャッキ");
            //((DataGridViewComboBoxColumn)dgvSupportParts.Columns["cmbStiUmu"]).DefaultCellStyle.NullValue = "スチフナー無";
            //((DataGridViewComboBoxColumn)dgvSupportParts.Columns["cmbFit"]).Items.AddRange("間詰材無", "L75x6", "L75x9");
            //((DataGridViewComboBoxColumn)dgvSupportParts.Columns["cmbFit"]).DefaultCellStyle.NullValue = "間詰材無";

        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            //TODO アクティビティ内容修正
            //DataGridView dg = (DataGridView)sender;
            //if (dg.CurrentCell == null) return;

            //DataGridViewCell cell = dg.CurrentCell;
            //if (dg.Columns[dg.CurrentCell.ColumnIndex].Name == "cmbSlopeStyle")
            //{
            //    if (cell.Value == null || cell.Value.ToString() == "スロープ無")
            //    {
            //        foreach (DataGridViewCell cells in dg.CurrentRow.Cells)
            //        {
            //            if (cells.ColumnIndex == cell.ColumnIndex) { continue; }
            //            cells.Style.BackColor = SystemColors.ControlDark;
            //            cells.Style.ForeColor = SystemColors.GrayText;
            //            cells.ReadOnly = true;
            //        }
            //        return;
            //    }
            //    else
            //    {
            //        foreach (DataGridViewCell cells in dg.CurrentRow.Cells)
            //        {
            //            if (cells.ColumnIndex == cell.ColumnIndex) { continue; }
            //            cells.Style.BackColor = SystemColors.Window;
            //            cells.Style.ForeColor = SystemColors.ControlText;
            //            cell.ReadOnly = false;
            //        }
            //    }

            //    if (cell.Value.ToString() == "乗り入れ部")
            //    {
            //        dg.Rows[cell.RowIndex].Cells[cell.ColumnIndex + 1].ReadOnly = false;
            //        dg.Rows[cell.RowIndex].Cells[cell.ColumnIndex + 1].Style.BackColor = SystemColors.Window;
            //        dg.Rows[cell.RowIndex].Cells[cell.ColumnIndex + 1].Style.ForeColor = SystemColors.ControlText;

            //        dg.Rows[cell.RowIndex].Cells[cell.ColumnIndex + 2].ReadOnly = true;
            //        dg.Rows[cell.RowIndex].Cells[cell.ColumnIndex + 2].Style.BackColor = SystemColors.ControlDark;
            //        dg.Rows[cell.RowIndex].Cells[cell.ColumnIndex + 2].Style.ForeColor = SystemColors.GrayText;
            //    }
            //    else if (cell.Value.ToString() == "フラット部")
            //    {
            //        dg.Rows[cell.RowIndex].Cells[cell.ColumnIndex + 2].ReadOnly = false;
            //        dg.Rows[cell.RowIndex].Cells[cell.ColumnIndex + 2].Style.BackColor = SystemColors.Window;
            //        dg.Rows[cell.RowIndex].Cells[cell.ColumnIndex + 2].Style.ForeColor = SystemColors.ControlText;

            //        dg.Rows[cell.RowIndex].Cells[cell.ColumnIndex + 1].ReadOnly = true;
            //        dg.Rows[cell.RowIndex].Cells[cell.ColumnIndex + 1].Style.BackColor = SystemColors.ControlDark;
            //        dg.Rows[cell.RowIndex].Cells[cell.ColumnIndex + 1].Style.ForeColor = SystemColors.GrayText;
            //    }

            //    dg.Refresh();
            //}
        }

        private void InitializeBindings()
        {
            var vm = ViewModel;
            comboBox1.DataBindings.Add(nameof(System.Windows.Forms.ComboBox.DataSource), vm, nameof(FrmCreateSlopeViewModel.KodaiSet), true, DataSourceUpdateMode.OnPropertyChanged);
            comboBox1.DisplayMember = nameof(FrmCreateSlopeViewModel.KodaiSlopeModel.Name);
            comboBox1.DataBindings.Add(nameof(System.Windows.Forms.ComboBox.SelectedValue), vm, nameof(FrmCreateSlopeViewModel.SelectedKodai), true, DataSourceUpdateMode.OnPropertyChanged);

            colStyleHeader.DataPropertyName = nameof(FrmCreateSlopeViewModel.StyleRow.RowHeader);
            colSlopeStyle.DataPropertyName = nameof(FrmCreateSlopeViewModel.StyleRow.SlopeType);
            colSlopePercent.DataPropertyName = nameof(FrmCreateSlopeViewModel.StyleRow.Percent);
            colSlopeLevel.DataPropertyName = nameof(FrmCreateSlopeViewModel.StyleRow.Level);

            gridSlopeStyle.DataBindings.Add(nameof(DataGridView.DataSource), vm, nameof(FrmCreateSlopeViewModel.StyleRowSet), true, DataSourceUpdateMode.OnPropertyChanged);
            gridSlopeStyle.AutoGenerateColumns = false;

            colSupportHeader.DataPropertyName = nameof(FrmCreateSlopeViewModel.StyleRow.RowHeader);
            colTensetsuban.DataPropertyName = nameof(FrmCreateSlopeViewModel.SupportRow.HasTensetsuban);
            colMadumezai.DataPropertyName = nameof(FrmCreateSlopeViewModel.SupportRow.Madumezai);
            colStiffener.DataPropertyName = nameof(FrmCreateSlopeViewModel.SupportRow.Stiffener);

            //colMadumezai.DataBindings.Add(nameof(ComboBox.DataSource), vm, nameof(FrmCreateSlopeViewModel.KodaiSet), true, DataSourceUpdateMode.OnPropertyChanged);

            gridSupportParts.DataBindings.Add(nameof(DataGridView.DataSource), vm, nameof(FrmCreateSlopeViewModel.SupportRowSet), true, DataSourceUpdateMode.OnPropertyChanged);
            gridSupportParts.AutoGenerateColumns = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!InputValueChck())
            {
                return;
            }
            List<double> Height = GetPreviewData();
            if(Height==null)
            {
                return;
            }

            KoudaiData data = GantryUtil.GetKoudaiData(_doc, this.comboBox1.Text);
            if (data != null)
            {
                AllKoudaiFlatFrmData kData = data.AllKoudaiFlatData;
                if (preview.Visible)
                {
                    preview.DialogResult = DialogResult.None;
                    preview.Close();
                }
                preview = new FrmPreviewSlope(kData, Height, previewData);
                //モーダレスにして結果だけ取得する
                preview.FormClosed += new FormClosedEventHandler(Preview_Closed);
                preview.Show(this);
            }
        }

        private List<double> GetPreviewData()
        {
            previewData = new SlopeDataForPreview();

            List<double> Height = new List<double>();
            foreach (DataGridViewRow row in this.gridSlopeStyle.Rows)
            {
                string type = row.Cells[1].EditedFormattedValue.ToString();
                SlopeType sType = SlopeType.none;
                bool isEnd = false;
                double percent = 0;
                double height = 0;
                if (type == "スロープ無")
                {
                    height = RevitUtil.ClsCommonUtils.ChangeStrToDbl(row.Cells[3].Value.ToString());
                }
                else if (type == "スロープ")
                {
                    sType = SlopeType.slope;
                }
                else
                {
                    sType = SlopeType.noriire;
                    percent = RevitUtil.ClsCommonUtils.ChangeStrToDbl(row.Cells[2].Value.ToString());
                }
                if (row.Index == this.gridSlopeStyle.RowCount - 1)
                {
                    isEnd = true;
                }
                Height.Add(RevitUtil.ClsCommonUtils.ChangeStrToDbl(row.Cells[3].Value.ToString()));
                previewData.Data.Add((sType, isEnd, percent, height));
            }

            if (previewData.Data.All(x => x.type == SlopeType.noriire))
            {
                var taskDialog = new TaskDialog("スロープ向きの確認")
                {
                    TitleAutoPrefix = false,
                    MainIcon = TaskDialogIcon.TaskDialogIconInformation,
                    MainInstruction = "全スパン通しのスロープが入力されました",
                    MainContent = "スロープの向きを選択してください:",
                };
                taskDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, $"スパン1 ↘ スパン{this.gridSlopeStyle.RowCount}");
                taskDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, $"スパン1 ↙ スパン{this.gridSlopeStyle.RowCount}");
                var result = taskDialog.Show();
                if (result == TaskDialogResult.CommandLink1)
                {
                    previewData.allNoriireFromOrign = false;
                }
                else if (result == TaskDialogResult.CommandLink2)
                {
                    previewData.allNoriireFromOrign = true;
                }
                else
                {
                    return null;
                }
                previewData.allNoriire = true;
            }
            return Height;
        }

        /// <summary>
        /// プレビュー更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PrevireUpdata()
        {
            if (!InputValueChck())
            {
                return;
            }
            List<double> Height = GetPreviewData();
            if (Height == null)
            {
                return;
            }

            KoudaiData data = GantryUtil.GetKoudaiData(_doc, this.comboBox1.Text);
            if (data != null)
            {
                AllKoudaiFlatFrmData kData = data.AllKoudaiFlatData;
                if (preview.Visible)
                {
                    preview.DialogResult = DialogResult.None;
                    preview.Close();
                }

                if (preview == null)
                {
                    preview = new FrmPreviewSlope(kData, Height, previewData);
                }
                else
                {
                    preview.UpdatePreview(kData, Height, previewData);
                }

                preview = new FrmPreviewSlope(kData, Height, previewData);
                //モーダレスにして結果だけ取得する
                preview.FormClosed += new FormClosedEventHandler(Preview_Closed);
                preview.Show(this);
            }
        }


        /// <summary>
        /// 閉じられた結果取得
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Preview_Closed(object sender, FormClosedEventArgs e)
        {
            FrmPreviewSlope f = (FrmPreviewSlope)sender;
            //結果を表示する
            if (f.DialogResult == DialogResult.OK)
            {
                this.btnOk.PerformClick();
            }
        }

        private void gridSlopeStyle_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }

    public enum SlopeType
    {
        none,
        noriire,
        slope
    }

    public class SlopeDataForPreview
    {
        public List<(SlopeType type, bool isEnd, double percent, double height)> Data { get; set; }

        public bool allNoriire { get; set; }
        public bool allNoriireFromOrign { get; set; }

        public SlopeDataForPreview()
        {
            Data = new List<(SlopeType type, bool isEnd, double percent, double height)>();
            allNoriire = false;
            allNoriireFromOrign = false;
        }
    }
}
