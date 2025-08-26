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
using YMS.Command;
using YMS.Parts;
using static YMS.Parts.ClsAtamaTsunagi;

namespace YMS.DLG
{
    public partial class DlgCreateAtamatsunagi : System.Windows.Forms.Form
    {
        #region 定数

        /// <summary>
        /// Iniファイルのパス
        /// </summary>
        const string iniPath = "ini\\DlgCreateAtamatsunagi.ini";

        /// <summary>
        /// Iniファイルのセクション名
        /// </summary>
        const string sec = "DlgCreateAtamatsunagi";

        #endregion

        #region メンバー変数

        public Document m_doc;
        public ClsAtamaTsunagi m_ClsAtamaTsunagi;

        #endregion

        public DlgCreateAtamatsunagi(Document doc)
        {
            InitializeComponent();
            m_doc = doc;
            m_ClsAtamaTsunagi = new ClsAtamaTsunagi();
            initControl();
            getIniData();
        }

        public void initControl()
        {
            initComboBox();
        }

        public void initComboBox()
        {
            initComboBoxSteelType();
            initComboBoxSteelSize();

            initComboBoxBoltType();
            initComboBoxBoltSize();
        }

        public void initComboBoxSteelType()
        {
            // 鋼材タイプ
            string selectedText = cmbSteelType.Text;
            cmbSteelType.Items.Clear();
            List<string> lstStr = new List<string>();
            lstStr = new List<string>() { Master.ClsChannelCsv.TypeCannel,
                                          Master.ClsAngleCsv.TypeAngle,
                                          Master.ClsHBeamCsv.TypeHiro ,
                                          Master.ClsYamadomeCsv.TypeShuzai,
                                          };
            foreach (string str in lstStr)
            {
                cmbSteelType.Items.Add(str);
            }
            if (cmbSteelType.Items.Count > 0)
            {
                if (lstStr.Contains(selectedText))
                {
                    cmbSteelType.Text = selectedText;
                }
                else
                {
                    cmbSteelType.SelectedIndex = 0;
                }
            }
        }

        public void initComboBoxSteelSize()
        {
            // 鋼材サイズ
            if (cmbSteelSize != null)
            {
                string selectedText = cmbSteelSize.Text;
                cmbSteelSize.Items.Clear();
                List<string> lstStr = new List<string>();
                switch (cmbSteelType.Text)
                {
                    case Master.ClsChannelCsv.TypeCannel:
                        lstStr = Master.ClsChannelCsv.GetSizeList(bAtamaTsunagi: true);
                        break;
                    case Master.ClsAngleCsv.TypeAngle:
                        lstStr = Master.ClsAngleCsv.GetSizeList(Master.ClsAngleCsv.TypeAngle, bAtamaTsunagi: true);
                        break;
                    case Master.ClsHBeamCsv.TypeHiro:
                        lstStr = Master.ClsHBeamCsv.GetSizeList(Master.ClsHBeamCsv.TypeHiro, bAtamaTsunagi: true);
                        break;
                    case Master.ClsYamadomeCsv.TypeShuzai:
                        lstStr = Master.ClsYamadomeCsv.GetSizeList(Master.ClsYamadomeCsv.TypeShuzai, bAtamaTsunagi: true);
                        break;
                    default:
                        return;
                }

                foreach (string str in lstStr)
                {
                    cmbSteelSize.Items.Add(str);
                }
                if (cmbSteelSize.Items.Count > 0)
                {
                    if (lstStr.Contains(selectedText))
                    {
                        cmbSteelSize.Text = selectedText;
                    }
                    else
                    {
                        cmbSteelSize.SelectedIndex = 0;
                    }
                }
            }
        }

