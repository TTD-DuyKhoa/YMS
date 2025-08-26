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
using YMS.Parts;
using static YMS.ClsCASE;

namespace YMS.DLG
{
    public partial class DlgCreateRenzokukabe : Form
    {
        #region 定数
        /// <summary>
        /// Iniファイルのパス
        /// </summary>
        const string IniPath = "ini\\DlgCreateRenzokukabe.ini";

        /// <summary>
        /// Iniファイルのセクション名
        /// </summary>
        const string sec = "DlgRenzokukabe";
        #endregion

        #region メンバー
        public ClsRenzokukabe m_ClsRenzokukabe;
        private bool m_ChangeDGVKuiLength = false;
        private ClsCASE m_caseInfo;
        #endregion
        public DlgCreateRenzokukabe(Autodesk.Revit.DB.Document doc)
        {
            InitializeComponent();
            m_ClsRenzokukabe = new ClsRenzokukabe();
            m_caseInfo = new ClsCASE(ClsCASE.enKabeshu.renzokukabe,doc);
            initComboBox();
            GetIniData();
        }

        public DlgCreateRenzokukabe(Autodesk.Revit.DB.Document doc,ClsRenzokukabe clsRenzokukabe)
        {
            InitializeComponent();
            m_ClsRenzokukabe = clsRenzokukabe;
            m_caseInfo = new ClsCASE(ClsCASE.enKabeshu.renzokukabe, doc);
            initComboBox();
            GetIniData();
            SetControl();
        }
        public void initComboBox()
        {
            string bk = string.Empty;
            List<string> lstStr = new List<string>();

            //処理タイプ・残置
            cmbShoriType.Items.AddRange(ClsGlobal.m_syorihouhou4desumi.ToArray());
            cmbZanchi.Items.AddRange(ClsGlobal.m_zanti.ToArray());
            cmbShoriType.SelectedIndex = 1;

            initComboBoxBoltType();

            //芯材タイプ
            bk = cmbShinzaiType.Text;
            cmbShinzaiType.Items.Clear();
            lstStr = Master.ClsHBeamCsv.GetTypeList();
            foreach (string str in lstStr)
            {
                cmbShinzaiType.Items.Add(str);
            }
            if (lstStr.Contains(bk)) cmbShinzaiType.Text = bk;
            else cmbShinzaiType.SelectedIndex = 0;

            //芯材サイズ
            initComboBoxSteelSize();
        }

        public void initComboBoxSteelSize()
        {
            string bk = string.Empty;
            List<string> lstStr = new List<string>();

            //芯材サイズ
            bk = cmbShinzaiSize.Text;
            cmbShinzaiSize.Items.Clear();
            lstStr = Master.ClsHBeamCsv.GetSizeList(cmbShinzaiType.Text);
            foreach (string str in lstStr)
            {
                cmbShinzaiSize.Items.Add(str);
            }
            if (lstStr.Contains(bk)) cmbShinzaiSize.Text = bk;
            else cmbShinzaiSize.SelectedIndex = 0;

            initOtherControl();
        }

