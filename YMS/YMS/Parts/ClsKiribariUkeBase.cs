using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;

namespace YMS.Parts
{
  public class ClsKiribariUkeBase
  {
    #region 定数

    public const string baseName = "切梁受け材ベース" ;

    #endregion

    #region Enum

    /// <summary>
    /// 処理方法
    /// </summary>
    public enum ShoriType
    {
      Replace,
      PileSelect,
      PtoP
    }

    #endregion

    #region プロパティ

    /// <summary>
    /// 処理タイプ
    /// </summary>
    public ShoriType m_ShoriType { get ; set ; }

    /// <summary>
    /// 作成方法：鋼材タイプ
    /// </summary>
    public string m_SteelType { get ; set ; }

    /// <summary>
    /// 作成方法：鋼材サイズ
    /// </summary>
    public string m_SteelSize { get ; set ; }

    /// <summary>
    /// 突き出し量-始点
    /// </summary>
    public int m_TsukidashiRyoS { get ; set ; }

    /// <summary>
    /// 突き出し量-終点
    /// </summary>
    public int m_TsukidashiRyoE { get ; set ; }

    /// <summary>
    /// 編集用：フロア
    /// </summary>
    public string m_Floor { get ; set ; }

    /// <summary>
    /// 編集用：エレメントID
    /// </summary>
    public ElementId m_ElementId { get ; set ; }

    /// <summary>
    /// 編集用：段
    /// </summary>
    public string m_Dan { get ; set ; }

    /// <summary>
    /// 編集用：編集フラグ
    /// </summary>
    public bool m_FlgEdit { get ; set ; }

    #endregion

    #region コンストラクタ

    public ClsKiribariUkeBase()
    {
      //初期化
      Init() ;
    }

    #endregion

    #region メソッド

    /// <summary>
    /// 初期化
    /// </summary>
    public void Init()
    {
      m_ShoriType = ShoriType.Replace ;
      m_SteelType = string.Empty ;
      m_SteelSize = string.Empty ;
      m_TsukidashiRyoS = 0 ;
      m_TsukidashiRyoE = 0 ;
      m_Floor = string.Empty ;
      m_Dan = string.Empty ;
      m_FlgEdit = false ;
    }

    public bool CreateKiribariUkeBase( Document doc, List<ElementId> modelLineIdList, ElementId levelID )
    {
      string symbolFolpath = ClsZumenInfo.GetYMSFolder() ;
      string shinfamily = System.IO.Path.Combine( symbolFolpath, "ベース関係\\" + baseName + ".rfa" ) ;
      //シンボル読込
      if ( ! ClsRevitUtil.LoadFamilySymbolData( doc, shinfamily, baseName, out FamilySymbol sym ) ) {
        return false ;
      }

      foreach ( ElementId modelLineId in modelLineIdList ) {
        ModelLine modelLine = doc.GetElement( modelLineId ) as ModelLine ;
        LocationCurve lCurve = modelLine.Location as LocationCurve ;
        if ( lCurve == null ) {
          continue ;
        }

        Curve cv = lCurve.Curve ;
        XYZ tmpStPoint = cv.GetEndPoint( 0 ) ;
        XYZ tmpEdPoint = cv.GetEndPoint( 1 ) ;
        double tsukidashiRyoS = ClsRevitUtil.CovertToAPI( m_TsukidashiRyoS ) ;
        double tsukidashiRyoE = ClsRevitUtil.CovertToAPI( m_TsukidashiRyoE ) ;
        XYZ Direction = Line.CreateBound( tmpStPoint, tmpEdPoint ).Direction ;
        tmpStPoint = new XYZ( tmpStPoint.X - ( tsukidashiRyoS * Direction.X ),
          tmpStPoint.Y - ( tsukidashiRyoS * Direction.Y ), tmpStPoint.Z ) ;
        tmpEdPoint = new XYZ( tmpEdPoint.X + ( tsukidashiRyoE * Direction.X ),
          tmpEdPoint.Y + ( tsukidashiRyoE * Direction.Y ), tmpEdPoint.Z ) ;
        cv = Line.CreateBound( tmpStPoint, tmpEdPoint ) ;

        using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
          t.Start() ;
          ElementId CreatedID = ClsRevitUtil.Create( doc, cv, levelID, sym ) ;
          ClsRevitUtil.SetMojiParameter( doc, CreatedID, "段", "下段" ) ;
          ClsRevitUtil.SetMojiParameter( doc, CreatedID, "鋼材サイズ", m_SteelSize ) ;
          ClsRevitUtil.SetMojiParameter( doc, CreatedID, "鋼材タイプ", m_SteelType ) ;
          ClsRevitUtil.SetParameter( doc, CreatedID, "突き出し量始点", tsukidashiRyoS ) ;
          ClsRevitUtil.SetParameter( doc, CreatedID, "突き出し量終点", tsukidashiRyoE ) ;
          t.Commit() ;
        }
      }

