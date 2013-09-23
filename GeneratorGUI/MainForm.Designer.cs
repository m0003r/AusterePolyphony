namespace GeneratorGUI
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
            this.barCountLabel = new System.Windows.Forms.Label();
            this.engraveButton = new System.Windows.Forms.Button();
            this.drawGraphButton = new System.Windows.Forms.Button();
            this.randSeedDD = new System.Windows.Forms.NumericUpDown();
            this.maxSteps = new System.Windows.Forms.NumericUpDown();
            this.seedLabel = new System.Windows.Forms.Label();
            this.stepLimitLabel = new System.Windows.Forms.Label();
            this.gvOut = new System.Windows.Forms.TextBox();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.barsCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.randSeedDD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxSteps)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // outputArea
            // 
            this.outputArea.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.outputArea.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.outputArea.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.outputArea.Location = new System.Drawing.Point(3, 3);
            this.outputArea.Multiline = true;
            this.outputArea.Name = "outputArea";
            this.outputArea.ReadOnly = true;
            this.outputArea.Size = new System.Drawing.Size(314, 447);
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
            this.modiList.Location = new System.Drawing.Point(9, 19);
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
            this.startNotes.Location = new System.Drawing.Point(123, 19);
            this.startNotes.Name = "startNotes";
            this.startNotes.Size = new System.Drawing.Size(63, 95);
            this.startNotes.TabIndex = 3;
            // 
            // perfectTime
            // 
            this.perfectTime.AutoSize = true;
            this.perfectTime.Location = new System.Drawing.Point(301, 19);
            this.perfectTime.Name = "perfectTime";
            this.perfectTime.Size = new System.Drawing.Size(42, 17);
            this.perfectTime.TabIndex = 4;
            this.perfectTime.Text = "3/2";
            this.perfectTime.UseVisualStyleBackColor = true;
            // 
            // imperfectTime
            // 
            this.imperfectTime.AutoSize = true;
            this.imperfectTime.Checked = true;
            this.imperfectTime.Location = new System.Drawing.Point(301, 42);
            this.imperfectTime.Name = "imperfectTime";
            this.imperfectTime.Size = new System.Drawing.Size(42, 17);
            this.imperfectTime.TabIndex = 5;
            this.imperfectTime.TabStop = true;
            this.imperfectTime.Text = "4/2";
            this.imperfectTime.UseVisualStyleBackColor = true;
            // 
            // barsCount
            // 
            this.barsCount.Location = new System.Drawing.Point(301, 94);
            this.barsCount.Minimum = new decimal(new int[] {
            1,
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
            this.makeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.makeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.makeButton.Location = new System.Drawing.Point(88, 66);
            this.makeButton.Name = "makeButton";
            this.makeButton.Size = new System.Drawing.Size(128, 48);
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
            this.clefList.Location = new System.Drawing.Point(202, 19);
            this.clefList.Name = "clefList";
            this.clefList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.clefList.Size = new System.Drawing.Size(79, 95);
            this.clefList.TabIndex = 8;
            // 
            // barCountLabel
            // 
            this.barCountLabel.AutoSize = true;
            this.barCountLabel.Location = new System.Drawing.Point(298, 78);
            this.barCountLabel.Name = "barCountLabel";
            this.barCountLabel.Size = new System.Drawing.Size(103, 13);
            this.barCountLabel.TabIndex = 10;
            this.barCountLabel.Text = "Количество тактов";
            // 
            // engraveButton
            // 
            this.engraveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.engraveButton.Enabled = false;
            this.engraveButton.Location = new System.Drawing.Point(212, 454);
            this.engraveButton.Name = "engraveButton";
            this.engraveButton.Size = new System.Drawing.Size(106, 25);
            this.engraveButton.TabIndex = 12;
            this.engraveButton.Text = "Рисовать";
            this.engraveButton.UseVisualStyleBackColor = true;
            this.engraveButton.Click += new System.EventHandler(this.engraveButton_Click);
            // 
            // drawGraphButton
            // 
            this.drawGraphButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.drawGraphButton.Enabled = false;
            this.drawGraphButton.Location = new System.Drawing.Point(210, 455);
            this.drawGraphButton.Name = "drawGraphButton";
            this.drawGraphButton.Size = new System.Drawing.Size(106, 23);
            this.drawGraphButton.TabIndex = 14;
            this.drawGraphButton.Text = "Рисовать";
            this.drawGraphButton.UseVisualStyleBackColor = true;
            this.drawGraphButton.Click += new System.EventHandler(this.drawGraphButton_Click);
            // 
            // randSeedDD
            // 
            this.randSeedDD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.randSeedDD.Location = new System.Drawing.Point(87, 20);
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
            // maxSteps
            // 
            this.maxSteps.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.maxSteps.Location = new System.Drawing.Point(87, 42);
            this.maxSteps.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.maxSteps.Name = "maxSteps";
            this.maxSteps.Size = new System.Drawing.Size(127, 20);
            this.maxSteps.TabIndex = 16;
            this.maxSteps.Value = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            // 
            // seedLabel
            // 
            this.seedLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.seedLabel.AutoSize = true;
            this.seedLabel.Location = new System.Drawing.Point(46, 21);
            this.seedLabel.Name = "seedLabel";
            this.seedLabel.Size = new System.Drawing.Size(38, 13);
            this.seedLabel.TabIndex = 17;
            this.seedLabel.Text = "Зерно";
            // 
            // stepLimitLabel
            // 
            this.stepLimitLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.stepLimitLabel.AutoSize = true;
            this.stepLimitLabel.Location = new System.Drawing.Point(10, 44);
            this.stepLimitLabel.Name = "stepLimitLabel";
            this.stepLimitLabel.Size = new System.Drawing.Size(74, 13);
            this.stepLimitLabel.TabIndex = 17;
            this.stepLimitLabel.Text = "Лимит шагов";
            // 
            // gvOut
            // 
            this.gvOut.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gvOut.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.gvOut.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.gvOut.Location = new System.Drawing.Point(3, 3);
            this.gvOut.Multiline = true;
            this.gvOut.Name = "gvOut";
            this.gvOut.ReadOnly = true;
            this.gvOut.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.gvOut.Size = new System.Drawing.Size(313, 447);
            this.gvOut.TabIndex = 9;
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.IsSplitterFixed = true;
            this.splitContainer.Location = new System.Drawing.Point(12, 137);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.outputArea);
            this.splitContainer.Panel1.Controls.Add(this.engraveButton);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.gvOut);
            this.splitContainer.Panel2.Controls.Add(this.drawGraphButton);
            this.splitContainer.Size = new System.Drawing.Size(643, 482);
            this.splitContainer.SplitterDistance = 320;
            this.splitContainer.TabIndex = 18;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.barCountLabel);
            this.groupBox1.Controls.Add(this.clefList);
            this.groupBox1.Controls.Add(this.barsCount);
            this.groupBox1.Controls.Add(this.imperfectTime);
            this.groupBox1.Controls.Add(this.perfectTime);
            this.groupBox1.Controls.Add(this.startNotes);
            this.groupBox1.Controls.Add(this.modiList);
            this.groupBox1.Location = new System.Drawing.Point(12, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(415, 124);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Параметры мелодии";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.stepLimitLabel);
            this.groupBox2.Controls.Add(this.seedLabel);
            this.groupBox2.Controls.Add(this.maxSteps);
            this.groupBox2.Controls.Add(this.randSeedDD);
            this.groupBox2.Controls.Add(this.makeButton);
            this.groupBox2.Location = new System.Drawing.Point(433, 7);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(222, 123);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Параметры генерации";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(663, 623);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.splitContainer);
            this.Name = "MainForm";
            this.Text = "Генератор мелодий";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barsCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.randSeedDD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxSteps)).EndInit();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

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
        private System.Windows.Forms.Label barCountLabel;
        private System.Windows.Forms.Button engraveButton;
        private System.Windows.Forms.Button drawGraphButton;
        private System.Windows.Forms.NumericUpDown randSeedDD;
        private System.Windows.Forms.NumericUpDown maxSteps;
        private System.Windows.Forms.Label seedLabel;
        private System.Windows.Forms.Label stepLimitLabel;
        private System.Windows.Forms.TextBox gvOut;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;

    }
}

