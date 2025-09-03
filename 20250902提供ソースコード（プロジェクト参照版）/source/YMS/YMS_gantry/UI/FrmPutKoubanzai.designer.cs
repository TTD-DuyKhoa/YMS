
namespace YMS_gantry.UI
{
    partial class FrmPutKoubanzai
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.RbtShikiteppan = new System.Windows.Forms.RadioButton();
            this.RbtShimakouhan = new System.Windows.Forms.RadioButton();
            this.CmbKoudaiName = new System.Windows.Forms.ComboBox();
            this.label24 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.RbtFree = new System.Windows.Forms.RadioButton();
            this.RbtFace = new System.Windows.Forms.RadioButton();
            this.NmcOffset = new System.Windows.Forms.NumericUpDown();
            this.label86 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.CmbLevel = new System.Windows.Forms.ComboBox();
            this.label34 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.NmcShortCnt = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.NmcLongCnt = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.CmbSize = new System.Windows.Forms.ComboBox();
            this.CmbThick = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NmcOffset)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NmcShortCnt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NmcLongCnt)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.RbtShikiteppan);
            this.groupBox1.Controls.Add(this.RbtShimakouhan);
            this.groupBox1.Location = new System.Drawing.Point(12, 38);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(141, 43);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "鉄板タイプ";
            // 
            // RbtShikiteppan
            // 
            this.RbtShikiteppan.AutoSize = true;
            this.RbtShikiteppan.Location = new System.Drawing.Point(71, 18);
            this.RbtShikiteppan.Name = "RbtShikiteppan";
            this.RbtShikiteppan.Size = new System.Drawing.Size(59, 16);
            this.RbtShikiteppan.TabIndex = 1;
            this.RbtShikiteppan.TabStop = true;
            this.RbtShikiteppan.Text = "敷鉄板";
            this.RbtShikiteppan.UseVisualStyleBackColor = true;
            // 
            // RbtShimakouhan
            // 
            this.RbtShimakouhan.AutoSize = true;
            this.RbtShimakouhan.Checked = true;
            this.RbtShimakouhan.Location = new System.Drawing.Point(6, 18);
            this.RbtShimakouhan.Name = "RbtShimakouhan";
            this.RbtShimakouhan.Size = new System.Drawing.Size(59, 16);
            this.RbtShimakouhan.TabIndex = 0;
            this.RbtShimakouhan.TabStop = true;
            this.RbtShimakouhan.Text = "縞鋼板";
            this.RbtShimakouhan.UseVisualStyleBackColor = true;
            this.RbtShimakouhan.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // CmbKoudaiName
            // 
            this.CmbKoudaiName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbKoudaiName.FormattingEnabled = true;
            this.CmbKoudaiName.Location = new System.Drawing.Point(73, 12);
            this.CmbKoudaiName.Name = "CmbKoudaiName";
            this.CmbKoudaiName.Size = new System.Drawing.Size(325, 20);
            this.CmbKoudaiName.TabIndex = 1;
            this.CmbKoudaiName.SelectedIndexChanged += new System.EventHandler(this.comboBox12_SelectedIndexChanged);
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
            this.button1.Location = new System.Drawing.Point(243, 190);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 9;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(325, 190);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 10;
            this.button2.Text = "キャンセル";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.RbtFree);
            this.groupBox2.Controls.Add(this.RbtFace);
            this.groupBox2.Controls.Add(this.NmcOffset);
            this.groupBox2.Controls.Add(this.label86);
            this.groupBox2.Controls.Add(this.label40);
            this.groupBox2.Controls.Add(this.CmbLevel);
            this.groupBox2.Controls.Add(this.label34);
            this.groupBox2.Location = new System.Drawing.Point(12, 87);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(247, 97);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "配置方法";
            // 
            // RbtFree
            // 
            this.RbtFree.AutoSize = true;
            this.RbtFree.Location = new System.Drawing.Point(116, 23);
            this.RbtFree.Name = "RbtFree";
            this.RbtFree.Size = new System.Drawing.Size(95, 16);
            this.RbtFree.TabIndex = 1;
            this.RbtFree.Text = "自由位置指定";
            this.RbtFree.UseVisualStyleBackColor = true;
            // 
            // RbtFace
            // 
            this.RbtFace.AutoSize = true;
            this.RbtFace.Checked = true;
            this.RbtFace.Location = new System.Drawing.Point(13, 23);
            this.RbtFace.Name = "RbtFace";
            this.RbtFace.Size = new System.Drawing.Size(83, 16);
            this.RbtFace.TabIndex = 0;
            this.RbtFace.TabStop = true;
            this.RbtFace.Text = "配置面指定";
            this.RbtFace.UseVisualStyleBackColor = true;
            this.RbtFace.CheckedChanged += new System.EventHandler(this.radioButton4_CheckedChanged);
            // 
            // NmcOffset
            // 
            this.NmcOffset.Location = new System.Drawing.Point(147, 72);
            this.NmcOffset.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.NmcOffset.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.NmcOffset.Name = "NmcOffset";
            this.NmcOffset.Size = new System.Drawing.Size(64, 19);
            this.NmcOffset.TabIndex = 5;
            this.NmcOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NmcOffset.Leave += new System.EventHandler(this.FrmPutKoubanzai_Leave);
            // 
            // label86
            // 
            this.label86.AutoSize = true;
            this.label86.Location = new System.Drawing.Point(11, 50);
            this.label86.Name = "label86";
            this.label86.Size = new System.Drawing.Size(54, 12);
            this.label86.TabIndex = 2;
            this.label86.Text = "基準ﾚﾍﾞﾙ";
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(217, 74);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(23, 12);
            this.label40.TabIndex = 6;
            this.label40.Text = "mm";
            // 
            // CmbLevel
            // 
            this.CmbLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbLevel.FormattingEnabled = true;
            this.CmbLevel.Location = new System.Drawing.Point(71, 46);
            this.CmbLevel.Name = "CmbLevel";
            this.CmbLevel.Size = new System.Drawing.Size(140, 20);
            this.CmbLevel.TabIndex = 3;
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(11, 74);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(129, 12);
            this.label34.TabIndex = 4;
            this.label34.Text = "基準ﾚﾍﾞﾙからのｵﾌｾｯﾄ値";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.NmcShortCnt);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.NmcLongCnt);
            this.groupBox3.Location = new System.Drawing.Point(265, 87);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(133, 62);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "配置枚数";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(89, 19);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 1;
            this.label5.Text = "短辺";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "長辺";
            // 
            // NmcShortCnt
            // 
            this.NmcShortCnt.Location = new System.Drawing.Point(80, 37);
            this.NmcShortCnt.Name = "NmcShortCnt";
            this.NmcShortCnt.Size = new System.Drawing.Size(47, 19);
            this.NmcShortCnt.TabIndex = 4;
            this.NmcShortCnt.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NmcShortCnt.Leave += new System.EventHandler(this.FrmPutKoubanzai_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(58, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "×";
            // 
            // NmcLongCnt
            // 
            this.NmcLongCnt.Location = new System.Drawing.Point(6, 37);
            this.NmcLongCnt.Name = "NmcLongCnt";
            this.NmcLongCnt.Size = new System.Drawing.Size(47, 19);
            this.NmcLongCnt.TabIndex = 2;
            this.NmcLongCnt.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NmcLongCnt.Leave += new System.EventHandler(this.FrmPutKoubanzai_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(158, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "サイズ";
            // 
            // CmbSize
            // 
            this.CmbSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbSize.FormattingEnabled = true;
            this.CmbSize.Location = new System.Drawing.Point(198, 41);
            this.CmbSize.Name = "CmbSize";
            this.CmbSize.Size = new System.Drawing.Size(200, 20);
            this.CmbSize.TabIndex = 4;
            // 
            // CmbThick
            // 
            this.CmbThick.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbThick.FormattingEnabled = true;
            this.CmbThick.Items.AddRange(new object[] {
            "PL19",
            "PL22",
            "PL25"});
            this.CmbThick.Location = new System.Drawing.Point(198, 67);
            this.CmbThick.Name = "CmbThick";
            this.CmbThick.Size = new System.Drawing.Size(49, 20);
            this.CmbThick.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(158, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "厚み";
            // 
            // FrmPutKoubanzai
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(409, 225);
            this.Controls.Add(this.CmbThick);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.CmbSize);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CmbKoudaiName);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(425, 264);
            this.Name = "FrmPutKoubanzai";
            this.ShowIcon = false;
            this.Text = "[個別] 鋼板材配置";
            this.TopMost = true;
            this.Activated += new System.EventHandler(this.FrmPutKoubanzai_Activated);
            this.Load += new System.EventHandler(this.FrmPutKoubanzai_Load);
            this.Leave += new System.EventHandler(this.FrmPutKoubanzai_Leave);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NmcOffset)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NmcShortCnt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NmcLongCnt)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.NumericUpDown NmcOffset;
        public System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.RadioButton RbtShikiteppan;
        public System.Windows.Forms.RadioButton RbtShimakouhan;
        public System.Windows.Forms.ComboBox CmbKoudaiName;
        public System.Windows.Forms.Label label24;
        public System.Windows.Forms.Button button1;
        public System.Windows.Forms.Button button2;
        public System.Windows.Forms.GroupBox groupBox2;
        public System.Windows.Forms.Label label86;
        public System.Windows.Forms.Label label40;
        public System.Windows.Forms.ComboBox CmbLevel;
        public System.Windows.Forms.Label label34;
        public System.Windows.Forms.GroupBox groupBox3;
        public System.Windows.Forms.NumericUpDown NmcShortCnt;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.NumericUpDown NmcLongCnt;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox CmbSize;
        public System.Windows.Forms.ComboBox CmbThick;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.RadioButton RbtFree;
        public System.Windows.Forms.RadioButton RbtFace;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
    }
}