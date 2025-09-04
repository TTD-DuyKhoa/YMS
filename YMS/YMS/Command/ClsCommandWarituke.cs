using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using Autodesk.Revit.UI.Selection ;
using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Runtime.CompilerServices ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;
using System.Windows.Media ;
using System.Windows.Media.Animation ;
using System.Windows.Media.Media3D ;
using YMS.DLG ;
using YMS.Parts ;
using static YMS.DLG.DlgWaritsuke ;

namespace YMS.Command
{
  public class ClsCommandWarituke
  {
    #region メンバ変数

    private static Document m_doc { get ; set ; }

    public static ElementId m_baseId { get ; set ; }

    public static XYZ m_InsertPoint { get ; set ; }

    public static XYZ m_EndPoint { get ; set ; }

    public static DlgWaritsuke.WaritsukeMode m_WaritsukeMode { get ; set ; }

    public static List<ElementId> m_createdIds { get ; set ; }

    public static List<ElementId> m_createdIds_HaraokoshiTateW { get ; set ; }

    public static List<XYZ> m_createdXYZs { get ; set ; }

    public static bool m_isHaraokoshiTateW { get ; set ; }

    public static double m_VerticalGap { get ; set ; }

    public static bool m_isKussaku { get ; set ; }

    public static bool m_isHiuchiTateW { get ; set ; }

    public static bool m_isTop { get ; set ; }

    public static System.Drawing.Point m_DialogPoint { get ; set ; }

    public static double m_BaseOffset { get ; set ; }

    /// <summary>
    /// メガビーム腹起の位置補正用のベクトル
    /// </summary>
    public static XYZ m_vecMegabeam { get ; set ; }

    #endregion

