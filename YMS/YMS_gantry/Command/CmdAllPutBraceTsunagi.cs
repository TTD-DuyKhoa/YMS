using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.IO ;
using System.Linq ;
using System.Windows.Forms ;
using YMS_gantry.Data ;
using YMS_gantry.Master ;
using YMS_gantry.UI ;
using static YMS_gantry.UI.FrmCreateBraceAndJointViewModel ;

namespace YMS_gantry.Command
{
  internal class CmdAllPutBraceTsunagi
  {
    public enum MetalShape
    {
      L,
      H,
      C
    }

    static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger() ;

    public bool Execute( UIDocument uidoc )
    {
      var masterHBeam = ClsHBeamCsv.GetCsvData() ;
      var doc = uidoc.Document ;
      var f = new FrmCreateBraceAndJoint() ;
      var modelData = ModelData.GetModelData( doc ) ;
      foreach ( var x in modelData.ListKoudaiData.Select( x => x.AllKoudaiFlatData ) )
        f.ViewModel.KodaiSet.Add( x ) ;
      f.ViewModel.RevitDoc = doc ;
      if ( f.ShowDialog() != System.Windows.Forms.DialogResult.OK ) {
        return false ;
      }

      using ( var transaction = new Transaction( doc ) ) {
        #if DEBUG
        #else
                try
        #endif
        {
          transaction.Start( Guid.NewGuid().ToString() ) ;
          var viewModel = f.ViewModel ;

          var kodaiName = viewModel.SelectedKodai.KoudaiName ;
          var kodaiData = viewModel.SelectedKodai ;
          var danData = viewModel.DanRowSet ;
          var pillarHSet = kodaiData.PillarHSetFt() ;
          var pillarKSet = kodaiData.PillarKSetFt() ;
          if ( kodaiData.IsFirstShikigeta ) {
            pillarKSet = pillarKSet.Skip( 1 ).ToArray() ;
          }

          if ( kodaiData.IsLastShikigeta ) {
            pillarKSet = pillarKSet.Take( pillarKSet.Length - 1 ).ToArray() ;
          }

          var (origin, axisH, axisK, axisZ) = kodaiData.GetBasis( doc ) ;

          var baseLevel = new FilteredElementCollector( doc ).OfClass( typeof( Level ) ).Cast<Level>()
            .FirstOrDefault( x => x.Name == kodaiData.SelectedLevel ) ;

          if ( baseLevel == null ) {
            MessageUtil.Error( "構台の基準レベルが取得できません。", "error" ) ;
            return false ;
          }

          // 最高大引下端の取得
          var highestOhbikiBottomFt = MaterialSuper.Collect( doc ).Where( x => x.m_KodaiName == kodaiName )
            .Select( x => x as Ohbiki ).Where( x => x != null ).Select( x =>
              ClsRevitUtil.GetParameterDouble( doc, x.m_ElementId, DefineUtil.PARAM_BASE_OFFSET ) -
              0.5 * ( x.SteelSize?.HeightFeet ?? 0.0 ) ).Max() ;
          var danTopSet = Enumerable.Range( 0, danData.Count )
            .Select( x => danData.Take( x ).Select( y => y.IntervalFt ).Sum() ).ToArray() ;

          var jointSizeH = 0.0 ;
          if ( viewModel.HrzJointSize is ClsChannelCsv channelRec ) {
            jointSizeH = channelRec.HFt ;
          }
          else if ( viewModel.HrzJointSize is ClsHBeamCsv hbeamRec ) {
            jointSizeH = hbeamRec.BFt ;
          }

          var pillarSize = ClsMasterCsv.Shared.Select( x => x as ClsHBeamCsv )
            .FirstOrDefault( x => x?.Size == kodaiData.pilePillerData.PillarSize.Replace( "-", "" ) ) ;
          var pillarSizeH = pillarSize?.HFt ?? 0.0 ;
          var pillarSizeB = pillarSize?.BFt ?? 0.0 ;

          // 水平つなぎ
          if ( viewModel.HrzJointHasK || viewModel.HrzJointHasH ) {
            var metalShape = MetalShape.C ;
            if ( viewModel.HrzJointSize is ClsHBeamCsv )
              metalShape = MetalShape.H ;
            else if ( viewModel.HrzJointSize is ClsAngleCsv )
              metalShape = MetalShape.L ;

            //var familyPath = System.IO.Path.Combine(PathUtil.GetYMSFolder(), viewModel.HrzJointSize.FamilyPath);
            var familyPath = Directory.EnumerateFiles( PathUtil.GetYMSFolder(),
              Path.GetFileName( viewModel.HrzJointSize.FamilyPath ), SearchOption.AllDirectories ).FirstOrDefault() ;
            var familyName = Path.GetFileNameWithoutExtension( familyPath ) ;
            var familyTypeName = viewModel.HrzJointSize.HrzJoint ; // "水平ツナギ";// "ﾂﾅｷﾞ";
            if ( GantryUtil.GetFamilySymbol( doc, familyPath, familyTypeName, out var symbol, true ) ) {
              var pillarMaster =
                masterHBeam.FirstOrDefault( x => x.Size == kodaiData.pilePillerData.PillarSize.Replace( "-", "" ) ) ;
              // 水平つなぎ
              for ( int indexDan = 0 ; indexDan < danData.Count ; indexDan++ ) {
                var dan = danData.ElementAtOrDefault( indexDan ) ;
                if ( dan == null ) continue ;
                if ( ! dan.HasJoint ) {
                  continue ;
                }

                var levelFromTopFt = -danData.Take( indexDan + 1 ).Select( x => x.IntervalFt ).Sum() ;
                var levelOffsetFt = baseLevel.ProjectElevation + highestOhbikiBottomFt + levelFromTopFt ;

                var danPointOnPlane = levelOffsetFt * XYZ.BasisZ ;
                var danRefPlane = doc.Create.NewReferencePlane( danPointOnPlane, danPointOnPlane + XYZ.BasisY,
                  XYZ.BasisX, doc.ActiveView ) ;
                danRefPlane.Maximize3DExtents() ;


                // 0:橋長方向, 1:幅員方向
                var requireFlagSet = new bool[] { viewModel.HrzJointHasK, viewModel.HrzJointHasH } ;
                var pillarKSetSet = new double[][] { pillarKSet, pillarHSet } ;
                var pillarHSetSet = new double[][] { pillarHSet, pillarKSet, } ;
                var dirKSet = new XYZ[] { axisK, axisH } ;
                var dirHSet = new XYZ[] { axisH, axisK } ;

                var hbeamB = 0.0 ;
                if ( viewModel.HrzJointSize is ClsHBeamCsv hbeamRec ) {
                  hbeamB = hbeamRec.BFt ;
                }

                // 以下2つはL材とH鋼とC材で要分岐
                var pillarWidthSet = Enumerable.Range( 0, 2 ).Select( x => 0.0 ) ;
                switch ( metalShape ) {
                  case MetalShape.L :
                  case MetalShape.C :
                    pillarWidthSet = new double[] { 0.5 * pillarMaster.HFt, -0.5 * pillarMaster.BFt, } ;
                    break ;
                  case MetalShape.H :
                    pillarWidthSet = new double[]
                    {
                      0.5 * ( pillarMaster.HFt + hbeamB ), -0.5 * ( pillarMaster.BFt + hbeamB ),
                    } ;
                    break ;
                }

                var levelDiffSet = Enumerable.Range( 0, 2 ).Select( x => 0.0 ) ;
                var angleLength = 0.0 ;
                if ( viewModel.HrzJointSize is ClsAngleCsv angleRec ) {
                  angleLength = angleRec.AFt ;
                }

                switch ( metalShape ) {
                  case MetalShape.L :
                    levelDiffSet = new double[]
                    {
                      viewModel.HrzJointLowerIsH ? angleLength : 0.0, viewModel.HrzJointLowerIsH ? 0.0 : angleLength,
                    } ;
                    break ;
                  case MetalShape.H :
                  case MetalShape.C :
                    levelDiffSet = new double[]
                    {
                      viewModel.HrzJointLowerIsH ? 0.5 * jointSizeH : -0.5 * jointSizeH,
                      viewModel.HrzJointLowerIsH ? -0.5 * jointSizeH : 0.5 * jointSizeH,
                    } ;
                    break ;
                }

                for ( int i = 0 ; i < 2 ; i++ ) {
                  // 橋長側のつなぎ
                  if ( requireFlagSet.ElementAtOrDefault( i ) ) {
                    var pillarSet = pillarKSetSet.ElementAtOrDefault( i ) ;
                    var minK = pillarSet.FirstOrDefault() ;
                    var maxK = pillarSet.LastOrDefault() ;

                    var pillarHeightFt = pillarWidthSet.ElementAtOrDefault( i ) ;

                    var levelDiff = levelDiffSet.ElementAtOrDefault( i ) ;

                    foreach ( var pillarH in pillarHSetSet.ElementAtOrDefault( i ) ) {
                      var signSet = new double[] { -1.0, 1.0 } ;
                      foreach ( var sign in signSet ) {
                        var dirH = dirHSet.ElementAtOrDefault( i ) ;
                        var dirK = dirKSet.ElementAtOrDefault( i ) ;
                        var startK = sign > 0 ? Math.Min( minK, maxK ) : Math.Max( minK, maxK ) ;
                        var endK = sign > 0 ? Math.Max( minK, maxK ) : Math.Min( minK, maxK ) ;
                        var start = origin + ( pillarH + sign * pillarHeightFt ) * dirH +
                                    ( startK + ( -sign ) * viewModel.HrzJointStartFt ) * dirK ;
                        var end = origin + ( pillarH + sign * pillarHeightFt ) * dirH +
                                  ( endK + ( sign ) * viewModel.HrzJointEndFt ) * dirK ;

                        //L材のときのみ start end を入れ替える必要あり
                        if ( viewModel.HrzJointSize is ClsAngleCsv ) {
                          var tmp = end ;
                          end = start ;
                          start = tmp ;
                        }

                        var id = MaterialSuper.PlaceWithTwoPoints( symbol, danRefPlane.GetReference(), end, start,
                          levelDiff ) ;
                        //var id = MaterialSuper.PlaceWithTwoPoints(symbol, danRefPlane.GetReference(), start, end, levelDiff);
                        if ( id == ElementId.InvalidElementId ) {
                          _logger.Error( "ツナギ材ファミリインスタンスの配置に失敗" ) ;
                          continue ;
                        }

                        ClsRevitUtil.SetParameter( doc, id, DefineUtil.PARAM_ROTATE, -0.5 * Math.PI ) ;
                        var material = new HorizontalJoint
                        {
                          m_KodaiName = kodaiName,
                          m_Document = doc,
                          m_ElementId = id,
                          m_Size = viewModel.HrzJointSize.Size,
                          m_TeiketsuType = ( viewModel.HrzJointIsBolt )
                            ? DefineUtil.eJoinType.Bolt
                            : ( ( viewModel.HrzJointIsMetal )
                              ? DefineUtil.eJoinType.Support
                              : DefineUtil.eJoinType.Welding )
                        } ;
                        MaterialSuper.WriteToElement( material, doc ) ;
                      }
                    }
                  }
                }
              }
            }
            else {
              _logger.Error( $"family:\"{familyName}\", type:\"{familyTypeName}\" cannot load" ) ;
            }
          }

          // 水平ブレス
          if ( viewModel.HrzBraceHasPlacing && viewModel.HrzBraceSize is ClsAngleCsv hrzBraceSizeRec ) {
            var familyPath = Directory.EnumerateFiles( PathUtil.GetYMSFolder(),
              Path.GetFileName( viewModel.HrzBraceSize.FamilyPath ), SearchOption.AllDirectories ).FirstOrDefault() ;
            var familyName = Path.GetFileNameWithoutExtension( familyPath ) ;
            var familyTypeName = "ﾌﾞﾚｰｽ" ;

            if ( GantryUtil.GetFamilySymbol( doc, familyPath, familyTypeName, out var symbol, true ) ) {
              var angleWidth = hrzBraceSizeRec.AFt ;

              var positivePlane = PositivePlane( doc, baseLevel ) ;
              var negativePlane = NegativePlane( doc, baseLevel ) ;

              var ohbikiSizeValue = kodaiData.ohbikiData.OhbikiSize ;
              if ( ohbikiSizeValue.Contains( "HA" ) || ohbikiSizeValue.Contains( "SMH" ) ) {
                var obikiSizeTrim = ohbikiSizeValue.Contains( "_" )
                  ? ohbikiSizeValue.Substring( 0, ohbikiSizeValue.IndexOf( '_' ) )
                  : ohbikiSizeValue ;
                ohbikiSizeValue = ClsKouzaiSpecify.GetKouzaiSize( obikiSizeTrim ) ;
              }

              var ohbikiSize = ClsMasterCsv.Shared.Select( x => x as ClsHBeamCsv ).Where( x => x != null )
                .FirstOrDefault( x => x.Size == ohbikiSizeValue.Replace( "-", "" ) ) ;

              var levelOffsetSet = new double[ 0 ] ;
              if ( viewModel.HrzBraceHasTopPlate &&
                   ( kodaiData.pilePillerData.HasTopPlate || kodaiData.pilePillerData.ExtensionPile ) ) {
                // トッププレート
                var plateSizeText = kodaiData.pilePillerData.ExtensionPile
                  ? kodaiData.pilePillerData.extensionPileData.topPlateData.PlateSize
                  : kodaiData.pilePillerData.topPlateData.PlateSize ;
                var topplateSize =
                  ClsMasterCsv.Shared.FirstOrDefault( x => x.Size == plateSizeText ) as ClsTopPlateCsv ;
                var plateThick = topplateSize?.T.ToFt() ?? 0.0 ;
                levelOffsetSet = new double[] { -highestOhbikiBottomFt, highestOhbikiBottomFt - plateThick } ;
              }
              else if ( viewModel.HrzBraceIsLowerOhbiki ) {
                // 大引下部
                var ohbikiFrangeThick = ohbikiSize?.T2Ft ?? 0.0 ;
                levelOffsetSet = new double[] { -highestOhbikiBottomFt - ohbikiFrangeThick, highestOhbikiBottomFt, } ;
              }
              else {
                // 大引の上下フランジ
                var ohbikiFrangeThick = ohbikiSize?.T2Ft ?? 0.0 ;
                //var ohbikiB = ohbikiSize?.BFt ?? 0.0;高さだからHでは？
                var ohbikiB = ohbikiSize?.HFt ?? 0.0 ;
                levelOffsetSet = new double[]
                {
                  -highestOhbikiBottomFt - ohbikiFrangeThick, highestOhbikiBottomFt + ohbikiB - ohbikiFrangeThick,
                } ;
              }

              for ( int indexK = 0 ; indexK < pillarKSet.Length - 1 ; indexK++ ) {
                var startK = pillarKSet[ indexK ] ;
                var endK = pillarKSet[ ( indexK + 1 ) % pillarKSet.Length ] ;
                var lengthK = Math.Abs( endK - startK ) ;
                var targetWidthK = lengthK - 2.0 * viewModel.HrzBraceXFt ;

                for ( int indexH = 0 ; indexH < pillarHSet.Length - 1 ; indexH++ ) {
                  var startH = pillarHSet[ indexH ] ;
                  var endH = pillarHSet[ ( indexH + 1 ) % pillarHSet.Length ] ;
                  var lengthH = Math.Abs( endH - startH ) ;
                  var targetWidthH = lengthH - 2.0 * viewModel.HrzBraceYFt ;

                  var a = targetWidthK ;
                  var b = targetWidthH ;
                  var c = angleWidth ;

                  var coefSet = new double[] { c * c, -2 * b * c, a * a + b * b, 0.0, -a * a, } ;

                  var k = 0 ;
                  var roots = UtilAlgebra.SolveQuarticEquation( coefSet[ k++ ], coefSet[ k++ ], coefSet[ k++ ],
                    coefSet[ k++ ], coefSet[ k++ ] ) ;

                  var cosTheta = roots.FirstOrDefault( x => UtilAlgebra.Eq( x.Imaginary, 0.0 ) && x.Real > 0 ).Real ;
                  var sinTheta = Math.Sqrt( 1.0 - cosTheta * cosTheta ) ;

                  var braceOrigin = origin + ( startH + viewModel.HrzBraceYFt ) * axisH +
                                    ( startK + viewModel.HrzBraceXFt ) * axisK ;

                  //var aaaa = braceOrigin + c * cosTheta * axisH + (a + 0.5 * sinTheta) * axisK;
                  var braceEndSet = new XYZ[]
                  {
                    braceOrigin + 0.5 * c * sinTheta * axisK, braceOrigin + b * axisH + 0.5 * c * sinTheta * axisK,
                  } ;
                  var braceStartSet = new XYZ[]
                  {
                    braceOrigin + ( a + 0.5 * c * sinTheta ) * axisK + ( b - c * cosTheta ) * axisH,
                    braceOrigin + c * cosTheta * axisH + ( a + 0.5 * c * sinTheta ) * axisK,
                  } ;

                  var referenceSet = new Reference[] { negativePlane.GetReference(), positivePlane.GetReference(), } ;

                  for ( int i = 0 ; i < 2 ; i++ ) {
                    var braceStart =
                      braceStartSet.ElementAtOrDefault( i ) ; // braceOrigin + 0.5 * c * sinTheta * axisK;
                    var braceEnd =
                      braceEndSet.ElementAtOrDefault(
                        i ) ; // braceOrigin + (a + 0.5 * c * sinTheta) * axisK + (b - c * cosTheta) * axisH;
                    var levelOffset = levelOffsetSet.ElementAtOrDefault( i ) ; // 
                    var reference = referenceSet.ElementAtOrDefault( i ) ; // baseLevel.GetPlaneReference();

                    var id = MaterialSuper.PlaceWithTwoPoints( symbol, reference, braceStart, braceEnd, levelOffset ) ;
                    if ( id == ElementId.InvalidElementId ) {
                      _logger.Error( "ツナギ材ファミリインスタンスの配置に失敗" ) ;
                      continue ;
                    }

                    var material = new HorizontalBrace
                    {
                      m_KodaiName = kodaiName,
                      m_Document = doc,
                      m_ElementId = id,
                      m_Size = viewModel.HrzBraceSize.Size,
                      m_TeiketsuType = ( viewModel.HrzBraceConnectingType == ConnectingType.Bolt )
                        ? DefineUtil.eJoinType.Bolt
                        : ( ( viewModel.HrzBraceConnectingType == ConnectingType.Metal )
                          ? DefineUtil.eJoinType.Support
                          : DefineUtil.eJoinType.Welding )
                    } ;
                    MaterialSuper.WriteToElement( material, doc ) ;
                  }
                }
              }
            }
            else {
              _logger.Error( $"family:\"{familyName}\", type:\"{familyTypeName}\" cannot load" ) ;
            }
          }

          // 垂直ブレス
          if ( ( viewModel.VrtBraceHasH || viewModel.VrtBraceHasK ) &&
               viewModel.VrtBraceSize is ClsAngleCsv vrtBraceSizeRec ) {
            var familyPath = Directory.EnumerateFiles( PathUtil.GetYMSFolder(),
              Path.GetFileName( viewModel.VrtBraceSize.FamilyPath ), SearchOption.AllDirectories ).FirstOrDefault() ;
            var familyName = Path.GetFileNameWithoutExtension( familyPath ) ;
            var familyTypeName = "ﾌﾞﾚｰｽ" ;

            if ( GantryUtil.GetFamilySymbol( doc, familyPath, familyTypeName, out var symbol, true ) ) {
              var insets = new List<BraceInsets>() ;

              var flags = new bool[] { viewModel.VrtBracePriorH, ! viewModel.VrtBracePriorH, } ;
              for ( int indexFlag = 0 ; indexFlag < flags.Length ; indexFlag++ ) {
                var preferH = flags.ElementAtOrDefault( indexFlag ) ;
                //var preferH = viewModel.VrtBracePriorH;
                var preferred = indexFlag == 0 ;

                var braceOffset = -0.5 * ( preferH ? pillarSizeB : pillarSizeH ) ;
                var pillarHalfWidth = 0.5 * ( preferH ? pillarSizeH : pillarSizeB ) ;

                // 勝ち側の杭座標
                var pillarsH = preferH ? pillarHSet : pillarKSet ;
                var pillarsK = preferH ? pillarKSet : pillarHSet ;
                var dirH = preferH ? axisH : axisK ;
                var dirK = preferH ? axisK : axisH ;

                // 
                for ( int indexH = 0 ; indexH < pillarsH.Length - 1 ; indexH++ ) {
                  var startH = pillarsH.ElementAtOrDefault( indexH ) ;
                  var endH = pillarsH.ElementAtOrDefault( indexH + 1 ) ;

                  var lengthH = Math.Abs( endH - startH ) ;

                  for ( int indexK = 0 ; indexK < pillarsK.Length ; indexK++ ) {
                    var pillarK = pillarsK.ElementAtOrDefault( indexK ) ;

                    var areaOriginKH = origin + startH * dirH + pillarK * dirK ;

                    var referenceSet = new Reference[]
                    {
                      doc.Create.NewReferencePlane( areaOriginKH, areaOriginKH + dirH, axisZ, doc.ActiveView )
                        ?.GetReference(),
                      doc.Create.NewReferencePlane( areaOriginKH, areaOriginKH - dirH, axisZ, doc.ActiveView )
                        ?.GetReference(),
                    } ;

                    for ( int indexDan = 0 ; indexDan < danData.Count() ; indexDan++ ) {
                      var dan = danData.ElementAtOrDefault( indexDan ) ;
                      if ( dan == null ) {
                        continue ;
                      }

                      var isFirst = indexDan == 0 ;
                      var isLast = false ; // indexDan == danData.Count() - 1;

                      var upperX = isFirst ? viewModel.VrtBrace1UpperXFt : viewModel.VrtBrace2UpperXFt ;
                      var upperY = isFirst ? viewModel.VrtBrace1UpperYFt : viewModel.VrtBrace2UpperYFt ;
                      var lowerX = isFirst ? viewModel.VrtBrace1LowerXFt : viewModel.VrtBrace2LowerXFt ;
                      var lowerY = isFirst ? viewModel.VrtBrace1LowerYFt : viewModel.VrtBrace2LowerYFt ;

                      if ( ! preferred ) {
                        var targetInsets = insets.Where( x => x.K == indexH && x.H == indexK && x.D == indexDan )
                          .ToArray() ;
                        if ( targetInsets.Any() ) {
                          upperY += targetInsets.Select( x => x.Upper ).Max() ;
                          lowerY += targetInsets.Select( x => x.Lower ).Max() ;
                        }
                      }

                      var elevation = baseLevel.ProjectElevation + highestOhbikiBottomFt -
                                      danTopSet.ElementAtOrDefault( indexDan ) ;

                      var areaOrigin = new XYZ( areaOriginKH.X, areaOriginKH.Y, elevation ) ;

                      var lengthZ = dan.IntervalFt - ( isFirst || isLast ? 1.0 : 2.0 ) * jointSizeH ;

                      var a = lengthH + 2.0 * upperX ;
                      var b = lengthH + 2.0 * lowerX ;
                      var d = vrtBraceSizeRec.AFt ;
                      var aplusb = a + b ;

                      var c = lengthZ - upperY - lowerY ;

                      var a2 = 0.25 * aplusb * aplusb + c * c ;
                      var a1 = aplusb * d ;
                      var a0 = d * d - c * c ;

                      var roots = UtilAlgebra.SolveQuadraticEquation( a2, a1, a0 ) ;
                      var sinTheta = roots.FirstOrDefault( x => UtilAlgebra.IsReal( x ) && x.Real > 0.0 ).Real ;
                      var cosTheta = Math.Sqrt( 1 - sinTheta * sinTheta ) ;
                      var tanTheta = sinTheta / cosTheta ;

                      var upperInset = ( upperX + pillarHalfWidth ) * tanTheta + d / cosTheta ;
                      var lowerInset = ( lowerX + pillarHalfWidth ) * tanTheta + d / cosTheta ;
                      insets.Add( new BraceInsets
                      {
                        H = indexH,
                        K = indexK,
                        D = indexDan,
                        Upper = upperInset,
                        Lower = lowerInset,
                      } ) ;
                      insets.Add( new BraceInsets
                      {
                        H = indexH + 1,
                        K = indexK,
                        D = indexDan,
                        Upper = upperInset,
                        Lower = lowerInset,
                      } ) ;

                      //var aa = areaOrigin + (upperX + a + 0.5 * d * sinTheta) * dirH - (upperY + 0.5 * d * cosTheta) * axisZ;
                      var braceOrigin = areaOrigin - ( isFirst ? 0.0 : jointSizeH ) * axisZ ;
                      var braceStartSet = new XYZ[]
                      {
                        braceOrigin + ( -upperX + a ) * dirH - ( upperY ) * axisZ,
                        braceOrigin + ( -upperX ) * dirH - ( upperY ) * axisZ,
                      } ;
                      var braceEndSet = new XYZ[]
                      {
                        braceOrigin + ( -lowerX - d * sinTheta ) * dirH - ( upperY + c - d * cosTheta ) * axisZ,
                        braceOrigin + ( -lowerX + b + d * sinTheta ) * dirH - ( upperY + c - d * cosTheta ) * axisZ,
                      } ;

                      if ( viewModel.DanRowSet.FirstOrDefault( x => x.Index == indexDan + 1 )?.HasBrace != true ) {
                        continue ;
                      }

                      // 最下段に配置するか確認
                      //if (isLast && !viewModel.VrtBraceHasLowest) { continue; }

                      if ( ! viewModel.VrtBraceHasH && preferH ) {
                        continue ;
                      }

                      if ( ! viewModel.VrtBraceHasK && ! preferH ) {
                        continue ;
                      }

                      for ( int k = 0 ; k < 2 ; k++ ) {
                        var braceStart = braceStartSet.ElementAtOrDefault( k ) ;
                        var braceEnd = braceEndSet.ElementAtOrDefault( k ) ;

                        var reference = referenceSet.ElementAtOrDefault( k ) ;

                        XYZ start, end ;
                        if ( viewModel.VrtBraceHasRound ) {
                          var originalLength = braceStart.DistanceTo( braceEnd ) ;
                          var roundedLength = ( Math.Floor( originalLength / viewModel.VrtBraceRoundFt ) + 1.0 ) *
                                              viewModel.VrtBraceRoundFt ;

                          var diff = 0.5 * ( roundedLength - originalLength ) ;
                          var startToEnd = ( braceEnd - braceStart ).Normalize() ;

                          start = braceStart - diff * startToEnd ;
                          end = braceEnd + diff * startToEnd ;
                        }
                        else {
                          start = braceStart ;
                          end = braceEnd ;
                        }

                        var id = MaterialSuper.PlaceWithTwoPoints( symbol, reference, start, end, braceOffset ) ;
                        if ( id == ElementId.InvalidElementId ) {
                          _logger.Error( "ツナギ材ファミリインスタンスの配置に失敗" ) ;
                          continue ;
                        }

                        var material = new VerticalBrace
                        {
                          m_KodaiName = kodaiName,
                          m_Document = doc,
                          m_ElementId = id,
                          m_Size = viewModel.HrzBraceSize.Size,
                          m_TeiketsuType = ( viewModel.VrtBraceConnectingType == ConnectingType.Bolt )
                            ? DefineUtil.eJoinType.Bolt
                            : ( ( viewModel.VrtBraceConnectingType == ConnectingType.Metal )
                              ? DefineUtil.eJoinType.Support
                              : DefineUtil.eJoinType.Welding )
                        } ;
                        MaterialSuper.WriteToElement( material, doc ) ;
                      }
                    }
                  }
                }
              }
            }
          }

          // 構台情報を取得
          var koudaiData = modelData.ListKoudaiData.FirstOrDefault( x =>
            x.AllKoudaiFlatData.KoudaiName == viewModel.SelectedKodai.KoudaiName ) ;

          if ( koudaiData != null ) {
            // koudaiData.BraceTsunagiData 内の各種パラメータに画面の値を設定する
            koudaiData.BraceTsunagiData = new BraceTsunagiData() ;
            //段数設定
            koudaiData.BraceTsunagiData.DanSetting = new Data.DanSetting() ;

            koudaiData.BraceTsunagiData.DanSetting.TsunagiDansu = viewModel.DanCount ;
            koudaiData.BraceTsunagiData.DanSetting.BaseSpan = viewModel.DanBaseInterval ;
            foreach ( DansuRow row in viewModel.DanRowSet ) {
              DanSettingListData danSettingListData = new DanSettingListData() ;
              danSettingListData.Dan = row.Index.ToString() ;
              danSettingListData.Span = row.IntervalMm ;
              danSettingListData.TsunagiUmu = row.HasJoint ;
              danSettingListData.BraceUmu = row.HasBrace ;
              koudaiData.BraceTsunagiData.DanSetting.DanSettingDataList.Add( danSettingListData ) ;
            }

            //垂直ブレース
            koudaiData.BraceTsunagiData.VerticalBrace = new Data.VerticalBrace() ;

            koudaiData.BraceTsunagiData.VerticalBrace.KyoujikuUmu = viewModel.VrtBraceHasK ;
            koudaiData.BraceTsunagiData.VerticalBrace.FukuinUmu = viewModel.VrtBraceHasH ;
            koudaiData.BraceTsunagiData.VerticalBrace.Size = viewModel.VrtBraceSize.Size ;
            koudaiData.BraceTsunagiData.VerticalBrace.KachiFukuin = viewModel.VrtBracePriorH ;
            koudaiData.BraceTsunagiData.VerticalBrace.KachiKyoujiku = viewModel.VrtBracePriorK ;

            koudaiData.BraceTsunagiData.VerticalBrace.FirstHanareTopX = viewModel.VrtBrace1UpperXMm ;
            koudaiData.BraceTsunagiData.VerticalBrace.FirstHanareTopY = viewModel.VrtBrace1UpperYMm ;
            koudaiData.BraceTsunagiData.VerticalBrace.FirstHanareBottomX = viewModel.VrtBrace1LowerXMm ;
            koudaiData.BraceTsunagiData.VerticalBrace.FirstHanareBottomY = viewModel.VrtBrace1LowerYMm ;

            koudaiData.BraceTsunagiData.VerticalBrace.SecondLaterHanareTopX = viewModel.VrtBrace2UpperXMm ;
            koudaiData.BraceTsunagiData.VerticalBrace.SecondLaterHanareTopY = viewModel.VrtBrace2UpperYMm ;
            koudaiData.BraceTsunagiData.VerticalBrace.SecondLaterHanareBottomX = viewModel.VrtBrace2LowerXMm ;
            koudaiData.BraceTsunagiData.VerticalBrace.SecondLaterHanareBottomY = viewModel.VrtBrace2LowerYMm ;

            koudaiData.BraceTsunagiData.VerticalBrace.RoundON = viewModel.VrtBraceHasRound ;
            koudaiData.BraceTsunagiData.VerticalBrace.RoundLength = viewModel.VrtBraceRoundMm.ToString() ;

            koudaiData.BraceTsunagiData.VerticalBrace.ToritsukiYousetsu = viewModel.VrtBraceIsWelding ;
            koudaiData.BraceTsunagiData.VerticalBrace.ToritsukiBolt = viewModel.VrtBraceIsBolt ;
            koudaiData.BraceTsunagiData.VerticalBrace.ToritsukiTeiketsuKanagu = viewModel.VrtBraceIsMetal ;

            //水平ブレース
            koudaiData.BraceTsunagiData.HorizontalBrace = new Data.HorizontalBrace() ;

            koudaiData.BraceTsunagiData.HorizontalBrace.HaichiOn = viewModel.HrzBraceHasPlacing ;
            koudaiData.BraceTsunagiData.HorizontalBrace.HaichiOFF = viewModel.HrzBraceHasNotPlacing ;
            koudaiData.BraceTsunagiData.HorizontalBrace.Size = viewModel.HrzBraceSize.Size ;

            koudaiData.BraceTsunagiData.HorizontalBrace.PositionTopFlaBottomAndBottomFlaTop =
              viewModel.HrzBraceIsOhbikiInner ;
            koudaiData.BraceTsunagiData.HorizontalBrace.PositionBottomFlaTopAndBottomFlaBottom =
              viewModel.HrzBraceIsLowerOhbiki ;
            koudaiData.BraceTsunagiData.HorizontalBrace.PutTopPL = viewModel.HrzBraceHasTopPlate ;

            koudaiData.BraceTsunagiData.HorizontalBrace.PutBanRangeX = viewModel.HrzBraceXMm ;
            koudaiData.BraceTsunagiData.HorizontalBrace.PutBanRangeY = viewModel.HrzBraceYMm ;

            koudaiData.BraceTsunagiData.HorizontalBrace.ToritsukiYousetsu = viewModel.HrzBraceIsWelding ;
            koudaiData.BraceTsunagiData.HorizontalBrace.ToritsukiBolt = viewModel.HrzBraceIsBolt ;
            koudaiData.BraceTsunagiData.HorizontalBrace.ToritsukiTeiketsuKanagu = viewModel.HrzBraceIsMetal ;

            //水平ツナギ
            koudaiData.BraceTsunagiData.HorizontalTsunagi = new Data.HorizontalTsunagi() ;

            koudaiData.BraceTsunagiData.HorizontalTsunagi.KyoujikuUmu = viewModel.HrzJointHasK ;
            koudaiData.BraceTsunagiData.HorizontalTsunagi.FukuinUmu = viewModel.HrzJointHasH ;
            koudaiData.BraceTsunagiData.HorizontalTsunagi.Size = viewModel.HrzJointSize.Size ;

            koudaiData.BraceTsunagiData.HorizontalTsunagi.PositionFukuinTopAndKyoujikuBottom =
              viewModel.HrzJointUpperIsH ;
            koudaiData.BraceTsunagiData.HorizontalTsunagi.PositionFukuinBottomAndKyoujikuTop =
              viewModel.HrzJointLowerIsH ;

            koudaiData.BraceTsunagiData.HorizontalTsunagi.TsukidashiStart = viewModel.HrzJointStartMm ;
            koudaiData.BraceTsunagiData.HorizontalTsunagi.TsukidashiEnd = viewModel.HrzJointEndMm ;

            koudaiData.BraceTsunagiData.HorizontalTsunagi.ToritsukiYousetsu = viewModel.HrzJointIsWelding ;
            koudaiData.BraceTsunagiData.HorizontalTsunagi.ToritsukiBolt = viewModel.HrzJointIsBolt ;
            koudaiData.BraceTsunagiData.HorizontalTsunagi.ToritsukiTeiketsuKanagu = viewModel.HrzJointIsMetal ;

            // 構台情報を更新
            modelData.UpdateKoudaiData( koudaiData ) ;
            // モデルデータを更新
            ModelData.SetModelData( viewModel.RevitDoc, modelData ) ;
          }

          transaction.Commit() ;
          return true ;
        }
        #if DEBUG
        #else
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                    transaction.RollBack();
                    return false;
                }
        #endif
      }
    }

