using RevitUtil;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using YMS.Parts;

namespace YMS.DLG
{
    public partial class DlgCreateKiribariHiuchiBase : Form
    {
        #region 定数
        /// <summary>
        /// Iniファイルのパス
        /// </summary>
        const string IniPath = "ini\\DlgCreateKiribariHiuchiBase.ini";

        /// <summary>
        /// Iniファイルのセクション名
        /// </summary>
        const string sec = "DlgCreateKiribariHiuchiBase";
        #endregion

        #region メンバー
        /// <summary>
        /// 切梁火打ベースクラス
        /// </summary>
        public ClsKiribariHiuchiBase m_KiribariHiuchiBase;
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="kiriHiuchiBase"></param>
        public DlgCreateKiribariHiuchiBase(ClsKiribariHiuchiBase kiriHiuchiBase)
        {
            InitializeComponent();
            m_KiribariHiuchiBase = kiriHiuchiBase;
            InitControl();
            SetControl();
            groupBox1.Enabled = false;
            chkSingleDouble();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="kiriHiuchiBase"></param>
        public DlgCreateKiribariHiuchiBase()
        {
            InitializeComponent();
            m_KiribariHiuchiBase = new ClsKiribariHiuchiBase();
            InitControl();
            GetIniData();
            groupBox1.Enabled = true;
            chkSingleDouble();
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
                if (rbnHaichiKiriKiri.Checked)
                    return Master.ClsHiuchiCsv.GetTypeListKiribariHiuchiJouge();
                else if (rbnHaichiKiriHara.Checked)
                    return Master.ClsHiuchiCsv.GetTypeListKiribariHiuchiKiriSide();
                else
                    return Master.ClsHiuchiCsv.GetTypeListKiribariHiuchiSanjikuPieceSide();
            });
            initComboBoxSub(cmbBuhinType2, () =>
            {
                if (rbnHaichiKiriKiri.Checked)
                    return Master.ClsHiuchiCsv.GetTypeListKiribariHiuchiJouge();
                else if (rbnHaichiKiriHara.Checked)
                    return Master.ClsHiuchiCsv.GetTypeListKiribariHiuchiHaraSide();
                else
                    return Master.ClsHiuchiCsv.GetTypeListKiribariHiuchiHaraSide();
            });

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
            initComboBoxSizeSub(cmbSteelSizeSingle, cmbSteelType.Text);
            initComboBoxSizeSub(cmbSteelSizeDoubleUpper, cmbSteelType.Text);
            initComboBoxSizeSub(cmbSteelSizeDoubleUnder, cmbSteelType.Text);
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

            List<string> lstStr = null;

            if (rbnHaichiKiriKiri.Checked)
            {
                lstStr = Master.ClsHiuchiCsv.GetSizeListKiribariHiuchiJouge(selectedBuhinType);
            }
            else if (rbnHaichiKiriHara.Checked && comboBox == cmbBuhinSize1)
            {
                lstStr = Master.ClsHiuchiCsv.GetSizeListKiribariHiuchiKiriSide(selectedBuhinType);
            }
            else if (comboBox == cmbBuhinSize2)
            {
                lstStr = Master.ClsHiuchiCsv.GetSizeListKiribariHiuchiHaraSide(selectedBuhinType);
            }

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
            ClsKiribariHiuchiBase clsKHB = m_KiribariHiuchiBase;

            if (rbnHaichiKiriKiri.Checked)
                clsKHB.m_ShoriType = ClsKiribariHiuchiBase.ShoriType.KiriKiri;
            else if (rbnHaichiKiriHara.Checked)
                clsKHB.m_ShoriType = ClsKiribariHiuchiBase.ShoriType.KiriHara;
            else
                clsKHB.m_ShoriType = ClsKiribariHiuchiBase.ShoriType.SanjikuHara;

