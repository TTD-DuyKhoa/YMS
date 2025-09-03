
namespace YMS_gantry.UI
{
    partial class FrmEditLengthChange
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.LstKoudai = new System.Windows.Forms.ListBox();
            this.CmbShouBunrui = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.CmbChuuBunrui = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.CmbDaiBunrui = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.DgvEditLengthChangeList = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.選択ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.解除oolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.BtmCancel = new System.Windows.Forms.Button();
            this.BtnOK = new System.Windows.Forms.Button();
            this.BtnSelectedAll = new System.Windows.Forms.Button();
            this.BtnUnSelectedAll = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.ChkLengthChangeIsBothEnd = new System.Windows.Forms.CheckBox();
            this.ColLengthChangeListID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColElementID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColLengthChangeListSelected = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColLengthChangeListFamilyName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColLengthChangeListBeforeLength = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColLengthChangeListAfterLength = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DgvEditLengthChangeList)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.LstKoudai);
            this.groupBox1.Controls.Add(this.CmbShouBunrui);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.CmbChuuBunrui);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.CmbDaiBunrui);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(650, 104);
            this.groupBox1.TabIndex = 0;
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
            this.LstKoudai.Size = new System.Drawing.Size(350, 76);
            this.LstKoudai.TabIndex = 1;
            this.LstKoudai.SelectedIndexChanged += new System.EventHandler(this.LstKoudai_SelectedIndexChanged);
            // 
            // CmbShouBunrui
            // 
            this.CmbShouBunrui.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbShouBunrui.FormattingEnabled = true;
            this.CmbShouBunrui.Location = new System.Drawing.Point(458, 72);
            this.CmbShouBunrui.Name = "CmbShouBunrui";
            this.CmbShouBunrui.Size = new System.Drawing.Size(180, 20);
            this.CmbShouBunrui.TabIndex = 7;
            this.CmbShouBunrui.SelectedIndexChanged += new System.EventHandler(this.CmbShouBunrui_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(411, 75);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "小分類";
            // 
            // CmbChuuBunrui
            // 
            this.CmbChuuBunrui.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbChuuBunrui.FormattingEnabled = true;
            this.CmbChuuBunrui.Location = new System.Drawing.Point(458, 46);
            this.CmbChuuBunrui.Name = "CmbChuuBunrui";
            this.CmbChuuBunrui.Size = new System.Drawing.Size(180, 20);
            this.CmbChuuBunrui.TabIndex = 5;
            this.CmbChuuBunrui.SelectedIndexChanged += new System.EventHandler(this.CmbChuuBunrui_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(411, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "中分類";
            // 
            // CmbDaiBunrui
            // 
            this.CmbDaiBunrui.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbDaiBunrui.FormattingEnabled = true;
            this.CmbDaiBunrui.Location = new System.Drawing.Point(458, 20);
            this.CmbDaiBunrui.Name = "CmbDaiBunrui";
            this.CmbDaiBunrui.Size = new System.Drawing.Size(180, 20);
            this.CmbDaiBunrui.TabIndex = 3;
            this.CmbDaiBunrui.SelectedIndexChanged += new System.EventHandler(this.CmbDaiBunrui_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(411, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "大分類";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "構台";
            // 
            // DgvEditLengthChangeList
            // 
            this.DgvEditLengthChangeList.AllowUserToAddRows = false;
            this.DgvEditLengthChangeList.AllowUserToDeleteRows = false;
            this.DgvEditLengthChangeList.AllowUserToResizeColumns = false;
            this.DgvEditLengthChangeList.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DgvEditLengthChangeList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.DgvEditLengthChangeList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvEditLengthChangeList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColLengthChangeListID,
            this.ColElementID,
            this.ColLengthChangeListSelected,
            this.Column7,
            this.Column5,
            this.Column6,
            this.Column4,
            this.ColLengthChangeListFamilyName,
            this.ColLengthChangeListBeforeLength,
            this.ColLengthChangeListAfterLength});
            this.DgvEditLengthChangeList.ContextMenuStrip = this.contextMenuStrip1;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DgvEditLengthChangeList.DefaultCellStyle = dataGridViewCellStyle2;
            this.DgvEditLengthChangeList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DgvEditLengthChangeList.Location = new System.Drawing.Point(12, 0);
            this.DgvEditLengthChangeList.Name = "DgvEditLengthChangeList";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DgvEditLengthChangeList.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.DgvEditLengthChangeList.RowHeadersVisible = false;
            this.DgvEditLengthChangeList.RowTemplate.Height = 21;
            this.DgvEditLengthChangeList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DgvEditLengthChangeList.Size = new System.Drawing.Size(824, 348);
            this.DgvEditLengthChangeList.TabIndex = 1;
            this.DgvEditLengthChangeList.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvEditLengthChangeList_CellValueChanged);
            this.DgvEditLengthChangeList.CurrentCellDirtyStateChanged += new System.EventHandler(this.DgvEditLengthChangeList_CurrentCellDirtyStateChanged);
            this.DgvEditLengthChangeList.SelectionChanged += new System.EventHandler(this.DgvEditLengthList_SelectionChanged);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.選択ToolStripMenuItem,
            this.解除oolStripMenuItem});
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
            // 解除oolStripMenuItem
            // 
            this.解除oolStripMenuItem.Name = "解除oolStripMenuItem";
            this.解除oolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.解除oolStripMenuItem.Text = "解除";
            this.解除oolStripMenuItem.Click += new System.EventHandler(this.解除ToolStripMenuItem_Click);
            // 
            // BtmCancel
            // 
            this.BtmCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtmCancel.Location = new System.Drawing.Point(761, 6);
            this.BtmCancel.Name = "BtmCancel";
            this.BtmCancel.Size = new System.Drawing.Size(75, 23);
            this.BtmCancel.TabIndex = 1;
            this.BtmCancel.Text = "キャンセル";
            this.BtmCancel.UseVisualStyleBackColor = true;
            this.BtmCancel.Click += new System.EventHandler(this.BtmCancel_Click);
            // 
            // BtnOK
            // 
            this.BtnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnOK.Location = new System.Drawing.Point(680, 6);
            this.BtnOK.Name = "BtnOK";
            this.BtnOK.Size = new System.Drawing.Size(75, 23);
            this.BtnOK.TabIndex = 0;
            this.BtnOK.Text = "OK";
            this.BtnOK.UseVisualStyleBackColor = true;
            this.BtnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // BtnSelectedAll
            // 
            this.BtnSelectedAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnSelectedAll.Location = new System.Drawing.Point(680, 123);
            this.BtnSelectedAll.Name = "BtnSelectedAll";
            this.BtnSelectedAll.Size = new System.Drawing.Size(75, 23);
            this.BtnSelectedAll.TabIndex = 2;
            this.BtnSelectedAll.Text = "一括選択";
            this.BtnSelectedAll.UseVisualStyleBackColor = true;
            this.BtnSelectedAll.Click += new System.EventHandler(this.BtnSelectedAll_Click);
            // 
            // BtnUnSelectedAll
            // 
            this.BtnUnSelectedAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnUnSelectedAll.Location = new System.Drawing.Point(761, 123);
            this.BtnUnSelectedAll.Name = "BtnUnSelectedAll";
            this.BtnUnSelectedAll.Size = new System.Drawing.Size(75, 23);
            this.BtnUnSelectedAll.TabIndex = 3;
            this.BtnUnSelectedAll.Text = "一括解除";
            this.BtnUnSelectedAll.UseVisualStyleBackColor = true;
            this.BtnUnSelectedAll.Click += new System.EventHandler(this.BtnUnSelectedAll_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel7);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(848, 537);
            this.panel1.TabIndex = 0;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.panel3);
            this.panel7.Controls.Add(this.panel6);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel7.Location = new System.Drawing.Point(0, 154);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(848, 383);
            this.panel7.TabIndex = 2;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.DgvEditLengthChangeList);
            this.panel3.Controls.Add(this.panel5);
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(848, 348);
            this.panel3.TabIndex = 1;
            // 
            // panel5
            // 
            this.panel5.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel5.Location = new System.Drawing.Point(836, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(12, 348);
            this.panel5.TabIndex = 2;
            // 
            // panel4
            // 
            this.panel4.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(12, 348);
            this.panel4.TabIndex = 0;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.BtnOK);
            this.panel6.Controls.Add(this.BtmCancel);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel6.Location = new System.Drawing.Point(0, 348);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(848, 35);
            this.panel6.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.ChkLengthChangeIsBothEnd);
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Controls.Add(this.BtnUnSelectedAll);
            this.panel2.Controls.Add(this.BtnSelectedAll);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(848, 154);
            this.panel2.TabIndex = 0;
            // 
            // ChkLengthChangeIsBothEnd
            // 
            this.ChkLengthChangeIsBothEnd.AutoSize = true;
            this.ChkLengthChangeIsBothEnd.Location = new System.Drawing.Point(13, 126);
            this.ChkLengthChangeIsBothEnd.Name = "ChkLengthChangeIsBothEnd";
            this.ChkLengthChangeIsBothEnd.Size = new System.Drawing.Size(208, 16);
            this.ChkLengthChangeIsBothEnd.TabIndex = 1;
            this.ChkLengthChangeIsBothEnd.Text = "長さの変更を部材の両端に対して反映";
            this.ChkLengthChangeIsBothEnd.UseVisualStyleBackColor = true;
            // 
            // ColLengthChangeListID
            // 
            this.ColLengthChangeListID.HeaderText = "ID";
            this.ColLengthChangeListID.Name = "ColLengthChangeListID";
            this.ColLengthChangeListID.ReadOnly = true;
            this.ColLengthChangeListID.Visible = false;
            // 
            // ColElementID
            // 
            this.ColElementID.HeaderText = "ElementID";
            this.ColElementID.Name = "ColElementID";
            this.ColElementID.ReadOnly = true;
            this.ColElementID.Visible = false;
            // 
            // ColLengthChangeListSelected
            // 
            this.ColLengthChangeListSelected.HeaderText = "";
            this.ColLengthChangeListSelected.MinimumWidth = 30;
            this.ColLengthChangeListSelected.Name = "ColLengthChangeListSelected";
            this.ColLengthChangeListSelected.Width = 30;
            // 
            // Column7
            // 
            this.Column7.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column7.HeaderText = "構台";
            this.Column7.MinimumWidth = 100;
            this.Column7.Name = "Column7";
            this.Column7.ReadOnly = true;
            // 
            // Column5
            // 
            this.Column5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column5.HeaderText = "大分類";
            this.Column5.MinimumWidth = 100;
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            // 
            // Column6
            // 
            this.Column6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column6.HeaderText = "中分類";
            this.Column6.MinimumWidth = 100;
            this.Column6.Name = "Column6";
            this.Column6.ReadOnly = true;
            // 
            // Column4
            // 
            this.Column4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column4.HeaderText = "小分類";
            this.Column4.MinimumWidth = 100;
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            // 
            // ColLengthChangeListFamilyName
            // 
            this.ColLengthChangeListFamilyName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColLengthChangeListFamilyName.HeaderText = "ファミリ名";
            this.ColLengthChangeListFamilyName.MinimumWidth = 100;
            this.ColLengthChangeListFamilyName.Name = "ColLengthChangeListFamilyName";
            this.ColLengthChangeListFamilyName.ReadOnly = true;
            // 
            // ColLengthChangeListBeforeLength
            // 
            this.ColLengthChangeListBeforeLength.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColLengthChangeListBeforeLength.HeaderText = "現在長さ";
            this.ColLengthChangeListBeforeLength.MinimumWidth = 100;
            this.ColLengthChangeListBeforeLength.Name = "ColLengthChangeListBeforeLength";
            this.ColLengthChangeListBeforeLength.ReadOnly = true;
            // 
            // ColLengthChangeListAfterLength
            // 
            this.ColLengthChangeListAfterLength.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColLengthChangeListAfterLength.HeaderText = "変更後長さ";
            this.ColLengthChangeListAfterLength.MinimumWidth = 100;
            this.ColLengthChangeListAfterLength.Name = "ColLengthChangeListAfterLength";
            this.ColLengthChangeListAfterLength.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // FrmEditLengthChange
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(848, 537);
            this.Controls.Add(this.panel1);
            this.MinimumSize = new System.Drawing.Size(864, 546);
            this.Name = "FrmEditLengthChange";
            this.ShowIcon = false;
            this.Text = "[修正] 長さ変更";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmEditLength_FormClosed);
            this.Load += new System.EventHandler(this.FrmEditLength_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DgvEditLengthChangeList)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox LstKoudai;
        private System.Windows.Forms.ComboBox CmbShouBunrui;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox CmbChuuBunrui;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox CmbDaiBunrui;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button BtmCancel;
        private System.Windows.Forms.Button BtnOK;
        private System.Windows.Forms.Button BtnSelectedAll;
        private System.Windows.Forms.Button BtnUnSelectedAll;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 選択ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 解除oolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel7;
        public System.Windows.Forms.DataGridView DgvEditLengthChangeList;
        public System.Windows.Forms.CheckBox ChkLengthChangeIsBothEnd;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColLengthChangeListID;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColElementID;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ColLengthChangeListSelected;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColLengthChangeListFamilyName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColLengthChangeListBeforeLength;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColLengthChangeListAfterLength;
    }
}