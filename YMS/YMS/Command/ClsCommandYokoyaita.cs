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
  class ClsCommandYokoyaita
  {
    public static void CommandYokoyaita( UIDocument uidoc )
    {
      //ドキュメントを取得
      Document doc = uidoc.Document ;
      //クラス作成
      ClsYokoyaita clsYokoyaita = new ClsYokoyaita() ;
      //ElementId id = null;
      //if (!ClsOyakui.PickBaseObject(uidoc, ref id))
      //{
      //    return;
      //}


      DLG.DlgCreateYokoyaita yokoyaita = new DLG.DlgCreateYokoyaita() ;
      DialogResult result = yokoyaita.ShowDialog() ;
      if ( result == DialogResult.Cancel ) {
        return ;
      }

      clsYokoyaita = yokoyaita.m_ClsYokoyaita ;

      List<ElementId> idList = null ;
      if ( ! ClsOyakui.PickObjects( uidoc, ref idList ) ) {
        return ;
      }

      ElementId levelId = null ;

      //for (int i = 0; i < idList.Count - 1; i++)
      //{
      //    ElementId id1 = idList[i];
      //    ElementId id2 = idList[i + 1];
      //    FamilyInstance inst1 = doc.GetElement(id1) as FamilyInstance;
      //    XYZ pt1 = (inst1.Location as LocationPoint).Point;
      //    FamilyInstance inst2 = doc.GetElement(id2) as FamilyInstance;
      //    XYZ pt2 = (inst2.Location as LocationPoint).Point;

      //    levelId = ClsRevitUtil.GetParameterElementId(doc, id1, "集計レベル");

      //    Line line = Line.CreateBound(pt1, pt2);
      //    double lenght = line.Length;
      //    XYZ dir = line.Direction;
      //    //double dVoid = ClsRevitUtil.GetHBeamVoid(doc, id1);
      //    double dVoid = ClsVoid.GetVoidDep(doc, id1);
      //    if (clsYokoyaita.m_type == "木矢板")
      //        clsYokoyaita.CreateMokuyaita(doc, id1, id2, ClsRevitUtil.CovertFromAPI(lenght), ClsRevitUtil.CovertToAPI(dVoid), levelId);
      //    else
      //        clsYokoyaita.CreateYokoyaita(doc, id1, id2, ClsRevitUtil.CovertFromAPI(lenght), ClsRevitUtil.CovertToAPI(dVoid), levelId);

      //}

      List<List<ElementId>> handGroupList =
        ClsYokoyaita.GroupingOyakui( doc, idList, out List<ElementId> cornerGroupOriginalList ) ;
      foreach ( var handGroup in handGroupList ) {
        List<ElementId> cornerGroupList = cornerGroupOriginalList.ToList() ;
        for ( int i = 0 ; i < handGroup.Count - 1 ; i++ ) {
          ElementId id1 = handGroup[ i ] ;
          ElementId id2 = handGroup[ i + 1 ] ;
          FamilyInstance inst1 = doc.GetElement( id1 ) as FamilyInstance ;
          XYZ pt1 = ( inst1.Location as LocationPoint ).Point ;
          FamilyInstance inst2 = doc.GetElement( id2 ) as FamilyInstance ;
          XYZ pt2 = ( inst2.Location as LocationPoint ).Point ;

          levelId = ClsRevitUtil.GetParameterElementId( doc, id1, "集計レベル" ) ;
          Line line = Line.CreateBound( pt1, pt2 ) ;
          double lenght = line.Length ;
          XYZ dir = line.Direction ;
          double dHTop = ClsRevitUtil.GetParameterDouble( doc, id1, ClsGlobal.m_refLvTop ) ;
          double dVoid = ClsVoid.GetVoidDep( doc, id1 ) ;
          if ( clsYokoyaita.m_type == "木矢板" )
            clsYokoyaita.CreateMokuyaita( doc, id1, id2, ClsRevitUtil.CovertFromAPI( lenght ),
              ClsRevitUtil.CovertToAPI( dVoid ), levelId, dHTop ) ;
          else
            clsYokoyaita.CreateYokoyaita( doc, id1, id2, ClsRevitUtil.CovertFromAPI( lenght ),
              ClsRevitUtil.CovertToAPI( dVoid ), levelId, dHTop ) ;
        }

        if ( 0 < cornerGroupList.Count && 0 < handGroup.Count ) {
          //コーナーとの横矢板1
          ElementId id1 = handGroup[ 0 ] ;
          FamilyInstance inst1 = doc.GetElement( id1 ) as FamilyInstance ;
          XYZ pt1 = ( inst1.Location as LocationPoint ).Point ;

          ElementId id2 = cornerGroupList[ 0 ] ;
          FamilyInstance inst2 = doc.GetElement( id2 ) as FamilyInstance ;
          XYZ pt2 = ( inst2.Location as LocationPoint ).Point ;
          double length = Line.CreateBound( pt1, pt2 ).Length ;
          foreach ( var cornerGroup in cornerGroupList ) {
            FamilyInstance cornerInst = doc.GetElement( cornerGroup ) as FamilyInstance ;
            XYZ ptCorner = ( cornerInst.Location as LocationPoint ).Point ;
            double cornerLength = Line.CreateBound( pt1, ptCorner ).Length ;
            if ( cornerLength < length ) {
              length = cornerLength ;
              id2 = cornerGroup ;
              pt2 = ptCorner ;
            }
          }

          //一度作成したコーナーには作成しないようRemove
          cornerGroupList.Remove( id2 ) ;

          levelId = ClsRevitUtil.GetParameterElementId( doc, id1, "集計レベル" ) ;
          Line line = Line.CreateBound( pt1, pt2 ) ;
          double lenght = line.Length ;
          XYZ dir = line.Direction ;
          double dHTop = ClsRevitUtil.GetParameterDouble( doc, id1, ClsGlobal.m_refLvTop ) ;
          double dVoid = ClsVoid.GetVoidDep( doc, id1 ) ;
          if ( clsYokoyaita.m_type == "木矢板" )
            clsYokoyaita.CreateMokuyaita( doc, id1, id2, ClsRevitUtil.CovertFromAPI( lenght ),
              ClsRevitUtil.CovertToAPI( dVoid ), levelId, dHTop ) ;
          else
            clsYokoyaita.CreateYokoyaita( doc, id1, id2, ClsRevitUtil.CovertFromAPI( lenght ),
              ClsRevitUtil.CovertToAPI( dVoid ), levelId, dHTop ) ;
        }

        if ( 0 < cornerGroupList.Count && 1 < handGroup.Count ) {
          //コーナーとの横矢板2
          ElementId id1 = handGroup[ handGroup.Count - 1 ] ;
          FamilyInstance inst1 = doc.GetElement( id1 ) as FamilyInstance ;
          XYZ pt1 = ( inst1.Location as LocationPoint ).Point ;

          ElementId id2 = cornerGroupList[ 0 ] ;
          FamilyInstance inst2 = doc.GetElement( id2 ) as FamilyInstance ;
          XYZ pt2 = ( inst2.Location as LocationPoint ).Point ;
          double length = Line.CreateBound( pt1, pt2 ).Length ;
          foreach ( var cornerGroup in cornerGroupList ) {
            FamilyInstance cornerInst = doc.GetElement( cornerGroup ) as FamilyInstance ;
            XYZ ptCorner = ( cornerInst.Location as LocationPoint ).Point ;
            double cornerLength = Line.CreateBound( pt1, ptCorner ).Length ;
            if ( cornerLength < length ) {
              length = cornerLength ;
              id2 = cornerGroup ;
              pt2 = ptCorner ;
            }
          }

          levelId = ClsRevitUtil.GetParameterElementId( doc, id1, "集計レベル" ) ;
          Line line = Line.CreateBound( pt1, pt2 ) ;
          double lenght = line.Length ;
          XYZ dir = line.Direction ;
          double dHTop = ClsRevitUtil.GetParameterDouble( doc, id1, ClsGlobal.m_refLvTop ) ;
          double dVoid = ClsVoid.GetVoidDep( doc, id1 ) ;
          if ( clsYokoyaita.m_type == "木矢板" )
            clsYokoyaita.CreateMokuyaita( doc, id1, id2, ClsRevitUtil.CovertFromAPI( lenght ),
              ClsRevitUtil.CovertToAPI( dVoid ), levelId, dHTop ) ;
          else
            clsYokoyaita.CreateYokoyaita( doc, id1, id2, ClsRevitUtil.CovertFromAPI( lenght ),
              ClsRevitUtil.CovertToAPI( dVoid ), levelId, dHTop ) ;
        }
      }
    }

    public static void CommandChangeYokoyaita( UIDocument uidoc )
    {
      //ドキュメントを取得
      Document doc = uidoc.Document ;
      //クラス作成
      ClsYokoyaita clsYokoyaita = new ClsYokoyaita() ;

      List<ElementId> idList = null ;
      List<string> yokoyaNameList = Master.ClsYokoyaitaCSV.GetFamilyNameList().ToList() ;
      // 変更する横矢板を選択
      if ( ! ClsRevitUtil.PickObjectsSymbolFilters( uidoc, "横矢板を選択してください", yokoyaNameList, ref idList ) ) {
        return ;
      }

      List<Yokoyaita> yokoyaList = new List<Yokoyaita>() ;

      ElementId levelId = ClsRevitUtil.GetParameterElementId( doc, idList[ 0 ], "集計レベル" ) ;
      for ( int i = 0 ; i < idList.Count ; i++ ) {
        ElementId id = idList[ i ] ;
        FamilyInstance inst = doc.GetElement( id ) as FamilyInstance ;
        LocationPoint lPoint = inst.Location as LocationPoint ;
        double kussaku ;

        if ( inst.Symbol.Name.Contains( "木目" ) ) {
          kussaku = ClsRevitUtil.GetTypeParameter( inst.Symbol, "掘削深さ" ) ;
        }
        else {
          //横矢板の高さを取得
          kussaku = ClsRevitUtil.CovertToAPI( Master.ClsYokoyaitaCSV.GetHightToFamilyName( inst.Symbol.Name ) ) ;
        }

        double length = ClsRevitUtil.GetParameterDouble( doc, id, ClsGlobal.m_length ) ;
        double dHTop = ClsRevitUtil.GetParameterDouble( doc, id, ClsGlobal.m_refLvTop ) ;
        string CASE = ClsRevitUtil.GetTypeParameterString( inst.Symbol, "CASE" ) ;
        yokoyaList.Add( new Yokoyaita( lPoint.Point, lPoint.Rotation, length, kussaku, dHTop, CASE ) ) ;
      }

      yokoyaList = Yokoyaita.SortPoint( yokoyaList ).ToList() ;

      DLG.DlgCreateYokoyaita yokoyaita = new DLG.DlgCreateYokoyaita() ;
      DialogResult result = yokoyaita.ShowDialog() ;
      if ( result == DialogResult.Cancel ) {
        return ;
      }

      clsYokoyaita = yokoyaita.m_ClsYokoyaita ;

      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        t.Start() ;
        ClsRevitUtil.Delete( doc, idList ) ;
        t.Commit() ;
      }

      for ( int i = 0 ; i < yokoyaList.Count ; i++ ) {
        if ( ! yokoyaList[ i ].Create )
          continue ;
        if ( clsYokoyaita.m_type == "木矢板" )
          clsYokoyaita.CreateMokuyaita( doc, yokoyaList[ i ].Point, yokoyaList[ i ].Angle, yokoyaList[ i ].Length,
            yokoyaList[ i ].Kussaku, levelId, yokoyaList[ i ].CASE, yokoyaList[ i ].HTop ) ;
        else
          clsYokoyaita.CreateYokoyaita( doc, yokoyaList[ i ].Point, yokoyaList[ i ].Angle, yokoyaList[ i ].Length,
            yokoyaList[ i ].Kussaku, levelId, yokoyaList[ i ].CASE, yokoyaList[ i ].HTop ) ;
      }
    }
  }
}