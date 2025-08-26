#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;

#endregion

namespace YMS_gantry
{
    class App : IExternalApplication
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly string _dllPath = SystemData.GetDLLPath();

        public Result OnStartup(UIControlledApplication a)
        {
            InitNLog();
            AddRibbonMenu(a);

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }

        /// <summary>
        /// NLog の初期化処理(NLog.config 読込)
        /// </summary>
        private void InitNLog()
        {
            var configPath = Path.Combine(PathUtil.GetYMSgantryDir(), "NLog.config");
            if (File.Exists(configPath))
            {
                NLog.LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration(configPath);
            }
            logger.Info("Setup NLog.LogManager.Configuration");
        }

        public void AddRibbonMenu(UIControlledApplication app)
        {
            string dllPath = SystemData.GetDLLPath();
            app.CreateRibbonTab("YMS構台");

            RibbonPanel panelAllPut = app.CreateRibbonPanel("YMS構台", "一括配置");

            // [一括配置] 構台(フラット)
            CreatePushButton(panelAllPut, "CmdAllPut1", "フラット", "AllPutKoudaiFlat", "構台(フラット)を一括配置します。", "1_フラット");

            //// [一括配置] 構台(特殊形状)
            //CreatePushButton(panelAllPut, "CmdAllPut2", "構台(特殊形状)", "AllPutKoudaiUnique", "構台(特殊形状)を一括配置します。");

            // [一括配置] スロープ作成
            CreatePushButton(panelAllPut, "CmdAllPut3", "スロープ", "CreateSlope", "スロープを作成します。", "2_スロープ");

            // [一括配置] ブレース・ツナギ配置
            CreatePushButton(panelAllPut, "CmdAllPut4", "ブレース・ツナギ", "AllPutBraceTsunagi", "ブレース・ツナギ材を一括配置します。", "3_ブレース・ツナギ");

            // [一括配置] グルーピング
            CreatePushButton(panelAllPut, "CmdAllPut5", "グルーピング", "Grouping", "構台をグルーピングします。", "4_グルーピング");

            RibbonPanel panelPut = app.CreateRibbonPanel("YMS構台", "個別配置");

            //// [個別配置] テスト用 ※最終的には削除
            //CreatePushButton(panelPut, "CmdPutTest", "テスト用", "PutTest", "テスト用のコマンドです。", "0_Large_dummy");

            // [個別配置] 覆工板
            CreatePushButton(panelPut, "CmdPut1", "覆工板", "PutFukkouban", "覆工板を配置します。", "5_覆工板");

            // [個別配置] 大引(桁受)
            // [個別配置] 根太(主桁)
            PushButtonDef pushButtonDef1 = new PushButtonDef("CmdPut2", "大引(桁受)", "PutOobikiKetauke", "大引(桁受)を配置します。", "6_大引");
            PushButtonDef pushButtonDef2 = new PushButtonDef("CmdPut3", "根太(主桁)", "PutNedaShugeta", "根太(主桁)を配置します。", "7_根太");
            CreateStackedPushButton(panelPut, new List<PushButtonDef> { pushButtonDef1, pushButtonDef2 });

            // [個別配置] 覆工桁配置
            // [個別配置] 敷桁配置
            PushButtonDef pushButtonDef3 = new PushButtonDef("CmdPut4", "覆工桁", "PutFukkougeta", "覆工桁を配置します。", "8_覆工桁");
            PushButtonDef pushButtonDef4 = new PushButtonDef("CmdPut5", "敷桁", "PutShikigeta", "敷桁を配置します。", "9_敷桁");
            CreateStackedPushButton(panelPut, new List<PushButtonDef> { pushButtonDef3, pushButtonDef4 });

            // [個別配置] 対傾構・対傾構取付プレート配置
            CreatePushButton(panelPut, "CmdPut6", "対傾構", "PutTaikeikou", "対傾構・対傾構取付プレートを配置します。", "10_対傾構");

            // [個別配置] スチフナープレート・スチフナージャッキ配置
            CreatePushButton(panelPut, "CmdPut7", "スチフナー", "PutSuchifuna", "スチフナープレート・スチフナージャッキを配置します。", "11_スチフナー");

            // [個別配置] 高さ調整プレート・調整材配置
            CreatePushButton(panelPut, "CmdPut8", " 調整材", "PutTakasaChouseiPlate", " 高さ調整プレート・調整材を配置します。", "12_調整材");

            // [個別配置] 敷鉄板配置
            CreatePushButton(panelPut, "CmdPut9", "敷鉄板", "PutKoubanzai", "敷鉄板を配置します。", "13_敷鉄板");

            // [個別配置] 地覆・覆工板ズレ止め材配置
            CreatePushButton(panelPut, "CmdPut10", "覆工板ズレ止め", "PutJifukuFukkoubanZuredome", "地覆・覆工板ズレ止め材を配置します。", "14_覆工板ズレ止め");

            // [個別配置] 手摺・手摺支柱配置
            CreatePushButton(panelPut, "CmdPut11", "手摺", "PutTesuriTesurishichu", "手摺・手摺支柱を配置します。", "15_手摺");

            // [個別配置] 水平ブレース配置
            // [個別配置] 垂直ブレース配置
            // [個別配置] 水平ツナギ配置
            PushButtonDef pushButtonDef5 = new PushButtonDef("CmdPut12", "水平ブレース", "PutHorizontalBrace", "水平ブレースを配置します。", "16_水平ブレース");
            PushButtonDef pushButtonDef6 = new PushButtonDef("CmdPut13", "垂直ブレース", "PutVerticalBrace", "垂直ブレースを配置します。", "17_垂直ブレース");
            PushButtonDef pushButtonDef7 = new PushButtonDef("CmdPut14", "水平ツナギ", "PutHorizontalTsunagi", "水平ツナギを配置します。", "18_水平ツナギ");
            CreateStackedPushButton(panelPut, new List<PushButtonDef> { pushButtonDef5, pushButtonDef6, pushButtonDef7 });

            // [個別配置] 取付補助材配置
            CreatePushButton(panelPut, "CmdPut15", "取付補助材", "PutTeiketsuHojyozai", "締結補助材を配置します。", "19_取付補助材");

            // [個別配置] 方杖配置
            CreatePushButton(panelPut, "CmdPut16", "方杖", "PutHoudue", "方杖を配置します。", "20_方杖");

            // [個別配置] 支柱配置
            CreatePushButton(panelPut, "CmdPut17", "支柱", "PutShichu", "支柱を配置します。", "21_支柱");

            // [個別配置] 構台杭(支持杭)
            CreatePushButton(panelPut, "CmdPut18", "構台杭(支持杭)", "PutKui", "構台杭(支持杭)を配置します。", "22_構台杭");

            RibbonPanel panelEdit = app.CreateRibbonPanel("YMS構台", "部材修正");

            // [部材修正] サイズ一覧
            CreatePushButton(panelEdit, "CmdEdit1", "サイズ一覧", "EditSizeList", "各種部材のサイズを変更します。", "23_サイズ一覧");

            // [部材修正] 位置変更
            CreatePushButton(panelEdit, "CmdEdit2", "位置変更", "EditLocationChange", "各種部材の位置を変更します。", "24_位置変更");

            // [部材修正] 長さ変更
            CreatePushButton(panelEdit, "CmdEdit3", "長さ変更", "EditLengthChange", "各種部材の長さを変更します。", "25_長さ変更");

            RibbonPanel panelWaritsuke = app.CreateRibbonPanel("YMS構台", "割付");

            // [割付] 部材割付
            CreatePushButton(panelWaritsuke, "CmdWaritsuke1", "部材割付", "WaritsukeElement", "各種部材の割付をします。", "26_部材割付");

            RibbonPanel panelSonota = app.CreateRibbonPanel("YMS構台", "その他");

            // [その他] 干渉チェック
            CreatePushButton(panelSonota, "CmdSonota1", "干渉チェック", "SonotaKanshouCheck", "干渉チェックを確認します。", "27_干渉チェック");

            // [その他] 個別配置
            CreatePushButton(panelSonota, "PushButton Command64", "個別配置", "KobetsuHaichi", "各部材の個別配置を行います。", "33 個別配置");

        }

