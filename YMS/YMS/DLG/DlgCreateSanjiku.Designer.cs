namespace YMS.DLG
{
    partial class DlgCreateSanjiku
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
            this.txtHaraokoshiHanareRyo = new System.Windows.Forms.TextBox();
            this.btnMove = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnMobePlus = new System.Windows.Forms.Button();
            this.btnMoveMinus = new System.Windows.Forms.Button();
            this.txtShitenkanKyoriRight = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtShitenkanKyoriLeft = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "腹起からの離れ量指定";
            // 
            // txtHaraokoshiHanareRyo
            // 
            this.txtHaraokoshiHanareRyo.Location = new System.Drawing.Point(14, 25);
            this.txtHaraokoshiHanareRyo.Name = "txtHaraokoshiHanareRyo";
            this.txtHaraokoshiHanareRyo.Size = new System.Drawing.Size(114, 19);
            this.txtHaraokoshiHanareRyo.TabIndex = 1;
            this.txtHaraokoshiHanareRyo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnMove
            // 
            this.btnMove.Location = new System.Drawing.Point(12, 47);
            this.btnMove.Name = "btnMove";
            this.btnMove.Size = new System.Drawing.Size(59, 23);
            this.btnMove.TabIndex = 2;
            this.btnMove.Text = "移動";
            this.btnMove.UseVisualStyleBackColor = true;
            this.btnMove.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 82);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "移動　＠100";
            // 
            // btnMobePlus
            // 
            this.btnMobePlus.Location = new System.Drawing.Point(16, 97);
            this.btnMobePlus.Name = "btnMobePlus";
            this.btnMobePlus.Size = new System.Drawing.Size(30, 23);
            this.btnMobePlus.TabIndex = 4;
            this.btnMobePlus.Text = "+";
            this.btnMobePlus.UseVisualStyleBackColor = true;
            this.btnMobePlus.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnMoveMinus
            // 
            this.btnMoveMinus.Location = new System.Drawing.Point(52, 97);
            this.btnMoveMinus.Name = "btnMoveMinus";
            this.btnMoveMinus.Size = new System.Drawing.Size(30, 23);
            this.btnMoveMinus.TabIndex = 5;
            this.btnMoveMinus.Text = "-";
            this.btnMoveMinus.UseVisualStyleBackColor = true;
            this.btnMoveMinus.Click += new System.EventHandler(this.button3_Click);
            // 
            // txtShitenkanKyoriRight
            // 
            this.txtShitenkanKyoriRight.Location = new System.Drawing.Point(12, 145);
            this.txtShitenkanKyoriRight.Name = "txtShitenkanKyoriRight";
            this.txtShitenkanKyoriRight.ReadOnly = true;
            this.txtShitenkanKyoriRight.Size = new System.Drawing.Size(114, 19);
            this.txtShitenkanKyoriRight.TabIndex = 7;
            this.txtShitenkanKyoriRight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 130);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "支点間距離右";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 167);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "支点間距離左";
            // 
            // txtShitenkanKyoriLeft
            // 
            this.txtShitenkanKyoriLeft.Location = new System.Drawing.Point(10, 182);
            this.txtShitenkanKyoriLeft.Name = "txtShitenkanKyoriLeft";
            this.txtShitenkanKyoriLeft.ReadOnly = true;
            this.txtShitenkanKyoriLeft.Size = new System.Drawing.Size(114, 19);
            this.txtShitenkanKyoriLeft.TabIndex = 9;
            this.txtShitenkanKyoriLeft.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // DlgCreateSanjiku
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(132, 208);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtShitenkanKyoriLeft);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtShitenkanKyoriRight);
            this.Controls.Add(this.btnMoveMinus);
            this.Controls.Add(this.btnMobePlus);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnMove);
            this.Controls.Add(this.txtHaraokoshiHanareRyo);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgCreateSanjiku";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DlgCreateSanjiku_FormClosed);
            this.Load += new System.EventHandler(this.DlgCreateSanjiku_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtHaraokoshiHanareRyo;
        private System.Windows.Forms.Button btnMove;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnMobePlus;
        private System.Windows.Forms.Button btnMoveMinus;
        private System.Windows.Forms.TextBox txtShitenkanKyoriRight;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtShitenkanKyoriLeft;
    }
}