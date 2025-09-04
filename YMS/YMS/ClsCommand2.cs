using Autodesk.Revit.Attributes ;
using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using Autodesk.Revit.UI.Selection ;
using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using YMS.Parts ;

namespace YMS
{
  /// <summary>
  /// 斜梁ベースを腹起ベース間に配置
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class TestShabariBase : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      try {
        var uiapp = commandData.Application ;
        var uidoc = uiapp.ActiveUIDocument ;
        var doc = uidoc.Document ;

        var num = 2 ;
        var haraokoshiBase = new ElementId[ num ] ;
        var haraDir = new XYZ[ num ] ;
        var vertex = new XYZ[ num ] ;

        for ( int i = 0 ; i < num ; i++ ) {
          //uidoc.Selection.PickObject(ObjectType.PointOnElement);
          var hara = doc.GetElement( uidoc.Selection.PickObject( ObjectType.Element ) ) as FamilyInstance ;
          haraokoshiBase[ i ] = hara.Id ;
          var haraCoordinate = hara.GetTotalTransform() ;
          haraDir[ i ] = haraCoordinate.BasisY ;
          //var haraLength = ClsRevitUtil.GetParameterDouble(doc, hara.Id, "長さ");
          var haraStartPt = haraCoordinate.Origin ;
          //var haraEndPt = haraStartPt + haraLength * haraDir;
          var haraLine = ClsYMSUtil.GetBaseLine( doc, hara.Id ) ; // Line.CreateUnbound(haraStartPt, haraDir[i]);

          var pickedPt = uidoc.Selection.PickPoint() ;
          vertex[ i ] = SLibRevitReo.ClosestLineAndPoint( haraLine, pickedPt ) ;
        }

        // 斜梁ベースファミリの読込
        var symbolFolpath = ClsZumenInfo.GetYMSFolder() ;
        var shinfamily = System.IO.Path.Combine( symbolFolpath, "ベース関係", "斜梁ベース" + ".rfa" ) ;
        if ( ! ClsRevitUtil.LoadFamilySymbolData( doc, shinfamily, "斜梁ベース", out FamilySymbol sym ) ) {
          return Result.Failed ;
        }

        using ( var transaction = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
          transaction.Start() ;

          var tmpStPoint = vertex[ 0 ] ;
          var tmpEdPoint = vertex[ 1 ] ;
          var tmpThirdPoint = vertex[ 0 ] - haraDir[ 0 ] ;

          var slopePlane = ClsRevitUtil.CreateReferencePlane( doc, tmpStPoint, tmpEdPoint, tmpThirdPoint ) ;

          if ( ! sym.IsActive ) {
            sym.Activate() ;
          }

          var family = sym.Family ;
          //このタイプでないと参照面に配置できない
          if ( family.FamilyPlacementType == FamilyPlacementType.WorkPlaneBased ) {
            var reference = slopePlane.GetReference() ;
            var dir = ( tmpEdPoint - tmpStPoint ).Normalize() ;
            var instance = doc.Create.NewFamilyInstance( reference, tmpStPoint, dir, sym ) ;

            ClsRevitUtil.SetParameter( doc, instance.Id, "長さ", tmpStPoint.DistanceTo( tmpEdPoint ) ) ;
            ClsRevitUtil.SetParameter( doc, instance.Id, "鋼材サイズ", "35HA" ) ;

            var id = instance.Id ;
            var element = doc.GetElement( reference ) ;
            // ここで一回移動などをしないと Location が原点のまま返ってくる。
            ElementTransformUtils.MoveElement( doc, id, XYZ.Zero ) ;
            slopePlane.Name = "斜梁" + id.IntegerValue.ToString() ;
          }
          else {
            //作成出来ない
          }

          transaction.Commit() ;
        }
      }
      catch ( Exception ex ) {
        //#if DEBUG
        TaskDialog taskDialog = new TaskDialog( $"[{GetType().Name}] Error!!" )
        {
          TitleAutoPrefix = true,
          MainIcon = TaskDialogIcon.TaskDialogIconError,
          MainInstruction = ex.GetType().ToString().Split( '.' ).LastOrDefault(),
          MainContent = ex.Message,
          ExpandedContent = ex.StackTrace,
          CommonButtons = TaskDialogCommonButtons.Close,
        } ;
        taskDialog.Show() ;
        //#endif
        return Result.Failed ;
      }

