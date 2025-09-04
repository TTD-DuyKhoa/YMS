using Autodesk.Revit.DB ;
using Autodesk.Revit.DB.Structure ;
using Autodesk.Revit.UI ;
using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.IO ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using YMS_gantry.Data ;
using YMS_gantry.UI ;
using static YMS_gantry.DefineUtil ;

namespace YMS_gantry
{
  [MaterialCategory( "外形線" )]
  public sealed class OutLine : MaterialSuper
  {
    public static new string Name = "OutLine" ;

    public OutLine() : base()
    {
    }

    public List<ElementId> CreateOutLine( ModelData md, string groupname )
    {
      Document doc = m_Document ;

      GroupData gd = md.GetGroupData( groupname ) ;

      List<ElementId> lstLineId = new List<ElementId>() ;

      //構台情報リストを取得
      List<string> lstKoudaiName = gd.ListKoudaiName ;

      //構台上の覆工板のリストを取得
      MaterialSuper[] unionMaterials = MaterialSuper.Collect( doc ) ;
      XYZ firstP = new XYZ() ;
      List<OutLineKoudai> lstOLK = new List<OutLineKoudai>() ;
      foreach ( string koudaiName in lstKoudaiName ) {
        List<ElementId> lstFId = new List<ElementId>() ;
        foreach ( MaterialSuper ms in unionMaterials ) {
          string kn = ms.m_KodaiName ;
          if ( koudaiName != kn ) {
            continue ;
          }

          if ( ms.m_Size == null ) {
            continue ;
          }

          if ( string.IsNullOrEmpty( Master.ClsFukkoubanCsv.ExistSize( ms.m_Size ) ) ) {
            continue ;
          }

          lstFId.Add( ms.m_ElementId ) ;
        }

        //4つ角を精査し、近い順に並べ替え
        List<XYZ> lstP = new List<XYZ>() ;
        List<XYZ> lstCornerP = new List<XYZ>() ;
        foreach ( ElementId id in lstFId ) {
          lstP.AddRange( GetFukkoubanEdge( id ) ) ;
        }

        lstCornerP = SelectCornerPoint( 1, lstP ) ;
        lstP = SelectOutSidePoint( 1, lstP ) ;

        List<XYZ> lstTemp = new List<XYZ>() ;
        foreach ( XYZ p in lstP ) {
          bool add = true ;
          foreach ( XYZ pt in lstTemp ) {
            if ( ClsGeo.GEO_EQ( p.X, pt.X ) && ClsGeo.GEO_EQ( p.Y, pt.Y ) && ClsGeo.GEO_EQ( p.Z, pt.Z ) ) {
              add = false ;
              break ;
            }
          }

          if ( add ) {
            lstTemp.Add( p ) ;
          }
        }

        lstP = lstTemp ;

        List<XYZ> lstSortP = SortNearPoint( lstP ) ;

        //各構造の必要な情報
        List<Curve> lstCurve = new List<Curve>() ;
        for ( int i = 0 ; i < lstSortP.Count - 1 ; i++ ) {
          Curve c = Line.CreateBound( lstSortP[ i ], lstSortP[ i + 1 ] ) ;
          lstCurve.Add( c ) ;
        }

        OutLineKoudai olk = new OutLineKoudai() ;
        olk.koudaiName = koudaiName ;
        olk.lstSortPoint = lstSortP ;
        olk.lstCornerPoint = lstCornerP ;
        olk.lstCurve = lstCurve ;
        lstOLK.Add( olk ) ;

        //名前とラインを書く処理
        ////Lineを作図
        //View view =  doc.ActiveView;

        //// トランザクション開始
        //List<ElementId> lstCurveId = new List<ElementId>();
        //using (Transaction transaction = new Transaction(doc))
        //{
        //    transaction.Start("Create Polyline");
        //    for (int i = 0; i < lstSortP.Count - 1; i++)
        //    {
        //        Curve c = Line.CreateBound(lstSortP[i], lstSortP[i + 1]);
        //        //DetailCurve detailCurve = doc.Create.NewDetailCurve(view, c);
        //        Plane plane = Plane.CreateByThreePoints(lstSortP[i], lstSortP[i + 1], XYZ.Zero);
        //        SketchPlane sketchPlane = SketchPlane.Create(doc, plane);
        //        ModelLine modelLine = doc.Create.NewModelCurve(c, sketchPlane) as ModelLine;
        //        ClsRevitUtil.ChangeLineColor(doc, modelLine.Id, new Color(255, 0, 0));
        //        ClsRevitUtil.ChangeLineWeight(doc, modelLine.Id, 10);

        //        //Autodesk.Revit.DB.ModelCurve modelLine = doc.Create.NewModelCurve(c, SketchPlane.Create(doc, Plane.CreateByNormalAndOrigin(doc.ActiveView.ViewDirection, lstSortP[i])));
        //        //lstCurveId.Add(modelLine.Id);
        //        lstLineId.Add(modelLine.Id);
        //    }
        //    // トランザクション終了
        //    transaction.Commit();
        //}

        //using (Transaction transaction = new Transaction(doc))
        //{
        //    transaction.Start("Create GroupName");
        //    TextNoteOptions options = new TextNoteOptions();
        //    options.KeepRotatedTextReadable = false;
        //    options.TypeId = doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);

        //    Autodesk.Revit.DB.View activeView = doc.ActiveView;
        //    TextNote note = TextNote.Create(doc, activeView.Id, firstP, groupname + "_" + koudaiName, options);
        //    lstLineId.Add(note.Id);

        //    // トランザクション終了
        //    transaction.Commit();
        //}
      }

      //ハンチの情報
      List<ElementId> lstHunchId = gd.GetListHunchElementId() ;
      List<OutLineHunch> lstOLH = new List<OutLineHunch>() ;
      foreach ( ElementId hId in lstHunchId ) {
        OutLineHunch olh = new OutLineHunch() ;
        List<XYZ> lstHunchP = GetHunchEdge( hId ) ;
        olh.lstCornerPoint = lstHunchP ;
        List<Curve> lstCurve = new List<Curve>() ;
        for ( int i = 0 ; i < lstHunchP.Count - 1 ; i++ ) {
          Curve c = Line.CreateBound( lstHunchP[ i ], lstHunchP[ i + 1 ] ) ;
          lstCurve.Add( c ) ;

          if ( lstHunchP.Count - 2 == i ) {
            Curve c2 = Line.CreateBound( lstHunchP[ 0 ], lstHunchP[ i + 1 ] ) ;
            lstCurve.Add( c2 ) ;
          }
        }

        olh.lstCurve = lstCurve ;
        lstOLH.Add( olh ) ;
      }


      List<XYZ> lstPoint = new List<XYZ>() ;
      foreach ( OutLineKoudai olk in lstOLK ) {
        foreach ( XYZ p in olk.lstSortPoint ) {
          //他の構台のエッジに近い場合は除外
          bool bNG = false ;
          foreach ( OutLineKoudai olk2 in lstOLK ) {
            if ( olk.koudaiName == olk2.koudaiName )
              continue ;

            //他の構台のエッジに近い場合は除外
            if ( olk2.IsNearEdge( 1, p ) ) {
              bNG = true ;
              break ;
            }
          }

          foreach ( OutLineHunch olh in lstOLH ) {
            //他の構台のエッジに近い場合は除外
            if ( olh.IsNearEdge( 1, p ) ) {
              bNG = true ;
              break ;
            }
          }

          if ( ! bNG ) {
            lstPoint.Add( p ) ;
          }
        }

        //ハンチのエッジに近い場合は、コーナーポイントも除外
        bool bNGcorner = false ;
        foreach ( XYZ pCorner in olk.lstCornerPoint ) {
          foreach ( OutLineHunch olh in lstOLH ) {
            //他の構台のエッジに近い場合は除外
            if ( olh.IsNearEdge( 1, pCorner ) ) {
              bNGcorner = true ;
              break ;
            }
          }

          if ( ! bNGcorner )
            lstPoint.Add( pCorner ) ;
        }
      }

      //ハンチの2点を追加
      foreach ( OutLineHunch olh in lstOLH ) {
        lstPoint.AddRange( olh.GetOutsidePoint() ) ;
      }

      //重複を削除
      List<XYZ> lstPTemp = new List<XYZ>() ;
      foreach ( XYZ p in lstPoint ) {
        bool bContain = false ;
        foreach ( XYZ pTmp in lstPTemp ) {
          if ( ClsGeo.GEO_EQ( p.X, pTmp.X ) && ClsGeo.GEO_EQ( p.Y, pTmp.Y ) && ClsGeo.GEO_EQ( p.Z, pTmp.Z ) ) {
            bContain = true ;
            break ;
          }
        }

        if ( bContain ) {
          continue ;
        }
        else {
          lstPTemp.Add( p ) ;
        }
      }

      //再ソート
      List<XYZ> lstSortP2 = SortNearPoint( lstPTemp ) ;

      //名前とラインを書く処理
      ////Lineを作図
      View view = doc.ActiveView ;

      // トランザクション開始
      List<ElementId> lstCurveId = new List<ElementId>() ;
      using ( Transaction transaction = new Transaction( doc ) ) {
        transaction.Start( "Create Polyline" ) ;

        // 警告を制御する処理（部材を削除する前に同一点に部材を置くとコミット時に警告が出るので対応）
        FailureHandlingOptions failOpt = transaction.GetFailureHandlingOptions() ;
        failOpt.SetFailuresPreprocessor( new WarningSwallower() ) ;
        transaction.SetFailureHandlingOptions( failOpt ) ;

        for ( int i = 0 ; i < lstSortP2.Count - 1 ; i++ ) {
          Curve c = Line.CreateBound( lstSortP2[ i ], lstSortP2[ i + 1 ] ) ;
          //DetailCurve detailCurve = doc.Create.NewDetailCurve(view, c);
          Plane plane = Plane.CreateByThreePoints( lstSortP2[ i ], lstSortP2[ i + 1 ], XYZ.Zero ) ;
          SketchPlane sketchPlane = SketchPlane.Create( doc, plane ) ;
          ModelLine modelLine = doc.Create.NewModelCurve( c, sketchPlane ) as ModelLine ;
          ClsRevitUtil.ChangeLineColor( doc, modelLine.Id, new Color( 255, 0, 0 ) ) ;
          ClsRevitUtil.ChangeLineWeight( doc, modelLine.Id, 10 ) ;

          //Autodesk.Revit.DB.ModelCurve modelLine = doc.Create.NewModelCurve(c, SketchPlane.Create(doc, Plane.CreateByNormalAndOrigin(doc.ActiveView.ViewDirection, lstSortP[i])));
          //lstCurveId.Add(modelLine.Id);
          lstLineId.Add( modelLine.Id ) ;
        }

        // トランザクション終了
        transaction.Commit() ;
      }

      using ( Transaction transaction = new Transaction( doc ) ) {
        transaction.Start( "Create GroupName" ) ;
        TextNoteOptions options = new TextNoteOptions() ;
        options.KeepRotatedTextReadable = false ;
        options.TypeId = doc.GetDefaultElementTypeId( ElementTypeGroup.TextNoteType ) ;

        Autodesk.Revit.DB.View activeView = doc.ActiveView ;
        TextNote note = TextNote.Create( doc, activeView.Id, lstSortP2[ 0 ], groupname, options ) ;
        lstLineId.Add( note.Id ) ;

        // トランザクション終了
        transaction.Commit() ;
      }

      return lstLineId ;
    }

