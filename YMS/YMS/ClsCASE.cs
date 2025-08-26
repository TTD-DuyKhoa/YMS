using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YMS.Master;
using YMS.Parts;


namespace YMS
{
    public class ClsCASE
    {
        /// <summary>
        /// 最大ジョイント数
        /// </summary>
        public const int PileJointMax = 10;

        public List<stCASEKoyaita> m_koyaitaDataList { get; set; }
        public List<stCASEOyagui> m_oyaguiDataList { get; set; }
        public List<stCASESMW> m_smwDataList { get; set; }
        public List<stCASERenzokuKabe> m_kabeDataList { get; set; }

        public ClsCASE(enKabeshu kabeshu, Document doc = null)
        {
            m_koyaitaDataList = new List<stCASEKoyaita>();
            m_oyaguiDataList = new List<stCASEOyagui>();
            m_smwDataList = new List<stCASESMW>();
            m_kabeDataList = new List<stCASERenzokuKabe>();
            if (doc != null)
            {
                GetCASE_Data(doc, kabeshu);
            }
        }

        public enum enKabeshu
        {
            koyauita,
            oyagui,
            smw,
            renzokukabe,
            all
        }

        public enum enCASEHit
        {
            sameKabeshu,//同じ壁内に同じCASEが存在する
            otherKabeshu,//異なる壁種に同じCASEが存在する
            noHit//同じCASEは存在しない
        }

        public enum enCASE_EdabanHit
        {
            sameKabeshu,//同じ壁内に同じCASE-枝番が存在する
            otherKabeshu,//異なる壁種に同じCASE-枝番が存在する
            noHit//同じCASE-枝番は存在しない
        }

        //public enum enEdabanHit
        //{
        //    sameKabeshu,//同じ壁内に同じ枝番が存在する
        //    otherKabeshu,//異なる壁種に同じ枝番が存在する
        //    noHit//同じ枝番は存在しない
        //}

        public enum CASECommand : int
        {
            HighLight,
            Change,
            Wariate,
            Close
        }

        /// <summary>
        /// 鋼矢板のCASE情報
        /// </summary>
        public class stCASEKoyaita
        {
            #region プロパティ
            /// <summary>
            /// CASE番号
            /// </summary>
            public string CASE { get; set; }//ex. CASE1
            /// <summary>
            /// 変更後のCASE
            /// </summary>
            public string NewCASE { get; set; }//ex. CASE1
            /// <summary>
            /// 部材タイプ
            /// </summary>
            public string buzaiType { get; set; }//ex. 鋼矢板
            /// <summary>
            /// 部材サイズ
            /// </summary>
            public string buzaiSize { get; set; }//ex. SP-3
            /// <summary>
            /// 全長
            /// </summary>
            public double TotalLength { get; set; }//ex. 10000
            /// <summary>
            /// 材質
            /// </summary>
            public string zaishitsu { get; set; }//ex. SYM295
            /// <summary>
            /// 残置
            /// </summary>
            public string zanchi { get; set; }//ex.　一部埋め殺し
            /// <summary>
            /// 残置長さ
            /// </summary>
            public string zanchiLength { get; set; }// ex. 6000　（全埋め殺しの時は0でよいか）

            /// 枝番
            /// </summary>
            public string Edaban { get; set; }//B
            /// <summary>
            /// 変更後の枝番
            /// </summary>
            public string NewEdaban { get; set; }//B
            /// <summary>
            /// ジョイント数
            /// </summary>
            public int JointNum { get; set; }//ex. 2
            /// <summary>
            /// 杭の分割長さのリスト
            /// </summary>
            public List<int> KuiLengthList { get; set; }//ex. 5000　3000　2000
            /// <summary>
            /// 図面上に存在するケースと枝番が一致する数
            /// </summary>
            public int count { get; set; }//20本
            /// <summary>
            /// 対象のElementId
            /// </summary>
            public ElementId ElementId { get; set; }
            /// <summary>
            /// 対象の壁のElementIdList
            /// </summary>
            public List<ElementId> idList { get; set; }
            #endregion
            public stCASEKoyaita(Document doc, ElementId id)
            {
                ClsKouyaita clsKouyaita = new ClsKouyaita();
                clsKouyaita.SetParameter(doc, id);
                CASE = clsKouyaita.m_case.ToString();
                NewCASE = clsKouyaita.m_case.ToString();
                buzaiType = clsKouyaita.m_type;
                buzaiSize = clsKouyaita.m_size;
                TotalLength = clsKouyaita.m_HLen;
                zaishitsu = clsKouyaita.m_zaishitu;
                zanchi = clsKouyaita.m_zanti;
                zanchiLength = clsKouyaita.m_zantiLength;

                Edaban = clsKouyaita.m_edaNum;
                NewEdaban = clsKouyaita.m_edaNum;
                JointNum = clsKouyaita.m_Kasho1;
                KuiLengthList = clsKouyaita.m_ListPileLength1;
                count = 0;
                ElementId = id;
                idList = new List<ElementId>();
            }
            public stCASEKoyaita()
            {
                KuiLengthList = new List<int>();
                idList = new List<ElementId>();
            }

            public static List<stCASEKoyaita> OverLapCASE(List<stCASEKoyaita> stCASEKoyaitaList, bool list = false)
            {
                List<stCASEKoyaita> dataList = new List<stCASEKoyaita>();

                for (int i = 0; i < stCASEKoyaitaList.Count; i++)
                {
                    bool bFlag = false;
                    for (int j = 0; j < dataList.Count; j++)
                    {
                        if (dataList[j].CASE == stCASEKoyaitaList[i].CASE && dataList[j].Edaban == stCASEKoyaitaList[i].Edaban)
                        {
                            bFlag = true;
                            break;
                        }
                    }
                    if (bFlag)
                        continue;
                    dataList.Add(stCASEKoyaitaList[i]);

                    int count = 0;
                    List<ElementId> idList = new List<ElementId>();
                    for (int j = 0; j < stCASEKoyaitaList.Count; j++)
                    {
                        if (stCASEKoyaitaList[i].CASE == stCASEKoyaitaList[j].CASE && stCASEKoyaitaList[i].Edaban == stCASEKoyaitaList[j].Edaban)
                        {
                            idList.Add(stCASEKoyaitaList[j].ElementId);
                            count++;
                        }
                    }

                    dataList[dataList.Count - 1].count = count;
                    if (list)
                        dataList[dataList.Count - 1].idList = idList.ToList();
                }


                return dataList;
            }

