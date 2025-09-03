using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMS
{
    class MonitorFairCommandEnabler : IExternalCommandAvailability
    {
		#region Methods (SC)

		public bool IsCommandAvailable(UIApplication uiApp, CategorySet catSet)//Element select 時走った
		{
			try
			{
				var uidoc = uiApp.ActiveUIDocument;
				// If no Document...
				if (uidoc == null)
				{
					// Assert Button Is Disabled
					return false;
				}

				var doc = uidoc.Document;

				// Raise the SelectionChangedEvent
				List<ElementId> selectElementIds = App.uiapp.ActiveUIDocument.Selection.GetElementIds().OrderBy(elementId => elementId.IntegerValue).ToList();

                //Messaging.DebugMessage(true, elementIds, "Fair59 - Availability Class Name Workaround", true);
                //MakeRequest(RequestId.UpdateSelectCASE);
                DLG.DlgCASETest m_FormCASE = Application.thisApp.GetForm_dlgCASETest();

                if (m_FormCASE != null && m_FormCASE.Visible)
                {
                    List<string> allCASEList = new List<string>();

                    for (int i = 0; i < selectElementIds.Count; i++)
                    {
                        allCASEList.Add(RequestHandler.GetParameter(doc, selectElementIds[i], "CASE文字"));
                    }
                    m_FormCASE.SetDataGridViewLo(allCASEList);
                }

                //ベースリスト選択
                DLG.DlgBaseList dlgBaseList = Application.thisApp.GetForm_dlgBaseList();
				if (dlgBaseList != null && dlgBaseList.Visible)
				{
					foreach (ElementId id in selectElementIds)
					{
						dlgBaseList.SelectBaseRow(doc, id);
					}
				}
			}
			catch
            {
				return false;
            }
			// Assert Button Is Disabled
			return false;
		}
		
		#endregion
	}
}
