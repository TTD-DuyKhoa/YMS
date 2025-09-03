
namespace YMS.DLG
{
    partial class DlgChangeLength
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
            this.label2 = new System.Windows.Forms.Label();
            this.txtLength = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.rdoEdit = new System.Windows.Forms.RadioButton();
            this.rdoPartition = new System.Windows.Forms.RadioButton();
            this.rdoCombine = new System.Windows.Forms.RadioButton();
            this.numNewLength = new System.Windows.Forms.NumericUpDown();
            this.numDiv1NewLength = new System.Windows.Forms.NumericUpDown();
            this.numDiv2NewLength = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numNewLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDiv1NewLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDiv2NewLength)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 9);
            this.label1.Name = "label1";
            this.label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "現在の長さ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(197, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "m";
            // 
            // txtLength
            // 
            this.txtLength.Location = new System.Drawing.Point(91, 6);
            this.txtLength.Name = "txtLength";
            this.txtLength.ReadOnly = true;
            this.txtLength.Size = new System.Drawing.Size(100, 19);
            this.txtLength.TabIndex = 1;
            this.txtLength.Text = "5.0";
            this.txtLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(197, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "m";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(2, 59);
            this.label4.Name = "label4";
            this.label4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label4.Size = new System.Drawing.Size(71, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "変更後の長さ";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(68, 133);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 15;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(149, 133);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 16;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(197, 87);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(14, 12);
            this.label7.TabIndex = 11;
            this.label7.Text = "m";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(2, 85);
            this.label8.Name = "label8";
            this.label8.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label8.Size = new System.Drawing.Size(77, 12);
            this.label8.TabIndex = 9;
            this.label8.Text = "分割後の長さ1";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(197, 112);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(14, 12);
            this.label9.TabIndex = 14;
            this.label9.Text = "m";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(2, 111);
            this.label10.Name = "label10";
            this.label10.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label10.Size = new System.Drawing.Size(77, 12);
            this.label10.TabIndex = 12;
            this.label10.Text = "分割後の長さ2";
            // 
            // rdoEdit
            // 
            this.rdoEdit.AutoSize = true;
            this.rdoEdit.Checked = true;
            this.rdoEdit.Location = new System.Drawing.Point(4, 31);
            this.rdoEdit.Name = "rdoEdit";
            this.rdoEdit.Size = new System.Drawing.Size(47, 16);
            this.rdoEdit.TabIndex = 3;
            this.rdoEdit.TabStop = true;
            this.rdoEdit.Text = "変更";
            this.rdoEdit.UseVisualStyleBackColor = true;
            this.rdoEdit.CheckedChanged += new System.EventHandler(this.rdoEdit_CheckedChanged);
            // 
            // rdoPartition
            // 
            this.rdoPartition.AutoSize = true;
            this.rdoPartition.Location = new System.Drawing.Point(91, 31);
            this.rdoPartition.Name = "rdoPartition";
            this.rdoPartition.Size = new System.Drawing.Size(47, 16);
            this.rdoPartition.TabIndex = 4;
            this.rdoPartition.TabStop = true;
            this.rdoPartition.Text = "分割";
            this.rdoPartition.UseVisualStyleBackColor = true;
            // 
            // rdoCombine
            // 
            this.rdoCombine.AutoSize = true;
            this.rdoCombine.Location = new System.Drawing.Point(173, 31);
            this.rdoCombine.Name = "rdoCombine";
            this.rdoCombine.Size = new System.Drawing.Size(47, 16);
            this.rdoCombine.TabIndex = 5;
            this.rdoCombine.TabStop = true;
            this.rdoCombine.Text = "結合";
            this.rdoCombine.UseVisualStyleBackColor = true;
            this.rdoCombine.CheckedChanged += new System.EventHandler(this.rdoCombine_CheckedChanged);
            // 
            // numNewLength
            // 
            this.numNewLength.DecimalPlaces = 1;
            this.numNewLength.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.numNewLength.Location = new System.Drawing.Point(91, 57);
            this.numNewLength.Name = "numNewLength";
            this.numNewLength.Size = new System.Drawing.Size(100, 19);
            this.numNewLength.TabIndex = 7;
            // 
            // numDiv1NewLength
            // 
            this.numDiv1NewLength.BackColor = System.Drawing.SystemColors.Window;
            this.numDiv1NewLength.DecimalPlaces = 1;
            this.numDiv1NewLength.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.numDiv1NewLength.Location = new System.Drawing.Point(91, 82);
            this.numDiv1NewLength.Name = "numDiv1NewLength";
            this.numDiv1NewLength.ReadOnly = true;
            this.numDiv1NewLength.Size = new System.Drawing.Size(100, 19);
            this.numDiv1NewLength.TabIndex = 10;
            this.numDiv1NewLength.ValueChanged += new System.EventHandler(this.numDiv1NewLength_ValueChanged);
            // 
            // numDiv2NewLength
            // 
            this.numDiv2NewLength.BackColor = System.Drawing.SystemColors.Window;
            this.numDiv2NewLength.DecimalPlaces = 1;
            this.numDiv2NewLength.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.numDiv2NewLength.Location = new System.Drawing.Point(91, 107);
            this.numDiv2NewLength.Name = "numDiv2NewLength";
            this.numDiv2NewLength.ReadOnly = true;
            this.numDiv2NewLength.Size = new System.Drawing.Size(100, 19);
            this.numDiv2NewLength.TabIndex = 13;
            this.numDiv2NewLength.ValueChanged += new System.EventHandler(this.numDiv2NewLength_ValueChanged);
            // 
            // DlgChangeLength
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(237, 161);
            this.ControlBox = false;
            this.Controls.Add(this.numDiv2NewLength);
            this.Controls.Add(this.numDiv1NewLength);
            this.Controls.Add(this.numNewLength);
            this.Controls.Add(this.rdoCombine);
            this.Controls.Add(this.rdoPartition);
            this.Controls.Add(this.rdoEdit);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtLength);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgChangeLength";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "長さ変更";
            this.Load += new System.EventHandler(this.DlgChangeLength_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numNewLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDiv1NewLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDiv2NewLength)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLength;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.RadioButton rdoEdit;
        private System.Windows.Forms.RadioButton rdoPartition;
        private System.Windows.Forms.RadioButton rdoCombine;
        private System.Windows.Forms.NumericUpDown numNewLength;
        private System.Windows.Forms.NumericUpDown numDiv1NewLength;
        private System.Windows.Forms.NumericUpDown numDiv2NewLength;
    }
}