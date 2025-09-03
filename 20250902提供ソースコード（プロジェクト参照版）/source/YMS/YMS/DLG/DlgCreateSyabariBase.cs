using RevitUtil;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using YMS.Parts;

namespace YMS.DLG
{
    public partial class DlgCreateSyabariBase : Form
    {
        #region 定数

        /// <summary>
        /// Iniファイルのパス
        /// </summary>
        const string IniPath = "ini\\DlgCreateSyabariBase.ini";

        /// <summary>
        /// Iniファイルのセクション名
        /// </summary>
        const string sec = "DlgCreateSyabariBase";

        #endregion


        #region メンバー

        public ClsSyabariBase m_ClsSyabariBase;

        #endregion

        public DlgCreateSyabariBase(ClsSyabariBase clsSyabariBase)
        {
            InitializeComponent();
            InitControl();
            m_ClsSyabariBase = clsSyabariBase;

            SetControl();
            //オフセットは変更の対象ではない
            txtEndOffset1.Enabled = false;
        }
        public DlgCreateSyabariBase()
        {
            InitializeComponent();
            InitControl();
            m_ClsSyabariBase = new ClsSyabariBase();
            GetIniData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!CheckInput())
                return;

            SetData();
            SetIniData();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void DlgCreateSyabari_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// コントロールの値をメンバクラスに保持
        /// </summary>
        private void InitControl()
        {
            initComboBox();
        }

        public void initComboBox()
        {
            string bk = string.Empty;
            List<string> lstStr = new List<string>();

            //鋼材タイプ
            bk = cmbSteelType.Text;
            cmbSteelType.Items.Clear();
            lstStr = Master.ClsYamadomeCsv.GetTypeList(bHigh: true, bMega: false, bTwin: true);
            foreach (string str in lstStr)
            {
                cmbSteelType.Items.Add(str);
            }
            if (lstStr.Contains(bk)) cmbSteelType.Text = bk;
            else cmbSteelType.SelectedIndex = 0;

            //部品タイプ(始点側) & 部品タイプ(終点側)
            initComboBoxType();

            //ジャッキタイプ1
            bk = cmbJackType1.Text;
            cmbJackType1.Items.Clear();
            lstStr = Master.ClsJackCsv.GetTypeList();
            foreach (string str in lstStr)
            {
                cmbJackType1.Items.Add(str);
            }
            if (lstStr.Contains(bk)) cmbJackType1.Text = bk;
            else cmbJackType1.SelectedIndex = 0;

            //ジャッキタイプ2
            bk = cmbJackType2.Text;
            cmbJackType2.Items.Clear();
            lstStr = Master.ClsJackCsv.GetTypeList();
            foreach (string str in lstStr)
            {
                cmbJackType2.Items.Add(str);
            }
            if (lstStr.Contains(bk)) cmbJackType2.Text = bk;
            else cmbJackType2.SelectedIndex = 0;
        }

