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
  class ClsCommandSanjikuPiece
  {
    /// <summary>
    /// 三軸ピース作図
    /// </summary>
    /// <param name="uidoc"></param>
    public static void CommandSanjikuPiece( UIDocument uidoc )
    {
      //ドキュメントを取得
      Document doc = uidoc.Document ;

      ////ワークセット
      //ClsWorkset clsWS = new ClsWorkset();
      //clsWS.SetWorkSetTest2(doc);

      //作図に必要な作図箇所の座標を取得
      ElementId idHara = null ;
      if ( ! ClsHaraokoshiBase.PickBaseObject( uidoc, ref idHara ) ) {
        return ;
      }

      ElementId idKiri = null ;
      if ( ! ClsKiribariBase.PickBaseObject( uidoc, ref idKiri ) ) {
        return ;
      }

      //三軸ピースを作図
      ClsSanjikuPeace.CreateSanjikuPeace( doc, idHara, idKiri ) ;

      //ダイアログを表示する
      ClsSanjikuPeace.MovedSanjikuPeace( uidoc ) ;
      Application.thisApp.ShowForm_dlgCreateSanjiku() ;
      return ;
    }
  }
}