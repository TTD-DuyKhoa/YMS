using Autodesk.Revit.DB ;
using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;

namespace YMS.Parts
{
  public class ClsStiffener
  {
    #region 定数

    public const string PLATE = "プレート" ;
    public const string JACK = "ジャッキ" ;

    #endregion

    #region プロパティ

    /// <summary>
    /// レベル
    /// </summary>
    public string m_Level { get ; set ; }

    /// <summary>
    /// 切梁＿タイプ
    /// </summary>
    public string m_Kiribari_StiffenerType { get ; set ; }

    /// <summary>
    /// 切梁＿ジャッキタイプ
    /// </summary>
    public string m_Kiribari_StiffenerJackType { get ; set ; }

    /// <summary>
    /// 切梁＿プレートタイプ
    /// </summary>
    public string m_Kiribari_StiffenerPlateType { get ; set ; }

    /// <summary>
    /// 切梁＿個数
    /// </summary>
    public int m_Kiribari_Count { get ; set ; }

    /// <summary>
    /// 火打ブロック＿タイプ
    /// </summary>
    public string m_HiuchiBlock_StiffenerType { get ; set ; }

    /// <summary>
    /// 火打ブロック＿ジャッキタイプ
    /// </summary>
    public string m_HiuchiBlock_StiffenerJackType { get ; set ; }

    /// <summary>
    /// 火打ブロック＿プレートタイプ
    /// </summary>
    public string m_HiuchiBlock_StiffenerPlateType { get ; set ; }

    /// <summary>
    /// 火打ブロック＿切梁側＿個数
    /// </summary>
    public int m_HiuchiBlock_Kiribari_Count { get ; set ; }

    /// <summary>
    /// 火打ブロック＿火打側＿個数
    /// </summary>
    public int m_HiuchiBlock_Hiuchi_Count { get ; set ; }

    /// <summary>
    /// 切梁火打＿タイプ
    /// </summary>
    public string m_KiribariHiuchi_StiffenerType { get ; set ; }

    /// <summary>
    /// 切梁火打＿ジャッキタイプ
    /// </summary>
    public string m_KiribariHiuchi_StiffenerJackType { get ; set ; }

    /// <summary>
    /// 切梁火打＿プレートタイプ
    /// </summary>
    public string m_KiribariHiuchi_StiffenerPlateType { get ; set ; }

    /// <summary>
    /// 切梁火打＿個数
    /// </summary>
    public int m_KiribariHiuchi_Count { get ; set ; }

    /// <summary>
    /// 隅火打＿タイプ
    /// </summary>
    public string m_CornerHiuchi_StiffenerType { get ; set ; }

    /// <summary>
    /// 隅火打＿ジャッキタイプ
    /// </summary>
    public string m_CornerHiuchi_StiffenerJackType { get ; set ; }

    /// <summary>
    /// 隅火打＿プレートタイプ
    /// </summary>
    public string m_CornerHiuchi_StiffenerPlateType { get ; set ; }

    /// <summary>
    /// 隅火打＿個数
    /// </summary>
    public int m_CornerHiuchi_Count { get ; set ; }

    /// <summary>
    /// 作成ID
    /// </summary>
    public ElementId m_ElementId { get ; set ; }

    /// <summary>
    /// 作成用スチフナータイプ
    /// </summary>
    public string m_StiffenerType { get ; set ; }

    /// <summary>
    /// 作成用ジャッキタイプ
    /// </summary>
    public string m_StiffenerJackType { get ; set ; }

    /// <summary>
    /// 作成用プレートタイプ
    /// </summary>
    public string m_StiffenerPlateType { get ; set ; }

    /// <summary>
    /// 作成用個数
    /// </summary>
    public int m_Count { get ; set ; }

    #endregion

    #region コンストラクタ

    public ClsStiffener()
    {
      Init() ;
    }

    #endregion

    #region メソッド

    public void Init()
    {
      m_Level = string.Empty ;

      m_Kiribari_StiffenerType = string.Empty ;
      m_Kiribari_StiffenerJackType = string.Empty ;
      m_Kiribari_StiffenerPlateType = string.Empty ;
      m_Kiribari_Count = 3 ;

      m_HiuchiBlock_StiffenerType = string.Empty ;
      m_HiuchiBlock_StiffenerJackType = string.Empty ;
      m_HiuchiBlock_StiffenerPlateType = string.Empty ;
      m_HiuchiBlock_Kiribari_Count = 3 ;
      m_HiuchiBlock_Hiuchi_Count = 3 ;

      m_KiribariHiuchi_StiffenerType = string.Empty ;
      m_KiribariHiuchi_StiffenerJackType = string.Empty ;
      m_KiribariHiuchi_StiffenerPlateType = string.Empty ;
      m_KiribariHiuchi_Count = 3 ;

      m_CornerHiuchi_StiffenerType = string.Empty ;
      m_CornerHiuchi_StiffenerJackType = string.Empty ;
      m_CornerHiuchi_StiffenerPlateType = string.Empty ;
      m_CornerHiuchi_Count = 3 ;

      m_ElementId = null ;
    }

