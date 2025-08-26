using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitUtil;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using YMS.Command;
using YMS.Parts;

namespace YMS.DLG
{

    public partial class DlgBaseList : System.Windows.Forms.Form
    {

        #region メンバ変数
        public Document m_doc;
        private RequestHandler m_Handler;
        private ExternalEvent m_ExEvent;
        public ElementId m_HighLightId;
        public BaseListCommand m_Command;
        public List<ElementId> m_NumberList;
        public string m_TabName;
        public OverrideGraphicSettings m_originalSettings;
        public ElementId m_originalSettingsId;
        #endregion

        #region プロパティ
        public List<ClsHaraokoshiBase> ListHaraokoshiBase;
        public List<ClsKiribariBase> ListKiribariBase;
        public List<ClsCornerHiuchiBase> ListCornerHiuchiBase;
        public List<ClsKiribariHiuchiBase> ListKiribariHiuchiBase;
        public List<ClsKiribariUkeBase> ListKiribariUkeBase;
        public List<ClsKiribariTsunagizaiBase> ListKiribariTsunagizaiBase;
        public List<ClsHiuchiTsunagizaiBase> ListHiuchiTsunagizaiBase;
        public List<ClsKiribariTsugiBase> ListKiribariTsugiBase;
        public List<ClsSyabariBase> ListSyabariBase;
        public List<ClsSyabariTsunagizaiBase> ListSyabariTsunagizaiBase;
        public List<ClsSyabariUkeBase> ListSyabariUkeBase;
        #endregion

        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DlgBaseList(Document doc, ExternalEvent exEvent, RequestHandler handler,
                           List<ClsHaraokoshiBase> lstH,
                           List<ClsKiribariBase> lstK,
                           List<ClsCornerHiuchiBase> lstHB,
                           List<ClsKiribariHiuchiBase> lstKH,
                           List<ClsKiribariUkeBase> lstKU,
                           List<ClsKiribariTsunagizaiBase> lstKT,
                           List<ClsHiuchiTsunagizaiBase> lstHT,
                           List<ClsKiribariTsugiBase> lstKTsugi,
                           List<ClsSyabariBase> lstSB,
                           List<ClsSyabariTsunagizaiBase> lstST,
                           List<ClsSyabariUkeBase> lstSU)
        {
            InitializeComponent();

            m_doc = doc;
            m_Handler = handler;
            m_ExEvent = exEvent;
            ListHaraokoshiBase = lstH;
            ListKiribariBase = lstK;
            ListCornerHiuchiBase = lstHB;
            ListKiribariHiuchiBase = lstKH;
            ListKiribariUkeBase = lstKU;
            ListKiribariTsunagizaiBase = lstKT;
            ListHiuchiTsunagizaiBase = lstHT;
            ListKiribariTsugiBase = lstKTsugi;
            ListSyabariBase = lstSB;
            ListSyabariTsunagizaiBase = lstST;
            ListSyabariUkeBase = lstSU;

            SetDataGridView();

        }
        #endregion

        #region イベント処理
        private void DlgBaseList_Load(object sender, EventArgs e)
        {
            if(ClsBaseList.m_LastLocation != System.Drawing.Point.Empty)
            {
                this.StartPosition = FormStartPosition.Manual;
                this.Location = ClsBaseList.m_LastLocation;
            }
        }
        private void DlgBaseList_Shown(object sender, EventArgs e)
        {
            //タブ変更イベントを走らせたい
            TabBaseList.SelectedTab = tabKiribari;
            TabBaseList.SelectedTab = tabHaraokoshi;
            if(ClsBaseList.m_LastTabName != string.Empty)
            {
                foreach(TabPage tab in TabBaseList.TabPages)
                {
                    if(ClsBaseList.m_LastTabName == tab.Text)
                    {
                        TabBaseList.SelectedTab = tab;
                    }
                }
            }
        }

        private void TabBaseList_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabPage tabPage = TabBaseList.SelectedTab;
            m_TabName = tabPage.Text;
            SetDGVSelectedRow(m_TabName);
            m_Command = BaseListCommand.CreateNumber;
            MakeRequest(RequestId.BaseList);
        }

        private void DlgBaseList_FormClosed(object sender, FormClosedEventArgs e)
        {
            //初期化処理
            MakeRequest(RequestId.End);
            m_ExEvent.Dispose();
            m_ExEvent = null;
            m_Handler = null;
        }

        /// <summary>
        /// 編集ボタン押下処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (TabBaseList.SelectedTab == null)
            {
                return;
            }
            string selectedTabName = TabBaseList.SelectedTab.Text;

            switch (selectedTabName)
            {
                case "腹起":
                    EditHaraokoshi();
                    break;
                case "切梁":
                    EditKiribari();
                    break;
                case "隅火打":
                    EditCornerHiuchi();
                    break;
                case "切梁火打":
                    EditKiribariHiuchi();
                    break;
                case "切梁受け":
                    EditKiribariUke();
                    break;
                case "切梁繋ぎ材":
                    EditKiribariTsunagizai();
                    break;
                case "火打繋ぎ材":
                    EditHiuchiTsunagizai();
                    break;
                case "切梁継ぎ":
                    EditKiribariTsugi();
                    break;
                case "斜梁":
                    EditSyabari();
                    break;
                case "斜梁繋ぎ材":
                    EditSyabariTsunagizai();
                    break;
                case "斜梁受け材":
                    EditSyabariUke();
                    break;
                default:
                    break;
            }

