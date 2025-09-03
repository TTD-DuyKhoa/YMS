
namespace YMS_gantry.UI
{
    partial class FrmPutSuchifuna
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.RbtAuto = new System.Windows.Forms.RadioButton();
            this.RbtFree = new System.Windows.Forms.RadioButton();
            this.ChkNeda = new System.Windows.Forms.CheckBox();
            this.ChkOhbiki = new System.Windows.Forms.CheckBox();
            this.ChkShikigeta = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.CmbSize = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.CmbSizeType = new System.Windows.Forms.ComboBox();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // CmbKoudaiName
            // 
            this.CmbKoudaiName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbKoudaiName.FormattingEnabled = true;
            this.CmbKoudaiName.Location = new System.Drawing.Point(53, 10);
            this.CmbKoudaiName.Name = "CmbKoudaiName";
            this.CmbKoudaiName.Size = new System.Drawing.Size(327, 20);
            this.CmbKoudaiName.TabIndex = 1;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(12, 13);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(29, 12);
            this.label24.TabIndex = 0;
            this.label24.Text = "構台";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(224, 107);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(305, 107);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 8;
            this.button2.Text = "キャンセル";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.RbtAuto);
            this.groupBox2.Controls.Add(this.RbtFree);
            this.groupBox2.Location = new System.Drawing.Point(14, 63);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(116, 38);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "配置方法";
            // 
            // RbtAuto
            // 
            this.RbtAuto.AutoSize = true;
            this.RbtAuto.Checked = true;
            this.RbtAuto.Location = new System.Drawing.Point(6, 15);
            this.RbtAuto.Name = "RbtAuto";
            this.RbtAuto.Size = new System.Drawing.Size(47, 16);
            this.RbtAuto.TabIndex = 0;
            this.RbtAuto.TabStop = true;
            this.RbtAuto.Text = "自動";
            this.RbtAuto.UseVisualStyleBackColor = true;
            this.RbtAuto.CheckedChanged += new System.EventHandler(this.RbtAuto_CheckedChanged);
            // 
            // RbtFree
            // 
            this.RbtFree.AutoSize = true;
            this.RbtFree.Location = new System.Drawing.Point(59, 15);
            this.RbtFree.Name = "RbtFree";
            this.RbtFree.Size = new System.Drawing.Size(47, 16);
            this.RbtFree.TabIndex = 1;
            this.RbtFree.TabStop = true;
            this.RbtFree.Text = "任意";
            this.RbtFree.UseVisualStyleBackColor = true;
            this.RbtFree.CheckedChanged += new System.EventHandler(this.radioButton4_CheckedChanged);
            // 
            // ChkNeda
            // 
            this.ChkNeda.AutoSize = true;
            this.ChkNeda.Checked = true;
            this.ChkNeda.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChkNeda.Location = new System.Drawing.Point(13, 16);
            this.ChkNeda.Name = "ChkNeda";
            this.ChkNeda.Size = new System.Drawing.Size(80, 16);
            this.ChkNeda.TabIndex = 0;
            this.ChkNeda.Text = "根太(主桁)";
            this.ChkNeda.UseVisualStyleBackColor = true;
            // 
            // ChkOhbiki
            // 
            this.ChkOhbiki.AutoSize = true;
            this.ChkOhbiki.Checked = true;
            this.ChkOhbiki.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChkOhbiki.Location = new System.Drawing.Point(99, 16);
            this.ChkOhbiki.Name = "ChkOhbiki";
            this.ChkOhbiki.Size = new System.Drawing.Size(80, 16);
            this.ChkOhbiki.TabIndex = 1;
            this.ChkOhbiki.Text = "大引(桁受)";
            this.ChkOhbiki.UseVisualStyleBackColor = true;
            // 
            // ChkShikigeta
            // 
            this.ChkShikigeta.AutoSize = true;
            this.ChkShikigeta.Checked = true;
            this.ChkShikigeta.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChkShikigeta.Location = new System.Drawing.Point(185, 16);
            this.ChkShikigeta.Name = "ChkShikigeta";
            this.ChkShikigeta.Size = new System.Drawing.Size(48, 16);
            this.ChkShikigeta.TabIndex = 2;
            this.ChkShikigeta.Text = "敷桁";
            this.ChkShikigeta.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ChkShikigeta);
            this.groupBox1.Controls.Add(this.ChkNeda);
            this.groupBox1.Controls.Add(this.ChkOhbiki);
            this.groupBox1.Location = new System.Drawing.Point(136, 63);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(244, 38);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "配置対象部材";
            // 
            // CmbSize
            // 
            this.CmbSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbSize.FormattingEnabled = true;
            this.CmbSize.Location = new System.Drawing.Point(149, 36);
            this.CmbSize.Name = "CmbSize";
            this.CmbSize.Size = new System.Drawing.Size(231, 20);
            this.CmbSize.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "サイズ";
            // 
            // CmbSizeType
            // 
            this.CmbSizeType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbSizeType.FormattingEnabled = true;
            this.CmbSizeType.Location = new System.Drawing.Point(53, 36);
            this.CmbSizeType.Name = "CmbSizeType";
            this.CmbSizeType.Size = new System.Drawing.Size(90, 20);
            this.CmbSizeType.TabIndex = 3;
            this.CmbSizeType.SelectedIndexChanged += new System.EventHandler(this.CmbSizeType_SelectedIndexChanged);
            // 
            // FrmPutSuchifuna
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(389, 136);
            this.Controls.Add(this.CmbSizeType);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CmbSize);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.CmbKoudaiName);
            this.Controls.Add(this.label24);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(405, 175);
            this.Name = "FrmPutSuchifuna";
            this.ShowIcon = false;
            this.Text = "[個別] スチフナープレート・スチフナージャッキ配置";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FrmPutSuchifuna_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.ComboBox CmbKoudaiName;
        public System.Windows.Forms.Label label24;
        public System.Windows.Forms.Button button1;
        public System.Windows.Forms.Button button2;
        public System.Windows.Forms.GroupBox groupBox2;
        public System.Windows.Forms.RadioButton RbtAuto;
        public System.Windows.Forms.RadioButton RbtFree;
        public System.Windows.Forms.CheckBox ChkShikigeta;
        public System.Windows.Forms.CheckBox ChkOhbiki;
        public System.Windows.Forms.CheckBox ChkNeda;
        public System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.ComboBox CmbSize;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox CmbSizeType;
    }
}