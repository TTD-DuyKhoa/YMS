using Autodesk.Revit.DB;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YMS.Parts;

namespace YMS.DLG
{
    public partial class DlgCreateSendanBoltHokyozai : System.Windows.Forms.Form
    {
        #region 定数

        /// <summary>
        /// Iniファイルのパス
        /// </summary>
        const string iniPath = "ini\\DlgCreateSendanBoltHokyozai.ini";

        /// <summary>
        /// Iniファイルのセクション名
        /// </summary>
        const string sec = "DlgCreateSendanBoltHokyozai";

        #endregion

        #region メンバー変数

        public Document m_doc;
        public ClsSendanBoltHokyouzai m_clsSendanBoltHokyouzai;

        #endregion

        public DlgCreateSendanBoltHokyozai(Document doc)
        {
            InitializeComponent();
            m_doc = doc;
            m_clsSendanBoltHokyouzai = new ClsSendanBoltHokyouzai();
            InitControl();
            GetIniData();
        }

        public void InitControl()
        {
            InitComboBoxSteelType();
            InitComboBoxSteelSize();

            InitComboBoxBoltType();
            InitComboBoxBoltSize();
            txtBoltNum.Text = "0";
        }

        public void InitComboBoxSteelType()
        {
            // 鋼材タイプ
            cmbSteelType.Items.Clear();
            cmbSteelType.Items.Add("せん断ボルト補強材");
            cmbSteelType.SelectedIndex = 0;
        }

        public void InitComboBoxSteelSize()
        {
            // 鋼材サイズ
            if (cmbSteelSize != null)
            {
                string selectedText = cmbSteelSize.Text;
                cmbSteelSize.Items.Clear();
                List<string> lstStr = Master.ClsSendanBoltHokyouzaiCsv.GetSizeList();
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

        public void InitComboBoxBoltType()
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

        }

        public void InitComboBoxBoltSize()
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

        private void GetIniData()
        {
            try
            {
                string path = ClsIni.GetIniFilePath(iniPath);

                cmbSteelType.Text = ClsIni.GetIniFile(sec, cmbSteelType.Name, path);
                cmbSteelSize.Text = ClsIni.GetIniFile(sec, cmbSteelSize.Name, path);

                cmbBoltType.Text = ClsIni.GetIniFile(sec, cmbBoltType.Name, path);
                cmbBoltSize.Text = ClsIni.GetIniFile(sec, cmbBoltSize.Name, path);
                txtBoltNum.Text = ClsIni.GetIniFile(sec, txtBoltNum.Name, path);
            }
            catch (Exception)
            {
                //MessageBox.Show("前回値の取得に失敗しました");
            }
        }

        private void SetIniData()
        {
            string path = ClsIni.GetIniFilePath(iniPath);

            ClsIni.WritePrivateProfileString(sec, cmbSteelType.Name, cmbSteelType.Text, path);
            ClsIni.WritePrivateProfileString(sec, cmbSteelSize.Name, cmbSteelSize.Text, path);

            ClsIni.WritePrivateProfileString(sec, cmbBoltType.Name, cmbBoltType.Text, path);
            ClsIni.WritePrivateProfileString(sec, cmbBoltSize.Name, cmbBoltSize.Text, path);
            ClsIni.WritePrivateProfileString(sec, txtBoltNum.Name, txtBoltNum.Text, path);
        }

        public bool CheckInput()
        {
            if (!ClsCommonUtils.ChkContorolTextDouble(txtBoltNum, "ボルト本数", bMinus: true, bZero: true, bComma: true))
            {
                return false;
            }

            return true;
        }

        private void SetData()
        {
            ClsSendanBoltHokyouzai clsSendanBoltHokyouzai = m_clsSendanBoltHokyouzai;

            clsSendanBoltHokyouzai.m_KouzaiType = cmbSteelType.SelectedItem.ToString();
            clsSendanBoltHokyouzai.m_KouzaiSize = cmbSteelSize.SelectedItem.ToString();

            clsSendanBoltHokyouzai.m_BoltType = cmbBoltType.SelectedItem.ToString();
            clsSendanBoltHokyouzai.m_BoltSize = cmbBoltSize.SelectedItem.ToString();
            clsSendanBoltHokyouzai.m_BoltNum = int.Parse(txtBoltNum.Text);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!CheckInput())
            {
                return;
            }

            SetData();
            SetIniData();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnHint_Click(object sender, EventArgs e)
        {
            string symbolFolpath = Parts.ClsZumenInfo.GetYMSFolder();
            string fileName = "PDF_SendanBoltHokyozai.pdf";
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

        private void cmbBoltType_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitComboBoxBoltSize();
        }
    }
}
