#region Namespaces
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media.Imaging;
using YMS.Parts;
using YMS.UI;
#endregion

namespace YMS
{
    class App : IExternalApplication
    {
        public static UIApplication uiapp;

        public static UIControlledApplication uiContApp;

        public Result OnStartup(UIControlledApplication a)
        {
            uiContApp = a;
            uiapp = GetUiApplication();
            EventFactory.StartUp();
            var ribbonTab = new Ribbon("Selection Monitor", "Monitor");
            // Register wall updater with Revit
            WallUpdater updater = new WallUpdater(a.ActiveAddInId);
            UpdaterRegistry.RegisterUpdater(updater);

            // Change Scope = any Wall element
            ElementClassFilter wallFilter = new ElementClassFilter(typeof(Wall));

            // Change type = element addition
            //UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), wallFilter, Element.GetChangeTypeElementAddition());
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), wallFilter, Element.GetChangeTypeGeometry());

            // Register wall updater with Revit
            LineUpdater updaterL = new LineUpdater(a.ActiveAddInId);
            UpdaterRegistry.RegisterUpdater(updaterL);

            // Change Scope = any Wall element
            //ElementClassFilter LineFilter = new ElementClassFilter(typeof(Line));
            ElementCategoryFilter cLineFilter = new ElementCategoryFilter(BuiltInCategory.OST_Lines);

