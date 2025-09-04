using Autodesk.Revit.Creation ;
using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using Autodesk.Revit.UI.Selection ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;
using YMS_gantry.Data ;
using YMS_gantry.Material ;
using YMS_gantry.UI ;

namespace YMS_gantry.Command
{
  class CmdCreateTeiketsuHojo
  {
    static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger() ;
    Autodesk.Revit.DB.Document _doc { get ; set ; }
    UIDocument _uiDoc { get ; set ; }

    public CmdCreateTeiketsuHojo( UIDocument uiDoc )
    {
      _uiDoc = uiDoc ;
      _doc = _uiDoc.Document ;
    }

    private List<string> teikesuList()
    {
      return new List<string>() { "ﾌﾞﾙﾏﾝ", "ﾘｷﾏﾝ", "取付補助材" } ;
    }

    /// <summary>
    /// 実行
    /// </summary>
    /// <returns></returns>
    public bool Excute()
    {
      try {
        //YMS_gantry.UI.FrmPutTeiketsuHojyozai frm = new FrmPutTeiketsuHojyozai(_uiDoc.Application);
        //if (frm.ShowDialog() != System.Windows.Forms.DialogResult.OK) { return false; }
        FrmPutTeiketsuHojyozai frm = Application.thisApp.frmPutTeiketsuHojyozai ;
        List<string> retMs = new List<string>() ;
        //自動
        if ( frm.RbtAuto.Checked ) {
          List<ElementId> kiribaiUkeIds = new List<ElementId>() ;
          using ( Transaction tr = new Transaction( _doc ) ) {
            tr.Start( "TeiketsuHojo Palcement" ) ;
            //切梁受けを水平つなぎとして締結補助材を付ける場合
            if ( frm.ChkKiribariAsJoint.Checked ) {
              kiribaiUkeIds = ReckonKiribariUkeAsHorizontalJoint( frm.CmbKoudaiName.Text ) ;
            }

            var ms = MaterialSuper.Collect( _doc ).Where( x => x.m_KodaiName == frm.CmbKoudaiName.Text ) ;
            if ( ms == null || ms.Count() < 1 ) {
              return false ;
            }

            List<MaterialSuper> materilas = ms.ToList() ;
            if ( frm.ChkUp.Checked ) {
              //杭(TopPL無)-桁材

              //根太-大引、根太-敷桁                           
              if ( ! CreateKetazaiHojo( materilas, frm.CmbKoudaiName.Text, frm.CmbUpSize.Text,
                    frm.RbtBulmanUp.Checked ) ) {
                retMs.Add( "上部工締結補助材の配置に失敗しました" ) ;
              }

              //水平ブレス
              if ( ! CreateHorizontalHojo( materilas, frm.CmbKoudaiName.Text, frm.CmbUpSize.Text,
                    frm.RbtBulmanUp.Checked ) ) {
                retMs.Add( "水平ブレース締結補助材の配置に失敗しました" ) ;
              }
            }

            if ( frm.ChkBtm.Checked ) {
              //垂直ブレス
              if ( ! CreateVerticalBraceHojo( materilas, frm.CmbKoudaiName.Text, frm.CmbBtmSize.Text,
                    frm.RbtBulmanBtm.Checked ) ) {
                retMs.Add( "垂直ブレース用締結補助材の配置に失敗しました" ) ;
              }

              //水平ツナギ
              if ( ! CreateHorizontalJointHojo( materilas, frm.CmbKoudaiName.Text, frm.CmbBtmSize.Text,
                    frm.RbtBulmanBtm.Checked ) ) {
                retMs.Add( "水平ツナギ用締結補助材の配置に失敗しました" ) ;
              }
            }

            //ネコ材配置
            if ( frm.ChkNeko.Checked ) {
              if ( ! CreateNeko( materilas, frm.CmbHolBrace.Text, frm.CmbHolJoint_Web.Text, frm.CmbHolJoint_Frange.Text,
                    frm.CmbVertBrace_Web.Text, frm.CmbVertBrace_Frange.Text, frm.CmbKoudaiName.Text ) ) {
                retMs.Add( "ネコ材の配置に失敗しました" ) ;
              }
            }

            //水平つなぎとして扱った切梁受から情報を削除
            foreach ( ElementId id in kiribaiUkeIds ) {
              RevitUtil.ClsRevitUtil.SetMojiParameter( _doc, id, DefineUtil.PARAM_MATERIAL_DETAIL, "" ) ;
            }

            tr.Commit() ;
          }
        }
        else {
          string type = frm.RbtNekoFree.Checked ? Master.ClsTeiketsuHojoCsv.TypeNeko :
            frm.RbtBulFree.Checked ? Master.ClsTeiketsuHojoCsv.TypeBulman : Master.ClsTeiketsuHojoCsv.TypeRikiman ;
          string size = frm.RbtNekoFree.Checked ? frm.CmbNekoFree.Text :
            frm.RbtBulFree.Checked ? frm.CmbBulFree.Text : frm.CmbRikiFree.Text ;
          using ( TransactionGroup tG = new TransactionGroup( _doc ) ) {
            FamilySymbol sym = null ;
            string path = Master.ClsTeiketsuHojoCsv.GetFamilyPath( type, size ) ;
            type = frm.RbtNekoFree.Checked ? Master.ClsTeiketsuHojoCsv.HojoType_B :
              frm.RbtBulFree.Checked ? Master.ClsTeiketsuHojoCsv.TypeBulman : Master.ClsTeiketsuHojoCsv.TypeRikiman ;
            if ( ! GantryUtil.GetFamilySymbol( _doc, path, type, out sym, false ) ) {
              _logger.Error( $"ファミリロードに失敗しました。\r{path}" ) ;
              return false ;
            }

            using ( Transaction tr = new Transaction( _doc ) ) {
              GantryUtil util = new GantryUtil() ;
              //配置
              TeiketsuHojo th = new TeiketsuHojo() ;
              th.m_KodaiName = frm.CmbKoudaiName.Text ;
              th.m_Size = size ;

              List<ElementId> ids = util.PlaceFamilyInstance( _uiDoc.Application, sym, false ) ;
              tr.Start( "TeiketsuRalated" ) ;
              foreach ( ElementId id in ids ) {
                th.m_ElementId = id ;
                TeiketsuHojo.WriteToElement( th, _doc ) ;
              }

              tr.Commit() ;
            }

            tG.Assimilate() ;
          }
        }

        if ( retMs.Count > 0 ) {
          string err = string.Join( "\r\n", retMs.Select( x => x ) ) ;
          MessageUtil.Warning( $"下記の配置に失敗しました。詳細はログをご確認ください\r\n{err}", "締結補助材配置" ) ;
        }
        else {
          MessageUtil.Information( "締結補助材の配置が完了しました", "締結補助材配置" ) ;
        }

        return true ;
      }
      catch ( Autodesk.Revit.Exceptions.OperationCanceledException ) {
        //Nothing
        return false ;
      }
      catch ( Exception ex ) {
        return false ;
      }
    }

    /// <summary>
    /// 上部工(根太-大引交点,根太-敷桁交点,水平ブレス)にブルマン・リキマンを配置する
    /// </summary>
    /// <param name="materials"></param>
    /// <param name="symbol"></param>
    private bool CreateKetazaiHojo( List<MaterialSuper> materials, string koudaiName, string size, bool isBulman )
    {
      try {
        string teiketsuType = Master.ClsTeiketsuHojoCsv.GetTeiketsuType(
          isBulman ? Master.ClsTeiketsuHojoCsv.TypeBulman : Master.ClsTeiketsuHojoCsv.TypeRikiman, size ) ;
        if ( teiketsuType != Master.ClsTeiketsuHojoCsv.TeiketsuC ) {
          return true ;
        }

        //根太
        List<Neda> nedas = materials.Select( x => x as Neda ).Where( x =>
          x != null && x.m_ElementId != ElementId.InvalidElementId && x.m_KodaiName == koudaiName ).ToList() ;
        //大引
        List<ElementId> ohbikis = materials.Select( x => x as Ohbiki )
          .Where( x => x != null && x.m_ElementId != ElementId.InvalidElementId && x.m_KodaiName == koudaiName )
          .Select( x => x.m_ElementId ).ToList() ;
        //敷桁
        List<ElementId> shikigetas = MaterialSuper.Collect( _doc ).Where( x => x.m_ElementId != null &&
                                                                               x.m_KodaiName == koudaiName &&
                                                                               x.GetType().Equals(
                                                                                 typeof( Shikigeta ) ) )
          .Select( x => x.m_ElementId ).ToList() ;

        List<XYZ> intersectPnt = new List<XYZ>() ;
        List<FamilyInstanceCreationData> dataList = new List<FamilyInstanceCreationData>() ;

        if ( ! nedas.Select( x => x.MaterialSize().Shape == MaterialShape.H ).Any() ) {
          MessageUtil.Warning( "H鋼以外の上部工に締結補助材を配置できませんでした", "締結補助材配置" ) ;
          return true ;
        }

        FamilySymbol sym = null ;
        string path = Master.ClsTeiketsuHojoCsv.GetFamilyPath(
          isBulman ? Master.ClsTeiketsuHojoCsv.TypeBulman : Master.ClsTeiketsuHojoCsv.TypeRikiman, size ) ;
        string type = isBulman ? Master.ClsTeiketsuHojoCsv.TypeBulman : Master.ClsTeiketsuHojoCsv.TypeRikiman ;
        if ( ! GantryUtil.GetFamilySymbol( _doc, path, type, out sym, true ) ) {
          _logger.Error( $"ファミリロードに失敗しました。\r{path}" ) ;
          return false ;
        }

        foreach ( Neda neda in nedas ) {
          //List<ElementId> intsId = new List<ElementId>();
          List<ElementId> intsId =
            RevitUtil.ClsRevitUtil.GetIntersectFamilys( _doc, neda.m_ElementId, ignorelist: teikesuList() ) ;
          List<ElementId> ohbikiIds = intsId.Where( x => ohbikis.Contains( x ) ).ToList() ;
          List<ElementId> shikigetaIds = intsId.Where( x => shikigetas.Contains( x ) ).ToList() ;

          Curve c = GantryUtil.GetCurve( _doc, neda.m_ElementId ) ;
          List<(XYZ, ElementId)> ohbikiIntPnts = CalcCurveIntersection( c, ohbikiIds ) ;
          List<(XYZ, ElementId)> shikigetaIntPnts = CalcCurveIntersection( c, shikigetaIds ) ;
          MaterialSize nS = neda.MaterialSize() ;
          foreach ( (XYZ, ElementId) itm in ohbikiIntPnts ) {
            Curve tC = GantryUtil.GetCurve( _doc, itm.Item2 ) ;
            Ohbiki o = Ohbiki.ReadFromElement( itm.Item2, _doc ) as Ohbiki ;
            if ( tC.GetEndPoint( 0 ).DistanceTo( itm.Item1 ) > tC.GetEndPoint( 1 ).DistanceTo( itm.Item1 ) ) {
              tC = Line.CreateBound( tC.GetEndPoint( 1 ), tC.GetEndPoint( 0 ) ) ;
            }

            if ( c.GetEndPoint( 0 ).DistanceTo( itm.Item1 ) > c.GetEndPoint( 1 ).DistanceTo( itm.Item1 ) ) {
              c = Line.CreateBound( c.GetEndPoint( 1 ), c.GetEndPoint( 0 ) ) ;
            }

            XYZ vecN = ( c.GetEndPoint( 1 ) - c.GetEndPoint( 0 ) ).Normalize() ;
            XYZ crsN = vecN.CrossProduct( XYZ.BasisZ ).Normalize() ;
            MaterialSize oS = o.MaterialSize() ;
            XYZ faceP = itm.Item1 + XYZ.BasisZ * ( RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( oS.Height / 2 ) ) ;
            Face f = GantryUtil.GetTopFaceOfFamilyInstance( _doc.GetElement( itm.Item2 ) as FamilyInstance ) ;
            List<XYZ> pnts = ( new List<XYZ>()
            {
              ( itm.Item1 + ( vecN * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( nS.Width / 2 ) ) +
                ( crsN * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( oS.Width / 2 ) ) ),
              ( itm.Item1 + ( vecN.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( nS.Width / 2 ) ) +
                ( crsN * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( oS.Width / 2 ) ) ),
              ( itm.Item1 + ( vecN * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( nS.Width / 2 ) ) +
                ( crsN.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( oS.Width / 2 ) ) ),
              ( itm.Item1 + ( vecN.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( nS.Width / 2 ) ) +
                ( crsN.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( oS.Width / 2 ) ) )
            } ) ;

            foreach ( XYZ p in pnts ) {
              if ( GantryUtil.IsXYZInList( p, intersectPnt ) ) {
                continue ;
              }

              XYZ dir = ( faceP - p ).Normalize() ;
              dataList.Add( new FamilyInstanceCreationData( f, p, dir, sym ) ) ;
              intersectPnt.Add( p ) ;
            }
          }

          foreach ( (XYZ, ElementId) itm in shikigetaIntPnts ) {
            Curve tC = GantryUtil.GetCurve( _doc, itm.Item2 ) ;
            Shikigeta o = Shikigeta.ReadFromElement( itm.Item2, _doc ) as Shikigeta ;
            if ( tC.GetEndPoint( 0 ).DistanceTo( itm.Item1 ) > tC.GetEndPoint( 1 ).DistanceTo( itm.Item1 ) ) {
              tC = Line.CreateBound( tC.GetEndPoint( 1 ), tC.GetEndPoint( 0 ) ) ;
            }

            if ( c.GetEndPoint( 0 ).DistanceTo( itm.Item1 ) > c.GetEndPoint( 1 ).DistanceTo( itm.Item1 ) ) {
              c = Line.CreateBound( c.GetEndPoint( 1 ), c.GetEndPoint( 0 ) ) ;
            }

            XYZ vecN = ( c.GetEndPoint( 1 ) - c.GetEndPoint( 0 ) ).Normalize() ;
            XYZ crsN = vecN.CrossProduct( XYZ.BasisZ ).Normalize() ;
            MaterialSize oS = o.MaterialSize() ;
            Face f = GantryUtil.GetTopFaceOfFamilyInstance( _doc.GetElement( itm.Item2 ) as FamilyInstance ) ;
            XYZ faceP = itm.Item1 + XYZ.BasisZ * ( RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( oS.Height / 2 ) ) ;
            List<XYZ> pnts = ( new List<XYZ>()
            {
              ( itm.Item1 + ( vecN * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( nS.Width / 2 ) ) +
                ( crsN * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( oS.Width / 2 ) ) ),
              ( itm.Item1 + ( vecN.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( nS.Width / 2 ) ) +
                ( crsN * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( oS.Width / 2 ) ) ),
              ( itm.Item1 + ( vecN * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( nS.Width / 2 ) ) +
                ( crsN.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( oS.Width / 2 ) ) ),
              ( itm.Item1 + ( vecN.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( nS.Width / 2 ) ) +
                ( crsN.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( oS.Width / 2 ) ) )
            } ) ;

            foreach ( XYZ p in pnts ) {
              if ( GantryUtil.IsXYZInList( p, intersectPnt ) ) {
                continue ;
              }

              XYZ dir = ( faceP - p ).Normalize() ;
              dataList.Add( new FamilyInstanceCreationData( f, p, dir, sym ) ) ;
              intersectPnt.Add( p ) ;
            }
          }
        }

        var ids = _doc.Create.NewFamilyInstances2( dataList ) ;
        foreach ( ElementId id in ids ) {
          RevitUtil.ClsRevitUtil.SetParameter( _doc, id, DefineUtil.PARAM_HOST_OFFSET, 0 ) ;
          TeiketsuHojo th = new TeiketsuHojo() { m_ElementId = id, m_KodaiName = koudaiName, m_Size = size } ;
          TeiketsuHojo.WriteToElement( th, _doc ) ;
        }

        return true ;
      }
      catch ( Exception ex ) {
        _logger.Error( ex.StackTrace ) ;
        return false ;
      }
    }