        public void initComboBoxBoltType()
        {
            // ボルトタイプ
            string selectedText = cmbBoltType.Text;
            cmbBoltType.Items.Clear();
            List<string> lstStr = new List<string>();
            lstStr = new List<string>() { Master.ClsBoltCsv.BoltTypeN,
                                          Master.ClsBoltCsv.BoltTypeH,
                                          Master.ClsBoltCsv.BoltTypeT,
                                          };
            foreach (string str in lstStr)
            {
                cmbBoltType.Items.Add(str);
            }
            if (cmbBoltType.Items.Count > 0)
            {
                if (lstStr.Contains(selectedText))
                {
                    cmbBoltType.Text = selectedText;
                }
                else
                {
                    cmbBoltType.SelectedIndex = 0;
                }
            }

            if(rbnBolt.Enabled == true && rbnBolt.Checked == true)
            {
                cmbBoltType.Enabled = true;
                cmbBoltSize.Enabled = true;
                txtBoltNum.Enabled = true;//#33920
            }
            else
            {
                cmbBoltType.Enabled = false;
                cmbBoltSize.Enabled = false;
                txtBoltNum.Enabled = false;//#33920
            }

        }

        public void initComboBoxBoltSize()
        {
            // ボルトサイズ
            if (cmbBoltSize != null)
            {
                string selectedText = cmbBoltSize.Text;
                cmbBoltSize.Items.Clear();
                List<string> lstStr = new List<string>();
                switch (cmbBoltType.Text)
                {
                    case Master.ClsBoltCsv.BoltTypeN:
                    case Master.ClsBoltCsv.BoltTypeH:
                    case Master.ClsBoltCsv.BoltTypeT:
                        lstStr = Master.ClsBoltCsv.GetSizeList(cmbBoltType.Text);
                        break;
                    default:
                        return;
                }

                foreach (string str in lstStr)
                {
                    cmbBoltSize.Items.Add(str);
                }
                if (cmbBoltSize.Items.Count > 0)
                {
                    if (lstStr.Contains(selectedText))
                    {
                        cmbBoltSize.Text = selectedText;
                    }
                    else
                    {
                        cmbBoltSize.SelectedIndex = 0;
                    }
                }
            }
        }

