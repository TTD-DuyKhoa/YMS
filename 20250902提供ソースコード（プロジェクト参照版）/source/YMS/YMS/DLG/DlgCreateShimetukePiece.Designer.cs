namespace YMS.DLG
{
    partial class DlgCreateShimetukePiece
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cmbBuzaiSize = new System.Windows.Forms.ComboBox();
            this.grpBuzaiSize = new System.Windows.Forms.GroupBox();
            this.grpBuzaiSize.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(54, 66);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(100, 23);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(160, 66);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(92, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cmbBuzaiSize
            // 
            this.cmbBuzaiSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBuzaiSize.FormattingEnabled = true;
            this.cmbBuzaiSize.Location = new System.Drawing.Point(6, 18);
            this.cmbBuzaiSize.Name = "cmbBuzaiSize";
            this.cmbBuzaiSize.Size = new System.Drawing.Size(228, 20);
            this.cmbBuzaiSize.TabIndex = 0;
            // 
            // grpBuzaiSize
            // 
            this.grpBuzaiSize.Controls.Add(this.cmbBuzaiSize);
            this.grpBuzaiSize.Location = new System.Drawing.Point(12, 12);
            this.grpBuzaiSize.Name = "grpBuzaiSize";
            this.grpBuzaiSize.Size = new System.Drawing.Size(240, 50);
            this.grpBuzaiSize.TabIndex = 10;
            this.grpBuzaiSize.TabStop = false;
            this.grpBuzaiSize.Text = "部材サイズ";
            // 
            // DlgCreateShimetukePiece
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(264, 101);
            this.Controls.Add(this.grpBuzaiSize);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgCreateShimetukePiece";
            this.Text = "締付用ピース";
            this.grpBuzaiSize.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cmbBuzaiSize;
        private System.Windows.Forms.GroupBox grpBuzaiSize;
    }
}