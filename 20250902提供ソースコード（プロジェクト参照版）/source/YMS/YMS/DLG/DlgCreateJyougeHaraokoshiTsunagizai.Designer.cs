
namespace YMS.DLG
{
    partial class DlgCreateJyougeHaraokoshiTsunagizai
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
            this.rbnKiteiSize = new System.Windows.Forms.RadioButton();
            this.rbnShiteiSize = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbSteelSize = new System.Windows.Forms.ComboBox();
            this.cmbKotei = new System.Windows.Forms.ComboBox();
            this.cmbBoltSize = new System.Windows.Forms.ComboBox();
            this.txtBoltNum = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbnKiteiSize);
            this.groupBox1.Controls.Add(this.rbnShiteiSize);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(250, 51);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "サイズ決定方法";
            // 
            // rbnKiteiSize
            // 
            this.rbnKiteiSize.AutoSize = true;
            this.rbnKiteiSize.Checked = true;
            this.rbnKiteiSize.Location = new System.Drawing.Point(30, 18);
            this.rbnKiteiSize.Name = "rbnKiteiSize";
            this.rbnKiteiSize.Size = new System.Drawing.Size(76, 16);
            this.rbnKiteiSize.TabIndex = 0;
            this.rbnKiteiSize.TabStop = true;
            this.rbnKiteiSize.Text = "規定サイズ";
            this.rbnKiteiSize.UseVisualStyleBackColor = true;
            // 
            // rbnShiteiSize
            // 
            this.rbnShiteiSize.AutoSize = true;
            this.rbnShiteiSize.Location = new System.Drawing.Point(135, 18);
            this.rbnShiteiSize.Name = "rbnShiteiSize";
            this.rbnShiteiSize.Size = new System.Drawing.Size(76, 16);
            this.rbnShiteiSize.TabIndex = 1;
            this.rbnShiteiSize.Text = "指定サイズ";
            this.rbnShiteiSize.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "鋼材サイズ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 106);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "固定方法";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 132);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "ボルト本数";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(231, 132);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "本";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 157);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "ボルトサイズ";
            // 
            // cmbSteelSize
            // 
            this.cmbSteelSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSteelSize.FormattingEnabled = true;
            this.cmbSteelSize.Items.AddRange(new object[] {
            "C-100X50X5"});
            this.cmbSteelSize.Location = new System.Drawing.Point(102, 77);
            this.cmbSteelSize.Name = "cmbSteelSize";
            this.cmbSteelSize.Size = new System.Drawing.Size(146, 20);
            this.cmbSteelSize.TabIndex = 2;
            // 
            // cmbKotei
            // 
            this.cmbKotei.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbKotei.FormattingEnabled = true;
            this.cmbKotei.Items.AddRange(new object[] {
            "溶接"});
            this.cmbKotei.Location = new System.Drawing.Point(102, 103);
            this.cmbKotei.Name = "cmbKotei";
            this.cmbKotei.Size = new System.Drawing.Size(146, 20);
            this.cmbKotei.TabIndex = 4;
            // 
            // cmbBoltSize
            // 
            this.cmbBoltSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBoltSize.FormattingEnabled = true;
            this.cmbBoltSize.Location = new System.Drawing.Point(102, 154);
            this.cmbBoltSize.Name = "cmbBoltSize";
            this.cmbBoltSize.Size = new System.Drawing.Size(146, 20);
            this.cmbBoltSize.TabIndex = 9;
            // 
            // txtBoltNum
            // 
            this.txtBoltNum.Location = new System.Drawing.Point(133, 129);
            this.txtBoltNum.Name = "txtBoltNum";
            this.txtBoltNum.Size = new System.Drawing.Size(92, 19);
            this.txtBoltNum.TabIndex = 6;
            this.txtBoltNum.Text = "0";
            this.txtBoltNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(60, 180);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(98, 23);
            this.btnOK.TabIndex = 10;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(164, 180);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(98, 23);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // DlgCreateJyougeHaraokoshiTsunagizai
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(275, 212);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtBoltNum);
            this.Controls.Add(this.cmbBoltSize);
            this.Controls.Add(this.cmbKotei);
            this.Controls.Add(this.cmbSteelSize);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgCreateJyougeHaraokoshiTsunagizai";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "上下腹起ツナギ材作成";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbnKiteiSize;
        private System.Windows.Forms.RadioButton rbnShiteiSize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbSteelSize;
        private System.Windows.Forms.ComboBox cmbKotei;
        private System.Windows.Forms.ComboBox cmbBoltSize;
        private System.Windows.Forms.TextBox txtBoltNum;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}