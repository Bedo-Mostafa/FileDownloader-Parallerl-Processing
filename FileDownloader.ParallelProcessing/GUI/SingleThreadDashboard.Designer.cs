//using FileDownloader.ParallelProcessing.Models;
using FileDownloader.ParallelProcessing.Models;
using FileInfo = FileDownloader.ParallelProcessing.Models.FileInfo;

namespace FileDownloader.ParallelProcessing
{
    partial class SingleThreadDashboard
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
            ApplicationContainer = new Panel();
            DownloadsContainer = new Panel();
            LocationInput = new TextBox();
            SetLocationButton = new Button();
            DownloadButton = new Button();
            InstructionLabel = new Label();
            URLTextBox = new TextBox();
            URLLabel = new Label();
            DownloadPanel = new Panel();
            Resume = new Button();
            Cancel = new Button();
            Pause = new Button();
            SpeedValue = new Label();
            SpeedLabel = new Label();
            DownloadedValue = new Label();
            DownloadedLabel = new Label();
            ProgressBar = new ProgressBar();
            FileNameValue = new Label();
            FileNameLabel = new Label();
            ApplicationContainer.SuspendLayout();
            SuspendLayout();
            // 
            // ApplicationContainer
            // 
            ApplicationContainer.Controls.Add(DownloadsContainer);
            ApplicationContainer.Controls.Add(LocationInput);
            ApplicationContainer.Controls.Add(SetLocationButton);
            ApplicationContainer.Controls.Add(DownloadButton);
            ApplicationContainer.Controls.Add(InstructionLabel);
            ApplicationContainer.Controls.Add(URLTextBox);
            ApplicationContainer.Controls.Add(URLLabel);
            ApplicationContainer.ForeColor = SystemColors.ButtonHighlight;
            ApplicationContainer.Location = new Point(36, 12);
            ApplicationContainer.Name = "ApplicationContainer";
            ApplicationContainer.Size = new Size(1214, 649);
            ApplicationContainer.TabIndex = 1;
            // 
            // DownloadsContainer
            // 
            DownloadsContainer.AutoScroll = true;
            DownloadsContainer.Location = new Point(15, 193);
            DownloadsContainer.Name = "DownloadsContainer";
            DownloadsContainer.Size = new Size(1196, 444);
            DownloadsContainer.TabIndex = 7;
            // 
            // LocationInput
            // 
            LocationInput.BackColor = Color.FromArgb(50, 50, 50);
            LocationInput.ForeColor = SystemColors.Info;
            LocationInput.Location = new Point(537, 104);
            LocationInput.Name = "LocationInput";
            LocationInput.Size = new Size(500, 27);
            LocationInput.TabIndex = 6;
            //LocationInput.TextChanged += DownloadPath_Text;
            // 
            // SetLocationButton
            // 
            SetLocationButton.BackColor = Color.SandyBrown;
            SetLocationButton.FlatAppearance.BorderColor = Color.Black;
            SetLocationButton.ForeColor = SystemColors.HighlightText;
            SetLocationButton.Location = new Point(432, 103);
            SetLocationButton.Name = "SetLocationButton";
            SetLocationButton.Size = new Size(99, 28);
            SetLocationButton.TabIndex = 5;
            SetLocationButton.Text = "Browse";
            SetLocationButton.UseVisualStyleBackColor = false;
            SetLocationButton.Click += SetLocationButton_Click;
            // 
            // DownloadButton
            // 
            DownloadButton.BackColor = Color.RoyalBlue;
            DownloadButton.FlatAppearance.BorderColor = Color.Black;
            DownloadButton.ForeColor = SystemColors.HighlightText;
            DownloadButton.Location = new Point(68, 104);
            DownloadButton.Name = "DownloadButton";
            DownloadButton.Size = new Size(150, 28);
            DownloadButton.TabIndex = 4;
            DownloadButton.Text = "Download";
            DownloadButton.UseVisualStyleBackColor = false;
            DownloadButton.Click += DownloadButtonSingleThread;
            // 
            // InstructionLabel
            // 
            InstructionLabel.AutoSize = true;
            InstructionLabel.Font = new Font("Segoe Script", 9F);
            InstructionLabel.ForeColor = Color.White;
            InstructionLabel.Location = new Point(68, 26);
            InstructionLabel.Name = "InstructionLabel";
            InstructionLabel.Size = new Size(403, 23);
            InstructionLabel.TabIndex = 3;
            InstructionLabel.Text = "Please Enter the URL Link in the TextBox Below\r\n";
            // 
            // URLTextBox
            // 
            URLTextBox.BackColor = Color.FromArgb(50, 50, 50);
            URLTextBox.ForeColor = SystemColors.Info;
            URLTextBox.Location = new Point(68, 65);
            URLTextBox.Name = "URLTextBox";
            URLTextBox.Size = new Size(969, 27);
            URLTextBox.TabIndex = 1;
            URLTextBox.Text = "HTTTP";
            // 
            // URLLabel
            // 
            URLLabel.AutoSize = true;
            URLLabel.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            URLLabel.ForeColor = SystemColors.ButtonHighlight;
            URLLabel.Location = new Point(15, 65);
            URLLabel.Name = "URLLabel";
            URLLabel.Size = new Size(47, 28);
            URLLabel.TabIndex = 0;
            URLLabel.Text = "URL";
            // 
            // DownloadPanel
            // 
            DownloadPanel.Location = new Point(0, 0);
            DownloadPanel.Name = "DownloadPanel";
            DownloadPanel.Size = new Size(200, 100);
            DownloadPanel.TabIndex = 0;
            // 
            // Resume
            // 
            Resume.Location = new Point(0, 0);
            Resume.Name = "Resume";
            Resume.Size = new Size(75, 23);
            Resume.TabIndex = 0;
            // 
            // Cancel
            // 
            Cancel.Location = new Point(0, 0);
            Cancel.Name = "Cancel";
            Cancel.Size = new Size(75, 23);
            Cancel.TabIndex = 0;
            // 
            // Pause
            // 
            Pause.Location = new Point(0, 0);
            Pause.Name = "Pause";
            Pause.Size = new Size(75, 23);
            Pause.TabIndex = 0;
            // 
            // SpeedValue
            // 
            SpeedValue.Location = new Point(0, 0);
            SpeedValue.Name = "SpeedValue";
            SpeedValue.Size = new Size(100, 23);
            SpeedValue.TabIndex = 0;
            // 
            // SpeedLabel
            // 
            SpeedLabel.Location = new Point(0, 0);
            SpeedLabel.Name = "SpeedLabel";
            SpeedLabel.Size = new Size(100, 23);
            SpeedLabel.TabIndex = 0;
            // 
            // DownloadedValue
            // 
            DownloadedValue.Location = new Point(0, 0);
            DownloadedValue.Name = "DownloadedValue";
            DownloadedValue.Size = new Size(100, 23);
            DownloadedValue.TabIndex = 0;
            // 
            // DownloadedLabel
            // 
            DownloadedLabel.Location = new Point(0, 0);
            DownloadedLabel.Name = "DownloadedLabel";
            DownloadedLabel.Size = new Size(100, 23);
            DownloadedLabel.TabIndex = 0;
            // 
            // ProgressBar
            // 
            ProgressBar.Location = new Point(0, 0);
            ProgressBar.Name = "ProgressBar";
            ProgressBar.Size = new Size(100, 23);
            ProgressBar.TabIndex = 0;
            // 
            // FileNameValue
            // 
            FileNameValue.Location = new Point(0, 0);
            FileNameValue.Name = "FileNameValue";
            FileNameValue.Size = new Size(100, 23);
            FileNameValue.TabIndex = 0;
            // 
            // FileNameLabel
            // 
            FileNameLabel.Location = new Point(0, 0);
            FileNameLabel.Name = "FileNameLabel";
            FileNameLabel.Size = new Size(100, 23);
            FileNameLabel.TabIndex = 0;
            // 
            // SingleThreadDashboard
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(28, 28, 28);
            ClientSize = new Size(1262, 673);
            Controls.Add(ApplicationContainer);
            Name = "SingleThreadDashboard";
            Text = "SingleThreadDashboard";
            ApplicationContainer.ResumeLayout(false);
            ApplicationContainer.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private Panel ApplicationContainer;
        private TextBox URLTextBox;
        private Label URLLabel;
        private Panel DownloadPanel;
        private Label InstructionLabel;
        private Button DownloadButton;
        private Button SetLocationButton;
        private TextBox LocationInput;
        private Label FileNameLabel;
        private Label FileNameValue;
        private Button Cancel;
        private Button Pause;
        private Label SpeedValue;
        private Label SpeedLabel;
        private Label DownloadedValue;
        private Label DownloadedLabel;
        private ProgressBar ProgressBar;
        private Button Resume;
        private Panel DownloadsContainer;
        private int _currentPanelYPosition = 15;


