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
            UpdaterRegistry.AddTrigger(updaterL.GetUpdaterId(), cLineFilter, Element.GetChangeTypeAny());//�����ModelLine�𓮂����Ɣ�������

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
            //string ibPath = System.IO.Path.Combine(symbolFolpath, "icon\\�A�C�R��32");
            //string isPath = System.IO.Path.Combine(symbolFolpath, "icon\\�A�C�R��16");
            app.CreateRibbonTab("TEST");
            app.CreateRibbonTab("TEST�p�R�}���h");
            RibbonPanel panel =
              app.CreateRibbonPanel("TEST", "TEST");
            RibbonPanel panel2 =
              app.CreateRibbonPanel("TEST�p�R�}���h", "TEST�p�R�}���h");

            PushButtonData pushButtonData1 = new PushButtonData("PushButton Command1", "test",
                            dllPath, "YMS.CommandTest");


            PushButton pushButton1 = panel.AddItem(pushButtonData1) as PushButton;
            //string symbolFolpath = ClsZumenInfo.GetFamilyFolder();
            //string iconPath = System.IO.Path.Combine(symbolFolpath, "icon\\testicon");
            //pushButton1.LargeImage = new BitmapImage(new Uri(iconPath));

            pushButton1.ToolTip = "test";

            ///////////////////////////////////////////////////////////////////////////
            //ReplaceModelLineToShin
            PushButtonData pushButtonData2 = new PushButtonData("PushButton Command2", " ���f����������c�֒u�� ",
                          dllPath, "YMS.ReplaceModelLineToShin");


            PushButton pushButton2 = panel.AddItem(pushButtonData2) as PushButton;

            pushButton2.ToolTip = "���f����������c�֒u��";
            /////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData3 = new PushButtonData("PushButton Command3", " ���ʐc�ɉ��z�u ",
                         dllPath, "YMS.CreateKariKiribari");


            PushButton pushButton3 = panel.AddItem(pushButtonData3) as PushButton;

            pushButton3.ToolTip = "���ʐc�ɉ��z�u";

            /////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData4 = new PushButtonData("PushButton Command4", "���[���[���t�e�X�g",
                         dllPath, "YMS.testtest");


            PushButton pushButton4 = panel.AddItem(pushButtonData4) as PushButton;

            pushButton4.ToolTip = "�s�b�N�_����X�����փ��[���[����������̂ňʒu�w������Ă��������B���t���s���܂��B";

            /////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData5 = new PushButtonData("PushButton Command5", " FormTest ",
                          dllPath, "YMS.FormTest");


            PushButton pushButton5 = panel.AddItem(pushButtonData5) as PushButton;

            pushButton5.ToolTip = "FormTest";

            /////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData6 = new PushButtonData("PushButton Command6", "�@��p�{�C�h�t�@�~���̔z�u  ",
                          dllPath, "YMS.FormDlgCreateVoidFamily");


            PushButton pushButton6 = panel.AddItem(pushButtonData6) as PushButton;

            pushButton6.ToolTip = "�{�C�h��z�u���w��̃t�@�~�����@�킵�܂��B";

            /////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData7 = new PushButtonData("PushButton Command7", "��_�Ƀt�@�~����z�u  ",
                          dllPath, "YMS.CreateIntersectionFamilyCommand");


            PushButton pushButton7 = panel.AddItem(pushButtonData7) as PushButton;

            pushButton7.ToolTip = "����2�{�w�肵���̌�_�Ƀt�@�~����z�u���܂��B";

            /////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData8 = new PushButtonData("PushButton Command8", "�x�[�X�A���|�ގ擾  ",
                          dllPath, "YMS.CreateSymbolCommand");


            PushButton pushButton8 = panel.AddItem(pushButtonData8) as PushButton;

            pushButton8.ToolTip = "�x�[�X�A���|�ނ��擾�B";

            /////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData9 = new PushButtonData("PushButton Command9", "�w��c�ɕǂ�z�u  ",
                          dllPath, "YMS.CmdElevationWatcher");


            PushButton pushButton9 = panel.AddItem(pushButtonData9) as PushButton;

            pushButton9.ToolTip = "�w�肵���c�ɕǂ�z�u���܂��B";

            /////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData10 = new PushButtonData("PushButton Command10", "testteat  ",
                          dllPath, "YMS.Rubberband");


            PushButton pushButton10 = panel.AddItem(pushButtonData10) as PushButton;

            pushButton10.ToolTip = "�}�ʏ���ݒ肵�܂��B";
            /////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData11 = new PushButtonData("PushButton Command11", "CASETEST  ",
                          dllPath, "YMS.FormDlgCreateCASETest");


            PushButton pushButton11 = panel.AddItem(pushButtonData11) as PushButton;

            pushButton11.ToolTip = "CASE����ݒ肵�܂��B";
            /////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData12 = new PushButtonData("PushButton Command12", "���o�[�o���h  ",
                          dllPath, "YMS.Rubberband");


            PushButton pushButton12 = panel.AddItem(pushButtonData12) as PushButton;

            pushButton12.ToolTip = "���o�[�o���h�B";
            //#if DEBUG
            //            RibbonPanel panelTest =
            //              app.CreateRibbonPanel("Revit PHS TOOL", "Test");


            //            //test
            //            PushButtonData pushButtonDataTest = new PushButtonData("PushButton CommandTest", "test",
            //                            dllPath, "DaifukuRevitPHSTool.testkamada");
            //            PushButton pushButtonTest = panelTest.AddItem(pushButtonDataTest) as PushButton;

            //            pushButtonTest.ToolTip = "TEST";
            //#endif


            ////�A�C�R���̐ݒ�
            //string iconPath = System.IO.Path.Combine(dir, "icon", "PHS_Replace.png");
            //if (System.IO.File.Exists(iconPath))
            //{
            //    pushButton1.LargeImage = new BitmapImage(new Uri(iconPath));
            //}

            PulldownButtonData groupTest_Kurane = new PulldownButtonData("TEST_Kurane", "TEST_Kurane");
            PulldownButton groupYamadomeTest_Kurane = panel.AddItem(groupTest_Kurane) as PulldownButton;

            PushButtonData pushButtonData13 = new PushButtonData("PushButton Command13", "�R�}���h1", dllPath, "YMS.FormDlgTest_Kurane");
            PushButton pushButton13 = groupYamadomeTest_Kurane.AddPushButton(pushButtonData13) as PushButton;
            pushButton13.ToolTip = "�e�X�g�R�}���h";

            PushButtonData pushButtonData14 = new PushButtonData("PushButton Command14", "�R�}���h2", dllPath, "YMS.FormDlgTest_Kurane2");
            PushButton pushButton14 = groupYamadomeTest_Kurane.AddPushButton(pushButtonData14) as PushButton;
            pushButton13.ToolTip = "�e�X�g�R�}���h";



            /////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData15 = new PushButtonData("PushButton Command15", "�����E�o���`�F�b�N",
                          dllPath, "YMS.CheckHaraokoshiIrizumi");


            PushButton pushButton15 = panel.AddItem(pushButtonData15) as PushButton;

            pushButton15.ToolTip = "�����E�o���`�F�b�N���܂��B";
            /////////////////////////////////////////////////////////////////////////////

            //���TESTcommand
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

            //�A�C�R���̐ݒ�

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
            app.CreateRibbonTab("TEST�p�R�}���h");
            RibbonPanel panel =
              app.CreateRibbonPanel("TEST�p�R�}���h", "TEST�p�R�}���h");

            //���TESTcommand
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
                var buttonData = new PushButtonData("�Η�B", "�Η�B", dllPath, typeof(YMS.TestShabariBase).FullName);
                var button = panel.AddItem(buttonData) as PushButton;
                button.ToolTip = nameof(YMS.TestShabariBase);
            }
            {
                var buttonData = new PushButtonData("�Η��Α�", "�Η��Α�", dllPath, typeof(YMS.TestShabariHiuchi).FullName);
                var button = panel.AddItem(buttonData) as PushButton;
                button.ToolTip = nameof(YMS.TestShabariHiuchi);
            }
            /////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData15 = new PushButtonData("�ʔz�uTEST", "�ʔz�uTEST",
                          dllPath, "YMS.TEST15");


            PushButton pushButton15 = panel.AddItem(pushButtonData15) as PushButton;

            pushButton8.ToolTip = "TEST15";
            /////////////////////////////////////////////////////////////////////////////

        }
        public void AddRibbonMenu_Setting(UIControlledApplication app)
        {
            string dllPath = ClsSystemData.GetDLLPath();

            // �^�u�̒ǉ�
            app.CreateRibbonTab("YMS�ݒ�");

            // �p�l���̒ǉ�
            RibbonPanel panelSyokisettei = app.CreateRibbonPanel("YMS�ݒ�", "�����ݒ�E���m�F");

            // �{�^���̒ǉ�
            PushButtonData pushButtonData1 = new PushButtonData("PushButton Command1", "�}��\n���", dllPath, "YMS.CommandZumenInfo");
            PushButton pushButton1 = panelSyokisettei.AddItem(pushButtonData1) as PushButton;
            pushButton1.LongDescription = "�}�ʏ���ݒ肵�܂�";

            if (System.IO.File.Exists(GetIconPath(dllPath, "1 �}�ʏ��")))
            {
                pushButton1.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "1 �}�ʏ��")));
                pushButton1.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "1 �}�ʏ��")));
                pushButton1.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "1 �}�ʏ��")));
            }

            //#33548
            PushButtonData pushButtonData2 = new PushButtonData("PushButton Command200", "Light\n�A�g", dllPath, "YMS.CommandReadJson");
            PushButton pushButton2 = panelSyokisettei.AddItem(pushButtonData2) as PushButton;
            pushButton2.LongDescription = "YMS Light����o�͂��ꂽJSON�t�@�C����ǂݍ��݂܂�";

            if (System.IO.File.Exists(GetIconPath(dllPath, "Light�A�g")))
            {
                pushButton2.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "Light�A�g")));
                pushButton2.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "Light�A�g")));
                pushButton2.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "Light�A�g")));
            }

        }

        public void AddRibbonMenu_Yamadome(UIControlledApplication app)
        {
            string dllPath = ClsSystemData.GetDLLPath();

            app.CreateRibbonTab("YMS�R��");

            //RibbonPanel panelworkset = app.CreateRibbonPanel("YMS", "���[�N�Z�b�g");
            RibbonPanel panelKabeShin = app.CreateRibbonPanel("YMS�R��", "�ǐc");
            RibbonPanel panelYamdomeKabe = app.CreateRibbonPanel("YMS�R��", "�R����");
            RibbonPanel panelKui = app.CreateRibbonPanel("YMS�R��", "�Y");
            RibbonPanel panelBase = app.CreateRibbonPanel("YMS�R��", "�x�[�X");
            //RibbonPanel panelJack = app.CreateRibbonPanel("YMS�R��", "�W���b�L");
            //RibbonPanel panelAtamaTsunagi = app.CreateRibbonPanel("YMS�R��", "���c�i�M");
            //RibbonPanel panelDesumibuHokyouzai = app.CreateRibbonPanel("YMS�R��", "�o�����⋭��");
            //RibbonPanel panelKariKouzai = app.CreateRibbonPanel("YMS�R��", "���|��");
            //RibbonPanel panelSumibuPiace = app.CreateRibbonPanel("YMS�R��", "�����s�[�X");
            //RibbonPanel panelBracket = app.CreateRibbonPanel("YMS�R��", "�u���P�b�g");
            //RibbonPanel panelSendanBoltHokyouzai = app.CreateRibbonPanel("YMS�R��", "����f�{���g�⋭��");
            //RibbonPanel panelHaraokoshiSuberidome = app.CreateRibbonPanel("YMS�R��", "���N�X�x���~��");
            //RibbonPanel panelKousyabuMawari = app.CreateRibbonPanel("YMS�R��", "����������");
            //RibbonPanel panelStiffener = app.CreateRibbonPanel("YMS�R��", "�X�`�t�i�[");
            //RibbonPanel panelWaritukeSyudou = app.CreateRibbonPanel("YMS�R��", "�蓮���t");
            //RibbonPanel panelLengthKouzai = app.CreateRibbonPanel("YMS�R��", "�|�ޒ���");
            //RibbonPanel panelHaraokoshiBracket = app.CreateRibbonPanel("YMS�R��", "���N�u���P�b�g");
            RibbonPanel panelBuhinHaici = app.CreateRibbonPanel("YMS�R��", "�e���i�z�u");


            PulldownButtonData groupKouyaitaData = new PulldownButtonData("�|���", "�|���");
            PulldownButtonData groupOyakuiData = new PulldownButtonData("�e�Y", "�e�Y");
            PulldownButtonData groupYokoyaitaData = new PulldownButtonData("�����", "�����");
            PulldownButtonData groupRenzokuKabeData = new PulldownButtonData("�A����", "�A����");
            PulldownButtonData groupSMWData = new PulldownButtonData("SMW", "SMW");

            PulldownButtonData groupKuiData = new PulldownButtonData("TC�Y�E�\��Y�E���p�Y�E�f�ʕω��Y", "TC�Y�E\n�\��Y�E\n���p�Y�E\n�f�ʕω��Y");
            PulldownButtonData groupTanakuiData = new PulldownButtonData("�I�Y�E���ԍY", "�I�Y�E\n���ԍY\n\n");

            PulldownButtonData groupHaraokoshiBaseData = new PulldownButtonData("���N�x�[�X", "���N");
            PulldownButtonData groupKiribariBaseData = new PulldownButtonData("�ؗ��x�[�X", "�ؗ�");
            PulldownButtonData groupSanjikuBaseData = new PulldownButtonData("�O���s�[�X", "�O��");
            PulldownButtonData groupSumibuBaseData = new PulldownButtonData("���ΑŃx�[�X", "���Α�");
            PulldownButtonData groupKiribariHiuchiBaseData = new PulldownButtonData("�ؗ��ΑŃx�[�X", "�ؗ��Α�");
            PulldownButtonData groupKiribariUkeBaseData = new PulldownButtonData("�ؗ��󂯃x�[�X", "�ؗ���");
            PulldownButtonData groupKiribariTsunagizaiBaseData = new PulldownButtonData("�ؗ��c�i�M�ރx�[�X", "�ؗ��c�i�M��");
            PulldownButtonData groupHiuchiTsunagizaiBaseData = new PulldownButtonData("�ΑŃc�i�M�ރx�[�X", "�ΑŃc�i�M��");
            PulldownButtonData groupKiribariTsunagiBaseData = new PulldownButtonData("�ؗ��p���x�[�X", "�ؗ��p��");
            PulldownButtonData groupSyabariBaseData = new PulldownButtonData("�Η��x�[�X", "�Η�");
            PulldownButtonData groupSyabariTsunagizaiBaseData = new PulldownButtonData("�Η��c�i�M�ރx�[�X", "�Η��c�i�M");
            PulldownButtonData groupSyabariUkeBaseData = new PulldownButtonData("�Η��󂯃x�[�X", "�Η���");
            PulldownButtonData groupSyabariHiuchiBaseData = new PulldownButtonData("�Η��ΑŃx�[�X", "�Η��Α�");



            PulldownButtonData groupJackData = new PulldownButtonData("�W���b�L", "�W���b�L");
            if (System.IO.File.Exists(GetIconPath(dllPath, "16 �W���b�L-�쐬")))
            {
                groupJackData.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "16 �W���b�L-�쐬")));
                groupJackData.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "16 �W���b�L-�쐬")));
                groupJackData.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "16 �W���b�L-�쐬")));
            }
            PulldownButtonData groupAtamaTsunagiData = new PulldownButtonData("���c�i�M", "���c�i�M");
            if (System.IO.File.Exists(GetIconPath(dllPath, "17 ���c�i�M-�쐬")))
            {
                groupAtamaTsunagiData.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "17 ���c�i�M-�쐬")));
                groupAtamaTsunagiData.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "17 ���c�i�M-�쐬")));
                groupAtamaTsunagiData.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "17 ���c�i�M-�쐬")));
            }
            PulldownButtonData groupDesumibuHokyouzaiData = new PulldownButtonData("�o�����⋭��", "�o�����⋭��");
            if (System.IO.File.Exists(GetIconPath(dllPath, "17 �o��-�쐬")))
            {
                groupDesumibuHokyouzaiData.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "17 �o��-�쐬")));
                groupDesumibuHokyouzaiData.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "17 �o��-�쐬")));
                groupDesumibuHokyouzaiData.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "17 �o��-�쐬")));
            }
            PulldownButtonData groupSumibuPiaceData = new PulldownButtonData("�����s�[�X", "�����s�[�X");
            if (System.IO.File.Exists(GetIconPath(dllPath, "19 ����-�쐬")))
            {
                groupSumibuPiaceData.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "19 ����-�쐬")));
                groupSumibuPiaceData.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "19 ����-�쐬")));
                groupSumibuPiaceData.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "19 ����-�쐬")));
            }
            PulldownButtonData groupSendanBoltHokyouzaiData = new PulldownButtonData("����f�{���g�⋭��", "����f�{���g�⋭��");
            if (System.IO.File.Exists(GetIconPath(dllPath, "21 �{���g-�쐬")))
            {
                groupSendanBoltHokyouzaiData.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "21 �{���g-�쐬")));
                groupSendanBoltHokyouzaiData.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "21 �{���g-�쐬")));
                groupSendanBoltHokyouzaiData.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "21 �{���g-�쐬")));
            }
            PulldownButtonData groupHaraokoshiSuberidomeData = new PulldownButtonData("���N�X�x���~��", "���N�X�x���~��");
            if (System.IO.File.Exists(GetIconPath(dllPath, "23�@���N�X�x��-�쐬")))
            {
                groupHaraokoshiSuberidomeData.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "23�@���N�X�x��-�쐬")));
                groupHaraokoshiSuberidomeData.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "23�@���N�X�x��-�쐬")));
                groupHaraokoshiSuberidomeData.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "23�@���N�X�x��-�쐬")));
            }
            IList<RibbonItem> addedButtons = panelYamdomeKabe.AddStackedItems(groupKouyaitaData, groupOyakuiData, groupYokoyaitaData);
            List<PulldownButton> pullBtnList = new List<PulldownButton>();
            foreach (RibbonItem item in addedButtons)
            {
                pullBtnList.Add(item as PulldownButton);
            }
            PulldownButton groupYamadomeKouyaita = pullBtnList[0];
            groupYamadomeKouyaita.ToolTip = "�|���";
            if (System.IO.File.Exists(GetIconPath(dllPath, "5 �|���-�쐬 - 16")))
            {
                groupYamadomeKouyaita.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "5 �|���-�쐬 - 16")));
                groupYamadomeKouyaita.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "5 �|���-�쐬 - 16")));
                groupYamadomeKouyaita.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "5 �|���-�쐬 - 16")));
            }

            PulldownButton groupYamadomeOyakui = pullBtnList[1];
            groupYamadomeOyakui.ToolTip = "�e�Y";
            if (System.IO.File.Exists(GetIconPath(dllPath, "6 �e�Y-�쐬 - 16")))
            {
                groupYamadomeOyakui.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "6 �e�Y-�쐬 - 16")));
                groupYamadomeOyakui.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "6 �e�Y-�쐬 - 16")));
                groupYamadomeOyakui.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "6 �e�Y-�쐬 - 16")));
            }
            PulldownButton groupYamadomeYokoyaita = pullBtnList[2];
            groupYamadomeYokoyaita.ToolTip = "�����";
            if (System.IO.File.Exists(GetIconPath(dllPath, "7 �����-�쐬 - 16")))
            {
                groupYamadomeYokoyaita.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "7 �����-�쐬 - 16")));
                groupYamadomeYokoyaita.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "7 �����-�쐬 - 16")));
                groupYamadomeYokoyaita.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "7 �����-�쐬 - 16")));
            }
            addedButtons = panelYamdomeKabe.AddStackedItems(groupRenzokuKabeData, groupSMWData);
            pullBtnList = new List<PulldownButton>();
            foreach (RibbonItem item in addedButtons)
            {
                pullBtnList.Add(item as PulldownButton);
            }
            PulldownButton groupYamadomeRenzokuKabe = pullBtnList[0];
            groupYamadomeRenzokuKabe.ToolTip = "�A����";
            if (System.IO.File.Exists(GetIconPath(dllPath, "8 �A����-�쐬 - 16")))
            {
                groupYamadomeRenzokuKabe.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "8 �A����-�쐬 - 16")));
                groupYamadomeRenzokuKabe.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "8 �A����-�쐬 - 16")));
                groupYamadomeRenzokuKabe.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "8 �A����-�쐬 - 16")));
            }
            PulldownButton groupYamadomeSMW = pullBtnList[1];
            groupYamadomeSMW.ToolTip = "SMW";
            if (System.IO.File.Exists(GetIconPath(dllPath, "9 SMW-�쐬 - 16")))
            {
                groupYamadomeSMW.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "9 SMW-�쐬 - 16")));
                groupYamadomeSMW.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "9 SMW-�쐬 - 16")));
                groupYamadomeSMW.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "9 SMW-�쐬 - 16")));
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
            groupBaseHaraokoshi.ToolTip = "���N�x�[�X";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 ���N")))
            {
                groupBaseHaraokoshi.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 ���N")));
                groupBaseHaraokoshi.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 ���N")));
                groupBaseHaraokoshi.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 ���N")));
            }
            PulldownButton groupBaseKiribari = pullBtnList[1];
            groupBaseKiribari.ToolTip = "�ؗ��x�[�X";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 �ؗ�")))
            {
                groupBaseKiribari.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ�")));
                groupBaseKiribari.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ�")));
                groupBaseKiribari.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ�")));
            }
            PulldownButton groupBaseSanjiku = pullBtnList[2];
            groupBaseSanjiku.ToolTip = "�O���s�[�X";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 �O��")))
            {
                groupBaseSanjiku.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �O��")));
                groupBaseSanjiku.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �O��")));
                groupBaseSanjiku.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �O��")));
            }
            addedButtons = panelBase.AddStackedItems(groupSumibuBaseData, groupKiribariHiuchiBaseData, groupKiribariUkeBaseData);
            pullBtnList = new List<PulldownButton>();
            foreach (RibbonItem item in addedButtons)
            {
                pullBtnList.Add(item as PulldownButton);
            }
            PulldownButton groupBasesumibu = pullBtnList[0];
            groupBasesumibu.ToolTip = "���ΑŃx�[�X";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 ���Α�")))
            {
                groupBasesumibu.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 ���Α�")));
                groupBasesumibu.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 ���Α�")));
                groupBasesumibu.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 ���Α�")));
            }
            PulldownButton groupBaseKiribarHiuchi = pullBtnList[1];
            groupBaseKiribarHiuchi.ToolTip = "�ؗ��ΑŃx�[�X";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 �ؗ��Α�")))
            {
                groupBaseKiribarHiuchi.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ��Α�")));
                groupBaseKiribarHiuchi.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ��Α�")));
                groupBaseKiribarHiuchi.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ��Α�")));
            }
            PulldownButton groupBaseKiribariUke = pullBtnList[2];
            groupBaseKiribariUke.ToolTip = "�ؗ��󂯃x�[�X";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 �ؗ���")))
            {
                groupBaseKiribariUke.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ���")));
                groupBaseKiribariUke.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ���")));
                groupBaseKiribariUke.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ���")));
            }
            addedButtons = panelBase.AddStackedItems(groupKiribariTsunagizaiBaseData, groupHiuchiTsunagizaiBaseData, groupKiribariTsunagiBaseData);
            pullBtnList = new List<PulldownButton>();
            foreach (RibbonItem item in addedButtons)
            {
                pullBtnList.Add(item as PulldownButton);
            }
            PulldownButton groupBaseKiribariTsunagizai = pullBtnList[0];
            groupBaseKiribariTsunagizai.ToolTip = "�ؗ��c�i�M�ރx�[�X";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 �ؗ��c�i�M")))
            {
                groupBaseKiribariTsunagizai.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ��c�i�M")));
                groupBaseKiribariTsunagizai.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ��c�i�M")));
                groupBaseKiribariTsunagizai.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ��c�i�M")));
            }
            PulldownButton groupBaseHiuchiTsunagizai = pullBtnList[1];
            groupBaseHiuchiTsunagizai.ToolTip = "�ΑŃc�i�M�ރx�[�X";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 �ΑŃc�i�M")))
            {
                groupBaseHiuchiTsunagizai.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ΑŃc�i�M")));
                groupBaseHiuchiTsunagizai.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ΑŃc�i�M")));
                groupBaseHiuchiTsunagizai.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ΑŃc�i�M")));
            }
            PulldownButton groupBaseKiribariTsunagi = pullBtnList[2];
            groupBaseKiribariTsunagi.ToolTip = "�ؗ��p���x�[�X";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 �ؗ��p��")))
            {
                groupBaseKiribariTsunagi.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ��p��")));
                groupBaseKiribariTsunagi.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ��p��")));
                groupBaseKiribariTsunagi.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ��p��")));
            }

            addedButtons = panelBase.AddStackedItems(groupSyabariBaseData, groupSyabariTsunagizaiBaseData, groupSyabariUkeBaseData);
            pullBtnList = new List<PulldownButton>();
            foreach (RibbonItem item in addedButtons)
            {
                pullBtnList.Add(item as PulldownButton);
            }
            PulldownButton groupBaseSyabari = pullBtnList[0];
            groupBaseSyabari.ToolTip = "�Η��x�[�X";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 �Η�")))
            {
                groupBaseSyabari.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η�")));
                groupBaseSyabari.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η�")));
                groupBaseSyabari.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η�")));
            }
            PulldownButton groupBaseSyabariTsunagizai = pullBtnList[1];
            groupBaseSyabariTsunagizai.ToolTip = "�Η��c�i�M�ރx�[�X";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 �Η��c�i�M��")))
            {
                groupBaseSyabariTsunagizai.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η��c�i�M��")));
                groupBaseSyabariTsunagizai.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η��c�i�M��")));
                groupBaseSyabariTsunagizai.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η��c�i�M��")));
            }
            PulldownButton groupBaseSyabariUke = pullBtnList[2];
            groupBaseSyabariUke.ToolTip = "�Η��󂯃x�[�X";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 �Η���")))
            {
                groupBaseSyabariUke.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η���")));
                groupBaseSyabariUke.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η���")));
                groupBaseSyabariUke.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η���")));
            }

			//RibbonItem addedButton = panelBase.AddItem(groupSyabariHiuchiBaseData);
   //         pullBtnList = new List<PulldownButton>();
   //         pullBtnList.Add(addedButton as PulldownButton);
   //         PulldownButton groupBaseSyabariHiuchi = pullBtnList[0];
   //         groupBaseSyabariHiuchi.ToolTip = "�Η��ΑŃx�[�X";
   //         if (System.IO.File.Exists(GetIconPath(dllPath, "14 �Η��Α�")))
   //         {
   //             groupBaseSyabariHiuchi.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η��Α�")));
   //             groupBaseSyabariHiuchi.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η��Α�")));
   //             groupBaseSyabariHiuchi.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η��Α�")));
   //         }

            //�p�l���𓝍����Ȃ��ꍇ
            //PulldownButton groupJack = panelJack.AddItem(groupJackData) as PulldownButton;
            //PulldownButton groupAtamaTsunagi = panelAtamaTsunagi.AddItem(groupAtamaTsunagiData) as PulldownButton;
            //PulldownButton groupDesumibuHokyouzai = panelDesumibuHokyouzai.AddItem(groupDesumibuHokyouzaiData) as PulldownButton;
            //PulldownButton groupSumibuPiace = panelSumibuPiace.AddItem(groupSumibuPiaceData) as PulldownButton;
            //PulldownButton groupSendanBoltHokyouzai = panelSendanBoltHokyouzai.AddItem(groupSendanBoltHokyouzaiData) as PulldownButton;
            //PulldownButton groupHaraokoshiSuberidome = panelHaraokoshiSuberidome.AddItem(groupHaraokoshiSuberidomeData) as PulldownButton;

            //�p�l���𓝍�����ꍇ
            PulldownButton groupJack = panelBuhinHaici.AddItem(groupJackData) as PulldownButton;
            PulldownButton groupAtamaTsunagi = panelBuhinHaici.AddItem(groupAtamaTsunagiData) as PulldownButton;
            PulldownButton groupDesumibuHokyouzai = panelBuhinHaici.AddItem(groupDesumibuHokyouzaiData) as PulldownButton;
            PulldownButton groupSumibuPiace = panelBuhinHaici.AddItem(groupSumibuPiaceData) as PulldownButton;
            PulldownButton groupSendanBoltHokyouzai = panelBuhinHaici.AddItem(groupSendanBoltHokyouzaiData) as PulldownButton;
            PulldownButton groupHaraokoshiSuberidome = panelBuhinHaici.AddItem(groupHaraokoshiSuberidomeData) as PulldownButton;

            ///////////////////////////////////////////////////////////////////////////




            ///////////////////////////////////////////////////////////////////////////
            //PushButtonData pushButtonData2 = new PushButtonData("PushButton Command2", "����\n�L��",
            //                dllPath, "YMS.ChangeShikibetuSym");

            //PushButton pushButton2 = panelSyokisettei.AddItem(pushButtonData2) as PushButton;

            //pushButton2.ToolTip = "���ʋL����ύX���܂��B";
            //if (System.IO.File.Exists(GetIconPath(dllPath, "2 ���ʋL��")))
            //{
            //    pushButton2.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "2 ���ʋL��")));
            //    pushButton2.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "2 ���ʋL��")));
            //    pushButton2.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "2 ���ʋL��")));
            //}
            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData3 = new PushButtonData("PushButton Command3", "�ǐc\n",
                            dllPath, "YMS.CreateKabeshin");

            PushButton pushButton3 = panelKabeShin.AddItem(pushButtonData3) as PushButton;

            pushButton3.ToolTip = "�ǐc���쐬���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "4 �ǐc")))
            {
                pushButton3.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "4 �ǐc")));
                pushButton3.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "4 �ǐc")));
                pushButton3.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "4 �ǐc")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData4 = new PushButtonData("PushButton Command4", "�쐬",
                            dllPath, "YMS.CreateKouyaita");

            PushButton pushButton4 = groupYamadomeKouyaita.AddPushButton(pushButtonData4) as PushButton;

            pushButton4.ToolTip = "�|����쐬���܂��B";

            if (System.IO.File.Exists(GetIconPath(dllPath, "5 �|���-�쐬")))
            {
                pushButton4.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "5 �|���-�쐬")));
                pushButton4.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "5 �|���-�쐬")));
                pushButton4.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "5 �|���-�쐬")));
            }

            ///////////////////////////////////////////////////////////////////////////
            //PushButtonData pushButtonData5 = new PushButtonData("PushButton Command5", "�ύX",
            //                dllPath, "YMS.ChangeKouyaita");

            //PushButton pushButton5 = groupYamadomeKouyaita.AddPushButton(pushButtonData5) as PushButton;

            //pushButton5.ToolTip = "�|���ύX���܂��B";

            //if (System.IO.File.Exists(GetIconPath(dllPath, "5 �|���-�ύX")))
            //{
            //    pushButton5.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "5 �|���-�ύX")));
            //    pushButton5.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "5 �|���-�ύX")));
            //    pushButton5.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "5 �|���-�ύX")));
            //}

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData6 = new PushButtonData("PushButton Command6", "���]",
                            dllPath, "YMS.ReverseKouyaita");

            PushButton pushButton6 = groupYamadomeKouyaita.AddPushButton(pushButtonData6) as PushButton;

            pushButton6.ToolTip = "�|��𔽓]���܂��B";

            if (System.IO.File.Exists(GetIconPath(dllPath, "5 �|���-���]")))
            {
                pushButton6.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "5 �|���-���]")));
                pushButton6.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "5 �|���-���]")));
                pushButton6.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "5 �|���-���]")));
            }

            /////////////////////////////////////////////////////////////////////////////
            //PushButtonData pushButtonData7 = new PushButtonData("PushButton Command7", "����T�R�[�i�[�ϊ�",
            //                dllPath, "YMS.MergeKoyaita");

            //PushButton pushButton7 = groupYamadomeKouyaita.AddPushButton(pushButtonData7) as PushButton;

            //pushButton7.ToolTip = "����T�R�[�i�[�ϊ����s���܂��B";

            /////////////////////////////////////////////////////////////////////////////
            //groupYamadome.AddSeparator();//��؂��t����
            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData8 = new PushButtonData("PushButton Command8", "�쐬",
                            dllPath, "YMS.CreateOyakui");

            PushButton pushButton8 = groupYamadomeOyakui.AddPushButton(pushButtonData8) as PushButton;

            pushButton8.ToolTip = "�e�Y���쐬���܂��B";

            if (System.IO.File.Exists(GetIconPath(dllPath, "6 �e�Y-�쐬")))
            {
                pushButton8.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "6 �e�Y-�쐬")));
                pushButton8.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "6 �e�Y-�쐬")));
                pushButton8.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "6 �e�Y-�쐬")));
            }

            ///////////////////////////////////////////////////////////////////////////
            //PushButtonData pushButtonData9 = new PushButtonData("PushButton Command9", "�ύX",
            //                dllPath, "YMS.ChangeOyakui");

            //PushButton pushButton9 = groupYamadomeOyakui.AddPushButton(pushButtonData9) as PushButton;

            //pushButton9.ToolTip = "�e�Y��ύX���܂��B";

            //if (System.IO.File.Exists(GetIconPath(dllPath, "6 �e�Y-�ύX")))
            //{
            //    pushButton9.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "6 �e�Y-�ύX")));
            //    pushButton9.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "6 �e�Y-�ύX")));
            //    pushButton9.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "6 �e�Y-�ύX")));
            //}

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData10 = new PushButtonData("PushButton Command10", "�쐬",
                            dllPath, "YMS.CreateYokoyaita");

            PushButton pushButton10 = groupYamadomeYokoyaita.AddPushButton(pushButtonData10) as PushButton;

            pushButton10.ToolTip = "������쐬���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "7 �����-�쐬")))
            {
                pushButton10.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "7 �����-�쐬")));
                pushButton10.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "7 �����-�쐬")));
                pushButton10.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "7 �����-�쐬")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData11 = new PushButtonData("PushButton Command11", "�ύX",
                            dllPath, "YMS.ChangeYokoyaita");

            PushButton pushButton11 = groupYamadomeYokoyaita.AddPushButton(pushButtonData11) as PushButton;

            pushButton11.ToolTip = "�����ύX���܂��B";

            if (System.IO.File.Exists(GetIconPath(dllPath, "7 �����-�ύX")))
            {
                pushButton11.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "7 �����-�ύX")));
                pushButton11.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "7 �����-�ύX")));
                pushButton11.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "7 �����-�ύX")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData12 = new PushButtonData("PushButton Command12", "�쐬",
                            dllPath, "YMS.CreateRenzokuKabe");

            PushButton pushButton12 = groupYamadomeRenzokuKabe.AddPushButton(pushButtonData12) as PushButton;

            pushButton12.ToolTip = "�A���ǂ��쐬���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "8 �A����-�쐬")))
            {
                pushButton12.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "8 �A����-�쐬")));
                pushButton12.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "8 �A����-�쐬")));
                pushButton12.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "8 �A����-�쐬")));
            }

            ///////////////////////////////////////////////////////////////////////////
            //PushButtonData pushButtonData13 = new PushButtonData("PushButton Command13", "�ύX",
            //                dllPath, "YMS.ChangeRenzokuKabe");

            //PushButton pushButton13 = groupYamadomeRenzokuKabe.AddPushButton(pushButtonData13) as PushButton;

            //pushButton13.ToolTip = "�A���ǂ�ύX���܂��B";
            //if (System.IO.File.Exists(GetIconPath(dllPath, "8 �A����-�ύX")))
            //{
            //    pushButton13.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "8 �A����-�ύX")));
            //    pushButton13.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "8 �A����-�ύX")));
            //    pushButton13.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "8 �A����-�ύX")));
            //}

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData14 = new PushButtonData("PushButton Command14", "�쐬",
                            dllPath, "YMS.CreateSMW");

            PushButton pushButton14 = groupYamadomeSMW.AddPushButton(pushButtonData14) as PushButton;

            pushButton14.ToolTip = "SMW���쐬���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "9 SMW-�쐬")))
            {
                pushButton14.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "9 SMW-�쐬")));
                pushButton14.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "9 SMW-�쐬")));
                pushButton14.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "9 SMW-�쐬")));
            }

            ///////////////////////////////////////////////////////////////////////////
            //PushButtonData pushButtonData15 = new PushButtonData("PushButton Command15", "�ύX",
            //                dllPath, "YMS.ChangeSMW");

            //PushButton pushButton15 = groupYamadomeSMW.AddPushButton(pushButtonData15) as PushButton;

            //pushButton15.ToolTip = "SMW��ύX���܂��B";
            //if (System.IO.File.Exists(GetIconPath(dllPath, "9 SMW-�ύX")))
            //{
            //    pushButton15.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "9 SMW-�ύX")));
            //    pushButton15.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "9 SMW-�ύX")));
            //    pushButton15.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "9 SMW-�ύX")));
            //}

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData16 = new PushButtonData("PushButton Command16", "�{�C�h\n",
                            dllPath, "YMS.CreateVoidFamily");

            PushButton pushButton16 = panelYamdomeKabe.AddItem(pushButtonData16) as PushButton;

            pushButton16.ToolTip = "�ǂ�����ʂ��܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "10 �{�C�h-�쐬")))
            {
                pushButton16.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "10 �{�C�h-�쐬")));
                pushButton16.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "10 �{�C�h-�쐬")));
                pushButton16.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "10 �{�C�h-�쐬")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData17 = new PushButtonData("PushButton Command17", "CASE\n�Ǘ�",
                            dllPath, "YMS.ManegeCASE");

            PushButton pushButton17 = panelYamdomeKabe.AddItem(pushButtonData17) as PushButton;
            pushButton17.ToolTip = "CASE�Ǘ��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "11 CASE-�Ǘ�")))
            {
                pushButton17.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "11 CASE-�Ǘ�")));
                pushButton17.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "11 CASE-�Ǘ�")));
                pushButton17.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "11 CASE-�Ǘ�")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData18 = new PushButtonData("PushButton Command18", "�쐬",
                            dllPath, "YMS.CreateSanbashiKui");

            PushButton pushButton18 = groupKui.AddPushButton(pushButtonData18) as PushButton;

            pushButton18.ToolTip = "TC�Y�E�\��Y�E���p�Y�E�f�ʕω��Y���쐬���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "12 �Y-�쐬")))
            {
                pushButton18.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "12 �Y-�쐬")));
                pushButton18.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "12 �Y-�쐬")));
                pushButton18.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "12 �Y-�쐬")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData19 = new PushButtonData("PushButton Command19", "�ύX",
                            dllPath, "YMS.ChangeKui");

            PushButton pushButton19 = groupKui.AddPushButton(pushButtonData19) as PushButton;

            pushButton19.ToolTip = "TC�Y�E�\��Y�E���p�Y�E�f�ʕω��Y��ύX���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "12 �Y-�ύX")))
            {
                pushButton19.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "12 �Y-�ύX")));
                pushButton19.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "12 �Y-�ύX")));
                pushButton19.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "12 �Y-�ύX")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData20 = new PushButtonData("PushButton Command20", "�쐬",
                            dllPath, "YMS.CreateTanakui");

            PushButton pushButton20 = groupTanaKui.AddPushButton(pushButtonData20) as PushButton;

            pushButton20.ToolTip = "�I�Y�E���ԍY���쐬���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "13 ���Y-�쐬")))
            {
                pushButton20.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "13 ���Y-�쐬")));
                pushButton20.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "13 ���Y-�쐬")));
                pushButton20.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "13 ���Y-�쐬")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData21 = new PushButtonData("PushButton Command21", "�ύX",
                            dllPath, "YMS.ChangeTanakui");

            PushButton pushButton21 = groupTanaKui.AddPushButton(pushButtonData21) as PushButton;

            pushButton21.ToolTip = "�I�Y�E���ԍY��ύX���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "13 ���Y-�ύX")))
            {
                pushButton21.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "13 ���Y-�ύX")));
                pushButton21.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "13 ���Y-�ύX")));
                pushButton21.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "13 ���Y-�ύX")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData22 = new PushButtonData("PushButton Command22", "�쐬",
                            dllPath, "YMS.CreateHaraokoshiBase");

            PushButton pushButton22 = groupBaseHaraokoshi.AddPushButton(pushButtonData22) as PushButton;

            pushButton22.ToolTip = "���N�x�[�X���쐬���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 ���N")))
            {
                pushButton22.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 ���N")));
                pushButton22.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 ���N")));
                pushButton22.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 ���N")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData23 = new PushButtonData("PushButton Command23", "�ύX",
                            dllPath, "YMS.ChangeHaraokoshiBase");

            PushButton pushButton23 = groupBaseHaraokoshi.AddPushButton(pushButtonData23) as PushButton;

            pushButton23.ToolTip = "���N�x�[�X��ύX���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 ���N")))
            {
                pushButton23.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 ���N")));
                pushButton23.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 ���N")));
                pushButton23.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 ���N")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData24 = new PushButtonData("PushButton Command24", "�쐬",
                            dllPath, "YMS.CreateKiribariBase");

            PushButton pushButton24 = groupBaseKiribari.AddPushButton(pushButtonData24) as PushButton;

            pushButton24.ToolTip = "�ؗ��x�[�X���쐬���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 �ؗ�")))
            {
                pushButton24.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ�")));
                pushButton24.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ�")));
                pushButton24.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ�")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData25 = new PushButtonData("PushButton Command25", "�ύX",
                            dllPath, "YMS.ChangeKiribariBase");

            PushButton pushButton25 = groupBaseKiribari.AddPushButton(pushButtonData25) as PushButton;

            pushButton25.ToolTip = "�ؗ��x�[�X��ύX���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 �ؗ�")))
            {
                pushButton25.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ�")));
                pushButton25.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ�")));
                pushButton25.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ�")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData26 = new PushButtonData("PushButton Command26", "�z�u",
                            dllPath, "YMS.PutSanjikuPiece");

            PushButton pushButton26 = groupBaseSanjiku.AddPushButton(pushButtonData26) as PushButton;

            pushButton26.ToolTip = "�O���s�[�X��z�u���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 �O��")))
            {
                pushButton26.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �O��")));
                pushButton26.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �O��")));
                pushButton26.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �O��")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData27 = new PushButtonData("PushButton Command27", "�ʒu����",
                            dllPath, "YMS.PositionSanjikuPiece");

            PushButton pushButton27 = groupBaseSanjiku.AddPushButton(pushButtonData27) as PushButton;

            pushButton27.ToolTip = "�O���s�[�X�̈ʒu�𒲐����܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 �O��")))
            {
                pushButton27.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �O��")));
                pushButton27.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �O��")));
                pushButton27.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �O��")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData28 = new PushButtonData("PushButton Command28", "�쐬",
                            dllPath, "YMS.CreateCornerHiuchiBase");

            PushButton pushButton28 = groupBasesumibu.AddPushButton(pushButtonData28) as PushButton;

            pushButton28.ToolTip = "���ΑŃx�[�X���쐬���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 ���Α�")))
            {
                pushButton28.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 ���Α�")));
                pushButton28.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 ���Α�")));
                pushButton28.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 ���Α�")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData29 = new PushButtonData("PushButton Command29", "�ύX",
                            dllPath, "YMS.ChangeCornerHiuchiBase");

            PushButton pushButton29 = groupBasesumibu.AddPushButton(pushButtonData29) as PushButton;

            pushButton29.ToolTip = "���ΑŃx�[�X��ύX���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 ���Α�")))
            {
                pushButton29.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 ���Α�")));
                pushButton29.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 ���Α�")));
                pushButton29.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 ���Α�")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData30 = new PushButtonData("PushButton Command30", "�쐬",
                            dllPath, "YMS.CreateKiribariHiuchiBase");

            PushButton pushButton30 = groupBaseKiribarHiuchi.AddPushButton(pushButtonData30) as PushButton;

            pushButton30.ToolTip = "�ؗ��ΑŃx�[�X���쐬���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 �ؗ��Α�")))
            {
                pushButton30.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ��Α�")));
                pushButton30.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ��Α�")));
                pushButton30.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ��Α�")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData31 = new PushButtonData("PushButton Command31", "�ύX",
                            dllPath, "YMS.ChangeKiribariHiuchiBase");

            PushButton pushButton31 = groupBaseKiribarHiuchi.AddPushButton(pushButtonData31) as PushButton;

            pushButton31.ToolTip = "�ؗ��ΑŃx�[�X��ύX���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 �ؗ��Α�")))
            {
                pushButton31.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ��Α�")));
                pushButton31.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ��Α�")));
                pushButton31.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ��Α�")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData32 = new PushButtonData("PushButton Command32", "�쐬",
                            dllPath, "YMS.CreateKiribariUkeBase");

            PushButton pushButton32 = groupBaseKiribariUke.AddPushButton(pushButtonData32) as PushButton;

            pushButton32.ToolTip = "�ؗ��󂯃x�[�X���쐬���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 �ؗ���")))
            {
                pushButton32.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ���")));
                pushButton32.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ���")));
                pushButton32.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ���")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData33 = new PushButtonData("PushButton Command33", "�ύX",
                            dllPath, "YMS.ChangeKiribariUkeBase");

            PushButton pushButton33 = groupBaseKiribariUke.AddPushButton(pushButtonData33) as PushButton;

            pushButton33.ToolTip = "�ؗ��󂯃x�[�X��ύX���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 �ؗ���")))
            {
                pushButton33.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ���")));
                pushButton33.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ���")));
                pushButton33.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ���")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData34 = new PushButtonData("PushButton Command34", "�쐬",
                            dllPath, "YMS.CreateKiribariTsunagizaiBase");

            PushButton pushButton34 = groupBaseKiribariTsunagizai.AddPushButton(pushButtonData34) as PushButton;

            pushButton34.ToolTip = "�ؗ��c�i�M�ރx�[�X���쐬���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 �ؗ��c�i�M")))
            {
                pushButton34.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ��c�i�M")));
                pushButton34.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ��c�i�M")));
                pushButton34.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ��c�i�M")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData35 = new PushButtonData("PushButton Command35", "�ύX",
                            dllPath, "YMS.ChangeKiribariTsunagizaiBase");

            PushButton pushButton35 = groupBaseKiribariTsunagizai.AddPushButton(pushButtonData35) as PushButton;

            pushButton35.ToolTip = "�ؗ��c�i�M�ރx�[�X��ύX���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 �ؗ��c�i�M")))
            {
                pushButton35.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ��c�i�M")));
                pushButton35.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ��c�i�M")));
                pushButton35.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ��c�i�M")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData36 = new PushButtonData("PushButton Command36", "�쐬",
                            dllPath, "YMS.CreateHiuchiTsunagizaiBase");

            PushButton pushButton36 = groupBaseHiuchiTsunagizai.AddPushButton(pushButtonData36) as PushButton;

            pushButton36.ToolTip = "�ΑŃc�i�M�ރx�[�X���쐬���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 �ΑŃc�i�M")))
            {
                pushButton36.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ΑŃc�i�M")));
                pushButton36.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ΑŃc�i�M")));
                pushButton36.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ΑŃc�i�M")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData37 = new PushButtonData("PushButton Command37", "�ύX",
                            dllPath, "YMS.ChangeHIuchiTsunagizaiBase");

            PushButton pushButton37 = groupBaseHiuchiTsunagizai.AddPushButton(pushButtonData37) as PushButton;

            pushButton37.ToolTip = "�ΑŃc�i�M�ރx�[�X��ύX���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 �ΑŃc�i�M")))
            {
                pushButton37.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ΑŃc�i�M")));
                pushButton37.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ΑŃc�i�M")));
                pushButton37.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ΑŃc�i�M")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData38 = new PushButtonData("PushButton Command38", "�쐬",
                            dllPath, "YMS.CreateKiribariTsunagiBase");

            PushButton pushButton38 = groupBaseKiribariTsunagi.AddPushButton(pushButtonData38) as PushButton;

            pushButton38.ToolTip = "�ؗ��p���x�[�X���쐬���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 �ؗ��p��")))
            {
                pushButton38.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ��p��")));
                pushButton38.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ��p��")));
                pushButton38.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ��p��")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData39 = new PushButtonData("PushButton Command39", "�ύX",
                            dllPath, "YMS.ChangeKiribariTsunagiBase");

            PushButton pushButton39 = groupBaseKiribariTsunagi.AddPushButton(pushButtonData39) as PushButton;

            pushButton39.ToolTip = "�ؗ��p���x�[�X��ύX���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 �ؗ��p��")))
            {
                pushButton39.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ��p��")));
                pushButton39.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ��p��")));
                pushButton39.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �ؗ��p��")));
            }

            /////////////////////////////////////////////////////////////////////////////
            //PushButtonData pushButtonData82 = new PushButtonData("PushButton Command82", "��̗p�����z�u",
            //                dllPath, "YMS.PutSyabariStructureLine");

            //PushButton pushButton82 = groupBaseSyabari.AddPushButton(pushButtonData82) as PushButton;

            //pushButton82.ToolTip = "�Η��̋�̗p������z�u���܂��B";
            //if (System.IO.File.Exists(GetIconPath(dllPath, "14 �Η�")))
            //{
            //    pushButton82.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η�")));
            //    pushButton82.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η�")));
            //    pushButton82.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η�")));
            //}

            /////////////////////////////////////////////////////////////////////////////
            //PushButtonData pushButtonData71 = new PushButtonData("PushButton Command71", "�[�����i�z�u�i�ʁj",
            //                dllPath, "YMS.PutSyabariPieceIndividual");

            //PushButton pushButton71 = groupBaseSyabari.AddPushButton(pushButtonData71) as PushButton;

            //pushButton71.ToolTip = "�Η��̒[�����i���ʔz�u���܂��B";
            //if (System.IO.File.Exists(GetIconPath(dllPath, "14 �Η�")))
            //{
            //    pushButton71.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η�")));
            //    pushButton71.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η�")));
            //    pushButton71.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η�")));
            //}

            /////////////////////////////////////////////////////////////////////////////
            //PushButtonData pushButtonData81 = new PushButtonData("PushButton Command81", "�[�����i�z�u�i��_�j",
            //                dllPath, "YMS.PutSyabariPieceIntersectionPoint");

            //PushButton pushButton81 = groupBaseSyabari.AddPushButton(pushButtonData81) as PushButton;

            //pushButton81.ToolTip = "���N�x�[�X�ƃ��f�������̌�_�ɎΗ��̒[�����i��z�u���܂��B";
            //if (System.IO.File.Exists(GetIconPath(dllPath, "14 �Η�")))
            //{
            //    pushButton81.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η�")));
            //    pushButton81.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η�")));
            //    pushButton81.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η�")));
            //}

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData72 = new PushButtonData("PushButton Command72", "�Η��쐬",
                            dllPath, "YMS.CreateSyabariBase");

            PushButton pushButton72 = groupBaseSyabari.AddPushButton(pushButtonData72) as PushButton;

            pushButton72.ToolTip = "�Η��x�[�X���쐬���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 �Η�")))
            {
                pushButton72.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η�")));
                pushButton72.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η�")));
                pushButton72.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η�")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData73 = new PushButtonData("PushButton Command73", "�Η��ύX",
                            dllPath, "YMS.ChangeSyabariBase");

            PushButton pushButton73 = groupBaseSyabari.AddPushButton(pushButtonData73) as PushButton;

            pushButton73.ToolTip = "�Η��x�[�X��ύX���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 �Η�")))
            {
                pushButton73.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η�")));
                pushButton73.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η�")));
                pushButton73.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η�")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData79 = new PushButtonData("PushButton Command79", "�Η��Αō쐬",
                            dllPath, "YMS.CreateSyabariHiuchiBase");

            PushButton pushButton79 = groupBaseSyabari.AddPushButton(pushButtonData79) as PushButton;
            //PushButton pushButton79 = groupBaseSyabariHiuchi.AddPushButton(pushButtonData79) as PushButton;

            pushButton79.ToolTip = "�Η��ΑŃx�[�X���쐬���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 �Η��Α�")))
            {
                pushButton79.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η��Α�")));
                pushButton79.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η��Α�")));
                pushButton79.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η��Α�")));
                //pushButton79.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η��Α�")));
                //pushButton79.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η��Α�")));
                //pushButton79.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η��Α�")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData80 = new PushButtonData("PushButton Command80", "�Η��ΑŕύX",
                            dllPath, "YMS.ChangeSyabariHiuchiBase");

            PushButton pushButton80 = groupBaseSyabari.AddPushButton(pushButtonData80) as PushButton;
            //PushButton pushButton80 = groupBaseSyabariHiuchi.AddPushButton(pushButtonData80) as PushButton;

            pushButton80.ToolTip = "�Η��ΑŃx�[�X��ύX���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 �Η��Α�")))
            {
                pushButton80.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η��Α�")));
                pushButton80.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η��Α�")));
                pushButton80.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η��Α�")));
                //pushButton80.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η��Α�")));
                //pushButton80.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η��Α�")));
                //pushButton80.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η��Α�")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData74 = new PushButtonData("PushButton Command74", "�쐬",
                            dllPath, "YMS.CreateSyabariTsunagizaiBase");

            PushButton pushButton74 = groupBaseSyabariTsunagizai.AddPushButton(pushButtonData74) as PushButton;

            pushButton74.ToolTip = "�Η��c�i�M�ރx�[�X���쐬���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 �Η��c�i�M��")))
            {
                pushButton74.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η��c�i�M��")));
                pushButton74.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η��c�i�M��")));
                pushButton74.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η��c�i�M��")));
            }

			///////////////////////////////////////////////////////////////////////////
			PushButtonData pushButtonData75 = new PushButtonData("PushButton Command75", "�ύX",
							dllPath, "YMS.ChangeSyabariTsunagizaiBase");

			PushButton pushButton75 = groupBaseSyabariTsunagizai.AddPushButton(pushButtonData75) as PushButton;

            pushButton75.ToolTip = "�Η��c�i�M�ރx�[�X��ύX���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 �Η��c�i�M��")))
            {
                pushButton75.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η��c�i�M��")));
                pushButton75.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η��c�i�M��")));
                pushButton75.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η��c�i�M��")));
            }

			///////////////////////////////////////////////////////////////////////////
			PushButtonData pushButtonData76 = new PushButtonData("PushButton Command76", "�쐬",
                            dllPath, "YMS.CreateSyabariUkeBase");

            PushButton pushButton76 = groupBaseSyabariUke.AddPushButton(pushButtonData76) as PushButton;

            pushButton76.ToolTip = "�Η��󂯃x�[�X���쐬���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 �Η���")))
            {
                pushButton76.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η���")));
                pushButton76.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η���")));
                pushButton76.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η���")));
            }

			///////////////////////////////////////////////////////////////////////////
			PushButtonData pushButtonData77 = new PushButtonData("PushButton Command77", "�ύX",
							dllPath, "YMS.ChangeSyabariUkeBase");

			PushButton pushButton77 = groupBaseSyabariUke.AddPushButton(pushButtonData77) as PushButton;

            pushButton77.ToolTip = "�Η��󂯃x�[�X��ύX���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "14 �Η���")))
            {
                pushButton77.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η���")));
                pushButton77.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η���")));
                pushButton77.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η���")));
            }

            /////////////////////////////////////////////////////////////////////////////
            //PushButtonData pushButtonData78 = new PushButtonData("PushButton Command78", "�[�����i�z�u",
            //                         dllPath, "YMS.PutSyabariHiuchiPiece");

            //         PushButton pushButton78 = groupBaseSyabari.AddPushButton(pushButtonData78) as PushButton;
            ////         PushButton pushButton78 = groupBaseSyabariHiuchi.AddPushButton(pushButtonData78) as PushButton;

            //         pushButton78.ToolTip = "�Η��Αł̒[�����i��z�u���܂��B";
            //         if (System.IO.File.Exists(GetIconPath(dllPath, "14 �Η��Α�")))
            //         {
            //             pushButton78.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η��Α�")));
            //             pushButton78.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η��Α�")));
            //             pushButton78.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "14 �Η��Α�")));
            //         }

            ///////////////////////////////////////////////////////////////////////////
            //PushButtonData pushButtonData40 = new PushButtonData("PushButton Command40", "���x�[�X�쐬",
            //                dllPath, "YMS.CreateKariBase");

            //PushButton pushButton40 = panelBase.AddItem(pushButtonData40) as PushButton;

            //pushButton40.ToolTip = "���x�[�X���쐬���܂��B";

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData101 = new PushButtonData("PushButton Command101", "�x�[�X�R�s�[",
                            dllPath, "YMS.CopyBase");

            PushButton pushButton101 = panelBase.AddItem(pushButtonData101) as PushButton;

            pushButton101.ToolTip = "�x�[�X�̃R�s�[���s���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "�x�[�X�R�s�[")))
            {
                pushButton101.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "�x�[�X�R�s�[")));
                pushButton101.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "�x�[�X�R�s�[")));
                pushButton101.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "�x�[�X�R�s�[")));
            }


            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData41 = new PushButtonData("PushButton Command41", "�x�[�X\n�ꗗ�E�ҏW",
                            dllPath, "YMS.EditBaseList");

            PushButton pushButton41 = panelBase.AddItem(pushButtonData41) as PushButton;

            pushButton41.ToolTip = "�x�[�X�ꗗ�E�ҏW���s���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "15 �x�[�X�ꗗ")))
            {
                pushButton41.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "15 �x�[�X�ꗗ")));
                pushButton41.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "15 �x�[�X�ꗗ")));
                pushButton41.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "15 �x�[�X�ꗗ")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData42 = new PushButtonData("PushButton Command42", "�쐬",
                            dllPath, "YMS.CreateJack");

            PushButton pushButton42 = groupJack.AddPushButton(pushButtonData42) as PushButton;

            pushButton42.ToolTip = "�W���b�L���쐬���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "16 �W���b�L-�쐬")))
            {
                pushButton42.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "16 �W���b�L-�쐬")));
                pushButton42.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "16 �W���b�L-�쐬")));
                pushButton42.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "16 �W���b�L-�쐬")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData43 = new PushButtonData("PushButton Command43", "�ύX",
                            dllPath, "YMS.Changejack");

            PushButton pushButton43 = groupJack.AddPushButton(pushButtonData43) as PushButton;

            pushButton43.ToolTip = "�W���b�L��ύX���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "16 �W���b�L-�ύX")))
            {
                pushButton43.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "16 �W���b�L-�ύX")));
                pushButton43.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "16 �W���b�L-�ύX")));
                pushButton43.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "16 �W���b�L-�ύX")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData44 = new PushButtonData("PushButton Command44", "���c�i�M�ލ쐬",
                            dllPath, "YMS.CreateAtamaTsunagiZai");

            //SplitButtonData sb1 = new SplitButtonData("atamaTsunagi", "atamaTsunagi");
            //SplitButton sb = panelAtamaTsunagi.AddItem(sb1) as SplitButton;
            //PushButton pushButton44 = sb.AddPushButton(pushButtonData44) as PushButton;
            PushButton pushButton44 = groupAtamaTsunagi.AddPushButton(pushButtonData44) as PushButton;

            pushButton44.ToolTip = "���c�i�M�ނ��쐬���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "17 ���c�i�M-�쐬")))
            {
                pushButton44.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "17 ���c�i�M-�쐬")));
                pushButton44.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "17 ���c�i�M-�쐬")));
                pushButton44.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "17 ���c�i�M-�쐬")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData70 = new PushButtonData("PushButton Command70", "���c�i�M�⏕�ލ쐬",
                            dllPath, "YMS.CreateAtamaTsunagiHojoZai");

            PushButton pushButton70 = groupAtamaTsunagi.AddPushButton(pushButtonData70) as PushButton;

            pushButton70.ToolTip = "���c�i�M�⏕�ނ�ݒ肵�܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "17 ���c�i�M-�쐬")))
            {
                pushButton70.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "17 ���c�i�M-�쐬")));
                pushButton70.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "17 ���c�i�M-�쐬")));
                pushButton70.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "17 ���c�i�M-�쐬")));
            }

            ///////////////////////////////////////////////////////////////////////////

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData45 = new PushButtonData("PushButton Command45", "���c�i�M�ލ폜",
                            dllPath, "YMS.DeleteAtamaTsunagi");

            PushButton pushButton45 = groupAtamaTsunagi.AddPushButton(pushButtonData45) as PushButton;

            pushButton45.ToolTip = "���c�i�M�ނ��폜���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "17 ���c�i�M-�ύX"))) 
            {
                pushButton45.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "17 ���c�i�M-�ύX")));
                pushButton45.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "17 ���c�i�M-�ύX")));
                pushButton45.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "17 ���c�i�M-�ύX")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData46 = new PushButtonData("PushButton Command46", "�쐬",
                            dllPath, "YMS.CreateDesumibuHokyouzai");

            PushButton pushButton46 = groupDesumibuHokyouzai.AddPushButton(pushButtonData46) as PushButton;

            pushButton46.ToolTip = "�o�����⋭�ނ��쐬���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "17 �o��-�쐬")))
            {
                pushButton46.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "17 �o��-�쐬")));
                pushButton46.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "17 �o��-�쐬")));
                pushButton46.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "17 �o��-�쐬")));
            }

            ///////////////////////////////////////////////////////////////////////////
            //PushButtonData pushButtonData47 = new PushButtonData("PushButton Command47", "�ύX",
            //                dllPath, "YMS.ChangeDesumibuHokyouzai");

            //PushButton pushButton47 = groupDesumibuHokyouzai.AddPushButton(pushButtonData47) as PushButton;

            //pushButton47.ToolTip = "�o�����⋭�ނ�ύX���܂��B";
            //if (System.IO.File.Exists(GetIconPath(dllPath, "17 �o��-�ύX")))
            //{
            //    pushButton47.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "17 �o��-�ύX")));
            //    pushButton47.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "17 �o��-�ύX")));
            //    pushButton47.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "17 �o��-�ύX")));
            //}

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData48 = new PushButtonData("PushButton Command48", "���|��\n�z�u",
                            dllPath, "YMS.PutKariKouzai");

            //PushButton pushButton48 = panelKariKouzai.AddItem(pushButtonData48) as PushButton;
            //�p�l���𓝍�����ꍇ
            PushButton pushButton48 = panelBuhinHaici.AddItem(pushButtonData48) as PushButton;

            pushButton48.ToolTip = "���|�ނ�z�u���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "29���|�ޔz�u")))
            {
                pushButton48.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "29���|�ޔz�u")));
                pushButton48.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "29���|�ޔz�u")));
                pushButton48.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "29���|�ޔz�u")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData49 = new PushButtonData("PushButton Command49", "�쐬",
                            dllPath, "YMS.CreateSumibuPiace");

            PushButton pushButton49 = groupSumibuPiace.AddPushButton(pushButtonData49) as PushButton;

            pushButton49.ToolTip = "�����s�[�X���쐬���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "19 ����-�쐬")))
            {
                pushButton49.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "19 ����-�쐬")));
                pushButton49.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "19 ����-�쐬")));
                pushButton49.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "19 ����-�쐬")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData50 = new PushButtonData("PushButton Command50", "�ύX",
                            dllPath, "YMS.ChangeSumibuPiace");

            PushButton pushButton50 = groupSumibuPiace.AddPushButton(pushButtonData50) as PushButton;

            pushButton50.ToolTip = "�����s�[�X��ύX���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "19 ����-�ύX")))
            {
                pushButton50.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "19 ����-�ύX")));
                pushButton50.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "19 ����-�ύX")));
                pushButton50.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "19 ����-�ύX")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData51 = new PushButtonData("PushButton Command51", "�u���P�b�g\n�쐬",
                            dllPath, "YMS.CreateBracket");

            //PushButton pushButton51 = panelBracket.AddItem(pushButtonData51) as PushButton;
            //�p�l���𓝍�����ꍇ
            PushButton pushButton51 = panelBuhinHaici.AddItem(pushButtonData51) as PushButton;

            pushButton51.ToolTip = "�u���P�b�g���쐬���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "20 �u���P�b�g-�쐬")))
            {
                pushButton51.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "20 �u���P�b�g-�쐬")));
                pushButton51.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "20 �u���P�b�g-�쐬")));
                pushButton51.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "20 �u���P�b�g-�쐬")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData52 = new PushButtonData("PushButton Command52", "�쐬",
                            dllPath, "YMS.CreateSendanBoltHokyouzai");

            PushButton pushButton52 = groupSendanBoltHokyouzai.AddPushButton(pushButtonData52) as PushButton;

            pushButton52.ToolTip = "����f�{���g�⋭�ނ��쐬���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "21 �{���g-�쐬")))
            {
                pushButton52.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "21 �{���g-�쐬")));
                pushButton52.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "21 �{���g-�쐬")));
                pushButton52.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "21 �{���g-�쐬")));
            }

            ///////////////////////////////////////////////////////////////////////////
            //PushButtonData pushButtonData53 = new PushButtonData("PushButton Command53", "�ύX",
            //                dllPath, "YMS.ChangeSendanBoltHokyouzai");

            //PushButton pushButton53 = groupSendanBoltHokyouzai.AddPushButton(pushButtonData53) as PushButton;

            //pushButton53.ToolTip = "����f�{���g�⋭�ނ�ύX���܂��B";
            //if (System.IO.File.Exists(GetIconPath(dllPath, "21 �{���g-�ύX")))
            //{
            //    pushButton53.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "21 �{���g-�ύX")));
            //    pushButton53.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "21 �{���g-�ύX")));
            //    pushButton53.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "21 �{���g-�ύX")));
            //}

            /////////////////////////////////////////////////////////////////////////////
            //PushButtonData pushButtonData54 = new PushButtonData("PushButton Command54", "�쐬",
            //                dllPath, "YMS.CreateJyougeHaraokoshiTsunagi");

            //PushButton pushButton54 = groupJyougeHaraokoshiTsunagi.AddPushButton(pushButtonData54) as PushButton;

            //pushButton54.ToolTip = "�㉺���N�c�i�M�ނ��쐬���܂��B";

            /////////////////////////////////////////////////////////////////////////////
            //PushButtonData pushButtonData55 = new PushButtonData("PushButton Command55", "�ύX",
            //                dllPath, "YMS.ChangeJyougeHaraokoshiTsunagi");

            //PushButton pushButton55 = groupJyougeHaraokoshiTsunagi.AddPushButton(pushButtonData55) as PushButton;

            //pushButton55.ToolTip = "�㉺���N�c�i�M�ނ�ύX���܂��B";

            /////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData56 = new PushButtonData("PushButton Command56", "�쐬",
                            dllPath, "YMS.CreateHaraokoshiSuberidome");

            PushButton pushButton56 = groupHaraokoshiSuberidome.AddPushButton(pushButtonData56) as PushButton;

            pushButton56.ToolTip = "���N�X�x���~�߂��쐬���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "23�@���N�X�x��-�쐬")))
            {
                pushButton56.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "23�@���N�X�x��-�쐬")));
                pushButton56.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "23�@���N�X�x��-�쐬")));
                pushButton56.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "23�@���N�X�x��-�쐬")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData57 = new PushButtonData("PushButton Command57", "�ύX",
                            dllPath, "YMS.ChangeHaraokoshiSuberidome");

            PushButton pushButton57 = groupHaraokoshiSuberidome.AddPushButton(pushButtonData57) as PushButton;

            pushButton57.ToolTip = "���N�X�x���~�߂�ύX���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "23�@���N�X�x��-�ύX")))
            {
                pushButton57.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "23�@���N�X�x��-�ύX")));
                pushButton57.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "23�@���N�X�x��-�ύX")));
                pushButton57.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "23�@���N�X�x��-�ύX")));
            }

            ///////////////////////////////////////////////////////////////////////////
            //PushButtonData pushButtonData58 = new PushButtonData("PushButton Command58", "������\n����\n����\n�쐬",
            //                dllPath, "YMS.CreateKousyabuMawari");
            PushButtonData pushButtonData58 = new PushButtonData("PushButton Command58", "����������\n�����쐬",
                            dllPath, "YMS.CreateKousyabuMawari");

            //PushButton pushButton58 = panelKousyabuMawari.AddItem(pushButtonData58) as PushButton;
            //�p�l���𓝍�����ꍇ
            PushButton pushButton58 = panelBuhinHaici.AddItem(pushButtonData58) as PushButton;

            pushButton58.ToolTip = "����������������쐬���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "24 ����")))
            {
                pushButton58.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "24 ����")));
                pushButton58.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "24 ����")));
                pushButton58.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "24 ����")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData65 = new PushButtonData("PushButton Command65", "���t�p�s�[�X\n�쐬",
                            dllPath, "YMS.CreateShimetukePiece");

            PushButton pushButton65 = panelBuhinHaici.AddItem(pushButtonData65) as PushButton;

            pushButton65.ToolTip = "���t�p�s�[�X���쐬���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "30 ���t�p�s�[�X�쐬")))
            {
                pushButton65.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "30 ���t�p�s�[�X�쐬")));
                pushButton65.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "30 ���t�p�s�[�X�쐬")));
                pushButton65.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "30 ���t�p�s�[�X�쐬")));
            }

            /////////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData59 = new PushButtonData("PushButton Command59", "�X�`�t�i�[\n�쐬",
                            dllPath, "YMS.CreateStiffener");

            //PushButton pushButton59 = panelStiffener.AddItem(pushButtonData59) as PushButton;
            //�p�l���𓝍�����ꍇ
            PushButton pushButton59 = panelBuhinHaici.AddItem(pushButtonData59) as PushButton;

            pushButton59.ToolTip = "�X�`�t�i�[���쐬���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "25 �X�`�t�i�[")))
            {
                pushButton59.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "25 �X�`�t�i�[")));
                pushButton59.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "25 �X�`�t�i�[")));
                pushButton59.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "25 �X�`�t�i�[")));
            }

            ///////////////////////////////////////////////////////////////////////////
            PushButtonData pushButtonData66 = new PushButtonData("PushButton Command66", "����\n���t",
                           dllPath, "YMS.WaritukeJidou");

            //PushButton pushButton60 = panelWaritukeSyudou.AddItem(pushButtonData60) as PushButton;
            //�p�l���𓝍�����ꍇ
            PushButton pushButton66 = panelBuhinHaici.AddItem(pushButtonData66) as PushButton;

            pushButton66.ToolTip = "���|�ނ̊��t���ނւ̎����u�����s���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "31�������t")))
            {
                pushButton66.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "31�������t")));
                pushButton66.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "31�������t")));
                pushButton66.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "31�������t")));
            }



            PushButtonData pushButtonData60 = new PushButtonData("PushButton Command60", "����\n���t",
                            dllPath, "YMS.WaritukeSyudou");

            //PushButton pushButton60 = panelWaritukeSyudou.AddItem(pushButtonData60) as PushButton;
            //�p�l���𓝍�����ꍇ
            PushButton pushButton60 = panelBuhinHaici.AddItem(pushButtonData60) as PushButton;

            pushButton60.ToolTip = "���ފ��t���s���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "26 �蓮���t")))
            {
                pushButton60.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "26 �蓮���t")));
                pushButton60.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "26 �蓮���t")));
                pushButton60.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "26 �蓮���t")));
            }

            PushButtonData pushButtonData67 = new PushButtonData("PushButton Command67", "��ލ폜",
                          dllPath, "YMS.DeleteWarituke");

            //PushButton pushButton60 = panelWaritukeSyudou.AddItem(pushButtonData60) as PushButton;
            //�p�l���𓝍�����ꍇ
            PushButton pushButton67 = panelBuhinHaici.AddItem(pushButtonData67) as PushButton;

            pushButton67.ToolTip = "��ނ̑I���폜���s���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "32�@���t�폜")))
            {
                pushButton67.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "32�@���t�폜")));
                pushButton67.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "32�@���t�폜")));
                pushButton67.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "32�@���t�폜")));
            }

            ///////////////////////////////////////////////////////////////////////////
            //PushButtonData pushButtonData61 = new PushButtonData("PushButton Command61", "�|��\n����\n�ύX",
            //                dllPath, "YMS.ChangeLengthKouzai");
            PushButtonData pushButtonData61 = new PushButtonData("PushButton Command61", "�|�ޒ���\n�ύX",
                            dllPath, "YMS.ChangeLengthKouzai");

            //PushButton pushButton61 = panelLengthKouzai.AddItem(pushButtonData61) as PushButton;
            //�p�l���𓝍�����ꍇ
            PushButton pushButton61 = panelBuhinHaici.AddItem(pushButtonData61) as PushButton;

            pushButton61.ToolTip = "�|�ޒ�����ύX���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "27 �|�ޒ���")))
            {
                pushButton61.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "27 �|�ޒ���")));
                pushButton61.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "27 �|�ޒ���")));
                pushButton61.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "27 �|�ޒ���")));
            }

            /////////////////////////////////////////////////////////////////////////////
            ////PushButtonData pushButtonData62 = new PushButtonData("PushButton Command62", "���N\n�u���P�b�g\n�쐬",
            ////                dllPath, "YMS.CreateHaraokoshiBracket");
            //PushButtonData pushButtonData62 = new PushButtonData("PushButton Command62", "���N�u���P�b�g\n�쐬",
            //                dllPath, "YMS.CreateHaraokoshiBracket");

            ////PushButton pushButton62 = panelHaraokoshiBracket.AddItem(pushButtonData62) as PushButton;
            ////�p�l���𓝍�����ꍇ
            //PushButton pushButton62 = panelBuhinHaici.AddItem(pushButtonData62) as PushButton;

            //pushButton62.ToolTip = "���N�u���P�b�g���쐬���܂��B";
            //if (System.IO.File.Exists(GetIconPath(dllPath, "28 ���N���u���P�b�g")))
            //{
            //    pushButton62.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "28 ���N���u���P�b�g")));
            //    pushButton62.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "28 ���N���u���P�b�g")));
            //    pushButton62.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "28 ���N���u���P�b�g")));
            //}

            ///////////////////////////////////////////////////////////////////////////
            //PushButtonData pushButtonData63 = new PushButtonData("PushButton Command63", "���[�N�Z�b�g\n�쐬",
            //                dllPath, "YMS.CreateWorkset");

            //PushButton pushButton63 = panelworkset.AddItem(pushButtonData63) as PushButton;

            //pushButton63.ToolTip = "���[�N�Z�b�g���쐬���܂��B";
            //if (System.IO.File.Exists(GetIconPath(dllPath, "3 ���[�N�Z�b�g")))
            //{
            //    pushButton63.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "3 ���[�N�Z�b�g")));
            //    pushButton63.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "3 ���[�N�Z�b�g")));
            //    pushButton63.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "3 ���[�N�Z�b�g")));
            //}


            ///////////////////////////////////////////////////////////////////////////

            PushButtonData pushButtonData64 = new PushButtonData("PushButton Command64", "�ʔz�u",
                           dllPath, "YMS.KobetsuHaichi");

            //PushButton pushButton62 = panelHaraokoshiBracket.AddItem(pushButtonData62) as PushButton;
            //�p�l���𓝍�����ꍇ
            PushButton pushButton64 = panelBuhinHaici.AddItem(pushButtonData64) as PushButton;

            pushButton64.ToolTip = "�e���ނ̌ʔz�u���s���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "33 �ʔz�u")))
            {
                pushButton64.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "33 �ʔz�u")));
                pushButton64.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "33 �ʔz�u")));
                pushButton64.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "33 �ʔz�u")));
            }




            PushButtonData pushButtonData100 = new PushButtonData("PushButton Command100", "�ؗ����t�R�s�[",
                          dllPath, "YMS.KiribariCopy");

            PushButton pushButton100 = panelBuhinHaici.AddItem(pushButtonData100) as PushButton;

            pushButton100.ToolTip = "����t�����ؗ����ނ̃R�s�[���s���܂��B";
            if (System.IO.File.Exists(GetIconPath(dllPath, "34 �ؗ����t�R�s�[")))
            {
                pushButton100.LargeImage = new BitmapImage(new Uri(GetIconPath(dllPath, "34 �ؗ����t�R�s�[")));
                pushButton100.Image = new BitmapImage(new Uri(GetIconPath(dllPath, "34 �ؗ����t�R�s�[")));
                pushButton100.ToolTipImage = new BitmapImage(new Uri(GetIconPath(dllPath, "34 �ؗ����t�R�s�[")));
            }

        }

        /// <summary>
        /// �A�C�R���̃t���p�X�擾
        /// </summary>
        /// <param name="dllPath">�A�h�C���̏ꏊ</param>
        /// <param name="name">�A�C�R�����i�g���q�Ȃ��j</param>
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
