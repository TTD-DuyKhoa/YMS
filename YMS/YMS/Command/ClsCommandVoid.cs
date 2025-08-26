using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YMS.Parts;

namespace YMS.Command
{
    class ClsCommandVoid
    {
        public static void CommandVoid(UIDocument uidoc)
        {
            //ドキュメントを取得
            Document doc = uidoc.Document;

            //作図に必要な作図箇所の座標を取得
            Selection selection = uidoc.Selection;
            try
            {
                List<string> kabeFilter = new List<string>();
                kabeFilter.Add("Soil");
                kabeFilter.Add(ClsRenzokukabe.RENZOKUKABE);

                //選択した部材ID
                List<ElementId> selId = new List<ElementId>();
                if (!ClsRevitUtil.PickObjectsPartListFilter(uidoc, "壁を選択してください", kabeFilter, ref selId))
                    return;
                if (selId.Count < 1)
                    return;
                List<List<ElementId>> voidIdList = new List<List<ElementId>>();
                List<ElementId> voidGroupList = new List<ElementId>();
                //部材は同一想定
                double dVoid = ClsVoid.GetVoidDep(doc, selId[0]);
                for(int i = 0; i < selId.Count; i++)
                {
                    if(dVoid == ClsVoid.GetVoidDep(doc, selId[i]))
                        voidGroupList.Add(selId[i]);
                    else
                    {
                        voidIdList.Add(voidGroupList.ToList());
                        dVoid = ClsVoid.GetVoidDep(doc, selId[i]);
                        voidGroupList.Clear();
                        voidGroupList.Add(selId[i]);
                    }
                    if(i == selId.Count - 1)
                    {
                        voidIdList.Add(voidGroupList.ToList());
                    }
                }


                //ボイドの深さ毎に補助線の選択、掘削を行う
                foreach (List<ElementId> voidId in voidIdList)
                {
                    dVoid = ClsVoid.GetVoidDep(doc, voidId[0]);
                    ElementId levelId = null;
                    int haba = 200;
                    FamilyInstance inst = doc.GetElement(voidId[0]) as FamilyInstance;
                    if (inst.Name.Contains(ClsRenzokukabe.RENZOKUKABE))
                    {
                        haba = (int)ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(inst.Symbol, "W"));
                        string levelName = ClsRevitUtil.GetParameter(doc, voidId[0], "作業面");
                        levelId = ClsRevitUtil.GetLevelID(doc, levelName);
                    }
                    else
                    {
                        haba = (int)ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(inst.Symbol, ClsGlobal.m_dia));
                        levelId = ClsRevitUtil.GetParameterElementId(doc, voidId[0], "集計レベル");
                    }
                    //線分選択
                    ISelectionFilter modelLineFilter = new FamilySelectionFilter("モデル線分");
                    Reference picLine = selection.PickObject(ObjectType.Element, modelLineFilter, "線分を選択してください");
                    ElementId selLine = picLine.ElementId;
                    Element line = null;
                    try
                    {
                        line = doc.GetElement(selLine);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    if (line == null)
                    {
                        return;
                    }


                    //ボイドの作図処理を実行
                    ClsVoid.DoVoid(doc, voidId, line, dVoid, haba / 2, levelId);
                }
            }
            catch (OperationCanceledException e)
            {
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return;
        }
    }
}
