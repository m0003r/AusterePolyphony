namespace Generator
{
    partial class MainForm
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
            this.outputArea = new System.Windows.Forms.TextBox();
            this.modiList = new System.Windows.Forms.ListBox();
            this.startNotes = new System.Windows.Forms.ListBox();
            this.perfectTime = new System.Windows.Forms.RadioButton();
            this.imperfectTime = new System.Windows.Forms.RadioButton();
            this.barsCount = new System.Windows.Forms.NumericUpDown();
            this.makeButton = new System.Windows.Forms.Button();
            this.clefList = new System.Windows.Forms.ListBox();
            this.gvOut = new System.Windows.Forms.TextBox();
            this.barCountLabel = new System.Windows.Forms.Label();
            this.saveLily = new System.Windows.Forms.Button();
            this.engrave = new System.Windows.Forms.Button();
            this.graphViz = new System.Windows.Forms.Button();
            this.saveGV = new System.Windows.Forms.Button();
            this.randSeedDD = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.barsCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.randSeedDD)).BeginInit();
            this.SuspendLayout();
            // 
            // outputArea
            // 
            this.outputArea.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.outputArea.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.outputArea.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.outputArea.Location = new System.Drawing.Point(12, 125);
            this.outputArea.Multiline = true;
            this.outputArea.Name = "outputArea";
            this.outputArea.ReadOnly = true;
            this.outputArea.Size = new System.Drawing.Size(290, 397);
            this.outputArea.TabIndex = 1;
            // 
            // modiList
            // 
            this.modiList.FormattingEnabled = true;
            this.modiList.Items.AddRange(new object[] {
            "Ионийский",
            "Дорийский",
            "Фригийский",
            "Лидийский",
            "Миксолидийский",
            "Эолийский"});
            this.modiList.Location = new System.Drawing.Point(12, 14);
            this.modiList.Name = "modiList";
            this.modiList.Size = new System.Drawing.Size(99, 95);
            this.modiList.TabIndex = 2;
            // 
            // startNotes
            // 
            this.startNotes.FormattingEnabled = true;
            this.startNotes.Items.AddRange(new object[] {
            "до",
            "до#",
            "ре",
            "ми b",
            "ми",
            "фа",
            "фа #",
            "соль",
            "соль #",
            "ля",
            "си b",
            "си"});
            this.startNotes.Location = new System.Drawing.Point(136, 14);
            this.startNotes.Name = "startNotes";
            this.startNotes.Size = new System.Drawing.Size(63, 95);
            this.startNotes.TabIndex = 3;
            // 
            // perfectTime
            // 
            this.perfectTime.AutoSize = true;
            this.perfectTime.Location = new System.Drawing.Point(330, 12);
            this.perfectTime.Name = "perfectTime";
            this.perfectTime.Size = new System.Drawing.Size(42, 17);
            this.perfectTime.TabIndex = 4;
            this.perfectTime.Text = "3/4";
            this.perfectTime.UseVisualStyleBackColor = true;
            // 
            // imperfectTime
            // 
            this.imperfectTime.AutoSize = true;
            this.imperfectTime.Checked = true;
            this.imperfectTime.Location = new System.Drawing.Point(330, 35);
            this.imperfectTime.Name = "imperfectTime";
            this.imperfectTime.Size = new System.Drawing.Size(42, 17);
            this.imperfectTime.TabIndex = 5;
            this.imperfectTime.TabStop = true;
            this.imperfectTime.Text = "4/4";
            this.imperfectTime.UseVisualStyleBackColor = true;
            // 
            // barsCount
            // 
            this.barsCount.Location = new System.Drawing.Point(330, 89);
            this.barsCount.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.barsCount.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.barsCount.Name = "barsCount";
            this.barsCount.Size = new System.Drawing.Size(107, 20);
            this.barsCount.TabIndex = 6;
            this.barsCount.Value = new decimal(new int[] {
            11,
            0,
            0,
            0});
            // 
            // makeButton
            // 
            this.makeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.makeButton.Location = new System.Drawing.Point(470, 37);
            this.makeButton.Name = "makeButton";
            this.makeButton.Size = new System.Drawing.Size(128, 71);
            this.makeButton.TabIndex = 7;
            this.makeButton.Text = "СДЕЛАТЬ";
            this.makeButton.UseVisualStyleBackColor = true;
            this.makeButton.Click += new System.EventHandler(this.makeButton_Click);
            // 
            // clefList
            // 
            this.clefList.FormattingEnabled = true;
            this.clefList.Items.AddRange(new object[] {
            "Скрипичный",
            "Сопрановый",
            "Меццо",
            "Альтовый",
            "Теноровый",
            "Баритоновый",
            "Басовый"});
            this.clefList.Location = new System.Drawing.Point(223, 12);
            this.clefList.Name = "clefList";
            this.clefList.Size = new System.Drawing.Size(79, 95);
            this.clefList.TabIndex = 8;
            // 
            // gvOut
            // 
            this.gvOut.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gvOut.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.gvOut.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.gvOut.Location = new System.Drawing.Point(308, 125);
            this.gvOut.Multiline = true;
            this.gvOut.Name = "gvOut";
            this.gvOut.ReadOnly = true;
            this.gvOut.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.gvOut.Size = new System.Drawing.Size(290, 397);
            this.gvOut.TabIndex = 9;
            // 
            // barCountLabel
            // 
            this.barCountLabel.AutoSize = true;
            this.barCountLabel.Location = new System.Drawing.Point(327, 73);
            this.barCountLabel.Name = "barCountLabel";
            this.barCountLabel.Size = new System.Drawing.Size(103, 13);
            this.barCountLabel.TabIndex = 10;
            this.barCountLabel.Text = "Количество тактов";
            // 
            // saveLily
            // 
            this.saveLily.Enabled = false;
            this.saveLily.Location = new System.Drawing.Point(12, 528);
            this.saveLily.Name = "saveLily";
            this.saveLily.Size = new System.Drawing.Size(105, 25);
            this.saveLily.TabIndex = 11;
            this.saveLily.Text = "Сохранить";
            this.saveLily.UseVisualStyleBackColor = true;
            this.saveLily.Click += new System.EventHandler(this.saveLily_Click);
            // 
            // engrave
            // 
            this.engrave.Enabled = false;
            this.engrave.Location = new System.Drawing.Point(195, 530);
            this.engrave.Name = "engrave";
            this.engrave.Size = new System.Drawing.Size(106, 25);
            this.engrave.TabIndex = 12;
            this.engrave.Text = "Рисовать";
            this.engrave.UseVisualStyleBackColor = true;
            this.engrave.Click += new System.EventHandler(this.engrave_Click);
            // 
            // graphViz
            // 
            this.graphViz.Enabled = false;
            this.graphViz.Location = new System.Drawing.Point(491, 532);
            this.graphViz.Name = "graphViz";
            this.graphViz.Size = new System.Drawing.Size(106, 23);
            this.graphViz.TabIndex = 14;
            this.graphViz.Text = "Рисовать";
            this.graphViz.UseVisualStyleBackColor = true;
            this.graphViz.Click += new System.EventHandler(this.graphViz_Click);
            // 
            // saveGV
            // 
            this.saveGV.Enabled = false;
            this.saveGV.Location = new System.Drawing.Point(308, 530);
            this.saveGV.Name = "saveGV";
            this.saveGV.Size = new System.Drawing.Size(105, 25);
            this.saveGV.TabIndex = 13;
            this.saveGV.Text = "Сохранить";
            this.saveGV.UseVisualStyleBackColor = true;
            this.saveGV.Click += new System.EventHandler(this.saveGV_Click);
            // 
            // randSeedDD
            // 
            this.randSeedDD.Location = new System.Drawing.Point(470, 13);
            this.randSeedDD.Maximum = new decimal(new int[] {
            -559939585,
            902409669,
            54,
            0});
            this.randSeedDD.Minimum = new decimal(new int[] {
            1874919423,
            2328306,
            0,
            -2147483648});
            this.randSeedDD.Name = "randSeedDD";
            this.randSeedDD.Size = new System.Drawing.Size(127, 20);
            this.randSeedDD.TabIndex = 15;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(605, 560);
            this.Controls.Add(this.randSeedDD);
            this.Controls.Add(this.graphViz);
            this.Controls.Add(this.saveGV);
            this.Controls.Add(this.engrave);
            this.Controls.Add(this.saveLily);
            this.Controls.Add(this.barCountLabel);
            this.Controls.Add(this.gvOut);
            this.Controls.Add(this.clefList);
            this.Controls.Add(this.makeButton);
            this.Controls.Add(this.barsCount);
            this.Controls.Add(this.imperfectTime);
            this.Controls.Add(this.perfectTime);
            this.Controls.Add(this.startNotes);
            this.Controls.Add(this.modiList);
            this.Controls.Add(this.outputArea);
            this.Name = "MainForm";
            this.Text = "Генератор мелодий";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barsCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.randSeedDD)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox outputArea;
        private System.Windows.Forms.ListBox modiList;
        private System.Windows.Forms.ListBox startNotes;
        private System.Windows.Forms.RadioButton perfectTime;
        private System.Windows.Forms.RadioButton imperfectTime;
        private System.Windows.Forms.NumericUpDown barsCount;
        private System.Windows.Forms.Button makeButton;
        private System.Windows.Forms.ListBox clefList;
        private System.Windows.Forms.TextBox gvOut;
        private System.Windows.Forms.Label barCountLabel;
        private System.Windows.Forms.Button saveLily;
        private System.Windows.Forms.Button engrave;
        private System.Windows.Forms.Button graphViz;
        private System.Windows.Forms.Button saveGV;
        private System.Windows.Forms.NumericUpDown randSeedDD;

    }
}

