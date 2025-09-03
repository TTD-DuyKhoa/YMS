using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using YMS_gantry.Data;
using YMS_gantry.GantryUtils;
using YMS.gantry;
using System.IO;
using System.Linq;

namespace YMS_gantry.UI
{
    public partial class FrmPutKui : System.Windows.Forms.Form
    {
        #region 定数
        /// <summary>
        /// Iniファイルのパス
        /// </summary>
        const string IniPath = "ini\\FrmPutKui.ini";
        /// <summary>
        /// Iniファイルのセクション名
        /// </summary>
        const string sec = "FrmPutKui";
        #endregion

        private RequestHandler m_Handler;
        private ExternalEvent m_ExEvent;

        private UIApplication _uiApp { get; set; }
        private Document doc { get; set; }
        public FrmPutKui(UIApplication uiApp)
        {
            InitializeComponent();
            _uiApp = uiApp;
            doc = uiApp.ActiveUIDocument.Document;
        }
        public FrmPutKui(ExternalEvent exEvent, RequestHandler handler, UIApplication uiApp)
        {
            InitializeComponent();
            _uiApp = uiApp;
            doc = uiApp.ActiveUIDocument.Document;

            m_Handler = handler;
            m_ExEvent = exEvent;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //初期化処理
            MakeRequest(RequestId.End);
            m_ExEvent.Dispose();
            m_ExEvent = null;
            m_Handler = null;

            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void FrmPutKui_Load(object sender, EventArgs e)
        {   //構台名をコンボボックスに追加
            //comboBox12.Items.Clear();
            //comboBox12.Items.AddRange(GantryUtil.GetAllKoudaiName(doc).ToArray());
            ////comboBox12.Text = comboBox12.Items[0].ToString();

            ////レベル追加
            //comboBox18.Items.Clear();
            //comboBox18.Items.AddRange(GantryUtil.GetAllLevelName(doc).ToArray());
            //comboBox18.Text = comboBox18.Items[0].ToString();

            // コンボボックス初期化
            InitComboBox();
            GetIniData();
            chkHasCut();
        }


        /// <summary>
        /// コンボボックス初期化
        /// </summary>
        private void InitComboBox()
        {
            List<string> lstStr = new List<string>();

            //イベント
            CmbSizeType.SelectedIndexChanged += InitComboBoxKui;

            //レベル名
            lstStr = GantryUtil.GetAllLevelName(doc);
            ControlUtils.SetComboBoxItems(CmbLevel, lstStr);

            //全構台名
            lstStr = GantryUtil.GetAllKoudaiName(doc);
            ControlUtils.SetComboBoxItems(CmbKoudaiName, lstStr);

            //杭
            lstStr = new List<string>() {Master.ClsHBeamCsv.TypeHiro,
                                          Master.ClsHBeamCsv.TypeNaka ,
                Master.ClsHBeamCsv.TypeHoso
                                          };
            ControlUtils.SetComboBoxItems(CmbSizeType, lstStr);

            //トッププレート
            lstStr = Master.ClsTopPlateCsv.GetSizeList();
            ControlUtils.SetComboBoxItems(CmbTopPlateSize, lstStr, true);

            //材質
            lstStr = DefineUtil.ListMaterial;
            ControlUtils.SetComboBoxItems(CmbMaterial, lstStr);
        }

        /// <summary>
        /// コンボボックス初期化（杭）
        /// </summary>
        private void InitComboBoxKui(object sender, EventArgs e)
        {
            List<string> lstStr = new List<string>();

            //杭
            string type = CmbSizeType.Text;

            if (type == Master.ClsHBeamCsv.TypeHoso ||
                type == Master.ClsHBeamCsv.TypeNaka ||
                type == Master.ClsHBeamCsv.TypeHiro) //H鋼
            {
                lstStr = Master.ClsHBeamCsv.GetSizeList(type, true);
            }
            ControlUtils.SetComboBoxItems(CmbSize, lstStr);
        }

        /// <summary>
        /// 杭コントロール更新
        /// </summary>
        public void InitPileControl()
        {
            string bk = string.Empty;
            List<string> lstStr = new List<string>();

            if (CmbJointAttach.Text == Master.ClsPileTsugiteCsv.KoteiHohoBolt)
            {
                lstStr = Master.ClsBoltCsv.GetSizeList(Master.ClsBoltCsv.BoltTypeT);
            }
            else
            {
                lstStr = new List<string>() { string.Empty };
            }
            lstStr = Master.ClsBoltCsv.GetSizeList(Master.ClsBoltCsv.BoltTypeT);

            //ボルト1-1
            ControlUtils.SetComboBoxItems(CmbJointFlangeBolt, lstStr);

            //ボルト1-2
            ControlUtils.SetComboBoxItems(CmbJointWebBolt, lstStr);

            string strKotei = CmbJointAttach.Text;
            if (string.IsNullOrEmpty(strKotei))
                return;

            //CSVから継手情報を取得
            Master.ClsPileTsugiteCsv pileCsv = Master.ClsPileTsugiteCsv.GetCls(strKotei, CmbSize.Text);
            if (string.IsNullOrEmpty(pileCsv.PlateSizeFOut))
                return;

            //各コントロールに値を設定
            MaterialSize size = GantryUtil.GetKouzaiSize(pileCsv.PlateSizeFOut);
            if (size != null)
            {
                txtJointPlSizeFO1.Text = size.Thick.ToString();
                txtJointPlSizeFO2.Text = size.Height.ToString();
                txtJointPlSizeFO3.Text = size.Width.ToString();
                txtJointPlSizeFO4.Text = pileCsv.PlateNumFOut.ToString();
            }
            else
            {
                txtJointPlSizeFO1.Text = string.Empty;
                txtJointPlSizeFO2.Text = string.Empty;
                txtJointPlSizeFO3.Text = string.Empty;
                txtJointPlSizeFO4.Text = string.Empty;
            }

            size = GantryUtil.GetKouzaiSize(pileCsv.PlateSizeFIn);
            if (size != null)
            {
                txtJointPlSizeFI1.Text = size.Thick.ToString();
                txtJointPlSizeFI2.Text = size.Height.ToString();
                txtJointPlSizeFI3.Text = size.Width.ToString();
                txtJointPlSizeFI4.Text = pileCsv.PlateNumFIn.ToString();
            }
            else
            {
                txtJointPlSizeFI1.Text = string.Empty;
                txtJointPlSizeFI2.Text = string.Empty;
                txtJointPlSizeFI3.Text = string.Empty;
                txtJointPlSizeFI4.Text = string.Empty;
            }

            size = GantryUtil.GetKouzaiSize(pileCsv.PlateSizeW);
            if (size != null)
            {
                txtJointPlSizeW1.Text = size.Thick.ToString();
                txtJointPlSizeW2.Text = size.Height.ToString();
                txtJointPlSizeW3.Text = size.Width.ToString();
                txtJointPlSizeW4.Text = pileCsv.PlateNumW.ToString();
            }
            else
            {
                txtJointPlSizeW1.Text = string.Empty;
                txtJointPlSizeW2.Text = string.Empty;
                txtJointPlSizeW3.Text = string.Empty;
                txtJointPlSizeW4.Text = string.Empty;
            }

            CmbJointFlangeBolt.Text = pileCsv.BoltSizeF;
            txtJointBoltF.Text = pileCsv.BoltNumF.ToString();
            CmbJointWebBolt.Text = pileCsv.BoltSizeW;
            txtJointBoltW.Text = pileCsv.BoltNumW.ToString();
        }

        ///// <summary>
        ///// 杭継手コントロール更新
        ///// </summary>
        //public void InitExPileControl()
        //{
        //    string bk = string.Empty;
        //    List<string> lstStr2 = new List<string>();

        //    //if (this.CmbExAttach.Text == Master.ClsPileTsugiteCsv.KoteiHohoBolt)
        //    //{
        //    //    lstStr2 = Master.ClsBoltCsv.GetSizeList(Master.ClsBoltCsv.BoltTypeT);
        //    //}
        //    //else
        //    //{
        //        lstStr2 = new List<string>() { string.Empty };
        //    //}

        //    //ボルト2-1
        //    ControlUtils.SetComboBoxItems(CmbExFlangeBolt, lstStr2);

        //    //ボルト2-2
        //    ControlUtils.SetComboBoxItems(CmbExWebBolt, lstStr2);

        //    string strKotei2 = "溶接"/*this.CmbExAttach.Text*/;
        //    if (string.IsNullOrEmpty(strKotei2))
        //        return;

        //    Master.ClsPileTsugiteCsv pileCsv2 = Master.ClsPileTsugiteCsv.GetCls(strKotei2, CmbSize.Text);
        //    if (string.IsNullOrEmpty(pileCsv2.PlateSizeFOut))
        //        return;

        //    //各コントロールに値を設定
        //    MaterialSize size = GantryUtil.GetKouzaiSize(pileCsv2.PlateSizeFOut);
        //    if (size != null)
        //    {
        //        txtExJointPlSizeFO1.Text = size.Thick.ToString();
        //        txtExJointPlSizeFO2.Text = size.Height.ToString();
        //        txtExJointPlSizeFO3.Text = size.Width.ToString();
        //        txtExJointPlSizeFO4.Text = pileCsv2.PlateNumFOut.ToString();
        //    }
        //    else
        //    {
        //        txtExJointPlSizeFO1.Text = string.Empty;
        //        txtExJointPlSizeFO2.Text = string.Empty;
        //        txtExJointPlSizeFO3.Text = string.Empty;
        //        txtExJointPlSizeFO4.Text = string.Empty;
        //    }

        //    size = GantryUtil.GetKouzaiSize(pileCsv2.PlateSizeFIn);
        //    if (size != null)
        //    {
        //        txtExJointPlSizeFI1.Text = size.Thick.ToString();
        //        txtExJointPlSizeFI2.Text = size.Height.ToString();
        //        txtExJointPlSizeFI3.Text = size.Width.ToString();
        //        txtExJointPlSizeFI4.Text = pileCsv2.PlateNumFIn.ToString();
        //    }
        //    else
        //    {
        //        txtExJointPlSizeFI1.Text = string.Empty;
        //        txtExJointPlSizeFI2.Text = string.Empty;
        //        txtExJointPlSizeFI3.Text = string.Empty;
        //        txtExJointPlSizeFI4.Text = string.Empty;
        //    }

        //    size = GantryUtil.GetKouzaiSize(pileCsv2.PlateSizeW);
        //    if (size != null)
        //    {
        //        txtExJointPlSizeW1.Text = size.Thick.ToString();
        //        txtExJointPlSizeW2.Text = size.Height.ToString();
        //        txtExJointPlSizeW3.Text = size.Width.ToString();
        //        txtExJointPlSizeW4.Text = pileCsv2.PlateNumW.ToString();
        //    }
        //    else
        //    {
        //        txtExJointPlSizeW1.Text = string.Empty;
        //        txtExJointPlSizeW2.Text = string.Empty;
        //        txtExJointPlSizeW3.Text = string.Empty;
        //        txtExJointPlSizeW4.Text = string.Empty;
        //    }

        //    CmbExFlangeBolt.Text = pileCsv2.BoltSizeF;
        //    txtExJointBoltF.Text = pileCsv2.BoltNumF.ToString();
        //    CmbExWebBolt.Text = pileCsv2.BoltSizeW;
        //    txtExJointBoltW.Text = pileCsv2.BoltNumW.ToString();
        //}


        ///// <summary>
        ///// 杭継手コントロール更新
        ///// </summary>
        public void InitExPileControl()
        {
            //活性非活性
            bool bChkEx = ChkExPile.Checked;

            string bk = string.Empty;
            List<string> lstStr2 = new List<string>();

            if (this.label29.Text == Master.ClsPileTsugiteCsv.KoteiHohoBolt)
            {
                lstStr2 = Master.ClsBoltCsv.GetSizeList(Master.ClsBoltCsv.BoltTypeT);
            }
            else
            {
                lstStr2 = new List<string>() { string.Empty };
            }

            //ボルト2-1
            ControlUtils.SetComboBoxItems(CmbExFlangeBolt, lstStr2);

            //ボルト2-2
            ControlUtils.SetComboBoxItems(CmbExWebBolt, lstStr2);

            string strKotei2 = this.label29.Text;
            if (string.IsNullOrEmpty(strKotei2))
                return;

            Master.ClsPileTsugiteCsv pileCsv2 = Master.ClsPileTsugiteCsv.GetCls(strKotei2, CmbSize.Text);
            if (string.IsNullOrEmpty(pileCsv2.PlateSizeFOut))
                return;

            //各コントロールに値を設定
            MaterialSize size = GantryUtil.GetKouzaiSize(pileCsv2.PlateSizeFOut);
            if (size != null)
            {
                txtExJointPlSizeFO1.Text = size.Thick.ToString();
                txtExJointPlSizeFO2.Text = size.Height.ToString();
                txtExJointPlSizeFO3.Text = size.Width.ToString();
                txtExJointPlSizeFO4.Text = pileCsv2.PlateNumFOut.ToString();
            }
            else
            {
                txtExJointPlSizeFO1.Text = string.Empty;
                txtExJointPlSizeFO2.Text = string.Empty;
                txtExJointPlSizeFO3.Text = string.Empty;
                txtExJointPlSizeFO4.Text = string.Empty;
            }

            size = GantryUtil.GetKouzaiSize(pileCsv2.PlateSizeFIn);
            if (size != null)
            {
                txtExJointPlSizeFI1.Text = size.Thick.ToString();
                txtExJointPlSizeFI2.Text = size.Height.ToString();
                txtExJointPlSizeFI3.Text = size.Width.ToString();
                txtExJointPlSizeFI4.Text = pileCsv2.PlateNumFIn.ToString();
            }
            else
            {
                txtExJointPlSizeFI1.Text = string.Empty;
                txtExJointPlSizeFI2.Text = string.Empty;
                txtExJointPlSizeFI3.Text = string.Empty;
                txtExJointPlSizeFI4.Text = string.Empty;
            }

            size = GantryUtil.GetKouzaiSize(pileCsv2.PlateSizeW);
            if (size != null)
            {
                txtExJointPlSizeW1.Text = size.Thick.ToString();
                txtExJointPlSizeW2.Text = size.Height.ToString();
                txtExJointPlSizeW3.Text = size.Width.ToString();
                txtExJointPlSizeW4.Text = pileCsv2.PlateNumW.ToString();
            }
            else
            {
                txtExJointPlSizeW1.Text = string.Empty;
                txtExJointPlSizeW2.Text = string.Empty;
                txtExJointPlSizeW3.Text = string.Empty;
                txtExJointPlSizeW4.Text = string.Empty;
            }

            CmbExFlangeBolt.Text = pileCsv2.BoltSizeF;
            txtExJointBoltF.Text = pileCsv2.BoltNumF.ToString();
            CmbExWebBolt.Text = pileCsv2.BoltSizeW;
            txtExJointBoltW.Text = pileCsv2.BoltNumW.ToString();
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
            ClsIni.WritePrivateProfileString(sec, CmbLevel.Name, CmbLevel.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, NmcOffset.Name, NmcOffset.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, NmcKuiLng.Name, NmcKuiLng.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, NmcKuiWholeLng.Name, NmcKuiWholeLng.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, ChckKuiHasCut.Name, ChckKuiHasCut.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, NmcKuiHeadLng.Name, NmcKuiHeadLng.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, ChkTopPlate.Name, ChkTopPlate.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, CmbTopPlateSize.Name, CmbTopPlateSize.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, NmcJointCnt.Name, NmcJointCnt.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, CmbJointAttach.Name, CmbJointAttach.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, CmbJointFlangeBolt.Name, CmbJointFlangeBolt.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, CmbJointWebBolt.Name, CmbJointWebBolt.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, ChkExPile.Name, ChkExPile.Checked.ToString(), iniPath);
            ClsIni.WritePrivateProfileString(sec, NmcKuiExLng.Name, NmcKuiExLng.Text, iniPath);
            //ClsIni.WritePrivateProfileString(sec, CmbExAttach.Name, CmbExAttach.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, CmbExFlangeBolt.Name, CmbExFlangeBolt.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, CmbExWebBolt.Name, CmbExWebBolt.Text, iniPath);

            var rowSpanList = new List<string>();
            foreach (DataGridViewRow row in DgvJointSpan.Rows)
            {
                var rowSpanValue = row.Cells[1].Value?.ToString();
                if (string.IsNullOrEmpty(rowSpanValue)) continue;
                rowSpanList.Add(rowSpanValue);
            }

            var rowSpan = string.Join("|", rowSpanList);
            ClsIni.WritePrivateProfileString(sec, DgvJointSpan.Name, rowSpan, iniPath);
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
                CmbLevel.Text = ClsIni.GetIniFile(sec, CmbLevel.Name, iniPath);
                NmcOffset.Text = ClsIni.GetIniFile(sec, NmcOffset.Name, iniPath);
                NmcKuiLng.Text = ClsIni.GetIniFile(sec, NmcKuiLng.Name, iniPath);
                NmcKuiWholeLng.Text = ClsIni.GetIniFile(sec, NmcKuiWholeLng.Name, iniPath);
                ChckKuiHasCut.Checked = bool.Parse(ClsIni.GetIniFile(sec, ChckKuiHasCut.Name, iniPath));
                NmcKuiHeadLng.Text = ClsIni.GetIniFile(sec, NmcKuiHeadLng.Name, iniPath);
                ChkTopPlate.Checked = bool.Parse(ClsIni.GetIniFile(sec, ChkTopPlate.Name, iniPath));
                CmbTopPlateSize.Text = ClsIni.GetIniFile(sec, CmbTopPlateSize.Name, iniPath);
                NmcJointCnt.Text = ClsIni.GetIniFile(sec, NmcJointCnt.Name, iniPath);
                CmbJointAttach.Text = ClsIni.GetIniFile(sec, CmbJointAttach.Name, iniPath);
                CmbJointFlangeBolt.Text = ClsIni.GetIniFile(sec, CmbJointFlangeBolt.Name, iniPath);
                CmbJointWebBolt.Text = ClsIni.GetIniFile(sec, CmbJointWebBolt.Name, iniPath);
                ChkExPile.Checked = bool.Parse(ClsIni.GetIniFile(sec, ChkExPile.Name, iniPath));
                NmcKuiExLng.Text = ClsIni.GetIniFile(sec, NmcKuiExLng.Name, iniPath);
                //CmbExAttach.Text = ClsIni.GetIniFile(sec, CmbExAttach.Name, iniPath);
                CmbExFlangeBolt.Text = ClsIni.GetIniFile(sec, CmbExFlangeBolt.Name, iniPath);
                CmbExWebBolt.Text = ClsIni.GetIniFile(sec, CmbExWebBolt.Name, iniPath);

                var dgvJointSpanValue = ClsIni.GetIniFile(sec, DgvJointSpan.Name, iniPath);
                if (!string.IsNullOrEmpty(dgvJointSpanValue) && dgvJointSpanValue != "none")
                {
                    var dgvJointSpanArray = dgvJointSpanValue.Split('|');

                    for (var i = 0; i < dgvJointSpanArray.Count(); i++)
                    {
                        DgvJointSpan.Rows[i].Cells[1].Value = dgvJointSpanArray[i];
                    }
                }
            }
        }

