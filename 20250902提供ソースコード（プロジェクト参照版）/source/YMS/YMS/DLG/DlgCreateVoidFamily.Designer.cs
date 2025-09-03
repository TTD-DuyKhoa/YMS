
namespace YMS.DLG
{
    partial class DlgCreateVoidFamily
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
            this.btnEnd = new System.Windows.Forms.Button();
            this.btnKabeKussaku = new System.Windows.Forms.Button();
            this.txtKussakuFukasa1 = new System.Windows.Forms.TextBox();
            this.txtKussakuFukasa2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSoiruKussaku = new System.Windows.Forms.Button();
            this.btnKijunLineSoiru = new System.Windows.Forms.Button();
            this.btnKijunLineKabeKussaku = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(12, 214);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(108, 23);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // btnEnd
            // 
            this.btnEnd.Location = new System.Drawing.Point(138, 214);
            this.btnEnd.Name = "btnEnd";
            this.btnEnd.Size = new System.Drawing.Size(108, 23);
            this.btnEnd.TabIndex = 9;
            this.btnEnd.Text = "終了";
            this.btnEnd.UseVisualStyleBackColor = true;
            this.btnEnd.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // btnKabeKussaku
            // 
            this.btnKabeKussaku.Location = new System.Drawing.Point(12, 12);
            this.btnKabeKussaku.Name = "btnKabeKussaku";
            this.btnKabeKussaku.Size = new System.Drawing.Size(234, 23);
            this.btnKabeKussaku.TabIndex = 0;
            this.btnKabeKussaku.Text = "壁掘削用ボイドのファミリ配置";
            this.btnKabeKussaku.UseVisualStyleBackColor = true;
            this.btnKabeKussaku.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtKussakuFukasa1
            // 
            this.txtKussakuFukasa1.Location = new System.Drawing.Point(138, 136);
            this.txtKussakuFukasa1.Name = "txtKussakuFukasa1";
            this.txtKussakuFukasa1.Size = new System.Drawing.Size(75, 19);
            this.txtKussakuFukasa1.TabIndex = 5;
            this.txtKussakuFukasa1.Text = "1500";
            this.txtKussakuFukasa1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtKussakuFukasa2
            // 
            this.txtKussakuFukasa2.Location = new System.Drawing.Point(138, 172);
            this.txtKussakuFukasa2.Name = "txtKussakuFukasa2";
            this.txtKussakuFukasa2.Size = new System.Drawing.Size(75, 19);
            this.txtKussakuFukasa2.TabIndex = 7;
            this.txtKussakuFukasa2.Text = "1500";
            this.txtKussakuFukasa2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(42, 139);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "掘削深さ1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(42, 175);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "掘削深さ2";
            // 
            // btnSoiruKussaku
            // 
            this.btnSoiruKussaku.Location = new System.Drawing.Point(12, 41);
            this.btnSoiruKussaku.Name = "btnSoiruKussaku";
            this.btnSoiruKussaku.Size = new System.Drawing.Size(234, 23);
            this.btnSoiruKussaku.TabIndex = 1;
            this.btnSoiruKussaku.Text = "ソイル掘削用ボイドファミリの配置";
            this.btnSoiruKussaku.UseVisualStyleBackColor = true;
            this.btnSoiruKussaku.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnKijunLineSoiru
            // 
            this.btnKijunLineSoiru.Location = new System.Drawing.Point(12, 99);
            this.btnKijunLineSoiru.Name = "btnKijunLineSoiru";
            this.btnKijunLineSoiru.Size = new System.Drawing.Size(234, 23);
            this.btnKijunLineSoiru.TabIndex = 3;
            this.btnKijunLineSoiru.Text = "基準線選択ソイル掘削用ボイドファミリの配置";
            this.btnKijunLineSoiru.UseVisualStyleBackColor = true;
            this.btnKijunLineSoiru.Click += new System.EventHandler(this.button3_Click);
            // 
            // btnKijunLineKabeKussaku
            // 
            this.btnKijunLineKabeKussaku.Location = new System.Drawing.Point(12, 70);
            this.btnKijunLineKabeKussaku.Name = "btnKijunLineKabeKussaku";
            this.btnKijunLineKabeKussaku.Size = new System.Drawing.Size(234, 23);
            this.btnKijunLineKabeKussaku.TabIndex = 2;
            this.btnKijunLineKabeKussaku.Text = "基準線選択壁掘削用ボイドファミリの配置";
            this.btnKijunLineKabeKussaku.UseVisualStyleBackColor = true;
            this.btnKijunLineKabeKussaku.Click += new System.EventHandler(this.button4_Click);
            // 
            // DlgCreateVoidFamily
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(254, 258);
            this.Controls.Add(this.btnKijunLineKabeKussaku);
            this.Controls.Add(this.btnKijunLineSoiru);
            this.Controls.Add(this.btnSoiruKussaku);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtKussakuFukasa2);
            this.Controls.Add(this.txtKussakuFukasa1);
            this.Controls.Add(this.btnKabeKussaku);
            this.Controls.Add(this.btnEnd);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgCreateVoidFamily";
            this.Opacity = 0.75D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ModelessFormTest";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ModelessFormTest_FormClosed);
            this.Shown += new System.EventHandler(this.ModelessFormTest_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnEnd;
        private System.Windows.Forms.Button btnKabeKussaku;
        private System.Windows.Forms.TextBox txtKussakuFukasa1;
        private System.Windows.Forms.TextBox txtKussakuFukasa2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSoiruKussaku;
        private System.Windows.Forms.Button btnKijunLineSoiru;
        private System.Windows.Forms.Button btnKijunLineKabeKussaku;
    }
}