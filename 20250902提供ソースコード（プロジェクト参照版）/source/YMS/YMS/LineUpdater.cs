using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMS
{
    class LineUpdater : IUpdater
    {
        static AddInId m_appId;
        static UpdaterId m_updaterId;
        WallType m_wallType = null;

        // constructor takes the AddInId for the add-in associated with this updater
        public LineUpdater(AddInId id)
        {
            m_appId = id;
            m_updaterId = new UpdaterId(m_appId, new Guid("FBFBF6B2-4C06-42d4-97C1-D1B4EB593EFE"));//Guid設定値不明
        }

        public void Execute(UpdaterData data)
        {
            Document doc = data.GetDocument();

            // 変更された壁を全て取得
            var lineSet = data.GetModifiedElementIds()
                .Select(x => doc.GetElement(x) as ModelLine)
                .Where(x => x != null)
                .ToArray();

            var schema = Schema.Lookup(CmdElevationWatcher.SchemaId);
            if (schema != null)
            {
                // Updater の中で transaction を開くことは出来ない
                //using (var transaction = new Transaction(doc, Guid.NewGuid().ToString()))
                //{
                //    transaction.Start();
                foreach (var line in lineSet)
                {
                    try
                    {
                        // MLオブジェクトの拡張ストレージに保存されている Id を取得
                        var retrievedEntity = line.GetEntity(schema);
                        if (!retrievedEntity.IsValid()) { continue; }

                        var tiedCircleId = retrievedEntity.Get<ElementId>(CmdElevationWatcher.FieldName);
                        var wall = doc.GetElement(tiedCircleId) as Wall;
                        if (wall == null) { continue; }

                        var (plane, p) = CmdElevationWatcher.CreateWall(line);
                        //wall.SetGeometryCurve(cv, false);
                        //wall.SetEntity(cv);
                        var wallLocation = wall.Location as LocationCurve;
                        var start = wallLocation.Curve.GetEndPoint(0);
                        var end = wallLocation.Curve.GetEndPoint(1);
                        var mid = 0.5 * (end + start);

                        wallLocation.Move(p - mid);//壁の元あった場所から線移動分を引かないといけない
                    }
                    catch (Exception ex)
                    {
                        var a = ex.Message;
                    }
                }
            }
            //    transaction.Commit();
            //}
        }

        public string GetAdditionalInformation()
        {
            return "Wall type updater example: updates all newly created walls to a special wall";
        }

        public ChangePriority GetChangePriority()
        {
            return ChangePriority.FloorsRoofsStructuralWalls;
        }

        public UpdaterId GetUpdaterId()
        {
            return m_updaterId;
        }

        public string GetUpdaterName()
        {
            return "Wall Type Updater";
        }
    }
}