    public bool DeleteOutLine( ref GroupData gd )
    {
      //全てのアウトラインを削除
      using ( Transaction t = new Transaction( m_Document, Guid.NewGuid().GetHashCode().ToString() ) ) {
        t.Start() ;
        foreach ( ElementId id in gd.GetListLineElementId() ) {
          ClsRevitUtil.Delete( m_Document, id ) ;
        }

        t.Commit() ;
      }

      gd.ListLineId = new List<int>() ;

      return true ;
    }

    /// <summary>
    /// 警告表示の制御(無視できる警告が表示されなくなる)
    /// </summary>
    /// <remarks>下記のように使う
    /// t.Start();
    ///FailureHandlingOptions failOpt = t.GetFailureHandlingOptions();
    ///failOpt.SetFailuresPreprocessor(new WarningSwallower());
    ///t.SetFailureHandlingOptions(failOpt);</remarks>
    public class WarningSwallower : IFailuresPreprocessor
    {
      public FailureProcessingResult PreprocessFailures( FailuresAccessor failuresAccessor )
      {
        IList<FailureMessageAccessor> failList = new List<FailureMessageAccessor>() ;
        failList = failuresAccessor.GetFailureMessages() ;
        foreach ( FailureMessageAccessor failure in failList ) {
          FailureDefinitionId failID = failure.GetFailureDefinitionId() ;
          failuresAccessor.DeleteWarning( failure ) ;
        }

        return FailureProcessingResult.Continue ;
      }
    }

