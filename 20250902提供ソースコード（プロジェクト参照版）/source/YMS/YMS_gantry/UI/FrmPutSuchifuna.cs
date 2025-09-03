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
    public partial class FrmPutSuchifuna : System.Windows.Forms.Form
    {
        #region 定数
        /// <summary>
        /// Iniファイルのパス
        /// </summary>
        const string IniPath = "ini\\FrmPutSuchifuna.ini";
        /// <summary>
        /// Iniファイルのセクション名
        /// </summary>
        const string sec = "FrmPutSuchifuna";
        #endregion

        private RequestHandler m_Handler;
        private ExternalEvent m_ExEvent;

        private UIApplication _uiApp { get; set; }
        private Document doc { get; set; }

        public FrmPutSuchifuna(UIApplication uiApp)
        {
            InitializeComponent();

            _uiApp = uiApp;
            doc = _uiApp.ActiveUIDocument.Document;
        }
        public FrmPutSuchifuna(ExternalEvent exEvent, RequestHandler handler, UIApplication uiApp)
        {
            InitializeComponent();

            _uiApp = uiApp;
            doc = _uiApp.ActiveUIDocument.Document;
            m_Handler = handler;
            m_ExEvent = exEvent;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            InitEnable();
        }

        private void FrmPutSuchifuna_Load(object sender, EventArgs e)
        {
            InitComboBox();
            InitEnable();
            GetIniData();
            InitSizeList();
        }

        private void InitComboBox()
        {
            //構台名をコンボボックスに追加
            ControlUtils.SetComboBoxItems(this.CmbKoudaiName, GantryUtil.GetAllKoudaiName(doc).ToList());

            //種類
            ControlUtils.SetComboBoxItems(this.CmbSizeType, new List<string>(){Master.ClsStiffenerCsv.PL,Master.ClsStiffenerCsv.SHJack,Master.ClsStiffenerCsv.DWJJack });
        }

        private void CmbSizeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitSizeList();
        }

        private void InitSizeList()
        {
            if (this.RbtFree.Checked)
            {
                List<string> lstSt = Master.ClsStiffenerCsv.GetSizeList(this.CmbSizeType.Text);
                ControlUtils.SetComboBoxItems(this.CmbSize, lstSt, true);
            }
            else
            {
                this.CmbSize.Items.Clear();
                this.CmbSize.Text = "";
            }
        }

        /// <summary>
        /// ダイアログデータをini二セット
        /// </summary>
        public void SetIniData()
        {
            string iniPath = ClsIni.GetIniFilePath(IniPath);
            ClsIni.WritePrivateProfileString(sec, CmbSizeType.Name, CmbSizeType.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, CmbSize.Name, CmbSize.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, RbtAuto.Name, RbtAuto.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, RbtFree.Name, RbtFree.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, ChkNeda.Name, ChkNeda.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, ChkOhbiki.Name, ChkOhbiki.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, ChkShikigeta.Name, ChkShikigeta.Checked.ToString(), iniPath);
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
                RbtAuto.Checked = bool.Parse(ClsIni.GetIniFile(sec, RbtAuto.Name, iniPath));
                RbtFree.Checked = bool.Parse(ClsIni.GetIniFile(sec, RbtFree.Name, iniPath));
                if (this.RbtFree.Checked)
                {
                    List<string> lstSt = Master.ClsStiffenerCsv.GetSizeList(this.CmbSizeType.Text);
                    ControlUtils.SetComboBoxItems(this.CmbSize, lstSt, true);
                    string size = CmbSize.Text = ClsIni.GetIniFile(sec, CmbSize.Name, iniPath);
                    if (CmbSize.Items.Contains(size))
                    {
                        CmbSize.SelectedIndex = CmbSize.Items.IndexOf(size);
                    }
                }
                else
                {
                    this.CmbSize.Items.Clear();
                    this.CmbSize.Text = "";
                }
                ChkNeda.Checked = bool.Parse(ClsIni.GetIniFile(sec, ChkNeda.Name, iniPath));
                ChkOhbiki.Checked = bool.Parse(ClsIni.GetIniFile(sec, ChkOhbiki.Name, iniPath));
                ChkShikigeta.Checked = bool.Parse(ClsIni.GetIniFile(sec, ChkShikigeta.Name, iniPath));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!CheckInpurValues())
            {
                return;
            }

            MakeRequest(RequestId.Stiffener);
            SetIniData();
            this.DialogResult = DialogResult.OK;
        }
        private bool CheckInpurValues()
        {
            List<string> errMsg = new List<string>();

            if (string.IsNullOrEmpty(CmbKoudaiName.Text))
            {
                errMsg.Add("構台名を指定してください");
            }
            if (RbtFree.Checked&&string.IsNullOrEmpty(CmbSize.Text))
            {
                errMsg.Add("サイズを設定してください");
            }
            if(RbtAuto.Checked&&!ChkNeda.Checked&&!ChkOhbiki.Checked&&!ChkShikigeta.Checked)
            {
                errMsg.Add("対象とする部材にチェックを入れてください");
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
        private void button2_Click(object sender, EventArgs e)
        {
            //初期化処理
            SetIniData();
            MakeRequest(RequestId.End);
            m_ExEvent.Dispose();
            m_ExEvent = null;
            m_Handler = null;

            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void InitEnable()
        {
            this.CmbSize.Enabled = !this.RbtAuto.Checked;
            this.groupBox1.Enabled = this.RbtAuto.Checked;
        }

        private void RbtAuto_CheckedChanged(object sender, EventArgs e)
        {
            InitSizeList();
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
