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
            this.randSeedDD = new System.Windows.Forms.NumericUpDown();
            this.maxSteps = new System.Windows.Forms.NumericUpDown();
            this.seedLabel = new System.Windows.Forms.Label();
            this.stepLimitLabel = new System.Windows.Forms.Label();
            this.playButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.settingsButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.stepsProgressBar = new System.Windows.Forms.ProgressBar();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.imitationSettingsBox = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.imitationRange = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.ImitationTopFirst = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.imitationDelay = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.imitationInterval = new System.Windows.Forms.ListBox();
            this.imitationEnabled = new System.Windows.Forms.CheckBox();
            this.melodyProgressBar = new System.Windows.Forms.ProgressBar();
            this.rollbacksProgressBar = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.barsCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.randSeedDD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxSteps)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.imitationSettingsBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imitationRange)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imitationDelay)).BeginInit();
            this.SuspendLayout();
            // 
            // outputArea
            // 
            this.outputArea.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.outputArea.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.outputArea.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.outputArea.Location = new System.Drawing.Point(308, 12);
            this.outputArea.Multiline = true;
            this.outputArea.Name = "outputArea";
            this.outputArea.ReadOnly = true;
            this.outputArea.Size = new System.Drawing.Size(381, 520);
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
            this.perfectTime.Location = new System.Drawing.Point(10, 19);
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
            this.imperfectTime.Location = new System.Drawing.Point(10, 42);
            this.imperfectTime.Name = "imperfectTime";
            this.imperfectTime.Size = new System.Drawing.Size(42, 17);
            this.imperfectTime.TabIndex = 5;
            this.imperfectTime.TabStop = true;
            this.imperfectTime.Text = "4/2";
            this.imperfectTime.UseVisualStyleBackColor = true;
            // 
            // barsCount
            // 
            this.barsCount.Location = new System.Drawing.Point(89, 39);
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
            this.makeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.makeButton.Location = new System.Drawing.Point(11, 515);
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
            this.clefList.SelectedIndexChanged += new System.EventHandler(this.UpdateImitationSettingsEnabled);
            // 
            // barCountLabel
            // 
            this.barCountLabel.AutoSize = true;
            this.barCountLabel.Location = new System.Drawing.Point(86, 23);
            this.barCountLabel.Name = "barCountLabel";
            this.barCountLabel.Size = new System.Drawing.Size(103, 13);
            this.barCountLabel.TabIndex = 10;
            this.barCountLabel.Text = "Количество тактов";
            // 
            // engraveButton
            // 
            this.engraveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.engraveButton.Enabled = false;
            this.engraveButton.Location = new System.Drawing.Point(583, 538);
            this.engraveButton.Name = "engraveButton";
            this.engraveButton.Size = new System.Drawing.Size(106, 25);
            this.engraveButton.TabIndex = 12;
            this.engraveButton.Text = "Рисовать";
            this.engraveButton.UseVisualStyleBackColor = true;
            this.engraveButton.Click += new System.EventHandler(this.engraveButton_Click);
            // 
            // randSeedDD
            // 
            this.randSeedDD.Location = new System.Drawing.Point(87, 19);
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
            this.randSeedDD.Size = new System.Drawing.Size(108, 20);
            this.randSeedDD.TabIndex = 15;
            // 
            // maxSteps
            // 
            this.maxSteps.Location = new System.Drawing.Point(87, 41);
            this.maxSteps.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.maxSteps.Name = "maxSteps";
            this.maxSteps.Size = new System.Drawing.Size(108, 20);
            this.maxSteps.TabIndex = 16;
            this.maxSteps.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            // 
            // seedLabel
            // 
            this.seedLabel.AutoSize = true;
            this.seedLabel.Location = new System.Drawing.Point(46, 20);
            this.seedLabel.Name = "seedLabel";
            this.seedLabel.Size = new System.Drawing.Size(38, 13);
            this.seedLabel.TabIndex = 17;
            this.seedLabel.Text = "Зерно";
            // 
            // stepLimitLabel
            // 
            this.stepLimitLabel.AutoSize = true;
            this.stepLimitLabel.Location = new System.Drawing.Point(10, 43);
            this.stepLimitLabel.Name = "stepLimitLabel";
            this.stepLimitLabel.Size = new System.Drawing.Size(74, 13);
            this.stepLimitLabel.TabIndex = 17;
            this.stepLimitLabel.Text = "Лимит шагов";
            // 
            // playButton
            // 
            this.playButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.playButton.Enabled = false;
            this.playButton.Location = new System.Drawing.Point(471, 538);
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(106, 25);
            this.playButton.TabIndex = 13;
            this.playButton.Text = "Послушать";
            this.playButton.UseVisualStyleBackColor = true;
            this.playButton.Click += new System.EventHandler(this.playButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.clefList);
            this.groupBox1.Controls.Add(this.startNotes);
            this.groupBox1.Controls.Add(this.modiList);
            this.groupBox1.Location = new System.Drawing.Point(12, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(290, 127);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Лад и ключи";
            // 
            // settingsButton
            // 
            this.settingsButton.Location = new System.Drawing.Point(88, 68);
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Size = new System.Drawing.Size(107, 27);
            this.settingsButton.TabIndex = 11;
            this.settingsButton.Text = "Настройки...";
            this.settingsButton.UseVisualStyleBackColor = true;
            this.settingsButton.Click += new System.EventHandler(this.settingsButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.stepLimitLabel);
            this.groupBox2.Controls.Add(this.settingsButton);
            this.groupBox2.Controls.Add(this.seedLabel);
            this.groupBox2.Controls.Add(this.maxSteps);
            this.groupBox2.Controls.Add(this.randSeedDD);
            this.groupBox2.Location = new System.Drawing.Point(11, 407);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(203, 102);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Параметры генерации";
            // 
            // stepsProgressBar
            // 
            this.stepsProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stepsProgressBar.Location = new System.Drawing.Point(11, 569);
            this.stepsProgressBar.Name = "stepsProgressBar";
            this.stepsProgressBar.Size = new System.Drawing.Size(678, 10);
            this.stepsProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.stepsProgressBar.TabIndex = 21;
            this.stepsProgressBar.Click += new System.EventHandler(this.generationProgressBar_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.perfectTime);
            this.groupBox3.Controls.Add(this.imperfectTime);
            this.groupBox3.Controls.Add(this.barCountLabel);
            this.groupBox3.Controls.Add(this.barsCount);
            this.groupBox3.Location = new System.Drawing.Point(11, 144);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(203, 68);
            this.groupBox3.TabIndex = 22;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Размер и продолжительность";
            // 
            // imitationSettingsBox
            // 
            this.imitationSettingsBox.Controls.Add(this.label5);
            this.imitationSettingsBox.Controls.Add(this.imitationRange);
            this.imitationSettingsBox.Controls.Add(this.label6);
            this.imitationSettingsBox.Controls.Add(this.radioButton2);
            this.imitationSettingsBox.Controls.Add(this.ImitationTopFirst);
            this.imitationSettingsBox.Controls.Add(this.label4);
            this.imitationSettingsBox.Controls.Add(this.label3);
            this.imitationSettingsBox.Controls.Add(this.imitationDelay);
            this.imitationSettingsBox.Controls.Add(this.label2);
            this.imitationSettingsBox.Controls.Add(this.label1);
            this.imitationSettingsBox.Controls.Add(this.imitationInterval);
            this.imitationSettingsBox.Enabled = false;
            this.imitationSettingsBox.Location = new System.Drawing.Point(12, 246);
            this.imitationSettingsBox.Name = "imitationSettingsBox";
            this.imitationSettingsBox.Size = new System.Drawing.Size(255, 155);
            this.imitationSettingsBox.TabIndex = 23;
            this.imitationSettingsBox.TabStop = false;
            this.imitationSettingsBox.Text = "Параметры имитации";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(83, 131);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "тактов";
            // 
            // imitationRange
            // 
            this.imitationRange.Location = new System.Drawing.Point(12, 129);
            this.imitationRange.Name = "imitationRange";
            this.imitationRange.Size = new System.Drawing.Size(65, 20);
            this.imitationRange.TabIndex = 9;
            this.imitationRange.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 113);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(111, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Продолжительность";
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Checked = true;
            this.radioButton2.Location = new System.Drawing.Point(79, 32);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(65, 17);
            this.radioButton2.TabIndex = 7;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "Нижний";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // ImitationTopFirst
            // 
            this.ImitationTopFirst.AutoSize = true;
            this.ImitationTopFirst.Location = new System.Drawing.Point(10, 32);
            this.ImitationTopFirst.Name = "ImitationTopFirst";
            this.ImitationTopFirst.Size = new System.Drawing.Size(67, 17);
            this.ImitationTopFirst.TabIndex = 6;
            this.ImitationTopFirst.Text = "Верхний";
            this.ImitationTopFirst.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Пропоста";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(84, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "половиных";
            // 
            // imitationDelay
            // 
            this.imitationDelay.Location = new System.Drawing.Point(13, 83);
            this.imitationDelay.Name = "imitationDelay";
            this.imitationDelay.Size = new System.Drawing.Size(65, 20);
            this.imitationDelay.TabIndex = 3;
            this.imitationDelay.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Расстояние";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(147, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Интервал";
            // 
            // imitationInterval
            // 
            this.imitationInterval.FormattingEnabled = true;
            this.imitationInterval.Items.AddRange(new object[] {
            "Прима",
            "Секунда",
            "Терция",
            "Кварта",
            "Квинта",
            "Секста",
            "Септима",
            "Октава",
            "Нона",
            "Децима"});
            this.imitationInterval.Location = new System.Drawing.Point(150, 32);
            this.imitationInterval.Name = "imitationInterval";
            this.imitationInterval.Size = new System.Drawing.Size(94, 108);
            this.imitationInterval.TabIndex = 0;
            // 
            // imitationEnabled
            // 
            this.imitationEnabled.AutoSize = true;
            this.imitationEnabled.Location = new System.Drawing.Point(18, 223);
            this.imitationEnabled.Name = "imitationEnabled";
            this.imitationEnabled.Size = new System.Drawing.Size(116, 17);
            this.imitationEnabled.TabIndex = 8;
            this.imitationEnabled.Text = "Имитация (канон)";
            this.imitationEnabled.UseVisualStyleBackColor = true;
            this.imitationEnabled.CheckedChanged += new System.EventHandler(this.UpdateImitationSettingsEnabled);
            // 
            // melodyProgressBar
            // 
            this.melodyProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.melodyProgressBar.Location = new System.Drawing.Point(353, 585);
            this.melodyProgressBar.Name = "melodyProgressBar";
            this.melodyProgressBar.Size = new System.Drawing.Size(336, 10);
            this.melodyProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.melodyProgressBar.TabIndex = 24;
            // 
            // rollbacksProgressBar
            // 
            this.rollbacksProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rollbacksProgressBar.Location = new System.Drawing.Point(12, 585);
            this.rollbacksProgressBar.Name = "rollbacksProgressBar";
            this.rollbacksProgressBar.Size = new System.Drawing.Size(335, 10);
            this.rollbacksProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.rollbacksProgressBar.TabIndex = 25;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(697, 597);
            this.Controls.Add(this.rollbacksProgressBar);
            this.Controls.Add(this.melodyProgressBar);
            this.Controls.Add(this.imitationEnabled);
            this.Controls.Add(this.imitationSettingsBox);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.engraveButton);
            this.Controls.Add(this.playButton);
            this.Controls.Add(this.makeButton);
            this.Controls.Add(this.stepsProgressBar);
            this.Controls.Add(this.outputArea);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Генератор мелодий";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barsCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.randSeedDD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxSteps)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.imitationSettingsBox.ResumeLayout(false);
            this.imitationSettingsBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imitationRange)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imitationDelay)).EndInit();
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
        private System.Windows.Forms.Label barCountLabel;
        private System.Windows.Forms.Button engraveButton;
        private System.Windows.Forms.NumericUpDown randSeedDD;
        private System.Windows.Forms.NumericUpDown maxSteps;
        private System.Windows.Forms.Label seedLabel;
        private System.Windows.Forms.Label stepLimitLabel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button playButton;
        private System.Windows.Forms.Button settingsButton;
        private System.Windows.Forms.ProgressBar stepsProgressBar;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox imitationSettingsBox;
        private System.Windows.Forms.ListBox imitationInterval;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown imitationDelay;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton ImitationTopFirst;
        private System.Windows.Forms.CheckBox imitationEnabled;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown imitationRange;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ProgressBar melodyProgressBar;
        private System.Windows.Forms.ProgressBar rollbacksProgressBar;

    }
}

