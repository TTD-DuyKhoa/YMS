using Autodesk.Revit.UI;
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
    public partial class DlgCreateSanjiku : Form
    {
        public static double m_constDist { get; set; }
        public static double m_distR { get; set; }
        public static double m_distL { get; set; }
        public static double m_sanjikuDist { get; set; }

        private RequestHandler m_Handler;
        private ExternalEvent m_ExEvent;
        public DlgCreateSanjiku(ExternalEvent exEvent, RequestHandler handler)
        {
            InitializeComponent();
            m_Handler = handler;
            m_ExEvent = exEvent;
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
                //this.buttonCancel.Enabled = true;
            }
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
        private void DlgCreateSanjiku_Load(object sender, EventArgs e)
        {
            SetShitenkanKyori(m_distR, m_distL);
            SetSanjikuDist(m_sanjikuDist);
        }
        private void DlgCreateSanjiku_FormClosed(object sender, FormClosedEventArgs e)
        {
            //初期化処理
            MakeRequest(RequestId.End);
            m_ExEvent.Dispose();
            m_ExEvent = null;
            m_Handler = null;
            ClsSanjikuPeace.Reset();
            
        }
        private void button1_Click(object sender, EventArgs e)
        {
            m_constDist = ClsCommonUtils.ChangeStrToDbl(txtHaraokoshiHanareRyo.Text) - m_sanjikuDist;

            MakeRequest(RequestId.SanjikuDist);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            m_constDist = 100.0;
            MakeRequest(RequestId.SanjikuDist);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            m_constDist = -100.0;
            MakeRequest(RequestId.SanjikuDist);
        }
        public void SetShitenkanKyori(double distR, double distL)
        {
            txtShitenkanKyoriRight.Text = distR.ToString();
            txtShitenkanKyoriLeft.Text = distL.ToString();
        }  
        public void SetSanjikuDist(double dist)
        {
            txtHaraokoshiHanareRyo.Text = dist.ToString();
        }
    }
}
