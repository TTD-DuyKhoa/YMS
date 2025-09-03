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

namespace YMS_Schedule
{
    public class YamadomeKoudaiBoltSekisan
    {
        const string BoltCSV = "ボルト箇所マスタ.csv";
        const string F10T = "F10T-M22x";
        const string S10T = "S10T-";
        const string BN = "BN-";
        const string sumibuV = "V";
        const string sumibuH = "H";
        const string KOUZAI_FLANGE = "フランジ";
        const string KOUZAI_WEB = "ウェブ";
        const string KOUZAI_Plate = "プレート";

        public enum enBotType
        {
            BN,//普通
            F10T,//ハイテンション
            S10T,//トルシアボルト
        }

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

        public struct stMasterData
        {
            /// <summary>
            /// fixFlg (空白の場合は首下長さを動的に計算する)
            /// </summary>
            public string No0_FixFlg;

            /// <summary>
            /// 対象
            /// </summary>
            public string No1_Target;

            /// <summary>
            /// 対象詳細
            /// </summary>
            public string No2_TargetDetail;

            /// <summary>
            /// 厚さ(空白はゼロ)
            /// </summary>
            public double No3_Thick;

            /// <summary>
            /// 接合相手
            /// </summary>
            public string No4_JoiningPartner;

            /// <summary>
            /// 接合相手貫通箇所
            /// </summary>
            public string No5_PenetrationPoint;

            /// <summary>
            /// 接合相手貫通厚さ(空白はゼロ)
            /// </summary>
            public double No6_PenetrationThick;

            /// <summary>
            /// タイプ
            /// </summary>
            public string No7_Type;

            /// <summary>
            /// ボルト
            /// </summary>
            public string No8_Bolt;

            /// <summary>
            /// 本数1(空白はゼロ)
            /// </summary>
            public int No9_Number1;

            /// <summary>
            /// 本数2(空白はゼロ)
            /// </summary>
            public int No10_Number2;

            /// <summary>
            /// 複数ヒットの場合全てを対象とする（0：No、1：Yes 2:特殊ケース）
            /// </summary>
            public string No11_multiCount;

            /// <summary>
            /// 備考
            /// </summary>
            public string No12_Remarks;

            /// <summary>
            /// 追加ボルト
            /// </summary>
            public string No13_AddBolt;

            /// <summary>
            /// 追加ボルト本数
            /// </summary>
            public int No14_AddBoltNum;

        }

