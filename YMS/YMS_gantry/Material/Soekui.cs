using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YMS_gantry.UI;
using static YMS_gantry.DefineUtil;

namespace YMS_gantry.Material
{
    [MaterialCategory("添え杭")]
    public sealed class Soekui : MaterialSuper
    {
        public static new string Name = "Soekui";

        public Soekui() : base()
        {

        }

        public Soekui(ElementId _id, string koudaname, string size, string material) : base(_id, koudaname, material, size)
        {

        }
    }
}