      return Result.Succeeded ;
    }
  }

  /// <summary>
  /// 次の処理を一括で行うテストコマンド
  /// 1. 斜梁端部部品配置
  /// 2. 斜梁端部部品の角度調整
  /// 3. 斜梁ベースを端部部品の角度に合わせて調整
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  public class TestShabariIkkatsu : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      try {
        var uiapp = commandData.Application ;
        var uidoc = uiapp.ActiveUIDocument ;
        var doc = uidoc.Document ;

        var originalId = uidoc.Selection.PickObject( ObjectType.Element ).ElementId ;


        return Result.Succeeded ;
      }
      catch ( Exception ex ) {
        //#if DEBUG
        TaskDialog taskDialog = new TaskDialog( $"[{GetType().Name}] Error!!" )
        {
          TitleAutoPrefix = true,
          MainIcon = TaskDialogIcon.TaskDialogIconError,
          MainInstruction = ex.GetType().ToString().Split( '.' ).LastOrDefault(),
          MainContent = ex.Message,
          ExpandedContent = ex.StackTrace,
          CommonButtons = TaskDialogCommonButtons.Close,
        } ;
        taskDialog.Show() ;
        //#endif
        return Result.Failed ;
      }
    }
  }

  public static class ShabariMethods
  {
    /// <summary>
    /// 両端の斜梁ピースから斜梁仮鋼材ベースを作成
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="piece1"></param>
    /// <param name="piece2"></param>
    /// <returns>作成した斜梁ベースの Id</returns>
    public static ElementId CreateShabariBase( Document doc, ElementId piece1, ElementId piece2 )
    {
      ElementId resultId = null ;
      var keyCenter1 = "回転軸1" ;
      var keyCenter2 = "回転軸2" ;
      var keyEdge1 = "仮鋼材端点" ;
      var keyEdge2 = "仮鋼材端点2" ;
      var keyTheta0 = "θ0" ;
      var keyTheta1 = "θ1" ;
      var keyTheta2 = "θ2" ;
      var keyTheta3 = "設置角度" ;

      var num = 2 ;
      var pieceIds = new ElementId[] { piece1, piece2 } ;
      var pieceInstances = new FamilyInstance[ num ] ;
      var origin = new XYZ[ num ] ; //ファミリ基点
      var basisX = new XYZ[ num ] ; //ファミリ X基底
      var basisY = new XYZ[ num ] ; //ファミリ Y基底
      var basisZ = new XYZ[ num ] ; //ファミリ Z基底
      var center1Set = new XYZ[ num ] ; // 回転軸1 の座標
      var center2Set = new XYZ[ num ] ; // 回転軸2 の座標
      var edge1Set = new XYZ[ num ] ; // 仮鋼材端点 の座標
      var edge2Set = new XYZ[ num ] ; // 仮鋼材端点2 の座標
      var l3 = new double[ num ] ; // ファミリ基点から V3 への距離
      var l2 = new double[ num ] ; // V3 から回転軸2への距離
      var l1Set = new double[ num ] ; // 回転軸2から回転軸1への距離
      var l0Set = new double[ num ] ; // 回転軸1から仮鋼材端点への距離
      var v3 = new XYZ[ num ] ; //V3 : V2をZX平面へ射影した点
      var v2 = new XYZ[ num ] ;
      var v1 = new XYZ[ num ] ;
      var v0 = new XYZ[ num ] ;
      var e0 = new XYZ[ num ] ;
      //var v3ToOrigin = new XYZ[num];

      for ( int i = 0 ; i < num ; i++ ) {
        var pickedElem = doc.GetElement( pieceIds[ i ] ) ;
        var pickedInstance = pickedElem as FamilyInstance ;
        pieceInstances[ i ] = pickedInstance ;

        var instanceCoodinate = pickedInstance.GetTotalTransform() ;
        origin[ i ] = instanceCoodinate.Origin ;
        basisY[ i ] = instanceCoodinate.BasisY ;

        var hasTheta3 = ClsRevitUtil.FindParameter( doc, pickedInstance.Id, keyTheta3 ) ;
        var theta3 = hasTheta3 ? ClsRevitUtil.GetParameterDouble( doc, pickedInstance.Id, keyTheta3 ) : 0.0 ;
        var basisRotation = SMatrix3d.Rotation( -basisY[ i ], XYZ.Zero, theta3 ) ;

        basisX[ i ] = basisRotation * instanceCoodinate.BasisX ;
        basisZ[ i ] = basisRotation * instanceCoodinate.BasisZ ;

        center1Set[ i ] = ClsRevitUtil.GetPointSyabariTanten( doc, pickedInstance, keyCenter1 ) ;
        center2Set[ i ] = ClsRevitUtil.GetPointSyabariTanten( doc, pickedInstance, keyCenter2 ) ;

        var toZXplane =
          SMatrix3d.BasisTransform( Plane.CreateByOriginAndBasis( origin[ i ], basisZ[ i ], basisX[ i ] ) ) ;
        var fromZXplane = toZXplane.Inverse() ;
        var proj = fromZXplane * SMatrix3d.ProjXY * toZXplane ;
        v3[ i ] = proj * center2Set[ i ] ;

        var aaaa = center2Set[ i ] - origin[ i ] ;

        edge1Set[ i ] = ClsRevitUtil.GetPointSyabariTanten( doc, pickedInstance, keyEdge1 ) ;
        edge2Set[ i ] = ClsRevitUtil.GetPointSyabariTanten( doc, pickedInstance, keyEdge2 ) ;

        #if DEBUG
        var length3 = ClsRevitUtil.CovertFromAPI( origin[ i ].DistanceTo( v3[ i ] ) ) ;
        var length2 = ClsRevitUtil.CovertFromAPI( v3[ i ].DistanceTo( center2Set[ i ] ) ) ;
        var length1 = ClsRevitUtil.CovertFromAPI( center2Set[ i ].DistanceTo( center1Set[ i ] ) ) ;
        var length0 = ClsRevitUtil.CovertFromAPI( center1Set[ i ].DistanceTo( edge1Set[ i ] ) ) ;
        var w = ClsRevitUtil.CovertFromAPI( edge1Set[ i ].DistanceTo( edge2Set[ i ] ) ) ;
        #endif
        l3[ i ] = origin[ i ].DistanceTo( v3[ i ] ) ;
        l2[ i ] = v3[ i ].DistanceTo( center2Set[ i ] ) ;
        l1Set[ i ] = center2Set[ i ].DistanceTo( center1Set[ i ] ) ;
        l0Set[ i ] = center1Set[ i ].DistanceTo( edge1Set[ i ] ) ;

        //v3[i] = v3;
        v2[ i ] = v3[ i ] + l2[ i ] * basisY[ i ] ;
        v1[ i ] = v2[ i ] + l1Set[ i ] * basisY[ i ] ;
        v0[ i ] = v1[ i ] + l0Set[ i ] * basisY[ i ] ;

        var edgeDist = edge1Set[ i ].DistanceTo( edge2Set[ i ] ) ;
        e0[ i ] = v0[ i ] + /*l3Set[i] * */( origin[ i ] - v3[ i ] ) ; // - edgeDist * basisX[i];
      }

      // theta2 の算出
      var theta2Set = new double[ num ] ;
      var rotateTheta2 = new SMatrix3d[ num ] ;
      var v1prime = new XYZ[ num ] ;
      var basisYprime = new XYZ[ num ] ;
      var basisZprime = new XYZ[ num ] ;
      for ( int i = 0 ; i < num ; i++ ) {
        var buddy = ( i + 1 ) % num ;

        var pt = v2[ i ] ; // originSet[i] + l2Set[i] * basisYSet[i];
        var buddyPt = v2[ buddy ] ; // originSet[buddy] + l2Set[buddy] * basisYSet[buddy];

        var yzPlane = Plane.CreateByOriginAndBasis( XYZ.Zero, basisY[ i ], basisZ[ i ] ) ;
        //var yzPlane = Plane.CreateByNormalAndOrigin(-basisX[i], XYZ.Zero);
        var projection = SMatrix3d.ProjXY * SMatrix3d.BasisTransform( yzPlane ) ;

        var p1 = projection * pt ;
        var p2 = projection * buddyPt ;

        var originToV3_1 = projection * ( v3[ i ] - origin[ i ] ) ;
        var originToV3_2 = projection * ( v3[ buddy ] - origin[ buddy ] ) ;

        var h = Math.Abs( p1.Y - p2.Y ) ;
        var l = Math.Abs( p1.X - p2.X ) ;
        var w1 = -originToV3_1.Y ; // l3[i];
        var w2 = originToV3_2.Y ; //l3[buddy];
        var a = w1 + w2 ;

        //var ans = UtilAlgebra.SolveQuadraticEquation(
        //    h * h + l * l,
        //    2.0 * a * h,
        //    a * a - l * l);

        var cos = UtilAlgebra.SolveQuadraticEquation( h * h + l * l, 2.0 * a * h, a * a - l * l )
          .Where( x => ClsGeo.GEO_EQ( x.Imaginary, 0.0 ) ).Select( x => x.Real )
          .FirstOrDefault( x => ClsGeo.GEO_LE( 0, x ) || ClsGeo.GEO_LE( x, 1.0 ) ) ;

        var theta_ = Math.Acos( cos ) ;
        var theta0 = p2.Y < p1.Y ? theta_ : -theta_ ;

        #if DEBUG
        var vec2 = projection * ( buddyPt - pt ) ;
        var vec1 = projection * basisY[ i ] ;

        var theta1 = vec1.AngleOnPlaneTo( vec2, XYZ.BasisZ ) ;
        var diff = SLibRevitReo.PrincipalRadian( theta1 ) - SLibRevitReo.PrincipalRadian( theta0 ) ;
        #endif

        var theta = theta0 ;
        theta2Set[ i ] = SLibRevitReo.PrincipalRadian( theta ) ;
        rotateTheta2[ i ] = SMatrix3d.Rotation( -basisX[ i ], v2[ i ], theta2Set[ i ] ) ;
        v1prime[ i ] = rotateTheta2[ i ] * v1[ i ] ;
        var rotateBasis = SMatrix3d.Rotation( -basisX[ i ], XYZ.Zero, theta2Set[ i ] ) ;
        basisYprime[ i ] = rotateBasis * basisY[ i ] ;
        basisZprime[ i ] = rotateBasis * basisZ[ i ] ;
      }

      // theta1 の算出
      var theta1Set = new double[ num ] ;
      var rotateTheta1 = new SMatrix3d[ num ] ;
      var v0prime = new XYZ[ num ] ;
      var e0prime = new XYZ[ num ] ;
      var basisXprime = new XYZ[ num ] ;
      var basisYprime2 = new XYZ[ num ] ;
      for ( int i = 0 ; i < num ; i++ ) {
        var buddy = ( i + 1 ) % num ;

        var pt = v1prime[ i ] ;
        var buddyPt = v1prime[ buddy ] ;

        var xyPrimePlane = Plane.CreateByNormalAndOrigin( -basisZprime[ i ], XYZ.Zero ) ;
        var projection = SMatrix3d.ProjXY * SMatrix3d.BasisTransform( xyPrimePlane ) ;

        var vec2 = projection * ( buddyPt - pt ) ;
        var vec1 = projection * basisYprime[ i ] ;
        var theta = vec1.AngleOnPlaneTo( vec2, XYZ.BasisZ ) ;
        theta1Set[ i ] = SLibRevitReo.PrincipalRadian( theta ) ;
        rotateTheta1[ i ] = SMatrix3d.Rotation( -basisZprime[ i ], pt, theta1Set[ i ] ) * rotateTheta2[ i ] ;
        v0prime[ i ] = rotateTheta1[ i ] * v0[ i ] ;
        e0prime[ i ] = rotateTheta1[ i ] * e0[ i ] ;
        var rotateBasis = SMatrix3d.Rotation( -basisZprime[ i ], XYZ.Zero, theta1Set[ i ] ) ;
        basisYprime2[ i ] = rotateBasis * basisYprime[ i ] ;
        basisXprime[ i ] = rotateBasis * basisX[ i ] ;
      }

      // theta0 の算出
      var theta0Set = new double[ num ] ;
      var rotateTheta0 = new SMatrix3d[ num ] ;
      var basisXprime2 = new XYZ[ num ] ;
      for ( int i = 0 ; i < num ; i++ ) {
        var a = basisZprime[ i ] ;
        var b = basisZ[ i ] ;
        var radian = SLibRevitReo.PrincipalRadian( a.AngleOnPlaneTo( b, -basisYprime2[ i ] ) ) ;

        // theta0 パラメータが存在しないときは 0 として計算を進める
        var existsTheta0 = ClsRevitUtil.FindParameter( doc, pieceInstances[ i ].Id, keyTheta0 ) ;
        theta0Set[ i ] = existsTheta0 ? radian : 0.0 ;

        rotateTheta0[ i ] = SMatrix3d.Rotation( -basisYprime2[ i ], e0prime[ i ], theta0Set[ i ] ) * rotateTheta1[ i ] ;
        basisXprime2[ i ] = SMatrix3d.Rotation( -basisYprime2[ i ], XYZ.Zero, theta0Set[ i ] ) * basisXprime[ i ] ;
      }

      // 斜梁ベースファミリの読込
      var symbolFolpath = ClsZumenInfo.GetYMSFolder() ;
      var shinfamily = System.IO.Path.Combine( symbolFolpath, "ベース関係", "斜梁ベース" + ".rfa" ) ;
      if ( ! ClsRevitUtil.LoadFamilySymbolData( doc, shinfamily, "斜梁ベース", out FamilySymbol sym ) ) {
        return default ;
      }

      using ( var transaction = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        transaction.Start() ;
        for ( int i = 0 ; i < num ; i++ ) {
          ClsRevitUtil.SetParameter( doc, pieceInstances[ i ].Id, keyTheta2,
            ClsYMSUtil.CheckSyabariAngle( theta2Set[ i ] ) ) ;
          ClsRevitUtil.SetParameter( doc, pieceInstances[ i ].Id, keyTheta1,
            ClsYMSUtil.CheckSyabariAngle( theta1Set[ i ] ) ) ;
          ClsRevitUtil.SetParameter( doc, pieceInstances[ i ].Id, keyTheta0,
            ClsYMSUtil.CheckSyabariAngle( theta0Set[ i ] ) ) ;
        }

        var tmpStPoint = e0prime[ 0 ] ;
        var tmpEdPoint = e0prime[ 1 ] ;
        var tmpThirdPoint = e0prime[ 0 ] - basisXprime2[ 0 ] ;

        var slopePlane = ClsRevitUtil.CreateReferencePlane( doc, tmpStPoint, tmpEdPoint, tmpThirdPoint ) ;

        if ( ! sym.IsActive ) {
          sym.Activate() ;
        }

        try {
          var family = sym.Family ;
          //このタイプでないと参照面に配置できない
          if ( family.FamilyPlacementType == FamilyPlacementType.WorkPlaneBased ) {
            var reference = slopePlane.GetReference() ;
            var dir = ( tmpEdPoint - tmpStPoint ).Normalize() ;
            var instance = doc.Create.NewFamilyInstance( reference, tmpStPoint, dir, sym ) ; //tmpEdPoint - tmpStPoint

            ClsRevitUtil.SetParameter( doc, instance.Id, "長さ", tmpStPoint.DistanceTo( tmpEdPoint ) ) ;
            ClsRevitUtil.SetParameter( doc, instance.Id, "鋼材サイズ",
              "35HA" ) ; // ClsCommonUtils.GetOnlyNumberInStringReturnString(name1) + "HA");//サイズは斜梁受ピース名から取得するが高強度などを使用するかは不明

            var id = instance.Id ;
            var element = doc.GetElement( reference ) ;
            // ここで一回移動などをしないと Location が原点のまま返ってくる。
            ElementTransformUtils.MoveElement( doc, id, XYZ.Zero ) ;
            slopePlane.Name = "斜梁" + id.IntegerValue.ToString() ;
            resultId = id ;
          }
          else {
            //作成出来ない
          }
        }
        catch ( Exception ex ) {
          TaskDialog.Show( "TEST4", ex.Message ) ;
        }

        transaction.Commit() ;
      }

      return resultId ;
      //}
      //catch (Exception ex)
      //{
      //    //#if DEBUG
      //    TaskDialog taskDialog = new TaskDialog("TEST3")
      //    {
      //        TitleAutoPrefix = true,
      //        MainIcon = TaskDialogIcon.TaskDialogIconError,
      //        MainInstruction = ex.GetType().ToString().Split('.').LastOrDefault(),
      //        MainContent = ex.Message,
      //        ExpandedContent = ex.StackTrace,
      //        CommonButtons = TaskDialogCommonButtons.Close,
      //    };
      //    taskDialog.Show();
      //    //#endif
      //    return Result.Failed;
      //}
    }

    ///// <summary>
    ///// 斜梁ベースから両端のピースと斜梁仮鋼材ベースを作成
    ///// </summary>
    ///// <param name="doc"></param>
    ///// <param name="originalBaseId"></param>
    ///// <returns></returns>
    //public static (ElementId baseId, ElementId piece1, ElementId piece2) ReplaceShabariBase(Document doc, ElementId originalBaseId)
    //{
    //    //斜梁ベースを選択
    //    var shaBase = doc.GetElement(originalBaseId) as FamilyInstance;
    //    var shaBaseLine = ClsYMSUtil.GetBaseLine(doc, shaBase.Id);
    //    var shaBaseMid = SLibRevitReo.MidPoint(shaBaseLine.GetEndPoint(0), shaBaseLine.GetEndPoint(1));

    //    var shabariClass = new ClsSyabariBase();
    //    shabariClass.SetClassParameter(doc, shaBase.Id);
    //    var num = 2;
    //    var buhinType = new string[]
    //    {
    //            shabariClass.m_buhinTypeStart,
    //            shabariClass.m_buhinTypeEnd,
    //    };
    //    var buhinSize = new string[]
    //    {
    //            shabariClass.m_buhinSizeStart,
    //            shabariClass.m_buhinSizeEnd,
    //    };
    //    var sym = new FamilySymbol[num];

    //    // 斜梁ピースファミリの読込
    //    var symbolFolpath = ClsZumenInfo.GetYMSFolder();
    //    for (int i = 0; i < num; i++)
    //    {
    //        var shinfamily = Master.ClsSyabariPieceCSV.GetFamilyPath(buhinSize[i]);
    //        //var shinfamily = System.IO.Path.Combine(symbolFolpath, "支保工関係", buhinType[i], $"{buhinSize[i]}.rfa");
    //        var symbolName = System.IO.Path.GetFileNameWithoutExtension(shinfamily);// string.IsNullOrWhiteSpace(buhinSize[i]) ? buhinType[i] : buhinSize[i];
    //        if (!ClsRevitUtil.LoadFamilySymbolData(doc, shinfamily, symbolName, out FamilySymbol sym1))
    //        {
    //            return default;
    //        }
    //        sym[i] = sym1;
    //    }

    //    var haraPtSet = ClsHaraokoshiBase.GetIntersectionBase2(doc, shaBase.Id);
    //    if (haraPtSet.Count != 2)
    //    {
    //        return default;
    //    }

    //    //var aaa = ClsHaraokoshiBase.GetIntersectionBase2(doc, shaBase.Id);

    //    // 両方の端部に斜梁ピースを配置
    //    var pieceIds = new ElementId[num];
    //    using (var transaction = new Transaction(doc, Guid.NewGuid().ToString()))
    //    {
    //        transaction.Start();
    //        for (int i = 0; i < num; i++)
    //        {
    //            var buddy = (i + 1) % num;
    //            var pt = shaBaseLine.GetEndPoint(i);
    //            var buddyPt = shaBaseLine.GetEndPoint(buddy);
    //            var haraPt = haraPtSet.FirstOrDefault(x => ClsGeo.GEO_EQ0(x.Point.DistanceTo(pt)));
    //            if (haraPt == null) { continue; }

    //            if (!sym[i].IsActive) { sym[i].Activate(); }
    //            //var pt = haraPt.Point;
    //            var toMid = shaBaseMid - haraPt.Point;
    //            var haraLine = ClsYMSUtil.GetBaseLine(doc, haraPt.Id);
    //            var haraOrthSystem = Plane.CreateByOriginAndBasis(XYZ.Zero, XYZ.BasisZ, haraLine.Direction.Normalize());
    //            var m = SMatrix3d.BasisTransform(haraOrthSystem);
    //            var a = m.Inverse() * SMatrix3d.ProjZ * m;
    //            var haraOrthDir = a * toMid;
    //            var tanbuDir = SMatrix3d.Rotation(-XYZ.BasisZ, XYZ.Zero, 0.5 * Math.PI) * haraOrthDir;

    //            var tanbuPlane = ClsRevitUtil.CreateReferencePlane(doc, pt, pt + XYZ.BasisX, pt + XYZ.BasisY);
    //            //tanbuPlane.Name = $"斜梁端部{tanbuPlane.Id}";
    //            //tanbuPlane.Maximize3DExtents();
    //            var reference = tanbuPlane.GetReference();
    //            var instance = doc.Create.NewFamilyInstance(reference, pt, tanbuDir.Normalize(), sym[i]);
    //            if (ClsGeo.GEO_LT(pt.Z, buddyPt.Z))
    //            {
    //                ClsRevitUtil.SetParameter(doc, instance.Id, "設置角度", Math.PI);
    //            }


    //            pieceIds[i] = instance.Id;
    //        }
    //        transaction.Commit();
    //    }

    //    var baseId = ShabariMethods.CreateShabariBase(doc, pieceIds.ElementAtOrDefault(0), pieceIds.ElementAtOrDefault(1));
    //    return (baseId, pieceIds[0], pieceIds[1]);
    //}
  }

  [Transaction( TransactionMode.Manual )]
  public class TestShabariHiuchi : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      var uiapp = commandData.Application ;
      var uidoc = uiapp.ActiveUIDocument ;
      var doc = uidoc.Document ;

      var haraId = ElementId.InvalidElementId ;
      var shaBaseId = ElementId.InvalidElementId ;

      var hiuchiLength = 5e3 ;
      FamilySymbol haraBuhin = null ;
      FamilySymbol shaBuhin = null ;

      var shaBaseRef = uidoc.Selection.PickObject( ObjectType.Element, "斜梁ベースを選択" ) ;
      var haraBaseRef = uidoc.Selection.PickObject( ObjectType.PointOnElement, "腹起ベースを選択" ) ;


      return Result.Succeeded ;
    }
  }

  public static class SHabariHiuchiMethods
  {
    public static void CreateShabariHiuchi( Document doc, ElementId shaBaseId, ElementId haraBaseId, XYZ haraSelectedPt,
      double hiuchiLength, FamilySymbol shaPiece, FamilySymbol haraPiece )
    {
      var shaBaseLine = ClsYMSUtil.GetBaseLine( doc, shaBaseId ) ;
      var shaBaseElem = doc.GetElement( shaBaseId ) as FamilyInstance ;
      var shaBaseHost = shaBaseElem.Host as ReferencePlane ;
      var shaBasePlane = shaBaseHost.GetPlane() ;

      var haraBaseLine = ClsYMSUtil.GetBaseLine( doc, haraBaseId ) ;
      var haraSection = Plane.CreateByNormalAndOrigin( normal: haraBaseLine.Direction.CrossProduct( XYZ.BasisZ ),
        origin: haraBaseLine.Origin ) ;

      var p = SLibRevitReo.IntersectPlaneAndLine( haraSection, shaBaseLine ) ;
      var haraLine = SLibRevitReo.IntersectPlaneAndPlane( shaBasePlane, haraSection ) ;
    }
  }
}