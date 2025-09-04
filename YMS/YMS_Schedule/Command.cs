//#define CHECK_LATER //仮鋼材チェックを後回しにする

#region Namespaces

using Autodesk.Revit.ApplicationServices ;
using Autodesk.Revit.Attributes ;
using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using Autodesk.Revit.UI.Selection ;
using Autodesk.Revit.DB.Structure ;
using Autodesk.Revit.DB.Events ;
using System ;
using System.Collections.Generic ;
using System.Diagnostics ;
using System.IO ;
using System.Linq ;

#endregion

using Microsoft.VisualBasic.FileIO ;
using System.Net ;
using System.Reflection ;
using System.Windows.Forms ;
using static System.Net.Mime.MediaTypeNames ;
using RevitUtil ;

namespace YMS_Schedule
{
  [Transaction( TransactionMode.Manual )]
  public class CommandExport : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      UIApplication uiapp = commandData.Application ;
      UIDocument uidoc = uiapp.ActiveUIDocument ;
      Autodesk.Revit.ApplicationServices.Application app = uiapp.Application ;
      Document doc = uidoc.Document ;
      UIControlledApplication uicapp = App.uicapp ;

      //一度も保存されていなかったら実行しない
      if ( doc.PathName == "" ) {
        MessageBox.Show( "プロジェクトファイルとして保存してから実行してください" ) ;
        return Result.Cancelled ;
      }


      ////selection?
      //ICollection<ElementId> selIds = uidoc.Selection.GetElementIds();

      //if (BoltEstimation.IsMixedWaritukeShuzaiAndKarikouzai(doc,selIds.ToList()))
      //{
      //    MessageBox.Show("仮鋼材と割付主材が混在しています。");
      //    return Result.Failed ;
      //}


