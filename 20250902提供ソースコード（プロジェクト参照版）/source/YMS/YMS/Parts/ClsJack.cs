using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YMS.DLG;
using static Autodesk.Revit.DB.SpecTypeId;

namespace YMS.Parts
{
    public class ClsJack
    {
        #region 定数

        public const string JACK = "ジャッキ";
        public const string JIKURYOKU = "軸力";
        public const string PLUSJIKURYOKU = "+軸力";

        #endregion

        #region プロパティ

        /// <summary>
        /// ジャッキの店所
        /// </summary>
        public string m_JackTensyo { get; set; }

        /// <summary>
        /// ジャッキ種類
        /// </summary>
        public string m_JackType { get; set; }

        /// <summary>
        /// 離れ量の有無
        /// </summary>
        public bool m_UseOffset { get; set; }

        /// <summary>
        /// 離れ量
        /// </summary>
        public double m_Offset { get; set; }

        /// <summary>
        /// ジャッキカバーの有無
        /// </summary>
        public bool m_UseJackCover { get; set; }

        /// <summary>
        /// 対象ジャッキID
        /// </summary>
        public ElementId m_CreatedJackId { get; set; }

        /// <summary>
        /// 対象ジャッキカバーID
        /// </summary>
        public ElementId m_CreatedJackCoverId { get; set; }


        /// <summary>
        /// 主材サイズ
        /// </summary>
        public string m_SyuzaiSize { get; set; }

        #endregion

        #region コンストラクタ

        public ClsJack()
        {
            Init();
        }

        #endregion

        #region メソッド

        public void Init()
        {
            m_JackTensyo = string.Empty;
            m_JackType = string.Empty;
            m_UseOffset = false;
            m_Offset = 0.0;
            m_UseJackCover = false;
            m_CreatedJackId = null;
            m_CreatedJackCoverId = null;
        }

