
namespace YMS_gantry.UI
{
    partial class FrmPutOhbikiKetauke
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
            this.CmbKoudaiName = new System.Windows.Forms.ComboBox();
            this.label24 = new System.Windows.Forms.Label();
            this.CmbSizeType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.RbtKui = new System.Windows.Forms.RadioButton();
            this.button1 = new System.Windows.Forms.Button();
            this.RbtFree = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.NmcOffset = new System.Windows.Forms.NumericUpDown();
            this.label86 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.CmbLevel = new System.Windows.Forms.ComboBox();
            this.label34 = new System.Windows.Forms.Label();
            this.CmbMaterial = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label11 = new System.Windows.Forms.Label();
            this.RbtBothSide = new System.Windows.Forms.RadioButton();
            this.RbtOneSide = new System.Windows.Forms.RadioButton();
            this.NmcOhbikiBoltCnt = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.RbtOhbikiBHTB = new System.Windows.Forms.RadioButton();
            this.RbtOhbikiNormalB = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.RbtOhbikiWeld = new System.Windows.Forms.RadioButton();
            this.RbtOhbikiBolt = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.NmcELng = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.NmcSLng = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.CmbSize = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.RbtOhbikiSingle = new System.Windows.Forms.RadioButton();
            this.RbtOhbikiDouble = new System.Windows.Forms.RadioButton();
            this.RbtOhbikiTriple = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NmcOffset)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NmcOhbikiBoltCnt)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NmcELng)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NmcSLng)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // CmbKoudaiName
            // 
            this.CmbKoudaiName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbKoudaiName.FormattingEnabled = true;
            this.CmbKoudaiName.Location = new System.Drawing.Point(73, 12);
            this.CmbKoudaiName.Name = "CmbKoudaiName";
            this.CmbKoudaiName.Size = new System.Drawing.Size(280, 20);
            this.CmbKoudaiName.TabIndex = 1;
            this.CmbKoudaiName.SelectedIndexChanged += new System.EventHandler(this.comboBox12_SelectedIndexChanged);
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(10, 15);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(29, 12);
            this.label24.TabIndex = 0;
            this.label24.Text = "構台";
            // 
            // CmbSizeType
            // 
            this.CmbSizeType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbSizeType.FormattingEnabled = true;
            this.CmbSizeType.Items.AddRange(new object[] {
            "H鋼",
            "C材",
            "L材"});
            this.CmbSizeType.Location = new System.Drawing.Point(73, 40);
            this.CmbSizeType.Name = "CmbSizeType";
            this.CmbSizeType.Size = new System.Drawing.Size(133, 20);
            this.CmbSizeType.TabIndex = 3;
            this.CmbSizeType.SelectedIndexChanged += new System.EventHandler(this.comboBox3_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "サイズ";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(370, 369);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 13;
            this.button3.Text = "キャンセル";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // RbtKui
            // 
            this.RbtKui.AutoSize = true;
            this.RbtKui.Checked = true;
            this.RbtKui.Location = new System.Drawing.Point(6, 21);
            this.RbtKui.Name = "RbtKui";
            this.RbtKui.Size = new System.Drawing.Size(59, 16);
            this.RbtKui.TabIndex = 0;
            this.RbtKui.TabStop = true;
            this.RbtKui.Text = "杭指定";
            this.RbtKui.UseVisualStyleBackColor = true;
            this.RbtKui.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(289, 369);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 12;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // RbtFree
            // 
            this.RbtFree.AutoSize = true;
            this.RbtFree.Location = new System.Drawing.Point(71, 21);
            this.RbtFree.Name = "RbtFree";
            this.RbtFree.Size = new System.Drawing.Size(95, 16);
            this.RbtFree.TabIndex = 1;
            this.RbtFree.Text = "自由位置指定";
            this.RbtFree.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.NmcOffset);
            this.groupBox1.Controls.Add(this.label86);
            this.groupBox1.Controls.Add(this.label40);
            this.groupBox1.Controls.Add(this.RbtKui);
            this.groupBox1.Controls.Add(this.CmbLevel);
            this.groupBox1.Controls.Add(this.RbtFree);
            this.groupBox1.Controls.Add(this.label34);
            this.groupBox1.Location = new System.Drawing.Point(12, 124);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(251, 96);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "配置方法";
            // 
            // NmcOffset
            // 
            this.NmcOffset.Location = new System.Drawing.Point(140, 66);
            this.NmcOffset.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.NmcOffset.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.NmcOffset.Name = "NmcOffset";
            this.NmcOffset.Size = new System.Drawing.Size(73, 19);
            this.NmcOffset.TabIndex = 5;
            this.NmcOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NmcOffset.Leave += new System.EventHandler(this.numericUpDown_Leave);
            // 
            // label86
            // 
            this.label86.AutoSize = true;
            this.label86.Location = new System.Drawing.Point(6, 46);
            this.label86.Name = "label86";
            this.label86.Size = new System.Drawing.Size(54, 12);
            this.label86.TabIndex = 2;
            this.label86.Text = "基準ﾚﾍﾞﾙ";
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(222, 70);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(23, 12);
            this.label40.TabIndex = 6;
            this.label40.Text = "mm";
            // 
            // CmbLevel
            // 
            this.CmbLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbLevel.FormattingEnabled = true;
            this.CmbLevel.Location = new System.Drawing.Point(73, 43);
            this.CmbLevel.Name = "CmbLevel";
            this.CmbLevel.Size = new System.Drawing.Size(140, 20);
            this.CmbLevel.TabIndex = 3;
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(6, 70);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(129, 12);
            this.label34.TabIndex = 4;
            this.label34.Text = "基準ﾚﾍﾞﾙからのｵﾌｾｯﾄ値";
            // 
            // CmbMaterial
            // 
            this.CmbMaterial.FormattingEnabled = true;
            this.CmbMaterial.Items.AddRange(new object[] {
            "SS400",
            "SM490"});
            this.CmbMaterial.Location = new System.Drawing.Point(73, 66);
            this.CmbMaterial.Name = "CmbMaterial";
            this.CmbMaterial.Size = new System.Drawing.Size(97, 20);
            this.CmbMaterial.TabIndex = 6;
            this.CmbMaterial.Text = "SS400";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "材質";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.panel3);
            this.groupBox2.Controls.Add(this.NmcOhbikiBoltCnt);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.panel2);
            this.groupBox2.Controls.Add(this.panel1);
            this.groupBox2.Location = new System.Drawing.Point(12, 226);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(435, 137);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "H鋼以外の時";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label11);
            this.panel3.Controls.Add(this.RbtBothSide);
            this.panel3.Controls.Add(this.RbtOneSide);
            this.panel3.Location = new System.Drawing.Point(6, 18);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(200, 22);
            this.panel3.TabIndex = 0;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 5);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(55, 12);
            this.label11.TabIndex = 0;
            this.label11.Text = "取付タイプ";
            // 
            // RbtBothSide
            // 
            this.RbtBothSide.AutoSize = true;
            this.RbtBothSide.Location = new System.Drawing.Point(137, 3);
            this.RbtBothSide.Name = "RbtBothSide";
            this.RbtBothSide.Size = new System.Drawing.Size(47, 16);
            this.RbtBothSide.TabIndex = 2;
            this.RbtBothSide.Text = "両側";
            this.RbtBothSide.UseVisualStyleBackColor = true;
            // 
            // RbtOneSide
            // 
            this.RbtOneSide.AutoSize = true;
            this.RbtOneSide.Checked = true;
            this.RbtOneSide.Location = new System.Drawing.Point(84, 3);
            this.RbtOneSide.Name = "RbtOneSide";
            this.RbtOneSide.Size = new System.Drawing.Size(47, 16);
            this.RbtOneSide.TabIndex = 1;
            this.RbtOneSide.TabStop = true;
            this.RbtOneSide.Text = "片側";
            this.RbtOneSide.UseVisualStyleBackColor = true;
            // 
            // NmcOhbikiBoltCnt
            // 
            this.NmcOhbikiBoltCnt.Location = new System.Drawing.Point(90, 100);
            this.NmcOhbikiBoltCnt.Name = "NmcOhbikiBoltCnt";
            this.NmcOhbikiBoltCnt.Size = new System.Drawing.Size(58, 19);
            this.NmcOhbikiBoltCnt.TabIndex = 5;
            this.NmcOhbikiBoltCnt.Leave += new System.EventHandler(this.numericUpDown_Leave);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 117);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(66, 12);
            this.label6.TabIndex = 4;
            this.label6.Text = "(1ヶ所あたり)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 102);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "ボルト本数";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.RbtOhbikiBHTB);
            this.panel2.Controls.Add(this.RbtOhbikiNormalB);
            this.panel2.Location = new System.Drawing.Point(6, 74);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(200, 22);
            this.panel2.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 5);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "ボルトタイプ";
            // 
            // RbtOhbikiBHTB
            // 
            this.RbtOhbikiBHTB.AutoSize = true;
            this.RbtOhbikiBHTB.Location = new System.Drawing.Point(137, 3);
            this.RbtOhbikiBHTB.Name = "RbtOhbikiBHTB";
            this.RbtOhbikiBHTB.Size = new System.Drawing.Size(46, 16);
            this.RbtOhbikiBHTB.TabIndex = 2;
            this.RbtOhbikiBHTB.Text = "HTB";
            this.RbtOhbikiBHTB.UseVisualStyleBackColor = true;
            // 
            // RbtOhbikiNormalB
            // 
            this.RbtOhbikiNormalB.AutoSize = true;
            this.RbtOhbikiNormalB.Checked = true;
            this.RbtOhbikiNormalB.Location = new System.Drawing.Point(84, 3);
            this.RbtOhbikiNormalB.Name = "RbtOhbikiNormalB";
            this.RbtOhbikiNormalB.Size = new System.Drawing.Size(47, 16);
            this.RbtOhbikiNormalB.TabIndex = 1;
            this.RbtOhbikiNormalB.TabStop = true;
            this.RbtOhbikiNormalB.Text = "普通";
            this.RbtOhbikiNormalB.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.RbtOhbikiWeld);
            this.panel1.Controls.Add(this.RbtOhbikiBolt);
            this.panel1.Location = new System.Drawing.Point(6, 46);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 22);
            this.panel1.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "取付方法";
            // 
            // RbtOhbikiWeld
            // 
            this.RbtOhbikiWeld.AutoSize = true;
            this.RbtOhbikiWeld.Location = new System.Drawing.Point(137, 3);
            this.RbtOhbikiWeld.Name = "RbtOhbikiWeld";
            this.RbtOhbikiWeld.Size = new System.Drawing.Size(47, 16);
            this.RbtOhbikiWeld.TabIndex = 2;
            this.RbtOhbikiWeld.Text = "溶接";
            this.RbtOhbikiWeld.UseVisualStyleBackColor = true;
            // 
            // RbtOhbikiBolt
            // 
            this.RbtOhbikiBolt.AutoSize = true;
            this.RbtOhbikiBolt.Checked = true;
            this.RbtOhbikiBolt.Location = new System.Drawing.Point(84, 3);
            this.RbtOhbikiBolt.Name = "RbtOhbikiBolt";
            this.RbtOhbikiBolt.Size = new System.Drawing.Size(51, 16);
            this.RbtOhbikiBolt.TabIndex = 1;
            this.RbtOhbikiBolt.TabStop = true;
            this.RbtOhbikiBolt.Text = "ボルト";
            this.RbtOhbikiBolt.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.NmcELng);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.NmcSLng);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Location = new System.Drawing.Point(269, 124);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(178, 96);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "突き出し長さ";
            // 
            // NmcELng
            // 
            this.NmcELng.Location = new System.Drawing.Point(55, 59);
            this.NmcELng.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.NmcELng.Name = "NmcELng";
            this.NmcELng.Size = new System.Drawing.Size(73, 19);
            this.NmcELng.TabIndex = 4;
            this.NmcELng.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NmcELng.Leave += new System.EventHandler(this.numericUpDown_Leave);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(134, 28);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(23, 12);
            this.label7.TabIndex = 2;
            this.label7.Text = "mm";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(134, 66);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(23, 12);
            this.label8.TabIndex = 5;
            this.label8.Text = "mm";
            // 
            // NmcSLng
            // 
            this.NmcSLng.Location = new System.Drawing.Point(55, 21);
            this.NmcSLng.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.NmcSLng.Name = "NmcSLng";
            this.NmcSLng.Size = new System.Drawing.Size(73, 19);
            this.NmcSLng.TabIndex = 1;
            this.NmcSLng.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NmcSLng.Leave += new System.EventHandler(this.numericUpDown_Leave);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 61);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 12);
            this.label9.TabIndex = 3;
            this.label9.Text = "終点側";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(8, 25);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(41, 12);
            this.label10.TabIndex = 0;
            this.label10.Text = "始点側";
            // 
            // CmbSize
            // 
            this.CmbSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbSize.FormattingEnabled = true;
            this.CmbSize.Location = new System.Drawing.Point(212, 40);
            this.CmbSize.Name = "CmbSize";
            this.CmbSize.Size = new System.Drawing.Size(141, 20);
            this.CmbSize.TabIndex = 4;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(10, 97);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(17, 12);
            this.label12.TabIndex = 7;
            this.label12.Text = "縦";
            // 
            // RbtOhbikiSingle
            // 
            this.RbtOhbikiSingle.AutoSize = true;
            this.RbtOhbikiSingle.Checked = true;
            this.RbtOhbikiSingle.Location = new System.Drawing.Point(6, 12);
            this.RbtOhbikiSingle.Name = "RbtOhbikiSingle";
            this.RbtOhbikiSingle.Size = new System.Drawing.Size(61, 16);
            this.RbtOhbikiSingle.TabIndex = 0;
            this.RbtOhbikiSingle.TabStop = true;
            this.RbtOhbikiSingle.Text = "シングル";
            this.RbtOhbikiSingle.UseVisualStyleBackColor = true;
            // 
            // RbtOhbikiDouble
            // 
            this.RbtOhbikiDouble.AutoSize = true;
            this.RbtOhbikiDouble.Location = new System.Drawing.Point(73, 12);
            this.RbtOhbikiDouble.Name = "RbtOhbikiDouble";
            this.RbtOhbikiDouble.Size = new System.Drawing.Size(51, 16);
            this.RbtOhbikiDouble.TabIndex = 1;
            this.RbtOhbikiDouble.Text = "ダブル";
            this.RbtOhbikiDouble.UseVisualStyleBackColor = true;
            // 
            // RbtOhbikiTriple
            // 
            this.RbtOhbikiTriple.AutoSize = true;
            this.RbtOhbikiTriple.Location = new System.Drawing.Point(130, 12);
            this.RbtOhbikiTriple.Name = "RbtOhbikiTriple";
            this.RbtOhbikiTriple.Size = new System.Drawing.Size(57, 16);
            this.RbtOhbikiTriple.TabIndex = 2;
            this.RbtOhbikiTriple.Text = "トリプル";
            this.RbtOhbikiTriple.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.RbtOhbikiSingle);
            this.groupBox4.Controls.Add(this.RbtOhbikiTriple);
            this.groupBox4.Controls.Add(this.RbtOhbikiDouble);
            this.groupBox4.Location = new System.Drawing.Point(70, 88);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(190, 34);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            // 
            // FrmPutOhbikiKetauke
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(457, 404);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.CmbSize);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.CmbMaterial);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.CmbKoudaiName);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.CmbSizeType);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmPutOhbikiKetauke";
            this.ShowIcon = false;
            this.Text = "[個別] 大引(桁受)配置";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FrmPutOhbikiKetauke_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NmcOffset)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NmcOhbikiBoltCnt)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NmcELng)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NmcSLng)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.NumericUpDown NmcOffset;
        public System.Windows.Forms.NumericUpDown NmcELng;
        public System.Windows.Forms.NumericUpDown NmcSLng;
        public System.Windows.Forms.ComboBox CmbKoudaiName;
        public System.Windows.Forms.Label label24;
        public System.Windows.Forms.ComboBox CmbSizeType;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.Button button3;
        public System.Windows.Forms.RadioButton RbtKui;
        public System.Windows.Forms.Button button1;
        public System.Windows.Forms.RadioButton RbtFree;
        public System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.ComboBox CmbMaterial;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.GroupBox groupBox2;
        public System.Windows.Forms.NumericUpDown NmcOhbikiBoltCnt;
        public System.Windows.Forms.Label label6;
        public System.Windows.Forms.Label label4;
        public System.Windows.Forms.Panel panel2;
        public System.Windows.Forms.Label label5;
        public System.Windows.Forms.RadioButton RbtOhbikiBHTB;
        public System.Windows.Forms.RadioButton RbtOhbikiNormalB;
        public System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.RadioButton RbtOhbikiWeld;
        public System.Windows.Forms.RadioButton RbtOhbikiBolt;
        public System.Windows.Forms.Label label86;
        public System.Windows.Forms.Label label40;
        public System.Windows.Forms.ComboBox CmbLevel;
        public System.Windows.Forms.Label label34;
        public System.Windows.Forms.GroupBox groupBox3;
        public System.Windows.Forms.Label label9;
        public System.Windows.Forms.Label label10;
        public System.Windows.Forms.ComboBox CmbSize;
        public System.Windows.Forms.Panel panel3;
        public System.Windows.Forms.Label label11;
        public System.Windows.Forms.RadioButton RbtBothSide;
        public System.Windows.Forms.RadioButton RbtOneSide;
        public System.Windows.Forms.Label label12;
        public System.Windows.Forms.RadioButton RbtOhbikiSingle;
        public System.Windows.Forms.RadioButton RbtOhbikiDouble;
        public System.Windows.Forms.RadioButton RbtOhbikiTriple;
        public System.Windows.Forms.Label label7;
        public System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox4;
    }
}