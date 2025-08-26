using Autodesk.Revit.DB;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using YMS.Parts;

namespace YMS.DLG
{
    public partial class DlgCreateHaraokoshiBase : System.Windows.Forms.Form
    {
        #region 定数
        /// <summary>
        /// Iniファイルのパス
        /// </summary>
        const string IniPath = "ini\\DlgCreateHaraokoshiBase.ini";

        /// <summary>
        /// Iniファイルのセクション名
        /// </summary>
        const string sec = "DlgCreateHaraokoshiBase";

        /// <summary>
        /// コンボボックス用
        /// </summary>
        const string cmbSukima1 = "使用しない";
        const string cmbSukima2 = "H形鋼 広幅";
        const string cmbSukima3 = "チャンネル";


        const string inisec = "DlgHaraokoshiBase";
        const string iniName = "ini\\DlgCreateHaraokoshiBase.ini"; //ここはファイル名とフォルダ名で分けたほうがいい　kamada
        #endregion

        #region メンバー
        public Document m_doc;
        public ClsHaraokoshiBase m_ClsHaraBase;
        public bool m_Minus;
        #endregion

        public DlgCreateHaraokoshiBase(Document doc, ClsHaraokoshiBase clsHaraBase)
        {
            InitializeComponent();
            m_doc = doc;
            m_ClsHaraBase = clsHaraBase;
            m_Minus = false;
            InitControl();

            SetControl();
            groupBox1.Enabled = false;
            cmbHaichiLevel.Enabled = false;
        }
        public DlgCreateHaraokoshiBase(Document doc)
        {
            InitializeComponent();
            m_doc = doc;
            m_ClsHaraBase = new ClsHaraokoshiBase();
            m_Minus = true;

            InitControl();
            GetIniData();
            groupBox1.Enabled = true;
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            if (!CheckInput())
                return;

            SetData();
            SetIniData();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public void InitControl()
        {
            cmbHaichiLevel.Items.AddRange(ClsYMSUtil.GetLevelNames(m_doc).ToArray());
            initComboBox();
        }


        public void initComboBox()
        {
            string bk = string.Empty;
            List<string> lstStr = new List<string>();

            //鋼材タイプ
            bk = cmbSteelType.Text;
            cmbSteelType.Items.Clear();
            if(rbnSideDouble.Checked == true && rbnVerticalDouble.Checked == false)
            {
                lstStr = Master.ClsYamadomeCsv.GetTypeList(bHigh: true, bMega: true, bTwin: false);
            }
            else
            {
                lstStr = Master.ClsYamadomeCsv.GetTypeList(bHigh: true, bMega: false, bTwin: false);
            }
            foreach (string str in lstStr)
            {
                cmbSteelType.Items.Add(str);
            }
            if (lstStr.Contains(bk)) cmbSteelType.Text = bk;
            else cmbSteelType.SelectedIndex = 0;

            //鋼材サイズ
            initComboBoxSteelSize();

            //隙間調整材鋼材タイプ
            bk = cmbGapAdjustSteelType.Text;
            cmbGapAdjustSteelType.Items.Clear();
            lstStr = new List<string>() { cmbSukima1, cmbSukima2, cmbSukima3 };
            foreach (string str in lstStr)
            {
                cmbGapAdjustSteelType.Items.Add(str);
            }
            if (lstStr.Contains(bk)) cmbGapAdjustSteelType.Text = bk;
            else cmbGapAdjustSteelType.SelectedIndex = 0;

            //隙間調整材鋼材
            initComboBoxGap();
        }

        public void initComboBoxSteelSize()
        {
            string bk = string.Empty;
            List<string> lstStr = new List<string>();

            // 鋼材サイズ
            if (cmbSteelSize != null && cmbSteelType != null)
            {
                bk = cmbSteelSize.Text;
                cmbSteelSize.Items.Clear();

                lstStr = Master.ClsYamadomeCsv.GetSizeList(cmbSteelType.Text);

                if (lstStr != null)
                {
                    foreach (string str in lstStr)
                    {
                        cmbSteelSize.Items.Add(str);
                    }

                    if (lstStr.Contains(bk))
                    {
                        cmbSteelSize.Text = bk;
                    }
                    else if (cmbSteelSize.Items.Count > 0)
                    {
                        cmbSteelSize.SelectedIndex = 0;
                    }
                    else
                    {
                        cmbSteelSize.Text = string.Empty;
                    }
                }
                else
                {
                    cmbSteelSize.Text = string.Empty;
                }
            }

        }

        public void initComboBoxGap()
        {
            string selectedText = cmbGapAdjustSteel.Text;
            cmbGapAdjustSteel.Items.Clear();

            List<string> lstStr;

            if (cmbGapAdjustSteelType.Text == cmbSukima1)
            {
                lstStr = new List<string>() { string.Empty };
            }
            else if (cmbGapAdjustSteelType.Text == cmbSukima2)
            {
                lstStr = Master.ClsHBeamCsv.GetSizeList(Master.ClsHBeamCsv.TypeHiro);
            }
            else
            {
                lstStr = Master.ClsChannelCsv.GetSizeList();
            }

            foreach (string str in lstStr)
            {
                cmbGapAdjustSteel.Items.Add(str);
            }

            if (lstStr.Contains(selectedText))
            {
                cmbGapAdjustSteel.Text = selectedText;
            }
            else if (cmbGapAdjustSteel.Items.Count > 0)
            {
                cmbGapAdjustSteel.SelectedIndex = 0;
            }
            else
            {
                cmbGapAdjustSteel.Text = string.Empty;
            }
        }


        private void DlgCreateHaraokoshiBase_Load(object sender, System.EventArgs e)
        {
            //cmbHaichiLevel.Items.AddRange(ClsRevitUtil.GetLevelNames(m_doc).ToArray());

            ////iniファイルデータセット
            //string iniPath = ClsIni.GetIniFilePath(iniName);
            //cmbHaichiLevel.SelectedItem = ClsIni.GetIniFile(inisec, "LEVEL", iniPath);
            //if (cmbHaichiLevel.SelectedItem == null) cmbHaichiLevel.SelectedIndex = 0;

            //string danName = ClsIni.GetIniFile(inisec, "DANLEVEL", iniPath);
            //var RadioButtonChecked_InGroup = groupBox3.Controls.OfType<RadioButton>().SingleOrDefault(rb => rb.Text == danName);
            //RadioButtonChecked_InGroup.Checked = true;

            //txtOffset.Text = ClsIni.GetIniFile(inisec, "OFFSET", iniPath);

            //cmbSteel.SelectedIndex = 0;

            //chkMegabeam();

        }


        public bool CheckInput()
        {
            if (!ClsCommonUtils.ChkContorolCommbo(cmbHaichiLevel, "配置レベル", true))
                return false;
            if (!ClsCommonUtils.ChkContorolCommbo(cmbSteelType, "鋼材タイプ", true))
                return false;
            if (!ClsCommonUtils.ChkContorolCommbo(cmbSteelSize, "鋼材", true))
                return false;
            if (!ClsCommonUtils.ChkContorolTextDouble(txtMegaBeam, "メガビーム", bMinus: true, bComma: true))
                return false;
            if (!ClsCommonUtils.ChkContorolTextDouble(txtOffset, "オフセット量",bMinus: m_Minus))
                return false;
            if (!ClsCommonUtils.ChkContorolTextDouble(txtVerticalGap, "縦方向の隙間", bMinus: true))
                return false;
            if (!ClsCommonUtils.ChkContorolCommbo(cmbGapAdjustSteelType, "隙間調整材鋼材タイプ", true))
                return false;
            //if (!ClsCommonUtils.ChkContorolCommbo(cmbGapAdjustSteel, "隙間調整材", true))
            //    return false;
            if (!ClsCommonUtils.ChkContorolTextDouble(txtGapAdjustSteelLength, "隙間調整材長さ", bMinus: true))
                return false;

            return true;
        }

        public void SetData()
        {
            ClsHaraokoshiBase clsHaraBase = m_ClsHaraBase;


            if (rbnShoriTypeBaseLineOut.Checked)
            {
                clsHaraBase.m_ShoriType = ClsHaraokoshiBase.ShoriType.BaseLineZureAri;
            }
            else if (rbnShoriTypePtoP.Checked)
            {
                clsHaraBase.m_ShoriType = ClsHaraokoshiBase.ShoriType.PtoPZureNashi;
            }
            else if (rbnShoriTypePtoPOut.Checked)
            {
                clsHaraBase.m_ShoriType = ClsHaraokoshiBase.ShoriType.PtoPZureAri;
            }
            else
            {
                clsHaraBase.m_ShoriType = ClsHaraokoshiBase.ShoriType.Replace;
            }

            clsHaraBase.m_level = cmbHaichiLevel.SelectedItem.ToString();

            //段レベルは文字列で管理
            var RadioButtonChecked_InGroup = groupBox3.Controls.OfType<RadioButton>().SingleOrDefault(rb => rb.Checked == true);
            clsHaraBase.m_dan = RadioButtonChecked_InGroup.Text;


            if (rbnSideSingle.Checked)
            {
                clsHaraBase.m_yoko = ClsHaraokoshiBase.SideNum.Single;
            }
            else
            {
                clsHaraBase.m_yoko = ClsHaraokoshiBase.SideNum.Double;
            }

            if (rbnVerticalSingle.Checked)
            {
                clsHaraBase.m_tate = ClsHaraokoshiBase.VerticalNum.Single;
            }
            else
            {
                clsHaraBase.m_tate = ClsHaraokoshiBase.VerticalNum.Double;
            }

            clsHaraBase.m_kouzaiType = cmbSteelType.Text;
            clsHaraBase.m_kouzaiSize = cmbSteelSize.Text;
            //clsHaraBase.m_bMega = chkMegaBeam.Checked;
            clsHaraBase.m_mega = ClsCommonUtils.ChangeStrToInt(txtMegaBeam.Text);
            //clsHaraBase.m_bSMH = chkSMH.Checked;
            clsHaraBase.m_offset = ClsCommonUtils.ChangeStrToDbl(txtOffset.Text);

            clsHaraBase.m_tateGap = ClsCommonUtils.ChangeStrToDbl(txtVerticalGap.Text);
            clsHaraBase.m_gapTyouseiType = cmbGapAdjustSteelType.Text;
            clsHaraBase.m_gapTyousei = cmbGapAdjustSteel.Text;

            clsHaraBase.m_gapTyouseiLenght = ClsCommonUtils.ChangeStrToDbl(txtGapAdjustSteelLength.Text);

            if (rbnWin.Checked)
            {
                clsHaraBase.m_katimake = ClsHaraokoshiBase.WinLose.Win;
            }
            else
            {
                clsHaraBase.m_katimake = ClsHaraokoshiBase.WinLose.Lose;
            }

            ////iniファイルデータセット
            //string iniPath = ClsIni.GetIniFilePath(iniName);
            //ClsIni.WritePrivateProfileString(inisec, "LEVEL", clsHaraBase.m_level, iniPath);
            //ClsIni.WritePrivateProfileString(inisec, "DANLEVEL", clsHaraBase.m_dan, iniPath);
            //ClsIni.WritePrivateProfileString(inisec, "OFFSET", clsHaraBase.m_offset.ToString(), iniPath);

            //メンバ変数に格納
            m_ClsHaraBase = clsHaraBase;
        }

        /// <summary>
        /// メンバの値をコントロールにセット
        /// </summary>
        private void SetControl()
        {
            ClsHaraokoshiBase clsHaraBase = m_ClsHaraBase;

            if (clsHaraBase.m_ShoriType == ClsHaraokoshiBase.ShoriType.BaseLineZureAri)
            {
                rbnShoriTypeBaseLineOut.Checked = true;
            }
            else if (clsHaraBase.m_ShoriType == ClsHaraokoshiBase.ShoriType.PtoPZureNashi)
            {
                rbnShoriTypePtoP.Checked = true;
            }
            else if (clsHaraBase.m_ShoriType == ClsHaraokoshiBase.ShoriType.PtoPZureAri)
            {
                rbnShoriTypePtoPOut.Checked = true;
            }
            else
            {
                rbnReplace.Checked = true;
            }

            cmbHaichiLevel.Text = clsHaraBase.m_level;

            //段レベルは文字列で管理
            if (clsHaraBase.m_dan == rbnDanLevelUpper.Text)
            {
                rbnDanLevelUpper.Checked = true;
            }
            else if (clsHaraBase.m_dan == rbnDanLevelJust.Text)
            {
                rbnDanLevelJust.Checked = true;
            }
            else if (clsHaraBase.m_dan == rbnDanLevelLower.Text)
            {
                rbnDanLevelLower.Checked = true;
            }
            else
            {
                rbnDanLevelJust.Checked = true;
            }

            if (clsHaraBase.m_yoko == ClsHaraokoshiBase.SideNum.Single)
            {
                rbnSideSingle.Checked = true;
            }
            else if (clsHaraBase.m_yoko == ClsHaraokoshiBase.SideNum.Double)
            {
                rbnSideDouble.Checked = true;
            }
            else
            {
                rbnSideSingle.Checked = true;
            }

            if (clsHaraBase.m_tate == ClsHaraokoshiBase.VerticalNum.Single)
            {
                rbnVerticalSingle.Checked = true;
            }
            else if (clsHaraBase.m_tate == ClsHaraokoshiBase.VerticalNum.Double)
            {
                rbnVerticalDouble.Checked = true;
            }
            else
            {
                rbnVerticalSingle.Checked = true;
            }

            cmbSteelType.Text = clsHaraBase.m_kouzaiType;
            cmbSteelSize.Text = clsHaraBase.m_kouzaiSize;
            txtMegaBeam.Text = clsHaraBase.m_mega.ToString();
            txtOffset.Text = clsHaraBase.m_offset.ToString();

            txtVerticalGap.Text = clsHaraBase.m_tateGap.ToString();
            cmbGapAdjustSteelType.Text = clsHaraBase.m_gapTyouseiType;
            cmbGapAdjustSteel.Text = clsHaraBase.m_gapTyousei;

            txtGapAdjustSteelLength.Text = clsHaraBase.m_gapTyouseiLenght.ToString();

            if (clsHaraBase.m_katimake == ClsHaraokoshiBase.WinLose.Win)
            {
                rbnWin.Checked = true;
            }
            else
            {
                rbnLose.Checked = true;
            }
        }

        /// <summary>
        /// Iniの情報をコントロールにセット
        /// </summary>
        private void GetIniData()
        {
            try
            {
                string iniPath = ClsIni.GetIniFilePath(IniPath);

                rbnShoriTypeBaseLineOut.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnShoriTypeBaseLineOut.Name, iniPath));
                rbnShoriTypePtoPOut.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnShoriTypePtoPOut.Name, iniPath));
                rbnShoriTypePtoP.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnShoriTypePtoP.Name, iniPath));
                rbnReplace.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnReplace.Name, iniPath));
                cmbHaichiLevel.Text = ClsIni.GetIniFile(sec, cmbHaichiLevel.Name, iniPath);

                rbnDanLevelUpper.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnDanLevelUpper.Name, iniPath));
                rbnDanLevelJust.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnDanLevelJust.Name, iniPath));
                rbnDanLevelLower.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnDanLevelLower.Name, iniPath));
                rbnSideSingle.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnSideSingle.Name, iniPath));
                rbnSideDouble.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnSideDouble.Name, iniPath));
                rbnVerticalSingle.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnVerticalSingle.Name, iniPath));
                rbnVerticalDouble.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnVerticalDouble.Name, iniPath));

                cmbSteelType.Text = ClsIni.GetIniFile(sec, cmbSteelType.Name, iniPath);
                cmbSteelSize.Text = ClsIni.GetIniFile(sec, cmbSteelSize.Name, iniPath);

                //chkMegaBeam.Checked = bool.Parse(ClsIni.GetIniFile(sec, chkMegaBeam.Name, iniPath));
                txtMegaBeam.Text = ClsIni.GetIniFile(sec, txtMegaBeam.Name, iniPath);
                //chkSMH.Checked = bool.Parse(ClsIni.GetIniFile(sec, chkSMH.Name, iniPath));

                txtOffset.Text = ClsIni.GetIniFile(sec, txtOffset.Name, iniPath);
                txtVerticalGap.Text = ClsIni.GetIniFile(sec, txtVerticalGap.Name, iniPath);
                cmbGapAdjustSteelType.Text = ClsIni.GetIniFile(sec, cmbGapAdjustSteelType.Name, iniPath);
                cmbGapAdjustSteel.Text = ClsIni.GetIniFile(sec, cmbGapAdjustSteel.Name, iniPath);
                txtGapAdjustSteelLength.Text = ClsIni.GetIniFile(sec, txtGapAdjustSteelLength.Name, iniPath);

                rbnWin.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnWin.Name, iniPath));
                rbnLose.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnLose.Name, iniPath));
            }
            catch
            {
               // MessageBox.Show("前回値の取得に失敗しました");
            }
        }

        /// <summary>
        /// コントロールの情報をIniにセット
        /// </summary>
        private void SetIniData()
        {
            string iniPath = ClsIni.GetIniFilePath(IniPath);


            ClsIni.WritePrivateProfileString(sec, rbnShoriTypeBaseLineOut.Name, rbnShoriTypeBaseLineOut.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnShoriTypePtoPOut.Name, rbnShoriTypePtoPOut.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnShoriTypePtoP.Name, rbnShoriTypePtoP.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnReplace.Name, rbnReplace.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, cmbHaichiLevel.Name, cmbHaichiLevel.Text, iniPath);

            ClsIni.WritePrivateProfileString(sec, rbnDanLevelUpper.Name, rbnDanLevelUpper.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnDanLevelJust.Name, rbnDanLevelJust.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnDanLevelLower.Name, rbnDanLevelLower.Checked.ToString(), iniPath);

            ClsIni.WritePrivateProfileString(sec, rbnSideSingle.Name, rbnSideSingle.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnSideDouble.Name, rbnSideDouble.Checked.ToString(), iniPath);

            ClsIni.WritePrivateProfileString(sec, rbnVerticalSingle.Name, rbnVerticalSingle.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnVerticalDouble.Name, rbnVerticalDouble.Checked.ToString(), iniPath);

            ClsIni.WritePrivateProfileString(sec, cmbSteelType.Name, cmbSteelType.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, cmbSteelSize.Name, cmbSteelSize.Text, iniPath);
            //ClsIni.WritePrivateProfileString(sec, chkMegaBeam.Name, chkMegaBeam.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, txtMegaBeam.Name, txtMegaBeam.Text, iniPath);
            //ClsIni.WritePrivateProfileString(sec, chkSMH.Name, chkSMH.Checked.ToString(), iniPath);

            ClsIni.WritePrivateProfileString(sec, txtOffset.Name, txtOffset.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, txtVerticalGap.Name, txtVerticalGap.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, cmbGapAdjustSteelType.Name, cmbGapAdjustSteelType.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, cmbGapAdjustSteel.Name, cmbGapAdjustSteel.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, txtGapAdjustSteelLength.Name, txtGapAdjustSteelLength.Text, iniPath);

            ClsIni.WritePrivateProfileString(sec, rbnWin.Name, rbnWin.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnLose.Name, rbnLose.Checked.ToString(), iniPath);
        }

        private void rbnVerticalSingle_CheckedChanged(object sender, System.EventArgs e)
        {
            chkGapAdjust();
            initComboBox();
        }

        private void cmbSteel_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            chkMegabeam();
        }

        private void chkMegabeam()
        {
            if (cmbSteelType.Text == Master.ClsYamadomeCsv.TypeMegaBeam)
            {
                txtMegaBeam.Enabled = true;
            }
            else
            {
                txtMegaBeam.Enabled = false;
            }
        }


        //private void chkMegaBeam_CheckedChanged(object sender, System.EventArgs e)
        //{
        //    if(chkMegaBeam.Checked == true)
        //    {
        //        chkSMH.Enabled = false;
        //    }
        //    else
        //    {
        //        chkSMH.Enabled = true;
        //    }
        //}

        //private void chkSMH_CheckedChanged(object sender, System.EventArgs e)
        //{
        //    if(chkSMH.Checked == true)
        //    {
        //        chkMegaBeam.Enabled= false;
        //        txtMegaBeam.Enabled = false;
        //    }
        //    else
        //    {
        //        chkMegaBeam.Enabled = true;
        //        txtMegaBeam.Enabled = true;
        //    }
        //}

        private void cmbGapAdjustSteelType_SelectedIndexChanged(object sender, EventArgs e)
        {
            initComboBoxGap();
        }

        private void cmbSteelType_SelectedIndexChanged(object sender, EventArgs e)
        {
            initComboBoxSteelSize();
        }

        private void chkGapAdjust()
        {
            if (rbnVerticalSingle.Checked == true)
            {
                txtVerticalGap.Enabled = false;                  //縦方向の隙間
                cmbGapAdjustSteelType.Enabled = false;           //隙間調整材鋼材タイプ
                cmbGapAdjustSteel.Enabled = false;               //隙間調整材鋼材サイズ
                txtGapAdjustSteelLength.Enabled = false;
                cmbGapAdjustSteelType.Text = cmbSukima1;
                cmbGapAdjustSteel.Items.Clear();
                cmbGapAdjustSteel.Items.Add(string.Empty);
            }
            else
            {
                txtVerticalGap.Enabled = true;                   //縦方向の隙間
                cmbGapAdjustSteelType.Enabled = true;            //隙間調整材鋼材タイプ
                cmbGapAdjustSteel.Enabled = true;                //隙間調整材鋼材サイズ
                txtGapAdjustSteelLength.Enabled = true;          //隙間調整材鋼材長さ
            }
        }

        private void rbnSideDouble_CheckedChanged(object sender, EventArgs e)
        {
            initComboBox();
        }

        private void rbnVerticalDouble_CheckedChanged(object sender, EventArgs e)
        {
            initComboBox();
        }

        //private void chkMegabeam()
        //{
        //    string selectedItem = cmbSteel.SelectedItem.ToString();
        //    switch (selectedItem)
        //    {
        //        case "35HA":
        //            chkSMH.Enabled = true;

        //            break;
        //        case "40HA":
        //            chkMegaBeam.Enabled = true;
        //            txtMegaBeam.Enabled = true;

        //            chkSMH.Enabled = true;
        //            break;
        //        default:
        //            chkMegaBeam.Enabled = false;
        //            txtMegaBeam.Enabled = false;

        //            chkSMH.Enabled = false;
        //            break;
        //    }
        //}
    }
}
