using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.IO ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;
using YMS_gantry.Master ;
using YMS_gantry.Material ;
using YMS_gantry.UI ;

namespace YMS_gantry.Command
{
  class CmdSizeChange
  {
    static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger() ;

    private const string COMMAND_NAME = "サイズ変更" ;

    private const string TOP_PL_OPTIONAL_NAME = "ﾄｯﾌﾟﾌﾟﾚｰﾄ_任意" ;
    private const string BASE_PL_TYPE_STRONG = "最強型" ;
    private const string BASE_PL_TYPE_NORMAL = "標準型" ;
    private const string BASE_PL_TYPE_SIMPLE = "簡易型" ;

    Document _doc { get ; set ; }
    UIDocument _uiDoc { get ; set ; }

    public CmdSizeChange( UIDocument uiDoc )
    {
      _uiDoc = uiDoc ;
      _doc = _uiDoc.Document ;
    }

    public Result ExcuteOfSize()
    {
      FrmEditSizeList f = Application.thisApp.frmEditSizeList ;

      try {
        using ( TransactionGroup tr = new TransactionGroup( _doc ) ) {
          tr.Start( "Size List Change" ) ;

          foreach ( DataGridViewRow row in f.DgvEditSizeList.Rows ) {
            var afterSizeCell = row.Cells[ "ColSizeListAfterSize" ] as DataGridViewComboBoxCell ;
            var afterSize = afterSizeCell.Value?.ToString() ;

            if ( string.IsNullOrEmpty( afterSize ) ) continue ;

            var elementIdCell = row.Cells[ "ColSizeListElementID" ] as DataGridViewTextBoxCell ;
            var elementIdValue = (int) elementIdCell.Value ;
            var elementId = new ElementId( elementIdValue ) ;

            var element = _doc.GetElement( elementId ) ;
            if ( element == null ) continue ;

            // 変更前の部材のファミリ名取得
            var familyName = GantryUtil.GetFamilyName( element ) ;
            // 変更前の部材のタイプ名取得
            var typeName = GantryUtil.GetTypeName( element ) ;
            // 変更前の部材のファミリインスタンス取得
            var familyInstance = GantryUtil.GetFamilyInstance( _doc, element.Id ) ;
            // 変更前の部材のファミリシンボル取得
            var familySymbol = ClsRevitUtil.GetFamilySymbol( _doc, familyName, typeName ) ;
            // 変更前の部材の部材情報を取得
            var material = MaterialSuper.ReadFromElement( familyInstance.Id, _doc ) ;
            if ( material == null ) continue ;

            // 変更前の部材の大元のタイプ名を取得する
            var originalType = GantryUtil.GetOriginalTypeName( _doc, familyInstance ) ;

            var _path = Master.ClsMasterCsv.GetFamilyPathBySize( afterSize, originalType ) ;
            GantryUtil.GetFamilySymbol( _doc, _path, Path.GetFileNameWithoutExtension( _path ),
              out FamilySymbol afterFamilySymbol, false ) ;

            if ( afterFamilySymbol == null ) {
              var isPile = false ;
              var isPilePiller = false ;
              var isExPile = false ;

              if ( originalType.Contains( "支持杭" ) ) {
                isPile = true ;
              }
              else if ( originalType.Contains( "支柱" ) ) {
                isPilePiller = true ;
              }
              else if ( originalType.Contains( "継ぎ足し杭" ) ) {
                isExPile = true ;
              }

              // 変更後サイズのファミリパス取得
              var afterFamilyPath = Master.ClsMasterCsv.GetFamilyPath( afterSize, bPile: isPile,
                bPilePiller: isPilePiller, bExPile: isExPile ) ;
              if ( originalType.Contains( "高さ調整ﾋﾟｰｽ" ) ) {
                afterFamilyPath = Master.ClsTakasaChouseiCsv.GetFamilyPath( afterSize ) ;
              }

              if ( originalType.Contains( "対傾構" ) ) {
                afterFamilyPath = Master.ClsTaikeikouCsv.GetFamilyPath( afterSize ) ;
              }

              // 変更後サイズのファミリシンボル取得
              GantryUtil.GetFamilySymbol( _doc, afterFamilyPath, originalType, out afterFamilySymbol, false ) ;

              if ( afterFamilySymbol == null ) {
                var _type = Master.ClsMasterCsv.GetFamilyTypeBySize( afterSize ) ;
                if ( string.IsNullOrEmpty( _type ) ) {
                  // タイプが取得できない場合
                  GantryUtil.GetFamilySymbol( _doc, afterFamilyPath, afterSize, out afterFamilySymbol, false ) ;
                  if ( originalType.Contains( "高さ調整ﾋﾟｰｽ" ) ) {
                    var rotate =
                      ClsRevitUtil.CovertFromAPI(
                        ClsRevitUtil.GetTypeParameter( familySymbol, DefineUtil.PARAM_ROTATE ) ) ;
                    if ( rotate == 0 ) {
                      GantryUtil.GetFamilySymbol( _doc, afterFamilyPath, "ﾌﾗﾝｼﾞ面", out afterFamilySymbol, false ) ;
                    }
                    else {
                      GantryUtil.GetFamilySymbol( _doc, afterFamilyPath, "ｳｪﾌﾞ面", out afterFamilySymbol, false ) ;
                    }
                  }

                  if ( originalType.Contains( "対傾構" ) ) {
                    string type = Path.GetFileNameWithoutExtension( afterFamilyPath ) ;
                    GantryUtil.GetFamilySymbol( _doc, afterFamilyPath, type, out afterFamilySymbol, false ) ;
                  }
                }
                else {
                  // 再度変更後サイズのファミリシンボル取得
                  GantryUtil.GetFamilySymbol( _doc, afterFamilyPath, _type, out afterFamilySymbol, false ) ;
                }
              }
            }
            else {
              originalType = Path.GetFileNameWithoutExtension( _path ) ;
            }

            using ( Transaction rnTr = new Transaction( _doc ) ) {
              rnTr.Start( "Rename Type" ) ;
              // 変更後サイズのタイプを複製（タイプ複製が不要な部材に関しては複製しない）
              afterFamilySymbol = DuplicateFamilySymbol( _doc, material, afterFamilySymbol, originalType ) ;
              rnTr.Commit() ;
            }

            ElementId createdID = ElementId.InvalidElementId ;

            using ( Transaction scTr = new Transaction( _doc ) ) {
              scTr.Start( "Size Change" ) ;
              // 警告を制御する処理（部材を削除する前に同一点に部材を置くとコミット時に警告が出るので対応）
              FailureHandlingOptions failOpt = scTr.GetFailureHandlingOptions() ;
              failOpt.SetFailuresPreprocessor( new WarningSwallower() ) ;
              scTr.SetFailureHandlingOptions( failOpt ) ;

              // サイズ変更
              createdID = SizeChange( _doc, familyInstance, material, afterFamilySymbol ) ;
              if ( createdID == null ) continue ;
              scTr.Commit() ;
            }

            using ( Transaction atTr = new Transaction( _doc ) ) {
              atTr.Start( "Attach Material" ) ;
              // 変更後の部材の部材情報を共通パラメータに書き込み
              AttachMaterialSuper( _doc, createdID, material, afterSize ) ;
              atTr.Commit() ;
            }

            // 変更後サイズのファミリインスタンス取得
            var afterFamilyInstance = GantryUtil.GetFamilyInstance( _doc, createdID ) ;
            // 元の部材の連動対象となる部材をサイズ変更
            ReplaceIntersectElement( familyInstance, material, afterFamilyInstance, false ) ;
            // 元の部材を参照していた部材を新たな参照にて置き直し
            ReplaceReferenceDestinationElement( familyInstance, material, afterFamilyInstance ) ;

            // 対象部材一覧データの更新
            var index = f._targetElementUniqueIdList.IndexOf( element.UniqueId ) ;
            f._targetElementUniqueIdList.Remove( element.UniqueId ) ;
            var afterFiElement = _doc.GetElement( afterFamilyInstance.Id ) ;
            f._targetElementUniqueIdList.Insert( index, afterFiElement.UniqueId ) ;

            using ( Transaction dlTr = new Transaction( _doc ) ) {
              dlTr.Start( "Delete Element" ) ;
              // 変更前の部材の削除
              _doc.Delete( elementId ) ;
              dlTr.Commit() ;
            }
          }

          tr.Assimilate() ;
        }
      }
      catch ( Autodesk.Revit.Exceptions.OperationCanceledException ) {
        return Result.Cancelled ;
      }
      catch ( Exception ex ) {
        MessageUtil.Error( $"サイズ変更に失敗しました。", COMMAND_NAME, f ) ;
        _logger.Error( ex.Message ) ;
        return Result.Failed ;
      }

      // 個別一覧データグリッドビューのレコードを更新
      f.SearchSizeList() ;

      return Result.Succeeded ;
    }

    private void ReplaceIntersectElement( FamilyInstance beforeFamilyInstance, MaterialSuper material,
      FamilyInstance afterFamilyInstance, bool isChangeSizeCategory )
    {
      var koudaiName = material.m_KodaiName ;
      var koudaiData = GantryUtil.GetKoudaiData( _doc, koudaiName ) ;
      if ( koudaiData == null ) return ;

      // 変更対象部材に接触(近くに存在)する部材のIDを取得
      // ここの近くの部材を取得する処理は、現状、対傾構取付PLを取得するためだけにあるが、distanceBorderを多めに(デフォ0.1→10)取らないと取得できないケースが存在する
      var nearElementIds = ClsRevitUtil.GetIntersectFamilys( _doc, beforeFamilyInstance.Id, distanceBorder: 10,
        ignorelist: new List<string>() { beforeFamilyInstance.Name } ) ;

      var nearMaterialList = new List<MaterialSuper>() ;
      foreach ( var nearElementId in nearElementIds ) {
        var nearFi = GantryUtil.GetFamilyInstance( _doc, nearElementId ) ;

        // 対象部材のホストになっているものは除外
        if ( nearFi.Host != null && nearFi.Host.Id.IntegerValue == beforeFamilyInstance.Id.IntegerValue ) continue ;

        var nearMaterial = MaterialSuper.ReadFromElement( nearFi.Id, _doc ) ;
        if ( nearMaterial == null ) continue ;

        if ( nearMaterial.GetType() == typeof( TaikeikouPL ) ) {
          // 特定の部材（対象部材と連動してサイズ変更をおこないたいもの）のみ追加
          // 対象部材と連動してサイズ変更させたいが、対象部材をホストとして持っていない部材が対象
          nearMaterialList.Add( nearMaterial ) ;
        }
      }

      var afterFiMaterial = MaterialSuper.ReadFromElement( afterFamilyInstance.Id, _doc ) ;
      if ( afterFiMaterial == null ) return ;

      foreach ( var _material in nearMaterialList ) {
        ElementId createdId = null ;
        if ( material.m_KodaiName == material.m_KodaiName ) {
          var fi = GantryUtil.GetFamilyInstance( _doc, _material.m_ElementId ) ;

          var typeName = fi.Symbol.Name ;
          var familyName = fi.Symbol.Family.Name ;

          if ( _material.GetType() == typeof( TaikeikouPL ) ) {
            var size = afterFiMaterial.m_Size ;
            if ( afterFiMaterial.SteelSize.Shape == SteelShape.HA ||
                 afterFiMaterial.SteelSize.Shape == SteelShape.SMH ) {
              // 対象の桁材が山留仮鋼材の場合
              size = ClsKouzaiSpecify.GetKouzaiSize( afterFiMaterial.m_Size ) ;
            }

            var _familyPath = ClsTaikeikouPLCsv.GetFamilyPath( size ) ;
            var _typeName = ClsTaikeikouPLCsv.GetFamilyTypeName( size ) ;
            if ( ! string.IsNullOrEmpty( _familyPath ) && ! string.IsNullOrEmpty( _typeName ) ) {
              familyName = _familyPath ;
              typeName = _typeName ;
            }
          }

          GantryUtil.GetFamilySymbol( _doc, familyName, typeName, out FamilySymbol chirdNewFs ) ;

          if ( chirdNewFs == null ) continue ;

          using ( Transaction tr = new Transaction( _doc ) ) {
            tr.Start( "Replace Element" ) ;

            // 警告を制御する処理（部材を削除する前に同一点に部材を置くとコミット時に警告が出るので対応）
            FailureHandlingOptions failOpt = tr.GetFailureHandlingOptions() ;
            failOpt.SetFailuresPreprocessor( new WarningSwallower() ) ;
            tr.SetFailureHandlingOptions( failOpt ) ;

            // 新たな参照部材より、元の参照部材面と同一面を指定して再配置
            createdId = SizeChange( _doc, fi, _material, chirdNewFs ) ;
            // 部材情報付与
            AttachMaterialSuper( _doc, createdId, _material, _material.m_Size ) ;

            tr.Commit() ;
          }

          var _afterFamilyInstance = GantryUtil.GetFamilyInstance( _doc, createdId ) ;

          // 種別一覧からのサイズ変更の場合にのみ、部材に対して位置移動をおこなう
          if ( isChangeSizeCategory ) {
            using ( Transaction tr = new Transaction( _doc ) ) {
              tr.Start( "Move Element" ) ;

              var beforeMaterial = MaterialSuper.ReadFromElement( beforeFamilyInstance.Id, _doc ) ;
              var afterMaterial = MaterialSuper.ReadFromElement( afterFamilyInstance.Id, _doc ) ;
              if ( beforeMaterial != null && afterMaterial != null ) {
                var beforeSizeH = beforeMaterial.MaterialSize()?.Height ;
                var afterSizeH = afterMaterial.MaterialSize()?.Height ;

                if ( ! ( ( beforeSizeH == null || beforeSizeH == 0 ) && ( afterSizeH == null || afterSizeH == 0 ) ) ) {
                  var moveValue = Math.Abs( ( beforeSizeH.Value - afterSizeH.Value ) / 2 ) ;

                  var offset = ClsRevitUtil.CovertFromAPI( ClsRevitUtil.GetParameterDouble( _doc,
                    _afterFamilyInstance.Id, DefineUtil.PARAM_HOST_OFFSET ) ) ;
                  var changeOffset = offset + moveValue ;
                  if ( koudaiData.AllKoudaiFlatData.BaseLevel == DefineUtil.eBaseLevel.FukkouTop ) {
                    changeOffset = offset - moveValue ;
                  }

                  if ( beforeSizeH > afterSizeH ) {
                    // 変更前サイズ(H)の方が大きい場合（サイズが小さくなる）
                    changeOffset = offset - moveValue ;
                    if ( koudaiData.AllKoudaiFlatData.BaseLevel == DefineUtil.eBaseLevel.FukkouTop ) {
                      changeOffset = offset + moveValue ;
                    }
                  }

                  ClsRevitUtil.SetParameter( _doc, _afterFamilyInstance.Id, DefineUtil.PARAM_HOST_OFFSET,
                    ClsRevitUtil.CovertToAPI( changeOffset ) ) ;
                }
              }

              tr.Commit() ;
            }
          }

          using ( Transaction tr = new Transaction( _doc ) ) {
            tr.Start( "Delete Element" ) ;

            _doc.Delete( fi.Id ) ;

            tr.Commit() ;
          }
        }
      }
    }

