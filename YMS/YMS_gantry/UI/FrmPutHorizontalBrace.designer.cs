
namespace YMS_gantry.UI
{
    partial class FrmPutHorizontalBrace
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
            this.CmbSize = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbKoudaiName = new System.Windows.Forms.ComboBox();
            this.label24 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox15 = new System.Windows.Forms.GroupBox();
            this.NmcBanRangeY = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.NmcBanRangeX = new System.Windows.Forms.NumericUpDown();
            this.label25 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.RbtAttachWayTeiketsu = new System.Windows.Forms.RadioButton();
            this.label26 = new System.Windows.Forms.Label();
            this.RbtAttachWayWelding = new System.Windows.Forms.RadioButton();
            this.RbtAttachWayBolt = new System.Windows.Forms.RadioButton();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.RbtPutLocationOhbikiBottomFlBottom = new System.Windows.Forms.RadioButton();
            this.RbtPutLocationOhbikiBottomFlTop = new System.Windows.Forms.RadioButton();
            this.RbtPutLocationOhbikiTopFlBottom = new System.Windows.Forms.RadioButton();
            this.RbtPutLocationOnTopPL = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.RbtPutWayContinue = new System.Windows.Forms.RadioButton();
            this.RbtPutWaySingle = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chkTurnBackle = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox15.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NmcBanRangeY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NmcBanRangeX)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox13.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
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
            // cmbKoudaiName
            // 
            this.cmbKoudaiName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbKoudaiName.FormattingEnabled = true;
            this.cmbKoudaiName.Location = new System.Drawing.Point(50, 12);
            this.cmbKoudaiName.Name = "cmbKoudaiName";
            this.cmbKoudaiName.Size = new System.Drawing.Size(254, 20);
            this.cmbKoudaiName.TabIndex = 69;
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
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(429, 455);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(510, 455);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 8;
            this.button3.Text = "キャンセル";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Image = global::YMS_gantry.Properties.Resources.凡例図_水平ブレース;
            this.pictureBox1.Location = new System.Drawing.Point(280, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(297, 327);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 75;
            this.pictureBox1.TabStop = false;
            // 
            // groupBox15
            // 
            this.groupBox15.Controls.Add(this.NmcBanRangeY);
            this.groupBox15.Controls.Add(this.label2);
            this.groupBox15.Controls.Add(this.NmcBanRangeX);
            this.groupBox15.Controls.Add(this.label25);
            this.groupBox15.Location = new System.Drawing.Point(7, 119);
            this.groupBox15.Name = "groupBox15";
            this.groupBox15.Size = new System.Drawing.Size(200, 44);
            this.groupBox15.TabIndex = 1;
            this.groupBox15.TabStop = false;
            this.groupBox15.Text = "配置禁止範囲 (mm)";
            // 
            // NmcBanRangeY
            // 
            this.NmcBanRangeY.Location = new System.Drawing.Point(124, 18);
            this.NmcBanRangeY.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.NmcBanRangeY.Name = "NmcBanRangeY";
            this.NmcBanRangeY.Size = new System.Drawing.Size(56, 19);
            this.NmcBanRangeY.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(106, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(12, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Y";
            // 
            // NmcBanRangeX
            // 
            this.NmcBanRangeX.Location = new System.Drawing.Point(27, 18);
            this.NmcBanRangeX.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.NmcBanRangeX.Name = "NmcBanRangeX";
            this.NmcBanRangeX.Size = new System.Drawing.Size(56, 19);
            this.NmcBanRangeX.TabIndex = 1;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(9, 20);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(12, 12);
            this.label25.TabIndex = 0;
            this.label25.Text = "X";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(443, 86);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(142, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "一括配置時の値参照";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.RbtAttachWayTeiketsu);
            this.groupBox1.Controls.Add(this.label26);
            this.groupBox1.Controls.Add(this.RbtAttachWayWelding);
            this.groupBox1.Controls.Add(this.RbtAttachWayBolt);
            this.groupBox1.Location = new System.Drawing.Point(7, 169);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 63);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "取付方法";
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
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(11, 39);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(161, 12);
            this.label26.TabIndex = 3;
            this.label26.Text = "※ 締結金具は個別機能で配置";
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
            // groupBox13
            // 
            this.groupBox13.Controls.Add(this.RbtPutLocationOhbikiBottomFlBottom);
            this.groupBox13.Controls.Add(this.RbtPutLocationOhbikiBottomFlTop);
            this.groupBox13.Controls.Add(this.RbtPutLocationOhbikiTopFlBottom);
            this.groupBox13.Controls.Add(this.RbtPutLocationOnTopPL);
            this.groupBox13.Location = new System.Drawing.Point(7, 7);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Size = new System.Drawing.Size(200, 106);
            this.groupBox13.TabIndex = 0;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "配置位置";
            // 
            // RbtPutLocationOhbikiBottomFlBottom
            // 
            this.RbtPutLocationOhbikiBottomFlBottom.AutoSize = true;
            this.RbtPutLocationOhbikiBottomFlBottom.Location = new System.Drawing.Point(9, 61);
            this.RbtPutLocationOhbikiBottomFlBottom.Name = "RbtPutLocationOhbikiBottomFlBottom";
            this.RbtPutLocationOhbikiBottomFlBottom.Size = new System.Drawing.Size(122, 16);
            this.RbtPutLocationOhbikiBottomFlBottom.TabIndex = 2;
            this.RbtPutLocationOhbikiBottomFlBottom.TabStop = true;
            this.RbtPutLocationOhbikiBottomFlBottom.Text = "大引 下フランジ下側";
            this.RbtPutLocationOhbikiBottomFlBottom.UseVisualStyleBackColor = true;
            // 
            // RbtPutLocationOhbikiBottomFlTop
            // 
            this.RbtPutLocationOhbikiBottomFlTop.AutoSize = true;
            this.RbtPutLocationOhbikiBottomFlTop.Location = new System.Drawing.Point(9, 39);
            this.RbtPutLocationOhbikiBottomFlTop.Name = "RbtPutLocationOhbikiBottomFlTop";
            this.RbtPutLocationOhbikiBottomFlTop.Size = new System.Drawing.Size(126, 16);
            this.RbtPutLocationOhbikiBottomFlTop.TabIndex = 1;
            this.RbtPutLocationOhbikiBottomFlTop.Text = "大引 下フランジ上側 ";
            this.RbtPutLocationOhbikiBottomFlTop.UseVisualStyleBackColor = true;
            // 
            // RbtPutLocationOhbikiTopFlBottom
            // 
            this.RbtPutLocationOhbikiTopFlBottom.AutoSize = true;
            this.RbtPutLocationOhbikiTopFlBottom.Checked = true;
            this.RbtPutLocationOhbikiTopFlBottom.Location = new System.Drawing.Point(9, 17);
            this.RbtPutLocationOhbikiTopFlBottom.Name = "RbtPutLocationOhbikiTopFlBottom";
            this.RbtPutLocationOhbikiTopFlBottom.Size = new System.Drawing.Size(122, 16);
            this.RbtPutLocationOhbikiTopFlBottom.TabIndex = 0;
            this.RbtPutLocationOhbikiTopFlBottom.TabStop = true;
            this.RbtPutLocationOhbikiTopFlBottom.Text = "大引 上フランジ下側";
            this.RbtPutLocationOhbikiTopFlBottom.UseVisualStyleBackColor = true;
            // 
            // RbtPutLocationOnTopPL
            // 
            this.RbtPutLocationOnTopPL.AutoSize = true;
            this.RbtPutLocationOnTopPL.Checked = true;
            this.RbtPutLocationOnTopPL.CheckState = System.Windows.Forms.CheckState.Checked;
            this.RbtPutLocationOnTopPL.Location = new System.Drawing.Point(6, 83);
            this.RbtPutLocationOnTopPL.Name = "RbtPutLocationOnTopPL";
            this.RbtPutLocationOnTopPL.Size = new System.Drawing.Size(117, 16);
            this.RbtPutLocationOnTopPL.TabIndex = 3;
            this.RbtPutLocationOnTopPL.Text = "トッププレートに配置";
            this.RbtPutLocationOnTopPL.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.RbtPutWayContinue);
            this.groupBox2.Controls.Add(this.RbtPutWaySingle);
            this.groupBox2.Location = new System.Drawing.Point(12, 64);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(126, 47);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "配置方法";
            // 
            // RbtPutWayContinue
            // 
            this.RbtPutWayContinue.AutoSize = true;
            this.RbtPutWayContinue.Checked = true;
            this.RbtPutWayContinue.Location = new System.Drawing.Point(9, 21);
            this.RbtPutWayContinue.Name = "RbtPutWayContinue";
            this.RbtPutWayContinue.Size = new System.Drawing.Size(47, 16);
            this.RbtPutWayContinue.TabIndex = 0;
            this.RbtPutWayContinue.TabStop = true;
            this.RbtPutWayContinue.Text = "連続";
            this.RbtPutWayContinue.UseVisualStyleBackColor = true;
            this.RbtPutWayContinue.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
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
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox13);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.groupBox15);
            this.panel1.Location = new System.Drawing.Point(5, 115);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(580, 334);
            this.panel1.TabIndex = 6;
            // 
            // chkTurnBackle
            // 
            this.chkTurnBackle.AutoSize = true;
            this.chkTurnBackle.Location = new System.Drawing.Point(250, 40);
            this.chkTurnBackle.Name = "chkTurnBackle";
            this.chkTurnBackle.Size = new System.Drawing.Size(86, 16);
            this.chkTurnBackle.TabIndex = 70;
            this.chkTurnBackle.Text = "ターンバックル";
            this.chkTurnBackle.UseVisualStyleBackColor = true;
            this.chkTurnBackle.CheckedChanged += new System.EventHandler(this.chkTurnBackle_CheckedChanged);
            // 
            // FrmPutHorizontalBrace
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(594, 490);
            this.Controls.Add(this.chkTurnBackle);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.CmbSize);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbKoudaiName);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmPutHorizontalBrace";
            this.ShowIcon = false;
            this.Text = "[個別] 水平ブレース配置";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FrmPutHorizontalBrace_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox15.ResumeLayout(false);
            this.groupBox15.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NmcBanRangeY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NmcBanRangeX)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox13.ResumeLayout(false);
            this.groupBox13.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox CmbSize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbKoudaiName;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.GroupBox groupBox15;
        private System.Windows.Forms.NumericUpDown NmcBanRangeY;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown NmcBanRangeX;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton RbtAttachWayTeiketsu;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.RadioButton RbtAttachWayWelding;
        private System.Windows.Forms.RadioButton RbtAttachWayBolt;
        private System.Windows.Forms.GroupBox groupBox13;
        private System.Windows.Forms.RadioButton RbtPutLocationOhbikiBottomFlTop;
        private System.Windows.Forms.RadioButton RbtPutLocationOhbikiTopFlBottom;
        private System.Windows.Forms.CheckBox RbtPutLocationOnTopPL;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton RbtPutWayContinue;
        private System.Windows.Forms.RadioButton RbtPutWaySingle;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.RadioButton RbtPutLocationOhbikiBottomFlBottom;
        private System.Windows.Forms.CheckBox chkTurnBackle;
    }
}