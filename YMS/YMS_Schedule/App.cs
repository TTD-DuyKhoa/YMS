#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Events;
using System;
using System.Collections.Generic;

#endregion

namespace YMS_Schedule
{
    class App : IExternalApplication
    {
        public static UIControlledApplication uicapp { set; get; }

        public Result OnStartup(UIControlledApplication a)
        {
            uicapp = a;

            string iniPath = Env.iniPath();
            string tabName = MiscWin.GetIniValue(iniPath, Env.sectionRibbon, Env.keyTabName);
            string panelName = MiscWin.GetIniValue(iniPath, Env.sectionRibbonPanel, Env.keyPanelName);

            a.CreateRibbonTab(tabName);

            RibbonPanel panel1 = a.CreateRibbonPanel(tabName, panelName);
            panel1.Enabled = true;
            panel1.Visible = true;
            panel1.Name = panelName;
            panel1.Title = panelName;

            List<string> sections = MiscWin.GetSectionNames(iniPath);
            foreach (var s in sections)
            {
                if (s.ToLower().Contains(Env.sectionButton))
                {
                    string buttonName = MiscWin.GetIniValue(iniPath, s, Env.keyButtonName);
                    string imageS = MiscWin.GetIniValue(iniPath, s, Env.keyImageS);
                    string imageL = MiscWin.GetIniValue(iniPath, s, Env.keyImageL);
                    string description = MiscWin.GetIniValue(iniPath, s, Env.keyDescription);
                    string command = MiscWin.GetIniValue(iniPath, s, Env.keyCommand);
                    Env.ribbonButtonAdd(panel1, buttonName, command, description, imageL, imageS);
                }
            }

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
    }
}
