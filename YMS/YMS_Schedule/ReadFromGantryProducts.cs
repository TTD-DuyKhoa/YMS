using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using YMS_gantry ;
using Autodesk.Revit.DB ;

namespace YMS_Schedule
{
  public class ReadFromGantryProducts
  {
    /// <summary>
    /// 大引(桁受)-根太(主桁)の交差するセットを返す
    /// </summary>
    /// <param name="doc">現在のドキュメント</param>
    /// <param name="targetSlope">スロープ部の部材のみを拾うか true=スロープの部材のみを対象とする false=スロープ以外の部材を対象とする Default=false</param>
    /// <param name="koudaiName">特定の構台のみを対象とするか 空=図面内のすべての構台から部材を収集する 指定:指定した構台からのみ部材を収集する  Default=""</param>
    /// <param name="includeShikigeta">敷桁を含めるかtrue=敷桁と根太の交点も対象とする　false=敷桁を対象としない Default=false</param>
    /// <returns></returns>
    /// <remarks>半がかり(スロープとフラット部の交点にある大引)はフラット部として扱う</remarks>
    public static List<(FamilyInstance, List<FamilyInstance>)> GetOhbikiAndNedaSet( Document doc,
      bool targetSlope = false, string koudaiName = "", bool includeShikigeta = false )
    {
      List<(FamilyInstance, List<FamilyInstance>)> retList = new List<(FamilyInstance, List<FamilyInstance>)>() ;
      try {
        //指定した構台名の部材を収集(構台名未指定の場合は図面上すべての部材を収集)
        var ms = string.IsNullOrEmpty( koudaiName )
          ? MaterialSuper.Collect( doc )
          : MaterialSuper.Collect( doc ).Where( x => x.m_KodaiName == koudaiName ) ;
        if ( ms == null || ms.Count() < 1 ) {
          return new List<(FamilyInstance, List<FamilyInstance>)>() ;
        }

        List<MaterialSuper> materilas = ms.ToList() ;

        //大引のみ抽出
        List<OhbikiSuper> lstOhbiki = materilas.Where( x => x.GetType() == typeof( Ohbiki ) )
          .Select( x => x as OhbikiSuper )?.ToList() ?? null ;
        //敷桁を含む場合は敷桁も
        if ( includeShikigeta ) {
          List<OhbikiSuper> lstShikigeta = materilas.Where( x => x.GetType() == typeof( Shikigeta ) )
            .Select( x => x as OhbikiSuper )?.ToList() ?? null ;
          if ( lstShikigeta != null && lstShikigeta.Count > 0 ) lstOhbiki.AddRange( lstShikigeta ) ;
        }

        if ( lstOhbiki == null ) {
          return new List<(FamilyInstance, List<FamilyInstance>)>() ;
        }

        //根太のみ抽出
        List<Neda> lstNeda = materilas.Where( x => x.GetType() == typeof( Neda ) ).Select( x => x as Neda ).ToList() ;
        if ( lstNeda == null ) {
          return new List<(FamilyInstance, List<FamilyInstance>)>() ;
        }

        List<ElementId> nedaIds = lstNeda.Where( x => x != null ).Select( x => x.m_ElementId ).ToList() ;

        //大引に接する根太のリストを作成
        foreach ( OhbikiSuper ohbiki in lstOhbiki ) {
          FamilyInstance ohbikiIns = doc.GetElement( ohbiki.m_ElementId ) as FamilyInstance ;

          //接する根太のインスタンスを収集
          List<ElementId> intsId =
            RevitUtil.ClsRevitUtil.GetIntersectFamilys( doc, ohbiki.m_ElementId, serchIds: nedaIds ) ;
          List<FamilyInstance> nedaIns = new List<FamilyInstance>() ;
          List<FamilyInstance> nedaSlopeIns = new List<FamilyInstance>() ;

          List<ElementId> intersectNedaIds = new List<ElementId>() ;
          List<ElementId> intersectSlopeNedaIds = new List<ElementId>() ;

          foreach ( ElementId id in intsId ) {
            ////自分（根太）以外リスト
            //List<ElementId> igList = new List<ElementId>();
            //igList.AddRange(intsId);
            //igList.Remove(id);

            FamilyInstance nIns = doc.GetElement( id ) as FamilyInstance ;
            if ( nIns != null ) {
              List<XYZ> pnts = GantryUtil.GetCurvePoints( doc, id ) ;
              XYZ nedaDir = ( pnts[ 1 ] - pnts[ 0 ] ) ;


              List<ElementId> tmpList = new List<ElementId>() ;
              if ( intersectNedaIds.Count > 0 ) {
                tmpList = RevitUtil.ClsRevitUtil.GetIntersectFamilys( doc, id, 0.1, serchIds: intersectNedaIds ) ;
              }

              //List<ElementId> tmpList2 = new List<ElementId>();
              //if (intersectSlopeNedaIds.Count > 0)
              //{
              //    tmpList2 = RevitUtil.ClsRevitUtil.GetIntersectFamilys(doc, id, 0.1, serchIds: intersectSlopeNedaIds);
              //}


              if ( ! RevitUtil.ClsGeo.GEO_EQ0( nedaDir.Z ) ) {
                if ( tmpList.Count < 1 ) {
                  nedaSlopeIns.Add( nIns ) ;
                  intersectSlopeNedaIds.Add( nIns.Id ) ;
                }
              }
              else {
                if ( tmpList.Count < 1 ) {
                  nedaIns.Add( nIns ) ;
                  intersectNedaIds.Add( nIns.Id ) ;
                }
              }
            }
          }

          //スロープのみ
          if ( targetSlope && nedaSlopeIns.Count > 0 && nedaIns.Count == 0 ) {
            //根太に角度がついていたら含める
            retList.Add( ( ohbikiIns, nedaSlopeIns ) ) ;
          }
          else if ( ! targetSlope && nedaIns.Count > 0 ) {
            //スロープ以外を収集
            retList.Add( ( ohbikiIns, nedaIns ) ) ;
          }
          ////全てが対象ならすべて含める
          //else
          //{
          //    nedaSlopeIns.AddRange(nedaIns);
          //    retList.Add((ohbikiIns, nedaSlopeIns));
          //}
        }
      }
      catch ( Exception ex ) {
        retList = new List<(FamilyInstance, List<FamilyInstance>)>() ;
      }

      return retList ;
    }