        private void comboBox12_SelectedIndexChanged(object sender, EventArgs e)
        {
            string koudaiName = this.CmbKoudaiName.Text;
            KoudaiData data = GantryUtil.GetKoudaiData(doc, koudaiName);
            if (data.AllKoudaiFlatData != null && data.AllKoudaiFlatData.KoudaiName != "")
            {
                AllKoudaiFlatFrmData kdata = data.AllKoudaiFlatData;
                this.CmbSize.Text = kdata.pilePillerData.PillarSize;
                this.CmbMaterial.Text = kdata.pilePillerData.PillarMaterial;
                this.CmbLevel.Text = kdata.SelectedLevel;
                this.NmcOffset.Value = (decimal)kdata.LevelOffset;
                this.NmcKuiLng.Value = (decimal)kdata.pilePillerData.PillarLength;
                this.ChckKuiHasCut.Checked = kdata.pilePillerData.IsCut;
                this.NmcKuiWholeLng.Value = (decimal)kdata.pilePillerData.PillarWholeLength;

                this.ChkTopPlate.Checked = kdata.pilePillerData.HasTopPlate;
                this.CmbTopPlateSize.Text = kdata.pilePillerData.topPlateData.PlateSize;
                    //MaterialSize plateSize = GantryUtil.GetKouzaiSize(kdata.pilePillerData.topPlateData.PlateSize);
                    //this.NmcPLH.Value = (decimal)plateSize.Thick;
                    //this.NmcPLW.Value = (decimal)plateSize.Width;
                    //this.NmcPLD.Value = (decimal)plateSize.Height;
                this.NmcKuiHeadLng.Value = (decimal)kdata.pilePillerData.exPillarHeadLeng;

                //継手詳細
                this.NmcJointCnt.Value = kdata.pilePillerData.pJointCount;
                this.CmbJointAttach.Text = kdata.pilePillerData.jointDetailData.JointType == DefineUtil.eJoinType.Bolt ? "ボルト" : "溶接";
                DgvJointSpan.Rows.Clear();
                foreach (double pitch in kdata.pilePillerData.pJointPitch)
                {
                    if(kdata.pilePillerData.pJointPitch.IndexOf(pitch)== kdata.pilePillerData.pJointPitch.Count - 1 && pitch == 0) { continue; }
                    DgvJointSpan.Rows.Add(new string[] { $"{DgvJointSpan.Rows.Count + 1}", $"{pitch}" });
                }

                this.ChkExPile.Checked = kdata.pilePillerData.ExtensionPile;
                if(ChkExPile.Checked)
                {
                    this.NmcKuiExLng.Value = (decimal)kdata.pilePillerData.extensionPileData.Length;
                }
                //this.CmbExAttach.Text = kdata.pilePillerData.extensionPileData.attachType == DefineUtil.eAttachType.Bolt ? "ボルト" : "溶接";
                if (data.AllKoudaiFlatData.KoujiType == DefineUtil.eKoujiType.Doboku)
                {
                    this.Text = "[個別] 支持杭配置";
                }
                else
                {
                    this.Text = "[個別] 構台杭配置";
                }
            }
            CalcPileLength();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!CheckInpurValues())
            {
                return;
            }
            MakeRequest(RequestId.Kui);
            SetIniData();
            this.DialogResult = DialogResult.OK;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            decimal currentValue = NmcJointCnt.Value;

