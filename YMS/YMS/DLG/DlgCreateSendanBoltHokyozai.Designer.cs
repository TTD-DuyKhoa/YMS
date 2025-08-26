namespace YMS.DLG
{
    partial class DlgCreateSendanBoltHokyozai
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
            this.cmbSteelType = new System.Windows.Forms.ComboBox();
            this.cmbSteelSize = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtBoltNum = new System.Windows.Forms.TextBox();
            this.label48 = new System.Windows.Forms.Label();
            this.cmbBoltType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbBoltSize = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnHint = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cmbSteelType
            // 
            this.cmbSteelType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSteelType.FormattingEnabled = true;
            this.cmbSteelType.Location = new System.Drawing.Point(119, 7);
            this.cmbSteelType.Name = "cmbSteelType";
            this.cmbSteelType.Size = new System.Drawing.Size(121, 20);
            this.cmbSteelType.TabIndex = 65;
            // 
            // cmbSteelSize
            // 
            this.cmbSteelSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSteelSize.FormattingEnabled = true;
            this.cmbSteelSize.Items.AddRange(new object[] {
            "H-300×300×10×15"});
            this.cmbSteelSize.Location = new System.Drawing.Point(119, 33);
            this.cmbSteelSize.Name = "cmbSteelSize";
            this.cmbSteelSize.Size = new System.Drawing.Size(121, 20);
            this.cmbSteelSize.TabIndex = 66;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 36);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(58, 12);
            this.label7.TabIndex = 64;
            this.label7.Text = "鋼材サイズ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 11);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 12);
            this.label6.TabIndex = 63;
            this.label6.Text = "鋼材タイプ";
            // 
            // txtBoltNum
            // 
            this.txtBoltNum.Location = new System.Drawing.Point(119, 111);
            this.txtBoltNum.Name = "txtBoltNum";
            this.txtBoltNum.Size = new System.Drawing.Size(121, 19);
            this.txtBoltNum.TabIndex = 77;
            this.txtBoltNum.Text = "0";
            this.txtBoltNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label48
            // 
            this.label48.AutoSize = true;
            this.label48.Location = new System.Drawing.Point(12, 114);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(57, 12);
            this.label48.TabIndex = 76;
            this.label48.Text = "ボルト本数";
            // 
            // cmbBoltType
            // 
            this.cmbBoltType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBoltType.FormattingEnabled = true;
            this.cmbBoltType.Items.AddRange(new object[] {
            "ハイテンションボルト"});
            this.cmbBoltType.Location = new System.Drawing.Point(119, 59);
            this.cmbBoltType.Name = "cmbBoltType";
            this.cmbBoltType.Size = new System.Drawing.Size(121, 20);
            this.cmbBoltType.TabIndex = 79;
            this.cmbBoltType.SelectedIndexChanged += new System.EventHandler(this.cmbBoltType_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 63);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 78;
            this.label1.Text = "ボルトタイプ";
            // 
            // cmbBoltSize
            // 
            this.cmbBoltSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBoltSize.FormattingEnabled = true;
            this.cmbBoltSize.Items.AddRange(new object[] {
            "F10T-100"});
            this.cmbBoltSize.Location = new System.Drawing.Point(119, 85);
            this.cmbBoltSize.Name = "cmbBoltSize";
            this.cmbBoltSize.Size = new System.Drawing.Size(121, 20);
            this.cmbBoltSize.TabIndex = 81;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 89);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 12);
            this.label2.TabIndex = 80;
            this.label2.Text = "ボルトサイズ";
            // 
            // btnHint
            // 
            this.btnHint.Location = new System.Drawing.Point(5, 137);
            this.btnHint.Name = "btnHint";
            this.btnHint.Size = new System.Drawing.Size(76, 23);
            this.btnHint.TabIndex = 82;
            this.btnHint.Text = "ヒント";
            this.btnHint.UseVisualStyleBackColor = true;
            this.btnHint.Click += new System.EventHandler(this.btnHint_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(166, 166);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(76, 23);
            this.btnCancel.TabIndex = 84;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(84, 166);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 23);
            this.btnOK.TabIndex = 83;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("MS UI Gothic", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label4.Location = new System.Drawing.Point(87, 143);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(122, 11);
            this.label4.TabIndex = 85;
            this.label4.Text = "別途PDFファイルが開きます";
            // 
            // DlgCreateSendanBoltHokyozai
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(254, 201);
            this.ControlBox = false;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnHint);
            this.Controls.Add(this.cmbBoltSize);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbBoltType);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtBoltNum);
            this.Controls.Add(this.label48);
            this.Controls.Add(this.cmbSteelType);
            this.Controls.Add(this.cmbSteelSize);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgCreateSendanBoltHokyozai";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "せん断ボルト補強材";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbSteelType;
        private System.Windows.Forms.ComboBox cmbSteelSize;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtBoltNum;
        private System.Windows.Forms.Label label48;
        private System.Windows.Forms.ComboBox cmbBoltType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbBoltSize;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnHint;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label4;
    }
}