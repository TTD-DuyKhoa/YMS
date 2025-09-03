namespace YMS.DLG
{
    partial class DlgZumenInfo
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
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdoDoboku = new System.Windows.Forms.RadioButton();
            this.rdoKenchiku = new System.Windows.Forms.RadioButton();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtTokuisaki = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtGenba = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.txtSekkeiNum = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdoDoboku);
            this.groupBox1.Controls.Add(this.rdoKenchiku);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(136, 42);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "設計指針";
            // 
            // rdoDoboku
            // 
            this.rdoDoboku.AutoSize = true;
            this.rdoDoboku.Location = new System.Drawing.Point(73, 18);
            this.rdoDoboku.Name = "rdoDoboku";
            this.rdoDoboku.Size = new System.Drawing.Size(47, 16);
            this.rdoDoboku.TabIndex = 1;
            this.rdoDoboku.Text = "土木";
            this.rdoDoboku.UseVisualStyleBackColor = true;
            // 
            // rdoKenchiku
            // 
            this.rdoKenchiku.AutoSize = true;
            this.rdoKenchiku.Checked = true;
            this.rdoKenchiku.Location = new System.Drawing.Point(7, 18);
            this.rdoKenchiku.Name = "rdoKenchiku";
            this.rdoKenchiku.Size = new System.Drawing.Size(47, 16);
            this.rdoKenchiku.TabIndex = 0;
            this.rdoKenchiku.TabStop = true;
            this.rdoKenchiku.Text = "建築";
            this.rdoKenchiku.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtTokuisaki);
            this.groupBox2.Location = new System.Drawing.Point(13, 61);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(227, 50);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "得意先名";
            // 
            // txtTokuisaki
            // 
            this.txtTokuisaki.Location = new System.Drawing.Point(6, 18);
            this.txtTokuisaki.Name = "txtTokuisaki";
            this.txtTokuisaki.Size = new System.Drawing.Size(214, 19);
            this.txtTokuisaki.TabIndex = 0;
            this.txtTokuisaki.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtGenba);
            this.groupBox3.Location = new System.Drawing.Point(12, 117);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(227, 50);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "現場名";
            // 
            // txtGenba
            // 
            this.txtGenba.Location = new System.Drawing.Point(7, 18);
            this.txtGenba.Name = "txtGenba";
            this.txtGenba.Size = new System.Drawing.Size(214, 19);
            this.txtGenba.TabIndex = 0;
            this.txtGenba.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.txtSekkeiNum);
            this.groupBox4.Location = new System.Drawing.Point(12, 173);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(227, 50);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "設計番号";
            // 
            // txtSekkeiNum
            // 
            this.txtSekkeiNum.Location = new System.Drawing.Point(7, 18);
            this.txtSekkeiNum.Name = "txtSekkeiNum";
            this.txtSekkeiNum.Size = new System.Drawing.Size(214, 19);
            this.txtSekkeiNum.TabIndex = 0;
            this.txtSekkeiNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(165, 229);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(74, 229);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // DlgZumenInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(255, 257);
            this.ControlBox = false;
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgZumenInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "図面情報設定";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DlgZumenInfo_FormClosed);
            this.Load += new System.EventHandler(this.DlgZumenInfo_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdoDoboku;
        private System.Windows.Forms.RadioButton rdoKenchiku;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtTokuisaki;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtGenba;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox txtSekkeiNum;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
    }
}