using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMS_gantry
{
    public abstract class Stiffener : SlopeSupport
    {
        [MaterialProperty("H")]
        public int m_H { get; set; } = 0;
    }
}
