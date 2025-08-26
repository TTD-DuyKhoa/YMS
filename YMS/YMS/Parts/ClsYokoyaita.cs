using Autodesk.Revit.DB;
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
    public class ClsYokoyaita
    {
        #region 定数
        const string yokoyaitaUnderbar = "横矢板_";
        #endregion
        #region プロパティ
        /// <summary>
        /// 横矢板：サイズ
        /// </summary>
        public string m_size { get; set; }
        /// <summary>
        /// 横矢板：タイプ
        /// </summary>
        public string m_type { get; set; }
        /// <summary>
        /// 横矢板：配置位置
        /// </summary>
        public int m_putPosFlag { get; set; }
        #endregion

        #region コンストラクタ
        public ClsYokoyaita()
        {
            //初期化
            Init();
        }
        #endregion

        #region メソッド

        public void Init()
        {
            m_size = string.Empty;
            m_type = string.Empty;
            m_putPosFlag = 0;
        }
        /// <summary>
        /// H鋼一点から水平に横矢板作図する
        /// </summary>
        /// <param name="doc">ドキュメント</param>
        /// <param name="oyaId1">H鋼</param>
        /// <param name="lenght">横矢板長さ</param>
        /// <param name="kussaku">掘削深さ</param>
        /// <returns></returns>
        public bool CreateYokoyaita(Document doc, ElementId oyaId1, double lenght, double kussaku, ElementId levelId)
        {
            try
            {
                using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                {
                    string symbolFolpath = ClsZumenInfo.GetYMSFolder();

                    string familyPath = Master.ClsYokoyaitaCSV.GetFamilyPath(m_size);
                    string familyName = ClsRevitUtil.GetFamilyName(familyPath);

                    //横矢板配置
                    if (!ClsRevitUtil.LoadFamilySymbolData(doc, familyPath, familyName, out FamilySymbol sym))
                    {
                        return false;
                    }

                    //配置する横矢板の高さは何処から取得がベストか
                    double yokoyaHigh = ClsRevitUtil.CovertToAPI(272.5);
                    //横矢板横幅
                    double yokoyaWidthHalf = ClsRevitUtil.CovertToAPI(36.0 / 2);

                    t.Start();
                    FailureHandlingOptions failOpt = t.GetFailureHandlingOptions();
                    failOpt.SetFailuresPreprocessor(new WarningSwallower());
                    t.SetFailureHandlingOptions(failOpt);

                    FamilyInstance oyaInst1 = doc.GetElement(oyaId1) as FamilyInstance;
                    XYZ oyaPnt1 = (oyaInst1.Location as LocationPoint).Point;
                    string oyaSymName = oyaInst1.Symbol.FamilyName;

                    double fullLenght = kussaku;

                    //高さ
                    double oyaSizeHalf = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou1(oyaSymName, 1)) / 2);//ClsRevitUtil.CovertToAPI(300 / 2);
                    //フランジ幅
                    double oyaBHalf = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou1(oyaSymName, 2)) / 2);//ClsRevitUtil.CovertToAPI(300 / 2);
                    //腹板厚半分※Point内ずらしと長さ変更
                    double oyaT1Half = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou1(oyaSymName, 3)) / 2);//ClsRevitUtil.CovertToAPI(10 / 2);
                    //フランジ厚
                    double oyaT2 = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou1(oyaSymName, 4)));//ClsRevitUtil.CovertToAPI(15);

                    double oyaSizeHalfIn = oyaSizeHalf - oyaT2 - yokoyaWidthHalf;
                    double oyaSizeHalfOut = oyaSizeHalf + yokoyaWidthHalf;

                    oyaPnt1 = new XYZ(oyaPnt1.X + (oyaBHalf / 2 + oyaT1Half) * oyaInst1.HandOrientation.X,
                                          oyaPnt1.Y + (oyaBHalf / 2 + oyaT1Half) * oyaInst1.HandOrientation.Y,
                                          oyaPnt1.Z);
                    double yokoyaLenght = ClsRevitUtil.CovertToAPI(lenght) - oyaBHalf;// oyaT1Half * 2;

                    //ClsRevitUtil.SetTypeParameter(sym, ClsGlobal.m_length, yokoyaLenght);
                    

                    if (m_putPosFlag == 0)
                    {
                        //内にずらす
                        oyaPnt1 = new XYZ(oyaPnt1.X + oyaSizeHalfIn * oyaInst1.FacingOrientation.X,
                                          oyaPnt1.Y + oyaSizeHalfIn * oyaInst1.FacingOrientation.Y,
                                          oyaPnt1.Z);
                    }
                    else if (m_putPosFlag == 1)
                    {
                        //外にずらす
                        oyaPnt1 = new XYZ(oyaPnt1.X - oyaSizeHalfOut * oyaInst1.FacingOrientation.X,
                                          oyaPnt1.Y - oyaSizeHalfOut * oyaInst1.FacingOrientation.Y,
                                          oyaPnt1.Z);
                    }

                    Line axis = Line.CreateBound(oyaPnt1, oyaPnt1 + XYZ.BasisZ);
                    //杭の上から作図していき積み下げていく
                    for (int i = 0; i < fullLenght / yokoyaHigh; i++)
                    {
                        ElementId CreatedID = ClsRevitUtil.Create(doc, oyaPnt1, levelId, sym);
                        ClsRevitUtil.SetParameter(doc, CreatedID, ClsGlobal.m_refLvTop, -yokoyaHigh * i);
                        //sym = ClsRevitUtil.ChangeTypeID(doc, sym, CreatedID, ClsRevitUtil.CreateTypeName(familyName, yokoyaLenght));
                        //回転
                        FamilyInstance yokoInst1 = doc.GetElement(CreatedID) as FamilyInstance;
                        double angle = oyaInst1.HandOrientation.AngleTo(yokoInst1.HandOrientation);

                        if(!ClsGeo.IsLeft(yokoInst1.HandOrientation, oyaInst1.HandOrientation))
                        {
                            angle = -angle;
                        }
                        ClsRevitUtil.RotateElement(doc, CreatedID, axis, angle); 
                    }
                    ClsRevitUtil.SetTypeParameter(sym, ClsGlobal.m_length, yokoyaLenght);

                    //変更が加わるメッセージ原因不明
                    t.Commit();
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Info Message23", ex.Message);
                string message = ex.Message;
                MessageBox.Show(message);
            }

            return true;
        }
        /// <summary>
        /// H鋼一点から任意の角度に横矢板を作図する
        /// </summary>
        /// <param name="doc">ドキュメント</param>
        /// <param name="oyaId1">H鋼</param>
        /// <param name="lenght">横矢板長さ</param>
        /// <param name="kussaku">掘削深さ</param>
        /// <param name="oyaPnt2">H鋼2の位置</param>
        /// <returns></returns>
        public bool CreateYokoyaita(Document doc, ElementId oyaId1, ElementId oyaId2, double lenght, double kussaku, ElementId levelId, double oyaKijyun = 0.0)
        {
            try
            {
                string symbolFolpath = ClsZumenInfo.GetYMSFolder();

                string familyPath = Master.ClsYokoyaitaCSV.GetFamilyPath(m_size);
                string familyName = ClsRevitUtil.GetFamilyName(familyPath);

                //横矢板配置
                if (!ClsRevitUtil.LoadFamilySymbolData(doc, familyPath, familyName, out FamilySymbol sym))
                {
                    return false;
                }

                //作成する親杭同士の位置判定
                (oyaId1, oyaId2) = CheckOyakuiIsRight(doc, oyaId1, oyaId2);

                //配置する横矢板の高さは何処から取得がベストか
                double yokoyaHigh = ClsRevitUtil.CovertToAPI(Master.ClsYokoyaitaCSV.GetHight(m_size));
                //横矢板横幅
                double yokoyaWidthHalf = ClsRevitUtil.CovertToAPI(Master.ClsYokoyaitaCSV.GetWidth(m_size) / 2);

                FamilyInstance oyaInst1 = doc.GetElement(oyaId1) as FamilyInstance;
                XYZ oyaPnt1 = (oyaInst1.Location as LocationPoint).Point;
                string oyaSymName1 = oyaInst1.Symbol.FamilyName;
                FamilyInstance oyaInst2 = doc.GetElement(oyaId2) as FamilyInstance;
                XYZ oyaPnt2 = (oyaInst2.Location as LocationPoint).Point;
                string oyaSymName2 = oyaInst1.Symbol.FamilyName;

                double fullLenght = kussaku;

                //ﾌﾗﾝｼﾞにかかる横矢板のかかりしろ
                double settingKakarishiro = GetKakarishiroLength();//#31504
                double kakarishiro = ClsRevitUtil.CovertToAPI(settingKakarishiro);

                //高さ
                double oyaSizeHalf = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou1(oyaSymName1, 1)) / 2);//ClsRevitUtil.CovertToAPI(300 / 2);
                                                                                                                                                  //フランジ幅
                double oyaBHalf = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou1(oyaSymName1, 2)) / 2);//ClsRevitUtil.CovertToAPI(300 / 2);
                                                                                                                                               //腹板厚半分※Point内ずらしと長さ変更
                double oyaT1Half = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou1(oyaSymName1, 3)) / 2);//ClsRevitUtil.CovertToAPI(10 / 2);
                                                                                                                                                //フランジ厚
                double oyaT2 = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou1(oyaSymName1, 4)));//ClsRevitUtil.CovertToAPI(15);

                //高さ2
                double oya2SizeHalf = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou1(oyaSymName2, 1)) / 2);//ClsRevitUtil.CovertToAPI(300 / 2);
                                                                                                                                                   //フランジ幅2
                double oya2BHalf = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou1(oyaSymName2, 2)) / 2);//ClsRevitUtil.CovertToAPI(300 / 2);
                                                                                                                                                //腹板厚半分2
                double oya2T1Half = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou1(oyaSymName2, 3)) / 2);//ClsRevitUtil.CovertToAPI(10 / 2);
                                                                                                                                                 //フランジ厚2
                double oya2T2 = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou1(oyaSymName2, 4)));//ClsRevitUtil.CovertToAPI(15);


                double oyaSizeHalfIn = oyaSizeHalf - oyaT2 - yokoyaWidthHalf;
                double oyaSizeHalfOut = oyaSizeHalf + yokoyaWidthHalf;

                double oya2SizeHalfIn = oya2SizeHalf - oya2T2 - yokoyaWidthHalf;
                double oya2SizeHalfOut = oya2SizeHalf + yokoyaWidthHalf;

                XYZ sPnt1 = new XYZ(oyaPnt1.X + (oya2T1Half + yokoyaWidthHalf) * oyaInst1.HandOrientation.X,
                                      oyaPnt1.Y + (oya2T1Half + yokoyaWidthHalf) * oyaInst1.HandOrientation.Y,
                                      oyaPnt1.Z);
                XYZ sPnt2 = new XYZ(oyaPnt2.X - (oya2T1Half + yokoyaWidthHalf) * oyaInst2.HandOrientation.X,
                                      oyaPnt2.Y - (oya2T1Half + yokoyaWidthHalf) * oyaInst2.HandOrientation.Y,
                                      oyaPnt2.Z);

                oyaPnt1 = new XYZ(oyaPnt1.X + (oyaBHalf - kakarishiro) * oyaInst1.HandOrientation.X,
                                      oyaPnt1.Y + (oyaBHalf - kakarishiro) * oyaInst1.HandOrientation.Y,
                                      oyaPnt1.Z);
                oyaPnt2 = new XYZ(oyaPnt2.X - (oya2BHalf - kakarishiro) * oyaInst2.HandOrientation.X,
                                      oyaPnt2.Y - (oya2BHalf - kakarishiro) * oyaInst2.HandOrientation.Y,
                                      oyaPnt2.Z);
                //double yokoyaLenght = ClsRevitUtil.CovertToAPI(ClsGeo.FloorAtDigitAdjust(1, lenght)) - oyaBHalf;// oyaT1Half * 2;


                XYZ kariPnt1 = oyaPnt1;
                XYZ kariPnt2 = oyaPnt2;

                XYZ ePnt1 = new XYZ(oyaPnt1.X + (kakarishiro) * oyaInst1.HandOrientation.X,
                                      oyaPnt1.Y + (kakarishiro) * oyaInst1.HandOrientation.Y,
                                      oyaPnt1.Z);
                XYZ ePnt2 = new XYZ(oyaPnt2.X - (kakarishiro) * oyaInst2.HandOrientation.X,
                                      oyaPnt2.Y - (kakarishiro) * oyaInst2.HandOrientation.Y,
                                      oyaPnt2.Z);

                if (m_putPosFlag == 0)
                {
                    //内にずらす
                    oyaPnt1 = new XYZ(oyaPnt1.X + oyaSizeHalfIn * oyaInst1.FacingOrientation.X,
                                      oyaPnt1.Y + oyaSizeHalfIn * oyaInst1.FacingOrientation.Y,
                                      oyaPnt1.Z);
                    sPnt1 = new XYZ(sPnt1.X + oyaSizeHalfIn * oyaInst1.FacingOrientation.X,
                                      sPnt1.Y + oyaSizeHalfIn * oyaInst1.FacingOrientation.Y,
                                      sPnt1.Z);
                    ePnt1 = new XYZ(ePnt1.X + oyaSizeHalfIn * oyaInst1.FacingOrientation.X,
                                      ePnt1.Y + oyaSizeHalfIn * oyaInst1.FacingOrientation.Y,
                                      ePnt1.Z);
                    kariPnt1 = new XYZ(kariPnt1.X + (oyaSizeHalfIn - oyaT2) * oyaInst1.FacingOrientation.X,
                                      kariPnt1.Y + (oyaSizeHalfIn - oyaT2) * oyaInst1.FacingOrientation.Y,
                                      kariPnt1.Z);

                    oyaPnt2 = new XYZ(oyaPnt2.X + oya2SizeHalfIn * oyaInst2.FacingOrientation.X,
                                      oyaPnt2.Y + oya2SizeHalfIn * oyaInst2.FacingOrientation.Y,
                                      oyaPnt2.Z);
                    sPnt2 = new XYZ(sPnt2.X + oya2SizeHalfIn * oyaInst2.FacingOrientation.X,
                                      sPnt2.Y + oya2SizeHalfIn * oyaInst2.FacingOrientation.Y,
                                      sPnt2.Z);
                    ePnt2 = new XYZ(ePnt2.X + oya2SizeHalfIn * oyaInst2.FacingOrientation.X,
                                      ePnt2.Y + oya2SizeHalfIn * oyaInst2.FacingOrientation.Y,
                                      ePnt2.Z);
                    kariPnt2 = new XYZ(kariPnt2.X + (oya2SizeHalfIn - oyaT2) * oyaInst2.FacingOrientation.X,
                                      kariPnt2.Y + (oya2SizeHalfIn - oyaT2) * oyaInst2.FacingOrientation.Y,
                                      kariPnt2.Z);
                }
                else if (m_putPosFlag == 1)
                {
                    //外にずらす
                    oyaPnt1 = new XYZ(oyaPnt1.X - oyaSizeHalfOut * oyaInst1.FacingOrientation.X,
                                      oyaPnt1.Y - oyaSizeHalfOut * oyaInst1.FacingOrientation.Y,
                                      oyaPnt1.Z);
                    sPnt1 = new XYZ(sPnt1.X - oyaSizeHalfOut * oyaInst1.FacingOrientation.X,
                                      sPnt1.Y - oyaSizeHalfOut * oyaInst1.FacingOrientation.Y,
                                      sPnt1.Z);
                    ePnt1 = new XYZ(ePnt1.X - oyaSizeHalfOut * oyaInst1.FacingOrientation.X,
                                      ePnt1.Y - oyaSizeHalfOut * oyaInst1.FacingOrientation.Y,
                                      ePnt1.Z);
                    kariPnt1 = new XYZ(kariPnt1.X - (oyaSizeHalfOut + oyaT2) * oyaInst1.FacingOrientation.X,
                                      kariPnt1.Y - (oyaSizeHalfOut + oyaT2) * oyaInst1.FacingOrientation.Y,
                                      kariPnt1.Z);

                    oyaPnt2 = new XYZ(oyaPnt2.X - oya2SizeHalfOut * oyaInst2.FacingOrientation.X,
                                      oyaPnt2.Y - oya2SizeHalfOut * oyaInst2.FacingOrientation.Y,
                                      oyaPnt2.Z);
                    sPnt2 = new XYZ(sPnt2.X - oya2SizeHalfOut * oyaInst2.FacingOrientation.X,
                                      sPnt2.Y - oya2SizeHalfOut * oyaInst2.FacingOrientation.Y,
                                      sPnt2.Z);
                    ePnt2 = new XYZ(ePnt2.X - oya2SizeHalfOut * oyaInst2.FacingOrientation.X,
                                      ePnt2.Y - oya2SizeHalfOut * oyaInst2.FacingOrientation.Y,
                                      ePnt2.Z);
                    kariPnt2 = new XYZ(kariPnt2.X - (oya2SizeHalfOut + oyaT2) * oyaInst2.FacingOrientation.X,
                                      kariPnt2.Y - (oya2SizeHalfOut + oyaT2) * oyaInst2.FacingOrientation.Y,
                                      kariPnt2.Z);
                }
                Line line = Line.CreateBound(oyaPnt1, oyaPnt2);
                double yokoyaLenght = line.Length;

                //斜めかの判定用
                Curve kariLine = Line.CreateBound(kariPnt1, kariPnt2);

                //ModelLine linekari = ClsYMSUtil.CreateKabeHojyoLine(doc, kariPnt1, kariPnt2, 0, 0);
                //linekari = ClsYMSUtil.CreateKabeHojyoLine(doc, sPnt1, ePnt1, 0, 0);
                //linekari = ClsYMSUtil.CreateKabeHojyoLine(doc, sPnt2, ePnt2, 0, 0);

                //斜めの時用
                Curve line1 = Line.CreateBound(sPnt1, ePnt1);
                XYZ insec = ClsRevitUtil.GetIntersection(kariLine, line1);
                if (insec != null)
                {
                    oyaPnt1 = new XYZ(oyaPnt1.X + (kakarishiro) * oyaInst1.HandOrientation.X,// - (yokoyaWidthHalf + oyaT2) * oyaInst1.FacingOrientation.X,
                                      oyaPnt1.Y + (kakarishiro) * oyaInst1.HandOrientation.Y,// - (yokoyaWidthHalf + oyaT2) * oyaInst1.FacingOrientation.Y,
                                      oyaPnt1.Z);
                    if (m_putPosFlag == 0)
                        oyaPnt2 = sPnt2;
                }
                Curve line2 = Line.CreateBound(sPnt2, ePnt2);
                insec = ClsRevitUtil.GetIntersection(kariLine, line2);
                if (insec != null)
                {
                    oyaPnt2 = new XYZ(oyaPnt2.X - (kakarishiro) * oyaInst2.HandOrientation.X,
                                      oyaPnt2.Y - (kakarishiro) * oyaInst2.HandOrientation.Y,
                                      oyaPnt2.Z);
                    if (m_putPosFlag == 0)
                        oyaPnt1 = sPnt1;
                }

                line = Line.CreateBound(oyaPnt1, oyaPnt2);
                yokoyaLenght = line.Length;
                XYZ dir = line.Direction;
                Line axis = Line.CreateBound(oyaPnt1, oyaPnt1 + XYZ.BasisZ);

                if (!ClsGeo.GEO_EQ(dir.X, 0) && !ClsGeo.GEO_EQ(dir.Y, 0))
                {
                    //斜めであると判定
                }

                //CASE取得
                string caseNum = ClsRevitUtil.GetTypeParameterString(oyaInst1.Symbol, "CASE");

                using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                {
                    t.Start();
                    FailureHandlingOptions failOpt = t.GetFailureHandlingOptions();
                    failOpt.SetFailuresPreprocessor(new WarningSwallower());
                    t.SetFailureHandlingOptions(failOpt);

                    ClsRevitUtil.SetTypeParameter(sym, "CASE", caseNum);

                    //杭の上から作図していき積み下げていく
                    for (int i = 0; i < fullLenght / yokoyaHigh; i++)
                    {
                        ElementId CreatedID = ClsRevitUtil.Create(doc, oyaPnt1, levelId, sym);
                        ClsRevitUtil.SetParameter(doc, CreatedID, ClsGlobal.m_refLvTop, -yokoyaHigh * i + oyaKijyun);
                        ClsRevitUtil.SetParameter(doc, CreatedID, ClsGlobal.m_length, yokoyaLenght);
                        //長さでタイプを複製
                        //sym = ClsRevitUtil.ChangeTypeID(doc, sym, CreatedID, ClsRevitUtil.CreateTypeName(familyName, yokoyaLenght));
                        //回転
                        FamilyInstance yokoInst1 = doc.GetElement(CreatedID) as FamilyInstance;
                        double angle = dir.AngleTo(yokoInst1.HandOrientation);

                        if (!ClsGeo.IsLeft(yokoInst1.HandOrientation, dir))
                        {
                            angle = -angle;
                        }
                        ClsRevitUtil.RotateElement(doc, CreatedID, axis, angle);
                    }
                    //ClsRevitUtil.SetTypeParameter(sym, ClsGlobal.m_length, yokoyaLenght);

                    //変更が加わるメッセージ原因不明
                    t.Commit();
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Info Message24", ex.Message);
                string message = ex.Message;
                MessageBox.Show(message);
            }

            return true;
        }
        /// <summary>
        /// 横矢板を作成する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="pnt">配置点</param>
        /// <param name="dAngle">回転角度</param>
        /// <param name="lenght">横長さ</param>
        /// <param name="kussaku">配置する木矢板の高さ</param>
        /// <returns></returns>
        public bool CreateYokoyaita(Document doc, XYZ pnt, double dAngle, double lenght, double kussaku, ElementId levelId, string CASE, double oyaKijyun = 0.0)
        {
            try
            {
                string symbolFolpath = ClsZumenInfo.GetYMSFolder();

                string familyPath = Master.ClsYokoyaitaCSV.GetFamilyPath(m_size);
                string familyName = ClsRevitUtil.GetFamilyName(familyPath);

                //横矢板配置
                if (!ClsRevitUtil.LoadFamilySymbolData(doc, familyPath, familyName, out FamilySymbol sym))
                {
                    return false;
                }

                //配置する横矢板の高さは何処から取得がベストか
                double yokoyaHigh = ClsRevitUtil.CovertToAPI(Master.ClsYokoyaitaCSV.GetHight(m_size));
                Line axis = Line.CreateBound(pnt, pnt + XYZ.BasisZ);

                using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                {
                    t.Start();
                    ClsRevitUtil.SetTypeParameter(sym, "CASE", CASE);
                    //杭の上から作図していき積み下げていく
                    for (int i = 0; i < kussaku / yokoyaHigh; i++)
                    {
                        ElementId CreatedID = ClsRevitUtil.Create(doc, pnt, levelId, sym);
                        ClsRevitUtil.SetParameter(doc, CreatedID, ClsGlobal.m_refLvTop, -yokoyaHigh * i + oyaKijyun);
                        ClsRevitUtil.SetParameter(doc, CreatedID, ClsGlobal.m_length, lenght);
                        ClsRevitUtil.RotateElement(doc, CreatedID, axis, dAngle);
                    }
                    t.Commit();
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Info Message25", ex.Message);
                string message = ex.Message;
                MessageBox.Show(message);
            }

            return true;
        }
        public bool CreateMokuyaita(Document doc, ElementId oyaId1, ElementId oyaId2, double lenght, double kussaku, ElementId levelId, double oyaKijyun = 0.0)
        {
            try
            {
                string symbolFolpath = ClsZumenInfo.GetYMSFolder();

                string familyPath = Master.ClsYokoyaitaCSV.GetFamilyPath(m_size);
                string familyName = ClsRevitUtil.GetFamilyName(familyPath);

                //横矢板配置
                if (!ClsRevitUtil.LoadFamilySymbolData(doc, familyPath, familyName, out FamilySymbol sym))
                {
                    return false;
                }

                //作成する親杭同士の位置判定
                (oyaId1, oyaId2) = CheckOyakuiIsRight(doc, oyaId1, oyaId2);

                //配置する木矢板の高さは掘削深さ
                double yokoyaHigh = kussaku;
                //木矢板横幅
                double yokoyaWidthHalf = ClsRevitUtil.CovertToAPI(Master.ClsYokoyaitaCSV.GetWidth(m_size) / 2);

                FamilyInstance oyaInst1 = doc.GetElement(oyaId1) as FamilyInstance;
                XYZ oyaPnt1 = (oyaInst1.Location as LocationPoint).Point;
                string oyaSymName = oyaInst1.Symbol.FamilyName;
                FamilyInstance oyaInst2 = doc.GetElement(oyaId2) as FamilyInstance;
                XYZ oyaPnt2 = (oyaInst2.Location as LocationPoint).Point;
                string oyaSymName2 = oyaInst1.Symbol.FamilyName;

                //ﾌﾗﾝｼﾞにかかる横矢板のかかりしろ
                double kakarishiro = ClsRevitUtil.CovertToAPI(40.0);

                //高さ
                double oyaSizeHalf = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou1(oyaSymName, 1)) / 2);//ClsRevitUtil.CovertToAPI(300 / 2);
                                                                                                                                                 //フランジ幅
                double oyaBHalf = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou1(oyaSymName, 2)) / 2);//ClsRevitUtil.CovertToAPI(300 / 2);
                                                                                                                                              //腹板厚半分※Point内ずらしと長さ変更
                double oyaT1Half = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou1(oyaSymName, 3)) / 2);//ClsRevitUtil.CovertToAPI(10 / 2);
                                                                                                                                               //フランジ厚
                double oyaT2 = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou1(oyaSymName, 4)));//ClsRevitUtil.CovertToAPI(15);

                //高さ2
                double oya2SizeHalf = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou1(oyaSymName2, 1)) / 2);//ClsRevitUtil.CovertToAPI(300 / 2);
                                                                                                                                                   //フランジ幅2
                double oya2BHalf = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou1(oyaSymName2, 2)) / 2);//ClsRevitUtil.CovertToAPI(300 / 2);
                                                                                                                                                //腹板厚半分2
                double oya2T1Half = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou1(oyaSymName2, 3)) / 2);//ClsRevitUtil.CovertToAPI(10 / 2);
                                                                                                                                                 //フランジ厚2
                double oya2T2 = ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(ClsYMSUtil.GetKouzaiSizeSunpou1(oyaSymName2, 4)));//ClsRevitUtil.CovertToAPI(15);


                double oyaSizeHalfIn = oyaSizeHalf - oyaT2 - yokoyaWidthHalf;
                double oyaSizeHalfOut = oyaSizeHalf + yokoyaWidthHalf;

                double oya2SizeHalfIn = oya2SizeHalf - oya2T2 - yokoyaWidthHalf;
                double oya2SizeHalfOut = oya2SizeHalf + yokoyaWidthHalf;

                XYZ sPnt1 = oyaPnt1;
                XYZ sPnt2 = oyaPnt2;

                oyaPnt1 = new XYZ(oyaPnt1.X + (oyaBHalf - kakarishiro) * oyaInst1.HandOrientation.X,
                                      oyaPnt1.Y + (oyaBHalf - kakarishiro) * oyaInst1.HandOrientation.Y,
                                      oyaPnt1.Z);
                oyaPnt2 = new XYZ(oyaPnt2.X - (oya2BHalf - kakarishiro) * oyaInst2.HandOrientation.X,
                                      oyaPnt2.Y - (oya2BHalf - kakarishiro) * oyaInst2.HandOrientation.Y,
                                      oyaPnt2.Z);
                //double yokoyaLenght = ClsRevitUtil.CovertToAPI(ClsGeo.FloorAtDigitAdjust(1, lenght)) - oyaBHalf;// oyaT1Half * 2;


                XYZ kariPnt1 = oyaPnt1;
                XYZ kariPnt2 = oyaPnt2;

                XYZ ePnt1 = new XYZ(oyaPnt1.X + (kakarishiro) * oyaInst1.HandOrientation.X,
                                      oyaPnt1.Y + (kakarishiro) * oyaInst1.HandOrientation.Y,
                                      oyaPnt1.Z);
                XYZ ePnt2 = new XYZ(oyaPnt2.X - (kakarishiro) * oyaInst2.HandOrientation.X,
                                      oyaPnt2.Y - (kakarishiro) * oyaInst2.HandOrientation.Y,
                                      oyaPnt2.Z);

                if (m_putPosFlag == 0)
                {
                    //内にずらす
                    oyaPnt1 = new XYZ(oyaPnt1.X + oyaSizeHalfIn * oyaInst1.FacingOrientation.X,
                                      oyaPnt1.Y + oyaSizeHalfIn * oyaInst1.FacingOrientation.Y,
                                      oyaPnt1.Z);
                    sPnt1 = new XYZ(sPnt1.X + oyaSizeHalfIn * oyaInst1.FacingOrientation.X,
                                      sPnt1.Y + oyaSizeHalfIn * oyaInst1.FacingOrientation.Y,
                                      sPnt1.Z);
                    ePnt1 = new XYZ(ePnt1.X + oyaSizeHalfIn * oyaInst1.FacingOrientation.X,
                                      ePnt1.Y + oyaSizeHalfIn * oyaInst1.FacingOrientation.Y,
                                      ePnt1.Z);
                    kariPnt1 = new XYZ(kariPnt1.X + (oyaSizeHalfIn - oyaT2) * oyaInst1.FacingOrientation.X,
                                      kariPnt1.Y + (oyaSizeHalfIn - oyaT2) * oyaInst1.FacingOrientation.Y,
                                      kariPnt1.Z);

                    oyaPnt2 = new XYZ(oyaPnt2.X + oya2SizeHalfIn * oyaInst2.FacingOrientation.X,
                                      oyaPnt2.Y + oya2SizeHalfIn * oyaInst2.FacingOrientation.Y,
                                      oyaPnt2.Z);
                    sPnt2 = new XYZ(sPnt2.X + oya2SizeHalfIn * oyaInst2.FacingOrientation.X,
                                      sPnt2.Y + oya2SizeHalfIn * oyaInst2.FacingOrientation.Y,
                                      sPnt2.Z);
                    ePnt2 = new XYZ(ePnt2.X + oya2SizeHalfIn * oyaInst2.FacingOrientation.X,
                                      ePnt2.Y + oya2SizeHalfIn * oyaInst2.FacingOrientation.Y,
                                      ePnt2.Z);
                    kariPnt2 = new XYZ(kariPnt2.X + (oya2SizeHalfIn - oyaT2) * oyaInst2.FacingOrientation.X,
                                      kariPnt2.Y + (oya2SizeHalfIn - oyaT2) * oyaInst2.FacingOrientation.Y,
                                      kariPnt2.Z);
                }
                else if (m_putPosFlag == 1)
                {
                    //外にずらす
                    oyaPnt1 = new XYZ(oyaPnt1.X - oyaSizeHalfOut * oyaInst1.FacingOrientation.X,
                                      oyaPnt1.Y - oyaSizeHalfOut * oyaInst1.FacingOrientation.Y,
                                      oyaPnt1.Z);
                    sPnt1 = new XYZ(sPnt1.X - oyaSizeHalfOut * oyaInst1.FacingOrientation.X,
                                      sPnt1.Y - oyaSizeHalfOut * oyaInst1.FacingOrientation.Y,
                                      sPnt1.Z);
                    ePnt1 = new XYZ(ePnt1.X - oyaSizeHalfOut * oyaInst1.FacingOrientation.X,
                                      ePnt1.Y - oyaSizeHalfOut * oyaInst1.FacingOrientation.Y,
                                      ePnt1.Z);
                    kariPnt1 = new XYZ(kariPnt1.X - (oyaSizeHalfOut + oyaT2) * oyaInst1.FacingOrientation.X,
                                      kariPnt1.Y - (oyaSizeHalfOut + oyaT2) * oyaInst1.FacingOrientation.Y,
                                      kariPnt1.Z);

                    oyaPnt2 = new XYZ(oyaPnt2.X - oya2SizeHalfOut * oyaInst2.FacingOrientation.X,
                                      oyaPnt2.Y - oya2SizeHalfOut * oyaInst2.FacingOrientation.Y,
                                      oyaPnt2.Z);
                    sPnt2 = new XYZ(sPnt2.X - oya2SizeHalfOut * oyaInst2.FacingOrientation.X,
                                      sPnt2.Y - oya2SizeHalfOut * oyaInst2.FacingOrientation.Y,
                                      sPnt2.Z);
                    ePnt2 = new XYZ(ePnt2.X - oya2SizeHalfOut * oyaInst2.FacingOrientation.X,
                                      ePnt2.Y - oya2SizeHalfOut * oyaInst2.FacingOrientation.Y,
                                      ePnt2.Z);
                    kariPnt2 = new XYZ(kariPnt2.X - (oya2SizeHalfOut + oyaT2) * oyaInst2.FacingOrientation.X,
                                      kariPnt2.Y - (oya2SizeHalfOut + oyaT2) * oyaInst2.FacingOrientation.Y,
                                      kariPnt2.Z);
                }
                Line line = Line.CreateBound(oyaPnt1, oyaPnt2);
                double yokoyaLenght = line.Length;

                //斜めかの判定用
                Curve kariLine = Line.CreateBound(kariPnt1, kariPnt2);

                //ModelLine linekari = ClsYMSUtil.CreateKabeHojyoLine(doc, kariPnt1, kariPnt2, 0, 0);
                //linekari = ClsYMSUtil.CreateKabeHojyoLine(doc, sPnt1, ePnt1, 0, 0);
                //linekari = ClsYMSUtil.CreateKabeHojyoLine(doc, sPnt2, ePnt2, 0, 0);

                //斜めの時用
                Curve line1 = Line.CreateBound(sPnt1, ePnt1);
                XYZ insec = ClsRevitUtil.GetIntersection(kariLine, line1);
                if (insec != null)
                {
                    oyaPnt1 = new XYZ(oyaPnt1.X + (kakarishiro) * oyaInst1.HandOrientation.X,// - (yokoyaWidthHalf + oyaT2) * oyaInst1.FacingOrientation.X,
                                      oyaPnt1.Y + (kakarishiro) * oyaInst1.HandOrientation.Y,// - (yokoyaWidthHalf + oyaT2) * oyaInst1.FacingOrientation.Y,
                                      oyaPnt1.Z);
                }
                Curve line2 = Line.CreateBound(sPnt2, ePnt2);
                insec = ClsRevitUtil.GetIntersection(kariLine, line2);
                if (insec != null)
                {
                    oyaPnt2 = new XYZ(oyaPnt2.X - (kakarishiro) * oyaInst2.HandOrientation.X,
                                      oyaPnt2.Y - (kakarishiro) * oyaInst2.HandOrientation.Y,
                                      oyaPnt2.Z);
                }

                line = Line.CreateBound(oyaPnt1, oyaPnt2);
                yokoyaLenght = line.Length;
                XYZ dir = line.Direction;
                Line axis = Line.CreateBound(oyaPnt1, oyaPnt1 + XYZ.BasisZ);

                //CASE取得
                string caseNum = ClsRevitUtil.GetTypeParameterString(oyaInst1.Symbol, "CASE");

                using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                {
                    t.Start();
                    FailureHandlingOptions failOpt = t.GetFailureHandlingOptions();
                    failOpt.SetFailuresPreprocessor(new WarningSwallower());
                    t.SetFailureHandlingOptions(failOpt);

                    
                    ElementId CreatedID = ClsRevitUtil.Create(doc, oyaPnt1, levelId, sym);
                    ClsRevitUtil.SetParameter(doc, CreatedID, ClsGlobal.m_length, yokoyaLenght);

                    //CASEでタイプを複製
                    sym = ClsRevitUtil.ChangeTypeID(doc, sym, CreatedID, familyName + "_" + caseNum);
                    ClsRevitUtil.SetTypeParameter(sym, "CASE", caseNum);

                    ClsRevitUtil.SetParameter(doc, CreatedID, ClsGlobal.m_refLvTop, oyaKijyun);

                    //回転
                    FamilyInstance yokoInst1 = doc.GetElement(CreatedID) as FamilyInstance;
                    double angle = dir.AngleTo(yokoInst1.HandOrientation);

                    if (!ClsGeo.IsLeft(yokoInst1.HandOrientation, dir))
                    {
                        angle = -angle;
                    }
                    ClsRevitUtil.RotateElement(doc, CreatedID, axis, angle);

                    //長さと深さを決めるタイプパラメータなのでどのようになるかは検討中
                    //ClsRevitUtil.SetTypeParameter(sym, ClsGlobal.m_length, yokoyaLenght);
                    ClsRevitUtil.SetTypeParameter(sym, "掘削深さ", yokoyaHigh);

                    t.Commit();
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Info Message26", ex.Message);
                string message = ex.Message;
                MessageBox.Show(message);
            }

            return true;
        }
        /// <summary>
        /// 木矢板を作成する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="pnt">配置点</param>
        /// <param name="dAngle">回転角度</param>
        /// <param name="lenght">横長さ</param>
        /// <param name="kussaku">配置する木矢板の高さ</param>
        /// <returns></returns>
        public bool CreateMokuyaita(Document doc, XYZ pnt, double dAngle, double lenght, double kussaku, ElementId levelId, string CASE, double oyaKijyun = 0.0)
        {
            try
            {
                string familyPath = Master.ClsYokoyaitaCSV.GetFamilyPath(m_size);
                string familyName = ClsRevitUtil.GetFamilyName(familyPath);

                //横矢板配置
                if (!ClsRevitUtil.LoadFamilySymbolData(doc, familyPath, familyName, out FamilySymbol sym))
                {
                    return false;
                }

                //配置する木矢板の高さは掘削深さ
                double yokoyaHigh = kussaku;

                Line axis = Line.CreateBound(pnt, pnt + XYZ.BasisZ);

                using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                {
                    t.Start();
                    ElementId CreatedID = ClsRevitUtil.Create(doc, pnt, levelId, sym);
                    ClsRevitUtil.SetParameter(doc, CreatedID, ClsGlobal.m_length, lenght);
                    ClsRevitUtil.RotateElement(doc, CreatedID, axis, dAngle);

                    //CASEでタイプを複製
                    sym = ClsRevitUtil.ChangeTypeID(doc, sym, CreatedID, familyName + "_" + CASE);
                    ClsRevitUtil.SetTypeParameter(sym, "CASE", CASE);

                    ClsRevitUtil.SetParameter(doc, CreatedID, ClsGlobal.m_refLvTop, oyaKijyun);
                    ClsRevitUtil.SetTypeParameter(sym, "掘削深さ", yokoyaHigh);

                    t.Commit();
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Info Message27", ex.Message);
                string message = ex.Message;
                MessageBox.Show(message);
            }

            return true;
        }
        
        
        /// <summary>
        /// 横矢板かかりしろ長さのiniファイルからの取得
        /// </summary>
        /// <returns></returns>
        public static double GetKakarishiroLength()
        {
            const double kakarishiro = 40.0;//40が一応のデフォルト

            string iniPath = ClsIni.GetIniFilePath(ClsZumenInfo.iniName);
            string res = ClsIni.GetIniFile(ClsZumenInfo.inisecSetting, ClsZumenInfo.inikeyYOKOYAITA_KAKARISHIRO, iniPath);

            if (string.IsNullOrWhiteSpace(res))
            {
                //取得に失敗した場合は規定値返す
                return kakarishiro;
            }

            double r = 0;
            if (double.TryParse(res, out r))
            {
                return r;
            }

            //取得に失敗した場合は規定値を返す
            return kakarishiro;
        }

        public static (ElementId, ElementId) CheckOyakuiIsRight(Document doc, ElementId oyaId1, ElementId oyaId2)
        {
            FamilyInstance oyaInst1 = doc.GetElement(oyaId1) as FamilyInstance;
            XYZ oyaPnt1 = (oyaInst1.Location as LocationPoint).Point;

            FamilyInstance oyaInst2 = doc.GetElement(oyaId2) as FamilyInstance;
            XYZ oyaPnt2 = (oyaInst2.Location as LocationPoint).Point;

            XYZ dir = Line.CreateBound(oyaPnt1, oyaPnt2).Direction;

            if (ClsGeo.IsLeft(oyaInst1.FacingOrientation, dir))
            {
                return (oyaId2, oyaId1);//左にあれば入れ替え
            }
            else
                return (oyaId1, oyaId2);//右にあればそのまま
        }

        public static List<List<ElementId>> GroupingOyakui(Document doc, List<ElementId> oyaIdList, out List<ElementId> cornerGroupList)
        {
            List<List<ElementId>> handGroupList = new List<List<ElementId>>();
            cornerGroupList = new List<ElementId>();
            List<XYZ> handDirList = new List<XYZ>();
            //
            foreach(ElementId id in oyaIdList)
            {
                FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                XYZ instHand = inst.HandOrientation;
                //コーナー親杭をまとめる
                if (ClsOyakui.GetCornerFlag(doc, id))
                {
                    cornerGroupList.Add(id);
                    continue;
                }

                bool handCheck = true;
                foreach(XYZ hand in handDirList)
                {
                    if(ClsGeo.GEO_EQ(instHand, hand))
                    {
                        handCheck = false;
                        break;
                    }
                }
                if (handCheck)
                    handDirList.Add(instHand);
            }
            
            foreach(XYZ hand in handDirList)
            {
                List<ElementId> handList = new List<ElementId>();
                foreach (ElementId id in oyaIdList)
                {
                    FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
                    XYZ instHand = inst.HandOrientation;
                    if (ClsGeo.GEO_EQ(instHand, hand))
                        handList.Add(id);
                }
                handGroupList.Add(handList);
            }

            return handGroupList;
        }
        #endregion
    }

    public class Yokoyaita
    {
        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="point"></param>
        /// <param name="index"></param>
        /// <param name="sortedSp"></param>
        /// <param name="dir"></param>
        public Yokoyaita(XYZ point, double angle,
                           double length, double kussaku, double dHTop, string CASE)
        {
            this.Point = point;
            this.Angle = angle;
            this.Length = length;
            this.Kussaku = kussaku;
            this.Create = true;
            this.HTop = dHTop;
            this.CASE = CASE;
        }

        #endregion

        #region プロパティ

        /// <summary>点の座標</summary>
        public XYZ Point { get; set; }

        /// <summary>点の角度</summary>
        public double Angle { get; set; }

        /// <summary>横矢板の長さ</summary>
        public double Length { get; set; }

        /// <summary>掘削深さ</summary>
        public double Kussaku { get; set; }

        /// <summary>この点で作図するか否か</summary>
        public bool Create { get; set; }

        /// <summary> 横矢板の基準高さ</summary>
        public double HTop { get; set; }

        /// <summary>CASE番号/// </summary>
        public string CASE { get; set; }
        #endregion

        /// <summary>
        /// XYが同一の点を絞る、同一の点の高さを足す
        /// </summary>
        /// <param name="yokoyaList"></param>
        /// <returns></returns>
        public static List<Yokoyaita> SortPoint(List<Yokoyaita> yokoyaList)
        {
            for (int i = 0; i < yokoyaList.Count; i++)
            {
                if (!yokoyaList[i].Create)
                    continue;
                XYZ point = yokoyaList[i].Point;
                for(int j = 0; j < yokoyaList.Count; j++)
                {
                    if (i == j)
                        continue;
                    XYZ point2 = yokoyaList[j].Point;
                    if (ClsGeo.GEO_EQ(point.X, point2.X) && ClsGeo.GEO_EQ(point.Y, point2.Y))
                    {
                        yokoyaList[i].Kussaku += yokoyaList[j].Kussaku;
                        yokoyaList[j].Create = false;
                    }
                }
            }
            return yokoyaList;
        }
    }
}
