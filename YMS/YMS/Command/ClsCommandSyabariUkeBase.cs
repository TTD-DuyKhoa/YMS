using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
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
  class ClsCommandSyabariUkeBase
  {
    public static void CommandCreateSyabariUkeBase( UIDocument uidoc )
    {
      //ドキュメントを取得
      Document doc = uidoc.Document ;

      //ダイアログを表示
      //ダイアログを表示
      YMS.DLG.DlgCreateSyabariUkeBase SUB = new DLG.DlgCreateSyabariUkeBase() ;
      DialogResult result = SUB.ShowDialog() ;
      if ( result == DialogResult.Cancel ) {
        return ;
      }

      ClsSyabariUkeBase clsSUB = SUB.m_SyabariUkeBase ;

      for ( ; ; ) {
        try {
          var idSyabariListPick = new List<ElementId>() ;
          //if (!ClsSyabariBase.PickBaseObjects(uidoc, ref idSyabariList))
          if ( ! ClsSyabariBase.PickObjects( uidoc, ref idSyabariListPick ) ) {
            return ;
          }

          List<ElementId> idSyabariList = new List<ElementId>() ;
          foreach ( ElementId pickId in idSyabariListPick ) {
            idSyabariList.Add( YMS.ClsKariKouzai.GetBaseId( doc, pickId ) ) ;
          }

          //一番低い斜梁の選定
          ElementId idSyabari = null ;
          for ( int i = 0 ; i < idSyabariList.Count ; i++ ) {
            if ( i == 0 ) {
              idSyabari = idSyabariList[ i ] ;
              continue ;
            }

            var syabariInst = doc.GetElement( idSyabari ) as FamilyInstance ;
            var inster = doc.GetElement( idSyabariList[ i ] ) as FamilyInstance ;
            if ( syabariInst.HandOrientation.Z > inster.HandOrientation.Z )
              idSyabari = idSyabariList[ i ] ;
          }

          if ( idSyabari == null )
            return ;

          FamilyInstance inst = doc.GetElement( idSyabari ) as FamilyInstance ;
          ReferencePlane referencePlane = inst.Host as ReferencePlane ;
          ElementId levelID = ClsRevitUtil.GetParameterElementId( doc, idSyabari, "集計レベル" ) ;
          var point = ( inst.Location as LocationPoint ).Point ;
          var cvSyabari = ClsYMSUtil.GetBaseLine( doc, idSyabari ) ;
          XYZ midSyabari = ( cvSyabari.GetEndPoint( 0 ) + cvSyabari.GetEndPoint( 1 ) ) / 2 ;

          if ( clsSUB.m_ShoriType == ClsSyabariUkeBase.ShoriType.Replace ) {
            //作図に必要な作図箇所の座標を取得
            List<ElementId> ids = new List<ElementId>() ;
            if ( ! ClsRevitUtil.PickObjects( uidoc, "切梁受けベースに置換するモデル線分", "モデル線分", ref ids ) ) {
              return ;
            }

            if ( ids.Count < 1 ) {
              MessageBox.Show( "モデル線分が選択されていません。" ) ;
              return ;
            }

            foreach ( var lineId in ids ) {
              var line = ( doc.GetElement( lineId ).Location as LocationCurve ).Curve ;

              var zLine = ClsRevitUtil.GetIntersectionZ0( cvSyabari, line, true ) ;
              var tmpStPoint = new XYZ( line.GetEndPoint( 0 ).X, line.GetEndPoint( 0 ).Y, zLine.Z ) ;
              var tmpEdPoint = new XYZ( line.GetEndPoint( 1 ).X, line.GetEndPoint( 1 ).Y, zLine.Z ) ;
              var tmpThirdPoint = new XYZ( point.X, point.Y, zLine.Z ) ;

              ClsSyabariTsunagizaiBase.ReverceTsunagiBaseVec( doc, idSyabari, ref tmpStPoint, ref tmpEdPoint ) ;

              var zureS = ClsRevitUtil.CovertToAPI( clsSUB.m_TsukidashiRyoS ) ;
              var zureE = ClsRevitUtil.CovertToAPI( clsSUB.m_TsukidashiRyoE ) ;
              var vecS = ( tmpStPoint - tmpEdPoint ).Normalize() ;
              var vecE = ( tmpEdPoint - tmpStPoint ).Normalize() ;
              tmpStPoint = tmpStPoint + vecS * zureS ;
              tmpEdPoint = tmpEdPoint + vecE * zureE ;

              if ( ! clsSUB.CreateSyabariUkeBase( doc, tmpStPoint, tmpEdPoint, tmpThirdPoint, levelID ) )
                continue ;
              if ( ! clsSUB.CreateSyabariUke( doc, idSyabari, clsSUB.m_ElementId ) )
                return ;
            }

            //補助線を削除
            using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
              t.Start() ;
              ClsRevitUtil.Delete( doc, ids ) ;
              t.Commit() ;
            }
          }
          else if ( clsSUB.m_ShoriType == ClsSyabariUkeBase.ShoriType.PtoP ) {
            XYZ tmpStPoint, tmpEdPoint ;
            ( tmpStPoint, tmpEdPoint ) = ClsRevitUtil.GetSelect2Point( uidoc ) ;
            if ( tmpStPoint == null || tmpEdPoint == null ) return ;

            //選択した切梁の中点を掘削側とし向きに対して左側に来るように変更
            XYZ Direction = Line.CreateBound( tmpStPoint, tmpEdPoint ).Direction ;
            XYZ kussakuDir = Line.CreateBound( tmpStPoint, midSyabari ).Direction ;
            if ( ! ClsGeo.IsLeft( Direction, kussakuDir ) ) {
              //SとE入れ替え
              XYZ p = tmpStPoint ;
              tmpStPoint = tmpEdPoint ;
              tmpEdPoint = p ;
            }

            var zureS = ClsRevitUtil.CovertToAPI( clsSUB.m_TsukidashiRyoS ) ;
            var zureE = ClsRevitUtil.CovertToAPI( clsSUB.m_TsukidashiRyoE ) ;
            var vecS = ( tmpStPoint - tmpEdPoint ).Normalize() ;
            var vecE = ( tmpEdPoint - tmpStPoint ).Normalize() ;
            var line = Line.CreateBound( tmpStPoint + vecS * zureS, tmpEdPoint + vecE * zureE ) ;
            //var line = Line.CreateBound(tmpStPoint, tmpEdPoint);
            var zLine = ClsRevitUtil.GetIntersectionZ0( cvSyabari, line, true ) ;
            tmpStPoint = new XYZ( line.GetEndPoint( 0 ).X, line.GetEndPoint( 0 ).Y, zLine.Z ) ;
            tmpEdPoint = new XYZ( line.GetEndPoint( 1 ).X, line.GetEndPoint( 1 ).Y, zLine.Z ) ;
            var tmpThirdPoint = new XYZ( point.X, point.Y, zLine.Z ) ;
            if ( ! clsSUB.CreateSyabariUkeBase( doc, tmpStPoint, tmpEdPoint, tmpThirdPoint, levelID ) )
              continue ;
            if ( ! clsSUB.CreateSyabariUke( doc, idSyabari, clsSUB.m_ElementId ) )
              return ;
          }
          else {
            XYZ tmpStPoint, tmpEdPoint ;
            ElementId id1 = null, id2 = null ;

            if ( ! ClsOyakui.PickBaseObject( uidoc, ref id1 ) ) {
              return ;
            }

            if ( ! ClsOyakui.PickBaseObject( uidoc, ref id2 ) ) {
              return ;
            }

            FamilyInstance inst1 = doc.GetElement( id1 ) as FamilyInstance ;
            LocationPoint lPoint1 = inst1.Location as LocationPoint ;
            if ( lPoint1 == null ) {
              return ;
            }

            FamilyInstance inst2 = doc.GetElement( id2 ) as FamilyInstance ;
            LocationPoint lPoint2 = inst2.Location as LocationPoint ;
            if ( lPoint2 == null ) {
              return ;
            }

            tmpStPoint = lPoint1.Point ;
            tmpEdPoint = lPoint2.Point ;

            //選択した切梁の中点を掘削側とし向きに対して左側に来るように変更
            XYZ Direction = Line.CreateBound( tmpStPoint, tmpEdPoint ).Direction ;
            XYZ kussakuDir = Line.CreateBound( tmpStPoint, midSyabari ).Direction ;
            if ( ! ClsGeo.IsLeft( Direction, kussakuDir ) ) {
              //SとE入れ替え
              XYZ p = tmpStPoint ;
              tmpStPoint = tmpEdPoint ;
              tmpEdPoint = p ;
              //インスタンスも入れ替え
              FamilyInstance f = inst1 ;
              inst1 = inst2 ;
              inst2 = f ;
            }

            //
            XYZ picPoint = uidoc.Selection.PickPoint( "作図側を指定してください" ) ;
            Direction = Line.CreateBound( tmpStPoint, tmpEdPoint ).Direction ;
            kussakuDir = Line.CreateBound( tmpStPoint, picPoint ).Direction ;
            if ( ! ClsGeo.IsLeft( Direction, kussakuDir ) ) {
              //SとE入れ替え
              XYZ p = tmpStPoint ;
              tmpStPoint = tmpEdPoint ;
              tmpEdPoint = p ;
              //インスタンスも入れ替え
              FamilyInstance f = inst1 ;
              inst1 = inst2 ;
              inst2 = f ;
            }

            string kui1 = inst1.Symbol.FamilyName ;
            string H1 = ClsYMSUtil.GetKouzaiSizeSunpou1( kui1, 1 ) ;
            string B1 = ClsYMSUtil.GetKouzaiSizeSunpou1( kui1, 2 ) ;

            double dH1 = ClsRevitUtil.CovertToAPI( ClsCommonUtils.ChangeStrToDbl( H1 ) ) / 2 ;
            double dB1 = ClsRevitUtil.CovertToAPI( ClsCommonUtils.ChangeStrToDbl( B1 ) ) / 2 ;

            string kui2 = inst2.Symbol.FamilyName ;
            string H2 = ClsYMSUtil.GetKouzaiSizeSunpou1( kui2, 1 ) ;
            string B2 = ClsYMSUtil.GetKouzaiSizeSunpou1( kui2, 2 ) ;

            double dH2 = ClsRevitUtil.CovertToAPI( ClsCommonUtils.ChangeStrToDbl( H2 ) ) / 2 ;
            double dB2 = ClsRevitUtil.CovertToAPI( ClsCommonUtils.ChangeStrToDbl( B2 ) ) / 2 ;

            Direction = Line.CreateBound( tmpStPoint, tmpEdPoint ).Direction ;
            if ( ClsGeo.IsLeft( inst1.HandOrientation, Direction ) ) {
              tmpStPoint -= dB1 * inst1.HandOrientation ;
              tmpEdPoint -= dB2 * inst2.HandOrientation ;
              dB1 = 0.0 ;
            }
            else {
              tmpStPoint += dB1 * inst1.HandOrientation ;
              tmpEdPoint += dB2 * inst2.HandOrientation ;
              dB2 = 0.0 ;
            }

            var zureS = ClsRevitUtil.CovertToAPI( clsSUB.m_TsukidashiRyoS ) ;
            var zureE = ClsRevitUtil.CovertToAPI( clsSUB.m_TsukidashiRyoE ) ;
            var width = ClsRevitUtil.CovertToAPI( clsSUB.GetSyabariUkeWidth() / 2 ) ;
            var line = ClsYMSUtil.CreateLine( doc, tmpStPoint, tmpEdPoint, -dH1 - width, -dH2 - width, dB1 * 2 + zureS,
              dB2 * 2 + zureE ) ;
            var zLine = ClsRevitUtil.GetIntersectionZ0( cvSyabari, line, true ) ;
            tmpStPoint = new XYZ( line.GetEndPoint( 0 ).X, line.GetEndPoint( 0 ).Y, zLine.Z ) ;
            tmpEdPoint = new XYZ( line.GetEndPoint( 1 ).X, line.GetEndPoint( 1 ).Y, zLine.Z ) ;
            var tmpThirdPoint = new XYZ( point.X, point.Y, zLine.Z ) ;
            if ( ! clsSUB.CreateSyabariUkeBase( doc, tmpStPoint, tmpEdPoint, tmpThirdPoint, levelID ) )
              continue ;
            if ( ! clsSUB.CreateSyabariUke( doc, idSyabari, clsSUB.m_ElementId ) )
              return ;
          }

          if ( clsSUB.m_ShoriType == ClsSyabariUkeBase.ShoriType.Replace )
            break ;
        }
        catch {
          break ;
        }
      }

      return ;
    }

    public static void CommandChangeSyabariUkeBase( UIDocument uidoc )
    {
      //ドキュメントを取得
      Document doc = uidoc.Document ;

      ////ワークセット
      //ClsWorkset clsWS = new ClsWorkset();
      //clsWS.SetWorkSetTest1(doc);

      List<ElementId> ids = null ;
      if ( ! ClsSyabariUkeBase.PickBaseObjects( uidoc, ref ids ) )
        return ;
      if ( ids.Count < 1 )
        return ;

      ClsSyabariUkeBase clsSUB = new ClsSyabariUkeBase() ;
      //基本同じものを選択している想定。違う場合でも最終的には同じパラメータに直すので1つ目のデータを採用
      clsSUB.SetClassParameter( doc, ids[ 0 ] ) ;

      //ダイアログを表示
      YMS.DLG.DlgCreateSyabariUkeBase SUB = new DLG.DlgCreateSyabariUkeBase( clsSUB ) ;
      var result = SUB.ShowDialog() ;
      if ( result == DialogResult.Cancel ) {
        return ;
      }

      clsSUB = SUB.m_SyabariUkeBase ;

      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        t.Start() ;
        foreach ( var id in ids ) {
          clsSUB.SetParameter( doc, id ) ;
        }

        t.Commit() ;
      }

      return ;
    }
  }
}