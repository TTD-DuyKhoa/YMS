using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using Autodesk.Revit.UI.Selection ;
using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.IO ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using YMS.Parts ;

namespace YMS
{
  public class ClsWaritsuke
  {
    public static XYZ maeP = null ;
    public static Element baseLine = null ;
    public static ElementId selShinId = null ;
    public static FamilyInstance inst = null ;
    public static XYZ geoDirection = null ;

    const string kouzai = "形鋼_H300X300X10X15" ;

    public static bool CreateHari( UIApplication uiapp )
    {
      UIDocument uidoc = uiapp.ActiveUIDocument ;
      Document doc = uidoc.Document ;

      //string symName = kouzai;//仮
      //string familyPath = GetSymbolPath("山留壁ファミリ\\02_形鋼\\" + symName + ".rfa");

      //bool bCreateWaritsuke = false;

      //using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
      //{

      //    Selection selection = uidoc.Selection;
      //    List<ElementId> ids = new List<ElementId>();
      //    List<ElementId> hideList = new List<ElementId>();
      //    bool sel0 = true;


      //    //芯を選択済みかの判定
      //    if (baseLine == null)
      //    {
      //        List<string> filterList = ClsGlobal.m_baseShinList.ToList();
      //        filterList.Add(kouzai);

      //        //基準となるファミリを選択する
      //        if (!PickObjectFilter(uidoc, filterList, "割付を行うベースもしくは仮鋼材を選択してください", ref selShinId)) return false;

      //        baseLine = doc.GetElement(selShinId);
      //        if (baseLine == null)
      //        {
      //            return false;
      //        }
      //        inst = baseLine as FamilyInstance;
      //        //ベースが選択された際に紐図いている仮鋼材に対象を変更する
      //        if (inst.Symbol.FamilyName != kouzai)
      //        {
      //            //ベース芯を選択した場合
      //            selShinId = GetParameterElementId(doc, selShinId, "仮鋼材");
      //            if (selShinId != null)
      //            {
      //                baseLine = doc.GetElement(selShinId);
      //                inst = baseLine as FamilyInstance;
      //            }
      //        }
      //        hideList.Add(selShinId);
      //        ClsRevitUtil.HideElement(doc, hideList);
      //    }

      //    //FamilyInstanceから情報を使う
      //    string instName = inst.Name;
      //    LocationCurve lCurve = inst.Location as LocationCurve;
      //    if (lCurve == null)
      //    {
      //        return false;
      //    }

      //    //XYZ tmpStPoint = lCurve.Curve.GetEndPoint(0);
      //    //XYZ tmpEdPoint = lCurve.Curve.GetEndPoint(1);

      //    Curve geoCurve = lCurve.Curve;
      //    Line geoLine = geoCurve as Line;
      //    geoDirection = geoLine.Direction;//芯のvector
      //    //double length = geoLine.Length;//芯の長さ

      //    XYZ p = new XYZ();
      //    //初期位置の判定
      //    if (maeP == null)
      //    {
      //        p = geoCurve.GetEndPoint(0);//芯作成位置（始点）
      //    }
      //    else
      //    {
      //        p = maeP;
      //    }

      //    ElementId levelID = inst.Host.Id;
      //    Autodesk.Revit.DB.View v = getView(doc, ClsKabeなしShin.GL, ViewType.FloorPlan);
      //    Autodesk.Revit.DB.View v3 = getView(doc, "{3D}", ViewType.ThreeD);
      //    Autodesk.Revit.DB.View vActv = doc.ActiveView;
      //    double vSize = 1.0;
      //    if (vActv.Id == v3.Id)
      //    {
      //        vSize = 1.0;
      //    }
      //    if (vActv.Id == v.Id)
      //    {
      //        vSize = 0.3;
      //    }
      //    //単位ベクトルの符号
      //    int vSignX = Math.Sign(geoDirection.X);
      //    int vSignY = Math.Sign(geoDirection.Y);
      //    int vSignZ = Math.Sign(geoDirection.Z);

      //    //割付部材読込処理
      //    if (!RoadFamilyData(doc, familyPath, symName, out Family oyaFam))
      //    {
      //        return false;
      //    }
      //    FamilySymbol sym = GetFamilySymbol(doc, symName, instName);

      //    for (; ; )
      //    {
      //        try
      //        {
      //            if (!sel0)
      //            {
      //                p = selection.PickPoint("位置を選択！");
      //                XYZ nearPoint = new XYZ((p.X - maeP.X) * geoDirection.X, (p.Y - maeP.Y) * geoDirection.Y, 0);
      //                double nearPointLength = nearPoint.GetLength();

      //                int len = (int)(nearPointLength / ClsRevitUtil.CovertToAPI(500));
      //                p = new XYZ(maeP.X + (ClsRevitUtil.CovertToAPI(len * 500) * geoDirection.X),
      //                            maeP.Y + (ClsRevitUtil.CovertToAPI(len * 500) * geoDirection.Y),
      //                            maeP.Z);

      //                Curve cv0 = Line.CreateBound(maeP, p);

      //                t.Start();
      //                ElementId CreatedID = Create(doc, cv0, levelID, sym);
      //                t.Commit();

      //                bCreateWaritsuke = true;
      //                maeP = p;
      //            }
      //            else
      //            {
      //                maeP = p;
      //                sel0 = false;

      //            }

      //            t.Start();

      //            foreach (ElementId id in ids)
      //            {
      //                doc.Delete(id);
      //            }

      //            ids = new List<ElementId>();

      //            for (int i = 1; i < 151; i++)
      //            {
      //                XYZ newPoint = new XYZ(p.X + (ClsRevitUtil.CovertToAPI(i * 100) * geoDirection.X),
      //                                   p.Y + (ClsRevitUtil.CovertToAPI(i * 100) * geoDirection.Y),
      //                                   p.Z + (ClsRevitUtil.CovertToAPI(i * 100) * geoDirection.Z));
      //                XYZ bubbleEnd = new XYZ(newPoint.X, newPoint.Y, newPoint.Z);
      //                //これなら動く
      //                XYZ freeEnd;
      //                if (Math.Abs(geoDirection.Y) < Math.Abs(geoDirection.X))
      //                {
      //                    vSignY = 0;
      //                    freeEnd = new XYZ(newPoint.X, 0, newPoint.Z);
      //                }
      //                else
      //                {
      //                    vSignX = 0;
      //                    freeEnd = new XYZ(0, newPoint.Y, newPoint.Z);
      //                }
      //                Curve cv;
      //                if (i % 10 == 0)
      //                {
      //                    cv = Line.CreateBound(new XYZ(newPoint.X + (3 * vSignY), newPoint.Y + (3 * vSignX), newPoint.Z + (3 * vSignZ)),
      //                                          new XYZ(newPoint.X - (3 * vSignY), newPoint.Y - (3 * vSignX), newPoint.Z - (3 * vSignZ)));
      //                    ElementId defaultTypeId = doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);

      //                    TextNoteOptions opts = new TextNoteOptions(defaultTypeId);
      //                    double maxWidth = TextNote.GetMaximumAllowedWidth(doc, defaultTypeId);
      //                    maxWidth = ClsRevitUtil.CovertToAPI(maxWidth);
      //                    TextNote tx = TextNote.Create(doc, vActv.Id, new XYZ(newPoint.X - (0.56 * vSize), newPoint.Y + (0.56 * vSize), newPoint.Z),
      //                                                  maxWidth, (i / 10.0).ToString() + ".0", opts);
      //                    TextElementType textType = tx.Symbol;
      //                    BuiltInParameter paraIndex = BuiltInParameter.TEXT_SIZE;
      //                    Parameter textSize = textType.get_Parameter(paraIndex);

      //                    textSize.Set(maxWidth / (10 * vSize));

      //                    paraIndex = BuiltInParameter.TEXT_BACKGROUND;
      //                    Parameter textBack = textType.get_Parameter(paraIndex);
      //                    textBack.Set(1);// 0 = Opaque（不透明） :: 1 = Transparent（透過）
      //                    ids.Add(tx.Id);
      //                }
      //                else if (i % 5 == 0)
      //                {
      //                    cv = Line.CreateBound(new XYZ(newPoint.X + (2 * vSignY), newPoint.Y + (2 * vSignX), newPoint.Z + (2 * vSignZ)),
      //                                          new XYZ(newPoint.X - (2 * vSignY), newPoint.Y - (2 * vSignX), newPoint.Z - (2 * vSignZ)));
      //                }
      //                else
      //                {
      //                    cv = Line.CreateBound(new XYZ(newPoint.X + (1 * vSignY), newPoint.Y + (1 * vSignX), newPoint.Z + (1 * vSignZ)),
      //                                          new XYZ(newPoint.X - (1 * vSignY), newPoint.Y - (1 * vSignX), newPoint.Z - (1 * vSignZ)));
      //                }

      //                if (vActv == null)
      //                {
      //                    return false;
      //                }

      //                XYZ cutvec = new XYZ(0, 0, newPoint.Z);
      //                ReferencePlane plane = doc.Create.NewReferencePlane(bubbleEnd, freeEnd, cutvec, vActv);
      //                SketchPlane skp = SketchPlane.Create(doc, plane.Id);
      //                ModelCurve mc = doc.Create.NewModelCurve(cv, skp);

      //                ids.Add(mc.Id);
      //            }
      //            t.Commit();

      //        }
      //        catch (OperationCanceledException e)
      //        {
      //            break;
      //        }
      //        catch (Exception e)
      //        {
      //            foreach (ElementId id in ids)
      //            {
      //                t.Start();
      //                doc.Delete(id);
      //                t.Commit();
      //            }
      //            if (bCreateWaritsuke)
      //            {
      //                t.Start();
      //                doc.Delete(selShinId);//1つでも割付が行われた場合、仮鋼材を削除する
      //                t.Commit();
      //            }
      //            else
      //            {
      //                //再表示
      //                ClsRevitUtil.UnhideElement(doc, hideList);
      //            }
      //            break;
      //        }
      //    }
      //}
      return true ;
    }


