using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using Autodesk.Revit.UI.Selection ;
using System ;
using System.Collections.Generic ;
using System.IO ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;
using YMS_gantry.Material ;
using YMS_gantry.UI ;
using static YMS_gantry.DefineUtil ;

namespace YMS_gantry.Command
{
  class CmdCreateTakasaChousei
  {
    static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger() ;
    Document _doc { get ; set ; }
    UIDocument _uiDoc { get ; set ; }

    public CmdCreateTakasaChousei( UIDocument uiDoc )
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
        //FrmPutTakasaChouseiPlateChouseizai frm = new FrmPutTakasaChouseiPlateChouseizai(_uiDoc.Application);
        //if (frm.ShowDialog() != DialogResult.OK)
        //{
        //    return false;
        //}
        FrmPutTakasaChouseiPlateChouseizai frm = Application.thisApp.frmPutTakasaChouseiPlateChouseizai ;

        FamilySymbol sym = null ;
        Selection selection = _uiDoc.Selection ;
        if ( frm.RbtPlate.Checked ) {
          //構成要素
          TakasaChouseizai tc = new TakasaChouseizai() ;
          tc.m_KodaiName = frm.CmbKoudaiName.Text ;
          tc.m_Material = frm.CmbMaterial.Text ;
          tc.m_W = (int) frm.NmcPLW.Value ;
          tc.m_D = (int) frm.NmcPLD.Value ;
          tc.m_H1 = (int) frm.NmcPLH1.Value ;
          tc.m_H2 = (int) frm.NmcPLH2.Value ;
          tc.m_Size = $"{tc.m_W}x{tc.m_D}x{tc.m_H1}x{tc.m_H2}" ;
          tc.m_Document = _doc ;

          string platePath = Master.ClsTakasaChouseiCsv.GetFamilyPath( tc.typeName ) ;
          if ( ! File.Exists( platePath ) ) {
            MessageUtil.Error( "ファミリが見つかりません", "高さ調整材" ) ;
            return false ;
          }

          Dictionary<string, string> paramList = new Dictionary<string, string>() ;
          paramList.Add( DefineUtil.PARAM_MATERIAL, tc.m_Material ) ;
          paramList.Add( "W", $"{tc.m_W}" ) ;
          paramList.Add( "D", $"{tc.m_D}" ) ;
          paramList.Add( "H1", $"{tc.m_H1}" ) ;
          paramList.Add( "H2", $"{tc.m_H2}" ) ;
          string typeName = GantryUtil.CreateTypeName( tc.typeName, paramList ) ;

          if ( frm.RbtFace.Checked ) {
            //高さ調整材
            using ( Transaction tr = new Transaction( _doc ) ) {
              tr.Start( "TakasaChousei" ) ;
              string type = Path.GetFileNameWithoutExtension( platePath ) ;
              //シンボル作成
              if ( ! GantryUtil.GetFamilySymbol( _doc, platePath, type, out sym, true ) ) {
                return false ;
              }

              sym = GantryUtil.DuplicateTypeWithNameRule( _doc, frm.CmbKoudaiName.Text, sym, type ) ;
              //sym = GantryUtil.ChangeTypeID(_doc, sym, typeName);
              //タイプパラメータ設定
              foreach ( KeyValuePair<string, string> kv in paramList ) {
                GantryUtil.SetSymbolParameterValueByParameterName( sym, kv.Key, kv.Value ) ;
              }

              tr.Commit() ;

              GantryUtil util = new GantryUtil() ;
              //配置
              List<ElementId> ids = util.PlaceFamilyInstance( _uiDoc.Application, sym, false ) ;
              tr.Start( "ChouseiRelated" ) ;
              foreach ( ElementId id in ids ) {
                tc.m_ElementId = id ;
                TakasaChouseizai.WriteToElement( tc, _doc ) ;
              }

              tr.Commit() ;
            }
          }
          else {
            //自由位置
            //基準レベル
            Level baselevel = null ;
            double offset = (double) frm.NmcOffset.Value ;
            while ( true ) {
              if ( frm.CmbLevel.Text == "部材選択" ) {
                ElementId id = selection.PickObject( ObjectType.Element, "基準となる部材を選択してください" ).ElementId ;
                FamilyInstance ins = _doc.GetElement( id ) as FamilyInstance ;
                baselevel = GantryUtil.GetInstanceLevelAndOffset( _doc, ins, ref offset ) ;
              }
              else {
                baselevel = RevitUtil.ClsRevitUtil.GetLevel( _doc, frm.CmbLevel.Text ) as Level ;
              }

              if ( baselevel == null ) {
                return false ;
              }

              XYZ origin, sidePnt, vecSide, headPnt, orthogolanVec ;
              ElementId pointId = ElementId.InvalidElementId, LineId = ElementId.InvalidElementId ;
              try {
                //配置基準＋幅員、橋長方向取得
                origin = selection.PickPoint( ObjectSnapTypes.Nearest, "挿入の基点を指定" ) ;
                pointId = GantryUtil.InsertPointFamily( _doc, origin, baselevel ) ;

                //幅員方向ベクトル
                sidePnt = selection.PickPoint( ObjectSnapTypes.Nearest, "横方向を指定" ) ;
                vecSide = ( sidePnt - origin ).Normalize() ;
                LineId = GantryUtil.InsertLineFamily( _doc, origin, sidePnt, baselevel ) ;

                //橋軸方向ベクトル
                headPnt = selection.PickPoint( ObjectSnapTypes.Nearest, "縦方向を指定" ) ;
                orthogolanVec = vecSide.CrossProduct( XYZ.BasisZ ) ;

                using ( Transaction tr = new Transaction( _doc ) ) {
                  tr.Start( "FlatSettings" ) ;
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

                  XYZ vecHead = ( headPnt - origin ).Normalize() ;
                  if ( RevitUtil.ClsGeo.GEO_GE0( orthogolanVec.DotProduct( vecHead ) ) ) {
                    vecHead = orthogolanVec.Normalize() ;
                  }
                  else {
                    vecHead = orthogolanVec.Negate().Normalize() ;
                  }

                  //シンボル作成
                  if ( ! GantryUtil.GetFamilySymbol( _doc, platePath, "高さ調整ﾌﾟﾚｰﾄ", out sym, true ) ) {
                    return false ;
                  }

                  bool isLeft = RevitUtil.ClsGeo.IsLeft( vecSide, vecHead ) ;
                  sym = GantryUtil.DuplicateTypeWithNameRule( _doc, frm.CmbKoudaiName.Text, sym, "高さ調整ﾌﾟﾚｰﾄ" ) ;

                  double adjustAngle = XYZ.BasisX.AngleTo( vecSide ) *
                                       ( RevitUtil.ClsGeo.GEO_LT0( vecSide.Y ) ? -1 : 1 ) ;
                  FamilyInstance newins =
                    GantryUtil.CreateInstanceWith1point( _doc, origin, baselevel.GetPlaneReference(), sym ) ;
                  if ( newins != null ) {
                    ElementId idnew = newins.Id ;
                    RevitUtil.ClsRevitUtil.SetParameter( _doc, idnew, DefineUtil.PARAM_BASE_OFFSET,
                      RevitUtil.ClsRevitUtil.CovertToAPI( offset ) ) ;
                    RevitUtil.ClsRevitUtil.RotateElement( _doc, idnew, Line.CreateBound( origin, origin + XYZ.BasisZ ),
                      adjustAngle ) ;
                    RevitUtil.ClsRevitUtil.SetParameter( _doc, newins.Symbol.Id, "W",
                      RevitUtil.ClsRevitUtil.CovertToAPI( tc.m_W ) ) ;
                    RevitUtil.ClsRevitUtil.SetParameter( _doc, newins.Symbol.Id, "D",
                      RevitUtil.ClsRevitUtil.CovertToAPI( tc.m_D ) ) ;
                    RevitUtil.ClsRevitUtil.SetParameter( _doc, newins.Symbol.Id, "H1",
                      RevitUtil.ClsRevitUtil.CovertToAPI( tc.m_H1 ) ) ;
                    RevitUtil.ClsRevitUtil.SetParameter( _doc, newins.Symbol.Id, "H2",
                      RevitUtil.ClsRevitUtil.CovertToAPI( tc.m_H2 ) ) ;

                    //個別情報追加
                    tc.m_ElementId = idnew ;
                    TakasaChouseizai.WriteToElement( tc, _doc ) ;
                    tr.Commit() ;
                  }
                }
              }
              catch ( Autodesk.Revit.Exceptions.OperationCanceledException ) {
                break ;
              }
              finally {
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
            }
          }
        }
        //補助ピース
        else {
          //部材情報
          HojoPieace hP = new HojoPieace() ;
          hP.m_Document = _doc ;
          hP.m_KodaiName = frm.CmbKoudaiName.Text ;
          hP.m_Material = frm.CmbMaterial.Text ;
          hP.m_Size = frm.CmbPieaceSize.Text ;
          if ( frm.CmbPieaceSize.Text.StartsWith( "H" ) ) {
            hP.m_H1 = (int) frm.NmcPieaceH1.Value ;
            hP.m_H2 = (int) frm.NmcPieaceH2.Value ;
            if ( ! frm.ChckCutOn.Checked ) {
              hP.m_H2 = (int) frm.NmcPieaceH1.Value ;
            }
          }

          string pieacePath = Master.ClsTakasaChouseiCsv.GetFamilyPath( hP.m_Size ) ;
          if ( ! File.Exists( pieacePath ) ) {
            MessageUtil.Error( "ファミリが見つかりません", "高さ調整材" ) ;
            return false ;
          }

          Dictionary<string, string> paramList = new Dictionary<string, string>() ;
          paramList.Add( DefineUtil.PARAM_MATERIAL, hP.m_Material ) ;
          if ( frm.CmbPieaceSize.Text.StartsWith( "H" ) ) {
            paramList.Add( "H1", $"{hP.m_H1}" ) ;
            paramList.Add( "H2", $"{hP.m_H2}" ) ;
          }

          string pieaceType = frm.CmbPieceType.Text ;
          if ( frm.RbtFace.Checked ) {
            //高さ調整材
            using ( Transaction tr = new Transaction( _doc ) ) {
              tr.Start( "TakasaChousei" ) ;
              //シンボル作成
              string type = frm.CmbPieaceSize.Text.StartsWith( "H" )
                ?
                pieaceType.Contains( "ウェブ" ) ? "ｳｪﾌﾞ面" : "ﾌﾗﾝｼﾞ面"
                : frm.CmbPieaceSize.Text.StartsWith( "SM" )
                  ? "高強度補助ﾋﾟｰｽ"
                  : "補助ﾋﾟｰｽ" ;
              if ( ! GantryUtil.GetFamilySymbol( _doc, pieacePath, type, out sym, true ) ) {
                return false ;
              }

              //EndPL
              string topPLType = "調整ﾋﾟｰｽ_ｴﾝﾄﾞﾌﾟﾚｰﾄ_" + frm.CmbTopEndPLSize.Text.Replace( "00", "0" )
                .Replace( "50", "5" ).Replace( "(ｶｲｻｷ)", "" ).Replace( "END PL", "ENDPL" ) ;
              if ( frm.CmbPieaceSize.Text.StartsWith( "H5" ) ) {
                topPLType = topPLType.Replace( "5", "50" ) ;
              }

              string typName = frm.CmbTopEndPLSize.Text ;
              FamilySymbol pltSym = RevitUtil.ClsRevitUtil.GetFamilySymbolWithFuzzy( _doc, topPLType, typName ) ;
              if ( pltSym == null ) {
                pltSym = RevitUtil.ClsRevitUtil.GetFamilySymbolWithFuzzy( _doc, "調整ﾋﾟｰｽ_ｴﾝﾄﾞﾌﾟﾚｰﾄ_任意", "上" ) ;
              }

              if ( pltSym != null ) {
                paramList.Add( DefineUtil.PARAM_END_PLATE_SIZE_U, pltSym.Id.ToString() ) ;
              }

              string btmPLType = "調整ﾋﾟｰｽ_ｴﾝﾄﾞﾌﾟﾚｰﾄ_" + frm.CmbBtmEndPLSize.Text.Replace( "00", "0" )
                .Replace( "50", "5" ).Replace( "(ｶｲｻｷ)", "" ).Replace( "END PL", "ENDPL" ) ;
              if ( frm.CmbPieaceSize.Text.StartsWith( "H5" ) ) {
                btmPLType = btmPLType.Replace( "5", "50" ) ;
              }

              typName = frm.CmbBtmEndPLSize.Text ;
              pltSym = RevitUtil.ClsRevitUtil.GetFamilySymbolWithFuzzy( _doc, btmPLType, typName ) ;
              if ( pltSym == null ) {
                pltSym = RevitUtil.ClsRevitUtil.GetFamilySymbolWithFuzzy( _doc, "調整ﾋﾟｰｽ_ｴﾝﾄﾞﾌﾟﾚｰﾄ_任意", "下" ) ;
              }

              if ( pltSym != null ) {
                paramList.Add( DefineUtil.PARAM_END_PLATE_SIZE_B, pltSym.Id.ToString() ) ;
              }

              //sym = GantryUtil.ChangeTypeID(_doc, sym, typeName);
              if ( frm.CmbPieaceSize.Text.StartsWith( "H" ) ) {
                sym = GantryUtil.DuplicateTypeWithNameRule( _doc, frm.CmbKoudaiName.Text, sym, "高さ調整ﾋﾟｰｽ" ) ;
              }

              //タイプパラメータ設定
              foreach ( KeyValuePair<string, string> kv in paramList ) {
                GantryUtil.SetSymbolParameterValueByParameterName( sym, kv.Key, kv.Value ) ;
              }

              tr.Commit() ;

              GantryUtil util = new GantryUtil() ;
              //配置
              List<ElementId> ids = util.PlaceFamilyInstance( _uiDoc.Application, sym, false ) ;
              tr.Start( "ChouseiRelated" ) ;
              foreach ( ElementId id in ids ) {
                hP.m_ElementId = id ;
                HojoPieace.WriteToElement( hP, _doc ) ;
              }

              tr.Commit() ;
            }
          }
          else {
            //自由位置
            //基準レベル
            Level baselevel = null ;
            double offset = (double) frm.NmcOffset.Value ;
            while ( true ) {
              if ( frm.CmbLevel.Text == "部材選択" ) {
                ElementId id = selection.PickObject( ObjectType.Element, "基準となる部材を選択してください" ).ElementId ;
                FamilyInstance ins = _doc.GetElement( id ) as FamilyInstance ;
                baselevel = GantryUtil.GetInstanceLevelAndOffset( _doc, ins, ref offset ) ;
              }
              else {
                baselevel = RevitUtil.ClsRevitUtil.GetLevel( _doc, frm.CmbLevel.Text ) as Level ;
              }

              if ( baselevel == null ) {
                return false ;
              }

              XYZ origin, sidePnt, vecSide, headPnt, orthogolanVec ;
              ElementId pointId = ElementId.InvalidElementId, LineId = ElementId.InvalidElementId ;
              using ( Transaction tr = new Transaction( _doc ) ) {
                tr.Start( "FlatSettings" ) ;

                try {
                  //配置基準＋幅員、橋長方向取得
                  origin = selection.PickPoint( ObjectSnapTypes.Nearest, "挿入の基点を指定" ) ;
                  pointId = GantryUtil.InsertPointFamily( _doc, origin, baselevel, true ) ;
                  _doc.Regenerate() ;
                  //幅員方向ベクトル
                  sidePnt = selection.PickPoint( ObjectSnapTypes.Nearest, "横方向を指定" ) ;
                  vecSide = ( sidePnt - origin ).Normalize() ;
                  LineId = GantryUtil.InsertLineFamily( _doc, origin, sidePnt, baselevel, true ) ;
                  _doc.Regenerate() ;

                  //橋軸方向ベクトル
                  headPnt = selection.PickPoint( ObjectSnapTypes.Nearest, "縦方向を指定" ) ;
                  orthogolanVec = vecSide.CrossProduct( XYZ.BasisZ ) ;


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

                  XYZ vecHead = ( headPnt - origin ).Normalize() ;
                  if ( RevitUtil.ClsGeo.GEO_GE0( orthogolanVec.DotProduct( vecHead ) ) ) {
                    vecHead = orthogolanVec.Normalize() ;
                  }
                  else {
                    vecHead = orthogolanVec.Negate().Normalize() ;
                  }

                  bool isLeft = RevitUtil.ClsGeo.IsLeft( vecSide, vecHead ) ;

                  double adjustAngle = XYZ.BasisX.AngleTo( vecSide ) *
                                       ( RevitUtil.ClsGeo.GEO_LT0( vecSide.Y ) ? -1 : 1 ) ;
                  //シンボル作成
                  string type = frm.CmbPieaceSize.Text.StartsWith( "H" )
                    ?
                    pieaceType.Contains( "ウェブ" ) ? "ｳｪﾌﾞ面" : "ﾌﾗﾝｼﾞ面"
                    : frm.CmbPieaceSize.Text.StartsWith( "SM" )
                      ? "高強度補助ﾋﾟｰｽ"
                      : "補助ﾋﾟｰｽ" ;
                  if ( ! GantryUtil.GetFamilySymbol( _doc, pieacePath, type, out sym, true ) ) {
                    return false ;
                  }

                  //EndPL
                  string topPLType = "調整ﾋﾟｰｽ_ｴﾝﾄﾞﾌﾟﾚｰﾄ_" + frm.CmbTopEndPLSize.Text.Replace( "00", "0" )
                    .Replace( "50", "5" ).Replace( "(ｶｲｻｷ)", "" ).Replace( "END PL", "ENDPL" ) ;
                  if ( frm.CmbPieaceSize.Text.StartsWith( "H5" ) ) {
                    topPLType = topPLType.Replace( "5", "50" ) ;
                  }

                  string typName = frm.CmbTopEndPLSize.Text ;
                  FamilySymbol pltSym = RevitUtil.ClsRevitUtil.GetFamilySymbolWithFuzzy( _doc, topPLType, typName ) ;
                  if ( pltSym == null ) {
                    pltSym = RevitUtil.ClsRevitUtil.GetFamilySymbolWithFuzzy( _doc, "調整ﾋﾟｰｽ_ｴﾝﾄﾞﾌﾟﾚｰﾄ_任意", "上" ) ;
                  }

                  if ( pltSym != null ) {
                    paramList.Add( DefineUtil.PARAM_END_PLATE_SIZE_U, pltSym.Id.ToString() ) ;
                  }

                  string btmPLType = "調整ﾋﾟｰｽ_ｴﾝﾄﾞﾌﾟﾚｰﾄ_" + frm.CmbBtmEndPLSize.Text.Replace( "00", "0" )
                    .Replace( "50", "5" ).Replace( "(ｶｲｻｷ)", "" ).Replace( "END PL", "ENDPL" ) ;
                  if ( frm.CmbPieaceSize.Text.StartsWith( "H5" ) ) {
                    btmPLType = btmPLType.Replace( "5", "50" ) ;
                  }

                  typName = frm.CmbBtmEndPLSize.Text ;
                  pltSym = RevitUtil.ClsRevitUtil.GetFamilySymbolWithFuzzy( _doc, btmPLType, typName ) ;
                  if ( pltSym == null ) {
                    pltSym = RevitUtil.ClsRevitUtil.GetFamilySymbolWithFuzzy( _doc, "調整ﾋﾟｰｽ_ｴﾝﾄﾞﾌﾟﾚｰﾄ_任意", "下" ) ;
                  }

                  if ( pltSym != null ) {
                    paramList.Add( DefineUtil.PARAM_END_PLATE_SIZE_B, pltSym.Id.ToString() ) ;
                  }

                  foreach ( KeyValuePair<string, string> kv in paramList ) {
                    GantryUtil.SetSymbolParameterValueByParameterName( sym, kv.Key, kv.Value ) ;
                  }

                  if ( frm.CmbPieaceSize.Text.StartsWith( "H" ) ) {
                    sym = GantryUtil.DuplicateTypeWithNameRule( _doc, frm.CmbKoudaiName.Text, sym, "高さ調整ﾋﾟｰｽ" ) ;
                  }

                  //sym = GantryUtil.ChangeTypeID(_doc, sym, typeName);
                  ElementId idnew = GantryUtil
                    .CreateInstanceWith1point( _doc, origin, baselevel.GetPlaneReference(), sym ).Id ;
                  RevitUtil.ClsRevitUtil.SetParameter( _doc, idnew, DefineUtil.PARAM_BASE_OFFSET,
                    RevitUtil.ClsRevitUtil.CovertToAPI( offset ) ) ;
                  RevitUtil.ClsRevitUtil.RotateElement( _doc, idnew, Line.CreateBound( origin, origin + XYZ.BasisZ ),
                    adjustAngle ) ;
                  //個別情報追加
                  hP.m_ElementId = idnew ;
                  HojoPieace.WriteToElement( hP, _doc ) ;
                }
                catch ( Autodesk.Revit.Exceptions.OperationCanceledException ) {
                  break ;
                }
                finally {
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

                tr.Commit() ;
              }
            }
          }
        }

        return true ;
      }
      catch ( Exception ex ) {
        return false ;
      }
    }
  }
}