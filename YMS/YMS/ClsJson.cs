using Autodesk.Revit.DB ;
using Newtonsoft.Json ;
using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.IO ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using YMS.Parts ;

namespace YMS
{
  public class ClsJson
  {
    const string KABE = "山留壁" ;
    const string SHIHOKOU = "支保工" ;
    const string KOUDAI = "構台" ;

    public static void ReadJOSN( Document doc )
    {
      string path = string.Empty ;
      if ( ClsYMSUtil.SelectJSONFile( ref path ) ) {
        var data = DeserializeFromFile<JosnData>( path ) ;
        if ( data is JosnData jData && data != null ) {
          CreateJsonData( doc, jData ) ;
        }
      }
    }

    /// <summary>
    /// JSONファイルからの読み込み
    /// </summary>
    /// <typeparam name="T">データ型</typeparam>
    /// <param name="path">ファイルパス</param>
    /// <returns></returns>
    public static T DeserializeFromFile<T>( string path )
    {
      if ( File.Exists( path ) is false ) return default ;
      T data ;
      try {
        // JSONファイルを開く
        using ( var stream = new FileStream( path, FileMode.Open ) ) {
          // JSONファイルを読み出す
          using ( var sr = new StreamReader( stream ) ) {
            // デシリアライズオブジェクト関数に読み込んだデータを渡して、
            // 指定されたデータ用のクラス型で値を返す。
            //string test = "{\"metaData\":{\"version\":\"0.0.1\", \"created_at\":\"past\", \"updated_at\":\"2025_01_30_12_10_18\", \"created_by\":\"whoami\", \"updated_by\":\"su\"}}";//TEST用
            var readEnd = sr.ReadToEnd() ;
            data = JsonConvert.DeserializeObject<T>( readEnd ) ; //直接sr.ReadToEnd()を引数に設定すると読込が出来ない

            //return JsonConvert.DeserializeObject<T>(sr.ReadToEnd());
          }
        }
      }
      catch ( Exception ex ) {
        string mess = ex.Message ;
        return default ;
      }

      return data ;
    }

    public static void CreateJsonData( Document doc, JosnData jData )
    {
      foreach ( var rootLayer in jData.RootLayers ) {
        var objectCustomData = rootLayer.Object.UserData.CustomData ;
        var levelName = objectCustomData.name ;
        //レベルの作成
        using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
          try {
            t.Start() ;
            if ( ! ClsYMSUtil.CreateNewLevel( doc, levelName, ClsRevitUtil.CovertToAPI( objectCustomData.height ) ) ) {
              t.RollBack() ;
              continue ;
            }

            t.Commit() ;
          }
          catch ( Exception ex ) {
          }
        } //using

        if ( rootLayer.Object.children == null )
          continue ;

        foreach ( var chil in rootLayer.Object.children ) {
          switch ( chil.name ) {
            case KABE :
            {
              foreach ( var chilchil in chil.children ) {
                var Case = chilchil.name ;
                foreach ( var chilchilchil in chilchil.children ) {
                  switch ( chilchilchil.name ) {
                    case "SP" :
                    {
                      foreach ( var chilchilchilchil in chilchilchil.children ) {
                        try {
                          var customData = chilchilchilchil.UserData.CustomData ;
                          var array = GetTargetArray( rootLayer.Geometries, chilchilchilchil.geometry ) ;
                          if ( ! CreateKouyaita( doc, customData, array, levelName ) ) {
                            //読み込み失敗※arrayが不正かsymbol読込失敗
                          }
                        }
                        catch {
                        }
                      }

                      break ;
                    }
                    case "親杭" :
                    {
                      foreach ( var chilchilchilchil in chilchilchil.children ) {
                        try {
                          var customData = chilchilchilchil.UserData.CustomData ;
                          var array = GetTargetArray( rootLayer.Geometries, chilchilchilchil.geometry ) ;
                          if ( ! CreateOyagkui( doc, customData, array, levelName ) ) {
                            //読み込み失敗※arrayが不正かsymbol読込失敗
                          }
                        }
                        catch {
                        }
                      }

                      break ;
                    }
                    case "SMW" :
                    {
                      foreach ( var chilchilchilchil in chilchilchil.children ) {
                        try {
                          var customData = chilchilchilchil.UserData.CustomData ;
                          var array = GetTargetArray( rootLayer.Geometries, chilchilchilchil.geometry ) ;
                          if ( ! CreateSMW( doc, customData, array, levelName ) ) {
                            //読み込み失敗※arrayが不正かsymbol読込失敗
                          }
                        }
                        catch {
                        }
                      }

                      break ;
                    }
                  }
                }
              }

              break ;
            }
            case SHIHOKOU :
            {
              foreach ( var chilchil in chil.children ) {
                switch ( chilchil.name ) {
                  case "腹起" :
                  case "腹起し" : //JSONのミスで腹起しになっている修正が確認出来次第腹起に変更
                  {
                    foreach ( var chilchilchil in chilchil.children ) {
                      try {
                        var customData = chilchilchil.UserData.CustomData ;
                        var array = GetTargetArray( rootLayer.Geometries, chilchilchil.geometry ) ;
                        var tmpStPoint = GetWorldPosition( chilchilchil.matrix, chilchil.matrix,
                          rootLayer.Object.matrix, array, 0 ) ;
                        var tmpEdPoint = GetWorldPosition( chilchilchil.matrix, chilchil.matrix,
                          rootLayer.Object.matrix, array, 1 ) ;
                        if ( ! CreateHaraokoshi( doc, customData, tmpStPoint, tmpEdPoint, levelName ) ) {
                          //読み込み失敗※arrayが不正かsymbol読込失敗
                        }
                      }
                      catch {
                      }
                    }

                    break ;
                  }
                  case "切梁" :
                  {
                    foreach ( var chilchilchil in chilchil.children ) {
                      try {
                        var customData = chilchilchil.UserData.CustomData ;
                        var array = GetTargetArray( rootLayer.Geometries, chilchilchil.geometry ) ;
                        var tmpStPoint = GetWorldPosition( chilchilchil.matrix, chilchil.matrix,
                          rootLayer.Object.matrix, array, 0 ) ;
                        var tmpEdPoint = GetWorldPosition( chilchilchil.matrix, chilchil.matrix,
                          rootLayer.Object.matrix, array, 1 ) ;
                        if ( ! CreateKiribari( doc, customData, tmpStPoint, tmpEdPoint, levelName ) ) {
                          //読み込み失敗※arrayが不正かsymbol読込失敗
                        }
                      }
                      catch {
                      }
                    }

                    break ;
                  }
                  case "隅火打" :
                  {
                    foreach ( var chilchilchil in chilchil.children ) {
                      try {
                        var customData = chilchilchil.UserData.CustomData ;
                        var array = GetTargetArray( rootLayer.Geometries, chilchilchil.geometry ) ;
                        var tmpStPoint = GetWorldPosition( chilchilchil.matrix, chilchil.matrix,
                          rootLayer.Object.matrix, array, 0 ) ;
                        var tmpEdPoint = GetWorldPosition( chilchilchil.matrix, chilchil.matrix,
                          rootLayer.Object.matrix, array, 1 ) ;
                        if ( ! CreateCornaerHiuchi( doc, customData, tmpStPoint, tmpEdPoint, levelName ) ) {
                          //読み込み失敗※arrayが不正かsymbol読込失敗
                        }
                      }
                      catch {
                      }
                    }

                    break ;
                  }
                  case "切梁火打" :
                  {
                    foreach ( var chilchilchil in chilchil.children ) {
                      try {
                        var customData = chilchilchil.UserData.CustomData ;
                        var array = GetTargetArray( rootLayer.Geometries, chilchilchil.geometry ) ;
                        var tmpStPoint = GetWorldPosition( chilchilchil.matrix, chilchil.matrix,
                          rootLayer.Object.matrix, array, 0 ) ;
                        var tmpEdPoint = GetWorldPosition( chilchilchil.matrix, chilchil.matrix,
                          rootLayer.Object.matrix, array, 1 ) ;
                        switch ( chilchilchil.name ) {
                          case "切梁火打" :
                          {
                            if ( ! CreateKiribariHiuchi( doc, customData, tmpStPoint, tmpEdPoint, levelName ) ) {
                              //読み込み失敗※arrayが不正かsymbol読込失敗
                            }

                            break ;
                          }
                          case "三軸ピース" :
                          {
                            var insertPoint = GetWorldPosition( chilchilchil.matrix, chilchil.matrix,
                              rootLayer.Object.matrix, array, 0 ) ;
                            var center = GetWorldPosition( chilchilchil.matrix, chilchil.matrix,
                              rootLayer.Object.matrix, array, 2 ) ;
                            if ( ! CreateSanjikuPeace( doc, customData, insertPoint, center,
                                  levelName ) ) //作成出来る。位置に問題があるかもしれないが。
                            {
                              //読み込み失敗※arrayが不正かsymbol読込失敗
                            }

                            break ;
                          }
                        }
                      }
                      catch {
                      }
                    }

                    break ;
                  }
                  case "切梁継材・受け材" :
                  case "切梁受け" :
                  {
                    foreach ( var chilchilchil in chilchil.children ) {
                      try {
                        var customData = chilchilchil.UserData.CustomData ;
                        var array = GetTargetArray( rootLayer.Geometries, chilchilchil.geometry ) ;
                        var tmpStPoint = GetWorldPosition( chilchilchil.matrix, chilchil.matrix,
                          rootLayer.Object.matrix, array, 0 ) ;
                        var tmpEdPoint = GetWorldPosition( chilchilchil.matrix, chilchil.matrix,
                          rootLayer.Object.matrix, array, 1 ) ;
                        switch ( chilchilchil.name ) {
                          case "火打繋ぎ材" :
                          {
                            if ( ! CreateHiuchiTsunagizai( doc, customData, tmpStPoint, tmpEdPoint, levelName ) ) {
                              //読み込み失敗※arrayが不正かsymbol読込失敗
                            }

                            break ;
                          }
                          case "切梁継材" :
                          {
                            if ( ! CreateKiribariTsugi( doc, customData, tmpStPoint, tmpEdPoint, levelName ) ) {
                              //読み込み失敗※arrayが不正かsymbol読込失敗
                            }

                            break ;
                          }
                          case "切梁繋ぎ" :
                          {
                            if ( ! CreateKiribariTsunagizai( doc, customData, tmpStPoint, tmpEdPoint, levelName ) ) {
                              //読み込み失敗※arrayが不正かsymbol読込失敗
                            }

                            break ;
                          }
                          case "切梁受け" :
                          {
                            if ( ! CreateKiribariUke( doc, customData, tmpStPoint, tmpEdPoint, levelName ) ) {
                              //読み込み失敗※arrayが不正かsymbol読込失敗
                            }

                            break ;
                          }
                        }
                      }
                      catch {
                      }
                    }

                    break ;
                  }
                  case "ジャッキ" :
                  {
                    foreach ( var chilchilchil in chilchil.children ) {
                      try {
                        var customData = chilchilchil.UserData.CustomData ;
                        var array = GetTargetArray( rootLayer.Geometries, chilchilchil.geometry ) ;
                        //var insertPoint = (GetWorldPosition(chilchilchil.matrix, chilchil.matrix, rootLayer.Object.matrix, array, 0) +
                        //                   GetWorldPosition(chilchilchil.matrix, chilchil.matrix, rootLayer.Object.matrix, array, 3)) / 2;
                        var insertPoint =
                          ( GetWorldPosition( chilchilchil.matrix, chilchil.matrix, rootLayer.Object.matrix, array,
                            1 ) + GetWorldPosition( chilchilchil.matrix, chilchil.matrix, rootLayer.Object.matrix,
                            array, 2 ) ) / 2 ;
                        if ( ! CreateJack( doc, customData, insertPoint, levelName ) ) //作成出来る。位置に問題があるかもしれないが。
                        {
                          //読み込み失敗※arrayが不正かsymbol読込失敗
                        }
                      }
                      catch {
                      }
                    }

                    break ;
                  }
                  case "棚杭" :
                  {
                    foreach ( var chilchilchil in chilchil.children ) {
                      try {
                        var customData = chilchilchil.UserData.CustomData ;
                        var array = GetTargetArray( rootLayer.Geometries, chilchilchil.geometry ) ;
                        if ( ! CreateTanakui( doc, customData, array, levelName ) ) {
                          //読み込み失敗※arrayが不正かsymbol読込失敗
                        }
                      }
                      catch {
                      }
                    }

                    break ;
                  }
                  default :
                    break ;
                }
              }

              break ;
            }
            case KOUDAI :
            {
              break ;
            }
            default :
              break ;
          }
        }
      }
    }

    /// <summary>
    /// 位置のみでクラスデータを持っているため対象のarrayを探す
    /// </summary>
    /// <param name="geometries">位置情報のみを持つクラス</param>
    /// <param name="targetGeometrie">どの位置情報か判別するためのid</param>
    /// <returns></returns>
    private static List<double> GetTargetArray( List<Geometries> geometries, string targetGeometrie )
    {
      //foreach (var geometrie in geometries)
      //{
      //    if (geometrie.uuid == targetGeometrie)
      //    {
      //        var array = geometrie.data.Attributes.Position.array.ToList();
      //        return array;
      //    }
      //}

      //var array =
      return ( from data in geometries where data.uuid == targetGeometrie select data.data.Attributes.Position.array )
        .ToList().First() ;
      //var tetet = geometries.Where(x => x.uuid == targetGeometrie).First().data.Attributes.Position.array.ToList();

      //return new List<double>();
    }

    private static bool CreateHaraokoshi( Document doc, CustomData customData, XYZ tmpStPoint, XYZ tmpEdPoint,
      string levelName )
    {
      //if (array.Count != 6)//始終点のみのはず
      //    return false;

      //var tmpStPoint = new XYZ(ClsRevitUtil.CovertToAPI(array[0]), ClsRevitUtil.CovertToAPI(array[1]), ClsRevitUtil.CovertToAPI(array[2]));
      //var tmpEdPoint = new XYZ(ClsRevitUtil.CovertToAPI(array[3]), ClsRevitUtil.CovertToAPI(array[4]), ClsRevitUtil.CovertToAPI(array[5]));

      var clsHaraokoshiBase = new ClsHaraokoshiBase() ;
      /*ここは読み込んだ値*/
      clsHaraokoshiBase.m_kouzaiType = customData.steelType ;
      clsHaraokoshiBase.m_kouzaiSize = customData.steelSize ;
      clsHaraokoshiBase.m_level = levelName ;
      clsHaraokoshiBase.m_dan = customData.stage ;
      clsHaraokoshiBase.m_yoko = customData.horizontalBraceCount == "シングル"
        ? ClsHaraokoshiBase.SideNum.Single
        : ClsHaraokoshiBase.SideNum.Double ;
      clsHaraokoshiBase.m_tate = customData.verticalBraceCount == "シングル"
        ? ClsHaraokoshiBase.VerticalNum.Single
        : ClsHaraokoshiBase.VerticalNum.Double ;
      /*ここは読み込んだ値*/

      //Lightで腹起が中心で作成されている場合オフセット
      double syuzaiSize = Master.ClsYamadomeCsv.GetWidth( customData.steelSize ) ;
      return　clsHaraokoshiBase.CreateHaraokoshiBase( doc, tmpStPoint, tmpEdPoint, syuzaiSize ) ;
    }

    private static bool CreateKiribari( Document doc, CustomData customData, XYZ tmpStPoint, XYZ tmpEdPoint,
      string levelName )
    {
      //if (array.Count != 6)//始終点のみのはず
      //    return false;

      //var tmpStPoint = new XYZ(ClsRevitUtil.CovertToAPI(array[0]), ClsRevitUtil.CovertToAPI(array[1]), ClsRevitUtil.CovertToAPI(array[2]));
      //var tmpEdPoint = new XYZ(ClsRevitUtil.CovertToAPI(array[3]), ClsRevitUtil.CovertToAPI(array[4]), ClsRevitUtil.CovertToAPI(array[5]));

      var clsKiribariBase = new ClsKiribariBase() ;
      /*ここは読み込んだ値*/
      clsKiribariBase.m_kouzaiType = customData.steelType ;
      clsKiribariBase.m_kouzaiSize = customData.steelSize ;
      clsKiribariBase.m_Floor = levelName ;
      clsKiribariBase.m_Dan = customData.supportObjects[ 0 ].stage ;
      clsKiribariBase.m_tanbuStart = customData.endPartStartPoint ;
      clsKiribariBase.m_tanbuEnd = customData.endPartEndPoint ;
      clsKiribariBase.m_jack1 = "ｷﾘﾝｼﾞｬｯｷ(KJ)" ;
      clsKiribariBase.m_jack2 = "油圧ｼﾞｬｯｷ(KOP)" ;
      /*ここは読み込んだ値*/
      //切梁ベースの向きを統一する
      var cv = ClsRevitUtil.ChangDirection( tmpStPoint, tmpEdPoint ) ;
      tmpStPoint = cv.GetEndPoint( 0 ) ;
      tmpEdPoint = cv.GetEndPoint( 1 ) ;

      return clsKiribariBase.CreateKiribariBase( doc, tmpStPoint, tmpEdPoint ) ;
    }

