using Autodesk.Revit.DB;
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
using YMS.Master;
using YMS.Parts;

namespace YMS.DLG
{
    public partial class DlgCreateBracket : System.Windows.Forms.Form
    {
        #region 定数

        /// <summary>
        /// Iniファイルのパス
        /// </summary>
        const string iniPath = "ini\\DlgCreateBracket.ini";

        /// <summary>
        /// Iniファイルのセクション名
        /// </summary>
        const string sec = "DlgCreateBracket";

        #endregion

        #region メンバー変数

        public Document m_doc;
        public ClsBracket m_ClsBracket;

        #endregion

        public DlgCreateBracket(Document doc)
        {
            InitializeComponent();
            m_doc = doc;
            m_ClsBracket = new ClsBracket();
            initControl();
            getIniData();
        }

        private void initControl()
        {
            rbnKiriBracket.Checked = true;
            initCreateMode();
            initProhibitionLength();
            initComboBox();
        }

        private void initComboBox()
        {
            // ブラケットサイズ
            initComboBoxBracketSize();
        }

        private void initComboBoxBracketSize()
        {
            if (cmbBracketSize != null)
            {
                string selectedText = cmbBracketSize.Text;
                cmbBracketSize.Items.Clear();

                List<string> lstStr = new List<string>();
                var RadioButtonChecked_InGroup = grpBracketType.Controls.OfType<RadioButton>().SingleOrDefault(rb => rb.Checked == true);
                if (RadioButtonChecked_InGroup == null)
                {
                    return;
                }
                lstStr = Master.ClsBracketCsv.GetSizeList(RadioButtonChecked_InGroup.Text);

                foreach (string str in lstStr)
                {
                    cmbBracketSize.Items.Add(str);
                }
                if (cmbBracketSize.Items.Count > 0)
                {
                    if (lstStr.Contains(selectedText))
                    {
                        cmbBracketSize.Text = selectedText;
                    }
                    else
                    {
                        cmbBracketSize.SelectedIndex = 0;
                    }
                }
            }
        }

        private void initCreateMode()
        {
            var RBChecked_InGroup_Type = grpBracketType.Controls.OfType<RadioButton>().SingleOrDefault(rb => rb.Checked == true);
            if (RBChecked_InGroup_Type != null)
            {
                if (RBChecked_InGroup_Type.Text.Substring(0, 2) == "腹起")
                {
                    grpCreateMode.Enabled = true;
                    cmbBracketSize.Enabled = false;
                }
                else
                {
                    grpCreateMode.Enabled = false;
                    rbnManual.Checked = true;
                    cmbBracketSize.Enabled = true;
                }
            }
        }

        private void initProhibitionLength()
        {
            var RBChecked_InGroup_Mode = grpCreateMode.Controls.OfType<RadioButton>().SingleOrDefault(rb => rb.Checked == true);
            if (RBChecked_InGroup_Mode != null)
            {
                if (RBChecked_InGroup_Mode.Text == "手動")
                {
                    cmbBracketSize.Enabled = true;
                    txtProhibitionLength.Enabled = false;
                }
                else if (RBChecked_InGroup_Mode.Text == "自動")
                {
                    cmbBracketSize.Enabled = false;
                    txtProhibitionLength.Enabled = true;
                }
                else if (RBChecked_InGroup_Mode.Text == "任意")
                {
                    cmbBracketSize.Enabled = false;
                    txtProhibitionLength.Enabled = true;
                }
            }
        }

