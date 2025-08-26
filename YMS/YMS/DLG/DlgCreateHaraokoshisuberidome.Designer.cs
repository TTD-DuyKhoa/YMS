
namespace YMS.DLG
{
    partial class DlgCreateHaraokoshisuberidome
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgCreateHaraokoshisuberidome));
            this.grpBuzaiType = new System.Windows.Forms.GroupBox();
            this.rbnTypeChannel = new System.Windows.Forms.RadioButton();
            this.rbnTypeHojoPiece = new System.Windows.Forms.RadioButton();
            this.cmbBuzaiSize = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.cmbBoltType = new System.Windows.Forms.ComboBox();
            this.cmbBoltSize = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.txtBoltNum = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpBuzaiSize = new System.Windows.Forms.GroupBox();
            this.grpSuberiDirection = new System.Windows.Forms.GroupBox();
            this.picSuberiDirectionL = new System.Windows.Forms.PictureBox();
            this.picSuberiDirectionR = new System.Windows.Forms.PictureBox();
            this.rbnSuberiDirectionL = new System.Windows.Forms.RadioButton();
            this.rbnSuberiDirectionR = new System.Windows.Forms.RadioButton();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.grpBuzaiType.SuspendLayout();
            this.grpBuzaiSize.SuspendLayout();
            this.grpSuberiDirection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picSuberiDirectionL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSuberiDirectionR)).BeginInit();
            this.SuspendLayout();
            // 
            // grpBuzaiType
            // 
            this.grpBuzaiType.Controls.Add(this.rbnTypeChannel);
            this.grpBuzaiType.Controls.Add(this.rbnTypeHojoPiece);
            this.grpBuzaiType.Location = new System.Drawing.Point(12, 12);
            this.grpBuzaiType.Name = "grpBuzaiType";
            this.grpBuzaiType.Size = new System.Drawing.Size(240, 50);
            this.grpBuzaiType.TabIndex = 0;
            this.grpBuzaiType.TabStop = false;
            this.grpBuzaiType.Text = "部材タイプ";
            // 
            // rbnTypeChannel
            // 
            this.rbnTypeChannel.AutoSize = true;
            this.rbnTypeChannel.Location = new System.Drawing.Point(87, 18);
            this.rbnTypeChannel.Name = "rbnTypeChannel";
            this.rbnTypeChannel.Size = new System.Drawing.Size(148, 16);
            this.rbnTypeChannel.TabIndex = 1;
            this.rbnTypeChannel.Text = "チャンネル(PL)＋チャンネル";
            this.rbnTypeChannel.UseVisualStyleBackColor = true;
            this.rbnTypeChannel.CheckedChanged += new System.EventHandler(this.rbnPartsTypeChannel_CheckedChanged);
            // 
            // rbnTypeHojoPiece
            // 
            this.rbnTypeHojoPiece.AutoSize = true;
            this.rbnTypeHojoPiece.Checked = true;
            this.rbnTypeHojoPiece.Cursor = System.Windows.Forms.Cursors.Default;
            this.rbnTypeHojoPiece.Location = new System.Drawing.Point(6, 18);
            this.rbnTypeHojoPiece.Name = "rbnTypeHojoPiece";
            this.rbnTypeHojoPiece.Size = new System.Drawing.Size(75, 16);
            this.rbnTypeHojoPiece.TabIndex = 0;
            this.rbnTypeHojoPiece.TabStop = true;
            this.rbnTypeHojoPiece.Text = "補助ピース";
            this.rbnTypeHojoPiece.UseVisualStyleBackColor = true;
            this.rbnTypeHojoPiece.CheckedChanged += new System.EventHandler(this.rbnPartsTypeHojoPeace_CheckedChanged);
            // 
            // cmbBuzaiSize
            // 
            this.cmbBuzaiSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBuzaiSize.FormattingEnabled = true;
            this.cmbBuzaiSize.Location = new System.Drawing.Point(6, 18);
            this.cmbBuzaiSize.Name = "cmbBuzaiSize";
            this.cmbBuzaiSize.Size = new System.Drawing.Size(228, 20);
            this.cmbBuzaiSize.TabIndex = 0;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 277);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(57, 12);
            this.label10.TabIndex = 4;
            this.label10.Text = "ボルト種類";
            // 
            // cmbBoltType
            // 
            this.cmbBoltType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBoltType.FormattingEnabled = true;
            this.cmbBoltType.Location = new System.Drawing.Point(131, 273);
            this.cmbBoltType.Name = "cmbBoltType";
            this.cmbBoltType.Size = new System.Drawing.Size(121, 20);
            this.cmbBoltType.TabIndex = 5;
            this.cmbBoltType.SelectedIndexChanged += new System.EventHandler(this.cmbBoltType_SelectedIndexChanged);
            // 
            // cmbBoltSize
            // 
            this.cmbBoltSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBoltSize.FormattingEnabled = true;
            this.cmbBoltSize.Location = new System.Drawing.Point(131, 302);
            this.cmbBoltSize.Name = "cmbBoltSize";
            this.cmbBoltSize.Size = new System.Drawing.Size(121, 20);
            this.cmbBoltSize.TabIndex = 7;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(12, 306);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(62, 12);
            this.label11.TabIndex = 6;
            this.label11.Text = "ボルトサイズ";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(12, 331);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(57, 12);
            this.label12.TabIndex = 8;
            this.label12.Text = "ボルト本数";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(235, 331);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(17, 12);
            this.label13.TabIndex = 10;
            this.label13.Text = "本";
            // 
            // txtBoltNum
            // 
            this.txtBoltNum.Location = new System.Drawing.Point(131, 328);
            this.txtBoltNum.Name = "txtBoltNum";
            this.txtBoltNum.Size = new System.Drawing.Size(84, 19);
            this.txtBoltNum.TabIndex = 9;
            this.txtBoltNum.Text = "0";
            this.txtBoltNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(12, 371);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(120, 23);
            this.btnOK.TabIndex = 11;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(132, 371);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(120, 23);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // grpBuzaiSize
            // 
            this.grpBuzaiSize.Controls.Add(this.cmbBuzaiSize);
            this.grpBuzaiSize.Location = new System.Drawing.Point(12, 113);
            this.grpBuzaiSize.Name = "grpBuzaiSize";
            this.grpBuzaiSize.Size = new System.Drawing.Size(240, 50);
            this.grpBuzaiSize.TabIndex = 2;
            this.grpBuzaiSize.TabStop = false;
            this.grpBuzaiSize.Text = "部材サイズ";
            // 
            // grpSuberiDirection
            // 
            this.grpSuberiDirection.Controls.Add(this.picSuberiDirectionL);
            this.grpSuberiDirection.Controls.Add(this.picSuberiDirectionR);
            this.grpSuberiDirection.Controls.Add(this.rbnSuberiDirectionL);
            this.grpSuberiDirection.Controls.Add(this.rbnSuberiDirectionR);
            this.grpSuberiDirection.Location = new System.Drawing.Point(12, 169);
            this.grpSuberiDirection.Name = "grpSuberiDirection";
            this.grpSuberiDirection.Size = new System.Drawing.Size(240, 96);
            this.grpSuberiDirection.TabIndex = 3;
            this.grpSuberiDirection.TabStop = false;
            this.grpSuberiDirection.Text = "すべり方向";
            // 
            // picSuberiDirectionL
            // 
            this.picSuberiDirectionL.Image = ((System.Drawing.Image)(resources.GetObject("picSuberiDirectionL.Image")));
            this.picSuberiDirectionL.Location = new System.Drawing.Point(145, 18);
            this.picSuberiDirectionL.Name = "picSuberiDirectionL";
            this.picSuberiDirectionL.Size = new System.Drawing.Size(70, 70);
            this.picSuberiDirectionL.TabIndex = 4;
            this.picSuberiDirectionL.TabStop = false;
            this.picSuberiDirectionL.Click += new System.EventHandler(this.picSuberiDirectionL_Click);
            // 
            // picSuberiDirectionR
            // 
            this.picSuberiDirectionR.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.picSuberiDirectionR.Image = ((System.Drawing.Image)(resources.GetObject("picSuberiDirectionR.Image")));
            this.picSuberiDirectionR.Location = new System.Drawing.Point(45, 18);
            this.picSuberiDirectionR.Name = "picSuberiDirectionR";
            this.picSuberiDirectionR.Size = new System.Drawing.Size(70, 70);
            this.picSuberiDirectionR.TabIndex = 3;
            this.picSuberiDirectionR.TabStop = false;
            this.picSuberiDirectionR.Click += new System.EventHandler(this.picSuberiDirectionR_Click);
            // 
            // rbnSuberiDirectionL
            // 
            this.rbnSuberiDirectionL.AutoSize = true;
            this.rbnSuberiDirectionL.Location = new System.Drawing.Point(125, 18);
            this.rbnSuberiDirectionL.Name = "rbnSuberiDirectionL";
            this.rbnSuberiDirectionL.Size = new System.Drawing.Size(14, 13);
            this.rbnSuberiDirectionL.TabIndex = 1;
            this.rbnSuberiDirectionL.UseVisualStyleBackColor = true;
            // 
            // rbnSuberiDirectionR
            // 
            this.rbnSuberiDirectionR.AutoSize = true;
            this.rbnSuberiDirectionR.Checked = true;
            this.rbnSuberiDirectionR.Location = new System.Drawing.Point(25, 18);
            this.rbnSuberiDirectionR.Name = "rbnSuberiDirectionR";
            this.rbnSuberiDirectionR.Size = new System.Drawing.Size(14, 13);
            this.rbnSuberiDirectionR.TabIndex = 0;
            this.rbnSuberiDirectionR.TabStop = true;
            this.rbnSuberiDirectionR.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Location = new System.Drawing.Point(12, 68);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(240, 39);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = "※配置対象が鋼矢板の場合\r\n　部材が配置可能か否かは自動で判別されます";
            // 
            // DlgCreateHaraokoshisuberidome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(264, 401);
            this.ControlBox = false;
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.grpSuberiDirection);
            this.Controls.Add(this.grpBuzaiSize);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtBoltNum);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.cmbBoltSize);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.cmbBoltType);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.grpBuzaiType);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgCreateHaraokoshisuberidome";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "腹起スベリ止メ";
            this.grpBuzaiType.ResumeLayout(false);
            this.grpBuzaiType.PerformLayout();
            this.grpBuzaiSize.ResumeLayout(false);
            this.grpSuberiDirection.ResumeLayout(false);
            this.grpSuberiDirection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picSuberiDirectionL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSuberiDirectionR)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpBuzaiType;
        private System.Windows.Forms.ComboBox cmbBuzaiSize;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox cmbBoltType;
        private System.Windows.Forms.ComboBox cmbBoltSize;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtBoltNum;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.RadioButton rbnTypeChannel;
        private System.Windows.Forms.RadioButton rbnTypeHojoPiece;
        private System.Windows.Forms.GroupBox grpBuzaiSize;
        private System.Windows.Forms.GroupBox grpSuberiDirection;
        private System.Windows.Forms.PictureBox picSuberiDirectionL;
        private System.Windows.Forms.PictureBox picSuberiDirectionR;
        private System.Windows.Forms.RadioButton rbnSuberiDirectionL;
        private System.Windows.Forms.RadioButton rbnSuberiDirectionR;
        private System.Windows.Forms.TextBox textBox1;
    }
}