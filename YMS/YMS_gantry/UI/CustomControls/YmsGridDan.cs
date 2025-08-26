using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YMS_gantry.UI.FrnCreateSlopeControls;

namespace YMS_gantry.UI
{
    public partial class YmsGridDan : YMSDataGridViewSuper
    {
        public YmsGridDan()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }
    }
}
