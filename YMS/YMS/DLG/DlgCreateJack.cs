using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
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
    public partial class DlgCreateJack : System.Windows.Forms.Form
    {
        #region 定数

        /// <summary>
        /// Iniファイルのパス
        /// </summary>
        const string IniPath = "ini\\DlgCreateJack.ini";

        /// <summary>
        /// Iniファイルのセクション名
        /// </summary>
        const string sec = "DlgCreateJack";

        List<string> BtypeList = new List<string> { "九州", "大阪", "四国", "東京", "名古屋", "北海道" };
        List<string> CtypeList = new List<string> { "九州", "大阪", "四国", "東京", "名古屋", "北海道", "仙台" };
        List<string> DtypeList = new List<string> { "九州", "大分" };//, "大阪", "四国", "東京", "名古屋", "北海道"他と違うため対応を考える

        #endregion

        #region メンバー変数

        public Document m_doc;

        public ClsJack m_ClsJack { get; set; }

        #endregion

        public DlgCreateJack(Document doc, string syuzai, string jack)
        {
            InitializeComponent();
            m_doc = doc;
            m_ClsJack = new ClsJack();
            m_ClsJack.m_SyuzaiSize = syuzai;
            m_ClsJack.m_JackType = jack;
            InitControl();
            GetIniData();
        }

        private void InitControl()
        {
            // コンボボックスに選択肢を追加
            cmbJackType.Items.AddRange(Master.ClsJackCsv.GetTypeList().ToArray());

            if (cmbJackType.Items.Contains(m_ClsJack.m_JackType))
            {
                cmbJackType.SelectedItem = m_ClsJack.m_JackType;
            }
            else
            {
                cmbJackType.SelectedIndex = 0;
            }

            return;
        }

        private void InitializeUseOffset()
        {
            if (chkUseOffset.Checked)
            {
                groupBox1.Enabled = true;
            }
            else
            {
                groupBox1.Enabled = false;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!bCheckNecessity())
            {
                return;
            }

            SetIniData();
            SetData();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// OKボタン押下時の入力値チェック
        /// </summary>
        /// <returns></returns>
        private bool bCheckNecessity()
        {
            // ジャッキタイプのチェック
            if (string.IsNullOrWhiteSpace(cmbJackType.Text))
            {
                MessageBox.Show("ジャッキタイプを選択してください", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (chkUseOffset.Checked)
            {
                // 離れ量の入力値チェック
                string input = txtOffset.Text;
                if (!bIsRealNumber(input))
                {
                    MessageBox.Show("離れ量は実数値入力です", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                double hanareryo = double.Parse(input);
                if (hanareryo < 0.0)
                {
                    MessageBox.Show("離れ量は0.0以上です", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            return true;
        }

        private bool bIsRealNumber(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            if (double.TryParse(input, out double result))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 半角数字以外の入力をさせない
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtOffset_KeyPress(object sender, KeyPressEventArgs e)
        {
            //バックスペースが押された時は有効（Deleteキーも有効）
            if (e.KeyChar == '\b')
            {
                return;
            }

            //数値0～9以外が押された時はイベントをキャンセルする
            if ((e.KeyChar < '0' || '9' < e.KeyChar))
            {
                e.Handled = true;
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

                // ジャッキの種類
                this.cmbJackType.SelectedItem = ClsIni.GetIniFile(sec, cmbJackType.Name, iniPath);
                // 離れ量の指定有無
                this.chkUseOffset.Checked = bool.Parse(ClsIni.GetIniFile(sec, chkUseOffset.Name, iniPath));
                // 離れ量の値
                this.txtOffset.Text = ClsIni.GetIniFile(sec, txtOffset.Name, iniPath);
                // ジャッキカバーの有無
                this.chkUseJackCover.Checked = bool.Parse(ClsIni.GetIniFile(sec, chkUseJackCover.Name, iniPath));
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

            // ジャッキの種類
            ClsIni.WritePrivateProfileString(sec, cmbJackType.Name, cmbJackType.Text, iniPath);
            // 離れ量の指定有無
            ClsIni.WritePrivateProfileString(sec, chkUseOffset.Name, chkUseOffset.Checked.ToString(), iniPath);
            // 離れ量の値
            ClsIni.WritePrivateProfileString(sec, txtOffset.Name, txtOffset.Text, iniPath);
            // ジャッキカバーの有無
            ClsIni.WritePrivateProfileString(sec, chkUseJackCover.Name, chkUseJackCover.Checked.ToString(), iniPath);
        }

        private void SetData()
        {
            ClsJack clsJack = m_ClsJack;
            clsJack.m_JackTensyo = cmbTensyo.Text;
            clsJack.m_JackType = cmbJackType.Text;
            clsJack.m_UseOffset = chkUseOffset.Checked;
            if (chkUseOffset.Checked)
            {
                clsJack.m_Offset = double.Parse(txtOffset.Text);
            }
            clsJack.m_UseJackCover = chkUseJackCover.Checked;
        }

        private void cmbJackType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string strJackSize = Master.ClsJackCsv.GetJackSize(cmbJackType.Text, m_ClsJack.m_SyuzaiSize);
            string strTypeNameType = Master.ClsJackCsv.GetTypeNameType(strJackSize);//Atypeなど

            cmbTensyo.Items.Clear();
            if (strTypeNameType == "Btype")
            {
                cmbTensyo.Enabled = true;
                cmbTensyo.Items.AddRange(BtypeList.ToArray());
                cmbTensyo.SelectedIndex = 0;
            }
            else if (strTypeNameType == "Ctype")
            {
                cmbTensyo.Enabled = true;
                cmbTensyo.Items.AddRange(CtypeList.ToArray());
                cmbTensyo.SelectedIndex = 0;
            }
            else if (strTypeNameType == "Dtype")
            {
                cmbTensyo.Enabled = true;
                cmbTensyo.Items.AddRange(DtypeList.ToArray());
                cmbTensyo.SelectedIndex = 0;
            }
            else
            {
                cmbTensyo.Enabled = false;
            }
        }

        private void chkUseOffset_CheckedChanged(object sender, EventArgs e)
        {
            InitializeUseOffset();
        }
    }
}
