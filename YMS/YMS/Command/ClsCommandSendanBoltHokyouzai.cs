using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.Drawing ;
using System.Linq ;
using System.Security.Cryptography ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;
using System.Windows.Media ;
using YMS.DLG ;
using YMS.Parts ;

namespace YMS.Command
{
  internal class ClsCommandSendanBoltHokyouzai
  {
    public static void CommandCreateSendanBoltHokyouzai( UIDocument uidoc )
    {
      Document doc = uidoc.Document ;

      DlgCreateSendanBoltHokyozai dlgCreateSendanBoltHokyozai = new DlgCreateSendanBoltHokyozai( doc ) ;
      DialogResult result = dlgCreateSendanBoltHokyozai.ShowDialog() ;
      if ( result != DialogResult.OK ) {
        return ;
      }

      while ( true ) {
        try {
          // 腹起ベースを選択
          ElementId idHaraokoshiBase = null ;
          if ( ! ClsHaraokoshiBase.PickBaseObject( uidoc, ref idHaraokoshiBase, "腹起ベース" ) ) {
            return ;
          }

          // ベースから情報を取得
          FamilyInstance instBase = doc.GetElement( idHaraokoshiBase ) as FamilyInstance ;
          Curve cvBase = ( instBase.Location as LocationCurve ).Curve ;
          XYZ tmpStPoint = cvBase.GetEndPoint( 0 ) ;
          XYZ tmpEdPoint = cvBase.GetEndPoint( 1 ) ;

          // 火打受けピースを選択
          ElementId idPiece = null ;
          if ( ! ClsSendanBoltHokyouzai.PickObject( uidoc, ref idPiece, "火打受ピース" ) ) {
            return ;
          }

          // ピースから情報を取得
          FamilyInstance instPiece = doc.GetElement( idPiece ) as FamilyInstance ;
          LocationPoint locationPoint = instPiece.Location as LocationPoint ;
          if ( locationPoint == null ) {
            continue ;
          }

          double offset = ClsRevitUtil.GetParameterDouble( doc, idPiece, "基準レベルからの高さ" ) ;
          var idHiuchiBase = ClsKariKouzai.GetBaseId( doc, idPiece, ClsKiribariHiuchiBase.BASEID端部 ) ;
          FamilyInstance instHiuchiBase = doc.GetElement( idHiuchiBase ) as FamilyInstance ;
          Curve cvHiuchiBase = ( instBase.Location as LocationCurve ).Curve ;
          XYZ pointHiuchiBaseS = cvHiuchiBase.GetEndPoint( 0 ) ;
          XYZ pointHiuchiBaseE = cvHiuchiBase.GetEndPoint( 1 ) ;
          XYZ vecHiuchiBase = ( pointHiuchiBaseS - pointHiuchiBaseE ).Normalize() ;

          // ピースの幅を取得
          string familyName = instPiece.Symbol.Family.Name ;
          if ( ! familyName.Contains( "火打" ) ) {
            // 火打受ピースのファミリの名称が統一されれば、この処理は不要
            MessageBox.Show( "対象外の部材です" ) ;
            continue ;
          }

          string[] parts = familyName.Split( '_' ) ;
          string extractedName = parts[ parts.Length - 1 ] ;
          double pieceHaba =
            ClsRevitUtil.ConvertDoubleGeo2Revit( Master.ClsHiuchiCsv.GetHaraGroundLength( extractedName ) ) ;

          tmpStPoint = new XYZ( tmpStPoint.X, tmpStPoint.Y, locationPoint.Point.Z ) ;
          tmpEdPoint = new XYZ( tmpEdPoint.X, tmpEdPoint.Y, locationPoint.Point.Z ) ;

          // 腹起ベースのZ軸を火打受けピースと合わせる
          tmpStPoint = new XYZ( tmpStPoint.X, tmpStPoint.Y, locationPoint.Point.Z ) ;
          tmpEdPoint = new XYZ( tmpEdPoint.X, tmpEdPoint.Y, locationPoint.Point.Z ) ;
          XYZ tmpMidPoint = new XYZ( ( tmpStPoint.X + tmpEdPoint.X ) / 2, ( tmpStPoint.Y + tmpEdPoint.Y ) / 2,
            ( tmpStPoint.Z + tmpEdPoint.Z ) / 2 ) ;

          // 腹起ベースと火打受けピースの交点を取得
          //XYZ ptInsert = ClsSendanBoltHokyouzai.GetIntersectionPointWithLine(doc, tmpStPoint, tmpEdPoint, idPiece);
          XYZ[] ptInserts =
            ClsSendanBoltHokyouzai.GetIntersectionPointsWithLine( doc, tmpStPoint, tmpEdPoint, idPiece ) ;
          if ( ptInserts.Length == 0 ) {
            continue ;
          }

          double distanceToS = locationPoint.Point.DistanceTo( ptInserts.First() ) ;
          double distanceToE = locationPoint.Point.DistanceTo( ptInserts.Last() ) ;
          XYZ ptInsert = ptInserts.First() ;
          if ( distanceToS < distanceToE ) {
            if ( ! familyName.Contains( "自在火打" ) ) {
              ptInsert = ptInserts.Last() ;
            }
          }
          else {
            if ( familyName.Contains( "自在火打" ) ) {
              ptInsert = ptInserts.Last() ;
            }
          }

          XYZ vecPiece = ( ptInserts.Last() - ptInserts.First() ).Normalize() ;

          string tmp = dlgCreateSendanBoltHokyozai.m_clsSendanBoltHokyouzai.m_KouzaiSize.Replace( "SC", "" ) ;
          double hokyouzaiHaba = ClsRevitUtil.ConvertDoubleGeo2Revit( double.Parse( tmp ) ) ;

          ClsSendanBoltHokyouzai clsSendanBoltHokyouzai = dlgCreateSendanBoltHokyozai.m_clsSendanBoltHokyouzai ;
          clsSendanBoltHokyouzai.CreateSendanBoltHokyouzai( doc, ptInsert, instBase.Host.Id, tmpStPoint, tmpEdPoint,
            offset, idPiece, hokyouzaiHaba ) ;
        }
        catch {
          break ;
        }
      }
    }
  }
}