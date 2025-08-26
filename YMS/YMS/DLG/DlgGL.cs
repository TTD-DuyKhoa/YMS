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

namespace YMS.DLG
{
    public partial class DlgGL : System.Windows.Forms.Form
    {
        #region 定数
        /// <summary>
        /// Iniファイルのパス
        /// </summary>
        const string IniPath = "ini\\DlgGL.ini";

        /// <summary>
        /// Iniファイルのセクション名
        /// </summary>
        const string sec = "DlgGL";
        #endregion
        #region メンバー
        public Document m_doc;
        public string m_Level { get; set; }
        #endregion
        public DlgGL(Document doc)
        {
            InitializeComponent();
            m_doc = doc;
            m_Level = string.Empty;

            InitControl();
            GetIniData();
        }
        public void InitControl()
        {
            //GLレベル
            cmbLevel.Items.Clear();
            cmbLevel.Items.AddRange(ClsYMSUtil.GetLevelNames(m_doc).ToArray());
            cmbLevel.SelectedIndex = 0;
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!CheckInput())
                return;

            SetIniData();
            m_Level = cmbLevel.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public bool CheckInput()
        {
            if (!ClsCommonUtils.ChkContorolCommbo(cmbLevel, "配置レベル", true))
                return false;

            return true;
        }

        /// <summary>
        /// Iniの情報をコントロールにセット
        /// </summary>
        private void GetIniData()
        {
            try
            {
                string iniPath = ClsIni.GetIniFilePath(IniPath);

                cmbLevel.Text = ClsIni.GetIniFile(sec, cmbLevel.Name, iniPath);
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

            ClsIni.WritePrivateProfileString(sec, cmbLevel.Name, cmbLevel.Text, iniPath);
        }
    }
}