            public static void SetCASE(Document doc, List<stCASEKoyaita> stCASEKoyaitaList)
            {
                List<ElementId> allKouyaitaList = ClsKouyaita.GetAllKouyaitaList(doc);
                foreach (stCASEKoyaita stCASEKoyaita in stCASEKoyaitaList)
                {
                    foreach (ElementId id in allKouyaitaList)
                    {
                        ClsKouyaita clsKouyaita = new ClsKouyaita();
                        clsKouyaita.SetParameter(doc, id);
                        if (stCASEKoyaita.CASE == clsKouyaita.m_case.ToString() && stCASEKoyaita.Edaban == clsKouyaita.m_edaNum)
                        {
                            FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                            FamilySymbol sym = inst.Symbol;
                            string[] name = inst.Name.Split('_');
                            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                            {
                                t.Start();
                                FailureHandlingOptions failOpt = t.GetFailureHandlingOptions();
                                failOpt.SetFailuresPreprocessor(new WarningSwallower());
                                t.SetFailureHandlingOptions(failOpt);

                                sym = ClsRevitUtil.ChangeTypeID(doc, sym, id, name + "_" + stCASEKoyaita.NewCASE + "_" + stCASEKoyaita.NewEdaban);

                                ClsRevitUtil.SetTypeParameter(sym, "CASE", stCASEKoyaita.NewCASE);
                                ClsRevitUtil.SetTypeParameter(sym, "長さ", ClsRevitUtil.CovertToAPI(stCASEKoyaita.TotalLength));
                                ClsRevitUtil.SetTypeParameter(sym, "材質", stCASEKoyaita.zaishitsu);
                                if (stCASEKoyaita.zanchi == "一部埋め殺し")
                                    ClsRevitUtil.SetTypeParameter(sym, "残置/引抜", stCASEKoyaita.zanchi + "(" + stCASEKoyaita.zanchiLength + ")");
                                else
                                    ClsRevitUtil.SetTypeParameter(sym, "残置/引抜", stCASEKoyaita.zanchi);

                                ClsRevitUtil.SetTypeParameter(sym, "枝番", stCASEKoyaita.NewEdaban);
                                ClsRevitUtil.SetTypeParameter(sym, "ジョイント数", stCASEKoyaita.JointNum);

                                for (int i = 0; i < PileJointMax; i++)
                                {
                                    int nPileLength = 0;
                                    if (i < stCASEKoyaita.KuiLengthList.Count)
                                    {
                                        nPileLength = stCASEKoyaita.KuiLengthList[i];
                                    }
                                    ClsRevitUtil.SetTypeParameter(sym, "杭" + (i + 1).ToString(), ClsRevitUtil.CovertToAPI(nPileLength));
                                }

                            }
                        }
                    }
                }
            }
            public static void CreateCASE(Document doc, List<stCASEKoyaita> stCASEKoyaitaList, List<ElementId> allKouyaitaList)
            {
                List<ElementId> deleteList = new List<ElementId>();

                foreach (stCASEKoyaita stCASEKoyaita in stCASEKoyaitaList)
                {
                    foreach (ElementId id in allKouyaitaList)
                    {
                        ClsKouyaita clsKouyaita = new ClsKouyaita();
                        clsKouyaita.SetParameter(doc, id);
                        if (stCASEKoyaita.CASE == clsKouyaita.m_case.ToString() && stCASEKoyaita.Edaban == clsKouyaita.m_edaNum)
                        {
                            clsKouyaita.m_case = ClsCommonUtils.ChangeStrToInt(stCASEKoyaita.NewCASE);
                            clsKouyaita.m_type = stCASEKoyaita.buzaiType;
                            clsKouyaita.m_size = stCASEKoyaita.buzaiSize;
                            clsKouyaita.m_kouyaitaSize = "なし";
                            clsKouyaita.m_HLen = (int)stCASEKoyaita.TotalLength;
                            clsKouyaita.m_zaishitu = stCASEKoyaita.zaishitsu;
                            clsKouyaita.m_zanti = stCASEKoyaita.zanchi;
                            clsKouyaita.m_zantiLength = stCASEKoyaita.zanchiLength;

                            clsKouyaita.m_edaNum = stCASEKoyaita.NewEdaban;
                            clsKouyaita.m_Kasho1 = stCASEKoyaita.JointNum;
                            clsKouyaita.m_ListPileLength1 = stCASEKoyaita.KuiLengthList;

                            FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                            XYZ dir = inst.HandOrientation;
                            XYZ insertPoint = (inst.Location as LocationPoint).Point;
                            ElementId levelId = ClsRevitUtil.GetParameterElementId(doc, id, "集計レベル");
                            VoidVec voidVec = ClsKouyaita.GetVoidvec(doc, id) == 1 ? VoidVec.Kussaku : VoidVec.Kabe;
                            //鋼矢板を作成する
                            clsKouyaita.CreateSingleKouyaita(doc, insertPoint, dir, levelId, voidVec);

                            deleteList.Add(id);
                        }
                    }
                }
                using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                {
                    t.Start();
                    ClsRevitUtil.Delete(doc, deleteList);
                    t.Commit();
                }
            }
            public static void CreateCASE(Document doc, stCASEKoyaita stCASEKoyaita, List<ElementId> allKouyaitaList)
            {
                List<ElementId> deleteList = new List<ElementId>();

                foreach (ElementId id in allKouyaitaList)
                {
                    ClsKouyaita clsKouyaita = new ClsKouyaita();
                    clsKouyaita.SetParameter(doc, id);
                    if (stCASEKoyaita.CASE == clsKouyaita.m_case.ToString() && stCASEKoyaita.Edaban == clsKouyaita.m_edaNum)
                    {
                        clsKouyaita.m_case = ClsCommonUtils.ChangeStrToInt(stCASEKoyaita.NewCASE);
                        clsKouyaita.m_type = stCASEKoyaita.buzaiType;
                        clsKouyaita.m_size = stCASEKoyaita.buzaiSize;
                        clsKouyaita.m_kouyaitaSize = "なし";
                        clsKouyaita.m_HLen = (int)stCASEKoyaita.TotalLength;
                        clsKouyaita.m_zaishitu = stCASEKoyaita.zaishitsu;
                        clsKouyaita.m_zanti = stCASEKoyaita.zanchi;
                        clsKouyaita.m_zantiLength = stCASEKoyaita.zanchiLength;

                        clsKouyaita.m_edaNum = stCASEKoyaita.NewEdaban;
                        clsKouyaita.m_Kasho1 = stCASEKoyaita.JointNum;
                        clsKouyaita.m_ListPileLength1 = stCASEKoyaita.KuiLengthList;

                        FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                        XYZ dir = inst.HandOrientation;
                        XYZ insertPoint = (inst.Location as LocationPoint).Point;
                        ElementId levelId = ClsRevitUtil.GetParameterElementId(doc, id, "集計レベル");
                        VoidVec voidVec = ClsKouyaita.GetVoidvec(doc, id) == 1 ? VoidVec.Kussaku : VoidVec.Kabe;
                        //鋼矢板を作成する
                        clsKouyaita.CreateSingleKouyaita(doc, insertPoint, dir, levelId, voidVec);

                        deleteList.Add(id);
                    }
                }

                using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                {
                    t.Start();
                    ClsRevitUtil.Delete(doc, deleteList);
                    t.Commit();
                }
            }
        }

        /// <summary>
        /// 親杭のCASE情報
        /// </summary>
        public class stCASEOyagui
        {
            #region プロパティ
            /// <summary>
            /// CASE番号
            /// </summary>
            public string CASE { get; set; }//ex. CASE1
            /// <summary>
            /// 変更後のCASE
            /// </summary>
            public string NewCASE { get; set; }//ex. CASE1
            /// <summary>
            /// 部材タイプ
            /// </summary>
            public string buzaiType { get; set; }//ex. 細幅
            /// <summary>
            /// 部材サイズ
            /// </summary>
            public string buzaiSize { get; set; }//ex. H-300x150x6.5x9
            /// <summary>
            /// 全長
            /// </summary>
            public double TotalLength { get; set; }//ex. 10000
            /// <summary>
            /// 材質
            /// </summary>
            public string zaishitsu { get; set; }//ex. SYM295
            /// <summary>
            /// 残置
            /// </summary>
            public string zanchi { get; set; }//ex.　一部埋め殺し
            /// <summary>
            /// 残置長さ
            /// </summary>
            public string zanchiLength { get; set; }// ex. 6000　（全埋め殺しの時は0でよいか）
            /// <summary>
            /// 枝番
            /// </summary>
            public string Edaban { get; set; }//B
            /// <summary>
            /// 変更後の枝番
            /// </summary>
            public string NewEdaban { get; set; }//B
            /// <summary>
            /// ジョイント数
            /// </summary>
            public int JointNum { get; set; }//ex. 2
            /// <summary>
            /// 杭の分割長さのリスト
            /// </summary>
            public List<int> KuiLengthList { get; set; }//ex. 5000　3000　2000
            /// <summary>
            /// 図面上に存在するケースと枝番が一致する数
            /// </summary>
            public int count { get; set; }//20本
            /// <summary>
            /// 対象のElementId
            /// </summary>
            public ElementId ElementId { get; set; }
            /// <summary>
            /// 対象の壁のElementIdList
            /// </summary>
            public List<ElementId> idList { get; set; }
            #endregion
            public stCASEOyagui(Document doc, ElementId id)
            {
                ClsOyakui clsOyakui = new ClsOyakui();
                clsOyakui.SetParameter(doc, id);
                CASE = clsOyakui.m_case.ToString();
                NewCASE = clsOyakui.m_case.ToString();
                buzaiType = clsOyakui.m_type;
                buzaiSize = clsOyakui.m_size;
                TotalLength = clsOyakui.m_HLen;
                zaishitsu = clsOyakui.m_zaishitu;
                zanchi = clsOyakui.m_zanti;
                zanchiLength = clsOyakui.m_zantiLength;

                Edaban = clsOyakui.m_edaNum;
                NewEdaban = clsOyakui.m_edaNum;
                JointNum = clsOyakui.m_Kasho1;
                KuiLengthList = clsOyakui.m_ListPileLength1;
                count = 0;
                ElementId = id;
                idList = new List<ElementId>();
            }

            public stCASEOyagui()
            {
                KuiLengthList = new List<int>();
                idList = new List<ElementId>();
            }

            public static List<stCASEOyagui> OverLapCASE(List<stCASEOyagui> stCASEOyaguiList, bool list = false)
            {
                List<stCASEOyagui> dataList = new List<stCASEOyagui>();

                for (int i = 0; i < stCASEOyaguiList.Count; i++)
                {
                    bool bFlag = false;
                    for (int j = 0; j < dataList.Count; j++)
                    {
                        if (dataList[j].CASE == stCASEOyaguiList[i].CASE && dataList[j].Edaban == stCASEOyaguiList[i].Edaban)
                        {
                            bFlag = true;
                            break;
                        }
                    }
                    if (bFlag)
                        continue;
                    dataList.Add(stCASEOyaguiList[i]);

                    int count = 0;
                    List<ElementId> idList = new List<ElementId>();
                    for (int j = 0; j < stCASEOyaguiList.Count; j++)
                    {
                        if (stCASEOyaguiList[i].CASE == stCASEOyaguiList[j].CASE && stCASEOyaguiList[i].Edaban == stCASEOyaguiList[j].Edaban)
                        {
                            idList.Add(stCASEOyaguiList[j].ElementId);
                            count++;
                        }
                    }

                    dataList[dataList.Count - 1].count = count;
                    if (list)
                        dataList[dataList.Count - 1].idList = idList.ToList();
                }
                return dataList;
            }