    private static bool CreateCornaerHiuchi( Document doc, CustomData customData, XYZ tmpStPoint, XYZ tmpEdPoint,
      string levelName ) //List<double> array
    {
      //if (array.Count != 6)//始終点のみのはず
      //    return false;

      //var tmpStPoint = new XYZ(ClsRevitUtil.CovertToAPI(array[0]), ClsRevitUtil.CovertToAPI(array[1]), ClsRevitUtil.CovertToAPI(array[2]));
      //var tmpEdPoint = new XYZ(ClsRevitUtil.CovertToAPI(array[3]), ClsRevitUtil.CovertToAPI(array[4]), ClsRevitUtil.CovertToAPI(array[5]));

      double length = Line.CreateBound( tmpStPoint, tmpEdPoint ).Length ;

      var clsCornerHiuchiBase = new ClsCornerHiuchiBase() ;
      /*ここは読み込んだ値*/
      clsCornerHiuchiBase.m_SteelType = customData.steelType ;
      clsCornerHiuchiBase.m_SteelSize = customData.steelSize ;
      clsCornerHiuchiBase.m_Floor = levelName ;
      var levelId = ClsRevitUtil.GetLevelID( doc, levelName ) ;
      /*ここは読み込んだ値*/

      //腹起との判定になりそう
      string kousei = "シングル" ;
      string dan = "同段" ;
      if ( customData.supportObjects != null ) {
        if ( customData.supportObjects.Count == 2 ) {
          //接続する腹起の段を比較して違う場合はダブルと判定する
          if ( customData.supportObjects[ 0 ].stage != customData.supportObjects[ 1 ].stage ) {
            kousei = "ダブル" ;
          }
          else {
            dan = customData.supportObjects[ 0 ].stage ;
          }
        }
      }

      clsCornerHiuchiBase.m_Kousei =
        kousei == "シングル" ? ClsCornerHiuchiBase.Kousei.Single : ClsCornerHiuchiBase.Kousei.Double ;
      clsCornerHiuchiBase.m_Dan = dan ; //まだ不明。腹起との接触判定で段を取得するものと思われる

      //隅火打と接触する腹起ベースをこちらで見つける
      XYZ haraS1 = new XYZ(), haraE1 = new XYZ(), haraS2 = new XYZ(), haraE2 = new XYZ() ;
      var haraBaseList = ClsHaraokoshiBase.GetAllHaraokoshiBaseList( doc ) ; //, levelId);
      foreach ( var haraBase in haraBaseList ) {
        var inst = doc.GetElement( haraBase ) as FamilyInstance ;
        var cv = ( inst.Location as LocationCurve ).Curve ;
        //
        //cv = 
        if ( ClsRevitUtil.IsPointOnCurve( cv, tmpStPoint ) ) {
          haraS1 = cv.GetEndPoint( 0 ) ;
          haraE1 = cv.GetEndPoint( 1 ) ;
        }
        else if ( ClsRevitUtil.IsPointOnCurve( cv, tmpEdPoint ) ) {
          haraS2 = cv.GetEndPoint( 0 ) ;
          haraE2 = cv.GetEndPoint( 1 ) ;
        }
      }

      //angleC = 一本目の腹起ベースと二本目の腹起ベースのなす角
      double angleC = ClsRevitUtil.CalculateAngleBetweenLines( haraS1, haraE1, haraS2, haraE2 ) ;
      clsCornerHiuchiBase.m_angle = ClsGeo.FloorAtDigitAdjust( 1, ( 180 - angleC ) / 2 ) ; //接触する腹起とのなす角を求める※方法等分と同じ

      //とりあえずなしで設定。Lightで端部部品が設定できないのに何か設定してしまうと長さが合わないため
      clsCornerHiuchiBase.m_TanbuParts1 = customData.componentName ; // "なし";
      clsCornerHiuchiBase.m_HiuchiUkePieceSize1 =
        Master.ClsHiuchiCsv.GetSize( customData.componentName, customData.steelSize ) ; // "なし";
      if ( customData.componentName == "火打受ピース" & customData.steelSize == "25HA" )
        clsCornerHiuchiBase.m_HiuchiUkePieceSize1 = Master.ClsHiuchiCsv.GetSize( customData.componentName, "20HA" ) ;

      clsCornerHiuchiBase.m_TanbuParts2 = customData.componentName ; //"なし";
      clsCornerHiuchiBase.m_HiuchiUkePieceSize2 =
        Master.ClsHiuchiCsv.GetSize( customData.componentName, customData.steelSize ) ; //"なし";
      if ( customData.componentName == "火打受ピース" & customData.steelSize == "25HA" )
        clsCornerHiuchiBase.m_HiuchiUkePieceSize2 = Master.ClsHiuchiCsv.GetSize( customData.componentName, "20HA" ) ;

      /*位置から算出した値*/
      clsCornerHiuchiBase.m_HiuchiZureryo = 0.0 ;
      //端部部品を抜いた長さを設定する
      var l = length ;
      XYZ ps = new XYZ(), pe = new XYZ() ;
      clsCornerHiuchiBase.GetTanbuPartsPoints( doc, tmpStPoint, tmpEdPoint, ref ps, ref pe ) ;
      if ( ! ClsGeo.GEO_EQ( ps, pe ) )
        l = Line.CreateBound( ps, pe ).Length ;
      clsCornerHiuchiBase.m_HiuchiLengthDoubleUpperL1 = ClsRevitUtil.CovertFromAPI( l ) ;
      clsCornerHiuchiBase.m_HiuchiLengthDoubleUnderL2 = ClsRevitUtil.CovertFromAPI( l ) ;
      clsCornerHiuchiBase.m_HiuchiLengthSingleL = ClsRevitUtil.CovertFromAPI( l ) ;
      /*位置から算出した値*/

      return clsCornerHiuchiBase.CreateCornerHiuchi( doc, tmpStPoint, tmpEdPoint ) ;
    }

    private static bool CreateKiribariHiuchi( Document doc, CustomData customData, XYZ tmpStPoint, XYZ tmpEdPoint,
      string levelName )
    {
      //if (array.Count != 6)//始終点のみのはず
      //    return false;

      //var tmpStPoint = new XYZ(ClsRevitUtil.CovertToAPI(array[0]), ClsRevitUtil.CovertToAPI(array[1]), ClsRevitUtil.CovertToAPI(array[2]));
      //var tmpEdPoint = new XYZ(ClsRevitUtil.CovertToAPI(array[3]), ClsRevitUtil.CovertToAPI(array[4]), ClsRevitUtil.CovertToAPI(array[5]));

      double length = Line.CreateBound( tmpStPoint, tmpEdPoint ).Length ;

      var clsKiribariHiuchiBase = new ClsKiribariHiuchiBase() ;
      /*ここは読み込んだ値*/
      clsKiribariHiuchiBase.m_SteelType = customData.steelType ;
      clsKiribariHiuchiBase.m_Floor = levelName ;
      if ( customData.processingMethod == "CutBeam-CutBeam" ) //切梁-切梁
      {
        clsKiribariHiuchiBase.m_ShoriType = ClsKiribariHiuchiBase.ShoriType.KiriKiri ;
      }
      else if ( customData.processingMethod == "Abutment-CutBeam" && customData.componentName == "三軸ピース" ) //三軸-腹起
      {
        clsKiribariHiuchiBase.m_ShoriType = ClsKiribariHiuchiBase.ShoriType.SanjikuHara ;
      }
      else //切梁-腹起
      {
        clsKiribariHiuchiBase.m_ShoriType = ClsKiribariHiuchiBase.ShoriType.KiriHara ;
      }

      //切梁火打の向きが切梁始点腹起側終点になるように変更する
      if ( customData.processingMethod != "CutBeam-CutBeam" ) {
        var haraBaseList = ClsHaraokoshiBase.GetAllHaraokoshiBaseList( doc ) ;
        foreach ( var haraBase in haraBaseList ) {
          var inst = doc.GetElement( haraBase ) as FamilyInstance ;
          var cv = ( inst.Location as LocationCurve ).Curve ;
          //始点が腹起を通る場合は切梁火打の始終点を入れ替える
          if ( ClsRevitUtil.IsPointOnCurve( cv, tmpStPoint ) ) {
            var c = tmpStPoint ;
            tmpStPoint = tmpEdPoint ;
            tmpEdPoint = c ;
            break ;
          }
        }
      }

      string kousei = "シングル" ;
      string dan = "同段" ;
      if ( customData.supportObjects != null ) {
        if ( customData.supportObjects.Count == 2 ) {
          //接続するベースの段を比較して違う場合はダブルと判定する
          if ( customData.supportObjects[ 0 ].stage != customData.supportObjects[ 1 ].stage ) {
            kousei = "ダブル" ;
          }
          else {
            dan = customData.supportObjects[ 0 ].stage ;
          }
        }
      }

      clsKiribariHiuchiBase.m_CreateHoho = kousei == "シングル"
        ? ClsKiribariHiuchiBase.CreateHoho.Single
        : ClsKiribariHiuchiBase.CreateHoho.Double ;
      if ( clsKiribariHiuchiBase.m_CreateHoho == ClsKiribariHiuchiBase.CreateHoho.Single ) {
        clsKiribariHiuchiBase.m_SteelSizeSingle = customData.steelSize ;
      }
      else {
        clsKiribariHiuchiBase.m_SteelSizeDoubleUpper = customData.steelSize ;
        clsKiribariHiuchiBase.m_SteelSizeDoubleUnder = customData.steelSize ;
      }

      clsKiribariHiuchiBase.m_Dan = dan ;
      clsKiribariHiuchiBase.m_Angle = customData.angle ;
      /*ここは読み込んだ値*/

      if ( customData.componentName == "三軸ピース" )
        clsKiribariHiuchiBase.m_PartsTypeKiriSide = "なし" ;
      else
        clsKiribariHiuchiBase.m_PartsTypeKiriSide = customData.componentName ;

      //主材サイズ端部部品から引っ張ってくる
      clsKiribariHiuchiBase.m_PartsSizeKiriSide =
        Master.ClsHiuchiCsv.GetSize2( customData.componentName, customData.steelSize ) ;
      if ( customData.componentName == "火打受ピース" && customData.steelSize == "25HA" )
        clsKiribariHiuchiBase.m_PartsSizeKiriSide = Master.ClsHiuchiCsv.GetSize2( customData.componentName, "20HA" ) ;

      //とりあえずなしで設定。Lightで端部部品が設定できないのに何か設定してしまうと長さが合わないため
      clsKiribariHiuchiBase.m_PartsTypeHaraSide = "なし" ;
      //clsKiribariHiuchiBase.m_PartsSizeHaraSide = "なし";

      //端部部品を抜いた長さを設定する
      var l = length ;
      XYZ ps = new XYZ(), pe = new XYZ() ;
      clsKiribariHiuchiBase.GetTanbuPartsPoints( doc, tmpStPoint, tmpEdPoint, ref ps, ref pe,
        Master.ClsSanjikuPieceCSV.GetSize( customData.steelSize ) ) ;
      if ( ! ClsGeo.GEO_EQ( ps, pe ) )
        l = Line.CreateBound( ps, pe ).Length ;

      /*位置から算出した値*/
      clsKiribariHiuchiBase.m_HiuchiZureRyo = 0 ;
      clsKiribariHiuchiBase.m_HiuchiLengthDoubleUpperL1 = (int) ClsRevitUtil.CovertFromAPI( l ) ;
      clsKiribariHiuchiBase.m_HiuchiLengthDoubleUnderL2 = (int) ClsRevitUtil.CovertFromAPI( l ) ;
      clsKiribariHiuchiBase.m_HiuchiLengthSingleL = (int) ClsRevitUtil.CovertFromAPI( l ) ;
      /*位置から算出した値*/

      return clsKiribariHiuchiBase.CreateKiribariHiuchiBase( doc, tmpStPoint, tmpEdPoint ) ;
    }

    private static bool CreateKiribariUke( Document doc, CustomData customData, XYZ tmpStPoint, XYZ tmpEdPoint,
      string levelName )
    {
      //if (array.Count != 6)//始終点のみのはず
      //    return false;

      //var tmpStPoint = new XYZ(ClsRevitUtil.CovertToAPI(array[0]), ClsRevitUtil.CovertToAPI(array[1]), ClsRevitUtil.CovertToAPI(array[2]));
      //var tmpEdPoint = new XYZ(ClsRevitUtil.CovertToAPI(array[3]), ClsRevitUtil.CovertToAPI(array[4]), ClsRevitUtil.CovertToAPI(array[5]));

      var clsKiribariUkeBase = new ClsKiribariUkeBase() ;
      /*ここは読み込んだ値*/
      clsKiribariUkeBase.m_SteelType = customData.steelType ;
      clsKiribariUkeBase.m_SteelSize = customData.steelSize ;
      clsKiribariUkeBase.m_Floor = levelName ;
      /*ここは読み込んだ値*/

      clsKiribariUkeBase.m_Dan = "下段" ; //固定
      clsKiribariUkeBase.m_TsukidashiRyoS = 0 ; //突き出し量のデフォルトは不明
      clsKiribariUkeBase.m_TsukidashiRyoE = 0 ;

      return clsKiribariUkeBase.ChangeKiribariUkeBase( doc, tmpStPoint, tmpEdPoint ) ;
    }

    private static bool CreateKiribariTsunagizai( Document doc, CustomData customData, XYZ tmpStPoint, XYZ tmpEdPoint,
      string levelName )
    {
      //if (array.Count != 6)//始終点のみのはず
      //    return false;

      //var tmpStPoint = new XYZ(ClsRevitUtil.CovertToAPI(array[0]), ClsRevitUtil.CovertToAPI(array[1]), ClsRevitUtil.CovertToAPI(array[2]));
      //var tmpEdPoint = new XYZ(ClsRevitUtil.CovertToAPI(array[3]), ClsRevitUtil.CovertToAPI(array[4]), ClsRevitUtil.CovertToAPI(array[5]));

      var clsKiribariTsunagizaiBase = new ClsKiribariTsunagizaiBase() ;
      /*ここは読み込んだ値*/
      clsKiribariTsunagizaiBase.m_SteelType = customData.steelType ;
      clsKiribariTsunagizaiBase.m_SteelSize = customData.steelSize ;
      clsKiribariTsunagizaiBase.m_Floor = levelName ;
      string torituke = customData.installationMethod ;
      if ( torituke == "ボルト" ) {
        clsKiribariTsunagizaiBase.m_ToritsukeHoho = ClsKiribariTsunagizaiBase.ToritsukeHoho.Bolt ;
        clsKiribariTsunagizaiBase.m_BoltType1 = customData.boltType ;
        List<string> sizeList = Master.ClsBoltCsv.GetSizeList( clsKiribariTsunagizaiBase.m_BoltType1 ) ;
        if ( sizeList.Count != 0 )
          clsKiribariTsunagizaiBase.m_BoltType2 = sizeList[ 0 ] ; //ボルト種類ごとの初期定義をデフォルトに
        clsKiribariTsunagizaiBase.m_BoltNum = 1 ; //ボルトデフォルト本数は不明
      }
      else if ( torituke == "ブルマン" ) {
        clsKiribariTsunagizaiBase.m_ToritsukeHoho = ClsKiribariTsunagizaiBase.ToritsukeHoho.Buruman ;
      }
      else {
        clsKiribariTsunagizaiBase.m_ToritsukeHoho = ClsKiribariTsunagizaiBase.ToritsukeHoho.Rikiman ;
      }
      /*ここは読み込んだ値*/

      clsKiribariTsunagizaiBase.m_Dan = "上段" ; //固定

      return clsKiribariTsunagizaiBase.ChangeKiribariTsunagizaiBase( doc, tmpStPoint, tmpEdPoint ) ;
    }

    private static bool CreateHiuchiTsunagizai( Document doc, CustomData customData, XYZ tmpStPoint, XYZ tmpEdPoint,
      string levelName )
    {
      //if (array.Count != 6)//始終点のみのはず
      //    return false;

      //var tmpStPoint = new XYZ(ClsRevitUtil.CovertToAPI(array[0]), ClsRevitUtil.CovertToAPI(array[1]), ClsRevitUtil.CovertToAPI(array[2]));
      //var tmpEdPoint = new XYZ(ClsRevitUtil.CovertToAPI(array[3]), ClsRevitUtil.CovertToAPI(array[4]), ClsRevitUtil.CovertToAPI(array[5]));

      var clsHiuchiTsunagizaiBase = new ClsHiuchiTsunagizaiBase() ;
      /*ここは読み込んだ値*/
      clsHiuchiTsunagizaiBase.m_SteelType = customData.steelType ;
      clsHiuchiTsunagizaiBase.m_SteelSize = customData.steelSize ;
      clsHiuchiTsunagizaiBase.m_Floor = levelName ;
      string torituke = customData.installationMethod ;
      if ( torituke == "ボルト" ) {
        clsHiuchiTsunagizaiBase.m_ToritsukeHoho = ClsHiuchiTsunagizaiBase.ToritsukeHoho.Bolt ;
        clsHiuchiTsunagizaiBase.m_BoltType1 = customData.boltType ;
        List<string> sizeList = Master.ClsBoltCsv.GetSizeList( clsHiuchiTsunagizaiBase.m_BoltType1 ) ;
        if ( sizeList.Count != 0 )
          clsHiuchiTsunagizaiBase.m_BoltType2 = sizeList[ 0 ] ; //ボルト種類ごとの初期定義をデフォルトに
        clsHiuchiTsunagizaiBase.m_BoltNum = 1 ; //ボルトデフォルト本数は不明
      }
      else if ( torituke == "ブルマン" ) {
        clsHiuchiTsunagizaiBase.m_ToritsukeHoho = ClsHiuchiTsunagizaiBase.ToritsukeHoho.Buruman ;
      }
      else {
        clsHiuchiTsunagizaiBase.m_ToritsukeHoho = ClsHiuchiTsunagizaiBase.ToritsukeHoho.Rikiman ;
      }
      /*ここは読み込んだ値*/

      clsHiuchiTsunagizaiBase.m_Dan = "上段" ; //固定

      return clsHiuchiTsunagizaiBase.ChangeHiuchiTsunagizaiBase( doc, tmpStPoint, tmpEdPoint ) ;
    }

