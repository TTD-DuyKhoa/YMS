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

namespace YMS
{
    public class ClsKabeShin
    {
        #region 定数
        const string familyName = "壁芯";
        public const string GL = "GL";// "GL±0";
        const string PUTORDER = "配置順";
        #endregion
        public static bool CreateKabeShin(UIDocument uidoc)
        {
            try
            {
                Document doc = uidoc.Document;
                DLG.DlgGL dlgGL = new DLG.DlgGL(doc);
                DialogResult result = dlgGL.ShowDialog();
                if (result == DialogResult.Cancel)
                {
                    return false;
                }
                string levelName = dlgGL.m_Level;
                //手動作成に類似
                for (; ; )
                {
                   
                    ElementId selShin = null;
                    if (!ClsRevitUtil.PickObject(uidoc, "モデル線分", "モデル線分", ref selShin))
                    {
                        return false;
                    }
                    ElementId createId = null;
                    CreateKabeShin(uidoc, levelName, selShin, ref createId);
                    //CreateKabeShin(uidoc, createId);
                }

            }
            catch
            {

            }
            return true;
        }
        /// <summary>
        /// 壁芯を1本作成する処理
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="selShinid">選択したモデル線分</param>
        /// <param name="createId">作成した壁芯</param>
        /// <returns></returns>
        private static bool CreateKabeShin(UIDocument uidoc, string levelName, ElementId selShinid, ref ElementId createId)
        {
            Document doc = uidoc.Document;
            string symbolFolpath = ClsZumenInfo.GetYMSFolder();
            string familyPath = System.IO.Path.Combine(symbolFolpath, "ベース関係\\" + familyName + ".rfa");

            //シンボル配置           
            if (!ClsRevitUtil.LoadFamilySymbolData(doc, familyPath, familyName, out FamilySymbol sym))
            {
                return false;
            }
            //掘削側を指定
            (XYZ tmpStPoint, XYZ tmpEdPoint) = ClsVoid.SelectVoidSide(uidoc, selShinid);
            if (tmpStPoint == null || tmpEdPoint == null) return false;

            XYZ Direction = Line.CreateBound(tmpStPoint, tmpEdPoint).Direction;//単位ベクトル取得
            Curve cv = Line.CreateBound(tmpStPoint, tmpEdPoint);

            ElementId levelID = ClsRevitUtil.GetLevelID(doc, levelName);

            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                try
                {
                    t.Start();
                    FailureHandlingOptions failOpt = t.GetFailureHandlingOptions();
                    failOpt.SetFailuresPreprocessor(new WarningSwallower());
                    t.SetFailureHandlingOptions(failOpt);

                    ElementId CreatedID = ClsRevitUtil.Create(doc, cv, levelID, sym);
                    
                    //変更が加わるメッセージ原因不明
                    t.Commit();

                    createId = CreatedID;

                }
                catch (Exception ex)
                {
                    TaskDialog.Show("Info Message13", ex.Message);
                    string message = ex.Message;
                    MessageBox.Show(message);
                }
            }//using

            return true;
        }
        // <summary>
        /// 壁芯を1本作成する処理
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="selShinid">選択したモデル線分</param>
        /// <param name="createId">作成した壁芯</param>
        /// <returns></returns>
        public static bool CreateKabeShin(Document doc, string levelName, XYZ tmpStPoint, XYZ tmpEdPoint, ref ElementId createId)
        {
            string symbolFolpath = ClsZumenInfo.GetYMSFolder();
            string familyPath = System.IO.Path.Combine(symbolFolpath, "ベース関係\\" + familyName + ".rfa");

            //シンボル配置           
            if (!ClsRevitUtil.LoadFamilySymbolData(doc, familyPath, familyName, out FamilySymbol sym))
            {
                return false;
            }
            if (tmpStPoint == null || tmpEdPoint == null) return false;

            XYZ Direction = Line.CreateBound(tmpStPoint, tmpEdPoint).Direction;//単位ベクトル取得
            Curve cv = Line.CreateBound(tmpStPoint, tmpEdPoint);

            ElementId levelID = ClsRevitUtil.GetLevelID(doc, levelName);

            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                try
                {
                    t.Start();
                    FailureHandlingOptions failOpt = t.GetFailureHandlingOptions();
                    failOpt.SetFailuresPreprocessor(new WarningSwallower());
                    t.SetFailureHandlingOptions(failOpt);

                    ElementId CreatedID = ClsRevitUtil.Create(doc, cv, levelID, sym);

                    //変更が加わるメッセージ原因不明
                    t.Commit();

                    createId = CreatedID;

                }
                catch (Exception ex)
                {
                    TaskDialog.Show("Info Message13", ex.Message);
                    string message = ex.Message;
                    MessageBox.Show(message);
                }
            }//using