    /// <summary>
    /// 対象の根太が、同じ大引きを共有しているスロープ根太と結合しているか　又は対象のスロープ根太が、同じ大引きを共有している根太と結合しているか
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="nedaID"></param>
    /// <param name="koudaiName"></param>
    /// <param name="includeShikigeta"></param>
    /// <returns></returns>
    public static bool SlopenedaAndNedaConnect( Document doc, ElementId nedaID, ElementId ohbikiID = null,
      string koudaiName = "", bool includeShikigeta = false )
    {
      bool hit = false ;

      List<(FamilyInstance, List<FamilyInstance>)> retList = new List<(FamilyInstance, List<FamilyInstance>)>() ;
      try {
        //指定した構台名の部材を収集(構台名未指定の場合は図面上すべての部材を収集)
        var ms = string.IsNullOrEmpty( koudaiName )
          ? MaterialSuper.Collect( doc )
          : MaterialSuper.Collect( doc ).Where( x => x.m_KodaiName == koudaiName ) ;
        if ( ms == null || ms.Count() < 1 ) {
          return false ;
        }

        List<MaterialSuper> materilas = ms.ToList() ;

        //大引のみ抽出
        List<OhbikiSuper> lstOhbiki = materilas.Where( x => x.GetType() == typeof( Ohbiki ) )
          .Select( x => x as OhbikiSuper )?.ToList() ?? null ;
        //敷桁を含む場合は敷桁も
        if ( includeShikigeta ) {
          List<OhbikiSuper> lstShikigeta = materilas.Where( x => x.GetType() == typeof( Shikigeta ) )
            .Select( x => x as OhbikiSuper )?.ToList() ?? null ;
          if ( lstShikigeta != null && lstShikigeta.Count > 0 ) lstOhbiki.AddRange( lstShikigeta ) ;
        }

        if ( lstOhbiki == null ) {
          return false ;
        }

        //根太のみ抽出
        List<Neda> lstNeda = materilas.Where( x => x.GetType() == typeof( Neda ) ).Select( x => x as Neda ).ToList() ;
        if ( lstNeda == null ) {
          return false ;
        }

        List<ElementId> nedaIds = lstNeda.Where( x => x != null ).Select( x => x.m_ElementId ).ToList() ;

        //大引に接する根太のリストを作成
        List<ElementId> intersectNedaIds = new List<ElementId>() ;
        List<ElementId> intersectSlopeNedaIds = new List<ElementId>() ;
        bool isSlope = false ;

        foreach ( OhbikiSuper ohbiki in lstOhbiki ) {
          if ( ohbikiID != null && ohbikiID != ohbiki.m_ElementId ) {
            continue ;
          }

          // FamilyInstance ohbikiIns = doc.GetElement(ohbiki.m_ElementId) as FamilyInstance;

          //接する根太のインスタンスを収集
          List<ElementId> intsId =
            RevitUtil.ClsRevitUtil.GetIntersectFamilys( doc, ohbiki.m_ElementId, serchIds: nedaIds ) ;
          //List<FamilyInstance> nedaIns = new List<FamilyInstance>();
          //List<FamilyInstance> nedaSlopeIns = new List<FamilyInstance>();

          foreach ( ElementId id in intsId ) {
            List<XYZ> pnts = GantryUtil.GetCurvePoints( doc, id ) ;
            XYZ nedaDir = ( pnts[ 1 ] - pnts[ 0 ] ) ;
            if ( ! RevitUtil.ClsGeo.GEO_EQ0( nedaDir.Z ) ) {
              if ( id == nedaID ) {
                isSlope = true ;
                continue ;
              }

              intersectSlopeNedaIds.Add( id ) ;
            }
            else {
              if ( id == nedaID ) {
                isSlope = false ;
                //continue;
              }

              intersectNedaIds.Add( id ) ;
            }
          }
        }

        if ( isSlope ) {
          List<ElementId> tmpList = new List<ElementId>() ;
          tmpList = RevitUtil.ClsRevitUtil.GetIntersectFamilys( doc, nedaID, 0.1, serchIds: intersectNedaIds ) ;
          if ( tmpList.Count > 0 ) {
            hit = true ;
          }
        }
        else {
          List<ElementId> tmpList = new List<ElementId>() ;
          tmpList = RevitUtil.ClsRevitUtil.GetIntersectFamilys( doc, nedaID, 0.1, serchIds: intersectSlopeNedaIds ) ;
          if ( tmpList.Count > 0 ) {
            hit = true ;
          }
        }
      }
      catch ( Exception ex ) {
        hit = false ;
        return hit ;
      }

      return hit ;
    }

    /// <summary>
    /// 杭(または継足し杭、支柱)と大引（桁受)の交差判定結果をセットで返す
    /// </summary>
    /// <param name="doc">現在のドキュメント</param>
    /// <param name="koudaiName">特定の構台のみを対象とするか 空=図面内のすべての構台から部材を収集する 指定:指定した構台からのみ部材を収集する  Default=""</param>
    /// <param name="includeShikigeta">敷桁を含めるかtrue=敷桁と根太の交点も対象とする　false=敷桁を対象としない Default=false</param>
    /// <returns></returns>
    public static List<(FamilyInstance, List<FamilyInstance>)> GetPilePillerAndKetaUkeSet( Document doc,
      string koudaiName = "", bool includeShikigeta = false )
    {
      List<(FamilyInstance, List<FamilyInstance>)> retList = new List<(FamilyInstance, List<FamilyInstance>)>() ;
      try {
        //指定した構台名の部材を収集(構台名未指定の場合は図面上すべての部材を収集)
        var ms = string.IsNullOrEmpty( koudaiName )
          ? MaterialSuper.Collect( doc )
          : MaterialSuper.Collect( doc ).Where( x => x.m_KodaiName == koudaiName ) ;
        if ( ms == null || ms.Count() < 1 ) {
          return new List<(FamilyInstance, List<FamilyInstance>)>() ;
        }

        List<MaterialSuper> materilas = ms.ToList() ;

        //杭関連ファミリのみ抽出
        List<PilePillerSuper> lstPiler = materilas.Where( x => x.GetType() == typeof( PilePiller ) )
          .Select( x => x as PilePillerSuper ).ToList() ;
        if ( lstPiler == null ) {
          return new List<(FamilyInstance, List<FamilyInstance>)>() ;
        }

        List<PilePillerSuper> lstExPiler = materilas.Where( x => x.GetType() == typeof( ExtensionPile ) )
          .Select( x => x as PilePillerSuper ).ToList() ;
        if ( lstExPiler.Count > 0 ) {
          lstPiler.AddRange( lstExPiler ) ;
        }

        //大引のみ抽出
        List<OhbikiSuper> lstOhbiki = materilas.Where( x => x.GetType() == typeof( Ohbiki ) )
          .Select( x => x as OhbikiSuper )?.ToList() ?? null ;
        //敷桁を含む場合は敷桁も
        if ( includeShikigeta ) {
          List<OhbikiSuper> lstShikigeta = materilas.Where( x => x.GetType() == typeof( Shikigeta ) )
            .Select( x => x as OhbikiSuper )?.ToList() ?? null ;
          if ( lstShikigeta != null && lstShikigeta.Count > 0 ) lstOhbiki.AddRange( lstShikigeta ) ;
        }

        if ( lstOhbiki == null ) {
          return new List<(FamilyInstance, List<FamilyInstance>)>() ;
        }

        List<ElementId> ohbikiIds = lstOhbiki.Where( x => x != null ).Select( x => x.m_ElementId ).ToList() ;

        //杭に接する大引（桁受)のリストを作成
        foreach ( PilePillerSuper piller in lstPiler ) {
          FamilyInstance pillerIns = doc.GetElement( piller.m_ElementId ) as FamilyInstance ;

          //接するインスタンスを収集
          List<ElementId> intsId =
            RevitUtil.ClsRevitUtil.GetIntersectFamilys( doc, piller.m_ElementId, 2, serchIds: ohbikiIds ) ;

          List<FamilyInstance> ohbikiIns = new List<FamilyInstance>() ;
          foreach ( ElementId id in intsId ) {
            FamilyInstance nIns = doc.GetElement( id ) as FamilyInstance ;
            if ( nIns != null ) {
              ohbikiIns.Add( nIns ) ;
            }
          }

          retList.Add( ( pillerIns, ohbikiIns ) ) ;
        }
      }
      catch ( Exception ex ) {
        return new List<(FamilyInstance, List<FamilyInstance>)>() ;
      }

      return retList ;
    }

