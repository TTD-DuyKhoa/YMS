using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using Autodesk.Revit.UI.Selection ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using YMS_gantry.Material ;
using YMS_gantry.UI ;

namespace YMS_gantry.Command
{
  class CmdCreateTesuri
  {
    static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger() ;
    Document _doc { get ; set ; }
    UIDocument _uiDoc { get ; set ; }

    public CmdCreateTesuri( UIDocument uiDoc )
    {
      _uiDoc = uiDoc ;
      _doc = _uiDoc.Document ;
    }

    /// <summary>
    /// 実行
    /// </summary>
    /// <returns></returns>
    public bool Excute()
    {
      try {
        //FrmPutTesuriTesuriShichu f = new FrmPutTesuriTesuriShichu(_uiDoc.Application);
        //if (f.ShowDialog() != System.Windows.Forms.DialogResult.OK) { return false; }
        FrmPutTesuriTesuriShichu f = Application.thisApp.frmPutTesuriTesuriShichu ;

        Curve l = null ;
        Level level = null ;
        double offset = 0 ;
        FamilyInstance fam = null ;
        Face topFace = null ;
        double baseOffset = 0 ;

        //部材選択
        if ( f.RbtFace.Checked ) {
          //SelectionFilterUtil pickFilter = new SelectionFilterUtil(_uiDoc, new List<string> { "根太","主桁","地覆"});
          //pickFilter.Select("配置する部材を選択してください",out fam);
          //topFace = GantryUtil.GetTopFaceOfFamilyInstance(fam);
          YMSPickFilter filter =
            new YMSPickFilter( _uiDoc.Document, new List<string> { "根太", "主桁", "地覆" }, isFace: true ) ;
          Reference refer = _uiDoc.Selection.PickObject( Autodesk.Revit.UI.Selection.ObjectType.Face, filter,
            "配置したい部材の面を指定してください" ) ;
          fam = _doc.GetElement( refer.ElementId ) as FamilyInstance ;
          topFace = GantryUtil.IdentifyFaceOfFamilyInstance( fam, refer ) ;
          if ( topFace == null ) {
            return false ;
          }

          List<XYZ> pnts = GantryUtil.GetCurvePoints( _doc, fam.Id ) ;
          if ( pnts.Count != 2 ) {
            return false ;
          }

          pnts[ 0 ] = GantryUtil.ProjectPoint( topFace.Triangulate().Vertices as List<XYZ>, pnts[ 0 ] ) ;
          pnts[ 1 ] = GantryUtil.ProjectPoint( topFace.Triangulate().Vertices as List<XYZ>, pnts[ 1 ] ) ;

          l = Line.CreateBound( pnts[ 0 ], pnts[ 1 ] ) ;
          MaterialSize ms = GantryUtil.GetKouzaiSize( fam.Symbol.FamilyName ) ;
          baseOffset = ms.Height / 2 ;
          level = GantryUtil.GetInstanceLevelAndOffset( _doc, fam, ref offset ) ;
        }
        //2点指定
        else {
          if ( f.CmbLevel.Text == "部材選択" ) {
            ElementId id = _uiDoc.Selection.PickObject( ObjectType.Element, "基準となる部材を選択してください" ).ElementId ;
            FamilyInstance ins = _doc.GetElement( id ) as FamilyInstance ;
            level = GantryUtil.GetInstanceLevelAndOffset( _doc, ins, ref offset ) ;
            offset = offset + (double) f.NmcOffset.Value ;
          }
          else {
            level = RevitUtil.ClsRevitUtil.GetLevel( _doc, f.CmbLevel.Text ) as Level ;
            offset = (double) f.NmcOffset.Value ;
          }

          XYZ p1, p2 ;
          ( p1, p2 ) = RevitUtil.ClsRevitUtil.GetSelect2Point( _uiDoc ) ;
          if ( p1 == null || p2 == null ) {
            return false ;
          }

          l = Line.CreateBound( p1, p2 ) ;
        }

        XYZ vecL = l.GetEndPoint( 1 ) - l.GetEndPoint( 0 ) ;
        XYZ orVec = vecL.CrossProduct( XYZ.BasisZ ).Normalize() ;

        //手摺の向きを指定
        XYZ pntTesuri = _uiDoc.Selection.PickPoint( "手摺の向く方向を指定してください" ) ;
        bool isLeft = RevitUtil.ClsGeo.IsLeft( vecL, ( pntTesuri - l.GetEndPoint( 0 ) ).Normalize() ) ;
        XYZ vecT = orVec ;
        if ( ( isLeft && ! RevitUtil.ClsGeo.IsLeft( vecL, orVec ) ) ||
             ( ! isLeft && RevitUtil.ClsGeo.IsLeft( vecL, orVec ) ) ) {
          orVec = orVec.Negate() ;
        }

        //全長をスパンで割る
        int wholeLeng = (int) Math.Abs( RevitUtil.ClsRevitUtil.CovertFromAPI( l.Length ) ) ;
        int spanCnt = (int) Math.Floor( wholeLeng / f.NmcBasePitch.Value ) ;
        //割り切れるけれど割り切らない場合
        if ( f.ChkTsukidashiKakuho.Checked && ( wholeLeng % spanCnt == 0 ) ) {
          spanCnt = spanCnt - 1 ;
        }

        //手摺支柱
        int danC = (int) f.NmcDanCount.Value ;
        int pitch = (int) f.NmcDanPitch.Value ;
        int headDiff = (int) f.NmcTesuriLng.Value ;
        int length = (int) f.NmcHight.Value ;

        using ( Transaction tr = new Transaction( _doc ) ) {
          tr.Start( "Placement Tesuri" ) ;
          double off = f.RbtFace.Checked ? 0 : offset ;
          Reference refer = f.RbtFace.Checked ? topFace.Reference : level.GetPlaneReference() ;

          //手摺支柱
          List<XYZ> listSichuPnts = calcSichuPnts( wholeLeng, spanCnt, (int) f.NmcBasePitch.Value, l ) ;
          TesuriShichu tesuriShichu = new TesuriShichu( null, f.CmbKoudaiName.Text, f.CmbTesurishichuMaterial.Text,
            f.CmbTesurishichuSize.Text ) ;
          XYZ vecNorm = ( isLeft ) ? vecL.Negate().Normalize() : vecL.Normalize() ;
          FamilySymbol sym ;
          string familyPath =
            Master.ClsTesuriCsv.GetFamilyPath( f.CmbTesurishichuSize.Text, Master.ClsTesuriCsv.TypeTesuriShichu ) ;
          string familyName = System.IO.Path.GetFileNameWithoutExtension( familyPath ) ;
          if ( GantryUtil.GetFamilySymbol( _doc, familyPath, familyName, out sym, true ) ) {
            sym = GantryUtil.DuplicateTypeWithNameRule( _doc, f.CmbKoudaiName.Text, sym, familyName ) ;
          }

          foreach ( XYZ pnt in listSichuPnts ) {
            XYZ p = pnt + orVec.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 75 ) ;
            tesuriShichu.m_ElementId = tesuriShichu.CreateTesuriShichu( _doc, f.CmbKoudaiName.Text, p,
              f.CmbTesurishichuSize.Text, f.CmbTesurishichuMaterial.Text, refer, off, (double) length, vecNorm, sym ) ;
            TesuriShichu.WriteToElement( tesuriShichu, _doc ) ;
          }

          //手摺
          double rota = 180 ;
          if ( ! isLeft ) {
            l = Line.CreateBound( l.GetEndPoint( 1 ), l.GetEndPoint( 0 ) ) ;
            //rota = -90;
          }

          l = Line.CreateBound(
            l.GetEndPoint( 0 ) + orVec.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 75 ),
            l.GetEndPoint( 1 ) + orVec.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 75 ) ) ;
          List<double> tesuriList = calcTesuriPnts( danC, pitch, headDiff, length, l, f.RbtFace.Checked ? 0 : off ) ;
          Tesuri tesuri = new Tesuri( null, f.CmbKoudaiName.Text, f.CmbTesuriMaterial.Text, f.CmbTesuriSize.Text ) ;