    /// <summary>
    /// 指定部材を参照先としている部材を参照先を変更して再配置
    /// </summary>
    /// <param name="beforeFamilyInstance">再配置前の参照先</param>
    /// <param name="material">部材情報</param>
    /// <param name="afterFamilyInstance">再配置後の参照先</param>
    private void ReplaceReferenceDestinationElement( FamilyInstance beforeFamilyInstance, MaterialSuper material,
      FamilyInstance afterFamilyInstance, List<FamilyInstance> list = null )
    {
      List<FamilyInstance> chirdFiList = null ;
      if ( list == null ) {
        FilteredElementCollector collector = new FilteredElementCollector( _doc ) ;
        collector.OfCategory( BuiltInCategory.OST_GenericModel ) ;
        chirdFiList = collector.OfClass( typeof( FamilyInstance ) ).Cast<FamilyInstance>()
          .Where( x => x.Host != null && x.Host.Id == beforeFamilyInstance.Id ).ToList() ;
      }
      else {
        chirdFiList = list ;
      }

      var afterFiMaterial = MaterialSuper.ReadFromElement( afterFamilyInstance.Id, _doc ) ;
      if ( afterFiMaterial == null ) return ;

      Transform ts = beforeFamilyInstance.GetTotalTransform() ;

      foreach ( var fi in chirdFiList ) {
        Reference reference = fi.HostFace ;
        var _face = GantryUtil.IdentifyFaceOfFamilyInstance( beforeFamilyInstance, reference ) ;
        XYZ transformedNormal = ts.OfVector( _face.ComputeNormal( new UV( 0, 0 ) ) ) ;

        var face = GantryUtil.GetSpecifyFaceOfFamilyInstance( afterFamilyInstance, transformedNormal ) ;
        if ( face == null ) continue ;

        reference = face.Reference ;

        var _material = MaterialSuper.ReadFromElement( fi.Id, _doc ) ;
        if ( _material == null ) continue ;

        ElementId createdId = null ;
        if ( material.m_KodaiName == material.m_KodaiName ) {
          var typeName = fi.Symbol.Name ;
          var familyName = fi.Symbol.Family.Name ;

          if ( _material.GetType() == typeof( StiffenerPlate ) ) {
            if ( afterFiMaterial.SteelSize.Shape == SteelShape.HA ||
                 afterFiMaterial.SteelSize.Shape == SteelShape.SMH ) {
              // 対象の桁材が山留仮鋼材の場合
              var kouzaiSize = ClsKouzaiSpecify.GetKouzaiSize( afterFiMaterial.m_Size ) ;
              if ( ! string.IsNullOrEmpty( kouzaiSize ) ) {
                familyName = ClsStiffenerCsv.GetFamilyPath( kouzaiSize, "プレート" ) ;
                typeName = ClsStiffenerCsv.GetType( kouzaiSize ) ;
              }
            }
            else {
              var size = ClsStiffenerCsv.GetSize( afterFiMaterial.m_Size ) ;
              if ( ! string.IsNullOrEmpty( size ) ) {
                familyName = ClsStiffenerCsv.GetFamilyPath( afterFiMaterial.m_Size, "プレート" ) ;
                typeName = ClsStiffenerCsv.GetFamilyType( size ) ;
              }
            }
          }
          else if ( _material.GetType() == typeof( StiffenerJack ) ) {
            var _type = string.Empty ;
            if ( ! string.IsNullOrEmpty( familyName ) && familyName.Contains( "SH" ) ) {
              _type = "SHジャッキ" ;
            }
            else if ( ! string.IsNullOrEmpty( familyName ) && familyName.Contains( "DWJ" ) ) {
              _type = "DWJジャッキ" ;
            }

            if ( string.IsNullOrEmpty( _type ) ) continue ;

            if ( afterFiMaterial.SteelSize.Shape == SteelShape.HA ||
                 afterFiMaterial.SteelSize.Shape == SteelShape.SMH ) {
              // 対象の桁材が山留仮鋼材の場合
              var kouzaiSize = ClsKouzaiSpecify.GetKouzaiSize( afterFiMaterial.m_Size ) ;
              if ( ! string.IsNullOrEmpty( kouzaiSize ) ) {
                familyName = ClsStiffenerCsv.GetFamilyPath( kouzaiSize, _type ) ;
                typeName = ClsStiffenerCsv.GetType( kouzaiSize, _type ) ;
              }
            }
            else {
              var size = ClsStiffenerCsv.GetSize( afterFiMaterial.m_Size, _type ) ;
              if ( ! string.IsNullOrEmpty( size ) ) {
                familyName = ClsStiffenerCsv.GetFamilyPath( afterFiMaterial.m_Size, _type ) ;
                typeName = ClsStiffenerCsv.GetFamilyType( size ) ;
              }
            }
          }
          else if ( _material.GetType() == typeof( TaikeikouPL ) ) {
            var size = afterFiMaterial.m_Size ;
            if ( afterFiMaterial.SteelSize.Shape == SteelShape.HA ||
                 afterFiMaterial.SteelSize.Shape == SteelShape.SMH ) {
              // 対象の桁材が山留仮鋼材の場合
              size = ClsKouzaiSpecify.GetKouzaiSize( afterFiMaterial.m_Size ) ;
            }

            var _familyPath = ClsTaikeikouPLCsv.GetFamilyPath( size ) ;
            var _typeName = ClsTaikeikouPLCsv.GetFamilyTypeName( size ) ;
            if ( ! string.IsNullOrEmpty( _familyPath ) && ! string.IsNullOrEmpty( _typeName ) ) {
              familyName = _familyPath ;
              typeName = _typeName ;
            }
          }

          var chirdNewFs = GantryUtil.GetFamilySymbol( _doc, typeName, familyName ) ;
          if ( chirdNewFs == null ) {
            GantryUtil.GetFamilySymbol( _doc, familyName, typeName, out chirdNewFs ) ;
          }

          List<FamilyInstance> _chirdFiList = null ;
          using ( Transaction tr = new Transaction( _doc ) ) {
            tr.Start( "Replace Element" ) ;

            // 警告を制御する処理（部材を削除する前に同一点に部材を置くとコミット時に警告が出るので対応）
            FailureHandlingOptions failOpt = tr.GetFailureHandlingOptions() ;
            failOpt.SetFailuresPreprocessor( new WarningSwallower() ) ;
            tr.SetFailureHandlingOptions( failOpt ) ;

            // 置き直す前に置き直す対象を参照している部材を取得
            FilteredElementCollector _collector = new FilteredElementCollector( _doc ) ;
            _collector.OfCategory( BuiltInCategory.OST_GenericModel ) ;
            _chirdFiList = _collector.OfClass( typeof( FamilyInstance ) ).Cast<FamilyInstance>()
              .Where( x => x.Host != null && x.Host.Id == fi.Id ).ToList() ;

            // 新たな参照部材より、元の参照部材面と同一面を指定して再配置
            createdId = SizeChange( _doc, fi, _material, chirdNewFs, reference ) ;
            // 部材情報付与
            AttachMaterialSuper( _doc, createdId, _material, _material.m_Size ) ;

            tr.Commit() ;
          }

          var _afterFamilyInstance = GantryUtil.GetFamilyInstance( _doc, createdId ) ;

          // 再帰的に呼び出す
          ReplaceReferenceDestinationElement( fi, _material, _afterFamilyInstance, _chirdFiList ) ;

          using ( Transaction tr = new Transaction( _doc ) ) {
            tr.Start( "Delete Element" ) ;

            _doc.Delete( fi.Id ) ;

            tr.Commit() ;
          }
        }
      }
    }

    /// <summary>
    /// タイプを複製
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="materialSuper"></param>
    /// <param name="familySymbol"></param>
    /// <param name="orgTypeName"></param>
    /// <remarks>タイプ複製が不要な部材についてはタイプをそのまま返却</remarks>
    /// <returns>複製したタイプ</returns>
    private FamilySymbol DuplicateFamilySymbol( Document doc, MaterialSuper materialSuper, FamilySymbol familySymbol,
      string orgTypeName )
    {
      if ( materialSuper.GetType() == typeof( Fukkouban ) || materialSuper.GetType() == typeof( HorizontalBrace ) ||
           materialSuper.GetType() == typeof( HorizontalJoint ) || materialSuper.GetType() == typeof( Madumezai ) ||
           materialSuper.GetType() == typeof( HorizontalBrace ) || materialSuper.GetType() == typeof( Shikiteppan ) ||
           materialSuper.GetType() == typeof( Shimakouhan ) || materialSuper.GetType() == typeof( Stiffener ) ||
           materialSuper.GetType() == typeof( StiffenerJack ) || materialSuper.GetType() == typeof( StiffenerPlate ) ||
           materialSuper.GetType() == typeof( TeiketsuHojo ) || materialSuper.GetType() == typeof( Tensetsuban ) ||
           materialSuper.GetType() == typeof( VerticalBrace ) || materialSuper.GetType() == typeof( TaikeikouPL ) ||
           ( materialSuper.GetType() == typeof( Houdue ) && ( (Houdue) materialSuper ).m_Syuzai == false ) ||
           ( materialSuper.GetType() == typeof( JifukuZuredomezai ) &&
             ! ( (JifukuZuredomezai) materialSuper ).m_Size.StartsWith( "[" ) ) ||
           materialSuper.GetType() == typeof( Jack ) || materialSuper.GetType() == typeof( TaikeikouPL ) ||
           materialSuper.GetType() == typeof( Tensetsuban ) ) {
        return familySymbol ;
      }

      var duplicateFamilySymbol =
        GantryUtil.DuplicateTypeWithNameRule( _doc, materialSuper.m_KodaiName, familySymbol, orgTypeName ) ;
      return duplicateFamilySymbol ;
    }

