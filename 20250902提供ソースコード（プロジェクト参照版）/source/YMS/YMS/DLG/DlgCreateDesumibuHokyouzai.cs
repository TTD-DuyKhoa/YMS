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
    public partial class DlgCreateDesumibuHokyouzai : Form
    {
        #region 定数
        /// <summary>
        /// Iniファイルのパス
        /// </summary>
        const string IniPath = "ini\\DlgCreateDesumibuHokyouzai.ini";

        /// <summary>
        /// Iniファイルのセクション名
        /// </summary>
        const string sec = "DlgDesumibuHokyouzai";
        #endregion

        public ClsDesumibuHokyouzai m_clsDesumibuHokyouzai;

        public DlgCreateDesumibuHokyouzai()
        {
            InitializeComponent();
            m_clsDesumibuHokyouzai = new ClsDesumibuHokyouzai();
            GetIniData();

        }

        public DlgCreateDesumibuHokyouzai(ClsDesumibuHokyouzai clsDesumibuHokyouzai)
        {
            InitializeComponent();
            m_clsDesumibuHokyouzai = clsDesumibuHokyouzai;
            GetIniData();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!CheckInput())
                return;

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

        public void SetData()
        {
            ClsDesumibuHokyouzai cdh = m_clsDesumibuHokyouzai;
            cdh.m_Top = ClsCommonUtils.ChangeStrToInt(txtToritsukeLevel.Text);
            if (0 > cdh.m_Top)
                cdh.m_Top *= -1;
            cdh.m_Pitch = ClsCommonUtils.ChangeStrToInt(txtPitch.Text);

            m_clsDesumibuHokyouzai = cdh;
        }

        /// <summary>
        /// 入力チェック
        /// </summary>
        /// <returns></returns>
        public bool CheckInput()
        {
            if (!ClsCommonUtils.ChkContorolTextDouble(txtToritsukeLevel, "取付レベル（杭天端 －）"))
                return false;
            if (!ClsCommonUtils.ChkContorolTextDouble(txtPitch, "ピッチ", bMinus: true))
                return false;
            
            return true;
        }

        /// <summary>
        /// ダイアログデータをini二セット
        /// </summary>
        public void SetIniData()
        {
            string iniPath = ClsIni.GetIniFilePath(IniPath);

            ClsIni.WritePrivateProfileString(sec, txtToritsukeLevel.Name, txtToritsukeLevel.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, txtPitch.Name, txtPitch.Text, iniPath);
        }

        /// <summary>
        /// iniデータをダイアログにセット
        /// </summary>
        public void GetIniData()
        {
            string iniPath = ClsIni.GetIniFilePath(IniPath);
            txtToritsukeLevel.Text = ClsIni.GetIniFile(sec, txtToritsukeLevel.Name, iniPath);
            txtPitch.Text = ClsIni.GetIniFile(sec, txtPitch.Name, iniPath);
        }
    }
}