            // 以前の値が存在する場合、比較を行う
            if (NmcJointCnt.Tag != null)
            {
                decimal previousValue = decimal.Parse(NmcJointCnt.Tag.ToString());

                if (currentValue > previousValue)
                {
                    int dif = (int)(currentValue- previousValue);
                    // 増加した場合の処理
                    for (int i=0;i<dif;i++)
                    {
                        object[] aR = new object[] { $"{ DgvJointSpan.Rows.Count + 1}", 0 };
                        DgvJointSpan.Rows.Add(aR);
                    }
                }
                else if (currentValue < previousValue && DgvJointSpan.Rows.Count > 0)
                {
                    // 減少した場合の処理
                    int dif = (int)(previousValue - currentValue);
                    if(dif>0)
                    {
                        for (int i = 0; i < dif; i++)
                        {
                            DgvJointSpan.Rows.RemoveAt(DgvJointSpan.Rows.Count - 1);
                        }
                    }
                    else
                    {
                        DgvJointSpan.Rows.Clear();
                    }
                }
                else
                {
                    // 値が変化していない場合の処理
                }
            }

            // 現在の値を以前の値として保存
            NmcJointCnt.Tag = currentValue;
        }

        /// <summary>
        /// NumericupDownの空回避用処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numeriUpDown_Leave(object sender, EventArgs e)
        {
            NumericUpDown nmc = (NumericUpDown)sender;
            if (nmc.Text == "")
            {
                nmc.Value = 0;
                nmc.Text = "0";
            }
            base.OnLostFocus(e);
        }