    /// <summary>
    /// 水平ブレス用のブルマン・リキマン配置
    /// </summary>
    /// <param name="materials"></param>
    /// <param name="koudaiName"></param>
    /// <param name="size"></param>
    /// <param name="isBulman"></param>
    /// <returns></returns>
    private bool CreateHorizontalHojo( List<MaterialSuper> materials, string koudaiName, string size, bool isBulman )
    {
      try {
        KoudaiData kData = GantryUtil.GetKoudaiData( _doc, koudaiName ) ;
        if ( kData == null ) {
          return false ;
        }

        var (origin, axisH, axisK, axisZ) = kData.AllKoudaiFlatData.GetBasis( _doc ) ;
        List<FamilyInstanceCreationData> dataList = new List<FamilyInstanceCreationData>() ;

        //水平ブレス
        if ( ! string.IsNullOrEmpty( size ) ) {
          List<HorizontalBrace> horBraces = materials.Select( x => x as HorizontalBrace ).Where( x =>
            x != null && x.m_ElementId != ElementId.InvalidElementId && x.m_KodaiName == koudaiName ).ToList() ;
          if ( horBraces.Count < 1 ) {
            return true ;
          }

          //柱、杭
          List<ElementId> pillers = materials.Select( x => x as PilePillerSuper )
            .Where( x => x != null && x.m_ElementId != ElementId.InvalidElementId && x.m_KodaiName == koudaiName )
            .Select( x => x.m_ElementId ).ToList() ;

          //柱、杭
          List<ElementId> ketas = materials.Select( x => x as OhbikiSuper )
            .Where( x => x != null && x.m_ElementId != ElementId.InvalidElementId && x.m_KodaiName == koudaiName )
            .Select( x => x.m_ElementId ).ToList() ;

          FamilySymbol sym ;
          string path = Master.ClsTeiketsuHojoCsv.GetFamilyPath(
            isBulman ? Master.ClsTeiketsuHojoCsv.TypeBulman : Master.ClsTeiketsuHojoCsv.TypeRikiman, size ) ;
          string type = Master.ClsTeiketsuHojoCsv.TypeBulman ;
          string teiketsuType = Master.ClsTeiketsuHojoCsv.GetTeiketsuType(
            isBulman ? Master.ClsTeiketsuHojoCsv.TypeBulman : Master.ClsTeiketsuHojoCsv.TypeRikiman, size ) ;

          if ( ! GantryUtil.GetFamilySymbol( _doc, path, type, out sym, true ) ) {
            _logger.Error( $"ファミリロードに失敗しました。\r{path}" ) ;
          }

          //全水平ブレス
          foreach ( HorizontalBrace brace in horBraces ) {
            FamilyInstance braceIns = _doc.GetElement( brace.m_ElementId ) as FamilyInstance ;
            //アングル材以外のブレース（ターンバックル）は対象外
            if ( brace.MaterialSize().Shape != MaterialShape.L ) {
              continue ;
            }

            List<ElementId> intsId =
              RevitUtil.ClsRevitUtil.GetIntersectFamilys( _doc, brace.m_ElementId, ignorelist: teikesuList() ) ;
            List<ElementId> pillerIds = intsId.Where( x => pillers.Contains( x ) ).ToList() ;
            List<ElementId> ketaIds = intsId.Where( x => ketas.Contains( x ) ).ToList() ;
            Curve c = GantryUtil.GetCurve( _doc, brace.m_ElementId ) ;
            XYZ braceVec = ( c.GetEndPoint( 1 ) - c.GetEndPoint( 0 ) ) ;
            List<XYZ> pnts = new List<XYZ>() ;
            Transform bTs = braceIns.GetTransform() ;
            Face f = GantryUtil.GetTopFaceOfFamilyInstance( braceIns ) ;
            MaterialSize ms = brace.MaterialSize() ;
            if ( ketaIds.Count > 0 ) {
              foreach ( ElementId id in ketaIds ) {
                pnts = GantryUtil.FindIntersectionPnts( _doc, brace.m_ElementId, id ) ;
                XYZ nearP = ( pnts[ 0 ].DistanceTo( c.GetEndPoint( 0 ) ) < pnts[ 0 ].DistanceTo( c.GetEndPoint( 1 ) ) )
                  ? c.GetEndPoint( 0 )
                  : c.GetEndPoint( 1 ) ;
                if ( c.Distance( c.GetEndPoint( 1 ) ) < 0.1 ) {
                  c = Line.CreateBound( c.GetEndPoint( 1 ), c.GetEndPoint( 0 ) ) ;
                  braceVec = braceVec.Negate() ;
                }

                bool bIsNarrowAngle = ( axisH.Normalize() ).AngleTo( braceVec.Normalize() ) < ( Math.PI / 2 ) ;
                double dotProduct = axisH.DotProduct( bTs.BasisY ) ;
                double magnitude1 = axisH.GetLength() ;
                double magnitude2 = bTs.BasisY.GetLength() ;
                double cosTheta = dotProduct / ( magnitude1 * magnitude2 ) ;
                double angleInRadians = Math.Acos( cosTheta ) ;
                bool isSame = angleInRadians < Math.PI / 2 ;

                List<XYZ> onList = new List<XYZ>() ;
                List<XYZ> noList = new List<XYZ>() ;
                foreach ( XYZ pn in pnts ) {
                  if ( RevitUtil.ClsRevitUtil.CovertFromAPI( c.Distance( pn ) ) < ms.Thick ) {
                    onList.Add( pn ) ;
                  }
                  else {
                    noList.Add( pn ) ;
                  }
                }

                bool isCType = ( isSame == bIsNarrowAngle ) ;
                XYZ normal = isCType ? bTs.BasisY.Normalize() : bTs.BasisY.Negate().Normalize() ;

                if ( teiketsuType == Master.ClsTeiketsuHojoCsv.TeiketsuC && ( isCType ) ) {
                  XYZ p = noList.OrderBy( x => x.DistanceTo( nearP ) ).Last() ;
                  XYZ placeP = p + bTs.BasisY.Normalize() *
                    RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( ms.Height / 2 ) ;
                  dataList.Add( new FamilyInstanceCreationData( f, placeP, normal, sym ) ) ;
                }
                else if ( teiketsuType == Master.ClsTeiketsuHojoCsv.TeiketsuG && ( ! isCType ) ) {
                  XYZ p = onList.OrderBy( x => x.DistanceTo( nearP ) ).Last() ;
                  XYZ placeP = p + bTs.BasisY.Negate().Normalize() *
                    RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( ms.Height / 2 ) ;
                  dataList.Add( new FamilyInstanceCreationData( f, placeP, normal, sym ) ) ;
                }
              }
            }
            else if ( pillerIds.Count > 0 ) {
              foreach ( ElementId id in pillerIds ) {
                pnts = GantryUtil.FindIntersectionPnts( _doc, brace.m_ElementId, id ) ;
                XYZ nearP = ( pnts[ 0 ].DistanceTo( c.GetEndPoint( 0 ) ) < pnts[ 0 ].DistanceTo( c.GetEndPoint( 1 ) ) )
                  ? c.GetEndPoint( 0 )
                  : c.GetEndPoint( 1 ) ;
                if ( c.Distance( c.GetEndPoint( 1 ) ) < 0.1 ) {
                  c = Line.CreateBound( c.GetEndPoint( 1 ), c.GetEndPoint( 0 ) ) ;
                  braceVec = braceVec.Negate() ;
                }

                bool bIsNarrowAngle = ( axisH.Normalize() ).AngleTo( braceVec.Normalize() ) < ( Math.PI / 2 ) ;
                double dotProduct = axisH.DotProduct( bTs.BasisY ) ;
                double magnitude1 = axisH.GetLength() ;
                double magnitude2 = bTs.BasisY.GetLength() ;
                double cosTheta = dotProduct / ( magnitude1 * magnitude2 ) ;
                double angleInRadians = Math.Acos( cosTheta ) ;
                bool isSame = angleInRadians < Math.PI / 2 ;

                List<XYZ> onList = new List<XYZ>() ;
                List<XYZ> noList = new List<XYZ>() ;
                foreach ( XYZ pn in pnts ) {
                  if ( RevitUtil.ClsRevitUtil.CovertFromAPI( c.Distance( pn ) ) < ms.Thick ) {
                    onList.Add( pn ) ;
                  }
                  else {
                    noList.Add( pn ) ;
                  }
                }

                bool isCType = ( isSame == bIsNarrowAngle ) ;
                XYZ normal = isCType ? bTs.BasisY.Normalize() : bTs.BasisY.Negate().Normalize() ;

                if ( teiketsuType == Master.ClsTeiketsuHojoCsv.TeiketsuC && ( isCType ) ) {
                  XYZ p = noList.OrderBy( x => x.DistanceTo( nearP ) ).Last() ;
                  XYZ placeP = p + bTs.BasisY.Normalize() *
                    RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( ms.Height / 2 ) ;
                  dataList.Add( new FamilyInstanceCreationData( f, placeP, normal, sym ) ) ;
                }
                else if ( teiketsuType == Master.ClsTeiketsuHojoCsv.TeiketsuG && ( ! isCType ) ) {
                  XYZ p = onList.OrderBy( x => x.DistanceTo( nearP ) ).Last() ;
                  XYZ placeP = p + bTs.BasisY.Negate().Normalize() *
                    RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( ms.Height / 2 ) ;
                  dataList.Add( new FamilyInstanceCreationData( f, placeP, normal, sym ) ) ;
                }
              }
            }
          }
        }

        var ids = _doc.Create.NewFamilyInstances2( dataList ) ;
        foreach ( ElementId id in ids ) {
          RevitUtil.ClsRevitUtil.SetParameter( _doc, id, DefineUtil.PARAM_HOST_OFFSET, 0 ) ;
          TeiketsuHojo th = new TeiketsuHojo() { m_ElementId = id, m_KodaiName = koudaiName, m_Size = size } ;
          TeiketsuHojo.WriteToElement( th, _doc ) ;
        }

        return true ;
      }
      catch ( Exception ex ) {
        _logger.Error( ex.StackTrace ) ;
        return false ;
      }
    }

