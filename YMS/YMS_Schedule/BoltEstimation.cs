using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.UI;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using YMS;
using YMS.Parts;
using YMS.Master;
using static Autodesk.Revit.DB.SpecTypeId;
using static YMS.Parts.ClsTanakui;
using static YMS_Schedule.Enum;
using YMS_gantry;

namespace YMS_Schedule
{

    /// <summary>
    /// ルール積算クラス
    /// </summary>
    public class RuleEstimation
    {
        const string F10T = "F10T-M22x";
        const string S10T = "S10T-";
        const string BN = "BN-";
        const string sumibuV = "V";
        const string sumibuH = "H";
        const string sumibuHR = "HR";
        const string KOUZAI_FLANGE = "フランジ";
        const string KOUZAI_WEB = "ウェブ";
        const string KOUZAI_Plate = "プレート";
        const string KOUKYOUDO = "高強度主材";
        const string SHUZAI = "主材";

        //const string BoltCSV = "ボルト箇所マスタ.csv";

        //const string YOUSETU = "溶接";

        public enum enBotType
        {
            BN,//普通
            F10T,//ハイテンション
            S10T,//トルシアボルト
        }

        //裏込めのデータ
        private Dictionary<string, int> uragomeDictionary = new Dictionary<string, int>();
        //裏込めのデータ　コーナーピース用
        private Dictionary<string, int> uragomeDictionaryForCornerPiece = new Dictionary<string, int>();
        //穴明ライナーのデータ (いまは未使用　空の値が入ってる)
        private Dictionary<string, int> holelinerDictionary = new Dictionary<string, int>();
        //山留の部材+主材系のボルトデータ
        private List<YamadomeKoudaiBoltSekisan.stBoltDataForBoltSheet> yamadomeBoltSheetDatalist = new List<YamadomeKoudaiBoltSekisan.stBoltDataForBoltSheet>();
        //山留の部材+主材系のボルトデータ（ElementID付き）
        private List<YamadomeKoudaiBoltSekisan.stBoltDataForBoltSheet> yamadomeBoltSheetDatalistWithID = new List<YamadomeKoudaiBoltSekisan.stBoltDataForBoltSheet>();
        //主材+主材系のボルトデータ
        private List<YamadomeKoudaiBoltSekisan.stBoltDataForBoltSheet> shuzaiBoltSheetDatalist = new List<YamadomeKoudaiBoltSekisan.stBoltDataForBoltSheet>();
        //主材+主材系のボルトデータ（ElementID付き）
        private List<YamadomeKoudaiBoltSekisan.stBoltDataForBoltSheet> shuzaiBoltSheetDatalistWithID = new List<YamadomeKoudaiBoltSekisan.stBoltDataForBoltSheet>();

        //構台系のボルトデータ
        private List<YamadomeKoudaiBoltSekisan.stBoltDataForBoltSheet> koudaiBoltSheetDatalist = new List<YamadomeKoudaiBoltSekisan.stBoltDataForBoltSheet>();
        //構台系のボルトデータ（ElemetnID付き）
        private List<YamadomeKoudaiBoltSekisan.stBoltDataForBoltSheet> koudaiBoltSheetDatalistWithID = new List<YamadomeKoudaiBoltSekisan.stBoltDataForBoltSheet>();

        //仮鋼材から収集した主材のデータ
        private List<stShuzaiData> shuzaiDataList = new List<stShuzaiData>();
        //仮鋼材から収集した構台側の主材データリスト
        private List<stShuzaiData> koudaiShuzaiDataList = new List<stShuzaiData>();
        //ルール拾いした座金のデータ
        private List<stZaganeData> zaganeDataList = new List<stZaganeData>();
        //ルール拾いしたブラケットのデータ
        private List<stBracketData> bracketDataList = new List<stBracketData>();
        //ルール拾いした斜梁チェーンのデータ
        private List<stSybariChainData> syabariChainDataList = new List<stSybariChainData>();
        //取集した加工品シート（山留）のデータ　
        //private List<stSybariTokushuPieceData> syabariTokushuPieceDataList = new List<stSybariTokushuPieceData>();
        private List<stkakouhinSheetYamadomeData> kakouhinSheetYamadomeDataList = new List<stkakouhinSheetYamadomeData>();
        //取集した加工品シート（構台）のデータ　
        private List<stkakouhinSheetKoudaiData> kakouhinSheetKoudaiDataList = new List<stkakouhinSheetKoudaiData>();


        //収集したジャッキのデータ
        private List<stJackData> JackDataList = new List<stJackData>();


        /// <summary>
        /// ルール拾いしたカバープレートのデータ
        /// </summary>
        public List<stCoverPlate> coverPlateDataList = new List<stCoverPlate>();
        //ボルト箇所マスタ.csv のデータ
        private static List<RuleEstimation> m_Data = new List<RuleEstimation>();

        /// <summary>
        /// 数量表上の「ボルト数量表(山留)」シート用の成型データ構造体
        /// </summary>
        public struct stBoltDataForBoltSheet
        {
            public string hinshu;    //ex.A部品
            public string buzaiName; // ex.カバープレート
            public ElementId buzaiId;
            public string DanName;   //ex.支保工1段目
            public string Kigou;     //ex.50P ﾌﾟﾚ-ﾄ
            public int Kashosu;      // 数量表上は「個数」　意味的には箇所数
            public string boltName;  //ボルト種類 ex.BN-80
            public int num;          //一箇所あたりのボルト本数
            public List<ElementId> PenetrationIds;
        }

        /// <summary>
        /// 仮鋼材　→　主材　情報の構造体
        /// </summary>
        public struct stShuzaiData
        {
            public ElementId id;
            public string DanName;   //ex.支保工1段目
            public string buzaiType;//ex. 腹起
            public string buzaiSize;//ex.40HA
            public bool haraokoshiYokoW;// true　腹起し横W
            public bool haraokoshiTateW;// true　腹起し縦W
            public List<int> lengthList;
        }

        /// <summary>
        /// 仮鋼材→主材に変換した際にルール拾いするカバープレートのデータ
        /// </summary>
        public struct stCoverPlate
        {
            public ElementId karikouzaiId;//積算根拠の仮鋼材のID　または三軸ピースや火打ちブロックのID
            public string DanName; //ex.支保工2段目
            public string setugouBuzai;// ex.腹起し
            public string buzaiSize;//ex. カバープレートのサイズ 20PL 
            public int num;//個数
        }

        /// <summary>
        /// ルール拾いした座金のデータ
        /// </summary>
        public struct stZaganeData
        {
            public ElementId blockId;//積算根拠の(小)火打ちブロックのID
            public string target;//接続してる部材
            public string DanName; //ex.支保工2段目
            public int num;//個数
        }

        ///// <summary>
        ///// 普通ボルトに付随するスプリングワッシャーの情報
        ///// </summary>
        //public struct stWasherWithNB
        //{
        //    public string DanName; //ex.支保工2段目
        //    public int num;//個数
        //}

        /// <summary>
        /// ルール拾いしたブラケットのデータ
        /// </summary>
        public struct stBracketData
        {
            public ElementId karikouzaiId;//積算根拠の仮鋼材のID
            public string youto;//腹起ブラケット or 腹起押えブラケット
            public string name;//ブラケット名称
            public string DanName; //ex.支保工2段目
            public int num;//個数
        }

        /// <summary>
        /// ルール拾いしたブラケットのデータ
        /// </summary>
        public struct stJackData
        {
            public ElementId jackId;//ジャッキのID
            public string kouzaiSize;//鋼材サイズ
            public string jackFamilyName;//ジャッキのファミリ名
        }

        /// <summary>
        /// ルール拾いした斜梁チェーンのデータ
        /// </summary>
        public struct stSybariChainData
        {
            public string buzaiName;
            public ElementId syabariukeId;//積算根拠の斜梁受けピースのID
            public string DanName; //ex.支保工2段目
            public int num;//個数
        }

        public struct stKakouhinData
        {
            public string name;//商品名称　ex.特殊ピース、エンドプレート
            public string type;//鋼材タイプ ex.H型鋼 広幅、トッププレート・エンドプレート
            public string size;// サイズ　ex. H-300X300X10X15
            public string length;//長さ
            public int num;//数量
            public string koukei; //孔径
            public string kouakeKosuu;//孔明け個数
            public string unitWeight;//単位質量
            public string weight;//質量
            public double θ0;//θ0　表示はしない
            public double θ1;//θ1　表示はしない
            public double θ2;//θ2　表示はしない

        }

        //加工品シート（山留）のデータ
        public struct stkakouhinSheetYamadomeData  
        {
            public stKakouhinData oyadata;
            public List<stKakouhinData> koDataList;
            public int num;
        }

        //加工品シート（構台）のデータ
        public struct stkakouhinSheetKoudaiData
        {
            public stKakouhinData oyadata;
            public List<stKakouhinData> koDataList;
            public int num;
        }

        //public struct stSybariTokushuPieceData
        //{
        //    public string buzaiName;
        //    public string DanName; //ex.支保工2段目
        //    public int num;//個数
        //}

        /// <summary>
        /// 積算メイン関数
        /// </summary>
        /// <param name="targetBuzaiIds">検索対象の部材ID 個数が0なら図面からインスタンスを全取得しそれを対象とする</param>
        /// <returns></returns>
        public bool BoltEstimationMain(UIDocument uidoc, List<ElementId> targetBuzaiIds, ClsSekisanSetting sekisansetting)
        {
            ////ダミーファイルの読み込み
            //string dummy = DummyFamily.familyPath();
            //FamilySymbol dummySym = DummyFamily.load(doc, dummy);
            //if (dummySym == null)
            //{
            //    MessageBox.Show("ダミーファイルの読み込みに失敗しました。\n" + dummy);
            //    return false;
            //}

            Document doc = uidoc.Document;

            // 裏込めカウント処理
            //Dictionary<string, int> uragomeDictionary = new Dictionary<string, int>();
            if (!GetUragomeCount(uidoc, uragomeDictionary))
            {
                return false;
            }

            // コーナーピース用を積算する場合のフラグを設定する
            if (sekisansetting.Urakome)
            {
                if (!GetUragomeCountForCornerPiece(doc, uragomeDictionaryForCornerPiece))
                {
                    //return false;
                }
            }

            try
            {
                //図面上からファミリインスタンス全収集
                List<ElementId> elements = ClsRevitUtil.GetAllCreatedFamilyInstanceList(doc);


                if (targetBuzaiIds.Count < 1)
                {
                    targetBuzaiIds.AddRange(elements);
                }

               
                //主材系IDリスト
                List<ElementId> idBuzaiList = new List<ElementId>();
                //主材系以外の山留部材リスト
                List<ElementId> idShuzaiList = new List<ElementId>();

                
                //検索対象のファミリインスタンスを主材系とその他に振り分ける
                foreach (ElementId elem in targetBuzaiIds)
                {

                    FamilyInstance inst = doc.GetElement(elem) as FamilyInstance;
                    if (inst == null)
                    {
                        continue;
                    }

                    //主材系のインスタンスは別途リストに寄り分ける
                    if (inst != null && inst.Symbol.FamilyName.Contains("主材"))
                    {
                        idShuzaiList.Add(inst.Id);
                    }
                    else　if(inst != null && inst.Symbol.FamilyName.Contains("高強度腹起"))//メガビーム対応
                    {
                        idShuzaiList.Add(inst.Id);
                    }
                    else if (inst != null && inst.Symbol.FamilyName.Contains("高強度切梁"))//ツインビーム対応
                    {
                        idShuzaiList.Add(inst.Id);
                    }
                    else if (inst != null && IsKoudaiKarikouzai(inst))
                    {
                        idShuzaiList.Add(inst.Id);
                    }
                    else
                    {
                        idBuzaiList.Add(inst.Id);

                    }
                }



                ///ジャッキ情報のカウント
                CountJack(doc, targetBuzaiIds, idShuzaiList);
                //斜梁チェーンのカウント
                CountSyabariChain(doc, targetBuzaiIds);
                //加工品シート（山留）のカウント
                CountKakouhinYamadome(doc, targetBuzaiIds);
                //加工品シート（構台）のカウント
                CountkakouhinKoudai(doc, targetBuzaiIds);
                //仮鋼材→主材(ついでにカバープレートのルール拾いも行う)
                karikouzai2Shuzai(doc, sekisansetting, idShuzaiList);
                //構台側 仮鋼材→主材
                KoudaiKarikouzai2Shuzai(doc, sekisansetting, idShuzaiList);

                YamadomeKoudaiBoltSekisan YKBolt = new YamadomeKoudaiBoltSekisan();

                //構台ボルト積算
                if (sekisansetting.KoudaiBoltSekisan)
                {
                    YKBolt.KoudaiBoltSekisan(doc, sekisansetting,
                        ref koudaiBoltSheetDatalist, ref koudaiBoltSheetDatalistWithID);
                }
                //主材+主材のボルト積算
                if (sekisansetting.YamdomeBoltSekisan)
                {
                    YKBolt.ShuzaiAndShuzaiBoltData(doc, sekisansetting, idShuzaiList,
                        ref shuzaiBoltSheetDatalist,ref shuzaiBoltSheetDatalistWithID);
                }
                //部材+主材のボルト積算
                if (sekisansetting.YamdomeBoltSekisan)
                {
                    YKBolt.buzaiAndShuzaiBoltData(doc, idBuzaiList, idShuzaiList,
                        ref yamadomeBoltSheetDatalist, ref yamadomeBoltSheetDatalistWithID);
                }
            }
            catch (Exception)
            {

                return false;
            }
            return true;

        }