    /// <summary>
    /// シンボルのパスを取得する
    /// </summary>
    /// <param name="file">"山留壁ファミリ\\"or"支保工ファミリ\\" + "ファミリ名" + ".rfa"</param>
    /// <returns>シンボルパス</returns>
    public static string GetSymbolPath( string file )
    {
      string symbolFolpath = ClsZumenInfo.GetYMSFolder() ;
      string shinfamily = System.IO.Path.Combine( symbolFolpath, file ) ;
      return shinfamily ;
    }

    public static bool RoadFamilySymbolData( Document doc, string rfaFilePath, string name, out FamilySymbol sym )
    {
      sym = null ;
      // 既に読み込まれているかどうかをチェックする//symにするとうまく機能していない
      FilteredElementCollector collector = new FilteredElementCollector( doc ) ;
      IEnumerable<Element> elements = collector.OfClass( typeof( FamilySymbol ) ).ToElements() ;
      foreach ( Element elem in elements ) {
        FamilySymbol loadedFamily = elem as FamilySymbol ;
        if ( loadedFamily != null && loadedFamily.Name == Path.GetFileNameWithoutExtension( rfaFilePath ) ) {
          sym = loadedFamily ;
          break ;
        }
      }

      // まだ読み込まれていない場合は読み込む
      if ( sym == null ) {
        using ( Transaction tx = new Transaction( doc, "Load Family" ) ) {
          tx.Start() ;
          doc.LoadFamilySymbol( rfaFilePath, name, out sym ) ;
          tx.Commit() ;
        }
      }

      if ( sym == null ) {
        return false ;
      }

      return true ;
    }

