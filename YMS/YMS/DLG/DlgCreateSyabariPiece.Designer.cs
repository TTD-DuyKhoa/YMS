
namespace YMS.DLG
{
    partial class DlgCreateSyabariPiece
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
            this.cmbBuhinSize1 = new System.Windows.Forms.ComboBox();
            this.lblBuhinSize1 = new System.Windows.Forms.Label();
            this.cmbBuhinType1 = new System.Windows.Forms.ComboBox();
            this.lblBuhinType1 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cmbBuhinSize2 = new System.Windows.Forms.ComboBox();
            this.lblBuhinSize2 = new System.Windows.Forms.Label();
            this.cmbBuhinType2 = new System.Windows.Forms.ComboBox();
            this.lblBuhinType2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cmbBuhinSize1
            // 
            this.cmbBuhinSize1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBuhinSize1.FormattingEnabled = true;
            this.cmbBuhinSize1.Items.AddRange(new object[] {
            "35VP ヒウチウケ"});
            this.cmbBuhinSize1.Location = new System.Drawing.Point(109, 38);
            this.cmbBuhinSize1.Name = "cmbBuhinSize1";
            this.cmbBuhinSize1.Size = new System.Drawing.Size(163, 20);
            this.cmbBuhinSize1.TabIndex = 7;
            // 
            // lblBuhinSize1
            // 
            this.lblBuhinSize1.AutoSize = true;
            this.lblBuhinSize1.Location = new System.Drawing.Point(4, 41);
            this.lblBuhinSize1.Name = "lblBuhinSize1";
            this.lblBuhinSize1.Size = new System.Drawing.Size(58, 12);
            this.lblBuhinSize1.TabIndex = 6;
            this.lblBuhinSize1.Text = "部品サイズ";
            // 
            // cmbBuhinType1
            // 
            this.cmbBuhinType1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBuhinType1.FormattingEnabled = true;
            this.cmbBuhinType1.Items.AddRange(new object[] {
            "30度火打受ピース",
            "火打受ピース"});
            this.cmbBuhinType1.Location = new System.Drawing.Point(109, 12);
            this.cmbBuhinType1.Name = "cmbBuhinType1";
            this.cmbBuhinType1.Size = new System.Drawing.Size(163, 20);
            this.cmbBuhinType1.TabIndex = 5;
            this.cmbBuhinType1.SelectedIndexChanged += new System.EventHandler(this.cmbBuhinType1_SelectedIndexChanged);
            // 
            // lblBuhinType1
            // 
            this.lblBuhinType1.AutoSize = true;
            this.lblBuhinType1.Location = new System.Drawing.Point(4, 15);
            this.lblBuhinType1.Name = "lblBuhinType1";
            this.lblBuhinType1.Size = new System.Drawing.Size(55, 12);
            this.lblBuhinType1.TabIndex = 4;
            this.lblBuhinType1.Text = "部品タイプ";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(28, 120);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(90, 23);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK(O)";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(144, 120);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "キャンセル(C)";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cmbBuhinSize2
            // 
            this.cmbBuhinSize2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBuhinSize2.FormattingEnabled = true;
            this.cmbBuhinSize2.Items.AddRange(new object[] {
            "35VP ヒウチウケ"});
            this.cmbBuhinSize2.Location = new System.Drawing.Point(109, 93);
            this.cmbBuhinSize2.Name = "cmbBuhinSize2";
            this.cmbBuhinSize2.Size = new System.Drawing.Size(163, 20);
            this.cmbBuhinSize2.TabIndex = 13;
            // 
            // lblBuhinSize2
            // 
            this.lblBuhinSize2.AutoSize = true;
            this.lblBuhinSize2.Location = new System.Drawing.Point(4, 93);
            this.lblBuhinSize2.Name = "lblBuhinSize2";
            this.lblBuhinSize2.Size = new System.Drawing.Size(102, 12);
            this.lblBuhinSize2.TabIndex = 12;
            this.lblBuhinSize2.Text = "部品サイズ(終点側)";
            // 
            // cmbBuhinType2
            // 
            this.cmbBuhinType2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBuhinType2.FormattingEnabled = true;
            this.cmbBuhinType2.Items.AddRange(new object[] {
            "30度火打受ピース",
            "火打受ピース"});
            this.cmbBuhinType2.Location = new System.Drawing.Point(109, 64);
            this.cmbBuhinType2.Name = "cmbBuhinType2";
            this.cmbBuhinType2.Size = new System.Drawing.Size(163, 20);
            this.cmbBuhinType2.TabIndex = 11;
            this.cmbBuhinType2.SelectedIndexChanged += new System.EventHandler(this.cmbBuhinType2_SelectedIndexChanged);
            // 
            // lblBuhinType2
            // 
            this.lblBuhinType2.AutoSize = true;
            this.lblBuhinType2.Location = new System.Drawing.Point(4, 67);
            this.lblBuhinType2.Name = "lblBuhinType2";
            this.lblBuhinType2.Size = new System.Drawing.Size(99, 12);
            this.lblBuhinType2.TabIndex = 10;
            this.lblBuhinType2.Text = "部品タイプ(終点側)";
            // 
            // DlgCreateSyabariPiece
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 152);
            this.ControlBox = false;
            this.Controls.Add(this.cmbBuhinSize2);
            this.Controls.Add(this.lblBuhinSize2);
            this.Controls.Add(this.cmbBuhinType2);
            this.Controls.Add(this.lblBuhinType2);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.cmbBuhinSize1);
            this.Controls.Add(this.lblBuhinSize1);
            this.Controls.Add(this.cmbBuhinType1);
            this.Controls.Add(this.lblBuhinType1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgCreateSyabariPiece";
            this.Text = "斜張用端部部品作成";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbBuhinSize1;
        private System.Windows.Forms.Label lblBuhinSize1;
        private System.Windows.Forms.ComboBox cmbBuhinType1;
        private System.Windows.Forms.Label lblBuhinType1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cmbBuhinSize2;
        private System.Windows.Forms.Label lblBuhinSize2;
        private System.Windows.Forms.ComboBox cmbBuhinType2;
        private System.Windows.Forms.Label lblBuhinType2;
    }
}