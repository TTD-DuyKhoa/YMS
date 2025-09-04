using System ;
using Autodesk.Revit.DB.Events ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;

namespace YMS
{
  class EventFactory
  {
    public static void ShutDown()
    {
      App.uiContApp.ControlledApplication.ApplicationInitialized -= ApplicationEvents.FairApplicationInitialized ;
    }


    public static void StartUp()
    {
      //中に処理は入らない
      App.uiContApp.ControlledApplication.ApplicationInitialized +=
        new EventHandler<ApplicationInitializedEventArgs>( ApplicationEvents.FairApplicationInitialized ) ;
    }
  }
}