            public static void CreateCASE(Document doc, List<stCASEOyagui> stCASEOyaguiList, List<ElementId> allOyaguiList)
            {
                List<ElementId> deleteList = new List<ElementId>();

                foreach (stCASEOyagui stCASEOyagui in stCASEOyaguiList)
                {
                    foreach (ElementId id in allOyaguiList)
                    {
                        ClsOyakui clsOyakui = new ClsOyakui();
                        clsOyakui.SetParameter(doc, id);
                        if (stCASEOyagui.CASE == clsOyakui.m_case.ToString() && stCASEOyagui.Edaban == clsOyakui.m_edaNum)
                        {
                            clsOyakui.m_case = ClsCommonUtils.ChangeStrToInt(stCASEOyagui.NewCASE);
                            clsOyakui.m_type = stCASEOyagui.buzaiType;
                            clsOyakui.m_size = stCASEOyagui.buzaiSize;

                            clsOyakui.m_HLen = (int)stCASEOyagui.TotalLength;
                            clsOyakui.m_zaishitu = stCASEOyagui.zaishitsu;
                            clsOyakui.m_zanti = stCASEOyagui.zanchi;
                            clsOyakui.m_zantiLength = stCASEOyagui.zanchiLength;

                            clsOyakui.m_edaNum = stCASEOyagui.NewEdaban;
                            clsOyakui.m_Kasho1 = stCASEOyagui.JointNum;
                            clsOyakui.m_ListPileLength1 = stCASEOyagui.KuiLengthList;

                            clsOyakui.m_putPosFlag = 1;//既存の配置位置に置くため

                            FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                            XYZ dir = inst.HandOrientation;
                            XYZ insertPoint = (inst.Location as LocationPoint).Point;
                            ElementId levelId = ClsRevitUtil.GetParameterElementId(doc, id, "集計レベル");
                            //杭に紐づいている壁芯IDを取得
                            ElementId kabeShinId = ClsKabeShin.GetKabeShinId(doc, id);
                            //親杭を作成する
                            clsOyakui.CreateSingleOyakui(doc, insertPoint, dir, levelId, kabeShinId: kabeShinId);

                            deleteList.Add(id);
                        }
                    }
                }
                using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                {
                    t.Start();
                    ClsRevitUtil.Delete(doc, deleteList);
                    t.Commit();
                }
            }
            public static void CreateCASE(Document doc, stCASEOyagui stCASEOyagui, List<ElementId> allOyaguiList)
            {
                List<ElementId> deleteList = new List<ElementId>();

                foreach (ElementId id in allOyaguiList)
                {
                    ClsOyakui clsOyakui = new ClsOyakui();
                    clsOyakui.SetParameter(doc, id);
                    if (stCASEOyagui.CASE == clsOyakui.m_case.ToString() && stCASEOyagui.Edaban == clsOyakui.m_edaNum)
                    {
                        clsOyakui.m_case = ClsCommonUtils.ChangeStrToInt(stCASEOyagui.NewCASE);
                        clsOyakui.m_type = stCASEOyagui.buzaiType;
                        clsOyakui.m_size = stCASEOyagui.buzaiSize;

                        clsOyakui.m_HLen = (int)stCASEOyagui.TotalLength;
                        clsOyakui.m_zaishitu = stCASEOyagui.zaishitsu;
                        clsOyakui.m_zanti = stCASEOyagui.zanchi;
                        clsOyakui.m_zantiLength = stCASEOyagui.zanchiLength;

                        clsOyakui.m_edaNum = stCASEOyagui.NewEdaban;
                        clsOyakui.m_Kasho1 = stCASEOyagui.JointNum;
                        clsOyakui.m_ListPileLength1 = stCASEOyagui.KuiLengthList;

                        clsOyakui.m_putPosFlag = 1;//既存の配置位置に置くため

                        FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                        XYZ dir = inst.HandOrientation;
                        XYZ insertPoint = (inst.Location as LocationPoint).Point;
                        ElementId levelId = ClsRevitUtil.GetParameterElementId(doc, id, "集計レベル");
                        //杭に紐づいている壁芯IDを取得
                        ElementId kabeShinId = ClsKabeShin.GetKabeShinId(doc, id);
                        //親杭を作成する
                        clsOyakui.CreateSingleOyakui(doc, insertPoint, dir, levelId, kabeShinId: kabeShinId);

                        deleteList.Add(id);
                    }
                }

                using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                {
                    t.Start();
                    ClsRevitUtil.Delete(doc, deleteList);
                    t.Commit();
                }
            }
        }

        /// <summary>
        /// SMWのCASE情報
        /// </summary>
        public class stCASESMW
        {
            #region プロパティ
            /// <summary>
            /// CASE番号
            /// </summary>
            public string CASE { get; set; }//ex. CASE1
            /// <summary>
            /// 変更後のCASE
            /// </summary>
            public string NewCASE { get; set; }//ex. CASE1
            /// <summary>
            /// 部材タイプ
            /// </summary>
            public string buzaiType { get; set; }//ex. 細幅
            /// <summary>
            /// 部材サイズ
            /// </summary>
            public string buzaiSize { get; set; }//ex. H-300x150x6.5x9
            /// <summary>
            /// 全長
            /// </summary>
            public double TotalLength { get; set; }//ex. 10000
            /// <summary>
            /// ソイル径
            /// </summary>
            public double soilKei { get; set; }//ex.550
            /// <summary>
            /// ソイル長
            /// </summary>
            public double soilLength { get; set; }//ex.13000
            /// <summary>
            /// 材質
            /// </summary>
            public string zaishitsu { get; set; }//ex. SYM295
            /// <summary>
            /// 残置
            /// </summary>
            public string zanchi { get; set; }//ex.　一部埋め殺し
            /// <summary>
            /// 残置長さ
            /// </summary>
            public string zanchiLength { get; set; }// ex. 6000　（全埋め殺しの時は0でよいか）
            /// <summary>
            /// 枝番
            /// </summary>
            public string Edaban { get; set; }//B
            /// <summary>
            /// 変更後の枝番
            /// </summary>
            public string NewEdaban { get; set; }//B
            /// <summary>
            /// ジョイント数
            /// </summary>
            public int JointNum { get; set; }//ex. 2
            /// <summary>
            /// 杭の分割長さのリスト
            /// </summary>
            public List<int> KuiLengthList { get; set; }//ex. 5000　3000　2000
            /// <summary>
            /// 図面上に存在するケースと枝番が一致する数
            /// </summary>
            public int count { get; set; }//20本
            /// <summary>
            /// 対象のElementId
            /// </summary>
            public ElementId ElementId { get; set; }
            /// <summary>
            /// 対象の壁のElementIdList
            /// </summary>
            public List<ElementId> idList { get; set; }
            #endregion
            public stCASESMW(Document doc, ElementId id)
            {
                ClsSMW clsSMW = new ClsSMW();
                clsSMW.SetParameter(doc, id);
                CASE = clsSMW.m_case.ToString();
                NewCASE = clsSMW.m_case.ToString();
                buzaiType = clsSMW.m_type;
                buzaiSize = clsSMW.m_size;
                TotalLength = clsSMW.m_HLen;
                soilKei = clsSMW.m_dia;
                soilLength = clsSMW.m_soilLen;
                zaishitsu = clsSMW.m_zaishitu;
                zanchi = clsSMW.m_zanti;
                zanchiLength = clsSMW.m_zantiLength;

                Edaban = clsSMW.m_edaNum;
                NewEdaban = clsSMW.m_edaNum;
                JointNum = clsSMW.m_Kasho1;
                KuiLengthList = clsSMW.m_ListPileLength1;
                count = 0;
                ElementId = id;
                idList = new List<ElementId>();
            }

            public stCASESMW()
            {
                KuiLengthList = new List<int>();
                idList = new List<ElementId>();
            }

