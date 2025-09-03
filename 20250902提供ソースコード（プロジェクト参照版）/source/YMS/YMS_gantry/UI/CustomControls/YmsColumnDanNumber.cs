using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YMS_gantry.UI
{
    public class YmsColumnDanNumber : DataGridViewTextBoxColumn
    {
        public YmsColumnDanNumber()
        {
            ReadOnly = true;
            DefaultCellStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleRight
            };
            HeaderText = "段";
        }
    }
}