    /// <summary>
    /// サイズを変更
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="beforeFamilyInstance">変更前の部材のファミリインスタンス</param>
    /// <param name="beforeMaterialSuper">変更前の部材の部材情報</param>
    /// <param name="afterFamilySymbol">変更後の部材のファミリシンボル</param>
    /// <returns>変更後サイズの部材のElementId</returns>
    private ElementId SizeChange( Document doc, FamilyInstance beforeFamilyInstance, MaterialSuper beforeMaterialSuper,
      FamilySymbol afterFamilySymbol, Reference reference = null )
    {
      // 変更前の部材の基準レベルとオフセット値を取得
      double offset = 0 ;

      if ( reference == null ) {
        var level = GantryUtil.GetInstanceLevelAndOffset( _doc, beforeFamilyInstance, ref offset ) ;
        if ( level == null ) {
          reference = beforeFamilyInstance.HostFace ;
        }
        else {
          reference = level.GetPlaneReference() ;
        }
      }

      offset = ClsRevitUtil.CovertFromAPI(
        ClsRevitUtil.GetParameterDouble( doc, beforeFamilyInstance.Id, DefineUtil.PARAM_HOST_OFFSET ) ) ;

      // タイプパラメータから材質を取得
      var materialByTypeParam =
        ClsRevitUtil.GetTypeParameterString( beforeFamilyInstance.Symbol, DefineUtil.PARAM_MATERIAL ) ;
      if ( ! string.IsNullOrEmpty( materialByTypeParam ) ) {
        // 材質を変更後サイズのタイプパラメータに転写
        ClsRevitUtil.SetTypeParameter( afterFamilySymbol, DefineUtil.PARAM_MATERIAL, materialByTypeParam ) ;
      }

      if ( beforeMaterialSuper.GetType() == typeof( Ohbiki ) || beforeMaterialSuper.GetType() == typeof( Shikigeta ) ||
           beforeMaterialSuper.GetType() == typeof( Neda ) || beforeMaterialSuper.GetType() == typeof( FukkouGeta ) ||
           beforeMaterialSuper.GetType() == typeof( HorizontalBrace ) ||
           beforeMaterialSuper.GetType() == typeof( HorizontalJoint ) ||
           beforeMaterialSuper.GetType() == typeof( VerticalBrace ) ||
           ( beforeMaterialSuper.GetType() == typeof( Houdue ) && ( (Houdue) beforeMaterialSuper ).m_Syuzai ) ||
           ( beforeMaterialSuper.GetType() == typeof( JifukuZuredomezai ) &&
             ( (JifukuZuredomezai) beforeMaterialSuper ).m_Size.StartsWith( "[" ) ) ||
           beforeMaterialSuper.GetType() == typeof( Madumezai ) ||
           beforeMaterialSuper.GetType() == typeof( Taikeikou ) || beforeMaterialSuper.GetType() == typeof( Tesuri ) ) {
        // 変更前の部材の端点を取得
        Curve cvBase = GantryUtil.GetCurve( _doc, beforeFamilyInstance.Id ) ;

        // 変更前の部材の始点終点を取得
        XYZ tmpStPoint = cvBase.GetEndPoint( 0 ) ;
        XYZ tmpEdPoint = cvBase.GetEndPoint( 1 ) ;

        // タイプパラメータから回転を取得
        var rotateByTypeParam = ClsRevitUtil
          .CovertFromAPI( ClsRevitUtil.GetTypeParameter( beforeFamilyInstance.Symbol, DefineUtil.PARAM_ROTATE ) )
          .ToString() ;
        if ( ! string.IsNullOrEmpty( rotateByTypeParam ) ) {
          // Revit単位系に変換
          var convRotate = ClsRevitUtil.CovertToAPI( double.Parse( rotateByTypeParam ) ) ;
          // 回転を変更後サイズのタイプパラメータに転写
          ClsRevitUtil.SetTypeParameter( afterFamilySymbol, DefineUtil.PARAM_ROTATE, convRotate ) ;
        }

        var result = MaterialSuper.PlaceWithTwoPoints( afterFamilySymbol, reference, tmpStPoint, tmpEdPoint,
          ClsRevitUtil.CovertToAPI( offset ) ) ;

        if ( beforeMaterialSuper.GetType() == typeof( Taikeikou ) ) {
          if ( ! afterFamilySymbol.Family.Name.Contains( "TC" ) ) {
            // 対傾構は「長さ」がタイプパラメータなのでここで設定（定形サイズの場合はスキップ）
            ClsRevitUtil.SetTypeParameter( afterFamilySymbol, DefineUtil.PARAM_LENGTH,
              tmpStPoint.DistanceTo( tmpEdPoint ) ) ;
          }
        }

        return result ;
      }

      if ( beforeMaterialSuper.GetType() == typeof( TakasaChouseizai ) ) {
        // 高さ調整プレートはサイズ(可変)が1種類しかないのでサイズ変更されることがなさそうだが一応

        var wByTypeParam = ClsRevitUtil
          .CovertFromAPI( ClsRevitUtil.GetTypeParameter( beforeFamilyInstance.Symbol, "W" ) ).ToString() ;
        var dByTypeParam = ClsRevitUtil
          .CovertFromAPI( ClsRevitUtil.GetTypeParameter( beforeFamilyInstance.Symbol, "D" ) ).ToString() ;
        var h1ByTypeParam = ClsRevitUtil
          .CovertFromAPI( ClsRevitUtil.GetTypeParameter( beforeFamilyInstance.Symbol, "H1" ) ).ToString() ;
        var h2ByTypeParam = ClsRevitUtil
          .CovertFromAPI( ClsRevitUtil.GetTypeParameter( beforeFamilyInstance.Symbol, "H2" ) ).ToString() ;

        if ( ! string.IsNullOrEmpty( wByTypeParam ) ) {
          var convW = ClsRevitUtil.CovertToAPI( double.Parse( wByTypeParam ) ) ;
          ClsRevitUtil.SetTypeParameter( afterFamilySymbol, "W", convW ) ;
        }

        if ( ! string.IsNullOrEmpty( dByTypeParam ) ) {
          var convD = ClsRevitUtil.CovertToAPI( double.Parse( dByTypeParam ) ) ;
          ClsRevitUtil.SetTypeParameter( afterFamilySymbol, "D", convD ) ;
        }

        if ( ! string.IsNullOrEmpty( h1ByTypeParam ) ) {
          var convH1 = ClsRevitUtil.CovertToAPI( double.Parse( h1ByTypeParam ) ) ;
          ClsRevitUtil.SetTypeParameter( afterFamilySymbol, "H1", convH1 ) ;
        }

        if ( ! string.IsNullOrEmpty( h2ByTypeParam ) ) {
          var convH2 = ClsRevitUtil.CovertToAPI( double.Parse( h2ByTypeParam ) ) ;
          ClsRevitUtil.SetTypeParameter( afterFamilySymbol, "H2", convH2 ) ;
        }
      }
      else if ( beforeMaterialSuper.GetType() == typeof( HojoPieace ) ) {
        var h1ByTypeParam = ClsRevitUtil
          .CovertFromAPI( ClsRevitUtil.GetTypeParameter( beforeFamilyInstance.Symbol, "H1" ) ).ToString() ;
        var h2ByTypeParam = ClsRevitUtil
          .CovertFromAPI( ClsRevitUtil.GetTypeParameter( beforeFamilyInstance.Symbol, "H2" ) ).ToString() ;
        var topWByTypeParam = ClsRevitUtil
          .CovertFromAPI( ClsRevitUtil.GetTypeParameter( beforeFamilyInstance.Symbol, "エンドプレート上W" ) ).ToString() ;
        var topDByTypeParam = ClsRevitUtil
          .CovertFromAPI( ClsRevitUtil.GetTypeParameter( beforeFamilyInstance.Symbol, "エンドプレート上D" ) ).ToString() ;
        var topHByTypeParam = ClsRevitUtil
          .CovertFromAPI( ClsRevitUtil.GetTypeParameter( beforeFamilyInstance.Symbol, "エンドプレート上厚さ" ) ).ToString() ;

        var topKindByTypeParam = GantryUtil.GetTypeParameter( beforeFamilyInstance.Symbol,
          DefineUtil.PARAM_END_PLATE_SIZE_U, isValue: true ) ;

        var endWByTypeParam = ClsRevitUtil
          .CovertFromAPI( ClsRevitUtil.GetTypeParameter( beforeFamilyInstance.Symbol, "エンドプレート下W" ) ).ToString() ;
        var endDByTypeParam = ClsRevitUtil
          .CovertFromAPI( ClsRevitUtil.GetTypeParameter( beforeFamilyInstance.Symbol, "エンドプレート下D" ) ).ToString() ;
        var endHByTypeParam = ClsRevitUtil
          .CovertFromAPI( ClsRevitUtil.GetTypeParameter( beforeFamilyInstance.Symbol, "エンドプレート下厚さ" ) ).ToString() ;

        var endKindByTypeParam = GantryUtil.GetTypeParameter( beforeFamilyInstance.Symbol,
          DefineUtil.PARAM_END_PLATE_SIZE_B, isValue: true ) ;

        if ( ! string.IsNullOrEmpty( h1ByTypeParam ) ) {
          var convH1 = ClsRevitUtil.CovertToAPI( double.Parse( h1ByTypeParam ) ) ;
          ClsRevitUtil.SetTypeParameter( afterFamilySymbol, "H1", convH1 ) ;
        }

        if ( ! string.IsNullOrEmpty( h2ByTypeParam ) ) {
          var convH2 = ClsRevitUtil.CovertToAPI( double.Parse( h2ByTypeParam ) ) ;
          ClsRevitUtil.SetTypeParameter( afterFamilySymbol, "H2", convH2 ) ;
        }

        if ( ! string.IsNullOrEmpty( topWByTypeParam ) ) {
          var convTopW = ClsRevitUtil.CovertToAPI( double.Parse( topWByTypeParam ) ) ;
          ClsRevitUtil.SetTypeParameter( afterFamilySymbol, "エンドプレート上W", convTopW ) ;
        }

        if ( ! string.IsNullOrEmpty( topDByTypeParam ) ) {
          var convTopD = ClsRevitUtil.CovertToAPI( double.Parse( topDByTypeParam ) ) ;
          ClsRevitUtil.SetTypeParameter( afterFamilySymbol, "エンドプレート上D", convTopD ) ;
        }

        if ( ! string.IsNullOrEmpty( topHByTypeParam ) ) {
          var convTopH = ClsRevitUtil.CovertToAPI( double.Parse( topHByTypeParam ) ) ;
          ClsRevitUtil.SetTypeParameter( afterFamilySymbol, "エンドプレート上厚さ", convTopH ) ;
        }

        if ( ! string.IsNullOrEmpty( topKindByTypeParam ) ) {
          var topKindSizeFamilyName = string.Empty ;
          var topKindSizeTypeName = string.Empty ;
          if ( topKindByTypeParam.Contains( " : " ) ) {
            var topKindSizeByTypeParamSplitIndex = topKindByTypeParam.IndexOf( " : " ) ;
            topKindSizeFamilyName = topKindByTypeParam.Substring( 0, topKindSizeByTypeParamSplitIndex ) ;
            topKindSizeTypeName = topKindByTypeParam.Replace( $"{topKindSizeFamilyName} : ", "" ) ;
          }

          var topKindFs = ClsRevitUtil.GetFamilySymbolWithFuzzy( doc, topKindSizeFamilyName, topKindSizeTypeName ) ;
          if ( topKindFs != null ) {
            GantryUtil.SetSymbolParameterValueByParameterName( afterFamilySymbol, DefineUtil.PARAM_END_PLATE_SIZE_U,
              topKindFs.Id.ToString() ) ;
          }
        }

        if ( ! string.IsNullOrEmpty( endWByTypeParam ) ) {
          var convEndW = ClsRevitUtil.CovertToAPI( double.Parse( endWByTypeParam ) ) ;
          ClsRevitUtil.SetTypeParameter( afterFamilySymbol, "エンドプレート下W", convEndW ) ;
        }

        if ( ! string.IsNullOrEmpty( endDByTypeParam ) ) {
          var convEndD = ClsRevitUtil.CovertToAPI( double.Parse( endDByTypeParam ) ) ;
          ClsRevitUtil.SetTypeParameter( afterFamilySymbol, "エンドプレート下D", convEndD ) ;
        }

        if ( ! string.IsNullOrEmpty( endHByTypeParam ) ) {
          var convEndH = ClsRevitUtil.CovertToAPI( double.Parse( endHByTypeParam ) ) ;
          ClsRevitUtil.SetTypeParameter( afterFamilySymbol, "エンドプレート下厚さ", convEndH ) ;
        }

        if ( ! string.IsNullOrEmpty( endKindByTypeParam ) ) {
          var endKindSizeFamilyName = string.Empty ;
          var endKindSizeTypeName = string.Empty ;
          if ( endKindByTypeParam.Contains( " : " ) ) {
            var endKindSizeByTypeParamSplitIndex = endKindByTypeParam.IndexOf( " : " ) ;
            endKindSizeFamilyName = endKindByTypeParam.Substring( 0, endKindSizeByTypeParamSplitIndex ) ;
            endKindSizeTypeName = endKindByTypeParam.Replace( $"{endKindSizeFamilyName} : ", "" ) ;
          }

          var endKindFs = ClsRevitUtil.GetFamilySymbolWithFuzzy( doc, endKindSizeFamilyName, endKindSizeTypeName ) ;
          if ( endKindFs != null ) {
            GantryUtil.SetSymbolParameterValueByParameterName( afterFamilySymbol, DefineUtil.PARAM_END_PLATE_SIZE_B,
              endKindFs.Id.ToString() ) ;
          }
        }
      }
      else if ( beforeMaterialSuper.GetType() == typeof( StiffenerJack ) ) {
        //var lengthByTypeParam = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetTypeParameter(beforeFamilyInstance.Symbol, DefineUtil.PARAM_LENGTH)).ToString();

        //if (!string.IsNullOrEmpty(lengthByTypeParam))
        //{
        //    var convLength = ClsRevitUtil.CovertToAPI(double.Parse(lengthByTypeParam));
        //    ClsRevitUtil.SetTypeParameter(afterFamilySymbol, DefineUtil.PARAM_LENGTH, convLength);
        //}
      }
      else if ( beforeMaterialSuper.GetType() == typeof( TesuriShichu ) ) {
        var lengthByTypeParam = ClsRevitUtil
          .CovertFromAPI( ClsRevitUtil.GetTypeParameter( beforeFamilyInstance.Symbol, DefineUtil.PARAM_LENGTH ) )
          .ToString() ;

        if ( ! string.IsNullOrEmpty( lengthByTypeParam ) ) {
          var convLength = ClsRevitUtil.CovertToAPI( double.Parse( lengthByTypeParam ) ) ;
          ClsRevitUtil.SetTypeParameter( afterFamilySymbol, DefineUtil.PARAM_LENGTH, convLength ) ;
        }
      }
      else if ( beforeMaterialSuper.GetType() == typeof( PilePiller ) ) {
        var lengthByTypeParam = ClsRevitUtil
          .CovertFromAPI( ClsRevitUtil.GetTypeParameter( beforeFamilyInstance.Symbol, DefineUtil.PARAM_LENGTH ) )
          .ToString() ;
        var rotateByTypeParam = ClsRevitUtil
          .CovertFromAPI( ClsRevitUtil.GetTypeParameter( beforeFamilyInstance.Symbol, DefineUtil.PARAM_ROTATE ) )
          .ToString() ;

        var jointCountByTypeParam = GantryUtil
          .GetTypeParameter( beforeFamilyInstance.Symbol, DefineUtil.PARAM_JOINT_COUNT, isValue: true ).ToString() ;
        var pileCutLengByTypeParam = ClsRevitUtil
          .CovertFromAPI( ClsRevitUtil.GetTypeParameter( beforeFamilyInstance.Symbol, DefineUtil.PARAM_PILE_CUT_LENG ) )
          .ToString() ;

        var jointTypeByTypeParam =
          GantryUtil.GetTypeParameter( beforeFamilyInstance.Symbol, DefineUtil.PARAM_JOINT_TYPE, isValue: true ) ;
        if ( ! string.IsNullOrEmpty( jointTypeByTypeParam ) ) {
          var jointType = jointTypeByTypeParam.Contains( "ﾎﾞﾙﾄ" )
            ? DefineUtil.eJoinType.Bolt
            : ( jointTypeByTypeParam.Contains( "溶接" ) ? DefineUtil.eJoinType.Welding : DefineUtil.eJoinType.None ) ;
          var shichuSize = GantryUtil.GetTypeParameter( afterFamilySymbol, DefineUtil.PARAM_SIZE, isValue: true ) ;
          var jointName = GantryUtil.GetJointTypeName( jointType, shichuSize ) ;
          var jointFs = ClsRevitUtil.GetFamilySymbol( _doc, jointName, jointName ) ;
          if ( jointFs != null ) {
            GantryUtil.SetSymbolParameterValueByParameterName( afterFamilySymbol, DefineUtil.PARAM_JOINT_TYPE,
              jointFs.Id.ToString() ) ;
          }
        }

        var useTopPLByTypeParam =
          GantryUtil.GetTypeParameter( beforeFamilyInstance.Symbol, DefineUtil.PARAM_TOP_PLATE, isValue: true ) ;

        var topPLSizeByTypeParam = GantryUtil.GetTypeParameter( beforeFamilyInstance.Symbol,
          DefineUtil.PARAM_TOP_PLATE_SIZE, isValue: true ) ;
        if ( ! string.IsNullOrEmpty( topPLSizeByTypeParam ) ) {
          var topPLSizeFamilyName = string.Empty ;
          var topPLSizeTypeName = string.Empty ;
          if ( topPLSizeByTypeParam.Contains( " : " ) ) {
            var topPLSizeByTypeParamSplitIndex = topPLSizeByTypeParam.IndexOf( " : " ) ;
            topPLSizeFamilyName = topPLSizeByTypeParam.Substring( 0, topPLSizeByTypeParamSplitIndex ) ;
            topPLSizeTypeName = topPLSizeByTypeParam.Replace( $"{topPLSizeFamilyName} : ", "" ) ;
          }
          else {
            if ( topPLSizeByTypeParam == TOP_PL_OPTIONAL_NAME ) {
              topPLSizeFamilyName = TOP_PL_OPTIONAL_NAME ;
              topPLSizeTypeName = TOP_PL_OPTIONAL_NAME ;
            }
          }

          var topPLFs = ClsRevitUtil.GetFamilySymbolWithFuzzy( doc, topPLSizeFamilyName, topPLSizeTypeName ) ;
          if ( topPLFs != null ) {
            GantryUtil.SetSymbolParameterValueByParameterName( afterFamilySymbol, DefineUtil.PARAM_TOP_PLATE_SIZE,
              topPLFs.Id.ToString() ) ;
          }
        }

        var useBasePLByTypeParam =
          GantryUtil.GetTypeParameter( beforeFamilyInstance.Symbol, DefineUtil.PARAM_BASE_PLATE, isValue: true ) ;

        var basePLSizeByTypeParam = GantryUtil.GetTypeParameter( beforeFamilyInstance.Symbol,
          DefineUtil.PARAM_BASE_PLATE_SIZE, isValue: true ) ;
        if ( ! string.IsNullOrEmpty( basePLSizeByTypeParam ) ) {
          var kouzaiSize = GantryUtil.GetTypeParameter( afterFamilySymbol, DefineUtil.PARAM_SIZE, isValue: true ) ;
          if ( kouzaiSize.Contains( "-" ) ) {
            kouzaiSize = kouzaiSize.Replace( "-", "" ) ;
          }

          kouzaiSize = kouzaiSize.Replace( "x", "X" ) ;

          var basePLSizeType = basePLSizeByTypeParam.Contains( BASE_PL_TYPE_STRONG )
            ? BASE_PL_TYPE_STRONG
            : ( basePLSizeByTypeParam.Contains( BASE_PL_TYPE_NORMAL ) ? BASE_PL_TYPE_NORMAL : BASE_PL_TYPE_SIMPLE ) ;
          string[] bpl = ClsSichuCsv.GetBPLData( kouzaiSize, basePLSizeType ).Split( '_' ) ;
          if ( bpl.Length == 2 ) {
            var basePLFs = ClsRevitUtil.GetFamilySymbolWithFuzzy( _doc, bpl[ 0 ], bpl[ 1 ] ) ;
            if ( basePLFs != null ) {
              GantryUtil.SetSymbolParameterValueByParameterName( afterFamilySymbol, DefineUtil.PARAM_BASE_PLATE_SIZE,
                basePLFs.Id.ToString() ) ;
            }
          }
        }

        var topPLWByTypeParam = ClsRevitUtil
          .CovertFromAPI( ClsRevitUtil.GetTypeParameter( beforeFamilyInstance.Symbol, DefineUtil.PARAM_TOP_PLATE_W ) )
          .ToString() ;
        var topPLDByTypeParam = ClsRevitUtil
          .CovertFromAPI( ClsRevitUtil.GetTypeParameter( beforeFamilyInstance.Symbol, DefineUtil.PARAM_TOP_PLATE_D ) )
          .ToString() ;
        var topPLTByTypeParam = ClsRevitUtil
          .CovertFromAPI( ClsRevitUtil.GetTypeParameter( beforeFamilyInstance.Symbol, DefineUtil.PARAM_TOP_PLATE_T ) )
          .ToString() ;

        var kuiByTypeParamList = new List<string>() ;
        for ( int i = 0 ; i < 10 ; i++ ) {
          kuiByTypeParamList.Add( ClsRevitUtil
            .CovertFromAPI( ClsRevitUtil.GetTypeParameter( beforeFamilyInstance.Symbol, $"杭{i + 1}" ) ).ToString() ) ;
        }

        var shichuByTypeParamList = new List<string>() ;
        for ( int i = 0 ; i < 10 ; i++ ) {
          shichuByTypeParamList.Add( ClsRevitUtil
            .CovertFromAPI( ClsRevitUtil.GetTypeParameter( beforeFamilyInstance.Symbol, $"{i + 1}段目長さ" ) )
            .ToString() ) ;
        }

        if ( ! string.IsNullOrEmpty( lengthByTypeParam ) ) {
          var convLength = ClsRevitUtil.CovertToAPI( double.Parse( lengthByTypeParam ) ) ;
          ClsRevitUtil.SetTypeParameter( afterFamilySymbol, DefineUtil.PARAM_LENGTH, convLength ) ;
        }

        if ( ! string.IsNullOrEmpty( rotateByTypeParam ) ) {
          var convRotate = ClsRevitUtil.CovertToAPI( double.Parse( rotateByTypeParam ) ) ;
          ClsRevitUtil.SetTypeParameter( afterFamilySymbol, DefineUtil.PARAM_ROTATE, convRotate ) ;
        }

        if ( ! string.IsNullOrEmpty( jointCountByTypeParam ) ) {
          GantryUtil.SetSymbolParameterValueByParameterName( afterFamilySymbol, DefineUtil.PARAM_JOINT_COUNT,
            jointCountByTypeParam ) ;
        }

        if ( ! string.IsNullOrEmpty( pileCutLengByTypeParam ) ) {
          var convPileCutLengt = ClsRevitUtil.CovertToAPI( double.Parse( pileCutLengByTypeParam ) ) ;
          ClsRevitUtil.SetTypeParameter( afterFamilySymbol, DefineUtil.PARAM_PILE_CUT_LENG, convPileCutLengt ) ;
        }

        if ( ! string.IsNullOrEmpty( useTopPLByTypeParam ) ) {
          GantryUtil.SetSymbolParameterValueByParameterName( afterFamilySymbol, DefineUtil.PARAM_TOP_PLATE,
            useTopPLByTypeParam == "はい"
              ? ( (int) DefineUtil.PramYesNo.Yes ).ToString()
              : ( (int) DefineUtil.PramYesNo.No ).ToString() ) ;
        }

        if ( ! string.IsNullOrEmpty( useBasePLByTypeParam ) ) {
          GantryUtil.SetSymbolParameterValueByParameterName( afterFamilySymbol, DefineUtil.PARAM_BASE_PLATE,
            useBasePLByTypeParam == "はい"
              ? ( (int) DefineUtil.PramYesNo.Yes ).ToString()
              : ( (int) DefineUtil.PramYesNo.No ).ToString() ) ;
        }

        if ( ! string.IsNullOrEmpty( topPLWByTypeParam ) ) {
          var convTopPLW = ClsRevitUtil.CovertToAPI( double.Parse( topPLWByTypeParam ) ) ;
          ClsRevitUtil.SetTypeParameter( afterFamilySymbol, DefineUtil.PARAM_TOP_PLATE_W, convTopPLW ) ;
        }

        if ( ! string.IsNullOrEmpty( topPLDByTypeParam ) ) {
          var convTopPLD = ClsRevitUtil.CovertToAPI( double.Parse( topPLDByTypeParam ) ) ;
          ClsRevitUtil.SetTypeParameter( afterFamilySymbol, DefineUtil.PARAM_TOP_PLATE_D, convTopPLD ) ;
        }

        if ( ! string.IsNullOrEmpty( topPLTByTypeParam ) ) {
          var convTopPLT = ClsRevitUtil.CovertToAPI( double.Parse( topPLTByTypeParam ) ) ;
          ClsRevitUtil.SetTypeParameter( afterFamilySymbol, DefineUtil.PARAM_TOP_PLATE_T, convTopPLT ) ;
        }

        for ( int i = 0 ; i < 10 ; i++ ) {
          var kuiTypeParam = kuiByTypeParamList[ i ] ;
          var convKui = ClsRevitUtil.CovertToAPI( double.Parse( kuiTypeParam ) ) ;
          ClsRevitUtil.SetTypeParameter( afterFamilySymbol, $"杭{i + 1}", convKui ) ;
        }

        for ( int i = 0 ; i < 10 ; i++ ) {
          var shichuTypeParam = shichuByTypeParamList[ i ] ;
          var convShichu = ClsRevitUtil.CovertToAPI( double.Parse( shichuTypeParam ) ) ;
          ClsRevitUtil.SetTypeParameter( afterFamilySymbol, $"{i + 1}段目長さ", convShichu ) ;
        }
      }
      else if ( beforeMaterialSuper.GetType() == typeof( ExtensionPile ) ) {
        var lengthByTypeParam = ClsRevitUtil
          .CovertFromAPI( ClsRevitUtil.GetTypeParameter( beforeFamilyInstance.Symbol, DefineUtil.PARAM_LENGTH ) )
          .ToString() ;
        var rotateByTypeParam = ClsRevitUtil
          .CovertFromAPI( ClsRevitUtil.GetTypeParameter( beforeFamilyInstance.Symbol, DefineUtil.PARAM_ROTATE ) )
          .ToString() ;

        var jointCountByTypeParam = GantryUtil
          .GetTypeParameter( beforeFamilyInstance.Symbol, DefineUtil.PARAM_JOINT_COUNT, isValue: true ).ToString() ;

        var jointTypeByTypeParam =
          GantryUtil.GetTypeParameter( beforeFamilyInstance.Symbol, DefineUtil.PARAM_JOINT_TYPE, isValue: true ) ;
        if ( ! string.IsNullOrEmpty( jointTypeByTypeParam ) ) {
          var jointType = jointTypeByTypeParam.Contains( "ﾎﾞﾙﾄ" )
            ? DefineUtil.eJoinType.Bolt
            : ( jointTypeByTypeParam.Contains( "溶接" ) ? DefineUtil.eJoinType.Welding : DefineUtil.eJoinType.None ) ;
          var kouzaiSize = GantryUtil.GetTypeParameter( afterFamilySymbol, DefineUtil.PARAM_SIZE, isValue: true ) ;
          var jointName = GantryUtil.GetJointTypeName( jointType, kouzaiSize ) ;
          var jointFs = ClsRevitUtil.GetFamilySymbol( _doc, jointName, jointName ) ;
          if ( jointFs != null ) {
            GantryUtil.SetSymbolParameterValueByParameterName( afterFamilySymbol, DefineUtil.PARAM_JOINT_TYPE,
              jointFs.Id.ToString() ) ;
          }
        }

        var useTopPLByTypeParam =
          GantryUtil.GetTypeParameter( beforeFamilyInstance.Symbol, DefineUtil.PARAM_TOP_PLATE, isValue: true ) ;

        var topPLSizeByTypeParam = GantryUtil.GetTypeParameter( beforeFamilyInstance.Symbol,
          DefineUtil.PARAM_TOP_PLATE_SIZE, isValue: true ) ;
        if ( ! string.IsNullOrEmpty( topPLSizeByTypeParam ) ) {
          var topPLSizeFamilyName = string.Empty ;
          var topPLSizeTypeName = string.Empty ;
          if ( topPLSizeByTypeParam.Contains( " : " ) ) {
            var topPLSizeByTypeParamSplitIndex = topPLSizeByTypeParam.IndexOf( " : " ) ;
            topPLSizeFamilyName = topPLSizeByTypeParam.Substring( 0, topPLSizeByTypeParamSplitIndex ) ;
            topPLSizeTypeName = topPLSizeByTypeParam.Replace( $"{topPLSizeFamilyName} : ", "" ) ;
          }
          else {
            if ( topPLSizeByTypeParam == TOP_PL_OPTIONAL_NAME ) {
              topPLSizeFamilyName = TOP_PL_OPTIONAL_NAME ;
              topPLSizeTypeName = TOP_PL_OPTIONAL_NAME ;
            }
          }

          var topPLFs = ClsRevitUtil.GetFamilySymbolWithFuzzy( doc, topPLSizeFamilyName, topPLSizeTypeName ) ;
          if ( topPLFs != null ) {
            GantryUtil.SetSymbolParameterValueByParameterName( afterFamilySymbol, DefineUtil.PARAM_TOP_PLATE_SIZE,
              topPLFs.Id.ToString() ) ;
          }
        }

        var topPLTByTypeParam = ClsRevitUtil
          .CovertFromAPI( ClsRevitUtil.GetTypeParameter( beforeFamilyInstance.Symbol, DefineUtil.PARAM_TOP_PLATE_T ) )
          .ToString() ;

        if ( ! string.IsNullOrEmpty( lengthByTypeParam ) ) {
          var convLength = ClsRevitUtil.CovertToAPI( double.Parse( lengthByTypeParam ) ) ;
          ClsRevitUtil.SetTypeParameter( afterFamilySymbol, DefineUtil.PARAM_LENGTH, convLength ) ;
        }

        if ( ! string.IsNullOrEmpty( rotateByTypeParam ) ) {
          var convRotate = ClsRevitUtil.CovertToAPI( double.Parse( rotateByTypeParam ) ) ;
          ClsRevitUtil.SetTypeParameter( afterFamilySymbol, DefineUtil.PARAM_ROTATE, convRotate ) ;
        }

        if ( ! string.IsNullOrEmpty( jointCountByTypeParam ) ) {
          GantryUtil.SetSymbolParameterValueByParameterName( afterFamilySymbol, DefineUtil.PARAM_JOINT_COUNT,
            jointCountByTypeParam ) ;
        }

        if ( ! string.IsNullOrEmpty( useTopPLByTypeParam ) ) {
          GantryUtil.SetSymbolParameterValueByParameterName( afterFamilySymbol, DefineUtil.PARAM_TOP_PLATE,
            useTopPLByTypeParam == "はい"
              ? ( (int) DefineUtil.PramYesNo.Yes ).ToString()
              : ( (int) DefineUtil.PramYesNo.No ).ToString() ) ;
        }

        if ( ! string.IsNullOrEmpty( topPLTByTypeParam ) ) {
          var convTopPLT = ClsRevitUtil.CovertToAPI( double.Parse( topPLTByTypeParam ) ) ;
          ClsRevitUtil.SetTypeParameter( afterFamilySymbol, DefineUtil.PARAM_TOP_PLATE_T, convTopPLT ) ;
        }
      }

      return GantryUtil.CreateInstanceWith1point( doc, beforeMaterialSuper.Position, reference, afterFamilySymbol,
        beforeFamilyInstance.HandOrientation, followWithHostType: false )?.Id ;
    }

