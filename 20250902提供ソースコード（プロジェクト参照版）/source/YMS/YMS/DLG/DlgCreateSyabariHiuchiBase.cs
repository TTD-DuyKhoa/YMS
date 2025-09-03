using RevitUtil;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using YMS.Parts;

namespace YMS.DLG
{
    public partial class DlgCreateSyabariHiuchiBase : Form
    {
        #region 定数
        /// <summary>
        /// Iniファイルのパス
        /// </summary>
        const string IniPath = "ini\\DlgCreateSyabariHiuchiBase.ini";

        /// <summary>
        /// Iniファイルのセクション名
        /// </summary>
        const string sec = "DlgCreateSyabariHiuchiBase";
        #endregion

        #region メンバー
        /// <summary>
        /// 斜梁火打ベースクラス
        /// </summary>
        public ClsSyabariHiuchiBase m_SyabariHiuchiBase;
        private string m_SyabariTanbuSize;
        private string m_SyabariTanbuType;
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="hiuchiBase"></param>
        public DlgCreateSyabariHiuchiBase(ClsSyabariHiuchiBase hiuchiBase)
        {
            InitializeComponent();
            m_SyabariHiuchiBase = hiuchiBase ?? throw new ArgumentNullException(nameof(hiuchiBase));
            InitControl();
            SetControl();
            groupBox1.Enabled = false;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="kiriHiuchiBase"></param>
        public DlgCreateSyabariHiuchiBase(string tanbuType, string tanbuSize)
        {
            InitializeComponent();
            m_SyabariHiuchiBase = new ClsSyabariHiuchiBase();
            m_SyabariTanbuType = tanbuType;
            m_SyabariTanbuSize = tanbuSize;
            InitControl();
            GetIniData();
            groupBox1.Enabled = true;
        }

        public void InitControl()
        {
            initComboBox();
        }

        public void initComboBox()
        {
            initComboBoxSub(cmbSteelType, () => Master.ClsYamadomeCsv.GetTypeList(bHigh: true, bMega: false, bTwin: false));
            initComboBoxSub(cmbBuhinType1, () =>
            {
                var lst = Master.ClsHiuchiCsv.GetTypeListKiribariHiuchiKiriSide();
                lst.Remove("なし");
                return lst;
            });
            initComboBoxSub(cmbBuhinType2, () =>
            {
                var lst = Master.ClsSyabariPieceCSV.GetTypeListHiuchi();
                //lst.Add(Master.ClsHiuchiCsv.TypeNameFVP);
                return lst;
                }); //m_SyabariTanbuType));));));//全部表示させるためコメントアウト2025/04/07
        }
        private void initComboBoxSub(ComboBox comboBox, Func<List<string>> getListFunc)
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
                foreach (string str in lstStr)
                {
                    comboBox.Items.Add(str);
                }

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
            initComboBoxSizeSub(cmbSteelSize, cmbSteelType.Text);
        }
        private void initComboBoxSizeSub(ComboBox comboBox, string selectedSteelType)
        {
            if (comboBox == null)
            {
                return;
            }

            string selectedText = comboBox.Text;
            comboBox.Items.Clear();

            List<string> lstStr = Master.ClsYamadomeCsv.GetSizeList(selectedSteelType);

            if (lstStr != null)
            {
                foreach (string str in lstStr)
                {
                    comboBox.Items.Add(str);
                }

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

        public void initComboBoxParts1()
        {
            initComboBoxPartsSub(cmbBuhinSize1, cmbBuhinType1.Text);
        }
        public void initComboBoxParts2()
        {
            initComboBoxPartsSub(cmbBuhinSize2, cmbBuhinType2.Text);
        }
        private void initComboBoxPartsSub(ComboBox comboBox, string selectedBuhinType)
        {
            if (comboBox == null)
            {
                return;
            }

            string selectedText = comboBox.Text;
            comboBox.Items.Clear();

            List<string> lstStr = new List<string>();

            if (comboBox == cmbBuhinSize1)
                lstStr = Master.ClsHiuchiCsv.GetSizeListKiribariHiuchiKiriSide(selectedBuhinType);
            //else if (comboBox == cmbBuhinSize2 && m_SyabariTanbuType == Master.ClsSyabariPieceCSV.SyabariUkePiece)
            //    lstStr.Add(m_SyabariTanbuSize);//全部表示させるためコメントアウト2025/04/07
            //else if (comboBox == cmbBuhinSize2 && selectedBuhinType == Master.ClsHiuchiCsv.TypeNameFVP)
            //    lstStr = Master.ClsHiuchiCsv.GetSizeList(selectedBuhinType);
            else if (comboBox == cmbBuhinSize2)
                lstStr = Master.ClsSyabariPieceCSV.GetSizeListHiuchi(selectedBuhinType);

            //if (rbnHaichiKiriKiri.Checked)
            //{
            //    lstStr = Master.ClsHiuchiCsv.GetSizeListKiribariHiuchiJouge(selectedBuhinType);
            //}
            //else if (rbnHaichiKiriHara.Checked && comboBox == cmbBuhinSize1)
            //{
            //    lstStr = Master.ClsHiuchiCsv.GetSizeListKiribariHiuchiKiriSide(selectedBuhinType);
            //}
            //else if (comboBox == cmbBuhinSize2)
            //{
            //    lstStr = Master.ClsHiuchiCsv.GetSizeListKiribariHiuchiHaraSide(selectedBuhinType);
            //}

            if (lstStr != null)
            {
                foreach (string str in lstStr)
                {
                    comboBox.Items.Add(str);
                }

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

        /// <summary>
        /// OKボタン押下処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!CheckInput())
                return;

            SetData();
            SetIniData();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// キャンセルボタン押下処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// コントロールの値をメンバクラスに保持
        /// </summary>
        private void SetData()
        {
            ClsSyabariHiuchiBase clsKHB = m_SyabariHiuchiBase;

            if (rbnHaichiCreateHohoBoth.Checked)
                clsKHB.m_CreateType = ClsSyabariHiuchiBase.CreateType.Both;
            else if (rbnHaichiCreateHohoAnyLengthManual.Checked)
                clsKHB.m_CreateType = ClsSyabariHiuchiBase.CreateType.AnyLengthManual;
            else
                clsKHB.m_CreateType = ClsSyabariHiuchiBase.CreateType.AnyLengthAuto;

            clsKHB.m_SteelType = cmbSteelType.Text;
            clsKHB.m_SteelSize = cmbSteelSize.Text;
            clsKHB.m_HiuchiLengthL = int.Parse(txtHiuchiLengthL.Text);

            clsKHB.m_PartsTypeSyabariSide = cmbBuhinType1.Text;
            clsKHB.m_PartsSizeSyabariSide = cmbBuhinSize1.Text;
            clsKHB.m_PartsTypeHaraSide = cmbBuhinType2.Text;
            clsKHB.m_PartsSizeHaraSide = cmbBuhinSize2.Text;

            clsKHB.m_Angle = double.Parse(txtAngle.Text);
        }

        /// <summary>
        /// メンバーの値をコントロールにセット
        /// </summary>
        private void SetControl()
        {
            ClsSyabariHiuchiBase clsKHB = m_SyabariHiuchiBase;

            if (clsKHB.m_CreateType == ClsSyabariHiuchiBase.CreateType.Both)
                rbnHaichiCreateHohoBoth.Checked = true;
            else
                rbnHaichiCreateHohoAnyLengthManual.Checked = true;

            cmbSteelType.Text = clsKHB.m_SteelType;

            cmbSteelSize.Text = clsKHB.m_SteelSize;
            txtHiuchiLengthL.Text = clsKHB.m_HiuchiLengthL.ToString();

            cmbBuhinType1.Text = clsKHB.m_PartsTypeSyabariSide;
            cmbBuhinSize1.Text = clsKHB.m_PartsSizeSyabariSide;
            cmbBuhinType2.Text = clsKHB.m_PartsTypeHaraSide;
            cmbBuhinSize2.Text = clsKHB.m_PartsSizeHaraSide;

            txtAngle.Text = clsKHB.m_Angle.ToString();
        }

        /// <summary>
        /// Iniの情報をコントロールにセット
        /// </summary>
        private void GetIniData()
        {
            try
            {
                string iniPath = ClsIni.GetIniFilePath(IniPath);
                rbnHaichiCreateHohoBoth.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnHaichiCreateHohoBoth.Name, iniPath));
                rbnHaichiCreateHohoAnyLengthManual.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnHaichiCreateHohoAnyLengthManual.Name, iniPath));
                //rbnHaichiCreateHohoAnyLengthAuto.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnHaichiCreateHohoAnyLengthAuto.Name, iniPath));

                cmbSteelSize.Text = ClsIni.GetIniFile(sec, cmbSteelSize.Name, iniPath);
                txtHiuchiLengthL.Text = ClsIni.GetIniFile(sec, txtHiuchiLengthL.Name, iniPath);

                cmbBuhinType1.Text = ClsIni.GetIniFile(sec, cmbBuhinType1.Name, iniPath);
                cmbBuhinSize1.Text = ClsIni.GetIniFile(sec, cmbBuhinSize1.Name, iniPath);
                cmbBuhinType2.Text = ClsIni.GetIniFile(sec, cmbBuhinType2.Name, iniPath);
                cmbBuhinSize2.Text = ClsIni.GetIniFile(sec, cmbBuhinSize2.Name, iniPath);
                txtAngle.Text = ClsIni.GetIniFile(sec, txtAngle.Name, iniPath);
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
            ClsIni.WritePrivateProfileString(sec, rbnHaichiCreateHohoBoth.Name, rbnHaichiCreateHohoBoth.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnHaichiCreateHohoAnyLengthManual.Name, rbnHaichiCreateHohoAnyLengthManual.Checked.ToString(), iniPath);
            //ClsIni.WritePrivateProfileString(sec, rbnHaichiCreateHohoAnyLengthAuto.Name, rbnHaichiCreateHohoAnyLengthAuto.Checked.ToString(), iniPath);

            ClsIni.WritePrivateProfileString(sec, cmbSteelSize.Name, cmbSteelSize.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, txtHiuchiLengthL.Name, txtHiuchiLengthL.Text, iniPath);

            ClsIni.WritePrivateProfileString(sec, cmbBuhinType1.Name, cmbBuhinType1.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, cmbBuhinSize1.Name, cmbBuhinSize1.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, cmbBuhinType2.Name, cmbBuhinType2.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, cmbBuhinSize2.Name, cmbBuhinSize2.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, txtAngle.Name, txtAngle.Text, iniPath);
        }

        private void rbnHaichiCreateHohoBoth_CheckedChanged(object sender, EventArgs e)
        {
            chkHiuchiNagasa();
        }

        private void rbnHaichiCreateHohoAnyLengthManual_CheckedChanged(object sender, EventArgs e)
        {
            chkHiuchiNagasa();
        }

        //private void rbnHaichiCreateHohoAnyLengthAuto_CheckedChanged(object sender, EventArgs e)
        //{
        //    chkHiuchiNagasa();
        //}

        private void chkHiuchiNagasa()
        {
            //if(rbnHaichiCreateHohoAnyLengthAuto.Checked== true)
            //{
            //    txtHiuchiLengthL.Enabled = false;
            //}
            //else
            //{
            //    txtHiuchiLengthL.Enabled = true;
            //}
            txtHiuchiLengthL.Enabled = true;
        }

        private void cmbSteelType_SelectedIndexChanged(object sender, EventArgs e)
        {
            initComboBoxSize();
        }

        private void cmbBuhinType1_SelectedIndexChanged(object sender, EventArgs e)
        {
            initComboBoxParts1();
            ChangeAngle();
        }

        private void cmbBuhinType2_SelectedIndexChanged(object sender, EventArgs e)
        {
            initComboBoxParts2();
        }

        /// <summary>
        /// 入力チェック
        /// </summary>
        /// <returns></returns>
        public bool CheckInput()
        {
            if (!ClsCommonUtils.ChkContorolTextDouble(txtHiuchiLengthL, "火打長さL", bMinus: true))
                return false;
            if (!ClsCommonUtils.ChkContorolTextDouble(txtAngle, "角度"))
                return false;

            return true;
        }

        private void ChangeAngle()
        {
            if(cmbBuhinType1.Text.Contains("30度火打受ピース"))
            {
                cmbBuhinSize1.Enabled = true;
                txtAngle.Text = "30.00";
                txtAngle.Enabled = false;
            }
            else if (cmbBuhinType1.Text == "火打受ピース")
            {
                cmbBuhinSize1.Enabled = true;
                txtAngle.Text = "45.00";
                txtAngle.Enabled = false;
            }
            //else if (cmbBuhinType1.Text == "なし")
            //{
            //    cmbBuhinSize1.Enabled = false;
            //    txtAngle.Text = "";
            //    txtAngle.Enabled = true;
            //}
            else
            {
                txtAngle.Enabled = false;
            }
        }

    }
}