            return true;
        }
        /// <summary>
        /// 基準の壁芯を元に壁芯を自動で作図する処理
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="createId">基準となる壁芯</param>
        /// <returns></returns>
        private static bool CreateKabeShin(UIDocument uidoc, ElementId createId, string levelName)
        {
            //Document doc = uidoc.Document;
            //string symbolFolpath = ClsZumenInfo.GetYMSFolder();
            //string familyPath = System.IO.Path.Combine(symbolFolpath, "山留壁ファミリ\\familyName.rfa");

            //using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            //{
            //    try
            //    {
            //        Selection selection = uidoc.Selection;
            //        Element selShin = doc.GetElement(createId);
            //        LocationCurve lCurve = selShin.Location as LocationCurve;
            //        if (lCurve == null)
            //        {
            //            return false;
            //        }
            //        XYZ picPointS = lCurve.Curve.GetEndPoint(0);
            //        XYZ picPointE = lCurve.Curve.GetEndPoint(1);
            //        Curve orgCv = Line.CreateBound(picPointS, picPointE);
            //        Curve copyCv = orgCv;

            //        //シンボル読込
            //        if (!ClsRevitUtil.RoadFamilySymbolData(doc, familyPath, familyName, out FamilySymbol sym))
            //        {
            //            return false;
            //        }

            //        //図面のモデル線分を全て取得
            //        List<ElementId> modelLineList = ClsRevitUtil.GetALLModelLine(doc);
            //        for (; ; )
            //        {
            //            Curve cv = null;
            //            foreach (ElementId id in modelLineList)
            //            {
            //                Element modelLine = doc.GetElement(id);
            //                Curve cvShin = (modelLine.Location as LocationCurve).Curve;
            //                XYZ mlS = cvShin.GetEndPoint(0);
            //                XYZ mlE = cvShin.GetEndPoint(1);

            //                //1つ前に作成した壁芯と同様かのチェック
            //                if (ClsGeo.GEO_EQ(cvShin, copyCv))
            //                {
            //                    continue;
            //                }
            //                if (ClsGeo.GEO_EQ(mlS, picPointE))
            //                {
            //                    cv = Line.CreateBound(mlS, mlE);
            //                    picPointE = cv.GetEndPoint(1);
            //                    copyCv = cv;
            //                    break;
            //                }
            //                if (ClsGeo.GEO_EQ(mlE, picPointE))
            //                {
            //                    cv = Line.CreateBound(mlE, mlS);
            //                    picPointE = cv.GetEndPoint(1);
            //                    copyCv = cv;
            //                    break;
            //                }
            //            }
            //            if (cv == null) return false;

            //            //初期に作成した壁芯と同様かのチェック
            //            if(ClsGeo.GEO_EQ(cv, orgCv))
            //            {
            //                break;
            //            }

            //            ElementId levelID = ClsRevitUtil.GetLevelID(doc, levelName);

            //            t.Start();
            //            //FailureHandlingOptions failOpt = t.GetFailureHandlingOptions();
            //            //failOpt.SetFailuresPreprocessor(new WarningSwallower());
            //            //t.SetFailureHandlingOptions(failOpt);

            //            ElementId CreatedID = ClsRevitUtil.Create(doc, cv, levelID, sym);

            //            //変更が加わるメッセージ原因不明
            //            t.Commit();
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        TaskDialog.Show("Info Message14", ex.Message);
            //        string message = ex.Message;
            //        MessageBox.Show(message);
            //    }
            //}//using

            return true;
        }

        /// <summary>
        /// 壁芯 のみを単独選択
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="id">選択した 壁芯 のID</param>
        /// <param name="message">指示</param>
        /// <returns></returns>
        public static bool PickBaseObject(UIDocument uidoc, ref ElementId id, string message = familyName)
        {
            return ClsRevitUtil.PickObject(uidoc, message, familyName, ref id);
        }
        /// <summary>
        /// 壁芯 のみを単独選択
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="id">選択した 壁芯 のID</param>
        /// <param name="message">指示</param>
        /// <returns></returns>
        public static bool PickBaseObject(UIDocument uidoc, ref Reference rf, string message = familyName)
        {
            return ClsRevitUtil.PickObject(uidoc, message, familyName, ref rf);
        }
        /// <summary>
        /// 図面上の壁芯を全て取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<ElementId> GetAllKabeShinList(Document doc)
        {
            //図面上の壁芯を全て取得
            List<ElementId> idList = ClsRevitUtil.GetSelectCreatedFamilyInstanceList(doc, familyName);
            return idList;
        }
        public static List<List<XYZ>> GetALLKabeShinStartEndPointList(Document doc)
        {
            List<List<XYZ>> res = new List<List<XYZ>>();

            List<ElementId> idList = GetAllKabeShinList(doc);
            List<LocationCurve> lcList = new List<LocationCurve>();

            foreach (ElementId id in idList)
            {
                FamilyInstance instance = doc.GetElement(id) as FamilyInstance;
                LocationCurve lCurve = instance.Location as LocationCurve;
                lcList.Add(lCurve);
            }
            res = ClsRevitUtil.GetCurveStartEndPoints(lcList);

            return res;
        }

        /// <summary>
        /// 対象のファミリに配置順をカスタムデータとして持たせる
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id">ファミリ</param>
        /// <param name="order">0or1or-1(単独で作成されたもの)</param>
        /// <returns></returns>
        public static bool SetPutOrder(Document doc, ElementId id, int order)
        {
            return ClsRevitUtil.CustomDataSet(doc, id, PUTORDER, order);
        }
        /// <summary>
        /// 対象のファミリから配置順のカスタムデータを取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id">ファミリ</param>
        /// <returns>0or1or-1(単独で作成されたもの)</returns>
        public static int GetPutOrder(Document doc, ElementId id)
        {
            ClsRevitUtil.CustomDataGet(doc, id, PUTORDER, out int order);
            return order;
        }

        /// <summary>
        /// 対象のファミリに壁芯のIdをカスタムデータとして持たせる
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id">ファミリ</param>
        /// <param name="baseId">ベースのID</param>
        /// <returns></returns>
        public static bool SetKabeShinId(Document doc, ElementId id, ElementId kabeShinId)
        {
            if (kabeShinId == null)
                return false;
            int iKabeShinId = kabeShinId.IntegerValue;
            return ClsRevitUtil.CustomDataSet<int>(doc, id, familyName, iKabeShinId);
        }
        /// <summary>
        /// 対象のファミリから壁芯のIdのカスタムデータを取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id">ファミリ</param>
        /// <returns>ベースのID</returns>
        public static ElementId GetKabeShinId(Document doc, ElementId id)
        {
            ClsRevitUtil.CustomDataGet<int>(doc, id, familyName, out int kabeShinId);

            if (kabeShinId == 0)
                return null;

            ElementId e = new ElementId(kabeShinId);
            return e;
        }

        /// <summary>
        /// 対象の壁芯を元に作成された壁関係を全て取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="kabeShinId">対象の壁芯ID</param>
        /// <returns>壁芯IDを持つ壁</returns>
        public static List<ElementId> GetKabeIdList(Document doc, ElementId kabeShinId)
        {
            List<ElementId> kabeIdList = new List<ElementId>();

            List<ElementId> allKabeList = new List<ElementId>();
            allKabeList.AddRange(ClsKouyaita.GetAllKouyaitaList(doc).ToList());
            allKabeList.AddRange(ClsOyakui.GetAllOyakuiList(doc).ToList());
            allKabeList.AddRange(ClsSMW.GetAllSMWList(doc).ToList());
            allKabeList.AddRange(ClsRenzokukabe.GetAllRenzokuKabeList(doc).ToList());

            foreach(ElementId id in allKabeList)
            {
                ElementId customId = GetKabeShinId(doc, id);
                if (customId == null)
                    continue;

                if (customId == kabeShinId)
                    kabeIdList.Add(id);
            }

            return kabeIdList;
        }

        public static List<ElementId> GetKouyaitaKabeIdList(Document doc, ElementId kabeShinId = null)
        {
            List<ElementId> kabeIdList = new List<ElementId>();

            List<ElementId> allKabeList = new List<ElementId>();
            allKabeList.AddRange(ClsKouyaita.GetAllKouyaitaList(doc).ToList());

            if (kabeShinId != null)
            {
                foreach (ElementId id in allKabeList)
                {
                    ElementId customId = GetKabeShinId(doc, id);
                    if (customId == null)
                        continue;

                    if (customId == kabeShinId)
                        kabeIdList.Add(id);
                }
            }
            else
            {
                kabeIdList.AddRange(allKabeList);
            }

            return kabeIdList;
        }

        public static List<ElementId> GetOyakuiKabeIdList(Document doc, ElementId kabeShinId = null)
        {
            List<ElementId> kabeIdList = new List<ElementId>();

            List<ElementId> allKabeList = new List<ElementId>();;
            allKabeList.AddRange(ClsOyakui.GetAllOyakuiList(doc).ToList());


            if (kabeShinId != null)
            {
                foreach (ElementId id in allKabeList)
                {
                    ElementId customId = GetKabeShinId(doc, id);
                    if (customId == null)
                        continue;

                    if (customId == kabeShinId)
                        kabeIdList.Add(id);
                }
            }
            else
            {
                kabeIdList.AddRange(allKabeList);
            }
            

            return kabeIdList;
        }

        public static List<ElementId> GetSMWKabeIdList(Document doc, ElementId kabeShinId = null)
        {
            List<ElementId> kabeIdList = new List<ElementId>();

            List<ElementId> allKabeList = new List<ElementId>();
            allKabeList.AddRange(ClsSMW.GetAllSMWList(doc).ToList());

            if (kabeShinId != null)
            {
                foreach (ElementId id in allKabeList)
                {
                    ElementId customId = GetKabeShinId(doc, id);
                    if (customId == null)
                        continue;

                    if (customId == kabeShinId)
                        kabeIdList.Add(id);
                }
            }
            else
            {
                kabeIdList.AddRange(allKabeList);
            }

            return kabeIdList;
        }

        public static List<ElementId> GetRenzokuKabeIdList(Document doc, ElementId kabeShinId = null)
        {
            List<ElementId> kabeIdList = new List<ElementId>();

            List<ElementId> allKabeList = new List<ElementId>();
            allKabeList.AddRange(ClsRenzokukabe.GetAllRenzokuKabeList(doc).ToList());

            if (kabeShinId != null)
            {
                foreach (ElementId id in allKabeList)
                {
                    ElementId customId = GetKabeShinId(doc, id);
                    if (customId == null)
                        continue;

                    if (customId == kabeShinId)
                        kabeIdList.Add(id);
                }
            }
            else
            {
                kabeIdList.AddRange(allKabeList);
            }

            return kabeIdList;
        }

        public static List<List<string>> GetCASEPitch(Document doc)
        {
            List<List<string>> CASEPitchList = new List<List<string>>();

            List<ElementId> allKouyaitaList = ClsKouyaita.GetAllKouyaitaList(doc);
            List<ElementId> allOyakuiList = ClsOyakui.GetAllOyakuiList(doc);
            List<ElementId> allSMWList = ClsSMW.GetAllSMWList(doc);
            List<ElementId> allRenzokuKabeList = ClsRenzokukabe.GetAllRenzokuKabeList(doc);

            List<string> CASEList = new List<string>();
            foreach(ElementId id in allKouyaitaList)
            {
                FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                FamilySymbol sym = inst.Symbol;
                string CASE = ClsRevitUtil.GetTypeParameterString(sym, "CASE");
                ClsRevitUtil.CustomDataGet(doc, id, "ピッチ", out int pitch);
                string CASEKabe = ClsKouyaita.KOUYAITA + "CASE" + CASE;
                string CASEPitch = CASEKabe + "," + pitch.ToString();

                string original = string.Empty;
                for (int j = 0; j < CASEList.Count; j++)
                {
                    string[] CASESprit = CASEList[j].Split(',');
                    if (CASESprit[0] == CASEKabe)
                    {
                        original = CASEList[j];
                        int sumPitch = ClsCommonUtils.ChangeStrToInt(CASESprit[1]) + pitch;
                        CASEPitch = CASEKabe + "," + sumPitch.ToString();
                        break;
                    }
                }
                CASEList.Remove(original);
                CASEList.Add(CASEPitch);
            }
            CASEPitchList.Add(CASEList.ToList());

            CASEList.Clear();
            foreach (ElementId id in allOyakuiList)
            {
                FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                FamilySymbol sym = inst.Symbol;
                string CASE = ClsRevitUtil.GetTypeParameterString(sym, "CASE");
                ClsRevitUtil.CustomDataGet(doc, id, "ピッチ", out int pitch);
                string CASEKabe = ClsOyakui.oyakui + "CASE" + CASE;
                string CASEPitch = CASEKabe + "," + pitch.ToString();

                string original = string.Empty;
                for (int j = 0; j < CASEList.Count; j++)
                {
                    string[] CASESprit = CASEList[j].Split(',');
                    if (CASESprit[0] == CASEKabe)
                    {
                        original = CASEList[j];
                        int sumPitch = ClsCommonUtils.ChangeStrToInt(CASESprit[1]) + pitch;
                        CASEPitch = CASEKabe + "," + sumPitch.ToString();
                        break;
                    }
                }
                CASEList.Remove(original);
                CASEList.Add(CASEPitch);
            }
            CASEPitchList.Add(CASEList.ToList());

            CASEList.Clear();
            foreach (ElementId id in allSMWList)
            {
                FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                FamilySymbol sym = inst.Symbol;
                string CASE = ClsRevitUtil.GetTypeParameterString(sym, "CASE");
                ClsRevitUtil.CustomDataGet(doc, id, "ピッチ", out int pitch);
                string CASEKabe = ClsSMW.SMW + "CASE" + CASE;
                string CASEPitch = CASEKabe + "," + pitch.ToString();

                string original = string.Empty;
                for (int j = 0; j < CASEList.Count; j++)
                {
                    string[] CASESprit = CASEList[j].Split(',');
                    if (CASESprit[0] == CASEKabe)
                    {
                        original = CASEList[j];
                        int sumPitch = ClsCommonUtils.ChangeStrToInt(CASESprit[1]) + pitch;
                        CASEPitch = CASEKabe + "," + sumPitch.ToString();
                        break;
                    }
                }
                CASEList.Remove(original);
                CASEList.Add(CASEPitch);
            }
            CASEPitchList.Add(CASEList.ToList());

            CASEList.Clear();
            foreach (ElementId id in allRenzokuKabeList)
            {
                FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                FamilySymbol sym = inst.Symbol;
                string CASE = ClsRevitUtil.GetTypeParameterString(sym, "CASE");
                ClsRevitUtil.CustomDataGet(doc, id, "ピッチ", out int pitch);
                string CASEKabe = ClsRenzokukabe.RENZOKUKABE + "CASE" + CASE;
                string CASEPitch = CASEKabe + "," + pitch.ToString();

                string original = string.Empty;
                for (int j = 0; j < CASEList.Count; j++)
                {
                    string[] CASESprit = CASEList[j].Split(',');
                    if (CASESprit[0] == CASEKabe)
                    {
                        original = CASEList[j];
                        int sumPitch = ClsCommonUtils.ChangeStrToInt(CASESprit[1]) + pitch;
                        CASEPitch = CASEKabe + "," + sumPitch.ToString();
                        break;
                    }
                }
                CASEList.Remove(original);
                CASEList.Add(CASEPitch);
            }
            CASEPitchList.Add(CASEList.ToList());

            return CASEPitchList;
        }

        public static List<string> GetKabeTotalLength(Document doc)
        {
            List<string> KabeTotalList = new List<string>();
            List<ElementId> kabeShinList = GetAllKabeShinList(doc);

            double kouyaita = 0, oyakui = 0, smw = 0, renzokukabe = 0;


            foreach(ElementId id in kabeShinList)
            {
                string kabe = ClsRevitUtil.GetParameter(doc, id, "壁");
                FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                Curve cv = (inst.Location as LocationCurve).Curve;
                double length = ClsRevitUtil.CovertFromAPI(cv.Length);
                switch(kabe)
                {
                    case ClsKouyaita.KOUYAITA:
                        {
                            kouyaita += length;
                            break;
                        }
                    case ClsOyakui.oyakui:
                        {
                            oyakui += length;
                            break;
                        }
                    case ClsSMW.SMW:
                        {
                            smw += length;
                            break;
                        }
                    case ClsRenzokukabe.RENZOKUKABE:
                        {
                            renzokukabe += length;
                            break;
                        }
                }
            }
            if (kouyaita != 0)
                KabeTotalList.Add(ClsKouyaita.KOUYAITA + "," + kouyaita.ToString());
            if (oyakui != 0)
                KabeTotalList.Add(ClsOyakui.oyakui + "," + oyakui.ToString());
            if (smw != 0)
                KabeTotalList.Add(ClsSMW.SMW + "," + smw.ToString());
            if (renzokukabe != 0)
                KabeTotalList.Add(ClsRenzokukabe.RENZOKUKABE + "," + renzokukabe.ToString());

            return KabeTotalList;
        }
        public static bool GetCsvLineKabe(Document doc, out List<string> kabeList)
        {
            kabeList = new List<string>();
            List<KabeKuiData> kabeDataList = new List<KabeKuiData>();

            List<ElementId> allKabeList = new List<ElementId>();
            allKabeList.AddRange(ClsKouyaita.GetAllKouyaitaList(doc).ToList());
            allKabeList.AddRange(ClsOyakui.GetAllOyakuiList(doc).ToList());
            allKabeList.AddRange(ClsSMW.GetAllSMWList(doc).ToList());
            allKabeList.AddRange(ClsRenzokukabe.GetAllRenzokuKabeList(doc).ToList());
            try
            {
                foreach (ElementId id in allKabeList)
                {
                    KabeKuiData kabeData = new KabeKuiData();
                    FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                    FamilySymbol sym = inst.Symbol;

                    kabeData.Classification = ClsRevitUtil.GetTypeParameterString(sym, "大分類");
                    kabeData.Usage = ClsRevitUtil.GetTypeParameterString(sym, "中分類");
                    if (kabeData.Usage != ClsKouyaita.KOUYAITA)
                    {
                        kabeData.SteelType = ClsRevitUtil.GetTypeParameterString(sym, "名称");
                        kabeData.Size = Master.ClsHBeamCsv.GetSizePileFamilyPathInName(sym.FamilyName); //ClsRevitUtil.GetTypeParameterString(sym, "サイズ");
                    }
                    else
                    {
                        kabeData.SteelType = ClsRevitUtil.GetTypeParameterString(sym, "小分類");
                        kabeData.Size = Master.ClsKouyaitaCsv.GetSizePileFamilyPathInName(sym.FamilyName); //ClsRevitUtil.GetTypeParameterString(sym, "サイズ");
                    }
                    kabeData.CaseName = ClsRevitUtil.GetTypeParameterString(sym, "CASE") + "-" + ClsRevitUtil.GetTypeParameterString(sym, "枝番");

                    // すでに同じ CaseName と Usage の組み合わせが存在する場合、Quantity を加算する
                    var existingData = kabeDataList.FirstOrDefault(data => data.CaseName == kabeData.CaseName && data.Usage == kabeData.Usage);
                    if (existingData.CaseName != null)
                    {
                        int num = kabeDataList.IndexOf(existingData);
                        existingData.Quantity += 1;
                        kabeDataList[num] = existingData;
                        continue;
                    }

                    kabeData.Length = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(sym, "長さ"));
                    kabeData.Pitch = ClsCommonUtils.ChangeStrToDbl(ClsRevitUtil.GetTypeParameterString(sym, "ピッチ"));
                    kabeData.EmbedmentLength = 0;
                    string zanti = ClsRevitUtil.GetTypeParameterString(sym, "残置/引抜");
                    if (zanti.Contains("一部埋め殺し"))
                    {
                        string[] split = zanti.Split('(');
                        zanti = split[0];
                        string[] split1 = split[1].Split(')');
                        kabeData.EmbedmentLength = ClsCommonUtils.ChangeStrToDbl(split1[0]);
                    }
                    kabeData.JointCount = ClsRevitUtil.GetTypeParameterInt(sym, "ジョイント数");
                    kabeData.Quantity = 1;
                    kabeData.UnitWeight = ClsRevitUtil.GetTypeParameterAsValueString(sym, "単位質量");
                    kabeData.SoilDiameter = 0;
                    if (sym.Name.Contains("SMW"))
                    {
                        ClsSMW clsSMW = new ClsSMW();
                        clsSMW.SetParameter(doc, id);
                        kabeData.SoilDiameter = clsSMW.m_dia;
                    }
                    kabeData.WallLength = 0;
                    kabeData.WallExtension = 0;

                    // 継手情報
                    kabeData.JointSpecification = ClsYMSUtil.GetKotei(doc, id);
                    
                    if (kabeData.Usage != ClsKouyaita.KOUYAITA)
                    {
                        YMS.Master.ClsPileTsugiteCsv pileCsv = YMS.Master.ClsPileTsugiteCsv.GetCls(kabeData.JointSpecification, kabeData.Size);
                        kabeData.Plate1 = pileCsv.PlateSizeFOut;
                        kabeData.Plate1Count = pileCsv.PlateNumFOut;
                        kabeData.Plate2 = pileCsv.PlateSizeFIn;
                        kabeData.Plate2Count = pileCsv.PlateNumFIn;
                        (string boltTypeF, int boltCountF) = ClsYMSUtil.GetBoltFlange(doc, id);
                        kabeData.BoltTypeF = boltTypeF;
                        kabeData.BoltCountF = boltCountF;
                        kabeData.PlateW = pileCsv.PlateSizeW;
                        kabeData.PlateCountW = pileCsv.PlateNumW;
                        (string boltTypeW, int boltCountW) = ClsYMSUtil.GetBoltWeb(doc, id);
                        kabeData.BoltTypeW = boltTypeW;
                        kabeData.BoltCountW = boltCountW;
                        kabeData.BoltDiameter = 0.0;
                    }
                    else
                    {
                        Master.ClsKouyaitaTsugiteCsv pileCsv = Master.ClsKouyaitaTsugiteCsv.GetCls(kabeData.JointSpecification, kabeData.Size);
                        kabeData.Plate1 = pileCsv.PlateSizeF;
                        kabeData.Plate1Count = pileCsv.PlateNumF;
                        kabeData.Plate2 = pileCsv.PlateSizeW;
                        kabeData.Plate2Count = pileCsv.PlateNumW;
                        (string boltTypeF, int boltCountF) = ClsYMSUtil.GetBoltFlange(doc, id);
                        kabeData.BoltTypeF = boltTypeF;
                        kabeData.BoltCountF = boltCountF;
                        kabeData.PlateW = pileCsv.PlateSizeW2;
                        kabeData.PlateCountW = pileCsv.PlateNumW2;
                        (string boltTypeW, int boltCountW) = ClsYMSUtil.GetBoltWeb(doc, id);
                        kabeData.BoltTypeW = boltTypeW;
                        kabeData.BoltCountW = boltCountW;
                        kabeData.BoltDiameter = 0.0;
                    }
                    
                    kabeData.Remarks = "";

                    kabeDataList.Add(kabeData);
                }

                // カンマ区切りの1行に変換してリストに追加
                foreach (var data in kabeDataList)
                {
                    string csvLine = ConvertDataToCsvLine(data);
                    kabeList.Add(csvLine);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public static bool GetCsvLineKui(Document doc, out List<string> kuiList)
        {
            kuiList = new List<string>();
            List<ElementId> tanakuiIds = new List<ElementId>();
            tanakuiIds.AddRange(ClsTanakui.GetAllKuiList(doc).ToList());
            tanakuiIds.AddRange(ClsSanbashiKui.GetAllKuiList(doc).ToList());

            try
            {
                // 新しいデータを格納するためのリスト
                List<KabeKuiData> tanakuiDataList = new List<KabeKuiData>();

                foreach (var tanakuiId in tanakuiIds)
                {
                    FamilyInstance instTanakui = doc.GetElement(tanakuiId) as FamilyInstance;

                    KabeKuiData tanakuiData = new KabeKuiData();
                    tanakuiData.Classification = System.Text.RegularExpressions.Regex.Replace(instTanakui.Symbol.Name, @"_\d+$", "");
                    tanakuiData.Usage = System.Text.RegularExpressions.Regex.Replace(instTanakui.Symbol.Name, @"_\d+$", "");
                    tanakuiData.SteelType = ClsRevitUtil.GetTypeParameterString(instTanakui.Symbol, "名称");
                    tanakuiData.Size = Master.ClsHBeamCsv.GetSizePileFamilyPathInName(instTanakui.Symbol.FamilyName); //ClsRevitUtil.GetTypeParameterString(instTanakui.Symbol, "サイズ");
                    tanakuiData.CaseName = instTanakui.Symbol.Name;
                    tanakuiData.Length = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(instTanakui.Symbol, "長さ"));
                    tanakuiData.Pitch = ClsRevitUtil.GetTypeParameter(instTanakui.Symbol, "ピッチ");
                    tanakuiData.EmbedmentLength = 0;
                    tanakuiData.JointCount = ClsRevitUtil.GetTypeParameterInt(instTanakui.Symbol, "ジョイント数");
                    tanakuiData.Quantity = 1;
                    tanakuiData.UnitWeight = ClsRevitUtil.GetTypeParameterAsValueString(instTanakui.Symbol, "単位質量");
                    tanakuiData.SoilDiameter = 0;
                    tanakuiData.WallLength = 0;
                    tanakuiData.WallExtension = 0;

                    // 継手情報
                    tanakuiData.JointSpecification = ClsYMSUtil.GetKotei(doc, instTanakui.Id);
                    YMS.Master.ClsPileTsugiteCsv pileCsv = YMS.Master.ClsPileTsugiteCsv.GetCls(tanakuiData.JointSpecification, tanakuiData.Size);
                    tanakuiData.Plate1 = pileCsv.PlateSizeFOut;
                    tanakuiData.Plate1Count = pileCsv.PlateNumFOut;
                    tanakuiData.Plate2 = pileCsv.PlateSizeFIn;
                    tanakuiData.Plate2Count = pileCsv.PlateNumFIn;
                    (string boltTypeF, int boltCountF) = ClsYMSUtil.GetBoltFlange(doc, instTanakui.Id);
                    tanakuiData.BoltTypeF = boltTypeF;
                    tanakuiData.BoltCountF = boltCountF;
                    tanakuiData.PlateW = pileCsv.PlateSizeW;
                    tanakuiData.PlateCountW = pileCsv.PlateNumW;
                    (string boltTypeW, int boltCountW) = ClsYMSUtil.GetBoltWeb(doc, instTanakui.Id);
                    tanakuiData.BoltTypeW = boltTypeW;
                    tanakuiData.BoltCountW = boltCountW;
                    tanakuiData.BoltDiameter = 0.0;
                    tanakuiData.Remarks = "";

                    

                    // すでに同じ CaseName と Size の組み合わせが存在する場合、Quantity を加算する
                    var existingData = tanakuiDataList.FirstOrDefault(data => data.CaseName == tanakuiData.CaseName && data.Size == tanakuiData.Size);
                    if (existingData.CaseName != null)
                    {
                        int num = tanakuiDataList.IndexOf(existingData);
                        existingData.Quantity += 1;
                        tanakuiDataList[num] = existingData;
                        if (tanakuiData.SteelType.Contains("断面変化"))
                        {
                            existingData.Remarks = "下段";
                            tanakuiDataList[num + 1] = existingData;
                        }
                    }
                    else
                    {
                        if (tanakuiData.SteelType.Contains("断面変化"))
                        {
                            tanakuiData.Remarks = "上段";
                            tanakuiDataList.Add(tanakuiData);
                            tanakuiData.Remarks = "下段";
                        }
                        tanakuiDataList.Add(tanakuiData);

                    }

                }

                // カンマ区切りの1行に変換してリストに追加
                for (int i = 0; i < tanakuiDataList.Count(); i++)
                {
                    var data = tanakuiDataList[i];
                    data.CaseName = "";
                    string csvLine = ConvertDataToCsvLine(data);
                    kuiList.Add(csvLine);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }


        private static string ConvertDataToCsvLine(KabeKuiData kabeKuiData)
        {
            StringBuilder csvLineBuilder = new StringBuilder();

            // 構造体の各フィールドをカンマ区切りで文字列に追加
            csvLineBuilder.Append($"{kabeKuiData.Classification},");
            csvLineBuilder.Append($"{kabeKuiData.Usage},");
            csvLineBuilder.Append($"{kabeKuiData.SteelType},");
            csvLineBuilder.Append($"{kabeKuiData.Size},");
            csvLineBuilder.Append($"{kabeKuiData.CaseName},");
            csvLineBuilder.Append($"{kabeKuiData.Length},");
            csvLineBuilder.Append($"{kabeKuiData.Pitch},");
            csvLineBuilder.Append($"{kabeKuiData.EmbedmentLength},");
            csvLineBuilder.Append($"{kabeKuiData.JointCount},");
            csvLineBuilder.Append($"{kabeKuiData.Quantity},");
            csvLineBuilder.Append($"{kabeKuiData.UnitWeight},");
            csvLineBuilder.Append($"{kabeKuiData.SoilDiameter},");
            csvLineBuilder.Append($"{kabeKuiData.WallLength},");
            csvLineBuilder.Append($"{kabeKuiData.WallExtension},");
            csvLineBuilder.Append($"{kabeKuiData.JointSpecification},");
            csvLineBuilder.Append($"{kabeKuiData.Plate1},");
            csvLineBuilder.Append($"{kabeKuiData.Plate1Count},");
            csvLineBuilder.Append($"{kabeKuiData.Plate2},");
            csvLineBuilder.Append($"{kabeKuiData.Plate2Count},");
            csvLineBuilder.Append($"{kabeKuiData.BoltTypeF},");
            csvLineBuilder.Append($"{kabeKuiData.BoltCountF},");
            csvLineBuilder.Append($"{kabeKuiData.PlateW},");
            csvLineBuilder.Append($"{kabeKuiData.PlateCountW},");
            csvLineBuilder.Append($"{kabeKuiData.BoltTypeW},");
            csvLineBuilder.Append($"{kabeKuiData.BoltCountW},");
            csvLineBuilder.Append($"{kabeKuiData.BoltDiameter},");
            csvLineBuilder.Append($"{kabeKuiData.Remarks}");

            return csvLineBuilder.ToString();
        }

    }

    public struct KabeKuiData
    {
        public string Classification { get; set; }     // 分類
        public string Usage { get; set; }              // 用途
        public string SteelType { get; set; }          // 鋼材タイプ
        public string Size { get; set; }               // サイズ
        public string CaseName { get; set; }           // CASE名
        public double Length { get; set; }             // 長さ
        public double Pitch { get; set; }              // ピッチ
        public double EmbedmentLength { get; set; }    // 埋め殺し長さ
        public int JointCount { get; set; }            // ジョイント数
        public int Quantity { get; set; }              // 数量
        public string UnitWeight { get; set; }         // 単位質量
        public double SoilDiameter { get; set; }       // ソイル径
        public double WallLength { get; set; }         // 壁長
        public double WallExtension { get; set; }      // 壁延長
        public string JointSpecification { get; set; } // 継手仕様
        public string Plate1 { get; set; }             // プレート1(F)
        public int Plate1Count { get; set; }           // 枚数１
        public string Plate2 { get; set; }             // プレート2(F)
        public int Plate2Count { get; set; }           // 枚数２
        public string BoltTypeF { get; set; }          // ボルトタイプ(F)
        public int BoltCountF { get; set; }            // ボルト本数(F)
        public string PlateW { get; set; }             // プレート(W)
        public int PlateCountW { get; set; }           // 枚数
        public string BoltTypeW { get; set; }          // ボルトタイプ(W)
        public int BoltCountW { get; set; }            // ボルト本数(W)
        public double BoltDiameter { get; set; }       // ボルト孔径
        public string Remarks { get; set; }            // 備考欄
    }
}
