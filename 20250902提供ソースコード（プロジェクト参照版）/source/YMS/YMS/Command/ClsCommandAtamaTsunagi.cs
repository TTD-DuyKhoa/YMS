using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitUtil;
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
    internal class ClsCommandAtamaTsunagi
    {
        public static void CommandCreateAtamaTsunagiZai(UIDocument uidoc)
        {
            Document doc = uidoc.Document;

            YMS.DLG.DlgCreateAtamatsunagi dlgCreateAtamatsunagi = new DLG.DlgCreateAtamatsunagi(doc);
            DialogResult result = dlgCreateAtamatsunagi.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            ClsAtamaTsunagi clsAtamaTsunagi = dlgCreateAtamatsunagi.m_ClsAtamaTsunagi;

            if (clsAtamaTsunagi.m_KouzaiType == "主材")
            {
                ElementId oyakuiIdSt = null;
                if (!ClsAtamaTsunagi.PickObjectKabe(uidoc, ref oyakuiIdSt, "始点となる親杭or鋼矢板"))
                {
                    return;
                }

                ElementId oyakuiIdEd = null;
                if (!ClsAtamaTsunagi.PickObjectKabe(uidoc, ref oyakuiIdEd, "終点方向となる親杭or鋼矢板"))
                {
                    return;
                }

                clsAtamaTsunagi.CreateAtamaTsunagiZaiSyuzai(doc, oyakuiIdSt, oyakuiIdEd);

                return;
            }
            else
            {
                while (true)
                {
                    try
                    {
                        ElementId oyakuiIdSt = null;
                        if (!ClsAtamaTsunagi.PickObjectKabe(uidoc, ref oyakuiIdSt, "始点となる親杭or鋼矢板"))
                        {
                            return;
                        }

                        ElementId oyakuiIdEd = null;
                        if (!ClsAtamaTsunagi.PickObjectKabe(uidoc, ref oyakuiIdEd, "終点となる親杭or鋼矢板"))
                        {
                            return;
                        }

                        clsAtamaTsunagi.CreateAtamaTsunagiZai(doc, oyakuiIdSt, oyakuiIdEd);
                    }
                    catch
                    {
                        break;
                    }
                }

                return;
            }

        }

        public static void CommandCreateAtamaTsunagiHojoZai(UIDocument uidoc)
        {
            Document doc = uidoc.Document;

            ElementId atamatsunagiId = null;
            if (!ClsAtamaTsunagi.PickObject(uidoc, ref atamatsunagiId))
            {
                return;
            }

            ClsAtamaTsunagi clsAtamaTsunagi = new ClsAtamaTsunagi(doc, atamatsunagiId);

            DlgCreateAtamatsunagiHojo dlgCreateAtamatsunagiHojo = new DlgCreateAtamatsunagiHojo(clsAtamaTsunagi);
            DialogResult result = dlgCreateAtamatsunagiHojo.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            ElementId oyakuiId = null;
            if (!ClsAtamaTsunagi.PickObjectKabe(uidoc, ref oyakuiId, "始点となる親杭or鋼矢板"))
            {
                return;
            }

            clsAtamaTsunagi.CreateAtamaTsunagiHojoZai(doc, oyakuiId, atamatsunagiId, dlgCreateAtamatsunagiHojo.m_IsAoutoLayout, dlgCreateAtamatsunagiHojo.m_BracketInterval,dlgCreateAtamatsunagiHojo.m_IsSide);
        }

        public static void CommandDeleteAtamaTsunagi(UIDocument uidoc)
        {
            Document doc = uidoc.Document;

            List<ElementId> ids = new List<ElementId>();
            if (!ClsAtamaTsunagi.PickAtamatsunagiObjects(uidoc, ref ids))
            {
                return;
            }

            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                t.Start();
                ClsRevitUtil.Delete(doc, ids);
                t.Commit();
            }

        }

    }
}