    public List<XYZ> SortNearPoint( List<XYZ> lstP )
    {
      List<XYZ> lstSortP = new List<XYZ>() ;

      XYZ firstP = lstP.First() ;
      lstSortP.Add( firstP ) ;

      List<XYZ> lstBanXYZ = new List<XYZ>() ;
      lstBanXYZ.Add( firstP ) ;
      XYZ bP = firstP ;
      while ( true ) {
        double dMinDis = double.MaxValue ;
        XYZ minP = null ;
        foreach ( XYZ p2 in lstP ) {
          if ( ClsGeo.GEO_EQ( bP.X, p2.X ) && ClsGeo.GEO_EQ( bP.Y, p2.Y ) && ClsGeo.GEO_EQ( bP.Z, p2.Z ) ) {
            continue ;
          }

          bool bBan = false ;
          foreach ( XYZ ban in lstBanXYZ ) {
            if ( ClsGeo.GEO_EQ( p2.X, ban.X ) && ClsGeo.GEO_EQ( p2.Y, ban.Y ) && ClsGeo.GEO_EQ( p2.Z, ban.Z ) ) {
              bBan = true ;
              break ;
            }
          }

          if ( bBan ) continue ;

          double dis = bP.DistanceTo( p2 ) ;
          if ( dMinDis > dis ) {
            dMinDis = dis ;
            minP = p2 ;
          }
        }

        if ( minP == null ) break ;
        lstSortP.Add( minP ) ;
        lstBanXYZ.Add( minP ) ;
        bP = minP ;
      }

      lstSortP.Add( firstP ) ;

      return lstSortP ;
    }

