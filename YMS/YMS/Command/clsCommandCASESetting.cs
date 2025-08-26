using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using YMS.DLG;
using YMS.Parts;
using static YMS.ClsCASE;
using static YMS.DLG.DlgWaritsuke;

namespace YMS.Command
{
    /// <summary>
    /// CASE設定コマンドクラス
    /// </summary>
    class clsCommandCASESetting
    {
        public static void CommandCASESetting(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            //ドキュメントを取得
            Document doc = uidoc.Document;

            DlgCaseSelect selDlg = new DlgCaseSelect();
            if (selDlg.ShowDialog() == DialogResult.No)
            {
                //割当モード
                //情報変更モード
                ClsCASE cCase = new ClsCASE(ClsCASE.enKabeshu.all);
                DlgCASE dlg = new DlgCASE(DlgCASE.enMode.wariateMode);
                dlg.m_koyaitaDataList = cCase.GetKoyaitaCaseList(doc);
                dlg.m_oyaguiDataList = cCase.GetOyaguiCaseList(doc);
                dlg.m_smwDataList = cCase.GetSMWCaseList(doc);
                dlg.m_kabeDataList = cCase.GetRenzokukabeCaseList(doc);
            }
            else
            {
                //情報変更モード
                ClsCASE cCase = new ClsCASE(ClsCASE.enKabeshu.all);
                DlgCASE dlg = new DlgCASE(DlgCASE.enMode.editMode);
                dlg.m_koyaitaDataList = cCase.GetKoyaitaCaseList(doc);
                dlg.m_oyaguiDataList = cCase.GetOyaguiCaseList(doc);
                dlg.m_smwDataList = cCase.GetSMWCaseList(doc);
                dlg.m_kabeDataList = cCase.GetRenzokukabeCaseList(doc);

                if (dlg.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                List<ClsCASE.stCASEKoyaita> koyaitaCaseData = dlg.m_koyaitaDataList;
                List<ClsCASE.stCASEOyagui> oyaguiCaseData = dlg.m_oyaguiDataList;
                List<ClsCASE.stCASESMW> smwCaseData = dlg.m_smwDataList;
                List<ClsCASE.stCASERenzokuKabe> kabeCaseData = dlg.m_kabeDataList;

                List<ElementId> allKouyaitaList = ClsKouyaita.GetAllKouyaitaList(doc);
                List<ElementId> allOyakuiList = ClsOyakui.GetAllOyakuiList(doc);
                List<ElementId> allSMWList = ClsSMW.GetAllSMWList(doc);
                List<ElementId> allRenzokuKabeList = ClsRenzokukabe.GetAllRenzokuKabeList(doc);

                ClsCASE.stCASEKoyaita.CreateCASE(doc, koyaitaCaseData, allKouyaitaList);
                ClsCASE.stCASEOyagui.CreateCASE(doc, oyaguiCaseData, allOyakuiList);
                ClsCASE.stCASESMW.CreateCASE(doc, smwCaseData, allSMWList);
                ClsCASE.stCASERenzokuKabe.CreateCASE(doc, kabeCaseData, allRenzokuKabeList);
            }

            return;
        }

        public static void CommandMoodLessCASESetting(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application.thisApp.ShowForm_dlgCASE(uidoc);
            return;
        }

        public static bool CommandRequestCASE(UIDocument uidoc)
        {
            DlgCASE dlg = Application.thisApp.GetForm_dlgCASE();
            if (dlg != null && dlg.Visible)
            {
                switch (dlg.m_Command)
                {
                    case CASECommand.HighLight:
                        {
                            ClsCASE.HighLight(uidoc, dlg);
                            break;
                        }
                    case CASECommand.Change:
                        {
                            ClsCASE.ChangeCASE(uidoc, dlg);
                            ClsCASE.CloseDlg(uidoc, dlg);
                            break;
                        }
                    case CASECommand.Wariate:
                        {
                            ClsCASE.WariateCASE(uidoc, dlg);
                            ClsCASE.CloseDlg(uidoc, dlg);
                            break;
                        }
                    case CASECommand.Close:
                        {
                            ClsCASE.CloseDlg(uidoc, dlg);
                            break;
                        }
                }
            }

            return true;
        }
    }
}
