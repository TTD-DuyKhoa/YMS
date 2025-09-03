using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using YMS.DLG;
using static Autodesk.Revit.DB.SpecTypeId;
using Document = Autodesk.Revit.DB.Document;

namespace YMS.Parts
{
    public class ClsAtamaTsunagi
    {
        #region 定数

        public const string L75X75X6X150 = "L-75x75x6 L=150";
        public const string L75X75X6X300 = "L-75x75x6 L=300";
        public const string BL30 = "30BL";
        public const string C150X75X6_5 = "C-150X75X6.5";
        public const string C150X75X9 = "C-150X75X9";

        #endregion

        #region Enum

        /// <summary>
        /// チャンネル材の向き
        /// </summary>
        public enum ChannelDirection
        {
            //Cの開いている方向
            None,
            DirectionVertical,    //3時方向 右向き
            DirectionHorizontal   //6時方向 下向き
        }

        /// <summary>
        /// 接合方法
        /// </summary>
        public enum JointType
        {
            Welding,    //溶接
            Bolt,       //ボルト
            BurumanC,   //ブルマンC-50
            RikimanG,   //リキマンGタイプ
        }

        /// <summary>
        /// 配置方向
        /// </summary>
        public enum ConfigurationDirection
        {
            None,
            Front,      //前面(採掘側)
            Back,       //背面(山留側)
        }

        /// <summary>
        /// 作成方法
        /// </summary>
        public enum CreateType
        {
            PileTop,    //杭天端
            FromToP     //杭天端からの高さ入力
        }

        #endregion

        #region 変数

        /// <summary>
        /// 鋼材タイプ
        /// </summary>
        public string m_KouzaiType { get; set; }

        /// <summary>
        /// 鋼材サイズ
        /// </summary>
        public string m_KouzaiSize { get; set; }

        /// <summary>
        /// チャンネルの向き
        /// </summary>
        public ChannelDirection m_ChannelDirection { get; set; }

        /// <summary>
        /// 取付方法
        /// </summary>
        public JointType m_JointType { get; set; }

        /// <summary>
        /// ボルトタイプ
        /// </summary>
        public string m_BoltType { get; set; }

        /// <summary>
        /// ボルトサイズ
        /// </summary>
        public string m_BoltSize { get; set; }

        /// <summary>
        /// ボルトサイズ
        /// </summary>
        public int m_BoltNum { get; set; } //#33920

        /// <summary>
        /// 配置方向
        /// </summary>
        public ConfigurationDirection m_ConfigurationDirection { get; set; }

        /// <summary>
        /// 突き出し量 始点側
        /// </summary>
        public double m_TsukidashiSt { get; set; }

        /// <summary>
        /// 突き出し量 終点側
        /// </summary>
        public double m_TsukidashiEd { get; set; }

        /// <summary>
        /// 作成方法
        /// </summary>
        public CreateType m_CreateType { get; set; }

        /// <summary>
        /// 杭天端からの高さ
        /// </summary>
        public double m_HeightfromToP { get; set; }

        /// <summary>
        /// 取付補助材
        /// </summary>
        public string m_ToritsukeHojozai { get; set; }

        /// <summary>
        /// ブラケット
        /// </summary>
        public string m_Bracket { get; set; }

        #endregion

        public ClsAtamaTsunagi()
        {
            init();
        }

        public ClsAtamaTsunagi(Document doc, ElementId id)
        {
            init();
            GetCustomData(doc, id);
        }

        public void init()
        {
            m_KouzaiType = string.Empty;
            m_KouzaiSize = string.Empty;
            m_ChannelDirection = ChannelDirection.None;
            m_JointType = JointType.Welding;
            m_BoltType = string.Empty;
            m_BoltSize = string.Empty;
            m_BoltNum = 0; //#33920
            m_ToritsukeHojozai = string.Empty;
            m_Bracket = string.Empty;
            m_ConfigurationDirection = ConfigurationDirection.None;
            m_TsukidashiSt = 0;
            m_TsukidashiEd = 0;
            m_CreateType = CreateType.PileTop;
            m_HeightfromToP = 0;
        }