    /// <summary>
    /// 覆工板外側の点を抽出
    /// </summary>
    /// <param name="dBorderDis"></param>
    /// <param name="lstP"></param>
    /// <returns></returns>
    public List<XYZ> SelectOutSidePoint( double dBorderDis, List<XYZ> lstP )
    {
      List<XYZ> lstOutSide = new List<XYZ>() ;
      List<XYZ> lstOutSideS = new List<XYZ>() ;
      List<XYZ> lstOutSideOther = new List<XYZ>() ;
      foreach ( XYZ p in lstP ) {
        int c = ( from data in lstP where data.DistanceTo( p ) < dBorderDis select data ).Count() ;

        if ( ( c == 1 ) ) {
          lstOutSideS.Add( p ) ;
        }
        else if ( ! ( c >= 4 ) ) {
          lstOutSideOther.Add( p ) ;
        }
      }

      //角を先頭にする
      lstOutSide.AddRange( lstOutSideS ) ;
      lstOutSide.AddRange( lstOutSideOther ) ;
      return lstOutSide ;
    }

    /// <summary>
    /// 覆工板外側の点を抽出
    /// </summary>
    /// <param name="dBorderDis"></param>
    /// <param name="lstP"></param>
    /// <returns></returns>
    public List<XYZ> SelectCornerPoint( double dBorderDis, List<XYZ> lstP )
    {
      List<XYZ> lstCorner = new List<XYZ>() ;
      List<XYZ> lstOutSideS = new List<XYZ>() ;
      foreach ( XYZ p in lstP ) {
        int c = ( from data in lstP where data.DistanceTo( p ) < dBorderDis select data ).Count() ;

        if ( ( c == 1 ) ) {
          lstOutSideS.Add( p ) ;
        }
      }

      lstCorner.AddRange( lstOutSideS ) ;
      return lstCorner ;
    }


    public List<XYZ> GetFukkoubanEdge( ElementId id )
    {
      List<XYZ> lstP = new List<XYZ>() ;
      List<XYZ> lstP1 = new List<XYZ>() ;
      EdgeArray elm1EdgeArrays = RevitUtil.ClsRevitUtil.GetEdgeArray( m_Document.GetElement( id ) ) ;
      foreach ( Edge edge in elm1EdgeArrays ) {
        Curve curve = edge.AsCurve() ;
        XYZ curveSP = curve.GetEndPoint( 0 ) ;
        XYZ curveEP = curve.GetEndPoint( 1 ) ;
        lstP1.Add( curveSP ) ;
        lstP1.Add( curveEP ) ;
      }

      List<XYZ> lstBanP = new List<XYZ>() ;
      foreach ( XYZ edge in lstP1 ) {
        foreach ( XYZ banP in lstBanP ) {
          if ( ClsGeo.GEO_EQ( edge.X, banP.X ) && ClsGeo.GEO_EQ( edge.Y, banP.Y ) && ClsGeo.GEO_EQ( edge.Z, banP.Z ) ) {
            continue ;
          }
        }

        double dMin = double.MaxValue ;
        XYZ pMin = null ;
        foreach ( XYZ edge2 in lstP1 ) {
          if ( ClsGeo.GEO_EQ( edge.X, edge2.X ) && ClsGeo.GEO_EQ( edge.Y, edge2.Y ) &&
               ClsGeo.GEO_EQ( edge.Z, edge2.Z ) ) {
            continue ;
          }

          double dDis = edge.DistanceTo( edge2 ) ;
          if ( dMin > dDis ) {
            dMin = dDis ;
            pMin = edge2 ;
          }
        }

        if ( edge.Z > pMin.Z ) {
          lstBanP.Add( pMin ) ;
        }
        else {
          lstBanP.Add( edge ) ;
        }
      }

      foreach ( XYZ edge in lstP1 ) {
        bool bBan = false ;
        foreach ( XYZ banP in lstBanP ) {
          if ( ClsGeo.GEO_EQ( edge.X, banP.X ) && ClsGeo.GEO_EQ( edge.Y, banP.Y ) && ClsGeo.GEO_EQ( edge.Z, banP.Z ) ) {
            bBan = true ;
            break ;
          }
        }

        if ( ! bBan ) {
          if ( ! ( from data in lstP
                where ClsGeo.GEO_EQ( edge.X, data.X ) && ClsGeo.GEO_EQ( edge.Y, data.Y ) &&
                      ClsGeo.GEO_EQ( edge.Z, data.Z )
                select data ).Any() ) {
            lstP.Add( edge ) ;
          }
        }
      }

      return lstP ;
    }


