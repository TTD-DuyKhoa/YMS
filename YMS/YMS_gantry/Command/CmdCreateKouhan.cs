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
using YMS_gantry.Material ;
using YMS_gantry.UI ;
using static YMS_gantry.DefineUtil ;


namespace YMS_gantry.Command
{
  class CmdCreateKouhan
  {
    static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger() ;
    Document _doc { get ; set ; }
    UIDocument _uiDoc { get ; set ; }

    public CmdCreateKouhan( UIDocument uiDoc )
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
        //FrmPutKoubanzai f = new FrmPutKoubanzai(_uiDoc.Application);
        //if (f.ShowDialog() != DialogResult.OK) { return false; }
        FrmPutKoubanzai f = Application.thisApp.frmPutKoubanzai ;

        Selection selection = _uiDoc.Selection ;
        double shortSize = 0, longSize = 0 ;
        ( shortSize, longSize ) = GetPlateSize( f.CmbSize.Text ) ;

        //面指定
        if ( f.RbtFace.Checked ) {
          //配置基準面選択
          Reference refer = selection.PickObject( ObjectType.Face, "配置基準とする面を選択してください)" ) ;
          Element element = _doc.GetElement( refer ) ;
          Face face = element.GetGeometryObjectFromReference( refer ) as Face ;
          face.ComputeNormal( UV.Zero ) ;
          if ( face == null ) {
            return false ;
          }

          using ( Transaction tr = new Transaction( _doc ) ) {
            tr.Start( "Fukkouban placement" ) ;
            XYZ origin, hukuinPnt, vecHukuin, kyoutyouPnt, orthogolanVec ;

            ElementId pointId = ElementId.InvalidElementId, LineId = ElementId.InvalidElementId ;
            double offset = (double) f.NmcOffset.Value ;
            try {
              //配置基準＋幅員、橋長方向取得
              origin = selection.PickPoint( ObjectSnapTypes.Nearest, "挿入の基点を指定" ) ;
              pointId = GantryUtil.InsertPointFamily( _doc, origin, refer, true ) ;
              _doc.Regenerate() ;

              //幅員方向ベクトル
              hukuinPnt = selection.PickPoint( ObjectSnapTypes.Nearest, "長辺方向を指定" ) ;
              LineId = GantryUtil.InsertLineFamily( _doc, origin, hukuinPnt, refer, true ) ;
              _doc.Regenerate() ;

              vecHukuin = ( hukuinPnt - origin ).Normalize() ;
              //橋軸方向ベクトル
              kyoutyouPnt = selection.PickPoint( ObjectSnapTypes.Nearest, "短辺方向を指定" ) ;
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

              XYZ vecKyoutyou = ( kyoutyouPnt - origin ).Normalize() ;
              if ( RevitUtil.ClsGeo.GEO_GE0( orthogolanVec.DotProduct( vecKyoutyou ) ) ) {
                vecKyoutyou = orthogolanVec.Normalize() ;
              }
              else {
                vecKyoutyou = orthogolanVec.Negate().Normalize() ;
              }

              bool isLeft = RevitUtil.ClsGeo.IsLeft( vecHukuin, vecKyoutyou ) ;

              double adjustAngle = XYZ.BasisX.AngleTo( vecHukuin ) *
                                   ( RevitUtil.ClsGeo.GEO_LT0( vecHukuin.Y ) ? -1 : 1 ) ;
              int hukuinCnt = (int) f.NmcLongCnt.Value ;
              int kyoutyouCnt = (int) f.NmcShortCnt.Value ;
              List<XYZ> plateList = calcKouhanPlacement( origin, vecHukuin, vecKyoutyou, hukuinCnt, kyoutyouCnt,
                shortSize, longSize ) ;
              XYZ adVec = ( ! isLeft )
                ? vecKyoutyou.Normalize() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( shortSize )
                : XYZ.Zero ;
              foreach ( XYZ pnt in plateList ) {
                ElementId idnew = ElementId.InvalidElementId ;
                if ( f.RbtShimakouhan.Checked ) {
                  idnew = Shimakouhan.CreateShimakouhanOnFace( _doc, f.CmbSize.Text, f.CmbThick.Text, pnt, adjustAngle,
                    refer, vecHukuin, adVec ) ;
                  Shimakouhan s = new Shimakouhan( idnew, f.CmbKoudaiName.Text, f.CmbSize.Text ) ;
                  Shimakouhan.WriteToElement( s, _doc ) ;
                }
                else {
                  idnew = Shikiteppan.CreateShikiTeppanOnFace( _doc, f.CmbSize.Text, f.CmbThick.Text, pnt, adjustAngle,
                    refer, vecHukuin, adVec ) ;
                  Shikiteppan s = new Shikiteppan( idnew, f.CmbKoudaiName.Text, f.CmbSize.Text ) ;
                  Shikiteppan.WriteToElement( s, _doc ) ;
                }
              }

              tr.Commit() ;
            }
            catch ( Autodesk.Revit.Exceptions.OperationCanceledException ) {
              return false ;
            }
            catch ( Exception ex ) {
              MessageUtil.Error( "鋼板材配置に失敗しました", "鋼板材個別配置" ) ;
              _logger.Error( ex.Message ) ;
            }
          }
        }
        else {
          //基準レベル
          Level baselevel = null ;
          double offset = (double) f.NmcOffset.Value ;
          if ( f.CmbLevel.Text == "部材選択" ) {
            ( baselevel, offset ) = GantryUtil.GetLevelAndOffsetWithSelect( _uiDoc ) ;
            offset += (double) f.NmcOffset.Value ;
            if ( baselevel == null ) {
              MessageUtil.Warning( "指定した部材のホストに設定されたレベルが取得できません\r\nホストがレベルの部材を選択してください", "鋼板材配置" ) ;
              return false ;
            }
          }
          else {
            baselevel = RevitUtil.ClsRevitUtil.GetLevel( _doc, f.CmbLevel.Text ) as Level ;
          }

          XYZ origin, hukuinPnt, vecHukuin, kyoutyouPnt, orthogolanVec ;

          ElementId pointId = ElementId.InvalidElementId, LineId = ElementId.InvalidElementId ;

          try {
            //配置基準＋幅員、橋長方向取得

            origin = selection.PickPoint( ObjectSnapTypes.Nearest, "挿入の基点を指定" ) ;
            pointId = GantryUtil.InsertPointFamily( _doc, origin, baselevel ) ;

            //幅員方向ベクトル
            hukuinPnt = selection.PickPoint( ObjectSnapTypes.Nearest, "長辺方向を指定" ) ;
            vecHukuin = ( hukuinPnt - origin ).Normalize() ;
            LineId = GantryUtil.InsertLineFamily( _doc, origin, hukuinPnt, baselevel ) ;

            //橋軸方向ベクトル
            kyoutyouPnt = selection.PickPoint( ObjectSnapTypes.Nearest, "短辺方向を指定" ) ;
            orthogolanVec = vecHukuin.CrossProduct( XYZ.BasisZ ) ;

            using ( Transaction tr = new Transaction( _doc ) ) {
              tr.Start( "Kouhan Way" ) ;
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

              XYZ vecKyoutyou = ( kyoutyouPnt - origin ).Normalize() ;
              if ( RevitUtil.ClsGeo.GEO_GE0( orthogolanVec.DotProduct( vecKyoutyou ) ) ) {
                vecKyoutyou = orthogolanVec.Normalize() ;
              }
              else {
                vecKyoutyou = orthogolanVec.Negate().Normalize() ;
              }

              bool isLeft = RevitUtil.ClsGeo.IsLeft( vecHukuin, vecKyoutyou ) ;

              double adjustAngle = XYZ.BasisX.AngleTo( vecHukuin ) *
                                   ( RevitUtil.ClsGeo.GEO_LT0( vecHukuin.Y ) ? -1 : 1 ) ;
              int hukuinCnt = (int) f.NmcLongCnt.Value ;
              int kyoutyouCnt = (int) f.NmcShortCnt.Value ;
              List<XYZ> plateList = calcKouhanPlacement( origin, vecHukuin, vecKyoutyou, hukuinCnt, kyoutyouCnt,
                shortSize, longSize ) ;
              XYZ adVec = ( ! isLeft )
                ? vecKyoutyou.Normalize() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( shortSize )
                : XYZ.Zero ;
              foreach ( XYZ pnt in plateList ) {
                ElementId idnew = ElementId.InvalidElementId ;

                if ( f.RbtShimakouhan.Checked ) {
                  idnew = Shikiteppan.CreateShikiTeppan( _doc, f.CmbSize.Text, f.CmbThick.Text, pnt, adjustAngle, adVec,
                    baselevel, offset ) ;
                  Shikiteppan s = new Shikiteppan( idnew, f.CmbKoudaiName.Text, f.CmbSize.Text ) ;
                  Shikiteppan.WriteToElement( s, _doc ) ;
                }
                else {
                  idnew = Shimakouhan.CreateShimakouhan( _doc, f.CmbSize.Text, f.CmbThick.Text, pnt, adjustAngle, adVec,
                    baselevel, offset ) ;
                  Shimakouhan s = new Shimakouhan( idnew, f.CmbKoudaiName.Text, f.CmbSize.Text ) ;
                  Shimakouhan.WriteToElement( s, _doc ) ;
                }
              }

              tr.Commit() ;
            }
          }
          catch ( Autodesk.Revit.Exceptions.OperationCanceledException ) {
            return false ;
          }
        }
      }
      catch ( Exception ex ) {
        _logger.Error( ex.Message ) ;
        return false ;
      }