        /// <summary>
        /// 頭ツナギ材の作成
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="oyakuiIdSt">始点の親杭ID</param>
        /// <param name="oyakuiIdEd">終点の親杭ID</param>
        /// <returns></returns>
        public bool CreateAtamaTsunagiZai(Document doc, ElementId oyakuiIdSt, ElementId oyakuiIdEd)
        {
            // ファミリの取得処理
            string symbolFolpath = ClsZumenInfo.GetYMSFolder();
            string familyPath = string.Empty;
            switch (m_KouzaiType)
            {
                case Master.ClsChannelCsv.TypeCannel:
                    familyPath = Master.ClsChannelCsv.GetFamilyPath(m_KouzaiSize);
                    break;
                case Master.ClsHBeamCsv.TypeHoso:
                case Master.ClsHBeamCsv.TypeNaka:
                case Master.ClsHBeamCsv.TypeHiro:
                    familyPath = Master.ClsHBeamCsv.GetFamilyPath(m_KouzaiSize);
                    break;
                case Master.ClsAngleCsv.TypeAngle:
                    familyPath = Master.ClsAngleCsv.GetFamilyPath(m_KouzaiSize);
                    break;
                default:
                    return false;
            }
            string familyName = ClsRevitUtil.GetFamilyName(familyPath);

            if (!ClsRevitUtil.LoadFamilyData(doc, familyPath, out Family family))
            {
                return false;
            }

            string typeName = "頭ツナギ材";
            switch (m_ChannelDirection)
            {
                case ChannelDirection.DirectionHorizontal:  // 水平方向
                    typeName = "頭ツナギ材";
                    break;
                case ChannelDirection.DirectionVertical:    // 垂直方向
                    typeName = "頭ツナギ材2";
                    break;
                default:
                    break;
            }
            FamilySymbol sym = (ClsRevitUtil.GetFamilySymbol(doc, familyName, typeName));

            // 作図位置の算出処理
            FamilyInstance instOyaSt = doc.GetElement(oyakuiIdSt) as FamilyInstance;
            XYZ ptOyaSt = (instOyaSt.Location as LocationPoint).Point;
            XYZ instOyaStFacingOrientation = instOyaSt.FacingOrientation;
            FamilyInstance instOyaEd = doc.GetElement(oyakuiIdEd) as FamilyInstance;
            XYZ ptOyaEd = (instOyaEd.Location as LocationPoint).Point;
            XYZ instOyaEdFacingOrientation = instOyaEd.FacingOrientation;

            // 選択座標の補正
            if (m_ChannelDirection.Equals(ChannelDirection.DirectionHorizontal))
            {
                SwapStartAndEnd(instOyaSt, ref ptOyaSt, ref ptOyaEd);
            }
            else if (m_ChannelDirection.Equals(ChannelDirection.DirectionVertical))
            {
                if (instOyaSt.Symbol.FamilyName.Contains("杭"))
                {
                    var vec = instOyaSt.FacingOrientation;
                    if (vec.X < 0)
                    {
                        SwapStartAndEnd(instOyaSt, ref ptOyaSt, ref ptOyaEd);
                    }
                    else
                    {
                        SwapStartAndEnd(instOyaSt, ref ptOyaSt, ref ptOyaEd);
                        SwapStartAndEnd(instOyaSt, ref ptOyaEd, ref ptOyaSt);
                    }

                }
                else if (instOyaSt.Symbol.FamilyName.Contains("鋼矢板"))
                {
                    var vec = instOyaSt.FacingOrientation;
                    //if (vec.X < 0)
                    if(ClsGeo.IsLeft(ptOyaEd- ptOyaSt,vec))
                    {
                        //SwapStartAndEnd(instOyaSt, ref ptOyaSt, ref ptOyaEd);
                    }
                    else
                    {
                        FamilyInstance instOyaStTemp = instOyaSt;
                        instOyaSt = instOyaEd;
                        instOyaEd = instOyaStTemp;
                        //SwapStartAndEnd(instOyaSt, ref ptOyaSt, ref ptOyaEd);
                        //SwapStartAndEnd(instOyaSt, ref ptOyaEd, ref ptOyaSt);
                    }
                }
            }
            else if (m_KouzaiType == Master.ClsAngleCsv.TypeAngle)
            {
                var vec = instOyaSt.FacingOrientation;
                if (!ClsGeo.IsLeft(ptOyaEd - ptOyaSt, vec))
                {
                    FamilyInstance instOyaStTemp = instOyaSt;
                    instOyaSt = instOyaEd;
                    instOyaEd = instOyaStTemp;
                }
            }

            ElementId levelID = ClsRevitUtil.GetParameterElementId(doc, oyakuiIdSt, "集計レベル");

            double insertPointZ1 = 0.0;
            double insertPointZ2 = 0.0;
            if (m_CreateType == CreateType.PileTop)
            {
                // 杭天端
                insertPointZ1 = ClsRevitUtil.GetTypeParameter(instOyaSt.Symbol, "基準レベルからの高さ");
                insertPointZ2 = ClsRevitUtil.GetTypeParameter(instOyaEd.Symbol, "基準レベルからの高さ");
            }
            else
            {
                // 杭天端からの高さ
                Level lv = doc.GetElement(levelID) as Level;
                double elevation = lv.Elevation;
                insertPointZ1 = elevation - ClsRevitUtil.CovertToAPI(m_HeightfromToP);
                insertPointZ2 = elevation - ClsRevitUtil.CovertToAPI(m_HeightfromToP);
            }

            string oyaSymName = instOyaSt.Symbol.FamilyName;
            double oyaSize = double.NaN;
            if (oyaSymName.Contains("杭"))
            {
                oyaSize = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou1(oyaSymName, 1)) / 2);
            }
            else if (oyaSymName.Contains("鋼矢板"))
            {
                oyaSize = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(Master.ClsKouyaitaCsv.GetHeight(oyaSymName)));
                if (ClsKouyaita.GetVoidvec(doc, oyakuiIdSt) == 1)
                {
                    instOyaStFacingOrientation *= -1;
                    instOyaEdFacingOrientation *= -1;
                }
            }

            double atamaSize = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou(familyName, 1)) / 2);
            if (m_KouzaiType == "アングル") { atamaSize = atamaSize - atamaSize; }
            double offset = oyaSize + atamaSize;

            if (oyaSymName.Contains("鋼矢板"))
            {
                // 鋼矢板は挿入点が端部のため、中央に補正する

                //XYZ vec = (ptOyaEd - ptOyaSt).Normalize(); 
                //double oyaWidth = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(Master.ClsKouyaitaCsv.GetWidth(oyaSymName))) / 2;
                //ptOyaSt = ptOyaSt + (vec * oyaWidth);
                //ptOyaEd = ptOyaEd + (vec * oyaWidth);
                ptOyaSt = GetInsertPoint_Kouyaita(doc, instOyaSt, oyaSymName, false);
                ptOyaEd = GetInsertPoint_Kouyaita(doc, instOyaEd, oyaSymName, false);
            }

            XYZ ptAtamaSt = XYZ.Zero;
            XYZ ptAtamaEd = XYZ.Zero;
            if (m_KouzaiType.Contains("H形鋼") || m_KouzaiType == "主材")
            {
                if (m_ConfigurationDirection == ConfigurationDirection.Front)
                {
                    ptAtamaSt = new XYZ(ptOyaSt.X + (offset * instOyaStFacingOrientation.X), ptOyaSt.Y + (offset * instOyaStFacingOrientation.Y), insertPointZ1);
                    ptAtamaEd = new XYZ(ptOyaEd.X + (offset * instOyaEdFacingOrientation.X), ptOyaEd.Y + (offset * instOyaEdFacingOrientation.Y), insertPointZ2);
                }
                else
                {
                    ptAtamaSt = new XYZ(ptOyaSt.X - (offset * instOyaStFacingOrientation.X), ptOyaSt.Y - (offset * instOyaStFacingOrientation.Y), insertPointZ1);
                    ptAtamaEd = new XYZ(ptOyaEd.X - (offset * instOyaEdFacingOrientation.X), ptOyaEd.Y - (offset * instOyaEdFacingOrientation.Y), insertPointZ2);
                }
            }
            else if (m_KouzaiType.Contains("チャンネル"))
            {
                if (m_ChannelDirection.Equals(ChannelDirection.DirectionVertical))
                {
                    ptAtamaSt = new XYZ(ptOyaSt.X + (oyaSize * instOyaStFacingOrientation.X), ptOyaSt.Y + (oyaSize * instOyaStFacingOrientation.Y), insertPointZ1);
                    ptAtamaEd = new XYZ(ptOyaEd.X + (oyaSize * instOyaEdFacingOrientation.X), ptOyaEd.Y + (oyaSize * instOyaEdFacingOrientation.Y), insertPointZ2);
                }
                else if (m_ChannelDirection.Equals(ChannelDirection.DirectionHorizontal))
                {
                    ptAtamaSt = new XYZ(ptOyaSt.X + (offset * instOyaStFacingOrientation.X), ptOyaSt.Y + (offset * instOyaStFacingOrientation.Y), insertPointZ1);
                    ptAtamaEd = new XYZ(ptOyaEd.X + (offset * instOyaEdFacingOrientation.X), ptOyaEd.Y + (offset * instOyaEdFacingOrientation.Y), insertPointZ2);
                }
            }
            else
            {
                ptAtamaSt = new XYZ(ptOyaSt.X + (offset * instOyaStFacingOrientation.X), ptOyaSt.Y + (offset * instOyaStFacingOrientation.Y), insertPointZ1);
                ptAtamaEd = new XYZ(ptOyaEd.X + (offset * instOyaEdFacingOrientation.X), ptOyaEd.Y + (offset * instOyaEdFacingOrientation.Y), insertPointZ2);
            }

            XYZ vecAtama = (ptAtamaEd - ptAtamaSt).Normalize();

            // 頭ツナギ材は杭の中央ではなく端と端で作図される
            ptAtamaSt = ptAtamaSt + (-vecAtama * oyaSize);
            ptAtamaEd = ptAtamaEd + (vecAtama * oyaSize);

            // 突き出し量を加算する
            ptAtamaSt = ptAtamaSt + (-vecAtama * ClsRevitUtil.CovertToAPI(m_TsukidashiSt));
            ptAtamaEd = ptAtamaEd + (vecAtama * ClsRevitUtil.CovertToAPI(m_TsukidashiEd));

            // ファミリを作成するための芯を生成
            Curve cv = Line.CreateBound(ptAtamaSt, ptAtamaEd);

            // 作図処理
            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                t.Start();

                ElementId createdId = ClsRevitUtil.Create(doc, cv, levelID, sym);
                FamilyInstance instAtama = doc.GetElement(createdId) as FamilyInstance;

                if (m_CreateType == CreateType.FromToP)
                {
                    ClsRevitUtil.SetParameter(doc, createdId, "ホストからのオフセット", insertPointZ1);
                }
                else
                {
                    if (m_KouzaiType == "アングル")
                    {
                        // アングル材は挿入基点が中央ではなく下部のため
                        atamaSize = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou(familyName, 1)));
                        ClsRevitUtil.SetParameter(doc, createdId, "ホストからのオフセット", insertPointZ1 - atamaSize);
                    }
                    else
                    {
                        ClsRevitUtil.SetParameter(doc, createdId, "ホストからのオフセット", insertPointZ1 - atamaSize);
                    }
                }

                // インスタンスに拡張データを付与
                SetCustomData(doc, createdId);
                if (m_JointType == JointType.Bolt)
                {
                    //ボルト情報をカスタムデータとして設定する　#31575
                    //ClsYMSUtil.SetBolt(doc, createdId, m_BoltSize, 2);
                    ClsYMSUtil.SetBolt(doc, createdId, m_BoltSize, m_BoltNum); //#33920
                }
                else
                {
                    //ボルト情報のカスタムデータを削除する
                    ClsYMSUtil.DeleteBolt(doc, createdId);
                }
                t.Commit();
            }

            return true;
        }

        /// <summary>
        /// 頭ツナギ材(主材)の作成
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="oyakuiIdSt"></param>
        /// <param name="oyakuiIdEd"></param>
        /// <returns></returns>
        /// <remarks>主材は1.0m～7.0mの0.5mピッチで固定長のため</remarks>
        public bool CreateAtamaTsunagiZaiSyuzai(Document doc, ElementId oyakuiIdSt, ElementId oyakuiIdEd)
        {
            List<ElementId> createdIds = new List<ElementId>(); // 作成した頭ツナギ材のIDリスト
            List<XYZ> createdXYZs = new List<XYZ>();            // 作成した頭ツナギ材の座標リスト
            XYZ newPoint = null;

            while (true)
            {
                DlgCreateAtamatsunagiSyuzai dlgCreateAtamatsunagiSyuzai = new DlgCreateAtamatsunagiSyuzai();
                DialogResult result = dlgCreateAtamatsunagiSyuzai.ShowDialog();
                if (result != DialogResult.OK)
                {
                    break;
                }

                // Undo処理
                if (dlgCreateAtamatsunagiSyuzai.m_Length == 0.0 && result == DialogResult.OK)
                {
                    if (createdIds.Count > 0 && createdXYZs.Count > 0)
                    {
                        using (Transaction transaction = new Transaction(doc, "Delete Family"))
                        {
                            transaction.Start();

                            // ファミリの削除処理
                            ICollection<ElementId> deletedFamilyIds = doc.Delete(createdIds.Last());

                            transaction.Commit();

                            newPoint = createdXYZs.Last();

                            createdIds.RemoveAt(createdIds.Count - 1);
                            createdXYZs.RemoveAt(createdXYZs.Count - 1);
                        }
                    }
                    continue;
                }

                // ファミリの取得処理
                string familyPath = string.Empty;
                familyPath = Master.ClsYamadomeCsv.GetFamilyPath(m_KouzaiSize, dlgCreateAtamatsunagiSyuzai.m_Length);
                string tmp = ClsRevitUtil.GetFamilyName(familyPath);
                if (!tmp.Contains("."))
                {
                    tmp += ".0";

                    familyPath = familyPath.Replace(".rfa", ".0.rfa");
                }
                string familyName = tmp;
                if (!ClsRevitUtil.LoadFamilyData(doc, familyPath, out Family family))
                {
                    return false;
                }
                FamilySymbol sym = (ClsRevitUtil.GetFamilySymbol(doc, familyName, "頭ツナギ材"));

                // 作図位置の算出処理
                FamilyInstance instOyaSt = doc.GetElement(oyakuiIdSt) as FamilyInstance;
                XYZ ptOyaSt = instOyaSt.GetTransform().Origin;
                XYZ instOyaStFacingOrientation = instOyaSt.FacingOrientation;
                FamilyInstance instOyaEd = doc.GetElement(oyakuiIdEd) as FamilyInstance;
                XYZ ptOyaEd = instOyaEd.GetTransform().Origin;
                XYZ instOyaEdFacingOrientation = instOyaEd.FacingOrientation;

                ElementId levelID = ClsRevitUtil.GetParameterElementId(doc, oyakuiIdSt, "集計レベル");

                double insertPointZ1 = 0.0;
                double insertPointZ2 = 0.0;
                if (m_CreateType == CreateType.PileTop)
                {
                    // 杭天端
                    insertPointZ1 = ClsRevitUtil.GetTypeParameter(instOyaSt.Symbol, "基準レベルからの高さ");
                    insertPointZ2 = ClsRevitUtil.GetTypeParameter(instOyaEd.Symbol, "基準レベルからの高さ");
                }
                else
                {
                    // 杭天端からの高さ
                    Level lv = doc.GetElement(levelID) as Level;
                    double elevation = lv.Elevation;
                    insertPointZ1 = elevation - ClsRevitUtil.CovertToAPI(m_HeightfromToP);
                    insertPointZ2 = elevation - ClsRevitUtil.CovertToAPI(m_HeightfromToP);
                }

                string oyaSymName = instOyaSt.Symbol.FamilyName;
                double oyaSize = double.NaN;
                if (oyaSymName.Contains("杭"))
                {
                    oyaSize = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou1(oyaSymName, 1)) / 2);
                }
                else if (oyaSymName.Contains("鋼矢板"))
                {
                    oyaSize = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(Master.ClsKouyaitaCsv.GetHeight(oyaSymName)));
                    if (ClsKouyaita.GetVoidvec(doc, oyakuiIdSt) == 1)
                    {
                        instOyaStFacingOrientation *= -1;
                        instOyaEdFacingOrientation *= -1;
                    }
                }

                // 挿入点の補正処理
                if (oyaSymName.Contains("杭"))
                {
                    if (m_ConfigurationDirection == ConfigurationDirection.Front)
                    {
                        ptOyaSt = ptOyaSt + (oyaSize * instOyaStFacingOrientation);
                        ptOyaEd = ptOyaEd + (oyaSize * instOyaEdFacingOrientation);
                    }
                    else
                    {
                        ptOyaSt = ptOyaSt - (oyaSize * instOyaStFacingOrientation);
                        ptOyaEd = ptOyaEd - (oyaSize * instOyaEdFacingOrientation);
                    }
                }
                else if (oyaSymName.Contains("鋼矢板"))
                {
                    XYZ ptOyaSt_Origin = ptOyaSt;
                    XYZ ptOyaEd_Origin = ptOyaEd;
                    //ptOyaSt = GetInsertPoint_Kouyaita(doc, instOyaSt, ptOyaSt_Origin, ptOyaEd_Origin, oyaSymName);
                    //ptOyaEd = GetInsertPoint_Kouyaita(doc, instOyaEd, ptOyaSt_Origin, ptOyaEd_Origin, oyaSymName);

                    ptOyaSt = GetInsertPoint_Kouyaita(doc, instOyaSt, oyaSymName, true);
                    ptOyaEd = GetInsertPoint_Kouyaita(doc, instOyaEd, oyaSymName, true);
                }

                // 頭ツナギ材のサイズ分だけオフセットする
                double atamaSize = ClsRevitUtil.CovertToAPI((ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetSyuzaiSize(sym.FamilyName).Replace("HA", "")) * 10) / 2);
                XYZ ptAtamaSt = XYZ.Zero;
                XYZ ptAtamaEd = XYZ.Zero;
                if (m_ConfigurationDirection == ConfigurationDirection.Front)
                {
                    ptAtamaSt = new XYZ(ptOyaSt.X + (atamaSize * instOyaStFacingOrientation.X), ptOyaSt.Y + (atamaSize * instOyaStFacingOrientation.Y), insertPointZ1);
                    ptAtamaEd = new XYZ(ptOyaEd.X + (atamaSize * instOyaEdFacingOrientation.X), ptOyaEd.Y + (atamaSize * instOyaEdFacingOrientation.Y), insertPointZ2);
                }
                else
                {
                    ptAtamaSt = new XYZ(ptOyaSt.X - (atamaSize * instOyaStFacingOrientation.X), ptOyaSt.Y - (atamaSize * instOyaStFacingOrientation.Y), insertPointZ1);
                    ptAtamaEd = new XYZ(ptOyaEd.X - (atamaSize * instOyaEdFacingOrientation.X), ptOyaEd.Y - (atamaSize * instOyaEdFacingOrientation.Y), insertPointZ2);
                }

                // 頭ツナギ材は杭の中央ではなく端と端で作図される
                XYZ vecAtama = (ptAtamaEd - ptAtamaSt).Normalize();
                ptAtamaSt = ptAtamaSt + (-vecAtama * oyaSize);
                ptAtamaEd = ptAtamaEd + (vecAtama * oyaSize);

                if (newPoint != null)
                {
                    ptAtamaSt = newPoint;
                }

                // 作図処理
                using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                {
                    t.Start();

                    ElementId createdId = ClsRevitUtil.Create(doc, ptAtamaSt, levelID, sym);
                    FamilyInstance instAT = doc.GetElement(createdId) as FamilyInstance;

                    // シンボルを回転
                    Line rotationAxis = Line.CreateBound(ptAtamaSt, ptAtamaSt + XYZ.BasisZ);    // Z軸周りに回転
                    double radians = XYZ.BasisX.AngleOnPlaneTo(vecAtama, XYZ.BasisZ);           // シンボルの向き（デフォルトは3時方向）
                    ElementTransformUtils.RotateElement(doc, createdId, rotationAxis, radians);

                    // 高さの設定
                    ClsRevitUtil.SetParameter(doc, createdId, "ホストからのオフセット", insertPointZ1 - atamaSize);

                    // インスタンスに拡張データを付与
                    SetCustomData(doc, createdId);
                    if (m_JointType == JointType.Bolt)
                    {
                        //ボルト情報をカスタムデータとして設定する　#31575
                        ClsYMSUtil.SetBolt(doc, createdId, m_BoltSize, 1);
                    }
                    else 
                    {
                        //ボルト情報のカスタムデータを削除する
                        ClsYMSUtil.DeleteBolt(doc, createdId);
                    }
                    
                    t.Commit();

                    if (createdId != null)
                    {
                        // 作成済みリストに追加
                        createdIds.Add(createdId);
                        createdXYZs.Add(ptAtamaSt);

                        // 作図地点の更新
                        newPoint = ptAtamaSt + (vecAtama * ClsRevitUtil.ConvertDoubleGeo2Revit((dlgCreateAtamatsunagiSyuzai.m_Length * 1000)));
                    }
                }

            }

            return true;
        }
        /// <summary>
        /// 頭ツナギ材(主材)の作成
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="oyakuiIdSt"></param>
        /// <param name="oyakuiIdEd"></param>
        /// <returns></returns>
        /// <remarks>主材は1.0m～7.0mの0.5mピッチで固定長のため</remarks>
        public bool CreateAutoAtamaTsunagiZaiSyuzai(Document doc, ElementId oyakuiIdSt, ElementId oyakuiIdEd)
        {
            XYZ newPoint = null;

            // 作図位置の算出処理
            FamilyInstance instOyaSt = doc.GetElement(oyakuiIdSt) as FamilyInstance;
            XYZ ptOyaSt = instOyaSt.GetTransform().Origin;
            XYZ instOyaStFacingOrientation = instOyaSt.FacingOrientation;
            FamilyInstance instOyaEd = doc.GetElement(oyakuiIdEd) as FamilyInstance;
            XYZ ptOyaEd = instOyaEd.GetTransform().Origin;
            XYZ instOyaEdFacingOrientation = instOyaEd.FacingOrientation;

            int totalLength = (int)ClsRevitUtil.CovertFromAPI(Line.CreateBound(ptOyaSt, ptOyaEd).Length);
            //1000未満の主材は存在しない
            if (totalLength < 1000)
                return false;
            int quotient = Math.DivRem(totalLength, 7000, out int remainder);
            remainder = (remainder / 1000) % 10;
            List<double> lengthList = new List<double>();
            for(int i = 0; i < quotient; i++)
            {
                lengthList.Add(7);
            }
            lengthList.Add(remainder);

            foreach (var length in lengthList)
            {
                // ファミリの取得処理
                string familyPath = string.Empty;
                familyPath = Master.ClsYamadomeCsv.GetFamilyPath(m_KouzaiSize, length);
                string tmp = ClsRevitUtil.GetFamilyName(familyPath);
                if (!tmp.Contains("."))
                {
                    tmp += ".0";

                    familyPath = familyPath.Replace(".rfa", ".0.rfa");
                }
                string familyName = tmp;
                if (!ClsRevitUtil.LoadFamilyData(doc, familyPath, out Family family))
                {
                    return false;
                }
                FamilySymbol sym = (ClsRevitUtil.GetFamilySymbol(doc, familyName, "頭ツナギ材"));

                ElementId levelID = ClsRevitUtil.GetParameterElementId(doc, oyakuiIdSt, "集計レベル");

                double insertPointZ1 = 0.0;
                double insertPointZ2 = 0.0;
                if (m_CreateType == CreateType.PileTop)
                {
                    // 杭天端
                    insertPointZ1 = ClsRevitUtil.GetTypeParameter(instOyaSt.Symbol, "基準レベルからの高さ");
                    insertPointZ2 = ClsRevitUtil.GetTypeParameter(instOyaEd.Symbol, "基準レベルからの高さ");
                }
                else
                {
                    // 杭天端からの高さ
                    Level lv = doc.GetElement(levelID) as Level;
                    double elevation = lv.Elevation;
                    insertPointZ1 = elevation - ClsRevitUtil.CovertToAPI(m_HeightfromToP);
                    insertPointZ2 = elevation - ClsRevitUtil.CovertToAPI(m_HeightfromToP);
                }

                string oyaSymName = instOyaSt.Symbol.FamilyName;
                double oyaSize = double.NaN;
                if (oyaSymName.Contains("杭"))
                {
                    oyaSize = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou1(oyaSymName, 1)) / 2);
                }
                else if (oyaSymName.Contains("鋼矢板"))
                {
                    oyaSize = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(Master.ClsKouyaitaCsv.GetHeight(oyaSymName)));
                    if (ClsKouyaita.GetVoidvec(doc, oyakuiIdSt) == 1)
                    {
                        instOyaStFacingOrientation *= -1;
                        instOyaEdFacingOrientation *= -1;
                    }
                }

                // 挿入点の補正処理
                if (oyaSymName.Contains("杭"))
                {
                    if (m_ConfigurationDirection == ConfigurationDirection.Front)
                    {
                        ptOyaSt = ptOyaSt + (oyaSize * instOyaStFacingOrientation);
                        ptOyaEd = ptOyaEd + (oyaSize * instOyaEdFacingOrientation);
                    }
                    else
                    {
                        ptOyaSt = ptOyaSt - (oyaSize * instOyaStFacingOrientation);
                        ptOyaEd = ptOyaEd - (oyaSize * instOyaEdFacingOrientation);
                    }
                }
                else if (oyaSymName.Contains("鋼矢板"))
                {
                    XYZ ptOyaSt_Origin = ptOyaSt;
                    XYZ ptOyaEd_Origin = ptOyaEd;
                    ptOyaSt = GetInsertPoint_Kouyaita(doc, instOyaSt, ptOyaSt_Origin, ptOyaEd_Origin, oyaSymName);
                    ptOyaEd = GetInsertPoint_Kouyaita(doc, instOyaEd, ptOyaSt_Origin, ptOyaEd_Origin, oyaSymName);
                }

                // 頭ツナギ材のサイズ分だけオフセットする
                double atamaSize = ClsRevitUtil.CovertToAPI((ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetSyuzaiSize(sym.FamilyName).Replace("HA", "")) * 10) / 2);
                XYZ ptAtamaSt = XYZ.Zero;
                XYZ ptAtamaEd = XYZ.Zero;
                if (m_ConfigurationDirection == ConfigurationDirection.Front)
                {
                    ptAtamaSt = new XYZ(ptOyaSt.X + (atamaSize * instOyaStFacingOrientation.X), ptOyaSt.Y + (atamaSize * instOyaStFacingOrientation.Y), insertPointZ1);
                    ptAtamaEd = new XYZ(ptOyaEd.X + (atamaSize * instOyaEdFacingOrientation.X), ptOyaEd.Y + (atamaSize * instOyaEdFacingOrientation.Y), insertPointZ2);
                }
                else
                {
                    ptAtamaSt = new XYZ(ptOyaSt.X - (atamaSize * instOyaStFacingOrientation.X), ptOyaSt.Y - (atamaSize * instOyaStFacingOrientation.Y), insertPointZ1);
                    ptAtamaEd = new XYZ(ptOyaEd.X - (atamaSize * instOyaEdFacingOrientation.X), ptOyaEd.Y - (atamaSize * instOyaEdFacingOrientation.Y), insertPointZ2);
                }

                // 頭ツナギ材は杭の中央ではなく端と端で作図される
                XYZ vecAtama = (ptAtamaEd - ptAtamaSt).Normalize();
                ptAtamaSt = ptAtamaSt + (-vecAtama * oyaSize);
                ptAtamaEd = ptAtamaEd + (vecAtama * oyaSize);

                if (newPoint != null)
                {
                    ptAtamaSt = newPoint;
                }

                // 作図処理
                using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                {
                    t.Start();

                    ElementId createdId = ClsRevitUtil.Create(doc, ptAtamaSt, levelID, sym);
                    FamilyInstance instAT = doc.GetElement(createdId) as FamilyInstance;

                    // シンボルを回転
                    Line rotationAxis = Line.CreateBound(ptAtamaSt, ptAtamaSt + XYZ.BasisZ);    // Z軸周りに回転
                    double radians = XYZ.BasisX.AngleOnPlaneTo(vecAtama, XYZ.BasisZ);           // シンボルの向き（デフォルトは3時方向）
                    ElementTransformUtils.RotateElement(doc, createdId, rotationAxis, radians);

                    // 高さの設定
                    ClsRevitUtil.SetParameter(doc, createdId, "ホストからのオフセット", insertPointZ1 - atamaSize);

                    // インスタンスに拡張データを付与
                    SetCustomData(doc, createdId);
                    if (m_JointType == JointType.Bolt)
                    {
                        //ボルト情報をカスタムデータとして設定する　#31575
                        ClsYMSUtil.SetBolt(doc, createdId, m_BoltSize, 1);
                    }
                    else
                    {
                        //ボルト情報のカスタムデータを削除する
                        ClsYMSUtil.DeleteBolt(doc, createdId);
                    }
                    t.Commit();

                    if (createdId != null)
                    {
                        // 作図地点の更新
                        newPoint = ptAtamaSt + (vecAtama * ClsRevitUtil.ConvertDoubleGeo2Revit((length * 1000)));
                    }
                }

            }

            return true;
        }
        /// <summary>
        /// 頭ツナギ補助材の作成
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="idOyakui"></param>
        /// <param name="idAtama"></param>
        /// <param name="isAuto"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public bool CreateAtamaTsunagiHojoZai(Document doc, ElementId idOyakui, ElementId idAtama, bool isAuto = false, double interval = 0.0, bool isSide = false)
        {
            // 頭ツナギ材の情報を取得する
            FamilyInstance instAtama = doc.GetElement(idAtama) as FamilyInstance;

            // 頭ツナギ材の始点と終点
            XYZ ptAtamaSt = new XYZ();
            XYZ ptAtamaEd = new XYZ();
            LocationCurve lCurve = instAtama.Location as LocationCurve;
            if (lCurve == null)
            {
                LocationPoint location = instAtama.Location as LocationPoint;
                XYZ position = location.Point;
                XYZ facingOrientation = instAtama.FacingOrientation;
                double atamaLength = ClsRevitUtil.CovertToAPI(ClsYMSUtil.GetSyuzaiLength(instAtama.Symbol.Family.Name));
                if (atamaLength == 0)
                {
                    return false;
                }
                ptAtamaSt = position;
                Transform rotation = Transform.CreateRotationAtPoint(XYZ.BasisZ, -Math.PI / 2, position);
                facingOrientation = rotation.OfVector(facingOrientation);
                ptAtamaEd = position + (atamaLength * facingOrientation);
            }
            else
            {
                ptAtamaSt = lCurve.Curve.GetEndPoint(0);
                ptAtamaEd = lCurve.Curve.GetEndPoint(1);
            }

            // 杭または鋼矢板の情報を取得する
            FamilyInstance instOyakui = doc.GetElement(idOyakui) as FamilyInstance;
            if (instOyakui.Symbol.FamilyName.Contains("杭"))
            {
                SwapStartAndEnd(instOyakui, ref ptAtamaSt, ref ptAtamaEd);
            }

            //杭または鋼矢板の掘削向きを取得
            XYZ oyaDir = instOyakui.FacingOrientation;

            // 頭ツナギ材の高さ
            double atamaHeight = double.NaN;
            if (m_KouzaiType == "主材")
            {
                atamaHeight = ClsRevitUtil.CovertToAPI((ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetSyuzaiSize(instAtama.Symbol.FamilyName).Replace("HA", "")) * 10) / 2);
            }
            else if (m_KouzaiType == "アングル")
            {
                atamaHeight = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou(instAtama.Symbol.FamilyName, 1)));
            }
            else
            {
                atamaHeight = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou(instAtama.Symbol.FamilyName, 1)) / 2);
            }

            //頭繋ぎ材が垂直方向かの判定
            bool vertical = false;
            if (instAtama.Symbol.Name.Contains("2"))
                vertical = true;

            // 頭ツナギ材のオフセット量
            double atamaOffset = ClsRevitUtil.GetParameterDouble(doc, idAtama, "ホストからのオフセット");

            //断面の場合は90度回す
            double danmenAngle = 0.0;
            if (isSide)
                danmenAngle = Math.PI / 2;
            double danmenAngleLow = (m_Bracket == BL30? 0:Math.PI / 2);

            // ブラケット(下部パーツ)の作成
            if (m_Bracket != "")
            {
                // ファミリの取得処理
                string familyPathLow = string.Empty;
                string typeName = string.Empty;
                double atamaSize = 0.0;
                switch (m_Bracket)
                {
                    case L75X75X6X150:
                        familyPathLow = Master.ClsNekozai.GetFamilyPath("L-75x75x6", "D-15");
                        typeName = "頭ﾂﾅｷﾞ取付補助材(下)";
                        atamaSize = 150.0;
                        break;
                    case L75X75X6X300:
                        familyPathLow = Master.ClsNekozai.GetFamilyPath("L-75x75x6", "D-30");
                        typeName = "頭ﾂﾅｷﾞ取付補助材(下)";
                        atamaSize = 300.0;
                        break;
                    case BL30:
                        familyPathLow = Master.ClsBracketCsv.GetFamilyPath(m_Bracket);
                        typeName = "頭ツナギBL";
                        break;
                    case C150X75X6_5:
                    case C150X75X9:
                        familyPathLow = Master.ClsChannelCsv.GetFamilyPath(m_Bracket);
                        typeName = "頭ツナギ材";
                        break;
                    default:
                        return false;
                }
                string familyNameLow = ClsRevitUtil.GetFamilyName(familyPathLow);
                if (!ClsRevitUtil.LoadFamilyData(doc, familyPathLow, out Family family2))
                {
                    return false;
                }
                FamilySymbol symLow = (ClsRevitUtil.GetFamilySymbol(doc, familyNameLow, typeName));

                // 作図処理本体
                if (isAuto)
                {
                    // 自動の場合
                    // ブラケット間隔で終点までブラケットを作図

                    string oyaSymName = instOyakui.Symbol.FamilyName;

                    // 挿入点を算出
                    XYZ insertPoint = XYZ.Zero;
                    XYZ centerPoint = XYZ.Zero;
                    double oyaSize = double.NaN;
                    List<ElementId> oyakuiIds = new List<ElementId>();

                    if (oyaSymName.Contains("杭"))
                    {
                        if (m_ConfigurationDirection == ConfigurationDirection.Front)
                        {
                            insertPoint = GetInsertPoint_Oyakui(doc, instOyakui, oyaSymName, ref centerPoint, true);
                        }
                        else if (m_ConfigurationDirection == ConfigurationDirection.Back)
                        {
                            insertPoint = GetInsertPoint_Oyakui(doc, instOyakui, oyaSymName, ref centerPoint, false);
                        }
                        else
                        {
                            insertPoint = GetInsertPoint_Oyakui(doc, instOyakui, oyaSymName, ref centerPoint, true);
                        }

                        oyaSize = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou1(oyaSymName, 1)) / 2);
                    }
                    else if (oyaSymName.Contains("鋼矢板"))
                    {
                        //insertPoint = GetInsertPoint_Kouyaita(doc, instOyakui, oyaSymName, ref centerPoint);
                        insertPoint = GetInsertPoint_Kouyaita(doc, instOyakui, oyaSymName, ref centerPoint);
                        //oyaSize = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(Master.ClsKouyaitaCsv.GetHeight(oyaSymName)));
                        oyaSize = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(Master.ClsKouyaitaCsv.GetWidth(oyaSymName))/2);
                    }

                    // 判定用の頭ツナギ材の線分を作成する
                    XYZ vecAtamaStoE = XYZ.Zero;
                    XYZ ptAtamaOriginEnd = XYZ.Zero;
                    XYZ ptAtamaOriginStr = XYZ.Zero;
                    if (centerPoint.DistanceTo(ptAtamaSt) > centerPoint.DistanceTo(ptAtamaEd))
                    {
                        vecAtamaStoE = (ptAtamaSt - ptAtamaEd).Normalize();
                        ptAtamaOriginEnd = new XYZ(ptAtamaSt.X, ptAtamaSt.Y, 0);
                        ptAtamaOriginStr = new XYZ(ptAtamaEd.X, ptAtamaEd.Y, 0);
                    }
                    else
                    {
                        vecAtamaStoE = (ptAtamaEd - ptAtamaSt).Normalize();
                        ptAtamaOriginEnd = new XYZ(ptAtamaEd.X, ptAtamaEd.Y, 0);
                        ptAtamaOriginStr = new XYZ(ptAtamaSt.X, ptAtamaSt.Y, 0);
                    }

                    double atamaTotalLength = ptAtamaSt.DistanceTo(ptAtamaEd);
                    double atamaPileSpacingLength = double.NaN;
                    atamaPileSpacingLength = atamaTotalLength - (ClsRevitUtil.CovertToAPI(m_TsukidashiSt) + ClsRevitUtil.CovertToAPI(m_TsukidashiEd) + (oyaSize * 2));

                    XYZ ptLineSt = new XYZ(insertPoint.X, insertPoint.Y, 0);
                    XYZ ptLineEd = ptLineSt + (atamaPileSpacingLength * vecAtamaStoE);
                    ptLineSt = ptLineSt + (oyaSize * -vecAtamaStoE);
                    ptLineEd = ptLineEd + (oyaSize * vecAtamaStoE);

                    XYZ ptLineEdTemp = GetClosestPointOnSegment(ptLineSt, ptLineEd, ptAtamaOriginEnd);
                    XYZ ptLineStTemp = GetClosestPointOnSegment(ptLineSt, ptLineEd, ptAtamaOriginStr);
                    ptLineSt = ptLineStTemp;
                    ptLineEd = ptLineEdTemp;

                    Line lineAtama = Line.CreateBound(ptLineSt, ptLineEd);
                    //CreateDebugLine(doc, centerPoint, lineAtama);

                    while (true)
                    {
                        if (m_Bracket == BL30 || m_Bracket == L75X75X6X150 || m_Bracket == L75X75X6X300)
                        {
                            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                            {
                                t.Start();

                                ElementId levelID = ClsRevitUtil.GetParameterElementId(doc, idOyakui, "集計レベル");

                                ElementId createdId = ClsRevitUtil.Create(doc, insertPoint, levelID, symLow);
                                FamilyInstance instLowParts = doc.GetElement(createdId) as FamilyInstance;

                                // 部材を杭の向きに合わせて回転
                                double angle = GetRotateAngle(centerPoint, insertPoint, instLowParts);
                                //ElementTransformUtils.RotateElement(doc, createdId, Line.CreateBound(insertPoint, insertPoint + XYZ.BasisZ), angle + danmenAngle);
                                ElementTransformUtils.RotateElement(doc, createdId, Line.CreateBound(insertPoint, insertPoint + XYZ.BasisZ), angle + danmenAngleLow); //#33967 ブラケットは断面固定

                                // 部材の高さを設定
                                ClsRevitUtil.SetParameter(doc, createdId, "ホストからのオフセット", (atamaOffset - atamaHeight));

                                //断面の場合は移動
                                //if (isSide)　 //#33967 ブラケットは断面固定
                                ClsRevitUtil.MoveFamilyInstance(instLowParts, atamaSize / 2, oyaDir);

                                t.Commit();
                            }
                        }
                        else if (m_Bracket == C150X75X6_5 || m_Bracket == C150X75X9)
                        {
                            // この分岐に来るという事はH鋼材か主材を使用した背面(山留壁側)配置である事が確定

                            XYZ insertPointSt = insertPoint;
                            XYZ insertPointEd = insertPoint;

                            // ブラケットの長さ L=頭ツナギ材の幅 + 100
                            double extLength = double.NaN;
                            if (m_KouzaiType == "主材")
                            {
                                extLength = ClsRevitUtil.CovertToAPI((ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetSyuzaiSize(instAtama.Symbol.FamilyName).Replace("HA", "")) * 10) + 100.0);
                            }
                            else
                            {
                                extLength = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou(instAtama.Symbol.FamilyName, 1)) + 100.0);
                            }

                            // ブラケットの向きを補正
                            if (oyaSymName.Contains("杭"))
                            {
                                insertPointEd = insertPointSt - (extLength * instOyakui.FacingOrientation);
                            }
                            else if (oyaSymName.Contains("鋼矢板"))
                            {
                                insertPointEd = insertPointSt + (extLength * instOyakui.FacingOrientation);
                            }

                            Curve cv = Line.CreateBound(insertPointSt, insertPointEd);
                            //CreateDebugLine(doc, centerPoint, cv);

                            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                            {
                                t.Start();

                                ElementId levelID = ClsRevitUtil.GetParameterElementId(doc, idOyakui, "集計レベル");

                                ElementId createdId = ClsRevitUtil.Create(doc, cv, levelID, symLow);
                                FamilyInstance instLowParts = doc.GetElement(createdId) as FamilyInstance;

                                ClsRevitUtil.SetParameter(doc, createdId, "ホストからのオフセット", (atamaOffset - atamaHeight));

                                t.Commit();
                            }
                        }

                        centerPoint = centerPoint + (ClsRevitUtil.ConvertDoubleGeo2Revit(interval) * vecAtamaStoE);
                        insertPoint = insertPoint + (ClsRevitUtil.ConvertDoubleGeo2Revit(interval) * vecAtamaStoE);
                        XYZ nextPoint = insertPoint + (ClsRevitUtil.ConvertDoubleGeo2Revit(interval) * vecAtamaStoE);
                        Line tmpline = Line.CreateBound(ptLineSt, nextPoint);
                        //if (tmpline.Length > lineAtama.Length)
                        //{
                        //    break;
                        //}
                        if (insertPoint.DistanceTo(ptLineSt) >= ptAtamaOriginEnd.DistanceTo(ptLineSt))
                        {
                            break;
                        }
                    }
                }
                else
                {
                    // 手動の場合
                    // 対象となる杭を選択する
                    List<ElementId> oyakuiIdList = null;
                    if (!ClsAtamaTsunagi.PickObjectsKabe(new UIDocument(doc), ref oyakuiIdList, "ブラケットを作成する親杭 or 鋼矢板"))
                    {
                        return false;
                    }

                    // 選択された杭に対して下部パーツを配置
                    foreach (ElementId id in oyakuiIdList)
                    {
                        FamilyInstance instTrgOyakui = doc.GetElement(id) as FamilyInstance;
                        XYZ ptTrgOyakui = instTrgOyakui.GetTotalTransform().Origin;
                        string oyaSymName = instTrgOyakui.Symbol.FamilyName;

                        // 挿入点を算出
                        XYZ insertPoint = XYZ.Zero;
                        XYZ centerPoint = XYZ.Zero;
                        if (oyaSymName.Contains("杭"))
                        {
                            if (m_ConfigurationDirection == ConfigurationDirection.Front)
                            {
                                insertPoint = GetInsertPoint_Oyakui(doc, instTrgOyakui, oyaSymName, ref centerPoint, true);
                            }
                            else if (m_ConfigurationDirection == ConfigurationDirection.Back)
                            {
                                insertPoint = GetInsertPoint_Oyakui(doc, instTrgOyakui, oyaSymName, ref centerPoint, false);
                            }
                            else
                            {
                                insertPoint = GetInsertPoint_Oyakui(doc, instTrgOyakui, oyaSymName, ref centerPoint, true);
                            }
                        }
                        else if (oyaSymName.Contains("鋼矢板"))
                        {
                            insertPoint = GetInsertPoint_Kouyaita(doc, instTrgOyakui, oyaSymName, ref centerPoint);
                        }

                        if (m_Bracket == BL30 || m_Bracket == L75X75X6X150 || m_Bracket == L75X75X6X300)
                        {
                            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                            {
                                t.Start();

                                ElementId levelID = ClsRevitUtil.GetParameterElementId(doc, id, "集計レベル");

                                ElementId createdId = ClsRevitUtil.Create(doc, insertPoint, levelID, symLow);
                                FamilyInstance instLowParts = doc.GetElement(createdId) as FamilyInstance;

                                // 部材を杭の向きに合わせて回転
                                double angle = GetRotateAngle(centerPoint, insertPoint, instLowParts);
                                //ElementTransformUtils.RotateElement(doc, createdId, Line.CreateBound(insertPoint, insertPoint + XYZ.BasisZ), angle + danmenAngle);
                                ElementTransformUtils.RotateElement(doc, createdId, Line.CreateBound(insertPoint, insertPoint + XYZ.BasisZ), angle + danmenAngleLow); //#33967 ブラケットは断面固定

                                // 部材の高さを設定
                                ClsRevitUtil.SetParameter(doc, createdId, "ホストからのオフセット", (atamaOffset - atamaHeight));

                                //断面の場合は移動
                                //if (isSide) //#33967 ブラケットは断面固定
                                ClsRevitUtil.MoveFamilyInstance(instLowParts, atamaSize / 2, oyaDir);

                                t.Commit();
                            }
                        }
                        else if (m_Bracket == C150X75X6_5 || m_Bracket == C150X75X9)
                        {
                            // この分岐に来るという事はH鋼材か主材を使用した背面(山留壁側)配置である事が確定

                            XYZ insertPointSt = insertPoint;
                            XYZ insertPointEd = insertPoint;

                            // ブラケットの長さ L=頭ツナギ材の幅 + 100
                            double extLength = double.NaN;
                            if (m_KouzaiType == "主材")
                            {
                                extLength = ClsRevitUtil.CovertToAPI((ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetSyuzaiSize(instAtama.Symbol.FamilyName).Replace("HA", "")) * 10) + 100.0);
                            }
                            else
                            {
                                extLength = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou(instAtama.Symbol.FamilyName, 1)) + 100.0);
                            }

                            // ブラケットの向きを補正
                            if (oyaSymName.Contains("杭"))
                            {
                                insertPointEd = insertPointSt - (extLength * instTrgOyakui.FacingOrientation);
                            }
                            else if (oyaSymName.Contains("鋼矢板"))
                            {
                                insertPointEd = insertPointSt + (extLength * instTrgOyakui.FacingOrientation);
                            }

                            Curve cv = Line.CreateBound(insertPointSt, insertPointEd);
                            //CreateDebugLine(doc, centerPoint, cv);

                            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                            {
                                t.Start();

                                ElementId levelID = ClsRevitUtil.GetParameterElementId(doc, id, "集計レベル");

                                ElementId createdId = ClsRevitUtil.Create(doc, cv, levelID, symLow);
                                FamilyInstance instLowParts = doc.GetElement(createdId) as FamilyInstance;

                                ClsRevitUtil.SetParameter(doc, createdId, "ホストからのオフセット", (atamaOffset - atamaHeight));

                                t.Commit();
                            }
                        }
                    }
                }
            }

            // 取付補助材(上部パーツ)の作成
            if (m_ToritsukeHojozai != "")
            {
                // ファミリの取得
                string familyPathUp = Master.ClsNekozai.GetFamilyPath("L-75x75x6", "D-15");
                string familyNameUp = ClsRevitUtil.GetFamilyName(familyPathUp);
                if (!ClsRevitUtil.LoadFamilyData(doc, familyPathUp, out Family family1))
                {
                    return false;
                }
                FamilySymbol symUp = (ClsRevitUtil.GetFamilySymbol(doc, familyNameUp, "頭ﾂﾅｷﾞ取付補助材"));

                string oyaSymName = instOyakui.Symbol.FamilyName;
                double atamaSize = 150.0;

                // 挿入点を算出
                XYZ insertPoint = XYZ.Zero;
                XYZ centerPoint = XYZ.Zero;
                double oyaSize = double.NaN;
                List<ElementId> oyakuiIds = new List<ElementId>();

                if (oyaSymName.Contains("杭"))
                {
                    if (m_ConfigurationDirection == ConfigurationDirection.Front)
                    {
                        insertPoint = GetInsertPoint_Oyakui(doc, instOyakui, oyaSymName, ref centerPoint, true);
                    }
                    else if (m_ConfigurationDirection == ConfigurationDirection.Back)
                    {
                        insertPoint = GetInsertPoint_Oyakui(doc, instOyakui, oyaSymName, ref centerPoint, false);
                    }
                    else
                    {
                        insertPoint = GetInsertPoint_Oyakui(doc, instOyakui, oyaSymName, ref centerPoint, true);
                    }

                    oyaSize = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou1(oyaSymName, 1)) / 2);
                    oyakuiIds = ClsOyakui.GetAllOyakuiList(doc);
                }
                else if (oyaSymName.Contains("鋼矢板"))
                {
                    insertPoint = GetInsertPoint_Kouyaita(doc, instOyakui, oyaSymName, ref centerPoint);
                    oyaSize = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(Master.ClsKouyaitaCsv.GetHeight(oyaSymName)));
                    oyakuiIds = ClsKouyaita.GetAllKouyaitaList(doc);
                }

                // 判定用の頭ツナギ材の線分を作成する
                XYZ vecAtamaStoE = XYZ.Zero;
                XYZ ptAtamaOriginEnd = XYZ.Zero;
                XYZ ptAtamaOriginStr = XYZ.Zero;
                if (centerPoint.DistanceTo(ptAtamaSt) > centerPoint.DistanceTo(ptAtamaEd))
                {
                    vecAtamaStoE = (ptAtamaSt - ptAtamaEd).Normalize();
                    ptAtamaOriginEnd = new XYZ(ptAtamaSt.X, ptAtamaSt.Y, 0);
                    ptAtamaOriginStr = new XYZ(ptAtamaEd.X, ptAtamaEd.Y, 0);
                }
                else
                {
                    vecAtamaStoE = (ptAtamaEd - ptAtamaSt).Normalize();
                    ptAtamaOriginEnd = new XYZ(ptAtamaEd.X, ptAtamaEd.Y, 0);
                    ptAtamaOriginStr = new XYZ(ptAtamaSt.X, ptAtamaSt.Y, 0);
                }

                double atamaTotalLength = ptAtamaSt.DistanceTo(ptAtamaEd);
                double atamaPileSpacingLength = double.NaN;
                atamaPileSpacingLength = atamaTotalLength - (ClsRevitUtil.CovertToAPI(m_TsukidashiSt) + ClsRevitUtil.CovertToAPI(m_TsukidashiEd) + (oyaSize * 2));

                XYZ ptLineSt = new XYZ(insertPoint.X, insertPoint.Y, 0);
                XYZ ptLineEd = ptLineSt + (atamaPileSpacingLength * vecAtamaStoE);
                ptLineSt = ptLineSt + (oyaSize * -vecAtamaStoE);
                ptLineEd = ptLineEd + (oyaSize * vecAtamaStoE);

                XYZ ptLineEdTemp = GetClosestPointOnSegment(ptLineSt, ptLineEd, ptAtamaOriginEnd);
                XYZ ptLineStTemp = GetClosestPointOnSegment(ptLineSt, ptLineEd, ptAtamaOriginStr);
                ptLineSt = ptLineStTemp;
                ptLineEd = ptLineEdTemp;

                Line lineAtama = Line.CreateBound(ptLineSt, ptLineEd);
                //CreateDebugLine(doc, centerPoint, lineAtama);

                // 頭ツナギ材と接している親杭を取得して、全てに取付補助材(上部パーツ)を配置する
                foreach (var id in oyakuiIds)
                {
                    FamilyInstance instTrgOyakui = doc.GetElement(id) as FamilyInstance;
                    string trgName = instTrgOyakui.Symbol.FamilyName;

                    XYZ trgInsertPoint = XYZ.Zero;
                    XYZ trgCenterPoint = XYZ.Zero;
                    if (trgName.Contains("杭"))
                    {
                        trgInsertPoint = GetInsertPoint_Oyakui(doc, instTrgOyakui, trgName, ref trgCenterPoint);
                    }
                    else if (trgName.Contains("鋼矢板"))
                    {
                        trgInsertPoint = GetInsertPoint_Kouyaita(doc, instTrgOyakui, trgName, ref trgCenterPoint);
                    }

                    // 線分の上に中点が存在しているのか
                    if (IsPointOnLine(trgInsertPoint, lineAtama))
                    {
                        using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                        {
                            t.Start();

                            ElementId levelID = ClsRevitUtil.GetParameterElementId(doc, id, "集計レベル");

                            ElementId createdId = ClsRevitUtil.Create(doc, trgInsertPoint, levelID, symUp);
                            FamilyInstance instUpParts = doc.GetElement(createdId) as FamilyInstance;

                            // 部材を杭の向きに合わせて回転
                            double angle = GetRotateAngle(trgCenterPoint, trgInsertPoint, instUpParts);
                            ElementTransformUtils.RotateElement(doc, createdId, Line.CreateBound(trgInsertPoint, trgInsertPoint + XYZ.BasisZ), angle + danmenAngle);

                            if (!vertical)
                                ClsRevitUtil.SetParameter(doc, createdId, "ホストからのオフセット", atamaOffset);
                            else
                                ClsRevitUtil.SetParameter(doc, createdId, "ホストからのオフセット", atamaOffset + atamaHeight);

                            //断面の場合は移動
                            if (isSide)
                                ClsRevitUtil.MoveFamilyInstance(instUpParts, atamaSize / 2, oyaDir);

                            t.Commit();
                        }
                    }
                }
            }

            // ブルマンとリキマンの作成
            if (m_JointType == JointType.BurumanC || m_JointType == JointType.RikimanG)
            {
                string jointPartsPath = string.Empty;
                if (m_JointType == JointType.BurumanC)
                {
                    jointPartsPath = Master.ClsBurumanCSV.GetFamilyPath("C-50");
                }
                else
                {
                    jointPartsPath = Master.ClsRikimanCSV.GetFamilyPath("G");
                }
                string familyName = RevitUtil.ClsRevitUtil.GetFamilyName(jointPartsPath);
                if (!ClsRevitUtil.LoadFamilyData(doc, jointPartsPath, out Family fam))
                {
                    //return false; ;
                }
                FamilySymbol sym = (ClsRevitUtil.GetFamilySymbol(doc, familyName, "頭ﾂﾅｷﾞ"));
                if (sym == null)
                {
                    sym = (ClsRevitUtil.GetFamilySymbol(doc, familyName, "頭ツナギ"));
                }
                if (sym == null)
                {
                    return false;
                }

                string oyaSymName = instOyakui.Symbol.FamilyName;

                // 挿入点を算出
                XYZ insertPoint = XYZ.Zero;
                XYZ centerPoint = XYZ.Zero;
                double oyaSize = double.NaN;
                List<ElementId> oyakuiIds = new List<ElementId>();

                if (oyaSymName.Contains("杭"))
                {
                    if (m_ConfigurationDirection == ConfigurationDirection.Front)
                    {
                        insertPoint = GetInsertPoint_Oyakui(doc, instOyakui, oyaSymName, ref centerPoint, true);
                    }
                    else if (m_ConfigurationDirection == ConfigurationDirection.Back)
                    {
                        insertPoint = GetInsertPoint_Oyakui(doc, instOyakui, oyaSymName, ref centerPoint, false);
                    }
                    else
                    {
                        insertPoint = GetInsertPoint_Oyakui(doc, instOyakui, oyaSymName, ref centerPoint, true);
                    }

                    oyaSize = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou1(oyaSymName, 1)) / 2);
                    oyakuiIds = ClsOyakui.GetAllOyakuiList(doc);
                }
                else if (oyaSymName.Contains("鋼矢板"))
                {
                    insertPoint = GetInsertPoint_Kouyaita(doc, instOyakui, oyaSymName, ref centerPoint);
                    oyaSize = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(Master.ClsKouyaitaCsv.GetHeight(oyaSymName)));
                    oyakuiIds = ClsKouyaita.GetAllKouyaitaList(doc);
                }


                // 判定用の頭ツナギ材の線分を作成する
                XYZ vecAtamaStoE = XYZ.Zero;
                XYZ ptAtamaOriginEnd = XYZ.Zero;
                XYZ ptAtamaOriginStr = XYZ.Zero;
                if (centerPoint.DistanceTo(ptAtamaSt) > centerPoint.DistanceTo(ptAtamaEd))
                {
                    vecAtamaStoE = (ptAtamaSt - ptAtamaEd).Normalize();
                    ptAtamaOriginEnd = new XYZ(ptAtamaSt.X, ptAtamaSt.Y, 0);
                    ptAtamaOriginStr = new XYZ(ptAtamaEd.X, ptAtamaEd.Y, 0);
                }
                else
                {
                    vecAtamaStoE = (ptAtamaEd - ptAtamaSt).Normalize();
                    ptAtamaOriginEnd = new XYZ(ptAtamaEd.X, ptAtamaEd.Y, 0);
                    ptAtamaOriginStr = new XYZ(ptAtamaSt.X, ptAtamaSt.Y, 0);
                }

                double atamaTotalLength = ptAtamaSt.DistanceTo(ptAtamaEd);
                double atamaPileSpacingLength = double.NaN;
                atamaPileSpacingLength = atamaTotalLength - (ClsRevitUtil.CovertToAPI(m_TsukidashiSt) + ClsRevitUtil.CovertToAPI(m_TsukidashiEd) + (oyaSize * 2));

                XYZ ptLineSt = new XYZ(insertPoint.X, insertPoint.Y, 0);
                XYZ ptLineEd = ptLineSt + (atamaPileSpacingLength * vecAtamaStoE);
                ptLineSt = ptLineSt + (oyaSize * -vecAtamaStoE);
                ptLineEd = ptLineEd + (oyaSize * vecAtamaStoE);
                XYZ ptLineEdTemp = GetClosestPointOnSegment(ptLineSt, ptLineEd, ptAtamaOriginEnd);
                XYZ ptLineStTemp = GetClosestPointOnSegment(ptLineSt, ptLineEd, ptAtamaOriginStr);
                ptLineSt = ptLineStTemp;
                ptLineEd = ptLineEdTemp;

                Line lineAtama = Line.CreateBound(ptLineSt, ptLineEd);
                //CreateDebugLine(doc, centerPoint, lineAtama);

                // 頭ツナギ材と接している親杭を取得して、全てに取付補助材(上部パーツ)を配置する
                foreach (var id in oyakuiIds)
                {
                    FamilyInstance instTrgOyakui = doc.GetElement(id) as FamilyInstance;
                    string trgName = instTrgOyakui.Symbol.FamilyName;

                    XYZ trgInsertPoint = XYZ.Zero;
                    XYZ trgCenterPoint = XYZ.Zero;
                    if (trgName.Contains("杭"))
                    {
                        // 親杭はウェブと干渉するので少しずらす
                        trgInsertPoint = GetInsertPoint_Oyakui(doc, instTrgOyakui, trgName, ref trgCenterPoint);
                        trgInsertPoint = trgInsertPoint + ((oyaSize / 2) * vecAtamaStoE);
                    }
                    else if (trgName.Contains("鋼矢板"))
                    {
                        // 鋼矢板は板の中央に配置
                        trgInsertPoint = GetInsertPoint_Kouyaita(doc, instTrgOyakui, trgName, ref trgCenterPoint);
                    }

                    // 線分の上に中点が存在しているのか
                    if (IsPointOnLine(trgInsertPoint, lineAtama))
                    {
                        using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                        {
                            t.Start();

                            ElementId levelID = ClsRevitUtil.GetParameterElementId(doc, id, "集計レベル");

                            ElementId createdId = ClsRevitUtil.Create(doc, trgInsertPoint, levelID, sym);
                            FamilyInstance instBuruman = doc.GetElement(createdId) as FamilyInstance;

                            // 部材を杭の向きに合わせて回転
                            double angle = GetRotateAngle(ptLineSt, ptLineEd, instBuruman);
                            ElementTransformUtils.RotateElement(doc, createdId, Line.CreateBound(trgInsertPoint, trgInsertPoint + XYZ.BasisZ), angle);

                            double offset = ClsRevitUtil.GetParameterDouble(doc, id, "ホストからのオフセット");
                            ClsRevitUtil.SetParameter(doc, createdId, "ホストからのオフセット", offset);

                            t.Commit();
                        }
                    }
                }

            }

            return true;
        }

        public XYZ GetClosestPointOnSegment(XYZ p1, XYZ p2, XYZ targetPoint)
        {
            XYZ lineDir = (p2 - p1).Normalize();
            double length = p1.DistanceTo(p2);
            XYZ v = targetPoint - p1;

            double dot = v.DotProduct(lineDir);
            dot = Math.Max(0, Math.Min(dot, length)); // 線分の範囲に制限

            return p1 + dot * lineDir;
        }

        public void DrawCrossMark(Document doc, XYZ center, double size = 1.0)
        {
            using (Transaction tx = new Transaction(doc, "Draw Cross Mark"))
            {
                tx.Start();

                // SketchPlane の作成（XY 平面）
                Plane plane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, center);
                SketchPlane sketchPlane = SketchPlane.Create(doc, plane);

                // 斜め線のベクトル（×の線）
                XYZ offset1 = new XYZ(size / 2, size / 2, 0);
                XYZ offset2 = new XYZ(size / 2, -size / 2, 0);

                // 2本の交差する線
                Line line1 = Line.CreateBound(center - offset1, center + offset1);
                Line line2 = Line.CreateBound(center - offset2, center + offset2);

                // モデル線（ModelCurve）として描画
                doc.Create.NewModelCurve(line1, sketchPlane);
                doc.Create.NewModelCurve(line2, sketchPlane);

                tx.Commit();
            }
        }
        private void SwapStartAndEnd(FamilyInstance instOyaSt, ref XYZ ptOyaSt, ref XYZ ptOyaEd)
        {
            if (instOyaSt.FacingOrientation.Y > 0)
            {
                if (ClsGeo.IsLeft(ptOyaSt, ptOyaEd))
                {
                    XYZ temp = ptOyaSt;
                    ptOyaSt = ptOyaEd;
                    ptOyaEd = temp;

                    double temp2 = m_TsukidashiSt;
                    m_TsukidashiSt = m_TsukidashiEd;
                    m_TsukidashiEd = temp2;
                }
            }
            else if (instOyaSt.FacingOrientation.Y < 0)
            {
                if (ClsGeo.IsLeft(ptOyaEd, ptOyaSt))
                {
                    XYZ temp = ptOyaSt;
                    ptOyaSt = ptOyaEd;
                    ptOyaEd = temp;

                    double temp2 = m_TsukidashiSt;
                    m_TsukidashiSt = m_TsukidashiEd;
                    m_TsukidashiEd = temp2;
                }
            }

            if (instOyaSt.FacingOrientation.X > 0)
            {
                if (ClsGeo.IsLeft(ptOyaSt, ptOyaEd))
                {
                    XYZ temp = ptOyaSt;
                    ptOyaSt = ptOyaEd;
                    ptOyaEd = temp;

                    double temp2 = m_TsukidashiSt;
                    m_TsukidashiSt = m_TsukidashiEd;
                    m_TsukidashiEd = temp2;
                }
            }
            else if (instOyaSt.FacingOrientation.X < 0)
            {
                if (ClsGeo.IsLeft(ptOyaEd, ptOyaSt))
                {
                    XYZ temp = ptOyaSt;
                    ptOyaSt = ptOyaEd;
                    ptOyaEd = temp;

                    double temp2 = m_TsukidashiSt;
                    m_TsukidashiSt = m_TsukidashiEd;
                    m_TsukidashiEd = temp2;
                }
            }
        }

        private static double GetRotateAngle(XYZ pOyakui, XYZ insertPointSt, FamilyInstance instA)
        {
            // 基準点と挿入点
            XYZ basePoint = pOyakui; // 基準点の座標

            // 基準点から挿入点へのベクトルを取得
            XYZ direction = basePoint - insertPointSt;

            // ベクトルをZ軸に対応する方向に変換（プロジェクト平面への変換）
            XYZ projectedDirection = new XYZ(direction.X, direction.Y, 0).Normalize();

            // デフォルトの向きを取得
            XYZ defaultFacing = instA.FacingOrientation;

            // Z軸と計算した方向ベクトルのなす角を求める
            double angle = defaultFacing.AngleTo(projectedDirection);
            if (!ClsGeo.IsLeft(defaultFacing, direction))
            {
                angle = -angle;
            }

            return angle;
        }

        /// <summary>
        /// 頭ツナギ材を親杭の基点までずらした線分を取得
        /// </summary>
        /// <param name="ptAtamaSt"></param>
        /// <param name="ptAtamaEd"></param>
        /// <param name="ptOyakui"></param>
        /// <param name="oyaSize"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private Line GetLineAtama(XYZ ptAtamaSt, XYZ ptAtamaEd, XYZ ptOyakui, double oyaSize, double size)
        {
            XYZ vecAtamaStoE = XYZ.Zero;
            if (ptOyakui.DistanceTo(ptAtamaSt) > ptOyakui.DistanceTo(ptAtamaEd))
            {
                vecAtamaStoE = (ptAtamaSt - ptAtamaEd).Normalize();
            }
            else
            {
                vecAtamaStoE = (ptAtamaEd - ptAtamaSt).Normalize();
            }

            double atamaTotalLength = ptAtamaSt.DistanceTo(ptAtamaEd);
            double atamaPileSpacingLength = atamaTotalLength - (ClsRevitUtil.CovertToAPI(m_TsukidashiSt) + ClsRevitUtil.CovertToAPI(m_TsukidashiEd) + (oyaSize * 2));

            ptOyakui = new XYZ(ptOyakui.X, ptOyakui.Y, 0);
            XYZ ptLineEd = ptOyakui + (atamaPileSpacingLength * vecAtamaStoE);
            Line lineAtama = Line.CreateBound(ptOyakui, ptLineEd);

            return lineAtama;
        }

        /// <summary>
        /// 親杭の部材の取り付く位置を取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="instOyakui"></param>
        /// <param name="oyakuiSize"></param>
        /// <returns></returns>
        private static XYZ GetInsertPoint_Oyakui(Document doc, FamilyInstance instOyakui, string oyakuiSize, ref XYZ centerPoint, bool isFront = true)
        {
            XYZ ptOyakui = instOyakui.GetTransform().Origin;
            XYZ vecOyakui = instOyakui.FacingOrientation;

            centerPoint = ptOyakui;

            double oyaHeight = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou1(oyakuiSize, 1)));
            XYZ ptOyakuiInsert = XYZ.Zero;
            if (isFront)
            {
                ptOyakuiInsert = ptOyakui + (vecOyakui * (oyaHeight / 2));
            }
            else
            {
                ptOyakuiInsert = ptOyakui - (vecOyakui * (oyaHeight / 2));
            }

            return ptOyakuiInsert;
        }

        //private static XYZ GetInsertPoint_Kouyaita(Document doc, FamilyInstance instKouyaita, string kouyaitaSize, ref XYZ centerPoint)
        //{
        //    // 鋼矢板の挿入点は中央ではなく端部

        //    XYZ ptKouyaita = instKouyaita.GetTransform().Origin;
        //    XYZ vecKouyaita = instKouyaita.FacingOrientation;
        //    XYZ vecOffset = XYZ.Zero;

        //    // 鋼矢板の向きを判定
        //    double tolerance = 0.0001; // 適切な許容範囲の値を設定してください
        //    int voidVec = ClsKouyaita.GetVoidvec(doc, instKouyaita.Id);
        //    if (voidVec == 1)
        //    {
        //        // 壁側方向 (凸)
        //        if (Math.Abs(vecKouyaita.X - 1) < tolerance && Math.Abs(vecKouyaita.Y) < tolerance && Math.Abs(vecKouyaita.Z) < tolerance)
        //        {
        //            vecOffset = new XYZ(0, 1, 0);
        //        }
        //        else if (Math.Abs(vecKouyaita.X) < tolerance && Math.Abs(vecKouyaita.Y - 1) < tolerance && Math.Abs(vecKouyaita.Z) < tolerance)
        //        {
        //            vecOffset = new XYZ(1, 0, 0);
        //        }
        //    }
        //    else
        //    {
        //        // 穴側方向 (凹)
        //        if (Math.Abs(vecKouyaita.X + 1) < tolerance && Math.Abs(vecKouyaita.Y) < tolerance && Math.Abs(vecKouyaita.Z) < tolerance)
        //        {
        //            vecOffset = new XYZ(0, -1, 0);
        //        }
        //        else if (Math.Abs(vecKouyaita.X) < tolerance && Math.Abs(vecKouyaita.Y + 1) < tolerance && Math.Abs(vecKouyaita.Z) < tolerance)
        //        {
        //            vecOffset = new XYZ(-1, 0, 0);
        //        }
        //    }

        //    // 鋼矢板は挿入点が端部のため、中央に補正する
        //    double oyaWidth = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(Master.ClsKouyaitaCsv.GetWidth(kouyaitaSize)));
        //    XYZ ptKouyaitaCenter = ptKouyaita + (vecOffset * (oyaWidth / 2));
        //    centerPoint = ptKouyaitaCenter;

        //    double oyaHeight = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(Master.ClsKouyaitaCsv.GetHeight(kouyaitaSize)));
        //    XYZ ptKouyaitaInsert = ptKouyaitaCenter + (vecKouyaita * oyaHeight);

        //    return ptKouyaitaInsert;
        //}

        private static XYZ GetInsertPoint_Kouyaita(Document doc, FamilyInstance instKouyaita, string kouyaitaSize, ref XYZ centerPoint)
        {
            XYZ ptKouyaita = instKouyaita.GetTransform().Origin;
            XYZ vecKouyaita = instKouyaita.FacingOrientation;
            XYZ vecAtama = XYZ.Zero;

            XYZ forward = vecKouyaita;
            XYZ up = -XYZ.BasisZ; // 通常の上方向
            vecAtama = up.CrossProduct(forward).Normalize();

            // 鋼矢板は挿入点が端部のため、中央に補正する
            double oyaWidth = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(Master.ClsKouyaitaCsv.GetWidth(kouyaitaSize)));
            XYZ ptKouyaitaCenter = ptKouyaita + (vecAtama * (oyaWidth / 2));

            XYZ ptKouyaitaInsert = ptKouyaitaCenter;
            double oyaHeight = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(Master.ClsKouyaitaCsv.GetHeight(kouyaitaSize)));
            ptKouyaitaInsert = ptKouyaitaCenter + (vecKouyaita * oyaHeight);

            centerPoint = ptKouyaitaCenter;
            return ptKouyaitaInsert;
        }

        /// <summary>
        /// 鋼矢板の部材の取り付く位置を取得する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="instKouyaita"></param>
        /// <param name="ptSt"></param>
        /// <param name="ptEd"></param>
        /// <param name="kouyaitaSize"></param>
        /// <returns></returns>
        private static XYZ GetInsertPoint_Kouyaita(Document doc, FamilyInstance instKouyaita, XYZ ptSt, XYZ ptEd, string kouyaitaSize)
        {
            XYZ ptKouyaita = instKouyaita.GetTransform().Origin;
            XYZ vecKouyaita = instKouyaita.FacingOrientation;
            XYZ vecAtama = XYZ.Zero;

            // 鋼矢板の向きを判定
            if (ClsKouyaita.GetVoidvec(doc, instKouyaita.Id) == 1)
            {
                // 壁側方向 (凸)
                vecAtama = (ptEd - ptSt).Normalize();
            }
            else
            {
                // 穴側方向 (凹)
                vecAtama = (ptSt - ptEd).Normalize();
            }

            // 鋼矢板は挿入点が端部のため、中央に補正する
            double oyaWidth = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(Master.ClsKouyaitaCsv.GetWidth(kouyaitaSize)));
            XYZ ptKouyaitaCenter = ptKouyaita + (vecAtama * (oyaWidth / 2));

            double oyaHeight = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(Master.ClsKouyaitaCsv.GetHeight(kouyaitaSize)));
            XYZ ptKouyaitaInsert = ptKouyaitaCenter + (vecKouyaita * oyaHeight);

            return ptKouyaitaInsert;
        }

        private static XYZ GetInsertPoint_Kouyaita(Document doc, FamilyInstance instKouyaita, string kouyaitaSize,bool bHeight)
        {
            XYZ ptKouyaita = instKouyaita.GetTransform().Origin;
            XYZ vecKouyaita = instKouyaita.FacingOrientation;
            XYZ vecAtama = XYZ.Zero;

            XYZ forward = vecKouyaita;
            XYZ up = -XYZ.BasisZ; // 通常の上方向
            vecAtama = up.CrossProduct(forward).Normalize();

            // 鋼矢板は挿入点が端部のため、中央に補正する
            double oyaWidth = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(Master.ClsKouyaitaCsv.GetWidth(kouyaitaSize)));
            XYZ ptKouyaitaCenter = ptKouyaita + (vecAtama * (oyaWidth / 2));

            XYZ ptKouyaitaInsert = ptKouyaitaCenter;
            if (bHeight)
            {
                double oyaHeight = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(Master.ClsKouyaitaCsv.GetHeight(kouyaitaSize)));
                ptKouyaitaInsert = ptKouyaitaCenter + (vecKouyaita * oyaHeight);
            }

            return ptKouyaitaInsert;
        }

        private static bool IsPointOnLine(XYZ point, Line line)
        {
            XYZ startPoint = new XYZ(line.GetEndPoint(0).X, line.GetEndPoint(0).Y, point.Z);
            XYZ endPoint = new XYZ(line.GetEndPoint(1).X, line.GetEndPoint(1).Y, point.Z);

            // 線分の方向ベクトル
            XYZ lineDirection = endPoint - startPoint;

            // ポイントから線分の始点までのベクトル
            XYZ pointToStart = startPoint - point;

            // ポイントから線分の終点までのベクトル
            XYZ pointToEnd = endPoint - point;

            // ポイントが線分の始点と終点の間にあるかどうかを確認
            bool isBetween = lineDirection.DotProduct(pointToStart) * lineDirection.DotProduct(pointToEnd) < 0;

            if (!isBetween)
            {
                return false;
            }

            // ポイントから線分への最短距離を計算
            double distanceToLine = Math.Abs(startPoint.Subtract(point).CrossProduct(lineDirection).GetLength()) / lineDirection.GetLength();

            // 許容誤差内であればポイントは線分上にあると見なす
            double tolerance = 0.0001; // 適切な許容範囲の値を設定してください

            return Math.Abs(distanceToLine) < tolerance;
        }

        private void GetCustomData(Document doc, ElementId id)
        {
            string tmp = string.Empty;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "鋼材タイプ", out tmp);
            m_KouzaiType = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "鋼材サイズ", out tmp);
            m_KouzaiSize = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "Cの向き", out tmp);
            switch (tmp)
            {
                case "DirectionVertical":
                    m_ChannelDirection = ClsAtamaTsunagi.ChannelDirection.DirectionVertical;
                    break;
                case "DirectionHorizontal":
                    m_ChannelDirection = ClsAtamaTsunagi.ChannelDirection.DirectionHorizontal;
                    break;
                default:
                    m_ChannelDirection = ClsAtamaTsunagi.ChannelDirection.None;
                    break;
            }

            ClsRevitUtil.CustomDataGet<string>(doc, id, "取付方法", out tmp);
            switch (tmp)
            {
                case "Welding":
                    m_JointType = JointType.Welding;
                    break;
                case "Bolt":
                    m_JointType = JointType.Bolt;
                    break;
                case "BurumanC":
                    m_JointType = JointType.BurumanC;
                    break;
                case "RikimanG":
                    m_JointType = JointType.RikimanG;
                    break;
                default:
                    break;
            }

            ClsRevitUtil.CustomDataGet<string>(doc, id, "ボルトタイプ", out tmp);
            m_BoltType = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "ボルトサイズ", out tmp);
            m_BoltSize = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "配置方向", out tmp);
            switch (tmp)
            {
                case "None":
                    m_ConfigurationDirection = ConfigurationDirection.None;
                    break;
                case "Front":
                    m_ConfigurationDirection = ConfigurationDirection.Front;
                    break;
                case "Back":
                    m_ConfigurationDirection = ConfigurationDirection.Back;
                    break;
                default:
                    break;
            }

            ClsRevitUtil.CustomDataGet<string>(doc, id, "作成方法", out tmp);
            switch (tmp)
            {
                case "PileTop":
                    m_CreateType = CreateType.PileTop;
                    break;
                case "FromToP":
                    m_CreateType = CreateType.FromToP;
                    break;
                default:
                    break;
            }

            ClsRevitUtil.CustomDataGet<string>(doc, id, "高さ", out tmp);
            m_HeightfromToP = double.Parse(tmp);

            ClsRevitUtil.CustomDataGet<string>(doc, id, "取付補助材", out tmp);
            m_ToritsukeHojozai = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "ブラケット", out tmp);
            m_Bracket = tmp;

            ClsRevitUtil.CustomDataGet<string>(doc, id, "突出量S", out tmp);
            m_TsukidashiSt = double.Parse(tmp);

            ClsRevitUtil.CustomDataGet<string>(doc, id, "突出量E", out tmp);
            m_TsukidashiEd = double.Parse(tmp);
        }

        private void SetCustomData(Document doc, ElementId id)
        {
            ClsRevitUtil.CustomDataSet<string>(doc, id, "鋼材タイプ", m_KouzaiType);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "鋼材サイズ", m_KouzaiSize);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "Cの向き", m_ChannelDirection.ToString());
            ClsRevitUtil.CustomDataSet<string>(doc, id, "取付方法", m_JointType.ToString());
            ClsRevitUtil.CustomDataSet<string>(doc, id, "ボルトタイプ", m_BoltType);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "ボルトサイズ", m_BoltSize);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "配置方向", m_ConfigurationDirection.ToString());
            ClsRevitUtil.CustomDataSet<string>(doc, id, "作成方法", m_CreateType.ToString());
            ClsRevitUtil.CustomDataSet<string>(doc, id, "高さ", m_HeightfromToP.ToString());
            ClsRevitUtil.CustomDataSet<string>(doc, id, "取付補助材", m_ToritsukeHojozai.ToString());
            ClsRevitUtil.CustomDataSet<string>(doc, id, "ブラケット", m_Bracket);
            ClsRevitUtil.CustomDataSet<string>(doc, id, "突出量S", m_TsukidashiSt.ToString());
            ClsRevitUtil.CustomDataSet<string>(doc, id, "突出量E", m_TsukidashiEd.ToString());
        }

        public static bool PickObject(UIDocument uidoc, ref ElementId id, string message = "頭ツナギ材")
        {
            return ClsRevitUtil.PickObjectPartFilter(uidoc, message + "を選択してください", "頭ツナギ材", ref id);
        }

        /// <summary>
        /// タイプ名に「頭ツナギ材」が含まれるものを複数選択
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="ids"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool PickAtamatsunagizaiObjects(UIDocument uidoc, ref List<ElementId> ids, string message = "頭ツナギ材")
        {
            return ClsRevitUtil.PickObjectsPartFilter(uidoc, message + "を選択してください", "頭ツナギ材", ref ids);
        }

        /// <summary>
        /// タイプ名に「頭ツナギ」が含まれるものを複数選択
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="ids"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool PickAtamatsunagiObjects(UIDocument uidoc, ref List<ElementId> ids, string message = "頭ツナギ")
        {
            return ClsRevitUtil.PickObjectsPartFilter(uidoc, message + "を選択してください", "頭ツナギ", ref ids);
        }

        /// <summary>
        /// 親杭、鋼矢板 のみを単独選択
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="id">選択した 壁 のID</param>
        /// <param name="message">指示</param>
        /// <returns></returns>
        public static bool PickObjectKabe(UIDocument uidoc, ref ElementId id, string message = "壁")
        {
            List<string> filterList = new List<string>();
            filterList.Add(ClsKouyaita.KOUYAITA);
            filterList.Add(ClsKouyaita.CORNERYAITA);
            filterList.Add(ClsKouyaita.IKEIYAITA);
            filterList.Add(ClsOyakui.oyakui);
            return ClsRevitUtil.PickObjectPartListFilter(uidoc, message + "を選択してください", filterList, ref id);
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
            return ClsRevitUtil.PickObjectsPartListFilter(uidoc, message + "を選択してください", filterList, ref ids);
        }

        public static JointType GetJointType(string jointType)
        {
            switch(jointType)
            {
                case "溶接":
                    return JointType.Welding;
                case "ボルト":
                    return JointType.Bolt;
                case "ブルマン C-50":
                    return JointType.BurumanC;
                case "リキマンGタイプ":
                    return JointType.RikimanG;
                default:
                    return JointType.BurumanC;
            }
        }

        public static ChannelDirection GetChannelDirection(string channelDirection)
        {
            switch(channelDirection)
            {
                case "Vertical":
                    return ChannelDirection.DirectionVertical;
                case "Horizontal":
                    return ChannelDirection.DirectionHorizontal;
                default:
                    return ChannelDirection.None;
            }
        }

        public static ConfigurationDirection GetConfigurationDirection(string configurationDirection)
        {
            switch(configurationDirection)
            {
                case "全面(採掘側)":
                    return ConfigurationDirection.Front;
                case "背面(山留側)":
                    return ConfigurationDirection.Back;
                default:
                    return ConfigurationDirection.None;
            }
        }
        private static void CreateDebugLine(Document doc, XYZ pt, Curve line)
        {
            // デバッグ用の線分を表示
            using (Transaction transaction = new Transaction(doc, "Create Model Line"))
            {
                transaction.Start();

                // モデル線分を作成
                ElementId levelID = ClsRevitUtil.GetLevelID(doc, ClsKabeShin.GL);
                Plane plane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, pt);
                SketchPlane sketchPlane = SketchPlane.Create(doc, plane);
                ModelCurve modelLineF = doc.Create.NewModelCurve(line, sketchPlane);

                transaction.Commit();
            }
        }
    }
}
