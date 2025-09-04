using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using Autodesk.Revit.UI.Selection ;
using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;
using YMS_gantry.Material ;
using YMS_gantry.UI ;

namespace YMS_gantry.Command
{
  class CmdCreateFukkougeta
  {
    static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger() ;
    Document _doc { get ; set ; }
    UIDocument _uiDoc { get ; set ; }

    public CmdCreateFukkougeta( UIDocument uiDoc )
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
      Selection selection = _uiDoc.Selection ;
      try {
        //FrmPutFukkougeta frm = new FrmPutFukkougeta(_uiDoc.Application);
        //if(frm.ShowDialog()!=System.Windows.Forms.DialogResult.OK)
        //{
        //    return false;
        //}
        FrmPutFukkougeta frm = Application.thisApp.frmPutFukkougeta ;

        //部材情報
        FukkouGeta fg = new FukkouGeta() ;
        fg.m_KodaiName = frm.CmbKoudaiName.Text ;
        fg.m_Material = frm.CmbMaterial.Text ;
        fg.m_Size = frm.CmbSize.Text ;
        double rotate = 0 ;
        if ( frm.CmbSizeType.Text == Master.ClsFukkouGetaCsv.TypeC ) {
          string cRbt = (string) frm.panel1.Controls.OfType<RadioButton>().SingleOrDefault( rb => rb.Checked == true )
            .Tag ;
          rotate = RevitUtil.ClsCommonUtils.ChangeStrToDbl( cRbt ) ;
        }

        //面指定
        if ( frm.RbtFace.Checked ) {
          //配置基準面選択
          Reference refer = selection.PickObject( ObjectType.Face, "配置基準とする面を選択してください)" ) ;
          Element element = _doc.GetElement( refer ) ;
          Face face = element.GetGeometryObjectFromReference( refer ) as Face ;
          if ( face == null ) {
            return false ;
          }

          using ( Transaction tr = new Transaction( _doc ) ) {
            tr.Start( "Fukkouban placement" ) ;
            double offset = (double) frm.NmcOffset.Value ;
            try {
              //2点指定
              XYZ p1 = selection.PickPoint( "1点目を指定してください" ) ;
              XYZ p2 = selection.PickPoint( "2点目を指定してください" ) ;
              ElementId newId = FukkouGeta.CreateOnFace( _doc, fg, p1, p2, refer, rotate, frm.CmbKoudaiName.Text ) ;
            }
            catch ( Autodesk.Revit.Exceptions.OperationCanceledException ) {
              return false ;
            }

            tr.Commit() ;
          }
        }
        //自由位置
        else {
          FamilySymbol sym ;
          string familyPath = Master.ClsFukkouGetaCsv.GetFamilyPath( fg.m_Size ) ;
          Level level = ClsRevitUtil.GetLevel( _doc, frm.CmbLevel.Text ) as Level ;
          double offset = (double) frm.NmcOffset.Value ;
          if ( ! GantryUtil.GetFamilySymbol( _doc, familyPath, FukkouGeta.typeName, out sym, true ) ) {
            return false ;
          }

          ElementId newId = ElementId.InvalidElementId ;

          using ( Transaction tr = new Transaction( _doc ) ) {
            tr.Start( "Placement Fukkougeta" ) ;
            try {
              Dictionary<string, string> paramList = new Dictionary<string, string>() ;
              paramList.Add( DefineUtil.PARAM_MATERIAL, fg.m_Material ) ;
              //paramList.Add("サイズ", fg.m_Size);
              paramList.Add( DefineUtil.PARAM_ROTATE, $"{rotate / ( 180 / Math.PI )}" ) ;
              sym = GantryUtil.DuplicateTypeWithNameRule( _doc, frm.CmbKoudaiName.Text, sym, FukkouGeta.typeName ) ;
              //sym = GantryUtil.ChangeTypeID(_doc, sym, GantryUtil.CreateTypeName(FukkouGeta.typeName, paramList));
              //タイプパラメータ設定
              foreach ( KeyValuePair<string, string> kv in paramList ) {
                GantryUtil.SetParameterValueByParameterName( sym, kv.Key, kv.Value ) ;
              }

              if ( frm.CmbLevel.Text == "部材選択" ) {
                newId = GantryUtil.PlaceSymbolOverTheSelectedElm( _uiDoc, sym, frm.CmbKoudaiName.Text, level, offset,
                  functionName: "覆工桁配置", paramList: paramList ) ;
              }
              else {
                XYZ p1 = selection.PickPoint( "1点目を指定してください" ) ;
                XYZ p2 = selection.PickPoint( "2点目を指定してください" ) ;
                p1 = new XYZ( p1.X, p1.Y, level.Elevation ) ;
                p2 = new XYZ( p2.X, p2.Y, level.Elevation ) ;
                newId = MaterialSuper.PlaceWithTwoPoints( sym, level.GetPlaneReference(), p1, p2,
                  RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( offset ) ) ;
              }

              if ( newId != ElementId.InvalidElementId || newId != null ) {
                fg.m_ElementId = newId ;
                FukkouGeta.WriteToElement( fg, _doc ) ;
              }
            }
            catch ( Autodesk.Revit.Exceptions.OperationCanceledException ) {
            }

            tr.Commit() ;
          }
        }

        return true ;
      }
      catch ( Exception ex ) {
        _logger.Error( ex.Message ) ;
        return false ;
      }
    }
  }
}