    /// <summary>
    /// 杭(または継足し杭、支柱)と水平ブレスの交差判定結果をセットで返す
    /// </summary>
    /// <param name="doc">現在のドキュメント</param>
    /// <param name="koudaiName">特定の構台のみを対象とするか 空=図面内のすべての構台から部材を収集する 指定:指定した構台からのみ部材を収集する  Default=""</param>
    /// <returns></returns>
    public static List<(FamilyInstance, List<FamilyInstance>)> GetPilePillerAndHBraceSet( Document doc,
      string koudaiName = "" )
    {
      List<(FamilyInstance, List<FamilyInstance>)> retList = new List<(FamilyInstance, List<FamilyInstance>)>() ;
      try {
        //指定した構台名の部材を収集(構台名未指定の場合は図面上すべての部材を収集)
        var ms = string.IsNullOrEmpty( koudaiName )
          ? MaterialSuper.Collect( doc )
          : MaterialSuper.Collect( doc ).Where( x => x.m_KodaiName == koudaiName ) ;
        if ( ms == null || ms.Count() < 1 ) {
          return new List<(FamilyInstance, List<FamilyInstance>)>() ;
        }

        List<MaterialSuper> materilas = ms.ToList() ;

        //杭関連ファミリのみ抽出
        List<PilePillerSuper> lstPiler = materilas.Where( x => x.GetType() == typeof( PilePiller ) )
          .Select( x => x as PilePillerSuper ).ToList() ;
        if ( lstPiler == null ) {
          return new List<(FamilyInstance, List<FamilyInstance>)>() ;
        }

        List<PilePillerSuper> lstExPiler = materilas.Where( x => x.GetType() == typeof( ExtensionPile ) )
          .Select( x => x as PilePillerSuper ).ToList() ;
        if ( lstExPiler.Count > 0 ) {
          lstPiler.AddRange( lstExPiler ) ;
        }

        //水平ブレス取得
        List<FamilyInstance> suiheiBraceLst =
          ReadFromGantryProducts.GetFamilyInstanceListBy( doc, ReadFromGantryProducts.eElementType.HBrace ) ;
        List<ElementId> HBraceIds = new List<ElementId>() ;
        foreach ( FamilyInstance fi in suiheiBraceLst ) {
          HBraceIds.Add( fi.Id ) ;
        }
        //List<OhbikiSuper> lstOhbiki = materilas.Where(x => x.GetType() == typeof(Ohbiki)).Select(x => x as OhbikiSuper)?.ToList() ?? null;

        if ( suiheiBraceLst == null ) {
          return new List<(FamilyInstance, List<FamilyInstance>)>() ;
        }
        //List<ElementId> ohbikiIds = suiheiBraceLst.Where(x => x != null).Select(x => x.m_ElementId).ToList();

        //杭に接する水平ブレスのリストを作成
        foreach ( PilePillerSuper piller in lstPiler ) {
          FamilyInstance pillerIns = doc.GetElement( piller.m_ElementId ) as FamilyInstance ;

          //接する水平ブレスのインスタンスを収集
          List<ElementId> intsId =
            RevitUtil.ClsRevitUtil.GetIntersectFamilys( doc, piller.m_ElementId, 1, serchIds: HBraceIds ) ;

          List<FamilyInstance> HBIns = new List<FamilyInstance>() ;
          foreach ( ElementId id in intsId ) {
            FamilyInstance nIns = doc.GetElement( id ) as FamilyInstance ;
            if ( nIns != null ) {
              HBIns.Add( nIns ) ;
            }
          }

          retList.Add( ( pillerIns, HBIns ) ) ;
        }
      }
      catch ( Exception ex ) {
        return new List<(FamilyInstance, List<FamilyInstance>)>() ;
      }

      return retList ;
    }

    /// <summary>
    /// 大引きと水平ブレスの交差判定結果をセットで返す
    /// </summary>
    /// <param name="doc">現在のドキュメント</param>
    /// <param name="koudaiName">特定の構台のみを対象とするか 空=図面内のすべての構台から部材を収集する 指定:指定した構台からのみ部材を収集する  Default=""</param>
    /// <returns></returns>
    public static List<(FamilyInstance, List<FamilyInstance>)> GetOhbikiAndHBraceSet( Document doc,
      string koudaiName = "", bool includeShikigeta = false )
    {
      List<(FamilyInstance, List<FamilyInstance>)> retList = new List<(FamilyInstance, List<FamilyInstance>)>() ;
      try {
        //指定した構台名の部材を収集(構台名未指定の場合は図面上すべての部材を収集)
        var ms = string.IsNullOrEmpty( koudaiName )
          ? MaterialSuper.Collect( doc )
          : MaterialSuper.Collect( doc ).Where( x => x.m_KodaiName == koudaiName ) ;
        if ( ms == null || ms.Count() < 1 ) {
          return new List<(FamilyInstance, List<FamilyInstance>)>() ;
        }

        List<MaterialSuper> materilas = ms.ToList() ;


        //水平ブレス取得
        List<FamilyInstance> suiheiBraceLst =
          ReadFromGantryProducts.GetFamilyInstanceListBy( doc, ReadFromGantryProducts.eElementType.HBrace ) ;
        List<ElementId> HBraceIds = new List<ElementId>() ;
        foreach ( FamilyInstance fi in suiheiBraceLst ) {
          HBraceIds.Add( fi.Id ) ;
        }
        //List<OhbikiSuper> lstOhbiki = materilas.Where(x => x.GetType() == typeof(Ohbiki)).Select(x => x as OhbikiSuper)?.ToList() ?? null;

        if ( suiheiBraceLst == null ) {
          return new List<(FamilyInstance, List<FamilyInstance>)>() ;
        }

        List<OhbikiSuper> lstOhbiki =
          materilas.Where( x => x.GetType() == typeof( Ohbiki ) && x.m_Size.StartsWith( "[" ) )
            .Select( x => x as OhbikiSuper )?.ToList() ?? null ;
        //敷桁を含む場合は敷桁も
        if ( includeShikigeta ) {
          List<OhbikiSuper> lstShikigeta = materilas.Where( x => x.GetType() == typeof( Shikigeta ) )
            .Select( x => x as OhbikiSuper )?.ToList() ?? null ;
          if ( lstShikigeta != null && lstShikigeta.Count > 0 ) lstOhbiki.AddRange( lstShikigeta ) ;
        }

        if ( lstOhbiki == null ) {
          return new List<(FamilyInstance, List<FamilyInstance>)>() ;
        }

        List<ElementId> ohbikiIds = lstOhbiki.Where( x => x != null ).Select( x => x.m_ElementId ).ToList() ;

        //大引きに接する水平ブレスのリストを作成
        foreach ( OhbikiSuper ohbiki in lstOhbiki ) {
          FamilyInstance ohbikiIns = doc.GetElement( ohbiki.m_ElementId ) as FamilyInstance ;

          //接する水平ブレスのインスタンスを収集
          List<ElementId> intsId =
            RevitUtil.ClsRevitUtil.GetIntersectFamilys( doc, ohbiki.m_ElementId, 1, serchIds: HBraceIds ) ;

          List<FamilyInstance> HBIns = new List<FamilyInstance>() ;
          foreach ( ElementId id in intsId ) {
            FamilyInstance nIns = doc.GetElement( id ) as FamilyInstance ;
            if ( nIns != null ) {
              HBIns.Add( nIns ) ;
            }
          }

          retList.Add( ( ohbikiIns, HBIns ) ) ;
        }
      }
      catch ( Exception ex ) {
        return new List<(FamilyInstance, List<FamilyInstance>)>() ;
      }

      return retList ;
    }