    public List<XYZ> GetHunchEdge( ElementId id )
    {
      List<XYZ> lstP = new List<XYZ>() ;
      List<XYZ> lstP1 = new List<XYZ>() ;
      EdgeArray elm1EdgeArrays = RevitUtil.ClsRevitUtil.GetEdgeArray( m_Document.GetElement( id ) ) ;
      foreach ( Edge edge in elm1EdgeArrays ) {
        Curve curve = edge.AsCurve() ;
        XYZ curveSP = curve.GetEndPoint( 0 ) ;
        XYZ curveEP = curve.GetEndPoint( 1 ) ;
        lstP1.Add( curveSP ) ;
        lstP1.Add( curveEP ) ;
      }

      List<XYZ> lstPTemp = new List<XYZ>() ;
      foreach ( XYZ p in lstP1 ) {
        bool bContain = false ;
        foreach ( XYZ pTmp in lstPTemp ) {
          if ( ClsGeo.GEO_EQ( p.X, pTmp.X ) && ClsGeo.GEO_EQ( p.Y, pTmp.Y ) && ClsGeo.GEO_EQ( p.Z, pTmp.Z ) ) {
            bContain = true ;
            break ;
          }
        }

        if ( bContain ) {
          continue ;
        }
        else {
          lstPTemp.Add( p ) ;
        }
      }

      lstPTemp = lstPTemp.OrderByDescending( x => x.Z ).ToList() ;

      List<XYZ> lstPTemp2 = new List<XYZ>() ;
      lstPTemp2.Add( lstPTemp[ 0 ] ) ;
      lstPTemp2.Add( lstPTemp[ 1 ] ) ;
      lstPTemp2.Add( lstPTemp[ 2 ] ) ;
      lstPTemp2.Add( lstPTemp[ 3 ] ) ;
      lstPTemp2.Add( lstPTemp[ 4 ] ) ;

      return lstPTemp2 ;
    }
  }

  public sealed class OutLineKoudai
  {
    public string koudaiName ;
    public List<XYZ> lstSortPoint ;
    public List<XYZ> lstCornerPoint ;
    public List<Curve> lstCurve ;

    public bool IsNearEdge( double dDis, XYZ p )
    {
      foreach ( Curve c in lstCurve ) {
        if ( c.Distance( p ) < dDis )
          return true ;
      }

      return false ;
    }
  }

  public sealed class OutLineHunch
  {
    public List<XYZ> lstCornerPoint ;
    public List<Curve> lstCurve ;

    public List<XYZ> GetOutsidePoint()
    {
      XYZ CP = new XYZ() ;
      List<XYZ> lstRes = new List<XYZ>() ;
      foreach ( XYZ p in lstCornerPoint ) {
        bool bC = false ;
        foreach ( XYZ p2 in lstCornerPoint ) {
          if ( ClsGeo.GEO_EQ( p.X, p2.X ) && ClsGeo.GEO_EQ( p.Y, p2.Y ) && ClsGeo.GEO_EQ( p.Z, p2.Z ) ) {
            continue ;
          }

          if ( p.DistanceTo( p2 ) < 1 ) {
            bC = true ;
          }
        }

        if ( ! bC )
          CP = p ;
      }

      foreach ( XYZ p in lstCornerPoint ) {
        if ( ClsGeo.GEO_EQ( p.X, CP.X ) && ClsGeo.GEO_EQ( p.Y, CP.Y ) && ClsGeo.GEO_EQ( p.Z, CP.Z ) ) {
          continue ;
        }

        lstRes.Add( p ) ;
      }

      return lstRes ;
    }

    public bool IsNearEdge( double dDis, XYZ p )
    {
      foreach ( Curve c in lstCurve ) {
        if ( c.Distance( p ) < dDis )
          return true ;
      }

      return false ;
    }
  }
}