    /// <summary>
    /// 配置部材に部材情報を付与する
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="id"></param>
    /// <param name="koudaiName"></param>
    /// <param name="materialSuper"></param>
    /// <param name="size"></param>
    private void AttachMaterialSuper( Document doc, ElementId id, MaterialSuper materialSuper, string size )
    {
      if ( materialSuper.GetType() == typeof( Fukkouban ) ) {
        var orgFukkouban = (Fukkouban) materialSuper ;

        Fukkouban newMs = new Fukkouban() ;
        newMs.m_ElementId = id ;
        newMs.m_KodaiName = orgFukkouban.m_KodaiName ;
        ;
        newMs.m_Size = size ;
        newMs.m_Material = orgFukkouban.m_Material ;
        newMs.m_hukuinNum = orgFukkouban.m_hukuinNum ;
        newMs.m_kyoutyouNum = orgFukkouban.m_kyoutyouNum ;
        newMs.m_TeiketsuType = orgFukkouban.m_TeiketsuType ;
        newMs.m_Bolt1Cnt = orgFukkouban.m_Bolt1Cnt ;
        newMs.m_Bolt2Cnt = orgFukkouban.m_Bolt2Cnt ;
        newMs.m_BoltInfo1 = orgFukkouban.m_BoltInfo1 ;
        newMs.m_BoltInfo2 = orgFukkouban.m_BoltInfo2 ;
        Fukkouban.WriteToElement( newMs, doc ) ;
      }
      else if ( materialSuper.GetType() == typeof( Neda ) ) {
        var orgNeda = (Neda) materialSuper ;

        Neda newMs = new Neda() ;
        newMs.m_ElementId = id ;
        newMs.m_KodaiName = orgNeda.m_KodaiName ;
        newMs.m_Size = size ;
        newMs.m_Material = orgNeda.m_Material ;
        newMs.m_ExStartLen = orgNeda.m_ExStartLen ;
        newMs.m_ExEndLen = orgNeda.m_ExEndLen ;
        newMs.m_H = orgNeda.m_H ;
        newMs.m_MinK = orgNeda.m_MinK ;
        newMs.m_MaxK = orgNeda.m_MaxK ;
        newMs.m_TeiketsuType = orgNeda.m_TeiketsuType ;
        newMs.m_Bolt1Cnt = orgNeda.m_Bolt1Cnt ;
        newMs.m_Bolt2Cnt = orgNeda.m_Bolt2Cnt ;
        newMs.m_BoltInfo1 = orgNeda.m_BoltInfo1 ;
        newMs.m_BoltInfo2 = orgNeda.m_BoltInfo2 ;
        Neda.WriteToElement( newMs, doc ) ;
      }
      else if ( materialSuper.GetType() == typeof( Ohbiki ) ) {
        var orgOhbiki = (Ohbiki) materialSuper ;

        Ohbiki newMs = new Ohbiki() ;
        newMs.m_ElementId = id ;
        newMs.m_KodaiName = orgOhbiki.m_KodaiName ;
        ;
        newMs.m_Size = size ;
        newMs.m_Material = orgOhbiki.m_Material ;
        newMs.m_K = orgOhbiki.m_K ;
        newMs.m_ExStartLen = orgOhbiki.m_ExStartLen ;
        newMs.m_TeiketsuType = orgOhbiki.m_TeiketsuType ;
        newMs.m_Bolt1Cnt = orgOhbiki.m_Bolt1Cnt ;
        newMs.m_Bolt2Cnt = orgOhbiki.m_Bolt2Cnt ;
        newMs.m_BoltInfo1 = orgOhbiki.m_BoltInfo1 ;
        newMs.m_BoltInfo2 = orgOhbiki.m_BoltInfo2 ;
        Ohbiki.WriteToElement( newMs, doc ) ;
      }
      else if ( materialSuper.GetType() == typeof( Shikigeta ) ) {
        var orgShikigeta = (Shikigeta) materialSuper ;

        Shikigeta newMs = new Shikigeta() ;
        newMs.m_ElementId = id ;
        newMs.m_KodaiName = orgShikigeta.m_KodaiName ;
        newMs.m_Size = size ;
        newMs.m_Material = orgShikigeta.m_Material ;
        newMs.m_K = orgShikigeta.m_K ;
        newMs.m_ExStartLen = orgShikigeta.m_ExStartLen ;
        newMs.m_TeiketsuType = orgShikigeta.m_TeiketsuType ;
        newMs.m_Bolt1Cnt = orgShikigeta.m_Bolt1Cnt ;
        newMs.m_Bolt2Cnt = orgShikigeta.m_Bolt2Cnt ;
        newMs.m_BoltInfo1 = orgShikigeta.m_BoltInfo1 ;
        newMs.m_BoltInfo2 = orgShikigeta.m_BoltInfo2 ;
        Shikigeta.WriteToElement( newMs, doc ) ;
      }
      else if ( materialSuper.GetType() == typeof( FukkouGeta ) ) {
        var orgFukkouGeta = (FukkouGeta) materialSuper ;

        FukkouGeta newMs = new FukkouGeta() ;
        newMs.m_ElementId = id ;
        newMs.m_KodaiName = orgFukkouGeta.m_KodaiName ;
        newMs.m_Size = size ;
        newMs.m_Material = orgFukkouGeta.m_Material ;
        newMs.m_TeiketsuType = orgFukkouGeta.m_TeiketsuType ;
        newMs.m_Bolt1Cnt = orgFukkouGeta.m_Bolt1Cnt ;
        newMs.m_Bolt2Cnt = orgFukkouGeta.m_Bolt2Cnt ;
        newMs.m_BoltInfo1 = orgFukkouGeta.m_BoltInfo1 ;
        newMs.m_BoltInfo2 = orgFukkouGeta.m_BoltInfo2 ;
        FukkouGeta.WriteToElement( newMs, doc ) ;
      }
      else if ( materialSuper.GetType() == typeof( TeiketsuHojo ) ) {
        var orgTeiketsuHojo = (TeiketsuHojo) materialSuper ;

        TeiketsuHojo newMs = new TeiketsuHojo() ;
        newMs.m_ElementId = id ;
        newMs.m_KodaiName = orgTeiketsuHojo.m_KodaiName ;
        newMs.m_Size = size ;
        newMs.m_Material = orgTeiketsuHojo.m_Material ;
        newMs.m_TeiketsuType = orgTeiketsuHojo.m_TeiketsuType ;
        newMs.m_Bolt1Cnt = orgTeiketsuHojo.m_Bolt1Cnt ;
        newMs.m_Bolt2Cnt = orgTeiketsuHojo.m_Bolt2Cnt ;
        newMs.m_BoltInfo1 = orgTeiketsuHojo.m_BoltInfo1 ;
        newMs.m_BoltInfo2 = orgTeiketsuHojo.m_BoltInfo2 ;
        TeiketsuHojo.WriteToElement( newMs, doc ) ;
      }
      else if ( materialSuper.GetType() == typeof( Shikiteppan ) ) {
        var orgShikiteppan = (Shikiteppan) materialSuper ;

        Shikiteppan newMs = new Shikiteppan() ;
        newMs.m_ElementId = id ;
        newMs.m_KodaiName = orgShikiteppan.m_KodaiName ;
        ;
        newMs.m_Size = size ;
        newMs.m_Material = orgShikiteppan.m_Material ;
        newMs.m_TeiketsuType = orgShikiteppan.m_TeiketsuType ;
        newMs.m_Bolt1Cnt = orgShikiteppan.m_Bolt1Cnt ;
        newMs.m_Bolt2Cnt = orgShikiteppan.m_Bolt2Cnt ;
        newMs.m_BoltInfo1 = orgShikiteppan.m_BoltInfo1 ;
        newMs.m_BoltInfo2 = orgShikiteppan.m_BoltInfo2 ;
        Shikiteppan.WriteToElement( newMs, doc ) ;
      }
      else if ( materialSuper.GetType() == typeof( Shimakouhan ) ) {
        var orgShimakouhan = (Shimakouhan) materialSuper ;

        Shimakouhan newMs = new Shimakouhan() ;
        newMs.m_ElementId = id ;
        newMs.m_KodaiName = orgShimakouhan.m_KodaiName ;
        ;
        newMs.m_Size = size ;
        newMs.m_Material = orgShimakouhan.m_Material ;
        newMs.m_TeiketsuType = orgShimakouhan.m_TeiketsuType ;
        newMs.m_Bolt1Cnt = orgShimakouhan.m_Bolt1Cnt ;
        newMs.m_Bolt2Cnt = orgShimakouhan.m_Bolt2Cnt ;
        newMs.m_BoltInfo1 = orgShimakouhan.m_BoltInfo1 ;
        newMs.m_BoltInfo2 = orgShimakouhan.m_BoltInfo2 ;
        Shimakouhan.WriteToElement( newMs, doc ) ;
      }
      else if ( materialSuper.GetType() == typeof( TakasaChouseizai ) ) {
        var orgTakasaChouseizai = (TakasaChouseizai) materialSuper ;

        TakasaChouseizai newMs = new TakasaChouseizai() ;
        newMs.m_ElementId = id ;
        newMs.m_KodaiName = orgTakasaChouseizai.m_KodaiName ;
        ;
        newMs.m_Size = size ;
        newMs.m_Material = orgTakasaChouseizai.m_Material ;
        newMs.m_W = orgTakasaChouseizai.m_W ;
        newMs.m_D = orgTakasaChouseizai.m_D ;
        newMs.m_H1 = orgTakasaChouseizai.m_H1 ;
        newMs.m_H2 = orgTakasaChouseizai.m_H2 ;
        newMs.m_TeiketsuType = orgTakasaChouseizai.m_TeiketsuType ;
        newMs.m_Bolt1Cnt = orgTakasaChouseizai.m_Bolt1Cnt ;
        newMs.m_Bolt2Cnt = orgTakasaChouseizai.m_Bolt2Cnt ;
        newMs.m_BoltInfo1 = orgTakasaChouseizai.m_BoltInfo1 ;
        newMs.m_BoltInfo2 = orgTakasaChouseizai.m_BoltInfo2 ;
        TakasaChouseizai.WriteToElement( newMs, doc ) ;
      }
      else if ( materialSuper.GetType() == typeof( HojoPieace ) ) {
        var orgHojoPieace = (HojoPieace) materialSuper ;

        HojoPieace newMs = new HojoPieace() ;
        newMs.m_ElementId = id ;
        newMs.m_KodaiName = orgHojoPieace.m_KodaiName ;
        ;
        newMs.m_Size = size ;
        newMs.m_Material = orgHojoPieace.m_Material ;
        newMs.m_H1 = orgHojoPieace.m_H1 ;
        newMs.m_H2 = orgHojoPieace.m_H2 ;
        newMs.m_TopW = orgHojoPieace.m_TopW ;
        newMs.m_TopD = orgHojoPieace.m_TopD ;
        newMs.m_TopH = orgHojoPieace.m_TopH ;
        newMs.m_TopType = orgHojoPieace.m_TopType ;
        newMs.m_EndW = orgHojoPieace.m_EndW ;
        newMs.m_EndD = orgHojoPieace.m_EndD ;
        newMs.m_EndH = orgHojoPieace.m_EndH ;
        newMs.m_EndType = orgHojoPieace.m_EndType ;
        newMs.m_TeiketsuType = orgHojoPieace.m_TeiketsuType ;
        newMs.m_Bolt1Cnt = orgHojoPieace.m_Bolt1Cnt ;
        newMs.m_Bolt2Cnt = orgHojoPieace.m_Bolt2Cnt ;
        newMs.m_BoltInfo1 = orgHojoPieace.m_BoltInfo1 ;
        newMs.m_BoltInfo2 = orgHojoPieace.m_BoltInfo2 ;
        HojoPieace.WriteToElement( newMs, doc ) ;
      }
      else if ( materialSuper.GetType() == typeof( HorizontalBrace ) ) {
        var orgHorizontalBrace = (HorizontalBrace) materialSuper ;

        HorizontalBrace newMs = new HorizontalBrace() ;
        newMs.m_ElementId = id ;
        newMs.m_KodaiName = orgHorizontalBrace.m_KodaiName ;
        newMs.m_Size = size ;
        newMs.m_Material = orgHorizontalBrace.m_Material ;
        newMs.m_TeiketsuType = orgHorizontalBrace.m_TeiketsuType ;
        newMs.m_Bolt1Cnt = orgHorizontalBrace.m_Bolt1Cnt ;
        newMs.m_Bolt2Cnt = orgHorizontalBrace.m_Bolt2Cnt ;
        newMs.m_BoltInfo1 = orgHorizontalBrace.m_BoltInfo1 ;
        newMs.m_BoltInfo2 = orgHorizontalBrace.m_BoltInfo2 ;
        HorizontalBrace.WriteToElement( newMs, doc ) ;
      }
      else if ( materialSuper.GetType() == typeof( HorizontalJoint ) ) {
        var orgHorizontalJoint = (HorizontalJoint) materialSuper ;

        HorizontalJoint newMs = new HorizontalJoint() ;
        newMs.m_ElementId = id ;
        newMs.m_KodaiName = orgHorizontalJoint.m_KodaiName ;
        newMs.m_Size = size ;
        newMs.m_Material = orgHorizontalJoint.m_Material ;
        newMs.m_TeiketsuType = orgHorizontalJoint.m_TeiketsuType ;
        newMs.m_Bolt1Cnt = orgHorizontalJoint.m_Bolt1Cnt ;
        newMs.m_Bolt2Cnt = orgHorizontalJoint.m_Bolt2Cnt ;
        newMs.m_BoltInfo1 = orgHorizontalJoint.m_BoltInfo1 ;
        newMs.m_BoltInfo2 = orgHorizontalJoint.m_BoltInfo2 ;
        HorizontalJoint.WriteToElement( newMs, doc ) ;
      }
      else if ( materialSuper.GetType() == typeof( VerticalBrace ) ) {
        var orgVerticalBrace = (VerticalBrace) materialSuper ;

        VerticalBrace newMs = new VerticalBrace() ;
        newMs.m_ElementId = id ;
        newMs.m_KodaiName = orgVerticalBrace.m_KodaiName ;
        newMs.m_Size = size ;
        newMs.m_Material = orgVerticalBrace.m_Material ;
        newMs.m_TeiketsuType = orgVerticalBrace.m_TeiketsuType ;
        newMs.m_Bolt1Cnt = orgVerticalBrace.m_Bolt1Cnt ;
        newMs.m_Bolt2Cnt = orgVerticalBrace.m_Bolt2Cnt ;
        newMs.m_BoltInfo1 = orgVerticalBrace.m_BoltInfo1 ;
        newMs.m_BoltInfo2 = orgVerticalBrace.m_BoltInfo2 ;
        VerticalBrace.WriteToElement( newMs, doc ) ;
      }
      else if ( materialSuper.GetType() == typeof( Houdue ) ) {
        var orgHoudue = (Houdue) materialSuper ;

        Houdue newMs = new Houdue() ;
        newMs.m_ElementId = id ;
        newMs.m_KodaiName = orgHoudue.m_KodaiName ;
        newMs.m_Size = size ;
        newMs.m_Material = orgHoudue.m_Material ;
        newMs.m_Syuzai = orgHoudue.m_Syuzai ;
        newMs.m_TeiketsuType = orgHoudue.m_TeiketsuType ;
        newMs.m_Bolt1Cnt = orgHoudue.m_Bolt1Cnt ;
        newMs.m_Bolt2Cnt = orgHoudue.m_Bolt2Cnt ;
        newMs.m_BoltInfo1 = orgHoudue.m_BoltInfo1 ;
        newMs.m_BoltInfo2 = orgHoudue.m_BoltInfo2 ;
        Houdue.WriteToElement( newMs, doc ) ;
      }
      else if ( materialSuper.GetType() == typeof( Jack ) ) {
        var orgJack = (Jack) materialSuper ;

        Jack newMs = new Jack() ;
        newMs.m_ElementId = id ;
        newMs.m_KodaiName = orgJack.m_KodaiName ;
        newMs.m_Size = size ;
        newMs.m_Material = orgJack.m_Material ;
        newMs.m_TeiketsuType = orgJack.m_TeiketsuType ;
        newMs.m_Bolt1Cnt = orgJack.m_Bolt1Cnt ;
        newMs.m_Bolt2Cnt = orgJack.m_Bolt2Cnt ;
        newMs.m_BoltInfo1 = orgJack.m_BoltInfo1 ;
        newMs.m_BoltInfo2 = orgJack.m_BoltInfo2 ;
        Jack.WriteToElement( newMs, doc ) ;
      }
      else if ( materialSuper.GetType() == typeof( JifukuZuredomezai ) ) {
        var orgJifukuZuredomezai = (JifukuZuredomezai) materialSuper ;

        JifukuZuredomezai newMs = new JifukuZuredomezai() ;
        newMs.m_ElementId = id ;
        newMs.m_KodaiName = orgJifukuZuredomezai.m_KodaiName ;
        newMs.m_Size = size ;
        newMs.m_Material = orgJifukuZuredomezai.m_Material ;
        newMs.m_TeiketsuType = orgJifukuZuredomezai.m_TeiketsuType ;
        newMs.m_Bolt1Cnt = orgJifukuZuredomezai.m_Bolt1Cnt ;
        newMs.m_Bolt2Cnt = orgJifukuZuredomezai.m_Bolt2Cnt ;
        newMs.m_BoltInfo1 = orgJifukuZuredomezai.m_BoltInfo1 ;
        newMs.m_BoltInfo2 = orgJifukuZuredomezai.m_BoltInfo2 ;
        JifukuZuredomezai.WriteToElement( newMs, doc ) ;
      }
      else if ( materialSuper.GetType() == typeof( Madumezai ) ) {
        var orgMadumezai = (Madumezai) materialSuper ;

        Madumezai newMs = new Madumezai() ;
        newMs.m_ElementId = id ;
        newMs.m_KodaiName = orgMadumezai.m_KodaiName ;
        newMs.m_Size = size ;
        newMs.m_Material = orgMadumezai.m_Material ;
        newMs.m_K = orgMadumezai.m_K ;
        newMs.m_TeiketsuType = orgMadumezai.m_TeiketsuType ;
        newMs.m_Bolt1Cnt = orgMadumezai.m_Bolt1Cnt ;
        newMs.m_Bolt2Cnt = orgMadumezai.m_Bolt2Cnt ;
        newMs.m_BoltInfo1 = orgMadumezai.m_BoltInfo1 ;
        newMs.m_BoltInfo2 = orgMadumezai.m_BoltInfo2 ;
        Madumezai.WriteToElement( newMs, doc ) ;
      }
      else if ( materialSuper.GetType() == typeof( PilePiller ) ) {
        var orgPilePiller = (PilePiller) materialSuper ;

        PilePiller newMs = new PilePiller() ;
        newMs.m_ElementId = id ;
        newMs.m_KodaiName = orgPilePiller.m_KodaiName ;
        newMs.m_Size = size ;
        newMs.m_Material = orgPilePiller.m_Material ;
        newMs.m_KyoutyouNum = orgPilePiller.m_KyoutyouNum ;
        newMs.m_HukuinNum = orgPilePiller.m_HukuinNum ;
        newMs.m_TeiketsuType = orgPilePiller.m_TeiketsuType ;
        newMs.m_Bolt1Cnt = orgPilePiller.m_Bolt1Cnt ;
        newMs.m_Bolt2Cnt = orgPilePiller.m_Bolt2Cnt ;
        newMs.m_BoltInfo1 = orgPilePiller.m_BoltInfo1 ;
        newMs.m_BoltInfo2 = orgPilePiller.m_BoltInfo2 ;
        PilePiller.WriteToElement( newMs, doc ) ;
      }
      else if ( materialSuper.GetType() == typeof( StiffenerJack ) ) {
        var orgStiffenerJack = (StiffenerJack) materialSuper ;

        StiffenerJack newMs = new StiffenerJack() ;
        newMs.m_ElementId = id ;
        newMs.m_KodaiName = orgStiffenerJack.m_KodaiName ;
        newMs.m_Size = size ;
        newMs.m_Material = orgStiffenerJack.m_Material ;
        newMs.m_H = orgStiffenerJack.m_H ;
        newMs.m_K = orgStiffenerJack.m_K ;
        newMs.m_TeiketsuType = orgStiffenerJack.m_TeiketsuType ;
        newMs.m_Bolt1Cnt = orgStiffenerJack.m_Bolt1Cnt ;
        newMs.m_Bolt2Cnt = orgStiffenerJack.m_Bolt2Cnt ;
        newMs.m_BoltInfo1 = orgStiffenerJack.m_BoltInfo1 ;
        newMs.m_BoltInfo2 = orgStiffenerJack.m_BoltInfo2 ;
        StiffenerJack.WriteToElement( newMs, doc ) ;
      }
      else if ( materialSuper.GetType() == typeof( StiffenerPlate ) ) {
        var orgStiffenerPlate = (StiffenerPlate) materialSuper ;

        StiffenerPlate newMs = new StiffenerPlate() ;
        newMs.m_ElementId = id ;
        newMs.m_KodaiName = orgStiffenerPlate.m_KodaiName ;
        newMs.m_Size = size ;
        newMs.m_Material = orgStiffenerPlate.m_Material ;
        newMs.m_H = orgStiffenerPlate.m_H ;
        newMs.m_K = orgStiffenerPlate.m_K ;
        newMs.m_TeiketsuType = orgStiffenerPlate.m_TeiketsuType ;
        newMs.m_Bolt1Cnt = orgStiffenerPlate.m_Bolt1Cnt ;
        newMs.m_Bolt2Cnt = orgStiffenerPlate.m_Bolt2Cnt ;
        newMs.m_BoltInfo1 = orgStiffenerPlate.m_BoltInfo1 ;
        newMs.m_BoltInfo2 = orgStiffenerPlate.m_BoltInfo2 ;
        StiffenerPlate.WriteToElement( newMs, doc ) ;
      }
      else if ( materialSuper.GetType() == typeof( Taikeikou ) ) {
        var orgTaikeikou = (Taikeikou) materialSuper ;

        Taikeikou newMs = new Taikeikou() ;
        newMs.m_ElementId = id ;
        newMs.m_KodaiName = orgTaikeikou.m_KodaiName ;
        newMs.m_Size = size ;
        newMs.m_Material = orgTaikeikou.m_Material ;
        newMs.m_JointType = orgTaikeikou.m_JointType ;
        newMs.m_BoltCount = orgTaikeikou.m_BoltCount ;
        newMs.m_BoltType = orgTaikeikou.m_BoltType ;
        newMs.m_TeiketsuType = orgTaikeikou.m_TeiketsuType ;
        newMs.m_Bolt1Cnt = orgTaikeikou.m_Bolt1Cnt ;
        newMs.m_Bolt2Cnt = orgTaikeikou.m_Bolt2Cnt ;
        newMs.m_BoltInfo1 = orgTaikeikou.m_BoltInfo1 ;
        newMs.m_BoltInfo2 = orgTaikeikou.m_BoltInfo2 ;
        Taikeikou.WriteToElement( newMs, doc ) ;
      }
      else if ( materialSuper.GetType() == typeof( TaikeikouPL ) ) {
        var orgTaikeikouPL = (TaikeikouPL) materialSuper ;

        TaikeikouPL newMs = new TaikeikouPL() ;
        newMs.m_ElementId = id ;
        newMs.m_KodaiName = orgTaikeikouPL.m_KodaiName ;
        newMs.m_Size = size ;
        newMs.m_Material = orgTaikeikouPL.m_Material ;
        newMs.m_TeiketsuType = orgTaikeikouPL.m_TeiketsuType ;
        newMs.m_Bolt1Cnt = orgTaikeikouPL.m_Bolt1Cnt ;
        newMs.m_Bolt2Cnt = orgTaikeikouPL.m_Bolt2Cnt ;
        newMs.m_BoltInfo1 = orgTaikeikouPL.m_BoltInfo1 ;
        newMs.m_BoltInfo2 = orgTaikeikouPL.m_BoltInfo2 ;
        StiffenerPlate.WriteToElement( newMs, doc ) ;
      }
      else if ( materialSuper.GetType() == typeof( Tensetsuban ) ) {
        var orgTensetsuban = (Tensetsuban) materialSuper ;

        Tensetsuban newMs = new Tensetsuban() ;
        newMs.m_ElementId = id ;
        newMs.m_KodaiName = orgTensetsuban.m_KodaiName ;
        newMs.m_Size = size ;
        newMs.m_Material = orgTensetsuban.m_Material ;
        newMs.m_H = orgTensetsuban.m_H ;
        newMs.m_K = orgTensetsuban.m_K ;
        newMs.m_TeiketsuType = orgTensetsuban.m_TeiketsuType ;
        newMs.m_Bolt1Cnt = orgTensetsuban.m_Bolt1Cnt ;
        newMs.m_Bolt2Cnt = orgTensetsuban.m_Bolt2Cnt ;
        newMs.m_BoltInfo1 = orgTensetsuban.m_BoltInfo1 ;
        newMs.m_BoltInfo2 = orgTensetsuban.m_BoltInfo2 ;
        Tensetsuban.WriteToElement( newMs, doc ) ;
      }
      else if ( materialSuper.GetType() == typeof( Tesuri ) ) {
        var orgTesuri = (Tesuri) materialSuper ;

        Tesuri newMs = new Tesuri() ;
        newMs.m_ElementId = id ;
        newMs.m_KodaiName = orgTesuri.m_KodaiName ;
        newMs.m_Size = size ;
        newMs.m_Material = orgTesuri.m_Material ;
        newMs.m_TeiketsuType = orgTesuri.m_TeiketsuType ;
        newMs.m_Bolt1Cnt = orgTesuri.m_Bolt1Cnt ;
        newMs.m_Bolt2Cnt = orgTesuri.m_Bolt2Cnt ;
        newMs.m_BoltInfo1 = orgTesuri.m_BoltInfo1 ;
        newMs.m_BoltInfo2 = orgTesuri.m_BoltInfo2 ;
        Tesuri.WriteToElement( newMs, doc ) ;
      }
      else if ( materialSuper.GetType() == typeof( TesuriShichu ) ) {
        var orgTesuriShichu = (TesuriShichu) materialSuper ;

        TesuriShichu newMs = new TesuriShichu() ;
        newMs.m_ElementId = id ;
        newMs.m_KodaiName = orgTesuriShichu.m_KodaiName ;
        newMs.m_Size = size ;
        newMs.m_Material = orgTesuriShichu.m_Material ;
        newMs.m_TeiketsuType = orgTesuriShichu.m_TeiketsuType ;
        newMs.m_Bolt1Cnt = orgTesuriShichu.m_Bolt1Cnt ;
        newMs.m_Bolt2Cnt = orgTesuriShichu.m_Bolt2Cnt ;
        newMs.m_BoltInfo1 = orgTesuriShichu.m_BoltInfo1 ;
        newMs.m_BoltInfo2 = orgTesuriShichu.m_BoltInfo2 ;
        TesuriShichu.WriteToElement( newMs, doc ) ;
      }
      else if ( materialSuper.GetType() == typeof( ExtensionPile ) ) {
        var orgExtensionPile = (ExtensionPile) materialSuper ;

        ExtensionPile newMs = new ExtensionPile() ;
        newMs.m_ElementId = id ;
        newMs.m_KodaiName = orgExtensionPile.m_KodaiName ;
        newMs.m_Size = size ;
        newMs.m_Material = orgExtensionPile.m_Material ;
        newMs.m_HukuinNum = orgExtensionPile.m_HukuinNum ;
        newMs.m_KyoutyouNum = orgExtensionPile.m_KyoutyouNum ;
        newMs.m_TeiketsuType = orgExtensionPile.m_TeiketsuType ;
        newMs.m_Bolt1Cnt = orgExtensionPile.m_Bolt1Cnt ;
        newMs.m_Bolt2Cnt = orgExtensionPile.m_Bolt2Cnt ;
        newMs.m_BoltInfo1 = orgExtensionPile.m_BoltInfo1 ;
        newMs.m_BoltInfo2 = orgExtensionPile.m_BoltInfo2 ;
        ExtensionPile.WriteToElement( newMs, doc ) ;
      }
    }

