﻿using Autodesk.Revit.DB.Events;
using Autodesk.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YMS.UI;

namespace YMS
{
    class ApplicationEvents
    {
		public static void FairApplicationInitialized(object sender, ApplicationInitializedEventArgs e)
		{
            RibbonTab fairTab = null;
            RibbonPanel fairPanel = null;
            RibbonItem fairButton = null;

            foreach (var tab in ComponentManager.Ribbon.Tabs)
            {
                if (tab.Id == Ribbon.RibbonTabName)
                {
                    fairTab = tab;

                    //Messaging.DebugMessage($"Found Tab: {fairTab}");

                    foreach (var panel in tab.Panels)
                    {
                        if (panel.Source.Title == "FairPanel")
                        {
                            fairPanel = panel;

                            //Messaging.DebugMessage($"Found Panel: {fairPanel}");

                            foreach (var item in panel.Source.Items)
                            {
                                if (item.Id == "CustomCtrl_%CustomCtrl_%Selection Monitor%FairPanel%FairButtonItem")
                                {
                                    fairButton = item;

                                    //Messaging.DebugMessage($"Found Button: {fairButton}");

                                    break;
                                }
                            }
                        }
                    }

                    break;
                }
            }

            if (fairPanel != null && fairButton != null)
            {
                var position = Utilities.GetPositionBeforeButton("ID_REVIT_FILE_PRINT");

                Utilities.PlaceButtonOnQuickAccess(position, fairButton);

                Utilities.RemovePanelFromTab(fairTab, fairPanel);

                Utilities.RemoveTabFromRibbon(fairTab);
            }
        }
	}
}
