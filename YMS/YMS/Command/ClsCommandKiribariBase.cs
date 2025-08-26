using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
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
    class ClsCommandKiribariBase
    {
        public static bool CreateKiribariBase(UIDocument uidoc)
        {
            Document doc = uidoc.Document;
            List<ElementId> ids = new List<ElementId>();

            DLG.DlgCreateKiribariBase kiribari = new DLG.DlgCreateKiribariBase();
            DialogResult result = kiribari.ShowDialog();
            if (result != DialogResult.OK)
            {
                return false;
            }

            ClsKiribariBase clskiriBase = kiribari.m_ClsKiribariBase;

            ////ワークセット
            //ClsWorkset clsWS = new ClsWorkset();
            //clsWS.SetWorkSetTest1(doc);

            ElementId id = null;
            ModelLine line = null;
            if (clskiriBase.m_ShoriType == ClsKiribariBase.ShoriType.BaseLine)
            {
                //基準線
                if (!ClsHaraokoshiBase.PickBaseObject(uidoc, ref id))//ClsRevitUtil.PickObject(uidoc, "切梁ベースを配置する段の腹起ベース", "腹起ベース", ref id))
                {
                    return false;
                }

                if (!ClsRevitUtil.PickObjects(uidoc, "基準線となるモデル線分", "モデル線分", ref ids))
                {
                    return false;
                }

                if (ids.Count < 1)
                {
                    MessageBox.Show("芯が選択されていません。");
                    return false;
                }
            }
            else
            {
                //基準線
                if (!ClsHaraokoshiBase.PickBaseObject(uidoc, ref id))//ClsRevitUtil.PickObject(uidoc, "切梁ベースを配置する段の腹起ベース", "腹起ベース", ref id))
                {
                    return false;
                }
                (XYZ tmpStPoint, XYZ tmpEdPoint) = ClsRevitUtil.GetSelect2Point(uidoc);
                if (tmpStPoint == null || tmpEdPoint == null) return false;
                using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                {
                    t.Start();
                    line = ClsYMSUtil.CreateKabeHojyoLine(doc, tmpStPoint, tmpEdPoint, 0, 0);
                    t.Commit();
                }

                ids.Add(line.Id);

                //2点間の処理はここに記述
                //MessageBox.Show("2点間ずれありの処理は現在実装中です。");
            }

            //切梁ベース作成
            clskiriBase.CreateKiribariBase(doc, ids, id, ClsRevitUtil.GetParameter(doc, id, "段"));

            //補助線を削除
            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                t.Start();
                ClsRevitUtil.Delete(doc, ids);
                t.Commit();
            }

            return true;
        }

        public static bool CommandChangeKiribariBase(UIDocument uidoc)
        {
            Document doc = uidoc.Document;

            // 対象となるベースを複数選択
            List<ElementId> idList = null;
            if (!ClsKiribariBase.PickBaseObjects(uidoc, ref idList))
            {
                return false;
            }

            ClsKiribariBase templateBase = new ClsKiribariBase();
            for (int i = 0; i < idList.Count(); i++)
            {
                ElementId id = idList[i];

                if (i == 0)
                {
                    templateBase.SetParameter(doc, id);
                }
                else
                {
                    FamilyInstance shinInstLevel = doc.GetElement(id) as FamilyInstance;
                    ElementId levelID = shinInstLevel.Host.Id;
                    templateBase.m_kouzaiType = ClsRevitUtil.CompareValues(ClsRevitUtil.GetParameter(doc, id, "鋼材タイプ"), templateBase.m_kouzaiType);
                    templateBase.m_kouzaiSize = ClsRevitUtil.CompareValues(ClsRevitUtil.GetParameter(doc, id, "鋼材サイズ"), templateBase.m_kouzaiSize);
                    templateBase.m_tanbuStart = ClsRevitUtil.CompareValues(ClsRevitUtil.GetParameter(doc, id, "端部部品(始点側)"), templateBase.m_tanbuStart);
                    templateBase.m_tanbuEnd = ClsRevitUtil.CompareValues(ClsRevitUtil.GetParameter(doc, id, "端部部品(終点側)"), templateBase.m_tanbuEnd);
                    templateBase.m_jack1 = ClsRevitUtil.CompareValues(ClsRevitUtil.GetParameter(doc, id, "ジャッキタイプ(1)"), templateBase.m_jack1);
                    templateBase.m_jack2 = ClsRevitUtil.CompareValues(ClsRevitUtil.GetParameter(doc, id, "ジャッキタイプ(2)"), templateBase.m_jack2);
                }
            }

            DLG.DlgCreateKiribariBase kiribariBase = new DLG.DlgCreateKiribariBase(templateBase);
            DialogResult result = kiribariBase.ShowDialog();
            if (result != DialogResult.OK)
            {
                return false;
            }

            foreach (var id in idList)
            {
                ClsKiribariBase clsKiriBase = kiribariBase.m_ClsKiribariBase;
                
                // 始点と終点を取得
                FamilyInstance shinInstLevel = doc.GetElement(id) as FamilyInstance;
                ElementId levelID = shinInstLevel.Host.Id;
                string dan = ClsRevitUtil.GetParameter(doc, id, "段");
                string beforeSize = ClsRevitUtil.GetParameter(doc, id, "鋼材サイズ");
                //LocationCurve lCurve = shinInstLevel.Location as LocationCurve;
                //if (lCurve == null)
                //{
                //    return false;
                //}
                //XYZ tmpStPoint = lCurve.Curve.GetEndPoint(0);
                //XYZ tmpEdPoint = lCurve.Curve.GetEndPoint(1);

                ////元のベースを削除
                //using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                //{
                //    t.Start();
                //    ClsRevitUtil.Delete(doc, id);
                //    t.Commit();
                //}

                //切梁ベース変更
                clsKiriBase.ChangeKiribariBase(doc, id, dan, levelID);
                clsKiriBase.ChangeInterSectionKiribariBaseWithKiribariHiuchBase(doc, id, beforeSize);
            }

            return true;
        }
    }
}
