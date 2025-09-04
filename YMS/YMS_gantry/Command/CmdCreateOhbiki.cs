using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using Autodesk.Revit.UI.Selection ;
using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.IO ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;
using YMS_gantry.Data ;
using YMS_gantry.UI ;
using static YMS_gantry.DefineUtil ;

namespace YMS_gantry.Command
{
  class CmdCreateOhbiki
  {
    static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger() ;
    Document _doc { get ; set ; }
    UIDocument _uiDoc { get ; set ; }

    public CmdCreateOhbiki( UIDocument uiDoc )
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
      bool retBool = true ;
      try {
        //FrmPutOhbikiKetauke frm = new FrmPutOhbikiKetauke(_uiDoc.Application);
        //if (frm.ShowDialog() != DialogResult.OK) { return false; }
        FrmPutOhbikiKetauke frm = Application.thisApp.frmPutOhbikiKetauke ;

        Selection selection = _uiDoc.Selection ;

        //大引用データ作成-------------------------------------------------------------------------------
        AllKoudaiFlatFrmData allData = GantryUtil.GetKoudaiData( _doc, frm.CmbKoudaiName.Text ).AllKoudaiFlatData ;
        OhbikiData oData = new OhbikiData() ;
        XYZ kyoutyou = allData.GetKyoutyouVec( _uiDoc.Document ) ;
        XYZ Hukuin = allData.GetHukuinVec( _uiDoc.Document ) ;
        oData.OhbikiSize = frm.CmbSize.Text ;
        oData.OhbikiMaterial = frm.CmbMaterial.Text ;
        oData.isHkou = frm.CmbSizeType.Text.Contains( "H形鋼" ) || frm.CmbSizeType.Text.Contains( "主材" ) ;
        ;
        oData.OhbikiDan = ( frm.RbtOhbikiSingle.Checked ) ? 1 : ( ( frm.RbtOhbikiDouble.Checked ) ? 2 : 3 ) ;
        oData.OhbikiAttachType = frm.RbtOhbikiBolt.Checked ? eJoinType.Bolt : eJoinType.Welding ;
        oData.OhbikiAttachWay = frm.RbtOneSide.Checked ? eAttachWay.OneSide : eAttachWay.BothSide ;
        oData.OhbikiBoltType = frm.RbtOhbikiNormalB.Checked ? eBoltType.Normal : eBoltType.HTB ;
        oData.OhbikiBoltCount = (int) frm.NmcOhbikiBoltCnt.Value ;
        //基準レベル取得
        Level level = ClsRevitUtil.GetLevel( _doc, frm.CmbLevel.Text ) as Level ;
        double offset = (double) frm.NmcOffset.Value ;
        bool isDoboku = allData.KoujiType == DefineUtil.eKoujiType.Doboku ;
        //各部材高さ
        MaterialSize ohbikiSize = GantryUtil.GetKouzaiSize( oData.OhbikiSize ) ;
        //大引用データ作成-------------------------------------------------------------------------------

        //配置
        using ( Transaction trans = new Transaction( _doc ) ) {
          if ( frm.RbtKui.Checked ) {
            trans.Start( "Ohbiki Placement" ) ;
            ClsRevitUtil.UnSelectElement( _uiDoc ) ;
            SelectionFilterUtil pickFilter = new SelectionFilterUtil( _uiDoc,
              new List<string> { PilePiller.typeName, PilePiller.sichuTypeName } ) ;
            List<FamilyInstance> list = new List<FamilyInstance>() ;
            if ( ! pickFilter.Selection( "杭または支柱を選択してください", out list ) ) {
              return false ;
            }

            if ( list.Count == 0 ) {
              return false ;
            }

            if ( list.Count == 1 ) {
              MessageUtil.Warning( "作成には２つ以上選択してください", "大引・桁受配置" ) ;
              return false ;
            }

            MaterialSuper[] ExPiles = MaterialSuper.Collect( _doc ).Where( x =>
              x.m_KodaiName == frm.CmbKoudaiName.Text && x.GetType() == typeof( ExtensionPile ) ).ToArray() ;
            List<ElementId> exPileId = new List<ElementId>() ;
            if ( ExPiles.Length > 0 ) {
              exPileId = ExPiles.Select( x => x.m_ElementId ).ToList() ;
            }

            //選択された柱を順に見る
            for ( int pCount = 0 ; pCount < list.Count - 1 ; pCount++ ) {
              FamilyInstance ins1 = list[ pCount ] ;
              FamilyInstance ins2 = list[ pCount + 1 ] ;
              XYZ p1 = ( ins1.Location as LocationPoint ).Point ;
              XYZ p2 = ( ins2.Location as LocationPoint ).Point ;
              level = GantryUtil.GetInstanceLevelAndOffset( _doc, ins1, ref offset ) ;
              if ( level == null ) {
                MessageUtil.Warning( "選択した杭または支柱がレベルに配置されていないため、配置できません", "大引・桁受配置" ) ;
                return false ;
              }

              p1 = new XYZ( p1.X, p1.Y, level.Elevation ) ;
              p2 = new XYZ( p2.X, p2.Y, level.Elevation ) ;
              XYZ vec = ( p2 - p1 ).Normalize() ;

              //1本目の高さに合わせて継足し杭有無、切断長さ、ﾄｯﾌﾟﾌﾟﾚｰﾄ有無を判定して高さを変える
              double cutLen = RevitUtil.ClsRevitUtil.GetTypeParameter( ins1.Symbol, DefineUtil.PARAM_PILE_CUT_LENG ) ;
              int topPum = RevitUtil.ClsRevitUtil.GetTypeParameterInt( ins1.Symbol, DefineUtil.PARAM_TOP_PLATE ) ;
              if ( exPileId.Count > 0 ) {
                List<ElementId> intExPile =
                  RevitUtil.ClsRevitUtil.GetIntersectFamilys( _doc, ins1.Id, serchIds: exPileId ) ;
                if ( intExPile.Count > 0 ) {
                  FamilyInstance ins = _doc.GetElement( intExPile.First() ) as FamilyInstance ;
                  offset += GetTopPLThick( ins.Symbol ) ;
                }
                else {
                  offset -= RevitUtil.ClsRevitUtil.CovertFromAPI( cutLen ) ;
                  if ( topPum.Equals( 1 ) ) {
                    offset += GetTopPLThick( ins1.Symbol ) ;
                  }
                }
              }
              else {
                offset -= RevitUtil.ClsRevitUtil.CovertFromAPI( cutLen ) ;
                if ( topPum.Equals( 1 ) ) {
                  offset += GetTopPLThick( ins1.Symbol ) ;
                }
              }


              if ( oData.isHkou ) {
                //始めの１つは始点側分伸ばす
                if ( pCount == 0 ) {
                  oData.exOhbikiStartLeng = (double) frm.NmcSLng.Value ;
                }
                else {
                  oData.exOhbikiStartLeng = 0 ;
                }

                //終点側も指定分伸ばす
                if ( pCount + 1 == list.Count - 1 ) {
                  oData.exOhbikiEndLeng = (double) frm.NmcELng.Value ;
                }
                else {
                  oData.exOhbikiEndLeng = 0 ;
                }

                List<ElementId> oId = Ohbiki.CreateOhbiki( _doc, oData, p1, p2, level, isDoboku, frm.CmbKoudaiName.Text,
                  offset + ( ohbikiSize.Height / 2 ) ) ;
                //個別情報追加
                foreach ( ElementId id in oId ) {
                  Ohbiki o = new Ohbiki( id, frm.CmbKoudaiName.Text, oData ) ;
                  Ohbiki.WriteToElement( o, _doc ) ;
                }
              }
              else
                //H鋼以外
              {
                //柱のサイズを見て外側にずらす
                MaterialSize p1Size = GantryUtil.GetKouzaiSize( ins1.Symbol.FamilyName ) ;
                MaterialSize p2Size = GantryUtil.GetKouzaiSize( ins2.Symbol.FamilyName ) ;
                XYZ vecCros = vec.CrossProduct( XYZ.BasisZ ).Normalize() ;

                //片側の時はさらに選択させる
                if ( oData.OhbikiAttachWay == eAttachWay.OneSide ) {
                  bool isLeft = ClsGeo.IsLeft( vec, vecCros ) ;
                  ClsRevitUtil.UnSelectElement( _uiDoc ) ;
                  ClsRevitUtil.SelectElement( _uiDoc, new List<ElementId> { ins1.Id, ins2.Id } ) ;
                  XYZ pWay = selection.PickPoint( "配置する側を選択してください" ) ;
                  ClsRevitUtil.UnSelectElement( _uiDoc ) ;

                  if ( ( isLeft && ! ClsGeo.IsLeft( vec, ( pWay - p1 ) ) ) ||
                       ( ! isLeft && ClsGeo.IsLeft( vec, ( pWay - p1 ) ) ) ) {
                    vecCros = vecCros.Negate() ;
                  }
                }

                XYZ pnt1 = p1 + vecCros * ( ClsRevitUtil.ConvertDoubleGeo2Revit( p1Size.Height / 2 ) ) +
                           XYZ.BasisZ * ( ClsRevitUtil.ConvertDoubleGeo2Revit( ohbikiSize.Height ) ) ;

                XYZ pnt2 = p2 + vecCros * ( ClsRevitUtil.ConvertDoubleGeo2Revit( p2Size.Height / 2 ) ) +
                           XYZ.BasisZ * ( ClsRevitUtil.ConvertDoubleGeo2Revit( ohbikiSize.Height ) ) ;
                //始めの１つは始点側分伸ばす
                if ( pCount == 0 ) {
                  oData.exOhbikiStartLeng = (double) frm.NmcSLng.Value ;
                }
                else {
                  oData.exOhbikiStartLeng = 0 ;
                }

                //終点側も指定分伸ばす
                if ( pCount + 1 == list.Count - 1 ) {
                  oData.exOhbikiEndLeng = (double) frm.NmcELng.Value ;
                }
                else {
                  oData.exOhbikiEndLeng = 0 ;
                }

                double rotate = ohbikiSize.Shape == MaterialShape.C ? 270 : 180 ;
                if ( allData.BaseLevel == eBaseLevel.FukkouTop ) {
                  MaterialSize neda = GantryUtil.GetKouzaiSize( allData.nedaData.NedaSize ) ;
                  offset = offset - neda.Height - FukkouBAN_THICK - ( ohbikiSize.Height * oData.OhbikiDan ) +
                           ( ohbikiSize.Height / 2 ) ;
                }
                else {
                  offset = offset - ( ohbikiSize.Height / 2 ) ;
                }

                List<ElementId> oId = Ohbiki.CreateOhbiki( _doc, oData, pnt1, pnt2, level, isDoboku,
                  frm.CmbKoudaiName.Text, offset, rotate ) ;

                if ( oData.OhbikiAttachWay == eAttachWay.BothSide ) {
                  pnt1 = p2 + vecCros.Negate() * ( ClsRevitUtil.ConvertDoubleGeo2Revit( p1Size.Height / 2 ) ) ;
                  pnt2 = p1 + vecCros.Negate() * ( ClsRevitUtil.ConvertDoubleGeo2Revit( p2Size.Height / 2 ) ) ;
                  oId.AddRange( Ohbiki.CreateOhbiki( _doc, oData, pnt1, pnt2, level, isDoboku, frm.CmbKoudaiName.Text,
                    offset, rotate, reverse: true ) ) ;
                }

                foreach ( ElementId id in oId ) {
                  Ohbiki o = new Ohbiki( id, frm.CmbKoudaiName.Text, oData ) ;
                  o.m_TeiketsuType = oData.OhbikiAttachType ;
                  if ( o.m_TeiketsuType == eJoinType.Bolt ) {
                    o.m_BoltInfo1 = oData.OhbikiBoltType == eBoltType.Normal ? "通常" : "ハイテンション" ;
                    o.m_Bolt1Cnt = oData.OhbikiBoltCount ;
                  }

                  Ohbiki.WriteToElement( o, _doc ) ;
                }
              }
            }

            trans.Commit() ;
          }
          else
            //自由位置配置
          {
            while ( true ) {
              trans.Start( "Ohbiki Placement" ) ;
              FamilySymbol sym ;
              string familyPath = Master.ClsMasterCsv.GetFamilyPath( oData.OhbikiSize ) ;
              string type = frm.Text.Contains( "桁受" ) ? "桁受" : Ohbiki.typeName ;
              if ( ! GantryUtil.GetFamilySymbol( _doc, familyPath, type, out sym, true ) ) {
                return false ;
              }

              ElementId newId = ElementId.InvalidElementId ;
              Dictionary<string, string> paramList = new Dictionary<string, string>() ;
              paramList.Add( DefineUtil.PARAM_MATERIAL, oData.OhbikiMaterial ) ;
              sym = GantryUtil.DuplicateTypeWithNameRule( _doc, frm.CmbKoudaiName.Text, sym, Ohbiki.typeName ) ;

              //sym = GantryUtil.ChangeTypeID(_doc, sym,  GantryUtil.CreateTypeName(Ohbiki.typeName, paramList));
              //タイプパラメータ設定
              foreach ( KeyValuePair<string, string> kv in paramList ) {
                GantryUtil.SetParameterValueByParameterName( sym, kv.Key, kv.Value ) ;
              }

              try {
                if ( frm.CmbLevel.Text == "部材選択" ) {
                  newId = GantryUtil.PlaceSymbolOverTheSelectedElm( _uiDoc, sym, frm.CmbKoudaiName.Text, level, offset,
                    (double) frm.NmcSLng.Value, (double) frm.NmcELng.Value ) ;
                }
                else {
                  XYZ p1 = selection.PickPoint( "1点目を指定してください" ) ;
                  XYZ p2 = selection.PickPoint( "2点目を指定してください" ) ;
                  if ( p1 == null || p2 == null ) {
                    return false ;
                  }

                  p1 = new XYZ( p1.X, p1.Y, level.Elevation ) ;
                  p2 = new XYZ( p2.X, p2.Y, level.Elevation ) ;
                  XYZ vec = ( p2 - p1 ).Normalize() ;
                  if ( frm.NmcSLng.Value > 0 ) {
                    p1 = p1 + vec.Negate() *
                      RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( (double) frm.NmcSLng.Value ) ;
                  }

                  if ( frm.NmcELng.Value > 0 ) {
                    p2 = p2 + vec * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( (double) frm.NmcELng.Value ) ;
                  }

                  newId = MaterialSuper.PlaceWithTwoPoints( sym, level.GetPlaneReference(), p1, p2,
                    RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( offset ) ) ;
                }

                if ( newId != ElementId.InvalidElementId || newId != null ) {
                  Ohbiki ohbiki = new Ohbiki( newId, frm.CmbKoudaiName.Text, oData ) ;
                  Ohbiki.WriteToElement( ohbiki, _doc ) ;
                }
              }
              catch ( Autodesk.Revit.Exceptions.OperationCanceledException ) {
                return false ;
              }

              trans.Commit() ;
            }
          }
        }
      }
      catch ( Exception ex ) {
        retBool = false ;
      }

      return retBool ;
    }

    private double GetTopPLThick( FamilySymbol sym )
    {
      double retB = 0 ;
      if ( sym != null ) {
        int topPum = RevitUtil.ClsRevitUtil.GetTypeParameterInt( sym, DefineUtil.PARAM_TOP_PLATE ) ;
        if ( topPum.Equals( 1 ) ) {
          Parameter topP = sym.LookupParameter( DefineUtil.PARAM_TOP_PLATE_SIZE ) ;
          if ( topP != null ) {
            var rr = topP.AsElementId() ;
            FamilySymbol s = _doc.GetElement( rr ) as FamilySymbol ;
            if ( s.FamilyName.Contains( "定形" ) ) {
              string n = s.Name.Replace( "PL-", "" ) ;
              retB = RevitUtil.ClsCommonUtils.ChangeStrToDbl( n.Substring( 0, 2 ) ) ;
            }
            else {
              retB = RevitUtil.ClsRevitUtil.GetTypeParameter( sym, DefineUtil.PARAM_TOP_PLATE_T ) ;
            }
          }
        }
      }

      return retB ;
    }
  }
}