          familyPath = Master.ClsTesuriCsv.GetFamilyPath( f.CmbTesuriSize.Text, Master.ClsTesuriCsv.TypeTesuri ) ;
          if ( GantryUtil.GetFamilySymbol( _doc, familyPath, Master.ClsTesuriCsv.TypeTesuri, out sym, true ) ) {
            sym = GantryUtil.DuplicateTypeWithNameRule( _doc, f.CmbKoudaiName.Text, sym,
              Master.ClsTesuriCsv.TypeTesuri ) ;
          }

          foreach ( double dOff in tesuriList ) {
            double o = dOff + ( f.RbtFace.Checked ? 0 : off ) ;
            tesuri.m_ElementId = tesuri.CreateTesuri( _doc, f.CmbKoudaiName.Text, l, f.CmbTesuriSize.Text,
              f.CmbTesuriMaterial.Text, refer, RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( o ), rota, sym ) ;
            Tesuri.WriteToElement( tesuri, _doc ) ;
          }

          tr.Commit() ;
        }
      }
      catch ( Autodesk.Revit.Exceptions.OperationCanceledException ) {
      }
      catch ( Exception ex ) {
        _logger.Error( ex.Message ) ;
        return false ;
      }

      return true ;
    }

    private List<XYZ> calcSichuPnts( int wholeLeng, int spanCnt, int baseSpan, Curve c )
    {
      List<XYZ> retPnts = new List<XYZ>() ;
      XYZ start = c.GetEndPoint( 0 ) ;
      XYZ end = c.GetEndPoint( 1 ) ;
      XYZ vec = ( end - start ).Normalize() ;
      double exLeng = wholeLeng - ( spanCnt * baseSpan ) ;
      retPnts.Add( start + vec * ( RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( exLeng / 2 ) ) ) ;
      for ( int i = 1 ; i <= spanCnt ; i++ ) {
        retPnts.Add( retPnts[ 0 ] + vec * ( RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( baseSpan * i ) ) ) ;
      }

      return retPnts ;
    }

    private List<double> calcTesuriPnts( int danCnt, int pitch, int headDiff, int length, Curve c,
      double baseOffset = 0 )
    {
      List<double> retOffsets = new List<double>() ;
      //XYZ vecNorm = GantoryUtil.GetNegativeZVectorFromCurve(_doc, c).Negate();

      //XYZ start = c.GetEndPoint(0) + vecNorm * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(length-headDiff + baseOffset);
      //XYZ end = c.GetEndPoint(1) + vecNorm * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(length - headDiff + baseOffset);

      //for (int i = 1; i <= danCnt; i++)
      //{
      //    XYZ p1 = start + vecNorm.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(pitch * i);
      //    XYZ p2 = end + vecNorm.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(pitch * i);
      //    retPnts.Add(Line.CreateBound(p1,p2));
      //}
      retOffsets.Add( length - headDiff ) ;
      for ( int dan = 1 ; dan < danCnt ; dan++ ) {
        retOffsets.Add( retOffsets[ retOffsets.Count - 1 ] - pitch ) ;
      }

      return retOffsets ;
    }
  }
}