      //ダミーファイル削除
      bool removeDummy = false ;
      if ( removeDummy ) {
        List<ElementId> AllElements = RevitUtil.ClsRevitUtil.GetAllCreatedFamilyInstanceList( doc ) ;
        List<ElementId> delElements = new List<ElementId>() ;
        string iniDummyy = MiscWin.GetIniValue( Env.iniPath(), Env.DummyFamilyPath, Env.keyFilepath ) ;
        string name = System.IO.Path.GetFileNameWithoutExtension( iniDummyy ) ;
        foreach ( ElementId tmpId in AllElements ) {
          FamilyInstance inst = doc.GetElement( tmpId ) as FamilyInstance ;
          if ( inst != null && inst.Symbol.FamilyName.Contains( name ) ) {
            delElements.Add( tmpId ) ;
          }
        }

        if ( delElements.Count > 0 ) {
          using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
            t.Start() ;
            RevitUtil.ClsRevitUtil.Delete( doc, delElements ) ;
            t.Commit() ;
          }
        }
      }

      //Shared Parameter
      doc.Application.SharedParametersFilename = Env.sharedParameterFile() ;
      DefinitionFile sharedFile = app.OpenSharedParameterFile() ;

      if ( sharedFile == null ) {
        MessageBox.Show( "共有パラメータファイルの取得に失敗しました" ) ;
        return Result.Failed ;
      }

      FilteredElementCollector sharedParam = new FilteredElementCollector( doc ).WhereElementIsNotElementType()
        .OfClass( typeof( SharedParameterElement ) ) ;

      List<ElementId> eIds = new List<ElementId>() ;
      foreach ( Element e in sharedParam ) {
        ElementId eId = null ;

        SharedParameterElement param = e as SharedParameterElement ;
        Definition paramDef = param.GetDefinition() ;

        foreach ( DefinitionGroup group in sharedFile.Groups ) {
          foreach ( ExternalDefinition def in group.Definitions ) {
            if ( paramDef.Name == def.Name ) {
              eId = param.Id ;
              break ;
            }
          }

          if ( eId != null ) {
            break ;
          }
        }

        if ( eId != null ) {
          eIds.Add( eId ) ;
        }
      }

      List<ElementId> builtinIds = new List<ElementId>() ;

      //Add parameter
      List<string> paramLst = Env.sharedParameters() ;
      foreach ( var x in paramLst ) {
        if ( x == Env.paramLevel ) {
          builtinIds.Add( new ElementId( BuiltInParameter.SCHEDULE_LEVEL_PARAM ) ) ;
        }
        else if ( x == Env.paramPhaseCreated ) {
          builtinIds.Add( new ElementId( BuiltInParameter.PHASE_CREATED ) ) ;
        }
        else if ( x == Env.paramPhaseDemolished ) {
          builtinIds.Add( new ElementId( BuiltInParameter.PHASE_DEMOLISHED ) ) ;
        }
        else if ( x == Env.paramFamily ) {
          builtinIds.Add( new ElementId( BuiltInParameter.ELEM_FAMILY_PARAM ) ) ;
        }
        else if ( x == Env.paramFamilyAndType ) {
          builtinIds.Add( new ElementId( BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM ) ) ;
        }
        else if ( x == Env.paramVolume ) {
          builtinIds.Add( new ElementId( BuiltInParameter.HOST_VOLUME_COMPUTED ) ) ;
        }
      }

      //Create Schedule
      string schduleName = Env.sheduleName() ;

      ViewSchedule vs = null ;
      using ( Transaction t = new Transaction( doc, "Create single-category" ) ) {
        t.Start() ;

        //remove previous
        vs = Schedule.view( doc ) ;
        if ( vs != null ) {
          if ( doc.ActiveView.Id == vs.Id ) {
            MessageBox.Show( "他のビューに切り替えてから実行して下さい" ) ;
            return Result.Cancelled ;
          }

          doc.Delete( vs.Id ) ;
        }

        //create new
        vs = ViewSchedule.CreateSchedule( doc, new ElementId( BuiltInCategory.OST_GenericModel ) ) ;
        vs.Name = schduleName ;

        doc.Regenerate() ;

        ScheduleDefinition definition = vs.Definition ;

        foreach ( var eId in eIds ) {
          definition.AddField( ScheduleFieldType.Instance, eId ) ;
        }

        foreach ( var eId in builtinIds ) {
          AddFieldToSchedule( vs, eId ) ;
        }

        t.Commit() ;
      }


      //selection?
      ICollection<ElementId> selIds = uidoc.Selection.GetElementIds() ;

      #if !CHECK_LATER
      if ( RuleEstimation.IsMixedWaritukeShuzaiAndKarikouzai( doc, selIds.ToList() ) ) {
        MessageBox.Show( "仮鋼材と割付主材が混在しています。" ) ;
        return Result.Failed ;
      }
      #endif

      //積算ダイアログオープン
      FormSekisanSetting sekisandlg = new FormSekisanSetting() ;
      DialogResult result = sekisandlg.ShowDialog() ;

      if ( result != DialogResult.OK ) {
        return Result.Cancelled ;
      }

      ////ボルト集計をここで走らせる
      RuleEstimation ruleEstimation = new RuleEstimation() ;
      ruleEstimation.BoltEstimationMain( uidoc, selIds.ToList(), sekisandlg.m_ClsSekisaiSetting ) ;

      //この中で部材収集をしてるっぽい　→　ここ以前にはダミーファイルが必要
      Schedule sch = new Schedule( vs ) ;

      //Filter Element
      if ( selIds.Count >= 1 ) {
        Dictionary<ElementId, Item> dict = new Dictionary<ElementId, Item>() ;
        foreach ( var x in sch.itms ) {
          dict.Add( x.elmId, x ) ;
        }

        List<Item> itms = new List<Item>() ;
        foreach ( var x in selIds ) {
          if ( dict.ContainsKey( x ) ) {
            itms.Add( dict[ x ] ) ;
          }
        }

        List<ElementId> ids = new List<ElementId>() ;
        foreach ( var x in itms ) {
          ids.Add( x.elmId ) ;
        }

        foreach ( var x in ids ) {
          FamilyInstance fi = doc.GetElement( x ) as FamilyInstance ;
          if ( fi == null ) {
            continue ;
          }

          nestCollect( fi, ref dict, ref itms ) ;
        }

        //ここで全部材を選択部材のみに置き換えてる（ぽい）
        sch.itms = itms ;
      }

      //ネストしたファミリのレベルがRevit集計表で設定されていない（ようだ）
      //Revit仕様なのかファミリ定義の問題かは？
      nestFillLevel( doc, sch ) ;

      //ネストしたファミリのレベル調整用に保持していたYMS対象外の分を除去
      Env.ScheduleYMSFilter filter = Env.sheduleFilter( false ) ;
      int indexItem1 = sch.h.FindIndex( x => x == filter[ Env.keyFilter1 ] ) ;
      int indexItem2 = sch.h.FindIndex( x => x == filter[ Env.keyFilter2 ] ) ;
      int indexItem3 = sch.h.FindIndex( x => x == filter[ Env.keyFilter3 ] ) ;
      foreach ( var x in sch.itms.ToList() ) {
        if ( ! Schedule.isYMS( x.items[ indexItem1 ], x.items[ indexItem2 ], x.items[ indexItem3 ] ) ) {
          sch.itms.Remove( x ) ;
        }
      }

      FormScheduleFilter dlg = new FormScheduleFilter( sch ) ;
      DialogResult res2 = dlg.ShowDialog() ;
      dlg.Dispose() ;
      if ( DialogResult.OK != res2 ) {
        return Result.Cancelled ;
      }

      //select by dialog
      string idName = Env.sharedParameterId() ;
      int idIndex = sch.h.FindIndex( v => v == idName ) ;

      sch.itms.Clear() ;
      foreach ( var x in dlg.selectedTbl ) {
        int id ;
        if ( int.TryParse( x[ idIndex ], out id ) ) {
          sch.itms.Add( new Item( new ElementId( id ), x ) ) ;
        }
      }

      //Check Level
      bool bLog = false ;
      bool bDialog = false ;
      Env.levelCheck( ref bLog, ref bDialog ) ;
      if ( bLog ) {
        int indexLevel = sch.h.FindIndex( v => v == "レベル" ) ;
        List<Item> lstLevelEmpty = sch.itms.FindAll( v => string.IsNullOrEmpty( v.items[ indexLevel ] ) ) ;
        if ( lstLevelEmpty.Count >= 1 ) {
          List<string> pList = Env.levelCheckParam() ;

          List<int> idxList = new List<int>() ;
          foreach ( var x in pList ) {
            idxList.Add( sch.h.FindIndex( v => v == x ) ) ;
          }

          Dictionary<string, string> dictLevelEmpty = new Dictionary<string, string>() ;
          foreach ( var x in lstLevelEmpty ) {
            string sKey = "" ;
            foreach ( var y in idxList ) {
              sKey += ( x.items[ y ] + "," ) ;
            }

            if ( dictLevelEmpty.ContainsKey( sKey ) ) {
              dictLevelEmpty[ sKey ] += ( ";" + x.elmId.ToString() ) ;
            }
            else {
              dictLevelEmpty.Add( sKey, x.elmId.ToString() ) ;
            }
          }

          string sMsg = "" ;
          foreach ( var x in dictLevelEmpty.Keys ) {
            sMsg += ( x + dictLevelEmpty[ x ] + Environment.NewLine ) ;
          }

          if ( sMsg.Length >= 1 ) {
            string sHeader = "" ;
            foreach ( var x in pList ) {
              sHeader += ( x + "," ) ;
            }

            sHeader += ( "ID " + Environment.NewLine ) ;

            string fName = Path.GetFileNameWithoutExtension( doc.PathName ) ;
            string dir = Path.GetDirectoryName( doc.PathName ) ;
            string logPath = Path.Combine( dir, fName + "_レベル指定無.csv" ) ;
            File.WriteAllText( logPath, sHeader + sMsg, Env.encoding ) ;

            if ( bDialog ) {
              Process.Start( logPath ) ;
            }
          }
        }
      }

      if ( bLog ) {
        string sMsg = "" ;
        foreach ( var x in sch.rm.Keys ) {
          sMsg += ( x + "," + sch.rm[ x ] + Environment.NewLine ) ;
        }

        if ( sMsg.Length >= 1 ) {
          string sHeader = "Family&Type, count" + Environment.NewLine ;

          string fName = Path.GetFileNameWithoutExtension( doc.PathName ) ;
          string dir = Path.GetDirectoryName( doc.PathName ) ;
          string logPath = Path.Combine( dir, fName + "_大中小分類無.csv" ) ;
          File.WriteAllText( logPath, sHeader + sMsg, Env.encoding ) ;

          if ( bDialog ) {
            Process.Start( logPath ) ;
          }
        }
      }

      #if CHECK_LATER
            List<ElementId> lst = (from itm in sch.itms select itm.elmId).ToList();
            if (RuleEstimation.IsMixedWaritukeShuzaiAndKarikouzai(doc, lst))
            {
                MessageBox.Show("仮鋼材と割付主材が混在しています。");
                return Result.Failed;
            }
      #endif

      //export
      string rvtPath = doc.PathName ;
      SaveFileDialog sfd = new SaveFileDialog() ;
      sfd.FileName = Path.GetFileNameWithoutExtension( rvtPath ) ;
      sfd.InitialDirectory = Path.GetDirectoryName( rvtPath ) ;
      sfd.Filter = "CSVファイル(*.csv)|*.csv|すべてのファイル(*.*)|*.*" ;
      sfd.Title = "保存先のファイルを選択してください" ;
      sfd.RestoreDirectory = true ;
      if ( DialogResult.OK != sfd.ShowDialog() ) {
        return Result.Cancelled ;
      }

      string csvPath = sfd.FileName ;

      prjInfo pInfo = new prjInfo( doc ) ;
      string contents = "" ;
      contents += ( Env.separatorProjectInfo() + Environment.NewLine ) ;
      contents += ( pInfo.toCSVString() ) ;
      contents += ( Env.separatorSchedule() + Environment.NewLine ) ;
      contents += ( sch.toCSVString() ) ;
      //山留ボルトデータ
      contents += "yamadomeBolt" + Environment.NewLine ;
      contents += ruleEstimation.GetYamadomeBoltData() ;
      contents += "yamadomeBoltWithElementId" + Environment.NewLine ;
      contents += ruleEstimation.GetYamadomeBoltDataWithElementID() ;
      //構台ボルトデータ
      contents += "koudaiBolt" + Environment.NewLine ;
      contents += ruleEstimation.GetKoudaiBoltData() ;
      contents += "koudaiBoltWithElementId" + Environment.NewLine ;
      contents += ruleEstimation.GetKoudaiBoltDataWithElementID() ;
      //構台ボルトに付随するスプリングワッシャデータ
      contents += "washerWithBN" + Environment.NewLine ;
      contents += ruleEstimation.GetWasherWithBNData() ;
      //裏込め裏込めデータ
      contents += "uragome" + Environment.NewLine ;
      contents += ruleEstimation.GetUragomeData() ;
      contents += "uragomeForCP" + Environment.NewLine ;
      contents += ruleEstimation.GetUragomeDataForCornerPiece() ;
      //穴明ライナーデータ
      contents += "holeliner" + Environment.NewLine ;
      contents += ruleEstimation.GetHolelinerData() ;
      //仮鋼材→主材データ
      contents += "karikouzaiToShuzai" + Environment.NewLine ;
      contents += ruleEstimation.GetKarikouzaiToShuzaiData() ;
      //構台側 仮鋼材→主材データ
      contents += "koudaikarikouzaiToShuzai" + Environment.NewLine ;
      contents += ruleEstimation.GetkoudaiKarikouzaiToShuzaiData() ;
      //仮鋼材→主材データからルール拾いしたカバープレートのリスト
      contents += "CoverPlate" + Environment.NewLine ;
      contents += ruleEstimation.GetCoverPlateData() ;
      //座金データ
      contents += "Zagane" + Environment.NewLine ;
      contents += ruleEstimation.GetZaganeData() ;
      //ブラケットデータ
      contents += "Bracket" + Environment.NewLine ;
      contents += ruleEstimation.GetBracketData() ;
      //ジャッキデータ
      contents += "JackKouzai" + Environment.NewLine ;
      contents += ruleEstimation.GetJackData() ; //GetSyabariChainData
      //斜梁チェーンデータデータ
      contents += "SyabariChain" + Environment.NewLine ;
      contents += ruleEstimation.GetSyabariChainData() ;
      //加工品(山留)データ
      contents += "加工品(山留)" + Environment.NewLine ;
      contents += ruleEstimation.GetKaouhinYamadomeData() ;
      //加工品(山留)データ
      contents += "加工品(構台)" + Environment.NewLine ;
      contents += ruleEstimation.GetKaouhinKoudaiData() ;
      //山留壁・杭シートデータ
      contents += "kabeAndkuiSheet" + Environment.NewLine ;
      contents += ruleEstimation.GetKabekuiSheetData( doc ) ;
      //構台用 支持杭、継足し杭、支柱データ
      contents += "koudaikabeAndkuiSheet" + Environment.NewLine ;
      contents += ruleEstimation.GetKoudaiKabekuiSheetData( doc ) ;
      //レベル名称の列挙
      contents += "AllLevelNames" + Environment.NewLine ;
      contents += ruleEstimation.GetAllLevelName( doc ) ;
      ////レベル名が抜けている部材のIDとレベル名の列挙
      //contents += "EmptyLevelNameBuzai" + Environment.NewLine;
      //contents += ruleEstimation.WriteLevelNameFamily(doc);
      //EOD
      contents += ( Env.separatorEnd() + Environment.NewLine ) ;

      File.WriteAllText( csvPath, contents, Env.encoding ) ;
      #if DEBUG
      Process.Start( "notepad.exe", "\"" + csvPath + "\"" ) ;
      #endif
      return Result.Succeeded ;
    }

    public static void AddFieldToSchedule( ViewSchedule schedule, ElementId paramId )
    {
      ScheduleDefinition definition = schedule.Definition ;

      SchedulableField schedulableField = definition.GetSchedulableFields()
        .FirstOrDefault<SchedulableField>( sf => sf.ParameterId == paramId ) ;
      if ( schedulableField != null ) {
        definition.AddField( schedulableField ) ;
      }
    }

    public DefinitionFile defFile( Autodesk.Revit.ApplicationServices.Application app )
    {
      return app.OpenSharedParameterFile() ;
    }

    public List<ExternalDefinition> defExternal( Autodesk.Revit.ApplicationServices.Application app )
    {
      List<ExternalDefinition> res = new List<ExternalDefinition>() ;

      DefinitionFile sharedFile = defFile( app ) ;

      List<string> paramLst = Env.sharedParameters() ;
      foreach ( var x in paramLst ) {
        bool found = false ;
        foreach ( DefinitionGroup group in sharedFile.Groups ) {
          foreach ( ExternalDefinition def in group.Definitions ) {
            if ( def.Name == x ) {
              res.Add( def ) ;
              found = true ;
              break ;
            }
          }

          if ( found ) {
            break ;
          }
        }
      }

      return res ;
    }

    private void nestCollect( FamilyInstance fi, ref Dictionary<ElementId, Item> dict, ref List<Item> itms )
    {
      if ( fi == null ) {
        return ;
      }

      List<FamilyInstance> nest = Misc.SubComponents( fi ) ;
      if ( nest == null ) {
        return ;
      }

      if ( nest.Count <= 0 ) {
        return ;
      }

      foreach ( var x in nest ) {
        if ( dict.ContainsKey( x.Id ) ) {
          itms.Add( dict[ x.Id ] ) ;
        }

        nestCollect( x, ref dict, ref itms ) ;
      }
    }

    private void nestFillLevel( Document doc, Schedule sch )
    {
      Dictionary<ElementId, string> lvlDict = Misc.levelName( doc ) ;
      int indexLvl = sch.h.FindIndex( x => x == Env.paramLevel ) ;
      int indexDan = sch.h.FindIndex( x => x == Env.sharedParameterDan() ) ;

      foreach ( var x in sch.itms ) {
        ElementId lvlId = ElementId.InvalidElementId ;
        string lvlName = "" ;

        ElementId elmId = x.elmId ;
        if ( elmId == ElementId.InvalidElementId ) {
          continue ;
        }

        FamilyInstance fi = doc.GetElement( elmId ) as FamilyInstance ;
        if ( fi == null ) {
          continue ;
        }

        if ( fi.LevelId != ElementId.InvalidElementId ) {
          lvlId = fi.LevelId ;
        }
        else {
          if ( fi.Host == null ) {
            Item item = sch.find( fi.Id ) ;
            if ( item == null ) {
              continue ;
            }

            lvlName = item.items[ indexLvl ] ;
          }
          else {
            if ( fi.Host is Level lvl ) {
              if ( ! lvlDict.ContainsKey( lvl.Id ) ) {
                continue ;
              }

              lvlName = lvlDict[ lvl.Id ] ;
            }
            else {
              continue ;
            }
          }
        }

        //set own
        x.items[ indexLvl ] = lvlName ;
        if ( string.IsNullOrEmpty( x.items[ indexLvl ] ) ) {
          lvlName = x.items[ indexDan ] ;
          x.items[ indexLvl ] = lvlName ;
        }

        //set nest
        List<FamilyInstance> nest = Misc.SubComponents( fi ) ;
        if ( nest == null || nest.Count <= 0 ) {
          continue ;
        }

        foreach ( var y in nest ) {
          nestFillLevel( y, lvlName, indexLvl, sch ) ;
        }
      }
    }

    private void nestFillLevel( FamilyInstance fi, string lvlName, int index, Schedule sch )
    {
      if ( fi == null ) {
        return ;
      }

      Item item = sch.find( fi.Id ) ;
      if ( item == null ) {
        return ;
      }

      item.items[ index ] = lvlName ;

      List<FamilyInstance> nest = Misc.SubComponents( fi ) ;
      if ( nest == null ) {
        return ;
      }

      foreach ( var x in nest ) {
        nestFillLevel( x, lvlName, index, sch ) ;
      }
    }
  }

  [Transaction( TransactionMode.Manual )]
  public class CommandImport : IExternalCommand
  {
    string errPath = "" ;

    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      UIApplication uiapp = commandData.Application ;
      UIDocument uidoc = uiapp.ActiveUIDocument ;
      Autodesk.Revit.ApplicationServices.Application app = uiapp.Application ;
      Document doc = uidoc.Document ;
      UIControlledApplication uicapp = App.uicapp ;

      //一度も保存されていなかったら実行しない
      if ( doc.PathName == "" ) {
        MessageBox.Show( "プロジェクトファイルとして保存してから実行してください" ) ;
        return Result.Cancelled ;
      }

      string rvtPath = doc.PathName ;
      OpenFileDialog sfd = new OpenFileDialog() ;
      sfd.FileName = Path.GetFileNameWithoutExtension( rvtPath ) ;
      sfd.InitialDirectory = Path.GetDirectoryName( rvtPath ) ;
      sfd.Filter = "CSVファイル(*.csv)|*.csv|すべてのファイル(*.*)|*.*" ;
      sfd.Title = "ファイルを選択してください" ;
      sfd.RestoreDirectory = true ;
      if ( DialogResult.OK != sfd.ShowDialog() ) {
        return Result.Cancelled ;
      }

      string csvPath = sfd.FileName ;

      TextFieldParser tfp = new TextFieldParser( csvPath, Env.encoding ) ;
      tfp.Delimiters = new string[] { "," } ;
      List<string[]> lists = new List<string[]>() ;
      while ( ! tfp.EndOfData ) {
        lists.Add( tfp.ReadFields() ) ;
      }

      tfp.Close() ;

      //
      DummyFamily df = new DummyFamily( doc ) ;

      //Previous
      IEnumerable<FamilyInstance> prev = Misc.familyInstancesByName( doc, df.symbol.Name ) ;

      using ( Transaction t = new Transaction( doc, "set parameters" ) ) {
        t.Start() ;

        foreach ( var x in prev.ToList() ) {
          doc.Delete( x.Id ) ;
        }

        EventHandler<FailuresProcessingEventArgs> h = null ;
        h = new EventHandler<FailuresProcessingEventArgs>( Misc.AppEvent_FailuresProcessing_Handler ) ;
        uicapp.ControlledApplication.FailuresProcessing += h ;

        FailureHandlingOptions failOpt = t.GetFailureHandlingOptions() ;
        failOpt.SetFailuresPreprocessor( new YMS.WarningSwallower() ) ;
        t.SetFailureHandlingOptions( failOpt ) ;

        try {
          Dictionary<string, Level> lvlDict = Misc.levelLevel( doc ) ;
          XYZ location = null ;
          string lvlParam = "レベル" ;
          foreach ( var x in lists ) {
            Level lvl = null ;

            Dictionary<string, string> parameters = parametersFromString( x ) ;
            if ( parameters.ContainsKey( lvlParam ) ) {
              string lvlName = parameters[ lvlParam ] ;
              if ( lvlDict.ContainsKey( lvlName ) ) {
                lvl = lvlDict[ lvlName ] ;
              }

              parameters.Remove( lvlParam ) ;
            }

            FamilyInstance fi = null ;
            if ( lvl != null ) {
              fi = doc.Create.NewFamilyInstance( location == null ? XYZ.Zero : location, df.symbol, lvl,
                StructuralType.NonStructural ) ;
            }
            else {
              fi = doc.Create.NewFamilyInstance( location == null ? XYZ.Zero : location, df.symbol,
                StructuralType.NonStructural ) ;
            }

            Misc.ParameterSet( doc, fi, parameters ) ;
          }

          t.Commit() ;
        }
        //catch
        //{
        //}
        finally {
          uicapp.ControlledApplication.FailuresProcessing -= h ;
        }
      }

      return Result.Succeeded ;
    }

    public static Dictionary<string, string> parametersFromString( string[] strs )
    {
      Dictionary<string, string> res = new Dictionary<string, string>() ;

      for ( int i = 0 ; i < strs.Length ; i++ ) {
        string[] parts = strs[ i ].Split( '=' ) ;
        if ( parts.Length >= 2 ) {
          if ( ! res.ContainsKey( parts[ 0 ] ) ) {
            res.Add( parts[ 0 ], parts[ 1 ] ) ;
          }
        }
      }

      return res ;
    }

    /// <summary>
    /// パラメータ一覧作成
    /// </summary>
    /// <returns>なし</returns>
    /// <param name="commandData">コマンドデータ</param>
    public Result chkRfas( ExternalCommandData commandData )
    {
      UIApplication uiapp = commandData.Application ;
      UIDocument uidoc = uiapp.ActiveUIDocument ;
      Autodesk.Revit.ApplicationServices.Application app = uiapp.Application ;
      Document doc = uidoc.Document ;

      FolderBrowserDialog fbd = new FolderBrowserDialog() ;
      fbd.Description = "フォルダを指定してください。" ;
      fbd.RootFolder = Environment.SpecialFolder.Desktop ;
      fbd.SelectedPath = @"C:" ;
      fbd.ShowNewFolderButton = false ;
      if ( fbd.ShowDialog() != DialogResult.OK ) {
        return Result.Cancelled ;
      }

      string root = fbd.SelectedPath ;

      string ext = "*.rfa" ;
      string dir = Path.GetFileName( root ) ;
      string listPath = Path.Combine( root, dir + "List.csv" ) ;
      errPath = Path.Combine( root, dir + "Error.txt" ) ;

      if ( File.Exists( listPath ) ) {
        File.Delete( listPath ) ;
      }

      if ( File.Exists( errPath ) ) {
        File.Delete( errPath ) ;
      }

      string[] files = System.IO.Directory.GetFiles( root, ext, System.IO.SearchOption.AllDirectories ) ;

      List<string> keys = new List<string>
      {
        "大分類",
        "中分類",
        "小分類",
        "品種",
        "名称",
        "記号",
        "サイズ",
        "リース/売切",
        "品名コード"
      } ;

      //shared parameter
      doc.Application.SharedParametersFilename = Env.sharedParameterFile() ;
      DefinitionFile sharedFile = app.OpenSharedParameterFile() ;
      if ( sharedFile == null ) {
        MessageBox.Show( "共有パラメータファイルの取得に失敗しました" ) ;
        return Result.Failed ;
      }

      FilteredElementCollector sharedParam = new FilteredElementCollector( doc ).WhereElementIsNotElementType()
        .OfClass( typeof( SharedParameterElement ) ) ;

      List<string> paramNames = new List<string>() ;
      foreach ( Element e in sharedParam ) {
        ElementId eId = null ;

        SharedParameterElement param = e as SharedParameterElement ;
        Definition paramDef = param.GetDefinition() ;
        foreach ( DefinitionGroup group in sharedFile.Groups ) {
          foreach ( ExternalDefinition def in group.Definitions ) {
            if ( paramDef.Name == def.Name ) {
              eId = param.Id ;
              break ;
            }
          }

          if ( eId != null ) {
            break ;
          }
        }

        if ( eId != null ) {
          param = doc.GetElement( eId ) as SharedParameterElement ;
          paramNames.Add( param.Name ) ;
        }
      }

      List<ElementId> builtinIds = new List<ElementId>() ;

      //Add parameter
      List<string> paramLst = Env.sharedParameters() ;
      foreach ( var x in paramLst ) {
        paramNames.Add( x ) ;

        if ( x == Env.paramLevel ) {
          builtinIds.Add( new ElementId( BuiltInParameter.SCHEDULE_LEVEL_PARAM ) ) ;
        }
        else if ( x == Env.paramPhaseCreated ) {
          builtinIds.Add( new ElementId( BuiltInParameter.PHASE_CREATED ) ) ;
        }
        else if ( x == Env.paramPhaseDemolished ) {
          builtinIds.Add( new ElementId( BuiltInParameter.PHASE_DEMOLISHED ) ) ;
        }
        else if ( x == Env.paramFamily ) {
          builtinIds.Add( new ElementId( BuiltInParameter.ELEM_FAMILY_PARAM ) ) ;
        }
        else if ( x == Env.paramFamilyAndType ) {
          builtinIds.Add( new ElementId( BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM ) ) ;
        }
      }

      string header = root + Environment.NewLine ;

      keys = paramNames ;
      foreach ( var x in keys ) {
        header += x + "," ;
      }

      header += Environment.NewLine ;
      File.AppendAllText( listPath, header, Env.encoding ) ;

      foreach ( var x in files ) {
        File.AppendAllText( listPath, ExtractFamilyInfo( app, x, root, keys ), Env.encoding ) ;
      }

      MessageBox.Show( "終了" + Environment.NewLine + listPath ) ;
      return Result.Succeeded ;
    }

    private string ExtractFamilyInfo( Autodesk.Revit.ApplicationServices.Application app, string FamilyPath,
      string root = null, List<string> keys = null )
    {
      string res = "" ;

      string path = ( root == null ) ? FamilyPath : FamilyPath.Replace( root, "" ) ;

      var famDoc = app.OpenDocumentFile( FamilyPath ) ;
      var familyManager = famDoc.FamilyManager ;

      Dictionary<string, FamilyParameter> fps = new Dictionary<string, FamilyParameter>() ;
      foreach ( FamilyParameter fp in familyManager.Parameters ) {
        string name = fp.Definition.Name ;
        if ( fps.ContainsKey( name ) ) {
          string msg = string.Format( "パラメータ重複[{0}],{1}{2}", name, FamilyPath, Environment.NewLine ) ;
          File.AppendAllText( errPath, msg, Env.encoding ) ;

          continue ;
        }

        fps.Add( fp.Definition.Name, fp ) ;
      }

      foreach ( FamilyType t in familyManager.Types ) {
        string s1 = "" ;
        foreach ( string key in keys ) {
          if ( fps.ContainsKey( key ) ) {
            FamilyParameter fp = fps[ key ] ;
            s1 += ( t.HasValue( fp ) ) ? FamilyParamValueString( t, fp, famDoc ) : "" ;
          }

          s1 += "," ;
        }

        res += ( s1 + path + Environment.NewLine ) ;
      }

      famDoc.Close( false ) ;

      return res ;
    }

    static string FamilyParamValueString( FamilyType t, FamilyParameter fp, Document doc )
    {
      string value = "" ;
      switch ( fp.StorageType ) {
        case StorageType.Double :
          value += t.AsDouble( fp ).ToString() ;
          break ;

        case StorageType.ElementId :
          ElementId id = t.AsElementId( fp ) ;
          Element e = doc.GetElement( id ) ;
          value += id.ToString() ;
          break ;

        case StorageType.Integer :
          value += t.AsInteger( fp ).ToString() ;
          break ;

        case StorageType.String :
          value += t.AsString( fp ) ;
          break ;
      }

      return value ;
    }
  }


  [Transaction( TransactionMode.Manual )]
  public class DummyFamilyAdd : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      UIApplication uiapp = commandData.Application ;
      UIDocument uidoc = uiapp.ActiveUIDocument ;
      Autodesk.Revit.ApplicationServices.Application app = uiapp.Application ;
      Document doc = uidoc.Document ;

      DummyFamily df = new DummyFamily( doc ) ;
      FamilyInstance fi = df.install() ;
      Dictionary<string, string> parameters = DummyFamily.parameters( Schedule.view( doc ) ) ;
      parameters[ "大分類" ] = "abcdefg" ;
      parameters[ "中分類" ] = "中分類みほん" ;

      using ( Transaction t = new Transaction( doc, "set parameters" ) ) {
        t.Start() ;
        Misc.ParameterSet( doc, fi, parameters ) ;
        t.Commit() ;
      }

      return Result.Succeeded ;
    }
  }
}