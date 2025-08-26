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
using YMS.gantry;
using System.Windows.Forms;
using YMS_gantry.Data;
using YMS_gantry.GantryUtils;
using RevitUtil;
using System.IO;
using YMS_gantry.GantryUtils;

namespace YMS_gantry.UI
{
    public partial class FrmPutHorizontalBrace : System.Windows.Forms.Form
    {

        #region 定数
        /// <summary>
        /// Iniファイルのパス
        /// </summary>
        const string IniPath = "ini\\FrmPutHorizontalBrace.ini";
        /// <summary>
        /// Iniファイルのセクション名
        /// </summary>
        const string sec = "FrmPutHorizontalBrace";
        #endregion

        public PutHorizontalBraceId putHorizontalBraceId { get; set; }

        public enum PutHorizontalBraceId : int
        {
            OK,
            Cancel
        }

        #region コンストラクタ

        private RequestHandler m_Handler;
        private ExternalEvent m_ExEvent;
        private UIApplication _uiApp { get; set; }
        private Document doc { get; set; }
        public FrmPutHorizontalBraceData data { get; set; }

        public FrmPutHorizontalBrace(ExternalEvent exEvent, RequestHandler handler, UIApplication uiApp)
        {
            InitializeComponent();
            _uiApp = uiApp;
            doc = uiApp.ActiveUIDocument.Document;

            m_Handler = handler;
            m_ExEvent = exEvent;

            InitComboBox();
        }
        #endregion

        private void InitComboBox()
        {
            List<string> lstStr = new List<string>();

            //全構台名
            lstStr = GantryUtil.GetAllKoudaiName(doc);
            ControlUtils.SetComboBoxItems(cmbKoudaiName, lstStr);

            //部材
            if(chkTurnBackle.Checked)
            {
                lstStr = Master.ClsTurnBackleCsv.GetSizeList(Master.ClsTurnBackleCsv.TurnBackle);
            }
            else
            {
                lstStr = Master.ClsAngleCsv.GetSizeList(Master.ClsAngleCsv.TypeAngle);
            }
            ControlUtils.SetComboBoxItems(CmbSize, lstStr);
        }

        private void FrmPutHorizontalBrace_Load(object sender, EventArgs e)
        {
            GetIniData();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            //if (radioButton1.Checked)
            //{
            //    this.panel1.Enabled = true;
            //    return;
            //}

            //this.panel1.Enabled = false;

            if (RbtPutWayContinue.Checked)
            {
                this.groupBox15.Enabled = true;
                return;
            }

            this.groupBox15.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //コントロールの値をセット
            FrmPutHorizontalBraceData fData = new FrmPutHorizontalBraceData();
            fData.SelectedKodai = cmbKoudaiName.Text;
            fData.Size = CmbSize.Text;
            fData.PlaceIsSingle = RbtPutWaySingle.Checked;
            fData.PlaceIsMultiple = RbtPutWayContinue.Checked;
            fData.SetObikiFace = (RbtPutLocationOhbikiTopFlBottom.Checked ? FrmPutHorizontalBraceData.SetFace.UpperB :
                                   (RbtPutLocationOhbikiBottomFlTop.Checked ? FrmPutHorizontalBraceData.SetFace.LowerT :
                                                           FrmPutHorizontalBraceData.SetFace.LowerB));
            fData.IsTopPlate = RbtPutLocationOnTopPL.Checked;

            fData.VrtBrace1UpperXMm = (int)NmcBanRangeX.Value;
            fData.VrtBrace1UpperYMm = (int)NmcBanRangeY.Value;
            fData.VrtBraceConnectingType = (RbtAttachWayWelding.Checked ? FrmPutHorizontalBraceData.ConnectingType.Welding :
                                           (RbtAttachWayBolt.Checked ? FrmPutHorizontalBraceData.ConnectingType.Bolt :
                                                                    FrmPutHorizontalBraceData.ConnectingType.Metal));
            fData.IsTurnBackle = chkTurnBackle.Checked;
            data = fData;

            putHorizontalBraceId = PutHorizontalBraceId.OK;
            MakeRequest(RequestId.HorizontalBrace);
            SetIniData();
            this.DialogResult = DialogResult.OK;

        }
        private void button3_Click(object sender, EventArgs e)
        {
            //初期化処理
            this.DialogResult = DialogResult.Cancel;
            putHorizontalBraceId = PutHorizontalBraceId.Cancel;
            MakeRequest(RequestId.HorizontalBrace);
            m_ExEvent.Dispose();
            m_ExEvent = null;
            m_Handler = null;

            this.Close();

        }

        /// <summary>
        /// ダイアログデータをini二セット
        /// </summary>
        public void SetIniData()
        {
            string iniPath = ClsIni.GetIniFilePath(IniPath);
            ClsIni.WritePrivateProfileString(sec, CmbSize.Name, CmbSize.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, RbtPutWayContinue.Name, RbtPutWayContinue.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, RbtPutWaySingle.Name, RbtPutWaySingle.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, RbtPutLocationOhbikiTopFlBottom.Name, RbtPutLocationOhbikiTopFlBottom.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, RbtPutLocationOhbikiBottomFlTop.Name, RbtPutLocationOhbikiBottomFlTop.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, RbtPutLocationOhbikiBottomFlBottom.Name, RbtPutLocationOhbikiBottomFlBottom.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, RbtPutLocationOnTopPL.Name, RbtPutLocationOnTopPL.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, NmcBanRangeX.Name, NmcBanRangeX.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, NmcBanRangeY.Name, NmcBanRangeY.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, RbtAttachWayWelding.Name, RbtAttachWayWelding.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, RbtAttachWayBolt.Name, RbtAttachWayBolt.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, RbtAttachWayTeiketsu.Name, RbtAttachWayTeiketsu.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, chkTurnBackle.Name, chkTurnBackle.Checked.ToString(), iniPath);
        }

