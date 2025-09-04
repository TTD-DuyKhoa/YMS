using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using Autodesk.Revit.UI.Selection ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;
using YMS_gantry.UI ;
using static YMS_gantry.DefineUtil ;
using YMS_gantry.Material ;

namespace YMS_gantry.Command
{
  class CmdCreateStiffener
  {
    static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger() ;
    Document _doc { get ; set ; }
    UIDocument _uiDoc { get ; set ; }

    public CmdCreateStiffener( UIDocument uiDoc )
    {
      _uiDoc = uiDoc ;
      _doc = _uiDoc.Document ;
    }

    /// <summary>
    /// 実行
    /// </summary>
    /// <returns></returns>
    public Result Excute()
    {
      Selection selection = _uiDoc.Selection ;

      //FrmPutSuchifuna frm = new FrmPutSuchifuna(_uiDoc.Application);
      //if (frm.ShowDialog() != DialogResult.OK) { return Result.Cancelled; }
      FrmPutSuchifuna frm = Application.thisApp.frmPutSuchifuna ;

      bool isNeda = frm.ChkNeda.Checked ;
      List<string> filterList = new List<string>() ;
      if ( frm.ChkNeda.Checked ) {
        filterList.Add( "根太" ) ;
        filterList.Add( "主桁" ) ;
      }

      if ( frm.ChkOhbiki.Checked ) {
        filterList.Add( "大引" ) ;
        filterList.Add( "桁受" ) ;
      }

      if ( frm.ChkShikigeta.Checked ) {
        filterList.Add( "敷桁" ) ;
      }

      try {
        YMSPickFilter pickFilter = new YMSPickFilter( _uiDoc.Document, filterList ) ;

        //自動配置
        if ( frm.RbtAuto.Checked ) {
          //範囲選択
          List<Element> materilaPicks = selection.PickElementsByRectangle( pickFilter, "対象とする部材を範囲選択してください" ).ToList() ;
          List<ElementId> ids = materilaPicks.Select( x => x.Id ).ToList() ;
          MaterialSuper[] materials = MaterialSuper.Collect( _doc )
            .Where( x => ids.Contains( x.m_ElementId ) && x.m_KodaiName == frm.CmbKoudaiName.Text ).ToArray() ;
          if ( materials.Length <= 0 ) {
            MessageUtil.Warning( "指定した構台の部材が選択されていません", "スチフナー配置" ) ;
            return Result.Cancelled ;
          }

          List<ElementId> ohbikiId = MaterialSuper.Collect( _doc ).Where( x => x.m_ElementId != null &&
                                                                               x.m_KodaiName ==
                                                                               frm.CmbKoudaiName.Text &&
                                                                               ( x.GetType()
                                                                                   .Equals( typeof( Ohbiki ) ) ||
                                                                                 x.GetType().Equals(
                                                                                   typeof( Shikigeta ) ) ) )
            .Select( x => x.m_ElementId ).ToList() ;

          List<ElementId> nedaId = MaterialSuper.Collect( _doc ).Where( x => x.m_ElementId != null &&
                                                                             x.m_KodaiName == frm.CmbKoudaiName.Text &&
                                                                             x.GetType().Equals( typeof( Neda ) ) )
            .Select( x => x.m_ElementId ).ToList() ;

          List<ElementId> pileId = MaterialSuper.Collect( _doc ).Where( x => x.m_ElementId != null &&
                                                                             x.m_KodaiName == frm.CmbKoudaiName.Text &&
                                                                             x.GetType().Equals(
                                                                               typeof( PilePiller ) ) )
            .Select( x => x.m_ElementId ).ToList() ;

          using ( Transaction tr = new Transaction( _doc ) ) {
            tr.Start( "Stiffener Palcement" ) ;
            //根太
            List<Neda> nedas = materials.Select( x => x as Neda ).Where( x => x != null ).ToList() ;
            if ( ohbikiId.Count > 0 ) {
              foreach ( Neda neda in nedas ) {
                List<ElementId> intsId =
                  RevitUtil.ClsRevitUtil.GetIntersectFamilys( _doc, neda.m_ElementId, serchIds: ohbikiId ) ;
                List<ElementId> ohbikiIds = intsId.Where( x => ohbikiId.Contains( x ) ).ToList() ;
                Curve c = GantryUtil.GetCurve( _doc, neda.m_ElementId ) ;
                List<XYZ> intPnts = CalcCurveIntersection( c, ohbikiIds ) ;

                FamilyInstance ins = _doc.GetElement( neda.m_ElementId ) as FamilyInstance ;
                Transform ts = ins.GetTransform() ;
                string path, type ;
                ( path, type ) = getKouzaiStiffenerPath( neda, frm.CmbSizeType.Text ) ;
                if ( path == "" || type == "" ) {
                  MessageUtil.Warning( "配置サイズに対応するスチフナーが自動で判別できませんでした。\r\n" + $"根太:{neda.m_Size}", "スチフナー配置", frm ) ;
                  break ;
                }

                FamilySymbol sym ;
                if ( ! GantryUtil.GetFamilySymbol( _doc, path, type, out sym, true ) ) {
                  _logger.Warn( $"ファミリロードに失敗しました{path}:{type}" ) ;
                  continue ;
                }

                if ( ! sym.IsActive ) {
                  sym.Activate() ;
                }

                MaterialSize ms = GantryUtil.GetKouzaiSize( neda.m_Size ) ;
                Face f = GantryUtil.GetTopFaceOfFamilyInstance( ins ) ;
                double horOff = ( ms.Thick / 2 ) + 1 ;
                foreach ( XYZ pnt in intPnts ) {
                  XYZ norm = ( c.GetEndPoint( 1 ) - c.GetEndPoint( 0 ) ).Normalize() ;

                  if ( frm.CmbSizeType.Text == "プレート" ) {
                    if ( pnt.DistanceTo( c.GetEndPoint( 0 ) ) < RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 10 ) ) {
                      XYZ point = c.GetEndPoint( 0 ) +
                                  ts.BasisY.Negate().Normalize() *
                                  RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( horOff ) +
                                  norm * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 10 ) ;
                      CreateStiffener( frm.CmbKoudaiName.Text, f, point, norm, sym, sym.Name, -ms.Height / 2 ) ;
                      point = c.GetEndPoint( 0 ) +
                              ts.BasisY.Normalize() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( horOff ) +
                              norm * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 10 ) ;
                      CreateStiffener( frm.CmbKoudaiName.Text, f, point, norm.Negate(), sym, sym.Name,
                        -ms.Height / 2 ) ;
                    }
                    else if ( pnt.DistanceTo( c.GetEndPoint( 1 ) ) <
                              RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 10 ) ) {
                      norm = ( c.GetEndPoint( 0 ) - c.GetEndPoint( 1 ) ).Normalize() ;
                      XYZ point = c.GetEndPoint( 1 ) +
                                  ts.BasisY.Normalize() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( horOff ) +
                                  norm * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 10 ) ;
                      CreateStiffener( frm.CmbKoudaiName.Text, f, point, norm, sym, sym.Name, -ms.Height / 2 ) ;
                      point = c.GetEndPoint( 1 ) +
                              ts.BasisY.Negate().Normalize() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( horOff ) +
                              norm * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 10 ) ;
                      CreateStiffener( frm.CmbKoudaiName.Text, f, point, norm.Negate(), sym, sym.Name,
                        -ms.Height / 2 ) ;
                    }
                    else {
                      XYZ point = pnt + ts.BasisY.Negate().Normalize() *
                        RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( horOff ) ;
                      CreateStiffener( frm.CmbKoudaiName.Text, f, point, norm, sym, sym.Name, -ms.Height / 2 ) ;
                      point = pnt + ts.BasisY.Normalize() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( horOff ) ;
                      CreateStiffener( frm.CmbKoudaiName.Text, f, point, norm.Negate(), sym, sym.Name,
                        -ms.Height / 2 ) ;
                    }
                  }
                  else {
                    horOff = ( ms.Width / 2 ) ;
                    string jSize = System.IO.Path.GetFileNameWithoutExtension( path ).Replace( "スチフナージャッキ_", "" )
                      .Replace( "_V", "" ) ;
                    double jPL = jSize.Contains( "DWJ" ) ? jSize.Contains( "50" ) ? 100 : 200 :
                      jSize.Contains( "30" ) ? 175 : 75 ;
                    bool isLeft = RevitUtil.ClsGeo.IsLeft( norm, ts.BasisY ) ;
                    XYZ lineD = norm ;
                    norm = jSize.Contains( "DWJ" ) ? ts.BasisY : ts.BasisY.Negate() ;
                    double off = ( jSize.Contains( "SH-30" ) ? ( isLeft ) ? 125 : 50 : jPL / 2 ) ;
                    double off2 = ( jSize.Contains( "SH-30" ) ? ( isLeft ) ? 50 : 125 : jPL / 2 ) ;
                    SteelSize sS = neda.SteelSize ;

                    if ( pnt.DistanceTo( c.GetEndPoint( 0 ) ) < RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 10 ) ) {
                      XYZ point = c.GetEndPoint( 0 ) +
                                  ts.BasisY.Negate().Normalize() *
                                  RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( horOff ) +
                                  lineD * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( off ) ;
                      CreateStiffener( frm.CmbKoudaiName.Text, f, point, norm, sym, sym.Name,
                        -ms.Height + sS.FrangeThick, true ) ;
                      point = c.GetEndPoint( 0 ) +
                              ts.BasisY.Normalize() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( horOff ) +
                              lineD * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( off2 ) ;
                      CreateStiffener( frm.CmbKoudaiName.Text, f, point, norm.Negate(), sym, sym.Name,
                        -ms.Height + sS.FrangeThick, true ) ;
                    }
                    else if ( pnt.DistanceTo( c.GetEndPoint( 1 ) ) <
                              RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 10 ) ) {
                      XYZ point = c.GetEndPoint( 1 ) +
                                  ts.BasisY.Normalize() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( horOff ) +
                                  lineD.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( off2 ) ;
                      CreateStiffener( frm.CmbKoudaiName.Text, f, point, norm, sym, sym.Name,
                        -ms.Height + sS.FrangeThick, true ) ;
                      point = c.GetEndPoint( 1 ) +
                              ts.BasisY.Negate().Normalize() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( horOff ) +
                              lineD.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( off ) ;
                      CreateStiffener( frm.CmbKoudaiName.Text, f, point, norm.Negate(), sym, sym.Name,
                        -ms.Height + +sS.FrangeThick, true ) ;
                    }
                    else {
                      XYZ point = pnt + ts.BasisY.Negate().Normalize() *
                        RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( horOff ) ;
                      CreateStiffener( frm.CmbKoudaiName.Text, f, point, norm, sym, sym.Name,
                        -ms.Height + sS.FrangeThick, true ) ;
                      point = pnt + ts.BasisY.Normalize() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( horOff ) ;
                      CreateStiffener( frm.CmbKoudaiName.Text, f, point, norm.Negate(), sym, sym.Name,
                        -ms.Height + sS.FrangeThick, true ) ;
                    }
                  }
                }
              }
            }

            //大引　敷桁
            List<MaterialSuper> ohbikis = materials.Where( x =>
              x != null && ( x.GetType() == typeof( Ohbiki ) || x.GetType() == typeof( Shikigeta ) ) ).ToList() ;
            if ( ohbikiId.Count > 0 ) {
              foreach ( MaterialSuper ohbiki in ohbikis ) {
                List<ElementId> intsId =
                  RevitUtil.ClsRevitUtil.GetIntersectFamilys( _doc, ohbiki.m_ElementId, serchIds: nedaId ) ;
                List<ElementId> nedaIds = intsId.Where( x => nedaId.Contains( x ) ).ToList() ;

                Curve c = GantryUtil.GetCurve( _doc, ohbiki.m_ElementId ) ;
                List<XYZ> intPnts = CalcCurveIntersection( c, nedaIds ) ;
                if ( typeof( Shikigeta ) != ohbiki.GetType() ) {
                  foreach ( XYZ p in CalcPntIntersection( c, pileId ) ) {
                    bool isIn = false ;
                    foreach ( XYZ x in intPnts ) {
                      if ( RevitUtil.ClsGeo.GEO_EQ( x, p ) ) {
                        isIn = true ;
                        break ;
                      }
                    }

                    if ( ! isIn ) {
                      intPnts.Add( p ) ;
                    }
                  }
                }

                intPnts = intPnts.Distinct().ToList() ;
                FamilyInstance ins = _doc.GetElement( ohbiki.m_ElementId ) as FamilyInstance ;
                Transform ts = ins.GetTransform() ;
                MaterialSize ms = GantryUtil.GetKouzaiSize( ohbiki.m_Size ) ;
                string path, type ;
                ( path, type ) = getKouzaiStiffenerPath( ohbiki, frm.CmbSizeType.Text ) ;
                if ( path == "" || type == "" ) {
                  MessageUtil.Warning( "配置サイズに対応するスチフナーが自動で判別できませんでした。\r\n" + $"桁材:{ohbiki.m_Size}", "スチフナー配置", frm ) ;
                  break ;
                }

                FamilySymbol sym ;
                if ( ! GantryUtil.GetFamilySymbol( _doc, path, type, out sym, true ) ) {
                  _logger.Warn( $"ファミリロードに失敗しました{path}:{type}" ) ;
                  return Result.Cancelled ;
                }

                if ( ! sym.IsActive ) {
                  sym.Activate() ;
                }

                Face f = GantryUtil.GetTopFaceOfFamilyInstance( ins ) ;
                double horOff = ( ms.Thick / 2 ) + 1 ;

                foreach ( XYZ pnt in intPnts ) {
                  XYZ norm = ( c.GetEndPoint( 1 ) - c.GetEndPoint( 0 ) ).Normalize() ;
                  if ( frm.CmbSizeType.Text == "プレート" ) {
                    if ( pnt.DistanceTo( c.GetEndPoint( 0 ) ) < RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 10 ) ) {
                      XYZ point = c.GetEndPoint( 0 ) +
                                  ts.BasisY.Negate().Normalize() *
                                  RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( horOff ) +
                                  norm * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 10 ) ;
                      if ( ms.Shape == MaterialShape.H ) {
                        CreateStiffener( frm.CmbKoudaiName.Text, f, point, norm, sym, sym.Name, -ms.Height / 2 ) ;
                      }

                      point = c.GetEndPoint( 0 ) +
                              ts.BasisY.Normalize() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( horOff ) +
                              norm * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 10 ) ;
                      CreateStiffener( frm.CmbKoudaiName.Text, f, point, norm.Negate(), sym, sym.Name,
                        -ms.Height / 2 ) ;
                    }
                    else if ( pnt.DistanceTo( c.GetEndPoint( 1 ) ) <
                              RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 10 ) ) {
                      norm = ( c.GetEndPoint( 0 ) - c.GetEndPoint( 1 ) ).Normalize() ;
                      XYZ point = c.GetEndPoint( 1 ) +
                                  ts.BasisY.Normalize() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( horOff ) +
                                  norm * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 10 ) ;
                      if ( ms.Shape == MaterialShape.H ) {
                        CreateStiffener( frm.CmbKoudaiName.Text, f, point, norm, sym, sym.Name, -ms.Height / 2 ) ;
                      }

                      point = c.GetEndPoint( 1 ) +
                              ts.BasisY.Negate().Normalize() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( horOff ) +
                              norm * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 10 ) ;
                      CreateStiffener( frm.CmbKoudaiName.Text, f, point, norm.Negate(), sym, sym.Name,
                        -ms.Height / 2 ) ;
                    }
                    else {
                      XYZ point = pnt + ts.BasisY.Negate().Normalize() *
                        RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( horOff ) ;
                      if ( ms.Shape == MaterialShape.H ) {
                        CreateStiffener( frm.CmbKoudaiName.Text, f, point, norm, sym, sym.Name, -ms.Height / 2 ) ;
                      }

                      point = pnt + ts.BasisY.Normalize() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( horOff ) ;
                      CreateStiffener( frm.CmbKoudaiName.Text, f, point, norm.Negate(), sym, sym.Name,
                        -ms.Height / 2 ) ;
                    }
                  }
                  else {
                    horOff = ( ms.Width / 2 ) ;
                    string jSize = System.IO.Path.GetFileNameWithoutExtension( path ).Replace( "スチフナージャッキ_", "" )
                      .Replace( "_V", "" ) ;
                    double jPL = jSize.Contains( "DWJ" ) ? jSize.Contains( "50" ) ? 100 : 200 :
                      jSize.Contains( "30" ) ? 175 : 75 ;
                    bool isLeft = RevitUtil.ClsGeo.IsLeft( norm, ts.BasisY ) ;
                    XYZ lineD = norm ;
                    norm = jSize.Contains( "DWJ" ) ? ts.BasisY : ts.BasisY.Negate() ;
                    double off = ( jSize.Contains( "SH-30" ) ? ( isLeft ) ? 125 : 50 : jPL / 2 ) ;
                    double off2 = ( jSize.Contains( "SH-30" ) ? ( isLeft ) ? 50 : 125 : jPL / 2 ) ;
                    SteelSize ss = ohbiki.SteelSize ;

                    if ( pnt.DistanceTo( c.GetEndPoint( 0 ) ) < RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 10 ) ) {
                      XYZ point = c.GetEndPoint( 0 ) +
                                  ts.BasisY.Negate().Normalize() *
                                  RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( horOff ) +
                                  lineD * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( off ) ;
                      if ( ms.Shape == MaterialShape.H ) {
                        CreateStiffener( frm.CmbKoudaiName.Text, f, point, norm, sym, sym.Name,
                          -ms.Height + ss.FrangeThick, true ) ;
                      }

                      point = c.GetEndPoint( 0 ) +
                              ts.BasisY.Normalize() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( horOff ) +
                              lineD * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( off2 ) ;
                      CreateStiffener( frm.CmbKoudaiName.Text, f, point, norm.Negate(), sym, sym.Name,
                        -ms.Height + ss.FrangeThick, true ) ;
                    }
                    else if ( pnt.DistanceTo( c.GetEndPoint( 1 ) ) <
                              RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 10 ) ) {
                      XYZ point = c.GetEndPoint( 1 ) +
                                  ts.BasisY.Normalize() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( horOff ) +
                                  lineD.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( off2 ) ;
                      if ( ms.Shape == MaterialShape.H ) {
                        CreateStiffener( frm.CmbKoudaiName.Text, f, point, norm, sym, sym.Name,
                          -ms.Height + ss.FrangeThick, true ) ;
                      }

                      point = c.GetEndPoint( 1 ) +
                              ts.BasisY.Negate().Normalize() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( horOff ) +
                              lineD.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( off ) ;
                      CreateStiffener( frm.CmbKoudaiName.Text, f, point, norm.Negate(), sym, sym.Name,
                        -ms.Height + ss.FrangeThick, true ) ;
                    }
                    else {
                      XYZ point = pnt + ts.BasisY.Negate().Normalize() *
                        RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( horOff ) ;
                      if ( ms.Shape == MaterialShape.H ) {
                        CreateStiffener( frm.CmbKoudaiName.Text, f, point, norm, sym, sym.Name,
                          -ms.Height + ss.FrangeThick, true ) ;
                      }

                      point = pnt + ts.BasisY.Normalize() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( horOff ) ;
                      CreateStiffener( frm.CmbKoudaiName.Text, f, point, norm.Negate(), sym, sym.Name,
                        -ms.Height + ss.FrangeThick, true ) ;
                    }
                  }
                }
              }

              tr.Commit() ;
            }
          }

          MessageUtil.Information( "スチフナーの自動配置が完了しました", "スチフナー配置" ) ;
        }
        //任意配置
        else {
          bool end = false ;
          YMSPickFilter filter = new YMSPickFilter( _doc, new List<string>
          {
            "根太",
            "主桁",
            "大引",
            "桁受",
            "敷桁"
          } ) ;
          using ( Transaction tr = new Transaction( _doc ) ) {
            while ( ! end ) {
              tr.Start( "StiffenerPlacement" ) ;
              //配置対象部材を指定
              ElementId pickId = selection.PickObject( ObjectType.Element, filter, "配置する部材を選択してください" ).ElementId ;
              if ( pickId == ElementId.InvalidElementId ) {
                continue ;
              }

              MaterialSuper material = MaterialSuper.ReadFromElement( pickId, _doc ) ;
              if ( material.m_Size == "" ) {
                continue ;
              }

              RevitUtil.ClsRevitUtil.SelectElement( _uiDoc, pickId ) ;

              XYZ pnt = selection.PickPoint( ObjectSnapTypes.Nearest, "スチフナー配置点を指定" ) ;
              Curve c = GantryUtil.GetCurve( _doc, pickId ) ;
              XYZ norm = ( c.GetEndPoint( 1 ) - c.GetEndPoint( 0 ) ).Normalize() ;
              FamilyInstance ins = _doc.GetElement( pickId ) as FamilyInstance ;
              Transform ts = ins.GetTransform() ;
              string path, type ;
              path = Master.ClsStiffenerCsv.GetFamilyPath( frm.CmbSize.Text ) ;
              type = Master.ClsStiffenerCsv.GetTypeWithSize( frm.CmbSize.Text, frm.CmbSizeType.Text ) ;
              FamilySymbol sym ;
              Element host = ( ins.Host == null ) ? ins : ins.Host ;
              Face f = GantryUtil.GetTopFaceOfFamilyInstance( ins ) ;
              if ( ! GantryUtil.GetFamilySymbol( _doc, path, type, out sym, true ) ) {
                MessageUtil.Warning( "指定したサイズのファミリが取得できませんでした。", "スチフナー配置" ) ;
                return Result.Cancelled ;
              }

              if ( ! sym.IsActive ) {
                sym.Activate() ;
              }

              MaterialSize ms = material.MaterialSize() ;
              pnt = GantryUtil.GetClosestPointOnVector( c.GetEndPoint( 1 ) - c.GetEndPoint( 0 ), c.GetEndPoint( 0 ),
                pnt ) ;
              double horOff = ( ms.Thick / 2 ) + 1 ;


              if ( frm.CmbSizeType.Text.Contains( "プレート" ) ) {
                if ( pnt.DistanceTo( c.GetEndPoint( 0 ) ) < RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 10 ) ) {
                  XYZ point = c.GetEndPoint( 0 ) +
                              ts.BasisY.Negate().Normalize() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( horOff ) +
                              norm * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 10 ) ;
                  if ( ms.Shape == MaterialShape.H ) {
                    CreateStiffener( frm.CmbKoudaiName.Text, f, point, norm, sym, sym.Name, -ms.Height / 2 ) ;
                  }

                  point = c.GetEndPoint( 0 ) +
                          ts.BasisY.Normalize() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( horOff ) +
                          norm * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 10 ) ;
                  CreateStiffener( frm.CmbKoudaiName.Text, f, point, norm.Negate(), sym, sym.Name, -ms.Height / 2 ) ;
                }
                else if ( pnt.DistanceTo( c.GetEndPoint( 1 ) ) < RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 10 ) ) {
                  norm = ( c.GetEndPoint( 0 ) - c.GetEndPoint( 1 ) ).Normalize() ;
                  XYZ point = pnt + ts.BasisY.Normalize() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( horOff ) +
                              norm * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 10 ) ;
                  if ( ms.Shape == MaterialShape.H ) {
                    CreateStiffener( frm.CmbKoudaiName.Text, f, point, norm, sym, sym.Name, -ms.Height / 2 ) ;
                  }

                  point = pnt +
                          ts.BasisY.Negate().Normalize() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( horOff ) +
                          norm * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 10 ) ;
                  CreateStiffener( frm.CmbKoudaiName.Text, f, point, norm.Negate(), sym, sym.Name, -ms.Height / 2 ) ;
                }
                else {
                  XYZ point = pnt + ts.BasisY.Negate().Normalize() *
                    RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( horOff ) ;
                  if ( ms.Shape == MaterialShape.H ) {
                    CreateStiffener( frm.CmbKoudaiName.Text, f, point, norm, sym, sym.Name, -ms.Height / 2 ) ;
                  }

                  point = pnt + ts.BasisY.Normalize() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( horOff ) ;
                  CreateStiffener( frm.CmbKoudaiName.Text, f, point, norm.Negate(), sym, sym.Name, -ms.Height / 2 ) ;
                }
              }
              else {
                horOff = ( ms.Width / 2 ) ;
                string jSize = System.IO.Path.GetFileNameWithoutExtension( path ).Replace( "スチフナージャッキ_", "" )
                  .Replace( "_V", "" ) ;
                double jPL = jSize.Contains( "DWJ" ) ? jSize.Contains( "50" ) ? 100 : 200 :
                  jSize.Contains( "30" ) ? 175 : 75 ;
                bool isLeft = RevitUtil.ClsGeo.IsLeft( norm, ts.BasisY ) ;
                XYZ lineD = norm ;
                norm = jSize.Contains( "DWJ" ) ? ts.BasisY : ts.BasisY.Negate() ;
                double off = ( jSize.Contains( "SH-30" ) ? ( isLeft ) ? 125 : 50 : jPL / 2 ) ;
                double off2 = ( jSize.Contains( "SH-30" ) ? ( isLeft ) ? 50 : 125 : jPL / 2 ) ;
                SteelSize ss = material.SteelSize ;

                if ( pnt.DistanceTo( c.GetEndPoint( 0 ) ) < RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 10 ) ) {
                  XYZ point = c.GetEndPoint( 0 ) +
                              ts.BasisY.Negate().Normalize() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( horOff ) +
                              lineD * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( off ) ;
                  if ( ms.Shape == MaterialShape.H ) {
                    CreateStiffener( frm.CmbKoudaiName.Text, f, point, norm, sym, sym.Name, -ms.Height + ss.FrangeThick,
                      true ) ;
                  }

                  point = c.GetEndPoint( 0 ) +
                          ts.BasisY.Normalize() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( horOff ) +
                          lineD * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( off2 ) ;
                  CreateStiffener( frm.CmbKoudaiName.Text, f, point, norm.Negate(), sym, sym.Name,
                    -ms.Height + ss.FrangeThick, true ) ;
                }
                else if ( pnt.DistanceTo( c.GetEndPoint( 1 ) ) < RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 10 ) ) {
                  XYZ point = c.GetEndPoint( 1 ) +
                              ts.BasisY.Normalize() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( horOff ) +
                              lineD.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( off2 ) ;
                  if ( ms.Shape == MaterialShape.H ) {
                    CreateStiffener( frm.CmbKoudaiName.Text, f, point, norm, sym, sym.Name, -ms.Height + ss.FrangeThick,
                      true ) ;
                  }

                  point = c.GetEndPoint( 1 ) +
                          ts.BasisY.Negate().Normalize() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( horOff ) +
                          lineD.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( off ) ;
                  CreateStiffener( frm.CmbKoudaiName.Text, f, point, norm.Negate(), sym, sym.Name,
                    -ms.Height + ss.FrangeThick, true ) ;
                }
                else {
                  XYZ point = pnt + ts.BasisY.Negate().Normalize() *
                    RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( horOff ) ;
                  if ( ms.Shape == MaterialShape.H ) {
                    CreateStiffener( frm.CmbKoudaiName.Text, f, point, norm, sym, sym.Name, -ms.Height + ss.FrangeThick,
                      true ) ;
                  }

                  point = pnt + ts.BasisY.Normalize() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( horOff ) ;
                  CreateStiffener( frm.CmbKoudaiName.Text, f, point, norm.Negate(), sym, sym.Name,
                    -ms.Height + ss.FrangeThick, true ) ;
                }
              }

              tr.Commit() ;
            }
          }
        }

        return Result.Succeeded ;
      }
      catch ( Autodesk.Revit.Exceptions.OperationCanceledException ) {
        return Result.Succeeded ;
      }
      catch ( Exception ex ) {
        _logger.Error( ex.Message ) ;
        return Result.Failed ;
      }
    }

    private void CreateStiffener( string koudaiName, Face f, XYZ point, XYZ normal, FamilySymbol sym, string size,
      double offset, bool isJack = false )
    {
      FamilyInstance ins = _doc.Create.NewFamilyInstance( f, point, normal, sym ) ;
      if ( ins == null ) {
        return ;
      }

      RevitUtil.ClsRevitUtil.SetParameter( _doc, ins.Id, DefineUtil.PARAM_HOST_OFFSET,
        RevitUtil.ClsRevitUtil.CovertToAPI( offset ) ) ;

      if ( isJack ) {
        var data = new StiffenerJack
        {
          m_Document = _doc, m_ElementId = ins.Id, m_Size = size, m_KodaiName = koudaiName,
        } ;
        MaterialSuper.WriteToElement( data, _doc ) ;
      }
      else {
        var data = new StiffenerPlate
        {
          m_Document = _doc, m_ElementId = ins.Id, m_Size = size, m_KodaiName = koudaiName,
        } ;
        MaterialSuper.WriteToElement( data, _doc ) ;
      }
    }

    #region "交点算出関数"

    private List<XYZ> CalcCurveIntersection( Curve c, List<ElementId> ids )
    {
      List<XYZ> list = new List<XYZ>() ;
      try {
        foreach ( ElementId id in ids ) {
          Curve tC = GantryUtil.GetCurve( _doc, id ) ;
          XYZ intPnt = FindIntersectionPoint( c, tC ) ;
          if ( intPnt != null ) {
            if ( HasListLocation( list, intPnt ) ) {
              continue ;
            }

            list.Add( intPnt ) ;
          }
        }
      }
      catch ( Exception ) {
      }

      return list ;
    }

    private bool HasListLocation( List<XYZ> list, XYZ target )
    {
      foreach ( XYZ p in list ) {
        if ( RevitUtil.ClsRevitUtil.CovertFromAPI( p.DistanceTo( target ) ) < 0.1 ) {
          return true ;
        }
      }

      return false ;
    }

    public XYZ FindIntersectionPoint( Curve curve1, Curve curve2 )
    {
      // Curve1とCurve2のXY平面上での投影を取得
      Curve projection1 = ProjectToXYPlane( curve1 ) ;
      Curve projection2 = ProjectToXYPlane( curve2 ) ;

      // 投影が交わるかどうかを調べる
      IntersectionResultArray intersectionResults = new IntersectionResultArray() ;
      SetComparisonResult result = projection1.Intersect( projection2, out intersectionResults ) ;

      if ( result == SetComparisonResult.Overlap && intersectionResults.Size > 0 ) {
        // 交点を取得
        XYZ intersectionPoint = intersectionResults.get_Item( 0 ).XYZPoint ;

        // 1つ目のCurve上に射影
        XYZ projectedPoint = GantryUtil.ProjectPointToCurve( intersectionPoint, curve1 ) ;

        return projectedPoint ;
      }

      return null ;
    }

    private Curve ProjectToXYPlane( Curve curve )
    {
      // CurveをXY平面に投影
      XYZ startPoint = curve.GetEndPoint( 0 ) ;
      XYZ endPoint = curve.GetEndPoint( 1 ) ;

      XYZ startPointProjected = new XYZ( startPoint.X, startPoint.Y, 0 ) ;
      XYZ endPointProjected = new XYZ( endPoint.X, endPoint.Y, 0 ) ;

      Line line = Line.CreateBound( startPointProjected, endPointProjected ) ;
      return line ;
    }

    private List<XYZ> CalcPntIntersection( Curve curve, List<ElementId> ids )
    {
      List<XYZ> list = new List<XYZ>() ;
      try {
        foreach ( ElementId id in ids ) {
          FamilyInstance ins = _doc.GetElement( id ) as FamilyInstance ;
          if ( ins == null ) {
            continue ;
          }

          XYZ pnt = ( ins.Location as LocationPoint ).Point ;
          XYZ intPnt = GantryUtil.ProjectPointToCurve( pnt, curve ) ;
          if ( intPnt != null && ! list.Contains( intPnt ) ) {
            list.Add( intPnt ) ;
          }
        }
      }
      catch ( Exception ) {
      }

      return list ;
    }

    private (string, string) getKouzaiStiffenerPath( MaterialSuper ms, string selectedType )
    {
      string retPath = "" ;
      string retType = "" ;
      string kouzaiSize = ms.m_Size ;
      MaterialSize mtSize = ms.MaterialSize() ;
      if ( mtSize.Shape == MaterialShape.HA || mtSize.Shape == MaterialShape.SMH ) {
        kouzaiSize = Master.ClsKouzaiSpecify.GetKouzaiSize( kouzaiSize ) ;
      }

      retPath = Master.ClsStiffenerCsv.GetFamilyPath( kouzaiSize, selectedType ) ;
      retType = Master.ClsStiffenerCsv.GetType( kouzaiSize, selectedType ) ;

      return ( retPath, retType ) ;
    }

    #endregion
  }
}