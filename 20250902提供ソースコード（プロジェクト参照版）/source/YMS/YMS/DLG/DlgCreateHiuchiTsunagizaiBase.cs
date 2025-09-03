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

namespace YMS.DLG
{
    public partial class DlgCreateHiuchiTsunagizaiBase : Form
    {
        #region 定数
        /// <summary>
        /// Iniファイルのパス
        /// </summary>
        const string IniPath = "ini\\DlgCreateHiuchiTsunagizaiBase.ini";

        /// <summary>
        /// Iniファイルのセクション名
        /// </summary>
        const string sec = "DlgCreateHiuchiTsunagizaiBase";
        #endregion

        #region メンバー
        /// <summary>
        /// 火打ちツナギ材ベースクラス
        /// </summary>
        public ClsHiuchiTsunagizaiBase m_HiuchiTsunagizaiBase;
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="kiriHiuchiBase"></param>
        public DlgCreateHiuchiTsunagizaiBase(ClsHiuchiTsunagizaiBase hiuchiTsunagizaiBase)
        {
            InitializeComponent();
            m_HiuchiTsunagizaiBase = hiuchiTsunagizaiBase;
            initComboBox();
            SetContlol();
            groupBox3.Enabled = false;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="kiriHiuchiBase"></param>
        public DlgCreateHiuchiTsunagizaiBase()
        {
            InitializeComponent();
            m_HiuchiTsunagizaiBase = new ClsHiuchiTsunagizaiBase();
            initComboBox();
            GetIniData();
            groupBox3.Enabled = true;
        }

        public void initComboBox()
        {
            if (cmbSteelType == null || cmbBoltType1 == null || cmbBoltType2 == null)
            {
                // Null チェックのためのエラーハンドリングまたは処理を記述
                return;
            }

            string selectedSteelType = cmbSteelType.Text;
            cmbSteelType.Items.Clear();
            List<string> steelTypes = new List<string>() { Master.ClsYamadomeCsv.TypeShuzai,
                                                           Master.ClsHBeamCsv.TypeHiro,
                                                           Master.ClsHBeamCsv.TypeNaka,
                                                           Master.ClsHBeamCsv.TypeHoso,
                                                           Master.ClsAngleCsv.TypeAngle,
                                                           Master.ClsAngleCsv.TypeFutohenAngle,
                                                           Master.ClsChannelCsv.TypeCannel};

            cmbSteelType.Items.AddRange(steelTypes.ToArray());

            if (cmbSteelType.Items.Count > 0)
            {
                if (steelTypes.Contains(selectedSteelType))
                {
                    cmbSteelType.Text = selectedSteelType;
                }
                else
                {
                    cmbSteelType.SelectedIndex = 0;
                }
            }

            // 鋼材サイズ
            initComboBoxSteelSize();

            // ボルト種類
            string selectedBoltType = cmbBoltType1.Text;
            cmbBoltType1.Items.Clear();
            List<string> boltTypes = Master.ClsBoltCsv.GetTypeList();
            cmbBoltType1.Items.AddRange(boltTypes.ToArray());

            if (cmbBoltType1.Items.Count > 0)
            {
                if (boltTypes.Contains(selectedBoltType))
                {
                    cmbBoltType1.Text = selectedBoltType;
                }
                else
                {
                    cmbBoltType1.SelectedIndex = 0;
                }
            }

            // ボルトタイプ
            initComboBoxSteelSize();
        }

        public void initComboBoxSteelSize()
        {
            // Null チェック
            if (cmbSteelType == null)
            {
                // エラーハンドリングまたは処理を行う
                return;
            }

            string selectedText = cmbSteelSize.Text;
            cmbSteelSize.Items.Clear();

            List<string> lstStr = new List<string>();

            switch (cmbSteelType.Text)
            {
                case Master.ClsYamadomeCsv.TypeShuzai:
                    lstStr = Master.ClsYamadomeCsv.GetSizeList(cmbSteelType.Text);
                    break;

                case Master.ClsHBeamCsv.TypeHiro:
                case Master.ClsHBeamCsv.TypeNaka:
                case Master.ClsHBeamCsv.TypeHoso:
                    lstStr = Master.ClsHBeamCsv.GetSizeList(cmbSteelType.Text);
                    break;

                case Master.ClsAngleCsv.TypeAngle:
                case Master.ClsAngleCsv.TypeFutohenAngle:
                    lstStr = Master.ClsAngleCsv.GetSizeList(cmbSteelType.Text);
                    break;

                default:
                    lstStr = Master.ClsChannelCsv.GetSizeList();
                    break;
            }

            cmbSteelSize.Items.AddRange(lstStr.ToArray());

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

        public void initComboBoxBoltType()
        {
            if (cmbBoltType1 == null || cmbBoltType2 == null)
            {
                return;
            }

            string selectedText = cmbBoltType2.Text;
            cmbBoltType2.Items.Clear();

            List<string> lstStr = Master.ClsBoltCsv.GetSizeList(cmbBoltType1.Text);

            cmbBoltType2.Items.AddRange(lstStr.ToArray());

            if (cmbBoltType2.Items.Count > 0)
            {
                if (lstStr.Contains(selectedText))
                {
                    cmbBoltType2.Text = selectedText;
                }
                else
                {
                    cmbBoltType2.SelectedIndex = 0;
                }
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
            ClsHiuchiTsunagizaiBase clsHTB = m_HiuchiTsunagizaiBase;
            clsHTB.m_SteelType = cmbSteelType.Text;
            clsHTB.m_SteelSize = cmbSteelSize.Text;
            clsHTB.m_SplitFlg = chkSplit.Checked;

            if (rbnToritsukeBolt.Checked)
                clsHTB.m_ToritsukeHoho = ClsHiuchiTsunagizaiBase.ToritsukeHoho.Bolt;
            else if (rbnToritsukeBuruman.Checked)
                clsHTB.m_ToritsukeHoho = ClsHiuchiTsunagizaiBase.ToritsukeHoho.Buruman;
            else
                clsHTB.m_ToritsukeHoho = ClsHiuchiTsunagizaiBase.ToritsukeHoho.Rikiman;

            if (rbnShoriTypeReplace.Checked)
                clsHTB.m_ShoriType = ClsHiuchiTsunagizaiBase.ShoriType.Replace;
            else
                clsHTB.m_ShoriType = ClsHiuchiTsunagizaiBase.ShoriType.Manual;

            clsHTB.m_BoltType1 = cmbBoltType1.Text;
            clsHTB.m_BoltType2 = cmbBoltType2.Text;
            clsHTB.m_BoltNum = ClsCommonUtils.ChangeStrToInt(txtBoltNum.Text);
        }

        /// <summary>
        /// コントロールの値をメンバクラスに保持
        /// </summary>
        private void SetContlol()
        {
            ClsHiuchiTsunagizaiBase clsHTB = m_HiuchiTsunagizaiBase;
            cmbSteelType.Text = clsHTB.m_SteelType;
            cmbSteelSize.Text = clsHTB.m_SteelSize;
            chkSplit.Checked = clsHTB.m_SplitFlg;

            if (clsHTB.m_ToritsukeHoho == ClsHiuchiTsunagizaiBase.ToritsukeHoho.Bolt)
                rbnToritsukeBolt.Checked = true;
            else if (clsHTB.m_ToritsukeHoho == ClsHiuchiTsunagizaiBase.ToritsukeHoho.Buruman)
                rbnToritsukeBuruman.Checked = true;
            else
                rbnToritsukeRikiman.Checked = true;

            if (clsHTB.m_ShoriType == ClsHiuchiTsunagizaiBase.ShoriType.Replace)
                rbnShoriTypeReplace.Checked = true;
            else
                rbnShoriTypeManual.Checked = true;

            cmbBoltType1.Text = clsHTB.m_BoltType1;
            cmbBoltType2.Text = clsHTB.m_BoltType2;
            txtBoltNum.Text = clsHTB.m_BoltNum.ToString();
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
                chkSplit.Checked = bool.Parse(ClsIni.GetIniFile(sec, chkSplit.Name, iniPath));
                rbnToritsukeBolt.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnToritsukeBolt.Name, iniPath));
                rbnToritsukeBuruman.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnToritsukeBuruman.Name, iniPath));
                rbnToritsukeRikiman.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnToritsukeRikiman.Name, iniPath));
                rbnShoriTypeReplace.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnShoriTypeReplace.Name, iniPath));
                rbnShoriTypeManual.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnShoriTypeManual.Name, iniPath));
                cmbBoltType1.Text = ClsIni.GetIniFile(sec, cmbBoltType1.Name, iniPath);
                cmbBoltType2.Text = ClsIni.GetIniFile(sec, cmbBoltType2.Name, iniPath);
                txtBoltNum.Text = ClsIni.GetIniFile(sec, txtBoltNum.Name, iniPath);
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
            ClsIni.WritePrivateProfileString(sec, cmbSteelType.Name, cmbSteelType.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, cmbSteelSize.Name, cmbSteelSize.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, chkSplit.Name, chkSplit.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnToritsukeBolt.Name, rbnToritsukeBolt.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnToritsukeBuruman.Name, rbnToritsukeBuruman.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnToritsukeRikiman.Name, rbnToritsukeRikiman.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnShoriTypeReplace.Name, rbnShoriTypeReplace.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnShoriTypeManual.Name, rbnShoriTypeManual.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, cmbBoltType1.Name, cmbBoltType1.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, cmbBoltType2.Name, cmbBoltType2.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, txtBoltNum.Name, txtBoltNum.Text, iniPath);
        }

        private void rbnToritsukeBolt_CheckedChanged(object sender, EventArgs e)
        {
            chkToritsukeHouhou();
        }

        private void rbnToritsukeBuruman_CheckedChanged(object sender, EventArgs e)
        {
            chkToritsukeHouhou();
        }

        private void rbnToritsukeRikiman_CheckedChanged(object sender, EventArgs e)
        {
            chkToritsukeHouhou();
        }

        private void cmbSteelType_SelectedIndexChanged(object sender, EventArgs e)
        {
            initComboBoxSteelSize();
        }

        private void cmbBoltType1_SelectedIndexChanged(object sender, EventArgs e)
        {
            initComboBoxBoltType();
        }

        private void chkToritsukeHouhou()
        {
            if (rbnToritsukeBolt.Checked == true)
            {
                cmbBoltType1.Enabled = true;
                cmbBoltType2.Enabled = true;
                txtBoltNum.Enabled = true;
            }
            else
            {
                cmbBoltType1.Enabled = false;
                cmbBoltType2.Enabled = false;
                txtBoltNum.Enabled = false;
            }
        }


        /// <summary>
        /// 入力チェック
        /// </summary>
        /// <returns></returns>
        public bool CheckInput()
        {
            if (!ClsCommonUtils.ChkContorolTextDouble(txtBoltNum, "ボルト本数", bZero: true, bMinus: true, bComma: true))
                return false;

            return true;
        }
    }
}