        /// <summary>
        /// ジャッキの配置
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="baseId"></param>
        /// <param name="baseSp"></param>
        /// <param name="baseEp"></param>
        /// <param name="insertPoint"></param>
        /// <param name="useAngle"></param>
        /// <param name="angle"></param>
        /// <param name="remainingLength"></param>
        /// <param name="flagName"></param>
        /// <returns></returns>
        public bool CreateJack(Document doc, ElementId baseId, XYZ baseSp, XYZ baseEp, XYZ insertPoint, bool useAngle = false, double angle = 0.0, double remainingLength = 0.0, string flagName = "通常")
        {
            FamilyInstance instBase = doc.GetElement(baseId) as FamilyInstance;

            string jackSize = Master.ClsJackCsv.GetJackSize(m_JackType, m_SyuzaiSize);
            string familyPath = Master.ClsJackCsv.GetFamilyPath(jackSize);
            string familyName = RevitUtil.ClsRevitUtil.GetFamilyName(familyPath);
            string typeName = ChangeTypeNameToBaseName(instBase.Name);
            string typeNameJackCover = typeName;

            typeName += ChangeTensyoToType(m_JackTensyo);
            if (jackSize == "")
            {
                TaskDialog.Show("Inserted Symbol Error", "対象サイズのジャッキシンボルが見つかりませんでした");
                return false;
            }

            if (!ClsRevitUtil.LoadFamilyData(doc, familyPath, out Family family))
            {
                return false;
            }
            FamilySymbol symJack = (ClsRevitUtil.GetFamilySymbol(doc, familyName, typeName));

            // ジャッキの配置高さ
            double offset = 0.0;
            string dan = ClsRevitUtil.GetParameter(doc, baseId, "段");
            if (instBase.Name == ClsCornerHiuchiBase.baseName)
            {
                if (ClsRevitUtil.GetInstMojiParameter(doc, baseId, "構成") == "ダブル")
                {
                    if (DlgWaritsuke.ShowSelectDialogCornerHiuchi2())
                    {
                        dan = "上段";
                    }
                    else
                    {
                        dan = "下段";
                    }
                }
            }

            ClsYMSUtil.GetDifferenceWithAllBase(doc, baseId, out double diff, out double diff2);
            double syuzaiWidth = ClsRevitUtil.CovertToAPI(Master.ClsYamadomeCsv.GetWidth(m_SyuzaiSize) / 2) + diff;
            if (dan == "上段")
            {
                offset = +syuzaiWidth;
            }
            else if (dan == "下段")
            {
                offset = -syuzaiWidth;
            }

            FamilyInstance placedInstance = null;
            if (symJack != null)
            {
                using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                {
                    t.Start();
                    FailureHandlingOptions failOpt = t.GetFailureHandlingOptions();
                    failOpt.SetFailuresPreprocessor(new WarningSwallower());
                    t.SetFailureHandlingOptions(failOpt);

                    ElementId createdId = null;
                    XYZ vec = (baseEp - baseSp).Normalize();
                    Line axis = Line.CreateBound(insertPoint, insertPoint + XYZ.BasisZ);
                    XYZ normal = XYZ.BasisZ;
                    if (instBase.Host is ReferencePlane referencePlane)
                    {
                        createdId = ClsRevitUtil.Create(doc, insertPoint, vec, referencePlane, symJack);
                        ClsRevitUtil.SetParameter(doc, createdId, "集計レベル", ClsRevitUtil.GetParameterElementId(doc, baseId, "集計レベル"));
                        //offset += ClsRevitUtil.GetParameterDouble(doc, createdId, "ホストからのオフセット");
                        //ClsRevitUtil.SetParameter(doc, createdId, "ホストからのオフセット", offset);
                        //axis = Line.CreateBound(insertPoint, insertPoint + referencePlane.Normal);
                        //normal = referencePlane.Normal;
                    }
                    else
                    {
                        // ジャッキの配置
                        createdId = ClsRevitUtil.Create(doc, insertPoint, instBase.Host.Id, symJack);
                        // ジャッキを回転させる
                        if (!useAngle)
                        {
                            angle = GetRotateAngle(baseEp, baseSp);
                        }
                        ElementTransformUtils.RotateElement(doc, createdId, axis, angle);
                        // ジャッキの基準レベルからの高さを設定
                        if (!ClsRevitUtil.SetParameter(doc, createdId, "基準レベルからの高さ", offset))
                            ClsRevitUtil.SetParameter(doc, createdId, "ホストからのオフセット", offset);
                    }
                    FamilyInstance instJack = doc.GetElement(createdId) as FamilyInstance;
                    //if (!useAngle)
                    //{
                    //    // Z軸と計算した方向ベクトルのなす角を求める
                    //    angle = instJack.HandOrientation.AngleOnPlaneTo(vec, normal);
                    //}
                    //ElementTransformUtils.RotateElement(doc, createdId, axis, angle);

                    
                    // ジャッキの長さを取得し、調整する
                    double jackLength = ClsRevitUtil.GetParameterDouble(doc, createdId, "長さ");
                    //Curve cvBase = (instBase.Location as LocationCurve).Curve;
                    //XYZ tmpStPoint = cvBase.GetEndPoint(0);
                    //XYZ tmpEdPoint = cvBase.GetEndPoint(1);
                    //remainingLength = tmpStPoint.DistanceTo(tmpEdPoint);
                    double addLength = GetAdjustedJackStrokeLength(remainingLength, jackLength);
                    ClsRevitUtil.SetParameter(doc, createdId, "長さ", jackLength + addLength);

                    

                    // カスタムデータを設定
                    if (m_JackType.Contains(PLUSJIKURYOKU))
                    {
                        //ClsRevitUtil.CustomDataSet(doc, createdId, PLUSJIKURYOKU, true);
                        ClsRevitUtil.CustomDataSet(doc, createdId, "プラス軸力", true);
                    }

                    // 基準IDを設定
                    ClsKariKouzai.SetBaseId(doc, createdId, baseId);
                    SetBaseId(doc, createdId, flagName);

                    // トランザクションをコミット
                    t.Commit();

                    // 配置されたインスタンスを取得
                    m_CreatedJackId = createdId;
                    placedInstance = doc.GetElement(createdId) as FamilyInstance;

                }
            }

            // ジャッキカバーの作図
            if (placedInstance != null && m_UseJackCover)
            {
                string strFamilyPath = Master.ClsJackCoverCSV.GetFamilyPath(m_SyuzaiSize);
                string strFamilyName = RevitUtil.ClsRevitUtil.GetFamilyName(strFamilyPath);

                bool bInversion = false;
                XYZ dir = placedInstance.HandOrientation;
                if (m_SyuzaiSize == "60SMH")
                {
                    // 軸力計(土圧計とも呼ぶ)付きジャッキの場合はフタの枚数が変わる
                    if (m_JackType.Contains(JIKURYOKU))
                    {
                        typeNameJackCover = typeNameJackCover + "_CV2枚";
                    }
                    else
                    {
                        typeNameJackCover = typeNameJackCover + "_CV3枚";
                    }
                    bInversion = true;

                    double dZure = 245.0; 
                    insertPoint = insertPoint - dir * ClsRevitUtil.ConvertDoubleGeo2Revit(dZure);
                }

                if (!ClsRevitUtil.LoadFamilyData(doc, strFamilyPath, out Family famJackCover))
                {
                    return false;
                }
                FamilySymbol symJackCover = (ClsRevitUtil.GetFamilySymbol(doc, strFamilyName, typeNameJackCover));

                if (symJackCover != null)
                {
                    using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                    {
                        t.Start();

                        ElementId CreatedId = null;
                        XYZ vec = (baseEp - baseSp).Normalize();
                        Line axis = Line.CreateBound(insertPoint, insertPoint + XYZ.BasisZ);
                        if (instBase.Host is ReferencePlane referencePlane)
                        {
                            if (bInversion)
                                vec *= -1;
                            CreatedId = ClsRevitUtil.Create(doc, insertPoint, -vec, referencePlane, symJackCover);
                            ClsRevitUtil.SetParameter(doc, CreatedId, "集計レベル", ClsRevitUtil.GetParameterElementId(doc, baseId, "集計レベル"));
                            //offset += ClsRevitUtil.GetParameterDouble(doc, CreatedId, "ホストからのオフセット");
                            //ClsRevitUtil.SetParameter(doc, CreatedId, "ホストからのオフセット", offset);
                            //axis = Line.CreateBound(insertPoint, insertPoint + referencePlane.Normal);
                        }
                        else
                        {
                            CreatedId = ClsRevitUtil.Create(doc, insertPoint, instBase.Host.Id, symJackCover);
                            ClsRevitUtil.RotateElement(doc, CreatedId, axis, angle);
                            ClsRevitUtil.RotateElement(doc, CreatedId, axis, Math.PI); // 180度回転 (ラジアン単位)
                            if (bInversion)
                            {
                                // ジャッキの挿入点の都合で前後が逆
                                ClsRevitUtil.RotateElement(doc, CreatedId, axis, Math.PI); // 180度回転 (ラジアン単位)
                            }
                            if (!ClsRevitUtil.SetParameter(doc, CreatedId, "基準レベルからの高さ", offset))
                                ClsRevitUtil.SetParameter(doc, CreatedId, "ホストからのオフセット", offset);
                        }
                        //ClsRevitUtil.RotateElement(doc, CreatedId, axis, angle);
                        //ClsRevitUtil.RotateElement(doc, CreatedId, axis, Math.PI); // 180度回転 (ラジアン単位)

                        
                        //if (bInversion)
                        //{
                        //    // ジャッキの挿入点の都合で前後が逆
                        //    ClsRevitUtil.RotateElement(doc, CreatedId, axis, Math.PI); // 180度回転 (ラジアン単位)
                        //}
                        ClsKariKouzai.SetBaseId(doc, CreatedId, baseId);

                        t.Commit();

                        m_CreatedJackCoverId = CreatedId;
                    }
                }
            }

            return true;
        }
        /// <summary>
        /// ジャッキの配置
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="baseId"></param>
        /// <param name="baseSp"></param>
        /// <param name="baseEp"></param>
        /// <param name="insertPoint"></param>
        /// <param name="useAngle"></param>
        /// <param name="angle"></param>
        /// <param name="remainingLength"></param>
        /// <param name="flagName"></param>
        /// <returns></returns>
        public bool CreateJacka(Document doc, ElementId baseId, XYZ baseSp, XYZ baseEp, XYZ insertPoint, bool useAngle = false, double angle = 0.0, double remainingLength = 0.0, string flagName = "通常")
        {
            FamilyInstance instBase = doc.GetElement(baseId) as FamilyInstance;

            string jackSize = Master.ClsJackCsv.GetJackSize(m_JackType, m_SyuzaiSize);
            string familyPath = Master.ClsJackCsv.GetFamilyPath(jackSize);
            string familyName = RevitUtil.ClsRevitUtil.GetFamilyName(familyPath);
            string typeName = ChangeTypeNameToBaseName(instBase.Name);

            typeName += ChangeTensyoToType(m_JackTensyo);
            if (jackSize == "")
            {
                TaskDialog.Show("Inserted Symbol Error", "対象サイズのジャッキシンボルが見つかりませんでした");
                return false;
            }

            if (!ClsRevitUtil.LoadFamilyData(doc, familyPath, out Family family))
            {
                return false;
            }
            FamilySymbol symJack = (ClsRevitUtil.GetFamilySymbol(doc, familyName, typeName));

            // ジャッキの配置高さ
            double offset = 0.0;
            string dan = ClsRevitUtil.GetParameter(doc, baseId, "段");
            if (instBase.Name == ClsCornerHiuchiBase.baseName)
            {
                if (ClsRevitUtil.GetInstMojiParameter(doc, baseId, "構成") == "ダブル")
                {
                    if (DlgWaritsuke.ShowSelectDialogCornerHiuchi2())
                    {
                        dan = "上段";
                    }
                    else
                    {
                        dan = "下段";
                    }
                }
            }

            ClsYMSUtil.GetDifferenceWithAllBase(doc, baseId, out double diff, out double diff2);
            double syuzaiWidth = ClsRevitUtil.CovertToAPI(Master.ClsYamadomeCsv.GetWidth(m_SyuzaiSize) / 2) + diff;
            if (dan == "上段")
            {
                offset = +syuzaiWidth;
            }
            else if (dan == "下段")
            {
                offset = -syuzaiWidth;
            }

            FamilyInstance placedInstance = null;
            if (symJack != null)
            {
                using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                {
                    t.Start();

                    ElementId createdId = null;
                    XYZ vec = (baseEp - baseSp).Normalize();
                    if (instBase.Host is ReferencePlane referencePlane)
                    {
                        createdId = ClsRevitUtil.Create(doc, insertPoint, vec, referencePlane, symJack);
                        //ジャッキは参照面でも回転が必要かも
                    }
                    else
                    {
                        // ジャッキの配置
                        createdId = ClsRevitUtil.Create(doc, insertPoint, instBase.Host.Id, symJack);
                        // ジャッキを回転させる
                        if (!useAngle)
                        {
                            angle = GetRotateAngle(baseEp, baseSp);
                        }
                        ElementTransformUtils.RotateElement(doc, createdId, Line.CreateBound(insertPoint, insertPoint + XYZ.BasisZ), angle);
                    }

                    FamilyInstance instJack = doc.GetElement(createdId) as FamilyInstance;
                    // ジャッキの長さを取得し、調整する
                    double jackLength = ClsRevitUtil.GetParameterDouble(doc, createdId, "長さ");
                    //Curve cvBase = (instBase.Location as LocationCurve).Curve;
                    //XYZ tmpStPoint = cvBase.GetEndPoint(0);
                    //XYZ tmpEdPoint = cvBase.GetEndPoint(1);
                    //remainingLength = tmpStPoint.DistanceTo(tmpEdPoint);
                    double addLength = GetAdjustedJackStrokeLength(remainingLength, jackLength);
                    ClsRevitUtil.SetParameter(doc, createdId, "長さ", jackLength + addLength);

                    // ジャッキの基準レベルからの高さを設定
                    if (!ClsRevitUtil.SetParameter(doc, createdId, "基準レベルからの高さ", offset))
                        ClsRevitUtil.SetParameter(doc, createdId, "ホストからのオフセット", offset);

                    // カスタムデータを設定
                    if (m_JackType.Contains(PLUSJIKURYOKU))
                    {
                        //ClsRevitUtil.CustomDataSet(doc, createdId, PLUSJIKURYOKU, true);
                        ClsRevitUtil.CustomDataSet(doc, createdId, "プラス軸力", true);
                    }

                    // 基準IDを設定
                    ClsKariKouzai.SetBaseId(doc, createdId, baseId);
                    SetBaseId(doc, createdId, flagName);

                    // トランザクションをコミット
                    t.Commit();

                    // 配置されたインスタンスを取得
                    m_CreatedJackId = createdId;
                    placedInstance = doc.GetElement(createdId) as FamilyInstance;

                }
            }

            // ジャッキカバーの作図
            if (placedInstance != null && m_UseJackCover)
            {
                string strFamilyPath = Master.ClsJackCoverCSV.GetFamilyPath(m_SyuzaiSize);
                string strFamilyName = RevitUtil.ClsRevitUtil.GetFamilyName(strFamilyPath);

                bool bInversion = false;
                XYZ dir = placedInstance.HandOrientation;
                if (m_SyuzaiSize == "60SMH")
                {
                    // 軸力計(土圧計とも呼ぶ)付きジャッキの場合はフタの枚数が変わる
                    if (m_JackType.Contains(JIKURYOKU))
                    {
                        typeName = typeName + "_CV2枚";
                    }
                    else
                    {
                        typeName = typeName + "_CV3枚";
                    }
                    bInversion = true;

                    double dZure = 245.0; 
                    insertPoint = insertPoint - dir * ClsRevitUtil.ConvertDoubleGeo2Revit(dZure);
                }

                if (!ClsRevitUtil.LoadFamilyData(doc, strFamilyPath, out Family famJackCover))
                {
                    return false;
                }
                FamilySymbol symJackCover = (ClsRevitUtil.GetFamilySymbol(doc, strFamilyName, typeName));

                if (symJackCover != null)
                {
                    using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                    {
                        t.Start();

                        ElementId CreatedId = null;
                        XYZ vec = (baseEp - baseSp).Normalize();
                        Line axis = Line.CreateBound(insertPoint, insertPoint + XYZ.BasisZ);
                        if (instBase.Host is ReferencePlane referencePlane)
                        {
                            CreatedId = ClsRevitUtil.Create(doc, insertPoint, vec, referencePlane, symJackCover);
                            //ジャッキは参照面でも回転が必要かも配置向きに関しては本実装時にTEST
                        }
                        else
                        {
                            CreatedId = ClsRevitUtil.Create(doc, insertPoint, instBase.Host.Id, symJackCover);

                            ClsRevitUtil.RotateElement(doc, CreatedId, axis, angle);
                            ClsRevitUtil.RotateElement(doc, CreatedId, axis, Math.PI); // 180度回転 (ラジアン単位)
                        }

                        if (!ClsRevitUtil.SetParameter(doc, CreatedId, "基準レベルからの高さ", offset))
                            ClsRevitUtil.SetParameter(doc, CreatedId, "ホストからのオフセット", offset);


                        if (bInversion)
                        {
                            // ジャッキの挿入点の都合で前後が逆
                            ClsRevitUtil.RotateElement(doc, CreatedId, axis, Math.PI); // 180度回転 (ラジアン単位)
                        }

                        ClsKariKouzai.SetBaseId(doc, CreatedId, baseId);

                        t.Commit();

                        m_CreatedJackCoverId = CreatedId;
                    }
                }
            }

            return true;
        }
        public string ChangeTypeNameToBaseName(string typeName)
        {
            if (typeName.Contains("切梁火打"))
                return "切梁火打";
            else if (typeName.Contains("隅火打"))
                return "隅火打";
            else
                return "切梁";
        }

