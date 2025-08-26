
namespace YMS.DLG
{
    partial class DlgCreateBracket
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
            this.rbnKiriBracket = new System.Windows.Forms.RadioButton();
            this.rbnKiriOsaeBracket = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.rbnHaraOsaeBracket = new System.Windows.Forms.RadioButton();
            this.rbnHaraBracket = new System.Windows.Forms.RadioButton();
            this.grpCreateMode = new System.Windows.Forms.GroupBox();
            this.rbnOptional = new System.Windows.Forms.RadioButton();
            this.rbnManual = new System.Windows.Forms.RadioButton();
            this.rbnAuto = new System.Windows.Forms.RadioButton();
            this.grpBracketType = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbBracketSize = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtProhibitionLength = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.grpCreateMode.SuspendLayout();
            this.grpBracketType.SuspendLayout();
            this.SuspendLayout();
            // 
            // rbnKiriBracket
            // 
            this.rbnKiriBracket.AutoSize = true;
            this.rbnKiriBracket.Checked = true;
            this.rbnKiriBracket.Location = new System.Drawing.Point(6, 15);
            this.rbnKiriBracket.Name = "rbnKiriBracket";
            this.rbnKiriBracket.Size = new System.Drawing.Size(88, 16);
            this.rbnKiriBracket.TabIndex = 0;
            this.rbnKiriBracket.TabStop = true;
            this.rbnKiriBracket.Text = "切梁ブラケット";
            this.rbnKiriBracket.UseVisualStyleBackColor = true;
            this.rbnKiriBracket.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // rbnKiriOsaeBracket
            // 
            this.rbnKiriOsaeBracket.AutoSize = true;
            this.rbnKiriOsaeBracket.Location = new System.Drawing.Point(97, 15);
            this.rbnKiriOsaeBracket.Name = "rbnKiriOsaeBracket";
            this.rbnKiriOsaeBracket.Size = new System.Drawing.Size(109, 16);
            this.rbnKiriOsaeBracket.TabIndex = 1;
            this.rbnKiriOsaeBracket.Text = "切梁押えブラケット";
            this.rbnKiriOsaeBracket.UseVisualStyleBackColor = true;
            this.rbnKiriOsaeBracket.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(16, 173);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(90, 23);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(112, 173);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // rbnHaraOsaeBracket
            // 
            this.rbnHaraOsaeBracket.AutoSize = true;
            this.rbnHaraOsaeBracket.Location = new System.Drawing.Point(97, 42);
            this.rbnHaraOsaeBracket.Name = "rbnHaraOsaeBracket";
            this.rbnHaraOsaeBracket.Size = new System.Drawing.Size(109, 16);
            this.rbnHaraOsaeBracket.TabIndex = 3;
            this.rbnHaraOsaeBracket.Text = "腹起押えブラケット";
            this.rbnHaraOsaeBracket.UseVisualStyleBackColor = true;
            this.rbnHaraOsaeBracket.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // rbnHaraBracket
            // 
            this.rbnHaraBracket.AutoSize = true;
            this.rbnHaraBracket.Location = new System.Drawing.Point(6, 42);
            this.rbnHaraBracket.Name = "rbnHaraBracket";
            this.rbnHaraBracket.Size = new System.Drawing.Size(88, 16);
            this.rbnHaraBracket.TabIndex = 2;
            this.rbnHaraBracket.Text = "腹起ブラケット";
            this.rbnHaraBracket.UseVisualStyleBackColor = true;
            this.rbnHaraBracket.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // grpCreateMode
            // 
            this.grpCreateMode.Controls.Add(this.rbnOptional);
            this.grpCreateMode.Controls.Add(this.rbnManual);
            this.grpCreateMode.Controls.Add(this.rbnAuto);
            this.grpCreateMode.Enabled = false;
            this.grpCreateMode.Location = new System.Drawing.Point(9, 73);
            this.grpCreateMode.Name = "grpCreateMode";
            this.grpCreateMode.Size = new System.Drawing.Size(217, 41);
            this.grpCreateMode.TabIndex = 1;
            this.grpCreateMode.TabStop = false;
            // 
            // rbnOptional
            // 
            this.rbnOptional.AutoSize = true;
            this.rbnOptional.Location = new System.Drawing.Point(163, 15);
            this.rbnOptional.Name = "rbnOptional";
            this.rbnOptional.Size = new System.Drawing.Size(47, 16);
            this.rbnOptional.TabIndex = 2;
            this.rbnOptional.Text = "任意";
            this.rbnOptional.UseVisualStyleBackColor = true;
            // 
            // rbnManual
            // 
            this.rbnManual.AutoSize = true;
            this.rbnManual.Checked = true;
            this.rbnManual.Location = new System.Drawing.Point(17, 15);
            this.rbnManual.Name = "rbnManual";
            this.rbnManual.Size = new System.Drawing.Size(47, 16);
            this.rbnManual.TabIndex = 1;
            this.rbnManual.TabStop = true;
            this.rbnManual.Text = "手動";
            this.rbnManual.UseVisualStyleBackColor = true;
            this.rbnManual.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // rbnAuto
            // 
            this.rbnAuto.AutoSize = true;
            this.rbnAuto.Location = new System.Drawing.Point(90, 15);
            this.rbnAuto.Name = "rbnAuto";
            this.rbnAuto.Size = new System.Drawing.Size(47, 16);
            this.rbnAuto.TabIndex = 0;
            this.rbnAuto.Text = "自動";
            this.rbnAuto.UseVisualStyleBackColor = true;
            this.rbnAuto.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // grpBracketType
            // 
            this.grpBracketType.Controls.Add(this.rbnKiriBracket);
            this.grpBracketType.Controls.Add(this.rbnHaraBracket);
            this.grpBracketType.Controls.Add(this.rbnHaraOsaeBracket);
            this.grpBracketType.Controls.Add(this.rbnKiriOsaeBracket);
            this.grpBracketType.Location = new System.Drawing.Point(9, 2);
            this.grpBracketType.Name = "grpBracketType";
            this.grpBracketType.Size = new System.Drawing.Size(217, 65);
            this.grpBracketType.TabIndex = 0;
            this.grpBracketType.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 151);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "ブラケットサイズ";
            // 
            // cmbBracketSize
            // 
            this.cmbBracketSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBracketSize.FormattingEnabled = true;
            this.cmbBracketSize.Location = new System.Drawing.Point(94, 147);
            this.cmbBracketSize.Name = "cmbBracketSize";
            this.cmbBracketSize.Size = new System.Drawing.Size(132, 20);
            this.cmbBracketSize.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 125);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "配置禁止距離";
            // 
            // txtProhibitionLength
            // 
            this.txtProhibitionLength.Location = new System.Drawing.Point(92, 122);
            this.txtProhibitionLength.Name = "txtProhibitionLength";
            this.txtProhibitionLength.Size = new System.Drawing.Size(100, 19);
            this.txtProhibitionLength.TabIndex = 3;
            this.txtProhibitionLength.Text = "0";
            this.txtProhibitionLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(203, 125);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "mm";
            // 
            // DlgCreateBracket
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(231, 201);
            this.ControlBox = false;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtProhibitionLength);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbBracketSize);
            this.Controls.Add(this.grpBracketType);
            this.Controls.Add(this.grpCreateMode);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgCreateBracket";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ブラケット作成";
            this.grpCreateMode.ResumeLayout(false);
            this.grpCreateMode.PerformLayout();
            this.grpBracketType.ResumeLayout(false);
            this.grpBracketType.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rbnKiriBracket;
        private System.Windows.Forms.RadioButton rbnKiriOsaeBracket;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.RadioButton rbnHaraOsaeBracket;
        private System.Windows.Forms.RadioButton rbnHaraBracket;
        private System.Windows.Forms.GroupBox grpCreateMode;
        private System.Windows.Forms.RadioButton rbnManual;
        private System.Windows.Forms.RadioButton rbnAuto;
        private System.Windows.Forms.GroupBox grpBracketType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbBracketSize;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtProhibitionLength;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton rbnOptional;
    }
}