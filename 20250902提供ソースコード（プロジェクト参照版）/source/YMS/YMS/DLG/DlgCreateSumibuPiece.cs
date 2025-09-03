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
    public partial class DlgCreateSumibuPiece : Form
    {
        #region 定数
        /// <summary>
        /// Iniファイルのパス
        /// </summary>
        const string IniPath = "ini\\DlgCreateSumibuPiece.ini";

        /// <summary>
        /// Iniファイルのセクション名
        /// </summary>
        const string sec = "DlgCreateSumibuPiece";
        #endregion

        #region メンバー
        /// <summary>
        /// 切梁受けベースクラス
        /// </summary>
        public ClsSumibuPiece m_ClsSumibuPiece;
        #endregion

        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="sumibuPiece"></param>
        public DlgCreateSumibuPiece(ClsSumibuPiece sumibuPiece)
        {
            InitializeComponent();
            m_ClsSumibuPiece = sumibuPiece;

            initComboBox();
            SetControl();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="kiriHiuchiBase"></param>
        public DlgCreateSumibuPiece()
        {
            InitializeComponent();
            m_ClsSumibuPiece = new ClsSumibuPiece();
            initComboBox();
            GetIniData();
        }
        #endregion

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

        #region イベント
        /// <summary>
        /// コンボボックス変更処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbSumibuPeaceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            initComboBoxSteelSize();
        }

        /// <summary>
        /// チェックボックス変更処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkSumibuPeaceSize_CheckedChanged(object sender, EventArgs e)
        {
            cmbSumibuPeaceSize.Enabled = chkSumibuPeaceSize.Checked;
        }
        #endregion

        #region メソッド
        /// <summary>
        /// コンボボックス初期化
        /// </summary>
        public void initComboBox()
        {
            string bk = string.Empty;
            List<string> lstStr = new List<string>();

            //タイプ
            bk = cmbSumibuPeaceType.Text;
            cmbSumibuPeaceType.Items.Clear();
            lstStr = Master.ClsSumibuPieceCsv.GetTypeList();

            foreach (string str in lstStr)
            {
                cmbSumibuPeaceType.Items.Add(str);
            }
            if (lstStr.Contains(bk)) cmbSumibuPeaceType.Text = bk;
            else cmbSumibuPeaceType.SelectedIndex = 0;
        }

        /// <summary>
        /// コンボボックス初期化（サイズ）
        /// </summary>
        public void initComboBoxSteelSize()
        {
            string bk = string.Empty;
            List<string> lstStr = new List<string>();

            //サイズ
            bk = cmbSumibuPeaceSize.Text;
            cmbSumibuPeaceSize.Items.Clear();
            lstStr = Master.ClsSumibuPieceCsv.GetSizeList(cmbSumibuPeaceType.Text);

            foreach (string str in lstStr)
            {
                cmbSumibuPeaceSize.Items.Add(str);
            }
            if (lstStr.Contains(bk)) cmbSumibuPeaceSize.Text = bk;
            else cmbSumibuPeaceSize.SelectedIndex = 0;
        }
        #endregion


        /// <summary>
        /// コントロールの値をメンバクラスに保持
        /// </summary>
        private void SetData()
        {
            ClsSumibuPiece clsSP = m_ClsSumibuPiece;

            if (rbnCreateTypeAuto.Checked)
                clsSP.m_CreateType = ClsSumibuPiece.CreateType.Auto;
            else
                clsSP.m_CreateType = ClsSumibuPiece.CreateType.Manual;

            clsSP.m_Type = cmbSumibuPeaceType.Text;
            clsSP.m_Size = cmbSumibuPeaceSize.Text;
            clsSP.m_SizeSelectFlg = chkSumibuPeaceSize.Checked;
            clsSP.m_Tsumeryo = RevitUtil.ClsCommonUtils.ChangeStrToDbl(txtAidaTsumeRyo.Text);
        }

        /// <summary>
        /// コントロールの値をメンバクラスに保持
        /// </summary>
        private void SetControl()
        {
            ClsSumibuPiece clsSP = m_ClsSumibuPiece;

            if (clsSP.m_CreateType == ClsSumibuPiece.CreateType.Auto)
                rbnCreateTypeAuto.Checked = true;
            else
                rbnCreateTypeManual.Checked = true;

            cmbSumibuPeaceType.Text = clsSP.m_Type;
            cmbSumibuPeaceSize.Text = clsSP.m_Size;
            chkSumibuPeaceSize.Checked = clsSP.m_SizeSelectFlg;
            txtAidaTsumeRyo.Text = clsSP.m_Tsumeryo.ToString();
        }

        /// <summary>
        /// Iniの情報をコントロールにセット
        /// </summary>
        private void GetIniData()
        {
            try
            {
                string iniPath = ClsIni.GetIniFilePath(IniPath);
                rbnCreateTypeAuto.Checked = RevitUtil.ClsCommonUtils.ChangeStrToBool(ClsIni.GetIniFile(sec, rbnCreateTypeAuto.Name, iniPath));
                rbnCreateTypeManual.Checked = RevitUtil.ClsCommonUtils.ChangeStrToBool(ClsIni.GetIniFile(sec, rbnCreateTypeManual.Name, iniPath));
                cmbSumibuPeaceType.Text = ClsIni.GetIniFile(sec, cmbSumibuPeaceType.Name, iniPath);
                cmbSumibuPeaceSize.Text = ClsIni.GetIniFile(sec, cmbSumibuPeaceSize.Name, iniPath);
                chkSumibuPeaceSize.Checked = RevitUtil.ClsCommonUtils.ChangeStrToBool(ClsIni.GetIniFile(sec, chkSumibuPeaceSize.Name, iniPath));
                txtAidaTsumeRyo.Text = ClsIni.GetIniFile(sec, txtAidaTsumeRyo.Name, iniPath);
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

            ClsIni.WritePrivateProfileString(sec, rbnCreateTypeAuto.Name, rbnCreateTypeAuto.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnCreateTypeManual.Name, rbnCreateTypeManual.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, cmbSumibuPeaceType.Name, cmbSumibuPeaceType.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, cmbSumibuPeaceSize.Name, cmbSumibuPeaceSize.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, chkSumibuPeaceSize.Name, chkSumibuPeaceSize.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, txtAidaTsumeRyo.Name, txtAidaTsumeRyo.Text, iniPath);
        }

        /// <summary>
        /// 入力チェック
        /// </summary>
        /// <returns></returns>
        public bool CheckInput()
        {
            if (!ClsCommonUtils.ChkContorolTextDouble(txtAidaTsumeRyo, "間詰め量"))
                return false;

            return true;
        }

    }
}