        public string ChangeTensyoToType(string tensyo)
        {
            switch (tensyo)
            {
                case "九州":
                    {
                        return "_九";
                    }
                case "大阪":
                case "四国":
                    {
                        return "_大四";
                    }
                case "東京":
                case "名古屋":
                case "北海道":
                    {
                        return "_東名北";
                    }
                case "仙台":
                    {
                        return "_東名仙";
                    }
                case "大分":
                    {
                        return "_東名北大";
                    }
                default:
                    {
                        return string.Empty;
                    }
            }
        }

        /// <summary>
        /// 対象のジャッキに何で作成されたかのフラグを持たせる
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id">ジャッキ</param>
        /// <param name="baseId">ベースのID</param>
        /// <returns></returns>
        public static bool SetBaseId(Document doc, ElementId id, string flagName)
        {
            return ClsRevitUtil.CustomDataSet(doc, id, JACK, flagName);
        }

        /// <summary>
        /// 対象のジャッキが何で作成されたかのフラグを取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id">ジャッキ</param>
        /// <returns>ベースのID</returns>
        public static string GetBaseId(Document doc, ElementId id)
        {
            ClsRevitUtil.CustomDataGet(doc, id, JACK, out string flagName);

            return flagName;
        }

        /// <summary>
        /// ジャッキに対応するジャッキカバーを取得
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="jackId"></param>
        /// <param name="jackCoverId"></param>
        /// <returns></returns>
        public static bool GetConnectedJackCover(Document doc,ElementId jackId, ref ElementId jackCoverId)
        {
            bool res = false;

            List<string> JackNameList = new List<string>();
            JackNameList.AddRange(Master.ClsJackCsv.GetFamilyNameList().ToList());

            FamilyInstance inst = doc.GetElement(jackId) as FamilyInstance;

            if (!JackNameList.Contains(inst.Symbol.FamilyName))
            {
                //検索対象がジャッキではない
                return res;
            }

            //図面上のジャッキカバーを全て取得
            List<ElementId> idList = ClsRevitUtil.GetSelectCreatedFamilyInstanceFamilySymbolList(doc, "ジャッキカバー", true);

            if (idList.Count > 0)
            {
                List<ElementId> hitIds = ClsRevitUtil.GetIntersectFamilysToDependentElements(doc, jackId, 10, null, idList);
                //一つしかヒットしないはずだが複数あれば一番目を返す
                if (hitIds.Count > 0)
                {
                    jackCoverId = hitIds[0];
                    res = true;
                }
            }

            return res;
        }

