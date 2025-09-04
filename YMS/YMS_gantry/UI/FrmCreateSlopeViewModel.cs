using Autodesk.Revit.DB ;
using NLog.LayoutRenderers ;
using System ;
using System.CodeDom ;
using System.Collections.Generic ;
using System.ComponentModel ;
using System.Linq ;
using System.Reflection ;
using System.Runtime.CompilerServices ;
using System.Security.Policy ;
using System.Text ;
using System.Threading.Tasks ;
using System.Xml.Serialization ;
using YMS_gantry.UI.FrnCreateSlopeControls ;

namespace YMS_gantry.UI
{
  public class FrmCreateSlopeViewModel : BindableSuper
  {
    // Doc 上の全ての構台情報
    public BindingList<KodaiSlopeModel> KodaiSet { get ; } = new BindingList<KodaiSlopeModel>() ;

    // 構台名コンボで選択中の構台
    private KodaiSlopeModel m_SelectedKodai { get ; set ; }

    public KodaiSlopeModel SelectedKodai
    {
      get => m_SelectedKodai ;
      set
      {
        if ( m_SelectedKodai != value ) {
          m_SelectedKodai = value ;
          this.OnPropertyChanged() ;
          this.OnPropertyChanged( nameof( SelectedKodai ) ) ;
        }
      }
    }

    // 表示中のスロープ情報
    public BindingList<StyleRow> StyleRowSet { get ; } = new BindingList<StyleRow>() ;

    public BindingList<SupportRow> SupportRowSet { get ; } = new BindingList<SupportRow>() ;

    public FrmCreateSlopeViewModel()
    {
      PropertyChanged += ( s, e ) =>
      {
        var vm = s as FrmCreateSlopeViewModel ;
        if ( e.PropertyName == nameof( SelectedKodai ) ) {
          StyleRowSet.Clear() ;
          foreach ( var x in SelectedKodai.StyleList ) {
            StyleRowSet.Add( x ) ;
          }

          SupportRowSet.Clear() ;
          foreach ( var x in SelectedKodai.SupportList ) {
            SupportRowSet.Add( x ) ;
          }
        }
      } ;
    }

    [Serializable]
    [System.Xml.Serialization.XmlRoot( "KodaiSlopeModel" )]
    public class KodaiSlopeModel : BindableSuper, IEquatable<KodaiSlopeModel>
    {
      private string m_Name { get ; set ; }

      [System.Xml.Serialization.XmlElement( "Name" )]
      public string Name
      {
        get => m_Name ;
        set
        {
          if ( m_Name != value ) {
            m_Name = value ;
            this.OnPropertyChanged() ;
            this.OnPropertyChanged( nameof( Name ) ) ;
          }
        }
      }

      [System.Xml.Serialization.XmlElement( "StyleList" )]
      public StyleRow[] StyleList { get ; set ; }

      [System.Xml.Serialization.XmlElement( "SupportList" )]
      public SupportRow[] SupportList { get ; set ; }

      /// <summary>
      /// 全スパン乗り入れ部が選択されたときの勾配の向き
      /// </summary>
      [System.Xml.Serialization.XmlElement( "Direction" )]
      public SlopeDirection Direction { get ; set ; } = SlopeDirection.StartToEnd ;

      public bool Equals( KodaiSlopeModel other )
      {
        if ( other == null ) return false ;
        if ( Name != other?.Name ) return false ;
        if ( ! EqualValueArray( StyleList, other?.StyleList ) ) return false ;
        if ( ! EqualValueArray( SupportList, other?.SupportList ) ) return false ;
        return true ;
      }
    }

    public enum SlopeDirection
    {
      StartToEnd,
      EndToStart
    }

    /// <summary>
    /// スロープ種類の 1 列
    /// </summary>
    [Serializable]
    [System.Xml.Serialization.XmlRoot( "StyleRow" )]
    public class StyleRow : BindableSuper
    {
      [System.Xml.Serialization.XmlElement( "Index" )]
      public int Index { get ; set ; } = -1 ;

