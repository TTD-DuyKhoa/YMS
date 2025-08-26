using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using YMS_gantry.Data;
using YMS_gantry.Master;
using YMS_gantry.UI;
using YMS_gantry.UI.FrnCreateSlopeControls;
using static Autodesk.Revit.DB.SpecTypeId;
using static YMS_gantry.DefineUtil;
using static YMS_gantry.UI.FrmCreateSlopeViewModel;

namespace YMS_gantry
{
    public class CmdCreateSlope
    {
        static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        //static bool DEBUG_TEMP => false;

        //AllKoudaiFlatFrmData data { get; set; }

        Document _doc { get; set; }

        private static string StiffenerNameTableRawText => @"StiffenerType,OhbikiSize,PlateFamilyName,SymbolName
Plate,H200x200x8x12,ｽﾁﾌﾅｰPL_200S,ｽﾁﾌﾅｰPL_200S(構台)90°
Plate,20HA,ｽﾁﾌﾅｰPL_200S,ｽﾁﾌﾅｰPL_200S(構台)90°
Plate,H250x250x9x14,ｽﾁﾌﾅｰPL_250S,ｽﾁﾌﾅｰPL_250S(構台)90°
Plate,25HA,ｽﾁﾌﾅｰPL_250S,ｽﾁﾌﾅｰPL_250S(構台)90°
Plate,H300x300x10x15,ｽﾁﾌﾅｰPL_300S,ｽﾁﾌﾅｰPL_300S(構台)90°
Plate,30HA,ｽﾁﾌﾅｰPL_300S,ｽﾁﾌﾅｰPL_300S(構台)90°
Plate,H350x350x12x19,ｽﾁﾌﾅｰPL_350S,ｽﾁﾌﾅｰPL_350S(構台)90°
Plate,35HA,ｽﾁﾌﾅｰPL_350S,ｽﾁﾌﾅｰPL_350S(構台)90°
Plate,H400x400x13x21,ｽﾁﾌﾅｰPL_400S,ｽﾁﾌﾅｰPL_400S(構台)90°
Plate,40HA,ｽﾁﾌﾅｰPL_400S,ｽﾁﾌﾅｰPL_400S(構台)90°
Plate,50HA,ｽﾁﾌﾅｰPL_500S,ｽﾁﾌﾅｰPL_400S(構台)90°
Plate,H594x302x14x23,ｽﾁﾌﾅｰPL_594S,ｽﾁﾌﾅｰPL_594S(構台)90°
Plate,H700x300x13x24,ｽﾁﾌﾅｰPL_700S,ｽﾁﾌﾅｰPL_700S(構台)90°
Plate,H800x300x14x26,ｽﾁﾌﾅｰPL_800S,ｽﾁﾌﾅｰPL_800S(構台)90°
Plate,H900x300x16x28,ｽﾁﾌﾅｰPL_900S,ｽﾁﾌﾅｰPL_900S(構台)90°
Plate,C150x75x6.5x10,ｽﾁﾌﾅｰPL_C150x75x6.5x10用,ｽﾁﾌﾅｰPL_C150x75x6.5x10用(構台)90°
Plate,C200x80x7.5x11,ｽﾁﾌﾅｰPL_C200x80x7.5x11用,ｽﾁﾌﾅｰPL_C200x80x7.5x11用(構台)90°
Plate,C200x90x8x13.5,ｽﾁﾌﾅｰPL_C200x90x8x13.5用,ｽﾁﾌﾅｰPL_C200x90x8x13.5用(構台)90°
Plate,C250x90x9x13,ｽﾁﾌﾅｰPL_C250x90x9x13用,ｽﾁﾌﾅｰPL_C250x90x9x13用(構台)90°
Plate,C300x90x9x13,ｽﾁﾌﾅｰPL_C300x90x9x13用,ｽﾁﾌﾅｰPL_C300x90x9x13用(構台)90°
Plate,C300x90x10x15.5,ｽﾁﾌﾅｰPL_C300x90x10x15.5用,ｽﾁﾌﾅｰPL_C300x90x10x15.5用(構台)90°
Plate,C380x100x10.5x16,ｽﾁﾌﾅｰPL_C380x100x10.5x16用,ｽﾁﾌﾅｰPL_C380x100x10.5x16用(構台)90°
Plate,C380x100x13x20,ｽﾁﾌﾅｰPL_C380x100x13x20用,ｽﾁﾌﾅｰPL_C380x100x13x20用(構台)90°
Jack,H250x250,SH-30,30H(構台)
Jack,H300x300,SH-30,30H(構台)
Jack,H350x350,SH-40,40H(構台)
Jack,H400x400,SH-40,40H(構台)
JackDwj,H300x300,DWJ-30,30H(構台)
JackDwj,H350x350,DWJ-30,35H(構台)
JackDwj,H400x400,DWJ-40,40H(構台)
JackDwj,H500x500,DWJ-50,50H(構台)";

        class StiffenerPlateRec
        {
            public StiffenerType StiffenerType { get; set; }
            public string OhbikiSize { get; set; }
            public string FamilyName { get; set; }
            public string SymbolName { get; set; }
        }

        public static string[][] ReadArrayFrom(string csvContent)
        {
            var result = new List<string[]>();

            var lines = csvContent.Split(separator: new string[] { "\r\n", "\r", "\n" }, options: StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines.Skip(1))
            {
                var cells = line.Split(',');
                result.Add(cells);
            }

            return result.ToArray();
        }

        private static Lazy<StiffenerPlateRec[]> StiffenerTable { get; } = new Lazy<StiffenerPlateRec[]>(() =>
        {
            var textArray = ReadArrayFrom(StiffenerNameTableRawText);
            var result = new List<StiffenerPlateRec>();
            foreach (var line in textArray)
            {
                if (!Enum.TryParse<StiffenerType>(line.ElementAtOrDefault(0), out var type)) { continue; }
                result.Add(new StiffenerPlateRec
                {
                    StiffenerType = type,
                    OhbikiSize = line.ElementAtOrDefault(1),
                    FamilyName = line.ElementAtOrDefault(2),
                    SymbolName = string.IsNullOrWhiteSpace(line.ElementAtOrDefault(3)) ? line.ElementAtOrDefault(2) : line.ElementAtOrDefault(3),
                });
            }
            return result.ToArray();
        });

        static CmdCreateSlope()
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="doc"></param>
        public CmdCreateSlope(Document doc)
        {
            _doc = doc;
        }


