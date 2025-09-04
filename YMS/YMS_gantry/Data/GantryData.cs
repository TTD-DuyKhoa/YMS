using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using YMS_gantry.Data ;

namespace YMS_gantry
{
  /// <summary>
  /// YMS構台の部材データクラス
  /// </summary>
  [Serializable]
  public abstract class GantryData
  {
    public string SchemaName = "" ;
  }
}