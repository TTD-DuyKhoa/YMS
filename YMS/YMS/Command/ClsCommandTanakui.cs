using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using Autodesk.Revit.UI.Selection ;
using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;
using YMS.DLG ;
using YMS.Parts ;

namespace YMS.Command
{
  class ClsCommandTanakui
  {
    /// <summary>
    /// 棚杭・中間杭作成
    /// </summary>
    /// <param name="uidoc"></param>
    public static void CommandCreateTanakui( UIDocument uidoc )
    {
      Document doc = uidoc.Document ;

      DlgCreateTanakui dlgCreateTanakui = new DlgCreateTanakui( doc ) ;
      DialogResult result = dlgCreateTanakui.ShowDialog() ;
      if ( result != DialogResult.OK ) {
        return ;
      }

      ClsTanakui clsKui = dlgCreateTanakui.m_ClsKui ;

      string levelName = dlgCreateTanakui.m_Level ;
      ElementId levelID = ClsRevitUtil.GetLevelID( doc, levelName ) ;

      while ( true ) {
        try {
          string uniqueName = string.Empty ;

          List<XYZ> insertPoints = new List<XYZ>() ;
          switch ( clsKui.m_CreateType ) {
            case (ClsTanakui.CreateType) ClsSanbashiKui.CreateType.Intersection :
              List<ElementId> ids = new List<ElementId>() ;
              ClsRevitUtil.PickObjectsPartFilter( uidoc, "切梁ベースを選択してください", "切梁ベース", ref ids ) ;
              GetIntersectionPoints( uidoc, insertPoints, ids ) ;
              break ;
            case (ClsTanakui.CreateType) ClsSanbashiKui.CreateType.OnePoint :
              Selection selection = uidoc.Selection ;
              XYZ selectedPoint = selection.PickPoint( "挿入点を指定してください" ) ;
              insertPoints.Add( selectedPoint ) ;
              break ;
            default :
              return ;
          }

          // XとYが重複する座標を除外する
          //insertPoints = insertPoints.GroupBy(p => new { p.X, p.Y }).SelectMany(g => g.Skip(1)).ToList();
          List<XYZ> uniqueInsertPoints = insertPoints
            .GroupBy( p => new { x = ClsGeo.RoundOff( p.X, 4 ), y = ClsGeo.RoundOff( p.Y, 4 ) } ) // X軸とY軸が同じ要素でグループ化
            .Select( g => g.First() ) // 各グループから最初の要素を選択
            .ToList() ; // リストに変換

          if ( uniqueInsertPoints.Count == 0 ) {
            break ;
          }

          bool renzoku = false ;
          int cnt = 1 ;
          foreach ( XYZ point in uniqueInsertPoints ) {
            if ( cnt > 1 ) {
              renzoku = true ;
            }

            clsKui.CreateTanaKui( doc, point, levelID, renzoku, ref uniqueName ) ;
            cnt++ ;

            // 切梁ブラケットも同時に作成する場合
            if ( dlgCreateTanakui.m_ClsKui.m_CreateKiribariBracket ) {
              ClsBracket clsBracket = new ClsBracket() ;
              clsBracket.m_BracketSize = dlgCreateTanakui.m_ClsKui.m_KiribariBracketSize ;
              clsBracket.m_TargetKuiAngle = dlgCreateTanakui.m_ClsKui.m_PileHaichiAngle ;
              clsBracket.m_TargetKuiCreatePosition = dlgCreateTanakui.m_ClsKui.m_CreatePosition ;
              clsBracket.m_TargetKuiCreateType = dlgCreateTanakui.m_ClsKui.m_CreateType ;
              if ( clsBracket.CreateKiribariBracket( doc, clsKui.m_CreateId,
                    dlgCreateTanakui.m_ClsKui.m_KiribariBracketSizeIsAuto ) ) {
                // プレロードガイド材も作成する場合
                if ( dlgCreateTanakui.m_ClsKui.m_CreatePreloadGuide ) {
                  for ( int i = 0 ; i < clsBracket.m_CreateIds.Count() ; i++ ) {
                    clsBracket.CreatePreloadGuideZai( doc, clsBracket.m_CreateIds[ i ],
                      clsBracket.m_TargetKiribariBaseIds[ i ] ) ;
                  }
                }
              }
            }

            // 切梁押えブラケットも同時に作成する場合
            if ( dlgCreateTanakui.m_ClsKui.m_CreateKiribariOsaeBracket ) {
              ClsBracket clsBracket = new ClsBracket() ;
              clsBracket.m_BracketSize = dlgCreateTanakui.m_ClsKui.m_KiribariOsaeBracketSize ;
              clsBracket.m_TargetKuiAngle = dlgCreateTanakui.m_ClsKui.m_PileHaichiAngle ;
              clsBracket.m_TargetKuiCreatePosition = dlgCreateTanakui.m_ClsKui.m_CreatePosition ;
              clsBracket.m_TargetKuiCreateType = dlgCreateTanakui.m_ClsKui.m_CreateType ;
              if ( clsBracket.CreateKiribariOsaeBracket( doc, clsKui.m_CreateId,
                    dlgCreateTanakui.m_ClsKui.m_KiribariOsaeBracketSizeIsAuto ) ) {
                // ガイド材も作成する場合
                if ( dlgCreateTanakui.m_ClsKui.m_CreateGuide ) {
                  for ( int i = 0 ; i < clsBracket.m_CreateIds.Count() ; i++ ) {
                    clsBracket.CreateGuideZai( doc, clsBracket.m_CreateIds[ i ],
                      clsBracket.m_TargetKiribariBaseIds[ i ] ) ;
                  }
                }
              }
            }
          }
        }
        catch ( Exception ) {
          break ;
        }
      }
    }

