using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RevitUtil;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YMS.Parts;

namespace YMS
{
    public class ClsKariKouzai
    {
        #region 定数
        public const string BASEID = "ベースID";
        const string KARIKOUZAI = "H_仮鋼材";
        const string KOUSEI = "構成";
        const string DOUBLE = "ダブル";
        const string KOUZAISIZE = "鋼材サイズ";
        const string KOUZAITYPE = "鋼材タイプ";
        const string KOUZAISIZESHINGLE = "鋼材サイズ(シングル)";
        const string KOUZAISIZEDOUBLEUP = "鋼材サイズ(ダブル上)";
        const string KOUZAISIZEDOUBLEDN = "鋼材サイズ(ダブル下)";
        //const string HIUCHILENGTHSHINGLE  = "火打長さ(シングル)L";
        const string HIUCHILENGTHDOUBLEUP = "火打長さ(ダブル上)L1";
        const string HIUCHILENGTHDOUBLEDN = "火打長さ(ダブル下)L2";
        const string HIUCHIZURERYOU = "上下火打ずれ量a";
        const string BUNRUI = "分類";
        const string SIZE = "サイズ";
        const string KOUZAISIZEKIRIBARI = "切梁側/鋼材サイズ(ダブル)";//切梁側/切梁繋ぎ長さ
        const string KOUZAISIZEHARAOKOSHI = "腹起側/鋼材サイズ(ダブル)";
        const string KOUZAILENGTHKIRIBARI = "切梁側/切梁繋ぎ長さ";
        const string KOUZAILENGTHHARAOKOSHI = "腹起側/切梁繋ぎ長さ";
        const string TANBUBUHINSHITEN = "端部部品(始点側)";
        const string TANBUBUHINSYUTEN = "端部部品(終点側)";
        const string HIUCHIUKEPIECESIZE1 = "火打受ピースサイズ(1)";
        const string HIUCHIUKEPIECESIZE2 = "火打受ピースサイズ(2)";
        const string BUHINTYPEKIRIBARI = "部品タイプ(切梁側)";
        const string BUHINSIZEKIRIBARI = "部品サイズ(切梁側)";
        const string BUHINTYPEHARAOKOSHI = "部品タイプ(腹起側)";
        const string BUHINSIZEHARAOKOSHI = "部品サイズ(腹起側)";
        const string KIRIBARISIDEBUHIN = "切梁側/部品";
        const string HARAOKOSHISIDEBUHIN = "腹起側/部品";

        //回転角度設定INI関連の定数
        const string ROTATIONANGLESETTINGSINIPATH = "RotationAngleSettings.ini";
        const string SECKAITENPIECE = "SEC_KAITEN_PIECE";
        const string SECSYABARIUKEPIECE = "SEC_SYABARI_UKE_PIECE";
        const string KEYLOWERLIMITθ1 = "LowerLimitθ1";
        const string KEYUPPERLIMITθ1 = "UpperLimitθ1";
        const string KEYLOWERLIMITθ2 = "LowerLimitθ2";
        const string KEYUPPERLIMITθ2 = "UpperLimitθ2";
        public const string BASEMESSAGE = "回転角度が許容範囲外の斜梁受ピース、回転ピースの一覧を表示します。\r\n\r\n部材ID,パラメータ名,角度";
        #endregion

        /// <summary>
        /// 仮鋼材を作成
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static bool CreateKariKouzai(Document doc, ref string contents, ref List<ElementId> selectionIds)
        {
            //斜梁につく「斜梁受ピース」「回転ピース」のうち、回転角度が許容範囲外のものをテキストファイルに表示する
            //contents = "回転角度が許容範囲外の斜梁受ピース、回転ピースの一覧を表示します。\r\n\r\n部材ID,パラメータ名,角度";
            contents = BASEMESSAGE; //メッセージに追記があった場合は出力する #34194
            //INIからの読み込み
            double syabariukePieceLowerLimitθ1 = 0, syabariukePieceUpperLimitθ1 = 0, syabariukePieceLowerLimitθ2 = 0, syabariukePieceUpperLimitθ2 = 0,
                kaitenPieceLowerLimitθ2 = 0, kaitenPieceUpperLimitθ2 = 0;
            bool needCheckingRotationAngle = false;
            string iniPath = Path.Combine(ClsZumenInfo.GetYMSFolder(), ROTATIONANGLESETTINGSINIPATH);
			if (File.Exists(iniPath))
			{
                string syabariukePieceLowerLimitθ1Str = YMS.ClsIni.GetIniFile(SECSYABARIUKEPIECE, KEYLOWERLIMITθ1, iniPath);
                string syabariukePieceUpperLimitθ1Str = YMS.ClsIni.GetIniFile(SECSYABARIUKEPIECE, KEYUPPERLIMITθ1, iniPath);
                string syabariukePieceLowerLimitθ2Str = YMS.ClsIni.GetIniFile(SECSYABARIUKEPIECE, KEYLOWERLIMITθ2, iniPath);
                string syabariukePieceUpperLimitθ2Str = YMS.ClsIni.GetIniFile(SECSYABARIUKEPIECE, KEYUPPERLIMITθ2, iniPath);
                string kaitenPieceLowerLimitθ2Str = YMS.ClsIni.GetIniFile(SECKAITENPIECE, KEYLOWERLIMITθ2, iniPath);
                string kaitenPieceUpperLimitθ2Str = YMS.ClsIni.GetIniFile(SECKAITENPIECE, KEYUPPERLIMITθ2, iniPath);

                //回転角度設定INIから数値が取れているかチェック
                needCheckingRotationAngle = double.TryParse(syabariukePieceLowerLimitθ1Str, out syabariukePieceLowerLimitθ1)
                            && double.TryParse(syabariukePieceUpperLimitθ1Str, out syabariukePieceUpperLimitθ1)
                            && double.TryParse(syabariukePieceLowerLimitθ2Str, out syabariukePieceLowerLimitθ2)
                            && double.TryParse(syabariukePieceUpperLimitθ2Str, out syabariukePieceUpperLimitθ2)
                            && double.TryParse(kaitenPieceLowerLimitθ2Str, out kaitenPieceLowerLimitθ2)
                            && double.TryParse(kaitenPieceUpperLimitθ2Str, out kaitenPieceUpperLimitθ2);
            }

            //図面上のベースを全て取得
            List<ElementId> elements = ClsRevitUtil.GetSelectCreatedFamilyInstanceFamilySymbolList(doc, "ベース", true);
            List<ElementId> targetFamilies = new List<ElementId>();
            //ベース名順に並べ替える
            foreach (string baseName in ClsGlobal.m_baseShinList)
            {
                foreach (ElementId elem in elements)
                {
                    if (elem != null && doc.GetElement(elem).Name == baseName)
                    {
                        targetFamilies.Add(elem);
                    }
                }
            }

            foreach (ElementId baseID in targetFamilies)
            {
                //検証用
                if (ClsRevitUtil.GetParameter(doc, baseID, "コメント") == "TEST")
                {
                    string t = "breakPoint";
                }
                //string dan = ClsRevitUtil.GetParameter(doc, id, "段");
                FamilyInstance inst = doc.GetElement(baseID) as FamilyInstance;
                if (inst == null)
                {
                    continue;
                }
                
                //ElementId levelID = inst.Host.Id;

                string kouzaiType = ClsRevitUtil.GetParameter(doc, baseID, KOUZAITYPE);
                string kouzaiSize = ClsRevitUtil.GetParameter(doc, baseID, KOUZAISIZE);

                string buzaiShinName = inst.Name;
                string buzaiName = buzaiShinName.Replace("ベース", "");

                string kousei = ClsRevitUtil.GetParameter(doc, baseID, KOUSEI);

                double dKouzaiSize = 0.0;

                string kouzaiSizeUP = string.Empty;
                string kouzaiSizeDN = string.Empty;

                List<FamilySymbol> tanbuList = new List<FamilySymbol>();

                XYZ pS = null;
                XYZ pE = null;

                if (buzaiName == "腹起")
                {

                }
                if (buzaiName == "切梁")
                {
                    ClsKiribariBase ckb = new ClsKiribariBase();
                    ckb.SetParameter(doc, baseID);
                    ckb.CreateTanbuParts(doc, ref pS, ref pE);
                }
                if (buzaiName == "隅火打")
                {
                    kouzaiSizeUP = kouzaiSize;
                    kouzaiSizeDN = kouzaiSize;

                    ClsCornerHiuchiBase chb = new ClsCornerHiuchiBase();
                    chb.SetParameter(doc, baseID);
                    chb.CreateTanbuParts(doc, ref pS, ref pE); //自在火打ち対応
                }

                if (buzaiName == "切梁火打")//端部部品なしの場合は計算を抜く
                {
                    kouzaiSize = ClsRevitUtil.GetParameter(doc, baseID, KOUZAISIZESHINGLE);
                    kouzaiSizeUP = ClsRevitUtil.GetParameter(doc, baseID, KOUZAISIZEDOUBLEUP);
                    kouzaiSizeDN = ClsRevitUtil.GetParameter(doc, baseID, KOUZAISIZEDOUBLEDN);

                    double dAngle = ClsRevitUtil.GetParameterDouble(doc, baseID, "角度") / Math.PI * 180;
                    double hLengthUP = ClsCommonUtils.ChangeStrToDbl(ClsRevitUtil.GetParameter(doc, baseID, HIUCHILENGTHDOUBLEUP));

                    ClsKiribariHiuchiBase ckh = new ClsKiribariHiuchiBase();
                    ckh.SetParameter(doc, baseID);
                    if (!ckh.CreateTanbuParts1(doc, ref pS, ref pE))//falseが返ってくるときは基本腹起か切梁に接している箇所が1箇所以下
                        continue;
                }
                if (buzaiName == "切梁継ぎ")
                {
                    kouzaiSize = ClsRevitUtil.GetParameter(doc, baseID, KOUZAISIZESHINGLE);
                    if (kousei == DOUBLE)
                    {
                        kouzaiSizeUP = ClsRevitUtil.GetParameter(doc, baseID, KOUZAISIZEKIRIBARI);
                        kouzaiSizeDN = ClsRevitUtil.GetParameter(doc, baseID, KOUZAISIZEHARAOKOSHI);
                    }
                    else
                    {
                        kouzaiSizeUP = kouzaiSize;
                        kouzaiSizeDN = kouzaiSize;
                    }
                    //string tanbuTypeKiri = ClsRevitUtil.GetParameter(doc, id, KIRIBARISIDEBUHIN);
                    //tanbuList.Add(GetTanbuBuhinFamilySymbol(doc, buzaiName, tanbuTypeKiri, kouzaiSizeUP));

                    //string tanbuTypeHara = ClsRevitUtil.GetParameter(doc, id, HARAOKOSHISIDEBUHIN);
                    //tanbuList.Add(GetTanbuBuhinFamilySymbol(doc, buzaiName, tanbuTypeHara, kouzaiSizeDN));
                    ClsKiribariTsugiBase ckt = new ClsKiribariTsugiBase();
                    ckt.SetParameter(doc, baseID);
                    ckt.CreateTanbuParts(doc, ref pS, ref pE);
                }
                if (buzaiName == "切梁繋ぎ材" || buzaiName == "火打繋ぎ材")
                {
                    kouzaiType = ClsRevitUtil.GetParameter(doc, baseID, BUNRUI);
                    kouzaiSize = ClsRevitUtil.GetParameter(doc, baseID, SIZE);

                    List<ElementId> insecIdList = ClsYMSUtil.GetIntersectionBase(doc, baseID);
                    if (insecIdList.Count > 0)
                    {
                        string insecBaseKouzaiSize = ClsRevitUtil.GetParameter(doc, insecIdList[0], KOUZAISIZE);
                        string dan = ClsRevitUtil.GetParameter(doc, insecIdList[0], "段");
                        if (dan == "上段")
                        {
                            dKouzaiSize += ClsRevitUtil.CovertToAPI(Master.ClsYamadomeCsv.GetWidth(insecBaseKouzaiSize));
                        }
                        else if (dan == "同段" && ClsRevitUtil.GetParameter(doc, insecIdList[0], "構成") == "ダブル")
                        {
                            dKouzaiSize += ClsRevitUtil.CovertToAPI(Master.ClsYamadomeCsv.GetWidth(insecBaseKouzaiSize));
                        }
                        else if (dan == "同段")
                        {
                            dKouzaiSize += ClsRevitUtil.CovertToAPI(Master.ClsYamadomeCsv.GetWidth(insecBaseKouzaiSize) / 2);
                        }
                    }
                    //#31701一旦ﾌﾞﾙﾏﾝリキマン作図はコメントアウト
                    if (buzaiName == "切梁繋ぎ材")
                    {
                        ClsKiribariTsunagizaiBase ckt = new ClsKiribariTsunagizaiBase();
                        ckt.SetParameter(doc, baseID);
                        ckt.CreateTanbuParts(doc);
                    }
                    if (buzaiName == "火打繋ぎ材")
                    {
                        ClsHiuchiTsunagizaiBase cht = new ClsHiuchiTsunagizaiBase();
                        cht.SetParameter(doc, baseID);
                        cht.CreateTanbuParts(doc);
                    }
                }
                if (buzaiName == "切梁受け材")
                {
                    string aa = "test";
                }
                if (buzaiName == "斜梁") //仮鋼材オン→斜梁火打作成→仮鋼材オフ→仮鋼材オンの時の斜梁火打ちの再作成はここでやる　#34047
                {
                    var csh = new ClsSyabariBase();
                    //既存のデータをクラスにセット
                    csh.SetClassParameter(doc, baseID);
                    //始終点を保持
                    var baseLine = ClsYMSUtil.GetBaseLine(doc,baseID);
                    //var lPoint = inst.Location as LocationPoint;
                    XYZ tmpStPoint = baseLine.GetEndPoint(0); 
                    XYZ tmpEdPoint = baseLine.GetEndPoint(1);
                    var referencePlane = (doc.GetElement(baseID) as FamilyInstance).Host as ReferencePlane;
                    //端部部品作成

                    //斜張位置調整(新しい斜梁を作成する）
                    //var newSyabariId = id;
                    var (newSyabariId, piece1, piece2) = csh.ReplaceShabariBase(doc, baseID);
                    if (newSyabariId == null)
                        newSyabariId = baseID;
                    else
                    {
                        
                        List<ElementId> targetHiuchiList = GetTargetShabariHiuchi(doc, baseID); //#34047
                        using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                        {
                            t.Start();
                            ClsRevitUtil.Delete(doc, baseID);
                            //最初の位置を新しく作成した斜梁ベースにカスタムデータとして保持させる
                            try
                            {
                                ClsSyabariBase.SetCustomDataReferencePlane(doc, newSyabariId, referencePlane);
                                ClsSyabariBase.SetFirstPoint(doc, newSyabariId, tmpStPoint, tmpEdPoint);
                            }
                            catch (Exception ex)
                            {
                            }
                            t.Commit();
                        }
                        //斜梁火打ちをここで作成する #34047
                        foreach (ElementId hId in targetHiuchiList)
                        {
                            CreateShabariHiuchi(doc, hId, newSyabariId);
                        }
                    }

                    //端部部品の角度が許容範囲かチェック
                    if (needCheckingRotationAngle)
					{
                        for (int i = 0; i < 2; i++)
                        {
                            ElementId pieceId = i == 0 ? piece1 : piece2;
                            if (pieceId == null) { continue; }
                            FamilyInstance pieceInst = doc.GetElement(pieceId) as FamilyInstance;
                            if (pieceInst == null) { continue; }
                            string symbolName = pieceInst.Symbol.Name;
                            if (!symbolName.Contains("斜梁受") && !symbolName.Contains("回転")) { continue; }

                            string paramName = "θ1";
                            //θ1は斜梁受ピースが対象
                            if (symbolName.Contains("斜梁受"))
							{
                                string theta1Str = ClsRevitUtil.GetParameter(doc, pieceInst.Id, paramName);
                                //double theta1Rad = ClsRevitUtil.GetParameterDouble(doc, pieceInst.Id, paramName);
                                //double theta1 = UnitUtils.Convert(theta1Rad, UnitTypeId.Radians, UnitTypeId.Degrees);
								if (double.TryParse(theta1Str.Replace("°", ""), out double theta1))
								{
                                    //許容範囲外ならID、パラメータ名、角度を記載する
                                    if (!(ClsGeo.GEO_GE(theta1, syabariukePieceLowerLimitθ1) && ClsGeo.GEO_LE(theta1, syabariukePieceUpperLimitθ1)))
                                    {
                                        contents += "\r\n" + pieceInst.Id + "," + paramName + "," + theta1Str;
                                        selectionIds.Add(pieceInst.Id);
                                    }
                                }
                            }

                            //θ2は斜梁受ピース、回転ピースが対象
                            double lowerLimit = 0, upperLimit = 0;
                            if (symbolName.Contains("斜梁受"))
							{
                                lowerLimit = syabariukePieceLowerLimitθ2;
                                upperLimit = syabariukePieceUpperLimitθ2;
                            }
                            else if (symbolName.Contains("回転"))
                            {
                                lowerLimit = kaitenPieceLowerLimitθ2;
                                upperLimit = kaitenPieceUpperLimitθ2;
                            }
                            paramName = "θ2";
                            string theta2Str = ClsRevitUtil.GetParameter(doc, pieceInst.Id, paramName);
							//double theta2Rad = ClsRevitUtil.GetParameterDouble(doc, pieceInst.Id, paramName);
							//double theta2 = UnitUtils.Convert(theta2Rad, UnitTypeId.Radians, UnitTypeId.Degrees);
							if (double.TryParse(theta2Str.Replace("°", ""), out double theta2))
							{
                                //許容範囲外ならID、パラメータ名、角度を記載する
                                if (!(ClsGeo.GEO_GE(theta2, lowerLimit) && ClsGeo.GEO_LE(theta2, upperLimit)))
                                {
                                    contents += "\r\n" + pieceInst.Id + "," + paramName + "," + theta2Str;
                                    selectionIds.Add(pieceInst.Id);
                                }
                            }
                        }
                    }

                    //斜梁仮鋼材作成
                    CreateSingleKarikouzaiSyabari(doc, newSyabariId, buzaiName, kouzaiSize);
                    //斜梁は1点配置で特殊なため
                    continue;
                }
                if(buzaiName == "斜梁繋ぎ材")
                {
                    var cst = new ClsSyabariTsunagizaiBase();
                    cst.SetClassParameter(doc, baseID);
                    var syabariInsecList = ClsSyabariBase.GetIntersectionBase(doc, baseID);

                    //斜梁繋ぎ材は1点配置で特殊なため
                    if(1 < syabariInsecList.Count)
                        cst.CreateSyabariTsunagizai(doc, syabariInsecList[0], syabariInsecList[1], baseID);
                    continue;
                }
                if (buzaiName == "斜梁受け材")
                {
                    var csu = new ClsSyabariUkeBase();
                    csu.SetClassParameter(doc, baseID);
                    var syabariInsecList = ClsSyabariBase.GetIntersectionBase(doc, baseID);

                    //一番低い斜梁の選定
                    ElementId idSyabari = null;
                    for (int i = 0; i < syabariInsecList.Count; i++)
                    {
                        if (i == 0)
                        {
                            idSyabari = syabariInsecList[i];
                            continue;
                        }
                        var syabariInst = doc.GetElement(idSyabari) as FamilyInstance;
                        var inster = doc.GetElement(syabariInsecList[i]) as FamilyInstance;
                        if (syabariInst.HandOrientation.Z > inster.HandOrientation.Z)
                            idSyabari = syabariInsecList[i];
                    }

                    if (idSyabari == null)
                        continue;

                    //斜梁繋ぎ材は1点配置で特殊なため
                    csu.CreateSyabariUke(doc, idSyabari, baseID);
                    continue;
                }
                //端部部品作成(なしの場合は仮のものを作成して後に削除)

                if (kousei != DOUBLE)
                {
                    if (GetSingleFamilySymbol(ref doc, baseID, kouzaiType, buzaiName, kouzaiSize, out FamilySymbol sym, ref dKouzaiSize))
                        CreateSingleKarikouzai(ref doc, baseID, sym, dKouzaiSize, pS, pE);
                }
                else
                {
                    if (GetDoubleFamilySymbol(doc, kouzaiType, buzaiName, kouzaiSizeUP, kouzaiSizeDN, out List<FamilySymbol> symList, out List<double> kouzaiSizeList))
                        CreateDoubleKariKouzai(doc, baseID, symList, kouzaiSizeList, pS, pE);

                }


                //ここで重複している場合、警告メッセージが表示される
            }

            return true;
        }

