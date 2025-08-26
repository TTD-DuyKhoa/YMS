
using YMS_gantry.UI.FrnCreateSlopeControls;

namespace YMS_gantry.UI
{
    partial class FrmPutHorizontalTsunagi
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.CmbKoudaiName = new YMS_gantry.UI.YmsKodaiNameCombo();
            this.label24 = new System.Windows.Forms.Label();
            this.CmbSize = new YMS_gantry.UI.YmsComboTsunagiSize();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton2 = new YMS_gantry.UI.YmsRadioButton();
            this.radioButton1 = new YMS_gantry.UI.YmsRadioButton();
            this.groupBox16 = new System.Windows.Forms.GroupBox();
            this.RbtAttachWayTeiketsu = new YMS_gantry.UI.YmsRadioButton();
            this.label21 = new System.Windows.Forms.Label();
            this.RbtAttachWayWelding = new YMS_gantry.UI.YmsRadioButton();
            this.RbtAttachWayBolt = new YMS_gantry.UI.YmsRadioButton();
            this.groupBox17 = new System.Windows.Forms.GroupBox();
            this.NmcELng = new YMS_gantry.UI.YmsNumericInteger();
            this.NmcSLng = new YMS_gantry.UI.YmsNumericInteger();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.NmcDanSpan = new YMS_gantry.UI.YmsNumericInteger();
            this.DgvDansu = new YMS_gantry.UI.YmsGridDan();
            this.ColDansuNo = new YMS_gantry.UI.YmsColumnDanNumber();
            this.ColDansuSpan = new YMS_gantry.UI.FrnCreateSlopeControls.YMSGridColumnRealNum();
            this.chkTsunagiUmu = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.label27 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.NmcDanCount = new YMS_gantry.UI.YmsNumericInteger();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.RbtPutWayMulti = new YMS_gantry.UI.YmsRadioButton();
            this.RbtPutWaySingle = new YMS_gantry.UI.YmsRadioButton();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox16.SuspendLayout();
            this.groupBox17.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NmcELng)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NmcSLng)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NmcDanSpan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DgvDansu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NmcDanCount)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // CmbKoudaiName
            // 
            this.CmbKoudaiName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbKoudaiName.FormattingEnabled = true;
            this.CmbKoudaiName.Location = new System.Drawing.Point(50, 12);
            this.CmbKoudaiName.Name = "CmbKoudaiName";
            this.CmbKoudaiName.Size = new System.Drawing.Size(254, 20);
            this.CmbKoudaiName.TabIndex = 1;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(10, 15);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(29, 12);
            this.label24.TabIndex = 0;
            this.label24.Text = "構台";
            // 
            // CmbSize
            // 
            this.CmbSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbSize.FormattingEnabled = true;
            this.CmbSize.Location = new System.Drawing.Point(50, 37);
            this.CmbSize.Name = "CmbSize";
            this.CmbSize.Size = new System.Drawing.Size(193, 20);
            this.CmbSize.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "サイズ";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(144, 404);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(225, 404);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 8;
            this.button3.Text = "キャンセル";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Controls.Add(this.radioButton1);
            this.groupBox1.Location = new System.Drawing.Point(73, 183);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(177, 47);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "杭への取り付けタイプ";
            this.groupBox1.Visible = false;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(73, 20);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(47, 16);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.Text = "片側";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(8, 20);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(47, 16);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "両側";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // groupBox16
            // 
            this.groupBox16.Controls.Add(this.RbtAttachWayTeiketsu);
            this.groupBox16.Controls.Add(this.label21);
            this.groupBox16.Controls.Add(this.RbtAttachWayWelding);
            this.groupBox16.Controls.Add(this.RbtAttachWayBolt);
            this.groupBox16.Location = new System.Drawing.Point(8, 61);
            this.groupBox16.Name = "groupBox16";
            this.groupBox16.Size = new System.Drawing.Size(200, 63);
            this.groupBox16.TabIndex = 1;
            this.groupBox16.TabStop = false;
            this.groupBox16.Text = "取付方法";
            // 
            // RbtAttachWayTeiketsu
            // 
            this.RbtAttachWayTeiketsu.AutoSize = true;
            this.RbtAttachWayTeiketsu.Location = new System.Drawing.Point(122, 18);
            this.RbtAttachWayTeiketsu.Name = "RbtAttachWayTeiketsu";
            this.RbtAttachWayTeiketsu.Size = new System.Drawing.Size(71, 16);
            this.RbtAttachWayTeiketsu.TabIndex = 2;
            this.RbtAttachWayTeiketsu.TabStop = true;
            this.RbtAttachWayTeiketsu.Text = "締結金具";
            this.RbtAttachWayTeiketsu.UseVisualStyleBackColor = true;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(11, 39);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(161, 12);
            this.label21.TabIndex = 3;
            this.label21.Text = "※ 締結金具は個別機能で配置";
            // 
            // RbtAttachWayWelding
            // 
            this.RbtAttachWayWelding.AutoSize = true;
            this.RbtAttachWayWelding.Checked = true;
            this.RbtAttachWayWelding.Location = new System.Drawing.Point(12, 18);
            this.RbtAttachWayWelding.Name = "RbtAttachWayWelding";
            this.RbtAttachWayWelding.Size = new System.Drawing.Size(47, 16);
            this.RbtAttachWayWelding.TabIndex = 0;
            this.RbtAttachWayWelding.TabStop = true;
            this.RbtAttachWayWelding.Text = "溶接";
            this.RbtAttachWayWelding.UseVisualStyleBackColor = true;
            // 
            // RbtAttachWayBolt
            // 
            this.RbtAttachWayBolt.AutoSize = true;
            this.RbtAttachWayBolt.Location = new System.Drawing.Point(65, 18);
            this.RbtAttachWayBolt.Name = "RbtAttachWayBolt";
            this.RbtAttachWayBolt.Size = new System.Drawing.Size(51, 16);
            this.RbtAttachWayBolt.TabIndex = 1;
            this.RbtAttachWayBolt.TabStop = true;
            this.RbtAttachWayBolt.Text = "ボルト";
            this.RbtAttachWayBolt.UseVisualStyleBackColor = true;
            // 
            // groupBox17
            // 
            this.groupBox17.Controls.Add(this.NmcELng);
            this.groupBox17.Controls.Add(this.NmcSLng);
            this.groupBox17.Controls.Add(this.label7);
            this.groupBox17.Controls.Add(this.label8);
            this.groupBox17.Location = new System.Drawing.Point(8, 10);
            this.groupBox17.Name = "groupBox17";
            this.groupBox17.Size = new System.Drawing.Size(242, 45);
            this.groupBox17.TabIndex = 0;
            this.groupBox17.TabStop = false;
            this.groupBox17.Text = "突き出し量 (mm)";
            // 
            // NmcELng
            // 
            this.NmcELng.DoubleValue = 0;
            this.NmcELng.Location = new System.Drawing.Point(176, 19);
            this.NmcELng.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.NmcELng.Name = "NmcELng";
            this.NmcELng.Size = new System.Drawing.Size(55, 19);
            this.NmcELng.TabIndex = 3;
            this.NmcELng.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // NmcSLng
            // 
            this.NmcSLng.DoubleValue = 0;
            this.NmcSLng.Location = new System.Drawing.Point(53, 19);
            this.NmcSLng.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.NmcSLng.Name = "NmcSLng";
            this.NmcSLng.Size = new System.Drawing.Size(55, 19);
            this.NmcSLng.TabIndex = 1;
            this.NmcSLng.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 21);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "始点側";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(129, 21);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(41, 12);
            this.label8.TabIndex = 2;
            this.label8.Text = "終点側";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(133, 13);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 20;
            this.label6.Text = "基準間隔";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(8, 13);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(29, 12);
            this.label22.TabIndex = 12;
            this.label22.Text = "段数";
            // 
            // NmcDanSpan
            // 
            this.NmcDanSpan.DoubleValue = 0;
            this.NmcDanSpan.Location = new System.Drawing.Point(192, 11);
            this.NmcDanSpan.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.NmcDanSpan.Name = "NmcDanSpan";
            this.NmcDanSpan.Size = new System.Drawing.Size(55, 19);
            this.NmcDanSpan.TabIndex = 19;
            this.NmcDanSpan.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // DgvDansu
            // 
            this.DgvDansu.AllowUserToAddRows = false;
            this.DgvDansu.AllowUserToDeleteRows = false;
            this.DgvDansu.AllowUserToResizeColumns = false;
            this.DgvDansu.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DgvDansu.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.DgvDansu.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvDansu.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColDansuNo,
            this.ColDansuSpan,
            this.chkTsunagiUmu});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DgvDansu.DefaultCellStyle = dataGridViewCellStyle4;
            this.DgvDansu.Location = new System.Drawing.Point(8, 36);
            this.DgvDansu.Name = "DgvDansu";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DgvDansu.RowHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.DgvDansu.RowHeadersVisible = false;
            this.DgvDansu.RowTemplate.Height = 21;
            this.DgvDansu.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.DgvDansu.Size = new System.Drawing.Size(270, 213);
            this.DgvDansu.TabIndex = 1;
            // 
            // ColDansuNo
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.ColDansuNo.DefaultCellStyle = dataGridViewCellStyle2;
            this.ColDansuNo.HeaderText = "段";
            this.ColDansuNo.MinimumWidth = 40;
            this.ColDansuNo.Name = "ColDansuNo";
            this.ColDansuNo.ReadOnly = true;
            this.ColDansuNo.Width = 40;
            // 
            // ColDansuSpan
            // 
            this.ColDansuSpan.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.ColDansuSpan.DefaultCellStyle = dataGridViewCellStyle3;
            this.ColDansuSpan.HeaderText = "間隔 (ｍm)";
            this.ColDansuSpan.Name = "ColDansuSpan";
            // 
            // chkTsunagiUmu
            // 
            this.chkTsunagiUmu.HeaderText = "ツナギ有無";
            this.chkTsunagiUmu.MinimumWidth = 100;
            this.chkTsunagiUmu.Name = "chkTsunagiUmu";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(253, 13);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(23, 12);
            this.label27.TabIndex = 14;
            this.label27.Text = "mm";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(104, 13);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(17, 12);
            this.label28.TabIndex = 10;
            this.label28.Text = "段";
            // 
            // NmcDanCount
            // 
            this.NmcDanCount.DoubleValue = 0;
            this.NmcDanCount.Location = new System.Drawing.Point(43, 11);
            this.NmcDanCount.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.NmcDanCount.Name = "NmcDanCount";
            this.NmcDanCount.Size = new System.Drawing.Size(55, 19);
            this.NmcDanCount.TabIndex = 11;
            this.NmcDanCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NmcDanCount.ValueChanged += new System.EventHandler(this.numericUpDown14_ValueChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.RbtPutWayMulti);
            this.groupBox2.Controls.Add(this.RbtPutWaySingle);
            this.groupBox2.Location = new System.Drawing.Point(12, 64);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(126, 47);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "配置方法";
            // 
            // RbtPutWayMulti
            // 
            this.RbtPutWayMulti.AutoSize = true;
            this.RbtPutWayMulti.Checked = true;
            this.RbtPutWayMulti.Location = new System.Drawing.Point(9, 21);
            this.RbtPutWayMulti.Name = "RbtPutWayMulti";
            this.RbtPutWayMulti.Size = new System.Drawing.Size(59, 16);
            this.RbtPutWayMulti.TabIndex = 0;
            this.RbtPutWayMulti.TabStop = true;
            this.RbtPutWayMulti.Text = "まとめて";
            this.RbtPutWayMulti.UseVisualStyleBackColor = true;
            this.RbtPutWayMulti.CheckedChanged += new System.EventHandler(this.radioButton3_CheckedChanged);
            // 
            // RbtPutWaySingle
            // 
            this.RbtPutWaySingle.AutoSize = true;
            this.RbtPutWaySingle.Location = new System.Drawing.Point(72, 21);
            this.RbtPutWaySingle.Name = "RbtPutWaySingle";
            this.RbtPutWaySingle.Size = new System.Drawing.Size(47, 16);
            this.RbtPutWaySingle.TabIndex = 1;
            this.RbtPutWaySingle.Text = "単体";
            this.RbtPutWaySingle.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 117);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(292, 281);
            this.tabControl1.TabIndex = 6;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox17);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.groupBox16);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(284, 255);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "ツナギ";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.label22);
            this.tabPage2.Controls.Add(this.DgvDansu);
            this.tabPage2.Controls.Add(this.label27);
            this.tabPage2.Controls.Add(this.label28);
            this.tabPage2.Controls.Add(this.NmcDanCount);
            this.tabPage2.Controls.Add(this.NmcDanSpan);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(284, 255);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "段数設定";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(172, 110);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(132, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "一括配置時の値参照";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // FrmPutHorizontalTsunagi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(315, 443);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.CmbSize);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CmbKoudaiName);
            this.Controls.Add(this.label24);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "FrmPutHorizontalTsunagi";
            this.ShowIcon = false;
            this.Text = "[個別] 水平ツナギ配置";
            this.TopMost = true;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox16.ResumeLayout(false);
            this.groupBox16.PerformLayout();
            this.groupBox17.ResumeLayout(false);
            this.groupBox17.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NmcELng)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NmcSLng)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NmcDanSpan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DgvDansu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NmcDanCount)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private YmsKodaiNameCombo CmbKoudaiName;
        private System.Windows.Forms.Label label24;
        private YmsComboTsunagiSize CmbSize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.GroupBox groupBox1;
        private YmsRadioButton radioButton2;
        private YmsRadioButton radioButton1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label22;
        private YmsNumericInteger NmcDanSpan;
        private YmsGridDan DgvDansu;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label28;
        private YmsNumericInteger NmcDanCount;
        private System.Windows.Forms.GroupBox groupBox16;
        private YmsRadioButton RbtAttachWayTeiketsu;
        private System.Windows.Forms.Label label21;
        private YmsRadioButton RbtAttachWayWelding;
        private YmsRadioButton RbtAttachWayBolt;
        private System.Windows.Forms.GroupBox groupBox17;
        private YmsNumericInteger NmcELng;
        private YmsNumericInteger NmcSLng;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox2;
        private YmsRadioButton RbtPutWayMulti;
        private YmsRadioButton RbtPutWaySingle;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private YmsColumnDanNumber ColDansuNo;
        private YMSGridColumnRealNum ColDansuSpan;
        private System.Windows.Forms.DataGridViewCheckBoxColumn chkTsunagiUmu;
        private System.Windows.Forms.Button button2;
    }
}