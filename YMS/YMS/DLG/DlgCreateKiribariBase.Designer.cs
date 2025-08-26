
namespace YMS.DLG
{
    partial class DlgCreateKiribariBase
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
            this.cmbTanbuPartsS = new System.Windows.Forms.ComboBox();
            this.cmbTanbuPartsE = new System.Windows.Forms.ComboBox();
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
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbTanbuPartsS
            // 
            this.cmbTanbuPartsS.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTanbuPartsS.FormattingEnabled = true;
            this.cmbTanbuPartsS.Location = new System.Drawing.Point(120, 151);
            this.cmbTanbuPartsS.Name = "cmbTanbuPartsS";
            this.cmbTanbuPartsS.Size = new System.Drawing.Size(193, 20);
            this.cmbTanbuPartsS.TabIndex = 3;
            // 
            // cmbTanbuPartsE
            // 
            this.cmbTanbuPartsE.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTanbuPartsE.FormattingEnabled = true;
            this.cmbTanbuPartsE.Location = new System.Drawing.Point(120, 177);
            this.cmbTanbuPartsE.Name = "cmbTanbuPartsE";
            this.cmbTanbuPartsE.Size = new System.Drawing.Size(193, 20);
            this.cmbTanbuPartsE.TabIndex = 5;
            // 
            // cmbJackType1
            // 
            this.cmbJackType1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbJackType1.FormattingEnabled = true;
            this.cmbJackType1.Location = new System.Drawing.Point(120, 203);
            this.cmbJackType1.Name = "cmbJackType1";
            this.cmbJackType1.Size = new System.Drawing.Size(193, 20);
            this.cmbJackType1.TabIndex = 7;
            // 
            // cmbJackType2
            // 
            this.cmbJackType2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbJackType2.FormattingEnabled = true;
            this.cmbJackType2.Location = new System.Drawing.Point(120, 229);
            this.cmbJackType2.Name = "cmbJackType2";
            this.cmbJackType2.Size = new System.Drawing.Size(193, 20);
            this.cmbJackType2.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 154);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "端部部品(始点側)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 180);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "端部部品(終点側)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 206);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "ジャッキタイプ(１)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 232);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "ジャッキタイプ(２)";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(118, 256);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(94, 23);
            this.btnOK.TabIndex = 10;
            this.btnOK.Text = "OK(O)";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(218, 256);
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
            this.groupBox3.Text = "切梁情報";
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
            // DlgCreateKiribariBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 291);
            this.ControlBox = false;
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
            this.Controls.Add(this.cmbTanbuPartsE);
            this.Controls.Add(this.cmbTanbuPartsS);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgCreateKiribariBase";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "切梁ベース作成";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.DlgCreateKiribari_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox cmbTanbuPartsS;
        private System.Windows.Forms.ComboBox cmbTanbuPartsE;
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
    }
}