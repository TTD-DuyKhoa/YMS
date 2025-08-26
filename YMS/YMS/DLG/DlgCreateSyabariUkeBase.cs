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
    public partial class DlgCreateSyabariUkeBase : Form
    {
        #region 定数
        /// <summary>
        /// Iniファイルのパス
        /// </summary>
        const string IniPath = "ini\\DlgCreateSyabariUkeBase.ini";

        /// <summary>
        /// Iniファイルのセクション名
        /// </summary>
        const string sec = "DlgCreateSyabariUkeBase";
        #endregion

        #region メンバー
        /// <summary>
        /// 斜梁受けベースクラス
        /// </summary>
        public ClsSyabariUkeBase m_SyabariUkeBase;
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DlgCreateSyabariUkeBase(ClsSyabariUkeBase SyaUkeBase)
        {
            InitializeComponent();
            m_SyabariUkeBase = SyaUkeBase;

            initComboBox();
            SetControl();
            DoHiddenWay(false);
        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DlgCreateSyabariUkeBase( )
        {
            InitializeComponent();
            m_SyabariUkeBase = new ClsSyabariUkeBase();
            initComboBox();
            GetIniData();
            DoHiddenWay();
        }

        public void initComboBox()
        {
            string bk = string.Empty;
            List<string> lstStr = new List<string>();

            //鋼材タイプ
            bk = cmbSteelType.Text;
            cmbSteelType.Items.Clear();
            lstStr = Master.ClsSyabariKouzaiCSV.GetTypeList();

            foreach (string str in lstStr)
            {
                cmbSteelType.Items.Add(str);
            }
            if (lstStr.Contains(bk)) cmbSteelType.Text = bk;
            else cmbSteelType.SelectedIndex = 0;
        }

        public void initComboBoxSteelSize()
        {
            string bk = string.Empty;
            List<string> lstStr = Master.ClsSyabariKouzaiCSV.GetSizeList(cmbSteelType.Text);

            //鋼材サイズ
            bk = cmbSteelSize.Text;
            cmbSteelSize.Items.Clear();

            foreach (string str in lstStr)
            {
                cmbSteelSize.Items.Add(str);
            }
            if (lstStr.Contains(bk)) cmbSteelSize.Text = bk;
            else cmbSteelSize.SelectedIndex = 0;
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
            ClsSyabariUkeBase clsSUB = m_SyabariUkeBase;

            if (rbnShoriTypeReplace.Checked)
                clsSUB.m_ShoriType = ClsSyabariUkeBase.ShoriType.Replace;
            else if (rbnShoriTypePileSelect.Checked)
                clsSUB.m_ShoriType = ClsSyabariUkeBase.ShoriType.PileSelect;
            else
                clsSUB.m_ShoriType = ClsSyabariUkeBase.ShoriType.PtoP;

            clsSUB.m_SteelType = cmbSteelType.Text;
            clsSUB.m_SteelSize = cmbSteelSize.Text;
            clsSUB.m_TsukidashiRyoS = int.Parse(txtTsukidashiRyoS.Text);
            clsSUB.m_TsukidashiRyoE = int.Parse(txtTsukidashiRyoE.Text);
        }

        /// <summary>
        /// コントロールの値をメンバクラスに保持
        /// </summary>
        private void SetControl()
        {
            ClsSyabariUkeBase clsSUB = m_SyabariUkeBase;

            if (clsSUB.m_ShoriType == ClsSyabariUkeBase.ShoriType.Replace)
                rbnShoriTypeReplace.Checked = true;
            else if (clsSUB.m_ShoriType == ClsSyabariUkeBase.ShoriType.PileSelect)
                rbnShoriTypePileSelect.Checked = true;
            else
                rbnShoriTypePtoP.Checked = true;

            cmbSteelType.Text = clsSUB.m_SteelType;
            cmbSteelSize.Text = clsSUB.m_SteelSize;
            txtTsukidashiRyoS.Text = clsSUB.m_TsukidashiRyoS.ToString();
            txtTsukidashiRyoE.Text = clsSUB.m_TsukidashiRyoE.ToString();
        }

        /// <summary>
        /// Iniの情報をコントロールにセット
        /// </summary>
        private void GetIniData()
        {
            try
            {
                string iniPath = ClsIni.GetIniFilePath(IniPath);
                rbnShoriTypeReplace .Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnShoriTypeReplace.Name, iniPath));
                rbnShoriTypePileSelect.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnShoriTypePileSelect.Name, iniPath));
                rbnShoriTypePtoP.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnShoriTypePtoP.Name, iniPath));
                cmbSteelType.Text = ClsIni.GetIniFile(sec, cmbSteelType.Name, iniPath);
                cmbSteelSize.Text = ClsIni.GetIniFile(sec, cmbSteelSize.Name, iniPath);
                txtTsukidashiRyoS.Text = ClsIni.GetIniFile(sec, txtTsukidashiRyoS.Name, iniPath);
                txtTsukidashiRyoE.Text = ClsIni.GetIniFile(sec, txtTsukidashiRyoE.Name, iniPath);
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

            ClsIni.WritePrivateProfileString(sec, rbnShoriTypeReplace.Name, rbnShoriTypeReplace.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnShoriTypePileSelect.Name, rbnShoriTypePileSelect.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnShoriTypePtoP.Name, rbnShoriTypePtoP.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, cmbSteelType.Name, cmbSteelType.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, cmbSteelSize.Name, cmbSteelSize.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, txtTsukidashiRyoS.Name, txtTsukidashiRyoS.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, txtTsukidashiRyoE.Name, txtTsukidashiRyoE.Text, iniPath);
        }

        /// <summary>
        /// 入力チェック
        /// </summary>
        /// <returns></returns>
        public bool CheckInput()
        {
            if (!ClsCommonUtils.ChkContorolTextDouble(txtTsukidashiRyoS, "突き出し量-始点"))
                return false;
            if (!ClsCommonUtils.ChkContorolTextDouble(txtTsukidashiRyoE, "突き出し量-終点"))
                return false;

            return true;
        }
        private void cmbSteelType_SelectedIndexChanged(object sender, EventArgs e)
        {
            initComboBoxSteelSize();
        }

        private void DoHiddenWay(bool bFlag = true)
        {
            rbnShoriTypeReplace.Enabled = bFlag;
            rbnShoriTypePileSelect.Enabled = bFlag;
            rbnShoriTypePtoP.Enabled = bFlag;
        }
    }
}
