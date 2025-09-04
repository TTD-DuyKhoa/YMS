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
  public class ClsCommandKouyaita
  {
    public static void CommandKouyaita( UIApplication uiapp )
    {
      UIDocument uidoc = uiapp.ActiveUIDocument ;
      //ドキュメントを取得
      Document doc = uidoc.Document ;

      //Kouyaitaクラスを生成
      ClsKouyaita clsKouyaita = new ClsKouyaita() ;

      ////ワークセット
      //ClsWorkset clsWS = new ClsWorkset();
      //clsWS.SetWorkSetTest2(doc);

      //ダイアログを表示
      YMS.DLG.DlgCreateKouyaita Kouyaita = new DLG.DlgCreateKouyaita( doc ) ;
      //ClsGlobal.m_Kouyaita = Kouyaita;
      for ( ; ; ) {
        DialogResult result = Kouyaita.ShowDialog() ;
        if ( result == DialogResult.Cancel ) {
          return ;
        }
        else if ( result == DialogResult.Retry ) {
          ElementId id = null ;
          if ( ! ClsKouyaita.PickObject( uidoc, ref id ) )
            return ;
          clsKouyaita.SetParameter( doc, id ) ;
          Kouyaita = new DLG.DlgCreateKouyaita( doc, clsKouyaita ) ;
        }
        else
          break ;
      }

      clsKouyaita = Kouyaita.m_ClsKouyaita ;

      //作図に必要な作図箇所の座標を取得
      Selection selection = uidoc.Selection ;
      ElementId selShinId = null ;
      XYZ picPointS = null ;
      XYZ picPointE = null ;
      ElementId levelId = null ;
      VoidVec voidVec = VoidVec.Kussaku ;
      VoidVec lastVoidVec = VoidVec.Kabe ;

      YMS.DLG.DlgOppKouyaita oppKouyaita = new DLG.DlgOppKouyaita() ;
      DialogResult dResult ;

      List<ElementId> createKouyaitaList = new List<ElementId>() ;
      for ( ; ; ) {
        try {
          if ( clsKouyaita.m_way == 0 ) {
            if ( ! ClsKabeShin.PickBaseObject( uidoc, ref selShinId ) ) {
              return ;
            }

            FamilyInstance instance = doc.GetElement( selShinId ) as FamilyInstance ;
            LocationCurve lCurve = instance.Location as LocationCurve ;
            levelId = instance.Host.Id ;

            List<int> targetDesumiList = new List<int>() ;
            List<VoidVec> targetDesumilastVoidVecList = new List<VoidVec>() ;

            List<List<XYZ>> spList = ClsKabeShin.GetALLKabeShinStartEndPointList( doc ) ;
            List<List<XYZ>> resList = new List<List<XYZ>>() ;

            ClsYMSUtil.FindConnectedPoints( lCurve.Curve.GetEndPoint( 0 ), lCurve.Curve.GetEndPoint( 1 ), spList,
              ref resList ) ;
            List<ElementId> kabeShinList = ClsKabeShin.GetAllKabeShinList( doc ) ;

            bool opp = false ;

            while ( true ) {
              for ( int i = 0 ; i < resList.Count ; i++ ) {
                if ( resList[ i ].Count != 2 )
                  continue ;
                List<XYZ> list1 = new List<XYZ>() ;
                List<XYZ> list2 = new List<XYZ>() ;

                if ( i == 0 ) {
                  list1 = resList[ i ] ;
                  list2 = resList[ resList.Count - 1 ] ;
                }
                else {
                  list1 = resList[ i ] ;
                  list2 = resList[ i - 1 ] ;
                }

                Curve cv1 = Line.CreateBound( list1[ 0 ], list1[ 1 ] ) ;
                Curve cv2 = Line.CreateBound( list2[ 0 ], list2[ 1 ] ) ;

                //入隅か判定
                bool bIrizumi = false ;
                if ( ! ClsHaraokoshiBase.CheckIrizumi( cv1, cv2, ref bIrizumi ) ) {
                  continue ;
                }

                picPointS = list1[ 0 ] ;
                picPointE = list1[ 1 ] ;

                if ( ! bIrizumi ) //出隅のときのみ別で親杭を作成する
                {
                  if ( 0 < createKouyaitaList.Count )
                    targetDesumiList.Add( createKouyaitaList.Count - 1 ) ;
                  else
                    targetDesumiList.Add( -1 ) ;
                  targetDesumilastVoidVecList.Add( lastVoidVec ) ;
                }

                foreach ( ElementId id in kabeShinList ) {
                  FamilyInstance inst = doc.GetElement( id ) as FamilyInstance ;
                  Curve cv = ( inst.Location as LocationCurve ).Curve ;
                  if ( ClsGeo.GEO_EQ( picPointS, cv.GetEndPoint( 0 ) ) &&
                       ClsGeo.GEO_EQ( picPointE, cv.GetEndPoint( 1 ) ) ) {
                    selShinId = id ;
                    break ;
                  }
                }

                createKouyaitaList.AddRange( clsKouyaita.CreateKouyaita1( doc, picPointS, picPointE, levelId, voidVec,
                  ref lastVoidVec, true, bIrizumi, selShinId ).ToList() ) ;
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
                    targetDesumilastVoidVecList[ i ], selShinId ) ) ;
                }
              }

              if ( ! opp ) {
                //反転否か
                oppKouyaita.ShowDialog() ;
                dResult = oppKouyaita.dialogResult ;
                if ( dResult == DialogResult.Retry ) {
                  opp = true ;
                  //voidVec = VoidVec.Kabe;
                  lastVoidVec = VoidVec.Kussaku ;
                  using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
                    t.Start() ;
                    ClsRevitUtil.Delete( doc, createKouyaitaList ) ;
                    t.Commit() ;
                  }

                  createKouyaitaList.Clear() ;
                  targetDesumiList.Clear() ;
                  continue ;
                }
                else
                  break ;
              }
              else
                break ;
            }

            break ;
          }

          if ( clsKouyaita.m_way == 1 ) {
            if ( ! ClsKabeShin.PickBaseObject( uidoc,
                  ref selShinId ) ) //ClsRevitUtil.PickObject(uidoc, "壁芯", "壁芯", ref selShinId))
            {
              return ;
            }

            FamilyInstance instance = doc.GetElement( selShinId ) as FamilyInstance ;
            LocationCurve lCurve = instance.Location as LocationCurve ;
            levelId = instance.Host.Id ;
            Curve cvShin = lCurve.Curve ;
            picPointS = cvShin.GetEndPoint( 0 ) ;
            picPointE = cvShin.GetEndPoint( 1 ) ;
            XYZ selPoint = selection.PickPoint( "ベースの始点方向を指定してください" ) ;
            if ( ! ClsRevitUtil.CheckNearGetEndPoint( cvShin, selPoint ) ) {
              picPointS = cvShin.GetEndPoint( 1 ) ;
              picPointE = cvShin.GetEndPoint( 0 ) ;
              if ( clsKouyaita.m_putVec == PutVec.Kougo )
                voidVec = VoidVec.Kabe ; //終点方向を指定した場合デフォルトと反対に設定する
              clsKouyaita.m_defaultVec = false ;
            }
          }

          if ( clsKouyaita.m_way == 2 ) {
            //if (levelId == null)
            //{
            //    DLG.DlgGL dlgGL = new DLG.DlgGL(doc);
            //    DialogResult result = dlgGL.ShowDialog();
            //    if (result == DialogResult.Cancel)
            //    {
            //        return;
            //    }
            //    string levelName = dlgGL.m_Level;
            //    levelId = ClsRevitUtil.GetLevelID(doc, levelName);
            //}
            if ( ! ClsKabeShin.PickBaseObject( uidoc, ref selShinId ) ) {
              return ;
            }

            FamilyInstance instance = doc.GetElement( selShinId ) as FamilyInstance ;
            LocationCurve lCurve = instance.Location as LocationCurve ;
            levelId = instance.Host.Id ;

            picPointS = selection.PickPoint( clsKouyaita.FamilyNameKui + "を配置する開始位置を選択してください" ) ;
            if ( picPointS == null ) {
              return ;
            }

            picPointE = selection.PickPoint( clsKouyaita.FamilyNameKui + "を配置する終了位置を選択してください" ) ;
            if ( picPointE == null ) {
              return ;
            }
            //ClsCInput.Instance.Get2Point(uiapp, out picPointS, out picPointE);
          }

          //Kouyaitaを作図
          //if (!clsKouyaita.CreateKouyaita(doc, picPointS, picPointE, voidVec, out lastVoidVec))
          //{
          //    //作図に失敗した時のコメント
          //}
          createKouyaitaList.AddRange( clsKouyaita.CreateKouyaita1( doc, picPointS, picPointE, levelId, voidVec,
            ref lastVoidVec, true, true, selShinId ).ToArray() ) ;
          //反転否か
          //YMS.DLG.DlgOppKouyaita oppKouyaita = new DLG.DlgOppKouyaita();
          oppKouyaita.ShowDialog() ;
          dResult = oppKouyaita.dialogResult ;
          if ( dResult == DialogResult.Retry ) {
            if ( clsKouyaita.m_putVec == PutVec.Const ) {
              if ( clsKouyaita.m_defaultVec ) {
                clsKouyaita.m_defaultVec = false ;
                clsKouyaita.m_ConstOpp = true ;
              }
              else {
                clsKouyaita.m_ConstOpp = true ;
                clsKouyaita.m_defaultVec = true ;
              }
            }

            //voidVec = VoidVec.Kabe;
            lastVoidVec = VoidVec.Kussaku ;
            using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
              t.Start() ;
              ClsRevitUtil.Delete( doc, createKouyaitaList ) ;
              t.Commit() ;
            }

            createKouyaitaList.Clear() ;
            createKouyaitaList.AddRange( clsKouyaita.CreateKouyaita1( doc, picPointS, picPointE, levelId, voidVec,
              ref lastVoidVec, true, true, selShinId ).ToArray() ) ;
          }

          //初期状態に戻す
          createKouyaitaList.Clear() ;
          clsKouyaita.m_defaultVec = true ;
          clsKouyaita.m_ConstOpp = false ;
          lastVoidVec = VoidVec.Kabe ;
        }
        catch ( Exception e ) {
          string mess = e.Message ;
          break ;
        }
      }

      return ;
    }

    public static void CommandFlipKouyaita( UIApplication uiapp )
    {
      UIDocument uidoc = uiapp.ActiveUIDocument ;
      //ドキュメントを取得
      Document doc = uidoc.Document ;

      List<ElementId> idList = new List<ElementId>() ;

      if ( ! ClsKouyaita.PickObjects( uidoc, ref idList ) ) {
        return ;
      }

      //Kouyaitaクラスを生成
      ClsKouyaita clsKouyaita = new ClsKouyaita() ;

      foreach ( ElementId id in idList ) {
        clsKouyaita.FlipKouyaita( doc, id ) ;
      }
    }
  }
}