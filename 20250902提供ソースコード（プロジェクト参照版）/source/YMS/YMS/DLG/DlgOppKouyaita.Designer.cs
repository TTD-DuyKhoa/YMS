
namespace YMS.DLG
{
    partial class DlgOppKouyaita
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
            this.btnOpp = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(12, 14);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "確定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnOpp
            // 
            this.btnOpp.Location = new System.Drawing.Point(126, 14);
            this.btnOpp.Name = "btnOpp";
            this.btnOpp.Size = new System.Drawing.Size(75, 23);
            this.btnOpp.TabIndex = 1;
            this.btnOpp.Text = "反転";
            this.btnOpp.UseVisualStyleBackColor = true;
            this.btnOpp.Click += new System.EventHandler(this.btnOpp_Click);
            // 
            // DlgOppKouyaita
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(213, 49);
            this.ControlBox = false;
            this.Controls.Add(this.btnOpp);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgOppKouyaita";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "鋼矢板配置向き";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnOpp;
    }
}