    /// <summary>
    /// チャンネル桁受同士の接触からその部材および接触している桁受のリストを返す
    /// </summary>
    /// <param name="doc">現在のドキュメント</param>
    /// <param name="koudaiName">特定の構台のみを対象とするか 空=図面内のすべての構台から部材を収集する 指定:指定した構台からのみ部材を収集する  Default=""</param>
    /// <returns></returns>
    public static List<(FamilyInstance, List<FamilyInstance>)> GetChannelKetaukeSet( Document doc,
      string koudaiName = "" )
    {
      List<(FamilyInstance, List<FamilyInstance>)> retList = new List<(FamilyInstance, List<FamilyInstance>)>() ;
      try {
        //指定した構台名の部材を収集(構台名未指定の場合は図面上すべての部材を収集)
        var ms = string.IsNullOrEmpty( koudaiName )
          ? MaterialSuper.Collect( doc )
          : MaterialSuper.Collect( doc ).Where( x => x.m_KodaiName == koudaiName ) ;
        if ( ms == null || ms.Count() < 1 ) {
          return new List<(FamilyInstance, List<FamilyInstance>)>() ;
        }

        List<MaterialSuper> materilas = ms.ToList() ;

        //杭関連ファミリのみ抽出
        List<PilePillerSuper> lstPiler = materilas.Where( x => x.GetType() == typeof( PilePillerSuper ) )
          .Select( x => x as PilePillerSuper ).ToList() ;
        if ( lstPiler == null ) {
          return new List<(FamilyInstance, List<FamilyInstance>)>() ;
        }

        //チャンネル材の桁受のみ抽出
        List<OhbikiSuper> lstOhbiki =
          materilas.Where( x => x.GetType() == typeof( Ohbiki ) && x.m_Size.StartsWith( "[" ) )
            .Select( x => x as OhbikiSuper )?.ToList() ?? null ;
        if ( lstOhbiki == null ) {
          return new List<(FamilyInstance, List<FamilyInstance>)>() ;
        }

        List<ElementId> ohbikiIds = lstOhbiki.Where( x => x != null ).Select( x => x.m_ElementId ).ToList() ;

        //大引に接する根太のリストを作成
        foreach ( OhbikiSuper ohbiki in lstOhbiki ) {
          FamilyInstance oIns = doc.GetElement( ohbiki.m_ElementId ) as FamilyInstance ;
          Curve oCu = GantryUtil.GetCurve( doc, ohbiki.m_ElementId ) ;

          //接する根太のインスタンスを収集
          List<ElementId> ohbikiintIds = ohbikiIds.ToList() ;
          //自信は除く
          ohbikiintIds.Remove( ohbiki.m_ElementId ) ;
          List<ElementId> intsId =
            RevitUtil.ClsRevitUtil.GetIntersectFamilys( doc, ohbiki.m_ElementId, serchIds: ohbikiintIds ) ;
          List<FamilyInstance> ohbikiIns = new List<FamilyInstance>() ;
          BoundingBoxXYZ bb = oIns.get_BoundingBox( null ) ;
          foreach ( ElementId id in intsId ) {
            List<XYZ> pntsList = GantryUtil.FindIntersectionPnts( doc, ohbiki.m_ElementId, id ) ;
            if ( pntsList.Count <= 2 ) {
              continue ;
            }

            double totalX = 0.0 ;
            double totalY = 0.0 ;
            double totalZ = 0.0 ;
            // すべての点の座標を合計する
            foreach ( var point in pntsList ) {
              totalX += point.X ;
              totalY += point.Y ;
              totalZ += point.Z ;
            }

            // 平均値を計算する
            double centerX = totalX / pntsList.Count ;
            double centerY = totalY / pntsList.Count ;
            double centerZ = totalZ / pntsList.Count ;
            XYZ center = new XYZ( centerX, centerY, centerZ ) ;
            XYZ pp = GantryUtil.ProjectPointToCurve( center, oCu ) ;
            if ( pp.DistanceTo( oCu.GetEndPoint( 0 ) ) < RevitUtil.ClsRevitUtil.CovertToAPI( 1 ) ||
                 pp.DistanceTo( oCu.GetEndPoint( 1 ) ) < RevitUtil.ClsRevitUtil.CovertToAPI( 1 ) ) {
              continue ;
            }

            FamilyInstance nIns = doc.GetElement( id ) as FamilyInstance ;
            if ( nIns != null ) {
              ohbikiIns.Add( nIns ) ;
            }
          }

          retList.Add( ( oIns, ohbikiIns ) ) ;
        }
      }
      catch ( Exception ex ) {
        return new List<(FamilyInstance, List<FamilyInstance>)>() ;
      }

      return retList ;
    }

    /// <summary>
    /// 垂直ブレス、水平ブレスのボルト集計に必要な箇所数を返す
    /// </summary>
    /// <param name="doc">アクティブドキュメント</param>
    /// <param name="eType">垂直ブレス、水平ブレス、取付補助材と杭、切梁受</param>
    /// <param name="eSide">ウェブ側、フランジ側</param>
    /// <returns></returns>
    public static int GetKoudaivBracehJointBoltCnt( Document doc, eBoltElement eType, eAttachSide eSide )
    {
      int retInt = 0 ;

      try {
        var ms = MaterialSuper.Collect( doc ) ;
        if ( ms == null || ms.Count() < 1 ) {
          return 0 ;
        }

        List<MaterialSuper> materilas = ms.ToList() ;
        //柱,支柱
        List<ElementId> pillers = materilas.Select( x => x as PilePillerSuper )
          .Where( x => x != null && x.m_ElementId != ElementId.InvalidElementId ).Select( x => x.m_ElementId )
          .ToList() ;

        //垂直ブレス
        if ( eType == eBoltElement.VBrace ) {
          if ( eAttachSide.Flange == eSide ) {
            retInt = GetVBracesAttachCount( doc, ReadFromGantryProducts.eAttachSide.Flange ) ;
          }
          else {
            retInt = GetVBracesAttachCount( doc, ReadFromGantryProducts.eAttachSide.Web ) ;
          }
        }
        //水平ツナギ
        else if ( eType == eBoltElement.HJoint ) {
          if ( eAttachSide.Flange == eSide ) {
            retInt = GetHJointAttachCount( doc, ReadFromGantryProducts.eAttachSide.Flange ) ;
          }
          else {
            retInt = GetHJointAttachCount( doc, ReadFromGantryProducts.eAttachSide.Web ) ;
          }
        }
        //杭のフランジについている取付補助材
        else if ( eType == eBoltElement.Support && eAttachSide.Web == eSide ) {
          retInt = GetVBracesAttachCount( doc, ReadFromGantryProducts.eAttachSide.Web ) ;
          retInt += GetHJointAttachCount( doc, ReadFromGantryProducts.eAttachSide.Web ) ;
        }
        //切梁受
        else if ( eType == eBoltElement.Kiribari ) {
          if ( eAttachSide.Flange == eSide ) {
            retInt = GetKiribariUkeAttachCnt( doc, ReadFromGantryProducts.eAttachSide.Flange ) ;
          }
          else {
            retInt = GetKiribariUkeAttachCnt( doc, ReadFromGantryProducts.eAttachSide.Web ) ;
          }
        }
      }
      catch ( Exception ex ) {
        retInt = 0 ;
      }

      return retInt ;
    }

    /// <summary>
    /// 垂直ブレスについている取付補助材締結用ボルト箇所をカウントする
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="eSide"></param>
    /// <returns></returns>
    private static int GetVBracesAttachCount( Document doc, eAttachSide eSide )
    {
      int retInt = 0 ;
      try {
        List<FamilyInstance> teiketsuList = GetAttachSupportList( doc, eBoltElement.VBrace ) ;
        List<ElementId> supportId =
          teiketsuList.Count > 0 ? teiketsuList.Select( x => x.Id ).ToList() : new List<ElementId>() ;
        var ms = MaterialSuper.Collect( doc ) ;
        if ( ms == null || ms.Count() < 1 ) {
          return 0 ;
        }

        List<MaterialSuper> materilas = ms.ToList() ;
        //柱,支柱
        List<ElementId> pillers = materilas.Select( x => x as PilePillerSuper )
          .Where( x => x != null && x.m_ElementId != ElementId.InvalidElementId ).Select( x => x.m_ElementId )
          .ToList() ;
        List<FamilyInstance> vBrace = GetFamilyInstanceListBy( doc, eElementType.VBrace ) ;

        List<ElementId> webList = new List<ElementId>() ;
        //ブレースと触れる柱を対象とし、交点を求める
        foreach ( FamilyInstance ins in vBrace ) {
          VerticalBrace brace = MaterialSuper.ReadFromElement( ins.Id, doc ) as VerticalBrace ;
          //アングル材以外のブレース（ターンバックル）は対象外
          if ( brace.MaterialSize().Shape != MaterialShape.L ) {
            continue ;
          }

          //List<ElementId> intsId = new List<ElementId>();
          List<ElementId> pillerIds =
            RevitUtil.ClsRevitUtil.GetIntersectFamilys( doc, brace.m_ElementId, serchIds: pillers ) ;
          Curve c = GantryUtil.GetCurve( doc, brace.m_ElementId ) ;
          XYZ braceVec = ( c.GetEndPoint( 1 ) - c.GetEndPoint( 0 ) ) ;
          if ( braceVec.Z < 0 ) {
            c = Line.CreateBound( c.GetEndPoint( 1 ), c.GetEndPoint( 0 ) ) ;
            braceVec = ( c.GetEndPoint( 1 ) - c.GetEndPoint( 0 ) ) ;
          }

          XYZ bracePlaneVec = new XYZ( braceVec.X, braceVec.Y, 0 ) ;
          MaterialSize bS = brace.MaterialSize() ;
          double bRotate = bracePlaneVec.AngleTo( braceVec ) ;
          Transform ts = ins.GetTotalTransform() ;
          if ( ts.BasisZ.Y == -1 ) {
            bool b = true ;
          }

          List<ElementId> supprtIds = RevitUtil.ClsRevitUtil.GetIntersectFamilys( doc, ins.Id, serchIds: supportId ) ;
          if ( supportId.Count <= 0 ) {
            supprtIds = new List<ElementId>() ;
          }

          foreach ( ElementId id in pillerIds ) {
            FamilyInstance pillerIns = doc.GetElement( id ) as FamilyInstance ;
            XYZ pillerHandle = pillerIns.HandOrientation ;
            List<XYZ> pnts = GantryUtil.FindIntersectionPnts( doc, brace.m_ElementId, id ) ;
            bool isFlange = false ;
            XYZ pDirection = pillerIns.HandOrientation.Normalize() ;
            XYZ targetD = ( (Line) c ).Direction.Normalize() ;
            targetD = new XYZ( targetD.X, targetD.Y, 0 ) ;

            //ファミリの向きでｳｪﾌﾞ面かフランジ面かを判定
            if ( YMS_gantry.GantryUtil.AreComponentsSameSign( pDirection, targetD ) ||
                 YMS_gantry.GantryUtil.AreComponentsSameSign( pDirection, targetD.Negate() ) ) {
              isFlange = true ;
            }

            if ( pnts.Count > 1 ) {
              //フランジ側　柱との交点数を返す
              if ( isFlange && eSide == eAttachSide.Flange ) {
                retInt += 1 ;
              }
              //ウェブ側　取付補助材の数を返す
              else if ( ! isFlange && eSide == eAttachSide.Web ) {
                if ( supprtIds.Count > 0 ) {
                  var pInters = RevitUtil.ClsRevitUtil.GetIntersectFamilys( doc, id, serchIds: supportId ) ;
                  var jInters = RevitUtil.ClsRevitUtil.GetIntersectFamilys( doc, ins.Id, serchIds: supportId ) ;
                  var supId = pInters.Where( x => jInters.Contains( x ) ) ;
                  if ( supId.Any() ) {
                    webList.AddRange( supId ) ;
                  }
                }
              }
            }
          }
        }

        if ( eSide == eAttachSide.Web ) {
          webList.Distinct() ;
          retInt = webList.Count() ;
        }
      }
      catch ( Exception ex ) {
        retInt = 0 ;
      }

      return retInt ;
    }

