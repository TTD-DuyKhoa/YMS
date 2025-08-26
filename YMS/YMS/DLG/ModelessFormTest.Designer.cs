
namespace YMS.DLG
{
    partial class ModelessFormTest
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
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonWarituke = new System.Windows.Forms.Button();
            this.buttonTyouseizaiPut = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(53, 256);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(195, 256);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "終了";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonWarituke
            // 
            this.buttonWarituke.Location = new System.Drawing.Point(53, 174);
            this.buttonWarituke.Name = "buttonWarituke";
            this.buttonWarituke.Size = new System.Drawing.Size(75, 23);
            this.buttonWarituke.TabIndex = 0;
            this.buttonWarituke.Text = "割付";
            this.buttonWarituke.UseVisualStyleBackColor = true;
            this.buttonWarituke.Click += new System.EventHandler(this.buttonWarituke_Click);
            // 
            // buttonTyouseizaiPut
            // 
            this.buttonTyouseizaiPut.Location = new System.Drawing.Point(195, 174);
            this.buttonTyouseizaiPut.Name = "buttonTyouseizaiPut";
            this.buttonTyouseizaiPut.Size = new System.Drawing.Size(75, 23);
            this.buttonTyouseizaiPut.TabIndex = 1;
            this.buttonTyouseizaiPut.Text = "調整材配置";
            this.buttonTyouseizaiPut.UseVisualStyleBackColor = true;
            this.buttonTyouseizaiPut.Click += new System.EventHandler(this.buttonTyouseizaiPut_Click);
            // 
            // ModelessFormTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(328, 316);
            this.Controls.Add(this.buttonTyouseizaiPut);
            this.Controls.Add(this.buttonWarituke);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ModelessFormTest";
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

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonWarituke;
        private System.Windows.Forms.Button buttonTyouseizaiPut;
    }
}