        /// <summary>
        /// 加工品シート（山留）用データのカウント
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="targetBuzaiIds"></param>
        /// <returns></returns>
        private bool CountKakouhinYamadome(Document doc, List<ElementId> targetBuzaiIds)
        {

            //親ファミリの収集
            //斜梁用特殊ピース
            List<ElementId> oyaTokushu = new List<ElementId>();
            foreach (ElementId id in targetBuzaiIds)
            {
                FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                if (inst == null)
                {
                    continue;
                }

                //斜梁用特殊ピース
                string familyName = inst.Symbol.FamilyName;
                if (familyName.Contains("特殊ﾋﾟｰｽ("))
                {
                    //IDパラメータに任意の値をセットすると子ファミリにも同じパラメータが付与される仕掛けになっているのでこれを利用する
                    using (Transaction tx = new Transaction(doc, "ID set"))
                    {
                        tx.Start();
                        ClsRevitUtil.SetMojiParameter(doc, id, "ID", id.ToString());
                        tx.Commit();
                    }
                    oyaTokushu.Add(id);
                }
            }


            //斜梁用特殊ピース
            foreach (ElementId oyaId in oyaTokushu)
            {
                try
                {
                    string OyaIDparam = ClsRevitUtil.GetParameter(doc, oyaId, "ID");
                    if (string.IsNullOrWhiteSpace(OyaIDparam))
                    {
                        continue;
                    }

                    FamilyInstance inst = doc.GetElement(oyaId) as FamilyInstance;
                    if (inst == null)
                    {
                        continue;
                    }

                    try
                    {
                        stKakouhinData st = new stKakouhinData();
                        st.name = ClsRevitUtil.GetTypeParameterString(inst.Symbol, "中分類");
                        st.type = "H型鋼　広幅";
                        st.size = ClsRevitUtil.GetTypeParameterString(inst.Symbol, "サイズ");
                        double tmp = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetParameterDouble(doc, inst.Id, "長さ"));
                        if (ClsGeo.GEO_GT0(tmp))
                        {
                            st.length = tmp.ToString();
                        }
                        else
                        {
                            st.length = "";
                        }

                        st.num = 1;
                        st.koukei = "";//ClsRevitUtil.GetTypeParameterString(inst.Symbol, "孔径");
                        st.kouakeKosuu = "";// ClsRevitUtil.GetTypeParameterString(inst.Symbol, "孔明け個数");
                        // 1.Ｈ鋼の場合 単位質量はＨ列単位質量に出力、Ｉ列質量は質量パラメータを使用
                        //2.ｴﾝﾄﾞﾌﾟﾚｰﾄ（規格品）の場合 Ｈ列単位質量は質量パラメータを使用、Ｉ列質量は質量パラメータを使用
                        //3.ｴﾝﾄﾞﾌﾟﾚｰﾄ（可変）の場合 Ｈ列単位質量は質量パラメータを使用、Ｉ列質量は質量パラメータを使用
                        st.unitWeight = ClsRevitUtil.GetTypeParameterAsValueString(inst.Symbol, "単位質量");
                        st.weight  =  ClsRevitUtil.GetParameter(doc, inst.Id, "質量");
                        if (string.IsNullOrWhiteSpace(st.weight))
                        {
                            st.weight = ClsRevitUtil.GetTypeParameterAsValueString(inst.Symbol, "質量");
                        }
                        //test = ClsRevitUtil.GetParameter(doc, inst.Id, "質量");
                        st.θ0 = ClsRevitUtil.GetParameterDouble(doc, inst.Id, "θ0");//特殊ピース用の特殊処理
                        st.θ1 = ClsRevitUtil.GetParameterDouble(doc, inst.Id, "θ1");//特殊ピース用の特殊処理
                        st.θ2 = ClsRevitUtil.GetParameterDouble(doc, inst.Id, "θ2");//特殊ピース用の特殊処理

                        List<stKakouhinData> koDataList = new List<stKakouhinData>();
                        //List<FamilyInstance> NestedFamList = ClsRevitUtil.GetNestedFamilyInstances(doc, inst.Id);
                        foreach (ElementId instKOId in targetBuzaiIds)
                        {
                            if (oyaId == instKOId)
                            {
                                //親と同じファミリ
                                continue;
                            }

                            string KoIDparam = ClsRevitUtil.GetParameter(doc, instKOId, "ID");

                            if (OyaIDparam != KoIDparam)
                            {
                                //親と同じIDパラメータを持っていない→子ではない
                                continue;
                            }

                            FamilyInstance instKO = doc.GetElement(instKOId) as FamilyInstance;
                            if (instKO == null)
                            {
                                continue;
                            }

                            stKakouhinData stKo = new stKakouhinData();
                            stKo.name = ClsRevitUtil.GetTypeParameterString(instKO.Symbol, "中分類");
                            stKo.type = "ﾄｯﾌﾟﾌﾟﾚｰﾄ・ｴﾝﾄﾞﾌﾟﾚｰﾄ";
                            if (instKO.Symbol.FamilyName.Contains("可変"))
                            {
                                double W = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetParameterDouble(doc, instKO.Id, "W"));
                                double D = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetParameterDouble(doc, instKO.Id, "D"));
                                double t = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetParameterDouble(doc, instKO.Id, "t"));

                                string tempPLname = "PL-" + t.ToString() + "X" + W.ToString() + "X" + D.ToString();
                                stKo.size = tempPLname;
                            }
                            else
                            {
                                stKo.size = ClsRevitUtil.GetTypeParameterString(instKO.Symbol, "サイズ");
                            }

                            double tmp2 = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetParameterDouble(doc, instKO.Id, "長さ"));
                            if (ClsGeo.GEO_GT0(tmp2))
                            {
                                stKo.length = tmp2.ToString();
                            }
                            else
                            {
                                stKo.length = "";
                            }
                            stKo.num = 1;
                            stKo.koukei = ClsRevitUtil.GetTypeParameterString(instKO.Symbol, "孔径");
                            stKo.kouakeKosuu = "";// ClsRevitUtil.GetTypeParameterString(inst.Symbol, "孔明け個数");
                            //stKo.unitWeight = ClsRevitUtil.GetTypeParameterAsValueString(instKO.Symbol, "単位質量");
                            // 1.Ｈ鋼の場合 単位質量はＨ列単位質量に出力、Ｉ列質量は質量パラメータを使用
                            //2.ｴﾝﾄﾞﾌﾟﾚｰﾄ（規格品）の場合 Ｈ列単位質量は質量パラメータを使用、Ｉ列質量は質量パラメータを使用
                            //3.ｴﾝﾄﾞﾌﾟﾚｰﾄ（可変）の場合 Ｈ列単位質量は質量パラメータを使用、Ｉ列質量は質量パラメータを使用
                            stKo.unitWeight = ClsRevitUtil.GetTypeParameterAsValueString(instKO.Symbol, "質量");
                            if (string.IsNullOrWhiteSpace(stKo.unitWeight))
                            {
                                stKo.unitWeight = ClsRevitUtil.GetParameter(doc, instKO.Id, "質量");
                            }
                            stKo.weight = stKo.unitWeight;

