namespace SpellChecker_GUI
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.smallerFontButton = new System.Windows.Forms.Button();
            this.biggerFontButton = new System.Windows.Forms.Button();
            this.StopButton = new System.Windows.Forms.Button();
            this.startButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.NumOfErrorsLabel = new System.Windows.Forms.Label();
            this.pasteButton = new System.Windows.Forms.Button();
            this.copyButton = new System.Windows.Forms.Button();
            this.selectAllButton = new System.Windows.Forms.Button();
            this.langComboBox = new System.Windows.Forms.ComboBox();
            this.upButton = new System.Windows.Forms.Button();
            this.rightButton = new System.Windows.Forms.Button();
            this.downButton = new System.Windows.Forms.Button();
            this.leftButton = new System.Windows.Forms.Button();
            this.acceptWordButton = new System.Windows.Forms.Button();
            this.acceptedTextBox = new System.Windows.Forms.TextBox();
            this.sugestedListBox = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.richTextBox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.label6);
            this.splitContainer1.Panel2.Controls.Add(this.label5);
            this.splitContainer1.Panel2.Controls.Add(this.label4);
            this.splitContainer1.Panel2.Controls.Add(this.label3);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.smallerFontButton);
            this.splitContainer1.Panel2.Controls.Add(this.biggerFontButton);
            this.splitContainer1.Panel2.Controls.Add(this.StopButton);
            this.splitContainer1.Panel2.Controls.Add(this.startButton);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Controls.Add(this.NumOfErrorsLabel);
            this.splitContainer1.Panel2.Controls.Add(this.pasteButton);
            this.splitContainer1.Panel2.Controls.Add(this.copyButton);
            this.splitContainer1.Panel2.Controls.Add(this.selectAllButton);
            this.splitContainer1.Panel2.Controls.Add(this.langComboBox);
            this.splitContainer1.Panel2.Controls.Add(this.upButton);
            this.splitContainer1.Panel2.Controls.Add(this.rightButton);
            this.splitContainer1.Panel2.Controls.Add(this.downButton);
            this.splitContainer1.Panel2.Controls.Add(this.leftButton);
            this.splitContainer1.Panel2.Controls.Add(this.acceptWordButton);
            this.splitContainer1.Panel2.Controls.Add(this.acceptedTextBox);
            this.splitContainer1.Panel2.Controls.Add(this.sugestedListBox);
            this.splitContainer1.Size = new System.Drawing.Size(1008, 611);
            this.splitContainer1.SplitterDistance = 389;
            this.splitContainer1.TabIndex = 0;
            // 
            // richTextBox
            // 
            this.richTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox.Location = new System.Drawing.Point(0, 0);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.richTextBox.Size = new System.Drawing.Size(1008, 389);
            this.richTextBox.TabIndex = 0;
            this.richTextBox.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(735, 190);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(184, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "https://opensource.org/licenses/MIT";
            // 
            // smallerFontButton
            // 
            this.smallerFontButton.Location = new System.Drawing.Point(839, 15);
            this.smallerFontButton.Name = "smallerFontButton";
            this.smallerFontButton.Size = new System.Drawing.Size(140, 58);
            this.smallerFontButton.TabIndex = 16;
            this.smallerFontButton.Text = "Smaller font";
            this.smallerFontButton.UseVisualStyleBackColor = true;
            this.smallerFontButton.Click += new System.EventHandler(this.smallerFontButton_Click);
            // 
            // biggerFontButton
            // 
            this.biggerFontButton.Location = new System.Drawing.Point(685, 15);
            this.biggerFontButton.Name = "biggerFontButton";
            this.biggerFontButton.Size = new System.Drawing.Size(140, 58);
            this.biggerFontButton.TabIndex = 15;
            this.biggerFontButton.Text = "Bigger font";
            this.biggerFontButton.UseVisualStyleBackColor = true;
            this.biggerFontButton.Click += new System.EventHandler(this.biggerFontButton_Click);
            // 
            // StopButton
            // 
            this.StopButton.Location = new System.Drawing.Point(379, 133);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(136, 70);
            this.StopButton.TabIndex = 14;
            this.StopButton.Text = "Stop spell checking";
            this.StopButton.UseVisualStyleBackColor = true;
            this.StopButton.Click += new System.EventHandler(this.StopButton_Click);
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(379, 62);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(136, 65);
            this.startButton.TabIndex = 13;
            this.startButton.Text = "Start spell checking";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(380, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Number of errors:";
            // 
            // NumOfErrorsLabel
            // 
            this.NumOfErrorsLabel.AutoSize = true;
            this.NumOfErrorsLabel.Location = new System.Drawing.Point(480, 44);
            this.NumOfErrorsLabel.Name = "NumOfErrorsLabel";
            this.NumOfErrorsLabel.Size = new System.Drawing.Size(13, 13);
            this.NumOfErrorsLabel.TabIndex = 11;
            this.NumOfErrorsLabel.Text = "0";
            // 
            // pasteButton
            // 
            this.pasteButton.Location = new System.Drawing.Point(531, 145);
            this.pasteButton.Name = "pasteButton";
            this.pasteButton.Size = new System.Drawing.Size(140, 58);
            this.pasteButton.TabIndex = 10;
            this.pasteButton.Text = "Paste";
            this.pasteButton.UseVisualStyleBackColor = true;
            this.pasteButton.Click += new System.EventHandler(this.pasteButton_Click);
            // 
            // copyButton
            // 
            this.copyButton.Location = new System.Drawing.Point(531, 80);
            this.copyButton.Name = "copyButton";
            this.copyButton.Size = new System.Drawing.Size(140, 58);
            this.copyButton.TabIndex = 9;
            this.copyButton.Text = "Copy";
            this.copyButton.UseVisualStyleBackColor = true;
            this.copyButton.Click += new System.EventHandler(this.copyButton_Click);
            // 
            // selectAllButton
            // 
            this.selectAllButton.Location = new System.Drawing.Point(531, 15);
            this.selectAllButton.Name = "selectAllButton";
            this.selectAllButton.Size = new System.Drawing.Size(140, 58);
            this.selectAllButton.TabIndex = 8;
            this.selectAllButton.Text = "Select all";
            this.selectAllButton.UseVisualStyleBackColor = true;
            this.selectAllButton.Click += new System.EventHandler(this.selectAllButton_Click);
            // 
            // langComboBox
            // 
            this.langComboBox.FormattingEnabled = true;
            this.langComboBox.Location = new System.Drawing.Point(379, 16);
            this.langComboBox.Name = "langComboBox";
            this.langComboBox.Size = new System.Drawing.Size(136, 21);
            this.langComboBox.TabIndex = 7;
            this.langComboBox.SelectedIndexChanged += new System.EventHandler(this.langComboBox_SelectedIndexChanged);
            // 
            // upButton
            // 
            this.upButton.Image = global::SpellChecker_GUI.Properties.Resources.arrowUp;
            this.upButton.Location = new System.Drawing.Point(220, 133);
            this.upButton.Name = "upButton";
            this.upButton.Size = new System.Drawing.Size(70, 70);
            this.upButton.TabIndex = 6;
            this.upButton.UseVisualStyleBackColor = true;
            this.upButton.Click += new System.EventHandler(this.upButton_Click);
            // 
            // rightButton
            // 
            this.rightButton.Image = global::SpellChecker_GUI.Properties.Resources.arrowRight;
            this.rightButton.Location = new System.Drawing.Point(296, 57);
            this.rightButton.Name = "rightButton";
            this.rightButton.Size = new System.Drawing.Size(70, 70);
            this.rightButton.TabIndex = 5;
            this.rightButton.UseVisualStyleBackColor = true;
            this.rightButton.Click += new System.EventHandler(this.rightButton_Click);
            // 
            // downButton
            // 
            this.downButton.Image = global::SpellChecker_GUI.Properties.Resources.arrowDown;
            this.downButton.Location = new System.Drawing.Point(296, 133);
            this.downButton.Name = "downButton";
            this.downButton.Size = new System.Drawing.Size(70, 70);
            this.downButton.TabIndex = 4;
            this.downButton.UseVisualStyleBackColor = true;
            this.downButton.Click += new System.EventHandler(this.downButton_Click);
            // 
            // leftButton
            // 
            this.leftButton.Image = global::SpellChecker_GUI.Properties.Resources.arrowLeft;
            this.leftButton.Location = new System.Drawing.Point(220, 57);
            this.leftButton.Name = "leftButton";
            this.leftButton.Size = new System.Drawing.Size(70, 70);
            this.leftButton.TabIndex = 3;
            this.leftButton.UseVisualStyleBackColor = true;
            this.leftButton.Click += new System.EventHandler(this.leftButton_Click);
            // 
            // acceptWordButton
            // 
            this.acceptWordButton.Location = new System.Drawing.Point(220, 15);
            this.acceptWordButton.Name = "acceptWordButton";
            this.acceptWordButton.Size = new System.Drawing.Size(146, 36);
            this.acceptWordButton.TabIndex = 2;
            this.acceptWordButton.Text = "Accept word";
            this.acceptWordButton.UseVisualStyleBackColor = true;
            this.acceptWordButton.Click += new System.EventHandler(this.acceptWordButton_Click);
            // 
            // acceptedTextBox
            // 
            this.acceptedTextBox.Location = new System.Drawing.Point(13, 16);
            this.acceptedTextBox.Name = "acceptedTextBox";
            this.acceptedTextBox.Size = new System.Drawing.Size(193, 20);
            this.acceptedTextBox.TabIndex = 1;
            this.acceptedTextBox.TextChanged += new System.EventHandler(this.acceptedTextBox_TextChanged);
            // 
            // sugestedListBox
            // 
            this.sugestedListBox.FormattingEnabled = true;
            this.sugestedListBox.Location = new System.Drawing.Point(13, 43);
            this.sugestedListBox.Name = "sugestedListBox";
            this.sugestedListBox.Size = new System.Drawing.Size(193, 160);
            this.sugestedListBox.TabIndex = 0;
            this.sugestedListBox.Click += new System.EventHandler(this.sugestedListBox_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(691, 174);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(141, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "License: MIT Open software";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(691, 111);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(173, 13);
            this.label4.TabIndex = 19;
            this.label4.Text = "Developed by João Nuno Carvalho";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(691, 132);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(161, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "Email: joaonunocarv@gmail.com";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(691, 153);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(251, 13);
            this.label6.TabIndex = 21;
            this.label6.Text = "Project page: https://github.com/joaocarvalhoopen";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 611);
            this.Controls.Add(this.splitContainer1);
            this.MinimumSize = new System.Drawing.Size(1024, 300);
            this.Name = "MainForm";
            this.Text = "USB Spell Checker GUI";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.RichTextBox richTextBox;
        private System.Windows.Forms.TextBox acceptedTextBox;
        private System.Windows.Forms.ListBox sugestedListBox;
        private System.Windows.Forms.Button leftButton;
        private System.Windows.Forms.Button acceptWordButton;
        private System.Windows.Forms.Button upButton;
        private System.Windows.Forms.Button rightButton;
        private System.Windows.Forms.Button downButton;
        private System.Windows.Forms.Button pasteButton;
        private System.Windows.Forms.Button copyButton;
        private System.Windows.Forms.Button selectAllButton;
        private System.Windows.Forms.ComboBox langComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label NumOfErrorsLabel;
        private System.Windows.Forms.Button StopButton;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button biggerFontButton;
        private System.Windows.Forms.Button smallerFontButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
    }
}

