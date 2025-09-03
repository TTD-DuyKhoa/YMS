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
        /// NLog �̏���������(NLog.config �Ǎ�)
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
            app.CreateRibbonTab("YMS�\��");

            RibbonPanel panelAllPut = app.CreateRibbonPanel("YMS�\��", "�ꊇ�z�u");

            // [�ꊇ�z�u] �\��(�t���b�g)
            CreatePushButton(panelAllPut, "CmdAllPut1", "�t���b�g", "AllPutKoudaiFlat", "�\��(�t���b�g)���ꊇ�z�u���܂��B", "1_�t���b�g");

            //// [�ꊇ�z�u] �\��(����`��)
            //CreatePushButton(panelAllPut, "CmdAllPut2", "�\��(����`��)", "AllPutKoudaiUnique", "�\��(����`��)���ꊇ�z�u���܂��B");

            // [�ꊇ�z�u] �X���[�v�쐬
            CreatePushButton(panelAllPut, "CmdAllPut3", "�X���[�v", "CreateSlope", "�X���[�v���쐬���܂��B", "2_�X���[�v");

            // [�ꊇ�z�u] �u���[�X�E�c�i�M�z�u
            CreatePushButton(panelAllPut, "CmdAllPut4", "�u���[�X�E�c�i�M", "AllPutBraceTsunagi", "�u���[�X�E�c�i�M�ނ��ꊇ�z�u���܂��B", "3_�u���[�X�E�c�i�M");

            // [�ꊇ�z�u] �O���[�s���O
            CreatePushButton(panelAllPut, "CmdAllPut5", "�O���[�s���O", "Grouping", "�\����O���[�s���O���܂��B", "4_�O���[�s���O");

            RibbonPanel panelPut = app.CreateRibbonPanel("YMS�\��", "�ʔz�u");

            //// [�ʔz�u] �e�X�g�p ���ŏI�I�ɂ͍폜
            //CreatePushButton(panelPut, "CmdPutTest", "�e�X�g�p", "PutTest", "�e�X�g�p�̃R�}���h�ł��B", "0_Large_dummy");

            // [�ʔz�u] ���H��
            CreatePushButton(panelPut, "CmdPut1", "���H��", "PutFukkouban", "���H��z�u���܂��B", "5_���H��");

            // [�ʔz�u] ���(����)
            // [�ʔz�u] ����(�包)
            PushButtonDef pushButtonDef1 = new PushButtonDef("CmdPut2", "���(����)", "PutOobikiKetauke", "���(����)��z�u���܂��B", "6_���");
            PushButtonDef pushButtonDef2 = new PushButtonDef("CmdPut3", "����(�包)", "PutNedaShugeta", "����(�包)��z�u���܂��B", "7_����");
            CreateStackedPushButton(panelPut, new List<PushButtonDef> { pushButtonDef1, pushButtonDef2 });

            // [�ʔz�u] ���H���z�u
            // [�ʔz�u] �~���z�u
            PushButtonDef pushButtonDef3 = new PushButtonDef("CmdPut4", "���H��", "PutFukkougeta", "���H����z�u���܂��B", "8_���H��");
            PushButtonDef pushButtonDef4 = new PushButtonDef("CmdPut5", "�~��", "PutShikigeta", "�~����z�u���܂��B", "9_�~��");
            CreateStackedPushButton(panelPut, new List<PushButtonDef> { pushButtonDef3, pushButtonDef4 });

            // [�ʔz�u] �ΌX�\�E�ΌX�\��t�v���[�g�z�u
            CreatePushButton(panelPut, "CmdPut6", "�ΌX�\", "PutTaikeikou", "�ΌX�\�E�ΌX�\��t�v���[�g��z�u���܂��B", "10_�ΌX�\");

            // [�ʔz�u] �X�`�t�i�[�v���[�g�E�X�`�t�i�[�W���b�L�z�u
            CreatePushButton(panelPut, "CmdPut7", "�X�`�t�i�[", "PutSuchifuna", "�X�`�t�i�[�v���[�g�E�X�`�t�i�[�W���b�L��z�u���܂��B", "11_�X�`�t�i�[");

            // [�ʔz�u] ���������v���[�g�E�����ޔz�u
            CreatePushButton(panelPut, "CmdPut8", " ������", "PutTakasaChouseiPlate", " ���������v���[�g�E�����ނ�z�u���܂��B", "12_������");

            // [�ʔz�u] �~�S�z�u
            CreatePushButton(panelPut, "CmdPut9", "�~�S��", "PutKoubanzai", "�~�S��z�u���܂��B", "13_�~�S��");

            // [�ʔz�u] �n���E���H�Y���~�ߍޔz�u
            CreatePushButton(panelPut, "CmdPut10", "���H�Y���~��", "PutJifukuFukkoubanZuredome", "�n���E���H�Y���~�ߍނ�z�u���܂��B", "14_���H�Y���~��");

            // [�ʔz�u] �萠�E�萠�x���z�u
            CreatePushButton(panelPut, "CmdPut11", "�萠", "PutTesuriTesurishichu", "�萠�E�萠�x����z�u���܂��B", "15_�萠");

            // [�ʔz�u] �����u���[�X�z�u
            // [�ʔz�u] �����u���[�X�z�u
            // [�ʔz�u] �����c�i�M�z�u
            PushButtonDef pushButtonDef5 = new PushButtonDef("CmdPut12", "�����u���[�X", "PutHorizontalBrace", "�����u���[�X��z�u���܂��B", "16_�����u���[�X");
            PushButtonDef pushButtonDef6 = new PushButtonDef("CmdPut13", "�����u���[�X", "PutVerticalBrace", "�����u���[�X��z�u���܂��B", "17_�����u���[�X");
            PushButtonDef pushButtonDef7 = new PushButtonDef("CmdPut14", "�����c�i�M", "PutHorizontalTsunagi", "�����c�i�M��z�u���܂��B", "18_�����c�i�M");
            CreateStackedPushButton(panelPut, new List<PushButtonDef> { pushButtonDef5, pushButtonDef6, pushButtonDef7 });

            // [�ʔz�u] ��t�⏕�ޔz�u
            CreatePushButton(panelPut, "CmdPut15", "��t�⏕��", "PutTeiketsuHojyozai", "�����⏕�ނ�z�u���܂��B", "19_��t�⏕��");

            // [�ʔz�u] ����z�u
            CreatePushButton(panelPut, "CmdPut16", "����", "PutHoudue", "�����z�u���܂��B", "20_����");

            // [�ʔz�u] �x���z�u
            CreatePushButton(panelPut, "CmdPut17", "�x��", "PutShichu", "�x����z�u���܂��B", "21_�x��");

            // [�ʔz�u] �\��Y(�x���Y)
            CreatePushButton(panelPut, "CmdPut18", "�\��Y(�x���Y)", "PutKui", "�\��Y(�x���Y)��z�u���܂��B", "22_�\��Y");

            RibbonPanel panelEdit = app.CreateRibbonPanel("YMS�\��", "���ޏC��");

            // [���ޏC��] �T�C�Y�ꗗ
            CreatePushButton(panelEdit, "CmdEdit1", "�T�C�Y�ꗗ", "EditSizeList", "�e�핔�ނ̃T�C�Y��ύX���܂��B", "23_�T�C�Y�ꗗ");

            // [���ޏC��] �ʒu�ύX
            CreatePushButton(panelEdit, "CmdEdit2", "�ʒu�ύX", "EditLocationChange", "�e�핔�ނ̈ʒu��ύX���܂��B", "24_�ʒu�ύX");

            // [���ޏC��] �����ύX
            CreatePushButton(panelEdit, "CmdEdit3", "�����ύX", "EditLengthChange", "�e�핔�ނ̒�����ύX���܂��B", "25_�����ύX");

            RibbonPanel panelWaritsuke = app.CreateRibbonPanel("YMS�\��", "���t");

            // [���t] ���ފ��t
            CreatePushButton(panelWaritsuke, "CmdWaritsuke1", "���ފ��t", "WaritsukeElement", "�e�핔�ނ̊��t�����܂��B", "26_���ފ��t");

            RibbonPanel panelSonota = app.CreateRibbonPanel("YMS�\��", "���̑�");

            // [���̑�] ���`�F�b�N
            CreatePushButton(panelSonota, "CmdSonota1", "���`�F�b�N", "SonotaKanshouCheck", "���`�F�b�N���m�F���܂��B", "27_���`�F�b�N");

            // [���̑�] �ʔz�u
            CreatePushButton(panelSonota, "PushButton Command64", "�ʔz�u", "KobetsuHaichi", "�e���ނ̌ʔz�u���s���܂��B", "33 �ʔz�u");
            
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
        /// �A�C�R���̃t���p�X�擾
        /// </summary>
        /// <param name="name">�A�C�R�����i�g���q�Ȃ��j</param>
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
