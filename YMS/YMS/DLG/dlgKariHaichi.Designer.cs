namespace YMS.DLG
{
    partial class dlgKariHaichi
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
            this.rdoHaraokoshi = new System.Windows.Forms.RadioButton();
            this.rdoHiuchi = new System.Windows.Forms.RadioButton();
            this.rdoKiribari = new System.Windows.Forms.RadioButton();
            this.rdoKabe = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdoHaraokoshi);
            this.groupBox1.Controls.Add(this.rdoHiuchi);
            this.groupBox1.Controls.Add(this.rdoKiribari);
            this.groupBox1.Controls.Add(this.rdoKabe);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(259, 117);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "配置部材";
            // 
            // rdoHaraokoshi
            // 
            this.rdoHaraokoshi.AutoSize = true;
            this.rdoHaraokoshi.Location = new System.Drawing.Point(7, 88);
            this.rdoHaraokoshi.Name = "rdoHaraokoshi";
            this.rdoHaraokoshi.Size = new System.Drawing.Size(47, 16);
            this.rdoHaraokoshi.TabIndex = 3;
            this.rdoHaraokoshi.TabStop = true;
            this.rdoHaraokoshi.Text = "腹起";
            this.rdoHaraokoshi.UseVisualStyleBackColor = true;
            // 
            // rdoHiuchi
            // 
            this.rdoHiuchi.AutoSize = true;
            this.rdoHiuchi.Enabled = false;
            this.rdoHiuchi.Location = new System.Drawing.Point(7, 65);
            this.rdoHiuchi.Name = "rdoHiuchi";
            this.rdoHiuchi.Size = new System.Drawing.Size(47, 16);
            this.rdoHiuchi.TabIndex = 2;
            this.rdoHiuchi.TabStop = true;
            this.rdoHiuchi.Text = "火打";
            this.rdoHiuchi.UseVisualStyleBackColor = true;
            // 
            // rdoKiribari
            // 
            this.rdoKiribari.AutoSize = true;
            this.rdoKiribari.Location = new System.Drawing.Point(7, 42);
            this.rdoKiribari.Name = "rdoKiribari";
            this.rdoKiribari.Size = new System.Drawing.Size(47, 16);
            this.rdoKiribari.TabIndex = 1;
            this.rdoKiribari.TabStop = true;
            this.rdoKiribari.Text = "切梁";
            this.rdoKiribari.UseVisualStyleBackColor = true;
            // 
            // rdoKabe
            // 
            this.rdoKabe.AutoSize = true;
            this.rdoKabe.Location = new System.Drawing.Point(7, 19);
            this.rdoKabe.Name = "rdoKabe";
            this.rdoKabe.Size = new System.Drawing.Size(35, 16);
            this.rdoKabe.TabIndex = 0;
            this.rdoKabe.TabStop = true;
            this.rdoKabe.Text = "壁";
            this.rdoKabe.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(107, 317);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(197, 317);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("MS UI Gothic", 19F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(-2, 213);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(285, 26);
            this.label1.TabIndex = 1;
            this.label1.Text = "ここに各設定項目を並べる";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("MS UI Gothic", 19F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(44, 252);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(138, 26);
            this.label2.TabIndex = 2;
            this.label2.Text = "例）サイズ等";
            // 
            // dlgKariHaichi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 352);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "dlgKariHaichi";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "仮配置";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdoHaraokoshi;
        private System.Windows.Forms.RadioButton rdoHiuchi;
        private System.Windows.Forms.RadioButton rdoKiribari;
        private System.Windows.Forms.RadioButton rdoKabe;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}