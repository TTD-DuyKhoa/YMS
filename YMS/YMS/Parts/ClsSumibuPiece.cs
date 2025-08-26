using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YMS.Parts
{
    public class ClsSumibuPiece
    {
        #region 定数

        /// <summary>
        /// 名称
        /// </summary>
        public const string baseName = "隅部ピース";

        #endregion

        #region Enum

        /// <summary>
        /// 作成方法
        /// </summary>
        public enum CreateType
        {
            Auto,
            Manual
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// 作成方法
        /// </summary>
        public CreateType m_CreateType { get; set; }

        /// <summary>
        /// タイプ
        /// </summary>
        public string m_Type { get; set; }

        /// <summary>
        /// サイズ
        /// </summary>
        public string m_Size { get; set; }

        /// <summary>
        /// 指定フラグ
        /// </summary>
        public bool m_SizeSelectFlg { get; set; }

        /// <summary>
        /// 間詰め量
        /// </summary>
        public double m_Tsumeryo { get; set; }

        // 以下はダイアログには表示されない

        /// <summary>
        /// 基点
        /// </summary>
        public XYZ m_BasePoint { get; set; }

        /// <summary>
        /// 回転角度
        /// </summary>
        public double m_Angle { get; set; }

        /// <summary>
        /// 腹起ID1
        /// </summary>
        public ElementId m_ParentId1 { get; set; }

        /// <summary>
        /// 腹起ID2
        /// </summary>
        public ElementId m_ParentId2 { get; set; }

        /// <summary>
        /// 取付段
        /// </summary>
        public string m_ToritsukeDan { get; set; }


        #endregion

        #region コンストラクタ

        public ClsSumibuPiece()
        {
            //初期化
            Init();
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 初期化
        /// </summary>
        public void Init()
        {
            m_CreateType = CreateType.Auto;
            m_Type = string.Empty;
            m_Size = string.Empty;
            m_SizeSelectFlg = false;
            m_Tsumeryo = 0;
            m_BasePoint = new XYZ();
            m_Angle = 0.0;
            m_ParentId1 = ElementId.InvalidElementId;
            m_ParentId2 = ElementId.InvalidElementId;
        }

        /// <summary>
        /// 隅部ピースの作図処理
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="familyName"></param>
        /// <param name="insertPoint"></param>
        /// <param name="kouzaiSize"></param>
        /// <param name="angle"></param>
        /// <param name="bIsHorizontal"></param>
        /// <param name="bIsUpper"></param>
        /// <returns></returns>
        public bool CreateSumibuPiece(Document doc, ElementId id, XYZ insertPoint, string kouzaiSize, double angle, bool bIsHorizontal, bool bIsUpper = false)
        {
            string familyPath = "";
            FamilyInstance haraInst = doc.GetElement(id) as FamilyInstance;

            if (m_SizeSelectFlg)
            {
                familyPath = Master.ClsSumibuPieceCsv.GetFamilyPath(m_Size);
            }
            else
            {
                
                familyPath = Master.ClsSumibuPieceCsv.GetFamilyPath(m_Type, kouzaiSize);
            }
            string familyName = RevitUtil.ClsRevitUtil.GetFamilyName(familyPath);
            m_Size = familyName.Replace("隅部ピース_", "");

            // 既に同じ名前のファミリがロードされていなければロードする
            ClsYMSUtil.LoadFamilyIfNotExists(doc, familyName, familyPath);

            string symbolType = "";
            if (bIsHorizontal)
            {
                symbolType = "H";// 水平方向
            }
            else
            {
                if (bIsUpper)
                {
                    symbolType = "V_Upper";// 垂直方向-上段

                    // ファミリ内にシンボルが存在するか確認
                    if (!SymbolExistsInFamily(doc, familyName, symbolType))
                    {
                        // 存在しない場合は複製する
                        FamilySymbol sourceSymbol = new FilteredElementCollector(doc)
                            .OfClass(typeof(FamilySymbol))
                            .Cast<FamilySymbol>()
                            .FirstOrDefault(s => s.Family.Name == familyName && s.Name == "V");

                        if (sourceSymbol != null)
                        {
                            using (Transaction transaction = new Transaction(doc, "Duplicate FamilySymbol"))
                            {
                                transaction.Start();

                                FamilySymbol duplicatedSymbol = sourceSymbol.Duplicate(symbolType) as FamilySymbol;
                                Parameter parameter = duplicatedSymbol.LookupParameter("角度");
                                parameter.Set(270.0 * (Math.PI / 180.0)); // 反転して下向きにする

                                transaction.Commit();
                            }
                        }
                    }
                }
                else
                {
                    symbolType = "V";// 垂直方向-下段
                }
            }



            // 対象のシンボルをアクティブにする
            ClsYMSUtil.ActivateSymbolInFamily(doc, familyName, symbolType);

            // シンボルを配置する
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ICollection<Element> elements = collector.OfClass(typeof(Family)).ToElements();
            Family trgfamily = null;

            foreach (Element element in elements)
            {
                Family family = element as Family;
                if (family != null && family.Name == familyName)
                {
                    trgfamily = family;
                }
            }
            if (trgfamily != null)
            {
                FamilyInstance placedInstance = null;

                // ファミリ内のシンボルを取得
                var familySymbols = trgfamily.GetFamilySymbolIds();
                foreach (var symbolId in familySymbols)
                {
                    Element symbol = doc.GetElement(symbolId) as FamilySymbol;
                    if (symbol != null && symbol.Name == symbolType)
                    {
                        using (Transaction transaction = new Transaction(doc, "シンボルの挿入"))
                        {
                            transaction.Start();

                            ClsRevitUtil.SetTypeParameter((FamilySymbol)symbol, "断面三角形", 0);
                            // インスタンスを作成
                            placedInstance = doc.Create.NewFamilyInstance((XYZ)insertPoint, (FamilySymbol)symbol, StructuralType.NonStructural);

                            // 回転軸を作成
                            Line rotationAxis = Line.CreateBound((XYZ)insertPoint, (XYZ)insertPoint + new XYZ(0, 0, 1)); // Z軸周りに回転
                                                                                                                         // 配置された FamilyInstance を回転
                            ElementTransformUtils.RotateElement(doc, placedInstance.Id, rotationAxis, angle);

                            if (bIsHorizontal && ClsRevitUtil.CheckNearGetEndPoint((haraInst.Location as LocationCurve).Curve, insertPoint) && m_ToritsukeDan == "同段")
                            {
                                symbol = ClsRevitUtil.ChangeTypeID(doc, placedInstance.Id, symbolType + "R");
                                ClsRevitUtil.SetTypeParameter((FamilySymbol)symbol, "角度", Math.PI);
                                ElementTransformUtils.RotateElement(doc, placedInstance.Id, rotationAxis, Math.PI / 2);
                                ClsRevitUtil.SetTypeParameter((FamilySymbol)symbol, "平面三角形", 0);
                            }
                            string haraSize = kouzaiSize;
                            double flangeHaba = Master.ClsYamadomeCsv.GetWidth(haraSize) / 2;
                            double webAtsuHalf = Master.ClsYamadomeCsv.GetWeb(haraSize) / 2;
                            //double webAtsu = Master.ClsYamadomeCsv.GetWeb(haraSize) ;
                            if (m_ToritsukeDan == "下段")
                                flangeHaba = -ClsRevitUtil.CovertToAPI(flangeHaba - webAtsuHalf);
                            else if (m_ToritsukeDan == "上段")
                                flangeHaba = ClsRevitUtil.CovertToAPI(flangeHaba - webAtsuHalf);
                            else
                                flangeHaba = 0.0;

                            double offset = ClsRevitUtil.GetParameterDouble(doc, placedInstance.Id, "ホストからのオフセット");
                            ClsRevitUtil.SetParameter(doc, placedInstance.Id, "ホストからのオフセット", offset + flangeHaba);

                            // インスタンスに拡張データを付与
                            ClsRevitUtil.CustomDataSet<bool>(doc, placedInstance.Id, "作成方法", (m_CreateType == CreateType.Auto) ? true : false);
                            ClsRevitUtil.CustomDataSet<String>(doc, placedInstance.Id, "タイプ", m_Type);
                            ClsRevitUtil.CustomDataSet<String>(doc, placedInstance.Id, "サイズ", m_Size);
                            ClsRevitUtil.CustomDataSet<bool>(doc, placedInstance.Id, "指定フラグ", m_SizeSelectFlg);
                            ClsRevitUtil.CustomDataSet<string>(doc, placedInstance.Id, "間詰め量", m_Tsumeryo.ToString());

                            ClsRevitUtil.CustomDataSet<string>(doc, placedInstance.Id, "基点", m_BasePoint.ToString());
                            ClsRevitUtil.CustomDataSet<string>(doc, placedInstance.Id, "回転角度", m_Angle.ToString());
                            ClsRevitUtil.CustomDataSet<ElementId>(doc, placedInstance.Id, "腹起ID1", m_ParentId1);
                            ClsRevitUtil.CustomDataSet<ElementId>(doc, placedInstance.Id, "腹起ID2", m_ParentId2);
                            ClsRevitUtil.CustomDataSet<String>(doc, placedInstance.Id, "取付段", m_ToritsukeDan);

                            ElementId levelId = haraInst.Host.Id;

                            if (levelId != null)
                            {
                                ClsRevitUtil.SetParameter(doc, placedInstance.Id, "集計レベル", levelId);
                                ClsRevitUtil.ModifyLevel(doc, placedInstance.Id, levelId);
                            }

                            transaction.Commit();
                        }

                    }
                }
            }

            return true;
        }

        /// <summary>
        /// シンボルの存在確認
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="familyName"></param>
        /// <param name="symbolName"></param>
        /// <returns></returns>
        public static bool SymbolExistsInFamily(Document doc, string familyName, string symbolName)
        {
            // ファミリを取得
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            var families = collector.OfClass(typeof(Family))
                .Cast<Family>()
                .Where(family => family.Name == familyName);

            foreach (var family in families)
            {
                // ファミリ内のシンボルを取得
                var familySymbols = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_GenericModel)
                    .OfClass(typeof(FamilySymbol))
                    .Cast<FamilySymbol>()
                    .Where(symbol => symbol.Family.Id == family.Id && symbol.Name == symbolName);

                if (familySymbols.Any())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 交点と角度を求める
        /// </summary>
        /// <param name="lCurve1"></param>
        /// <param name="lCurve2"></param>
        /// <param name="crossPoint"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public bool GetCrossPointAndAngle(LocationCurve lCurve1, LocationCurve lCurve2, out XYZ crossPoint, out double angle)
        {
            crossPoint = null;
            angle = 0.0;

            // 腹起の交点を取得
            crossPoint = ClsRevitUtil.GetIntersection(lCurve1.Curve, lCurve2.Curve);
            if (crossPoint != null)
            {
                // lCurve1とlCurve2の中点を求める
                XYZ midPoint1 = (lCurve1.Curve.GetEndPoint(0) + lCurve1.Curve.GetEndPoint(1)) / 2.0;
                XYZ midPoint2 = (lCurve2.Curve.GetEndPoint(0) + lCurve2.Curve.GetEndPoint(1)) / 2.0;

                // 選択1の単位ベクトル
                XYZ selVec1 = midPoint1 - crossPoint;
                selVec1 = selVec1.Normalize();

                // 選択2の単位ベクトル
                XYZ selVec2 = midPoint2 - crossPoint;
                selVec2 = selVec2.Normalize();

                // 1と2の合成
                XYZ midVec = selVec1 + selVec2;

                // 無回転時に想定される腹起ベース2つの単位ベクトルとその合成
                XYZ vec1 = new XYZ(-1, 0, 0);
                XYZ vec2 = new XYZ(0, -1, 0);
                XYZ vec3 = vec1 + vec2;

                // 合成同士の角度算出
                angle = vec3.AngleOnPlaneTo(midVec, XYZ.BasisZ);

                return true;
            }
            else
            {
                //MessageBox.Show("交点が見つかりません");
                return false;
            }
        }

        /// <summary>
        /// 主材の中央に配置するためのオフセット量を算出
        /// </summary>
        /// <param name="lCurve1"></param>
        /// <param name="lCurve2"></param>
        /// <param name="crossPoint"></param>
        /// <param name="kouzaiHaba"></param>
        /// <param name="moveVector"></param>
        /// <returns></returns>
        public bool GetCenterPlaceOffset(LocationCurve lCurve1, LocationCurve lCurve2, XYZ crossPoint, double kouzaiHaba, out XYZ selVec1, out XYZ selVec2, out XYZ moveVector)
        {
            // 上段腹起の方向と下段腹起の方向を算出して渡す
            XYZ midPoint1 = (lCurve1.Curve.GetEndPoint(0) + lCurve1.Curve.GetEndPoint(1)) / 2.0;
            XYZ midPoint2 = (lCurve2.Curve.GetEndPoint(0) + lCurve2.Curve.GetEndPoint(1)) / 2.0;

            // 選択1の単位ベクトル
            selVec1 = (midPoint1 - crossPoint).Normalize();

            // 選択2の単位ベクトル
            selVec2 = (midPoint2 - crossPoint).Normalize();

            // 1と2の合成
            XYZ midVec = selVec1 + selVec2;

            // オフセット量を算出
            double dOffset1 = kouzaiHaba / 2;
            XYZ vec = crossPoint - midPoint1;
            vec = vec.Normalize();
            moveVector = vec * ClsRevitUtil.ConvertDoubleGeo2Revit(dOffset1);

            return true;
        }
        /// <summary>
        /// 図面上の隅部ピースを全て取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<ElementId> GetAllSumibuPiece(Document doc)
        {
            List<string> sumibuList = Master.ClsSumibuPieceCsv.GetFamilyNameList().ToList();
            //図面上のインスタンスを全て取得
            List<ElementId> elements = ClsRevitUtil.GetAllCreatedFamilyInstanceList(doc);
            List<ElementId> targetFamilies = new List<ElementId>();
            foreach (string name in sumibuList)
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
        /// 隅部ピースの穴表示のON/OFF
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="idList"></param>
        /// <param name="flag">false:穴非表示,true:穴表示</param>
        /// <returns></returns>
        private static void SetSumibuPieceHole(Document doc, List<ElementId> idList, bool flag)
        {
            int show = 1;
            if (!flag)
                show = 0;
            foreach (ElementId id in idList)
            {
                ClsRevitUtil.SetParameter(doc, id, "穴表示", show);
            }
        }
        /// <summary>
        /// 隅部ピースの穴表示のON/OFF
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="flag">false:穴非表示,true:穴表示</param>
        /// <returns></returns>
        public static void SetSumibuPieceHole(Document doc, bool flag)
        {
            List<ElementId> idList = GetAllSumibuPiece(doc);
            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                try
                {
                    t.Start();
                    SetSumibuPieceHole(doc, idList, flag);
                    t.Commit();
                }
                catch (Exception e)
                {
                }
            }
        }
        #endregion
    }
}
