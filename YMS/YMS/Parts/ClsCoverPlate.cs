using Autodesk.Revit.DB ;
using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;

namespace YMS.Parts
{
  public class ClsCoverPlate
  {
    #region Enum

    #endregion

    #region メンバ変数

    public ElementId m_CreatedId ;

    #endregion

    #region メソッド

    public bool CreateCoverPlate( Document doc, XYZ ptSt, XYZ ptEd, FamilyInstance instBase, string size,
      string baseName, bool isKussaku, XYZ vecOffset = null, double baseoffset = 0.0 )
    {
      ElementId levelId = instBase.Host.Id ;
      string steelSize = size.Contains( "SMH" ) ? size.Replace( "SMH", "" ) : size.Replace( "HA", "" ) ;
      double kouzaiSizeHalf = ClsRevitUtil.CovertToAPI( ( ClsCommonUtils.ChangeStrToDbl( steelSize ) * 10 ) / 2 ) ;

      double coverPlateThickness = double.Parse( Master.ClsCoverPlateCSV.GetThickness( size ) ) ;

      // ファミリの取得処理
      string familyPath = string.Empty ;
      familyPath = Master.ClsCoverPlateCSV.GetFamilyPath( size ) ;
      string symbolName = ClsRevitUtil.GetFamilyName( familyPath ) ;
      if ( ! ClsRevitUtil.LoadFamilyData( doc, familyPath, out Family family ) ) {
        return false ;
      }

      FamilySymbol kouzaiSym = ClsRevitUtil.GetFamilySymbol( doc, symbolName, baseName ) ;

      // ファミリの配置処理
      using ( Transaction t = new Transaction( doc, "Create First Cover Plate" ) ) {
        t.Start() ;
        ElementId createdId = null ;
        XYZ vec = ( ptEd - ptSt ).Normalize() ;
        Line rotationAxis = Line.CreateBound( ptSt, ptSt + XYZ.BasisZ ) ; // Z軸周りに回転
        XYZ normal = XYZ.BasisZ ;
        bool rPlane = false ;
        if ( doc.GetElement( levelId ) is ReferencePlane referencePlane ) {
          createdId = ClsRevitUtil.Create( doc, ptSt, vec, referencePlane, kouzaiSym ) ;
          ClsRevitUtil.SetParameter( doc, createdId, "集計レベル",
            ClsRevitUtil.GetParameterElementId( doc, instBase.Id, "集計レベル" ) ) ;
          //rotationAxis = Line.CreateBound(ptSt, ptSt + referencePlane.Normal);
          //normal = referencePlane.Normal;
          rPlane = true ;
        }
        else {
          createdId = ClsRevitUtil.Create( doc, ptSt, levelId, kouzaiSym ) ;
          // シンボルを回転
          double radians =
            XYZ.BasisX.AngleOnPlaneTo( vec, normal ) ; //inst.HandOrientation.AngleOnPlaneTo(vec, normal);
          ElementTransformUtils.RotateElement( doc, createdId, rotationAxis, radians ) ;
        }

        m_CreatedId = createdId ;
        FamilyInstance inst = doc.GetElement( createdId ) as FamilyInstance ;
        //// シンボルを回転
        //double radians = inst.HandOrientation.AngleOnPlaneTo(vec, normal);
        //ElementTransformUtils.RotateElement(doc, createdId, rotationAxis, radians);

        if ( isKussaku ) {
          if ( rPlane ) {
            baseoffset = ClsRevitUtil.GetParameterDouble( doc, createdId, "ホストからのオフセット" ) ;
            ClsRevitUtil.SetParameter( doc, createdId, "ホストからのオフセット",
              baseoffset - kouzaiSizeHalf - ClsRevitUtil.CovertToAPI( coverPlateThickness ) ) ;
          }
          else if ( baseName.Contains( "切梁" ) || baseName.Contains( "隅火打" ) ) {
            if ( ! ClsRevitUtil.SetParameter( doc, createdId, "基準レベルからの高さ",
                  baseoffset - kouzaiSizeHalf - ClsRevitUtil.CovertToAPI( ( coverPlateThickness ) ) ) )
              ClsRevitUtil.SetParameter( doc, createdId, "ホストからのオフセット",
                baseoffset - kouzaiSizeHalf - ClsRevitUtil.CovertToAPI( coverPlateThickness ) ) ; // / 2)));
          }
          else if ( baseName.Contains( "腹起" ) ) {
            bool isLeft = RevitUtil.ClsGeo.IsLeft( vec, vecOffset ) ;
            if ( isLeft ) {
              ElementTransformUtils.MoveElement( doc, createdId,
                ( kouzaiSizeHalf + ( ClsRevitUtil.CovertToAPI( coverPlateThickness ) ) ) * -vecOffset ) ;
            }
            else {
              ElementTransformUtils.MoveElement( doc, createdId, ( kouzaiSizeHalf ) * -vecOffset ) ;
            }

            ClsRevitUtil.SetParameter( doc, createdId, "基準レベルからの高さ", baseoffset ) ;
          }
        }
        else {
          if ( rPlane ) {
            baseoffset = ClsRevitUtil.GetParameterDouble( doc, createdId, "ホストからのオフセット" ) ;
            ClsRevitUtil.SetParameter( doc, createdId, "ホストからのオフセット", baseoffset + kouzaiSizeHalf ) ;
          }
          else if ( baseName.Contains( "切梁" ) || baseName.Contains( "隅火打" ) ) {
            if ( ! ClsRevitUtil.SetParameter( doc, createdId, "基準レベルからの高さ",
                  baseoffset + kouzaiSizeHalf ) ) // + ClsRevitUtil.CovertToAPI((coverPlateThickness / 2)));
              ClsRevitUtil.SetParameter( doc, createdId, "ホストからのオフセット", baseoffset + kouzaiSizeHalf ) ;
          }
          else if ( baseName.Contains( "腹起" ) ) {
            bool isLeft = RevitUtil.ClsGeo.IsLeft( vec, vecOffset ) ;
            if ( isLeft ) {
              ElementTransformUtils.MoveElement( doc, createdId, ( kouzaiSizeHalf ) * vecOffset ) ;
            }
            else {
              ElementTransformUtils.MoveElement( doc, createdId,
                ( kouzaiSizeHalf + ( ClsRevitUtil.CovertToAPI( coverPlateThickness ) ) ) * vecOffset ) ;
            }

            ClsRevitUtil.SetParameter( doc, createdId, "基準レベルからの高さ", baseoffset ) ;
          }
        }

        t.Commit() ;
      }

      return true ;
    }

    public static List<ElementId> GetAllCoverplate( Document doc )
    {
      List<ElementId> ids = new List<ElementId>() ;
      //部品名を取得
      List<string> kariKouzaiList = new List<string>() ;
      kariKouzaiList.AddRange( Master.ClsCoverPlateCSV.GetFamilyNameList().ToList() ) ;

      //図面上の端部部品を全て取得
      List<ElementId> elements = ClsRevitUtil.GetAllCreatedFamilyInstanceList( doc ) ;
      List<ElementId> targetFamilies = new List<ElementId>() ;
      foreach ( string name in kariKouzaiList ) {
        foreach ( ElementId elem in elements ) {
          FamilyInstance inst = doc.GetElement( elem ) as FamilyInstance ;
          if ( inst != null && inst.Symbol.FamilyName == name ) //ファミリ名でフィルター
          {
            targetFamilies.Add( elem ) ;
          }
        }
      }

      return targetFamilies ;
    }

    #endregion
  }
}