            return;
        }

        /// <summary>
        /// 適用ボタン押下処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApply_Click(object sender, EventArgs e)
        {
            UpdateBaseList();

            m_Command = BaseListCommand.Change;
            ClsBaseList.m_LastTabName = TabBaseList.SelectedTab.Text;
            MakeRequest(RequestId.BaseList);
        }

        /// <summary>
        /// 閉じるボタン押下処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            m_Command = BaseListCommand.Close;
            MakeRequest(RequestId.BaseList);
        }

        #region モーダレス
        private void MakeRequest(RequestId request)
        {
            m_Handler.Request.Make(request);
            m_ExEvent.Raise();
            DozeOff();
        }
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
            if (!status)
            {
                //this.btnCancel.Enabled = true;
            }
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

        #region 腹起イベント
        private void dgvHaraokoshi_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void dgvHaraokoshi_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int nRowIndex = e.RowIndex;
            if (nRowIndex < 0) return;
            HighLight_Haraokoshi(nRowIndex);
            m_Command = BaseListCommand.HighLight;
            MakeRequest(RequestId.BaseList);
        }
        #endregion
        #region 切梁イベント
        private void dgvKiribari_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
        }
        private void dgvKiribari_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int nRowIndex = e.RowIndex;
            if (nRowIndex < 0) return;
            HighLight_Kiribari(nRowIndex);
            m_Command = BaseListCommand.HighLight;
            MakeRequest(RequestId.BaseList);
        }
        #endregion
        #region 隅火打イベント
        private void dgvCornerHiuchi_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void dgvCornerHiuchi_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int nRowIndex = e.RowIndex;
            if (nRowIndex < 0) return;
            HighLight_CornerHiuchi(nRowIndex);
            m_Command = BaseListCommand.HighLight;
            MakeRequest(RequestId.BaseList);
        }
        #endregion
        #region 切梁火打イベント
        private void dgvKiribariHiuchi_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
        }
        private void dgvKiribariHiuchi_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int nRowIndex = e.RowIndex;
            if (nRowIndex < 0) return;
            HighLight_KiribariHiuchi(nRowIndex);
            m_Command = BaseListCommand.HighLight;
            MakeRequest(RequestId.BaseList);
        }
        #endregion
        #region 切梁受けイベント
        private void dgvKiribariUke_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
        }
        private void dgvKiribariUke_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int nRowIndex = e.RowIndex;
            if (nRowIndex < 0) return;
            HighLight_KiribariUke(nRowIndex);
            m_Command = BaseListCommand.HighLight;
            MakeRequest(RequestId.BaseList);
        }
        #endregion
        #region 切梁繋ぎ材
        private void dgvKiribariTsunagi_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
        }
        private void dgvKiribariTsunagi_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int nRowIndex = e.RowIndex;
            if (nRowIndex < 0) return;
            HighLight_KiribariTsunagizai(nRowIndex);
            m_Command = BaseListCommand.HighLight;
            MakeRequest(RequestId.BaseList);
        }
        #endregion
        #region 火打繋ぎ材
        private void dgvHiuchiTsunagi_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
        }
        private void dgvHiuchiTsunagi_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int nRowIndex = e.RowIndex;
            if (nRowIndex < 0) return;
            HighLight_HiuchiTsunagizai(nRowIndex);
            m_Command = BaseListCommand.HighLight;
            MakeRequest(RequestId.BaseList);
        }
        #endregion
        #region 切梁継ぎイベント
        private void dgvKiribariTsugi_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
        }
        private void dgvKiribariTsugi_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int nRowIndex = e.RowIndex;
            if (nRowIndex < 0) return;
            HighLight_KiribariTsugi(nRowIndex);
            m_Command = BaseListCommand.HighLight;
            MakeRequest(RequestId.BaseList);
        }
        #endregion
        #region 斜梁イベント
        private void dgvSyabari_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int nRowIndex = e.RowIndex;
            if (nRowIndex < 0) return;
            HighLight_Syabari(nRowIndex);
            m_Command = BaseListCommand.HighLight;
            MakeRequest(RequestId.BaseList);
        }
        #endregion
        #region 斜梁繋ぎ材イベント
        private void dgvSyabariTsunagizai_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int nRowIndex = e.RowIndex;
            if (nRowIndex < 0) return;
            HighLight_SyabariTsunagizai(nRowIndex);
            m_Command = BaseListCommand.HighLight;
            MakeRequest(RequestId.BaseList);
        }
        #endregion
        #region 斜梁受け材イベント
        private void dgvSyabariUke_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int nRowIndex = e.RowIndex;
            if (nRowIndex < 0) return;
            HighLight_SyabariUke(nRowIndex);
            m_Command = BaseListCommand.HighLight;
            MakeRequest(RequestId.BaseList);
        }
        #endregion
        #endregion

        #region メソッド

        /// <summary>
        /// メンバ変数の値でグリッドの情報を更新
        /// </summary>
        private void SetDataGridView()
        {
            SetDataGridViewHaraokoshi();
            SetDataGridViewKiribari();
            SetDataGridViewCornerHiuchi();
            SetDataGridViewKiribariHiuchi();
            SetDataGridViewKiribariUke();
            SetDataGridViewKiribariTsunagizai();
            SetDataGridViewHiuchiTsunagizai();
            SetDataGridViewKiribariTsugi();
            SetDataGridViewSyabari();
            SetDataGridViewSyabariTsunagizai();
            SetDataGridViewSyabariUke();
        }

        #region 腹起

        /// <summary>
        /// メンバ変数の値でグリッドの情報を更新(腹起)
        /// </summary>
        private void SetDataGridViewHaraokoshi()
        {
            DataGridView dgv = dgvHaraokoshi;
            List<ClsHaraokoshiBase> lstData = ListHaraokoshiBase;

            for (int i = 0; i < lstData.Count; i++)
            {
                ClsHaraokoshiBase data = lstData[i];
                dgv.Rows.Add();

                SetDataGridViewHaraokoshi(i, data);
            }
            ChangeDGVColor(dgv);
        }

        /// <summary>
        /// メンバ変数の値でグリッドの行を更新(腹起)
        /// </summary>
        /// <param name="nIndex"></param>
        /// <param name="data"></param>
        private void SetDataGridViewHaraokoshi(int nIndex, ClsHaraokoshiBase data)
        {
            DataGridView dgv = dgvHaraokoshi;
            string str = string.Empty;
            dgv.Rows[nIndex].Tag = data;
            dgv[dgv.Columns.IndexOf(clmH_Floor), nIndex].Value = data.m_level;
            dgv[dgv.Columns.IndexOf(clmH_No), nIndex].Value = (nIndex + 1).ToString();
            dgv[dgv.Columns.IndexOf(clmH_DanLevel), nIndex].Value = data.m_dan.ToString();
            dgv[dgv.Columns.IndexOf(clmH_YokoNum), nIndex].Value = (data.m_yoko == ClsHaraokoshiBase.SideNum.Single ? "シングル" : "ダブル");
            dgv[dgv.Columns.IndexOf(clmH_TateNum), nIndex].Value = (data.m_tate == ClsHaraokoshiBase.VerticalNum.Single ? "シングル" : "ダブル");
            dgv[dgv.Columns.IndexOf(clmH_SteelType), nIndex].Value = data.m_kouzaiType.ToString();
            dgv[dgv.Columns.IndexOf(clmH_SteelSize), nIndex].Value = data.m_kouzaiSize.ToString();
            //dgv[dgv.Columns.IndexOf(clmH_Mega), nIndex].Value = data.m_bMega;
            dgv[dgv.Columns.IndexOf(clmH_MegaNum), nIndex].Value = data.m_mega.ToString();
            //dgv[dgv.Columns.IndexOf(clmH_SMH), nIndex].Value = data.m_bSMH;
            dgv[dgv.Columns.IndexOf(clmH_Offset), nIndex].Value = data.m_offset.ToString();
            dgv[dgv.Columns.IndexOf(clmH_TateSukima), nIndex].Value = data.m_tateGap.ToString();
            dgv[dgv.Columns.IndexOf(clmH_SukimaSteelType), nIndex].Value = data.m_gapTyouseiType.ToString();
            dgv[dgv.Columns.IndexOf(clmH_SukimaChoseizai), nIndex].Value = data.m_gapTyousei.ToString();
            dgv[dgv.Columns.IndexOf(clmH_SukimaLength), nIndex].Value = data.m_gapTyouseiLenght.ToString();
            if (data.m_katimake == ClsHaraokoshiBase.WinLose.Win) str = "勝ち";
            else if (data.m_katimake == ClsHaraokoshiBase.WinLose.Lose) str = "負け";
            dgv[dgv.Columns.IndexOf(clmH_WinLose), nIndex].Value = str;
        }

        /// <summary>
        /// 編集ダイアログを呼び出し、情報を編集
        /// </summary>
        /// <param name="rows"></param>
        private void EditHaraokoshi()
        {
            ClsHaraokoshiBase templateBase = new ClsHaraokoshiBase();
            ClsHaraokoshiBase sameBase = new ClsHaraokoshiBase();
            bool first = true;
            double offset = 0.0;
            foreach (DataGridViewRow row in dgvHaraokoshi.SelectedRows)
            {
                if (first)
                {
                    templateBase = (ClsHaraokoshiBase)dgvHaraokoshi.Rows[row.Index].Tag;
                    first = false;
                }
                ClsHaraokoshiBase compareBase = (ClsHaraokoshiBase)dgvHaraokoshi.Rows[row.Index].Tag;
                sameBase.m_ShoriType = templateBase.m_ShoriType;//ClsRevitUtil.CompareValues(compareBase.m_ShoriType, templateBase.m_ShoriType);
                sameBase.m_level = templateBase.m_level;// ClsRevitUtil.CompareValues(compareBase.m_level, templateBase.m_level);
                sameBase.m_dan = templateBase.m_dan;// ClsRevitUtil.CompareValues(compareBase.m_dan, templateBase.m_dan);
                sameBase.m_yoko = templateBase.m_yoko;// ClsRevitUtil.CompareValues(compareBase.m_yoko, templateBase.m_yoko);
                sameBase.m_tate = templateBase.m_tate;// ClsRevitUtil.CompareValues(compareBase.m_tate, templateBase.m_tate);
                sameBase.m_kouzaiType = templateBase.m_kouzaiType;// ClsRevitUtil.CompareValues(compareBase.m_kouzaiType, templateBase.m_kouzaiType);
                sameBase.m_kouzaiSize = templateBase.m_kouzaiSize;// ClsRevitUtil.CompareValues(compareBase.m_kouzaiSize, templateBase.m_kouzaiSize);
                sameBase.m_mega = ClsRevitUtil.CompareValues(compareBase.m_mega, templateBase.m_mega);
                sameBase.m_offset = ClsRevitUtil.CompareValues(compareBase.m_offset, templateBase.m_offset);
                sameBase.m_tateGap = ClsRevitUtil.CompareValues(compareBase.m_tateGap, templateBase.m_tateGap);
                sameBase.m_gapTyouseiType = templateBase.m_gapTyouseiType;// ClsRevitUtil.CompareValues(compareBase.m_gapTyouseiType, templateBase.m_gapTyouseiType);
                sameBase.m_gapTyousei = templateBase.m_gapTyousei;// ClsRevitUtil.CompareValues(compareBase.m_gapTyousei, templateBase.m_gapTyousei);
                sameBase.m_gapTyouseiLenght = ClsRevitUtil.CompareValues(compareBase.m_gapTyouseiLenght, templateBase.m_gapTyouseiLenght);
                sameBase.m_katimake = templateBase.m_katimake;// ClsRevitUtil.CompareValues(compareBase.m_katimake, templateBase.m_katimake);
            }

            //offset = sameBase.m_offset;

            DlgCreateHaraokoshiBase dlg = new DlgCreateHaraokoshiBase(m_doc, sameBase);
            DialogResult res = dlg.ShowDialog();
            if (res == DialogResult.OK)
            {
                ClsHaraokoshiBase data = dlg.m_ClsHaraBase;
                
                foreach (DataGridViewRow row in dgvHaraokoshi.SelectedRows)
                {
                    ClsHaraokoshiBase originalBase = (ClsHaraokoshiBase)dgvHaraokoshi.Rows[row.Index].Tag;
                    originalBase.m_FlgEdit = true;
                    originalBase.m_ShoriType = data.m_ShoriType;
                    originalBase.m_level = data.m_level;
                    originalBase.m_dan = data.m_dan;
                    originalBase.m_yoko = data.m_yoko;
                    originalBase.m_tate = data.m_tate;
                    originalBase.m_kouzaiType = data.m_kouzaiType;
                    originalBase.m_kouzaiSize = data.m_kouzaiSize;
                    originalBase.m_mega = data.m_mega;
                    originalBase.m_offset = data.m_offset - offset;
                    originalBase.m_tateGap = data.m_tateGap;
                    originalBase.m_gapTyouseiType = data.m_gapTyouseiType;
                    originalBase.m_gapTyousei = data.m_gapTyousei;
                    originalBase.m_gapTyouseiLenght = data.m_gapTyouseiLenght;
                    originalBase.m_katimake = data.m_katimake;
                    SetDataGridViewHaraokoshi(row.Index, originalBase);
                    ChangeColorChangeRows(row);
                }
            }
        }

        /// <summary>
        /// グリッドで選択しているベースをハイライト
        /// </summary>
        /// <param name="nIndex"></param>
        private void HighLight_Haraokoshi(int nIndex)
        {
            ClsHaraokoshiBase data = (ClsHaraokoshiBase)dgvHaraokoshi.Rows[nIndex].Tag;
            m_HighLightId = data.m_ElementId;
        }
        #endregion

        #region 切梁

        /// <summary>
        /// メンバ変数の値でグリッドの情報を更新(切梁)
        /// </summary>
        private void SetDataGridViewKiribari()
        {
            DataGridView dgv = dgvKiribari;
            List<ClsKiribariBase> lstData = ListKiribariBase;

            for (int i = 0; i < lstData.Count; i++)
            {
                ClsKiribariBase data = lstData[i];
                dgv.Rows.Add();

                SetDataGridViewKiribari(i, data);
            }
            ChangeDGVColor(dgv);
        }

        /// <summary>
        /// メンバ変数の値でグリッドの行を更新(切梁)
        /// </summary>
        /// <param name="nIndex"></param>
        /// <param name="data"></param>
        private void SetDataGridViewKiribari(int nIndex, ClsKiribariBase data)
        {
            DataGridView dgv = dgvKiribari;
            string str = string.Empty;
            dgv.Rows[nIndex].Tag = data;
            dgv[dgv.Columns.IndexOf(clmK_Floor), nIndex].Value = data.m_Floor;
            dgv[dgv.Columns.IndexOf(clmK_No), nIndex].Value = (nIndex + 1).ToString();

            //dgv[dgv.Columns.IndexOf(clmK_SteelType), nIndex].Value = data.m_kouzaiType;
            dgv[dgv.Columns.IndexOf(clmK_SteelSize), nIndex].Value = data.m_kouzaiSize;
            //dgv[dgv.Columns.IndexOf(clmK_TwinBeam), nIndex].Value = data.m_bTwin;
            //dgv[dgv.Columns.IndexOf(clmK_SMW), nIndex].Value = data.m_bSMH;
            dgv[dgv.Columns.IndexOf(clmK_TanbuPartsS), nIndex].Value = data.m_tanbuStart;
            dgv[dgv.Columns.IndexOf(clmK_TanbuPartsE), nIndex].Value = data.m_tanbuEnd;
            dgv[dgv.Columns.IndexOf(clmK_jackType1), nIndex].Value = data.m_jack1;
            dgv[dgv.Columns.IndexOf(clmK_JackType2), nIndex].Value = data.m_jack2;
        }

        /// <summary>
        /// 編集ダイアログを呼び出し、情報を編集
        /// </summary>
        /// <param name="nIndex"></param>
        private void EditKiribari()
        {
            ClsKiribariBase templateBase = new ClsKiribariBase();
            ClsKiribariBase sameBase = new ClsKiribariBase();
            bool first = true;
            foreach (DataGridViewRow row in dgvKiribari.SelectedRows)
            {
                if (first)
                {
                    templateBase = (ClsKiribariBase)dgvKiribari.Rows[row.Index].Tag;
                    first = false;
                }
                ClsKiribariBase compareBase = (ClsKiribariBase)dgvKiribari.Rows[row.Index].Tag;
                sameBase.m_kouzaiType = templateBase.m_kouzaiType;// ClsRevitUtil.CompareValues(compareBase.m_kouzaiType, templateBase.m_kouzaiType);
                sameBase.m_kouzaiSize = templateBase.m_kouzaiSize;// ClsRevitUtil.CompareValues(compareBase.m_kouzaiSize, templateBase.m_kouzaiSize);
                sameBase.m_ShoriType = templateBase.m_ShoriType;// ClsRevitUtil.CompareValues(compareBase.m_ShoriType, templateBase.m_ShoriType);
                sameBase.m_tanbuStart = templateBase.m_tanbuStart;// ClsRevitUtil.CompareValues(compareBase.m_tanbuStart, templateBase.m_tanbuStart);
                sameBase.m_tanbuEnd = templateBase.m_tanbuEnd;// ClsRevitUtil.CompareValues(compareBase.m_tanbuEnd, templateBase.m_tanbuEnd);
                sameBase.m_jack1 = templateBase.m_jack1;// ClsRevitUtil.CompareValues(compareBase.m_jack1, templateBase.m_jack1);
                sameBase.m_jack2 = templateBase.m_jack2;// ClsRevitUtil.CompareValues(compareBase.m_jack2, templateBase.m_jack2);
                //sameBase.m_Floor = ClsRevitUtil.CompareValues(compareBase.m_Floor, templateBase.m_Floor);
                //sameBase.m_Dan = ClsRevitUtil.CompareValues(compareBase.m_Dan, templateBase.m_Dan);
            }

            DlgCreateKiribariBase dlg = new DlgCreateKiribariBase(sameBase);
            DialogResult res = dlg.ShowDialog();
            if (res == DialogResult.OK)
            {
                ClsKiribariBase data = dlg.m_ClsKiribariBase;
                
                foreach (DataGridViewRow row in dgvKiribari.SelectedRows)
                {
                    ClsKiribariBase originalBase = (ClsKiribariBase)dgvKiribari.Rows[row.Index].Tag;
                    originalBase.m_FlgEdit = true;
                    originalBase.m_kouzaiType = data.m_kouzaiType;
                    originalBase.m_kouzaiSize = data.m_kouzaiSize;
                    originalBase.m_ShoriType = data.m_ShoriType;
                    originalBase.m_tanbuStart = data.m_tanbuStart;
                    originalBase.m_tanbuEnd = data.m_tanbuEnd;
                    originalBase.m_jack1 = data.m_jack1;
                    originalBase.m_jack2 = data.m_jack2;
                    //originalBase.m_Floor = data.m_Floor;
                    //originalBase.m_Dan = data.m_Dan;
                    SetDataGridViewKiribari(row.Index, originalBase);
                    ChangeColorChangeRows(row);
                }
            }
        }

        /// <summary>
        /// グリッドで選択しているベースをハイライト
        /// </summary>
        /// <param name="nIndex"></param>
        private void HighLight_Kiribari(int nIndex)
        {
            ClsKiribariBase data = (ClsKiribariBase)dgvKiribari.Rows[nIndex].Tag;
            m_HighLightId = data.m_ElementId;
        }
        #endregion

        #region 隅火打

        /// <summary>
        /// メンバ変数の値でグリッドの情報を更新(隅火打)
        /// </summary>
        private void SetDataGridViewCornerHiuchi()
        {
            DataGridView dgv = dgvCornerHiuchi;
            List<ClsCornerHiuchiBase> lstData = ListCornerHiuchiBase;

            for (int i = 0; i < lstData.Count; i++)
            {
                ClsCornerHiuchiBase data = lstData[i];
                dgv.Rows.Add();

                SetDataGridViewCornerHiuchi(i, data);
            }
            ChangeDGVColor(dgv);
        }

        /// <summary>
        /// メンバ変数の値でグリッドの行を更新(隅火打)
        /// </summary>
        /// <param name="nIndex"></param>
        /// <param name="data"></param>
        private void SetDataGridViewCornerHiuchi(int nIndex, ClsCornerHiuchiBase data)
        {
            DataGridView dgv = dgvCornerHiuchi;
            string str = string.Empty;
            dgv.Rows[nIndex].Tag = data;
            dgv[dgv.Columns.IndexOf(clmCH_Floor), nIndex].Value = data.m_Floor;
            dgv[dgv.Columns.IndexOf(clmCH_No), nIndex].Value = (nIndex + 1).ToString();

            dgv[dgv.Columns.IndexOf(clmCH_Kousei), nIndex].Value = (data.m_Kousei == ClsCornerHiuchiBase.Kousei.Single ? "シングル" : "ダブル");
            dgv[dgv.Columns.IndexOf(clmCH_Angle), nIndex].Value = (data.m_HiuchiAngle == ClsCornerHiuchiBase.HiuchiAngle.Toubun ? "等分" : "任意");
            dgv[dgv.Columns.IndexOf(clmCH_AngleNum), nIndex].Value = data.m_angle.ToString();
            dgv[dgv.Columns.IndexOf(clmCH_SteelType), nIndex].Value = data.m_SteelType;
            dgv[dgv.Columns.IndexOf(clmCH_SteelSize), nIndex].Value = data.m_SteelSize;
            dgv[dgv.Columns.IndexOf(clmCH_TanbuParts1), nIndex].Value = data.m_TanbuParts1;
            dgv[dgv.Columns.IndexOf(clmCH_HiuchiPieceSize1), nIndex].Value = data.m_HiuchiUkePieceSize1;
            dgv[dgv.Columns.IndexOf(clmCH_TanbuParts2), nIndex].Value = data.m_TanbuParts2;
            dgv[dgv.Columns.IndexOf(clmCH_HiuchiPieceSize2), nIndex].Value = data.m_HiuchiUkePieceSize2;
            dgv[dgv.Columns.IndexOf(clmCH_HiuchiLengthSingle), nIndex].Value = data.m_HiuchiLengthSingleL.ToString();
            dgv[dgv.Columns.IndexOf(clmCH_HiuchiLengthDoubleL1), nIndex].Value = data.m_HiuchiLengthDoubleUpperL1.ToString();
            dgv[dgv.Columns.IndexOf(clmCH_HiuchiLengthDoubleL2), nIndex].Value = data.m_HiuchiLengthDoubleUnderL2.ToString();
            dgv[dgv.Columns.IndexOf(clmCH_ZureRyo), nIndex].Value = data.m_HiuchiZureryo.ToString();
        }

        /// <summary>
        /// 編集ダイアログを呼び出し、情報を編集
        /// </summary>
        /// <param name="nIndex"></param>
        private void EditCornerHiuchi()
        {
            ClsCornerHiuchiBase templateBase = new ClsCornerHiuchiBase();
            ClsCornerHiuchiBase sameBase = new ClsCornerHiuchiBase();
            bool first = true;
            foreach (DataGridViewRow row in dgvCornerHiuchi.SelectedRows)
            {
                if (first)
                {
                    templateBase = (ClsCornerHiuchiBase)dgvCornerHiuchi.Rows[row.Index].Tag;
                    first = false;
                }
                ClsCornerHiuchiBase compareBase = (ClsCornerHiuchiBase)dgvCornerHiuchi.Rows[row.Index].Tag;
                sameBase.m_Kousei = templateBase.m_Kousei;// ClsRevitUtil.CompareValues(compareBase.m_Kousei, templateBase.m_Kousei);
                sameBase.m_HiuchiAngle = templateBase.m_HiuchiAngle;// ClsRevitUtil.CompareValues(compareBase.m_HiuchiAngle, templateBase.m_HiuchiAngle);
                sameBase.m_angle = ClsRevitUtil.CompareValues(compareBase.m_angle, templateBase.m_angle);
                sameBase.m_SteelType = templateBase.m_SteelType;// ClsRevitUtil.CompareValues(compareBase.m_SteelType, templateBase.m_SteelType);
                sameBase.m_SteelSize = templateBase.m_SteelSize;// ClsRevitUtil.CompareValues(compareBase.m_SteelSize, templateBase.m_SteelSize);
                sameBase.m_TanbuParts1 = templateBase.m_TanbuParts1;// ClsRevitUtil.CompareValues(compareBase.m_TanbuParts1, templateBase.m_TanbuParts1);
                sameBase.m_HiuchiUkePieceSize1 = templateBase.m_HiuchiUkePieceSize1;// ClsRevitUtil.CompareValues(compareBase.m_HiuchiUkePieceSize1, templateBase.m_HiuchiUkePieceSize1);
                sameBase.m_TanbuParts2 = templateBase.m_TanbuParts2;// ClsRevitUtil.CompareValues(compareBase.m_TanbuParts2, templateBase.m_TanbuParts2);
                sameBase.m_HiuchiUkePieceSize2 = templateBase.m_HiuchiUkePieceSize2;// ClsRevitUtil.CompareValues(compareBase.m_HiuchiUkePieceSize2, templateBase.m_HiuchiUkePieceSize2);
                sameBase.m_HiuchiTotalLength = ClsRevitUtil.CompareValues(compareBase.m_HiuchiTotalLength, templateBase.m_HiuchiTotalLength);
                sameBase.m_HiuchiLengthSingleL = ClsRevitUtil.CompareValues(compareBase.m_HiuchiLengthSingleL, templateBase.m_HiuchiLengthSingleL);
                sameBase.m_HiuchiLengthDoubleUpperL1 = ClsRevitUtil.CompareValues(compareBase.m_HiuchiLengthDoubleUpperL1, templateBase.m_HiuchiLengthDoubleUpperL1);
                sameBase.m_HiuchiLengthDoubleUnderL2 = ClsRevitUtil.CompareValues(compareBase.m_HiuchiLengthDoubleUnderL2, templateBase.m_HiuchiLengthDoubleUnderL2);
                sameBase.m_HiuchiZureryo = ClsRevitUtil.CompareValues(compareBase.m_HiuchiZureryo, templateBase.m_HiuchiZureryo);
            }

            DlgCreateCornerHiuchiBase dlg = new DlgCreateCornerHiuchiBase(sameBase);
            DialogResult res = dlg.ShowDialog();
            if (res == DialogResult.OK)
            {
                ClsCornerHiuchiBase data = dlg.m_ClsCornerHiuchiBase;
                foreach (DataGridViewRow row in dgvCornerHiuchi.SelectedRows)
                {
                    ClsCornerHiuchiBase originalBase = (ClsCornerHiuchiBase)dgvCornerHiuchi.Rows[row.Index].Tag;
                    originalBase.m_FlgEdit = true;
                    originalBase.m_Kousei = data.m_Kousei;
                    originalBase.m_HiuchiAngle = data.m_HiuchiAngle;
                    originalBase.m_angle = data.m_angle;
                    originalBase.m_SteelType = data.m_SteelType;
                    originalBase.m_SteelSize = data.m_SteelSize;
                    originalBase.m_TanbuParts1 = data.m_TanbuParts1;
                    originalBase.m_HiuchiUkePieceSize1 = data.m_HiuchiUkePieceSize1;
                    originalBase.m_TanbuParts2 = data.m_TanbuParts2;
                    originalBase.m_HiuchiUkePieceSize2 = data.m_HiuchiUkePieceSize2;
                    originalBase.m_HiuchiTotalLength = data.m_HiuchiTotalLength;
                    originalBase.m_HiuchiLengthSingleL = data.m_HiuchiLengthSingleL;
                    originalBase.m_HiuchiLengthDoubleUpperL1 = data.m_HiuchiLengthDoubleUpperL1;
                    originalBase.m_HiuchiLengthDoubleUnderL2 = data.m_HiuchiLengthDoubleUnderL2;
                    originalBase.m_HiuchiZureryo = data.m_HiuchiZureryo;

                    SetDataGridViewCornerHiuchi(row.Index, originalBase);
                    ChangeColorChangeRows(row);
                }
            }
        }

        /// <summary>
        /// グリッドで選択しているベースをハイライト
        /// </summary>
        /// <param name="nIndex"></param>
        private void HighLight_CornerHiuchi(int nIndex)
        {
            ClsCornerHiuchiBase data = (ClsCornerHiuchiBase)dgvCornerHiuchi.Rows[nIndex].Tag;
            m_HighLightId = data.m_cornerHiuchiID;
        }
        #endregion

        #region 切梁火打

        /// <summary>
        /// メンバ変数の値でグリッドの情報を更新(切梁火打)
        /// </summary>
        private void SetDataGridViewKiribariHiuchi()
        {
            DataGridView dgv = dgvKiribariHiuchi;
            List<ClsKiribariHiuchiBase> lstData = ListKiribariHiuchiBase;

            for (int i = 0; i < lstData.Count; i++)
            {
                ClsKiribariHiuchiBase data = lstData[i];
                dgv.Rows.Add();

                SetDataGridViewKiribariHiuchi(i, data);
            }
            ChangeDGVColor(dgv);
        }

        /// <summary>
        /// メンバ変数の値でグリッドの行を更新(切梁火打)
        /// </summary>
        /// <param name="nIndex"></param>
        /// <param name="data"></param>
        private void SetDataGridViewKiribariHiuchi(int nIndex, ClsKiribariHiuchiBase data)
        {
            DataGridView dgv = dgvKiribariHiuchi;
            string str = string.Empty;
            dgv.Rows[nIndex].Tag = data;
            dgv[dgv.Columns.IndexOf(clmKH_Floor), nIndex].Value = data.m_Floor;
            dgv[dgv.Columns.IndexOf(clmKH_No), nIndex].Value = (nIndex + 1).ToString();

            if (data.m_ShoriType == ClsKiribariHiuchiBase.ShoriType.KiriKiri) str = "切梁-切梁";
            else if (data.m_ShoriType == ClsKiribariHiuchiBase.ShoriType.KiriHara) str = "切梁-腹起";
            else if (data.m_ShoriType == ClsKiribariHiuchiBase.ShoriType.SanjikuHara) str = "三軸ピース-腹起";
            dgv[dgv.Columns.IndexOf(clmKH_ShoriType), nIndex].Value = str;

            if (data.m_CreateType == ClsKiribariHiuchiBase.CreateType.Both) str = "両方";
            else if (data.m_CreateType == ClsKiribariHiuchiBase.CreateType.AnyLengthAuto) str = "片方（長さ入力）";
            else if (data.m_CreateType == ClsKiribariHiuchiBase.CreateType.AnyLengthManual) str = "片方（長さ自動）";
            dgv[dgv.Columns.IndexOf(clmKH_SakuseiHoho), nIndex].Value = str;

            dgv[dgv.Columns.IndexOf(clmKH_SingleDouble), nIndex].Value = (data.m_CreateHoho == ClsKiribariHiuchiBase.CreateHoho.Single ? "シングル" : "ダブル");
            dgv[dgv.Columns.IndexOf(clmKH_SteelType), nIndex].Value = data.m_SteelType;
            dgv[dgv.Columns.IndexOf(clmKH_SteelSizeSingle), nIndex].Value = data.m_SteelSizeSingle;
            dgv[dgv.Columns.IndexOf(clmKH_SteelSizeDouble1), nIndex].Value = data.m_SteelSizeDoubleUpper;

            dgv[dgv.Columns.IndexOf(clmKH_SteelSizeDouble2), nIndex].Value = data.m_SteelSizeDoubleUnder;
            dgv[dgv.Columns.IndexOf(clmKH_HiuchLengthSingleL), nIndex].Value = data.m_HiuchiLengthSingleL.ToString();
            dgv[dgv.Columns.IndexOf(clmKH_HiuchiLengthDoubleL1), nIndex].Value = data.m_HiuchiLengthDoubleUpperL1.ToString();
            dgv[dgv.Columns.IndexOf(clmKH_HiuchiLengthDoubleL2), nIndex].Value = data.m_HiuchiLengthDoubleUnderL2.ToString();
            dgv[dgv.Columns.IndexOf(clmKH_zureryo), nIndex].Value = data.m_HiuchiZureRyo.ToString();
            dgv[dgv.Columns.IndexOf(clmKH_PartsType1), nIndex].Value = data.m_PartsTypeKiriSide;
            dgv[dgv.Columns.IndexOf(clmKH_PartsSize1), nIndex].Value = data.m_PartsSizeKiriSide;
            dgv[dgv.Columns.IndexOf(clmKH_PartsType2), nIndex].Value = data.m_PartsTypeHaraSide;
            dgv[dgv.Columns.IndexOf(clmKH_PartsSize2), nIndex].Value = data.m_PartsSizeHaraSide;
            dgv[dgv.Columns.IndexOf(clmKH_Angle), nIndex].Value = data.m_Angle;
        }

        /// <summary>
        /// 編集ダイアログを呼び出し、情報を編集
        /// </summary>
        /// <param name="nIndex"></param>
        private void EditKiribariHiuchi()
        {
            ClsKiribariHiuchiBase templateBase = new ClsKiribariHiuchiBase();
            ClsKiribariHiuchiBase sameBase = new ClsKiribariHiuchiBase();
            bool first = true;
            foreach (DataGridViewRow row in dgvKiribariHiuchi.SelectedRows)
            {
                if (first)
                {
                    templateBase = (ClsKiribariHiuchiBase)dgvKiribariHiuchi.Rows[row.Index].Tag;
                    first = false;
                }
                ClsKiribariHiuchiBase compareBase = (ClsKiribariHiuchiBase)dgvKiribariHiuchi.Rows[row.Index].Tag;
                sameBase.m_CreateHoho = ClsRevitUtil.CompareValues(compareBase.m_CreateHoho, templateBase.m_CreateHoho);
                sameBase.m_SteelType = ClsRevitUtil.CompareValues(compareBase.m_SteelType, templateBase.m_SteelType);
                sameBase.m_SteelSizeSingle = templateBase.m_SteelSizeSingle;// ClsRevitUtil.CompareValues(compareBase.m_SteelSizeSingle, templateBase.m_SteelSizeSingle);
                sameBase.m_SteelSizeDoubleUpper = templateBase.m_SteelSizeDoubleUpper;// ClsRevitUtil.CompareValues(compareBase.m_SteelSizeDoubleUpper, templateBase.m_SteelSizeDoubleUpper);
                sameBase.m_SteelSizeDoubleUnder = templateBase.m_SteelSizeDoubleUnder;// ClsRevitUtil.CompareValues(compareBase.m_SteelSizeDoubleUnder, templateBase.m_SteelSizeDoubleUnder);
                sameBase.m_HiuchiLengthSingleL = ClsRevitUtil.CompareValues(compareBase.m_HiuchiLengthSingleL, templateBase.m_HiuchiLengthSingleL);
                sameBase.m_HiuchiLengthDoubleUpperL1 = ClsRevitUtil.CompareValues(compareBase.m_HiuchiLengthDoubleUpperL1, templateBase.m_HiuchiLengthDoubleUpperL1);
                sameBase.m_HiuchiLengthDoubleUnderL2 = ClsRevitUtil.CompareValues(compareBase.m_HiuchiLengthDoubleUnderL2, templateBase.m_HiuchiLengthDoubleUnderL2);
                sameBase.m_HiuchiZureRyo = ClsRevitUtil.CompareValues(compareBase.m_HiuchiZureRyo, templateBase.m_HiuchiZureRyo);
                sameBase.m_PartsTypeKiriSide = templateBase.m_PartsTypeKiriSide;// ClsRevitUtil.CompareValues(compareBase.m_PartsTypeKiriSide, templateBase.m_PartsTypeKiriSide);
                sameBase.m_PartsSizeKiriSide = templateBase.m_PartsSizeKiriSide;// ClsRevitUtil.CompareValues(compareBase.m_PartsSizeKiriSide, templateBase.m_PartsSizeKiriSide);
                sameBase.m_PartsTypeHaraSide = templateBase.m_PartsTypeHaraSide;// ClsRevitUtil.CompareValues(compareBase.m_PartsTypeHaraSide, templateBase.m_PartsTypeHaraSide);
                sameBase.m_PartsSizeHaraSide = templateBase.m_PartsSizeHaraSide;// ClsRevitUtil.CompareValues(compareBase.m_PartsSizeHaraSide, templateBase.m_PartsSizeHaraSide);
                sameBase.m_Angle = ClsRevitUtil.CompareValues(compareBase.m_Angle, templateBase.m_Angle);
            }

            DlgCreateKiribariHiuchiBase dlg = new DlgCreateKiribariHiuchiBase(templateBase);
            DialogResult res = dlg.ShowDialog();
            if (res == DialogResult.OK)
            {
                ClsKiribariHiuchiBase data = dlg.m_KiribariHiuchiBase;
                
                foreach (DataGridViewRow row in dgvKiribariHiuchi.SelectedRows)
                {
                    ClsKiribariHiuchiBase originalBase = (ClsKiribariHiuchiBase)dgvKiribariHiuchi.Rows[row.Index].Tag;
                    originalBase.m_FlgEdit = true;
                    originalBase.m_CreateHoho = data.m_CreateHoho;
                    originalBase.m_SteelType = data.m_SteelType;
                    originalBase.m_SteelSizeSingle = data.m_SteelSizeSingle;
                    originalBase.m_SteelSizeDoubleUpper = data.m_SteelSizeDoubleUpper;
                    originalBase.m_SteelSizeDoubleUnder = data.m_SteelSizeDoubleUnder;
                    originalBase.m_HiuchiLengthSingleL = data.m_HiuchiLengthSingleL;
                    originalBase.m_HiuchiLengthDoubleUpperL1 = data.m_HiuchiLengthDoubleUpperL1;
                    originalBase.m_HiuchiLengthDoubleUnderL2 = data.m_HiuchiLengthDoubleUnderL2;
                    originalBase.m_HiuchiZureRyo = data.m_HiuchiZureRyo;
                    originalBase.m_PartsTypeKiriSide = data.m_PartsTypeKiriSide;
                    originalBase.m_PartsSizeKiriSide = data.m_PartsSizeKiriSide;
                    originalBase.m_PartsTypeHaraSide = data.m_PartsTypeHaraSide;
                    originalBase.m_PartsSizeHaraSide = data.m_PartsSizeHaraSide;
                    originalBase.m_Angle = data.m_Angle;
                    SetDataGridViewKiribariHiuchi(row.Index, originalBase);
                    ChangeColorChangeRows(row);
                }
            }
        }

        /// <summary>
        /// グリッドで選択しているベースをハイライト
        /// </summary>
        /// <param name="nIndex"></param>
        private void HighLight_KiribariHiuchi(int nIndex)
        {
            ClsKiribariHiuchiBase data = (ClsKiribariHiuchiBase)dgvKiribariHiuchi.Rows[nIndex].Tag;
            m_HighLightId = data.m_ElementId;
        }
        #endregion

        #region 切梁受け

        /// <summary>
        /// メンバ変数の値でグリッドの情報を更新(切梁受け)
        /// </summary>
        private void SetDataGridViewKiribariUke()
        {
            DataGridView dgv = dgvKiribariUke;
            List<ClsKiribariUkeBase> lstData = ListKiribariUkeBase;

            for (int i = 0; i < lstData.Count; i++)
            {
                ClsKiribariUkeBase data = lstData[i];
                dgv.Rows.Add();

                SetDataGridViewKiribariUke(i, data);
            }
            ChangeDGVColor(dgv);
        }

        /// <summary>
        /// メンバ変数の値でグリッドの行を更新(切梁受け)
        /// </summary>
        /// <param name="nIndex"></param>
        /// <param name="data"></param>
        private void SetDataGridViewKiribariUke(int nIndex, ClsKiribariUkeBase data)
        {
            DataGridView dgv = dgvKiribariUke;
            string str = string.Empty;
            dgv.Rows[nIndex].Tag = data;
            dgv[dgv.Columns.IndexOf(clmKU_Floor), nIndex].Value = data.m_Floor;
            dgv[dgv.Columns.IndexOf(clmKU_No), nIndex].Value = (nIndex + 1).ToString();
            dgv[dgv.Columns.IndexOf(clmKU_SteeelSize), nIndex].Value = data.m_SteelType;
            dgv[dgv.Columns.IndexOf(clmKU_SteeelType), nIndex].Value = data.m_SteelSize;
            dgv[dgv.Columns.IndexOf(clmKU_TsukidashiRyoS), nIndex].Value = data.m_TsukidashiRyoS.ToString();
            dgv[dgv.Columns.IndexOf(clmKU_TsukidashiryoE), nIndex].Value = data.m_TsukidashiRyoE.ToString();
        }

        /// <summary>
        /// 編集ダイアログを呼び出し、情報を編集
        /// </summary>
        /// <param name="nIndex"></param>
        private void EditKiribariUke()
        {
            ClsKiribariUkeBase templateBase = new ClsKiribariUkeBase();
            ClsKiribariUkeBase sameBase = new ClsKiribariUkeBase();
            bool first = true;
            foreach (DataGridViewRow row in dgvKiribariUke.SelectedRows)
            {
                if (first)
                {
                    templateBase = (ClsKiribariUkeBase)dgvKiribariUke.Rows[row.Index].Tag;
                    first = false;
                }
                ClsKiribariUkeBase compareBase = (ClsKiribariUkeBase)dgvKiribariUke.Rows[row.Index].Tag;
                sameBase.m_ShoriType = ClsRevitUtil.CompareValues(compareBase.m_ShoriType, templateBase.m_ShoriType);
                sameBase.m_SteelType = templateBase.m_SteelType;// ClsRevitUtil.CompareValues(compareBase.m_SteelType, templateBase.m_SteelType);
                sameBase.m_SteelSize = templateBase.m_SteelSize;// ClsRevitUtil.CompareValues(compareBase.m_SteelSize, templateBase.m_SteelSize);
                sameBase.m_TsukidashiRyoS = ClsRevitUtil.CompareValues(compareBase.m_TsukidashiRyoS, templateBase.m_TsukidashiRyoS);
                sameBase.m_TsukidashiRyoE = ClsRevitUtil.CompareValues(compareBase.m_TsukidashiRyoE, templateBase.m_TsukidashiRyoE);
            }

            DlgCreateKiribariUkeBase dlg = new DlgCreateKiribariUkeBase(sameBase);
            DialogResult res = dlg.ShowDialog();
            if (res == DialogResult.OK)
            {
                ClsKiribariUkeBase data = dlg.m_KiribariUkeBase;
                
                foreach (DataGridViewRow row in dgvKiribariUke.SelectedRows)
                {
                    ClsKiribariUkeBase originalBase = (ClsKiribariUkeBase)dgvKiribariUke.Rows[row.Index].Tag;
                    originalBase.m_FlgEdit = true;
                    originalBase.m_ShoriType = data.m_ShoriType;
                    originalBase.m_SteelType = data.m_SteelType;
                    originalBase.m_SteelSize = data.m_SteelSize;
                    originalBase.m_TsukidashiRyoS = data.m_TsukidashiRyoS;
                    originalBase.m_TsukidashiRyoE = data.m_TsukidashiRyoE;
                    SetDataGridViewKiribariUke(row.Index, originalBase);
                    ChangeColorChangeRows(row);
                }
            }
        }

        /// <summary>
        /// グリッドで選択しているベースをハイライト
        /// </summary>
        /// <param name="nIndex"></param>
        private void HighLight_KiribariUke(int nIndex)
        {
            ClsKiribariUkeBase data = (ClsKiribariUkeBase)dgvKiribariUke.Rows[nIndex].Tag;
            m_HighLightId = data.m_ElementId;
        }
        #endregion

        #region 切梁繋ぎ材

        /// <summary>
        /// メンバ変数の値でグリッドの情報を更新(切梁繋ぎ材)
        /// </summary>
        private void SetDataGridViewKiribariTsunagizai()
        {
            DataGridView dgv = dgvKiribariTsunagi;
            List<ClsKiribariTsunagizaiBase> lstData = ListKiribariTsunagizaiBase;

            for (int i = 0; i < lstData.Count; i++)
            {
                ClsKiribariTsunagizaiBase data = lstData[i];
                dgv.Rows.Add();

                SetDataGridViewKiribariTsunagizai(i, data);
            }
            ChangeDGVColor(dgv);
        }

        /// <summary>
        /// メンバ変数の値でグリッドの行を更新(切梁繋ぎ材)
        /// </summary>
        /// <param name="nIndex"></param>
        /// <param name="data"></param>
        private void SetDataGridViewKiribariTsunagizai(int nIndex, ClsKiribariTsunagizaiBase data)
        {
            DataGridView dgv = dgvKiribariTsunagi;
            string str = string.Empty;
            dgv.Rows[nIndex].Tag = data;
            dgv[dgv.Columns.IndexOf(clmKT_Floor), nIndex].Value = data.m_Floor;
            dgv[dgv.Columns.IndexOf(clmKT_No), nIndex].Value = (nIndex + 1).ToString();
            dgv[dgv.Columns.IndexOf(clmKT_SteelType), nIndex].Value = data.m_SteelType;
            dgv[dgv.Columns.IndexOf(clmKT_SteelSize), nIndex].Value = data.m_SteelSize;
            dgv[dgv.Columns.IndexOf(clmKT_Bunrui), nIndex].Value = data.m_SplitFlg;


            if (data.m_ToritsukeHoho == ClsKiribariTsunagizaiBase.ToritsukeHoho.Bolt) str = "ボルト";
            else if (data.m_ToritsukeHoho == ClsKiribariTsunagizaiBase.ToritsukeHoho.Buruman) str = "ブルマン";
            else if (data.m_ToritsukeHoho == ClsKiribariTsunagizaiBase.ToritsukeHoho.Rikiman) str = "リキマン";
            dgv[dgv.Columns.IndexOf(clmKT_ToritsukeHoho), nIndex].Value = str;

            dgv[dgv.Columns.IndexOf(clmKT_BoltType), nIndex].Value = data.m_BoltType1;
            dgv[dgv.Columns.IndexOf(clmKT_BoltSize), nIndex].Value = data.m_BoltType2;
            dgv[dgv.Columns.IndexOf(clmKT_BoltNum), nIndex].Value = data.m_BoltNum.ToString();
        }

        /// <summary>
        /// 編集ダイアログを呼び出し、情報を編集
        /// </summary>
        /// <param name="nIndex"></param>
        private void EditKiribariTsunagizai()
        {
            ClsKiribariTsunagizaiBase templateBase = new ClsKiribariTsunagizaiBase();
            ClsKiribariTsunagizaiBase sameBase = new ClsKiribariTsunagizaiBase();
            bool first = true;
            foreach (DataGridViewRow row in dgvKiribariTsunagi.SelectedRows)
            {
                if (first)
                {
                    templateBase = (ClsKiribariTsunagizaiBase)dgvKiribariTsunagi.Rows[row.Index].Tag;
                    first = false;
                }
                ClsKiribariTsunagizaiBase compareBase = (ClsKiribariTsunagizaiBase)dgvKiribariTsunagi.Rows[row.Index].Tag;
                sameBase.m_SteelType = templateBase.m_SteelType;// ClsRevitUtil.CompareValues(compareBase.m_SteelType, templateBase.m_SteelType);
                sameBase.m_SteelSize = templateBase.m_SteelSize;// ClsRevitUtil.CompareValues(compareBase.m_SteelSize, templateBase.m_SteelSize);
                sameBase.m_ShoriType = ClsRevitUtil.CompareValues(compareBase.m_ShoriType, templateBase.m_ShoriType);
                sameBase.m_SplitFlg = ClsRevitUtil.CompareValues(compareBase.m_SplitFlg, templateBase.m_SplitFlg);
                sameBase.m_ToritsukeHoho = templateBase.m_ToritsukeHoho;// ClsRevitUtil.CompareValues(compareBase.m_ToritsukeHoho, templateBase.m_ToritsukeHoho);
                sameBase.m_ShoriType = ClsRevitUtil.CompareValues(compareBase.m_ShoriType, templateBase.m_ShoriType);
                sameBase.m_BoltType1 = templateBase.m_BoltType1;// ClsRevitUtil.CompareValues(compareBase.m_BoltType1, templateBase.m_BoltType1);
                sameBase.m_BoltType2 = templateBase.m_BoltType2;// ClsRevitUtil.CompareValues(compareBase.m_BoltType2, templateBase.m_BoltType2);
                sameBase.m_BoltNum = ClsRevitUtil.CompareValues(compareBase.m_BoltNum, templateBase.m_BoltNum);
            }

            DlgCreateKiribariTsunagizaiBase dlg = new DlgCreateKiribariTsunagizaiBase(sameBase);
            DialogResult res = dlg.ShowDialog();
            if (res == DialogResult.OK)
            {
                ClsKiribariTsunagizaiBase data = dlg.m_KiribariTsunagizaiBase;
                
                foreach (DataGridViewRow row in dgvKiribariTsunagi.SelectedRows)
                {
                    ClsKiribariTsunagizaiBase originalBase = (ClsKiribariTsunagizaiBase)dgvKiribariTsunagi.Rows[row.Index].Tag;
                    originalBase.m_FlgEdit = true;
                    originalBase.m_SteelType = data.m_SteelType;
                    originalBase.m_SteelSize = data.m_SteelSize;
                    originalBase.m_ShoriType = data.m_ShoriType;
                    originalBase.m_SplitFlg = data.m_SplitFlg;
                    originalBase.m_ToritsukeHoho = data.m_ToritsukeHoho;
                    originalBase.m_ShoriType = data.m_ShoriType;
                    originalBase.m_BoltType1 = data.m_BoltType1;
                    originalBase.m_BoltType2 = data.m_BoltType2;
                    originalBase.m_BoltNum = data.m_BoltNum;

                    SetDataGridViewKiribariTsunagizai(row.Index, originalBase);
                    ChangeColorChangeRows(row);
                }
            }

        }

        /// <summary>
        /// グリッドで選択しているベースをハイライト
        /// </summary>
        /// <param name="nIndex"></param>
        private void HighLight_KiribariTsunagizai(int nIndex)
        {
            ClsKiribariTsunagizaiBase data = (ClsKiribariTsunagizaiBase)dgvKiribariTsunagi.Rows[nIndex].Tag;
            m_HighLightId = data.m_ElementId;
        }
        #endregion

        #region 火打繋ぎ材

        /// <summary>
        /// メンバ変数の値でグリッドの情報を更新(火打繋ぎ材)
        /// </summary>
        private void SetDataGridViewHiuchiTsunagizai()
        {
            DataGridView dgv = dgvHiuchiTsunagi;
            List<ClsHiuchiTsunagizaiBase> lstData = ListHiuchiTsunagizaiBase;

            for (int i = 0; i < lstData.Count; i++)
            {
                ClsHiuchiTsunagizaiBase data = lstData[i];
                dgv.Rows.Add();

                SetDataGridViewHiuchiTsunagizai(i, data);
            }
            ChangeDGVColor(dgv);
        }

        /// <summary>
        /// メンバ変数の値でグリッドの行を更新(火打繋ぎ材)
        /// </summary>
        /// <param name="nIndex"></param>
        /// <param name="data"></param>
        private void SetDataGridViewHiuchiTsunagizai(int nIndex, ClsHiuchiTsunagizaiBase data)
        {
            DataGridView dgv = dgvHiuchiTsunagi;
            string str = string.Empty;
            dgv.Rows[nIndex].Tag = data;

            dgv[dgv.Columns.IndexOf(clmHT_Floor), nIndex].Value = data.m_Floor;
            dgv[dgv.Columns.IndexOf(clmHT_No), nIndex].Value = (nIndex + 1).ToString();
            dgv[dgv.Columns.IndexOf(clmHT_SteelType), nIndex].Value = data.m_SteelType;
            dgv[dgv.Columns.IndexOf(clmHT_SteelSize), nIndex].Value = data.m_SteelSize;
            dgv[dgv.Columns.IndexOf(clmHT_Bunrui), nIndex].Value = data.m_SplitFlg;

            if (data.m_ToritsukeHoho == ClsHiuchiTsunagizaiBase.ToritsukeHoho.Bolt) str = "ボルト";
            else if (data.m_ToritsukeHoho == ClsHiuchiTsunagizaiBase.ToritsukeHoho.Buruman) str = "ブルマン";
            else if (data.m_ToritsukeHoho == ClsHiuchiTsunagizaiBase.ToritsukeHoho.Rikiman) str = "リキマン";
            dgv[dgv.Columns.IndexOf(clmHT_ToritsukeHoho), nIndex].Value = str;

            dgv[dgv.Columns.IndexOf(clmHT_BoltType), nIndex].Value = data.m_BoltType1;
            dgv[dgv.Columns.IndexOf(clmHT_BoltSize), nIndex].Value = data.m_BoltType2;
            dgv[dgv.Columns.IndexOf(clmHT_BoltNum), nIndex].Value = data.m_BoltNum.ToString();
        }

        /// <summary>
        /// 編集ダイアログを呼び出し、情報を編集
        /// </summary>
        /// <param name="nIndex"></param>
        private void EditHiuchiTsunagizai()
        {
            ClsHiuchiTsunagizaiBase templateBase = new ClsHiuchiTsunagizaiBase();
            ClsHiuchiTsunagizaiBase sameBase = new ClsHiuchiTsunagizaiBase();
            bool first = true;
            foreach (DataGridViewRow row in dgvHiuchiTsunagi.SelectedRows)
            {
                if (first)
                {
                    templateBase = (ClsHiuchiTsunagizaiBase)dgvHiuchiTsunagi.Rows[row.Index].Tag;
                    first = false;
                }
                ClsHiuchiTsunagizaiBase compareBase = (ClsHiuchiTsunagizaiBase)dgvHiuchiTsunagi.Rows[row.Index].Tag;
                sameBase.m_SteelType = templateBase.m_SteelType;// ClsRevitUtil.CompareValues(compareBase.m_SteelType, templateBase.m_SteelType);
                sameBase.m_SteelSize = templateBase.m_SteelSize;// ClsRevitUtil.CompareValues(compareBase.m_SteelSize, templateBase.m_SteelSize);
                sameBase.m_ShoriType = ClsRevitUtil.CompareValues(compareBase.m_ShoriType, templateBase.m_ShoriType);
                sameBase.m_SplitFlg = ClsRevitUtil.CompareValues(compareBase.m_SplitFlg, templateBase.m_SplitFlg);
                sameBase.m_ToritsukeHoho = templateBase.m_ToritsukeHoho;// ClsRevitUtil.CompareValues(compareBase.m_ToritsukeHoho, templateBase.m_ToritsukeHoho);
                sameBase.m_ShoriType = ClsRevitUtil.CompareValues(compareBase.m_ShoriType, templateBase.m_ShoriType);
                sameBase.m_BoltType1 = templateBase.m_BoltType1;// ClsRevitUtil.CompareValues(compareBase.m_BoltType1, templateBase.m_BoltType1);
                sameBase.m_BoltType2 = templateBase.m_BoltType2;// ClsRevitUtil.CompareValues(compareBase.m_BoltType2, templateBase.m_BoltType2);
                sameBase.m_BoltNum = ClsRevitUtil.CompareValues(compareBase.m_BoltNum, templateBase.m_BoltNum);
            }

            DlgCreateHiuchiTsunagizaiBase dlg = new DlgCreateHiuchiTsunagizaiBase(sameBase);
            DialogResult res = dlg.ShowDialog();
            if (res == DialogResult.OK)
            {
                ClsHiuchiTsunagizaiBase data = dlg.m_HiuchiTsunagizaiBase;
                
                foreach (DataGridViewRow row in dgvHiuchiTsunagi.SelectedRows)
                {
                    ClsHiuchiTsunagizaiBase originalBase = (ClsHiuchiTsunagizaiBase)dgvHiuchiTsunagi.Rows[row.Index].Tag;
                    originalBase.m_FlgEdit = true;
                    originalBase.m_SteelType = data.m_SteelType;
                    originalBase.m_SteelSize = data.m_SteelSize;
                    originalBase.m_ShoriType = data.m_ShoriType;
                    originalBase.m_SplitFlg = data.m_SplitFlg;
                    originalBase.m_ToritsukeHoho = data.m_ToritsukeHoho;
                    originalBase.m_ShoriType = data.m_ShoriType;
                    originalBase.m_BoltType1 = data.m_BoltType1;
                    originalBase.m_BoltType2 = data.m_BoltType2;
                    originalBase.m_BoltNum = data.m_BoltNum;

                    SetDataGridViewHiuchiTsunagizai(row.Index, originalBase);
                    ChangeColorChangeRows(row);
                }
            }

        }

        /// <summary>
        /// グリッドで選択しているベースをハイライト
        /// </summary>
        /// <param name="nIndex"></param>
        private void HighLight_HiuchiTsunagizai(int nIndex)
        {
            ClsHiuchiTsunagizaiBase data = (ClsHiuchiTsunagizaiBase)dgvHiuchiTsunagi.Rows[nIndex].Tag;
            m_HighLightId = data.m_ElementId;
        }
        #endregion

        #region 切梁継ぎ

        /// <summary>
        /// メンバ変数の値でグリッドの情報を更新(切梁継ぎ)
        /// </summary>
        private void SetDataGridViewKiribariTsugi()
        {
            DataGridView dgv = dgvKiribariTsugi;
            List<ClsKiribariTsugiBase> lstData = ListKiribariTsugiBase;

            for (int i = 0; i < lstData.Count; i++)
            {
                ClsKiribariTsugiBase data = lstData[i];
                dgv.Rows.Add();

                SetDataGridViewKiribariTsugi(i, data);
            }
            ChangeDGVColor(dgv);
        }

        /// <summary>
        /// メンバ変数の値でグリッドの行を更新(切梁継ぎ)
        /// </summary>
        /// <param name="nIndex"></param>
        /// <param name="data"></param>
        private void SetDataGridViewKiribariTsugi(int nIndex, ClsKiribariTsugiBase data)
        {
            DataGridView dgv = dgvKiribariTsugi;
            string str = string.Empty;
            dgv.Rows[nIndex].Tag = data;
            dgv[dgv.Columns.IndexOf(clmKTsugi_Floor), nIndex].Value = data.m_Floor;
            dgv[dgv.Columns.IndexOf(clmKTsugi_No), nIndex].Value = (nIndex + 1).ToString();
            //dgv[dgv.Columns.IndexOf(clmKTsugi_PartsType), nIndex].Value = (data.m_PartsType == ClsKiribariTsugiBase.PartsType.KiriKiri? "切梁-切梁" : "切梁-腹起"); ;
            dgv[dgv.Columns.IndexOf(clmKTsugi_SteelType), nIndex].Value = data.m_SteelType;
            dgv[dgv.Columns.IndexOf(clmKTsugi_Kosei), nIndex].Value = (data.m_Kousei == ClsKiribariTsugiBase.Kousei.Single ? "シングル" : "ダブル");
            dgv[dgv.Columns.IndexOf(clmKTsugi_SteelSizeSingle), nIndex].Value = data.m_SteelSizeSingle;
            dgv[dgv.Columns.IndexOf(clmKTsugi_KiriSideSteelSize), nIndex].Value = data.m_KiriSideSteelSizeDouble;
            dgv[dgv.Columns.IndexOf(clmKTsugi_KiriSideLength), nIndex].Value = data.m_KiriSideTsunagiLength.ToString();
            dgv[dgv.Columns.IndexOf(clmKTsugi_KiriSideParts), nIndex].Value = data.m_KiriSideParts;
            dgv[dgv.Columns.IndexOf(clmKTsugi_HaraSideSteelSize), nIndex].Value = data.m_HaraSideSteelSizeDouble;
            dgv[dgv.Columns.IndexOf(clmKTsugi_HaraSideLength), nIndex].Value = data.m_HaraSideTsunagiLength.ToString();
            dgv[dgv.Columns.IndexOf(clmKTsugi_HaraSideParts), nIndex].Value = data.m_HaraSideParts;
        }

        /// <summary>
        /// 編集ダイアログを呼び出し、情報を編集
        /// </summary>
        /// <param name="nIndex"></param>
        private void EditKiribariTsugi()
        {
            ClsKiribariTsugiBase templateBase = new ClsKiribariTsugiBase();
            ClsKiribariTsugiBase sameBase = new ClsKiribariTsugiBase();
            bool first = true;
            foreach (DataGridViewRow row in dgvKiribariTsugi.SelectedRows)
            {
                if (first)
                {
                    templateBase = (ClsKiribariTsugiBase)dgvKiribariTsugi.Rows[row.Index].Tag;
                    first = false;
                }
                ClsKiribariTsugiBase compareBase = (ClsKiribariTsugiBase)dgvKiribariTsugi.Rows[row.Index].Tag;
                sameBase.m_PartsType = ClsRevitUtil.CompareValues(compareBase.m_PartsType, templateBase.m_PartsType);
                sameBase.m_SteelType = templateBase.m_SteelType;// ClsRevitUtil.CompareValues(compareBase.m_SteelType, templateBase.m_SteelType);
                sameBase.m_ShoriType = ClsRevitUtil.CompareValues(compareBase.m_ShoriType, templateBase.m_ShoriType);
                sameBase.m_Kousei = templateBase.m_Kousei;// ClsRevitUtil.CompareValues(compareBase.m_Kousei, templateBase.m_Kousei);
                sameBase.m_SteelSizeSingle = templateBase.m_SteelSizeSingle;// ClsRevitUtil.CompareValues(compareBase.m_SteelSizeSingle, templateBase.m_SteelSizeSingle);
                sameBase.m_KiriSideSteelSizeDouble = templateBase.m_KiriSideSteelSizeDouble;// ClsRevitUtil.CompareValues(compareBase.m_KiriSideSteelSizeDouble, templateBase.m_KiriSideSteelSizeDouble);
                sameBase.m_KiriSideTsunagiLength = ClsRevitUtil.CompareValues(compareBase.m_KiriSideTsunagiLength, templateBase.m_KiriSideTsunagiLength);
                sameBase.m_KiriSideParts = templateBase.m_KiriSideParts;// ClsRevitUtil.CompareValues(compareBase.m_KiriSideParts, templateBase.m_KiriSideParts);
                sameBase.m_HaraSideSteelSizeDouble = templateBase.m_HaraSideSteelSizeDouble;// ClsRevitUtil.CompareValues(compareBase.m_HaraSideSteelSizeDouble, templateBase.m_HaraSideSteelSizeDouble);
                sameBase.m_HaraSideTsunagiLength = ClsRevitUtil.CompareValues(compareBase.m_HaraSideTsunagiLength, templateBase.m_HaraSideTsunagiLength);
                sameBase.m_HaraSideParts = templateBase.m_HaraSideParts;// ClsRevitUtil.CompareValues(compareBase.m_HaraSideParts, templateBase.m_HaraSideParts);

            }

            DlgCreateKiribariTsugiBase dlg = new DlgCreateKiribariTsugiBase(sameBase);
            DialogResult res = dlg.ShowDialog();
            if (res == DialogResult.OK)
            {
                ClsKiribariTsugiBase data = dlg.m_KiribariTsugiBase;
                
                foreach (DataGridViewRow row in dgvKiribariTsugi.SelectedRows)
                {
                    ClsKiribariTsugiBase originalBase = (ClsKiribariTsugiBase)dgvKiribariTsugi.Rows[row.Index].Tag;
                    originalBase.m_FlgEdit = true;
                    originalBase.m_PartsType = data.m_PartsType;
                    originalBase.m_SteelType = data.m_SteelType;
                    originalBase.m_ShoriType = data.m_ShoriType;
                    originalBase.m_Kousei = data.m_Kousei;
                    originalBase.m_SteelSizeSingle = data.m_SteelSizeSingle;
                    originalBase.m_ShoriType = data.m_ShoriType;
                    originalBase.m_KiriSideSteelSizeDouble = data.m_KiriSideSteelSizeDouble;
                    originalBase.m_KiriSideTsunagiLength = data.m_KiriSideTsunagiLength;
                    originalBase.m_KiriSideParts = data.m_KiriSideParts;
                    originalBase.m_HaraSideSteelSizeDouble = data.m_HaraSideSteelSizeDouble;
                    originalBase.m_HaraSideTsunagiLength = data.m_HaraSideTsunagiLength;
                    originalBase.m_HaraSideParts = data.m_HaraSideParts;
                    SetDataGridViewKiribariTsugi(row.Index, originalBase);
                    ChangeColorChangeRows(row);
                }
            }

        }

        /// <summary>
        /// グリッドで選択しているベースをハイライト
        /// </summary>
        /// <param name="nIndex"></param>
        private void HighLight_KiribariTsugi(int nIndex)
        {
            ClsKiribariTsugiBase data = (ClsKiribariTsugiBase)dgvKiribariTsugi.Rows[nIndex].Tag;
            m_HighLightId = data.m_ElementId;
        }
        #endregion

        #region 斜梁

        /// <summary>
        /// メンバ変数の値でグリッドの情報を更新(斜梁)
        /// </summary>
        private void SetDataGridViewSyabari()
        {
            var dgv = dgvSyabari;
            var lstData = ListSyabariBase;

            for (int i = 0; i < lstData.Count; i++)
            {
                var data = lstData[i];
                dgv.Rows.Add();

                SetDataGridViewSyabari(i, data);
            }
            ChangeDGVColor(dgv);
        }

        /// <summary>
        /// メンバ変数の値でグリッドの行を更新(斜梁)
        /// </summary>
        /// <param name="nIndex"></param>
        /// <param name="data"></param>
        private void SetDataGridViewSyabari(int nIndex, ClsSyabariBase data)
        {
            var dgv = dgvSyabari;
            string str = string.Empty;
            dgv.Rows[nIndex].Tag = data;
            dgv[dgv.Columns.IndexOf(clmS_Floor), nIndex].Value = data.m_Floor;
            dgv[dgv.Columns.IndexOf(clmS_No), nIndex].Value = (nIndex + 1).ToString();
            dgv[dgv.Columns.IndexOf(clmS_SteelSize), nIndex].Value = data.m_kouzaiSize;
            dgv[dgv.Columns.IndexOf(clmS_TanbuPartsS), nIndex].Value = data.m_buhinTypeStart;
            dgv[dgv.Columns.IndexOf(clmS_TanbuPartsSizeS), nIndex].Value = data.m_buhinSizeStart;
            dgv[dgv.Columns.IndexOf(clmS_TanbuPartsE), nIndex].Value = data.m_buhinTypeEnd;
            dgv[dgv.Columns.IndexOf(clmS_TanbuPartsSizeE), nIndex].Value = data.m_buhinSizeEnd;
            dgv[dgv.Columns.IndexOf(clmS_jackType1), nIndex].Value = data.m_jack1;
            dgv[dgv.Columns.IndexOf(clmS_jackType2), nIndex].Value = data.m_jack2;
        }

        /// <summary>
        /// 編集ダイアログを呼び出し、情報を編集
        /// </summary>
        /// <param name="nIndex"></param>
        private void EditSyabari()
        {
            var templateBase = new ClsSyabariBase();
            var sameBase = new ClsSyabariBase();
            foreach (DataGridViewRow row in dgvSyabari.SelectedRows)
            {
                templateBase = (ClsSyabariBase)dgvSyabari.Rows[row.Index].Tag;
                sameBase.m_kouzaiType = templateBase.m_kouzaiType;
                sameBase.m_kouzaiSize = templateBase.m_kouzaiSize;
                sameBase.m_buhinTypeStart = templateBase.m_buhinTypeStart;
                sameBase.m_buhinSizeStart = templateBase.m_buhinSizeStart;
                sameBase.m_buhinTypeEnd = templateBase.m_buhinTypeEnd;
                sameBase.m_buhinSizeEnd = templateBase.m_buhinSizeEnd;
                sameBase.m_jack1 = templateBase.m_jack1;
                sameBase.m_jack2 = templateBase.m_jack2;
            }
            
            var dlg = new DlgCreateSyabariBase(sameBase);
            DialogResult res = dlg.ShowDialog();
            if (res == DialogResult.OK)
            {
                var data = dlg.m_ClsSyabariBase;
                
                foreach (DataGridViewRow row in dgvSyabari.SelectedRows)
                {
                    var originalBase = (ClsSyabariBase)dgvSyabari.Rows[row.Index].Tag;
                    originalBase.m_FlgEdit = true;
                    originalBase.m_kouzaiType = data.m_kouzaiType;
                    originalBase.m_kouzaiSize = data.m_kouzaiSize;
                    originalBase.m_buhinTypeStart = data.m_buhinTypeStart;
                    originalBase.m_buhinSizeStart = data.m_buhinSizeStart;
                    originalBase.m_buhinTypeEnd = data.m_buhinTypeEnd;
                    originalBase.m_buhinSizeEnd = data.m_buhinSizeEnd;
                    originalBase.m_jack1 = data.m_jack1;
                    originalBase.m_jack2 = data.m_jack2;
                    SetDataGridViewSyabari(row.Index, originalBase);
                    ChangeColorChangeRows(row);
                }
            }

        }
        /// <summary>
        /// グリッドで選択しているベースをハイライト
        /// </summary>
        /// <param name="nIndex"></param>
        private void HighLight_Syabari(int nIndex)
        {
            var data = (ClsSyabariBase)dgvSyabari.Rows[nIndex].Tag;
            m_HighLightId = data.m_ElementId;
        }
        #endregion

        #region 斜梁繋ぎ材

        /// <summary>
        /// メンバ変数の値でグリッドの情報を更新(斜梁繋ぎ材)
        /// </summary>
        private void SetDataGridViewSyabariTsunagizai()
        {
            var dgv = dgvSyabariTsunagizai;
            var lstData = ListSyabariTsunagizaiBase;

            for (int i = 0; i < lstData.Count; i++)
            {
                var data = lstData[i];
                dgv.Rows.Add();

                SetDataGridViewSyabariTsunagizai(i, data);
            }
            ChangeDGVColor(dgv);
        }

        /// <summary>
        /// メンバ変数の値でグリッドの行を更新(斜梁繋ぎ材)
        /// </summary>
        /// <param name="nIndex"></param>
        /// <param name="data"></param>
        private void SetDataGridViewSyabariTsunagizai(int nIndex, ClsSyabariTsunagizaiBase data)
        {
            var dgv = dgvSyabariTsunagizai;
            string str = string.Empty;
            dgv.Rows[nIndex].Tag = data;
            dgv[dgv.Columns.IndexOf(clmST_Floor), nIndex].Value = data.m_Floor;
            dgv[dgv.Columns.IndexOf(clmST_No), nIndex].Value = (nIndex + 1).ToString();
            dgv[dgv.Columns.IndexOf(clmST_SteelType), nIndex].Value = data.m_SteelType;
            dgv[dgv.Columns.IndexOf(clmST_SteelSize), nIndex].Value = data.m_SteelSize;
        }

        /// <summary>
        /// 編集ダイアログを呼び出し、情報を編集
        /// </summary>
        /// <param name="nIndex"></param>
        private void EditSyabariTsunagizai()
        {
            var templateBase = new ClsSyabariTsunagizaiBase();
            var sameBase = new ClsSyabariTsunagizaiBase();
            foreach (DataGridViewRow row in dgvSyabariTsunagizai.SelectedRows)
            {
                templateBase = (ClsSyabariTsunagizaiBase)dgvSyabariTsunagizai.Rows[row.Index].Tag;
                sameBase.m_SteelType = templateBase.m_SteelType;
                sameBase.m_SteelSize = templateBase.m_SteelSize;
            }

            var dlg = new DlgCreateSyabariTsunagizaiBase(templateBase);
            DialogResult res = dlg.ShowDialog();
            if (res == DialogResult.OK)
            {
                var data = dlg.m_SyabariTsunagizaiBase;
                

                foreach (DataGridViewRow row in dgvSyabariTsunagizai.SelectedRows)
                {
                    var originalBase = (ClsSyabariTsunagizaiBase)dgvSyabariTsunagizai.Rows[row.Index].Tag;
                    originalBase.m_FlgEdit = true;
                    originalBase.m_SteelType = data.m_SteelType;
                    originalBase.m_SteelSize = data.m_SteelSize;
                    SetDataGridViewSyabariTsunagizai(row.Index, originalBase);
                    ChangeColorChangeRows(row);
                }
            }

        }
        /// <summary>
        /// グリッドで選択しているベースをハイライト
        /// </summary>
        /// <param name="nIndex"></param>
        private void HighLight_SyabariTsunagizai(int nIndex)
        {
            var data = (ClsSyabariTsunagizaiBase)dgvSyabariTsunagizai.Rows[nIndex].Tag;
            m_HighLightId = data.m_ElementId;
        }
        #endregion

        #region 斜梁受け材

        /// <summary>
        /// メンバ変数の値でグリッドの情報を更新(斜梁受け材)
        /// </summary>
        private void SetDataGridViewSyabariUke()
        {
            var dgv = dgvSyabariUke;
            var lstData = ListSyabariUkeBase;

            for (int i = 0; i < lstData.Count; i++)
            {
                var data = lstData[i];
                dgv.Rows.Add();

                SetDataGridViewSyabariUke(i, data);
            }
            ChangeDGVColor(dgv);
        }

        /// <summary>
        /// メンバ変数の値でグリッドの行を更新(斜梁受け材)
        /// </summary>
        /// <param name="nIndex"></param>
        /// <param name="data"></param>
        private void SetDataGridViewSyabariUke(int nIndex, ClsSyabariUkeBase data)
        {
            var dgv = dgvSyabariUke;
            string str = string.Empty;
            dgv.Rows[nIndex].Tag = data;
            dgv[dgv.Columns.IndexOf(clmSU_Floor), nIndex].Value = data.m_Floor;
            dgv[dgv.Columns.IndexOf(clmSU_No), nIndex].Value = (nIndex + 1).ToString();
            dgv[dgv.Columns.IndexOf(clmSU_SteeelType), nIndex].Value = data.m_SteelType;
            dgv[dgv.Columns.IndexOf(clmSU_SteeelSize), nIndex].Value = data.m_SteelSize;
            dgv[dgv.Columns.IndexOf(clmSU_TsukidashiRyoS), nIndex].Value = data.m_TsukidashiRyoS;
            dgv[dgv.Columns.IndexOf(clmSU_TsukidashiRyoE), nIndex].Value = data.m_TsukidashiRyoE;
        }

        /// <summary>
        /// 編集ダイアログを呼び出し、情報を編集
        /// </summary>
        /// <param name="nIndex"></param>
        private void EditSyabariUke()
        {
            var templateBase = new ClsSyabariUkeBase();
            var sameBase = new ClsSyabariUkeBase();
            foreach (DataGridViewRow row in dgvSyabariUke.SelectedRows)
            {
                templateBase = (ClsSyabariUkeBase)dgvSyabariUke.Rows[row.Index].Tag;
                sameBase.m_SteelSize = templateBase.m_SteelSize;
                sameBase.m_SteelType = templateBase.m_SteelType;
                sameBase.m_TsukidashiRyoS = templateBase.m_TsukidashiRyoS;
                sameBase.m_TsukidashiRyoE = templateBase.m_TsukidashiRyoE;
            }

            var dlg = new DlgCreateSyabariUkeBase(templateBase);
            DialogResult res = dlg.ShowDialog();
            if (res == DialogResult.OK)
            {
                var data = dlg.m_SyabariUkeBase;
                

                foreach (DataGridViewRow row in dgvSyabariUke.SelectedRows)
                {
                    var originalBase = (ClsSyabariUkeBase)dgvSyabariUke.Rows[row.Index].Tag;
                    originalBase.m_FlgEdit = true;
                    originalBase.m_SteelSize = data.m_SteelSize;
                    originalBase.m_SteelType = data.m_SteelType;
                    originalBase.m_TsukidashiRyoS = data.m_TsukidashiRyoS;
                    originalBase.m_TsukidashiRyoE = data.m_TsukidashiRyoE;
                    SetDataGridViewSyabariUke(row.Index, originalBase);
                    ChangeColorChangeRows(row);
                }
            }

        }
        /// <summary>
        /// グリッドで選択しているベースをハイライト
        /// </summary>
        /// <param name="nIndex"></param>
        private void HighLight_SyabariUke(int nIndex)
        {
            var data = (ClsSyabariUkeBase)dgvSyabariUke.Rows[nIndex].Tag;
            m_HighLightId = data.m_ElementId;
        }
        #endregion

        /// <summary>
        /// 内部に保持しているBaseListを更新
        /// </summary>
        private void UpdateBaseList()
        {
            List<ClsHaraokoshiBase> lstH = new List<ClsHaraokoshiBase>();
            foreach (DataGridViewRow row in dgvHaraokoshi.Rows)
            {
                lstH.Add((ClsHaraokoshiBase)row.Tag);
            }
            ListHaraokoshiBase = lstH;

            List<ClsKiribariBase> lstK = new List<ClsKiribariBase>();
            foreach (DataGridViewRow row in dgvKiribari.Rows)
            {
                lstK.Add((ClsKiribariBase)row.Tag);
            }
            ListKiribariBase = lstK;

            List<ClsCornerHiuchiBase> lstCH = new List<ClsCornerHiuchiBase>();
            foreach (DataGridViewRow row in dgvCornerHiuchi.Rows)
            {
                lstCH.Add((ClsCornerHiuchiBase)row.Tag);
            }
            ListCornerHiuchiBase = lstCH;

            List<ClsKiribariHiuchiBase> lstKH = new List<ClsKiribariHiuchiBase>();
            foreach (DataGridViewRow row in dgvKiribariHiuchi.Rows)
            {
                lstKH.Add((ClsKiribariHiuchiBase)row.Tag);
            }
            ListKiribariHiuchiBase = lstKH;

            List<ClsKiribariUkeBase> lstKU = new List<ClsKiribariUkeBase>();
            foreach (DataGridViewRow row in dgvKiribariUke.Rows)
            {
                lstKU.Add((ClsKiribariUkeBase)row.Tag);
            }
            ListKiribariUkeBase = lstKU;

            List<ClsKiribariTsunagizaiBase> lstKT = new List<ClsKiribariTsunagizaiBase>();
            foreach (DataGridViewRow row in dgvKiribariTsunagi.Rows)
            {
                lstKT.Add((ClsKiribariTsunagizaiBase)row.Tag);
            }
            ListKiribariTsunagizaiBase = lstKT;

            List<ClsHiuchiTsunagizaiBase> lstHT = new List<ClsHiuchiTsunagizaiBase>();
            foreach (DataGridViewRow row in dgvHiuchiTsunagi.Rows)
            {
                lstHT.Add((ClsHiuchiTsunagizaiBase)row.Tag);
            }
            ListHiuchiTsunagizaiBase = lstHT;

            List<ClsKiribariTsugiBase> lstKTsugi = new List<ClsKiribariTsugiBase>();
            foreach (DataGridViewRow row in dgvKiribariTsugi.Rows)
            {
                lstKTsugi.Add((ClsKiribariTsugiBase)row.Tag);
            }
            ListKiribariTsugiBase = lstKTsugi;

            var lstS = new List<ClsSyabariBase>();
            foreach (DataGridViewRow row in dgvSyabari.Rows)
            {
                lstS.Add((ClsSyabariBase)row.Tag);
            }
            ListSyabariBase = lstS;

            var lstST = new List<ClsSyabariTsunagizaiBase>();
            foreach (DataGridViewRow row in dgvSyabariTsunagizai.Rows)
            {
                lstST.Add((ClsSyabariTsunagizaiBase)row.Tag);
            }
            ListSyabariTsunagizaiBase = lstST;

            var lstSU = new List<ClsSyabariUkeBase>();
            foreach (DataGridViewRow row in dgvSyabariUke.Rows)
            {
                lstSU.Add((ClsSyabariUkeBase)row.Tag);
            }
            ListSyabariUkeBase = lstSU;
        }

        /// <summary>
        /// 選択されているベースの行を選択状態にする
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id">選択されているベースId</param>
        public void SelectBaseRow(Document doc, ElementId id)
        {
            m_doc = doc;
            FamilyInstance inst = m_doc.GetElement(id) as FamilyInstance;
            if (inst == null) return;
            string baseName = inst.Name;
            DataGridView dgv;
            switch (baseName)
            {
                case ClsHaraokoshiBase.baseName:
                    {
                        for (int i = 0; i < ListHaraokoshiBase.Count; i++)
                        {
                            if (ListHaraokoshiBase[i].m_ElementId == id)
                            {
                                dgv = dgvHaraokoshi;
                                dgv.Rows[i].Selected = true;
                                TabBaseList.SelectedTab = tabHaraokoshi;
                                break;
                            }
                        }
                        break;
                    }
                case ClsKiribariBase.baseName:
                    {
                        for (int i = 0; i < ListKiribariBase.Count; i++)
                        {
                            if (ListKiribariBase[i].m_ElementId == id)
                            {
                                dgv = dgvKiribari;
                                dgv.Rows[i].Selected = true;
                                TabBaseList.SelectedTab = tabKiribari;
                                break;
                            }
                        }
                        break;
                    }
                case ClsCornerHiuchiBase.baseName:
                    {
                        for (int i = 0; i < ListCornerHiuchiBase.Count; i++)
                        {
                            if (ListCornerHiuchiBase[i].m_cornerHiuchiID == id)
                            {
                                dgv = dgvCornerHiuchi;
                                dgv.Rows[i].Selected = true;
                                TabBaseList.SelectedTab = tabCornerHiuchi;
                                break;
                            }
                        }
                        break;
                    }
                case ClsKiribariHiuchiBase.baseName:
                    {
                        for (int i = 0; i < ListKiribariHiuchiBase.Count; i++)
                        {
                            if (ListKiribariHiuchiBase[i].m_ElementId == id)
                            {
                                dgv = dgvKiribariHiuchi;
                                dgv.Rows[i].Selected = true;
                                TabBaseList.SelectedTab = tabKiribariHiuchi;
                                break;
                            }
                        }
                        break;
                    }
                case ClsKiribariUkeBase.baseName:
                    {
                        for (int i = 0; i < ListKiribariUkeBase.Count; i++)
                        {
                            if (ListKiribariUkeBase[i].m_ElementId == id)
                            {
                                dgv = dgvKiribariUke;
                                dgv.Rows[i].Selected = true;
                                TabBaseList.SelectedTab = tabKiribariUke;
                                break;
                            }
                        }
                        break;
                    }
                case ClsKiribariTsunagizaiBase.baseName:
                    {
                        for (int i = 0; i < ListKiribariTsunagizaiBase.Count; i++)
                        {
                            if (ListKiribariTsunagizaiBase[i].m_ElementId == id)
                            {
                                dgv = dgvKiribariTsunagi;
                                dgv.Rows[i].Selected = true;
                                TabBaseList.SelectedTab = tabKiribariTsunagizai;
                                break;
                            }
                        }
                        break;
                    }
                case ClsHiuchiTsunagizaiBase.baseName:
                    {
                        for (int i = 0; i < ListHiuchiTsunagizaiBase.Count; i++)
                        {
                            if (ListHiuchiTsunagizaiBase[i].m_ElementId == id)
                            {
                                dgv = dgvHiuchiTsunagi;
                                dgv.Rows[i].Selected = true;
                                TabBaseList.SelectedTab = tabHiuchiTsunagizai;
                                break;
                            }
                        }
                        break;
                    }
                case ClsKiribariTsugiBase.baseName:
                    {
                        for (int i = 0; i < ListKiribariTsugiBase.Count; i++)
                        {
                            if (ListKiribariTsugiBase[i].m_ElementId == id)
                            {
                                dgv = dgvKiribariTsugi;
                                dgv.Rows[i].Selected = true;
                                TabBaseList.SelectedTab = tabKiribariTsugi;
                                break;
                            }
                        }
                        break;
                    }
                case ClsSyabariBase.baseName:
                    {
                        for (int i = 0; i < ListSyabariBase.Count; i++)
                        {
                            if (ListSyabariBase[i].m_ElementId == id)
                            {
                                dgv = dgvSyabari;
                                dgv.Rows[i].Selected = true;
                                TabBaseList.SelectedTab = tabSyabari;
                                break;
                            }
                        }
                        break;
                    }
                case ClsSyabariTsunagizaiBase.baseName:
                    {
                        for (int i = 0; i < ListSyabariTsunagizaiBase.Count; i++)
                        {
                            if (ListSyabariTsunagizaiBase[i].m_ElementId == id)
                            {
                                dgv = dgvSyabariTsunagizai;
                                dgv.Rows[i].Selected = true;
                                TabBaseList.SelectedTab = tabSyabariTsunagizai;
                                break;
                            }
                        }
                        break;
                    }
                case ClsSyabariUkeBase.baseName:
                    {
                        for (int i = 0; i < ListSyabariUkeBase.Count; i++)
                        {
                            if (ListSyabariUkeBase[i].m_ElementId == id)
                            {
                                dgv = dgvSyabariUke;
                                dgv.Rows[i].Selected = true;
                                TabBaseList.SelectedTab = tabSyabariUke;
                                break;
                            }
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            m_HighLightId = id;
        }

        /// <summary>
        /// DGVで選択されている行のHLできる準備を行う
        /// </summary>
        /// <param name="tabName"></param>
        public void SetDGVSelectedRow(string tabName)
        {
            DataGridView dgv;
            int index;
            switch (tabName)
            {
                case "腹起":
                    {
                        dgv = dgvHaraokoshi;
                        if (dgv.SelectedRows.Count < 1)
                        {
                            m_HighLightId = null;
                            break;
                        }
                        index = dgv.SelectedRows[0].Index;
                        HighLight_Haraokoshi(index);
                        break;
                    }
                case "切梁":
                    {
                        dgv = dgvKiribari;
                        if (dgv.SelectedRows.Count < 1)
                        {
                            m_HighLightId = null;
                            break;
                        }
                        index = dgv.SelectedRows[0].Index;
                        HighLight_Kiribari(index);
                        break;
                    }
                case "隅火打":
                    {
                        dgv = dgvCornerHiuchi;
                        if (dgv.SelectedRows.Count < 1)
                        {
                            m_HighLightId = null;
                            break;
                        }
                        index = dgv.SelectedRows[0].Index;
                        HighLight_CornerHiuchi(index);
                        break;
                    }
                case "切梁火打":
                    {
                        dgv = dgvKiribariHiuchi;
                        if (dgv.SelectedRows.Count < 1)
                        {
                            m_HighLightId = null;
                            break;
                        }
                        index = dgv.SelectedRows[0].Index;
                        HighLight_KiribariHiuchi(index);
                        break;
                    }
                case "切梁受け":
                    {
                        dgv = dgvKiribariUke;
                        if (dgv.SelectedRows.Count < 1)
                        {
                            m_HighLightId = null;
                            break;
                        }
                        index = dgv.SelectedRows[0].Index;
                        HighLight_KiribariUke(index);
                        break;
                    }
                case "切梁繋ぎ材":
                    {
                        dgv = dgvKiribariTsunagi;
                        if (dgv.SelectedRows.Count < 1)
                        {
                            m_HighLightId = null;
                            break;
                        }
                        index = dgv.SelectedRows[0].Index;
                        HighLight_KiribariTsunagizai(index);
                        break;
                    }
                case "火打繋ぎ材":
                    {
                        dgv = dgvHiuchiTsunagi;
                        if (dgv.SelectedRows.Count < 1)
                        {
                            m_HighLightId = null;
                            break;
                        }
                        index = dgv.SelectedRows[0].Index;
                        HighLight_HiuchiTsunagizai(index);
                        break;
                    }
                case "切梁継ぎ":
                    {
                        dgv = dgvKiribariTsugi;
                        if (dgv.SelectedRows.Count < 1)
                        {
                            m_HighLightId = null;
                            break;
                        }
                        index = dgv.SelectedRows[0].Index;
                        HighLight_KiribariTsugi(index);
                        break;
                    }
                case "斜梁":
                    {
                        dgv = dgvSyabari;
                        if (dgv.SelectedRows.Count < 1)
                        {
                            m_HighLightId = null;
                            break;
                        }
                        index = dgv.SelectedRows[0].Index;
                        HighLight_Syabari(index);
                        break;
                    }
                case "斜梁繋ぎ材":
                    {
                        dgv = dgvSyabariTsunagizai;
                        if (dgv.SelectedRows.Count < 1)
                        {
                            m_HighLightId = null;
                            break;
                        }
                        index = dgv.SelectedRows[0].Index;
                        HighLight_SyabariTsunagizai(index);
                        break;
                    }
                case "斜梁受け材":
                    {
                        dgv = dgvSyabariUke;
                        if (dgv.SelectedRows.Count < 1)
                        {
                            m_HighLightId = null;
                            break;
                        }
                        index = dgv.SelectedRows[0].Index;
                        HighLight_SyabariUke(index);
                        break;
                    }
            }
        }

        private void ChangeDGVColor(DataGridView dgv)
        {
            dgv.RowsDefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
        }

        private void ChangeColorChangeRows(DataGridViewRow row)
        {
            row.DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
        }

        #endregion

        #region Formプロパティ
        // CreateParams プロパティをオーバーライドする
        protected override System.Windows.Forms.CreateParams CreateParams
        {
            get
            {
                const int CS_NOCLOSE = 0x200;

                System.Windows.Forms.CreateParams createParams = base.CreateParams;
                createParams.ClassStyle |= CS_NOCLOSE;

                return createParams;
            }
        }


        #endregion

        
    }
}
