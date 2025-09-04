using System.Threading ;

namespace YMS.gantry
{
  /// <summary>
  ///   A list of requests the dialog has available
  /// </summary>
  /// 
  public enum RequestId : int
  {
    /// <summary>
    /// None
    /// </summary>
    None = 0,

    /// <summary>
    /// "終了" request
    /// </summary>
    End = 1,

    /// <summary>
    /// 覆工板 request
    /// </summary>
    Fukkouban = 2,

    /// <summary>
    /// "EditSize" request
    /// </summary>
    EditSize = 3,

    /// <summary>
    /// "EditSizeCategory" request
    /// </summary>
    EditSizeCategory = 4,

    /// <summary>
    /// "EditLengthChange" request
    /// </summary>
    EditLengthChange = 5,

    /// <summary>
    /// 割付
    /// </summary>
    Waritsuke = 6,

    /// <summary>
    /// "Grouping" request
    /// </summary>
    Grouping = 7,

    /// <summary>
    /// "鋼板材" request
    /// </summary>
    Koubanzai = 8,

    /// <summary>
    /// "大引(桁受)" request
    /// </summary>
    Ohbiki = 9,

    /// <summary>
    /// "根太(主桁)" request
    /// </summary>
    Neda = 10,

    /// <summary>
    /// "覆工桁" request
    /// </summary>
    Fukkougeta = 11,

    /// <summary>
    /// "対傾構" request
    /// </summary>
    Taikeikou = 12,

    /// <summary>
    /// "敷桁" request
    /// </summary>
    Shikigeta = 13,

    /// <summary>
    /// "スチフナー" request
    /// </summary>
    Stiffener = 14,

    /// <summary>
    /// "高さ調整" request
    /// </summary>
    TakasaChousei = 15,

    /// <summary>
    /// "地覆・ズレ止め" request
    /// </summary>
    JifukuZuredome = 16,

    /// <summary>
    /// "手摺" request
    /// </summary>
    Tesuri = 17,

    /// <summary>
    /// "水平ブレス" request
    /// </summary>
    HorizontalBrace = 18,

    /// <summary>
    /// "垂直ブレス" request
    /// </summary>
    VerticalBrace = 19,

    /// <summary>
    /// "締結補助材" request
    /// </summary>
    TeiketsuHojozai = 20,

    /// <summary>
    /// "水平つなぎ" request
    /// </summary>
    HorizontalTsunagi = 21,

    /// <summary>
    /// "方杖" request
    /// </summary>
    Houdue = 22,

    /// <summary>
    /// "支柱" request
    /// </summary>
    Shichu = 23,

    /// <summary>
    /// "構台杭" request
    /// </summary>
    Kui = 24,

    /// <summary>
    /// 位置移動
    /// </summary>
    EditLocation = 25,
  }

  public class Request
  {
    private int m_request = (int) RequestId.None ;

    public RequestId Take()
    {
      return (RequestId) Interlocked.Exchange( ref m_request, (int) RequestId.None ) ;
    }

    public void Make( RequestId request )
    {
      Interlocked.Exchange( ref m_request, (int) request ) ;
    }
  }
}