        /// <summary>
        /// iniデータをダイアログにセット
        /// </summary>
        public void GetIniData()
        {
            string iniPath = ClsIni.GetIniFilePath(IniPath);
            if (File.Exists(iniPath))
            {
                CmbSize.Text = ClsIni.GetIniFile(sec, CmbSize.Name, iniPath);
                RbtPutWayContinue.Checked = bool.Parse(ClsIni.GetIniFile(sec, RbtPutWayContinue.Name, iniPath));
                RbtPutWaySingle.Checked = bool.Parse(ClsIni.GetIniFile(sec, RbtPutWaySingle.Name, iniPath));
                RbtPutLocationOhbikiTopFlBottom.Checked = bool.Parse(ClsIni.GetIniFile(sec, RbtPutLocationOhbikiTopFlBottom.Name, iniPath));
                RbtPutLocationOhbikiBottomFlTop.Checked = bool.Parse(ClsIni.GetIniFile(sec, RbtPutLocationOhbikiBottomFlTop.Name, iniPath));
                RbtPutLocationOhbikiBottomFlBottom.Checked = bool.Parse(ClsIni.GetIniFile(sec, RbtPutLocationOhbikiBottomFlBottom.Name, iniPath));
                RbtPutLocationOnTopPL.Checked = bool.Parse(ClsIni.GetIniFile(sec, RbtPutLocationOnTopPL.Name, iniPath));
                NmcBanRangeX.Text = ClsIni.GetIniFile(sec, NmcBanRangeX.Name, iniPath);
                NmcBanRangeY.Text = ClsIni.GetIniFile(sec, NmcBanRangeY.Name, iniPath);
                RbtAttachWayWelding.Checked = bool.Parse(ClsIni.GetIniFile(sec, RbtAttachWayWelding.Name, iniPath));
                RbtAttachWayBolt.Checked = bool.Parse(ClsIni.GetIniFile(sec, RbtAttachWayBolt.Name, iniPath));
                RbtAttachWayTeiketsu.Checked = bool.Parse(ClsIni.GetIniFile(sec, RbtAttachWayTeiketsu.Name, iniPath));
                chkTurnBackle.Checked = bool.Parse(ClsIni.GetIniFile(sec, chkTurnBackle.Name, iniPath));
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

        //一括配置時の値参照ボタン
        private void button2_Click(object sender, EventArgs e)
        {
            var koudaiData = GantryUtil.GetKoudaiData(doc, cmbKoudaiName.Text);
            if (koudaiData == null || koudaiData.BraceTsunagiData == null || koudaiData.BraceTsunagiData.HorizontalBrace == null)
            {
                MessageUtil.Information("一括配置情報が存在しません。", this.Text);
                return;
            }
            else
            {
                if (MessageUtil.YesNo("一括配置情報が存在します。展開しますか？", this.Text) == DialogResult.No)
                    return;
                else
                {
                    //ダイアログに一括配置情報を展開
                    var hrzBracedata = koudaiData.BraceTsunagiData.HorizontalBrace;
                    CmbSize.Text = hrzBracedata.Size;
                    //checkBox13.Checked = hrzBracedata.PutTopPL;
                    NmcBanRangeX.Value = hrzBracedata.PutBanRangeX;
                    NmcBanRangeY.Value = hrzBracedata.PutBanRangeY;
                    RbtAttachWayWelding.Checked = hrzBracedata.ToritsukiYousetsu;
                    RbtAttachWayBolt.Checked = hrzBracedata.ToritsukiBolt;
                    RbtAttachWayTeiketsu.Checked = hrzBracedata.ToritsukiTeiketsuKanagu;
                }
            }
        }

        private void chkTurnBackle_CheckedChanged(object sender, EventArgs e)
        {
            InitComboBox();

            if (chkTurnBackle.Checked)
            {
                RbtPutWaySingle.Checked = true;
                //panel1.Enabled = false;
                groupBox13.Enabled = false;
                groupBox15.Enabled = false;
                groupBox2.Enabled = false;
            }
            else
            {
                //panel1.Enabled = true;
                groupBox13.Enabled = true;
                groupBox15.Enabled = true;
                groupBox2.Enabled = true;
            }
        }
    }


    public class FrmPutHorizontalBraceData : BindableSuper
    {
        public enum ConnectingType
        {
            Nil, Welding, Bolt, Metal
        }

        public enum SetFace
        {
            UpperB, LowerT, LowerB
        }

        public string SelectedKodai { get; set; }
        public string Size { get; set; }
        public bool PlaceIsSingle { get; set; }
        public bool PlaceIsMultiple { get; set; } = true;
        public bool IsTopPlate { get; set; }


        public int VrtBrace1UpperXMm { get; set; }
        public double VrtBrace1UpperXFt => ClsRevitUtil.CovertToAPI(VrtBrace1UpperXMm);
        public int VrtBrace1UpperYMm { get; set; }
        public double VrtBrace1UpperYFt => ClsRevitUtil.CovertToAPI(VrtBrace1UpperYMm);

        public SetFace SetObikiFace { get; set; }
        public ConnectingType VrtBraceConnectingType { get; set; }
        public bool IsTurnBackle { get; set; }
    }
}
