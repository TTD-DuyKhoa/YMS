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
using static YMS_gantry.DefineUtil ;

namespace YMS_gantry
{
  class CmdCreateKoudaiFlat
  {
    static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger() ;
    Document _doc { get ; set ; }
    UIDocument _uiDoc { get ; set ; }

    public CmdCreateKoudaiFlat( UIDocument uiDoc )
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
      bool retval = true ;
      try {
        using ( var f = new FrmAllPutKoudaiFlat( _doc ) ) {
          //OK以外は続行しない
          if ( f.ShowDialog() != System.Windows.Forms.DialogResult.OK ) {
            return true ;
          }

          //強制的にビュー切り替え
          string viewName = f.data.SelectedLevel ;
          Autodesk.Revit.DB.View selView =
            RevitUtil.ClsRevitUtil.getView( _doc, viewName, GantryUtil.getViewType( _doc, viewName ) ) ;
          if ( selView != null ) {
            _uiDoc.ActiveView = selView ;
          }

          Selection selection = _uiDoc.Selection ;
          //基準レベル
          Level baselevel = RevitUtil.ClsRevitUtil.GetLevel( _doc, f.data.SelectedLevel ) as Level ;
          eBaseLevel eBase = f.data.BaseLevel ;

          XYZ origin, hukuinPnt, vecHukuin, kyoutyouPnt, orthogolanVec ;
          ElementId pointId = ElementId.InvalidElementId, LineId = ElementId.InvalidElementId ;
          using ( Transaction tr = new Transaction( _doc ) ) {
            tr.Start( "Create Koudai" ) ;
            try {
              //配置基準＋幅員、橋長方向取得
              origin = selection.PickPoint( ObjectSnapTypes.Intersections, "構台の基点を指定" ) ;
              pointId = GantryUtil.InsertPointFamily( _doc, origin, baselevel, true ) ;
              _doc.Regenerate() ;

              //幅員方向ベクトル
              hukuinPnt = selection.PickPoint( ObjectSnapTypes.Nearest, "幅員方向を指定" ) ;
              vecHukuin = ( hukuinPnt - origin ).Normalize() ;
              LineId = GantryUtil.InsertLineFamily( _doc, origin, hukuinPnt, baselevel, true ) ;
              _doc.Regenerate() ;

              //橋軸方向ベクトル
              kyoutyouPnt = selection.PickPoint( ObjectSnapTypes.Nearest, "橋長方向を指定" ) ;
              orthogolanVec = vecHukuin.CrossProduct( XYZ.BasisZ ) ;

              if ( pointId != null && pointId != ElementId.InvalidElementId ) {
                FamilyInstance ins = _doc.GetElement( pointId ) as FamilyInstance ;
                if ( ins != null ) {
                  _doc.Delete( pointId ) ;
                }
              }

              if ( LineId != null && LineId != ElementId.InvalidElementId ) {
                FamilyInstance ins = _doc.GetElement( LineId ) as FamilyInstance ;
                if ( ins != null ) {
                  _doc.Delete( LineId ) ;
                }
              }
            }
            catch ( Autodesk.Revit.Exceptions.OperationCanceledException ) {
              if ( pointId != null && pointId != ElementId.InvalidElementId ) {
                FamilyInstance ins = _doc.GetElement( pointId ) as FamilyInstance ;
                if ( ins != null ) {
                  _doc.Delete( pointId ) ;
                }
              }

              if ( LineId != null && LineId != ElementId.InvalidElementId ) {
                FamilyInstance ins = _doc.GetElement( LineId ) as FamilyInstance ;
                if ( ins != null ) {
                  _doc.Delete( LineId ) ;
                }
              }

              return false ;
            }
            catch ( Exception ex ) {
              if ( pointId != null && pointId != ElementId.InvalidElementId ) {
                FamilyInstance ins = _doc.GetElement( pointId ) as FamilyInstance ;
                if ( ins != null ) {
                  _doc.Delete( pointId ) ;
                }
              }

              if ( LineId != null && LineId != ElementId.InvalidElementId ) {
                FamilyInstance ins = _doc.GetElement( LineId ) as FamilyInstance ;
                if ( ins != null ) {
                  _doc.Delete( LineId ) ;
                }
              }

              return false ;
            }

            XYZ vecKyoutyou = ( kyoutyouPnt - origin ).Normalize() ;
            if ( RevitUtil.ClsGeo.GEO_GE0( orthogolanVec.DotProduct( vecKyoutyou ) ) ) {
              vecKyoutyou = orthogolanVec.Normalize() ;
            }
            else {
              vecKyoutyou = orthogolanVec.Negate().Normalize() ;
            }

            bool isLeft = RevitUtil.ClsGeo.IsLeft( vecHukuin, vecKyoutyou ) ;

            //橋長方向を正面にした際に必ず左端が基点になるよう調整
            if ( ! isLeft ) {
              origin = origin + vecHukuin * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( f.data.HukuinLength ) ;
              vecHukuin = vecHukuin.Negate() ;
            }

            double adjustAngle =
              XYZ.BasisX.AngleTo( vecHukuin ) * ( RevitUtil.ClsGeo.GEO_LT0( vecHukuin.Y ) ? -1 : 1 ) ;
            //基点を回転
            pointId = GantryUtil.InsertReferenceFamily( _doc, origin, baselevel, true ) ;
            //adjustAngle = Math.PI/2;
            _doc.Regenerate() ;
            RevitUtil.ClsRevitUtil.RotateElement( _doc, pointId, Line.CreateBound( origin, origin + XYZ.BasisZ ),
              adjustAngle ) ;
            RevitUtil.ClsRevitUtil.SetParameter( _doc, pointId, DefineUtil.PARAM_HOST_OFFSET,
              RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( f.data.LevelOffset ) ) ;
            KoudaiReference refe = new KoudaiReference( _doc, f.data.KoudaiName, pointId ) ;
            KoudaiReference.WriteToElement( refe, _doc ) ;

            XYZ adjustVec = XYZ.Zero ;

            //各部材高さ
            MaterialSize ohbikiSize = GantryUtil.GetKouzaiSize( f.data.ohbikiData.OhbikiSize ) ;
            MaterialSize nedaSize = GantryUtil.GetKouzaiSize( f.data.nedaData.NedaSize ) ;
            MaterialSize shikiSize = GantryUtil.GetKouzaiSize( f.data.shikigetaData.ShikigetaSize ) ;

            bool isDoboku = f.data.KoujiType == DefineUtil.eKoujiType.Doboku ;

            //覆工板配置
            if ( f.data.HasFukkouban ) {
              //配置位置算出
              Dictionary<XYZ, string> FukkoubanPnts = Fukkouban.CalcArrangementPoint( f.data.KyoutyouLength,
                f.data.HukuinLength, f.data.FukkoubanPitch, origin, vecKyoutyou, vecHukuin ) ;
              //配置
              List<ElementId> FukkoubanIds = new List<ElementId>() ;
              int FukkourC = 0 ;
              for ( int FukkouC = 0 ; FukkouC < FukkoubanPnts.Count ; FukkouC++ ) {
                FukkourC = ( FukkourC > f.data.FukkoubanPitch.Count - 1 ) ? 0 : FukkourC ;
                XYZ position = FukkoubanPnts.Keys.ElementAt( FukkouC ) ;
                eFukkoubanType hType = ( RevitUtil.ClsGeo.GEO_EQ( f.data.FukkoubanPitch[ FukkourC ], 2000 ) )
                  ? eFukkoubanType.TwoM
                  : eFukkoubanType.Three ;
                ElementId hId = Fukkouban.CreateFukkouban( _doc, f.data.FukkoubanData.FukkouMaterial, position, hType,
                  f.data.FukkoubanSizeList[ FukkourC ], adjustAngle, adjustVec, baselevel,
                  Fukkouban.CalcLevelOffsetLevel( baselevel, eBase, f.data.LevelOffset, ohbikiSize.Height,
                    nedaSize.Height, f.data.ohbikiData.OhbikiDan, f.data.ohbikiData.isHkou ) ) ;

                string[] pitchs = FukkoubanPnts.Values.ElementAt( FukkouC ).Split( ',' ) ;
                Fukkouban fukkouban = new Fukkouban( hId, f.data.KoudaiName, f.data.FukkoubanSizeList[ FukkourC ],
                  f.data.FukkoubanData, RevitUtil.ClsCommonUtils.ChangeStrToInt( pitchs[ 0 ] ),
                  RevitUtil.ClsCommonUtils.ChangeStrToInt( pitchs[ 1 ] ) ) ;
                Fukkouban.WriteToElement( fukkouban, _doc ) ;
                FukkoubanIds.Add( hId ) ;
                FukkourC++ ;
              }
            }

            //根太配置
            List<XYZ> NedaPnts = Neda.CalcArrangementPoint( f.data.KyoutyouLength, f.data.HukuinLength,
              f.data.NedaPitch, origin, vecKyoutyou, vecHukuin ) ;
            List<ElementId> nedaIds = new List<ElementId>() ;
            adjustAngle = XYZ.BasisX.AngleTo( vecKyoutyou ) * ( RevitUtil.ClsGeo.GEO_LT0( vecKyoutyou.Y ) ? -1 : 1 ) ;
            FamilySymbol sym ;
            string familyPath = Master.ClsMasterCsv.GetFamilyPath( f.data.nedaData.NedaSize ) ;
            string type = isDoboku ? "主桁" : Neda.typeName ;
            if ( GantryUtil.GetFamilySymbol( _doc, familyPath, type, out sym, true ) ) {
              sym = GantryUtil.DuplicateTypeWithNameRule( _doc, f.data.KoudaiName, sym, type, f.data.KoudaiType ) ;
              foreach ( XYZ position in NedaPnts ) {
                XYZ start = position ;
                XYZ end = position + vecKyoutyou *
                  ( RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( f.data.KyoutyouLength ) ) ;
                ElementId nId = Neda.CreateNeda( _doc, f.data.nedaData, start, end, baselevel, isDoboku,
                  f.data.KoudaiName, adjustAngle,
                  Neda.CalcLevelOffsetLevel( baselevel, eBase, f.data.HasFukkouban, f.data.LevelOffset,
                    ohbikiSize.Height, nedaSize.Height, f.data.ohbikiData.OhbikiDan, f.data.ohbikiData.isHkou ), sym ) ;
                //個別情報追加
                Neda neda = new Neda( nId, f.data.KoudaiName, f.data.nedaData, 0, f.data.KyoutyouPillarPitch.Count - 1,
                  NedaPnts.IndexOf( position ) ) ;
                Neda.WriteToElement( neda, _doc ) ;
                nedaIds.Add( nId ) ;
              }
            }

            //大引配置
            Dictionary<XYZ, string> OhbikiPnts = Ohbiki.CalcArrangementPoint( f.data, origin, vecKyoutyou ) ;
            List<ElementId> ohbikiIds = new List<ElementId>() ;
            adjustAngle = XYZ.BasisX.AngleTo( vecHukuin ) * ( RevitUtil.ClsGeo.GEO_LT0( vecHukuin.Y ) ? -1 : 1 ) ;
            familyPath = Master.ClsMasterCsv.GetFamilyPath( f.data.ohbikiData.OhbikiSize ) ;
            type = isDoboku ? "桁受" : Ohbiki.typeName ;
            if ( GantryUtil.GetFamilySymbol( _doc, familyPath, type, out sym, true ) ) {
              sym = GantryUtil.DuplicateTypeWithNameRule( _doc, f.data.KoudaiName, sym, type, f.data.KoudaiType ) ;
              for ( int OhbikiC = 0 ; OhbikiC < OhbikiPnts.Count ; OhbikiC++ ) {
                XYZ position = OhbikiPnts.ElementAt( OhbikiC ).Key ;
                List<ElementId> oId = new List<ElementId>() ;
                string indSt = OhbikiPnts.ElementAt( OhbikiC ).Value ;
                if ( indSt.Contains( "-" ) ) {
                  indSt = indSt.Replace( "-", "" ).Replace( "F", "" ).Replace( "B", "" ) ;
                }

                int ind = RevitUtil.ClsCommonUtils.ChangeStrToInt( indSt ) ;

                XYZ start = position ;
                XYZ end = position +
                          vecHukuin * ( RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( f.data.HukuinLength ) ) ;
                double rotate = ohbikiSize.Shape == MaterialShape.C ? 270 : 180 ;
                bool rev = false ;
                if ( OhbikiPnts.ElementAt( OhbikiC ).Value.Contains( "F" ) ) {
                  end = position ;
                  start = position +
                          vecHukuin * ( RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( f.data.HukuinLength ) ) ;
                  rev = true ;
                }

                oId.AddRange( Ohbiki.CreateOhbiki( _doc, f.data.ohbikiData, start, end, baselevel, isDoboku,
                  f.data.KoudaiName,
                  OhbikiSuper.CalcLevelOffsetLevel( baselevel, eBase, f.data.LevelOffset, f.data.HasFukkouban,
                    ohbikiSize.Height, nedaSize.Height, f.data.ohbikiData.OhbikiDan, f.data.ohbikiData.isHkou, rotate ),
                  rotate, sym, rev ) ) ;

                //個別情報追加
                foreach ( ElementId id in oId ) {
                  Ohbiki ohbiki = new Ohbiki( id, f.data.KoudaiName, f.data.ohbikiData, ind ) ;
                  if ( ! f.data.ohbikiData.isHkou ) {
                    ohbiki.m_TeiketsuType = f.data.ohbikiData.OhbikiAttachType ;
                    if ( ohbiki.m_TeiketsuType == eJoinType.Bolt ) {
                      ohbiki.m_BoltInfo1 = ( eBoltType.Normal == f.data.ohbikiData.OhbikiBoltType ) ? "通常" : "ハイテンション" ;
                      ohbiki.m_Bolt1Cnt = f.data.ohbikiData.OhbikiBoltCount ;
                    }

                    ohbiki.m_AttachSide = ( OhbikiPnts.ElementAt( OhbikiC ).Value.Contains( "F" ) ) ? "F" : "B" ;
                  }

                  Ohbiki.WriteToElement( ohbiki, _doc ) ;
                  ohbikiIds.Add( id ) ;
                }
              }
            }

            //敷桁配置
            Dictionary<XYZ, string> shikiGetaPnts = Shikigeta.CalcArrangementPoint( f.data, origin, vecKyoutyou ) ;
            List<ElementId> shikiIds = new List<ElementId>() ;
            familyPath = Master.ClsMasterCsv.GetFamilyPath( f.data.shikigetaData.ShikigetaSize ) ;
            if ( GantryUtil.GetFamilySymbol( _doc, familyPath, OhbikiSuper.shikiGetaName, out sym, true ) ) {
              sym = GantryUtil.DuplicateTypeWithNameRule( _doc, f.data.KoudaiName, sym, OhbikiSuper.shikiGetaName,
                f.data.KoudaiType ) ;
              for ( int shikiC = 0 ; shikiC < shikiGetaPnts.Count ; shikiC++ ) {
                XYZ position = shikiGetaPnts.ElementAt( shikiC ).Key ;
                List<ElementId> sId = new List<ElementId>() ;
                XYZ start = position ;
                XYZ end = position +
                          vecHukuin * ( RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( f.data.HukuinLength ) ) ;
                double rotate = 0 ;
                double off = OhbikiSuper.CalcLevelOffsetLevel( baselevel, eBase, f.data.LevelOffset,
                  f.data.HasFukkouban, shikiSize.Height, nedaSize.Height, 1, true, rotate ) ;
                off = ( f.data.ohbikiData.isHkou && ohbikiSize.Height != shikiSize.Height &&
                        f.data.BaseLevel == eBaseLevel.OhbikiBtm )
                  ? off + ( ohbikiSize.Height - shikiSize.Height )
                  : off ;
                sId.AddRange( Shikigeta.CreateShikiGeta( _doc, f.data.ohbikiData, f.data.shikigetaData, start, end,
                  baselevel, f.data.KoudaiName, rotate, off, sym ) ) ;

                string indSt = shikiGetaPnts.ElementAt( shikiC ).Value.Contains( "-" )
                  ? shikiGetaPnts.ElementAt( shikiC ).Value.Substring( 0, 2 )
                  : shikiGetaPnts.ElementAt( shikiC ).Value ;
                int ind = RevitUtil.ClsCommonUtils.ChangeStrToInt( indSt ) ;

                //個別情報追加
                foreach ( ElementId id in sId ) {
                  Shikigeta shikigeta = new Shikigeta( id, f.data.KoudaiName, f.data.shikigetaData, ind ) ;
                  Ohbiki.WriteToElement( shikigeta, _doc ) ;
                  ohbikiIds.Add( id ) ;
                }
              }
            }


            //杭、柱配置（継足し杭＋PL等）
            //柱
            Dictionary<XYZ, string> pPnts = PilePillerSuper.CalcArrangementPoint( f.data.KyoutyouLength,
              f.data.HukuinLength, f.data.beginNedaDiff, f.data.HukuinPillarPitch, f.data.KyoutyouPillarPitch, origin,
              vecKyoutyou, vecHukuin ) ;
            if ( f.data.HasPilePiller ) {
              List<ElementId> pIds = new List<ElementId>() ;
              adjustAngle = XYZ.BasisX.AngleTo( vecKyoutyou ) * ( RevitUtil.ClsGeo.GEO_LT0( vecKyoutyou.Y ) ? -1 : 1 ) ;
              //土木は90度回転
              if ( f.data.KoujiType == eKoujiType.Doboku ) {
                adjustAngle += Math.PI / 2 ;
              }

              f.data.pilePillerData.PileCutLength = PilePillerSuper.CalcPileCutLength( _doc, f.data ) ;
              familyPath = Master.ClsHBeamCsv.GetPileFamilyPath( f.data.pilePillerData.PillarSize ) ;
              if ( GantryUtil.GetFamilySymbol( _doc, familyPath, PilePillerSuper.typeName, out sym, true ) ) {
                sym = GantryUtil.DuplicateTypeWithNameRule( _doc, f.data.KoudaiName, sym, PilePillerSuper.typeName,
                  f.data.KoudaiType ) ;
              }

              FamilySymbol exSym = null ;
              familyPath = Master.ClsHBeamCsv.GetExPileFamilyPath( f.data.pilePillerData.PillarSize ) ;
              if ( GantryUtil.GetFamilySymbol( _doc, familyPath, PilePillerSuper.exPileTypeName, out exSym, true ) ) {
                exSym = GantryUtil.DuplicateTypeWithNameRule( _doc, f.data.KoudaiName, exSym,
                  PilePillerSuper.exPileTypeName, f.data.KoudaiType ) ;
              }

              if ( sym != null ) {
                double pOffset =
                  PilePillerSuper.CalcLevelOffsetLevel( baselevel, f.data, ohbikiSize.Height, nedaSize.Height ) ;
                double exPOffset = PilePillerSuper.CalcLevelOffsetExtensionPileLevel( baselevel, eBase,
                  f.data.LevelOffset, ohbikiSize.Height, nedaSize.Height ) ;

                foreach ( KeyValuePair<XYZ, string> kv in pPnts ) {
                  string[] pitchs = kv.Value.Split( ',' ) ;
                  //敷桁配置位置には柱を配置しない
                  if ( ( f.data.IsFirstShikigeta && pitchs[ 0 ] == "0" ) || ( f.data.IsLastShikigeta &&
                                                                              pitchs[ 0 ] ==
                                                                              $"{f.data.KyoutyouPillarPitch.Count - 1}" ) ) {
                    continue ;
                  }

                  ElementId pId = PilePillerSuper.CreatePile( _doc, kv.Key, adjustAngle, f.data.pilePillerData,
                    f.data.KoudaiName, adjustVec, baselevel, pOffset, sym ) ;
                  PilePiller pile = new PilePiller( pId, f.data.KoudaiName, f.data.pilePillerData,
                    RevitUtil.ClsCommonUtils.ChangeStrToInt( pitchs[ 0 ] ),
                    RevitUtil.ClsCommonUtils.ChangeStrToInt( pitchs[ 1 ] ) ) ;
                  if ( f.data.pilePillerData.pJointCount > 0 &&
                       f.data.pilePillerData.jointDetailData.JointType == eJoinType.Bolt ) {
                    pile.m_BoltInfo1 = f.data.pilePillerData.jointDetailData.WebBolt.BoltSize ;
                    pile.m_Bolt1Cnt = f.data.pilePillerData.jointDetailData.WebBolt.BoltCount *
                                      f.data.pilePillerData.pJointCount ;
                    pile.m_BoltInfo2 = f.data.pilePillerData.jointDetailData.FlangeBolt.BoltSize ;
                    pile.m_Bolt2Cnt = f.data.pilePillerData.jointDetailData.FlangeBolt.BoltCount *
                                      f.data.pilePillerData.pJointCount ;
                  }

                  PilePiller.WriteToElement( pile, _doc ) ;
                  pIds.Add( pId ) ;

                  //継足し杭
                  if ( f.data.pilePillerData.ExtensionPile && exSym != null ) {
                    pId = PilePillerSuper.CreateExtraPile( _doc, kv.Key, adjustAngle, f.data.pilePillerData,
                      f.data.KoudaiName, adjustVec, baselevel, exPOffset, exSym ) ;
                    ExtensionPile exPile = new ExtensionPile( pId, f.data.KoudaiName, f.data.pilePillerData,
                      RevitUtil.ClsCommonUtils.ChangeStrToInt( pitchs[ 0 ] ),
                      RevitUtil.ClsCommonUtils.ChangeStrToInt( pitchs[ 1 ] ) ) ;
                    ExtensionPile.WriteToElement( exPile, _doc ) ;
                    pIds.Add( pId ) ;
                  }
                }
              }
            }

            //モデルデータセット
            f.data.KyoutyouVecMem = new List<double> { vecKyoutyou.X, vecKyoutyou.Y, vecKyoutyou.Z } ;
            f.data.HukuinVecMem = new List<double> { vecHukuin.X, vecHukuin.Y, vecHukuin.Z } ;
            ModelData md = ModelData.GetModelData( _doc ) ;
            KoudaiData kd = new KoudaiData() ;
            kd.AllKoudaiFlatData = f.data ;
            kd.CalcFileData = f.calcFileData ;
            //md.ListKoudaiData.Add(kd);
            md.UpdateKoudaiData( kd ) ;
            ModelData.SetModelData( _doc, md ) ;

            tr.Commit() ;
          }
        }

        MessageUtil.Information( "構台フラット一括配置が完了しました", "構台一括配置" ) ;
      }
      catch ( Exception ex ) {
        #if DEBUG
        TaskDialog taskDialog = new TaskDialog( "Error!!!" )
        {
          TitleAutoPrefix = true,
          MainIcon = TaskDialogIcon.TaskDialogIconError,
          MainInstruction = ex.GetType().ToString().Split( '.' ).LastOrDefault(),
          MainContent = ex.Message,
          ExpandedContent = ex.StackTrace,
          CommonButtons = TaskDialogCommonButtons.Close,
        } ;
        taskDialog.Show() ;
        #else
                _logger.Error(ex.Message);
                MessageUtil.Error("構台フラット一括配置に失敗しました", "構台一括配置");
        #endif
      }

      return retval ;
    }
  }
}