    /// <summary>
    /// 締結補助材との干渉チェックをよけるためのリスト
    /// </summary>
    /// <returns></returns>
    private static List<string> teikesuList()
    {
      return new List<string>() { "ﾌﾞﾙﾏﾝ", "ﾘｷﾏﾝ", "取付補助材" } ;
    }

    /// <summary>
    /// 水平ツナギについている取付補助材締結用ボルト箇所をカウントする
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="eSide"></param>
    /// <returns></returns>
    private static int GetHJointAttachCount( Document doc, eAttachSide eSide )
    {
      double retInt = 0 ;
      try {
        List<FamilyInstance> teiketsuList = GetAttachSupportList( doc, eBoltElement.HJoint ) ;
        List<ElementId> supportId =
          teiketsuList.Count > 0 ? teiketsuList.Select( x => x.Id ).ToList() : new List<ElementId>() ;
        var ms = MaterialSuper.Collect( doc ) ;
        if ( ms == null || ms.Count() < 1 ) {
          return 0 ;
        }

        List<MaterialSuper> materilas = ms.ToList() ;

        //水平ツナギ
        List<ElementId> joints = materilas.Select( x => x as HorizontalJoint )
          .Where( x => x != null && x.m_ElementId != ElementId.InvalidElementId ).Select( x => x.m_ElementId )
          .ToList() ;
        if ( joints.Count < 1 ) {
          return 0 ;
        }

        //柱、杭
        List<PilePillerSuper> pillers = materilas.Select( x => x as PilePillerSuper )
          .Where( x => x != null && x.m_ElementId != ElementId.InvalidElementId ).ToList() ;
        List<ElementId> webList = new List<ElementId>() ;
        //柱と触れる部材を対象とし、交点を求める
        foreach ( PilePillerSuper pil in pillers ) {
          FamilyInstance pillerIns = doc.GetElement( pil.m_ElementId ) as FamilyInstance ;
          //柱に接するファミリのリスト(締結補助材は除く)
          List<ElementId> intsId = RevitUtil.ClsRevitUtil.GetIntersectFamilys( doc, pil.m_ElementId,
            ignorelist: teikesuList(), serchIds: joints ) ;
          //柱に接するツナギ材だけを収集
          List<ElementId> jointIds = intsId.Where( x => joints.Contains( x ) ).ToList() ;
          //柱の基点
          XYZ pillerP = pil.Position ;

          foreach ( ElementId jId in jointIds ) {
            FamilyInstance jins = doc.GetElement( jId ) as FamilyInstance ;
            Curve jC = GantryUtil.GetCurve( doc, jId ) ;
            XYZ jointVec = ( jC.GetEndPoint( 1 ) - jC.GetEndPoint( 0 ) ).Normalize() ;
            //ツナギ材に接する別のツナギ材を取得
            List<ElementId> jintsId =
              RevitUtil.ClsRevitUtil.GetIntersectFamilys( doc, jId, ignorelist: teikesuList(), serchIds: jointIds ) ;
            List<ElementId> jjIds = jintsId.Where( x => joints.Contains( x ) && intsId.Contains( x ) ).ToList() ;
            //このツナギがフランジ方向に平行か
            bool isFlange = GantryUtil.AreComponentsSameSign( pillerIns.HandOrientation, jointVec ) ||
                            GantryUtil.AreComponentsSameSign( pillerIns.HandOrientation, jointVec.Negate() ) ;

            //ツナギと柱の交点
            List<XYZ> pnts = GantryUtil.FindIntersectionPnts( doc, pil.m_ElementId, jId ) ;
            if ( pnts.Count < 1 ) {
              continue ;
            }

            XYZ nearP = ( pnts[ 0 ].DistanceTo( jC.GetEndPoint( 0 ) ) < pnts[ 0 ].DistanceTo( jC.GetEndPoint( 1 ) ) )
              ? jC.GetEndPoint( 0 )
              : jC.GetEndPoint( 1 ) ;
            //交点群が近いほうの点からのベクトル
            if ( nearP.DistanceTo( jC.GetEndPoint( 1 ) ) < 0.1 ) {
              jointVec = jointVec.Negate() ;
              jC = Line.CreateBound( jC.GetEndPoint( 1 ), jC.GetEndPoint( 0 ) ) ;
            }

            //ツナギ材のマテリアルクラス
            HorizontalJoint mainJ = HorizontalJoint.ReadFromElement( jId, doc ) as HorizontalJoint ;
            if ( mainJ.MaterialSize().Shape != MaterialShape.C ) {
              continue ;
            }

            List<ElementId> supprtIds = RevitUtil.ClsRevitUtil.GetIntersectFamilys( doc, jId, serchIds: supportId ) ;
            if ( supportId.Count <= 0 ) {
              supprtIds = new List<ElementId>() ;
            }

            //接する別のツナギも考慮
            bool isHalfAttach = false ;

            //接するツナギ材を確認
            foreach ( ElementId id in jjIds ) {
              if ( id == jId ) {
                continue ;
              }

              FamilyInstance jjins = doc.GetElement( id ) as FamilyInstance ;
              //接するツナギ材のどちらが高い位置にあるか返す(同じ高さの場合はnull)
              FamilyInstance intIns = GetHigherInstance( doc, jins, jjins ) ;
              Curve jjC = GantryUtil.GetCurve( doc, id ) ;
              XYZ jjointVec = ( jjC.GetEndPoint( 1 ) - jjC.GetEndPoint( 0 ) ).Normalize() ;
              //ツナギとツナギの交点を算出
              List<XYZ> pntsJ = GantryUtil.FindIntersectionPnts( doc, jId, id ) ;
              if ( pntsJ.Count < 1 ) {
                continue ;
              }

              XYZ nearPJ =
                ( pntsJ[ 0 ].DistanceTo( jjC.GetEndPoint( 0 ) ) < pntsJ[ 0 ].DistanceTo( jjC.GetEndPoint( 1 ) ) )
                  ? jjC.GetEndPoint( 0 )
                  : jjC.GetEndPoint( 1 ) ;
              HorizontalJoint jH = HorizontalJoint.ReadFromElement( id, doc ) as HorizontalJoint ;
              pillerP = new XYZ( pillerP.X, pillerP.Y, nearPJ.Z ) ;
              if ( jjC.GetEndPoint( 1 ).DistanceTo( nearPJ ) < 0.1 ) {
                jjC = Line.CreateBound( jjC.GetEndPoint( 1 ), jjC.GetEndPoint( 0 ) ) ;
                jjointVec = ( jjC.GetEndPoint( 1 ) - jjC.GetEndPoint( 0 ) ).Normalize() ;
              }

              //自信がフランジ側
              if ( isFlange && intIns == null ) {
                //隣り合う水平材がある場合
                if ( nearP.DistanceTo( jjC.GetEndPoint( 0 ) ) < 0.1 ||
                     nearP.DistanceTo( jjC.GetEndPoint( 0 ) ) < 0.1 ) {
                  //NT＋Cが必要
                  isHalfAttach = true ;
                }
              }
            }

            if ( isFlange && eSide == eAttachSide.Flange ) {
              //半掛けは0.5で数える
              retInt += isHalfAttach ? 0.5 : 1 ;
            }
            else if ( ! isFlange && eSide == eAttachSide.Web ) {
              if ( supprtIds.Count > 0 ) {
                var pInters = RevitUtil.ClsRevitUtil.GetIntersectFamilys( doc, pil.m_ElementId, serchIds: supportId ) ;
                var jInters = RevitUtil.ClsRevitUtil.GetIntersectFamilys( doc, jId, serchIds: supportId ) ;
                var supId = pInters.Where( x => jInters.Contains( x ) ) ;
                if ( supId.Any() ) {
                  webList.AddRange( supId ) ;
                }
              }
            }
          }
        }

        if ( eSide == eAttachSide.Web ) {
          webList.Distinct() ;
          retInt = webList.Count() ;
        }
      }
      catch ( Exception ex ) {
        retInt = 0 ;
      }

      return (int) retInt ;
    }

