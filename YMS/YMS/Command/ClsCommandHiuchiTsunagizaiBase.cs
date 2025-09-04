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
  class ClsCommandHiuchiTsunagizaiBase
  {
    public static void CommandHiuchiTsunagizaiBase( UIDocument uidoc )
    {
      //ドキュメントを取得
      Document doc = uidoc.Document ;

      ////ワークセット
      //ClsWorkset clsWS = new ClsWorkset();
      //clsWS.SetWorkSetTest1(doc);

      //ダイアログを表示
      YMS.DLG.DlgCreateHiuchiTsunagizaiBase HTB = new DLG.DlgCreateHiuchiTsunagizaiBase() ;
      DialogResult result = HTB.ShowDialog() ;
      if ( result == DialogResult.Cancel ) {
        return ;
      }

      ClsHiuchiTsunagizaiBase clsHTB = HTB.m_HiuchiTsunagizaiBase ;

      for ( ; ; ) {
        //作図に必要な作図箇所の座標を取得
        List<ElementId> ids = new List<ElementId>() ;
        ElementId levelID = null ;
        try {
          if ( clsHTB.m_ShoriType == ClsHiuchiTsunagizaiBase.ShoriType.Replace ) {
            //レベルを合わせる火打ベースを選択
            ElementId idHiuchi = null ;
            if ( ! ClsRevitUtil.PickObjectFilters( uidoc, "レベルを合わせる火打ベース", ClsGlobal.m_baseHiuchiList,
                  ref idHiuchi ) ) {
              return ;
            }

            FamilyInstance inst = doc.GetElement( idHiuchi ) as FamilyInstance ;
            levelID = inst.Host.Id ;
            if ( ! ClsRevitUtil.PickObjects( uidoc, "火打繋ぎ材ベースに置換するモデル線分", "モデル線分", ref ids ) ) {
              return ;
            }

            if ( ids.Count < 1 ) {
              MessageBox.Show( "モデル線分が選択されていません。" ) ;
              return ;
            }
            //置換の場合の処理を実装
            //MessageBox.Show("置換の処理は現在実装中です。");
          }
          else {
            List<ElementId> filterList = new List<ElementId>() ;
            //火打1本目
            ElementId idHiuchi1 = null ;
            if ( ! ClsRevitUtil.PickObjectFilters( uidoc, "火打ベース1本目を指定してください", ClsGlobal.m_baseHiuchiList,
                  ref idHiuchi1 ) ) {
              return ;
            }

            FamilyInstance inst1 = doc.GetElement( idHiuchi1 ) as FamilyInstance ;
            double length1 = ( inst1.Location as LocationCurve ).Curve.Length ;

            levelID = inst1.Host.Id ;

            //火打2本目
            ElementId idHiuchi2 = null ;
            if ( ! ClsRevitUtil.PickObjectFilters( uidoc, "火打ベース2本目を指定してください", ClsGlobal.m_baseHiuchiList,
                  ref idHiuchi2 ) ) {
              return ;
            }

            FamilyInstance inst2 = doc.GetElement( idHiuchi2 ) as FamilyInstance ;
            double length2 = ( inst2.Location as LocationCurve ).Curve.Length ;

            string selMess = "1本目の" ;
            ElementId notSelId = idHiuchi2 ;
            if ( length2 < length1 ) {
              filterList.Add( idHiuchi1 ) ;
            }
            else {
              filterList.Add( idHiuchi2 ) ;
              notSelId = idHiuchi1 ;
              selMess = "2本目の" ;
            }

            //片方のベース上から1点を指定
            Reference rfPick = null ;
            if ( ! ClsRevitUtil.PickObjectElementFilter( uidoc, selMess + "火打ベース上で配置点を指定してください", filterList,
                  ref rfPick ) ) {
              return ;
            }

            XYZ picPoint = rfPick.GlobalPoint ;
            filterList.Remove( rfPick.ElementId ) ;

            Element selHiuchi = doc.GetElement( rfPick.ElementId ) ;

            Element notSelHiuchi = doc.GetElement( notSelId ) ; // filterList[0]);

            //選択された火打
            inst1 = selHiuchi as FamilyInstance ;
            Curve cvHiuchi1 = ( inst1.Location as LocationCurve ).Curve ;
            XYZ dirHiuchi1 = Line.CreateBound( cvHiuchi1.GetEndPoint( 0 ), cvHiuchi1.GetEndPoint( 1 ) ).Direction ;
            XYZ facOriHiuchi1 = inst1.FacingOrientation ;
            //火打の鋼材サイズを取得する//とりあえず300にしているがベースから鋼材サイズを取得する
            double dHiuchiSize1 = 300.0 / 2 ;


            //選択されなかった火打
            inst2 = notSelHiuchi as FamilyInstance ;
            Curve cvHiuchi2 = ( inst2.Location as LocationCurve ).Curve ;
            //火打の鋼材サイズを取得する//とりあえず300にしているがベースから鋼材サイズを取得する
            double dHiuchiSize2 = 300.0 / 2 ;

            //選択されなかった火打の中点を取得
            XYZ midHiuchi2 = ( cvHiuchi2.GetEndPoint( 0 ) + cvHiuchi2.GetEndPoint( 1 ) ) / 2 ;

            //選択した火打から見て選択していない火打の中点は左にあるかで向きを決める
            XYZ dir1toMid = Line.CreateBound( cvHiuchi1.GetEndPoint( 0 ), midHiuchi2 ).Direction ;
            if ( ClsGeo.IsLeft( dirHiuchi1, dir1toMid ) ) {
              //左に90度
              dirHiuchi1 = new XYZ( facOriHiuchi1.X, facOriHiuchi1.Y, 0 ) ;
            }
            else {
              //右に90度
              dirHiuchi1 = new XYZ( -facOriHiuchi1.X, -facOriHiuchi1.Y, 0 ) ;
            }

            XYZ insec = null ;
            //for (int i = 1; i < int.MaxValue; i++)
            //{
            //    //500増やすかは未定
            //    XYZ newPoint = new XYZ(picPoint.X + (ClsRevitUtil.CovertToAPI(i * 500) * dirHiuchi1.X),
            //                           picPoint.Y + (ClsRevitUtil.CovertToAPI(i * 500) * dirHiuchi1.Y),
            //                           picPoint.Z + (ClsRevitUtil.CovertToAPI(i * 500) * dirHiuchi1.Z));

            //    Curve cvBase = Line.CreateBound(picPoint, newPoint);
            //    insec = ClsRevitUtil.GetIntersection(cvHiuchi2, cvBase);
            //    if (insec != null)
            //    {
            //        break;
            //    }
            //}
            XYZ newPoint = new XYZ( picPoint.X + ( ClsRevitUtil.CovertToAPI( int.MaxValue ) * dirHiuchi1.X ),
              picPoint.Y + ( ClsRevitUtil.CovertToAPI( int.MaxValue ) * dirHiuchi1.Y ),
              picPoint.Z + ( ClsRevitUtil.CovertToAPI( int.MaxValue ) * dirHiuchi1.Z ) ) ;

            Curve cvBase = Line.CreateBound( picPoint, newPoint ) ;
            insec = ClsRevitUtil.GetIntersection( cvHiuchi2, cvBase ) ;
            if ( insec == null ) {
              MessageBox.Show( "交点が存在しません" ) ;
              return ; //交点が存在しない
            }

            Line kariLine = Line.CreateBound( picPoint, insec ) ;
            dirHiuchi1 = kariLine.Direction ;
            insec = kariLine.GetEndPoint( 1 ) ;
            double dKariLine = ClsRevitUtil.CovertFromAPI( kariLine.Length ) ;
            for ( int i = 0 ; i < int.MaxValue ; i++ ) {
              double d = 500.0 * i - dKariLine ;
              if ( 0 < d ) {
                if ( d <= 100.0 + dHiuchiSize1 + dHiuchiSize2 ) {
                  i += 1 ;
                }

                d = 500.0 * i - dKariLine - dHiuchiSize1 - dHiuchiSize2 ;
                picPoint = new XYZ( picPoint.X - ( ClsRevitUtil.CovertToAPI( dHiuchiSize1 + d / 2 ) * dirHiuchi1.X ),
                  picPoint.Y - ( ClsRevitUtil.CovertToAPI( dHiuchiSize1 + d / 2 ) * dirHiuchi1.Y ),
                  picPoint.Z - ( ClsRevitUtil.CovertToAPI( dHiuchiSize1 + d / 2 ) * dirHiuchi1.Z ) ) ;
                insec = new XYZ( insec.X + ( ClsRevitUtil.CovertToAPI( dHiuchiSize2 + d / 2 ) * dirHiuchi1.X ),
                  insec.Y + ( ClsRevitUtil.CovertToAPI( dHiuchiSize2 + d / 2 ) * dirHiuchi1.Y ),
                  insec.Z + ( ClsRevitUtil.CovertToAPI( dHiuchiSize2 + d / 2 ) * dirHiuchi1.Z ) ) ;
                break ;
              }
            }


            if ( clsHTB.m_SplitFlg ) {
              Curve cvKari = Line.CreateBound( picPoint, insec ) ;
              List<ElementId> hiuchiList = new List<ElementId>() ;
              foreach ( string baseName in ClsGlobal.m_baseHiuchiList ) {
                hiuchiList.AddRange( ClsRevitUtil.GetSelectCreatedFamilyInstanceList( doc, baseName ) ) ;
              }

              XYZ p1 = picPoint ;
              XYZ p2 ;
              foreach ( ElementId hiuchiId in hiuchiList ) {
                if ( hiuchiId == idHiuchi1 || hiuchiId == idHiuchi2 ) continue ;

                FamilyInstance inst = doc.GetElement( hiuchiId ) as FamilyInstance ;
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
            //任意配置の場合の処理を実装
            //MessageBox.Show("任意配置の処理は現在実装中です。");
          }

          //火打ち繋ぎ材ベースを作図
          if ( ! clsHTB.CreateHiuchiTsunagizaiBase( doc, ids, levelID ) ) {
            //作図に失敗した時のコメント
          }

          //補助線を削除
          using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
            t.Start() ;
            ClsRevitUtil.Delete( doc, ids ) ;
            t.Commit() ;
          }

          if ( clsHTB.m_ShoriType == ClsHiuchiTsunagizaiBase.ShoriType.Replace )
            break ;
        }
        catch {
          break ;
        }
      }

      return ;
    }

    public static void CommandChangeHiuchiTsunagizaiBase( UIDocument uidoc )
    {
      Document doc = uidoc.Document ;

      ClsHiuchiTsunagizaiBase clsHTB = new ClsHiuchiTsunagizaiBase() ;
      ElementId id = null ;
      if ( ! ClsHiuchiTsunagizaiBase.PickBaseObject( uidoc, ref id ) ) {
        return ;
      }

      FamilyInstance shinInstLevel = doc.GetElement( id ) as FamilyInstance ;
      LocationCurve lCurve = shinInstLevel.Location as LocationCurve ;
      if ( lCurve == null ) {
        return ;
      }

      XYZ tmpStPoint = lCurve.Curve.GetEndPoint( 0 ) ;
      XYZ tmpEdPoint = lCurve.Curve.GetEndPoint( 1 ) ;

      //元データを取得
      ElementId levelID = shinInstLevel.Host.Id ;
      string dan = ClsRevitUtil.GetParameter( doc, id, "段" ) ;
      clsHTB.m_SteelType = ClsRevitUtil.GetParameter( doc, id, "分類" ) ;
      clsHTB.m_SteelSize = ClsRevitUtil.GetParameter( doc, id, "サイズ" ) ;
      string toritsuke = ClsRevitUtil.GetParameter( doc, id, "取付方法" ) ;
      clsHTB.m_ToritsukeHoho = clsHTB.GetToritsukeHoho( toritsuke ) ;
      if ( clsHTB.m_ToritsukeHoho == ClsHiuchiTsunagizaiBase.ToritsukeHoho.Bolt ) {
        clsHTB.m_BoltType1 = ClsRevitUtil.GetParameter( doc, id, "ボルトタイプ1" ) ;
        clsHTB.m_BoltType2 = ClsRevitUtil.GetParameter( doc, id, "ボルトタイプ2" ) ;
        clsHTB.m_BoltNum = ClsRevitUtil.GetParameterInteger( doc, id, "ボルト本数" ) ;
      }

      DLG.DlgCreateHiuchiTsunagizaiBase HTB = new DLG.DlgCreateHiuchiTsunagizaiBase( clsHTB ) ;
      DialogResult result = HTB.ShowDialog() ;
      if ( result != DialogResult.OK ) {
        return ;
      }

      clsHTB = HTB.m_HiuchiTsunagizaiBase ;

      //分割がある場合の処理
      ( List<XYZ> stList, List<XYZ> edList ) = clsHTB.ChangeSplitHiuchiTsunagizaiBase( doc, id ) ;

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
        clsHTB.ChangeHiuchiTsunagizaiBase( doc, tmpStPoint, tmpEdPoint, dan, levelID ) ;
      }
    }
  }
}