            public static List<stCASESMW> OverLapCASE(List<stCASESMW> stCASESMWList, bool list = false)
            {
                List<stCASESMW> dataList = new List<stCASESMW>();

                for (int i = 0; i < stCASESMWList.Count; i++)
                {
                    bool bFlag = false;
                    for (int j = 0; j < dataList.Count; j++)
                    {
                        if (dataList[j].CASE == stCASESMWList[i].CASE && dataList[j].Edaban == stCASESMWList[i].Edaban)
                        {
                            bFlag = true;
                            break;
                        }
                    }
                    if (bFlag)
                        continue;
                    dataList.Add(stCASESMWList[i]);

                    int count = 0;
                    List<ElementId> idList = new List<ElementId>();
                    for (int j = 0; j < stCASESMWList.Count; j++)
                    {
                        if (stCASESMWList[i].CASE == stCASESMWList[j].CASE && stCASESMWList[i].Edaban == stCASESMWList[j].Edaban)
                        {
                            idList.Add(stCASESMWList[j].ElementId);
                            count++;
                        }
                    }

                    dataList[dataList.Count - 1].count = count;
                    if (list)
                        dataList[dataList.Count - 1].idList = idList.ToList();
                }
                return dataList;
            }
            public static void CreateCASE(Document doc, List<stCASESMW> stCASESMWList, List<ElementId> allSMWList)
            {
                //ソイル
                List<ElementId> soilList = ClsSMW.GetAllSoilList(doc);

                List<ElementId> deleteList = new List<ElementId>();

                foreach (stCASESMW stCASESMW in stCASESMWList)
                {
                    foreach (ElementId id in allSMWList)
                    {
                        ClsSMW clsSMW = new ClsSMW();
                        clsSMW.SetParameter(doc, id);
                        if (stCASESMW.CASE == clsSMW.m_case.ToString() && stCASESMW.Edaban == clsSMW.m_edaNum)
                        {
                            clsSMW.m_case = ClsCommonUtils.ChangeStrToInt(stCASESMW.NewCASE);
                            clsSMW.m_type = stCASESMW.buzaiType;
                            clsSMW.m_size = stCASESMW.buzaiSize;
                            clsSMW.m_HLen = (int)stCASESMW.TotalLength;
                            clsSMW.m_dia = (int)stCASESMW.soilKei;
                            clsSMW.m_soilLen = (int)stCASESMW.soilLength;
                            clsSMW.m_zaishitu = stCASESMW.zaishitsu;
                            clsSMW.m_zanti = stCASESMW.zanchi;
                            clsSMW.m_zantiLength = stCASESMW.zanchiLength;

                            clsSMW.m_edaNum = stCASESMW.NewEdaban;
                            clsSMW.m_Kasho1 = stCASESMW.JointNum;
                            clsSMW.m_ListPileLength1 = stCASESMW.KuiLengthList;

                            FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                            XYZ dir = inst.HandOrientation;
                            XYZ insertPoint = (inst.Location as LocationPoint).Point;
                            ElementId levelId = ClsRevitUtil.GetParameterElementId(doc, id, "集計レベル");
                            //杭に紐づいている壁芯IDを取得
                            ElementId kabeShinId = ClsKabeShin.GetKabeShinId(doc, id);
                            //SMWを作成する
                            clsSMW.CreateSingleSMW(doc, insertPoint, dir, levelId, true, false, kabeShinId: kabeShinId);

                            deleteList.Add(id);
                        }
                    }

                    foreach (ElementId soilId in soilList)
                    {
                        FamilyInstance soilInst = doc.GetElement(soilId) as FamilyInstance;
                        FamilySymbol soilSym = soilInst.Symbol;
                        string CASE = ClsRevitUtil.GetTypeParameterString(soilSym, "CASE");
                        string edaNum = ClsRevitUtil.GetTypeParameterString(soilSym, "枝番");
                        //ソイルのCASEと枝番号が一致するものからデータを取得する
                        if (stCASESMW.CASE == CASE && stCASESMW.Edaban == edaNum)
                        {
                            ClsSMW.ChangeSoil(doc, soilId, stCASESMW.NewCASE, stCASESMW.NewEdaban, (int)stCASESMW.soilLength, (int)stCASESMW.soilKei);
                        }
                        //else if(stCASESMW.CASE == CASE && string.IsNullOrWhiteSpace(edaNum))
                        //{
                        //    ClsSMW.ChangeSoil(doc, soilId, stCASESMW.NewCASE, edaNum, (int)stCASESMW.soilLength, (int)stCASESMW.soilKei);
                        //}
                    }
                }
                using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                {
                    t.Start();
                    ClsRevitUtil.Delete(doc, deleteList);
                    t.Commit();
                }
            }
            public static void CreateCASE(Document doc, stCASESMW stCASESMW, List<ElementId> allSMWList)
            {
                //ソイル
                List<ElementId> soilList = ClsSMW.GetAllSoilList(doc);

                List<ElementId> deleteList = new List<ElementId>();

                foreach (ElementId id in allSMWList)
                {
                    ClsSMW clsSMW = new ClsSMW();
                    clsSMW.SetParameter(doc, id);
                    if (stCASESMW.CASE == clsSMW.m_case.ToString() && stCASESMW.Edaban == clsSMW.m_edaNum)
                    {
                        clsSMW.m_case = ClsCommonUtils.ChangeStrToInt(stCASESMW.NewCASE);
                        clsSMW.m_type = stCASESMW.buzaiType;
                        clsSMW.m_size = stCASESMW.buzaiSize;
                        clsSMW.m_HLen = (int)stCASESMW.TotalLength;
                        clsSMW.m_dia = (int)stCASESMW.soilKei;
                        clsSMW.m_soilLen = (int)stCASESMW.soilLength;
                        clsSMW.m_zaishitu = stCASESMW.zaishitsu;
                        clsSMW.m_zanti = stCASESMW.zanchi;
                        clsSMW.m_zantiLength = stCASESMW.zanchiLength;

                        clsSMW.m_edaNum = stCASESMW.NewEdaban;
                        clsSMW.m_Kasho1 = stCASESMW.JointNum;
                        clsSMW.m_ListPileLength1 = stCASESMW.KuiLengthList;

                        FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                        XYZ dir = inst.HandOrientation;
                        XYZ insertPoint = (inst.Location as LocationPoint).Point;
                        ElementId levelId = ClsRevitUtil.GetParameterElementId(doc, id, "集計レベル");
                        //杭に紐づいている壁芯IDを取得
                        ElementId kabeShinId = ClsKabeShin.GetKabeShinId(doc, id);
                        //SMWを作成する
                        clsSMW.CreateSingleSMW(doc, insertPoint, dir, levelId, true, false, kabeShinId: kabeShinId);

                        deleteList.Add(id);
                        foreach (ElementId soilId in soilList)
                        {
                            FamilyInstance soilInst = doc.GetElement(soilId) as FamilyInstance;
                            FamilySymbol soilSym = soilInst.Symbol;
                            string CASE = ClsRevitUtil.GetTypeParameterString(soilSym, "CASE");
                            string edaNum = ClsRevitUtil.GetTypeParameterString(soilSym, "枝番");
                            //ソイルのCASEと枝番号が一致するものからデータを取得する
                            if (stCASESMW.CASE == CASE && stCASESMW.Edaban == edaNum)
                            {
                                if (ClsGeo.GEO_EQ((inst.Location as LocationPoint).Point, (soilInst.Location as LocationPoint).Point))
                                {
                                    ClsSMW.ChangeSoil(doc, soilId, stCASESMW.NewCASE, stCASESMW.NewEdaban, (int)stCASESMW.soilLength, (int)stCASESMW.soilKei);
                                    break;
                                }
                            }
                        }
                    }
                }

                using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                {
                    t.Start();
                    ClsRevitUtil.Delete(doc, deleteList);
                    t.Commit();
                }
            }
        }

        /// <summary>
        /// 連続壁のCASE情報
        /// </summary>
        public class stCASERenzokuKabe
        {
            #region プロパティ
            /// <summary>
            /// CASE番号
            /// </summary>
            public string CASE { get; set; }//ex. CASE1
            /// <summary>
            /// 変更後のCASE
            /// </summary>
            public string NewCASE { get; set; }//ex. CASE1
            /// <summary>
            /// 部材タイプ
            /// </summary>
            public string buzaiType { get; set; }//ex. 細幅
            /// <summary>
            /// 部材サイズ
            /// </summary>
            public string buzaiSize { get; set; }//ex. H-300x150x6.5x9
            /// <summary>
            /// 全長
            /// </summary>
            public double TotalLength { get; set; }//ex. 10000
            /// <summary>
            /// 壁厚
            /// </summary>
            public double kabeThickness { get; set; }//ex.550
            /// <summary>
            /// 壁長
            /// </summary>
            public double kabeLength { get; set; }//ex.13000
            /// <summary>
            /// 材質
            /// </summary>
            public string zaishitsu { get; set; }//ex. SYM295
            /// <summary>
            /// 残置
            /// </summary>
            public string zanchi { get; set; }//ex.　一部埋め殺し
            /// <summary>
            /// 残置長さ
            /// </summary>
            public string zanchiLength { get; set; }// ex. 6000　（全埋め殺しの時は0でよいか）
            /// <summary>
            /// 枝番
            /// </summary>
            public string Edaban { get; set; }//B
            /// <summary>
            /// 変更後の枝番
            /// </summary>
            public string NewEdaban { get; set; }//B
            /// <summary>
            /// ジョイント数
            /// </summary>
            public int JointNum { get; set; }//ex. 2
            /// <summary>
            /// 杭の分割長さのリスト
            /// </summary>
            public List<int> KuiLengthList { get; set; }//ex. 5000　3000　2000
            /// <summary>
            /// 図面上に存在するケースと枝番が一致する数
            /// </summary>
            public int count { get; set; }//20本
            /// <summary>
            /// 対象のElementId
            /// </summary>
            public ElementId ElementId { get; set; }
            /// <summary>
            /// 対象の壁のElementIdList
            /// </summary>
            public List<ElementId> idList { get; set; }
            #endregion
            public stCASERenzokuKabe(Document doc, ElementId id)
            {
                ClsRenzokukabe clsRenzokukabe = new ClsRenzokukabe();
                clsRenzokukabe.SetParameter(doc, id);
                CASE = clsRenzokukabe.m_case.ToString();
                NewCASE = clsRenzokukabe.m_case.ToString();
                buzaiType = clsRenzokukabe.m_type;
                buzaiSize = clsRenzokukabe.m_size;
                TotalLength = clsRenzokukabe.m_HLen;
                kabeThickness = clsRenzokukabe.m_kabeAtumi;
                kabeLength = clsRenzokukabe.m_kabeLen;
                zaishitsu = clsRenzokukabe.m_zaishitu;
                zanchi = clsRenzokukabe.m_zanti;
                zanchiLength = clsRenzokukabe.m_zantiLength;

                Edaban = clsRenzokukabe.m_edaNum;
                NewEdaban = clsRenzokukabe.m_edaNum;
                JointNum = clsRenzokukabe.m_Kasho1;
                KuiLengthList = clsRenzokukabe.m_ListPileLength1;
                count = 0;
                ElementId = id;
                idList = new List<ElementId>();
            }

            public stCASERenzokuKabe()
            {
                KuiLengthList = new List<int>();
                idList = new List<ElementId>();
            }

