using Autodesk.Revit.DB ;
using Autodesk.Revit.DB.ExtensibleStorage ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;
using YMS_gantry.UI ;
using View = Autodesk.Revit.DB.View ;

namespace YMS_gantry.Data
{
  [Serializable]
  [System.Xml.Serialization.XmlRoot( "modelData" )]
  public class ModelData : GantryData
  {
    #region 定数

    public const string SchemaName2 = "SchemaModeldata" ;

    public const string SchemaId = "FBFBF6B2-4C06-42d4-97C1-D1B4EBA93EFE" ;

    public const string FieldName = "ModelData" ;

    public const string VendorId = "fresco.co.jp" ;

    #endregion

    #region プロパティ

    /// <summary>
    /// 構台情報リスト
    /// </summary>
    /// </summary>
    [System.Xml.Serialization.XmlArray( "ListKoudaiData" )]
    [System.Xml.Serialization.XmlArrayItem( "KoudaiData" )]
    public List<KoudaiData> ListKoudaiData ;

    /// <summary>
    /// グループ情報リスト
    /// </summary>
    /// </summary>
    [System.Xml.Serialization.XmlArray( "ListGroupData" )]
    [System.Xml.Serialization.XmlArrayItem( "GroupData" )]
    public List<GroupData> ListGroupData ;

    #endregion

    #region コンストラクタ

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ModelData()
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
      ListKoudaiData = new List<KoudaiData>() ;
      ListGroupData = new List<GroupData>() ;
    }

    /// <summary>
    /// 構台情報を取得
    /// </summary>
    /// <param name="koudaiName">構台名称</param>
    /// <returns></returns>
    public KoudaiData GetKoudaiData( string koudaiName )
    {
      KoudaiData kd = new KoudaiData() ;
      if ( ( from data in ListKoudaiData where data.AllKoudaiFlatData.KoudaiName == koudaiName select data ).Any() ) {
        kd = ( from data in ListKoudaiData where data.AllKoudaiFlatData.KoudaiName == koudaiName select data ).First() ;
      }

      return kd ;
    }

    /// <summary>
    /// グループ情報を取得
    /// </summary>
    /// <param name="koudaiName">構台名称</param>
    /// <returns></returns>
    public GroupData GetGroupData( string groupName )
    {
      GroupData gd = new GroupData() ;
      if ( ( from data in ListGroupData where data.GroupName == groupName select data ).Any() ) {
        gd = ( from data in ListGroupData where data.GroupName == groupName select data ).First() ;
      }

      return gd ;
    }

    /// <summary>
    /// 構台情報をリストから削除
    /// </summary>
    /// <param name="koudaiName">構台名称</param>
    /// <returns></returns>
    public void DeleteKoudaiData( string koudaiName )
    {
      List<KoudaiData> lst = ListKoudaiData ;
      List<KoudaiData> lstNew = new List<KoudaiData>() ;
      foreach ( KoudaiData kd in lst ) {
        if ( kd.AllKoudaiFlatData.KoudaiName == koudaiName ) {
          continue ;
        }

        lstNew.Add( kd ) ;
      }

      ListKoudaiData = lstNew ;

      return ;
    }

    /// <summary>
    /// 構台情報を更新
    /// </summary>
    /// <param name="kd">構台情報</param>
    /// <returns></returns>
    public void UpdateKoudaiData( KoudaiData newkd )
    {
      List<KoudaiData> lst = ListKoudaiData ;
      List<KoudaiData> lstNew = new List<KoudaiData>() ;
      bool bUpdate = false ;
      foreach ( KoudaiData kd in lst ) {
        if ( kd.AllKoudaiFlatData.KoudaiName == newkd.AllKoudaiFlatData.KoudaiName ) {
          lstNew.Add( newkd ) ;
          bUpdate = true ;
        }
        else {
          lstNew.Add( kd ) ;
        }
      }

      //存在しない場合はリストに追加
      if ( bUpdate == false ) {
        lstNew.Add( newkd ) ;
      }

      ListKoudaiData = lstNew ;

      return ;
    }

    /// <summary>
    /// モデルデータの取得
    /// </summary>
    /// <returns></returns>
    public static ModelData GetModelData( Document doc )
    {
      ModelData md = new ModelData() ;
      string strXml = string.Empty ;
      try {
        // EXStrageから文字列を取得
        var schema = Schema.Lookup( new Guid( SchemaId ) ) ;
        if ( schema == null ) {
          return md ;
        }

        //View3D View3D = RevitUtil.ClsRevitUtil.Get3DView(doc);
        View vi = GetYMSView( doc ) ;
        if ( vi == null ) {
          return md ;
        }

        // MLオブジェクトの拡張ストレージに保存されている Id を取得
        var retrievedEntity = vi.GetEntity( schema ) ;
        if ( ! retrievedEntity.IsValid() ) {
          return md ;
        }

        strXml = retrievedEntity.Get<String>( FieldName ) ;
        if ( string.IsNullOrEmpty( strXml ) ) {
          return md ;
        }

        //文字列からtextを出力
        if ( ! ExStrageUtils.WriteText( PathUtil.GetTempModelFilePath(), strXml ) ) {
          return md ;
        }

        // XMLとして取り込み
        if ( ! ReadXML( ref md ) ) {
          return md ;
        }
      }
      catch {
        return md ;
      }

      return md ;
    }

