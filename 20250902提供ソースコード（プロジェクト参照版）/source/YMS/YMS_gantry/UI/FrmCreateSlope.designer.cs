
using YMS_gantry.UI.FrnCreateSlopeControls;

namespace YMS_gantry.UI
{
    partial class FrmCreateSlope
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.gridSupportParts = new YMS_gantry.UI.FrnCreateSlopeControls.YMSGridViewSupportParts();
            this.colSupportHeader = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTensetsuban = new YMS_gantry.UI.FrnCreateSlopeControls.YMSGridColumnTensetsubanType();
            this.colMadumezai = new YMS_gantry.UI.FrnCreateSlopeControls.YMSGridColumnMadumezaiType();
            this.colStiffener = new YMS_gantry.UI.FrnCreateSlopeControls.YMSGridColumnStiffenerType();
            this.gridSlopeStyle = new YMS_gantry.UI.FrnCreateSlopeControls.YMSGridViewSlopeStyle();
            this.colStyleHeader = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSlopeStyle = new YMS_gantry.UI.FrnCreateSlopeControls.YMSGridColumnSlopeType();
            this.colSlopePercent = new YMS_gantry.UI.FrnCreateSlopeControls.YMSGridColumnRealNum();
            this.colSlopeLevel = new YMS_gantry.UI.FrnCreateSlopeControls.YMSGridColumnRealNum();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.PicSlopeGuideImage = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.flowLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridSupportParts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridSlopeStyle)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PicSlopeGuideImage)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "構台";
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(40, 0);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(235, 20);
            this.comboBox1.TabIndex = 1;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(819, 11);
            this.btnOk.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(897, 11);
            this.button2.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "キャンセル";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button3.Enabled = false;
            this.button3.Location = new System.Drawing.Point(12, 11);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(154, 23);
            this.button3.TabIndex = 0;
            this.button3.Text = "プレビュー";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel1, 2);
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.comboBox1);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(275, 20);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.gridSupportParts, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.gridSlopeStyle, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(984, 231);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // gridSupportParts
            // 
            this.gridSupportParts.AllowUserToAddRows = false;
            this.gridSupportParts.AllowUserToDeleteRows = false;
            this.gridSupportParts.AllowUserToResizeRows = false;
            this.gridSupportParts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridSupportParts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridSupportParts.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridSupportParts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridSupportParts.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSupportHeader,
            this.colTensetsuban,
            this.colMadumezai,
            this.colStiffener});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridSupportParts.DefaultCellStyle = dataGridViewCellStyle2;
            this.gridSupportParts.Location = new System.Drawing.Point(495, 29);
            this.gridSupportParts.Name = "gridSupportParts";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridSupportParts.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.gridSupportParts.RowHeadersVisible = false;
            this.gridSupportParts.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.gridSupportParts.RowTemplate.Height = 21;
            this.gridSupportParts.Size = new System.Drawing.Size(486, 199);
            this.gridSupportParts.TabIndex = 2;
            // 
            // colSupportHeader
            // 
            this.colSupportHeader.HeaderText = "位置";
            this.colSupportHeader.Name = "colSupportHeader";
            this.colSupportHeader.ReadOnly = true;
            // 
            // colTensetsuban
            // 
            this.colTensetsuban.HeaderText = "添接板有無";
            this.colTensetsuban.Name = "colTensetsuban";
            // 
            // colMadumezai
            // 
            this.colMadumezai.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colMadumezai.HeaderText = "間詰材有無";
            this.colMadumezai.Name = "colMadumezai";
            this.colMadumezai.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // colStiffener
            // 
            this.colStiffener.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colStiffener.HeaderText = "スチフナー有無";
            this.colStiffener.Name = "colStiffener";
            // 
            // gridSlopeStyle
            // 
            this.gridSlopeStyle.AllowUserToAddRows = false;
            this.gridSlopeStyle.AllowUserToDeleteRows = false;
            this.gridSlopeStyle.AllowUserToResizeRows = false;
            this.gridSlopeStyle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridSlopeStyle.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridSlopeStyle.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.gridSlopeStyle.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridSlopeStyle.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colStyleHeader,
            this.colSlopeStyle,
            this.colSlopePercent,
            this.colSlopeLevel});
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridSlopeStyle.DefaultCellStyle = dataGridViewCellStyle7;
            this.gridSlopeStyle.Location = new System.Drawing.Point(3, 29);
            this.gridSlopeStyle.Name = "gridSlopeStyle";
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridSlopeStyle.RowHeadersDefaultCellStyle = dataGridViewCellStyle8;
            this.gridSlopeStyle.RowHeadersVisible = false;
            this.gridSlopeStyle.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.gridSlopeStyle.RowTemplate.Height = 21;
            this.gridSlopeStyle.Size = new System.Drawing.Size(486, 199);
            this.gridSlopeStyle.TabIndex = 1;
            this.gridSlopeStyle.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridSlopeStyle_CellContentClick);
            this.gridSlopeStyle.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dataGridView1_CellValidating);
            // 
            // colStyleHeader
            // 
            this.colStyleHeader.HeaderText = "位置";
            this.colStyleHeader.Name = "colStyleHeader";
            this.colStyleHeader.ReadOnly = true;
            // 
            // colSlopeStyle
            // 
            this.colSlopeStyle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colSlopeStyle.HeaderText = "スロープ種類";
            this.colSlopeStyle.Name = "colSlopeStyle";
            this.colSlopeStyle.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colSlopeStyle.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colSlopePercent
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colSlopePercent.DefaultCellStyle = dataGridViewCellStyle5;
            this.colSlopePercent.HeaderText = "勾配設定（%）";
            this.colSlopePercent.Name = "colSlopePercent";
            // 
            // colSlopeLevel
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colSlopeLevel.DefaultCellStyle = dataGridViewCellStyle6;
            this.colSlopeLevel.HeaderText = "レベル設定(mm)";
            this.colSlopeLevel.Name = "colSlopeLevel";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(984, 530);
            this.panel1.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.tableLayoutPanel1);
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(984, 485);
            this.panel3.TabIndex = 0;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.PicSlopeGuideImage);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(0, 231);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(984, 254);
            this.panel4.TabIndex = 0;
            // 
            // PicSlopeGuideImage
            // 
            this.PicSlopeGuideImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PicSlopeGuideImage.Image = global::YMS_gantry.Properties.Resources.凡例図_スロープ;
            this.PicSlopeGuideImage.Location = new System.Drawing.Point(0, 0);
            this.PicSlopeGuideImage.MinimumSize = new System.Drawing.Size(984, 255);
            this.PicSlopeGuideImage.Name = "PicSlopeGuideImage";
            this.PicSlopeGuideImage.Size = new System.Drawing.Size(984, 255);
            this.PicSlopeGuideImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PicSlopeGuideImage.TabIndex = 0;
            this.PicSlopeGuideImage.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.button3);
            this.panel2.Controls.Add(this.button2);
            this.panel2.Controls.Add(this.btnOk);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 485);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(984, 45);
            this.panel2.TabIndex = 0;
            // 
            // FrmCreateSlope
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 530);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1000, 569);
            this.Name = "FrmCreateSlope";
            this.Text = "スロープ作成";
            this.Load += new System.EventHandler(this.FrnCreateSlope_Load);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridSupportParts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridSlopeStyle)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PicSlopeGuideImage)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        public YMSGridViewSlopeStyle gridSlopeStyle;
        public System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStyleHeader;
        private YMSGridColumnSlopeType colSlopeStyle;
        private YMSGridColumnRealNum colSlopePercent;
        private YMSGridColumnRealNum colSlopeLevel;
        public YMSGridViewSupportParts gridSupportParts;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSupportHeader;
        private YMSGridColumnTensetsubanType colTensetsuban;
        private YMSGridColumnMadumezaiType colMadumezai;
        private YMSGridColumnStiffenerType colStiffener;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.PictureBox PicSlopeGuideImage;
        private System.Windows.Forms.Panel panel2;
    }
}