
namespace YMS_gantry.UI
{
    partial class FrmPutFukkougeta
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
            this.CmbKoudaiName = new System.Windows.Forms.ComboBox();
            this.label24 = new System.Windows.Forms.Label();
            this.CmbSizeType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.CmbSize = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.RbtSide = new System.Windows.Forms.RadioButton();
            this.RbtDown = new System.Windows.Forms.RadioButton();
            this.RbtUp = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.BtnOK = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.NmcOffset = new System.Windows.Forms.NumericUpDown();
            this.RbtFace = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.CmbLevel = new System.Windows.Forms.ComboBox();
            this.RbtFree = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.CmbMaterial = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NmcOffset)).BeginInit();
            this.SuspendLayout();
            // 
            // CmbKoudaiName
            // 
            this.CmbKoudaiName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbKoudaiName.FormattingEnabled = true;
            this.CmbKoudaiName.Location = new System.Drawing.Point(73, 12);
            this.CmbKoudaiName.Name = "CmbKoudaiName";
            this.CmbKoudaiName.Size = new System.Drawing.Size(263, 20);
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
            // CmbSizeType
            // 
            this.CmbSizeType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbSizeType.FormattingEnabled = true;
            this.CmbSizeType.Items.AddRange(new object[] {
            "H鋼",
            "C材"});
            this.CmbSizeType.Location = new System.Drawing.Point(73, 40);
            this.CmbSizeType.Name = "CmbSizeType";
            this.CmbSizeType.Size = new System.Drawing.Size(74, 20);
            this.CmbSizeType.TabIndex = 3;
            this.CmbSizeType.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "サイズ";
            // 
            // CmbSize
            // 
            this.CmbSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbSize.FormattingEnabled = true;
            this.CmbSize.Location = new System.Drawing.Point(153, 40);
            this.CmbSize.Name = "CmbSize";
            this.CmbSize.Size = new System.Drawing.Size(183, 20);
            this.CmbSize.TabIndex = 4;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.RbtSide);
            this.panel1.Controls.Add(this.RbtDown);
            this.panel1.Controls.Add(this.RbtUp);
            this.panel1.Location = new System.Drawing.Point(5, 18);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(310, 74);
            this.panel1.TabIndex = 0;
            // 
            // RbtSide
            // 
            this.RbtSide.Appearance = System.Windows.Forms.Appearance.Button;
            this.RbtSide.BackgroundImage = global::YMS_gantry.Properties.Resources.チャンネル材向き4;
            this.RbtSide.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.RbtSide.Location = new System.Drawing.Point(206, 5);
            this.RbtSide.Name = "RbtSide";
            this.RbtSide.Size = new System.Drawing.Size(96, 61);
            this.RbtSide.TabIndex = 2;
            this.RbtSide.Tag = "90";
            this.RbtSide.UseVisualStyleBackColor = true;
            // 
            // RbtDown
            // 
            this.RbtDown.Appearance = System.Windows.Forms.Appearance.Button;
            this.RbtDown.BackgroundImage = global::YMS_gantry.Properties.Resources.チャンネル材向き2;
            this.RbtDown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.RbtDown.Location = new System.Drawing.Point(104, 5);
            this.RbtDown.Name = "RbtDown";
            this.RbtDown.Size = new System.Drawing.Size(96, 61);
            this.RbtDown.TabIndex = 1;
            this.RbtDown.Tag = "0";
            this.RbtDown.UseVisualStyleBackColor = true;
            // 
            // RbtUp
            // 
            this.RbtUp.Appearance = System.Windows.Forms.Appearance.Button;
            this.RbtUp.BackColor = System.Drawing.SystemColors.Control;
            this.RbtUp.BackgroundImage = global::YMS_gantry.Properties.Resources.チャンネル材向き1;
            this.RbtUp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.RbtUp.Checked = true;
            this.RbtUp.Location = new System.Drawing.Point(5, 5);
            this.RbtUp.Name = "RbtUp";
            this.RbtUp.Size = new System.Drawing.Size(96, 61);
            this.RbtUp.TabIndex = 0;
            this.RbtUp.TabStop = true;
            this.RbtUp.Tag = "180";
            this.RbtUp.UseVisualStyleBackColor = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Enabled = false;
            this.groupBox1.Location = new System.Drawing.Point(12, 92);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(324, 99);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "形状タイプ";
            // 
            // BtnCancel
            // 
            this.BtnCancel.Location = new System.Drawing.Point(261, 302);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 23);
            this.BtnCancel.TabIndex = 10;
            this.BtnCancel.Text = "キャンセル";
            this.BtnCancel.UseVisualStyleBackColor = true;
            this.BtnCancel.Click += new System.EventHandler(this.button5_Click);
            // 
            // BtnOK
            // 
            this.BtnOK.Location = new System.Drawing.Point(180, 302);
            this.BtnOK.Name = "BtnOK";
            this.BtnOK.Size = new System.Drawing.Size(75, 23);
            this.BtnOK.TabIndex = 9;
            this.BtnOK.Text = "OK";
            this.BtnOK.UseVisualStyleBackColor = true;
            this.BtnOK.Click += new System.EventHandler(this.button6_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.NmcOffset);
            this.groupBox2.Controls.Add(this.RbtFace);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.CmbLevel);
            this.groupBox2.Controls.Add(this.RbtFree);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(12, 197);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(251, 96);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "配置方法";
            // 
            // NmcOffset
            // 
            this.NmcOffset.Location = new System.Drawing.Point(141, 68);
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
            this.NmcOffset.Size = new System.Drawing.Size(65, 19);
            this.NmcOffset.TabIndex = 5;
            this.NmcOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // RbtFace
            // 
            this.RbtFace.AutoSize = true;
            this.RbtFace.Checked = true;
            this.RbtFace.Location = new System.Drawing.Point(6, 21);
            this.RbtFace.Name = "RbtFace";
            this.RbtFace.Size = new System.Drawing.Size(83, 16);
            this.RbtFace.TabIndex = 0;
            this.RbtFace.TabStop = true;
            this.RbtFace.Text = "配置面指定";
            this.RbtFace.UseVisualStyleBackColor = true;
            this.RbtFace.CheckedChanged += new System.EventHandler(this.RbtFace_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "基準ﾚﾍﾞﾙ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(212, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "mm";
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
            this.RbtFree.Location = new System.Drawing.Point(109, 21);
            this.RbtFree.Name = "RbtFree";
            this.RbtFree.Size = new System.Drawing.Size(95, 16);
            this.RbtFree.TabIndex = 1;
            this.RbtFree.Text = "自由位置指定";
            this.RbtFree.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 70);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(129, 12);
            this.label4.TabIndex = 4;
            this.label4.Text = "基準ﾚﾍﾞﾙからのｵﾌｾｯﾄ値";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 69);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 5;
            this.label5.Text = "材質";
            // 
            // CmbMaterial
            // 
            this.CmbMaterial.FormattingEnabled = true;
            this.CmbMaterial.Items.AddRange(new object[] {
            "SS400",
            "SM490"});
            this.CmbMaterial.Location = new System.Drawing.Point(73, 66);
            this.CmbMaterial.Name = "CmbMaterial";
            this.CmbMaterial.Size = new System.Drawing.Size(74, 20);
            this.CmbMaterial.TabIndex = 6;
            this.CmbMaterial.Text = "SS400";
            // 
            // FrmPutFukkougeta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(348, 331);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.CmbMaterial);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.BtnOK);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.CmbSize);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.CmbSizeType);
            this.Controls.Add(this.CmbKoudaiName);
            this.Controls.Add(this.label24);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(364, 370);
            this.Name = "FrmPutFukkougeta";
            this.ShowIcon = false;
            this.Text = "[個別] 覆工桁配置";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FrmPutFukkougeta_Load);
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NmcOffset)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.ComboBox CmbKoudaiName;
        public System.Windows.Forms.Label label24;
        public System.Windows.Forms.ComboBox CmbSizeType;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.ComboBox CmbSize;
        public System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.Button BtnCancel;
        public System.Windows.Forms.Button BtnOK;
        public System.Windows.Forms.RadioButton RbtSide;
        public System.Windows.Forms.RadioButton RbtDown;
        public System.Windows.Forms.RadioButton RbtUp;
        public System.Windows.Forms.GroupBox groupBox2;
        public System.Windows.Forms.RadioButton RbtFace;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.ComboBox CmbLevel;
        public System.Windows.Forms.RadioButton RbtFree;
        public System.Windows.Forms.Label label4;
        public System.Windows.Forms.Label label5;
        public System.Windows.Forms.ComboBox CmbMaterial;
        public System.Windows.Forms.NumericUpDown NmcOffset;
    }
}