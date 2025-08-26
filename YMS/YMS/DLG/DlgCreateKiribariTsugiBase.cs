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
    public partial class DlgCreateKiribariTsugiBase : Form
    {
        #region 定数
        /// <summary>
        /// Iniファイルのパス
        /// </summary>
        const string IniPath = "ini\\DlgCreateKiribariTsugiBase.ini";

        /// <summary>
        /// Iniファイルのセクション名
        /// </summary>
        const string sec = "DlgCreateKiribariTsugiBase";
        #endregion

        #region メンバー
        /// <summary>
        /// 切梁受けベースクラス
        /// </summary>
        public ClsKiribariTsugiBase m_KiribariTsugiBase;
        #endregion

        #region コンストラクタ
        public DlgCreateKiribariTsugiBase(ClsKiribariTsugiBase KiribariTsugiBase)
        {
            InitializeComponent();
            m_KiribariTsugiBase = KiribariTsugiBase;
            initComboBox();
            SetControl();
            groupBox4.Enabled = false;
        }
        public DlgCreateKiribariTsugiBase()
        {
            InitializeComponent();
            m_KiribariTsugiBase = new ClsKiribariTsugiBase();
            initComboBox();
            GetIniData();
            groupBox4.Enabled = true;
        }
        #endregion


        public void initComboBox()
        {
            string bk = string.Empty;
            List<string> lstStr = new List<string>();

            //鋼材タイプ
            bk = cmbSteelType.Text;
            cmbSteelType.Items.Clear();
            lstStr = new List<string>() { Master.ClsYamadomeCsv.TypeShuzai,
                                          Master.ClsHBeamCsv.TypeHiro};
            foreach (string str in lstStr)
            {
                cmbSteelType.Items.Add(str);
            }
            if (lstStr.Contains(bk)) cmbSteelType.Text = bk;
            else cmbSteelType.SelectedIndex = 0;

            //切梁側部品
            bk = cmbKiriSideParts.Text;
            cmbKiriSideParts.Items.Clear();
            lstStr = Master.ClsHiuchiCsv.GetTypeListKiribariTsunagiKiriSide();
            foreach (string str in lstStr)
            {
                cmbKiriSideParts.Items.Add(str);
            }
            if (lstStr.Contains(bk)) cmbKiriSideParts.Text = bk;
            else cmbKiriSideParts.SelectedIndex = 0;

            //腹起側部品
            bk = cmbHaraSideParts.Text;
            cmbHaraSideParts.Items.Clear();
            lstStr = Master.ClsHiuchiCsv.GetTypeListKiribariTsunagiHaraSide();
            foreach (string str in lstStr)
            {
                cmbHaraSideParts.Items.Add(str);
            }
            if (lstStr.Contains(bk)) cmbHaraSideParts.Text = bk;
            else cmbHaraSideParts.SelectedIndex = 0;

        }

        public void InitComboBoxSteelSize()
        {
            string selectedText = string.Empty;

            cmbSteelSizeSingle.Items.Clear();
            cmbHaraSideSteelSizeDouble.Items.Clear();
            cmbKiriSideSteelSizeDouble.Items.Clear();

            if (cmbSteelType != null && cmbSteelType.Text != null)
            {
                selectedText = cmbSteelSizeSingle.Text;

                List<string> sizeList = new List<string>();
                if (cmbSteelType.Text == Master.ClsYamadomeCsv.TypeShuzai)
                {
                    sizeList = Master.ClsYamadomeCsv.GetSizeList(cmbSteelType.Text);
                }
                else
                {
                    sizeList = Master.ClsHBeamCsv.GetSizeList(cmbSteelType.Text);
                }

                foreach (string str in sizeList)
                {
                    cmbSteelSizeSingle.Items.Add(str);
                    cmbHaraSideSteelSizeDouble.Items.Add(str);
                    cmbKiriSideSteelSizeDouble.Items.Add(str);
                }

                if (!string.IsNullOrEmpty(selectedText) && sizeList.Contains(selectedText))
                {
                    cmbSteelSizeSingle.Text = selectedText;
                    cmbHaraSideSteelSizeDouble.Text = selectedText;
                    cmbKiriSideSteelSizeDouble.Text = selectedText;
                }
                else
                {
                    cmbSteelSizeSingle.SelectedIndex = cmbSteelSizeSingle.Items.Count > 0 ? 0 : -1;
                    cmbHaraSideSteelSizeDouble.SelectedIndex = cmbHaraSideSteelSizeDouble.Items.Count > 0 ? 0 : -1;
                    cmbKiriSideSteelSizeDouble.SelectedIndex = cmbKiriSideSteelSizeDouble.Items.Count > 0 ? 0 : -1;
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
            ClsKiribariTsugiBase clsKTB = m_KiribariTsugiBase;
            clsKTB.m_SteelType = cmbSteelType.Text;

            if (rbnShoriTypePoint.Checked)
                clsKTB.m_ShoriType = ClsKiribariTsugiBase.ShoriType.Point;
            else if (rbnShoriTypeSanjikuPeace.Checked)
                clsKTB.m_ShoriType = ClsKiribariTsugiBase.ShoriType.SanjikuPiece;
            else
                clsKTB.m_ShoriType = ClsKiribariTsugiBase.ShoriType.Replace;

            if (rbnKoseSingle.Checked)
                clsKTB.m_Kousei = ClsKiribariTsugiBase.Kousei.Single;
            else
                clsKTB.m_Kousei = ClsKiribariTsugiBase.Kousei.Double;

            clsKTB.m_SteelSizeSingle = cmbSteelSizeSingle.Text;

            clsKTB.m_KiriSideSteelSizeDouble = cmbKiriSideSteelSizeDouble.Text;
            clsKTB.m_KiriSideTsunagiLength = int.Parse(txtKiriSideTsugiLength.Text);
            clsKTB.m_KiriSideParts = cmbKiriSideParts.Text;

            clsKTB.m_HaraSideSteelSizeDouble = cmbHaraSideSteelSizeDouble.Text;
            clsKTB.m_HaraSideTsunagiLength = int.Parse(txtHaraSideTsugiLength.Text);
            clsKTB.m_HaraSideParts = cmbHaraSideParts.Text;
        }

        /// <summary>
        /// コントロールの値をメンバクラスに保持
        /// </summary>
        private void SetControl()
        {
            ClsKiribariTsugiBase clsKTB = m_KiribariTsugiBase;
            cmbSteelType.Text = clsKTB.m_SteelType;

            if (clsKTB.m_ShoriType == ClsKiribariTsugiBase.ShoriType.Point)
                rbnShoriTypePoint.Checked = true;
            else if (clsKTB.m_ShoriType == ClsKiribariTsugiBase.ShoriType.SanjikuPiece)
                rbnShoriTypeSanjikuPeace.Checked = true;
            else
                rbnShoriTypeReplace.Checked = true;

            if (clsKTB.m_Kousei == ClsKiribariTsugiBase.Kousei.Single)
                rbnKoseSingle.Checked = true;
            else
                rbnKoseDouble.Checked = true;

            cmbSteelSizeSingle.Text = clsKTB.m_SteelSizeSingle;

            cmbKiriSideSteelSizeDouble.Text = clsKTB.m_KiriSideSteelSizeDouble;
            txtKiriSideTsugiLength.Text = clsKTB.m_KiriSideTsunagiLength.ToString();
            cmbKiriSideParts.Text = clsKTB.m_KiriSideParts;

            cmbHaraSideSteelSizeDouble.Text = clsKTB.m_HaraSideSteelSizeDouble;
            txtHaraSideTsugiLength.Text = clsKTB.m_HaraSideTsunagiLength.ToString();
            cmbHaraSideParts.Text = clsKTB.m_HaraSideParts;
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
                rbnShoriTypePoint.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnShoriTypePoint.Name, iniPath));
                rbnShoriTypeSanjikuPeace.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnShoriTypeSanjikuPeace.Name, iniPath));
                rbnShoriTypeReplace.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnShoriTypeReplace.Name, iniPath));
                rbnKoseSingle.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnKoseSingle.Name, iniPath));
                rbnKoseDouble.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnKoseDouble.Name, iniPath));
                cmbSteelSizeSingle.Text = ClsIni.GetIniFile(sec, cmbSteelSizeSingle.Name, iniPath);
                cmbKiriSideSteelSizeDouble.Text = ClsIni.GetIniFile(sec, cmbKiriSideSteelSizeDouble.Name, iniPath);
                txtKiriSideTsugiLength.Text = ClsIni.GetIniFile(sec, txtKiriSideTsugiLength.Name, iniPath);
                cmbKiriSideParts.Text = ClsIni.GetIniFile(sec, cmbKiriSideParts.Name, iniPath);
                cmbHaraSideSteelSizeDouble.Text = ClsIni.GetIniFile(sec, cmbHaraSideSteelSizeDouble.Name, iniPath);
                txtHaraSideTsugiLength.Text = ClsIni.GetIniFile(sec, txtHaraSideTsugiLength.Name, iniPath);
                cmbHaraSideParts.Text = ClsIni.GetIniFile(sec, cmbHaraSideParts.Name, iniPath);
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
            ClsIni.WritePrivateProfileString(sec, cmbSteelType.Name, cmbSteelType.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnShoriTypePoint.Name, rbnShoriTypePoint.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnShoriTypeSanjikuPeace.Name, rbnShoriTypeSanjikuPeace.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnShoriTypeReplace.Name, rbnShoriTypeReplace.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnKoseSingle.Name, rbnKoseSingle.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnKoseDouble.Name, rbnKoseDouble.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, cmbSteelSizeSingle.Name, cmbSteelSizeSingle.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, cmbKiriSideSteelSizeDouble.Name, cmbKiriSideSteelSizeDouble.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, txtKiriSideTsugiLength.Name, txtKiriSideTsugiLength.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, cmbKiriSideParts.Name, cmbKiriSideParts.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, cmbHaraSideSteelSizeDouble.Name, cmbHaraSideSteelSizeDouble.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, txtHaraSideTsugiLength.Name, txtHaraSideTsugiLength.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, cmbHaraSideParts.Name, cmbHaraSideParts.Text, iniPath);
        }

        private void rbnPartsTypeKiriKiri_CheckedChanged(object sender, EventArgs e)
        {
            chkKiribari_Haraokoshi();
        }

        private void rbnPartsTypeKiriHara_CheckedChanged(object sender, EventArgs e)
        {
            chkKiribari_Haraokoshi();
        }

        private void rbnKoseSingle_CheckedChanged(object sender, EventArgs e)
        {
            chkKousei();
        }

        private void rbnKoseDouble_CheckedChanged(object sender, EventArgs e)
        {
            chkKousei();
        }

        private void chkKiribari_Haraokoshi()
        {
            //if (rbnPartsTypeKiriHara.Checked == true)
            //{
            //    grpKiribariHaraokoshi.Enabled = true;
            //}
            //else
            //{
            //    grpKiribariHaraokoshi.Enabled = false;
            //}
        }

        private void chkKousei()
        {
            if(rbnKoseSingle.Checked == true)
            {
                cmbSteelSizeSingle.Enabled= true;

                cmbKiriSideSteelSizeDouble.Enabled = false;
                txtKiriSideTsugiLength.Enabled = false;
                cmbHaraSideSteelSizeDouble.Enabled = false;
                txtHaraSideTsugiLength.Enabled = false;
            }
            else if (rbnKoseDouble.Checked == true)
            {
                cmbSteelSizeSingle.Enabled = false;

                cmbKiriSideSteelSizeDouble.Enabled = true;
                txtKiriSideTsugiLength.Enabled = true;
                cmbHaraSideSteelSizeDouble.Enabled = true;
                txtHaraSideTsugiLength.Enabled = true;
            }
        }

        private void cmbSteelType_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitComboBoxSteelSize();
        }

        /// <summary>
        /// 入力チェック
        /// </summary>
        /// <returns></returns>
        public bool CheckInput()
        {
            if (!ClsCommonUtils.ChkContorolTextDouble(txtKiriSideTsugiLength, "切梁側：切梁繋ぎ長さ", bMinus: true))
                return false;
            if (!ClsCommonUtils.ChkContorolTextDouble(txtHaraSideTsugiLength, "腹起側：切梁繋ぎ長さ", bMinus: true))
                return false;

            return true;
        }
    }
}
