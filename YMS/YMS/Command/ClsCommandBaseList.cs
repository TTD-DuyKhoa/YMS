using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YMS.DLG;
using YMS.Parts;

namespace YMS.Command
{
    public enum BaseListCommand : int
    {
        HighLight,
        CreateNumber,
        Change,
        Close
    }
    /// <summary>
    /// ベース一覧コマンドクラス
    /// </summary>
    class ClsCommandBaseList
    {
        /// <summary>
        /// ベース一覧コマンドクラス
        /// </summary>
        /// <param name="uiapp"></param>
        /// <returns></returns>
        public static bool CommandBaseList(UIApplication uiapp)
        {
            try
            {
                ClsBaseList.m_LastLocation = System.Drawing.Point.Empty;
                ClsBaseList.m_LastTabName = string.Empty;
                ClsBaseList.m_Close = false;

                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                Application.thisApp.ShowForm_dlgBaseList(uidoc);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public static bool CommandRequestBaseList(UIDocument uidoc)
        {
            DLG.DlgBaseList dlgBaseList = Application.thisApp.GetForm_dlgBaseList();
            if (dlgBaseList != null && dlgBaseList.Visible)
            {
                switch (dlgBaseList.m_Command)
                {
                    case BaseListCommand.HighLight:
                        {
                            ClsBaseList.HighLightBase(uidoc, dlgBaseList);
                            break;
                        }
                    case BaseListCommand.CreateNumber:
                        {
                            ClsBaseList.CreateNumber(uidoc, dlgBaseList);
                            break;
                        }
                    case BaseListCommand.Change:
                        {
                            ClsBaseList.ChangeBase(uidoc, dlgBaseList);
                            ClsBaseList.CloseDlg(uidoc, dlgBaseList);
                            break;
                        }
                    case BaseListCommand.Close:
                        {
                            ClsBaseList.CloseDlg(uidoc, dlgBaseList);
                            ClsBaseList.m_Close = true;
                            ClsBaseList.m_LastLocation = System.Drawing.Point.Empty;
                            break;
                        }
                }
            }
            if (!ClsBaseList.m_Close)
            {
                //閉じる以外でダイアログが閉じたときには再表示
                Application.thisApp.ShowForm_dlgBaseList(uidoc);
            }
            return true;
        }
    }
}
