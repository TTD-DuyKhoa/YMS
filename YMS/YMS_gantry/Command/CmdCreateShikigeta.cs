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

namespace YMS_gantry
{
  class CmdCreateShikigeta
  {
    static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger() ;
    Document _doc { get ; set ; }
    UIDocument _uiDoc { get ; set ; }

    public CmdCreateShikigeta( UIDocument uiDoc )
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
        //FrmPutShikigeta frm = new FrmPutShikigeta(_uiDoc.Application);
        //if (frm.ShowDialog() != DialogResult.OK) { return false; }
        FrmPutShikigeta frm = Application.thisApp.frmPutShikigeta ;

        //配置に必要な情報
        ShikigetaData sData = new ShikigetaData() ;
        sData.ShikigetaSize = frm.CmbSize.Text ;
        sData.ShikigetaMaterial = frm.CmbMaterial.Text ;
        sData.exShikigetaEndLeng = (double) frm.NmcELng.Value ;
        sData.exShikigetaStartLeng = (double) frm.NmcSLng.Value ;

        Level level = ClsRevitUtil.GetLevel( _doc, frm.CmbLevel.Text ) as Level ;
        double offset = (double) frm.NmcOffset.Value ;

        //配置
        using ( Transaction trans = new Transaction( _doc ) ) {
          trans.Start( "Shikigeta Placement" ) ;
          //if (frm.RbtAuto.Checked)
          //{

          //}
          //else
          //{
          FamilySymbol sym ;
          string familyPath = Master.ClsMasterCsv.GetFamilyPath( sData.ShikigetaSize ) ;
          if ( ! GantryUtil.GetFamilySymbol( _doc, familyPath, Shikigeta.shikiGetaName, out sym, true ) ) {
            return false ;
          }

          ElementId newId = ElementId.InvalidElementId ;
          Dictionary<string, string> paramList = new Dictionary<string, string>() ;
          paramList.Add( DefineUtil.PARAM_MATERIAL, sData.ShikigetaMaterial ) ;
          sym = GantryUtil.DuplicateTypeWithNameRule( _doc, frm.CmbKoudaiName.Text, sym, Shikigeta.shikiGetaName ) ;
          //sym = GantryUtil.ChangeTypeID(_doc, sym, GantryUtil.CreateTypeName(Shikigeta.shikiGetaName, paramList));
          //タイプパラメータ設定
          foreach ( KeyValuePair<string, string> kv in paramList ) {
            GantryUtil.SetParameterValueByParameterName( sym, kv.Key, kv.Value ) ;
          }

          if ( frm.CmbLevel.Text == "部材選択" ) {
            newId = GantryUtil.PlaceSymbolOverTheSelectedElm( _uiDoc, sym, frm.CmbKoudaiName.Text, level, offset,
              sData.exShikigetaStartLeng, sData.exShikigetaEndLeng ) ;
          }
          else {
            XYZ p1 = _uiDoc.Selection.PickPoint( "1点目を指定してください" ) ;
            XYZ p2 = _uiDoc.Selection.PickPoint( "2点目を指定してください" ) ;
            p1 = new XYZ( p1.X, p1.Y, level.Elevation ) ;
            p2 = new XYZ( p2.X, p2.Y, level.Elevation ) ;
            XYZ vec = ( p2 - p1 ).Normalize() ;
            if ( frm.NmcSLng.Value > 0 ) {
              p1 = p1 + vec.Negate() * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( (double) frm.NmcSLng.Value ) ;
            }

            if ( frm.NmcELng.Value > 0 ) {
              p2 = p2 + vec * RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( (double) frm.NmcELng.Value ) ;
            }

            newId = MaterialSuper.PlaceWithTwoPoints( sym, level.GetPlaneReference(), p1, p2,
              RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( offset ) ) ;
          }

          if ( newId != ElementId.InvalidElementId || newId != null ) {
            Shikigeta dNd = new Shikigeta( newId, frm.CmbKoudaiName.Text, sData ) ;
            Shikigeta.WriteToElement( dNd, _doc ) ;
          }

          trans.Commit() ;
          //    }
        }
      }
      catch ( Exception ex ) {
        _logger.Error( ex.Message ) ;
        return false ;
      }

      return true ;
    }
  }
}