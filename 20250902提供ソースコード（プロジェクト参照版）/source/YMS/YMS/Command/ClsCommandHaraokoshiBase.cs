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
    class ClsCommandHaraokoshiBase
    {
        #region 定数
        /// <summary>
        /// Iniファイルのパス
        /// </summary>
        const string IniPath = "ini\\DlgCreateHaraokoshiBase.ini";

        /// <summary>
        /// Iniファイルのセクション名
        /// </summary>
        const string sec = "DlgCreateHaraokoshiBase";

        #endregion
        public static bool CommandHaraokoshiBase(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            DLG.DlgCreateHaraokoshiBase haraokoshiBase = new DLG.DlgCreateHaraokoshiBase(doc);
            DialogResult result = haraokoshiBase.ShowDialog();
            if (result != DialogResult.OK)
            {
                return false;
            }
            ClsHaraokoshiBase clsHaraBase = haraokoshiBase.m_ClsHaraBase;
            ////ワークセット
            //ClsWorkset clsWS = new ClsWorkset();
            //clsWS.SetWorkSetTest1(doc);

            try
            {
                for (; ; )
                {
                    ElementId id = null;
                    XYZ tmpStPoint, tmpEdPoint;
                    if (clsHaraBase.m_ShoriType == ClsHaraokoshiBase.ShoriType.BaseLineZureAri)
                    {
                        if (!ClsRevitUtil.PickObject(uidoc, "基準となるモデル線分", "モデル線分", ref id))
                        {
                            return false;
                        }
                        if (id == null)
                        {
                            MessageBox.Show("芯が選択されていません。");
                            return false;
                        }

                        //掘削側を指定
                        (tmpStPoint, tmpEdPoint) = ClsVoid.SelectVoidSide(uidoc, id);
                        if (tmpStPoint == null || tmpEdPoint == null) return false;

                        double syuzaiSize = Master.ClsYamadomeCsv.GetWidth(clsHaraBase.m_kouzaiSize);
                        if (clsHaraBase.m_yoko == ClsHaraokoshiBase.SideNum.Double)
                            syuzaiSize *= 2;
                        //腹起ベース作成
                        clsHaraBase.CreateHaraokoshiBase(doc, tmpStPoint, tmpEdPoint, syuzaiSize);

                    }
                    else if (clsHaraBase.m_ShoriType == ClsHaraokoshiBase.ShoriType.PtoPZureAri)
                    {
                        //2点間指定
                        //ClsCInput.Instance.Get2Point(uiapp, out tmpStPoint, out tmpEdPoint);
                        (tmpStPoint, tmpEdPoint) = ClsRevitUtil.GetSelect2Point(uidoc);
                        if (tmpStPoint == null || tmpEdPoint == null) return false;

                        using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                        {
                            t.Start();
                            ModelLine line = ClsYMSUtil.CreateKabeHojyoLine(doc, tmpStPoint, tmpEdPoint, 0, 0);
                            t.Commit();
                            id = line.Id;
                        }

                        //掘削側を指定
                        (tmpStPoint, tmpEdPoint) = ClsVoid.SelectVoidSide(uidoc, id);
                        if (tmpStPoint == null || tmpEdPoint == null) return false;

                        double syuzaiSize = Master.ClsYamadomeCsv.GetWidth(clsHaraBase.m_kouzaiSize);
                        if (clsHaraBase.m_yoko == ClsHaraokoshiBase.SideNum.Double)
                            syuzaiSize *= 2;

                        //腹起ベース作成
                        clsHaraBase.CreateHaraokoshiBase(doc, tmpStPoint, tmpEdPoint, syuzaiSize);

                        //補助線を削除
                        using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                        {
                            t.Start();
                            ClsRevitUtil.Delete(doc, id);
                            t.Commit();
                        }

                        //2点間ずれありの処理を記述
                        //MessageBox.Show("2点間ずれありの処理は現在実装中です。");
                        break;
                    }
                    else if (clsHaraBase.m_ShoriType == ClsHaraokoshiBase.ShoriType.PtoPZureNashi)
                    {
                        //2点間指定
                        //ClsCInput.Instance.Get2Point(uiapp, out tmpStPoint, out tmpEdPoint);
                        (tmpStPoint, tmpEdPoint) = ClsRevitUtil.GetSelect2Point(uidoc);
                        if (tmpStPoint == null || tmpEdPoint == null) return false;

                        using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                        {
                            t.Start();
                            ModelLine line = ClsYMSUtil.CreateKabeHojyoLine(doc, tmpStPoint, tmpEdPoint, 0, 0);
                            t.Commit();
                            id = line.Id;
                        }

                        //掘削側を指定
                        (tmpStPoint, tmpEdPoint) = ClsVoid.SelectVoidSide(uidoc, id);
                        if (tmpStPoint == null || tmpEdPoint == null) return false;

                        //腹起ベース作成
                        clsHaraBase.CreateHaraokoshiBase(doc, tmpStPoint, tmpEdPoint, 0);

                        //補助線を削除
                        using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                        {
                            t.Start();
                            ClsRevitUtil.Delete(doc, id);
                            t.Commit();
                        }

                        //2点間ずれなしの処理を記述
                        //MessageBox.Show("2点間ずれなしの処理は現在実装中です。");
                        break;
                    }
                    else
                    {
                        if (!ClsRevitUtil.PickObject(uidoc, "腹起ベースに置換するモデル線分", "モデル線分", ref id))
                        {
                            return false;
                        }
                        if (id == null)
                        {
                            MessageBox.Show("芯が選択されていません。");
                            return false;
                        }

                        //掘削側を指定
                        (tmpStPoint, tmpEdPoint) = ClsVoid.SelectVoidSide(uidoc, id);
                        if (tmpStPoint == null || tmpEdPoint == null) return false;

                        //腹起ベース作成
                        clsHaraBase.CreateHaraokoshiBase(doc, tmpStPoint, tmpEdPoint, 0);

                        //補助線を削除
                        using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                        {
                            t.Start();
                            ClsRevitUtil.Delete(doc, id);
                            t.Commit();
                        }

                        //置換の処理を記述
                        //MessageBox.Show("置換の処理は現在実装中です。");
                        //break;
                    }
                }
            }
            catch (Exception e)
            {

            }
            return true;
        }

        public static bool CommandChangeHaraokoshiBase(UIDocument uidoc, List<ElementId> idList = null)
        {
            Document doc = uidoc.Document;

            // 対象となるベースを複数選択
            if (idList == null)
            {
                if (!ClsHaraokoshiBase.PickBaseObjects(uidoc, ref idList))
                {
                    return false;
                }
            }

            double offset = 0.0;
            List<ClsHaraokoshiBase> clsHaraBaseList = new List<ClsHaraokoshiBase>();
            ClsHaraokoshiBase templateBase = new ClsHaraokoshiBase();
            for (int i = 0; i < idList.Count(); i++)
            {
                ElementId id = idList[i];

                if (i == 0)
                {
                    templateBase.SetParameter(doc, id);
                }
                else
                {
                    // 各項目を比較し、異なる場合は空白やNullに設定
                    FamilyInstance shinInstLevel = doc.GetElement(id) as FamilyInstance;
                    templateBase.m_dan = ClsRevitUtil.CompareValues(ClsRevitUtil.GetParameter(doc, id, "段"), templateBase.m_dan);
                    templateBase.m_yoko = ClsRevitUtil.CompareValues(ClsRevitUtil.GetParameter(doc, id, "横本数") == "シングル" ? ClsHaraokoshiBase.SideNum.Single : ClsHaraokoshiBase.SideNum.Double, templateBase.m_yoko);
                    templateBase.m_tate = ClsRevitUtil.CompareValues(ClsRevitUtil.GetParameter(doc, id, "縦本数") == "シングル" ? ClsHaraokoshiBase.VerticalNum.Single : ClsHaraokoshiBase.VerticalNum.Double, templateBase.m_tate);
                    templateBase.m_kouzaiType = ClsRevitUtil.CompareValues(ClsRevitUtil.GetParameter(doc, id, "鋼材タイプ"), templateBase.m_kouzaiType);
                    templateBase.m_kouzaiSize = ClsRevitUtil.CompareValues(ClsRevitUtil.GetParameter(doc, id, "鋼材サイズ"), templateBase.m_kouzaiSize);
                    if (templateBase.IsMega) templateBase.m_mega = ClsRevitUtil.CompareValues(ClsRevitUtil.GetParameterInteger(doc, id, "メガビーム本数"), templateBase.m_mega);
                    templateBase.m_offset = ClsRevitUtil.CompareValues(ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetParameterDouble(doc, id, "オフセット量")), templateBase.m_offset);
                    templateBase.m_tateGap = ClsRevitUtil.CompareValues(ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetParameterDouble(doc, id, "縦方向の隙間")), templateBase.m_tateGap);
                    templateBase.m_gapTyouseiType = ClsRevitUtil.CompareValues(ClsRevitUtil.GetParameter(doc, id, "隙間調整材鋼材ﾀｲﾌﾟ"), templateBase.m_gapTyouseiType);
                    templateBase.m_gapTyousei = ClsRevitUtil.CompareValues(ClsRevitUtil.GetParameter(doc, id, "隙間調整材"), templateBase.m_gapTyousei);
                    templateBase.m_gapTyouseiLenght = ClsRevitUtil.CompareValues(ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetParameterDouble(doc, id, "隙間調整材長さ")), templateBase.m_gapTyouseiLenght);
                }
            }

           // offset = templateBase.m_offset;

            DLG.DlgCreateHaraokoshiBase haraokoshiBase = new DLG.DlgCreateHaraokoshiBase(doc, templateBase);
            DialogResult result = haraokoshiBase.ShowDialog();
            if (result != DialogResult.OK)
            {
                return false;
            }

            foreach (var id in idList)
            {
                ClsHaraokoshiBase clsHaraBase = haraokoshiBase.m_ClsHaraBase;

                FamilyInstance shinInstLevel = doc.GetElement(id) as FamilyInstance;
                ElementId levelID = ClsRevitUtil.GetLevelID(doc, clsHaraBase.m_level);
                if (levelID != shinInstLevel.Host.Id)
                {
                    LocationCurve lCurve = shinInstLevel.Location as LocationCurve;
                    if (lCurve == null)
                    {
                        continue;
                    }
                    XYZ tmpStPoint = lCurve.Curve.GetEndPoint(0);
                    XYZ tmpEdPoint = lCurve.Curve.GetEndPoint(1);
                    //clsHaraBase.ChangeInterSectionHaraokoshiBase(doc, id);
                    //元のベースを削除
                    using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                    {
                        t.Start();
                        ClsRevitUtil.Delete(doc, id);
                        t.Commit();
                    }

                    //ベース作成
                    clsHaraBase.CreateHaraokoshiBase(doc, tmpStPoint, tmpEdPoint, 0);
                }
                else
                {
                    clsHaraBase.m_offset -= offset;
                    clsHaraBase.ChangeHaraokoshiBase(doc, id);
                    clsHaraBase.ChangeInterSectionHaraokoshiBase(doc, id);
                }
            }

            return true;
        }

    }
}