    /// <summary>
    /// 二つのインスタンスを比べて、より高い位置にあるインスタンスを返す
    /// </summary>
    /// <param name="instance1"></param>
    /// <param name="instance2"></param>
    /// <returns></returns>
    private static FamilyInstance GetHigherInstance( Document doc, FamilyInstance instance1, FamilyInstance instance2 )
    {
      // instance1の高さを取得
      Curve locationCurve1 = GantryUtil.GetCurve( doc, instance1.Id ) ;
      double height1 = locationCurve1.GetEndPoint( 0 ).Z ; // 開始点のZ座標を取得（高さ）

      // instance2の高さを取得
      Curve locationCurve2 = GantryUtil.GetCurve( doc, instance2.Id ) ;
      double height2 = locationCurve2.GetEndPoint( 0 ).Z ; // 開始点のZ座標を取得（高さ）

      // より高い位置にあるファミリインスタンスを返す
      if ( RevitUtil.ClsGeo.GEO_GT( height1, height2 ) ) {
        return instance1 ;
      }
      else if ( RevitUtil.ClsGeo.GEO_GT( height2, height1 ) ) {
        return instance2 ;
      }
      else {
        return null ;
      }
    }

    /// <summary>
    /// 取付補助材を収集する
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="eType"></param>
    /// <returns></returns>
    private static List<FamilyInstance> GetAttachSupportList( Document doc, eBoltElement eType )
    {
      List<FamilyInstance> retList = new List<FamilyInstance>() ;
      IEnumerable<Element> elems =
        from elem in ( new FilteredElementCollector( doc ).OfClass( typeof( FamilyInstance ) ).ToElements() )
        let sym = elem as FamilyInstance
        where sym.Symbol.FamilyName.Contains( "取付用補助材" )
        select sym ;

      foreach ( Element el in elems ) {
        string type = ( el as FamilyInstance ).Symbol.Name ;
        if ( eType == eBoltElement.VBrace && type.Equals( "" ) ||
             eType == eBoltElement.VBrace && type.Equals( "ブレス" ) ) {
          retList.Add( el as FamilyInstance ) ;
        }
        else if ( eType == eBoltElement.HJoint && type.Equals( "ツナギ" ) ) {
          retList.Add( el as FamilyInstance ) ;
        }
      }

      return retList ;
    }

    /// <summary>
    /// 柱に付くツナギ用取付補助材（ウェブ面側のみ）の取付箇所数をカウントする
    /// </summary>
    /// <param name="doc"></param>
    /// <returns></returns>
    private static int GetWebSupprtItemCnt( Document doc )
    {
      int retInt = 0 ;
      try {
        //図面上の部材をそれぞれ集める
        List<FamilyInstance> teiketsuList = GetAttachSupportList( doc, eBoltElement.HJoint ) ;
        List<ElementId> supportId =
          teiketsuList.Count > 0 ? teiketsuList.Select( x => x.Id ).ToList() : new List<ElementId>() ;
        var ms = MaterialSuper.Collect( doc ) ;
        if ( ms == null || ms.Count() < 1 ) {
          return 0 ;
        }

        List<MaterialSuper> materilas = ms.ToList() ;

        //柱、杭
        List<PilePillerSuper> pillers = materilas.Select( x => x as PilePillerSuper )
          .Where( x => x != null && x.m_ElementId != ElementId.InvalidElementId ).ToList() ;
        List<ElementId> webList = new List<ElementId>() ;
        //水平ツナギ
        List<ElementId> joints = materilas.Select( x => x as HorizontalJoint )
          .Where( x => x != null && x.m_ElementId != ElementId.InvalidElementId ).Select( x => x.m_ElementId )
          .ToList() ;
        if ( joints.Count < 1 ) {
          return 0 ;
        }

        //柱それぞれに取り付く補助材を見る
        foreach ( PilePillerSuper p in pillers ) {
          FamilyInstance pillerIns = doc.GetElement( p.m_ElementId ) as FamilyInstance ;
          //柱に接する取付補助材
          List<ElementId> intsId =
            RevitUtil.ClsRevitUtil.GetIntersectFamilys( doc, p.m_ElementId, serchIds: supportId ) ;
          //取付補助材がツナギ用の物かどうか判定する
          foreach ( ElementId sId in intsId ) {
            //接するツナギがあるかどうか判定
            List<ElementId> jintsId =
              RevitUtil.ClsRevitUtil.GetIntersectFamilys( doc, sId, ignorelist: teikesuList(), serchIds: joints ) ;
            if ( jintsId.Count <= 0 ) {
              continue ;
            }

            //食い込む形での接触なら別の用途の取付補助材とみなし、対象から除外
            CheckCollision( doc, sId, ref jintsId ) ;

            bool isForWebJoint = false ;
            foreach ( ElementId id in jintsId ) {
              FamilyInstance jjins = doc.GetElement( id ) as FamilyInstance ;
              Curve jjC = GantryUtil.GetCurve( doc, id ) ;
              XYZ jointVec = ( jjC.GetEndPoint( 1 ) - jjC.GetEndPoint( 0 ) ).Normalize() ;
              bool isFlange = GantryUtil.AreComponentsSameSign( pillerIns.HandOrientation, jointVec ) ||
                              GantryUtil.AreComponentsSameSign( pillerIns.HandOrientation, jointVec.Negate() ) ;
              //フランジ側のツナギは無視
              if ( isFlange ) {
                continue ;
              }

              //ウェブ面のツナギと接触する物はカウント
              isForWebJoint = true ;
              break ;
            }

            //ツナギ用（ウェブ面)の物ならカウント
            retInt += isForWebJoint ? 1 : 0 ;
          }
        }
      }
      catch ( Exception ex ) {
        retInt = 0 ;
      }

      return retInt ;
    }

