using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using Autodesk.Revit.UI.Selection ;
using Fresco.Geometory ;
using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.Drawing.Drawing2D ;
using System.IO ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;
using YMS_gantry.Data ;
using YMS_gantry.Master ;
using YMS_gantry.UI ;
using YMS_gantry.UI.FrnCreateSlopeControls ;
using static YMS_gantry.DefineUtil ;
using static YMS_gantry.UI.FrmCreateBraceAndJointViewModel ;

namespace YMS_gantry
{
  public class CmdPutHorizontalTsunagi
  {
    static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger() ;
    Document _doc { get ; set ; }
    UIDocument _uiDoc { get ; set ; }

    public CmdPutHorizontalTsunagi( UIDocument uiDoc )
    {
      _uiDoc = uiDoc ;
      _doc = _uiDoc.Document ;
    }

    public Result Excute()
    {
      //var uiapp = commandData.Application;
      var uidoc = _uiDoc ;
      var doc = uidoc.Document ;

      //var form = new FrmPutHorizontalTsunagi();
      var form = Application.thisApp.frmPutHorizontalTsunagi ;
      var modelData = ModelData.GetModelData( doc ) ;
      foreach ( var x in modelData.ListKoudaiData.Select( x => x.AllKoudaiFlatData ) )
        form.ViewModel.KodaiSet.Add( x ) ;
      form.ViewModel.RevitDoc = doc ;
      //if (form.ShowDialog() != System.Windows.Forms.DialogResult.OK) { return Result.Cancelled; }

      var vm = form.ViewModel ;
      var kodaiData = vm.SelectedKodai ;
      var selection = uidoc.Selection ;

      if ( vm.PlaceIsMultiple ) {
        // 端点をピックするためのビューへ切り替え
        var view3d = new FilteredElementCollector( doc ).OfClass( typeof( View3D ) ).Cast<View3D>()
          .FirstOrDefault( x => ! x.Name.Contains( "CASE" ) ) ;
        if ( view3d != null ) {
          // 3D ビューがあれば 3D ビューを上から見た状態にする
          var viewOrientation = view3d.GetOrientation() ;
          var toBottomOrientation = new ViewOrientation3D( XYZ.Zero, XYZ.BasisY, -XYZ.BasisZ ) ;
          view3d.SetOrientation( toBottomOrientation ) ;
          view3d.OrientTo( -XYZ.BasisZ ) ;
          uidoc.ActiveView = view3d ;
        }
        else {
          // 3D ビューが無ければ基準レベルの平面図ビューにする
          var levelName = kodaiData.SelectedLevel ;
          var levelView = RevitUtil.ClsRevitUtil.getView( doc, levelName, GantryUtil.getViewType( doc, levelName ) ) ;
          if ( levelView != null ) {
            uidoc.ActiveView = levelView ;
          }
        }
        //uidoc.ActiveView = new FilteredElementCollector(doc)
        //       .OfClass(typeof(ViewPlan))
        //       .Cast<ViewPlan>()
        //       .FirstOrDefault(x => x.Name.ToLower() != "gl" && !x.IsTemplate);

        // 段インデックスと参照面のペア
        var refPlanes = new ReferencePlane[ vm.DanRowSet.Count ] ;

        // 構台の基準レベル
        var kodaiBaseLevel = new FilteredElementCollector( doc ).OfClass( typeof( Level ) ).Cast<Level>()
          .FirstOrDefault( x => x.Name == kodaiData.SelectedLevel ) ;
        // 構台の基準 Z
        var kodaiBaseElev = kodaiBaseLevel.ProjectElevation + kodaiData.LevelOffset.ToFt() ;

        // 最高大引下端の取得
        var highestOhbikiBottomFt = MaterialSuper.Collect( doc ).Where( x => x.m_KodaiName == kodaiData.KoudaiName )
          .Select( x => x as Ohbiki ).Where( x => x != null ).Select( x =>
            ClsRevitUtil.GetParameterDouble( doc, x.m_ElementId, DefineUtil.PARAM_BASE_OFFSET ) -
            0.5 * ( x.SteelSize?.HeightFeet ?? 0.0 ) ).Max() ;

        using ( var transaction = new Transaction( doc, Guid.NewGuid().ToString() ) ) {
          transaction.Start() ;
          var originalGraphicStyle = view3d.get_Parameter( BuiltInParameter.MODEL_GRAPHICS_STYLE ).AsInteger() ;
          using ( var defer = new Defer(
                   () => { view3d.get_Parameter( BuiltInParameter.MODEL_GRAPHICS_STYLE ).Set( 1 ) ; },
                   () =>
                   {
                     view3d.get_Parameter( BuiltInParameter.MODEL_GRAPHICS_STYLE ).Set( originalGraphicStyle ) ;
                   } ) ) {
            while ( true ) {
              try {
                // 端点.1 を選択
                var picked1 = selection.PickPoint( statusPrompt: "1 点目の端点を選択" ) ;

                // 端点.2 を選択
                XYZ picked2 ;
                do {
                  picked2 = selection.PickPoint( statusPrompt: "2 点目の端点を選択" ) ;
                } while ( UtilAlgebra.Eq( picked1.DistanceTo( picked2 ), 0.0 ) ) ;

                var picked1to2 = picked2 - picked1 ;

                // つなぎ材の向きを選択
                XYZ thirdPt ;
                do {
                  thirdPt = selection.PickPoint( statusPrompt: "つなぎ材の配置方向を指定" ) ;
                } while ( UtilAlgebra.Eq( picked1to2.CrossProduct( thirdPt - picked1 ).Z, 0.0 ) ) ;

                // 突き出し量の反映
                var unit1to2 = picked1to2.Normalize() ;
                var picked1Extend = picked1 - vm.HrzJointStartFt * unit1to2 ;
                var picked2Extend = picked2 + vm.HrzJointEndFt * unit1to2 ;

                // つなぎ材の向きからファミリの始終点を算出
                XYZ startXY ;
                XYZ endXY ;
                if ( picked1to2.CrossProduct( thirdPt - picked1 ).Z > 0.0 ) {
                  startXY = picked1Extend ;
                  endXY = picked2Extend ;
                }
                else {
                  startXY = picked2Extend ;
                  endXY = picked1Extend ;
                }

                //L材のときのみ start end を入れ替える必要あり
                if ( vm.HrzJointSize is ClsAngleCsv ) {
                  var tmp = endXY ;
                  endXY = startXY ;
                  startXY = tmp ;
                }
                else if ( vm.HrzJointSize is ClsHBeamCsv hbeamRec ) {
                  var start2end = ( endXY - startXY ).Normalize() ;
                  var orth = -start2end.Y * XYZ.BasisX + start2end.X * XYZ.BasisY ;
                  var hbeamWidth = 0.5 * hbeamRec.HFt ;
                  startXY += hbeamWidth * orth ;
                  endXY += hbeamWidth * orth ;
                }

                //using (var transaction = new Transaction(doc, Guid.NewGuid().ToString()))
                //{
                //    transaction.Start();

                // ファミリロード
                var familyPath = Directory.EnumerateFiles( PathUtil.GetYMSFolder(),
                  Path.GetFileName( vm.HrzJointSize.FamilyPath ), SearchOption.AllDirectories ).FirstOrDefault() ;
                var familyName = Path.GetFileNameWithoutExtension( familyPath ) ;
                var familyTypeName = vm.HrzJointSize.HrzJoint ;
                if ( GantryUtil.GetFamilySymbol( doc, familyPath, familyTypeName, out var symbol, true ) ) {
                  var danData = vm.DanRowSet ;

                  for ( int indexDan = 0 ; indexDan < danData.Count ; indexDan++ ) {
                    var dan = danData.ElementAtOrDefault( indexDan ) ;
                    if ( dan == null ) {
                      continue ;
                    }

                    if ( ! dan.HasJoint ) {
                      continue ;
                    }

                    var index = dan.Index - 1 ;

                    // 対応する参照面を取得 (無ければ作成)
                    var refPlane = refPlanes.ElementAtOrDefault( indexDan ) ;
                    var levelFromTopFt = -danData.Take( indexDan + 1 ).Select( x => x.IntervalFt ).Sum() ;
                    var levelOffsetFt = kodaiBaseLevel.ProjectElevation + highestOhbikiBottomFt + levelFromTopFt ;
                    if ( refPlane == null ) {
                      // 参照面の作成
                      var elevationOrigin = levelOffsetFt * XYZ.BasisZ ;
                      refPlane = doc.Create.NewReferencePlane( elevationOrigin, elevationOrigin + XYZ.BasisY,
                        XYZ.BasisX, doc.ActiveView ) ;
                      refPlanes[ index ] = refPlane ;
                    }
                    //var levelDiff = 0.0;

                    var start = new XYZ( startXY.X, startXY.Y, levelOffsetFt ) ;
                    var end = new XYZ( endXY.X, endXY.Y, levelOffsetFt ) ;

                    var id = MaterialSuper.PlaceWithTwoPoints( symbol, refPlane.GetReference(), start, end, 0.0,
                      noOffset: true ) ;
                    if ( id == ElementId.InvalidElementId ) {
                      _logger.Error( "ツナギ材ファミリインスタンスの配置に失敗" ) ;
                      continue ;
                    }

                    ClsRevitUtil.SetParameter( doc, id, DefineUtil.PARAM_ROTATE, -0.5 * Math.PI ) ;
                    var material = new HorizontalJoint
                    {
                      m_KodaiName = kodaiData.KoudaiName,
                      m_Document = doc,
                      m_ElementId = id,
                      m_Size = vm.HrzJointSize.Size,
                      m_TeiketsuType = vm.HrzJointIsWelding ? DefineUtil.eJoinType.Welding :
                        vm.HrzJointIsBolt ? DefineUtil.eJoinType.Bolt : DefineUtil.eJoinType.Support
                    } ;
                    MaterialSuper.WriteToElement( material, doc ) ;
                  }
                }
                else {
                  _logger.Error( $"family:\"{familyName}\", type:\"{familyTypeName}\" cannot load" ) ;
                }
                //    transaction.Commit();
                //}
              }
              catch ( Autodesk.Revit.Exceptions.OperationCanceledException ) {
                break ;
              }
            }
          }

          transaction.Commit() ;
        }
      }
      else if ( vm.PlaceIsSingle ) {
        try {
          //単体配置処理
          var pickedRef = selection.PickObject( Autodesk.Revit.UI.Selection.ObjectType.Element, new FilterRefPlane(),
            "水平つなぎを配置する参照面を選択" ) ;
          var refPlaneElem = doc.GetElement( pickedRef.ElementId ) as ReferencePlane ;
          var refPlane = refPlaneElem.GetPlane() ;
          var view3d = new FilteredElementCollector( doc ).OfClass( typeof( View3D ) ).Cast<View3D>()
            .FirstOrDefault( x => ! x.Name.Contains( "CASE" ) ) ;

          // 3D ビューがあれば 3D ビューを上から見た状態にする
          var viewOrientation = view3d.GetOrientation() ;
          var toBottomOrientation = new ViewOrientation3D( XYZ.Zero, XYZ.BasisY, -XYZ.BasisZ ) ;
          view3d.SetOrientation( toBottomOrientation ) ;
          view3d.OrientTo( -XYZ.BasisZ ) ;
          //view3d.get_Parameter(BuiltInParameter.MODEL_GRAPHICS_STYLE).Set(1);
          uidoc.ActiveView = view3d ;

          using ( var transaction = new Transaction( doc, Guid.NewGuid().ToString() ) ) {
            transaction.Start() ;

            // 端点指定の時だけ Wireframe 表示に切り替え
            var originalGraphicStyle = view3d.get_Parameter( BuiltInParameter.MODEL_GRAPHICS_STYLE ).AsInteger() ;
            using ( var defer = new Defer(
                     () => { view3d.get_Parameter( BuiltInParameter.MODEL_GRAPHICS_STYLE ).Set( 1 ) ; },
                     () =>
                     {
                       view3d.get_Parameter( BuiltInParameter.MODEL_GRAPHICS_STYLE ).Set( originalGraphicStyle ) ;
                     } ) ) {
              while ( true ) {
                try {
                  // 端点.1 を選択
                  var picked1 = selection.PickPoint( statusPrompt: "1 点目の端点を選択" ) ;
                  picked1 = CalcPerp( refPlane, picked1, view3d.GetOrientation().ForwardDirection ) ;

                  // 端点.2 を選択
                  XYZ picked2 ;
                  do {
                    picked2 = selection.PickPoint( statusPrompt: "2 点目の端点を選択" ) ;
                    picked2 = CalcPerp( refPlane, picked2, view3d.GetOrientation().ForwardDirection ) ;
                  } while ( UtilAlgebra.Eq( picked1.DistanceTo( picked2 ), 0.0 ) ) ;

                  var picked1to2 = picked2 - picked1 ;

                  // つなぎ材の向きを選択
                  XYZ thirdPt ;
                  do {
                    thirdPt = selection.PickPoint( statusPrompt: "つなぎ材の配置方向を指定" ) ;
                    thirdPt = CalcPerp( refPlane, thirdPt, view3d.GetOrientation().ForwardDirection ) ;
                  } while ( UtilAlgebra.Eq( picked1to2.CrossProduct( thirdPt - picked1 ).Z, 0.0 ) ) ;

                  // 突き出し量の反映
                  var unit1to2 = picked1to2.Normalize() ;
                  var picked1Extend = picked1 - vm.HrzJointStartFt * unit1to2 ;
                  var picked2Extend = picked2 + vm.HrzJointEndFt * unit1to2 ;

                  // つなぎ材の向きからファミリの始終点を算出
                  XYZ startXY ;
                  XYZ endXY ;
                  if ( picked1to2.CrossProduct( thirdPt - picked1 ).Z > 0.0 ) {
                    startXY = picked1Extend ;
                    endXY = picked2Extend ;
                  }
                  else {
                    startXY = picked2Extend ;
                    endXY = picked1Extend ;
                  }

                  //doc.Create.NewModelCurve(Line.CreateBound(startXY, endXY), SketchPlane.Create(doc, refPlane));

                  // ファミリロード
                  var familyPath = Directory.EnumerateFiles( PathUtil.GetYMSFolder(),
                    Path.GetFileName( vm.HrzJointSize.FamilyPath ), SearchOption.AllDirectories ).FirstOrDefault() ;
                  var familyName = Path.GetFileNameWithoutExtension( familyPath ) ;
                  var familyTypeName = vm.HrzJointSize.HrzJoint ;
                  if ( GantryUtil.GetFamilySymbol( doc, familyPath, familyTypeName, out var symbol, true ) ) {
                    var id = MaterialSuper.PlaceWithTwoPoints( symbol, refPlaneElem.GetReference(), startXY, endXY, 0.0,
                      noOffset: true ) ;
                    if ( id == ElementId.InvalidElementId ) {
                      _logger.Error( "ツナギ材ファミリインスタンスの配置に失敗" ) ;
                      continue ;
                    }

                    ClsRevitUtil.SetParameter( doc, id, DefineUtil.PARAM_ROTATE, -0.5 * Math.PI ) ;
                    var material = new HorizontalJoint
                    {
                      m_KodaiName = kodaiData.KoudaiName,
                      m_Document = doc,
                      m_ElementId = id,
                      m_Size = vm.HrzJointSize.Size,
                      m_TeiketsuType = vm.HrzJointIsWelding ? DefineUtil.eJoinType.Welding :
                        vm.HrzJointIsBolt ? DefineUtil.eJoinType.Bolt : DefineUtil.eJoinType.Support
                    } ;
                    MaterialSuper.WriteToElement( material, doc ) ;
                    //var danData = vm.DanRowSet;

                    //for (int indexDan = 0; indexDan < danData.Count; indexDan++)
                    //{
                    //    var dan = danData.ElementAtOrDefault(indexDan);
                    //    if (dan == null) { continue; }
                    //    if (!dan.HasJoint) { continue; }

                    //    var index = dan.Index - 1;

                    //    // 対応する参照面を取得 (無ければ作成)
                    //    var refPlane = refPlanes.ElementAtOrDefault(indexDan);
                    //    var levelFromTopFt = -danData.Take(indexDan + 1).Select(x => x.IntervalFt).Sum();
                    //    if (refPlane == null)
                    //    {
                    //        // 参照面の作成
                    //        var elevationOrigin = (kodaiBaseElev + levelFromTopFt) * XYZ.BasisZ;
                    //        refPlane = doc.Create.NewReferencePlane(elevationOrigin, elevationOrigin + XYZ.BasisY, XYZ.BasisX, doc.ActiveView);
                    //        refPlanes[index] = refPlane;
                    //    }
                    //    //var levelDiff = 0.0;

                    //    var start = new XYZ(startXY.X, startXY.Y, kodaiBaseElev + levelFromTopFt);
                    //    var end = new XYZ(endXY.X, endXY.Y, kodaiBaseElev + levelFromTopFt);

                    //    var id = MaterialSuper.PlaceWithTwoPoints(symbol, refPlane.GetReference(), start, end, 0.0, noOffset: true);
                    //    if (id == ElementId.InvalidElementId)
                    //    {
                    //        _logger.Error("ツナギ材ファミリインスタンスの配置に失敗");
                    //        continue;
                    //    }
                    //    ClsRevitUtil.SetParameter(doc, id, DefineUtil.PARAM_ROTATE, -0.5 * Math.PI);
                    //    var material = new HorizontalJoint
                    //    {
                    //        m_KodaiName = kodaiData.KoudaiName,
                    //        m_Document = doc,
                    //        m_ElementId = id,
                    //        m_Size = vm.HrzJointSize.Size
                    //    };
                    //    MaterialSuper.WriteToElement(material, doc);
                    //}
                  }
                  else {
                    _logger.Error( $"family:\"{familyName}\", type:\"{familyTypeName}\" cannot load" ) ;
                  }
                }
                catch ( Autodesk.Revit.Exceptions.OperationCanceledException ) {
                  break ;
                }
              }
            }

            transaction.Commit() ;
          }
        }
        catch ( Autodesk.Revit.Exceptions.OperationCanceledException ) {
        }
      }

      return Result.Succeeded ;
    }

    /// <summary>
    /// pt を direction で plane へ射影した結果を求める。
    /// 不定の場合は null を返す
    /// </summary>
    /// <param name="plane"></param>
    /// <param name="pt"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    static XYZ CalcPerp( Plane plane, XYZ pt, XYZ direction )
    {
      var x = plane.XVec ;
      var y = plane.YVec ;
      var p = pt ;
      var z = plane.Origin ;
      var d = direction ;
      var matrix = new Matrix2d( new double[] { x.X, y.X, -d.X, x.Y, y.Y, -d.Y, x.Z, y.Z, -d.Z } ) ;
      var inv = matrix.Inverse() ;
      if ( inv == null ) {
        return null ;
      }

      var c = p - z ;
      var gamma = inv[ 2, 0 ] * c.X + inv[ 2, 1 ] * c.Y + inv[ 2, 2 ] * c.Z ;
      return p + gamma * d ;
    }

    class FilterRefPlane : ISelectionFilter
    {
      public bool AllowElement( Element elem )
      {
        if ( elem is ReferencePlane ) {
          return true ;
        }

        //else if (elem is Level) { return true; }
        return false ;
      }

      public bool AllowReference( Reference reference, XYZ position )
      {
        return true ;
      }
    }
  }
}