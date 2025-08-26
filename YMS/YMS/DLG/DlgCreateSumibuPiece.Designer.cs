
namespace YMS.DLG
{
    partial class DlgCreateSumibuPiece
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
            this.rbnCreateTypeAuto = new System.Windows.Forms.RadioButton();
            this.rbnCreateTypeManual = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtAidaTsumeRyo = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbSumibuPeaceSize = new System.Windows.Forms.ComboBox();
            this.chkSumibuPeaceSize = new System.Windows.Forms.CheckBox();
            this.cmbSumibuPeaceType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbnCreateTypeAuto);
            this.groupBox1.Controls.Add(this.rbnCreateTypeManual);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(281, 48);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "作成方法";
            // 
            // rbnCreateTypeAuto
            // 
            this.rbnCreateTypeAuto.AutoSize = true;
            this.rbnCreateTypeAuto.Checked = true;
            this.rbnCreateTypeAuto.Location = new System.Drawing.Point(29, 18);
            this.rbnCreateTypeAuto.Name = "rbnCreateTypeAuto";
            this.rbnCreateTypeAuto.Size = new System.Drawing.Size(47, 16);
            this.rbnCreateTypeAuto.TabIndex = 0;
            this.rbnCreateTypeAuto.TabStop = true;
            this.rbnCreateTypeAuto.Text = "自動";
            this.rbnCreateTypeAuto.UseVisualStyleBackColor = true;
            // 
            // rbnCreateTypeManual
            // 
            this.rbnCreateTypeManual.AutoSize = true;
            this.rbnCreateTypeManual.Location = new System.Drawing.Point(106, 18);
            this.rbnCreateTypeManual.Name = "rbnCreateTypeManual";
            this.rbnCreateTypeManual.Size = new System.Drawing.Size(47, 16);
            this.rbnCreateTypeManual.TabIndex = 1;
            this.rbnCreateTypeManual.Text = "手動";
            this.rbnCreateTypeManual.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtAidaTsumeRyo);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.cmbSumibuPeaceSize);
            this.groupBox2.Controls.Add(this.chkSumibuPeaceSize);
            this.groupBox2.Controls.Add(this.cmbSumibuPeaceType);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(12, 66);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(281, 108);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "隅部ピース情報";
            // 
            // txtAidaTsumeRyo
            // 
            this.txtAidaTsumeRyo.Location = new System.Drawing.Point(140, 78);
            this.txtAidaTsumeRyo.Name = "txtAidaTsumeRyo";
            this.txtAidaTsumeRyo.Size = new System.Drawing.Size(100, 19);
            this.txtAidaTsumeRyo.TabIndex = 5;
            this.txtAidaTsumeRyo.Text = "0";
            this.txtAidaTsumeRyo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(246, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "mm";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(74, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "間詰め量";
            // 
            // cmbSumibuPeaceSize
            // 
            this.cmbSumibuPeaceSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSumibuPeaceSize.Enabled = false;
            this.cmbSumibuPeaceSize.FormattingEnabled = true;
            this.cmbSumibuPeaceSize.Location = new System.Drawing.Point(140, 52);
            this.cmbSumibuPeaceSize.Name = "cmbSumibuPeaceSize";
            this.cmbSumibuPeaceSize.Size = new System.Drawing.Size(129, 20);
            this.cmbSumibuPeaceSize.TabIndex = 3;
            // 
            // chkSumibuPeaceSize
            // 
            this.chkSumibuPeaceSize.AutoSize = true;
            this.chkSumibuPeaceSize.Location = new System.Drawing.Point(29, 56);
            this.chkSumibuPeaceSize.Name = "chkSumibuPeaceSize";
            this.chkSumibuPeaceSize.Size = new System.Drawing.Size(105, 16);
            this.chkSumibuPeaceSize.TabIndex = 2;
            this.chkSumibuPeaceSize.Text = "サイズを指定する";
            this.chkSumibuPeaceSize.UseVisualStyleBackColor = true;
            this.chkSumibuPeaceSize.CheckedChanged += new System.EventHandler(this.chkSumibuPeaceSize_CheckedChanged);
            // 
            // cmbSumibuPeaceType
            // 
            this.cmbSumibuPeaceType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSumibuPeaceType.FormattingEnabled = true;
            this.cmbSumibuPeaceType.Items.AddRange(new object[] {
            "隅部ピース(CN)"});
            this.cmbSumibuPeaceType.Location = new System.Drawing.Point(140, 26);
            this.cmbSumibuPeaceType.Name = "cmbSumibuPeaceType";
            this.cmbSumibuPeaceType.Size = new System.Drawing.Size(129, 20);
            this.cmbSumibuPeaceType.TabIndex = 1;
            this.cmbSumibuPeaceType.SelectedIndexChanged += new System.EventHandler(this.cmbSumibuPeaceType_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "タイプ";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(113, 180);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(87, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(206, 180);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(87, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // DlgCreateSumibuPiece
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(305, 211);
            this.ControlBox = false;
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgCreateSumibuPiece";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "隅部ピース作成";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbnCreateTypeAuto;
        private System.Windows.Forms.RadioButton rbnCreateTypeManual;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtAidaTsumeRyo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbSumibuPeaceSize;
        private System.Windows.Forms.CheckBox chkSumibuPeaceSize;
        private System.Windows.Forms.ComboBox cmbSumibuPeaceType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}