    /// <summary>
    /// 切梁受についている取付補助材締結用ボルト箇所をカウントする
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="eSide"></param>
    /// <returns></returns>
    private static int GetKiribariUkeAttachCnt( Document doc, eAttachSide eSide )
    {
      double retInt = 0 ;

      List<FamilyInstance> teiketsuList = GetAttachSupportList( doc, eBoltElement.HJoint ) ;
      List<ElementId> supportId =
        teiketsuList.Count > 0 ? teiketsuList.Select( x => x.Id ).ToList() : new List<ElementId>() ;
      var ms = MaterialSuper.Collect( doc ) ;
      if ( ms == null || ms.Count() < 1 ) {
        return 0 ;
      }

      List<MaterialSuper> materilas = ms.ToList() ;

      //切梁受材
      List<ElementId> kiribari = new List<ElementId>() ;
      IEnumerable<Element> elems = null ;
      elems = from elem in ( new FilteredElementCollector( doc ).OfClass( typeof( FamilyInstance ) ).ToElements() )
        let sym = elem as FamilyInstance
        where sym.Name.Contains( "切梁受け材" )
        select sym ;
      foreach ( Element elem in elems ) {
        kiribari.Add( elem.Id ) ;
      }

      if ( kiribari.Count < 1 ) {
        return 0 ;
      }

      //柱、杭
      List<PilePillerSuper> pillers = materilas.Select( x => x as PilePillerSuper )
        .Where( x => x != null && x.m_ElementId != ElementId.InvalidElementId ).ToList() ;
      List<ElementId> webList = new List<ElementId>() ;
      //柱と触れる部材を対象とし、交点を求める
      foreach ( PilePillerSuper pil in pillers ) {
        FamilyInstance pillerIns = doc.GetElement( pil.m_ElementId ) as FamilyInstance ;
        //柱に接するファミリのリスト(締結補助材は除く)
        List<ElementId> intsId = RevitUtil.ClsRevitUtil.GetIntersectFamilys( doc, pil.m_ElementId,
          ignorelist: teikesuList(), serchIds: kiribari ) ;
        //柱に接する切梁受け材だけを収集
        List<ElementId> kiribariIds = intsId.Where( x => kiribari.Contains( x ) ).ToList() ;
        //柱の基点
        XYZ pillerP = pil.Position ;

        foreach ( ElementId jId in kiribariIds ) {
          FamilyInstance jins = doc.GetElement( jId ) as FamilyInstance ;
          Curve jC = GantryUtil.GetCurve( doc, jId ) ;
          XYZ jointVec = ( jC.GetEndPoint( 1 ) - jC.GetEndPoint( 0 ) ).Normalize() ;
          //切梁受に接する別の切梁受を取得
          List<ElementId> jintsId =
            RevitUtil.ClsRevitUtil.GetIntersectFamilys( doc, jId, ignorelist: teikesuList(), serchIds: kiribariIds ) ;
          List<ElementId> jjIds = jintsId.Where( x => kiribari.Contains( x ) && intsId.Contains( x ) ).ToList() ;
          //この切梁受がフランジ方向に平行か
          bool isFlange = GantryUtil.AreComponentsSameSign( pillerIns.HandOrientation, jointVec ) ||
                          GantryUtil.AreComponentsSameSign( pillerIns.HandOrientation, jointVec.Negate() ) ;

          //切梁受と柱の交点
          List<XYZ> pnts = GantryUtil.FindIntersectionPnts( doc, pil.m_ElementId, jId ) ;
          if ( pnts.Count < 1 ) {
            continue ;
          }

          XYZ nearP = ( pnts[ 0 ].DistanceTo( jC.GetEndPoint( 0 ) ) < pnts[ 0 ].DistanceTo( jC.GetEndPoint( 1 ) ) )
            ? jC.GetEndPoint( 0 )
            : jC.GetEndPoint( 1 ) ;
          //チャンネル以外は対象外
          if ( ! jins.Symbol.FamilyName.Contains( "形鋼_C" ) ) {
            continue ;
          }

          //交点群が近いほうの点からのベクトル
          if ( nearP.DistanceTo( jC.GetEndPoint( 1 ) ) < 0.1 ) {
            jointVec = jointVec.Negate() ;
            jC = Line.CreateBound( jC.GetEndPoint( 1 ), jC.GetEndPoint( 0 ) ) ;
          }

          //切梁受けはベース線の位置が
          if ( jins.Symbol.Name.Contains( "切梁受" ) ) {
            double kiribariH = jins.LookupParameter( DefineUtil.PARAM_HOST_OFFSET ).AsDouble() ;
            nearP = nearP + XYZ.BasisZ.Normalize() * kiribariH ;
            jC = Line.CreateBound( jC.GetEndPoint( 0 ) + XYZ.BasisZ.Normalize() * kiribariH,
              jC.GetEndPoint( 1 ) + XYZ.BasisZ.Normalize() * kiribariH ) ;
            jointVec = ( jC.GetEndPoint( 1 ) - jC.GetEndPoint( 0 ) ).Normalize() ;
          }

          List<ElementId> supprtIds = RevitUtil.ClsRevitUtil.GetIntersectFamilys( doc, jId, serchIds: supportId ) ;
          if ( supportId.Count <= 0 ) {
            supprtIds = new List<ElementId>() ;
          }

          //接する別の切梁受も考慮
          bool isHalfAttach = false ;

          //接する切梁受材を確認
          foreach ( ElementId id in jjIds ) {
            if ( id == jId ) {
              continue ;
            }

            FamilyInstance jjins = doc.GetElement( id ) as FamilyInstance ;
            //接する切梁受材のどちらが高い位置にあるか返す(同じ高さの場合はnull)
            FamilyInstance intIns = GetHigherInstance( doc, jins, jjins ) ;
            Curve jjC = GantryUtil.GetCurve( doc, id ) ;
            XYZ jjointVec = ( jjC.GetEndPoint( 1 ) - jjC.GetEndPoint( 0 ) ).Normalize() ;
            //ツナギと切梁受の交点を算出
            List<XYZ> pntsJ = GantryUtil.FindIntersectionPnts( doc, jId, id ) ;
            if ( pntsJ.Count < 1 ) {
              continue ;
            }

            XYZ nearPJ =
              ( pntsJ[ 0 ].DistanceTo( jjC.GetEndPoint( 0 ) ) < pntsJ[ 0 ].DistanceTo( jjC.GetEndPoint( 1 ) ) )
                ? jjC.GetEndPoint( 0 )
                : jjC.GetEndPoint( 1 ) ;
            pillerP = new XYZ( pillerP.X, pillerP.Y, nearPJ.Z ) ;
            if ( jjC.GetEndPoint( 1 ).DistanceTo( nearPJ ) < 0.1 ) {
              jjC = Line.CreateBound( jjC.GetEndPoint( 1 ), jjC.GetEndPoint( 0 ) ) ;
              jjointVec = ( jjC.GetEndPoint( 1 ) - jjC.GetEndPoint( 0 ) ).Normalize() ;
            }

            //切梁受けはベース線の位置が
            if ( jjins.Symbol.Name.Contains( "切梁受" ) ) {
              double kiribariH = jjins.LookupParameter( DefineUtil.PARAM_HOST_OFFSET ).AsDouble() ;
              nearPJ = nearPJ + XYZ.BasisZ.Normalize() * kiribariH ;
              jjC = Line.CreateBound( jjC.GetEndPoint( 0 ) + XYZ.BasisZ.Normalize() * kiribariH,
                jjC.GetEndPoint( 1 ) + XYZ.BasisZ.Normalize() * kiribariH ) ;
              jointVec = ( jjC.GetEndPoint( 1 ) - jjC.GetEndPoint( 0 ) ).Normalize() ;
            }

            //自信がフランジ側
            if ( isFlange && intIns == null ) {
              //隣り合う切梁受がある場合
              if ( nearP.DistanceTo( jjC.GetEndPoint( 0 ) ) < 0.1 || nearP.DistanceTo( jjC.GetEndPoint( 0 ) ) < 0.1 ) {
                isHalfAttach = true ;
              }
            }
          }

          if ( isFlange && eSide == eAttachSide.Flange ) {
            //半掛けは0.5で数える
            retInt += isHalfAttach ? 0.5 : 1 ;
          }
          else if ( ! isFlange && eSide == eAttachSide.Web ) {
            if ( supprtIds.Count > 0 ) {
              var pInters = RevitUtil.ClsRevitUtil.GetIntersectFamilys( doc, pil.m_ElementId, serchIds: supportId ) ;
              var jInters = RevitUtil.ClsRevitUtil.GetIntersectFamilys( doc, jId, serchIds: supportId ) ;
              var supId = pInters.Where( x => jInters.Contains( x ) ) ;
              if ( supId.Any() ) {
                webList.AddRange( supId ) ;
              }
            }
          }
        }
      }

      if ( eSide == eAttachSide.Web ) {
        webList.Distinct() ;
        retInt = webList.Count() ;
      }

      return (int) retInt ;
    }