        /// <summary>
        /// 山留部材+山留主材のボルトデータを取得
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="idBuzaiList"></param>
        /// <param name="idShuzaiList"></param>
        /// <param name="yamadomeBoltSheetDatalist"></param>
        /// <param name="yamadomeBoltSheetDatalistWithID"></param>
        /// <returns></returns>
        public bool buzaiAndShuzaiBoltData(Document doc, List<ElementId> idBuzaiList, List<ElementId> idShuzaiList,
            ref List<stBoltDataForBoltSheet> yamadomeBoltSheetDatalist, ref List<stBoltDataForBoltSheet> yamadomeBoltSheetDatalistWithID)
        {
            try
            {
                //マスタ読み込み
                List<stMasterData> masterData = new List<stMasterData>();
                if (!GetBoltCsvData(ref masterData))
                {
                    return false;
                }

                List<FamilyInstance> instBuzaiList = YamadomeKoudaiBoltSekisan.ElementIdList2FamilyinstanceList(doc, idBuzaiList);

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
                List<ElementId> hojoPieceIdList = new List<ElementId>();
                //スチフナージャッキ
                List<FamilyInstance> stJackInstanceList = new List<FamilyInstance>();
                //ジャッキ
                List<FamilyInstance> jackInstanceList = new List<FamilyInstance>();
                //切替ピース
                List<FamilyInstance> kirikaePieceInstanceList = new List<FamilyInstance>();
                //三軸ピース
                List<FamilyInstance> sanjikuPieceInstanceList = new List<FamilyInstance>();
                //特殊ピース
                List<FamilyInstance> tokushuPieceInstanceList = new List<FamilyInstance>();
                //回転ピース
                List<FamilyInstance> kaitenPieceInstanceList = new List<FamilyInstance>();
                //斜梁受ピース
                List<FamilyInstance> shabariukePieceInstanceList = new List<FamilyInstance>();
                //切梁繋ぎ材ベース
                List<FamilyInstance> kiribariTsunagizaiBaseInstanceList = new List<FamilyInstance>();
                //火打繋ぎ材ベース
                List<FamilyInstance> hiuchiTsunagizaiBaseInstanceList = new List<FamilyInstance>();
                //頭つなぎ材
                List<FamilyInstance> atamaTsunagizaiInstanceList = new List<FamilyInstance>();
                //せん断ボルト補強材
                List<FamilyInstance> sendanBoltHokyozaiInstanceList = new List<FamilyInstance>();
                //腹起スベリ止メ
                List<FamilyInstance> haraokoshiSuberidomeInstanceList = new List<FamilyInstance>();
                //斜梁繋ぎ材ベース
                List<FamilyInstance> shabariTsunagizaiInstanceList = new List<FamilyInstance>();

                foreach (ElementId id in idBuzaiList)
                {
                    FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                    if (inst == null)
                    {
                        continue;
                    }

                    string familyName = inst.Symbol.FamilyName;
                    string typeName = inst.Symbol.Name;
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
                        hojoPieceIdList.Add(inst.Id);

                        //#31575 

                        if (familyName.Contains("補助ﾋﾟｰｽ_30D-2") && typeName.Contains("切梁"))
                        {
                            haraokoshiSuberidomeInstanceList.Add(inst);
                        }
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
                    else if (familyName.Contains("特殊ピース"))
                    {
                        tokushuPieceInstanceList.Add(inst);
                    }
                    else if (familyName.Contains("回転ピース"))
                    {
                        kaitenPieceInstanceList.Add(inst);
                    }
                    else if (familyName.Contains("斜梁受ピース"))
                    {
                        shabariukePieceInstanceList.Add(inst);
                    }
                    else if (familyName.Contains("切梁繋ぎ材ベース"))
                    {
                        kiribariTsunagizaiBaseInstanceList.Add(inst);
                    }
                    else if (familyName.Contains("火打繋ぎ材ベース"))
                    {
                        hiuchiTsunagizaiBaseInstanceList.Add(inst);
                    }
                    else if (typeName.Contains("頭ツナギ材"))
                    {
                        atamaTsunagizaiInstanceList.Add(inst);
                    }
                    else if (familyName.Contains("せん断ボルト補強材"))
                    {
                        sendanBoltHokyozaiInstanceList.Add(inst);
                    }
                    else if (familyName.Contains("腹起ｽﾍﾞﾘ止ﾒ"))
                    {
                        haraokoshiSuberidomeInstanceList.Add(inst);
                    }
                    else if (familyName.Contains("斜梁繋ぎ材ベース"))
                    {
                        shabariTsunagizaiInstanceList.Add(inst);
                    }
                }

                //①カバープレートの積算
                foreach (FamilyInstance fi in coverplateInstanceList)
                {
                    string familyName = fi.Symbol.FamilyName;
                    List<stMasterData> checkList = GetBoltEstimationList(familyName,masterData);
                   
                    foreach (stMasterData md in checkList)
                    {
                        //対象は主材
                        //接続してる主材のリスト

                        List<ElementId> targetIds = new List<ElementId>();

                        if (!string.IsNullOrWhiteSpace(md.No7_Type))
                        {
                            targetIds = GetSameLevelShuzaiList(doc, fi, idShuzaiList, md.No7_Type);
                        }
                        else
                        {
                            targetIds = GetSameLevelShuzaiList(doc, fi, idShuzaiList);
                        }
 
                        if (targetIds.Count < 0)
                        {
                            continue;
                        }

                        List<ElementId> Ids = ClsRevitUtil.GetIntersectFamilys(doc, fi.Id, 0.1, null, targetIds);
                        if (Ids.Count < 1)
                        {
                            continue;
                        }

                        string kigou = ClsRevitUtil.GetTypeParameterString(fi.Symbol, "記号");
                        kigou = kigou.TrimEnd('L');

                        SetBoltData(doc, md, fi, Ids[0], ref yamadomeBoltSheetDatalist, ref yamadomeBoltSheetDatalistWithID, outputKigou: kigou);
                        if (!string.IsNullOrWhiteSpace(md.No13_AddBolt))
                        {
                            SetBoltDataAddBolt(doc, md, fi, Ids[0], ref yamadomeBoltSheetDatalist, ref yamadomeBoltSheetDatalistWithID);
                        }

                        break;
                    }


                }
                //②隅部ピースの積算
                foreach (FamilyInstance fi in sumibuPieceInstanceList)
                {
                    string familyName = fi.Symbol.FamilyName;
                    List<stMasterData> checkList = GetBoltEstimationList(familyName, masterData);
                   
                    if (checkList.Count < 1)
                    {
                        continue;
                    }

                    foreach(stMasterData md in checkList)
                    {
                        if (!fi.Symbol.Name.Contains(md.No2_TargetDetail))
                        {
                            continue;
                        }
                        //対象は主材
                        //接続してる主材のリスト
                        List<ElementId> targetIds = GetSameLevelShuzaiList(doc, fi, idShuzaiList,md.No7_Type);
                        if (targetIds.Count < 0)
                        {
                            continue;
                        }
                        List<ElementId> Ids = ClsRevitUtil.GetIntersectFamilysLite(doc, fi.Id,null, targetIds);
                        //List<ElementId> Ids = ClsRevitUtil.GetIntersectFamilys(doc, fi.Id, 10, null, idShuzaiList);
                        if (Ids.Count < 1)
                        {
                            continue;
                        }
                        SetBoltData(doc, md, fi, Ids[0], ref yamadomeBoltSheetDatalist, ref yamadomeBoltSheetDatalistWithID,plusAtsumi:10);//YSSのDBと比較して首下が短くなるので10補正を入れておく　裏込めの分？
                        break;
                    }
                }
                //③火打ち受けピース積算
                foreach (FamilyInstance fi in hiuchiukePieceInstanceList)
                {
                    string familyName = fi.Symbol.FamilyName;
                    List<stMasterData> checkList = GetBoltEstimationList(familyName, masterData);

                    //if (fi.Id.ToString() == "498196")
                    //{
                    //    int gg = 0;
                    //}

                    //if (fi.Id.ToString() == "498203")
                    //{
                    //    int gg = 0;
                    //}

                    if (checkList.Count < 1)
                    {
                        continue;
                    }

                    int cnt = 0;
                    foreach (stMasterData md in checkList)
                    {
                       
                        //対象は主材
                        //接続してる主材のリスト
                        List<ElementId> targetIds = GetSameLevelShuzaiList(doc, fi, idShuzaiList, md.No7_Type);
                        if (targetIds.Count < 0)
                        {
                            continue;
                        }
                        List<ElementId> Ids = ClsRevitUtil.GetIntersectFamilysLite(doc, fi.Id, null, targetIds);
                        //List<ElementId> Ids = ClsRevitUtil.GetIntersectFamilys(doc, fi.Id, 10, null, idShuzaiList);
                        if (Ids.Count < 1)
                        {
                            continue;
                        }

                        string outputName = string.Empty;
                        if (familyName.Contains("VP-30"))
                        {
                            outputName = "30度火打受ピース";
                        }

                        SetBoltData(doc, md, fi, Ids[0], ref yamadomeBoltSheetDatalist, ref yamadomeBoltSheetDatalistWithID, outputName: outputName, plusAtsumi: 5);//YSSのDBと比較して首下が短くなるので5補正を入れておく
                        cnt++;
                        if (cnt == 1)
                        {
                            SetBoltDataAddBolt(doc, md, fi, Ids[0], ref yamadomeBoltSheetDatalist, ref yamadomeBoltSheetDatalistWithID, outputName: outputName);
                        }

                        if (cnt == 2)
                        {
                            //一つの火打受ピースが三カ所以上と接続することは無い
                            break;
                        }
                       
                    }
                }

                
                //④自在火打ち受けピース積算
                foreach (FamilyInstance fi in jizaiHiuchiukePieceInstanceList)
                {
                    string familyName = fi.Symbol.FamilyName;
                    List<stMasterData> checkList = GetBoltEstimationList(familyName, masterData);

                    if (checkList.Count < 1)
                    {
                        continue;
                    }

                    int cnt = 0;
                    foreach (stMasterData md in checkList)
                    {

                        //対象は主材
                        //接続してる主材のリスト
                        List<ElementId> targetIds = GetSameLevelShuzaiList(doc, fi, idShuzaiList, md.No7_Type);
                        if (targetIds.Count < 0)
                        {
                            continue;
                        }
                        List<ElementId> Ids = ClsRevitUtil.GetIntersectFamilysLite(doc, fi.Id, null, targetIds);
                        //List<ElementId> Ids = ClsRevitUtil.GetIntersectFamilys(doc, fi.Id, 10, null, idShuzaiList);
                        if (Ids.Count < 1)
                        {
                            continue;
                        }
                        SetBoltData(doc, md, fi, Ids[0], ref yamadomeBoltSheetDatalist, ref yamadomeBoltSheetDatalistWithID, plusAtsumi: 0);
                        cnt++;
                        if (cnt == 1)
                        {
                            SetBoltDataAddBolt(doc, md, fi, Ids[0], ref yamadomeBoltSheetDatalist, ref yamadomeBoltSheetDatalistWithID);
                        }
                        
                    }
                }

                //⑤ジャッキカバー積算
                foreach (FamilyInstance fi in jackCoverInstanceList)
                {
                    string familyName = fi.Symbol.FamilyName;
                    List<stMasterData> checkList = GetBoltEstimationList(familyName, masterData);
                    //対象は主材
                    //接続してる主材のリスト
                    List<ElementId> targetIds = GetSameLevelShuzaiList(doc, fi, idShuzaiList);
                    if (targetIds.Count < 0)
                    {
                        continue;
                    }
                    List<ElementId> Ids = ClsRevitUtil.GetIntersectFamilysLite(doc, fi.Id, null, targetIds);
                    if (Ids.Count < 1)
                    {
                        continue;
                    }

                    //マスタ行は一つのみ
                    if (checkList.Count < 1)
                    {
                        continue;
                    }
                    stMasterData hitData = checkList[0];
                    //ユニット単位で拾われてるので
                    //箇所数を倍にし、一箇所当たりの個数を半分にする
                    SetBoltDataForJackCover(doc, hitData, fi, Ids[0], ref yamadomeBoltSheetDatalist, ref yamadomeBoltSheetDatalistWithID);

                }

                //⑥ガセットプレート積算
                foreach (FamilyInstance fi in gussetPlateInstanceList)
                {
                    string familyName = fi.Symbol.FamilyName;
                    List<stMasterData> checkList = GetBoltEstimationList(familyName, masterData);
                    //マスタ行は一つのみ
                    if (checkList.Count < 1)
                    {
                        continue;
                    }
                    stMasterData hitData = checkList[0];
                    //対象は主材
                    //接続してる主材のリスト
                    List<ElementId> targetIds = GetSameLevelShuzaiList(doc, fi, idShuzaiList, hitData.No7_Type);
                    if (targetIds.Count < 0)
                    {
                        continue;
                    }
                    List<ElementId> Ids = ClsRevitUtil.GetIntersectFamilysLite(doc, fi.Id,  null, targetIds);
                    if (Ids.Count < 1)
                    {
                        continue;
                    }

                   
                    SetBoltData(doc, hitData, fi, Ids[0], ref yamadomeBoltSheetDatalist, ref yamadomeBoltSheetDatalistWithID);

                }

                //⑦小火打ちブロック積算
                foreach (FamilyInstance fi in shohiuchiBlockInstanceList)
                {
                    string familyName = fi.Symbol.FamilyName;
                    List<stMasterData> checkList = GetBoltEstimationList(familyName, masterData);

                    if (checkList.Count < 1)
                    {
                        continue;
                    }

                    foreach (stMasterData md in checkList)
                    {
                        //hojoPieceInstanceList
                        //接続してる補助ピース/主材のリスト
                        List<ElementId> targetHojoPieceInstanceListIds = GetSameLevelShuzaiList(doc, fi, hojoPieceIdList, md.No7_Type);
                        List<ElementId> targetIds = GetSameLevelShuzaiList(doc, fi, idShuzaiList, md.No7_Type);
                         
                        if (targetIds.Count < 1 && targetHojoPieceInstanceListIds.Count < 1)
                        {
                            continue;
                        }

                        List<ElementId> Ids = ClsRevitUtil.GetIntersectFamilysLite(doc, fi.Id, null, targetIds);
                        //List<ElementId> Ids = ClsRevitUtil.GetIntersectFamilys(doc, fi.Id, 10, null, idShuzaiList);
                        if (Ids.Count < 1)
                        {
                            Ids = ClsRevitUtil.GetIntersectFamilysLite(doc, fi.Id, null, targetHojoPieceInstanceListIds);
                        }

                        if (Ids.Count < 1)
                        {
                            continue;
                        }

                        SetBoltData(doc, md, fi, Ids[0], ref yamadomeBoltSheetDatalist, ref yamadomeBoltSheetDatalistWithID, plusAtsumi: 7);//ワッシャーの分
                       
                    }
                }

                //⑧火打ちブロック積算
                foreach (FamilyInstance fi in hiuchiBlockInstanceList)
                {
                    string familyName = fi.Symbol.FamilyName;
                    List<stMasterData> checkList = GetBoltEstimationList(familyName, masterData);

                    if (checkList.Count < 1)
                    {
                        continue;
                    }

                    foreach (stMasterData md in checkList)
                    {
                        //hojoPieceInstanceList
                        //接続してる補助ピース/主材のリスト
                        List<ElementId> targetHojoPieceInstanceListIds = GetSameLevelShuzaiList(doc, fi, hojoPieceIdList, md.No7_Type);
                        List<ElementId> targetIds = GetSameLevelShuzaiList(doc, fi, idShuzaiList, md.No7_Type);

                        if (targetIds.Count < 1 && targetHojoPieceInstanceListIds.Count < 1)
                        {
                            continue;
                        }

                        List<ElementId> Ids = ClsRevitUtil.GetIntersectFamilysLite(doc, fi.Id, null, targetIds);
                        //List<ElementId> Ids = ClsRevitUtil.GetIntersectFamilys(doc, fi.Id, 10, null, idShuzaiList);
                        if (Ids.Count < 1)
                        {
                            Ids = ClsRevitUtil.GetIntersectFamilysLite(doc, fi.Id, null, targetHojoPieceInstanceListIds);
                        }

                        if (Ids.Count < 1)
                        {
                            continue;
                        }

                        SetBoltData(doc, md, fi, Ids[0], ref yamadomeBoltSheetDatalist, ref yamadomeBoltSheetDatalistWithID, plusAtsumi: 7);//ワッシャーの分

                        ////対象は主材
                        ////接続してる主材のリスト
                        //List<ElementId> targetIds = GetSameLevelShuzaiList(doc, fi, idShuzaiList, md.No7_Type);
                        //if (targetIds.Count < 0)
                        //{
                        //    continue;
                        //}
                        //List<ElementId> Ids = ClsRevitUtil.GetIntersectFamilysLite(doc, fi.Id, null, targetIds);
                        ////List<ElementId> Ids = ClsRevitUtil.GetIntersectFamilys(doc, fi.Id, 10, null, idShuzaiList);
                        //if (Ids.Count < 1)
                        //{
                        //    continue;
                        //}
                        //SetBoltData(doc, md, fi, Ids[0], ref yamadomeBoltSheetDatalist, ref yamadomeBoltSheetDatalistWithID, plusAtsumi: 7);//ワッシャーの分
                    }
                }

                //⑨補助ピースの積算
                foreach (FamilyInstance fi in hojoPieceInstanceList)
                {
                    string familyName = fi.Symbol.FamilyName;
                    List<stMasterData> checkList = GetBoltEstimationList(familyName, masterData);

                    if (checkList.Count < 1)
                    {
                        continue;
                    }

                    foreach (stMasterData md in checkList)
                    {

                        //対象は主材
                        //接続してる主材のリスト
                        List<ElementId> targetIds = new List<ElementId>();
                        if (string.IsNullOrWhiteSpace(md.No7_Type))
                        {
                            SetBoltDataSingle(doc, md, fi, ref yamadomeBoltSheetDatalist, ref yamadomeBoltSheetDatalistWithID, outputName: "補助ピース");
                            break;
                        }
                        else
                        {
                            if (md.No7_Type == fi.Symbol.Name)
                            {
                                SetBoltDataSingle(doc, md, fi, ref yamadomeBoltSheetDatalist, ref yamadomeBoltSheetDatalistWithID, outputName: "補助ピース");
                                break;
                            }
                        }
                    }
                }

                //⑩スチフナージャッキ積算
                foreach (FamilyInstance fi in stJackInstanceList)
                {
                    string familyName = fi.Symbol.FamilyName;
                    List<stMasterData> checkList = GetBoltEstimationList(familyName, masterData);
                    //マスタ行は一つのみ
                    if (checkList.Count < 1)
                    {
                        continue;
                    }
                    stMasterData hitData = checkList[0];
                    //対象は主材
                    //接続してる主材のリスト
                    List<ElementId> targetIds = GetSameLevelShuzaiList(doc, fi, idShuzaiList);
                    if (targetIds.Count < 0)
                    {
                        continue;
                    }
                    List<ElementId> Ids = ClsRevitUtil.GetIntersectFamilysLite(doc, fi.Id, null, targetIds);
                    if (Ids.Count < 1)
                    {
                        continue;
                    }

                    SetBoltData(doc, hitData, fi, Ids[0], ref yamadomeBoltSheetDatalist, ref yamadomeBoltSheetDatalistWithID);

                }

                //⑪ジャッキ積算
                foreach (FamilyInstance fi in jackInstanceList)
                {
                    string familyName = fi.Symbol.FamilyName;
                    List<stMasterData> checkList = GetBoltEstimationList(familyName, masterData);
                    //マスタ行は一つのみ
                    if (checkList.Count < 1)
                    {
                        continue;
                    }
                    stMasterData hitData = checkList[0];
                    ////対象は主材
                    ////接続してる主材のリスト
                    //List<ElementId> targetIds = GetSameLevelShuzaiList(doc, fi, idShuzaiList);
                    //if (targetIds.Count < 0)
                    //{
                    //    continue;
                    //}
                    //List<ElementId> Ids = ClsRevitUtil.GetIntersectFamilysLite(doc, fi.Id, null, targetIds);
                    //if (Ids.Count < 1)
                    //{
                    //    continue;
                    //}

                    SetBoltDataSingle(doc, hitData, fi, ref yamadomeBoltSheetDatalist, ref yamadomeBoltSheetDatalistWithID);
                    //SetBoltData(doc, hitData, fi, Ids[0], ref yamadomeBoltSheetDatalist, ref yamadomeBoltSheetDatalistWithID);

                }

                //⑫切替ピース積算
                foreach (FamilyInstance fi in kirikaePieceInstanceList)
                {
                    string familyName = fi.Symbol.FamilyName;
                    List<stMasterData> checkList = GetBoltEstimationList(familyName, masterData);
                    //マスタ行は一つのみ
                    if (checkList.Count < 1)
                    {
                        continue;
                    }
                    stMasterData hitData = checkList[0];
                    ////対象は主材
                    ////接続してる主材のリスト
                    //List<ElementId> targetIds = GetSameLevelShuzaiList(doc, fi, idShuzaiList);
                    //if (targetIds.Count < 0)
                    //{
                    //    continue;
                    //}
                    //List<ElementId> Ids = ClsRevitUtil.GetIntersectFamilysLite(doc, fi.Id, null, targetIds);
                    //if (Ids.Count < 1)
                    //{
                    //    continue;
                    //}

                    SetBoltDataSingle(doc, hitData, fi,  ref yamadomeBoltSheetDatalist, ref yamadomeBoltSheetDatalistWithID);

                }

                //⑬三軸ピース積算
                foreach (FamilyInstance fi in sanjikuPieceInstanceList)
                {
                    string familyName = fi.Symbol.FamilyName;
                    List<stMasterData> checkList = GetBoltEstimationList(familyName, masterData);
                    //マスタ行は一つのみ
                    if (checkList.Count < 1)
                    {
                        continue;
                    }
                    stMasterData hitData = checkList[0];
                    ////対象は主材
                    ////接続してる主材のリスト
                    //List<ElementId> targetIds = GetSameLevelShuzaiList(doc, fi, idShuzaiList);
                    //if (targetIds.Count < 0)
                    //{
                    //    continue;
                    //}
                    //List<ElementId> Ids = ClsRevitUtil.GetIntersectFamilysLite(doc, fi.Id, null, targetIds);
                    //if (Ids.Count < 1)
                    //{
                    //    continue;
                    //}

                    SetBoltDataSingle(doc, hitData, fi, ref yamadomeBoltSheetDatalist, ref yamadomeBoltSheetDatalistWithID);

                }

                //⑭特殊ピース積算
                //foreach (FamilyInstance fi in tokushuPieceInstanceList)
                //{
                //    List<ElementId> NestedFamList = ClsRevitUtil.GetNestedFamilyInstanceIds(doc, fi.Id);
                //    int nnn = NestedFamList.Count;
                //}

                ////図面から全収集した腹起ベースのIDリスト
                //List<ElementId> allHaraokoshiIdList = ClsHaraokoshiBase.GetAllHaraokoshiBaseList(doc);
                //⑮回転ピース積算
                foreach (FamilyInstance fi in kaitenPieceInstanceList)
                {
                    string familyName = fi.Symbol.FamilyName;
                    List<stMasterData> checkList = GetBoltEstimationList(familyName, masterData);

                    foreach (stMasterData md in checkList)
					{
                        //対象は主材
                        //接続してる主材のリスト
                        List<ElementId> targetIds = new List<ElementId>();

                        if (!string.IsNullOrWhiteSpace(md.No7_Type))
                        {
                            targetIds = GetSameLevelShuzaiList(doc, fi, idShuzaiList, md.No7_Type);
                        }
                        else
                        {
                            targetIds = GetSameLevelShuzaiList(doc, fi, idShuzaiList);
                        }

                        if (targetIds.Count < 0)
                        {
                            continue;
                        }

                        List<ElementId> Ids = ClsRevitUtil.GetIntersectFamilysLiteExtensionZDirIs0pnt1(doc, fi.Id, null, targetIds);
                        if (Ids.Count < 1)
                        {
                            continue;
                        }
                        ElementId targetId = Ids[0];
                        FamilyInstance haraokoshiInst = doc.GetElement(targetId) as FamilyInstance;
                        if (haraokoshiInst == null)
                        {
                            continue;
                        }

                        //備考欄の値を「|」で区切って取得。List には「30HA」などが入る
                        List<string> kigouList = md.No12_Remarks.Split('|').ToList();
                        
                        //備考欄が空欄の場合
                        if(kigouList.Count == 1 && string.IsNullOrEmpty(kigouList[0]))
						{
                            SetBoltData(doc, md, fi, targetId, ref yamadomeBoltSheetDatalist, ref yamadomeBoltSheetDatalistWithID);
                            break;
                        }
                        else
						{
                            //接触している腹起主材のうち、最初ものの「記号」パラメータの判定
                            string kigou = ClsRevitUtil.GetTypeParameterString(haraokoshiInst.Symbol, "記号");
                            if (kigouList.Contains(kigou))
                            {
                                SetBoltData(doc, md, fi, targetId, ref yamadomeBoltSheetDatalist, ref yamadomeBoltSheetDatalistWithID);
                                break;
                            }
                        }
                    }

                    #region 未使用
                    ////各回転ピースと接触する腹起ベースのIDリスト
                    //List<ElementId> intersectHaraokoshiIdList = ClsRevitUtil.GetIntersectFamilysLiteExtensionZDirIs0pnt1(doc, fi.Id, null, allHaraokoshiIdList);
                    //               foreach (ElementId intersectHaraokoshiId in intersectHaraokoshiIdList)
                    //               {
                    //                   FamilyInstance haraokoshiBaseInstance = doc.GetElement(intersectHaraokoshiId) as FamilyInstance;
                    //                   if (haraokoshiBaseInstance != null)
                    //                   {
                    //                       //接触する腹起ベースの「鋼材サイズ」パラメータを取得する
                    //                       Parameter kouzaiSizeParam = haraokoshiBaseInstance.LookupParameter("鋼材サイズ");
                    //		if (kouzaiSizeParam != null)
                    //		{
                    //                           string kouzaiSize = kouzaiSizeParam.AsString();
                    //		}
                    //                   }
                    //               }
                    #endregion
                }

                //⑯斜梁受ピース積算
                foreach (FamilyInstance fi in shabariukePieceInstanceList)
                {
                    string familyName = fi.Symbol.FamilyName;
                    List<stMasterData> checkList = GetBoltEstimationList(familyName, masterData);
                    //マスタ行は一つのみ
                    if (checkList.Count < 1)
                    {
                        continue;
                    }
                    stMasterData hitData = checkList[0];
                    SetBoltDataSingle(doc, hitData, fi, ref yamadomeBoltSheetDatalist, ref yamadomeBoltSheetDatalistWithID);
                    if (!string.IsNullOrWhiteSpace(hitData.No13_AddBolt))
                    {
                        SetBoltDataSingleAddBolt(doc, hitData, fi, ref yamadomeBoltSheetDatalist, ref yamadomeBoltSheetDatalistWithID);
                    }
                }

                //⑰切梁繋ぎ材ベース積算
                foreach (FamilyInstance fi in kiribariTsunagizaiBaseInstanceList)
                {
                    SetBoldDataFromCustumDataForBase(doc, fi, ref yamadomeBoltSheetDatalist, ref yamadomeBoltSheetDatalistWithID, outputName: "切梁繋ぎ材");
                }

                //⑱火打繋ぎ材ベース積算
                foreach (FamilyInstance fi in hiuchiTsunagizaiBaseInstanceList)
                {
                    SetBoldDataFromCustumDataForBase(doc, fi, ref yamadomeBoltSheetDatalist, ref yamadomeBoltSheetDatalistWithID, outputName: "火打繋ぎ材");
                }

                //⑲頭つなぎ材積算
                foreach (FamilyInstance fi in atamaTsunagizaiInstanceList)
                {
                    SetBoldDataFromCustumData(doc, fi, ref yamadomeBoltSheetDatalist, ref yamadomeBoltSheetDatalistWithID, outputName: "頭つなぎ材");
                }

                //⑳せん断ボルト補強材積算
                foreach (FamilyInstance fi in sendanBoltHokyozaiInstanceList)
                {
                    SetBoldDataFromCustumData(doc, fi, ref yamadomeBoltSheetDatalist, ref yamadomeBoltSheetDatalistWithID, outputName: "せん断ボルト補強材");
                }

                //㉑腹起スベリ止メ積算
                foreach (FamilyInstance fi in haraokoshiSuberidomeInstanceList)
                {
                    SetBoldDataFromCustumDataForHaraokoshiSuberidome(doc, fi, ref yamadomeBoltSheetDatalist, ref yamadomeBoltSheetDatalistWithID, outputName: "腹起スベリ止メ");
                }
                //㉒斜梁繋ぎ材積算
                foreach (FamilyInstance fi in shabariTsunagizaiInstanceList)
                {
                    SetBoldDataFromCustumDataForBase(doc, fi, ref yamadomeBoltSheetDatalist, ref yamadomeBoltSheetDatalistWithID, outputName: "斜梁繋ぎ材");
                }

            }
            catch (Exception)
            {
                return false;
               
            }
            return true;
        }

