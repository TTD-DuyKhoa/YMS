using Autodesk.Windows ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;

namespace YMS.UI
{
  class Utilities
  {
    public static int GetPositionBeforeButton( string s )
    {
      var position = 0 ;

      foreach ( var item in ComponentManager.QuickAccessToolBar.Items ) {
        if ( string.IsNullOrWhiteSpace( item.Id ) ) {
          continue ;
        }

        position++ ;

        if ( item.Id == s ) {
          break ;
        }
      }

      return position ;
    }


    public static void PlaceButtonOnQuickAccess( int position, RibbonItem ribbonItem )
    {
      if ( position < ComponentManager.QuickAccessToolBar.Items.Count ) {
        ComponentManager.QuickAccessToolBar.InsertStandardItem( position, ribbonItem ) ;
      }
      else {
        ComponentManager.QuickAccessToolBar.AddStandardItem( ribbonItem ) ;
      }
    }


    public static void RemovePanelFromTab( RibbonTab ribbonTab, RibbonPanel ribbonPanel )
    {
      ribbonTab.Panels.Remove( ribbonPanel ) ;
    }


    public static void RemoveTabFromRibbon( RibbonTab ribbonTab )
    {
      if ( ribbonTab.Panels.Count != 0 ) {
        return ;
      }

      ribbonTab.IsVisible = false ;
    }
  }
}