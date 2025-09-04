using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;

namespace YMS.Parts
{
  public class ClsKirikaePiece
  {
    public ElementId m_CreatedId ;

    public bool CreateKirikaePiece( Document doc, XYZ ptSt, XYZ ptEd, ElementId levelId, string baseName,
      bool isReverse, double baseOffset = 0.0 )
    {
      // ファミリの取得処理
      string familyPath = string.Empty ;
      familyPath = Master.ClsSupportPieceCsv.GetFamilyPath( "604CD" ) ;
      string symbolName = ClsRevitUtil.GetFamilyName( familyPath ) ;
      if ( ! ClsRevitUtil.LoadFamilyData( doc, familyPath, out Family family ) ) {
        return false ;
      }

      FamilySymbol kouzaiSym = ClsRevitUtil.GetFamilySymbol( doc, symbolName, baseName ) ;

      // ファミリの配置処理
      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        t.Start() ;

        ElementId createdId = ClsRevitUtil.Create( doc, ptSt, levelId, kouzaiSym ) ;
        m_CreatedId = createdId ;
        FamilyInstance inst = doc.GetElement( createdId ) as FamilyInstance ;
        ClsRevitUtil.SetParameter( doc, createdId, "基準レベルからの高さ", baseOffset ) ;

        //bool isReverse = ShowRotationDialog();

        double angle = GetRotateAngle( ptEd, ptSt, inst ) ;
        if ( ! isReverse ) {
          angle = GetRotateAngle( ptSt, ptEd, inst ) ;
        }

        ElementTransformUtils.RotateElement( doc, createdId, Line.CreateBound( ptSt, ptSt + XYZ.BasisZ ), angle ) ;
        if ( ! isReverse ) {
          XYZ vec = ( ptEd - ptSt ).Normalize() ;
          XYZ moveVector = vec * ClsRevitUtil.CovertToAPI( 500.0 ) ;
          ElementTransformUtils.MoveElement( doc, createdId, moveVector ) ;
        }

        t.Commit() ;
      }

      return true ;
    }

    private bool ShowRotationDialog()
    {
      TaskDialog td = new TaskDialog( "切替ピースの反転" ) ;
      td.MainInstruction = "切替ピースを反転しますか？" ;
      td.CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No ;
      td.DefaultButton = TaskDialogResult.No ;

      TaskDialogResult result = td.Show() ;
      if ( result == TaskDialogResult.Yes ) {
        return true ;
      }
      else if ( result == TaskDialogResult.No ) {
        return false ;
      }

      return false ;
    }

    private static double GetRotateAngle( XYZ basePoint, XYZ insertPointSt, FamilyInstance inst )
    {
      // 基準点から挿入点へのベクトルを取得
      XYZ direction = basePoint - insertPointSt ;

      // ベクトルをZ軸に対応する方向に変換（プロジェクト平面への変換）
      XYZ projectedDirection = new XYZ( direction.X, direction.Y, 0 ).Normalize() ;

      // デフォルトの向きを取得
      //XYZ defaultFacing = inst.FacingOrientation;

      // 無回転時の向き
      XYZ defaultFacing = new XYZ( -1, 0, 0 ) ;

      // Z軸と計算した方向ベクトルのなす角を求める
      double angle = projectedDirection.AngleOnPlaneTo( -XYZ.BasisX, XYZ.BasisZ ) ;

      return -angle ;
    }
  }
}