    /// <summary>
    /// 下部工(垂直ブレス、水平つなぎ)にブルマン・リキマンを配置する
    /// </summary>
    /// <param name="materials"></param>
    /// <param name="koudaiName"></param>
    /// <param name="size"></param>
    /// <param name="isBulman"></param>
    private bool CreateVerticalBraceHojo( List<MaterialSuper> materials, string koudaiName, string size, bool isBulman )
    {
      try {
        //垂直ブレース
        List<VerticalBrace> verBraces = materials.Select( x => x as VerticalBrace ).Where( x =>
          x != null && x.m_ElementId != ElementId.InvalidElementId && x.m_KodaiName == koudaiName ).ToList() ;
        if ( verBraces.Count < 1 ) {
          return true ;
        }

        //柱、杭
        List<ElementId> pillers = materials.Select( x => x as PilePillerSuper )
          .Where( x => x != null && x.m_ElementId != ElementId.InvalidElementId && x.m_KodaiName == koudaiName )
          .Select( x => x.m_ElementId ).ToList() ;

        List<FamilyInstanceCreationData> dataList = new List<FamilyInstanceCreationData>() ;
        FamilySymbol sym = null ;
        string path = Master.ClsTeiketsuHojoCsv.GetFamilyPath(
          isBulman ? Master.ClsTeiketsuHojoCsv.TypeBulman : Master.ClsTeiketsuHojoCsv.TypeRikiman, size ) ;
        string type = isBulman ? Master.ClsTeiketsuHojoCsv.TypeBulman : Master.ClsTeiketsuHojoCsv.TypeRikiman ;
        string teiketsuType = Master.ClsTeiketsuHojoCsv.GetTeiketsuType(
          isBulman ? Master.ClsTeiketsuHojoCsv.TypeBulman : Master.ClsTeiketsuHojoCsv.TypeRikiman, size ) ;
        if ( ! GantryUtil.GetFamilySymbol( _doc, path, type, out sym, true ) ) {
          _logger.Error( $"ファミリロードに失敗しました。\r{path}" ) ;
          return false ;
        }

        //ブレースと触れる柱を対象とし、交点を求める
        foreach ( VerticalBrace brace in verBraces ) {
          FamilyInstance braceIns = _doc.GetElement( brace.m_ElementId ) as FamilyInstance ;
          //アングル材以外のブレース（ターンバックル）は対象外
          if ( brace.MaterialSize().Shape != MaterialShape.L ) {
            continue ;
          }

          //List<ElementId> intsId = new List<ElementId>();
          List<ElementId> intsId =
            RevitUtil.ClsRevitUtil.GetIntersectFamilys( _doc, brace.m_ElementId, ignorelist: teikesuList() ) ;
          List<ElementId> pillerIds = intsId.Where( x => pillers.Contains( x ) ).ToList() ;
          Curve c = GantryUtil.GetCurve( _doc, brace.m_ElementId ) ;
          XYZ braceVec = ( c.GetEndPoint( 1 ) - c.GetEndPoint( 0 ) ) ;
          if ( braceVec.Z < 0 ) {
            c = Line.CreateBound( c.GetEndPoint( 1 ), c.GetEndPoint( 0 ) ) ;
            braceVec = ( c.GetEndPoint( 1 ) - c.GetEndPoint( 0 ) ) ;
          }

          XYZ bracePlaneVec = new XYZ( braceVec.X, braceVec.Y, 0 ) ;
          MaterialSize bS = brace.MaterialSize() ;
          double bRotate = bracePlaneVec.AngleTo( braceVec ) ;
          Transform ts = braceIns.GetTotalTransform() ;
          if ( ts.BasisZ.Y == -1 ) {
            bool b = true ;
          }

          foreach ( ElementId id in pillerIds ) {
            FamilyInstance pillerIns = _doc.GetElement( id ) as FamilyInstance ;
            XYZ pillerHandle = pillerIns.HandOrientation ;
            List<XYZ> pnts = GantryUtil.FindIntersectionPnts( _doc, brace.m_ElementId, id ) ;
            bool isFlange = GantryUtil.AreComponentsSameSign( pillerHandle, bracePlaneVec ) ||
                            GantryUtil.AreComponentsSameSign( pillerHandle.Negate(), bracePlaneVec ) ;
            if ( pnts.Count > 1 ) {
              XYZ nearP = ( pnts[ 0 ].DistanceTo( c.GetEndPoint( 0 ) ) < pnts[ 0 ].DistanceTo( c.GetEndPoint( 1 ) ) )
                ? c.GetEndPoint( 0 )
                : c.GetEndPoint( 1 ) ;
              bool isUpSize = nearP.DistanceTo( c.GetEndPoint( 1 ) ) < 0.1 ;
              XYZ p = isUpSize
                ? pnts.OrderBy( x => x.DistanceTo( c.GetEndPoint( 0 ) ) ).First()
                : pnts.OrderBy( x => x.DistanceTo( c.GetEndPoint( 1 ) ) ).ToList().ElementAt( 1 ) ;

              //C型
              if ( isFlange && teiketsuType == Master.ClsTeiketsuHojoCsv.TeiketsuC ) {
                XYZ normal = ts.BasisY.Normalize() ;
                if ( isUpSize ) {
                  double ro = ( RevitUtil.ClsGeo.IsLeft( braceVec, normal ) ? -45 : 45 ) / ( 180 / Math.PI ) ;
                  Transform rotationTransform = Transform.CreateRotationAtPoint( ts.BasisZ.Normalize(), ro, p ) ;
                  normal = rotationTransform.OfVector( normal ) ;
                }

                //点
                double moveDist = ( bS.Height / 4 ) / Math.Cos( bRotate ) ;
                XYZ pnt = p + XYZ.BasisZ * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( moveDist ) ;
                //面
                Face f = GantryUtil.GetSpecifyFaceOfFamilyInstance( braceIns, ts.BasisZ.Negate() ) ;
                if ( f != null ) {
                  dataList.Add( new FamilyInstanceCreationData( f, pnt, normal, sym ) ) ;
                }
              }
              //LA型
              else if ( ! isFlange && teiketsuType == Master.ClsTeiketsuHojoCsv.TeiketsuLA ) {
                List<XYZ> laList = new List<XYZ>() ;
                foreach ( XYZ pn in pnts ) {
                  if ( RevitUtil.ClsRevitUtil.CovertFromAPI( c.Distance( pn ) ) > bS.Thick ) {
                    laList.Add( pn ) ;
                  }
                }

                p = isUpSize
                  ? laList.OrderBy( x => x.DistanceTo( c.GetEndPoint( 0 ) ) ).First()
                  : laList.OrderBy( x => x.DistanceTo( c.GetEndPoint( 0 ) ) ).ToList().Last() ;
                double moveDist = ( bS.Height / 4 ) / Math.Cos( bRotate ) ;
                XYZ pnt = p + XYZ.BasisZ * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( moveDist ) ;
                //面
                Face f = GantryUtil.GetSpecifyFaceOfFamilyInstance( braceIns, ts.BasisZ.Negate() ) ;
                if ( f != null ) {
                  dataList.Add( new FamilyInstanceCreationData( f, pnt, XYZ.BasisZ.Normalize(), sym ) ) ;
                }
              }
            }
          }
        }

        var ids = _doc.Create.NewFamilyInstances2( dataList ) ;
        foreach ( ElementId id in ids ) {
          RevitUtil.ClsRevitUtil.SetParameter( _doc, id, DefineUtil.PARAM_HOST_OFFSET, 0 ) ;
          TeiketsuHojo th = new TeiketsuHojo() { m_ElementId = id, m_KodaiName = koudaiName, m_Size = size } ;
          TeiketsuHojo.WriteToElement( th, _doc ) ;
        }

        return true ;
      }
      catch ( Exception ex ) {
        _logger.Error( ex.StackTrace ) ;
        return false ;
      }
    }