        /// <summary>
        ///図面中の斜梁火打ちベースを全取得し、oldShabariIdに紐づけられている斜梁火打ベースを取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="oldShabariId"></param>
        /// <returns></returns>
        public static  List<ElementId> GetTargetShabariHiuchi(Document doc, ElementId oldShabariId)
        {
            List<ElementId> res = new List<ElementId>();


            List<ElementId> temp = ClsSyabariHiuchiBase.GetAllBaseList(doc);

            foreach (ElementId id in temp)
            {
                string shabariBaseIdStr = string.Empty;

                if (!ClsSyabariHiuchiBase.GetShabariBaseIdString(doc, id, ref shabariBaseIdStr))
                {
                    //ベースデータが不正
                    continue;
                }

                if(oldShabariId.ToString() == shabariBaseIdStr)
                {
                    res.Add(id);
                }
            }

            return res;
        }

        public static bool CreateShabariHiuchi(Document doc,ElementId hiuchibaseid,ElementId shabariBaseId)
        {
            try
            {
                var csh = new ClsSyabariHiuchiBase();
                //既存のデータをクラスにセット
                csh.SetClassParameter(doc, hiuchibaseid);

                string haraokoshiOrModellineIdStr = string.Empty;
                //string shabariBaseIdStr = string.Empty;

                //if (!ClsSyabariHiuchiBase.GetShabariBaseIdString(doc, hiuchibaseid, ref shabariBaseIdStr))
                //{
                //    //ベースデータが不正
                //    return false;
                //}

                if (!ClsSyabariHiuchiBase.GetHaraokoshiBaseIdString(doc, hiuchibaseid, ref haraokoshiOrModellineIdStr))
                {
                    //ベースデータが不正
                    return false;
                }

                //int intIds = int.Parse(shabariBaseIdStr);
                //ElementId shabId = new ElementId(intIdh); 
                int intIdh = int.Parse(haraokoshiOrModellineIdStr);

                ElementId haraId = new ElementId(intIdh);


                var haraLine = ClsYMSUtil.GetBaseLine(doc, haraId);
                var hiuchiLine = ClsYMSUtil.GetBaseLine(doc, hiuchibaseid);

                XYZ interPnt = FindIntersectionAfterZAlignment(haraLine, hiuchiLine);
                if (interPnt == null)
                {
                    return false;
                }

                if (!csh.CreateSyabariHiuchiBaseForKarikouzai(doc, haraId, interPnt, shabariBaseId))
                {
                    return false;
                }

                using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                {
                    t.Start();
                    ClsRevitUtil.Delete(doc, hiuchibaseid);
                    t.Commit();
                }

            }
            catch (Exception)
            {

                return false;
            }
            return true;
        }

