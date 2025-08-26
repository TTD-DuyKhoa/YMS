
namespace YMS.DLG
{
    partial class DlgCreateKobetsuHaichi
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
            this.comboBoxFolder1 = new System.Windows.Forms.ComboBox();
            this.comboBoxFolder2 = new System.Windows.Forms.ComboBox();
            this.comboBoxFamily = new System.Windows.Forms.ComboBox();
            this.comboBoxType = new System.Windows.Forms.ComboBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.labelFolder = new System.Windows.Forms.Label();
            this.labelSubFolder = new System.Windows.Forms.Label();
            this.labelFamily = new System.Windows.Forms.Label();
            this.labelType = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxBunrui = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // comboBoxFolder1
            // 
            this.comboBoxFolder1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFolder1.FormattingEnabled = true;
            this.comboBoxFolder1.Location = new System.Drawing.Point(82, 49);
            this.comboBoxFolder1.Name = "comboBoxFolder1";
            this.comboBoxFolder1.Size = new System.Drawing.Size(229, 20);
            this.comboBoxFolder1.TabIndex = 3;
            this.comboBoxFolder1.SelectedIndexChanged += new System.EventHandler(this.comboBoxFolder1_SelectedIndexChanged);
            // 
            // comboBoxFolder2
            // 
            this.comboBoxFolder2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFolder2.FormattingEnabled = true;
            this.comboBoxFolder2.Location = new System.Drawing.Point(82, 92);
            this.comboBoxFolder2.Name = "comboBoxFolder2";
            this.comboBoxFolder2.Size = new System.Drawing.Size(229, 20);
            this.comboBoxFolder2.TabIndex = 5;
            this.comboBoxFolder2.SelectedIndexChanged += new System.EventHandler(this.comboBoxFolder2_SelectedIndexChanged);
            // 
            // comboBoxFamily
            // 
            this.comboBoxFamily.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFamily.FormattingEnabled = true;
            this.comboBoxFamily.Location = new System.Drawing.Point(82, 133);
            this.comboBoxFamily.Name = "comboBoxFamily";
            this.comboBoxFamily.Size = new System.Drawing.Size(229, 20);
            this.comboBoxFamily.TabIndex = 7;
            this.comboBoxFamily.SelectedIndexChanged += new System.EventHandler(this.comboBoxFamily_SelectedIndexChanged);
            // 
            // comboBoxType
            // 
            this.comboBoxType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxType.FormattingEnabled = true;
            this.comboBoxType.Location = new System.Drawing.Point(82, 175);
            this.comboBoxType.Name = "comboBoxType";
            this.comboBoxType.Size = new System.Drawing.Size(229, 20);
            this.comboBoxType.TabIndex = 9;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(140, 223);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 10;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(236, 223);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // labelFolder
            // 
            this.labelFolder.AutoSize = true;
            this.labelFolder.Location = new System.Drawing.Point(12, 52);
            this.labelFolder.Name = "labelFolder";
            this.labelFolder.Size = new System.Drawing.Size(40, 12);
            this.labelFolder.TabIndex = 2;
            this.labelFolder.Text = "フォルダ";
            // 
            // labelSubFolder
            // 
            this.labelSubFolder.AutoSize = true;
            this.labelSubFolder.Location = new System.Drawing.Point(12, 95);
            this.labelSubFolder.Name = "labelSubFolder";
            this.labelSubFolder.Size = new System.Drawing.Size(59, 12);
            this.labelSubFolder.TabIndex = 4;
            this.labelSubFolder.Text = "サブフォルダ";
            // 
            // labelFamily
            // 
            this.labelFamily.AutoSize = true;
            this.labelFamily.Location = new System.Drawing.Point(12, 136);
            this.labelFamily.Name = "labelFamily";
            this.labelFamily.Size = new System.Drawing.Size(34, 12);
            this.labelFamily.TabIndex = 6;
            this.labelFamily.Text = "ファミリ";
            // 
            // labelType
            // 
            this.labelType.AutoSize = true;
            this.labelType.Location = new System.Drawing.Point(12, 178);
            this.labelType.Name = "labelType";
            this.labelType.Size = new System.Drawing.Size(31, 12);
            this.labelType.TabIndex = 8;
            this.labelType.Text = "タイプ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "分類";
            // 
            // comboBoxBunrui
            // 
            this.comboBoxBunrui.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBunrui.FormattingEnabled = true;
            this.comboBoxBunrui.Location = new System.Drawing.Point(82, 6);
            this.comboBoxBunrui.Name = "comboBoxBunrui";
            this.comboBoxBunrui.Size = new System.Drawing.Size(229, 20);
            this.comboBoxBunrui.TabIndex = 1;
            this.comboBoxBunrui.SelectedIndexChanged += new System.EventHandler(this.comboBoxBunrui_SelectedIndexChanged);
            // 
            // DlgCreateKobetsuHaichi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(331, 262);
            this.ControlBox = false;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxBunrui);
            this.Controls.Add(this.labelType);
            this.Controls.Add(this.labelFamily);
            this.Controls.Add(this.labelSubFolder);
            this.Controls.Add(this.labelFolder);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.comboBoxType);
            this.Controls.Add(this.comboBoxFamily);
            this.Controls.Add(this.comboBoxFolder2);
            this.Controls.Add(this.comboBoxFolder1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgCreateKobetsuHaichi";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "個別配置";
            this.Load += new System.EventHandler(this.DlgCreateKobetsuHaichi_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxFolder1;
        private System.Windows.Forms.ComboBox comboBoxFolder2;
        private System.Windows.Forms.ComboBox comboBoxFamily;
        private System.Windows.Forms.ComboBox comboBoxType;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label labelFolder;
        private System.Windows.Forms.Label labelSubFolder;
        private System.Windows.Forms.Label labelFamily;
        private System.Windows.Forms.Label labelType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxBunrui;
    }
}