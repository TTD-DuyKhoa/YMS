using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMS_gantry.Data
{
    [Serializable]
    [System.Xml.Serialization.XmlRoot("BraceTsunagiData")]
    public class BraceTsunagiData
    {
        #region プロパティ
        /// <summary>
        /// 段数設定
        /// </summary>
        [System.Xml.Serialization.XmlElement("DanSetting")]
        public DanSetting DanSetting { get; set; }

        /// <summary>
        /// 垂直ブレース
        /// </summary>
        [System.Xml.Serialization.XmlElement("VerticalBrace")]
        public VerticalBrace VerticalBrace { get; set; }

        /// <summary>
        /// 水平ブレース
        /// </summary>
        [System.Xml.Serialization.XmlElement("HorizontalBrace")]
        public HorizontalBrace HorizontalBrace { get; set; }

        /// <summary>
        /// 水平ツナギ
        /// </summary>
        [System.Xml.Serialization.XmlElement("HorizontalTsunagi")]
        public HorizontalTsunagi HorizontalTsunagi { get; set; }
        #endregion
    }

    #region サブクラス
    /// <summary>
    /// 段数設定
    /// </summary>
    [Serializable]
    [System.Xml.Serialization.XmlRoot("DanSetting")]
    public class DanSetting
    {
        /// <summary>
        /// 水平ツナギ段数
        /// </summary>
        [System.Xml.Serialization.XmlElement("TsunagiDansu")]
        public int TsunagiDansu { get; set; }

        /// <summary>
        /// 基準間隔
        /// </summary>
        [System.Xml.Serialization.XmlElement("BaseSpan")]
        public int BaseSpan { get; set; }

        /// <summary>
        /// 段数設定一覧
        /// </summary>
        [System.Xml.Serialization.XmlElement("DanSettingDataList")]
        public List<DanSettingListData> DanSettingDataList { get; set; } = new List<DanSettingListData>();
    }

    /// <summary>
    /// 段数設定一覧データ
    /// </summary>
    [Serializable]
    [System.Xml.Serialization.XmlRoot("DanSettingListData")]
    public class DanSettingListData
    {
        /// <summary>
        /// 段
        /// </summary>
        [System.Xml.Serialization.XmlElement("Dan")]
        public string Dan { get; set; }

        /// <summary>
        /// 間隔
        /// </summary>
        [System.Xml.Serialization.XmlElement("Span")]
        public int Span { get; set; }

        /// <summary>
        /// 水平ツナギ有無
        /// </summary>
        [System.Xml.Serialization.XmlElement("TsunagiUmu")]
        public bool TsunagiUmu { get; set; }

        /// <summary>
        /// 垂直ブレース有無
        /// </summary>
        [System.Xml.Serialization.XmlElement("BraceUmu")]
        public bool BraceUmu { get; set; }
    }

    /// <summary>
    /// 垂直ブレース
    /// </summary>
    [Serializable]
    [System.Xml.Serialization.XmlRoot("VerticalBrace")]
    public class VerticalBrace
    {
        /// <summary>
        /// 橋軸側
        /// </summary>
        [System.Xml.Serialization.XmlElement("KyoujikuUmu")]
        public bool KyoujikuUmu { get; set; }

        /// <summary>
        /// 幅員側
        /// </summary>
        [System.Xml.Serialization.XmlElement("FukuinUmu")]
        public bool FukuinUmu { get; set; }

        /// <summary>
        /// サイズ
        /// </summary>
        [System.Xml.Serialization.XmlElement("Size")]
        public string Size { get; set; }

        /// <summary>
        /// 勝ち 幅員側
        /// </summary>
        [System.Xml.Serialization.XmlElement("KachiFukuin")]
        public bool KachiFukuin { get; set; }

        /// <summary>
        /// 勝ち 橋軸側
        /// </summary>
        [System.Xml.Serialization.XmlElement("KachiKyoujiku")]
        public bool KachiKyoujiku { get; set; }

        /// <summary>
        /// 一段目離れ(配置禁止範囲) 上 X
        /// </summary>
        [System.Xml.Serialization.XmlElement("FirstHanareTopX")]
        public int FirstHanareTopX { get; set; }

        /// <summary>
        /// 一段目離れ(配置禁止範囲) 上 Y
        /// </summary>
        [System.Xml.Serialization.XmlElement("FirstHanareTopY")]
        public int FirstHanareTopY { get; set; }

        /// <summary>
        /// 一段目離れ(配置禁止範囲) 下 X
        /// </summary>
        [System.Xml.Serialization.XmlElement("FirstHanareBottomX")]
        public int FirstHanareBottomX { get; set; }

        /// <summary>
        /// 一段目離れ(配置禁止範囲) 下 Y
        /// </summary>
        [System.Xml.Serialization.XmlElement("FirstHanareBottomY")]
        public int FirstHanareBottomY { get; set; }

        /// <summary>
        /// 二段目以降の離れ(配置禁止範囲) 上 X
        /// </summary>
        [System.Xml.Serialization.XmlElement("SecondLaterHanareTopX")]
        public int SecondLaterHanareTopX { get; set; }

        /// <summary>
        /// 二段目以降の離れ(配置禁止範囲) 上 Y
        /// </summary>
        [System.Xml.Serialization.XmlElement("SecondLaterHanareTopY")]
        public int SecondLaterHanareTopY { get; set; }

        /// <summary>
        /// 二段目以降の離れ(配置禁止範囲) 下 X
        /// </summary>
        [System.Xml.Serialization.XmlElement("SecondLaterHanareBottomX")]
        public int SecondLaterHanareBottomX { get; set; }

        /// <summary>
        /// 二段目以降の離れ(配置禁止範囲) 下 Y
        /// </summary>
        [System.Xml.Serialization.XmlElement("SecondLaterHanareBottomY")]
        public int SecondLaterHanareBottomY { get; set; }

        /// <summary>
        /// ラウンドあり
        /// </summary>
        [System.Xml.Serialization.XmlElement("RoundON")]
        public bool RoundON { get; set; }

        /// <summary>
        /// ラウンド長さ
        /// </summary>
        [System.Xml.Serialization.XmlElement("RoundLength")]
        public string RoundLength { get; set; }

        /// <summary>
        /// 取付方法 溶接
        /// </summary>
        [System.Xml.Serialization.XmlElement("ToritsukiYousetsu")]
        public bool ToritsukiYousetsu { get; set; }

        /// <summary>
        /// 取付方法 ボルト
        /// </summary>
        [System.Xml.Serialization.XmlElement("ToritsukiBolt")]
        public bool ToritsukiBolt { get; set; }

        /// <summary>
        /// 取付方法 締結金具
        /// </summary>
        [System.Xml.Serialization.XmlElement("ToritsukiTeiketsuKanagu")]
        public bool ToritsukiTeiketsuKanagu { get; set; }
    }

    /// <summary>
    /// 水平ブレース
    /// </summary>
    [Serializable]
    [System.Xml.Serialization.XmlRoot("HorizontalBrace")]
    public class HorizontalBrace
    {
        /// <summary>
        /// 配置有無 有り
        /// </summary>
        [System.Xml.Serialization.XmlElement("HaichiOn")]
        public bool HaichiOn { get; set; }

        /// <summary>
        /// 配置有無 無し
        /// </summary>
        [System.Xml.Serialization.XmlElement("HaichiOFF")]
        public bool HaichiOFF { get; set; }

        /// <summary>
        /// サイズ
        /// </summary>
        [System.Xml.Serialization.XmlElement("Size")]
        public string Size { get; set; }

        /// <summary>
        /// 配置位置 大引_上フランジ下側_下フランジ上側
        /// </summary>
        [System.Xml.Serialization.XmlElement("PositionTopFlaBottomAndBottomFlaTop")]
        public bool PositionTopFlaBottomAndBottomFlaTop { get; set; }

        /// <summary>
        /// 配置位置 大引_下フランジ上側_下フランジ下側
        /// </summary>
        [System.Xml.Serialization.XmlElement("PositionBottomFlaTopAndBottomFlaBottom")]
        public bool PositionBottomFlaTopAndBottomFlaBottom { get; set; }

        /// <summary>
        /// トッププレートに配置
        /// </summary>
        [System.Xml.Serialization.XmlElement("PutTopPL")]
        public bool PutTopPL { get; set; }

        /// <summary>
        /// 配置禁止範囲 X
        /// </summary>
        [System.Xml.Serialization.XmlElement("PutBanRangeX")]
        public int PutBanRangeX { get; set; }

        /// <summary>
        /// 配置禁止範囲 Y
        /// </summary>
        [System.Xml.Serialization.XmlElement("PutBanRangeY")]
        public int PutBanRangeY { get; set; }

        /// <summary>
        /// 取付方法 溶接
        /// </summary>
        [System.Xml.Serialization.XmlElement("ToritsukiYousetsu")]
        public bool ToritsukiYousetsu { get; set; }

        /// <summary>
        /// 取付方法 ボルト
        /// </summary>
        [System.Xml.Serialization.XmlElement("ToritsukiBolt")]
        public bool ToritsukiBolt { get; set; }

        /// <summary>
        /// 取付方法 締結金具
        /// </summary>
        [System.Xml.Serialization.XmlElement("ToritsukiTeiketsuKanagu")]
        public bool ToritsukiTeiketsuKanagu { get; set; }
    }

    /// <summary>
    /// 水平ツナギ
    /// </summary>
    [Serializable]
    [System.Xml.Serialization.XmlRoot("HorizontalTsunagi")]
    public class HorizontalTsunagi
    {
        /// <summary>
        /// 橋軸側
        /// </summary>
        [System.Xml.Serialization.XmlElement("KyoujikuUmu")]
        public bool KyoujikuUmu { get; set; }

        /// <summary>
        /// 幅員側
        /// </summary>
        [System.Xml.Serialization.XmlElement("FukuinUmu")]
        public bool FukuinUmu { get; set; }

        /// <summary>
        /// サイズ
        /// </summary>
        [System.Xml.Serialization.XmlElement("Size")]
        public string Size { get; set; }

        /// <summary>
        /// 上下設定 幅員側上_橋軸側下
        /// </summary>
        [System.Xml.Serialization.XmlElement("PositionFukuinTopAndKyoujikuBottom")]
        public bool PositionFukuinTopAndKyoujikuBottom { get; set; }

        /// <summary>
        /// 上下設定 幅員側下_橋軸側上
        /// </summary>
        [System.Xml.Serialization.XmlElement("PositionFukuinBottomAndKyoujikuTop")]
        public bool PositionFukuinBottomAndKyoujikuTop { get; set; }

        /// <summary>
        /// 突き出し量 始点側
        /// </summary>
        [System.Xml.Serialization.XmlElement("TsukidashiStart")]
        public int TsukidashiStart { get; set; }

        /// <summary>
        /// 突き出し量 終点側
        /// </summary>
        [System.Xml.Serialization.XmlElement("TsukidashiEnd")]
        public int TsukidashiEnd { get; set; }

        /// <summary>
        /// 取付方法 溶接
        /// </summary>
        [System.Xml.Serialization.XmlElement("ToritsukiYousetsu")]
        public bool ToritsukiYousetsu { get; set; }

        /// <summary>
        /// 取付方法 ボルト
        /// </summary>
        [System.Xml.Serialization.XmlElement("ToritsukiBolt")]
        public bool ToritsukiBolt { get; set; }

        /// <summary>
        /// 取付方法 締結金具
        /// </summary>
        [System.Xml.Serialization.XmlElement("ToritsukiTeiketsuKanagu")]
        public bool ToritsukiTeiketsuKanagu { get; set; }
    }
    #endregion
}
