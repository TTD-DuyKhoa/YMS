
namespace YMS.DLG
{
    partial class DlgCreateSyabariBase
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
            this.cmbBuhinTypeS = new System.Windows.Forms.ComboBox();
            this.cmbBuhinSizeS = new System.Windows.Forms.ComboBox();
            this.cmbJackType1 = new System.Windows.Forms.ComboBox();
            this.cmbJackType2 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbnShoriTypeBaseLine = new System.Windows.Forms.RadioButton();
            this.rbnShoriTypePtoP = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cmbSteelType = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbSteelSize = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtEndOffset1 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.cmbBuhinSizeE = new System.Windows.Forms.ComboBox();
            this.cmbBuhinTypeE = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbBuhinTypeS
            // 
            this.cmbBuhinTypeS.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBuhinTypeS.FormattingEnabled = true;
            this.cmbBuhinTypeS.Location = new System.Drawing.Point(121, 173);
            this.cmbBuhinTypeS.Name = "cmbBuhinTypeS";
            this.cmbBuhinTypeS.Size = new System.Drawing.Size(193, 20);
            this.cmbBuhinTypeS.TabIndex = 3;
            this.cmbBuhinTypeS.SelectedIndexChanged += new System.EventHandler(this.cmbBuhinTypeS_SelectedIndexChanged);
            // 
            // cmbBuhinSizeS
            // 
            this.cmbBuhinSizeS.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBuhinSizeS.FormattingEnabled = true;
            this.cmbBuhinSizeS.Location = new System.Drawing.Point(121, 199);
            this.cmbBuhinSizeS.Name = "cmbBuhinSizeS";
            this.cmbBuhinSizeS.Size = new System.Drawing.Size(193, 20);
            this.cmbBuhinSizeS.TabIndex = 5;
            // 
            // cmbJackType1
            // 
            this.cmbJackType1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbJackType1.FormattingEnabled = true;
            this.cmbJackType1.Location = new System.Drawing.Point(122, 278);
            this.cmbJackType1.Name = "cmbJackType1";
            this.cmbJackType1.Size = new System.Drawing.Size(193, 20);
            this.cmbJackType1.TabIndex = 7;
            // 
            // cmbJackType2
            // 
            this.cmbJackType2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbJackType2.FormattingEnabled = true;
            this.cmbJackType2.Location = new System.Drawing.Point(122, 304);
            this.cmbJackType2.Name = "cmbJackType2";
            this.cmbJackType2.Size = new System.Drawing.Size(193, 20);
            this.cmbJackType2.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 176);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "端部部品(始点側)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 202);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "サイズ(始点側)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 281);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "ジャッキタイプ(１)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 307);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "ジャッキタイプ(２)";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(120, 331);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(94, 23);
            this.btnOK.TabIndex = 10;
            this.btnOK.Text = "OK(O)";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(220, 331);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(94, 23);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "キャンセル(C)";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbnShoriTypeBaseLine);
            this.groupBox1.Controls.Add(this.rbnShoriTypePtoP);
            this.groupBox1.Location = new System.Drawing.Point(12, 97);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(301, 44);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "処理方法";
            // 
            // rbnShoriTypeBaseLine
            // 
            this.rbnShoriTypeBaseLine.AutoSize = true;
            this.rbnShoriTypeBaseLine.Checked = true;
            this.rbnShoriTypeBaseLine.Location = new System.Drawing.Point(131, 18);
            this.rbnShoriTypeBaseLine.Name = "rbnShoriTypeBaseLine";
            this.rbnShoriTypeBaseLine.Size = new System.Drawing.Size(59, 16);
            this.rbnShoriTypeBaseLine.TabIndex = 0;
            this.rbnShoriTypeBaseLine.TabStop = true;
            this.rbnShoriTypeBaseLine.Text = "基準線";
            this.rbnShoriTypeBaseLine.UseVisualStyleBackColor = true;
            // 
            // rbnShoriTypePtoP
            // 
            this.rbnShoriTypePtoP.AutoSize = true;
            this.rbnShoriTypePtoP.Location = new System.Drawing.Point(212, 18);
            this.rbnShoriTypePtoP.Name = "rbnShoriTypePtoP";
            this.rbnShoriTypePtoP.Size = new System.Drawing.Size(53, 16);
            this.rbnShoriTypePtoP.TabIndex = 1;
            this.rbnShoriTypePtoP.Text = "2点間";
            this.rbnShoriTypePtoP.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cmbSteelType);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.cmbSteelSize);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(300, 79);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "斜梁情報";
            // 
            // cmbSteelType
            // 
            this.cmbSteelType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSteelType.FormattingEnabled = true;
            this.cmbSteelType.Items.AddRange(new object[] {
            "H300X300X10X15",
            "20H　シュザイ",
            "35HA",
            "40HA"});
            this.cmbSteelType.Location = new System.Drawing.Point(108, 18);
            this.cmbSteelType.Name = "cmbSteelType";
            this.cmbSteelType.Size = new System.Drawing.Size(186, 20);
            this.cmbSteelType.TabIndex = 1;
            this.cmbSteelType.SelectedIndexChanged += new System.EventHandler(this.cmbSteelType_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(19, 22);
            this.label6.Name = "label6";
            this.label6.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label6.Size = new System.Drawing.Size(55, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "鋼材タイプ";
            // 
            // cmbSteelSize
            // 
            this.cmbSteelSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSteelSize.FormattingEnabled = true;
            this.cmbSteelSize.Items.AddRange(new object[] {
            "H300X300X10X15",
            "20H　シュザイ",
            "35HA",
            "40HA"});
            this.cmbSteelSize.Location = new System.Drawing.Point(108, 47);
            this.cmbSteelSize.Name = "cmbSteelSize";
            this.cmbSteelSize.Size = new System.Drawing.Size(186, 20);
            this.cmbSteelSize.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(19, 51);
            this.label5.Name = "label5";
            this.label5.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label5.Size = new System.Drawing.Size(58, 12);
            this.label5.TabIndex = 2;
            this.label5.Text = "鋼材サイズ";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 151);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(107, 12);
            this.label7.TabIndex = 12;
            this.label7.Text = "終端側オフセット（－）";
            // 
            // txtEndOffset1
            // 
            this.txtEndOffset1.Location = new System.Drawing.Point(121, 148);
            this.txtEndOffset1.Name = "txtEndOffset1";
            this.txtEndOffset1.Size = new System.Drawing.Size(193, 19);
            this.txtEndOffset1.TabIndex = 13;
            this.txtEndOffset1.Text = "0";
            this.txtEndOffset1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(13, 254);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(78, 12);
            this.label8.TabIndex = 16;
            this.label8.Text = "サイズ(終点側)";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(13, 228);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(97, 12);
            this.label9.TabIndex = 14;
            this.label9.Text = "端部部品(終点側)";
            // 
            // cmbBuhinSizeE
            // 
            this.cmbBuhinSizeE.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBuhinSizeE.FormattingEnabled = true;
            this.cmbBuhinSizeE.Location = new System.Drawing.Point(121, 251);
            this.cmbBuhinSizeE.Name = "cmbBuhinSizeE";
            this.cmbBuhinSizeE.Size = new System.Drawing.Size(193, 20);
            this.cmbBuhinSizeE.TabIndex = 17;
            // 
            // cmbBuhinTypeE
            // 
            this.cmbBuhinTypeE.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBuhinTypeE.FormattingEnabled = true;
            this.cmbBuhinTypeE.Location = new System.Drawing.Point(121, 225);
            this.cmbBuhinTypeE.Name = "cmbBuhinTypeE";
            this.cmbBuhinTypeE.Size = new System.Drawing.Size(193, 20);
            this.cmbBuhinTypeE.TabIndex = 15;
            this.cmbBuhinTypeE.SelectedIndexChanged += new System.EventHandler(this.cmbBuhinTypeE_SelectedIndexChanged);
            // 
            // DlgCreateSyabariBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 364);
            this.ControlBox = false;
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.cmbBuhinSizeE);
            this.Controls.Add(this.cmbBuhinTypeE);
            this.Controls.Add(this.txtEndOffset1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbJackType2);
            this.Controls.Add(this.cmbJackType1);
            this.Controls.Add(this.cmbBuhinSizeS);
            this.Controls.Add(this.cmbBuhinTypeS);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgCreateSyabariBase";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "斜梁ベース作成";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.DlgCreateSyabari_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox cmbBuhinTypeS;
        private System.Windows.Forms.ComboBox cmbBuhinSizeS;
        private System.Windows.Forms.ComboBox cmbJackType1;
        private System.Windows.Forms.ComboBox cmbJackType2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox cmbSteelSize;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton rbnShoriTypeBaseLine;
        private System.Windows.Forms.RadioButton rbnShoriTypePtoP;
        private System.Windows.Forms.ComboBox cmbSteelType;
        private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox txtEndOffset1;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.ComboBox cmbBuhinSizeE;
		private System.Windows.Forms.ComboBox cmbBuhinTypeE;
	}
}