using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMS.Parts
{
    public class ClsHaraokoshiBase
    {
        #region 定数
        public const string baseName = "腹起ベース";
        public const string GapTyousei = "腹起隙間調整材";
        public const string SukimaH = "H形鋼 広幅";
        public const string SukimaC = "チャンネル";

        /// <summary>
        /// 処理タイプ
        /// </summary>
        public enum ShoriType
        {
            BaseLineZureAri,
            PtoPZureAri,
            PtoPZureNashi,
            Replace
        }

        /// <summary>
        /// 段レベル
        /// </summary>
        public enum DanLevel
        {
            Upper,
            Just,
            Lower
        }

        /// <summary>
        /// 横本数
        /// </summary>
        public enum SideNum
        {
            Single,
            Double
        }

        /// <summary>
        /// 縦本数
        /// </summary>
        public enum VerticalNum
        {
            Single,
            Double
        }

        /// <summary>
        /// 勝ち負け
        /// </summary>
        public enum WinLose
        {
            Win,
            Lose
        }

        #endregion

        #region プロパティ
        /// <summary>
        /// 処理方法
        /// </summary>
        public ShoriType m_ShoriType { get; set; }
        /// <summary>
        /// 配置レベル
        /// </summary>
        public string m_level { get; set; }
        /// <summary>
        /// 段レベル
        /// </summary>
        public string m_dan { get; set; }
        /// <summary>
        /// 横本数
        /// </summary>
        public SideNum m_yoko { get; set; }
        /// <summary>
        /// 縦本数
        /// </summary>
        public VerticalNum m_tate { get; set; }
        /// <summary>
        /// 鋼材タイプ
        /// </summary>
        public string m_kouzaiType { get; set; }
        /// <summary>
        /// 鋼材サイズ
        /// </summary>
        public string m_kouzaiSize { get; set; }
        ///// <summary>
        ///// メガビーム
        ///// </summary>
        //public bool m_bMega { get; set; }
        /// <summary>
        /// メガビーム本数
        /// </summary>
        public int m_mega { get; set; }
        ///// <summary>
        ///// SMH
        ///// </summary>
        //public bool m_bSMH { get; set; }
        /// <summary>
        /// オフセット量
        /// </summary>
        public double m_offset { get; set; }
        /// <summary>
        /// 縦方向の隙間
        /// </summary>
        public double m_tateGap { get; set; }
        /// <summary>
        /// 隙間調整材鋼材ﾀｲﾌﾟ
        /// </summary>
        public string m_gapTyouseiType { get; set; }
        /// <summary>
        /// 隙間調整材
        /// </summary>
        public string m_gapTyousei { get; set; }
        /// <summary>
        /// 隙間調整材長さ
        /// </summary>
        public double m_gapTyouseiLenght { get; set; }
        /// <summary>
        /// 勝ち負け
        /// </summary>
        public WinLose m_katimake { get; set; }

        /// <summary>
        /// 編集用：フロア
        /// </summary>
        //public string m_Floor { get; set; }

        /// <summary>
        /// 編集用：エレメントID
        /// </summary>
        public ElementId m_ElementId { get; set; }

        /// <summary>
        /// 編集用：編集フラグ
        /// </summary>
        public bool m_FlgEdit { get; set; }

        public bool IsMega
        {
            get
            {
                return m_kouzaiType == Master.ClsYamadomeCsv.TypeMegaBeam;
            }
        }
        #endregion

        #region コンストラクタ
        public ClsHaraokoshiBase()
        {
            //初期化
            Init();
        }
        #endregion

        #region メソッド
        public void Init()
        {
            m_ShoriType = ShoriType.BaseLineZureAri;
            m_level = string.Empty;
            m_dan = string.Empty;
            m_yoko = SideNum.Single;
            m_tate = VerticalNum.Single;
            m_kouzaiType = string.Empty;
            m_kouzaiSize = string.Empty;
            //m_bMega = false;
            m_mega = 0;
            //m_bSMH = false;
            m_offset = 0.0;
            m_tateGap = 0.0;
            m_gapTyouseiType = string.Empty;
            m_gapTyousei = string.Empty;
            m_gapTyouseiLenght = 0.0;
            m_katimake = WinLose.Win;
            //m_Floor = string.Empty;
            m_ElementId = null;
            m_FlgEdit = false;
        }
        /// <summary>
        /// 選択したモデル線分に腹起ベースを作成する（掘削側指定）
        /// </summary>
        /// <param name="doc">ドキュメント</param>
        /// <param name="tmpStPoint">始点</param>
        /// <param name="tmpEdPoint">終点</param>
        /// <returns></returns>
        public bool CreateHaraokoshiBase(Document doc, XYZ tmpStPoint, XYZ tmpEdPoint, double shift)
        {
            string symbolFolpath = ClsZumenInfo.GetYMSFolder();
            string shinfamily = System.IO.Path.Combine(symbolFolpath, "ベース関係\\" + baseName + ".rfa");

            //シンボル読込
            if (!ClsRevitUtil.LoadFamilySymbolData(doc, shinfamily, baseName, out FamilySymbol sym))
            {
                return false;
            }

            XYZ Direction = Line.CreateBound(tmpStPoint, tmpEdPoint).Direction;//単位ベクトル取得

            ElementId levelID = ClsRevitUtil.GetLevelID(doc, m_level);

            //腹起ベースを内側にオフセット（鋼材サイズ+オフセット量）
            double dEx = ClsRevitUtil.CovertToAPI(shift + m_offset);//300はダイアログ鋼材サイズから取得する

            tmpStPoint = new XYZ(tmpStPoint.X - (dEx * Direction.Y),
                                 tmpStPoint.Y + (dEx * Direction.X),
                                 tmpStPoint.Z);
            tmpEdPoint = new XYZ(tmpEdPoint.X - (dEx * Direction.Y),
                                 tmpEdPoint.Y + (dEx * Direction.X),
                                 tmpEdPoint.Z);

            Curve cv = Line.CreateBound(tmpStPoint, tmpEdPoint);

            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                t.Start();
                FailureHandlingOptions failOpt = t.GetFailureHandlingOptions();
                failOpt.SetFailuresPreprocessor(new WarningSwallower());
                t.SetFailureHandlingOptions(failOpt);
                ElementId CreatedID = ClsRevitUtil.Create(doc, cv, levelID, sym);

                ClsRevitUtil.SetMojiParameter(doc, CreatedID, "段", m_dan);
                if (m_yoko == SideNum.Double)
                {
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "横本数", "ダブル");
                    ClsRevitUtil.ChangeLineColor(doc, CreatedID, ClsGlobal.m_redColor);
                }
                else
                {
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "横本数", "シングル");
                }
                if (m_tate == VerticalNum.Double)
                {
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "縦本数", "ダブル");
                    ClsRevitUtil.ChangeLineColor(doc, CreatedID, ClsGlobal.m_redColor);
                }
                else
                {
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "縦本数", "シングル");
                }
                ClsRevitUtil.SetMojiParameter(doc, CreatedID, "鋼材タイプ", m_kouzaiType);
                ClsRevitUtil.SetMojiParameter(doc, CreatedID, "鋼材サイズ", m_kouzaiSize);
                //ClsRevitUtil.SetMojiParameter(doc, CreatedID, "メガビーム", m_bMega.ToString());
                if (IsMega) ClsRevitUtil.SetParameter(doc, CreatedID, "メガビーム本数", m_mega);
                //ClsRevitUtil.SetMojiParameter(doc, CreatedID, "SMH", m_bSMH.ToString());
                ClsRevitUtil.SetParameter(doc, CreatedID, "オフセット量", ClsRevitUtil.CovertToAPI(m_offset));
                ClsRevitUtil.SetParameter(doc, CreatedID, "縦方向の隙間", ClsRevitUtil.CovertToAPI(m_tateGap));
                ClsRevitUtil.SetMojiParameter(doc, CreatedID, "隙間調整材鋼材ﾀｲﾌﾟ", m_gapTyouseiType);
                ClsRevitUtil.SetMojiParameter(doc, CreatedID, "隙間調整材", m_gapTyousei);
                ClsRevitUtil.SetParameter(doc, CreatedID, "隙間調整材長さ", ClsRevitUtil.CovertToAPI(m_gapTyouseiLenght));
                ClsRevitUtil.SetParameter(doc, CreatedID, "勝ち負け", m_katimake == WinLose.Win ? "勝ち" : "負け");

                t.Commit();
            }
            return true;
        }

        /// <summary>
        /// ベースコピー機能で使用する作成処理　オフセットは行わない※すでにオフセットされた座標をセットする想定なので
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="tmpStPoint"></param>
        /// <param name="tmpEdPoint"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        public bool CreateHaraokoshiBase(Document doc, XYZ tmpStPoint, XYZ tmpEdPoint)
        {
            string symbolFolpath = ClsZumenInfo.GetYMSFolder();
            string shinfamily = System.IO.Path.Combine(symbolFolpath, "ベース関係\\" + baseName + ".rfa");

            //シンボル読込
            if (!ClsRevitUtil.LoadFamilySymbolData(doc, shinfamily, baseName, out FamilySymbol sym))
            {
                return false;
            }

            XYZ Direction = Line.CreateBound(tmpStPoint, tmpEdPoint).Direction;//単位ベクトル取得

            ElementId levelID = ClsRevitUtil.GetLevelID(doc, m_level);

            Curve cv = Line.CreateBound(tmpStPoint, tmpEdPoint);

            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                t.Start();
                ElementId CreatedID = ClsRevitUtil.Create(doc, cv, levelID, sym);

                ClsRevitUtil.SetMojiParameter(doc, CreatedID, "段", m_dan);
                if (m_yoko == SideNum.Double)
                {
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "横本数", "ダブル");
                    ClsRevitUtil.ChangeLineColor(doc, CreatedID, ClsGlobal.m_redColor);
                }
                else
                {
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "横本数", "シングル");
                }
                if (m_tate == VerticalNum.Double)
                {
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "縦本数", "ダブル");
                    ClsRevitUtil.ChangeLineColor(doc, CreatedID, ClsGlobal.m_redColor);
                }
                else
                {
                    ClsRevitUtil.SetMojiParameter(doc, CreatedID, "縦本数", "シングル");
                }
                ClsRevitUtil.SetMojiParameter(doc, CreatedID, "鋼材タイプ", m_kouzaiType);
                ClsRevitUtil.SetMojiParameter(doc, CreatedID, "鋼材サイズ", m_kouzaiSize);
                //ClsRevitUtil.SetMojiParameter(doc, CreatedID, "メガビーム", m_bMega.ToString());
                if (IsMega) ClsRevitUtil.SetParameter(doc, CreatedID, "メガビーム本数", m_mega);
                //ClsRevitUtil.SetMojiParameter(doc, CreatedID, "SMH", m_bSMH.ToString());
                ClsRevitUtil.SetParameter(doc, CreatedID, "オフセット量", ClsRevitUtil.CovertToAPI(m_offset));
                ClsRevitUtil.SetParameter(doc, CreatedID, "縦方向の隙間", ClsRevitUtil.CovertToAPI(m_tateGap));
                ClsRevitUtil.SetMojiParameter(doc, CreatedID, "隙間調整材鋼材ﾀｲﾌﾟ", m_gapTyouseiType);
                ClsRevitUtil.SetMojiParameter(doc, CreatedID, "隙間調整材", m_gapTyousei);
                ClsRevitUtil.SetParameter(doc, CreatedID, "隙間調整材長さ", ClsRevitUtil.CovertToAPI(m_gapTyouseiLenght));
                ClsRevitUtil.SetParameter(doc, CreatedID, "勝ち負け", m_katimake == WinLose.Win ? "勝ち" : "負け");

                t.Commit();
            }
            return true;
        }

        public bool ChangeInterSectionHaraokoshiBase(Document doc, ElementId haraBaseId)
        {
            FamilyInstance instHaraBase = doc.GetElement(haraBaseId) as FamilyInstance;
            Curve cvBase = (instHaraBase.Location as LocationCurve).Curve;

            List<ElementId> insecIdList = new List<ElementId>();

            List<ElementId> allBaseIdList = new List<ElementId>();
            allBaseIdList.AddRange(ClsCornerHiuchiBase.GetAllCornerHiuchiBaseList(doc).ToList());
            allBaseIdList.AddRange(ClsKiribariHiuchiBase.GetAllKiribariHiuchiBaseList(doc).ToList());

            foreach (ElementId id in allBaseIdList)
            {
                FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                if (instHaraBase == inst)
                    continue;
                Curve cv = (inst.Location as LocationCurve).Curve;
                XYZ insec = ClsRevitUtil.GetIntersection(cvBase, cv);
                if (insec != null)//ClsRevitUtil.IsPointOnCurve(tgCv, sPnt))
                {
                    insecIdList.Add(id);
                }
            }

            List<ElementId> deleteList = new List<ElementId>();
            foreach (ElementId id in insecIdList)
            {
                FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                {
                    t.Start();
                    ClsRevitUtil.MoveFamilyInstance(inst, m_offset, instHaraBase.FacingOrientation);
                    t.Commit();
                }
                //(inst.Location as LocationCurve);
                //ClsRevitUtil.MoveFamilyInstance(inst, m_offset, inst.FacingOrientation);
                //switch(inst.Name)
                //{
                //    case ClsKiribariBase.baseName:
                //        {
                //            ClsKiribariBase clskiriBase = new ClsKiribariBase();
                //            FamilyInstance shinInstLevel = doc.GetElement(id) as FamilyInstance;
                //            LocationCurve lCurve = shinInstLevel.Location as LocationCurve;
                //            if (lCurve == null)
                //            {
                //                continue;
                //            }
                //            XYZ tmpStPoint = lCurve.Curve.GetEndPoint(0);
                //            XYZ tmpEdPoint = lCurve.Curve.GetEndPoint(1);

                //            XYZ insec = ClsRevitUtil.GetIntersection(cvBase, lCurve.Curve);

                //            if (!ClsRevitUtil.CheckNearGetEndPoint(lCurve.Curve, insec))
                //            {
                //                XYZ change = picPointE;
                //                picPointE = picPointS;
                //                picPointS = change;
                //                defVoid = false;
                //            }

                //            //元データを取得
                //            ElementId levelID = shinInstLevel.Host.Id;
                //            string dan = ClsRevitUtil.GetParameter(doc, id, "段");

                //            //切梁ベース変更
                //            clskiriBase.ChangeKiribariBase(doc, tmpStPoint, tmpEdPoint, dan, levelID);

                //            deleteList.Add(id);
                //            break;
                //        }
                //    case ClsCornerHiuchiBase.baseName:
                //        {
                //            ClsCornerHiuchiBase clsCHB = new ClsCornerHiuchiBase();
                //            clsCHB.SetParameter(doc, id);
                //            //変更する隅火打ベースに接している腹起ベースを2本取得する
                //            (clsCHB.m_HaraokoshiBaseID1, clsCHB.m_HaraokoshiBaseID2) = clsCHB.GetInsecHaraokoshiBase(doc, id);
                //            if (clsCHB.m_HaraokoshiBaseID1 == null || clsCHB.m_HaraokoshiBaseID2 == null)
                //            {
                //                continue;
                //            }
                //            FamilyInstance shinInstLevel = doc.GetElement(id) as FamilyInstance;
                //            ElementId levelID = shinInstLevel.Host.Id;
                //            string dan = ClsRevitUtil.GetParameter(doc, id, "段");


                //            //ベース作成
                //            XYZ pntS = new XYZ();
                //            XYZ pntE = new XYZ();
                //            if (clsCHB.GetBaseStartEndPoint(doc, ref pntS, ref pntE))
                //            {
                //                clsCHB.CreateCornerHiuchi(doc, pntS, pntE, levelID, dan);
                //            }

                //            deleteList.Add(id);
                //            break;
                //        }
                //    case ClsKiribariHiuchiBase.baseName:
                //        {
                //            ClsKiribariHiuchiBase clsKHB = new ClsKiribariHiuchiBase();

                //            FamilyInstance shinInstLevel = doc.GetElement(id) as FamilyInstance;
                //            LocationCurve lCurve = shinInstLevel.Location as LocationCurve;
                //            if (lCurve == null)
                //            {
                //                continue;
                //            }
                //            XYZ tmpStPoint = lCurve.Curve.GetEndPoint(0);
                //            XYZ tmpEdPoint = lCurve.Curve.GetEndPoint(1);

                //            //元データを取得
                //            ElementId levelID = shinInstLevel.Host.Id;
                //            string dan = ClsRevitUtil.GetParameter(doc, id, "段");

                //            //切梁-切梁or切梁-腹起のid
                //            (ElementId id1, ElementId id2) = clsKHB.GetInsecHaraokoshi_KiribariBase(doc, id);

                //            if (id1 == null || id2 == null)
                //            {
                //                continue;
                //            }

                //            XYZ pntS = new XYZ();
                //            XYZ pntE = new XYZ();
                //            XYZ insec = new XYZ();

                //            FamilyInstance inst1 = doc.GetElement(id1) as FamilyInstance;
                //            LocationCurve lCurve1 = inst1.Location as LocationCurve;
                //            //選択した切梁火打の始点と取得したベースが同一線上にあるかの確認
                //            if (!ClsRevitUtil.IsPointOnCurve(lCurve1.Curve, tmpStPoint))
                //            {
                //                ElementId c = id1;
                //                id1 = id2;
                //                id2 = c;
                //            }

                //            if (clsKHB.GetBaseStartEndPoint(doc, id1, id2, ref pntS, ref pntE, ref insec))
                //            {
                //                XYZ newPoint = pntS;
                //                XYZ newEndPoint = pntE;
                //                if (!ClsRevitUtil.JudgeOnLine(insec, pntS, tmpStPoint))
                //                {
                //                    Line kiriLine = Line.CreateBound(insec, pntS);
                //                    XYZ dirKiriLine = kiriLine.Direction;
                //                    newPoint = ClsRevitUtil.GetDistanceNewPoint(insec, -dirKiriLine, ClsRevitUtil.CovertFromAPI(kiriLine.Length));
                //                }

                //                if (!ClsRevitUtil.JudgeOnLine(insec, pntE, tmpEdPoint))
                //                {
                //                    Line kiriLine = Line.CreateBound(insec, pntE);
                //                    XYZ dirKiriLine = kiriLine.Direction;
                //                    newEndPoint = ClsRevitUtil.GetDistanceNewPoint(insec, -dirKiriLine, ClsRevitUtil.CovertFromAPI(kiriLine.Length));
                //                }

                //                clsKHB.CreateKiribariHiuchiBase(doc, newPoint, newEndPoint, levelID, dan);
                //            }
                //            deleteList.Add(id);
                //            break;
                //        }
                //    default:
                //        {
                //            ClsRevitUtil.MoveFamilyInstance(inst, m_offset, inst.FacingOrientation);
                //            break;
                //        }
                //}
            }

            //元のベースを削除
            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                t.Start();
                ClsRevitUtil.Delete(doc, deleteList);
                t.Commit();
            }

            return true;
        }
        public bool ChangeHaraokoshiBase(Document doc, ElementId id)
        {
            FamilyInstance inst = doc.GetElement(id) as FamilyInstance;

            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                t.Start();

                ClsRevitUtil.MoveFamilyInstance(inst, m_offset, inst.FacingOrientation);

                ElementId levelID = ClsRevitUtil.GetLevelID(doc, m_level);
                //if (levelID != null)
                //{
                //    try
                //    {
                //        //inst.get_Parameter(BuiltInParameter.SKETCH_PLANE_PARAM).Set(levelID);
                //        ClsRevitUtil.ModifyLevel(doc, id, levelID);
                //        // Get family associated with this
                //        Family family = inst.Symbol.Family;
                //        //inst.
                //        // Get Family document for family
                //        Document familyDoc = doc.EditFamily(family);
                //        if (null != familyDoc && familyDoc.IsFamilyDocument == true)
                //        {
                //            String loadedFamilies = "FamilySymbols in " + family.Name + ":\n";
                //            FilteredElementCollector collector = new FilteredElementCollector(familyDoc);
                //            ICollection<Element> collection =
                //                collector.OfClass(typeof(FamilySymbol)).ToElements();
                //            foreach (Element e in collection)
                //            {
                //                FamilySymbol fs = e as FamilySymbol;
                //                loadedFamilies += "\t" + fs.Name + "\n";
                //            }

                //            TaskDialog.Show("Revit", loadedFamilies);
                //        }
                //    }
                //    catch { }
                //}
                //ClsRevitUtil.SetMojiParameter(doc, id, "作業面", m_level);
                ClsRevitUtil.SetMojiParameter(doc, id, "段", m_dan);
                if (m_yoko == SideNum.Double)
                {
                    ClsRevitUtil.SetMojiParameter(doc, id, "横本数", "ダブル");
                    ClsRevitUtil.ChangeLineColor(doc, id, ClsGlobal.m_redColor);
                }
                else
                {
                    ClsRevitUtil.SetMojiParameter(doc, id, "横本数", "シングル");
                }
                if (m_tate == VerticalNum.Double)
                {
                    ClsRevitUtil.SetMojiParameter(doc, id, "縦本数", "ダブル");
                    ClsRevitUtil.ChangeLineColor(doc, id, ClsGlobal.m_redColor);
                }
                else
                {
                    ClsRevitUtil.SetMojiParameter(doc, id, "縦本数", "シングル");
                }
                ClsRevitUtil.SetMojiParameter(doc, id, "鋼材タイプ", m_kouzaiType);
                ClsRevitUtil.SetMojiParameter(doc, id, "鋼材サイズ", m_kouzaiSize);
                //ClsRevitUtil.SetMojiParameter(doc, CreatedID, "メガビーム", m_bMega.ToString());
                if (IsMega) ClsRevitUtil.SetParameter(doc, id, "メガビーム本数", m_mega);
                //ClsRevitUtil.SetMojiParameter(doc, CreatedID, "SMH", m_bSMH.ToString());
                ClsRevitUtil.SetParameter(doc, id, "オフセット量", ClsRevitUtil.CovertToAPI(m_offset));
                ClsRevitUtil.SetParameter(doc, id, "縦方向の隙間", ClsRevitUtil.CovertToAPI(m_tateGap));
                ClsRevitUtil.SetMojiParameter(doc, id, "隙間調整材鋼材ﾀｲﾌﾟ", m_gapTyouseiType);
                ClsRevitUtil.SetMojiParameter(doc, id, "隙間調整材", m_gapTyousei);
                ClsRevitUtil.SetParameter(doc, id, "隙間調整材長さ", ClsRevitUtil.CovertToAPI(m_gapTyouseiLenght));
                ClsRevitUtil.SetParameter(doc, id, "勝ち負け", m_katimake == WinLose.Win ? "勝ち" : "負け");

                t.Commit();
            }
            return true;
        }
        /// <summary>
        /// 図面上の腹起ベースを全て取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="levelId"></param>
        /// <returns></returns>
        public static List<ElementId> GetAllHaraokoshiBaseList(Document doc, ElementId levelId = null)
        {
            //図面上の腹起ベースを全て取得
            List<ElementId> haraIdList = new List<ElementId>();
            if (levelId == null)
                haraIdList = ClsRevitUtil.GetSelectCreatedFamilyInstanceList(doc, baseName);
            else
                haraIdList = ClsRevitUtil.GetSelectLevelCreatedFamilyInstanceList(doc, baseName, levelId);
            return haraIdList;
        }
        /// <summary>
        /// 図面上の腹起を全て取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<ElementId> GetAllHaraokoshiList(Document doc)
        {
            //図面上の腹起を全て取得
            List<ElementId> haraIdList = ClsRevitUtil.GetSelectCreatedFamilyInstanceList(doc, "腹起", true);
            return haraIdList;
        }
        /// <summary>
        ///  図面上の腹起ベースを全てクラスで取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<ClsHaraokoshiBase> GetAllClsHaraokoshiBaseList(Document doc)
        {
            List<ClsHaraokoshiBase> lstBase = new List<ClsHaraokoshiBase>();

            List<ElementId> lstId = GetAllHaraokoshiBaseList(doc);
            foreach (ElementId id in lstId)
            {
                ClsHaraokoshiBase clsH = new ClsHaraokoshiBase();
                clsH.SetParameter(doc, id);
                lstBase.Add(clsH);
            }

            return lstBase;
        }

        /// <summary>
        /// 指定したIDから腹起ベースクラスを取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public void SetParameter(Document doc, ElementId id)
        {

            FamilyInstance shinInstLevel = doc.GetElement(id) as FamilyInstance;
            m_level = shinInstLevel.Host.Name;
            m_dan = ClsRevitUtil.GetParameter(doc, id, "段");
            m_yoko = ClsRevitUtil.GetParameter(doc, id, "横本数") == "シングル" ? ClsHaraokoshiBase.SideNum.Single : ClsHaraokoshiBase.SideNum.Double;
            m_tate = ClsRevitUtil.GetParameter(doc, id, "縦本数") == "シングル" ? ClsHaraokoshiBase.VerticalNum.Single : ClsHaraokoshiBase.VerticalNum.Double;

            m_kouzaiType = ClsRevitUtil.GetParameter(doc, id, "鋼材タイプ");
            m_kouzaiSize = ClsRevitUtil.GetParameter(doc, id, "鋼材サイズ");
            //m_bMega = ClsRevitUtil.GetParameter(doc, id, "メガビーム") == bool.TrueString ? true : false;
            if (IsMega) m_mega = (int)ClsRevitUtil.GetParameterInteger(doc, id, "メガビーム本数");
            //m_bSMH = ClsRevitUtil.GetParameter(doc, id, "SMH") == bool.TrueString ? true : false;
            m_offset = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetParameterDouble(doc, id, "オフセット量"));
            m_tateGap = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetParameterDouble(doc, id, "縦方向の隙間"));
            m_gapTyouseiType = ClsRevitUtil.GetParameter(doc, id, "隙間調整材鋼材ﾀｲﾌﾟ");
            m_gapTyousei = ClsRevitUtil.GetParameter(doc, id, "隙間調整材");
            m_gapTyouseiLenght = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetParameterDouble(doc, id, "隙間調整材長さ"));
            m_katimake = ClsRevitUtil.GetParameter(doc, id, "勝ち負け") == "勝ち" ? WinLose.Win : WinLose.Lose;
            //m_Floor = shinInstLevel.Host.Name;
            m_ElementId = id;

            return;
        }

        public bool CreateHojyoPeace(Document doc, XYZ tmpStPoint, XYZ tmpEdPoint, double haraokoshiTateOffset, double oop = 1)
        {

            ElementId baseID = m_ElementId;
            if (baseID == null)
            {
                return false;
            }

            //ベース
            FamilyInstance ShinInstLevel = doc.GetElement(baseID) as FamilyInstance;
            ElementId levelID = ShinInstLevel.Host.Id;
            string dan = m_dan;
            double size = ClsRevitUtil.CovertToAPI(Master.ClsYamadomeCsv.GetWidth(m_kouzaiSize) / 2);
            if (m_tate == VerticalNum.Double)
            {
                size += ClsRevitUtil.CovertToAPI(m_tateGap / 2);
                if (dan == "同段")//同段の場合は上段に変える
                {
                    dan = "上段";
                }
            }

            //Element inst = doc.GetElement(baseID);
            //LocationCurve lCurve = inst.Location as LocationCurve;
            //if (lCurve == null)
            //{
            //    return false;
            //}

            //XYZ tmpStPoint = lCurve.Curve.GetEndPoint(0);
            //XYZ tmpEdPoint = lCurve.Curve.GetEndPoint(1);
            XYZ dir = Line.CreateBound(tmpStPoint, tmpEdPoint).Direction;

            string hojyo2 = Master.ClsSupportPieceCsv.GetSize(m_kouzaiSize, 2);
            string path2 = Master.ClsSupportPieceCsv.GetFamilyPath(hojyo2);
            string familyName2 = RevitUtil.ClsRevitUtil.GetFamilyName(path2);

            string hojyo3 = Master.ClsSupportPieceCsv.GetSize(m_kouzaiSize, 3);
            string path3 = Master.ClsSupportPieceCsv.GetFamilyPath(hojyo3);
            string familyName3 = RevitUtil.ClsRevitUtil.GetFamilyName(path3);

            if (!ClsRevitUtil.LoadFamilyData(doc, path2, out Family fam2))
            {
                return false;
            }
            FamilySymbol sym2 = (ClsRevitUtil.GetFamilySymbol(doc, familyName2, "腹起"));

            if (!ClsRevitUtil.LoadFamilyData(doc, path3, out Family fam3))
            {
                return false;
            }
            FamilySymbol sym3 = (ClsRevitUtil.GetFamilySymbol(doc, familyName3, "腹起"));

            using (Transaction tx = new Transaction(doc, "Load Family"))
            {
                tx.Start();
                //■■■始点側処理■■■
                if (sym2 != null)
                {
                    ElementId CreatedID = ClsRevitUtil.Create(doc, tmpStPoint, levelID, sym2);
                    FamilyInstance ch = doc.GetElement(CreatedID) as FamilyInstance;

                    XYZ dirV = ch.HandOrientation;//new XYZ(1, 0, 0);//ファミリインスタンスの向き
                    double dVectAngle = dirV.AngleOnPlaneTo(dir, XYZ.BasisZ);
                    Line axis = Line.CreateBound(tmpStPoint, tmpStPoint + XYZ.BasisZ);

                    ClsRevitUtil.RotateElement(doc, CreatedID, axis, dVectAngle);
                    if (dan == "上段")
                        ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", (size * oop) + haraokoshiTateOffset);
                    else if (dan == "下段")
                        ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", (-size * oop) + haraokoshiTateOffset);
                    else
                        ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", haraokoshiTateOffset);
                    ClsKariKouzai.SetBaseId(doc, CreatedID, baseID);
                    ClsKariKouzai.SetKariKouzaiFlag(doc, CreatedID);
                }
                //■■■終点側処理■■■
                if (sym3 != null)
                {
                    ElementId CreatedID = ClsRevitUtil.Create(doc, tmpEdPoint, levelID, sym3);
                    FamilyInstance ch = doc.GetElement(CreatedID) as FamilyInstance;

                    XYZ dirV = ch.HandOrientation;//new XYZ(1, 0, 0);//ファミリインスタンスの向き
                    double dVectAngle = dirV.AngleOnPlaneTo(dir, XYZ.BasisZ) + Math.PI;
                    Line axis = Line.CreateBound(tmpEdPoint, tmpEdPoint + XYZ.BasisZ);

                    ClsRevitUtil.RotateElement(doc, CreatedID, axis, dVectAngle);

                    if (dan == "上段")
                        ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", (size * oop) + haraokoshiTateOffset);
                    else if (dan == "下段")
                        ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", (-size * oop) + haraokoshiTateOffset);
                    else
                        ClsRevitUtil.SetParameter(doc, CreatedID, "基準レベルからの高さ", haraokoshiTateOffset);
                    ClsKariKouzai.SetBaseId(doc, CreatedID, baseID);
                    ClsKariKouzai.SetKariKouzaiFlag(doc, CreatedID);
                }
                tx.Commit();
            }
            return true;
        }

        public bool CreateKarikouzaiMegabeam(Document doc, XYZ tmpStPoint, XYZ tmpEdPoint, out bool megaFull)
        {
            megaFull = false;
            ElementId baseID = m_ElementId;
            if (baseID == null)
            {
                return false;
            }

            if (m_kouzaiType != "メガビーム")
                return false;

            if (m_yoko != ClsHaraokoshiBase.SideNum.Double)
                return false;

            //ベース
            FamilyInstance ShinInstLevel = doc.GetElement(baseID) as FamilyInstance;
            ElementId levelID = ShinInstLevel.Host.Id;
            string dan = ClsRevitUtil.GetParameter(doc, baseID, "段");

            //80HAシンボルを読み込む
            string buzaiName = "腹起";
            string familyPath = Master.ClsYamadomeCsv.GetKariFamilyPath(m_kouzaiSize);
            string symbolName = ClsRevitUtil.GetFamilyName(familyPath);

            if (!ClsRevitUtil.LoadFamilyData(doc, familyPath, out Family kouFam))
            {
                return false;
            }
            FamilySymbol kouzaiSym = ClsRevitUtil.GetFamilySymbol(doc, symbolName, buzaiName);

            // 線分の情報を取得
            XYZ lineStartPoint = tmpStPoint;
            XYZ lineEndPoint = tmpEdPoint;
            XYZ lineMidPoint = (tmpStPoint + tmpEdPoint) * 0.5;
            double lineLength = lineEndPoint.DistanceTo(lineStartPoint);
            XYZ directionVector = (lineEndPoint - lineStartPoint).Normalize();

            // メガビームの本数を取得
            double megabeamLength = ClsRevitUtil.ConvertDoubleGeo2Revit(9000.0);
            int megaBeamCount = m_mega;
            if (megaBeamCount == 0)
            {
                return false;
            }

            double totalMegaBeamLength = megaBeamCount * megabeamLength;
            if (totalMegaBeamLength > lineLength)
            {
                TaskDialog.Show("エラー", "部材の長さが線分の長さを上回ります。");
                return false;
            }

            double surplusLength = lineLength - totalMegaBeamLength;
            XYZ insertPoint = lineStartPoint + (directionVector * surplusLength / 2);

            // シンボルを回転させるための準備
            double radians = XYZ.BasisX.AngleOnPlaneTo(directionVector, XYZ.BasisZ);        // シンボルの向き（デフォルトは3時方向）
            Line rotationAxis = Line.CreateBound(insertPoint, insertPoint + XYZ.BasisZ);    // Z軸周りに回転

            // シンボルを芯に合うようにするための準備
            double angleInRadians = -Math.PI / 2.0; // 90度をラジアンに変換
            Transform rotationTransform = Transform.CreateRotationAtPoint(XYZ.BasisZ, angleInRadians, lineStartPoint);
            XYZ leftVector = rotationTransform.OfVector(directionVector);// ベクトルを回転して左側のベクトルを得る

            double dKouzaiSize = 0.0;
            if (dan == "上段")
            {
                dKouzaiSize = ClsRevitUtil.ConvertDoubleGeo2Revit(200.0);
            }
            else if (dan == "下段")
            {
                dKouzaiSize = ClsRevitUtil.ConvertDoubleGeo2Revit(-200.0);
            }


            using (Transaction tx = new Transaction(doc, "Mega Family"))
            {
                tx.Start();
                if (kouzaiSym != null)
                {
                    // メガビーム腹起を作成
                    for (int i = 0; i < megaBeamCount; i++)
                    {

                        // シンボルを作成
                        ElementId CreatedID = ClsRevitUtil.Create(doc, insertPoint, levelID, kouzaiSym);
                        //選択された芯の段によって調整
                        ClsRevitUtil.SetParameter(doc, CreatedID, ClsGlobal.m_refLvTop, dKouzaiSize);

                        // シンボルを回転
                        rotationAxis = Line.CreateBound(insertPoint, insertPoint + XYZ.BasisZ);    // Z軸周りに回転
                        ElementTransformUtils.RotateElement(doc, CreatedID, rotationAxis, radians);

                        // シンボル腹起ベースに合うように移動
                        ElementTransformUtils.MoveElement(doc, CreatedID, leftVector * ClsRevitUtil.ConvertDoubleGeo2Revit(200.0));
                        ClsKariKouzai.SetBaseId(doc, CreatedID, baseID);
                        ClsKariKouzai.SetKariKouzaiFlag(doc, CreatedID);

                        if (m_tate == ClsHaraokoshiBase.VerticalNum.Double)
                        {
                            CreatedID = ClsRevitUtil.Create(doc, insertPoint, levelID, kouzaiSym);
                            //選択された芯の段によって調整
                            ClsRevitUtil.SetParameter(doc, CreatedID, ClsGlobal.m_refLvTop, -dKouzaiSize);

                            ElementTransformUtils.RotateElement(doc, CreatedID, rotationAxis, radians);

                            // シンボル腹起ベースに合うように移動
                            ElementTransformUtils.MoveElement(doc, CreatedID, leftVector * ClsRevitUtil.ConvertDoubleGeo2Revit(200.0));
                            ClsKariKouzai.SetBaseId(doc, CreatedID, baseID);
                        }

                        insertPoint += directionVector * megabeamLength;
                    }
                }
                tx.Commit();
            }

            //メガビーム配置後の残り長さが100以下かをチェック
            if (surplusLength < ClsRevitUtil.ConvertDoubleGeo2Revit(100.0))
            {
                megaFull = true;
            }
            return true;
        }
        public bool CreateGapTyousei(Document doc, XYZ tmpStPoint, XYZ tmpEdPoint, double dKouzaiSize)
        {
            if (m_gapTyouseiLenght <= 0)
                return false;
            if (m_tate != ClsHaraokoshiBase.VerticalNum.Double)
                return false;

            ElementId baseID = m_ElementId;
            if (baseID == null)
            {
                return false;
            }

            //ベース
            FamilyInstance ShinInstLevel = doc.GetElement(baseID) as FamilyInstance;
            ElementId levelID = ShinInstLevel.Host.Id;

            XYZ dir = Line.CreateBound(tmpStPoint, tmpEdPoint).Direction;
            tmpStPoint = new XYZ(tmpStPoint.X + (dKouzaiSize * dir.Y),
                                 tmpStPoint.Y - (dKouzaiSize * dir.X),
                                 tmpStPoint.Z);
            tmpEdPoint = new XYZ(tmpEdPoint.X + (dKouzaiSize * dir.Y),
                                 tmpEdPoint.Y - (dKouzaiSize * dir.X),
                                 tmpEdPoint.Z);


            string path = string.Empty;
            string familyName = string.Empty;
            double size = 0;
            if (m_gapTyouseiType == SukimaH)
            {
                path = Master.ClsHBeamCsv.GetFamilyPath(m_gapTyousei);
                familyName = RevitUtil.ClsRevitUtil.GetFamilyName(path);
            }
            else if (m_gapTyouseiType == SukimaC)
            {
                path = Master.ClsChannelCsv.GetFamilyPath(m_gapTyousei);
                familyName = RevitUtil.ClsRevitUtil.GetFamilyName(path);
                size = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou(familyName, 2)) / 2);
            }
            else
            {
                return false;
            }

            if (!ClsRevitUtil.LoadFamilyData(doc, path, out Family fam))
            {
                return false;
            }
            FamilySymbol sym = (ClsRevitUtil.GetFamilySymbol(doc, familyName, GapTyousei));

            using (Transaction tx = new Transaction(doc, "Load Family"))
            {
                tx.Start();
                //■■■始点側処理■■■
                if (sym != null)
                {
                    double length = ClsRevitUtil.CovertToAPI(m_gapTyouseiLenght);
                    XYZ point = (tmpStPoint + tmpEdPoint) * 0.5;
                    XYZ endPoint = new XYZ(point.X + length * dir.X, point.Y + length * dir.Y, point.Z);

                    Curve cv = Line.CreateBound(point, endPoint);
                    ElementId CreatedID = ClsRevitUtil.Create(doc, cv, levelID, sym);

                    Line axis = Line.CreateBound(point, point + XYZ.BasisZ);

                    ClsRevitUtil.RotateElement(doc, CreatedID, axis, Math.PI / 2);
                    //ベースの真ん中に付くため上下の概念がない
                    ClsRevitUtil.SetParameter(doc, CreatedID, "ホストからのオフセット", size);
                    ClsKariKouzai.SetBaseId(doc, CreatedID, baseID);
                }
                tx.Commit();
            }


            return true;
        }
        public bool CreateGapTyousei(Document doc, XYZ point)
        {
            if (m_gapTyouseiLenght <= 0)
                return false;
            if (m_tate != ClsHaraokoshiBase.VerticalNum.Double)
                return false;

            ElementId baseID = m_ElementId;
            if (baseID == null)
            {
                return false;
            }

            //ベース
            FamilyInstance inst = doc.GetElement(baseID) as FamilyInstance;
            ElementId levelID = inst.Host.Id;
            Curve cvBase = (inst.Location as LocationCurve).Curve;
            XYZ dir = (cvBase as Line).Direction;

            string path = string.Empty;
            string familyName = string.Empty;
            double size = 0;
            if (m_gapTyouseiType == SukimaH)
            {
                path = Master.ClsHBeamCsv.GetFamilyPath(m_gapTyousei);
                familyName = RevitUtil.ClsRevitUtil.GetFamilyName(path);
            }
            else if (m_gapTyouseiType == SukimaC)
            {
                path = Master.ClsChannelCsv.GetFamilyPath(m_gapTyousei);
                familyName = RevitUtil.ClsRevitUtil.GetFamilyName(path);
                size = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou(familyName, 2)) / 2);
            }
            else
            {
                return false;
            }

            if (!ClsRevitUtil.LoadFamilyData(doc, path, out Family fam))
            {
                return false;
            }
            FamilySymbol sym = (ClsRevitUtil.GetFamilySymbol(doc, familyName, GapTyousei));

            using (Transaction tx = new Transaction(doc, "Load Family"))
            {
                tx.Start();
                //■■■始点側処理■■■
                if (sym != null)
                {
                    double length = ClsRevitUtil.CovertToAPI(m_gapTyouseiLenght);
                    XYZ endPoint = new XYZ(point.X + length * dir.X, point.Y + length * dir.Y, point.Z);

                    Curve cv = Line.CreateBound(point, endPoint);
                    ElementId CreatedID = ClsRevitUtil.Create(doc, cv, levelID, sym);

                    Line axis = Line.CreateBound(point, point + XYZ.BasisZ);

                    ClsRevitUtil.RotateElement(doc, CreatedID, axis, Math.PI / 2);
                    //ベースの真ん中に付くため上下の概念がない
                    ClsRevitUtil.SetParameter(doc, CreatedID, "ホストからのオフセット", size);
                    ClsKariKouzai.SetBaseId(doc, CreatedID, baseID);
                }
                tx.Commit();
            }


            return true;
        }
        /// <summary>
        /// 対象のベースと接続する腹起ベースを取得
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id">対象のベースID</param>
        /// <returns></returns>
        public static List<ElementId> GetIntersectionBase(Document doc, ElementId id, ElementId levelId = null)
        {
            List<ElementId> insecIdList = new List<ElementId>();
            List<ElementId> targetFamilies = GetAllHaraokoshiBaseList(doc, levelId);

            FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
            Curve cvBase = (inst.Location as LocationCurve).Curve;

            //XYZ sPnt = cvBase.GetEndPoint(0);
            //XYZ ePnt = cvBase.GetEndPoint(1);

            foreach (ElementId tgId in targetFamilies)
            {
                FamilyInstance tgInst = doc.GetElement(tgId) as FamilyInstance;
                if (inst == tgInst)
                    continue;
                Curve tgCv = (tgInst.Location as LocationCurve).Curve;
                XYZ insec = ClsRevitUtil.GetIntersection(cvBase, tgCv);
                if (insec != null)
                {
                    insecIdList.Add(tgId);
                }
            }
            return insecIdList;
        }
        public static List<GetIntersectionBase2Result> GetIntersectionBase2(Document doc, ElementId id, ElementId levelId = null)
        {
            //var aa = GetIntersectionBase(doc, id);
            var insecIdList = new List<GetIntersectionBase2Result>();
            List<ElementId> targetFamilies = GetAllHaraokoshiBaseList(doc, levelId);

            FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
            //Curve cvBase = (inst.Location as LocationCurve).Curve;
            var cvBase = ClsYMSUtil.GetBaseLine(doc, id);
            //var cvBase = Line.CreateUnbound(cvBase0.Origin, cvBase0.Direction);

            foreach (ElementId tgId in targetFamilies)
            {
                var tgInst = doc.GetElement(tgId) as FamilyInstance;
                if (inst == tgInst)
                    continue;
                //Curve tgCv = (tgInst.Location as LocationCurve).Curve;
                var tgCv = ClsYMSUtil.GetBaseLine(doc, tgInst.Id);
                //var tgCv = Line.CreateUnbound(tgCv0.Origin, tgCv0.Direction);

                SLibRevitReo.ClosestLineAndLine(cvBase, tgCv, out var p0, out var p1);
                if (ClsGeo.GEO_EQ0(p0.DistanceTo(p1)))
                {
                    insecIdList.Add(new GetIntersectionBase2Result
                    {
                        Id = tgId,
                        Point = p1
                    });
                }
                //XYZ insec = ClsRevitUtil.GetIntersection(cvBase, tgCv);
                //if (insec != null)
                //{
                //    //insecIdList.Add();
                //    insecIdList.Add(new GetIntersectionBase2Result
                //    {
                //        Id = tgId,
                //        Point = insec
                //    });
                //}
            }
            return insecIdList;
        }
        /// <summary>
        /// モデル線分と腹起ベースを対象に
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id"></param>
        /// <param name="levelId"></param>
        /// <returns></returns>
        public static List<GetIntersectionBase2Result> GetIntersectionBase2ModelLine(Document doc, ElementId id, ElementId levelId = null)
        {
            var insecIdList = new List<GetIntersectionBase2Result>();
            List<ElementId> targetFamilies = new List<ElementId>();
            targetFamilies.AddRange(GetAllHaraokoshiBaseList(doc, levelId));
            targetFamilies.AddRange(ClsRevitUtil.GetALLModelLine(doc));
            var cvBase = ClsYMSUtil.GetBaseLine(doc, id);
            var targetStatements = SLibRevitReo.DivisionStatement.Terminal | SLibRevitReo.DivisionStatement.Internal;

            foreach (ElementId tgId in targetFamilies)
            {
                if (id == tgId)
                    continue;
                var tgCv = ClsYMSUtil.GetBaseLine(doc, tgId);

                if (!SLibRevitReo.ClosestLineAndLine(cvBase, tgCv, out var p0, out var p1))
                {
                    continue;
                }
                if (ClsGeo.GEO_EQ0(p0.DistanceTo(p1)))
                {
                    var division1 = SLibRevitReo.CalcDivisionStatement(cvBase, p0);
                    var division2 = SLibRevitReo.CalcDivisionStatement(tgCv, p0);
                    if (targetStatements.HasFlag(division1) && targetStatements.HasFlag(division2))
                    {
                        insecIdList.Add(new GetIntersectionBase2Result
                        {
                            Id = tgId,
                            Point = p1
                        });
                    }
                }
            }
            return insecIdList;
        }
        public class GetIntersectionBase2Result
        {
            public ElementId Id { get; set; }
            public XYZ Point { get; set; }
        }

        public static List<List<XYZ>> GetALLHaraokoshiBaseStartEndPointList(Document doc)
        {
            List<List<XYZ>> res = new List<List<XYZ>>();

            List<ElementId> haraBaseIdList = GetAllHaraokoshiBaseList(doc);
            List<LocationCurve> lcList = new List<LocationCurve>();

            foreach (ElementId id in haraBaseIdList)
            {
                FamilyInstance instance = doc.GetElement(id) as FamilyInstance;
                LocationCurve lCurve = instance.Location as LocationCurve;
                lcList.Add(lCurve);
            }
            res = ClsRevitUtil.GetCurveStartEndPoints(lcList);

            return res;
        }
        /// <summary>
        /// 腹起（割付主材）の挿入基点を取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<XYZ> GetALLHaraokoshInsertPointList(Document doc)
        {
            List<XYZ> res = new List<XYZ>();

            List<ElementId> haraIdList = GetAllHaraokoshiList(doc);

            foreach (ElementId id in haraIdList)
            {
                FamilyInstance instance = doc.GetElement(id) as FamilyInstance;
                LocationPoint lPoint = instance.Location as LocationPoint;
                if (lPoint == null)
                    continue;
                res.Add(lPoint.Point);
            }

            return res;
        }
        /// <summary>
        /// 腹起ベース のみを単独選択
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="id">選択した 腹起ベース のID</param>
        /// <param name="message">指示</param>
        /// <returns></returns>
        public static bool PickBaseObject(UIDocument uidoc, ref ElementId id, string message = baseName)
        {
            return ClsRevitUtil.PickObject(uidoc, message, baseName, ref id);
        }
        /// <summary>
        /// 腹起ベース のみを単独選択
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="id">選択した 腹起ベース のID</param>
        /// <param name="message">指示</param>
        /// <returns></returns>
        public static bool PickBaseObject(UIDocument uidoc, ref Reference rf, string message = baseName)
        {
            return ClsRevitUtil.PickObject(uidoc, message, baseName, ref rf);
        }

        /// <summary>
        /// 腹起ベース のみを複数選択
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="ids"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool PickBaseObjects(UIDocument uidoc, ref List<ElementId> ids, string message = baseName)
        {
            return ClsRevitUtil.PickObjectsPartFilter(uidoc, message + "を選択してください", baseName, ref ids);
        }

        /// <summary>
        /// 入隅かどうか判定する
        /// </summary>
        /// <param name="cv1"></param>
        /// <param name="cv2"></param>
        /// <returns></returns>
        public static bool CheckIrizumi(Curve cv1, Curve cv2, ref bool res)
        {
            XYZ pIntersect = RevitUtil.ClsRevitUtil.GetIntersection(cv1, cv2);
            if (pIntersect == null)
            {
                return false;
            }

            bool bAFirst = false;
            if (pIntersect.DistanceTo(cv1.GetEndPoint(1)) <= pIntersect.DistanceTo(cv2.GetEndPoint(1)))
                bAFirst = true;

            XYZ vec1 = new XYZ();
            XYZ p = new XYZ();
            if (bAFirst)
            {
                vec1 = cv1.GetEndPoint(1) - cv1.GetEndPoint(0);
                p = cv2.GetEndPoint(1) - cv1.GetEndPoint(0);
            }
            else
            {
                vec1 = cv2.GetEndPoint(1) - cv2.GetEndPoint(0);
                p = cv1.GetEndPoint(1) - cv2.GetEndPoint(0);
            }

            XYZ crossProduct = vec1.CrossProduct(p);
            if (crossProduct.Z > 0)
            {
                res = true;
            }
            else if (crossProduct.Z < 0)
            {
                res = false;
            }
            else
            {
                res = false;
                return false;
            }

            return true;
        }

        #endregion
    }
}
