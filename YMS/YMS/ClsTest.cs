using Autodesk.Revit.DB ;
using Autodesk.Revit.DB.Structure ;
using Autodesk.Revit.UI ;
using Autodesk.Revit.UI.Selection ;
using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.Drawing ;
using System.IO ;
using System.Linq ;
using System.Net ;
using System.Runtime.InteropServices ;
using System.Security.Cryptography ;
using System.Text ;
using System.Windows.Forms ;
using System.Windows.Media ;
using YMS.Parts ;

namespace YMS
{
  class ClsTest
  {
    /// <summary>
    /// 配置レベル
    /// </summary>
    public string m_level { get ; set ; }

    public ClsTest()
    {
      //初期化
      Init() ;
    }

    public void Init()
    {
      m_level = string.Empty ;
    }

    private static bool CreateVoid1( Document doc )
    {
      //try to create a new sketch plane
      XYZ newNormal = new XYZ( 1, 1, 0 ) ; // the normal vector
      XYZ newOrigin = new XYZ( 0, 0, 0 ) ; // the origin point
      // create geometry plane
      //Plane geometryPlane = application.Application.Create.NewPlane(newNormal, newOrigin);

      Autodesk.Revit.DB.View v = getView( doc, "レベル 1", ViewType.FloorPlan ) ;
      SketchPlane sketchPlan = v.SketchPlane ;

      // create sketch plane
      //SketchPlane sketchPlane = SketchPlane.Create(application.ActiveUIDocument.Document, geometryPlane);

      CreateExtrusion( doc, sketchPlan ) ;

      return true ;
    }

    private static Extrusion CreateExtrusion( Autodesk.Revit.DB.Document document, SketchPlane sketchPlane )
    {
      Extrusion rectExtrusion = null ;

      // make sure we have a family document
      if ( true != document.IsFamilyDocument ) {
        // define the profile for the extrusion
        CurveArrArray curveArrArray = new CurveArrArray() ;
        CurveArray curveArray1 = new CurveArray() ;
        CurveArray curveArray2 = new CurveArray() ;
        CurveArray curveArray3 = new CurveArray() ;

        // create a rectangular profile
        XYZ p0 = XYZ.Zero ;
        XYZ p1 = new XYZ( 10, 0, 0 ) ;
        XYZ p2 = new XYZ( 10, 10, 0 ) ;
        XYZ p3 = new XYZ( 0, 10, 0 ) ;
        Line line1 = Line.CreateBound( p0, p1 ) ;
        Line line2 = Line.CreateBound( p1, p2 ) ;
        Line line3 = Line.CreateBound( p2, p3 ) ;
        Line line4 = Line.CreateBound( p3, p0 ) ;
        curveArray1.Append( line1 ) ;
        curveArray1.Append( line2 ) ;
        curveArray1.Append( line3 ) ;
        curveArray1.Append( line4 ) ;

        curveArrArray.Append( curveArray1 ) ;

        // create solid rectangular extrusion
        try {
          rectExtrusion = document.FamilyCreate.NewExtrusion( true, curveArrArray, sketchPlane, 10 ) ;
        }
        catch ( Exception e ) {
          return null ;
        }


        if ( null != rectExtrusion ) {
          // move extrusion to proper place
          XYZ transPoint1 = new XYZ( -16, 0, 0 ) ;
          ElementTransformUtils.MoveElement( document, rectExtrusion.Id, transPoint1 ) ;
        }
        else {
          throw new Exception( "Create new Extrusion failed." ) ;
        }
      }
      else {
        throw new Exception( "Please open a Family document before invoking this command." ) ;
      }

      return rectExtrusion ;
    }

    public static bool testCommand11( UIDocument uidoc )
    {
      Document doc = uidoc.Document ;

      List<XYZ> polygonVertices = new List<XYZ>() ;
      XYZ p1 = new XYZ( 0, 0, 0 ) ;
      XYZ p2 = new XYZ( 50, 150, 0 ) ;
      XYZ p3 = new XYZ( 100, 100, 0 ) ;
      XYZ p4 = new XYZ( 20, 50, 0 ) ;
      XYZ p5 = new XYZ( 200, -50, 0 ) ;
      XYZ p6 = new XYZ( 30, -150, 0 ) ;
      polygonVertices.Add( p1 ) ;
      polygonVertices.Add( p2 ) ;
      polygonVertices.Add( p3 ) ;
      polygonVertices.Add( p4 ) ;
      polygonVertices.Add( p5 ) ;
      polygonVertices.Add( p6 ) ;

      List<ElementId> lineIds = new List<ElementId>() ;

      using ( Transaction tx = new Transaction( doc, "Create Box" ) ) {
        tx.Start() ;
        Line ln1 = Line.CreateBound( p1, p2 ) ;
        XYZ mid1 = 0.5 * ( p1 + p2 ) ;
        Plane plane1 = Plane.CreateByNormalAndOrigin( XYZ.BasisZ, mid1 ) ;
        SketchPlane sketchPlane1 = SketchPlane.Create( doc, plane1 ) ;
        ModelLine modelLine1 = doc.Create.NewModelCurve( ln1, sketchPlane1 ) as ModelLine ;
        lineIds.Add( modelLine1.Id ) ;

        Line ln2 = Line.CreateBound( p2, p3 ) ;
        XYZ mid2 = 0.5 * ( p2 + p3 ) ;
        Plane plane2 = Plane.CreateByNormalAndOrigin( XYZ.BasisZ, mid2 ) ;
        SketchPlane sketchPlane2 = SketchPlane.Create( doc, plane2 ) ;
        ModelLine modelLine2 = doc.Create.NewModelCurve( ln2, sketchPlane2 ) as ModelLine ;
        lineIds.Add( modelLine2.Id ) ;

        Line ln3 = Line.CreateBound( p3, p4 ) ;
        XYZ mid3 = 0.5 * ( p3 + p4 ) ;
        Plane plane3 = Plane.CreateByNormalAndOrigin( XYZ.BasisZ, mid3 ) ;
        SketchPlane sketchPlane3 = SketchPlane.Create( doc, plane3 ) ;
        ModelLine modelLine3 = doc.Create.NewModelCurve( ln3, sketchPlane3 ) as ModelLine ;
        lineIds.Add( modelLine3.Id ) ;


        Line ln4 = Line.CreateBound( p4, p5 ) ;
        XYZ mid4 = 0.5 * ( p4 + p5 ) ;
        Plane plane4 = Plane.CreateByNormalAndOrigin( XYZ.BasisZ, mid4 ) ;
        SketchPlane sketchPlane4 = SketchPlane.Create( doc, plane4 ) ;
        ModelLine modelLine4 = doc.Create.NewModelCurve( ln4, sketchPlane4 ) as ModelLine ;
        lineIds.Add( modelLine4.Id ) ;

        Line ln5 = Line.CreateBound( p5, p6 ) ;
        XYZ mid5 = 0.5 * ( p5 + p6 ) ;
        Plane plane5 = Plane.CreateByNormalAndOrigin( XYZ.BasisZ, mid5 ) ;
        SketchPlane sketchPlane5 = SketchPlane.Create( doc, plane5 ) ;
        ModelLine modelLine5 = doc.Create.NewModelCurve( ln5, sketchPlane5 ) as ModelLine ;
        lineIds.Add( modelLine5.Id ) ;

        Line ln6 = Line.CreateBound( p6, p1 ) ;
        XYZ mid6 = 0.5 * ( p6 + p1 ) ;
        Plane plane6 = Plane.CreateByNormalAndOrigin( XYZ.BasisZ, mid6 ) ;
        SketchPlane sketchPlane6 = SketchPlane.Create( doc, plane6 ) ;
        ModelLine modelLine6 = doc.Create.NewModelCurve( ln6, sketchPlane6 ) as ModelLine ;
        lineIds.Add( modelLine6.Id ) ;

        tx.Commit() ;
      }

      for ( ; ; ) {
        Selection selection = uidoc.Selection ;
        XYZ pnt = selection.PickPoint( "位置を指定してください" ) ;
        if ( pnt == null ) {
          break ;
        }
        // XYZ pntnew = new XYZ(pnt.X, pnt.Y, 0);

        // ポリゴンとテストポイントの関係を確認
        bool windingNumber = ClsRevitUtil.IsInArea( polygonVertices, pnt, XYZ.BasisZ ) ;

        // 結果を出力
        if ( windingNumber ) {
          MessageBox.Show( "内" ) ;
        }
        else {
          MessageBox.Show( "外" ) ;
        }
      }

      using ( Transaction tx = new Transaction( doc, "Create Box" ) ) {
        tx.Start() ;
        doc.Delete( lineIds ) ;
        tx.Commit() ;
      }


      return true ;
    }

    public static bool testCommand3( /*UIDocument uidoc, AddInId activeId*/ )
    {
      DLG.DlgKarikouzaiSuryo dlg = new DLG.DlgKarikouzaiSuryo() ;

      if ( dlg.ShowDialog() != DialogResult.OK ) {
        return false ;
      }

      //Document doc = uidoc.Document;

      //string pathA = "支保工関係\\10_自在火打受ﾋﾟｰｽ\\自在火打ち受けピース_25FVP.rfa";
      //string symbolFolpath = ClsZumenInfo.GetYMSFolder();
      //string path = System.IO.Path.Combine(symbolFolpath, pathA);
      //string familyName = RevitUtil.ClsRevitUtil.GetFamilyName(path);
      //string strSymbolType = "切梁";


      //if (!ClsRevitUtil.RoadFamilyData(doc, path, familyName, out Family fam))
      //{
      //    return false;
      //}
      //FamilySymbol sym = (ClsRevitUtil.GetFamilySymbol(doc, familyName, strSymbolType));


      //try
      //{
      //    //PromptForFamilyInstancePlacementOptions opt = new PromptForFamilyInstancePlacementOptions();
      //    //opt.FaceBasedPlacementType = FaceBasedPlacementType.PlaceOnFace;
      //    ////opt.FaceBasedPlacementType = FaceBasedPlacementType.PlaceOnWorkPlane;

      //    //opt.SketchGalleryOptions = SketchGalleryOptions.SGO_Default;
      //    //uidoc.PromptForFamilyInstancePlacement(sym, opt);


      //    //uidoc.PromptForFamilyInstancePlacement(sym);

      //    // ElementType et = ClsRevitUtil.GetElementType(doc, sym, strSymbolType);


      //    //--------------------------------------------------------------------->>>

      //    DLG.DlgKobetsuHaichi_Jizai dlg = new DLG.DlgKobetsuHaichi_Jizai();

      //    if (dlg.ShowDialog() != DialogResult.OK)
      //    {
      //        return false;
      //    }

      //    bool left = dlg.m_isLeft;
      //    DLG.DlgKobetsuHaichi_Jizai.EnumPtn ptn = dlg.m_ptn;

      //    if (ptn == DLG.DlgKobetsuHaichi_Jizai.EnumPtn.typeA)
      //    {
      //        using (Transaction tx = new Transaction(doc, "haiti"))
      //        {
      //            tx.Start();
      //            Selection selection = uidoc.Selection;
      //            Reference pic = selection.PickObject(ObjectType.Element, "腹起を選択してください");
      //            Element ele = doc.GetElement(pic);
      //            Face face = ele.GetGeometryObjectFromReference(pic) as Face;
      //            XYZ pnt = selection.PickPoint("配置位置を指定してください");


      //            ElementId CreatedID = ClsRevitUtil.Create(doc, pnt, face, sym, new XYZ(0, 0, 1));
      //            tx.Commit();
      //        }

      //    }
      //    else if (ptn == DLG.DlgKobetsuHaichi_Jizai.EnumPtn.typeB)
      //    {
      //        //Updater01 updater01 = new Updater01(doc, activeId, ptn,true, left);
      //        ElementType et = doc.GetElement(sym.Id) as ElementType;


      //        uidoc.PostRequestForElementTypePlacement(et);

      //    }
      //    else if (ptn == DLG.DlgKobetsuHaichi_Jizai.EnumPtn.typeC)
      //    {
      //        //Updater01 updater01 = new Updater01(doc, activeId, ptn, true, left);
      //        ElementType et = doc.GetElement(sym.Id) as ElementType;


      //        uidoc.PostRequestForElementTypePlacement(et);

      //    }


      //    //---------------------------------------------------------------------<<<

      //    // MessageBox.Show("test");

      //}
      //catch
      //{

      //}


      return true ;
    }

    public static bool testCommand4( UIDocument uidoc )
    {
      Document doc = uidoc.Document ;

      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        Selection selection = uidoc.Selection ;

        List<ElementId> ids = new List<ElementId>() ;

        //bool sel0 = true;
        XYZ maeP = new XYZ() ;

        Reference picShin = selection.PickObject( ObjectType.Element, "芯を選択してください" ) ;

        Element selShin = GetSelectLine( doc, picShin ) ;

        if ( selShin == null ) {
          return false ;
        }

        ModelLine mdLine = selShin as ModelLine ;
        Curve geoCurve = mdLine.GeometryCurve ;
        Line geoLine = geoCurve as Line ;
        XYZ geoDirection = geoLine.Direction ; //芯のvector
        //double length = geoLine.Length;//芯の長さ

        //芯位置作成
        Curve cvShin = ( selShin.Location as LocationCurve ).Curve ;
        XYZ p = cvShin.GetEndPoint( 0 ) ; //芯作成位置（始点）

        ElementId levelID = GetLevelID( doc, "レベル 1" ) ;
        Autodesk.Revit.DB.View v = getView( doc, "レベル 1", ViewType.FloorPlan ) ;
        Autodesk.Revit.DB.View v3 = getView( doc, "{3D}", ViewType.ThreeD ) ;
        Autodesk.Revit.DB.View vActv = doc.ActiveView ;
        double vSize = 1.0 ;
        if ( vActv.Id == v3.Id ) {
          vSize = 1.0 ;
        }

        if ( vActv.Id == v.Id ) {
          vSize = 0.3 ;
        }

        //単位ベクトルの符号
        int vSignX = Math.Sign( geoDirection.X ) ;
        int vSignY = Math.Sign( geoDirection.Y ) ;
        int vSignZ = Math.Sign( geoDirection.Z ) ;

        for ( ; ; ) {
          try {
            //if (!sel0)
            //{
            //    p = selection.PickPoint("位置を選択！");
            //    XYZ nearPoint = new XYZ((p.X - maeP.X) * geoDirection.X, (p.Y - maeP.Y) * geoDirection.Y, 0);//改善の余地あり
            //    double nearPointLength = nearPoint.GetLength();

            //    int len = (int)(nearPointLength / ClsRevitUtil.CovertToAPI(500));
            //    p = new XYZ(maeP.X + (ClsRevitUtil.CovertToAPI(len * 500) * geoDirection.X),
            //                maeP.Y + (ClsRevitUtil.CovertToAPI(len * 500) * geoDirection.Y),
            //                maeP.Z);

            //    Curve cv0 = Line.CreateBound(maeP, p);

            //    FamilySymbol sym = GetFamilySymbol(doc, "H形鋼200ｘ200", "切梁");
            //    if (sym == null)
            //    {
            //        return false;
            //    }
            //    t.Start();
            //    ElementId CreatedID = Create(doc, cv0, levelID, sym);
            //    t.Commit();

            //    maeP = p;
            //}
            //else
            //{
            //    maeP = p;
            //    sel0 = false;

            //}

            t.Start() ;

            maeP = p ;

            foreach ( ElementId id in ids ) {
              doc.Delete( id ) ;
            }

            ids = new List<ElementId>() ;


            for ( int i = 1 ; i < 71 ; i++ ) {
              XYZ newPoint = new XYZ( p.X + ( ClsRevitUtil.CovertToAPI( i * 100 ) * geoDirection.X ),
                p.Y + ( ClsRevitUtil.CovertToAPI( i * 100 ) * geoDirection.Y ),
                p.Z + ( ClsRevitUtil.CovertToAPI( i * 100 ) * geoDirection.Z ) ) ;
              XYZ bubbleEnd = new XYZ( newPoint.X, newPoint.Y, newPoint.Z ) ;
              //これなら動く
              XYZ freeEnd ;
              if ( Math.Abs( geoDirection.Y ) < Math.Abs( geoDirection.X ) ) {
                vSignY = 0 ;
                freeEnd = new XYZ( newPoint.X, 0, newPoint.Z ) ;
              }
              else {
                vSignX = 0 ;
                freeEnd = new XYZ( 0, newPoint.Y, newPoint.Z ) ;
              }

              Curve cv ;
              if ( i % 10 == 0 ) {
                cv = Line.CreateBound(
                  new XYZ( newPoint.X + ( 3 * vSignY ), newPoint.Y + ( 3 * vSignX ), newPoint.Z + ( 3 * vSignZ ) ),
                  new XYZ( newPoint.X - ( 3 * vSignY ), newPoint.Y - ( 3 * vSignX ), newPoint.Z - ( 3 * vSignZ ) ) ) ;
                ElementId defaultTypeId = doc.GetDefaultElementTypeId( ElementTypeGroup.TextNoteType ) ;

                TextNoteOptions opts = new TextNoteOptions( defaultTypeId ) ;
                //opts.HorizontalAlignment = HorizontalTextAlignment.Left;
                //opts.Rotation = Math.PI / 4;
                double maxWidth = TextNote.GetMaximumAllowedWidth( doc, defaultTypeId ) ;
                maxWidth = ClsRevitUtil.CovertToAPI( maxWidth ) ;
                TextNote tx = TextNote.Create( doc, vActv.Id,
                  new XYZ( newPoint.X - ( 0.56 * vSize ), newPoint.Y + ( 0.56 * vSize ), newPoint.Z ), maxWidth,
                  ( i / 10.0 ).ToString() + ".0", opts ) ;
                TextElementType textType = tx.Symbol ;
                BuiltInParameter paraIndex = BuiltInParameter.TEXT_SIZE ;
                Parameter textSize = textType.get_Parameter( paraIndex ) ;

                textSize.Set( maxWidth / ( 10.0 * vSize ) ) ;

                paraIndex = BuiltInParameter.TEXT_BACKGROUND ;
                Parameter textBack = textType.get_Parameter( paraIndex ) ;
                textBack.Set( 1 ) ; // 0 = Opaque（不透明） :: 1 = Transparent（透過）
                ids.Add( tx.Id ) ;
              }
              else if ( i % 5 == 0 ) {
                cv = Line.CreateBound(
                  new XYZ( newPoint.X + ( 2 * vSignY ), newPoint.Y + ( 2 * vSignX ), newPoint.Z + ( 2 * vSignZ ) ),
                  new XYZ( newPoint.X - ( 2 * vSignY ), newPoint.Y - ( 2 * vSignX ), newPoint.Z - ( 2 * vSignZ ) ) ) ;
              }
              else {
                cv = Line.CreateBound(
                  new XYZ( newPoint.X + ( 1 * vSignY ), newPoint.Y + ( 1 * vSignX ), newPoint.Z + ( 1 * vSignZ ) ),
                  new XYZ( newPoint.X - ( 1 * vSignY ), newPoint.Y - ( 1 * vSignX ), newPoint.Z - ( 1 * vSignZ ) ) ) ;
              }

              if ( vActv == null ) {
                return false ;
              }

              XYZ cutvec = new XYZ( 0, 0, newPoint.Z ) ;
              ReferencePlane plane = doc.Create.NewReferencePlane( bubbleEnd, freeEnd, cutvec, vActv ) ;
              SketchPlane skp = SketchPlane.Create( doc, plane.Id ) ;
              ModelCurve mc = doc.Create.NewModelCurve( cv, skp ) ;

              ids.Add( mc.Id ) ;
            }

            t.Commit() ;

            p = selection.PickPoint( "位置を選択！" ) ;
            XYZ nearPoint =
              new XYZ( ( p.X - maeP.X ) * geoDirection.X, ( p.Y - maeP.Y ) * geoDirection.Y, 0 ) ; //改善の余地あり
            double nearPointLength = nearPoint.GetLength() ;

            int len = (int) ( nearPointLength / ClsRevitUtil.CovertToAPI( 500 ) ) ;
            if ( 14 < len ) {
              len = 14 ;
            }

            p = new XYZ( maeP.X + ( ClsRevitUtil.CovertToAPI( len * 500 ) * geoDirection.X ),
              maeP.Y + ( ClsRevitUtil.CovertToAPI( len * 500 ) * geoDirection.Y ), maeP.Z ) ;

            Curve cv0 = Line.CreateBound( maeP, p ) ;

            FamilySymbol sym = GetFamilySymbol( doc, "H形鋼200ｘ200", "切梁" ) ;
            if ( sym == null ) {
              return false ;
            }

            t.Start() ;
            ElementId CreatedID = Create( doc, cv0, levelID, sym ) ;
            t.Commit() ;
          }
          catch ( OperationCanceledException e ) {
            break ;
          }
          catch ( Exception e ) {
            MessageBox.Show( e.Message ) ;
            foreach ( ElementId id in ids ) {
              t.Start() ;
              doc.Delete( id ) ;
              t.Commit() ;
            }

            break ;
          }
        }
      } //using

