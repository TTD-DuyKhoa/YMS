using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using Autodesk.Revit.UI.Selection ;
using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.Diagnostics ;
using System.IO ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;
using YMS.Parts ;

namespace YMS.Command
{
  class ClsCommandKariKozai
  {
    const string RotationAngleSettingsIni = "RotationAngleSettings.ini" ;

    public static void CommandCreateKariKouzai( UIDocument uidoc )
    {
      Document doc = uidoc.Document ;

      ////ワークセット
      //ClsWorkset clsWS = new ClsWorkset();
      //clsWS.SetWorkSetTest3(doc);

      //回転角度が許容範囲外の斜梁受ピース、回転ピースの一覧を保持する文字列
      string contents = "" ;
      //回転角度が許容範囲外の斜梁受ピース、回転ピースのIDリスト
      List<ElementId> selectionIds = new List<ElementId>() ;

      //仮鋼材を作成
      int create = 0 ;
      if ( ClsKariKouzai.GetProjectInfo( doc ) ) {
        ClsSanjikuPeace.ChangeSanjikuMode( doc, true ) ;
        ClsKariKouzai.CreateKariKouzai( doc, ref contents, ref selectionIds ) ;
        ClsSumibuPiece.SetSumibuPieceHole( doc, false ) ;
        create = 1 ;
      }
      else {
        ClsSanjikuPeace.ChangeSanjikuMode( doc, false ) ;
        ClsKariKouzai.DeleteKariKouzai( doc ) ;
        ClsSumibuPiece.SetSumibuPieceHole( doc, true ) ;
      }

      ClsKariKouzai.SaveProjectInfo( doc, create ) ;

      if ( create == 1 && File.Exists( Path.Combine( ClsZumenInfo.GetYMSFolder(), RotationAngleSettingsIni ) ) ) {
        string txtPath = Path.Combine( Path.GetTempPath(), "回転角度が許容範囲外の端部部品ファミリ.txt" ) ;

        //contents += (Env.separatorProjectInfo() + Environment.NewLine);
        ////EOD
        //contents += (Env.separatorEnd() + Environment.NewLine);

        if ( contents != ClsKariKouzai.BASEMESSAGE ) //メッセージに追記があった場合は出力する #34194
        {
          //回転角度が許容範囲外の端部部品ファミリの一覧をテキストファイルに出力する
          File.WriteAllText( txtPath, contents, Encoding.GetEncoding( "Shift_JIS" ) ) ;

          //メモ帳を起動して、テキストファイルを表示
          Process.Start( "notepad.exe", "\"" + txtPath + "\"" ) ;
        }

        //回転角度が許容範囲外のピースファミリを選択状態にする
        if ( selectionIds.Count > 0 )
          uidoc.Selection.SetElementIds( selectionIds ) ;
      }

      return ;
    }
  }
}