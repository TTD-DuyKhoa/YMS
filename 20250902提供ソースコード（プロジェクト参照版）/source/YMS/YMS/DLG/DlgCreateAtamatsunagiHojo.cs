using Autodesk.Revit.DB;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YMS.Parts;

namespace YMS.DLG
{
    public partial class DlgCreateAtamatsunagiHojo : System.Windows.Forms.Form
    {
        #region 定数

        /// <summary>
        /// Iniファイルのパス
        /// </summary>
        const string iniPath = "ini\\DlgCreateAtamatsunagiHojo.ini";

        /// <summary>
        /// Iniファイルのセクション名
        /// </summary>
        const string sec = "DlgCreateAtamatsunagiHojo";

        #endregion

        #region メンバー変数

        public ClsAtamaTsunagi m_ClsAtamaTsunagi;
        public bool m_IsAoutoLayout;            // 自動配置
        public bool m_IsSide;                   // アングル材の向き
        public double m_BracketInterval;        // 配置間隔

        #endregion

        public DlgCreateAtamatsunagiHojo()
        {
            InitializeComponent();
            m_ClsAtamaTsunagi = new ClsAtamaTsunagi();
            initControl();
            getIniData();
        }

        public DlgCreateAtamatsunagiHojo(ClsAtamaTsunagi clsAtamaTsunagi)
        {
            InitializeComponent();
            m_ClsAtamaTsunagi = clsAtamaTsunagi;
            initControl();
            getIniData();
        }

        public void initControl()
        {
            cmbToritsukeHojoZai.Items.Clear();
            cmbToritsukeHojoZai.Items.Add(m_ClsAtamaTsunagi.m_ToritsukeHojozai);
            if (cmbToritsukeHojoZai.Items.Count > 0)
            {
                cmbToritsukeHojoZai.SelectedIndex = 0;
            }

            if (cmbToritsukeHojoZai.Text == "")
            {
                rbnSide.Enabled = false;
                rbnBack.Enabled = false;
            }

            cmbBracket.Items.Clear();
            if ((m_ClsAtamaTsunagi.m_KouzaiType.Contains("H形鋼") || m_ClsAtamaTsunagi.m_KouzaiType == "主材")
                && m_ClsAtamaTsunagi.m_ConfigurationDirection == ClsAtamaTsunagi.ConfigurationDirection.Back)
            {
                cmbBracket.Items.Add(ClsAtamaTsunagi.C150X75X6_5);
                cmbBracket.Items.Add(ClsAtamaTsunagi.C150X75X9);
            }
            else
            {
                cmbBracket.Items.Add(m_ClsAtamaTsunagi.m_Bracket);
            }

            if (cmbBracket.Items.Count > 0)
            {
                cmbBracket.SelectedIndex = 0;
            }

        }

        private void getIniData()
        {
            try
            {
                string path = ClsIni.GetIniFilePath(iniPath);

                rbnManual.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnManual.Name, path));
                rbnAuto.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnAuto.Name, path));

                rbnSide.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnSide.Name, path));
                rbnBack.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnBack.Name, path));

                txtBracketInterval.Text = ClsIni.GetIniFile(sec, txtBracketInterval.Name, path);
            }
            catch
            {
                //MessageBox.Show("前回値の取得に失敗しました");
            }
        }

        private void setIniData()
        {
            string path = ClsIni.GetIniFilePath(iniPath);

            ClsIni.WritePrivateProfileString(sec, rbnManual.Name, rbnManual.Checked.ToString(), path);
            ClsIni.WritePrivateProfileString(sec, rbnAuto.Name, rbnAuto.Checked.ToString(), path);

            ClsIni.WritePrivateProfileString(sec, rbnSide.Name, rbnSide.Checked.ToString(), path);
            ClsIni.WritePrivateProfileString(sec, rbnBack.Name, rbnBack.Checked.ToString(), path);

            ClsIni.WritePrivateProfileString(sec, txtBracketInterval.Name, txtBracketInterval.Text, path);
        }

        public bool checkInput()
        {
            if (!ClsCommonUtils.ChkContorolTextDouble(txtBracketInterval, "ブラケット配置間隔", bMinus: true))
            {
                return false;
            }

            return true;
        }

        public void setData()
        {
            if (rbnAuto.Checked)
            {
                m_IsAoutoLayout = true;
            }
            else
            {
                m_IsAoutoLayout = false;
            }

            if (rbnSide.Checked)
            {
                m_IsSide = true;
            }
            else
            {
                m_IsSide = false;
            }

            if (double.TryParse(txtBracketInterval.Text, out double res))
            {
                m_BracketInterval = res;
            }
            else
            {
                m_BracketInterval = 0.0;
            }

        }

        private void rbnManual_CheckedChanged(object sender, EventArgs e)
        {
            txtBracketInterval.Enabled = false;
        }

        private void rbnAuto_CheckedChanged(object sender, EventArgs e)
        {
            txtBracketInterval.Enabled = true;
        }

        private void btnYoshikiKakunin_Click(object sender, EventArgs e)
        {
            string symbolFolpath = Parts.ClsZumenInfo.GetYMSFolder();
            string fileName = "PDF_Atamatsunagi.pdf";
            string filePath = System.IO.Path.Combine(symbolFolpath, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                MessageBox.Show("PDFファイルが見つかりません");
                return;
            }

            var startInfo = new System.Diagnostics.ProcessStartInfo()
            {
                FileName = filePath,
                UseShellExecute = true,
                CreateNoWindow = true,
            };
            System.Diagnostics.Process.Start(startInfo);
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

    }
}