    /// <summary>
    /// ツナギにブルマン、リキマンを配置する
    /// </summary>
    /// <param name="materials"></param>
    /// <param name="koudaiName"></param>
    /// <param name="size"></param>
    /// <param name="isBulman"></param>
    /// <returns></returns>
    private bool CreateHorizontalJointHojo( List<MaterialSuper> materials, string koudaiName, string size,
      bool isBulman )
    {
      try {
        //水平ツナギ
        List<ElementId> joints = materials.Select( x => x as HorizontalJoint )
          .Where( x => x != null && x.m_ElementId != ElementId.InvalidElementId && x.m_KodaiName == koudaiName )
          .Select( x => x.m_ElementId ).ToList() ;
        if ( joints.Count < 1 ) {
          return true ;
        }

        //柱、杭
        List<PilePillerSuper> pillers = materials.Select( x => x as PilePillerSuper ).Where( x =>
          x != null && x.m_ElementId != ElementId.InvalidElementId && x.m_KodaiName == koudaiName ).ToList() ;

        //まとめて配置するのでリスト作成
        List<FamilyInstanceCreationData> dataList = new List<FamilyInstanceCreationData>() ;
        FamilySymbol sym = null ;
        string path = Master.ClsTeiketsuHojoCsv.GetFamilyPath(
          isBulman ? Master.ClsTeiketsuHojoCsv.TypeBulman : Master.ClsTeiketsuHojoCsv.TypeRikiman, size ) ;
        string type = isBulman ? Master.ClsTeiketsuHojoCsv.TypeBulman : Master.ClsTeiketsuHojoCsv.TypeRikiman ;
        string teiketsuType = Master.ClsTeiketsuHojoCsv.GetTeiketsuType(
          isBulman ? Master.ClsTeiketsuHojoCsv.TypeBulman : Master.ClsTeiketsuHojoCsv.TypeRikiman, size ) ;
        if ( ! GantryUtil.GetFamilySymbol( _doc, path, type, out sym, true ) ) {
          _logger.Error( $"ファミリロードに失敗しました。\r{path}" ) ;
          return false ;
        }

        bool hasNoChannel = false ;
        //柱と触れる部材を対象とし、交点を求める
        foreach ( PilePillerSuper pil in pillers ) {
          FamilyInstance pillerIns = _doc.GetElement( pil.m_ElementId ) as FamilyInstance ;
          //柱に接するファミリのリスト(締結補助材は除く)
          List<ElementId> intsId = RevitUtil.ClsRevitUtil.GetIntersectFamilys( _doc, pil.m_ElementId,
            ignorelist: teikesuList(), serchIds: joints ) ;
          //柱に接するツナギ材だけを収集
          List<ElementId> jointIds = intsId.Where( x => joints.Contains( x ) ).ToList() ;
          //柱の基点
          XYZ pillerP = pil.Position ;

          foreach ( ElementId jId in jointIds ) {
            FamilyInstance jins = _doc.GetElement( jId ) as FamilyInstance ;
            Curve jC = GantryUtil.GetCurve( _doc, jId ) ;
            XYZ jointVec = ( jC.GetEndPoint( 1 ) - jC.GetEndPoint( 0 ) ).Normalize() ;
            //ツナギ材に接する別のツナギ材を取得
            List<ElementId> jintsId =
              RevitUtil.ClsRevitUtil.GetIntersectFamilys( _doc, jId, ignorelist: teikesuList(), serchIds: jointIds ) ;
            List<ElementId> jjIds = jintsId.Where( x => joints.Contains( x ) && intsId.Contains( x ) ).ToList() ;
            //このツナギがフランジ方向に平行か
            bool isFlange = GantryUtil.AreComponentsSameSign( pillerIns.HandOrientation, jointVec ) ||
                            GantryUtil.AreComponentsSameSign( pillerIns.HandOrientation, jointVec.Negate() ) ;

            //ツナギと柱の交点
            List<XYZ> pnts = GantryUtil.FindIntersectionPnts( _doc, pil.m_ElementId, jId ) ;
            if ( pnts.Count < 1 ) {
              continue ;
            }

            XYZ nearP = ( pnts[ 0 ].DistanceTo( jC.GetEndPoint( 0 ) ) < pnts[ 0 ].DistanceTo( jC.GetEndPoint( 1 ) ) )
              ? jC.GetEndPoint( 0 )
              : jC.GetEndPoint( 1 ) ;
            //交点群が近いほうの点からのベクトル
            if ( nearP.DistanceTo( jC.GetEndPoint( 1 ) ) < 0.1 ) {
              jointVec = jointVec.Negate() ;
              jC = Line.CreateBound( jC.GetEndPoint( 1 ), jC.GetEndPoint( 0 ) ) ;
            }

            //切梁受けはベース線の位置が
            if ( jins.Symbol.Name.Contains( "切梁受" ) ) {
              double kiribariH = jins.LookupParameter( DefineUtil.PARAM_HOST_OFFSET ).AsDouble() ;
              nearP = nearP + XYZ.BasisZ.Normalize() * kiribariH ;
              jC = Line.CreateBound( jC.GetEndPoint( 0 ) + XYZ.BasisZ.Normalize() * kiribariH,
                jC.GetEndPoint( 1 ) + XYZ.BasisZ.Normalize() * kiribariH ) ;
              jointVec = ( jC.GetEndPoint( 1 ) - jC.GetEndPoint( 0 ) ).Normalize() ;
            }

            //ツナギ材のマテリアルクラス
            HorizontalJoint mainJ = HorizontalJoint.ReadFromElement( jId, _doc ) as HorizontalJoint ;
            if ( mainJ.MaterialSize().Shape != MaterialShape.C ) {
              hasNoChannel = true ;
              continue ;
            }

            //接するものをすべて考慮し、あとから配置するためフラグを持っておく
            bool needNTForjoint = false ;
            bool isNeedWebNtUp = true ;
            bool needNtcN = false ;
            bool needNtcF = false ;
            bool hasCrossUp = false ;
            bool noCross = true ;

            //接するツナギ材を確認
            foreach ( ElementId id in jjIds ) {
              if ( id.Equals( jId ) ) {
                continue ;
              }

              FamilyInstance jjins = _doc.GetElement( id ) as FamilyInstance ;
              //接するツナギ材のどちらが高い位置にあるか返す(同じ高さの場合はnull)
              FamilyInstance intIns = GetHigherInstance( jins, jjins ) ;
              Curve jjC = GantryUtil.GetCurve( _doc, id ) ;
              XYZ jjointVec = ( jjC.GetEndPoint( 1 ) - jjC.GetEndPoint( 0 ) ).Normalize() ;
              //ツナギとツナギの交点を算出
              List<XYZ> pntsJ = GantryUtil.FindIntersectionPnts( _doc, jId, id ) ;
              if ( pntsJ.Count < 1 ) {
                continue ;
              }

              XYZ nearPJ =
                ( pntsJ[ 0 ].DistanceTo( jjC.GetEndPoint( 0 ) ) < pntsJ[ 0 ].DistanceTo( jjC.GetEndPoint( 1 ) ) )
                  ? jjC.GetEndPoint( 0 )
                  : jjC.GetEndPoint( 1 ) ;
              HorizontalJoint jH = HorizontalJoint.ReadFromElement( id, _doc ) as HorizontalJoint ;
              pillerP = new XYZ( pillerP.X, pillerP.Y, nearPJ.Z ) ;
              if ( jjC.GetEndPoint( 1 ).DistanceTo( nearPJ ) < 0.1 ) {
                jjC = Line.CreateBound( jjC.GetEndPoint( 1 ), jjC.GetEndPoint( 0 ) ) ;
                jjointVec = ( jjC.GetEndPoint( 1 ) - jjC.GetEndPoint( 0 ) ).Normalize() ;
              }

              //切梁受けはベース線の位置が
              if ( jjins.Symbol.Name.Contains( "切梁受" ) ) {
                double kiribariH = jjins.LookupParameter( DefineUtil.PARAM_HOST_OFFSET ).AsDouble() ;
                nearPJ = nearPJ + XYZ.BasisZ.Normalize() * kiribariH ;
                jjC = Line.CreateBound( jjC.GetEndPoint( 0 ) + XYZ.BasisZ.Normalize() * kiribariH,
                  jjC.GetEndPoint( 1 ) + XYZ.BasisZ.Normalize() * kiribariH ) ;
                jointVec = ( jjC.GetEndPoint( 1 ) - jjC.GetEndPoint( 0 ) ).Normalize() ;
              }

              //自信がフランジ側
              if ( isFlange && intIns == null ) {
                //隣り合う水平材がある場合
                if ( nearP.DistanceTo( jjC.GetEndPoint( 0 ) ) < 0.1 ||
                     nearP.DistanceTo( jjC.GetEndPoint( 0 ) ) < 0.1 ) {
                  //NT＋Cが必要
                  needNTForjoint = true ;
                }
              }
              //自信がフランジ側で、ウェブとの交差で自信が上側にいる
              else if ( isFlange && intIns.Id.Equals( jId ) ) {
                if ( teiketsuType == Master.ClsTeiketsuHojoCsv.TeiketsuC ) {
                  Face f = GantryUtil.GetBtmFaceOfFamilyInstance( jins ) ;
                  if ( f == null ) {
                    continue ;
                  }

                  XYZ p = pntsJ.Where( x => nearPJ.Z < x.Z ).OrderBy( x => x.DistanceTo( nearPJ ) ).ToList()
                    .OrderBy( x => x.DistanceTo( pillerP ) ).First() ;
                  XYZ plP = p +
                            jins.GetTransform().BasisY *
                            RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( jH.MaterialSize().Width * 0.75 ) +
                            jjins.GetTransform().BasisY *
                            RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( jH.MaterialSize().Width * 0.75 ) ;
                  dataList.Add( new FamilyInstanceCreationData( f, plP, ( pillerP - p ).Normalize(), sym ) ) ;
                }

                noCross = false ;
                needNTForjoint = true ;
                if ( GantryUtil.AreComponentsSameSign( jointVec.Normalize(),
                      jjins.GetTotalTransform().BasisY.Negate().Normalize() ) ) {
                  needNtcN = true ;
                }
                else {
                  needNtcF = true ;
                }
              }
              //フランジ側部材で、ウェブとの交差で下側にいる
              else if ( isFlange && intIns.Id.Equals( id ) ) {
                if ( teiketsuType == Master.ClsTeiketsuHojoCsv.TeiketsuC ) {
                  Face f = GantryUtil.GetTopFaceOfFamilyInstance( jins ) ;
                  if ( f == null ) {
                    continue ;
                  }

                  XYZ p = pntsJ.Where( x => nearPJ.Z > x.Z ).OrderBy( x => x.DistanceTo( nearPJ ) ).ToList()
                    .OrderBy( x => x.DistanceTo( pillerP ) ).First() ;
                  XYZ plP = p +
                            jins.GetTransform().BasisY *
                            RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( jH.MaterialSize().Width * 0.75 ) +
                            jjins.GetTransform().BasisY *
                            RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( jH.MaterialSize().Width * 0.75 ) ;

                  dataList.Add( new FamilyInstanceCreationData( f, plP, ( pillerP - p ).Normalize(), sym ) ) ;
                }

                noCross = false ;
                hasCrossUp = true ;
                needNTForjoint = true ;
                if ( GantryUtil.AreComponentsSameSign( jointVec.Normalize(),
                      jjins.GetTotalTransform().BasisY.Negate().Normalize() ) ) {
                  needNtcN = true ;
                }
                else {
                  needNtcF = true ;
                }
              }
              ///ウェブ側で下にいる
              else if ( ! isFlange && intIns != null && intIns.Id.Equals( id ) ) {
                isNeedWebNtUp = false ;
              }
            }


            pillerP = new XYZ( pillerP.X, pillerP.Y, nearP.Z ) ;
            if ( isFlange ) {
              if ( teiketsuType == Master.ClsTeiketsuHojoCsv.TeiketsuG ) {
                XYZ p = pnts.Where( x => nearP.Z < x.Z ).OrderBy( x => x.DistanceTo( nearP ) ).ToList().Last() ;
                XYZ plP = p + jointVec.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 15 ) +
                          XYZ.BasisZ.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 15 ) ;
                Face f = GantryUtil.GetSpecifyFaceOfFamilyInstance( jins, jins.GetTransform().BasisY.Negate() ) ;
                if ( f != null ) {
                  if ( ( needNTForjoint && ! hasCrossUp && needNtcN && ! needNtcF ) || ( noCross ) ) {
                    dataList.Add( new FamilyInstanceCreationData( f, plP, ( pillerP - p ).Normalize(), sym ) ) ;
                  }

                  if ( ( needNTForjoint && ! hasCrossUp && ! needNtcN && needNtcF ) || ( noCross ) ) {
                    XYZ np = pnts.Where( x => nearP.Z < x.Z ).OrderBy( x => x.DistanceTo( nearP ) ).ToList().First() ;
                    if ( np.DistanceTo( p ) > RevitUtil.ClsRevitUtil.CovertToAPI( mainJ.MaterialSize().Width ) ) {
                      plP = np + jointVec * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 15 ) +
                            XYZ.BasisZ.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 15 ) ;
                      dataList.Add( new FamilyInstanceCreationData( f, plP, ( pillerP - np ).Normalize(), sym ) ) ;
                    }
                  }
                }
              }
              else if ( needNTForjoint && teiketsuType == Master.ClsTeiketsuHojoCsv.TeiketsuNT ||
                        teiketsuType == Master.ClsTeiketsuHojoCsv.TeiketsuC ) {
                XYZ p = pnts.Where( x => nearP.Z < x.Z ).OrderBy( x => x.DistanceTo( nearP ) ).ToList().Last() ;
                XYZ plP = p + jins.GetTransform().BasisY * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 13 ) ;
                var r = jins.GetTotalTransform() ;
                var t = jins.GetTransform() ;
                var h = jins.Host ;

                Face f = GantryUtil.GetTopFaceOfFamilyInstance( jins ) ;
                if ( f == null ) {
                  continue ;
                }

                if ( ! hasCrossUp ) {
                  XYZ norm = GetNTWay( pillerP, pillerIns.HandOrientation, p ).Negate() ;

                  if ( needNtcF ) {
                    if ( teiketsuType == Master.ClsTeiketsuHojoCsv.TeiketsuC ) {
                      plP += jins.GetTransform().BasisY * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 50 ) +
                             jointVec * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 50 ) ;
                      norm = jins.GetTransform().BasisY.Negate() ;
                    }

                    dataList.Add( new FamilyInstanceCreationData( f, plP, norm, sym ) ) ;
                  }

