using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YMS_gantry.Master;
using YMS_gantry.Material;
using YMS_gantry.UI;

namespace YMS_gantry.Command
{
    class CmdLengthChange
    {
        static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private const string COMMAND_NAME = "長さ変更";

        Document _doc { get; set; }
        UIDocument _uiDoc { get; set; }

        public CmdLengthChange(UIDocument uiDoc)
        {
            _uiDoc = uiDoc;
            _doc = _uiDoc.Document;
        }

        public Result Excute()
        {
            FrmEditLengthChange f = Application.thisApp.frmEditLengthChange;

            var resultElementIdList = new List<ElementId>();

            try
            {
                using (TransactionGroup tr = new TransactionGroup(_doc))
                {
                    tr.Start("Length Change");

                    foreach (DataGridViewRow row in f.DgvEditLengthChangeList.Rows)
                    {
                        var afterLengthCell = row.Cells["ColLengthChangeListAfterLength"] as DataGridViewTextBoxCell;
                        var afterLength = afterLengthCell.Value?.ToString();

                        if (!string.IsNullOrEmpty(afterLength))
                        {
                            var elementIdCell = row.Cells["ColElementID"] as DataGridViewTextBoxCell;
                            var elementIdValue = (int)elementIdCell.Value;
                            var elementId = new ElementId(elementIdValue);

                            var element = _doc.GetElement(elementId);
                            if (element == null) continue;

                            // ファミリ名取得
                            var familyName = GantryUtil.GetFamilyName(element);
                            // タイプ名取得
                            var typeName = GantryUtil.GetTypeName(element);
                            // ファミリインスタンス取得
                            var familyInstance = GantryUtil.GetFamilyInstance(_doc, element.Id);
                            // ファミリシンボル取得
                            var familySymbol = ClsRevitUtil.GetFamilySymbol(_doc, familyName, typeName);

                            var material = MaterialSuper.ReadFromElement(familyInstance.Id, _uiDoc.Document);
                            if (material == null) continue;

                            // 大元のタイプ名を取得する
                            var originalType = GantryUtil.GetOriginalTypeName(_doc, familyInstance);

                            string retSt = "";
                            //頭に[部材種類]_～がついている場合は除く
                            int ub = familyName.IndexOf('_');
                            retSt = familyName;
                            if (ub != -1)
                            {
                                retSt = familyName.Remove(0, ub + 1);
                            }

                            var resultElementId = element.Id;

                            if (retSt.Contains("TC300"))
                            {
                                // 定形サイズの対傾構の場合は長さ変更処理をおこなわない
                                continue;
                            }

                            if ((retSt.Contains("HA") || retSt.Contains("SMH")) && retSt.Contains("_"))
                            {
                                // 対象が山留材(仮鋼材でない)の場合は、「長さ」パラメータの値変更ではなく、該当長さのファミリを置き直しとなる
                                // ファミリ名にHAまたはSMHを含む、かつ、"_"を含む(末尾に長さ値がある)場合に仮鋼材でない山留材と認識する

                                var sizeTrim = retSt.Substring(0, retSt.IndexOf('_'));
                                var length = double.Parse(afterLength) / 1000;
                                var afterSize = $"{sizeTrim}_{length.ToString("0.0")}";

                                var afterFamilyPath = string.Empty;

                                if (originalType == "支柱")
                                {
                                    if (retSt.Contains("HA"))
                                    {
                                        afterFamilyPath = ClsWaritsukeCsv.GetShicuFamilyPath(afterSize, "山留主材");
                                    }
                                    else if (retSt.Contains("SMH"))
                                    {
                                        afterFamilyPath = ClsWaritsukeCsv.GetShicuFamilyPath(afterSize, "高強度山留主材");
                                    }
                                }
                                else
                                {
                                    if (retSt.Contains("HA"))
                                    {
                                        afterFamilyPath = ClsWaritsukeCsv.GetFamilyPath(afterSize, "山留主材");
                                    }
                                    else if (retSt.Contains("SMH"))
                                    {
                                        afterFamilyPath = ClsWaritsukeCsv.GetFamilyPath(afterSize, "高強度山留主材");
                                    }
                                }

                                if (string.IsNullOrEmpty(afterFamilyPath)) continue;

                                GantryUtil.GetFamilySymbol(_doc, afterFamilyPath, originalType, out FamilySymbol afterFamilySymbol, false);

                                using (Transaction rnTr = new Transaction(_doc))
                                {
                                    rnTr.Start("Rename Type");

                                    afterFamilySymbol = GantryUtil.DuplicateTypeWithNameRule(_doc, material.m_KodaiName, afterFamilySymbol, originalType, typeName);

                                    rnTr.Commit();
                                }

                                var createdId = ElementId.InvalidElementId;

                                using (Transaction rpTr = new Transaction(_doc))
                                {
                                    rpTr.Start("Replace Element");

                                    createdId = ReplaceYamadomeKouzai(_doc, familyInstance, material, afterFamilySymbol);
                                    AttachMaterialSuper(_doc, createdId, material, $"{sizeTrim}_{length.ToString("0.0")}");

                                    rpTr.Commit();
                                }

                                var afterFamilyInstance = GantryUtil.GetFamilyInstance(_doc, createdId);
                                ReplaceReferenceDestinationElement(familyInstance, material, afterFamilyInstance);

                                using (Transaction mvTr = new Transaction(_doc))
                                {
                                    mvTr.Start("Move Element");

                                    // 長さ変更が両端に対しておこなわれたように見せる為、部材の位置を変更する
                                    if (f.ChkLengthChangeIsBothEnd.Checked)
                                    {
                                        var beforeLength = row.Cells["ColLengthChangeListBeforeLength"].Value.ToString();

                                        if (originalType == "支柱")
                                        {
                                            GantryUtil.SizeEditMove(_doc, createdId, double.Parse(beforeLength), double.Parse(afterLength), true, true);
                                        }
                                        else
                                        {
                                            GantryUtil.SizeEditMove(_doc, createdId, double.Parse(beforeLength), double.Parse(afterLength), true, false);
                                        }
                                    }

                                    mvTr.Commit();
                                }

                                resultElementId = createdId;
                                var createdElement = _doc.GetElement(createdId);

                                resultElementIdList.Remove(element.Id);
                                f._targetElementUniqueIdList.Remove(element.UniqueId);
                                f._targetElementUniqueIdList.Add(createdElement.UniqueId);

                                using (Transaction dlTr = new Transaction(_doc))
                                {
                                    dlTr.Start("Delete Element");
                                    _doc.Delete(elementId);
                                    dlTr.Commit();
                                }
                            }
                            else
                            {
                                // タイプを複製
                                familySymbol = GantryUtil.DuplicateTypeWithNameRule(_doc, material.m_KodaiName, familySymbol, originalType);
                                // 複製したタイプで差し替え
                                familySymbol = ClsRevitUtil.ChangeTypeID(_doc, familySymbol, element.Id, familySymbol.Name);

                                // ファミリパラメータから長さを取得
                                var lengthByParam = ClsRevitUtil.GetParameter(_doc, element.Id, DefineUtil.PARAM_LENGTH);
                                // タイプパラメータから長さを取得
                                var lengthByTypeParam = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(familySymbol, DefineUtil.PARAM_LENGTH)).ToString();

                                // Revit単位系に変換
                                var convLength = ClsRevitUtil.CovertToAPI(double.Parse(afterLength));

                                using (Transaction clTr = new Transaction(_doc))
                                {
                                    clTr.Start("Change Length");

                                    // 長さ変更
                                    if (!string.IsNullOrEmpty(lengthByParam))
                                    {
                                        ClsRevitUtil.SetParameter(_doc, element.Id, DefineUtil.PARAM_LENGTH, convLength);
                                    }
                                    else if (!string.IsNullOrEmpty(lengthByTypeParam))
                                    {
                                        ClsRevitUtil.SetTypeParameter(familySymbol, DefineUtil.PARAM_LENGTH, convLength);
                                    }

                                    clTr.Commit();
                                }

                                using (Transaction mvTr = new Transaction(_doc))
                                {
                                    mvTr.Start("Move Element");

                                    var beforeLength = row.Cells["ColLengthChangeListBeforeLength"].Value.ToString();

                                    // 長さ変更が両端に対しておこなわれたように見せる為、部材の位置を変更する
                                    if (f.ChkLengthChangeIsBothEnd.Checked)
                                    {
                                        if (originalType == "手摺支柱")
                                        {
                                            GantryUtil.SizeEditMove(_doc, element.Id, double.Parse(beforeLength), double.Parse(afterLength), false, true);
                                        }
                                        else if (originalType == "支持杭" || originalType == "継ぎ足し杭" || originalType == "支柱")
                                        {
                                            GantryUtil.SizeEditMove(_doc, element.Id, double.Parse(beforeLength), double.Parse(afterLength), true, true);
                                        }
                                        else
                                        {
                                            GantryUtil.SizeEditMove(_doc, element.Id, double.Parse(beforeLength), double.Parse(afterLength), true, false);
                                        }

                                        var _Length = (double.Parse(afterLength) - double.Parse(beforeLength)) / 2;

                                        if (material.GetType() == typeof(Neda))
                                        {
                                            ((Neda)material).m_ExStartLen += _Length;
                                            ((Neda)material).m_ExEndLen += _Length;
                                            Neda.WriteToElement(material, _doc);
                                        }
                                        else if (material.GetType() == typeof(Ohbiki))
                                        {
                                            ((Ohbiki)material).m_ExStartLen += _Length;
                                            ((Ohbiki)material).m_ExEndLen += _Length;
                                            Ohbiki.WriteToElement(material, _doc);
                                        }
                                        else if (material.GetType() == typeof(Shikigeta))
                                        {
                                            ((Shikigeta)material).m_ExStartLen += _Length;
                                            ((Shikigeta)material).m_ExEndLen += _Length;
                                            Shikigeta.WriteToElement(material, _doc);
                                        }
                                    }
                                    else
                                    {
                                        var _Length = double.Parse(afterLength) - double.Parse(beforeLength);

                                        if (material.GetType() == typeof(Neda))
                                        {
                                            ((Neda)material).m_ExEndLen += _Length;
                                            Neda.WriteToElement(material, _doc);
                                        }
                                        else if (material.GetType() == typeof(Ohbiki))
                                        {
                                            ((Ohbiki)material).m_ExEndLen += _Length;
                                            Ohbiki.WriteToElement(material, _doc);
                                        }
                                        else if (material.GetType() == typeof(Shikigeta))
                                        {
                                            ((Shikigeta)material).m_ExEndLen += _Length;
                                            Shikigeta.WriteToElement(material, _doc);
                                        }
                                    }

                                    mvTr.Commit();
                                }
                            }

                            resultElementIdList.Add(resultElementId);
                        }
                    }

                    tr.Assimilate();
                }
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }
            catch (Exception ex)
            {
                MessageUtil.Error($"長さ変更に失敗しました。", COMMAND_NAME, f);
                _logger.Error(ex.Message);
                return Result.Failed;
            }

