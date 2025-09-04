using Autodesk.Revit.DB ;
using Autodesk.Revit.DB.ExtensibleStorage ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;

namespace YMS
{
  class WallUpdater : IUpdater
  {
    static AddInId m_appId ;
    static UpdaterId m_updaterId ;
    WallType m_wallType = null ;

    // constructor takes the AddInId for the add-in associated with this updater
    public WallUpdater( AddInId id )
    {
      m_appId = id ;
      m_updaterId = new UpdaterId( m_appId, new Guid( "FBFBF6B2-4C06-42d4-97C1-D1B4EB593EFF" ) ) ;
    }

    public void Execute( UpdaterData data )
    {
      Document doc = data.GetDocument() ;

      // 変更された壁を全て取得
      var wallSet = data.GetModifiedElementIds().Select( x => doc.GetElement( x ) as Wall ).Where( x => x != null )
        .ToArray() ;

      var schema = Schema.Lookup( CmdElevationWatcher.SchemaId ) ;
      if ( schema != null ) {
        // Updater の中で transaction を開くことは出来ない
        //using (var transaction = new Transaction(doc, Guid.NewGuid().ToString()))
        //{
        //    transaction.Start();
        foreach ( var wall in wallSet ) {
          try {
            // 壁オブジェクトの拡張ストレージに保存されている Id を取得
            var retrievedEntity = wall.GetEntity( schema ) ;
            if ( ! retrievedEntity.IsValid() ) {
              continue ;
            }

            var tiedCircleId = retrievedEntity.Get<ElementId>( CmdElevationWatcher.FieldName ) ;
            var modelLine = doc.GetElement( tiedCircleId ) as ModelLine ;
            if ( modelLine == null ) {
              continue ;
            }

            var (plane, line) = CmdElevationWatcher.CreateLine( wall ) ;
            modelLine.SetGeometryCurve( line, false ) ;
          }
          catch ( Exception ex ) {
            var a = ex.Message ;
          }
        }
      }
      //    transaction.Commit();
      //}
    }

    public string GetAdditionalInformation()
    {
      return "Wall type updater example: updates all newly created walls to a special wall" ;
    }

    public ChangePriority GetChangePriority()
    {
      return ChangePriority.FloorsRoofsStructuralWalls ;
    }

    public UpdaterId GetUpdaterId()
    {
      return m_updaterId ;
    }

    public string GetUpdaterName()
    {
      return "Wall Type Updater" ;
    }
  }
}