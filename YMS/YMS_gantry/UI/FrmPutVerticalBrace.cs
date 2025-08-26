using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YMS.gantry;
using YMS_gantry.GantryUtils;
using static YMS_gantry.UI.FrmPutVerticalBrace.FrmPutVerticalBraceData;

namespace YMS_gantry.UI
{
    public partial class FrmPutVerticalBrace : System.Windows.Forms.Form
    {

        #region 定数
        /// <summary>
        /// Iniファイルのパス
        /// </summary>
        const string IniPath = "ini\\FrmPutVerticalBrace.ini";
        /// <summary>
        /// Iniファイルのセクション名
        /// </summary>
        const string sec = "FrmPutVerticalBrace";
        #endregion
        public PutVerticalBraceId putVerticalBraceId { get; set; }

        public enum PutVerticalBraceId : int
        {
            OK,
            Cancel
        }


        private RequestHandler m_Handler;
        private ExternalEvent m_ExEvent;

        private UIApplication _uiApp { get; set; }
        private Document doc { get; set; }
        public FrmPutVerticalBraceData data { get; set; }

        public FrmPutVerticalBrace(ExternalEvent exEvent, RequestHandler handler, UIApplication uiApp)
        {
            InitializeComponent();
            _uiApp = uiApp;
            doc = uiApp.ActiveUIDocument.Document;

            m_Handler = handler;
            m_ExEvent = exEvent;

            InitComboBox();

            ColDansuNo.DataPropertyName = nameof(DansuRow.Index);
            ColDansuSpan.DataPropertyName = nameof(DansuRow.IntervalMm);
            chkSuichokuBraceUmu.DataPropertyName = nameof(DansuRow.HasBrace);
            DgvDansu.DataBindings.Add(nameof(DataGridView.DataSource), this, nameof(DanRowSet), true, DataSourceUpdateMode.OnPropertyChanged);
            DgvDansu.AutoGenerateColumns = false;
        }

        private void FrmPutVerticalBrace_Load(object sender, EventArgs e)
        {
            GetIniData();
        }


        private void InitComboBox()
        {
            List<string> lstStr = new List<string>();

            //全構台名
            lstStr = GantryUtil.GetAllKoudaiName(doc);
            ControlUtils.SetComboBoxItems(CmbKoudaiName, lstStr);

            //部材
            if (chkTurnBackle.Checked)
            {
                lstStr = Master.ClsTurnBackleCsv.GetSizeList(Master.ClsTurnBackleCsv.TurnBackle);
            }
            else
            {
                lstStr = Master.ClsAngleCsv.GetSizeList(Master.ClsAngleCsv.TypeAngle);
            }
            ControlUtils.SetComboBoxItems(CmbSize, lstStr);
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.RbtPutWayMulti.Checked)
            {
                //this.tabControl1.Enabled = true;

                this.groupBox7.Enabled = true;
                this.groupBox5.Enabled = true;
                this.NmcDanCount.Enabled = true;
                this.NmcDanSpan.Enabled = true;
                this.DgvDansu.Enabled = true;
                return;
            }

            //this.tabControl1.Enabled = false;

            this.groupBox7.Enabled = false;
            this.groupBox5.Enabled = false;
            this.NmcDanCount.Enabled = false;
            this.NmcDanSpan.Enabled = false;
            this.DgvDansu.Enabled = false;
        }