    public static bool RoadFamilyData( Document doc, string rfaFilePath, string name, out Family family )
    {
      family = null ;
      // 既に読み込まれているかどうかをチェックする
      FilteredElementCollector collector = new FilteredElementCollector( doc ) ;
      IEnumerable<Element> elements = collector.OfClass( typeof( Family ) ).ToElements() ;
      foreach ( Element elem in elements ) {
        Family loadedFamily = elem as Family ;
        if ( loadedFamily != null && loadedFamily.Name == Path.GetFileNameWithoutExtension( rfaFilePath ) ) {
          family = loadedFamily ;
          break ;
        }
      }

      // まだ読み込まれていない場合は読み込む
      if ( family == null ) {
        using ( Transaction tx = new Transaction( doc, "Load Family" ) ) {
          tx.Start() ;
          doc.LoadFamily( rfaFilePath, out family ) ;
          tx.Commit() ;
        }
      }

      if ( family == null ) {
        return false ;
      }

      return true ;
    }

    /// <summary>
    /// FamilySymbol取得
    /// </summary>
    /// <param name="doc">Document</param>
    /// <param name="familyName">ファミリ名</param>
    /// <param name="typeName">タイプ名</param>
    /// <param name="categoryName">カテゴリ名</param>
    /// <returns></returns>
    public static FamilySymbol GetFamilySymbol( Document doc, string familyName, string typeName,
      string categoryName = "" )
    {
      if ( doc == null ) {
        return null ;
      }

      FamilySymbol symbol = null ;
      IEnumerable<Element> elems = null ;
      if ( string.IsNullOrEmpty( categoryName ) ) {
        elems = from elem in ( new FilteredElementCollector( doc ).OfClass( typeof( FamilySymbol ) ).ToElements() )
          let sym = elem as FamilySymbol
          where sym.Name == typeName && sym.Family.Name == familyName
          select sym ;
      }
      else {
        elems = from elem in ( new FilteredElementCollector( doc ).OfClass( typeof( FamilySymbol ) ).ToElements() )
          let sym = elem as FamilySymbol
          where sym.Name == typeName && sym.Family.Name == familyName && sym.Family.FamilyCategory.Name == categoryName
          select sym ;
      }

      foreach ( Element elemTmp in elems ) {
        symbol = elemTmp as FamilySymbol ;
        break ;
      }

      return symbol ;
    }