        public void initComboBoxType()
        {
            initComboBoxTypeSub(cmbBuhinTypeS, () => Master.ClsSyabariPieceCSV.GetTypeListSyabari());
            initComboBoxTypeSub(cmbBuhinTypeE, () => Master.ClsSyabariPieceCSV.GetTypeListSyabari());
        }
        private void initComboBoxTypeSub(ComboBox comboBox, Func<List<string>> getListFunc)
        {
            if (comboBox == null || getListFunc == null)
            {
                return;
            }

            string selectedText = comboBox.Text;
            comboBox.Items.Clear();

            List<string> lstStr = getListFunc.Invoke();

            if (lstStr != null)
            {
                comboBox.Items.AddRange(lstStr.ToArray());

                if (lstStr.Contains(selectedText))
                {
                    comboBox.Text = selectedText;
                }
                else if (comboBox.Items.Count > 0)
                {
                    comboBox.SelectedIndex = 0;
                }
                else
                {
                    comboBox.Text = string.Empty;
                }
            }
            else
            {
                comboBox.Text = string.Empty;
            }
        }
        public void initComboBoxSize()
        {
            initComboBoxSizeSub(cmbBuhinSizeS, cmbBuhinTypeS.Text);
            initComboBoxSizeSub(cmbBuhinSizeE, cmbBuhinTypeE.Text);
        }
        private void initComboBoxSizeSub(ComboBox comboBox, string selectedSteelType)
        {
            if (comboBox == null)
            {
                return;
            }

            string selectedText = comboBox.Text;
            comboBox.Items.Clear();

            List<string> lstStr = Master.ClsSyabariPieceCSV.GetSizeListSyabari(selectedSteelType);

            if (lstStr != null)
            {
                comboBox.Items.AddRange(lstStr.ToArray());

                if (lstStr.Contains(selectedText))
                {
                    comboBox.Text = selectedText;
                }
                else if (comboBox.Items.Count > 0)
                {
                    comboBox.SelectedIndex = 0;
                }
                else
                {
                    comboBox.Text = string.Empty;
                }
            }
            else
            {
                comboBox.Text = string.Empty;
            }

            if (string.IsNullOrWhiteSpace(comboBox.Text))
                comboBox.Enabled = false;
            else
                comboBox.Enabled = true;
        }

        public void initComboBoxSteelSize()
        {
            string bk = string.Empty;
            List<string> lstStr = new List<string>();

            //鋼材サイズ
            bk = cmbSteelSize.Text;
            cmbSteelSize.Items.Clear();
            lstStr = Master.ClsYamadomeCsv.GetSizeList(cmbSteelType.Text);
            foreach (string str in lstStr)
            {
                if (str == "20HA" || str == "25HA")
                    continue;
                cmbSteelSize.Items.Add(str);
            }
            if (lstStr.Contains(bk)) cmbSteelSize.Text = bk;
            else cmbSteelSize.SelectedIndex = 0;
        }

        /// <summary>
        /// コントロールの値をメンバクラスに保持
        /// </summary>
        private void SetData()
        {
            ClsSyabariBase clsKB = m_ClsSyabariBase;
            clsKB.m_kouzaiType = cmbSteelType.Text;
            clsKB.m_kouzaiSize = cmbSteelSize.Text;

            if (rbnShoriTypeBaseLine.Checked)
            {
                clsKB.m_ShoriType = ClsSyabariBase.ShoriType.BaseLine;
            }
            else
            {
                clsKB.m_ShoriType = ClsSyabariBase.ShoriType.PtoP;
            }

            clsKB.m_endOffset1 = ClsCommonUtils.ChangeStrToDbl(txtEndOffset1.Text);
            clsKB.m_buhinTypeStart = cmbBuhinTypeS.Text;
            clsKB.m_buhinSizeStart = cmbBuhinSizeS.Text;
            clsKB.m_buhinTypeEnd = cmbBuhinTypeE.Text;
            clsKB.m_buhinSizeEnd = cmbBuhinSizeE.Text;
            clsKB.m_jack1 = cmbJackType1.Text;
            clsKB.m_jack2 = cmbJackType2.Text;
        }

        /// <summary>
        /// コントロールの値をメンバクラスに保持
        /// </summary>
        private void SetControl()
        {
            ClsSyabariBase clsKB = m_ClsSyabariBase;
            cmbSteelType.Text = clsKB.m_kouzaiType;
            cmbSteelSize.Text = clsKB.m_kouzaiSize;

            if (clsKB.m_ShoriType == ClsSyabariBase.ShoriType.BaseLine)
            {
                rbnShoriTypeBaseLine.Checked = true;
            }
            else
            {
                rbnShoriTypePtoP.Checked = true;
            }

            txtEndOffset1.Text = clsKB.m_endOffset1.ToString();
            cmbBuhinTypeS.Text = clsKB.m_buhinTypeStart;
            cmbBuhinSizeS.Text = clsKB.m_buhinSizeStart;
            cmbBuhinTypeE.Text = clsKB.m_buhinTypeEnd;
            cmbBuhinSizeE.Text = clsKB.m_buhinSizeEnd;
            cmbJackType1.Text = clsKB.m_jack1;
            cmbJackType2.Text = clsKB.m_jack2;
        }

