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
using YMS_gantry.Data;
using YMS_gantry.GantryUtils;

namespace YMS_gantry.UI
{
    
    public partial class FrmPutJifukuFukkoubanZuredomezai : System.Windows.Forms.Form
    {
        #region 定数
        /// <summary>
        /// Iniファイルのパス
        /// </summary>
        const string IniPath = "ini\\FrmPutJifukuFukkoubanZuredomezai.ini";
        /// <summary>
        /// Iniファイルのセクション名
        /// </summary>
        const string sec = "FrmPutJifukuFukkoubanZuredomezai";
        #endregion

        private RequestHandler m_Handler;
        private ExternalEvent m_ExEvent;

        private UIApplication _uiApp { get; set; }
        private Document doc { get; set; }

        public FrmPutJifukuFukkoubanZuredomezai(UIApplication uiApp)
        {
            InitializeComponent();

            _uiApp = uiApp;
            doc = uiApp.ActiveUIDocument.Document;
        }
        public FrmPutJifukuFukkoubanZuredomezai(ExternalEvent exEvent, RequestHandler handler, UIApplication uiApp)
        {
            InitializeComponent();

            _uiApp = uiApp;
            doc = uiApp.ActiveUIDocument.Document;

            m_Handler = handler;
            m_ExEvent = exEvent;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(!CheckInpurValues())
            {
                return;
            }
            MakeRequest(RequestId.JifukuZuredome);
            SetIniData();
            this.DialogResult = DialogResult.OK;
        }

        private void FrmPutJifukuFukkoubanZuredomezai_Load(object sender, EventArgs e)
        {
            InitComboBox();
            changeEnabler();
            GetIniData();
        }

        /// <summary>
        /// コンボボックス初期化
        /// </summary>
        private void InitComboBox()
        {
            List<string> lstStr = new List<string>();

            //構台名をコンボボックスに追加
            ControlUtils.SetComboBoxItems(CmbKoudaiName, GantryUtil.GetAllKoudaiName(doc).ToList());

            //レベル追加
            ControlUtils.SetComboBoxItems(CmbLevel, GantryUtil.GetAllLevelName(doc).ToList());

            //材質
            ControlUtils.SetComboBoxItems(CmbMaterial, DefineUtil.ListMaterial, true);
        }

        /// <summary>
        /// コンボボックス初期化
        /// </summary>
        private void InitComboBoxSizeType()
        {
            List<string> lstStr = new List<string>();
            string title = "地覆";
            if (!string.IsNullOrEmpty(CmbKoudaiName.Text))
            {
                KoudaiData kData = GantryUtil.GetKoudaiData(doc, CmbKoudaiName.Text);
                string type = Master.ClsJifukuZuredomeCsv.TypeJifuku;
                lstStr.Add(Master.ClsJifukuZuredomeCsv.TypeC);
                lstStr.Add(Master.ClsJifukuZuredomeCsv.TypeL);

                if (kData.AllKoudaiFlatData.KoujiType == DefineUtil.eKoujiType.Kenchiku)
                {
                    type = Master.ClsJifukuZuredomeCsv.TypeZuredomezai;
                    title = "覆工板ズレ止め材";
                }
            }
            this.Text = $"[個別] {title}配置";
            ControlUtils.SetComboBoxItems(CmbSizeType, lstStr);
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

            this.NmcZuredomeLeng.Enabled = this.RbtAuto.Checked;

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
            ClsIni.WritePrivateProfileString(sec, RbtAuto.Name, RbtAuto.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, RbtAutoAll.Name, RbtAutoAll.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, NmcZuredomeLeng.Name, NmcZuredomeLeng.Text, iniPath);
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
                RbtAuto.Checked = bool.Parse(ClsIni.GetIniFile(sec, RbtAuto.Name, iniPath));
                RbtAutoAll.Checked = bool.Parse(ClsIni.GetIniFile(sec, RbtAutoAll.Name, iniPath));
                NmcZuredomeLeng.Text = ClsIni.GetIniFile(sec, NmcZuredomeLeng.Name, iniPath);
                RbtFace.Checked = bool.Parse(ClsIni.GetIniFile(sec, RbtFace.Name, iniPath));
                RbtFree.Checked = bool.Parse(ClsIni.GetIniFile(sec, RbtFree.Name, iniPath));
                CmbLevel.Text = ClsIni.GetIniFile(sec, CmbLevel.Name, iniPath);
                NmcOffset.Text = ClsIni.GetIniFile(sec, NmcOffset.Name, iniPath);
            }
        }

        private void RbtFree_CheckedChanged(object sender, EventArgs e)
        {
            changeEnabler();
        }

        private void CmbKoudaiName_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitComboBoxSizeType();

        }

        private void CmbSizeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<string> lstStr = new List<string>();
            if (!string.IsNullOrEmpty(CmbKoudaiName.Text))
            {
                KoudaiData kData = GantryUtil.GetKoudaiData(doc, CmbKoudaiName.Text);
                string type = Master.ClsJifukuZuredomeCsv.TypeJifuku;
                if (kData.AllKoudaiFlatData.KoujiType == DefineUtil.eKoujiType.Kenchiku)
                {
                    type = Master.ClsJifukuZuredomeCsv.TypeZuredomezai;
                }
                lstStr = Master.ClsJifukuZuredomeCsv.GetSizeList(type, CmbSizeType.Text);

                if (kData.AllKoudaiFlatData.KoujiType == DefineUtil.eKoujiType.Doboku)
                {
                    this.Text = "[個別] 地覆配置";
                }
                else
                {
                    this.Text = "[個別] 覆工板ズレ止め材配置";
                }
            }

            ControlUtils.SetComboBoxItems(CmbSize, lstStr);
            if(CmbSizeType.Text==Master.ClsJifukuZuredomeCsv.TypeL)
            {
                NmcZuredomeLeng.ReadOnly = true;
                NmcZuredomeLeng.Value = 0;
                RbtAutoAll.Checked = false;
                RbtAutoAll.Enabled=false;
                RbtAuto.Checked = true;

            }else
            {
                NmcZuredomeLeng.ReadOnly = false;
                RbtAutoAll.Enabled = true;
            }
        }

        private void RbtAuto_CheckedChanged(object sender, EventArgs e)
        {
            changeEnabler();
        }

        private void NmcZuredomeLeng_Leave(object sender, EventArgs e)
        {
            NumericUpDown nmc = (NumericUpDown)sender;
            if (nmc.Text == "")
            {
                nmc.Value = 0;
                nmc.Text = "0";
            }
            base.OnLostFocus(e);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //初期化処理
            MakeRequest(RequestId.End);
            m_ExEvent.Dispose();
            m_ExEvent = null;
            m_Handler = null;

            this.DialogResult = DialogResult.Cancel;
            this.Close();
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

            if(RbtAuto.Checked&&CmbSize.Text.StartsWith("[")&&NmcZuredomeLeng.Value<=0)
            {
                errMsg.Add("長さを指定してください");
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
