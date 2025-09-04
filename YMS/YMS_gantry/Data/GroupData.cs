using Autodesk.Revit.DB ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using YMS_gantry.UI ;
using static YMS_gantry.UI.FrmCreateSlopeViewModel ;

namespace YMS_gantry.Data
{
  [Serializable]
  [System.Xml.Serialization.XmlRoot( "groupData" )]
  public class GroupData : GantryData
  {
    #region 定数

    /// <summary>
    /// スキーマ名称
    /// </summary>
    public new string SchemaName = "SchemaGroup" ;

    #endregion

    #region プロパティ

    /// <summary>
    /// グループ名
    /// </summary>
    [System.Xml.Serialization.XmlElement( "GroupName" )]
    public string GroupName ;

    /// <summary>
    /// 構台名リスト
    /// </summary>
    [System.Xml.Serialization.XmlArray( "ListKoudaiName" )]
    [System.Xml.Serialization.XmlArrayItem( "KoudaiName" )]
    public List<string> ListKoudaiName ;

    /// <summary>
    /// ハンチIDリスト
    /// </summary>
    [System.Xml.Serialization.XmlArray( "ListHunchId" )]
    [System.Xml.Serialization.XmlArrayItem( "HunchId" )]
    public List<int> ListHunchId ;

    /// <summary>
    /// グループラインの作図状態
    /// </summary>
    [System.Xml.Serialization.XmlElement( "FlgLineView" )]
    public bool FlgLineView ;

    /// <summary>
    /// 作図したLINEID
    /// </summary>
    [System.Xml.Serialization.XmlArray( "ListLineId" )]
    [System.Xml.Serialization.XmlArrayItem( "LineId" )]
    public List<int> ListLineId ;

    #endregion

    #region コンストラクタ

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public GroupData()
    {
      InitializeVariables() ;
    }

    #endregion

    #region メソッド

    /// <summary>
    /// 初期化
    /// </summary>
    private void InitializeVariables()
    {
      GroupName = string.Empty ;
      ListKoudaiName = new List<string>() ;
      ListHunchId = new List<int>() ;
      ListLineId = new List<int>() ;
      FlgLineView = false ;
    }

    /// <summary>
    /// 構台情報を取得
    /// </summary>
    /// <param name="koudaiName">構台名称</param>
    /// <returns></returns>
    public List<KoudaiData> GetKoudaiList( ModelData md )
    {
      List<KoudaiData> kd = new List<KoudaiData>() ;
      if ( ( from data in md.ListKoudaiData
            where ListKoudaiName.Contains( data.AllKoudaiFlatData.KoudaiName )
            select data ).Any() ) {
        kd = ( from data in md.ListKoudaiData
          where ListKoudaiName.Contains( data.AllKoudaiFlatData.KoudaiName )
          select data ).ToList() ;
      }

      return kd ;
    }

    /// <summary>
    /// 外形線のIDを取得
    /// </summary>
    public List<ElementId> GetListLineElementId()
    {
      List<ElementId> lst = new List<ElementId>() ;
      foreach ( int n in ListLineId ) {
        lst.Add( new ElementId( n ) ) ;
      }

      return lst ;
    }

    /// <summary>
    /// ハンチのIDを取得
    /// </summary>
    public List<ElementId> GetListHunchElementId()
    {
      List<ElementId> lst = new List<ElementId>() ;
      foreach ( int n in ListHunchId ) {
        lst.Add( new ElementId( n ) ) ;
      }

      return lst ;
    }

    /// <summary>
    /// ハンチのIDを設定
    /// </summary>
    public void SetListHunchElementId( List<ElementId> lstId )
    {
      List<int> lst = new List<int>() ;
      foreach ( ElementId id in lstId ) {
        lst.Add( id.IntegerValue ) ;
      }

      ListHunchId = lst ;
    }

    /// <summary>
    /// 外形線のIDをセット
    /// </summary>
    public void SetListLineElementId( List<ElementId> lstId )
    {
      ListLineId = new List<int>() ;
      if ( lstId.Count > 0 )
        ListLineId = ( from data in lstId select data.IntegerValue ).ToList() ;
      return ;
    }

    #endregion
  }
}