        /// <summary>
        /// Iniの情報をコントロールにセット
        /// </summary>
        private void GetIniData()
        {
            try
            {
                string iniPath = ClsIni.GetIniFilePath(IniPath);
                cmbSteelType.Text = ClsIni.GetIniFile(sec, cmbSteelType.Name, iniPath);
                cmbSteelSize.Text = ClsIni.GetIniFile(sec, cmbSteelSize.Name, iniPath);
                rbnShoriTypeBaseLine.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnShoriTypeBaseLine.Name, iniPath));
                rbnShoriTypePtoP.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnShoriTypePtoP.Name, iniPath));
                txtEndOffset1.Text = ClsIni.GetIniFile(sec, txtEndOffset1.Name, iniPath);
                cmbBuhinTypeS.Text = ClsIni.GetIniFile(sec, cmbBuhinTypeS.Name, iniPath);
                cmbBuhinSizeS.Text = ClsIni.GetIniFile(sec, cmbBuhinSizeS.Name, iniPath);
                cmbBuhinTypeE.Text = ClsIni.GetIniFile(sec, cmbBuhinTypeE.Name, iniPath);
                cmbBuhinSizeE.Text = ClsIni.GetIniFile(sec, cmbBuhinSizeE.Name, iniPath);
                cmbJackType1.Text = ClsIni.GetIniFile(sec, cmbJackType1.Name, iniPath);
                cmbJackType2.Text = ClsIni.GetIniFile(sec, cmbJackType2.Name, iniPath);
            }
            catch
            {
                //MessageBox.Show("前回値の取得に失敗しました");
            }
        }

        /// <summary>
        /// コントロールの情報をIniにセット
        /// </summary>
        private void SetIniData()
        {
            string iniPath = ClsIni.GetIniFilePath(IniPath);

            ClsIni.WritePrivateProfileString(sec, cmbSteelType.Name, cmbSteelType.Text.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, cmbSteelSize.Name, cmbSteelSize.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnShoriTypeBaseLine.Name, rbnShoriTypeBaseLine.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnShoriTypePtoP.Name, rbnShoriTypePtoP.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, txtEndOffset1.Name, txtEndOffset1.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, cmbBuhinTypeS.Name, cmbBuhinTypeS.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, cmbBuhinSizeS.Name, cmbBuhinSizeS.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, cmbBuhinTypeE.Name, cmbBuhinTypeE.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, cmbBuhinSizeE.Name, cmbBuhinSizeE.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, cmbJackType1.Name, cmbJackType1.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, cmbJackType2.Name, cmbJackType2.Text, iniPath);
        }

        private void cmbSteelType_SelectedIndexChanged(object sender, EventArgs e)
        {
            initComboBoxSteelSize();
        }

        private void cmbBuhinTypeS_SelectedIndexChanged(object sender, EventArgs e)
        {
            initComboBoxSizeSub(cmbBuhinSizeS, cmbBuhinTypeS.Text);
        }

        private void cmbBuhinTypeE_SelectedIndexChanged(object sender, EventArgs e)
        {
            initComboBoxSizeSub(cmbBuhinSizeE, cmbBuhinTypeE.Text);
        }

        /// <summary>
        /// 入力チェック
        /// </summary>
        /// <returns></returns>
        public bool CheckInput()
        {
            if (!ClsCommonUtils.ChkContorolTextDouble(txtEndOffset1, "終端側オフセット（－）", bMinus: true,bZero: true))
                return false;

            return true;
        }
	}
}
