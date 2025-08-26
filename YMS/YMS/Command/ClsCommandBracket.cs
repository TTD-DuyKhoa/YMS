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
using static YMS.Parts.ClsBracket;

namespace YMS.Command
{
    internal class ClsCommandBracket
    {
        public static void CommandCreateBracket(UIDocument uidoc)
        {
            Document doc = uidoc.Document;

            DlgCreateBracket dlgCreateBracket = new DlgCreateBracket(doc);
            DialogResult result = dlgCreateBracket.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            ClsBracket clsBracket = dlgCreateBracket.m_ClsBracket;

            while (true)
            {
                try
                {
                    List<string> typeNameKui = new List<string>();
                    ElementId kuiId = null;

                    switch (clsBracket.m_CreateType)
                    {
                        case ClsBracket.CreateType.Kiribari:
                            typeNameKui.Add("棚杭・中間杭");
                            typeNameKui.Add("兼用杭_棚杭、支持杭");
                            if (!ClsRevitUtil.PickObjectPartListFilter(uidoc, "対象の杭を選択してください", typeNameKui, ref kuiId))
                            {
                                return;
                            }
                            clsBracket.m_TargetKuiCreateType = ClsTanakui.CreateType.OnePoint;
                            clsBracket.CreateKiribariBracket(doc, kuiId);

                            break;
                        case ClsBracket.CreateType.kiribariOsae:
                            typeNameKui.Add("棚杭・中間杭");
                            typeNameKui.Add("兼用杭_棚杭、支持杭");
                            if (!ClsRevitUtil.PickObjectPartListFilter(uidoc, "対象の杭を選択してください", typeNameKui, ref kuiId))
                            {
                                return;
                            }
                            clsBracket.m_TargetKuiCreateType = ClsTanakui.CreateType.OnePoint;
                            clsBracket.CreateKiribariOsaeBracket(doc, kuiId);

                            break;
                        case ClsBracket.CreateType.Haraokoshi:
                        case ClsBracket.CreateType.HaraokoshiOsae:

                            string typeName = "";
                            if (clsBracket.m_CreateType == ClsBracket.CreateType.Haraokoshi)
                            {
                                typeName = "腹起BL";
                            }
                            else
                            {
                                typeName = "腹起押えBL";
                            }

                            if (clsBracket.m_CreateMode == CreateMode.Manual)
                            {
                                typeNameKui.Add("親杭");
                                typeNameKui.Add("鋼矢板");
                                typeNameKui.Add("連続杭");
                                typeNameKui.Add("SMW");

                                List<ElementId> oyakuiIds = new List<ElementId>();
                                if (!ClsRevitUtil.PickObjectPartListFilter(uidoc, "対象の杭を選択してください", typeNameKui, ref kuiId))
                                {
                                    return;
                                }
                                oyakuiIds.Add(kuiId);

                                // 図面上の腹起を取得
                                List<ElementId> haraokoshiIds = new List<ElementId>();
                                haraokoshiIds = ClsRevitUtil.GetSelectCreatedFamilyInstanceList(doc, "腹起", true);
                                if (haraokoshiIds.Count == 0)
                                {
                                    return;
                                }

                                clsBracket.CreateHaraokoshiBracket(doc, oyakuiIds, haraokoshiIds, typeName);
                            }
                            else if (clsBracket.m_CreateMode == CreateMode.Auto)
                            {
                                // 壁芯を選択
                                ElementId kabeshinId = null;
                                if (!ClsKabeShin.PickBaseObject(uidoc, ref kabeshinId))
                                {
                                    return;
                                }

                                // 壁芯から杭を取得
                                List<ElementId> oyakuiIds = ClsKabeShin.GetKabeIdList(doc, kabeshinId);

                                // 図面上の腹起を取得
                                List<ElementId> haraokoshiIds = new List<ElementId>();
                                haraokoshiIds = ClsRevitUtil.GetSelectCreatedFamilyInstanceList(doc, "腹起", true);
                                if (haraokoshiIds.Count == 0)
                                {
                                    return;
                                }
                                //foreach (var i in haraokoshiIds)
                                //{
                                //    FamilyInstance ins = doc.GetElement(i) as FamilyInstance;
                                //    string l = ins.Host.Name;
                                //}

                                clsBracket.CreateHaraokoshiBracket(doc, oyakuiIds, haraokoshiIds, typeName);
                            }
                            else if (clsBracket.m_CreateMode == CreateMode.Optional)
                            {
                                // 図面上の杭を取得
                                List<ElementId> oyakuiIds = new List<ElementId>();
                                oyakuiIds.AddRange(ClsOyakui.GetAllOyakuiList(doc));
                                oyakuiIds.AddRange(ClsKouyaita.GetAllKouyaitaList(doc));
                                oyakuiIds.AddRange(ClsRenzokukabe.GetAllRenzokuKabeList(doc));
                                oyakuiIds.AddRange(ClsSMW.GetAllSMWList(doc));

                                // 図面上の腹起を取得
                                List<ElementId> haraokoshiIds = new List<ElementId>();
                                if (!ClsRevitUtil.PickObjectsPartFilter(uidoc, "対象の腹起を選択してください", "腹起", ref haraokoshiIds))
                                {
                                    return;
                                }

                                clsBracket.CreateHaraokoshiBracket(doc, oyakuiIds, haraokoshiIds, typeName);
                            }

                            break;
                    }
                }
                catch (Exception e)
                {
                    string mes = e.Message;
                    break;
                }
            }

        }
    }
}