        public bool Excute(UIDocument uiDoc)
        {
            Document doc = uiDoc.Document;
            using (var transaction = new Transaction(doc))
            {
#if DEBUG
#else
                try
#endif
                {
                    transaction.Start("CreateSlope");

                    var data = ModelData.GetModelData(doc);
                    foreach (var x in data.ListKoudaiData)
                    {
                        x.SlopeData = x.SlopeData ?? FrmCreateSlopeViewModel.InitWith(x.AllKoudaiFlatData);
                    }
                    // 変更前の情報
                    var originalData = FrmCreateSlopeViewModel.DeepClone(data);

                    KodaiSlopeModel targetKodai = null;
                    using (var dialog = new FrmCreateSlope(doc))
                    {
                        dialog.ModelData = data;
                        //OK以外は続行しない
                        if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                        {
                            return true;
                        }
                        targetKodai = dialog.ViewModel.SelectedKodai;
                    }
                    if (targetKodai == null) { return true; }
                    var originalSlopeData = originalData.ListKoudaiData.Select(x => x.SlopeData).ToArray();
                    var edittedSlopeData = data.ListKoudaiData.Select(x => x.SlopeData).ToArray();

                    // スロープ作成対象の構台を収集
                    var slopeInputData = edittedSlopeData.FirstOrDefault(x => x.Name == targetKodai.Name);
                    if (slopeInputData == null) { return true; }

                    // 変更が無ければ何もしない
                    var originalInputData = originalSlopeData.FirstOrDefault(x => x.Name == targetKodai.Name);
                    if (originalInputData?.Equals(slopeInputData) == true) { return true; }

                    // 全て乗り入れ部となっている構台が対象のとき
                    if (slopeInputData.StyleList.All(x => x.SlopeType == SlopeStyle.Entrance))
                    {
                        var taskDialog = new TaskDialog("スロープ向きの確認")
                        {
                            TitleAutoPrefix = false,
                            MainIcon = TaskDialogIcon.TaskDialogIconInformation,
                            MainInstruction = "全スパン通しのスロープが入力されました",
                            MainContent = "スロープの向きを選択してください:",
                            //ExtraCheckBoxText = "チェックボックス",
                            //ExpandedContent = "閉じているテキスト",
                            //FooterText = "フッター"
                        };
                        var n = slopeInputData.StyleList.Length;
                        taskDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, /*$"スパン1 : 上 → スパン{n} : 下",*/ $"スパン1 ↘ スパン{n}");
                        taskDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, /*$"スパン1 : 下 ← スパン{n} : 上",*/ $"スパン1 ↙ スパン{n}");

                        var result = taskDialog.Show();

                        var slopeDir = SlopeDirection.StartToEnd;
                        if (result == TaskDialogResult.CommandLink1)
                        {
                            slopeDir = SlopeDirection.StartToEnd;
                        }
                        else if (result == TaskDialogResult.CommandLink2)
                        {
                            slopeDir = SlopeDirection.EndToStart;
                        }
                        else
                        {
                            return true;
                        }
                        slopeInputData.Direction = slopeDir;
                    }

                    ModelData.SetModelData(doc, data);

                    var unionMaterials = MaterialSuper.Collect(doc);

                    //foreach (var slopeInputData in targetSlopeData)
                    {
                        var materials = unionMaterials.Where(x => x.m_KodaiName == slopeInputData.Name).ToArray();
                        var kodaiData = data.ListKoudaiData.FirstOrDefault(x => x.AllKoudaiFlatData.KoudaiName == slopeInputData.Name)?.AllKoudaiFlatData;

                        // 現状のスロープ角度情報
                        var originalSlopeInputData = originalData.ListKoudaiData
                            .FirstOrDefault(x => x.AllKoudaiFlatData.KoudaiName == slopeInputData.Name)
                            ?.SlopeData;
                        var originalAreaRangeSet = ConvertAreaRangeFromInputData(originalSlopeInputData, materials, kodaiData, inverse: true);

                        // 一度フラットに戻す
                        TransformSloping(doc, kodaiData, slopeInputData, originalAreaRangeSet, true);

                        var flatMaterials = MaterialSuper.Collect(doc).Where(x => x.m_KodaiName == slopeInputData.Name).ToArray();
                        // 変更後のスロープ角度情報
                        var areaRangeSet = ConvertAreaRangeFromInputData(slopeInputData, flatMaterials, kodaiData, inverse: false);

                        // スロープを設定
                        TransformSloping(doc, kodaiData, slopeInputData, areaRangeSet, false);
                    }
                    transaction.Commit();
                    return true;
                }
#if DEBUG
#else
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                    transaction.RollBack();
                    return false;
                }
#endif
            }
        }

        private static void TransformSloping(Document doc, AllKoudaiFlatFrmData kodaiData, KodaiSlopeModel slopeModel, AreaRange[] areaRangeSet, bool inverse)
        {
            var (origin, axisH, axisK, axisZ) = kodaiData.GetBasis(doc,true);
            var materials = MaterialSuper.Collect(doc).Where(x => x.m_KodaiName == kodaiData.KoudaiName).ToArray();

            // 大引の上下移動
            var ohbikiBottoms = areaRangeSet.SelectMany(x => x._OhbikiBottoms).GroupBy(x => x._K).ToArray();
            var sign = inverse ? -1.0 : 1.0;
            foreach (var ohbikiBottom in ohbikiBottoms)
            {
                var k = ohbikiBottom.Key;
                var ohbikiBottomMm = ohbikiBottom.OrderBy(x => x._Z).FirstOrDefault()?._Z ?? 0.0;
                var ohbikiBottomFeet = RevitUtil.ClsRevitUtil.CovertToAPI(ohbikiBottomMm);
                //var originalOhbikiBettomFeet = RevitUtil.ClsRevitUtil.CovertToAPI(originalOhbikiBottoms.FirstOrDefault(x => x.Key == k).OrderBy(x => x._Z).FirstOrDefault()?._Z ?? 0.0);
                var transform = sign * ohbikiBottomFeet * axisZ;

                var ohbikis= materials
                    .Select(x => x as OhbikiSuper)
                    .Where(x => x != null && x.m_K == k)
                    .ToArray();

                if (ohbikis.Any())
                { //
                    var ohbikiIds = ohbikis
               .Where(x => x != null && x.m_K == k)
               .Select(x => x.m_ElementId)
               .ToArray();
                    ElementTransformUtils.MoveElements(doc, ohbikiIds, transform);

                    //H鋼以外
                    if (ohbikis.FirstOrDefault().m_Size.StartsWith("[") || ohbikis.FirstOrDefault().m_Size.StartsWith("L"))
                    {
                        var arearange = areaRangeSet.Where(x =>x is AreaRangeSloped&& x._OhbikiBottoms.Where(y => y._K == k).Any());
                        if(arearange.Any())
                        {
                            var cArea = arearange.First();
                            var sO = cArea._OhbikiBottoms.OrderBy(x => x._K).First();
                            var sE = cArea._OhbikiBottoms.OrderBy(x => x._K).Last();
                            bool toB = sO._Z < sE._Z;
                            
                            var pileSize = GantryUtil.GetKouzaiSize(kodaiData.pilePillerData.PillarSize);
                            var ohbikiSize = GantryUtil.GetKouzaiSize(kodaiData.ohbikiData.OhbikiSize);
                            //B
                            var ohbikiB = ohbikis.Where(x => x != null && x.m_K == k&&x.m_AttachSide == "B").Select(x => x.m_ElementId).ToArray();
                            //F
                            var ohbikiF = ohbikis.Where(x => x != null && x.m_K == k&&x.m_AttachSide == "F").Select(x => x.m_ElementId).ToArray();

                            var dDist = ohbikiSize.Width * Math.Tan(((AreaRangeSloped)cArea)._Radian);
                            var uDist = pileSize.Width   * Math.Tan(((AreaRangeSloped)cArea)._Radian);
                            var dTra = sign * ClsRevitUtil.CovertToAPI(dDist) * -axisZ;
                            var uTra = sign * ClsRevitUtil.CovertToAPI(uDist) * axisZ;

                            if (toB)
                            {
                                if(cArea._Type==SlopeStyle.Entrance||(cArea._Type == SlopeStyle.Slope&&k!=cArea._StartOhbikiK))
                                {
                                    ElementTransformUtils.MoveElements(doc, ohbikiB, dTra);
                                }

                                if ((cArea._Type == SlopeStyle.Entrance&& k != cArea._EndOhbikiK) || (cArea._Type == SlopeStyle.Slope && k != cArea._EndOhbikiK))
                                {
                                    ElementTransformUtils.MoveElements(doc, ohbikiF, uTra);
                                }
                            }
                            else
                            {
                                if ((cArea._Type == SlopeStyle.Entrance && k != cArea._StartOhbikiK) || (cArea._Type == SlopeStyle.Slope && k != cArea._StartOhbikiK))
                                {
                                    dTra = sign * ClsRevitUtil.CovertToAPI(uDist) * -axisZ;
                                   ElementTransformUtils.MoveElements(doc, ohbikiB, dTra);
                                }
                                if (cArea._Type == SlopeStyle.Entrance || (cArea._Type == SlopeStyle.Slope && k != cArea._EndOhbikiK))
                                {
                                    uTra = sign * ClsRevitUtil.CovertToAPI(dDist) * axisZ;
                                    ElementTransformUtils.MoveElements(doc, ohbikiF, uTra);
                                }
                            }

                            if (ohbikis.FirstOrDefault().m_Size.StartsWith("["))
                            {
                            //    //F
                            //    ElementTransformUtils.MoveElements(doc, ohbikiIds, transform);

                            //    //B
                            //    ElementTransformUtils.MoveElements(doc, ohbikiIds, transform);

                            //}
                            //else
                            //{
                            //    //F
                            //    ElementTransformUtils.MoveElements(doc, ohbikiIds, transform);

                            //    //B
                            //    ElementTransformUtils.MoveElements(doc, ohbikiIds, transform);

                            }
                        }   
                    }
                }       

                // 杭の対応
                if (!inverse)
                {
                    var pileSetAtK = materials
                        .Select(x => x as PilePillerSuper)
                        .Where(x => x != null)
                        .Where(x => x.m_KyoutyouNum == k)
                        .ToArray();

                    var levelSet = new FilteredElementCollector(doc)
                        .OfClass(typeof(Level))
                        .Cast<Level>()
                        .ToArray();

                    //継足し有無の基準は一括フラットの基準から
                    // 構台の基準レベル
                    var kodaiBaseLevel = new FilteredElementCollector(doc)
                        .OfClass(typeof(Level))
                        .Cast<Level>()
                        .FirstOrDefault(x => x.Name == kodaiData.SelectedLevel);
                    // 構台の基準 Z
                    var baseLevelMm = ClsRevitUtil.CovertFromAPI(kodaiBaseLevel.ProjectElevation);

                    var baseLevel = kodaiBaseLevel;// levelSet
                                                   //.FirstOrDefault(x => x.Name.ToUpper() == kodaiData.SelectedLevel.ToUpper());
                                                   //var baseLevelFt = baseLevelMm;// baseLevel?.Elevation ?? 0.0;

                    var pillarData = kodaiData.pilePillerData;

                    var ohBikiBottomLevelMm = baseLevelMm + kodaiData.LevelOffset;
                    if (kodaiData.BaseLevel == eBaseLevel.FukkouTop)
                    {
                        var fukkoubanH = DefineUtil.FukkouBAN_THICK;
                        var nedaMaterialSize = GantryUtil.GetKouzaiSize(kodaiData.nedaData.NedaSize);
                        var nedaH = nedaMaterialSize.Height;
                        var ohbikiMaterialSize = GantryUtil.GetKouzaiSize(kodaiData.ohbikiData.OhbikiSize);
                        var ohbikiH = nedaMaterialSize.Height;
                        ohBikiBottomLevelMm = ohBikiBottomLevelMm - (fukkoubanH + nedaH + ohbikiH);
                    }
                    var ohbikiZ = ohbikiBottomMm == 0 ? ohBikiBottomLevelMm : ohBikiBottomLevelMm + ohbikiBottomMm;

                    var hasExtraPileEx = false;
                    if (ohbikiBottomMm == 0)
                    {
                        if (pillarData.ExtensionPile && ohBikiBottomLevelMm <= baseLevelMm)
                        {
                            hasExtraPileEx = false;
                        }
                        else
                        {
                            hasExtraPileEx = pillarData.ExtensionPile;
                        }
                    }
                    else
                    {
                        if (ohBikiBottomLevelMm > baseLevelMm && ohbikiZ > baseLevelMm)
                        {
                            hasExtraPileEx = pillarData.ExtensionPile;
                        }
                        else if (ohBikiBottomLevelMm > baseLevelMm && ohbikiZ < baseLevelMm)
                        {
                            hasExtraPileEx = false;
                        }
                        else if (ohBikiBottomLevelMm < baseLevelMm && ohbikiZ > baseLevelMm)
                        {
                            hasExtraPileEx = true;
                        }
                        else if (ohBikiBottomLevelMm < baseLevelMm && ohbikiZ < baseLevelMm)
                        {
                            hasExtraPileEx = false;
                        }
                        else
                        {
                            if (ohbikiZ > baseLevelMm)
                            {
                                hasExtraPileEx = true;
                            }
                            else
                            {
                                hasExtraPileEx = false;
                            }
                        }
                    }

                    var hasExtraPile = hasExtraPileEx;
                    var mainPileSet = pileSetAtK.Where(x => x is PilePiller);
                    var extraPileData = FrmCreateSlopeViewModel.DeepClone(pillarData);

                    var originalWholeLength = pillarData.PillarWholeLength;
                    var mainPileData = FrmCreateSlopeViewModel.DeepClone(pillarData);

                    var cutLength = pillarData.PileCutLength - ohbikiBottomMm;
                    mainPileData.PileCutLength = Math.Sign(cutLength) == -1 ? pillarData.PileCutLength : cutLength;
                    mainPileData.PillarWholeLength = originalWholeLength;
                    mainPileData.ExtensionPile = hasExtraPile;
                    mainPileData.HasTopPlate = pillarData.ExtensionPile ? extraPileData.HasTopPlate : pillarData.HasTopPlate;

                    if (!hasExtraPile && pillarData.ExtensionPile)
                    {
                        mainPileData.IsCut = true;
                        mainPileData.PileCutLength = Math.Abs((extraPileData.extensionPileData.Length + ohbikiBottomMm) - pillarData.PileCutLength);
                    }
                    else if (!hasExtraPile && !pillarData.ExtensionPile)
                    {
                        if (Math.Sign(ohbikiBottomMm) == 1 && pillarData.PileCutLength < ohbikiBottomMm && kodaiData.BaseLevel != eBaseLevel.FukkouTop)
                        {
                            // 切断有の場合に支持杭長の長さを伸ばすような仕様である場合は以下コメントを外す
                            //mainPileData.PillarWholeLength += ohbikiBottomMm;
                        }
                        else
                        {
                            if (pillarData.PileCutLength != cutLength && kodaiData.BaseLevel != eBaseLevel.FukkouTop)
                            {
                                mainPileData.PileCutLength = Math.Abs(cutLength);
                            }
                        }
                    }
                    else if (hasExtraPile && !pillarData.ExtensionPile)
                    {
                        extraPileData.extensionPileData.Length += ohbikiBottomMm;
                    }
                    else if (hasExtraPile && pillarData.ExtensionPile)
                    {
                        if (pillarData.PileCutLength < ohbikiBottomMm)
                        {
                            mainPileData.IsCut = true;
                            mainPileData.PileCutLength = 0;
                            extraPileData.extensionPileData.Length += Math.Abs(cutLength);
                        }
                    }

                    foreach (var srcPile in mainPileSet)
                    {
                        var mainLevelOffset = 0.0;
                        var srcInstance = doc.GetElement(srcPile.m_ElementId) as FamilyInstance;
                        var location = srcInstance.Location as LocationPoint;
                        var originalLevelOffset = srcPile.LevelOffsetFeet;

                        if (pillarData.IsCut)
                        {
                            if (hasExtraPile)
                            {
                                MaterialSize ohbikiSize = GantryUtil.GetKouzaiSize(kodaiData.ohbikiData.OhbikiSize);
                                MaterialSize nedaSize = GantryUtil.GetKouzaiSize(kodaiData.nedaData.NedaSize);
                                var extraLevelOffset = PilePillerSuper.CalcLevelOffsetExtensionPileLevel(baseLevel, kodaiData.BaseLevel, kodaiData.LevelOffset, ohbikiSize.Height, nedaSize.Height);
                                if (!kodaiData.ohbikiData.isHkou) { extraLevelOffset += ohbikiSize.Height; }
                                var extraId = PilePillerSuper.CreateExtraPile(doc, location.Point, location.Rotation, extraPileData, kodaiData.KoudaiName, XYZ.Zero, baseLevel, extraLevelOffset + ohbikiBottomMm);

                                var instance = new ExtensionPile
                                {
                                    m_Document = doc,
                                    m_ElementId = extraId,
                                    m_HukuinNum = srcPile.m_HukuinNum,
                                    m_KodaiName = srcPile.m_KodaiName,
                                    m_KyoutyouNum = srcPile.m_KyoutyouNum,
                                    m_Material = srcPile.m_Material,
                                    m_Size = srcPile.m_Size,
                                };
                                MaterialSuper.WriteToElement(instance, doc);
                            }

                            if (kodaiData.BaseLevel == eBaseLevel.FukkouTop)
                            {
                                double.TryParse(ClsRevitUtil.GetParameter(doc, srcInstance.Id, DefineUtil.PARAM_BASE_OFFSET), out var offsetMm);

                                if (!hasExtraPile && !pillarData.ExtensionPile)
                                {
                                    if (Math.Sign(ohbikiBottomMm) == 1 && pillarData.PileCutLength < ohbikiBottomMm)
                                    {
                                        mainLevelOffset = offsetMm + ohbikiBottomMm;
                                    }
                                    else
                                    {
                                        mainLevelOffset = offsetMm;
                                    }
                                }
                                else
                                {
                                    mainLevelOffset = offsetMm;
                                }
                            }
                            else
                            {
                                if (!hasExtraPile && !pillarData.ExtensionPile)
                                {
                                    if (Math.Sign(ohbikiBottomMm) == 1)
                                    {
                                        if (Math.Sign(kodaiData.LevelOffset) == -1)
                                        {
                                            mainLevelOffset = baseLevelMm + Math.Abs(kodaiData.LevelOffset) + ohbikiBottomMm;
                                        }
                                        else
                                        {

                                            mainLevelOffset = kodaiData.LevelOffset + ohbikiBottomMm;
                                        }
                                    }
                                    else
                                    {
                                        mainLevelOffset = kodaiData.LevelOffset;
                                    }
                                }
                            }
                        }
                        else
                        {
                            mainPileData = FrmCreateSlopeViewModel.DeepClone(pillarData);
                            double.TryParse(ClsRevitUtil.GetParameter(doc, srcInstance.Id, DefineUtil.PARAM_BASE_OFFSET), out var offsetMm);
                            mainLevelOffset = offsetMm + ohbikiBottomMm;

                            MaterialSize plsize = GantryUtil.GetKouzaiSize(mainPileData.topPlateData.PlateSize);
                            mainLevelOffset += mainPileData.HasTopPlate ? plsize.Thick : 0;
                        }

                        var mainId = PilePillerSuper.CreatePile(doc, location.Point, location.Rotation, mainPileData, kodaiData.KoudaiName, XYZ.Zero, baseLevel, mainLevelOffset);

                        if (pillarData.IsCut && !hasExtraPile && !pillarData.ExtensionPile)
                        {
                            if (Math.Sign(kodaiData.LevelOffset) == -1 && kodaiData.BaseLevel == eBaseLevel.OhbikiBtm)
                            {
                                ClsRevitUtil.SetParameter(doc, mainId, DefineUtil.PARAM_HOST_OFFSET, RevitUtil.ClsRevitUtil.CovertToAPI(baseLevelMm));
                            }
                        }

                        var mainInstance = new PilePiller
                        {
                            m_Document = doc,
                            m_ElementId = mainId,
                            m_HukuinNum = srcPile.m_HukuinNum,
                            m_KodaiName = srcPile.m_KodaiName,
                            m_KyoutyouNum = srcPile.m_KyoutyouNum,
                            m_Material = srcPile.m_Material,
                            m_Size = srcPile.m_Size,
                        };
                        MaterialSuper.WriteToElement(mainInstance, doc);
                    }
                    doc.Delete(pileSetAtK.Select(x => x.m_ElementId).ToArray());
                }
            }

            // 根太の分割
            var splitedNedaSet = new List<Neda>();
            if (!inverse)
            {
                var ohbikiSpanNum = kodaiData.KyoutyouPillarPitch.Count() - 1;
                // 根太を分割すべき K
                var nedaSplitKSet = areaRangeSet
                    .SelectMany(x => new int[] { x._StartOhbikiK, x._EndOhbikiK })
                    .Distinct()
                    .Where(x => x != 0 && x != ohbikiSpanNum)
                    .ToArray();

                // 分割対象の根太集合
                var splitingNedaSet = materials
                    .Select(x => x as Neda)
                    .Where(x => x != null)
                    .Where(x => x.m_KodaiName == kodaiData.KoudaiName)
                    .ToArray();

                var kodaiOrigin = BasePoint(doc, kodaiData);
                var valueKSet = Enumerable.Range(0, kodaiData.KyoutyouPillarPitch.Count())
                    .Select(x => RevitUtil.ClsRevitUtil.CovertToAPI(kodaiData.KyoutyouPillarPitch.Skip(1).Take(x).Sum()))
                    .ToArray();
               
                //var valueHSet = splitingNedaSet.GroupBy(x => x.m_H)
                //    .Select(x => (x.FirstOrDefault().CenterLine().GetEndPoint(0) - kodaiOrigin).DotProduct(axisH))
                //    .ToArray();
                var valueHSet = Enumerable.Range(0, kodaiData.NedaPitch.Count())
                    .Select(x => RevitUtil.ClsRevitUtil.CovertToAPI(kodaiData.NedaPitch.Skip(1).Take(x).Sum()))
                    .ToArray();
                
                foreach (var neda in splitingNedaSet)
                {
                    var splitKSet = nedaSplitKSet.Where(x => neda.m_MinK < x && x < neda.m_MaxK);
                    if (!splitKSet.Any())
                    {
                        splitedNedaSet.Add(neda);
                        continue;
                    }
                    var nedaH = neda.m_H;
                    var nedaValueH = valueHSet.ElementAtOrDefault(nedaH);
                    var nedaPtSet = splitKSet.Append(neda.m_MinK).Append(neda.m_MaxK).OrderBy(x => x)
                        .Select(x => new { K = x, Pt = kodaiOrigin + nedaValueH * axisH + valueKSet.ElementAtOrDefault(x) * axisK })
                        .ToArray();

                    for (int i = 0; i < nedaPtSet.Length - 1; i++)
                    {
                        var startPt = nedaPtSet.ElementAtOrDefault(i);
                        var endPt = nedaPtSet.ElementAtOrDefault(i + 1);

                        //var nedaData = new NedaData
                        //{
                        //    NedaMaterial = neda.m_Material,
                        //    NedaSize = neda.m_Size,
                        //    exNedaEndLeng = neda.m_ExEndLen,
                        //    exNedaStartLeng = neda.m_ExStartLen,
                        //    NedaType = "",
                        //};
                        //var (level, levelOffset) = neda.GetLevelAndOffset();

                        var start = startPt.Pt;
                        var end = endPt.Pt;
                        var startToEndUnit = (end - start).Normalize();
                        var hasStartExtend = false;
                        var hasEndExtend = false;
                        if (i == 0)
                        {
                            start -= ClsRevitUtil.CovertToAPI(neda.m_ExStartLen) * startToEndUnit;
                            hasStartExtend = true;
                            if (kodaiData.KyoutyouPillarPitch[0] != 0)
                            {
                                 start -= ClsRevitUtil.CovertToAPI(kodaiData.KyoutyouPillarPitch[0]) * startToEndUnit;
                            }
                        }
                        if (i == nedaPtSet.Length - 2)
                        {
                            end += ClsRevitUtil.CovertToAPI(neda.m_ExEndLen) * startToEndUnit;
                            hasEndExtend = true;
                            if (kodaiData.KyoutyouPillarPitch[0] != 0)
                            {
                                end += ClsRevitUtil.CovertToAPI(kodaiData.KyoutyouPillarPitch[0]) * startToEndUnit;
                            }
                        }

                        var instanceId = ElementId.InvalidElementId;
                        using (var sourceInstance = doc.GetElement(neda.m_ElementId) as FamilyInstance)
                        {
                            var sss = 0.0;

                            //neda.LevelOffsetFeet
                            var level = GantryUtil.GetInstanceLevelAndOffset(doc, sourceInstance, ref sss);
                            instanceId = MaterialSuper.PlaceWithTwoPoints(sourceInstance.Symbol, level.GetPlaneReference(), start, end,  neda.LevelOffsetFeet);
                            //instanceId = instance.Id;
                        }

                        var splitedNeda = new Neda
                        {
                            m_ElementId = instanceId,
                            m_H = neda.m_H,
                            m_KodaiName = neda.m_KodaiName,
                            m_MinK = startPt.K,
                            m_MaxK = endPt.K,
                            m_Size = neda.m_Size,
                            m_ExEndLen = hasEndExtend ? neda.m_ExEndLen : 0.0,
                            m_ExStartLen = hasStartExtend ? neda.m_ExStartLen : 0.0,
                            m_Material = neda.m_Material,
                            m_Document = doc,
                        };
                        MaterialSuper.WriteToElement(splitedNeda, doc);
                        splitedNedaSet.Add(splitedNeda);
                    }
                    RevitUtil.ClsRevitUtil.Delete(neda.m_Document, neda.m_ElementId);
                }
            }
            else
            {
                splitedNedaSet = MaterialSuper.Collect(doc)
                    .Where(x => x.m_KodaiName == kodaiData.KoudaiName)
                    .Select(x => x as Neda)
                    .Where(x => x != null)
                    .ToList();
            }

            // 根太と覆工板
            var fukkobanSet = MaterialSuper.Collect(doc)
                .Where(x => x.m_KodaiName == kodaiData.KoudaiName)
                .Select(x => x as Fukkouban)
                .Where(x => x != null)
                .ToArray();
            foreach (var areaRange in areaRangeSet)
            {
                var minKFeet = ClsRevitUtil.CovertToAPI(kodaiData.KyoutyouPillarPitch.Skip(1).Take(areaRange._StartOhbikiK).Sum());
                var maxKFeet = ClsRevitUtil.CovertToAPI(kodaiData.KyoutyouPillarPitch.Skip(1).Take(areaRange._EndOhbikiK).Sum());

                if (kodaiData.KyoutyouPillarPitch[0] != 0)
                {
                    maxKFeet += ClsRevitUtil.CovertToAPI(kodaiData.KyoutyouPillarPitch[0]);
                }
                var targetNedaSet = splitedNedaSet
                    .Where(x => areaRange._StartOhbikiK <= x.m_MinK && x.m_MaxK <= areaRange._EndOhbikiK)
                    .ToArray();
                splitedNedaSet = splitedNedaSet.Except(targetNedaSet).ToList();

                var targetFukkobanSet = fukkobanSet
                    .Select(x => new { X = x, FeetK = (x.Center - origin).DotProduct(axisK) })
                    .Where(x =>minKFeet <= x.FeetK && ClsGeo.GEO_LT(x.FeetK , maxKFeet))
                    .Select(x => x.X)
                    .ToArray();
                fukkobanSet = fukkobanSet.Except(targetFukkobanSet).ToArray();

                if (areaRange is AreaRangeFlat flatArea)
                {
                    var levelFeet = ClsRevitUtil.CovertToAPI(flatArea._StyleRow.Level);
                    var transform = sign * levelFeet * axisZ;

                    var fukkobanIds = targetFukkobanSet.Select(x => x.m_ElementId).ToArray();

                    if (fukkobanIds.Any())
                        ElementTransformUtils.MoveElements(doc, fukkobanIds, transform);

                    var nedaIds = targetNedaSet.Select(x => x.m_ElementId).Where(x => doc.GetElement(x) != null).ToArray();
                    //var transform = ClsRevitUtil.CovertToAPI(flatArea._OhbikiBottom) * axisZ;
                    if (nedaIds.Any())
                        ElementTransformUtils.MoveElements(doc, nedaIds, transform);
                }
                else if (areaRange is AreaRangeSloped slopeRange)
                {
                    // 回転行列を作成
                    var rotateAxisOrigin = slopeRange._RotateAxis.Origin;
                    var rotateAxisDir = slopeRange._RotateAxis.Direction;
                    var rotateAxisDirOrth = rotateAxisDir.Y * XYZ.BasisX - rotateAxisDir.X * XYZ.BasisY;
                    var rotation = Transform.CreateRotationAtPoint(rotateAxisDir, slopeRange._Radian, rotateAxisOrigin);

                    if (!inverse)
                    {
                        // 傾斜つきの参照面を作成
                        var slopePlaneDir = rotation.OfVector(rotateAxisDirOrth);
                        var cutDir = -rotateAxisDir; //slopeRange._Radian > 0 ? -rotateAxisDir : rotateAxisDir;

                        var axisU = slopePlaneDir.Normalize();
                        var axisV = cutDir.Normalize();
                        var axisW = axisU.CrossProduct(axisV);

                        var slopePlane = doc.Create.NewReferencePlane(rotateAxisOrigin, rotateAxisOrigin + 10 * slopePlaneDir, cutDir, doc.ActiveView);
                        slopePlane.Name = GenerateSlopePlaneName(kodaiData, slopeRange);
                        slopePlane.Maximize3DExtents();

                        var slopedNedaSet = new List<Neda>();
                        foreach (var neda in targetNedaSet)
                        {
                            //var neda = splitedNedaSet.FirstOrDefault(x => x != null);
                            var nedaSlopedId = ElementId.InvalidElementId;
                            using (var familyInstance = doc.GetElement(neda.m_ElementId) as FamilyInstance)
                            {
                                if (familyInstance == null)
                                {
                                    continue;
                                }
                                FamilySymbol symbol = familyInstance.Symbol;

                                var nedaLine = neda.CenterLine;
                                var nedaSlopedPoints = new List<XYZ>();
                                for (int i = 0; i < 2; i++)
                                {
                                    var nedaStart = nedaLine.GetEndPoint(i);
                                    var valueZ = (nedaStart - rotateAxisOrigin).DotProduct(axisZ);
                                    var nedaPtOnPlane = nedaStart - valueZ * axisZ;
                                    nedaSlopedPoints.Add(rotation.OfPoint(nedaPtOnPlane));
                                }

                                var nedaSlopedStart = nedaSlopedPoints.ElementAtOrDefault(0);
                                var nedaSlopedEnd = nedaSlopedPoints.ElementAtOrDefault(1);
                                var nedaData = new NedaData
                                {
                                    NedaMaterial = neda.m_Material,
                                    NedaSize = neda.m_Size,
                                    exNedaEndLeng = neda.m_ExEndLen,
                                    exNedaStartLeng = neda.m_ExStartLen,
                                    NedaType = "",
                                };
                                var nedaHeight = neda.SteelSize.Height;

                                nedaSlopedId = MaterialSuper.PlaceWithTwoPoints(symbol, slopePlane.GetReference(), nedaSlopedStart, nedaSlopedEnd, RevitUtil.ClsRevitUtil.CovertToAPI(0.5 * nedaHeight));
                                //nedaSlopedId = slopedInstance.Id;
                            }
                            var slopedNeda = new Neda
                            {
                                m_ElementId = nedaSlopedId,
                                m_H = neda.m_H,
                                m_KodaiName = neda.m_KodaiName,
                                m_MinK = neda.m_MinK,
                                m_MaxK = neda.m_MaxK,
                                m_Size = neda.m_Size,
                                m_ExEndLen = neda.m_ExEndLen,
                                m_ExStartLen = neda.m_ExStartLen,
                                m_Material = neda.m_Material,
                                m_Document = doc,
                            };
                            MaterialSuper.WriteToElement(slopedNeda, doc);
                            slopedNedaSet.Add(slopedNeda);
                            ClsRevitUtil.Delete(neda.m_Document, neda.m_ElementId);
                        }

                        foreach (var fukkoban in targetFukkobanSet)
                        {
                            var nedaHeight = slopedNedaSet.FirstOrDefault()?.SteelSize?.Height ?? 0.0;
                            var originalFukkobanPosition = fukkoban.Position;
                            var originalZ = (originalFukkobanPosition - rotateAxisOrigin).DotProduct(axisZ);
                            var positionOnPlane = originalFukkobanPosition + (-originalZ) * axisZ;
                            var positionSloped = rotation.OfPoint(positionOnPlane);

                            var familyInstance = doc.GetElement(fukkoban.m_ElementId) as FamilyInstance;
                            var direction = rotation.OfVector(familyInstance.GetTransform().BasisX);

                            var instance = doc.Create.NewFamilyInstance(slopePlane.GetReference(), positionSloped, direction, familyInstance.Symbol);

                            ClsRevitUtil.SetParameter(doc, instance.Id, DefineUtil.PARAM_HOST_OFFSET, RevitUtil.ClsRevitUtil.CovertToAPI(nedaHeight));

                            var slopedFukkoban = new Fukkouban
                            {
                                m_Document = doc,
                                m_ElementId = instance.Id,
                                m_hukuinNum = fukkoban.m_hukuinNum,
                                m_KodaiName = fukkoban.m_KodaiName,
                                m_kyoutyouNum = fukkoban.m_kyoutyouNum,
                                m_Material = fukkoban.m_Material,
                                m_Size = fukkoban.m_Size,
                            };
                            MaterialSuper.WriteToElement(slopedFukkoban, doc);
                            ClsRevitUtil.Delete(fukkoban.m_Document, fukkoban.m_ElementId);
                        }
                    }
                    else
                    {
                        //戻す処理
                        // 参照面削除
                        var planeName = GenerateSlopePlaneName(kodaiData, slopeRange);
                        var slopePlane = new FilteredElementCollector(doc)
                            .OfClass(typeof(ReferencePlane))
                            .Cast<ReferencePlane>()
                            .FirstOrDefault(x => x.Name == planeName);
                        if (slopePlane != null)
                        {
                            ClsRevitUtil.Delete(doc, slopePlane.Id);
                        }

                        var evenLevel = new FilteredElementCollector(doc)
                            .OfClass(typeof(Level))
                            .Cast<Level>()
                            .FirstOrDefault(x => x.Name == kodaiData.SelectedLevel);
                        var evenLevelOffset = ClsRevitUtil.CovertToAPI(kodaiData.LevelOffset);

                        //回転して水平に戻したのちに evenLevel まで上下移動する行列
                        var inverseRotation = rotation.Inverse;

                        var ohbikiTopSet = MaterialSuper.Collect(doc)
                            .Where(x => x.m_KodaiName == kodaiData.KoudaiName)
                            .Select(x => x as OhbikiSuper)
                            .Where(x => x != null)
                            .Where(x => slopeRange._StartOhbikiK <= x.m_K && x.m_K <= slopeRange._EndOhbikiK)
                            .Select(x => x.LevelOffsetFeet + 0.5 * x.SteelSize.HeightFeet)
                            .ToArray();
                        var ohbikiTopFeet = 0.0;
                        if (ohbikiTopSet.Any())
                        {
                            ohbikiTopFeet = ohbikiTopSet.Max();
                        }
                        var evenNedaSet = new List<Neda>();
                        foreach (var neda in targetNedaSet)
                        {
                            var familyInstance = doc.GetElement(neda.m_ElementId) as FamilyInstance;

                            var nedaLine = neda.CenterLine;
                            var nedaSlopedPoints = new List<XYZ>();
                            for (int i = 0; i < 2; i++)
                            {
                                var nedaStart = nedaLine.GetEndPoint(i);
                                nedaSlopedPoints.Add(inverseRotation.OfPoint(nedaStart));
                            }

                            var evenStart = nedaSlopedPoints.ElementAtOrDefault(0);
                            var evenEnd = nedaSlopedPoints.ElementAtOrDefault(1);
                            //var nedaData = new NedaData
                            //{
                            //    NedaMaterial = neda.m_Material,
                            //    NedaSize = neda.m_Size,
                            //    exNedaEndLeng = neda.m_ExEndLen,
                            //    exNedaStartLeng = neda.m_ExStartLen,
                            //    NedaType = "",
                            //};
                            var nedaHeightFeet = neda.SteelSize.HeightFeet;

                            var evenInstanceId = MaterialSuper.PlaceWithTwoPoints(familyInstance.Symbol, evenLevel.GetPlaneReference(), evenStart, evenEnd, 0.5 * nedaHeightFeet + ohbikiTopFeet);
                            //ElementId nedaSlopedId;

                            //var instance = doc.Create.NewFamilyInstance(evenLevel.GetPlaneReference(), evenStart, evenEnd - evenStart, familyInstance.Symbol);
                            //ClsRevitUtil.SetParameter(doc, instance.Id, "YMS_Length", evenStart.DistanceTo(evenEnd));
                            //ClsRevitUtil.SetParameter(doc, instance.Id, DefineUtil.PARAM_HOST_OFFSET, RevitUtil.ClsRevitUtil.CovertToAPI(0.5 * nedaHeight));
                            //nedaSlopedId = instance.Id;

                            var evenNeda = new Neda
                            {
                                m_ElementId = evenInstanceId,
                                m_H = neda.m_H,
                                m_KodaiName = neda.m_KodaiName,
                                m_MinK = neda.m_MinK,
                                m_MaxK = neda.m_MaxK,
                                m_Size = neda.m_Size,
                                m_ExEndLen = neda.m_ExEndLen,
                                m_ExStartLen = neda.m_ExStartLen,
                                m_Material = neda.m_Material,
                                m_Document = doc,
                            };
                            MaterialSuper.WriteToElement(evenNeda, doc);
                            evenNedaSet.Add(evenNeda);
                            ClsRevitUtil.Delete(neda.m_Document, neda.m_ElementId);
                        }

                        // 大引天端 + 根太高さ で覆工板のレベルオフセットを求める
                        var fukkobanLevelOffsetFeet = ohbikiTopFeet;
                        if (evenNedaSet.Any())
                        {
                            fukkobanLevelOffsetFeet += evenNedaSet.Select(x => x.SteelSize.HeightFeet).Max();
                        }

                        foreach (var fukkoban in targetFukkobanSet)
                        {
                            var familyInstance = doc.GetElement(fukkoban.m_ElementId) as FamilyInstance;

                            var evenPosition = inverseRotation.OfPoint(fukkoban.Position);
                            var evenDirection = inverseRotation.OfVector(familyInstance.GetTransform().BasisX);
                            var instance = doc.Create.NewFamilyInstance(evenLevel.GetPlaneReference(), evenPosition, evenDirection, familyInstance.Symbol);

                            ClsRevitUtil.SetParameter(doc, instance.Id, DefineUtil.PARAM_BASE_OFFSET, fukkobanLevelOffsetFeet);

                            var evenFukkoban = new Fukkouban
                            {
                                m_Document = doc,
                                m_ElementId = instance.Id,
                                m_hukuinNum = fukkoban.m_hukuinNum,
                                m_KodaiName = fukkoban.m_KodaiName,
                                m_kyoutyouNum = fukkoban.m_kyoutyouNum,
                                m_Material = fukkoban.m_Material,
                                m_Size = fukkoban.m_Size,
                            };
                            MaterialSuper.WriteToElement(evenFukkoban, doc);
                            ClsRevitUtil.Delete(fukkoban.m_Document, fukkoban.m_ElementId);
                        }
                    }
                }
            }

            // 補助部材
            if (inverse)
            {
                // フラットに戻すときは全て削除
                var slopeSupportIds = MaterialSuper.Collect(doc)
                    .Where(x => x.m_KodaiName == kodaiData.KoudaiName)
                    .Select(x => x as SlopeSupport)
                    .Where(x => x != null)
                    .Select(x => x.m_ElementId)
                    .ToList();
                ClsRevitUtil.Delete(doc, slopeSupportIds);
            }
            if (!inverse)
            {
                var materialSet = MaterialSuper.Collect(doc)
                    .Where(x => x.m_KodaiName == kodaiData.KoudaiName)
                    .ToArray();
                var ohbikiSet = materialSet
                    .Select(x => x as OhbikiSuper)
                    .Where(x => x != null)
                    .ToArray();
                var nedaSet = materialSet
                    .Select(x => x as Neda)
                    .Where(x => x != null)
                    .ToArray();
                var baseLevel = new FilteredElementCollector(doc)
                    .OfClass(typeof(Level))
                    .Cast<Level>()
                    .FirstOrDefault(x => x.Name == kodaiData.SelectedLevel);
                foreach (var a in slopeModel.SupportList.Where(x => x.HasTensetsuban != YMSGridNames.Nil))
                {
                    var k = a.Index;
                    if (k < 0) { continue; }
                    var rec = ClsMasterCsv.Shared
                        .Select(x => x as ClsStiffenerCsv)
                        .Where(x => x != null)
                        .FirstOrDefault(x => x.Size == a.HasTensetsuban);
                    if (rec == null) { continue; }
                    var familyName = rec.FamilyName;// "添接板（スロープ桁）_PL9x150x150_21";
                    var familyTypeName = rec.TypeName;// "ｽﾁﾌﾅｰﾌﾟﾚｰﾄ";
                    var familyFileName = $"{familyName}.rfa";
                    var familyPath = Directory.EnumerateFiles(PathUtil.GetYMSFolder(), familyFileName, SearchOption.AllDirectories).FirstOrDefault();
                    if (string.IsNullOrEmpty(familyPath))
                    {
                        _logger.Error($"\"{familyFileName}\" does not exist");
                        continue;
                    }
                    if (!GantryUtil.GetFamilySymbol(doc, familyPath, familyTypeName, out var symbol, true))
                    {
                        _logger.Error($"family:\"{familyName}\", type:\"{familyTypeName}\" cannot load");
                        continue;
                    }

                    var ohbiki = ohbikiSet.FirstOrDefault(x => x.m_K == k);
                    var neda = nedaSet.FirstOrDefault(x => x.m_MinK <= k && k <= x.m_MaxK);

                    var nedaWebThickFt = neda?.SteelSize?.WebThickFeet ?? 0.0;
                    var nedaHeightFt = neda?.SteelSize?.HeightFeet ?? 0.0;
                    var ohbikiHeightFt = ohbiki?.SteelSize?.HeightFeet ?? 0.0;
                    var ohbikiHost = ohbiki?.HostLevel;
                    var ohbikiLevelOffsetFt = ohbiki?.LevelOffsetFeet ?? 0.0;
                    var ohbikiPt = ohbiki.Position;

                    var levelOffsetFeet = ohbikiLevelOffsetFt + 0.5 * (ohbikiHeightFt + nedaHeightFt);

                    var elemK = (ohbikiPt - origin).DotProduct(axisK);
                    var elemHSet = Enumerable.Range(0, kodaiData.NedaPitch.Count())
                        .Select(x => ClsRevitUtil.CovertToAPI(kodaiData.NedaPitch.Skip(1).Take(x).Sum())).ToArray();

                    for (int h = 0; h < elemHSet.Length; h++)
                    //foreach (var elemH in elemHSet)
                    {
                        var elemH = elemHSet.ElementAtOrDefault(h);
                        var ptOnXY = origin + elemK * axisK + elemH * axisH;
                        foreach (var coef in new int[] { 1, -1 })
                        {
                            var dirH = coef * axisH;
                            var dirK = -coef * axisK;

                            if (!symbol.IsActive) { symbol.Activate(); }
                            var instance = doc.Create.NewFamilyInstance(ohbikiHost.GetPlaneReference(), ptOnXY - 0.5 * nedaWebThickFt * dirH, dirK, symbol);
                            ClsRevitUtil.SetParameter(doc, instance.Id, DefineUtil.PARAM_HOST_OFFSET, levelOffsetFeet);

                            var data = new Tensetsuban
                            {
                                m_Document = doc,
                                m_ElementId = instance.Id,
                                m_K = k,
                                m_H = h,
                                m_KodaiName = kodaiData.KoudaiName,
                            };

                            MaterialSuper.WriteToElement(data, doc);
                        }
                    }
                }

                // スティフナー
                foreach (var a in slopeModel.SupportList.Where(x => x.Stiffener != YMSGridNames.Nil))
                {
                    var k = a.Index;
                    if (k < 0) { continue; }

                    // 大引のサイズからスティフナーファミリ名を引き当て
                    var ohbiki = ohbikiSet.FirstOrDefault(x => x.m_K == k);
                    if (ohbiki == null) { continue; }
                    var ohbikiSizeText = ohbiki.m_Size.Replace("-", "").ToLower();
                    //var stiffenerRec = StiffenerTable.Value.FirstOrDefault(x => x.StiffenerType == a.Stiffener && ohbikiSizeText.Contains(x.OhbikiSize.ToLower()));
                    //if (stiffenerRec == null)
                    //{
                    //    _logger.Error($"StiffenerType:\"{a.Stiffener}\", OhbikiSize:\"{ohbikiSizeText}\" に適合するスチフナが見つかりません。");
                    //    continue;
                    //}
                    var familyName = "";//= stiffenerRec.FamilyName;// "ｽﾁﾌﾅｰﾌﾟﾚｰﾄ_各桁サイズ用_21";
                    var familyTypeName = "";//= stiffenerRec.SymbolName;// "ｽﾁﾌﾅｰﾌﾟﾚｰﾄ_各桁サイズ用_21";

                    var isJack = true;
                    if (a.Stiffener == YMSGridNames.Plate)
                    {
                        isJack = false;
                        var stiffenerRec = StiffenerTable.Value.FirstOrDefault(x => x.StiffenerType == StiffenerType.Plate && ohbikiSizeText.Contains(x.OhbikiSize.ToLower()));
                        if (stiffenerRec == null)
                        {
                            _logger.Error($"StiffenerType:\"{a.Stiffener}\", OhbikiSize:\"{ohbikiSizeText}\" に適合するスチフナが見つかりません。");
                            continue;
                        }
                        familyName = stiffenerRec.FamilyName;// "ｽﾁﾌﾅｰﾌﾟﾚｰﾄ_各桁サイズ用_21";
                        familyTypeName = stiffenerRec.SymbolName;// "ｽﾁﾌﾅｰﾌﾟﾚｰﾄ_各桁サイズ用_21";
                    }
                    else
                    {
                        isJack = true;
                        var jackSizeText = a.Stiffener;
                        var stiffenerRec = ClsMasterCsv.Shared
                            .Select(x => x as ClsStiffenerCsv)
                            .Where(x => x != null)
                            .Where(x => x.Type == ClsStiffenerCsv.SHJack || x.Type == ClsStiffenerCsv.DWJJack)
                            .FirstOrDefault(x => x.Size == jackSizeText);
                        if (stiffenerRec == null)
                        {
                            _logger.Error($"StiffenerType:\"{a.Stiffener}\", OhbikiSize:\"{ohbikiSizeText}\" に適合するスチフナが見つかりません。");
                            continue;
                        }
                        familyName = stiffenerRec.FamilyName;// "ｽﾁﾌﾅｰﾌﾟﾚｰﾄ_各桁サイズ用_21";
                        familyTypeName = stiffenerRec.TypeName;// "ｽﾁﾌﾅｰﾌﾟﾚｰﾄ_各桁サイズ用_21";
                    }

                    var familyFileName = $"{familyName}.rfa";
                    var familyPath = Directory.EnumerateFiles(PathUtil.GetYMSFolder(), familyFileName, SearchOption.AllDirectories).FirstOrDefault();
                    if (string.IsNullOrEmpty(familyPath))
                    {
                        _logger.Error($"\"{familyFileName}\" does not exist");
                        continue;
                    }
                    if (!GantryUtil.GetFamilySymbol(doc, familyPath, familyTypeName, out var symbol, true))
                    {
                        _logger.Error($"family:\"{familyName}\", type:\"{familyTypeName}\" cannot load");
                        continue;
                    }

                    //var neda = nedaSet.FirstOrDefault(x => x.m_MinK <= k && k <= x.m_MaxK);

                    //var nedaWebThickFt = neda?.SteelSize?.WebThickFeet ?? 0.0;
                    //var nedaHeightFt = neda?.SteelSize?.HeightFeet ?? 0.0;
                    //var ohbikiHeightFt = ohbiki?.SteelSize?.HeightFeet ?? 0.0;
                    var ohbikiHost = ohbiki?.HostLevel;
                    var ohbikiLevelOffsetFt = ohbiki?.LevelOffsetFeet ?? 0.0;
                    var ohbikiPt = ohbiki.Position;
                    var ohbikiWebThickFt = ohbiki?.SteelSize?.WebThickFeet ?? 0.0;
                    var ohbikiSize = ohbiki?.SteelSize;

                    var levelOffsetFeet = ohbikiLevelOffsetFt;

                    var elemK = (ohbikiPt - origin).DotProduct(axisK);
                    var elemHSet = Enumerable.Range(0, kodaiData.NedaPitch.Count())
                        .Select(x => ClsRevitUtil.CovertToAPI(kodaiData.NedaPitch.Skip(1).Take(x).Sum())).ToArray();

                    var slopedRange = areaRangeSet.Select(x => x as AreaRangeSloped)
                        .Where(x => x != null)
                        .FirstOrDefault(x => x._StartOhbikiK <= k && k <= x._EndOhbikiK);
                    var dirSign = -1;
                    if (slopedRange != null)
                    {
                        var thetaIsNegative = slopedRange._Radian < 0;
                        dirSign = thetaIsNegative ? 1 : -1;
                        if (slopedRange._Type == SlopeStyle.Slope)
                        {
                            if ((!thetaIsNegative && k == slopedRange._StartOhbikiK) || (thetaIsNegative && k == slopedRange._EndOhbikiK))
                            {
                                dirSign *= -1;
                            }
                        }
                    }

                    for (int h = 0; h < elemHSet.Length; h++)
                    {
                        var elemH = elemHSet.ElementAtOrDefault(h);

                        var dirH = axisH;
                        var dirK = dirSign * axisK;
                        var basePt = origin + elemK * axisK + elemH * axisH;
                        var ptOnXY = origin + elemK * axisK + elemH * axisH /*+ 0.5 * ohbikiWebThickFt * dirK*/;
                        var levelOffsetLocal = 0.0;

                        if (isJack)
                        {
                            //var rotation = Transform.Identity;//.CreateRotationAtPoint(XYZ.BasisZ, Math.PI, basePt);
                            var rotation = Transform.CreateRotationAtPoint(XYZ.BasisZ, Math.PI, basePt);
                            dirK = rotation.OfVector(dirK);// - dirK.Y * XYZ.BasisX + dirK.X * XYZ.BasisY;
                            if (familyName.Contains("DWJ")) { dirK = dirK.Negate(); }
                            var bb = 0.5 * (ohbikiSize.FrangeWidthFeet/* - ohbikiSize.WebThickFeet*/);
                            ptOnXY = basePt - bb * dirK;
                            var aa = (ohbikiSize.HeightFeet - 2.0 * ohbikiSize.FrangeThick.ToFt()) * 0.5;
                            levelOffsetLocal = levelOffsetFeet - aa;
                        }
                        else
                        {
                            var rotation = Transform.CreateRotationAtPoint(XYZ.BasisZ, Math.PI * 0.5, basePt);
                            dirK = rotation.OfVector(dirK);// - dirK.Y * XYZ.BasisX + dirK.X * XYZ.BasisY;
                            ptOnXY = rotation.Inverse.OfPoint(ptOnXY);
                            var sRotation = Transform.CreateRotationAtPoint(XYZ.BasisZ, Math.PI/2, dirK);
                            var sdirK = rotation.OfVector(dirK.Negate());
                            ptOnXY = ptOnXY + sdirK.Normalize() * ClsRevitUtil.CovertToAPI((ohbiki.SteelSize.WebThick / 2) + 1);

                            levelOffsetLocal = levelOffsetFeet;
                        }

                        if (!symbol.IsActive) { symbol.Activate(); }
                        var instance = doc.Create.NewFamilyInstance(ohbikiHost.GetPlaneReference(), ptOnXY, dirK, symbol);
                        ClsRevitUtil.SetParameter(doc, instance.Id, DefineUtil.PARAM_HOST_OFFSET, levelOffsetLocal);

                        if (isJack)
                        {
                            var aa = Transform.Identity;
                            ElementTransformUtils.RotateElement(doc, instance.Id, Line.CreateUnbound(ptOnXY, XYZ.BasisZ), Math.PI);

                            var data = new StiffenerJack
                            {
                                m_Document = doc,
                                m_ElementId = instance.Id,
                                m_K = k,
                                m_H = h,
                                m_KodaiName = kodaiData.KoudaiName,
                            };

                            MaterialSuper.WriteToElement(data, doc);
                        }
                        else
                        {
                            var data = new StiffenerPlate
                            {
                                m_Document = doc,
                                m_ElementId = instance.Id,
                                m_K = k,
                                m_H = h,
                                m_KodaiName = kodaiData.KoudaiName,
                            };

                            MaterialSuper.WriteToElement(data, doc);
                        }
                    }
                }

                // 間詰材
                foreach (var a in slopeModel.SupportList.Where(x => x.Madumezai != YMSGridNames.Nil))
                {
                    var k = a.Index;
                    if (k < 0) { continue; }
                    var familyName = "";
                    var familyTypeName = "";
                    var widthLSteel = 0.0;
                    var thickLSteel = 0.0;

                    var rec = ClsMasterCsv.Shared
                        .Select(x => x as ClsAngleCsv)
                        .Where(x => x != null)
                        .FirstOrDefault(x => x.Size == a.Madumezai);
                    if (rec == null) { continue; }
                    familyName = rec.FamilyName;
                    familyTypeName = rec.MadumeSymbol;
                    widthLSteel = rec.A.ToFt();
                    thickLSteel = rec.T.ToFt();// ClsRevitUtil.CovertToAPI(9.0);
                    //if (a.Madumezai == MadumezaiType.L75x9)
                    //{
                    //    familyName = "形鋼_L-75x75x9";
                    //    familyTypeName = "ｽﾛｰﾌﾟ間詰め材";
                    //    widthLSteel = ClsRevitUtil.CovertToAPI(75.0);
                    //    thickLSteel = ClsRevitUtil.CovertToAPI(9.0);
                    //}
                    //else if (a.Madumezai == MadumezaiType.L75x6)
                    //{
                    //    familyName = "形鋼_L-75x75x6";
                    //    familyTypeName = "ｽﾛｰﾌﾟ間詰め材";
                    //    widthLSteel = ClsRevitUtil.CovertToAPI(75.0);
                    //    thickLSteel = ClsRevitUtil.CovertToAPI(6.0);
                    //}

                    var familyFileName = $"{familyName}.rfa";
                    var familyPath = Directory.EnumerateFiles(PathUtil.GetYMSFolder(), familyFileName, SearchOption.AllDirectories).FirstOrDefault();
                    if (string.IsNullOrEmpty(familyPath))
                    {
                        _logger.Error($"\"{familyFileName}\" does not exist");
                        continue;
                    }
                    if (!GantryUtil.GetFamilySymbol(doc, familyPath, familyTypeName, out var symbol, true))
                    {
                        _logger.Error($"family:\"{familyName}\", type:\"{familyTypeName}\" cannot load");
                        continue;
                    }

                    var ohbiki = ohbikiSet.FirstOrDefault(x => x.m_K == k);
                    var neda = nedaSet.FirstOrDefault(x => x.m_MinK <= k && k <= x.m_MaxK);

                    //var nedaWebThickFt = neda?.SteelSize?.WebThickFeet ?? 0.0;
                    //var nedaHeightFt = neda?.SteelSize?.HeightFeet ?? 0.0;
                    //var ohbikiHeightFt = ohbiki?.SteelSize?.HeightFeet ?? 0.0;
                    var ohbikiHost = ohbiki?.HostLevel;
                    var ohbikiLevelOffsetFt = ohbiki?.LevelOffsetFeet ?? 0.0;
                    var ohbikiPt = ohbiki.Position;
                    var ohbikiHeightFt = ohbiki?.SteelSize?.HeightFeet ?? 0.0;
                    //var ohbikiWebThickFt = ohbiki?.SteelSize?.WebThickFeet ?? 0.0;

                    var levelOffsetFeet = ohbikiLevelOffsetFt;

                    var elemK = (ohbikiPt - origin).DotProduct(axisK);
                    var elemHSet = Enumerable.Range(0, kodaiData.NedaPitch.Count())
                        .Select(x => ClsRevitUtil.CovertToAPI(kodaiData.NedaPitch.Skip(1).Take(x).Sum())).ToArray();

                    var slopedRange = areaRangeSet.Select(x => x as AreaRangeSloped)
                        .Where(x => x != null)
                        .FirstOrDefault(x => (x._Radian > 0 && x._EndOhbikiK == k) || (x._Radian < 0 && x._StartOhbikiK == k));
                    if (slopedRange == null) { continue; }

                    var thetaIsNegative = slopedRange._Radian < 0;
                    var dirH = thetaIsNegative ? axisH : -axisH;
                    var dirK = thetaIsNegative ? axisK : -axisK;
                    var lastH = kodaiData.NedaPitch.Count() - 1;
                    var startH = thetaIsNegative ? 0 : lastH;
                    var endH = thetaIsNegative ? lastH : 0;

                    var theta = Math.Abs(slopedRange._Radian);
                    var ohbikiHalf = 0.5 * (ohbiki?.SteelSize?.FrangeWidthFeet ?? 0);
                    var nedaHeight = neda?.SteelSize?.HeightFeet ?? 0;
                    var fukkoHeight = ClsRevitUtil.CovertToAPI(DefineUtil.FukkouBAN_THICK); // 覆工板の種類に依らず固定
                    var height = nedaHeight + fukkoHeight;
                    var w = ohbikiHalf;
                    var h = height;
                    var alpha = widthLSteel - thickLSteel;

                    var spaceLength = w + (h - alpha) * Math.Tan(theta) - w / Math.Cos(theta);
                    //=  ohbikiHalf * (1 - Math.Cos(theta) - Math.Sin(theta)) + height - widthLSteel + thickLSteel;

                    var diff = Math.Min(spaceLength, widthLSteel - ClsRevitUtil.CovertToAPI(1e1));

                    var startPt = origin + elemHSet.ElementAtOrDefault(startH) * axisH + elemK * axisK + diff * dirK;
                    var endPt = origin + elemHSet.ElementAtOrDefault(endH) * axisH + elemK * axisK + diff * dirK;

                    var levelFromBase = ohbikiLevelOffsetFt + 0.5 * ohbikiHeightFt + height + thickLSteel;
                    var baseElevation = baseLevel?.ProjectElevation ?? 0;
                    var levelOffsetFt = levelFromBase + baseElevation;

                    if (!symbol.IsActive) { symbol.Activate(); }
                    var negativePlane = PositivePlane(doc);// NegativePlane(doc);
                    ElementId instanceId = ElementId.InvalidElementId;
                    //var instance = doc.Create.NewFamilyInstance(ohbikiHost.GetPlaneReference(), ptOnXY, dirK, symbol);
                    //ClsRevitUtil.SetParameter(doc, instanceId, DefineUtil.PARAM_HOST_OFFSET, levelOffsetFeet);

                    if(!kodaiData.ohbikiData.isHkou&&ohbiki.MaterialSize().Shape!=MaterialShape.H)
                    {
                        ohbiki = ohbikiSet.Where(x => x.m_K == k).OrderBy(x=>x.Position.Z).Select(x=>x as OhbikiSuper).FirstOrDefault();
                        startPt = origin + elemHSet.ElementAtOrDefault(startH) * axisH + XYZ.BasisZ * (nedaHeight + DefineUtil.FukkouBAN_THICK);
                        endPt = origin + elemHSet.ElementAtOrDefault(endH) * axisH + XYZ.BasisZ * (nedaHeight + DefineUtil.FukkouBAN_THICK);
                        if (ohbiki != null)
                        {
                            neda = nedaSet.FirstOrDefault(x => x.m_MinK <= k && k <= x.m_MaxK);
                            nedaHeight = neda?.SteelSize?.HeightFeet ?? 0;

                            var pSize = GantryUtil.GetKouzaiSize(kodaiData.pilePillerData.PillarSize);
                            var dir = ((ohbiki.m_AttachSide == "F") ? -axisK : axisK)*ClsRevitUtil.CovertToAPI(pSize.Height/2);
                            var pnts = GantryUtil.GetCurvePoints(doc, ohbiki.m_ElementId);
                            var ps = ((pnts[0].DistanceTo(origin) < pnts[1].DistanceTo(origin)) ? pnts[0] : pnts[1])+dir;
                            var pe = ((pnts[0].DistanceTo(origin) < pnts[1].DistanceTo(origin)) ? pnts[1]:pnts[0])+dir;
                            startPt = new XYZ(ps.X, ps.Y, startPt.Z);
                            endPt = new XYZ(pe.X, pe.Y, endPt.Z);
                        }
                        var st=slopedRange._OhbikiBottoms.Where(x=>x._K==k).FirstOrDefault();
                        levelFromBase =ohbiki.LevelOffsetFeet+ohbiki.LevelOffsetFeet*-1;
                        levelOffsetFt = levelFromBase + baseElevation + ClsRevitUtil.CovertToAPI(st?._Z??0);
                        instanceId = MaterialSuper.PlaceWithTwoPoints(symbol, reference: negativePlane.GetReference(), startPt, endPt, levelOffsetFeet: levelOffsetFt);
                    }
                    else
                    {
                        instanceId = MaterialSuper.PlaceWithTwoPoints(symbol, reference: negativePlane.GetReference(), startPt, endPt, levelOffsetFeet: levelOffsetFt);
                    }

                    if(instanceId!=null)
                    {
                        var data = new Madumezai
                        {
                            m_Document = doc,
                            m_ElementId = instanceId,
                            m_K = k,
                            m_KodaiName = kodaiData.KoudaiName,
                        };

                        MaterialSuper.WriteToElement(data, doc);

                    }
                }
            }
        }

        private static string GenerateSlopePlaneName(AllKoudaiFlatFrmData kodaiData, AreaRangeSloped areaRange)
        => $"\"{kodaiData.KoudaiName}\",\"{areaRange._StartOhbikiK}\",\"{areaRange._EndOhbikiK}\"";

        public static ReferencePlane NegativePlane(Document doc)
        {
            var planeName = "NegativePlane";
            var existedPlane = new FilteredElementCollector(doc)
                .OfClass(typeof(ReferencePlane))
                .Cast<ReferencePlane>()
                .FirstOrDefault(x => x.Name == planeName);
            if (existedPlane != null)
            {
                return existedPlane;
            }
            var result = doc.Create.NewReferencePlane(XYZ.Zero, XYZ.Zero + XYZ.BasisY, -XYZ.BasisX, doc.ActiveView);
            result.Name = planeName;
            return result;
        }

        public static ReferencePlane PositivePlane(Document doc)
        {
            var planeName = "PositivePlane";
            var existedPlane = new FilteredElementCollector(doc)
                .OfClass(typeof(ReferencePlane))
                .Cast<ReferencePlane>()
                .FirstOrDefault(x => x.Name == planeName);
            if (existedPlane != null)
            {
                return existedPlane;
            }
            var result = doc.Create.NewReferencePlane(XYZ.Zero, XYZ.Zero + XYZ.BasisY, XYZ.BasisX, doc.ActiveView);
            result.Name = planeName;
            return result;
        }

        //private static string GenerateMadumezaiPlaneName(string kodaiName, int k)
        //=> $"\"間詰材\",\"{kodaiName}\",\"{k}\"";

        //private static ElementId[] PlacePillar(Document doc, PilePiller source, XYZ pt, double radian, double ohbikiBottomFt, double groundLevelFt)
        //{
        //    var result = new List<ElementId>();
        //    var hasExtensionPillar = true;
        //    if (hasExtensionPillar)
        //    {
        //        // 継足し杭あり
        //        var data = new PilePillerData();
        //        var extraPileId = PilePillerSuper.CreateExtraPile(doc, pt, radian, data, XYZ.Zero);
        //        var mainPileId = PilePillerSuper.CreatePilePiller(doc, pt, radian, data, XYZ.Zero);

        //        var extraPile = new ExtensionPile();

        //        result.Add(extraPileId);
        //        result.Add(mainPileId);
        //    }
        //    else
        //    {
        //        // 継足し杭無し
        //        var mainPileId = PilePillerSuper.CreatePilePiller();
        //        result.Add(mainPileId);
        //    }
        //    return result.ToArray();
        //}

        private static XYZ BasePoint(Document doc, AllKoudaiFlatFrmData kodaiData)
        {
            var coordinate = kodaiData.GetCoordinate(doc);
            var hukuinPitches = kodaiData.NedaPitch;
            var kyochoPitches = kodaiData.KyoutyouPillarPitch;

            return coordinate.Origin + ClsRevitUtil.CovertToAPI(hukuinPitches.FirstOrDefault()) * coordinate.BasisH + ClsRevitUtil.CovertToAPI(kyochoPitches.FirstOrDefault()) * coordinate.BasisK;
        }

        private static XYZ To2d(XYZ pt)
        {
            return new XYZ(pt.X, pt.Y, 0.0);
        }

        private static bool EqualSlopeRow(FrmCreateSlopeViewModel.StyleRow a, FrmCreateSlopeViewModel.StyleRow b)
        {
            if (a.SlopeType != b.SlopeType) { return false; }
            if (a.SlopeType == FrmCreateSlopeViewModel.SlopeStyle.Nil)
            {
                return a.Level == b.Level;
            }
            else if (a.SlopeType == FrmCreateSlopeViewModel.SlopeStyle.Slope)
            {
                return Math.Abs(a.Percent - b.Percent) < 1e-3;
            }
            return true;
        }

        private static AreaRange CreateAreaRangeFrom(FrmCreateSlopeViewModel.StyleRow a, int startK, int endK)
        {
            if (a.SlopeType == FrmCreateSlopeViewModel.SlopeStyle.Nil)
            {
                return new AreaRangeFlat
                {
                    _StyleRow = a,
                    _Type = a.SlopeType,
                    _StartOhbikiK = startK,
                    _EndOhbikiK = endK,
                    _OhbikiBottom = a.Level,
                };
            }
            else
            {
                return new AreaRangeSloped
                {
                    _StyleRow = a,
                    _Type = a.SlopeType,
                    _StartOhbikiK = startK,
                    _EndOhbikiK = endK,
                };
            }
        }

        private static AreaRange[] ConvertAreaRangeFromInputData(FrmCreateSlopeViewModel.KodaiSlopeModel slopeInputData, IEnumerable<MaterialSuper> materials, AllKoudaiFlatFrmData kodaiData, bool inverse)
        {
            var areaRangeSet = new List<AreaRange>();
            var startIndex = 0;
            FrmCreateSlopeViewModel.StyleRow prevData = null;
            var ohbikiSpanNum = slopeInputData.StyleList.Length;
            for (int i = 0; i < ohbikiSpanNum; i++)
            {
                var currentData = slopeInputData.StyleList.ElementAtOrDefault(i);
                if (prevData == null)
                {
                    prevData = currentData;
                    startIndex = i;
                    continue;
                }

                if (EqualSlopeRow(currentData, prevData))
                {
                    continue;
                }

                areaRangeSet.Add(CreateAreaRangeFrom(prevData, startIndex, i));
                prevData = currentData;
                startIndex = i;
            }
            if (prevData != null)
            {
                areaRangeSet.Add(CreateAreaRangeFrom(prevData, startIndex, ohbikiSpanNum));
            }

            foreach (var areaRange in areaRangeSet)
            {
                areaRange.Calculate(areaRangeSet, materials, kodaiData, inverse, slopeInputData);
            }

            return areaRangeSet.ToArray();
        }

        class SlopeData
        {
            public AreaRangeSloped SlopeRange { get; set; }
            public ReferencePlane Plane { get; set; }
            public XYZ AxisOrigin { get; set; }
            public Transform Rotation { get; set; }
        }

        abstract class AreaRange
        {
            public FrmCreateSlopeViewModel.StyleRow _StyleRow { get; set; }
            public FrmCreateSlopeViewModel.SlopeStyle _Type { get; set; }
            public int _StartOhbikiK { get; set; }
            public int _EndOhbikiK { get; set; }
            public OhbikiBottomLevel[] _OhbikiBottoms { get; set; }

            public virtual void Calculate(IEnumerable<AreaRange> unionSet, IEnumerable<MaterialSuper> materials, AllKoudaiFlatFrmData kodaiData, bool inverse, KodaiSlopeModel slopeInputData) { }
        }

        class AreaRangeFlat : AreaRange
        {
            public double _OhbikiBottom { get; set; }
            public override void Calculate(IEnumerable<AreaRange> unionSet, IEnumerable<MaterialSuper> materials, AllKoudaiFlatFrmData kodaiData, bool inverse, KodaiSlopeModel slopeInputData)
            {
                _OhbikiBottom = _StyleRow.Level;
                _OhbikiBottoms = Enumerable.Range(_StartOhbikiK, _EndOhbikiK - _StartOhbikiK + 1)
                    .Select(x => new OhbikiBottomLevel { _K = x, _Z = _OhbikiBottom })
                    .ToArray();
            }
        }

        class AreaRangeSloped : AreaRange
        {
            //public OhbikiSuper _RotateOrigin { get; set; }
            public Line _RotateAxis { get; set; }
            public double _Radian { get; set; }
            public override void Calculate(IEnumerable<AreaRange> unionSet, IEnumerable<MaterialSuper> materials, AllKoudaiFlatFrmData kodaiData, bool inverse, KodaiSlopeModel slopeInputData)
            {
                var flatAreas = unionSet
                    .Select(x => x as AreaRangeFlat)
                    .Where(x => x != null)
                    .ToArray();

                var startSideFlat = flatAreas.FirstOrDefault(x => x._EndOhbikiK == _StartOhbikiK);
                var endSideFlat = flatAreas.FirstOrDefault(x => x._StartOhbikiK == _EndOhbikiK);

                var axisIsStart = true;

                if (startSideFlat == null && endSideFlat != null)
                    axisIsStart = false;
                else if (startSideFlat != null && endSideFlat == null)
                    axisIsStart = true;
                else if (startSideFlat != null && endSideFlat != null)
                    // 高い方を回転軸とする
                    axisIsStart = (startSideFlat._StyleRow.Level > endSideFlat._StyleRow.Level);
                else
                    // 全スパンのスロープ
                    axisIsStart = slopeInputData.Direction == SlopeDirection.StartToEnd;

                var axisSide = axisIsStart ? startSideFlat : endSideFlat;

                var axisIndex = axisIsStart ? _StartOhbikiK : _EndOhbikiK;
                var axisOhbiki = materials.Select(x => x as OhbikiSuper).OrderByDescending(x=>x?.Position.Z).FirstOrDefault(x => x?.m_K == axisIndex);
                if(axisOhbiki.MaterialSize().Shape==MaterialShape.C|| axisOhbiki.MaterialSize().Shape == MaterialShape.L)
                {
                    if (axisIndex == _StartOhbikiK)
                    {
                        axisOhbiki = materials.Select(x => x as OhbikiSuper).OrderByDescending(x => x?.Position.Z).FirstOrDefault(x => x?.m_K == axisIndex && x?.m_AttachSide == "F");
                    }
                    else
                    {
                        axisOhbiki = materials.Select(x => x as OhbikiSuper).OrderByDescending(x => x?.Position.Z).FirstOrDefault(x => x?.m_K == axisIndex && x?.m_AttachSide == "B");
                    }
                }
                var axisOhbikiSize = axisOhbiki?.SteelSize;
                var axisOhbikiWidthFeet = axisOhbikiSize?.FrangeWidthFeet ?? 0.0;

                var axisLevel = ClsRevitUtil.CovertToAPI(axisSide?._StyleRow.Level ?? 0.0);

                var doc = axisOhbiki.m_Document;

                var axisH = kodaiData.GetHukuinVec(doc);
                var axisK = kodaiData.GetKyoutyouVec(doc);
                var axisZ = kodaiData.GetZVec(doc);

                var ohbikiPt = axisOhbiki.Position;
                var signAxisK = axisIsStart ? 1 : -1;
                var levelValue = inverse ? 0.0 : axisLevel;
                var rotateAxisOrigin = ohbikiPt
                    + signAxisK * 0.5 * axisOhbikiWidthFeet * axisK
                    + (0.5 * axisOhbikiSize.HeightFeet + levelValue) * axisZ;

                if(!kodaiData.ohbikiData.isHkou)
                {
                    var p= materials.Select(x => x as PilePillerSuper).OrderByDescending(x => x?.Position.Z).FirstOrDefault(x => x?.m_KyoutyouNum== axisIndex);
                    if(p!=null)
                    {
                        ohbikiPt = p.Position;
                        var ms = p.MaterialSize();
                        FamilyInstance insP = doc.GetElement(p.m_ElementId) as FamilyInstance;
                        if (insP.Symbol.LookupParameter(DefineUtil.PARAM_PILE_CUT_LENG) !=null)
                        {
                            var cutL = insP.Symbol.LookupParameter(DefineUtil.PARAM_PILE_CUT_LENG).AsDouble();
                            ohbikiPt = p.Position - XYZ.BasisZ * cutL+ XYZ.BasisZ*(axisLevel>0?0.5 *axisLevel:0);
                        }
                        
                        signAxisK = axisIsStart ? 1 : -1;
                        levelValue = inverse ? 0.0 : axisLevel;
                        rotateAxisOrigin = ohbikiPt
                        + signAxisK * 0.5 * ClsRevitUtil.CovertToAPI(ms.Width) * axisK
                        + (0.5 * +levelValue) * axisZ;
                    }
                }

                _RotateAxis = Line.CreateUnbound(rotateAxisOrigin, axisH);

                if (_StyleRow.SlopeType == FrmCreateSlopeViewModel.SlopeStyle.Slope)
                {
                    if (startSideFlat != null && endSideFlat != null)
                    {
                        var ohbikiPitch = /*ClsRevitUtil.CovertToAPI*/(kodaiData.KyoutyouPillarPitch.Skip(1).Skip(_StartOhbikiK).Take(_EndOhbikiK - _StartOhbikiK).Sum());
                        var hypotenuse = ohbikiPitch - 0.5 * (axisOhbikiSize?.FrangeWidth ?? 0.0);//) axisOhbikiWidthFeet;
                        var height = /*ClsRevitUtil.CovertToAPI*/(Math.Abs(startSideFlat._StyleRow.Level - endSideFlat._StyleRow.Level));
                        var sin = height / hypotenuse;
                        var theta = Math.Asin(sin);
                        _Radian = axisIsStart ? -theta : theta;
                        var a = _Radian * 180.0 / Math.PI;

                        if (!kodaiData.ohbikiData.isHkou)
                        {
                            var p = materials.Select(x => x as PilePillerSuper).OrderByDescending(x => x?.Position.Z).FirstOrDefault(x => x?.m_KyoutyouNum == axisIndex);
                            if (p != null)
                            {
                                ohbikiPitch = (kodaiData.KyoutyouPillarPitch.Skip(1).Skip(_StartOhbikiK).Take(_EndOhbikiK - _StartOhbikiK).Sum());
                                hypotenuse = ohbikiPitch - 0.5 * (p?.MaterialSize().Height ?? 0.0);
                                height =(Math.Abs(startSideFlat._StyleRow.Level - endSideFlat._StyleRow.Level));
                                sin = height / hypotenuse;
                                theta = Math.Asin(sin);
                                _Radian = axisIsStart ? -theta : theta;
                            }
                        }
                    }
                    else
                    {
                        _Radian = 0.0;
                    }
                }
                else if (_StyleRow.SlopeType == FrmCreateSlopeViewModel.SlopeStyle.Entrance)
                {
                    var tan = Math.Abs(1e-2 * _StyleRow.Percent);
                    var theta = Math.Atan(tan);
                    _Radian = axisIsStart ? -theta : theta;
                }
                else
                {
                    _Radian = 0.0;
                }


                // _StartOhbikiK ... _EndOhbikiK の大引下端レベルの算出
                var baseOhbikiLevel = (axisIsStart ? startSideFlat : endSideFlat)?._StyleRow.Level ?? 0.0;
                var positionKSet = Enumerable.Range(0, kodaiData.KyoutyouPillarPitch.Count())
                    .Select(x => kodaiData.KyoutyouPillarPitch.Skip(1).Take(x).Sum())
                    .ToArray();
                var basePositionK = positionKSet.ElementAtOrDefault(axisIndex);
                var tanAbsTheta = Math.Tan(Math.Abs(_Radian));

                // 下がった側の
                var isSlope = _StyleRow.SlopeType == FrmCreateSlopeViewModel.SlopeStyle.Slope;
                var lowerEndK = axisIsStart ? _EndOhbikiK : _StartOhbikiK;

                _OhbikiBottoms = Enumerable.Range(_StartOhbikiK, _EndOhbikiK - _StartOhbikiK + 1)
                    .Where(x => !(isSlope && x == lowerEndK))
                    .Select(x =>
                    {
                        var positionK = positionKSet.ElementAtOrDefault(x);
                        var pitchFromBase = Math.Abs(positionK - basePositionK);
                        var depthFromBase = pitchFromBase * tanAbsTheta;
                        return new OhbikiBottomLevel
                        {
                            _K = x,
                            _Z = baseOhbikiLevel - depthFromBase
                        };
                    }).ToArray();


                //var a = kodaiData.BaseLevel;
                //var b = kodaiData.LevelOffset;

                //var p = axisOhbiki.Position();

                //var c = kodaiData.SelectedLevel;
                //var baseLevel = new FilteredElementCollector(materials.FirstOrDefault().m_Document)
                //    .OfClass(typeof(Level))
                //    .Cast<Level>()
                //    .FirstOrDefault(x => x.Name == kodaiData.SelectedLevel);


            }
        }

        class OhbikiBottomLevel
        {
            public int _K { get; set; }
            public double _Z { get; set; }
        }
    }
}
