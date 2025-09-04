using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;

namespace YMS_gantry.Master
{
  [AttributeUsage( AttributeTargets.Method, AllowMultiple = false )]
  internal class YmsMasterLoadAttribute : Attribute
  {
  }
}