        public static XYZ FindIntersectionAfterZAlignment(Line lineA, Line lineB)
        {
            if (lineA == null || lineB == null)
                return null;

            // --- Step ①: BのZ座標をAに合わせる ---
            double targetZ = lineA.GetEndPoint(0).Z;
            XYZ startB = new XYZ(lineB.GetEndPoint(0).X, lineB.GetEndPoint(0).Y, targetZ);
            XYZ endB = new XYZ(lineB.GetEndPoint(1).X, lineB.GetEndPoint(1).Y, targetZ);

            // 無限大に延長するため、方向ベクトルを作る
            XYZ directionB = (endB - startB).Normalize();
            Line extendedB = Line.CreateUnbound(startB, directionB);

            // 同様に、Aも無限線にしておく（念のため）
            XYZ startA = lineA.GetEndPoint(0);
            XYZ endA = lineA.GetEndPoint(1);
            XYZ directionA = (endA - startA).Normalize();
            Line extendedA = Line.CreateUnbound(startA, directionA);

            // --- Step ②: 交点を探す ---
            IntersectionResultArray resultArray;
            SetComparisonResult result = extendedA.Intersect(extendedB, out resultArray);

            if (result == SetComparisonResult.Overlap && resultArray != null && resultArray.Size > 0)
            {
                IntersectionResult intersection = resultArray.get_Item(0);
                return intersection.XYZPoint;
            }
            else
            {
                return null; // 交点なし
            }
        }
        public static bool DeleteKariKouzai(Document doc)
        {
            //仮鋼材部品名を取得
            List<string> kariKouzaiList = new List<string>();
            kariKouzaiList.AddRange(Master.ClsYamadomeCsv.GetKariFamilyNameList().ToList());
            kariKouzaiList.AddRange(Master.ClsYamadomeCsv.GetKariSyabariFamilyNameList().ToList());
            kariKouzaiList.AddRange(Master.ClsHBeamCsv.GetFamilyNameList().ToList());
            kariKouzaiList.AddRange(Master.ClsChannelCsv.GetFamilyNameList().ToList());
            kariKouzaiList.AddRange(Master.ClsAngleCsv.GetFamilyNameList().ToList());
            kariKouzaiList.AddRange(Master.ClsHiuchiCsv.GetFamilyNameList().ToList());
            // kariKouzaiList.AddRange(Master.ClsJackCsv.GetFamilyNameList().ToList()); //＃31634
            kariKouzaiList.AddRange(Master.ClsSupportPieceCsv.GetFamilyNameList().ToList());
            kariKouzaiList.AddRange(Master.ClsBurumanCSV.GetFamilyNameList().ToList());
            kariKouzaiList.AddRange(Master.ClsRikimanCSV.GetFamilyNameList().ToList());
            kariKouzaiList.AddRange(Master.ClsSyabariPieceCSV.GetFamilyNameList().ToList());
            kariKouzaiList.AddRange(Master.ClsSyabariKouzaiCSV.GetFamilyNameList().ToList());


            // フォルダのパスを指定
            string symbolFolpath = ClsZumenInfo.GetYMSFolder() + "\\支保工ファミリ\\00_仮鋼材";
            if (System.IO.Directory.Exists(symbolFolpath))
            {
                // フォルダ内のファイル一覧を取得
                string[] files = System.IO.Directory.GetFiles(symbolFolpath);

                // ファイル一覧を表示
                foreach (string file in files)
                {
                    string fileName = System.IO.Path.GetFileName(file);
                    fileName = fileName.Replace(".rfa", "");
                    kariKouzaiList.Add(fileName);
                }
            }

            //図面上の仮鋼材を全て取得
            List<ElementId> elements = ClsRevitUtil.GetAllCreatedFamilyInstanceList(doc);
            List<ElementId> targetFamilies = new List<ElementId>();
            foreach (string name in kariKouzaiList)
            {
                foreach (ElementId elem in elements)
                {
                    FamilyInstance inst = doc.GetElement(elem) as FamilyInstance;
                    if (inst != null && inst.Symbol.FamilyName == name)//ファミリ名でフィルター
                    {
                        if (ClsWarituke.GetWarituke(doc, elem) != ClsWarituke.WARITUKE && GetKariKouzaiFlag(doc, elem))
                            targetFamilies.Add(elem);
                    }
                }
            }
            //仮鋼材を削除
            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                t.Start();
                FailureHandlingOptions failOpt = t.GetFailureHandlingOptions();
                failOpt.SetFailuresPreprocessor(new WarningSwallower());
                t.SetFailureHandlingOptions(failOpt);
                ClsRevitUtil.Delete(doc, targetFamilies);
                t.Commit();
            }

            List<Tuple<string, string>> idPairs = new List<Tuple<string, string>>();

            //斜梁ベースは元のベースに戻す
            var syabariList = ClsSyabariBase.GetAllBaseList(doc);
            foreach(var id in syabariList)
            {
                var csh = new ClsSyabariBase();
                //既存のデータをクラスにセット
                csh.SetClassParameter(doc, id);
                //始終点を保持
                var baseLine = ClsYMSUtil.GetBaseLine(doc, id);
                //var lPoint = inst.Location as LocationPoint;
                if (!ClsSyabariBase.GetFirstPoint(doc, id, out var tmpStPoint, out var tmpEdPoint))
                    continue;
                //斜張位置調整(新しい斜梁を作成する）
                if (!ClsSyabariBase.GetCustomDataReferencePlane(doc, id, out var referencePlane))
                    continue;
                //カスタムデータを持っていない斜梁は変換しない
                if (tmpStPoint == null || tmpEdPoint == null || referencePlane == null)
                    continue;
                if (!csh.CreateSyabariBase(doc, tmpStPoint, tmpEdPoint, referencePlane, csh.m_LevelId))
                    continue;

                if (csh.m_ElementId != null)
                {
                    idPairs.Add(Tuple.Create(id.ToString(), csh.m_ElementId.ToString()));
                }

                var oldSyabariId = csh.m_ElementId;
                if (oldSyabariId != null)
                {
                    using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                    {
                        t.Start();
                        ClsRevitUtil.Delete(doc, id);
                        t.Commit();
                    }
                }
            }

            //斜梁火打ベース　#34047
            var syabariHiuchiList = ClsSyabariHiuchiBase.GetAllBaseList(doc);
            foreach (var id in syabariHiuchiList) 
            {
                string haraokoshiOrModellineIdStr = string.Empty;
                string shabariBaseIdStr = string.Empty;

                if (!ClsSyabariHiuchiBase.GetShabariBaseIdString(doc, id, ref shabariBaseIdStr))
                {
                    //ベースデータが不正
                    continue;
                }

                if (!ClsSyabariHiuchiBase.GetHaraokoshiBaseIdString(doc, id, ref haraokoshiOrModellineIdStr))
                {
                    //ベースデータが不正
                    continue;
                }

                string foundValue = null;
                foreach (var pair in idPairs)
                {
                    if (pair.Item1.ToString() == shabariBaseIdStr)
                    {
                        foundValue = pair.Item2;
                        break;
                    }
                }

                using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                {
                    t.Start();
                    ClsSyabariHiuchiBase.SetHaraokoshiBaseShabariBaseIdString(doc, id, haraokoshiOrModellineIdStr, foundValue);
                    t.Commit();
                }

                //using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                //{
                //    t.Start();
                //    ClsRevitUtil.Delete(doc, id);
                //    t.Commit();
                //}
            }
            
            return true;
        }

        /// <summary>
        /// 図面上の端部部品を全取得
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<ElementId> GetTanbubuhin(Document doc)
        {
            //部品名を取得
            List<string> kariKouzaiList = new List<string>();
            kariKouzaiList.AddRange(Master.ClsHiuchiCsv.GetFamilyNameList().ToList());
            kariKouzaiList.AddRange(Master.ClsSupportPieceCsv.GetFamilyNameList().ToList());

            // フォルダのパスを指定
            string symbolFolpath = ClsZumenInfo.GetYMSFolder() + "\\支保工ファミリ\\00_仮鋼材";
            if (System.IO.Directory.Exists(symbolFolpath))
            {
                // フォルダ内のファイル一覧を取得
                string[] files = System.IO.Directory.GetFiles(symbolFolpath);

                // ファイル一覧を表示
                foreach (string file in files)
                {
                    string fileName = System.IO.Path.GetFileName(file);
                    fileName = fileName.Replace(".rfa", "");
                    kariKouzaiList.Add(fileName);
                }
            }

            //図面上の端部部品を全て取得
            List<ElementId> elements = ClsRevitUtil.GetAllCreatedFamilyInstanceList(doc);
            List<ElementId> targetFamilies = new List<ElementId>();
            foreach (string name in kariKouzaiList)
            {
                foreach (ElementId elem in elements)
                {
                    FamilyInstance inst = doc.GetElement(elem) as FamilyInstance;
                    if (inst != null && inst.Symbol.FamilyName == name)//ファミリ名でフィルター
                    {
                        targetFamilies.Add(elem);
                    }
                }
            }


            return targetFamilies;
        }