    class BraceInsets
    {
      public int K { get ; set ; }
      public int H { get ; set ; }
      public int D { get ; set ; }
      public double Upper { get ; set ; }
      public double Lower { get ; set ; }
    }

    static ReferencePlane NegativePlane( Document doc, Level baseLevel )
    {
      var elevation = baseLevel.ProjectElevation ;
      var origin = XYZ.Zero + elevation * XYZ.BasisZ ;
      return doc.Create.NewReferencePlane( origin, origin + XYZ.BasisY, -XYZ.BasisX, doc.ActiveView ) ;
    }

    static ReferencePlane PositivePlane( Document doc, Level baseLevel )
    {
      var elevation = baseLevel.ProjectElevation ;
      var origin = XYZ.Zero + elevation * XYZ.BasisZ ;
      return doc.Create.NewReferencePlane( origin, origin + XYZ.BasisY, XYZ.BasisX, doc.ActiveView ) ;
    }

    /// <summary>
    /// 一括削除
    /// </summary>
    /// <param name="doc"></param>
    public static void DeleteAll( FrmCreateBraceAndJointViewModel viewModel )
    {
      var kodaiData = viewModel.SelectedKodai ;

      if ( kodaiData == null ) {
        MessageUtil.Warning( "構台が選択されていません。", "一括削除" ) ;
        return ;
      }

      if ( MessageUtil.YesNo( $"構台「{kodaiData.KoudaiName}」に該当するブレース・ツナギ材を全て削除します。\r\n続行しますか?", "一括削除" ) !=
           DialogResult.Yes ) {
        return ;
      }

      var doc = viewModel.RevitDoc ;

      using ( var transaction = new Transaction( doc, Guid.NewGuid().ToString() ) ) {
        transaction.Start() ;
        var kodaiName = kodaiData.KoudaiName ;

        var targetInstances = MaterialSuper.Collect( doc ).Where( x => x is JointAndBrace )
          .Where( x => x.m_KodaiName == kodaiName ) ;

        var targetReferences = targetInstances.Select( x => x.m_Document.GetElement( x.m_ElementId ) as FamilyInstance )
          .Select( x => x?.Host?.Id ).Where( x => x != null ).Distinct() ;

        foreach ( var reference in targetReferences ) {
          var deleteInstances = new FilteredElementCollector( doc ).OfClass( typeof( FamilyInstance ) )
            .Select( x => x as FamilyInstance ).Where( x => x != null ).Where( x => x.Host?.Id == reference )
            .Select( x => x?.Id ).Where( x => x != null ).ToList() ;

          RevitUtil.ClsRevitUtil.Delete( doc, deleteInstances ) ;
          RevitUtil.ClsRevitUtil.Delete( doc, reference ) ;
        }

        transaction.Commit() ;
      }
    }
  }
}