      return true ;
    }

    /// <summary>
    /// 交点にファミリを配置
    /// </summary>
    /// <param name="uidoc"></param>
    /// <returns></returns>
    public static bool CreateIntersectionFamily( UIDocument uidoc )
    {
      Document doc = uidoc.Document ;

      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        try {
          Selection selection = uidoc.Selection ;

          Reference picShin1 = selection.PickObject( ObjectType.Element, "1本目の芯を選択してください" ) ;

          Element selShin1 = GetSelectLine( doc, picShin1 ) ;

          if ( selShin1 == null ) {
            return false ;
          }

          Reference picShin2 = selection.PickObject( ObjectType.Element, "2本目の芯を選択してください" ) ;

          Element selShin2 = GetSelectLine( doc, picShin2 ) ;

          if ( selShin1 == null ) {
            return false ;
          }


          Curve cvShin1 = ( selShin1.Location as LocationCurve ).Curve ;
          Curve cvShin2 = ( selShin2.Location as LocationCurve ).Curve ;

          XYZ intersectionPoint = GetIntersection( cvShin1, cvShin2 ) ;
          if ( intersectionPoint == null ) {
            return false ;
          }

          //交点にソイル配置
          FamilySymbol sym = GetFamilySymbol( doc, "ソイル_3軸のみ_21", "ソイル_3軸のみ_21" ) ;
          if ( sym == null ) {
            return false ;
          }

          t.Start() ;
          ElementId levelID = GetLevelID( doc, "レベル 1" ) ;
          ElementId CreatedID = Create( doc, intersectionPoint, levelID, sym ) ;
          t.Commit() ;

          DialogResult dResult = MessageBox.Show( "位置をずらしますか？", "確認", MessageBoxButtons.YesNo ) ;
          if ( dResult == DialogResult.Yes ) {
            t.Start() ;
            doc.Delete( CreatedID ) ;
            XYZ rPoint = intersectionPoint + new XYZ( 10, 10, 0 ) ;
            ElementId CreatedIDRSet = Create( doc, rPoint, levelID, sym ) ;
            t.Commit() ;
          }
        }
        catch {
        }
      } //using

