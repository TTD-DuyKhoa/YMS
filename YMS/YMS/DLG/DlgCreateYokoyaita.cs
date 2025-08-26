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
    public partial class DlgCreateYokoyaita : Form
    {
        #region 定数
        /// <summary>
        /// Iniファイルのパス
        /// </summary>
        const string IniPath = "ini\\DlgCreateYokoyaita.ini";

        /// <summary>
        /// Iniファイルのセクション名
        /// </summary>
        const string sec = "DlgCreateYokoyaita";
        #endregion

        #region メンバ変数
        /// <summary>
        /// 横矢板クラス
        /// </summary>
        public ClsYokoyaita m_ClsYokoyaita;
        #endregion

        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DlgCreateYokoyaita()
        {
            InitializeComponent();
            m_ClsYokoyaita = new ClsYokoyaita();
            InitControl();
            GetIniData();
        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DlgCreateYokoyaita(ClsYokoyaita clsYokoyaita)
        {
            InitializeComponent();

            m_ClsYokoyaita = clsYokoyaita;
            InitControl();
            SetControl();
        }
        #endregion

        #region イベント
        /// <summary>
        /// 種別コンボボックスの変更処理
        /// </summary>
        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //サイズコンボボックスの初期化
            initComboBoxSize();
        }

        /// <summary>
        /// OKボタン押下処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
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
        #endregion

        #region　メソッド
        /// <summary>
        /// コントロールの初期化
        /// </summary>
        private void InitControl()
        {
            initComboBox();
        }

        /// <summary>
        /// コンボボックス初期化
        /// </summary>
        public void initComboBox()
        {
            string bk = string.Empty;
            List<string> lstStr = new List<string>();

            //種別
            bk = cmbType.Text;
            cmbType.Items.Clear();
            lstStr = Master.ClsYokoyaitaCSV.GetTypeList();
            foreach (string str in lstStr)
            {
                cmbType.Items.Add(str);
            }
            if (lstStr.Contains(bk)) cmbType.Text = bk;
            else cmbType.SelectedIndex = 0;
        }

        /// <summary>
        /// コンボボックス初期化（サイズ）
        /// </summary>
        public void initComboBoxSize()
        {
            string bk = string.Empty;
            List<string> lstStr = new List<string>();

            //サイズ
            bk = cmbSize.Text;
            cmbSize.Items.Clear();
            lstStr = Master.ClsYokoyaitaCSV.GetSizeList(cmbType.Text);
            foreach (string str in lstStr)
            {
                cmbSize.Items.Add(str);
            }
            if (lstStr.Contains(bk)) cmbSize.Text = bk;
            else cmbSize.SelectedIndex = 0;
        }

        /// <summary>
        /// コントロールの値をメンバクラスに保持
        /// </summary>
        private void SetData()
        {
            ClsYokoyaita clsY = m_ClsYokoyaita;
            clsY.m_type = cmbType.Text;
            clsY.m_size = cmbSize.Text;
            clsY.m_putPosFlag = (rbnInside.Checked?0:1);
        }

        /// <summary>
        /// コントロールの値をメンバクラスに保持
        /// </summary>
        private void SetControl()
        {
            ClsYokoyaita clsY = m_ClsYokoyaita;
            cmbType.Text = clsY.m_type;
            cmbSize.Text = clsY.m_size;

            if (clsY.m_putPosFlag == 0)
            {
                rbnInside.Checked = true;
            }
            else
            {
                rbnOutSide.Checked = true;
            }
        }

        /// <summary>
        /// Iniの情報をコントロールにセット
        /// </summary>
        private void GetIniData()
        {
            try
            {
                string iniPath = ClsIni.GetIniFilePath(IniPath);
                cmbType.Text = ClsIni.GetIniFile(sec, cmbType.Name, iniPath);
                cmbSize.Text = ClsIni.GetIniFile(sec, cmbSize.Name, iniPath);
                rbnInside.Checked = RevitUtil.ClsCommonUtils.ChangeStrToBool(ClsIni.GetIniFile(sec, rbnInside.Name, iniPath));
                rbnOutSide.Checked = RevitUtil.ClsCommonUtils.ChangeStrToBool(ClsIni.GetIniFile(sec, rbnOutSide.Name, iniPath));
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

            ClsIni.WritePrivateProfileString(sec, cmbType.Name, cmbType.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, cmbSize.Name, cmbSize.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnInside.Name, rbnInside.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, rbnOutSide.Name, rbnOutSide.Checked.ToString(), iniPath);
        }
        #endregion

    }
}