        private void getIniData()
        {
            try
            {
                string path = ClsIni.GetIniFilePath(iniPath);

                cmbSteelType.Text = ClsIni.GetIniFile(sec, cmbSteelType.Name, path);
                cmbSteelSize.Text = ClsIni.GetIniFile(sec, cmbSteelSize.Name, path);

                rbnChannelDirection1.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnChannelDirection1.Name, path));
                rbnChannelDirection2.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnChannelDirection2.Name, path));

                rbnWelding.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnWelding.Name, path));
                rbnBolt.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnBolt.Name, path));
                rbnBurumanC50.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnBurumanC50.Name, path));
                rbnRikimanGType.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnRikimanGType.Name, path));

                cmbBoltType.Text = ClsIni.GetIniFile(sec, cmbBoltType.Name, path);
                cmbBoltSize.Text = ClsIni.GetIniFile(sec, cmbBoltSize.Name, path);
                txtBoltNum.Text = ClsIni.GetIniFile(sec, txtBoltNum.Name, path);//#33920

                rbnFront.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnFront.Name, path));
                rbnBack.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnBack.Name, path));

                txtTsukidashiSt.Text = ClsIni.GetIniFile(sec, txtTsukidashiSt.Name, path);
                txtTsukidashiEd.Text = ClsIni.GetIniFile(sec, txtTsukidashiEd.Name, path);

                rbnKuiTenba.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnKuiTenba.Name, path));
                rbnFromToP.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnFromToP.Name, path));
                txtFromToP.Text = ClsIni.GetIniFile(sec, txtFromToP.Name, path);

                initComboBox();
            }
            catch
            {
                //MessageBox.Show("前回値の取得に失敗しました");
            }
        }

        private void setIniData()
        {
            string path = ClsIni.GetIniFilePath(iniPath);

            ClsIni.WritePrivateProfileString(sec, cmbSteelType.Name, cmbSteelType.Text, path);
            ClsIni.WritePrivateProfileString(sec, cmbSteelSize.Name, cmbSteelSize.Text, path);

            ClsIni.WritePrivateProfileString(sec, rbnChannelDirection1.Name, rbnChannelDirection1.Checked.ToString(), path);
            ClsIni.WritePrivateProfileString(sec, rbnChannelDirection2.Name, rbnChannelDirection2.Checked.ToString(), path);

            ClsIni.WritePrivateProfileString(sec, rbnWelding.Name, rbnWelding.Checked.ToString(), path);
            ClsIni.WritePrivateProfileString(sec, rbnBolt.Name, rbnBolt.Checked.ToString(), path);
            ClsIni.WritePrivateProfileString(sec, rbnBurumanC50.Name, rbnBurumanC50.Checked.ToString(), path);
            ClsIni.WritePrivateProfileString(sec, rbnRikimanGType.Name, rbnRikimanGType.Checked.ToString(), path);

            ClsIni.WritePrivateProfileString(sec, cmbBoltType.Name, cmbBoltType.Text, path);
            ClsIni.WritePrivateProfileString(sec, cmbBoltSize.Name, cmbBoltSize.Text, path);
            ClsIni.WritePrivateProfileString(sec, txtBoltNum.Name, txtBoltNum.Text, path);//#33920

            ClsIni.WritePrivateProfileString(sec, rbnFront.Name, rbnFront.Checked.ToString(), path);
            ClsIni.WritePrivateProfileString(sec, rbnBack.Name, rbnBack.Checked.ToString(), path);

            ClsIni.WritePrivateProfileString(sec, txtTsukidashiSt.Name, txtTsukidashiSt.Text, path);
            ClsIni.WritePrivateProfileString(sec, txtTsukidashiEd.Name, txtTsukidashiEd.Text, path);

            ClsIni.WritePrivateProfileString(sec, rbnKuiTenba.Name, rbnKuiTenba.Checked.ToString(), path);
            ClsIni.WritePrivateProfileString(sec, rbnFromToP.Name, rbnFromToP.Checked.ToString(), path);
            ClsIni.WritePrivateProfileString(sec, txtFromToP.Name, txtFromToP.Text, path);

        }

        public bool checkInput()
        {
            if (!ClsCommonUtils.ChkContorolTextDouble(txtTsukidashiSt, "突き出し量 始点側", bMinus: true))
            {
                return false;
            }

            if (!ClsCommonUtils.ChkContorolTextDouble(txtTsukidashiEd, "突き出し量 終点側", bMinus: true))
            {
                return false;
            }

            if (!ClsCommonUtils.ChkContorolTextDouble(txtFromToP, "杭天端からの高さ", bMinus: true))
            {
                return false;
            }

            if (!ClsCommonUtils.ChkContorolTextDouble(txtBoltNum, "ボルト本数", bMinus: true, bZero: true, bComma: true))//#33920
            {
                return false;
            }

            return true;
        }

        private void setData()
        {
            ClsAtamaTsunagi clsAtamaTsunagi = m_ClsAtamaTsunagi;

            clsAtamaTsunagi.m_KouzaiType = cmbSteelType.SelectedItem.ToString();
            clsAtamaTsunagi.m_KouzaiSize = cmbSteelSize.SelectedItem.ToString();

            if (groupBox2.Enabled == true)
            {
                if (rbnChannelDirection1.Checked == true)
                {
                    clsAtamaTsunagi.m_ChannelDirection = ChannelDirection.DirectionVertical;
                }
                else if (rbnChannelDirection2.Checked == true)
                {
                    clsAtamaTsunagi.m_ChannelDirection = ChannelDirection.DirectionHorizontal;
                }
                else
                {
                    clsAtamaTsunagi.m_ChannelDirection = ChannelDirection.None;
                }
            }
            else
            {
                clsAtamaTsunagi.m_ChannelDirection = ChannelDirection.None;
            }

            if (rbnWelding.Checked == true)
            {
                clsAtamaTsunagi.m_JointType = JointType.Welding;
            }
            else if (rbnBolt.Checked == true)
            {
                clsAtamaTsunagi.m_JointType = JointType.Bolt;
            }
            else if (rbnBurumanC50.Checked == true)
            {
                clsAtamaTsunagi.m_JointType = JointType.BurumanC;
            }
            else if (rbnRikimanGType.Checked == true)
            {
                clsAtamaTsunagi.m_JointType = JointType.RikimanG;
            }

            if (rbnBolt.Checked == true)
            {
                clsAtamaTsunagi.m_BoltType = cmbBoltType.SelectedItem.ToString();
                clsAtamaTsunagi.m_BoltSize = cmbBoltSize.SelectedItem.ToString();
                clsAtamaTsunagi.m_BoltNum = int.Parse(txtBoltNum.Text.ToString());//#33920
            }
            else
            {
                clsAtamaTsunagi.m_BoltType = "";
                clsAtamaTsunagi.m_BoltSize = "";
                clsAtamaTsunagi.m_BoltNum = 0;//#33920
            }

            if (rbnFront.Enabled == true && rbnFront.Checked == true)
            {
                clsAtamaTsunagi.m_ConfigurationDirection = ConfigurationDirection.Front;
            }
            else if (rbnBack.Enabled == true && rbnBack.Checked == true)
            {
                clsAtamaTsunagi.m_ConfigurationDirection = ConfigurationDirection.Back;
            }
            else
            {
                clsAtamaTsunagi.m_ConfigurationDirection = ConfigurationDirection.None;
            }

            if (cmbSteelType.SelectedItem.ToString() != "主材")
            {
                clsAtamaTsunagi.m_TsukidashiSt = double.Parse(txtTsukidashiSt.Text);
                clsAtamaTsunagi.m_TsukidashiEd = double.Parse(txtTsukidashiEd.Text);
            }

            if (rbnKuiTenba.Checked == true)
            {
                clsAtamaTsunagi.m_CreateType = CreateType.PileTop;
            }
            else if (rbnFromToP.Checked == true)
            {
                clsAtamaTsunagi.m_CreateType = CreateType.FromToP;
                clsAtamaTsunagi.m_HeightfromToP = double.Parse(txtFromToP.Text);
            }

            // 取付補助材とブラケットの選定
            switch (cmbSteelType.SelectedItem.ToString())
            {
                case "チャンネル":
                    if (clsAtamaTsunagi.m_ChannelDirection == ChannelDirection.DirectionVertical)
                    {
                        if (clsAtamaTsunagi.m_JointType == JointType.Welding)
                        {
                            clsAtamaTsunagi.m_ToritsukeHojozai = L75X75X6X150;
                            clsAtamaTsunagi.m_Bracket = L75X75X6X150;
                        }
                        else if (clsAtamaTsunagi.m_JointType == JointType.Bolt)
                        {
                            clsAtamaTsunagi.m_ToritsukeHojozai = "";
                            clsAtamaTsunagi.m_Bracket = L75X75X6X150;
                        }
                    }
                    else if (clsAtamaTsunagi.m_ChannelDirection == ChannelDirection.DirectionHorizontal)
                    {
                        clsAtamaTsunagi.m_ToritsukeHojozai = L75X75X6X150;
                        clsAtamaTsunagi.m_Bracket = L75X75X6X300;
                    }
                    break;
                case "アングル":
                    clsAtamaTsunagi.m_ToritsukeHojozai = "";
                    clsAtamaTsunagi.m_Bracket = L75X75X6X150;
                    break;
                case "H形鋼 広幅":
                case "主材":
                    if (clsAtamaTsunagi.m_ConfigurationDirection == ConfigurationDirection.Front)
                    {
                        clsAtamaTsunagi.m_ToritsukeHojozai = "";
                        clsAtamaTsunagi.m_Bracket = BL30;
                    }
                    else if (clsAtamaTsunagi.m_ConfigurationDirection == ConfigurationDirection.Back)
                    {
                        clsAtamaTsunagi.m_ToritsukeHojozai = "";
                        clsAtamaTsunagi.m_Bracket = C150X75X6_5;
                    }
                    break;
                default:
                    break;
            }
        }

        #region コントロールイベント

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

        private void cmbSteelType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSteelType.Text == "チャンネル")
            {
                groupBox2.Enabled = true;
            }
            else
            {
                groupBox2.Enabled = false;
            }

            if (cmbSteelType.Text == "チャンネル")
            {
                rbnWelding.Enabled = true;
                if (rbnChannelDirection1.Checked == true)
                {
                    rbnBolt.Enabled = true;
                }
                rbnBurumanC50.Enabled = false;
                rbnRikimanGType.Enabled = false;

                rbnWelding.Checked = true;
            }
            else if (cmbSteelType.Text == "アングル")
            {
                rbnWelding.Enabled = true;
                rbnBolt.Enabled = false;
                rbnBurumanC50.Enabled = false;
                rbnRikimanGType.Enabled = false;
                cmbBoltType.Enabled = false;
                cmbBoltSize.Enabled = false;
                txtBoltNum.Enabled = false;//#33920

                rbnWelding.Checked = true;
            }
            else if (cmbSteelType.Text.Contains("H形鋼") || cmbSteelType.Text == "主材")
            {
                rbnWelding.Enabled = false;
                rbnBolt.Enabled = false;
                rbnBurumanC50.Enabled = true;
                rbnRikimanGType.Enabled = true;

                rbnBurumanC50.Checked = true;
            }

            if (cmbSteelType.Text.Contains("H形鋼") || cmbSteelType.Text == "主材")
            {
                rbnFront.Enabled = true;
                rbnBack.Enabled = true;
            }
            else
            {
                rbnFront.Enabled = false;
                rbnBack.Enabled = false;
            }

            if (cmbSteelType.Text == "主材")
            {
                txtTsukidashiSt.Enabled = false;
                txtTsukidashiEd.Enabled = false;
            }
            else
            {
                txtTsukidashiSt.Enabled = true;
                txtTsukidashiEd.Enabled = true;
            }

            initComboBoxSteelSize();
        }

        private void cmbBoltType_SelectedIndexChanged(object sender, EventArgs e)
        {
            initComboBoxBoltSize();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (groupBox2.Enabled == true)
            {
                rbnChannelDirection1.Checked = true;
                rbnChannelDirection2.Checked = false;
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (groupBox2.Enabled == true)
            {
                rbnChannelDirection1.Checked = false;
                rbnChannelDirection2.Checked = true;
            }
        }

        private void rbnChannelDirection1_CheckedChanged(object sender, EventArgs e)
        {
            rbnBolt.Enabled = true;
            cmbBoltType.Enabled = true;
            cmbBoltSize.Enabled = true;
            txtBoltNum.Enabled = true;
        }

        private void rbnChannelDirection2_CheckedChanged(object sender, EventArgs e)
        {
            rbnWelding.Checked = true;
            rbnBolt.Enabled = false;
            cmbBoltType.Enabled = false;
            cmbBoltSize.Enabled = false;
            txtBoltNum.Enabled = false;//#33920
        }

        private void rbnWelding_CheckedChanged(object sender, EventArgs e)
        {
            cmbBoltType.Enabled = false;
            cmbBoltSize.Enabled = false;
            txtBoltNum.Enabled = false;//#33920
        }

        private void rbnBolt_CheckedChanged(object sender, EventArgs e)
        {
            cmbBoltType.Enabled = true;
            cmbBoltSize.Enabled = true;
            txtBoltNum.Enabled = true;//#33920
        }

        private void rbnBurumanC50_CheckedChanged(object sender, EventArgs e)
        {
            cmbBoltType.Enabled = false;
            cmbBoltSize.Enabled = false;
            txtBoltNum.Enabled = false;//#33920
        }

        private void rbnRikimanGType_CheckedChanged(object sender, EventArgs e)
        {
            cmbBoltType.Enabled = false;
            cmbBoltSize.Enabled = false;
            txtBoltNum.Enabled = false;
        }

        private void rbnKuiTenba_CheckedChanged(object sender, EventArgs e)
        {
            txtFromToP.Enabled = false;
        }

        private void rbnLevelInput_CheckedChanged(object sender, EventArgs e)
        {
            txtFromToP.Enabled = true;
        }

        #endregion

    }
}