    public bool CreateStiffener( Document doc, string haraSize, string dan, XYZ point, XYZ dir, ElementId levelId,
      double tanbuHaba )
    {
      string path ;
      FamilySymbol sym, symUra = null ;
      double dHaraSize = Master.ClsYamadomeCsv.GetWidth( haraSize ) ;
      double offset ;
      if ( dan == "上段" )
        offset = ClsRevitUtil.CovertToAPI( dHaraSize / 2 ) ;
      else if ( dan == "下段" )
        offset = -ClsRevitUtil.CovertToAPI( dHaraSize / 2 ) ;
      else
        offset = 0.0 ;
      double angle = XYZ.BasisX.AngleOnPlaneTo( dir, XYZ.BasisZ ), dist = ClsRevitUtil.CovertToAPI( dHaraSize / 2 ) ;
      double web = 0.0 ;

      if ( m_StiffenerType == PLATE ) {
        path = Master.ClsStiffenerCSV.GetFamilyPath( haraSize, m_StiffenerPlateType ) ;
        string familyName = RevitUtil.ClsRevitUtil.GetFamilyName( path ) ;
        familyName = familyName.Replace( "ｽﾁﾌﾅｰ", "スチフナー" ) ;
        List<string> list = Master.ClsStiffenerCSV.GetTypeCount( haraSize, m_StiffenerType ) ;
        if ( list.Count > 1 )
          familyName = Master.ClsStiffenerCSV.GetMark( haraSize, m_StiffenerPlateType ) ;
        if ( ! ClsRevitUtil.LoadFamilySymbolData( doc, path, familyName, out sym ) ) {
          return false ;
        }

        dist = ClsRevitUtil.CovertToAPI( dHaraSize / 2 ) ;
        point = new XYZ( point.X + dist * dir.Y, point.Y - dist * dir.X, point.Z ) ;
        web = ClsRevitUtil.CovertToAPI( Master.ClsYamadomeCsv.GetWeb( haraSize ) / 2 ) ;
      }
      else {
        path = Master.ClsStiffenerCSV.GetFamilyPath( haraSize, m_StiffenerJackType ) ;
        string familyName = RevitUtil.ClsRevitUtil.GetFamilyName( path ) ;

        if ( ! ClsRevitUtil.LoadFamilyData( doc, path, out Family famJack ) ) {
          return false ;
        }

        string typeName = haraSize.Replace( "A", "" ) ;
        if ( haraSize == "50HA" && m_StiffenerJackType == "SH-" )
          typeName += "+A" ;
        if ( haraSize.Contains( "SMH" ) )
          typeName = haraSize.Replace( "SM", "" ) ;
        if ( haraSize == "80SMH" ) {
          typeName = haraSize.Replace( "SM", "" ) + "+A" ;
          dHaraSize *= 2 ;
        }

        sym = ( ClsRevitUtil.GetFamilySymbol( doc, familyName, typeName ) ) ;
        symUra = ( ClsRevitUtil.GetFamilySymbol( doc, familyName, typeName + "(裏)" ) ) ;

        dist = ClsRevitUtil.CovertToAPI( dHaraSize - Master.ClsYamadomeCsv.GetFlange( haraSize ) ) ; //かかりしろは山留のフランジ幅
        point = new XYZ( point.X + dist * dir.Y, point.Y - dist * dir.X, point.Z ) ;
        angle -= Math.PI / 2 ;
      }

      if ( sym != null ) {
        using ( Transaction transaction = new Transaction( doc, "スチフナー作成" ) ) {
          List<XYZ> pointList = new List<XYZ>() ;
          switch ( m_Count ) {
            case 1 :
            {
              pointList.Add( point ) ;
              break ;
            }
            case 2 :
            {
              dist = ClsRevitUtil.CovertToAPI( tanbuHaba / 4 ) ;
              XYZ point1 = new XYZ( point.X + dist * dir.X, point.Y + dist * dir.Y, point.Z ) ;
              pointList.Add( point1 ) ;

              XYZ point2 = new XYZ( point.X - dist * dir.X, point.Y - dist * dir.Y, point.Z ) ;
              pointList.Add( point2 ) ;
              break ;
            }
            case 3 :
            {
              pointList.Add( point ) ;

              dist = ClsRevitUtil.CovertToAPI( tanbuHaba / 3 ) ;
              XYZ point1 = new XYZ( point.X + dist * dir.X, point.Y + dist * dir.Y, point.Z ) ;
              pointList.Add( point1 ) ;

              XYZ point2 = new XYZ( point.X - dist * dir.X, point.Y - dist * dir.Y, point.Z ) ;
              pointList.Add( point2 ) ;
              break ;
            }
            default :
            {
              break ;
            }
          }

          transaction.Start() ;
          foreach ( XYZ insertPoint in pointList ) {
            ElementId CreatedID = ClsRevitUtil.Create( doc, insertPoint, levelId, sym ) ;
            Line axis = Line.CreateBound( insertPoint, insertPoint + XYZ.BasisZ ) ;

            ClsRevitUtil.RotateElement( doc, CreatedID, axis, angle ) ;
            ClsRevitUtil.SetParameter( doc, CreatedID, "基準レベルからの高さ", offset + web ) ;

            if ( m_StiffenerType == PLATE ) {
              CreatedID = ClsRevitUtil.Create( doc, insertPoint, levelId, sym ) ;
              ClsRevitUtil.RotateElement( doc, CreatedID, axis, angle ) ;
              ClsRevitUtil.SetParameter( doc, CreatedID, "基準レベルからの高さ",
                offset - ClsRevitUtil.CovertToAPI( dHaraSize / 2 ) ) ;
            }
            else {
              CreatedID = ClsRevitUtil.Create( doc, insertPoint, levelId, symUra ) ;
              ClsRevitUtil.RotateElement( doc, CreatedID, axis, angle ) ;
              ClsRevitUtil.SetParameter( doc, CreatedID, "基準レベルからの高さ", offset ) ;
            }
            //m_ElementId = CreatedID;
          }

          transaction.Commit() ;
        }
      }

      return true ;
    }

    #endregion
  }
}