      return true ;
    }

    /// <summary>
    /// 配置枚数に応じて配置位置を返す
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="vecLong"></param>
    /// <param name="vecShort"></param>
    /// <param name="longCnt"></param>
    /// <param name="shortCnt"></param>
    /// <param name="shortSize"></param>
    /// <param name="longSize"></param>
    /// <returns></returns>
    private List<XYZ> calcKouhanPlacement( XYZ origin, XYZ vecLong, XYZ vecShort, int longCnt, int shortCnt,
      double shortSize, double longSize )
    {
      List<XYZ> retList = new List<XYZ>() ;
      XYZ baseP = origin ;
      double longPitch = RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( longSize ) ;
      for ( int kC = 0 ; kC < shortCnt ; kC++ ) {
        baseP = origin + vecShort.Normalize() * ( RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( kC * shortSize ) ) ;
        XYZ newP = baseP ;
        for ( int hC = 0 ; hC < longCnt ; hC++ ) {
          newP = ( hC == 0 ) ? newP : newP + vecLong.Normalize() * longPitch ;
          retList.Add( newP ) ;
        }
      }

      return retList ;
    }

    /// <summary>
    /// サイズ毎の短辺、長辺サイズを返す
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    private (double, double) GetPlateSize( string size )
    {
      if ( size.Contains( "3x6" ) ) {
        return ( 914, 1829 ) ;
      }
      else if ( size.Contains( "4x8" ) ) {
        return ( 1219, 2438 ) ;
      }
      else if ( size.Contains( "5x10" ) ) {
        return ( 1524, 3048 ) ;
      }
      else if ( size.Contains( "5x20" ) ) {
        return ( 1524, 6096 ) ;
      }
      else {
        return ( 0, 0 ) ;
      }
    }
  }
}