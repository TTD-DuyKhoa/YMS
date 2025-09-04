using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using Autodesk.Revit.UI.Selection ;
using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;
using YMS.Parts ;

namespace YMS.Command
{
  class ClsCommandKiribariTsunagizaiBase
  {
    public static void CommandKiribariTsunagizaiBase( UIDocument uidoc )
    {
      //ドキュメントを取得
      Document doc = uidoc.Document ;

      ////ワークセット
      //ClsWorkset clsWS = new ClsWorkset();
      //clsWS.SetWorkSetTest1(doc);

      //ダイアログを表示
      YMS.DLG.DlgCreateKiribariTsunagizaiBase KTB = new DLG.DlgCreateKiribariTsunagizaiBase() ;
      DialogResult result = KTB.ShowDialog() ;
      if ( result == DialogResult.Cancel ) {
        return ;
      }

      ClsKiribariTsunagizaiBase clsKTB = KTB.m_KiribariTsunagizaiBase ;

      for ( ; ; ) {
        //作図に必要な作図箇所の座標を取得
        List<ElementId> ids = new List<ElementId>() ;
        ElementId levelID = null ;
        try {
          if ( clsKTB.m_ShoriType == ClsKiribariTsunagizaiBase.ShoriType.Replace ) {
            //切梁1本目
            ElementId idKiri = null ;
            if ( ! ClsKiribariBase.PickBaseObject( uidoc, ref idKiri, "レベルを合わせる切梁ベース" ) ) {
              return ;
            }

            FamilyInstance inst = doc.GetElement( idKiri ) as FamilyInstance ;
            levelID = inst.Host.Id ;
            if ( ! ClsRevitUtil.PickObjects( uidoc, "切梁繋ぎ材ベースに置換するモデル線分", "モデル線分", ref ids ) ) {
              return ;
            }

            if ( ids.Count < 1 ) {
              MessageBox.Show( "モデル線分が選択されていません。" ) ;
              return ;
            }

            //置換の処理をここに記述
            //MessageBox.Show("置換の処理は現在実装中です。");
          }
          else {
            List<ElementId> filterList = new List<ElementId>() ;
            //切梁1本目
            ElementId idKiri1 = null ;
            if ( ! ClsKiribariBase.PickBaseObject( uidoc, ref idKiri1 ) ) {
              return ;
            }

            filterList.Add( idKiri1 ) ;
            FamilyInstance inst1 = doc.GetElement( idKiri1 ) as FamilyInstance ;

            levelID = inst1.Host.Id ;


            //切梁2本目
            ElementId idKiri2 = null ;
            if ( ! ClsKiribariBase.PickBaseObject( uidoc, ref idKiri2 ) ) {
              return ;
            }

            filterList.Add( idKiri2 ) ;
            FamilyInstance inst2 = doc.GetElement( idKiri2 ) as FamilyInstance ;


            //片方のベース上から1点を指定
            Reference rfPick = null ;
            if ( ! ClsRevitUtil.PickObjectElementFilter( uidoc, "切梁ベース上で配置点を指定してください", filterList, ref rfPick ) ) {
              return ;
            }

            XYZ picPoint = rfPick.GlobalPoint ;
            filterList.Remove( rfPick.ElementId ) ;

            Element selKiri = doc.GetElement( rfPick.ElementId ) ;

            Element notSelKiri = doc.GetElement( filterList[ 0 ] ) ;

            //選択された切梁
            inst1 = selKiri as FamilyInstance ;
            Curve cvKiri1 = ( inst1.Location as LocationCurve ).Curve ;
            XYZ dirKiri1 = Line.CreateBound( cvKiri1.GetEndPoint( 0 ), cvKiri1.GetEndPoint( 1 ) ).Direction ;
            XYZ facOriKiri1 = inst1.FacingOrientation ;
            //切梁の鋼材サイズを取得する//とりあえず300にしているがベースから鋼材サイズを取得する
            double dKiriSize1 = 300.0 / 2 ;


            //選択されなかった切梁
            inst2 = notSelKiri as FamilyInstance ;
            Curve cvKiri2 = ( inst2.Location as LocationCurve ).Curve ;
            //切梁の鋼材サイズを取得する//とりあえず300にしているがベースから鋼材サイズを取得する
            double dKiriSize2 = 300.0 / 2 ;

            //選択されなかった切梁の中点を取得
            XYZ midKiri2 = ( cvKiri2.GetEndPoint( 0 ) + cvKiri2.GetEndPoint( 1 ) ) / 2 ;

            //選択した切梁から見て選択していない切梁の中点は左にあるかで向きを決める
            XYZ dir1toMid = Line.CreateBound( cvKiri1.GetEndPoint( 0 ), midKiri2 ).Direction ;
            if ( ClsGeo.IsLeft( dirKiri1, dir1toMid ) ) {
              //左に90度
              dirKiri1 = new XYZ( facOriKiri1.X, facOriKiri1.Y, 0 ) ;
            }
            else {
              //右に90度
              dirKiri1 = new XYZ( -facOriKiri1.X, -facOriKiri1.Y, 0 ) ;
            }

            XYZ insec = null ;
            //for (int i = 1; i < int.MaxValue; i++)
            //{
            //    //500増やすかは未定
            //    XYZ newPoint = new XYZ(picPoint.X + (ClsRevitUtil.CovertToAPI(i * 500) * dirKiri1.X),
            //                           picPoint.Y + (ClsRevitUtil.CovertToAPI(i * 500) * dirKiri1.Y),
            //                           picPoint.Z + (ClsRevitUtil.CovertToAPI(i * 500) * dirKiri1.Z));

            //    Curve cvBase = Line.CreateBound(picPoint, newPoint);
            //    insec = ClsRevitUtil.GetIntersection(cvKiri2, cvBase);
            //    if (insec != null)
            //    {
            //        break;
            //    }
            //}
            XYZ newPoint = new XYZ( picPoint.X + ( ClsRevitUtil.CovertToAPI( int.MaxValue ) * dirKiri1.X ),
              picPoint.Y + ( ClsRevitUtil.CovertToAPI( int.MaxValue ) * dirKiri1.Y ),
              picPoint.Z + ( ClsRevitUtil.CovertToAPI( int.MaxValue ) * dirKiri1.Z ) ) ;

            Curve cvBase = Line.CreateBound( picPoint, newPoint ) ;
            insec = ClsRevitUtil.GetIntersection( cvKiri2, cvBase ) ;
            if ( insec == null ) {
              MessageBox.Show( "交点が存在しません" ) ;
              return ; //交点が存在しない
            }

            Line kariLine = Line.CreateBound( picPoint, insec ) ;
            dirKiri1 = kariLine.Direction ;
            insec = kariLine.GetEndPoint( 1 ) ;
            double dKariLine = ClsRevitUtil.CovertFromAPI( kariLine.Length ) ;
            for ( int i = 0 ; i < int.MaxValue ; i++ ) {
              double d = 500.0 * i - dKariLine ;
              if ( 0 < d ) {
                if ( d <= 100.0 + dKiriSize1 + dKiriSize2 ) {
                  i += 1 ;
                }

                d = 500.0 * i - dKariLine - dKiriSize1 - dKiriSize2 ;
                picPoint = new XYZ( picPoint.X - ( ClsRevitUtil.CovertToAPI( dKiriSize1 + d / 2 ) * dirKiri1.X ),
                  picPoint.Y - ( ClsRevitUtil.CovertToAPI( dKiriSize1 + d / 2 ) * dirKiri1.Y ),
                  picPoint.Z - ( ClsRevitUtil.CovertToAPI( dKiriSize1 + d / 2 ) * dirKiri1.Z ) ) ;
                insec = new XYZ( insec.X + ( ClsRevitUtil.CovertToAPI( dKiriSize2 + d / 2 ) * dirKiri1.X ),
                  insec.Y + ( ClsRevitUtil.CovertToAPI( dKiriSize2 + d / 2 ) * dirKiri1.Y ),
                  insec.Z + ( ClsRevitUtil.CovertToAPI( dKiriSize2 + d / 2 ) * dirKiri1.Z ) ) ;
                break ;
              }
            }

            if ( clsKTB.m_SplitFlg ) {
              Curve cvKari = Line.CreateBound( picPoint, insec ) ;
              List<ElementId> kiriList = ClsKiribariBase.GetAllKiribariBaseList( doc ) ;
              XYZ p1 = picPoint ;
              XYZ p2 ;
              foreach ( ElementId kiriId in kiriList ) {
                if ( kiriId == idKiri1 || kiriId == idKiri2 ) continue ;

                FamilyInstance inst = doc.GetElement( kiriId ) as FamilyInstance ;
                Curve cv = ( inst.Location as LocationCurve ).Curve ;
                p2 = ClsRevitUtil.GetIntersection( cvKari, cv ) ;
                if ( p2 != null ) {
                  using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
                    t.Start() ;
                    ModelLine line = ClsYMSUtil.CreateKabeHojyoLine( doc, p1, p2, 0, 0 ) ;
                    t.Commit() ;
                    ids.Add( line.Id ) ;
                  }

                  p1 = p2 ;
                }
              }

              p2 = insec ;
              using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
                t.Start() ;
                ModelLine line = ClsYMSUtil.CreateKabeHojyoLine( doc, p1, p2, 0, 0 ) ;
                t.Commit() ;
                ids.Add( line.Id ) ;
              }
            }
            else {
              using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
                t.Start() ;
                ModelLine line = ClsYMSUtil.CreateKabeHojyoLine( doc, picPoint, insec, 0, 0 ) ;
                t.Commit() ;
                ids.Add( line.Id ) ;
              }
            }

            //自動配置の処理をここに記述
            //MessageBox.Show("自動配置の処理は現在実装中です。");
          }


