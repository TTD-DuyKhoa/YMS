using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMS_gantry
{
    [MaterialCategory("添接板")]
    public sealed class Tensetsuban : SlopeSupport
    {
        [MaterialProperty("H")]
        public int m_H { get; set; } = 0;
    }
}