    /// <summary>
    /// 杭との取付関係
    /// </summary>
    public enum eAttachSide
    {
      /// <summary>
      /// ｳｪﾌﾞ面
      /// </summary>
      Web,

      /// <summary>
      /// フランジ面
      /// </summary>
      Flange
    }

    public enum eBoltElement
    {
      /// <summary>
      /// 水平ブレース
      /// </summary>
      HBrace,

      /// <summary>
      /// 垂直ブレース
      /// </summary>
      VBrace,

      /// <summary>
      /// 水平ツナギ
      /// </summary>
      HJoint,

      /// <summary>
      /// 取付補助材
      /// </summary>
      Support,

      /// <summary>
      /// 切梁受け
      /// </summary>
      Kiribari
    }

    /// <summary>
    /// 部材種類
    /// </summary>
    public enum eElementType
    {
      /// <summary>
      /// 水平ブレース
      /// </summary>
      HBrace,

      /// <summary>
      /// 垂直ブレース
      /// </summary>
      VBrace,

      /// <summary>
      /// 水平ツナギ
      /// </summary>
      HJoint
    }

    /// <summary>
    /// 引数で指定した部材種類に該当するファミリインスタンスを取得する
    /// </summary>
    /// <param name="doc">対象のRevit.DB.Document</param>
    /// <param name="eType">部材種類</param>
    /// <param name="koudaiName">構台名</param>
    /// <remarks>構台名を指定しない場合は図面に配置された全てのファミリインスタンスが収集対象となる</remarks>
    /// <returns>引数で指定した部材種類に該当するファミリインスタンスのリスト</returns>
    public static List<FamilyInstance> GetFamilyInstanceListBy( Document doc, eElementType eType,
      string koudaiName = "" )
    {
      var retList = new List<FamilyInstance>() ;

      try {
        //指定した構台名の部材を収集(構台名未指定の場合は図面上すべての部材を収集)
        var ms = string.IsNullOrEmpty( koudaiName )
          ? MaterialSuper.Collect( doc )
          : MaterialSuper.Collect( doc ).Where( x => x.m_KodaiName == koudaiName ) ;
        if ( ms == null || ms.Count() < 1 ) return retList ;

        List<MaterialSuper> targetMsList = ms.ToList() ;
        List<MaterialSuper> retMsList = new List<MaterialSuper>() ;

        switch ( eType ) {
          // 水平ブレース
          case eElementType.HBrace :
            List<HorizontalBrace> lstHBrace = targetMsList.Where( x => x.GetType() == typeof( HorizontalBrace ) )
              .Select( x => x as HorizontalBrace )?.ToList() ?? null ;
            if ( lstHBrace == null ) break ;
            retMsList.AddRange( lstHBrace ) ;
            break ;

          // 垂直ブレース
          case eElementType.VBrace :
            List<VerticalBrace> lstVBrace = targetMsList.Where( x => x.GetType() == typeof( VerticalBrace ) )
              .Select( x => x as VerticalBrace )?.ToList() ?? null ;
            if ( lstVBrace == null ) break ;
            retMsList.AddRange( lstVBrace ) ;
            break ;

          // 水平ツナギ
          case eElementType.HJoint :
            List<HorizontalJoint> lstHJoint = targetMsList.Where( x => x.GetType() == typeof( HorizontalJoint ) )
              .Select( x => x as HorizontalJoint )?.ToList() ?? null ;
            if ( lstHJoint == null ) break ;
            retMsList.AddRange( lstHJoint ) ;
            break ;
        }

        foreach ( var retMs in retMsList ) {
          FamilyInstance fi = doc.GetElement( retMs.m_ElementId ) as FamilyInstance ;
          retList.Add( fi ) ;
        }
      }
      catch ( Exception ex ) {
        // 必要に応じてログ出力なりなんなり
      }

      return retList ;
    }

    /// <summary>
    /// 杭のトップPLの厚みを返す
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="id"></param>
    /// <returns>トップPLがある場合はその厚みを、ない場合またはエラーの場合は0</returns>
    public static int GetPileTopPLThick( Document doc, ElementId id )
    {
      int retVal = 0 ;
      try {
        FamilyInstance ins = doc.GetElement( id ) as FamilyInstance ;
        if ( ins == null ) {
          return 0 ;
        }

        FamilySymbol sym = ins.Symbol ;

        Parameter paramUmu = sym.LookupParameter( DefineUtil.PARAM_TOP_PLATE ) ;
        if ( paramUmu != null && paramUmu.AsInteger() == (int) DefineUtil.PramYesNo.Yes ) {
          Parameter paramType = sym.LookupParameter( DefineUtil.PARAM_TOP_PLATE_SIZE ) ;
          var p = paramType.AsValueString() ;
          if ( p.Contains( "定形" ) ) {
            string size = p.Replace( "ﾄｯﾌﾟﾌﾟﾚｰﾄ_定形 : ", "" ).Replace( "PL-", "" ) ;
            size = size.Substring( 0, 2 ) ;
            retVal = RevitUtil.ClsCommonUtils.ChangeStrToInt( size ) ;
          }
          else {
            Parameter pthick = sym.LookupParameter( DefineUtil.PARAM_TOP_PLATE_T ) ;
            retVal = RevitUtil.ClsCommonUtils.ChangeStrToInt( pthick.AsValueString() ) ;
          }
        }
      }
      catch ( Exception ) {
        retVal = 0 ;
      }

      return retVal ;
    }

    /// <summary>
    /// 収集する柱(杭）の種類を指定して集める
    /// </summary>
    /// <param name="pType"></param>
    /// <returns></returns>
    public static List<ElementId> CollectPilePiller( Document doc, PType pType )
    {
      List<ElementId> retlist = new List<ElementId>() ;
      string typeName = "" ;
      switch ( pType ) {
        case PType.Pile :
          typeName = "支持杭" ;
          break ;
        case PType.ExPile :
          typeName = "継ぎ足し杭" ;
          break ;
        case PType.Piller :
          typeName = "支柱" ;
          break ;
        default :
          return new List<ElementId>() ;
      }

      try {
        IEnumerable<Element> elems = null ;
        elems = from elem in ( new FilteredElementCollector( doc ).OfClass( typeof( FamilyInstance ) ).ToElements() )
          let sym = elem as FamilyInstance
          where sym.Name.Contains( typeName )
          select sym ;
        foreach ( Element elem in elems ) {
          retlist.Add( elem.Id ) ;
        }
      }
      catch ( Exception ) {
        retlist = new List<ElementId>() ;
      }

      return retlist ;
    }

    /// <summary>
    /// ファミリインスタンスと衝突するファミリのElementIDを取得
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="targetInstanceId">ファミリインスタンスID</param>
    /// <param name="serchId">衝突判定する対象</param>
    /// <param name="ids">衝突と見做されたelementID</param>
    /// <remarks>targetInstanceIdをSolidまで分解しフィルターで検索　接触は衝突と判定されないようだ</remarks>
    /// <returns></returns>
    private static bool CheckCollision( Document doc, ElementId targetInstanceId, ref List<ElementId> serchId )
    {
      bool hit = false ;

      FilteredElementCollector interferingCollector = new FilteredElementCollector( doc, serchId ) ;
      interferingCollector.WhereElementIsNotElementType() ;
      List<Solid> ls = RevitUtil.ClsRevitUtil.GetTargetSolids( doc, targetInstanceId ) ;
      List<ElementId> hitids = new List<ElementId>() ;

      foreach ( Solid sol in ls ) {
        ElementIntersectsSolidFilter intersectionFilter = new ElementIntersectsSolidFilter( sol ) ;
        interferingCollector.WherePasses( intersectionFilter ) ;
        ICollection<ElementId> ic = interferingCollector.ToElementIds() ;

        if ( ic.Count > 0 ) {
          hitids.AddRange( ic ) ;
          hit = true ;
        }
      }

      foreach ( ElementId id in hitids ) {
        if ( ! id.Equals( targetInstanceId ) && serchId.Contains( id ) ) {
          serchId.Remove( id ) ;
        }
      }

      return hit ;
    }
  }

  /// <summary>
  /// 杭(柱)タイプ
  /// </summary>
  public enum PType
  {
    Pile = 0,
    ExPile = 1,
    Piller = 2
  }
}