      //private string m_RowHeader { get; set; }
      public string RowHeader
      {
        get
        {
          if ( Index < 0 ) {
            return "error" ;
          }

          return $"スパン{Index + 1}" ;
        }
      }

      private SlopeStyle m_SlopeType { get ; set ; }

      [System.Xml.Serialization.XmlElement( "SlopeType" )]
      public SlopeStyle SlopeType
      {
        get => m_SlopeType ;
        set
        {
          if ( m_SlopeType != value ) {
            m_SlopeType = value ;
            this.OnPropertyChanged() ;
            this.OnPropertyChanged( nameof( SlopeType ) ) ;
          }
        }
      }

      private double m_Percent { get ; set ; }

      [System.Xml.Serialization.XmlElement( "Percent" )]
      public double Percent
      {
        get => m_Percent ;
        set
        {
          if ( m_Percent != value ) {
            m_Percent = value ;
            this.OnPropertyChanged() ;
            this.OnPropertyChanged( nameof( Percent ) ) ;
          }
        }
      }

      private double m_Level { get ; set ; }

      [System.Xml.Serialization.XmlElement( "Level" )]
      public double Level
      {
        get => m_Level ;
        set
        {
          if ( m_Level != value ) {
            m_Level = value ;
            this.OnPropertyChanged() ;
            this.OnPropertyChanged( nameof( Level ) ) ;
          }
        }
      }
    }

    /// <summary>
    /// スロープ補助部材の 1 列
    /// </summary>
    [Serializable]
    [System.Xml.Serialization.XmlRoot( "SupportRow" )]
    public class SupportRow : BindableSuper
    {
      [System.Xml.Serialization.XmlElement( "Index" )]
      public int Index { get ; set ; } = -1 ;

      /// <summary>
      /// 柱 橋長ピッチの個数
      /// </summary>
      [System.Xml.Serialization.XmlElement( "KyochoPitchNum" )]
      public int KyochoPitchNum { get ; set ; } = -1 ;

      public string RowHeader
      {
        get
        {
          if ( Index < 0 ) {
            return "error" ;
          }
          else if ( Index == 0 ) {
            return "スパン1始点側" ;
          }
          else if ( Index == KyochoPitchNum ) {
            return $"スパン{Index}終点側" ;
          }

          return $"スパン{Index}～{Index + 1}" ;
        }
      }

      private string m_HasTensetsuban { get ; set ; }

      [System.Xml.Serialization.XmlElement( "HasTensetsuban" )]
      public string HasTensetsuban
      {
        get => m_HasTensetsuban ;
        set
        {
          if ( m_HasTensetsuban != value ) {
            m_HasTensetsuban = value ;
            this.OnPropertyChanged() ;
            this.OnPropertyChanged( nameof( HasTensetsuban ) ) ;
          }
        }
      }

      private string m_Madumezai { get ; set ; }

      [System.Xml.Serialization.XmlElement( "Madumezai" )]
      public string Madumezai
      {
        get => m_Madumezai ;
        set
        {
          if ( m_Madumezai != value ) {
            m_Madumezai = value ;
            this.OnPropertyChanged() ;
            this.OnPropertyChanged( nameof( Madumezai ) ) ;
          }
        }
      }

      private string m_Stiffener { get ; set ; }

      [System.Xml.Serialization.XmlElement( "Stiffener" )]
      public string Stiffener
      {
        get => m_Stiffener ;
        set
        {
          if ( m_Stiffener != value ) {
            m_Stiffener = value ;
            this.OnPropertyChanged() ;
            this.OnPropertyChanged( nameof( Stiffener ) ) ;
          }
        }
      }
    }

    public enum SlopeStyle
    {
      [ComboEnum( "スロープ無" )]
      Nil,

      [ComboEnum( "乗り入れ部" )]
      Entrance,

      [ComboEnum( "スロープ" )]
      Slope,
    }

    public enum MadumezaiType
    {
      [ComboEnum( "間詰材無" )]
      Nil,

      [ComboEnum( "L75x6" )]
      L75x6,