        public void initOtherControl()
        {
            string bk = string.Empty;
            List<string> lstStr = new List<string>();
            List<string> lstStr2 = new List<string>();
            List<string> lstStrW = new List<string>();
            List<string> lstStrW2 = new List<string>();

            if (rbnKoteiTypeBolt.Checked)
            {
                lstStr = Master.ClsBoltCsv.GetSizeList(cmbBoltTypeF.Text);
                lstStrW = Master.ClsBoltCsv.GetSizeList(cmbBoltTypeW.Text);
            }
            else
            {
                lstStr = new List<string>() { string.Empty };
                lstStrW = new List<string>() { string.Empty };
            }
            cmbBoltTypeF.Enabled = rbnKoteiTypeBolt.Checked;
            cmbBoltSizeFlangeSide.Enabled = rbnKoteiTypeBolt.Checked;
            txtBoltCountFlangeSide.Enabled = rbnKoteiTypeBolt.Checked;
            cmbBoltTypeW.Enabled = rbnKoteiTypeBolt.Checked;
            cmbBoltSizeWebSide.Enabled = rbnKoteiTypeBolt.Checked;
            txtBoltCountWebSide.Enabled = rbnKoteiTypeBolt.Checked;
            if (rbnKoteiTypeBolt2.Checked)
            {
                lstStr2 = Master.ClsBoltCsv.GetSizeList(cmbBoltTypeF2.Text);
                lstStrW2 = Master.ClsBoltCsv.GetSizeList(cmbBoltTypeW2.Text);
            }
            else
            {
                lstStr2 = new List<string>() { string.Empty };
                lstStrW2 = new List<string>() { string.Empty };
            }
            cmbBoltTypeF2.Enabled = rbnKoteiTypeBolt2.Checked;
            cmbBoltSizeFlangeSide2.Enabled = rbnKoteiTypeBolt2.Checked;
            txtBoltCountFlangeSide2.Enabled = rbnKoteiTypeBolt2.Checked;
            cmbBoltTypeW2.Enabled = rbnKoteiTypeBolt2.Checked;
            cmbBoltSizeWebSide2.Enabled = rbnKoteiTypeBolt2.Checked;
            txtBoltCountWebSide2.Enabled = rbnKoteiTypeBolt2.Checked;

            //ボルト1-1
            bk = cmbBoltSizeFlangeSide.Text;
            cmbBoltSizeFlangeSide.Items.Clear();
            foreach (string str in lstStr)
            {
                cmbBoltSizeFlangeSide.Items.Add(str);
            }
            if (!string.IsNullOrWhiteSpace(cmbBoltTypeF.Text))
            {
                if (lstStr.Contains(bk)) cmbBoltSizeFlangeSide.Text = bk;
                else cmbBoltSizeFlangeSide.SelectedIndex = 0;
            }
            //ボルト1-2
            bk = cmbBoltSizeWebSide.Text;
            cmbBoltSizeWebSide.Items.Clear();
            foreach (string str in lstStrW)
            {
                cmbBoltSizeWebSide.Items.Add(str);
            }
            if (!string.IsNullOrWhiteSpace(cmbBoltTypeW.Text))
            {
                if (lstStrW.Contains(bk)) cmbBoltSizeWebSide.Text = bk;
                else cmbBoltSizeWebSide.SelectedIndex = 0;
            }
            //ボルト2-1
            bk = cmbBoltSizeFlangeSide2.Text;
            cmbBoltSizeFlangeSide2.Items.Clear();
            foreach (string str in lstStr2)
            {
                cmbBoltSizeFlangeSide2.Items.Add(str);
            }
            if (!string.IsNullOrWhiteSpace(cmbBoltTypeF2.Text))
            {
                if (lstStr2.Contains(bk)) cmbBoltSizeFlangeSide2.Text = bk;
                else cmbBoltSizeFlangeSide2.SelectedIndex = 0;
            }
            //ボルト2-2
            bk = cmbBoltSizeWebSide2.Text;
            cmbBoltSizeWebSide2.Items.Clear();
            foreach (string str in lstStrW2)
            {
                cmbBoltSizeWebSide2.Items.Add(str);
            }
            if (!string.IsNullOrWhiteSpace(cmbBoltTypeW2.Text))
            {
                if (lstStrW2.Contains(bk)) cmbBoltSizeWebSide2.Text = bk;
                else cmbBoltSizeWebSide2.SelectedIndex = 0;
            }
            //CSVから継手情報を取得
            string strKotei = (rbnKoteiTypeBolt.Checked ? Master.ClsPileTsugiteCsv.KoteiHohoBolt : Master.ClsPileTsugiteCsv.KoteiHohoYousetsu);
            Master.ClsPileTsugiteCsv pileCsv = Master.ClsPileTsugiteCsv.GetCls(strKotei, cmbShinzaiSize.Text);

            string strKotei2 = (rbnKoteiTypeBolt2.Checked ? Master.ClsPileTsugiteCsv.KoteiHohoBolt : Master.ClsPileTsugiteCsv.KoteiHohoYousetsu);
            Master.ClsPileTsugiteCsv pileCsv2 = Master.ClsPileTsugiteCsv.GetCls(strKotei2, cmbShinzaiSize.Text);

            //各コントロールに値を設定
            txtPlateSizeFlangeOutSide.Text = pileCsv.PlateSizeFOut;
            txtPlateCountFlangeOutSide.Text = pileCsv.PlateNumFOut.ToString();
            txtPlateSizeFlangeInSide.Text = pileCsv.PlateSizeFIn;
            txtPlateCountFlangeInSide.Text = pileCsv.PlateNumFIn.ToString();
            txtPlateSizeWebSide1.Text = pileCsv.PlateSizeW;
            txtPlateCountWebSide1.Text = pileCsv.PlateNumW.ToString();
            txtPlateSizeWebSide2.Text = pileCsv.PlateSizeW2;
            txtPlateCountWebSide2.Text = pileCsv.PlateNumW2.ToString();
            cmbBoltSizeFlangeSide.Text = pileCsv.BoltSizeF;
            txtBoltCountFlangeSide.Text = pileCsv.BoltNumF.ToString();
            cmbBoltSizeWebSide.Text = pileCsv.BoltSizeW;
            txtBoltCountWebSide.Text = pileCsv.BoltNumW.ToString();

            txtPlateSizeFlangeOutSide2.Text = pileCsv2.PlateSizeFOut;
            txtPlateCountFlangeOutSide2.Text = pileCsv2.PlateNumFOut.ToString();
            txtPlateSizeFlangeInSide2.Text = pileCsv2.PlateSizeFIn;
            txtPlateCountFlangeInSide2.Text = pileCsv2.PlateNumFIn.ToString();
            txtPlateSizeWebSide12.Text = pileCsv2.PlateSizeW;
            txtPlateCountWebSide12.Text = pileCsv2.PlateNumW.ToString();
            txtPlateSizeWebSide22.Text = pileCsv2.PlateSizeW2;
            txtPlateCountWebSide22.Text = pileCsv2.PlateNumW2.ToString();
            cmbBoltSizeFlangeSide2.Text = pileCsv2.BoltSizeF;
            txtBoltCountFlangeSide2.Text = pileCsv2.BoltNumF.ToString();
            cmbBoltSizeWebSide2.Text = pileCsv2.BoltSizeW;
            txtBoltCountWebSide2.Text = pileCsv2.BoltNumW.ToString();
        }