            // Change type = element addition
            //UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), wallFilter, Element.GetChangeTypeElementAddition());
            //UpdaterRegistry.AddTrigger(updaterL.GetUpdaterId(), cLineFilter, Element.GetChangeTypeElementAddition());
            //UpdaterRegistry.AddTrigger(updaterL.GetUpdaterId(), cLineFilter, Element.GetChangeTypeGeometry());
            UpdaterRegistry.AddTrigger(updaterL.GetUpdaterId(), cLineFilter, Element.GetChangeTypeAny());//これでModelLineを動かすと発動する

#if DEBUG
            AddRibbonMenu_test2(a);
#endif
            AddRibbonMenu_Setting(a);
            AddRibbonMenu_Yamadome(a);
            ClsCInput.Instance.setup(a.ActiveAddInId, "rubberLine");
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            EventFactory.ShutDown();
            WallUpdater updater = new WallUpdater(a.ActiveAddInId);
            UpdaterRegistry.UnregisterUpdater(updater.GetUpdaterId());
            LineUpdater updaterL = new LineUpdater(a.ActiveAddInId);
            UpdaterRegistry.UnregisterUpdater(updaterL.GetUpdaterId());
            return Result.Succeeded;
        }

        private static UIApplication GetUiApplication()
        {
            var versionNumber = uiContApp.ControlledApplication.VersionNumber;

            var fieldName = string.Empty;

            switch (versionNumber)
            {
                case "2019":

                    fieldName = "m_uiapplication";

                    break;

                case "2020":

                    fieldName = "m_uiapplication";

                    break;

                case "2021":

                    fieldName = "m_uiapplication";

                    break;

                case "2022":

                    fieldName = "m_uiapplication";

                    break;
                case "2023":

                    fieldName = "m_uiapplication";

                    break;
                default:

                    fieldName = "m_uiapplication";

                    break;
            }

            var fieldInfo = uiContApp.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

            var uiApplication = (UIApplication)fieldInfo?.GetValue(uiContApp);

            return uiApplication;
        }

        public void AddRibbonMenu_test(UIControlledApplication app)
        {
            //string dir = ClsSystemData.GetSystemFileFolder();
            string dllPath = ClsSystemData.GetDLLPath();

            string symbolFolpath = ClsYMSUtil.GetExecutingAssemblyYMSPath();
            //string ibPath = System.IO.Path.Combine(symbolFolpath, "icon\\アイコン32");
            //string isPath = System.IO.Path.Combine(symbolFolpath, "icon\\アイコン16");
            app.CreateRibbonTab("TEST");
            app.CreateRibbonTab("TEST用コマンド");
            RibbonPanel panel =
              app.CreateRibbonPanel("TEST", "TEST");
            RibbonPanel panel2 =
              app.CreateRibbonPanel("TEST用コマンド", "TEST用コマンド");

            PushButtonData pushButtonData1 = new PushButtonData("PushButton Command1", "test",
                            dllPath, "YMS.CommandTest");


            PushButton pushButton1 = panel.AddItem(pushButtonData1) as PushButton;
            //string symbolFolpath = ClsZumenInfo.GetFamilyFolder();
            //string iconPath = System.IO.Path.Combine(symbolFolpath, "icon\\testicon");
            //pushButton1.LargeImage = new BitmapImage(new Uri(iconPath));

            pushButton1.ToolTip = "test";

            ///////////////////////////////////////////////////////////////////////////
            //ReplaceModelLineToShin
            PushButtonData pushButtonData2 = new PushButtonData("PushButton Command2", " モデル線分から芯へ置換 ",
                          dllPath, "YMS.ReplaceModelLineToShin");


            PushButton pushButton2 = panel.AddItem(pushButtonData2) as PushButton;

            pushButton2.ToolTip = "モデル線分から芯へ置換";
            /////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData3 = new PushButtonData("PushButton Command3", " 共通芯に仮配置 ",
                         dllPath, "YMS.CreateKariKiribari");


            PushButton pushButton3 = panel.AddItem(pushButtonData3) as PushButton;

            pushButton3.ToolTip = "共通芯に仮配置";

            /////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData4 = new PushButtonData("PushButton Command4", "ルーラー割付テスト",
                         dllPath, "YMS.testtest");


            PushButton pushButton4 = panel.AddItem(pushButtonData4) as PushButton;

            pushButton4.ToolTip = "ピック点からX方向へルーラーが発生するので位置指定をしてください。割付が行われます。";

            /////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData5 = new PushButtonData("PushButton Command5", " FormTest ",
                          dllPath, "YMS.FormTest");


            PushButton pushButton5 = panel.AddItem(pushButtonData5) as PushButton;

            pushButton5.ToolTip = "FormTest";

            /////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData6 = new PushButtonData("PushButton Command6", "掘削用ボイドファミリの配置  ",
                          dllPath, "YMS.FormDlgCreateVoidFamily");


            PushButton pushButton6 = panel.AddItem(pushButtonData6) as PushButton;

            pushButton6.ToolTip = "ボイドを配置し指定のファミリを掘削します。";

            /////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData7 = new PushButtonData("PushButton Command7", "交点にファミリを配置  ",
                          dllPath, "YMS.CreateIntersectionFamilyCommand");


            PushButton pushButton7 = panel.AddItem(pushButtonData7) as PushButton;

            pushButton7.ToolTip = "線を2本指定しその交点にファミリを配置します。";

            /////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData8 = new PushButtonData("PushButton Command8", "ベース、仮鋼材取得  ",
                          dllPath, "YMS.CreateSymbolCommand");


            PushButton pushButton8 = panel.AddItem(pushButtonData8) as PushButton;

            pushButton8.ToolTip = "ベース、仮鋼材を取得。";

            /////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData9 = new PushButtonData("PushButton Command9", "指定芯に壁を配置  ",
                          dllPath, "YMS.CmdElevationWatcher");


            PushButton pushButton9 = panel.AddItem(pushButtonData9) as PushButton;

            pushButton9.ToolTip = "指定した芯に壁を配置します。";

            /////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData10 = new PushButtonData("PushButton Command10", "testteat  ",
                          dllPath, "YMS.Rubberband");


            PushButton pushButton10 = panel.AddItem(pushButtonData10) as PushButton;

            pushButton10.ToolTip = "図面情報を設定します。";
            /////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData11 = new PushButtonData("PushButton Command11", "CASETEST  ",
                          dllPath, "YMS.FormDlgCreateCASETest");


            PushButton pushButton11 = panel.AddItem(pushButtonData11) as PushButton;

            pushButton11.ToolTip = "CASE情報を設定します。";
            /////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData12 = new PushButtonData("PushButton Command12", "ラバーバンド  ",
                          dllPath, "YMS.Rubberband");


            PushButton pushButton12 = panel.AddItem(pushButtonData12) as PushButton;

            pushButton12.ToolTip = "ラバーバンド。";
            //#if DEBUG
            //            RibbonPanel panelTest =
            //              app.CreateRibbonPanel("Revit PHS TOOL", "Test");


            //            //test
            //            PushButtonData pushButtonDataTest = new PushButtonData("PushButton CommandTest", "test",
            //                            dllPath, "DaifukuRevitPHSTool.testkamada");
            //            PushButton pushButtonTest = panelTest.AddItem(pushButtonDataTest) as PushButton;

            //            pushButtonTest.ToolTip = "TEST";
            //#endif


            ////アイコンの設定
            //string iconPath = System.IO.Path.Combine(dir, "icon", "PHS_Replace.png");
            //if (System.IO.File.Exists(iconPath))
            //{
            //    pushButton1.LargeImage = new BitmapImage(new Uri(iconPath));
            //}

            PulldownButtonData groupTest_Kurane = new PulldownButtonData("TEST_Kurane", "TEST_Kurane");
            PulldownButton groupYamadomeTest_Kurane = panel.AddItem(groupTest_Kurane) as PulldownButton;

            PushButtonData pushButtonData13 = new PushButtonData("PushButton Command13", "コマンド1", dllPath, "YMS.FormDlgTest_Kurane");
            PushButton pushButton13 = groupYamadomeTest_Kurane.AddPushButton(pushButtonData13) as PushButton;
            pushButton13.ToolTip = "テストコマンド";

            PushButtonData pushButtonData14 = new PushButtonData("PushButton Command14", "コマンド2", dllPath, "YMS.FormDlgTest_Kurane2");
            PushButton pushButton14 = groupYamadomeTest_Kurane.AddPushButton(pushButtonData14) as PushButton;
            pushButton13.ToolTip = "テストコマンド";



            /////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData15 = new PushButtonData("PushButton Command15", "入隅・出隅チェック",
                          dllPath, "YMS.CheckHaraokoshiIrizumi");


            PushButton pushButton15 = panel.AddItem(pushButtonData15) as PushButton;

            pushButton15.ToolTip = "入隅・出隅チェックします。";
            /////////////////////////////////////////////////////////////////////////////

            //空のTESTcommand
            /////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData16 = new PushButtonData("PushButton Command16", "TEST1",
                          dllPath, "YMS.TEST1");


            PushButton pushButton16 = panel2.AddItem(pushButtonData16) as PushButton;

            pushButton16.ToolTip = "TEST1";
            /////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData17 = new PushButtonData("PushButton Command17", "TEST2",
                          dllPath, "YMS.TEST2");


            PushButton pushButton17 = panel2.AddItem(pushButtonData17) as PushButton;

            pushButton17.ToolTip = "TEST2";
            /////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData18 = new PushButtonData("PushButton Command18", "TEST3",
                          dllPath, "YMS.TEST3");


            PushButton pushButton18 = panel2.AddItem(pushButtonData18) as PushButton;

            pushButton18.ToolTip = "TEST3";
            /////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData19 = new PushButtonData("PushButton Command19", "TEST4",
                          dllPath, "YMS.TEST4");


            PushButton pushButton19 = panel2.AddItem(pushButtonData19) as PushButton;

            pushButton19.ToolTip = "TEST4";
            /////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData20 = new PushButtonData("PushButton Command20", "TEST5",
                          dllPath, "YMS.TEST5");


            PushButton pushButton20 = panel2.AddItem(pushButtonData20) as PushButton;

            pushButton20.ToolTip = "TEST5";
            /////////////////////////////////////////////////////////////////////////////

            //アイコンの設定

            //if (System.IO.File.Exists(ibPath) && System.IO.File.Exists(isPath))
            //{
            //    pushButton1.LargeImage = new BitmapImage(new Uri(ibPath));
            //    pushButton1.Image = new BitmapImage(new Uri(isPath));
            //    pushButton1.ToolTipImage = new BitmapImage(new Uri(isPath));
            //    pushButton2.LargeImage = new BitmapImage(new Uri(ibPath));
            //    pushButton2.Image = new BitmapImage(new Uri(isPath));
            //    pushButton2.ToolTipImage = new BitmapImage(new Uri(isPath));
            //    pushButton3.LargeImage = new BitmapImage(new Uri(ibPath));
            //    pushButton3.Image = new BitmapImage(new Uri(isPath));
            //    pushButton3.ToolTipImage = new BitmapImage(new Uri(isPath));
            //    pushButton4.LargeImage = new BitmapImage(new Uri(ibPath));
            //    pushButton4.Image = new BitmapImage(new Uri(isPath));
            //    pushButton4.ToolTipImage = new BitmapImage(new Uri(isPath));
            //    pushButton5.LargeImage = new BitmapImage(new Uri(ibPath));
            //    pushButton5.Image = new BitmapImage(new Uri(isPath));
            //    pushButton5.ToolTipImage = new BitmapImage(new Uri(isPath));
            //    pushButton6.LargeImage = new BitmapImage(new Uri(ibPath));
            //    pushButton6.Image = new BitmapImage(new Uri(isPath));
            //    pushButton6.ToolTipImage = new BitmapImage(new Uri(isPath));
            //    pushButton7.LargeImage = new BitmapImage(new Uri(ibPath));
            //    pushButton7.Image = new BitmapImage(new Uri(isPath));
            //    pushButton7.ToolTipImage = new BitmapImage(new Uri(isPath));
            //    pushButton8.LargeImage = new BitmapImage(new Uri(ibPath));
            //    pushButton8.Image = new BitmapImage(new Uri(isPath));
            //    pushButton8.ToolTipImage = new BitmapImage(new Uri(isPath));
            //    pushButton9.LargeImage = new BitmapImage(new Uri(ibPath));
            //    pushButton9.Image = new BitmapImage(new Uri(isPath));
            //    pushButton9.ToolTipImage = new BitmapImage(new Uri(isPath));
            //    pushButton10.LargeImage = new BitmapImage(new Uri(ibPath));
            //    pushButton10.Image = new BitmapImage(new Uri(isPath));
            //    pushButton10.ToolTipImage = new BitmapImage(new Uri(isPath));
            //    pushButton11.LargeImage = new BitmapImage(new Uri(ibPath));
            //    pushButton11.Image = new BitmapImage(new Uri(isPath));
            //    pushButton11.ToolTipImage = new BitmapImage(new Uri(isPath));
            //    pushButton12.LargeImage = new BitmapImage(new Uri(ibPath));
            //    pushButton12.Image = new BitmapImage(new Uri(isPath));
            //    pushButton12.ToolTipImage = new BitmapImage(new Uri(isPath));
            //    pushButton13.LargeImage = new BitmapImage(new Uri(ibPath));
            //    pushButton13.Image = new BitmapImage(new Uri(isPath));
            //    pushButton13.ToolTipImage = new BitmapImage(new Uri(isPath));
            //    pushButton14.LargeImage = new BitmapImage(new Uri(ibPath));
            //    pushButton14.Image = new BitmapImage(new Uri(isPath));
            //    pushButton14.ToolTipImage = new BitmapImage(new Uri(isPath));
            //    pushButton15.LargeImage = new BitmapImage(new Uri(ibPath));
            //    pushButton15.Image = new BitmapImage(new Uri(isPath));
            //    pushButton15.ToolTipImage = new BitmapImage(new Uri(isPath));


            //}
        }
        public void AddRibbonMenu_test2(UIControlledApplication app)
        {
            //string dir = ClsSystemData.GetSystemFileFolder();
            string dllPath = ClsSystemData.GetDLLPath();

            string symbolFolpath = ClsYMSUtil.GetExecutingAssemblyYMSPath();
            app.CreateRibbonTab("TEST用コマンド");
            RibbonPanel panel =
              app.CreateRibbonPanel("TEST用コマンド", "TEST用コマンド");

            //空のTESTcommand
            /////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData1 = new PushButtonData("PushButton Command1", "TEST1",
                          dllPath, "YMS.TEST1");


            PushButton pushButton1 = panel.AddItem(pushButtonData1) as PushButton;

            pushButton1.ToolTip = "TEST1";
            /////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData2 = new PushButtonData("PushButton Command2", "TEST2",
                          dllPath, "YMS.TEST2");


            PushButton pushButton2 = panel.AddItem(pushButtonData2) as PushButton;

            pushButton2.ToolTip = "TEST2";
            /////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData3 = new PushButtonData("PushButton Command3", "TEST3",
                          dllPath, "YMS.TEST3");


            PushButton pushButton3 = panel.AddItem(pushButtonData3) as PushButton;

            pushButton3.ToolTip = "TEST3";
            /////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData4 = new PushButtonData("PushButton Command4", "TEST4",
                          dllPath, "YMS.TEST4");


            PushButton pushButton4 = panel.AddItem(pushButtonData4) as PushButton;

            pushButton4.ToolTip = "TEST4";
            /////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData5 = new PushButtonData("PushButton Command5", "TEST5",
                          dllPath, "YMS.TEST5");


            PushButton pushButton5 = panel.AddItem(pushButtonData5) as PushButton;

            pushButton5.ToolTip = "TEST5";
            /////////////////////////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData6 = new PushButtonData("PushButton Command6", "TEST6",
                          dllPath, "YMS.TEST6");


            PushButton pushButton6 = panel.AddItem(pushButtonData6) as PushButton;

            pushButton6.ToolTip = "TEST6";
            /////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData7 = new PushButtonData("PushButton Command7", "TEST7",
                          dllPath, "YMS.TEST7");


            PushButton pushButton7 = panel.AddItem(pushButtonData7) as PushButton;

            pushButton7.ToolTip = "TEST7";
            /////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData8 = new PushButtonData("PushButton Command8", "TEST8",
                          dllPath, "YMS.TEST8");


            PushButton pushButton8 = panel.AddItem(pushButtonData8) as PushButton;

            pushButton8.ToolTip = "TEST8";
            /////////////////////////////////////////////////////////////////////////////
            ///
            PushButtonData pushButtonData9 = new PushButtonData("PushButton Command9", nameof(YMS.TEST9),
                          dllPath, typeof(YMS.TEST9).FullName);


            PushButton pushButton9 = panel.AddItem(pushButtonData9) as PushButton;

            pushButton9.ToolTip = nameof(YMS.TEST9);
            /////////////////////////////////////////////////////////////////////////////
            ///
            PushButtonData pushButtonData10 = new PushButtonData("PushButton Command10", "TEST10",
                          dllPath, "YMS.TEST10");


            PushButton pushButton10 = panel.AddItem(pushButtonData10) as PushButton;

            pushButton10.ToolTip = "TEST10";
            /////////////////////////////////////////////////////////////////////////////
            ///
            PushButtonData pushButtonData11 = new PushButtonData("PushButton Command11", "TEST11",
                          dllPath, "YMS.TEST11");


            PushButton pushButton11= panel.AddItem(pushButtonData11) as PushButton;

            pushButton11.ToolTip = "TEST11";
            /////////////////////////////////////////////////////////////////////////////
            ///
            PushButtonData pushButtonData12 = new PushButtonData("PushButton Command12", "TEST12",
                          dllPath, "YMS.TEST12");


            PushButton pushButton12 = panel.AddItem(pushButtonData12) as PushButton;

            pushButton12.ToolTip = "TEST12";
            /////////////////////////////////////////////////////////////////////////////
            ///
            PushButtonData pushButtonData13 = new PushButtonData("PushButton Command13", "TEST13",
                          dllPath, "YMS.TEST13");


            PushButton pushButton13 = panel.AddItem(pushButtonData13) as PushButton;

            pushButton13.ToolTip = "TEST13";
            /////////////////////////////////////////////////////////////////////////////
            ///
            PushButtonData pushButtonData14 = new PushButtonData("PushButton Command14", "TEST14",
                          dllPath, "YMS.TEST14");


            PushButton pushButton14 = panel.AddItem(pushButtonData14) as PushButton;

            pushButton14.ToolTip = "TEST14";
            /////////////////////////////////////////////////////////////////////////////
            ///
            {
                var buttonData = new PushButtonData("斜梁B", "斜梁B", dllPath, typeof(YMS.TestShabariBase).FullName);
                var button = panel.AddItem(buttonData) as PushButton;
                button.ToolTip = nameof(YMS.TestShabariBase);
            }
            {
                var buttonData = new PushButtonData("斜梁火打", "斜梁火打", dllPath, typeof(YMS.TestShabariHiuchi).FullName);
                var button = panel.AddItem(buttonData) as PushButton;
                button.ToolTip = nameof(YMS.TestShabariHiuchi);
            }
            /////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData15 = new PushButtonData("個別配置TEST", "個別配置TEST",
                          dllPath, "YMS.TEST15");


            PushButton pushButton15 = panel.AddItem(pushButtonData15) as PushButton;

            pushButton8.ToolTip = "TEST15";
            /////////////////////////////////////////////////////////////////////////////

        }
        public void AddRibbonMenu_Setting(UIControlledApplication app)
        {
            string dllPath = ClsSystemData.GetDLLPath();

            // タブの追加
            app.CreateRibbonTab("YMS設定");

            // パネルの追加
            RibbonPanel panelSyokisettei = app.CreateRibbonPanel("YMS設定", "初期設定・情報確認");

            // ボタンの追加
            PushButtonData pushButtonData1 = new PushButtonData("PushButton Command1", "図面\n情報", dllPath, "YMS.CommandZumenInfo");
            PushButton pushButton1 = panelSyokisettei.AddItem(pushButtonData1) as PushButton;
            pushButton1.LongDescription = "図面情報を設定します";

            if (System.IO.File.Exists(GetIconPath(dllPath, "1 図面情報")))
            {
                pushButton1.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "1 図面情報")));
                pushButton1.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "1 図面情報")));
                pushButton1.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "1 図面情報")));
            }

            //#33548
            PushButtonData pushButtonData2 = new PushButtonData("PushButton Command200", "Light\n連携", dllPath, "YMS.CommandReadJson");
            PushButton pushButton2 = panelSyokisettei.AddItem(pushButtonData2) as PushButton;
            pushButton2.LongDescription = "YMS Lightから出力されたJSONファイルを読み込みます";

            if (System.IO.File.Exists(GetIconPath(dllPath, "Light連携")))
            {
                pushButton2.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "Light連携")));
                pushButton2.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "Light連携")));
                pushButton2.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "Light連携")));
            }

        }

        public void AddRibbonMenu_Yamadome(UIControlledApplication app)
        {
            string dllPath = ClsSystemData.GetDLLPath();

            app.CreateRibbonTab("YMS山留");

            //RibbonPanel panelworkset = app.CreateRibbonPanel("YMS", "ワークセット");
            RibbonPanel panelKabeShin = app.CreateRibbonPanel("YMS山留", "壁芯");
            RibbonPanel panelYamdomeKabe = app.CreateRibbonPanel("YMS山留", "山留壁");
            RibbonPanel panelKui = app.CreateRibbonPanel("YMS山留", "杭");
            RibbonPanel panelBase = app.CreateRibbonPanel("YMS山留", "ベース");
            //RibbonPanel panelJack = app.CreateRibbonPanel("YMS山留", "ジャッキ");
            //RibbonPanel panelAtamaTsunagi = app.CreateRibbonPanel("YMS山留", "頭ツナギ");
            //RibbonPanel panelDesumibuHokyouzai = app.CreateRibbonPanel("YMS山留", "出隅部補強材");
            //RibbonPanel panelKariKouzai = app.CreateRibbonPanel("YMS山留", "仮鋼材");
            //RibbonPanel panelSumibuPiace = app.CreateRibbonPanel("YMS山留", "隅部ピース");
            //RibbonPanel panelBracket = app.CreateRibbonPanel("YMS山留", "ブラケット");
            //RibbonPanel panelSendanBoltHokyouzai = app.CreateRibbonPanel("YMS山留", "せん断ボルト補強材");
            //RibbonPanel panelHaraokoshiSuberidome = app.CreateRibbonPanel("YMS山留", "腹起スベリ止め");
            //RibbonPanel panelKousyabuMawari = app.CreateRibbonPanel("YMS山留", "交叉部周り");
            //RibbonPanel panelStiffener = app.CreateRibbonPanel("YMS山留", "スチフナー");
            //RibbonPanel panelWaritukeSyudou = app.CreateRibbonPanel("YMS山留", "手動割付");
            //RibbonPanel panelLengthKouzai = app.CreateRibbonPanel("YMS山留", "鋼材長さ");
            //RibbonPanel panelHaraokoshiBracket = app.CreateRibbonPanel("YMS山留", "腹起ブラケット");
            RibbonPanel panelBuhinHaici = app.CreateRibbonPanel("YMS山留", "各部品配置");


            PulldownButtonData groupKouyaitaData = new PulldownButtonData("鋼矢板", "鋼矢板");
            PulldownButtonData groupOyakuiData = new PulldownButtonData("親杭", "親杭");
            PulldownButtonData groupYokoyaitaData = new PulldownButtonData("横矢板", "横矢板");
            PulldownButtonData groupRenzokuKabeData = new PulldownButtonData("連続壁", "連続壁");
            PulldownButtonData groupSMWData = new PulldownButtonData("SMW", "SMW");

            PulldownButtonData groupKuiData = new PulldownButtonData("TC杭・構台杭・兼用杭・断面変化杭", "TC杭・\n構台杭・\n兼用杭・\n断面変化杭");
            PulldownButtonData groupTanakuiData = new PulldownButtonData("棚杭・中間杭", "棚杭・\n中間杭\n\n");

            PulldownButtonData groupHaraokoshiBaseData = new PulldownButtonData("腹起ベース", "腹起");
            PulldownButtonData groupKiribariBaseData = new PulldownButtonData("切梁ベース", "切梁");
            PulldownButtonData groupSanjikuBaseData = new PulldownButtonData("三軸ピース", "三軸");
            PulldownButtonData groupSumibuBaseData = new PulldownButtonData("隅火打ベース", "隅火打");
            PulldownButtonData groupKiribariHiuchiBaseData = new PulldownButtonData("切梁火打ベース", "切梁火打");
            PulldownButtonData groupKiribariUkeBaseData = new PulldownButtonData("切梁受けベース", "切梁受け");
            PulldownButtonData groupKiribariTsunagizaiBaseData = new PulldownButtonData("切梁ツナギ材ベース", "切梁ツナギ材");
            PulldownButtonData groupHiuchiTsunagizaiBaseData = new PulldownButtonData("火打ツナギ材ベース", "火打ツナギ材");
            PulldownButtonData groupKiribariTsunagiBaseData = new PulldownButtonData("切梁継ぎベース", "切梁継ぎ");
            PulldownButtonData groupSyabariBaseData = new PulldownButtonData("斜梁ベース", "斜梁");
            PulldownButtonData groupSyabariTsunagizaiBaseData = new PulldownButtonData("斜梁ツナギ材ベース", "斜梁ツナギ");
            PulldownButtonData groupSyabariUkeBaseData = new PulldownButtonData("斜梁受けベース", "斜梁受け");
            PulldownButtonData groupSyabariHiuchiBaseData = new PulldownButtonData("斜梁火打ベース", "斜梁火打");



            PulldownButtonData groupJackData = new PulldownButtonData("ジャッキ", "ジャッキ");
            if (System.IO.File.Exists(GetIconPath(dllPath, "16 ジャッキ-作成")))
            {
                groupJackData.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "16 ジャッキ-作成")));
                groupJackData.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "16 ジャッキ-作成")));
                groupJackData.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "16 ジャッキ-作成")));
            }
            PulldownButtonData groupAtamaTsunagiData = new PulldownButtonData("頭ツナギ", "頭ツナギ");
            if (System.IO.File.Exists(GetIconPath(dllPath, "17 頭ツナギ-作成")))
            {
                groupAtamaTsunagiData.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "17 頭ツナギ-作成")));
                groupAtamaTsunagiData.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "17 頭ツナギ-作成")));
                groupAtamaTsunagiData.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "17 頭ツナギ-作成")));
            }
            PulldownButtonData groupDesumibuHokyouzaiData = new PulldownButtonData("出隅部補強材", "出隅部補強材");
            if (System.IO.File.Exists(GetIconPath(dllPath, "17 出隅-作成")))
            {
                groupDesumibuHokyouzaiData.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "17 出隅-作成")));
                groupDesumibuHokyouzaiData.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "17 出隅-作成")));
                groupDesumibuHokyouzaiData.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "17 出隅-作成")));
            }
            PulldownButtonData groupSumibuPiaceData = new PulldownButtonData("隅部ピース", "隅部ピース");
            if (System.IO.File.Exists(GetIconPath(dllPath, "19 隅部-作成")))
            {
                groupSumibuPiaceData.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "19 隅部-作成")));
                groupSumibuPiaceData.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "19 隅部-作成")));
                groupSumibuPiaceData.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "19 隅部-作成")));
            }
            PulldownButtonData groupSendanBoltHokyouzaiData = new PulldownButtonData("せん断ボルト補強材", "せん断ボルト補強材");
            if (System.IO.File.Exists(GetIconPath(dllPath, "21 ボルト-作成")))
            {
                groupSendanBoltHokyouzaiData.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "21 ボルト-作成")));
                groupSendanBoltHokyouzaiData.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "21 ボルト-作成")));
                groupSendanBoltHokyouzaiData.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "21 ボルト-作成")));
            }
            PulldownButtonData groupHaraokoshiSuberidomeData = new PulldownButtonData("腹起スベリ止め", "腹起スベリ止め");
            if (System.IO.File.Exists(GetIconPath(dllPath, "23　腹起スベリ-作成")))
            {
                groupHaraokoshiSuberidomeData.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "23　腹起スベリ-作成")));
                groupHaraokoshiSuberidomeData.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "23　腹起スベリ-作成")));
                groupHaraokoshiSuberidomeData.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "23　腹起スベリ-作成")));
            }
            IList<RibbonItem> addedButtons = panelYamdomeKabe.AddStackedItems(groupKouyaitaData, groupOyakuiData, groupYokoyaitaData);
            List<PulldownButton> pullBtnList = new List<PulldownButton>();
            foreach (RibbonItem item in addedButtons)
            {
                pullBtnList.Add(item as PulldownButton);
            }
            PulldownButton groupYamadomeKouyaita = pullBtnList[0];
            groupYamadomeKouyaita.ToolTip = "鋼矢板";
            if (System.IO.File.Exists(GetIconPath(dllPath, "5 鋼矢板-作成 - 16")))
            {
                groupYamadomeKouyaita.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "5 鋼矢板-作成 - 16")));
                groupYamadomeKouyaita.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "5 鋼矢板-作成 - 16")));
                groupYamadomeKouyaita.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "5 鋼矢板-作成 - 16")));
            }

            PulldownButton groupYamadomeOyakui = pullBtnList[1];
            groupYamadomeOyakui.ToolTip = "親杭";
            if (System.IO.File.Exists(GetIconPath(dllPath, "6 親杭-作成 - 16")))
            {
                groupYamadomeOyakui.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "6 親杭-作成 - 16")));
                groupYamadomeOyakui.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "6 親杭-作成 - 16")));
                groupYamadomeOyakui.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "6 親杭-作成 - 16")));
            }
            PulldownButton groupYamadomeYokoyaita = pullBtnList[2];
            groupYamadomeYokoyaita.ToolTip = "横矢板";
            if (System.IO.File.Exists(GetIconPath(dllPath, "7 横矢板-作成 - 16")))
            {
                groupYamadomeYokoyaita.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "7 横矢板-作成 - 16")));
                groupYamadomeYokoyaita.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "7 横矢板-作成 - 16")));
                groupYamadomeYokoyaita.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "7 横矢板-作成 - 16")));
            }
            addedButtons = panelYamdomeKabe.AddStackedItems(groupRenzokuKabeData, groupSMWData);
            pullBtnList = new List<PulldownButton>();
            foreach (RibbonItem item in addedButtons)
            {
                pullBtnList.Add(item as PulldownButton);
            }
            PulldownButton groupYamadomeRenzokuKabe = pullBtnList[0];
            groupYamadomeRenzokuKabe.ToolTip = "連続壁";
            if (System.IO.File.Exists(GetIconPath(dllPath, "8 連続壁-作成 - 16")))
            {
                groupYamadomeRenzokuKabe.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "8 連続壁-作成 - 16")));
                groupYamadomeRenzokuKabe.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "8 連続壁-作成 - 16")));
                groupYamadomeRenzokuKabe.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "8 連続壁-作成 - 16")));
            }
            PulldownButton groupYamadomeSMW = pullBtnList[1];
            groupYamadomeSMW.ToolTip = "SMW";
            if (System.IO.File.Exists(GetIconPath(dllPath, "9 SMW-作成 - 16")))
            {
                groupYamadomeSMW.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "9 SMW-作成 - 16")));
                groupYamadomeSMW.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "9 SMW-作成 - 16")));
                groupYamadomeSMW.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "9 SMW-作成 - 16")));
            }

            PulldownButton groupKui = panelKui.AddItem(groupKuiData) as PulldownButton;
            PulldownButton groupTanaKui = panelKui.AddItem(groupTanakuiData) as PulldownButton;

            addedButtons = panelBase.AddStackedItems(groupHaraokoshiBaseData, groupKiribariBaseData, groupSanjikuBaseData);
            pullBtnList = new List<PulldownButton>();
            foreach (RibbonItem item in addedButtons)
            {
                pullBtnList.Add(item as PulldownButton);
            }
            PulldownButton groupBaseHaraokoshi = pullBtnList[0];
            groupBaseHaraokoshi.ToolTip = "腹起ベース";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 腹起")))
            {
                groupBaseHaraokoshi.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 腹起")));
                groupBaseHaraokoshi.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 腹起")));
                groupBaseHaraokoshi.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 腹起")));
            }
            PulldownButton groupBaseKiribari = pullBtnList[1];
            groupBaseKiribari.ToolTip = "切梁ベース";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 切梁")))
            {
                groupBaseKiribari.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁")));
                groupBaseKiribari.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁")));
                groupBaseKiribari.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁")));
            }
            PulldownButton groupBaseSanjiku = pullBtnList[2];
            groupBaseSanjiku.ToolTip = "三軸ピース";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 三軸")))
            {
                groupBaseSanjiku.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 三軸")));
                groupBaseSanjiku.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 三軸")));
                groupBaseSanjiku.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 三軸")));
            }
            addedButtons = panelBase.AddStackedItems(groupSumibuBaseData, groupKiribariHiuchiBaseData, groupKiribariUkeBaseData);
            pullBtnList = new List<PulldownButton>();
            foreach (RibbonItem item in addedButtons)
            {
                pullBtnList.Add(item as PulldownButton);
            }
            PulldownButton groupBasesumibu = pullBtnList[0];
            groupBasesumibu.ToolTip = "隅火打ベース";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 隅火打")))
            {
                groupBasesumibu.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 隅火打")));
                groupBasesumibu.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 隅火打")));
                groupBasesumibu.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 隅火打")));
            }
            PulldownButton groupBaseKiribarHiuchi = pullBtnList[1];
            groupBaseKiribarHiuchi.ToolTip = "切梁火打ベース";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 切梁火打")))
            {
                groupBaseKiribarHiuchi.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁火打")));
                groupBaseKiribarHiuchi.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁火打")));
                groupBaseKiribarHiuchi.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁火打")));
            }
            PulldownButton groupBaseKiribariUke = pullBtnList[2];
            groupBaseKiribariUke.ToolTip = "切梁受けベース";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 切梁受け")))
            {
                groupBaseKiribariUke.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁受け")));
                groupBaseKiribariUke.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁受け")));
                groupBaseKiribariUke.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁受け")));
            }
            addedButtons = panelBase.AddStackedItems(groupKiribariTsunagizaiBaseData, groupHiuchiTsunagizaiBaseData, groupKiribariTsunagiBaseData);
            pullBtnList = new List<PulldownButton>();
            foreach (RibbonItem item in addedButtons)
            {
                pullBtnList.Add(item as PulldownButton);
            }
            PulldownButton groupBaseKiribariTsunagizai = pullBtnList[0];
            groupBaseKiribariTsunagizai.ToolTip = "切梁ツナギ材ベース";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 切梁ツナギ")))
            {
                groupBaseKiribariTsunagizai.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁ツナギ")));
                groupBaseKiribariTsunagizai.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁ツナギ")));
                groupBaseKiribariTsunagizai.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁ツナギ")));
            }
            PulldownButton groupBaseHiuchiTsunagizai = pullBtnList[1];
            groupBaseHiuchiTsunagizai.ToolTip = "火打ツナギ材ベース";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 火打ツナギ")))
            {
                groupBaseHiuchiTsunagizai.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 火打ツナギ")));
                groupBaseHiuchiTsunagizai.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 火打ツナギ")));
                groupBaseHiuchiTsunagizai.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 火打ツナギ")));
            }
            PulldownButton groupBaseKiribariTsunagi = pullBtnList[2];
            groupBaseKiribariTsunagi.ToolTip = "切梁継ぎベース";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 切梁継ぎ")))
            {
                groupBaseKiribariTsunagi.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁継ぎ")));
                groupBaseKiribariTsunagi.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁継ぎ")));
                groupBaseKiribariTsunagi.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁継ぎ")));
            }

            addedButtons = panelBase.AddStackedItems(groupSyabariBaseData, groupSyabariTsunagizaiBaseData, groupSyabariUkeBaseData);
            pullBtnList = new List<PulldownButton>();
            foreach (RibbonItem item in addedButtons)
            {
                pullBtnList.Add(item as PulldownButton);
            }
            PulldownButton groupBaseSyabari = pullBtnList[0];
            groupBaseSyabari.ToolTip = "斜梁ベース";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 斜梁")))
            {
                groupBaseSyabari.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁")));
                groupBaseSyabari.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁")));
                groupBaseSyabari.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁")));
            }
            PulldownButton groupBaseSyabariTsunagizai = pullBtnList[1];
            groupBaseSyabariTsunagizai.ToolTip = "斜梁ツナギ材ベース";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 斜梁ツナギ材")))
            {
                groupBaseSyabariTsunagizai.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁ツナギ材")));
                groupBaseSyabariTsunagizai.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁ツナギ材")));
                groupBaseSyabariTsunagizai.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁ツナギ材")));
            }
            PulldownButton groupBaseSyabariUke = pullBtnList[2];
            groupBaseSyabariUke.ToolTip = "斜梁受けベース";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 斜梁受け")))
            {
                groupBaseSyabariUke.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁受け")));
                groupBaseSyabariUke.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁受け")));
                groupBaseSyabariUke.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁受け")));
            }

			//RibbonItem addedButton = panelBase.AddItem(groupSyabariHiuchiBaseData);
   //         pullBtnList = new List<PulldownButton>();
   //         pullBtnList.Add(addedButton as PulldownButton);
   //         PulldownButton groupBaseSyabariHiuchi = pullBtnList[0];
   //         groupBaseSyabariHiuchi.ToolTip = "斜梁火打ベース";
   //         if (System.IO.File.Exists(GetIconPath(dllPath, "14 斜梁火打")))
   //         {
   //             groupBaseSyabariHiuchi.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁火打")));
   //             groupBaseSyabariHiuchi.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁火打")));
   //             groupBaseSyabariHiuchi.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁火打")));
   //         }

            //パネルを統合しない場合
            //PulldownButton groupJack = panelJack.AddItem(groupJackData) as PulldownButton;
            //PulldownButton groupAtamaTsunagi = panelAtamaTsunagi.AddItem(groupAtamaTsunagiData) as PulldownButton;
            //PulldownButton groupDesumibuHokyouzai = panelDesumibuHokyouzai.AddItem(groupDesumibuHokyouzaiData) as PulldownButton;
            //PulldownButton groupSumibuPiace = panelSumibuPiace.AddItem(groupSumibuPiaceData) as PulldownButton;
            //PulldownButton groupSendanBoltHokyouzai = panelSendanBoltHokyouzai.AddItem(groupSendanBoltHokyouzaiData) as PulldownButton;
            //PulldownButton groupHaraokoshiSuberidome = panelHaraokoshiSuberidome.AddItem(groupHaraokoshiSuberidomeData) as PulldownButton;

            //パネルを統合する場合
            PulldownButton groupJack = panelBuhinHaici.AddItem(groupJackData) as PulldownButton;
            PulldownButton groupAtamaTsunagi = panelBuhinHaici.AddItem(groupAtamaTsunagiData) as PulldownButton;
            PulldownButton groupDesumibuHokyouzai = panelBuhinHaici.AddItem(groupDesumibuHokyouzaiData) as PulldownButton;
            PulldownButton groupSumibuPiace = panelBuhinHaici.AddItem(groupSumibuPiaceData) as PulldownButton;
            PulldownButton groupSendanBoltHokyouzai = panelBuhinHaici.AddItem(groupSendanBoltHokyouzaiData) as PulldownButton;
            PulldownButton groupHaraokoshiSuberidome = panelBuhinHaici.AddItem(groupHaraokoshiSuberidomeData) as PulldownButton;

            ///////////////////////////////////////////////////////////////////////////




            ///////////////////////////////////////////////////////////////////////////
            //PushButtonData pushButtonData2 = new PushButtonData("PushButton Command2", "識別\n記号",
            //                dllPath, "YMS.ChangeShikibetuSym");

            //PushButton pushButton2 = panelSyokisettei.AddItem(pushButtonData2) as PushButton;

            //pushButton2.ToolTip = "識別記号を変更します。";
            //if (System.IO.File.Exists(GetIconPath(dllPath, "2 識別記号")))
            //{
            //    pushButton2.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "2 識別記号")));
            //    pushButton2.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "2 識別記号")));
            //    pushButton2.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "2 識別記号")));
            //}
            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData3 = new PushButtonData("PushButton Command3", "壁芯\n",
                            dllPath, "YMS.CreateKabeshin");

            PushButton pushButton3 = panelKabeShin.AddItem(pushButtonData3) as PushButton;

            pushButton3.ToolTip = "壁芯を作成します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "4 壁芯")))
            {
                pushButton3.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "4 壁芯")));
                pushButton3.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "4 壁芯")));
                pushButton3.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "4 壁芯")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData4 = new PushButtonData("PushButton Command4", "作成",
                            dllPath, "YMS.CreateKouyaita");

            PushButton pushButton4 = groupYamadomeKouyaita.AddPushButton(pushButtonData4) as PushButton;

            pushButton4.ToolTip = "鋼矢板を作成します。";

            if (System.IO.File.Exists(GetIconPath(dllPath, "5 鋼矢板-作成")))
            {
                pushButton4.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "5 鋼矢板-作成")));
                pushButton4.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "5 鋼矢板-作成")));
                pushButton4.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "5 鋼矢板-作成")));
            }

            ///////////////////////////////////////////////////////////////////////////
            //PushButtonData pushButtonData5 = new PushButtonData("PushButton Command5", "変更",
            //                dllPath, "YMS.ChangeKouyaita");

            //PushButton pushButton5 = groupYamadomeKouyaita.AddPushButton(pushButtonData5) as PushButton;

            //pushButton5.ToolTip = "鋼矢板を変更します。";

            //if (System.IO.File.Exists(GetIconPath(dllPath, "5 鋼矢板-変更")))
            //{
            //    pushButton5.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "5 鋼矢板-変更")));
            //    pushButton5.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "5 鋼矢板-変更")));
            //    pushButton5.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "5 鋼矢板-変更")));
            //}

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData6 = new PushButtonData("PushButton Command6", "反転",
                            dllPath, "YMS.ReverseKouyaita");

            PushButton pushButton6 = groupYamadomeKouyaita.AddPushButton(pushButtonData6) as PushButton;

            pushButton6.ToolTip = "鋼矢板を反転します。";

            if (System.IO.File.Exists(GetIconPath(dllPath, "5 鋼矢板-反転")))
            {
                pushButton6.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "5 鋼矢板-反転")));
                pushButton6.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "5 鋼矢板-反転")));
                pushButton6.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "5 鋼矢板-反転")));
            }

            /////////////////////////////////////////////////////////////////////////////
            //PushButtonData pushButtonData7 = new PushButtonData("PushButton Command7", "特殊Tコーナー変換",
            //                dllPath, "YMS.MergeKoyaita");

            //PushButton pushButton7 = groupYamadomeKouyaita.AddPushButton(pushButtonData7) as PushButton;

            //pushButton7.ToolTip = "特殊Tコーナー変換を行います。";

            /////////////////////////////////////////////////////////////////////////////
            //groupYamadome.AddSeparator();//区切りを付ける
            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData8 = new PushButtonData("PushButton Command8", "作成",
                            dllPath, "YMS.CreateOyakui");

            PushButton pushButton8 = groupYamadomeOyakui.AddPushButton(pushButtonData8) as PushButton;

            pushButton8.ToolTip = "親杭を作成します。";

            if (System.IO.File.Exists(GetIconPath(dllPath, "6 親杭-作成")))
            {
                pushButton8.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "6 親杭-作成")));
                pushButton8.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "6 親杭-作成")));
                pushButton8.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "6 親杭-作成")));
            }

            ///////////////////////////////////////////////////////////////////////////
            //PushButtonData pushButtonData9 = new PushButtonData("PushButton Command9", "変更",
            //                dllPath, "YMS.ChangeOyakui");

            //PushButton pushButton9 = groupYamadomeOyakui.AddPushButton(pushButtonData9) as PushButton;

            //pushButton9.ToolTip = "親杭を変更します。";

            //if (System.IO.File.Exists(GetIconPath(dllPath, "6 親杭-変更")))
            //{
            //    pushButton9.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "6 親杭-変更")));
            //    pushButton9.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "6 親杭-変更")));
            //    pushButton9.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "6 親杭-変更")));
            //}

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData10 = new PushButtonData("PushButton Command10", "作成",
                            dllPath, "YMS.CreateYokoyaita");

            PushButton pushButton10 = groupYamadomeYokoyaita.AddPushButton(pushButtonData10) as PushButton;

            pushButton10.ToolTip = "横矢板を作成します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "7 横矢板-作成")))
            {
                pushButton10.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "7 横矢板-作成")));
                pushButton10.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "7 横矢板-作成")));
                pushButton10.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "7 横矢板-作成")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData11 = new PushButtonData("PushButton Command11", "変更",
                            dllPath, "YMS.ChangeYokoyaita");

            PushButton pushButton11 = groupYamadomeYokoyaita.AddPushButton(pushButtonData11) as PushButton;

            pushButton11.ToolTip = "横矢板を変更します。";

            if (System.IO.File.Exists(GetIconPath(dllPath, "7 横矢板-変更")))
            {
                pushButton11.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "7 横矢板-変更")));
                pushButton11.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "7 横矢板-変更")));
                pushButton11.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "7 横矢板-変更")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData12 = new PushButtonData("PushButton Command12", "作成",
                            dllPath, "YMS.CreateRenzokuKabe");

            PushButton pushButton12 = groupYamadomeRenzokuKabe.AddPushButton(pushButtonData12) as PushButton;

            pushButton12.ToolTip = "連続壁を作成します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "8 連続壁-作成")))
            {
                pushButton12.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "8 連続壁-作成")));
                pushButton12.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "8 連続壁-作成")));
                pushButton12.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "8 連続壁-作成")));
            }

            ///////////////////////////////////////////////////////////////////////////
            //PushButtonData pushButtonData13 = new PushButtonData("PushButton Command13", "変更",
            //                dllPath, "YMS.ChangeRenzokuKabe");

            //PushButton pushButton13 = groupYamadomeRenzokuKabe.AddPushButton(pushButtonData13) as PushButton;

            //pushButton13.ToolTip = "連続壁を変更します。";
            //if (System.IO.File.Exists(GetIconPath(dllPath, "8 連続壁-変更")))
            //{
            //    pushButton13.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "8 連続壁-変更")));
            //    pushButton13.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "8 連続壁-変更")));
            //    pushButton13.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "8 連続壁-変更")));
            //}

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData14 = new PushButtonData("PushButton Command14", "作成",
                            dllPath, "YMS.CreateSMW");

            PushButton pushButton14 = groupYamadomeSMW.AddPushButton(pushButtonData14) as PushButton;

            pushButton14.ToolTip = "SMWを作成します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "9 SMW-作成")))
            {
                pushButton14.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "9 SMW-作成")));
                pushButton14.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "9 SMW-作成")));
                pushButton14.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "9 SMW-作成")));
            }

            ///////////////////////////////////////////////////////////////////////////
            //PushButtonData pushButtonData15 = new PushButtonData("PushButton Command15", "変更",
            //                dllPath, "YMS.ChangeSMW");

            //PushButton pushButton15 = groupYamadomeSMW.AddPushButton(pushButtonData15) as PushButton;

            //pushButton15.ToolTip = "SMWを変更します。";
            //if (System.IO.File.Exists(GetIconPath(dllPath, "9 SMW-変更")))
            //{
            //    pushButton15.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "9 SMW-変更")));
            //    pushButton15.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "9 SMW-変更")));
            //    pushButton15.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "9 SMW-変更")));
            //}

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData16 = new PushButtonData("PushButton Command16", "ボイド\n",
                            dllPath, "YMS.CreateVoidFamily");

            PushButton pushButton16 = panelYamdomeKabe.AddItem(pushButtonData16) as PushButton;

            pushButton16.ToolTip = "壁をくりぬきます。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "10 ボイド-作成")))
            {
                pushButton16.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "10 ボイド-作成")));
                pushButton16.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "10 ボイド-作成")));
                pushButton16.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "10 ボイド-作成")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData17 = new PushButtonData("PushButton Command17", "CASE\n管理",
                            dllPath, "YMS.ManegeCASE");

            PushButton pushButton17 = panelYamdomeKabe.AddItem(pushButtonData17) as PushButton;
            pushButton17.ToolTip = "CASE管理。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "11 CASE-管理")))
            {
                pushButton17.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "11 CASE-管理")));
                pushButton17.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "11 CASE-管理")));
                pushButton17.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "11 CASE-管理")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData18 = new PushButtonData("PushButton Command18", "作成",
                            dllPath, "YMS.CreateSanbashiKui");

            PushButton pushButton18 = groupKui.AddPushButton(pushButtonData18) as PushButton;

            pushButton18.ToolTip = "TC杭・構台杭・兼用杭・断面変化杭を作成します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "12 杭-作成")))
            {
                pushButton18.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "12 杭-作成")));
                pushButton18.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "12 杭-作成")));
                pushButton18.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "12 杭-作成")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData19 = new PushButtonData("PushButton Command19", "変更",
                            dllPath, "YMS.ChangeKui");

            PushButton pushButton19 = groupKui.AddPushButton(pushButtonData19) as PushButton;

            pushButton19.ToolTip = "TC杭・構台杭・兼用杭・断面変化杭を変更します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "12 杭-変更")))
            {
                pushButton19.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "12 杭-変更")));
                pushButton19.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "12 杭-変更")));
                pushButton19.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "12 杭-変更")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData20 = new PushButtonData("PushButton Command20", "作成",
                            dllPath, "YMS.CreateTanakui");

            PushButton pushButton20 = groupTanaKui.AddPushButton(pushButtonData20) as PushButton;

            pushButton20.ToolTip = "棚杭・中間杭を作成します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "13 中杭-作成")))
            {
                pushButton20.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "13 中杭-作成")));
                pushButton20.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "13 中杭-作成")));
                pushButton20.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "13 中杭-作成")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData21 = new PushButtonData("PushButton Command21", "変更",
                            dllPath, "YMS.ChangeTanakui");

            PushButton pushButton21 = groupTanaKui.AddPushButton(pushButtonData21) as PushButton;

            pushButton21.ToolTip = "棚杭・中間杭を変更します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "13 中杭-変更")))
            {
                pushButton21.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "13 中杭-変更")));
                pushButton21.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "13 中杭-変更")));
                pushButton21.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "13 中杭-変更")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData22 = new PushButtonData("PushButton Command22", "作成",
                            dllPath, "YMS.CreateHaraokoshiBase");

            PushButton pushButton22 = groupBaseHaraokoshi.AddPushButton(pushButtonData22) as PushButton;

            pushButton22.ToolTip = "腹起ベースを作成します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 腹起")))
            {
                pushButton22.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 腹起")));
                pushButton22.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 腹起")));
                pushButton22.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 腹起")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData23 = new PushButtonData("PushButton Command23", "変更",
                            dllPath, "YMS.ChangeHaraokoshiBase");

            PushButton pushButton23 = groupBaseHaraokoshi.AddPushButton(pushButtonData23) as PushButton;

            pushButton23.ToolTip = "腹起ベースを変更します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 腹起")))
            {
                pushButton23.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 腹起")));
                pushButton23.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 腹起")));
                pushButton23.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 腹起")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData24 = new PushButtonData("PushButton Command24", "作成",
                            dllPath, "YMS.CreateKiribariBase");

            PushButton pushButton24 = groupBaseKiribari.AddPushButton(pushButtonData24) as PushButton;

            pushButton24.ToolTip = "切梁ベースを作成します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 切梁")))
            {
                pushButton24.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁")));
                pushButton24.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁")));
                pushButton24.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData25 = new PushButtonData("PushButton Command25", "変更",
                            dllPath, "YMS.ChangeKiribariBase");

            PushButton pushButton25 = groupBaseKiribari.AddPushButton(pushButtonData25) as PushButton;

            pushButton25.ToolTip = "切梁ベースを変更します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 切梁")))
            {
                pushButton25.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁")));
                pushButton25.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁")));
                pushButton25.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData26 = new PushButtonData("PushButton Command26", "配置",
                            dllPath, "YMS.PutSanjikuPiece");

            PushButton pushButton26 = groupBaseSanjiku.AddPushButton(pushButtonData26) as PushButton;

            pushButton26.ToolTip = "三軸ピースを配置します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 三軸")))
            {
                pushButton26.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 三軸")));
                pushButton26.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 三軸")));
                pushButton26.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 三軸")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData27 = new PushButtonData("PushButton Command27", "位置調整",
                            dllPath, "YMS.PositionSanjikuPiece");

            PushButton pushButton27 = groupBaseSanjiku.AddPushButton(pushButtonData27) as PushButton;

            pushButton27.ToolTip = "三軸ピースの位置を調整します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 三軸")))
            {
                pushButton27.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 三軸")));
                pushButton27.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 三軸")));
                pushButton27.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 三軸")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData28 = new PushButtonData("PushButton Command28", "作成",
                            dllPath, "YMS.CreateCornerHiuchiBase");

            PushButton pushButton28 = groupBasesumibu.AddPushButton(pushButtonData28) as PushButton;

            pushButton28.ToolTip = "隅火打ベースを作成します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 隅火打")))
            {
                pushButton28.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 隅火打")));
                pushButton28.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 隅火打")));
                pushButton28.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 隅火打")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData29 = new PushButtonData("PushButton Command29", "変更",
                            dllPath, "YMS.ChangeCornerHiuchiBase");

            PushButton pushButton29 = groupBasesumibu.AddPushButton(pushButtonData29) as PushButton;

            pushButton29.ToolTip = "隅火打ベースを変更します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 隅火打")))
            {
                pushButton29.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 隅火打")));
                pushButton29.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 隅火打")));
                pushButton29.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 隅火打")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData30 = new PushButtonData("PushButton Command30", "作成",
                            dllPath, "YMS.CreateKiribariHiuchiBase");

            PushButton pushButton30 = groupBaseKiribarHiuchi.AddPushButton(pushButtonData30) as PushButton;

            pushButton30.ToolTip = "切梁火打ベースを作成します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 切梁火打")))
            {
                pushButton30.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁火打")));
                pushButton30.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁火打")));
                pushButton30.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁火打")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData31 = new PushButtonData("PushButton Command31", "変更",
                            dllPath, "YMS.ChangeKiribariHiuchiBase");

            PushButton pushButton31 = groupBaseKiribarHiuchi.AddPushButton(pushButtonData31) as PushButton;

            pushButton31.ToolTip = "切梁火打ベースを変更します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 切梁火打")))
            {
                pushButton31.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁火打")));
                pushButton31.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁火打")));
                pushButton31.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁火打")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData32 = new PushButtonData("PushButton Command32", "作成",
                            dllPath, "YMS.CreateKiribariUkeBase");

            PushButton pushButton32 = groupBaseKiribariUke.AddPushButton(pushButtonData32) as PushButton;

            pushButton32.ToolTip = "切梁受けベースを作成します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 切梁受け")))
            {
                pushButton32.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁受け")));
                pushButton32.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁受け")));
                pushButton32.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁受け")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData33 = new PushButtonData("PushButton Command33", "変更",
                            dllPath, "YMS.ChangeKiribariUkeBase");

            PushButton pushButton33 = groupBaseKiribariUke.AddPushButton(pushButtonData33) as PushButton;

            pushButton33.ToolTip = "切梁受けベースを変更します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 切梁受け")))
            {
                pushButton33.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁受け")));
                pushButton33.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁受け")));
                pushButton33.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁受け")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData34 = new PushButtonData("PushButton Command34", "作成",
                            dllPath, "YMS.CreateKiribariTsunagizaiBase");

            PushButton pushButton34 = groupBaseKiribariTsunagizai.AddPushButton(pushButtonData34) as PushButton;

            pushButton34.ToolTip = "切梁ツナギ材ベースを作成します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 切梁ツナギ")))
            {
                pushButton34.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁ツナギ")));
                pushButton34.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁ツナギ")));
                pushButton34.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁ツナギ")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData35 = new PushButtonData("PushButton Command35", "変更",
                            dllPath, "YMS.ChangeKiribariTsunagizaiBase");

            PushButton pushButton35 = groupBaseKiribariTsunagizai.AddPushButton(pushButtonData35) as PushButton;

            pushButton35.ToolTip = "切梁ツナギ材ベースを変更します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 切梁ツナギ")))
            {
                pushButton35.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁ツナギ")));
                pushButton35.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁ツナギ")));
                pushButton35.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁ツナギ")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData36 = new PushButtonData("PushButton Command36", "作成",
                            dllPath, "YMS.CreateHiuchiTsunagizaiBase");

            PushButton pushButton36 = groupBaseHiuchiTsunagizai.AddPushButton(pushButtonData36) as PushButton;

            pushButton36.ToolTip = "火打ツナギ材ベースを作成します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 火打ツナギ")))
            {
                pushButton36.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 火打ツナギ")));
                pushButton36.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 火打ツナギ")));
                pushButton36.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 火打ツナギ")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData37 = new PushButtonData("PushButton Command37", "変更",
                            dllPath, "YMS.ChangeHIuchiTsunagizaiBase");

            PushButton pushButton37 = groupBaseHiuchiTsunagizai.AddPushButton(pushButtonData37) as PushButton;

            pushButton37.ToolTip = "火打ツナギ材ベースを変更します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 火打ツナギ")))
            {
                pushButton37.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 火打ツナギ")));
                pushButton37.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 火打ツナギ")));
                pushButton37.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 火打ツナギ")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData38 = new PushButtonData("PushButton Command38", "作成",
                            dllPath, "YMS.CreateKiribariTsunagiBase");

            PushButton pushButton38 = groupBaseKiribariTsunagi.AddPushButton(pushButtonData38) as PushButton;

            pushButton38.ToolTip = "切梁継ぎベースを作成します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 切梁継ぎ")))
            {
                pushButton38.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁継ぎ")));
                pushButton38.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁継ぎ")));
                pushButton38.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁継ぎ")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData39 = new PushButtonData("PushButton Command39", "変更",
                            dllPath, "YMS.ChangeKiribariTsunagiBase");

            PushButton pushButton39 = groupBaseKiribariTsunagi.AddPushButton(pushButtonData39) as PushButton;

            pushButton39.ToolTip = "切梁継ぎベースを変更します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 切梁継ぎ")))
            {
                pushButton39.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁継ぎ")));
                pushButton39.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁継ぎ")));
                pushButton39.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 切梁継ぎ")));
            }

            /////////////////////////////////////////////////////////////////////////////
            //PushButtonData pushButtonData82 = new PushButtonData("PushButton Command82", "躯体用線分配置",
            //                dllPath, "YMS.PutSyabariStructureLine");

            //PushButton pushButton82 = groupBaseSyabari.AddPushButton(pushButtonData82) as PushButton;

            //pushButton82.ToolTip = "斜梁の躯体用線分を配置します。";
            //if (System.IO.File.Exists(GetIconPath(dllPath, "14 斜梁")))
            //{
            //    pushButton82.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁")));
            //    pushButton82.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁")));
            //    pushButton82.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁")));
            //}

            /////////////////////////////////////////////////////////////////////////////
            //PushButtonData pushButtonData71 = new PushButtonData("PushButton Command71", "端部部品配置（個別）",
            //                dllPath, "YMS.PutSyabariPieceIndividual");

            //PushButton pushButton71 = groupBaseSyabari.AddPushButton(pushButtonData71) as PushButton;

            //pushButton71.ToolTip = "斜梁の端部部品を個別配置します。";
            //if (System.IO.File.Exists(GetIconPath(dllPath, "14 斜梁")))
            //{
            //    pushButton71.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁")));
            //    pushButton71.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁")));
            //    pushButton71.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁")));
            //}

            /////////////////////////////////////////////////////////////////////////////
            //PushButtonData pushButtonData81 = new PushButtonData("PushButton Command81", "端部部品配置（交点）",
            //                dllPath, "YMS.PutSyabariPieceIntersectionPoint");

            //PushButton pushButton81 = groupBaseSyabari.AddPushButton(pushButtonData81) as PushButton;

            //pushButton81.ToolTip = "腹起ベースとモデル線分の交点に斜梁の端部部品を配置します。";
            //if (System.IO.File.Exists(GetIconPath(dllPath, "14 斜梁")))
            //{
            //    pushButton81.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁")));
            //    pushButton81.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁")));
            //    pushButton81.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁")));
            //}

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData72 = new PushButtonData("PushButton Command72", "斜梁作成",
                            dllPath, "YMS.CreateSyabariBase");

            PushButton pushButton72 = groupBaseSyabari.AddPushButton(pushButtonData72) as PushButton;

            pushButton72.ToolTip = "斜梁ベースを作成します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 斜梁")))
            {
                pushButton72.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁")));
                pushButton72.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁")));
                pushButton72.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData73 = new PushButtonData("PushButton Command73", "斜梁変更",
                            dllPath, "YMS.ChangeSyabariBase");

            PushButton pushButton73 = groupBaseSyabari.AddPushButton(pushButtonData73) as PushButton;

            pushButton73.ToolTip = "斜梁ベースを変更します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 斜梁")))
            {
                pushButton73.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁")));
                pushButton73.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁")));
                pushButton73.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData79 = new PushButtonData("PushButton Command79", "斜梁火打作成",
                            dllPath, "YMS.CreateSyabariHiuchiBase");

            PushButton pushButton79 = groupBaseSyabari.AddPushButton(pushButtonData79) as PushButton;
            //PushButton pushButton79 = groupBaseSyabariHiuchi.AddPushButton(pushButtonData79) as PushButton;

            pushButton79.ToolTip = "斜梁火打ベースを作成します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 斜梁火打")))
            {
                pushButton79.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁火打")));
                pushButton79.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁火打")));
                pushButton79.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁火打")));
                //pushButton79.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁火打")));
                //pushButton79.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁火打")));
                //pushButton79.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁火打")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData80 = new PushButtonData("PushButton Command80", "斜梁火打変更",
                            dllPath, "YMS.ChangeSyabariHiuchiBase");

            PushButton pushButton80 = groupBaseSyabari.AddPushButton(pushButtonData80) as PushButton;
            //PushButton pushButton80 = groupBaseSyabariHiuchi.AddPushButton(pushButtonData80) as PushButton;

            pushButton80.ToolTip = "斜梁火打ベースを変更します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 斜梁火打")))
            {
                pushButton80.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁火打")));
                pushButton80.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁火打")));
                pushButton80.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁火打")));
                //pushButton80.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁火打")));
                //pushButton80.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁火打")));
                //pushButton80.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁火打")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData74 = new PushButtonData("PushButton Command74", "作成",
                            dllPath, "YMS.CreateSyabariTsunagizaiBase");

            PushButton pushButton74 = groupBaseSyabariTsunagizai.AddPushButton(pushButtonData74) as PushButton;

            pushButton74.ToolTip = "斜梁ツナギ材ベースを作成します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 斜梁ツナギ材")))
            {
                pushButton74.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁ツナギ材")));
                pushButton74.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁ツナギ材")));
                pushButton74.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁ツナギ材")));
            }

			///////////////////////////////////////////////////////////////////////////
			PushButtonData pushButtonData75 = new PushButtonData("PushButton Command75", "変更",
							dllPath, "YMS.ChangeSyabariTsunagizaiBase");

			PushButton pushButton75 = groupBaseSyabariTsunagizai.AddPushButton(pushButtonData75) as PushButton;

            pushButton75.ToolTip = "斜梁ツナギ材ベースを変更します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 斜梁ツナギ材")))
            {
                pushButton75.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁ツナギ材")));
                pushButton75.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁ツナギ材")));
                pushButton75.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁ツナギ材")));
            }

			///////////////////////////////////////////////////////////////////////////
			PushButtonData pushButtonData76 = new PushButtonData("PushButton Command76", "作成",
                            dllPath, "YMS.CreateSyabariUkeBase");

            PushButton pushButton76 = groupBaseSyabariUke.AddPushButton(pushButtonData76) as PushButton;

            pushButton76.ToolTip = "斜梁受けベースを作成します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 斜梁受け")))
            {
                pushButton76.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁受け")));
                pushButton76.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁受け")));
                pushButton76.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁受け")));
            }

			///////////////////////////////////////////////////////////////////////////
			PushButtonData pushButtonData77 = new PushButtonData("PushButton Command77", "変更",
							dllPath, "YMS.ChangeSyabariUkeBase");

			PushButton pushButton77 = groupBaseSyabariUke.AddPushButton(pushButtonData77) as PushButton;

            pushButton77.ToolTip = "斜梁受けベースを変更します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 斜梁受け")))
            {
                pushButton77.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁受け")));
                pushButton77.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁受け")));
                pushButton77.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁受け")));
            }

            /////////////////////////////////////////////////////////////////////////////
            //PushButtonData pushButtonData78 = new PushButtonData("PushButton Command78", "端部部品配置",
            //                         dllPath, "YMS.PutSyabariHiuchiPiece");

            //         PushButton pushButton78 = groupBaseSyabari.AddPushButton(pushButtonData78) as PushButton;
            ////         PushButton pushButton78 = groupBaseSyabariHiuchi.AddPushButton(pushButtonData78) as PushButton;

            //         pushButton78.ToolTip = "斜梁火打の端部部品を配置します。";
            //         if (System.IO.File.Exists(GetIconPath(dllPath, "14 斜梁火打")))
            //         {
            //             pushButton78.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁火打")));
            //             pushButton78.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁火打")));
            //             pushButton78.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 斜梁火打")));
            //         }

            ///////////////////////////////////////////////////////////////////////////
            //PushButtonData pushButtonData40 = new PushButtonData("PushButton Command40", "仮ベース作成",
            //                dllPath, "YMS.CreateKariBase");

            //PushButton pushButton40 = panelBase.AddItem(pushButtonData40) as PushButton;

            //pushButton40.ToolTip = "仮ベースを作成します。";

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData101 = new PushButtonData("PushButton Command101", "ベースコピー",
                            dllPath, "YMS.CopyBase");

            PushButton pushButton101 = panelBase.AddItem(pushButtonData101) as PushButton;

            pushButton101.ToolTip = "ベースのコピーを行います。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "ベースコピー")))
            {
                pushButton101.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "ベースコピー")));
                pushButton101.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "ベースコピー")));
                pushButton101.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "ベースコピー")));
            }


            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData41 = new PushButtonData("PushButton Command41", "ベース\n一覧・編集",
                            dllPath, "YMS.EditBaseList");

            PushButton pushButton41 = panelBase.AddItem(pushButtonData41) as PushButton;

            pushButton41.ToolTip = "ベース一覧・編集を行います。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "15 ベース一覧")))
            {
                pushButton41.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "15 ベース一覧")));
                pushButton41.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "15 ベース一覧")));
                pushButton41.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "15 ベース一覧")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData42 = new PushButtonData("PushButton Command42", "作成",
                            dllPath, "YMS.CreateJack");

            PushButton pushButton42 = groupJack.AddPushButton(pushButtonData42) as PushButton;

            pushButton42.ToolTip = "ジャッキを作成します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "16 ジャッキ-作成")))
            {
                pushButton42.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "16 ジャッキ-作成")));
                pushButton42.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "16 ジャッキ-作成")));
                pushButton42.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "16 ジャッキ-作成")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData43 = new PushButtonData("PushButton Command43", "変更",
                            dllPath, "YMS.Changejack");

            PushButton pushButton43 = groupJack.AddPushButton(pushButtonData43) as PushButton;

            pushButton43.ToolTip = "ジャッキを変更します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "16 ジャッキ-変更")))
            {
                pushButton43.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "16 ジャッキ-変更")));
                pushButton43.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "16 ジャッキ-変更")));
                pushButton43.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "16 ジャッキ-変更")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData44 = new PushButtonData("PushButton Command44", "頭ツナギ材作成",
                            dllPath, "YMS.CreateAtamaTsunagiZai");

            //SplitButtonData sb1 = new SplitButtonData("atamaTsunagi", "atamaTsunagi");
            //SplitButton sb = panelAtamaTsunagi.AddItem(sb1) as SplitButton;
            //PushButton pushButton44 = sb.AddPushButton(pushButtonData44) as PushButton;
            PushButton pushButton44 = groupAtamaTsunagi.AddPushButton(pushButtonData44) as PushButton;

            pushButton44.ToolTip = "頭ツナギ材を作成します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "17 頭ツナギ-作成")))
            {
                pushButton44.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "17 頭ツナギ-作成")));
                pushButton44.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "17 頭ツナギ-作成")));
                pushButton44.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "17 頭ツナギ-作成")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData70 = new PushButtonData("PushButton Command70", "頭ツナギ補助材作成",
                            dllPath, "YMS.CreateAtamaTsunagiHojoZai");

            PushButton pushButton70 = groupAtamaTsunagi.AddPushButton(pushButtonData70) as PushButton;

            pushButton70.ToolTip = "頭ツナギ補助材を設定します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "17 頭ツナギ-作成")))
            {
                pushButton70.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "17 頭ツナギ-作成")));
                pushButton70.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "17 頭ツナギ-作成")));
                pushButton70.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "17 頭ツナギ-作成")));
            }

            ///////////////////////////////////////////////////////////////////////////

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData45 = new PushButtonData("PushButton Command45", "頭ツナギ材削除",
                            dllPath, "YMS.DeleteAtamaTsunagi");

            PushButton pushButton45 = groupAtamaTsunagi.AddPushButton(pushButtonData45) as PushButton;

            pushButton45.ToolTip = "頭ツナギ材を削除します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "17 頭ツナギ-変更"))) 
            {
                pushButton45.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "17 頭ツナギ-変更")));
                pushButton45.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "17 頭ツナギ-変更")));
                pushButton45.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "17 頭ツナギ-変更")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData46 = new PushButtonData("PushButton Command46", "作成",
                            dllPath, "YMS.CreateDesumibuHokyouzai");

            PushButton pushButton46 = groupDesumibuHokyouzai.AddPushButton(pushButtonData46) as PushButton;

            pushButton46.ToolTip = "出隅部補強材を作成します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "17 出隅-作成")))
            {
                pushButton46.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "17 出隅-作成")));
                pushButton46.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "17 出隅-作成")));
                pushButton46.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "17 出隅-作成")));
            }

            ///////////////////////////////////////////////////////////////////////////
            //PushButtonData pushButtonData47 = new PushButtonData("PushButton Command47", "変更",
            //                dllPath, "YMS.ChangeDesumibuHokyouzai");

            //PushButton pushButton47 = groupDesumibuHokyouzai.AddPushButton(pushButtonData47) as PushButton;

            //pushButton47.ToolTip = "出隅部補強材を変更します。";
            //if (System.IO.File.Exists(GetIconPath(dllPath, "17 出隅-変更")))
            //{
            //    pushButton47.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "17 出隅-変更")));
            //    pushButton47.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "17 出隅-変更")));
            //    pushButton47.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "17 出隅-変更")));
            //}

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData48 = new PushButtonData("PushButton Command48", "仮鋼材\n配置",
                            dllPath, "YMS.PutKariKouzai");

            //PushButton pushButton48 = panelKariKouzai.AddItem(pushButtonData48) as PushButton;
            //パネルを統合する場合
            PushButton pushButton48 = panelBuhinHaici.AddItem(pushButtonData48) as PushButton;

            pushButton48.ToolTip = "仮鋼材を配置します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "29仮鋼材配置")))
            {
                pushButton48.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "29仮鋼材配置")));
                pushButton48.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "29仮鋼材配置")));
                pushButton48.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "29仮鋼材配置")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData49 = new PushButtonData("PushButton Command49", "作成",
                            dllPath, "YMS.CreateSumibuPiace");

            PushButton pushButton49 = groupSumibuPiace.AddPushButton(pushButtonData49) as PushButton;

            pushButton49.ToolTip = "隅部ピースを作成します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "19 隅部-作成")))
            {
                pushButton49.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "19 隅部-作成")));
                pushButton49.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "19 隅部-作成")));
                pushButton49.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "19 隅部-作成")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData50 = new PushButtonData("PushButton Command50", "変更",
                            dllPath, "YMS.ChangeSumibuPiace");

            PushButton pushButton50 = groupSumibuPiace.AddPushButton(pushButtonData50) as PushButton;

            pushButton50.ToolTip = "隅部ピースを変更します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "19 隅部-変更")))
            {
                pushButton50.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "19 隅部-変更")));
                pushButton50.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "19 隅部-変更")));
                pushButton50.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "19 隅部-変更")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData51 = new PushButtonData("PushButton Command51", "ブラケット\n作成",
                            dllPath, "YMS.CreateBracket");

            //PushButton pushButton51 = panelBracket.AddItem(pushButtonData51) as PushButton;
            //パネルを統合する場合
            PushButton pushButton51 = panelBuhinHaici.AddItem(pushButtonData51) as PushButton;

            pushButton51.ToolTip = "ブラケットを作成します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "20 ブラケット-作成")))
            {
                pushButton51.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "20 ブラケット-作成")));
                pushButton51.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "20 ブラケット-作成")));
                pushButton51.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "20 ブラケット-作成")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData52 = new PushButtonData("PushButton Command52", "作成",
                            dllPath, "YMS.CreateSendanBoltHokyouzai");

            PushButton pushButton52 = groupSendanBoltHokyouzai.AddPushButton(pushButtonData52) as PushButton;

            pushButton52.ToolTip = "せん断ボルト補強材を作成します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "21 ボルト-作成")))
            {
                pushButton52.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "21 ボルト-作成")));
                pushButton52.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "21 ボルト-作成")));
                pushButton52.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "21 ボルト-作成")));
            }

            ///////////////////////////////////////////////////////////////////////////
            //PushButtonData pushButtonData53 = new PushButtonData("PushButton Command53", "変更",
            //                dllPath, "YMS.ChangeSendanBoltHokyouzai");

            //PushButton pushButton53 = groupSendanBoltHokyouzai.AddPushButton(pushButtonData53) as PushButton;

            //pushButton53.ToolTip = "せん断ボルト補強材を変更します。";
            //if (System.IO.File.Exists(GetIconPath(dllPath, "21 ボルト-変更")))
            //{
            //    pushButton53.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "21 ボルト-変更")));
            //    pushButton53.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "21 ボルト-変更")));
            //    pushButton53.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "21 ボルト-変更")));
            //}

            /////////////////////////////////////////////////////////////////////////////
            //PushButtonData pushButtonData54 = new PushButtonData("PushButton Command54", "作成",
            //                dllPath, "YMS.CreateJyougeHaraokoshiTsunagi");

            //PushButton pushButton54 = groupJyougeHaraokoshiTsunagi.AddPushButton(pushButtonData54) as PushButton;

            //pushButton54.ToolTip = "上下腹起ツナギ材を作成します。";

            /////////////////////////////////////////////////////////////////////////////
            //PushButtonData pushButtonData55 = new PushButtonData("PushButton Command55", "変更",
            //                dllPath, "YMS.ChangeJyougeHaraokoshiTsunagi");

            //PushButton pushButton55 = groupJyougeHaraokoshiTsunagi.AddPushButton(pushButtonData55) as PushButton;

            //pushButton55.ToolTip = "上下腹起ツナギ材を変更します。";

            /////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData56 = new PushButtonData("PushButton Command56", "作成",
                            dllPath, "YMS.CreateHaraokoshiSuberidome");

            PushButton pushButton56 = groupHaraokoshiSuberidome.AddPushButton(pushButtonData56) as PushButton;

            pushButton56.ToolTip = "腹起スベリ止めを作成します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "23　腹起スベリ-作成")))
            {
                pushButton56.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "23　腹起スベリ-作成")));
                pushButton56.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "23　腹起スベリ-作成")));
                pushButton56.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "23　腹起スベリ-作成")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData57 = new PushButtonData("PushButton Command57", "変更",
                            dllPath, "YMS.ChangeHaraokoshiSuberidome");

            PushButton pushButton57 = groupHaraokoshiSuberidome.AddPushButton(pushButtonData57) as PushButton;

            pushButton57.ToolTip = "腹起スベリ止めを変更します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "23　腹起スベリ-変更")))
            {
                pushButton57.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "23　腹起スベリ-変更")));
                pushButton57.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "23　腹起スベリ-変更")));
                pushButton57.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "23　腹起スベリ-変更")));
            }

            ///////////////////////////////////////////////////////////////////////////
            //PushButtonData pushButtonData58 = new PushButtonData("PushButton Command58", "交叉部\n周り\n自動\n作成",
            //                dllPath, "YMS.CreateKousyabuMawari");
            PushButtonData pushButtonData58 = new PushButtonData("PushButton Command58", "交叉部周り\n自動作成",
                            dllPath, "YMS.CreateKousyabuMawari");

            //PushButton pushButton58 = panelKousyabuMawari.AddItem(pushButtonData58) as PushButton;
            //パネルを統合する場合
            PushButton pushButton58 = panelBuhinHaici.AddItem(pushButtonData58) as PushButton;

            pushButton58.ToolTip = "交叉部周りを自動作成します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "24 交差")))
            {
                pushButton58.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "24 交差")));
                pushButton58.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "24 交差")));
                pushButton58.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "24 交差")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData65 = new PushButtonData("PushButton Command65", "締付用ピース\n作成",
                            dllPath, "YMS.CreateShimetukePiece");

            PushButton pushButton65 = panelBuhinHaici.AddItem(pushButtonData65) as PushButton;

            pushButton65.ToolTip = "締付用ピースを作成します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "30 締付用ピース作成")))
            {
                pushButton65.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "30 締付用ピース作成")));
                pushButton65.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "30 締付用ピース作成")));
                pushButton65.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "30 締付用ピース作成")));
            }

            /////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData59 = new PushButtonData("PushButton Command59", "スチフナー\n作成",
                            dllPath, "YMS.CreateStiffener");

            //PushButton pushButton59 = panelStiffener.AddItem(pushButtonData59) as PushButton;
            //パネルを統合する場合
            PushButton pushButton59 = panelBuhinHaici.AddItem(pushButtonData59) as PushButton;

            pushButton59.ToolTip = "スチフナーを作成します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "25 スチフナー")))
            {
                pushButton59.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "25 スチフナー")));
                pushButton59.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "25 スチフナー")));
                pushButton59.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "25 スチフナー")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData66 = new PushButtonData("PushButton Command66", "自動\n割付",
                           dllPath, "YMS.WaritukeJidou");

            //PushButton pushButton60 = panelWaritukeSyudou.AddItem(pushButtonData60) as PushButton;
            //パネルを統合する場合
            PushButton pushButton66 = panelBuhinHaici.AddItem(pushButtonData66) as PushButton;

            pushButton66.ToolTip = "仮鋼材の割付部材への自動置換を行います。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "31自動割付")))
            {
                pushButton66.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "31自動割付")));
                pushButton66.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "31自動割付")));
                pushButton66.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "31自動割付")));
            }



            PushButtonData pushButtonData60 = new PushButtonData("PushButton Command60", "部材\n割付",
                            dllPath, "YMS.WaritukeSyudou");

            //PushButton pushButton60 = panelWaritukeSyudou.AddItem(pushButtonData60) as PushButton;
            //パネルを統合する場合
            PushButton pushButton60 = panelBuhinHaici.AddItem(pushButtonData60) as PushButton;

            pushButton60.ToolTip = "部材割付を行います。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "26 手動割付")))
            {
                pushButton60.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "26 手動割付")));
                pushButton60.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "26 手動割付")));
                pushButton60.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "26 手動割付")));
            }

            PushButtonData pushButtonData67 = new PushButtonData("PushButton Command67", "主材削除",
                          dllPath, "YMS.DeleteWarituke");

            //PushButton pushButton60 = panelWaritukeSyudou.AddItem(pushButtonData60) as PushButton;
            //パネルを統合する場合
            PushButton pushButton67 = panelBuhinHaici.AddItem(pushButtonData67) as PushButton;

            pushButton67.ToolTip = "主材の選択削除を行います。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "32　割付削除")))
            {
                pushButton67.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "32　割付削除")));
                pushButton67.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "32　割付削除")));
                pushButton67.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "32　割付削除")));
            }

            ///////////////////////////////////////////////////////////////////////////
            //PushButtonData pushButtonData61 = new PushButtonData("PushButton Command61", "鋼材\n長さ\n変更",
            //                dllPath, "YMS.ChangeLengthKouzai");
            PushButtonData pushButtonData61 = new PushButtonData("PushButton Command61", "鋼材長さ\n変更",
                            dllPath, "YMS.ChangeLengthKouzai");

            //PushButton pushButton61 = panelLengthKouzai.AddItem(pushButtonData61) as PushButton;
            //パネルを統合する場合
            PushButton pushButton61 = panelBuhinHaici.AddItem(pushButtonData61) as PushButton;

            pushButton61.ToolTip = "鋼材長さを変更します。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "27 鋼材長さ")))
            {
                pushButton61.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "27 鋼材長さ")));
                pushButton61.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "27 鋼材長さ")));
                pushButton61.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "27 鋼材長さ")));
            }

            /////////////////////////////////////////////////////////////////////////////
            ////PushButtonData pushButtonData62 = new PushButtonData("PushButton Command62", "腹起\nブラケット\n作成",
            ////                dllPath, "YMS.CreateHaraokoshiBracket");
            //PushButtonData pushButtonData62 = new PushButtonData("PushButton Command62", "腹起ブラケット\n作成",
            //                dllPath, "YMS.CreateHaraokoshiBracket");

            ////PushButton pushButton62 = panelHaraokoshiBracket.AddItem(pushButtonData62) as PushButton;
            ////パネルを統合する場合
            //PushButton pushButton62 = panelBuhinHaici.AddItem(pushButtonData62) as PushButton;

            //pushButton62.ToolTip = "腹起ブラケットを作成します。";
            //if (System.IO.File.Exists(GetIconPath(dllPath, "28 腹起しブラケット")))
            //{
            //    pushButton62.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "28 腹起しブラケット")));
            //    pushButton62.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "28 腹起しブラケット")));
            //    pushButton62.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "28 腹起しブラケット")));
            //}

            ///////////////////////////////////////////////////////////////////////////
            //PushButtonData pushButtonData63 = new PushButtonData("PushButton Command63", "ワークセット\n作成",
            //                dllPath, "YMS.CreateWorkset");

            //PushButton pushButton63 = panelworkset.AddItem(pushButtonData63) as PushButton;

            //pushButton63.ToolTip = "ワークセットを作成します。";
            //if (System.IO.File.Exists(GetIconPath(dllPath, "3 ワークセット")))
            //{
            //    pushButton63.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "3 ワークセット")));
            //    pushButton63.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "3 ワークセット")));
            //    pushButton63.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "3 ワークセット")));
            //}


            ///////////////////////////////////////////////////////////////////////////

            PushButtonData pushButtonData64 = new PushButtonData("PushButton Command64", "個別配置",
                           dllPath, "YMS.KobetsuHaichi");

            //PushButton pushButton62 = panelHaraokoshiBracket.AddItem(pushButtonData62) as PushButton;
            //パネルを統合する場合
            PushButton pushButton64 = panelBuhinHaici.AddItem(pushButtonData64) as PushButton;

            pushButton64.ToolTip = "各部材の個別配置を行います。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "33 個別配置")))
            {
                pushButton64.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "33 個別配置")));
                pushButton64.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "33 個別配置")));
                pushButton64.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "33 個別配置")));
            }




            PushButtonData pushButtonData100 = new PushButtonData("PushButton Command100", "切梁割付コピー",
                          dllPath, "YMS.KiribariCopy");

            PushButton pushButton100 = panelBuhinHaici.AddItem(pushButtonData100) as PushButton;

            pushButton100.ToolTip = "割り付けた切梁部材のコピーを行います。";
            if (System.IO.File.Exists(GetIconPath(dllPath, "34 切梁割付コピー")))
            {
                pushButton100.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "34 切梁割付コピー")));
                pushButton100.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "34 切梁割付コピー")));
                pushButton100.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "34 切梁割付コピー")));
            }

        }

        /// <summary>
        /// アイコンのフルパス取得
        /// </summary>
        /// <param name="dllPath">アドインの場所</param>
        /// <param name="name">アイコン名（拡張子なし）</param>
        /// <param name="jpg">true = png  false = jpg</param>
        /// <returns></returns>
        private string GetIconPath(string dllPath, string name, bool png = true)
        {
            string res = string.Empty;
            string ymsFolpath = ClsYMSUtil.GetExecutingAssemblyYMSPath();
            string iconFolderPath = System.IO.Path.Combine(ymsFolpath, "icon");

            res = System.IO.Path.Combine(iconFolderPath, name);

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
}
