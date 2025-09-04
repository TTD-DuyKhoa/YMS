using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;

namespace YMS_gantry
{
  public static class DefineUtil
  {
    #region パス関連

    #endregion


    #region Enum群

    /// <summary>
    /// 工事種別
    /// </summary>
    public enum eKoujiType
    {
      None,
      Kenchiku,
      Doboku
    }

    /// <summary>
    /// 配置基準種別
    /// </summary>
    public enum eBaseLevel
    {
      None,
      OhbikiBtm,
      FukkouTop,
      Free
    }

    /// <summary>
    /// 材質
    /// </summary>
    public enum eMaterial
    {
      None,
      SS400,
      SM490,
      Free
    }

    /// <summary>
    /// 取付タイプ
    /// </summary>
    public enum eAttachWay
    {
      None,
      OneSide,
      BothSide
    }

    /// <summary>
    /// 取付方法
    /// </summary>
    public enum eJoinType
    {
      None,
      Bolt,
      Welding,
      Plate,
      Support
    }

    /// <summary>
    /// ボルト種類
    /// </summary>
    public enum eBoltType
    {
      None,
      Normal,
      HTB
    }

    /// <summary>
    /// 覆工板タイプ
    /// </summary>
    public enum eFukkoubanType
    {
      Free = 0,
      TwoM = 2,
      Three = 3
    }

    /// <summary>
    /// 部材種別
    /// </summary>
    public enum eBuzaiCategory
    {
      None,
      ShichuShijikui,
      OhbikiKetauke,
      Shikigeta,
      NedaShugeta,
      JifukuZuredome
    }

    #endregion

    #region 定数

    /// <summary>
    /// 覆工板の厚み
    /// </summary>
    public const double FukkouBAN_THICK = 208 ;

    /// <summary>
    /// mのmm換算
    /// </summary>
    public const double ONE_M_AS_MM = 1000 ;

    public const string NEDA = "根太" ;

    public const string SHUGETA = "主桁" ;

    public const string OHBIKI = "大引" ;

    public const string KETAUKE = "桁受" ;

    #endregion

    #region レベル名

    /// <summary>
    /// 覆工天端
    /// </summary>
    public const string LEVEL_Fukkou_TOP = "覆工天端" ;

    /// <summary>
    /// 大引下端
    /// </summary>
    public const string LEVEL_OHBIKI_BTM = "大引下端" ;

    #endregion

    #region パラメータ名

    /// <summary>
    /// 図面情報 設計指針
    /// </summary>
    public const string ZumenInfo_Sekkeishishin = "H_設計指針" ;

    /// <summary>
    /// 図面情報 得意先名
    /// </summary>
    public const string ZumenInfo_Tokuisaki = "H_得意先名" ;

    /// <summary>
    /// 図面情報 現場名
    /// </summary>
    public const string ZumenInfo_GenbaName = "H_現場名" ;

    /// <summary>
    /// 図面情報 設計番号
    /// </summary>
    public const string ZumenInfo_SekkeiNum = "H_設計番号" ;

    /// <summary>
    /// YMS共通パラメータ名
    /// </summary>
    public const string PARAM_MATERIAL_DETAIL = "H_YMS_KOUDAI_PARAM" ;

    /// <summary>
    /// パラメータ名:H長さ
    /// </summary>
    public const string PARAM_MATERIAL_LENGTH = "H長さ" ;

    /// <summary>
    /// ホストからのオフセット
    /// </summary>
    public const string PARAM_HOST_OFFSET = "ホストからのオフセット" ;

    /// <summary>
    /// 基準からのオフセット
    /// </summary>
    public const string PARAM_BASE_OFFSET = "基準レベルからの高さ" ;

    /// <summary>
    /// エンドプレート
    /// </summary>
    public const string PARAM_END_PLATE = "エンドプレート" ;

    /// <summary>
    /// ベースプレート
    /// </summary>
    public const string PARAM_BASE_PLATE = "ベースプレート" ;

    /// <summary>
    /// ベースプレート種類
    /// </summary>
    public const string PARAM_BASE_PLATE_SIZE = "ベースプレートの種類" ;

    /// <summary>
    /// トッププレート
    /// </summary>
    public const string PARAM_TOP_PLATE = "トッププレート" ;

    /// <summary>
    /// トッププレートW
    /// </summary>
    public const string PARAM_TOP_PLATE_W = "トッププレートW" ;

    /// <summary>
    /// トッププレートD
    /// </summary>
    public const string PARAM_TOP_PLATE_D = "トッププレートD" ;

    /// <summary>
    /// トッププレート厚さ
    /// </summary>
    public const string PARAM_TOP_PLATE_T = "トッププレート厚さ" ;

    /// <summary>
    /// トッププレート種類
    /// </summary>
    public const string PARAM_TOP_PLATE_SIZE = "トッププレート種類" ;

    /// <summary>
    /// トッププレート種類
    /// </summary>
    public const string PARAM_END_PLATE_SIZE_U = "エンドプレート上種類" ;

    /// <summary>
    /// トッププレート種類
    /// </summary>
    public const string PARAM_END_PLATE_SIZE_B = "エンドプレート下種類" ;

    /// <summary>
    /// 長さ
    /// </summary>
    public const string PARAM_LENGTH = "長さ" ;

    /// <summary>
    /// 切断長さ
    /// </summary>
    public const string PARAM_PILE_CUT_LENG = "切断長さ" ;

    /// <summary>
    /// 材質
    /// </summary>
    public const string PARAM_MATERIAL = "材質" ;

    /// <summary>
    /// 回転
    /// </summary>
    public const string PARAM_ROTATE = "回転" ;

    /// <summary>
    /// ジョイント数
    /// </summary>
    public const string PARAM_JOINT_COUNT = "ジョイント数" ;

    /// <summary>
    /// 継手
    /// </summary>
    public const string PARAM_JOINT_TYPE = "継手" ;

    /// <summary>
    /// 構台名
    /// </summary>
    public const string PARAM_KOUDAI_NAME = "構台名" ;

    /// <summary>
    /// 大分類
    /// </summary>
    public const string PARAM_DAI_BUNRUI = "大分類" ;

    /// <summary>
    /// 中分類
    /// </summary>
    public const string PARAM_CHUU_BUNRUI = "中分類" ;

    /// <summary>
    /// 小分類
    /// </summary>
    public const string PARAM_SHOU_BUNRUI = "小分類" ;

    /// <summary>
    /// サイズ
    /// </summary>
    public const string PARAM_SIZE = "サイズ" ;

    #endregion

    #region パラメータ値

    public enum PramYesNo
    {
      No = 0,
      Yes = 1
    }

    #endregion

    #region "コンボボックス"

    public static List<string> ListMaterial = new List<string>() { "SS400", "SM490" } ;

    #endregion
  }
}