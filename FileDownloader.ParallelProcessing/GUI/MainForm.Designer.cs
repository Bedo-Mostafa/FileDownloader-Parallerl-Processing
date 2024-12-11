namespace FileDownloader.ParallelProcessing
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            MultipleThreadButton = new Button();
            MultiThreadLabel = new Label();
            SingleThreadButton = new Button();
            MultiThreadNoteLabel = new Label();
            ProjectTitle = new Label();
            SingleThreadLabel = new Label();
            NoteLabel = new Label();
            ThreadsNumberInput = new TextBox();
            label1 = new Label();
            SuspendLayout();
            // 
            // MultipleThreadButton
            // 
            MultipleThreadButton.BackColor = Color.LimeGreen;
            MultipleThreadButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            MultipleThreadButton.ForeColor = Color.White;
            MultipleThreadButton.Location = new Point(95, 227);
            MultipleThreadButton.Name = "MultipleThreadButton";
            MultipleThreadButton.Size = new Size(191, 56);
            MultipleThreadButton.TabIndex = 0;
            MultipleThreadButton.Text = "Download From Here";
            MultipleThreadButton.UseVisualStyleBackColor = false;
            MultipleThreadButton.Click += MultipleThreadButtonClick;
            // 
            // MultiThreadLabel
            // 
            MultiThreadLabel.AutoSize = true;
            MultiThreadLabel.ForeColor = Color.White;
            MultiThreadLabel.Location = new Point(95, 184);
            MultiThreadLabel.Name = "MultiThreadLabel";
            MultiThreadLabel.Size = new Size(120, 20);
            MultiThreadLabel.TabIndex = 1;
            MultiThreadLabel.Text = "Multiple Threads";
            MultiThreadLabel.Click += MultipleThreadLabelClick;
            // 
            // SingleThreadButton
            // 
            SingleThreadButton.BackColor = Color.FromArgb(255, 128, 0);
            SingleThreadButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            SingleThreadButton.ForeColor = SystemColors.ControlLightLight;
            SingleThreadButton.Location = new Point(543, 227);
            SingleThreadButton.Name = "SingleThreadButton";
            SingleThreadButton.Size = new Size(191, 56);
            SingleThreadButton.TabIndex = 2;
            SingleThreadButton.Text = "Download From Here\r\n";
            SingleThreadButton.UseVisualStyleBackColor = false;
            SingleThreadButton.Click += SingleThreadLabelClick;
            // 
            // MultiThreadNoteLabel
            // 
            MultiThreadNoteLabel.AutoSize = true;
            MultiThreadNoteLabel.BackColor = SystemColors.InfoText;
            MultiThreadNoteLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            MultiThreadNoteLabel.ForeColor = Color.LightGreen;
            MultiThreadNoteLabel.Location = new Point(95, 304);
            MultiThreadNoteLabel.Name = "MultiThreadNoteLabel";
            MultiThreadNoteLabel.Size = new Size(399, 23);
            MultiThreadNoteLabel.TabIndex = 4;
            MultiThreadNoteLabel.Text = "You Will be able to download Multiple Files here";
            MultiThreadNoteLabel.Click += MultiThreadNoteLabelClick;
            // 
            // ProjectTitle
            // 
            ProjectTitle.AutoSize = true;
            ProjectTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            ProjectTitle.ForeColor = Color.White;
            ProjectTitle.Location = new Point(269, 38);
            ProjectTitle.Name = "ProjectTitle";
            ProjectTitle.Size = new Size(248, 41);
            ProjectTitle.TabIndex = 5;
            ProjectTitle.Text = "File Downloader";
            ProjectTitle.Click += ProjectTitleClick;
            // 
            // SingleThreadLabel
            // 
            SingleThreadLabel.AutoSize = true;
            SingleThreadLabel.ForeColor = Color.White;
            SingleThreadLabel.Location = new Point(543, 184);
            SingleThreadLabel.Name = "SingleThreadLabel";
            SingleThreadLabel.Size = new Size(100, 20);
            SingleThreadLabel.TabIndex = 6;
            SingleThreadLabel.Text = "Single Thread";
            SingleThreadLabel.Click += SingleThreadLabelClick;
            // 
            // NoteLabel
            // 
            NoteLabel.AutoSize = true;
            NoteLabel.BackColor = SystemColors.InfoText;
            NoteLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            NoteLabel.ForeColor = Color.Tomato;
            NoteLabel.Location = new Point(95, 109);
            NoteLabel.Name = "NoteLabel";
            NoteLabel.Size = new Size(392, 23);
            NoteLabel.TabIndex = 7;
            NoteLabel.Text = "You Can Download Files from any Button Below";
            NoteLabel.Click += NoteLabelClick;
            // 
            // ThreadsNumberInput
            // 
            ThreadsNumberInput.BackColor = SystemColors.HotTrack;
            ThreadsNumberInput.Font = new Font("Segoe UI", 9F, FontStyle.Underline);
            ThreadsNumberInput.ForeColor = SystemColors.InactiveBorder;
            ThreadsNumberInput.Location = new Point(95, 411);
            ThreadsNumberInput.Name = "ThreadsNumberInput";
            ThreadsNumberInput.Size = new Size(42, 27);
            ThreadsNumberInput.TabIndex = 8;
            ThreadsNumberInput.Text = "5";
            ThreadsNumberInput.TextChanged += ThreadsNumberInput_TextChanged;
            ThreadsNumberInput.KeyPress += ThreadsNumberInput_KeyPress;
            ThreadsNumberInput.Validating += ThreadsNumberInput_Validating;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = SystemColors.InfoText;
            label1.Font = new Font("Verdana", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.LightGreen;
            label1.Location = new Point(95, 343);
            label1.Name = "label1";
            label1.Padding = new Padding(4);
            label1.Size = new Size(536, 48);
            label1.TabIndex = 9;
            label1.Text = "Enter the number of threads you want to \r\nuse (if you don't understand it just ignore it) by default is 5\r\n";
            label1.Click += label1_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(28, 28, 28);
            ClientSize = new Size(800, 470);
            Controls.Add(label1);
            Controls.Add(ThreadsNumberInput);
            Controls.Add(NoteLabel);
            Controls.Add(SingleThreadLabel);
            Controls.Add(ProjectTitle);
            Controls.Add(MultiThreadNoteLabel);
            Controls.Add(SingleThreadButton);
            Controls.Add(MultiThreadLabel);
            Controls.Add(MultipleThreadButton);
            Name = "MainForm";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button MultipleThreadButton;
        private Label MultiThreadLabel;
        private Button SingleThreadButton;
        private Label MultiThreadNoteLabel;
        private Label ProjectTitle;
        private Label SingleThreadLabel;
        private Label NoteLabel;
        private TextBox ThreadsNumberInput;
        private Label label1;
    }
}