        private void RearrangePanels()
        {
            int currentYPosition = 15; // Starting position for the first panel

            foreach (Control panel in DownloadsContainer.Controls)
            {
                // Set the new position for each panel
                panel.Location = new Point(panel.Location.X, currentYPosition);
                currentYPosition += panel.Height + 10; // Add spacing between panels
            }

            // Reset the current Y position tracker
            _currentPanelYPosition = currentYPosition;
        }

        private Downloadpanel CreateDownloadPanel(FileInfo file, CancellationTokenSource _cancellationTokenSource, string url, string destination, IProgress<DownloadProgress> progress)
        {
            Downloadpanel result = new Downloadpanel();
            DownloadPanel.Name = "DownloadPanel";
            DownloadPanel.TabIndex = 2;
            Panel downloadPanel = new Panel
            {
                Size = new Size(1154, 141),
                Location = new Point(12, _currentPanelYPosition),

            };

            Label FileNameValue = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = SystemColors.ButtonHighlight,
                Location = new Point(140, 13),
                Name = "FileNameValue",
                Size = new Size(39, 28),
                TabIndex = 8,
                Text = file.FileName
            };

            Label FileNameLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = SystemColors.ButtonHighlight,
                Location = new Point(18, 13),
                Name = "FileNameLabel",
                Size = new Size(113, 28),
                TabIndex = 7,
                Text = "File Name : "
            };

            ProgressBar ProgressBar = new ProgressBar
            {
                Location = new Point(18, 54),
                Name = "ProgressBar",
                Size = new Size(822, 29),
                TabIndex = 9,
                Maximum = 100
            };

            Label DownloadedLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = SystemColors.ButtonHighlight,
                Location = new Point(18, 97),
                Name = "DownloadedLabel",
                Size = new Size(133, 28),
                TabIndex = 10,
                Text = "Downloaded :"
            };

            Label DownloadedValue = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = SystemColors.ButtonHighlight,
                Location = new Point(182, 97),
                Name = "DownloadedValue",
                Size = new Size(42, 28),
                TabIndex = 11,
                Text = $"Mb"
            };

            Label SpeedLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = SystemColors.ButtonHighlight,
                Location = new Point(653, 97),
                Name = "SpeedLabel",
                Size = new Size(81, 28),
                TabIndex = 12,
                Text = "Speed : "
            };

            Label SpeedValue = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = SystemColors.ButtonHighlight,
                Location = new Point(740, 97),
                Name = "SpeedValue",
                Size = new Size(90, 28),
                TabIndex = 13,
                Text = "??? Mb/s"
            };

            Button ResumeButton = new Button
            {
                BackColor = Color.ForestGreen,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(846, 54),
                Name = "Resume",
                Size = new Size(94, 29),
                TabIndex = 16,
                Text = "Resume",
                UseVisualStyleBackColor = false
            };
            ResumeButton.Click += (sender, e) => ResumeButton_Click(sender, e, result);

            Button CancelButton = new Button
            {
                BackColor = Color.Maroon,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(1046, 54),
                Name = "Cancel",
                Size = new Size(94, 29),
                TabIndex = 15,
                Text = "Cancel",
                UseVisualStyleBackColor = false
            };
            CancelButton.Click += (sender, e) => CancelButton_Click(sender, e, result);

            Button PauseButton = new Button
            {
                BackColor = Color.Goldenrod,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(946, 54),
                Name = "Pause",
                Size = new Size(94, 29),
                TabIndex = 14,
                Text = "Pause",
                UseVisualStyleBackColor = false
            };

            PauseButton.Click += (sender, e) => PauseButton_Click(sender, e, result);

            downloadPanel.Controls.AddRange([
                FileNameValue,
                FileNameLabel,
                PauseButton,
                ResumeButton,
                CancelButton,
                SpeedLabel,
                SpeedValue,
                ProgressBar,
                DownloadedValue,
                DownloadedLabel
            ]);
            DownloadsContainer.Controls.Add(downloadPanel);

            
            result.downloadpanel = downloadPanel;
            result._cancellationTokenSource = _cancellationTokenSource;
            result.URL = url;
            result.Destination = destination;
            result.progress = progress;
            // 10 for spacing between panels
            _currentPanelYPosition += downloadPanel.Height + 10;
            return result;
        }
    }
}