        private void comboBox17_SelectedIndexChanged(object sender, EventArgs e)
        {
            string str = CmbTopPlateSize.Text;
            //bool bE = true;
            //if (str != Master.ClsTopPlateCsv.FreeSize)
            //{
            //    bE = false;
            //}
            //NmcPLH.Enabled = bE;
            //NmcPLW.Enabled = bE;
            //NmcPLD.Enabled = bE;
            CalcPileLength();
        }

        private void CmbJointAttach_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitPileControl();
        }

        private void CmbExAttach_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitExPileControl();
        }

        private void CmbSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            string size = CmbSize.Text;
            bool bEx = Master.ClsHBeamCsv.IsExPileFamily(size);
            if (bEx)
                ChkExPile.Enabled = true;
            else
            {
                ChkExPile.Checked = false;
                ChkExPile.Enabled = false;
            }

            InitPileControl();
            InitExPileControl();
        }

        private void ChkExPile_CheckedChanged(object sender, EventArgs e)
        {
            foreach (System.Windows.Forms.Control c in groupBox2.Controls)
            {
                if (c.Name == ChkExPile.Name)
                    continue;

                c.Enabled = ChkExPile.Checked;
            }
            InitExPileControl();
        }

        private void NmcOffset_ValueChanged(object sender, EventArgs e)
        {
            CalcPileLength();
        }

        private void NmcKuiLng_ValueChanged(object sender, EventArgs e)
        {
            CalcPileLength();
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
            if (string.IsNullOrEmpty(CmbLevel.Text))
            {
                errMsg.Add("配置するレベルを指定してください");
            }
            if(NmcKuiLng.Value<=0)
            {
                errMsg.Add("杭長を指定してください");
            }
            if(ChkTopPlate.Checked&&string.IsNullOrEmpty(CmbTopPlateSize.Text))
            {
                errMsg.Add("トッププレートサイズを指定してください");
            }

            if(NmcJointCnt.Value>0)
            {
                foreach(DataGridViewRow row in DgvJointSpan.Rows)
                {
                    double span =RevitUtil.ClsCommonUtils.ChangeStrToDbl(row.Cells[1].Value.ToString());
                    if(span<=0)
                    {
                        errMsg.Add("継手スパンが０に設定されています");
                        break;
                    }
                }

                if(CmbJointAttach.Text=="ボルト")
                {
                    if (string.IsNullOrEmpty(CmbJointFlangeBolt.Text))
                    {
                        errMsg.Add("継手フランジ側ボルト種類を指定してください");
                    }
                    if (string.IsNullOrEmpty(CmbJointWebBolt.Text))
                    {
                        errMsg.Add("継手ウェブ側ボルト種類を指定してください");
                    }
                }
            }
            if(ChkExPile.Checked&&NmcKuiExLng.Value<=0)
            {
                errMsg.Add("継足し杭の長さを設定してください");
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


        private void CalcPileLength()
        {

            AllKoudaiFlatFrmData kData = GantryUtil.GetKoudaiData(doc, this.CmbKoudaiName.Text).AllKoudaiFlatData;
            if (kData == null) { return; }
            double fukkouThick = DefineUtil.FukkouBAN_THICK;

            double nedaHeight = (kData.nedaData.NedaSize != "") ? GantryUtil.GetKouzaiSize(kData.nedaData.NedaSize).Height : 0;
            double ohbikiHeight = (kData.ohbikiData.OhbikiSize != "") ? GantryUtil.GetKouzaiSize(kData.ohbikiData.OhbikiSize).Height : 0;
            double topPLthick = (kData.pilePillerData.topPlateData.PlateSize != "") ? GantryUtil.GetKouzaiSize(kData.pilePillerData.topPlateData.PlateSize).Thick : 0;
            double offset = (double)this.NmcOffset.Value;

            bool isFukkouTop = kData.BaseLevel == DefineUtil.eBaseLevel.FukkouTop;
            double retLeng = (double)this.NmcKuiLng.Value;

            if (isFukkouTop)
            {
                if (this.ChckKuiHasCut.Checked)
                {
                    if (this.NmcOffset.Value < 0)
                    {
                        retLeng = retLeng + (fukkouThick + nedaHeight + ohbikiHeight) + Math.Abs(offset);
                    }
                    else
                    {
                        retLeng = retLeng + (fukkouThick + nedaHeight + ohbikiHeight) - offset;
                    }
                }
            }
            else
            {
                if (this.ChckKuiHasCut.Checked && this.ChkExPile.Checked)
                {
                    retLeng = retLeng - Math.Abs(offset);
                }
                else if (this.ChckKuiHasCut.Checked && !this.ChkExPile.Checked)
                {
                    if (this.NmcOffset.Value < 0)
                    {
                        retLeng = retLeng + Math.Abs(offset);
                    }
                }
            }

            this.NmcKuiWholeLng.Value = (decimal)retLeng;
        }
        private void chkHasCut()
        {
            bool hasCut = this.ChckKuiHasCut.Checked;
            string koudaiName = this.CmbKoudaiName.Text;
            KoudaiData data = GantryUtil.GetKoudaiData(doc, koudaiName);
            if (data.AllKoudaiFlatData!=null&&data.AllKoudaiFlatData.KoudaiName != "")
            {
                AllKoudaiFlatFrmData kData = data.AllKoudaiFlatData;
                if(kData.BaseLevel==DefineUtil.eBaseLevel.OhbikiBtm)
                {
                    this.ChkExPile.Checked = hasCut;
                    this.ChkExPile.Enabled = hasCut;
                    this.NmcKuiExLng.Enabled = hasCut;
                }
                else
                {
                    this.ChkExPile.Checked =false;
                    this.ChkExPile.Enabled =false;
                    this.NmcKuiExLng.Enabled = false;
                }

            }
            CalcPileLength();
        }

        private void ChckKuiHasCut_CheckedChanged(object sender, EventArgs e)
        {
            chkHasCut();
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

        private void NmcKuiExLng_ValueChanged(object sender, EventArgs e)
        {
            CalcPileLength();
        }

        private void CmbSizeType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void CmbLevel_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
