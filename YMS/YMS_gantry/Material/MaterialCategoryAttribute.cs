using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMS_gantry
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MaterialCategoryAttribute : NameAttribute
    {
        public MaterialCategoryAttribute(string name) : base(name) { }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MaterialPropertyAttribute : Attribute
    {
        public string Name { get; }
        public MaterialPropertyAttribute(string name)
        {
            Name = name;
        }
    }
}
