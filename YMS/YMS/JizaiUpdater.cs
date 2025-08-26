using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using YMS.Parts;

namespace YMS
{
    class Updater01 : IUpdater
    {
        // ----------------------------------------------------------------------------------------
        // | Updater Id |
        // ----------------------------------------------------------------------------------------
        private const string Id = "B7FA01EA-F749-473F-AD7A-4A35A44802B3";
        
        private bool m_actionFlg = false;
        public bool m_isleft { get; set; }

        public DLG.DlgKobetsuHaichi_Jizai.EnumPtn m_ptn { get; set; }

        // ----------------------------------------------------------------------------------------
        // | Constructor |
        // ----------------------------------------------------------------------------------------
        public UpdaterId updater { get; set; }
        public Updater01(Document doc, AddInId addInId, DLG.DlgKobetsuHaichi_Jizai.EnumPtn ptn,bool actFlg,bool isleft = true)
        {

            m_isleft = isleft;
            m_ptn = ptn;
            m_actionFlg = actFlg;

            updater = new UpdaterId(addInId, new Guid(Id));
            RegisterUpdater(doc);
            RegisterTriggers();
        }

        // ----------------------------------------------------------------------------------------
        // | Register Updater |
        // ----------------------------------------------------------------------------------------
        public void RegisterUpdater(Document doc)
        {
            if (UpdaterRegistry.IsUpdaterRegistered(this.updater, doc))
            {
                UpdaterRegistry.RemoveAllTriggers(this.updater);
                UpdaterRegistry.UnregisterUpdater(this.updater, doc);
            }

            UpdaterRegistry.RegisterUpdater(this, doc);
        }

        // ----------------------------------------------------------------------------------------
        // | Register Trigger | <-- Contains Element Filter
        // ----------------------------------------------------------------------------------------
        public void RegisterTriggers()
        {
            if (this.updater != null && UpdaterRegistry.IsUpdaterRegistered(this.updater))
            {
                UpdaterRegistry.RemoveAllTriggers(this.updater);
                UpdaterRegistry.AddTrigger(this.updater,
                    new ElementClassFilter(typeof(FamilyInstance)), Element.GetChangeTypeElementAddition());
            }

        }

        // ----------------------------------------------------------------------------------------
        // | Execute Method | <-- Stuff to do
        // ----------------------------------------------------------------------------------------
        public void Execute(UpdaterData data)
        {
            
            try
            {
                if (!m_actionFlg)
                {
                    return;
                }

                Document doc = data.GetDocument();

                List<ElementId> elementIds = data.GetAddedElementIds().ToList();
                foreach (ElementId id in elementIds)
                {
                    var ins = doc.GetElement(id) as FamilyInstance;

                    if (ins != null)
                    {
                        FamilySymbol fs = ins.Symbol;


                        if (fs.FamilyName.Contains("自在火打ち受けピース_"))
                        {
                            if (!m_isleft && m_ptn == DLG.DlgKobetsuHaichi_Jizai.EnumPtn.typeC)
                            {
                                ins.flipHand();
                            }
                        }
                    }

                    //TaskDialog.Show("td", e.Name);
                }
            }
            catch(Exception e)
            {
                string mes = e.Message;
               // flg = false;
            }
            finally 
            {
                //flg = false;
            }
        }

        // ----------------------------------------------------------------------------------------
        // | Updater Members | <-- Extra members
        // ----------------------------------------------------------------------------------------
        public string GetAdditionalInformation()
        {
            return "NA";
        }

        public ChangePriority GetChangePriority()
        {
            return ChangePriority.MEPFixtures;
        }

        public UpdaterId GetUpdaterId()
        {
            return updater;
        }

        public string GetUpdaterName()
        {
            return "Updater01";
        }
    }

    //class Updater01 : IUpdater
    //{
    //    static AddInId m_appId;
    //    static UpdaterId m_updaterId;

    //    public Updater01(AddInId id)
    //    {
    //        m_appId = id;
    //        m_updaterId = new UpdaterId(m_appId, new Guid("1B94324D-0728-4819-82E3-22B194313DF6"));
    //    }

    //    public void Execute(UpdaterData data)
    //    {
    //        Document doc = data.GetDocument();



    //        // 変更された壁を全て取得
    //        var insSet = data.GetAddedElementIds()
    //            .Select(x => doc.GetElement(x) as FamilyInstance)
    //            .Where(x => x != null)
    //            .ToArray();

    //        List<ElementId> ids = new List<ElementId>();

    //        foreach (var inst in insSet)
    //        {
    //            //FamilyInstance inst = doc.GetElement(id) as FamilyInstance;
    //            if (inst.Name.Contains("自在火打ち受けピース"))
    //            {
    //                ids.Add(inst.Id);
    //            }
    //        }

    //        int sss = ids.Count();

    //        int ghh = 0;



    //        //var schema = Schema.Lookup(CmdElevationWatcher.SchemaId);
    //        //if (schema != null)
    //        //{
    //        //    // Updater の中で transaction を開くことは出来ない
    //        //    //using (var transaction = new Transaction(doc, Guid.NewGuid().ToString()))
    //        //    //{
    //        //    //    transaction.Start();
    //        //    foreach (var wall in wallSet)
    //        //    {
    //        //        try
    //        //        {
    //        //            // 壁オブジェクトの拡張ストレージに保存されている Id を取得
    //        //            var retrievedEntity = wall.GetEntity(schema);
    //        //            if (!retrievedEntity.IsValid()) { continue; }

    //        //            var tiedCircleId = retrievedEntity.Get<ElementId>(CmdElevationWatcher.FieldName);
    //        //            var modelLine = doc.GetElement(tiedCircleId) as ModelLine;
    //        //            if (modelLine == null) { continue; }

    //        //            var (plane, line) = CmdElevationWatcher.CreateLine(wall);
    //        //            modelLine.SetGeometryCurve(line, false);
    //        //        }
    //        //        catch (Exception ex)
    //        //        {
    //        //            var a = ex.Message;
    //        //        }
    //        //    }
    //    //}
    //        //    transaction.Commit();
    //        //}
    //    }

    //    public string GetAdditionalInformation()
    //    {
    //        return "Jizai updater example: ";
    //    }

    //    public ChangePriority GetChangePriority()
    //    {
    //        return ChangePriority.FloorsRoofsStructuralWalls;
    //    }

    //    public UpdaterId GetUpdaterId()
    //    {
    //        return m_updaterId;
    //    }

    //    public string GetUpdaterName()
    //    {
    //        return "Jizai  Updater";
    //    }

    //}
}
