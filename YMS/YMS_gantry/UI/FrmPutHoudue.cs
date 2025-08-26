using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YMS_gantry.GantryUtils;
using YMS.gantry;
using System.IO;

namespace YMS_gantry.UI
{
    public partial class FrmPutHoudue : System.Windows.Forms.Form
    {
        #region 定数
        /// <summary>
        /// Iniファイルのパス
        /// </summary>
        const string IniPath = "ini\\FrmPutHoudue.ini";
        /// <summary>
        /// Iniファイルのセクション名
        /// </summary>
        const string sec = "FrmPutHoudue";
        #endregion

        private RequestHandler m_Handler;
        private ExternalEvent m_ExEvent;

        private UIApplication _uiApp { get; set; }
        private Document doc { get; set; }

        public FrmPutHoudue(UIApplication uiApp)
        {
            InitializeComponent();

            _uiApp = uiApp;
            doc = _uiApp.ActiveUIDocument.Document;
        }
        public FrmPutHoudue(ExternalEvent exEvent, RequestHandler handler, UIApplication uiApp)
        {
            InitializeComponent();

            _uiApp = uiApp;
            doc = _uiApp.ActiveUIDocument.Document;

            m_Handler = handler;
            m_ExEvent = exEvent;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            TypeChange();
        }

        private void numericUpDown1_Leave(object sender, EventArgs e)
        {

        }

        private void FrmPutHoudue_Load(object sender, EventArgs e)
        {
            InitCombo();
            GetIniData();
        }

        private void InitCombo()
        { 
            //構台名をコンボボックスに追加
            ControlUtils.SetComboBoxItems(this.CmbKoudaiName, GantryUtil.GetAllKoudaiName(doc).ToList());

            //サイズ
            TypeChange();

            //火打受Ps
            List<string> lstSt = new List<string>();
            lstSt = Master.ClsHoudueCsv.GetSizeList(Master.ClsHoudueCsv.TypeHiuchiUke);
            ControlUtils.SetComboBoxItems(this.CmbKetaSize, lstSt);
            ControlUtils.SetComboBoxItems(this.CmbKuiSize, lstSt);
        }

        private void TypeChange()
        {
            string type = Master.ClsHoudueCsv.TypeYamadome;
            if (this.RbtHiuchi.Checked)
            {
                this.NmcLength.Enabled = true;
                this.groupBox1.Enabled = true;
            }
            else
            {
                this.NmcLength.Enabled = false;
                this.groupBox1.Enabled = false;
                type= (this.RbtEdge.Checked) ? Master.ClsHoudueCsv.TypeSumibu : Master.ClsHoudueCsv.TypeSozai;
            }
            List<string> lstSt = Master.ClsHoudueCsv.GetSizeList(type);
            ControlUtils.SetComboBoxItems(this.CmbSize, lstSt);
        }

        /// <summary>
        /// サイズリスト更新
        /// </summary>
        private void InitSizeList()
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            TypeChange();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            TypeChange();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!CheckInpurValues())
            {
                return;
            }
            MakeRequest(RequestId.Houdue);
            SetIniData();
            this.DialogResult = DialogResult.OK;
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

        /// <summary>
        /// ダイアログデータをini二セット
        /// </summary>
        public void SetIniData()
        {
            string iniPath = ClsIni.GetIniFilePath(IniPath);
            ClsIni.WritePrivateProfileString(sec, RbtHiuchi.Name, RbtHiuchi.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, RbtEdge.Name, RbtEdge.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, RbtMaterial.Name, RbtMaterial.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, CmbSize.Name, CmbSize.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, NmcLength.Name, NmcLength.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, CmbKuiSize.Name, CmbKuiSize.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, CmbKetaSize.Name, CmbKetaSize.Text, iniPath);
        }

        /// <summary>
        /// iniデータをダイアログにセット
        /// </summary>
        public void GetIniData()
        {
            string iniPath = ClsIni.GetIniFilePath(IniPath);
            if (File.Exists(iniPath))
            {
                RbtHiuchi.Checked = bool.Parse(ClsIni.GetIniFile(sec, RbtHiuchi.Name, iniPath));
                RbtEdge.Checked = bool.Parse(ClsIni.GetIniFile(sec, RbtEdge.Name, iniPath));
                RbtMaterial.Checked = bool.Parse(ClsIni.GetIniFile(sec, RbtMaterial.Name, iniPath));
                CmbSize.Text = ClsIni.GetIniFile(sec, CmbSize.Name, iniPath);
                NmcLength.Text = ClsIni.GetIniFile(sec, NmcLength.Name, iniPath);
                CmbKuiSize.Text = ClsIni.GetIniFile(sec, CmbKuiSize.Name, iniPath);
                CmbKetaSize.Text = ClsIni.GetIniFile(sec, CmbKetaSize.Name, iniPath);
            }
        }

        private bool CheckInpurValues()
        {
            List<string> errMsg = new List<string>();

            if (string.IsNullOrEmpty(CmbKoudaiName.Text))
            {
                errMsg.Add("構台名を指定してください");
            }

            if (string.IsNullOrEmpty(CmbSize.Text))
            {
                errMsg.Add("サイズを設定してください");
            }

            if (RbtHiuchi.Checked)
            {
                if(NmcLength.Value<=0)
                {
                    errMsg.Add("主材長さを指定してください");
                }

                if(string.IsNullOrEmpty(CmbKetaSize.Text))
                {
                    errMsg.Add("桁側のVPサイズを指定してください");
                }
                if(string.IsNullOrEmpty(CmbKuiSize.Text))
                {
                    errMsg.Add("杭側のVPサイズを指定してください");
                }
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