            public static List<stCASERenzokuKabe> OverLapCASE(List<stCASERenzokuKabe> stCASERenzokuKabeList, bool list = false)
            {
                List<stCASERenzokuKabe> dataList = new List<stCASERenzokuKabe>();

                for (int i = 0; i < stCASERenzokuKabeList.Count; i++)
                {
                    bool bFlag = false;
                    for (int j = 0; j < dataList.Count; j++)
                    {
                        if (dataList[j].CASE == stCASERenzokuKabeList[i].CASE && dataList[j].Edaban == stCASERenzokuKabeList[i].Edaban)
                        {
                            bFlag = true;
                            break;
                        }
                    }
                    if (bFlag)
                        continue;
                    dataList.Add(stCASERenzokuKabeList[i]);

                    int count = 0;
                    List<ElementId> idList = new List<ElementId>();
                    for (int j = 0; j < stCASERenzokuKabeList.Count; j++)
                    {
                        if (stCASERenzokuKabeList[i].CASE == stCASERenzokuKabeList[j].CASE && stCASERenzokuKabeList[i].Edaban == stCASERenzokuKabeList[j].Edaban)
                        {
                            idList.Add(stCASERenzokuKabeList[j].ElementId);
                            count++;
                        }
                    }

                    dataList[dataList.Count - 1].count = count;
                    if (list)
                        dataList[dataList.Count - 1].idList = idList.ToList();
                }
                return dataList;
            }

            public static void CreateCASE(Document doc, List<stCASERenzokuKabe> stCASERenzokuKabeList, List<ElementId> allRenzokuKabeList)
            {
                List<ElementId> kabeList = ClsRenzokukabe.GetAllKabeList(doc);

                List<ElementId> deleteList = new List<ElementId>();
                foreach (stCASERenzokuKabe stCASERenzokuKabe in stCASERenzokuKabeList)
                {
                    foreach (ElementId id in allRenzokuKabeList)
                    {
                        ClsRenzokukabe clsRenzokuKabe = new ClsRenzokukabe();
                        clsRenzokuKabe.SetParameter(doc, id);
                        if (stCASERenzokuKabe.CASE == clsRenzokuKabe.m_case.ToString() && stCASERenzokuKabe.Edaban == clsRenzokuKabe.m_edaNum)
                        {
                            clsRenzokuKabe.m_case = ClsCommonUtils.ChangeStrToInt(stCASERenzokuKabe.NewCASE);
                            clsRenzokuKabe.m_type = stCASERenzokuKabe.buzaiType;
                            clsRenzokuKabe.m_size = stCASERenzokuKabe.buzaiSize;
                            clsRenzokuKabe.m_HLen = (int)stCASERenzokuKabe.TotalLength;
                            clsRenzokuKabe.m_kabeAtumi = (int)stCASERenzokuKabe.kabeThickness;
                            clsRenzokuKabe.m_kabeLen = (int)stCASERenzokuKabe.kabeLength;
                            clsRenzokuKabe.m_zaishitu = stCASERenzokuKabe.zaishitsu;
                            clsRenzokuKabe.m_zanti = stCASERenzokuKabe.zanchi;
                            clsRenzokuKabe.m_zantiLength = stCASERenzokuKabe.zanchiLength;

                            clsRenzokuKabe.m_edaNum = stCASERenzokuKabe.NewEdaban;
                            clsRenzokuKabe.m_Kasho1 = stCASERenzokuKabe.JointNum;
                            clsRenzokuKabe.m_ListPileLength1 = stCASERenzokuKabe.KuiLengthList;

                            FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                            XYZ dir = inst.HandOrientation;
                            XYZ insertPoint = (inst.Location as LocationPoint).Point;
                            ElementId levelId = ClsRevitUtil.GetParameterElementId(doc, id, "集計レベル");
                            //RenzokuKabeを作成する
                            clsRenzokuKabe.CreateSingleRenzokukabe(doc, insertPoint, dir, levelId, true, false);//壁は作成しない

                            deleteList.Add(id);
                        }
                    }

                    foreach (ElementId kabeId in kabeList)
                    {
                        FamilyInstance kabeInst = doc.GetElement(kabeId) as FamilyInstance;
                        FamilySymbol kabeSym = kabeInst.Symbol;
                        string CASE = ClsRevitUtil.GetTypeParameterString(kabeSym, "CASE");
                        //連続壁ののCASEが一致するものからデータを取得する
                        if (stCASERenzokuKabe.CASE == CASE)
                        {
                            ClsRenzokukabe.ChangeKabeParameter(doc, kabeId, stCASERenzokuKabe.NewCASE, (int)stCASERenzokuKabe.kabeThickness, (int)stCASERenzokuKabe.kabeLength);
                        }
                    }
                }
                using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                {
                    t.Start();
                    ClsRevitUtil.Delete(doc, deleteList);
                    t.Commit();
                }
            }
            public static void CreateCASE(Document doc, stCASERenzokuKabe stCASERenzokuKabe, List<ElementId> allRenzokuKabeList)
            {
                List<ElementId> kabeList = ClsRenzokukabe.GetAllKabeList(doc);

                List<ElementId> deleteList = new List<ElementId>();
                foreach (ElementId id in allRenzokuKabeList)
                {
                    ClsRenzokukabe clsRenzokuKabe = new ClsRenzokukabe();
                    clsRenzokuKabe.SetParameter(doc, id);
                    if (stCASERenzokuKabe.CASE == clsRenzokuKabe.m_case.ToString() && stCASERenzokuKabe.Edaban == clsRenzokuKabe.m_edaNum)
                    {
                        clsRenzokuKabe.m_case = ClsCommonUtils.ChangeStrToInt(stCASERenzokuKabe.NewCASE);
                        clsRenzokuKabe.m_type = stCASERenzokuKabe.buzaiType;
                        clsRenzokuKabe.m_size = stCASERenzokuKabe.buzaiSize;
                        clsRenzokuKabe.m_HLen = (int)stCASERenzokuKabe.TotalLength;
                        clsRenzokuKabe.m_kabeAtumi = (int)stCASERenzokuKabe.kabeThickness;
                        clsRenzokuKabe.m_kabeLen = (int)stCASERenzokuKabe.kabeLength;
                        clsRenzokuKabe.m_zaishitu = stCASERenzokuKabe.zaishitsu;
                        clsRenzokuKabe.m_zanti = stCASERenzokuKabe.zanchi;
                        clsRenzokuKabe.m_zantiLength = stCASERenzokuKabe.zanchiLength;

                        clsRenzokuKabe.m_edaNum = stCASERenzokuKabe.NewEdaban;
                        clsRenzokuKabe.m_Kasho1 = stCASERenzokuKabe.JointNum;
                        clsRenzokuKabe.m_ListPileLength1 = stCASERenzokuKabe.KuiLengthList;

                        FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                        XYZ dir = inst.HandOrientation;
                        XYZ insertPoint = (inst.Location as LocationPoint).Point;
                        ElementId levelId = ClsRevitUtil.GetParameterElementId(doc, id, "集計レベル");
                        //RenzokuKabeを作成する
                        clsRenzokuKabe.CreateSingleRenzokukabe(doc, insertPoint, dir, levelId, true, false);//壁は作成しない

                        deleteList.Add(id);
                    }
                }

                foreach (ElementId kabeId in kabeList)
                {
                    FamilyInstance kabeInst = doc.GetElement(kabeId) as FamilyInstance;
                    FamilySymbol kabeSym = kabeInst.Symbol;
                    string CASE = ClsRevitUtil.GetTypeParameterString(kabeSym, "CASE");
                    //連続壁ののCASEが一致するものからデータを取得する
                    if (stCASERenzokuKabe.CASE == CASE)
                    {
                        ClsRenzokukabe.ChangeKabeParameter(doc, kabeId, stCASERenzokuKabe.NewCASE, (int)stCASERenzokuKabe.kabeThickness, (int)stCASERenzokuKabe.kabeLength);
                    }
                }

                using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                {
                    t.Start();
                    ClsRevitUtil.Delete(doc, deleteList);
                    t.Commit();
                }
            }
        }

        public struct stEdaban
        {
            /// <summary>
            /// 枝番
            /// </summary>
            public string Edaban;//B
            /// <summary>
            /// ジョイント数
            /// </summary>
            public int JointNum;//ex. 2
            /// <summary>
            /// 杭の分割長さのリスト
            /// </summary>
            public List<int> KuiLengthList;//ex. 5000　3000　2000
            /// <summary>
            /// 図面上に存在するケースと枝番が一致する数
            /// </summary>
            public int count;//20本

            public stEdaban(string edaban, int jointNum, List<int> kuiLengthList)
            {
                Edaban = edaban;
                JointNum = jointNum;
                KuiLengthList = kuiLengthList;
                count = 0;
            }
        }


