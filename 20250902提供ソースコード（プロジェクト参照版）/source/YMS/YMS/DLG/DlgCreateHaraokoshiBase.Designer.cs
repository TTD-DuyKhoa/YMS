namespace YMS.DLG
{
    partial class DlgCreateHaraokoshiBase
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbnReplace = new System.Windows.Forms.RadioButton();
            this.rbnShoriTypePtoPOut = new System.Windows.Forms.RadioButton();
            this.rbnShoriTypePtoP = new System.Windows.Forms.RadioButton();
            this.rbnShoriTypeBaseLineOut = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmbHaichiLevel = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rbnDanLevelLower = new System.Windows.Forms.RadioButton();
            this.rbnDanLevelJust = new System.Windows.Forms.RadioButton();
            this.rbnDanLevelUpper = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rbnSideDouble = new System.Windows.Forms.RadioButton();
            this.rbnSideSingle = new System.Windows.Forms.RadioButton();
            this.grpVerticalCount = new System.Windows.Forms.GroupBox();
            this.rbnVerticalDouble = new System.Windows.Forms.RadioButton();
            this.rbnVerticalSingle = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbSteelType = new System.Windows.Forms.ComboBox();
            this.cmbSteelSize = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtOffset = new System.Windows.Forms.TextBox();
            this.txtVerticalGap = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbGapAdjustSteelType = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cmbGapAdjustSteel = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtGapAdjustSteelLength = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.rbnLose = new System.Windows.Forms.RadioButton();
            this.rbnWin = new System.Windows.Forms.RadioButton();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.txtMegaBeam = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.grpVerticalCount.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbnReplace);
            this.groupBox1.Controls.Add(this.rbnShoriTypePtoPOut);
            this.groupBox1.Controls.Add(this.rbnShoriTypePtoP);
            this.groupBox1.Controls.Add(this.rbnShoriTypeBaseLineOut);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(222, 86);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "処理方法";
            // 
            // rbnReplace
            // 
            this.rbnReplace.AutoSize = true;
            this.rbnReplace.Location = new System.Drawing.Point(6, 63);
            this.rbnReplace.Name = "rbnReplace";
            this.rbnReplace.Size = new System.Drawing.Size(47, 16);
            this.rbnReplace.TabIndex = 3;
            this.rbnReplace.Text = "置換";
            this.rbnReplace.UseVisualStyleBackColor = true;
            // 
            // rbnShoriTypePtoPOut
            // 
            this.rbnShoriTypePtoPOut.AutoSize = true;
            this.rbnShoriTypePtoPOut.Location = new System.Drawing.Point(6, 41);
            this.rbnShoriTypePtoPOut.Name = "rbnShoriTypePtoPOut";
            this.rbnShoriTypePtoPOut.Size = new System.Drawing.Size(102, 16);
            this.rbnShoriTypePtoPOut.TabIndex = 1;
            this.rbnShoriTypePtoPOut.Text = "2点間（ズレあり）";
            this.rbnShoriTypePtoPOut.UseVisualStyleBackColor = true;
            // 
            // rbnShoriTypePtoP
            // 
            this.rbnShoriTypePtoP.AutoSize = true;
            this.rbnShoriTypePtoP.Location = new System.Drawing.Point(114, 41);
            this.rbnShoriTypePtoP.Name = "rbnShoriTypePtoP";
            this.rbnShoriTypePtoP.Size = new System.Drawing.Size(103, 16);
            this.rbnShoriTypePtoP.TabIndex = 2;
            this.rbnShoriTypePtoP.Text = "2点間（ズレなし）";
            this.rbnShoriTypePtoP.UseVisualStyleBackColor = true;
            // 
            // rbnShoriTypeBaseLineOut
            // 
            this.rbnShoriTypeBaseLineOut.AutoSize = true;
            this.rbnShoriTypeBaseLineOut.Checked = true;
            this.rbnShoriTypeBaseLineOut.Location = new System.Drawing.Point(6, 19);
            this.rbnShoriTypeBaseLineOut.Name = "rbnShoriTypeBaseLineOut";
            this.rbnShoriTypeBaseLineOut.Size = new System.Drawing.Size(108, 16);
            this.rbnShoriTypeBaseLineOut.TabIndex = 0;
            this.rbnShoriTypeBaseLineOut.TabStop = true;
            this.rbnShoriTypeBaseLineOut.Text = "基準線（ズレあり）";
            this.rbnShoriTypeBaseLineOut.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cmbHaichiLevel);
            this.groupBox2.Location = new System.Drawing.Point(13, 121);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(222, 53);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "配置レベル";
            // 
            // cmbHaichiLevel
            // 
            this.cmbHaichiLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbHaichiLevel.FormattingEnabled = true;
            this.cmbHaichiLevel.Location = new System.Drawing.Point(7, 19);
            this.cmbHaichiLevel.Name = "cmbHaichiLevel";
            this.cmbHaichiLevel.Size = new System.Drawing.Size(206, 20);
            this.cmbHaichiLevel.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rbnDanLevelLower);
            this.groupBox3.Controls.Add(this.rbnDanLevelJust);
            this.groupBox3.Controls.Add(this.rbnDanLevelUpper);
            this.groupBox3.Location = new System.Drawing.Point(12, 196);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(223, 45);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "段レベル";
            // 
            // rbnDanLevelLower
            // 
            this.rbnDanLevelLower.AutoSize = true;
            this.rbnDanLevelLower.Location = new System.Drawing.Point(160, 18);
            this.rbnDanLevelLower.Name = "rbnDanLevelLower";
            this.rbnDanLevelLower.Size = new System.Drawing.Size(47, 16);
            this.rbnDanLevelLower.TabIndex = 2;
            this.rbnDanLevelLower.Tag = "2";
            this.rbnDanLevelLower.Text = "下段";
            this.rbnDanLevelLower.UseVisualStyleBackColor = true;
            // 
            // rbnDanLevelJust
            // 
            this.rbnDanLevelJust.AutoSize = true;
            this.rbnDanLevelJust.Location = new System.Drawing.Point(91, 18);
            this.rbnDanLevelJust.Name = "rbnDanLevelJust";
            this.rbnDanLevelJust.Size = new System.Drawing.Size(47, 16);
            this.rbnDanLevelJust.TabIndex = 1;
            this.rbnDanLevelJust.Tag = "0";
            this.rbnDanLevelJust.Text = "同段";
            this.rbnDanLevelJust.UseVisualStyleBackColor = true;
            // 
            // rbnDanLevelUpper
            // 
            this.rbnDanLevelUpper.AutoSize = true;
            this.rbnDanLevelUpper.Checked = true;
            this.rbnDanLevelUpper.Location = new System.Drawing.Point(22, 18);
            this.rbnDanLevelUpper.Name = "rbnDanLevelUpper";
            this.rbnDanLevelUpper.Size = new System.Drawing.Size(47, 16);
            this.rbnDanLevelUpper.TabIndex = 0;
            this.rbnDanLevelUpper.TabStop = true;
            this.rbnDanLevelUpper.Tag = "1";
            this.rbnDanLevelUpper.Text = "上段";
            this.rbnDanLevelUpper.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rbnSideDouble);
            this.groupBox4.Controls.Add(this.rbnSideSingle);
            this.groupBox4.Location = new System.Drawing.Point(13, 263);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(223, 45);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "横本数";
            // 
            // rbnSideDouble
            // 
            this.rbnSideDouble.AutoSize = true;
            this.rbnSideDouble.Location = new System.Drawing.Point(114, 18);
            this.rbnSideDouble.Name = "rbnSideDouble";
            this.rbnSideDouble.Size = new System.Drawing.Size(51, 16);
            this.rbnSideDouble.TabIndex = 1;
            this.rbnSideDouble.TabStop = true;
            this.rbnSideDouble.Text = "ダブル";
            this.rbnSideDouble.UseVisualStyleBackColor = true;
            this.rbnSideDouble.CheckedChanged += new System.EventHandler(this.rbnSideDouble_CheckedChanged);
            // 
            // rbnSideSingle
            // 
            this.rbnSideSingle.AutoSize = true;
            this.rbnSideSingle.Checked = true;
            this.rbnSideSingle.Location = new System.Drawing.Point(47, 18);
            this.rbnSideSingle.Name = "rbnSideSingle";
            this.rbnSideSingle.Size = new System.Drawing.Size(61, 16);
            this.rbnSideSingle.TabIndex = 0;
            this.rbnSideSingle.TabStop = true;
            this.rbnSideSingle.Text = "シングル";
            this.rbnSideSingle.UseVisualStyleBackColor = true;
            // 
            // grpVerticalCount
            // 
            this.grpVerticalCount.Controls.Add(this.rbnVerticalDouble);
            this.grpVerticalCount.Controls.Add(this.rbnVerticalSingle);
            this.grpVerticalCount.Location = new System.Drawing.Point(12, 330);
            this.grpVerticalCount.Name = "grpVerticalCount";
            this.grpVerticalCount.Size = new System.Drawing.Size(223, 45);
            this.grpVerticalCount.TabIndex = 4;
            this.grpVerticalCount.TabStop = false;
            this.grpVerticalCount.Text = "縦本数";
            // 
            // rbnVerticalDouble
            // 
            this.rbnVerticalDouble.AutoSize = true;
            this.rbnVerticalDouble.Location = new System.Drawing.Point(115, 19);
            this.rbnVerticalDouble.Name = "rbnVerticalDouble";
            this.rbnVerticalDouble.Size = new System.Drawing.Size(51, 16);
            this.rbnVerticalDouble.TabIndex = 1;
            this.rbnVerticalDouble.TabStop = true;
            this.rbnVerticalDouble.Text = "ダブル";
            this.rbnVerticalDouble.UseVisualStyleBackColor = true;
            this.rbnVerticalDouble.CheckedChanged += new System.EventHandler(this.rbnVerticalDouble_CheckedChanged);
            // 
            // rbnVerticalSingle
            // 
            this.rbnVerticalSingle.AutoSize = true;
            this.rbnVerticalSingle.Checked = true;
            this.rbnVerticalSingle.Location = new System.Drawing.Point(48, 19);
            this.rbnVerticalSingle.Name = "rbnVerticalSingle";
            this.rbnVerticalSingle.Size = new System.Drawing.Size(61, 16);
            this.rbnVerticalSingle.TabIndex = 0;
            this.rbnVerticalSingle.TabStop = true;
            this.rbnVerticalSingle.Text = "シングル";
            this.rbnVerticalSingle.UseVisualStyleBackColor = true;
            this.rbnVerticalSingle.CheckedChanged += new System.EventHandler(this.rbnVerticalSingle_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(263, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "鋼材タイプ";
            // 
            // cmbSteelType
            // 
            this.cmbSteelType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSteelType.FormattingEnabled = true;
            this.cmbSteelType.Items.AddRange(new object[] {
            "主材"});
            this.cmbSteelType.Location = new System.Drawing.Point(387, 13);
            this.cmbSteelType.Name = "cmbSteelType";
            this.cmbSteelType.Size = new System.Drawing.Size(204, 20);
            this.cmbSteelType.TabIndex = 6;
            this.cmbSteelType.SelectedIndexChanged += new System.EventHandler(this.cmbSteelType_SelectedIndexChanged);
            // 
            // cmbSteelSize
            // 
            this.cmbSteelSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSteelSize.FormattingEnabled = true;
            this.cmbSteelSize.Items.AddRange(new object[] {
            "H300X300X10X15",
            "25HA",
            "35HA",
            "40HA"});
            this.cmbSteelSize.Location = new System.Drawing.Point(387, 44);
            this.cmbSteelSize.Name = "cmbSteelSize";
            this.cmbSteelSize.Size = new System.Drawing.Size(204, 20);
            this.cmbSteelSize.TabIndex = 8;
            this.cmbSteelSize.SelectedIndexChanged += new System.EventHandler(this.cmbSteel_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(263, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "鋼材サイズ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(267, 112);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 11;
            this.label3.Text = "オフセット量";
            // 
            // txtOffset
            // 
            this.txtOffset.Location = new System.Drawing.Point(391, 109);
            this.txtOffset.Name = "txtOffset";
            this.txtOffset.Size = new System.Drawing.Size(204, 19);
            this.txtOffset.TabIndex = 12;
            this.txtOffset.Text = "0.0";
            this.txtOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtVerticalGap
            // 
            this.txtVerticalGap.Location = new System.Drawing.Point(391, 140);
            this.txtVerticalGap.Name = "txtVerticalGap";
            this.txtVerticalGap.Size = new System.Drawing.Size(204, 19);
            this.txtVerticalGap.TabIndex = 15;
            this.txtVerticalGap.Text = "0.0";
            this.txtVerticalGap.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(267, 143);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 12);
            this.label4.TabIndex = 14;
            this.label4.Text = "縦方向の隙間";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(601, 143);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(23, 12);
            this.label5.TabIndex = 16;
            this.label5.Text = "mm";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(601, 112);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(23, 12);
            this.label6.TabIndex = 13;
            this.label6.Text = "mm";
            // 
            // cmbGapAdjustSteelType
            // 
            this.cmbGapAdjustSteelType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGapAdjustSteelType.FormattingEnabled = true;
            this.cmbGapAdjustSteelType.Items.AddRange(new object[] {
            "チャンネル"});
            this.cmbGapAdjustSteelType.Location = new System.Drawing.Point(391, 170);
            this.cmbGapAdjustSteelType.Name = "cmbGapAdjustSteelType";
            this.cmbGapAdjustSteelType.Size = new System.Drawing.Size(204, 20);
            this.cmbGapAdjustSteelType.TabIndex = 18;
            this.cmbGapAdjustSteelType.SelectedIndexChanged += new System.EventHandler(this.cmbGapAdjustSteelType_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(267, 174);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(112, 12);
            this.label7.TabIndex = 17;
            this.label7.Text = "隙間調整材鋼材ﾀｲﾌﾟ";
            // 
            // cmbGapAdjustSteel
            // 
            this.cmbGapAdjustSteel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGapAdjustSteel.FormattingEnabled = true;
            this.cmbGapAdjustSteel.Items.AddRange(new object[] {
            "C-100x50x5"});
            this.cmbGapAdjustSteel.Location = new System.Drawing.Point(391, 201);
            this.cmbGapAdjustSteel.Name = "cmbGapAdjustSteel";
            this.cmbGapAdjustSteel.Size = new System.Drawing.Size(204, 20);
            this.cmbGapAdjustSteel.TabIndex = 20;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(267, 205);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 12);
            this.label8.TabIndex = 19;
            this.label8.Text = "隙間調整材";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(601, 236);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(23, 12);
            this.label9.TabIndex = 23;
            this.label9.Text = "mm";
            // 
            // txtGapAdjustSteelLength
            // 
            this.txtGapAdjustSteelLength.Location = new System.Drawing.Point(391, 233);
            this.txtGapAdjustSteelLength.Name = "txtGapAdjustSteelLength";
            this.txtGapAdjustSteelLength.Size = new System.Drawing.Size(204, 19);
            this.txtGapAdjustSteelLength.TabIndex = 22;
            this.txtGapAdjustSteelLength.Text = "400";
            this.txtGapAdjustSteelLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(267, 236);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(85, 12);
            this.label10.TabIndex = 21;
            this.label10.Text = "隙間調整材長さ";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.rbnLose);
            this.groupBox6.Controls.Add(this.rbnWin);
            this.groupBox6.Location = new System.Drawing.Point(267, 263);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(324, 68);
            this.groupBox6.TabIndex = 24;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "勝ち負け";
            // 
            // rbnLose
            // 
            this.rbnLose.AutoSize = true;
            this.rbnLose.Location = new System.Drawing.Point(6, 41);
            this.rbnLose.Name = "rbnLose";
            this.rbnLose.Size = new System.Drawing.Size(45, 16);
            this.rbnLose.TabIndex = 2;
            this.rbnLose.Text = "負け";
            this.rbnLose.UseVisualStyleBackColor = true;
            // 
            // rbnWin
            // 
            this.rbnWin.AutoSize = true;
            this.rbnWin.Checked = true;
            this.rbnWin.Location = new System.Drawing.Point(7, 19);
            this.rbnWin.Name = "rbnWin";
            this.rbnWin.Size = new System.Drawing.Size(44, 16);
            this.rbnWin.TabIndex = 0;
            this.rbnWin.TabStop = true;
            this.rbnWin.Text = "勝ち";
            this.rbnWin.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(547, 394);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 26;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(466, 394);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 25;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.button2_Click);
            // 
            // txtMegaBeam
            // 
            this.txtMegaBeam.Location = new System.Drawing.Point(470, 78);
            this.txtMegaBeam.Name = "txtMegaBeam";
            this.txtMegaBeam.Size = new System.Drawing.Size(121, 19);
            this.txtMegaBeam.TabIndex = 9;
            this.txtMegaBeam.Text = "0";
            this.txtMegaBeam.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(601, 81);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(17, 12);
            this.label11.TabIndex = 10;
            this.label11.Text = "本";
            // 
            // DlgCreateHaraokoshiBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 421);
            this.ControlBox = false;
            this.Controls.Add(this.label11);
            this.Controls.Add(this.txtMegaBeam);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtGapAdjustSteelLength);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.cmbGapAdjustSteel);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.cmbGapAdjustSteelType);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtVerticalGap);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtOffset);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbSteelSize);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbSteelType);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.grpVerticalCount);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgCreateHaraokoshiBase";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "腹起ベース作成";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.DlgCreateHaraokoshiBase_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.grpVerticalCount.ResumeLayout(false);
            this.grpVerticalCount.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbnShoriTypePtoP;
        private System.Windows.Forms.RadioButton rbnShoriTypeBaseLineOut;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cmbHaichiLevel;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rbnDanLevelLower;
        private System.Windows.Forms.RadioButton rbnDanLevelJust;
        private System.Windows.Forms.RadioButton rbnDanLevelUpper;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton rbnSideDouble;
        private System.Windows.Forms.RadioButton rbnSideSingle;
        private System.Windows.Forms.GroupBox grpVerticalCount;
        private System.Windows.Forms.RadioButton rbnVerticalDouble;
        private System.Windows.Forms.RadioButton rbnVerticalSingle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbSteelType;
        private System.Windows.Forms.ComboBox cmbSteelSize;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtOffset;
        private System.Windows.Forms.TextBox txtVerticalGap;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbGapAdjustSteelType;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmbGapAdjustSteel;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtGapAdjustSteelLength;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.RadioButton rbnLose;
        private System.Windows.Forms.RadioButton rbnWin;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.RadioButton rbnShoriTypePtoPOut;
        private System.Windows.Forms.RadioButton rbnReplace;
        private System.Windows.Forms.TextBox txtMegaBeam;
        private System.Windows.Forms.Label label11;
    }
}