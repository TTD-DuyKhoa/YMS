
namespace YMS_gantry.UI
{
    partial class FrmPutFukkouban
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
            this.CmbMaterial = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.CmbKoudaiName = new System.Windows.Forms.ComboBox();
            this.label24 = new System.Windows.Forms.Label();
            this.CmbSize = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.BtnOK = new System.Windows.Forms.Button();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.NmcOffset = new System.Windows.Forms.NumericUpDown();
            this.label86 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.RbtFree = new System.Windows.Forms.RadioButton();
            this.CmbLevel = new System.Windows.Forms.ComboBox();
            this.RbtFace = new System.Windows.Forms.RadioButton();
            this.label34 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.NmcKyoutyouCnt = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.NmcHukuinCnt = new System.Windows.Forms.NumericUpDown();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NmcOffset)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NmcKyoutyouCnt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NmcHukuinCnt)).BeginInit();
            this.SuspendLayout();
            // 
            // CmbMaterial
            // 
            this.CmbMaterial.FormattingEnabled = true;
            this.CmbMaterial.Items.AddRange(new object[] {
            "SS400",
            "SM490"});
            this.CmbMaterial.Location = new System.Drawing.Point(75, 40);
            this.CmbMaterial.Name = "CmbMaterial";
            this.CmbMaterial.Size = new System.Drawing.Size(97, 20);
            this.CmbMaterial.TabIndex = 3;
            this.CmbMaterial.Text = "SS400";
            this.CmbMaterial.SelectedIndexChanged += new System.EventHandler(this.CmbMaterial_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "材質";
            // 
            // CmbKoudaiName
            // 
            this.CmbKoudaiName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbKoudaiName.FormattingEnabled = true;
            this.CmbKoudaiName.Location = new System.Drawing.Point(75, 12);
            this.CmbKoudaiName.Name = "CmbKoudaiName";
            this.CmbKoudaiName.Size = new System.Drawing.Size(323, 20);
            this.CmbKoudaiName.TabIndex = 1;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(12, 15);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(29, 12);
            this.label24.TabIndex = 0;
            this.label24.Text = "構台";
            // 
            // CmbSize
            // 
            this.CmbSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbSize.FormattingEnabled = true;
            this.CmbSize.Items.AddRange(new object[] {
            "MD1.0x2.0",
            "MD1.0x3.0",
            "MD(M)1.0x2.0",
            "MD(M)1.0x3.0"});
            this.CmbSize.Location = new System.Drawing.Point(75, 66);
            this.CmbSize.Name = "CmbSize";
            this.CmbSize.Size = new System.Drawing.Size(143, 20);
            this.CmbSize.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "サイズ";
            // 
            // BtnOK
            // 
            this.BtnOK.Location = new System.Drawing.Point(242, 193);
            this.BtnOK.Name = "BtnOK";
            this.BtnOK.Size = new System.Drawing.Size(75, 23);
            this.BtnOK.TabIndex = 8;
            this.BtnOK.Text = "OK";
            this.BtnOK.UseVisualStyleBackColor = true;
            this.BtnOK.Click += new System.EventHandler(this.button1_Click);
            // 
            // BtnCancel
            // 
            this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnCancel.Location = new System.Drawing.Point(323, 193);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 23);
            this.BtnCancel.TabIndex = 9;
            this.BtnCancel.Text = "キャンセル";
            this.BtnCancel.UseVisualStyleBackColor = true;
            this.BtnCancel.Click += new System.EventHandler(this.button3_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.NmcOffset);
            this.groupBox2.Controls.Add(this.label86);
            this.groupBox2.Controls.Add(this.label40);
            this.groupBox2.Controls.Add(this.RbtFree);
            this.groupBox2.Controls.Add(this.CmbLevel);
            this.groupBox2.Controls.Add(this.RbtFace);
            this.groupBox2.Controls.Add(this.label34);
            this.groupBox2.Location = new System.Drawing.Point(12, 92);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(247, 96);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "配置方法";
            // 
            // NmcOffset
            // 
            this.NmcOffset.Location = new System.Drawing.Point(138, 68);
            this.NmcOffset.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.NmcOffset.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.NmcOffset.Name = "NmcOffset";
            this.NmcOffset.Size = new System.Drawing.Size(68, 19);
            this.NmcOffset.TabIndex = 5;
            this.NmcOffset.Leave += new System.EventHandler(this.numericUpDown_Leave);
            // 
            // label86
            // 
            this.label86.AutoSize = true;
            this.label86.Location = new System.Drawing.Point(6, 46);
            this.label86.Name = "label86";
            this.label86.Size = new System.Drawing.Size(54, 12);
            this.label86.TabIndex = 2;
            this.label86.Text = "基準ﾚﾍﾞﾙ";
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(212, 70);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(23, 12);
            this.label40.TabIndex = 6;
            this.label40.Text = "mm";
            // 
            // RbtFree
            // 
            this.RbtFree.AutoSize = true;
            this.RbtFree.Location = new System.Drawing.Point(111, 20);
            this.RbtFree.Name = "RbtFree";
            this.RbtFree.Size = new System.Drawing.Size(95, 16);
            this.RbtFree.TabIndex = 1;
            this.RbtFree.Text = "自由位置指定";
            this.RbtFree.UseVisualStyleBackColor = true;
            this.RbtFree.CheckedChanged += new System.EventHandler(this.radioButton3_CheckedChanged);
            // 
            // CmbLevel
            // 
            this.CmbLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbLevel.FormattingEnabled = true;
            this.CmbLevel.Location = new System.Drawing.Point(66, 42);
            this.CmbLevel.Name = "CmbLevel";
            this.CmbLevel.Size = new System.Drawing.Size(140, 20);
            this.CmbLevel.TabIndex = 3;
            // 
            // RbtFace
            // 
            this.RbtFace.AutoSize = true;
            this.RbtFace.Checked = true;
            this.RbtFace.Location = new System.Drawing.Point(8, 20);
            this.RbtFace.Name = "RbtFace";
            this.RbtFace.Size = new System.Drawing.Size(83, 16);
            this.RbtFace.TabIndex = 0;
            this.RbtFace.TabStop = true;
            this.RbtFace.Text = "配置面指定";
            this.RbtFace.UseVisualStyleBackColor = true;
            this.RbtFace.CheckedChanged += new System.EventHandler(this.radioButton4_CheckedChanged);
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(6, 70);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(129, 12);
            this.label34.TabIndex = 4;
            this.label34.Text = "基準ﾚﾍﾞﾙからのｵﾌｾｯﾄ値";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.NmcKyoutyouCnt);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.NmcHukuinCnt);
            this.groupBox1.Location = new System.Drawing.Point(265, 92);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(133, 62);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "配置枚数";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(89, 15);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 1;
            this.label5.Text = "橋長";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "幅員";
            // 
            // NmcKyoutyouCnt
            // 
            this.NmcKyoutyouCnt.Location = new System.Drawing.Point(80, 36);
            this.NmcKyoutyouCnt.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NmcKyoutyouCnt.Name = "NmcKyoutyouCnt";
            this.NmcKyoutyouCnt.Size = new System.Drawing.Size(47, 19);
            this.NmcKyoutyouCnt.TabIndex = 4;
            this.NmcKyoutyouCnt.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NmcKyoutyouCnt.Leave += new System.EventHandler(this.numericUpDown_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(58, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "×";
            // 
            // NmcHukuinCnt
            // 
            this.NmcHukuinCnt.Location = new System.Drawing.Point(6, 36);
            this.NmcHukuinCnt.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NmcHukuinCnt.Name = "NmcHukuinCnt";
            this.NmcHukuinCnt.Size = new System.Drawing.Size(47, 19);
            this.NmcHukuinCnt.TabIndex = 2;
            this.NmcHukuinCnt.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NmcHukuinCnt.Leave += new System.EventHandler(this.numericUpDown_Leave);
            // 
            // FrmPutFukkouban
            // 
            this.AcceptButton = this.BtnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BtnCancel;
            this.ClientSize = new System.Drawing.Size(410, 228);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.BtnOK);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.CmbMaterial);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.CmbKoudaiName);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.CmbSize);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(426, 267);
            this.Name = "FrmPutFukkouban";
            this.ShowIcon = false;
            this.Text = "[個別] 覆工板配置";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FrmPutFukkouban_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NmcOffset)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NmcKyoutyouCnt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NmcHukuinCnt)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.ComboBox CmbMaterial;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.ComboBox CmbKoudaiName;
        public System.Windows.Forms.Label label24;
        public System.Windows.Forms.ComboBox CmbSize;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.Button BtnOK;
        public System.Windows.Forms.Button BtnCancel;
        public System.Windows.Forms.GroupBox groupBox2;
        public System.Windows.Forms.Label label86;
        public System.Windows.Forms.Label label40;
        public System.Windows.Forms.RadioButton RbtFree;
        public System.Windows.Forms.ComboBox CmbLevel;
        public System.Windows.Forms.RadioButton RbtFace;
        public System.Windows.Forms.Label label34;
        public System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.NumericUpDown NmcKyoutyouCnt;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.NumericUpDown NmcHukuinCnt;
        public System.Windows.Forms.NumericUpDown NmcOffset;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}