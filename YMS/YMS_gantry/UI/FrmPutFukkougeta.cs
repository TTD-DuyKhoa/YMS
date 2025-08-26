using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YMS.gantry;
using YMS_gantry.GantryUtils;

namespace YMS_gantry.UI
{
    public partial class FrmPutFukkougeta : System.Windows.Forms.Form
    {
        #region 定数
        /// <summary>
        /// Iniファイルのパス
        /// </summary>
        const string IniPath = "ini\\FrmPutFukkougeta.ini";
        /// <summary>
        /// Iniファイルのセクション名
        /// </summary>
        const string sec = "FrmPutFukkougeta";
        #endregion

        private RequestHandler m_Handler;
        private ExternalEvent m_ExEvent;

        private UIApplication _uiApp { get; set; }
        private Document doc { get; set; }

        public FrmPutFukkougeta(UIApplication uiApp)
        {
            InitializeComponent();
            _uiApp = uiApp;
            doc = uiApp.ActiveUIDocument.Document;
        }
        public FrmPutFukkougeta(ExternalEvent exEvent, RequestHandler handler, UIApplication uiApp)
        {
            InitializeComponent();
            _uiApp = uiApp;
            doc = uiApp.ActiveUIDocument.Document;

            m_Handler = handler;
            m_ExEvent = exEvent;
        }

        private void FrmPutFukkougeta_Load(object sender, EventArgs e)
        {
            //構台名をコンボボックスに追加
            ControlUtils.SetComboBoxItems(CmbKoudaiName, GantryUtil.GetAllKoudaiName(doc).ToList());

            //レベル追加
            ControlUtils.SetComboBoxItems(CmbLevel, GantryUtil.GetAllLevelName(doc).ToList());

            //材種
            ControlUtils.SetComboBoxItems(CmbSizeType,new List<string>() {Master.ClsFukkouGetaCsv.TypeH, Master.ClsFukkouGetaCsv.TypeYamadome, Master.ClsFukkouGetaCsv.TypeC });

            GetIniData();
            changeEnabler();
        }

        /// <summary>
        /// ダイアログデータをini二セット
        /// </summary>
        public void SetIniData()
        {
            string iniPath = ClsIni.GetIniFilePath(IniPath);
            ClsIni.WritePrivateProfileString(sec, CmbSizeType.Name, CmbSizeType.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, CmbSize.Name, CmbSize.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, CmbMaterial.Name, CmbMaterial.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, RbtUp.Name, RbtUp.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, RbtDown.Name, RbtDown.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, RbtSide.Name, RbtSide.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, RbtFace.Name, RbtFace.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, RbtFree.Name, RbtFree.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, CmbLevel.Name, CmbLevel.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, NmcOffset.Name, NmcOffset.Text, iniPath);
        }

        /// <summary>
        /// iniデータをダイアログにセット
        /// </summary>
        public void GetIniData()
        {
            string iniPath = ClsIni.GetIniFilePath(IniPath);
            if (File.Exists(iniPath))
            {
                CmbSizeType.Text = ClsIni.GetIniFile(sec, CmbSizeType.Name, iniPath);
                CmbSize.Text = ClsIni.GetIniFile(sec, CmbSize.Name, iniPath);
                CmbMaterial.Text = ClsIni.GetIniFile(sec, CmbMaterial.Name, iniPath);
                RbtUp.Checked = bool.Parse(ClsIni.GetIniFile(sec, RbtUp.Name, iniPath));
                RbtDown.Checked = bool.Parse(ClsIni.GetIniFile(sec, RbtDown.Name, iniPath));
                RbtSide.Checked = bool.Parse(ClsIni.GetIniFile(sec, RbtSide.Name, iniPath));
                RbtFace.Checked = bool.Parse(ClsIni.GetIniFile(sec, RbtFace.Name, iniPath));
                RbtFree.Checked = bool.Parse(ClsIni.GetIniFile(sec, RbtFree.Name, iniPath));
                CmbLevel.Text = ClsIni.GetIniFile(sec, CmbLevel.Name, iniPath);
                NmcOffset.Text = ClsIni.GetIniFile(sec, NmcOffset.Name, iniPath);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<string> lstStr = new List<string>();
            string type = this.CmbSizeType.SelectedItem.ToString();
            lstStr = Master.ClsFukkouGetaCsv.GetSizeList(type);
            ControlUtils.SetComboBoxItems(CmbSize, lstStr);
            this.groupBox1.Enabled = (type == Master.ClsFukkouGetaCsv.TypeC);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (!CheckInpurValues())
            {
                return;
            }

            MakeRequest(RequestId.Fukkougeta);
            SetIniData();
            this.DialogResult = DialogResult.OK;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //初期化処理
            MakeRequest(RequestId.End);
            m_ExEvent.Dispose();
            m_ExEvent = null;
            m_Handler = null;

            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void RbtFace_CheckedChanged(object sender, EventArgs e)
        {
            changeEnabler();
        }

         private void changeEnabler()
        {
            if (!this.RbtFree.Checked)
            {
                this.CmbLevel.Enabled = false;
                this.NmcOffset.Enabled = false;
                if (CmbLevel.Items.Contains("部材選択"))
                {
                    CmbLevel.Items.Remove("部材選択");
                }
            }
            else
            {
                this.CmbLevel.Enabled = true;
                this.NmcOffset.Enabled = true;
                if (!CmbLevel.Items.Contains("部材選択"))
                {
                    CmbLevel.Items.Add("部材選択");
                }
            }

        }

        private bool CheckInpurValues()
        {
            List<string> errMsg = new List<string>();

            if (string.IsNullOrEmpty(CmbKoudaiName.Text))
            {
                errMsg.Add("構台名を指定してください");
            }

            if (string.IsNullOrEmpty(CmbMaterial.Text))
            {
                errMsg.Add("材質を設定してください");
            }
            if (string.IsNullOrEmpty(CmbSize.Text))
            {
                errMsg.Add("サイズを設定してください");
            }

            if (RbtFree.Checked && string.IsNullOrEmpty(CmbLevel.Text))
            {
                errMsg.Add("配置するレベルを指定してください");
            }

            if(CmbSizeType.Text==Master.ClsFukkouGetaCsv.TypeC&&!RbtDown.Checked&&!RbtUp.Checked&&!RbtSide.Checked)
            {
                errMsg.Add("配置する向きを指定してください");
            }

            if (errMsg.Count > 0)
            {
                FrmErrorInformation frm = new FrmErrorInformation(errMsg, this);
                return false;
            }
            else
            {
                return true;
            }
        }

        #region "モードレス対応"
        /// <summary>
        ///   WakeUp -> enable all controls
        /// </summary>
        /// 
        public void WakeUp()
        {
            EnableCommands(true);
        }
        /// <summary>
        ///   Control enabler / disabler 
        /// </summary>
        ///
        private void EnableCommands(bool status)
        {
            foreach (System.Windows.Forms.Control ctrl in this.Controls)
            {
                ctrl.Enabled = status;
            }
            //if (!status)
            //{
            //    this.button3.Enabled = true;
            //}
        }
        private void MakeRequest(RequestId request)
        {
            m_Handler.Request.Make(request);
            m_ExEvent.Raise();
            DozeOff();
        }
        /// <summary>
        ///   DozeOff -> disable all controls (but the Exit button)
        /// </summary>
        /// 
        private void DozeOff()
        {
            EnableCommands(false);
        }
        #endregion

    }
}
