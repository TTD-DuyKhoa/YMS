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
    public partial class YmsComboInteger : ComboBox
    {
        private int? m_Number { get; set; } = null;

        public int? _Number
        {
            get
            {
                return m_Number;
            }
            set
            {
                if (m_Number == value)
                {
                    return;
                }
                if (true)
                {
                    Text = ConvertNumber(value);
                    m_Number = value;
                    return;
                }
            }
        }

        private string ConvertNumber(int? input)
        {
            if (input == null)
            {
                return "";
            }
            return input.Value.ToString();
        }

        public YmsComboInteger()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            var text = this.Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                m_Number = null;
                ForeColor = DefaultForeColor;
            }
            else
            {
                if (int.TryParse(text, out var textNum))
                {
                    ForeColor = DefaultForeColor;
                    m_Number = textNum;
                }
                else
                {
                    ForeColor = System.Drawing.Color.Red;
                }
            }
            base.OnTextChanged(e);
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            Text = ConvertNumber(m_Number);
        }
    }
}