      return true ;
    } //CreateIntersectionFamily

    /// <summary>
    /// 選択した位置にシンボルを配置する
    /// </summary>
    /// <param name="uidoc"></param>
    /// <returns></returns>
    public static bool CreateSymbol( UIDocument uidoc, DLG.DlgCreateSMW SMW )
    {
      //    string symbolFolpath = ClsZumenInfo.GetFamilyFolder();

      //    Document doc = uidoc.Document;
      //    Autodesk.Revit.DB.View vActv = doc.ActiveView;
      //    View3D view3D = doc.ActiveView as View3D;

      //    string familyPathSoil = System.IO.Path.Combine(symbolFolpath,"山留壁ファミリ\\10_ｿｲﾙ\\ソイル_単軸.rfa");
      //    string familyNameSoil = "ソイル_単軸";

      //    string familyPathKui = System.IO.Path.Combine(symbolFolpath, "山留壁ファミリ\\06_杭\\ファミリ\\杭_H300x300x10x15.rfa");
      //    string familyNameKui = "杭_" + SMW.m_size;//"杭_H300x300x10x15";

      //    using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
      //    {
      //        try
      //        {
      //            Selection selection = uidoc.Selection;
      //            Line soilLine;
      //            Element selShin = null;
      //            XYZ picPointS = null;
      //            XYZ picPointE = null;
      //            if (SMW.m_way == 1)
      //            {
      //                //Reference picShin = selection.PickObject(ObjectType.Element, "芯を選択してください");//壁芯

      //                //selShin = GetSelectLine(doc, picShin);
      //                ElementId selShinId = null;
      //                if (!ClsRevitUtil.PickObject(uidoc, "壁芯", "壁芯", ref selShinId))
      //                {
      //                    return false;
      //                }
      //                //if (selShin == null)
      //                //{
      //                //    return false;
      //                //}
      //                selShin = doc.GetElement(selShinId);
      //                Curve cvShin = (selShin.Location as LocationCurve).Curve;
      //                picPointS = cvShin.GetEndPoint(0);
      //                picPointE = cvShin.GetEndPoint(1);

      //            }
      //            if (SMW.m_way == 2)
      //            {
      //                picPointS = selection.PickPoint(familyNameKui + "を配置する開始位置を選択してください");
      //                if (picPointS == null)
      //                {
      //                    return false;
      //                }
      //                picPointE = selection.PickPoint(familyNameKui + "を配置する終了位置を選択してください");
      //                if (picPointE == null)
      //                {
      //                    return false;
      //                }

      //            }

      //            //if (SMW.m_bTanbu)//保留
      //            //{
      //            //    //始点と終点を入れ替える
      //            //    XYZ e = picPointS;
      //            //    picPointS = picPointE;
      //            //    picPointE = e;
      //            //}
      //            soilLine = Line.CreateBound(picPointS, picPointE);

      //            XYZ soilDirection = soilLine.Direction;//単位ベクトル
      //            double length = ClsRevitUtil.CovertFromAPI(soilLine.Length);//芯の長さ


      //            //XYZ kussaku = selection.PickPoint("掘削側を指定してください");
      //            //XYZ mid = (picPointS + picPointE) / 2;
      //            ////kussaku = new XYZ(kussaku.X * soilDirection.Y, kussaku.Y * soilDirection.X, kussaku.Z);
      //            //if (ClsGeo.GEO_GT0(soilDirection.Y))//0より大きいか
      //            //{
      //            //    if (kussaku.X > mid.X)//(ClsGeo.GEO_GT0(kussaku.X, mid.X))
      //            //    {
      //            //        //SとE入れ替え
      //            //        XYZ p = picPointS;
      //            //        picPointS = picPointE;
      //            //        picPointE = p;
      //            //    }
      //            //}
      //            //if (ClsGeo.GEO_LT0(soilDirection.Y))//0より小さいか
      //            //{
      //            //    if (kussaku.X < mid.X)//(ClsGeo.GEO_LT0(kussaku.X, mid.X))
      //            //    {
      //            //        //SとE入れ替え
      //            //        XYZ p = picPointS;
      //            //        picPointS = picPointE;
      //            //        picPointE = p;
      //            //    }
      //            //}
      //            //if (ClsGeo.GEO_GT0(soilDirection.X))//0より大きいか
      //            //{
      //            //    if (kussaku.Y < mid.Y)//(ClsGeo.GEO_LT0(kussaku.Y, mid.Y))
      //            //    {
      //            //        //SとE入れ替え
      //            //        XYZ p = picPointS;
      //            //        picPointS = picPointE;
      //            //        picPointE = p;
      //            //    }
      //            //}
      //            //if (ClsGeo.GEO_LT0(soilDirection.X))//0より小さいか
      //            //{
      //            //    if (kussaku.Y > mid.Y)//(ClsGeo.GEO_GT0(kussaku.Y, mid.Y))
      //            //    {
      //            //        //SとE入れ替え
      //            //        XYZ p = picPointS;
      //            //        picPointS = picPointE;
      //            //        picPointE = p;
      //            //    }
      //            //}

      //            //soilDirection = Line.CreateBound(picPointS, picPointE).Direction;//単位ベクトル再取得

      //            //シンボル配置
      //            if (!RoadFamilySymbolData(doc, familyPathSoil, familyNameSoil, out FamilySymbol sym))
      //            {
      //                return false;
      //            }

      //            //親杭配置
      //            if (!RoadFamilyData(doc, familyPathKui, familyNameKui, out Family oyaFam))
      //            {
      //                return false;
      //            }
      //            FamilySymbol oya = GetFamilySymbol(doc, familyNameKui, "SMW");


      //            t.Start();
      //            FailureHandlingOptions failOpt = t.GetFailureHandlingOptions();
      //            failOpt.SetFailuresPreprocessor(new WarningSwallower());
      //            t.SetFailureHandlingOptions(failOpt);

      //            //ダイアログ指定の値をSET
      //            //ソイル
      //            SetTypeParameter(sym, ClsGlobal.m_length, ClsRevitUtil.CovertToAPI(SMW.m_soilLen));
      //            SetTypeParameter(sym, ClsGlobal.m_dia, ClsRevitUtil.CovertToAPI(SMW.m_dia));
      //            int refPDist = SMW.m_refPDist;
      //            int soil = SMW.m_soil;
      //            //基準点から指定の数値離す
      //            picPointS = new XYZ(picPointS.X + (ClsRevitUtil.CovertToAPI(refPDist) * soilDirection.X),
      //                                picPointS.Y + (ClsRevitUtil.CovertToAPI(refPDist) * soilDirection.Y),
      //                                picPointS.Z);
      //            length -= refPDist;
      //            //H形鋼
      //            SetTypeParameter(oya, ClsGlobal.m_length, ClsRevitUtil.CovertToAPI(SMW.m_HLen));


      //            //ビューの向き//ビューがどの向きを向いていようが作成方向は同じであるため固定値
      //            XYZ vecView = new XYZ(0, 1, 0);
      //            if(picPointS.X < picPointE.X)
      //            {
      //                vecView = new XYZ(0, -1, 0);
      //            }
      //            //角度//X軸がプラスの時回転しすぎてしまうので分岐が必要
      //            double dAngle = soilDirection.AngleTo(vecView);
      //            //法線
      //            XYZ vec = new XYZ(0, 0, 1);//部材の法線ベクトルはZ軸を基準にしている

      //            ElementId levelID = GetLevelID(doc, "GL");
      //            XYZ picPoint = null;
      //            //グループ化するIDを保持する変数の宣言(nullだとAdd出来ないため)
      //            ICollection<ElementId> selectionIds = selection.GetElementIds();
      //            List<ElementId> selId = new List<ElementId>();//ボイド用

      //            double dCount = length / soil;//ダイアログソイルピッチずつ親杭、ソイルを配置するため何個置けるか
      //            int x = 0;
      //            for (int i = 0; i < dCount; i++)
      //            {
      //                picPoint = new XYZ(picPointS.X + (ClsRevitUtil.CovertToAPI(soil * i) * soilDirection.X),
      //                                   picPointS.Y + (ClsRevitUtil.CovertToAPI(soil * i) * soilDirection.Y),
      //                                   picPointS.Z + (ClsRevitUtil.CovertToAPI(soil * i) * soilDirection.Z));

      //                //******ソイル作成*******//
      //                ElementId CreatedID = Create(doc, picPoint, levelID, sym);//ソイル作成
      //                SetParameter(doc, CreatedID, ClsGlobal.m_refLvTop, ClsRevitUtil.CovertToAPI(SMW.m_soilTop));
      //                selectionIds.Add(CreatedID);
      //                selId.Add(CreatedID);
      //                //******ソイル作成*******//

      //                //配置パターン
      //                x++;
      //                if (x == 3) x = 0;
      //                if (x == SMW.m_putPtnFlag) continue;


      //                ElementId CreatedOyaID = Create(doc, picPoint, levelID, oya);//親杭作成
      //                SetParameter(doc, CreatedOyaID, ClsGlobal.m_refLvTop, ClsRevitUtil.CovertToAPI(SMW.m_HTop));

      //                Line axis = Line.CreateBound(picPoint, picPoint + vec);
      //                ClsRevitUtil.RotateElement(doc, CreatedOyaID, axis, Math.PI / 2 + dAngle);
      //                selectionIds.Add(CreatedOyaID);

      //                ////交互対応
      //                int nKougo = (SMW.m_KougoFlg && (i % 2) == 1 ? 2 : 1);
      //                oya = ChangeTypeID(doc, oya, CreatedOyaID, "SMW" + nKougo.ToString());

      //                //ジョイント数
      //                int nKasho = 0;
      //                if (nKougo == 1) nKasho = SMW.m_Kasho1;
      //                else nKasho = SMW.m_Kasho2;

      //                SetTypeParameter(oya, "ジョイント数", nKasho);

      //                //杭長さ
      //                List<int> pileLengths = new List<int>();
      //                if (nKougo == 1) pileLengths = SMW.m_ListPileLength1;
      //                else pileLengths = SMW.m_ListPileLength2;

      //                for (int j = 0; j < pileLengths.Count; j++)
      //                {
      //                    int nPileLength = pileLengths[j];
      //                    SetTypeParameter(oya, "杭" + (j+1).ToString(), ClsRevitUtil.CovertToAPI(nPileLength));
      //                }
      //            }


      //            //*******壁作成*******//
      //            //Curve wallCV = Line.CreateBound(picPointS, picPoint);
      //            //Element wall = Wall.Create(doc, wallCV, levelID, false);//壁作成
      //            //selectionIds.Add(wall.Id);
      //            //length = 8000.0;//本来は親杭の高さから取得するべき
      //            ////パラメーターの設定処理
      //            //SetParameter(doc, wall.Id, "指定高さ", ClsRevitUtil.CovertToAPI(length));
      //            //SetParameter(doc, wall.Id, "基準レベル オフセット", ClsRevitUtil.CovertToAPI(-length));
      //            //*******壁作成*******//

      //            //補助線の作成
      //            double dH = ClsRevitUtil.CovertToAPI(150);//H鋼材によって150は変動(300/2)
      //            ModelLine outLine = CreateKabeHojyoLine(doc, picPointS, picPointE, dH, ClsRevitUtil.CovertToAPI(300));//外側
      //            ModelLine inLine = CreateKabeHojyoLine(doc, picPointS, picPointE, -dH, ClsRevitUtil.CovertToAPI(300));//内側

      //            //選択した線を削除して再度モデル線分を作成
      //            //if (selShin != null)
      //            //{
      //            //    doc.Delete(selShin.Id);
      //            //    ModelLine selLine = CreateKabeHojyoLine(doc, picPointS, picPointE, 0, 0);
      //            //}
      //            Group group = null;
      //            if (selectionIds.Count > 0)
      //            {
      //                // Group all selected elements
      //                //group = doc.Create.NewGroup(selectionIds);
      //            }
      //            //変更が加わるメッセージ原因不明
      //            t.Commit();

      //            //壁くり抜き処理
      //            if (SMW.m_bVoid) DoVoid(doc, selId, inLine);
      //        }
      //        catch (Exception ex)
      //        {
      //            TaskDialog.Show("Info Message", ex.Message);
      //            string message = ex.Message;
      //            MessageBox.Show(message);
      //        }
      //    }//using
      //    return true;
      //}//CreateSymbol

      //public static bool CreateSymbol2(UIDocument uidoc, DLG.DlgCreateSMW SMW)
      //{

      //    Document doc = uidoc.Document;
      //    Autodesk.Revit.DB.View vActv = doc.ActiveView;
      //    View3D view3D = doc.ActiveView as View3D;

      //    using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
      //    {
      //        try
      //        {
      //            Selection selection = uidoc.Selection;
      //            Line soilLine;
      //            Element selShin = null;
      //            XYZ picPointS = null;
      //            XYZ picPointE = null;
      //            if (SMW.m_way == 1)
      //            {
      //                ElementId selShinId = null;
      //                if (!ClsRevitUtil.PickObject(uidoc, "壁芯", "壁芯", ref selShinId))
      //                {
      //                    return false;
      //                }
      //                selShin = doc.GetElement(selShinId);
      //                Curve cvShin = (selShin.Location as LocationCurve).Curve;
      //                picPointS = cvShin.GetEndPoint(0);
      //                picPointE = cvShin.GetEndPoint(1);

      //            }
      //            if (SMW.m_way == 2)
      //            {
      //                picPointS = selection.PickPoint(familyNameKui + "を配置する開始位置を選択してください");
      //                if (picPointS == null)
      //                {
      //                    return false;
      //                }
      //                picPointE = selection.PickPoint(familyNameKui + "を配置する終了位置を選択してください");
      //                if (picPointE == null)
      //                {
      //                    return false;
      //                }

      //            }

      //            soilLine = Line.CreateBound(picPointS, picPointE);

      //            XYZ soilDirection = soilLine.Direction;//単位ベクトル
      //            double length = ClsRevitUtil.CovertFromAPI(soilLine.Length);//芯の長さ


      //            //シンボル配置
      //            if (!RoadFamilySymbolData(doc, familyPathSoil, familyNameSoil, out FamilySymbol sym))
      //            {
      //                return false;
      //            }

      //            //親杭配置
      //            if (!RoadFamilyData(doc, familyPathKui, familyNameKui, out Family oyaFam))
      //            {
      //                return false;
      //            }
      //            FamilySymbol oya = GetFamilySymbol(doc, familyNameKui, "SMW");


      //            t.Start();
      //            FailureHandlingOptions failOpt = t.GetFailureHandlingOptions();
      //            failOpt.SetFailuresPreprocessor(new WarningSwallower());
      //            t.SetFailureHandlingOptions(failOpt);

      //            //ダイアログ指定の値をSET
      //            //ソイル
      //            SetTypeParameter(sym, ClsGlobal.m_length, ClsRevitUtil.CovertToAPI(SMW.m_soilLen));
      //            SetTypeParameter(sym, ClsGlobal.m_dia, ClsRevitUtil.CovertToAPI(SMW.m_dia));
      //            int refPDist = SMW.m_refPDist;
      //            int soil = SMW.m_soil;
      //            //基準点から指定の数値離す
      //            picPointS = new XYZ(picPointS.X + (ClsRevitUtil.CovertToAPI(refPDist) * soilDirection.X),
      //                                picPointS.Y + (ClsRevitUtil.CovertToAPI(refPDist) * soilDirection.Y),
      //                                picPointS.Z);
      //            length -= refPDist;
      //            //H形鋼
      //            SetTypeParameter(oya, ClsGlobal.m_length, ClsRevitUtil.CovertToAPI(SMW.m_HLen));


      //            //ビューの向き//ビューがどの向きを向いていようが作成方向は同じであるため固定値
      //            XYZ vecView = new XYZ(0, 1, 0);
      //            if (picPointS.X < picPointE.X)
      //            {
      //                vecView = new XYZ(0, -1, 0);
      //            }
      //            //角度//X軸がプラスの時回転しすぎてしまうので分岐が必要
      //            double dAngle = soilDirection.AngleTo(vecView);
      //            //法線
      //            XYZ vec = new XYZ(0, 0, 1);//部材の法線ベクトルはZ軸を基準にしている

      //            ElementId levelID = GetLevelID(doc, "GL");
      //            XYZ picPoint = null;
      //            //グループ化するIDを保持する変数の宣言(nullだとAdd出来ないため)
      //            ICollection<ElementId> selectionIds = selection.GetElementIds();
      //            List<ElementId> selId = new List<ElementId>();//ボイド用

      //            double dCount = length / soil;//ダイアログソイルピッチずつ親杭、ソイルを配置するため何個置けるか
      //            int x = 0;
      //            for (int i = 0; i < dCount; i++)
      //            {
      //                picPoint = new XYZ(picPointS.X + (ClsRevitUtil.CovertToAPI(soil * i) * soilDirection.X),
      //                                   picPointS.Y + (ClsRevitUtil.CovertToAPI(soil * i) * soilDirection.Y),
      //                                   picPointS.Z + (ClsRevitUtil.CovertToAPI(soil * i) * soilDirection.Z));

      //                //******ソイル作成*******//
      //                ElementId CreatedID = Create(doc, picPoint, levelID, sym);//ソイル作成
      //                SetParameter(doc, CreatedID, ClsGlobal.m_refLvTop, ClsRevitUtil.CovertToAPI(SMW.m_soilTop));
      //                selectionIds.Add(CreatedID);
      //                selId.Add(CreatedID);
      //                //******ソイル作成*******//

      //                //配置パターン
      //                x++;
      //                if (x == 3) x = 0;
      //                if (x == SMW.m_putPtnFlag) continue;


      //                ElementId CreatedOyaID = Create(doc, picPoint, levelID, oya);//親杭作成
      //                SetParameter(doc, CreatedOyaID, ClsGlobal.m_refLvTop, ClsRevitUtil.CovertToAPI(SMW.m_HTop));

      //                Line axis = Line.CreateBound(picPoint, picPoint + vec);
      //                ClsRevitUtil.RotateElement(doc, CreatedOyaID, axis, Math.PI / 2 + dAngle);
      //                selectionIds.Add(CreatedOyaID);

      //                ////交互対応
      //                int nKougo = (SMW.m_KougoFlg && (i % 2) == 1 ? 2 : 1);
      //                oya = ChangeTypeID(doc, oya, CreatedOyaID, "SMW" + nKougo.ToString());

      //                //ジョイント数
      //                int nKasho = 0;
      //                if (nKougo == 1) nKasho = SMW.m_Kasho1;
      //                else nKasho = SMW.m_Kasho2;

      //                SetTypeParameter(oya, "ジョイント数", nKasho);

      //                //杭長さ
      //                List<int> pileLengths = new List<int>();
      //                if (nKougo == 1) pileLengths = SMW.m_ListPileLength1;
      //                else pileLengths = SMW.m_ListPileLength2;

      //                for (int j = 0; j < pileLengths.Count; j++)
      //                {
      //                    int nPileLength = pileLengths[j];
      //                    SetTypeParameter(oya, "杭" + (j + 1).ToString(), ClsRevitUtil.CovertToAPI(nPileLength));
      //                }
      //            }


      //            //*******壁作成*******//
      //            //Curve wallCV = Line.CreateBound(picPointS, picPoint);
      //            //Element wall = Wall.Create(doc, wallCV, levelID, false);//壁作成
      //            //selectionIds.Add(wall.Id);
      //            //length = 8000.0;//本来は親杭の高さから取得するべき
      //            ////パラメーターの設定処理
      //            //SetParameter(doc, wall.Id, "指定高さ", ClsRevitUtil.CovertToAPI(length));
      //            //SetParameter(doc, wall.Id, "基準レベル オフセット", ClsRevitUtil.CovertToAPI(-length));
      //            //*******壁作成*******//

      //            //補助線の作成
      //            double dH = ClsRevitUtil.CovertToAPI(150);//H鋼材によって150は変動(300/2)
      //            ModelLine outLine = CreateKabeHojyoLine(doc, picPointS, picPointE, dH, ClsRevitUtil.CovertToAPI(300));//外側
      //            ModelLine inLine = CreateKabeHojyoLine(doc, picPointS, picPointE, -dH, ClsRevitUtil.CovertToAPI(300));//内側

      //            //選択した線を削除して再度モデル線分を作成
      //            //if (selShin != null)
      //            //{
      //            //    doc.Delete(selShin.Id);
      //            //    ModelLine selLine = CreateKabeHojyoLine(doc, picPointS, picPointE, 0, 0);
      //            //}
      //            Group group = null;
      //            if (selectionIds.Count > 0)
      //            {
      //                // Group all selected elements
      //                //group = doc.Create.NewGroup(selectionIds);
      //            }
      //            //変更が加わるメッセージ原因不明
      //            t.Commit();

      //            //壁くり抜き処理
      //            if (SMW.m_bVoid) DoVoid(doc, selId, inLine);
      //        }
      //        catch (Exception ex)
      //        {
      //            TaskDialog.Show("Info Message", ex.Message);
      //            string message = ex.Message;
      //            MessageBox.Show(message);
      //        }
      //    }//using
      return true ;
    } //CreateSymbol

    #region "TypeIDを変更"

    public static FamilySymbol ChangeTypeID( Document doc, FamilySymbol familySymbol, ElementId targetId,
      string strNewType )
    {
      // 複製元のタイプを取得
      FamilySymbol sourceType = familySymbol ; // 複製元のタイプ

      //タイプが存在しない場合追加
      List<string> typeNamses = GetTypeNames( doc, sourceType ) ;
      ElementType elementType = null ;
      if ( ! typeNamses.Contains( strNewType ) ) {
        // タイプを複製
        elementType = sourceType.Duplicate( strNewType ) ;
      }
      else {
        // タイプを取得
        elementType = GetElementType( doc, familySymbol, strNewType ) ;
      }

      Element instance = doc.GetElement( targetId ) ;
      instance.ChangeTypeId( elementType.Id ) ;

      return (FamilySymbol) elementType ;
    }

    ///タイプIDの一覧を取得
    public static List<string> GetTypeNames( Document doc, FamilySymbol familySymbol )
    {
      // 複製元のタイプを取得
      FamilySymbol sourceType = familySymbol ; // 複製元のタイプ

      //タイプの存在チェック
      List<string> typeNames = new List<string>() ;
      List<ElementId> typeIds = familySymbol.GetSimilarTypes().ToList() ;
      foreach ( ElementId id in typeIds ) {
        Element instance = doc.GetElement( id ) ;
        string typeName = instance.Name ;
        typeNames.Add( typeName ) ;
      }

      return typeNames ;
    }

    public static ElementType GetElementType( Document doc, FamilySymbol familySymbol, string strTypeName )
    {
      // 複製元のタイプを取得
      FamilySymbol sourceType = familySymbol ; // 複製元のタイプ

      //タイプの存在チェック
      List<ElementId> typeIds = familySymbol.GetSimilarTypes().ToList() ;
      ElementType elementType = null ;
      foreach ( ElementId id in typeIds ) {
        Element instance = doc.GetElement( id ) ;
        string typeName = instance.Name ;
        if ( typeName == strTypeName ) {
          elementType = doc.GetElement( id ) as ElementType ;
          break ;
        }
      }

      return elementType ;
    }

    #endregion

    #region 移動予定関数

    public static bool RoadFamilySymbolData( Document doc, string rfaFilePath, string name, out FamilySymbol sym )
    {
      sym = null ;
      // 既に読み込まれているかどうかをチェックする//symにするとうまく機能していない
      FilteredElementCollector collector = new FilteredElementCollector( doc ) ;
      IEnumerable<Element> elements = collector.OfClass( typeof( FamilySymbol ) ).ToElements() ;
      foreach ( Element elem in elements ) {
        FamilySymbol loadedFamily = elem as FamilySymbol ;
        if ( loadedFamily != null && loadedFamily.Name == Path.GetFileNameWithoutExtension( rfaFilePath ) ) {
          sym = loadedFamily ;
          break ;
        }
      }

      // まだ読み込まれていない場合は読み込む
      if ( sym == null ) {
        using ( Transaction tx = new Transaction( doc, "Load Family" ) ) {
          tx.Start() ;
          //doc.LoadFamily(rfaFilePath, out family);
          doc.LoadFamilySymbol( rfaFilePath, name, out sym ) ;
          tx.Commit() ;
        }
      }

      if ( sym == null ) {
        return false ;
      }

      //doc.LoadFamilySymbol(rfaFilePath, name, out sym);
      return true ;
    }

    public static bool RoadFamilyData( Document doc, string rfaFilePath, string name, out Family family )
    {
      family = null ;
      // 既に読み込まれているかどうかをチェックする
      FilteredElementCollector collector = new FilteredElementCollector( doc ) ;
      IEnumerable<Element> elements = collector.OfClass( typeof( Family ) ).ToElements() ;
      foreach ( Element elem in elements ) {
        Family loadedFamily = elem as Family ;
        if ( loadedFamily != null && loadedFamily.Name == Path.GetFileNameWithoutExtension( rfaFilePath ) ) {
          family = loadedFamily ;
          break ;
        }
      }

      // まだ読み込まれていない場合は読み込む
      if ( family == null ) {
        using ( Transaction tx = new Transaction( doc, "Load Family" ) ) {
          tx.Start() ;
          doc.LoadFamily( rfaFilePath, out family ) ;
          //doc.LoadFamilySymbol(rfaFilePath, name, out sym);
          tx.Commit() ;
        }
      }

      if ( family == null ) {
        return false ;
      }

      //doc.LoadFamilySymbol(rfaFilePath, name, out sym);
      return true ;
    }

    /// <summary>
    /// 壁に補助線を作成
    /// </summary>
    /// <param name="doc">ドキュメント</param>
    /// <param name="pointS">開始位置</param>
    /// <param name="pointE">終了位置</param>
    /// <param name="dH">H形鋼サイズ/2(+:外側, -:内側)</param>
    /// <param name="dEx">補助線の延長</param>
    /// <returns>補助線</returns>
    public static ModelLine CreateKabeHojyoLine( Document doc, XYZ pointS, XYZ pointE, double dH, double dEx )
    {
      XYZ Direction = Line.CreateBound( pointS, pointE ).Direction ;
      //補助線の作成
      XYZ hojyoS = new XYZ( pointS.X + ( dH * Direction.Y ) - ( dEx * Direction.X ),
        pointS.Y - ( dH * Direction.X ) - ( dEx * Direction.Y ), pointS.Z ) ;
      XYZ hojyoE = new XYZ( pointE.X + ( dH * Direction.Y ) + ( dEx * Direction.X ),
        pointE.Y - ( dH * Direction.X ) + ( dEx * Direction.Y ), pointE.Z ) ;
      Line hojyo = Line.CreateBound( hojyoS, hojyoE ) ;
      if ( dH > 0 ) //補助線の内外を判別するためにDirectionを反対にする
      {
        hojyo = Line.CreateBound( hojyoE, hojyoS ) ;
      }

      XYZ mid = 0.5 * ( hojyoS + hojyoE ) ;
      //作成した壁に補助線を引く
      Plane plane = Plane.CreateByNormalAndOrigin( XYZ.BasisZ, mid ) ;
      SketchPlane sketchPlane = SketchPlane.Create( doc, plane ) ;
      ModelLine modelLine = doc.Create.NewModelCurve( hojyo, sketchPlane ) as ModelLine ;
      return modelLine ;
    }

    public static double GetLineNearPointDistance( Curve cv, XYZ point )
    {
      XYZ startPoint = cv.GetEndPoint( 0 ) ;
      XYZ endPoint = cv.GetEndPoint( 1 ) ;
      double distanceToStart = point.DistanceTo( startPoint ) ;
      double distanceToEnd = point.DistanceTo( endPoint ) ;
      if ( distanceToStart < distanceToEnd ) {
        // 判定したい点は始点に近いです
        return distanceToEnd ;
      }
      else {
        // 判定したい点は終点に近いです
        return distanceToStart ;
      }
    }

    /// <summary>
    /// 選択したモデル線分を伸ばしていきベースとの交点を取得する
    /// </summary>
    /// <param name="doc">ドキュメント</param>
    /// <param name="point">ベースと接していなかったため、伸ばしていく点</param>
    /// <param name="constPoint">選択したモデル線分の基点となる点</param>
    /// <param name="elementIds">交点を見つけるベースととなるファミリインスタンスList</param>
    /// <returns>ベースと選択したモデル線分の交点</returns>
    public static XYZ GetLineNearFamilyInstancePoint( Document doc, XYZ point, XYZ constPoint,
      List<ElementId> elementIds )
    {
      XYZ insec = point ;
      FamilyInstance closestInstance = null ;
      double closestDistance = double.MaxValue ;

      foreach ( ElementId id in elementIds ) {
        FamilyInstance instance = doc.GetElement( id ) as FamilyInstance ;
        //LocationPoint location = instance.Location as LocationPoint;//LocationCurveだった腹起ベースのどこかしらのポイントを指定
        LocationCurve lCurve = instance.Location as LocationCurve ;
        XYZ midPoint =
          ( lCurve.Curve.GetEndPoint( 0 ) + lCurve.Curve.GetEndPoint( 1 ) ) / 2 ; //とりあえず中点//中点だけでは不十分な可能性がある

        // ファミリインスタンスの位置情報を取得して、判定したい点との距離を計算する
        double distance = midPoint.DistanceTo( point ) ;

        if ( distance < closestDistance ) {
          closestInstance = instance ;
          closestDistance = distance ; //pointとベースの中点との距離
        }
      }

      // 最も近いファミリインスタンスが見つかった場合の処理
      if ( closestInstance != null ) {
        // closestInstanceが最も近いファミリインスタンスです
        LocationCurve lCurve = closestInstance.Location as LocationCurve ;
        Curve cv = lCurve.Curve ;

        closestDistance = GetLineNearPointDistance( cv, point ) ;

        double dEx = ClsRevitUtil.CovertToAPI( 100 ) ;
        XYZ Direction = Line.CreateBound( constPoint, point ).Direction ;
        //100xXで探して見つかったインスタンスの中点までの距離X2に交点がない場合とりあえずと終了する//いくら伸ばしても交点が一生現れない（平行など）の判定が必要かもしれない
        //上記終了条件だと交わることが出来るのに届かない可能性がある
        //伸ばしていく端点から最も遠い端点までの距離を上限とする
        for ( int ext = 1 ; dEx * ext <= closestDistance ; ext++ ) {
          //切梁ベースを配置するために選択した線分のみ伸ばしているためモデル線分の進行方向に腹起ベースの1点が見えていないといけない
          XYZ hojyoS = new XYZ( point.X + ( dEx * ext * Direction.X ), point.Y + ( dEx * ext * Direction.Y ),
            point.Z ) ;
          Curve cvHojyo = Line.CreateBound( constPoint, hojyoS ) ;
          XYZ insecP = GetIntersection( cvHojyo, cv ) ;
          if ( insecP != null ) {
            insec = insecP ;
            break ;
          }
        }
      }

      return insec ;
    }

    public static Curve GetInsecCurve( Document doc, Curve cvBase, List<ElementId> idList )
    {
      List<XYZ> insecList = new List<XYZ>() ;
      Curve cv = null ;
      XYZ tmpStPoint = cvBase.GetEndPoint( 0 ) ;
      XYZ tmpEdPoint = cvBase.GetEndPoint( 1 ) ;
      foreach ( ElementId id in idList ) {
        FamilyInstance inst = doc.GetElement( id ) as FamilyInstance ;
        LocationCurve lCurve = inst.Location as LocationCurve ;
        if ( lCurve != null ) {
          XYZ sInst = lCurve.Curve.GetEndPoint( 0 ) ;
          XYZ eInst = lCurve.Curve.GetEndPoint( 1 ) ;
          //tmpStPointHara = new XYZ(tmpStPointHara.X, tmpStPointHara.Y, 0.0);
          //tmpEdPointHara = new XYZ(tmpEdPointHara.X, tmpEdPointHara.Y, 0.0);
          Curve cvInst = Line.CreateBound( sInst, eInst ) ;
          XYZ insec = GetIntersection( cvBase, cvInst ) ;
          if ( insec != null ) {
            insecList.Add( insec ) ;
          }
        }
      }

      //交点が１つもない場合は選択したモデル線分の端点に近い腹起ベースまで交点を伸ばす
      if ( insecList.Count == 0 ) {
        XYZ insec1 = GetLineNearFamilyInstancePoint( doc, tmpStPoint, tmpEdPoint, idList ) ;
        XYZ insec2 = GetLineNearFamilyInstancePoint( doc, tmpEdPoint, tmpStPoint, idList ) ;
        cv = Line.CreateBound( insec1, insec2 ) ;
      }

      //交点が１つしかない場合は選択したモデル線分のどちらの端点に近いかを判定する
      if ( insecList.Count == 1 ) {
        double distanceToStart = insecList[ 0 ].DistanceTo( tmpStPoint ) ;
        double distanceToEnd = insecList[ 0 ].DistanceTo( tmpEdPoint ) ;
        XYZ insec ;
        if ( distanceToStart < distanceToEnd ) {
          // 判定したい点は始点に近いです
          insec = GetLineNearFamilyInstancePoint( doc, tmpEdPoint, tmpStPoint, idList ) ;
          cv = Line.CreateBound( insecList[ 0 ], insec ) ;
        }
        else {
          // 判定したい点は終点に近いです
          insec = GetLineNearFamilyInstancePoint( doc, tmpStPoint, tmpEdPoint, idList ) ;
          cv = Line.CreateBound( insec, insecList[ 0 ] ) ;
        }
      }

      //交点が2つある場合は交点で切梁ベースを作成する
      if ( insecList.Count == 2 ) {
        cv = Line.CreateBound( insecList[ 0 ], insecList[ 1 ] ) ;
      }

      return cv ;
    }

    /// <summary>
    /// 2点間の向きを+方向に変換する
    /// </summary>
    /// <param name="tmpStPoint">始点</param>
    /// <param name="tmpEdPoint">終点</param>
    /// <returns>+向きのポイント</returns>
    public static Curve ChangDirection( XYZ tmpStPoint, XYZ tmpEdPoint )
    {
      Curve cv = Line.CreateBound( tmpStPoint, tmpEdPoint ) ;
      //ベースの向きを統一する
      XYZ BaseDir = Line.CreateBound( tmpStPoint, tmpEdPoint ).Direction ;
      if ( ClsGeo.GEO_LT0( BaseDir.X ) || ClsGeo.GEO_LT0( BaseDir.Y ) ) {
        //SとE入れ替え
        cv = Line.CreateBound( tmpEdPoint, tmpStPoint ) ;
      }

      return cv ;
    }

    /// <summary>
    /// シンボルのパスを取得する
    /// </summary>
    /// <param name="file">"山留壁ファミリ\\"or"支保工ファミリ\\" + "ファミリ名" + ".rfa"</param>
    /// <returns>シンボルパス</returns>
    public static string GetSymbolPath( string file )
    {
      string symbolFolpath = ClsZumenInfo.GetYMSFolder() ;
      string shinfamily = System.IO.Path.Combine( symbolFolpath, file ) ;
      return shinfamily ;
    }

    public static double GetVoidDep( string kabe )
    {
      double dep = 0.0 ;
      switch ( kabe ) {
        case "kouya" :
        {
          dep = 0.0 ;
          break ;
        }
        case "oyakui" :
        {
          dep = 0.0 ;
          break ;
        }
        case "renzoku" :
        {
          dep = 0.0 ;
          break ;
        }
        case "ソイル_単軸" :
        {
          DLG.DlgCreateSMW SMW = ClsGlobal.m_SMW ;
          if ( SMW != null ) {
            //dep = SMW.m_void; //★扱い要相談
          }

          break ;
        }
        default :
        {
          dep = 0.0 ;
          break ;
        }
      }

      return dep ;
    }

    public static bool TestCreateBase( UIDocument uidoc )
    {
      ElementId id = null ;
      if ( ! PickObject( uidoc, "TESTベース", "モデル線分", ref id ) ) {
        return false ;
      }

      if ( id == null ) {
        MessageBox.Show( "芯が選択されていません。" ) ;
        return false ;
      }

      Document doc = uidoc.Document ;
      TestCreateBase( doc, id ) ;
      return true ;
    }

    public static bool TestCreateBase( Document doc, ElementId id )
    {
      string shinName = "隅火打ベース" ;
      double offset = 300.0 ;
      string symbolFolpath = ClsZumenInfo.GetYMSFolder() ;
      string shinfamily = System.IO.Path.Combine( symbolFolpath, "山留壁ファミリ\\" + shinName + ".rfa" ) ;
      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        //シンボル読込
        if ( ! RoadFamilySymbolData( doc, shinfamily, shinName, out FamilySymbol sym ) ) {
          return false ;
        }

        Element inst = doc.GetElement( id ) ;
        LocationCurve lCurve = inst.Location as LocationCurve ;
        if ( lCurve == null ) {
          return false ;
        }

        XYZ tmpStPoint = lCurve.Curve.GetEndPoint( 0 ) ;
        XYZ tmpEdPoint = lCurve.Curve.GetEndPoint( 1 ) ;

        ElementId levelID = GetLevelID( doc, "GL" ) ;

        //腹起ベースを内側にオフセット（鋼材サイズ+オフセット量）
        double dEx = ClsRevitUtil.CovertToAPI( 300 + offset ) ; //300はダイアログ鋼材サイズから取得する
        XYZ Direction = Line.CreateBound( tmpStPoint, tmpEdPoint ).Direction ;
        tmpStPoint = new XYZ( tmpStPoint.X - ( dEx * Direction.Y ), tmpStPoint.Y + ( dEx * Direction.X ),
          tmpStPoint.Z ) ;
        tmpEdPoint = new XYZ( tmpEdPoint.X - ( dEx * Direction.Y ), tmpEdPoint.Y + ( dEx * Direction.X ),
          tmpEdPoint.Z ) ;

        Curve cv = Line.CreateBound( tmpStPoint, tmpEdPoint ) ;
        t.Start() ;
        ElementId CreatedID = Create( doc, cv, levelID, sym ) ;
        t.Commit() ;
      }

      return true ;
    }

    /// <summary>
    /// 選択した芯上に部材芯を作成する
    /// </summary>
    /// <param name="doc">ドキュメント</param>
    /// <param name="ids">選択された芯</param>
    /// <param name="shinName">配置する部材芯名</param>
    /// <param name="nDan">1:上段 0:同段 2:下段</param>
    /// <returns></returns>
    public static bool CreateHaraokoshiShin( Document doc, List<ElementId> ids, string shinName,
      DLG.DlgCreateHaraokoshiBase haraokoshiBase )
    {
      //string symbolFolpath = ClsZumenInfo.GetFamilyFolder();
      //string shinfamily = System.IO.Path.Combine(symbolFolpath, "山留壁ファミリ\\" + shinName + ".rfa");
      //using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
      //{
      //    //シンボル読込
      //    if (!RoadFamilySymbolData(doc, shinfamily, shinName, out FamilySymbol sym))
      //    {
      //        return false;
      //    }

      //    foreach (ElementId id in ids)
      //    {
      //        Element inst = doc.GetElement(id);
      //        LocationCurve lCurve = inst.Location as LocationCurve;
      //        if (lCurve == null)
      //        {
      //            continue;
      //        }

      //        XYZ tmpStPoint = lCurve.Curve.GetEndPoint(0);
      //        XYZ tmpEdPoint = lCurve.Curve.GetEndPoint(1);

      //        ElementId levelID = GetLevelID(doc, haraokoshiBase.m_level);

      //        //腹起ベースを内側にオフセット（鋼材サイズ+オフセット量）
      //        double dEx = ClsRevitUtil.CovertToAPI(300 + haraokoshiBase.m_offset);//300はダイアログ鋼材サイズから取得する
      //        XYZ Direction = Line.CreateBound(tmpStPoint, tmpEdPoint).Direction;
      //        tmpStPoint = new XYZ(tmpStPoint.X - (dEx * Direction.Y),
      //                             tmpStPoint.Y + (dEx * Direction.X),
      //                             tmpStPoint.Z);
      //        tmpEdPoint = new XYZ(tmpEdPoint.X - (dEx * Direction.Y),
      //                             tmpEdPoint.Y + (dEx * Direction.X),
      //                             tmpEdPoint.Z);

      //        Curve cv = Line.CreateBound(tmpStPoint, tmpEdPoint);
      //        t.Start();
      //        ElementId CreatedID = Create(doc, cv, levelID, sym);

      //        SetMojiParameter(doc, CreatedID, "段", haraokoshiBase.m_dan);

      //        //選択された芯の順番で上下を交互に割り振っていく
      //        //if (nDan != 0 )//上下段かのフラグ
      //        //{
      //        //    if (nDan % 2 != 0)
      //        //    {
      //        //        SetMojiParameter(doc, CreatedID, "段", "上段");
      //        //    }
      //        //    else
      //        //    {
      //        //        SetMojiParameter(doc, CreatedID, "段", "下段");
      //        //    }
      //        //    nDan++;
      //        //}
      //        t.Commit();
      //    }
      //}
      return true ;
    }

    /// <summary>
    /// 選択した芯上に部材芯を作成する(掘削側指定)
    /// </summary>
    /// <param name="doc">ドキュメント</param>
    /// <param name="ids">選択された芯</param>
    /// <param name="shinName">配置する部材芯名</param>
    /// <param name="nDan">1:上段 0:同段 2:下段</param>
    /// <returns></returns>
    public static bool CreateHaraokoshiShin( UIDocument uidoc, ElementId id, string shinName,
      DLG.DlgCreateHaraokoshiBase haraokoshiBase )
    {
      //Document doc = uidoc.Document;
      //string symbolFolpath = ClsZumenInfo.GetFamilyFolder();
      //string shinfamily = System.IO.Path.Combine(symbolFolpath, "山留壁ファミリ\\" + shinName + ".rfa");
      //using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
      //{
      //    //シンボル読込
      //    if (!RoadFamilySymbolData(doc, shinfamily, shinName, out FamilySymbol sym))
      //    {
      //        return false;
      //    }
      //    //掘削側を指定
      //    (XYZ tmpStPoint,  XYZ tmpEdPoint) = ClsVoid.SelectVoidSide(uidoc, id);
      //    if (tmpStPoint == null || tmpEdPoint == null) return false;

      //    XYZ Direction = Line.CreateBound(tmpStPoint, tmpEdPoint).Direction;//単位ベクトル取得

      //    ElementId levelID = GetLevelID(doc, haraokoshiBase.m_level);

      //    //腹起ベースを内側にオフセット（鋼材サイズ+オフセット量）
      //    double dEx = ClsRevitUtil.CovertToAPI(300 + haraokoshiBase.m_offset);//300はダイアログ鋼材サイズから取得する

      //    tmpStPoint = new XYZ(tmpStPoint.X - (dEx * Direction.Y),
      //                         tmpStPoint.Y + (dEx * Direction.X),
      //                         tmpStPoint.Z);
      //    tmpEdPoint = new XYZ(tmpEdPoint.X - (dEx * Direction.Y),
      //                         tmpEdPoint.Y + (dEx * Direction.X),
      //                         tmpEdPoint.Z);

      //    Curve cv = Line.CreateBound(tmpStPoint, tmpEdPoint);
      //    t.Start();
      //    ElementId CreatedID = Create(doc, cv, levelID, sym);

      //    SetMojiParameter(doc, CreatedID, "段", haraokoshiBase.m_dan);

      //    t.Commit();
      //}
      return true ;
    }

    public static bool CreateKiribariShin( Document doc, List<ElementId> ids, string shinName, ElementId haraokoshiShin,
      string sameDan )
    {
      //string symbolFolpath = ClsZumenInfo.GetFamilyFolder();
      //string shinfamily = System.IO.Path.Combine(symbolFolpath, "山留壁ファミリ\\" + shinName + ".rfa");
      //using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
      //{
      //    //シンボル読込
      //    if (!RoadFamilySymbolData(doc, shinfamily, shinName, out FamilySymbol sym))
      //    {
      //        return false;
      //    }

      //    //図面上の腹起ベースを全て取得
      //    List<ElementId> haraIdList = GetSelectCreatedFamilyInstanceList(doc, "腹起ベース");

      //    foreach (ElementId id in ids)
      //    {
      //        Element inst = doc.GetElement(id);
      //        FamilyInstance haraokoshiShinInstLevel = doc.GetElement(haraokoshiShin) as FamilyInstance;
      //        LocationCurve lCurve = inst.Location as LocationCurve;
      //        if (lCurve == null)
      //        {
      //            continue;
      //        }

      //        XYZ tmpStPoint = lCurve.Curve.GetEndPoint(0);
      //        XYZ tmpEdPoint = lCurve.Curve.GetEndPoint(1);

      //        ElementId levelID = haraokoshiShinInstLevel.Host.Id;//GetLevelID(doc, "レベル 1");
      //        Curve cv = Line.CreateBound(tmpStPoint, tmpEdPoint);

      //        //腹起ベースとの交点を見つける
      //        //Z軸を0で統一すると腹起ベースが複数あった場合に交点が複数できてしまう
      //        //Z軸を選択した腹起ベースと同じ高さに設定
      //        LocationCurve lCurveZ = haraokoshiShinInstLevel.Location as LocationCurve;
      //        if (lCurveZ == null)
      //        {
      //            continue;
      //        }
      //        XYZ tmpStPointZ = lCurveZ.Curve.GetEndPoint(0);
      //        XYZ tmpEdPointZ = lCurveZ.Curve.GetEndPoint(1);
      //        //X,Y:選択したモデル線分, Z:選択した腹起ベース//モデル線分の長さを伸ばすならここ
      //        tmpStPointZ = new XYZ(tmpStPoint.X, tmpStPoint.Y, tmpStPointZ.Z);
      //        tmpEdPointZ = new XYZ(tmpEdPoint.X, tmpEdPoint.Y, tmpEdPointZ.Z);
      //        Curve cvZ = Line.CreateBound(tmpStPointZ, tmpEdPointZ);

      //        List<XYZ> insecList = new List<XYZ>();
      //        foreach (ElementId haraId in haraIdList)
      //        {
      //            FamilyInstance haraokoshiShinInst = doc.GetElement(haraId) as FamilyInstance;
      //            LocationCurve lCurveHara = haraokoshiShinInst.Location as LocationCurve;
      //            if (lCurveHara != null)
      //            {
      //                XYZ tmpStPointHara = lCurveHara.Curve.GetEndPoint(0);
      //                XYZ tmpEdPointHara = lCurveHara.Curve.GetEndPoint(1);
      //                //tmpStPointHara = new XYZ(tmpStPointHara.X, tmpStPointHara.Y, 0.0);
      //                //tmpEdPointHara = new XYZ(tmpEdPointHara.X, tmpEdPointHara.Y, 0.0);
      //                Curve cvHara = Line.CreateBound(tmpStPointHara, tmpEdPointHara);
      //                XYZ insec = GetIntersection(cvZ, cvHara);
      //                if (insec != null)
      //                {
      //                    insecList.Add(insec);
      //                }
      //            }
      //        }
      //        //交点が１つもない場合は選択したモデル線分の端点に近い腹起ベースまで交点を伸ばす
      //        if (insecList.Count == 0)
      //        {
      //            XYZ insec1 = GetLineNearFamilyInstancePoint(doc, tmpStPointZ, tmpEdPointZ, haraIdList);
      //            XYZ insec2 = GetLineNearFamilyInstancePoint(doc, tmpEdPointZ, tmpStPointZ, haraIdList);
      //            cv = Line.CreateBound(insec1, insec2);
      //        }
      //        //交点が１つしかない場合は選択したモデル線分のどちらの端点に近いかを判定する
      //        if(insecList.Count == 1)
      //        {
      //            double distanceToStart = insecList[0].DistanceTo(tmpStPointZ);
      //            double distanceToEnd = insecList[0].DistanceTo(tmpEdPointZ);
      //            XYZ insec;
      //            if (distanceToStart < distanceToEnd)
      //            {
      //                // 判定したい点は始点に近いです
      //                insec = GetLineNearFamilyInstancePoint(doc, tmpEdPointZ, tmpStPointZ, haraIdList);
      //                cv = Line.CreateBound(insecList[0], insec);
      //            }
      //            else
      //            {
      //                // 判定したい点は終点に近いです
      //                insec = GetLineNearFamilyInstancePoint(doc, tmpStPointZ, tmpEdPointZ, haraIdList);
      //                cv = Line.CreateBound(insec, insecList[0]);
      //            }
      //        }
      //        //交点が2つある場合は交点で切梁ベースを作成する
      //        if(insecList.Count == 2)
      //        {
      //            cv = Line.CreateBound(insecList[0], insecList[1]);
      //        }
      //        //切梁ベースの向きを統一する
      //        cv = ChangDirection(cv.GetEndPoint(0), cv.GetEndPoint(1));

      //        t.Start();
      //        ElementId CreatedID = Create(doc, cv, levelID, sym);
      //        SetMojiParameter(doc, CreatedID, "段", sameDan);
      //        t.Commit();
      //    }
      //}
      return true ;
    }

    /// <summary>
    /// 芯材に仮鋼材を配置する
    /// </summary>
    /// <param name="doc">ドキュメント</param>
    /// <returns></returns>
    public static bool CreateKariKouzai( Document doc )
    {
      ////ワークセット
      //ClsWorkset clsWS = new ClsWorkset();
      //clsWS.SetWorkSetTest3(doc);

      string symbolFolpath = ClsZumenInfo.GetYMSFolder() ;
      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        //移動予定//
        string kozaifamily = System.IO.Path.Combine( symbolFolpath, "支保工ファミリ\\02_形鋼\\形鋼_H300X300X10X15.rfa" ) ;
        string kouzaiName = "形鋼_H300X300X10X15" ; //芯材から情報を取ってくる予定
        //図面上の腹起ベースを全て取得
        List<ElementId> haraIdList = GetSelectCreatedFamilyInstanceList( doc, "腹起ベース" ) ;

        List<ElementId> elements = GetAllCreatedFamilyInstanceList( doc ) ;
        List<ElementId> targetFamilies = new List<ElementId>() ;
        foreach ( string baseName in ClsGlobal.m_baseShinList ) {
          foreach ( ElementId elem in elements ) {
            if ( elem != null && doc.GetElement( elem ).Name == baseName ) {
              targetFamilies.Add( elem ) ;
            }
          }
        }

        foreach ( ElementId id in targetFamilies ) {
          string dan = GetParameter( doc, id, "段" ) ;
          //Element inst = doc.GetElement(id);
          FamilyInstance inst = doc.GetElement( id ) as FamilyInstance ;
          LocationCurve lCurve = inst.Location as LocationCurve ;
          if ( lCurve == null ) {
            continue ;
          }

          XYZ tmpStPoint = lCurve.Curve.GetEndPoint( 0 ) ;
          XYZ tmpEdPoint = lCurve.Curve.GetEndPoint( 1 ) ;

          string buzaiShinName = inst.Name ;
          string buzaiName = buzaiShinName.Replace( "ベース", "" ) ;

          if ( ! RoadFamilyData( doc, kozaifamily, kouzaiName, out Family kouFam ) ) {
            return false ;
          }

          FamilySymbol kozaiSym = GetFamilySymbol( doc, kouzaiName, buzaiName ) ;

          string sunpou = ClsYMSUtil.GetKouzaiSizeSunpou( kouzaiName, 1 ) ;
          double kouzaiSize =
            ClsRevitUtil.CovertToAPI( ClsCommonUtils.ChangeStrToDbl( sunpou ) ) /
            2 ; //GetTypeParameter(kozaiSym, "エンドプレートD") / 2;
          double kouzaiSizeD =
            ClsRevitUtil.CovertToAPI( ClsCommonUtils.ChangeStrToDbl( sunpou ) ) /
            2 ; //GetTypeParameter(kozaiSym, "エンドプレートW") / 2;
          //double kouzaiSizeKiribari = GetTypeParameter(kozaiSym, "エンドプレートD");

          if ( buzaiName == "腹起" ) //腹起の場合は外側に仮鋼材を配置
          {
            //内外判定のための向き
            XYZ Direction = Line.CreateBound( tmpStPoint, tmpEdPoint ).Direction ;

            //腹起ベースとの交点を探す
            Curve cvHaraBase = Line.CreateBound( tmpStPoint, tmpEdPoint ) ;
            List<XYZ> insecList = new List<XYZ>() ;
            foreach ( ElementId haraId in haraIdList ) {
              FamilyInstance haraokoshiShinInst = doc.GetElement( haraId ) as FamilyInstance ;
              LocationCurve lCurveHara = haraokoshiShinInst.Location as LocationCurve ;
              if ( lCurveHara != null ) {
                XYZ tmpStPointHara = lCurveHara.Curve.GetEndPoint( 0 ) ;
                XYZ tmpEdPointHara = lCurveHara.Curve.GetEndPoint( 1 ) ;
                //tmpStPointHara = new XYZ(tmpStPointHara.X, tmpStPointHara.Y, 0.0);
                //tmpEdPointHara = new XYZ(tmpEdPointHara.X, tmpEdPointHara.Y, 0.0);
                Curve cvHara = Line.CreateBound( tmpStPointHara, tmpEdPointHara ) ;
                XYZ insec = GetIntersection( cvHaraBase, cvHara ) ;
                if ( insec != null ) {
                  insecList.Add( insec ) ;
                }
              }
            }

            //交点が1つ以上ある場合は交点で腹起仮鋼材を作成する
            if ( 1 <= insecList.Count ) {
              for ( int i = 0 ; i < insecList.Count ; i++ ) {
                double distanceToStart = insecList[ i ].DistanceTo( tmpStPoint ) ;
                double distanceToEnd = insecList[ i ].DistanceTo( tmpEdPoint ) ;
                if ( distanceToStart < distanceToEnd ) {
                  // 判定したい点は始点に近いです
                  tmpStPoint = new XYZ( insecList[ i ].X - ( kouzaiSizeD * 2 * Direction.X ),
                    insecList[ i ].Y - ( kouzaiSizeD * 2 * Direction.Y ), insecList[ i ].Z ) ;
                }
                else {
                  // 判定したい点は終点に近いです
                  tmpEdPoint = new XYZ( insecList[ i ].X + ( kouzaiSizeD * 2 * Direction.X ),
                    insecList[ i ].Y + ( kouzaiSizeD * 2 * Direction.Y ), insecList[ i ].Z ) ;
                }
              }
            }

            tmpStPoint = new XYZ( tmpStPoint.X + ( kouzaiSizeD * Direction.Y ),
              tmpStPoint.Y - ( kouzaiSizeD * Direction.X ), tmpStPoint.Z ) ;
            tmpEdPoint = new XYZ( tmpEdPoint.X + ( kouzaiSizeD * Direction.Y ),
              tmpEdPoint.Y - ( kouzaiSizeD * Direction.X ), tmpEdPoint.Z ) ;
          }

          //if (buzaiName == "切梁")//切梁の場合は短く仮鋼材を配置
          //{
          //    //補助線の作成
          //    XYZ Direction = Line.CreateBound(tmpStPoint, tmpEdPoint).Direction;
          //    tmpStPoint = new XYZ(tmpStPoint.X + (kouzaiSizeKiribari * Direction.X),
          //                         tmpStPoint.Y + (kouzaiSizeKiribari * Direction.Y),
          //                         tmpStPoint.Z);
          //    tmpEdPoint = new XYZ(tmpEdPoint.X - (kouzaiSizeKiribari * Direction.X),
          //                         tmpEdPoint.Y - (kouzaiSizeKiribari * Direction.Y),
          //                         tmpEdPoint.Z);
          //}
          ElementId levelID = inst.Host.Id ;
          Curve cv = Line.CreateBound( tmpStPoint, tmpEdPoint ) ;

          t.Start() ;
          ElementId kariId = GetParameterElementId( doc, id, "仮鋼材" ) ;
          ElementId CreatedID = Create( doc, cv, levelID, kozaiSym ) ; //例外処理
          SetMojiParameter( doc, id, "仮鋼材", CreatedID ) ;

          //選択された芯の段によって調整
          if ( dan == "上段" ) {
            SetParameter( doc, CreatedID, "ホストからのオフセット", kouzaiSize ) ;
          }
          else if ( dan == "下段" ) {
            SetParameter( doc, CreatedID, "ホストからのオフセット", -kouzaiSize ) ;
          }

          t.Commit() ; //ここで重複している場合、警告メッセージが表示される
        }
      }

      return true ;
    }

    /// <summary>
    /// 選択された壁を補助線を基準にボイドする
    /// </summary>
    /// <param name="doc">ドキュメント</param>
    /// <param name="selId">選択された壁</param>
    /// <param name="line">補助線</param>
    /// <returns></returns>
    public static bool DoVoid( Document doc, List<ElementId> selId, Element line )
    {
      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        Autodesk.Revit.DB.View v = getView( doc, "GL", ViewType.FloorPlan ) ;
        try {
          //ソイル位置作成
          Curve cv = ( line.Location as LocationCurve ).Curve ;
          XYZ lineOrigin = cv.GetEndPoint( 0 ) ;
          XYZ lineEndPoint = cv.GetEndPoint( 1 ) ;
          XYZ Direction = Line.CreateBound( lineOrigin, lineEndPoint ).Direction ;
          XYZ soilDirection = lineEndPoint - lineOrigin ;
          double wallLength = soilDirection.GetLength() ;
          XYZ p = new XYZ( ( lineOrigin.X + lineEndPoint.X ) / 2.0, ( lineOrigin.Y + lineEndPoint.Y ) / 2.0,
            ( lineOrigin.Z + lineEndPoint.Z ) / 2.0 ) ;

          //掘削シンボル作成
          string symbolFolpath = ClsZumenInfo.GetYMSFolder() ;
          string voidfamily = System.IO.Path.Combine( symbolFolpath, "山留壁ファミリ\\掘削用ボイド.rfa" ) ;
          if ( ! RoadFamilySymbolData( doc, voidfamily, "掘削用ボイド", out FamilySymbol sym ) ) {
            return false ;
          }

          //選択した1つ目を使用する（全て同一の壁が選択されているものとする）
          Element kabeElem = doc.GetElement( selId[ 0 ] ) ;
          //選択したシンボル名からどの壁か判別し深さを取得する
          double voidDip = GetVoidDep( kabeElem.Name ) ;
          if ( voidDip <= 0.0 ) return false ;

          //ビューの向き//ビューがどの向きを向いていようが作成方向は同じであるため固定値
          XYZ vecView = new XYZ( 0, 1, 0 ) ;

          //角度//X軸がプラスの時回転しすぎてしまうので分岐が必要
          double dAngle = Direction.AngleTo( vecView ) ;
          if ( lineOrigin.X < lineEndPoint.X ) {
            dAngle = -dAngle ;
          }

          XYZ vec = new XYZ( 0, 0, 1 ) ; //部材の法線ベクトルはZ軸を基準にしている
          Line axis = Line.CreateBound( p, p + vec ) ;

          t.Start() ;
          ElementId levelID = GetLevelID( doc, "GL" ) ;
          ElementId CreatedID = Create( doc, p, levelID, sym ) ;

          //ボイドを回転
          ClsRevitUtil.RotateElement( doc, CreatedID, axis, -Math.PI / 2 + dAngle ) ;
          //パラメーターの設定処理
          SetParameter( doc, CreatedID, ClsGlobal.m_length, wallLength ) ;
          //SetParameter(doc, CreatedID, "幅", ClsRevitUtil.CovertToAPI(300));
          SetParameter( doc, CreatedID, "掘削深さ1", ClsRevitUtil.CovertToAPI( voidDip ) ) ;
          SetParameter( doc, CreatedID, "掘削深さ2", ClsRevitUtil.CovertToAPI( voidDip ) ) ;

          //切り取り処理
          for ( int i = 0 ; i < selId.Count ; i++ ) {
            Element hostElem = doc.GetElement( selId[ i ] ) ;
            Element cutElem = doc.GetElement( CreatedID ) ;

            InstanceVoidCutUtils.AddInstanceVoidCut( doc, hostElem, cutElem ) ;
          }

          doc.Delete( line.Id ) ;
          t.Commit() ;
        }
        catch ( OperationCanceledException e ) {
        }
        catch ( Exception e ) {
          MessageBox.Show( e.Message ) ;
        }
      }

      return true ;
    }

    public static bool CreateSanjikuPeace( Document doc, ElementId idHara, ElementId idKiri )
    {
      string symbolFolpath = ClsZumenInfo.GetYMSFolder() ;
      string shinfamily = System.IO.Path.Combine( symbolFolpath, "支保工ファミリ\\23_三軸ピース\\三軸ピース _30SHP.rfa" ) ;
      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        //シンボル読込
        if ( ! RoadFamilySymbolData( doc, shinfamily, "三軸ピース _30SHP", out FamilySymbol sym ) ) {
          return false ;
        }

        FamilyInstance kiriShinInstLevel = doc.GetElement( idKiri ) as FamilyInstance ;
        LocationCurve lCurve = kiriShinInstLevel.Location as LocationCurve ;
        if ( lCurve == null ) {
          return false ;
        }

        Curve cvKiri = lCurve.Curve ;

        ElementId levelID = kiriShinInstLevel.Host.Id ;

        FamilyInstance haraokoshiShinInst = doc.GetElement( idHara ) as FamilyInstance ;
        LocationCurve lCurveHara = haraokoshiShinInst.Location as LocationCurve ;
        if ( lCurveHara != null ) {
          Curve cvHara = lCurveHara.Curve ;
          XYZ insec = GetIntersection( cvKiri, cvHara ) ;
          if ( insec != null ) {
            XYZ directionKiri = Line.CreateBound( cvKiri.GetEndPoint( 0 ), cvKiri.GetEndPoint( 1 ) ).Direction ;
            XYZ directionHara = Line.CreateBound( cvHara.GetEndPoint( 0 ), cvHara.GetEndPoint( 1 ) ).Direction ;
            double dAngle = directionKiri.AngleTo( XYZ.BasisY ) ;
            if ( ClsGeo.GEO_LT0( directionHara.Y ) ) {
              dAngle = -dAngle ;
            }

            if ( ClsGeo.GEO_LT0( directionHara.X ) ) {
              dAngle = Math.PI + dAngle ;
            }

            t.Start() ;
            ElementId CreatedID = Create( doc, insec, levelID, sym ) ;
            Line axis = Line.CreateBound( insec, insec + XYZ.BasisZ ) ;
            ClsRevitUtil.RotateElement( doc, CreatedID, axis, dAngle ) ;
            string dan = GetParameter( doc, idKiri, "段" ) ;
            double kouzaiSize = ClsRevitUtil.CovertToAPI( 300 / 2 ) ;
            //選択された芯の段によって調整
            //集計レベルの場合基準レベルを指定
            if ( dan == "上段" ) {
              SetParameter( doc, CreatedID, "基準レベルからの高さ", kouzaiSize ) ;
            }
            else if ( dan == "下段" ) {
              SetParameter( doc, CreatedID, "基準レベルからの高さ", -kouzaiSize ) ;
            }

            t.Commit() ;
          }
        }
      }

      return true ;
    }

    #endregion

    //public static bool CreateVoidFamily(UIDocument uidoc)
    //{
    //    //Document doc = uidoc.Document;

    //    //using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
    //    //{
    //    //    //ISelectionFilter selFilter = new WallSelectionFilter();
    //    //    Selection selection = uidoc.Selection;

    //    //    Autodesk.Revit.DB.View v = getView(doc, "GL", ViewType.FloorPlan);
    //    //    try
    //    //    {
    //    //        ISelectionFilter soilFilter = new FamilySelectionFilter("ソイル_単軸");//フィルターをかけるファミリ名は変更必要これだとソイルのみしか選択できない他の壁が選択できない
    //    //        IList<Reference> picSoil = selection.PickObjects(ObjectType.Element, soilFilter,"ソイルを選択してください");
    //    //        if (picSoil == null)
    //    //        {
    //    //            return false;
    //    //        }
    //    //        //選択した部材ID
    //    //        List<ElementId> selId = new List<ElementId>();
    //    //        foreach (Reference rf in picSoil)
    //    //        {
    //    //            selId.Add(rf.ElementId);
    //    //        }
    //    //        if (selId.Count < 1)
    //    //        {
    //    //            return false;
    //    //        }
    //    //        //線分選択
    //    //        ISelectionFilter modelLineFilter = new FamilySelectionFilter("モデル線分");
    //    //        Reference picLine = selection.PickObject(ObjectType.Element, modelLineFilter, "線分を選択してください");
    //    //        ElementId selLine = picLine.ElementId;
    //    //        Element line = null;
    //    //        try
    //    //        {
    //    //            line = doc.GetElement(selLine);
    //    //        }
    //    //        catch (Exception)
    //    //        {
    //    //            throw;
    //    //        }
    //    //        if (line == null)
    //    //        {
    //    //            return false;
    //    //        }
    //    //        //ソイル位置作成
    //    //        Curve cv = (line.Location as LocationCurve).Curve;
    //    //        XYZ lineOrigin = cv.GetEndPoint(0);
    //    //        XYZ lineEndPoint = cv.GetEndPoint(1);
    //    //        XYZ Direction = Line.CreateBound(lineOrigin, lineEndPoint).Direction;
    //    //        XYZ soilDirection = lineEndPoint - lineOrigin;
    //    //        double wallLength = soilDirection.GetLength();
    //    //        XYZ p = new XYZ((lineOrigin.X + lineEndPoint.X) / 2.0,
    //    //                        (lineOrigin.Y + lineEndPoint.Y) / 2.0,
    //    //                        (lineOrigin.Z + lineEndPoint.Z) / 2.0);

    //    //        //掘削シンボル作成
    //    //        string symbolFolpath = ClsZumenInfo.GetFamilyFolder();
    //    //        string voidfamily = System.IO.Path.Combine(symbolFolpath, "山留壁ファミリ\\掘削用ボイド.rfa");
    //    //        if (!RoadFamilySymbolData(doc, voidfamily, "掘削用ボイド", out FamilySymbol sym))
    //    //        {
    //    //            return false;
    //    //        }

    //    //        //選択した1つ目を使用する（全て同一の壁が選択されているものとする）
    //    //        Element kabeElem = doc.GetElement(selId[0]);
    //    //        //選択したシンボル名からどの壁か判別し深さを取得する
    //    //        double voidDip = GetVoidDep(kabeElem.Name);
    //    //        if (voidDip <= 0.0) return false;

    //    //        //ビューの向き//ビューがどの向きを向いていようが作成方向は同じであるため固定値
    //    //        XYZ vecView = new XYZ(0, 1, 0);

    //    //        //角度//X軸がプラスの時回転しすぎてしまうので分岐が必要
    //    //        double dAngle = Direction.AngleTo(vecView);
    //    //        if (lineOrigin.X < lineEndPoint.X)
    //    //        {
    //    //            dAngle = -dAngle;
    //    //        }
    //    //        XYZ vec = new XYZ(0, 0, 1);//部材の法線ベクトルはZ軸を基準にしている
    //    //        Line axis = Line.CreateBound(p, p + vec);

    //    //        t.Start();
    //    //        ElementId levelID = GetLevelID(doc, "GL");
    //    //        ElementId CreatedID = Create(doc, p, levelID, sym);

    //    //        //ボイドを回転
    //    //        ClsRevitUtil.RotateElement(doc, CreatedID, axis, -Math.PI / 2 + dAngle);
    //    //        //パラメーターの設定処理
    //    //        SetParameter(doc, CreatedID, ClsGlobal.m_length, wallLength);
    //    //        //SetParameter(doc, CreatedID, "幅", ClsRevitUtil.CovertToAPI(300));
    //    //        SetParameter(doc, CreatedID, "掘削深さ1", ClsRevitUtil.CovertToAPI(voidDip));//長さは取得する必要がある
    //    //        SetParameter(doc, CreatedID, "掘削深さ2", ClsRevitUtil.CovertToAPI(voidDip));

    //    //        //切り取り処理
    //    //        for (int i = 0; i < selId.Count; i++)
    //    //        {
    //    //            Element hostElem = doc.GetElement(selId[i]);
    //    //            Element cutElem = doc.GetElement(CreatedID);

    //    //            InstanceVoidCutUtils.AddInstanceVoidCut(doc, hostElem, cutElem);
    //    //        }
    //    //        doc.Delete(selLine);
    //    //        t.Commit();

    //    //    }
    //    //    catch (OperationCanceledException e)
    //    //    {
    //    //    }
    //    //    catch (Exception e)
    //    //    {
    //    //        MessageBox.Show(e.Message);
    //    //    }
    //    //}
    //    //return true;
    //}
    public static bool GetBaseCount( UIDocument uidoc )
    {
      Document doc = uidoc.Document ;
      List<ElementId> haraList = ClsHaraokoshiBase.GetAllHaraokoshiBaseList( doc ) ;
      List<ElementId> kiriList = ClsKiribariBase.GetAllKiribariBaseList( doc ) ;
      MessageBox.Show( "腹起ベースが" + haraList.Count.ToString() + "本、" + "切梁ベースが" + kiriList.Count.ToString() + "本、" + "計" +
                       ( haraList.Count + kiriList.Count ).ToString() + "本です。" ) ;
      return true ;
    }

    public static bool ChangeKariKouzai( UIDocument uidoc )
    {
      Document doc = uidoc.Document ;
      string kouzaiName = "形鋼_H300X300X10X15" ;
      List<ElementId> kariList = ClsRevitUtil.GetSelectCreatedFamilyInstanceFamilySymbolList( doc, kouzaiName ) ;
      string para1 = "真" ;
      string para2 = "偽" ;
      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        t.Start() ;
        foreach ( ElementId id in kariList ) {
          if ( ClsRevitUtil.GetInstMojiParameter( doc, id, "コメント" ) == "真" ) {
            ClsRevitUtil.SetInstMojiParameter( doc, id, "コメント", "偽" ) ;
          }
          else {
            ClsRevitUtil.SetInstMojiParameter( doc, id, "コメント", "真" ) ;
            para1 = "偽" ;
            para2 = "真" ;
          }
        }

        t.Commit() ;
      }

      MessageBox.Show( "仮鋼材" + kariList.Count.ToString() + "本のパラメータ" + "「コメント」の値を" + para1 + "から" + para2 +
                       "に変更しました。" ) ;
      return true ;
    }

    public static bool CreateHaraokoshiBase( UIDocument uidoc, DLG.DlgCreateHaraokoshiBase haraokoshiBase )
    {
      //Document doc = uidoc.Document;
      //List<ElementId> ids = new List<ElementId>();

      ////ワークセット
      //ClsWorkset clsWS = new ClsWorkset();
      //clsWS.SetWorkSetTest1(doc);

      ////for (int danText = haraokoshiBase.m_danFlag; ; )
      ////{
      ////    string dan = "同段";
      ////    if (danText != 0)//上下段かのフラグ
      ////    {
      ////        if (danText % 2 != 0)
      ////        {
      ////            dan = "上段";
      ////        }
      ////        else
      ////        {
      ////            dan = "下段";
      ////        }
      ////        danText++;//0以外なら加算していく
      ////    }
      ////    ElementId id = null;
      ////    if(!PickObject(uidoc, dan + "腹起ベースを配置する芯", "モデル線分", ref id))
      ////    {
      ////        break;
      ////    }
      ////    ids.Add(id);
      ////}


      ////ベータ版
      ////if (!PickObjects(uidoc, "腹起ベース", "モデル線分", ref ids))
      ////{
      ////    return false;
      ////}
      ////if (ids.Count < 1)
      ////{
      ////    MessageBox.Show("芯が選択されていません。");
      ////    return false;
      ////}
      ////CreateHaraokoshiShin(doc, ids, "腹起ベース", haraokoshiBase);
      //try
      //{
      //    ElementId id = null;
      //    for (; ; )
      //    {
      //        if (!PickObject(uidoc, "腹起ベース", "モデル線分", ref id))
      //        {
      //            return false;
      //        }
      //        if (id == null)
      //        {
      //            MessageBox.Show("芯が選択されていません。");
      //            return false;
      //        }
      //        CreateHaraokoshiShin(uidoc, id, "腹起ベース", haraokoshiBase);
      //    }
      //}
      //catch { }
      return true ;
    }

    public static bool CreateKiribariBase( UIDocument uidoc, DLG.DlgCreateKiribariBase kiribari )
    {
      //Document doc = uidoc.Document;
      //List<ElementId> ids = new List<ElementId>();

      ////ワークセット
      //ClsWorkset clsWS = new ClsWorkset();
      //clsWS.SetWorkSetTest1(doc);

      //ElementId id = null;
      //if (!PickObject(uidoc, "切梁ベースを配置する段の腹起ベース", "腹起ベース", ref id))
      //{
      //    return false;
      //}

      //if (!PickObjects(uidoc, "切梁ベース", "モデル線分", ref ids))
      //{
      //    return false;
      //}

      //if (ids.Count < 1)
      //{
      //    MessageBox.Show("芯が選択されていません。");
      //    return false;
      //}

      //CreateKiribariShin(doc, ids, "切梁ベース", id, GetParameter(doc, id, "段"));
      return true ;
    }

    public static bool CreataKariKouzai( UIDocument uidoc )
    {
      Document doc = uidoc.Document ;

      string buzainame = string.Empty ;

      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        //List<ElementId> ids = new List<ElementId>();
        //List<string> filters = new List<string> { "腹起ベース", "切梁ベース" };
        //if (!PickObjectsFilters(uidoc, "仮鋼材", filters, ref ids))
        //{
        //    return false;
        //}

        //if (ids.Count < 1)
        //{
        //    MessageBox.Show("腹起ベースが選択されていません。");
        //    return false;
        //}
        CreateKariKouzai( doc ) ;

        return true ;
      }
    }

    public static bool CreateSanjikuPeace( UIDocument uidoc )
    {
      Document doc = uidoc.Document ;
      ElementId idHara = null ;
      if ( ! PickObject( uidoc, "腹起ベース", "腹起ベース", ref idHara ) ) {
        return false ;
      }

      ElementId idKiri = null ;
      if ( ! PickObject( uidoc, "切梁ベース", "切梁ベース", ref idKiri ) ) {
        return false ;
      }

      CreateSanjikuPeace( doc, idHara, idKiri ) ;
      return true ;
    }

    /// <summary>
    /// 芯を選択して壁を配置
    /// </summary>
    /// <param name="uidoc"></param>
    /// <returns></returns>
    public static (Wall, Element) CreateWallSelectLine( UIDocument uidoc )
    {
      Document doc = uidoc.Document ;
      Autodesk.Revit.DB.View vActv = doc.ActiveView ;
      Wall wall = null ;
      Element selShin = null ;
      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        try {
          Selection selection = uidoc.Selection ;
          Reference picShin = selection.PickObject( ObjectType.Element, "芯を選択してください" ) ;

          selShin = GetSelectLine( doc, picShin ) ;

          if ( selShin == null ) {
            return ( wall, selShin ) ;
          }

          //ModelLine mdLine = selShin as ModelLine;
          //Curve geoCurve = mdLine.GeometryCurve;
          //Line geoLine = geoCurve as Line;
          //XYZ geoDirection = geoLine.Direction;//芯のvector
          ////double length = geoLine.Length;//芯の長さ

          //芯位置作成
          Curve cvShin = ( selShin.Location as LocationCurve ).Curve ;


          t.Start() ;

          ElementId levelID = GetLevelID( doc, "GL" ) ;

          //*******壁作成*******//
          wall = Wall.Create( doc, cvShin, levelID, false ) ; //壁作成
          //length = 8000.0;//本来は親杭の高さから取得するべき
          //パラメーターの設定処理
          //SetParameter(doc, wall.Id, "指定高さ", ClsRevitUtil.CovertToAPI(length));
          //SetParameter(doc, wall.Id, "基準レベル オフセット", ClsRevitUtil.CovertToAPI(-length));
          //*******壁作成*******//


          //変更が加わるメッセージ原因不明
          t.Commit() ;
        }
        catch {
        }
      } //using

      return ( wall, selShin ) ;
    } //CreateWallSelectLine


    public static bool testCommand2( UIDocument uidoc )
    {
      Document doc = uidoc.Document ;

      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        t.Start() ;


        List<List<XYZ>> xyzlistlist = new List<List<XYZ>>() ;

        List<ElementId> modellineIdList = GetALLModelLine( doc ) ;
        foreach ( ElementId lid in modellineIdList ) {
          ModelLine ml = doc.GetElement( lid ) as ModelLine ;
          GeometryElement ge = ml.get_Geometry( new Options() ) ;
          if ( ge != null ) {
            foreach ( GeometryObject go in ge ) {
              Line line = go as Line ;
              List<XYZ> xyzlist = new List<XYZ>() ;
              XYZ p1 = line.GetEndPoint( 0 ) ;
              XYZ p2 = line.GetEndPoint( 1 ) ;

              xyzlist.Add( p1 ) ;
              xyzlist.Add( p2 ) ;
              xyzlistlist.Add( xyzlist ) ;
            }

            doc.Delete( lid ) ;
          }
        }

        // List<ElementId> levelList = GetAllLevelID(doc);

        ElementId levelID = GetLevelID( doc, "レベル 1" ) ;
        FamilySymbol sym = GetFamilySymbol( doc, "芯", "共通芯" ) ;
        if ( sym == null ) {
          return false ;
        }

        int cnt = 0 ;
        foreach ( List<XYZ> lst in xyzlistlist ) {
          if ( lst.Count < 2 ) {
            continue ;
          }

          Curve cv = Line.CreateBound( lst[ 0 ], lst[ 1 ] ) ;
          ElementId CreatedID = Create( doc, cv, levelID, sym ) ;
          if ( CreatedID != null ) {
            cnt++ ;
          }
        }

        t.Commit() ;

        MessageBox.Show( cnt.ToString() + "本のモデル線分を共通芯に置き換えました" ) ;
      }

      return true ;
    }

    /// <summary>
    /// 壁作成
    /// </summary>
    /// <param name="doc">ドキュメント</param>
    /// <param name="startPt">始点</param>
    /// <param name="endPt">終点</param>
    /// <param name="baseLevelId">基準レベルID</param>
    /// <param name="kind">ファミリ名（Unknown：不明、Basic標準：、Curtain：カーテン、Stacked：重ね）</param>
    /// <param name="typeName">タイプ名</param>
    /// <param name="flip">フリップ（表裏）</param>
    /// <param name="structural">構造のon/off</param>
    /// <returns></returns>
    public static ElementId CreateWall( Document doc, XYZ startPt, XYZ endPt, ElementId baseLevelId, WallKind kind,
      string typeName, bool flip = false, bool structural = true )
    {
      WallType symW = GetWallType( doc, kind, typeName ) ;
      return CreateWall( doc, startPt, endPt, baseLevelId, symW, flip, structural ) ;
    }

    /// <summary>
    /// 壁作成
    /// </summary>
    /// <param name="doc">ドキュメント</param>
    /// <param name="startPt">始点</param>
    /// <param name="endPt">終点</param>
    /// <param name="baseLevelId">基準レベルID</param>
    /// <param name="symW">WallType</param>
    /// <param name="flip">フリップ（表裏）</param>
    /// <param name="structural">構造のon/off</param>
    /// <returns></returns>
    public static ElementId CreateWall( Document doc, XYZ startPt, XYZ endPt, ElementId baseLevelId, WallType symW,
      bool flip = false, bool structural = true )
    {
      if ( symW == null ) {
        return null ;
      }

      Curve c = Autodesk.Revit.DB.Line.CreateBound( startPt, endPt ) ;
      Wall newWall = Wall.Create( doc, c, baseLevelId, structural ) ;
      //base.ID = newWall.Id;

      if ( true != WallModifyFamily( doc, newWall.Id, symW ) ) {
        return null ;
      }

      if ( true != WallModifyFlip( doc, newWall.Id, flip ) ) {
        return null ;
      }

      return newWall.Id ;
    }

    public static bool WallModifyFamily( Document doc, ElementId id, WallType type )
    {
      if ( id == null || doc == null ) {
        return false ;
      }

      try {
        Wall wall = doc.GetElement( id ) as Wall ;
        if ( wall == null ) {
          return false ;
        }

        wall.WallType = type ;
      }
      catch {
        return false ;
      }

      return true ;
    }

    /// <summary>
    /// フリップ変更
    /// </summary>
    /// <param name="doc">ドキュメント</param>
    /// <param name="id">id</param>
    /// <param name="structural"></param>
    /// <returns></returns>
    public static bool WallModifyFlip( Document doc, ElementId id, bool flip )
    {
      if ( id == null || doc == null ) {
        return false ;
      }

      try {
        Wall wall = doc.GetElement( id ) as Wall ;
        if ( wall == null ) {
          return false ;
        }

        if ( wall.Flipped != flip ) {
          wall.Flip() ;
        }
      }
      catch {
        return false ;
      }

      return true ;
    }

    /// <summary>
    /// WallType取得
    /// </summary>
    /// <param name="doc">Document</param>
    /// <param name="kind">ファミリ名</param>
    /// <param name="typeName">タイプ名</param>
    /// <returns></returns>
    public static WallType GetWallType( Document doc, WallKind kind, string typeName, string categoryName = "" )
    {
      WallType wType = null ;
      IEnumerable<Element> elems = null ;
      if ( string.IsNullOrEmpty( categoryName ) ) {
        elems = from elem in ( new FilteredElementCollector( doc ).OfClass( typeof( WallType ) ).ToElements() )
          let sym = elem as WallType
          where sym.Kind == kind && sym.Name == typeName
          select sym ;
      }
      else {
        elems = from elem in ( new FilteredElementCollector( doc ).OfClass( typeof( WallType ) ).ToElements() )
          let sym = elem as WallType
          where sym.Kind == kind && sym.Name == typeName && sym.Category.Name == categoryName
          select sym ;
      }

      foreach ( Element elemTmp in elems ) {
        wType = elemTmp as WallType ;
        break ;
      }

      return wType ;
    }

    public static bool PickObjects( UIDocument uidoc, string shinName, string filterName, ref List<ElementId> ids )
    {
      Document doc = uidoc.Document ;
      ids = new List<ElementId>() ;

      ISelectionFilter selFilter = new FamilySelectionFilter( filterName ) ;
      Selection selection = uidoc.Selection ;

      try {
        //IList<Reference> pickedElements = selection.PickObjects(ObjectType.Element, selFilter, "切梁を配置する芯を選択してください。");
        IList<Reference> pickedElements =
          selection.PickObjects( ObjectType.Element, selFilter, shinName + "を配置する芯を選択してください。" ) ;

        foreach ( Reference rf in pickedElements ) {
          ids.Add( rf.ElementId ) ;
        }
      }
      catch ( OperationCanceledException ) {
        return false ;
      }
      catch ( Exception e ) {
        return false ;
      }

      return true ;
    }

    public static bool PickObjectsFilters( UIDocument uidoc, string shinName, List<string> filterNameList,
      ref List<ElementId> ids )
    {
      Document doc = uidoc.Document ;
      ids = new List<ElementId>() ;

      ISelectionFilter selFilters = new FamilySelectionListFilter( filterNameList ) ;
      Selection selection = uidoc.Selection ;
      try {
        //IList<Reference> pickedElements = selection.PickObjects(ObjectType.Element, selFilter, "切梁を配置する芯を選択してください。");
        IList<Reference> pickedElements =
          selection.PickObjects( ObjectType.Element, selFilters, shinName + "を配置する芯を選択してください。" ) ;

        foreach ( Reference rf in pickedElements ) {
          ids.Add( rf.ElementId ) ;
        }
      }
      catch ( OperationCanceledException ) {
        return false ;
      }
      catch ( Exception e ) {
        return false ;
      }

      return true ;
    }

    public static bool PickObjectFilters( UIDocument uidoc, string mess, List<string> filterNameList, ref ElementId id )
    {
      Document doc = uidoc.Document ;
      id = null ;

      ISelectionFilter selFilters = new FamilySelectionListFilter( filterNameList ) ;
      Selection selection = uidoc.Selection ;
      try {
        Reference rf = selection.PickObject( ObjectType.Element, selFilters, mess ) ;
        id = rf.ElementId ;
      }
      catch ( OperationCanceledException ) {
        return false ;
      }
      catch ( Exception e ) {
        return false ;
      }

      return true ;
    }

    public static bool PickObjectLoop( UIDocument uidoc, string shinName, string filterName, ref List<ElementId> ids )
    {
      Document doc = uidoc.Document ;

      ISelectionFilter selFilter = new FamilySelectionFilter( filterName ) ;
      Selection selection = uidoc.Selection ;
      for ( ; ; ) {
        try {
          Reference pickedElement =
            selection.PickObject( ObjectType.Element, selFilter, shinName + "を配置する芯を選択してください。" ) ;
          ids.Add( pickedElement.ElementId ) ;
        }
        catch ( OperationCanceledException ) {
          break ;
        }
        catch ( Exception e ) {
          break ;
        }
      }

      return true ;
    }

    public static bool PickObject( UIDocument uidoc, string shinName, string filterName, ref ElementId id )
    {
      Document doc = uidoc.Document ;

      ISelectionFilter selFilter = new FamilySelectionFilter( filterName ) ;
      Selection selection = uidoc.Selection ;

      try {
        Reference pickedElement = selection.PickObject( ObjectType.Element, selFilter, shinName + "を選択してください。" ) ;
        id = pickedElement.ElementId ;
      }
      catch ( OperationCanceledException ) {
        return false ;
      }
      catch ( Exception e ) {
        return false ;
      }

      return true ;
    }

    public static List<ElementId> GetAllLevelID( Document doc )
    {
      if ( doc == null ) {
        return null ;
      }

      List<ElementId> elemIDList = new List<ElementId>() ;
      FilteredElementCollector collector = new FilteredElementCollector( doc ) ;
      ICollection<Element> collection = collector.OfClass( typeof( Level ) ).ToElements() ;

      foreach ( Element elem in collection ) {
        elemIDList.Add( elem.Id ) ;
      }

      return elemIDList ;
    }

    public static ElementId Create( Document doc, Curve cv, ElementId levelId, FamilySymbol symbol )
    {
      if ( symbol == null ) {
        return null ;
      }

      ElementId id = null ;

      if ( ! symbol.IsActive ) {
        symbol.Activate() ;
      }

      try {
        Level lv = doc.GetElement( levelId ) as Level ;
        FamilyInstance newInstance = doc.Create.NewFamilyInstance( cv, symbol, lv,
          Autodesk.Revit.DB.Structure.StructuralType.NonStructural ) ;

        //配置後に改めてデフォルト値設定
        ModifyLevel( doc, newInstance.Id, levelId ) ;
        ModifyOffset( doc, newInstance.Id, 0 ) ;
        id = newInstance.Id ;
      }
      catch {
        return null ;
      }

      return id ;
    }

    /// <summary>
    /// レベル変更
    /// </summary>
    /// <param name="doc">ドキュメント</param>
    /// <param name="id">ID</param>
    /// <param name="levelId"></param>
    /// <returns></returns>
    public static bool ModifyLevel( Document doc, ElementId id, ElementId levelId )
    {
      if ( id == null || doc == null ) {
        return false ;
      }

      Parameter param = doc.GetElement( id ).get_Parameter( BuiltInParameter.FAMILY_LEVEL_PARAM ) ;
      if ( param == null ) {
        return false ;
      }

      if ( param.IsReadOnly ) {
        return false ;
      }

      param.Set( levelId ) ;

      return true ;
    }

    public static bool ModifyOffset( Document doc, ElementId id, double offsetValue )
    {
      if ( id == null || doc == null ) {
        return false ;
      }

      Parameter param = doc.GetElement( id ).get_Parameter( BuiltInParameter.INSTANCE_FREE_HOST_OFFSET_PARAM ) ;
      if ( param == null ) {
        return false ;
      }

      if ( param.IsReadOnly ) {
        return false ;
      }

      param.Set( offsetValue ) ;
      return true ;
    }

    /// <summary>
    /// FamilySymbol取得
    /// </summary>
    /// <param name="doc">Document</param>
    /// <param name="familyName">ファミリ名</param>
    /// <param name="typeName">タイプ名</param>
    /// <param name="categoryName">カテゴリ名</param>
    /// <returns></returns>
    public static FamilySymbol GetFamilySymbol( Document doc, string familyName, string typeName,
      string categoryName = "" )
    {
      if ( doc == null ) {
        return null ;
      }

      FamilySymbol symbol = null ;
      IEnumerable<Element> elems = null ;
      if ( string.IsNullOrEmpty( categoryName ) ) {
        elems = from elem in ( new FilteredElementCollector( doc ).OfClass( typeof( FamilySymbol ) ).ToElements() )
          let sym = elem as FamilySymbol
          where sym.Name == typeName && sym.Family.Name == familyName
          select sym ;
      }
      else {
        elems = from elem in ( new FilteredElementCollector( doc ).OfClass( typeof( FamilySymbol ) ).ToElements() )
          let sym = elem as FamilySymbol
          where sym.Name == typeName && sym.Family.Name == familyName && sym.Family.FamilyCategory.Name == categoryName
          select sym ;
      }

      foreach ( Element elemTmp in elems ) {
        symbol = elemTmp as FamilySymbol ;
        break ;
      }

      return symbol ;
    }

    public static ElementId GetLevelID( Document doc, string levelName )
    {
      if ( doc == null ) {
        return null ;
      }

      ElementId elemId = null ;
      IEnumerable<Element> elems = null ;
      elems = from elem in ( new FilteredElementCollector( doc ).OfClass( typeof( Level ) ).ToElements() )
        let level = elem as Level
        where level.Name == levelName
        select level ;

      foreach ( Level lv in elems ) {
        elemId = lv.Id ;
      }

      return elemId ;
    }

    public static Element GetLevel( Document doc, string levelName )
    {
      if ( doc == null ) {
        return null ;
      }

      Element elemt = null ;
      IEnumerable<Element> elems = null ;
      elems = from elem in ( new FilteredElementCollector( doc ).OfClass( typeof( Level ) ).ToElements() )
        let level = elem as Level
        where level.Name == levelName
        select level ;

      foreach ( Level lv in elems ) {
        elemt = lv ;
      }

      return elemt ;
    }

    /// <summary>
    /// 指定ビューを取得
    /// </summary>
    /// <param name="doc">Document</param>
    /// <param name="name">ビュー名</param>
    public static Autodesk.Revit.DB.View getView( Document doc, string viewName, ViewType type )
    {
      Autodesk.Revit.DB.View getV = null ;

      IList<Element> views = new FilteredElementCollector( doc ).OfClass( typeof( Autodesk.Revit.DB.View ) )
        .ToElements() ;
      foreach ( Autodesk.Revit.DB.View v in views ) {
        if ( v.Name == viewName && v.ViewType == type ) {
          getV = v ;
          break ;
        }
      }

      return getV ;
    }

    public static List<ElementId> GetALLModelLine( Document doc )
    {
      List<ElementId> idList = new List<ElementId>() ;
      try {
        IEnumerable<Element> elems = null ;
        elems = from elem in ( new FilteredElementCollector( doc ).OfClass( typeof( CurveElement ) ).ToElements() )
          let sym = elem as CurveElement
          select sym ;
        foreach ( Element elem in elems ) {
          if ( elem is ModelLine ) {
            idList.Add( elem.Id ) ;
          }
          else {
            continue ;
          }
        }
      }
      catch ( Exception e ) {
        MessageBox.Show( e.Message ) ;
      }


      return idList ;
    }

    public static bool IsShin( Document doc, ElementId id )
    {
      bool res = false ;

      FamilyInstance inst = doc.GetElement( id ) as FamilyInstance ;
      if ( inst == null ) {
        return false ;
      }


      Type tp = inst.GetType() ;

      if ( tp.Name == "共通芯" ) {
        res = true ;
      }
      //string name = inst.Name;
      //if (name.StartsWith("PHS"))
      //{
      //    res = true;
      //}

      return res ;
    }

    /// <summary>
    /// 作図済みの全FamilyInstanceを取得
    /// </summary>
    /// <returns></returns>
    public static List<ElementId> GetAllCreatedFamilyInstanceList( Document doc )
    {
      List<ElementId> idList = new List<ElementId>() ;
      IEnumerable<Element> idIe = GetAllCreatedFamilyInstance( doc ) ;
      foreach ( Element elem in idIe ) {
        idList.Add( elem.Id ) ;
      }

      return idList ;
    }

    public static IEnumerable<Element> GetAllCreatedFamilyInstance( Document doc )
    {
      IEnumerable<Element> elems = null ;

      elems = from elem in ( new FilteredElementCollector( doc ).OfClass( typeof( FamilyInstance ) ).ToElements() )
        let sym = elem as FamilyInstance
        select sym ;

      return elems ;
    }

    /// <summary>
    /// 作図済みの指定のFamilyInstanceを取得
    /// </summary>
    /// <param name="doc">ドキュメント</param>
    /// <param name="name">取得するファミリインスタンスの名前</param>
    /// <returns>FamilyInstanceのList</returns>
    public static List<ElementId> GetSelectCreatedFamilyInstanceList( Document doc, string name )
    {
      List<ElementId> idList = new List<ElementId>() ;
      IEnumerable<Element> idIe = GetSelectCreatedFamilyInstance( doc, name ) ;
      foreach ( Element elem in idIe ) {
        idList.Add( elem.Id ) ;
      }

      return idList ;
    }

    public static IEnumerable<Element> GetSelectCreatedFamilyInstance( Document doc, string name )
    {
      IEnumerable<Element> elems = null ;

      elems = from elem in ( new FilteredElementCollector( doc ).OfClass( typeof( FamilyInstance ) ).ToElements() )
        let sym = elem as FamilyInstance
        where sym.Name == name
        select sym ;

      return elems ;
    }

    /// <summary>
    /// LiNE選択オブジェクト取得
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="picLine"></param>
    /// <returns>選択したLINEのElement</returns>
    public static Element GetSelectLine( Document doc, Reference picLine )
    {
      Element selLine = null ;
      if ( picLine == null ) {
        return selLine ;
      }

      //選択した部材ID
      ElementId selId = picLine.ElementId ;
      try {
        selLine = doc.GetElement( selId ) ;
      }
      catch ( Exception ) {
        throw ;
      }

      return selLine ;
    }

    /// <summary>
    /// 線の交点を取得
    /// </summary>
    /// <param name="cv1">線1</param>
    /// <param name="cv2">線2</param>
    /// <returns>交点</returns>
    public static XYZ GetIntersection( Curve cv1, Curve cv2 )
    {
      XYZ intersectionPoint = null ;
      IntersectionResultArray results ;
      SetComparisonResult result = cv1.Intersect( cv2, out results ) ;
      if ( result != SetComparisonResult.Overlap ) {
        return intersectionPoint ;
      }

      if ( results == null || results.Size != 1 ) {
        return intersectionPoint ;
      }

      IntersectionResult iResult = results.get_Item( 0 ) ;
      intersectionPoint = iResult.XYZPoint ;
      return intersectionPoint ;
    }

    /// <summary>
    /// 指定の点がCurveが通るか判定する
    /// </summary>
    /// <param name="curve">線分</param>
    /// <param name="point">指定点</param>
    /// <returns></returns>
    public static bool IsPointOnCurve( Curve curve, XYZ point )
    {
      // 曲線の始点から終点までのパラメータ範囲を取得
      double startParam = curve.GetEndParameter( 0 ) ;
      double endParam = curve.GetEndParameter( 1 ) ;

      // 点の座標をパラメータ空間に変換
      double pointParam = curve.Project( point ).Parameter ;

      // パラメータがパラメータ範囲内にあるかどうかを判定
      if ( pointParam >= startParam && pointParam <= endParam ) {
        return true ; // 点は曲線上に存在します
      }
      else {
        return false ; // 点は曲線上に存在しません
      }
    }

    /// <summary>
    /// 1点指示のファミリ作図
    /// </summary>
    /// <param name="doc">ドキュメント</param>
    /// <param name="point">座標</param>
    /// <param name="levelId">レベル</param>
    /// <param name="symbol">ファミリシンボル</param>
    /// <returns></returns>
    public static ElementId Create( Document doc, XYZ point, ElementId levelId, FamilySymbol symbol )
    {
      if ( symbol == null ) {
        return null ;
      }

      ElementId id = null ;

      if ( ! symbol.IsActive ) {
        symbol.Activate() ;
      }

      try {
        Level lv = doc.GetElement( levelId ) as Level ;
        FamilyInstance newInstance = doc.Create.NewFamilyInstance( point, symbol, lv,
          Autodesk.Revit.DB.Structure.StructuralType.NonStructural ) ;

        //配置後に改めてデフォルト値設定
        ModifyLevel( doc, newInstance.Id, levelId ) ;
        ModifyOffset( doc, newInstance.Id, 0 ) ;
        id = newInstance.Id ;
      }
      catch {
        return null ;
      }

      return id ;
    }

    /// <summary>
    /// インスタンスファミリDoubleパラメーターの変更
    /// </summary>
    /// <param name="doc">ドキュメント</param>
    /// <param name="CreatedID">シンボルID</param>
    /// <param name="paramName">パラメーター名</param>
    /// <param name="val">Double値</param>
    /// <returns></returns>
    public static bool SetParameter( Document doc, ElementId CreatedID, string paramName, Double val )
    {
      Parameter parm = doc.GetElement( CreatedID ).LookupParameter( paramName ) ;
      if ( parm == null ) {
        return false ;
      }

      parm.Set( val ) ;
      return true ;
    }

    /// <summary>
    /// タイププロパティDoubleパラメーターの変更
    /// </summary>
    /// <param name="sym">指定のFamilySymbol</param>
    /// <param name="paramName">パラメータ名</param>
    /// <param name="val">変更値</param>
    /// <returns></returns>
    public static bool SetTypeParameter( FamilySymbol sym, string paramName, Double val )
    {
      Parameter parm = sym.LookupParameter( paramName ) ;
      if ( parm == null ) return false ;
      parm.Set( val ) ;
      return true ;
    }

    /// <summary>
    /// タイププロパティDoubleパラメーターの取得
    /// </summary>
    /// <param name="sym">指定のFamilySymbol</param>
    /// <param name="paramName">パラメータ名</param>
    /// <returns></returns>
    public static double GetTypeParameter( FamilySymbol sym, string paramName )
    {
      try {
        Parameter parm = sym.LookupParameter( paramName ) ;
        if ( parm == null ) {
          return 0.0 ;
        }

        return parm.AsDouble() ;
      }
      catch {
        return 0.0 ;
      }
    }

    /// <summary>
    /// インスタンスファミリ文字パラメーターの変更
    /// </summary>
    /// <param name="doc">ドキュメント</param>
    /// <param name="CreatedID">シンボルID</param>
    /// <param name="paramName">パラメーター名</param>
    /// <param name="val">文字</param>
    /// <returns></returns>
    public static bool SetMojiParameter( Document doc, ElementId CreatedID, string paramName, string val )
    {
      Parameter parm = doc.GetElement( CreatedID ).LookupParameter( paramName ) ;
      if ( parm == null ) {
        return false ;
      }

      parm.Set( val ) ;
      return true ;
    }

    /// <summary>
    /// インスタンスファミリ文字パラメーターの変更
    /// </summary>
    /// <param name="doc">ドキュメント</param>
    /// <param name="CreatedID">シンボルID</param>
    /// <param name="paramName">パラメーター名</param>
    /// <param name="val">ElementId</param>
    /// <returns></returns>
    public static bool SetMojiParameter( Document doc, ElementId CreatedID, string paramName, ElementId val )
    {
      try {
        Parameter parm = doc.GetElement( CreatedID ).LookupParameter( paramName ) ;
        if ( parm == null ) {
          return false ;
        }

        if ( parm.StorageType == StorageType.Integer ) {
          parm.Set( val.IntegerValue ) ;
        }
      }
      catch {
      }

      return true ;
    }

    /// <summary>
    /// 指定オブジェクトのパラメータ取得
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="CreatedID"></param>
    /// <param name="paramName"></param>
    /// <returns></returns>
    //public static string GetParameter(Document doc, ElementId CreatedID, string paramName)
    //{
    //    IList<Parameter> iparm = doc.GetElement(CreatedID).GetParameters(paramName);
    //    List<Parameter> parmList = new List<Parameter>();
    //    if (iparm == null)
    //    {
    //        return null;
    //    }
    //    foreach (Parameter pr in iparm)
    //    {
    //        parmList.Add(pr);
    //    }
    //    if (parmList.Count < 1)
    //    {
    //        return null;
    //    }
    //    string dPara = parmList[0].AsString();
    //    return dPara;
    //}
    /// <summary>
    /// ElementIdパラメーターの取得
    /// </summary>
    /// <param name="doc">ドキュメント</param>
    /// <param name="CreatedID">シンボルID</param>
    /// <param name="paramName">パラメーター名</param>
    /// <returns></returns>
    public static ElementId GetParameterElementId( Document doc, ElementId CreatedID, string paramName )
    {
      try {
        Parameter parm = doc.GetElement( CreatedID ).LookupParameter( paramName ) ;
        if ( parm == null ) {
          return null ;
        }

        int id = parm.AsInteger() ;
        ElementId e = new ElementId( id ) ;
        return e ;
      }
      catch {
        return null ;
      }
    }

    /// <summary>
    /// Stringパラメーターの取得
    /// </summary>
    /// <param name="doc">ドキュメント</param>
    /// <param name="CreatedID">シンボルID</param>
    /// <param name="paramName">パラメーター名</param>
    /// <returns></returns>
    public static string GetParameter( Document doc, ElementId CreatedID, string paramName )
    {
      try {
        Parameter parm = doc.GetElement( CreatedID ).LookupParameter( paramName ) ;
        if ( parm == null ) {
          return string.Empty ;
        }

        return parm.AsValueString() ;
      }
      catch {
        return string.Empty ;
      }
    }

    /// <summary>
    /// Doubleパラメーターの取得
    /// </summary>
    /// <param name="doc">ドキュメント</param>
    /// <param name="CreatedID">シンボルID</param>
    /// <param name="paramName">パラメーター名</param>
    /// <returns></returns>
    public static double GetParameterDouble( Document doc, ElementId CreatedID, string paramName )
    {
      try {
        Parameter parm = doc.GetElement( CreatedID ).LookupParameter( paramName ) ;
        if ( parm == null ) {
          return 0.0 ;
        }

        return parm.AsDouble() ;
      }
      catch {
        return 0.0 ;
      }
    }

    public static int GetDan( string dan )
    {
      switch ( dan ) {
        case "上段" :
        {
          return 1 ;
        }
        case "下段" :
        {
          return 2 ;
        }
        default :
        {
          return 0 ;
        }
      }
    }


    public bool CommandTest_Kurane( UIApplication uniapp )
    {
      UIDocument uidoc = uniapp.ActiveUIDocument ;
      Document doc = uidoc.Document ;

      Selection selection = uidoc.Selection ;
      Reference pickedElement = selection.PickObject( ObjectType.Element, "test" ) ;
      ElementId id = pickedElement.ElementId ;


      List<ElementId> Ids = ClsRevitUtil.GetIntersectFamilys( doc, id ) ;
      ClsRevitUtil.SelectElement( uidoc, Ids ) ;
      List<ElementId> Ids2 = new List<ElementId>() ;
      //ClsRevitUtil.CheckCollision2(doc, id, Ids,ref Ids2);

      //ClsRevitUtil.SelectElement(uidoc, Ids2);

      //FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
      //if (inst == null)
      //{
      //    return false;
      //}

      //Element element = doc.GetElement(inst.Id);
      //ElementId type =  element.GetTypeId();

      //using (Transaction tx = new Transaction(doc, "Load Family"))
      //{
      //    tx.Start();


      //    tx.Commit();
      //}
      return true ;
    }

    /// <summary>
    /// 出力ファミリタイプ一覧のCSVを出力する
    /// </summary>
    /// <param name="uniapp"></param>
    /// <param name="needHinmeiCode">出力CSVの各行の末尾に1つ目のタイプの品名コードが必要か</param>
    /// <returns></returns>
    public bool CommandTest_Kurane2( UIApplication uniapp, bool needHinmeiCode )
    {
      UIDocument uidoc = uniapp.ActiveUIDocument ;
      Document doc = uidoc.Document ;

      #region サンプル処理

      // //図面上の端部部品をすべて取得
      // List<ElementId> allInsIds = ClsKariKouzai.GetTanbubuhin(doc);

      // //カバープレートを選択する
      // Selection selection = uidoc.Selection;
      // Reference pickedElement = selection.PickObject(ObjectType.Element, "1つ目のファミリを選択");
      // ElementId id = pickedElement.ElementId;
      // FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
      // if (inst == null)
      // {
      //     return false;
      // }
      // List<ElementId> hitIds = new List<ElementId>();

      // Autodesk.Revit.DB.Transform ts = inst.GetTotalTransform();


      // //ts = ts.Inverse;


      // BoundingBoxXYZ bb = inst.get_BoundingBox(null);
      // XYZ XminYminZmin = new XYZ(bb.Min.X, bb.Min.Y, bb.Min.Z);
      // XYZ XminYminZmax = new XYZ(bb.Min.X, bb.Min.Y, bb.Max.Z);
      // XYZ XmaxYminZmin = new XYZ(bb.Max.X, bb.Min.Y, bb.Min.Z);
      // XYZ XmaxYminZmax = new XYZ(bb.Max.X, bb.Min.Y, bb.Max.Z);
      // XYZ XminYmaxZmin = new XYZ(bb.Min.X, bb.Max.Y, bb.Min.Z);
      // XYZ XminYmaxZmax = new XYZ(bb.Min.X, bb.Max.Y, bb.Max.Z);
      // XYZ XmaxYmaxZmin = new XYZ(bb.Max.X, bb.Max.Y, bb.Min.Z);
      // XYZ XmaxYmaxZmax = new XYZ(bb.Max.X, bb.Max.Y, bb.Max.Z);

      // //XYZ XminYminZmin2 = ts.OfPoint(XminYminZmin);
      // //XYZ XminYminZmax2 = ts.OfPoint(XminYminZmax);
      // //XYZ XmaxYminZmin2 = ts.OfPoint(XmaxYminZmin);
      // //XYZ XmaxYminZmax2 = ts.OfPoint(XmaxYminZmax);
      // //XYZ XminYmaxZmin2 = ts.OfPoint(XminYmaxZmin);
      // //XYZ XminYmaxZmax2 = ts.OfPoint(XminYmaxZmax);
      // //XYZ XmaxYmaxZmin2 = ts.OfPoint(XmaxYmaxZmin);
      // //XYZ XmaxYmaxZmax2 = ts.OfPoint(XmaxYmaxZmax);

      // XYZ XminYminZmin2 = ClsGeo.TransformPoint(XminYminZmin,ts);
      // XYZ XminYminZmax2 = ClsGeo.TransformPoint(XminYminZmax, ts);
      // XYZ XmaxYminZmin2 = ClsGeo.TransformPoint(XmaxYminZmin, ts);
      // XYZ XmaxYminZmax2 = ClsGeo.TransformPoint(XmaxYminZmax, ts);
      // XYZ XminYmaxZmin2 = ClsGeo.TransformPoint(XminYmaxZmin, ts);
      // XYZ XminYmaxZmax2 = ClsGeo.TransformPoint(XminYmaxZmax, ts);
      // XYZ XmaxYmaxZmin2 = ClsGeo.TransformPoint(XmaxYmaxZmin, ts);
      // XYZ XmaxYmaxZmax2 = ClsGeo.TransformPoint(XmaxYmaxZmax, ts);

      // var cv1 = Line.CreateBound(XminYminZmin, XminYminZmax);
      // var cv2 = Line.CreateBound(XminYminZmin, XmaxYminZmin);
      // var cv3 = Line.CreateBound(XminYminZmax, XmaxYminZmax);
      // var cv4 = Line.CreateBound(XmaxYminZmin, XmaxYminZmax);

      // var cv5 = Line.CreateBound(XminYmaxZmin, XminYminZmax);
      // var cv6 = Line.CreateBound(XminYmaxZmin, XmaxYminZmin);
      // var cv7 = Line.CreateBound(XminYmaxZmax, XmaxYminZmax);
      // var cv8 = Line.CreateBound(XmaxYmaxZmin, XmaxYminZmax);

      // var cv9 = Line.CreateBound(XminYminZmin2, XmaxYminZmin2);
      // var cv10 = Line.CreateBound(XminYminZmax2, XmaxYminZmax2);
      // var cv11 = Line.CreateBound(XminYmaxZmin2, XmaxYmaxZmin2);
      // var cv12 = Line.CreateBound(XminYmaxZmax2, XmaxYmaxZmax2);


      //// SketchPlane sketchPlane = SketchPlane.Create(doc, plane);
      // using (Transaction tx = new Transaction(doc, "Load Family"))
      // {
      //     tx.Start();

      //     //ModelLine modelLine1 = doc.Create.NewModelCurve(cv1, sketchPlane) as ModelLine;
      //     //ModelLine modelLine2 = doc.Create.NewModelCurve(cv2, sketchPlane) as ModelLine;
      //     //ModelLine modelLine3 = doc.Create.NewModelCurve(cv3, sketchPlane) as ModelLine;
      //     //ModelLine modelLine4 = doc.Create.NewModelCurve(cv4, sketchPlane) as ModelLine;
      //     //ModelLine modelLine5 = doc.Create.NewModelCurve(cv5, sketchPlane) as ModelLine;
      //     //ModelLine modelLine6 = doc.Create.NewModelCurve(cv6, sketchPlane) as ModelLine;
      //     //ModelLine modelLine7 = doc.Create.NewModelCurve(cv7, sketchPlane) as ModelLine;
      //     //ModelLine modelLine8 = doc.Create.NewModelCurve(cv8, sketchPlane) as ModelLine;
      //     XYZ mid = 0.5 * (XminYminZmin2 + XmaxYminZmin2);
      //     Plane plane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, mid);
      //     SketchPlane sketchPlane = SketchPlane.Create(doc, plane);
      //     ModelLine modelLine9 = doc.Create.NewModelCurve(cv9, sketchPlane) as ModelLine;

      //     mid = 0.5 * (XminYminZmax2 + XmaxYminZmax2);
      //     plane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, mid);
      //     sketchPlane = SketchPlane.Create(doc, plane);
      //     ModelLine modelLine10 = doc.Create.NewModelCurve(cv10, sketchPlane) as ModelLine;

      //     mid = 0.5 * (XminYmaxZmin2 + XmaxYmaxZmin2);
      //     plane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, mid);
      //     sketchPlane = SketchPlane.Create(doc, plane);
      //     ModelLine modelLine11 = doc.Create.NewModelCurve(cv11, sketchPlane) as ModelLine;

      //     mid = 0.5 * (XminYmaxZmax2 + XmaxYmaxZmax2);
      //     plane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, mid);
      //     sketchPlane = SketchPlane.Create(doc, plane);
      //     ModelLine modelLine12 = doc.Create.NewModelCurve(cv12, sketchPlane) as ModelLine;

      //     tx.Commit();
      // }


      ////inst.GetTotalTransform
      //int bb = 0;

      //Selection selection2 = uidoc.Selection;
      //Reference pickedElement2 = selection.PickObject(ObjectType.Element, "2つ目のファミリを選択");
      //ElementId id2 = pickedElement2.ElementId;


      //Element elem = doc.GetElement(id);
      //Element elem2 = doc.GetElement(id2);

      //var sym = elem as FamilySymbol;
      //var sym2 = elem2 as FamilySymbol;

      //Family fam1 = sym.Family;
      //Family fam2 = sym2.Family;

      //List<Element> vIds = GetVoidsInFamily(fam1);

      //List<Element> vIds2 = GetVoidsInFamily(fam2);

      //foreach (Element etmp in vIds)
      //{
      //    FamilyInstance instance = etmp as FamilyInstance;
      //    if (instance != null && instance.Symbol.FamilyName.Contains("Void"))
      //    {
      //        // 壁に関連するヴォイドを取得
      //        LocationPoint location = instance.Location as LocationPoint;
      //        if (location != null && location.Point != null)
      //        {
      //            foreach (Element etmp2 in vIds2)
      //            {
      //                FamilyInstance instance2 = etmp as FamilyInstance;
      //                if (instance2 != null && instance2.Symbol.FamilyName.Contains("Void"))
      //                {
      //                    // 壁に関連するヴォイドを取得
      //                    LocationPoint location2 = instance2.Location as LocationPoint;
      //                    if (location2 != null && location2.Point != null)
      //                    {
      //                        double dist = location2.Point.DistanceTo(location.Point);

      //                        double rd = ClsRevitUtil.CovertFromAPI(dist);
      //                        if(rd < 5)
      //                        {
      //                           var cv = Line.CreateBound(location2.Point, location.Point);
      //                            Plane plane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, location2.Point);
      //                            SketchPlane sketchPlane = SketchPlane.Create(doc, plane);
      //                            using (Transaction tx = new Transaction(doc, "Load Family"))
      //                            {
      //                                tx.Start();
      //                                ModelLine modelLine3 = doc.Create.NewModelCurve(cv, sketchPlane) as ModelLine;
      //                                tx.Commit();
      //                            }
      //                            break;
      //                        }
      //                    }


      //                }

      //            }
      //        }
      //    }


      //}


      ////カバープレートと端部部品たちの衝突チェックを行う
      //if (ClsRevitUtil.CheckCollision(doc, id,ref hitIds))
      //{

      //    //ClsRevitUtil.SelectElement(uidoc, hitIds);
      //    MessageBox.Show("接触");
      //    ////カバープレート削除
      //    //using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
      //    //{
      //    //    t.Start();
      //    //    ClsRevitUtil.Delete(doc, id);
      //    //    t.Commit();
      //    //}

      //}
      //else
      //{
      //    //MessageBox.Show("非接触");
      //}

      #endregion

      #region ファミリタイプ一覧の取得処理(メッセージボックスで結果を表示)

      ////ファミリタイプ一覧
      //FamilyInstance element = doc.GetElement(pickedElement) as FamilyInstance;
      //string filePath = GetSymbolPath(element.Symbol.FamilyName + ".rfa");
      //string symbolName = ClsRevitUtil.GetFamilyName(filePath);
      //if (!ClsRevitUtil.RoadFamilyData(doc, filePath, symbolName, out Family family))
      //{
      //    MessageBox.Show("ファミリの取得に失敗しました");
      //    return false;
      //}

      //Autodesk.Revit.ApplicationServices.Application app = uniapp.Application;
      //Document elDoc = doc.EditFamily(family);

      //if (elDoc.IsFamilyDocument)
      //{
      //    FamilyManager familyManager = elDoc.FamilyManager;

      //    // get types in family
      //    string types = "ファミリタイプ一覧: ";
      //    FamilyTypeSet familyTypes = familyManager.Types;
      //    FamilyTypeSetIterator familyTypesItor = familyTypes.ForwardIterator();
      //    familyTypesItor.Reset();
      //    while (familyTypesItor.MoveNext())
      //    {
      //        FamilyType familyType = familyTypesItor.Current as FamilyType;
      //        types += "\n" + familyType.Name;
      //    }
      //    TaskDialog.Show("Revit", types);
      //}

      #endregion

      #region csv出力先の選択処理

      //SaveFileDialogクラスのインスタンスを作成
      SaveFileDialog sfd = new SaveFileDialog() ;

      //タイトルを設定する
      string csvType = needHinmeiCode ? "出力ファミリタイプ&品名コード一覧" : "出力ファミリタイプ一覧" ;
      sfd.Title = csvType + "の保存先を指定" ;
      //はじめのファイル名を指定する
      //はじめに「ファイル名」で表示される文字列を指定する
      string csvName = csvType + ".csv" ;
      sfd.FileName = csvName ;
      //[ファイルの種類]に表示される選択肢を指定する
      //指定しない（空の文字列）の時は、現在のディレクトリが表示される
      sfd.Filter = "csvファイル(*.csv)|*.csv" ;
      //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
      //sfd.RestoreDirectory = true;
      //既に存在するファイル名を指定したとき警告する
      //デフォルトでTrueなので指定する必要はない
      sfd.OverwritePrompt = true ;
      //存在しないパスが指定されたとき警告を表示する
      //デフォルトでTrueなので指定する必要はない
      sfd.CheckPathExists = true ;

      string ymsFolderPath = ClsZumenInfo.GetYMSFolder() ;
      string outputCsvPath = System.IO.Path.Combine( ymsFolderPath, csvName ) ; //仮のパス（ユーザー選択によって更新される）
      //ダイアログを表示する
      if ( sfd.ShowDialog() == DialogResult.OK ) {
        //OKボタンがクリックされたとき、選択されたファイル名を表示する
        outputCsvPath = sfd.FileName ;
      }
      else {
        return true ;
      }

      #endregion

      #region ファミリタイプ一覧表(csv形式)の出力処理

      //
      //ファミリタイプ一覧表(csv形式)の出力処理
      //出力時間は7分～8分 出力時間が長いため、コメントアウトしてコミットする
      List<string> dt = new List<string>() ;
      string[] targetFolders = { "構台関係", "山留壁関係", "支保工関係" } ;
      foreach ( string targetFolder in targetFolders ) {
        string path = System.IO.Path.Combine( ymsFolderPath, targetFolder ) ;
        string[] allSubFolders =
          System.IO.Directory.GetDirectories( path, "*", System.IO.SearchOption.AllDirectories ) ;
        List<string> targetSubFolders = new List<string>() ;
        foreach ( string subFolder in allSubFolders ) {
          string dirName = new DirectoryInfo( subFolder ).Name ;
          if ( dirName == "子ﾌｧﾐﾘ" ) {
            continue ;
          }

          if ( dirName == "孫ﾌｧﾐﾘ" ) {
            continue ;
          }

          targetSubFolders.Add( subFolder ) ;
        }

        var dtsort = new Dictionary<string, int>() ;
        int sortKeyForNotHasNum = 10000 ; //サブフォルダ名に数字がないためにフォルダ名でのソート判定ができないものは、最後部にする（10000個もサブフォルダはないという前提）
        foreach ( string subFolder in targetSubFolders ) {
          string[] files = Directory.GetFiles( subFolder, "*.rfa" ) ;
          if ( files.Length == 0 ) {
            continue ;
          }

          foreach ( string file in files ) {
            int index = file.LastIndexOf( targetFolder ) + targetFolder.Length ;
            string types = file.Substring( index, file.Length - index ) ;

            int sortKey = -1 ;
            index = types.IndexOf( "_" ) ;
            if ( index != -1 ) {
              string txtKey = types.Substring( 1, index - 1 ) ;
              int.TryParse( txtKey, out sortKey ) ;
            }
            else {
              sortKey = sortKeyForNotHasNum ;
              sortKeyForNotHasNum++ ;
            }

            types = "\\" + targetFolder + types ;
            if ( ! ClsRevitUtil.LoadFamilyData( doc, file, out Family family ) ) {
              dtsort.Add( types + ",ファミリ取得に失敗", sortKey ) ;
              continue ;
            }

            Autodesk.Revit.ApplicationServices.Application app = uniapp.Application ;
            Document elDoc = doc.EditFamily( family ) ;

            if ( elDoc.IsFamilyDocument ) {
              FamilyManager familyManager = elDoc.FamilyManager ;

              FamilyTypeSet familyTypes = familyManager.Types ;
              FamilyTypeSetIterator familyTypesItor = familyTypes.ForwardIterator() ;
              familyTypesItor.Reset() ;
              var familyTypeList = new List<string>() ; //タイプ名のソート用
              var ftAndhcDic = new Dictionary<string, string>() ; //タイプ名と品名コードのディクショナリ
              //「品名コード」パラメータを取得
              FamilyParameter parameter = familyManager.get_Parameter( "品名コード" ) ;
              while ( familyTypesItor.MoveNext() ) {
                //ファミリタイプを取得
                FamilyType familyType = familyTypesItor.Current as FamilyType ;
                familyTypeList.Add( familyType.Name ) ;

                if ( parameter != null ) {
                  // パラメータの値を取得
                  var parameterValue = familyType.AsString( parameter ) ;
                  //                           //パラメータの値に改行があるケースあり
                  //                           int indexOfKaigyo = parameterValue.IndexOf("\r\n");
                  //                           if (indexOfKaigyo != -1)
                  //{
                  //                               parameterValue = parameterValue.Substring(0, indexOfKaigyo - 1);
                  //                           }
                  ftAndhcDic[ familyType.Name ] = parameterValue ;
                }
              }

              //タイプ名ソート
              familyTypeList.Sort() ;

              foreach ( string txt in familyTypeList ) {
                types += "," + txt ;
              }

              //「品名コード」の出力が必要な時
              if ( needHinmeiCode && familyTypeList.Count > 0 ) {
                //ソート後の1つ目のタイプ名の「品名コード」を取得
                string txt = ftAndhcDic.ContainsKey( familyTypeList[ 0 ] ) ? ftAndhcDic[ familyTypeList[ 0 ] ] : "" ;
                types += "," + txt ;
              }

              dtsort.Add( types, sortKey ) ;
            }
          }
        }

        //フォルダごとに出力順をソート
        IOrderedEnumerable<KeyValuePair<string, int>> sortedDt = dtsort.OrderBy( pair => pair.Key ) ;
        foreach ( var data in sortedDt ) {
          dt.Add( data.Key ) ;
        }
      }

      ConvertDataTableToCsv( dt, outputCsvPath ) ;

      #endregion

      return true ;
    }

    /// <summary>
    /// CSV出力
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="csvPath"></param>
    public void ConvertDataTableToCsv( List<string> dt, string csvPath )
    {
      //CSVファイルに書き込むときに使うEncoding
      System.Text.Encoding enc = System.Text.Encoding.GetEncoding( "Shift_JIS" ) ;

      //書き込むファイルを開く
      System.IO.StreamWriter sr = new System.IO.StreamWriter( csvPath, false, enc ) ;

      //レコードを書き込む
      foreach ( string row in dt ) {
        //string cb = "\\支保工関係\\" + row; 
        string cb = row ;
        sr.Write( cb ) ;
        //改行する
        sr.Write( "\r\n" ) ;
      }

      //閉じる
      sr.Close() ;
    }

    // ファミリ内のヴォイドを取得するメソッド
    public List<Element> GetVoidsInFamily( Family family )
    {
      List<Element> voids = new List<Element>() ;

      // ファミリ内のすべての要素を取得
      FilteredElementCollector collector = new FilteredElementCollector( family.Document ) ;
      ICollection<Element> elements = collector.OfCategory( BuiltInCategory.OST_GenericModel )
        .OfClass( typeof( FamilyInstance ) ).ToElements() ;

      foreach ( Element element in elements ) {
        FamilyInstance instance = element as FamilyInstance ;
        if ( instance != null ) {
          // ヴォイドかどうかをチェック
          if ( instance.Symbol.FamilyName.Contains( "Void" ) ) {
            voids.Add( instance ) ;
          }
        }
      }

      return voids ;
    }

    public static bool CheckIrizumi( UIDocument uidoc )
    {
      Document doc = uidoc.Document ;

      ElementId m_HaraokoshiBaseID1 = null ;
      ElementId m_HaraokoshiBaseID2 = null ;
      if ( m_HaraokoshiBaseID1 == null && m_HaraokoshiBaseID2 == null ) {
        ElementId id1 = null ;
        if ( ! ClsHaraokoshiBase.PickBaseObject( uidoc, ref id1,
              "隅火打ベースを配置する腹起ベース　1本目" ) ) //ClsRevitUtil.PickObject(uidoc, "隅火打ベースを配置する腹起ベース　1本目", "腹起ベース", ref id1))
        {
          return false ;
        }

        m_HaraokoshiBaseID1 = id1 ;

        ElementId id2 = null ;
        if ( ! ClsHaraokoshiBase.PickBaseObject( uidoc, ref id2,
              "隅火打ベースを配置する腹起ベース　2本目" ) ) //ClsRevitUtil.PickObject(uidoc, "隅火打ベースを配置する腹起ベース　2本目", "腹起ベース", ref id2))
        {
          return false ;
        }

        m_HaraokoshiBaseID2 = id2 ;
      }

      Element inst = doc.GetElement( m_HaraokoshiBaseID1 ) ;
      FamilyInstance ShinInstLevel = doc.GetElement( m_HaraokoshiBaseID1 ) as FamilyInstance ;
      LocationCurve lCurve = inst.Location as LocationCurve ;
      if ( lCurve == null ) {
        return false ;
      }

      XYZ tmpStPoint = lCurve.Curve.GetEndPoint( 0 ) ;
      XYZ tmpEdPoint = lCurve.Curve.GetEndPoint( 1 ) ;

      Element inst2 = doc.GetElement( m_HaraokoshiBaseID2 ) ;
      FamilyInstance ShinInstLevel2 = doc.GetElement( m_HaraokoshiBaseID2 ) as FamilyInstance ;
      LocationCurve lCurve2 = inst2.Location as LocationCurve ;
      if ( lCurve2 == null ) {
        return false ;
      }

      XYZ tmpStPoint2 = lCurve2.Curve.GetEndPoint( 0 ) ;
      XYZ tmpEdPoint2 = lCurve2.Curve.GetEndPoint( 1 ) ;

      //入隅か判定
      bool bIrizumi = false ;
      if ( ! Parts.ClsHaraokoshiBase.CheckIrizumi( lCurve.Curve, lCurve2.Curve, ref bIrizumi ) ) {
        MessageBox.Show( "コーナーを取得できませんでした。" ) ;
        return false ;
      }

      if ( ! bIrizumi ) {
        MessageBox.Show( "このコーナーは出隅です" ) ;
        return false ;
      }

      MessageBox.Show( "このコーナーは入隅です" ) ;

      return true ;
    }


    public static bool ObjectReverse( UIDocument uidoc )
    {
      Document doc = uidoc.Document ;

      //ClsCornerHiuchiBase c = new ClsCornerHiuchiBase();
      //DLG.DlgCreateCornerHiuchiBase f = new DLG.DlgCreateCornerHiuchiBase(c);
      //f.Show();
      //c = f.m_CornerHiuchiBase;


      ElementId id1 = null ;
      if ( ! ClsCornerHiuchiBase.PickBaseObject( uidoc, ref id1, "隅火打ベースを配置する腹起ベース　1本目" ) ) {
        return false ;
      }

      FamilyInstance ShinInstLevel = doc.GetElement( id1 ) as FamilyInstance ;

      Element inst = doc.GetElement( id1 ) ;
      LocationCurve lCurve = inst.Location as LocationCurve ;
      if ( lCurve == null ) {
        return false ;
      }

      ClsCornerHiuchiBase chb = new ClsCornerHiuchiBase() ;
      chb.SetParameter( doc, id1 ) ;

      //chb.CreateTanbuParts(doc);

      //           XYZ tmpStPoint = lCurve.Curve.GetEndPoint(0);
      //           XYZ tmpEdPoint = lCurve.Curve.GetEndPoint(1);

      //           string shinfamily = Master.ClsHiuchiCsv.GetFamilyPath(c.m_HiuchiUkePieceSize1);
      //           string shinfamilyName = RevitUtil.ClsRevitUtil.GetFamilyName(shinfamily);


      //           using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
      //           {
      //               //シンボル読込
      //               if (!ClsRevitUtil.RoadFamilySymbolData(doc, shinfamily, shinfamilyName, out FamilySymbol sym))
      //               {
      //                   return false;
      //               }

      //t.Start();

      //               FamilyInstance lv = doc.GetElement(id1) as FamilyInstance;
      //               ElementId levelID = lv.Host.Id;

      //               ElementId CreatedID = ClsRevitUtil.Create(doc, tmpStPoint, levelID, sym);


      //               FamilyInstance cr = doc.GetElement(CreatedID) as FamilyInstance;
      //               cr.flipFacing();


      //               //FamilyInstance sanjikuInst = doc.GetElement(CreatedID) as FamilyInstance;
      //               ////XYZ sanjikuPoint = (sanjikuInst.Location as LocationPoint).Point;
      //               //XYZ dirV = sanjikuInst.FacingOrientation;//ファミリインスタンスの向き
      //               //XYZ dirB = sanjikuInst.HandOrientation;//ファミリインスタンスの側面

      //               //XYZ directionKiri = Line.CreateBound(cvKiri.GetEndPoint(0), cvKiri.GetEndPoint(1)).Direction;
      //               //XYZ directionHara = Line.CreateBound(cvHara.GetEndPoint(0), cvHara.GetEndPoint(1)).Direction;

      //               //double dBaseAngle = dirB.AngleTo(directionHara);
      //               //double dVectAngle = dirV.AngleTo(directionKiri);
      //               //double dAngle = -dVectAngle;//時計回りになるように-をかける

      //               Line axis = Line.CreateBound(tmpStPoint, tmpStPoint + XYZ.BasisZ);
      //               //ClsRevitUtil.RotateElement(doc, CreatedID, axis, dAngle);

      //               t.Commit();
      //           }
      return true ;
    }
  } //ClsTest

  public class SelectionFilter : ISelectionFilter
  {
    Document m_doc = null ;

    public SelectionFilter( Document doc )
    {
      m_doc = doc ;
    }

    public bool AllowElement( Element element )
    {
      if ( ClsTest.IsShin( m_doc, element.Id ) ) {
        return true ;
      }

      return false ;
    }

    public bool AllowReference( Reference refer, XYZ point )
    {
      return false ;
    }
  }

  public class FamilySelectionFilter : ISelectionFilter
  {
    string m_famName = null ;

    public FamilySelectionFilter( string famName )
    {
      m_famName = famName ;
    }

    public bool AllowElement( Element element )
    {
      if ( element.Name == m_famName ) {
        return true ;
      }

      return false ;
    }

    public bool AllowReference( Reference refer, XYZ point )
    {
      return false ;
    }
  }

  public class FamilySelectionListFilter : ISelectionFilter
  {
    List<string> m_famNameList = null ;

    public FamilySelectionListFilter( List<string> famNameList )
    {
      m_famNameList = famNameList ;
    }

    public bool AllowElement( Element element )
    {
      for ( int i = 0 ; i < m_famNameList.Count ; i++ ) {
        if ( element.Name == m_famNameList[ i ] ) {
          return true ;
        }
      }

      return false ;
    }

    public bool AllowReference( Reference refer, XYZ point )
    {
      return false ;
    }
  }
}