        /// <summary>
        /// ジャッキか？？
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="jackId"></param>
        /// <returns></returns>
        public static bool IsJack(Document doc, ElementId jackId)
        {
            bool res = false;

            List<string> JackNameList = new List<string>();
            JackNameList.AddRange(Master.ClsJackCsv.GetFamilyNameList().ToList());

            FamilyInstance inst = doc.GetElement(jackId) as FamilyInstance;

            if (!JackNameList.Contains(inst.Symbol.FamilyName))
            {
                //検索対象がジャッキではない
                return res;
            }
            res = true;

            return res;
        }


        public static double GetTotalJack(Document doc, ElementId id)
        {
            double total = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetParameterDouble(doc, id, "長さ"));
            FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
            FamilySymbol sym = inst.Symbol;

            if (sym != null)
            {
                total += ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(sym, "調整材長さ_左"));
                total += ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(sym, "調整材長さ_右"));
            }

            return total;
        }

        private static double GetAdjustedJackStrokeLength(double remainingStrokeLength, double memberLength)
        {
            double additionalStroke = 0.0;

            // 残りのストローク長さ
            remainingStrokeLength = ClsRevitUtil.CovertFromAPI(remainingStrokeLength);
            double roundedRemainingStrokeLength = Math.Floor(remainingStrokeLength / 100) * 100.0;
            double remainingFraction = remainingStrokeLength - roundedRemainingStrokeLength;

            // 部材長さ
            memberLength = ClsRevitUtil.CovertFromAPI(memberLength);

            if (remainingStrokeLength % 100 == 0)
            {
                // ストロークを伸縮0mmで配置することはなく、常にストロークを伸ばす
                additionalStroke += 100;
                additionalStroke -= (int)((memberLength + additionalStroke) % 100);
            }
            else
            {
                additionalStroke = remainingFraction;
                var remainingDifference = remainingStrokeLength - (memberLength + additionalStroke);
                double remainingRounded = (int)(remainingDifference + 99) / 100 * 100;
                double remainingModulus = remainingDifference - remainingRounded;
                additionalStroke += remainingModulus;
            }

            return ClsRevitUtil.ConvertDoubleGeo2Revit(additionalStroke);
        }

        private static double GetRotateAngle(XYZ endpoint, XYZ startpoint)
        {
            // 基準点から挿入点へのベクトルを取得
            XYZ direction = endpoint - startpoint;

            // ベクトルをZ軸に対応する方向に変換（プロジェクト平面への変換）
            XYZ projectedDirection = new XYZ(direction.X, direction.Y, 0).Normalize();

            // Z軸と計算した方向ベクトルのなす角を求める
            double angle = XYZ.BasisX.AngleOnPlaneTo(projectedDirection, XYZ.BasisZ);
            //double angle = projectedDirection.AngleTo(XYZ.BasisX);
            //if (ClsGeo.IsLeft(insertPoint, basePoint))
            //{
            //    angle = -angle;
            //}

            return angle;
        }

        #endregion
    }
}