            // 長さ変更データグリッドビューのレコードを更新
            f.Search();
            //f.Activate();

            return Result.Succeeded;
        }

        private ElementId ReplaceYamadomeKouzai(Document doc, FamilyInstance beforeFamilyInstance, MaterialSuper beforeMaterialSuper, FamilySymbol afterFamilySymbol, Reference reference = null)
        {
            // 変更前の部材の基準レベルとオフセット値を取得
            double offset = 0;

            if (reference == null)
            {
                var level = GantryUtil.GetInstanceLevelAndOffset(_doc, beforeFamilyInstance, ref offset);
                if (level == null)
                {
                    reference = beforeFamilyInstance.HostFace;
                }
                else
                {
                    reference = level.GetPlaneReference();
                }
            }

            offset = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetParameterDouble(doc, beforeFamilyInstance.Id, DefineUtil.PARAM_HOST_OFFSET));

            // タイプパラメータから材質を取得
            var materialByTypeParam = ClsRevitUtil.GetTypeParameterString(beforeFamilyInstance.Symbol, DefineUtil.PARAM_MATERIAL);
            if (!string.IsNullOrEmpty(materialByTypeParam))
            {
                // 材質を変更後サイズのタイプパラメータに転写
                ClsRevitUtil.SetTypeParameter(afterFamilySymbol, DefineUtil.PARAM_MATERIAL, materialByTypeParam);
            }

            if (beforeMaterialSuper.GetType() == typeof(FukkouGeta)
                || beforeMaterialSuper.GetType() == typeof(HorizontalBrace)
                || beforeMaterialSuper.GetType() == typeof(HorizontalJoint)
                || beforeMaterialSuper.GetType() == typeof(VerticalBrace)
                || (beforeMaterialSuper.GetType() == typeof(Houdue) && ((Houdue)beforeMaterialSuper).m_Syuzai)
                || (beforeMaterialSuper.GetType() == typeof(JifukuZuredomezai) && ((JifukuZuredomezai)beforeMaterialSuper).m_Size.StartsWith("["))
                || beforeMaterialSuper.GetType() == typeof(Madumezai)
                || beforeMaterialSuper.GetType() == typeof(Taikeikou)
                || beforeMaterialSuper.GetType() == typeof(Tesuri))
            {
                // 変更前の部材の端点を取得
                Curve cvBase = GantryUtil.GetCurve(_doc, beforeFamilyInstance.Id);

                // 変更前の部材の始点終点を取得
                XYZ tmpStPoint = cvBase.GetEndPoint(0);
                XYZ tmpEdPoint = cvBase.GetEndPoint(1);

                // タイプパラメータから回転を取得
                var rotateByTypeParam = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(beforeFamilyInstance.Symbol, DefineUtil.PARAM_ROTATE)).ToString();
                if (!string.IsNullOrEmpty(rotateByTypeParam))
                {
                    // Revit単位系に変換
                    var convRotate = ClsRevitUtil.CovertToAPI(double.Parse(rotateByTypeParam));
                    // 回転を変更後サイズのタイプパラメータに転写
                    ClsRevitUtil.SetTypeParameter(afterFamilySymbol, DefineUtil.PARAM_ROTATE, convRotate);
                }

