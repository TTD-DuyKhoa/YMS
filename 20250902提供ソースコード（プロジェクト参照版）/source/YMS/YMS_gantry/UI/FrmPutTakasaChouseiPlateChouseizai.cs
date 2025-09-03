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
    public partial class FrmPutTakasaChouseiPlateChouseizai : System.Windows.Forms.Form
    {
        #region 定数
        /// <summary>
        /// Iniファイルのパス
        /// </summary>
        const string IniPath = "ini\\FrmPutTakasaChouseiPlateChouseizai.ini";
        /// <summary>
        /// Iniファイルのセクション名
        /// </summary>
        const string sec = "FrmPutTakasaChouseiPlateChouseizai";
        #endregion

        private RequestHandler m_Handler;
        private ExternalEvent m_ExEvent;

        private UIApplication _uiApp { get; set; }
        private Document doc { get; set; }
        public FrmPutTakasaChouseiPlateChouseizai(UIApplication uiApp)
        {
            InitializeComponent();
            _uiApp = uiApp;
            doc = uiApp.ActiveUIDocument.Document;
        }
        public FrmPutTakasaChouseiPlateChouseizai(ExternalEvent exEvent, RequestHandler handler, UIApplication uiApp)
        {
            InitializeComponent();
            _uiApp = uiApp;
            doc = uiApp.ActiveUIDocument.Document;

            m_Handler = handler;
            m_ExEvent = exEvent;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.RbtPlate.Checked)
            {
                this.panel2.Enabled = true;
                this.panel3.Enabled = false;
                return;
            }

            this.panel2.Enabled = false;
            this.panel3.Enabled = true;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.RbtAssistPiece.Checked)
            {
                this.panel2.Enabled = false;
                this.panel3.Enabled = true;
                return;
            }

            this.panel2.Enabled = true;
            this.panel3.Enabled = false;
        }

        private void FrmPutTakasaChouseiPlateChouseizai_Load(object sender, EventArgs e)
        {
            //構台名をコンボボックスに追加
            ControlUtils.SetComboBoxItems(CmbKoudaiName, GantryUtil.GetAllKoudaiName(doc).ToList(),true);

            //レベル追加
            ControlUtils.SetComboBoxItems(CmbLevel, GantryUtil.GetAllLevelName(doc).ToList(),true);

            //材質
            List<string> lstStr = new List<string>();
            lstStr = DefineUtil.ListMaterial;
            ControlUtils.SetComboBoxItems(CmbMaterial, lstStr,true);

            //調整ピース
            lstStr = Master.ClsTakasaChouseiCsv.GetSizeList(Master.ClsTakasaChouseiCsv.TypePieace);
            ControlUtils.SetComboBoxItems(CmbPieaceSize, lstStr,true);

            //タイプ選択
            lstStr = new List<string>() { "フランジ面カット", "ウェブ面カット" };
            ControlUtils.SetComboBoxItems(CmbPieceType, lstStr, true);

            IsCut();
            GetIniData();
        }

        /// <summary>
        /// ダイアログデータをini二セット
        /// </summary>
        public void SetIniData()
        {
            string iniPath = ClsIni.GetIniFilePath(IniPath);
            ClsIni.WritePrivateProfileString(sec, CmbMaterial.Name, CmbMaterial.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, RbtPlate.Name, RbtPlate.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, RbtAssistPiece.Name, RbtAssistPiece.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, NmcPLW.Name, NmcPLW.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, NmcPLD.Name, NmcPLD.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, NmcPLH1.Name, NmcPLH1.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, NmcPLH2.Name, NmcPLH2.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, CmbPieaceSize.Name, CmbPieaceSize.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, ChckCutOn.Name, ChckCutOn.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, CmbPieceType.Name, CmbPieceType.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, NmcPieaceH1.Name, NmcPieaceH1.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, NmcPieaceH2.Name, NmcPieaceH2.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, CmbTopEndPLSize.Name, CmbTopEndPLSize.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, CmbBtmEndPLSize.Name, CmbBtmEndPLSize.Text, iniPath);

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
                CmbMaterial.Text = ClsIni.GetIniFile(sec, CmbMaterial.Name, iniPath);
                RbtPlate.Checked = bool.Parse(ClsIni.GetIniFile(sec, RbtPlate.Name, iniPath));
                RbtAssistPiece.Checked = bool.Parse(ClsIni.GetIniFile(sec, RbtAssistPiece.Name, iniPath));
                NmcPLW.Text = ClsIni.GetIniFile(sec, NmcPLW.Name, iniPath);
                NmcPLD.Text = ClsIni.GetIniFile(sec, NmcPLD.Name, iniPath);
                NmcPLH1.Text = ClsIni.GetIniFile(sec, NmcPLH1.Name, iniPath);
                NmcPLH2.Text = ClsIni.GetIniFile(sec, NmcPLH2.Name, iniPath);
                CmbPieaceSize.Text = ClsIni.GetIniFile(sec, CmbPieaceSize.Name, iniPath);
                ChckCutOn.Checked = bool.Parse(ClsIni.GetIniFile(sec, ChckCutOn.Name, iniPath));
                CmbPieceType.Text = ClsIni.GetIniFile(sec, CmbPieceType.Name, iniPath);
                NmcPieaceH1.Text = ClsIni.GetIniFile(sec, NmcPieaceH1.Name, iniPath);
                NmcPieaceH2.Text = ClsIni.GetIniFile(sec, NmcPieaceH2.Name, iniPath);
                CmbTopEndPLSize.Text = ClsIni.GetIniFile(sec, CmbTopEndPLSize.Name, iniPath);
                CmbBtmEndPLSize.Text = ClsIni.GetIniFile(sec, CmbBtmEndPLSize.Name, iniPath);
                RbtFace.Checked = bool.Parse(ClsIni.GetIniFile(sec, RbtFace.Name, iniPath));
                RbtFree.Checked = bool.Parse(ClsIni.GetIniFile(sec, RbtFree.Name, iniPath));
                CmbLevel.Text = ClsIni.GetIniFile(sec, CmbLevel.Name, iniPath);
                NmcOffset.Text = ClsIni.GetIniFile(sec, NmcOffset.Name, iniPath);
            }
        }

        private void numericUpDown1_Leave(object sender, EventArgs e)
        {
            NumericUpDown nmc = (NumericUpDown)sender;
            if (nmc.Text == "")
            {
                nmc.Value = 0;
                nmc.Text = "0";
            }
            base.OnLostFocus(e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!CheckInpurValues())
            {
                return;
            }

            MakeRequest(RequestId.TakasaChousei);
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
            if (string.IsNullOrEmpty(CmbMaterial.Text))
            {
                errMsg.Add("材質を設定してください");
            }
            
            if(RbtPlate.Checked)
            {
                if((NmcPLD.Value <= 0 || NmcPLW.Value <= 0 || NmcPLH1.Value <= 0 || NmcPLH2.Value <= 0))
                {
                    errMsg.Add("プレートサイズに0は設定できません");
                }
            }
            if (RbtAssistPiece.Checked)
            {
                if(string.IsNullOrEmpty(CmbPieaceSize.Text))
                {
                    errMsg.Add("ピースサイズを設定してください");
                }

                if(ChckCutOn.Checked)
                {
                    if (string.IsNullOrEmpty(CmbPieceType.Text))
                    {
                        errMsg.Add("カットのタイプを選択してください");
                    }                    if(NmcPieaceH1.Enabled&&NmcPieaceH1.Value<=0)
                    {
                        errMsg.Add("ピースの高さＨ１を指定してください");
                    }

                    if (NmcPieaceH2.Enabled && NmcPieaceH2.Value <= 0)
                    {
                        errMsg.Add("ピースの高さＨ2を指定してください");
                    }                }

                if(CmbTopEndPLSize.Enabled&&string.IsNullOrEmpty(CmbTopEndPLSize.Text))
                {
                    errMsg.Add("トッププレートを指定してください");
                }
                if (CmbBtmEndPLSize.Enabled&&string.IsNullOrEmpty(CmbBtmEndPLSize.Text))
                {
                    errMsg.Add("エンドプレートを指定してください");
                }
            }

            if (RbtFree.Checked&& string.IsNullOrEmpty(CmbLevel.Text))
            {
                errMsg.Add("配置するレベルを指定してください");
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

        private void ComboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            string size = CmbPieaceSize.Text;
            //定形サイズだったら
            if (string.IsNullOrEmpty(size)||!size.StartsWith("H"))
            {
                this.CmbTopEndPLSize.Text = "";
                this.CmbBtmEndPLSize.Text = "";
                this.groupBox1.Enabled = false;
                this.groupBox3.Enabled = false;
                this.NmcPieaceH1.Enabled = false;
                this.NmcPieaceH2.Enabled = false;
                this.CmbPieceType.Text = "";
                this.CmbPieceType.Enabled = false;
                this.ChckCutOn.Enabled = false;
                return;
            }

            //可変H鋼なら
            if(size.StartsWith("H"))
            {
                this.groupBox1.Enabled = true;
                this.groupBox3.Enabled = true;
                this.NmcPieaceH1.Enabled = true;
                this.NmcPieaceH2.Enabled = true;
                this.CmbPieceType.Enabled = true;
                this.ChckCutOn.Enabled = true;
                IsCut();
            }

            //サイズごとにエンドPLのサイズも変える
            List<string> lstStr = new List<string>();
            lstStr = Master.ClsPieacePLSizeCsv.GetSizeList(size);
            var lstTop = lstStr; 
            //lstTop.Add("任意サイズ上");
            ControlUtils.SetComboBoxItems(this.CmbTopEndPLSize, lstTop,true);
            var lstBtm = lstStr;
            //lstBtm.Remove("任意サイズ上");lstBtm.Add("任意サイズ下");
            ControlUtils.SetComboBoxItems(this.CmbBtmEndPLSize, lstBtm,true);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            IsCut();
        }

        private void IsCut()
        {
            bool isCut = this.ChckCutOn.Checked;
            this.NmcPieaceH2.Enabled = isCut && CmbPieaceSize.Text.StartsWith("H");
            if(isCut)
            {
                label16.Text = "H1";
                label14.Text = "H2";
            }
            else
            {
                label16.Text = "H";
                label14.Text = "";
            }
        }

        private void checkBox1_Leave(object sender, EventArgs e)
        {
            NumericUpDown nmc = (NumericUpDown)sender;
            if (nmc.Text == "")
            {
                nmc.Value = 0;
                nmc.Text = "0";
            }
            base.OnLostFocus(e);
        }

        private void CmbTopEndPLSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool IsFree = this.CmbTopEndPLSize.Text.Contains("任意");
            //if (!string.IsNullOrEmpty(this.CmbTopEndPLSize.Text) && IsFree)
            //{
            //    this.NmcTopPLW.Enabled = true;
            //    this.NmcTopPLD.Enabled = true;
            //    this.NmcTopPLH.Enabled = true;
            //}
            //else
            //{
            //    this.NmcTopPLW.Enabled = false;
            //    this.NmcTopPLD.Enabled = false;
            //    this.NmcTopPLH.Enabled = false;
            //}
        }

        private void CmbBtmEndPLSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool IsFree = this.CmbBtmEndPLSize.Text.Contains("任意");
            //if (!string.IsNullOrEmpty(this.CmbBtmEndPLSize.Text) && IsFree)
            //{
            //    this.NmcEndPLW.Enabled = true;
            //    this.NmcEndPLD.Enabled = true;
            //    this.NmcEndPLH.Enabled = true;
            //}
            //else
            //{
            //    this.NmcEndPLW.Enabled = false;
            //    this.NmcEndPLD.Enabled = false;
            //    this.NmcEndPLH.Enabled = false;
            //}
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
