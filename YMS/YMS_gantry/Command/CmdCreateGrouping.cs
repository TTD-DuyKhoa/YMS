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
using YMS_gantry.Data ;
using YMS_gantry.Material ;
using YMS_gantry.UI ;

namespace YMS_gantry.Command
{
  class CmdCreateGrouping
  {
    static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger() ;
    Document _doc { get ; set ; }
    UIDocument _uiDoc { get ; set ; }

    public CmdCreateGrouping( UIDocument uiDoc )
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
        FrmGrouping f = Application.thisApp.frmGrouping ;

        FrmGrouping.GroupingId gid = f.groupingId ;
        MaterialSuper[] unionMaterials = MaterialSuper.Collect( _doc ) ;
        List<ElementId> elementIds = new List<ElementId>() ;
        ModelData md = f.GetEditModelData() ;
        switch ( gid ) {
          case FrmGrouping.GroupingId.Cancel :
            f.Close() ;
            break ;
          case FrmGrouping.GroupingId.OK :

            //ファミリに構台情報を付与
            using ( Transaction transaction = new Transaction( _doc ) ) {
              transaction.Start( "SetKoudaiName" ) ;
              foreach ( KoudaiData kd in md.ListKoudaiData ) {
                SetKoudaiName( kd.GetListWariateElementId(), kd.AllKoudaiFlatData.KoudaiName ) ;
              }

              transaction.Commit() ;
            }

            //外形線を作図
            OutLine outLine = new OutLine() ;
            outLine.m_Document = _doc ;
            for ( int i = 0 ; i < md.ListGroupData.Count() ; i++ ) {
              GroupData gd = md.ListGroupData[ i ] ;
              if ( gd.FlgLineView ) {
                outLine.DeleteOutLine( ref gd ) ;
                List<ElementId> lstLineId = outLine.CreateOutLine( md, gd.GroupName ) ;
                gd.SetListLineElementId( lstLineId ) ;
              }
              else {
                outLine.DeleteOutLine( ref gd ) ;
              }
            }

            using ( Transaction transaction = new Transaction( _doc ) ) {
              transaction.Start( "grouping" ) ;
              ModelData.SetModelData( _doc, md ) ;
              transaction.Commit() ;
            }

            f.SetEditModelData( md ) ;
            f.SetDataGridViewGroup() ;
            f.SetDataGridViewBuzai() ;

            break ;
          case FrmGrouping.GroupingId.GroupView :
            List<string> lstKoudaiName = f.selectGroupData.ListKoudaiName ;
            foreach ( string koudaiName in lstKoudaiName ) {
              List<ElementId> ids =
                ( from data in unionMaterials where data.m_KodaiName == koudaiName select data.m_ElementId ).ToList() ;
              elementIds.AddRange( ids ) ;
              List<ElementId> ids2 = md.GetKoudaiData( koudaiName ).GetListWariateElementId() ;
              elementIds.AddRange( ids ) ;
            }

            foreach ( int hId in f.selectGroupData.ListHunchId ) {
              elementIds.Add( new ElementId( hId ) ) ;
            }

            RevitUtil.ClsRevitUtil.SelectElement( _uiDoc, elementIds ) ;
            break ;
          case FrmGrouping.GroupingId.Delete :
            outLine = new OutLine() ;
            outLine.m_Document = _doc ;
            GroupData gd2 = f.selectGroupData ;
            outLine.DeleteOutLine( ref gd2 ) ;
            break ;
          case FrmGrouping.GroupingId.KoudaiView :
            List<ElementId> lstId = ( from data in unionMaterials
              where data.m_KodaiName == f.selectKoudaiName
              select data.m_ElementId ).ToList() ;
            List<ElementId> lstId2 = f.GetEditModelData().GetKoudaiData( f.selectKoudaiName )
              .GetListWariateElementId() ;
            lstId.AddRange( lstId2 ) ;
            RevitUtil.ClsRevitUtil.SelectElement( _uiDoc, lstId ) ;
            break ;
          case FrmGrouping.GroupingId.BuzaiView :
            ElementId id = f.selectBuzaiId ;
            RevitUtil.ClsRevitUtil.SelectElement( _uiDoc, id ) ;
            break ;
          case FrmGrouping.GroupingId.HunchSelect :
            if ( RevitUtil.ClsRevitUtil.PickObjectsPartSymbol( _uiDoc, "ハンチを選択してください。", ref elementIds ) ) {
              List<ElementId> lstHId = new List<ElementId>() ;
              foreach ( ElementId idH in elementIds ) {
                var familyName = GetFamilyName( _uiDoc.Document.GetElement( idH ) ) ;
                var typeName = GetTypeName( _uiDoc.Document.GetElement( idH ) ) ;
                var familySymbol = ClsRevitUtil.GetFamilySymbol( _uiDoc.Document, familyName, typeName ) ;
                string shoubunrui = GetTypeParameter( familySymbol, "小分類" ) ;
                if ( shoubunrui == "ﾊﾝﾁﾃﾞｯｷ" )
                  lstHId.Add( idH ) ;
              }

              f.AddHunchId( lstHId ) ;
            }

            break ;
        }

        return true ;
      }
      catch ( Exception ex ) {
        _logger.Error( ex.Message ) ;
        return false ;
      }
    }


    /// <summary>
    /// 構台名称を登録
    /// </summary>
    private bool SetKoudaiName( List<ElementId> lstElementId, string koudaiName )
    {
      try {
        foreach ( ElementId id in lstElementId ) {
          //2024/03/15 tsunoda mod ハンチと添杭はマテリアルクラスに入れる
          FamilyInstance ins = _doc.GetElement( id ) as FamilyInstance ;
          if ( ins != null ) {
            if ( ins.Symbol.FamilyName.Contains( "ﾊﾝﾁﾃﾞｯｷ" ) ) {
              HaunchDeck hd = new HaunchDeck() ;
              hd.m_ElementId = id ;
              hd.m_KodaiName = koudaiName ;
              HaunchDeck.WriteToElement( hd, _doc ) ;
              continue ;
            }
          }

          Dictionary<string, string> paramList = new Dictionary<string, string>() ;
          paramList.Add( DefineUtil.PARAM_KOUDAI_NAME, koudaiName ) ;
          var lineString = GantryUtil.CombineDictionaryToString( paramList ) ;
          RevitUtil.ClsRevitUtil.SetMojiParameter( _doc, id, DefineUtil.PARAM_MATERIAL_DETAIL, lineString ) ;
        }
      }
      catch {
        return false ;
      }

      return true ;
    }


    private string GetFamilyName( Element e )
    {
      var eId = e?.GetTypeId() ;
      if ( eId == null )
        return "" ;
      var elementType = e.Document.GetElement( eId ) as ElementType ;
      return elementType?.FamilyName ?? "" ;
    }

    private string GetTypeName( Element e )
    {
      var eId = e?.GetTypeId() ;
      if ( eId == null )
        return "" ;
      var elementType = e.Document.GetElement( eId ) as ElementType ;
      return elementType?.Name ?? "" ;
    }

    private string GetTypeParameter( FamilySymbol sym, string paramName )
    {
      try {
        Parameter parm = sym.LookupParameter( paramName ) ;
        if ( parm == null ) {
          return string.Empty ;
        }

        return parm.AsValueString() ;
      }
      catch {
        return string.Empty ;
      }
    }
  }
}