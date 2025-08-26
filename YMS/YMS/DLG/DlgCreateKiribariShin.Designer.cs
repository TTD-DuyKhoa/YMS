
namespace YMS.DLG
{
    partial class DlgCreateKiribariShin
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cmbSteelSize = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbnShoriHohoPtoP = new System.Windows.Forms.RadioButton();
            this.rbnShoriHohoSingle = new System.Windows.Forms.RadioButton();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cmbSteelSize);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(201, 59);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "切梁情報";
            // 
            // cmbSteelSize
            // 
            this.cmbSteelSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSteelSize.FormattingEnabled = true;
            this.cmbSteelSize.Items.AddRange(new object[] {
            "20H　シュザイ"});
            this.cmbSteelSize.Location = new System.Drawing.Point(74, 24);
            this.cmbSteelSize.Name = "cmbSteelSize";
            this.cmbSteelSize.Size = new System.Drawing.Size(121, 20);
            this.cmbSteelSize.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 27);
            this.label4.Name = "label4";
            this.label4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label4.Size = new System.Drawing.Size(58, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "鋼材サイズ";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(12, 127);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(93, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK(O)";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(120, 127);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(93, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "キャンセル(C)";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbnShoriHohoPtoP);
            this.groupBox1.Controls.Add(this.rbnShoriHohoSingle);
            this.groupBox1.Location = new System.Drawing.Point(12, 77);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(208, 44);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "処理方法";
            // 
            // rbnShoriHohoPtoP
            // 
            this.rbnShoriHohoPtoP.AutoSize = true;
            this.rbnShoriHohoPtoP.Location = new System.Drawing.Point(108, 18);
            this.rbnShoriHohoPtoP.Name = "rbnShoriHohoPtoP";
            this.rbnShoriHohoPtoP.Size = new System.Drawing.Size(53, 16);
            this.rbnShoriHohoPtoP.TabIndex = 1;
            this.rbnShoriHohoPtoP.TabStop = true;
            this.rbnShoriHohoPtoP.Text = "2点間";
            this.rbnShoriHohoPtoP.UseVisualStyleBackColor = true;
            // 
            // rbnShoriHohoSingle
            // 
            this.rbnShoriHohoSingle.AutoSize = true;
            this.rbnShoriHohoSingle.Location = new System.Drawing.Point(18, 18);
            this.rbnShoriHohoSingle.Name = "rbnShoriHohoSingle";
            this.rbnShoriHohoSingle.Size = new System.Drawing.Size(41, 16);
            this.rbnShoriHohoSingle.TabIndex = 0;
            this.rbnShoriHohoSingle.TabStop = true;
            this.rbnShoriHohoSingle.Text = "1辺";
            this.rbnShoriHohoSingle.UseVisualStyleBackColor = true;
            // 
            // DlgCreateKiribariShin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(222, 156);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgCreateKiribariShin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "切梁ベース作成";
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox cmbSteelSize;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbnShoriHohoPtoP;
        private System.Windows.Forms.RadioButton rbnShoriHohoSingle;
    }
}