                  if ( needNtcN ) {
                    p = pnts.Where( x => nearP.Z < x.Z ).OrderBy( x => x.DistanceTo( nearP ) ).ToList().First() ;
                    plP = p + jins.GetTransform().BasisY * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 13 ) ;
                    norm = GetNTWay( pillerP, pillerIns.HandOrientation, p ).Negate() ;
                    if ( teiketsuType == Master.ClsTeiketsuHojoCsv.TeiketsuC ) {
                      plP += jins.GetTransform().BasisY * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 50 ) +
                             jointVec.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 50 ) ;
                      norm = jins.GetTransform().BasisY.Negate() ;
                    }

                    dataList.Add( new FamilyInstanceCreationData( f, plP, norm, sym ) ) ;
                  }

                  if ( ! hasCrossUp && ! needNtcF && ! needNtcN ) {
                    p = pnts.Where( x => nearP.Z > x.Z ).OrderBy( x => x.DistanceTo( nearP ) ).ToList().Last() ;
                    plP = p + jins.GetTransform().BasisY * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 13 ) ;
                    f = GantryUtil.GetBtmFaceOfFamilyInstance( jins ) ;
                    if ( f == null ) {
                      continue ;
                    }

                    norm = GetNTWay( pillerP, pillerIns.HandOrientation, p, false ).Negate() ;
                    if ( teiketsuType == Master.ClsTeiketsuHojoCsv.TeiketsuC ) {
                      plP += jins.GetTransform().BasisY * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 50 ) +
                             jointVec * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 50 ) ;
                      norm = jins.GetTransform().BasisY.Negate() ;
                    }

                    dataList.Add( new FamilyInstanceCreationData( f, plP, norm, sym ) ) ;
                  }
                }
                else {
                  p = pnts.Where( x => nearP.Z > x.Z ).OrderBy( x => x.DistanceTo( nearP ) ).ToList().Last() ;
                  plP = p + jins.GetTransform().BasisY * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 13 ) ;
                  f = GantryUtil.GetBtmFaceOfFamilyInstance( jins ) ;
                  if ( f == null ) {
                    continue ;
                  }

                  XYZ norm = GetNTWay( pillerP, pillerIns.HandOrientation, p, false ).Negate() ;

                  if ( needNtcF ) {
                    if ( teiketsuType == Master.ClsTeiketsuHojoCsv.TeiketsuC ) {
                      plP += jins.GetTransform().BasisY * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 50 ) +
                             jointVec * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 50 ) ;
                      norm = jins.GetTransform().BasisY.Negate() ;
                    }

                    dataList.Add( new FamilyInstanceCreationData( f, plP, norm, sym ) ) ;
                  }

                  if ( needNtcN ) {
                    p = pnts.Where( x => nearP.Z > x.Z ).OrderBy( x => x.DistanceTo( nearP ) ).ToList().First() ;
                    plP = p + jins.GetTransform().BasisY * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 13 ) ;
                    norm = GetNTWay( pillerP, pillerIns.HandOrientation, p, false ).Negate() ;
                    if ( teiketsuType == Master.ClsTeiketsuHojoCsv.TeiketsuC ) {
                      plP += jins.GetTransform().BasisY * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 50 ) +
                             jointVec.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 50 ) ;
                      norm = jins.GetTransform().BasisY.Negate() ;
                    }

                    dataList.Add( new FamilyInstanceCreationData( f, plP, norm, sym ) ) ;
                  }
                }
              }
            }
            else if ( jjIds.Count > 0 ) {
              //ウェブ側で上部にNTが来る
              if ( ! isNeedWebNtUp && ( teiketsuType == Master.ClsTeiketsuHojoCsv.TeiketsuC ||
                                        teiketsuType == Master.ClsTeiketsuHojoCsv.TeiketsuNT ) ) {
                XYZ p = pnts.Where( x => nearP.Z > x.Z ).OrderBy( x => x.DistanceTo( nearP ) ).ToList().First() ;
                XYZ plP = p + jointVec * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 13 ) ;
                XYZ norm = GetNTWay( pillerP, pillerIns.HandOrientation, p ) ;
                if ( teiketsuType == Master.ClsTeiketsuHojoCsv.TeiketsuC ) {
                  plP += jins.GetTransform().BasisY * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 50 ) ;
                  plP += norm * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 50 ) ;
                  norm = jins.GetTransform().BasisY.Negate().Normalize() ;
                }

                Face f = GantryUtil.GetBtmFaceOfFamilyInstance( jins ) ;
                if ( f == null ) {
                  continue ;
                }

                dataList.Add( new FamilyInstanceCreationData( f, plP, norm, sym ) ) ;
              }
              //下にＮＴが来る
              else if ( ( teiketsuType == Master.ClsTeiketsuHojoCsv.TeiketsuC ||
                          teiketsuType == Master.ClsTeiketsuHojoCsv.TeiketsuNT ) ) {
                XYZ p = pnts.Where( x => nearP.Z < x.Z ).OrderBy( x => x.DistanceTo( nearP ) ).ToList().First() ;
                XYZ plP = p + jointVec * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 13 ) ;
                XYZ norm = GetNTWay( pillerP, pillerIns.HandOrientation, p, false ) ;
                if ( teiketsuType == Master.ClsTeiketsuHojoCsv.TeiketsuC ) {
                  plP += jins.GetTransform().BasisY * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 50 ) ;
                  plP += norm * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( 50 ) ;
                  norm = jins.GetTransform().BasisY.Negate().Normalize() ;
                }

                Face f = GantryUtil.GetTopFaceOfFamilyInstance( jins ) ;
                if ( f == null ) {
                  continue ;
                }

                dataList.Add( new FamilyInstanceCreationData( f, plP, norm, sym ) ) ;
              }
            }
          }
        }

        var ids = _doc.Create.NewFamilyInstances2( dataList ) ;
        foreach ( ElementId id in ids ) {
          RevitUtil.ClsRevitUtil.SetParameter( _doc, id, DefineUtil.PARAM_HOST_OFFSET, 0 ) ;
          TeiketsuHojo th = new TeiketsuHojo() { m_ElementId = id, m_KodaiName = koudaiName, m_Size = size } ;
          TeiketsuHojo.WriteToElement( th, _doc ) ;
        }

        if ( hasNoChannel ) {
          MessageUtil.Warning( "チャンネル材以外の水平ﾂﾅｷﾞに締結補助材を配置できませんでした", "締結補助材配置" ) ;
        }

        return true ;
      }
      catch ( Exception ex ) {
        _logger.Error( ex.StackTrace ) ;
        return false ;
      }
    }

    /// <summary>
    /// ネコ材を一括で配置する
    /// </summary>
    /// <param name="materials"></param>
    /// <param name="holBraceSize"></param>
    /// <param name="holTsunagiSize"></param>
    /// <param name="vertBraceSize_Web"></param>
    /// <returns></returns>
    private bool CreateNeko( List<MaterialSuper> materials, string horBraceSize, string holTsunagiSize_Web,
      string holTsunagiSize_Frange, string vertBraceSize_Web, string vertBraceSize_Frange, string koudaiName )
    {
      try {
        List<FamilyInstanceCreationData> dataList = new List<FamilyInstanceCreationData>() ;
        FamilySymbol Holsym ;

        //水平ブレス
        string path = Master.ClsTeiketsuHojoCsv.GetFamilyPath( Master.ClsTeiketsuHojoCsv.TypeNeko, horBraceSize ) ;
        string type = Master.ClsTeiketsuHojoCsv.HojoType_B ;
        if ( ! GantryUtil.GetFamilySymbol( _doc, path, type, out Holsym, true ) ) {
          _logger.Error( $"ファミリロードに失敗しました。\r{path}" ) ;
        }

        double nekoSizeHol = RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( getNekoLength( horBraceSize ) ) ;
        if ( ! string.IsNullOrEmpty( horBraceSize ) && Holsym != null && horBraceSize != "なし" ) {
          List<HorizontalBrace> horBraces = materials.Select( x => x as HorizontalBrace ).Where( x =>
            x != null && x.m_ElementId != ElementId.InvalidElementId && x.m_KodaiName == koudaiName ).ToList() ;
          foreach ( HorizontalBrace hBrace in horBraces ) {
            Curve c = GantryUtil.GetCurve( _doc, hBrace.m_ElementId ) ;
            if ( c == null ) {
              continue ;
            }

            FamilyInstance bIns = _doc.GetElement( hBrace.m_ElementId ) as FamilyInstance ;
            Transform ts = bIns.GetTotalTransform() ;
            XYZ p1 = c.GetEndPoint( 0 ) ;
            XYZ p2 = c.GetEndPoint( 1 ) ;
            XYZ vec = ( p2 - p1 ).Normalize() ;
            if ( ! RevitUtil.ClsGeo.IsLeft( ts.BasisY.Normalize(), vec ) ) {
              p1 = c.GetEndPoint( 1 ) ;
              p2 = c.GetEndPoint( 0 ) ;
              vec = ( p2 - p1 ).Normalize() ;
            }

            Face f = GantryUtil.GetSpecifyFaceOfFamilyInstance( bIns, ts.BasisY ) ;
            if ( f != null ) {
              //配置データ
              dataList.Add( new FamilyInstanceCreationData( f, p1 + vec * ( nekoSizeHol ), vec.Normalize(), Holsym ) ) ;
              dataList.Add( new FamilyInstanceCreationData( f, p2 + vec.Negate() * ( nekoSizeHol ), vec.Normalize(),
                Holsym ) ) ;
            }
          }

          //まとめて配置
          var ids = _doc.Create.NewFamilyInstances2( dataList ) ;
          foreach ( ElementId id in ids ) {
            RevitUtil.ClsRevitUtil.SetParameter( _doc, id, DefineUtil.PARAM_HOST_OFFSET, 0 ) ;
            TeiketsuHojo th = new TeiketsuHojo() { m_ElementId = id, m_KodaiName = koudaiName } ;
            TeiketsuHojo.WriteToElement( th, _doc ) ;
          }

          dataList = new List<FamilyInstanceCreationData>() ;
        }

        //垂直ブレース
        path = Master.ClsTeiketsuHojoCsv.GetFamilyPath( Master.ClsTeiketsuHojoCsv.TypeNeko, vertBraceSize_Web ) ;
        type = Master.ClsTeiketsuHojoCsv.HojoType_B ;
        FamilySymbol VerWSym ;
        if ( ! GantryUtil.GetFamilySymbol( _doc, path, type, out VerWSym, true ) ) {
          _logger.Error( $"ファミリロードに失敗しました。\r{path}" ) ;
        }

        double nekoSizeW = RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( getNekoLength( vertBraceSize_Web ) ) ;
        path = Master.ClsTeiketsuHojoCsv.GetFamilyPath( Master.ClsTeiketsuHojoCsv.TypeNeko, vertBraceSize_Frange ) ;
        type = Master.ClsTeiketsuHojoCsv.HojoType_B ;
        FamilySymbol VerFSym ;
        if ( ! GantryUtil.GetFamilySymbol( _doc, path, type, out VerFSym, true ) ) {
          _logger.Error( $"ファミリロードに失敗しました。\r{path}" ) ;
        }

        double nekoSizeF = RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( getNekoLength( vertBraceSize_Frange ) ) ;
        if ( true ) {
          List<VerticalBrace> verBraces = materials.Select( x => x as VerticalBrace ).Where( x =>
            x != null && x.m_ElementId != ElementId.InvalidElementId && x.m_KodaiName == koudaiName ).ToList() ;
          //柱、杭
          List<ElementId> pillers = materials.Select( x => x as PilePillerSuper )
            .Where( x => x != null && x.m_ElementId != ElementId.InvalidElementId && x.m_KodaiName == koudaiName )
            .Select( x => x.m_ElementId ).ToList() ;

          //ブレースと触れる柱を対象とし、交点を求める
          foreach ( VerticalBrace brace in verBraces ) {
            FamilyInstance braceIns = _doc.GetElement( brace.m_ElementId ) as FamilyInstance ;
            //アングル材以外のブレース（ターンバックル）は対象外
            if ( brace.MaterialSize().Shape != MaterialShape.L ) {
              continue ;
            }

            List<ElementId> intsId =
              RevitUtil.ClsRevitUtil.GetIntersectFamilys( _doc, brace.m_ElementId, ignorelist: teikesuList() ) ;
            List<ElementId> pillerIds = intsId.Where( x => pillers.Contains( x ) ).ToList() ;
            Curve c = GantryUtil.GetCurve( _doc, brace.m_ElementId ) ;
            XYZ braceVec = ( c.GetEndPoint( 1 ) - c.GetEndPoint( 0 ) ) ;
            if ( braceVec.Z < 0 ) {
              c = Line.CreateBound( c.GetEndPoint( 1 ), c.GetEndPoint( 0 ) ) ;
              braceVec = ( c.GetEndPoint( 1 ) - c.GetEndPoint( 0 ) ) ;
            }

            XYZ bracePlaneVec = new XYZ( braceVec.X, braceVec.Y, 0 ) ;
            MaterialSize bS = brace.MaterialSize() ;
            double bRotate = bracePlaneVec.AngleTo( braceVec ) ;
            Transform ts = braceIns.GetTotalTransform() ;
            if ( ts.BasisZ.Y == -1 ) {
              bool b = true ;
            }

            foreach ( ElementId id in pillerIds ) {
              FamilyInstance pillerIns = _doc.GetElement( id ) as FamilyInstance ;
              XYZ pillerHandle = pillerIns.HandOrientation ;
              List<XYZ> pnts = GantryUtil.FindIntersectionPnts( _doc, brace.m_ElementId, id ) ;
              bool isFlange = GantryUtil.AreComponentsSameSign( pillerHandle, bracePlaneVec ) ||
                              GantryUtil.AreComponentsSameSign( pillerHandle.Negate(), bracePlaneVec ) ;
              XYZ nearP = ( pnts[ 0 ].DistanceTo( c.GetEndPoint( 0 ) ) < pnts[ 0 ].DistanceTo( c.GetEndPoint( 1 ) ) )
                ? c.GetEndPoint( 0 )
                : c.GetEndPoint( 1 ) ;

              if ( pnts.Count > 1 ) {
                List<XYZ> pList = new List<XYZ>() ;
                if ( isFlange && vertBraceSize_Frange != "なし" && ! string.IsNullOrEmpty( vertBraceSize_Frange ) &&
                     VerFSym != null ) {
                  foreach ( XYZ pn in pnts ) {
                    if ( RevitUtil.ClsRevitUtil.CovertFromAPI( c.Distance( pn ) ) < bS.Thick ) {
                      pList.Add( pn ) ;
                    }
                  }

                  //杭、柱に接する部分のブレース面に配置
                  if ( pList.Count >= 2 ) {
                    XYZ ps = pList.OrderBy( x => x.DistanceTo( nearP ) ).First() ;
                    XYZ pe = pList.OrderBy( x => x.DistanceTo( nearP ) ).Last() ;
                    XYZ vecP = ( pe - ps ) ;
                    if ( RevitUtil.ClsGeo.IsLeft( ps + XYZ.BasisZ, ps + vecP.Normalize() ) ) {
                      ps = pe ;
                      pe = pList.OrderBy( x => x.DistanceTo( nearP ) ).First() ;
                      vecP = ( pe - ps ) ;
                    }

                    XYZ pntMid = ps + vecP / 2 ;
                    ps = pntMid + vecP.Normalize() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( nekoSizeF / 2 ) ;
                    if ( ! GantryUtil.AreComponentsSameSign( braceIns.HandOrientation, vecP ) ) {
                      ps = pntMid + vecP.Normalize().Negate() *
                        RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( nekoSizeF / 2 ) ;
                      vecP = vecP.Negate() ;
                    }

                    Face f = GantryUtil.GetSpecifyFaceOfFamilyInstance( braceIns, ts.BasisZ.Negate() ) ;
                    if ( f != null ) {
                      dataList.Add( new FamilyInstanceCreationData( f, ps, vecP.Normalize(), VerFSym ) ) ;
                    }
                  }
                  else if ( pList.Count == 1 ) {
                    Face f = GantryUtil.GetSpecifyFaceOfFamilyInstance( braceIns, ts.BasisZ.Negate() ) ;
                    if ( f != null ) {
                      XYZ p = pList.First() + braceVec.Negate() *
                        RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( nekoSizeF / 2 ) ;
                      dataList.Add( new FamilyInstanceCreationData( f, p, braceIns.HandOrientation, VerFSym ) ) ;
                    }
                  }
                }
                else if ( ! isFlange && vertBraceSize_Web != "なし" && ! string.IsNullOrEmpty( vertBraceSize_Web ) &&
                          VerWSym != null ) {
                  //上部の点のみ対象
                  foreach ( XYZ pn in pnts ) {
                    if ( RevitUtil.ClsRevitUtil.CovertFromAPI( c.Distance( pn ) ) < bS.Thick ) {
                      pList.Add( pn ) ;
                    }
                  }

                  //杭、柱に接する部分のブレース面に配置
                  if ( pList.Count >= 2 ) {
                    XYZ ps = pList.OrderBy( x => x.DistanceTo( nearP ) ).First() ;
                    XYZ pe = pList.OrderBy( x => x.DistanceTo( nearP ) ).Last() ;
                    XYZ vecP = ( pe - ps ) ;
                    if ( RevitUtil.ClsGeo.IsLeft( ps + XYZ.BasisZ, ps + vecP.Normalize() ) ) {
                      ps = pe ;
                      pe = pList.OrderBy( x => x.DistanceTo( nearP ) ).First() ;
                      vecP = ( pe - ps ) ;
                    }

                    XYZ pntMid = ps + vecP / 2 ;
                    Face Hf = GantryUtil.GetSpecifyFaceOfFamilyInstance( pillerIns,
                      new XYZ( vecP.X, vecP.Y, 0 ).Normalize() ) ;
                    Face noHF = GantryUtil.GetSpecifyFaceOfFamilyInstance( pillerIns,
                      new XYZ( vecP.X, vecP.Y, 0 ).Negate() ) ;
                    XYZ pPi = ( ( pillerIns.Location ) as LocationPoint ).Point ;
                    if ( Hf != null && noHF != null ) {
                      if ( ! RevitUtil.ClsGeo.IsLeft( ts.BasisZ.Normalize(), braceIns.HandOrientation.Normalize() ) ||
                           RevitUtil.ClsGeo.IsLeft( ts.BasisZ.Normalize(), vecP.Normalize() ) ) {
                        ps = ps - XYZ.BasisZ *
                          RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( ( nekoSizeW / 2 ) + ( bS.Height / 2 ) ) ;
                        dataList.Add( new FamilyInstanceCreationData( noHF, ps, XYZ.BasisZ, VerWSym ) ) ;
                        if ( pList.Count >= 4 ) {
                          pe = pe - XYZ.BasisZ *
                            RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( ( nekoSizeW / 2 ) + ( bS.Height / 2 ) ) ;
                          dataList.Add( new FamilyInstanceCreationData( Hf, pe, XYZ.BasisZ.Negate(), VerWSym ) ) ;
                        }
                      }
                      else {
                        if ( pList.Count >= 4 ) {
                          ps = ps - XYZ.BasisZ *
                            RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( ( nekoSizeW / 2 ) + ( bS.Height / 2 ) ) ;
                          dataList.Add( new FamilyInstanceCreationData( noHF, ps, XYZ.BasisZ.Negate(), VerWSym ) ) ;
                        }

                        pe = pe - XYZ.BasisZ *
                          RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( ( nekoSizeW / 2 ) + ( bS.Height / 2 ) ) ;
                        dataList.Add( new FamilyInstanceCreationData( Hf, pe, XYZ.BasisZ, VerWSym ) ) ;
                      }
                    }
                  }
                }
              }
            }
          }

          //まとめて配置
          var ids = _doc.Create.NewFamilyInstances2( dataList ) ;
          foreach ( ElementId id in ids ) {
            RevitUtil.ClsRevitUtil.SetParameter( _doc, id, DefineUtil.PARAM_HOST_OFFSET, 0 ) ;
            TeiketsuHojo th = new TeiketsuHojo() { m_ElementId = id, m_KodaiName = koudaiName } ;
            TeiketsuHojo.WriteToElement( th, _doc ) ;
          }

          dataList = new List<FamilyInstanceCreationData>() ;
        }

        //水平ツナギ
        path = Master.ClsTeiketsuHojoCsv.GetFamilyPath( Master.ClsTeiketsuHojoCsv.TypeNeko, holTsunagiSize_Web ) ;
        type = Master.ClsTeiketsuHojoCsv.HojoType_T ;
        FamilySymbol tWSym = null ;
        FamilySymbol tFSym = null ;
        if ( ! GantryUtil.GetFamilySymbol( _doc, path, type, out tWSym, true ) ) {
          _logger.Error( $"ファミリロードに失敗しました。\r{path}" ) ;
        }

        nekoSizeW = RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( getNekoLength( holTsunagiSize_Web ) ) ;
        path = Master.ClsTeiketsuHojoCsv.GetFamilyPath( Master.ClsTeiketsuHojoCsv.TypeNeko, holTsunagiSize_Frange ) ;
        type = Master.ClsTeiketsuHojoCsv.HojoType_T ;
        if ( ! GantryUtil.GetFamilySymbol( _doc, path, type, out tFSym, true ) ) {
          _logger.Error( $"ファミリロードに失敗しました。\r{path}" ) ;
        }

        nekoSizeF = RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( getNekoLength( holTsunagiSize_Frange ) ) ;
        if ( true ) {
          //継材
          List<ElementId> joints = materials.Select( x => x as HorizontalJoint )
            .Where( x => x != null && x.m_ElementId != ElementId.InvalidElementId && x.m_KodaiName == koudaiName )
            .Select( x => x.m_ElementId ).ToList() ;

          //柱、杭
          List<PilePillerSuper> pillers = materials.Select( x => x as PilePillerSuper ).Where( x =>
            x != null && x.m_ElementId != ElementId.InvalidElementId && x.m_KodaiName == koudaiName ).ToList() ;

          List<(XYZ, XYZ, Face, bool)> lstOrderP = new List<(XYZ, XYZ, Face, bool)>() ;
          if ( joints.Count > 0 ) {
            foreach ( PilePillerSuper pil in pillers ) {
              FamilyInstance pillerIns = _doc.GetElement( pil.m_ElementId ) as FamilyInstance ;
              //柱に接するファミリのリスト(締結補助材は除く)
              List<ElementId> intsId = RevitUtil.ClsRevitUtil.GetIntersectFamilys( _doc, pil.m_ElementId,
                ignorelist: teikesuList(), serchIds: joints ) ;
              //柱に接するツナギ材だけを収集
              List<ElementId> jointIds = intsId.Where( x => joints.Contains( x ) ).ToList() ;
              //柱の基点
              XYZ pillerP = pil.Position ;

              foreach ( ElementId jId in jointIds ) {
                FamilyInstance jins = _doc.GetElement( jId ) as FamilyInstance ;
                Curve jC = GantryUtil.GetCurve( _doc, jId ) ;
                XYZ jointVec = ( jC.GetEndPoint( 1 ) - jC.GetEndPoint( 0 ) ).Normalize() ;
                //ツナギ材に接する別のツナギ材を取得
                List<ElementId> jintsId =
                  RevitUtil.ClsRevitUtil.GetIntersectFamilys( _doc, jId, ignorelist: teikesuList(),
                    serchIds: jointIds ) ;
                List<ElementId> jjIds = jintsId.Where( x => joints.Contains( x ) && intsId.Contains( x ) ).ToList() ;
                //このツナギがフランジ方向に平行か
                bool isFlange = GantryUtil.AreComponentsSameSign( pillerIns.HandOrientation, jointVec ) ||
                                GantryUtil.AreComponentsSameSign( pillerIns.HandOrientation, jointVec.Negate() ) ;

                //ツナギと柱の交点
                List<XYZ> pnts = GantryUtil.FindIntersectionPnts( _doc, pil.m_ElementId, jId ) ;
                if ( pnts.Count < 1 ) {
                  continue ;
                }

                XYZ nearP =
                  ( pnts[ 0 ].DistanceTo( jC.GetEndPoint( 0 ) ) < pnts[ 0 ].DistanceTo( jC.GetEndPoint( 1 ) ) )
                    ? jC.GetEndPoint( 0 )
                    : jC.GetEndPoint( 1 ) ;
                //交点群が近いほうの点からのベクトル
                if ( nearP.DistanceTo( jC.GetEndPoint( 1 ) ) < 0.1 ) {
                  jointVec = jointVec.Negate() ;
                  jC = Line.CreateBound( jC.GetEndPoint( 1 ), jC.GetEndPoint( 0 ) ) ;
                }

                //切梁受けはベース線の位置が
                if ( jins.Symbol.Name.Contains( "切梁受" ) ) {
                  double kiribariH = jins.LookupParameter( DefineUtil.PARAM_HOST_OFFSET ).AsDouble() ;
                  nearP = nearP + XYZ.BasisZ.Normalize() * kiribariH ;
                  jC = Line.CreateBound( jC.GetEndPoint( 0 ) + XYZ.BasisZ.Normalize() * kiribariH,
                    jC.GetEndPoint( 1 ) + XYZ.BasisZ.Normalize() * kiribariH ) ;
                  jointVec = ( jC.GetEndPoint( 1 ) - jC.GetEndPoint( 0 ) ).Normalize() ;
                }

                //ツナギ材のマテリアルクラス
                HorizontalJoint mainJ = HorizontalJoint.ReadFromElement( jId, _doc ) as HorizontalJoint ;
                if ( mainJ == null ) {
                  continue ;
                }

                //接するものをすべて考慮し、あとから配置するためフラグを持っておく
                bool hasCrossNear = false ;
                bool hasCrossFar = false ;
                bool hasCrossUp = false ;
                bool noCross = true ;
                bool hasNext = false ;

                //接するツナギ材を確認
                foreach ( ElementId id in jjIds ) {
                  if ( id == jId ) {
                    continue ;
                  }

                  FamilyInstance jjins = _doc.GetElement( id ) as FamilyInstance ;
                  HorizontalJoint jH = HorizontalJoint.ReadFromElement( id, _doc ) as HorizontalJoint ;
                  if ( jH == null ) {
                    continue ;
                  }

                  //接するツナギ材のどちらが高い位置にあるか返す(同じ高さの場合はnull)
                  FamilyInstance intIns = GetHigherInstance( jins, jjins ) ;
                  Curve jjC = GantryUtil.GetCurve( _doc, id ) ;
                  XYZ jjointVec = ( jjC.GetEndPoint( 1 ) - jjC.GetEndPoint( 0 ) ).Normalize() ;
                  //ツナギとツナギの交点を算出
                  List<XYZ> pntsJ = GantryUtil.FindIntersectionPnts( _doc, jId, id ) ;
                  if ( pntsJ.Count < 1 ) {
                    continue ;
                  }

                  XYZ nearPJ =
                    ( pntsJ[ 0 ].DistanceTo( jjC.GetEndPoint( 0 ) ) < pntsJ[ 0 ].DistanceTo( jjC.GetEndPoint( 1 ) ) )
                      ? jjC.GetEndPoint( 0 )
                      : jjC.GetEndPoint( 1 ) ;
                  pillerP = new XYZ( pillerP.X, pillerP.Y, nearPJ.Z ) ;
                  if ( jjC.GetEndPoint( 1 ).DistanceTo( nearPJ ) < 0.1 ) {
                    jjC = Line.CreateBound( jjC.GetEndPoint( 1 ), jjC.GetEndPoint( 0 ) ) ;
                    jjointVec = ( jjC.GetEndPoint( 1 ) - jjC.GetEndPoint( 0 ) ).Normalize() ;
                  }

                  //切梁受けはベース線の位置が
                  if ( jjins.Symbol.Name.Contains( "切梁受" ) ) {
                    double kiribariH = jjins.LookupParameter( DefineUtil.PARAM_HOST_OFFSET ).AsDouble() ;
                    nearPJ = nearPJ + XYZ.BasisZ.Normalize() * kiribariH ;
                    jjC = Line.CreateBound( jjC.GetEndPoint( 0 ) + XYZ.BasisZ.Normalize() * kiribariH,
                      jjC.GetEndPoint( 1 ) + XYZ.BasisZ.Normalize() * kiribariH ) ;
                    jointVec = ( jjC.GetEndPoint( 1 ) - jjC.GetEndPoint( 0 ) ).Normalize() ;
                  }


                  //自信がフランジ側
                  if ( isFlange && intIns == null ) {
                    //隣り合う水平材がある場合
                    if ( nearP.DistanceTo( jjC.GetEndPoint( 0 ) ) < 0.1 ||
                         nearP.DistanceTo( jjC.GetEndPoint( 0 ) ) < 0.1 ) {
                      hasNext = true ;
                    }
                  }
                  //自信がフランジ側で、ウェブとの交差で自信が上側にいる
                  else if ( isFlange && intIns.Id.Equals( jId ) ) {
                    noCross = false ;
                    if ( GantryUtil.AreComponentsSameSign( jointVec.Normalize(),
                          jjins.GetTotalTransform().BasisY.Negate().Normalize() ) ) {
                      hasCrossNear = true ;
                    }
                    else {
                      hasCrossFar = true ;
                    }
                  }
                  //フランジ側部材で、ウェブとの交差で下側にいる
                  else if ( isFlange && intIns.Id.Equals( id ) ) {
                    noCross = false ;
                    hasCrossUp = true ;
                    if ( GantryUtil.AreComponentsSameSign( jointVec.Normalize(),
                          jjins.GetTotalTransform().BasisY.Negate().Normalize() ) ) {
                      hasCrossNear = true ;
                    }
                    else {
                      hasCrossFar = true ;
                    }
                  }
                  ///ウェブ側で下にいる
                  else if ( ! isFlange && intIns != null && intIns.Id.Equals( id ) ) {
                    hasCrossUp = true ;
                    if ( GantryUtil.AreComponentsSameSign( jointVec.Normalize(),
                          jjins.GetTotalTransform().BasisY.Negate().Normalize() ) ) {
                      hasCrossNear = true ;
                    }
                    else {
                      hasCrossFar = true ;
                    }
                  }
                  else if ( ! isFlange && intIns != null && intIns.Id.Equals( jId ) ) {
                    hasCrossUp = false ;
                    if ( GantryUtil.AreComponentsSameSign( jointVec.Normalize(),
                          jjins.GetTotalTransform().BasisY.Negate().Normalize() ) ) {
                      hasCrossNear = true ;
                    }
                    else {
                      hasCrossFar = true ;
                    }
                  }
                }


                pillerP = new XYZ( pillerP.X, pillerP.Y, nearP.Z ) ;

                bool isHlf = isHalfAtach( isFlange, pil, pillerP, nearP ) ;
                if ( isFlange && ! string.IsNullOrEmpty( holTsunagiSize_Frange ) && holTsunagiSize_Frange != "なし" &&
                     tFSym != null ) {
                  Face topFace = GantryUtil.GetTopFaceOfFamilyInstance( jins ) ;
                  Face btmFace = GantryUtil.GetBtmFaceOfFamilyInstance( jins ) ;
                  XYZ up = jins.GetTotalTransform().BasisZ.Z > 0
                    ? jins.GetTotalTransform().BasisZ
                    : jins.GetTotalTransform().BasisZ.Negate() ;
                  double leng = holTsunagiSize_Frange.StartsWith( "B" ) ? 100 : 75 ;
                  XYZ pntUp = null ;
                  XYZ pntBtm = null ;
                  XYZ normTop = null ;
                  XYZ normBtm = null ;
                  //継材中央位置にネコ材が来る
                  if ( pnts.Count >= 16 && ( noCross || ( hasCrossFar || hasCrossNear ) ) ) {
                    XYZ norm = jins.GetTransform().BasisZ.Z > 0 ? jins.HandOrientation : jins.HandOrientation.Negate() ;
                    if ( RevitUtil.ClsGeo.IsLeft( jins.FacingOrientation.Normalize(),
                          jins.HandOrientation.Normalize() ) ) {
                      norm = norm.Negate() ;
                    }

                    List<XYZ> topPnts = pnts.Where( x => x.Z > nearP.Z ).OrderBy( x => x.DistanceTo( nearP ) )
                      .ToList() ;
                    List<XYZ> btmPnts = pnts.Where( x => x.Z < nearP.Z ).OrderBy( x => x.DistanceTo( nearP ) )
                      .ToList() ;
                    if ( topPnts.Count < 2 ) {
                      continue ;
                    }

                    XYZ mid = ( topPnts.First() + topPnts.Last() ) / 2 ;
                    pntUp = mid ;
                    normTop = norm.Negate() ;
                    pntUp = pntUp + jins.FacingOrientation.Normalize() *
                      RevitUtil.ClsRevitUtil.CovertToAPI( pil.SteelSize.FrangeThick / 2 ) ;
                    if ( btmPnts.Count < 2 ) {
                      continue ;
                    }

                    mid = ( btmPnts.First() + btmPnts.Last() ) / 2 ;
                    pntBtm = mid ;
                    normBtm = norm ;
                    pntBtm = pntBtm + jins.FacingOrientation.Normalize() *
                      RevitUtil.ClsRevitUtil.CovertToAPI( pil.SteelSize.FrangeThick / 2 ) ;
                  }
                  else if ( hasNext || isHlf ) {
                    XYZ norm = jins.GetTransform().BasisZ.Z > 0 ? jins.HandOrientation : jins.HandOrientation.Negate() ;
                    if ( RevitUtil.ClsGeo.IsLeft( jins.FacingOrientation.Normalize(),
                          jins.HandOrientation.Normalize() ) ) {
                      norm = norm.Negate() ;
                    }

                    XYZ mid = nearP + up *
                      RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( ( mainJ.MaterialSize().Height ) / 2 ) ;
                    pntUp = mid ;
                    normTop = norm.Negate() ;
                    //pntUp = pntUp + jins.GetTotalTransform().BasisY.Normalize() * RevitUtil.ClsRevitUtil.CovertToAPI(pil.SteelSize.FrangeThick/2);
                    mid = nearP + up.Negate() *
                      RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( ( mainJ.MaterialSize().Height ) / 2 ) ;
                    pntBtm = mid ;
                    normBtm = norm ;
                    //pntBtm = pntBtm + jins.GetTotalTransform().BasisY.Normalize() * RevitUtil.ClsRevitUtil.CovertToAPI(pil.SteelSize.FrangeThick/2);
                  }


                  //if (!hasCrossUp && !isHlf)
                  //{
                  //    if (hasCrossFar)
                  //    {
                  //        pntUp += jointVec.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(leng);
                  //    }

                  //    if (hasCrossNear)
                  //    {
                  //        pntUp += jointVec * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(leng);
                  //    }
                  //}
                  //else if (hasCrossUp && !noCross && !isHlf)
                  //{
                  //    if (hasCrossFar)
                  //    {
                  //        pntBtm += jointVec.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(leng);
                  //    }

                  //    if (hasCrossNear)
                  //    {
                  //        pntBtm += jointVec * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(leng);
                  //    }
                  //}


                  //if (hasCrossUp && pntUp != null)
                  //{
                  //    pntUp += ((hasCrossNear) ? jointVec : jointVec.Negate()) * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(leng);
                  //}
                  //else if (!hasCrossUp && pntBtm != null)
                  //{
                  //    pntBtm += ((hasCrossNear) ? jointVec.Negate() : jointVec) * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(leng);
                  //}

                  if ( pntUp != null && normTop != null && topFace != null ) {
                    bool has = false ;
                    foreach ( (XYZ, XYZ, Face, bool) p in lstOrderP ) {
                      if ( p.Item1.DistanceTo( pntUp ) < RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( leng ) &&
                           ( GantryUtil.AreComponentsSameSign( p.Item2, normTop ) ||
                             GantryUtil.AreComponentsSameSign( p.Item2, normTop.Negate() ) ) ) {
                        has = true ;
                        break ;
                      }
                    }

                    if ( ! has ) {
                      lstOrderP.Add( ( pntUp, normTop, topFace, isFlange ) ) ;
                    }
                  }

                  if ( pntBtm != null && normBtm != null && btmFace != null ) {
                    bool has = false ;
                    foreach ( (XYZ, XYZ, Face, bool) p in lstOrderP ) {
                      if ( p.Item1.DistanceTo( pntBtm ) < RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( leng ) &&
                           ( GantryUtil.AreComponentsSameSign( p.Item2, normBtm ) ||
                             GantryUtil.AreComponentsSameSign( p.Item2, normBtm.Negate() ) ) ) {
                        has = true ;
                        break ;
                      }
                    }

                    if ( ! has ) {
                      lstOrderP.Add( ( pntBtm, normBtm, btmFace, isFlange ) ) ;
                    }
                  }
                }
                else if ( ! isFlange && ! string.IsNullOrEmpty( holTsunagiSize_Web ) && holTsunagiSize_Web != "なし" &&
                          tWSym != null ) {
                  double thicks = holTsunagiSize_Web.StartsWith( "B" ) ? 10 :
                    holTsunagiSize_Web.StartsWith( "A" ) ? 9 : 6 ;
                  Face Hf = GantryUtil.GetSpecifyFaceOfFamilyInstance( pillerIns,
                    new XYZ( jointVec.X, jointVec.Y, 0 ).Normalize() ) ;
                  Face noHF = GantryUtil.GetSpecifyFaceOfFamilyInstance( pillerIns,
                    new XYZ( jointVec.X, jointVec.Y, 0 ).Negate() ) ;
                  XYZ vecUP = ( jins.GetTransform().BasisZ.Z > 0 )
                    ? jins.GetTransform().BasisZ
                    : jins.GetTransform().BasisZ.Negate() ;
                  Curve lC = Line.CreateBound(
                    jC.GetEndPoint( 0 ) + vecUP.Negate() *
                    RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( mainJ.MaterialSize().Height / 2 ),
                    jC.GetEndPoint( 1 ) + vecUP.Negate() *
                    RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( mainJ.MaterialSize().Height / 2 ) ) ;
                  List<XYZ> lowerPnts = pnts.Where( x =>
                    ToCurve( jC, x ).Z > 0 && RevitUtil.ClsRevitUtil.CovertFromAPI( lC.Distance( x ) ) <
                    RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( thicks / 2 ) ).ToList() ;
                  XYZ vecToC = ToCurve( jC, pillerP ) ;
                  bool isLeft = RevitUtil.ClsGeo.IsLeft( new XYZ( jointVec.X, jointVec.Y, 0 ).Normalize(),
                    new XYZ( vecToC.X, vecToC.Y, 0 ) ) ;
                  XYZ pntN = null,
                    pntF = null,
                    normN = isLeft ? XYZ.BasisZ : XYZ.BasisZ.Negate(),
                    normF = isLeft ? XYZ.BasisZ.Negate() : XYZ.BasisZ ;
                  double half = Math.Abs( ( nekoSizeW / 2 ) -
                                          RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit(
                                            mainJ.MaterialSize().Height / 2 ) ) ;

                  //フランジの片側にしか接していない
                  if ( isHlf ) {
                    pntF = lowerPnts.OrderBy( x => x.DistanceTo( nearP ) ).ThenBy( x => x.Z ).Last() + vecUP *
                      RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( mainJ.MaterialSize().Height / 2 ) ;
                  }
                  else {
                    if ( lowerPnts.Count > 0 ) {
                      pntF = lowerPnts.OrderBy( x => x.DistanceTo( nearP ) ).ThenBy( x => x.Z ).Last() + vecUP *
                        RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( mainJ.MaterialSize().Height / 2 ) ;
                      pntN = lowerPnts.OrderBy( x => x.DistanceTo( nearP ) ).ThenBy( x => x.Z ).First() + vecUP *
                        RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( mainJ.MaterialSize().Height / 2 ) ;
                    }
                  }

                  if ( hasCrossUp ) {
                    if ( hasCrossFar && pntF != null ) {
                      pntF += vecUP.Negate() * half ;
                    }

                    if ( hasCrossNear && pntN != null ) {
                      pntN += vecUP.Negate() * half ;
                    }
                  }
                  else {
                    if ( hasCrossFar && pntF != null ) {
                      pntF += vecUP * half ;
                    }

                    if ( hasCrossNear && pntN != null ) {
                      pntN += vecUP * half ;
                    }
                  }

                  if ( pntF != null ) {
                    lstOrderP.Add( ( pntF, normF, Hf, false ) ) ;
                  }

                  if ( pntN != null ) {
                    lstOrderP.Add( ( pntN, normN, noHF, false ) ) ;
                  }
                }
              }
            }

            foreach ( (XYZ, XYZ, Face, bool) k in lstOrderP ) {
              FamilySymbol tsym = k.Item4 ? tFSym : tWSym ;
              if ( tsym != null ) {
                dataList.Add( new FamilyInstanceCreationData( k.Item3, k.Item1, k.Item2, tsym ) ) ;
              }
            }
          }


          //まとめて配置
          var ids = _doc.Create.NewFamilyInstances2( dataList ) ;
          foreach ( ElementId id in ids ) {
            RevitUtil.ClsRevitUtil.SetParameter( _doc, id, DefineUtil.PARAM_HOST_OFFSET, 0 ) ;
            TeiketsuHojo th = new TeiketsuHojo() { m_ElementId = id, m_KodaiName = koudaiName } ;
            TeiketsuHojo.WriteToElement( th, _doc ) ;
          }

          dataList = new List<FamilyInstanceCreationData>() ;
        }

        return true ;
      }
      catch ( Exception ex ) {
        _logger.Error( ex.StackTrace ) ;
        return false ;
      }
    }

    private XYZ ToCurve( Curve c, XYZ pnt )
    {
      XYZ p = GantryUtil.ProjectPointToCurve( pnt, c ) ;
      return ( p - pnt ).Normalize() ;
    }

    /// <summary>
    /// 文字列からネコ材の長さを返す
    /// </summary>
    /// <param name="nekoSize"></param>
    /// <returns></returns>
    private double getNekoLength( string nekoSize )
    {
      string leng = nekoSize.Substring( nekoSize.IndexOf( '-' ) + 1, 2 ) ;
      return RevitUtil.ClsCommonUtils.ChangeStrToDbl( leng ) * 10 ;
    }

    /// <summary>
    /// NT型の向きを柱の位置から割り出す
    /// </summary>
    /// <param name="pillerP"></param>
    /// <param name="pnt"></param>
    /// <returns></returns>
    private XYZ GetNTWay( XYZ pillerP, XYZ pillerHandle, XYZ pnt, bool isUpside = true )
    {
      XYZ retXYZ = pillerHandle.CrossProduct( XYZ.BasisZ ).Normalize() ;
      XYZ newpnt = new XYZ( pnt.X, pnt.Y, pillerP.Z ) ;
      XYZ pT = ( newpnt - pillerP ).Normalize() ;
      if ( ! RevitUtil.ClsGeo.IsLeft( pillerHandle, retXYZ ) ) {
        retXYZ = retXYZ.Negate() ;
      }

      if ( pillerHandle.AngleTo( pT ) > ( Math.PI / 2 ) ) {
        retXYZ = retXYZ.Negate() ;
      }

      return ( isUpside ) ? retXYZ : retXYZ.Negate() ;
    }

    /// <summary>
    /// 二つのインスタンスを比べて、より高い位置にあるインスタンスを返す
    /// </summary>
    /// <param name="instance1"></param>
    /// <param name="instance2"></param>
    /// <returns></returns>
    private FamilyInstance GetHigherInstance( FamilyInstance instance1, FamilyInstance instance2 )
    {
      // instance1の高さを取得
      Curve locationCurve1 = GantryUtil.GetCurve( _doc, instance1.Id ) ;
      double height1 = locationCurve1.GetEndPoint( 0 ).Z ; // 開始点のZ座標を取得（高さ）

      // instance2の高さを取得
      Curve locationCurve2 = GantryUtil.GetCurve( _doc, instance2.Id ) ;
      double height2 = locationCurve2.GetEndPoint( 0 ).Z ; // 開始点のZ座標を取得（高さ）

      // より高い位置にあるファミリインスタンスを返す
      if ( RevitUtil.ClsGeo.GEO_GT( height1, height2 ) ) {
        return instance1 ;
      }
      else if ( RevitUtil.ClsGeo.GEO_GT( height2, height1 ) ) {
        return instance2 ;
      }
      else {
        return null ;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="isFlange"></param>
    /// <param name="piller"></param>
    /// <param name="joint"></param>
    /// <returns></returns>
    private bool isHalfAtach( bool isFlange, PilePillerSuper piller, XYZ pillerP, XYZ nearP )
    {
      bool retB = false ;
      double flangeW = piller.MaterialSize().Height ;
      double webW = piller.MaterialSize().Width ;
      if ( isFlange ) {
        double atan = Math.Atan2( webW / 2, flangeW / 2 ) ;
        double dist = RevitUtil.ClsRevitUtil.CovertFromAPI( pillerP.DistanceTo( nearP ) ) ;
        double asin = Math.Asin( flangeW / dist ) ;
        retB = RevitUtil.ClsGeo.GEO_EQ( dist, webW / 2 ) ? true : atan <= asin ;
      }
      else {
        double atan = Math.Atan2( webW / 2, flangeW / 2 ) ;
        double dist = RevitUtil.ClsRevitUtil.CovertFromAPI( pillerP.DistanceTo( nearP ) ) ;
        double asin = Math.Asin( flangeW / dist ) ;
        retB = RevitUtil.ClsGeo.GEO_EQ( dist, flangeW / 2 ) ? true : atan <= asin ;
      }

      return retB ;
    }


    /// <summary>
    /// 交差するファミリを見つける
    /// </summary>
    /// <param name="c"></param>
    /// <param name="ids"></param>
    /// <returns></returns>
    private List<(XYZ, ElementId)> CalcCurveIntersection( Curve c, List<ElementId> ids )
    {
      List<(XYZ, ElementId)> list = new List<(XYZ, ElementId)>() ;
      try {
        foreach ( ElementId id in ids ) {
          Curve tC = GantryUtil.GetCurve( _doc, id ) ;
          XYZ intPnt = FindIntersectionPoint( c, tC ) ;
          if ( intPnt != null ) {
            list.Add( ( intPnt, id ) ) ;
          }
        }
      }
      catch ( Exception ) {
      }

      return list ;
    }

    /// <summary>
    /// Z軸に平行でない線分二つの交点を取得する
    /// </summary>
    /// <param name="curve1"></param>
    /// <param name="curve2"></param>
    /// <returns>交差しない場合はnull</returns>
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
        XYZ projectedPoint = ProjectPointToCurve( intersectionPoint, curve1 ) ;

        return projectedPoint ;
      }

      return null ;
    }

    /// <summary>
    /// CurveをXY平面上の線分に変換
    /// </summary>
    /// <param name="curve"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Curve上にpointを投影する
    /// </summary>
    /// <param name="point"></param>
    /// <param name="curve"></param>
    /// <returns></returns>
    private XYZ ProjectPointToCurve( XYZ point, Curve curve )
    {
      try {
        double parameter = curve.Project( point ).Parameter ;
        XYZ projectedPoint = curve.Evaluate( parameter, false ) ;
        return projectedPoint ;
      }
      catch ( Exception ex ) {
        return null ;
      }
    }

    /// <summary>
    /// 配置されている切梁受けを水平つなぎとして内部情報を持たせる
    /// </summary>
    private List<ElementId> ReckonKiribariUkeAsHorizontalJoint( string koudaiName )
    {
      YMSReckonKiribariUkeAsHolJoinFilter filter = new YMSReckonKiribariUkeAsHolJoinFilter( _doc ) ;
      Selection selection = _uiDoc.Selection ;
      IList<Reference> res = selection.PickObjects( ObjectType.Element, filter ) ;

      List<ElementId> ids = new List<ElementId>() ;
      foreach ( Reference refer in res ) {
        FamilyInstance ins = _doc.GetElement( refer.ElementId ) as FamilyInstance ;
        if ( ins != null ) {
          string symName = ins.Symbol.Family.Name ;
          symName = symName.Replace( "形鋼_", "" ).Replace( "x", "X" ).Replace( "C", "[" ) ;
          if ( ! symName.StartsWith( "[" ) ) {
            continue ;
          }

          HorizontalJoint hor = new HorizontalJoint() ;
          hor.m_ElementId = ins.Id ;
          hor.m_KodaiName = koudaiName ;
          hor.m_Size = symName ;
          HorizontalJoint.WriteToElement( hor, _doc ) ;
          ids.Add( ins.Id ) ;
        }
      }

      return ids ;
    }
  }
}