      return true ;
    }

    public bool ChangeKiribariUkeBase( Document doc, XYZ tmpStPoint, XYZ tmpEdPoint, string dan, ElementId levelID )
    {
      string symbolFolpath = ClsZumenInfo.GetYMSFolder() ;
      string shinfamily = System.IO.Path.Combine( symbolFolpath, "ベース関係\\" + baseName + ".rfa" ) ;
      //シンボル読込
      if ( ! ClsRevitUtil.LoadFamilySymbolData( doc, shinfamily, baseName, out FamilySymbol sym ) ) {
        return false ;
      }

      XYZ Direction = Line.CreateBound( tmpStPoint, tmpEdPoint ).Direction ;
      double tsukidashiRyoS = (int) ClsRevitUtil.CovertToAPI( m_TsukidashiRyoS ) ;
      double tsukidashiRyoE = (int) ClsRevitUtil.CovertToAPI( m_TsukidashiRyoE ) ;
      tmpStPoint = new XYZ( tmpStPoint.X - ( tsukidashiRyoS * Direction.X ),
        tmpStPoint.Y - ( tsukidashiRyoS * Direction.Y ), tmpStPoint.Z ) ;
      tmpEdPoint = new XYZ( tmpEdPoint.X + ( tsukidashiRyoE * Direction.X ),
        tmpEdPoint.Y + ( tsukidashiRyoE * Direction.Y ), tmpEdPoint.Z ) ;
      Curve cv = Line.CreateBound( tmpStPoint, tmpEdPoint ) ;

      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        t.Start() ;
        ElementId CreatedID = ClsRevitUtil.Create( doc, cv, levelID, sym ) ;
        ClsRevitUtil.SetMojiParameter( doc, CreatedID, "段", dan ) ;
        ClsRevitUtil.SetMojiParameter( doc, CreatedID, "鋼材サイズ", m_SteelSize ) ;
        ClsRevitUtil.SetMojiParameter( doc, CreatedID, "鋼材タイプ", m_SteelType ) ;
        ClsRevitUtil.SetParameter( doc, CreatedID, "突き出し量始点", tsukidashiRyoS ) ;
        ClsRevitUtil.SetParameter( doc, CreatedID, "突き出し量終点", tsukidashiRyoE ) ;
        t.Commit() ;
      }