                return MaterialSuper.PlaceWithTwoPoints(afterFamilySymbol, reference, tmpStPoint, tmpEdPoint, ClsRevitUtil.CovertToAPI(offset));
            }


            if (beforeMaterialSuper.GetType() == typeof(TakasaChouseizai))
            {
                // 高さ調整プレートはサイズ(可変)が1種類しかないのでサイズ変更されることがなさそうだが一応

                var wByTypeParam = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(beforeFamilyInstance.Symbol, "W")).ToString();
                var dByTypeParam = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(beforeFamilyInstance.Symbol, "D")).ToString();
                var h1ByTypeParam = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(beforeFamilyInstance.Symbol, "H1")).ToString();
                var h2ByTypeParam = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(beforeFamilyInstance.Symbol, "H2")).ToString();

                if (!string.IsNullOrEmpty(wByTypeParam))
                {
                    var convW = ClsRevitUtil.CovertToAPI(double.Parse(wByTypeParam));
                    ClsRevitUtil.SetTypeParameter(afterFamilySymbol, "W", convW);
                }

                if (!string.IsNullOrEmpty(dByTypeParam))
                {
                    var convD = ClsRevitUtil.CovertToAPI(double.Parse(dByTypeParam));
                    ClsRevitUtil.SetTypeParameter(afterFamilySymbol, "D", convD);
                }

                if (!string.IsNullOrEmpty(h1ByTypeParam))
                {
                    var convH1 = ClsRevitUtil.CovertToAPI(double.Parse(h1ByTypeParam));
                    ClsRevitUtil.SetTypeParameter(afterFamilySymbol, "H1", convH1);
                }

                if (!string.IsNullOrEmpty(h2ByTypeParam))
                {
                    var convH2 = ClsRevitUtil.CovertToAPI(double.Parse(h2ByTypeParam));
                    ClsRevitUtil.SetTypeParameter(afterFamilySymbol, "H2", convH2);
                }
            }
            else if (beforeMaterialSuper.GetType() == typeof(HojoPieace))
            {
                var h1ByTypeParam = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(beforeFamilyInstance.Symbol, "H1")).ToString();
                var h2ByTypeParam = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(beforeFamilyInstance.Symbol, "H2")).ToString();
                //var topWByTypeParam = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(beforeFamilyInstance.Symbol, "エンドプレート上W")).ToString();
                //var topDByTypeParam = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(beforeFamilyInstance.Symbol, "エンドプレート上D")).ToString();
                //var topHByTypeParam = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(beforeFamilyInstance.Symbol, "エンドプレート上厚さ")).ToString();
                //var topKindByTypeParam = GantryUtil.GetTypeParameterElementId(beforeFamilyInstance.Symbol, DefineUtil.PARAM_END_PLATE_SIZE_U);
                //var endWByTypeParam = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(beforeFamilyInstance.Symbol, "エンドプレート下W")).ToString();
                //var endDByTypeParam = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(beforeFamilyInstance.Symbol, "エンドプレート下D")).ToString();
                //var endHByTypeParam = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(beforeFamilyInstance.Symbol, "エンドプレート下厚さ")).ToString();
                //var endKindByTypeParam = GantryUtil.GetTypeParameterElementId(beforeFamilyInstance.Symbol, DefineUtil.PARAM_END_PLATE_SIZE_B);

                if (!string.IsNullOrEmpty(h1ByTypeParam))
                {
                    var convH1 = ClsRevitUtil.CovertToAPI(double.Parse(h1ByTypeParam));
                    ClsRevitUtil.SetTypeParameter(afterFamilySymbol, "H1", convH1);
                }

                if (!string.IsNullOrEmpty(h2ByTypeParam))
                {
                    var convH2 = ClsRevitUtil.CovertToAPI(double.Parse(h2ByTypeParam));
                    ClsRevitUtil.SetTypeParameter(afterFamilySymbol, "H2", convH2);
                }

                //if (!string.IsNullOrEmpty(topWByTypeParam))
                //{
                //    var convTopW = ClsRevitUtil.CovertToAPI(double.Parse(topWByTypeParam));
                //    ClsRevitUtil.SetTypeParameter(afterFamilySymbol, "エンドプレート上W", convTopW);
                //}

                //if (!string.IsNullOrEmpty(topDByTypeParam))
                //{
                //    var convTopD = ClsRevitUtil.CovertToAPI(double.Parse(topDByTypeParam));
                //    ClsRevitUtil.SetTypeParameter(afterFamilySymbol, "エンドプレート上D", convTopD);
                //}

                //if (!string.IsNullOrEmpty(topHByTypeParam))
                //{
                //    var convTopH = ClsRevitUtil.CovertToAPI(double.Parse(topHByTypeParam));
                //    ClsRevitUtil.SetTypeParameter(afterFamilySymbol, "エンドプレート上厚さ", convTopH);
                //}

                //if (topKindByTypeParam != null)
                //{
                //    GantryUtil.SetSymbolParameterValueByParameterName(afterFamilySymbol, DefineUtil.PARAM_END_PLATE_SIZE_U, topKindByTypeParam.IntegerValue.ToString());
                //}

                //if (!string.IsNullOrEmpty(endWByTypeParam))
                //{
                //    var convEndW = ClsRevitUtil.CovertToAPI(double.Parse(endWByTypeParam));
                //    ClsRevitUtil.SetTypeParameter(afterFamilySymbol, "エンドプレート下W", convEndW);
                //}

                //if (!string.IsNullOrEmpty(endDByTypeParam))
                //{
                //    var convEndD = ClsRevitUtil.CovertToAPI(double.Parse(endDByTypeParam));
                //    ClsRevitUtil.SetTypeParameter(afterFamilySymbol, "エンドプレート下D", convEndD);
                //}

                //if (!string.IsNullOrEmpty(endHByTypeParam))
                //{
                //    var convEndH = ClsRevitUtil.CovertToAPI(double.Parse(endHByTypeParam));
                //    ClsRevitUtil.SetTypeParameter(afterFamilySymbol, "エンドプレート下厚さ", convEndH);
                //}

                //if (endKindByTypeParam != null)
                //{
                //    GantryUtil.SetSymbolParameterValueByParameterName(afterFamilySymbol, DefineUtil.PARAM_END_PLATE_SIZE_B, endKindByTypeParam.IntegerValue.ToString());
                //}
            }
            else if (beforeMaterialSuper.GetType() == typeof(StiffenerJack))
            {
                var lengthByTypeParam = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(beforeFamilyInstance.Symbol, DefineUtil.PARAM_LENGTH)).ToString();

                if (!string.IsNullOrEmpty(lengthByTypeParam))
                {
                    var convLength = ClsRevitUtil.CovertToAPI(double.Parse(lengthByTypeParam));
                    ClsRevitUtil.SetTypeParameter(afterFamilySymbol, DefineUtil.PARAM_LENGTH, convLength);
                }
            }
            else if (beforeMaterialSuper.GetType() == typeof(TesuriShichu))
            {
                var lengthByTypeParam = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(beforeFamilyInstance.Symbol, DefineUtil.PARAM_LENGTH)).ToString();

                if (!string.IsNullOrEmpty(lengthByTypeParam))
                {
                    var convLength = ClsRevitUtil.CovertToAPI(double.Parse(lengthByTypeParam));
                    ClsRevitUtil.SetTypeParameter(afterFamilySymbol, DefineUtil.PARAM_LENGTH, convLength);
                }
            }
            else if (beforeMaterialSuper.GetType() == typeof(PilePiller))
            {
                //var lengthByTypeParam = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(beforeFamilyInstance.Symbol, DefineUtil.PARAM_LENGTH)).ToString();
                var rotateByTypeParam = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(beforeFamilyInstance.Symbol, DefineUtil.PARAM_ROTATE)).ToString();

                var jointCountByTypeParam = GantryUtil.GetTypeParameter(beforeFamilyInstance.Symbol, DefineUtil.PARAM_JOINT_COUNT, isValue: true).ToString();
                var pileCutLengByTypeParam = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(beforeFamilyInstance.Symbol, DefineUtil.PARAM_PILE_CUT_LENG)).ToString();

                //var jointTypeByTypeParam = GantryUtil.GetTypeParameterElementId(beforeFamilyInstance.Symbol, DefineUtil.PARAM_JOINT_TYPE);
                var useTopPLByTypeParam = GantryUtil.GetTypeParameter(beforeFamilyInstance.Symbol, DefineUtil.PARAM_TOP_PLATE, isValue: true);
                //var topPLSizeByTypeParam = GantryUtil.GetTypeParameterElementId(beforeFamilyInstance.Symbol, DefineUtil.PARAM_TOP_PLATE_SIZE);

                var useBasePLByTypeParam = GantryUtil.GetTypeParameter(beforeFamilyInstance.Symbol, DefineUtil.PARAM_BASE_PLATE, isValue: true);
                //var basePLSizeByTypeParam = GantryUtil.GetTypeParameterElementId(beforeFamilyInstance.Symbol, DefineUtil.PARAM_BASE_PLATE_SIZE);

                var topPLWByTypeParam = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(beforeFamilyInstance.Symbol, DefineUtil.PARAM_TOP_PLATE_W)).ToString();
                var topPLDByTypeParam = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(beforeFamilyInstance.Symbol, DefineUtil.PARAM_TOP_PLATE_D)).ToString();
                var topPLTByTypeParam = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(beforeFamilyInstance.Symbol, DefineUtil.PARAM_TOP_PLATE_T)).ToString();

                var kuiByTypeParamList = new List<string>();
                for (int i = 0; i < 10; i++)
                {
                    kuiByTypeParamList.Add(ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(beforeFamilyInstance.Symbol, $"杭{i + 1}")).ToString());
                }

                var shichuByTypeParamList = new List<string>();
                for (int i = 0; i < 10; i++)
                {
                    shichuByTypeParamList.Add(ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(beforeFamilyInstance.Symbol, $"{i + 1}段目長さ")).ToString());
                }

                //if (!string.IsNullOrEmpty(lengthByTypeParam))
                //{
                //    var convLength = ClsRevitUtil.CovertToAPI(double.Parse(lengthByTypeParam));
                //    ClsRevitUtil.SetTypeParameter(afterFamilySymbol, DefineUtil.PARAM_LENGTH, convLength);
                //}

                if (!string.IsNullOrEmpty(rotateByTypeParam))
                {
                    var convRotate = ClsRevitUtil.CovertToAPI(double.Parse(rotateByTypeParam));
                    ClsRevitUtil.SetTypeParameter(afterFamilySymbol, DefineUtil.PARAM_ROTATE, convRotate);
                }

                if (!string.IsNullOrEmpty(jointCountByTypeParam))
                {
                    GantryUtil.SetSymbolParameterValueByParameterName(afterFamilySymbol, DefineUtil.PARAM_JOINT_COUNT, jointCountByTypeParam);
                }

                if (!string.IsNullOrEmpty(pileCutLengByTypeParam))
                {
                    var convPileCutLengt = ClsRevitUtil.CovertToAPI(double.Parse(pileCutLengByTypeParam));
                    ClsRevitUtil.SetTypeParameter(afterFamilySymbol, DefineUtil.PARAM_PILE_CUT_LENG, convPileCutLengt);
                }

                //if (jointTypeByTypeParam != null)
                //{
                //    GantryUtil.SetSymbolParameterValueByParameterName(afterFamilySymbol, DefineUtil.PARAM_JOINT_TYPE, jointTypeByTypeParam.IntegerValue.ToString());
                //}

                if (!string.IsNullOrEmpty(useTopPLByTypeParam))
                {
                    GantryUtil.SetSymbolParameterValueByParameterName(afterFamilySymbol, DefineUtil.PARAM_TOP_PLATE, useTopPLByTypeParam == "はい" ? ((int)DefineUtil.PramYesNo.Yes).ToString() : ((int)DefineUtil.PramYesNo.No).ToString());
                }

                //if (topPLSizeByTypeParam != null)
                //{
                //    GantryUtil.SetSymbolParameterValueByParameterName(afterFamilySymbol, DefineUtil.PARAM_TOP_PLATE_SIZE, topPLSizeByTypeParam.IntegerValue.ToString());
                //}

                if (!string.IsNullOrEmpty(useBasePLByTypeParam))
                {
                    GantryUtil.SetSymbolParameterValueByParameterName(afterFamilySymbol, DefineUtil.PARAM_BASE_PLATE, useBasePLByTypeParam == "はい" ? ((int)DefineUtil.PramYesNo.Yes).ToString() : ((int)DefineUtil.PramYesNo.No).ToString());
                }

                //if (basePLSizeByTypeParam != null)
                //{
                //    GantryUtil.SetSymbolParameterValueByParameterName(afterFamilySymbol, DefineUtil.PARAM_BASE_PLATE_SIZE, basePLSizeByTypeParam.IntegerValue.ToString());
                //}

                if (!string.IsNullOrEmpty(topPLWByTypeParam))
                {
                    var convTopPLW = ClsRevitUtil.CovertToAPI(double.Parse(topPLWByTypeParam));
                    ClsRevitUtil.SetTypeParameter(afterFamilySymbol, DefineUtil.PARAM_TOP_PLATE_W, convTopPLW);
                }

                if (!string.IsNullOrEmpty(topPLDByTypeParam))
                {
                    var convTopPLD = ClsRevitUtil.CovertToAPI(double.Parse(topPLDByTypeParam));
                    ClsRevitUtil.SetTypeParameter(afterFamilySymbol, DefineUtil.PARAM_TOP_PLATE_D, convTopPLD);
                }

                if (!string.IsNullOrEmpty(topPLTByTypeParam))
                {
                    var convTopPLT = ClsRevitUtil.CovertToAPI(double.Parse(topPLTByTypeParam));
                    ClsRevitUtil.SetTypeParameter(afterFamilySymbol, DefineUtil.PARAM_TOP_PLATE_T, convTopPLT);
                }

                for (int i = 0; i < 10; i++)
                {
                    var kuiTypeParam = kuiByTypeParamList[i];
                    var convKui = ClsRevitUtil.CovertToAPI(double.Parse(kuiTypeParam));
                    ClsRevitUtil.SetTypeParameter(afterFamilySymbol, $"杭{i + 1}", convKui);
                }

                for (int i = 0; i < 10; i++)
                {
                    var shichuTypeParam = shichuByTypeParamList[i];
                    var convShichu = ClsRevitUtil.CovertToAPI(double.Parse(shichuTypeParam));
                    ClsRevitUtil.SetTypeParameter(afterFamilySymbol, $"{i + 1}段目長さ", convShichu);
                }
            }
            else if (beforeMaterialSuper.GetType() == typeof(ExtensionPile))
            {
                var lengthByTypeParam = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(beforeFamilyInstance.Symbol, DefineUtil.PARAM_LENGTH)).ToString();
                var rotateByTypeParam = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(beforeFamilyInstance.Symbol, DefineUtil.PARAM_ROTATE)).ToString();

                var jointCountByTypeParam = GantryUtil.GetTypeParameter(beforeFamilyInstance.Symbol, DefineUtil.PARAM_JOINT_COUNT, isValue: true).ToString();
                //var jointTypeByTypeParam = GantryUtil.GetTypeParameterElementId(beforeFamilyInstance.Symbol, DefineUtil.PARAM_JOINT_TYPE);
                var useTopPLByTypeParam = GantryUtil.GetTypeParameter(beforeFamilyInstance.Symbol, DefineUtil.PARAM_TOP_PLATE, isValue: true);
                //var topPLSizeByTypeParam = GantryUtil.GetTypeParameterElementId(beforeFamilyInstance.Symbol, DefineUtil.PARAM_TOP_PLATE_SIZE);
                var topPLTByTypeParam = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(beforeFamilyInstance.Symbol, DefineUtil.PARAM_TOP_PLATE_T)).ToString();

                if (!string.IsNullOrEmpty(lengthByTypeParam))
                {
                    var convLength = ClsRevitUtil.CovertToAPI(double.Parse(lengthByTypeParam));
                    ClsRevitUtil.SetTypeParameter(afterFamilySymbol, DefineUtil.PARAM_LENGTH, convLength);
                }

                if (!string.IsNullOrEmpty(rotateByTypeParam))
                {
                    var convRotate = ClsRevitUtil.CovertToAPI(double.Parse(rotateByTypeParam));
                    ClsRevitUtil.SetTypeParameter(afterFamilySymbol, DefineUtil.PARAM_ROTATE, convRotate);
                }

                if (!string.IsNullOrEmpty(jointCountByTypeParam))
                {
                    GantryUtil.SetSymbolParameterValueByParameterName(afterFamilySymbol, DefineUtil.PARAM_JOINT_COUNT, jointCountByTypeParam);
                }

                //if (jointTypeByTypeParam != null)
                //{
                //    GantryUtil.SetSymbolParameterValueByParameterName(afterFamilySymbol, DefineUtil.PARAM_JOINT_TYPE, jointTypeByTypeParam.IntegerValue.ToString());
                //}

                if (!string.IsNullOrEmpty(useTopPLByTypeParam))
                {
                    GantryUtil.SetSymbolParameterValueByParameterName(afterFamilySymbol, DefineUtil.PARAM_TOP_PLATE, useTopPLByTypeParam == "はい" ? ((int)DefineUtil.PramYesNo.Yes).ToString() : ((int)DefineUtil.PramYesNo.No).ToString());
                }

                //if (topPLSizeByTypeParam != null)
                //{
                //    GantryUtil.SetSymbolParameterValueByParameterName(afterFamilySymbol, DefineUtil.PARAM_TOP_PLATE_SIZE, topPLSizeByTypeParam.IntegerValue.ToString());
                //}

                if (!string.IsNullOrEmpty(topPLTByTypeParam))
                {
                    var convTopPLT = ClsRevitUtil.CovertToAPI(double.Parse(topPLTByTypeParam));
                    ClsRevitUtil.SetTypeParameter(afterFamilySymbol, DefineUtil.PARAM_TOP_PLATE_T, convTopPLT);
                }
            }

            return GantryUtil.CreateInstanceWith1point(doc, beforeMaterialSuper.Position, reference, afterFamilySymbol, beforeFamilyInstance.HandOrientation)?.Id;
        }

        /// <summary>
        /// 配置部材に部材情報を付与する
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id"></param>
        /// <param name="koudaiName"></param>
        /// <param name="materialSuper"></param>
        /// <param name="size"></param>
        private void AttachMaterialSuper(Document doc, ElementId id, MaterialSuper materialSuper, string size)
        {
            if (materialSuper.GetType() == typeof(Fukkouban))
            {
                var orgFukkouban = (Fukkouban)materialSuper;

                Fukkouban newMs = new Fukkouban();
                newMs.m_ElementId = id;
                newMs.m_KodaiName = orgFukkouban.m_KodaiName; ;
                newMs.m_Size = size;
                newMs.m_Material = orgFukkouban.m_Material;
                newMs.m_hukuinNum = orgFukkouban.m_hukuinNum;
                newMs.m_kyoutyouNum = orgFukkouban.m_kyoutyouNum;
                newMs.m_TeiketsuType = orgFukkouban.m_TeiketsuType;
                newMs.m_Bolt1Cnt = orgFukkouban.m_Bolt1Cnt;
                newMs.m_Bolt2Cnt = orgFukkouban.m_Bolt2Cnt;
                newMs.m_BoltInfo1 = orgFukkouban.m_BoltInfo1;
                newMs.m_BoltInfo2 = orgFukkouban.m_BoltInfo2;
                Fukkouban.WriteToElement(newMs, doc);
            }
            else if (materialSuper.GetType() == typeof(Neda))
            {
                var orgNeda = (Neda)materialSuper;

                Neda newMs = new Neda();
                newMs.m_ElementId = id;
                newMs.m_KodaiName = orgNeda.m_KodaiName;
                newMs.m_Size = size;
                newMs.m_Material = orgNeda.m_Material;
                newMs.m_ExStartLen = orgNeda.m_ExStartLen;
                newMs.m_ExEndLen = orgNeda.m_ExEndLen;
                newMs.m_H = orgNeda.m_H;
                newMs.m_MinK = orgNeda.m_MinK;
                newMs.m_MaxK = orgNeda.m_MaxK;
                newMs.m_TeiketsuType = orgNeda.m_TeiketsuType;
                newMs.m_Bolt1Cnt = orgNeda.m_Bolt1Cnt;
                newMs.m_Bolt2Cnt = orgNeda.m_Bolt2Cnt;
                newMs.m_BoltInfo1 = orgNeda.m_BoltInfo1;
                newMs.m_BoltInfo2 = orgNeda.m_BoltInfo2;
                Neda.WriteToElement(newMs, doc);
            }
            else if (materialSuper.GetType() == typeof(Ohbiki))
            {
                var orgOhbiki = (Ohbiki)materialSuper;

                Ohbiki newMs = new Ohbiki();
                newMs.m_ElementId = id;
                newMs.m_KodaiName = orgOhbiki.m_KodaiName; ;
                newMs.m_Size = size;
                newMs.m_Material = orgOhbiki.m_Material;
                newMs.m_K = orgOhbiki.m_K;
                newMs.m_ExStartLen = orgOhbiki.m_ExStartLen;
                newMs.m_TeiketsuType = orgOhbiki.m_TeiketsuType;
                newMs.m_Bolt1Cnt = orgOhbiki.m_Bolt1Cnt;
                newMs.m_Bolt2Cnt = orgOhbiki.m_Bolt2Cnt;
                newMs.m_BoltInfo1 = orgOhbiki.m_BoltInfo1;
                newMs.m_BoltInfo2 = orgOhbiki.m_BoltInfo2;
                Ohbiki.WriteToElement(newMs, doc);
            }
            else if (materialSuper.GetType() == typeof(Shikigeta))
            {
                var orgShikigeta = (Shikigeta)materialSuper;

                Shikigeta newMs = new Shikigeta();
                newMs.m_ElementId = id;
                newMs.m_KodaiName = orgShikigeta.m_KodaiName;
                newMs.m_Size = size;
                newMs.m_Material = orgShikigeta.m_Material;
                newMs.m_K = orgShikigeta.m_K;
                newMs.m_ExStartLen = orgShikigeta.m_ExStartLen;
                newMs.m_TeiketsuType = orgShikigeta.m_TeiketsuType;
                newMs.m_Bolt1Cnt = orgShikigeta.m_Bolt1Cnt;
                newMs.m_Bolt2Cnt = orgShikigeta.m_Bolt2Cnt;
                newMs.m_BoltInfo1 = orgShikigeta.m_BoltInfo1;
                newMs.m_BoltInfo2 = orgShikigeta.m_BoltInfo2;
                Shikigeta.WriteToElement(newMs, doc);
            }
            else if (materialSuper.GetType() == typeof(FukkouGeta))
            {
                var orgFukkouGeta = (FukkouGeta)materialSuper;

                FukkouGeta newMs = new FukkouGeta();
                newMs.m_ElementId = id;
                newMs.m_KodaiName = orgFukkouGeta.m_KodaiName;
                newMs.m_Size = size;
                newMs.m_Material = orgFukkouGeta.m_Material;
                newMs.m_TeiketsuType = orgFukkouGeta.m_TeiketsuType;
                newMs.m_Bolt1Cnt = orgFukkouGeta.m_Bolt1Cnt;
                newMs.m_Bolt2Cnt = orgFukkouGeta.m_Bolt2Cnt;
                newMs.m_BoltInfo1 = orgFukkouGeta.m_BoltInfo1;
                newMs.m_BoltInfo2 = orgFukkouGeta.m_BoltInfo2;
                FukkouGeta.WriteToElement(newMs, doc);
            }
            else if (materialSuper.GetType() == typeof(TeiketsuHojo))
            {
                var orgTeiketsuHojo = (TeiketsuHojo)materialSuper;

                TeiketsuHojo newMs = new TeiketsuHojo();
                newMs.m_ElementId = id;
                newMs.m_KodaiName = orgTeiketsuHojo.m_KodaiName;
                newMs.m_Size = size;
                newMs.m_Material = orgTeiketsuHojo.m_Material;
                newMs.m_TeiketsuType = orgTeiketsuHojo.m_TeiketsuType;
                newMs.m_Bolt1Cnt = orgTeiketsuHojo.m_Bolt1Cnt;
                newMs.m_Bolt2Cnt = orgTeiketsuHojo.m_Bolt2Cnt;
                newMs.m_BoltInfo1 = orgTeiketsuHojo.m_BoltInfo1;
                newMs.m_BoltInfo2 = orgTeiketsuHojo.m_BoltInfo2;
                TeiketsuHojo.WriteToElement(newMs, doc);
            }
            else if (materialSuper.GetType() == typeof(Shikiteppan))
            {
                var orgShikiteppan = (Shikiteppan)materialSuper;

                Shikiteppan newMs = new Shikiteppan();
                newMs.m_ElementId = id;
                newMs.m_KodaiName = orgShikiteppan.m_KodaiName; ;
                newMs.m_Size = size;
                newMs.m_Material = orgShikiteppan.m_Material;
                newMs.m_TeiketsuType = orgShikiteppan.m_TeiketsuType;
                newMs.m_Bolt1Cnt = orgShikiteppan.m_Bolt1Cnt;
                newMs.m_Bolt2Cnt = orgShikiteppan.m_Bolt2Cnt;
                newMs.m_BoltInfo1 = orgShikiteppan.m_BoltInfo1;
                newMs.m_BoltInfo2 = orgShikiteppan.m_BoltInfo2;
                Shikiteppan.WriteToElement(newMs, doc);
            }
            else if (materialSuper.GetType() == typeof(Shimakouhan))
            {
                var orgShimakouhan = (Shimakouhan)materialSuper;

                Shimakouhan newMs = new Shimakouhan();
                newMs.m_ElementId = id;
                newMs.m_KodaiName = orgShimakouhan.m_KodaiName; ;
                newMs.m_Size = size;
                newMs.m_Material = orgShimakouhan.m_Material;
                newMs.m_TeiketsuType = orgShimakouhan.m_TeiketsuType;
                newMs.m_Bolt1Cnt = orgShimakouhan.m_Bolt1Cnt;
                newMs.m_Bolt2Cnt = orgShimakouhan.m_Bolt2Cnt;
                newMs.m_BoltInfo1 = orgShimakouhan.m_BoltInfo1;
                newMs.m_BoltInfo2 = orgShimakouhan.m_BoltInfo2;
                Shimakouhan.WriteToElement(newMs, doc);
            }
            else if (materialSuper.GetType() == typeof(TakasaChouseizai))
            {
                var orgTakasaChouseizai = (TakasaChouseizai)materialSuper;

                TakasaChouseizai newMs = new TakasaChouseizai();
                newMs.m_ElementId = id;
                newMs.m_KodaiName = orgTakasaChouseizai.m_KodaiName; ;
                newMs.m_Size = size;
                newMs.m_Material = orgTakasaChouseizai.m_Material;
                newMs.m_W = orgTakasaChouseizai.m_W;
                newMs.m_D = orgTakasaChouseizai.m_D;
                newMs.m_H1 = orgTakasaChouseizai.m_H1;
                newMs.m_H2 = orgTakasaChouseizai.m_H2;
                newMs.m_TeiketsuType = orgTakasaChouseizai.m_TeiketsuType;
                newMs.m_Bolt1Cnt = orgTakasaChouseizai.m_Bolt1Cnt;
                newMs.m_Bolt2Cnt = orgTakasaChouseizai.m_Bolt2Cnt;
                newMs.m_BoltInfo1 = orgTakasaChouseizai.m_BoltInfo1;
                newMs.m_BoltInfo2 = orgTakasaChouseizai.m_BoltInfo2;
                TakasaChouseizai.WriteToElement(newMs, doc);
            }
            else if (materialSuper.GetType() == typeof(HojoPieace))
            {
                var orgHojoPieace = (HojoPieace)materialSuper;

                HojoPieace newMs = new HojoPieace();
                newMs.m_ElementId = id;
                newMs.m_KodaiName = orgHojoPieace.m_KodaiName; ;
                newMs.m_Size = size;
                newMs.m_Material = orgHojoPieace.m_Material;
                newMs.m_H1 = orgHojoPieace.m_H1;
                newMs.m_H2 = orgHojoPieace.m_H2;
                newMs.m_TopW = orgHojoPieace.m_TopW;
                newMs.m_TopD = orgHojoPieace.m_TopD;
                newMs.m_TopH = orgHojoPieace.m_TopH;
                newMs.m_TopType = orgHojoPieace.m_TopType;
                newMs.m_EndW = orgHojoPieace.m_EndW;
                newMs.m_EndD = orgHojoPieace.m_EndD;
                newMs.m_EndH = orgHojoPieace.m_EndH;
                newMs.m_EndType = orgHojoPieace.m_EndType;
                newMs.m_TeiketsuType = orgHojoPieace.m_TeiketsuType;
                newMs.m_Bolt1Cnt = orgHojoPieace.m_Bolt1Cnt;
                newMs.m_Bolt2Cnt = orgHojoPieace.m_Bolt2Cnt;
                newMs.m_BoltInfo1 = orgHojoPieace.m_BoltInfo1;
                newMs.m_BoltInfo2 = orgHojoPieace.m_BoltInfo2;
                HojoPieace.WriteToElement(newMs, doc);
            }
            else if (materialSuper.GetType() == typeof(HorizontalBrace))
            {
                var orgHorizontalBrace = (HorizontalBrace)materialSuper;

                HorizontalBrace newMs = new HorizontalBrace();
                newMs.m_ElementId = id;
                newMs.m_KodaiName = orgHorizontalBrace.m_KodaiName;
                newMs.m_Size = size;
                newMs.m_Material = orgHorizontalBrace.m_Material;
                newMs.m_TeiketsuType = orgHorizontalBrace.m_TeiketsuType;
                newMs.m_Bolt1Cnt = orgHorizontalBrace.m_Bolt1Cnt;
                newMs.m_Bolt2Cnt = orgHorizontalBrace.m_Bolt2Cnt;
                newMs.m_BoltInfo1 = orgHorizontalBrace.m_BoltInfo1;
                newMs.m_BoltInfo2 = orgHorizontalBrace.m_BoltInfo2;
                HorizontalBrace.WriteToElement(newMs, doc);
            }
            else if (materialSuper.GetType() == typeof(HorizontalJoint))
            {
                var orgHorizontalJoint = (HorizontalJoint)materialSuper;

                HorizontalJoint newMs = new HorizontalJoint();
                newMs.m_ElementId = id;
                newMs.m_KodaiName = orgHorizontalJoint.m_KodaiName;
                newMs.m_Size = size;
                newMs.m_Material = orgHorizontalJoint.m_Material;
                newMs.m_TeiketsuType = orgHorizontalJoint.m_TeiketsuType;
                newMs.m_Bolt1Cnt = orgHorizontalJoint.m_Bolt1Cnt;
                newMs.m_Bolt2Cnt = orgHorizontalJoint.m_Bolt2Cnt;
                newMs.m_BoltInfo1 = orgHorizontalJoint.m_BoltInfo1;
                newMs.m_BoltInfo2 = orgHorizontalJoint.m_BoltInfo2;
                HorizontalJoint.WriteToElement(newMs, doc);
            }
            else if (materialSuper.GetType() == typeof(VerticalBrace))
            {
                var orgVerticalBrace = (VerticalBrace)materialSuper;

                VerticalBrace newMs = new VerticalBrace();
                newMs.m_ElementId = id;
                newMs.m_KodaiName = orgVerticalBrace.m_KodaiName;
                newMs.m_Size = size;
                newMs.m_Material = orgVerticalBrace.m_Material;
                newMs.m_TeiketsuType = orgVerticalBrace.m_TeiketsuType;
                newMs.m_Bolt1Cnt = orgVerticalBrace.m_Bolt1Cnt;
                newMs.m_Bolt2Cnt = orgVerticalBrace.m_Bolt2Cnt;
                newMs.m_BoltInfo1 = orgVerticalBrace.m_BoltInfo1;
                newMs.m_BoltInfo2 = orgVerticalBrace.m_BoltInfo2;
                VerticalBrace.WriteToElement(newMs, doc);
            }
            else if (materialSuper.GetType() == typeof(Houdue))
            {
                var orgHoudue = (Houdue)materialSuper;

                Houdue newMs = new Houdue();
                newMs.m_ElementId = id;
                newMs.m_KodaiName = orgHoudue.m_KodaiName;
                newMs.m_Size = size;
                newMs.m_Material = orgHoudue.m_Material;
                newMs.m_Syuzai = orgHoudue.m_Syuzai;
                newMs.m_TeiketsuType = orgHoudue.m_TeiketsuType;
                newMs.m_Bolt1Cnt = orgHoudue.m_Bolt1Cnt;
                newMs.m_Bolt2Cnt = orgHoudue.m_Bolt2Cnt;
                newMs.m_BoltInfo1 = orgHoudue.m_BoltInfo1;
                newMs.m_BoltInfo2 = orgHoudue.m_BoltInfo2;
                Houdue.WriteToElement(newMs, doc);
            }
            else if (materialSuper.GetType() == typeof(Jack))
            {
                var orgJack = (Jack)materialSuper;

                Jack newMs = new Jack();
                newMs.m_ElementId = id;
                newMs.m_KodaiName = orgJack.m_KodaiName;
                newMs.m_Size = size;
                newMs.m_Material = orgJack.m_Material;
                newMs.m_TeiketsuType = orgJack.m_TeiketsuType;
                newMs.m_Bolt1Cnt = orgJack.m_Bolt1Cnt;
                newMs.m_Bolt2Cnt = orgJack.m_Bolt2Cnt;
                newMs.m_BoltInfo1 = orgJack.m_BoltInfo1;
                newMs.m_BoltInfo2 = orgJack.m_BoltInfo2;
                Jack.WriteToElement(newMs, doc);
            }
            else if (materialSuper.GetType() == typeof(JifukuZuredomezai))
            {
                var orgJifukuZuredomezai = (JifukuZuredomezai)materialSuper;

                JifukuZuredomezai newMs = new JifukuZuredomezai();
                newMs.m_ElementId = id;
                newMs.m_KodaiName = orgJifukuZuredomezai.m_KodaiName;
                newMs.m_Size = size;
                newMs.m_Material = orgJifukuZuredomezai.m_Material;
                newMs.m_TeiketsuType = orgJifukuZuredomezai.m_TeiketsuType;
                newMs.m_Bolt1Cnt = orgJifukuZuredomezai.m_Bolt1Cnt;
                newMs.m_Bolt2Cnt = orgJifukuZuredomezai.m_Bolt2Cnt;
                newMs.m_BoltInfo1 = orgJifukuZuredomezai.m_BoltInfo1;
                newMs.m_BoltInfo2 = orgJifukuZuredomezai.m_BoltInfo2;
                JifukuZuredomezai.WriteToElement(newMs, doc);
            }
            else if (materialSuper.GetType() == typeof(Madumezai))
            {
                var orgMadumezai = (Madumezai)materialSuper;

                Madumezai newMs = new Madumezai();
                newMs.m_ElementId = id;
                newMs.m_KodaiName = orgMadumezai.m_KodaiName;
                newMs.m_Size = size;
                newMs.m_Material = orgMadumezai.m_Material;
                newMs.m_K = orgMadumezai.m_K;
                newMs.m_TeiketsuType = orgMadumezai.m_TeiketsuType;
                newMs.m_Bolt1Cnt = orgMadumezai.m_Bolt1Cnt;
                newMs.m_Bolt2Cnt = orgMadumezai.m_Bolt2Cnt;
                newMs.m_BoltInfo1 = orgMadumezai.m_BoltInfo1;
                newMs.m_BoltInfo2 = orgMadumezai.m_BoltInfo2;
                Madumezai.WriteToElement(newMs, doc);
            }
            else if (materialSuper.GetType() == typeof(PilePiller))
            {
                var orgPilePiller = (PilePiller)materialSuper;

                PilePiller newMs = new PilePiller();
                newMs.m_ElementId = id;
                newMs.m_KodaiName = orgPilePiller.m_KodaiName;
                newMs.m_Size = size;
                newMs.m_Material = orgPilePiller.m_Material;
                newMs.m_KyoutyouNum = orgPilePiller.m_KyoutyouNum;
                newMs.m_HukuinNum = orgPilePiller.m_HukuinNum;
                newMs.m_TeiketsuType = orgPilePiller.m_TeiketsuType;
                newMs.m_Bolt1Cnt = orgPilePiller.m_Bolt1Cnt;
                newMs.m_Bolt2Cnt = orgPilePiller.m_Bolt2Cnt;
                newMs.m_BoltInfo1 = orgPilePiller.m_BoltInfo1;
                newMs.m_BoltInfo2 = orgPilePiller.m_BoltInfo2;
                PilePiller.WriteToElement(newMs, doc);
            }
            else if (materialSuper.GetType() == typeof(StiffenerJack))
            {
                var orgStiffenerJack = (StiffenerJack)materialSuper;

                StiffenerJack newMs = new StiffenerJack();
                newMs.m_ElementId = id;
                newMs.m_KodaiName = orgStiffenerJack.m_KodaiName;
                newMs.m_Size = size;
                newMs.m_Material = orgStiffenerJack.m_Material;
                newMs.m_H = orgStiffenerJack.m_H;
                newMs.m_K = orgStiffenerJack.m_K;
                newMs.m_TeiketsuType = orgStiffenerJack.m_TeiketsuType;
                newMs.m_Bolt1Cnt = orgStiffenerJack.m_Bolt1Cnt;
                newMs.m_Bolt2Cnt = orgStiffenerJack.m_Bolt2Cnt;
                newMs.m_BoltInfo1 = orgStiffenerJack.m_BoltInfo1;
                newMs.m_BoltInfo2 = orgStiffenerJack.m_BoltInfo2;
                StiffenerJack.WriteToElement(newMs, doc);
            }
            else if (materialSuper.GetType() == typeof(StiffenerPlate))
            {
                var orgStiffenerPlate = (StiffenerPlate)materialSuper;

                StiffenerPlate newMs = new StiffenerPlate();
                newMs.m_ElementId = id;
                newMs.m_KodaiName = orgStiffenerPlate.m_KodaiName;
                newMs.m_Size = size;
                newMs.m_Material = orgStiffenerPlate.m_Material;
                newMs.m_H = orgStiffenerPlate.m_H;
                newMs.m_K = orgStiffenerPlate.m_K;
                newMs.m_TeiketsuType = orgStiffenerPlate.m_TeiketsuType;
                newMs.m_Bolt1Cnt = orgStiffenerPlate.m_Bolt1Cnt;
                newMs.m_Bolt2Cnt = orgStiffenerPlate.m_Bolt2Cnt;
                newMs.m_BoltInfo1 = orgStiffenerPlate.m_BoltInfo1;
                newMs.m_BoltInfo2 = orgStiffenerPlate.m_BoltInfo2;
                StiffenerPlate.WriteToElement(newMs, doc);
            }
            else if (materialSuper.GetType() == typeof(Taikeikou))
            {
                var orgTaikeikou = (Taikeikou)materialSuper;

                Taikeikou newMs = new Taikeikou();
                newMs.m_ElementId = id;
                newMs.m_KodaiName = orgTaikeikou.m_KodaiName;
                newMs.m_Size = size;
                newMs.m_Material = orgTaikeikou.m_Material;
                newMs.m_JointType = orgTaikeikou.m_JointType;
                newMs.m_BoltCount = orgTaikeikou.m_BoltCount;
                newMs.m_BoltType = orgTaikeikou.m_BoltType;
                newMs.m_TeiketsuType = orgTaikeikou.m_TeiketsuType;
                newMs.m_Bolt1Cnt = orgTaikeikou.m_Bolt1Cnt;
                newMs.m_Bolt2Cnt = orgTaikeikou.m_Bolt2Cnt;
                newMs.m_BoltInfo1 = orgTaikeikou.m_BoltInfo1;
                newMs.m_BoltInfo2 = orgTaikeikou.m_BoltInfo2;
                Taikeikou.WriteToElement(newMs, doc);
            }
            else if (materialSuper.GetType() == typeof(Tensetsuban))
            {
                var orgTensetsuban = (Tensetsuban)materialSuper;

                Tensetsuban newMs = new Tensetsuban();
                newMs.m_ElementId = id;
                newMs.m_KodaiName = orgTensetsuban.m_KodaiName;
                newMs.m_Size = size;
                newMs.m_Material = orgTensetsuban.m_Material;
                newMs.m_H = orgTensetsuban.m_H;
                newMs.m_K = orgTensetsuban.m_K;
                newMs.m_TeiketsuType = orgTensetsuban.m_TeiketsuType;
                newMs.m_Bolt1Cnt = orgTensetsuban.m_Bolt1Cnt;
                newMs.m_Bolt2Cnt = orgTensetsuban.m_Bolt2Cnt;
                newMs.m_BoltInfo1 = orgTensetsuban.m_BoltInfo1;
                newMs.m_BoltInfo2 = orgTensetsuban.m_BoltInfo2;
                Tensetsuban.WriteToElement(newMs, doc);
            }
            else if (materialSuper.GetType() == typeof(Tesuri))
            {
                var orgTesuri = (Tesuri)materialSuper;

                Tesuri newMs = new Tesuri();
                newMs.m_ElementId = id;
                newMs.m_KodaiName = orgTesuri.m_KodaiName;
                newMs.m_Size = size;
                newMs.m_Material = orgTesuri.m_Material;
                newMs.m_TeiketsuType = orgTesuri.m_TeiketsuType;
                newMs.m_Bolt1Cnt = orgTesuri.m_Bolt1Cnt;
                newMs.m_Bolt2Cnt = orgTesuri.m_Bolt2Cnt;
                newMs.m_BoltInfo1 = orgTesuri.m_BoltInfo1;
                newMs.m_BoltInfo2 = orgTesuri.m_BoltInfo2;
                Tesuri.WriteToElement(newMs, doc);
            }
            else if (materialSuper.GetType() == typeof(TesuriShichu))
            {
                var orgTesuriShichu = (TesuriShichu)materialSuper;

                TesuriShichu newMs = new TesuriShichu();
                newMs.m_ElementId = id;
                newMs.m_KodaiName = orgTesuriShichu.m_KodaiName;
                newMs.m_Size = size;
                newMs.m_Material = orgTesuriShichu.m_Material;
                newMs.m_TeiketsuType = orgTesuriShichu.m_TeiketsuType;
                newMs.m_Bolt1Cnt = orgTesuriShichu.m_Bolt1Cnt;
                newMs.m_Bolt2Cnt = orgTesuriShichu.m_Bolt2Cnt;
                newMs.m_BoltInfo1 = orgTesuriShichu.m_BoltInfo1;
                newMs.m_BoltInfo2 = orgTesuriShichu.m_BoltInfo2;
                TesuriShichu.WriteToElement(newMs, doc);
            }
            else if (materialSuper.GetType() == typeof(ExtensionPile))
            {
                var orgExtensionPile = (ExtensionPile)materialSuper;

                ExtensionPile newMs = new ExtensionPile();
                newMs.m_ElementId = id;
                newMs.m_KodaiName = orgExtensionPile.m_KodaiName;
                newMs.m_Size = size;
                newMs.m_Material = orgExtensionPile.m_Material;
                newMs.m_HukuinNum = orgExtensionPile.m_HukuinNum;
                newMs.m_KyoutyouNum = orgExtensionPile.m_KyoutyouNum;
                newMs.m_TeiketsuType = orgExtensionPile.m_TeiketsuType;
                newMs.m_Bolt1Cnt = orgExtensionPile.m_Bolt1Cnt;
                newMs.m_Bolt2Cnt = orgExtensionPile.m_Bolt2Cnt;
                newMs.m_BoltInfo1 = orgExtensionPile.m_BoltInfo1;
                newMs.m_BoltInfo2 = orgExtensionPile.m_BoltInfo2;
                ExtensionPile.WriteToElement(newMs, doc);
            }
        }

        /// <summary>
        /// 指定部材を参照先としている部材を参照先を変更して再配置
        /// </summary>
        /// <param name="beforeFamilyInstance">再配置前の参照先</param>
        /// <param name="material">部材情報</param>
        /// <param name="afterFamilyInstance">再配置後の参照先</param>
        private void ReplaceReferenceDestinationElement(FamilyInstance beforeFamilyInstance, MaterialSuper material, FamilyInstance afterFamilyInstance, List<FamilyInstance> list = null)
        {
            List<FamilyInstance> chirdFiList = null;
            if (list == null)
            {
                FilteredElementCollector collector = new FilteredElementCollector(_doc);
                collector.OfCategory(BuiltInCategory.OST_GenericModel);
                chirdFiList = collector.OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().Where(x => x.Host != null && x.Host.Id == beforeFamilyInstance.Id).ToList();
            }
            else
            {
                chirdFiList = list;
            }

            var afterFiMaterial = MaterialSuper.ReadFromElement(afterFamilyInstance.Id, _doc);
            if (afterFiMaterial == null) return;

            Transform ts = beforeFamilyInstance.GetTotalTransform();

            foreach (var fi in chirdFiList)
            {
                Reference reference = fi.HostFace;
                var _face = GantryUtil.IdentifyFaceOfFamilyInstance(beforeFamilyInstance, reference);
                XYZ transformedNormal = ts.OfVector(_face.ComputeNormal(new UV(0, 0)));

                var face = GantryUtil.GetSpecifyFaceOfFamilyInstance(afterFamilyInstance, transformedNormal);
                if (face == null) continue;

                reference = face.Reference;

                var _material = MaterialSuper.ReadFromElement(fi.Id, _doc);
                if (_material == null) continue;

                ElementId createdId = null;
                if (material.m_KodaiName == material.m_KodaiName)
                {
                    var typeName = fi.Symbol.Name;
                    var familyName = fi.Symbol.Family.Name;

                    if (_material.GetType() == typeof(StiffenerPlate))
                    {
                        var size = ClsStiffenerCsv.GetSize(afterFiMaterial.m_Size);
                        if (!string.IsNullOrEmpty(size))
                        {
                            familyName = ClsStiffenerCsv.GetFamilyPath(afterFiMaterial.m_Size, "プレート");
                            typeName = ClsStiffenerCsv.GetFamilyType(size);
                        }
                    }
                    else if (_material.GetType() == typeof(StiffenerJack))
                    {
                        var _type = string.Empty;
                        if (!string.IsNullOrEmpty(familyName) && familyName.Contains("SH"))
                        {
                            _type = "SHジャッキ";
                        }
                        else if (!string.IsNullOrEmpty(familyName) && familyName.Contains("DWJ"))
                        {
                            _type = "DWJジャッキ";
                        }

                        if (string.IsNullOrEmpty(_type)) continue;

                        var size = ClsStiffenerCsv.GetSize(afterFiMaterial.m_Size, _type);
                        if (!string.IsNullOrEmpty(size))
                        {
                            familyName = ClsStiffenerCsv.GetFamilyPath(afterFiMaterial.m_Size, _type);
                            typeName = ClsStiffenerCsv.GetFamilyType(size);
                        }
                    }
                    else if (_material.GetType() == typeof(TaikeikouPL))
                    {
                        var _familyPath = ClsTaikeikouPLCsv.GetFamilyPath(afterFiMaterial.m_Size);
                        var _typeName = ClsTaikeikouPLCsv.GetFamilyTypeName(afterFiMaterial.m_Size);
                        if (!string.IsNullOrEmpty(_familyPath) && !string.IsNullOrEmpty(_typeName))
                        {
                            familyName = _familyPath;
                            typeName = _typeName;
                        }
                    }

                    var chirdNewFs = GantryUtil.GetFamilySymbol(_doc, typeName, familyName);
                    if (chirdNewFs == null)
                    {
                        GantryUtil.GetFamilySymbol(_doc, familyName, typeName, out chirdNewFs);
                    }

                    List<FamilyInstance> _chirdFiList = null;
                    using (Transaction tr = new Transaction(_doc))
                    {
                        tr.Start("Replace Element");

                        // 警告を制御する処理（部材を削除する前に同一点に部材を置くとコミット時に警告が出るので対応）
                        FailureHandlingOptions failOpt = tr.GetFailureHandlingOptions();
                        failOpt.SetFailuresPreprocessor(new WarningSwallower());
                        tr.SetFailureHandlingOptions(failOpt);

                        // 置き直す前に置き直す対象を参照している部材を取得
                        FilteredElementCollector _collector = new FilteredElementCollector(_doc);
                        _collector.OfCategory(BuiltInCategory.OST_GenericModel);
                        _chirdFiList = _collector.OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().Where(x => x.Host != null && x.Host.Id == fi.Id).ToList();

                        // 新たな参照部材より、元の参照部材面と同一面を指定して再配置
                        createdId = ReplaceYamadomeKouzai(_doc, fi, _material, chirdNewFs, reference);
                        // 部材情報付与
                        AttachMaterialSuper(_doc, createdId, _material, _material.m_Size);

                        tr.Commit();
                    }

                    var _afterFamilyInstance = GantryUtil.GetFamilyInstance(_doc, createdId);

                    // 再帰的に呼び出す
                    ReplaceReferenceDestinationElement(fi, _material, _afterFamilyInstance, _chirdFiList);

                    using (Transaction tr = new Transaction(_doc))
                    {
                        tr.Start("Delete Element");

                        _doc.Delete(fi.Id);

                        tr.Commit();
                    }
                }
            }
        }

        private string RemoveRight(string str, string removeStr)
        {
            var length = str.IndexOf(removeStr);
            if (length < 0)
            {
                return str;
            }

            return str.Substring(0, length);
        }

        /// <summary>
        /// 警告表示の制御(無視できる警告が表示されなくなる)
        /// </summary>
        /// <remarks>下記のように使う
        /// t.Start();
        ///FailureHandlingOptions failOpt = t.GetFailureHandlingOptions();
        ///failOpt.SetFailuresPreprocessor(new WarningSwallower());
        ///t.SetFailureHandlingOptions(failOpt);</remarks>
        public class WarningSwallower : IFailuresPreprocessor
        {
            public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
            {
                IList<FailureMessageAccessor> failList = new List<FailureMessageAccessor>();
                failList = failuresAccessor.GetFailureMessages();
                foreach (FailureMessageAccessor failure in failList)
                {
                    FailureDefinitionId failID = failure.GetFailureDefinitionId();
                    failuresAccessor.DeleteWarning(failure);
                }

                return FailureProcessingResult.Continue;
            }
        }
    }
}