    /// <summary>
    /// 棚杭・中間杭作成
    /// </summary>
    /// <param name="uidoc"></param>
    public static void CommandChangeTanakui( UIDocument uidoc )
    {
      Document doc = uidoc.Document ;

      List<ElementId> ids = new List<ElementId>() ;
      if ( ! ClsTanakui.PickObjects( uidoc, ref ids ) ) {
        return ;
      }

      //部材が未選択であれば処理を処理を終了
      if ( ids.Count == 0 ) {
        return ;
      }

      ClsTanakui tmp = new ClsTanakui() ;
      for ( int i = 0 ; i < ids.Count() ; i++ ) {
        ElementId id = ids[ i ] ;
        if ( i == 0 ) {
          tmp.SetParameter( doc, id ) ;
        }
        else {
          FamilyInstance instKui = doc.GetElement( id ) as FamilyInstance ;
          if ( instKui == null ) {
            return ;
          }

          ClsTanakui trgKui = new ClsTanakui() ;
          trgKui.SetParameter( doc, id ) ;

          tmp.m_IsFromGL = ClsRevitUtil.CompareValues( trgKui.m_IsFromGL, tmp.m_IsFromGL ) ;
          tmp.m_HightFromGL = ClsRevitUtil.CompareValues( trgKui.m_HightFromGL, tmp.m_HightFromGL ) ;
          tmp.m_CreateType = ClsRevitUtil.CompareValues( trgKui.m_CreateType, tmp.m_CreateType ) ;
          tmp.m_CreatePosition = ClsRevitUtil.CompareValues( trgKui.m_CreatePosition, tmp.m_CreatePosition ) ;
          tmp.m_ZureryouX = ClsRevitUtil.CompareValues( trgKui.m_ZureryouX, tmp.m_ZureryouX ) ;
          tmp.m_ZureryouY = ClsRevitUtil.CompareValues( trgKui.m_ZureryouY, tmp.m_ZureryouY ) ;
          tmp.m_KouzaiType = ClsRevitUtil.CompareValues( trgKui.m_KouzaiType, tmp.m_KouzaiType ) ;
          tmp.m_KouzaiSize = ClsRevitUtil.CompareValues( trgKui.m_KouzaiSize, tmp.m_KouzaiSize ) ;
          tmp.m_PileTotalLength = ClsRevitUtil.CompareValues( trgKui.m_PileTotalLength, tmp.m_PileTotalLength ) ;
          tmp.m_PileHaichiAngle = ClsRevitUtil.CompareValues( trgKui.m_PileHaichiAngle, tmp.m_PileHaichiAngle ) ;
          tmp.m_Zanti = ClsRevitUtil.CompareValues( trgKui.m_Zanti, tmp.m_Zanti ) ;
          tmp.m_ZantiLength = ClsRevitUtil.CompareValues( trgKui.m_ZantiLength, tmp.m_ZantiLength ) ;
          tmp.m_CreateKiribariBracket =
            ClsRevitUtil.CompareValues( trgKui.m_CreateKiribariBracket, tmp.m_CreateKiribariBracket ) ;
          tmp.m_CreatePreloadGuide =
            ClsRevitUtil.CompareValues( trgKui.m_CreatePreloadGuide, tmp.m_CreatePreloadGuide ) ;
          tmp.m_CreateKiribariOsaeBracket =
            ClsRevitUtil.CompareValues( trgKui.m_CreateKiribariOsaeBracket, tmp.m_CreateKiribariOsaeBracket ) ;
          tmp.m_CreateGuide = ClsRevitUtil.CompareValues( trgKui.m_CreateGuide, tmp.m_CreateGuide ) ;
          tmp.m_TsugiteCount = ClsRevitUtil.CompareValues( trgKui.m_TsugiteCount, tmp.m_TsugiteCount ) ;
          tmp.m_FixingType = ClsRevitUtil.CompareValues( trgKui.m_FixingType, tmp.m_FixingType ) ;
          tmp.m_PileLengthList = ClsRevitUtil.CompareValues( trgKui.m_PileLengthList, tmp.m_PileLengthList ) ;
          tmp.m_TsugitePlateSize_Out =
            ClsRevitUtil.CompareValues( trgKui.m_TsugitePlateSize_Out, tmp.m_TsugitePlateSize_Out ) ;
          tmp.m_TsugitePlateQuantity_Out =
            ClsRevitUtil.CompareValues( trgKui.m_TsugitePlateQuantity_Out, tmp.m_TsugitePlateQuantity_Out ) ;
          tmp.m_TsugitePlateSize_In =
            ClsRevitUtil.CompareValues( trgKui.m_TsugitePlateSize_In, tmp.m_TsugitePlateSize_In ) ;
          tmp.m_TsugitePlateQuantity_In =
            ClsRevitUtil.CompareValues( trgKui.m_TsugitePlateQuantity_In, tmp.m_TsugitePlateQuantity_In ) ;
          tmp.m_TsugitePlateSize_Web1 =
            ClsRevitUtil.CompareValues( trgKui.m_TsugitePlateSize_Web1, tmp.m_TsugitePlateSize_Web1 ) ;
          tmp.m_TsugitePlateQuantity_Web1 =
            ClsRevitUtil.CompareValues( trgKui.m_TsugitePlateQuantity_Web1, tmp.m_TsugitePlateQuantity_Web1 ) ;
          tmp.m_TsugitePlateSize_Web2 =
            ClsRevitUtil.CompareValues( trgKui.m_TsugitePlateSize_Web2, tmp.m_TsugitePlateSize_Web2 ) ;
          tmp.m_TsugitePlateQuantity_Web2 =
            ClsRevitUtil.CompareValues( trgKui.m_TsugitePlateQuantity_Web2, tmp.m_TsugitePlateQuantity_Web2 ) ;
          tmp.m_TsugiteBoltSize_Flange =
            ClsRevitUtil.CompareValues( trgKui.m_TsugiteBoltSize_Flange, tmp.m_TsugiteBoltSize_Flange ) ;
          tmp.m_TsugiteBoltQuantity_Flange = ClsRevitUtil.CompareValues( trgKui.m_TsugiteBoltQuantity_Flange,
            tmp.m_TsugiteBoltQuantity_Flange ) ;
          tmp.m_TsugiteBoltSize_Web =
            ClsRevitUtil.CompareValues( trgKui.m_TsugiteBoltSize_Web, tmp.m_TsugiteBoltSize_Web ) ;
          tmp.m_TsugiteBoltQuantity_Web =
            ClsRevitUtil.CompareValues( trgKui.m_TsugiteBoltQuantity_Web, tmp.m_TsugiteBoltQuantity_Web ) ;
        }
      }

      string levelName = ClsRevitUtil.GetInstMojiParameter( doc, ids[ 0 ], "集計レベル" ) ;
      DlgCreateTanakui dlgCreateTanakui = new DlgCreateTanakui( doc, tmp, levelName ) ;
      DialogResult result = dlgCreateTanakui.ShowDialog() ;
      if ( result != DialogResult.OK ) {
        return ;
      }

      bool renzoku = false ;
      int cnt = 1 ;
      string uniqueName = string.Empty ;
      foreach ( var id in ids ) {
        if ( cnt > 1 ) {
          renzoku = true ;
        }

        ClsTanakui clsTanakui = dlgCreateTanakui.m_ClsKui ;
        FamilyInstance instKui = doc.GetElement( id ) as FamilyInstance ;
        XYZ point = ( instKui.Location as LocationPoint ).Point ;
        levelName = dlgCreateTanakui.m_Level ;
        ElementId levelId = ClsRevitUtil.GetLevelID( doc, levelName ) ;

        using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
          t.Start() ;
          ClsRevitUtil.Delete( doc, id ) ;
          t.Commit() ;
        }

        clsTanakui.m_CreateType = ClsTanakui.CreateType.OnePoint ; //変更時の再作図では「1点」で作図する
        clsTanakui.CreateTanaKui( doc, point, levelId, renzoku, ref uniqueName ) ;

        cnt++ ;
      }
    }

