
namespace YMS_gantry.UI
{
    partial class FrmSonotaKanshouCheck
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.DgvKanshouCheckList = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.選択ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.解除ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.LstKoudai = new System.Windows.Forms.ListBox();
            this.CmbShouBunrui = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.CmbChuuBunrui = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.CmbDaiBunrui = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.BtnUnSelectedAll = new System.Windows.Forms.Button();
            this.BtnSelectedAll = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.BtnClose = new System.Windows.Forms.Button();
            this.DgvBunruiList = new System.Windows.Forms.DataGridView();
            this.BtnKanshouCheck = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel9 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.ColDaiBunrui = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColChuuBunrui = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColShouBunrui = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColKenshouCheckListID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColKenshouCheckListSelected = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ColKenshouCheckListKoudai = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColKenshouCheckListDaiBunrui = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColKenshouCheckListChuuBunrui = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColKenshouCheckListShouBunrui = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColKenshouCheckListFamilyName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.DgvKanshouCheckList)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DgvBunruiList)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel9.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // DgvKanshouCheckList
            // 
            this.DgvKanshouCheckList.AllowUserToAddRows = false;
            this.DgvKanshouCheckList.AllowUserToDeleteRows = false;
            this.DgvKanshouCheckList.AllowUserToResizeColumns = false;
            this.DgvKanshouCheckList.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DgvKanshouCheckList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.DgvKanshouCheckList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvKanshouCheckList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColKenshouCheckListID,
            this.ColKenshouCheckListSelected,
            this.ColKenshouCheckListKoudai,
            this.ColKenshouCheckListDaiBunrui,
            this.ColKenshouCheckListChuuBunrui,
            this.ColKenshouCheckListShouBunrui,
            this.ColKenshouCheckListFamilyName});
            this.DgvKanshouCheckList.ContextMenuStrip = this.contextMenuStrip1;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DgvKanshouCheckList.DefaultCellStyle = dataGridViewCellStyle2;
            this.DgvKanshouCheckList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DgvKanshouCheckList.Location = new System.Drawing.Point(12, 0);
            this.DgvKanshouCheckList.Name = "DgvKanshouCheckList";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DgvKanshouCheckList.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.DgvKanshouCheckList.RowHeadersVisible = false;
            this.DgvKanshouCheckList.RowTemplate.Height = 21;
            this.DgvKanshouCheckList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DgvKanshouCheckList.Size = new System.Drawing.Size(741, 262);
            this.DgvKanshouCheckList.TabIndex = 1;
            this.DgvKanshouCheckList.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvKanshouCheckList_CellValueChanged);
            this.DgvKanshouCheckList.CurrentCellDirtyStateChanged += new System.EventHandler(this.DgvKanshouCheckList_CurrentCellDirtyStateChanged);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.選択ToolStripMenuItem,
            this.解除ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(99, 48);
            // 
            // 選択ToolStripMenuItem
            // 
            this.選択ToolStripMenuItem.Name = "選択ToolStripMenuItem";
            this.選択ToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.選択ToolStripMenuItem.Text = "選択";
            this.選択ToolStripMenuItem.Click += new System.EventHandler(this.選択ToolStripMenuItem_Click);
            // 
            // 解除ToolStripMenuItem
            // 
            this.解除ToolStripMenuItem.Name = "解除ToolStripMenuItem";
            this.解除ToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.解除ToolStripMenuItem.Text = "解除";
            this.解除ToolStripMenuItem.Click += new System.EventHandler(this.解除ToolStripMenuItem_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.LstKoudai);
            this.groupBox1.Controls.Add(this.CmbShouBunrui);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.CmbChuuBunrui);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.CmbDaiBunrui);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Location = new System.Drawing.Point(12, 137);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(741, 104);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "絞り込み";
            // 
            // LstKoudai
            // 
            this.LstKoudai.FormattingEnabled = true;
            this.LstKoudai.ItemHeight = 12;
            this.LstKoudai.Location = new System.Drawing.Point(41, 18);
            this.LstKoudai.Name = "LstKoudai";
            this.LstKoudai.ScrollAlwaysVisible = true;
            this.LstKoudai.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.LstKoudai.Size = new System.Drawing.Size(380, 76);
            this.LstKoudai.TabIndex = 1;
            this.LstKoudai.SelectedIndexChanged += new System.EventHandler(this.LstKoudai_SelectedIndexChanged);
            // 
            // CmbShouBunrui
            // 
            this.CmbShouBunrui.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbShouBunrui.FormattingEnabled = true;
            this.CmbShouBunrui.Location = new System.Drawing.Point(511, 73);
            this.CmbShouBunrui.Name = "CmbShouBunrui";
            this.CmbShouBunrui.Size = new System.Drawing.Size(170, 20);
            this.CmbShouBunrui.TabIndex = 7;
            this.CmbShouBunrui.SelectedIndexChanged += new System.EventHandler(this.CmbShouBunrui_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(464, 76);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 12);
            this.label6.TabIndex = 6;
            this.label6.Text = "小分類";
            // 
            // CmbChuuBunrui
            // 
            this.CmbChuuBunrui.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbChuuBunrui.FormattingEnabled = true;
            this.CmbChuuBunrui.Location = new System.Drawing.Point(511, 47);
            this.CmbChuuBunrui.Name = "CmbChuuBunrui";
            this.CmbChuuBunrui.Size = new System.Drawing.Size(170, 20);
            this.CmbChuuBunrui.TabIndex = 5;
            this.CmbChuuBunrui.SelectedIndexChanged += new System.EventHandler(this.CmbChuuBunrui_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(464, 50);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 12);
            this.label7.TabIndex = 4;
            this.label7.Text = "中分類";
            // 
            // CmbDaiBunrui
            // 
            this.CmbDaiBunrui.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbDaiBunrui.FormattingEnabled = true;
            this.CmbDaiBunrui.Location = new System.Drawing.Point(511, 21);
            this.CmbDaiBunrui.Name = "CmbDaiBunrui";
            this.CmbDaiBunrui.Size = new System.Drawing.Size(170, 20);
            this.CmbDaiBunrui.TabIndex = 3;
            this.CmbDaiBunrui.SelectedIndexChanged += new System.EventHandler(this.CmbDaiBunrui_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(464, 24);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(41, 12);
            this.label8.TabIndex = 2;
            this.label8.Text = "大分類";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 21);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(29, 12);
            this.label9.TabIndex = 0;
            this.label9.Text = "構台";
            // 
            // BtnUnSelectedAll
            // 
            this.BtnUnSelectedAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnUnSelectedAll.Location = new System.Drawing.Point(678, 8);
            this.BtnUnSelectedAll.Name = "BtnUnSelectedAll";
            this.BtnUnSelectedAll.Size = new System.Drawing.Size(75, 23);
            this.BtnUnSelectedAll.TabIndex = 2;
            this.BtnUnSelectedAll.Text = "一括解除";
            this.BtnUnSelectedAll.UseVisualStyleBackColor = true;
            this.BtnUnSelectedAll.Click += new System.EventHandler(this.BtnUnSelectedAll_Click);
            // 
            // BtnSelectedAll
            // 
            this.BtnSelectedAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnSelectedAll.Location = new System.Drawing.Point(597, 9);
            this.BtnSelectedAll.Name = "BtnSelectedAll";
            this.BtnSelectedAll.Size = new System.Drawing.Size(75, 23);
            this.BtnSelectedAll.TabIndex = 1;
            this.BtnSelectedAll.Text = "一括選択";
            this.BtnSelectedAll.UseVisualStyleBackColor = true;
            this.BtnSelectedAll.Click += new System.EventHandler(this.BtnSelectedAll_Click);
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label10.Location = new System.Drawing.Point(15, 15);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(285, 12);
            this.label10.TabIndex = 0;
            this.label10.Text = "対象部材に対して以下の部材の干渉を検出しました。";
            // 
            // BtnClose
            // 
            this.BtnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnClose.Location = new System.Drawing.Point(678, 5);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(75, 23);
            this.BtnClose.TabIndex = 0;
            this.BtnClose.Text = "閉じる";
            this.BtnClose.UseVisualStyleBackColor = true;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // DgvBunruiList
            // 
            this.DgvBunruiList.AllowUserToAddRows = false;
            this.DgvBunruiList.AllowUserToDeleteRows = false;
            this.DgvBunruiList.AllowUserToResizeColumns = false;
            this.DgvBunruiList.AllowUserToResizeRows = false;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DgvBunruiList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.DgvBunruiList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvBunruiList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColDaiBunrui,
            this.ColChuuBunrui,
            this.ColShouBunrui});
            this.DgvBunruiList.ContextMenuStrip = this.contextMenuStrip1;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DgvBunruiList.DefaultCellStyle = dataGridViewCellStyle5;
            this.DgvBunruiList.Location = new System.Drawing.Point(12, 12);
            this.DgvBunruiList.Name = "DgvBunruiList";
            this.DgvBunruiList.ReadOnly = true;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DgvBunruiList.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.DgvBunruiList.RowHeadersVisible = false;
            this.DgvBunruiList.RowTemplate.Height = 21;
            this.DgvBunruiList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DgvBunruiList.Size = new System.Drawing.Size(475, 115);
            this.DgvBunruiList.TabIndex = 0;
            // 
            // BtnKanshouCheck
            // 
            this.BtnKanshouCheck.Location = new System.Drawing.Point(493, 12);
            this.BtnKanshouCheck.Name = "BtnKanshouCheck";
            this.BtnKanshouCheck.Size = new System.Drawing.Size(97, 23);
            this.BtnKanshouCheck.TabIndex = 1;
            this.BtnKanshouCheck.Text = "干渉チェック";
            this.BtnKanshouCheck.UseVisualStyleBackColor = true;
            this.BtnKanshouCheck.Click += new System.EventHandler(this.BtnKanshouCheck_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(765, 579);
            this.panel1.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.panel5);
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 244);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(765, 335);
            this.panel3.TabIndex = 0;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.panel9);
            this.panel5.Controls.Add(this.panel6);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 38);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(765, 297);
            this.panel5.TabIndex = 0;
            // 
            // panel9
            // 
            this.panel9.Controls.Add(this.DgvKanshouCheckList);
            this.panel9.Controls.Add(this.panel7);
            this.panel9.Controls.Add(this.panel8);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel9.Location = new System.Drawing.Point(0, 0);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(765, 262);
            this.panel9.TabIndex = 0;
            // 
            // panel7
            // 
            this.panel7.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel7.Location = new System.Drawing.Point(0, 0);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(12, 262);
            this.panel7.TabIndex = 0;
            // 
            // panel8
            // 
            this.panel8.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel8.Location = new System.Drawing.Point(753, 0);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(12, 262);
            this.panel8.TabIndex = 2;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.BtnClose);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel6.Location = new System.Drawing.Point(0, 262);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(765, 35);
            this.panel6.TabIndex = 0;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.BtnUnSelectedAll);
            this.panel4.Controls.Add(this.BtnSelectedAll);
            this.panel4.Controls.Add(this.label10);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(765, 38);
            this.panel4.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.DgvBunruiList);
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Controls.Add(this.BtnKanshouCheck);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(765, 244);
            this.panel2.TabIndex = 0;
            // 
            // ColDaiBunrui
            // 
            this.ColDaiBunrui.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColDaiBunrui.HeaderText = "大分類";
            this.ColDaiBunrui.MinimumWidth = 150;
            this.ColDaiBunrui.Name = "ColDaiBunrui";
            this.ColDaiBunrui.ReadOnly = true;
            this.ColDaiBunrui.Width = 150;
            // 
            // ColChuuBunrui
            // 
            this.ColChuuBunrui.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColChuuBunrui.HeaderText = "中分類";
            this.ColChuuBunrui.MinimumWidth = 150;
            this.ColChuuBunrui.Name = "ColChuuBunrui";
            this.ColChuuBunrui.ReadOnly = true;
            this.ColChuuBunrui.Width = 150;
            // 
            // ColShouBunrui
            // 
            this.ColShouBunrui.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColShouBunrui.HeaderText = "小分類";
            this.ColShouBunrui.MinimumWidth = 150;
            this.ColShouBunrui.Name = "ColShouBunrui";
            this.ColShouBunrui.ReadOnly = true;
            // 
            // ColKenshouCheckListID
            // 
            this.ColKenshouCheckListID.HeaderText = "ID";
            this.ColKenshouCheckListID.Name = "ColKenshouCheckListID";
            this.ColKenshouCheckListID.ReadOnly = true;
            this.ColKenshouCheckListID.Visible = false;
            // 
            // ColKenshouCheckListSelected
            // 
            this.ColKenshouCheckListSelected.HeaderText = "";
            this.ColKenshouCheckListSelected.MinimumWidth = 30;
            this.ColKenshouCheckListSelected.Name = "ColKenshouCheckListSelected";
            this.ColKenshouCheckListSelected.Width = 30;
            // 
            // ColKenshouCheckListKoudai
            // 
            this.ColKenshouCheckListKoudai.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColKenshouCheckListKoudai.HeaderText = "構台";
            this.ColKenshouCheckListKoudai.MinimumWidth = 100;
            this.ColKenshouCheckListKoudai.Name = "ColKenshouCheckListKoudai";
            this.ColKenshouCheckListKoudai.ReadOnly = true;
            // 
            // ColKenshouCheckListDaiBunrui
            // 
            this.ColKenshouCheckListDaiBunrui.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColKenshouCheckListDaiBunrui.HeaderText = "大分類";
            this.ColKenshouCheckListDaiBunrui.MinimumWidth = 100;
            this.ColKenshouCheckListDaiBunrui.Name = "ColKenshouCheckListDaiBunrui";
            this.ColKenshouCheckListDaiBunrui.ReadOnly = true;
            // 
            // ColKenshouCheckListChuuBunrui
            // 
            this.ColKenshouCheckListChuuBunrui.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColKenshouCheckListChuuBunrui.HeaderText = "中分類";
            this.ColKenshouCheckListChuuBunrui.MinimumWidth = 100;
            this.ColKenshouCheckListChuuBunrui.Name = "ColKenshouCheckListChuuBunrui";
            this.ColKenshouCheckListChuuBunrui.ReadOnly = true;
            // 
            // ColKenshouCheckListShouBunrui
            // 
            this.ColKenshouCheckListShouBunrui.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColKenshouCheckListShouBunrui.HeaderText = "小分類";
            this.ColKenshouCheckListShouBunrui.MinimumWidth = 100;
            this.ColKenshouCheckListShouBunrui.Name = "ColKenshouCheckListShouBunrui";
            this.ColKenshouCheckListShouBunrui.ReadOnly = true;
            // 
            // ColKenshouCheckListFamilyName
            // 
            this.ColKenshouCheckListFamilyName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColKenshouCheckListFamilyName.HeaderText = "ファミリ名";
            this.ColKenshouCheckListFamilyName.MinimumWidth = 100;
            this.ColKenshouCheckListFamilyName.Name = "ColKenshouCheckListFamilyName";
            this.ColKenshouCheckListFamilyName.ReadOnly = true;
            // 
            // FrmSonotaKanshouCheck
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(765, 579);
            this.Controls.Add(this.panel1);
            this.MinimumSize = new System.Drawing.Size(781, 618);
            this.Name = "FrmSonotaKanshouCheck";
            this.ShowIcon = false;
            this.Text = "[その他] 干渉チェック";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmSonotaKanshouCheck_FormClosed);
            this.Load += new System.EventHandler(this.FrmSonotaKanshouCheck_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DgvKanshouCheckList)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DgvBunruiList)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel9.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox LstKoudai;
        private System.Windows.Forms.ComboBox CmbShouBunrui;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox CmbChuuBunrui;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox CmbDaiBunrui;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button BtnUnSelectedAll;
        private System.Windows.Forms.Button BtnSelectedAll;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 選択ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 解除ToolStripMenuItem;
        private System.Windows.Forms.Button BtnClose;
        private System.Windows.Forms.Button BtnKanshouCheck;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.DataGridView DgvKanshouCheckList;
        private System.Windows.Forms.DataGridView DgvBunruiList;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColKenshouCheckListID;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ColKenshouCheckListSelected;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColKenshouCheckListKoudai;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColKenshouCheckListDaiBunrui;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColKenshouCheckListChuuBunrui;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColKenshouCheckListShouBunrui;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColKenshouCheckListFamilyName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColDaiBunrui;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColChuuBunrui;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColShouBunrui;
    }
}