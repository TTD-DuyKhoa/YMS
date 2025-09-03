
namespace YMS_gantry.UI
{
    partial class FrmPutHoudue
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
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.CmbKoudaiName = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.CmbSize = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.RbtHiuchi = new System.Windows.Forms.RadioButton();
            this.RbtEdge = new System.Windows.Forms.RadioButton();
            this.RbtMaterial = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.NmcLength = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.CmbKetaSize = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.CmbKuiSize = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.NmcLength)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(219, 196);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 11;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(300, 196);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 12;
            this.button3.Text = "キャンセル";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // CmbKoudaiName
            // 
            this.CmbKoudaiName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbKoudaiName.FormattingEnabled = true;
            this.CmbKoudaiName.Location = new System.Drawing.Point(52, 12);
            this.CmbKoudaiName.Name = "CmbKoudaiName";
            this.CmbKoudaiName.Size = new System.Drawing.Size(323, 20);
            this.CmbKoudaiName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "構台";
            // 
            // CmbSize
            // 
            this.CmbSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbSize.FormattingEnabled = true;
            this.CmbSize.Location = new System.Drawing.Point(52, 68);
            this.CmbSize.Name = "CmbSize";
            this.CmbSize.Size = new System.Drawing.Size(195, 20);
            this.CmbSize.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "サイズ";
            // 
            // RbtHiuchi
            // 
            this.RbtHiuchi.AutoSize = true;
            this.RbtHiuchi.Checked = true;
            this.RbtHiuchi.Location = new System.Drawing.Point(14, 41);
            this.RbtHiuchi.Name = "RbtHiuchi";
            this.RbtHiuchi.Size = new System.Drawing.Size(207, 16);
            this.RbtHiuchi.TabIndex = 2;
            this.RbtHiuchi.TabStop = true;
            this.RbtHiuchi.Text = "主材(補助ピース含む) + 火打受ピース";
            this.RbtHiuchi.UseVisualStyleBackColor = true;
            this.RbtHiuchi.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // RbtEdge
            // 
            this.RbtEdge.AutoSize = true;
            this.RbtEdge.Location = new System.Drawing.Point(227, 41);
            this.RbtEdge.Name = "RbtEdge";
            this.RbtEdge.Size = new System.Drawing.Size(75, 16);
            this.RbtEdge.TabIndex = 3;
            this.RbtEdge.Text = "隅部ピース";
            this.RbtEdge.UseVisualStyleBackColor = true;
            this.RbtEdge.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // RbtMaterial
            // 
            this.RbtMaterial.AutoSize = true;
            this.RbtMaterial.Location = new System.Drawing.Point(308, 41);
            this.RbtMaterial.Name = "RbtMaterial";
            this.RbtMaterial.Size = new System.Drawing.Size(47, 16);
            this.RbtMaterial.TabIndex = 4;
            this.RbtMaterial.Text = "素材";
            this.RbtMaterial.UseVisualStyleBackColor = true;
            this.RbtMaterial.CheckedChanged += new System.EventHandler(this.radioButton3_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 96);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "長さ";
            // 
            // NmcLength
            // 
            this.NmcLength.Location = new System.Drawing.Point(52, 94);
            this.NmcLength.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.NmcLength.Name = "NmcLength";
            this.NmcLength.Size = new System.Drawing.Size(68, 19);
            this.NmcLength.TabIndex = 8;
            this.NmcLength.Leave += new System.EventHandler(this.numericUpDown1_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(126, 96);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(23, 12);
            this.label4.TabIndex = 9;
            this.label4.Text = "mm";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.CmbKetaSize);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.CmbKuiSize);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Location = new System.Drawing.Point(14, 119);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(233, 71);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "火打受ピース";
            // 
            // CmbKetaSize
            // 
            this.CmbKetaSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbKetaSize.FormattingEnabled = true;
            this.CmbKetaSize.Location = new System.Drawing.Point(70, 43);
            this.CmbKetaSize.Name = "CmbKetaSize";
            this.CmbKetaSize.Size = new System.Drawing.Size(157, 20);
            this.CmbKetaSize.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 46);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 12);
            this.label6.TabIndex = 2;
            this.label6.Text = "桁側サイズ";
            // 
            // CmbKuiSize
            // 
            this.CmbKuiSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbKuiSize.FormattingEnabled = true;
            this.CmbKuiSize.Location = new System.Drawing.Point(70, 17);
            this.CmbKuiSize.Name = "CmbKuiSize";
            this.CmbKuiSize.Size = new System.Drawing.Size(157, 20);
            this.CmbKuiSize.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "杭側サイズ";
            // 
            // FrmPutHoudue
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(387, 231);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.NmcLength);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.RbtMaterial);
            this.Controls.Add(this.RbtEdge);
            this.Controls.Add(this.RbtHiuchi);
            this.Controls.Add(this.CmbKoudaiName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.CmbSize);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmPutHoudue";
            this.ShowIcon = false;
            this.Text = "[個別] 方杖配置";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FrmPutHoudue_Load);
            ((System.ComponentModel.ISupportInitialize)(this.NmcLength)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Button button1;
        public System.Windows.Forms.Button button3;
        public System.Windows.Forms.ComboBox CmbKoudaiName;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.ComboBox CmbSize;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.RadioButton RbtHiuchi;
        public System.Windows.Forms.RadioButton RbtEdge;
        public System.Windows.Forms.RadioButton RbtMaterial;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.NumericUpDown NmcLength;
        public System.Windows.Forms.Label label4;
        public System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.ComboBox CmbKetaSize;
        public System.Windows.Forms.Label label6;
        public System.Windows.Forms.ComboBox CmbKuiSize;
        public System.Windows.Forms.Label label5;
    }
}