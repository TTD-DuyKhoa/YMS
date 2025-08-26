
namespace YMS_gantry.UI
{
    partial class FrmPutTesuriTesuriShichu
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
            this.NmcOffset = new System.Windows.Forms.NumericUpDown();
            this.RbtFace = new System.Windows.Forms.RadioButton();
            this.label86 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.CmbLevel = new System.Windows.Forms.ComboBox();
            this.RbtFree = new System.Windows.Forms.RadioButton();
            this.label34 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.CmbKoudaiName = new System.Windows.Forms.ComboBox();
            this.label24 = new System.Windows.Forms.Label();
            this.CmbTesurishichuSize = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.CmbTesurishichuMaterial = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.NmcHight = new System.Windows.Forms.NumericUpDown();
            this.ChkTsukidashiKakuho = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.NmcBasePitch = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.NmcTesuriLng = new System.Windows.Forms.NumericUpDown();
            this.label22 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.CmbTesuriMaterial = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.NmcDanPitch = new System.Windows.Forms.NumericUpDown();
            this.label16 = new System.Windows.Forms.Label();
            this.NmcDanCount = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.CmbTesuriSize = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NmcOffset)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NmcHight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NmcBasePitch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NmcTesuriLng)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NmcDanPitch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NmcDanCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.NmcOffset);
            this.groupBox1.Controls.Add(this.RbtFace);
            this.groupBox1.Controls.Add(this.label86);
            this.groupBox1.Controls.Add(this.label40);
            this.groupBox1.Controls.Add(this.CmbLevel);
            this.groupBox1.Controls.Add(this.RbtFree);
            this.groupBox1.Controls.Add(this.label34);
            this.groupBox1.Location = new System.Drawing.Point(14, 365);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(251, 96);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "配置方法";
            // 
            // NmcOffset
            // 
            this.NmcOffset.Location = new System.Drawing.Point(141, 68);
            this.NmcOffset.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.NmcOffset.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.NmcOffset.Name = "NmcOffset";
            this.NmcOffset.Size = new System.Drawing.Size(65, 19);
            this.NmcOffset.TabIndex = 5;
            // 
            // RbtFace
            // 
            this.RbtFace.AutoSize = true;
            this.RbtFace.Checked = true;
            this.RbtFace.Location = new System.Drawing.Point(6, 20);
            this.RbtFace.Name = "RbtFace";
            this.RbtFace.Size = new System.Drawing.Size(83, 16);
            this.RbtFace.TabIndex = 0;
            this.RbtFace.TabStop = true;
            this.RbtFace.Text = "配置面指定";
            this.RbtFace.UseVisualStyleBackColor = true;
            this.RbtFace.CheckedChanged += new System.EventHandler(this.RbtFace_CheckedChanged);
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
            // CmbLevel
            // 
            this.CmbLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbLevel.FormattingEnabled = true;
            this.CmbLevel.Location = new System.Drawing.Point(66, 42);
            this.CmbLevel.Name = "CmbLevel";
            this.CmbLevel.Size = new System.Drawing.Size(140, 20);
            this.CmbLevel.TabIndex = 3;
            // 
            // RbtFree
            // 
            this.RbtFree.AutoSize = true;
            this.RbtFree.Location = new System.Drawing.Point(95, 20);
            this.RbtFree.Name = "RbtFree";
            this.RbtFree.Size = new System.Drawing.Size(95, 16);
            this.RbtFree.TabIndex = 1;
            this.RbtFree.Text = "自由位置指定";
            this.RbtFree.UseVisualStyleBackColor = true;
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
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(422, 447);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(503, 447);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 6;
            this.button3.Text = "キャンセル";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(130, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "mm";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 97);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "高さ";
            // 
            // CmbKoudaiName
            // 
            this.CmbKoudaiName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbKoudaiName.FormattingEnabled = true;
            this.CmbKoudaiName.Location = new System.Drawing.Point(47, 12);
            this.CmbKoudaiName.Name = "CmbKoudaiName";
            this.CmbKoudaiName.Size = new System.Drawing.Size(300, 20);
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
            // CmbTesurishichuSize
            // 
            this.CmbTesurishichuSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbTesurishichuSize.FormattingEnabled = true;
            this.CmbTesurishichuSize.Location = new System.Drawing.Point(66, 17);
            this.CmbTesurishichuSize.Name = "CmbTesurishichuSize";
            this.CmbTesurishichuSize.Size = new System.Drawing.Size(177, 20);
            this.CmbTesurishichuSize.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "サイズ";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.CmbTesurishichuMaterial);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.NmcHight);
            this.groupBox3.Controls.Add(this.ChkTsukidashiKakuho);
            this.groupBox3.Controls.Add(this.CmbTesurishichuSize);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.NmcBasePitch);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Location = new System.Drawing.Point(12, 44);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(251, 160);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "手摺支柱";
            // 
            // CmbTesurishichuMaterial
            // 
            this.CmbTesurishichuMaterial.FormattingEnabled = true;
            this.CmbTesurishichuMaterial.Items.AddRange(new object[] {
            "SS400",
            "SM490"});
            this.CmbTesurishichuMaterial.Location = new System.Drawing.Point(66, 43);
            this.CmbTesurishichuMaterial.Name = "CmbTesurishichuMaterial";
            this.CmbTesurishichuMaterial.Size = new System.Drawing.Size(89, 20);
            this.CmbTesurishichuMaterial.TabIndex = 3;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(8, 46);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(29, 12);
            this.label10.TabIndex = 2;
            this.label10.Text = "材質";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 142);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(165, 12);
            this.label1.TabIndex = 11;
            this.label1.Text = "(手摺の突き出し長さが無い場合)";
            // 
            // NmcHight
            // 
            this.NmcHight.Location = new System.Drawing.Point(66, 95);
            this.NmcHight.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.NmcHight.Name = "NmcHight";
            this.NmcHight.Size = new System.Drawing.Size(58, 19);
            this.NmcHight.TabIndex = 8;
            // 
            // ChkTsukidashiKakuho
            // 
            this.ChkTsukidashiKakuho.AutoSize = true;
            this.ChkTsukidashiKakuho.Checked = true;
            this.ChkTsukidashiKakuho.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChkTsukidashiKakuho.Location = new System.Drawing.Point(8, 123);
            this.ChkTsukidashiKakuho.Name = "ChkTsukidashiKakuho";
            this.ChkTsukidashiKakuho.Size = new System.Drawing.Size(213, 16);
            this.ChkTsukidashiKakuho.TabIndex = 10;
            this.ChkTsukidashiKakuho.Text = "支柱本数を減らして突き出しを確保する";
            this.ChkTsukidashiKakuho.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(130, 72);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(23, 12);
            this.label6.TabIndex = 6;
            this.label6.Text = "mm";
            // 
            // NmcBasePitch
            // 
            this.NmcBasePitch.Location = new System.Drawing.Point(66, 70);
            this.NmcBasePitch.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.NmcBasePitch.Name = "NmcBasePitch";
            this.NmcBasePitch.Size = new System.Drawing.Size(58, 19);
            this.NmcBasePitch.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 72);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(54, 12);
            this.label5.TabIndex = 4;
            this.label5.Text = "基準ピッチ";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(181, 73);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(23, 12);
            this.label18.TabIndex = 6;
            this.label18.Text = "mm";
            // 
            // NmcTesuriLng
            // 
            this.NmcTesuriLng.Location = new System.Drawing.Point(117, 71);
            this.NmcTesuriLng.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.NmcTesuriLng.Name = "NmcTesuriLng";
            this.NmcTesuriLng.Size = new System.Drawing.Size(58, 19);
            this.NmcTesuriLng.TabIndex = 5;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(6, 73);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(105, 12);
            this.label22.TabIndex = 4;
            this.label22.Text = "支柱天端からの距離";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.CmbTesuriMaterial);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.label15);
            this.groupBox4.Controls.Add(this.label18);
            this.groupBox4.Controls.Add(this.NmcDanPitch);
            this.groupBox4.Controls.Add(this.label16);
            this.groupBox4.Controls.Add(this.NmcTesuriLng);
            this.groupBox4.Controls.Add(this.NmcDanCount);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.label22);
            this.groupBox4.Controls.Add(this.CmbTesuriSize);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Location = new System.Drawing.Point(12, 207);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(251, 155);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "手摺";
            // 
            // CmbTesuriMaterial
            // 
            this.CmbTesuriMaterial.FormattingEnabled = true;
            this.CmbTesuriMaterial.Items.AddRange(new object[] {
            "SS400",
            "SM490"});
            this.CmbTesuriMaterial.Location = new System.Drawing.Point(64, 43);
            this.CmbTesuriMaterial.Name = "CmbTesuriMaterial";
            this.CmbTesuriMaterial.Size = new System.Drawing.Size(89, 20);
            this.CmbTesuriMaterial.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 46);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 12);
            this.label7.TabIndex = 2;
            this.label7.Text = "材質";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(128, 132);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(23, 12);
            this.label15.TabIndex = 9;
            this.label15.Text = "mm";
            // 
            // NmcDanPitch
            // 
            this.NmcDanPitch.Location = new System.Drawing.Point(64, 130);
            this.NmcDanPitch.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.NmcDanPitch.Name = "NmcDanPitch";
            this.NmcDanPitch.Size = new System.Drawing.Size(58, 19);
            this.NmcDanPitch.TabIndex = 8;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(6, 132);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(41, 12);
            this.label16.TabIndex = 7;
            this.label16.Text = "段間隔";
            // 
            // NmcDanCount
            // 
            this.NmcDanCount.Location = new System.Drawing.Point(64, 100);
            this.NmcDanCount.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.NmcDanCount.Name = "NmcDanCount";
            this.NmcDanCount.Size = new System.Drawing.Size(58, 19);
            this.NmcDanCount.TabIndex = 11;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 102);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 12);
            this.label8.TabIndex = 10;
            this.label8.Text = "段数";
            // 
            // CmbTesuriSize
            // 
            this.CmbTesuriSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbTesuriSize.FormattingEnabled = true;
            this.CmbTesuriSize.Items.AddRange(new object[] {
            "L-75x75x6",
            "L-75x75x9"});
            this.CmbTesuriSize.Location = new System.Drawing.Point(64, 17);
            this.CmbTesuriSize.Name = "CmbTesuriSize";
            this.CmbTesuriSize.Size = new System.Drawing.Size(177, 20);
            this.CmbTesuriSize.TabIndex = 1;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 20);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(34, 12);
            this.label9.TabIndex = 0;
            this.label9.Text = "サイズ";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Image = global::YMS_gantry.Properties.Resources.凡例図_手摺手摺支柱;
            this.pictureBox1.Location = new System.Drawing.Point(269, 44);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(309, 372);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 77;
            this.pictureBox1.TabStop = false;
            // 
            // FrmPutTesuriTesuriShichu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(590, 473);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.CmbKoudaiName);
            this.Controls.Add(this.label24);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmPutTesuriTesuriShichu";
            this.ShowIcon = false;
            this.Text = "[個別] 手摺・手摺支柱配置";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FrmPutTesuriTesuriShichu_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NmcOffset)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NmcHight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NmcBasePitch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NmcTesuriLng)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NmcDanPitch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NmcDanCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.RadioButton RbtFace;
        public System.Windows.Forms.Label label86;
        public System.Windows.Forms.Label label40;
        public System.Windows.Forms.ComboBox CmbLevel;
        public System.Windows.Forms.RadioButton RbtFree;
        public System.Windows.Forms.Label label34;
        public System.Windows.Forms.Button button1;
        public System.Windows.Forms.Button button3;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.ComboBox CmbKoudaiName;
        public System.Windows.Forms.Label label24;
        public System.Windows.Forms.ComboBox CmbTesurishichuSize;
        public System.Windows.Forms.Label label4;
        public System.Windows.Forms.GroupBox groupBox3;
        public System.Windows.Forms.Label label6;
        public System.Windows.Forms.NumericUpDown NmcBasePitch;
        public System.Windows.Forms.Label label5;
        public System.Windows.Forms.NumericUpDown NmcHight;
        public System.Windows.Forms.GroupBox groupBox4;
        public System.Windows.Forms.Label label15;
        public System.Windows.Forms.NumericUpDown NmcDanPitch;
        public System.Windows.Forms.Label label16;
        public System.Windows.Forms.NumericUpDown NmcDanCount;
        public System.Windows.Forms.Label label8;
        public System.Windows.Forms.ComboBox CmbTesuriSize;
        public System.Windows.Forms.Label label9;
        public System.Windows.Forms.Label label18;
        public System.Windows.Forms.NumericUpDown NmcTesuriLng;
        public System.Windows.Forms.Label label22;
        public System.Windows.Forms.CheckBox ChkTsukidashiKakuho;
        public System.Windows.Forms.NumericUpDown NmcOffset;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.PictureBox pictureBox1;
        public System.Windows.Forms.ComboBox CmbTesurishichuMaterial;
        public System.Windows.Forms.Label label10;
        public System.Windows.Forms.ComboBox CmbTesuriMaterial;
        public System.Windows.Forms.Label label7;
    }
}