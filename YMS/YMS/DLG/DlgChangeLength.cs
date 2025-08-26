using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RevitUtil;

namespace YMS.DLG
{

    public partial class DlgChangeLength : Form
    {
        public enum EMode
        {
            Edit = 0,
            Partition,
            Combine
        }

        #region メンバー
        private int m_selectCount;
        private double m_length; //mm
        private double m_div1;
        private double m_div2;
        private EMode m_mode;
        #endregion

        public DlgChangeLength(int selectCount,double length)
        {
            InitializeComponent();
            m_selectCount = selectCount;
            m_length = length;
        }

        private void DlgChangeLength_Load(object sender, EventArgs e)
        {
            double lengthM = ClsGeo.mm2m(m_length);
            txtLength.Text = lengthM.ToString("F1");

            if (m_selectCount < 1)
            {
                //不正値　
            }
            else if (m_selectCount == 1)
            {
                rdoEdit.Enabled = true;
                rdoPartition.Enabled = true;
                rdoCombine.Enabled = false;
                rdoEdit.Checked = true;
                numNewLength.Enabled = true;
                numDiv1NewLength.Enabled = false;
                numDiv2NewLength.Enabled = false;
            }
            else if (m_selectCount > 1)
            {
                rdoEdit.Enabled = false;
                rdoPartition.Enabled = false;
                rdoCombine.Enabled = true;
                rdoCombine.Checked = true;
                numNewLength.Enabled = false;
                numDiv1NewLength.Enabled = false;
                numDiv2NewLength.Enabled = false;
            }

            //長さ1、長さ2に値を設定する
            numDiv1NewLength.Value = (decimal)lengthM;
            numDiv2NewLength.Value = 0;

            //長さ1、長さ2の最大値・最小値を設定する
            numDiv1NewLength.Maximum = (decimal)lengthM;
            numDiv1NewLength.Minimum = 0;
            numDiv2NewLength.Maximum = (decimal)lengthM;
            numDiv2NewLength.Minimum = 0;
        }

        private void rdoEdit_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoEdit.Checked)
            {
                numNewLength.Enabled = true;
                numDiv1NewLength.Enabled = false;
                numDiv2NewLength.Enabled = false;
            }
            else
            {
                numNewLength.Enabled = false;
                numDiv1NewLength.Enabled = true;
                numDiv2NewLength.Enabled = true;
                double d = ClsGeo.mm2m(m_length);
                //decimal dc = System.Convert.ToDecimal(d);
                //numDiv1NewLength.Value = System.Convert.ToDecimal(d);
            }
        }

        private void rdoCombine_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoCombine.Checked)
            {
                numNewLength.Enabled = false;
                numDiv1NewLength.Enabled = false;
                numDiv2NewLength.Enabled = false;
                
            }
            else
            {
                //numNewLength.Enabled = true;
                //numDiv1NewLength.Enabled = false;
                //numDiv2NewLength.Enabled = false;
            }
        }

        public double GetLength()
        {
            return m_length;
        }

        public double GetDiv1Length()
        {
            return m_div1;
        }

        public double GetDiv2Length()
        {
            return m_div2;
        }

        public EMode GetMode()
        {
            return m_mode;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (rdoPartition.Checked)
            {
                if ((double)numDiv1NewLength.Value < 0.5)
                {
                    MessageBox.Show("分割値が不正です");
                    return;
                }

                if ((double)numDiv2NewLength.Value < 0.5)
                {
                    MessageBox.Show("分割値が不正です");
                    return;
                }

                m_div1 = ((double)numDiv1NewLength.Value);
                m_div2 = ((double)numDiv2NewLength.Value);
                m_mode = EMode.Partition;
            }

            if (rdoEdit.Checked)
            {
                m_length = ClsGeo.m2mm((double)numNewLength.Value);
                m_mode = EMode.Edit;
            }

            if (rdoCombine.Checked)
            {
                m_mode = EMode.Combine;
            }
            

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void numDiv1NewLength_ValueChanged(object sender, EventArgs e)
        {
            double lengthM = ClsGeo.mm2m(m_length);
            decimal diffValue = (decimal)lengthM - numDiv1NewLength.Value;
            numDiv2NewLength.Value = diffValue;
        }

        private void numDiv2NewLength_ValueChanged(object sender, EventArgs e)
        {
            double lengthM = ClsGeo.mm2m(m_length);
            decimal diffValue = (decimal)lengthM - numDiv2NewLength.Value;
            numDiv1NewLength.Value = diffValue;
        }

        //double div2 = m_length - ((double)numDiv1NewLength.Value);
        //double div2mm = ClsGeo.mm2m(div2);
        //    if (div2 % 0.5 == 0 && div2 > 0)
        //    {
        //        numDiv2NewLength.Value = System.Convert.ToDecimal(div2);
        //    }

    }
}
