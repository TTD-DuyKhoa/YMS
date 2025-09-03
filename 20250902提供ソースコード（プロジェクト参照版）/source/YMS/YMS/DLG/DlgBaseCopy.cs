using Autodesk.Revit.DB;
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
    public partial class DlgBaseCopy : System.Windows.Forms.Form
    {
        /// <summary>
        /// ドキュメント
        /// </summary>
        private Document m_doc;

        /// <summary>
        /// コピー元レベル
        /// </summary>
        private string m_BaseLevel;

        /// <summary>
        /// 指定レベル一覧
        /// </summary>
        public List<KeyValuePair<string,ElementId>> ListLevel;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DlgBaseCopy(Document doc, string baseLebel)
        {
            InitializeComponent();
            m_doc = doc;
            m_BaseLevel = baseLebel;
        }

        /// <summary>
        /// ロード処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DlgWorkSet_Load(object sender, EventArgs e)
        {
            //データグリッドに初期値を設定
            SetDataGridView();
        }

        /// <summary>
        /// OKボタン押下処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            SetData();
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
        /// DGVのデータをメンバ変数にセットする
        /// </summary>
        public void SetDataGridView()
        {
            List<string> lsLevel = ClsYMSUtil.GetLevelNames(m_doc);
            foreach(string level in lsLevel)
            {


                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgvLevelList);
                if (level == m_BaseLevel)
                {
                    row.Cells[0].Value = true;
                    System.Drawing.Color c = System.Drawing.Color.FromArgb(200, 200, 200);
                    row.Cells[0].Style.BackColor = c;
                    row.Cells[1].Style.BackColor = c;
                    row.Cells[0].ReadOnly = true;
                }
                else
                {
                    row.Cells[0].Value = false;
                }
                row.Cells[1].Value = level;
                dgvLevelList.Rows.Add(row);
            }
        }

        /// <summary>
        /// DGVのデータをメンバ変数にセットする
        /// </summary>
        public void SetData()
        {
            List<KeyValuePair<string,ElementId>> lstData = new List<KeyValuePair<string, ElementId>>();
            foreach(DataGridViewRow row in dgvLevelList.Rows)
            {
                string str = Convert.ToString( row.Cells[1].Value);
                if((bool)row.Cells[0].Value == true && str != m_BaseLevel)
                {
                    ElementId levelId = RevitUtil.ClsRevitUtil.GetLevelID(m_doc, str);
                    lstData.Add(new KeyValuePair<string, ElementId>(str,levelId));
                }
            }
            ListLevel = lstData;
        }

        /// <summary>
        /// 切り取りにチェックが入っているか取得
        /// </summary>
        /// <returns></returns>
        public bool GetChechCut() { return chcCut.Checked; }
    }
}