        public void initComboBoxBoltType()
        {
            // ボルトタイプ
            string selectedTextF = cmbBoltTypeF.Text;
            cmbBoltTypeF.Items.Clear();
            string selectedTextF2 = cmbBoltTypeF2.Text;
            cmbBoltTypeF2.Items.Clear();
            string selectedTextW = cmbBoltTypeW.Text;
            cmbBoltTypeW.Items.Clear();
            string selectedTextW2 = cmbBoltTypeW2.Text;
            cmbBoltTypeW2.Items.Clear();

            List<string> lstStr = new List<string>() { Master.ClsBoltCsv.BoltTypeT,
                                          Master.ClsBoltCsv.BoltTypeN,
                                          Master.ClsBoltCsv.BoltTypeH,
                                          };

            if (rbnKoteiTypeBolt.Checked)
            {
                foreach (string str in lstStr)
                {
                    cmbBoltTypeF.Items.Add(str);
                    cmbBoltTypeW.Items.Add(str);
                }
                if (lstStr.Contains(selectedTextF)) cmbBoltTypeF.Text = selectedTextF;
                else cmbBoltTypeF.SelectedIndex = 0;
                if (lstStr.Contains(selectedTextW)) cmbBoltTypeW.Text = selectedTextW;
                else cmbBoltTypeW.SelectedIndex = 0;
            }

            if (rbnKoteiTypeBolt2.Checked)
            {
                foreach (string str in lstStr)
                {
                    cmbBoltTypeF2.Items.Add(str);
                    cmbBoltTypeW2.Items.Add(str);
                }
                if (lstStr.Contains(selectedTextF2)) cmbBoltTypeF2.Text = selectedTextF2;
                else cmbBoltTypeF2.SelectedIndex = 0;
                if (lstStr.Contains(selectedTextW2)) cmbBoltTypeW2.Text = selectedTextW2;
                else cmbBoltTypeW2.SelectedIndex = 0;
            }
        }

        private void DlgCreateRenzokukabe_Load(object sender, EventArgs e)
        {
            //cmbShoriType.Items.AddRange(ClsGlobal.m_syorihouhou4desumi.ToArray());
            //cmbZanchi.Items.AddRange(ClsGlobal.m_zanti1.ToArray());
            cmbBoltSizeFlangeSide.Items.AddRange(ClsGlobal.m_boltsize.ToArray());
            cmbBoltSizeWebSide.Items.AddRange(ClsGlobal.m_boltsize.ToArray());
            int nKashoNum = 0;
            int.TryParse(txtKashoNum.Text, out nKashoNum);
            InitDataGridView(dgvPileLength, nKashoNum);

            int.TryParse(txtKashoNum2.Text, out nKashoNum);
            InitDataGridView(dgvPileLength2, nKashoNum);
            dgvPileLength2.Enabled = false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!CheckInput())
                return;

            SetData();
            SetIniData();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnReference_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Retry;
            this.Close();
        }

        public void SetData()
        {
            ClsRenzokukabe crk = m_ClsRenzokukabe;
            crk.m_case = ClsCommonUtils.ChangeStrToInt(txtCase.Text);
            crk.m_edaNum = txtEdaNumber.Text;
            crk.m_edaNum2 = tabPage2.Text;
            crk.m_way = cmbShoriType.SelectedIndex;
            crk.m_void = ClsCommonUtils.ChangeStrToInt(txtKussakuFukasa.Text);
            crk.m_refPDist = ClsCommonUtils.ChangeStrToInt(txtKijuntenKyori.Text);
            crk.m_zanti = cmbZanchi.Text;
            crk.m_zantiLength = txtZantiLength.Text;
            crk.m_kabeAtumi = ClsCommonUtils.ChangeStrToInt(txtRenzokuWallThickness.Text);
            crk.m_kabeLen = ClsCommonUtils.ChangeStrToInt(txtWallLength.Text);
            crk.m_bVoid = chkVoid.Checked;
            crk.m_size = cmbShinzaiSize.Text;
            crk.m_HTop = ClsCommonUtils.ChangeStrToInt(txtShinzaiTop.Text);
            crk.m_HLen = ClsCommonUtils.ChangeStrToInt(txtShinzaiTotalLength.Text);
            crk.m_pitch = ClsCommonUtils.ChangeStrToInt(txtPitch.Text);
            crk.m_KougoFlg = chkKougoHaichi.Checked;
            crk.m_Kasho1 = ClsCommonUtils.ChangeStrToInt(txtKashoNum.Text);
            crk.m_Kasho2 = ClsCommonUtils.ChangeStrToInt(txtKashoNum2.Text);
            crk.m_Kotei1 = rbnKoteiTypeBolt.Checked ? Master.Kotei.Bolt : Master.Kotei.Yousetsu;
            crk.m_Kotei2 = rbnKoteiTypeBolt2.Checked ? Master.Kotei.Bolt : Master.Kotei.Yousetsu;

            List<int> lstN1 = new List<int>();
            for (int i = 0; i < dgvPileLength.Columns.Count; i++)
            {
                lstN1.Add(ClsCommonUtils.ChangeStrToInt(dgvPileLength[i, 0].Value.ToString()));
            }
            crk.m_ListPileLength1 = lstN1;
            List<int> lstN2 = new List<int>();
            for (int i = 0; i < dgvPileLength2.Columns.Count; i++)
            {
                lstN2.Add(ClsCommonUtils.ChangeStrToInt(dgvPileLength2[i, 0].Value.ToString()));
            }
            crk.m_ListPileLength2 = lstN2;

            crk.m_BoltF1 = cmbBoltSizeFlangeSide.Text;
            crk.m_BoltFNum1 = txtBoltCountFlangeSide.Text;
            crk.m_BoltW1 = cmbBoltSizeWebSide.Text;
            crk.m_BoltWNum1 = txtBoltCountWebSide.Text;
            crk.m_BoltF2 = cmbBoltSizeFlangeSide2.Text;
            crk.m_BoltFNum2 = txtBoltCountFlangeSide2.Text;
            crk.m_BoltW2 = cmbBoltSizeWebSide2.Text;
            crk.m_BoltWNum2 = txtBoltCountWebSide2.Text;

            //メンバ変数に格納
            m_ClsRenzokukabe = crk;
        }
        private void txtKashoNum2_TextChanged(object sender, EventArgs e)
        {
            //int nKashoNum = 0;
            //if (!int.TryParse(txtKashoNum2.Text, out nKashoNum))
            //{
            //    return;
            //}

            //if (nKashoNum < 0 || nKashoNum > 10)
            //{
            //    MessageBox.Show("接続箇所数は0以上10以下の整数を入力してください。");
            //    return;
            //}
            //InitDataGridView(dgvPileLength2, nKashoNum);
        }

