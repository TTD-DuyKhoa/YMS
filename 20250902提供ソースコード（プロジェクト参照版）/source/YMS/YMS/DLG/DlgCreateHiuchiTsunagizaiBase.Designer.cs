
namespace YMS.DLG
{
    partial class DlgCreateHiuchiTsunagizaiBase
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
            this.label1 = new System.Windows.Forms.Label();
            this.cmbSteelType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbSteelSize = new System.Windows.Forms.ComboBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtBoltNum = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.cmbBoltType2 = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.cmbBoltType1 = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbnToritsukeRikiman = new System.Windows.Forms.RadioButton();
            this.rbnToritsukeBuruman = new System.Windows.Forms.RadioButton();
            this.rbnToritsukeBolt = new System.Windows.Forms.RadioButton();
            this.rbnShoriTypeReplace = new System.Windows.Forms.RadioButton();
            this.rbnShoriTypeManual = new System.Windows.Forms.RadioButton();
            this.chkSplit = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "分類";
            // 
            // cmbSteelType
            // 
            this.cmbSteelType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSteelType.FormattingEnabled = true;
            this.cmbSteelType.Items.AddRange(new object[] {
            "チャンネル"});
            this.cmbSteelType.Location = new System.Drawing.Point(76, 19);
            this.cmbSteelType.Name = "cmbSteelType";
            this.cmbSteelType.Size = new System.Drawing.Size(242, 20);
            this.cmbSteelType.TabIndex = 1;
            this.cmbSteelType.SelectedIndexChanged += new System.EventHandler(this.cmbSteelType_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "サイズ";
            // 
            // cmbSteelSize
            // 
            this.cmbSteelSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSteelSize.FormattingEnabled = true;
            this.cmbSteelSize.Items.AddRange(new object[] {
            "C-100×50×5"});
            this.cmbSteelSize.Location = new System.Drawing.Point(76, 47);
            this.cmbSteelSize.Name = "cmbSteelSize";
            this.cmbSteelSize.Size = new System.Drawing.Size(242, 20);
            this.cmbSteelSize.TabIndex = 3;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(328, 174);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(98, 23);
            this.btnOK.TabIndex = 11;
            this.btnOK.Text = "OK(O)";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(432, 174);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(98, 23);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "キャンセル(C)";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cmbSteelType);
            this.groupBox1.Controls.Add(this.cmbSteelSize);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(324, 72);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "鋼材情報";
            // 
            // txtBoltNum
            // 
            this.txtBoltNum.Location = new System.Drawing.Point(420, 140);
            this.txtBoltNum.Name = "txtBoltNum";
            this.txtBoltNum.Size = new System.Drawing.Size(84, 19);
            this.txtBoltNum.TabIndex = 9;
            this.txtBoltNum.Text = "1";
            this.txtBoltNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(510, 143);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(17, 12);
            this.label13.TabIndex = 10;
            this.label13.Text = "本";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(357, 143);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(57, 12);
            this.label12.TabIndex = 8;
            this.label12.Text = "ボルト本数";
            // 
            // cmbBoltType2
            // 
            this.cmbBoltType2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBoltType2.FormattingEnabled = true;
            this.cmbBoltType2.Items.AddRange(new object[] {
            "BN-100"});
            this.cmbBoltType2.Location = new System.Drawing.Point(230, 139);
            this.cmbBoltType2.Name = "cmbBoltType2";
            this.cmbBoltType2.Size = new System.Drawing.Size(121, 20);
            this.cmbBoltType2.TabIndex = 7;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(167, 143);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(59, 12);
            this.label11.TabIndex = 6;
            this.label11.Text = "ボルトタイプ";
            // 
            // cmbBoltType1
            // 
            this.cmbBoltType1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBoltType1.FormattingEnabled = true;
            this.cmbBoltType1.Items.AddRange(new object[] {
            "普通ボルト"});
            this.cmbBoltType1.Location = new System.Drawing.Point(75, 139);
            this.cmbBoltType1.Name = "cmbBoltType1";
            this.cmbBoltType1.Size = new System.Drawing.Size(86, 20);
            this.cmbBoltType1.TabIndex = 5;
            this.cmbBoltType1.SelectedIndexChanged += new System.EventHandler(this.cmbBoltType1_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 143);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(57, 12);
            this.label10.TabIndex = 4;
            this.label10.Text = "ボルト種類";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbnToritsukeRikiman);
            this.groupBox2.Controls.Add(this.rbnToritsukeBuruman);
            this.groupBox2.Controls.Add(this.rbnToritsukeBolt);
            this.groupBox2.Location = new System.Drawing.Point(12, 90);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(215, 40);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "取付方法";
            // 
            // rbnToritsukeRikiman
            // 
            this.rbnToritsukeRikiman.AutoSize = true;
            this.rbnToritsukeRikiman.Location = new System.Drawing.Point(142, 18);
            this.rbnToritsukeRikiman.Name = "rbnToritsukeRikiman";
            this.rbnToritsukeRikiman.Size = new System.Drawing.Size(58, 16);
            this.rbnToritsukeRikiman.TabIndex = 2;
            this.rbnToritsukeRikiman.Text = "リキマン";
            this.rbnToritsukeRikiman.UseVisualStyleBackColor = true;
            this.rbnToritsukeRikiman.CheckedChanged += new System.EventHandler(this.rbnToritsukeRikiman_CheckedChanged);
            // 
            // rbnToritsukeBuruman
            // 
            this.rbnToritsukeBuruman.AutoSize = true;
            this.rbnToritsukeBuruman.Location = new System.Drawing.Point(76, 18);
            this.rbnToritsukeBuruman.Name = "rbnToritsukeBuruman";
            this.rbnToritsukeBuruman.Size = new System.Drawing.Size(60, 16);
            this.rbnToritsukeBuruman.TabIndex = 1;
            this.rbnToritsukeBuruman.Text = "ブルマン";
            this.rbnToritsukeBuruman.UseVisualStyleBackColor = true;
            this.rbnToritsukeBuruman.CheckedChanged += new System.EventHandler(this.rbnToritsukeBuruman_CheckedChanged);
            // 
            // rbnToritsukeBolt
            // 
            this.rbnToritsukeBolt.AutoSize = true;
            this.rbnToritsukeBolt.Checked = true;
            this.rbnToritsukeBolt.Location = new System.Drawing.Point(14, 18);
            this.rbnToritsukeBolt.Name = "rbnToritsukeBolt";
            this.rbnToritsukeBolt.Size = new System.Drawing.Size(51, 16);
            this.rbnToritsukeBolt.TabIndex = 0;
            this.rbnToritsukeBolt.TabStop = true;
            this.rbnToritsukeBolt.Text = "ボルト";
            this.rbnToritsukeBolt.UseVisualStyleBackColor = true;
            this.rbnToritsukeBolt.CheckedChanged += new System.EventHandler(this.rbnToritsukeBolt_CheckedChanged);
            // 
            // rbnShoriTypeReplace
            // 
            this.rbnShoriTypeReplace.AutoSize = true;
            this.rbnShoriTypeReplace.Checked = true;
            this.rbnShoriTypeReplace.Location = new System.Drawing.Point(6, 18);
            this.rbnShoriTypeReplace.Name = "rbnShoriTypeReplace";
            this.rbnShoriTypeReplace.Size = new System.Drawing.Size(47, 16);
            this.rbnShoriTypeReplace.TabIndex = 0;
            this.rbnShoriTypeReplace.TabStop = true;
            this.rbnShoriTypeReplace.Text = "置換";
            this.rbnShoriTypeReplace.UseVisualStyleBackColor = true;
            // 
            // rbnShoriTypeManual
            // 
            this.rbnShoriTypeManual.AutoSize = true;
            this.rbnShoriTypeManual.Location = new System.Drawing.Point(59, 18);
            this.rbnShoriTypeManual.Name = "rbnShoriTypeManual";
            this.rbnShoriTypeManual.Size = new System.Drawing.Size(71, 16);
            this.rbnShoriTypeManual.TabIndex = 1;
            this.rbnShoriTypeManual.Text = "任意配置";
            this.rbnShoriTypeManual.UseVisualStyleBackColor = true;
            // 
            // chkSplit
            // 
            this.chkSplit.AutoSize = true;
            this.chkSplit.Location = new System.Drawing.Point(342, 21);
            this.chkSplit.Name = "chkSplit";
            this.chkSplit.Size = new System.Drawing.Size(48, 16);
            this.chkSplit.TabIndex = 1;
            this.chkSplit.Text = "分割";
            this.chkSplit.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rbnShoriTypeReplace);
            this.groupBox3.Controls.Add(this.rbnShoriTypeManual);
            this.groupBox3.Location = new System.Drawing.Point(233, 90);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(134, 40);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "処理方法";
            // 
            // DlgCreateHiuchiTsunagizaiBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(541, 209);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.chkSplit);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.txtBoltNum);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.cmbBoltType2);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.cmbBoltType1);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgCreateHiuchiTsunagizaiBase";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "火打繋ぎ材ベース";
            this.TopMost = true;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbSteelType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbSteelSize;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtBoltNum;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox cmbBoltType2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cmbBoltType1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbnToritsukeRikiman;
        private System.Windows.Forms.RadioButton rbnToritsukeBuruman;
        private System.Windows.Forms.RadioButton rbnToritsukeBolt;
        private System.Windows.Forms.RadioButton rbnShoriTypeReplace;
        private System.Windows.Forms.RadioButton rbnShoriTypeManual;
        private System.Windows.Forms.CheckBox chkSplit;
        private System.Windows.Forms.GroupBox groupBox3;
    }
}