using System;
using System.Collections.Generic;
using System.Windows.Forms;
using YMS.Parts;

namespace YMS.DLG
{
    public partial class DlgCreateKiribariBase : Form
    {
        #region 定数

        /// <summary>
        /// Iniファイルのパス
        /// </summary>
        const string IniPath = "ini\\DlgCreateKiribariBase.ini";

        /// <summary>
        /// Iniファイルのセクション名
        /// </summary>
        const string sec = "DlgCreateKiribariBase";

        #endregion


        #region メンバー

        public ClsKiribariBase m_ClsKiribariBase;

        #endregion

        public DlgCreateKiribariBase(ClsKiribariBase clsKiriBase)
        {
            InitializeComponent();
            InitControl();
            m_ClsKiribariBase = clsKiriBase;

            SetControl();
        }
        public DlgCreateKiribariBase()
        {
            InitializeComponent();
            InitControl();
            m_ClsKiribariBase = new ClsKiribariBase();
            GetIniData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
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

        private void DlgCreateKiribari_Load(object sender, EventArgs e)
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

            //端部部品(始点側)
            bk = cmbTanbuPartsS.Text;
            cmbTanbuPartsS.Items.Clear();
            lstStr = Master.ClsHiuchiCsv.GetTypeListKiribari();
            foreach (string str in lstStr)
            {
                cmbTanbuPartsS.Items.Add(str);
            }
            if (lstStr.Contains(bk)) cmbTanbuPartsS.Text = bk;
            else cmbTanbuPartsS.SelectedIndex = 0;

            //端部部品(終点側)
            bk = cmbTanbuPartsE.Text;
            cmbTanbuPartsE.Items.Clear();
            lstStr = Master.ClsHiuchiCsv.GetTypeListKiribari();
            foreach (string str in lstStr)
            {
                cmbTanbuPartsE.Items.Add(str);
            }
            if (lstStr.Contains(bk)) cmbTanbuPartsE.Text = bk;
            else cmbTanbuPartsE.SelectedIndex = 0;

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
            ClsKiribariBase clsKB = m_ClsKiribariBase;
            clsKB.m_kouzaiType = cmbSteelType.Text;
            clsKB.m_kouzaiSize = cmbSteelSize.Text;

            if (rbnShoriTypeBaseLine.Checked)
            {
                clsKB.m_ShoriType = ClsKiribariBase.ShoriType.BaseLine;
            }
            else
            {
                clsKB.m_ShoriType = ClsKiribariBase.ShoriType.PtoP;
            }

            clsKB.m_tanbuStart = cmbTanbuPartsS.Text;
            clsKB.m_tanbuEnd = cmbTanbuPartsE.Text;
            clsKB.m_jack1 = cmbJackType1.Text;
            clsKB.m_jack2 = cmbJackType2.Text;
        }

        /// <summary>
        /// コントロールの値をメンバクラスに保持
        /// </summary>
        private void SetControl()
        {
            ClsKiribariBase clsKB = m_ClsKiribariBase;
            cmbSteelType.Text = clsKB.m_kouzaiType;
            cmbSteelSize.Text = clsKB.m_kouzaiSize;

            if (clsKB.m_ShoriType == ClsKiribariBase.ShoriType.BaseLine)
            {
                rbnShoriTypeBaseLine.Checked = true;
            }
            else
            {
                rbnShoriTypePtoP.Checked = true;
            }

            cmbTanbuPartsS.Text = clsKB.m_tanbuStart;
            cmbTanbuPartsE.Text = clsKB.m_tanbuEnd;
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
                cmbTanbuPartsS.Text = ClsIni.GetIniFile(sec, cmbTanbuPartsS.Name, iniPath);
                cmbTanbuPartsE.Text = ClsIni.GetIniFile(sec, cmbTanbuPartsE.Name, iniPath);
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
            ClsIni.WritePrivateProfileString(sec, cmbTanbuPartsS.Name, cmbTanbuPartsS.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, cmbTanbuPartsE.Name, cmbTanbuPartsE.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, cmbJackType1.Name, cmbJackType1.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, cmbJackType2.Name, cmbJackType2.Text, iniPath);
        }

        private void cmbSteelType_SelectedIndexChanged(object sender, EventArgs e)
        {
            initComboBoxSteelSize();
        }
    }
}