    public Result ExcuteOfSizeCategory()
    {
      FrmEditSizeList f = Application.thisApp.frmEditSizeList ;

      try {
        using ( TransactionGroup tr = new TransactionGroup( _doc ) ) {
          tr.Start( "Size Category List Change" ) ;

          foreach ( DataGridViewRow row in f.DgvEditSizeCategoryList.Rows ) {
            var afterSizeCell = row.Cells[ "ColSizeCategoryListAfterSize" ] as DataGridViewComboBoxCell ;
            var afterSize = afterSizeCell.Value?.ToString() ;

            if ( string.IsNullOrEmpty( afterSize ) ) continue ;

            var koudaiNameCell = row.Cells[ "ColSizeCategoryListKoudai" ] as DataGridViewTextBoxCell ;
            var categoryCell = row.Cells[ "ColSizeCategoryListCategory" ] as DataGridViewTextBoxCell ;
            var koudaiName = koudaiNameCell.Value.ToString() ;
            var category = categoryCell.Value.ToString() ;

            // 図面上の全ファミリシンボルを取得
            var targetFsList = GantryUtil.GetAllFamilySymbol( _doc ) ;

            var chengedBuzaiCategory = new List<DefineUtil.eBuzaiCategory>() ;
            foreach ( var targetFs in targetFsList ) {
              // 対象のファミリシンボルに属するファミリインスタンスを取得
              var targetFiList = GantryUtil.GetFamilyInstanceList( _doc, targetFs ) ;

              var daiBunrui = GantryUtil.GetTypeParameter( targetFs, DefineUtil.PARAM_DAI_BUNRUI ) ;
              var chuuBunrui = GantryUtil.GetTypeParameter( targetFs, DefineUtil.PARAM_CHUU_BUNRUI ) ;
              var shouBunrui = GantryUtil.GetTypeParameter( targetFs, DefineUtil.PARAM_SHOU_BUNRUI ) ;

              DefineUtil.eBuzaiCategory eBuzaiCategory = DefineUtil.eBuzaiCategory.None ;

              foreach ( var targetFi in targetFiList ) {
                // 部材情報を取得
                var material = MaterialSuper.ReadFromElement( targetFi.Id, _doc ) ;
                if ( material == null ) continue ;

                // 構台名が一致しない場合はスキップ
                if ( material.m_KodaiName != koudaiName ) continue ;

                // 変更対象の種別でない場合はスキップ
                if ( category == f.CATEGORY_SHICHU ) {
                  if ( material.GetType() != typeof( PilePiller ) ) continue ;
                  if ( ! ( ( ( daiBunrui == f.DAIBUNRUI_KOUDAI_1 || daiBunrui == f.DAIBUNRUI_KOUDAI_2 ) &&
                             chuuBunrui == f.CHUUBUNRUI_SHUZAI && shouBunrui == f.SHOUBUNRUI_SHICHU_SOZAI ) ||
                           ( ( daiBunrui == f.DAIBUNRUI_KOUDAI_1 || daiBunrui == f.DAIBUNRUI_KOUDAI_2 ) &&
                             chuuBunrui == f.CHUUBUNRUI_SHUZAI && shouBunrui == f.SHOUBUNRUI_SHICHU_YAMADOME ) ||
                           ( ( daiBunrui == f.DAIBUNRUI_KOUDAI_1 || daiBunrui == f.DAIBUNRUI_KOUDAI_2 ) &&
                             chuuBunrui == f.CHUUBUNRUI_SHUZAI &&
                             shouBunrui == f.SHOUBUNRUI_SHICHU_KOUKYOUDOYAMADOME ) ) ) {
                    continue ;
                  }

                  eBuzaiCategory = DefineUtil.eBuzaiCategory.ShichuShijikui ;
                }
                else if ( category == f.CATEGORY_SHIJIKUI ) {
                  if ( ! ( material.GetType() == typeof( PilePiller ) ||
                           material.GetType() == typeof( ExtensionPile ) ) ) continue ;
                  if ( ! ( ( daiBunrui == f.DAIBUNRUI_YAMADOMEHEKI && chuuBunrui == f.CHUUBUNRUI_SHIJIKUI &&
                             shouBunrui == f.SHOUBUNRUI_KUI ) || ( daiBunrui == f.DAIBUNRUI_KOUDAIKUI &&
                                                                   chuuBunrui == f.CHUUBUNRUI_TSUGITASHIKUI &&
                                                                   shouBunrui == f.SHOUBUNRUI_TSUGITASHIKUI ) ) ) {
                    continue ;
                  }

                  eBuzaiCategory = DefineUtil.eBuzaiCategory.ShichuShijikui ;
                }
                else if ( category == f.CATEGORY_OHBIKI || category == f.CATEGORY_KETAUKE ) {
                  if ( material.GetType() != typeof( Ohbiki ) ) continue ;
                  eBuzaiCategory = DefineUtil.eBuzaiCategory.OhbikiKetauke ;
                }
                else if ( category == f.CATEGORY_SHIKIGETA ) {
                  if ( material.GetType() != typeof( Shikigeta ) ) continue ;
                  eBuzaiCategory = DefineUtil.eBuzaiCategory.Shikigeta ;
                }
                else if ( category == f.CATEGORY_NEDA || category == f.CATEGORY_SHUGETA ) {
                  if ( material.GetType() != typeof( Neda ) ) continue ;
                  eBuzaiCategory = DefineUtil.eBuzaiCategory.NedaShugeta ;
                }
                else if ( category == f.CATEGORY_JIFUKU || category == f.CATEGORY_FUKKOUBANZUREDOMEZAI ) {
                  if ( material.GetType() != typeof( JifukuZuredomezai ) ) continue ;
                  eBuzaiCategory = DefineUtil.eBuzaiCategory.JifukuZuredome ;
                }
                else {
                  continue ;
                }

                // 変更前の部材の大元のタイプ名を取得する
                var originalType = GantryUtil.GetOriginalTypeName( _doc, targetFi ) ;

                var _path = Master.ClsMasterCsv.GetFamilyPathBySize( afterSize, originalType ) ;
                GantryUtil.GetFamilySymbol( _doc, _path, Path.GetFileNameWithoutExtension( _path ),
                  out FamilySymbol afterFamilySymbol, false ) ;

                if ( afterFamilySymbol == null ) {
                  var isPile = false ;
                  var isPilePiller = false ;
                  var isExPile = false ;

                  if ( originalType.Contains( "支持杭" ) ) {
                    isPile = true ;
                  }
                  else if ( originalType.Contains( "支柱" ) ) {
                    isPilePiller = true ;
                  }
                  else if ( originalType.Contains( "継ぎ足し杭" ) ) {
                    isExPile = true ;
                  }

                  // 変更後サイズのファミリパス取得
                  var afterFamilyPath = Master.ClsMasterCsv.GetFamilyPath( afterSize, bPile: isPile,
                    bPilePiller: isPilePiller, bExPile: isExPile ) ;
                  // 変更後サイズのファミリシンボル取得
                  GantryUtil.GetFamilySymbol( _doc, afterFamilyPath, originalType, out afterFamilySymbol, false ) ;

                  if ( afterFamilySymbol == null ) {
                    var _type = Master.ClsMasterCsv.GetFamilyTypeBySize( afterSize ) ;
                    if ( string.IsNullOrEmpty( _type ) ) {
                      // タイプが取得できない場合
                      GantryUtil.GetFamilySymbol( _doc, afterFamilyPath, afterSize, out afterFamilySymbol, false ) ;
                    }
                    else {
                      // 再度変更後サイズのファミリシンボル取得
                      GantryUtil.GetFamilySymbol( _doc, afterFamilyPath, _type, out afterFamilySymbol, false ) ;
                    }
                  }
                }
                else {
                  originalType = Path.GetFileNameWithoutExtension( _path ) ;
                }

                if ( afterFamilySymbol == null ) continue ;

                using ( Transaction rnTr = new Transaction( _doc ) ) {
                  rnTr.Start( "Rename Type" ) ;
                  // 変更後サイズのタイプを複製（タイプ複製が不要な部材に関しては複製しない）
                  afterFamilySymbol = DuplicateFamilySymbol( _doc, material, afterFamilySymbol, originalType ) ;
                  rnTr.Commit() ;
                }

                ElementId createdID = ElementId.InvalidElementId ;

                using ( Transaction scTr = new Transaction( _doc ) ) {
                  scTr.Start( "Size Change" ) ;
                  // 警告を制御する処理（部材を削除する前に同一点に部材を置くとコミット時に警告が出るので対応）
                  FailureHandlingOptions failOpt = scTr.GetFailureHandlingOptions() ;
                  failOpt.SetFailuresPreprocessor( new WarningSwallower() ) ;
                  scTr.SetFailureHandlingOptions( failOpt ) ;

                  // サイズ変更
                  createdID = SizeChange( _doc, targetFi, material, afterFamilySymbol ) ;
                  if ( createdID == null ) continue ;
                  scTr.Commit() ;
                }

                using ( Transaction atTr = new Transaction( _doc ) ) {
                  atTr.Start( "Attach Material" ) ;
                  // 変更後の部材の部材情報を共通パラメータに書き込み
                  AttachMaterialSuper( _doc, createdID, material, afterSize ) ;
                  atTr.Commit() ;
                }

                // 変更後サイズのファミリインスタンス取得
                var afterFamilyInstance = GantryUtil.GetFamilyInstance( _doc, createdID ) ;
                // 元の部材の連動対象となる部材をサイズ変更
                ReplaceIntersectElement( targetFi, material, afterFamilyInstance, true ) ;
                // 元の部材を参照していた部材を新たな参照にて置き直し
                ReplaceReferenceDestinationElement( targetFi, material, afterFamilyInstance ) ;

                using ( Transaction meTr = new Transaction( _doc ) ) {
                  meTr.Start( "Move Element" ) ;
                  // 元の部材の連動対象となる部材を位置移動
                  ReplaceIntersectElementOfLocation( targetFi, material, afterFamilyInstance, eBuzaiCategory,
                    chengedBuzaiCategory ) ;
                  meTr.Commit() ;
                }

                if ( ! chengedBuzaiCategory.Contains( eBuzaiCategory ) ) {
                  chengedBuzaiCategory.Add( eBuzaiCategory ) ;
                }

                var element = _doc.GetElement( targetFi.Id ) ;
                if ( element == null ) continue ;

                // 対象部材一覧データの更新
                var index = f._targetElementUniqueIdList.IndexOf( element.UniqueId ) ;
                f._targetElementUniqueIdList.Remove( element.UniqueId ) ;
                var afterFiElement = _doc.GetElement( afterFamilyInstance.Id ) ;
                if ( index != -1 ) {
                  f._targetElementUniqueIdList.Insert( index, afterFiElement.UniqueId ) ;
                }

                using ( Transaction dlTr = new Transaction( _doc ) ) {
                  dlTr.Start( "Delete Element" ) ;
                  // 変更前の部材の削除
                  _doc.Delete( targetFi.Id ) ;
                  dlTr.Commit() ;
                }
              }
            }
          }

          tr.Assimilate() ;
        }
      }
      catch ( Autodesk.Revit.Exceptions.OperationCanceledException ) {
        return Result.Cancelled ;
      }
      catch ( Exception ex ) {
        MessageUtil.Error( $"サイズ変更に失敗しました。", COMMAND_NAME, f ) ;
        _logger.Error( ex.Message ) ;
        return Result.Failed ;
      }

      // 種別一覧データグリッドビューのレコードを更新
      f.SearchSizeCategoryList() ;

      return Result.Succeeded ;
    }