        public static (XYZ, XYZ) GetHaraokoshiLine(Document doc, ElementId id, double dKouzaiSize, ClsHaraokoshiBase.WinLose WinLose, ClsHaraokoshiBase.SideNum yoko)
        {
            FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
            LocationCurve lCurve = inst.Location as LocationCurve;
            XYZ tmpStPoint = lCurve.Curve.GetEndPoint(0);
            XYZ tmpEdPoint = lCurve.Curve.GetEndPoint(1);
            ElementId levelId = inst.Host.Id;

            //図面上の腹起ベースを全て取得
            List<ElementId> haraIdList = ClsHaraokoshiBase.GetAllHaraokoshiBaseList(doc, levelId);


            //内外判定のための向き
            XYZ Direction = Line.CreateBound(tmpStPoint, tmpEdPoint).Direction;

            //腹起ベースとの交点を探す
            Curve cvHaraBase = Line.CreateBound(tmpStPoint, tmpEdPoint);
            List<XYZ> insecList = new List<XYZ>();
            List<double> insecKouzaiSizeList = new List<double>();
            List<ClsHaraokoshiBase.SideNum> insecSideNumList = new List<ClsHaraokoshiBase.SideNum>();
            foreach (ElementId haraId in haraIdList)
            {
                FamilyInstance haraokoshiShinInst = doc.GetElement(haraId) as FamilyInstance;
                LocationCurve lCurveHara = haraokoshiShinInst.Location as LocationCurve;
                if (lCurveHara != null)
                {
                    Curve cvHara = lCurveHara.Curve;

                    XYZ insec = ClsRevitUtil.GetIntersection(cvHaraBase, cvHara);

                    //入隅か判定
                    bool bIrizumi = false;
                    if (!ClsHaraokoshiBase.CheckIrizumi(cvHaraBase, cvHara, ref bIrizumi))
                    {
                        continue;
                    }
                    if (!bIrizumi)
                    {
                        insec = AdjustIrizumiIntersection(cvHaraBase, insec, dKouzaiSize);
                    }

                    if (insec != null)
                    {
                        insecList.Add(insec);
                        insecKouzaiSizeList.Add(ClsRevitUtil.CovertToAPI(Master.ClsYamadomeCsv.GetWidth(ClsRevitUtil.GetParameter(doc, haraId, KOUZAISIZE))));
                        //if (!bIrizumi)//出隅の場合は接続する腹起がシングルダブルを問わないためシングルで統一
                        //    insecSideNumList.Add(ClsHaraokoshiBase.SideNum.Single);
                        //else
                        //    insecSideNumList.Add(ClsRevitUtil.GetParameter(doc, haraId, "横本数") == "シングル" ? ClsHaraokoshiBase.SideNum.Single : ClsHaraokoshiBase.SideNum.Double);
                        insecSideNumList.Add(ClsRevitUtil.GetParameter(doc, haraId, "横本数") == "シングル" ? ClsHaraokoshiBase.SideNum.Single : ClsHaraokoshiBase.SideNum.Double);
                    }
                }
            }

            //交点が1つ以上ある場合は交点で腹起仮鋼材を作成する
            if (1 <= insecList.Count)
            {
                for (int i = 0; i < insecList.Count; i++)
                {
                    //勝ちの場合交点から鋼材サイズ分伸ばす
                    double kouzaisize = insecKouzaiSizeList[i];
                    if (insecSideNumList[i] == ClsHaraokoshiBase.SideNum.Double)
                    {
                        kouzaisize *= 2;
                    }
                    if (WinLose != ClsHaraokoshiBase.WinLose.Win)
                    {
                        kouzaisize = 0.0;
                    }
                    double distanceToStart = insecList[i].DistanceTo(tmpStPoint);
                    double distanceToEnd = insecList[i].DistanceTo(tmpEdPoint);
                    //鋼材の重なりを考慮してサイズ分大きくする

                    if (distanceToStart < distanceToEnd)
                    {
                        // 判定したい点は始点に近いです
                        tmpStPoint = new XYZ(insecList[i].X - (kouzaisize * Direction.X),
                                 insecList[i].Y - (kouzaisize * Direction.Y),
                                 insecList[i].Z);
                    }
                    else
                    {
                        // 判定したい点は終点に近いです
                        tmpEdPoint = new XYZ(insecList[i].X + (kouzaisize * Direction.X),
                                     insecList[i].Y + (kouzaisize * Direction.Y),
                                     insecList[i].Z);
                    }

                }

            }
            //壁側に位置をずらす
            tmpStPoint = new XYZ(tmpStPoint.X + (dKouzaiSize * Direction.Y),
                                 tmpStPoint.Y - (dKouzaiSize * Direction.X),
                                 tmpStPoint.Z);
            tmpEdPoint = new XYZ(tmpEdPoint.X + (dKouzaiSize * Direction.Y),
                                 tmpEdPoint.Y - (dKouzaiSize * Direction.X),
                                 tmpEdPoint.Z);

            return (tmpStPoint, tmpEdPoint);
        }
        private static XYZ AdjustIrizumiIntersection(Curve cv, XYZ insec, double dKouzaiSize)
        {
            XYZ adjustPoint;
            XYZ tmpStPoint = cv.GetEndPoint(0);
            XYZ tmpEdPoint = cv.GetEndPoint(1);
            //内外判定のための向き
            XYZ Direction = Line.CreateBound(tmpStPoint, tmpEdPoint).Direction;
            double distanceToStart = insec.DistanceTo(tmpStPoint);
            double distanceToEnd = insec.DistanceTo(tmpEdPoint);
            //鋼材の重なりを考慮してサイズ分大きくする
            if (distanceToStart < distanceToEnd)
            {
                // 判定したい点は始点に近いです
                adjustPoint = new XYZ(insec.X + (dKouzaiSize * 2 * Direction.X),
                         insec.Y + (dKouzaiSize * 2 * Direction.Y),
                         insec.Z);
            }
            else
            {
                // 判定したい点は終点に近いです
                adjustPoint = new XYZ(insec.X - (dKouzaiSize * 2 * Direction.X),
                             insec.Y - (dKouzaiSize * 2 * Direction.Y),
                             insec.Z);
            }

            return adjustPoint;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id">ベースID</param>
        /// <param name="kouzaiSym"></param>
        /// <param name="dKouzaiSize"></param>
        /// <param name="pointS"></param>
        /// <param name="pointE"></param>
        /// <returns></returns>
        private static bool CreateSingleKarikouzai(ref Document doc, ElementId id, FamilySymbol kouzaiSym, double dKouzaiSize, XYZ pointS = null, XYZ pointE = null)
        {
            string dan = ClsRevitUtil.GetParameter(doc, id, "段");
            FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
            ElementId levelID = inst.Host.Id;
            string buzaiShinName = inst.Name;

            bool bHara = false;
            LocationCurve lCurve = inst.Location as LocationCurve;
            if (lCurve == null)
            {
                return false;
            }

            XYZ tmpStPoint = null;
            if (pointS == null) tmpStPoint = lCurve.Curve.GetEndPoint(0);
            else tmpStPoint = pointS;

            XYZ tmpEdPoint = null;
            if (pointE == null) tmpEdPoint = lCurve.Curve.GetEndPoint(1);
            else tmpEdPoint = pointE;
 
            //サイズ差の調整
            ClsYMSUtil.GetDifferenceWithAllBase(doc, id, out double diff, out double diff2);
            double size = 0.0;
            if (dan == "上段")
            {
                size = dKouzaiSize + diff;
            }
            else if (dan == "下段")
            {
                size = -dKouzaiSize - diff;
            }

            //size /= 2;
            //検証用
            //if (ClsRevitUtil.GetParameter(doc, id, "コメント") == "TEST")
            //{
            //    string t = "breakPoint";
            //}

            //腹起は配置位置が違うため事前に位置を特定しているため※腹起に部材が乗ることは無い
            ClsHaraokoshiBase.SideNum yoko = ClsHaraokoshiBase.SideNum.Single;
            ClsHaraokoshiBase.VerticalNum tate = ClsHaraokoshiBase.VerticalNum.Single;
            FamilySymbol megaNormalSymbol = null;
            if (buzaiShinName == ClsHaraokoshiBase.baseName)//腹起の場合は壁側に仮鋼材を配置
            {
                ClsHaraokoshiBase chb = new ClsHaraokoshiBase();
                chb.SetParameter(doc, id);

                (tmpStPoint, tmpEdPoint) = GetHaraokoshiLine(doc, id, dKouzaiSize, chb.m_katimake, chb.m_yoko);

                //#33457 斜梁によってオフセットされた腹起対応
                double haraokoshiTateOffset = ClsRevitUtil.GetParameterDouble(doc, id, "ホストからのオフセット");
                //XYZ tmpXYZ = new XYZ(tmpStPoint.X, tmpStPoint.Y, haraokoshiTateOffset);
                //tmpStPoint = tmpXYZ;

                //tmpXYZ = new XYZ(tmpEdPoint.X, tmpEdPoint.Y, haraokoshiTateOffset);
                //tmpEdPoint = tmpXYZ;

                yoko = chb.m_yoko;
                tate = chb.m_tate;

                if (tate == ClsHaraokoshiBase.VerticalNum.Double)
                {
                    //ダブルのときのみ縦方向の隙間を開ける
                    if (dan == "上段")
                    {
                        size += ClsRevitUtil.CovertToAPI(chb.m_tateGap / 2);
                    }
                    else if (dan == "下段")
                    {
                        size -= ClsRevitUtil.CovertToAPI(chb.m_tateGap / 2);
                    }
                    else
                    {
                        //同段縦ダブルのとき作成段を上段にする※同段のレベルに何かある場合よけない可能性あり
                        dan = "上段";
                        size += dKouzaiSize + diff + ClsRevitUtil.CovertToAPI(chb.m_tateGap / 2);
                    }
                }

                if (kouzaiSym.FamilyName.Contains("80SMH"))
                {
                    chb.CreateKarikouzaiMegabeam(doc, tmpStPoint, tmpEdPoint, out bool magaFull);
                    //メガビームだけで腹起ベースを満たす場合
                    if (magaFull)
                        return true;
                    //40HAシンボルを読み込む
                    string buzaiName = "腹起";
                    string kouzaiSize = "40HA";
                    string familyPath = Master.ClsYamadomeCsv.GetKariFamilyPath(kouzaiSize);
                    string symbolName = ClsRevitUtil.GetFamilyName(familyPath);

                    if (!ClsRevitUtil.LoadFamilyData(doc, familyPath, out Family kouFam))
                    {
                        return false;
                    }
                    kouzaiSym = ClsRevitUtil.GetFamilySymbol(doc, symbolName, buzaiName);
                }

                if (ClsYMSUtil.GetNonObjectFlagBaseLinePoints(doc, tmpStPoint, tmpEdPoint, size))
                    if (ClsYMSUtil.GetNonObjectFlagBaseLinePoints(doc, tmpStPoint, tmpEdPoint, size, true))
                        chb.CreateHojyoPeace(doc, tmpStPoint, tmpEdPoint, haraokoshiTateOffset);
                
                XYZ hojyoS = tmpStPoint;
                XYZ hojyoE = tmpEdPoint;
                if (tate == ClsHaraokoshiBase.VerticalNum.Double)
                {
                    if (ClsYMSUtil.GetNonObjectFlagBaseLinePoints(doc, tmpStPoint, tmpEdPoint, size))
                        if (ClsYMSUtil.GetNonObjectFlagBaseLinePoints(doc, tmpStPoint, tmpEdPoint, size, true))
                            chb.CreateHojyoPeace(doc, tmpStPoint, tmpEdPoint, haraokoshiTateOffset, - 1);
                }
                if (yoko == ClsHaraokoshiBase.SideNum.Double)
                {

                    XYZ dir = Line.CreateBound(hojyoS, hojyoE).Direction;
                    hojyoS = new XYZ(hojyoS.X + (dKouzaiSize * 2 * dir.Y),
                                     hojyoS.Y - (dKouzaiSize * 2 * dir.X),
                                     hojyoS.Z);
                    hojyoE = new XYZ(hojyoE.X + (dKouzaiSize * 2 * dir.Y),
                                     hojyoE.Y - (dKouzaiSize * 2 * dir.X),
                                     hojyoE.Z);
                    if (ClsYMSUtil.GetNonObjectFlagBaseLinePoints(doc, hojyoS, hojyoE, size))
                        if (ClsYMSUtil.GetNonObjectFlagBaseLinePoints(doc, tmpStPoint, tmpEdPoint, size, true))
                            chb.CreateHojyoPeace(doc, hojyoS, hojyoE, haraokoshiTateOffset);
                }
                if (tate == ClsHaraokoshiBase.VerticalNum.Double && yoko == ClsHaraokoshiBase.SideNum.Double)
                {
                    if (ClsYMSUtil.GetNonObjectFlagBaseLinePoints(doc, hojyoS, hojyoE, size))
                        if (ClsYMSUtil.GetNonObjectFlagBaseLinePoints(doc, tmpStPoint, tmpEdPoint, size, true))
                            chb.CreateHojyoPeace(doc, hojyoS, hojyoE, haraokoshiTateOffset ,- 1);
                }
                //chb.CreateGapTyousei(doc, tmpStPoint, tmpEdPoint, dKouzaiSize);
            }

            // #31699 切梁つぎの場合は始終点を接続している切梁腹起しの外面にずらす
            if (buzaiShinName == ClsKiribariTsugiBase.baseName)
            {
                //切梁ベースで交叉してるものを探す　※腹起しベースも接触可能性はあるが位置修正の必要が無いので無視
                List<ElementId> lstHari = YMS.ClsYMSUtil.GetIntersectionKiribariBase(doc, id);

                ElementId idA = null;
                ElementId idB = null;
                double dSMin = 0;
                double dEMin = 0;
                if (lstHari.Count == 1)
                {
                    //一本しか取れてない
                    Element el = doc.GetElement(lstHari[0]);
                    LocationCurve lCurve2 = el.Location as LocationCurve;
                    XYZ p = ClsRevitUtil.GetIntersection(lCurve.Curve, lCurve2.Curve);

                    double dS = tmpStPoint.DistanceTo(p);
                    double dE = tmpEdPoint.DistanceTo(p);
                    if (ClsGeo.GEO_GE(dS, dE))
                    {
                        idB = lstHari[0];
                    }
                    else
                    {
                        idA = lstHari[0];
                    }
                }
                else
                {
                    foreach (ElementId idtmp in lstHari)
                    {
                        Element el = doc.GetElement(idtmp);
                        LocationCurve lCurve2 = el.Location as LocationCurve;
                        XYZ p = ClsRevitUtil.GetIntersection(lCurve.Curve, lCurve2.Curve);

                        double dS = tmpStPoint.DistanceTo(p);
                        double dE = tmpEdPoint.DistanceTo(p);
                        if (idA == null || dSMin > dS)
                        {
                            idA = idtmp;
                            dSMin = dS;
                        }

                        if (idB == null || dEMin > dE)
                        {
                            idB = idtmp;
                            dEMin = dE;
                        }
                    }
                }

                if (idA != null)
                {
                    //始点側に接触してる切梁ベース有
                    double kiribariHaba = ClsYMSUtil.GetKouzaiHabaFromBase(doc, idA) / 2;
                    tmpStPoint = ClsRevitUtil.MoveCoordinates(tmpStPoint, tmpEdPoint, ClsRevitUtil.ConvertDoubleGeo2Revit(kiribariHaba));
                }

                if (idB != null)
                {
                    //始点側に接触してる切梁ベース有
                    double kiribariHaba = ClsYMSUtil.GetKouzaiHabaFromBase(doc, idB) / 2;
                    //終点側に接触してる切梁ベース有
                    tmpEdPoint = ClsRevitUtil.MoveCoordinates(tmpEdPoint, tmpStPoint, ClsRevitUtil.ConvertDoubleGeo2Revit(kiribariHaba));
                }
            }

            //#33457 斜梁によってオフセットされた腹起対応
            if (buzaiShinName == ClsHaraokoshiBase.baseName)
            {
                double haraokoshiTateOffset = ClsRevitUtil.GetParameterDouble(doc, id, "ホストからのオフセット");
                size += haraokoshiTateOffset;
            }
            tmpStPoint = new XYZ(tmpStPoint.X, tmpStPoint.Y, tmpStPoint.Z + size);// / 2);
            tmpEdPoint = new XYZ(tmpEdPoint.X, tmpEdPoint.Y, tmpEdPoint.Z + size);// / 2); ;

            //仮鋼材の上のファミリが乗っていないポイントList
            List<List<XYZ>> notPojntLists = ClsYMSUtil.GetNonObjectBaseLinePoints(doc, tmpStPoint, tmpEdPoint);
            if (notPojntLists.Count == 0)
                return false;

            //割付残長さが100以下の場合は返す
            if (notPojntLists.Count == 1)
            {
                List<XYZ> notPojntList = notPojntLists[0];
                Curve cvReminder = Line.CreateBound(notPojntList[0], notPojntList[1]);

                if ((cvReminder as Line).Length <= ClsRevitUtil.CovertToAPI(100))
                    return false;
            }
            if (buzaiShinName == ClsKiribariBase.baseName)
            {
                List<string> jackNameList = Master.ClsJackCsv.GetFamilyNameList().ToList();
                jackNameList.AddRange(Master.ClsJackCoverCSV.GetFamilyNameList().ToList());
                ClsKiribariBase ckb = new ClsKiribariBase();
                ckb.SetParameter(doc, id);
                if (kouzaiSym.FamilyName.Contains("60SMH"))
                {
                    //線分上のジャッキを全て削除
                    List<ElementId> jackIds = ClsYMSUtil.DeleteObjectBaseLinePoints(doc, tmpStPoint, tmpEdPoint, jackNameList);
                    foreach (ElementId jackId in jackIds)
                    {
                        ElementId jackCover = null;
                        ClsJack.GetConnectedJackCover(doc, jackId, ref jackCover);
                        using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                        {
                            t.Start();
                            ClsRevitUtil.Delete(doc, jackCover);
                            t.Commit();
                        }
                    }
                    using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                    {
                        t.Start();
                        ClsRevitUtil.Delete(doc, jackIds);
                        t.Commit();
                    }
                    //notPojntLists = ClsYMSUtil.GetNonObjectBaseLinePoints(doc, tmpStPoint, tmpEdPoint);
                    //List<XYZ> notPojntList = notPojntLists[0];
                    ckb.CreateTwinHojyoPeace(doc, tmpStPoint, ref tmpEdPoint);// notPojntList[0], notPojntList[1]);
                }
                else
                {
                    notPojntLists = ClsYMSUtil.GetNonObjectBaseLinePoints(doc, tmpStPoint, tmpEdPoint, jackNameList);

                    List<XYZ> notPojntList = notPojntLists[0];
                    var sPoint = notPojntList[0];
                    var ePoint = notPojntList[1];
                    var hojyoLength = Line.CreateBound(sPoint, ePoint).Length;
                    notPojntLists = ClsYMSUtil.GetNonObjectBaseLinePoints(doc, tmpEdPoint, tmpStPoint, jackNameList);
                    notPojntList = notPojntLists[0];
                    //始点と逆から検索した時の終点が一致する場合はジャッキが配置されていないので始終点の入れ替えをしない
                    var bJack = true;
                    if (!ClsGeo.GEO_EQ(sPoint, notPojntList[1]))
                    {
                        if (!ClsRevitUtil.CheckNearGetEndPoint(tmpStPoint, tmpEdPoint, ePoint))
                        {
                            //notPojntLists = ClsYMSUtil.GetNonObjectBaseLinePoints(doc, tmpEdPoint, tmpStPoint, jackNameList);
                            //notPojntList = notPojntLists[0];
                            sPoint = notPojntList[0];
                            //ePoint = notPojntList[1];
                            hojyoLength = Line.CreateBound(notPojntList[0], notPojntList[1]).Length;
                        }
                        else
                        {
                            //sPoint = notPojntList[0];
                            ePoint = notPojntList[1];
                        }
                    }
                    else
                        bJack = false;
                    var changeHojyoPeace = false;
                    //ジャッキが近い側の端部からジャッキまでの距離が300未満か判定誤差計算も含む
                    if (ClsGeo.GEO_LT(hojyoLength, ClsRevitUtil.CovertToAPI(300)))
                        changeHojyoPeace = true;
                    ckb.CreateHojyoPeace(doc, sPoint, ePoint, changeHojyoPeace, bJack);
                }
                notPojntLists = ClsYMSUtil.GetNonObjectBaseLinePoints(doc, tmpStPoint, tmpEdPoint);
            }
            if (buzaiShinName == ClsCornerHiuchiBase.baseName)
            {
                List<XYZ> notPojntList = notPojntLists[0];

                if (ClsYMSUtil.GetNonObjectFlagBaseLinePoints(doc, tmpStPoint, tmpEdPoint, size))
                    if (ClsYMSUtil.GetNonObjectFlagBaseLinePoints(doc, tmpStPoint, tmpEdPoint, size, true))
                    {
                        ClsCornerHiuchiBase chb = new ClsCornerHiuchiBase();
                        chb.SetParameter(doc, id);
                        chb.CreateHojyoPeace(doc, notPojntList[0], notPojntList[1], chb.m_HiuchiLengthSingleL, dan);//パラメータを増やすシングルも上下につく
                        notPojntLists = ClsYMSUtil.GetNonObjectBaseLinePoints(doc, tmpStPoint, tmpEdPoint);
                    }
            }
            if (buzaiShinName == ClsKiribariHiuchiBase.baseName)
            {
                List<XYZ> notPojntList = notPojntLists[0];

                if (ClsYMSUtil.GetNonObjectFlagBaseLinePoints(doc, tmpStPoint, tmpEdPoint, size))
                    if (ClsYMSUtil.GetNonObjectFlagBaseLinePoints(doc, tmpStPoint, tmpEdPoint, size, true))
                    {
                        ClsKiribariHiuchiBase ckb = new ClsKiribariHiuchiBase();
                        ckb.SetParameter(doc, id);
                        ckb.CreateHojyoPeace(doc, notPojntList[0], notPojntList[1], ckb.m_SteelSizeSingle, ckb.m_HiuchiLengthSingleL, dan);//パラメータを増やす
                        notPojntLists = ClsYMSUtil.GetNonObjectBaseLinePoints(doc, tmpStPoint, tmpEdPoint);
                    }
            }
            Curve cv = null;

            
            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                //鋼材作図処理
                foreach (List<XYZ> notPojntList in notPojntLists)
                {
                    //鋼材サイズ100以下は作成しない
                    cv = Line.CreateBound(notPojntList[0], notPojntList[1]);

                    if ((cv as Line).Length <= ClsRevitUtil.CovertToAPI(100))
                        continue;

                    t.Start();
                    FailureHandlingOptions failOpt = t.GetFailureHandlingOptions();
                    failOpt.SetFailuresPreprocessor(new WarningSwallower());
                    t.SetFailureHandlingOptions(failOpt);

                    //ElementId kariId = ClsRevitUtil.GetParameterElementId(doc, id, "仮鋼材");
                    ElementId CreatedID = null;
                    if (kouzaiSym.FamilyName.Contains("80SMH"))
                    {
                        if (!CreateSingleKarikouzaiMegabeam(ref doc, id, cv, dan, kouzaiSym, megaNormalSymbol))
                        {
                            t.RollBack();
                            continue;
                        }
                    }
                    else
                    {
                        //通常作成
                        CreatedID = ClsRevitUtil.Create(doc, cv, levelID, kouzaiSym);//例外処理

                        //腹起ダブル用
                        Curve yokoCv = cv;
                        if (yoko == ClsHaraokoshiBase.SideNum.Double)
                        {
                            XYZ hojyoS = notPojntList[0];
                            XYZ hojyoE = notPojntList[1];
                            XYZ dir = Line.CreateBound(hojyoS, hojyoE).Direction;
                            hojyoS = new XYZ(hojyoS.X + (dKouzaiSize * 2 * dir.Y),
                                             hojyoS.Y - (dKouzaiSize * 2 * dir.X),
                                             hojyoS.Z);
                            hojyoE = new XYZ(hojyoE.X + (dKouzaiSize * 2 * dir.Y),
                                             hojyoE.Y - (dKouzaiSize * 2 * dir.X),
                                             hojyoE.Z);
                            yokoCv = Line.CreateBound(hojyoS, hojyoE);
                            ElementId yokoCreatedID = ClsRevitUtil.Create(doc, yokoCv, levelID, kouzaiSym);
                            SetBaseId(doc, yokoCreatedID, id);
                            SetKariKouzaiFlag(doc, yokoCreatedID);

                            //選択された芯の段によって調整
                            if (dan == "上段")
                            {
                                ClsRevitUtil.SetParameter(doc, yokoCreatedID, "ホストからのオフセット", size);
                            }
                            else if (dan == "下段")
                            {
                                ClsRevitUtil.SetParameter(doc, yokoCreatedID, "ホストからのオフセット", size);
                            }
                            else if (dan == "同段")
                            {
                                ClsRevitUtil.SetParameter(doc, yokoCreatedID, "ホストからのオフセット", size);
                            }
                        }
                        if (tate == ClsHaraokoshiBase.VerticalNum.Double)
                        {
                            ElementId tateCreatedID = ClsRevitUtil.Create(doc, cv, levelID, kouzaiSym);
                            SetBaseId(doc, tateCreatedID, id);
                            SetKariKouzaiFlag(doc, tateCreatedID);

                            //選択された芯の段によって調整
                            if (dan == "上段")
                            {
                                ClsRevitUtil.SetParameter(doc, tateCreatedID, "ホストからのオフセット", -size);
                            }
                            else if (dan == "下段")
                            {
                                ClsRevitUtil.SetParameter(doc, tateCreatedID, "ホストからのオフセット", -size);
                            }
                            else if (dan == "同段")
                            {
                                ClsRevitUtil.SetParameter(doc, tateCreatedID, "ホストからのオフセット", -size);
                            }
                        }
                        if (tate == ClsHaraokoshiBase.VerticalNum.Double && yoko == ClsHaraokoshiBase.SideNum.Double)
                        {
                            ElementId tateyokoCreatedID = ClsRevitUtil.Create(doc, yokoCv, levelID, kouzaiSym);
                            SetBaseId(doc, tateyokoCreatedID, id);
                            SetKariKouzaiFlag(doc, tateyokoCreatedID);

                            //選択された芯の段によって調整
                            if (dan == "上段")
                            {
                                ClsRevitUtil.SetParameter(doc, tateyokoCreatedID, "ホストからのオフセット", -size);
                            }
                            else if (dan == "下段")
                            {
                                ClsRevitUtil.SetParameter(doc, tateyokoCreatedID, "ホストからのオフセット", -size);
                            }
                            else if (dan == "同段")
                            {
                                ClsRevitUtil.SetParameter(doc, tateyokoCreatedID, "ホストからのオフセット", -size);
                            }
                        }
                        SetBaseId(doc, CreatedID, id);
                        SetKariKouzaiFlag(doc, CreatedID);
                        ClsRevitUtil.SetMojiParameter(doc, id, "仮鋼材", CreatedID);


                        //選択された芯の段によって調整
                        if (dan == "上段")
                        {
                            ClsRevitUtil.SetParameter(doc, CreatedID, "ホストからのオフセット", size);
                        }
                        else if (dan == "下段")
                        {
                            ClsRevitUtil.SetParameter(doc, CreatedID, "ホストからのオフセット", size);
                        }
                        else if (dan == "同段")
                        {
                            ClsRevitUtil.SetParameter(doc, CreatedID, "ホストからのオフセット", size);
                        }

                        //#31695
                        if (buzaiShinName == ClsKiribariUkeBase.baseName)
                        {
                            double sageryou = dKouzaiSize;

                            //交叉してる切梁ベースを探す
                            List<ElementId> lstHari = YMS.ClsYMSUtil.GetIntersectionKiribariBase(doc, id);
                            if (lstHari.Count > 0)
                            {
                                ClsKiribariBase clsKiri = new ClsKiribariBase();
                                clsKiri.SetParameter(doc, lstHari[0]);
                                string strSize = clsKiri.m_kouzaiSize;
                                sageryou += ClsRevitUtil.CovertToAPI(Master.ClsYamadomeCsv.GetWidth(strSize));
                                sageryou += diff;
                            }
                            ClsRevitUtil.SetParameter(doc, CreatedID, "ホストからのオフセット", -sageryou);
                        }
                    }

                    t.Commit();
                }
            }
            return true;
        }