            if (rbnHaichiShoriHohoAuto.Checked)
                clsKHB.m_ShoriHoho = ClsKiribariHiuchiBase.ShoriHoho.Auto;
            else if (rbnHaichiShoriHohoManual.Checked)
                clsKHB.m_ShoriHoho = ClsKiribariHiuchiBase.ShoriHoho.Manual;
            else
                clsKHB.m_ShoriHoho = ClsKiribariHiuchiBase.ShoriHoho.Replace;

            if (rbnHaichiCreateHohoBoth.Checked)
                clsKHB.m_CreateType = ClsKiribariHiuchiBase.CreateType.Both;
            else if (rbnHaichiCreateHohoAnyLengthManual.Checked)
                clsKHB.m_CreateType = ClsKiribariHiuchiBase.CreateType.AnyLengthManual;
            else
                clsKHB.m_CreateType = ClsKiribariHiuchiBase.CreateType.AnyLengthAuto;

            if (rbnCreateHouhouSingle.Checked)
                clsKHB.m_CreateHoho = ClsKiribariHiuchiBase.CreateHoho.Single;
            else
                clsKHB.m_CreateHoho = ClsKiribariHiuchiBase.CreateHoho.Double;

            clsKHB.m_SteelType = cmbSteelType.Text;
            clsKHB.m_SteelSizeSingle = cmbSteelSizeSingle.Text;
            clsKHB.m_SteelSizeDoubleUpper = cmbSteelSizeDoubleUpper.Text;
            clsKHB.m_SteelSizeDoubleUnder = cmbSteelSizeDoubleUnder.Text;
            clsKHB.m_HiuchiLengthSingleL = int.Parse(txtHiuchiLengthSingleL.Text);
            clsKHB.m_HiuchiLengthDoubleUpperL1 = int.Parse(txtHiuchiLengthDoubleL1.Text);
            clsKHB.m_HiuchiLengthDoubleUnderL2 = int.Parse(txtHiuchiLengthDoubleL2.Text);
            clsKHB.m_HiuchiZureRyo = int.Parse(txtHiuchiZureryo.Text);

            clsKHB.m_PartsTypeKiriSide = cmbBuhinType1.Text;
            clsKHB.m_PartsSizeKiriSide = cmbBuhinSize1.Text;
            clsKHB.m_PartsTypeHaraSide = cmbBuhinType2.Text;
            clsKHB.m_PartsSizeHaraSide = cmbBuhinSize2.Text;

            clsKHB.m_Angle = double.Parse(txtAngle.Text);
        }

