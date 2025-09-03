using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using YMS_gantry.Data;
using YMS_gantry.GantryUtils;
using YMS_gantry.Material;
using static YMS_gantry.DefineUtil;

namespace YMS_gantry.UI
{
    public partial class FrmAllPutKoudaiFlat : System.Windows.Forms.Form
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public AllKoudaiFlatFrmData data { get; set; }
        private Document doc { get; set; }
        private FrmPreview frmPreView { get; set; }
        public CalcFileData calcFileData { get; set; }

        public eKoujiType defaultKoujiType { get; private set; }

        public FrmAllPutKoudaiFlat(Document _doc)
        {
            InitializeComponent();
            doc = _doc;
            frmPreView = new FrmPreview();
        }

        /// <summary>
        /// 橋長方向柱追加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {

            if (RbtKuiKyoutyouPitch.Checked)
            {
                if (NmcKyoutyouLng.Value < 1)
                {
                    MessageUtil.Warning("橋長方向長さが0以下です", "杭追加", this);
                    return;
                }

                if (NmcKuiKyoutyouPitch.Value < 1)
                {
                    MessageUtil.Warning("橋長方向杭ピッチが0以下です", "杭追加", this);
                    return;
                }

                if (DgvKuiKyoutyouPitch.Rows.Count > 0 && MessageUtil.YesNo("追加した橋長方向の杭ピッチをすべて更新します。\r\n続行しますか?", "杭追加", this) != DialogResult.Yes)
                {
                    return;
                }


                DgvKuiKyoutyouPitch.Rows.Clear();
                int kyoutyou = (int)(Math.Ceiling(NmcKyoutyouLng.Value / 1000)) * 1000;
                double pitch = (double)NmcKuiKyoutyouPitch.Value;
                int kC = (int)Math.Floor(kyoutyou / pitch);

                DgvKuiKyoutyouPitch.Rows.Add(new string[] { $"1", "0" });
                for (int i = 0; i < kC; i++)
                {
                    DgvKuiKyoutyouPitch.Rows.Add(new string[] { $"{DgvKuiKyoutyouPitch.Rows.Count + 1}", $"{pitch}" });
                }

            }
            else
            {
                var row = new DataGridViewRow();
                row.CreateCells(this.DgvKuiKyoutyouPitch);

                if (this.DgvKuiKyoutyouPitch.SelectedRows.Count > 0 && this.DgvKuiKyoutyouPitch.SelectedRows[0].Index != this.DgvKuiKyoutyouPitch.Rows.Count - 1)
                {
                    row.Cells[0].Value = this.DgvKuiKyoutyouPitch.SelectedRows[0].Index + 2;
                    row.Cells[1].Value = "0";
                    for (int i = this.DgvKuiKyoutyouPitch.SelectedRows[0].Index + 1; i < this.DgvKuiKyoutyouPitch.Rows.Count; i++)
                    {
                        this.DgvKuiKyoutyouPitch.Rows[i].Cells[0].Value = i + 2;
                    }

                    this.DgvKuiKyoutyouPitch.Rows.Insert(this.DgvKuiKyoutyouPitch.SelectedRows[0].Index + 1, row);
                }
                else
                {
                    DgvKuiKyoutyouPitch.Rows.Add(new string[] { $"{DgvKuiKyoutyouPitch.Rows.Count + 1}", "0" });
                }
            }
        }

        /// <summary>
        /// 幅員方向柱追加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (RbtKuiHukuinPitch.Checked)
            {
                if (NmcHukuinLng.Value < 1)
                {
                    MessageUtil.Warning("幅員方向の長さが0以下です", "杭追加", this);
                    return;
                }

                if (NmcKuiHukuinPitch.Value < 1)
                {
                    MessageUtil.Warning("橋長方向杭ピッチが0以下です", "杭追加", this);
                    return;
                }

                if (DgvKuiHukuinPitch.Rows.Count > 0 && MessageUtil.YesNo("追加した幅員方向の杭ピッチをすべて更新します。\r\n続行しますか?", "杭追加", this) != DialogResult.Yes)
                {
                    return;
                }

                DgvKuiHukuinPitch.Rows.Clear();
                int kyoutyou = (int)(Math.Ceiling(NmcHukuinLng.Value / 1000)) * 1000;
                double pitch = (double)NmcKuiHukuinPitch.Value;
                int kC = (int)Math.Floor(kyoutyou / pitch);
                DgvKuiHukuinPitch.Rows.Add(new string[] { $"1", "0" });
                for (int i = 0; i < kC; i++)
                {
                    DgvKuiHukuinPitch.Rows.Add(new string[] { $"{DgvKuiHukuinPitch.Rows.Count + 1}", $"{pitch}" });
                }

            }
            else
            {
                var row = new DataGridViewRow();
                row.CreateCells(this.DgvKuiHukuinPitch);

                if (this.DgvKuiHukuinPitch.SelectedRows.Count > 0 && this.DgvKuiHukuinPitch.SelectedRows[0].Index != this.DgvKuiHukuinPitch.Rows.Count - 1)
                {
                    row.Cells[0].Value = this.DgvKuiHukuinPitch.SelectedRows[0].Index + 2;
                    row.Cells[1].Value = "0";
                    for (int i = this.DgvKuiHukuinPitch.SelectedRows[0].Index + 1; i < this.DgvKuiHukuinPitch.Rows.Count; i++)
                    {
                        this.DgvKuiHukuinPitch.Rows[i].Cells[0].Value = i + 2;
                    }

                    this.DgvKuiHukuinPitch.Rows.Insert(this.DgvKuiHukuinPitch.SelectedRows[0].Index + 1, row);
                }
                else
                {
                    DgvKuiHukuinPitch.Rows.Add(new string[] { $"{DgvKuiHukuinPitch.Rows.Count + 1}", "0" });
                }
            }
        }

        /// <summary>
        /// フォームロードイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmAllPutKoudaiFlat_Load(object sender, EventArgs e)
        {
            // コンボボックス初期化
            InitComboBox();
            // 画面データの初期化
            data = AllKoudaiFlatFrmData.NewInstance();

            // プロジェクト情報より設計指針を取得
            var zumenInfo = new ZumenInfo();
            zumenInfo.GetProjectInfo(doc);

            var _koujiType = eKoujiType.None;
            var nedaShugentLabel = DefineUtil.NEDA;
            var ohbikiKetaukeLabel = DefineUtil.OHBIKI;

            if (zumenInfo.m_sekkeishishin == "建築")
            {
                data.KoujiType = eKoujiType.Kenchiku;
                _koujiType = data.KoujiType;
            }
            else if (zumenInfo.m_sekkeishishin == "土木")
            {
                data.KoujiType = eKoujiType.Doboku;
                _koujiType = data.KoujiType;
                nedaShugentLabel = DefineUtil.SHUGETA;
                ohbikiKetaukeLabel = DefineUtil.KETAUKE;
            }

            this.defaultKoujiType = _koujiType;
            this.LblNedaShugentSize.Text = nedaShugentLabel;
            this.LblNedaShugetaTsukidashi.Text = nedaShugentLabel;
            this.TabNedaShugeta.Text = nedaShugentLabel;
            this.LblOhbikiKetaukeSize.Text = ohbikiKetaukeLabel;
            this.LblOhbikiKetaukeTsukidashi.Text = ohbikiKetaukeLabel;
            this.TabOhbikiShugeta.Text = ohbikiKetaukeLabel;

            // 画面データを画面コントロールに展開
            DataToCntrl();

            //強制ビュー切り替えの間違い防止のため、レベルの初期値は空欄(前回値保持でも)
            this.CmbBaseLevel.Items.Insert(0, "");
            this.CmbBaseLevel.SelectedIndex = 0;

            bool isCut = this.ChckKuiHasCut.Checked;
            this.ChckKuiHasEx.Checked = isCut;
            this.ChckKuiHasEx.Enabled = isCut;
            this.NmcKuiExLng.Enabled = isCut;
            if (!isCut)
            {
                this.NmcKuiExLng.Value = 0;
            }
            UpdateFukkoubanSize();
            ChangePictureForPileTips();
            //テストデータ入力ボタンの表示
#if DEBUG
            button5.Enabled = true;
            button5.Visible = true;
#else
            button5.Enabled = false;
            button5.Visible = false;
#endif
        }

        /// <summary>
        /// コンボボックス初期化
        /// </summary>
        private void InitComboBox()
        {
            List<string> lstStr = new List<string>();

            //イベント
            CmbNedaType.SelectedIndexChanged += InitComboBoxNeda;
            CmbOhbikiType.SelectedIndexChanged += InitComboBoxOhbiki;
            CmbShikiketaType.SelectedIndexChanged += InitComboBoxShikiketa;
            CmbKuiType.SelectedIndexChanged += InitComboBoxKui;

            //レベル名
            lstStr = GantryUtil.GetAllLevelName(doc);
            ControlUtils.SetComboBoxItems(CmbBaseLevel, lstStr);

            //全構台名
            lstStr = GantryUtil.GetAllKoudaiName(doc);
            ControlUtils.SetComboBoxItems(CmbKoudaiName, lstStr, false);

            //覆工板
            lstStr = Master.ClsFukkoubanCsv.GetSizeList(Master.ClsFukkoubanCsv.TypeM);
            lstStr.AddRange(Master.ClsFukkoubanCsv.GetSizeList(Master.ClsFukkoubanCsv.TypeNormal));
            ControlUtils.SetComboBoxItems(CmbFukkouSize, lstStr);

            //根太
            lstStr = new List<string>() { Master.ClsHBeamCsv.TypeHiro,
                                          Master.ClsHBeamCsv.TypeNaka,
                                          Master.ClsHBeamCsv.TypeHoso,
                                          Master.ClsYamadomeCsv.TypeShuzai,
                                          Master.ClsYamadomeCsv.TypeHighShuzai};
            ControlUtils.SetComboBoxItems(CmbNedaType, lstStr);

            //大引
            if (this.RbtKenchiku.Checked)
            {
                lstStr = new List<string>() { Master.ClsHBeamCsv.TypeHiro,
                                          Master.ClsHBeamCsv.TypeNaka,
                                          Master.ClsHBeamCsv.TypeHoso,
                                          Master.ClsYamadomeCsv.TypeShuzai,
                                          Master.ClsYamadomeCsv.TypeHighShuzai};
            }
            else
            {
                lstStr = new List<string>() { Master.ClsHBeamCsv.TypeHiro,
                                          Master.ClsHBeamCsv.TypeNaka,
                                          Master.ClsHBeamCsv.TypeHoso,
                                          Master.ClsYamadomeCsv.TypeShuzai,
                                          Master.ClsYamadomeCsv.TypeHighShuzai,
                                          Master.ClsAngleCsv.TypeAngle,
                                          Master.ClsChannelCsv.TypeCannel};
            }
            ControlUtils.SetComboBoxItems(CmbOhbikiType, lstStr);

            //杭
            lstStr = new List<string>() { Master.ClsHBeamCsv.TypeHiro,
                                          Master.ClsHBeamCsv.TypeNaka ,
                                          Master.ClsHBeamCsv.TypeHoso};
            ControlUtils.SetComboBoxItems(CmbKuiType, lstStr);

            //敷桁
            lstStr = new List<string>() { Master.ClsHBeamCsv.TypeHiro,
                                          Master.ClsHBeamCsv.TypeNaka,
                                          Master.ClsHBeamCsv.TypeHoso,
                                          Master.ClsYamadomeCsv.TypeShuzai,
                                          Master.ClsYamadomeCsv.TypeHighShuzai};
            ControlUtils.SetComboBoxItems(CmbShikiketaType, lstStr);

            //トッププレート
            lstStr = Master.ClsTopPlateCsv.GetSizeList();
            ControlUtils.SetComboBoxItems(CmbKuiTopPLSize, lstStr,true);

            //固定方法(継手)
            lstStr = new List<string>() { Master.ClsPileTsugiteCsv.KoteiHohoBolt, Master.ClsPileTsugiteCsv.KoteiHohoYousetsu };
            ControlUtils.SetComboBoxItems(CmbKuiJointType, lstStr, true);

            //固定方法(継足杭)
            //lstStr = new List<string>() { Master.ClsPileTsugiteCsv.KoteiHohoBolt, Master.ClsPileTsugiteCsv.KoteiHohoYousetsu };
            //ControlUtils.SetComboBoxItems(CmbKuiExJointType, lstStr);
            this.txtExPileJointType.Text = "溶接";

            ////ボルト
            //lstStr = Master.ClsBoltCsv.GetSizeList(Master.ClsBoltCsv.BoltTypeT);
            //ControlUtils.SetComboBoxItems(CmbKuiJointBoltSizeF, lstStr);
            //ControlUtils.SetComboBoxItems(CmbKuiJointBoltSizeW, lstStr);
            //ControlUtils.SetComboBoxItems(CmbKuiExBoltSizeF, lstStr);
            //ControlUtils.SetComboBoxItems(CmbKuiExBoltSizeW, lstStr);
        }

        /// <summary>
        /// コンボボックス初期化（根太）
        /// </summary>
        private void InitComboBoxNeda(object sender, EventArgs e)
        {
            List<string> lstStr = new List<string>();

            //根太
            string type = CmbNedaType.Text;

            if (type == Master.ClsYamadomeCsv.TypeShuzai ||
                type == Master.ClsYamadomeCsv.TypeHighShuzai) //主材
            {
                lstStr = Master.ClsYamadomeCsv.GetSizeList(type);
            }
            else if (type == Master.ClsHBeamCsv.TypeHoso ||
                type == Master.ClsHBeamCsv.TypeNaka ||
                type == Master.ClsHBeamCsv.TypeHiro) //H鋼
            {
                lstStr = Master.ClsHBeamCsv.GetSizeList(type);
            }
            else if (type == Master.ClsChannelCsv.TypeCannel) //C材
            {
                lstStr = Master.ClsChannelCsv.GetSizeList();
            }
            else if (type == Master.ClsAngleCsv.TypeAngle) //L材
            {
                lstStr = Master.ClsAngleCsv.GetSizeList(type);
            }
            ControlUtils.SetComboBoxItems(CmbNedaSize, lstStr);
        }

        /// <summary>
        /// コンボボックス初期化（大引き）
        /// </summary>
        private void InitComboBoxOhbiki(object sender, EventArgs e)
        {
            List<string> lstStr = new List<string>();

            //大引き
            string type = CmbOhbikiType.Text;

            if (type == Master.ClsYamadomeCsv.TypeShuzai ||
                type == Master.ClsYamadomeCsv.TypeHighShuzai) //主材
            {
                lstStr = Master.ClsYamadomeCsv.GetSizeList(type);
            }
            else if (type == Master.ClsHBeamCsv.TypeHoso ||
                type == Master.ClsHBeamCsv.TypeNaka ||
                type == Master.ClsHBeamCsv.TypeHiro) //H鋼
            {
                lstStr = Master.ClsHBeamCsv.GetSizeList(type);
            }
            else if (type == Master.ClsChannelCsv.TypeCannel) //C材
            {
                lstStr = Master.ClsChannelCsv.GetSizeList();
            }
            else if (type == Master.ClsAngleCsv.TypeAngle) //L材
            {
                lstStr = Master.ClsAngleCsv.GetSizeList(type);
            }
            ControlUtils.SetComboBoxItems(CmbOhbikiSize, lstStr);
        }

        /// <summary>
        /// コンボボックス初期化（杭）
        /// </summary>
        private void InitComboBoxKui(object sender, EventArgs e)
        {
            List<string> lstStr = new List<string>();

            //杭
            string type = CmbKuiType.Text;

            if (type == Master.ClsYamadomeCsv.TypeShuzai ||
                type == Master.ClsYamadomeCsv.TypeHighShuzai) //主材
            {
                lstStr = Master.ClsYamadomeCsv.GetSizeList(type);
            }
            else if (type == Master.ClsHBeamCsv.TypeHoso ||
                type == Master.ClsHBeamCsv.TypeNaka ||
                type == Master.ClsHBeamCsv.TypeHiro) //H鋼
            {
                lstStr = Master.ClsHBeamCsv.GetSizeList(type, true);
            }
            ControlUtils.SetComboBoxItems(CmbKuiSize, lstStr);
        }

        /// <summary>
        /// コンボボックス初期化（敷桁）
        /// </summary>
        private void InitComboBoxShikiketa(object sender, EventArgs e)
        {
            List<string> lstStr = new List<string>();

            //敷桁
            string type = CmbShikiketaType.Text;

            if (type == Master.ClsYamadomeCsv.TypeShuzai ||
                type == Master.ClsYamadomeCsv.TypeHighShuzai) //主材
            {
                lstStr = Master.ClsYamadomeCsv.GetSizeList(type);
            }
            else if (type == Master.ClsHBeamCsv.TypeHoso ||
                type == Master.ClsHBeamCsv.TypeNaka ||
                type == Master.ClsHBeamCsv.TypeHiro) //H鋼
            {
                lstStr = Master.ClsHBeamCsv.GetSizeList(type);
            }
            else if (type == Master.ClsChannelCsv.TypeCannel) //C材
            {
                lstStr = Master.ClsChannelCsv.GetSizeList();
            }
            else if (type == Master.ClsAngleCsv.TypeAngle) //L材
            {
                lstStr = Master.ClsAngleCsv.GetSizeList(type);
            }
            ControlUtils.SetComboBoxItems(CmbShikiketaSize, lstStr);
        }

        /// <summary>
        /// 杭継手タブのコントロール更新
        /// </summary>
        public void InitPileControl()
        {
            string bk = string.Empty;
            List<string> lstStr = new List<string>();

            if (CmbKuiJointType.Text == Master.ClsPileTsugiteCsv.KoteiHohoBolt)
            {
                lstStr = Master.ClsBoltCsv.GetSizeList(Master.ClsBoltCsv.BoltTypeT);
            }
            else
            {
                lstStr = new List<string>() { string.Empty };
            }

            //ボルト1-1
            ControlUtils.SetComboBoxItems(CmbKuiJointBoltSizeF, lstStr);

            //ボルト1-2
            ControlUtils.SetComboBoxItems(CmbKuiJointBoltSizeW, lstStr);


            string strKotei = CmbKuiJointType.Text;
            if (string.IsNullOrEmpty(strKotei))
                return;

            //CSVから継手情報を取得
            Master.ClsPileTsugiteCsv pileCsv = Master.ClsPileTsugiteCsv.GetCls(strKotei, CmbKuiSize.Text);
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

            CmbKuiJointBoltSizeF.Text = pileCsv.BoltSizeF;
            txtJointBoltF.Text = pileCsv.BoltNumF.ToString();
            CmbKuiJointBoltSizeW.Text = pileCsv.BoltSizeW;
            txtJointBoltW.Text = pileCsv.BoltNumW.ToString();
        }

        /// <summary>
        /// 杭継手タブのコントロール更新
        /// </summary>
        public void InitExPileControl()
        {
            //活性非活性
            bool bChkEx = ChckKuiHasEx.Checked;
            groupBox15.Visible = bChkEx;

            string bk = string.Empty;
            List<string> lstStr2 = new List<string>();

            if (this.txtExPileJointType.Text == Master.ClsPileTsugiteCsv.KoteiHohoBolt)
            {
                lstStr2 = Master.ClsBoltCsv.GetSizeList(Master.ClsBoltCsv.BoltTypeT);
            }
            else
            {
                lstStr2 = new List<string>() { string.Empty };
            }

            //ボルト2-1
            ControlUtils.SetComboBoxItems(CmbKuiExBoltSizeF, lstStr2);

            //ボルト2-2
            ControlUtils.SetComboBoxItems(CmbKuiExBoltSizeW, lstStr2);

            string strKotei2 = this.txtExPileJointType.Text;
            if (string.IsNullOrEmpty(strKotei2))
                return;

            Master.ClsPileTsugiteCsv pileCsv2 = Master.ClsPileTsugiteCsv.GetCls(strKotei2, CmbKuiSize.Text);
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

            CmbKuiExBoltSizeF.Text = pileCsv2.BoltSizeF;
            txtExJointBoltF.Text = pileCsv2.BoltNumF.ToString();
            CmbKuiExBoltSizeW.Text = pileCsv2.BoltSizeW;
            txtExJointBoltW.Text = pileCsv2.BoltNumW.ToString();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// プレビュー表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            if (!CheckInPutValues())
            {
                return;
            }
            CntrlToData();
            if (frmPreView.Visible)
            {
                frmPreView.DialogResult = DialogResult.None;
                frmPreView.Close();
            }
            frmPreView = new FrmPreview(data);
            //モーダレスにして結果だけ取得する
            frmPreView.FormClosed += new FormClosedEventHandler(Preview_Closed);
            frmPreView.Show(this);
        }

        /// <summary>
        /// 閉じられた結果取得
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Preview_Closed(object sender, FormClosedEventArgs e)
        {
            FrmPreview f = (FrmPreview)sender;
            //結果を表示する
            if (f.DialogResult == DialogResult.OK)
            {
                this.BtnOK.PerformClick();
            }
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (this.ChckNoFukkouban.Checked)
            {
                this.label29.Enabled = false;
                this.CmbFukkouMaterial.Enabled = false;
                this.groupBox9.Enabled = false;
                return;
            }

            this.label29.Enabled = true;
            this.CmbFukkouMaterial.Enabled = true;
            this.groupBox9.Enabled = true;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (this.CmbOhbikiType.SelectedItem.ToString() != Master.ClsHBeamCsv.TypeHiro ||
            //    this.CmbOhbikiType.SelectedItem.ToString() != Master.ClsHBeamCsv.TypeHoso ||
            //    this.CmbOhbikiType.SelectedItem.ToString() != Master.ClsHBeamCsv.TypeNaka)
            //{
            //    this.groupBox19.Enabled = true;
            //    return;
            //}
            bool isHkou = !string.IsNullOrEmpty(this.CmbOhbikiType.Text) && (!this.CmbOhbikiType.SelectedItem.ToString().Equals(Master.ClsAngleCsv.TypeAngle) &&
                                                            !this.CmbOhbikiType.SelectedItem.ToString().Equals(Master.ClsChannelCsv.TypeCannel));
            //H鋼で不要な設定は触らせない
            NmcOhbikiBoltCnt.Enabled = !isHkou;
            RbtOhbikiDouble.Enabled = !isHkou;
            RbtOhbikiTriple.Enabled = !isHkou;
            NmcKuiHeadLng.Enabled = !isHkou;
            if (isHkou)
            {
                RbtOhbikiSingle.Checked = true;
            }
            else
            {
                NmcKuiHeadLng.Value = 0;
            }
           
            this.groupBox19.Enabled = !isHkou;
        }

        /// <summary>
        /// 画面情報をデータクラスに格納
        /// </summary>
        private void CntrlToData()
        {
            data = AllKoudaiFlatFrmData.NewInstance();
            //構台データ
            data.KoudaiType = CmbKoudaiType.Text;
            data.KoudaiName = CmbKoudaiName.Text;
            data.KoujiType = (RbtKenchiku.Checked) ? eKoujiType.Kenchiku : eKoujiType.Doboku;
            data.BaseLevel = (RbtOhbikiBtm.Checked) ? eBaseLevel.OhbikiBtm : eBaseLevel.FukkouTop;
            data.SelectedLevel = CmbBaseLevel.Text;
            data.LevelOffset = (double)NmcLevelOffset.Value;
            data.KyoutyouLength = (double)NmcKyoutyouLng.Value;
            data.HukuinLength = (double)NmcHukuinLng.Value;
            //覆工板データ
            data.HasFukkouban = !ChckNoFukkouban.Checked;
            data.FukkoubanData.FukkouMaterial = CmbFukkouMaterial.Text;
            data.FukkoubanData.FukkoubanSize = CmbFukkouSize.Text;
            List<double> hPitch = new List<double>();
            foreach (DataGridViewRow row in DgvFukkouban.Rows)
            {
                var fukkoubanSize = row.Cells[1].Value.ToString();
                var matcheValue = Regex.Match(fukkoubanSize, @"X[0-9]*").Value;
                var fukkoubanPitchValue = matcheValue.Replace("X", "");
                if (int.TryParse(fukkoubanPitchValue, out var fukkoubanPitch))
                {
                    data.FukkoubanPitch.Add(fukkoubanPitch);
                    data.FukkoubanSizeList.Add(fukkoubanSize);
                }
            }
            //根太データ
            data.nedaData.NedaMaterial = CmbNedaMaterial.Text;
            data.nedaData.NedaType = CmbNedaType.Text;
            data.nedaData.NedaSize = CmbNedaSize.Text;
            foreach (DataGridViewRow row in DgvNedaPitch.Rows)
            {
                data.NedaPitch.Add(RevitUtil.ClsCommonUtils.ChangeStrToDbl(row.Cells[1].Value.ToString()));
            }
            data.nedaData.exNedaStartLeng = (double)NmcNedaSLng.Value;
            data.nedaData.exNedaEndLeng = (double)NmcNedaELng.Value;
            //大引データ
            data.ohbikiData.OhbikiMaterial = CmbOhbikiMaterial.Text;
            data.ohbikiData.OhbikiType = CmbOhbikiType.Text;
            data.ohbikiData.OhbikiSize = CmbOhbikiSize.Text;
            data.ohbikiData.exOhbikiStartLeng = (double)NmcOhbikiSLng.Value;
            data.ohbikiData.exOhbikiEndLeng = (double)NmcOhbikiELng.Value;
            //Del H鋼以外の大引はすべて両側に
            //data.ohbikiData.OhbikiAttachWay = radioButton14.Checked ? eAttachWay.OneSide : eAttachWay.BothSide;
            data.ohbikiData.OhbikiAttachType = RbtOhbikiBolt.Checked ? eJoinType.Bolt : eJoinType.Welding;
            data.ohbikiData.OhbikiBoltType = RbtOhbikiNormalB.Checked ? eBoltType.Normal : eBoltType.HTB;
            data.ohbikiData.OhbikiBoltCount = (int)NmcOhbikiBoltCnt.Value;
            data.IsFirstShikigeta = ChckSikiketaStart.Checked;
            data.IsLastShikigeta = ChckSikiketaEnd.Checked;
            data.shikigetaData.ShikigetaMaterial = CmbShikiketaMaterial.Text;
            data.shikigetaData.ShikigetaType = CmbShikiketaType.Text;
            data.shikigetaData.ShikigetaSize = CmbShikiketaSize.Text;
            data.shikigetaData.exShikigetaStartLeng = (double)NmcShikiketaSLng.Value;
            data.shikigetaData.exShikigetaEndLeng = (double)NmcShikiketaELng.Value;
            data.ohbikiData.isHkou = CmbOhbikiType.Text.Equals(Master.ClsHBeamCsv.TypeHiro) || CmbOhbikiType.Text.Equals(Master.ClsHBeamCsv.TypeHoso) || CmbOhbikiType.Text.Equals(Master.ClsHBeamCsv.TypeNaka)
                || CmbOhbikiType.Text.Equals(Master.ClsYamadomeCsv.TypeShuzai) || CmbOhbikiType.Text.Equals(Master.ClsYamadomeCsv.TypeHighShuzai);
            data.ohbikiData.OhbikiDan = (this.RbtOhbikiSingle.Checked) ? 1 : ((this.RbtOhbikiDouble.Checked) ? 2 : 3);
            //柱データ
            data.pilePillerData.PillarType = CmbKuiType.Text;
            data.pilePillerData.PillarSize = CmbKuiSize.Text;
            data.HasPilePiller = !ChckExistPile.Checked;
            data.pilePillerData.PillarMaterial = CmbKuiMaterial.Text;
            data.pilePillerData.PillarWholeLength = (double)NmcKuiWholeLng.Value;
            data.pilePillerData.IsCut = ChckKuiHasCut.Checked;
            data.pilePillerData.PileCutLength =
            data.pilePillerData.PillarLength = (double)NmcKuiLng.Value;
            data.pilePillerData.exPillarHeadLeng = (double)NmcKuiHeadLng.Value;
            data.beginNedaDiff = (double)NmcKuiNedaDiff.Value;
            foreach (DataGridViewRow row in DgvKuiKyoutyouPitch.Rows)
            {
                data.KyoutyouPillarPitch.Add(RevitUtil.ClsCommonUtils.ChangeStrToDbl(row.Cells[1].Value.ToString()));
            }
            foreach (DataGridViewRow row in DgvKuiHukuinPitch.Rows)
            {
                data.HukuinPillarPitch.Add(RevitUtil.ClsCommonUtils.ChangeStrToDbl(row.Cells[1].Value.ToString()));
            }

            //杭詳細データ
            //継足し杭
            data.pilePillerData.ExtensionPile = ChckKuiHasEx.Checked;
            data.pilePillerData.extensionPileData.Length = (double)NmcKuiExLng.Value;
            data.pilePillerData.extensionPileData.attachType = this.txtExPileJointType.Text == "ボルト" ? eJoinType.Bolt : eJoinType.Welding;
            JointPlateData jpd = new JointPlateData();
            jpd.PlateSize = Master.ClsPileTsugiteCsv.CreatePlateName(txtExJointPlSizeFO1.Text, txtExJointPlSizeFO2.Text, txtExJointPlSizeFO3.Text);
            jpd.PlateCount = ClsCommonUtils.ChangeStrToInt(txtExJointPlSizeFO4.Text);
            data.pilePillerData.extensionPileData.PlateFlangeOutSide = jpd;
            jpd = new JointPlateData();
            jpd.PlateSize = Master.ClsPileTsugiteCsv.CreatePlateName(txtExJointPlSizeFI1.Text, txtExJointPlSizeFI2.Text, txtExJointPlSizeFI3.Text);
            jpd.PlateCount = ClsCommonUtils.ChangeStrToInt(txtExJointPlSizeFI4.Text);
            data.pilePillerData.extensionPileData.PlateFlangeInSide = jpd;
            jpd = new JointPlateData();
            jpd.PlateSize = Master.ClsPileTsugiteCsv.CreatePlateName(txtExJointPlSizeW1.Text, txtExJointPlSizeW2.Text, txtExJointPlSizeW3.Text);
            jpd.PlateCount = ClsCommonUtils.ChangeStrToInt(txtExJointPlSizeW4.Text);
            data.pilePillerData.extensionPileData.PlateWeb = jpd;
            JointBoltData jbd = new JointBoltData();
            jbd.BoltSize = CmbKuiExBoltSizeF.Text;
            jbd.BoltCount = ClsCommonUtils.ChangeStrToInt(txtExJointBoltF.Text);
            data.pilePillerData.extensionPileData.FlangeBolt = jbd;
            jbd = new JointBoltData();
            jbd.BoltSize = CmbKuiExBoltSizeW.Text;
            jbd.BoltCount = ClsCommonUtils.ChangeStrToInt(txtExJointBoltW.Text);
            data.pilePillerData.extensionPileData.WebBolt = jbd;

            //継手詳細
            data.pilePillerData.pJointCount = (int)NmcKuiJointCnt.Value;
            foreach (DataGridViewRow row in DgvKuiJointPitch.Rows)
            {
                double d = RevitUtil.ClsCommonUtils.ChangeStrToDbl(row.Cells[1].Value.ToString());
                if (d == 0) { continue; }
                data.pilePillerData.pJointPitch.Add(d);
            }
            data.pilePillerData.jointDetailData.JointType = CmbKuiJointType.Text == "ボルト" ? eJoinType.Bolt : eJoinType.Welding;
            jpd = new JointPlateData();
            jpd.PlateSize = Master.ClsPileTsugiteCsv.CreatePlateName(txtJointPlSizeFO1.Text, txtJointPlSizeFO2.Text, txtJointPlSizeFO3.Text);
            jpd.PlateCount = ClsCommonUtils.ChangeStrToInt(txtJointPlSizeFO4.Text);
            data.pilePillerData.jointDetailData.PlateFlangeOutSide = jpd;
            jpd = new JointPlateData();
            jpd.PlateSize = Master.ClsPileTsugiteCsv.CreatePlateName(txtJointPlSizeFI1.Text, txtJointPlSizeFI2.Text, txtJointPlSizeFI3.Text);
            jpd.PlateCount = ClsCommonUtils.ChangeStrToInt(txtJointPlSizeFI4.Text);
            data.pilePillerData.jointDetailData.PlateFlangeInSide = jpd;
            jpd = new JointPlateData();
            jpd.PlateSize = Master.ClsPileTsugiteCsv.CreatePlateName(txtJointPlSizeW1.Text, txtJointPlSizeW2.Text, txtJointPlSizeW3.Text);
            jpd.PlateCount = ClsCommonUtils.ChangeStrToInt(txtJointPlSizeW4.Text);
            data.pilePillerData.jointDetailData.PlateWeb = jpd;
            jbd = new JointBoltData();
            jbd.BoltSize = CmbKuiJointBoltSizeF.Text;
            jbd.BoltCount = ClsCommonUtils.ChangeStrToInt(txtJointBoltF.Text);
            data.pilePillerData.jointDetailData.FlangeBolt = jbd;
            jbd = new JointBoltData();
            jbd.BoltSize = CmbKuiJointBoltSizeW.Text;
            jbd.BoltCount = ClsCommonUtils.ChangeStrToInt(txtJointBoltW.Text);
            data.pilePillerData.jointDetailData.WebBolt = jbd;

            //トッププレート
            data.pilePillerData.HasTopPlate = ChckKuiHasTopPL.Checked;
            //if (CmbKuiTopPLSize.Text == "任意")
            //{
            //    data.pilePillerData.topPlateData.PlateSize = $"PL-{NmcKuiTopPLH.Value}x{NmcKuiTopPLW.Value}x{NmcKuiTopPLD.Value}";
            //    data.pilePillerData.extensionPileData.topPlateData.PlateSize = $"PL-{NmcKuiTopPLH.Value}x{NmcKuiTopPLW.Value}x{NmcKuiTopPLD.Value}";
            //}
            //else
            //{
            data.pilePillerData.topPlateData.PlateSize = CmbKuiTopPLSize.Text;
            data.pilePillerData.extensionPileData.topPlateData.PlateSize = CmbKuiTopPLSize.Text;
            //}

        }

        /// <summary>
        /// データクラスを画面情報に展開
        /// </summary>
        private void DataToCntrl()
        {
            //構台データ
            CmbKoudaiType.Text = data.KoudaiType;
            CmbKoudaiName.Text = data.KoudaiName;
            RbtKenchiku.Checked = data.KoujiType == eKoujiType.Kenchiku;
            RbtDoboku.Checked = data.KoujiType == eKoujiType.Doboku;
            RbtOhbikiBtm.Checked = data.BaseLevel == eBaseLevel.OhbikiBtm;
            RbtFukkouTop.Checked = data.BaseLevel == eBaseLevel.FukkouTop;
            CmbBaseLevel.Text = data.SelectedLevel;
            NmcLevelOffset.Value = (decimal)data.LevelOffset;
            NmcKyoutyouLng.Value = (decimal)data.KyoutyouLength;
            NmcHukuinLng.Value = (decimal)data.HukuinLength;
            //覆工板データ
            ChckNoFukkouban.Checked = !data.HasFukkouban;
            CmbFukkouMaterial.Text = data.FukkoubanData.FukkouMaterial;
            CmbFukkouSize.Text = data.FukkoubanData.FukkoubanSize;
            List<double> hPitch = new List<double>();
            DgvFukkouban.Rows.Clear();
            for (int i = 0; i < data.FukkoubanSizeList.Count; i++)
            {
                //double d = ;
                //string str = d == 2000 ? "MD 1.0×2.0" : "MD 1.0×3.0";
                string[] rowValue = new string[] { (i + 1).ToString(), data.FukkoubanSizeList[i] };
                DgvFukkouban.Rows.Add(rowValue);
            }

            //根太データ
            CmbNedaMaterial.Text = data.nedaData.NedaMaterial;
            CmbNedaType.Text = data.nedaData.NedaType;
            CmbNedaSize.Text = data.nedaData.NedaSize;
            DgvNedaPitch.Rows.Clear();
            for (int i = 0; i < data.NedaPitch.Count; i++)
            {
                double d = data.NedaPitch[i]; ;
                string[] rowValue = new string[] { (i + 1).ToString(), d.ToString() };
                DgvNedaPitch.Rows.Add(rowValue);
            }
            NmcNedaSLng.Value = (decimal)data.nedaData.exNedaStartLeng;
            NmcNedaELng.Value = (decimal)data.nedaData.exNedaEndLeng;
            //大引データ
            CmbOhbikiMaterial.Text = data.ohbikiData.OhbikiMaterial;
            CmbOhbikiType.Text = data.ohbikiData.OhbikiType;
            CmbOhbikiSize.Text = data.ohbikiData.OhbikiSize;
            NmcOhbikiSLng.Value = (decimal)data.ohbikiData.exOhbikiStartLeng;
            NmcOhbikiELng.Value = (decimal)data.ohbikiData.exOhbikiEndLeng;
            //Del H鋼以外の大引はすべて両側に
            //data.ohbikiData.OhbikiAttachWay = radioButton14.Checked ? eAttachWay.OneSide : eAttachWay.BothSide;
            RbtOhbikiBolt.Checked = data.ohbikiData.OhbikiAttachType == eJoinType.Bolt;
            RbtOhbikiWeld.Checked = data.ohbikiData.OhbikiAttachType == eJoinType.Welding;
            RbtOhbikiNormalB.Checked = data.ohbikiData.OhbikiBoltType == eBoltType.Normal;
            RbtOhbikiBHTB.Checked = data.ohbikiData.OhbikiBoltType == eBoltType.HTB;
            NmcOhbikiBoltCnt.Value = data.ohbikiData.OhbikiBoltCount;
            ChckSikiketaStart.Checked = data.IsFirstShikigeta;
            ChckSikiketaEnd.Checked = data.IsLastShikigeta;
            CmbShikiketaMaterial.Text = data.shikigetaData.ShikigetaMaterial;
            CmbShikiketaType.Text = data.shikigetaData.ShikigetaType;
            CmbShikiketaSize.Text = data.shikigetaData.ShikigetaSize;
            NmcShikiketaSLng.Value = (decimal)data.shikigetaData.exShikigetaStartLeng;
            NmcShikiketaELng.Value = (decimal)data.shikigetaData.exShikigetaEndLeng;
            RbtOhbikiSingle.Checked = data.ohbikiData.OhbikiDan == 1;
            RbtOhbikiDouble.Checked = data.ohbikiData.OhbikiDan == 2;
            RbtOhbikiTriple.Checked = data.ohbikiData.OhbikiDan == 3;
            //柱データ
            CmbKuiType.Text = data.pilePillerData.PillarType;
            CmbKuiSize.Text = data.pilePillerData.PillarSize;
            ChckExistPile.Checked = !data.HasPilePiller;
            CmbKuiMaterial.Text = data.pilePillerData.PillarMaterial;
            NmcKuiLng.Value = (decimal)data.pilePillerData.PillarLength;
            ChckKuiHasCut.Checked = data.pilePillerData.IsCut;
            //(decimal)data.pilePillerData.PileCutLength;
            NmcKuiWholeLng.Value = (decimal)data.pilePillerData.PillarWholeLength;
            NmcKuiHeadLng.Value = (decimal)data.pilePillerData.exPillarHeadLeng;
            NmcKuiNedaDiff.Value = (decimal)data.beginNedaDiff;
            DgvKuiKyoutyouPitch.Rows.Clear();
            for (int i = 0; i < data.KyoutyouPillarPitch.Count; i++)
            {
                double d = data.KyoutyouPillarPitch[i]; ;
                string[] rowValue = new string[] { (i + 1).ToString(), d.ToString() };
                DgvKuiKyoutyouPitch.Rows.Add(rowValue);
            }
            DgvKuiHukuinPitch.Rows.Clear();
            for (int i = 0; i < data.HukuinPillarPitch.Count; i++)
            {
                double d = data.HukuinPillarPitch[i]; ;
                string[] rowValue = new string[] { (i + 1).ToString(), d.ToString() };
                DgvKuiHukuinPitch.Rows.Add(rowValue);
            }

            //杭詳細データ
            //継足し杭
            ChckKuiHasEx.Checked = data.pilePillerData.ExtensionPile;
            NmcKuiExLng.Value = (decimal)data.pilePillerData.extensionPileData.Length;
            this.txtExPileJointType.Text = /*data.pilePillerData.extensionPileData.attachType == eAttachType.Bolt ? "ボルト" :*/ "溶接";
            CmbKuiExBoltSizeF.Text = data.pilePillerData.extensionPileData.FlangeBolt.BoltSize;
            CmbKuiExBoltSizeW.Text = data.pilePillerData.extensionPileData.WebBolt.BoltSize; ;

            //継手詳細
            NmcKuiJointCnt.Value = data.pilePillerData.pJointCount;
            DgvKuiJointPitch.Rows.Clear();
            for (int i = 0; i < data.pilePillerData.pJointPitch.Count; i++)
            {
                double d = data.pilePillerData.pJointPitch[i]; ;
                string[] rowValue = new string[] { (i + 1).ToString(), d.ToString() };
                DgvKuiJointPitch.Rows.Add(rowValue);
            }
            CmbKuiJointType.Text = data.pilePillerData.jointDetailData.JointType == eJoinType.Bolt ? "ボルト" : "溶接";
            CmbKuiJointBoltSizeF.Text = data.pilePillerData.jointDetailData.FlangeBolt.BoltSize;
            CmbKuiJointBoltSizeW.Text = data.pilePillerData.jointDetailData.WebBolt.BoltSize;
            CmbKuiJointBoltSizeF.Text = data.pilePillerData.jointDetailData.FlangeBolt.BoltSize;
            CmbKuiJointBoltSizeW.Text = data.pilePillerData.jointDetailData.WebBolt.BoltSize;
            textBox40.Text = ((decimal)data.pilePillerData.extensionPileData.Length).ToString();

            //トッププレート
            ChckKuiHasTopPL.Checked = data.pilePillerData.HasTopPlate;
            CmbKuiTopPLSize.Text= data.pilePillerData.topPlateData.PlateSize;
            if (string.IsNullOrEmpty(data.pilePillerData.topPlateData.PlateSize)&& CmbKuiTopPLSize.Items.Count > 0)
            {
                CmbKuiTopPLSize.SelectedIndex = 0;
            }
            //data.pilePillerData.topPlateData.PlateSize = $"{NmcKuiTopPLW.Value}x{NmcKuiTopPLD.Value}x{NmcKuiTopPLH.Value}";
            //if (!string.IsNullOrEmpty(data.pilePillerData.topPlateData.PlateSize))
            //{
            //    NmcKuiTopPLW.Value = (decimal)RevitUtil.ClsCommonUtils.ChangeStrToDbl(data.pilePillerData.topPlateData.PlateSize.Split('x')[0]);
            //    NmcKuiTopPLD.Value = (decimal)RevitUtil.ClsCommonUtils.ChangeStrToDbl(data.pilePillerData.topPlateData.PlateSize.Split('x')[1]);
            //    NmcKuiTopPLH.Value = (decimal)RevitUtil.ClsCommonUtils.ChangeStrToDbl(data.pilePillerData.topPlateData.PlateSize.Split('x')[2]);
            //}
        }

        /// <summary>
        /// OKボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (!CheckInPutValues())
            {
                this.DialogResult = DialogResult.None;
                return;
            }
            CntrlToData();
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// 入力値チェック
        /// </summary>
        /// <returns></returns>
        private bool CheckInPutValues(bool isPreview = false)
        {
            List<string> errMsg = new List<string>();
            try
            {
                //構台種別チェック
                if (string.IsNullOrEmpty(CmbKoudaiType.Text))
                {
                    errMsg.Add("構台種別を設定してください");
                }

                //構台名チェック
                if (string.IsNullOrEmpty(CmbKoudaiName.Text))
                {
                    errMsg.Add("構台名が空欄です 構台名を設定してください");
                }
                else if (GantryUtil.GetAllKoudaiName(doc).Contains(CmbKoudaiName.Text))
                {
                    errMsg.Add("構台名が重複しています 構台名を変更してください");
                }

                //基準レベル
                if (!GantryUtil.GetAllLevelName(doc).Contains(CmbBaseLevel.Text))
                {
                    errMsg.Add("基準レベルは図面内のレベルから選択してください");
                }
                //構台長さ
                if (this.NmcKyoutyouLng.Value <= 0)
                {
                    errMsg.Add("橋軸方向長さが0以下です");
                }
                if (this.NmcHukuinLng.Value <= 0)
                {
                    errMsg.Add("幅員方向長さが0以下です");
                }


                //部材サイズ
                //根太
                if (string.IsNullOrEmpty(CmbNedaSize.Text))
                {
                    errMsg.Add("根太のサイズを指定してください");
                }
                //大引
                if (string.IsNullOrEmpty(CmbOhbikiSize.Text))
                {
                    errMsg.Add("大引のサイズを指定してください");
                }
                //杭
                if (string.IsNullOrEmpty(CmbKuiSize.Text))
                {
                    errMsg.Add("杭のサイズを指定してください");
                }
                //敷桁
                if ((ChckSikiketaStart.Checked || ChckSikiketaEnd.Checked) && string.IsNullOrEmpty(CmbShikiketaSize.Text))
                {
                    errMsg.Add("敷桁のサイズを指定してください");
                }

                //覆工板
                if (string.IsNullOrEmpty(CmbFukkouMaterial.Text) && DgvFukkouban.Rows.Count > 0)
                {
                    errMsg.Add("覆工板の材質を指定してください");
                }
                if (!ChckNoFukkouban.Checked && DgvFukkouban.Rows.Count < 1)
                {
                    errMsg.Add("覆工板のピッチ指定がされていません");
                }
                foreach (DataGridViewRow row in DgvFukkouban.Rows)
                {
                    if (row.Cells[1].Value == null || string.IsNullOrEmpty(row.Cells[1].Value.ToString()))
                    {
                        errMsg.Add("覆工板のピッチ指定に空欄があります");
                        break;
                    }
                }

                //根太
                if (string.IsNullOrEmpty(CmbNedaMaterial.Text) && DgvNedaPitch.Rows.Count > 0)
                {
                    errMsg.Add("根太の材質を指定してください");
                }

                //大引
                if (string.IsNullOrEmpty(CmbOhbikiMaterial.Text) && DgvKuiKyoutyouPitch.Rows.Count > 0)
                {
                    errMsg.Add("大引の材質を指定してください");
                }

                //大引
                if (string.IsNullOrEmpty(CmbShikiketaMaterial.Text) && (ChckSikiketaStart.Checked || ChckSikiketaEnd.Checked))
                {
                    errMsg.Add("敷桁の材質を指定してください");
                }

                //杭
                if (!GantryUtil.ChkDoubleValueAsString(NmcKuiLng.Value.ToString(), true, true))
                {
                    errMsg.Add("杭長を指定してください");
                }
                if (!ChckExistPile.Checked && NmcKuiLng.Value == 0)
                {
                    errMsg.Add("杭長を指定してください");
                }
                if (ChckKuiHasTopPL.Checked && string.IsNullOrEmpty(CmbKuiTopPLSize.Text))
                {
                    errMsg.Add("杭のトッププレートサイズを指定してください");
                }
                if (ChckKuiHasEx.Checked && NmcKuiExLng.Value <= 0)
                {
                    errMsg.Add("継足し杭の長さを指定してください");
                }

                if (DgvKuiKyoutyouPitch.Rows.Count <= 0)
                {
                    errMsg.Add("橋軸方向の柱間隔を指定してください");
                }
                if (DgvKuiHukuinPitch.Rows.Count <= 0)
                {
                    errMsg.Add("幅員方向の柱間隔を指定してください");
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
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// テストデータ入力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            //構台データ
            CmbKoudaiType.Text = "構台";
            CmbKoudaiName.Text = "構台1";
            CmbBaseLevel.Text = "大引下端";
            NmcLevelOffset.Value = 0;
            NmcKyoutyouLng.Value = 29000;
            NmcHukuinLng.Value = 11000;

            //覆工板データ
            ChckNoFukkouban.Checked = false;
            CmbFukkouMaterial.Text = "SS400";
            DgvFukkouban.Rows.Clear();
            List<string[]> nPitch = new List<string[]> {
                new string[]{$"{1}", "MD1000X3000"},
                new string[]{$"{2}", "MD1000X2000"},
                new string[]{$"{3}", "MD1000X3000"},
                new string[]{$"{4}", "MD1000X3000" }
            };
            for (int i = 0; i < nPitch.Count; i++)
            {
                DgvFukkouban.Rows.Add(nPitch[i]);
            }

            //根太データ
            CmbNedaType.Text = Master.ClsHBeamCsv.TypeHiro;
            CmbNedaMaterial.Text = "SS400";
            CmbNedaSize.Text = "H250X250X9X14";
            DgvNedaPitch.Rows.Clear();
            nPitch = new List<string[]> {
                new string[]{$"1", "0"},
                new string[]{$"2", "3000"},
                new string[]{$"3", "2000"},
                new string[]{$"4", "3000" },
                new string[]{$"5", "3000" }
            };
            for (int i = 0; i < nPitch.Count; i++)
            {
                DgvNedaPitch.Rows.Add(nPitch[i]);
            }
            NmcNedaSLng.Value = 500;
            NmcNedaELng.Value = 300;
            //大引データ
            CmbOhbikiType.Text = Master.ClsHBeamCsv.TypeHiro;
            CmbOhbikiMaterial.Text = "SS400";
            CmbOhbikiSize.Text = "H250X250X9X14";
            NmcOhbikiSLng.Value = 200;
            NmcOhbikiELng.Value = 100;

            //敷桁
            CmbShikiketaType.Text = Master.ClsHBeamCsv.TypeHiro;
            CmbShikiketaSize.Text = "H250X250X9X14";
            ChckSikiketaStart.Checked = true;
            ChckSikiketaEnd.Checked = true;
            NmcShikiketaSLng.Value = 0;
            NmcShikiketaELng.Value = 0;

            //data.ohbikiData.OhbikiAttachWay = radioButton14.Checked ? eAttachWay.OneSide : eAttachWay.BothSide;
            //data.ohbikiData.OhbikiAttachType = radioButton12.Checked ? eAttachType.Bolt : eAttachType.Welding;
            //data.ohbikiData.OhbikiBoltType = radioButton10.Checked ? eBoltType.Normal : eBoltType.HTB;
            //data.ohbikiData.OhbikiBoltCount = (int)numericUpDown5.Value;

            //柱データ
            CmbKuiType.Text = Master.ClsHBeamCsv.TypeHiro;
            CmbKuiSize.Text = "H300X300X10X15";
            ChckExistPile.Checked = false;
            CmbKuiMaterial.Text = "SS400";
            NmcKuiWholeLng.Value = 15300;
            NmcKuiLng.Value = 15000;
            ChckKuiHasCut.Checked = true;

            NmcKuiHeadLng.Value = 0;
            NmcKuiNedaDiff.Value = 0;
            nPitch = new List<string[]> {
                new string[]{$"1", "0"},
                new string[]{$"2", "6000"},
                new string[]{$"3", "6000"},
                new string[]{$"4", "5000" },
                new string[]{$"5", "6000" },
                new string[]{$"6", "6000" }
            };
            DgvKuiKyoutyouPitch.Rows.Clear();
            for (int i = 0; i < nPitch.Count; i++)
            {
                DgvKuiKyoutyouPitch.Rows.Add(nPitch[i]);
            }
            nPitch = new List<string[]> {
                new string[]{$"1", "0"},
                new string[]{$"2", "5500"},
                new string[]{$"3", "5500"}
            };
            DgvKuiHukuinPitch.Rows.Clear();
            for (int i = 0; i < nPitch.Count; i++)
            {
                DgvKuiHukuinPitch.Rows.Add(nPitch[i]);
            }

            NmcKuiExLng.Value = 1012;
            CmbKuiTopPLSize.Text = "PL-12X348X348";
            //ChckKuiHasEx.Checked = true; //継ぎ足し杭の共有ファミリが改善されるまで
            ChckKuiHasEx.Checked = false;

            DgvKuiJointPitch.Rows.Clear();
            DgvKuiJointPitch.Rows.Add("1", 8000);
            CmbKuiJointType.Text = "ボルト";
            this.txtExPileJointType.Text = "溶接";
            NmcKuiJointCnt.Value = 1;

            //NmcKuiTopPLH.Value = 12;
            //NmcKuiTopPLW.Value = 348;
            //NmcKuiTopPLD.Value = 348;
            //radioButton5.Checked = true;
            RbtNedaKobetsu.Checked = true;
            RbtKuiKyoutyouKobetsu.Checked = true;
            RbtKuiHukuinKobetsu.Checked = true;
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            //if (this.ChckKuiHasEx.Checked)
            //{
            //    this.pictureBox1.Visible = true;
            //    this.pictureBox2.Visible = false;
            //    return;
            //}
            //this.pictureBox1.Visible = false;
            //this.pictureBox2.Visible = true;
            ChangePictureForPileTips();
            CalcPileLength();
            if (!this.ChckKuiHasEx.Checked)
            {
                NmcKuiExLng.Value = 0;
            }
            InitExPileControl();
        }

        /// <summary>
        /// 杭の凡例図切り替え
        /// </summary>
        private void ChangePictureForPileTips()
        {
            Bitmap img = global::YMS_gantry.Properties.Resources.杭凡例_覆天_切有;
            if (this.RbtKenchiku.Checked)
            {
                if (this.RbtFukkouTop.Checked && !this.ChckKuiHasCut.Checked)
                {
                    img = global::YMS_gantry.Properties.Resources.杭凡例_覆天_切無;
                }
                else if (this.RbtOhbikiBtm.Checked)
                {
                    if (this.ChckKuiHasCut.Checked && this.ChckKuiHasEx.Checked)
                    {
                        img = global::YMS_gantry.Properties.Resources.杭凡例_大下_切有_継有;
                    }
                    //else if (this.ChckKuiHasCut.Checked && !this.ChckKuiHasEx.Checked)
                    //{
                    //    img = global::YMS_gantry.Properties.Resources.杭凡例_大下_切有_継無;
                    //}
                    //else if(!this.ChckKuiHasCut.Checked && !this.ChckKuiHasEx.Checked)
                    //{
                    //    img = global::YMS_gantry.Properties.Resources.杭凡例_大下_切無_継無;
                    //}
                    else if(!ChckKuiHasEx.Checked)
                    {
                        if(NmcLevelOffset.Value>=0)
                        {
                            img = global::YMS_gantry.Properties.Resources.杭凡例_大下_切無_継無;
                        }
                        else
                        {
                            img = global::YMS_gantry.Properties.Resources.杭凡例_大下_切有_継無;
                        }
                    }
                }
            }
            else
            {
                if (this.ChckKuiHasCut.Checked)
                {
                    img = global::YMS_gantry.Properties.Resources.杭凡例_土_覆天_切有;
                }
                else
                {
                    img = global::YMS_gantry.Properties.Resources.杭凡例_土_覆天_切無;
                }
            }
            this.pictureBox1.Visible = true;
            this.pictureBox1.Image = img;
        }

        /// <summary>
        /// 杭の凡例図切り替え
        /// </summary>
        private void CalcPileLength()
        {
            Bitmap img = global::YMS_gantry.Properties.Resources.杭凡例_覆天_切有;
            double fukkouThick = DefineUtil.FukkouBAN_THICK;
            double nedaHeight = (this.CmbNedaSize.Text != "") ? GantryUtil.GetKouzaiSize(this.CmbNedaSize.Text).Height : 0;
            double ohbikiHeight = (this.CmbOhbikiSize.Text != "") ? GantryUtil.GetKouzaiSize(this.CmbOhbikiSize.Text).Height : 0;
            double topPLthick = (this.CmbKuiTopPLSize.Text != "") ? GantryUtil.GetKouzaiSize(this.CmbKuiTopPLSize.Text).Thick : 0;
            double offset = (double)this.NmcLevelOffset.Value;

            double retLeng = (double)this.NmcKuiLng.Value;

            if (this.RbtFukkouTop.Checked)
            {
                if (this.ChckKuiHasCut.Checked)
                {                    
                    if (this.NmcLevelOffset.Value < 0)
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
                if (this.ChckKuiHasCut.Checked && this.ChckKuiHasEx.Checked)
                {
                    retLeng = retLeng - Math.Abs(offset);
                }
                else if (this.ChckKuiHasCut.Checked && !this.ChckKuiHasEx.Checked)
                {
                    if (this.NmcLevelOffset.Value < 0)
                    {
                        retLeng = retLeng + Math.Abs(offset);
                    }
                }
            }

            this.NmcKuiWholeLng.Value= (decimal)retLeng;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            decimal currentValue = NmcKuiJointCnt.Value;

            // 以前の値が存在する場合、比較を行う
            if (NmcKuiJointCnt.Tag != null)
            {
                decimal previousValue = decimal.Parse(NmcKuiJointCnt.Tag.ToString());

                if (currentValue > previousValue)
                {
                    int dif = (int)(currentValue - previousValue);
                    // 増加した場合の処理
                    for (int i = 0; i < dif; i++)
                    {
                        object[] aR = new object[] { $"{DgvKuiJointPitch.Rows.Count + 1}", 0 };
                        DgvKuiJointPitch.Rows.Add(aR);
                    }
                }
                else if (currentValue < previousValue && DgvKuiJointPitch.Rows.Count > 0)
                {
                    // 減少した場合の処理
                    int dif = (int)(previousValue - currentValue);
                    if (dif > 0)
                    {
                        for (int i = 0; i < dif; i++)
                        {
                            DgvKuiJointPitch.Rows.RemoveAt(DgvKuiJointPitch.Rows.Count - 1);
                        }
                    }
                    else
                    {
                        DgvKuiJointPitch.Rows.Clear();
                    }
                }
                else
                {
                    // 値が変化していない場合の処理
                }
            }

            // 現在の値を以前の値として保存
            NmcKuiJointCnt.Tag = currentValue;
        }

        private void dataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                row.Cells[0].Value = $"{dgv.Rows.IndexOf(row) + 1}";
            }
        }


        /// <summary>
        /// 根太追加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            //個別追加
            if (RbtNedaKobetsu.Checked)
            {
                var row = new DataGridViewRow();
                row.CreateCells(this.DgvNedaPitch);

                if (this.DgvNedaPitch.SelectedRows.Count > 0 && this.DgvNedaPitch.SelectedRows[0].Index != this.DgvNedaPitch.Rows.Count - 1)
                {
                    row.Cells[0].Value = this.DgvNedaPitch.SelectedRows[0].Index + 2;
                    row.Cells[1].Value = "0";
                    for (int i = this.DgvNedaPitch.SelectedRows[0].Index + 1; i < this.DgvNedaPitch.Rows.Count; i++)
                    {
                        this.DgvNedaPitch.Rows[i].Cells[0].Value = i + 2;
                    }

                    this.DgvNedaPitch.Rows.Insert(this.DgvNedaPitch.SelectedRows[0].Index + 1, row);
                }
                else
                {
                    DgvNedaPitch.Rows.Add(new string[] { $"{DgvNedaPitch.Rows.Count + 1}", "0" });
                }
                return;
            }
            //覆工板直下配置
            else if (RbtNedaUFukkou.Checked)
            {
                if (DgvNedaPitch.Rows.Count > 0 && MessageUtil.YesNo("根太サイズをすべて更新します。\r\r続行しますか?", "根太追加", this) != DialogResult.Yes)
                {
                    return;
                }
                DgvNedaPitch.Rows.Clear();

                if (DgvFukkouban.Rows.Count < 1)
                {
                    MessageUtil.Warning("覆工板が設定されていません", "根太追加", this);
                    return;
                }
                double prePitch = 0;
                foreach (DataGridViewRow row in DgvFukkouban.Rows)
                {
                    int rowC = DgvNedaPitch.Rows.Count + 1;
                    double w = RevitUtil.ClsCommonUtils.ChangeStrToDbl(row.Cells[1].Value.ToString().Substring(row.Cells[1].Value.ToString().LastIndexOf("X") + 1));
                    int pitch = (int)(w != 0 ? w : (row.Cells[1].Value.ToString().EndsWith("2000")) ? 2000 : 3000);
                    if (ChckNedaHasAssist.Checked) { pitch = pitch / 2; }
                    if (row.Index == 0)
                    {
                        DgvNedaPitch.Rows.Add(new string[] { $"{1}", $"{0}" });
                        rowC += 1;
                    }
                    DgvNedaPitch.Rows.Add(new string[] { $"{rowC}", $"{pitch}" });
                    //補強桁あり
                    if (ChckNedaHasAssist.Checked)
                    {
                        rowC = DgvNedaPitch.Rows.Count + 1;
                        DgvNedaPitch.Rows.Add(new string[] { $"{rowC}", $"{pitch}" });
                    }
                }
            }
            //均一ピッチ
            else
            {

                if (NmcNedaPitch.Value < 1)
                {
                    MessageUtil.Warning("根太ピッチが0以下です", "根太追加", this);
                    return;
                }

                if (DgvNedaPitch.Rows.Count > 0 && MessageUtil.YesNo("根太サイズをすべて更新します。\r\n続行しますか?", "根太追加", this) != DialogResult.Yes)
                {
                    return;
                }

                DgvNedaPitch.Rows.Clear();
                int hukuin = (int)(Math.Ceiling(NmcHukuinLng.Value / 1000)) * 1000;
                double pitch = (double)NmcNedaPitch.Value;
                int nedaC = (int)Math.Floor(hukuin / pitch);
                DgvNedaPitch.Rows.Add(new string[] { "1", "0" });
                for (int i = 0; i < nedaC; i++)
                {
                    DgvNedaPitch.Rows.Add(new string[] { $"{DgvNedaPitch.Rows.Count + 1}", $"{pitch}" });
                }

            }
        }

        private void numericUpDown22_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown num = sender as NumericUpDown;
            textBox40.Text = num.Value.ToString();
            CalcPileLength();
        }

        /// <summary>
        /// 覆工板一枚追加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            if (CmbFukkouSize.Text == "")
            {
                MessageUtil.Warning("覆工板サイズが選択されていません", "覆工板追加", this);
                return;
            }

            if (RbtFukkouKobetsu.Checked)
            {
                var row = new DataGridViewRow();
                row.CreateCells(this.DgvFukkouban);

                if (this.DgvFukkouban.SelectedRows.Count > 0 && this.DgvFukkouban.SelectedRows[0].Index != this.DgvFukkouban.Rows.Count - 1)
                {
                    row.Cells[0].Value = this.DgvFukkouban.SelectedRows[0].Index + 2;
                    row.Cells[1].Value = CmbFukkouSize.Text;
                    for (int i = this.DgvFukkouban.SelectedRows[0].Index + 1; i < this.DgvFukkouban.Rows.Count; i++)
                    {
                        this.DgvFukkouban.Rows[i].Cells[0].Value = i + 2;
                    }

                    this.DgvFukkouban.Rows.Insert(this.DgvFukkouban.SelectedRows[0].Index + 1, row);
                }
                else
                {
                    row.Cells[0].Value = this.DgvFukkouban.Rows.Count + 1;
                    row.Cells[1].Value = CmbFukkouSize.Text;
                    this.DgvFukkouban.Rows.Add(row);
                }

            }
            else
            {

                if (NmcHukuinLng.Value < 1)
                {
                    MessageUtil.Warning("幅員方向の長さが0以下です", "覆工板追加", this);
                    return;
                }

                if (DgvFukkouban.Rows.Count > 0)
                {
                    if (MessageUtil.YesNo("追加した覆工板サイズをすべて更新します。\r\n続行しますか?", "覆工板追加", this) != DialogResult.Yes)
                    {
                        return;
                    }
                }
                DgvFukkouban.Rows.Clear();

                double w = RevitUtil.ClsCommonUtils.ChangeStrToDbl(CmbFukkouSize.Text.Substring(CmbFukkouSize.Text.LastIndexOf("X") + 1));
                int hukouban = (int)(w != 0 ? w : (CmbFukkouSize.Text.EndsWith("2000")) ? 2000 : 3000);
                int hukuin = (int)(Math.Ceiling(NmcHukuinLng.Value / 1000)) * 1000;
                bool isJust = (hukuin % hukouban) == 0;
                int FukkoubanC = (int)Math.Ceiling((double)(hukuin / hukouban));

                for (int i = 0; i < FukkoubanC; i++)
                {
                    var row = new DataGridViewRow();
                    row.CreateCells(this.DgvFukkouban);
                    row.Cells[0].Value = this.DgvFukkouban.Rows.Count + 1;
                    row.Cells[1].Value = CmbFukkouSize.Text;
                    this.DgvFukkouban.Rows.Add(row);
                }

                if (!isJust)
                {
                    MessageUtil.Warning("指定の覆工板サイズで幅員方向が割り切れませんでした。\r\n調整が必要です。", "覆工板追加", this);
                }
            }
        }

        /// <summary>
        /// プレビュー更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PrevireUpdata()
        {
            if (!CheckInPutValues())
            {
                return;
            }
            CntrlToData();
            if (frmPreView == null)
            {
                frmPreView = new FrmPreview(data);
            }
            else
            {
                frmPreView.UpdatePreview(data);
            }

            //モーダレスにして結果だけ取得する
            if (!frmPreView.Visible)
            {
                frmPreView.FormClosed += new FormClosedEventHandler(Preview_Closed);
            }
            else
            {
                frmPreView.Visible = false;
            }
            frmPreView.Show(this);
        }

        /// <summary>
        /// トッププレート更新処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmbKuiTopPLSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            //string str = CmbKuiTopPLSize.Text;
            //bool bE = true;
            //if (str != Master.ClsTopPlateCsv.FreeSize)
            //{
            //    bE = false;
            //}
            //NmcKuiTopPLD.Enabled = bE;
            //NmcKuiTopPLW.Enabled = bE;
            //NmcKuiTopPLH.Enabled = bE;
            CalcPileLength();
        }

        private void Nmc_Leave(object sender, EventArgs e)
        {
            NumericUpDown nmc = (NumericUpDown)sender;
            if (nmc.Text == "")
            {
                nmc.Value = 0;
                nmc.Text = "0";
            }
            base.OnLostFocus(e);
        }

        private void CmbKoudaiName_TextChanged(object sender, EventArgs e)
        {
            string kodaiName = CmbKoudaiName.Text;

            ModelData md = ModelData.GetModelData(doc);

            KoudaiData kd = md.GetKoudaiData(kodaiName);
            var isControlEnable = true;

            if (kd.AllKoudaiFlatData != null)
            {
                data = kd.AllKoudaiFlatData;
                // 既に配置済みの構台を選択した場合は該当コントロールを無効化
                isControlEnable = false;
                DataToCntrl();
            }

            this.CmbKoudaiType.Enabled = isControlEnable;
            this.GbxHaichiType.Enabled = isControlEnable;
            this.GbxHaichiBase.Enabled = isControlEnable;
            this.PnlHaichiSize.Enabled = isControlEnable;
            this.GbxElementSize.Enabled = isControlEnable;
            this.GbxTsukidashi.Enabled = isControlEnable;
            this.TabElement.Enabled = isControlEnable;
            this.BtnCalcFileDataLoad.Enabled = isControlEnable;
            this.BtnPreview.Enabled = isControlEnable;
            this.BtnOK.Enabled = isControlEnable;
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            string kodaiName = CmbKoudaiName.Text;

            // ダイアログに表示するメッセージ
            string message = "構台名「" + kodaiName + "」に該当する部材を全て削除します。よろしいですか？";

            // ダイアログを表示し、ユーザーの選択を取得
            DialogResult result = MessageBox.Show(message, "", MessageBoxButtons.YesNo);

            if (result == DialogResult.No)
            {
                return;
            }


            //ドキュメント上から指定された構台のインスタンスを削除
            MaterialSuper[] unionMaterials = MaterialSuper.Collect(doc);
            List<ElementId> lstId = (from data in unionMaterials where data.m_KodaiName == kodaiName select data.m_ElementId).ToList();

            ModelData md = ModelData.GetModelData(doc);
            md.DeleteKoudaiData(kodaiName);
            using (var transaction = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                transaction.Start();
                if (lstId != null)
                {
                    foreach (ElementId id in lstId)
                    {
                        ClsRevitUtil.Delete(doc, id);
                    }
                }
                ModelData.SetModelData(doc, md);
                transaction.Commit();
            }

            //全構台名
            //mod 24/03/13 削除後にパラメータは残す
            //List<string> lstStr = GantryUtil.GetAllKoudaiName(doc);
            //ControlUtils.SetComboBoxItems(CmbKoudaiName, lstStr);
            CmbKoudaiName.Items.Remove(kodaiName);
            CmbKoudaiName.SelectedItem = "";
            this.CmbKoudaiType.Enabled = true;
            this.GbxHaichiType.Enabled = true;
            this.GbxHaichiBase.Enabled = true;
            this.PnlHaichiSize.Enabled = true;
            this.GbxElementSize.Enabled = true;
            this.GbxTsukidashi.Enabled = true;
            this.TabElement.Enabled = true;
            this.BtnCalcFileDataLoad.Enabled = true;
            this.BtnPreview.Enabled = true;
            this.BtnOK.Enabled = true;
        }

        private void BtnCalcFileDataLoad_Click(object sender, EventArgs e)
        {
            var xmlFilePath = string.Empty;

            using (var openFileDlg = new OpenFileDialog())
            {

                openFileDlg.Title = "計算書取込ファイル選択";
                openFileDlg.Filter = "計算書データ|*.xml";

                if (openFileDlg.ShowDialog() == DialogResult.OK)
                {
                    xmlFilePath = openFileDlg.FileName;
                }
            }

            if (string.IsNullOrWhiteSpace(xmlFilePath)) return;

            // 計算書XMLファイルから計算書モデルデータにデシリアライズ
            var calcFileData = new CalcFileData();
            if (!CalcFileData.ReadXML(xmlFilePath, ref calcFileData))
            {
                MessageUtil.Error("計算書データの取込に失敗しました。", "構台一括配置");
                logger.Error($"計算書データの取込に失敗しました。{xmlFilePath}");
                return;
            }

            // 画面コントロールを画面データに格納
            CntrlToData();

            // プロジェクト情報の設計指針と計算書取込データの配置種別が一致している場合にのみ計算書データの取込をおこなう
            if ((data.KoujiType == eKoujiType.Kenchiku && calcFileData.Base.Type == "建築") || ((data.KoujiType == eKoujiType.Doboku && calcFileData.Base.Type == "土木")))
            {
                // 計算書取込データを画面データに読込
                data.ImportCalcFileData(calcFileData);
                // 画面データを画面コントロールに反映
                DataToCntrl();
                // 計算書取込データを画面に保持
                this.calcFileData = calcFileData;

                CalcPileLength();

                MessageUtil.Information("計算書の取込が完了しました。", "構台一括配置");
            }
            else
            {
                MessageUtil.Warning("画面で指定された配置種別と計算書取込データ内の配置種別が異なるため取込できません。", "構台一括配置");
            }
        }

        private void CmbKuiJointType_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitPileControl();
        }

        private void CmbKuiExJointType_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitExPileControl();
        }

        private void CmbKuiSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            string size = CmbKuiSize.Text;
            bool bEx = Master.ClsHBeamCsv.IsExPileFamily(size);
            if (bEx)
                ChckKuiHasEx.Enabled = true;
            else
            {
                ChckKuiHasEx.Checked = false;
                ChckKuiHasEx.Enabled = false;
            }

            InitPileControl();
            InitExPileControl();
        }

        private void RbtKenchiku_CheckedChanged(object sender, EventArgs e)
        {
            if (this.RbtKenchiku.Checked && this.defaultKoujiType == eKoujiType.Doboku)
            {
                if (MessageUtil.YesNo("設計指針と異なる値を指定しようとしています。変更しますか？", "") == DialogResult.No)
                {
                    this.RbtDoboku.Checked = true;
                }
            }
            else if (this.RbtDoboku.Checked && this.defaultKoujiType == eKoujiType.Kenchiku)
            {
                if (MessageUtil.YesNo("設計指針と異なる値を指定しようとしています。変更しますか？", "") == DialogResult.No)
                {
                    this.RbtKenchiku.Checked = true;
                }
            }

            // コンボボックス初期化
            InitComboBox();

            ChangePictureForPileTips();
            CalcPileLength();

            var nedaShugetaLabel = DefineUtil.NEDA;
            var ohbikiKetaukeLabel = DefineUtil.OHBIKI;
            if (!this.RbtKenchiku.Checked)
            {
                this.RbtFukkouTop.Checked = true;
                this.RbtOhbikiBtm.Enabled = false;
                this.ChckKuiHasEx.Checked = false;
                this.ChckKuiHasEx.Enabled = false;
                this.NmcKuiExLng.Value = 0;
                this.NmcKuiExLng.Enabled = false;
                nedaShugetaLabel = DefineUtil.SHUGETA;
                ohbikiKetaukeLabel = DefineUtil.KETAUKE;
            }
            else
            {
                this.RbtOhbikiBtm.Enabled = true;
                this.ChckKuiHasEx.Enabled = true;
                this.NmcKuiExLng.Enabled = true;
            }

            this.LblNedaShugentSize.Text = nedaShugetaLabel;
            this.LblNedaShugetaTsukidashi.Text = nedaShugetaLabel;
            this.TabNedaShugeta.Text = nedaShugetaLabel;
            this.LblOhbikiKetaukeSize.Text = ohbikiKetaukeLabel;
            this.LblOhbikiKetaukeTsukidashi.Text = ohbikiKetaukeLabel;
            this.TabOhbikiShugeta.Text = ohbikiKetaukeLabel;
        }

        private void RbtOhbikiBtm_CheckedChanged(object sender, EventArgs e)
        {
            ChangePictureForPileTips();
            CalcPileLength();

            if (!this.RbtOhbikiBtm.Checked)
            {
                this.ChckKuiHasEx.Checked = false;
            }
            this.ChckKuiHasEx.Enabled = this.RbtOhbikiBtm.Checked;
        }

        private void ChckKuiHasCut_CheckedChanged(object sender, EventArgs e)
        {
            bool isCut = this.ChckKuiHasCut.Checked;
            this.ChckKuiHasEx.Checked = isCut;
            this.ChckKuiHasEx.Enabled = isCut;
            this.NmcKuiExLng.Enabled = isCut;
            if(RbtFukkouTop.Checked)
            {
                this.ChckKuiHasEx.Checked =false;
                this.ChckKuiHasEx.Enabled =false;
                this.NmcKuiExLng.Enabled = false;
            }
           
            if(!isCut)
            {
                this.NmcKuiExLng.Value = 0;
            }
            ChangePictureForPileTips();
            CalcPileLength();
        }

        private void CmbNedaSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalcPileLength();
        }

        private void CmbOhbikiSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalcPileLength();
        }

        private void NmcKuiLng_ValueChanged(object sender, EventArgs e)
        {
            CalcPileLength();
        }

        private void NmcLevelOffset_ValueChanged(object sender, EventArgs e)
        {
            CalcPileLength();
        }

        private void CmbFukkouMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFukkoubanSize();
        }
        private void UpdateFukkoubanSize()
        {
            //材質によってサイズを更新
            List<string> sizeList = new List<string>();
            if (this.CmbFukkouMaterial.Text == "SS400")
            {
                sizeList = Master.ClsFukkoubanCsv.GetSizeList(Master.ClsFukkoubanCsv.TypeNormal);
            }
            else if (this.CmbFukkouMaterial.Text == "SM490")
            {
                sizeList = Master.ClsFukkoubanCsv.GetSizeList(Master.ClsFukkoubanCsv.TypeM);
            }
            else
            {
                sizeList = Master.ClsFukkoubanCsv.GetSizeList(Master.ClsFukkoubanCsv.TypeNormal);
                sizeList.AddRange(Master.ClsFukkoubanCsv.GetSizeList(Master.ClsFukkoubanCsv.TypeM));
            }
            ControlUtils.SetComboBoxItems(this.CmbFukkouSize, sizeList);
        }

        private void CmbKoudaiName_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void CmbKuiType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ChckExistPile_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void CmbKuiMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void NmcKuiWholeLng_ValueChanged(object sender, EventArgs e)
        {

        }

        private void ChckKuiHasTopPL_CheckedChanged(object sender, EventArgs e)
        {

        }
    }

    /// <summary>
    /// フラット構台一括配置画面データ
    /// </summary>
    [Serializable]
    [System.Xml.Serialization.XmlRoot("allKoudaiFlatFrmData")]
    public class AllKoudaiFlatFrmData
    {
#region 構台設定


        /// <summary>
        /// 構台種類
        /// </summary>
        [System.Xml.Serialization.XmlElement("KoudaiType")]
        public string KoudaiType { get; set; }

        /// <summary>
        /// 構台名
        /// </summary>
        [System.Xml.Serialization.XmlElement("KoudaiName")]
        public string KoudaiName { get; set; }

        /// <summary>
        /// 工事種別
        /// </summary>
        [System.Xml.Serialization.XmlElement("KoujiType")]
        public eKoujiType KoujiType { get; set; }

        /// <summary>
        /// 基準レベル
        /// </summary>
        [System.Xml.Serialization.XmlElement("BaseLevel")]
        public eBaseLevel BaseLevel { get; set; }

        /// <summary>
        /// 参照する図面のレベル
        /// </summary>
        [System.Xml.Serialization.XmlElement("SelectedLevel")]
        public string SelectedLevel { get; set; }

        /// <summary>
        /// レベルからのオフセット
        /// </summary>
        [System.Xml.Serialization.XmlElement("LevelOffset")]
        public double LevelOffset { get; set; }

        /// <summary>
        /// 橋長方向長さ
        /// </summary>
        [System.Xml.Serialization.XmlElement("KyoutyouLength")]
        public double KyoutyouLength { get; set; }

        /// <summary>
        /// 幅員方向長さ
        /// </summary>
        [System.Xml.Serialization.XmlElement("HukuinLength")]
        public double HukuinLength { get; set; }

        /// <summary>
        /// 橋長ベクトル
        /// </summary>
        [System.Xml.Serialization.XmlElement("KyoutyouVec")]
        public List<double> KyoutyouVecMem { get; set; }

        public (XYZ Origin, XYZ BasisH, XYZ BasisK, XYZ BasisZ) GetCoordinate(Document doc)
        {
            //var a = MaterialSuper.Collect(doc);
            var baseInstance = MaterialSuper.Collect(doc)
                .Select(x => x as KoudaiReference)
                .Where(x => x != null)
                .FirstOrDefault(x => x.m_KodaiName == KoudaiName);
            var familyInstance = doc.GetElement(baseInstance.m_ElementId) as FamilyInstance;
            var transform = familyInstance.GetTransform();
            var origin = baseInstance.Position;
            return (origin, transform.BasisX, transform.BasisY, transform.BasisZ);
        }

        /// <summary>
        /// 根太(h=0)と大引(k=0)の交点を原点とした座標系
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public (XYZ Origin, XYZ BasisH, XYZ BasisK, XYZ BasisZ) GetBasis(Document doc, bool isSlope = false)
        {
            var coordinate = GetCoordinate(doc);
            var hukuinPitches = isSlope?NedaPitch:HukuinPillarPitch;
            var kyochoPitches = KyoutyouPillarPitch;

            //var origin = coordinate.Origin - RevitUtil.ClsRevitUtil.CovertToAPI(hukuinPitches.FirstOrDefault()) * coordinate.BasisH + RevitUtil.ClsRevitUtil.CovertToAPI(kyochoPitches.FirstOrDefault()) * coordinate.BasisK;
            var origin = coordinate.Origin + RevitUtil.ClsRevitUtil.CovertToAPI(hukuinPitches.FirstOrDefault()) * coordinate.BasisH + RevitUtil.ClsRevitUtil.CovertToAPI(kyochoPitches.FirstOrDefault()) * coordinate.BasisK;

            return (origin, coordinate.BasisH, coordinate.BasisK, coordinate.BasisZ);
        }

        public XYZ GetKyoutyouVec(Document doc) => GetCoordinate(doc).BasisK;

        /// <summary>
        /// 柱の幅員座標列を取得 (単位は Feet)
        /// </summary>
        /// <returns></returns>
        public double[] PillarHSetFt()
        {
            var diff = beginNedaDiff.ToFt();
            return Enumerable
            .Range(0, this.HukuinPillarPitch.Count())
            .Select(x => this.HukuinPillarPitch.Skip(1).Take(x).Sum())
            .Select(x => ClsRevitUtil.CovertToAPI(x) + diff)
            .ToArray();
        }

        /// <summary>
        /// 柱の橋長座標列を取得 (単位は Feet)
        /// </summary>
        /// <returns></returns>
        public double[] PillarKSetFt() => Enumerable
            .Range(0, this.KyoutyouPillarPitch.Count())
            .Select(x => this.KyoutyouPillarPitch.Skip(1).Take(x).Sum())
            .Select(x => ClsRevitUtil.CovertToAPI(x))
            .ToArray();

        /// <summary>
        /// 幅員ベクトル
        /// </summary>
        [System.Xml.Serialization.XmlElement("HukuinVec")]
        public List<double> HukuinVecMem { get; set; }
        public XYZ GetHukuinVec(Document doc) => GetCoordinate(doc).BasisH;

        /// <summary>
        /// 鉛直方向の基底
        /// </summary>
        /// <returns></returns>
        public XYZ GetZVec(Document doc) => GetCoordinate(doc).BasisZ;

#endregion
#region 覆工板設定
        /// <summary>
        /// 覆工板有無
        /// </summary>
        [System.Xml.Serialization.XmlElement("HasFukkouban")]
        public bool HasFukkouban { get; set; }

        /// <summary>
        /// 配置間隔
        /// </summary>
        [System.Xml.Serialization.XmlElement("FukkoubanPitch")]
        public List<double> FukkoubanPitch { get; set; }

        /// <summary>
        /// 配置間隔
        /// </summary>
        [System.Xml.Serialization.XmlElement("FukkoubanSizeList")]
        public List<string> FukkoubanSizeList { get; set; }

        /// <summary>
        /// 覆工板データ
        /// </summary>
        [System.Xml.Serialization.XmlElement("FukkoubanData")]
        public FukkoubanData FukkoubanData { get; set; }
#endregion
#region 根太設定

        /// <summary>
        /// 根太ピッチ
        /// </summary>
        [System.Xml.Serialization.XmlElement("NedaPitch")]
        public List<double> NedaPitch { get; set; }

        /// <summary>
        /// 根太データ
        /// </summary>
        [System.Xml.Serialization.XmlElement("nedaData")]
        public NedaData nedaData { get; set; }
#endregion
#region　大引設定
        /// <summary>
        /// 大引データ
        /// </summary>
        [System.Xml.Serialization.XmlElement("ohbikiData")]
        public OhbikiData ohbikiData { get; set; }
#endregion
#region 敷桁設定
        /// <summary>
        /// 始点側大引が敷桁か
        /// </summary>
        [System.Xml.Serialization.XmlElement("IsFirstShikigeta")]
        public bool IsFirstShikigeta { get; set; }
        /// <summary>
        /// 始点側大引が敷桁か
        /// </summary>
        [System.Xml.Serialization.XmlElement("IsLastShikigeta")]
        public bool IsLastShikigeta { get; set; }
        /// <summary>
        /// 敷桁データ
        /// </summary>
        [System.Xml.Serialization.XmlElement("shikigetaData")]
        public ShikigetaData shikigetaData { get; set; }

#endregion
#region 柱・杭設定
        /// <summary>
        /// 柱有無
        /// </summary>
        [System.Xml.Serialization.XmlElement("HasPilePiller")]
        public bool HasPilePiller { get; set; }
        /// <summary>
        /// 柱橋長ピッチ
        /// </summary>
        [System.Xml.Serialization.XmlElement("KyoutyouPillarPitch")]
        public List<double> KyoutyouPillarPitch { get; set; }
        /// <summary>
        /// 柱幅員ピッチ
        /// </summary>
        [System.Xml.Serialization.XmlElement("HukuinPillarPitch")]
        public List<double> HukuinPillarPitch { get; set; }
        /// <summary>
        /// 基点側主桁とのズレ
        /// </summary>
        [System.Xml.Serialization.XmlElement("beginNedaDiff")]
        public double beginNedaDiff { get; set; }
        /// <summary>
        /// 柱データ
        /// </summary>
        [System.Xml.Serialization.XmlElement("pilePillerData")]
        public PilePillerData pilePillerData { get; set; }
#endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private AllKoudaiFlatFrmData()
        {
        }

        public static AllKoudaiFlatFrmData NewInstance()
        {
            var instance = new AllKoudaiFlatFrmData();
            instance.InitializeVariables();
            return instance;
        }

        private void InitializeVariables()
        {
            KoudaiType = "";
            KoudaiName = "";
            ClearParams();
        }

        private void ClearParams()
        {
            KoujiType = eKoujiType.Kenchiku;
            BaseLevel = eBaseLevel.OhbikiBtm;
            SelectedLevel = "";
            LevelOffset = 0.0;
            KyoutyouLength = 0.0;
            HukuinLength = 0.0;

            HasFukkouban = true;
            FukkoubanData = new FukkoubanData();
            FukkoubanPitch = new List<double>();
            FukkoubanSizeList = new List<string>();

            nedaData = new NedaData();
            NedaPitch = new List<double>();

            ohbikiData = new OhbikiData();
            shikigetaData = new ShikigetaData();
            HasPilePiller = true;
            pilePillerData = new PilePillerData();
            KyoutyouPillarPitch = new List<double>();
            HukuinPillarPitch = new List<double>();
            beginNedaDiff = 0;
        }

        public double GetKoudaHeight()
        {
            double retVal = 0;
            //覆工板高さ
            retVal += DefineUtil.FukkouBAN_THICK;

            //根太サイズ
            retVal += GantryUtil.GetKouzaiSize(nedaData.NedaSize).Height;

            //大引サイズ
            retVal += GantryUtil.GetKouzaiSize(ohbikiData.OhbikiSize).Height;

            //柱長さ
            retVal += pilePillerData.PillarLength;

            return retVal;
        }

        /// <summary>
        /// 画面データに計算書データを展開する
        /// </summary>
        /// <param name="calcFileData">計算書取込データ</param>
        public void ImportCalcFileData(CalcFileData calcFileData)
        {
            this.KoujiType = calcFileData.Base.Type == "建築" ? eKoujiType.Kenchiku : calcFileData.Base.Type == "土木" ? eKoujiType.Doboku : eKoujiType.None;

            // 構台情報を一部クリア
            ClearParams();

            // 基本情報 構台種別
            this.KoujiType = calcFileData.Base.Type == "建築" ? eKoujiType.Kenchiku : calcFileData.Base.Type == "土木" ? eKoujiType.Doboku : eKoujiType.Kenchiku;

            // 大引の突き出し量(LLまたはLR)が0より大きい場合は「順引」と判定
            var isJyunbiki = ((calcFileData.CatchBeam.LL ?? 0) > 0 || (calcFileData.CatchBeam.LR ?? 0) > 0);
            // 根太の突き出し量(LLまたはLR)が0より大きい場合は「逆引」と判定
            var isGyakubiki = ((calcFileData.Girder.LL ?? 0) > 0 || (calcFileData.Girder.LR ?? 0) > 0);

            if (isJyunbiki)
            {
                // 順引の場合は幅員の長さとして設定
                this.HukuinLength = calcFileData.Base.Bw ?? 0;
            }
            else if (isGyakubiki)
            {
                // 逆引の場合は橋軸の長さとして設定
                this.KyoutyouLength = calcFileData.Base.Bw ?? 0;
            }
            else
            {
                // どちらでもない場合は順引(幅員の長さ)として設定
                this.HukuinLength = calcFileData.Base.Bw ?? 0;
            }

            // 根太 サイズ
            var sizeNeda = GetKetazaiSizeByInputCalcFileData(calcFileData.Girder.Size, false);
            var typeNeda = GetKetazaiTypeByInputCalcFileData(sizeNeda, false);
            this.nedaData.NedaType = typeNeda;
            this.nedaData.NedaSize = sizeNeda;
            // 根太 突き出し長さ 始点側
            this.nedaData.exNedaStartLeng = calcFileData.Girder.LL ?? 0;
            // 根太 突き出し長さ 終点側
            this.nedaData.exNedaEndLeng = calcFileData.Girder.LR ?? 0;
            // 根太 材質
            if (!string.IsNullOrEmpty(calcFileData.Girder.Kind))
            {
                this.nedaData.NedaMaterial = calcFileData.Girder.Kind;
            }
            // 根太 ピッチ
            var nedaPitchValue = calcFileData.Girder.GirderPitch.Pitches.OrderBy(x => x.No).Select(x => double.Parse(x.Value.ToString())).ToList();
            // 計算書取込データには先頭ピッチ(0)が無いので挿入
            nedaPitchValue.Insert(0, 0);
            this.NedaPitch = nedaPitchValue;

            // 大引 サイズ
            var sizeOhbiki = GetKetazaiSizeByInputCalcFileData(calcFileData.CatchBeam.Size, false);
            var typeOhbiki = GetKetazaiTypeByInputCalcFileData(sizeOhbiki, false);
            this.ohbikiData.OhbikiType = typeOhbiki;
            this.ohbikiData.OhbikiSize = sizeOhbiki;
            // 大引 突き出し長さ 始点側
            this.ohbikiData.exOhbikiStartLeng = calcFileData.CatchBeam.LL ?? 0;
            // 大引 突き出し長さ 終点側
            this.ohbikiData.exOhbikiEndLeng = calcFileData.CatchBeam.LR ?? 0;
            // 大引 材質
            if (!string.IsNullOrEmpty(calcFileData.CatchBeam.Kind))
            {
                this.ohbikiData.OhbikiMaterial = calcFileData.CatchBeam.Kind;
            }
            // 大引 縦
            var obikiDanValue = calcFileData.CatchBeam.TATE;
            this.ohbikiData.OhbikiDan = obikiDanValue == "シングル" ? 1 : obikiDanValue == "ダブル" ? 2 : obikiDanValue == "トリプル" ? 3 : 1;
            // 大引 取付方法
            var ohbikiAttachTypeValue = calcFileData.CatchBeam.FitMeth;
            this.ohbikiData.OhbikiAttachType = ohbikiAttachTypeValue == "ボルト" ? eJoinType.Bolt : ohbikiAttachTypeValue == "溶接" ? eJoinType.Welding : eJoinType.Bolt;
            // 大引 ボルト本数
            var ohbikiBoltCountValue = int.TryParse(calcFileData.CatchBeam.BoltNum, out int ohbikiBoltCountConvertValue) ? ohbikiBoltCountConvertValue : 0;
            this.ohbikiData.OhbikiBoltCount = ohbikiBoltCountValue;

            // 杭 サイズ
            var sizeKui = GetKetazaiSizeByInputCalcFileData(calcFileData.Pile.Size, true);
            var typeKui = GetKetazaiTypeByInputCalcFileData(sizeKui, true);
            this.pilePillerData.PillarType = typeKui;
            this.pilePillerData.PillarSize = sizeKui;
            // 杭 材質
            if (!string.IsNullOrEmpty(calcFileData.Pile.Kind))
            {
                this.pilePillerData.PillarMaterial = calcFileData.Pile.Kind;
            }
            // 杭 杭長
            this.pilePillerData.PillarLength = calcFileData.Pile.Length ?? 0;
            // 杭 杭の頭出し量
            this.pilePillerData.exPillarHeadLeng = calcFileData.Pile.AtamaL ?? 0;
            // 杭 左端支柱と主桁のずれ
            this.beginNedaDiff = calcFileData.Pile.DD ?? 0;


            // 杭 ピッチ
            var pillarPitchValue = calcFileData.Pile.PilePitch.Pitches.OrderBy(x => x.No).Select(x => double.Parse(x.Value.ToString())).ToList();
            // 計算書取込データには先頭ピッチ(0)が無いので挿入
            pillarPitchValue.Insert(0, 0);

            if (isJyunbiki)
            {
                // 順引の場合は幅員方向を設定
                this.HukuinPillarPitch = pillarPitchValue;
            }
            else if (isGyakubiki)
            {
                // 順引の場合は橋軸方向を設定
                this.KyoutyouPillarPitch = pillarPitchValue;
            }
            else
            {
                // どちらでもない場合は順引(幅員方向)として設定
                this.HukuinPillarPitch = pillarPitchValue;
            }

            // 敷桁 サイズ
            var sizeShikigeta = GetKetazaiSizeByInputCalcFileData(calcFileData.WallGrider.Size, false);
            var typeShikigeta = GetKetazaiTypeByInputCalcFileData(sizeShikigeta, false);
            this.shikigetaData.ShikigetaType = typeShikigeta;
            this.shikigetaData.ShikigetaSize = sizeShikigeta;
            if (!string.IsNullOrEmpty(calcFileData.WallGrider.Kind))
            {
                this.shikigetaData.ShikigetaMaterial = calcFileData.WallGrider.Kind;
            }

            // 覆工板 覆工板無し
            this.HasFukkouban = calcFileData.CoverPlate.Exist == "ON" || (calcFileData.CoverPlate.Exist != "OFF");
            // 覆工板 材質
            if (!string.IsNullOrEmpty(calcFileData.CoverPlate.Kind))
            {
                this.FukkoubanData.FukkouMaterial = calcFileData.CoverPlate.Kind;
            }
            // 覆工板 幅員方向 サイズ
            this.FukkoubanSizeList = calcFileData.CoverPlate.CoverPlatePitch.Sizes.OrderBy(x => x.No).Select(x => GetFukkoubanSizeByInputCalcFileData(x.Value, calcFileData.CoverPlate.Kind)).ToList();
        }

        /// <summary>
        /// 計算書から取り込んだ桁材サイズをマスタファイルから引き当てる
        /// </summary>
        /// <param name="size">計算書から取り込んだ桁材サイズ</param>
        /// <param name="isKui">杭か否か</param>
        /// <returns>引き当てたサイズ</returns>
        private string GetKetazaiSizeByInputCalcFileData(string size, bool isKui)
        {
            var sizeHBeam = Master.ClsHBeamCsv.ExistSize(size);
            if (!string.IsNullOrEmpty(sizeHBeam))
            {
                return sizeHBeam;
            }

            if (!isKui)
            {
                var sizeChannel = Master.ClsChannelCsv.ExistSize(size);
                if (!string.IsNullOrEmpty(sizeChannel))
                {
                    return sizeChannel;
                }

                var sizeAngle = Master.ClsAngleCsv.ExistSize(size);
                if (!string.IsNullOrEmpty(sizeAngle))
                {
                    return sizeAngle;
                }
            }

            // 末尾に「*」が付与されている場合は山留仮鋼材を引き当てる
            if (size.EndsWith("*"))
            {
                size = size.TrimEnd('*');
                var sizeYamadome = Master.ClsKouzaiSpecify.GetYamadomeKarikouzai(size);
                if (!string.IsNullOrEmpty(sizeYamadome))
                {
                    return sizeYamadome;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 計算書から取り込んだ桁材サイズから種別を取得する
        /// </summary>
        /// <param name="size">計算書から取り込んだ桁材サイズ</param>
        /// <param name="isKui">杭か否か</param>
        /// <returns>種別</returns>
        private string GetKetazaiTypeByInputCalcFileData(string size, bool isKui)
        {
            var typeHBeam = Master.ClsHBeamCsv.GetTypeBySize(size);
            if (!string.IsNullOrEmpty(typeHBeam))
            {
                return typeHBeam;
            }

            if (!isKui)
            {
                var typeChannel = Master.ClsChannelCsv.GetTypeBySize(size);
                if (!string.IsNullOrEmpty(typeChannel))
                {
                    return typeChannel;
                }

                var typeAngle = Master.ClsAngleCsv.GetTypeBySize(size);
                if (!string.IsNullOrEmpty(typeAngle))
                {
                    return typeAngle;
                }
            }

            var typeYamadome = Master.ClsYamadomeCsv.GetTypeBySize(size);
            if (!string.IsNullOrEmpty(typeYamadome))
            {
                return typeYamadome;
            }

            return string.Empty;
        }

        /// <summary>
        /// 計算書から取り込んだ覆工板サイズをマスタファイルから引き当てる
        /// </summary>
        /// <param name="size">計算書から取り込んだ覆工板サイズ</param>
        /// <param name="material">計算書から取り込んだ覆工板材質</param>
        /// <returns>引き当てたサイズ</returns>
        private string GetFukkoubanSizeByInputCalcFileData(string size, string material)
        {
            var sizeFukkouban = Master.ClsFukkoubanCsv.ExistSizeForCalc(size, material);
            if (!string.IsNullOrEmpty(sizeFukkouban))
            {
                return sizeFukkouban;
            }

            return string.Empty;
        }
    }
}