        private static bool CreateSingleKarikouzaiMegabeam(ref Document doc, ElementId id, Curve cv, string dan, FamilySymbol kouzaiSym, FamilySymbol symbol)
        {
            FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
            ElementId levelID = inst.Host.Id;
            string buzaiShinName = inst.Name;

            bool isHaraBase = buzaiShinName == ClsHaraokoshiBase.baseName;
            bool isMegaBeam = kouzaiSym.FamilyName.Contains("80SMH");
            if (!isHaraBase && !isMegaBeam)
            {
                return false;
            }

            XYZ tmpStPoint = cv.GetEndPoint(0);

            XYZ tmpEdPoint = cv.GetEndPoint(1);

            // 線分の情報を取得
            XYZ lineStartPoint = tmpStPoint;
            XYZ lineEndPoint = tmpEdPoint;
            XYZ lineMidPoint = (tmpStPoint + tmpEdPoint) * 0.5;
            double lineLength = lineEndPoint.DistanceTo(lineStartPoint);
            XYZ directionVector = (lineEndPoint - lineStartPoint).Normalize();

            // メガビームの本数を取得
            double megabeamLength = ClsRevitUtil.ConvertDoubleGeo2Revit(9000.0);
            int megaBeamCount = 0;
            if (isMegaBeam)
            {
                string megaHousu = ClsRevitUtil.GetParameter(doc, id, "メガビーム本数");
                megaBeamCount = int.Parse(megaHousu);
            }
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

            ////// 端部の腹起ダブルを作成
            ////string kouzaiType = "主材";
            //string buzaiName = "腹起";
            //string kouzaiSize = "40HA";
            ////string kouzaiSize = "20HA";

            //string familyPath = Master.ClsYamadomeCsv.GetKariFamilyPath(kouzaiSize);
            //string symbolName = ClsRevitUtil.GetFamilyName(familyPath);

            //if (!ClsRevitUtil.LoadFamilyData(doc, familyPath, out Family kouFam))
            //{
            //    return false;
            //}
            //FamilySymbol symbol = ClsRevitUtil.GetFamilySymbol(doc, symbolName, buzaiName);
            ////GetSingleFamilySymbol(ref doc, id, kouzaiType, buzaiName, kouzaiSize, out FamilySymbol symbol, ref dKouzaiSize);

            double dKouzaiSize = 0.0;
            if (dan == "上段")
            {
                dKouzaiSize = ClsRevitUtil.ConvertDoubleGeo2Revit(200.0);
            }
            else if (dan == "下段")
            {
                dKouzaiSize = ClsRevitUtil.ConvertDoubleGeo2Revit(-200.0);
            }

            Curve cv11 = Line.CreateBound(lineStartPoint, lineStartPoint + (directionVector * surplusLength / 2));
            ElementId haraId11 = ClsRevitUtil.Create(doc, cv11, levelID, symbol);
            ElementTransformUtils.MoveElement(doc, haraId11, leftVector * ClsRevitUtil.ConvertDoubleGeo2Revit(200.0));
            ClsRevitUtil.SetParameter(doc, haraId11, "ホストからのオフセット", dKouzaiSize);

            Curve cv12 = Line.CreateBound(lineStartPoint, lineStartPoint + (directionVector * surplusLength / 2));
            ElementId haraId12 = ClsRevitUtil.Create(doc, cv12, levelID, symbol);
            ElementTransformUtils.MoveElement(doc, haraId12, leftVector * ClsRevitUtil.ConvertDoubleGeo2Revit(600.0));
            ClsRevitUtil.SetParameter(doc, haraId12, "ホストからのオフセット", dKouzaiSize);

            Curve cv21 = Line.CreateBound(lineEndPoint, lineEndPoint - (directionVector * surplusLength / 2));
            ElementId haraId21 = ClsRevitUtil.Create(doc, cv21, levelID, symbol);
            ElementTransformUtils.MoveElement(doc, haraId21, leftVector * ClsRevitUtil.ConvertDoubleGeo2Revit(200.0));
            ClsRevitUtil.SetParameter(doc, haraId21, "ホストからのオフセット", dKouzaiSize);

            Curve cv22 = Line.CreateBound(lineEndPoint, lineEndPoint - (directionVector * surplusLength / 2));
            ElementId haraId22 = ClsRevitUtil.Create(doc, cv22, levelID, symbol);
            ElementTransformUtils.MoveElement(doc, haraId22, leftVector * ClsRevitUtil.ConvertDoubleGeo2Revit(600.0));
            ClsRevitUtil.SetParameter(doc, haraId22, "ホストからのオフセット", dKouzaiSize);

            // メガビーム腹起を作成
            for (int i = 0; i < megaBeamCount; i++)
            {
                // シンボルを作成
                ElementId CreatedID = ClsRevitUtil.Create(doc, insertPoint, levelID, kouzaiSym);
                //選択された芯の段によって調整
                ClsRevitUtil.SetParameter(doc, CreatedID, ClsGlobal.m_refLvTop, dKouzaiSize);
                SetKariKouzaiFlag(doc, CreatedID);
                // シンボルを回転
                rotationAxis = Line.CreateBound(insertPoint, insertPoint + XYZ.BasisZ);    // Z軸周りに回転
                ElementTransformUtils.RotateElement(doc, CreatedID, rotationAxis, radians);

                // シンボル腹起ベースに合うように移動
                ElementTransformUtils.MoveElement(doc, CreatedID, leftVector * ClsRevitUtil.ConvertDoubleGeo2Revit(ClsYMSUtil.GetKouzaiHabaFromBase(doc, id)));

                insertPoint += directionVector * megabeamLength;
            }

            return true;
        }