        //キャンセルボタン
        private void button3_Click(object sender, EventArgs e)
        {
            //初期化処理
            MakeRequest(RequestId.End);
            m_ExEvent.Dispose();
            m_ExEvent = null;
            m_Handler = null;

            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        //OKボタン
        private void button1_Click(object sender, EventArgs e)
        {
            SetData();
            putVerticalBraceId = PutVerticalBraceId.OK;
            MakeRequest(RequestId.VerticalBrace);
            SetIniData();
            this.DialogResult = DialogResult.OK;
        }

        private void SetData()
        {
            FrmPutVerticalBraceData fData = new FrmPutVerticalBraceData();

            fData.SelectedKodai = CmbKoudaiName.Text;
            fData.Size = CmbSize.Text;
            fData.PlaceIsSingle = RbtPutWaySingle.Checked;
            fData.PlaceIsMultiple = RbtPutWayMulti.Checked;
            fData.VrtBraceHasRound = ChkOnRound.Checked;
            int nRound = 0;
            if(int.TryParse( CmbRound.Text,out nRound))
                {
                fData.RoundNum = nRound;
            }

            fData.VrtBrace1UpperXMm = (int)NmcFirstBanRangeTopX.Value;
            fData.VrtBrace1UpperYMm = (int)NmcFirstBanRangeTopY.Value;
            fData.VrtBrace1LowerXMm = (int)NmcFirstBanRangeBottomX.Value;
            fData.VrtBrace1LowerYMm = (int)NmcFirstBanRangeBottomY.Value;
            fData.VrtBrace2UpperXMm = (int)NmcSecondBanRangeTopX.Value;
            fData.VrtBrace2UpperYMm = (int)NmcSecondBanRangeTopY.Value;
            fData.VrtBrace2LowerXMm = (int)NmcSecondBanRangeBottomX.Value;
            fData.VrtBrace2LowerYMm = (int)NmcSecondBanRangeBottomY.Value;
            fData.VrtBraceConnectingType = (RbtAttachWayWelding.Checked ? FrmPutVerticalBraceData.ConnectingType.Welding :
                                           (RbtAttachWayBolt.Checked ? FrmPutVerticalBraceData.ConnectingType.Bolt :
                                                                    FrmPutVerticalBraceData.ConnectingType.Metal));

            fData.m_DanCount = (int)NmcDanCount.Value;
            fData.m_DanBaseInterval = (int)NmcDanSpan.Value;

            //List<DansuRow> lstR = new List<DansuRow>();
            //for (int i = 0; i < DgvDansu.Rows.Count; i++ )
            //{
            //    DataGridViewRow row = DgvDansu.Rows[i];
            //    DansuRow r = new DansuRow();
            //    r.Index = i;
            //    r.IntervalMm = (int)row.Cells[1].Value;
            //    r.HasBrace = (bool)row.Cells[2].Value;
            //    lstR.Add(r);
            //}
            fData.DanRowSet = this.DanRowSet;
            fData.IsTurnBackle = chkTurnBackle.Checked;

            data = fData;
    }

        /// <summary>
        /// ダイアログデータをini二セット
        /// </summary>
        public void SetIniData()
        {
            string iniPath = ClsIni.GetIniFilePath(IniPath);
            ClsIni.WritePrivateProfileString(sec, CmbSize.Name, CmbSize.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, RbtPutWayMulti.Name, RbtPutWayMulti.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, RbtPutWaySingle.Name, RbtPutWaySingle.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, NmcFirstBanRangeTopX.Name, NmcFirstBanRangeTopX.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, NmcFirstBanRangeTopY.Name, NmcFirstBanRangeTopY.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, NmcFirstBanRangeBottomX.Name, NmcFirstBanRangeBottomX.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, NmcFirstBanRangeBottomY.Name, NmcFirstBanRangeBottomY.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, NmcSecondBanRangeTopX.Name, NmcSecondBanRangeTopX.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, NmcSecondBanRangeTopY.Name, NmcSecondBanRangeTopY.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, NmcSecondBanRangeBottomX.Name, NmcSecondBanRangeBottomX.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, NmcSecondBanRangeBottomY.Name, NmcSecondBanRangeBottomY.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, ChkOnRound.Name, ChkOnRound.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, CmbRound.Name, CmbRound.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, RbtAttachWayWelding.Name, RbtAttachWayWelding.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, RbtAttachWayBolt.Name, RbtAttachWayBolt.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, RbtAttachWayTeiketsu.Name, RbtAttachWayTeiketsu.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, NmcDanCount.Name, NmcDanCount.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, NmcDanSpan.Name, NmcDanSpan.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, chkTurnBackle.Name, chkTurnBackle.Checked.ToString(), iniPath);

            var rowDanSettingList = new List<string>();
            foreach (DataGridViewRow row in DgvDansu.Rows)
            {
                var rowSpanValue = row.Cells[1].Value?.ToString();
                var rowHasBraceValue = row.Cells[2].Value?.ToString();
                if (string.IsNullOrEmpty(rowSpanValue) || string.IsNullOrEmpty(rowHasBraceValue)) continue;
                rowDanSettingList.Add($"{rowSpanValue}:{rowHasBraceValue}");
            }

            var rowDanSetting = string.Join("|", rowDanSettingList);
            ClsIni.WritePrivateProfileString(sec, DgvDansu.Name, rowDanSetting, iniPath);
        }

        /// <summary>
        /// iniデータをダイアログにセット
        /// </summary>
        public void GetIniData()
        {
            string iniPath = ClsIni.GetIniFilePath(IniPath);
            if (File.Exists(iniPath))
            {
                CmbSize.Text = ClsIni.GetIniFile(sec, CmbSize.Name, iniPath);
                RbtPutWayMulti.Checked = bool.Parse(ClsIni.GetIniFile(sec, RbtPutWayMulti.Name, iniPath));
                RbtPutWaySingle.Checked = bool.Parse(ClsIni.GetIniFile(sec, RbtPutWaySingle.Name, iniPath));
                NmcFirstBanRangeTopX.Text = ClsIni.GetIniFile(sec, NmcFirstBanRangeTopX.Name, iniPath);
                NmcFirstBanRangeTopY.Text = ClsIni.GetIniFile(sec, NmcFirstBanRangeTopY.Name, iniPath);
                NmcFirstBanRangeBottomX.Text = ClsIni.GetIniFile(sec, NmcFirstBanRangeBottomX.Name, iniPath);
                NmcFirstBanRangeBottomY.Text = ClsIni.GetIniFile(sec, NmcFirstBanRangeBottomY.Name, iniPath);
                NmcSecondBanRangeTopX.Text = ClsIni.GetIniFile(sec, NmcSecondBanRangeTopX.Name, iniPath);
                NmcSecondBanRangeTopY.Text = ClsIni.GetIniFile(sec, NmcSecondBanRangeTopY.Name, iniPath);
                NmcSecondBanRangeBottomX.Text = ClsIni.GetIniFile(sec, NmcSecondBanRangeBottomX.Name, iniPath);
                NmcSecondBanRangeBottomY.Text = ClsIni.GetIniFile(sec, NmcSecondBanRangeBottomY.Name, iniPath);
                ChkOnRound.Checked = bool.Parse(ClsIni.GetIniFile(sec, ChkOnRound.Name, iniPath));
                CmbRound.Text = ClsIni.GetIniFile(sec, CmbRound.Name, iniPath);
                RbtAttachWayWelding.Checked = bool.Parse(ClsIni.GetIniFile(sec, RbtAttachWayWelding.Name, iniPath));
                RbtAttachWayBolt.Checked = bool.Parse(ClsIni.GetIniFile(sec, RbtAttachWayBolt.Name, iniPath));
                RbtAttachWayTeiketsu.Checked = bool.Parse(ClsIni.GetIniFile(sec, RbtAttachWayTeiketsu.Name, iniPath));
                NmcDanCount.Text = ClsIni.GetIniFile(sec, NmcDanCount.Name, iniPath);
                NmcDanSpan.Text = ClsIni.GetIniFile(sec, NmcDanSpan.Name, iniPath);
                chkTurnBackle.Checked = bool.Parse(ClsIni.GetIniFile(sec, chkTurnBackle.Name, iniPath));

                var dgvDansuValue = ClsIni.GetIniFile(sec, DgvDansu.Name, iniPath);

                if (!string.IsNullOrEmpty(dgvDansuValue) && dgvDansuValue != "none")
                {
                    var dgvDansuArray = dgvDansuValue.Split('|');

                    for (var i = 0; i < dgvDansuArray.Count(); i++)
                    {
                        var valueArray = dgvDansuArray[i].Split(':');
                        DanRowSet[i].IntervalMm = int.Parse(valueArray[0]);
                        DanRowSet[i].HasBrace = bool.Parse(valueArray[1]);
                    }
                }
            }
        }


        public class FrmPutVerticalBraceData : BindableSuper
        {
            public enum ConnectingType
            {
                Nil, Welding, Bolt, Metal
            }

            public enum SetFace
            {
                UpperB, LowerT, LowerB
            }

            public string SelectedKodai { get; set; }
            public string Size { get; set; }
            public bool PlaceIsSingle { get; set; }
            public bool PlaceIsMultiple { get; set; } = true;
            public bool VrtBraceHasRound { get; set; }
            public int RoundNum { get; set; }
            public double RoundNumFt => ClsRevitUtil.CovertToAPI(RoundNum);

            public int VrtBrace1UpperXMm { get; set; }
            public double VrtBrace1UpperXFt => ClsRevitUtil.CovertToAPI(VrtBrace1UpperXMm);
            public int VrtBrace1UpperYMm { get; set; }
            public double VrtBrace1UpperYFt => ClsRevitUtil.CovertToAPI(VrtBrace1UpperYMm);

            public int VrtBrace2UpperXMm { get; set; }
            public double VrtBrace2UpperXFt => ClsRevitUtil.CovertToAPI(VrtBrace2UpperXMm);
            public int VrtBrace2UpperYMm { get; set; }
            public double VrtBrace2UpperYFt => ClsRevitUtil.CovertToAPI(VrtBrace2UpperYMm);

            public int VrtBrace1LowerXMm { get; set; }
            public double VrtBrace1LowerXFt => ClsRevitUtil.CovertToAPI(VrtBrace1LowerXMm);
            public int VrtBrace1LowerYMm { get; set; }
            public double VrtBrace1LowerYFt => ClsRevitUtil.CovertToAPI(VrtBrace1LowerYMm);

            public int VrtBrace2LowerXMm { get; set; }
            public double VrtBrace2LowerXFt => ClsRevitUtil.CovertToAPI(VrtBrace2LowerXMm);
            public int VrtBrace2LowerYMm { get; set; }
            public double VrtBrace2LowerYFt => ClsRevitUtil.CovertToAPI(VrtBrace2LowerYMm);

            public int m_DanCount { get; set; }
            public int m_DanBaseInterval { get; set; }

            public ConnectingType VrtBraceConnectingType { get; set; }
            public BindingList<DansuRow> DanRowSet { get; set; }

            public class DansuRow
            {
                public int Index { get; set; }
                public int IntervalMm { get; set; }
                public double IntervalFt => ClsRevitUtil.CovertToAPI(IntervalMm);
                public bool HasBrace { get; set; }
            }
            public bool IsTurnBackle { get; set; }

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

        public BindingList<DansuRow> DanRowSet { get; } = new BindingList<DansuRow>();
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            //if (!(sender is YmsNumericInteger numeric)) { return; }

            int nInterval = (int)NmcDanSpan.Value;

            var dstCount = (int)NmcDanCount.Value;
            var srcCount = DanRowSet.Count();

            if (dstCount < srcCount)
            {
                for (int i = 0; i < srcCount - dstCount; i++)
                {
                    DanRowSet.RemoveAt(DanRowSet.Count() - 1);
                }
            }
            else if (srcCount < dstCount)
            {
                for (int i = 0; i < dstCount - srcCount; i++)
                {
                    DanRowSet.Add(new DansuRow
                    {
                        Index = DanRowSet.Count() + 1,
                        IntervalMm = nInterval,
                        HasBrace = true,
                    });
                }
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            CmbRound.Enabled = ChkOnRound.Checked;
        }

        //一括配置時の値参照ボタン
        private void button2_Click(object sender, EventArgs e)
        {
            var koudaiData = GantryUtil.GetKoudaiData(doc, CmbKoudaiName.Text);
            if (koudaiData == null || koudaiData.BraceTsunagiData == null || koudaiData.BraceTsunagiData.VerticalBrace == null || koudaiData.BraceTsunagiData.DanSetting == null)
            {
                MessageUtil.Information("一括配置情報が存在しません。", this.Text);
                return;
            }
            else
            {
                if (MessageUtil.YesNo("一括配置情報が存在します。展開しますか？", this.Text) == DialogResult.No)
                    return;
                else
                {
                    //ダイアログに一括配置情報を展開
                    var vrtBraceData = koudaiData.BraceTsunagiData.VerticalBrace;
                    CmbSize.Text = vrtBraceData.Size;

                    //「ブレース」タブ
                    NmcFirstBanRangeTopX.Value = vrtBraceData.FirstHanareTopX;
                    NmcFirstBanRangeTopY.Value = vrtBraceData.FirstHanareTopY;
                    NmcFirstBanRangeBottomX.Value = vrtBraceData.FirstHanareBottomX;
                    NmcFirstBanRangeBottomY.Value = vrtBraceData.FirstHanareBottomY;

                    NmcSecondBanRangeTopX.Value = vrtBraceData.SecondLaterHanareTopX;
                    NmcSecondBanRangeTopY.Value = vrtBraceData.SecondLaterHanareTopY;
                    NmcSecondBanRangeBottomX.Value = vrtBraceData.SecondLaterHanareBottomX;
                    NmcSecondBanRangeBottomY.Value = vrtBraceData.SecondLaterHanareBottomY;

                    ChkOnRound.Checked = vrtBraceData.RoundON;
                    CmbRound.Text = vrtBraceData.RoundLength;

                    RbtAttachWayWelding.Checked = vrtBraceData.ToritsukiYousetsu;
                    RbtAttachWayBolt.Checked = vrtBraceData.ToritsukiBolt;
                    RbtAttachWayTeiketsu.Checked = vrtBraceData.ToritsukiTeiketsuKanagu;

                    //「段数設定」タブ
                    var danSettingData = koudaiData.BraceTsunagiData.DanSetting;
                    NmcDanCount.Value = danSettingData.TsunagiDansu;
                    NmcDanSpan.Value = danSettingData.BaseSpan;

                    this.DanRowSet.Clear();
                    foreach (var danSettingListData in danSettingData.DanSettingDataList)
                    {
                        DansuRow dansuRow = new DansuRow();
                        if (int.TryParse(danSettingListData.Dan, out int tmpInt)) dansuRow.Index = tmpInt;
                        dansuRow.IntervalMm = danSettingListData.Span;
                        dansuRow.HasBrace = danSettingListData.BraceUmu;

                        this.DanRowSet.Add(dansuRow);
                    }
                }
            }
        }

        private void chkTurnBackle_CheckedChanged(object sender, EventArgs e)
        {
            InitComboBox();

            if (chkTurnBackle.Checked)
            {
                RbtPutWaySingle.Checked = true;

                //tabControl1.Enabled = false;
                this.groupBox7.Enabled = false;
                this.groupBox5.Enabled = false;
                this.ChkOnRound.Enabled = false;
                this.CmbRound.Enabled = false;
                this.NmcDanCount.Enabled = false;
                this.NmcDanSpan.Enabled = false;
                this.DgvDansu.Enabled = false;

                this.groupBox1.Enabled = false;
            }
            else
            {
                //tabControl1.Enabled = true;
                this.groupBox7.Enabled = true;
                this.groupBox5.Enabled = true;
                this.ChkOnRound.Enabled = true;
                this.CmbRound.Enabled = true;
                this.NmcDanCount.Enabled = true;
                this.NmcDanSpan.Enabled = true;
                this.DgvDansu.Enabled = true;

                this.groupBox1.Enabled = true;
            }
        }
    }
}