      return true ;
    }

    public bool ChangeKiribariUkeBase( Document doc, XYZ tmpStPoint, XYZ tmpEdPoint )
    {
      ElementId levelID = ClsRevitUtil.GetLevelID( doc, m_Floor ) ;
      if ( levelID == null ) {
        return false ;
      }

      string symbolFolpath = ClsZumenInfo.GetYMSFolder() ;
      string shinfamily = System.IO.Path.Combine( symbolFolpath, "ベース関係\\" + baseName + ".rfa" ) ;
      //シンボル読込
      if ( ! ClsRevitUtil.LoadFamilySymbolData( doc, shinfamily, baseName, out FamilySymbol sym ) ) {
        return false ;
      }

      XYZ Direction = Line.CreateBound( tmpStPoint, tmpEdPoint ).Direction ;
      double tsukidashiRyoS = (int) ClsRevitUtil.CovertToAPI( m_TsukidashiRyoS ) ;
      double tsukidashiRyoE = (int) ClsRevitUtil.CovertToAPI( m_TsukidashiRyoE ) ;
      tmpStPoint = new XYZ( tmpStPoint.X - ( tsukidashiRyoS * Direction.X ),
        tmpStPoint.Y - ( tsukidashiRyoS * Direction.Y ), tmpStPoint.Z ) ;
      tmpEdPoint = new XYZ( tmpEdPoint.X + ( tsukidashiRyoE * Direction.X ),
        tmpEdPoint.Y + ( tsukidashiRyoE * Direction.Y ), tmpEdPoint.Z ) ;
      Curve cv = Line.CreateBound( tmpStPoint, tmpEdPoint ) ;

      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        t.Start() ;
        FailureHandlingOptions failOpt = t.GetFailureHandlingOptions() ;
        failOpt.SetFailuresPreprocessor( new WarningSwallower() ) ;
        t.SetFailureHandlingOptions( failOpt ) ;
        ElementId CreatedID = ClsRevitUtil.Create( doc, cv, levelID, sym ) ;
        ClsRevitUtil.SetMojiParameter( doc, CreatedID, "段", m_Dan ) ;
        ClsRevitUtil.SetMojiParameter( doc, CreatedID, "鋼材サイズ", m_SteelSize ) ;
        ClsRevitUtil.SetMojiParameter( doc, CreatedID, "鋼材タイプ", m_SteelType ) ;
        ClsRevitUtil.SetParameter( doc, CreatedID, "突き出し量始点", tsukidashiRyoS ) ;
        ClsRevitUtil.SetParameter( doc, CreatedID, "突き出し量終点", tsukidashiRyoE ) ;
        t.Commit() ;
      }

      return true ;
    }

    /// <summary>
    /// 切梁受けベース のみを単独選択
    /// </summary>
    /// <param name="uidoc"></param>
    /// <param name="id">選択した 切梁受けベース のID</param>
    /// <param name="message">指示</param>
    /// <returns></returns>
    public static bool PickBaseObject( UIDocument uidoc, ref ElementId id, string message = baseName )
    {
      return ClsRevitUtil.PickObject( uidoc, message, baseName, ref id ) ;
    }

    public static bool PickBaseObjects( UIDocument uidoc, ref List<ElementId> ids, string message = baseName )
    {
      return ClsRevitUtil.PickObjectsPartFilter( uidoc, message + "を選択してください", baseName, ref ids ) ;
    }

    /// <summary>
    /// 図面上の切梁受けベースを全て取得する
    /// </summary>
    /// <param name="doc"></param>
    /// <returns></returns>
    public static List<ElementId> GetAllKiribariUkeBaseList( Document doc )
    {
      //図面上の切梁受けベースを全て取得
      List<ElementId> htIdList = ClsRevitUtil.GetSelectCreatedFamilyInstanceList( doc, baseName ) ;
      return htIdList ;
    }

    /// <summary>
    ///  図面上の切梁受けベースを全てクラスで取得する
    /// </summary>
    /// <param name="doc"></param>
    /// <returns></returns>
    public static List<ClsKiribariUkeBase> GetAllClsKiribariUkeBaseList( Document doc )
    {
      List<ClsKiribariUkeBase> lstBase = new List<ClsKiribariUkeBase>() ;

      List<ElementId> lstId = GetAllKiribariUkeBaseList( doc ) ;
      foreach ( ElementId id in lstId ) {
        ClsKiribariUkeBase clsKU = new ClsKiribariUkeBase() ;
        clsKU.SetParameter( doc, id ) ;
        lstBase.Add( clsKU ) ;
      }

      return lstBase ;
    }

    /// <summary>
    /// 指定したIDから切梁受けベースクラスを取得する
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public void SetParameter( Document doc, ElementId id )
    {
      FamilyInstance shinInstLevel = doc.GetElement( id ) as FamilyInstance ;

      m_Floor = shinInstLevel.Host.Name ;
      m_ElementId = id ;
      string dan = ClsRevitUtil.GetParameter( doc, id, "段" ) ;
      m_SteelSize = ClsRevitUtil.GetParameter( doc, id, "鋼材サイズ" ) ;
      m_SteelType = ClsRevitUtil.GetParameter( doc, id, "鋼材タイプ" ) ;
      m_TsukidashiRyoS = (int) ClsRevitUtil.CovertFromAPI( ClsRevitUtil.GetParameterDouble( doc, id, "突き出し量始点" ) ) ;
      m_TsukidashiRyoE = (int) ClsRevitUtil.CovertFromAPI( ClsRevitUtil.GetParameterDouble( doc, id, "突き出し量終点" ) ) ;

      return ;
    }

    #endregion
  }
}