    private void ReplaceIntersectElementOfLocation( FamilyInstance beforeFamilyInstance, MaterialSuper material,
      FamilyInstance afterFamilyInstance, DefineUtil.eBuzaiCategory eBuzaiCategory,
      List<DefineUtil.eBuzaiCategory> chengedBuzaiCategory )
    {
      var koudaiName = material.m_KodaiName ;
      var koudaiData = GantryUtil.GetKoudaiData( _doc, koudaiName ) ;
      if ( koudaiData == null ) return ;

      var isCangeInteresectElement = ! chengedBuzaiCategory.Contains( eBuzaiCategory ) ;

      if ( koudaiData.AllKoudaiFlatData.BaseLevel == DefineUtil.eBaseLevel.OhbikiBtm ) {
        // 配置基準が「大引下端」の場合

        if ( eBuzaiCategory == DefineUtil.eBuzaiCategory.OhbikiKetauke ) {
          // サイズ変更対象の部材カテゴリが「大引(桁受)」の場合

          var beforeMaterial = Ohbiki.ReadFromElement( beforeFamilyInstance.Id, _doc ) ;
          var beforeSizeH = beforeMaterial.MaterialSize().Height ;
          var afterMaterial = Ohbiki.ReadFromElement( afterFamilyInstance.Id, _doc ) ;
          var afterSizeH = afterMaterial.MaterialSize().Height ;

          var moveValue = Math.Abs( ( beforeSizeH - afterSizeH ) / 2 ) ;

          var offset =
            ClsRevitUtil.CovertFromAPI(
              ClsRevitUtil.GetParameterDouble( _doc, afterFamilyInstance.Id, DefineUtil.PARAM_HOST_OFFSET ) ) ;
          var changeOffset = offset + moveValue ;

          if ( beforeSizeH > afterSizeH ) {
            // 変更前サイズ(H)の方が大きい場合（サイズが小さくなる）
            changeOffset = offset - moveValue ;
          }

          ClsRevitUtil.SetParameter( _doc, afterFamilyInstance.Id, DefineUtil.PARAM_HOST_OFFSET,
            ClsRevitUtil.CovertToAPI( changeOffset ) ) ;

          if ( isCangeInteresectElement ) {
            // 大引の基点を取得
            var beforeMaterialPosition = beforeMaterial.Position ;
            var beforeSizeFlangeH = beforeMaterial.SteelSize.FrangeThick ;
            var afterSizeFlangeH = afterMaterial.SteelSize.FrangeThick ;


            // 根太の位置変更処理
            var nedaFiList = GetFamilyInscanceWith( koudaiName, FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_1,
              FrmEditSizeList.CONST_CHUUBUNRUI_KETAZAI, FrmEditSizeList.CONST_SHOUBUNRUI_NEDA,
              FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_2 ) ;
            foreach ( var nedaFi in nedaFiList ) {
              SetHostOffsetByMoveValue( nedaFi, beforeSizeH, afterSizeH, moveValue ) ;
            }

            // 主桁の位置変更処理
            var shugetaFiList = GetFamilyInscanceWith( koudaiName, FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_1,
              FrmEditSizeList.CONST_CHUUBUNRUI_KETAZAI, FrmEditSizeList.CONST_SHOUBUNRUI_SHUGETA,
              FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_2 ) ;
            foreach ( var shugetaFi in shugetaFiList ) {
              SetHostOffsetByMoveValue( shugetaFi, beforeSizeH, afterSizeH, moveValue ) ;
            }

            // 覆工板の位置変更処理
            var fukkoubanFiList = GetFamilyInscanceWith( koudaiName, FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_1,
              FrmEditSizeList.CONST_CHUUBUNRUI_FUKKOUBAN, FrmEditSizeList.CONST_SHOUBUNRUI_FUKKOUBAN,
              FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_2 ) ;
            foreach ( var fukkoubanFi in fukkoubanFiList ) {
              SetHostOffsetByMoveValue( fukkoubanFi, beforeSizeH, afterSizeH, moveValue ) ;
            }

            // 覆工桁の位置変更 ※ ホストに根太(主桁)を保持しているため処理不要
            // 地覆の位置変更 ※ ホストに根太(主桁)を保持しているため処理不要
            // 手摺の位置変更 ※ ホストに根太(主桁)を保持しているため処理不要
            // 手摺支柱の位置変更 ※ ホストに根太(主桁)を保持しているため処理不要
            // 覆工板ズレ止め材の位置変更 ※ ホストに根太(主桁)を保持しているため処理不要

            // 対傾構取付ﾌﾟﾚｰﾄの位置変更
            var taikeikouPLFiList = GetFamilyInscanceWith( koudaiName, FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_1,
              FrmEditSizeList.CONST_CHUUBUNRUI_PL, FrmEditSizeList.CONST_SHOUBUNRUI_TAIKEIKOUPL,
              FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_2 ) ;
            foreach ( var taikeikouPLFi in taikeikouPLFiList ) {
              SetHostOffsetByMoveValue( taikeikouPLFi, beforeSizeH, afterSizeH, moveValue ) ;
            }

            // 対傾構の位置変更
            var taikeikouFiList = GetFamilyInscanceWith( koudaiName, FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_1,
              FrmEditSizeList.CONST_CHUUBUNRUI_KETAZAI, FrmEditSizeList.CONST_SHOUBUNRUI_TAIKEIKOU,
              FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_2 ) ;
            foreach ( var taikeikouFi in taikeikouFiList ) {
              SetHostOffsetByMoveValue( taikeikouFi, beforeSizeH, afterSizeH, moveValue ) ;
            }

            // 水平ブレースの位置変更
            var braceFiList = GetFamilyInscanceWith( koudaiName, FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_1,
              FrmEditSizeList.CONST_CHUUBUNRUI_KETAZAI, FrmEditSizeList.CONST_SHOUBUNRUI_BRACE,
              FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_2 ) ;
            foreach ( var braceFi in braceFiList ) {
              var _material = MaterialSuper.ReadFromElement( braceFi.Id, _doc ) ;
              // 水平ブレース以外はスキップ
              if ( _material.GetType() != typeof( HorizontalBrace ) ) continue ;

              // フランジの両端点を取得
              var endPoints = GantryUtil.GetCurvePoints( _doc, braceFi.Id ) ;
              if ( endPoints.Count == 2 ) {
                var nearEndPoint =
                  ( beforeMaterialPosition.DistanceTo( endPoints[ 0 ] ) >
                    beforeMaterialPosition.DistanceTo( endPoints[ 1 ] ) )
                    ? endPoints[ 1 ]
                    : endPoints[ 0 ] ;


                if ( beforeMaterialPosition.Z > nearEndPoint.Z ) {
                  var tr = braceFi.GetTransform() ;
                  var xyz = tr.BasisZ ;
                  if ( ClsGeo.GEO_GT0( xyz.Z ) ) {
                    // 下フランジ下側
                    continue ;
                  }
                  else {
                    // 下フランジ上側
                    var diffFlangeSize = beforeSizeFlangeH - afterSizeFlangeH ;

                    if ( beforeSizeFlangeH > afterSizeFlangeH ) {
                      diffFlangeSize = diffFlangeSize * -1 ;
                    }

                    SetHostOffsetByMoveValue( braceFi, beforeSizeH, afterSizeH, diffFlangeSize, false ) ;

                    continue ;
                  }
                }
                else {
                  // 上フランジ下側
                  var diffFlangeSize = beforeSizeFlangeH - afterSizeFlangeH ;

                  if ( beforeSizeFlangeH > afterSizeFlangeH ) {
                    diffFlangeSize = diffFlangeSize * -1 ;
                  }

                  diffFlangeSize = ( moveValue * 2 ) + diffFlangeSize ;

                  SetHostOffsetByMoveValue( braceFi, beforeSizeH, afterSizeH, diffFlangeSize, false ) ;
                }
              }
            }
          }

          // スチフナージャッキの位置変更処理
          var suchifunaJackFiList = GetFamilyInscanceWith( koudaiName, FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_1,
            FrmEditSizeList.CONST_CHUUBUNRUI_SONOTA, FrmEditSizeList.CONST_SHOUBUNRUI_SUCHIFUNAJACK,
            FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_2 ) ;
          foreach ( var suchifunaJackFi in suchifunaJackFiList ) {
            var hostElement = suchifunaJackFi.Host ;
            var targetHostFi = GantryUtil.GetFamilyInstance( _doc, hostElement.Id ) ;
            var targetHostMaterial = MaterialSuper.ReadFromElement( targetHostFi.Id, _doc ) ;
            var ohbikiHeight = afterMaterial.SteelSize.Height ;
            var ohbikiFrangeThick = afterMaterial.SteelSize.FrangeThick ;

            if ( targetHostMaterial.GetType() == typeof( Ohbiki ) ) {
              var _offset = ( ohbikiHeight - ohbikiFrangeThick ) * -1 ;

              ClsRevitUtil.SetParameter( _doc, suchifunaJackFi.Id, DefineUtil.PARAM_HOST_OFFSET,
                ClsRevitUtil.CovertToAPI( _offset ) ) ;
            }
          }
        }
        else if ( eBuzaiCategory == DefineUtil.eBuzaiCategory.NedaShugeta ) {
          // サイズ変更対象の部材カテゴリが「根太(主桁)」の場合

          var beforeMaterial = Neda.ReadFromElement( beforeFamilyInstance.Id, _doc ) ;
          var beforeSizeH = beforeMaterial.MaterialSize().Height ;
          var afterMaterial = Neda.ReadFromElement( afterFamilyInstance.Id, _doc ) ;
          var afterSizeH = afterMaterial.MaterialSize().Height ;

          var moveValue = Math.Abs( ( beforeSizeH - afterSizeH ) / 2 ) ;

          var offset =
            ClsRevitUtil.CovertFromAPI(
              ClsRevitUtil.GetParameterDouble( _doc, afterFamilyInstance.Id, DefineUtil.PARAM_HOST_OFFSET ) ) ;
          var changeOffset = offset + moveValue ;

          if ( beforeSizeH > afterSizeH ) {
            // 変更前サイズ(H)の方が大きい場合（サイズが小さくなる）
            changeOffset = offset - moveValue ;
          }

          ClsRevitUtil.SetParameter( _doc, afterFamilyInstance.Id, DefineUtil.PARAM_HOST_OFFSET,
            ClsRevitUtil.CovertToAPI( changeOffset ) ) ;

          if ( isCangeInteresectElement ) {
            // 覆工板の位置変更
            var fukkoubanFiList = GetFamilyInscanceWith( koudaiName, FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_1,
              FrmEditSizeList.CONST_CHUUBUNRUI_FUKKOUBAN, FrmEditSizeList.CONST_SHOUBUNRUI_FUKKOUBAN,
              FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_2 ) ;
            foreach ( var fukkoubanFi in fukkoubanFiList ) {
              SetHostOffsetByMoveValue( fukkoubanFi, beforeSizeH, afterSizeH, moveValue ) ;
            }

            // 覆工桁の位置変更 ※ ホストに根太(主桁)を保持しているため処理不要
            // 地覆の位置変更  ※ ホストに根太(主桁)を保持しているここの処理不要
            // 手摺の位置変更  ※ ホストに根太(主桁)を保持しているため処理不要

            // 手摺支柱の位置変更
            var tesuriShichuFiList = GetFamilyInscanceWith( koudaiName, FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_1,
              FrmEditSizeList.CONST_CHUUBUNRUI_SONOTA, FrmEditSizeList.CONST_SHOUBUNRUI_TESURISHICHU,
              FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_2 ) ;
            foreach ( var tesuriShichuFi in tesuriShichuFiList ) {
              SetHostOffsetByMoveValue( tesuriShichuFi, beforeSizeH, afterSizeH, moveValue, false ) ;
            }

            // 覆工板ズレ止め材の位置変更
            var fukkoubanZuredomeFiList = GetFamilyInscanceWith( koudaiName, FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_1,
              FrmEditSizeList.CONST_CHUUBUNRUI_SONOTA, FrmEditSizeList.CONST_SHOUBUNRUI_FUKKOUBANZUREDOMEZAI,
              FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_2 ) ;
            foreach ( var fukkoubanZuredomeFi in fukkoubanZuredomeFiList ) {
              SetHostOffsetByMoveValue( fukkoubanZuredomeFi, beforeSizeH, afterSizeH, moveValue, false ) ;
            }

            // 対傾構の位置変更
            var taikeikouFiList = GetFamilyInscanceWith( koudaiName, FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_1,
              FrmEditSizeList.CONST_CHUUBUNRUI_KETAZAI, FrmEditSizeList.CONST_SHOUBUNRUI_TAIKEIKOU,
              FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_2 ) ;
            foreach ( var taikeikouFi in taikeikouFiList ) {
              SetHostOffsetByMoveValue( taikeikouFi, beforeSizeH, afterSizeH, moveValue, false ) ;
            }
          }

          // スチフナージャッキの位置変更処理
          var suchifunaJackFiList = GetFamilyInscanceWith( koudaiName, FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_1,
            FrmEditSizeList.CONST_CHUUBUNRUI_SONOTA, FrmEditSizeList.CONST_SHOUBUNRUI_SUCHIFUNAJACK,
            FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_2 ) ;
          foreach ( var suchifunaJackFi in suchifunaJackFiList ) {
            var hostElement = suchifunaJackFi.Host ;
            var targetHostFi = GantryUtil.GetFamilyInstance( _doc, hostElement.Id ) ;
            var targetHostMaterial = MaterialSuper.ReadFromElement( targetHostFi.Id, _doc ) ;
            var nedaHeight = afterMaterial.SteelSize.Height ;
            var nedaFrangeThick = afterMaterial.SteelSize.FrangeThick ;

            if ( targetHostMaterial.GetType() == typeof( Neda ) ) {
              var _offset = ( nedaHeight - nedaFrangeThick ) * -1 ;

              ClsRevitUtil.SetParameter( _doc, suchifunaJackFi.Id, DefineUtil.PARAM_HOST_OFFSET,
                ClsRevitUtil.CovertToAPI( _offset ) ) ;
            }
          }
        }
        else if ( eBuzaiCategory == DefineUtil.eBuzaiCategory.JifukuZuredome ) {
          // サイズ変更対象の部材カテゴリが「覆工板ズレ止め(地覆)」の場合

          var beforeMaterial = JifukuZuredomezai.ReadFromElement( beforeFamilyInstance.Id, _doc ) ;
          var beforeSizeH = beforeMaterial.MaterialSize().Height ;
          var afterMaterial = JifukuZuredomezai.ReadFromElement( afterFamilyInstance.Id, _doc ) ;
          var afterSizeH = afterMaterial.MaterialSize().Height ;

          var moveValue = Math.Abs( ( beforeSizeH - afterSizeH ) / 2 ) ;

          var offset =
            ClsRevitUtil.CovertFromAPI(
              ClsRevitUtil.GetParameterDouble( _doc, afterFamilyInstance.Id, DefineUtil.PARAM_HOST_OFFSET ) ) ;
          var changeOffset = offset + moveValue ;

          if ( beforeSizeH > afterSizeH ) {
            // 変更前サイズ(H)の方が大きい場合（サイズが小さくなる）
            changeOffset = offset - moveValue ;
          }

          ClsRevitUtil.SetParameter( _doc, afterFamilyInstance.Id, DefineUtil.PARAM_HOST_OFFSET,
            ClsRevitUtil.CovertToAPI( changeOffset ) ) ;

          if ( isCangeInteresectElement ) {
            // 手摺の位置変更 ※ ホストに地覆・覆工板ズレ止め材を保持しているため処理不要

            // 手摺支柱の位置変更処理
            var tesuriShichuFiList = GetFamilyInscanceWith( koudaiName, FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_1,
              FrmEditSizeList.CONST_CHUUBUNRUI_SONOTA, FrmEditSizeList.CONST_SHOUBUNRUI_TESURISHICHU,
              FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_2 ) ;
            foreach ( var tesuriShichuFi in tesuriShichuFiList ) {
              SetHostOffsetByMoveValue( tesuriShichuFi, beforeSizeH, afterSizeH, moveValue, false ) ;
            }
          }
        }
        else if ( eBuzaiCategory == DefineUtil.eBuzaiCategory.Shikigeta ) {
          var beforeMaterial = Shikigeta.ReadFromElement( beforeFamilyInstance.Id, _doc ) ;
          var beforeSizeH = beforeMaterial.MaterialSize().Height ;
          var afterMaterial = Shikigeta.ReadFromElement( afterFamilyInstance.Id, _doc ) ;
          var afterSizeH = afterMaterial.MaterialSize().Height ;

          //var moveValue = Math.Abs((beforeSizeH - afterSizeH) / 2);

          //var offset = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetParameterDouble(_doc, afterFamilyInstance.Id, DefineUtil.PARAM_HOST_OFFSET));
          //var changeOffset = offset - moveValue;

          //if (beforeSizeH > afterSizeH)
          //{
          //    // 変更前サイズ(H)の方が大きい場合（サイズが小さくなる）
          //    changeOffset = offset + moveValue;
          //}

          //ClsRevitUtil.SetParameter(_doc, afterFamilyInstance.Id, DefineUtil.PARAM_HOST_OFFSET, ClsRevitUtil.CovertToAPI(changeOffset));

          // スチフナージャッキの位置変更処理
          var suchifunaJackFiList = GetFamilyInscanceWith( koudaiName, FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_1,
            FrmEditSizeList.CONST_CHUUBUNRUI_SONOTA, FrmEditSizeList.CONST_SHOUBUNRUI_SUCHIFUNAJACK,
            FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_2 ) ;
          foreach ( var suchifunaJackFi in suchifunaJackFiList ) {
            var hostElement = suchifunaJackFi.Host ;
            var targetHostFi = GantryUtil.GetFamilyInstance( _doc, hostElement.Id ) ;
            var targetHostMaterial = MaterialSuper.ReadFromElement( targetHostFi.Id, _doc ) ;
            var shikigetaHeight = afterMaterial.SteelSize.Height ;
            var shikigetaFrangeThick = afterMaterial.SteelSize.FrangeThick ;

            if ( targetHostMaterial.GetType() == typeof( Shikigeta ) ) {
              var _offset = ( shikigetaHeight - shikigetaFrangeThick ) * -1 ;

              ClsRevitUtil.SetParameter( _doc, suchifunaJackFi.Id, DefineUtil.PARAM_HOST_OFFSET,
                ClsRevitUtil.CovertToAPI( _offset ) ) ;
            }
          }
        }
      }
      else if ( koudaiData.AllKoudaiFlatData.BaseLevel == DefineUtil.eBaseLevel.FukkouTop ) {
        // 配置基準が「覆工天端」の場合

        if ( eBuzaiCategory == DefineUtil.eBuzaiCategory.OhbikiKetauke ) {
          // サイズ変更対象の部材カテゴリが「大引(桁受)」の場合

          var beforeMaterial = Ohbiki.ReadFromElement( beforeFamilyInstance.Id, _doc ) ;
          var beforeSizeH = beforeMaterial.MaterialSize().Height ;
          var afterMaterial = Ohbiki.ReadFromElement( afterFamilyInstance.Id, _doc ) ;
          var afterSizeH = afterMaterial.MaterialSize().Height ;

          var moveValue = Math.Abs( ( beforeSizeH - afterSizeH ) / 2 ) ;

          var offset =
            ClsRevitUtil.CovertFromAPI(
              ClsRevitUtil.GetParameterDouble( _doc, afterFamilyInstance.Id, DefineUtil.PARAM_HOST_OFFSET ) ) ;
          var changeOffset = offset - moveValue ;

          if ( beforeSizeH > afterSizeH ) {
            // 変更前サイズ(H)の方が大きい場合（サイズが小さくなる）
            changeOffset = offset + moveValue ;
          }

          ClsRevitUtil.SetParameter( _doc, afterFamilyInstance.Id, DefineUtil.PARAM_HOST_OFFSET,
            ClsRevitUtil.CovertToAPI( changeOffset ) ) ;

          if ( isCangeInteresectElement ) {
            // 大引の基点を取得
            var beforeMaterialPosition = beforeMaterial.Position ;
            var beforeSizeFlangeH = beforeMaterial.SteelSize.FrangeThick ;
            var afterSizeFlangeH = afterMaterial.SteelSize.FrangeThick ;

            // 水平ブレースの位置変更処理
            var braceFiList = GetFamilyInscanceWith( koudaiName, FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_1,
              FrmEditSizeList.CONST_CHUUBUNRUI_KETAZAI, FrmEditSizeList.CONST_SHOUBUNRUI_BRACE,
              FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_2 ) ;
            foreach ( var braceFi in braceFiList ) {
              var _material = MaterialSuper.ReadFromElement( braceFi.Id, _doc ) ;
              // 水平ブレース以外はスキップ
              if ( _material.GetType() != typeof( HorizontalBrace ) ) continue ;

              // フランジの両端点を取得
              var endPoints = GantryUtil.GetCurvePoints( _doc, braceFi.Id ) ;
              if ( endPoints.Count == 2 ) {
                var nearEndPoint =
                  ( beforeMaterialPosition.DistanceTo( endPoints[ 0 ] ) >
                    beforeMaterialPosition.DistanceTo( endPoints[ 1 ] ) )
                    ? endPoints[ 1 ]
                    : endPoints[ 0 ] ;
                var diffFlangeSize = Math.Abs( beforeSizeFlangeH - afterSizeFlangeH ) ;
                var _moveValue = diffFlangeSize * -1 ;

                if ( beforeMaterialPosition.Z > nearEndPoint.Z ) {
                  var tr = braceFi.GetTransform() ;
                  var xyz = tr.BasisZ ;
                  if ( ClsGeo.GEO_LT0( xyz.Z ) ) {
                    // 下フランジの上側
                    _moveValue = ( moveValue * 2 ) + ( diffFlangeSize * -1 ) ;
                  }
                  else {
                    // 下フランジの下側
                    _moveValue = moveValue * -2 ;
                  }
                }

                // 上フランジの下側
                SetHostOffsetByMoveValue( braceFi, beforeSizeH, afterSizeH, _moveValue, false ) ;
              }
            }

            // 継ぎ足し杭の位置変更
            var tsugitashikuiFiList = GetFamilyInscanceWith( koudaiName, FrmEditSizeList.CONST_DAIBUNRUI_KOUDAIKUI,
              FrmEditSizeList.CONST_CHUUBUNRUI_TSUGITASHIKUI, FrmEditSizeList.CONST_SHOUBUNRUI_TSUGITASHIKUI ) ;
            foreach ( var tsugitashikuiFi in tsugitashikuiFiList ) {
              SetHostOffsetOfFukkouTopByMoveValue( tsugitashikuiFi, beforeSizeH, afterSizeH, moveValue, false ) ;
            }
          }

          // スチフナージャッキの位置変更処理
          var suchifunaJackFiList = GetFamilyInscanceWith( koudaiName, FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_1,
            FrmEditSizeList.CONST_CHUUBUNRUI_SONOTA, FrmEditSizeList.CONST_SHOUBUNRUI_SUCHIFUNAJACK,
            FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_2 ) ;
          foreach ( var suchifunaJackFi in suchifunaJackFiList ) {
            var hostElement = suchifunaJackFi.Host ;
            var targetHostFi = GantryUtil.GetFamilyInstance( _doc, hostElement.Id ) ;
            var targetHostMaterial = MaterialSuper.ReadFromElement( targetHostFi.Id, _doc ) ;
            var ohbikiHeight = afterMaterial.SteelSize.Height ;
            var ohbikiFrangeThick = afterMaterial.SteelSize.FrangeThick ;

            if ( targetHostMaterial.GetType() == typeof( Ohbiki ) ) {
              var _offset = ( ohbikiHeight - ohbikiFrangeThick ) * -1 ;

              ClsRevitUtil.SetParameter( _doc, suchifunaJackFi.Id, DefineUtil.PARAM_HOST_OFFSET,
                ClsRevitUtil.CovertToAPI( _offset ) ) ;
            }
          }
        }
        else if ( eBuzaiCategory == DefineUtil.eBuzaiCategory.NedaShugeta ) {
          // サイズ変更対象の部材カテゴリが「根太(主桁)」の場合

          var beforeMaterial = Neda.ReadFromElement( beforeFamilyInstance.Id, _doc ) ;
          var beforeSizeH = beforeMaterial.MaterialSize().Height ;
          var afterMaterial = Neda.ReadFromElement( afterFamilyInstance.Id, _doc ) ;
          var afterSizeH = afterMaterial.MaterialSize().Height ;

          var moveValue = Math.Abs( ( beforeSizeH - afterSizeH ) / 2 ) ;

          var offset =
            ClsRevitUtil.CovertFromAPI(
              ClsRevitUtil.GetParameterDouble( _doc, afterFamilyInstance.Id, DefineUtil.PARAM_HOST_OFFSET ) ) ;
          var changeOffset = offset - moveValue ;

          if ( beforeSizeH > afterSizeH ) {
            // 変更前サイズ(H)の方が大きい場合（サイズが小さくなる）
            changeOffset = offset + moveValue ;
          }

          ClsRevitUtil.SetParameter( _doc, afterFamilyInstance.Id, DefineUtil.PARAM_HOST_OFFSET,
            ClsRevitUtil.CovertToAPI( changeOffset ) ) ;

          if ( isCangeInteresectElement ) {
            // 大引の位置変更
            var ohbikiFiList = GetFamilyInscanceWith( koudaiName, FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_1,
              FrmEditSizeList.CONST_CHUUBUNRUI_KETAZAI, FrmEditSizeList.CONST_SHOUBUNRUI_OHBIKI,
              FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_2 ) ;
            foreach ( var ohbikiFi in ohbikiFiList ) {
              SetHostOffsetOfFukkouTopByMoveValue( ohbikiFi, beforeSizeH, afterSizeH, moveValue ) ;
            }

            // 桁受の位置変更
            var ketaukeFiList = GetFamilyInscanceWith( koudaiName, FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_1,
              FrmEditSizeList.CONST_CHUUBUNRUI_KETAZAI, FrmEditSizeList.CONST_SHOUBUNRUI_KETAUKE,
              FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_2 ) ;
            foreach ( var ketaukeFi in ketaukeFiList ) {
              SetHostOffsetOfFukkouTopByMoveValue( ketaukeFi, beforeSizeH, afterSizeH, moveValue ) ;
            }

            // 継ぎ足し杭の位置変更
            var tsugitashikuiFiList = GetFamilyInscanceWith( koudaiName, FrmEditSizeList.CONST_DAIBUNRUI_KOUDAIKUI,
              FrmEditSizeList.CONST_CHUUBUNRUI_TSUGITASHIKUI, FrmEditSizeList.CONST_SHOUBUNRUI_TSUGITASHIKUI ) ;
            foreach ( var tsugitashikuiFi in tsugitashikuiFiList ) {
              SetHostOffsetOfFukkouTopByMoveValue( tsugitashikuiFi, beforeSizeH, afterSizeH, moveValue, false ) ;
            }

            // 対傾構の位置変更
            var taikeikouFiList = GetFamilyInscanceWith( koudaiName, FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_1,
              FrmEditSizeList.CONST_CHUUBUNRUI_KETAZAI, FrmEditSizeList.CONST_SHOUBUNRUI_TAIKEIKOU,
              FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_2 ) ;
            foreach ( var taikeikouFi in taikeikouFiList ) {
              SetHostOffsetOfFukkouTopByMoveValue( taikeikouFi, beforeSizeH, afterSizeH, moveValue, false ) ;
            }

            // 水平ブレースの位置変更
            var braceFiList = GetFamilyInscanceWith( koudaiName, FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_1,
              FrmEditSizeList.CONST_CHUUBUNRUI_KETAZAI, FrmEditSizeList.CONST_SHOUBUNRUI_BRACE,
              FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_2 ) ;
            foreach ( var braceFi in braceFiList ) {
              var _material = MaterialSuper.ReadFromElement( braceFi.Id, _doc ) ;
              // 水平ブレース以外はスキップ
              if ( _material.GetType() != typeof( HorizontalBrace ) ) continue ;

              var _offset =
                ClsRevitUtil.CovertFromAPI(
                  ClsRevitUtil.GetParameterDouble( _doc, braceFi.Id, DefineUtil.PARAM_HOST_OFFSET ) ) ;
              SetHostOffsetByMoveValue( braceFi, beforeSizeH, afterSizeH,
                Math.Sign( _offset ) < 0 ? moveValue * -1 : moveValue ) ;
            }
          }

          // スチフナージャッキの位置変更処理
          var suchifunaJackFiList = GetFamilyInscanceWith( koudaiName, FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_1,
            FrmEditSizeList.CONST_CHUUBUNRUI_SONOTA, FrmEditSizeList.CONST_SHOUBUNRUI_SUCHIFUNAJACK,
            FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_2 ) ;
          foreach ( var suchifunaJackFi in suchifunaJackFiList ) {
            var hostElement = suchifunaJackFi.Host ;
            var targetHostFi = GantryUtil.GetFamilyInstance( _doc, hostElement.Id ) ;
            var targetHostMaterial = MaterialSuper.ReadFromElement( targetHostFi.Id, _doc ) ;
            var nedaHeight = afterMaterial.SteelSize.Height ;
            var nedaFrangeThick = afterMaterial.SteelSize.FrangeThick ;

            if ( targetHostMaterial.GetType() == typeof( Neda ) ) {
              var _offset = ( nedaHeight - nedaFrangeThick ) * -1 ;

              ClsRevitUtil.SetParameter( _doc, suchifunaJackFi.Id, DefineUtil.PARAM_HOST_OFFSET,
                ClsRevitUtil.CovertToAPI( _offset ) ) ;
            }
          }
        }
        else if ( eBuzaiCategory == DefineUtil.eBuzaiCategory.JifukuZuredome ) {
          // サイズ変更対象の部材カテゴリが「覆工板ズレ止め(地覆)」の場合

          var beforeMaterial = JifukuZuredomezai.ReadFromElement( beforeFamilyInstance.Id, _doc ) ;
          var beforeSizeH = beforeMaterial.MaterialSize().Height ;
          var afterMaterial = JifukuZuredomezai.ReadFromElement( afterFamilyInstance.Id, _doc ) ;
          var afterSizeH = afterMaterial.MaterialSize().Height ;

          var moveValue = Math.Abs( ( beforeSizeH - afterSizeH ) / 2 ) ;

          var offset =
            ClsRevitUtil.CovertFromAPI(
              ClsRevitUtil.GetParameterDouble( _doc, afterFamilyInstance.Id, DefineUtil.PARAM_HOST_OFFSET ) ) ;
          var changeOffset = offset + moveValue ;

          if ( beforeSizeH > afterSizeH ) {
            // 変更前サイズ(H)の方が大きい場合（サイズが小さくなる）
            changeOffset = offset - moveValue ;
          }

          ClsRevitUtil.SetParameter( _doc, afterFamilyInstance.Id, DefineUtil.PARAM_HOST_OFFSET,
            ClsRevitUtil.CovertToAPI( changeOffset ) ) ;

          if ( isCangeInteresectElement ) {
            // 手摺の位置変更 ※ ホストに地覆・覆工板ズレ止め材を保持しているため処理不要

            // 手摺支柱の位置変更処理
            var tesuriShichuFiList = GetFamilyInscanceWith( koudaiName, FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_1,
              FrmEditSizeList.CONST_CHUUBUNRUI_SONOTA, FrmEditSizeList.CONST_SHOUBUNRUI_TESURISHICHU,
              FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_2 ) ;
            foreach ( var tesuriShichuFi in tesuriShichuFiList ) {
              SetHostOffsetByMoveValue( tesuriShichuFi, beforeSizeH, afterSizeH, moveValue, false ) ;
            }
          }
        }
        else if ( eBuzaiCategory == DefineUtil.eBuzaiCategory.Shikigeta ) {
          var beforeMaterial = Shikigeta.ReadFromElement( beforeFamilyInstance.Id, _doc ) ;
          var beforeSizeH = beforeMaterial.MaterialSize().Height ;
          var afterMaterial = Shikigeta.ReadFromElement( afterFamilyInstance.Id, _doc ) ;
          var afterSizeH = afterMaterial.MaterialSize().Height ;

          //var moveValue = Math.Abs((beforeSizeH - afterSizeH) / 2);

          //var offset = ClsRevitUtil.CovertFromAPI(ClsRevitUtil.GetParameterDouble(_doc, afterFamilyInstance.Id, DefineUtil.PARAM_HOST_OFFSET));
          //var changeOffset = offset - moveValue;

          //if (beforeSizeH > afterSizeH)
          //{
          //    // 変更前サイズ(H)の方が大きい場合（サイズが小さくなる）
          //    changeOffset = offset + moveValue;
          //}

          //ClsRevitUtil.SetParameter(_doc, afterFamilyInstance.Id, DefineUtil.PARAM_HOST_OFFSET, ClsRevitUtil.CovertToAPI(changeOffset));

          // スチフナージャッキの位置変更処理
          var suchifunaJackFiList = GetFamilyInscanceWith( koudaiName, FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_1,
            FrmEditSizeList.CONST_CHUUBUNRUI_SONOTA, FrmEditSizeList.CONST_SHOUBUNRUI_SUCHIFUNAJACK,
            FrmEditSizeList.CONST_DAIBUNRUI_KOUDAI_2 ) ;
          foreach ( var suchifunaJackFi in suchifunaJackFiList ) {
            var hostElement = suchifunaJackFi.Host ;
            var targetHostFi = GantryUtil.GetFamilyInstance( _doc, hostElement.Id ) ;
            var targetHostMaterial = MaterialSuper.ReadFromElement( targetHostFi.Id, _doc ) ;
            var shikigetaHeight = afterMaterial.SteelSize.Height ;
            var shikigetaFrangeThick = afterMaterial.SteelSize.FrangeThick ;

            if ( targetHostMaterial.GetType() == typeof( Shikigeta ) ) {
              var _offset = ( shikigetaHeight - shikigetaFrangeThick ) * -1 ;

              ClsRevitUtil.SetParameter( _doc, suchifunaJackFi.Id, DefineUtil.PARAM_HOST_OFFSET,
                ClsRevitUtil.CovertToAPI( _offset ) ) ;
            }
          }
        }
      }
    }