    private static void GetIntersectionPoints( UIDocument uidoc, List<XYZ> insertPoints, List<ElementId> ids )
    {
      FilteredElementCollector collector = new FilteredElementCollector( uidoc.Document ) ;
      List<Curve> curves = new List<Curve>() ;

      foreach ( ElementId id in ids ) {
        Element element = collector.WhereElementIsNotElementType().Where( x => x.Id == id ).FirstOrDefault() ;

        if ( element is CurveElement curveElement ) {
          curves.Add( curveElement.GeometryCurve ) ;
        }
        else if ( element is FamilyInstance familyInstance ) {
          Location location = familyInstance.Location ;

          if ( location is LocationCurve locationCurve ) {
            curves.Add( locationCurve.Curve ) ;
          }
        }
      }

      // 交点の取得
      for ( int i = 0 ; i < curves.Count - 1 ; i++ ) {
        for ( int j = i + 1 ; j < curves.Count ; j++ ) {
          if ( curves[ i ].Intersect( curves[ j ] ) is SetComparisonResult res && res == SetComparisonResult.Overlap ) {
            IntersectionResultArray intersectionPointsArray ;
            curves[ i ].Intersect( curves[ j ], out intersectionPointsArray ) ;

            foreach ( IntersectionResult intersectionResult in intersectionPointsArray ) {
              XYZ intersectionPoint = intersectionResult.XYZPoint ;
              insertPoints.Add( intersectionPoint ) ;
            }
          }
        }
      }
    }
  }
}