    private static bool CreateKiribariTsugi( Document doc, CustomData customData, XYZ tmpStPoint, XYZ tmpEdPoint,
      string levelName )
    {
      //if (array.Count != 6)//始終点のみのはず
      //    return false;

      //var tmpStPoint = new XYZ(ClsRevitUtil.CovertToAPI(array[0]), ClsRevitUtil.CovertToAPI(array[1]), ClsRevitUtil.CovertToAPI(array[2]));
      //var tmpEdPoint = new XYZ(ClsRevitUtil.CovertToAPI(array[3]), ClsRevitUtil.CovertToAPI(array[4]), ClsRevitUtil.CovertToAPI(array[5]));

      double length = Line.CreateBound( tmpStPoint, tmpEdPoint ).Length ;

      var clsKiribariTsugiBase = new ClsKiribariTsugiBase() ;
      /*ここは読み込んだ値*/
      clsKiribariTsugiBase.m_Floor = levelName ;
      if ( customData.supportObjects != null ) {
        if ( customData.supportObjects.Count == 1 ) {
          clsKiribariTsugiBase.m_Dan = customData.supportObjects[ 0 ].stage ;
          clsKiribariTsugiBase.m_SteelType = customData.supportObjects[ 0 ].steelType ;

          clsKiribariTsugiBase.m_Kousei =
            "シングル" == "シングル" ? ClsKiribariTsugiBase.Kousei.Single : ClsKiribariTsugiBase.Kousei.Double ;
          if ( clsKiribariTsugiBase.m_Kousei == ClsKiribariTsugiBase.Kousei.Single ) {
            clsKiribariTsugiBase.m_SteelSizeSingle = customData.supportObjects[ 0 ].steelSize ;
          }
          else {
            clsKiribariTsugiBase.m_KiriSideSteelSizeDouble = customData.supportObjects[ 0 ].steelSize ;
            clsKiribariTsugiBase.m_HaraSideSteelSizeDouble = customData.supportObjects[ 0 ].steelSize ;
          }
        }
      }
      /*ここは読み込んだ値*/

      //とりあえずなしで設定。Lightで端部部品が設定できないのに何か設定してしまうと長さが合わないため
      clsKiribariTsugiBase.m_KiriSideParts = "なし" ;
      clsKiribariTsugiBase.m_HaraSideParts = "なし" ;

      /*位置から算出した値*/
      clsKiribariTsugiBase.m_KiriSideTsunagiLength = (int) ClsRevitUtil.CovertFromAPI( length ) ;
      clsKiribariTsugiBase.m_HaraSideTsunagiLength = (int) ClsRevitUtil.CovertFromAPI( length ) ;
      /*位置から算出した値*/

      return clsKiribariTsugiBase.CreateKiribariTsugiBase( doc, tmpStPoint, tmpEdPoint ) ;
    }

    private static bool CreateJack( Document doc, CustomData customData, XYZ insertPoint, string levelName )
    {
      var clsJack = new ClsJack() ;
      /*ここは読み込んだ値*/
      clsJack.m_JackType = customData.jackType ;
      clsJack.m_SyuzaiSize = customData.supportObjects[ 0 ].steelSize ;
      string jackSize = Master.ClsJackCsv.GetJackSize( clsJack.m_JackType, clsJack.m_SyuzaiSize ) ;
      string strTypeNameType = Master.ClsJackCsv.GetTypeNameType( jackSize ) ; //Atypeなど
      if ( strTypeNameType != "Atype" ) {
        clsJack.m_JackTensyo = "九州" ;
      }

      ElementId levalId = ClsRevitUtil.GetLevelID( doc, levelName ) ;
      var baseIdList = ClsKiribariBase.GetAllKiribariBaseList( doc, levalId ) ;
      foreach ( var id in baseIdList ) {
        var inst = doc.GetElement( id ) as FamilyInstance ;
        var cv = ( inst.Location as LocationCurve ).Curve ;
        var tmpStPoint = cv.GetEndPoint( 0 ) ;
        var tmpEdPoint = cv.GetEndPoint( 1 ) ;
        //レベルでベースを選択しているためZを0に変更
        cv = Line.CreateBound( new XYZ( tmpStPoint.X, tmpStPoint.Y, 0 ), new XYZ( tmpEdPoint.X, tmpEdPoint.Y, 0 ) ) ;
        insertPoint = new XYZ( insertPoint.X, insertPoint.Y, 0 ) ;
        if ( ClsRevitUtil.IsPointOnCurve( cv, insertPoint ) ) {
          if ( ! ClsRevitUtil.CheckNearGetEndPoint( cv, insertPoint ) ) {
            var change = tmpStPoint ;
            tmpStPoint = tmpEdPoint ;
            tmpEdPoint = change ;
          }

          return clsJack.CreateJack( doc, id, tmpStPoint, tmpEdPoint, insertPoint ) ;
        }
      }
      /*ここは読み込んだ値*/

      return false ;
    }

    private static bool CreateSanjikuPeace( Document doc, CustomData customData, XYZ insertPoint, XYZ center,
      string levelName )
    {
      var dir = ( center - insertPoint ).Normalize() ;

      //中心点から乗っている切梁を特定する
      var kiriBaseList = ClsKiribariBase.GetAllKiribariBaseList( doc ) ;
      foreach ( var id in kiriBaseList ) {
        var inst = doc.GetElement( id ) as FamilyInstance ;
        var cv = ( inst.Location as LocationCurve ).Curve ;
        if ( ClsRevitUtil.IsPointOnCurve( cv, center ) ) {
          return ClsSanjikuPeace.CreateSanjikuPeace( doc, id, insertPoint, dir ) ;
        }
      }

      return false ;
    }

    private static bool CreateTanakui( Document doc, CustomData customData, List<double> array, string levelName )
    {
      //if (array.Count != 6)//始終点のみのはず
      //    return false;

      var point = new XYZ( ClsRevitUtil.CovertToAPI( customData.centerPoint.x ),
        ClsRevitUtil.CovertToAPI( customData.centerPoint.y ), ClsRevitUtil.CovertToAPI( customData.centerPoint.z ) ) ;

      var clsTanaKui = new ClsTanakui() ;
      /*ここは読み込んだ値*/
      clsTanaKui.m_HightFromGL = customData.pileTopEdge ;
      clsTanaKui.m_KouzaiType = customData.steelType ;
      clsTanaKui.m_KouzaiSize = customData.steelSize ;
      clsTanaKui.m_CreatePosition = ClsTanakui.GetCreatePoition( customData.placementPosition ) ;
      clsTanaKui.m_ZureryouX = customData.amountOfDriftX ;
      clsTanaKui.m_ZureryouY = customData.amountOfDriftY ;
      clsTanaKui.m_PileTotalLength = customData.totalLength ;
      clsTanaKui.m_PileHaichiAngle = customData.placementAngle ;
      clsTanaKui.m_CreateKiribariBracket = customData.placingCutBeamBracket ;
      clsTanaKui.m_CreateKiribariOsaeBracket = customData.placingCutBeamHoldingBracket ;
      clsTanaKui.m_TsugiteCount = (int) customData.numberOfJoints ;
      clsTanaKui.m_FixingType =
        customData.jointMethod == "ボルト" ? ClsTanakui.FixingType.Bolt : ClsTanakui.FixingType.Welding ;
      /*ここは読み込んだ値*/
      //作成方法が交点、1点に関わらずずれ量がある場合は位置がずれるため全て交点で固定
      clsTanaKui.m_CreateType = ClsTanakui.CreateType.Intersection ;

      List<int> lstN1 = new List<int>() ;
      int tsugiteCount = clsTanaKui.m_TsugiteCount + 1 ;
      int kuiLength1 = (int) clsTanaKui.m_PileTotalLength / tsugiteCount ;
      for ( int i = 0 ; i < tsugiteCount ; i++ ) {
        //最終杭長さは余りを全て吸収する
        if ( i + 1 == tsugiteCount ) {
          lstN1.Add( (int) clsTanaKui.m_PileTotalLength - kuiLength1 * i ) ;
        }
        else
          lstN1.Add( kuiLength1 ) ;
      }

      clsTanaKui.m_PileLengthList = lstN1.ToList() ;

      //ブラケットサイズを指定する項目がLightにはないため
      clsTanaKui.m_KiribariBracketSizeIsAuto = true ;
      clsTanaKui.m_KiribariOsaeBracketSizeIsAuto = true ;
      //プレロードとガイド材は作成しないと思われるため
      clsTanaKui.m_CreatePreloadGuide = false ;
      clsTanaKui.m_CreateGuide = false ;
      //Lightではずれ量を考慮した位置に配置されているためこちらでは交点配置に固定する
      clsTanaKui.m_CreateType = ClsTanakui.CreateType.Intersection ;

      string bolt = "BN-50" ;
      string boltNum = "8" ;
      clsTanaKui.m_TsugiteBoltSize_Flange = bolt ;
      clsTanaKui.m_TsugiteBoltQuantity_Flange = boltNum ;
      clsTanaKui.m_TsugiteBoltSize_Web = bolt ;
      clsTanaKui.m_TsugiteBoltQuantity_Web = boltNum ;

      ElementId levelId = ClsRevitUtil.GetLevelID( doc, levelName ) ;
      string uniqueName = string.Empty ;
      var create = clsTanaKui.CreateTanaKui( doc, point, levelId, false, ref uniqueName ) ;
      // 切梁ブラケットも同時に作成する場合
      if ( clsTanaKui.m_CreateKiribariBracket ) {
        ClsBracket clsBracket = new ClsBracket() ;
        clsBracket.m_BracketSize = clsTanaKui.m_KiribariBracketSize ;
        clsBracket.m_TargetKuiAngle = clsTanaKui.m_PileHaichiAngle ;
        clsBracket.m_TargetKuiCreatePosition = clsTanaKui.m_CreatePosition ;
        clsBracket.m_TargetKuiCreateType = clsTanaKui.m_CreateType ;
        if ( clsBracket.CreateKiribariBracket( doc, clsTanaKui.m_CreateId, clsTanaKui.m_KiribariBracketSizeIsAuto ) ) {
          // プレロードガイド材も作成する場合
          if ( clsTanaKui.m_CreatePreloadGuide ) {
            for ( int i = 0 ; i < clsBracket.m_CreateIds.Count() ; i++ ) {
              clsBracket.CreatePreloadGuideZai( doc, clsBracket.m_CreateIds[ i ],
                clsBracket.m_TargetKiribariBaseIds[ i ] ) ;
            }
          }
        }
      }

      // 切梁押えブラケットも同時に作成する場合
      if ( clsTanaKui.m_CreateKiribariOsaeBracket ) {
        ClsBracket clsBracket = new ClsBracket() ;
        clsBracket.m_BracketSize = clsTanaKui.m_KiribariOsaeBracketSize ;
        clsBracket.m_TargetKuiAngle = clsTanaKui.m_PileHaichiAngle ;
        clsBracket.m_TargetKuiCreatePosition = clsTanaKui.m_CreatePosition ;
        clsBracket.m_TargetKuiCreateType = clsTanaKui.m_CreateType ;
        if ( clsBracket.CreateKiribariOsaeBracket( doc, clsTanaKui.m_CreateId,
              clsTanaKui.m_KiribariOsaeBracketSizeIsAuto ) ) {
          // ガイド材も作成する場合※
          if ( clsTanaKui.m_CreateGuide ) {
            for ( int i = 0 ; i < clsBracket.m_CreateIds.Count() ; i++ ) {
              clsBracket.CreateGuideZai( doc, clsBracket.m_CreateIds[ i ], clsBracket.m_TargetKiribariBaseIds[ i ] ) ;
            }
          }
        }
      }

      return create ;
    }

    #region 山留壁

    private static bool CreateKouyaita( Document doc, CustomData customData, List<double> array, string levelName )
    {
      if ( array.Count < 6 ) //
        return false ;

      ElementId levelId = ClsRevitUtil.GetLevelID( doc, levelName ) ;

      ////////////////////鋼矢板/////////////////////
      /*ここは読み込んだ値*/
      var clsKouyaita = new ClsKouyaita() ;
      clsKouyaita.m_case = ClsCommonUtils.ChangeStrToInt( customData.Case ) ;
      clsKouyaita.m_type = customData.steelType ;
      clsKouyaita.m_size = customData.steelSize ;
      clsKouyaita.m_HTop = (int) customData.pileTopEdge ;
      clsKouyaita.m_HLen = (int) customData.pileLength ;
      clsKouyaita.m_bIzyou = customData.useDeformedSheetPile == "する" ? true : false ;
      clsKouyaita.m_zaishitu = customData.materialQuality ;
      clsKouyaita.m_zanti = customData.remain ;
      clsKouyaita.m_zantiLength = customData.remainingLength.ToString() ;
      clsKouyaita.m_Kasho1 = (int) customData.numberOfJoints ;
      //clsKouyaita.m_Kasho2 = ClsCommonUtils.ChangeStrToInt("2");//1と一緒
      clsKouyaita.m_Kotei1 = customData.jointMethod == "ボルト" ? Master.Kotei.Bolt : Master.Kotei.Yousetsu ;
      //clsKouyaita.m_Kotei2 = "ボルト" == "ボルト" ? Master.Kotei.Bolt : Master.Kotei.Yousetsu;
      /*ここは読み込んだ値*/
      //一緒枝番は何かに固定する必要がある
      clsKouyaita.m_edaNum = "A" ;
      //clsKouyaita.m_edaNum2 = "A";
      clsKouyaita.m_way = 0 ; //全て自動作成想定でないといけない
      //掘削深さの設定は要相談
      clsKouyaita.m_void = (int) customData.excavationDepth ;
      clsKouyaita.m_putVec = "交互" == "交互" ? PutVec.Kougo : PutVec.Const ; //作成方法によって必要性が変わる
      clsKouyaita.m_kouyaitaSize = "SP-C3" ; //コーナー鋼矢板サイズは自動設定の可能性あり
      clsKouyaita.m_KougoFlg = false ; //交互配置は無いので確定

      //設定があるか不明
      clsKouyaita.TensetuVec1 = "腹側" == "腹側" ? TensetuVec.HaraSide : TensetuVec.SenakaSide ;
      //clsKouyaita.TensetuVec2 = "腹側" == "腹側" ? TensetuVec.HaraSide : TensetuVec.SenakaSide;

      List<int> lstN1 = new List<int>() ;
      int kasho1 = clsKouyaita.m_Kasho1 + 1 ;
      int kuiLength1 = clsKouyaita.m_HLen / kasho1 ;
      for ( int i = 0 ; i < kasho1 ; i++ ) {
        //最終杭長さは余りを全て吸収する
        if ( i + 1 == kasho1 ) {
          lstN1.Add( clsKouyaita.m_HLen - kuiLength1 * i ) ;
        }
        else
          lstN1.Add( kuiLength1 ) ;
      }

      clsKouyaita.m_ListPileLength1 = lstN1.ToList() ;
      ;

      string bolt = customData.headWearBoltSize ;
      string boltNum = "8" ;
      clsKouyaita.m_BoltF1 = bolt ;
      clsKouyaita.m_BoltFNum1 = boltNum ;
      //Lightに設定項目が1つしかないので側やコーナーに関係なく同じ
      clsKouyaita.m_BoltW1 = bolt ;
      clsKouyaita.m_BoltWNum1 = boltNum ;
      clsKouyaita.m_BoltCornerF1 = bolt ;
      clsKouyaita.m_BoltCornerFNum1 = boltNum ;
      clsKouyaita.m_BoltCornerW1 = bolt ;
      clsKouyaita.m_BoltCornerWNum1 = boltNum ;
      bool createAtamaTsunagi = false ;

      //一度の処理で囲まれた壁を作成しているか（YMS自動作成に相当する作成方法）で出隅チェックを行うか判断する
      var checkIrizumi = true ;
      if ( array[ 0 ] != array[ array.Count - 3 ] || array[ 1 ] != array[ array.Count - 2 ] ||
           array[ 2 ] != array[ array.Count - 1 ] )
        checkIrizumi = false ;

      ElementId desumiKabeShinId = null ;
      VoidVec voidVec = VoidVec.Kussaku ;
      VoidVec lastVoidVec = VoidVec.Kabe ;
      List<int> targetDesumiList = new List<int>() ;
      List<VoidVec> targetDesumilastVoidVecList = new List<VoidVec>() ;
      List<ElementId> createKouyaitaList = new List<ElementId>() ;
      for ( int a = 0 ; a < ( array.Count - 3 ) ; a += 3 ) {
        var tmpStPoint = new XYZ( ClsRevitUtil.CovertToAPI( array[ a ] ), ClsRevitUtil.CovertToAPI( array[ a + 1 ] ),
          ClsRevitUtil.CovertToAPI( array[ a + 2 ] ) ) ;
        var tmpEdPoint = new XYZ( ClsRevitUtil.CovertToAPI( array[ a + 3 ] ),
          ClsRevitUtil.CovertToAPI( array[ a + 4 ] ), ClsRevitUtil.CovertToAPI( array[ a + 5 ] ) ) ;

        ElementId createId = null ;
        if ( ! ClsKabeShin.CreateKabeShin( doc, levelName, tmpStPoint, tmpEdPoint, ref createId ) )
          return false ;
        var kabeShin = createId ;
        var inst = doc.GetElement( kabeShin ) as FamilyInstance ;
        //var levelId = inst.Host.Id;

        //入隅か判定
        //入隅か判定※6この時は1本しか線を作成していないため入隅の判定は行わない
        bool bIrizumi = false ;
        if ( array.Count != 6 ) {
          XYZ tmpEdPoint2 = new XYZ() ;
          if ( array.Count <= a + 6 ) {
            tmpEdPoint2 = new XYZ( ClsRevitUtil.CovertToAPI( array[ 3 ] ), ClsRevitUtil.CovertToAPI( array[ 4 ] ),
              ClsRevitUtil.CovertToAPI( array[ 5 ] ) ) ;
          }
          else {
            tmpEdPoint2 = new XYZ( ClsRevitUtil.CovertToAPI( array[ a + 6 ] ),
              ClsRevitUtil.CovertToAPI( array[ a + 7 ] ), ClsRevitUtil.CovertToAPI( array[ a + 8 ] ) ) ;
          }

          var cv1 = Line.CreateBound( tmpStPoint, tmpEdPoint ) ;
          var cv2 = Line.CreateBound( tmpEdPoint, tmpEdPoint2 ) ;
          if ( ! ClsHaraokoshiBase.CheckIrizumi( cv1, cv2, ref bIrizumi ) ) {
            //continue;
          }


          if ( ! bIrizumi && checkIrizumi ) //出隅のときのみ別で作成する
          {
            if ( 0 < createKouyaitaList.Count )
              targetDesumiList.Add( createKouyaitaList.Count - 1 ) ;
            else
              targetDesumiList.Add( -1 ) ;
            targetDesumilastVoidVecList.Add( lastVoidVec ) ;
            desumiKabeShinId = kabeShin ;
          }
        }

        createKouyaitaList.AddRange( clsKouyaita.CreateKouyaita1( doc, tmpStPoint, tmpEdPoint, levelId, voidVec,
          ref lastVoidVec, createCorner: true, bIrizumi: bIrizumi, kabeShinId: kabeShin ).ToList() ) ;
        if ( createAtamaTsunagi ) {
          //頭ツナギ作成
          CreateAtamTsunagi( doc, customData, clsKouyaita.m_FirstKouyaita, clsKouyaita.m_LastKouyaita ) ;
          clsKouyaita.m_FirstKouyaita = null ;
        }
      }

      if ( 0 < targetDesumiList.Count ) {
        for ( int i = 0 ; i < targetDesumiList.Count ; i++ ) {
          int targetDesumi = targetDesumiList[ i ] ;
          if ( targetDesumi == -1 )
            targetDesumi = createKouyaitaList.Count - 1 ;
          //出隅一個前の鋼矢板を削除する
          ElementId lastDesumiKouyaita = createKouyaitaList[ targetDesumi ] ;
          createKouyaitaList.Remove( lastDesumiKouyaita ) ;
          FamilyInstance inst = doc.GetElement( lastDesumiKouyaita ) as FamilyInstance ;
          XYZ dir = inst.HandOrientation ;
          XYZ insertPoint = ( inst.Location as LocationPoint ).Point ;
          using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
            t.Start() ;
            ClsRevitUtil.Delete( doc, lastDesumiKouyaita ) ;
            t.Commit() ;
          }

          //出隅部専用のコーナー矢板を作成する
          createKouyaitaList.Add( clsKouyaita.CreateSingleKouyaita( doc, insertPoint, dir, levelId,
            targetDesumilastVoidVecList[ i ], desumiKabeShinId ) ) ;
        }
      }