        // 斜梁火打で呼び出したいので private -> public へ変更 M.Sakuraba
        public static bool CreateSingleKarikouzaiSyabari(Document doc, ElementId id, string buzaiName, string kouzaiSize, XYZ pointS = null)
        {
            //string symbolFolpath = Parts.ClsZumenInfo.GetYMSFolder();
            //string familyPath = System.IO.Path.Combine(symbolFolpath, "仮鋼材1点配置\\" + "主材_" + kouzaiSize + ".rfa");
            string familyPath = Master.ClsYamadomeCsv.GetKariFamilyPath(kouzaiSize, true);//本来はこっち
            string symbolName = ClsRevitUtil.GetFamilyName(familyPath);

            double sunpou2 = Master.ClsYamadomeCsv.GetWidth(kouzaiSize);
            double dKouzaiSize = ClsRevitUtil.CovertToAPI(sunpou2 / 2);

            if (!ClsRevitUtil.LoadFamilyData(doc, familyPath, out Family kouFam))
            {
                return false;
            }
            FamilySymbol sym = ClsRevitUtil.GetFamilySymbol(doc, symbolName, buzaiName);
            if (sym == null)
                return false;

            string dan = ClsRevitUtil.GetParameter(doc, id, "段");
            FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
            var referencePlane = inst.Host as ReferencePlane;
            var reference = referencePlane.GetReference();
            //ElementId levelID = inst.Host.Id;
            var dist = ClsRevitUtil.GetParameterDouble(doc, id, "長さ");

            var lPoint = inst.Location as LocationPoint;
            if (lPoint == null)
            {
                return false;
            }

            XYZ tmpStPoint = null;
            if (pointS == null) tmpStPoint = lPoint.Point;
            else tmpStPoint = pointS;

            XYZ tmpEdPoint = tmpStPoint + dist * inst.HandOrientation;

            //サイズ差の調整
            ClsYMSUtil.GetDifferenceWithAllBase(doc, id, out double diff, out double diff2);
            double size = 0.0;
            if (dan == "上段")
            {
                size = dKouzaiSize + diff;
            }
            else if (dan == "下段")
            {
                size = -dKouzaiSize - diff;
            }

            tmpStPoint = new XYZ(tmpStPoint.X, tmpStPoint.Y, tmpStPoint.Z + size);// / 2);
            tmpEdPoint = new XYZ(tmpEdPoint.X, tmpEdPoint.Y, tmpEdPoint.Z + size);// / 2); ;

            //仮鋼材の上のファミリが乗っていないポイントList
            List<List<XYZ>> notPojntLists = ClsYMSUtil.GetNonObjectBaseLinePoints(doc, tmpStPoint, tmpEdPoint);
            if (notPojntLists.Count == 0)
                return false;

            //割付残長さが100以下の場合は返す
            if (notPojntLists.Count == 1)
            {
                List<XYZ> notPojntList = notPojntLists[0];
                Curve cvReminder = Line.CreateBound(notPojntList[0], notPojntList[1]);

                if ((cvReminder as Line).Length <= ClsRevitUtil.CovertToAPI(100))
                    return false;
            }
            
            Curve cv = null;

            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                //鋼材作図処理
                foreach (List<XYZ> notPojntList in notPojntLists)
                {
                    //鋼材サイズ100以下は作成しない
                    cv = Line.CreateBound(notPojntList[0], notPojntList[1]);

                    if ((cv as Line).Length <= ClsRevitUtil.CovertToAPI(100))
                        continue;

                    t.Start();
                    FailureHandlingOptions failOpt = t.GetFailureHandlingOptions();
                    failOpt.SetFailuresPreprocessor(new WarningSwallower());
                    t.SetFailureHandlingOptions(failOpt);

                    ElementId CreatedID = ClsRevitUtil.Create(doc, tmpStPoint, tmpEdPoint - tmpStPoint, referencePlane, sym);
                    if (CreatedID == null)
                        continue;

                    ClsRevitUtil.SetParameter(doc, CreatedID, "長さ", dist);
                    SetBaseId(doc, CreatedID, id);
                    SetKariKouzaiFlag(doc, CreatedID);
                    ClsRevitUtil.SetMojiParameter(doc, id, "仮鋼材", CreatedID);

                    //選択された芯の段によって調整
                    if (dan == "上段")
                    {
                        ClsRevitUtil.SetParameter(doc, CreatedID, "ホストからのオフセット", size);
                    }
                    else if (dan == "下段")
                    {
                        ClsRevitUtil.SetParameter(doc, CreatedID, "ホストからのオフセット", size);
                    }
                    ElementId levelId = ClsRevitUtil.GetParameterElementId(doc, id, "集計レベル");
                    if (levelId != null)
                    {
                        ClsRevitUtil.SetParameter(doc, CreatedID, "集計レベル", levelId);
                    }
                    
                    t.Commit();
                }
            }
            return true;
        }

        private static bool GetSingleFamilySymbol(ref Document doc, ElementId id, string kouzaiType, string buzaiName, string kouzaiSize, out FamilySymbol sym, ref double dKouzaiSize)
        {
            sym = null;
            // 主材、チャンネル、H鋼の３種類でMstを分ける
            string familyPath = null;
            string symbolName = null;

            switch (kouzaiType)
            {
                case "チャンネル":
                    {
                        familyPath = Master.ClsChannelCsv.GetFamilyPath(kouzaiSize);
                        double sunpou2 = ClsCommonUtils.ChangeStrToDbl(Master.ClsYamadomeCsv.GetKouzaiSizeSunpou(kouzaiSize, 1));
                        dKouzaiSize += ClsRevitUtil.CovertToAPI(sunpou2 / 2);
                        break;
                    }
                case "アングル":
                    {
                        familyPath = Master.ClsAngleCsv.GetFamilyPath(kouzaiSize);
                        double sunpou2 = ClsCommonUtils.ChangeStrToDbl(Master.ClsYamadomeCsv.GetKouzaiSizeSunpou(kouzaiSize, 1));
                        dKouzaiSize += ClsRevitUtil.CovertToAPI(sunpou2 / 2);
                        break;
                    }
                case "H形鋼 広幅":
                case "H形鋼 中幅":
                case "H形鋼 細幅":
                    {
                        familyPath = Master.ClsHBeamCsv.GetFamilyPath(kouzaiSize);
                        double sunpou2 = ClsCommonUtils.ChangeStrToDbl(Master.ClsYamadomeCsv.GetKouzaiSizeSunpou(kouzaiSize, 1));
                        dKouzaiSize += ClsRevitUtil.CovertToAPI(sunpou2 / 2);
                        break;
                    }
                default:
                    {
                        familyPath = Master.ClsYamadomeCsv.GetKariFamilyPath(kouzaiSize);
                        double sunpou2 = Master.ClsYamadomeCsv.GetWidth(kouzaiSize);
                        dKouzaiSize += ClsRevitUtil.CovertToAPI(sunpou2 / 2);
                        break;
                    }
            }
            symbolName = ClsRevitUtil.GetFamilyName(familyPath);

            if (!ClsRevitUtil.LoadFamilyData(doc, familyPath, out Family kouFam))
            {
                return false;
            }
            FamilySymbol kozaiSym = ClsRevitUtil.GetFamilySymbol(doc, symbolName, buzaiName);
            if (kozaiSym == null)
                return false;

            //#31709
            using (Transaction t = new Transaction(doc, "エンドプレートオフ"))
            {
                t.Start();
                FailureHandlingOptions failOpt = t.GetFailureHandlingOptions();
                failOpt.SetFailuresPreprocessor(new WarningSwallower());
                t.SetFailureHandlingOptions(failOpt);
                try
                {
                    switch (kouzaiType)
                    {
                        case "チャンネル":
                        case "H形鋼 広幅":
                        case "H形鋼 中幅":
                        case "H形鋼 細幅":
                            {
                                ClsRevitUtil.SetTypeParameter(kozaiSym, "エンドプレート", 0);
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
                catch
                { t.RollBack(); }

                t.Commit();
            }

            sym = kozaiSym;

            switch (kouzaiType)
            {
                case "チャンネル"://チャンネルは寸法ー回転が90の時は高さ取得180の時は高さ0
                    {
                        double ra = ClsRevitUtil.GetTypeParameter(sym, "回転"); 
                        double sunpou2 = ClsCommonUtils.ChangeStrToDbl(Master.ClsYamadomeCsv.GetKouzaiSizeSunpou(kouzaiSize, 1));
                        //if(ra != Math.PI)
                        //    dKouzaiSize -= ClsRevitUtil.CovertToAPI(sunpou2 / 2);
                        if (ra == Math.PI)
                            dKouzaiSize -= ClsRevitUtil.CovertToAPI(sunpou2 / 2);
                        break;
                    }
                case "アングル":
                    {
                        break;
                    }
                case "H形鋼 広幅":
                case "H形鋼 中幅":
                case "H形鋼 細幅":
                    {
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            ////鋼材サイズの高さ
            //if (sym.FamilyName.Contains("形鋼"))
            //{
            //    string sunpou = ClsYMSUtil.GetKouzaiSizeSunpou(symbolName.Replace("仮鋼材_", ""), 1);
            //    dKouzaiSize += ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(sunpou)) / 2;
            //}
            //else
            //{
            //    double sunpou2 = Master.ClsYamadomeCsv.GetWidth(kouzaiSize);
            //    dKouzaiSize += ClsRevitUtil.CovertToAPI(sunpou2) / 2;
            //}

            return true;
        }

        /// <summary>
        /// ダブル仮鋼材配置
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id">対象のベース</param
        /// <returns></returns>
        private static bool CreateDoubleKariKouzai(Document doc, ElementId id, List<FamilySymbol> symList, List<double> kouzaiSizeList, XYZ pointS = null, XYZ pointE = null)
        {
            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                ElementId levelID = inst.Host.Id;

                List<ElementId> targetFamilies = new List<ElementId>();
                targetFamilies.AddRange(ClsHaraokoshiBase.GetAllHaraokoshiBaseList(doc, levelID));
                targetFamilies.AddRange(ClsKiribariBase.GetAllKiribariBaseList(doc, levelID));

                
                string buzaiShinName = inst.Name;
                string buzaiName = buzaiShinName.Replace("ベース", "");
                Curve cvBase = (inst.Location as LocationCurve).Curve;

                //構成ダブル用
                double hLengthUP = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsRevitUtil.GetParameter(doc, id, HIUCHILENGTHDOUBLEUP)));
                double hLengthDN = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsRevitUtil.GetParameter(doc, id, HIUCHILENGTHDOUBLEDN)));
                double hLengthZR = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsRevitUtil.GetParameter(doc, id, HIUCHIZURERYOU)));
                double hojyoJudgeLength = 0.0;
                string kouzaiSizeKiribariHiuchi = string.Empty;


                //始点側
                //XYZ sPnt = cvBase.GetEndPoint(0);
                //XYZ ePnt = cvBase.GetEndPoint(1);
                XYZ sPnt = null;
                if (pointS == null) sPnt = cvBase.GetEndPoint(0);
                else sPnt = pointS;
                XYZ ePnt = null;
                if (pointE == null) ePnt = cvBase.GetEndPoint(1);
                else ePnt = pointE;

                XYZ pCheckOnCurve = cvBase.GetEndPoint(0);

                //サイズ差の調整
                ClsYMSUtil.GetDifferenceWithAllBase(doc, id, out double diff, out double diff2);


                for (int i = 0; i < symList.Count; i++)
                {
                    XYZ dir = Line.CreateBound(sPnt, ePnt).Direction;
                    foreach (ElementId tgId in targetFamilies)
                    {
                        FamilyInstance tgInst = doc.GetElement(tgId) as FamilyInstance;
                        if (inst == tgInst)
                            continue;
                        Curve tgCv = (tgInst.Location as LocationCurve).Curve;
                        if (ClsRevitUtil.IsPointOnCurve(tgCv, pCheckOnCurve))
                        {
                            //ElementId insecId = tgId;

                            string dan = ClsRevitUtil.GetParameter(doc, tgId, "段");
                            double dLength = 0;
                            double dKouzaiSize = 0;

                            if (dan == "上段")
                            {
                                //ClsRevitUtil.Create(doc, sPnt, levelID, kozaiSym1);
                                dLength = hLengthUP;//ここで端部部品分長さを足せば良いのでは//なしも考えると足すよりかはスタート位置を動かす方が良いかも
                                hojyoJudgeLength = (ClsCommonUtils.ChangeStrToDbl(ClsRevitUtil.GetParameter(doc, id, HIUCHILENGTHDOUBLEUP)));
                                kouzaiSizeKiribariHiuchi = ClsRevitUtil.GetParameter(doc, id, KOUZAISIZEDOUBLEUP);
                                dKouzaiSize = kouzaiSizeList[i] + diff;
                            }
                            else if (dan == "下段")
                            {
                                //ClsRevitUtil.Create(doc, sPnt, levelID, kozaiSym2);
                                dLength = hLengthDN;//ここで端部部品分長さを足せば良いのでは
                                hojyoJudgeLength = (ClsCommonUtils.ChangeStrToDbl(ClsRevitUtil.GetParameter(doc, id, HIUCHILENGTHDOUBLEDN)));
                                kouzaiSizeKiribariHiuchi = ClsRevitUtil.GetParameter(doc, id, KOUZAISIZEDOUBLEDN);
                                dKouzaiSize = -kouzaiSizeList[i] - diff2;
                            }
                            else
                                return false;

                            if (buzaiShinName == ClsKiribariTsugiBase.baseName)
                            {
                                if (tgInst.Name == ClsHaraokoshiBase.baseName)
                                {
                                    dLength = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsRevitUtil.GetParameter(doc, id, KOUZAILENGTHHARAOKOSHI)));
                                }
                                else if (tgInst.Name == ClsKiribariBase.baseName)
                                {
                                    dLength = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsRevitUtil.GetParameter(doc, id, KOUZAILENGTHKIRIBARI)));
                                    string tanbuBuzai = ClsRevitUtil.GetParameter(doc, id, KIRIBARISIDEBUHIN);
                                    //鋼材サイズの半分内にずらす
                                    string noSizeKiri = ClsRevitUtil.GetParameter(doc, tgId, KOUZAISIZE);
                                    double dNoSizeKiri = Master.ClsYamadomeCsv.GetWidth(noSizeKiri);
                                    dNoSizeKiri = ClsRevitUtil.CovertToAPI(dNoSizeKiri) / 2;
                                    sPnt = new XYZ(sPnt.X + dNoSizeKiri * dir.X, sPnt.Y + dNoSizeKiri * dir.Y, sPnt.Z);
                                }
                            }

                            ePnt = new XYZ(sPnt.X + dLength * dir.X, sPnt.Y + dLength * dir.Y, sPnt.Z + dKouzaiSize);// / 2);
                            sPnt = new XYZ(sPnt.X, sPnt.Y, sPnt.Z + dKouzaiSize);// / 2);

                            Curve baseCurve = Line.CreateBound(sPnt, ePnt);
                            //端部部品を外した位置の取得
                            XYZ checkPoint = ClsYMSUtil.GetNonObjectBaseLinePoint(doc, sPnt, ePnt);
                            //端部部品を外した位置が始終点にきてしまう場合終了
                            if (ClsGeo.GEO_EQ(ePnt, checkPoint))//ClsGeo.GEO_EQ(sPnt, checkPoint) ||
                                return false;

                            sPnt = checkPoint;
                            ePnt = new XYZ(sPnt.X + dLength * dir.X, sPnt.Y + dLength * dir.Y, sPnt.Z);

                            //ダブル割付済みで仮鋼材が配置されてしまうときがあるため判定を追加
                            if (!ClsRevitUtil.IsPointOnCurve(baseCurve, ePnt))
                            {
                                continue;
                            }

                            //仮鋼材の上のファミリが乗っていないポイントList
                            List<List<XYZ>> notPojntLists = ClsYMSUtil.GetNonObjectBaseLinePoints(doc, sPnt, ePnt);
                            //bool bFirst = true;
                            if (notPojntLists.Count == 0)
                                return false;

                            bool doubleFlag = false;

                            if (buzaiShinName == ClsCornerHiuchiBase.baseName)
                            {
                                List<XYZ> notPojntList = notPojntLists[0];
                                ClsCornerHiuchiBase chb = new ClsCornerHiuchiBase();
                                chb.SetParameter(doc, id);
                                chb.CreateHojyoPeace(doc, notPojntList[0], notPojntList[1], hojyoJudgeLength, dan);//パラメータを増やす
                                notPojntLists = ClsYMSUtil.GetNonObjectBaseLinePoints(doc, sPnt, ePnt);
                                if (chb.m_Kousei == ClsCornerHiuchiBase.Kousei.Double)
                                    doubleFlag = true;

                                //接続ファミリチェック
                                if(dan == "下段"　&& i == 0)
                                {
                                    dKouzaiSize = dKouzaiSize + diff2 - diff;
                                }
                                else if(dan == "上段"　&& i == 1)
                                {
                                    dKouzaiSize = dKouzaiSize - diff + diff2;
                                }
                            }
                            if (buzaiShinName == ClsKiribariHiuchiBase.baseName)
                            {
                                List<XYZ> notPojntList = notPojntLists[0];
                                ClsKiribariHiuchiBase ckb = new ClsKiribariHiuchiBase();
                                ckb.SetParameter(doc, id);
                                ckb.CreateHojyoPeace(doc, notPojntList[0], notPojntList[1], kouzaiSizeKiribariHiuchi, hojyoJudgeLength, dan);//パラメータを増やす
                                notPojntLists = ClsYMSUtil.GetNonObjectBaseLinePoints(doc, sPnt, ePnt);
                                if (ckb.m_CreateHoho == ClsKiribariHiuchiBase.CreateHoho.Double)
                                    doubleFlag = true;
                            }

                            foreach (List<XYZ> notPojntList in notPojntLists)
                            {
                                //if (bFirst)
                                //{
                                //    notPojntList[1] = new XYZ(notPojntList[0].X + dLength * dir.X, notPojntList[0].Y + dLength * dir.Y, notPojntList[0].Z);
                                //    bFirst = false;
                                //}
                                Curve cv = Line.CreateBound(notPojntList[0], notPojntList[1]);
                                if ((cv as Line).Length <= ClsRevitUtil.CovertToAPI(100))
                                    continue;
                                t.Start();
                                FailureHandlingOptions failOpt = t.GetFailureHandlingOptions();
                                failOpt.SetFailuresPreprocessor(new WarningSwallower());
                                t.SetFailureHandlingOptions(failOpt);
                                //ElementId kariId = ClsRevitUtil.GetParameterElementId(doc, id, "仮鋼材");
                                ElementId CreatedID = ClsRevitUtil.Create(doc, cv, levelID, symList[i]);//例外処理
                                ClsRevitUtil.SetMojiParameter(doc, id, "仮鋼材", CreatedID);
                                if (doubleFlag)
                                    SetBaseIdDouble(doc, CreatedID, id, dan);
                                SetBaseId(doc, CreatedID, id);
                                SetKariKouzaiFlag(doc, CreatedID);
                                ClsRevitUtil.SetParameter(doc, CreatedID, "ホストからのオフセット", dKouzaiSize);
                                t.Commit();
                            }
                            break;
                        }
                    }
                    //終点側
                    //sPnt = cvBase.GetEndPoint(1);
                    //ePnt = cvBase.GetEndPoint(0);
                    if (pointS == null) sPnt = cvBase.GetEndPoint(1);
                    else sPnt = pointE;
                    if (pointE == null) ePnt = cvBase.GetEndPoint(0);
                    else ePnt = pointS;
                    pCheckOnCurve = cvBase.GetEndPoint(1);
                }
            }
            return true;
        }
        /// <summary>
        /// ダブルのシンボルを取得
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        private static bool GetDoubleFamilySymbol(Document doc, string kouzaiType, string buzaiName, string kouzaiSizeUP, string kouzaiSizeDN, out List<FamilySymbol> symList, out List<double> kouzaiSizeList)
        {
            //FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
            symList = new List<FamilySymbol>();
            kouzaiSizeList = new List<double>();

            if (kouzaiSizeUP == string.Empty || kouzaiSizeDN == string.Empty)
                return false;

            //string kouzaiType = ClsRevitUtil.GetParameter(doc, id, KOUZAITYPE);

            //string buzaiShinName = inst.Name;
            //string buzaiName = buzaiShinName.Replace("ベース", "");

            //構成ダブル用

            //ダブルを入れる
            //kouzaiSizeUP = ClsRevitUtil.GetParameter(doc, id, KOUZAISIZEDOUBLEUP);
            //kouzaiSizeDN = ClsRevitUtil.GetParameter(doc, id, KOUZAISIZEDOUBLEDN);


            //主材、チャンネル、H鋼の３種類でMstを分ける
            double sunpou, sunpou2;
            List<string> familyPathList = new List<string>();
            switch (kouzaiType)
            {
                case "チャンネル":
                    {
                        familyPathList.Add(Master.ClsChannelCsv.GetFamilyPath(kouzaiSizeUP));
                        familyPathList.Add(Master.ClsChannelCsv.GetFamilyPath(kouzaiSizeDN));
                        sunpou = ClsCommonUtils.ChangeStrToDbl(Master.ClsYamadomeCsv.GetKouzaiSizeSunpou(kouzaiSizeUP, 1));
                        sunpou2 = ClsCommonUtils.ChangeStrToDbl(Master.ClsYamadomeCsv.GetKouzaiSizeSunpou(kouzaiSizeDN, 1));
                        break;
                    }
                case "H形鋼 広幅":
                case "H形鋼 中幅":
                case "H形鋼 細幅":
                    {
                        familyPathList.Add(Master.ClsHBeamCsv.GetFamilyPath(kouzaiSizeUP));
                        familyPathList.Add(Master.ClsHBeamCsv.GetFamilyPath(kouzaiSizeDN));
                        sunpou = ClsCommonUtils.ChangeStrToDbl(Master.ClsYamadomeCsv.GetKouzaiSizeSunpou(kouzaiSizeUP, 1));
                        sunpou2 = ClsCommonUtils.ChangeStrToDbl(Master.ClsYamadomeCsv.GetKouzaiSizeSunpou(kouzaiSizeDN, 1));
                        break;
                    }
                default:
                    {
                        familyPathList.Add(Master.ClsYamadomeCsv.GetKariFamilyPath(kouzaiSizeUP));
                        familyPathList.Add(Master.ClsYamadomeCsv.GetKariFamilyPath(kouzaiSizeDN));
                        sunpou = Master.ClsYamadomeCsv.GetWidth(kouzaiSizeUP);
                        sunpou2 = Master.ClsYamadomeCsv.GetWidth(kouzaiSizeDN);
                        break;
                    }
            }
            //鋼材サイズの高さ
            double dKouzaiSize = ClsRevitUtil.CovertToAPI(sunpou / 2);
            kouzaiSizeList.Add(dKouzaiSize);
            dKouzaiSize = ClsRevitUtil.CovertToAPI(sunpou2 / 2);
            kouzaiSizeList.Add(dKouzaiSize);


            foreach (string familyPath in familyPathList)
            {
                string familyName = ClsRevitUtil.GetFamilyName(familyPath);

                if (!ClsRevitUtil.LoadFamilyData(doc, familyPath, out Family kouFam))
                {
                    continue;
                    //break;
                }
                FamilySymbol kozaiSym = ClsRevitUtil.GetFamilySymbol(doc, familyName, buzaiName);
                if (kozaiSym == null)
                    continue;

                //#31709
                using (Transaction t = new Transaction(doc, "エンドプレートオフ"))
                {
                    t.Start();
                    FailureHandlingOptions failOpt = t.GetFailureHandlingOptions();
                    failOpt.SetFailuresPreprocessor(new WarningSwallower());
                    t.SetFailureHandlingOptions(failOpt);
                    try
                    {
                        switch (kouzaiType)
                        {
                            case "チャンネル":
                            case "H形鋼 広幅":
                            case "H形鋼 中幅":
                            case "H形鋼 細幅":
                                {
                                    ClsRevitUtil.SetTypeParameter(kozaiSym, "エンドプレート", 0);
                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }
                    }
                    catch
                    { t.RollBack(); }

                    t.Commit();
                }
                symList.Add(kozaiSym);

                ////鋼材サイズの高さ
                //string sunpou = Master.ClsYamadomeCsv.GetWidth(kouzaiSize); //ClsYMSUtil.GetKouzaiSizeSunpou(familyName, 1);
                //double dKouzaiSize = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(sunpou)) / 2;
                //kouzaiSizeList.Add(dKouzaiSize);
            }
            return true;
        }

        public static bool SaveProjectInfo(Document doc, int create)
        {
            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                try
                {
                    t.Start();
                    FailureHandlingOptions failOpt = t.GetFailureHandlingOptions();
                    failOpt.SetFailuresPreprocessor(new WarningSwallower());
                    t.SetFailureHandlingOptions(failOpt);

                    ProjectInfo pInfo = doc.ProjectInformation;

                    Parameter parameter = null;
                    string mess = null;
                    parameter = pInfo.LookupParameter(KARIKOUZAI);
                    if (parameter != null)
                    {
                        parameter.Set(create);
                    }
                    else { mess = mess + "「" + KARIKOUZAI + "」"; }
                    if (mess != null)
                    {
                        mess += "がプロジェクト情報項目に存在しません。\n";
                        MessageBox.Show(mess);
                    }

                    t.Commit();
                }
                catch (OperationCanceledException e)
                {
                }
                catch (Exception e)
                {
                }
            }
            return true;
        }
        public static bool GetProjectInfo(Document doc)
        {
            try
            {
                ProjectInfo pInfo = doc.ProjectInformation;

                Parameter parameter = null;
                parameter = pInfo.LookupParameter(KARIKOUZAI);
                if (parameter != null)
                {
                    int create = parameter.AsInteger();
                    if (create == 0)
                        return true;
                }

            }
            catch (OperationCanceledException e)
            {
            }
            catch (Exception e)
            {
            }
            return false;
        }

        /// 仮鋼材 のみを単独選択
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="id">選択した 仮鋼材 のID</param>
        /// <param name="message">指示</param>
        /// <returns></returns>
        public static bool PickBaseObject(UIDocument uidoc, ref ElementId id, string message = "仮鋼材")
        {
            //仮鋼材部品名を取得
            List<string> kariKouzaiList = new List<string>();
            kariKouzaiList.AddRange(Master.ClsYamadomeCsv.GetKariFamilyNameList().ToList());
            kariKouzaiList.AddRange(Master.ClsChannelCsv.GetFamilyNameList().ToList());
            kariKouzaiList.AddRange(Master.ClsHiuchiCsv.GetFamilyNameList().ToList());
            kariKouzaiList.AddRange(Master.ClsJackCsv.GetFamilyNameList().ToList());
            kariKouzaiList.AddRange(Master.ClsSupportPieceCsv.GetFamilyNameList().ToList());
            kariKouzaiList.AddRange(Master.ClsBurumanCSV.GetFamilyNameList().ToList());

            return ClsRevitUtil.PickObjectSymbolFilters(uidoc, message, kariKouzaiList, ref id);
        }
        /// <summary>
        /// 図面上の仮鋼材を全て取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<ElementId> GetAllIdList(Document doc)
        {
            //仮鋼材一覧
            List<string> kariKouzaiList = new List<string>();
            kariKouzaiList.AddRange(Master.ClsYamadomeCsv.GetKariFamilyNameList().ToList());
            kariKouzaiList.AddRange(Master.ClsYamadomeCsv.GetKariSyabariFamilyNameList().ToList());
            kariKouzaiList.AddRange(Master.ClsChannelCsv.GetFamilyNameList().ToList());
            kariKouzaiList.AddRange(Master.ClsHiuchiCsv.GetFamilyNameList().ToList());
            kariKouzaiList.AddRange(Master.ClsJackCsv.GetFamilyNameList().ToList());
            kariKouzaiList.AddRange(Master.ClsSupportPieceCsv.GetFamilyNameList().ToList());
            kariKouzaiList.AddRange(Master.ClsBurumanCSV.GetFamilyNameList().ToList());

            //図面上の仮鋼材をを全て取得
            List<ElementId> elements = ClsRevitUtil.GetAllCreatedFamilyInstanceList(doc);
            List<ElementId> targetFamilies = new List<ElementId>();
            foreach (string name in kariKouzaiList)
            {
                foreach (ElementId elem in elements)
                {
                    FamilyInstance inst = doc.GetElement(elem) as FamilyInstance;
                    if (inst != null && inst.Symbol.FamilyName == name)//ファミリ名でフィルター
                    {
                        targetFamilies.Add(elem);
                    }
                }
            }
            return targetFamilies;
        }

        /// <summary>
        /// 図面上の仮鋼材を全て取得する（主材系のみ）
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<ElementId> GetAllIdList2(Document doc)
        {
            //仮鋼材一覧
            List<string> kariKouzaiList = new List<string>();
            kariKouzaiList.AddRange(Master.ClsYamadomeCsv.GetKariFamilyNameList().ToList());
            //kariKouzaiList.AddRange(Master.ClsYamadomeCsv.GetKariSyabariFamilyNameList().ToList());自動割付には対応していない

            //図面上の仮鋼材をを全て取得
            List<ElementId> elements = ClsRevitUtil.GetAllCreatedFamilyInstanceList(doc);
            List<ElementId> targetFamilies = new List<ElementId>();
            foreach (string name in kariKouzaiList)
            {
                foreach (ElementId elem in elements)
                {
                    FamilyInstance inst = doc.GetElement(elem) as FamilyInstance;
                    if (inst != null && inst.Symbol.FamilyName == name)//ファミリ名でフィルター
                    {
                        targetFamilies.Add(elem);
                    }
                }
            }
            return targetFamilies;
        }
        /// <summary>
        /// 対象のファミリにベースIdをカスタムデータとして持たせる
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id">ファミリ</param>
        /// <param name="baseId">ベースのID</param>
        /// <returns></returns>
        public static bool SetBaseId(Document doc, ElementId id, ElementId baseId, string dataname = "")
        {
            int iBaseId = baseId.IntegerValue;
            string s = string.IsNullOrEmpty(dataname) ? BASEID : dataname;
            return ClsRevitUtil.CustomDataSet<int>(doc, id, s, iBaseId);
        }
        /// <summary>
        /// 対象のファミリからベースIdのカスタムデータを取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id">ファミリ</param>
        /// <returns>ベースのID</returns>
        public static ElementId GetBaseId(Document doc, ElementId id, string dataname = "")
        {
            string s = string.IsNullOrEmpty(dataname) ? BASEID : dataname;
            ClsRevitUtil.CustomDataGet<int>(doc, id, s, out int iBaseId);

            ElementId e = new ElementId(iBaseId);
            return e;
        }

        /// <summary>
        /// ダブルベース段毎の対象ファミリにベースIdをカスタムデータとして持たせる
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id">ファミリ</param>
        /// <param name="baseId">ベースのID</param>
        /// <param name="dan">段</param>
        /// <returns></returns>
        public static bool SetBaseIdDouble(Document doc, ElementId id, ElementId baseId, string dan)
        {
            int iBaseId = baseId.IntegerValue;
            return ClsRevitUtil.CustomDataSet<int>(doc, id, "ベース" + dan, iBaseId);
        }
        /// <summary>
        /// ダブルベース段毎の対象ファミリからベースIdのカスタムデータを取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id">ファミリ</param>
        /// <param name="dan">段</param>
        /// <returns>ベースのID</returns>
        public static ElementId GetBaseIdDouble(Document doc, ElementId id, string dan)
        {
            ClsRevitUtil.CustomDataGet<int>(doc, id, "ベース" + dan, out int iBaseId);

            ElementId e = new ElementId(iBaseId);
            return e;
        }

        /// <summary>
        /// 対象の部品に仮鋼材配置で作成したフラグを持たせる
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id">部品ID</param>
        /// <returns></returns>
        public static bool SetKariKouzaiFlag(Document doc, ElementId id)
        {
            return ClsRevitUtil.CustomDataSet(doc, id, "仮鋼材で作成", true);
        }

        /// <summary>
        /// 対象の部品が仮鋼材配置で作成されたかのフラグを取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id">部品ID</param>
        /// <returns>true:仮鋼材で配置された部品</returns>
        public static bool GetKariKouzaiFlag(Document doc, ElementId id)
        {
            ClsRevitUtil.CustomDataGet(doc, id, "仮鋼材で作成", out bool flag);
            //return true;
            return flag;
        }
    }
}