        private void txtKashoNum_TextChanged(object sender, EventArgs e)
        {
            int nKashoNum = 0;
            if (!int.TryParse(txtKashoNum.Text, out nKashoNum))
            {
                return;
            }

            if (nKashoNum < 0 || nKashoNum > 10)
            {
                MessageBox.Show("接続箇所数は0以上10以下の整数を入力してください。");
                return;
            }

            InitDataGridView(dgvPileLength, nKashoNum);

            txtKashoNum2.Text = txtKashoNum.Text;
            InitDataGridView(dgvPileLength2, nKashoNum);
        }

        private void InitDataGridView(DataGridView dgv, int nColmnCount)
        {
            dgv.Columns.Clear();
            dgv.Rows.Clear();

            for (int i = 0; i <= nColmnCount; i++)
            {
                DataGridViewTextBoxColumn clm = new DataGridViewTextBoxColumn();
                clm.Name = "clm" + (i + 1).ToString();
                clm.HeaderText = "杭" + (i + 1).ToString() + "長さ";
                dgv.Columns.Add(clm);
            }
            if (nColmnCount >= 0) dgv.Rows.Add();

            int remainingLength = ClsCommonUtils.ChangeStrToInt(txtShinzaiTotalLength.Text);
            int kuiLength = remainingLength / (nColmnCount + 1);
            for (int i = 0; i <= nColmnCount; i++)
            {
                if (i != nColmnCount)
                {
                    dgv[i, 0].Value = kuiLength;
                    remainingLength -= kuiLength;
                }
                else
                    dgv[i, 0].Value = remainingLength;
            }
        }

        /// <summary>
        /// 入力チェック
        /// </summary>
        /// <returns></returns>
        public bool CheckInput()
        {
            if (!ClsCommonUtils.ChkContorolText(txtCase, "CASE", bNull: true))
                return false;
            if (!ClsCommonUtils.ChkContorolTextAlphabet(txtEdaNumber, "枝番号", bNull: true))
                return false;
            if (!ClsCommonUtils.ChkContorolTextDouble(txtKussakuFukasa, "掘削深さ"))
                return false;
            if (!ClsCommonUtils.ChkContorolTextDouble(txtKijuntenKyori, "基準点からの距離"))
                return false;
            if (!ClsCommonUtils.ChkContorolTextDouble(txtRenzokuWallThickness, "連続壁の厚み", bMinus: true, bZero: true))
                return false;
            if (!ClsCommonUtils.ChkContorolTextDouble(txtWallLength, "壁全長", bMinus: true, bZero: true))
                return false;
            
            if (!ClsCommonUtils.ChkContorolTextDouble(txtShinzaiTop, "芯材：天端", bMinus: true))
                return false;
            if (!ClsCommonUtils.ChkContorolTextDouble(txtShinzaiTotalLength, "芯材：全長", bMinus: true, bZero: true))
                return false;
            if (!ClsCommonUtils.ChkContorolTextDouble(txtPitch, "芯材：ピッチ", bMinus: true, bZero: true))
                return false;
            if (!ClsCommonUtils.ChkContorolTextDouble(txtKashoNum, "箇所数(1)", bMinus: true, bComma: true))
                return false;
            if (!ClsCommonUtils.ChkContorolTextDouble(txtKashoNum2, "箇所数(2)", bMinus: true, bComma: true))
                return false;
            if (!ClsCommonUtils.ChkContorolTextDouble(txtBoltCountFlangeSide, "ボルト本数(1)（フランジ側）", bMinus: true, bComma: true))
                return false;
            if (!ClsCommonUtils.ChkContorolTextDouble(txtBoltCountFlangeSide2, "ボルト本数(2)（フランジ側）", bMinus: true, bComma: true))
                return false;
            if (!ClsCommonUtils.ChkContorolTextDouble(txtBoltCountWebSide, "ボルト本数(1)（Web側）", bMinus: true, bComma: true))
                return false;
            if (!ClsCommonUtils.ChkContorolTextDouble(txtBoltCountWebSide2, "ボルト本数(2)（Web側）", bMinus: true, bComma: true))
                return false;

            //杭の全長チェック
            List<int> lstN1 = new List<int>();
            for (int i = 0; i < dgvPileLength.Columns.Count; i++)
            {
                lstN1.Add(ClsCommonUtils.ChangeStrToInt(dgvPileLength[i, 0].Value.ToString()));
            };
            if (lstN1.Sum() != (ClsCommonUtils.ChangeStrToInt(txtShinzaiTotalLength.Text)))
            {
                MessageBox.Show("杭の長さの合計と全長が一致しません。[タブ1]");
                return false;
            }

            if (chkKougoHaichi.Checked)
            {
                List<int> lstN2 = new List<int>();
                for (int i = 0; i < dgvPileLength2.Columns.Count; i++)
                {
                    lstN2.Add(ClsCommonUtils.ChangeStrToInt(dgvPileLength2[i, 0].Value.ToString()));
                }
                if (lstN2.Sum() != (ClsCommonUtils.ChangeStrToInt(txtShinzaiTotalLength.Text)))
                {
                    MessageBox.Show("杭の長さの合計と全長が一致しません。[タブ2]");
                    return false;
                }
            }

            return true;
        }
        private void rbnKoteiTypeBolt_CheckedChanged(object sender, EventArgs e)
        {
            initComboBoxBoltType();
            initOtherControl();
            SetKougoType();
        }