    private void SetHostOffsetByMoveValue( FamilyInstance fi, double beforeSizeH, double afterSizeH, double moveValue,
      bool isAddTargetElementH = true )
    {
      var _offset =
        ClsRevitUtil.CovertFromAPI( ClsRevitUtil.GetParameterDouble( _doc, fi.Id, DefineUtil.PARAM_HOST_OFFSET ) ) ;
      var _changeOffset = _offset + ( moveValue * ( isAddTargetElementH ? 2 : 1 ) ) ;

      if ( beforeSizeH > afterSizeH ) {
        _changeOffset = _offset - ( moveValue * ( isAddTargetElementH ? 2 : 1 ) ) ;
      }

      ClsRevitUtil.SetParameter( _doc, fi.Id, DefineUtil.PARAM_HOST_OFFSET,
        ClsRevitUtil.CovertToAPI( _changeOffset ) ) ;
    }

    private void SetHostOffsetOfFukkouTopByMoveValue( FamilyInstance fi, double beforeSizeH, double afterSizeH,
      double moveValue, bool isAddTargetElementH = true )
    {
      var _offset =
        ClsRevitUtil.CovertFromAPI( ClsRevitUtil.GetParameterDouble( _doc, fi.Id, DefineUtil.PARAM_HOST_OFFSET ) ) ;
      var _changeOffset = _offset - ( moveValue * ( isAddTargetElementH ? 2 : 1 ) ) ;

      if ( beforeSizeH > afterSizeH ) {
        _changeOffset = _offset + ( moveValue * ( isAddTargetElementH ? 2 : 1 ) ) ;
      }

      ClsRevitUtil.SetParameter( _doc, fi.Id, DefineUtil.PARAM_HOST_OFFSET,
        ClsRevitUtil.CovertToAPI( _changeOffset ) ) ;
    }

