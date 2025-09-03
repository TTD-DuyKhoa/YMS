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
using YMS.Parts;

namespace YMS.DLG
{
    public partial class DlgCreateStiffener : System.Windows.Forms.Form
    {
        #region 定数
        /// <summary>
        /// Iniファイルのパス
        /// </summary>
        const string IniPath = "ini\\DlgCreateStiffener.ini";

        /// <summary>
        /// Iniファイルのセクション名
        /// </summary>
        const string sec = "DlgCreateStiffener";
        #endregion
        public ClsStiffener m_ClsStiffener;

        public DlgCreateStiffener(Document doc, ClsStiffener clsStiffener)
        {
            InitializeComponent();
            m_ClsStiffener = clsStiffener;
            InitControl(doc);
        }
        public DlgCreateStiffener(Document doc)
        {
            InitializeComponent();
            m_ClsStiffener = new ClsStiffener();
            InitControl(doc);
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
        public void InitControl(Document doc)
        {
            cmbLevel.Items.AddRange(ClsYMSUtil.GetLevelNames(doc).ToArray());
        }
        public void SetData()
        {
            ClsStiffener csf = m_ClsStiffener;
            csf.m_Level = cmbLevel.Text;
            //////切梁//////
            if (rbnKiriTypeJack.Checked)
                csf.m_Kiribari_StiffenerType = rbnKiriTypeJack.Text;
            else
                csf.m_Kiribari_StiffenerType = rbnKiriTypePlate.Text;
            if (rbnKiriJackTypeSH.Checked)
                csf.m_Kiribari_StiffenerJackType = rbnKiriJackTypeSH.Text;
            else
                csf.m_Kiribari_StiffenerJackType = rbnKiriJackTypeDWJ.Text;
            if (rbnKiriPlateTypeDef.Checked)
                csf.m_Kiribari_StiffenerPlateType = rbnKiriPlateTypeDef.Text;
            else
                csf.m_Kiribari_StiffenerPlateType = rbnKiriPlateTypeAtsu.Text;
            csf.m_Kiribari_Count = ClsCommonUtils.ChangeStrToInt(txtKiriCount.Text);
            //////切梁//////
            //////火打ブロック//////
            if (rbnHiuchiTypeJack.Checked)
                csf.m_HiuchiBlock_StiffenerType = rbnHiuchiTypeJack.Text;
            else
                csf.m_HiuchiBlock_StiffenerType = rbnHiuchiTypePlate.Text;
            if (rbnHiuchiJackTypeSH.Checked)
                csf.m_HiuchiBlock_StiffenerJackType = rbnHiuchiJackTypeSH.Text;
            else
                csf.m_HiuchiBlock_StiffenerJackType = rbnHiuchiJackTypeDWJ.Text;
            if (rbnHiuchiPlateTypeDef.Checked)
                csf.m_HiuchiBlock_StiffenerPlateType = rbnHiuchiPlateTypeDef.Text;
            else
                csf.m_HiuchiBlock_StiffenerPlateType = rbnHiuchiPlateTypeAtsu.Text;
            csf.m_HiuchiBlock_Kiribari_Count = ClsCommonUtils.ChangeStrToInt(txtHiuchiKiriSideCount.Text);
            csf.m_HiuchiBlock_Hiuchi_Count = ClsCommonUtils.ChangeStrToInt(txtHiuchiHiuchiiSideCount.Text);
            //////火打ブロック//////
            //////切梁火打//////
            if (rbnKiriHiuchiTypeJack.Checked)
                csf.m_KiribariHiuchi_StiffenerType = rbnKiriHiuchiTypeJack.Text;
            else
                csf.m_KiribariHiuchi_StiffenerType = rbnKiriHiuchiTypePlate.Text;
            if (rbnKiriHiuchiJackTypeSH.Checked)
                csf.m_KiribariHiuchi_StiffenerJackType = rbnKiriHiuchiJackTypeSH.Text;
            else
                csf.m_KiribariHiuchi_StiffenerJackType = rbnKiriHiuchiJackTypeDWJ.Text;
            if (rbnKiriHiuchiPlateTypeDef.Checked)
                csf.m_KiribariHiuchi_StiffenerPlateType = rbnKiriHiuchiPlateTypeDef.Text;
            else
                csf.m_KiribariHiuchi_StiffenerPlateType = rbnKiriHiuchiPlateTypeAtsu.Text;
            csf.m_KiribariHiuchi_Count = ClsCommonUtils.ChangeStrToInt(txtKiriHiuchiCount.Text);
            //////切梁火打//////
            //////隅火打//////
            if (rbnSumiHiuchiTypeJack.Checked)
                csf.m_CornerHiuchi_StiffenerType = rbnSumiHiuchiTypeJack.Text;
            else
                csf.m_CornerHiuchi_StiffenerType = rbnSumiHiuchiTypePlate.Text;
            if (rbnSumiHiuchiJackTypeSH.Checked)
                csf.m_CornerHiuchi_StiffenerJackType = rbnSumiHiuchiJackTypeSH.Text;
            else
                csf.m_CornerHiuchi_StiffenerJackType = rbnSumiHiuchiPlateJackTypeDWJ.Text;
            if (rbnSumiHiuchiPlateTypeDef.Checked)
                csf.m_CornerHiuchi_StiffenerPlateType = rbnSumiHiuchiPlateTypeDef.Text;
            else
                csf.m_CornerHiuchi_StiffenerPlateType = rbnSumiHiuchiPlateTypeAtsu.Text;
            csf.m_CornerHiuchi_Count = ClsCommonUtils.ChangeStrToInt(txtCount.Text);
            //////隅火打//////

        }
        /// <summary>
        /// 入力チェック
        /// </summary>
        /// <returns></returns>
        public bool CheckInput()
        {
            if (!ClsCommonUtils.ChkContorolTextDouble(txtKiriCount, "切梁個数", bMinus:true, bComma: true, bMax: true, dMax:3))
                return false;
            if (!ClsCommonUtils.ChkContorolTextDouble(txtHiuchiKiriSideCount, "切梁側個数", bMinus: true, bComma: true, bMax: true, dMax: 3))
                return false;
            if (!ClsCommonUtils.ChkContorolTextDouble(txtHiuchiHiuchiiSideCount, "火打側個数", bMinus: true, bComma: true, bMax: true, dMax: 3))
                return false;
            if (!ClsCommonUtils.ChkContorolTextDouble(txtKiriHiuchiCount, "切梁火打個数", bMinus: true, bComma: true, bMax: true, dMax: 3))
                return false;
            if (!ClsCommonUtils.ChkContorolTextDouble(txtCount, "隅火打個数", bMinus: true, bComma: true, bMax: true, dMax: 3))
                return false;
            return true;
        }
        /// <summary>
        /// ダイアログデータをini二セット
        /// </summary>
        public void SetIniData()
        {
            string iniPath = ClsIni.GetIniFilePath(IniPath);

            ClsIni.WritePrivateProfileString(sec, cmbLevel.Name, cmbLevel.Text, iniPath);

            ClsIni.WritePrivateProfileString(sec, rbnKiriTypePlate.Name, rbnKiriTypePlate.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnKiriTypeJack.Name, rbnKiriTypeJack.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnKiriJackTypeSH.Name, rbnKiriJackTypeSH.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnKiriJackTypeDWJ.Name, rbnKiriJackTypeDWJ.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnKiriPlateTypeDef.Name, rbnKiriPlateTypeDef.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnKiriPlateTypeAtsu.Name, rbnKiriPlateTypeAtsu.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, txtKiriCount.Name, txtKiriCount.Text, iniPath);

            ClsIni.WritePrivateProfileString(sec, rbnHiuchiTypePlate.Name, rbnHiuchiTypePlate.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnHiuchiTypeJack.Name, rbnHiuchiTypeJack.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnHiuchiJackTypeSH.Name, rbnHiuchiJackTypeSH.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnHiuchiJackTypeDWJ.Name, rbnHiuchiJackTypeDWJ.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnHiuchiPlateTypeDef.Name, rbnHiuchiPlateTypeDef.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnHiuchiPlateTypeAtsu.Name, rbnHiuchiPlateTypeAtsu.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, txtHiuchiKiriSideCount.Name, txtHiuchiKiriSideCount.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, txtHiuchiHiuchiiSideCount.Name, txtHiuchiHiuchiiSideCount.Text, iniPath);

            ClsIni.WritePrivateProfileString(sec, rbnKiriHiuchiTypePlate.Name, rbnKiriHiuchiTypePlate.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnKiriHiuchiTypeJack.Name, rbnKiriHiuchiTypeJack.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnKiriHiuchiJackTypeSH.Name, rbnKiriHiuchiJackTypeSH.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnKiriHiuchiJackTypeDWJ.Name, rbnKiriHiuchiJackTypeDWJ.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnKiriHiuchiPlateTypeDef.Name, rbnKiriHiuchiPlateTypeDef.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnKiriHiuchiPlateTypeAtsu.Name, rbnKiriHiuchiPlateTypeAtsu.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, txtKiriHiuchiCount.Name, txtKiriHiuchiCount.Text, iniPath);

            ClsIni.WritePrivateProfileString(sec, rbnSumiHiuchiTypePlate.Name, rbnSumiHiuchiTypePlate.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnSumiHiuchiTypeJack.Name, rbnSumiHiuchiTypeJack.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnSumiHiuchiJackTypeSH.Name, rbnSumiHiuchiJackTypeSH.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnSumiHiuchiPlateJackTypeDWJ.Name, rbnSumiHiuchiPlateJackTypeDWJ.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnSumiHiuchiPlateTypeDef.Name, rbnSumiHiuchiPlateTypeDef.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnSumiHiuchiPlateTypeAtsu.Name, rbnSumiHiuchiPlateTypeAtsu.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, txtCount.Name, txtCount.Text, iniPath);
        }
        /// <summary>
        /// iniデータをダイアログにセット
        /// </summary>
        public void GetIniData()
        {
            string iniPath = ClsIni.GetIniFilePath(IniPath);
            cmbLevel.Text = ClsIni.GetIniFile(sec, cmbLevel.Name, iniPath);

            rbnKiriTypePlate.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnKiriTypePlate.Name, iniPath));
            rbnKiriTypeJack.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnKiriTypeJack.Name, iniPath));
            rbnKiriJackTypeSH.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnKiriJackTypeSH.Name, iniPath));
            rbnKiriJackTypeDWJ.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnKiriJackTypeDWJ.Name, iniPath));
            rbnKiriPlateTypeDef.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnKiriPlateTypeDef.Name, iniPath));
            rbnKiriPlateTypeAtsu.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnKiriPlateTypeAtsu.Name, iniPath));
            txtKiriCount.Text = ClsIni.GetIniFile(sec, txtKiriCount.Name, iniPath);

            rbnHiuchiTypePlate.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnHiuchiTypePlate.Name, iniPath));
            rbnHiuchiTypeJack.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnHiuchiTypeJack.Name, iniPath));
            rbnHiuchiJackTypeSH.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnHiuchiJackTypeSH.Name, iniPath));
            rbnHiuchiJackTypeDWJ.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnHiuchiJackTypeDWJ.Name, iniPath));
            rbnHiuchiPlateTypeDef.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnHiuchiPlateTypeDef.Name, iniPath));
            rbnHiuchiPlateTypeAtsu.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnHiuchiPlateTypeAtsu.Name, iniPath));
            txtHiuchiKiriSideCount.Text = ClsIni.GetIniFile(sec, txtHiuchiKiriSideCount.Name, iniPath);
            txtHiuchiHiuchiiSideCount.Text = ClsIni.GetIniFile(sec, txtHiuchiHiuchiiSideCount.Name, iniPath);

            rbnKiriHiuchiTypePlate.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnKiriHiuchiTypePlate.Name, iniPath));
            rbnKiriHiuchiTypeJack.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnKiriHiuchiTypeJack.Name, iniPath));
            rbnKiriHiuchiJackTypeSH.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnKiriHiuchiJackTypeSH.Name, iniPath));
            rbnKiriHiuchiJackTypeDWJ.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnKiriHiuchiJackTypeDWJ.Name, iniPath));
            rbnKiriHiuchiPlateTypeDef.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnKiriHiuchiPlateTypeDef.Name, iniPath));
            rbnKiriHiuchiPlateTypeAtsu.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnKiriHiuchiPlateTypeAtsu.Name, iniPath));
            txtKiriHiuchiCount.Text = ClsIni.GetIniFile(sec, txtKiriHiuchiCount.Name, iniPath);

            rbnSumiHiuchiTypePlate.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnSumiHiuchiTypePlate.Name, iniPath));
            rbnSumiHiuchiTypeJack.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnSumiHiuchiTypeJack.Name, iniPath));
            rbnSumiHiuchiJackTypeSH.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnSumiHiuchiJackTypeSH.Name, iniPath));
            rbnSumiHiuchiPlateJackTypeDWJ.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnSumiHiuchiPlateJackTypeDWJ.Name, iniPath));
            rbnSumiHiuchiPlateTypeDef.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnSumiHiuchiPlateTypeDef.Name, iniPath));
            rbnSumiHiuchiPlateTypeAtsu.Checked = bool.Parse(ClsIni.GetIniFile(sec, rbnSumiHiuchiPlateTypeAtsu.Name, iniPath));
            txtCount.Text = ClsIni.GetIniFile(sec, txtCount.Name, iniPath);
        }


    }
}