        private void rbnKoteiTypeYosetsu_CheckedChanged(object sender, EventArgs e)
        {
            initComboBoxBoltType();
            initOtherControl();
            SetKougoType();
        }
        private void rbnKoteiTypeBolt2_CheckedChanged(object sender, EventArgs e)
        {
            initComboBoxBoltType();
            initOtherControl();
        }

        private void rbnKoteiTypeYosetsu2_CheckedChanged(object sender, EventArgs e)
        {
            initComboBoxBoltType();
            initOtherControl();
        }

        private void cmbShinzaiSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            initComboBoxBoltType();
            initOtherControl();
        }

        private void cmbShinzaiType_SelectedIndexChanged(object sender, EventArgs e)
        {
            initComboBoxSteelSize();
        }

        private void cmbBoltTypeF_SelectedIndexChanged(object sender, EventArgs e)
        {
            initOtherControl();
        }

        private void cmbBoltTypeW_SelectedIndexChanged(object sender, EventArgs e)
        {
            initOtherControl();
        }

        private void cmbBoltTypeF2_SelectedIndexChanged(object sender, EventArgs e)
        {
            initOtherControl();
        }

        private void cmbBoltTypeW2_SelectedIndexChanged(object sender, EventArgs e)
        {
            initOtherControl();
        }

        private void txtShinzaiTotallength_TextChanged(object sender, EventArgs e)
        {
            int nKashoNum = 0;
            if (!int.TryParse(txtKashoNum.Text, out nKashoNum))
            {
                return;
            }

            InitDataGridView(dgvPileLength, nKashoNum);
        }


        private void dgvPileLength_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            dgvPileLengthCellValueChanged(dgvPileLength, e.ColumnIndex);
        }
        private void dgvPileLength_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            m_ChangeDGVKuiLength = true;
        }
        private void dgvPileLength2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //dgvPileLengthCellValueChanged(dgvPileLength2, e.ColumnIndex);
        }
        private void dgvPileLength2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            m_ChangeDGVKuiLength = true;
        }
        private void dgvPileLengthCellValueChanged(DataGridView dgv, int columnIndex)
        {
            if (!m_ChangeDGVKuiLength)
                return;
            m_ChangeDGVKuiLength = false;
            int nColmnCount = dgv.Columns.Count - 1;
            int kuiTotal = ClsCommonUtils.ChangeStrToInt(txtShinzaiTotalLength.Text);
            int total = 0;
            if (columnIndex == nColmnCount)//最終列変更時
            {
                for (int i = 0; i < columnIndex + 1; i++)
                {
                    total += ClsCommonUtils.GetCellValToInt(dgv[i, 0]);
                }
            }
            else
            {
                int remainingLength = 0;// kuiTotal - ClsCommonUtils.ChangeStrToInt(dgv[columnIndex, 0].Value.ToString());
                                        //int kuiLength = remainingLength / (nColmnCount - 1);
                for (int i = 0; i < nColmnCount; i++)
                {
                    remainingLength += ClsCommonUtils.GetCellValToInt(dgv[i, 0]);
                }
                dgv[nColmnCount, 0].Value = kuiTotal - remainingLength;
                total = kuiTotal;
            }
            if (kuiTotal != total)
            {
                //赤くする
                lblWarning.Text = "杭長さの合計と全長が一致しません。";
                ChangeCellColor(dgv, Color.Red);
            }
            else
            {
                lblWarning.Text = "";
                ChangeCellColor(dgv, Color.Empty);
            }
            if (chkKougoHaichi.Checked)
                SetKougoHaichi();
        }

        private void ChangeCellColor(DataGridView dgv, Color color)
        {
            for (int i = 0; i < dgvPileLength.Columns.Count; i++)
            {
                dgv[i, 0].Style.BackColor = color;
            }
        }

        private void SetKougoHaichi()
        {
            int nColmnCount = dgvPileLength.Columns.Count - 1;
            int nColmnCount2 = dgvPileLength2.Columns.Count - 1;
            if (nColmnCount != nColmnCount2)
                return;
            int nRowCount = dgvPileLength.Rows.Count;
            int nRowCount2 = dgvPileLength2.Rows.Count;
            if (nRowCount <= 0 || nRowCount2 <= 0)
                return;

            int total = 0;
            for (int i = 0; i <= nColmnCount; i++)
            {
                dgvPileLength2[i, 0].Value = dgvPileLength[nColmnCount - i, 0].Value;
                total += ClsCommonUtils.ChangeStrToInt(dgvPileLength[i, 0].Value.ToString());
            }
            if (ClsCommonUtils.ChangeStrToInt(txtShinzaiTotalLength.Text) != total)
                ChangeCellColor(dgvPileLength2, Color.Red);
            else
                ChangeCellColor(dgvPileLength2, Color.Empty);
        }
        private void SetKougoType()
        {
            if (rbnKoteiTypeBolt.Checked)
                rbnKoteiTypeBolt2.Checked = true;
            else
                rbnKoteiTypeYosetsu2.Checked = true;
        }
        private void chkKougoHaichi_CheckedChanged(object sender, EventArgs e)
        {
            if (chkKougoHaichi.Checked)
            {
                SetKougoHaichi();
                SetNextEdaNum();
                SetKougoType();
            }
        }

        /// <summary>
        /// ダイアログデータをini二セット
        /// </summary>
        public void SetIniData()
        {
            string iniPath = ClsIni.GetIniFilePath(IniPath);

            ClsIni.WritePrivateProfileString(sec, txtCase.Name, txtCase.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, txtEdaNumber.Name, txtEdaNumber.Text, iniPath);

            ClsIni.WritePrivateProfileString(sec, cmbShoriType.Name, cmbShoriType.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, txtKussakuFukasa.Name, txtKussakuFukasa.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, txtKijuntenKyori.Name, txtKijuntenKyori.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, cmbZanchi.Name, cmbZanchi.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, txtZantiLength.Name, txtZantiLength.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, txtRenzokuWallThickness.Name, txtRenzokuWallThickness.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, txtWallLength.Name, txtWallLength.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, chkVoid.Name, chkVoid.Checked.ToString(), iniPath);

            ClsIni.WritePrivateProfileString(sec, cmbShinzaiType.Name, cmbShinzaiType.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, cmbShinzaiSize.Name, cmbShinzaiSize.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, txtShinzaiTop.Name, txtShinzaiTop.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, txtShinzaiTotalLength.Name, txtShinzaiTotalLength.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, txtPitch.Name, txtPitch.Text, iniPath);
            
            ClsIni.WritePrivateProfileString(sec, chkKougoHaichi.Name, chkKougoHaichi.Checked.ToString(), iniPath);

            ClsIni.WritePrivateProfileString(sec, txtKashoNum.Name, txtKashoNum.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnKoteiTypeBolt.Name, rbnKoteiTypeBolt.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnKoteiTypeYosetsu.Name, rbnKoteiTypeYosetsu.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, txtKashoNum2.Name, txtKashoNum2.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnKoteiTypeBolt2.Name, rbnKoteiTypeBolt2.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnKoteiTypeYosetsu2.Name, rbnKoteiTypeYosetsu2.Checked.ToString(), iniPath);
        }

        /// <summary>
        /// iniデータをダイアログにセット
        /// </summary>
        public void GetIniData()
        {
            string iniPath = ClsIni.GetIniFilePath(IniPath);
            //txtCase.Text = ClsIni.GetIniFile(sec, txtCase.Name, iniPath);
            //txtEdaNumber.Text = ClsIni.GetIniFile(sec, txtEdaNumber.Name, iniPath);

            cmbShoriType.Text = ClsIni.GetIniFile(sec, cmbShoriType.Name, iniPath);
            txtKussakuFukasa.Text = ClsIni.GetIniFile(sec, txtKussakuFukasa.Name, iniPath);
            txtKijuntenKyori.Text = ClsIni.GetIniFile(sec, txtKijuntenKyori.Name, iniPath);
            cmbZanchi.Text = ClsIni.GetIniFile(sec, cmbZanchi.Name, iniPath);
            txtZantiLength.Text = ClsIni.GetIniFile(sec, txtZantiLength.Name, iniPath);
            txtRenzokuWallThickness.Text = ClsIni.GetIniFile(sec, txtRenzokuWallThickness.Name, iniPath);
            txtWallLength.Text = ClsIni.GetIniFile(sec, txtWallLength.Name, iniPath);
            chkVoid.Checked = bool.Parse(ClsIni.GetIniFile(sec, chkVoid.Name, iniPath));

            cmbShinzaiType.Text = ClsIni.GetIniFile(sec, cmbShinzaiType.Name, iniPath);
            cmbShinzaiSize.Text = ClsIni.GetIniFile(sec, cmbShinzaiSize.Name, iniPath);
            txtShinzaiTop.Text = ClsIni.GetIniFile(sec, txtShinzaiTop.Name, iniPath);
            txtShinzaiTotalLength.Text = ClsIni.GetIniFile(sec, txtShinzaiTotalLength.Name, iniPath);
            txtPitch.Text = ClsIni.GetIniFile(sec, txtPitch.Name, iniPath);

            chkKougoHaichi.Checked = bool.Parse(ClsIni.GetIniFile(sec, chkKougoHaichi.Name, iniPath));

            txtKashoNum.Text = ClsIni.GetIniFile(sec, txtKashoNum.Name, iniPath);
            rbnKoteiTypeBolt.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnKoteiTypeBolt.Name, iniPath));
            rbnKoteiTypeYosetsu.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnKoteiTypeYosetsu.Name, iniPath));
            txtKashoNum2.Text = ClsIni.GetIniFile(sec, txtKashoNum2.Name, iniPath);
            rbnKoteiTypeBolt2.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnKoteiTypeBolt2.Name, iniPath));
            rbnKoteiTypeYosetsu2.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnKoteiTypeYosetsu2.Name, iniPath));
        }

        /// <summary>
        /// メンバの値をコントロールにセット
        /// </summary>
        private void SetControl()
        {
            ClsRenzokukabe clsRenzokukabe = m_ClsRenzokukabe;
            txtCase.Text = clsRenzokukabe.m_case.ToString();
            txtEdaNumber.Text = clsRenzokukabe.m_edaNum;

            //clsRenzokukabe.m_way = cmbShoriType.SelectedIndex;
            txtKussakuFukasa.Text = clsRenzokukabe.m_void.ToString();
            //clsRenzokukabe.m_refPDist = ClsCommonUtils.ChangeStrToInt(txtKijuntenKyori.Text);
            //clsRenzokukabe.m_putPitch = ClsCommonUtils.ChangeStrToInt(txtPutPitch.Text);
            cmbZanchi.Text = clsRenzokukabe.m_zanti;
            txtZantiLength.Text = clsRenzokukabe.m_zantiLength;

            cmbShinzaiType.Text = clsRenzokukabe.m_type;
            cmbShinzaiSize.Text = clsRenzokukabe.m_size;
            txtShinzaiTop.Text = clsRenzokukabe.m_HTop.ToString();
            txtShinzaiTotalLength.Text = clsRenzokukabe.m_HLen.ToString();


            txtRenzokuWallThickness.Text = clsRenzokukabe.m_kabeAtumi.ToString();
            txtWallLength.Text = clsRenzokukabe.m_kabeLen.ToString();

            //グリッドビュー
            //clsRenzokukabe.m_KougoFlg = chkKougoHaichi.Checked;
            txtKashoNum.Text = clsRenzokukabe.m_Kasho1.ToString();
            //clsRenzokukabe.m_Kasho2 = ClsCommonUtils.ChangeStrToInt(txtKashoNum2.Text);

            List<int> lstN1 = clsRenzokukabe.m_ListPileLength1;
            for (int i = 0; i < lstN1.Count; i++)
            {
                dgvPileLength[i, 0].Value = lstN1[i].ToString();
            }
            //List<int> lstN2 = new List<int>();
            //for (int i = 0; i < dgvPileLength2.Columns.Count; i++)
            //{
            //    lstN2.Add(ClsCommonUtils.ChangeStrToInt(dgvPileLength2[i, 0].Value.ToString()));
            //}
            //clsRenzokukabe.m_ListPileLength2 = lstN2;
        }

        private void cmbZanchi_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbZanchi.SelectedItem.ToString() == "一部埋め殺し")
            {
                lblZantiLength.Enabled = true;
                txtZantiLength.Enabled = true;
            }
            else
            {
                lblZantiLength.Enabled = false;
                txtZantiLength.Enabled = false;
            }
        }

        private void txtCase_TextChanged(object sender, EventArgs e)
        {
            ClsCASE clsCASE = m_caseInfo;
            string nextCASE = string.Empty;
            enCASEHit enCASEHit = clsCASE.ExistCase(enKabeshu.renzokukabe, txtCase.Text, ref nextCASE);
            if (enCASEHit == enCASEHit.sameKabeshu)
            {
                foreach (stCASERenzokuKabe renzokukabe in clsCASE.m_kabeDataList)
                {
                    if (renzokukabe.CASE == txtCase.Text)
                    {
                        cmbShinzaiType.Text = renzokukabe.buzaiType;
                        cmbShinzaiSize.Text = renzokukabe.buzaiSize;
                        txtShinzaiTotalLength.Text = renzokukabe.TotalLength.ToString();
                        txtRenzokuWallThickness.Text = renzokukabe.kabeThickness.ToString();
                        txtWallLength.Text = renzokukabe.kabeLength.ToString();
                        cmbZanchi.Text = renzokukabe.zanchi;
                        txtZantiLength.Text = renzokukabe.zanchiLength;
                        break;
                    }
                }
                SetCASEControl(false);
            }
            else if (enCASEHit == enCASEHit.noHit)
                SetCASEControl(true);
        }

        private void txtEdaNumber_TextChanged(object sender, EventArgs e)
        {
            ClsCASE clsCASE = m_caseInfo;
            string nextCASE = string.Empty;
            string nextEdaban = string.Empty;
            enCASE_EdabanHit enCASE_EdabanHit = clsCASE.ExistCaseEdaban(enKabeshu.renzokukabe, txtCase.Text, txtEdaNumber.Text, ref nextCASE, ref nextEdaban);
            if (enCASE_EdabanHit == enCASE_EdabanHit.sameKabeshu)
            {
                foreach (stCASERenzokuKabe renzokukabe in clsCASE.m_kabeDataList)
                {
                    if (renzokukabe.CASE == txtCase.Text && renzokukabe.Edaban == txtEdaNumber.Text)
                    {
                        txtKashoNum.Text = renzokukabe.JointNum.ToString();
                        if (dgvPileLength.Columns.Count == renzokukabe.KuiLengthList.Count && 0 < dgvPileLength.Rows.Count)
                        {
                            for (int i = 0; i < dgvPileLength.Columns.Count; i++)
                            {
                                dgvPileLength[i, 0].Value = renzokukabe.KuiLengthList[i];
                            }
                        }
                        break;
                    }
                }
                SetEdaNum(false);
            }
            else if (enCASE_EdabanHit == enCASE_EdabanHit.noHit)
                SetEdaNum(true);

            tabPage1.Text = txtEdaNumber.Text;
            SetNextEdaNum();
        }

        private void SetCASEControl(bool set)
        {
            cmbShinzaiType.Enabled = set;
            cmbShinzaiSize.Enabled = set;
            txtShinzaiTotalLength.Enabled = set;
            txtRenzokuWallThickness.Enabled = set;
            txtWallLength.Enabled = set;
            cmbZanchi.Enabled = set;
            txtZantiLength.Enabled = set;
        }

        private void SetEdaNum(bool set)
        {
            txtKashoNum.Enabled = set;
            //dgvPileLength.Enabled = set;
            SetColorAndReadonyRow(dgvPileLength, set);
            SetColorAndReadonyRow(dgvPileLength2, set);
        }
        /// <summary>
        /// DGVの行をReadonyに変更
        /// </summary>
        private void SetColorAndReadonyRow(DataGridView dgv, bool set)
        {
            if (dgv.Rows.Count < 0)
                return;

            set = set == false ? true : false;
            DataGridViewRow row = dgv.Rows[0];
            foreach (DataGridViewCell cell in row.Cells)
            {
                if (set)
                    cell.Style.BackColor = ClsGlobal.m_GrayColor;
                else
                    cell.Style.BackColor = ClsGlobal.m_WhiteColor;
                cell.ReadOnly = set;
            }
        }

        private void SetNextEdaNum()
        {
            if (chkKougoHaichi.Checked && !string.IsNullOrWhiteSpace(txtEdaNumber.Text))
            {
                ClsCASE clsCASE = m_caseInfo;
                //ダイアログに入力された枝番の次のアルファベット
                int numeric = ClsCommonUtils.ConvertToNumeric(txtEdaNumber.Text);
                numeric += 1;
                string edaban = ClsCommonUtils.ConvertToAlphabet(numeric);

                string nextCASE = string.Empty;
                string nextEdaban = string.Empty;
                enCASE_EdabanHit enCASE_EdabanHit = clsCASE.ExistCaseEdaban(enKabeshu.renzokukabe, txtCase.Text, edaban, ref nextCASE, ref nextEdaban);
                bool use = true;
                if (enCASE_EdabanHit == enCASE_EdabanHit.sameKabeshu)
                {
                    foreach (stCASERenzokuKabe renzokukabe in clsCASE.m_kabeDataList)
                    {
                        if (renzokukabe.CASE == txtCase.Text && renzokukabe.Edaban == edaban)
                        {
                            if (txtKashoNum.Text == renzokukabe.JointNum.ToString())
                            {
                                if (dgvPileLength.Columns.Count == renzokukabe.KuiLengthList.Count && 0 < dgvPileLength.Rows.Count)
                                {
                                    for (int i = 0; i < dgvPileLength.Columns.Count; i++)
                                    {
                                        if ((int)dgvPileLength[i, 0].Value != renzokukabe.KuiLengthList[renzokukabe.KuiLengthList.Count - 1 - i])
                                        {
                                            use = false;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (use)
                            {
                                tabPage2.Text = edaban;
                                txtKashoNum2.Text = renzokukabe.JointNum.ToString();
                                if (dgvPileLength2.Columns.Count == renzokukabe.KuiLengthList.Count && 0 < dgvPileLength2.Rows.Count)
                                {
                                    for (int i = 0; i < dgvPileLength.Columns.Count; i++)
                                    {
                                        dgvPileLength2[i, 0].Value = renzokukabe.KuiLengthList[i];
                                    }
                                }
                            }
                            else
                            {
                                string nextEdaNum = clsCASE.GetEdabanAlphabet(enKabeshu.smw, txtCase.Text, txtEdaNumber.Text);
                                tabPage2.Text = nextEdaNum;
                                if (chkKougoHaichi.Checked)
                                {
                                    SetKougoHaichi();
                                }
                            }
                            break;
                        }
                    }
                }
                else if (enCASE_EdabanHit == enCASE_EdabanHit.noHit)
                {
                    string nextEdaNum = clsCASE.GetEdabanAlphabet(enKabeshu.renzokukabe, txtCase.Text, txtEdaNumber.Text);
                    tabPage2.Text = nextEdaNum;
                }
            }
        }
    }
}
