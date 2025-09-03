using Autodesk.Revit.UI;
using System;
using System.Windows.Forms;

namespace YMS.DLG
{
    public partial class DlgCreateVoidFamily : Form
    {
        private RequestHandler m_Handler;
        private ExternalEvent m_ExEvent;

        //private UIDocument uidocForm;
        public DlgCreateVoidFamily(ExternalEvent exEvent, RequestHandler handler)
        {
            InitializeComponent();
            m_Handler = handler;
            m_ExEvent = exEvent;
            //uidocForm = uidoc;
        }
        /// <summary>
        /// ダイアログの掘削の数値を取得
        /// </summary>
        /// <param name="num">掘削の番号</param>
        /// <returns></returns>
        public Double GetKussaku(int num)
        {
            switch(num)
            {
                case 1:
                    {
                        if (Double.TryParse(txtKussakuFukasa1.Text, out Double kussaku1))
                        {
                            return kussaku1;
                        }
                        return 1500.0;
                    }
                case 2:
                    {
                        if (Double.TryParse(txtKussakuFukasa2.Text, out Double kussaku2))
                        {
                            return kussaku2;
                        }
                        return 1500.0;
                    }
                default:
                    {
                        return 1500.0;
                    }
            }
        }
        private void buttonOK_Click(object sender, EventArgs e)
        {

            MessageBox.Show("Hello World");
            MakeRequest(RequestId.PutHari);
            //ダイアログの再表示

        }

        private void ModelessFormTest_Shown(object sender, EventArgs e)
        {
            //ClsTest.testCommand4(uidocForm);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        ///   WakeUp -> enable all controls
        /// </summary>
        /// 
        public void WakeUp()
        {
            EnableCommands(true);
        }
        /// <summary>
        ///   Control enabler / disabler 
        /// </summary>
        ///
        private void EnableCommands(bool status)
        {
            foreach (Control ctrl in this.Controls)
            {
                ctrl.Enabled = status;
            }
            if (!status)
            {
                this.btnEnd.Enabled = true;
            }
        }

        private void ModelessFormTest_FormClosed(object sender, FormClosedEventArgs e)
        {
            //初期化処理
            MakeRequest(RequestId.End);
            m_ExEvent.Dispose();
            m_ExEvent = null;
            m_Handler = null;
        }
        private void MakeRequest(RequestId request)
        {
            m_Handler.Request.Make(request);
            m_ExEvent.Raise();
            DozeOff();
        }
        /// <summary>
        ///   DozeOff -> disable all controls (but the Exit button)
        /// </summary>
        /// 
        private void DozeOff()
        {
            EnableCommands(false);
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            //壁掘削用ボイドのファミリ配置配置
            MakeRequest(RequestId.PutKussakuBoidKabe);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //ソイル掘削用ボイドのファミリ配置配置
            MakeRequest(RequestId.PutKussakuBoidSoil);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //基準線選択ソイル掘削用ボイドのファミリ配置
            MakeRequest(RequestId.PutKussakuBoidSoilLine);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //基準線選択壁掘削用ボイドのファミリ配置
            MakeRequest(RequestId.PutKussakuBoidKabeLine);
        }

    }
}
