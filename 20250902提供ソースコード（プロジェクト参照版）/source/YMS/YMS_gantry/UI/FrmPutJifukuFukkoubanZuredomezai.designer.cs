
namespace YMS_gantry.UI
{
    partial class FrmPutJifukuFukkoubanZuredomezai
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
            this.CmbSize = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.NmcOffset = new System.Windows.Forms.NumericUpDown();
            this.NmcZuredomeLeng = new System.Windows.Forms.NumericUpDown();
            this.RbtFree = new System.Windows.Forms.RadioButton();
            this.RbtAutoAll = new System.Windows.Forms.RadioButton();
            this.RbtAuto = new System.Windows.Forms.RadioButton();
            this.label86 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.CmbLevel = new System.Windows.Forms.ComboBox();
            this.RbtFace = new System.Windows.Forms.RadioButton();
            this.label34 = new System.Windows.Forms.Label();
            this.CmbSizeType = new System.Windows.Forms.ComboBox();
            this.CmbMaterial = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NmcOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NmcZuredomeLeng)).BeginInit();
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
            this.CmbKoudaiName.SelectedIndexChanged += new System.EventHandler(this.CmbKoudaiName_SelectedIndexChanged);
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(16, 15);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(29, 12);
            this.label24.TabIndex = 0;
            this.label24.Text = "構台";
            // 
            // CmbSize
            // 
            this.CmbSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbSize.FormattingEnabled = true;
            this.CmbSize.Location = new System.Drawing.Point(148, 40);
            this.CmbSize.Name = "CmbSize";
            this.CmbSize.Size = new System.Drawing.Size(177, 20);
            this.CmbSize.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "サイズ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "ズレ止メ材長さ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(207, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "mm";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(196, 237);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 8;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(278, 237);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 9;
            this.button3.Text = "キャンセル";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.NmcOffset);
            this.groupBox1.Controls.Add(this.NmcZuredomeLeng);
            this.groupBox1.Controls.Add(this.RbtFree);
            this.groupBox1.Controls.Add(this.RbtAutoAll);
            this.groupBox1.Controls.Add(this.RbtAuto);
            this.groupBox1.Controls.Add(this.label86);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label40);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.CmbLevel);
            this.groupBox1.Controls.Add(this.RbtFace);
            this.groupBox1.Controls.Add(this.label34);
            this.groupBox1.Location = new System.Drawing.Point(12, 92);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(341, 139);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "配置方法";
            // 
            // NmcOffset
            // 
            this.NmcOffset.Location = new System.Drawing.Point(170, 114);
            this.NmcOffset.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.NmcOffset.Name = "NmcOffset";
            this.NmcOffset.Size = new System.Drawing.Size(60, 19);
            this.NmcOffset.TabIndex = 10;
            this.NmcOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NmcOffset.Leave += new System.EventHandler(this.NmcZuredomeLeng_Leave);
            // 
            // NmcZuredomeLeng
            // 
            this.NmcZuredomeLeng.Location = new System.Drawing.Point(101, 41);
            this.NmcZuredomeLeng.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.NmcZuredomeLeng.Name = "NmcZuredomeLeng";
            this.NmcZuredomeLeng.Size = new System.Drawing.Size(100, 19);
            this.NmcZuredomeLeng.TabIndex = 3;
            this.NmcZuredomeLeng.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NmcZuredomeLeng.Leave += new System.EventHandler(this.NmcZuredomeLeng_Leave);
            // 
            // RbtFree
            // 
            this.RbtFree.AutoSize = true;
            this.RbtFree.Location = new System.Drawing.Point(95, 66);
            this.RbtFree.Name = "RbtFree";
            this.RbtFree.Size = new System.Drawing.Size(95, 16);
            this.RbtFree.TabIndex = 6;
            this.RbtFree.Text = "自由位置配置";
            this.RbtFree.UseVisualStyleBackColor = true;
            this.RbtFree.CheckedChanged += new System.EventHandler(this.RbtFree_CheckedChanged);
            // 
            // RbtAutoAll
            // 
            this.RbtAutoAll.AutoSize = true;
            this.RbtAutoAll.Location = new System.Drawing.Point(95, 20);
            this.RbtAutoAll.Name = "RbtAutoAll";
            this.RbtAutoAll.Size = new System.Drawing.Size(103, 16);
            this.RbtAutoAll.TabIndex = 1;
            this.RbtAutoAll.Text = "自動(全周配置)";
            this.RbtAutoAll.UseVisualStyleBackColor = true;
            // 
            // RbtAuto
            // 
            this.RbtAuto.AutoSize = true;
            this.RbtAuto.Checked = true;
            this.RbtAuto.Location = new System.Drawing.Point(6, 20);
            this.RbtAuto.Name = "RbtAuto";
            this.RbtAuto.Size = new System.Drawing.Size(47, 16);
            this.RbtAuto.TabIndex = 0;
            this.RbtAuto.TabStop = true;
            this.RbtAuto.Text = "自動";
            this.RbtAuto.UseVisualStyleBackColor = true;
            this.RbtAuto.CheckedChanged += new System.EventHandler(this.RbtAuto_CheckedChanged);
            // 
            // label86
            // 
            this.label86.AutoSize = true;
            this.label86.Location = new System.Drawing.Point(35, 91);
            this.label86.Name = "label86";
            this.label86.Size = new System.Drawing.Size(54, 12);
            this.label86.TabIndex = 7;
            this.label86.Text = "基準ﾚﾍﾞﾙ";
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(236, 116);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(23, 12);
            this.label40.TabIndex = 11;
            this.label40.Text = "mm";
            // 
            // CmbLevel
            // 
            this.CmbLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbLevel.FormattingEnabled = true;
            this.CmbLevel.Location = new System.Drawing.Point(101, 88);
            this.CmbLevel.Name = "CmbLevel";
            this.CmbLevel.Size = new System.Drawing.Size(129, 20);
            this.CmbLevel.TabIndex = 8;
            // 
            // RbtFace
            // 
            this.RbtFace.AutoSize = true;
            this.RbtFace.Location = new System.Drawing.Point(6, 67);
            this.RbtFace.Name = "RbtFace";
            this.RbtFace.Size = new System.Drawing.Size(83, 16);
            this.RbtFace.TabIndex = 5;
            this.RbtFace.Text = "指定面配置";
            this.RbtFace.UseVisualStyleBackColor = true;
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(35, 116);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(129, 12);
            this.label34.TabIndex = 9;
            this.label34.Text = "基準ﾚﾍﾞﾙからのｵﾌｾｯﾄ値";
            // 
            // CmbSizeType
            // 
            this.CmbSizeType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbSizeType.FormattingEnabled = true;
            this.CmbSizeType.Location = new System.Drawing.Point(73, 40);
            this.CmbSizeType.Name = "CmbSizeType";
            this.CmbSizeType.Size = new System.Drawing.Size(69, 20);
            this.CmbSizeType.TabIndex = 3;
            this.CmbSizeType.SelectedIndexChanged += new System.EventHandler(this.CmbSizeType_SelectedIndexChanged);
            // 
            // CmbMaterial
            // 
            this.CmbMaterial.FormattingEnabled = true;
            this.CmbMaterial.Location = new System.Drawing.Point(73, 66);
            this.CmbMaterial.Name = "CmbMaterial";
            this.CmbMaterial.Size = new System.Drawing.Size(103, 20);
            this.CmbMaterial.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 69);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "材質";
            // 
            // FrmPutJifukuFukkoubanZuredomezai
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(364, 268);
            this.Controls.Add(this.CmbMaterial);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.CmbSizeType);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.CmbSize);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CmbKoudaiName);
            this.Controls.Add(this.label24);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(380, 280);
            this.Name = "FrmPutJifukuFukkoubanZuredomezai";
            this.ShowIcon = false;
            this.Text = "[個別] 地覆・覆工板ズレ止め材配置";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FrmPutJifukuFukkoubanZuredomezai_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NmcOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NmcZuredomeLeng)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.ComboBox CmbKoudaiName;
        public System.Windows.Forms.Label label24;
        public System.Windows.Forms.ComboBox CmbSize;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.Button button1;
        public System.Windows.Forms.Button button3;
        public System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.RadioButton RbtAuto;
        public System.Windows.Forms.Label label86;
        public System.Windows.Forms.Label label40;
        public System.Windows.Forms.ComboBox CmbLevel;
        public System.Windows.Forms.RadioButton RbtFace;
        public System.Windows.Forms.Label label34;
        public System.Windows.Forms.RadioButton RbtAutoAll;
        public System.Windows.Forms.RadioButton RbtFree;
        public System.Windows.Forms.NumericUpDown NmcOffset;
        public System.Windows.Forms.NumericUpDown NmcZuredomeLeng;
        public System.Windows.Forms.ComboBox CmbSizeType;
        public System.Windows.Forms.ComboBox CmbMaterial;
        public System.Windows.Forms.Label label4;
    }
}