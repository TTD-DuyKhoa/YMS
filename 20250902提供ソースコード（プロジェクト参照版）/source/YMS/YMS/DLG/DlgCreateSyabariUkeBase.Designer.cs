
namespace YMS.DLG
{
    partial class DlgCreateSyabariUkeBase
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
            this.cmbSteelType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbSteelSize = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtTsukidashiRyoS = new System.Windows.Forms.TextBox();
            this.txtTsukidashiRyoE = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.OK = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.rbnShoriTypePileSelect = new System.Windows.Forms.RadioButton();
            this.rbnShoriTypePtoP = new System.Windows.Forms.RadioButton();
            this.rbnShoriTypeReplace = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "鋼材タイプ";
            // 
            // cmbSteelType
            // 
            this.cmbSteelType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSteelType.FormattingEnabled = true;
            this.cmbSteelType.Items.AddRange(new object[] {
            "チャンネル"});
            this.cmbSteelType.Location = new System.Drawing.Point(138, 49);
            this.cmbSteelType.Name = "cmbSteelType";
            this.cmbSteelType.Size = new System.Drawing.Size(121, 20);
            this.cmbSteelType.TabIndex = 5;
            this.cmbSteelType.SelectedIndexChanged += new System.EventHandler(this.cmbSteelType_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "鋼材サイズ";
            // 
            // cmbSteelSize
            // 
            this.cmbSteelSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSteelSize.FormattingEnabled = true;
            this.cmbSteelSize.Items.AddRange(new object[] {
            "C-100×50×5"});
            this.cmbSteelSize.Location = new System.Drawing.Point(138, 84);
            this.cmbSteelSize.Name = "cmbSteelSize";
            this.cmbSteelSize.Size = new System.Drawing.Size(121, 20);
            this.cmbSteelSize.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 123);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "突き出し量-始点";
            // 
            // txtTsukidashiRyoS
            // 
            this.txtTsukidashiRyoS.Location = new System.Drawing.Point(138, 120);
            this.txtTsukidashiRyoS.Name = "txtTsukidashiRyoS";
            this.txtTsukidashiRyoS.Size = new System.Drawing.Size(121, 19);
            this.txtTsukidashiRyoS.TabIndex = 9;
            this.txtTsukidashiRyoS.Text = "500";
            this.txtTsukidashiRyoS.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtTsukidashiRyoE
            // 
            this.txtTsukidashiRyoE.Location = new System.Drawing.Point(138, 155);
            this.txtTsukidashiRyoE.Name = "txtTsukidashiRyoE";
            this.txtTsukidashiRyoE.Size = new System.Drawing.Size(121, 19);
            this.txtTsukidashiRyoE.TabIndex = 11;
            this.txtTsukidashiRyoE.Text = "500";
            this.txtTsukidashiRyoE.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 158);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 12);
            this.label4.TabIndex = 10;
            this.label4.Text = "突き出し量-終点";
            // 
            // OK
            // 
            this.OK.Location = new System.Drawing.Point(59, 191);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(98, 23);
            this.OK.TabIndex = 12;
            this.OK.Text = "OK(O)";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(163, 191);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(98, 23);
            this.Cancel.TabIndex = 13;
            this.Cancel.Text = "キャンセル(C)";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 18);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "処理方法";
            // 
            // rbnShoriTypePileSelect
            // 
            this.rbnShoriTypePileSelect.AutoSize = true;
            this.rbnShoriTypePileSelect.Location = new System.Drawing.Point(141, 16);
            this.rbnShoriTypePileSelect.Name = "rbnShoriTypePileSelect";
            this.rbnShoriTypePileSelect.Size = new System.Drawing.Size(59, 16);
            this.rbnShoriTypePileSelect.TabIndex = 2;
            this.rbnShoriTypePileSelect.Text = "杭選択";
            this.rbnShoriTypePileSelect.UseVisualStyleBackColor = true;
            // 
            // rbnShoriTypePtoP
            // 
            this.rbnShoriTypePtoP.AutoSize = true;
            this.rbnShoriTypePtoP.Location = new System.Drawing.Point(206, 16);
            this.rbnShoriTypePtoP.Name = "rbnShoriTypePtoP";
            this.rbnShoriTypePtoP.Size = new System.Drawing.Size(53, 16);
            this.rbnShoriTypePtoP.TabIndex = 3;
            this.rbnShoriTypePtoP.Text = "2点間";
            this.rbnShoriTypePtoP.UseVisualStyleBackColor = true;
            // 
            // rbnShoriTypeReplace
            // 
            this.rbnShoriTypeReplace.AutoSize = true;
            this.rbnShoriTypeReplace.Checked = true;
            this.rbnShoriTypeReplace.Location = new System.Drawing.Point(88, 16);
            this.rbnShoriTypeReplace.Name = "rbnShoriTypeReplace";
            this.rbnShoriTypeReplace.Size = new System.Drawing.Size(47, 16);
            this.rbnShoriTypeReplace.TabIndex = 1;
            this.rbnShoriTypeReplace.TabStop = true;
            this.rbnShoriTypeReplace.Text = "置換";
            this.rbnShoriTypeReplace.UseVisualStyleBackColor = true;
            // 
            // DlgCreateShabariUkeBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(273, 226);
            this.ControlBox = false;
            this.Controls.Add(this.rbnShoriTypePileSelect);
            this.Controls.Add(this.rbnShoriTypePtoP);
            this.Controls.Add(this.rbnShoriTypeReplace);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.txtTsukidashiRyoE);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtTsukidashiRyoS);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbSteelSize);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbSteelType);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgCreateShabariUkeBase";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "斜梁受けベース";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbSteelType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbSteelSize;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtTsukidashiRyoS;
        private System.Windows.Forms.TextBox txtTsukidashiRyoE;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton rbnShoriTypePileSelect;
        private System.Windows.Forms.RadioButton rbnShoriTypePtoP;
        private System.Windows.Forms.RadioButton rbnShoriTypeReplace;
    }
}