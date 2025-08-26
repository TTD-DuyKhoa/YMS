using Autodesk.Revit.DB;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMS.Parts
{
    public class ClsDesumibuHokyouzai
    {
        public int m_Top { get; set; }


        public int m_Pitch { get; set; }

        #region コントラスタ
        public ClsDesumibuHokyouzai()
        {
            Init();
        }
        #endregion

        #region メソッド
        public void Init()
        {
            m_Top = 0;
            m_Pitch = 0;
        }
        /// <summary>
        /// 出隅部補強材作成
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="levelId"></param>
        /// <param name="insertPoint"></param>
        /// <param name="sumikiriLength"></param>
        /// <param name="length1"></param>
        /// <param name="length2"></param>
        /// <param name="voidDep"></param>
        /// <param name="angle">ファミリのパラメータに使用する角度</param>
        /// <param name="rotation">ファミリ自体を回す回転角</param>
        /// <returns></returns>
        public bool CreateDesumibuHokyouzai(Document doc, ElementId levelId, XYZ insertPoint, double sumikiriLength, double length1, double length2 ,double voidDep, double angle , double rotation)
        {
            try
            {
                string symbolFolpath = ClsZumenInfo.GetYMSFolder();
                string familyPath = System.IO.Path.Combine(symbolFolpath, "山留壁関係\\07_出隅部補強材\\出隅部補強材_FB6x65xL.rfa");
                string familyName = "出隅部補強材_FB6x65xL";
                //シンボル
                if (!ClsRevitUtil.LoadFamilySymbolData(doc, familyPath, familyName, out FamilySymbol sym))
                {
                    return false;
                }

                double pitch = m_Pitch;
                double count = voidDep / pitch;

                using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                {
                    t.Start();
                    FailureHandlingOptions failOpt = t.GetFailureHandlingOptions();
                    failOpt.SetFailuresPreprocessor(new WarningSwallower());
                    t.SetFailureHandlingOptions(failOpt);

                    for (int i = 0; i < count; i++)
                    {
                        ElementId CreatedId = ClsRevitUtil.Create(doc, insertPoint, levelId, sym);
                        Line axis = Line.CreateBound(insertPoint, insertPoint + XYZ.BasisZ);
                        ClsRevitUtil.RotateElement(doc, CreatedId, axis, -rotation);

                        ClsRevitUtil.SetParameter(doc, CreatedId, ClsGlobal.m_refLvTop, -ClsRevitUtil.CovertToAPI(m_Top) - (ClsRevitUtil.CovertToAPI(pitch * i)));

                        sym = ClsRevitUtil.ChangeTypeID(doc, sym, CreatedId, familyName + ClsGeo.FloorAtDigitAdjust(0, ClsRevitUtil.CovertFromAPI(length1)).ToString() + 
                            "+" + ClsGeo.FloorAtDigitAdjust(0, ClsRevitUtil.CovertFromAPI(length2)).ToString() + 
                            "+" + ClsGeo.FloorAtDigitAdjust(0, ClsRevitUtil.CovertFromAPI(sumikiriLength)).ToString());
                        ClsRevitUtil.SetTypeParameter(sym, "隅切長さ", sumikiriLength);
                        ClsRevitUtil.SetTypeParameter(sym, "長さ1",length1);
                        ClsRevitUtil.SetTypeParameter(sym, "長さ2", length2);
                        if (angle < 0)
                            angle *= -1;
                        ClsRevitUtil.SetTypeParameter(sym, "角度", angle);
                    }
                    t.Commit();
                }
            }
            catch
            {

            }
            return true;
        }

        public static List<ElementId> GetAllDesumibuhokyouzai(Document doc)
        {
            //図面上の連続壁を全て取得
            List<ElementId> idList = ClsRevitUtil.GetSelectCreatedFamilyInstanceList(doc, "出隅部補強材", true);
            return idList;
        }

        #endregion
    }
}
