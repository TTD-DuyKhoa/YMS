using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;

namespace YMS_Schedule
{
  public class Enum
  {
    /// <summary>
    /// 店所
    /// </summary>
    public enum Region
    {
      DUMMY, // ダミー
      Hokkaido, // 北海道
      Tohoku_General, // 東北 (一般)
      Tohoku_Kashima, // 東北 (鹿島)
      Tokyo, // 東京
      Yokohama, // 横浜
      Niigata, // 新潟
      Nagoya, // 名古屋
      Hokuriku, // 北陸
      Osaka, // 大阪
      Kobe, // 神戸
      Chugoku, // 中国
      Shikoku, // 四国
      Kyushu, // 九州
      Singapore // シンガポール
    }

    /// <summary>
    /// 裏込めタイプ
    /// </summary>
    public enum UragomeType
    {
      TypeNone, // タイプなし
      HS, // HS型
      RotaryBlock, // ロータリーブロック
      UniBlock, // ユニブロック
      ShortBlock // ショートブロック
    }
  }
}