
namespace YMS_gantry.UI
{
    partial class FrmWaritsukeElement
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
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.NmcPLD = new System.Windows.Forms.NumericUpDown();
            this.NmcPLW = new System.Windows.Forms.NumericUpDown();
            this.NmcPLH = new System.Windows.Forms.NumericUpDown();
            this.NmcJackLng = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.ChkNeedCoverPL = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.label15 = new System.Windows.Forms.Label();
            this.NmcWaritsukeLeng = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.RbtMainHigh = new System.Windows.Forms.RadioButton();
            this.RbtMainNormal = new System.Windows.Forms.RadioButton();
            this.LstMainSize = new System.Windows.Forms.ListBox();
            this.RbtMainMaterial = new System.Windows.Forms.RadioButton();
            this.label9 = new System.Windows.Forms.Label();
            this.BtnCreateMain = new System.Windows.Forms.Button();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.BtnCreatePL = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.RbtJackYuatsu = new System.Windows.Forms.RadioButton();
            this.RbtJackKirin = new System.Windows.Forms.RadioButton();
            this.LstJackSize = new System.Windows.Forms.ListBox();
            this.BtnCreateJack = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.LstPieace = new System.Windows.Forms.ListBox();
            this.BtnCreatePieace = new System.Windows.Forms.Button();
            this.RbtHighHojo = new System.Windows.Forms.RadioButton();
            this.RbtHojo = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.CmbKoudaiName = new System.Windows.Forms.ComboBox();
            this.CmbUnit = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.BtnEnd = new System.Windows.Forms.Button();
            this.btnUndo = new System.Windows.Forms.Button();
            this.CmbCoverPL = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.LblOriginalL = new System.Windows.Forms.Label();
            this.LblRemainL = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.NmcPLD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NmcPLW)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NmcPLH)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NmcJackLng)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NmcWaritsukeLeng)).BeginInit();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(9, 93);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(13, 12);
            this.label19.TabIndex = 59;
            this.label19.Text = "D";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(9, 68);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(14, 12);
            this.label18.TabIndex = 58;
            this.label18.Text = "W";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(9, 43);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(13, 12);
            this.label17.TabIndex = 57;
            this.label17.Text = "H";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(10, 11);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(34, 12);
            this.label14.TabIndex = 56;
            this.label14.Text = "サイズ";
            // 
            // NmcPLD
            // 
            this.NmcPLD.Location = new System.Drawing.Point(29, 91);
            this.NmcPLD.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.NmcPLD.Name = "NmcPLD";
            this.NmcPLD.Size = new System.Drawing.Size(72, 19);
            this.NmcPLD.TabIndex = 52;
            this.NmcPLD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NmcPLD.Leave += new System.EventHandler(this.NmcPLD_Leave);
            // 
            // NmcPLW
            // 
            this.NmcPLW.Location = new System.Drawing.Point(29, 66);
            this.NmcPLW.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.NmcPLW.Name = "NmcPLW";
            this.NmcPLW.Size = new System.Drawing.Size(72, 19);
            this.NmcPLW.TabIndex = 51;
            this.NmcPLW.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NmcPLW.Leave += new System.EventHandler(this.NmcPLD_Leave);
            // 
            // NmcPLH
            // 
            this.NmcPLH.Location = new System.Drawing.Point(29, 41);
            this.NmcPLH.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.NmcPLH.Name = "NmcPLH";
            this.NmcPLH.Size = new System.Drawing.Size(72, 19);
            this.NmcPLH.TabIndex = 50;
            this.NmcPLH.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NmcPLH.Leave += new System.EventHandler(this.NmcPLD_Leave);
            // 
            // NmcJackLng
            // 
            this.NmcJackLng.Location = new System.Drawing.Point(42, 161);
            this.NmcJackLng.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.NmcJackLng.Name = "NmcJackLng";
            this.NmcJackLng.Size = new System.Drawing.Size(46, 19);
            this.NmcJackLng.TabIndex = 60;
            this.NmcJackLng.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(11, 163);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(25, 12);
            this.label7.TabIndex = 59;
            this.label7.Text = "長さ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 11);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(34, 12);
            this.label6.TabIndex = 58;
            this.label6.Text = "サイズ";
            // 
            // ChkNeedCoverPL
            // 
            this.ChkNeedCoverPL.AutoSize = true;
            this.ChkNeedCoverPL.Location = new System.Drawing.Point(16, 266);
            this.ChkNeedCoverPL.Name = "ChkNeedCoverPL";
            this.ChkNeedCoverPL.Size = new System.Drawing.Size(186, 16);
            this.ChkNeedCoverPL.TabIndex = 3;
            this.ChkNeedCoverPL.Text = "割付部にカバープレートを配置する";
            this.ChkNeedCoverPL.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 12);
            this.label4.TabIndex = 56;
            this.label4.Text = "サイズ";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(12, 46);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(234, 214);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.label15);
            this.tabPage4.Controls.Add(this.NmcWaritsukeLeng);
            this.tabPage4.Controls.Add(this.label13);
            this.tabPage4.Controls.Add(this.RbtMainHigh);
            this.tabPage4.Controls.Add(this.RbtMainNormal);
            this.tabPage4.Controls.Add(this.LstMainSize);
            this.tabPage4.Controls.Add(this.RbtMainMaterial);
            this.tabPage4.Controls.Add(this.label9);
            this.tabPage4.Controls.Add(this.BtnCreateMain);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(226, 188);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "メイン部材";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(101, 165);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(23, 12);
            this.label15.TabIndex = 7;
            this.label15.Text = "mm";
            // 
            // NmcWaritsukeLeng
            // 
            this.NmcWaritsukeLeng.Location = new System.Drawing.Point(41, 161);
            this.NmcWaritsukeLeng.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.NmcWaritsukeLeng.Name = "NmcWaritsukeLeng";
            this.NmcWaritsukeLeng.Size = new System.Drawing.Size(60, 19);
            this.NmcWaritsukeLeng.TabIndex = 6;
            this.NmcWaritsukeLeng.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NmcWaritsukeLeng.Leave += new System.EventHandler(this.NmcPLD_Leave);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(10, 163);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(25, 12);
            this.label13.TabIndex = 5;
            this.label13.Text = "長さ";
            // 
            // RbtMainHigh
            // 
            this.RbtMainHigh.AutoSize = true;
            this.RbtMainHigh.Location = new System.Drawing.Point(131, 133);
            this.RbtMainHigh.Margin = new System.Windows.Forms.Padding(2);
            this.RbtMainHigh.Name = "RbtMainHigh";
            this.RbtMainHigh.Size = new System.Drawing.Size(83, 16);
            this.RbtMainHigh.TabIndex = 4;
            this.RbtMainHigh.Text = "高強度主材";
            this.RbtMainHigh.UseVisualStyleBackColor = true;
            this.RbtMainHigh.CheckedChanged += new System.EventHandler(this.RbtMainMaterial_CheckedChanged);
            // 
            // RbtMainNormal
            // 
            this.RbtMainNormal.AutoSize = true;
            this.RbtMainNormal.Location = new System.Drawing.Point(59, 133);
            this.RbtMainNormal.Margin = new System.Windows.Forms.Padding(2);
            this.RbtMainNormal.Name = "RbtMainNormal";
            this.RbtMainNormal.Size = new System.Drawing.Size(71, 16);
            this.RbtMainNormal.TabIndex = 3;
            this.RbtMainNormal.Text = "通常主材";
            this.RbtMainNormal.UseVisualStyleBackColor = true;
            this.RbtMainNormal.CheckedChanged += new System.EventHandler(this.RbtMainMaterial_CheckedChanged);
            // 
            // LstMainSize
            // 
            this.LstMainSize.Font = new System.Drawing.Font("MS UI Gothic", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.LstMainSize.FormattingEnabled = true;
            this.LstMainSize.HorizontalScrollbar = true;
            this.LstMainSize.Location = new System.Drawing.Point(12, 28);
            this.LstMainSize.Name = "LstMainSize";
            this.LstMainSize.Size = new System.Drawing.Size(205, 95);
            this.LstMainSize.TabIndex = 1;
            // 
            // RbtMainMaterial
            // 
            this.RbtMainMaterial.AutoSize = true;
            this.RbtMainMaterial.Checked = true;
            this.RbtMainMaterial.Location = new System.Drawing.Point(12, 133);
            this.RbtMainMaterial.Margin = new System.Windows.Forms.Padding(2);
            this.RbtMainMaterial.Name = "RbtMainMaterial";
            this.RbtMainMaterial.Size = new System.Drawing.Size(47, 16);
            this.RbtMainMaterial.TabIndex = 2;
            this.RbtMainMaterial.TabStop = true;
            this.RbtMainMaterial.Text = "素材";
            this.RbtMainMaterial.UseVisualStyleBackColor = true;
            this.RbtMainMaterial.CheckedChanged += new System.EventHandler(this.RbtMainMaterial_CheckedChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(10, 11);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(34, 12);
            this.label9.TabIndex = 0;
            this.label9.Text = "サイズ";
            // 
            // BtnCreateMain
            // 
            this.BtnCreateMain.Location = new System.Drawing.Point(144, 158);
            this.BtnCreateMain.Name = "BtnCreateMain";
            this.BtnCreateMain.Size = new System.Drawing.Size(75, 23);
            this.BtnCreateMain.TabIndex = 8;
            this.BtnCreateMain.Text = "配置";
            this.BtnCreateMain.UseVisualStyleBackColor = true;
            this.BtnCreateMain.Click += new System.EventHandler(this.BtnCreateMain_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label12);
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.BtnCreatePL);
            this.tabPage1.Controls.Add(this.label19);
            this.tabPage1.Controls.Add(this.label18);
            this.tabPage1.Controls.Add(this.label17);
            this.tabPage1.Controls.Add(this.NmcPLH);
            this.tabPage1.Controls.Add(this.label14);
            this.tabPage1.Controls.Add(this.NmcPLW);
            this.tabPage1.Controls.Add(this.NmcPLD);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(226, 188);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "プレート";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(107, 93);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(23, 12);
            this.label12.TabIndex = 64;
            this.label12.Text = "mm";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(107, 68);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(23, 12);
            this.label11.TabIndex = 63;
            this.label11.Text = "mm";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(107, 43);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(23, 12);
            this.label5.TabIndex = 62;
            this.label5.Text = "mm";
            // 
            // BtnCreatePL
            // 
            this.BtnCreatePL.Location = new System.Drawing.Point(144, 158);
            this.BtnCreatePL.Name = "BtnCreatePL";
            this.BtnCreatePL.Size = new System.Drawing.Size(75, 23);
            this.BtnCreatePL.TabIndex = 60;
            this.BtnCreatePL.Text = "配置";
            this.BtnCreatePL.UseVisualStyleBackColor = true;
            this.BtnCreatePL.Click += new System.EventHandler(this.BtnCreatePL_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.RbtJackYuatsu);
            this.tabPage2.Controls.Add(this.RbtJackKirin);
            this.tabPage2.Controls.Add(this.LstJackSize);
            this.tabPage2.Controls.Add(this.BtnCreateJack);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.NmcJackLng);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(226, 188);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "ジャッキ";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // RbtJackYuatsu
            // 
            this.RbtJackYuatsu.AutoSize = true;
            this.RbtJackYuatsu.Location = new System.Drawing.Point(119, 136);
            this.RbtJackYuatsu.Name = "RbtJackYuatsu";
            this.RbtJackYuatsu.Size = new System.Drawing.Size(82, 16);
            this.RbtJackYuatsu.TabIndex = 65;
            this.RbtJackYuatsu.Text = "油圧ジャッキ";
            this.RbtJackYuatsu.UseVisualStyleBackColor = true;
            // 
            // RbtJackKirin
            // 
            this.RbtJackKirin.AutoSize = true;
            this.RbtJackKirin.Checked = true;
            this.RbtJackKirin.Location = new System.Drawing.Point(13, 136);
            this.RbtJackKirin.Name = "RbtJackKirin";
            this.RbtJackKirin.Size = new System.Drawing.Size(84, 16);
            this.RbtJackKirin.TabIndex = 64;
            this.RbtJackKirin.TabStop = true;
            this.RbtJackKirin.Text = "キリンジャッキ";
            this.RbtJackKirin.UseVisualStyleBackColor = true;
            this.RbtJackKirin.CheckedChanged += new System.EventHandler(this.RbtJackKirin_CheckedChanged);
            // 
            // LstJackSize
            // 
            this.LstJackSize.Font = new System.Drawing.Font("MS UI Gothic", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.LstJackSize.FormattingEnabled = true;
            this.LstJackSize.Location = new System.Drawing.Point(12, 28);
            this.LstJackSize.Name = "LstJackSize";
            this.LstJackSize.Size = new System.Drawing.Size(205, 95);
            this.LstJackSize.TabIndex = 63;
            this.LstJackSize.SelectedIndexChanged += new System.EventHandler(this.LstJackSize_SelectedIndexChanged);
            // 
            // BtnCreateJack
            // 
            this.BtnCreateJack.Location = new System.Drawing.Point(144, 158);
            this.BtnCreateJack.Name = "BtnCreateJack";
            this.BtnCreateJack.Size = new System.Drawing.Size(75, 23);
            this.BtnCreateJack.TabIndex = 62;
            this.BtnCreateJack.Text = "配置";
            this.BtnCreateJack.UseVisualStyleBackColor = true;
            this.BtnCreateJack.Click += new System.EventHandler(this.BtnCreateJack_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(94, 168);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(23, 12);
            this.label8.TabIndex = 61;
            this.label8.Text = "mm";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.LstPieace);
            this.tabPage3.Controls.Add(this.BtnCreatePieace);
            this.tabPage3.Controls.Add(this.RbtHighHojo);
            this.tabPage3.Controls.Add(this.RbtHojo);
            this.tabPage3.Controls.Add(this.label4);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(226, 188);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "補助ピース";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // LstPieace
            // 
            this.LstPieace.Font = new System.Drawing.Font("MS UI Gothic", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.LstPieace.FormattingEnabled = true;
            this.LstPieace.Location = new System.Drawing.Point(12, 28);
            this.LstPieace.Name = "LstPieace";
            this.LstPieace.Size = new System.Drawing.Size(205, 95);
            this.LstPieace.TabIndex = 64;
            // 
            // BtnCreatePieace
            // 
            this.BtnCreatePieace.Location = new System.Drawing.Point(144, 158);
            this.BtnCreatePieace.Name = "BtnCreatePieace";
            this.BtnCreatePieace.Size = new System.Drawing.Size(75, 23);
            this.BtnCreatePieace.TabIndex = 63;
            this.BtnCreatePieace.Text = "配置";
            this.BtnCreatePieace.UseVisualStyleBackColor = true;
            this.BtnCreatePieace.Click += new System.EventHandler(this.BtnCreatePieace_Click);
            // 
            // RbtHighHojo
            // 
            this.RbtHighHojo.AutoSize = true;
            this.RbtHighHojo.Location = new System.Drawing.Point(106, 137);
            this.RbtHighHojo.Margin = new System.Windows.Forms.Padding(2);
            this.RbtHighHojo.Name = "RbtHighHojo";
            this.RbtHighHojo.Size = new System.Drawing.Size(111, 16);
            this.RbtHighHojo.TabIndex = 59;
            this.RbtHighHojo.Text = "高強度補助ピース";
            this.RbtHighHojo.UseVisualStyleBackColor = true;
            this.RbtHighHojo.CheckedChanged += new System.EventHandler(this.RbtHojo_CheckedChanged);
            // 
            // RbtHojo
            // 
            this.RbtHojo.AutoSize = true;
            this.RbtHojo.Checked = true;
            this.RbtHojo.Location = new System.Drawing.Point(15, 137);
            this.RbtHojo.Margin = new System.Windows.Forms.Padding(2);
            this.RbtHojo.Name = "RbtHojo";
            this.RbtHojo.Size = new System.Drawing.Size(75, 16);
            this.RbtHojo.TabIndex = 58;
            this.RbtHojo.TabStop = true;
            this.RbtHojo.Text = "補助ピース";
            this.RbtHojo.UseVisualStyleBackColor = true;
            this.RbtHojo.CheckedChanged += new System.EventHandler(this.RbtHojo_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "構台";
            // 
            // CmbKoudaiName
            // 
            this.CmbKoudaiName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbKoudaiName.FormattingEnabled = true;
            this.CmbKoudaiName.Location = new System.Drawing.Point(45, 13);
            this.CmbKoudaiName.Name = "CmbKoudaiName";
            this.CmbKoudaiName.Size = new System.Drawing.Size(201, 20);
            this.CmbKoudaiName.TabIndex = 1;
            // 
            // CmbUnit
            // 
            this.CmbUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbUnit.FormattingEnabled = true;
            this.CmbUnit.Items.AddRange(new object[] {
            "100",
            "10",
            "1"});
            this.CmbUnit.Location = new System.Drawing.Point(135, 400);
            this.CmbUnit.Name = "CmbUnit";
            this.CmbUnit.Size = new System.Drawing.Size(71, 20);
            this.CmbUnit.TabIndex = 21;
            this.CmbUnit.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 404);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 12);
            this.label2.TabIndex = 22;
            this.label2.Text = "割付メモリ最小単位";
            this.label2.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(212, 404);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 12);
            this.label3.TabIndex = 23;
            this.label3.Text = "mm";
            this.label3.Visible = false;
            // 
            // BtnEnd
            // 
            this.BtnEnd.Location = new System.Drawing.Point(167, 347);
            this.BtnEnd.Name = "BtnEnd";
            this.BtnEnd.Size = new System.Drawing.Size(75, 23);
            this.BtnEnd.TabIndex = 13;
            this.BtnEnd.Text = "終了";
            this.BtnEnd.UseVisualStyleBackColor = true;
            this.BtnEnd.Click += new System.EventHandler(this.button4_Click);
            // 
            // btnUndo
            // 
            this.btnUndo.Location = new System.Drawing.Point(85, 347);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(75, 23);
            this.btnUndo.TabIndex = 12;
            this.btnUndo.Text = "1つ戻る";
            this.btnUndo.UseVisualStyleBackColor = true;
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // CmbCoverPL
            // 
            this.CmbCoverPL.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbCoverPL.FormattingEnabled = true;
            this.CmbCoverPL.Location = new System.Drawing.Point(119, 283);
            this.CmbCoverPL.Name = "CmbCoverPL";
            this.CmbCoverPL.Size = new System.Drawing.Size(121, 20);
            this.CmbCoverPL.TabIndex = 5;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(10, 286);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(94, 12);
            this.label10.TabIndex = 4;
            this.label10.Text = "カバープレート種類";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(89, 309);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(61, 12);
            this.label16.TabIndex = 6;
            this.label16.Text = "基部材長さ";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(105, 327);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(45, 12);
            this.label20.TabIndex = 7;
            this.label20.Text = "残り長さ";
            // 
            // LblOriginalL
            // 
            this.LblOriginalL.AutoSize = true;
            this.LblOriginalL.Location = new System.Drawing.Point(182, 311);
            this.LblOriginalL.Name = "LblOriginalL";
            this.LblOriginalL.Size = new System.Drawing.Size(29, 12);
            this.LblOriginalL.TabIndex = 8;
            this.LblOriginalL.Text = "1000";
            // 
            // LblRemainL
            // 
            this.LblRemainL.AutoSize = true;
            this.LblRemainL.Location = new System.Drawing.Point(182, 329);
            this.LblRemainL.Name = "LblRemainL";
            this.LblRemainL.Size = new System.Drawing.Size(29, 12);
            this.LblRemainL.TabIndex = 9;
            this.LblRemainL.Text = "1000";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(217, 314);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(23, 12);
            this.label23.TabIndex = 10;
            this.label23.Text = "mm";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(217, 331);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(23, 12);
            this.label24.TabIndex = 11;
            this.label24.Text = "mm";
            // 
            // FrmWaritsukeElement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(257, 376);
            this.ControlBox = false;
            this.Controls.Add(this.label24);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.LblRemainL);
            this.Controls.Add(this.LblOriginalL);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.CmbCoverPL);
            this.Controls.Add(this.btnUndo);
            this.Controls.Add(this.BtnEnd);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.CmbUnit);
            this.Controls.Add(this.CmbKoudaiName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.ChkNeedCoverPL);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmWaritsukeElement";
            this.ShowIcon = false;
            this.Text = "[割付] 部材割付";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmWaritsuke_FormClosed);
            this.Load += new System.EventHandler(this.FrmWaritsuke_Load);
            ((System.ComponentModel.ISupportInitialize)(this.NmcPLD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NmcPLW)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NmcPLH)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NmcJackLng)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NmcWaritsukeLeng)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label label19;
        public System.Windows.Forms.Label label18;
        public System.Windows.Forms.Label label17;
        public System.Windows.Forms.Label label14;
        public System.Windows.Forms.NumericUpDown NmcPLD;
        public System.Windows.Forms.NumericUpDown NmcPLW;
        public System.Windows.Forms.NumericUpDown NmcPLH;
        public System.Windows.Forms.NumericUpDown NmcJackLng;
        public System.Windows.Forms.Label label7;
        public System.Windows.Forms.Label label6;
        public System.Windows.Forms.CheckBox ChkNeedCoverPL;
        public System.Windows.Forms.Label label4;
        public System.Windows.Forms.TabControl tabControl1;
        public System.Windows.Forms.TabPage tabPage1;
        public System.Windows.Forms.TabPage tabPage2;
        public System.Windows.Forms.TabPage tabPage3;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox CmbKoudaiName;
        public System.Windows.Forms.ComboBox CmbUnit;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.TabPage tabPage4;
        public System.Windows.Forms.RadioButton RbtMainHigh;
        public System.Windows.Forms.RadioButton RbtMainNormal;
        public System.Windows.Forms.RadioButton RbtMainMaterial;
        public System.Windows.Forms.RadioButton RbtHojo;
        public System.Windows.Forms.Button BtnCreatePL;
        public System.Windows.Forms.Button BtnCreateJack;
        public System.Windows.Forms.Button BtnCreatePieace;
        public System.Windows.Forms.Button BtnEnd;
        public System.Windows.Forms.Button btnUndo;
        public System.Windows.Forms.ListBox LstJackSize;
        public System.Windows.Forms.ListBox LstPieace;
        public System.Windows.Forms.RadioButton RbtHighHojo;
        public System.Windows.Forms.Label label9;
        public System.Windows.Forms.Button BtnCreateMain;
        public System.Windows.Forms.ListBox LstMainSize;
        public System.Windows.Forms.Label label12;
        public System.Windows.Forms.Label label11;
        public System.Windows.Forms.Label label5;
        public System.Windows.Forms.Label label8;
        public System.Windows.Forms.ComboBox CmbCoverPL;
        public System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.RadioButton RbtJackYuatsu;
        private System.Windows.Forms.RadioButton RbtJackKirin;
        public System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label LblOriginalL;
        private System.Windows.Forms.Label LblRemainL;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label24;
        public System.Windows.Forms.NumericUpDown NmcWaritsukeLeng;
    }
}