        /// <summary>
        /// 図面上から壁情報（杭情報）を集める
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public bool GetCASE_Data(Document doc, enKabeshu kabeshu)
        {

            if (kabeshu == enKabeshu.koyauita || kabeshu == enKabeshu.all)
            {
                List<ElementId> allKouyaitaList = ClsKouyaita.GetAllKouyaitaList(doc);
                foreach (ElementId id in allKouyaitaList)
                {
                    m_koyaitaDataList.Add(new stCASEKoyaita(doc, id));
                }
                m_koyaitaDataList = stCASEKoyaita.OverLapCASE(m_koyaitaDataList).ToList();
            }

            if (kabeshu == enKabeshu.oyagui || kabeshu == enKabeshu.all)
            {
                List<ElementId> allOyakuiList = ClsOyakui.GetAllOyakuiList(doc);
                foreach (ElementId id in allOyakuiList)
                {
                    m_oyaguiDataList.Add(new stCASEOyagui(doc, id));
                }
                m_oyaguiDataList = stCASEOyagui.OverLapCASE(m_oyaguiDataList).ToList();
            }

            if (kabeshu == enKabeshu.smw || kabeshu == enKabeshu.all)
            {
                List<ElementId> allSMWList = ClsSMW.GetAllSMWList(doc);
                foreach (ElementId id in allSMWList)
                {
                    m_smwDataList.Add(new stCASESMW(doc, id));
                }
                m_smwDataList = stCASESMW.OverLapCASE(m_smwDataList);
            }

            if (kabeshu == enKabeshu.renzokukabe || kabeshu == enKabeshu.all)
            {
                List<ElementId> allRenzokuKabeList = ClsRenzokukabe.GetAllRenzokuKabeList(doc);
                foreach (ElementId id in allRenzokuKabeList)
                {
                    m_kabeDataList.Add(new stCASERenzokuKabe(doc, id));
                }
                m_kabeDataList = stCASERenzokuKabe.OverLapCASE(m_kabeDataList);
            }


            return true;
        }

        /// <summary>
        /// 指定のCASEが既に存在するか確認
        /// </summary>
        /// <param name="kabe"></param>
        /// <param name="caseName"></param>
        /// <param name="nextCASE">次に使用できる空きCASE</param>
        /// <param name="checkOtherKabeshu">他の壁種類のCASEもチェックするか</param>
        /// <returns></returns>
        public enCASEHit ExistCase(enKabeshu kabe, string caseName, ref string nextCASE, bool checkOtherKabeshu = false)
        {
            enCASEHit hit = enCASEHit.noHit;
            //List<int> numbers = Enumerable.Range(1, 26).ToList(); //アルファベットのAからB
            List<int> usedCASE = new List<int>();

            if ((kabe == enKabeshu.koyauita) || (kabe != enKabeshu.koyauita && checkOtherKabeshu))
            {
                //鋼矢板検索
                foreach (stCASEKoyaita item in m_koyaitaDataList)
                {
                    if (item.CASE == caseName)
                    {
                        if (kabe == enKabeshu.koyauita)
                        {
                            hit = enCASEHit.sameKabeshu;
                        }
                        else if (kabe != enKabeshu.koyauita && checkOtherKabeshu)
                        {
                            hit = enCASEHit.otherKabeshu;
                        }
                    }
                    string caseStr = item.CASE;
                    int caseNum = 0;
                    if (int.TryParse(caseStr, out caseNum))
                    {
                        usedCASE.Add(caseNum);
                    }
                }

            }

            if ((kabe == enKabeshu.oyagui) || (kabe != enKabeshu.oyagui && checkOtherKabeshu))
            {
                //親杭検索
                foreach (stCASEOyagui item in m_oyaguiDataList)
                {
                    if (item.CASE == caseName)
                    {
                        if (kabe == enKabeshu.oyagui)
                        {
                            hit = enCASEHit.sameKabeshu;
                        }
                        else if (kabe != enKabeshu.oyagui && checkOtherKabeshu)
                        {
                            hit = enCASEHit.otherKabeshu;
                        }
                    }
                    string caseStr = item.CASE;
                    int caseNum = 0;
                    if (int.TryParse(caseStr, out caseNum))
                    {
                        usedCASE.Add(caseNum);
                    }
                }
            }

            if ((kabe == enKabeshu.smw) || (kabe != enKabeshu.smw && checkOtherKabeshu))
            {
                //SMW検索
                foreach (stCASESMW item in m_smwDataList)
                {
                    if (item.CASE == caseName)
                    {
                        if (kabe == enKabeshu.smw)
                        {
                            hit = enCASEHit.sameKabeshu;
                        }
                        else if (kabe != enKabeshu.smw && checkOtherKabeshu)
                        {
                            hit = enCASEHit.otherKabeshu;
                        }
                    }
                    string caseStr = item.CASE;
                    int caseNum = 0;
                    if (int.TryParse(caseStr, out caseNum))
                    {
                        usedCASE.Add(caseNum);
                    }
                }

            }

            if ((kabe == enKabeshu.renzokukabe) || (kabe != enKabeshu.renzokukabe && checkOtherKabeshu))
            {
                //連続壁検索
                foreach (stCASERenzokuKabe item in m_kabeDataList)
                {
                    if (item.CASE == caseName)
                    {
                        if (kabe == enKabeshu.renzokukabe)
                        {
                            hit = enCASEHit.sameKabeshu;
                        }
                        else if (kabe != enKabeshu.renzokukabe && checkOtherKabeshu)
                        {
                            hit = enCASEHit.otherKabeshu;
                        }
                    }
                    string caseStr = item.CASE;
                    int caseNum = 0;
                    if (int.TryParse(caseStr, out caseNum))
                    {
                        usedCASE.Add(caseNum);
                    }
                }
            }


            int tmp = 0;
            if (int.TryParse(caseName, out tmp))
            {
                if (!usedCASE.Contains(tmp))
                {
                    nextCASE = caseName;
                }
                else
                {
                    nextCASE = caseName;
                    if (usedCASE.Count > 0)
                    {
                        IOrderedEnumerable<int> sorted = usedCASE.OrderBy(n => n);
                        List<int> caselist = sorted.ToList();
                        for (int i = 1; i <= caselist[caselist.Count() - 1]; i++)
                        {
                            if (caselist.Contains(i))
                            {
                                continue;
                            }
                            else
                            {
                                nextCASE = i.ToString();
                                break;
                            }
                        }

                    }
                }
            }

            return hit;
        }

        /// <summary>
        /// 指定のCASE-枝番が既に存在するか確認
        /// </summary>
        /// <param name="kabe"></param>
        /// <param name="caseName"></param>
        /// <param name="edaban"></param>
        /// <param name="nextCASEEdaban"></param>
        /// <param name="nextEdaban">同CASE内の未使用の枝番号　※Zまで使い切っていたら「なし」を返却</param>
        /// <param name="checkOtherKabeshu"></param>
        /// <returns></returns>
        public enCASE_EdabanHit ExistCaseEdaban(enKabeshu kabe, string caseName, string edaban, ref string nextCASE, ref string nextEdaban, bool checkOtherKabeshu = false)
        {
            enCASE_EdabanHit hit = enCASE_EdabanHit.noHit;

            List<int> numbers = Enumerable.Range(1, 26).ToList(); //アルファベットのAからB
            List<string> usedEdaban = new List<string>();

            if ((kabe == enKabeshu.koyauita) || (kabe != enKabeshu.koyauita && checkOtherKabeshu))
            {
                //鋼矢板検索
                foreach (stCASEKoyaita item in m_koyaitaDataList)
                {
                    if (item.CASE == caseName && item.Edaban == edaban)
                    {
                        if (kabe == enKabeshu.koyauita)
                        {
                            hit = enCASE_EdabanHit.sameKabeshu;
                        }
                        else if (kabe != enKabeshu.koyauita && checkOtherKabeshu)
                        {
                            hit = enCASE_EdabanHit.otherKabeshu;
                        }
                    }
                    string edabanStr = item.Edaban;
                    if (!string.IsNullOrWhiteSpace(edabanStr))
                    {
                        usedEdaban.Add(item.Edaban);
                    }

                }

            }

            if ((kabe == enKabeshu.oyagui) || (kabe != enKabeshu.oyagui && checkOtherKabeshu))
            {
                //親杭検索
                foreach (stCASEOyagui item in m_oyaguiDataList)
                {
                    if (item.CASE == caseName && item.Edaban == edaban)
                    {
                        if (kabe == enKabeshu.oyagui)
                        {
                            hit = enCASE_EdabanHit.sameKabeshu;
                        }
                        else if (kabe != enKabeshu.oyagui && checkOtherKabeshu)
                        {
                            hit = enCASE_EdabanHit.otherKabeshu;
                        }
                    }
                    string edabanStr = item.Edaban;
                    if (!string.IsNullOrWhiteSpace(edabanStr))
                    {
                        usedEdaban.Add(item.Edaban);
                    }
                }
            }

            if ((kabe == enKabeshu.smw) || (kabe != enKabeshu.smw && checkOtherKabeshu))
            {
                //SMW検索
                foreach (stCASESMW item in m_smwDataList)
                {
                    if (item.CASE == caseName && item.Edaban == edaban)
                    {
                        if (kabe == enKabeshu.smw)
                        {
                            hit = enCASE_EdabanHit.sameKabeshu;
                        }
                        else if (kabe != enKabeshu.smw && checkOtherKabeshu)
                        {
                            hit = enCASE_EdabanHit.otherKabeshu;
                        }
                    }
                    string edabanStr = item.Edaban;
                    if (!string.IsNullOrWhiteSpace(edabanStr))
                    {
                        usedEdaban.Add(item.Edaban);
                    }
                }

            }

            if ((kabe == enKabeshu.renzokukabe) || (kabe != enKabeshu.renzokukabe && checkOtherKabeshu))
            {
                //連続壁検索
                foreach (stCASERenzokuKabe item in m_kabeDataList)
                {
                    if (item.CASE == caseName && item.Edaban == edaban)
                    {
                        if (kabe == enKabeshu.renzokukabe)
                        {
                            hit = enCASE_EdabanHit.sameKabeshu;
                        }
                        else if (kabe != enKabeshu.renzokukabe && checkOtherKabeshu)
                        {
                            hit = enCASE_EdabanHit.otherKabeshu;
                        }
                    }
                    string edabanStr = item.Edaban;
                    if (!string.IsNullOrWhiteSpace(edabanStr))
                    {
                        usedEdaban.Add(item.Edaban);
                    }
                }
            }


            int tmp = 0;
            if (int.TryParse(caseName, out tmp))
            {
                if (!usedEdaban.Contains(edaban))
                {
                    nextCASE = caseName;
                    nextEdaban = edaban;
                }
                else
                {
                    nextCASE = caseName;
                    bool edabanHit = false;
                    for (int i = 1; i <= numbers.Count(); i++)
                    {
                        string al = ClsCommonUtils.ConvertToAlphabet(i);
                        if (usedEdaban.Contains(al))
                        {
                            continue;
                        }
                        else
                        {
                            edabanHit = true;
                            nextCASE = al;
                            break;
                        }
                    }

                    if (!edabanHit)
                    {
                        nextEdaban = "なし";
                    }
                }
            }


            return hit;
        }