    /// <summary>
    /// 自動割付機能　1.0、1.5、2.0、2.5……などキリの良い長さの鋼材を割付用主材に置換する
    /// </summary>
    /// <param name="uiapp"></param>
    public static void CommandJidouWarituke( UIApplication uiapp )
    {
      UIDocument uidoc = uiapp.ActiveUIDocument ;
      Document doc = uidoc.Document ;

      DLG.DlgJougenNum dlg = new DLG.DlgJougenNum() ;
      DialogResult result = dlg.ShowDialog() ;
      if ( result != DialogResult.OK ) {
        return ;
      }

      double jougenM = dlg.m_jougen ;

      double dKensakuNum = Math.Floor( jougenM / 0.5 ) ;
      int nkensakuNum = (int) dKensakuNum - 1 ;

      List<ElementId> kariIdList = ClsKariKouzai.GetAllIdList2( doc ) ;

      List<string> kariPathList = new List<string>() ;
      foreach ( ElementId kariId in kariIdList ) {
        FamilyInstance inst = doc.GetElement( kariId ) as FamilyInstance ;

        if ( inst != null ) {
          //仮鋼材に紐づいたベースを取得（レベルを取りたい）
          ElementId baseId = ClsWarituke.GetConnectionBase( doc, kariId ) ;
          ;
          if ( baseId == null ) {
            continue ;
          }

          ElementId lvId = ClsRevitUtil.GetFamilyInstanceLevel( doc, baseId ) ;

          Level lv = doc.GetElement( lvId ) as Level ;

          string lvName = lv.Name ;

          //長さ取得
          LocationCurve lCurve = inst.Location as LocationCurve ;
          if ( lCurve != null ) {
            Curve cv = lCurve.Curve ;
            //XYZ tmpStPoint = ClsRevitUtil.ConvertPointRevit2Geo(cv.GetEndPoint(0));
            //XYZ tmpEdPoint = ClsRevitUtil.ConvertPointRevit2Geo(cv.GetEndPoint(1));
            XYZ tmpStPoint = cv.GetEndPoint( 0 ) ;
            XYZ tmpEdPoint = cv.GetEndPoint( 1 ) ;
            double dist = tmpStPoint.DistanceTo( tmpEdPoint ) ;
            double jougenMM = ClsRevitUtil.CovertToAPI( ClsGeo.m2mm( jougenM ) ) ;
            //Revit単位系での比較を行う誤差防止のため
            if ( ClsGeo.GEO_LE( dist, jougenMM ) ) {
              //Revit表示単位系に戻す
              dist = ClsRevitUtil.CovertFromAPI( dist ) ;
              //入力値以下の長さの仮鋼材確定
              double checkSize = 500 ;
              for ( int i = 1 ; i <= nkensakuNum ; i++ ) {
                checkSize += 500 ;
                //1.0～上限値まで0.5ピッチの鋼材があるか検索

                //#31932
                double sabunAbs = Math.Abs( checkSize - dist ) ;
                if ( ClsGeo.GEO_LE( sabunAbs, 0.99 ) ) {
                  dist = checkSize ;
                }

                if ( ClsGeo.GEO_EQ( checkSize, dist ) ) {
                  string familyName = inst.Symbol.FamilyName ;
                  string typeName = inst.Name ;
                  string path =
                    Master.ClsYamadomeCsv.GetYamadomeFamilyFromKarikouzai( familyName, ClsGeo.mm2m( checkSize ) ) ;
                  string symbolName = ClsRevitUtil.GetFamilyName( path ) ;
                  if ( ! ClsRevitUtil.LoadFamilyData( doc, path, out Family family1 ) ) {
                    MessageBox.Show( "ファミリの取得に失敗しました" ) ;
                    return ;
                  }

                  FamilySymbol kouzaiSym1 = ClsRevitUtil.GetFamilySymbol( doc, symbolName, typeName ) ;

                  double offset = ClsRevitUtil.GetParameterDouble( doc, kariId, "ホストからのオフセット" ) ;

                  using ( Transaction t = new Transaction( doc, "仮鋼材削除" ) ) {
                    t.Start() ;
                    ClsRevitUtil.Delete( doc, kariId ) ;
                    t.Commit() ;
                  }

                  using ( Transaction t = new Transaction( doc, "鋼材作成" ) ) {
                    t.Start() ;
                    ElementId createdId1 = ClsRevitUtil.Create( doc, cv.GetEndPoint( 0 ), lvId, kouzaiSym1 ) ;
                    t.Commit() ;
                    t.Start() ;
                    ClsRevitUtil.SetParameter( doc, createdId1, "基準レベルからの高さ", offset ) ;

                    // シンボルを回転
                    Line rotationAxis =
                      Line.CreateBound( cv.GetEndPoint( 0 ), cv.GetEndPoint( 0 ) + XYZ.BasisZ ) ; // Z軸周りに回転
                    XYZ vec1 = ( cv.GetEndPoint( 1 ) - cv.GetEndPoint( 0 ) ).Normalize() ;
                    double radians1 = XYZ.BasisX.AngleOnPlaneTo( vec1, XYZ.BasisZ ) ;
                    ElementTransformUtils.RotateElement( doc, createdId1, rotationAxis, radians1 ) ;
                    t.Commit() ;
                  }

                  break ;
                }
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// 割付部材削除機能
    /// </summary>
    /// <param name="uiapp"></param>
    public static void CommandDeleteWarituke( UIApplication uiapp )
    {
      List<ElementId> selectedIds = new List<ElementId>() ;
      UIDocument uidoc = uiapp.ActiveUIDocument ;
      Document doc = uidoc.Document ;
      try {
        TaskDialog td = new TaskDialog( "削除対象の選択" ) ;
        td.MainInstruction = "主材を削除しますか？\n" + "（はい:割付部材　いいえ:仮鋼材）" ;
        td.CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No ;
        td.DefaultButton = TaskDialogResult.Yes ;

        TaskDialogResult result = td.Show() ;
        if ( result == TaskDialogResult.Yes ) {
          List<string> kouzaiList = new List<string>() ;
          kouzaiList.AddRange( Master.ClsYamadomeCsv.GetYamadomeFamilynameList().ToList() ) ;
          kouzaiList.AddRange( Master.ClsSupportPieceCsv.GetFamilyNameList().ToList() ) ;
          kouzaiList.AddRange( Master.ClsCoverPlateCSV.GetFamilyNameList().ToList() ) ;
          kouzaiList.AddRange( Master.ClsJackCsv.GetFamilyNameList().ToList() ) ;
          kouzaiList.AddRange( Master.ClsJackCoverCSV.GetFamilyNameList().ToList() ) ;

          if ( ! ClsRevitUtil.PickObjectsSymbolFilters( uidoc, "削除する割付部材を選択してください。", kouzaiList, ref selectedIds ) ) {
            return ;
          }

          if ( selectedIds.Count < 1 ) {
            return ;
          }

          using ( Transaction t = new Transaction( doc, "割付部材削除" ) ) {
            t.Start() ;
            ClsRevitUtil.Delete( doc, selectedIds ) ;
            t.Commit() ;
          }
        }
        else if ( result == TaskDialogResult.No ) {
          //仮鋼材部品名を取得
          List<string> kariKouzaiList = new List<string>() ;
          kariKouzaiList.AddRange( Master.ClsYamadomeCsv.GetKariFamilyNameList().ToList() ) ;
          kariKouzaiList.AddRange( Master.ClsSupportPieceCsv.GetFamilyNameList().ToList() ) ;
          //kariKouzaiList.AddRange(Master.ClsHBeamCsv.GetFamilyNameList().ToList());
          //kariKouzaiList.AddRange(Master.ClsChannelCsv.GetFamilyNameList().ToList());
          //kariKouzaiList.AddRange(Master.ClsAngleCsv.GetFamilyNameList().ToList());
          //kariKouzaiList.AddRange(Master.ClsHiuchiCsv.GetFamilyNameList().ToList());
          //// kariKouzaiList.AddRange(Master.ClsJackCsv.GetFamilyNameList().ToList()); //＃31634
          //kariKouzaiList.AddRange(Master.ClsBurumanCSV.GetFamilyNameList().ToList());
          //kariKouzaiList.AddRange(Master.ClsRikimanCSV.GetFamilyNameList().ToList());


          //仮鋼材を削除
          if ( ! ClsRevitUtil.PickObjectsSymbolFilters( uidoc, "削除する割付部材を選択してください。", kariKouzaiList,
                ref selectedIds ) ) {
            return ;
          }

          if ( selectedIds.Count < 1 ) {
            return ;
          }

          //図面上の仮鋼材を全て取得
          List<ElementId> elements = ClsRevitUtil.GetAllCreatedFamilyInstanceList( doc ) ;
          List<ElementId> targetFamilies = new List<ElementId>() ;
          foreach ( string name in kariKouzaiList ) {
            foreach ( ElementId elem in selectedIds ) {
              FamilyInstance inst = doc.GetElement( elem ) as FamilyInstance ;
              if ( inst != null && inst.Symbol.FamilyName == name ) //ファミリ名でフィルター
              {
                if ( ClsWarituke.GetWarituke( doc, elem ) != ClsWarituke.WARITUKE )
                  targetFamilies.Add( elem ) ;
              }
            }
          }

          using ( Transaction t = new Transaction( doc, "割付部材削除" ) ) {
            t.Start() ;
            ClsRevitUtil.Delete( doc, targetFamilies ) ;
            t.Commit() ;
          }
        }
      }
      catch ( Exception e ) {
        return ;
      }
    }

    public static void CommandWarituke( UIApplication uiapp )
    {
      //ドキュメントを取得
      UIDocument uidoc = uiapp.ActiveUIDocument ;
      m_doc = uidoc.Document ;

      // 対象のベースを取得
      ElementId buzaiId = null ;
      if ( ! ClsWarituke.PickBaseObject( uidoc, ref buzaiId ) ) {
        return ;
      }

      FamilyInstance inst = m_doc.GetElement( buzaiId ) as FamilyInstance ;
      foreach ( string name in ClsGlobal.m_baseShinList ) {
        if ( inst.Name == name ) {
          m_baseId = buzaiId ;
          break ;
        }
        else {
          m_baseId = ClsWarituke.GetConnectionBase( m_doc, buzaiId ) ;
        }
      }

      List<ElementId> buzaiIdList = ClsWarituke.GetConnectionBuzai( m_doc, m_baseId ) ;

      List<ElementId> jackWaritukeList = new List<ElementId>() ;
      List<ElementId> jackCoverList = new List<ElementId>() ;
      foreach ( ElementId id in buzaiIdList ) {
        string flagName = ClsJack.GetBaseId( m_doc, id ) ;
        if ( flagName == "割付" ) {
          jackWaritukeList.Add( id ) ;
          continue ;
        }

        ElementId jcid = null ;
        if ( ClsJack.GetConnectedJackCover( m_doc, id, ref jcid ) ) {
          jackCoverList.Add( jcid ) ;
        }
      }

      foreach ( ElementId id in jackWaritukeList ) {
        buzaiIdList.Remove( id ) ;
      }

      if ( jackCoverList.Count > 0 ) {
        buzaiIdList.AddRange( jackCoverList ) ;
      }

      m_InsertPoint = null ;
      m_EndPoint = null ;
      m_WaritsukeMode = DlgWaritsuke.WaritsukeMode.Normal ;
      m_isHaraokoshiTateW = false ;
      m_createdIds = new List<ElementId>() ;
      m_createdXYZs = new List<XYZ>() ;
      m_createdIds_HaraokoshiTateW = new List<ElementId>() ;
      m_isKussaku = false ;
      m_isTop = false ;
      m_DialogPoint = System.Drawing.Point.Empty ;
      m_isHiuchiTateW = false ;
      m_BaseOffset = 0.0 ;

      WaritukeMain( uiapp, buzaiIdList ) ;
    }

    private static void WaritukeMain( UIApplication uiapp, List<ElementId> deleteBuuzaiIdList )
    {
      //ドキュメントを取得
      UIDocument uidoc = uiapp.ActiveUIDocument ;
      Document doc = uidoc.Document ;

      // ベースから情報を取得
      FamilyInstance instBase = doc.GetElement( m_baseId ) as FamilyInstance ;
      Curve cvBase = null ;
      XYZ tmpStPoint = new XYZ() ;
      XYZ tmpEdPoint = new XYZ() ;
      if ( instBase.Location is LocationPoint ) {
        tmpStPoint = ( instBase.Location as LocationPoint ).Point ;
        var dist = ClsRevitUtil.GetParameterDouble( doc, m_baseId, "長さ" ) ;
        tmpEdPoint = tmpStPoint + dist * instBase.HandOrientation ;
        cvBase = Line.CreateBound( tmpStPoint, tmpEdPoint ) ;
      }
      else {
        cvBase = ( instBase.Location as LocationCurve ).Curve ;
        tmpStPoint = cvBase.GetEndPoint( 0 ) ;
        tmpEdPoint = cvBase.GetEndPoint( 1 ) ;
      }

      string typeName = instBase.Name.Replace( "ベース", "" ) ;
      string steelSizeOrign = string.Empty ;
      double syuzaiDiff = 0.0 ;
      double syuzaiDiff2 = 0.0 ;
      string baseDan = ClsRevitUtil.GetParameter( doc, m_baseId, "段" ) ;
      double doubleMaxLength = 0.0 ;
      double kouzaiSize = 0.0 ;
      bool cross = false ;
      switch ( typeName ) {
        case "腹起" :
          steelSizeOrign = ClsRevitUtil.GetInstMojiParameter( doc, m_baseId, "鋼材サイズ" ) ;
          break ;
        case "切梁" :
          steelSizeOrign = ClsRevitUtil.GetInstMojiParameter( doc, m_baseId, "鋼材サイズ" ) ;
          ClsYMSUtil.GetDifferenceWithAllBase( doc, m_baseId, out syuzaiDiff, out syuzaiDiff2 ) ;
          //ClsYMSUtil.GetDifferenceWithBase(doc, m_baseId, out syuzaiDiff, steelSizeOrign);
          break ;
        case "切梁火打" :
          if ( ClsRevitUtil.GetInstMojiParameter( doc, m_baseId, "構成" ) == "シングル" ) {
            steelSizeOrign = ClsRevitUtil.GetInstMojiParameter( doc, m_baseId, "鋼材サイズ(シングル)" ) ;
            ClsYMSUtil.GetDifferenceWithAllBase( doc, m_baseId, out syuzaiDiff, out syuzaiDiff2 ) ;
            //ClsYMSUtil.GetDifferenceWithBase(doc, m_baseId, out syuzaiDiff, steelSizeOrign);
          }
          else {
            if ( DlgWaritsuke.ShowSelectDialogCornerHiuchi2() ) {
              m_isTop = true ;
              steelSizeOrign = ClsRevitUtil.GetInstMojiParameter( doc, m_baseId, "鋼材サイズ(ダブル上)" ) ;
              ClsYMSUtil.GetDifferenceWithAllBase( doc, m_baseId, out syuzaiDiff, out syuzaiDiff2 ) ;
              //ClsYMSUtil.GetDifferenceWithBase(doc, m_baseId, out syuzaiDiff, steelSizeOrign);
              doubleMaxLength = ClsRevitUtil.GetParameterDouble( doc, m_baseId, "火打長さ(ダブル上)L1" ) ;
            }
            else {
              m_isTop = false ;
              steelSizeOrign = ClsRevitUtil.GetInstMojiParameter( doc, m_baseId, "鋼材サイズ(ダブル下)" ) ;
              ClsYMSUtil.GetDifferenceWithAllBase( doc, m_baseId, out syuzaiDiff, out syuzaiDiff2 ) ;
              //ClsYMSUtil.GetDifferenceWithBase(doc, m_baseId, out syuzaiDiff, steelSizeOrign);
              doubleMaxLength = ClsRevitUtil.GetParameterDouble( doc, m_baseId, "火打長さ(ダブル下)L2" ) ;
            }
          }

          break ;
        case "隅火打" :
          if ( ClsRevitUtil.GetInstMojiParameter( doc, m_baseId, "構成" ) == "ダブル" ) {
            m_isTop = DlgWaritsuke.ShowSelectDialogCornerHiuchi2() ;
          }

          steelSizeOrign = ClsRevitUtil.GetInstMojiParameter( doc, m_baseId, "鋼材サイズ" ) ;
          //ClsYMSUtil.GetDifferenceWithBase(doc, m_baseId, out syuzaiDiff, steelSizeOrign);
          ClsYMSUtil.GetDifferenceWithAllBase( doc, m_baseId, out syuzaiDiff, out syuzaiDiff2 ) ;
          if ( m_isTop ) {
            doubleMaxLength = ClsRevitUtil.GetParameterDouble( doc, m_baseId, "火打長さ(ダブル上)L1" ) ;
          }
          else {
            doubleMaxLength = ClsRevitUtil.GetParameterDouble( doc, m_baseId, "火打長さ(ダブル下)L2" ) ;
          }

          break ;
        case "切梁継ぎ" :
          if ( ClsRevitUtil.GetInstMojiParameter( doc, m_baseId, "鋼材タイプ" ) != "主材" ) {
            MessageBox.Show( "割付対象外の鋼材です" ) ;
            return ;
          }

          string target1 = "切梁", target2 = "腹起" ;
          if ( ClsRevitUtil.GetInstMojiParameter( doc, m_baseId, "構成" ) == "シングル" ) {
            steelSizeOrign = ClsRevitUtil.GetInstMojiParameter( doc, m_baseId, "鋼材サイズ(シングル)" ) ;
            ClsYMSUtil.GetDifferenceWithAllBase( doc, m_baseId, out syuzaiDiff, out syuzaiDiff2 ) ;
          }
          else {
            if ( DlgWaritsuke.ShowSelectDialogKiribariTsugi( target1, target2 ) ) {
              m_isTop = true ;
              steelSizeOrign = ClsRevitUtil.GetInstMojiParameter( doc, m_baseId, "切梁側/鋼材サイズ(ダブル)" ) ;
              ClsYMSUtil.GetDifferenceWithAllBase( doc, m_baseId, out syuzaiDiff, out syuzaiDiff2 ) ;
              doubleMaxLength = ClsRevitUtil.GetParameterDouble( doc, m_baseId, "切梁側/切梁繋ぎ長さ" ) ;
            }
            else {
              m_isTop = false ;
              baseDan = baseDan != "上段" ? "上段" : "下段" ;
              steelSizeOrign = ClsRevitUtil.GetInstMojiParameter( doc, m_baseId, "腹起側/鋼材サイズ(ダブル)" ) ;
              ClsYMSUtil.GetDifferenceWithAllBase( doc, m_baseId, out syuzaiDiff, out syuzaiDiff2 ) ;
              doubleMaxLength = ClsRevitUtil.GetParameterDouble( doc, m_baseId, "腹起側/切梁繋ぎ長さ" ) ;
            }
          }

          break ;
        case "切梁受け材" :
          if ( ClsRevitUtil.GetInstMojiParameter( doc, m_baseId, "鋼材タイプ" ) != "主材" ) {
            MessageBox.Show( "割付対象外の鋼材です" ) ;
            return ;
          }

          steelSizeOrign = ClsRevitUtil.GetInstMojiParameter( doc, m_baseId, "鋼材サイズ" ) ;
          ClsYMSUtil.GetDifferenceWithAllBase( doc, m_baseId, out syuzaiDiff, out syuzaiDiff2 ) ;
          //接続しているベースのサイズ分オフセット値を増やす
          List<ElementId> insecIdUList = ClsYMSUtil.GetIntersectionBase( doc, m_baseId ) ;
          if ( insecIdUList.Count > 0 ) {
            string insecBaseKouzaiSize = ClsRevitUtil.GetParameter( doc, insecIdUList[ 0 ], "鋼材サイズ" ) ;
            string dan = ClsRevitUtil.GetParameter( doc, insecIdUList[ 0 ], "段" ) ;
            if ( dan == "下段" ) {
              kouzaiSize += ClsRevitUtil.CovertToAPI( Master.ClsYamadomeCsv.GetWidth( insecBaseKouzaiSize ) * 2 ) ;
            }
            else if ( dan == "同段" ) {
              kouzaiSize += ClsRevitUtil.CovertToAPI( Master.ClsYamadomeCsv.GetWidth( insecBaseKouzaiSize ) ) ;
            }
          }

          break ;
        case "切梁繋ぎ材" :
          if ( ClsRevitUtil.GetInstMojiParameter( doc, m_baseId, "分類" ) != "主材" ) {
            MessageBox.Show( "割付対象外の鋼材です" ) ;
            return ;
          }

          steelSizeOrign = ClsRevitUtil.GetInstMojiParameter( doc, m_baseId, "サイズ" ) ;
          ClsYMSUtil.GetDifferenceWithAllBase( doc, m_baseId, out syuzaiDiff, out syuzaiDiff2 ) ;
          //接続しているベースのサイズ分オフセット値を増やす
          List<ElementId> insecIdList = ClsYMSUtil.GetIntersectionBase( doc, m_baseId ) ;
          if ( insecIdList.Count > 0 ) {
            string insecBaseKouzaiSize = ClsRevitUtil.GetParameter( doc, insecIdList[ 0 ], "鋼材サイズ" ) ;
            string dan = ClsRevitUtil.GetParameter( doc, insecIdList[ 0 ], "段" ) ;
            if ( dan == "上段" ) {
              kouzaiSize += ClsRevitUtil.CovertToAPI( Master.ClsYamadomeCsv.GetWidth( insecBaseKouzaiSize ) * 2 ) ;
            }
            else if ( dan == "同段" && ClsRevitUtil.GetParameter( doc, insecIdList[ 0 ], "構成" ) == "ダブル" ) {
              kouzaiSize += ClsRevitUtil.CovertToAPI( Master.ClsYamadomeCsv.GetWidth( insecBaseKouzaiSize ) * 2 ) ;
            }
            else if ( dan == "同段" ) {
              kouzaiSize += ClsRevitUtil.CovertToAPI( Master.ClsYamadomeCsv.GetWidth( insecBaseKouzaiSize ) ) ;
            }
          }

          break ;
        case "火打繋ぎ材" :
          if ( ClsRevitUtil.GetInstMojiParameter( doc, m_baseId, "分類" ) != "主材" ) {
            MessageBox.Show( "割付対象外の鋼材です" ) ;
            return ;
          }

          steelSizeOrign = ClsRevitUtil.GetInstMojiParameter( doc, m_baseId, "サイズ" ) ;
          ClsYMSUtil.GetDifferenceWithAllBase( doc, m_baseId, out syuzaiDiff, out syuzaiDiff2 ) ;
          //接続しているベースのサイズ分オフセット値を増やす
          List<ElementId> insecIdHList = ClsYMSUtil.GetIntersectionBase( doc, m_baseId ) ;
          if ( insecIdHList.Count > 0 ) {
            string insecBaseKouzaiSize = ClsRevitUtil.GetParameter( doc, insecIdHList[ 0 ], "鋼材サイズ" ) ;
            string dan = ClsRevitUtil.GetParameter( doc, insecIdHList[ 0 ], "段" ) ;
            if ( dan == "上段" ) {
              kouzaiSize += ClsRevitUtil.CovertToAPI( Master.ClsYamadomeCsv.GetWidth( insecBaseKouzaiSize ) * 2 ) ;
            }
            else if ( dan == "同段" && ClsRevitUtil.GetParameter( doc, insecIdHList[ 0 ], "構成" ) == "ダブル" ) {
              kouzaiSize += ClsRevitUtil.CovertToAPI( Master.ClsYamadomeCsv.GetWidth( insecBaseKouzaiSize ) * 2 ) ;
            }
            else if ( dan == "同段" ) {
              kouzaiSize += ClsRevitUtil.CovertToAPI( Master.ClsYamadomeCsv.GetWidth( insecBaseKouzaiSize ) ) ;
            }
          }

          break ;
        case "斜梁" :
          steelSizeOrign = ClsRevitUtil.GetInstMojiParameter( doc, m_baseId, "鋼材サイズ" ) ;
          //var name = "斜張" + m_baseId.IntegerValue.ToString();
          //ClsYMSUtil.CreateBaseElevationView(doc, m_baseId, name);
          //ClsYMSUtil.ChangeView(uidoc, name);
          cross = true ;
          //ファミリにタイプが存在しないため仮で切梁に設定している
          //typeName = "切梁";
          break ;
        case "斜梁火打" :
          steelSizeOrign = ClsRevitUtil.GetInstMojiParameter( doc, m_baseId, "鋼材サイズ" ) ;
          //var name = "斜張" + m_baseId.IntegerValue.ToString();
          //ClsYMSUtil.CreateBaseElevationView(doc, m_baseId, name);
          //ClsYMSUtil.ChangeView(uidoc, name);
          cross = true ;
          //2025/4/23 M.Sakuraba ファミリにタイプが存在しないため仮で斜梁に設定している
          typeName = "斜梁" ;
          break ;
        case "斜梁繋ぎ材" :
          if ( ClsRevitUtil.GetInstMojiParameter( doc, m_baseId, "分類" ) != "主材" ) {
            MessageBox.Show( "割付対象外の鋼材です" ) ;
            return ;
          }

          steelSizeOrign = ClsRevitUtil.GetInstMojiParameter( doc, m_baseId, "サイズ" ) ;
          //typeName = "斜梁";
          break ;
        case "斜梁受け材" :
          if ( ClsRevitUtil.GetInstMojiParameter( doc, m_baseId, "鋼材タイプ" ) != "主材" ) {
            MessageBox.Show( "割付対象外の鋼材です" ) ;
            return ;
          }

          steelSizeOrign = ClsRevitUtil.GetInstMojiParameter( doc, m_baseId, "鋼材サイズ" ) ;
          //typeName = "斜梁";
          break ;
        default :
          MessageBox.Show( "対象外のベースです" ) ;
          return ;
      }

      //ベースがオフセットされている場合は取得位置のZ軸に足す。レベルの高さしか取得出来ていないため
      var baseOffset = ClsRevitUtil.GetParameterDouble( doc, m_baseId, "ホストからのオフセット" ) ;
      m_BaseOffset += baseOffset ;
      tmpStPoint = new XYZ( tmpStPoint.X, tmpStPoint.Y, tmpStPoint.Z + baseOffset ) ;
      tmpEdPoint = new XYZ( tmpEdPoint.X, tmpEdPoint.Y, tmpEdPoint.Z + baseOffset ) ;
      //kouzaiSize += ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(steelSizeOrign.Replace("HA", "")) * 10);
      //if (steelSizeOrign.Contains("SMH"))
      //{
      //    if (steelSizeOrign.Contains("80SMH"))
      //    {
      //        kouzaiSize = ClsRevitUtil.CovertToAPI(400.0);
      //    }
      //    else if (steelSizeOrign.Contains("60SMH"))
      //    {
      //        kouzaiSize = ClsRevitUtil.CovertToAPI(400.0);
      //    }
      //    else
      //    {
      //        kouzaiSize += ClsRevitUtil.CovertToAPI(ClsCommonUtils.ChangeStrToDbl(steelSizeOrign.Replace("SMH", "")) * 10);
      //    }
      //}
      kouzaiSize += ClsRevitUtil.CovertToAPI( Master.ClsYamadomeCsv.GetWidth( steelSizeOrign ) ) ;
      // 割付範囲の選択
      XYZ picS = tmpStPoint, picE = tmpEdPoint ;
      DlgWaritsukeMode dlgWaritsukeMode = new DlgWaritsukeMode() ;
      dlgWaritsukeMode.ShowDialog() ;
      if ( dlgWaritsukeMode.DialogResult == DialogResult.Yes ) {
        //始終点選択モード
        picS = GetBetweenBasePoint( uidoc, tmpStPoint, tmpEdPoint, "始点" ) ;
        picE = GetBetweenBasePoint( uidoc, tmpStPoint, tmpEdPoint, "終点" ) ;
        tmpStPoint = picS ;
        tmpEdPoint = picE ;
        if ( ClsGeo.GEO_EQ( tmpStPoint, tmpEdPoint ) ) {
          return ;
        }
      }
      else {
        //始終点自動決定モード※立面図では点の指定が出来ない
        if ( uidoc.ActiveView.ViewType != ViewType.Elevation ) {
          XYZ selPoint = uidoc.Selection.PickPoint( "始点方向を指定してください" ) ; //
          if ( ! ClsRevitUtil.CheckNearGetEndPoint( cvBase, selPoint ) ) {
            XYZ change = tmpEdPoint ;
            tmpEdPoint = tmpStPoint ;
            tmpStPoint = change ;
          }
        }
        else {
          //Z軸が下の点から始める
          //if(ClsGeo.GEO_GT(tmpStPoint.Z, tmpEdPoint.Z))
          //{
          //    XYZ change = tmpEdPoint;
          //    tmpEdPoint = tmpStPoint;
          //    tmpStPoint = change;
          //}
        }
      }

      //斜張専用View作成切替
      if ( cross ) {
        var name = "斜梁" + m_baseId.IntegerValue.ToString() ;
        ClsYMSUtil.CreateBaseElevationView( doc, m_baseId, name ) ;
        ClsYMSUtil.ChangeView( uidoc, name ) ;
      }

      switch ( baseDan ) {
        case "上段" :
          m_BaseOffset += ( kouzaiSize / 2 ) ;
          m_BaseOffset += syuzaiDiff ;
          break ;
        case "下段" :
          m_BaseOffset -= ( kouzaiSize / 2 ) ;
          m_BaseOffset -= syuzaiDiff2 ;
          break ;
        case "同段" :
          m_BaseOffset = baseOffset ; // 0.0;
          break ;
        default :
          break ;
      }

      double offset = m_BaseOffset ;

      XYZ vecCcoverPlateoffset = XYZ.Zero ;
      if ( typeName == "腹起" ) {
        ClsHaraokoshiBase chb = new ClsHaraokoshiBase() ;
        chb.SetParameter( doc, m_baseId ) ;

        if ( ( ClsRevitUtil.GetInstMojiParameter( doc, m_baseId, "横本数" ) == "ダブル" ) || ( steelSizeOrign == "80SMH" ) ) {
          m_isKussaku = DlgWaritsuke.ShowSelectDialogHaraokoshi() ;
        }

        if ( ClsRevitUtil.GetInstMojiParameter( doc, m_baseId, "縦本数" ) == "ダブル" ) {
          m_isHaraokoshiTateW = true ;
          //縦方向の隙間は事前に入れる
          string sVerticalGap = ClsRevitUtil.GetInstMojiParameter( doc, m_baseId, "縦方向の隙間" ) ;
          m_VerticalGap = ClsRevitUtil.CovertToAPI( ClsCommonUtils.ChangeStrToDbl( sVerticalGap ) ) ;
        }

        XYZ DirectionWin = Line.CreateBound( tmpStPoint, tmpEdPoint ).Direction ;

        bool sInsec = false, eInsec = false, sInsecDouble = false, eInsecDouble = false ;
        //自動モード時に始終点を腹起ベースの交点に調整
        if ( dlgWaritsukeMode.DialogResult == DialogResult.No ) {
          //図面上の腹起ベースを全て取得
          List<ElementId> haraIdList = ClsHaraokoshiBase.GetAllHaraokoshiBaseList( doc ) ;

          //腹起ベースとの交点を探す
          Curve cvHaraBase = Line.CreateBound( tmpStPoint, tmpEdPoint ) ;
          foreach ( ElementId haraId in haraIdList ) {
            FamilyInstance haraokoshiShinInst = doc.GetElement( haraId ) as FamilyInstance ;
            LocationCurve lCurveHara = haraokoshiShinInst.Location as LocationCurve ;
            if ( lCurveHara != null ) {
              Curve cvHara = lCurveHara.Curve ;

              XYZ insec = ClsRevitUtil.GetIntersection( cvHaraBase, cvHara ) ;
              if ( insec != null ) {
                if ( ! ClsRevitUtil.CheckNearGetEndPoint( cvHaraBase, insec ) ) {
                  tmpEdPoint = insec ;
                  eInsec = true ;
                  if ( ClsRevitUtil.GetInstMojiParameter( doc, haraId, "横本数" ) == "ダブル" &&
                       chb.m_katimake != ClsHaraokoshiBase.WinLose.Lose ) {
                    tmpEdPoint = new XYZ( tmpEdPoint.X + ( kouzaiSize * DirectionWin.X ),
                      tmpEdPoint.Y + ( kouzaiSize * DirectionWin.Y ), tmpEdPoint.Z ) ;
                  }
                }
                else {
                  tmpStPoint = insec ;
                  sInsec = true ;
                  if ( ClsRevitUtil.GetInstMojiParameter( doc, haraId, "横本数" ) == "ダブル" &&
                       chb.m_katimake != ClsHaraokoshiBase.WinLose.Lose ) {
                    tmpStPoint = new XYZ( tmpStPoint.X - ( kouzaiSize * DirectionWin.X ),
                      tmpStPoint.Y - ( kouzaiSize * DirectionWin.Y ), tmpStPoint.Z ) ;
                  }
                }
              }
            }
          }
        }


        XYZ ptBaseSp = cvBase.GetEndPoint( 0 ) ;
        XYZ ptBaseEd = cvBase.GetEndPoint( 1 ) ;
        XYZ Direction = Line.CreateBound( ptBaseSp, ptBaseEd ).Direction ;
        //XYZ DirectionWin = Line.CreateBound(tmpStPoint, tmpEdPoint).Direction;
        XYZ tmpStPointOrigin = tmpStPoint ;

        tmpStPoint = new XYZ( tmpStPoint.X + ( kouzaiSize / 2 * Direction.Y ),
          tmpStPoint.Y - ( kouzaiSize / 2 * Direction.X ), tmpStPoint.Z ) ;
        tmpEdPoint = new XYZ( tmpEdPoint.X + ( kouzaiSize / 2 * Direction.Y ),
          tmpEdPoint.Y - ( kouzaiSize / 2 * Direction.X ), tmpEdPoint.Z ) ;

        if ( ( ClsRevitUtil.GetInstMojiParameter( doc, m_baseId, "横本数" ) == "ダブル" && ! m_isKussaku ) ||
             ( steelSizeOrign == ( "80SMH" ) && ! m_isKussaku ) ) {
          tmpStPoint = new XYZ( tmpStPoint.X + ( kouzaiSize * Direction.Y ),
            tmpStPoint.Y - ( kouzaiSize * Direction.X ), tmpStPoint.Z ) ;
          tmpEdPoint = new XYZ( tmpEdPoint.X + ( kouzaiSize * Direction.Y ),
            tmpEdPoint.Y - ( kouzaiSize * Direction.X ), tmpEdPoint.Z ) ;
        }

        vecCcoverPlateoffset = Line.CreateBound( tmpStPointOrigin, tmpStPoint ).Direction.Normalize() ;
        if ( m_isKussaku ) {
          m_vecMegabeam = vecCcoverPlateoffset ;
        }
        else {
          m_vecMegabeam = -vecCcoverPlateoffset ;
        }

        //二点指定の場合は勝ちは無視 #31635
        if ( chb.m_katimake == ClsHaraokoshiBase.WinLose.Win && dlgWaritsukeMode.DialogResult != DialogResult.Yes ) {
          //その他の腹起ベースとの交点が存在しない場合、勝ちは無視）
          if ( sInsec )
            tmpStPoint = new XYZ( tmpStPoint.X - ( kouzaiSize * DirectionWin.X ),
              tmpStPoint.Y - ( kouzaiSize * DirectionWin.Y ), tmpStPoint.Z ) ;
          if ( eInsec )
            tmpEdPoint = new XYZ( tmpEdPoint.X + ( kouzaiSize * DirectionWin.X ),
              tmpEdPoint.Y + ( kouzaiSize * DirectionWin.Y ), tmpEdPoint.Z ) ;
        }
      }
      else if ( typeName == "隅火打" ) {
        ClsCornerHiuchiBase chb = new ClsCornerHiuchiBase() ;
        chb.SetParameter( doc, m_baseId ) ;
        if ( ClsRevitUtil.GetInstMojiParameter( doc, m_baseId, "構成" ) == "ダブル" ) {
          m_isHiuchiTateW = true ;
          string tanbuBuzai, dan ;
          if ( m_isTop ) {
            dan = "上段" ;
            tanbuBuzai = chb.m_TanbuParts1 ;
            offset = ( kouzaiSize / 2 ) ;
          }
          else {
            dan = "下段" ;
            tanbuBuzai = chb.m_TanbuParts2 ;
            offset = -( kouzaiSize / 2 ) ;
          }

          deleteBuuzaiIdList.Clear() ;
          deleteBuuzaiIdList = ClsWarituke.GetConnectionBuzaiDouble( m_doc, m_baseId, dan ) ;
          if ( tanbuBuzai == Master.ClsHiuchiCsv.TypeNameFVP ) {
            bool kariFlag = false ;
            foreach ( ElementId id in deleteBuuzaiIdList ) {
              FamilyInstance inst = doc.GetElement( id ) as FamilyInstance ;
              if ( inst.Symbol.FamilyName.Contains( "主材" ) ) {
                kariFlag = true ;
                break ;
              }
            }

            if ( ! kariFlag ) {
              MessageBox.Show( "端部部品自在火打受ピースで仮鋼材が存在しないため割り付けられません" ) ;
              return ;
            }

            //一旦オフセット
            tmpStPoint = new XYZ( tmpStPoint.X, tmpStPoint.Y, tmpStPoint.Z + offset ) ;
            tmpEdPoint = new XYZ( tmpEdPoint.X, tmpEdPoint.Y, tmpEdPoint.Z + offset ) ;
            List<List<XYZ>> notFVPPojntLists = ClsYMSUtil.GetNonObjectBaseLinePoints( doc, tmpStPoint, tmpEdPoint ) ;
            for ( int i = 0 ; i < notFVPPojntLists.Count ; i++ ) {
              if ( i == 0 )
                tmpStPoint = notFVPPojntLists[ i ][ 1 ] ;

              if ( i == notFVPPojntLists.Count - 1 )
                tmpEdPoint = notFVPPojntLists[ i ][ 0 ] ;
            }

            //一旦戻す
            tmpStPoint = new XYZ( tmpStPoint.X, tmpStPoint.Y, tmpStPoint.Z - offset ) ;
            tmpEdPoint = new XYZ( tmpEdPoint.X, tmpEdPoint.Y, tmpEdPoint.Z - offset ) ;
          }
        }
      }
      else if ( typeName == "切梁継ぎ" ) {
        //切梁ベースで交叉してるものを探す　※腹起しベースも接触可能性はあるが位置修正の必要が無いので無視
        List<ElementId> lstHari = YMS.ClsYMSUtil.GetIntersectionKiribariBase( doc, m_baseId ) ;

        LocationCurve lCurve = instBase.Location as LocationCurve ;
        ElementId idA = null ;
        ElementId idB = null ;
        double dSMin = 0 ;
        double dEMin = 0 ;
        if ( lstHari.Count == 1 ) {
          //一本しか取れてない
          Element el = doc.GetElement( lstHari[ 0 ] ) ;
          LocationCurve lCurve2 = el.Location as LocationCurve ;
          XYZ p = ClsRevitUtil.GetIntersection( lCurve.Curve, lCurve2.Curve ) ;

          double dS = tmpStPoint.DistanceTo( p ) ;
          double dE = tmpEdPoint.DistanceTo( p ) ;
          if ( ClsGeo.GEO_GE( dS, dE ) ) {
            idB = lstHari[ 0 ] ;
          }
          else {
            idA = lstHari[ 0 ] ;
          }
        }
        else {
          foreach ( ElementId idtmp in lstHari ) {
            Element el = doc.GetElement( idtmp ) ;
            LocationCurve lCurve2 = el.Location as LocationCurve ;
            XYZ p = ClsRevitUtil.GetIntersection( lCurve.Curve, lCurve2.Curve ) ;

            double dS = tmpStPoint.DistanceTo( p ) ;
            double dE = tmpEdPoint.DistanceTo( p ) ;
            if ( idA == null || dSMin > dS ) {
              idA = idtmp ;
              dSMin = dS ;
            }

            if ( idB == null || dEMin > dE ) {
              idB = idtmp ;
              dEMin = dE ;
            }
          }
        }

        if ( idA != null ) {
          //始点側に接触してる切梁ベース有
          double kiribariHaba = ClsYMSUtil.GetKouzaiHabaFromBase( doc, idA ) / 2 ;
          tmpStPoint = ClsRevitUtil.MoveCoordinates( tmpStPoint, tmpEdPoint,
            ClsRevitUtil.ConvertDoubleGeo2Revit( kiribariHaba ) ) ;
        }

        if ( idB != null ) {
          //始点側に接触してる切梁ベース有
          double kiribariHaba = ClsYMSUtil.GetKouzaiHabaFromBase( doc, idB ) / 2 ;
          //終点側に接触してる切梁ベース有
          tmpEdPoint = ClsRevitUtil.MoveCoordinates( tmpEdPoint, tmpStPoint,
            ClsRevitUtil.ConvertDoubleGeo2Revit( kiribariHaba ) ) ;
        }
      }
      else if ( typeName == "斜梁繋ぎ材" ) {
        var syabariInsecList = ClsSyabariBase.GetIntersectionBase( doc, m_baseId ) ;
        //斜梁繋ぎ材は1点配置で特殊なため
        if ( 1 < syabariInsecList.Count )
          ( tmpStPoint, tmpEdPoint ) =
            ClsSyabariTsunagizaiBase.GetSyabariTsunagizaiPoint( doc, syabariInsecList[ 0 ], syabariInsecList[ 1 ],
              m_baseId ) ;
        m_BaseOffset = ( kouzaiSize / 2 ) ;
        offset = m_BaseOffset ;
        typeName = "斜梁" ;
      }
      else if ( typeName == "斜梁受け材" ) {
        var syabariInsecList = ClsSyabariBase.GetIntersectionBase( doc, m_baseId ) ;

        //一番低い斜梁の選定
        ElementId idSyabari = null ;
        for ( int i = 0 ; i < syabariInsecList.Count ; i++ ) {
          if ( i == 0 ) {
            idSyabari = syabariInsecList[ i ] ;
            continue ;
          }

          var syabariInst = doc.GetElement( idSyabari ) as FamilyInstance ;
          var inster = doc.GetElement( syabariInsecList[ i ] ) as FamilyInstance ;
          if ( syabariInst.HandOrientation.Z > inster.HandOrientation.Z )
            idSyabari = syabariInsecList[ i ] ;
        }

        if ( idSyabari != null )
          ( tmpStPoint, tmpEdPoint ) = ClsSyabariUkeBase.GetSyabariUkePoint( doc, idSyabari, m_baseId,
            Master.ClsYamadomeCsv.GetWidth( steelSizeOrign ) ) ;

        m_BaseOffset = -( kouzaiSize / 2 ) ;
        offset = m_BaseOffset ;
        typeName = "斜梁" ;
      }

      // 仮鋼材を削除
      using ( Transaction t = new Transaction( m_doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        t.Start() ;
        ClsRevitUtil.Delete( m_doc, deleteBuuzaiIdList ) ;
        t.Commit() ;
      }

      //端部の部材をよける処理※中間に存在する部材はよけない
      tmpStPoint = new XYZ( tmpStPoint.X, tmpStPoint.Y, tmpStPoint.Z + offset ) ;
      tmpEdPoint = new XYZ( tmpEdPoint.X, tmpEdPoint.Y, tmpEdPoint.Z + offset ) ;
      //仮鋼材の上のファミリが乗っていないポイントList
      List<List<XYZ>> notPojntLists = ClsYMSUtil.GetNonObjectBaseLinePoints( doc, tmpStPoint, tmpEdPoint ) ;
      XYZ first = tmpStPoint, last = tmpEdPoint ;
      for ( int i = 0 ; i < notPojntLists.Count ; i++ ) {
        if ( i == 0 )
          first = notPojntLists[ i ][ 0 ] ;

        if ( i == notPojntLists.Count - 1 )
          last = notPojntLists[ i ][ 1 ] ;
      }

      tmpStPoint = first ;
      tmpEdPoint = last ;

      XYZ dir = ( tmpEdPoint - tmpStPoint ).Normalize() ;

      // ジャッキの吸収長さの算出のためにベースの長さを保持
      double baseOriginLength = tmpStPoint.DistanceTo( tmpEdPoint ) ;
      double baseOriginLength_Real = ClsRevitUtil.CovertFromAPI( baseOriginLength ) ;
      baseOriginLength_Real = Math.Round( baseOriginLength_Real, MidpointRounding.AwayFromZero ) ;
      baseOriginLength_Real = Math.Ceiling( baseOriginLength_Real / 10 ) * 10 ;
      baseOriginLength = ClsRevitUtil.ConvertDoubleGeo2Revit( baseOriginLength_Real ) ;

      //2D用2024/10/04時点上手く機能しない
      //notPojntLists = ClsYMSUtil.GetNonObjectBaseLinePoints2D(doc, tmpStPoint, tmpEdPoint); 
      //下記で衝突判定を行うかユーザーに選ばせる処理を追加する可能性あり
      notPojntLists = ClsYMSUtil.GetNonObjectBaseLinePoints( doc, tmpStPoint, tmpEdPoint ) ;
      bool exitOuterLoop = false ;
      foreach ( List<XYZ> notPojntList in notPojntLists ) {
        if ( exitOuterLoop ) // フラグが立っていたらforeachループを終了
        {
          break ;
        }

        tmpStPoint = notPojntList[ 0 ] ;
        tmpEdPoint = notPojntList[ 1 ] ;

        m_InsertPoint = tmpStPoint ;
        m_EndPoint = tmpEdPoint ;

        // 割付処理
        double buzaiLength = 0.0 ;
        string steelSize = string.Empty ;
        while ( true ) {
          // スケールの表示
          double maxLength = 0 ;
          double minLength = 0 ;
          switch ( m_WaritsukeMode ) {
            case DlgWaritsuke.WaritsukeMode.Normal :
              maxLength = 7000.0 ;
              minLength = 1000.0 ;
              if ( steelSizeOrign == "60SMH" || steelSizeOrign == "80SMH" ) {
                steelSizeOrign = "40HA" ;
              }

              steelSize = steelSizeOrign.Replace( "SMH", "HA" ) ;
              break ;
            case DlgWaritsuke.WaritsukeMode.SMH :
              maxLength = 7000.0 ;
              minLength = 1000.0 ;
              if ( steelSizeOrign == "60SMH" || steelSizeOrign == "80SMH" ) {
                steelSizeOrign = "40HA" ;
              }

              steelSize = steelSizeOrign.Replace( "HA", "SMH" ) ;
              break ;
            case DlgWaritsuke.WaritsukeMode.TwinBeam :
              maxLength = 9000.0 ;
              minLength = 4000.0 ;
              steelSize = "60SMH" ;
              break ;
          }

          //自動モードかつベースダブルのとき終点位置を設定長さまでとする※間にその他の部材があり設定長さ以下でしか割り付けられないか判定
          if ( doubleMaxLength != 0.0 && dlgWaritsukeMode.DialogResult == DialogResult.No &&
               doubleMaxLength < Line.CreateBound( m_InsertPoint, m_EndPoint ).Length ) {
            m_EndPoint = m_InsertPoint + doubleMaxLength * dir ;
          }

          List<ElementId> scaleIds = new List<ElementId>() ;
          //メモリ表示位置をベース上にするためオフセット値を戻す
          scaleIds = ClsWarituke.CreateScale( uidoc,
            new XYZ( m_InsertPoint.X, m_InsertPoint.Y, m_InsertPoint.Z - offset ),
            new XYZ( m_EndPoint.X, m_EndPoint.Y, m_EndPoint.Z - offset ), WaritukeDist.warituke100mm, maxLength,
            minLength, cross ) ;

          // 割付長さを補正
          double remainingLength_Real = ClsRevitUtil.CovertFromAPI( m_InsertPoint.DistanceTo( m_EndPoint ) ) ;
          remainingLength_Real = Math.Round( remainingLength_Real, MidpointRounding.AwayFromZero ) ;
          remainingLength_Real = Math.Ceiling( remainingLength_Real / 10 ) * 10 ;
          if ( remainingLength_Real < 100 ) {
            break ;
          }

          double remainingLength = ClsRevitUtil.ConvertDoubleGeo2Revit( remainingLength_Real ) ;

          // 割付ダイアログの表示
          DlgWaritsuke dlgWaritsuke = new DlgWaritsuke( doc, instBase, m_WaritsukeMode, m_isTop, m_DialogPoint ) ;
          dlgWaritsuke.Show() ;
          dlgWaritsuke.SetRemainingLength( ClsRevitUtil.CovertFromAPI( remainingLength ) ) ;

          // ダイアログが閉じられるまで待機
          while ( dlgWaritsuke.Visible ) {
            uidoc.RefreshActiveView() ;
            System.Windows.Forms.Application.DoEvents() ;
          }

          // ダイアログが閉じられた後でリザルトを取得
          DialogResult result = dlgWaritsuke.DialogResult ;
          if ( result == DialogResult.Cancel ) {
            if ( notPojntLists.Count > 1 ) {
              DialogResult res = MessageBox.Show( "次の割付に移行しますか？", "確認", MessageBoxButtons.YesNo ) ;
              if ( res == DialogResult.No ) {
                // ダイアログを閉じる
                dlgWaritsuke.Dispose() ;

                // スケールの削除
                DeleteScale( doc, scaleIds ) ;

                exitOuterLoop = true ;
              }
            }

            DeleteScale( doc, scaleIds ) ;
            break ;
          }

          // ダイアログの表示位置を記憶しておく
          m_DialogPoint = dlgWaritsuke.m_LastDialogLocation ;

          // 割付モードの取得
          m_WaritsukeMode = dlgWaritsuke.m_WaritsukeMode ;
          switch ( m_WaritsukeMode ) {
            case DlgWaritsuke.WaritsukeMode.Normal :
              steelSize = steelSizeOrign.Replace( "SMH", "HA" ) ;
              break ;
            case DlgWaritsuke.WaritsukeMode.SMH :
              steelSize = steelSizeOrign.Replace( "HA", "SMH" ) ;
              break ;
            case DlgWaritsuke.WaritsukeMode.TwinBeam :
              steelSize = "60SMH" ;
              break ;
          }

          // 各種割付処理
          try {
            switch ( dlgWaritsuke.m_Command ) {
              case WaritukeCommand.CreateSyuzai :

                // 始点側の火打ブロックと主材を接続するカバープレートの判定
                bool searchFlag = false ;
                XYZ tmpSp = new XYZ() ;
                XYZ tmpEp = new XYZ() ;
                if ( m_createdIds.Count == 0 && typeName == "切梁" ) {
                  searchFlag = true ;
                  tmpSp = m_InsertPoint ;
                  tmpEp = m_EndPoint ;
                }

                // ファミリの取得処理
                string familyPath = Master.ClsYamadomeCsv.GetFamilyPath( steelSize, dlgWaritsuke.m_Length ) ;
                string symbolName = ClsRevitUtil.GetFamilyName( familyPath ) ;
                if ( ! ClsRevitUtil.LoadFamilyData( doc, familyPath, out Family family ) ) {
                  MessageBox.Show( "ファミリの取得に失敗しました" ) ;
                  DeleteScale( doc, scaleIds ) ;
                  break ;
                }

                FamilySymbol kouzaiSym = ClsRevitUtil.GetFamilySymbol( doc, symbolName, typeName ) ;

                // カバープレートの作図
                if ( typeName != "切梁継ぎ" )
                  WaritsukeCoverPlate( doc, instBase, vecCcoverPlateoffset, steelSize, kouzaiSize, typeName ) ;

                // ファミリの配置処理
                WarituskeSyuzai( doc, instBase, kouzaiSym ) ;

                // 始点側の火打ブロックと主材を接続するカバープレートの処理
                if ( searchFlag ) {
                  AddCoverPlateWithHiuchiBlockSP( doc, instBase, typeName, kouzaiSize, vecCcoverPlateoffset, steelSize,
                    tmpSp, tmpEp ) ;
                }

                buzaiLength = dlgWaritsuke.m_Length * 1000 ;

                break ;
              case WaritukeCommand.CreateMegaBeam :
                WaritsukeMegaBeam( doc, typeName, instBase.Host.Id, out buzaiLength ) ;
                break ;
              case WaritukeCommand.CreateHojoPiece :
                WaritsukeHojoPiece( doc, typeName, dlgWaritsuke.m_Size, instBase.Host.Id, out buzaiLength ) ;
                break ;
              case WaritukeCommand.CreateJack1 :
                WaritsukeJack( doc, typeName, instBase.Host.Id, baseOriginLength, out buzaiLength, true,
                  dlgWaritsuke.m_UseJackCover ) ;
                break ;
              case WaritukeCommand.CreateJack2 :
                WaritsukeJack( doc, typeName, instBase.Host.Id, baseOriginLength, out buzaiLength, false,
                  dlgWaritsuke.m_UseJackCover ) ;
                break ;
              case WaritukeCommand.CreateKirikaePieceS :
                WaritsukeKirikaePiece( doc, typeName, instBase.Host.Id, false, out buzaiLength ) ;
                break ;
              case WaritukeCommand.CreateKirikaePieceE :
                WaritsukeKirikaePiece( doc, typeName, instBase.Host.Id, true, out buzaiLength ) ;
                break ;
              case WaritukeCommand.SwitchNormal :
              case WaritukeCommand.SwitchSMH :
              case WaritukeCommand.SwitchTwinBeam :
                buzaiLength = 0.0 ;
                break ;
              case WaritukeCommand.CommandUndo :
                WaritukeUndo( doc ) ;
                buzaiLength = 0.0 ;
                break ;
              case WaritukeCommand.CommandEnd :
                if ( notPojntLists.Count > 1 ) {
                  DialogResult res = MessageBox.Show( "次の割付に移行しますか？", "確認", MessageBoxButtons.YesNo ) ;
                  if ( res == DialogResult.No ) {
                    // ダイアログを閉じる
                    dlgWaritsuke.Dispose() ;

                    // スケールの削除
                    DeleteScale( doc, scaleIds ) ;

                    exitOuterLoop = true ;
                  }
                }

                break ;
              default :
                break ;
            }

            remainingLength_Real = ClsRevitUtil.CovertFromAPI( m_InsertPoint.DistanceTo( m_EndPoint ) ) ;
            remainingLength_Real = Math.Round( remainingLength_Real, MidpointRounding.AwayFromZero ) ;
            remainingLength_Real = Math.Ceiling( remainingLength_Real / 10 ) * 10 ;
            buzaiLength = ClsGeo.FloorAtDigitAdjust( 0, buzaiLength ) ;
            if ( ClsGeo.GEO_GT( buzaiLength, remainingLength_Real ) ) {
              MessageBox.Show( "割付の最大長を超えました" ) ;
              var tmpP = m_InsertPoint ;
              WaritukeUndo( doc ) ;
              m_InsertPoint = tmpP ;
              continue ;
            }

            m_InsertPoint = new XYZ( m_InsertPoint.X + ( ClsRevitUtil.CovertToAPI( buzaiLength ) * dir.X ),
              m_InsertPoint.Y + ( ClsRevitUtil.CovertToAPI( buzaiLength ) * dir.Y ),
              m_InsertPoint.Z + ( ClsRevitUtil.CovertToAPI( buzaiLength ) * dir.Z ) ) ;

            // 最終点まで到達したかの判定
            if ( ClsGeo.GEO_EQ( buzaiLength, remainingLength_Real ) ) {
              if ( notPojntList == notPojntLists.Last() ) {
                MessageBox.Show( "割付の最大長まで到達しました" ) ;
              }

              if ( typeName == "切梁" ) {
                AddCoverPlateWithHiuchiBlockEP( doc, instBase, typeName, kouzaiSize, vecCcoverPlateoffset, steelSize ) ;
              }

              DeleteScale( doc, scaleIds ) ;
              break ;
            }
          }
          catch ( Exception e ) {
            MessageBox.Show( e.Message ) ;
          }
          finally {
            // ダイアログを閉じる
            dlgWaritsuke.Dispose() ;

            // スケールの削除
            DeleteScale( doc, scaleIds ) ;
          }

          // ダイアログを閉じる
          dlgWaritsuke.Dispose() ;
        }
      }
    }


    private static void DeleteScale( Document doc, List<ElementId> scaleIds )
    {
      // スケールの削除
      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        t.Start() ;
        ClsRevitUtil.Delete( doc, scaleIds ) ;
        t.Commit() ;
      }
    }

    private static void WaritsukeMegaBeam( Document doc, string baseName, ElementId levelId, out double buzaiLength )
    {
      ClsMegaBeam clsMegaBeam = new ClsMegaBeam() ;
      clsMegaBeam.CreateMegaBeam( doc, m_InsertPoint, m_EndPoint, levelId, m_vecMegabeam, m_BaseOffset ) ;
      buzaiLength = 9000.0 ;

      m_createdIds.Add( clsMegaBeam.m_CreatedId ) ;
      m_createdXYZs.Add( m_InsertPoint ) ;
    }

    private static void WaritsukeHojoPiece( Document doc, string baseName, string size, ElementId levelId,
      out double buzaiLength )
    {
      ClsHojoPiece clsHojoPiece = new ClsHojoPiece() ;
      clsHojoPiece.CreateHojoPiece( doc, m_baseId, m_InsertPoint, m_EndPoint, levelId, size, baseName, out buzaiLength,
        m_BaseOffset ) ;

      // 手動配置のフラグを付与
      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        t.Start() ;
        ClsWarituke.SetWarituke( doc, clsHojoPiece.m_CreatedId ) ;
        t.Commit() ;
      }

      if ( m_isHaraokoshiTateW ) {
        string sSize = ClsRevitUtil.GetInstMojiParameter( doc, m_baseId, "鋼材サイズ" ).Replace( "HA", "" ) ;
        double dSize = ClsRevitUtil.CovertToAPI( ClsCommonUtils.ChangeStrToDbl( sSize ) * 10 ) ;

        using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
          t.Start() ;
          // 縦ダブルの下段に移動する
          ClsRevitUtil.SetParameter( doc, clsHojoPiece.m_CreatedId, "基準レベルからの高さ",
            -( dSize / 2 ) - ( m_VerticalGap / 2 ) ) ;
          t.Commit() ;
        }
      }

      m_createdIds.Add( clsHojoPiece.m_CreatedId ) ;
      m_createdXYZs.Add( m_InsertPoint ) ;

      if ( m_isHaraokoshiTateW ) {
        // 配置したファミリをコピーして上段にする
        CreateHaraokoshiTateDouble( doc, m_createdIds.Last() ) ;
      }

      if ( m_isHiuchiTateW ) {
        MoveHiuchiTateDouble( doc, m_createdIds.Last() ) ;
      }
    }

    private static void WaritsukeKirikaePiece( Document doc, string baseName, ElementId levelId, bool isReverse,
      out double buzaiLength )
    {
      ClsKirikaePiece clsKirikaePiece = new ClsKirikaePiece() ;
      clsKirikaePiece.CreateKirikaePiece( doc, m_InsertPoint, m_EndPoint, levelId, baseName, isReverse, m_BaseOffset ) ;
      buzaiLength = 500.0 ;

      // 手動配置のフラグを付与
      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        t.Start() ;
        ClsWarituke.SetWarituke( doc, clsKirikaePiece.m_CreatedId ) ;
        t.Commit() ;
      }

      m_createdIds.Add( clsKirikaePiece.m_CreatedId ) ;
      m_createdXYZs.Add( m_InsertPoint ) ;

      if ( m_isHiuchiTateW ) {
        MoveHiuchiTateDouble( doc, m_createdIds.Last() ) ;
      }
    }

    private static void WaritsukeJack( Document doc, string baseName, ElementId levelId, double remainingLength,
      out double buzaiLength, bool isType1, bool useJackCover )
    {
      ClsJack jack = new ClsJack() ;

      // ジャッキタイプ
      if ( isType1 ) {
        jack.m_JackType = ClsRevitUtil.GetInstMojiParameter( doc, m_baseId, "ジャッキタイプ(1)" ) ;
      }
      else {
        jack.m_JackType = ClsRevitUtil.GetInstMojiParameter( doc, m_baseId, "ジャッキタイプ(2)" ) ;
      }

      // 主材サイズ
      string kouzaiSize = ClsRevitUtil.GetParameter( doc, m_baseId, "鋼材サイズ" ) ;
      jack.m_SyuzaiSize = kouzaiSize ;

      // ジャッキの店所
      string jackSize = Master.ClsJackCsv.GetJackSize( jack.m_JackType, kouzaiSize ) ;
      string strTypeNameType = Master.ClsJackCsv.GetTypeNameType( jackSize ) ; //Atypeなど
      if ( strTypeNameType != "Atype" ) {
        jack.m_JackTensyo = "九州" ;
      }

      // ジャッキカバーの有無
      jack.m_UseJackCover = useJackCover ;

      // ジャッキの配置処理
      jack.CreateJack( doc, m_baseId, m_InsertPoint, m_EndPoint, m_InsertPoint, remainingLength: remainingLength,
        flagName: "割付" ) ;
      buzaiLength = ClsJack.GetTotalJack( doc, jack.m_CreatedJackId ) ;

      if ( jack.m_CreatedJackId != null ) {
        m_createdIds.Add( jack.m_CreatedJackId ) ;
        m_createdXYZs.Add( m_InsertPoint ) ;
      }

      if ( jack.m_CreatedJackCoverId != null ) {
        m_createdIds.Add( jack.m_CreatedJackCoverId ) ;
        m_createdXYZs.Add( m_InsertPoint ) ;
      }

      if ( m_isHiuchiTateW ) {
        MoveHiuchiTateDouble( doc, m_createdIds.Last() ) ;
      }
    }

    private static void WarituskeSyuzai( Document doc, FamilyInstance instBase, FamilySymbol kouzaiSym )
    {
      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        t.Start() ;

        ElementId createdId = null ;
        XYZ vec = ( m_EndPoint - m_InsertPoint ).Normalize() ;
        if ( instBase.Host is ReferencePlane referencePlane ) {
          createdId = ClsRevitUtil.Create( doc, m_InsertPoint, vec, referencePlane, kouzaiSym ) ;
          ClsRevitUtil.SetParameter( doc, createdId, "集計レベル",
            ClsRevitUtil.GetParameterElementId( doc, instBase.Id, "集計レベル" ) ) ;
          //m_BaseOffset += ClsRevitUtil.GetParameterDouble(doc, createdId, "ホストからのオフセット");
          //ClsRevitUtil.SetParameter(doc, createdId, "ホストからのオフセット", m_BaseOffset);
        }
        else {
          createdId = ClsRevitUtil.Create( doc, m_InsertPoint, instBase.Host.Id, kouzaiSym ) ;

          // シンボルを回転
          Line rotationAxis = Line.CreateBound( m_InsertPoint, m_InsertPoint + XYZ.BasisZ ) ; // Z軸周りに回転

          double radians = XYZ.BasisX.AngleOnPlaneTo( vec, XYZ.BasisZ ) ;
          ElementTransformUtils.RotateElement( doc, createdId, rotationAxis, radians ) ;
          if ( ! ClsRevitUtil.SetParameter( doc, createdId, "基準レベルからの高さ", m_BaseOffset ) )
            ClsRevitUtil.SetParameter( doc, createdId, "ホストからのオフセット", m_BaseOffset ) ;
        }


        if ( m_isHaraokoshiTateW ) {
          string sSize = ClsRevitUtil.GetInstMojiParameter( doc, m_baseId, "鋼材サイズ" ).Replace( "HA", "" ) ;
          double dSize = ClsRevitUtil.CovertToAPI( ClsCommonUtils.ChangeStrToDbl( sSize ) * 10 ) ;

          // 縦ダブルの下段に移動する
          ClsRevitUtil.SetParameter( doc, createdId, "基準レベルからの高さ",
            -( dSize / 2 ) - ( m_VerticalGap / 2 ) ) ; //m_BaseOffset縦ダブルのときは上下段オフセットは不要
        }

        // ベースIDを付与
        SetBaseId( doc, createdId, m_baseId ) ;

        // 掘削側か？
        SetIsKussaku( doc, createdId, m_isKussaku ) ;

        t.Commit() ;

        m_createdIds.Add( createdId ) ;
        m_createdXYZs.Add( m_InsertPoint ) ;
      }

      if ( m_isHaraokoshiTateW ) {
        // 配置したファミリをコピーして上段にする
        CreateHaraokoshiTateDouble( doc, m_createdIds.Last() ) ;
      }

      if ( m_isHiuchiTateW ) {
        MoveHiuchiTateDouble( doc, m_createdIds.Last() ) ;
      }
    }

    private static void WaritsukeCoverPlate( Document doc, FamilyInstance instBase, XYZ vecCcoverPlateoffset,
      string steelSize, double kouzaiSize, string typeName )
    {
      if ( m_createdIds.Count > 0 && m_createdXYZs.Count > 0 ) {
        FamilyInstance inst = m_doc.GetElement( m_createdIds.Last() ) as FamilyInstance ;
        if ( inst == null ) {
          return ;
        }

        if ( inst.Symbol.FamilyName.Contains( "主材" ) ) {
          if ( typeName.Contains( "斜梁" ) )
            typeName = "切梁" ; //斜梁の場合は切梁にタイプを変更する
          ElementId cpKussaku = null ;
          ElementId cpKabe = null ;

          ClsCoverPlate clsCoverPlate = new ClsCoverPlate() ;

          // 掘削側のカバープレートを作成
          clsCoverPlate.CreateCoverPlate( doc, m_InsertPoint, m_EndPoint, instBase, steelSize, typeName, true,
            vecCcoverPlateoffset, m_BaseOffset ) ;
          cpKussaku = clsCoverPlate.m_CreatedId ;

          // 壁側のカバープレートを作成
          clsCoverPlate.CreateCoverPlate( doc, m_InsertPoint, m_EndPoint, instBase, steelSize, typeName, false,
            vecCcoverPlateoffset, m_BaseOffset ) ;
          cpKabe = clsCoverPlate.m_CreatedId ;

          // 掘削側のカバープレートと端部部品たちの衝突チェックを行う
          if ( instBase.Symbol.FamilyName.Contains( "腹起" ) ) {
            List<ElementId> allInsIds = ClsKariKouzai.GetTanbubuhin( doc ) ;
            List<ElementId> hitIds = new List<ElementId>() ;
            if ( ClsRevitUtil.CheckCollision2( doc, cpKussaku, allInsIds, ref hitIds ) ) {
              DialogResult result = MessageBox.Show( "カバープレートが接触していますが、作成しますか？", "確認", MessageBoxButtons.YesNo,
                MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2 ) ;
              if ( result != DialogResult.Yes ) {
                using ( Transaction transaction = new Transaction( doc, "Delete Family" ) ) {
                  transaction.Start() ;
                  ICollection<ElementId> deletedFamilyIdss = doc.Delete( cpKussaku ) ;
                  transaction.Commit() ;
                }

                cpKussaku = null ;
              }
            }
          }

          // 横ダブルの場合はお互いに接合する面のカバープレートは不要
          ICollection<ElementId> deletedFamilyIds = null ;
          if ( ClsRevitUtil.GetInstMojiParameter( doc, m_baseId, "横本数" ) == "ダブル" ) {
            using ( Transaction transaction = new Transaction( doc, "Delete Family" ) ) {
              transaction.Start() ;
              if ( m_isKussaku ) {
                deletedFamilyIds = doc.Delete( cpKabe ) ;
                cpKabe = null ;
              }
              else {
                deletedFamilyIds = doc.Delete( cpKussaku ) ;
                cpKussaku = null ;
              }

              transaction.Commit() ;
            }
          }

          // 腹起縦ダブルの場合
          if ( m_isHaraokoshiTateW ) {
            if ( cpKussaku != null ) // || !deletedFamilyIds.Contains(cpKussaku))
            {
              // 縦ダブルの下段に移動する
              using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
                t.Start() ;
                ClsRevitUtil.SetParameter( doc, cpKussaku, "基準レベルからの高さ",
                  -( kouzaiSize / 2 ) - ( m_VerticalGap / 2 ) ) ; //m_BaseOffset
                t.Commit() ;
              }

              // 配置したファミリをコピーして上段にする
              CreateHaraokoshiTateDouble( doc, cpKussaku ) ;
            }

            if ( cpKabe != null ) // || !deletedFamilyIds.Contains(cpKabe))
            {
              using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
                t.Start() ;
                ClsRevitUtil.SetParameter( doc, cpKabe, "基準レベルからの高さ",
                  -( kouzaiSize / 2 ) - ( m_VerticalGap / 2 ) ) ; //m_BaseOffset
                t.Commit() ;
              }

              // 配置したファミリをコピーして上段にする
              CreateHaraokoshiTateDouble( doc, cpKabe ) ;
            }
          }

          // 火打縦ダブルの場合
          if ( m_isHiuchiTateW ) {
            MoveHiuchiTateDouble( doc, cpKussaku ) ;
            MoveHiuchiTateDouble( doc, cpKabe ) ;
          }

          if ( cpKussaku != null ) {
            m_createdIds.Add( cpKussaku ) ;
            m_createdXYZs.Add( m_InsertPoint ) ;
          }

          if ( cpKabe != null ) {
            m_createdIds.Add( cpKabe ) ;
            m_createdXYZs.Add( m_InsertPoint ) ;
          }
        }
      }
    }

