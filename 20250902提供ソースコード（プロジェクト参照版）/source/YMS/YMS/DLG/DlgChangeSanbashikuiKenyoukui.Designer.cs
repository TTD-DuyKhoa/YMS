namespace YMS.DLG
{
    partial class DlgChangeSanbashikuiKenyoukui
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
            this.grpPileType = new System.Windows.Forms.GroupBox();
            this.rbnPileTypeTC = new System.Windows.Forms.RadioButton();
            this.rbnPileTypeDanmenHenka = new System.Windows.Forms.RadioButton();
            this.rbnPileTypeKenyou = new System.Windows.Forms.RadioButton();
            this.rbnPileTypeSanbashi = new System.Windows.Forms.RadioButton();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.grpPileType.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpPileType
            // 
            this.grpPileType.Controls.Add(this.rbnPileTypeTC);
            this.grpPileType.Controls.Add(this.rbnPileTypeDanmenHenka);
            this.grpPileType.Controls.Add(this.rbnPileTypeKenyou);
            this.grpPileType.Controls.Add(this.rbnPileTypeSanbashi);
            this.grpPileType.Location = new System.Drawing.Point(12, 12);
            this.grpPileType.Name = "grpPileType";
            this.grpPileType.Size = new System.Drawing.Size(388, 48);
            this.grpPileType.TabIndex = 4;
            this.grpPileType.TabStop = false;
            this.grpPileType.Text = "杭タイプ";
            // 
            // rbnPileTypeTC
            // 
            this.rbnPileTypeTC.AutoSize = true;
            this.rbnPileTypeTC.Location = new System.Drawing.Point(329, 18);
            this.rbnPileTypeTC.Name = "rbnPileTypeTC";
            this.rbnPileTypeTC.Size = new System.Drawing.Size(50, 16);
            this.rbnPileTypeTC.TabIndex = 4;
            this.rbnPileTypeTC.Text = "TC杭";
            this.rbnPileTypeTC.UseVisualStyleBackColor = true;
            // 
            // rbnPileTypeDanmenHenka
            // 
            this.rbnPileTypeDanmenHenka.AutoSize = true;
            this.rbnPileTypeDanmenHenka.Location = new System.Drawing.Point(220, 18);
            this.rbnPileTypeDanmenHenka.Name = "rbnPileTypeDanmenHenka";
            this.rbnPileTypeDanmenHenka.Size = new System.Drawing.Size(83, 16);
            this.rbnPileTypeDanmenHenka.TabIndex = 3;
            this.rbnPileTypeDanmenHenka.Text = "断面変化杭";
            this.rbnPileTypeDanmenHenka.UseVisualStyleBackColor = true;
            // 
            // rbnPileTypeKenyou
            // 
            this.rbnPileTypeKenyou.AutoSize = true;
            this.rbnPileTypeKenyou.Location = new System.Drawing.Point(135, 18);
            this.rbnPileTypeKenyou.Name = "rbnPileTypeKenyou";
            this.rbnPileTypeKenyou.Size = new System.Drawing.Size(59, 16);
            this.rbnPileTypeKenyou.TabIndex = 1;
            this.rbnPileTypeKenyou.Text = "兼用杭";
            this.rbnPileTypeKenyou.UseVisualStyleBackColor = true;
            // 
            // rbnPileTypeSanbashi
            // 
            this.rbnPileTypeSanbashi.AutoSize = true;
            this.rbnPileTypeSanbashi.Checked = true;
            this.rbnPileTypeSanbashi.Location = new System.Drawing.Point(6, 18);
            this.rbnPileTypeSanbashi.Name = "rbnPileTypeSanbashi";
            this.rbnPileTypeSanbashi.Size = new System.Drawing.Size(103, 16);
            this.rbnPileTypeSanbashi.TabIndex = 0;
            this.rbnPileTypeSanbashi.TabStop = true;
            this.rbnPileTypeSanbashi.Text = "桟橋杭(支持杭)";
            this.rbnPileTypeSanbashi.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(307, 66);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(95, 23);
            this.btnCancel.TabIndex = 32;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(206, 66);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(95, 23);
            this.btnOK.TabIndex = 31;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // DlgChangeSanbashikuiKenyoukui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 101);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.grpPileType);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgChangeSanbashikuiKenyoukui";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "桟橋杭・兼用杭 変更";
            this.grpPileType.ResumeLayout(false);
            this.grpPileType.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpPileType;
        private System.Windows.Forms.RadioButton rbnPileTypeTC;
        private System.Windows.Forms.RadioButton rbnPileTypeDanmenHenka;
        private System.Windows.Forms.RadioButton rbnPileTypeKenyou;
        private System.Windows.Forms.RadioButton rbnPileTypeSanbashi;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
    }
}