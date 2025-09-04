using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Reflection ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Media.Imaging ;
using Autodesk.Revit.UI ;
using Autodesk.Windows ;

namespace YMS.UI
{
  class Ribbon
  {
    #region Constructors

    public Ribbon( string tabName, string panelName )
    {
      RibbonPanelName = panelName ;
      RibbonTabName = tabName ;
      Create() ;
    }

    #endregion

    #region Properties

    public static string RibbonPanelName { get ; private set ; }

    public static string RibbonTabName { get ; private set ; }

    private static string ImagePath
    {
      get { return "YMS.UI.Images." ; }
    }

    private static string Path
    {
      get { return Assembly.GetExecutingAssembly().Location ; }
    }

    #endregion

    #region Methods (SC)

    private static void Create()
    {
      // Tab
      try {
        App.uiContApp.CreateRibbonTab( RibbonTabName ) ;
        // Fair Selection Monitor
        var panelFairFiftyNine = App.uiContApp.CreateRibbonPanel( RibbonTabName, "FairPanel" ) ;
        panelFairFiftyNine.AddItem( FairFiftyNineCommand() ) ;
      }
      catch {
        //タブ名称が存在するとエラーが出ることがある
        //何故か存在のチェック方法が存在しない
      }
    }


    private static PushButtonData FairFiftyNineCommand()
    {
      var buttonName = "FairButtonItem" ;
      var fairButtonTitle = "Fair Method" ;

      return new PushButtonData( buttonName, fairButtonTitle, Path, "YMS.MonitorFairCommand" )
      {
        AvailabilityClassName = "YMS.MonitorFairCommandEnabler"
      } ;
    }

    #endregion
  }
}