    private static void WaritukeUndo( Document doc )
    {
      if ( m_createdIds.Count > 0 && m_createdXYZs.Count > 0 ) {
        using ( Transaction transaction = new Transaction( doc, "Delete Family" ) ) {
          transaction.Start() ;

          // ファミリの削除処理
          ICollection<ElementId> deletedFamilyIds = doc.Delete( m_createdIds.Last() ) ;
          if ( m_createdIds_HaraokoshiTateW.Count > 0 ) {
            doc.Delete( m_createdIds_HaraokoshiTateW.Last() ) ;
            m_createdIds_HaraokoshiTateW.RemoveAt( m_createdIds_HaraokoshiTateW.Count - 1 ) ;
          }

          transaction.Commit() ;

          m_InsertPoint = m_createdXYZs.Last() ;

          m_createdIds.RemoveAt( m_createdIds.Count - 1 ) ;
          m_createdXYZs.RemoveAt( m_createdXYZs.Count - 1 ) ;
        }

        for ( int i = 0 ; i < 2 ; i++ ) {
          if ( m_createdIds.Count > 0 && m_createdXYZs.Count > 0 ) {
            FamilyInstance inst = doc.GetElement( m_createdIds.Last() ) as FamilyInstance ;
            if ( inst != null ) {
              if ( inst.Symbol.FamilyName.Contains( "カバープレート" ) ) {
                using ( Transaction transaction = new Transaction( doc, "Delete Family" ) ) {
                  transaction.Start() ;

                  // ファミリの削除処理
                  ICollection<ElementId> deletedFamilyIds = doc.Delete( m_createdIds.Last() ) ;
                  if ( m_createdIds_HaraokoshiTateW.Count > 0 ) {
                    doc.Delete( m_createdIds_HaraokoshiTateW.Last() ) ;
                    m_createdIds_HaraokoshiTateW.RemoveAt( m_createdIds_HaraokoshiTateW.Count - 1 ) ;
                  }

                  transaction.Commit() ;

                  m_InsertPoint = m_createdXYZs.Last() ;

                  m_createdIds.RemoveAt( m_createdIds.Count - 1 ) ;
                  m_createdXYZs.RemoveAt( m_createdXYZs.Count - 1 ) ;
                }
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// 腹起の縦ダブルの部材を作図する
    /// </summary>
    /// <param name="doc"></param>
    private static void CreateHaraokoshiTateDouble( Document doc, ElementId createdId )
    {
      string sVerticalGap = ClsRevitUtil.GetInstMojiParameter( doc, m_baseId, "縦方向の隙間" ) ;
      m_VerticalGap = ClsRevitUtil.CovertToAPI( ClsCommonUtils.ChangeStrToDbl( sVerticalGap ) ) ;

      string steelSize = ClsRevitUtil.GetInstMojiParameter( doc, m_baseId, "鋼材サイズ" ).Replace( "HA", "" ) ;
      double kouzaiSize = ClsRevitUtil.CovertToAPI( ClsCommonUtils.ChangeStrToDbl( steelSize ) * 10 ) ;

      using ( Transaction t = new Transaction( doc, "Create and Move Second Parts" ) ) {
        t.Start() ;

        ICollection<ElementId> copiedElementIds = ElementTransformUtils.CopyElement( doc, createdId, XYZ.Zero ) ;
        ElementId copiedElementId = copiedElementIds.FirstOrDefault() ;
        m_createdIds_HaraokoshiTateW.Add( copiedElementId ) ;

        ClsRevitUtil.SetParameter( doc, copiedElementId, "基準レベルからの高さ", ( kouzaiSize / 2 ) + ( m_VerticalGap / 2 ) ) ;

        t.Commit() ;
      }
    }

    /// <summary>
    /// 火打が縦ダブルの場合に移動する
    /// </summary>
    /// <param name="doc"></param>
    /// <param name=""></param>
    /// <param name="createdId"></param>
    private static void MoveHiuchiTateDouble( Document doc, ElementId createdId )
    {
      string kouzaiSize = ClsRevitUtil.GetInstMojiParameter( doc, m_baseId, "鋼材サイズ" ) ;
      double dSize = ClsRevitUtil.CovertToAPI( Master.ClsYamadomeCsv.GetWidth( kouzaiSize ) ) ;

      double offset = ClsRevitUtil.GetParameterDouble( doc, createdId, "基準レベルからの高さ" ) ;

      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        t.Start() ;

        if ( m_isTop ) {
          ClsRevitUtil.SetParameter( doc, createdId, "基準レベルからの高さ", offset + ( dSize / 2 ) ) ;
        }
        else {
          ClsRevitUtil.SetParameter( doc, createdId, "基準レベルからの高さ", offset - ( dSize / 2 ) ) ;
        }

        t.Commit() ;
      }
    }

    /// <summary>
    /// 指定点を指定の始終点に収まるように調整する
    /// </summary>
    /// <param name="uidoc"></param>
    /// <param name="tmpStPoint"></param>
    /// <param name="tmpEdPoint"></param>
    /// <param name="selectPointMess"></param>
    /// <returns></returns>
    public static XYZ GetBetweenBasePoint( UIDocument uidoc, XYZ tmpStPoint, XYZ tmpEdPoint, string selectPointMess )
    {
      Selection selection = uidoc.Selection ;
      Curve cvBase = Line.CreateBound( tmpStPoint, tmpEdPoint ) ;
      XYZ dir = Line.CreateBound( tmpStPoint, tmpEdPoint ).Direction ;

      XYZ pickPoint = selection.PickPoint( "ベースの" + selectPointMess + "を指定してください" ) ;

      //Z軸の統一
      pickPoint = new XYZ( pickPoint.X, pickPoint.Y, tmpStPoint.Z ) ;

      if ( ClsGeo.GEO_EQ( tmpStPoint, pickPoint ) )
        return tmpStPoint ;
      if ( ClsGeo.GEO_EQ( tmpEdPoint, pickPoint ) )
        return tmpEdPoint ;

      //基準線からどちら周りで90度回転させるかの
      double harfPI = Math.PI / 2 ;
      XYZ picDir = Line.CreateBound( tmpStPoint, pickPoint ).Direction ;
      if ( ClsGeo.IsLeft( dir, picDir ) )
        harfPI = -harfPI ;

      //指定点から基準線への垂線ベクトルの作成
      picDir = new XYZ( dir.X * Math.Cos( harfPI ) - dir.Y * Math.Sin( harfPI ),
        dir.X * Math.Sin( harfPI ) + dir.Y * Math.Cos( harfPI ), dir.Z ) ;

      //指定点から無限に伸ばした直線
      XYZ perpendicPoint = new XYZ( pickPoint.X + ( ClsRevitUtil.CovertToAPI( int.MaxValue ) * picDir.X ),
        pickPoint.Y + ( ClsRevitUtil.CovertToAPI( int.MaxValue ) * picDir.Y ), pickPoint.Z ) ;
      Curve perpendicCv = Line.CreateBound( pickPoint, perpendicPoint ) ;

      //指定点から基準線に下ろした垂線との交点を取得
      XYZ insecPoint = ClsRevitUtil.GetIntersection( cvBase, perpendicCv ) ;
      if ( insecPoint != null ) {
        pickPoint = insecPoint ;
      }
      else {
        if ( ClsRevitUtil.CheckNearGetEndPoint( cvBase, pickPoint ) )
          pickPoint = tmpStPoint ;
        else
          pickPoint = tmpEdPoint ;
      }

      return pickPoint ;
    }

    /// <summary>
    /// 対象の主材が火打ブロックと干渉しているか
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="syuzaiId"></param>
    /// <returns></returns>
    private static bool HitWithHiuchiBlock( Document doc, ElementId syuzaiId )
    {
      List<ElementId> hiuchiBlockIds = ClsRevitUtil.GetSelectCreatedFamilyInstanceList( doc, "火打ブロック", true ) ;
      if ( hiuchiBlockIds.Count() == 0 ) {
        return false ;
      }

      List<ElementId> ids = ClsRevitUtil.GetIntersectFamilys( doc, syuzaiId, 0.1, null, hiuchiBlockIds ) ;
      foreach ( var item in ids ) {
        FamilyInstance inst = m_doc.GetElement( item ) as FamilyInstance ;
        if ( inst.Symbol.FamilyName.Contains( "小火打ブロック" ) ) {
          return false ;
        }
      }

      if ( ids.Count() > 0 ) {
        return true ;
      }

      return false ;
    }

    private static void AddCoverPlateWithHiuchiBlockSP( Document doc, FamilyInstance instBase, string typeName,
      double kouzaiSize, XYZ vecCcoverPlateoffset, string steelSize, XYZ tmpSp, XYZ tmpEp )
    {
      if ( ! HitWithHiuchiBlock( doc, m_createdIds.Last() ) ) {
        return ;
      }

      XYZ tmp1 = m_InsertPoint ;
      XYZ tmp2 = m_EndPoint ;
      m_InsertPoint = tmpSp ;
      m_EndPoint = tmpEp ;

      WaritsukeCoverPlate( doc, instBase, vecCcoverPlateoffset, steelSize, kouzaiSize, typeName ) ;

      for ( int i = 0 ; i < 2 ; i++ ) {
        // 最後の要素を一時的に保存
        var lastId = m_createdIds[ m_createdIds.Count - 1 ] ;

        // 最後の要素を削除
        m_createdIds.RemoveAt( m_createdIds.Count - 1 ) ;

        // 最初の位置に要素を挿入
        m_createdIds.Insert( 0, lastId ) ;
      }

      for ( int i = 0 ; i < 2 ; i++ ) {
        // 最後の要素を一時的に保存
        var lastXYZ = m_createdXYZs[ m_createdXYZs.Count - 1 ] ;

        // 最後の要素を削除
        m_createdXYZs.RemoveAt( m_createdXYZs.Count - 1 ) ;

        // 最初の位置に要素を挿入
        m_createdXYZs.Insert( 0, lastXYZ ) ;
      }

      m_InsertPoint = tmp1 ;
      m_EndPoint = tmp2 ;
    }

    private static void AddCoverPlateWithHiuchiBlockEP( Document doc, FamilyInstance instBase, string typeName,
      double kouzaiSize, XYZ vecCcoverPlateoffset, string steelSize )
    {
      if ( ! HitWithHiuchiBlock( doc, m_createdIds.Last() ) ) {
        return ;
      }

      m_EndPoint = m_createdXYZs.Last() ;
      WaritsukeCoverPlate( doc, instBase, vecCcoverPlateoffset, steelSize, kouzaiSize, typeName ) ;
    }

    /// <summary>
    /// 対象のファミリにベースIdをカスタムデータとして持たせる
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="id">ファミリ</param>
    /// <param name="baseId">ベースのID</param>
    /// <returns></returns>
    private static bool SetBaseId( Document doc, ElementId id, ElementId baseId )
    {
      int iBaseId = baseId.IntegerValue ;
      return ClsRevitUtil.CustomDataSet<int>( doc, id, "BaseId", iBaseId ) ;
    }

    /// <summary>
    /// 対象のファミリからベースIdのカスタムデータを取得する
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="id">ファミリ</param>
    /// <returns>ベースのID</returns>
    public static ElementId GetBaseId( Document doc, ElementId id )
    {
      ClsRevitUtil.CustomDataGet<int>( doc, id, "BaseId", out int iBaseId ) ;

      ElementId e = new ElementId( iBaseId ) ;
      return e ;
    }

    private static bool SetIsKussaku( Document doc, ElementId id, bool isKussaku )
    {
      return ClsRevitUtil.CustomDataSet<bool>( doc, id, "掘削側", isKussaku ) ;
    }

    public static bool GetIsKussaku( Document doc, ElementId id )
    {
      ClsRevitUtil.CustomDataGet<bool>( doc, id, "掘削側", out bool isKussaku ) ;
      return isKussaku ;
    }

    private static void CreateDebugLine( Document doc, XYZ pt, Curve line )
    {
      // デバッグ用の線分を表示
      using ( Transaction transaction = new Transaction( doc, "Create Model Line" ) ) {
        transaction.Start() ;

        // モデル線分を作成
        ElementId levelID = ClsRevitUtil.GetLevelID( doc, ClsKabeShin.GL ) ;
        Plane plane = Plane.CreateByNormalAndOrigin( XYZ.BasisZ, pt ) ;
        SketchPlane sketchPlane = SketchPlane.Create( doc, plane ) ;
        ModelCurve modelLineF = doc.Create.NewModelCurve( line, sketchPlane ) ;

        transaction.Commit() ;
      }
    }
  }
}