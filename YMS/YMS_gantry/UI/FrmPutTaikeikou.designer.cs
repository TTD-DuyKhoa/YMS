
namespace YMS_gantry.UI
{
    partial class FrmPutTaikeikou
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
            this.CmbKoudainame = new System.Windows.Forms.ComboBox();
            this.label24 = new System.Windows.Forms.Label();
            this.CmbSize = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.RbtAttachWelding = new System.Windows.Forms.RadioButton();
            this.RbtAttatchBolt = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.NmcBoltCnt = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.RbtHTB = new System.Windows.Forms.RadioButton();
            this.RbtBolt = new System.Windows.Forms.RadioButton();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.RbtStiffenerY = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.RbtStiffenerN = new System.Windows.Forms.RadioButton();
            this.ChkNormalSize = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.NmcClearlance = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.CmbMaterial = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NmcBoltCnt)).BeginInit();
            this.panel2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NmcClearlance)).BeginInit();
            this.SuspendLayout();
            // 
            // CmbKoudainame
            // 
            this.CmbKoudainame.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbKoudainame.FormattingEnabled = true;
            this.CmbKoudainame.Location = new System.Drawing.Point(52, 12);
            this.CmbKoudainame.Name = "CmbKoudainame";
            this.CmbKoudainame.Size = new System.Drawing.Size(371, 20);
            this.CmbKoudainame.TabIndex = 1;
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
            this.CmbSize.Location = new System.Drawing.Point(52, 40);
            this.CmbSize.Name = "CmbSize";
            this.CmbSize.Size = new System.Drawing.Size(195, 20);
            this.CmbSize.TabIndex = 3;
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
            this.groupBox1.Controls.Add(this.comboBox1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Location = new System.Drawing.Point(10, 124);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(213, 47);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "取付プレート";
            // 
            // comboBox1
            // 
            this.comboBox1.Enabled = false;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(90, 53);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(100, 20);
            this.comboBox1.TabIndex = 1;
            this.comboBox1.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "厚み";
            this.label2.Visible = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.RbtAttachWelding);
            this.panel1.Controls.Add(this.RbtAttatchBolt);
            this.panel1.Location = new System.Drawing.Point(6, 18);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 22);
            this.panel1.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "取付方法";
            // 
            // RbtAttachWelding
            // 
            this.RbtAttachWelding.AutoSize = true;
            this.RbtAttachWelding.Location = new System.Drawing.Point(137, 3);
            this.RbtAttachWelding.Name = "RbtAttachWelding";
            this.RbtAttachWelding.Size = new System.Drawing.Size(47, 16);
            this.RbtAttachWelding.TabIndex = 2;
            this.RbtAttachWelding.Text = "溶接";
            this.RbtAttachWelding.UseVisualStyleBackColor = true;
            this.RbtAttachWelding.CheckedChanged += new System.EventHandler(this.radioButton4_CheckedChanged);
            // 
            // RbtAttatchBolt
            // 
            this.RbtAttatchBolt.AutoSize = true;
            this.RbtAttatchBolt.Checked = true;
            this.RbtAttatchBolt.Location = new System.Drawing.Point(84, 3);
            this.RbtAttatchBolt.Name = "RbtAttatchBolt";
            this.RbtAttatchBolt.Size = new System.Drawing.Size(51, 16);
            this.RbtAttatchBolt.TabIndex = 1;
            this.RbtAttatchBolt.TabStop = true;
            this.RbtAttatchBolt.Text = "ボルト";
            this.RbtAttatchBolt.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.NmcBoltCnt);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.panel2);
            this.groupBox2.Location = new System.Drawing.Point(229, 124);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(213, 85);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "ボルト";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(142, 46);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(17, 12);
            this.label10.TabIndex = 4;
            this.label10.Text = "本";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(9, 61);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(68, 12);
            this.label9.TabIndex = 2;
            this.label9.Text = "(１ヶ所あたり)";
            // 
            // NmcBoltCnt
            // 
            this.NmcBoltCnt.Location = new System.Drawing.Point(90, 43);
            this.NmcBoltCnt.Name = "NmcBoltCnt";
            this.NmcBoltCnt.Size = new System.Drawing.Size(46, 19);
            this.NmcBoltCnt.TabIndex = 3;
            this.NmcBoltCnt.Leave += new System.EventHandler(this.NmcClearlance_Leave);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(9, 45);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 12);
            this.label8.TabIndex = 1;
            this.label8.Text = "本数";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.RbtHTB);
            this.panel2.Controls.Add(this.RbtBolt);
            this.panel2.Location = new System.Drawing.Point(6, 18);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(200, 22);
            this.panel2.TabIndex = 0;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 5);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "種類";
            // 
            // RbtHTB
            // 
            this.RbtHTB.AutoSize = true;
            this.RbtHTB.Location = new System.Drawing.Point(137, 3);
            this.RbtHTB.Name = "RbtHTB";
            this.RbtHTB.Size = new System.Drawing.Size(46, 16);
            this.RbtHTB.TabIndex = 2;
            this.RbtHTB.Text = "HTB";
            this.RbtHTB.UseVisualStyleBackColor = true;
            // 
            // RbtBolt
            // 
            this.RbtBolt.AutoSize = true;
            this.RbtBolt.Checked = true;
            this.RbtBolt.Location = new System.Drawing.Point(84, 3);
            this.RbtBolt.Name = "RbtBolt";
            this.RbtBolt.Size = new System.Drawing.Size(51, 16);
            this.RbtBolt.TabIndex = 1;
            this.RbtBolt.TabStop = true;
            this.RbtBolt.Text = "ボルト";
            this.RbtBolt.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(286, 215);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 13;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(367, 215);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 14;
            this.button2.Text = "キャンセル";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // RbtStiffenerY
            // 
            this.RbtStiffenerY.AutoSize = true;
            this.RbtStiffenerY.Checked = true;
            this.RbtStiffenerY.Location = new System.Drawing.Point(90, 12);
            this.RbtStiffenerY.Name = "RbtStiffenerY";
            this.RbtStiffenerY.Size = new System.Drawing.Size(35, 16);
            this.RbtStiffenerY.TabIndex = 0;
            this.RbtStiffenerY.TabStop = true;
            this.RbtStiffenerY.Text = "要";
            this.RbtStiffenerY.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.RbtStiffenerN);
            this.groupBox3.Controls.Add(this.RbtStiffenerY);
            this.groupBox3.Location = new System.Drawing.Point(10, 175);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(213, 34);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "端部スチフナー配置";
            // 
            // RbtStiffenerN
            // 
            this.RbtStiffenerN.AutoSize = true;
            this.RbtStiffenerN.Location = new System.Drawing.Point(143, 12);
            this.RbtStiffenerN.Name = "RbtStiffenerN";
            this.RbtStiffenerN.Size = new System.Drawing.Size(47, 16);
            this.RbtStiffenerN.TabIndex = 1;
            this.RbtStiffenerN.Text = "不要";
            this.RbtStiffenerN.UseVisualStyleBackColor = true;
            // 
            // ChkNormalSize
            // 
            this.ChkNormalSize.AutoSize = true;
            this.ChkNormalSize.Checked = true;
            this.ChkNormalSize.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChkNormalSize.Location = new System.Drawing.Point(261, 42);
            this.ChkNormalSize.Name = "ChkNormalSize";
            this.ChkNormalSize.Size = new System.Drawing.Size(162, 16);
            this.ChkNormalSize.TabIndex = 4;
            this.ChkNormalSize.Text = "定型サイズ (2m/3m) を優先";
            this.ChkNormalSize.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 96);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(166, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "根太中心からの離れ (クリアランス)";
            // 
            // NmcClearlance
            // 
            this.NmcClearlance.Location = new System.Drawing.Point(180, 92);
            this.NmcClearlance.Name = "NmcClearlance";
            this.NmcClearlance.Size = new System.Drawing.Size(65, 19);
            this.NmcClearlance.TabIndex = 8;
            this.NmcClearlance.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.NmcClearlance.Leave += new System.EventHandler(this.NmcClearlance_Leave);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(251, 94);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(23, 12);
            this.label5.TabIndex = 9;
            this.label5.Text = "mm";
            // 
            // CmbMaterial
            // 
            this.CmbMaterial.FormattingEnabled = true;
            this.CmbMaterial.Location = new System.Drawing.Point(52, 66);
            this.CmbMaterial.Name = "CmbMaterial";
            this.CmbMaterial.Size = new System.Drawing.Size(99, 20);
            this.CmbMaterial.TabIndex = 6;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 69);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 12);
            this.label6.TabIndex = 5;
            this.label6.Text = "材質";
            // 
            // FrmPutTaikeikou
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(454, 250);
            this.Controls.Add(this.CmbMaterial);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.NmcClearlance);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.ChkNormalSize);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.CmbKoudainame);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.CmbSize);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(470, 260);
            this.Name = "FrmPutTaikeikou";
            this.ShowIcon = false;
            this.Text = "[個別] 対傾構・対傾構取付プレート配置";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FrmPutTaikeikou_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NmcBoltCnt)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NmcClearlance)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.ComboBox CmbKoudainame;
        public System.Windows.Forms.Label label24;
        public System.Windows.Forms.ComboBox CmbSize;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.RadioButton RbtAttachWelding;
        public System.Windows.Forms.RadioButton RbtAttatchBolt;
        public System.Windows.Forms.GroupBox groupBox2;
        public System.Windows.Forms.Label label10;
        public System.Windows.Forms.Label label9;
        public System.Windows.Forms.NumericUpDown NmcBoltCnt;
        public System.Windows.Forms.Label label8;
        public System.Windows.Forms.Panel panel2;
        public System.Windows.Forms.Label label7;
        public System.Windows.Forms.RadioButton RbtHTB;
        public System.Windows.Forms.RadioButton RbtBolt;
        public System.Windows.Forms.Button button1;
        public System.Windows.Forms.Button button2;
        public System.Windows.Forms.RadioButton RbtStiffenerY;
        public System.Windows.Forms.GroupBox groupBox3;
        public System.Windows.Forms.RadioButton RbtStiffenerN;
        public System.Windows.Forms.ComboBox comboBox1;
        public System.Windows.Forms.CheckBox ChkNormalSize;
        public System.Windows.Forms.Label label4;
        public System.Windows.Forms.NumericUpDown NmcClearlance;
        public System.Windows.Forms.Label label5;
        public System.Windows.Forms.ComboBox CmbMaterial;
        public System.Windows.Forms.Label label6;
    }
}