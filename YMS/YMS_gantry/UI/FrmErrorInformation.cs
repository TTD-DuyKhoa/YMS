using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YMS_gantry.UI
{
    public partial class FrmErrorInformation : Form
    {
        private List<string> m_ErrorList { get; set; }
        public FrmErrorInformation(List<string>errorList,Form owner=null)
        {
            InitializeComponent();
            m_ErrorList = errorList;            
            this.ShowDialog(owner);
        }

        private void FrmErrorInformation_Load(object sender, EventArgs e)
        {
            LstErrorInfo.Items.Clear();
            //エラー情報を一覧で表示する
            foreach(string st in m_ErrorList)
            {
                LstErrorInfo.Items.Add($"・{st}");
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