        /// <summary>
        /// メンバーの値をコントロールにセット
        /// </summary>
        private void SetControl()
        {
            ClsKiribariHiuchiBase clsKHB = m_KiribariHiuchiBase;

            if (clsKHB.m_ShoriType == ClsKiribariHiuchiBase.ShoriType.KiriKiri)
                rbnHaichiKiriKiri.Checked = true;
            else if (clsKHB.m_ShoriType == ClsKiribariHiuchiBase.ShoriType.KiriHara)
                rbnHaichiKiriHara.Checked = true;
            else
                rbnHaichiSanjikuHara.Checked = true;

            if (rbnHaichiShoriHohoAuto.Checked)
                clsKHB.m_ShoriHoho = ClsKiribariHiuchiBase.ShoriHoho.Auto;
            else if (rbnHaichiShoriHohoManual.Checked)
                clsKHB.m_ShoriHoho = ClsKiribariHiuchiBase.ShoriHoho.Manual;
            else
                clsKHB.m_ShoriHoho = ClsKiribariHiuchiBase.ShoriHoho.Replace;

            if (rbnHaichiCreateHohoBoth.Checked)
                clsKHB.m_CreateType = ClsKiribariHiuchiBase.CreateType.Both;
            else if (rbnHaichiCreateHohoAnyLengthManual.Checked)
                clsKHB.m_CreateType = ClsKiribariHiuchiBase.CreateType.AnyLengthManual;
            else
                clsKHB.m_CreateType = ClsKiribariHiuchiBase.CreateType.AnyLengthAuto;

            //if (rbnCreateHouhouSingle.Checked)
            //    clsKHB.m_CreateHoho = ClsKiribariHiuchiBase.CreateHoho.Single;
            //else
            //    clsKHB.m_CreateHoho = ClsKiribariHiuchiBase.CreateHoho.Double;

            if (clsKHB.m_CreateHoho == ClsKiribariHiuchiBase.CreateHoho.Single)
                rbnCreateHouhouSingle.Checked = true;
            else
                rbnCreateHouhouDouble.Checked = true;

            //clsKHB.m_SteelSizeSingle = cmbSteelSizeSingle.Text;
            //clsKHB.m_SteelSizeDoubleUpper = cmbSteelSizeDoubleUpper.Text;
            //clsKHB.m_SteelSizeDoubleUnder = cmbSteelSizeDoubleUnder.Text;
            //clsKHB.m_HiuchiLengthSingleL = int.Parse(txtHiuchiLengthSingleL.Text);
            //clsKHB.m_HiuchiLengthDoubleUpperL1 = int.Parse(txtHiuchiLengthDoubleL1.Text);
            //clsKHB.m_HiuchiLengthDoubleUnderL2 = int.Parse(txtHiuchiLengthDoubleL2.Text);
            //clsKHB.m_HiuchiZureRyo = int.Parse(txtHiuchiZureryo.Text);

            //clsKHB.m_PartsTypeKiriSide = cmbBuhinType1.Text;
            //clsKHB.m_PartsSizeKiriSide = cmbBuhinSize1.Text;
            //clsKHB.m_PartsTypeHaraSide = cmbBuhinType2.Text;
            //clsKHB.m_PartsSizeHaraSide = cmbBuhinSize2.Text;

            //clsKHB.m_Angle = double.Parse(txtAngle.Text);
            cmbSteelType.Text = clsKHB.m_SteelType;

            cmbSteelSizeSingle.Text = clsKHB.m_SteelSizeSingle;
            cmbSteelSizeDoubleUpper.Text = clsKHB.m_SteelSizeDoubleUpper;
            cmbSteelSizeDoubleUnder.Text = clsKHB.m_SteelSizeDoubleUnder;
            txtHiuchiLengthSingleL.Text = clsKHB.m_HiuchiLengthSingleL.ToString();
            txtHiuchiLengthDoubleL1.Text = clsKHB.m_HiuchiLengthDoubleUpperL1.ToString();
            txtHiuchiLengthDoubleL2.Text = clsKHB.m_HiuchiLengthDoubleUnderL2.ToString();
            txtHiuchiZureryo.Text = clsKHB.m_HiuchiZureRyo.ToString();

            cmbBuhinType1.Text = clsKHB.m_PartsTypeKiriSide;
            cmbBuhinSize1.Text = clsKHB.m_PartsSizeKiriSide;
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
                rbnHaichiKiriKiri.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnHaichiKiriKiri.Name, iniPath));
                rbnHaichiKiriHara.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnHaichiKiriHara.Name, iniPath));
                rbnHaichiSanjikuHara.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnHaichiSanjikuHara.Name, iniPath));
                rbnHaichiShoriHohoAuto.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnHaichiShoriHohoAuto.Name, iniPath));
                rbnHaichiShoriHohoManual.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnHaichiShoriHohoManual.Name, iniPath));
                rbnHaichiShoriHohoReplace.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnHaichiShoriHohoReplace.Name, iniPath));

                rbnHaichiCreateHohoBoth.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnHaichiCreateHohoBoth.Name, iniPath));
                rbnHaichiCreateHohoAnyLengthManual.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnHaichiCreateHohoAnyLengthManual.Name, iniPath));
                rbnHaichiCreateHohoAnyLengthAuto.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnHaichiCreateHohoAnyLengthAuto.Name, iniPath));

                rbnCreateHouhouSingle.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnCreateHouhouSingle.Name, iniPath));
                rbnCreateHouhouDouble.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnCreateHouhouDouble.Name, iniPath));

                cmbSteelSizeSingle.Text = ClsIni.GetIniFile(sec, cmbSteelSizeSingle.Name, iniPath);
                cmbSteelSizeDoubleUpper.Text = ClsIni.GetIniFile(sec, cmbSteelSizeDoubleUpper.Name, iniPath);
                cmbSteelSizeDoubleUnder.Text = ClsIni.GetIniFile(sec, cmbSteelSizeDoubleUnder.Name, iniPath);
                txtHiuchiLengthSingleL.Text = ClsIni.GetIniFile(sec, txtHiuchiLengthSingleL.Name, iniPath);
                txtHiuchiLengthDoubleL1.Text = ClsIni.GetIniFile(sec, txtHiuchiLengthDoubleL1.Name, iniPath);
                txtHiuchiLengthDoubleL2.Text = ClsIni.GetIniFile(sec, txtHiuchiLengthDoubleL2.Name, iniPath);
                txtHiuchiZureryo.Text = ClsIni.GetIniFile(sec, txtHiuchiZureryo.Name, iniPath);

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
            ClsIni.WritePrivateProfileString(sec, rbnHaichiKiriKiri.Name, rbnHaichiKiriKiri.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnHaichiKiriHara.Name, rbnHaichiKiriHara.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnHaichiSanjikuHara.Name, rbnHaichiSanjikuHara.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnHaichiShoriHohoAuto.Name, rbnHaichiShoriHohoAuto.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnHaichiShoriHohoManual.Name, rbnHaichiShoriHohoManual.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnHaichiShoriHohoReplace.Name, rbnHaichiShoriHohoReplace.Checked.ToString(), iniPath);

            ClsIni.WritePrivateProfileString(sec, rbnHaichiCreateHohoBoth.Name, rbnHaichiCreateHohoBoth.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnHaichiCreateHohoAnyLengthManual.Name, rbnHaichiCreateHohoAnyLengthManual.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnHaichiCreateHohoAnyLengthAuto.Name, rbnHaichiCreateHohoAnyLengthAuto.Checked.ToString(), iniPath);

             ClsIni.WritePrivateProfileString(sec, rbnCreateHouhouSingle.Name, rbnCreateHouhouSingle.Checked.ToString(), iniPath);
             ClsIni.WritePrivateProfileString(sec, rbnCreateHouhouDouble.Name, rbnCreateHouhouDouble.Checked .ToString(),iniPath);

             ClsIni.WritePrivateProfileString(sec, cmbSteelSizeSingle.Name, cmbSteelSizeSingle.Text, iniPath);
             ClsIni.WritePrivateProfileString(sec, cmbSteelSizeDoubleUpper.Name, cmbSteelSizeDoubleUpper.Text, iniPath);
             ClsIni.WritePrivateProfileString(sec, cmbSteelSizeDoubleUnder.Name, cmbSteelSizeDoubleUnder.Text, iniPath);
             ClsIni.WritePrivateProfileString(sec, txtHiuchiLengthSingleL.Name, txtHiuchiLengthSingleL.Text, iniPath);
             ClsIni.WritePrivateProfileString(sec, txtHiuchiLengthDoubleL1.Name, txtHiuchiLengthDoubleL1.Text, iniPath);
             ClsIni.WritePrivateProfileString(sec, txtHiuchiLengthDoubleL2.Name, txtHiuchiLengthDoubleL2.Text,iniPath);
             ClsIni.WritePrivateProfileString(sec, txtHiuchiZureryo.Name, txtHiuchiZureryo.Text, iniPath);

             ClsIni.WritePrivateProfileString(sec, cmbBuhinType1.Name, cmbBuhinType1.Text, iniPath);
             ClsIni.WritePrivateProfileString(sec, cmbBuhinSize1.Name, cmbBuhinSize1.Text, iniPath);
             ClsIni.WritePrivateProfileString(sec, cmbBuhinType2.Name, cmbBuhinType2.Text,iniPath);
             ClsIni.WritePrivateProfileString(sec, cmbBuhinSize2.Name, cmbBuhinSize2.Text,iniPath);
             ClsIni.WritePrivateProfileString(sec, txtAngle.Name, txtAngle.Text,iniPath);
        }

        private void rbnHaichiCreateHohoBoth_CheckedChanged(object sender, EventArgs e)
        {
            chkHiuchiNagasa();
        }

        private void rbnHaichiCreateHohoAnyLengthManual_CheckedChanged(object sender, EventArgs e)
        {
            chkHiuchiNagasa();
        }

        private void rbnHaichiCreateHohoAnyLengthAuto_CheckedChanged(object sender, EventArgs e)
        {
            chkHiuchiNagasa();
        }

        private void rbnHaichiKiriKiri_CheckedChanged(object sender, EventArgs e)
        {
            chkSyoriType();
            chkCreateWay();
            chkSyoriHouhou();
            initComboBox();
        }

        private void rbnHaichiKiriHara_CheckedChanged(object sender, EventArgs e)
        {
            chkSyoriType();
            chkCreateWay();
            chkSyoriHouhou();
            initComboBox();
        }

        private void rbnHaichiSanjikuHara_CheckedChanged(object sender, EventArgs e)
        {
            chkSyoriType();
            chkCreateWay();
            chkSyoriHouhou();
            initComboBox();
        }

        private void rbnCreateHohoSingle_CheckedChanged(object sender, EventArgs e)
        {
            chkSingleDouble();
        }

        private void rbnCreateHohoDouble_CheckedChanged(object sender, EventArgs e)
        {
            chkSingleDouble();
        }

        private void rbnHaichiShoriHohoAuto_CheckedChanged(object sender, EventArgs e)
        {
            chkCreateWay();
        }

        private void rbnHaichiShoriHohoManual_CheckedChanged(object sender, EventArgs e)
        {
            chkCreateWay();
        }

        private void rbnHaichiShoriHohoReplace_CheckedChanged(object sender, EventArgs e)
        {
            chkCreateWay();
        }

        private void chkSyoriType()
        {
            if (rbnHaichiKiriKiri.Checked == true)
            {
                lblBuhinType1.Text = "部品タイプ(切梁上段)";
                lblBuhinSize1.Text = "部品サイズ(切梁上段)";
                lblBuhinType2.Text = "部品タイプ(切梁下段)";
                lblBuhinSize2.Text = "部品サイズ(切梁下段)";
            }
            else if(rbnHaichiKiriHara.Checked == true)
            {
                lblBuhinType1.Text = "部品タイプ(切梁側)";
                lblBuhinSize1.Text = "部品サイズ(切梁側)";
                lblBuhinType2.Text = "部品タイプ(腹起側)";
                lblBuhinSize2.Text = "部品サイズ(腹起側)";
            }
            else if (rbnHaichiSanjikuHara.Checked == true)
            {
                lblBuhinType1.Text = "部品タイプ(三軸ピース側)";
                lblBuhinSize1.Text = "部品サイズ(三軸ピース側)";
                lblBuhinType2.Text = "部品タイプ(腹起側)";
                lblBuhinSize2.Text = "部品サイズ(腹起側)";
            }
        }

        private void chkSyoriHouhou()
        {
            if (rbnHaichiKiriHara.Checked == true)
            {
                pnlSyoriHouhou.Enabled = true;
            }
            else
            {
                pnlSyoriHouhou.Enabled = false;
            }
        }

        private void chkCreateWay()
        {
            if (rbnHaichiKiriHara.Checked == true)
            {
                if(rbnHaichiShoriHohoAuto.Checked == true)
                {
                    rbnHaichiCreateHohoBoth.Enabled = true;
                    rbnHaichiCreateHohoAnyLengthAuto.Enabled = false;
                    if(rbnHaichiCreateHohoAnyLengthAuto.Checked == true)
                    {
                        rbnHaichiCreateHohoAnyLengthManual.Checked = true;
                    }
                }
                else
                {
                    rbnHaichiCreateHohoAnyLengthAuto.Enabled = true;
                    rbnHaichiCreateHohoBoth.Enabled = true;
                }
            }
            else if(rbnHaichiKiriKiri.Checked == true)
            {
                rbnHaichiCreateHohoAnyLengthAuto.Enabled = false;
                rbnHaichiCreateHohoBoth.Enabled = false;
                rbnHaichiCreateHohoAnyLengthManual.Checked = true;
            }
            else
            {
                rbnHaichiCreateHohoAnyLengthAuto.Enabled = true;
                rbnHaichiCreateHohoBoth.Enabled = true;
            }
        }

        private void chkHiuchiNagasa()
        {
            if(rbnHaichiCreateHohoAnyLengthAuto.Checked== true)
            {
                txtHiuchiLengthSingleL.Enabled = false;
                txtHiuchiLengthDoubleL1.Enabled = false;
                txtHiuchiLengthDoubleL2.Enabled = false;
                txtHiuchiZureryo.Enabled = false;
            }
            else
            {
                txtHiuchiLengthSingleL.Enabled = true;
                txtHiuchiLengthDoubleL1.Enabled = true;
                txtHiuchiLengthDoubleL2.Enabled = true;
                txtHiuchiZureryo.Enabled = true;
            }
        }

        private void chkSingleDouble()
        {
            if(rbnCreateHouhouSingle.Checked== true)
            {
                cmbSteelSizeSingle.Enabled = true;
                txtHiuchiLengthSingleL.Enabled=true;

                cmbSteelSizeDoubleUpper.Enabled = false;
                cmbSteelSizeDoubleUnder.Enabled= false;
                txtHiuchiLengthDoubleL1.Enabled=false;
                txtHiuchiLengthDoubleL2.Enabled=false;
                txtHiuchiZureryo.Enabled=false;
            }
            else if (rbnCreateHouhouDouble.Checked == true)
            {
                cmbSteelSizeSingle.Enabled = false;
                txtHiuchiLengthSingleL.Enabled = false;

                cmbSteelSizeDoubleUpper.Enabled = true;
                cmbSteelSizeDoubleUnder.Enabled = true;
                txtHiuchiLengthDoubleL1.Enabled = true;
                txtHiuchiLengthDoubleL2.Enabled = true;
                txtHiuchiZureryo.Enabled = true;
            }
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
            if (!ClsCommonUtils.ChkContorolTextDouble(txtHiuchiLengthSingleL, "火打長さ(シングル)L", bMinus: true))
                return false;
            if (!ClsCommonUtils.ChkContorolTextDouble(txtHiuchiLengthDoubleL1, "火打長さ(ダブル上)L1", bMinus: true))
                return false;
            if (!ClsCommonUtils.ChkContorolTextDouble(txtHiuchiLengthDoubleL2, "火打長さ(ダブル下)L2", bMinus: true))
                return false;
            if (!ClsCommonUtils.ChkContorolTextDouble(txtHiuchiZureryo, "上下火打ずれ量a"))
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
            else if (cmbBuhinType1.Text == "なし")
            {
                cmbBuhinSize1.Enabled = false;
                txtAngle.Text = "";
                txtAngle.Enabled = true;
            }
            else
            {
                txtAngle.Enabled = false;
            }

            if (rbnHaichiSanjikuHara.Checked == true)
            {
                txtAngle.Enabled = false;
                txtAngle.Text = "30.00";
            }
        }

    }
}
