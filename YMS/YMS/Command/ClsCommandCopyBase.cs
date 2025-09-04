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
  class ClsCommandCopyBase
  {
    /// <summary>
    /// ベースの作成コマンド　全ベース対象
    /// </summary>
    /// <param name="uidoc"></param>
    public static void CommandCopyBase( UIDocument uidoc )
    {
      Document doc = uidoc.Document ;

      // 対象となるベースを選択
      List<string> targetFilter = new List<string>() ;
      //targetFilter.AddRange(ClsGlobal.m_sanjikuPeaceList);
      targetFilter.AddRange( ClsGlobal.m_baseAllList ) ;

      List<ElementId> selectedbaseIds = new List<ElementId>() ;
      if ( ! ClsRevitUtil.PickObjectsFilters( uidoc, "コピーしたいベース", targetFilter, ref selectedbaseIds ) ) {
        return ;
      }

      int cnt = 0 ;
      List<ElementId> levelIdList = new List<ElementId>() ;
      foreach ( ElementId baseId in selectedbaseIds ) {
        Element elem = doc.GetElement( baseId ) ;
        FamilyInstance inst = elem as FamilyInstance ;
        if ( inst != null ) {
          ElementId levelID = inst.Host.Id ;
          if ( cnt == 0 ) {
            levelIdList.Add( levelID ) ;
          }
          else {
            if ( ! levelIdList.Contains( levelID ) ) {
              MessageBox.Show( "複数レベルのベースをまとめて選択はできません。" ) ;
              return ;
            }
            else {
              levelIdList.Add( levelID ) ;
            }
          }

          cnt++ ;
        }
      }

      if ( levelIdList.Count < 1 ) {
        return ;
      }

      List<string> copyDanList = new List<string>() ;

      Level lv = doc.GetElement( levelIdList[ 0 ] ) as Level ;
      string orgDanName = lv.Name ;

      double elevation1 = ClsRevitUtil.GetLevelElevation( doc, orgDanName ) ;


      DLG.DlgBaseCopy dlg = new DLG.DlgBaseCopy( doc, orgDanName ) ;
      if ( dlg.ShowDialog() != DialogResult.OK ) {
        return ;
      }

      bool cut = dlg.GetChechCut() ;

      List<KeyValuePair<string, ElementId>> res = dlg.ListLevel ;
      foreach ( KeyValuePair<string, ElementId> kv in res ) {
        copyDanList.Add( kv.Key ) ;
      }

      //コピーする段数のループ
      foreach ( string copyDanName in copyDanList ) {
        double elevation2 = ClsRevitUtil.GetLevelElevation( doc, copyDanName ) ;
        double dist = elevation2 - elevation1 ;
        ElementId copyLevelId = ClsRevitUtil.GetLevelID( doc, copyDanName ) ;

        if ( copyLevelId == null ) {
          continue ;
        }

        foreach ( ElementId baseId in selectedbaseIds ) {
          Element elem = doc.GetElement( baseId ) ;
          FamilyInstance inst = elem as FamilyInstance ;
          LocationCurve lCurve = inst.Location as LocationCurve ;
          if ( lCurve == null ) {
            continue ;
          }

          ElementId levelID = inst.Host.Id ;

          XYZ tmpStPoint = lCurve.Curve.GetEndPoint( 0 ) ;
          XYZ tmpEdPoint = lCurve.Curve.GetEndPoint( 1 ) ;

          //新しい高さにZ座標補正
          tmpStPoint = new XYZ( tmpStPoint.X, tmpStPoint.Y, tmpStPoint.Z + dist ) ;
          tmpEdPoint = new XYZ( tmpEdPoint.X, tmpEdPoint.Y, tmpEdPoint.Z + dist ) ;


          string strSymbolName = inst.Name ;
          switch ( strSymbolName ) {
            case ClsKiribariBase.baseName : //切梁ベース
              ClsKiribariBase clsKiri = new ClsKiribariBase() ;
              clsKiri.SetParameter( doc, baseId ) ;
              clsKiri.m_Floor = copyDanName ;
              clsKiri.CreateKiribariBase( doc, tmpStPoint, tmpEdPoint ) ;
              break ;
            case ClsKiribariHiuchiBase.baseName : //切梁火打ベース
              ClsKiribariHiuchiBase clsKiriHiuchi = new ClsKiribariHiuchiBase() ;
              clsKiriHiuchi.SetParameter( doc, baseId ) ;
              clsKiriHiuchi.m_Floor = copyDanName ;
              clsKiriHiuchi.CreateKiribariHiuchiBase( doc, tmpStPoint, tmpEdPoint ) ;
              break ;
            case ClsCornerHiuchiBase.baseName : //隅火打ベース
              ClsCornerHiuchiBase clsCHiuchi = new ClsCornerHiuchiBase() ;
              clsCHiuchi.SetParameter( doc, baseId ) ;
              clsCHiuchi.m_Floor = copyDanName ;
              clsCHiuchi.CreateCornerHiuchi( doc, tmpStPoint, tmpEdPoint ) ;
              break ;
            case ClsHaraokoshiBase.baseName : //腹起ベース
              ClsHaraokoshiBase clsHaraokoshi = new ClsHaraokoshiBase() ;
              clsHaraokoshi.SetParameter( doc, baseId ) ;
              clsHaraokoshi.m_level = copyDanName ;
              clsHaraokoshi.CreateHaraokoshiBase( doc, tmpStPoint, tmpEdPoint ) ;
              break ;
            case ClsKiribariUkeBase.baseName : //切梁受けベース
              ClsKiribariUkeBase clsKiribariUke = new ClsKiribariUkeBase() ;
              clsKiribariUke.SetParameter( doc, baseId ) ;
              clsKiribariUke.m_Floor = copyDanName ;
              clsKiribariUke.ChangeKiribariUkeBase( doc, tmpStPoint, tmpEdPoint ) ;
              break ;
            case ClsKiribariTsunagizaiBase.baseName : //切梁つなぎ材ベース
              ClsKiribariTsunagizaiBase clsKiribariTsunagizai = new ClsKiribariTsunagizaiBase() ;
              clsKiribariTsunagizai.SetParameter( doc, baseId ) ;
              clsKiribariTsunagizai.m_Floor = copyDanName ;
              clsKiribariTsunagizai.ChangeKiribariTsunagizaiBase( doc, tmpStPoint, tmpEdPoint ) ;
              break ;
            case ClsHiuchiTsunagizaiBase.baseName : //火打つなぎ材ベース
              ClsHiuchiTsunagizaiBase clsHiuchiTsunagizai = new ClsHiuchiTsunagizaiBase() ;
              clsHiuchiTsunagizai.SetParameter( doc, baseId ) ;
              clsHiuchiTsunagizai.m_Floor = copyDanName ;
              clsHiuchiTsunagizai.ChangeHiuchiTsunagizaiBase( doc, tmpStPoint, tmpEdPoint ) ;
              break ;
            case ClsKiribariTsugiBase.baseName : //切梁つぎベース
              ClsKiribariTsugiBase clsKiribari = new ClsKiribariTsugiBase() ;
              clsKiribari.SetParameter( doc, baseId ) ;
              clsKiribari.m_Floor = copyDanName ;
              clsKiribari.CreateKiribariTsugiBase( doc, tmpStPoint, tmpEdPoint ) ;
              break ;
            default :
              //if (ClsGlobal.m_sanjikuPeaceList.Contains(strSymbolName))
              //{
              //    //三軸ピースも対象にする？いまはしない
              //    //対象の切梁ベースのIDを持たせなければいけない。

              //}
              break ;
          }
        }
      }

      if ( cut ) {
        using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
          t.Start() ;
          ClsRevitUtil.Delete( doc, selectedbaseIds ) ;
          t.Commit() ;
        }
      }

      return ;
    }
  }
}