        /// <summary>
        /// 指定のCASE-枝番に使用できる枝番号を返す　同CASE内の未使用の枝番号　※Zまで使い切っていたら「なし」を返却
        /// </summary>
        /// <param name="kabe">壁の種類　鋼矢板or親杭orSMWor連続壁</param>
        /// <param name="caseName">ケース名</param>
        /// <param name="edaban">枝番（こいつと対になる枝番号を検索し返す）</param>
        /// <returns></returns>
        public string GetEdabanAlphabet(enKabeshu kabe, string caseName, string edaban)
        {
            string res = "なし";
            List<int> numbers = Enumerable.Range(1, 26).ToList(); //アルファベットのAからB

            List<string> edabanList = new List<string>();

            if (kabe == enKabeshu.koyauita)
            {
                //鋼矢板検索
                foreach (stCASEKoyaita item in m_koyaitaDataList)
                {
                    if (item.CASE == caseName)
                    {
                        edabanList.Add(item.Edaban);
                    }
                }

            }
            else if (kabe == enKabeshu.oyagui)
            {
                //親杭検索
                foreach (stCASEOyagui item in m_oyaguiDataList)
                {
                    if (item.CASE == caseName)
                    {
                        edabanList.Add(item.Edaban);
                    }

                }
            }
            else if (kabe == enKabeshu.smw)
            {
                //SMW検索
                foreach (stCASESMW item in m_smwDataList)
                {
                    if (item.CASE == caseName)
                    {
                        edabanList.Add(item.Edaban);
                    }

                }

            }
            else if (kabe == enKabeshu.renzokukabe)
            {
                //連続壁検索
                foreach (stCASERenzokuKabe item in m_kabeDataList)
                {
                    if (item.CASE == caseName)
                    {
                        edabanList.Add(item.Edaban);
                    }
                }
            }

            int numAL = ClsCommonUtils.ConvertToNumeric(edaban);
            if (numAL > 26)
            {
                //アルファベットの範囲を超えている
                return res;
            }

            //枝番号がC（3）ならプラス1したD（４）から26までを検索し未使用枝番を探す
            bool hit = false;
            for (int i = numAL + 1; i <= 26; i++)
            {
                if (!edabanList.Contains(ClsCommonUtils.ConvertToAlphabet(i)))//i.ToString()))
                {
                    hit = true;
                    res = ClsCommonUtils.ConvertToAlphabet(i);
                    break;
                }
            }

            //上記検索で空きがなかったので、1～枝番号-1までを検索
            if (!hit && 2 > numAL)
            {
                for (int i = 1 + 1; i < numAL; i++)
                {
                    if (!edabanList.Contains(ClsCommonUtils.ConvertToAlphabet(i)))//i.ToString()))
                    {
                        hit = true;
                        res = ClsCommonUtils.ConvertToAlphabet(i);
                        break;
                    }
                }

            }


            return res;
        }
        /// <summary>
        /// 指定のCASE-枝番に使用できる枝番号を返す　同CASE内の未使用の枝番号　※Zまで使い切っていたら「なし」を返却
        /// </summary>
        /// <param name="edaban">枝番（こいつと対になる枝番号を検索し返す）</param>
        /// <param name="edaNumList">現在使用されている枝番のList</param>
        /// <returns></returns>
        public static string GetEdabanAlphabet(string edaban, List<string>edaNumList)
        {
            string res = "なし";
            List<int> numbers = Enumerable.Range(1, 26).ToList(); //アルファベットのAからB


            int numAL = ClsCommonUtils.ConvertToNumeric(edaban);
            if (numAL > 26)
            {
                //アルファベットの範囲を超えている
                return res;
            }

            //枝番号がC（3）ならプラス1したD（４）から26までを検索し未使用枝番を探す
            bool hit = false;
            for (int i = numAL + 1; i <= 26; i++)
            {
                if (!edaNumList.Contains(ClsCommonUtils.ConvertToAlphabet(i)))//i.ToString()))
                {
                    hit = true;
                    res = ClsCommonUtils.ConvertToAlphabet(i);
                    break;
                }
            }

            //上記検索で空きがなかったので、1～枝番号-1までを検索
            if (!hit && 2 > numAL)
            {
                for (int i = 1 + 1; i < numAL; i++)
                {
                    if (!edaNumList.Contains(ClsCommonUtils.ConvertToAlphabet(i)))//i.ToString()))
                    {
                        hit = true;
                        res = ClsCommonUtils.ConvertToAlphabet(i);
                        break;
                    }
                }

            }

            return res;
        }
        public List<stCASEKoyaita> GetKoyaitaCaseList(Document doc)
        {
            List<stCASEKoyaita> koyaitaDataList = new List<stCASEKoyaita>();
            List<ElementId> allKouyaitaList = ClsKouyaita.GetAllKouyaitaList(doc);
            foreach (ElementId id in allKouyaitaList)
            {
                koyaitaDataList.Add(new stCASEKoyaita(doc, id));
            }
            koyaitaDataList = stCASEKoyaita.OverLapCASE(koyaitaDataList).ToList();
            return koyaitaDataList;
        }
        public List<stCASEKoyaita> GetKoyaitaCaseList(Document doc, List<ElementId> ids)
        {
            List<stCASEKoyaita> koyaitaDataList = new List<stCASEKoyaita>();
            foreach (ElementId id in ids)
            {
                FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                if (!inst.Symbol.Name.Contains(ClsKouyaita.KOUYAITA))
                    continue;
                koyaitaDataList.Add(new stCASEKoyaita(doc, id));
            }
            koyaitaDataList = stCASEKoyaita.OverLapCASE(koyaitaDataList, true).ToList();
            return koyaitaDataList;
        }
        public List<stCASEOyagui> GetOyaguiCaseList(Document doc)
        {
            List<stCASEOyagui> oyaguiDataList = new List<stCASEOyagui>();
            List<ElementId> allOyakuiList = ClsOyakui.GetAllOyakuiList(doc);
            foreach (ElementId id in allOyakuiList)
            {
                oyaguiDataList.Add(new stCASEOyagui(doc, id));
            }
            oyaguiDataList = stCASEOyagui.OverLapCASE(oyaguiDataList).ToList();
            return oyaguiDataList;
        }
        public List<stCASEOyagui> GetOyaguiCaseList(Document doc, List<ElementId> ids)
        {
            List<stCASEOyagui> oyaguiDataList = new List<stCASEOyagui>();
            foreach (ElementId id in ids)
            {
                FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                if (!inst.Symbol.Name.Contains(ClsOyakui.oyakui))
                    continue;
                oyaguiDataList.Add(new stCASEOyagui(doc, id));
            }
            oyaguiDataList = stCASEOyagui.OverLapCASE(oyaguiDataList, true).ToList();
            return oyaguiDataList;
        }