    /// <summary>
    /// 選択オブジェクトのElment取得
    /// </summary>
    /// <param name="doc">ドキュメント</param>
    /// <param name="rf">リファレンス</param>
    /// <returns>選択したオブジェクトのElement</returns>
    public static Element GetSelectElment( Document doc, Reference rf )
    {
      Element el = null ;
      if ( rf == null ) {
        return el ;
      }

      //選択した部材ID
      ElementId selId = rf.ElementId ;
      try {
        el = doc.GetElement( selId ) ;
      }
      catch ( Exception ) {
        throw ;
      }

      return el ;
    }

    public static ElementId Create( Document doc, Curve cv, ElementId levelId, FamilySymbol symbol )
    {
      if ( symbol == null ) {
        return null ;
      }

      ElementId id = null ;

      if ( ! symbol.IsActive ) {
        symbol.Activate() ;
      }

      try {
        Level lv = doc.GetElement( levelId ) as Level ;
        FamilyInstance newInstance = doc.Create.NewFamilyInstance( cv, symbol, lv,
          Autodesk.Revit.DB.Structure.StructuralType.NonStructural ) ;

        //配置後に改めてデフォルト値設定
        //ModifyLevel(doc, newInstance.Id, levelId);
        //ModifyOffset(doc, newInstance.Id, 0);
        id = newInstance.Id ;
      }
      catch {
        return null ;
      }

      return id ;
    }

    /// <summary>
    /// ElementIdパラメーターの取得
    /// </summary>
    /// <param name="doc">ドキュメント</param>
    /// <param name="CreatedID">シンボルID</param>
    /// <param name="paramName">パラメーター名</param>
    /// <returns></returns>
    public static ElementId GetParameterElementId( Document doc, ElementId CreatedID, string paramName )
    {
      try {
        Parameter parm = doc.GetElement( CreatedID ).LookupParameter( paramName ) ;
        if ( parm == null ) {
          return null ;
        }

        int id = parm.AsInteger() ;
        ElementId e = new ElementId( id ) ;
        return e ;
      }
      catch {
        return null ;
      }
    }

    public static bool PickObjectFilter( UIDocument uidoc, List<string> filterNameList, string mess, ref ElementId id )
    {
      id = null ;

      ISelectionFilter selFilters = new FamilySelectionListFilter( filterNameList ) ;
      Selection selection = uidoc.Selection ;
      try {
        Reference pickedElement = selection.PickObject( ObjectType.Element, selFilters, mess ) ;

        id = pickedElement.ElementId ;
      }
      catch ( OperationCanceledException ) {
        return false ;
      }
      catch ( Exception e ) {
        return false ;
      }

      return true ;
    }


    public static ElementId GetLevelID( Document doc, string levelName )
    {
      if ( doc == null ) {
        return null ;
      }

      ElementId elemId = null ;
      IEnumerable<Element> elems = null ;
      elems = from elem in ( new FilteredElementCollector( doc ).OfClass( typeof( Level ) ).ToElements() )
        let level = elem as Level
        where level.Name == levelName
        select level ;

      foreach ( Level lv in elems ) {
        elemId = lv.Id ;
      }

      return elemId ;
    }

    /// <summary>
    /// 指定ビューを取得
    /// </summary>
    /// <param name="doc">Document</param>
    /// <param name="name">ビュー名</param>
    public static Autodesk.Revit.DB.View getView( Document doc, string viewName, ViewType type )
    {
      Autodesk.Revit.DB.View getV = null ;

      IList<Element> views = new FilteredElementCollector( doc ).OfClass( typeof( Autodesk.Revit.DB.View ) )
        .ToElements() ;
      foreach ( Autodesk.Revit.DB.View v in views ) {
        if ( v.Name == viewName && v.ViewType == type ) {
          getV = v ;
          break ;
        }
      }

      return getV ;
    }

    /// <summary>
    /// 
    /// </summary>
    public class FamilySelectionListFilter : ISelectionFilter
    {
      List<string> m_famNameList = null ;

      /// <summary>
      /// ファミリインスタンスのファミリ名でフィルターをかける
      /// </summary>
      /// <param name="famNameList"></param>
      public FamilySelectionListFilter( List<string> famNameList )
      {
        m_famNameList = famNameList ;
      }

      public bool AllowElement( Element element )
      {
        for ( int i = 0 ; i < m_famNameList.Count ; i++ ) {
          try {
            FamilyInstance instance = element as FamilyInstance ;
            if ( instance == null ) {
              continue ;
            }

            if ( instance.Symbol.FamilyName == m_famNameList[ i ] ) {
              return true ;
            }
          }
          catch {
            continue ;
          }
        }

        return false ;
      }

      public bool AllowReference( Reference refer, XYZ point )
      {
        return false ;
      }
    }
  } //ClsWaritsuke
}