        private void getIniData()
        {
            try
            {
                string path = ClsIni.GetIniFilePath(iniPath);

                rbnKiriBracket.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnKiriBracket.Name, path));
                rbnKiriOsaeBracket.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnKiriOsaeBracket.Name, path));
                rbnHaraBracket.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnHaraBracket.Name, path));
                rbnHaraOsaeBracket.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnHaraOsaeBracket.Name, path));

                txtProhibitionLength.Text = ClsIni.GetIniFile(sec, txtProhibitionLength.Name, path);

                cmbBracketSize.Text = ClsIni.GetIniFile(sec, cmbBracketSize.Name, path);

                rbnManual.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnManual.Name, path));
                rbnAuto.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnAuto.Name, path));
                rbnOptional.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnOptional.Name, path));
            }
            catch
            {
                //MessageBox.Show("前回値の取得に失敗しました");
            }
        }

        private void setIniData()
        {
            string path = ClsIni.GetIniFilePath(iniPath);

            ClsIni.WritePrivateProfileString(sec, rbnKiriBracket.Name, rbnKiriBracket.Checked.ToString(), path);
            ClsIni.WritePrivateProfileString(sec, rbnKiriOsaeBracket.Name, rbnKiriOsaeBracket.Checked.ToString(), path);
            ClsIni.WritePrivateProfileString(sec, rbnHaraBracket.Name, rbnHaraBracket.Checked.ToString(), path);
            ClsIni.WritePrivateProfileString(sec, rbnHaraOsaeBracket.Name, rbnHaraOsaeBracket.Checked.ToString(), path);

            ClsIni.WritePrivateProfileString(sec, txtProhibitionLength.Name, txtProhibitionLength.Text, path);

            ClsIni.WritePrivateProfileString(sec, cmbBracketSize.Name, cmbBracketSize.Text, path);

            ClsIni.WritePrivateProfileString(sec, rbnManual.Name, rbnManual.Checked.ToString(), path);
            ClsIni.WritePrivateProfileString(sec, rbnAuto.Name, rbnAuto.Checked.ToString(), path);
            ClsIni.WritePrivateProfileString(sec, rbnOptional.Name, rbnOptional.Checked.ToString(), path);
        }

        private void setData()
        {
            ClsBracket clsBracket = m_ClsBracket;

            // 指定したグループ内でチェックされているラジオボタンを取得する
            var RadioButtonChecked_InGroup = grpBracketType.Controls.OfType<RadioButton>().SingleOrDefault(rb => rb.Checked == true);
            switch (RadioButtonChecked_InGroup.Text)
            {
                case "切梁ブラケット":
                    clsBracket.m_CreateType = ClsBracket.CreateType.Kiribari;
                    break;
                case "切梁押えブラケット":
                    clsBracket.m_CreateType = ClsBracket.CreateType.kiribariOsae;
                    break;
                case "腹起ブラケット":
                    clsBracket.m_CreateType = ClsBracket.CreateType.Haraokoshi;
                    break;
                case "腹起押えブラケット":
                    clsBracket.m_CreateType = ClsBracket.CreateType.HaraokoshiOsae;
                    break;
                default:
                    break;
            }

            clsBracket.m_ProhibitionLength = ClsRevitUtil.CovertToAPI(double.Parse(txtProhibitionLength.Text));

            clsBracket.m_BracketSize = cmbBracketSize.SelectedItem.ToString();

            if (grpCreateMode.Enabled && rbnManual.Checked)
            {
                clsBracket.m_CreateMode = ClsBracket.CreateMode.Manual;
            }
            else if (grpCreateMode.Enabled && rbnAuto.Checked)
            {
                clsBracket.m_CreateMode = ClsBracket.CreateMode.Auto;
            }
            else if (grpCreateMode.Enabled && rbnOptional.Checked)
            {
                clsBracket.m_CreateMode = ClsBracket.CreateMode.Optional;
            }

        }

        public bool checkInput()
        {
            if (!ClsCommonUtils.ChkContorolTextDouble(txtProhibitionLength, "割付禁止距離", bMinus: true))
            {
                return false;
            }

            return true;
        }

        #region コントロールイベント

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            initCreateMode();

            initProhibitionLength();

            initComboBoxBracketSize();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!checkInput())
            {
                return;
            }

            setData();
            setIniData();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #endregion
    }
}
