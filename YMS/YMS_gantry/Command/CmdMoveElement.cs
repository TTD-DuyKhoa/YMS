using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;
using YMS_gantry.UI ;

namespace YMS_gantry.Command
{
  class CmdMoveElement
  {
    static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger() ;

    private const string COMMAND_NAME = "位置変更" ;

    Document _doc { get ; set ; }
    UIDocument _uiDoc { get ; set ; }

    public CmdMoveElement( UIDocument uiDoc )
    {
      _uiDoc = uiDoc ;
      _doc = _uiDoc.Document ;
    }

    public Result Excute()
    {
      FrmEditLocationChange f = Application.thisApp.frmEditLocationChange ;
      try {
        using ( Transaction tr = new Transaction( _doc ) ) {
          tr.Start( "Move Element" ) ;

          MaterialSuper ms = MaterialSuper.ReadFromElement( f._targetElementId, _uiDoc.Document ) ;
          XYZ movedPosition = ms.Position ;
          XYZ moveDir = ( movedPosition - f._originalLocation ) ;
          if ( ! RevitUtil.ClsGeo.GEO_EQ0( f._originalLocation.DistanceTo( movedPosition ) ) ) {
            foreach ( DataGridViewRow row in f.DgvEditLocationChangeList.Rows ) {
              ElementId id = new ElementId( (int) row.Cells[ "ColElementID" ].Value ) ;
              if ( id == ElementId.InvalidElementId ) {
                continue ;
              }

              if ( (bool) row.Cells[ "ColLocationChangeListSelected" ].Value ) {
                ElementTransformUtils.MoveElement( _uiDoc.Document, id, moveDir ) ;
              }
            }
          }

          tr.Commit() ;
        }
      }
      catch ( Autodesk.Revit.Exceptions.OperationCanceledException ) {
        return Result.Cancelled ;
      }
      catch ( Exception ex ) {
        MessageUtil.Error( $"位置変更に失敗しました。", COMMAND_NAME, f ) ;
        _logger.Error( ex.Message ) ;
        return Result.Failed ;
      }

      // 長さ変更データグリッドビューのレコードを更新
      f.Search() ;
      //f.Activate();

      return Result.Succeeded ;
    }
  }
}