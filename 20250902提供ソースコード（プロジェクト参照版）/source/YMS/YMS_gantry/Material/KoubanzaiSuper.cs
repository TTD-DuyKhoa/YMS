using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMS_gantry.Material
{
    public abstract class KouhanzaiSuper : MaterialSuper
    {
        public static new string Name = "Kouhanzai";
        public KouhanzaiSuper():base()
        {

        }

        public KouhanzaiSuper(ElementId id, string koudaname,string size) : base(id,koudaname,size,"")
        {

        }

    }
}
