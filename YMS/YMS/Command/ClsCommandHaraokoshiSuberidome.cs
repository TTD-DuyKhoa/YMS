using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using System ;
using System.Collections.Generic ;
using System.IO ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;
using YMS.Parts ;

namespace YMS.Command
{
  public class ClsCommandHaraokoshiSuberidome
  {
    public static void CreateHaraokoshiSuberidome( UIApplication uiapp )
    {
      UIDocument uidoc = uiapp.ActiveUIDocument ;
      Document doc = uidoc.Document ;

      YMS.DLG.DlgCreateHaraokoshisuberidome
        dlgCreateHaraokoshisuberidome = new DLG.DlgCreateHaraokoshisuberidome( doc ) ;
      DialogResult result = dlgCreateHaraokoshisuberidome.ShowDialog() ;
      if ( result != DialogResult.OK ) {
        return ;
      }

      ClsHaraokoshiSuberidome clsHaraokoshiSuberidome = dlgCreateHaraokoshisuberidome.m_ClsHaraokoshiSuberidome ;

      List<ElementId> kuiIds = null ;
      if ( ! clsHaraokoshiSuberidome.PickObjectsKabe( uidoc, ref kuiIds ) ) {
        return ;
      }

      string uniqueName = string.Empty ;

      foreach ( var id in kuiIds ) {
        clsHaraokoshiSuberidome.CreateHaraokoshiSuberidome( doc, id, ref uniqueName ) ;
      }

      return ;
    }

    public static void ChangeHaraokoshiSuberidome( UIApplication uiapp )
    {
      UIDocument uidoc = uiapp.ActiveUIDocument ;
      Document doc = uidoc.Document ;

      List<ElementId> ids = new List<ElementId>() ;
      if ( ! ClsHaraokoshiSuberidome.PickHaraokoshiSuberidomeObjects( uidoc, ref ids ) ) {
        return ;
      }

      //部材が未選択であれば処理を処理を終了
      if ( ids.Count == 0 ) {
        return ;
      }

      //選択された部材が腹起こし滑り止め(チャンネル)以外が含まれていれば処理を終了
      if ( ! ClsHaraokoshiSuberidome.IsHaraokoshiSuberidomeChannel( doc, ids ) ) {
        return ;
      }

      ClsHaraokoshiSuberidome tmp = new ClsHaraokoshiSuberidome() ;

      //選択された部材が1つの場合　選択された部材のプロパティを対象に
      if ( ids.Count == 1 ) {
        tmp.SetParameter( doc, ids[ 0 ] ) ;
      }
      //選択された部材が2つ以上であれば　デフォルト値を指定
      else if ( ids.Count > 1 ) {
        tmp.SetDefaultParameter() ;
      }

      YMS.DLG.DlgCreateHaraokoshisuberidome dlgCreateHaraokoshisuberidome =
        new DLG.DlgCreateHaraokoshisuberidome( doc, tmp ) ;
      DialogResult result = dlgCreateHaraokoshisuberidome.ShowDialog() ;
      if ( result != DialogResult.OK ) {
        return ;
      }

      ClsHaraokoshiSuberidome clsHaraokoshiSuberidome = dlgCreateHaraokoshisuberidome.m_ClsHaraokoshiSuberidome ;
      string uniqueName = string.Empty ;
      foreach ( var id in ids ) {
        FamilyInstance targetHaraokoshiSuberidome = doc.GetElement( id ) as FamilyInstance ;
        FamilyInstance oyaKui = null ;
        if ( ! ClsHaraokoshiSuberidome.GetKuiConHaraokoshiSuberidome( doc, targetHaraokoshiSuberidome, out oyaKui ) ) {
          continue ;
        }

        //元の部材を削除
        using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
          t.Start() ;
          RevitUtil.ClsRevitUtil.Delete( doc, id ) ;
          t.Commit() ;
        }

        clsHaraokoshiSuberidome.CreateHaraokoshiSuberidome( doc, oyaKui.Id, ref uniqueName ) ;
      }

      return ;
    }
  }
}