                            koDataList.Add(stKo);
                        }
                        stkakouhinSheetYamadomeData tokushu = new stkakouhinSheetYamadomeData();
                        tokushu.oyadata = st;
                        tokushu.koDataList = koDataList;
                        tokushu.num = 1;
                        kakouhinSheetYamadomeDataList.Add(tokushu);
                    }
                    catch (Exception)
                    {

                        continue;
                    }
                }
                catch (Exception)
                {
                    continue;
                }

            }
            return true;
        }

        private bool CountkakouhinKoudai(Document doc, List<ElementId> targetBuzaiIds)
        {
            //親ファミリの収集
            //高さ調整ピース
            List<ElementId> oyaTakaasaChousei = new List<ElementId>();
            foreach (ElementId id in targetBuzaiIds)
            {
                FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                if (inst == null)
                {
                    continue;
                }

                //高さ調整ピース
                string familyName = inst.Symbol.FamilyName;
                if (familyName.Contains("調整ﾋﾟｰｽ_H-") && !familyName.Contains("ｴﾝﾄﾞﾌﾟﾚｰﾄ"))
                {
                    //IDパラメータに任意の値をセットすると子ファミリにも同じパラメータが付与される仕掛けになっているのでこれを利用する
                    using (Transaction tx = new Transaction(doc, "ID set"))
                    {
                        tx.Start();
                        ClsRevitUtil.SetMojiParameter(doc, id, "ID", id.ToString());
                        tx.Commit();
                    }
                    oyaTakaasaChousei.Add(id);
                }
            }

            //高さ調整ピース
            foreach (ElementId oyaId in oyaTakaasaChousei)
            {
                FamilyInstance inst = doc.GetElement(oyaId) as FamilyInstance;
                if (inst == null)
                {
                    continue;
                }

                //高さ調整ピース
                string familyName = inst.Symbol.FamilyName;
                if (familyName.Contains("調整ﾋﾟｰｽ_H-"))
                {
                    string OyaIDparam = ClsRevitUtil.GetParameter(doc, oyaId, "ID");
                    if (string.IsNullOrWhiteSpace(OyaIDparam))
                    {
                        continue;
                    }

                    try
                    {
                        stKakouhinData st = new stKakouhinData();
                        st.name = ClsRevitUtil.GetTypeParameterString(inst.Symbol, "名称");
                        st.type = ClsRevitUtil.GetTypeParameterString(inst.Symbol, "中分類");
                        st.size = ClsRevitUtil.GetTypeParameterString(inst.Symbol, "サイズ");
                        if (string.IsNullOrWhiteSpace(st.size))
                        {
                            st.size = ClsRevitUtil.GetTypeParameterString(inst.Symbol, "記号");
                        }
                        //double tmp = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetParameterDouble(doc, inst.Id, "長さ"));
                        double tmp = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(inst.Symbol, "長さ"));
                        if (ClsGeo.GEO_GT0(tmp))
                        {
                            st.length = tmp.ToString();
                        }
                        else
                        {
                            st.length = "";
                        }
                        st.num = 1;
                        st.koukei = "";// ClsRevitUtil.GetTypeParameterString(inst.Symbol, "孔径");
                        st.kouakeKosuu = "";// ClsRevitUtil.GetTypeParameterString(inst.Symbol, "孔明け個数");
                        st.unitWeight = ClsRevitUtil.GetTypeParameterAsValueString(inst.Symbol, "単位質量");
                        st.weight = ClsRevitUtil.GetParameter(doc, inst.Id, "質量");
                        if (string.IsNullOrWhiteSpace(st.weight))
                        {
                            st.weight = ClsRevitUtil.GetTypeParameterAsValueString(inst.Symbol, "質量");
                        }

                        List<stKakouhinData> koDataList = new List<stKakouhinData>();

                        foreach (ElementId instKOId in targetBuzaiIds)
                        {
                            if (oyaId == instKOId)
                            {
                                //親と同じファミリ
                                continue;
                            }

                            string KoIDparam = ClsRevitUtil.GetParameter(doc, instKOId, "ID");

                            if (OyaIDparam != KoIDparam)
                            {
                                //親と同じIDパラメータを持っていない→子ではない
                                continue;
                            }

                            FamilyInstance instKO = doc.GetElement(instKOId) as FamilyInstance;
                            if (instKO == null)
                            {
                                continue;
                            }

                            stKakouhinData stKo = new stKakouhinData();
                            stKo.name = ClsRevitUtil.GetTypeParameterString(instKO.Symbol, "中分類");
                            stKo.type = ""; // ClsRevitUtil.GetTypeParameterString(instKO.Symbol, "中分類"); 
                            stKo.size = ClsRevitUtil.GetTypeParameterString(instKO.Symbol, "サイズ");
                            if (string.IsNullOrWhiteSpace(stKo.size))
                            {
                                stKo.size = ClsRevitUtil.GetTypeParameterString(instKO.Symbol, "記号");
                            }
                            //double tmp2 = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetParameterDouble(doc, instKO.Id, "長さ"));
                            double tmp2 = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(instKO.Symbol, "長さ"));
                            if (ClsGeo.GEO_GT0(tmp2))
                            {
                                stKo.length = tmp2.ToString();
                            }
                            else
                            {
                                stKo.length = "";
                            }
                            stKo.num = 1;
                            stKo.koukei = ""; //ClsRevitUtil.GetTypeParameterString(instKO.Symbol, "孔径");
                            stKo.kouakeKosuu = "";// ClsRevitUtil.GetTypeParameterString(instKO.Symbol, "孔明け個数");
                            //stKo.unitWeight = ClsRevitUtil.GetTypeParameterAsValueString(instKO.Symbol, "単位質量");
                            stKo.unitWeight = ClsRevitUtil.GetTypeParameterAsValueString(instKO.Symbol, "質量");
                            if (string.IsNullOrWhiteSpace(stKo.unitWeight))
                            {
                                stKo.unitWeight = ClsRevitUtil.GetParameter(doc, instKO.Id, "質量");
                            }
                            stKo.weight = stKo.unitWeight;


                            koDataList.Add(stKo);
                        }
                        stkakouhinSheetKoudaiData tokushu = new stkakouhinSheetKoudaiData();
                        tokushu.oyadata = st;
                        tokushu.koDataList = koDataList;
                        tokushu.num = 1;
                        kakouhinSheetKoudaiDataList.Add(tokushu);
                    }
                    catch (Exception)
                    {

                        continue;
                    }
                } 
            }
            return true;
        }

        /// <summary>
        /// 斜梁チェーンのカウント　斜梁受けピース一個当たり二個
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="targetBuzaiIds"></param>
        /// <returns></returns>

        private bool CountSyabariChain(Document doc, List<ElementId> targetBuzaiIds)
        {
            
            foreach (ElementId id in targetBuzaiIds)
            {
                try
                {
                    FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                    if (inst == null)
                    {
                        continue;
                    }
                    string familyName = inst.Symbol.FamilyName;
                    if (familyName.Contains("斜梁受ピース"))
                    {
                        stSybariChainData st = new stSybariChainData();
                        st.buzaiName = "FNC";
                        st.syabariukeId = inst.Id;
                        st.DanName = ClsRevitUtil.GetInstMojiParameter(doc, inst.Id, "集計レベル");
                        st.num = 1;
                        syabariChainDataList.Add(st);
                    }
                }
                catch (Exception)
                {
                    continue;
                }
               
            }
            return true;
        }

        /// <summary>
        /// ジャッキ情報の収集
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="targetBuzaiIds"></param>
        /// <returns></returns>
        private bool CountJack(Document doc, List<ElementId> targetBuzaiIds, List<ElementId> shuzaiIds)
        {
            try
            {
                foreach (ElementId id in targetBuzaiIds)
                {
                    if (YMS.Parts.ClsJack.IsJack(doc, id))
                    {
                        string kouzaiSize = string.Empty;

                        ElementId baseId = YMS.ClsKariKouzai.GetBaseId(doc, id);
                        if (baseId == null)
                        {
                             kouzaiSize = GetJackKouzaiSizeOtherMethod(doc, id, shuzaiIds);
                            if (string.IsNullOrWhiteSpace(kouzaiSize))
                            {
                                continue;
                            }
                            FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                            if (inst == null)
                            {
                                continue;
                            }
                            string familyName = inst.Symbol.Family.Name;
                            if (string.IsNullOrWhiteSpace(familyName))
                            {
                                continue;
                            }
                            stJackData jd = new stJackData();
                            jd.jackId = id;
                            jd.kouzaiSize = kouzaiSize;
                            jd.jackFamilyName = familyName;
                            JackDataList.Add(jd);
                        }
                        else
                        {
                             kouzaiSize = ClsYMSUtil.GetKouzaiSizeFromBase(doc, baseId);
                            if (string.IsNullOrWhiteSpace(kouzaiSize))
                            {
                                kouzaiSize = GetJackKouzaiSizeOtherMethod(doc, id, shuzaiIds);
                                if (string.IsNullOrWhiteSpace(kouzaiSize))
                                {
                                    continue;
                                }
                            }
                            FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                            if (inst == null)
                            {
                                continue;
                            }
                            string familyName = inst.Symbol.Family.Name;
                            if (string.IsNullOrWhiteSpace(familyName))
                            {
                                continue;
                            }
                            stJackData jd = new stJackData();
                            jd.jackId = id;
                            jd.kouzaiSize = kouzaiSize;
                            jd.jackFamilyName = familyName;
                            JackDataList.Add(jd);
                        }
                    }
                }
            }
            catch (Exception e)
            {

                return false;
            }

            return true;
        }

        /// <summary>
        /// ジャッキからベース情報が取れてこないケースがあるので応急
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        private string GetJackKouzaiSizeOtherMethod(Document doc,ElementId jackId, List<ElementId> shuzaiIds)
        {
            string res = string.Empty;

            List<ElementId> AllBaseIds = ClsYMSUtil.GetAllBase(doc);
            if (AllBaseIds.Count > 0)
            {
                //ベースとの接触判定を行う
                List<ElementId> ids = ClsRevitUtil.GetIntersectFamilys(doc, jackId,10, null, AllBaseIds);
                foreach (ElementId baseid in ids)
                {
                    string kouzaiSize = ClsYMSUtil.GetKouzaiSizeFromBase(doc, baseid);
                    if (string.IsNullOrWhiteSpace(kouzaiSize))
                    {
                        continue;
                    }
                    res = kouzaiSize;
                }
            }

            //これでも無理なら両サイドの鋼材接触判定
            if (string.IsNullOrWhiteSpace(res))
            {
                List<ElementId> ids = ClsRevitUtil.GetIntersectFamilys(doc, jackId, 0.1, null, shuzaiIds);
                foreach (ElementId shuzaiId in ids)
                {
                    FamilyInstance inst = doc.GetElement(shuzaiId) as FamilyInstance;
                    if (inst == null)
                    {
                        continue;
                    }

                    string size = ClsYMSUtil.GetSyuzaiSize(inst.Symbol.FamilyName);
                    if (string.IsNullOrWhiteSpace(size))
                    {
                        continue;
                    }
                    res = size;
                }
            }

            return res;
        }

        /// <summary>
        /// 割付主材と仮鋼材が混在していたらtrueを返す
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="targetBuzaiIds">積算対象の部材　0なら全インスタンスを対象とする</param>
        /// <returns></returns>
        public static bool IsMixedWaritukeShuzaiAndKarikouzai(Document doc,List<ElementId> targetBuzaiIds)
        {
            bool res = false;
            int waritukeCnt = 0;
            int karikouzaiCnt = 0;

            //図面上からファミリインスタンス全収集
            List<ElementId> elements = ClsRevitUtil.GetAllCreatedFamilyInstanceList(doc);


            if (targetBuzaiIds.Count < 1)
            {
                targetBuzaiIds.AddRange(elements);
            }


            List<ElementId> targetFamilies = new List<ElementId>();

            foreach (ElementId elem in targetBuzaiIds)
            {

                FamilyInstance inst = doc.GetElement(elem) as FamilyInstance;
                if (inst == null)
                {
                    continue;
                }

                if (inst != null && inst.Symbol.FamilyName.Contains("主材"))
                {
                    if (inst.Symbol.FamilyName.Contains("仮鋼材"))
                    {
                        karikouzaiCnt++;
                    }
                    else
                    {
                        waritukeCnt++;
                    }
                }
                else if (inst != null && inst.Symbol.FamilyName.Contains("高強度腹起"))
                {
                    if (inst.Symbol.FamilyName.Contains("仮鋼材"))
                    {
                        karikouzaiCnt++;
                    }
                    else
                    {
                        waritukeCnt++;
                    }
                }
            }
            
            if (waritukeCnt > 0 && karikouzaiCnt > 0)
            {
                res = true;
            }

            return res;
        }

        /// <summary>
        /// 山留側の仮鋼材⇒主材に変換
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="sekisansetting"></param>
        /// <param name="shuzaiIdList"></param>
        /// <returns></returns>
        private bool karikouzai2Shuzai(Document doc, ClsSekisanSetting sekisansetting, List<ElementId> shuzaiIdList)
        {
            try
            {
                List<FamilyInstance> instShuzaiList = YamadomeKoudaiBoltSekisan.ElementIdList2FamilyinstanceList(doc,shuzaiIdList);

                //仮鋼材の収集
                foreach (FamilyInstance shuzaiInst in instShuzaiList)
                {
                    if (shuzaiInst.Symbol.FamilyName.Contains("仮鋼材"))
                    {
                        //メガビーム特殊処理
                        if (shuzaiInst.Symbol.FamilyName.Contains("高強度腹起_80SMH"))
                        {
                            string typeName = shuzaiInst.Name;
                            stShuzaiData st = new stShuzaiData();
                            st.lengthList = new List<int>();
                            st.lengthList.Add(9000);
                            st.buzaiType = typeName;
                            st.id = shuzaiInst.Id;
                            st.buzaiSize = ClsRevitUtil.GetTypeParameterString(shuzaiInst.Symbol, "記号");
                            //段名（レベル名）
                            //st.DanName = ClsRevitUtil.GetInstMojiParameter(doc, shuzaiInst.Id, "集計レベル");

                            ////仮鋼材に紐づいたベースを取得（レベルを取りたい）
                            //ElementId baseId = YMS.Parts.ClsWarituke.GetConnectionBase(doc, shuzaiInst.Id);
                            //if (baseId == null)
                            //{
                            //    continue;
                            //}

                            st.haraokoshiYokoW = false;

                            //ElementId lvId = ClsRevitUtil.GetFamilyInstanceLevel(doc, baseId);

                            //Level lv = doc.GetElement(lvId) as Level;
                            string levelName = ClsRevitUtil.GetInstMojiParameter(doc, shuzaiInst.Id, "集計レベル");
                            if (levelName != null && !string.IsNullOrWhiteSpace(levelName))
                            {
                                st.DanName = levelName;
                            }
                            
                            shuzaiDataList.Add(st);

                        }
                        else
                        {
                            string typeName = shuzaiInst.Name;
                            if (typeName == "腹起" || typeName == "切梁" || typeName == "隅火打" || typeName == "切梁火打")
                            {
                                double divShuzaiLength = 0.0;

                                if (typeName == "腹起")
                                {
                                    divShuzaiLength = sekisansetting.KariHaraOkoshi;
                                }
                                else if (typeName == "切梁")
                                {
                                    divShuzaiLength = sekisansetting.KariKiribari;
                                }
                                else if (typeName == "隅火打")
                                {
                                    divShuzaiLength = sekisansetting.KariCornerHiuchi;
                                }
                                else if (typeName == "切梁火打")
                                {
                                    divShuzaiLength = sekisansetting.KariKiribariHiuchi;
                                }

                                if (ClsGeo.GEO_EQ0(divShuzaiLength) || ClsGeo.GEO_LE0(divShuzaiLength))
                                {
                                    continue;
                                }

                                LocationCurve tmpC = shuzaiInst.Location as LocationCurve;
                                if (tmpC == null)
                                {
                                    continue;
                                }

                                decimal length = (decimal)ClsRevitUtil.CovertFromAPI(tmpC.Curve.GetEndPoint(0).DistanceTo(tmpC.Curve.GetEndPoint(1)));

                                double num = Math.Floor((double)Math.Abs(length) / divShuzaiLength);
                                int nNum = (int)num;
                                int amari = (int)((double)length % divShuzaiLength);

                                stShuzaiData st = new stShuzaiData();
                                st.lengthList = new List<int>();
                                for (int i = 0; i < nNum; i++)
                                {
                                    st.lengthList.Add((int)divShuzaiLength);
                                }
                                if (amari > 0)
                                {
                                    st.lengthList.Add(amari);
                                }
                                st.buzaiType = typeName;
                                st.id = shuzaiInst.Id;
                                st.buzaiSize = ClsRevitUtil.GetTypeParameterString(shuzaiInst.Symbol, "記号");
                                //段名（レベル名）
                                //st.DanName = ClsRevitUtil.GetInstMojiParameter(doc, shuzaiInst.Id, "集計レベル");

                                //仮鋼材に紐づいたベースを取得（レベルを取りたい）
                                ElementId baseId = YMS.Parts.ClsWarituke.GetConnectionBase(doc, shuzaiInst.Id);
                                if (baseId == null)
                                {
                                    continue;
                                }

                                bool haraokoshiYokoW = false;
                                if (ClsRevitUtil.GetInstMojiParameter(doc, baseId, "横本数") == "ダブル")
                                {
                                    haraokoshiYokoW = true;
                                }
                                st.haraokoshiYokoW = haraokoshiYokoW;

                                bool haraokoshiTateW = false;
                                if (ClsRevitUtil.GetInstMojiParameter(doc, baseId, "縦本数") == "ダブル")
                                {
                                    haraokoshiTateW = true;
                                }
                                st.haraokoshiTateW = haraokoshiTateW;

                                ElementId lvId = ClsRevitUtil.GetFamilyInstanceLevel(doc, baseId);

                                Level lv = doc.GetElement(lvId) as Level;

                                st.DanName = lv.Name;
                                shuzaiDataList.Add(st);
                            }
                        }
                    }
                }

                //仮鋼材→主材データを利用し、ｶﾊﾞｰﾌﾟﾚｰﾄの数量を計算
                if (coverPlateDataList == null)
                {
                    coverPlateDataList = new List<stCoverPlate>();
                }
                
                foreach (stShuzaiData st in shuzaiDataList)
                {

                    stCoverPlate cp = new stCoverPlate();
                    string cpSize = string.Empty;
                    string shuzaiSize = st.buzaiSize;
                    switch (shuzaiSize)
                    {
                        case "20HA":
                            cpSize = "カバープレート_20PL";
                            break;
                        case "25HA":
                            cpSize = "カバープレート_25PL";
                            break;
                        case "30HA":
                            cpSize = "カバープレート_30PL";
                            break;
                        case "35HA":
                            cpSize = "カバープレート_35PL";
                            break;
                        case "40HA":
                            cpSize = "カバープレート_40PL";
                            break;
                        case "50HA":
                            cpSize = "カバープレート_50PL";
                            break;
                        case "35SMH":
                            cpSize = "高強度ｶﾊﾞｰﾌﾟﾚｰﾄ_SM-35P";
                            break;
                        case "40SMH":
                            cpSize = "高強度ｶﾊﾞｰﾌﾟﾚｰﾄ_SM-40P";
                            break;
                        default:
                            continue;
                    }
                    cp.karikouzaiId = st.id;
                    cp.buzaiSize = cpSize;
                    cp.DanName = st.DanName;
                    cp.setugouBuzai = st.buzaiType;

                    int shuzaiBunkatuNum = st.lengthList.Count - 1;

                    if (shuzaiBunkatuNum < 1)
                    {
                        continue;
                    }

                    if (st.haraokoshiYokoW)
                    {
                        cp.num = 1 * shuzaiBunkatuNum;
                    }
                    else
                    {
                        cp.num = 2 * shuzaiBunkatuNum;
                    }


                    coverPlateDataList.Add(cp);
                }

                //火打ちブロックと三軸ピースとの接続も確認し、接続していたらｶﾊﾞｰﾌﾟﾚｰﾄを追加する
                CoverPlate_Sanjuku_HiutiBlock_karikouzai(doc);
                //座金の積算
                CountZageneData(doc);
                //ブラケットの積算
                CountBracketData(doc);
                CountSyabariOsaeBracketData(doc);

            }
            catch (Exception)
            {

                return false; ;
            }

            return true;
        }

        /// <summary>
        /// 構台側の仮鋼材⇒主材に変換
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="sekisansetting"></param>
        /// <param name="shuzaiIdList"></param>
        /// <returns></returns>
        private bool KoudaiKarikouzai2Shuzai(Document doc, ClsSekisanSetting sekisansetting,List<ElementId> shuzaiIdList)
        {
            try
            {
                List<FamilyInstance> instShuzaiList = YamadomeKoudaiBoltSekisan.ElementIdList2FamilyinstanceList(doc, shuzaiIdList);
                //仮鋼材の収集
                foreach (FamilyInstance shuzaiInst in instShuzaiList)
                {
                    if (IsKoudaiKarikouzai(shuzaiInst))
                    {
                        string typeName = shuzaiInst.Name;
                        if (typeName.Contains("支柱") || typeName.Contains("方杖"))
                        {
                            double divShuzaiLength = 0.0;
                            //sekisansetting.KariHaraOkoshi;

                            if (typeName.Contains("支柱"))
                            {
                                divShuzaiLength = sekisansetting.KariShichu;
                            }
                            else if (typeName.Contains("方杖"))
                            {
                                divShuzaiLength = sekisansetting.KariHoujou;
                            }

                            Curve c = YMS_gantry.GantryUtil.GetCurve(doc, shuzaiInst.Id);
                            if (c == null)
                            {
                                continue;
                            }
                            decimal length = (decimal)ClsRevitUtil.CovertFromAPI(c.GetEndPoint(0).DistanceTo(c.GetEndPoint(1)));

                            double num = Math.Floor((double)Math.Abs(length) / divShuzaiLength);
                            int nNum = (int)num;
                            int amari = (int)((double)length % divShuzaiLength);
                            stShuzaiData st = new stShuzaiData();
                            st.lengthList = new List<int>();
                            for (int i = 0; i < nNum; i++)
                            {
                                st.lengthList.Add((int)divShuzaiLength);
                            }
                            if(amari>0)
                            {
                                st.lengthList.Add(amari);
                            }
                            st.buzaiType = typeName.Contains("柱") ? "支柱" : "方杖";
                            st.id = shuzaiInst.Id;
                            st.buzaiSize = ClsRevitUtil.GetTypeParameterString(shuzaiInst.Symbol, "記号");

                            //仮鋼材のホストから配置レベルを取得
                            Autodesk.Revit.DB.Reference refer = YMS_gantry.GantryUtil.GetReference(shuzaiInst);
                            if (refer != null)
                            {
                                st.DanName = "";
                            }
                            else
                            {
                                Element elm = doc.GetElement(refer);
                                if (elm is Level level)
                                {
                                    st.DanName = level.Name;
                                }
                                else
                                {
                                    st.DanName = "";
                                }
                            }

                           
                            koudaiShuzaiDataList.Add(st);
                        }
                    }
                }

                //仮鋼材→主材データを利用し、ｶﾊﾞｰﾌﾟﾚｰﾄの数量を計算
                if (coverPlateDataList == null)
                {
                    coverPlateDataList = new List<stCoverPlate>();
                }

                foreach (stShuzaiData st in koudaiShuzaiDataList)
                {

                    stCoverPlate cp = new stCoverPlate();
                    string cpSize = string.Empty;
                    string shuzaiSize = st.buzaiSize;
                    switch (shuzaiSize)
                    {
                        case "20HA":
                            cpSize = "カバープレート_20PL";
                            break;
                        case "25HA":
                            cpSize = "カバープレート_25PL";
                            break;
                        case "30HA":
                            cpSize = "カバープレート_30PL";
                            break;
                        case "35HA":
                            cpSize = "カバープレート_35PL";
                            break;
                        case "40HA":
                            cpSize = "カバープレート_40PL";
                            break;
                        case "50HA":
                            cpSize = "カバープレート_50PL";
                            break;
                        case "35SMH":
                            cpSize = "高強度ｶﾊﾞｰﾌﾟﾚｰﾄ_SM-35P";
                            break;
                        case "40SMH":
                            cpSize = "高強度ｶﾊﾞｰﾌﾟﾚｰﾄ_SM-40P";
                            break;
                        default:
                            continue;
                    }
                    cp.karikouzaiId = st.id;
                    cp.buzaiSize = cpSize;
                    cp.DanName = st.DanName;
                    cp.setugouBuzai = st.buzaiType;

                    int shuzaiBunkatuNum = st.lengthList.Count - 1;

                    if (shuzaiBunkatuNum < 1)
                    {
                        continue;
                    }

                    cp.num = 2 * shuzaiBunkatuNum;


                    coverPlateDataList.Add(cp);
                }


            }
            catch (Exception)
            {

                return false; ;
            }

            return true;
        }

        /// <summary>
        /// 座金のカウント
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        private bool CountZageneData(Document doc)
        {
            try
            {
                if (zaganeDataList == null)
                {
                    zaganeDataList = new List<stZaganeData>();
                }

                List<ElementId> hiuchiBlockIds = ClsRevitUtil.GetSelectCreatedFamilyInstanceList(doc, "火打ブロック", true);
                if (hiuchiBlockIds.Count() == 0)
                {
                    return false;
                }

                foreach (var item in hiuchiBlockIds)
                {
                    stZaganeData st = new stZaganeData();

                    FamilyInstance inst = doc.GetElement(item) as FamilyInstance;
                    if (inst.Symbol.FamilyName.Contains("小火打ブロック"))
                    {
                        //小火打ちブロック
                        st.blockId = item;
                        st.DanName = ClsRevitUtil.GetInstMojiParameter(doc, item, "集計レベル");
                        st.num = 12;
                        st.target = "小火打ブロック";
                    }
                    else
                    {
                        //火打ちブロック
                        st.blockId = item;
                        st.DanName = ClsRevitUtil.GetInstMojiParameter(doc, item, "集計レベル");
                        st.num = 20;//#32506
                        st.target = "火打ブロック";
                    }
                    zaganeDataList.Add(st);
                }
            }
            catch (Exception)
            {

                return false; ;
            }

            return true;
        }

        private bool CountBracketData(Document doc)
        {
            try
            {
                foreach (stShuzaiData st in shuzaiDataList)
                {
                    bool megabeamFlg = false;
                    bool twinbeamFlg = false;
                    stBracketData bd = new stBracketData();
                    string shuzaiSize = st.buzaiSize;
                    
                    int numV, numH;
                    if (st.haraokoshiTateW)
                    {
                        numV = 2;
                    } else
                    {
                        numV = 1;
                    }

                    if (st.haraokoshiYokoW)
                    {
                        numH = 2;
                    } else
                    {
                        numH = 1;
                    }

                    if (shuzaiSize == "80SMH")
                    {
                        megabeamFlg = true;
                        numV = 1;
                        numH = 2;
                    }

                    if (shuzaiSize == "60SMH")
                    {
                        twinbeamFlg = true;
                        numV = 1;
                        numH = 1;
                    }

                    if (st.buzaiType != "腹起")
                    {
                        if (!twinbeamFlg)
                        {
                            continue;
                        }
                    }


                    string bSize = ClsBracket.GetBracketSize(shuzaiSize, numV, numH);

                    if (string.IsNullOrWhiteSpace(bSize))
                    {
                        continue;
                    }

                    bd.karikouzaiId = st.id;
                    bd.name = bSize;
                    bd.youto = megabeamFlg ? "腹起押えブラケット" : "腹起ブラケット";
                    if (!megabeamFlg && twinbeamFlg)
                    {
                        bd.youto = "腹起押えブラケット";
                    }
                    bd.DanName = st.DanName;

                    if (st.lengthList.Count < 1)
                    {
                        continue;
                    }

                    bd.num = 0;

                    if (twinbeamFlg)
                    {
                        //ツインビームは一つの芯辺りブラケット四本
                        bd.num = 4;
                    }
                    else
                    {
                        foreach (int len in st.lengthList)
                        {
                            if (RevitUtil.ClsGeo.GEO_LT(len, 1000))
                            {
                                //1000未満は存在しないのでスキップ
                                continue;
                            }
                            if (megabeamFlg)
                            {
                                bd.num += 3;
                            }
                            else
                            {
                                if (st.haraokoshiYokoW)
                                {
                                    //横ダブルは一組二本中一つだけに発生　通常は一本当たりブラケット2個だが1個として積算し、辻褄を合わせる
                                    bd.num += 1;
                                }
                                else
                                {
                                    bd.num += 2;
                                }
                            }

                        }

                        if (bd.num < 1)
                        {
                            continue;
                        }
                    }

                    bracketDataList.Add(bd);
                }
            }
            catch (Exception e)
            {

                return false;
            }
            return true;
        }

        /// <summary>
        /// 斜梁押えブラケットの積算
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="shuzaiIdList"></param>
        /// <returns></returns>
        private bool CountSyabariOsaeBracketData(Document doc)
        {
            try
            {

                //図面上からファミリインスタンス全収集
                List<ElementId> elements = ClsRevitUtil.GetAllCreatedFamilyInstanceList(doc);
                foreach (ElementId elem in elements)
                {
                    FamilyInstance inst = doc.GetElement(elem) as FamilyInstance;
                    if (inst == null)
                    {
                        continue;
                    }

                    //主材系のインスタンスは別途リストに寄り分ける
                    if (inst != null && inst.Symbol.FamilyName.Contains("斜梁"))
                    {
                        if (inst.Symbol.FamilyName.Contains("仮鋼材"))
                        {
                            stBracketData bd = new stBracketData();

                            bd.karikouzaiId = inst.Id;
                            bd.name = "";//空白で良い
                            bd.youto = "斜梁押えブラケット";
                            bd.DanName = ClsRevitUtil.GetInstMojiParameter(doc, inst.Id, "集計レベル");
                            bd.num = 2;
                            bracketDataList.Add(bd);
                        }
                    }
                    
                }
            }
            catch (Exception e)
            {

                return false;
            }
            return true;
        }

        private bool CoverPlate_Sanjuku_HiutiBlock_karikouzai(Document doc)
        {
            try
            {
                //仮鋼材の収集
                if (coverPlateDataList == null)
                {
                    coverPlateDataList = new List<stCoverPlate>();
                }

                foreach (stShuzaiData st in shuzaiDataList)
                {
                    if (st.buzaiType != "切梁")
                    {
                        continue;
                    }

                    if (st.lengthList.Count == 1 && st.lengthList[0] < 1000)
                    {
                        continue;
                    }

                    //三軸ピースと結合しているか
                    List<ElementId> sanjikuIds = new List<ElementId>();
                    if (!HitWithSanjikuBlock(doc, st.id,ref sanjikuIds))
                    {
                        continue;
                    }

                    foreach (ElementId sanjikId in sanjikuIds)
                    {
                        stCoverPlate cp = new stCoverPlate();
                        string cpSize = string.Empty;
                        string shuzaiSize = st.buzaiSize;
                        switch (shuzaiSize)
                        {
                            case "20HA":
                                cpSize = "カバープレート_20PL";
                                break;
                            case "25HA":
                                cpSize = "カバープレート_25PL";
                                break;
                            case "30HA":
                                cpSize = "カバープレート_30PL";
                                break;
                            case "35HA":
                                cpSize = "カバープレート_35PL";
                                break;
                            case "40HA":
                                cpSize = "カバープレート_40PL";
                                break;
                            case "50HA":
                                cpSize = "カバープレート_50PL";
                                break;
                            case "35SMH":
                                cpSize = "高強度ｶﾊﾞｰﾌﾟﾚｰﾄ_SM-35P";
                                break;
                            case "40SMH":
                                cpSize = "高強度ｶﾊﾞｰﾌﾟﾚｰﾄ_SM-40P";
                                break;
                            default:
                                continue;
                        }
                        cp.karikouzaiId = sanjikId;
                        cp.buzaiSize = cpSize;
                        cp.DanName = st.DanName;
                        cp.setugouBuzai = "三軸ピース";
                        cp.num = 2;
                        coverPlateDataList.Add(cp);
                        //if (coverPlateDataList.Count < 1)
                        //{
                        //    coverPlateDataList.Add(cp);
                        //    continue;
                        //}

                        //bool hit = false;
                        //for (int i = 0; i < coverPlateDataList.Count; i++)
                        //{
                        //    stCoverPlate sCp = coverPlateDataList[i];
                        //    if (sCp.buzaiSize == cpSize && sCp.DanName == cp.DanName && sCp.setugouBuzai == cp.setugouBuzai)
                        //    {
                        //        sCp.num += cp.num;
                        //        coverPlateDataList[i] = sCp;
                        //        hit = true;
                        //        break;
                        //    }
                        //}
                        //if (!hit)
                        //{
                        //    coverPlateDataList.Add(cp);
                        //}
                    }


                }

                if (true)
                {
                    //火打ちブロックはそのまま個数カウントする（接続を見ない）
                    List<ElementId> hiuchiBlockIds = ClsRevitUtil.GetSelectCreatedFamilyInstanceList(doc, "火打ブロック", true);
                    foreach (var item in hiuchiBlockIds)
                    {
                        FamilyInstance inst = doc.GetElement(item) as FamilyInstance;
                        string cpSize = string.Empty;
                        if (inst.Symbol.FamilyName.Contains("HB_30HB"))
                        {
                            cpSize = "カバープレート_30PL";
                        }
                        else if (inst.Symbol.FamilyName.Contains("HB_35HB"))
                        {
                            cpSize = "カバープレート_35PL";
                        }
                        else if (inst.Symbol.FamilyName.Contains("HB_40HB"))
                        {
                            cpSize = "カバープレート_40PL";
                        }
                        else if (inst.Symbol.FamilyName.Contains("HB_50HB"))
                        {
                            cpSize = "カバープレート_50PL";
                        }
                        else
                        {
                            continue;
                        }
                        stCoverPlate cp = new stCoverPlate();
                        cp.karikouzaiId = item;
                        cp.buzaiSize = cpSize;
                        cp.DanName = ClsRevitUtil.GetInstMojiParameter(doc, item, "集計レベル"); ;
                        cp.setugouBuzai = "火打ブロック";
                        cp.num = 2;
                        coverPlateDataList.Add(cp);
                    }
                }
            }
            catch (Exception)
            {

                return false; ;
            }

            return true;
        }

        /// <summary>
        /// 対象の部材が三軸ピースと干渉しているか
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="syuzaiId"></param>
        /// <returns></returns>
        private bool HitWithSanjikuBlock(Document doc, ElementId syuzaiId,ref List<ElementId> hitSanjikuIds)
        {
            List<ElementId> sanjikuIds = ClsSanjikuPeace.GetAllSanjikuPeaceList(doc);


            if (sanjikuIds.Count() == 0)
            {
                return false;
            }
            List<ElementId> ids = ClsRevitUtil.GetIntersectFamilys(doc, syuzaiId, 2, null, sanjikuIds);
            if (ids.Count() > 0)
            {
                hitSanjikuIds.AddRange(ids);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 対象の部材が火打ブロックと干渉しているか（小火打ちブロックは除く）
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="syuzaiId"></param>
        /// <returns></returns>
        private  bool HitWithHiuchiBlock(Document doc, ElementId syuzaiId)
        {
            List<ElementId> hiuchiBlockIds = ClsRevitUtil.GetSelectCreatedFamilyInstanceList(doc, "火打ブロック", true);
            if (hiuchiBlockIds.Count() == 0)
            {
                return false;
            }
            List<ElementId> ids = ClsRevitUtil.GetIntersectFamilys(doc, syuzaiId, 0.1, null, hiuchiBlockIds);
            foreach (var item in ids)
            {
                FamilyInstance inst = doc.GetElement(item) as FamilyInstance;
                if (inst.Symbol.FamilyName.Contains("小火打ブロック"))
                {
                    return false;
                }
            }

            if (ids.Count() > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 対象の部材が小火打ブロックと干渉しているか（火打ちブロックは除く）
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="syuzaiId"></param>
        /// <returns></returns>
        private bool HitWithShouHiuchiBlock(Document doc, ElementId syuzaiId)
        {
            List<ElementId> hiuchiBlockIds = ClsRevitUtil.GetSelectCreatedFamilyInstanceList(doc, "小火打ブロック", true);
            if (hiuchiBlockIds.Count() == 0)
            {
                return false;
            }
            List<ElementId> ids = ClsRevitUtil.GetIntersectFamilys(doc, syuzaiId, 0.1, null, hiuchiBlockIds);
    
            if (ids.Count() > 0)
            {
                return true;
            }

            return false;
        }


        public string GetYamadomeBoltData()
        {
            yamadomeBoltSheetDatalist.AddRange(shuzaiBoltSheetDatalist);
            string str = "品種,名称,取付段,記号,個数,ボルト種類,本/個数,ボルト数量" + Environment.NewLine;
            foreach (YamadomeKoudaiBoltSekisan.stBoltDataForBoltSheet st in yamadomeBoltSheetDatalist)
            {
                str += st.hinshu + "," + st.buzaiName + "," + st.DanName + "," + st.Kigou + "," + st.Kashosu + "," + st.boltName + "," + st.num + "," + (st.Kashosu * st.num).ToString() + Environment.NewLine;
            }
            return str;
        }

        public string GetYamadomeBoltDataWithElementID()
        {
            yamadomeBoltSheetDatalistWithID.AddRange(shuzaiBoltSheetDatalistWithID);
            string str = "部材ID,品種,名称,取付段,記号,個数,ボルト種類,本/個数,ボルト数量,接合部材ID" + Environment.NewLine; ;
            foreach (YamadomeKoudaiBoltSekisan.stBoltDataForBoltSheet st in yamadomeBoltSheetDatalistWithID)
            {
                int cnt = 0;
                string tmp = string.Empty;
                if (st.PenetrationIds != null)
                {
                    foreach (ElementId id in st.PenetrationIds)
                    {
                        if (cnt == 0)
                        {
                            tmp = id.ToString();
                        }
                        else
                        {
                            tmp += "|" + id.ToString();
                        }

                        cnt++;
                    }

                }

                str += st.buzaiId.ToString() + "," + st.hinshu + "," + st.buzaiName + "," + st.DanName + "," + st.Kigou + "," + st.Kashosu + "," + st.boltName + "," + st.num + "," + (st.Kashosu * st.num).ToString() + "," + tmp + Environment.NewLine;
            }
            return str;
        }

        public string GetKoudaiBoltData()
        {
            //List<string> boltData = new List<string>();
            string str = "品種,記号,交点数,ボルト種類,箇所/個,ボルト数量" + Environment.NewLine;
            //str += "(A)対傾構 " + "," + "" + "," + "4" + "," + "S10T - M22X60" + "," + "6" + "," + "24" + Environment.NewLine;
            foreach (YamadomeKoudaiBoltSekisan.stBoltDataForBoltSheet st in koudaiBoltSheetDatalist)
            {
                str += st.hinshu + "," + st.Kigou + "," + st.Kashosu + "," + st.boltName + "," + st.num + "," + (st.Kashosu * st.num).ToString() + Environment.NewLine;
            }
            return str;
        }

        public string GetKoudaiBoltDataWithElementID()
        {
            string str = "部材ID,品種,記号,交点数,ボルト種類,箇所/個,ボルト数量,接合部材ID" + Environment.NewLine;
            foreach (YamadomeKoudaiBoltSekisan.stBoltDataForBoltSheet st in koudaiBoltSheetDatalistWithID)
            {
                int cnt = 0;
                string tmp = string.Empty;
                if (st.PenetrationIds != null)
                {
                    foreach (ElementId id in st.PenetrationIds)
                    {
                        if (cnt == 0)
                        {
                            tmp = id.ToString();
                        }
                        else
                        {
                            tmp += "|" + id.ToString();
                        }

                        cnt++;
                    }
                }


                str += st.buzaiId.ToString() + "," + st.hinshu + "," + st.Kigou + "," + st.Kashosu + "," + st.boltName + "," + st.num + "," + (st.Kashosu * st.num).ToString() + "," + tmp + Environment.NewLine;
            }
            return str;
        }

        /// <summary>
        /// 構台普通ボルトに付随するスプリングワッシャの個数
        /// </summary>
        /// <returns></returns>
        public string GetWasherWithBNData()
        {
            int num = 0;
            string str = "スプリングワッシャー個数" + Environment.NewLine;
            foreach (YamadomeKoudaiBoltSekisan.stBoltDataForBoltSheet st in koudaiBoltSheetDatalistWithID)
            {
                if (st.boltName.Contains(BN))
                {
                    num += (st.Kashosu * st.num);
                }
            }

            str += num.ToString() + Environment.NewLine;
            return str;
        }

        public string GetKarikouzaiToShuzaiData()
        {
            string str = "部材ID,段数,部材タイプ,部材サイズ,長さリスト" + Environment.NewLine;
            foreach (stShuzaiData st in shuzaiDataList)
            {
                string tmp = string.Empty;
                foreach (int n in st.lengthList)
                {
                    tmp += n.ToString() + "|";
                }

                str += st.id.ToString() + "," + st.DanName + "," + st.buzaiType + "," + st.buzaiSize + "," + tmp + Environment.NewLine;
            }
            return str;
        }

        public string GetCoverPlateData()
        {
            string str = "部材ID,段数,取付先,部材サイズ,個数" + Environment.NewLine;
            foreach (stCoverPlate st in coverPlateDataList)
            {
                str += st.karikouzaiId.ToString() + "," + st.DanName + "," + st.setugouBuzai + "," + st.buzaiSize + "," + st.num.ToString() + Environment.NewLine;
            }
            return str;
        }

        public string GetZaganeData()
        {
            string str = "部材ID,段数,取付部材,個数" + Environment.NewLine;
            foreach (stZaganeData st in zaganeDataList)
            {
                str += st.blockId.ToString() + "," + st.DanName + "," + st.target + "," + st.num.ToString() + Environment.NewLine;
            }
            return str;
        }

        public string GetBracketData()
        {
            string str = "部材ID,段数,用途,ブラケット名称,個数" + Environment.NewLine;
            foreach (stBracketData st in bracketDataList)
            {
                str += st.karikouzaiId.ToString() + "," + st.DanName + "," + st.youto + "," + st.name + "," + st.num.ToString() + Environment.NewLine;
            }
            return str;
        }

        /// <summary>
        /// 構台側の仮鋼材⇒主材 情報を出力する
        /// </summary>
        /// <returns></returns>
        public string GetkoudaiKarikouzaiToShuzaiData()
        {
            string str = "部材ID,段数,部材タイプ,部材サイズ,長さリスト" + Environment.NewLine;
            foreach (stShuzaiData st in koudaiShuzaiDataList)
            {
                string tmp = string.Empty;
                foreach (int n in st.lengthList)
                {
                    tmp += n.ToString() + "|";
                }

                str += st.id.ToString() + "," + st.DanName + "," + st.buzaiType + "," + st.buzaiSize + "," + tmp + Environment.NewLine;
            }
            return str;
        }

        public string GetJackData()
        {
            string str = "部材ID,鋼材サイズ,ジャッキのファミリ名" + Environment.NewLine;
            foreach (stJackData st in JackDataList)
            {
                string tmp = string.Empty;
                

                str += st.jackId.ToString() + "," + st.kouzaiSize + "," + st.jackFamilyName + Environment.NewLine;
            }
            return str; 
        }

        public string GetSyabariChainData()
        {
            string str = "部材名,積算根拠斜梁受けID,段,個数" + Environment.NewLine;
            foreach (stSybariChainData st in syabariChainDataList)
            {
                string tmp = string.Empty;
                str += st.buzaiName + "," + st.syabariukeId.ToString() + "," + st.DanName + "," + st.num + Environment.NewLine;
            }
            return str;
        }

        //syabariTokushuPieceDataList
        public string GetKaouhinYamadomeData()
        {
            //パラメータが完全一致したファミリを除く　#34043
            List<stkakouhinSheetYamadomeData> DupLst = DupStkakouhinSheetYamadomeData(kakouhinSheetYamadomeDataList);

            string str = "商品名称,鋼材タイプ,サイズ,長さ,数量,孔径,孔明け個数,単質(kg),質量(kg),備考" + Environment.NewLine;
            foreach (stkakouhinSheetYamadomeData st in DupLst)
            {
                str += st.oyadata.name + "," + st.oyadata.type + "," + st.oyadata.size + "," + st.oyadata.length.ToString() + "," + st.oyadata.num.ToString() + "," +
                    st.oyadata.koukei + ","+st.oyadata.kouakeKosuu + ","+ st.oyadata.unitWeight + "," + st.oyadata.weight + "," +""+ Environment.NewLine;

                foreach (stKakouhinData koSt in st.koDataList)
                {
                    str += koSt.name + "," + koSt.type + "," + koSt.size + "," + koSt.length.ToString() + "," + koSt.num.ToString() + "," +
                    koSt.koukei + "," + koSt.kouakeKosuu + "," + koSt.unitWeight + "," + koSt.weight + "," + "" + Environment.NewLine;
                }
                str+= "-" + "," + "" + "," + ""+ "," + "" + "," + st.num.ToString() + Environment.NewLine;

            }
            return str;
        }


        public string GetKaouhinKoudaiData()
        {
            //パラメータが完全一致したファミリを除く #34043
            List<stkakouhinSheetKoudaiData> DupList = DupStkakouhinSheetKoudaiData(kakouhinSheetKoudaiDataList);

            string str = "商品名称,鋼材タイプ,サイズ,長さ,数量,孔径,孔明け個数,単質(kg),質量(kg),備考" + Environment.NewLine;
            foreach (stkakouhinSheetKoudaiData st in DupList)
            {
                str += st.oyadata.name + "," + st.oyadata.type + "," + st.oyadata.size + "," + st.oyadata.length.ToString() + "," + st.oyadata.num.ToString() + "," +
                    st.oyadata.koukei + "," + st.oyadata.kouakeKosuu + "," + st.oyadata.unitWeight.ToString() + "," + st.oyadata.weight + "," + "" + Environment.NewLine;

                foreach (stKakouhinData koSt in st.koDataList)
                {
                    str += koSt.name + "," + koSt.type + "," + koSt.size + "," + koSt.length.ToString() + "," + koSt.num.ToString() + "," +
                    koSt.koukei + "," + koSt.kouakeKosuu + "," + koSt.unitWeight.ToString() + "," + koSt.weight + "," + "" + Environment.NewLine;
                }
                str += "-" + "," + "" + "," + "" + "," + "" + "," + st.num.ToString() + Environment.NewLine;

            }
            return str;
        }

        public string GetUragomeData()
        {
            string str = "段名,個数,腹起,段" + Environment.NewLine;
            foreach (KeyValuePair<string, int> st in uragomeDictionary)
            {
                string[] arr = st.Key.Split(',');
                if (arr.Length < 3)
                {
                    continue;
                }
                str += arr[0]  +"," + st.Value + "," + arr[1] + "," + arr[2] + Environment.NewLine;
            }
            return str;
        }

        public string GetUragomeDataForCornerPiece()
        {
            string str = "段名,個数,腹起,段" + Environment.NewLine;

            if (uragomeDictionaryForCornerPiece.Count > 0)
            {
                foreach (KeyValuePair<string, int> st in uragomeDictionaryForCornerPiece)
                {
                    string[] arr = st.Key.Split(',');
                    if (arr.Length < 2)
                    {
                        continue;
                    }
                    str += arr[0] + "," + st.Value + "," + arr[1] + "," + arr[2] + Environment.NewLine;
                }

            } else
            {
                //#32614
                foreach (KeyValuePair<string, int> st in uragomeDictionary)
                {
                    string[] arr = st.Key.Split(',');
                    if (arr.Length < 3)
                    {
                        continue;
                    }
                    str += arr[0] + "," + "0" + "," + arr[1] + "," + arr[2] + Environment.NewLine;
                }
            }

           
            return str;
        }

        public string GetHolelinerData()
        {
            //現状は0　exe側で拾わせてる
            string str = "段名,個数" + Environment.NewLine;
            foreach (KeyValuePair<string, int> st in holelinerDictionary)
            {
                str += st.Key + "," + st.Value + Environment.NewLine;
            }
            return str;
        }

        public string GetKabekuiSheetData(Document doc)
        {
            List<string> data1 = new List<string>();
            List<string> data2 = new List<string>();

            GetCsvLineKabe(doc, out data1);
            GetCsvLineKui(doc, out data2);


            string str = "分類,用途,鋼材タイプ,サイズ,CASE名,長さ,ピッチ,埋め殺し長さ,ジョイント数,数量,単位質量,ソイル径,壁長,壁延長,継手仕様,プレート1(F),枚数１,プレート2(F),枚数２,ボルトタイプ(F),ボルト本数(F),プレート(W),枚数,ボルトタイプ(W),ボルト本数(W),ボルト孔径,備考欄" + Environment.NewLine;
            foreach (string s in data1)
            {
                str += s + Environment.NewLine;
            }

            foreach (string s in data2)
            {
                str += s + Environment.NewLine;
            }

            return str;
        }

        /// <summary>
        /// 構台杭用
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public string GetKoudaiKabekuiSheetData(Document doc)
        {
            List<string> data = new List<string>();

            GetCsvLineKoudaiKui(doc, out data);

            string str = "分類,用途,鋼材タイプ,サイズ,CASE名,長さ,ピッチ,埋め殺し長さ,ジョイント数,数量,単位質量,ソイル径,壁長,壁延長,継手仕様,プレート1(F),枚数１,プレート2(F),枚数２,ボルトタイプ(F),ボルト本数(F),プレート(W),枚数,ボルトタイプ(W),ボルト本数(W),ボルト孔径,備考欄" + Environment.NewLine;
            foreach (string s in data)
            {
                str += s + Environment.NewLine;
            }
            return str;
        }

        public string GetAllLevelName(Document doc)
        {
            string str = "レベル名称" + Environment.NewLine;

            List<string> list = ClsYMSUtil.GetLevelNames(doc).ToList();

            foreach (string s in list)
            {
                str += s + "|";
            }

            str += Environment.NewLine;
            return str;
        }

        /// <summary>
        /// 部材にレベル情報が抜けているか確認し、抜けているレベル名を特定しElementIdとともに列挙する（ジャッキとジャッキカバーのみ対象）
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public  string WriteLevelNameFamily(Document doc)
        {
            string res = "部材ID,レベル名" + Environment.NewLine;

            //図面上からファミリインスタンス全収集
            List<ElementId> elements = ClsRevitUtil.GetAllCreatedFamilyInstanceList(doc);


            //各調査対象部材により分ける
            //カバープレート
            List<FamilyInstance> coverplateInstanceList = new List<FamilyInstance>();
            //隅部ピース
            List<FamilyInstance> sumibuPieceInstanceList = new List<FamilyInstance>();
            //火打受ピース
            List<FamilyInstance> hiuchiukePieceInstanceList = new List<FamilyInstance>();
            //自在火打受ピース
            List<FamilyInstance> jizaiHiuchiukePieceInstanceList = new List<FamilyInstance>();
            //ジャッキカバー
            List<FamilyInstance> jackCoverInstanceList = new List<FamilyInstance>();
            //ガセットプレート
            List<FamilyInstance> gussetPlateInstanceList = new List<FamilyInstance>();
            //小火打ブロック
            List<FamilyInstance> shohiuchiBlockInstanceList = new List<FamilyInstance>();
            //火打ブロック
            List<FamilyInstance> hiuchiBlockInstanceList = new List<FamilyInstance>();
            //補助ﾋﾟｰｽ
            List<FamilyInstance> hojoPieceInstanceList = new List<FamilyInstance>();
            //スチフナージャッキ
            List<FamilyInstance> stJackInstanceList = new List<FamilyInstance>();
            //ジャッキ
            List<FamilyInstance> jackInstanceList = new List<FamilyInstance>();
            //切替ピース
            List<FamilyInstance> kirikaePieceInstanceList = new List<FamilyInstance>();
            //三軸ピース
            List<FamilyInstance> sanjikuPieceInstanceList = new List<FamilyInstance>();


            foreach (ElementId id in elements)
            {
                FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                if (inst == null)
                {
                    continue;
                }

                string familyName = inst.Symbol.FamilyName;
                if (familyName.Contains("カバープレート"))
                {
                    coverplateInstanceList.Add(inst);
                }
                else if (familyName.Contains("高強度ｶﾊﾞｰﾌﾟﾚｰﾄ"))
                {
                    coverplateInstanceList.Add(inst);
                }
                else if (familyName.Contains("隅部ピース"))
                {
                    sumibuPieceInstanceList.Add(inst);
                }
                else if (familyName.Contains("自在火打ち受けピース"))
                {
                    jizaiHiuchiukePieceInstanceList.Add(inst);
                }
                else if (familyName.Contains("火打受ピース"))
                {
                    hiuchiukePieceInstanceList.Add(inst);
                }
                else if (familyName.Contains("火打受ﾋﾟｰｽ"))
                {
                    hiuchiukePieceInstanceList.Add(inst);
                }
                else if (familyName.Contains("ジャッキカバー"))
                {
                    jackCoverInstanceList.Add(inst);
                }
                else if (familyName.Contains("ガセットプレート"))
                {
                    gussetPlateInstanceList.Add(inst);
                }
                else if (familyName.Contains("小火打ブロック"))
                {
                    shohiuchiBlockInstanceList.Add(inst);
                }
                else if (familyName.Contains("火打ブロック"))
                {
                    hiuchiBlockInstanceList.Add(inst);
                }
                else if (familyName.Contains("補助ﾋﾟｰｽ"))
                {
                    hojoPieceInstanceList.Add(inst);
                }
                else if (familyName.Contains("スチフナージャッキ"))
                {
                    stJackInstanceList.Add(inst);
                }
                else if (familyName.Contains("ジャッキ"))
                {
                    jackInstanceList.Add(inst);
                }
                else if (familyName.Contains("ｼﾞｬｯｷ"))
                {
                    jackInstanceList.Add(inst);
                }
                else if (familyName.Contains("切替ピース"))
                {
                    kirikaePieceInstanceList.Add(inst);
                }
                else if (familyName.Contains("三軸ピース"))
                {
                    sanjikuPieceInstanceList.Add(inst);
                }
            }


            List<FamilyInstance> targetInsList = new List<FamilyInstance>();
            targetInsList.AddRange(jackCoverInstanceList);
            targetInsList.AddRange(jackInstanceList);

            //現状ジャッキとジャッキカバーのみを対象とする
            foreach (FamilyInstance fi in targetInsList) 
            {
                string name = string.Empty;
                name = ClsRevitUtil.GetInstMojiParameter(doc, fi.Id, "集計レベル");
                if (string.IsNullOrWhiteSpace(name))
                {
                    name = ClsRevitUtil.GetInstMojiParameter(doc, fi.Id, "基準レベル");
                }


                if (string.IsNullOrWhiteSpace(name))
                {
                    //レベル名が抜けているとみなす
                    
                    ElementId buzaiId = fi.Id;// ジャッキOrジャッキカバーのIDをセット
                    if (buzaiId == null)
                    {
                        continue;
                    }

                    //ジャッキorジャッキカバーに付与されているベースのIDを取得
                    ElementId baseId = ClsKariKouzai.GetBaseId(doc, buzaiId);
                    if(baseId == null)
                    {
                        continue;
                    }
                    //ベースからレベル名を取得
                    FamilyInstance baseInst = doc.GetElement(baseId) as FamilyInstance;
                    if (baseInst == null)
                    {
                        continue;
                    }
                    string levelName = baseInst.Host.Name;

                    if (string.IsNullOrWhiteSpace(levelName))
                    {
                        continue;
                    }
                    res += buzaiId.ToString() + "," + levelName + Environment.NewLine;
                }
                else
                {
                    //レベル名は設定されている
                    continue;
                }

            }

            return res;
        }


        /// <summary>
        /// ブロック（受けピース）の数を取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="uragomeDictionary">レベルID / 個数</param>
        /// <returns></returns>
        /// <remarks>腹起用</remarks>
        public static bool GetUragomeCount(UIDocument uidoc, Dictionary<string, int> uragomeDictionary)
        {
            Document doc = uidoc.Document;
            // 杭からの判定線
            double searchRange = ClsRevitUtil.ConvertDoubleGeo2Revit(1100.0);

            //// 壁芯を全て取得
            //List<ElementId> kabeshinIds = ClsKabeShin.GetAllKabeShinList(doc);

            // 腹起ベースを全て取得
            List<ElementId> haraokoshiBaseIds = ClsHaraokoshiBase.GetAllHaraokoshiBaseList(doc);

            try
            {
                // 壁芯から杭を取得
                List<ElementId> kuiIds = new List<ElementId>();// ClsKabeShin.GetKabeIdList(doc, kabeshinId);
                List<ElementId> kouyaitaIds = ClsKabeShin.GetKouyaitaKabeIdList(doc);
                List<ElementId> SMWIds = ClsKabeShin.GetSMWKabeIdList(doc);
                List<ElementId> oyakuiIds = ClsKabeShin.GetOyakuiKabeIdList(doc);
                List<ElementId> renzokukabeIds = ClsKabeShin.GetRenzokuKabeIdList(doc);

                kuiIds.AddRange(kouyaitaIds);
                kuiIds.AddRange(SMWIds);
                kuiIds.AddRange(oyakuiIds);
                kuiIds.AddRange(renzokukabeIds);

                List<ElementId> idss = new List<ElementId>();

                foreach (var kabeKuiId in kuiIds)
                {
                    // 杭の判定線を作成
                    FamilyInstance instkui = doc.GetElement(kabeKuiId) as FamilyInstance;
                    if (instkui == null)
                    {
                        continue;
                    }
                    string symName = instkui.Symbol.FamilyName;

                    XYZ pKuiSt = XYZ.Zero;
                    if (symName.Contains("杭"))
                    {
                        pKuiSt = instkui.GetTotalTransform().Origin;
                    }
                    else if (symName.Contains("鋼矢板"))
                    {
                        if (ClsKouyaita.GetVoidvec(doc, instkui.Id) == 1)
                        {
                            // 壁側方向 (凸)
                            continue;
                        }
                        XYZ centerPoint = XYZ.Zero;
                        pKuiSt = ClsBracket.GetInsertPoint_Kouyaita(doc, instkui, symName, ref centerPoint);
                    }
                    pKuiSt = new XYZ(pKuiSt.X, pKuiSt.Y, XYZ.Zero.Z);


                    List<string> hitHaraBaseLevelName = new List<string>();

                    foreach (var haraokoshiBaseId in haraokoshiBaseIds)
                    {
                        FamilyInstance instHaraokoshiBase = doc.GetElement(haraokoshiBaseId) as FamilyInstance;
                        if (instHaraokoshiBase == null)
                        {
                            continue;
                        }


                        Level lv = doc.GetElement(instHaraokoshiBase.Host.Id) as Level;
                        string levelName = lv.Name;

                        // 腹起ベースの判定線を作成
                       
                        Curve cvHaraokoshiBase = (instHaraokoshiBase.Location as LocationCurve).Curve;
                        XYZ pBaseSt = cvHaraokoshiBase.GetEndPoint(0);
                        pBaseSt = new XYZ(pBaseSt.X, pBaseSt.Y, XYZ.Zero.Z);
                        XYZ pBaseEd = cvHaraokoshiBase.GetEndPoint(1);
                        pBaseEd = new XYZ(pBaseEd.X, pBaseEd.Y, XYZ.Zero.Z);
                        cvHaraokoshiBase = Line.CreateBound(pBaseSt, pBaseEd);

                        XYZ pKuiEd = pKuiSt + (searchRange * instHaraokoshiBase.FacingOrientation.Normalize());
                        pKuiEd = new XYZ(pKuiEd.X, pKuiEd.Y, XYZ.Zero.Z);
                        Curve cvOyakui = Line.CreateBound(pKuiSt, pKuiEd);

                        // 線分同士の交差を判定して取得
                        List<Tuple<XYZ, FamilyInstance>> intersectionPoints = new List<Tuple<XYZ, FamilyInstance>>();
                        SetComparisonResult result = cvHaraokoshiBase.Intersect(cvOyakui, out IntersectionResultArray intersectionResults);

                        bool hit = false;

                        if (result == SetComparisonResult.Overlap)
                        {
                            foreach (IntersectionResult intersectionResult in intersectionResults)
                            {
                                intersectionPoints.Add(new Tuple<XYZ, FamilyInstance>(intersectionResult.XYZPoint, instkui));

                                //一つの杭が同レベル内の複数腹起ベースに従属するケースは想定しない。するとややこしいことになる。
                                if (!hitHaraBaseLevelName.Contains(levelName))
                                {
                                    hit = true;
                                    hitHaraBaseLevelName.Add(levelName);
                                }
                                //ここでブレークしないと一つの杭と一つの腹起ベースに複数交点があることを許容する羽目になる（仕様上ありえないかもしれないがコード上許容してはダメ）
                                break;
                            }
                        }
                        else
                        {
                            //#32615
                            //壁芯の両端等にあるため腹起には接しているのに腹起ベースと判定線が交差できないケースがある
                            //腹起ベースと判定線での交差ではなく判定線の終点と腹起ベースの最短距離を検索し、searchRange以下であれば交差と同等に見なす
                            double dist = cvHaraokoshiBase.Distance(pKuiEd);
                            if(ClsGeo.GEO_LE(dist, searchRange))
                            {
                                if (!hitHaraBaseLevelName.Contains(levelName))
                                {
                                    intersectionPoints.Add(new Tuple<XYZ, FamilyInstance>(pKuiEd, instkui));
                                    hit = true;
                                    hitHaraBaseLevelName.Add(levelName);
                                }
                            }
                        }

                        if (!hit)
                        {
                            continue;
                        }

                        int numV = ClsRevitUtil.GetInstMojiParameter(doc, haraokoshiBaseId, "縦本数") == "シングル" ? 1 : 2;
                        int kuiCount = intersectionPoints.Count() * numV;
                        string dan = "段違";
                        string danPrm = ClsRevitUtil.GetParameter(doc, haraokoshiBaseId, "段");
                        if (danPrm == "同段")
                        {
                            dan = "同段";
                        }

                        // 裏込めの個数を算出(YSSより式を移植)
                        int uragomeCount = (int)(((kuiCount + 0.5) / 10) * 10);

                        // キーが存在すれば加算、存在しなければ追加
                        //Level lv = doc.GetElement(instHaraokoshiBase.Host.Id) as Level;
                        //string levelName = lv.Name;
                        string haraSize = ClsRevitUtil.GetParameter(doc, haraokoshiBaseId, "鋼材サイズ");
                        if (uragomeDictionary.ContainsKey(levelName + "," + haraSize + "," + dan))
                        {
                            uragomeDictionary[levelName + "," + haraSize + "," + dan] += uragomeCount;
                        }
                        else
                        {
                            uragomeDictionary.Add(levelName + "," + haraSize + "," + dan, uragomeCount);
                        }
                    }
                }


            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// ブロック（受けピース）の数を取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="uragomeDictionary">レベルID / 個数</param>
        /// <returns></returns>
        /// <remarks>コーナーピース(隅部ピース)用</remarks>
        public static bool GetUragomeCountForCornerPiece(Document doc, Dictionary<string, int> uragomeDictionary)
        {
            // コーナーピースを取得
            List<ElementId> cornerPiecePieceIds = ClsSumibuPiece.GetAllSumibuPiece(doc);

            try
            {
                foreach (var cornerPieceId in cornerPiecePieceIds)
                {
                    // キーが存在すれば加算、存在しなければ追加
                    string levelName = ClsRevitUtil.GetInstMojiParameter(doc, cornerPieceId, "集計レベル");
                    FamilyInstance inst = doc.GetElement(cornerPieceId) as FamilyInstance;
                    FamilySymbol sym = inst.Symbol;
                    string kigou = ClsRevitUtil.GetTypeParameterString(sym, "記号");

                    //#32117
                    string dan = "段違";
                    string typeName = inst.Symbol.Name;
                    if (typeName == sumibuH || typeName == sumibuHR)
                    {
                        dan = "同段";
                    }
                    
                    if (levelName == null || levelName == "")
                    {
                        levelName = ClsRevitUtil.GetInstMojiParameter(doc, cornerPieceId, "作業面");
                        levelName = levelName != null ? levelName.Replace("レベル : ", "") : null;
                    }

                    string haraSize = YMS.Master.ClsSumibuPieceCsv.GetShuzaiSize(kigou);

                    if (uragomeDictionary.ContainsKey(levelName + "," + haraSize + "," + dan))
                    {
                        uragomeDictionary[levelName + "," + haraSize + "," + dan]++;
                    }
                    else
                    {
                        uragomeDictionary.Add(levelName + "," + haraSize + "," + dan, 1);
                    }

                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// ライナー（打込みピース）の数を取得する
        /// </summary>
        /// <param name="type"></param>
        /// <param name="uragomeCount"></param>
        /// <returns></returns>
        public static int GetTotalLinerCount(UragomeType type, int uragomeCount)
        {
            switch (type)
            {
                case UragomeType.HS:
                case UragomeType.ShortBlock:
                    // 本体の個数
                    return uragomeCount;
                case UragomeType.RotaryBlock:
                case UragomeType.UniBlock:
                    // 本体の個数×2
                    return uragomeCount * 2;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// 穴明ライナー（ライナー）の数を取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="uragomeDictionary"></param>
        /// <returns></returns>
        public static bool GetHolelinerCount(Document doc, Dictionary<string, int> uragomeDictionary)
        {
            // 腹起ベースを全て取得
            List<ElementId> haraokoshiBaseIds = ClsHaraokoshiBase.GetAllHaraokoshiBaseList(doc);

            try
            {
                foreach (var haraokoshiBaseId in haraokoshiBaseIds)
                {
                    // 腹起ベースに配置されている部材を取得する
                    FamilyInstance instHaraokoshiBase = doc.GetElement(haraokoshiBaseId) as FamilyInstance;
                    Curve cvHaraokoshiBase = (instHaraokoshiBase.Location as LocationCurve).Curve;
                    XYZ pBaseSt = cvHaraokoshiBase.GetEndPoint(0);
                    XYZ pBaseEd = cvHaraokoshiBase.GetEndPoint(1);

                    List<ElementId> onBaseLineParts = ClsYMSUtil.GetObjectOnBaseLine2(doc, pBaseSt, pBaseEd);

                    int coverPlateCount = 0;
                    int hiuchiBlockCount = 0;

                    foreach (var partsId in onBaseLineParts)
                    {
                        FamilyInstance instParts = doc.GetElement(partsId) as FamilyInstance;
                        if (instParts == null)
                        {
                            continue;
                        }

                        if (instParts.Symbol.Name == "腹起")
                        {
                            if (instParts.Symbol.Family.Name.Contains("カバープレート"))
                            {
                                coverPlateCount++;
                            }
                        }

                        if (instParts.Symbol.Name.Contains("火打ブロック"))
                        {
                            if (instParts.Symbol.Family.Name.Contains("火打ブロック"))
                            {
                                hiuchiBlockCount++;
                            }
                        }
                    }

                    // 穴明ライナー個数の計算
                    // ((腹起ｶﾊﾞｰﾌﾟﾚｰﾄ数-火打ﾌﾞﾛｯｸ数) / 2) + 火打ﾌﾞﾛｯｸ数量
                    int holeLinerCount = (coverPlateCount - hiuchiBlockCount) / 2 + hiuchiBlockCount;

                    // キーが存在すれば加算、存在しなければ追加
                    Level lv = doc.GetElement(instHaraokoshiBase.Host.Id) as Level;
                    string levelName = lv.Name;
                    if (uragomeDictionary.ContainsKey(levelName))
                    {
                        uragomeDictionary[levelName] += holeLinerCount;
                    }
                    else
                    {
                        uragomeDictionary.Add(levelName, holeLinerCount);
                    }

                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
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

            List<ElementId> allSoilList = ClsSMW.GetAllSoilList(doc);
            List<ElementId> allDesumibuList = ClsDesumibuHokyouzai.GetAllDesumibuhokyouzai(doc);

            try
            {

                List<KeyValuePair<string, double>> soilData = new List<KeyValuePair<string, double>>();
                //ソイルデータを使って事前にSMWの壁延長情報を成型する #31729
                foreach (ElementId id in allSoilList)
                {
                    FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                    if (inst == null)
                    {
                        continue;
                    }
                    if (inst.Symbol == null)
                    {
                        continue;
                    }
                    FamilySymbol sym = inst.Symbol;
                    if (sym == null)
                    {
                        continue;
                    }

                    string caseEdaban = ClsRevitUtil.GetTypeParameterString(sym, "CASE") + "-" + ClsRevitUtil.GetTypeParameterString(sym, "枝番");
                    double pitch = ClsCommonUtils.ChangeStrToDbl(ClsRevitUtil.GetTypeParameterString(sym, "ピッチ"));

                    KeyValuePair<string, double> tmpKV = new KeyValuePair<string, double>(caseEdaban, pitch);
                    soilData.Add(tmpKV);
                }


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
                        kabeData.Size = YMS.Master.ClsHBeamCsv.GetSizePileFamilyPathInName(sym.FamilyName); //ClsRevitUtil.GetTypeParameterString(sym, "サイズ");
                    }
                    else
                    {
                        kabeData.SteelType = ClsRevitUtil.GetTypeParameterString(sym, "小分類");
                        kabeData.Size = YMS.Master.ClsKouyaitaCsv.GetSizePileFamilyPathInName(sym.FamilyName); //ClsRevitUtil.GetTypeParameterString(sym, "サイズ");
                    }
                    kabeData.CaseName = ClsRevitUtil.GetTypeParameterString(sym, "CASE") + "-" + ClsRevitUtil.GetTypeParameterString(sym, "枝番");


                    // すでに同じ CaseName と Usage の組み合わせが存在する場合、Quantity を加算する
                    var existingData = kabeDataList.FirstOrDefault(data => data.CaseName == kabeData.CaseName && data.Usage == kabeData.Usage 
                    && data.SteelType == kabeData.SteelType && data.Size == kabeData.Size);
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
                        kabeData.WallLength = clsSMW.m_soilLen; //##32434

                    }
                    else
                    {
                        kabeData.WallLength = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(sym, "長さ"));

                    }

                    if (kabeData.WallLength != 0)
                    {
                        kabeData.WallLength = kabeData.WallLength / 1000;
                    }

                    if (kabeData.Usage == ClsSMW.SMW)
                    {
                        //#31729
                        foreach (KeyValuePair<string, double> kv in soilData)
                        {
                            //本来はこうしたいが、現状ソイルにピッチ情報はついていない(Valueが全部0)
                            //if (kv.Key == kabeData.CaseName)
                            //{
                            //    kabeData.WallExtension += kv.Value;
                            //}

                            //なので杭の持つピッチを同じCASE枝番を持つソイルの数だけ足していく
                            if (kv.Key == kabeData.CaseName)
                            {
                                kabeData.WallExtension += kabeData.Pitch;
                            }
                        }
                    }
                    else 
                    {
                        kabeData.WallExtension = 0;
                    }

                    if (kabeData.WallExtension != 0)
                    {
                        kabeData.WallExtension = kabeData.WallExtension / 1000;
                    }
                        
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
                        kabeData.BoltDiameter = 24.5;
                    }
                    else
                    {
                        YMS.Master.ClsKouyaitaTsugiteCsv pileCsv = YMS.Master.ClsKouyaitaTsugiteCsv.GetCls(kabeData.JointSpecification, kabeData.Size);
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
                        kabeData.BoltDiameter = 24.5;
                    }

                    kabeData.Remarks = "";

                    kabeDataList.Add(kabeData);
                }

                //#32622 出隅部補強材のリスト
                foreach (ElementId id in allDesumibuList)
                {
                    KabeKuiData kabeData = new KabeKuiData();
                    FamilyInstance inst = doc.GetElement(id) as FamilyInstance;

                    if (inst == null)
                    {
                        continue;
                    }
                    if (inst.Symbol == null)
                    {
                        continue;
                    }
                    FamilySymbol sym = inst.Symbol;
                    if (sym == null)
                    {
                        continue;
                    }


                    kabeData.Classification = ClsRevitUtil.GetTypeParameterString(sym, "大分類");
                    //kabeData.Usage = ClsRevitUtil.GetTypeParameterString(sym, "中分類");
                    kabeData.Usage = ClsRevitUtil.GetTypeParameterString(sym, "小分類");
                    kabeData.SteelType = "出隅ﾌﾗｯﾄﾊﾞｰ";
                    kabeData.Size = ClsRevitUtil.GetTypeParameterString(sym, "サイズ");
                    kabeData.CaseName = "";

                    double d1 = ClsRevitUtil.GetTypeParameter(sym, "長さ1");
                    double d2 = ClsRevitUtil.GetTypeParameter(sym, "長さ2");
                    double d3 = ClsRevitUtil.GetTypeParameter(sym, "隅切長さ");
                    double len = d1 + d2 + d3;
                    kabeData.Length = ClsRevitUtil.CovertFromAPI(len);

                    // すでに同じ Size と Usage の組み合わせが存在する場合、Quantity を加算する
                    var existingData = kabeDataList.FirstOrDefault(data => data.Length == kabeData.Length && data.Usage == kabeData.Usage
                    && data.SteelType == kabeData.SteelType && data.Size == kabeData.Size);
                    if (existingData.SteelType == "出隅ﾌﾗｯﾄﾊﾞｰ") 
                    {
                        int num = kabeDataList.IndexOf(existingData);
                        existingData.Quantity += 1;
                        kabeDataList[num] = existingData;
                        continue;
                    }

                    kabeData.Pitch = 0;
                    kabeData.EmbedmentLength = 0;

                    kabeData.JointCount = 0;
                    kabeData.Quantity = 1;
                    kabeData.UnitWeight = ClsRevitUtil.GetTypeParameterAsValueString(sym, "質量");
                    kabeData.SoilDiameter = 0;
                    kabeData.WallLength = 0;
                    kabeData.WallExtension = 0;

                    // 継手情報
                    kabeData.JointSpecification = "";

                    kabeData.Plate1 = "";
                    kabeData.Plate1Count = 0;
                    kabeData.Plate2 = "";
                    kabeData.Plate2Count = 0;
                    kabeData.BoltTypeF = "";
                    kabeData.BoltCountF = 0;
                    kabeData.PlateW = "";
                    kabeData.PlateCountW = 0;
                    kabeData.BoltTypeW = "";
                    kabeData.BoltCountW = 0;
                    kabeData.BoltDiameter = 0;

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
            List<ElementId> koudaiKuiList = new List<ElementId>();
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
                    tanakuiData.Size = YMS.Master.ClsHBeamCsv.GetSizePileFamilyPathInName(instTanakui.Symbol.FamilyName); //ClsRevitUtil.GetTypeParameterString(instTanakui.Symbol, "サイズ");
                    tanakuiData.CaseName = instTanakui.Symbol.Name;
                    tanakuiData.Length = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(instTanakui.Symbol, "長さ"));
                    tanakuiData.Pitch = ClsRevitUtil.GetTypeParameter(instTanakui.Symbol, "ピッチ");
                    tanakuiData.EmbedmentLength = 0;
                    tanakuiData.JointCount = ClsRevitUtil.GetTypeParameterInt(instTanakui.Symbol, "ジョイント数");
                    tanakuiData.Quantity = 1;
                    tanakuiData.UnitWeight = ClsRevitUtil.GetTypeParameterAsValueString(instTanakui.Symbol, "単位質量");
                    tanakuiData.SoilDiameter = 0;
                    tanakuiData.WallLength = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(instTanakui.Symbol, "長さ")); ;
                    if (tanakuiData.WallLength != 0)
                    {
                        tanakuiData.WallLength = tanakuiData.WallLength / 1000;
                    }
                    tanakuiData.WallExtension = 0;

                    //mod 構台用の支持杭だったらスルー
                    if(instTanakui.Symbol.Name.Contains("支持杭"))
                    {
                        MaterialSuper ms = MaterialSuper.ReadFromElement(tanakuiId, doc);
                        if(ms!=null)
                        {
                            koudaiKuiList.Add(tanakuiId);
                            continue;
                        }
                    }

                    // 継手情報
                    tanakuiData.JointSpecification = ClsYMSUtil.GetKotei(doc, instTanakui.Id);
                    if (tanakuiData.JointSpecification == null)
                    {
                        string kotei= ClsRevitUtil.GetTypeParameterAsValueString(instTanakui.Symbol, DefineUtil.PARAM_JOINT_TYPE);
                        tanakuiData.JointSpecification = kotei != "" ? kotei.Contains("ﾎﾞﾙﾄ") ? "ボルト" : "溶接" : "";
                    }
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
                    var existingData = tanakuiDataList.FirstOrDefault(data => data.CaseName == tanakuiData.CaseName && data.Size == tanakuiData.Size && data.SteelType == tanakuiData.SteelType);
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

                //del 2024/05/27　別セクションで出力するように変更
                //Add 構台側の杭情報出力-----------------------------------------------------------------------------------------------------------------------------------------------------
                //koudaiKuiList.AddRange(ReadFromGantryProducts.CollectPilePiller(doc,PType.ExPile)); //継足し杭
                //koudaiKuiList.AddRange(ReadFromGantryProducts.CollectPilePiller(doc, PType.Piller)); //支柱
                //List<KabeKuiData> koudaikuiDataList = new List<KabeKuiData>();
                //foreach (var kuiId in koudaiKuiList)
                //{
                //    FamilyInstance instTanakui = doc.GetElement(kuiId) as FamilyInstance;
                //    MaterialSuper ms = MaterialSuper.ReadFromElement(kuiId, doc);

                //    KabeKuiData tanakuiData = new KabeKuiData();
                //    tanakuiData.Classification = System.Text.RegularExpressions.Regex.Replace(instTanakui.Symbol.Name, @"_\d+$", "");
                //    tanakuiData.Usage = System.Text.RegularExpressions.Regex.Replace(instTanakui.Symbol.Name, @"_\d+$", "");
                //    tanakuiData.SteelType = ClsRevitUtil.GetTypeParameterString(instTanakui.Symbol, "名称");
                //    tanakuiData.Size =ClsRevitUtil.GetTypeParameterString(instTanakui.Symbol, "サイズ").Replace("x","X");
                //    if(instTanakui.Symbol.FamilyName.Contains("HA")|| instTanakui.Symbol.FamilyName.Contains("SMH"))
                //    {
                //        tanakuiData.Size= ClsRevitUtil.GetTypeParameterString(instTanakui.Symbol, "記号");
                //    }
                //    tanakuiData.CaseName = instTanakui.Symbol.Name;
                //    tanakuiData.Length = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(instTanakui.Symbol, "長さ"));
                //    tanakuiData.Pitch = ClsRevitUtil.GetTypeParameter(instTanakui.Symbol, "ピッチ");
                //    tanakuiData.EmbedmentLength = 0;
                //    tanakuiData.JointCount = ClsRevitUtil.GetTypeParameterInt(instTanakui.Symbol, "ジョイント数");
                //    tanakuiData.Quantity = 1;
                //    tanakuiData.UnitWeight = ClsRevitUtil.GetTypeParameterAsValueString(instTanakui.Symbol, "単位質量");
                //    tanakuiData.SoilDiameter = 0;
                //    tanakuiData.WallLength = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(instTanakui.Symbol, "長さ")); ;
                //    if (tanakuiData.WallLength != 0)
                //    {
                //        tanakuiData.WallLength = tanakuiData.WallLength / 1000;
                //    }
                //    tanakuiData.WallExtension = 0;

                //    // 継手情報
                //    tanakuiData.JointSpecification = ClsYMSUtil.GetKotei(doc, instTanakui.Id);
                //    if (tanakuiData.JointSpecification == null)
                //    {
                //        string kotei = ClsRevitUtil.GetTypeParameterAsValueString(instTanakui.Symbol, DefineUtil.PARAM_JOINT_TYPE);
                //        tanakuiData.JointSpecification = kotei != "" ? kotei.Contains("ﾎﾞﾙﾄ") ? "ボルト" : "溶接" : "";
                //    }
                //    YMS.Master.ClsPileTsugiteCsv pileCsv = YMS.Master.ClsPileTsugiteCsv.GetCls(tanakuiData.JointSpecification, tanakuiData.Size);
                //    //TODO ジョイント数x数量にする必要がある場合は下記のコメントアウトを解除で有効
                //    tanakuiData.Plate1 = pileCsv.PlateSizeFOut;
                //    tanakuiData.Plate1Count = /*tanakuiData.JointCount**/pileCsv.PlateNumFOut;
                //    tanakuiData.Plate2 = pileCsv.PlateSizeFIn;
                //    tanakuiData.Plate2Count =/*tanakuiData.JointCount**/ pileCsv.PlateNumFIn;
                //    tanakuiData.BoltTypeF = /*tanakuiData.JointCount**/ (ms != null ? ms.m_BoltInfo2 : pileCsv.BoltSizeF);
                //    tanakuiData.BoltCountF =/*tanakuiData.JointCount**/(ms != null ? ms.m_Bolt2Cnt : pileCsv.BoltNumF );
                //    tanakuiData.PlateW = pileCsv.PlateSizeW;
                //    tanakuiData.PlateCountW =/*tanakuiData.JointCount**/ pileCsv.PlateNumW;
                //    tanakuiData.BoltTypeW =  ms != null ? ms.m_BoltInfo1 : pileCsv.BoltSizeW;
                //    tanakuiData.BoltCountW =/*tanakuiData.JointCount**/( ms != null ? ms.m_Bolt1Cnt : pileCsv.BoltNumW);
                //    tanakuiData.BoltDiameter = 0.0;
                //    tanakuiData.Remarks = "";

                //    // すでに同じタイプ名とサイズ、長さの組み合わせが存在する場合、Quantity を加算する
                //    var existingData = koudaikuiDataList.FirstOrDefault(data => data.Classification == tanakuiData.Classification && data.Size == tanakuiData.Size && data.Length == tanakuiData.Length&&data.JointCount==tanakuiData.JointCount);
                //    if (existingData.CaseName != null)
                //    {
                //        int num = koudaikuiDataList.IndexOf(existingData);
                //        existingData.Quantity += 1;
                //        koudaikuiDataList[num] = existingData;
                //    }
                //    else
                //    {
                //        koudaikuiDataList.Add(tanakuiData);
                //    }
                //}

                //// カンマ区切りの1行に変換してリストに追加
                //for (int i = 0; i < koudaikuiDataList.Count(); i++)
                //{
                //    var data = koudaikuiDataList[i];
                //    data.CaseName = "";
                //    string csvLine = ConvertDataToCsvLine(data);
                //    kuiList.Add(csvLine);
                //}
                //-----------------------------------------------------------------------------------------------------------------------------------------------------
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 構台杭用データリスト作成
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="kuiList"></param>
        /// <returns></returns>
        public static bool GetCsvLineKoudaiKui(Document doc, out List<string> kuiList)
        {
            kuiList = new List<string>();
            List<ElementId> tanakuiIds = new List<ElementId>();
            tanakuiIds.AddRange(ClsSanbashiKui.GetAllKuiList(doc).ToList());
            List<ElementId> koudaiKuiList = new List<ElementId>();
            try
            {
                // 新しいデータを格納するためのリスト
                List<KabeKuiData> tanakuiDataList = new List<KabeKuiData>();

                foreach (var tanakuiId in tanakuiIds)
                {
                    FamilyInstance instTanakui = doc.GetElement(tanakuiId) as FamilyInstance;
                    //mod 構台用の支持杭だったら格納
                    if (instTanakui.Symbol.Name.Contains("支持杭"))
                    {
                        MaterialSuper ms = MaterialSuper.ReadFromElement(tanakuiId, doc);
                        if (ms != null)
                        {
                            koudaiKuiList.Add(tanakuiId);
                            continue;
                        }
                    }
                }

                //Add 構台側の杭情報出力-----------------------------------------------------------------------------------------------------------------------------------------------------
                koudaiKuiList.AddRange(ReadFromGantryProducts.CollectPilePiller(doc, PType.ExPile)); //継足し杭
                //20240710  del 支柱は構台側の出力含まない
                //koudaiKuiList.AddRange(ReadFromGantryProducts.CollectPilePiller(doc, PType.Piller)); //支柱
                List<KabeKuiData> koudaikuiDataList = new List<KabeKuiData>();
                foreach (var kuiId in koudaiKuiList)
                {
                    FamilyInstance instTanakui = doc.GetElement(kuiId) as FamilyInstance;
                    MaterialSuper ms = MaterialSuper.ReadFromElement(kuiId, doc);

                    KabeKuiData tanakuiData = new KabeKuiData();
                    tanakuiData.Classification = System.Text.RegularExpressions.Regex.Replace(instTanakui.Symbol.Name, @"_\d+$", "");
                    tanakuiData.Usage = System.Text.RegularExpressions.Regex.Replace(instTanakui.Symbol.Name, @"_\d+$", "");
                    tanakuiData.SteelType = ClsRevitUtil.GetTypeParameterString(instTanakui.Symbol, "名称");
                    tanakuiData.Size = ClsRevitUtil.GetTypeParameterString(instTanakui.Symbol, "サイズ").Replace("x", "X");
                    if (instTanakui.Symbol.FamilyName.Contains("HA") || instTanakui.Symbol.FamilyName.Contains("SMH"))
                    {
                        tanakuiData.Size = ClsRevitUtil.GetTypeParameterString(instTanakui.Symbol, "記号");
                    }
                    tanakuiData.CaseName = instTanakui.Symbol.Name;
                    tanakuiData.Length = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(instTanakui.Symbol, "長さ"));
                    tanakuiData.Pitch = ClsRevitUtil.GetTypeParameter(instTanakui.Symbol, "ピッチ");
                    tanakuiData.EmbedmentLength = 0;
                    tanakuiData.JointCount = ClsRevitUtil.GetTypeParameterInt(instTanakui.Symbol, "ジョイント数");
                    tanakuiData.Quantity = 1;
                    tanakuiData.UnitWeight = ClsRevitUtil.GetTypeParameterAsValueString(instTanakui.Symbol, "単位質量");
                    tanakuiData.SoilDiameter = 0;
                    tanakuiData.WallLength = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(instTanakui.Symbol, "長さ")); ;
                    if (tanakuiData.WallLength != 0)
                    {
                        tanakuiData.WallLength = tanakuiData.WallLength / 1000;
                    }
                    tanakuiData.WallExtension = 0;

                    // 継手情報
                    tanakuiData.JointSpecification = ClsYMSUtil.GetKotei(doc, instTanakui.Id);
                    if (tanakuiData.JointSpecification == null)
                    {
                        string kotei = ClsRevitUtil.GetTypeParameterAsValueString(instTanakui.Symbol, DefineUtil.PARAM_JOINT_TYPE);
                        tanakuiData.JointSpecification = kotei != "" ? kotei.Contains("ﾎﾞﾙﾄ") ? "ボルト" : "溶接" : "";
                    }
                    YMS.Master.ClsPileTsugiteCsv pileCsv = YMS.Master.ClsPileTsugiteCsv.GetCls(tanakuiData.JointSpecification, tanakuiData.Size);
                    //TODO ジョイント数x数量にする必要がある場合は下記のコメントアウトを解除で有効
                    tanakuiData.Plate1 = pileCsv.PlateSizeFOut;
                    tanakuiData.Plate1Count = /*tanakuiData.JointCount**/pileCsv.PlateNumFOut;
                    tanakuiData.Plate2 = pileCsv.PlateSizeFIn;
                    tanakuiData.Plate2Count =/*tanakuiData.JointCount**/ pileCsv.PlateNumFIn;
                    tanakuiData.BoltTypeF = /*tanakuiData.JointCount**/ (ms != null ? ms.m_BoltInfo2 : pileCsv.BoltSizeF);
                    tanakuiData.BoltCountF =/*tanakuiData.JointCount**/(ms != null ? ms.m_Bolt2Cnt : pileCsv.BoltNumF);
                    tanakuiData.PlateW = pileCsv.PlateSizeW;
                    tanakuiData.PlateCountW =/*tanakuiData.JointCount**/ pileCsv.PlateNumW;
                    tanakuiData.BoltTypeW = ms != null ? ms.m_BoltInfo1 : pileCsv.BoltSizeW;
                    tanakuiData.BoltCountW =/*tanakuiData.JointCount**/(ms != null ? ms.m_Bolt1Cnt : pileCsv.BoltNumW);
                    tanakuiData.BoltDiameter = 0.0;
                    tanakuiData.Remarks = "";

                    // すでに同じタイプ名とサイズ、長さの組み合わせが存在する場合、Quantity を加算する
                    var existingData = koudaikuiDataList.FirstOrDefault(data => data.Classification == tanakuiData.Classification && data.Size == tanakuiData.Size && data.Length == tanakuiData.Length && data.JointCount == tanakuiData.JointCount);
                    if (existingData.CaseName != null)
                    {
                        int num = koudaikuiDataList.IndexOf(existingData);
                        existingData.Quantity += 1;
                        koudaikuiDataList[num] = existingData;
                    }
                    else
                    {
                        koudaikuiDataList.Add(tanakuiData);
                    }
                }

                // カンマ区切りの1行に変換してリストに追加
                for (int i = 0; i < koudaikuiDataList.Count(); i++)
                {
                    var data = koudaikuiDataList[i];
                    data.CaseName = "";
                    string csvLine = ConvertDataToCsvLine(data);
                    kuiList.Add(csvLine);
                }
                //-----------------------------------------------------------------------------------------------------------------------------------------------------
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






        public static List<List<string>> GetCASEPitch(Document doc)
        {
            List<List<string>> CASEPitchList = new List<List<string>>();

            List<ElementId> allKouyaitaList = ClsKouyaita.GetAllKouyaitaList(doc);
            List<ElementId> allOyakuiList = ClsOyakui.GetAllOyakuiList(doc);
            List<ElementId> allSMWList = ClsSMW.GetAllSMWList(doc);
            List<ElementId> allRenzokuKabeList = ClsRenzokukabe.GetAllRenzokuKabeList(doc);

            List<string> CASEList = new List<string>();
            foreach (ElementId id in allKouyaitaList)
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
            List<ElementId> kabeShinList = ClsKabeShin.GetAllKabeShinList(doc);

            double kouyaita = 0, oyakui = 0, smw = 0, renzokukabe = 0;

            foreach (ElementId id in kabeShinList)
            {
                string kabe = ClsRevitUtil.GetParameter(doc, id, "壁");
                FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                Curve cv = (inst.Location as LocationCurve).Curve;
                double length = ClsRevitUtil.CovertFromAPI(cv.Length);
                switch (kabe)
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

        /// <summary>
        /// インスタンスが構台側の仮鋼材かどうか判定する
        /// </summary>
        /// <param name="ins"></param>
        /// <returns></returns>
        private bool IsKoudaiKarikouzai(FamilyInstance ins)
        {
            bool retB = false;
            try
            {
                //桁材の仮鋼材
                if (ins.Symbol.FamilyName.Contains("桁材") && (ins.Symbol.FamilyName.Contains("HA") || ins.Symbol.FamilyName.Contains("SMH")))
                {
                    retB = true;
                }
                //柱の仮鋼材
                else if (ins.Symbol.FamilyName.Contains("柱") && (ins.Symbol.FamilyName.Contains("HA") || ins.Symbol.FamilyName.Contains("SMH")))
                {
                    if (!ins.Symbol.FamilyName.Contains("HA_") && !ins.Symbol.FamilyName.Contains("SMH_"))
                    {
                        retB = true;
                    }
                }
                return retB;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #region 構造体比較
        /// <summary>
        /// 加工品シート（山留）のデータ　重複確認
        /// </summary>
        /// <param name="lstSt">加工品シート（山留）の構造体リスト</param>
        /// <returns></returns>
        public static List<stkakouhinSheetYamadomeData> DupStkakouhinSheetYamadomeData(List<stkakouhinSheetYamadomeData> lstSt)
        {
            List<stkakouhinSheetYamadomeData> result = new List<stkakouhinSheetYamadomeData>();

            //番号を全て1にする
            for (int i = 0; i < lstSt.Count; i++)
            {
                stkakouhinSheetYamadomeData st = lstSt[i];
                st.num = 1;
                lstSt[i] = st;
            }

            foreach (stkakouhinSheetYamadomeData st in lstSt)
            {
                bool foundDuplicate = false;

                for( int i = 0; i < result.Count; i++)
                {
                    stkakouhinSheetYamadomeData existing = result[i];
                    if (EqualStruct(st.oyadata, existing.oyadata) &&
                        EqualStructList(st.koDataList, existing.koDataList))
                    {
                        existing.num++;
                        result[i] = existing;
                        foundDuplicate = true;
                        break;
                    }
                }

                if (!foundDuplicate)
                {
                    result.Add(st);
                }
            }

            return result;
        }

        /// <summary>
        /// 加工品シート（構台）のデータ　重複確認
        /// </summary>
        /// <param name="lstSt">加工品シート（構台）の構造体リスト</param>
        /// <returns></returns>
        public static List<stkakouhinSheetKoudaiData> DupStkakouhinSheetKoudaiData(List<stkakouhinSheetKoudaiData> lstSt)
        {
            List<stkakouhinSheetKoudaiData> result = new List<stkakouhinSheetKoudaiData>();

            //番号を全て1にする
            for (int i = 0; i < lstSt.Count; i++)
            {
                stkakouhinSheetKoudaiData st = lstSt[i];
                st.num = 1;
                lstSt[i] = st;
            }

            foreach (stkakouhinSheetKoudaiData st in lstSt)
            {
                bool foundDuplicate = false;

                for (int i = 0; i < result.Count; i++)
                {
                    stkakouhinSheetKoudaiData existing = result[i];
                    if (EqualStruct(st.oyadata, existing.oyadata) &&
                        EqualStructList(st.koDataList, existing.koDataList))
                    {
                        existing.num++;
                        result[i] = existing;
                        foundDuplicate = true;
                        break;
                    }
                }

                if (!foundDuplicate)
                {
                    result.Add(st);
                }
            }

            return result;
        }

        /// <summary>
        /// 構造体リストの比較
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listA"> 構造体リストA</param>
        /// <param name="listB"> 構造体リストB</param>
        /// <returns></returns>
        static bool EqualStructList<T>(List<T> listA, List<T> listB)
        {
            try
            {
                List<T> result = new List<T>();

                if (listA.Count != listB.Count)
                    return false;

                for (int i = 0; i < listA.Count; i++)
                {
                    if (!EqualStruct<T>(listA[i], listB[i]))
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }


            return true;
        }


        /// <summary>
        /// 構造体比較
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"> 構造体a</param>
        /// <param name="b"> 構造体b</param>
        /// <returns></returns>
        static bool EqualStruct<T>(T a, T b)
        {
            // 構造体の型を取得
            Type type = typeof(T);

            // プロパティ情報をすべて取得
            System.Reflection.FieldInfo[] properties = type.GetFields();

            foreach (System.Reflection.FieldInfo prop in properties)
            {
                // a と b の該当プロパティの値を取得
                object valueA = prop.GetValue(a);
                object valueB = prop.GetValue(b);

                if (valueA == null && valueB == null)
                    continue;

                if (valueA == null && valueB != null)
                    return false;

                if (valueA != null && valueB == null)
                    return false;

                if (!valueA.Equals(valueB))
                    return false;
            }
            return true;
        }
        #endregion

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