        public List<stCASESMW> GetSMWCaseList(Document doc)
        {
            List<stCASESMW> smwDataList = new List<stCASESMW>();
            List<ElementId> allSMWList = ClsSMW.GetAllSMWList(doc);
            foreach (ElementId id in allSMWList)
            {
                smwDataList.Add(new stCASESMW(doc, id));
            }
            smwDataList = stCASESMW.OverLapCASE(smwDataList);
            return smwDataList;
        }
        public List<stCASESMW> GetSMWCaseList(Document doc, List<ElementId> ids)
        {
            List<stCASESMW> smwDataList = new List<stCASESMW>();
            foreach (ElementId id in ids)
            {
                FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                if (!inst.Symbol.Name.Contains(ClsSMW.SMW))
                    continue;
                smwDataList.Add(new stCASESMW(doc, id));
            }
            smwDataList = stCASESMW.OverLapCASE(smwDataList, true);
            return smwDataList;
        }
        public List<stCASERenzokuKabe> GetRenzokukabeCaseList(Document doc)
        {
            List<stCASERenzokuKabe> kabeDataList = new List<stCASERenzokuKabe>();
            List<ElementId> allKabeList = ClsRenzokukabe.GetAllRenzokuKabeList(doc);
            foreach (ElementId id in allKabeList)
            {
                kabeDataList.Add(new stCASERenzokuKabe(doc, id));
            }
            kabeDataList = stCASERenzokuKabe.OverLapCASE(kabeDataList);
            return kabeDataList;
        }
        /// <summary>
        /// 対象の壁のみでクラスを作成する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<stCASERenzokuKabe> GetRenzokukabeCaseList(Document doc, List<ElementId> ids)
        {
            List<stCASERenzokuKabe> kabeDataList = new List<stCASERenzokuKabe>();
            foreach (ElementId id in ids)
            {
                FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                if (!inst.Symbol.Name.Contains(ClsRenzokukabe.RENZOKUKUI))
                    continue;
                kabeDataList.Add(new stCASERenzokuKabe(doc, id));
            }
            kabeDataList = stCASERenzokuKabe.OverLapCASE(kabeDataList, true);
            return kabeDataList;
        }
        ///// <summary>
        ///// CASE情報のリストを使用して杭を編集（再作図？）する処理
        ///// </summary>
        ///// <param name="document"></param>
        ///// <returns></returns>
        //public bool CreateKabeUseCASE(Document document)
        //{

        //    return true;
        //}

        ///// <summary>
        ///// 引数のCASE番号が既に存在しているか
        ///// </summary>
        ///// <param name="CASE"></param>
        ///// <returns></returns>
        //public bool ExistCASE(string CASE)
        //{

        //    return true;
        //}

        ///// <summary>
        ///// 引数のCASE番号+枝番の組み合わせは既に存在しているか
        ///// </summary>
        ///// <param name="CASE"></param>
        ///// <param name="edaban"></param>
        ///// <returns></returns>
        //public bool ExistCASE(string CASE,string edaban)
        //{

        //    return true;
        //}
        public static (string, string) GetCASEAndEdaNum(Document doc, ElementId id)
        {
            FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
            FamilySymbol sym = inst.Symbol;
            string CASE = ClsRevitUtil.GetTypeParameterString(sym, "CASE");
            string edaNum = ClsRevitUtil.GetTypeParameterString(sym, "枝番");
            return (CASE, edaNum);
        }
        /// <summary>
        /// 親杭、鋼矢板 のみを単独選択
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="id">選択した 壁 のID</param>
        /// <param name="message">指示</param>
        /// <returns></returns>
        public static bool PickObjectsKabe(UIDocument uidoc, ref List<ElementId> ids, string message = "壁")
        {
            List<string> filterList = new List<string>();
            filterList.Add(ClsKouyaita.KOUYAITA);
            filterList.Add(ClsKouyaita.CORNERYAITA);
            filterList.Add(ClsKouyaita.IKEIYAITA);
            filterList.Add(ClsOyakui.oyakui);
            filterList.Add(ClsSMW.SMW);
            filterList.Add(ClsRenzokukabe.RENZOKUKUI);

            return ClsRevitUtil.PickObjectsPartListFilter(uidoc, message + "を選択してください", filterList, ref ids);
        }

        public static (List<ElementId>, List<ElementId>) SetOrderKabe(Document doc, List<ElementId> idList)
        {
            List<ElementId> order1List = new List<ElementId>();
            List<ElementId> order2List = new List<ElementId>();

            foreach (ElementId id in idList)
            {
                int putOrder = ClsKabeShin.GetPutOrder(doc, id);
                if (putOrder == 0)
                    order1List.Add(id);
                else if (putOrder == 1)
                    order2List.Add(id);
                else
                    order1List.Add(id);
            }
            return (order1List, order2List);
        }

        #region モーダレス処理
        /// <summary>
        /// 一覧で選択されている行の壁の色を変更する
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="dlgBaseList"></param>
        public static void HighLight(UIDocument uidoc, DLG.DlgCASE dlg)
        {
            Document doc = uidoc.Document;

            //現在選ばれているものを元に戻す
            BackOriginalColor(doc, dlg);
            
            List<ElementId> highLightList = new List<ElementId>();
            List<ElementId> allKabeList = new List<ElementId>();
            switch (dlg.m_enKabeshu)
            {
                case enKabeshu.koyauita:
                    {
                        allKabeList = ClsKouyaita.GetAllKouyaitaList(doc);
                        break;
                    }
                case enKabeshu.oyagui:
                    {
                        allKabeList = ClsOyakui.GetAllOyakuiList(doc);
                        break;
                    }
                case enKabeshu.smw:
                    {
                        allKabeList = ClsSMW.GetAllSMWList(doc);
                        break;
                    }
                case enKabeshu.renzokukabe:
                    {
                        allKabeList = ClsRenzokukabe.GetAllRenzokuKabeList(doc);
                        break;
                    }
                default:
                    return;
            }

            foreach (ElementId id in allKabeList)
            {
                (string CASE, string edaNum) = GetCASEAndEdaNum(doc, id);
                if (dlg.m_HighLightCASE == CASE && dlg.m_HighLightEdaNum == edaNum)
                {
                    highLightList.Add(id);
                    dlg.m_originalSettingsIdList.Add(id);
                    dlg.m_originalSettingsList.Add(doc.ActiveView.GetElementOverrides(id));
                }
            }

            if (highLightList.Count == 0) return;

            //ハイライトにするベースと図面上で選択しているベースが違う場合、選択を解除
            //if (ClsRevitUtil.GetSelectElement(uidoc) != id) ClsRevitUtil.UnSelectElement(uidoc);

            //色の変更
            using (Transaction tx = new Transaction(doc, "Change Line Color"))
            {
                //#31454
                byte r = ClsZumenInfo.GetBaseListSelectedbaseColorR();
                byte g = ClsZumenInfo.GetBaseListSelectedbaseColorG();
                byte b = ClsZumenInfo.GetBaseListSelectedbaseColorB();

                tx.Start();
                ClsRevitUtil.ChangeLineColor(doc, highLightList, new Color(r, g, b));
                tx.Commit();
            }
        }

        /// <summary>
        /// 色が変更されているベースを元の色に戻す
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="dlgBaseList"></param>
        public static void BackOriginalColor(Document doc, DLG.DlgCASE dlg)
        {
            if (dlg.m_originalSettingsIdList != null && dlg.m_originalSettingsList != null)
            {
                for(int i = 0; i < dlg.m_originalSettingsIdList.Count; i++)
                {
                    ClsRevitUtil.SetElementOriginalColor(doc, dlg.m_originalSettingsIdList[i], dlg.m_originalSettingsList[i]);
                }
            }
        }

        public static void ChangeCASE(UIDocument uidoc, DLG.DlgCASE dlg)
        {
            Document doc = uidoc.Document;

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
        public static void WariateCASE(UIDocument uidoc, DLG.DlgCASE dlg)
        {
            Document doc = uidoc.Document;

            List<ClsCASE.stCASEKoyaita> koyaitaCaseData = new List<stCASEKoyaita>();
            foreach(stCASEKoyaita stCASEKoyaita in dlg.m_koyaitaDataList)
            {
                if (stCASEKoyaita.idList.Count != 0)
                    koyaitaCaseData.Add(stCASEKoyaita);
            }

            List<ClsCASE.stCASEOyagui> oyaguiCaseData = new List<stCASEOyagui>();
            foreach(stCASEOyagui stCASEOyagui in dlg.m_oyaguiDataList)
            {
                if (stCASEOyagui.idList.Count != 0)
                    oyaguiCaseData.Add(stCASEOyagui);
            }

            List<ClsCASE.stCASESMW> smwCaseData = new List<stCASESMW>();
            foreach(stCASESMW stCASESMW in dlg.m_smwDataList)
            {
                if (stCASESMW.idList.Count != 0)
                    smwCaseData.Add(stCASESMW);
            }
            List<ClsCASE.stCASERenzokuKabe> kabeCaseData = new List<stCASERenzokuKabe>();
            foreach(stCASERenzokuKabe stCASERenzokuKabe in dlg.m_kabeDataList)
            {
                if (stCASERenzokuKabe.idList.Count != 0)
                    kabeCaseData.Add(stCASERenzokuKabe);
            }

            foreach (stCASEKoyaita stCASEKoyaita in koyaitaCaseData)
            {
                ClsCASE.stCASEKoyaita.CreateCASE(doc, stCASEKoyaita, stCASEKoyaita.idList);
            }

            foreach(stCASEOyagui stCASEOyagui in oyaguiCaseData)
            {
                ClsCASE.stCASEOyagui.CreateCASE(doc, stCASEOyagui, stCASEOyagui.idList);
            }
            
            foreach(stCASESMW stCASESMW in smwCaseData)
            {
                ClsCASE.stCASESMW.CreateCASE(doc, stCASESMW, stCASESMW.idList);
            }
            
            foreach(stCASERenzokuKabe stCASERenzokuKabe in kabeCaseData)
            {
                ClsCASE.stCASERenzokuKabe.CreateCASE(doc, stCASERenzokuKabe, stCASERenzokuKabe.idList);
            }
            
        }
        /// <summary>
        /// ベースダイアログを閉じる
        /// </summary>
        public static void CloseDlg(UIDocument uidoc, DLG.DlgCASE dlg)
        {
            Document doc = uidoc.Document;

            BackOriginalColor(doc, dlg);

            dlg.Close();
        }
        #endregion
    }
}