        /// <summary>
        /// カスタムデータから情報取得するボルト積算の共通処理
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="fi"></param>
        /// <param name="boltName"></param>
        /// <param name="boltNum"></param>
        /// <param name="yamadomeBoltSheetDatalist"></param>
        /// <param name="yamadomeBoltSheetDatalistWithID"></param>
        private void SetBoldDataFromCustumData(Document doc, FamilyInstance fi, ref List<stBoltDataForBoltSheet> yamadomeBoltSheetDatalist, ref List<stBoltDataForBoltSheet> yamadomeBoltSheetDatalistWithID, string outputName = "")
		{
            var boltNameAndNum = ClsYMSUtil.GetBolt(doc, fi.Id);
            string boltName = boltNameAndNum.Item1 == null ? "ボルト情報なし" : boltNameAndNum.Item1;
            int boltNum = boltNameAndNum.Item2;

            string kigou = ClsRevitUtil.GetTypeParameterString( fi.Symbol, "サイズ");

            if (boltNum < 1)
            {
                return;
            }

            SetBoltDataSingle(doc, boltName, boltNum, fi, ref yamadomeBoltSheetDatalist, ref yamadomeBoltSheetDatalistWithID, outputName, outputKigou: kigou);
        }

        private void SetBoldDataFromCustumDataForBase(Document doc, FamilyInstance fi, ref List<stBoltDataForBoltSheet> yamadomeBoltSheetDatalist, ref List<stBoltDataForBoltSheet> yamadomeBoltSheetDatalistWithID, string outputName = "")
        {
            var boltNameAndNum = ClsYMSUtil.GetBolt(doc, fi.Id);
            string boltName = boltNameAndNum.Item1 == null ? "ボルト情報なし" : boltNameAndNum.Item1;
            int boltNum = boltNameAndNum.Item2;

            string kigou = ClsRevitUtil.GetParameter(doc, fi.Id, "サイズ");

            if (boltNum < 1)
            {
                return;
            }

            SetBoltDataSingle(doc, boltName, boltNum, fi, ref yamadomeBoltSheetDatalist, ref yamadomeBoltSheetDatalistWithID, outputName:outputName,outputKigou: kigou);
        }

        private void SetBoldDataFromCustumDataForHaraokoshiSuberidome(Document doc, FamilyInstance fi, ref List<stBoltDataForBoltSheet> yamadomeBoltSheetDatalist, ref List<stBoltDataForBoltSheet> yamadomeBoltSheetDatalistWithID, string outputName = "")
        {
            var boltNameAndNum = ClsYMSUtil.GetBolt(doc, fi.Id);
            string boltName = boltNameAndNum.Item1 == null ? "ボルト情報なし" : boltNameAndNum.Item1;
            int boltNum = boltNameAndNum.Item2;

            //string kigou = ClsRevitUtil.GetTypeParameterString(fi.Symbol, "サイズ");

            if (boltNum < 1)
            {
                return;
            }

            SetBoltDataSingle(doc, boltName, boltNum, fi, ref yamadomeBoltSheetDatalist, ref yamadomeBoltSheetDatalistWithID, outputName);
        }