      [ComboEnum( "L75x9" )]
      L75x9,
    }

    public enum StiffenerType
    {
      [ComboEnum( "スチフナー無" )]
      Nil,

      [ComboEnum( "プレート" )]
      Plate,

      [ComboEnum( "ジャッキ (SH)" )]
      Jack,

      [ComboEnum( "ジャッキ (DWJ)" )]
      JackDwj,
    }

    public static KodaiSlopeModel InitWith( AllKoudaiFlatFrmData kodaiData )
    {
      //var hukuinNum = kodaiData.HukuinPillarPitch.Count();
      var kyochoNum = kodaiData.KyoutyouPillarPitch.Count() - 1 ;
      var result = new KodaiSlopeModel
      {
        Name = kodaiData.KoudaiName,
        StyleList = Enumerable.Range( 0, kyochoNum )
          .Select( x => new StyleRow
          {
            Index = x, Level = 0.0, SlopeType = SlopeStyle.Nil, Percent = 0.0,
          } ).ToArray(),
        SupportList = Enumerable.Range( 0, kyochoNum + 1 ).Select( x => new SupportRow
        {
          Index = x,
          KyochoPitchNum = kyochoNum,
          HasTensetsuban = YMSGridNames.Nil, //false,
          Madumezai = YMSGridNames.Nil, //MadumezaiType.Nil,
          Stiffener = YMSGridNames.Nil, // "",//StiffenerType.Nil,
        } ).ToArray()
      } ;

      return result ;
    }

    public static bool EqualValue<T>( T a, T b ) where T : class
    {
      // 全ての XmlElement が付いたプロパティの値が一致しているとき真を返す
      var properties = typeof( T ).GetProperties().Where( y => y.GetCustomAttribute<XmlElementAttribute>() != null )
        .ToArray() ;
      return properties.All( x => x.GetValue( a ).Equals( x.GetValue( b ) ) ) ;
    }

    public static bool EqualValueArray<T>( IEnumerable<T> a, IEnumerable<T> b ) where T : class
    {
      var arrayA = a.ToArray() ;
      var arrayB = b.ToArray() ;
      if ( arrayA.Length != arrayB.Length ) {
        return false ;
      }

      for ( int i = 0 ; i < arrayA.Length ; i++ ) {
        if ( ! EqualValue( arrayA.ElementAtOrDefault( i ), arrayB.ElementAtOrDefault( i ) ) ) {
          return false ;
        }
      }

      return true ;
    }

    public static T DeepClone<T>( T a ) where T : class
    {
      using ( var memoryStream = new System.IO.MemoryStream() ) {
        var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter() ;
        binaryFormatter.Serialize( memoryStream, a ) ; // シリアライズ
        memoryStream.Seek( 0, System.IO.SeekOrigin.Begin ) ;
        return binaryFormatter.Deserialize( memoryStream ) as T ; // デシリアライズ
      }
      //var properties = typeof(T)
      //    .GetProperties(BindingFlags.Instance)
      //    .Where(y => y.GetCustomAttribute<XmlElementAttribute>() != null)
      //    .ToArray();
      //var result = Activator.CreateInstance<T>();
      //foreach (var x in properties)
      //{
      //    var original = x.GetValue(a);
      //    x.SetValue(result, original);
      //}
      //return result;
    }
  }

  class ComboEnumAttribute : Attribute
  {
    public string DisplayName { get ; set ; }

    public ComboEnumAttribute( string displayName )
    {
      DisplayName = displayName ;
    }
  }

  public static class EnumExtension
  {
    public static T GetAttrubute<T>( this Enum e ) where T : Attribute
    {
      var field = e?.GetType()?.GetField( e?.ToString() ) ;
      if ( field?.GetCustomAttribute<T>() is T att ) {
        return att ;
      }

      return null ;
    }
  }

  [Serializable]
  public abstract class BindableSuper : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged ;

    protected void OnPropertyChanged( [CallerMemberName] string propertyName = null ) =>
      PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) ) ;
  }
}