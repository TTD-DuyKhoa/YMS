using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YMS.Parts;

namespace YMS
{
    public class ClsGlobal
    {
        public static List<string> m_syorihouhou3 = new List<string>{ "自動", "1辺", "二点間" };
        public static List<string> m_corneryaitasize = new List<string> { "C3", "C4", "特殊コーナー", "ピースコーナー", "なし" };
        public static List<string> m_zanti = new List<string> { "なし", "全埋め殺し" ,"一部埋め殺し"};
        public static List<string> m_zanti1 = new List<string> { "なし"};
        public static List<string> m_zaishitu = new List<string> { "SY295", "SYW295" };
        public static List<string> m_zaishitu2 = new List<string> { "SS400", "SM490A","SY295" };
        public static List<string> m_boltsize = new List<string> { "ハイテン", "トルシア" };
        public static List<string> m_syorihouhou5 = new List<string> { "自動", "1辺", "2点間", "一点", "コーナー" };
        public static List<string> m_yokoyaitaytype = new List<string> { "軽量鋼矢板", "鋼矢板", "木矢板" };
        public static List<string> m_syorihouhou4desumi = new List<string> { "自動", "1辺", "2点間", "出隅コーナー" };
        public static List<string> m_haitiititype = new List<string> { "H型鋼　広幅", "H型鋼　中幅", "H型鋼　細幅" };
        public static List<string> m_syorihouhou4 = new List<string> { "自動", "1辺", "2点間", "コーナー" };
        public static List<string> m_tanbubuhin = new List<string> { "なし", "火打ブロック（小）", "火打ブロック",
            "自在火打受ピース", "2連式補助ピース", "回転火打受ピース" };
        public static List<string> m_jyakki = new List<string> { "油圧ジャッキ(NOP)" };
        public static List<string> m_baseShinList = new List<string> { ClsHaraokoshiBase.baseName, ClsKiribariBase.baseName, ClsCornerHiuchiBase.baseName,
                                ClsKiribariHiuchiBase.baseName, ClsKiribariUkeBase.baseName, ClsKiribariTsunagizaiBase.baseName,
                                ClsHiuchiTsunagizaiBase.baseName, ClsKiribariTsugiBase.baseName, 
                                ClsSyabariBase.baseName, ClsSyabariTsunagizaiBase.baseName, ClsSyabariUkeBase.baseName, ClsSyabariHiuchiBase.baseName};//都度ベース芯を追加予定
        public static List<string> m_sanjikuPeaceList = new List<string> { "三軸ピース _30SHP", "三軸ピース _35SHP", "三軸ピース _40SHP", "三軸ピース _50SHP-N" };
        public static List<string> m_baseHiuchiList = new List<string> { "隅火打ベース", "切梁火打ベース" };
        public static List<string> m_baseKiriOrHaraList = new List<string> { "腹起ベース", "切梁ベース" };
        public static List<string> m_baseKiriOrKHiuchiOrCHiuchiList = new List<string> { "切梁ベース", "切梁火打ベース", "隅火打ベース" };
        public static List<string> m_baseKiriOrKHiuchiOrCHiuchiOrSyabariList = new List<string> { "切梁ベース", "切梁火打ベース", "隅火打ベース", "斜梁ベース"};
        public static List<string> m_baseAllList = new List<string> { "切梁ベース", "切梁火打ベース", "隅火打ベース" , "火打繋ぎ材ベース" , "切梁継ぎベース", "切梁繋ぎ材ベース","切梁受け材ベース","腹起ベース"};

        //図面情報設定
        public const string m_zumeninfoSekkeishishin = "H_設計指針";
        public const string m_zumeninfoTokuisaki = "H_得意先名";
        public const string m_zumeninfoGenbaName = "H_現場名";
        public const string m_zumeninfoSekkeiNum = "H_設計番号";

        public const string m_length = "長さ";
        public const string m_dia = "直径";
        public const string m_refLvTop = "基準レベルからの高さ";

        public const string m_3DView = "{3D}";// //"解析モデル";

        //色
        public static Color m_redColor = new Color(255, 0, 0); // RGB値
        public static Color m_orangeColor = new Color(255, 165, 0);
        public static Color m_lightBlue = new Color(0, 255, 255);
        public static Color m_blackColor = new Color(255, 255, 255);
        public static System.Drawing.Color m_GrayColor = System.Drawing.Color.Gray;
        public static System.Drawing.Color m_WhiteColor = System.Drawing.Color.White;
        public static DLG.DlgCreateSMW m_SMW { get; set; }

        public static bool m_KariCreate = true;
    }
}
