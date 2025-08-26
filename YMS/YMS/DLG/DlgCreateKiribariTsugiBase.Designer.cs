
namespace YMS.DLG
{
    partial class DlgCreateKiribariTsugiBase
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtKiriSideTsugiLength = new System.Windows.Forms.TextBox();
            this.cmbKiriSideParts = new System.Windows.Forms.ComboBox();
            this.cmbKiriSideSteelSizeDouble = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbSteelSizeSingle = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpKiribariHaraokoshi = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtHaraSideTsugiLength = new System.Windows.Forms.TextBox();
            this.cmbHaraSideParts = new System.Windows.Forms.ComboBox();
            this.cmbHaraSideSteelSizeDouble = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.rbnKoseSingle = new System.Windows.Forms.RadioButton();
            this.rbnKoseDouble = new System.Windows.Forms.RadioButton();
            this.rbnShoriTypeSanjikuPeace = new System.Windows.Forms.RadioButton();
            this.rbnShoriTypePoint = new System.Windows.Forms.RadioButton();
            this.rbnShoriTypeReplace = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox2.SuspendLayout();
            this.grpKiribariHaraokoshi.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "鋼材タイプ";
            // 
            // cmbSteelType
            // 
            this.cmbSteelType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSteelType.FormattingEnabled = true;
            this.cmbSteelType.Items.AddRange(new object[] {
            "主材"});
            this.cmbSteelType.Location = new System.Drawing.Point(80, 26);
            this.cmbSteelType.Name = "cmbSteelType";
            this.cmbSteelType.Size = new System.Drawing.Size(126, 20);
            this.cmbSteelType.TabIndex = 1;
            this.cmbSteelType.SelectedIndexChanged += new System.EventHandler(this.cmbSteelType_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.txtKiriSideTsugiLength);
            this.groupBox2.Controls.Add(this.cmbKiriSideParts);
            this.groupBox2.Controls.Add(this.cmbKiriSideSteelSizeDouble);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(8, 68);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(251, 119);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "切梁側";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(217, 61);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(23, 12);
            this.label5.TabIndex = 4;
            this.label5.Text = "mm";
            // 
            // txtKiriSideTsugiLength
            // 
            this.txtKiriSideTsugiLength.Location = new System.Drawing.Point(125, 58);
            this.txtKiriSideTsugiLength.Name = "txtKiriSideTsugiLength";
            this.txtKiriSideTsugiLength.Size = new System.Drawing.Size(86, 19);
            this.txtKiriSideTsugiLength.TabIndex = 3;
            this.txtKiriSideTsugiLength.Text = "1000";
            this.txtKiriSideTsugiLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // cmbKiriSideParts
            // 
            this.cmbKiriSideParts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbKiriSideParts.FormattingEnabled = true;
            this.cmbKiriSideParts.Items.AddRange(new object[] {
            "SHG"});
            this.cmbKiriSideParts.Location = new System.Drawing.Point(125, 84);
            this.cmbKiriSideParts.Name = "cmbKiriSideParts";
            this.cmbKiriSideParts.Size = new System.Drawing.Size(120, 20);
            this.cmbKiriSideParts.TabIndex = 6;
            // 
            // cmbKiriSideSteelSizeDouble
            // 
            this.cmbKiriSideSteelSizeDouble.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbKiriSideSteelSizeDouble.Enabled = false;
            this.cmbKiriSideSteelSizeDouble.FormattingEnabled = true;
            this.cmbKiriSideSteelSizeDouble.Location = new System.Drawing.Point(125, 31);
            this.cmbKiriSideSteelSizeDouble.Name = "cmbKiriSideSteelSizeDouble";
            this.cmbKiriSideSteelSizeDouble.Size = new System.Drawing.Size(120, 20);
            this.cmbKiriSideSteelSizeDouble.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 61);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "長さ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 87);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "切梁側部品";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "鋼材サイズ(ダブル)";
            // 
            // cmbSteelSizeSingle
            // 
            this.cmbSteelSizeSingle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSteelSizeSingle.FormattingEnabled = true;
            this.cmbSteelSizeSingle.Items.AddRange(new object[] {
            "30HA　シュザイ"});
            this.cmbSteelSizeSingle.Location = new System.Drawing.Point(378, 36);
            this.cmbSteelSizeSingle.Name = "cmbSteelSizeSingle";
            this.cmbSteelSizeSingle.Size = new System.Drawing.Size(137, 20);
            this.cmbSteelSizeSingle.TabIndex = 2;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(268, 40);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(104, 12);
            this.label11.TabIndex = 1;
            this.label11.Text = "鋼材サイズ(シングル)";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(332, 256);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(94, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK(O)";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(452, 256);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(94, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "キャンセル(C)";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // grpKiribariHaraokoshi
            // 
            this.grpKiribariHaraokoshi.Controls.Add(this.groupBox3);
            this.grpKiribariHaraokoshi.Controls.Add(this.groupBox6);
            this.grpKiribariHaraokoshi.Controls.Add(this.groupBox2);
            this.grpKiribariHaraokoshi.Controls.Add(this.cmbSteelSizeSingle);
            this.grpKiribariHaraokoshi.Controls.Add(this.label11);
            this.grpKiribariHaraokoshi.Location = new System.Drawing.Point(12, 52);
            this.grpKiribariHaraokoshi.Name = "grpKiribariHaraokoshi";
            this.grpKiribariHaraokoshi.Size = new System.Drawing.Size(534, 198);
            this.grpKiribariHaraokoshi.TabIndex = 3;
            this.grpKiribariHaraokoshi.TabStop = false;
            this.grpKiribariHaraokoshi.Text = "切梁-腹起(切梁)";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.txtHaraSideTsugiLength);
            this.groupBox3.Controls.Add(this.cmbHaraSideParts);
            this.groupBox3.Controls.Add(this.cmbHaraSideSteelSizeDouble);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Location = new System.Drawing.Point(270, 68);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(251, 119);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "腹起側";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(217, 61);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(23, 12);
            this.label8.TabIndex = 4;
            this.label8.Text = "mm";
            // 
            // txtHaraSideTsugiLength
            // 
            this.txtHaraSideTsugiLength.Location = new System.Drawing.Point(125, 58);
            this.txtHaraSideTsugiLength.Name = "txtHaraSideTsugiLength";
            this.txtHaraSideTsugiLength.Size = new System.Drawing.Size(86, 19);
            this.txtHaraSideTsugiLength.TabIndex = 3;
            this.txtHaraSideTsugiLength.Text = "1000";
            this.txtHaraSideTsugiLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // cmbHaraSideParts
            // 
            this.cmbHaraSideParts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbHaraSideParts.FormattingEnabled = true;
            this.cmbHaraSideParts.Items.AddRange(new object[] {
            "自在火打受ピース"});
            this.cmbHaraSideParts.Location = new System.Drawing.Point(125, 84);
            this.cmbHaraSideParts.Name = "cmbHaraSideParts";
            this.cmbHaraSideParts.Size = new System.Drawing.Size(120, 20);
            this.cmbHaraSideParts.TabIndex = 6;
            // 
            // cmbHaraSideSteelSizeDouble
            // 
            this.cmbHaraSideSteelSizeDouble.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbHaraSideSteelSizeDouble.Enabled = false;
            this.cmbHaraSideSteelSizeDouble.FormattingEnabled = true;
            this.cmbHaraSideSteelSizeDouble.Location = new System.Drawing.Point(125, 31);
            this.cmbHaraSideSteelSizeDouble.Name = "cmbHaraSideSteelSizeDouble";
            this.cmbHaraSideSteelSizeDouble.Size = new System.Drawing.Size(120, 20);
            this.cmbHaraSideSteelSizeDouble.TabIndex = 1;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 61);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(25, 12);
            this.label9.TabIndex = 2;
            this.label9.Text = "長さ";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 87);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(65, 12);
            this.label10.TabIndex = 5;
            this.label10.Text = "腹起側部品";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 34);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(94, 12);
            this.label12.TabIndex = 0;
            this.label12.Text = "鋼材サイズ(ダブル)";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.rbnKoseSingle);
            this.groupBox6.Controls.Add(this.rbnKoseDouble);
            this.groupBox6.Location = new System.Drawing.Point(10, 18);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(200, 47);
            this.groupBox6.TabIndex = 0;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "構成";
            // 
            // rbnKoseSingle
            // 
            this.rbnKoseSingle.AutoSize = true;
            this.rbnKoseSingle.Checked = true;
            this.rbnKoseSingle.Location = new System.Drawing.Point(29, 18);
            this.rbnKoseSingle.Name = "rbnKoseSingle";
            this.rbnKoseSingle.Size = new System.Drawing.Size(61, 16);
            this.rbnKoseSingle.TabIndex = 0;
            this.rbnKoseSingle.TabStop = true;
            this.rbnKoseSingle.Text = "シングル";
            this.rbnKoseSingle.UseVisualStyleBackColor = true;
            this.rbnKoseSingle.CheckedChanged += new System.EventHandler(this.rbnKoseSingle_CheckedChanged);
            // 
            // rbnKoseDouble
            // 
            this.rbnKoseDouble.AutoSize = true;
            this.rbnKoseDouble.Location = new System.Drawing.Point(123, 18);
            this.rbnKoseDouble.Name = "rbnKoseDouble";
            this.rbnKoseDouble.Size = new System.Drawing.Size(51, 16);
            this.rbnKoseDouble.TabIndex = 1;
            this.rbnKoseDouble.Text = "ダブル";
            this.rbnKoseDouble.UseVisualStyleBackColor = true;
            this.rbnKoseDouble.CheckedChanged += new System.EventHandler(this.rbnKoseDouble_CheckedChanged);
            // 
            // rbnShoriTypeSanjikuPeace
            // 
            this.rbnShoriTypeSanjikuPeace.AutoSize = true;
            this.rbnShoriTypeSanjikuPeace.Location = new System.Drawing.Point(83, 18);
            this.rbnShoriTypeSanjikuPeace.Name = "rbnShoriTypeSanjikuPeace";
            this.rbnShoriTypeSanjikuPeace.Size = new System.Drawing.Size(75, 16);
            this.rbnShoriTypeSanjikuPeace.TabIndex = 1;
            this.rbnShoriTypeSanjikuPeace.TabStop = true;
            this.rbnShoriTypeSanjikuPeace.Text = "三軸ピース";
            this.rbnShoriTypeSanjikuPeace.UseVisualStyleBackColor = true;
            // 
            // rbnShoriTypePoint
            // 
            this.rbnShoriTypePoint.AutoSize = true;
            this.rbnShoriTypePoint.Checked = true;
            this.rbnShoriTypePoint.Location = new System.Drawing.Point(6, 18);
            this.rbnShoriTypePoint.Name = "rbnShoriTypePoint";
            this.rbnShoriTypePoint.Size = new System.Drawing.Size(71, 16);
            this.rbnShoriTypePoint.TabIndex = 0;
            this.rbnShoriTypePoint.TabStop = true;
            this.rbnShoriTypePoint.Text = "座標指定";
            this.rbnShoriTypePoint.UseVisualStyleBackColor = true;
            // 
            // rbnShoriTypeReplace
            // 
            this.rbnShoriTypeReplace.AutoSize = true;
            this.rbnShoriTypeReplace.Location = new System.Drawing.Point(164, 18);
            this.rbnShoriTypeReplace.Name = "rbnShoriTypeReplace";
            this.rbnShoriTypeReplace.Size = new System.Drawing.Size(47, 16);
            this.rbnShoriTypeReplace.TabIndex = 2;
            this.rbnShoriTypeReplace.TabStop = true;
            this.rbnShoriTypeReplace.Text = "置換";
            this.rbnShoriTypeReplace.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rbnShoriTypePoint);
            this.groupBox4.Controls.Add(this.rbnShoriTypeReplace);
            this.groupBox4.Controls.Add(this.rbnShoriTypeSanjikuPeace);
            this.groupBox4.Location = new System.Drawing.Point(235, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(217, 42);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "処理方法";
            // 
            // DlgCreateKiribariTsugiBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(556, 290);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.grpKiribariHaraokoshi);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.cmbSteelType);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgCreateKiribariTsugiBase";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "切梁継ぎベース";
            this.TopMost = true;
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.grpKiribariHaraokoshi.ResumeLayout(false);
            this.grpKiribariHaraokoshi.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbSteelType;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtKiriSideTsugiLength;
        private System.Windows.Forms.ComboBox cmbKiriSideParts;
        private System.Windows.Forms.ComboBox cmbKiriSideSteelSizeDouble;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbSteelSizeSingle;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox grpKiribariHaraokoshi;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.RadioButton rbnKoseSingle;
        private System.Windows.Forms.RadioButton rbnKoseDouble;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtHaraSideTsugiLength;
        private System.Windows.Forms.ComboBox cmbHaraSideParts;
        private System.Windows.Forms.ComboBox cmbHaraSideSteelSizeDouble;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.RadioButton rbnShoriTypeSanjikuPeace;
        private System.Windows.Forms.RadioButton rbnShoriTypePoint;
        private System.Windows.Forms.RadioButton rbnShoriTypeReplace;
        private System.Windows.Forms.GroupBox groupBox4;
    }
}