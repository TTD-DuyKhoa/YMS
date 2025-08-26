namespace YMS.DLG
{
    partial class DlgCreateJack
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
            this.txtOffset = new System.Windows.Forms.TextBox();
            this.cmbJackType = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.chkUseJackCover = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbTensyo = new System.Windows.Forms.ComboBox();
            this.chkUseOffset = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "ジャッキタイプ";
            // 
            // txtOffset
            // 
            this.txtOffset.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtOffset.Location = new System.Drawing.Point(146, 18);
            this.txtOffset.Name = "txtOffset";
            this.txtOffset.ShortcutsEnabled = false;
            this.txtOffset.Size = new System.Drawing.Size(100, 19);
            this.txtOffset.TabIndex = 1;
            this.txtOffset.Text = "0";
            this.txtOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtOffset.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtOffset_KeyPress);
            // 
            // cmbJackType
            // 
            this.cmbJackType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbJackType.FormattingEnabled = true;
            this.cmbJackType.Location = new System.Drawing.Point(85, 32);
            this.cmbJackType.Name = "cmbJackType";
            this.cmbJackType.Size = new System.Drawing.Size(210, 20);
            this.cmbJackType.TabIndex = 1;
            this.cmbJackType.SelectedIndexChanged += new System.EventHandler(this.cmbJackType_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtOffset);
            this.groupBox1.Location = new System.Drawing.Point(14, 80);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(281, 47);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(252, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "mm";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "離れ量";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(197, 156);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(95, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(96, 156);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(95, 23);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // chkUseJackCover
            // 
            this.chkUseJackCover.AutoSize = true;
            this.chkUseJackCover.Location = new System.Drawing.Point(14, 133);
            this.chkUseJackCover.Name = "chkUseJackCover";
            this.chkUseJackCover.Size = new System.Drawing.Size(140, 16);
            this.chkUseJackCover.TabIndex = 6;
            this.chkUseJackCover.Text = "ジャッキカバーを使用する";
            this.chkUseJackCover.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "店所";
            // 
            // cmbTensyo
            // 
            this.cmbTensyo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTensyo.FormattingEnabled = true;
            this.cmbTensyo.Location = new System.Drawing.Point(85, 6);
            this.cmbTensyo.Name = "cmbTensyo";
            this.cmbTensyo.Size = new System.Drawing.Size(210, 20);
            this.cmbTensyo.TabIndex = 3;
            // 
            // chkUseOffset
            // 
            this.chkUseOffset.AutoSize = true;
            this.chkUseOffset.Location = new System.Drawing.Point(14, 66);
            this.chkUseOffset.Name = "chkUseOffset";
            this.chkUseOffset.Size = new System.Drawing.Size(83, 16);
            this.chkUseOffset.TabIndex = 4;
            this.chkUseOffset.Text = "離れ量指定";
            this.chkUseOffset.UseVisualStyleBackColor = true;
            this.chkUseOffset.CheckedChanged += new System.EventHandler(this.chkUseOffset_CheckedChanged);
            // 
            // DlgCreateJack
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 191);
            this.ControlBox = false;
            this.Controls.Add(this.chkUseOffset);
            this.Controls.Add(this.cmbTensyo);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.chkUseJackCover);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cmbJackType);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgCreateJack";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ジャッキ・土圧計";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtOffset;
        private System.Windows.Forms.ComboBox cmbJackType;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox chkUseJackCover;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbTensyo;
        private System.Windows.Forms.CheckBox chkUseOffset;
    }
}