        private void CreatePushButton(RibbonPanel panel, string name, string text, string className, string toolTipText, string iconName)
        {
            PushButtonData pushButtonData = new PushButtonData(name, text, _dllPath, $"YMS_gantry.{className}");
            if (File.Exists(GetIconPath(iconName)))
            {
                pushButtonData.LargeImage = new BitmapImage(new Uri(GetIconPath(iconName)));
                pushButtonData.Image = new BitmapImage(new Uri(GetIconPath(iconName)));
                pushButtonData.ToolTipImage = new BitmapImage(new Uri(GetIconPath(iconName)));
            }
            PushButton pushButton = panel.AddItem(pushButtonData) as PushButton;
            pushButton.ToolTip = toolTipText;
        }

        private void CreateStackedPushButton(RibbonPanel panel, List<PushButtonDef> pushButtonDefList)
        {
            List<PushButtonData> pushButtonDataList = new List<PushButtonData>();
            foreach (var pushButtonDef in pushButtonDefList)
            {
                PushButtonData pushButtonData = new PushButtonData(pushButtonDef.Name, pushButtonDef.Text, _dllPath, $"YMS_gantry.{pushButtonDef.ClassName}");
                if (File.Exists(GetIconPath(pushButtonDef.IconName)))
                {
                    pushButtonData.LargeImage = new BitmapImage(new Uri(GetIconPath(pushButtonDef.IconName)));
                    pushButtonData.Image = new BitmapImage(new Uri(GetIconPath(pushButtonDef.IconName)));
                    pushButtonData.ToolTipImage = new BitmapImage(new Uri(GetIconPath(pushButtonDef.IconName)));
                }
                pushButtonDataList.Add(pushButtonData);
            }

            if (pushButtonDataList.Count == 2)
            {
                IList<RibbonItem> addedButtons = panel.AddStackedItems(pushButtonDataList[0], pushButtonDataList[1]);
                List<PushButton> pushBtnList = new List<PushButton>();
                foreach (var item in addedButtons)
                {
                    pushBtnList.Add(item as PushButton);
                }
                PushButton pushBtn1 = pushBtnList[0];
                pushBtn1.ToolTip = pushButtonDefList[0].ToolTipText;
                PushButton pushBtn2 = pushBtnList[1];
                pushBtn2.ToolTip = pushButtonDefList[1].ToolTipText;
            }

            if (pushButtonDataList.Count == 3)
            {
                IList<RibbonItem> addedButtons = panel.AddStackedItems(pushButtonDataList[0], pushButtonDataList[1], pushButtonDataList[2]);
                List<PushButton> pushBtnList = new List<PushButton>();
                foreach (var item in addedButtons)
                {
                    pushBtnList.Add(item as PushButton);
                }
                PushButton pushBtn1 = pushBtnList[0];
                pushBtn1.ToolTip = pushButtonDefList[0].ToolTipText;
                PushButton pushBtn2 = pushBtnList[1];
                pushBtn2.ToolTip = pushButtonDefList[1].ToolTipText;
                PushButton pushBtn3 = pushBtnList[2];
                pushBtn3.ToolTip = pushButtonDefList[2].ToolTipText;
            }
        }

        /// <summary>
        /// アイコンのフルパス取得
        /// </summary>
        /// <param name="name">アイコン名（拡張子なし）</param>
        /// <param name="jpg">true = png  false = jpg</param>
        /// <returns></returns>
        private string GetIconPath(string name, bool png = true)
        {
            string res = string.Empty;
            string ymsGantryDir = PathUtil.GetExecutingAssemblyYMSGantryPath();
            string iconFolderPath = Path.Combine(ymsGantryDir, "icon");

            res = Path.Combine(iconFolderPath, name);

            if (png)
            {
                res = res + ".png";
            }
            else
            {

                res = res + ".jpg";
            }

            return res;
        }
    }

    public class PushButtonDef
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public string ClassName { get; set; }
        public string ToolTipText { get; set; }
        public string IconName { get; set; }

        public PushButtonDef(string name, string text, string className, string toolTipText, string iconName)
        {
            this.Name = name;
            this.Text = text;
            this.ClassName = className;
            this.ToolTipText = toolTipText;
            this.IconName = iconName;
        }
    }
}
