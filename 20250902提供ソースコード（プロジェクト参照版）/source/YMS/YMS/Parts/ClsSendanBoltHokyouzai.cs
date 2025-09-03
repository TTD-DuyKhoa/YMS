using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YMS.DLG;

namespace YMS.Parts
{
    public class ClsSendanBoltHokyouzai
    {
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
        /// ボルトタイプ
        /// </summary>
        public string m_BoltType { get; set; }

        /// <summary>
        /// ボルトサイズ
        /// </summary>
        public string m_BoltSize { get; set; }

        /// <summary>
        /// ボルト本数
        /// </summary>
        public int m_BoltNum { get; set; }

        #endregion

        public bool CreateSendanBoltHokyouzai(Document doc, XYZ ptInsert, ElementId levelId, XYZ ptSt, XYZ ptEd, double offset, ElementId pieceId, double hokyouzaiHaba)
        {
            // ファミリの取得処理
            string familyPath = string.Empty;
            familyPath = Master.ClsSendanBoltHokyouzaiCsv.GetFamilyPath(m_KouzaiSize);
            string symbolName = ClsRevitUtil.GetFamilyName(familyPath);
            if (!ClsRevitUtil.LoadFamilyData(doc, familyPath, out Family family))
            {
                return false;
            }
            FamilySymbol kouzaiSym = ClsRevitUtil.GetFamilySymbol(doc, symbolName, symbolName);

            // ファミリの配置処理
            ElementId createdId = null;
            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                t.Start();

                createdId = ClsRevitUtil.Create(doc, ptInsert, levelId, kouzaiSym);
                ClsRevitUtil.SetParameter(doc, createdId, "基準レベルからの高さ", offset);

                //ボルト情報をカスタムデータとして設定する #31575
                ClsYMSUtil.SetBolt(doc, createdId, m_BoltSize, m_BoltNum);

                //シンボルを回転
                Line rotationAxis = Line.CreateBound(ptInsert, ptInsert + XYZ.BasisZ);    // Z軸周りに回転
                XYZ vec = (ptEd - ptSt).Normalize();
                double radians = XYZ.BasisX.AngleOnPlaneTo(vec, XYZ.BasisZ);
                ElementTransformUtils.RotateElement(doc, createdId, rotationAxis, radians);

                t.Commit();
            }

            List<ElementId> pieceIds = new List<ElementId>();
            pieceIds.Add(pieceId);
            List<ElementId> hitIds = new List<ElementId>();
            if (ClsRevitUtil.CheckCollision2(doc, createdId, pieceIds, ref hitIds))
            {
                using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
                {
                    t.Start();
                    XYZ vec = (ptEd - ptSt).Normalize();
                    ElementTransformUtils.MoveElement(doc, createdId, -(vec * hokyouzaiHaba));
                    t.Commit();
                }
            }

            return true;
        }

        public static bool PickObject(UIDocument uidoc, ref ElementId id, string message)
        {
            // 火打受ピースのファミリの名称が統一されれば、このリストは不要
            List<string> typeNameList = new List<string>();
            typeNameList.Add("火打受");
            typeNameList.Add("隅火打");

            return ClsRevitUtil.PickObjectPartListFilter(uidoc, message + "を選択してください", typeNameList, ref id);
        }

        public static XYZ GetIntersectionPointWithLine(Document doc, XYZ startPoint, XYZ endPoint, ElementId partId)
        {
            View3D view3D = ClsYMSUtil.Get3DView(doc);

            // 線分の方向ベクトルを計算
            XYZ rayDirection = endPoint - startPoint;

            // ReferenceIntersectorを初期化して交差を見つける
            ReferenceIntersector refIntersector = new ReferenceIntersector(view3D);
            IList<ReferenceWithContext> references = refIntersector.Find(startPoint, rayDirection);

            foreach (ReferenceWithContext referenceWithContext in references)
            {
                Reference reference = referenceWithContext.GetReference();
                ElementId intersectedElementId = reference.ElementId;

                // 部品と交差しているかどうかを確認
                if (intersectedElementId == partId)
                {
                    // 交差点を取得
                    XYZ intersectionPoint = reference.GlobalPoint;
                    return intersectionPoint;
                }
            }

            // 交差が見つからない場合はnullを返す
            return null;
        }

        public static XYZ[] GetIntersectionPointsWithLine(Document doc, XYZ startPoint, XYZ endPoint, ElementId partId)
        {
            View3D view3D = ClsYMSUtil.Get3DView(doc);

            // 線分の方向ベクトルを計算
            XYZ rayDirection = endPoint - startPoint;

            // ReferenceIntersectorを初期化して交差を見つける
            ReferenceIntersector refIntersector = new ReferenceIntersector(view3D);
            IList<ReferenceWithContext> references = refIntersector.Find(startPoint, rayDirection);

            // 交点のリストを作成
            List<XYZ> intersectionPoints = new List<XYZ>();
            XYZ firstIntersection = null;

            foreach (ReferenceWithContext referenceWithContext in references)
            {
                Reference reference = referenceWithContext.GetReference();
                ElementId intersectedElementId = reference.ElementId;

                // 部品と交差しているかどうかを確認
                if (intersectedElementId == partId)
                {
                    // 交差点を取得
                    XYZ intersectionPoint = reference.GlobalPoint;

                    if (firstIntersection == null)
                    {
                        // 最初の交点を設定
                        firstIntersection = intersectionPoint;
                        intersectionPoints.Add(firstIntersection);
                    }
                    else if (!firstIntersection.IsAlmostEqualTo(intersectionPoint))
                    {
                        // 最初の交点と異なる場合、次の交点を追加
                        intersectionPoints.Add(intersectionPoint);
                    }

                    // 2つの有効な交点が見つかったら終了
                    if (intersectionPoints.Count == 2)
                    {
                        break;
                    }
                }
            }

            // 最初の交点と次の交点を返す（なければnullを返す）
            if (intersectionPoints.Count > 0)
            {
                XYZ firstValidIntersection = intersectionPoints[0];
                XYZ secondValidIntersection = intersectionPoints.Count > 1 ? intersectionPoints[1] : null;
                return new XYZ[] { firstValidIntersection, secondValidIntersection };
            }
            else
            {
                return null;
            }
        }

    }
}
