
namespace YMS_gantry.UI
{
    partial class FrmPutShikigeta
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.NmcELng = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.NmcSLng = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.CmbKoudaiName = new System.Windows.Forms.ComboBox();
            this.label24 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.NmcOffset = new System.Windows.Forms.NumericUpDown();
            this.label86 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.CmbLevel = new System.Windows.Forms.ComboBox();
            this.label34 = new System.Windows.Forms.Label();
            this.CmbSizeType = new System.Windows.Forms.ComboBox();
            this.CmbMaterial = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NmcELng)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NmcSLng)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NmcOffset)).BeginInit();
            this.SuspendLayout();
            // 
            // CmbSize
            // 
            this.CmbSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbSize.FormattingEnabled = true;
            this.CmbSize.Location = new System.Drawing.Point(212, 40);
            this.CmbSize.Name = "CmbSize";
            this.CmbSize.Size = new System.Drawing.Size(141, 20);
            this.CmbSize.TabIndex = 4;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.NmcELng);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.NmcSLng);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Location = new System.Drawing.Point(268, 89);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(178, 75);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "突き出し長さ";
            // 
            // NmcELng
            // 
            this.NmcELng.Location = new System.Drawing.Point(68, 43);
            this.NmcELng.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.NmcELng.Name = "NmcELng";
            this.NmcELng.Size = new System.Drawing.Size(73, 19);
            this.NmcELng.TabIndex = 4;
            this.NmcELng.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(147, 48);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(23, 12);
            this.label7.TabIndex = 5;
            this.label7.Text = "mm";
            // 
            // NmcSLng
            // 
            this.NmcSLng.Location = new System.Drawing.Point(68, 18);
            this.NmcSLng.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.NmcSLng.Name = "NmcSLng";
            this.NmcSLng.Size = new System.Drawing.Size(73, 19);
            this.NmcSLng.TabIndex = 1;
            this.NmcSLng.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(147, 22);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(23, 12);
            this.label8.TabIndex = 2;
            this.label8.Text = "mm";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(20, 48);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 12);
            this.label9.TabIndex = 3;
            this.label9.Text = "終点側";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(21, 22);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(41, 12);
            this.label10.TabIndex = 0;
            this.label10.Text = "始点側";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(290, 170);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 9;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(371, 170);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 10;
            this.button3.Text = "キャンセル";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // CmbKoudaiName
            // 
            this.CmbKoudaiName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbKoudaiName.FormattingEnabled = true;
            this.CmbKoudaiName.Location = new System.Drawing.Point(73, 12);
            this.CmbKoudaiName.Name = "CmbKoudaiName";
            this.CmbKoudaiName.Size = new System.Drawing.Size(280, 20);
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "サイズ";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.NmcOffset);
            this.groupBox1.Controls.Add(this.label86);
            this.groupBox1.Controls.Add(this.label40);
            this.groupBox1.Controls.Add(this.CmbLevel);
            this.groupBox1.Controls.Add(this.label34);
            this.groupBox1.Location = new System.Drawing.Point(11, 89);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(251, 75);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "配置位置";
            // 
            // NmcOffset
            // 
            this.NmcOffset.Location = new System.Drawing.Point(138, 44);
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
            this.NmcOffset.TabIndex = 3;
            this.NmcOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label86
            // 
            this.label86.AutoSize = true;
            this.label86.Location = new System.Drawing.Point(2, 22);
            this.label86.Name = "label86";
            this.label86.Size = new System.Drawing.Size(54, 12);
            this.label86.TabIndex = 0;
            this.label86.Text = "基準ﾚﾍﾞﾙ";
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(208, 46);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(23, 12);
            this.label40.TabIndex = 4;
            this.label40.Text = "mm";
            // 
            // CmbLevel
            // 
            this.CmbLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbLevel.FormattingEnabled = true;
            this.CmbLevel.Location = new System.Drawing.Point(62, 18);
            this.CmbLevel.Name = "CmbLevel";
            this.CmbLevel.Size = new System.Drawing.Size(140, 20);
            this.CmbLevel.TabIndex = 1;
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(2, 46);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(129, 12);
            this.label34.TabIndex = 2;
            this.label34.Text = "基準ﾚﾍﾞﾙからのｵﾌｾｯﾄ値";
            // 
            // CmbSizeType
            // 
            this.CmbSizeType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbSizeType.FormattingEnabled = true;
            this.CmbSizeType.Location = new System.Drawing.Point(73, 40);
            this.CmbSizeType.Name = "CmbSizeType";
            this.CmbSizeType.Size = new System.Drawing.Size(133, 20);
            this.CmbSizeType.TabIndex = 3;
            // 
            // CmbMaterial
            // 
            this.CmbMaterial.FormattingEnabled = true;
            this.CmbMaterial.Location = new System.Drawing.Point(73, 66);
            this.CmbMaterial.Name = "CmbMaterial";
            this.CmbMaterial.Size = new System.Drawing.Size(74, 20);
            this.CmbMaterial.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "材質";
            // 
            // FrmPutShikigeta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(458, 197);
            this.Controls.Add(this.CmbMaterial);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.CmbSizeType);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.CmbSize);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.CmbKoudaiName);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmPutShikigeta";
            this.ShowIcon = false;
            this.Text = "[個別] 敷桁配置";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FrmPutShikigeta_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NmcELng)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NmcSLng)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NmcOffset)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.NumericUpDown NmcELng;
        public System.Windows.Forms.NumericUpDown NmcSLng;
        public System.Windows.Forms.NumericUpDown NmcOffset;
        public System.Windows.Forms.ComboBox CmbSizeType;
        public System.Windows.Forms.ComboBox CmbSize;
        public System.Windows.Forms.GroupBox groupBox3;
        public System.Windows.Forms.Label label7;
        public System.Windows.Forms.Label label8;
        public System.Windows.Forms.Label label9;
        public System.Windows.Forms.Label label10;
        public System.Windows.Forms.Button button1;
        public System.Windows.Forms.Button button3;
        public System.Windows.Forms.ComboBox CmbKoudaiName;
        public System.Windows.Forms.Label label24;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.Label label86;
        public System.Windows.Forms.Label label40;
        public System.Windows.Forms.ComboBox CmbLevel;
        public System.Windows.Forms.Label label34;
        public System.Windows.Forms.ComboBox CmbMaterial;
        public System.Windows.Forms.Label label2;
    }
}