    /// <summary>
    /// 引数に指定された条件に該当するファミリインスタンスを取得
    /// </summary>
    /// <param name="koudaiName"></param>
    /// <param name="daiBunrui"></param>
    /// <param name="chuuBunrui"></param>
    /// <param name="shouBunrui"></param>
    /// <returns></returns>
    private List<FamilyInstance> GetFamilyInscanceWith( string koudaiName, string daiBunrui_1, string chuuBunrui,
      string shouBunrui, string daiBunrui_2 = "" )
    {
      var resultList = new List<FamilyInstance>() ;

      // 図面内に配置されたファミリシンボルを取得
      var familySymbolList = GantryUtil.GetAllFamilySymbol( _doc ) ;
      foreach ( var fs in familySymbolList ) {
        var _daiBunrui = GantryUtil.GetTypeParameter( fs, DefineUtil.PARAM_DAI_BUNRUI ) ;
        var _chuuBunrui = GantryUtil.GetTypeParameter( fs, DefineUtil.PARAM_CHUU_BUNRUI ) ;
        var _shouBunrui = GantryUtil.GetTypeParameter( fs, DefineUtil.PARAM_SHOU_BUNRUI ) ;

        if ( string.IsNullOrEmpty( daiBunrui_2 ) ) {
          if ( ! ( daiBunrui_1 == _daiBunrui && _chuuBunrui == chuuBunrui && _shouBunrui == shouBunrui ) ) continue ;
        }
        else {
          if ( ! ( ( daiBunrui_1 == _daiBunrui || daiBunrui_2 == _daiBunrui ) && _chuuBunrui == chuuBunrui &&
                   _shouBunrui == shouBunrui ) ) continue ;
        }

        var fiList = GantryUtil.GetFamilyInstanceList( _doc, fs ) ;

        foreach ( var fi in fiList ) {
          var material = MaterialSuper.ReadFromElement( fi.Id, _doc ) ;
          if ( material == null ) continue ;

          if ( material.m_KodaiName == koudaiName ) {
            resultList.Add( fi ) ;
          }
        }
      }

      return resultList ;
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
      public FailureProcessingResult PreprocessFailures( FailuresAccessor failuresAccessor )
      {
        IList<FailureMessageAccessor> failList = new List<FailureMessageAccessor>() ;
        failList = failuresAccessor.GetFailureMessages() ;
        foreach ( FailureMessageAccessor failure in failList ) {
          FailureDefinitionId failID = failure.GetFailureDefinitionId() ;
          failuresAccessor.DeleteWarning( failure ) ;
        }

        return FailureProcessingResult.Continue ;
      }
    }
  }
}