      return true ;
    }

    private static bool CreateOyagkui( Document doc, CustomData customData, List<double> array, string levelName )
    {
      if ( array.Count < 6 ) //始終点のみのはず
        return false ;

      /*ここは読み込んだ値*/
      ClsOyakui clsOyakui = new ClsOyakui() ;
      clsOyakui.m_case = ClsCommonUtils.ChangeStrToInt( customData.Case ) ;
      clsOyakui.m_void = (int) customData.excavationDepth ;
      clsOyakui.m_refPDist = 0 ;
      clsOyakui.m_putPitch = (int) customData.placementPitch ;
      clsOyakui.m_type = customData.steelType ;
      clsOyakui.m_size = customData.steelSize ;
      clsOyakui.m_HTop = (int) customData.pileTopEdge ;
      clsOyakui.m_HLen = (int) customData.pileLength ;
      clsOyakui.m_zanti = customData.remain ;
      clsOyakui.m_zantiLength = customData.remainingLength.ToString() ;
      clsOyakui.m_Kasho1 = (int) customData.numberOfJoints ;
      //clsOyakui.m_Kasho2 = ClsCommonUtils.ChangeStrToInt("2");
      clsOyakui.m_Kotei1 = customData.jointMethod == "ボルト" ? Master.Kotei.Bolt : Master.Kotei.Yousetsu ;
      //clsOyakui.m_Kotei2 = "ボルト" == "ボルト" ? Master.Kotei.Bolt : Master.Kotei.Yousetsu;
      /*ここは読み込んだ値*/

      clsOyakui.m_edaNum = "A" ;
      //clsOyakui.m_edaNum2 = "A";
      clsOyakui.m_KougoFlg = false ; //Lightに交互フラグが無いため確定
      clsOyakui.m_way = 1 ;
      clsOyakui.m_putPosFlag = 1 ; //杭芯

      clsOyakui.m_bYokoyaita = false ; //Lightで付けられるか不明
      clsOyakui.m_typeYokoyaita = "木矢板" ;
      clsOyakui.m_sizeYokoyaita = "木矢板" ;
      clsOyakui.m_putPosYokoyaitaFlag = 0 ; //内
      bool createAtamaTsunagi = false ;

      List<int> lstN1 = new List<int>() ;
      int kasho1 = clsOyakui.m_Kasho1 + 1 ;
      int kuiLength1 = clsOyakui.m_HLen / kasho1 ;
      for ( int i = 0 ; i < kasho1 ; i++ ) {
        //最終杭長さは余りを全て吸収する
        if ( i + 1 == kasho1 ) {
          lstN1.Add( clsOyakui.m_HLen - kuiLength1 * i ) ;
        }
        else
          lstN1.Add( kuiLength1 ) ;
      }

      clsOyakui.m_ListPileLength1 = lstN1.ToList() ;

      string bolt = customData.headWearBoltSize ;
      string boltNum = "8" ;
      clsOyakui.m_BoltF1 = bolt ;
      clsOyakui.m_BoltFNum1 = boltNum ;
      clsOyakui.m_BoltW1 = bolt ;
      clsOyakui.m_BoltWNum1 = boltNum ;

      //一度の処理で囲まれた壁を作成しているか（YMS自動作成に相当する作成方法）で出隅チェックを行うか判断する
      var checkIrizumi = true ;
      if ( array[ 0 ] != array[ array.Count - 3 ] || array[ 1 ] != array[ array.Count - 2 ] ||
           array[ 2 ] != array[ array.Count - 1 ] )
        checkIrizumi = false ;

      for ( int a = 0 ; a < ( array.Count - 3 ) ; a += 3 ) {
        var tmpStPoint = new XYZ( ClsRevitUtil.CovertToAPI( array[ a ] ), ClsRevitUtil.CovertToAPI( array[ a + 1 ] ),
          ClsRevitUtil.CovertToAPI( array[ a + 2 ] ) ) ;
        var tmpEdPoint = new XYZ( ClsRevitUtil.CovertToAPI( array[ a + 3 ] ),
          ClsRevitUtil.CovertToAPI( array[ a + 4 ] ), ClsRevitUtil.CovertToAPI( array[ a + 5 ] ) ) ;

        ElementId createId = null ;
        if ( ! ClsKabeShin.CreateKabeShin( doc, levelName, tmpStPoint, tmpEdPoint, ref createId ) )
          return false ;

        var kabeShin = createId ;
        var inst = doc.GetElement( kabeShin ) as FamilyInstance ;
        var levelId = inst.Host.Id ;

        //入隅か判定
        //入隅か判定※6この時は1本しか線を作成していないため入隅の判定は行わない
        if ( array.Count != 6 ) {
          bool bIrizumi = false ;
          XYZ tmpEdPoint2 = new XYZ() ;
          if ( array.Count <= a + 6 ) {
            tmpEdPoint2 = new XYZ( ClsRevitUtil.CovertToAPI( array[ 3 ] ), ClsRevitUtil.CovertToAPI( array[ 4 ] ),
              ClsRevitUtil.CovertToAPI( array[ 5 ] ) ) ;
          }
          else {
            tmpEdPoint2 = new XYZ( ClsRevitUtil.CovertToAPI( array[ a + 6 ] ),
              ClsRevitUtil.CovertToAPI( array[ a + 7 ] ), ClsRevitUtil.CovertToAPI( array[ a + 8 ] ) ) ;
          }

          var cv1 = Line.CreateBound( tmpStPoint, tmpEdPoint ) ;
          var cv2 = Line.CreateBound( tmpEdPoint, tmpEdPoint2 ) ;
          if ( ! ClsHaraokoshiBase.CheckIrizumi( cv1, cv2, ref bIrizumi ) ) {
            //continue;
          }

          if ( ! bIrizumi && checkIrizumi ) //出隅のときのみ別で親杭を作成する
          {
            var dir1 = cv2.Direction ;
            var dir2 = cv1.Direction ;
            var dir = dir1 + dir2 ;
            var apexAngle = Math.PI - dir1.AngleOnPlaneTo( dir2, XYZ.BasisZ ) ;
            var desumipoint = tmpEdPoint ;
            if ( clsOyakui.m_putPosFlag != 0 ) {
              double dEx = ClsRevitUtil.CovertToAPI(
                ClsCommonUtils.ChangeStrToDbl( Master.ClsYamadomeCsv.GetKouzaiSizeSunpou( clsOyakui.m_size, 1 ) ) /
                2 ) ;
              desumipoint = desumipoint + dEx * ( dir2 - dir1 ) ;
            }

            clsOyakui.CreateSingleOyakui( doc, desumipoint, dir, levelId, bIrizumi, true, kabeShin, apexAngle ) ;
          }
        }

        clsOyakui.CreateOyakui( doc, tmpStPoint, tmpEdPoint, levelId, true, kabeShin ) ;
        if ( createAtamaTsunagi ) {
          //頭ツナギ作成
          CreateAtamTsunagi( doc, customData, clsOyakui.m_FirstKui, clsOyakui.m_LastKui ) ;
          clsOyakui.m_FirstKui = null ;
        }
      }

      return true ;
    }

    private static bool CreateSMW( Document doc, CustomData customData, List<double> array, string levelName )
    {
      if ( array.Count < 6 ) //始終点のみのはず
        return false ;

      /*ここは読み込んだ値*/
      ClsSMW clsSMW = new ClsSMW() ;
      clsSMW.m_case = ClsCommonUtils.ChangeStrToInt( customData.Case ) ;
      clsSMW.m_zanti = customData.remain ;
      clsSMW.m_zantiLength = customData.remainingLength.ToString() ;
      clsSMW.m_dia = (int) customData.soilDiameter ;
      clsSMW.m_soil = (int) customData.soilPitch ;
      clsSMW.m_soilTop = (int) customData.soilTopEdge ;
      clsSMW.m_soilLen = (int) customData.soilTotalLength ;
      clsSMW.m_type = customData.coreMaterialType ;
      clsSMW.m_size = customData.coreMaterialSize ;
      clsSMW.m_HTop = (int) customData.coreMaterialTopEdge ;
      clsSMW.m_HLen = (int) customData.coreMaterialTotalLength ;
      clsSMW.m_Kasho1 = (int) customData.numberOfJoints ;
      //clsSMW.m_Kasho2 = ClsCommonUtils.ChangeStrToInt("2");
      clsSMW.m_Kotei1 = customData.jointMethod == "ボルト" ? Master.Kotei.Bolt : Master.Kotei.Yousetsu ;
      //clsSMW.m_Kotei2 = "ボルト" == "ボルト" ? Master.Kotei.Bolt : Master.Kotei.Yousetsu;
      /*ここは読み込んだ値*/

      clsSMW.m_edaNum = "A" ;
      clsSMW.m_edaNum2 = "A" ;
      clsSMW.m_way = 1 ;
      clsSMW.m_void = (int) customData.soilExcavationDepth ;
      clsSMW.m_bVoid = false ; // true;// false;
      clsSMW.m_KougoFlg = false ;

      //Lightで設定できるか不明
      clsSMW.m_refPDist = ClsCommonUtils.ChangeStrToInt( "0" ) ;
      clsSMW.m_bTanbuS = true ;
      clsSMW.m_bTanbuE = false ;
      clsSMW.m_bCorner = customData.coreMaterialPlacedAtTheEntryCorner == "する" ? true : false ; //入隅判定が出来なさそう
      if ( customData.coreMaterialPlacedPattern == "LeftCenterRight" )
        clsSMW.m_putPtnFlag = ClsCommonUtils.ChangeStrToInt( "10" ) ;
      else if ( customData.coreMaterialPlacedPattern == "LeftCenter" )
        clsSMW.m_putPtnFlag = ClsCommonUtils.ChangeStrToInt( "3" ) ;
      else
        clsSMW.m_putPtnFlag = ClsCommonUtils.ChangeStrToInt( "2" ) ;

      List<int> lstN1 = new List<int>() ;
      int kasho1 = clsSMW.m_Kasho1 + 1 ;
      int kuiLength1 = clsSMW.m_HLen / kasho1 ;
      for ( int i = 0 ; i < kasho1 ; i++ ) {
        //最終杭長さは余りを全て吸収する
        if ( i + 1 == kasho1 ) {
          lstN1.Add( clsSMW.m_HLen - kuiLength1 * i ) ;
        }
        else
          lstN1.Add( kuiLength1 ) ;
      }

      clsSMW.m_ListPileLength1 = lstN1.ToList() ;

      string bolt = customData.headWearBoltSize ;
      string boltNum = "8" ;
      clsSMW.m_BoltF1 = bolt ;
      clsSMW.m_BoltFNum1 = boltNum ;
      clsSMW.m_BoltW1 = bolt ;
      clsSMW.m_BoltWNum1 = boltNum ;

      //一度の処理で囲まれた壁を作成しているか（YMS自動作成に相当する作成方法）で出隅チェックを行うか判断する
      var checkIrizumi = true ;
      if ( array[ 0 ] != array[ array.Count - 3 ] || array[ 1 ] != array[ array.Count - 2 ] ||
           array[ 2 ] != array[ array.Count - 1 ] )
        checkIrizumi = false ;

      for ( int a = 0 ; a < ( array.Count - 3 ) ; a += 3 ) {
        var tmpStPoint = new XYZ( ClsRevitUtil.CovertToAPI( array[ a ] ), ClsRevitUtil.CovertToAPI( array[ a + 1 ] ),
          ClsRevitUtil.CovertToAPI( array[ a + 2 ] ) ) ;
        var tmpEdPoint = new XYZ( ClsRevitUtil.CovertToAPI( array[ a + 3 ] ),
          ClsRevitUtil.CovertToAPI( array[ a + 4 ] ), ClsRevitUtil.CovertToAPI( array[ a + 5 ] ) ) ;

        ElementId createId = null ;
        if ( ! ClsKabeShin.CreateKabeShin( doc, levelName, tmpStPoint, tmpEdPoint, ref createId ) )
          return false ;

        var kabeShin = createId ;
        var inst = doc.GetElement( kabeShin ) as FamilyInstance ;
        var levelId = inst.Host.Id ;

        //入隅か判定※6この時は1本しか線を作成していないため入隅の判定は行わない
        if ( array.Count != 6 ) {
          bool bIrizumi = false ;
          XYZ tmpEdPoint2 = new XYZ() ;
          if ( array.Count <= a + 6 ) {
            tmpEdPoint2 = new XYZ( ClsRevitUtil.CovertToAPI( array[ 3 ] ), ClsRevitUtil.CovertToAPI( array[ 4 ] ),
              ClsRevitUtil.CovertToAPI( array[ 5 ] ) ) ;
          }
          else {
            tmpEdPoint2 = new XYZ( ClsRevitUtil.CovertToAPI( array[ a + 6 ] ),
              ClsRevitUtil.CovertToAPI( array[ a + 7 ] ), ClsRevitUtil.CovertToAPI( array[ a + 8 ] ) ) ;
          }

          var cv1 = Line.CreateBound( tmpStPoint, tmpEdPoint ) ;
          var cv2 = Line.CreateBound( tmpEdPoint, tmpEdPoint2 ) ;
          if ( ! ClsHaraokoshiBase.CheckIrizumi( cv1, cv2, ref bIrizumi ) ) {
            //continue;
          }

          var dir1 = cv2.Direction ;
          var dir2 = cv1.Direction ;
          var dir = dir1 + dir2 ;
          if ( ! bIrizumi && checkIrizumi ) //出隅のときのみ別でSMWを作成する
          {
            double apexAngle = Math.PI - dir1.AngleOnPlaneTo( dir2, XYZ.BasisZ ) ;
            XYZ desumipoint = tmpEdPoint ;
            double dEx = ClsRevitUtil.CovertToAPI(
              ClsCommonUtils.ChangeStrToDbl( Master.ClsYamadomeCsv.GetKouzaiSizeSunpou( clsSMW.m_size, 1 ) ) / 2 ) ;
            desumipoint = desumipoint + dEx * ( dir2 - dir1 ) ;

            clsSMW.CreateSingleSMW( doc, desumipoint, dir, levelId, bIrizumi, kabeShinId: kabeShin,
              apexAngle: apexAngle ) ;
          }
        }

        clsSMW.CreateSMW( doc, tmpStPoint, tmpEdPoint, levelId, true, true, kabeShin ) ;
      }

      return true ;
    }

    private static void CreateAtamTsunagi( Document doc, CustomData customData, ElementId oyakuiIdSt,
      ElementId oyakuiIdEd )
    {
      if ( oyakuiIdSt == null || oyakuiIdEd == null || oyakuiIdSt == oyakuiIdEd )
        return ;

      var clsAtamaTsunagi = new ClsAtamaTsunagi() ;

      clsAtamaTsunagi.m_KouzaiType = customData.headWearInfoSteelType ;
      clsAtamaTsunagi.m_KouzaiSize = customData.headWearInfoSteelSize ;
      clsAtamaTsunagi.m_ChannelDirection =
        ClsAtamaTsunagi.GetChannelDirection( customData.headWearChannelMaterialOrientation ) ;
      clsAtamaTsunagi.m_JointType = ClsAtamaTsunagi.GetJointType( customData.headWearJoiningMethod ) ;
      clsAtamaTsunagi.m_BoltType = customData.headWearBoltType ;
      clsAtamaTsunagi.m_BoltSize = customData.headWearBoltSize ;
      clsAtamaTsunagi.m_ConfigurationDirection =
        ClsAtamaTsunagi.GetConfigurationDirection( customData.headWearPlacementOrientation ) ;

      try {
        if ( clsAtamaTsunagi.m_KouzaiType == "主材" ) {
          clsAtamaTsunagi.CreateAutoAtamaTsunagiZaiSyuzai( doc, oyakuiIdSt, oyakuiIdEd ) ;
        }
        else {
          clsAtamaTsunagi.CreateAtamaTsunagiZai( doc, oyakuiIdSt, oyakuiIdEd ) ;
        }
      }
      catch {
      }
    }

    #endregion

    #region TEST

    public static void CreateBase( Document doc )
    {
      XYZ tmpStPoint = new XYZ() ;
      XYZ tmpEdPoint = new XYZ() ;

      List<List<XYZ>> sePointList = new List<List<XYZ>>() ;
      List<XYZ> pointList = new List<XYZ>() ;
      ////本目
      //pointList.Add(new XYZ(0, 0, 0));
      //pointList.Add(new XYZ(0, 0, 0));
      //sePointList.Add(pointList.ToList());
      //pointList.Clear();

      ////////////////////腹起ベース/////////////////////
      //1本目
      pointList.Add( new XYZ( -27.2144134912447, -7.883328489805398, 0 ) ) ;
      pointList.Add( new XYZ( 0.954177183384225, -7.88332848905407, 0 ) ) ;
      sePointList.Add( pointList.ToList() ) ;
      pointList.Clear() ;
      //2本目
      pointList.Add( new XYZ( -0.358158774621028, -9.19566444705932, 0 ) ) ;
      pointList.Add( new XYZ( -0.358158774620964, 10.4893749230194, 0 ) ) ;
      sePointList.Add( pointList.ToList() ) ;
      pointList.Clear() ;
      //3本目
      pointList.Add( new XYZ( 0.954177183384281, 9.17703896501417, 0 ) ) ;
      pointList.Add( new XYZ( -27.2610459137286, 9.17703896501426, 0 ) ) ;
      sePointList.Add( pointList.ToList() ) ;
      pointList.Clear() ;
      //4本目
      pointList.Add( new XYZ( -25.9020775332394, 11.145542902021, 0 ) ) ;
      pointList.Add( new XYZ( -25.9020775332395, -9.19566444705923, 0 ) ) ;
      sePointList.Add( pointList.ToList() ) ;
      pointList.Clear() ;

      foreach ( var point in sePointList ) {
        tmpStPoint = point[ 0 ] ;
        tmpEdPoint = point[ 1 ] ;

        var clsHaraokoshiBase = new ClsHaraokoshiBase() ;
        /*ここは読み込んだ値*/
        clsHaraokoshiBase.m_kouzaiType = "主材" ;
        clsHaraokoshiBase.m_kouzaiSize = "40HA" ;
        clsHaraokoshiBase.m_level = "支保工1段目" ;
        clsHaraokoshiBase.m_dan = "同段" ;
        clsHaraokoshiBase.m_yoko =
          "シングル" == "シングル" ? ClsHaraokoshiBase.SideNum.Single : ClsHaraokoshiBase.SideNum.Double ;
        clsHaraokoshiBase.m_tate =
          "シングル" == "シングル" ? ClsHaraokoshiBase.VerticalNum.Single : ClsHaraokoshiBase.VerticalNum.Double ;
        /*ここは読み込んだ値*/

        //Lightで腹起が中心で作成されている場合オフセット
        double syuzaiSize = 0 ; // Master.ClsYamadomeCsv.GetWidth("40HA");
        clsHaraokoshiBase.CreateHaraokoshiBase( doc, tmpStPoint, tmpEdPoint, syuzaiSize ) ;
      }

      sePointList.Clear() ;
      ////////////////////腹起ベース/////////////////////
      ////////////////////切梁ベース/////////////////////
      //1本目
      pointList.Add( new XYZ( -13.1301181539302, -7.88332848905403, 0 ) ) ;
      pointList.Add( new XYZ( -13.1301181539302, 9.17703896501421, 0 ) ) ;
      sePointList.Add( pointList.ToList() ) ;
      pointList.Clear() ;
      //2本目
      pointList.Add( new XYZ( -25.9020775332395, 0.974939227481457, 0 ) ) ;
      pointList.Add( new XYZ( -0.358158774620996, 0.974939227481374, 0 ) ) ;
      sePointList.Add( pointList.ToList() ) ;
      pointList.Clear() ;
      //3本目
      pointList.Add( new XYZ( -25.9020775332394, 3.62925721568506, 0 ) ) ;
      pointList.Add( new XYZ( -0.358158774620986, 3.62925721568498, 0 ) ) ;
      sePointList.Add( pointList.ToList() ) ;
      pointList.Clear() ;

      foreach ( var point in sePointList ) {
        tmpStPoint = point[ 0 ] ;
        tmpEdPoint = point[ 1 ] ;

        var clsKiribariBase = new ClsKiribariBase() ;
        /*ここは読み込んだ値*/
        clsKiribariBase.m_kouzaiType = "主材" ;
        clsKiribariBase.m_kouzaiSize = "40HA" ;
        clsKiribariBase.m_Floor = "支保工1段目" ;
        clsKiribariBase.m_tanbuStart = "なし" ;
        clsKiribariBase.m_tanbuEnd = "なし" ;
        clsKiribariBase.m_jack1 = "ｷﾘﾝｼﾞｬｯｷ(KJ)" ;
        clsKiribariBase.m_jack2 = "油圧ｼﾞｬｯｷ(KOP)" ;
        /*ここは読み込んだ値*/

        clsKiribariBase.m_Dan = "同段" ; //まだ不明。腹起との接触判定で段を取得するものと思われる

        clsKiribariBase.CreateKiribariBase( doc, tmpStPoint, tmpEdPoint ) ;
      }

      sePointList.Clear() ;
      ////////////////////切梁ベース///////////////////
      ////////////////////ジャッキ/////////////////////
      //1本目
      pointList.Add( new XYZ( -21.820998517, 0.974939227, -4.921259843 ) ) ;
      sePointList.Add( pointList.ToList() ) ;
      pointList.Clear() ;
      //2本目
      //pointList.Add(new XYZ(-25.9020775332395, 0.974939227481457, 0));
      //sePointList.Add(pointList.ToList());
      //pointList.Clear();

      foreach ( var point in sePointList ) {
        var insertPoint = point[ 0 ] ;
        insertPoint = new XYZ( insertPoint.X, insertPoint.Y, 0 ) ;

        var clsJack = new ClsJack() ;
        /*ここは読み込んだ値*/
        clsJack.m_JackType = "ｷﾘﾝｼﾞｬｯｷ(KJ)" ;
        clsJack.m_SyuzaiSize = "40HA" ;
        ElementId levalId = ClsRevitUtil.GetLevelID( doc, "支保工1段目" ) ;
        ElementId baseId = null ;
        var baseIdList = ClsKiribariBase.GetAllKiribariBaseList( doc, levalId ) ;
        foreach ( var id in baseIdList ) {
          var inst = doc.GetElement( id ) as FamilyInstance ;
          var cv = ( inst.Location as LocationCurve ).Curve ;
          tmpStPoint = cv.GetEndPoint( 0 ) ;
          tmpEdPoint = cv.GetEndPoint( 1 ) ;
          cv = Line.CreateBound( new XYZ( tmpStPoint.X, tmpStPoint.Y, 0 ), new XYZ( tmpEdPoint.X, tmpEdPoint.Y, 0 ) ) ;
          if ( ClsRevitUtil.IsPointOnCurve( cv, insertPoint ) ) {
            baseId = id ;
            //tmpStPoint = cv.GetEndPoint(0);
            //tmpEdPoint = cv.GetEndPoint(1);
            break ;
          }
        }
        /*ここは読み込んだ値*/

        if ( baseId != null )
          clsJack.CreateJack( doc, baseId, tmpStPoint, tmpEdPoint, insertPoint ) ;
      }

      sePointList.Clear() ;
      ////////////////////ジャッキ/////////////////////
      ////////////////////隅火打ベース/////////////////
      //1本目
      pointList.Add( new XYZ( -25.9020775332395, -4.40347228242728, 0 ) ) ;
      pointList.Add( new XYZ( -22.4222213266128, -7.883328489054, 0 ) ) ;
      sePointList.Add( pointList.ToList() ) ;
      pointList.Clear() ;
      //2本目
      pointList.Add( new XYZ( -3.83801498124773, -7.88332848905406, 0 ) ) ;
      pointList.Add( new XYZ( -0.358158774621012, -4.40347228242736, 0 ) ) ;
      sePointList.Add( pointList.ToList() ) ;
      pointList.Clear() ;
      //3本目
      pointList.Add( new XYZ( -0.358158774620979, 5.69718275838746, 0 ) ) ;
      pointList.Add( new XYZ( -3.83801498124768, 9.17703896501418, 0 ) ) ;
      sePointList.Add( pointList.ToList() ) ;
      pointList.Clear() ;
      //4本目
      pointList.Add( new XYZ( -22.4222213266127, 9.17703896501424, 0 ) ) ;
      pointList.Add( new XYZ( -25.9020775332394, 5.69718275838755, 0 ) ) ;
      sePointList.Add( pointList.ToList() ) ;
      pointList.Clear() ;
      //5本目
      pointList.Add( new XYZ( -20.1023171888616, -7.883328489054, 0 ) ) ;
      pointList.Add( new XYZ( -25.9020775332395, -2.08356814467613, 0 ) ) ;
      sePointList.Add( pointList.ToList() ) ;
      pointList.Clear() ;

      foreach ( var point in sePointList ) {
        tmpStPoint = point[ 0 ] ;
        tmpEdPoint = point[ 1 ] ;
        double length = Line.CreateBound( tmpStPoint, tmpEdPoint ).Length ;

        var clsCornerHiuchiBase = new ClsCornerHiuchiBase() ;
        /*ここは読み込んだ値*/
        clsCornerHiuchiBase.m_SteelType = "主材" ;
        clsCornerHiuchiBase.m_SteelSize = "40HA" ;
        clsCornerHiuchiBase.m_Floor = "支保工1段目" ;
        clsCornerHiuchiBase.m_Kousei =
          "ダブル" == "シングル" ? ClsCornerHiuchiBase.Kousei.Single : ClsCornerHiuchiBase.Kousei.Double ;
        /*ここは読み込んだ値*/

        clsCornerHiuchiBase.m_angle = 45.00 ; //角度の求め方は不明。接触する腹起とのなす角を求めるものと思われる※方法等分と同じ
        clsCornerHiuchiBase.m_Dan = "同段" ; //まだ不明。腹起との接触判定で段を取得するものと思われる

        //とりあえずなしで設定。Lightで端部部品が設定できないのに何か設定してしまうと長さが合わないため
        clsCornerHiuchiBase.m_TanbuParts1 = "なし" ;
        //clsCornerHiuchiBase.m_HiuchiUkePieceSize1 = "なし";
        clsCornerHiuchiBase.m_TanbuParts2 = "なし" ;
        //clsCornerHiuchiBase.m_HiuchiUkePieceSize2 = "なし";

        /*位置から算出した値*/
        clsCornerHiuchiBase.m_HiuchiZureryo = 0.0 ;
        clsCornerHiuchiBase.m_HiuchiLengthDoubleUpperL1 = ClsRevitUtil.CovertFromAPI( length ) ;
        clsCornerHiuchiBase.m_HiuchiLengthDoubleUnderL2 = ClsRevitUtil.CovertFromAPI( length ) ;
        clsCornerHiuchiBase.m_HiuchiLengthSingleL = ClsRevitUtil.CovertFromAPI( length ) ;
        /*位置から算出した値*/

        clsCornerHiuchiBase.CreateCornerHiuchi( doc, tmpStPoint, tmpEdPoint ) ;
      }

      sePointList.Clear() ;
      ///////////////////隅火打ベース/////////////////////
      ////////////////////切梁火打ベース//////////////////
      //1本目
      pointList.Add( new XYZ( -13.1301181539302, -4.38259314518755, 0 ) ) ;
      pointList.Add( new XYZ( -16.6308534977967, -7.88332848905402, 0 ) ) ;
      sePointList.Add( pointList.ToList() ) ;
      pointList.Clear() ;
      //2本目
      pointList.Add( new XYZ( -13.1301181539302, -4.38259314518756, 0 ) ) ;
      pointList.Add( new XYZ( -9.62938281006377, -7.88332848905404, 0 ) ) ;
      sePointList.Add( pointList.ToList() ) ;
      pointList.Clear() ;
      //3本目
      pointList.Add( new XYZ( -13.1301181539302, 5.67630362114775, 0 ) ) ;
      pointList.Add( new XYZ( -9.62938281006375, 9.1770389650142, 0 ) ) ;
      sePointList.Add( pointList.ToList() ) ;
      pointList.Clear() ;
      //4本目
      pointList.Add( new XYZ( -13.1301181539302, 5.67630362114774, 0 ) ) ;
      pointList.Add( new XYZ( -16.6308534977967, 9.17703896501422, 0 ) ) ;
      sePointList.Add( pointList.ToList() ) ;
      pointList.Clear() ;

      foreach ( var point in sePointList ) {
        tmpStPoint = point[ 0 ] ;
        tmpEdPoint = point[ 1 ] ;
        double length = Line.CreateBound( tmpStPoint, tmpEdPoint ).Length ;

        var clsKiribariHiuchiBase = new ClsKiribariHiuchiBase() ;
        /*ここは読み込んだ値*/
        clsKiribariHiuchiBase.m_SteelType = "主材" ;
        clsKiribariHiuchiBase.m_Floor = "支保工1段目" ;
        string syoriType = "切梁-腹起" ;
        if ( syoriType == "切梁－切梁" ) {
          clsKiribariHiuchiBase.m_ShoriType = ClsKiribariHiuchiBase.ShoriType.KiriKiri ;
        }
        else if ( syoriType == "切梁-腹起" ) {
          clsKiribariHiuchiBase.m_ShoriType = ClsKiribariHiuchiBase.ShoriType.KiriHara ;
        }
        else {
          clsKiribariHiuchiBase.m_ShoriType = ClsKiribariHiuchiBase.ShoriType.SanjikuHara ;
        }

        clsKiribariHiuchiBase.m_CreateHoho = "シングル" == "シングル"
          ? ClsKiribariHiuchiBase.CreateHoho.Single
          : ClsKiribariHiuchiBase.CreateHoho.Double ;
        if ( clsKiribariHiuchiBase.m_CreateHoho == ClsKiribariHiuchiBase.CreateHoho.Single ) {
          clsKiribariHiuchiBase.m_SteelSizeSingle = "40HA" ;
        }
        else {
          clsKiribariHiuchiBase.m_SteelSizeDoubleUpper = "40HA" ;
          clsKiribariHiuchiBase.m_SteelSizeDoubleUnder = "40HA" ;
        }

        clsKiribariHiuchiBase.m_Angle = 45.00 ;
        clsKiribariHiuchiBase.m_Dan = "同段" ;
        /*ここは読み込んだ値*/

        //とりあえずなしで設定。Lightで端部部品が設定できないのに何か設定してしまうと長さが合わないため
        clsKiribariHiuchiBase.m_PartsTypeKiriSide = "なし" ;
        //clsKiribariHiuchiBase.m_PartsSizeKiriSide = "なし";
        clsKiribariHiuchiBase.m_PartsTypeHaraSide = "なし" ;
        //clsKiribariHiuchiBase.m_PartsSizeHaraSide = "なし";

        /*位置から算出した値*/
        clsKiribariHiuchiBase.m_HiuchiZureRyo = 0 ;
        clsKiribariHiuchiBase.m_HiuchiLengthDoubleUpperL1 = (int) ClsRevitUtil.CovertFromAPI( length ) ;
        clsKiribariHiuchiBase.m_HiuchiLengthDoubleUnderL2 = (int) ClsRevitUtil.CovertFromAPI( length ) ;
        clsKiribariHiuchiBase.m_HiuchiLengthSingleL = (int) ClsRevitUtil.CovertFromAPI( length ) ;
        /*位置から算出した値*/

        clsKiribariHiuchiBase.CreateKiribariHiuchiBase( doc, tmpStPoint, tmpEdPoint ) ;
      }

      sePointList.Clear() ;
      ///////////////////切梁火打ベース/////////////////////
      ///////////////////切梁受けベース/////////////////////
      //1本目
      pointList.Add( new XYZ( -18.0716553876434, -1.62008661633595, 0 ) ) ;
      pointList.Add( new XYZ( -7.90105171310275, -1.62008661633599, 0 ) ) ;
      sePointList.Add( pointList.ToList() ) ;
      pointList.Clear() ;

      foreach ( var point in sePointList ) {
        tmpStPoint = point[ 0 ] ;
        tmpEdPoint = point[ 1 ] ;

        var clsKiribariUkeBase = new ClsKiribariUkeBase() ;
        /*ここは読み込んだ値*/
        clsKiribariUkeBase.m_SteelType = "主材" ;
        clsKiribariUkeBase.m_SteelSize = "40HA" ;
        clsKiribariUkeBase.m_Floor = "支保工1段目" ;
        /*ここは読み込んだ値*/

        clsKiribariUkeBase.m_Dan = "下段" ; //固定
        clsKiribariUkeBase.m_TsukidashiRyoS = 0 ; //突き出し量のデフォルトは不明
        clsKiribariUkeBase.m_TsukidashiRyoE = 0 ;

        clsKiribariUkeBase.ChangeKiribariUkeBase( doc, tmpStPoint, tmpEdPoint ) ;
      }

      sePointList.Clear() ;
      ////////////////////切梁受けベース/////////////////////
      ////////////////////切梁繋ぎ材ベース/////////////////////
      //1本目
      pointList.Add( new XYZ( -4.65044674951582, 4.76272814284305, 0 ) ) ;
      pointList.Add( new XYZ( -4.65044674951583, -0.158531699676638, 0 ) ) ;
      sePointList.Add( pointList.ToList() ) ;
      pointList.Clear() ;

      foreach ( var point in sePointList ) {
        tmpStPoint = point[ 0 ] ;
        tmpEdPoint = point[ 1 ] ;

        var clsKiribariTsunagizaiBase = new ClsKiribariTsunagizaiBase() ;
        /*ここは読み込んだ値*/
        clsKiribariTsunagizaiBase.m_SteelType = "主材" ;
        clsKiribariTsunagizaiBase.m_SteelSize = "40HA" ;
        clsKiribariTsunagizaiBase.m_Floor = "支保工1段目" ;
        string torituke = "ボルト" ;
        if ( torituke == "ボルト" ) {
          clsKiribariTsunagizaiBase.m_ToritsukeHoho = ClsKiribariTsunagizaiBase.ToritsukeHoho.Bolt ;
          clsKiribariTsunagizaiBase.m_BoltType1 = "普通ボルト" ; //orハイテンションボルトorトルシア型ボルト
          List<string> sizeList = Master.ClsBoltCsv.GetSizeList( clsKiribariTsunagizaiBase.m_BoltType1 ) ;
          if ( sizeList.Count != 0 )
            clsKiribariTsunagizaiBase.m_BoltType2 = sizeList[ 0 ] ; //ボルト種類ごとの初期定義をデフォルトに
          clsKiribariTsunagizaiBase.m_BoltNum = 1 ; //ボルトデフォルト本数は不明
        }
        else if ( torituke == "ブルマン" ) {
          clsKiribariTsunagizaiBase.m_ToritsukeHoho = ClsKiribariTsunagizaiBase.ToritsukeHoho.Buruman ;
        }
        else {
          clsKiribariTsunagizaiBase.m_ToritsukeHoho = ClsKiribariTsunagizaiBase.ToritsukeHoho.Rikiman ;
        }
        /*ここは読み込んだ値*/

        clsKiribariTsunagizaiBase.m_Dan = "上段" ; //固定

        clsKiribariTsunagizaiBase.ChangeKiribariTsunagizaiBase( doc, tmpStPoint, tmpEdPoint ) ;
      }

      sePointList.Clear() ;
      ////////////////////切梁繋ぎ材ベース/////////////////////
      ////////////////////火打繋ぎ材ベース/////////////////////
      //1本目
      pointList.Add( new XYZ( -21.7804336730623, -5.0452599359778, 0 ) ) ;
      pointList.Add( new XYZ( -24.1003378108134, -7.36516407372893, 0 ) ) ;
      sePointList.Add( pointList.ToList() ) ;
      pointList.Clear() ;

      foreach ( var point in sePointList ) {
        tmpStPoint = point[ 0 ] ;
        tmpEdPoint = point[ 1 ] ;

        var clsHiuchiTsunagizaiBase = new ClsHiuchiTsunagizaiBase() ;
        /*ここは読み込んだ値*/
        clsHiuchiTsunagizaiBase.m_SteelType = "主材" ;
        clsHiuchiTsunagizaiBase.m_SteelSize = "40HA" ;
        clsHiuchiTsunagizaiBase.m_Floor = "支保工1段目" ;
        string torituke = "ボルト" ;
        if ( torituke == "ボルト" ) {
          clsHiuchiTsunagizaiBase.m_ToritsukeHoho = ClsHiuchiTsunagizaiBase.ToritsukeHoho.Bolt ;
          clsHiuchiTsunagizaiBase.m_BoltType1 = "普通ボルト" ; //orハイテンションボルトorトルシア型ボルト
          List<string> sizeList = Master.ClsBoltCsv.GetSizeList( clsHiuchiTsunagizaiBase.m_BoltType1 ) ;
          if ( sizeList.Count != 0 )
            clsHiuchiTsunagizaiBase.m_BoltType2 = sizeList[ 0 ] ; //ボルト種類ごとの初期定義をデフォルトに
          clsHiuchiTsunagizaiBase.m_BoltNum = 1 ; //ボルトデフォルト本数は不明
        }
        else if ( torituke == "ブルマン" ) {
          clsHiuchiTsunagizaiBase.m_ToritsukeHoho = ClsHiuchiTsunagizaiBase.ToritsukeHoho.Buruman ;
        }
        else {
          clsHiuchiTsunagizaiBase.m_ToritsukeHoho = ClsHiuchiTsunagizaiBase.ToritsukeHoho.Rikiman ;
        }
        /*ここは読み込んだ値*/

        clsHiuchiTsunagizaiBase.m_Dan = "上段" ; //固定

        clsHiuchiTsunagizaiBase.ChangeHiuchiTsunagizaiBase( doc, tmpStPoint, tmpEdPoint ) ;
      }

      sePointList.Clear() ;
      ////////////////////火打繋ぎ材ベース/////////////////////
      ////////////////////切梁継ぎベース/////////////////////
      //1本目
      pointList.Add( new XYZ( -19.0123137537119, 3.62925721568504, 0 ) ) ;
      pointList.Add( new XYZ( -19.0123137537119, 9.17703896501423, 0 ) ) ;
      sePointList.Add( pointList.ToList() ) ;
      pointList.Clear() ;

      foreach ( var point in sePointList ) {
        tmpStPoint = point[ 0 ] ;
        tmpEdPoint = point[ 1 ] ;
        double length = Line.CreateBound( tmpStPoint, tmpEdPoint ).Length ;

        var clsKiribariTsugiBase = new ClsKiribariTsugiBase() ;
        /*ここは読み込んだ値*/
        clsKiribariTsugiBase.m_SteelType = "主材" ;
        clsKiribariTsugiBase.m_Floor = "支保工1段目" ;
        clsKiribariTsugiBase.m_Kousei =
          "ダブル" == "シングル" ? ClsKiribariTsugiBase.Kousei.Single : ClsKiribariTsugiBase.Kousei.Double ;
        if ( clsKiribariTsugiBase.m_Kousei == ClsKiribariTsugiBase.Kousei.Single ) {
          clsKiribariTsugiBase.m_SteelSizeSingle = "40HA" ;
        }
        else {
          clsKiribariTsugiBase.m_KiriSideSteelSizeDouble = "40HA" ;
          clsKiribariTsugiBase.m_HaraSideSteelSizeDouble = "40HA" ;
        }

        clsKiribariTsugiBase.m_Dan = "上段" ;
        /*ここは読み込んだ値*/

        //とりあえずなしで設定。Lightで端部部品が設定できないのに何か設定してしまうと長さが合わないため
        clsKiribariTsugiBase.m_KiriSideParts = "なし" ;
        clsKiribariTsugiBase.m_HaraSideParts = "なし" ;

        /*位置から算出した値*/
        clsKiribariTsugiBase.m_KiriSideTsunagiLength = (int) ClsRevitUtil.CovertFromAPI( length ) ;
        clsKiribariTsugiBase.m_HaraSideTsunagiLength = (int) ClsRevitUtil.CovertFromAPI( length ) ;
        /*位置から算出した値*/

        clsKiribariTsugiBase.CreateKiribariTsugiBase( doc, tmpStPoint, tmpEdPoint ) ;
      }

      sePointList.Clear() ;
      ///////////////////切梁継ぎベース/////////////////////
      //斜梁は未実装
    }

    public static void CreateKabe( Document doc )
    {
      XYZ tmpStPoint = new XYZ() ;
      XYZ tmpEdPoint = new XYZ() ;

      List<List<XYZ>> sePointList = new List<List<XYZ>>() ;
      List<XYZ> pointList = new List<XYZ>() ;
      ////本目
      //pointList.Add(new XYZ(0, 0, 0));
      //pointList.Add(new XYZ(0, 0, 0));
      //sePointList.Add(pointList.ToList());
      //pointList.Clear();

      ////////////////////壁芯/////////////////////
      //作成した壁芯のIDは保持しておく必要があると思われる
      List<JosnKabe> kabeShinList = new List<JosnKabe>() ;
      //1本目
      pointList.Add( new XYZ( -27.542497480746, -9.85183242606185, 0 ) ) ;
      pointList.Add( new XYZ( 1.32889359536944, -9.85183242606194, 0 ) ) ;
      sePointList.Add( pointList.ToList() ) ;
      pointList.Clear() ;
      //2本目
      pointList.Add( new XYZ( 1.32889359536944, -9.85183242606194, 0 ) ) ;
      pointList.Add( new XYZ( 1.32889359536951, 11.145542902022, 0 ) ) ;
      sePointList.Add( pointList.ToList() ) ;
      pointList.Clear() ;
      //3本目
      pointList.Add( new XYZ( 1.32889359536951, 11.1455429020221, 0 ) ) ;
      pointList.Add( new XYZ( -27.542497480746, 11.1455429020221, 0 ) ) ;
      sePointList.Add( pointList.ToList() ) ;
      pointList.Clear() ;
      //4本目
      pointList.Add( new XYZ( -27.542497480746, 11.1455429020221, 0 ) ) ;
      pointList.Add( new XYZ( -27.542497480746, -9.85183242606185, 0 ) ) ;
      sePointList.Add( pointList.ToList() ) ;
      pointList.Clear() ;

      foreach ( var point in sePointList ) {
        tmpStPoint = point[ 0 ] ;
        tmpEdPoint = point[ 1 ] ;

        var clsKabeShin = new ClsKabeShin() ;
        /*ここは読み込んだ値*/
        string levelName = "GL" ;
        string kabeType = "鋼矢板" ;
        /*ここは読み込んだ値*/

        ElementId createId = null ;
        if ( ! ClsKabeShin.CreateKabeShin( doc, levelName, tmpStPoint, tmpEdPoint, ref createId ) )
          continue ; //kabeShinList.Add(new JosnKabe(createId, kabeType));


        var kabeShin = createId ;
        var inst = doc.GetElement( kabeShin ) as FamilyInstance ;
        //var lCurve = inst.Location as LocationCurve;
        //tmpStPoint = lCurve.Curve.GetEndPoint(0);
        //tmpEdPoint = lCurve.Curve.GetEndPoint(1);
        var levelId = inst.Host.Id ;

        switch ( kabeType ) {
          case "鋼矢板" :
          {
            ////////////////////鋼矢板/////////////////////
            /*ここは読み込んだ値*/
            var clsKouyaita = new ClsKouyaita() ;
            clsKouyaita.m_case = ClsCommonUtils.ChangeStrToInt( "1" ) ;
            clsKouyaita.m_type = "鋼矢板" ;
            clsKouyaita.m_size = "SP-3" ;
            clsKouyaita.m_HTop = ClsCommonUtils.ChangeStrToInt( "0" ) ;
            clsKouyaita.m_HLen = ClsCommonUtils.ChangeStrToInt( "10000" ) ;
            clsKouyaita.m_bIzyou = false ; //値が何で来るかは不明
            clsKouyaita.m_zaishitu = "SY295" ;
            clsKouyaita.m_zanti = "なし" ;
            clsKouyaita.m_zantiLength = "0" ;
            clsKouyaita.m_Kasho1 = ClsCommonUtils.ChangeStrToInt( "2" ) ;
            //clsKouyaita.m_Kasho2 = ClsCommonUtils.ChangeStrToInt("2");//1と一緒
            clsKouyaita.m_Kotei1 = "ボルト" == "ボルト" ? Master.Kotei.Bolt : Master.Kotei.Yousetsu ;
            //clsKouyaita.m_Kotei2 = "ボルト" == "ボルト" ? Master.Kotei.Bolt : Master.Kotei.Yousetsu;
            /*ここは読み込んだ値*/
            //一緒枝番は何かに固定する必要がある
            clsKouyaita.m_edaNum = "A" ;
            //clsKouyaita.m_edaNum2 = "A";
            clsKouyaita.m_way = 1 ; //多分いらない
            //掘削深さの設定は要相談
            clsKouyaita.m_void = ClsCommonUtils.ChangeStrToInt( "5000" ) ;
            clsKouyaita.m_putVec = "交互" == "交互" ? PutVec.Kougo : PutVec.Const ; //作成方法によって必要性が変わる
            clsKouyaita.m_kouyaitaSize = "SP-C3" ; //コーナー鋼矢板サイズは自動設定の可能性あり
            clsKouyaita.m_KougoFlg = false ; //交互配置は無いので確定

            //設定があるか不明
            clsKouyaita.TensetuVec1 = "腹側" == "腹側" ? TensetuVec.HaraSide : TensetuVec.SenakaSide ;
            //clsKouyaita.TensetuVec2 = "腹側" == "腹側" ? TensetuVec.HaraSide : TensetuVec.SenakaSide;

            List<int> lstN1 = new List<int>() ;
            int kuiLength1 = clsKouyaita.m_HLen / clsKouyaita.m_Kasho1 ;
            for ( int i = 0 ; i < clsKouyaita.m_Kasho1 ; i++ ) {
              //最終杭長さは余りを全て吸収する
              if ( i + 1 == clsKouyaita.m_Kasho1 ) {
                lstN1.Add( clsKouyaita.m_HLen - kuiLength1 * i ) ;
              }
              else
                lstN1.Add( kuiLength1 ) ;
            }

            clsKouyaita.m_ListPileLength1 = lstN1.ToList() ;
            ;

            string bolt = "BN-50" ;
            string boltNum = "8" ;
            clsKouyaita.m_BoltF1 = bolt ;
            clsKouyaita.m_BoltFNum1 = boltNum ;
            //Lightに設定項目が1つしかないので側やコーナーに関係なく同じと思われる
            clsKouyaita.m_BoltW1 = bolt ;
            clsKouyaita.m_BoltWNum1 = boltNum ;
            clsKouyaita.m_BoltCornerF1 = bolt ;
            clsKouyaita.m_BoltCornerFNum1 = boltNum ;
            clsKouyaita.m_BoltCornerW1 = bolt ;
            clsKouyaita.m_BoltCornerWNum1 = boltNum ;
            VoidVec lastVoidVec = VoidVec.Kussaku ;
            clsKouyaita.CreateKouyaita1( doc, tmpStPoint, tmpEdPoint, levelId, VoidVec.Kussaku, ref lastVoidVec,
              kabeShinId: kabeShin ) ;
            break ;
          }
          case "親杭" :
          {
            /*ここは読み込んだ値*/
            ClsOyakui clsOyakui = new ClsOyakui() ;
            clsOyakui.m_case = ClsCommonUtils.ChangeStrToInt( "1" ) ;
            clsOyakui.m_void = ClsCommonUtils.ChangeStrToInt( "5000" ) ; //横矢板が付く関係か掘削深さが設定できる
            clsOyakui.m_refPDist = ClsCommonUtils.ChangeStrToInt( "0" ) ;
            clsOyakui.m_putPitch = ClsCommonUtils.ChangeStrToInt( "500" ) ;
            clsOyakui.m_type = "H形鋼 広幅" ;
            clsOyakui.m_size = "H-300X300X10X15" ;
            clsOyakui.m_HTop = ClsCommonUtils.ChangeStrToInt( "0" ) ;
            clsOyakui.m_HLen = ClsCommonUtils.ChangeStrToInt( "10000" ) ;
            clsOyakui.m_zanti = "なし" ;
            clsOyakui.m_zantiLength = "0" ;
            clsOyakui.m_Kasho1 = ClsCommonUtils.ChangeStrToInt( "2" ) ;
            //clsOyakui.m_Kasho2 = ClsCommonUtils.ChangeStrToInt("2");
            clsOyakui.m_Kotei1 = "ボルト" == "ボルト" ? Master.Kotei.Bolt : Master.Kotei.Yousetsu ;
            //clsOyakui.m_Kotei2 = "ボルト" == "ボルト" ? Master.Kotei.Bolt : Master.Kotei.Yousetsu;
            /*ここは読み込んだ値*/

            clsOyakui.m_edaNum = "A" ;
            //clsOyakui.m_edaNum2 = "A";
            clsOyakui.m_KougoFlg = false ; //Lightに交互フラグが無いため確定
            clsOyakui.m_way = 1 ;
            clsOyakui.m_putPosFlag = 1 ; //杭芯

            clsOyakui.m_bYokoyaita = false ; //Lightで付けられるか不明
            clsOyakui.m_typeYokoyaita = "木矢板" ;
            clsOyakui.m_sizeYokoyaita = "木矢板" ;
            clsOyakui.m_putPosYokoyaitaFlag = 0 ; //内


            List<int> lstN1 = new List<int>() ;
            int kasho1 = clsOyakui.m_Kasho1 + 1 ;
            int kuiLength1 = clsOyakui.m_HLen / kasho1 ;
            for ( int i = 0 ; i < kasho1 ; i++ ) {
              //最終杭長さは余りを全て吸収する
              if ( i + 1 == kasho1 ) {
                lstN1.Add( clsOyakui.m_HLen - kuiLength1 * i ) ;
              }
              else
                lstN1.Add( kuiLength1 ) ;
            }

            clsOyakui.m_ListPileLength1 = lstN1.ToList() ;

            string bolt = "BN-50" ;
            string boltNum = "8" ;
            clsOyakui.m_BoltF1 = bolt ;
            clsOyakui.m_BoltFNum1 = boltNum ;
            clsOyakui.m_BoltW1 = bolt ;
            clsOyakui.m_BoltWNum1 = boltNum ;


            break ;
          }
          case "SMW" :
          {
            /*ここは読み込んだ値*/
            ClsSMW clsSMW = new ClsSMW() ;
            clsSMW.m_case = ClsCommonUtils.ChangeStrToInt( "1" ) ;
            clsSMW.m_zanti = "なし" ;
            clsSMW.m_zantiLength = "0" ;
            clsSMW.m_dia = ClsCommonUtils.ChangeStrToInt( "550" ) ;
            clsSMW.m_soil = ClsCommonUtils.ChangeStrToInt( "450" ) ;
            clsSMW.m_soilTop = ClsCommonUtils.ChangeStrToInt( "0" ) ;
            clsSMW.m_soilLen = ClsCommonUtils.ChangeStrToInt( "10000" ) ;
            clsSMW.m_type = "H形鋼 広幅" ;
            clsSMW.m_size = "H-300X300X10X15" ;
            clsSMW.m_HTop = ClsCommonUtils.ChangeStrToInt( "0" ) ;
            clsSMW.m_HLen = ClsCommonUtils.ChangeStrToInt( "10000" ) ;
            clsSMW.m_Kasho1 = ClsCommonUtils.ChangeStrToInt( "2" ) ;
            //clsSMW.m_Kasho2 = ClsCommonUtils.ChangeStrToInt("2");
            clsSMW.m_Kotei1 = "ボルト" == "ボルト" ? Master.Kotei.Bolt : Master.Kotei.Yousetsu ;
            //clsSMW.m_Kotei2 = "ボルト" == "ボルト" ? Master.Kotei.Bolt : Master.Kotei.Yousetsu;
            /*ここは読み込んだ値*/

            clsSMW.m_edaNum = "A" ;
            clsSMW.m_edaNum2 = "A" ;
            clsSMW.m_way = 1 ;
            clsSMW.m_void = ClsCommonUtils.ChangeStrToInt( "5000" ) ;
            clsSMW.m_bVoid = false ;
            clsSMW.m_KougoFlg = false ;

            //Lightで設定できるか不明
            clsSMW.m_refPDist = ClsCommonUtils.ChangeStrToInt( "0" ) ;
            clsSMW.m_bTanbuS = true ;
            clsSMW.m_bTanbuE = false ;
            clsSMW.m_bCorner = false ; //入隅判定が出来なさそう
            clsSMW.m_putPtnFlag = ClsCommonUtils.ChangeStrToInt( "10" ) ;


            List<int> lstN1 = new List<int>() ;
            int kasho1 = clsSMW.m_Kasho1 + 1 ;
            int kuiLength1 = clsSMW.m_HLen / kasho1 ;
            for ( int i = 0 ; i < kasho1 ; i++ ) {
              //最終杭長さは余りを全て吸収する
              if ( i + 1 == kasho1 ) {
                lstN1.Add( clsSMW.m_HLen - kuiLength1 * i ) ;
              }
              else
                lstN1.Add( kuiLength1 ) ;
            }

            clsSMW.m_ListPileLength1 = lstN1.ToList() ;

            string bolt = "BN-50" ;
            string boltNum = "8" ;
            clsSMW.m_BoltF1 = bolt ;
            clsSMW.m_BoltFNum1 = boltNum ;
            clsSMW.m_BoltW1 = bolt ;
            clsSMW.m_BoltWNum1 = boltNum ;


            break ;
          }
        }
      }

      sePointList.Clear() ;
      ////////////////////壁芯/////////////////////


      ///////////////////各種壁作成///////////////////////
      //個所数は設定できるので長さは等分
      //交互配置はなし
      //枝番なし
      //読み込んだJOSNで作成した壁芯と対応する壁を見つける必要がある

      //if ("鋼矢板" == "鋼矢板")
      //{
      //    ////////////////////鋼矢板/////////////////////
      //    /*ここは読み込んだ値*/
      //    var clsKouyaita = new ClsKouyaita();
      //    clsKouyaita.m_case = ClsCommonUtils.ChangeStrToInt("1");
      //    clsKouyaita.m_type = "鋼矢板";
      //    clsKouyaita.m_size = "SP-3";
      //    clsKouyaita.m_HTop = ClsCommonUtils.ChangeStrToInt("0");
      //    clsKouyaita.m_HLen = ClsCommonUtils.ChangeStrToInt("10000");
      //    clsKouyaita.m_bIzyou = false;//値が何で来るかは不明
      //    clsKouyaita.m_zaishitu = "SY295";
      //    clsKouyaita.m_zanti = "なし";
      //    clsKouyaita.m_zantiLength = "0";
      //    clsKouyaita.m_Kasho1 = ClsCommonUtils.ChangeStrToInt("2");
      //    //clsKouyaita.m_Kasho2 = ClsCommonUtils.ChangeStrToInt("2");//1と一緒
      //    clsKouyaita.m_Kotei1 = "ボルト" == "ボルト" ? Master.Kotei.Bolt : Master.Kotei.Yousetsu;
      //    //clsKouyaita.m_Kotei2 = "ボルト" == "ボルト" ? Master.Kotei.Bolt : Master.Kotei.Yousetsu;
      //    /*ここは読み込んだ値*/
      //    //一緒枝番は何かに固定する必要がある
      //    clsKouyaita.m_edaNum = "A";
      //    //clsKouyaita.m_edaNum2 = "A";
      //    clsKouyaita.m_way = 1;//多分いらない
      //    //掘削深さの設定は要相談
      //    clsKouyaita.m_void = ClsCommonUtils.ChangeStrToInt("5000");
      //    clsKouyaita.m_putVec = "交互" == "交互" ? PutVec.Kougo : PutVec.Const;//作成方法によって必要性が変わる
      //    clsKouyaita.m_kouyaitaSize = "SP-C3";//コーナー鋼矢板サイズは自動設定の可能性あり
      //    clsKouyaita.m_KougoFlg = false;//交互配置は無いので確定

      //    //設定があるか不明
      //    clsKouyaita.TensetuVec1 = "腹側" == "腹側" ? TensetuVec.HaraSide : TensetuVec.SenakaSide;
      //    //clsKouyaita.TensetuVec2 = "腹側" == "腹側" ? TensetuVec.HaraSide : TensetuVec.SenakaSide;

      //    List<int> lstN1 = new List<int>();
      //    int kuiLength1 = clsKouyaita.m_HLen / clsKouyaita.m_Kasho1;
      //    for (int i = 0; i < clsKouyaita.m_Kasho1; i++)
      //    {
      //        //最終杭長さは余りを全て吸収する
      //        if (i + 1 == clsKouyaita.m_Kasho1)
      //        {
      //            lstN1.Add(clsKouyaita.m_HLen - kuiLength1 * i);
      //        }
      //        else
      //            lstN1.Add(kuiLength1);
      //    }
      //    clsKouyaita.m_ListPileLength1 = lstN1.ToList(); ;

      //    string bolt = "BN-50";
      //    string boltNum = "8";
      //    clsKouyaita.m_BoltF1 = bolt;
      //    clsKouyaita.m_BoltFNum1 = boltNum;
      //    //Lightに設定項目が1つしかないので側やコーナーに関係なく同じと思われる
      //    clsKouyaita.m_BoltW1 = bolt;
      //    clsKouyaita.m_BoltWNum1 = boltNum;
      //    clsKouyaita.m_BoltCornerF1 = bolt;
      //    clsKouyaita.m_BoltCornerFNum1 = boltNum;
      //    clsKouyaita.m_BoltCornerW1 = bolt;
      //    clsKouyaita.m_BoltCornerWNum1 = boltNum;

      //}
      //////////////////////鋼矢板/////////////////////
      //////////////////////親杭/////////////////////
      //if ("親杭" == "親杭")
      //{
      //    /*ここは読み込んだ値*/
      //    ClsOyakui clsOyakui = new ClsOyakui();
      //    clsOyakui.m_case = ClsCommonUtils.ChangeStrToInt("1");
      //    clsOyakui.m_void = ClsCommonUtils.ChangeStrToInt("5000");//横矢板が付く関係か掘削深さが設定できる
      //    clsOyakui.m_refPDist = ClsCommonUtils.ChangeStrToInt("0");
      //    clsOyakui.m_putPitch = ClsCommonUtils.ChangeStrToInt("500");
      //    clsOyakui.m_type = "H形鋼 広幅";
      //    clsOyakui.m_size = "H-300X300X10X15";
      //    clsOyakui.m_HTop = ClsCommonUtils.ChangeStrToInt("0");
      //    clsOyakui.m_HLen = ClsCommonUtils.ChangeStrToInt("10000");
      //    clsOyakui.m_zanti = "なし";
      //    clsOyakui.m_zantiLength = "0";
      //    clsOyakui.m_Kasho1 = ClsCommonUtils.ChangeStrToInt("2");
      //    //clsOyakui.m_Kasho2 = ClsCommonUtils.ChangeStrToInt("2");
      //    clsOyakui.m_Kotei1 = "ボルト" == "ボルト" ? Master.Kotei.Bolt : Master.Kotei.Yousetsu;
      //    //clsOyakui.m_Kotei2 = "ボルト" == "ボルト" ? Master.Kotei.Bolt : Master.Kotei.Yousetsu;
      //    /*ここは読み込んだ値*/

      //    clsOyakui.m_edaNum = "A";
      //    //clsOyakui.m_edaNum2 = "A";
      //    clsOyakui.m_KougoFlg = false;//Lightに交互フラグが無いため確定
      //    clsOyakui.m_way = 1;
      //    clsOyakui.m_putPosFlag = 1;//杭芯

      //    clsOyakui.m_bYokoyaita = false;//Lightで付けられるか不明
      //    clsOyakui.m_typeYokoyaita = "木矢板";
      //    clsOyakui.m_sizeYokoyaita = "木矢板";
      //    clsOyakui.m_putPosYokoyaitaFlag = 0;//内


      //    List<int> lstN1 = new List<int>();
      //    int kuiLength1 = clsOyakui.m_HLen / clsOyakui.m_Kasho1;
      //    for (int i = 0; i < clsOyakui.m_Kasho1; i++)
      //    {
      //        //最終杭長さは余りを全て吸収する
      //        if (i + 1 == clsOyakui.m_Kasho1)
      //        {
      //            lstN1.Add(clsOyakui.m_HLen - kuiLength1 * i);
      //        }
      //        else
      //            lstN1.Add(kuiLength1);
      //    }
      //    clsOyakui.m_ListPileLength1 = lstN1.ToList(); 

      //    string bolt = "BN-50";
      //    string boltNum = "8";
      //    clsOyakui.m_BoltF1 = bolt;
      //    clsOyakui.m_BoltFNum1 = boltNum;
      //    clsOyakui.m_BoltW1 = bolt;
      //    clsOyakui.m_BoltWNum1 = boltNum;
      //}
      //////////////////////親杭/////////////////////
      //////////////////////SMW/////////////////////
      //if("SMW" == "SMW")
      //{
      //    /*ここは読み込んだ値*/
      //    ClsSMW clsSMW = new ClsSMW();
      //    clsSMW.m_case = ClsCommonUtils.ChangeStrToInt("1");
      //    clsSMW.m_zanti = "なし";
      //    clsSMW.m_zantiLength = "0";
      //    clsSMW.m_dia = ClsCommonUtils.ChangeStrToInt("550");
      //    clsSMW.m_soil = ClsCommonUtils.ChangeStrToInt("450");
      //    clsSMW.m_soilTop = ClsCommonUtils.ChangeStrToInt("0");
      //    clsSMW.m_soilLen = ClsCommonUtils.ChangeStrToInt("10000");
      //    clsSMW.m_type = "H形鋼 広幅";
      //    clsSMW.m_size = "H-300X300X10X15";
      //    clsSMW.m_HTop = ClsCommonUtils.ChangeStrToInt("0");
      //    clsSMW.m_HLen = ClsCommonUtils.ChangeStrToInt("10000");
      //    clsSMW.m_Kasho1 = ClsCommonUtils.ChangeStrToInt("2");
      //    //clsSMW.m_Kasho2 = ClsCommonUtils.ChangeStrToInt("2");
      //    clsSMW.m_Kotei1 = "ボルト" == "ボルト" ? Master.Kotei.Bolt : Master.Kotei.Yousetsu;
      //    //clsSMW.m_Kotei2 = "ボルト" == "ボルト" ? Master.Kotei.Bolt : Master.Kotei.Yousetsu;
      //    /*ここは読み込んだ値*/

      //    clsSMW.m_edaNum = "A";
      //    clsSMW.m_edaNum2 = "A";
      //    clsSMW.m_way = 1;
      //    clsSMW.m_void = ClsCommonUtils.ChangeStrToInt("5000");
      //    clsSMW.m_bVoid = false;
      //    clsSMW.m_KougoFlg = false;

      //    //Lightで設定できるか不明
      //    clsSMW.m_refPDist = ClsCommonUtils.ChangeStrToInt("0");
      //    clsSMW.m_bTanbuS = true;
      //    clsSMW.m_bTanbuE = false;
      //    clsSMW.m_bCorner = false;//入隅判定が出来なさそう
      //    clsSMW.m_putPtnFlag = ClsCommonUtils.ChangeStrToInt("10");


      //    List<int> lstN1 = new List<int>();
      //    int kuiLength1 = clsSMW.m_HLen / clsSMW.m_Kasho1;
      //    for (int i = 0; i < clsSMW.m_Kasho1; i++)
      //    {
      //        //最終杭長さは余りを全て吸収する
      //        if (i + 1 == clsSMW.m_Kasho1)
      //        {
      //            lstN1.Add(clsSMW.m_HLen - kuiLength1 * i);
      //        }
      //        else
      //            lstN1.Add(kuiLength1);
      //    }
      //    clsSMW.m_ListPileLength1 = lstN1.ToList();

      //    string bolt = "BN-50";
      //    string boltNum = "8";
      //    clsSMW.m_BoltF1 = bolt;
      //    clsSMW.m_BoltFNum1 = boltNum;
      //    clsSMW.m_BoltW1 = bolt;
      //    clsSMW.m_BoltWNum1 = boltNum;
      //}
      ////////////////////SMW/////////////////////
    }

    public static void CreateTanakui( Document doc )
    {
      List<XYZ> pointList = new List<XYZ>() ;
      ////本目
      //pointList.Add(new XYZ(0, 0, 0));

      ////////////////////壁芯/////////////////////
      //作成した壁芯のIDは保持しておく必要があると思われる
      List<JosnKabe> kabeShinList = new List<JosnKabe>() ;
      //1本目
      pointList.Add( new XYZ( -14.770538101, -0.665480720, 0 ) ) ;

      foreach ( var point in pointList ) {
        var clsTanaKui = new ClsTanakui() ;
        /*ここは読み込んだ値*/
        string levelName = "GL" ;
        clsTanaKui.m_HightFromGL = 0.0 ;
        clsTanaKui.m_KouzaiType = "H形鋼 広幅" ;
        clsTanaKui.m_KouzaiSize = "H-300X300X10X15" ;
        clsTanaKui.m_PileTotalLength = 10000.0 ;
        clsTanaKui.m_PileHaichiAngle = 90 ;
        clsTanaKui.m_CreateKiribariBracket = true ;
        clsTanaKui.m_CreateKiribariOsaeBracket = true ;
        clsTanaKui.m_TsugiteCount = 1 ;
        clsTanaKui.m_FixingType = "ボルト" == "ボルト" ? ClsTanakui.FixingType.Bolt : ClsTanakui.FixingType.Welding ;
        /*ここは読み込んだ値*/

        List<int> lstN1 = new List<int>() ;
        int tsugiteCount = clsTanaKui.m_TsugiteCount + 1 ;
        int kuiLength1 = (int) clsTanaKui.m_PileTotalLength / tsugiteCount ;
        for ( int i = 0 ; i < tsugiteCount ; i++ ) {
          //最終杭長さは余りを全て吸収する
          if ( i + 1 == tsugiteCount ) {
            lstN1.Add( (int) clsTanaKui.m_PileTotalLength - kuiLength1 * i ) ;
          }
          else
            lstN1.Add( kuiLength1 ) ;
        }

        clsTanaKui.m_PileLengthList = lstN1.ToList() ;

        //ブラケットサイズを指定する項目がLightにはないため
        clsTanaKui.m_KiribariBracketSizeIsAuto = true ;
        clsTanaKui.m_KiribariOsaeBracketSizeIsAuto = true ;
        //プレロードとガイド材は作成しないと思われるため
        clsTanaKui.m_CreatePreloadGuide = false ;
        clsTanaKui.m_CreateGuide = false ;
        //Lightではずれ量を考慮した位置に配置されているためこちらでは1点配置に固定する
        clsTanaKui.m_CreateType = ClsTanakui.CreateType.OnePoint ;

        string bolt = "BN-50" ;
        string boltNum = "8" ;
        clsTanaKui.m_TsugiteBoltSize_Flange = bolt ;
        clsTanaKui.m_TsugiteBoltQuantity_Flange = boltNum ;
        clsTanaKui.m_TsugiteBoltSize_Web = bolt ;
        clsTanaKui.m_TsugiteBoltQuantity_Web = boltNum ;

        ElementId levelId = ClsRevitUtil.GetLevelID( doc, levelName ) ;
        string uniqueName = string.Empty ;
        clsTanaKui.CreateTanaKui( doc, point, levelId, false, ref uniqueName ) ;
        // 切梁ブラケットも同時に作成する場合
        if ( clsTanaKui.m_CreateKiribariBracket ) {
          ClsBracket clsBracket = new ClsBracket() ;
          clsBracket.m_BracketSize = clsTanaKui.m_KiribariBracketSize ;
          clsBracket.m_TargetKuiAngle = clsTanaKui.m_PileHaichiAngle ;
          clsBracket.m_TargetKuiCreatePosition = clsTanaKui.m_CreatePosition ;
          clsBracket.m_TargetKuiCreateType = clsTanaKui.m_CreateType ;
          if ( clsBracket.CreateKiribariBracket( doc, clsTanaKui.m_CreateId,
                clsTanaKui.m_KiribariBracketSizeIsAuto ) ) {
            // プレロードガイド材も作成する場合
            if ( clsTanaKui.m_CreatePreloadGuide ) {
              for ( int i = 0 ; i < clsBracket.m_CreateIds.Count() ; i++ ) {
                clsBracket.CreatePreloadGuideZai( doc, clsBracket.m_CreateIds[ i ],
                  clsBracket.m_TargetKiribariBaseIds[ i ] ) ;
              }
            }
          }
        }

        // 切梁押えブラケットも同時に作成する場合
        if ( clsTanaKui.m_CreateKiribariOsaeBracket ) {
          ClsBracket clsBracket = new ClsBracket() ;
          clsBracket.m_BracketSize = clsTanaKui.m_KiribariOsaeBracketSize ;
          clsBracket.m_TargetKuiAngle = clsTanaKui.m_PileHaichiAngle ;
          clsBracket.m_TargetKuiCreatePosition = clsTanaKui.m_CreatePosition ;
          clsBracket.m_TargetKuiCreateType = clsTanaKui.m_CreateType ;
          if ( clsBracket.CreateKiribariOsaeBracket( doc, clsTanaKui.m_CreateId,
                clsTanaKui.m_KiribariOsaeBracketSizeIsAuto ) ) {
            // ガイド材も作成する場合※
            if ( clsTanaKui.m_CreateGuide ) {
              for ( int i = 0 ; i < clsBracket.m_CreateIds.Count() ; i++ ) {
                clsBracket.CreateGuideZai( doc, clsBracket.m_CreateIds[ i ], clsBracket.m_TargetKiribariBaseIds[ i ] ) ;
              }
            }
          }
        }
      }

      pointList.Clear() ;
    }

    #endregion

    /// <summary>
    /// WorldPositionを求める
    /// </summary>
    /// <param name="matrix"></param>
    /// <param name="matrix1"></param>
    /// <param name="matrix2"></param>
    /// <param name="array"></param>
    /// <param name="arrayNum"></param>
    /// <returns></returns>
    private static XYZ GetWorldPosition( List<double> matrix, List<double> matrix1, List<double> matrix2,
      List<double> array, int arrayNum = 0 )
    {
      try {
        arrayNum *= 3 ;
        if ( matrix.Count != 16 || matrix1.Count != 16 || matrix2.Count != 16 || array.Count < arrayNum + 2 )
          return null ;
        var arrayX = array[ arrayNum ] ;
        var arrayY = array[ arrayNum + 1 ] ;
        var arrayZ = array[ arrayNum + 2 ] ;

        var worldPositionX = matrix[ 0 ] * arrayX + matrix[ 4 ] * arrayY + matrix[ 8 ] * arrayZ + matrix[ 12 ] * 1 ;
        var worldPositionY = matrix[ 1 ] * arrayX + matrix[ 5 ] * arrayY + matrix[ 9 ] * arrayZ + matrix[ 13 ] * 1 ;
        var worldPositionZ = matrix[ 2 ] * arrayX + matrix[ 6 ] * arrayY + matrix[ 10 ] * arrayZ + matrix[ 14 ] * 1 ;
        var worldPositionFouth =
          matrix[ 3 ] * arrayX + matrix[ 7 ] * arrayY + matrix[ 11 ] * arrayZ + matrix[ 15 ] * 1 ;

        worldPositionX = matrix1[ 0 ] * worldPositionX + matrix1[ 4 ] * worldPositionY + matrix1[ 8 ] * worldPositionZ +
                         matrix1[ 12 ] * worldPositionFouth ;
        worldPositionY = matrix1[ 1 ] * worldPositionX + matrix1[ 5 ] * worldPositionY + matrix1[ 9 ] * worldPositionZ +
                         matrix1[ 13 ] * worldPositionFouth ;
        worldPositionZ = matrix1[ 2 ] * worldPositionX + matrix1[ 6 ] * worldPositionY +
                         matrix1[ 10 ] * worldPositionZ + matrix1[ 14 ] * worldPositionFouth ;
        worldPositionFouth = matrix1[ 3 ] * worldPositionX + matrix1[ 7 ] * worldPositionY +
                             matrix1[ 11 ] * worldPositionZ + matrix1[ 15 ] * worldPositionFouth ;

        worldPositionX = matrix2[ 0 ] * worldPositionX + matrix2[ 4 ] * worldPositionY + matrix2[ 8 ] * worldPositionZ +
                         matrix2[ 12 ] * worldPositionFouth ;
        worldPositionY = matrix2[ 1 ] * worldPositionX + matrix2[ 5 ] * worldPositionY + matrix2[ 9 ] * worldPositionZ +
                         matrix2[ 13 ] * worldPositionFouth ;
        worldPositionZ = matrix2[ 2 ] * worldPositionX + matrix2[ 6 ] * worldPositionY +
                         matrix2[ 10 ] * worldPositionZ + matrix2[ 14 ] * worldPositionFouth ;
        worldPositionFouth = matrix2[ 3 ] * worldPositionX + matrix2[ 7 ] * worldPositionY +
                             matrix2[ 11 ] * worldPositionZ + matrix2[ 15 ] * worldPositionFouth ;

        //var worldPosition = new XYZ(ClsRevitUtil.CovertToAPI(matrix[12] + worldPositionX), ClsRevitUtil.CovertToAPI(matrix[13] + worldPositionY), ClsRevitUtil.CovertToAPI(matrix1[14] + worldPositionZ));
        var worldPosition = new XYZ( ClsRevitUtil.CovertToAPI( worldPositionX ),
          ClsRevitUtil.CovertToAPI( worldPositionY ), ClsRevitUtil.CovertToAPI( worldPositionZ ) ) ;

        return worldPosition ;
      }
      catch {
        return null ;
      }
    }
  }

  public class JosnKabe
  {
    public ElementId m_KabeShin { get ; set ; }

    public string m_KabeType { get ; set ; }

    public JosnKabe( ElementId kabeShin, string kabeType )
    {
      m_KabeShin = kabeShin ;
      m_KabeType = kabeType ;
    }
  }

  #region JSONDataClass

  public class JosnData
  {
    [JsonProperty( "metaData" )]
    public MetaData MetaData { get ; set ; }

    [JsonProperty( "rootLayers" )]
    public List<RootLayers> RootLayers { get ; set ; }
  }

  /// <summary>
  /// 作成情報
  /// </summary>
  public class MetaData
  {
    [JsonProperty( "version" )]
    public string version { get ; set ; }

    [JsonProperty( "created_at" )]
    public string created_at { get ; set ; }

    [JsonProperty( "updated_at" )]
    public string updated_at { get ; set ; }

    [JsonProperty( "created_by" )]
    public string created_by { get ; set ; }

    [JsonProperty( "updated_by" )]
    public string updated_by { get ; set ; }
  }

  /// <summary>
  /// メインのList
  /// </summary>
  public class RootLayers
  {
    [JsonProperty( "metadata" )]
    public Metadata Metadata { get ; set ; }

    [JsonProperty( "geometries" )]
    public List<Geometries> Geometries { get ; set ; }

    [JsonProperty( "object" )]
    public Object Object { get ; set ; }
  }

  public class Metadata
  {
    [JsonProperty( "version" )]
    public double version { get ; set ; }

    [JsonProperty( "type" )]
    public string type { get ; set ; }

    [JsonProperty( "generator" )]
    public string generator { get ; set ; }
  }

  public class Geometries
  {
    [JsonProperty( "uuid" )]
    public string uuid { get ; set ; }

    [JsonProperty( "type" )]
    public string type { get ; set ; }

    [JsonProperty( "data" )]
    public Data data { get ; set ; }
  }

  public class Data
  {
    [JsonProperty( "attributes" )]
    public Attributes Attributes { get ; set ; }

    [JsonProperty( "boundingSphere" )]
    public BoundingSphere BoundingSphere { get ; set ; }
  }

  public class Attributes
  {
    [JsonProperty( "position" )]
    public Position Position { get ; set ; }
  }

  public class Position
  {
    [JsonProperty( "itemSize" )]
    public int itemSize { get ; set ; }

    [JsonProperty( "type" )]
    public string type { get ; set ; }

    /// <summary>
    /// 配置点3個ずつで一区切りX,Y,Zの順で入っている※始点3終点3の順番
    /// </summary>
    [JsonProperty( "array" )]
    public List<double> array { get ; set ; }

    [JsonProperty( "normalized" )]
    public bool normalized { get ; set ; }
  }

  public class BoundingSphere
  {
    /// <summary>
    /// 中心点3個ずつで一区切りX,Y,Zの順で入っている
    /// </summary>
    [JsonProperty( "center" )]
    public List<double> center { get ; set ; }

    [JsonProperty( "radius" )]
    public double radius { get ; set ; }
  }

  public class Object
  {
    [JsonProperty( "uuid" )]
    public string uuid { get ; set ; }

    [JsonProperty( "type" )]
    public string type { get ; set ; }

    [JsonProperty( "name" )]
    public string name { get ; set ; }

    [JsonProperty( "layers" )]
    public double layers { get ; set ; }

    [JsonProperty( "matrix" )]
    public List<double> matrix { get ; set ; }

    [JsonProperty( "up" )]
    public List<int> up { get ; set ; }

    [JsonProperty( "children" )]
    public List<Children> children { get ; set ; }

    [JsonProperty( "userData" )]
    public UserData UserData { get ; set ; }
  }

  public class Children
  {
    [JsonProperty( "uuid" )]
    public string uuid { get ; set ; }

    [JsonProperty( "type" )]
    public string type { get ; set ; }

    [JsonProperty( "name" )]
    public string name { get ; set ; }

    [JsonProperty( "layers" )]
    public double layers { get ; set ; }

    [JsonProperty( "matrix" )]
    public List<double> matrix { get ; set ; }

    [JsonProperty( "up" )]
    public List<double> up { get ; set ; }

    [JsonProperty( "geometry" )]
    public string geometry { get ; set ; }

    [JsonProperty( "material" )]
    public string material { get ; set ; }

    [JsonProperty( "children" )]
    public List<Children> children { get ; set ; }

    [JsonProperty( "userData" )]
    public UserData UserData { get ; set ; }
  }

  public class UserData
  {
    //[JsonProperty("userData")]//同名なのと入るデータが不明
    //public UserData userData { get; set; }
    [JsonProperty( "customData" )]
    public CustomData CustomData { get ; set ; }
  }

  public class CustomData
  {
    [JsonProperty( "type" )]
    public string type { get ; set ; }

    [JsonProperty( "name" )]
    public string name { get ; set ; }

    [JsonProperty( "lineWidth" )]
    public int lineWidth { get ; set ; }

    [JsonProperty( "color" )]
    public string color { get ; set ; }

    [JsonProperty( "height" )]
    public double height { get ; set ; }

    [JsonProperty( "processingMethod" )]
    public string processingMethod { get ; set ; }

    /// <summary>
    /// 鋼材種類※主材、高強度など
    /// </summary>
    [JsonProperty( "steelType" )]
    public string steelType { get ; set ; }

    /// <summary>
    /// 主材サイズ
    /// </summary>
    [JsonProperty( "steelSize" )]
    public string steelSize { get ; set ; }

    /// <summary>
    /// 段
    /// </summary>
    [JsonProperty( "stage" )]
    public string stage { get ; set ; }

    /// <summary>
    /// 横本数※腹起
    /// </summary>
    [JsonProperty( "horizontalBraceCount" )]
    public string horizontalBraceCount { get ; set ; }

    /// <summary>
    /// 縦本数※腹起
    /// </summary>
    [JsonProperty( "verticalBraceCount" )]
    public string verticalBraceCount { get ; set ; }

    [JsonProperty( "slipStopPointsCount" )]
    public string slipStopPointsCount { get ; set ; }

    /// <summary>
    /// 壁で使用
    /// </summary>
    [JsonProperty( "case" )]
    public string Case { get ; set ; }

    /// <summary>
    /// 接続しているベースのID
    /// </summary>
    [JsonProperty( "connectedLines" )]
    public List<string> connectedLines { get ; set ; }

    /// <summary>
    /// 始点側端部部品※切梁
    /// </summary>
    [JsonProperty( "endPartStartPoint" )]
    public string endPartStartPoint { get ; set ; }

    /// <summary>
    /// 終点側端部部品※切梁
    /// </summary>
    [JsonProperty( "endPartEndPoint" )]
    public string endPartEndPoint { get ; set ; }

    /// <summary>
    /// 隅火打と切梁火打端部部品
    /// </summary>
    [JsonProperty( "componentName" )]
    public string componentName { get ; set ; }

    /// <summary>
    /// 角度※切梁火打
    /// </summary>
    [JsonProperty( "angle" )]
    public double angle { get ; set ; }

    /// <summary>
    /// 接続ベース情報※切梁、隅火打、切梁火打
    /// </summary>
    [JsonProperty( "supportObjects" )]
    public List<SupportObjects> supportObjects { get ; set ; }

    /// <summary>
    /// 取付方法※切梁繋ぎ、火打繋ぎ
    /// </summary>
    [JsonProperty( "installationMethod" )]
    public string installationMethod { get ; set ; }

    /// <summary>
    /// ボルト種類※切梁繋ぎ、火打繋ぎ
    /// </summary>
    [JsonProperty( "boltType" )]
    public string boltType { get ; set ; }

    /// <summary>
    /// ジャッキ種類
    /// </summary>
    [JsonProperty( "jackType" )]
    public string jackType { get ; set ; }

    /// <summary>
    /// 配置位置※棚杭
    /// </summary>
    [JsonProperty( "placementPosition" )]
    public string placementPosition { get ; set ; }

    /// <summary>
    /// ずれ量X※棚杭
    /// </summary>
    [JsonProperty( "amountOfDriftX" )]
    public double amountOfDriftX { get ; set ; }

    /// <summary>
    /// ずれ量Y※棚杭
    /// </summary>
    [JsonProperty( "amountOfDriftY" )]
    public double amountOfDriftY { get ; set ; }

    /// <summary>
    /// 配置角度※棚杭
    /// </summary>
    [JsonProperty( "placementAngle" )]
    public double placementAngle { get ; set ; }

    /// <summary>
    /// 基準からの高さ
    /// </summary>
    [JsonProperty( "pileTopEdge" )]
    public double pileTopEdge { get ; set ; }

    /// <summary>
    /// 杭全長
    /// </summary>
    [JsonProperty( "totalLength" )]
    public double totalLength { get ; set ; }

    /// <summary>
    /// 切梁ブラケット同時作成
    /// </summary>
    [JsonProperty( "placingCutBeamBracket" )]
    public bool placingCutBeamBracket { get ; set ; }

    /// <summary>
    /// 切梁押えブラケット同時作成
    /// </summary>
    [JsonProperty( "placingCutBeamHoldingBracket" )]
    public bool placingCutBeamHoldingBracket { get ; set ; }

    /// <summary>
    /// 個所数
    /// </summary>
    [JsonProperty( "numberOfJoints" )]
    public double numberOfJoints { get ; set ; }

    /// <summary>
    /// 固定方法
    /// </summary>
    [JsonProperty( "jointMethod" )]
    public string jointMethod { get ; set ; }

    /// <summary>
    /// 指定位置（=配置点ではない)※棚杭
    /// </summary>
    [JsonProperty( "centerPoint" )]
    public CenterPoint centerPoint { get ; set ; }

    #region 壁

    /// <summary>
    /// 杭全長※鋼矢板、親杭
    /// </summary>
    [JsonProperty( "pileLength" )]
    public double pileLength { get ; set ; }

    /// <summary>
    /// 異形矢板使用
    /// </summary>
    [JsonProperty( "useDeformedSheetPile" )]
    public string useDeformedSheetPile { get ; set ; }

    /// <summary>
    /// 材質
    /// </summary>
    [JsonProperty( "materialQuality" )]
    public string materialQuality { get ; set ; }

    /// <summary>
    /// 残置
    /// </summary>
    [JsonProperty( "remain" )]
    public string remain { get ; set ; }

    /// <summary>
    /// 残置長さ
    /// </summary>
    [JsonProperty( "remainingLength" )]
    public double remainingLength { get ; set ; }

    /// <summary>
    /// 掘削深さ
    /// </summary>
    [JsonProperty( "excavationDepth" )]
    public double excavationDepth { get ; set ; }

    /// <summary>
    /// 頭ツナギ鋼材タイプ
    /// </summary>
    [JsonProperty( "headWearInfoSteelType" )]
    public string headWearInfoSteelType { get ; set ; }

    /// <summary>
    /// 頭ツナギ鋼材サイズ
    /// </summary>
    [JsonProperty( "headWearInfoSteelSize" )]
    public string headWearInfoSteelSize { get ; set ; }

    /// <summary>
    /// 頭ツナギチャンネル材の向き
    /// </summary>
    [JsonProperty( "headWearChannelMaterialOrientation" )]
    public string headWearChannelMaterialOrientation { get ; set ; }

    /// <summary>
    /// 頭ツナギ配置方向
    /// </summary>
    [JsonProperty( "headWearPlacementOrientation" )]
    public string headWearPlacementOrientation { get ; set ; }

    /// <summary>
    /// 頭ツナギ接合方法
    /// </summary>
    [JsonProperty( "headWearJoiningMethod" )]
    public string headWearJoiningMethod { get ; set ; }

    /// <summary>
    /// ボルトタイプ
    /// </summary>
    [JsonProperty( "headWearBoltType" )]
    public string headWearBoltType { get ; set ; }

    /// <summary>
    /// ボルトサイズ
    /// </summary>
    [JsonProperty( "headWearBoltSize" )]
    public string headWearBoltSize { get ; set ; }

    /// <summary>
    /// ピッチ
    /// </summary>
    [JsonProperty( "placementPitch" )]
    public double placementPitch { get ; set ; }

    /// <summary>
    /// ソイル径
    /// </summary>
    [JsonProperty( "soilDiameter" )]
    public double soilDiameter { get ; set ; }

    /// <summary>
    /// ソイルピッチ
    /// </summary>
    [JsonProperty( "soilPitch" )]
    public double soilPitch { get ; set ; }

    /// <summary>
    /// ソイル天端
    /// </summary>
    [JsonProperty( "soilTopEdge" )]
    public double soilTopEdge { get ; set ; }

    /// <summary>
    /// ソイル全長
    /// </summary>
    [JsonProperty( "soilTotalLength" )]
    public double soilTotalLength { get ; set ; }

    /// <summary>
    /// ソイル掘削深さ
    /// </summary>
    [JsonProperty( "soilExcavationDepth" )]
    public double soilExcavationDepth { get ; set ; }

    /// <summary>
    /// SMW杭タイプ
    /// </summary>
    [JsonProperty( "coreMaterialType" )]
    public string coreMaterialType { get ; set ; }

    /// <summary>
    /// SMW杭タイプ
    /// </summary>
    [JsonProperty( "coreMaterialSize" )]
    public string coreMaterialSize { get ; set ; }

    /// <summary>
    /// SMW杭天端
    /// </summary>
    [JsonProperty( "coreMaterialTopEdge" )]
    public double coreMaterialTopEdge { get ; set ; }

    /// <summary>
    /// SMW杭全長
    /// </summary>
    [JsonProperty( "coreMaterialTotalLength" )]
    public double coreMaterialTotalLength { get ; set ; }

    /// <summary>
    /// SMW入隅コーナーに芯材を配置
    /// </summary>
    [JsonProperty( "coreMaterialPlacedAtTheEntryCorner" )]
    public string coreMaterialPlacedAtTheEntryCorner { get ; set ; }

    /// <summary>
    /// SMW杭配置パターン
    /// </summary>
    [JsonProperty( "coreMaterialPlacedPattern" )]
    public string coreMaterialPlacedPattern { get ; set ; }

    #endregion
  }

  public class SupportObjects
  {
    /// <summary>
    /// 段
    /// </summary>
    [JsonProperty( "stage" )]
    public string stage { get ; set ; }

    [JsonProperty( "steelSize" )]
    public string steelSize { get ; set ; }

    [JsonProperty( "steelType" )]
    public string steelType { get ; set ; }

    /// <summary>
    /// 縦本数※腹起
    /// </summary>
    [JsonProperty( "verticalBraceCount" )]
    public string verticalBraceCount { get ; set ; }
  }

  public class CenterPoint
  {
    [JsonProperty( "x" )]
    public double x { get ; set ; }

    [JsonProperty( "y" )]
    public double y { get ; set ; }

    [JsonProperty( "z" )]
    public double z { get ; set ; }
  }

  //public class

  #endregion
}