        /// <summary>
        /// 山留主材と山留主材のボルトデータを取得
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="shuzaiIdList"></param>
        /// <param name="shuzaiBoltSheetDatalist"></param>
        /// <param name="shuzaiBoltSheetDatalistWithID"></param>
        /// <returns></returns>
        public bool ShuzaiAndShuzaiBoltData(Document doc, ClsSekisanSetting sekisansetting,List<ElementId> shuzaiIdList,
            ref List<stBoltDataForBoltSheet> shuzaiBoltSheetDatalist, ref List<stBoltDataForBoltSheet> shuzaiBoltSheetDatalistWithID)
        {
            try
            {
                //マスタ読み込み
                List<stMasterData> masterData = new List<stMasterData>();
                if (!GetBoltCsvData(ref masterData))
                {
                    return false;
                }

                List<FamilyInstance> instShuzaiList = YamadomeKoudaiBoltSekisan.ElementIdList2FamilyinstanceList(doc, shuzaiIdList);

                //主材+主材ボルトはカバープレートで扱ってるので当面無視

                //Wのコーナー火打箇所 腹起し横Wのボルトのみ扱う
                //引数で渡されたインスタンス群のID版

                //コーナー火打ち仮鋼材分割用
                double divShuzaiLength = sekisansetting.KariCornerHiuchi;


                List <ElementId> targetIds = new List<ElementId>();
                foreach (FamilyInstance inst in instShuzaiList)
                {
                    string daibunrui = ClsRevitUtil.GetTypeParameterString(inst.Symbol, "大分類");
                    if (daibunrui.Contains("構台"))
                    {
                        continue;
                    }

                    //if (inst.Symbol.FamilyName.Contains("仮鋼材"))
                    //{
                    //    continue;
                    //}
                    targetIds.Add(inst.Id);
                }

                List<ElementId> JoudanTargetIds = new List<ElementId>();
                List<ElementId> GedanTargetIds = new List<ElementId>();

                foreach (ElementId id in targetIds)
                {
                    FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                    if (inst == null)
                    {
                        continue;
                    }

                    if (inst.Symbol.Name == "隅火打")
                    {
                        string familyName = inst.Symbol.FamilyName;
                        if (familyName.Contains("仮鋼材_"))
                        {
                            double kijyun = ClsRevitUtil.GetParameterDouble(doc, id, "ホストからのオフセット");
                            if (ClsGeo.GEO_GT0(kijyun))
                            {
                                //0より高い　0ではない
                                JoudanTargetIds.Add(id);
                            }
                            else if (ClsGeo.GEO_LT0(kijyun))
                            {
                                //0より低い　0ではない
                                GedanTargetIds.Add(id);
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            double kijyun = ClsRevitUtil.GetParameterDouble(doc, id, "基準レベルからの高さ");
                            if (ClsGeo.GEO_GT0(kijyun))
                            {
                                //0より高い　0ではない
                                JoudanTargetIds.Add(id);
                            }
                            else if (ClsGeo.GEO_LT0(kijyun))
                            {
                                //0より低い　0ではない
                                GedanTargetIds.Add(id);
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                }


                //隅火打
                foreach (ElementId joudanSumihiutiShuzaiId in JoudanTargetIds)
                {
                    FamilyInstance inst = doc.GetElement(joudanSumihiutiShuzaiId) as FamilyInstance;
                    if (inst == null)
                    {
                        continue;
                    }
                    string familyName = inst.Symbol.FamilyName;

                    bool karikouzaiFlg = false;
                    //仮鋼材名称のままではマスタに適合しないので名前を成型
                    if (familyName.Contains("仮鋼材_"))
                    {
                        karikouzaiFlg = true;

                        familyName = familyName.Replace("仮鋼材_", "");
                        if (!familyName.Contains("山留"))
                        {
                            familyName = "山留" + familyName;
                        }
                    }

                    string check = string.Empty;

                    if (!karikouzaiFlg)
                    {
                        string[] div = familyName.Split('_'); //山留主材_35HA_1.5 の形式になっているので　山留主材_35HA　にする
                        if (div.Length < 3)
                        {
                            continue;
                        }

                        check = div[0] + "_" + div[1];
                    }
                    else
                    {
                        check = familyName;
                    }
                   
                    List<stMasterData> checkList = GetBoltEstimationList(check, masterData,true);
                    //マスタ行は一つのみ
                    if (checkList.Count < 1)
                    {
                        continue;
                    }
                    stMasterData hitData = checkList[0];
                    //対象は主材
                   
                    List<ElementId> Ids = ClsRevitUtil.GetIntersectFamilysLite(doc, joudanSumihiutiShuzaiId, null, GedanTargetIds);
                    if (Ids.Count < 1)
                    {
                        continue;
                    }

                    SetBoltData(doc, hitData, inst, Ids[0], ref shuzaiBoltSheetDatalist, ref shuzaiBoltSheetDatalistWithID,outputName:"隅火打(W)");
                }


                //腹起横W
                HaraokoshiYokoW_BoltCount(doc, masterData, sekisansetting, targetIds, ref shuzaiBoltSheetDatalist, ref shuzaiBoltSheetDatalistWithID);

            }
            catch (Exception)
            {

                return false;
            }
            return true;
        }

        private bool HaraokoshiYokoW_BoltCount(Document doc, List<stMasterData> masterData,ClsSekisanSetting sekisansetting, List<ElementId> targetIds,
            ref List<stBoltDataForBoltSheet> shuzaiBoltSheetDatalist, ref List<stBoltDataForBoltSheet> shuzaiBoltSheetDatalistWithID)
        {
            List<ElementId> HaraokoshiKarikouzaiIds = new List<ElementId>();
            List<ElementId> HaraokoshiIds = new List<ElementId>();

            try
            {
                foreach (ElementId id in targetIds)
                {
                    FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                    if (inst == null)
                    {
                        continue;
                    }

                    if (inst.Symbol.Name == "腹起")
                    {
                        string familyName = inst.Symbol.FamilyName;
                        if (!familyName.Contains("35HA") && !familyName.Contains("35SMH") && !familyName.Contains("40HA")
                            && !familyName.Contains("40SMH") && !familyName.Contains("50HA"))
                        {
                            continue;
                        }

                        if (familyName.Contains("仮鋼材"))
                        {
                            HaraokoshiKarikouzaiIds.Add(id);
                        }
                        else
                        {
                            HaraokoshiIds.Add(id);
                        }
                    }
                }

                //腹起サイズ	ｶﾊﾞｰﾌﾟﾚｰﾄ長(m)
                const double dCPSize350 = 0.6;
                const double dCPSize400 = 0.8;
                const double dCPSize500 = 1.2;

                //腹起サイズ	ｶﾊﾞｰﾌﾟﾚｰﾄのﾎﾞﾙﾄ本数
                const int nCPBolt350 = 12;
                const int nCPBolt400 = 16;
                const int nCPBolt500 = 24;

                //一般部ボルト間隔(m)
                const double dBoltInterval = 1.0;

                List<YMS.Parts.ClsHaraokoshiBase> baseList = YMS.Parts.ClsHaraokoshiBase.GetAllClsHaraokoshiBaseList(doc);
                foreach (YMS.Parts.ClsHaraokoshiBase baseCls in baseList)
                {
                    if (baseCls.m_yoko != ClsHaraokoshiBase.SideNum.Double)
                    {
                        //横ダブル以外対象外
                        continue;
                    }

                    ElementId baseId = baseCls.m_ElementId;
                    List<ElementId> yokoWHaraList = new List<ElementId>();
                    //腹起横W　接続ボルト
                    foreach (ElementId shuzaiId in HaraokoshiIds)
                    {
                        ElementId shuzaisBaseId = YMS.Command.ClsCommandWarituke.GetBaseId(doc, shuzaiId);
                        if (shuzaisBaseId == null)
                        {
                            continue;
                        }
                        if (baseId == shuzaisBaseId)
                        {
                            yokoWHaraList.Add(shuzaiId);
                        }
                    }

                    FamilyInstance familyInstance =null;

                    //腹起合計長さ
                    double haraSumLen = 0.0;
                    //腹起合計数
                    int haraCount = 0;
                    string size = string.Empty;
                    foreach (ElementId id in yokoWHaraList)
                    {
                        FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                        if (inst == null)
                        {
                            continue;
                        }

                        familyInstance = inst;

                        string familyName = inst.Symbol.FamilyName;
                        string[] div = familyName.Split('_'); //山留主材_35HA_1.5 の1.5を取得
                        if (div.Length < 3)
                        {
                            continue;
                        }

                        size = div[1];
                        string slen = div[2];
                        double len = 0.0;
                        if (double.TryParse(slen, out len))
                        {
                            haraSumLen += len;
                            haraCount++;
                        }
                    }

                    double half = haraSumLen / 2;
                    double cpLen = 0.0;
                    int boltNum = 0;

                    if (size.Contains("35"))
                    {
                        cpLen = dCPSize350;
                        boltNum = nCPBolt350;
                    }
                    else if (size.Contains("40"))
                    {
                        cpLen = dCPSize400;
                        boltNum = nCPBolt400;
                    }
                    else if (size.Contains("50"))
                    {
                        cpLen = dCPSize500;
                        boltNum = nCPBolt500;
                    }
                    else { continue; }

                    //受領資料記載の計算マクロ
                    //=2*ROUNDUP((C$5-$B$12*C$6)/C$7,0)+16*C$6
                    //=2*ROUNDUP((腹起主材全長 - ｶﾊﾞｰﾌﾟﾚｰﾄ長 * ｶﾊﾞｰﾌﾟﾚｰﾄ想定位置数) / 一般部ﾎﾞﾙﾄ間隔,0) + ｶﾊﾞｰﾌﾟﾚｰﾄのﾎﾞﾙﾄ数 * ｶﾊﾞｰﾌﾟﾚｰﾄ想定位置数

                    //例
                    //腹起ｻｲｽﾞ=2H400
                    //腹起主材の全長=15(m)
                    //ｶﾊﾞｰﾌﾟﾚｰﾄ想定位置数=4
                    //一般部ﾎﾞﾙﾄ間隔=1.0(m)
                    //ｶﾊﾞｰﾌﾟﾚｰﾄ長=0.8(m)

                    //2 * (15 - 0.8 * 4) / 1 + 16 * 4
                    //= 24 + 64
                    //= 88

                    var nAns = 2.0 * Math.Ceiling((half - cpLen * (haraCount - 2)) / dBoltInterval) + boltNum * (haraCount - 2);
                    SetBoltDataSingle(doc, "BN-70",(int)nAns, familyInstance, ref shuzaiBoltSheetDatalist, ref shuzaiBoltSheetDatalistWithID, outputName: "腹起　横W");
                }

                //仮鋼材
                double haraDiv = sekisansetting.KariHaraOkoshi;

                foreach (YMS.Parts.ClsHaraokoshiBase baseCls in baseList)
                {
                    if (baseCls.m_yoko != ClsHaraokoshiBase.SideNum.Double)
                    {
                        //横ダブル以外対象外
                        continue;
                    }

                    ElementId baseId = baseCls.m_ElementId;
                    List<ElementId> yokoWHaraList = new List<ElementId>();
                    //腹起横W　接続ボルト
                    foreach (ElementId shuzaiId in HaraokoshiKarikouzaiIds)
                    {
                        ElementId shuzaisBaseId = YMS.Parts.ClsWarituke.GetConnectionBase(doc, shuzaiId);
                        if (shuzaisBaseId == null)
                        {
                            continue;
                        }
                        if (baseId == shuzaisBaseId)
                        {
                            yokoWHaraList.Add(shuzaiId);
                        }
                    }

                    FamilyInstance familyInstance = null;

                    //腹起合計長さ
                    double haraSumLen = 0.0;
                    //想定ｶﾊﾞｰﾌﾟﾚｰﾄ枚数
                    int cpCount = 0;
                    string size = string.Empty;
                    foreach (ElementId id in yokoWHaraList)
                    {
                        FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                        if (inst == null)
                        {
                            continue;
                        }

                        familyInstance = inst;

                        string familyName = inst.Symbol.FamilyName;
                        string[] div = familyName.Split('_'); //仮鋼材_主材_30HA の30HAを取得
                        if (div.Length < 3)
                        {
                            continue;
                        }

                        size = div[2];
                        //長さ取得
                        LocationCurve lCurve = inst.Location as LocationCurve;
                        if (lCurve == null)
                        {
                            continue;
                        }
                        Curve cv = lCurve.Curve;
                        if (ClsGeo.GEO_EQ0(haraDiv) || ClsGeo.GEO_LE0(haraDiv))
                        {
                            continue;
                        }


                        decimal length = (decimal)ClsRevitUtil.CovertFromAPI(lCurve.Curve.GetEndPoint(0).DistanceTo(lCurve.Curve.GetEndPoint(1)));
                        haraSumLen += (double)length;

                        double num = Math.Floor((double)Math.Abs(length) / haraDiv);
                        int nNum = (int)num;
                        int amari = (int)((double)length % haraDiv);
                        if (nNum >= 1)
                        {
                            cpCount += nNum - 1;
                        }
                        if (amari > 0 && haraDiv <= (double)length)
                        {
                            cpCount++;
                        }
                        
                        //double divlen = dist / haraDiv;
                        //divlen = Math.Floor(divlen);

                        //double len = dist / 1000;
                        //haraSumLen += len;
                        //haraCount += (int)divlen;
                    }

                    double half = haraSumLen / 2000;
                    double cpLen = 0.0;
                    int boltNum = 0;

                    if (size.Contains("35"))
                    {
                        cpLen = dCPSize350;
                        boltNum = nCPBolt350;
                    }
                    else if (size.Contains("40"))
                    {
                        cpLen = dCPSize400;
                        boltNum = nCPBolt400;
                    }
                    else if (size.Contains("50"))
                    {
                        cpLen = dCPSize500;
                        boltNum = nCPBolt500;
                    }
                    else { continue; }

                    //受領資料記載の計算マクロ
                    //=2*ROUNDUP((C$5-$B$12*C$6)/C$7,0)+16*C$6
                    //=2*ROUNDUP((腹起主材全長 - ｶﾊﾞｰﾌﾟﾚｰﾄ長 * ｶﾊﾞｰﾌﾟﾚｰﾄ想定位置数) / 一般部ﾎﾞﾙﾄ間隔,0) + ｶﾊﾞｰﾌﾟﾚｰﾄのﾎﾞﾙﾄ数 * ｶﾊﾞｰﾌﾟﾚｰﾄ想定位置数

                    //例
                    //腹起ｻｲｽﾞ=2H400
                    //腹起主材の全長=15(m)
                    //ｶﾊﾞｰﾌﾟﾚｰﾄ想定位置数=4
                    //一般部ﾎﾞﾙﾄ間隔=1.0(m)
                    //ｶﾊﾞｰﾌﾟﾚｰﾄ長=0.8(m)

                    //2 * (15 - 0.8 * 4) / 1 + 16 * 4
                    //= 24 + 64
                    //= 88

                    var nAns = 2.0 * Math.Ceiling((half - cpLen * cpCount) / dBoltInterval) + boltNum*2 * cpCount;
                    SetBoltDataSingle(doc, "BN-70", (int)nAns, familyInstance, ref shuzaiBoltSheetDatalist, ref shuzaiBoltSheetDatalistWithID, outputName: "腹起　横W");
                }

            }
            catch (Exception e)
            {

                return false; ;
            }

            return true;
        }

        private List<ElementId> GetSameLevelShuzaiList(Document doc, FamilyInstance fi, List<ElementId> idShuzaiList,string typeName = "")
        {
            List<ElementId> res = new List<ElementId>();

           string level = ClsRevitUtil.GetInstMojiParameter(doc, fi.Id, "集計レベル");
            if (string.IsNullOrWhiteSpace(level))
            {
                level = ClsRevitUtil.GetInstMojiParameter(doc, fi.Id, "基準レベル");
                if (string.IsNullOrWhiteSpace(level))
                {
                    if (fi.Host != null)
                    {
                        level = fi.Host.Name;
                    }

                }
            }

            foreach (ElementId id in idShuzaiList)
            {
                //接触判定された主材が構台主材なら対象外
                FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                if (inst == null)
                {
                    continue;
                    //return res;
                }

                string daibunrui = ClsRevitUtil.GetTypeParameterString(inst.Symbol, "大分類");
                if (daibunrui.Contains("構台"))
                {
                    continue;
                    //return res;
                }

                string level2 = ClsRevitUtil.GetInstMojiParameter(doc, id, "集計レベル");
                if (string.IsNullOrWhiteSpace(level2))
                {
                    if (inst.Host != null)
                    {
                        level2 = inst.Host.Name;
                    }
                    
                }

                if (level == level2)
                {
                    if (string.IsNullOrWhiteSpace(typeName))
                    {
                        res.Add(id);
                    }
                    else
                    {
                        if (inst.Symbol.Name == typeName)
                        {
                            res.Add(id);
                        }
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// 接触判定された部材情報とマスタ情報を基に出力用ボルトデータリストにデータをセットする
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="masterData">マスタデータ</param>
        /// <param name="inst">ボルト積算対象部材のインスタンス</param>
        /// <param name="hitTargetId">上記と接触判定された部材のID</param>
        /// <param name="yamadomeBoltSheetDatalist">出力用ボルト情報1</param>
        /// <param name="yamadomeBoltSheetDatalistWithID">出力用ボルト情報1</param>
        /// <param name="outputName">出力用部材名称</param>
        /// <returns></returns>
        private bool SetBoltData(Document doc, stMasterData masterData, FamilyInstance inst, ElementId hitTargetId, ref List<stBoltDataForBoltSheet> yamadomeBoltSheetDatalist, ref List<stBoltDataForBoltSheet> yamadomeBoltSheetDatalistWithID, 
            string outputName = "",string outputKigou = "", double plusAtsumi = 0,int kashoSuu = 1)
        {
            try
            {
                //接触判定された主材が鋼材主材をなら対象外
                FamilyInstance hitInst = doc.GetElement(hitTargetId) as FamilyInstance;
                if (hitInst == null)
                {
                    return false;
                }

                string daibunrui = ClsRevitUtil.GetTypeParameterString(hitInst.Symbol, "大分類");
                if (daibunrui.Contains("構台"))
                {
                    return false;
                }


                stBoltDataForBoltSheet stdata = new stBoltDataForBoltSheet();
                stdata.buzaiId = inst.Id;


                //品種
                stdata.hinshu = ClsRevitUtil.GetTypeParameterString(inst.Symbol, "品種");
                ElementId levelId = inst.LevelId;

                //段名（レベル名）
                stdata.DanName = ClsRevitUtil.GetInstMojiParameter(doc, inst.Id, "集計レベル");
                if (string.IsNullOrWhiteSpace(stdata.DanName)) 
                {
                    stdata.DanName = ClsRevitUtil.GetInstMojiParameter(doc, inst.Id, "基準レベル");
                    if (string.IsNullOrWhiteSpace(stdata.DanName))
                    {
                        if (inst.Host != null)
                        {
                            stdata.DanName = inst.Host.Name;
                        }

                    }
                }

                stdata.DanName = ConvDanmeiForSekisan(stdata.DanName);

                //名称
                if (string.IsNullOrWhiteSpace(outputName))
                {
                    stdata.buzaiName = ClsRevitUtil.GetTypeParameterString(inst.Symbol, "名称");
                }
                else
                {
                    stdata.buzaiName = outputName;
                }

                //記号
                if (string.IsNullOrWhiteSpace(outputKigou))
                {
                    stdata.Kigou = ClsRevitUtil.GetTypeParameterString(inst.Symbol, "記号");
                }
                else
                {
                    stdata.Kigou = outputKigou;
                }
                

                //箇所数
                stdata.Kashosu = kashoSuu;//

                //ボルト名
                int length = 0;

                if (masterData.No0_FixFlg != "1")
                {
                    int atsumi = GetKouzaiAtshumi(doc, hitTargetId, masterData.No5_PenetrationPoint);

                    if (masterData.No8_Bolt == BN)
                    {
                        length = GetYamadomeBoltLength(enBotType.BN, (double)atsumi + masterData.No3_Thick, plusAtsumi);
                    }
                    else if (masterData.No8_Bolt == F10T)
                    {
                        length = GetYamadomeBoltLength(enBotType.F10T, (double)atsumi + masterData.No3_Thick, plusAtsumi);
                    }
                    else if (masterData.No8_Bolt == S10T)
                    {
                        length = GetYamadomeBoltLength(enBotType.S10T, (double)atsumi + masterData.No3_Thick, plusAtsumi);
                    }
                    else
                    {
                        return false;
                    }

                    stdata.boltName = masterData.No8_Bolt + length.ToString();
                }
                else
                {
                    stdata.boltName = masterData.No8_Bolt;
                }

                //一箇所当たりのボルト本数
                stdata.num = masterData.No9_Number1;
                stdata.PenetrationIds = new List<ElementId>();
                stdata.PenetrationIds.Add(hitTargetId);
                yamadomeBoltSheetDatalistWithID.Add(stdata);//ElementIdつき情報はマージの必要が無いのでここでよい

                bool sameHit = false;
                for (int i = 0; i < yamadomeBoltSheetDatalist.Count; i++)
                {
                    stBoltDataForBoltSheet dt = yamadomeBoltSheetDatalist[i];
                    if (dt.hinshu == stdata.hinshu &&
                        dt.DanName == stdata.DanName &&
                        dt.buzaiName == stdata.buzaiName &&
                        dt.Kigou == stdata.Kigou &&
                        dt.boltName == stdata.boltName &&
                        dt.num == stdata.num)
                    {
                        dt.Kashosu += 1;
                        sameHit = true;
                        yamadomeBoltSheetDatalist[i] = dt;
                        break;
                    }
                }
                if (!sameHit)
                {
                    yamadomeBoltSheetDatalist.Add(stdata);

                }
            }
            catch (Exception)
            {

                return false;
            }
            return true;
        }

        /// <summary>
        /// 接触判定された部材情報とマスタ情報を基に出力用ボルトデータリストにデータをセットする　ジャッキカバー用
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="masterData"></param>
        /// <param name="inst"></param>
        /// <param name="hitTargetId"></param>
        /// <param name="yamadomeBoltSheetDatalist"></param>
        /// <param name="yamadomeBoltSheetDatalistWithID"></param>
        /// <param name="outputName"></param>
        /// <param name="outputKigou"></param>
        /// <param name="plusAtsumi"></param>
        /// <param name="kashoSuu"></param>
        /// <returns></returns>
        private bool SetBoltDataForJackCover(Document doc, stMasterData masterData, FamilyInstance inst, ElementId hitTargetId, ref List<stBoltDataForBoltSheet> yamadomeBoltSheetDatalist, ref List<stBoltDataForBoltSheet> yamadomeBoltSheetDatalistWithID,
            string outputName = "", string outputKigou = "", double plusAtsumi = 0, int kashoSuu = 1)
        {
            try
            {
                //接触判定された主材が鋼材主材をなら対象外
                FamilyInstance hitInst = doc.GetElement(hitTargetId) as FamilyInstance;
                if (hitInst == null)
                {
                    return false;
                }

                string daibunrui = ClsRevitUtil.GetTypeParameterString(hitInst.Symbol, "大分類");
                if (daibunrui.Contains("構台"))
                {
                    return false;
                }


                stBoltDataForBoltSheet stdata = new stBoltDataForBoltSheet();
                stdata.buzaiId = inst.Id;


                //品種
                stdata.hinshu = ClsRevitUtil.GetTypeParameterString(inst.Symbol, "品種");
                ElementId levelId = inst.LevelId;

                //段名（レベル名）
                stdata.DanName = ClsRevitUtil.GetInstMojiParameter(doc, inst.Id, "集計レベル");
                if (string.IsNullOrWhiteSpace(stdata.DanName))
                {
                    stdata.DanName = ClsRevitUtil.GetInstMojiParameter(doc, inst.Id, "基準レベル");
                    if (string.IsNullOrWhiteSpace(stdata.DanName))
                    {
                        if (inst.Host != null)
                        {
                            stdata.DanName = inst.Host.Name;
                        }

                    }
                }
                stdata.DanName = ConvDanmeiForSekisan(stdata.DanName);


                //名称
                if (string.IsNullOrWhiteSpace(outputName))
                {
                    stdata.buzaiName = ClsRevitUtil.GetTypeParameterString(inst.Symbol, "名称");
                }
                else
                {
                    stdata.buzaiName = outputName;
                }

                //記号
                if (string.IsNullOrWhiteSpace(outputKigou))
                {
                    stdata.Kigou = ClsRevitUtil.GetTypeParameterString(inst.Symbol, "記号");
                }
                else
                {
                    stdata.Kigou = outputKigou;
                }


                //箇所数
                stdata.Kashosu = kashoSuu * 2;//

                //ボルト名
                int length = 0;

                if (masterData.No0_FixFlg != "1")
                {
                    int atsumi = GetKouzaiAtshumi(doc, hitTargetId, masterData.No5_PenetrationPoint);

                    if (masterData.No8_Bolt == BN)
                    {
                        length = GetYamadomeBoltLength(enBotType.BN, (double)atsumi + masterData.No3_Thick, plusAtsumi);
                    }
                    else if (masterData.No8_Bolt == F10T)
                    {
                        length = GetYamadomeBoltLength(enBotType.F10T, (double)atsumi + masterData.No3_Thick, plusAtsumi);
                    }
                    else if (masterData.No8_Bolt == S10T)
                    {
                        length = GetYamadomeBoltLength(enBotType.S10T, (double)atsumi + masterData.No3_Thick, plusAtsumi);
                    }
                    else
                    {
                        return false;
                    }

                    stdata.boltName = masterData.No8_Bolt + length.ToString();
                }
                else
                {
                    stdata.boltName = masterData.No8_Bolt;
                }

                //一箇所当たりのボルト本数
                stdata.num = masterData.No9_Number1 / 2;
                stdata.PenetrationIds = new List<ElementId>();
                stdata.PenetrationIds.Add(hitTargetId);
                yamadomeBoltSheetDatalistWithID.Add(stdata);//ElementIdつき情報はマージの必要が無いのでここでよい

                bool sameHit = false;
                for (int i = 0; i < yamadomeBoltSheetDatalist.Count; i++)
                {
                    stBoltDataForBoltSheet dt = yamadomeBoltSheetDatalist[i];
                    if (dt.hinshu == stdata.hinshu &&
                        dt.DanName == stdata.DanName &&
                        dt.buzaiName == stdata.buzaiName &&
                        dt.Kigou == stdata.Kigou &&
                        dt.boltName == stdata.boltName &&
                        dt.num == stdata.num)
                    {
                        dt.Kashosu += (kashoSuu * 2);
                        sameHit = true;
                        yamadomeBoltSheetDatalist[i] = dt;
                        break;
                    }
                }
                if (!sameHit)
                {
                    yamadomeBoltSheetDatalist.Add(stdata);

                }
            }
            catch (Exception)
            {

                return false;
            }
            return true;
        }

        /// <summary>
        /// 接触判定とか無視して機械的に数量データを出す場合はこちら
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="masterData"></param>
        /// <param name="inst"></param>
        /// <param name="yamadomeBoltSheetDatalist"></param>
        /// <param name="yamadomeBoltSheetDatalistWithID"></param>
        /// <param name="outputName"></param>
        /// <param name="outputKigou"></param>
        /// <param name="plusAtsumi"></param>
        /// <param name="kashoSuu"></param>
        /// <returns></returns>
        private bool SetBoltDataSingle(Document doc, stMasterData masterData, FamilyInstance inst, ref List<stBoltDataForBoltSheet> yamadomeBoltSheetDatalist, ref List<stBoltDataForBoltSheet> yamadomeBoltSheetDatalistWithID,
            string outputName = "", string outputKigou = "", double plusAtsumi = 0, int kashoSuu = 1)
        {
            try
            {

                stBoltDataForBoltSheet stdata = new stBoltDataForBoltSheet();
                stdata.buzaiId = inst.Id;


                //品種
                stdata.hinshu = ClsRevitUtil.GetTypeParameterString(inst.Symbol, "品種");
                ElementId levelId = inst.LevelId;

                //段名（レベル名）
                stdata.DanName = ClsRevitUtil.GetInstMojiParameter(doc, inst.Id, "集計レベル");
                if (string.IsNullOrWhiteSpace(stdata.DanName))
                {
                    stdata.DanName = ClsRevitUtil.GetInstMojiParameter(doc, inst.Id, "基準レベル");
                    if (string.IsNullOrWhiteSpace(stdata.DanName))
                    {
                        if (inst.Host != null)
                        {
                            stdata.DanName = inst.Host.Name;
                        }

                    }
                }
                stdata.DanName = ConvDanmeiForSekisan(stdata.DanName);

                //名称
                if (string.IsNullOrWhiteSpace(outputName))
                {
                    stdata.buzaiName = ClsRevitUtil.GetTypeParameterString(inst.Symbol, "名称");
                }
                else
                {
                    stdata.buzaiName = outputName;
                }

                //記号
                if (string.IsNullOrWhiteSpace(outputKigou))
                {
                    stdata.Kigou = ClsRevitUtil.GetTypeParameterString(inst.Symbol, "記号");
                }
                else
                {
                    stdata.Kigou = outputKigou;
                }


                //箇所数
                stdata.Kashosu = kashoSuu;//

                //ボルト名

                if (masterData.No0_FixFlg != "1")
                {
                    return false;
                }
                else
                {
                    stdata.boltName = masterData.No8_Bolt;
                }




                //一箇所当たりのボルト本数
                stdata.num = masterData.No9_Number1;
                stdata.PenetrationIds = new List<ElementId>();
                stdata.PenetrationIds.Add(inst.Id);
                yamadomeBoltSheetDatalistWithID.Add(stdata);//ElementIdつき情報はマージの必要が無いのでここでよい

                bool sameHit = false;
                for (int i = 0; i < yamadomeBoltSheetDatalist.Count; i++)
                {
                    stBoltDataForBoltSheet dt = yamadomeBoltSheetDatalist[i];
                    if (dt.hinshu == stdata.hinshu &&
                        dt.DanName == stdata.DanName &&
                        dt.buzaiName == stdata.buzaiName &&
                        dt.Kigou == stdata.Kigou &&
                        dt.boltName == stdata.boltName &&
                        dt.num == stdata.num)
                    {
                        dt.Kashosu += 1;
                        sameHit = true;
                        yamadomeBoltSheetDatalist[i] = dt;
                        break;
                    }
                }
                if (!sameHit)
                {
                    yamadomeBoltSheetDatalist.Add(stdata);

                }
            }
            catch (Exception)
            {

                return false;
            }
            return true;
        }

        /// <summary>
        /// 接触判定とか無視して機械的に数量データを出す場合はこちら
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="masterData"></param>
        /// <param name="inst"></param>
        /// <param name="yamadomeBoltSheetDatalist"></param>
        /// <param name="yamadomeBoltSheetDatalistWithID"></param>
        /// <param name="outputName"></param>
        /// <param name="outputKigou"></param>
        /// <param name="plusAtsumi"></param>
        /// <param name="kashoSuu"></param>
        /// <returns></returns>
        private bool SetBoltDataSingle(Document doc, string boltName,int boltNum ,FamilyInstance inst, ref List<stBoltDataForBoltSheet> yamadomeBoltSheetDatalist, ref List<stBoltDataForBoltSheet> yamadomeBoltSheetDatalistWithID,
            string outputName = "", string outputKigou = "", double plusAtsumi = 0, int kashoSuu = 1)
        {
            try
            {

                stBoltDataForBoltSheet stdata = new stBoltDataForBoltSheet();
                stdata.buzaiId = inst.Id;


                //品種
                stdata.hinshu = ClsRevitUtil.GetTypeParameterString(inst.Symbol, "品種");
                if (string.IsNullOrWhiteSpace(stdata.hinshu))
                {
                    //stdata.hinshu = ClsRevitUtil.GetTypeParameterString(inst.Symbol, "分類");//#31575 余計な処理かも…
                }
                ElementId levelId = inst.LevelId;

                //段名（レベル名）
                stdata.DanName = ClsRevitUtil.GetInstMojiParameter(doc, inst.Id, "集計レベル");
                if (string.IsNullOrWhiteSpace(stdata.DanName))
                {
                    stdata.DanName = ClsRevitUtil.GetInstMojiParameter(doc, inst.Id, "基準レベル");
                    if (string.IsNullOrWhiteSpace(stdata.DanName))
                    {
                        if (inst.Host != null)
                        {
                            stdata.DanName = inst.Host.Name;
                        }

                    }
                }
                stdata.DanName = ConvDanmeiForSekisan(stdata.DanName);

                //名称
                if (string.IsNullOrWhiteSpace(outputName))
                {
                    stdata.buzaiName = ClsRevitUtil.GetTypeParameterString(inst.Symbol, "名称");
                }
                else
                {
                    stdata.buzaiName = outputName;
                }

                //記号
                if (string.IsNullOrWhiteSpace(outputKigou))
                {
                    stdata.Kigou = ClsRevitUtil.GetTypeParameterString(inst.Symbol, "記号");
                }
                else
                {
                    stdata.Kigou = outputKigou;
                }


                //箇所数
                stdata.Kashosu = kashoSuu;//

                //ボルト名

                stdata.boltName = boltName;




                //一箇所当たりのボルト本数
                stdata.num = boltNum;
                stdata.PenetrationIds = new List<ElementId>();
                stdata.PenetrationIds.Add(inst.Id);
                yamadomeBoltSheetDatalistWithID.Add(stdata);//ElementIdつき情報はマージの必要が無いのでここでよい

                bool sameHit = false;
                for (int i = 0; i < yamadomeBoltSheetDatalist.Count; i++)
                {
                    stBoltDataForBoltSheet dt = yamadomeBoltSheetDatalist[i];
                    if (dt.hinshu == stdata.hinshu &&
                        dt.DanName == stdata.DanName &&
                        dt.buzaiName == stdata.buzaiName &&
                        dt.Kigou == stdata.Kigou &&
                        dt.boltName == stdata.boltName &&
                        dt.num == stdata.num)
                    {
                        dt.Kashosu += 1;
                        sameHit = true;
                        yamadomeBoltSheetDatalist[i] = dt;
                        break;
                    }
                }
                if (!sameHit)
                {
                    yamadomeBoltSheetDatalist.Add(stdata);

                }
            }
            catch (Exception)
            {

                return false;
            }
            return true;
        }

        private bool SetBoltDataSingleAddBolt(Document doc, stMasterData masterData, FamilyInstance inst, ref List<stBoltDataForBoltSheet> yamadomeBoltSheetDatalist, ref List<stBoltDataForBoltSheet> yamadomeBoltSheetDatalistWithID, string outputName = "", string outputKigou = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(masterData.No13_AddBolt))
                {
                    return true;
                }


                stBoltDataForBoltSheet stdata = new stBoltDataForBoltSheet();
                stdata.buzaiId = inst.Id;


                //品種
                stdata.hinshu = ClsRevitUtil.GetTypeParameterString(inst.Symbol, "品種");
                ElementId levelId = inst.LevelId;

                //段名（レベル名）
                stdata.DanName = ClsRevitUtil.GetInstMojiParameter(doc, inst.Id, "集計レベル");
                if (string.IsNullOrWhiteSpace(stdata.DanName))
                {
                    stdata.DanName = ClsRevitUtil.GetInstMojiParameter(doc, inst.Id, "基準レベル");
                }
                stdata.DanName = ConvDanmeiForSekisan(stdata.DanName);

                //名称
                if (string.IsNullOrWhiteSpace(outputName))
                {
                    stdata.buzaiName = ClsRevitUtil.GetTypeParameterString(inst.Symbol, "名称");
                }
                else
                {
                    stdata.buzaiName = outputName;
                }

                //記号
                if (string.IsNullOrWhiteSpace(outputKigou))
                {
                    stdata.Kigou = ClsRevitUtil.GetTypeParameterString(inst.Symbol, "記号");
                }
                else
                {
                    stdata.Kigou = outputKigou;
                }


                //箇所数
                stdata.Kashosu = 1;//

                //ボルト名


                stdata.boltName = masterData.No13_AddBolt;

                //一箇所当たりのボルト本数
                stdata.num = masterData.No14_AddBoltNum;
                stdata.PenetrationIds = new List<ElementId>();
                stdata.PenetrationIds.Add(inst.Id);
                yamadomeBoltSheetDatalistWithID.Add(stdata);//ElementIdつき情報はマージの必要が無いのでここでよい

                bool sameHit = false;
                for (int i = 0; i < yamadomeBoltSheetDatalist.Count; i++)
                {
                    stBoltDataForBoltSheet dt = yamadomeBoltSheetDatalist[i];
                    if (dt.hinshu == stdata.hinshu &&
                        dt.DanName == stdata.DanName &&
                        dt.buzaiName == stdata.buzaiName &&
                        dt.Kigou == stdata.Kigou &&
                        dt.boltName == stdata.boltName &&
                        dt.num == stdata.num)
                    {
                        dt.Kashosu += 1;
                        sameHit = true;
                        yamadomeBoltSheetDatalist[i] = dt;
                        break;
                    }
                }
                if (!sameHit)
                {
                    yamadomeBoltSheetDatalist.Add(stdata);

                }
            }
            catch (Exception)
            {

                return false;
            }
            return true;
        }

        private bool SetBoltDataAddBolt(Document doc, stMasterData masterData, FamilyInstance inst, ElementId hitTargetId, ref List<stBoltDataForBoltSheet> yamadomeBoltSheetDatalist, ref List<stBoltDataForBoltSheet> yamadomeBoltSheetDatalistWithID, string outputName = "", string outputKigou = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(masterData.No13_AddBolt))
                {
                    return true;
                }

                //接触判定された主材が鋼材主材をなら対象外
                FamilyInstance hitInst = doc.GetElement(hitTargetId) as FamilyInstance;
                if (hitInst == null)
                {
                    return false;
                }

                string daibunrui = ClsRevitUtil.GetTypeParameterString(hitInst.Symbol, "大分類");
                if (daibunrui.Contains("構台"))
                {
                    return false;
                }


                stBoltDataForBoltSheet stdata = new stBoltDataForBoltSheet();
                stdata.buzaiId = inst.Id;


                //品種
                stdata.hinshu = ClsRevitUtil.GetTypeParameterString(inst.Symbol, "品種");
                ElementId levelId = inst.LevelId;

                //段名（レベル名）
                stdata.DanName = ClsRevitUtil.GetInstMojiParameter(doc, inst.Id, "集計レベル");
                if (string.IsNullOrWhiteSpace(stdata.DanName))
                {
                    stdata.DanName = ClsRevitUtil.GetInstMojiParameter(doc, inst.Id, "基準レベル");
                }
                stdata.DanName = ConvDanmeiForSekisan(stdata.DanName);

                //名称
                if (string.IsNullOrWhiteSpace(outputName))
                {
                    stdata.buzaiName = ClsRevitUtil.GetTypeParameterString(inst.Symbol, "名称");
                }
                else
                {
                    stdata.buzaiName = outputName;
                }

                //記号
                if (string.IsNullOrWhiteSpace(outputKigou))
                {
                    stdata.Kigou = ClsRevitUtil.GetTypeParameterString(inst.Symbol, "記号");
                }
                else
                {
                    stdata.Kigou = outputKigou;
                }


                //箇所数
                stdata.Kashosu = 1;//

                //ボルト名


                stdata.boltName = masterData.No13_AddBolt;

                //一箇所当たりのボルト本数
                stdata.num = masterData.No14_AddBoltNum;
                stdata.PenetrationIds = new List<ElementId>();
                stdata.PenetrationIds.Add(hitTargetId);
                yamadomeBoltSheetDatalistWithID.Add(stdata);//ElementIdつき情報はマージの必要が無いのでここでよい

                bool sameHit = false;
                for (int i = 0; i < yamadomeBoltSheetDatalist.Count; i++)
                {
                    stBoltDataForBoltSheet dt = yamadomeBoltSheetDatalist[i];
                    if (dt.hinshu == stdata.hinshu &&
                        dt.DanName == stdata.DanName &&
                        dt.buzaiName == stdata.buzaiName &&
                        dt.Kigou == stdata.Kigou &&
                        dt.boltName == stdata.boltName &&
                        dt.num == stdata.num)
                    {
                        dt.Kashosu += 1;
                        sameHit = true;
                        yamadomeBoltSheetDatalist[i] = dt;
                        break;
                    }
                }
                if (!sameHit)
                {
                    yamadomeBoltSheetDatalist.Add(stdata);

                }
            }
            catch (Exception)
            {

                return false;
            }
            return true;
        }

        public bool KoudaiBoltSekisan(Document doc, ClsSekisanSetting sekisansetting, 
            ref List<stBoltDataForBoltSheet> koudaiBoltSheetDatalist,ref List<stBoltDataForBoltSheet> koudaiBoltSheetDatalistWithID)
        {
            try
            {
               
                //構台ボルト収集
                //[水平ブレス]取付方法
                string suiheiBraceTorituke = sekisansetting.HorToritsukeHoho;
                //[水平ブレス]ボルト種類
                string suiheiBraceBoltType = sekisansetting.HorBoltType;
                //[水平ブレス]一か所あたりのボルト本数
                int suiheiBraceBoltNum = sekisansetting.HorBoltNum;
                if (suiheiBraceTorituke == "ボルト")
                {
                    if (suiheiBraceBoltType == "普通ボルト" || suiheiBraceBoltType == "HTB")
                    {
                        List<(FamilyInstance, List<FamilyInstance>)> dataOandH = ReadFromGantryProducts.GetOhbikiAndHBraceSet(doc, includeShikigeta: true);

                        List<(FamilyInstance, List<FamilyInstance>)> dataPandH = ReadFromGantryProducts.GetPilePillerAndHBraceSet(doc);

                        //大引とそれに接触する水平ブレスたち
                        foreach ((FamilyInstance, List<FamilyInstance>) setData in dataOandH)
                        {
                            //大引き
                            FamilyInstance obiki = setData.Item1;
                            //水平ブレスたち
                            List<FamilyInstance> HBList = setData.Item2;

                            int obikiAtumi = GetKouzaiAtshumi(doc, obiki.Id, KOUZAI_FLANGE);

                            foreach (FamilyInstance HB in HBList)
                            {
                                int hbAtumi = GetKouzaiAtshumi(doc, HB.Id, KOUZAI_FLANGE);
                                int length = 0;
                                string boltSettoumoji = string.Empty;
                                if (suiheiBraceBoltType == "普通ボルト")
                                {
                                    boltSettoumoji = BN;
                                    length = GetKoudaiBoltLength(enBotType.BN, false, (double)hbAtumi + obikiAtumi, 0);

                                }
                                else if (suiheiBraceBoltType == "HTB")
                                {
                                    boltSettoumoji = F10T;
                                    length = GetKoudaiBoltLength(enBotType.F10T, false, (double)hbAtumi + obikiAtumi, 0);
                                }
                                else if (suiheiBraceBoltType == "トルシア")
                                {
                                    boltSettoumoji = S10T;
                                    length = GetKoudaiBoltLength(enBotType.S10T, false, (double)hbAtumi + obikiAtumi, 0);
                                }

                                stBoltDataForBoltSheet stdata = new stBoltDataForBoltSheet();
                                stdata.buzaiId = HB.Id;
                                stdata.PenetrationIds = new List<ElementId>();
                                stdata.PenetrationIds.Add(obiki.Id);

                                //品種
                                string size = ClsRevitUtil.GetTypeParameterString(HB.Symbol, "サイズ");
                                string size2 = ClsRevitUtil.GetTypeParameterString(obiki.Symbol, "サイズ");
                                //stdata.hinshu = "水平ブレス - 大引 :" + size + " - " + size2;
                                stdata.hinshu = "水平ブレス - 大引";

                                //記号
                                stdata.Kigou = string.Empty;

                                //箇所数
                                stdata.Kashosu = 1;//

                                //ボルト名
                                stdata.boltName = boltSettoumoji + length.ToString();

                                //一箇所当たりのボルト本数
                                stdata.num = suiheiBraceBoltNum;


                                koudaiBoltSheetDatalistWithID.Add(stdata);//ElementIdつき情報はマージの必要が無いのでここでよい

                                bool sameHit = false;
                                for (int i = 0; i < koudaiBoltSheetDatalist.Count; i++)
                                {
                                    stBoltDataForBoltSheet dt0 = koudaiBoltSheetDatalist[i];
                                    if (dt0.hinshu == stdata.hinshu &&
                                        dt0.Kigou == stdata.Kigou &&
                                        dt0.boltName == stdata.boltName &&
                                        dt0.num == stdata.num)
                                    {
                                        dt0.Kashosu += 1;
                                        sameHit = true;
                                        koudaiBoltSheetDatalist[i] = dt0;
                                        break;
                                    }
                                }
                                if (!sameHit)
                                {
                                    koudaiBoltSheetDatalist.Add(stdata);

                                }

                            }
                        }

                        //杭とそれに接触する水平ブレスたち
                        foreach ((FamilyInstance, List<FamilyInstance>) setData in dataPandH)
                        {
                            //杭
                            FamilyInstance kui = setData.Item1;
                            //水平ブレスたち
                            List<FamilyInstance> HBList = setData.Item2;

                            int kuiAtumi = ReadFromGantryProducts.GetPileTopPLThick(doc, kui.Id);

                            foreach (FamilyInstance HB in HBList)
                            {
                                int hbAtumi = GetKouzaiAtshumi(doc, HB.Id, KOUZAI_FLANGE);
                                int length = 0;
                                string boltSettoumoji = string.Empty;
                                if (suiheiBraceBoltType == "普通ボルト")
                                {
                                    boltSettoumoji = BN;
                                    length = GetKoudaiBoltLength(enBotType.BN, false, (double)hbAtumi + kuiAtumi, 0);

                                }
                                else if (suiheiBraceBoltType == "HTB")
                                {
                                    boltSettoumoji = F10T;
                                    length = GetKoudaiBoltLength(enBotType.F10T, false, (double)hbAtumi + kuiAtumi, 0);
                                }
                                else if (suiheiBraceBoltType == "トルシア")
                                {
                                    boltSettoumoji = S10T;
                                    length = GetKoudaiBoltLength(enBotType.S10T, false, (double)hbAtumi + kuiAtumi, 0);
                                }

                                stBoltDataForBoltSheet stdata = new stBoltDataForBoltSheet();
                                stdata.buzaiId = HB.Id;
                                stdata.PenetrationIds = new List<ElementId>();
                                stdata.PenetrationIds.Add(kui.Id);

                                //品種
                                string size = ClsRevitUtil.GetTypeParameterString(HB.Symbol, "サイズ");
                                string size2 = ClsRevitUtil.GetTypeParameterString(kui.Symbol, "サイズ");
                                //stdata.hinshu = "水平ブレス - 杭 :" + size + " - " + size2;
                                stdata.hinshu = "水平ブレス - 杭";

                                //記号
                                stdata.Kigou = string.Empty;

                                //箇所数
                                stdata.Kashosu = 1;//

                                //ボルト名
                                stdata.boltName = boltSettoumoji + length.ToString();

                                //一箇所当たりのボルト本数
                                stdata.num = suiheiBraceBoltNum;


                                koudaiBoltSheetDatalistWithID.Add(stdata);//ElementIdつき情報はマージの必要が無いのでここでよい

                                bool sameHit = false;
                                for (int i = 0; i < koudaiBoltSheetDatalist.Count; i++)
                                {
                                    stBoltDataForBoltSheet dt0 = koudaiBoltSheetDatalist[i];
                                    if (dt0.hinshu == stdata.hinshu &&
                                        dt0.Kigou == stdata.Kigou &&
                                        dt0.boltName == stdata.boltName &&
                                        dt0.num == stdata.num)
                                    {
                                        dt0.Kashosu += 1;
                                        sameHit = true;
                                        koudaiBoltSheetDatalist[i] = dt0;
                                        break;
                                    }
                                }
                                if (!sameHit)
                                {
                                    koudaiBoltSheetDatalist.Add(stdata);

                                }

                            }
                        }
                    }
                }



                //切梁受け
                //切梁受け　取付方法
                //切梁受け　固定方法
                //フランジ側ボルト本数
                //ウェブ側ボルト本数
                //取付補助材と杭
                string kiribariukeToritukehouhou = sekisansetting.KiriToritsukeHoho;
                string kiribariukeKoteihouhou = sekisansetting.KiriKoteiHoho;
                int kiribariukeFlangeHonsu = sekisansetting.KiriFlangeSiedeBoltNum;
                int kiribariukeWebHonsu = sekisansetting.KiriWebSiedeBoltNum;
                int kiribariukeHojiozaiAndKui = sekisansetting.KiriToritsukeHojoAndPile;

                if (kiribariukeToritukehouhou == "なし")
                {
                    if (kiribariukeKoteihouhou == "ボルト")
                    {
                        // 1) 対象部材：切梁受け材 接地面：フランジ
                        var kF = ReadFromGantryProducts.GetKoudaivBracehJointBoltCnt(doc, ReadFromGantryProducts.eBoltElement.Kiribari, ReadFromGantryProducts.eAttachSide.Flange);
    
                        stBoltDataForBoltSheet dt = new stBoltDataForBoltSheet();
                        dt.hinshu = "切梁受け - 支柱";
                        dt.Kashosu = kF;
                        dt.num = kiribariukeFlangeHonsu;
                        dt.boltName = "BN-70";

                        koudaiBoltSheetDatalist.Add(dt);

                        // 2) 対象部材：切梁受け材 接地面：ウェブ
                        var kW = ReadFromGantryProducts.GetKoudaivBracehJointBoltCnt(doc, ReadFromGantryProducts.eBoltElement.Kiribari, ReadFromGantryProducts.eAttachSide.Web);
                        
                        dt = new stBoltDataForBoltSheet();
                        dt.hinshu = "切梁受け - 取付補助材";
                        dt.Kashosu = kW;
                        dt.num = kiribariukeWebHonsu;
                        dt.boltName = "BN-70";

                        koudaiBoltSheetDatalist.Add(dt);


                        // 3) 対象部材：杭とツナギ用取付補助材
                        dt = new stBoltDataForBoltSheet();
                        dt.hinshu = "支柱 - 取付補助材";
                        dt.Kashosu = kW;
                        dt.num = kiribariukeHojiozaiAndKui;
                        dt.boltName = "BN-70";

                        koudaiBoltSheetDatalist.Add(dt);

                    }
                }

                //[垂直ブレスまたは水平つなぎ]取付方法
                string VerToritsukeHoho = sekisansetting.VerToritsukeHoho;
                //[垂直ブレスまたは水平つなぎ]固定方法
                string VerKoteiHoho = sekisansetting.VerKoteiHoho;
                // [垂直ブレスまたは水平つなぎ]水平つなぎ用プレート高さ
                double VerHorPlateHeight = sekisansetting.VerHorPlateHeight;
                //[垂直ブレスまたは水平つなぎ]垂直ブレス用プレート厚み
                double VerVerPlateThickness = sekisansetting.VerVerPlateThickness;
                //[垂直ブレスまたは水平つなぎ]プレート厚み
                double VerPlateThickness = sekisansetting.VerPlateThickness;
                // [垂直ブレスまたは水平つなぎ]フランジ側ボルト本数
                int VerFlangeSiedeBoltNum = sekisansetting.VerFlangeSiedeBoltNum;
                //[垂直ブレスまたは水平つなぎ]ウェブ側ボルト本数
                int VerWebSiedeBoltNum = sekisansetting.VerWebSiedeBoltNum;
                //[垂直ブレスまたは水平つなぎ]取付補助材と杭
                int VerToritsukeHojoAndPile = sekisansetting.VerToritsukeHojoAndPile;

                if (VerToritsukeHoho == "なし" )
                {
                    if (VerKoteiHoho == "ボルト")
                    {
                        // 1) 対象部材：垂直ブレース 接地面：フランジ
                        var tF = ReadFromGantryProducts.GetKoudaivBracehJointBoltCnt(doc, ReadFromGantryProducts.eBoltElement.VBrace, ReadFromGantryProducts.eAttachSide.Flange);
                      
                        stBoltDataForBoltSheet dt = new stBoltDataForBoltSheet();
                        dt.hinshu = "垂直ブレス - 支柱";
                        dt.Kashosu = tF;
                        dt.num = VerFlangeSiedeBoltNum;
                        dt.boltName = "BN-70";

                        koudaiBoltSheetDatalist.Add(dt);

                        // 2) 対象部材：垂直ブレース 接地面：ウェブ
                        var tW = ReadFromGantryProducts.GetKoudaivBracehJointBoltCnt(doc, ReadFromGantryProducts.eBoltElement.VBrace, ReadFromGantryProducts.eAttachSide.Web);

                        dt = new stBoltDataForBoltSheet();
                        dt.hinshu = "垂直ブレス - 取付補助材";
                        dt.Kashosu = tW;
                        dt.num = VerWebSiedeBoltNum;
                        dt.boltName = "BN-70";

                        koudaiBoltSheetDatalist.Add(dt);

                        // 3) 対象部材：水平ツナギ 接地面：フランジ
                        var hF = ReadFromGantryProducts.GetKoudaivBracehJointBoltCnt(doc, ReadFromGantryProducts.eBoltElement.HJoint, ReadFromGantryProducts.eAttachSide.Flange);

                        dt = new stBoltDataForBoltSheet();
                        dt.hinshu = "水平ツナギ - 支柱";
                        dt.Kashosu = hF;
                        dt.num = VerFlangeSiedeBoltNum ;
                        dt.boltName = "BN-70";

                        koudaiBoltSheetDatalist.Add(dt);


                        // 4) 対象部材：水平ツナギ 接地面：ウェブ
                        var hW = ReadFromGantryProducts.GetKoudaivBracehJointBoltCnt(doc, ReadFromGantryProducts.eBoltElement.HJoint, ReadFromGantryProducts.eAttachSide.Web);

                        dt = new stBoltDataForBoltSheet();
                        dt.hinshu = "水平ツナギ - 取付補助材";
                        dt.Kashosu = hW;
                        dt.num = VerWebSiedeBoltNum ;
                        dt.boltName = "BN-70";

                        koudaiBoltSheetDatalist.Add(dt);

                        // 5) 対象部材：杭とツナギ用取付補助材 接地面：ウェブ
                        var ws = ReadFromGantryProducts.GetKoudaivBracehJointBoltCnt(doc, ReadFromGantryProducts.eBoltElement.Support, ReadFromGantryProducts.eAttachSide.Web);

                        dt = new stBoltDataForBoltSheet();
                        dt.hinshu = "支柱 - 取付補助材";
                        dt.Kashosu = ws;
                        dt.num = VerToritsukeHojoAndPile;
                        dt.boltName = "BN-70";

                        koudaiBoltSheetDatalist.Add(dt);

                    }
                }


                //[桁受]主桁（根太）―桁受（大引）
                string KetaShugetaKetauke = sekisansetting.KetaShugetaKetauke;
                //[桁受]主桁（根太）―桁受（大引）ボルト本数
                int KetaShugetaKetaukeNum = sekisansetting.KetaShugetaKetaukeNum;

                List<(FamilyInstance, List<FamilyInstance>)> dataON = ReadFromGantryProducts.GetOhbikiAndNedaSet(doc, false, includeShikigeta: true);
                List<(FamilyInstance, List<FamilyInstance>)> dataONSlope = ReadFromGantryProducts.GetOhbikiAndNedaSet(doc, true, includeShikigeta: true);

                if (KetaShugetaKetauke == "普通ボルト" || KetaShugetaKetauke == "HTB")
                {

                    foreach ((FamilyInstance, List<FamilyInstance>) setData in dataON)
                    {
                        FamilyInstance obiki = setData.Item1;

                        List<FamilyInstance> nedaList = setData.Item2;

                        int obikiAtumi = GetKouzaiAtshumi(doc, obiki.Id, KOUZAI_FLANGE);

                        foreach (FamilyInstance neda in nedaList)
                        {
                            int nedaAtumi = GetKouzaiAtshumi(doc, neda.Id, KOUZAI_FLANGE);
                            int length = 0;
                            string boltSettoumoji = string.Empty;
                            if (KetaShugetaKetauke == "普通ボルト")
                            {
                                boltSettoumoji = BN;
                                length = GetKoudaiBoltLength(enBotType.BN, false, (double)nedaAtumi + obikiAtumi, 0);

                            }
                            else if (KetaShugetaKetauke == "HTB")
                            {
                                boltSettoumoji = F10T;
                                length = GetKoudaiBoltLength(enBotType.F10T, false, (double)nedaAtumi + obikiAtumi, 0);
                            }
                            else if (KetaShugetaKetauke == "トルシア")
                            {
                                boltSettoumoji = S10T;
                                length = GetKoudaiBoltLength(enBotType.S10T, false, (double)nedaAtumi + obikiAtumi, 0);
                            }

                            stBoltDataForBoltSheet stdata = new stBoltDataForBoltSheet();
                            stdata.buzaiId = obiki.Id;
                            stdata.PenetrationIds = new List<ElementId>();
                            stdata.PenetrationIds.Add(neda.Id);

                            //品種
                            stdata.hinshu = "根太-大引(又は敷桁)";

                            //記号
                            stdata.Kigou = string.Empty;

                            //箇所数
                            stdata.Kashosu = 1;//

                            //ボルト名
                            stdata.boltName = boltSettoumoji + length.ToString();

                            //一箇所当たりのボルト本数

                            //#32025 スロープ根太とノーマル根太の連結箇所はボルト数を調整　ノーマル根太のボルト数を半分にし、スロープ箇所を1.5倍にする
                            bool hit = false;
                            foreach ((FamilyInstance, List<FamilyInstance>) d in dataONSlope)
                            {
                                FamilyInstance o = d.Item1;
                                //if (o.Id != obiki.Id)
                                //{
                                //    continue;
                                //}
                                List<ElementId> tmpList = new List<ElementId>();
                                List<ElementId> tmpList2 = new List<ElementId>();
                                tmpList2.Add(obiki.Id);
                                List<FamilyInstance> ceckNedaList = d.Item2;
                                foreach (FamilyInstance isttmp in ceckNedaList)
                                {
                                    //大引きと接していないスロープ根太は除く
                                    if (RevitUtil.ClsRevitUtil.GetIntersectFamilys(doc, isttmp.Id, 0.1, serchIds: tmpList2).Count > 0)
                                    {
                                        tmpList.Add(isttmp.Id);
                                    }

                                }


                                if (tmpList.Count > 0)
                                {
                                    tmpList = RevitUtil.ClsRevitUtil.GetIntersectFamilys(doc, neda.Id, 0.1, serchIds: tmpList);
                                    if (tmpList.Count > 0)
                                    {
                                        hit = true;
                                        break;
                                    }
                                }
                            }

                            if (hit/*ReadFromGantryProducts.SlopenedaAndNedaConnect(doc, neda.Id, obiki.Id, "", true)*/)
                            {
                                stdata.num = KetaShugetaKetaukeNum / 2;
                            }
                            else
                            {
                                stdata.num = KetaShugetaKetaukeNum;
                            }

                            koudaiBoltSheetDatalistWithID.Add(stdata);//ElementIdつき情報はマージの必要が無いのでここでよい

                            bool sameHit = false;
                            for (int i = 0; i < koudaiBoltSheetDatalist.Count; i++)
                            {
                                stBoltDataForBoltSheet dt = koudaiBoltSheetDatalist[i];
                                if (dt.hinshu == stdata.hinshu &&
                                    dt.Kigou == stdata.Kigou &&
                                    dt.boltName == stdata.boltName &&
                                    dt.num == stdata.num)
                                {
                                    dt.Kashosu += 1;
                                    sameHit = true;
                                    koudaiBoltSheetDatalist[i] = dt;
                                    break;
                                }
                            }
                            if (!sameHit)
                            {
                                koudaiBoltSheetDatalist.Add(stdata);

                            }

                        }

                    }
                }



                //[桁受]主桁（根太）―桁受（大引）スロープの場合
                string KetaShugetaKetaukeSlope = sekisansetting.KetaShugetaKetaukeSlope;
                //[桁受]主桁（根太）―桁受（大引）スロープの場合　本数
                int KetaShugetaKetaukeSlopeNu = sekisansetting.KetaShugetaKetaukeSlopeNum;

                if (KetaShugetaKetaukeSlope == "普通ボルト" || KetaShugetaKetaukeSlope == "HTB")
                {
                    //List<(FamilyInstance, List<FamilyInstance>)> dataONSlope = ReadFromGantryProducts.GetOhbikiAndNedaSet(doc, true, includeShikigeta: true);

                    foreach ((FamilyInstance, List<FamilyInstance>) setData in dataONSlope)
                    {
                        FamilyInstance obiki = setData.Item1;

                        List<FamilyInstance> nedaList = setData.Item2;

                        int obikiAtumi = GetKouzaiAtshumi(doc, obiki.Id, KOUZAI_FLANGE);

                        foreach (FamilyInstance neda in nedaList)
                        {
                            int nedaAtumi = GetKouzaiAtshumi(doc, neda.Id, KOUZAI_FLANGE);
                            int length = 0;
                            string boltSettoumoji = string.Empty;
                            if (KetaShugetaKetaukeSlope == "普通ボルト")
                            {
                                boltSettoumoji = BN;
                                length = GetKoudaiBoltLength(enBotType.BN, true, (double)nedaAtumi + obikiAtumi, 0);
                            }
                            else if (KetaShugetaKetaukeSlope == "HTB")
                            {
                                boltSettoumoji = F10T;
                                length = GetKoudaiBoltLength(enBotType.F10T, true, (double)nedaAtumi + obikiAtumi, 0);
                            }
                            else if (KetaShugetaKetaukeSlope == "トルシア")
                            {
                                boltSettoumoji = S10T;
                                length = GetKoudaiBoltLength(enBotType.S10T, true, (double)nedaAtumi + obikiAtumi, 0);
                            }

                            stBoltDataForBoltSheet stdata = new stBoltDataForBoltSheet();
                            stdata.buzaiId = obiki.Id;
                            stdata.PenetrationIds = new List<ElementId>();
                            stdata.PenetrationIds.Add(neda.Id);

                            //品種
                            stdata.hinshu = "根太-大引(又は敷桁)";

                            //記号
                            stdata.Kigou = string.Empty;

                            //箇所数
                            stdata.Kashosu = 1;//

                            //ボルト名
                            stdata.boltName = boltSettoumoji + length.ToString();

                            //一箇所当たりのボルト本数
                            //#32025 スロープ根太とノーマル根太の連結箇所はボルト数を調整　ノーマル根太のボルト数を半分にし、スロープ箇所を1.5倍にする
                            bool hit = false;
                            foreach ((FamilyInstance, List<FamilyInstance>) d in dataON)
                            {
                                FamilyInstance o = d.Item1;
                                //if (o.Id != obiki.Id)
                                //{
                                //    continue;
                                //}
                                List<ElementId> tmpList = new List<ElementId>();
                                List<FamilyInstance> ceckNedaList = d.Item2;
                                foreach (FamilyInstance isttmp in ceckNedaList)
                                {
                                    tmpList.Add(isttmp.Id);
                                }

                                if (tmpList.Count > 0)
                                {
                                    tmpList = RevitUtil.ClsRevitUtil.GetIntersectFamilys(doc, neda.Id, 0.1, serchIds: tmpList);
                                    if (tmpList.Count > 0)
                                    {
                                        hit = true;
                                        break;
                                    }
                                }
                            }
                            if (hit/*ReadFromGantryProducts.SlopenedaAndNedaConnect(doc, neda.Id, null, "", true)*/)
                            {
                                stdata.num = KetaShugetaKetaukeSlopeNu + (KetaShugetaKetaukeSlopeNu / 2);
                            }
                            else
                            {
                                stdata.num = KetaShugetaKetaukeSlopeNu;
                            }


                            koudaiBoltSheetDatalistWithID.Add(stdata);//ElementIdつき情報はマージの必要が無いのでここでよい

                            bool sameHit = false;
                            for (int i = 0; i < koudaiBoltSheetDatalist.Count; i++)
                            {
                                stBoltDataForBoltSheet dt = koudaiBoltSheetDatalist[i];
                                if (dt.hinshu == stdata.hinshu &&
                                    dt.Kigou == stdata.Kigou &&
                                    dt.boltName == stdata.boltName &&
                                    dt.num == stdata.num)
                                {
                                    dt.Kashosu += 1;
                                    sameHit = true;
                                    koudaiBoltSheetDatalist[i] = dt;
                                    break;
                                }
                            }
                            if (!sameHit)
                            {
                                koudaiBoltSheetDatalist.Add(stdata);

                            }

                        }

                    }
                }

                //[桁受]桁受（ﾁｬﾝﾈﾙ）―桁受（ﾁｬﾝﾈﾙ）の場合
                string KetaKetaukeKetauke = sekisansetting.KetaKetaukeKetauke;
                //[桁受]桁受（ﾁｬﾝﾈﾙ）―桁受（ﾁｬﾝﾈﾙ）の場合　本数
                int KetaKetaukeKetaukeNum = sekisansetting.KetaKetaukeKetaukeNum;
                if (KetaKetaukeKetauke == "普通ボルト" || KetaKetaukeKetauke == "HTB")
                {
                    List<(FamilyInstance, List<FamilyInstance>)> data = ReadFromGantryProducts.GetChannelKetaukeSet(doc);

                    foreach ((FamilyInstance, List<FamilyInstance>) setData in data)
                    {
                        FamilyInstance oIns = setData.Item1;

                        List<FamilyInstance> ohbikiIns = setData.Item2;

                        int oInsAtumi = GetKouzaiAtshumi(doc, oIns.Id, KOUZAI_FLANGE);

                        foreach (FamilyInstance ohbiki in ohbikiIns)
                        {
                            int ohbikiAtumi = GetKouzaiAtshumi(doc, ohbiki.Id, KOUZAI_FLANGE);
                            int length = 0;
                            string boltSettoumoji = string.Empty;
                            if (KetaKetaukeKetauke == "普通ボルト")
                            {
                                boltSettoumoji = BN;
                                length = GetKoudaiBoltLength(enBotType.BN, false, (double)ohbikiAtumi + oInsAtumi, 0);
                            }
                            else if (KetaKetaukeKetauke == "HTB")
                            {
                                boltSettoumoji = F10T;
                                length = GetKoudaiBoltLength(enBotType.F10T, false, (double)ohbikiAtumi + oInsAtumi, 0);
                            }
                            else if (KetaKetaukeKetauke == "トルシア")
                            {
                                boltSettoumoji = S10T;
                                length = GetKoudaiBoltLength(enBotType.S10T, false, (double)ohbikiAtumi + oInsAtumi, 0);
                            }

                            stBoltDataForBoltSheet stdata = new stBoltDataForBoltSheet();
                            stdata.buzaiId = oIns.Id;
                            stdata.PenetrationIds = new List<ElementId>();
                            stdata.PenetrationIds.Add(ohbiki.Id);

                            //品種
                            stdata.hinshu = "桁受（ﾁｬﾝﾈﾙ）- 桁受（ﾁｬﾝﾈﾙ）";

                            //記号
                            stdata.Kigou = string.Empty;

                            //箇所数
                            stdata.Kashosu = 1;//

                            //ボルト名
                            stdata.boltName = boltSettoumoji + length.ToString();

                            //一箇所当たりのボルト本数
                            stdata.num = KetaKetaukeKetaukeNum;
                            //if (KetaShugetaKetauke == "普通ボルト")
                            //{
                            //    washerWithBn += stdata.num;

                            //}
                            koudaiBoltSheetDatalistWithID.Add(stdata);//ElementIdつき情報はマージの必要が無いのでここでよい

                            bool sameHit = false;
                            for (int i = 0; i < koudaiBoltSheetDatalist.Count; i++)
                            {
                                stBoltDataForBoltSheet dt = koudaiBoltSheetDatalist[i];
                                if (dt.hinshu == stdata.hinshu &&
                                    dt.Kigou == stdata.Kigou &&
                                    dt.boltName == stdata.boltName &&
                                    dt.num == stdata.num)
                                {
                                    dt.Kashosu += 1;
                                    sameHit = true;
                                    koudaiBoltSheetDatalist[i] = dt;
                                    break;
                                }
                            }
                            if (!sameHit)
                            {
                                koudaiBoltSheetDatalist.Add(stdata);

                            }

                        }

                    }
                }


                //杭―桁受（大引）
                string KuiKuiKetauke = sekisansetting.KuiKuiKetauke;
                //杭―桁受（大引）本数
                int KuiKuiKetaukeNum = sekisansetting.KuiKuiKetaukeNum;
                if (KuiKuiKetauke == "普通ボルト" || KuiKuiKetauke == "HTB")
                {
                    List<(FamilyInstance, List<FamilyInstance>)> data = ReadFromGantryProducts.GetPilePillerAndKetaUkeSet(doc, includeShikigeta: true); ;

                    foreach ((FamilyInstance, List<FamilyInstance>) setData in data)
                    {
                        FamilyInstance pillerIns = setData.Item1;

                        List<FamilyInstance> ohbikiIns = setData.Item2;

                        int pillerAtumi = GetKouzaiAtshumi(doc, pillerIns.Id, KOUZAI_FLANGE);

                        foreach (FamilyInstance ohbiki in ohbikiIns)
                        {
                            int ohbikiAtumi = GetKouzaiAtshumi(doc, ohbiki.Id, KOUZAI_FLANGE);
                            int length = 0;
                            string boltSettoumoji = string.Empty;
                            if (KuiKuiKetauke == "普通ボルト")
                            {
                                boltSettoumoji = BN;
                                length = GetKoudaiBoltLength(enBotType.BN, false, (double)ohbikiAtumi + pillerAtumi, 0);
                            }
                            else if (KuiKuiKetauke == "HTB")
                            {
                                boltSettoumoji = F10T;
                                length = GetKoudaiBoltLength(enBotType.F10T, false, (double)ohbikiAtumi + pillerAtumi, 0);
                            }
                            else if (KuiKuiKetauke == "トルシア")
                            {
                                boltSettoumoji = S10T;
                                length = GetKoudaiBoltLength(enBotType.S10T, false, (double)ohbikiAtumi + pillerAtumi, 0);
                            }

                            stBoltDataForBoltSheet stdata = new stBoltDataForBoltSheet();
                            stdata.buzaiId = pillerIns.Id;
                            stdata.PenetrationIds = new List<ElementId>();
                            stdata.PenetrationIds.Add(ohbiki.Id);

                            //品種
                            stdata.hinshu = "ﾄｯﾌﾟﾌﾟﾚｰﾄと大引";

                            //記号
                            stdata.Kigou = string.Empty;

                            //箇所数
                            stdata.Kashosu = 1;//

                            //ボルト名
                            stdata.boltName = boltSettoumoji + length.ToString();

                            //一箇所当たりのボルト本数
                            stdata.num = KuiKuiKetaukeNum;

                            koudaiBoltSheetDatalistWithID.Add(stdata);//ElementIdつき情報はマージの必要が無いのでここでよい

                            bool sameHit = false;
                            for (int i = 0; i < koudaiBoltSheetDatalist.Count; i++)
                            {
                                stBoltDataForBoltSheet dt = koudaiBoltSheetDatalist[i];
                                if (dt.hinshu == stdata.hinshu &&
                                    dt.Kigou == stdata.Kigou &&
                                    dt.boltName == stdata.boltName &&
                                    dt.num == stdata.num)
                                {
                                    dt.Kashosu += 1;
                                    sameHit = true;
                                    koudaiBoltSheetDatalist[i] = dt;
                                    break;
                                }
                            }
                            if (!sameHit)
                            {
                                koudaiBoltSheetDatalist.Add(stdata);

                            }

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
        /// 鋼材のフランジやウェブ等の厚みを返す
        /// </summary>
        /// <param name="targetShuzaiId"></param>
        /// <param name="kasho"></param>
        /// <returns></returns>
        public int GetKouzaiAtshumi(Document doc, ElementId targetKouzaiiId, string kasho)
        {
            int res = 20;
            try
            {
                if (kasho == KOUZAI_FLANGE)
                {
                    FamilyInstance inst = doc.GetElement(targetKouzaiiId) as FamilyInstance;
                    FamilySymbol fs = inst.Symbol;
                    string size = ClsRevitUtil.GetTypeParameterString(fs, "サイズ");
                    string fl = YMS.ClsYMSUtil.GetKouzaiSizeSunpou2(size, 4);
                    res = RevitUtil.ClsCommonUtils.ChangeStrToInt(fl);
                }
                else if (kasho == KOUZAI_WEB)
                {
                    FamilyInstance inst = doc.GetElement(targetKouzaiiId) as FamilyInstance;
                    FamilySymbol fs = inst.Symbol;
                    string size = ClsRevitUtil.GetTypeParameterString(fs, "サイズ");
                    string web = YMS.ClsYMSUtil.GetKouzaiSizeSunpou2(size, 3);
                    res = RevitUtil.ClsCommonUtils.ChangeStrToInt(web);
                }
                else if (kasho == KOUZAI_Plate)
                {
                    FamilyInstance inst = doc.GetElement(targetKouzaiiId) as FamilyInstance;
                    string instName = inst.Symbol.FamilyName;
                    if (instName.Contains("20HA"))
                    {
                        res = 12;
                    }
                    else if (instName.Contains("25HA"))
                    {
                        res = 16;
                    }
                    else if (instName.Contains("30HA"))
                    {
                        res = 16;
                    }
                    else if (instName.Contains("35HA"))
                    {
                        res = 16;
                    }
                    else if (instName.Contains("40HA"))
                    {
                        res = 16;
                    }
                    else if (instName.Contains("40HA"))
                    {
                        res = 25;
                    }
                    else if (instName.Contains("35SMH"))
                    {
                        res = 16;
                    }
                    else if (instName.Contains("40SMH"))
                    {
                        res = 16;
                    }
                    else if (instName.Contains("80SMH"))
                    {
                        res = 16;
                    }
                    else if (instName.Contains("60SMH"))
                    {
                        res = 32;
                    }
                }

            }
            catch (Exception)
            {

                return res; ;
            }

            return res;
        }

        private string CreateF10T(string num1, string num2)
        {
            return F10T + num1 + "X" + num2;
        }

        private string CreateS10T(string num1, string num2)
        {
            return S10T + num1 + "X" + num2;
        }


        /// <summary>
        /// ボルトの首下長さを取得
        /// </summary>
        /// <param name="type">ボルトのタイプ　普通　ハイテン　トルシア</param>
        /// <param name="atsumiList">ボルトが貫通する部材の厚さリスト</param>
        /// <param name="sumAtsumi">厚み合計</param>
        /// <param name="plusAtsumi">それとは別に足しておきたい長さ　指定なしは0</param>
        /// <returns></returns>
        public static int GetYamadomeBoltLength(enBotType type, double sumAtsumi,/*List<double> atsumiList,*/ double plusAtsumi = 0)
        {
            double res = plusAtsumi + sumAtsumi;

            //foreach (double da in atsumiList)
            //{
            //    res += da;
            //}

            bool nishaSannyu = false;//2捨3入

            if (type == enBotType.BN)
            {
                res += 24;
                if (res < 65.0)
                {
                    //普通ボルトは下限が65
                    res = 65;
                }
            }
            else if (type == enBotType.F10T)
            {
                res += 40;
                nishaSannyu = true;

            }
            else if (type == enBotType.S10T)
            {
                res += 35;
            }

            if (nishaSannyu)
            {
                res = Math.Ceiling(res);
                double amari = res % 5;

                if (amari > 0 && amari < 3)
                {
                    res -= amari;
                }
                else if (amari >= 3)
                {
                    double plus = 5 - amari;
                    res += plus;
                }
            }
            else 
            {
                res = Math.Ceiling(res);
                double amari = res % 5;

                if (amari != 0)
                {
                    double plus = 5 - amari;
                    res += plus;
                }
            }

            int nres = (int)res;
            return nres;
        }

        public static int GetKoudaiBoltLength(enBotType type, bool slope = false, double sumAtsumi = 0, double plusAtsumi = 0)
        {
            double res = plusAtsumi + sumAtsumi;

            //foreach (double da in atsumiList)
            //{
            //    res += da;
            //}

            if (type == enBotType.BN)
            {
                if (slope)
                {
                    res = 120;
                }
                else
                {
                    res = 70;
                }
            }
            else if (type == enBotType.F10T)
            {
                res += 40;
                res = Math.Ceiling(res);
                double amari = res % 5;

                if (amari >= 3)
                {
                    double plus = 5 - amari;
                    res += plus;
                }
                else if (amari < 3 && amari > 0)
                {
                    res -= amari;
                }

            }
            else if (type == enBotType.S10T)
            {
                //ない
                return 0;
            }


            int nres = (int)res;
            return nres;
        }

        /// <summary>
        /// ファミリインスタンスのリストをElementIdのリストに変換
        /// </summary>
        /// <param name="familyInstancelistList"></param>
        /// <returns></returns>
        public static List<ElementId> FamilyInstanceList2ElementIdList(List<FamilyInstance> familyInstancelistList)
        {
            List<ElementId> res = new List<ElementId>();
            foreach (FamilyInstance inst in familyInstancelistList)
            {
                res.Add(inst.Id);
            }
            return res;
        }

        /// <summary>
        /// ElementIdのリストをファミリインスタンスのリストに変換
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="idList"></param>
        /// <returns></returns>

        public static List<FamilyInstance> ElementIdList2FamilyinstanceList(Document doc, List<ElementId> idList)
        {
            List<FamilyInstance> res = new List<FamilyInstance>();
            foreach (ElementId id in idList)
            {
                FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                if (inst == null)
                {
                    continue;
                }
                res.Add(inst);
            }
            return res;
        }

        /// <summary>
        /// ElementIdのリストをファミリインスタンスのリストに変換　インスタンスのパラメータ「大分類」に「構台」が含まれている場合は除去する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="idList"></param>
        /// <returns></returns>
        public static List<FamilyInstance> ElementIdList2FamilyinstanceList2(Document doc, List<ElementId> idList)
        {
            List<FamilyInstance> res = new List<FamilyInstance>();
            foreach (ElementId id in idList)
            {
                FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                if (inst == null)
                {
                    continue;
                }

                string daibunrui = ClsRevitUtil.GetTypeParameterString(inst.Symbol, "大分類");
                if (daibunrui.Contains("構台"))
                {
                    continue;
                }

                res.Add(inst);
            }
            return res;
        }

        /// <summary>
        /// ボルト箇所マスタの読み込み処理
        /// </summary>
        /// <returns></returns>
        public bool GetBoltCsvData(ref  List<stMasterData> masterData)
        {

            string symbolFolpath = YMS.Parts.ClsZumenInfo.GetYMSFolder();
            string fileName = System.IO.Path.Combine(symbolFolpath, BoltCSV);
            List<List<string>> lstlstStr = new List<List<string>>();
            if (!RevitUtil.ClsCommonUtils.ReadCsv(fileName, ref lstlstStr))
            {
                MessageBox.Show("CSVファイルの取得に失敗しました。：" + fileName);
                return false;
            }

            bool bHeader = true;
            List<stMasterData> boltEstimationList = new List<stMasterData>();

            foreach (List<string> lstStr in lstlstStr)
            {
                if (bHeader)
                {
                    bHeader = false;
                    continue;
                }

                stMasterData boltEstimation = new stMasterData();
                boltEstimation.No0_FixFlg = GetStringFromCSVList(lstStr, 0);
                boltEstimation.No1_Target = GetStringFromCSVList(lstStr, 1);
                boltEstimation.No2_TargetDetail = GetStringFromCSVList(lstStr, 2);
                boltEstimation.No3_Thick = GetDoubleFromCSVList(lstStr, 3);
                boltEstimation.No4_JoiningPartner = GetStringFromCSVList(lstStr, 4);
                boltEstimation.No5_PenetrationPoint = GetStringFromCSVList(lstStr, 5);
                boltEstimation.No6_PenetrationThick = GetDoubleFromCSVList(lstStr, 6);
                boltEstimation.No7_Type = GetStringFromCSVList(lstStr, 7);
                boltEstimation.No8_Bolt = GetStringFromCSVList(lstStr, 8);
                boltEstimation.No9_Number1 = GetIntFromCSVList(lstStr, 9);
                boltEstimation.No10_Number2 = GetIntFromCSVList(lstStr, 10);
                boltEstimation.No11_multiCount = GetStringFromCSVList(lstStr, 11);
                boltEstimation.No12_Remarks = GetStringFromCSVList(lstStr, 12);
                boltEstimation.No13_AddBolt = GetStringFromCSVList(lstStr, 13);
                boltEstimation.No14_AddBoltNum = GetIntFromCSVList(lstStr, 14);

                boltEstimationList.Add(boltEstimation);
            }
            masterData = boltEstimationList;

            return true;
        }

        /// <summary>
        /// 引数で指定した「対象」の値から、該当する行をListで取得する
        /// </summary>
        /// <param name="target">「対象」の値</param>
        /// <returns></returns>
        public List<stMasterData> GetBoltEstimationList(string target, List<stMasterData> masterData,bool partialMatch = false )
        {
            List<stMasterData> boltEstimationList = new List<stMasterData>();

            foreach (stMasterData boltEstimation in masterData)
            {
                if (partialMatch)
                {
                    if (boltEstimation.No1_Target.Contains(target))
                    {
                        boltEstimationList.Add(boltEstimation);
                    }
                }
                else
                {
                    if (boltEstimation.No1_Target == target)
                    {
                        boltEstimationList.Add(boltEstimation);
                    }
                }
                
            }

            return boltEstimationList;
        }


        /// <summary>
        /// 対象列と備考欄からボルト種類を取得（主材+主材用）
        /// </summary>
        /// <param name="target"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        public string GetBoltType(string target, string bikou, List<stMasterData> masterData)
        {
            string bolt = BN;

            foreach (stMasterData boltEstimation in masterData)
            {
                if (target.Contains(boltEstimation.No1_Target) && boltEstimation.No12_Remarks == bikou)
                {
                    bolt = boltEstimation.No8_Bolt;
                    break;
                }
            }

            return bolt;
        }

        /// <summary>
        /// 対象列と備考欄から本数を取得（主材+主材用）
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public int GetBoltNum(string target, string bikou, List<stMasterData> masterData)
        {
            int num = 0;

            foreach (stMasterData boltEstimation in masterData)
            {
                if (target.Contains(boltEstimation.No1_Target) && boltEstimation.No12_Remarks == bikou)
                {
                    num = boltEstimation.No9_Number1;
                    break;
                }
            }

            return num;
        }

        /// <summary>
        /// CSVのstring型のリストから文字列を取得(indexの値がリストのサイズ外であれば空白を返す)
        /// </summary>
        /// <param name="lstStr"></param>
        /// <param name="index">0～始まるリストのインデックス番号</param>
        /// <returns></returns>
        public string GetStringFromCSVList(List<string> lstStr, int index)
        {
            //indexの値がリストのサイズ外であれば空白を返す
            if (index >= lstStr.Count()) { return string.Empty; }
            return lstStr[index];
        }

        /// <summary>
        /// CSVのstring型のリストからint型の値を取得(値が空白であれば0を返す)(indexの値がリストのサイズ外であれば0を返す)
        /// </summary>
        /// <param name="lstStr"></param>
        /// <param name="index">0～始まるリストのインデックス番号</param>
        /// <returns></returns>
        public int GetIntFromCSVList(List<string> lstStr, int index)
        {
            //indexの値がリストのサイズ外であれば空白を返す
            if (index >= lstStr.Count()) { return 0; }
            //値が空白であれば0を返す
            if (string.IsNullOrEmpty(lstStr[index]) || string.IsNullOrWhiteSpace(lstStr[index])) { return 0; }

            int result;
            if (!int.TryParse(lstStr[index], out result)) { return 0; }
            return result;
        }

        /// <summary>
        /// CSVのstring型のリストから文字列を取得(indexの値がリストのサイズ外であれば1を返す 空白でも1を返す)
        /// </summary>
        /// <param name="lstStr"></param>
        /// <param name="index">0～始まるリストのインデックス番号</param>
        /// <returns></returns>
        public string GetStringFromCSVList2(List<string> lstStr, int index)
        {
            //indexの値がリストのサイズ外であれば空白を返す
            if (index >= lstStr.Count()) { return "1"; }
            return lstStr[index];
        }

        /// <summary>
        /// CSVのstring型のリストからdouble型の値を取得(値が空白であれば0を返す)(indexの値がリストのサイズ外であれば0を返す)
        /// </summary>
        /// <param name="lstStr"></param>
        /// <param name="index">0～始まるリストのインデックス番号</param>
        /// <returns></returns>
        public double GetDoubleFromCSVList(List<string> lstStr, int index)
        {
            //indexの値がリストのサイズ外であれば空白を返す
            if (index >= lstStr.Count()) { return 0; }
            //値が空白であれば0を返す
            if (string.IsNullOrEmpty(lstStr[index]) || string.IsNullOrWhiteSpace(lstStr[index])) { return 0; }

            double result;
            if (!double.TryParse(lstStr[index], out result)) { return 0; }
            return result;
        }

        /// <summary>
        /// レベル名「支保工1段目」を「1」に変換する
        /// </summary>
        /// <param name="danmei"></param>
        /// <remarks>数量側では段名は1,2などの整数値を要求しているため
        /// 変換失敗すれば999を返すy</remarks>
        /// <returns></returns>
        private string ConvDanmeiForSekisan(string danmei)
        {
            string res = string.Empty;

            bool morikae = false;
            //#32611
            if (danmei.Contains("盛替"))
            {
                morikae = true;
            }

            res = Regex.Replace(danmei, @"[^0-9]", "");
            if (string.IsNullOrWhiteSpace(res))
            {
                res = "999";
            }

            if (morikae)
            {
                res = "盛替" + res;
            }

            return res;
        }
    }
}