    /// <summary>
    /// モデルデータの更新
    /// </summary>
    /// <returns></returns>
    public static bool SetModelData( Document doc, ModelData md )
    {
      try {
        //xmlファイルに設計したモデル情報を出力
        if ( ! WriteXML( md ) ) {
          return false ;
        }

        //xmlファイルの内容をtxt形式で読み込み
        string strXml = string.Empty ;
        if ( ! ExStrageUtils.ReadText( PathUtil.GetTempModelFilePath(), ref strXml ) ) {
          return false ;
        }

        //テキスト情報を図面のExStrageに格納
        //View3D View3D = RevitUtil.ClsRevitUtil.Get3DView(doc);
        View vi = GetYMSView( doc ) ;
        //23 10/26 mod トランザクションは呼び出し元でStartする
        //using (var transaction = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
        //{
        //    transaction.Start();

        if ( vi == null ) {
          //MessageBox.Show("3DView({3D})が存在しません。");
          //return false;
          vi = CreateYMSView( doc ) ;
        }

        using ( var schema = CreateSchema() ) {
          using ( var entity = new Entity( schema ) ) {
            // set the value for this entity
            entity.Set<String>( FieldName, strXml ) ;
            vi.SetEntity( entity ) ;
          }
        }
        //    transaction.Commit();
        //}
      }
      catch {
        return false ;
      }

      return true ;
    }

    public static Schema CreateSchema()
    {
      var existedSchema = Schema.Lookup( new Guid( SchemaId ) ) ;
      if ( existedSchema != null ) {
        return existedSchema ;
      }

      using ( var schemaBuilder = new SchemaBuilder( new Guid( SchemaId ) ) ) {
        //PublicにしないとAutodesk.Revit.Exceptions InvalidOperationException発生
        schemaBuilder.SetReadAccessLevel( AccessLevel.Public ) ; // allow anyone to read the object
        schemaBuilder.SetWriteAccessLevel( AccessLevel.Public ) ; // restrict writing to this vendor only
        schemaBuilder.SetVendorId( VendorId ) ; // required because of restricted write-access
        schemaBuilder.SetSchemaName( SchemaName2 ) ;
        schemaBuilder.AddSimpleField( FieldName, typeof( String ) ) ;

        return schemaBuilder.Finish() ; // return the Schema object
      }
    }

    /// <summary>
    /// XMLからモデル情報を取得
    /// </summary>
    /// <param name="modelData">モデル情報</param>
    /// <returns></returns>
    public static bool ReadXML( ref ModelData modelData )
    {
      string filePath = PathUtil.GetTempModelFilePath() ;
      if ( System.IO.File.Exists( filePath ) ) {
        ModelData md = new ModelData() ;
        if ( ! ExStrageUtils.ReadXml<ModelData>( filePath, ref md ) ) {
          return false ;
        }

        modelData = md ;
      }

      return true ;
    }

    /// <summary>
    /// XMLからモデル情報を取得
    /// </summary>
    /// <param name="modelData">モデル情報</param>
    /// <returns></returns>
    public static bool WriteXML( ModelData modelData )
    {
      string filePath = PathUtil.GetTempModelFilePath() ;
      return ExStrageUtils.WriteXml<ModelData>( filePath, modelData ) ;
    }

    /// <summary>
    /// YMSビューを取得
    /// </summary>
    /// <param name="doc"></param>
    /// <returns></returns>
    public static View GetYMSView( Document doc )
    {
      FilteredElementCollector collector = new FilteredElementCollector( doc ) ;
      collector.OfClass( typeof( View ) ) ;
      foreach ( View v in collector ) {
        // skip view templates here because they are invisible in project browsers:
        if ( v != null && ! v.IsTemplate && v.Name == "YMS用" ) {
          return v ;
        }
      }

      return null ;
    }

    /// <summary>
    /// YMSビューを取得
    /// </summary>
    /// <param name="doc"></param>
    /// <returns></returns>
    public static View CreateYMSView( Document doc )
    {
      FilteredElementCollector collector = new FilteredElementCollector( doc ) ;
      collector.OfClass( typeof( View ) ) ;
      foreach ( View v in collector ) {
        // skip view templates here because they are invisible in project browsers:
        if ( v != null && ! v.IsTemplate && v.Name == "YMS用" ) {
          return v ;
        }

        // ViewFamilyTypeを取得する（3Dビュー用のViewFamilyTypeを取得する）
        IEnumerable<ViewFamilyType> viewFamilyTypes =
          from elem in new FilteredElementCollector( doc ).OfClass( typeof( ViewFamilyType ) )
          let type = elem as ViewFamilyType
          where type.ViewFamily == ViewFamily.Drafting
          select type ;
        ;

        // ViewFamilyTypeが取得できたかを確認する
        if ( viewFamilyTypes != null && viewFamilyTypes.Count() > 0 ) {
          // 新しいビューを作成
          using ( Transaction transaction = new Transaction( doc, "Create View" ) ) {
            // ビューを作成
            View newView = ViewDrafting.Create( doc, viewFamilyTypes.First().Id ) ;

            // ビューの名前を設定（必要に応じて名前を変更してください）
            newView.Name = "YMS用" ;
            return newView ;
          }
        }
      }

      return null ;
    }

    #endregion
  }
}