          //切梁受けベースを作図
          if ( ! clsKTB.CreateKiribariTsunagizaiBase( doc, ids, levelID ) ) {
            //作図に失敗した時のコメント
          }

          //補助線を削除
          using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
            t.Start() ;
            ClsRevitUtil.Delete( doc, ids ) ;
            t.Commit() ;
          }

          if ( clsKTB.m_ShoriType == ClsKiribariTsunagizaiBase.ShoriType.Replace )
            break ;
        }
        catch {
          break ;
        }
      }

      return ;
    }

    public static bool CommandChangeKiribariTsunagizaiBase( UIDocument uidoc )
    {
      Document doc = uidoc.Document ;

      // 対象となるベースを複数選択
      List<ElementId> idList = null ;
      if ( ! ClsKiribariTsunagizaiBase.PickBaseObjects( uidoc, ref idList ) ) {
        return false ;
      }

      ClsKiribariTsunagizaiBase templateBase = new ClsKiribariTsunagizaiBase() ;
      for ( int i = 0 ; i < idList.Count() ; i++ ) {
        ElementId id = idList[ i ] ;
        if ( i == 0 ) {
          templateBase.SetParameter( doc, id ) ;
        }
        else {
          FamilyInstance shinInstLevel = doc.GetElement( id ) as FamilyInstance ;
          ElementId levelID = shinInstLevel.Host.Id ;
          LocationCurve lCurve = shinInstLevel.Location as LocationCurve ;
          if ( lCurve == null ) {
            return false ;
          }

          //元データを取得
          templateBase.m_SteelType =
            ClsRevitUtil.CompareValues( ClsRevitUtil.GetParameter( doc, id, "分類" ), templateBase.m_SteelType ) ;
          templateBase.m_SteelSize =
            ClsRevitUtil.CompareValues( ClsRevitUtil.GetParameter( doc, id, "サイズ" ), templateBase.m_SteelSize ) ;
          templateBase.m_ToritsukeHoho = ClsRevitUtil.CompareValues(
            templateBase.GetToritsukeHoho( ClsRevitUtil.GetParameter( doc, id, "取付方法" ) ),
            templateBase.m_ToritsukeHoho ) ;
          templateBase.m_BoltType1 = ClsRevitUtil.CompareValues( ClsRevitUtil.GetParameter( doc, id, "ボルトタイプ1" ),
            templateBase.m_BoltType1 ) ;
          templateBase.m_BoltType2 = ClsRevitUtil.CompareValues( ClsRevitUtil.GetParameter( doc, id, "ボルトタイプ2" ),
            templateBase.m_BoltType2 ) ;
          templateBase.m_BoltNum = ClsRevitUtil.CompareValues( ClsRevitUtil.GetParameterInteger( doc, id, "ボルト本数" ),
            templateBase.m_BoltNum ) ;
        }
      }

      //ダイアログを表示
      DLG.DlgCreateKiribariTsunagizaiBase kiribariTsunagizaiBase =
        new DLG.DlgCreateKiribariTsunagizaiBase( templateBase ) ;
      DialogResult result = kiribariTsunagizaiBase.ShowDialog() ;
      if ( result != DialogResult.OK ) {
        return false ;
      }

      foreach ( var id in idList ) {
        ClsKiribariTsunagizaiBase clsKiribariTsunagizaiBase = kiribariTsunagizaiBase.m_KiribariTsunagizaiBase ;

        // 始点と終点を取得
        FamilyInstance shinInstLevel = doc.GetElement( id ) as FamilyInstance ;
        ElementId levelID = shinInstLevel.Host.Id ;
        LocationCurve lCurve = shinInstLevel.Location as LocationCurve ;
        if ( lCurve == null ) {
          return false ;
        }

        XYZ tmpStPoint = lCurve.Curve.GetEndPoint( 0 ) ;
        XYZ tmpEdPoint = lCurve.Curve.GetEndPoint( 1 ) ;

        string dan = ClsRevitUtil.GetParameter( doc, id, "段" ) ;

        //分割がある場合の処理
        ( List<XYZ> stList, List<XYZ> edList ) =
          clsKiribariTsunagizaiBase.ChangeSplitKiribariTsunagizaiBase( doc, id ) ;

        //元のベースを削除
        using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
          t.Start() ;
          ClsRevitUtil.Delete( doc, id ) ;
          t.Commit() ;
        }

        //ベース変更
        for ( int i = 0 ; i < stList.Count ; i++ ) {
          tmpStPoint = stList[ i ] ;
          tmpEdPoint = edList[ i ] ;
          clsKiribariTsunagizaiBase.ChangeKiribariTsunagizaiBase( doc, tmpStPoint, tmpEdPoint, dan, levelID ) ;
        }
      }

      return true ;
    }
  }
}