
namespace YMS.DLG
{
    partial class DlgCreateSyabariHiuchiBase
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
            this.pnlSakuseiHouhou = new System.Windows.Forms.Panel();
            this.rbnHaichiCreateHohoAnyLengthManual = new System.Windows.Forms.RadioButton();
            this.rbnHaichiCreateHohoBoth = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmbSteelType = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cmbSteelSize = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtHiuchiLengthL = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtAngle = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.cmbBuhinSize2 = new System.Windows.Forms.ComboBox();
            this.lblBuhinSize2 = new System.Windows.Forms.Label();
            this.cmbBuhinType2 = new System.Windows.Forms.ComboBox();
            this.lblBuhinType2 = new System.Windows.Forms.Label();
            this.cmbBuhinSize1 = new System.Windows.Forms.ComboBox();
            this.lblBuhinSize1 = new System.Windows.Forms.Label();
            this.cmbBuhinType1 = new System.Windows.Forms.ComboBox();
            this.lblBuhinType1 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.pnlSakuseiHouhou.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pnlSakuseiHouhou);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(433, 55);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "配置方法";
            // 
            // pnlSakuseiHouhou
            // 
            this.pnlSakuseiHouhou.Controls.Add(this.rbnHaichiCreateHohoAnyLengthManual);
            this.pnlSakuseiHouhou.Controls.Add(this.rbnHaichiCreateHohoBoth);
            this.pnlSakuseiHouhou.Location = new System.Drawing.Point(77, 18);
            this.pnlSakuseiHouhou.Name = "pnlSakuseiHouhou";
            this.pnlSakuseiHouhou.Size = new System.Drawing.Size(342, 25);
            this.pnlSakuseiHouhou.TabIndex = 5;
            // 
            // rbnHaichiCreateHohoAnyLengthManual
            // 
            this.rbnHaichiCreateHohoAnyLengthManual.AutoSize = true;
            this.rbnHaichiCreateHohoAnyLengthManual.Location = new System.Drawing.Point(97, 6);
            this.rbnHaichiCreateHohoAnyLengthManual.Name = "rbnHaichiCreateHohoAnyLengthManual";
            this.rbnHaichiCreateHohoAnyLengthManual.Size = new System.Drawing.Size(99, 16);
            this.rbnHaichiCreateHohoAnyLengthManual.TabIndex = 1;
            this.rbnHaichiCreateHohoAnyLengthManual.Text = "片方(長さ入力)";
            this.rbnHaichiCreateHohoAnyLengthManual.UseVisualStyleBackColor = true;
            this.rbnHaichiCreateHohoAnyLengthManual.CheckedChanged += new System.EventHandler(this.rbnHaichiCreateHohoAnyLengthManual_CheckedChanged);
            // 
            // rbnHaichiCreateHohoBoth
            // 
            this.rbnHaichiCreateHohoBoth.AutoSize = true;
            this.rbnHaichiCreateHohoBoth.Checked = true;
            this.rbnHaichiCreateHohoBoth.Location = new System.Drawing.Point(14, 6);
            this.rbnHaichiCreateHohoBoth.Name = "rbnHaichiCreateHohoBoth";
            this.rbnHaichiCreateHohoBoth.Size = new System.Drawing.Size(47, 16);
            this.rbnHaichiCreateHohoBoth.TabIndex = 0;
            this.rbnHaichiCreateHohoBoth.TabStop = true;
            this.rbnHaichiCreateHohoBoth.Text = "両方";
            this.rbnHaichiCreateHohoBoth.UseVisualStyleBackColor = true;
            this.rbnHaichiCreateHohoBoth.CheckedChanged += new System.EventHandler(this.rbnHaichiCreateHohoBoth_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "作成方法";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cmbSteelType);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.cmbSteelSize);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.txtHiuchiLengthL);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Location = new System.Drawing.Point(13, 73);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(433, 96);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "作成方法";
            // 
            // cmbSteelType
            // 
            this.cmbSteelType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSteelType.FormattingEnabled = true;
            this.cmbSteelType.Items.AddRange(new object[] {
            "35HA　シュザイ"});
            this.cmbSteelType.Location = new System.Drawing.Point(256, 14);
            this.cmbSteelType.Name = "cmbSteelType";
            this.cmbSteelType.Size = new System.Drawing.Size(162, 20);
            this.cmbSteelType.TabIndex = 4;
            this.cmbSteelType.SelectedIndexChanged += new System.EventHandler(this.cmbSteelType_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(15, 17);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(55, 12);
            this.label9.TabIndex = 3;
            this.label9.Text = "鋼材タイプ";
            // 
            // cmbSteelSize
            // 
            this.cmbSteelSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSteelSize.FormattingEnabled = true;
            this.cmbSteelSize.Items.AddRange(new object[] {
            "35HA　シュザイ"});
            this.cmbSteelSize.Location = new System.Drawing.Point(256, 40);
            this.cmbSteelSize.Name = "cmbSteelSize";
            this.cmbSteelSize.Size = new System.Drawing.Size(162, 20);
            this.cmbSteelSize.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 43);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 12);
            this.label5.TabIndex = 5;
            this.label5.Text = "鋼材サイズ";
            // 
            // txtHiuchiLengthL
            // 
            this.txtHiuchiLengthL.Location = new System.Drawing.Point(256, 66);
            this.txtHiuchiLengthL.Name = "txtHiuchiLengthL";
            this.txtHiuchiLengthL.Size = new System.Drawing.Size(162, 19);
            this.txtHiuchiLengthL.TabIndex = 12;
            this.txtHiuchiLengthL.Text = "1000";
            this.txtHiuchiLengthL.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(15, 69);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(55, 12);
            this.label8.TabIndex = 11;
            this.label8.Text = "火打長さL";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtAngle);
            this.groupBox3.Controls.Add(this.label16);
            this.groupBox3.Controls.Add(this.cmbBuhinSize2);
            this.groupBox3.Controls.Add(this.lblBuhinSize2);
            this.groupBox3.Controls.Add(this.cmbBuhinType2);
            this.groupBox3.Controls.Add(this.lblBuhinType2);
            this.groupBox3.Controls.Add(this.cmbBuhinSize1);
            this.groupBox3.Controls.Add(this.lblBuhinSize1);
            this.groupBox3.Controls.Add(this.cmbBuhinType1);
            this.groupBox3.Controls.Add(this.lblBuhinType1);
            this.groupBox3.Location = new System.Drawing.Point(13, 175);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(433, 150);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "部品";
            // 
            // txtAngle
            // 
            this.txtAngle.Location = new System.Drawing.Point(257, 120);
            this.txtAngle.Name = "txtAngle";
            this.txtAngle.Size = new System.Drawing.Size(162, 19);
            this.txtAngle.TabIndex = 9;
            this.txtAngle.Text = "30.00";
            this.txtAngle.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(16, 123);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(45, 12);
            this.label16.TabIndex = 8;
            this.label16.Text = "角度(R)";
            // 
            // cmbBuhinSize2
            // 
            this.cmbBuhinSize2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBuhinSize2.FormattingEnabled = true;
            this.cmbBuhinSize2.Items.AddRange(new object[] {
            "35VP ヒウチウケ"});
            this.cmbBuhinSize2.Location = new System.Drawing.Point(256, 94);
            this.cmbBuhinSize2.Name = "cmbBuhinSize2";
            this.cmbBuhinSize2.Size = new System.Drawing.Size(163, 20);
            this.cmbBuhinSize2.TabIndex = 7;
            // 
            // lblBuhinSize2
            // 
            this.lblBuhinSize2.AutoSize = true;
            this.lblBuhinSize2.Location = new System.Drawing.Point(16, 96);
            this.lblBuhinSize2.Name = "lblBuhinSize2";
            this.lblBuhinSize2.Size = new System.Drawing.Size(102, 12);
            this.lblBuhinSize2.TabIndex = 6;
            this.lblBuhinSize2.Text = "部品サイズ(腹起側)";
            // 
            // cmbBuhinType2
            // 
            this.cmbBuhinType2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBuhinType2.FormattingEnabled = true;
            this.cmbBuhinType2.Items.AddRange(new object[] {
            "30度火打受ピース",
            "火打受ピース"});
            this.cmbBuhinType2.Location = new System.Drawing.Point(256, 66);
            this.cmbBuhinType2.Name = "cmbBuhinType2";
            this.cmbBuhinType2.Size = new System.Drawing.Size(163, 20);
            this.cmbBuhinType2.TabIndex = 5;
            this.cmbBuhinType2.SelectedIndexChanged += new System.EventHandler(this.cmbBuhinType2_SelectedIndexChanged);
            // 
            // lblBuhinType2
            // 
            this.lblBuhinType2.AutoSize = true;
            this.lblBuhinType2.Location = new System.Drawing.Point(16, 69);
            this.lblBuhinType2.Name = "lblBuhinType2";
            this.lblBuhinType2.Size = new System.Drawing.Size(99, 12);
            this.lblBuhinType2.TabIndex = 4;
            this.lblBuhinType2.Text = "部品タイプ(腹起側)";
            // 
            // cmbBuhinSize1
            // 
            this.cmbBuhinSize1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBuhinSize1.FormattingEnabled = true;
            this.cmbBuhinSize1.Items.AddRange(new object[] {
            "35VP ヒウチウケ"});
            this.cmbBuhinSize1.Location = new System.Drawing.Point(256, 39);
            this.cmbBuhinSize1.Name = "cmbBuhinSize1";
            this.cmbBuhinSize1.Size = new System.Drawing.Size(163, 20);
            this.cmbBuhinSize1.TabIndex = 3;
            // 
            // lblBuhinSize1
            // 
            this.lblBuhinSize1.AutoSize = true;
            this.lblBuhinSize1.Location = new System.Drawing.Point(16, 41);
            this.lblBuhinSize1.Name = "lblBuhinSize1";
            this.lblBuhinSize1.Size = new System.Drawing.Size(102, 12);
            this.lblBuhinSize1.TabIndex = 2;
            this.lblBuhinSize1.Text = "部品サイズ(斜梁側)";
            // 
            // cmbBuhinType1
            // 
            this.cmbBuhinType1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBuhinType1.FormattingEnabled = true;
            this.cmbBuhinType1.Items.AddRange(new object[] {
            "30度火打受ピース",
            "火打受ピース"});
            this.cmbBuhinType1.Location = new System.Drawing.Point(256, 12);
            this.cmbBuhinType1.Name = "cmbBuhinType1";
            this.cmbBuhinType1.Size = new System.Drawing.Size(163, 20);
            this.cmbBuhinType1.TabIndex = 1;
            this.cmbBuhinType1.SelectedIndexChanged += new System.EventHandler(this.cmbBuhinType1_SelectedIndexChanged);
            // 
            // lblBuhinType1
            // 
            this.lblBuhinType1.AutoSize = true;
            this.lblBuhinType1.Location = new System.Drawing.Point(16, 15);
            this.lblBuhinType1.Name = "lblBuhinType1";
            this.lblBuhinType1.Size = new System.Drawing.Size(99, 12);
            this.lblBuhinType1.TabIndex = 0;
            this.lblBuhinType1.Text = "部品タイプ(斜梁側)";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(240, 331);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(90, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK(O)";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(356, 331);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "キャンセル(C)";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // DlgCreateSyabariHiuchiBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(458, 363);
            this.ControlBox = false;
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgCreateSyabariHiuchiBase";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "斜梁火打ベース作成";
            this.TopMost = true;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.pnlSakuseiHouhou.ResumeLayout(false);
            this.pnlSakuseiHouhou.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel pnlSakuseiHouhou;
        private System.Windows.Forms.RadioButton rbnHaichiCreateHohoAnyLengthManual;
        private System.Windows.Forms.RadioButton rbnHaichiCreateHohoBoth;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cmbSteelSize;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtHiuchiLengthL;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtAngle;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ComboBox cmbBuhinSize2;
        private System.Windows.Forms.Label lblBuhinSize2;
        private System.Windows.Forms.ComboBox cmbBuhinType2;
        private System.Windows.Forms.Label lblBuhinType2;
        private System.Windows.Forms.ComboBox cmbBuhinSize1;
        private System.Windows.Forms.Label lblBuhinSize1;
        private System.Windows.Forms.ComboBox cmbBuhinType1;
        private System.Windows.Forms.Label lblBuhinType1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cmbSteelType;
        private System.Windows.Forms.Label label9;
    }
}