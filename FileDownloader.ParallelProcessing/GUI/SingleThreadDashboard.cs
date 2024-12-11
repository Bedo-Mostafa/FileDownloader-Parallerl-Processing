using FileInfo = FileDownloader.ParallelProcessing.Models.FileInfo;
using FileDownloadSingelThread = FileDownloader.ParallelProcessing.Services.FileDownloadSingleThread;
using FileDownloader.ParallelProcessing.Models;

namespace FileDownloader.ParallelProcessing
{
    public partial class SingleThreadDashboard : Form
    {
        public SingleThreadDashboard()
        {
            InitializeComponent();
        }

        FileDownloadSingelThread fileDownloadSingleThread = new FileDownloadSingelThread();
        FileInfo information = new FileInfo();
        CancellationTokenSource _cancellationTokenSource;

        private void button1_Click(object sender, EventArgs e)
        {
            //CreateDownloadPanel(information);
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {
            //CreateDownloadPanel(information);

        }

        private void label2_Click(object sender, EventArgs e)
        {
            //CreateDownloadPanel(information);

        }

        /*private async void DownloadButtonSingleThread(object sender, EventArgs e)
        {
            // clear the inputs
            //URLTextBox.Clear();
            //LocationInput.Clear();

            if (string.IsNullOrWhiteSpace(LocationInput.Text))
            {
                MessageBox.Show("Please select a destination folder.");
                return;
            }

            string url = URLTextBox.Text.Trim();
            string fileName = Path.GetFileName(new Uri(URLTextBox.Text).LocalPath);
            string destination = Path.Combine(LocationInput.Text, fileName);
            FileInfo file = new FileInfo(fileName);

            _cancellationTokenSource = new CancellationTokenSource(); // Reset the token source
            var _token = _cancellationTokenSource.Token;

            // Create a new download panel
            Panel downloadPanel = CreateDownloadPanel(file, _cancellationTokenSource);
            var fileNameLabel = (downloadPanel.Controls["FileNameValue"] as Label);
            var progressBar = (downloadPanel.Controls["ProgressBar"] as ProgressBar);
            var downloadedBytesLabel = (downloadPanel.Controls["DownloadedValue"] as Label);
            var speedValue = (downloadPanel.Controls["SpeedValue"] as Label);
            var cancelButton = (downloadPanel.Controls["CancelButton"] as Button);


            // Create a progress reporter
            var progress = new Progress<DownloadProgress>(p =>
            {
                // Update the progress bar and other UI elements
                progressBar.Value = p.Percentage;
                fileNameLabel.Text = file.FileName;
                downloadedBytesLabel.Text = $"{p.BytesReceived / (1024 * 1024)} MB / {p.TotalBytesToReceive / (1024 * 1024)} MB";
                speedValue.Text = $"{(p.Speed / 1024.0):F2} MB/s"; // Display speed in MB/s with 2 decimal places
            });

            try
            {
                await Task.Run(() =>
                {
                    fileDownloadSingleThread.DownloadFilesSequentiallyAsync(url, destination, progress, _token);
                });
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Download was canceled.", "Canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }*/


        private async void DownloadButtonSingleThread(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LocationInput.Text))
            {
                MessageBox.Show("Please select a destination folder.");
                return;
            }

            string url = URLTextBox.Text.Trim();
            string fileName = Path.GetFileName(new Uri(URLTextBox.Text).LocalPath);
            string destination = Path.Combine(LocationInput.Text, fileName);
            FileInfo file = new FileInfo(fileName);

            _cancellationTokenSource = new CancellationTokenSource(); // Reset the token source
            var _token = _cancellationTokenSource.Token;

            // Create a new download panel
            Panel downloadPanel = CreateDownloadPanel(file, _cancellationTokenSource);
            var fileNameLabel = (downloadPanel.Controls["FileNameValue"] as Label);
            var progressBar = (downloadPanel.Controls["ProgressBar"] as ProgressBar);
            var downloadedBytesLabel = (downloadPanel.Controls["DownloadedValue"] as Label);
            var speedValue = (downloadPanel.Controls["SpeedValue"] as Label);

            // Create a progress reporter
            var progress = new Progress<DownloadProgress>(p =>
            {
                // Update the progress bar and other UI elements
                progressBar.Value = p.Percentage;
                fileNameLabel.Text = file.FileName;
                downloadedBytesLabel.Text = $"{p.BytesReceived / (1024 * 1024)} MB / {p.TotalBytesToReceive / (1024 * 1024)} MB";
                speedValue.Text = $"{(p.Speed / 1024.0):F2} MB/s"; // Display speed in MB/s with 2 decimal places
            });

            try
            {
                await fileDownloadSingleThread.DownloadFilesSequentiallyAsync(url, destination, progress, _token);
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Download was canceled.", "Canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void CancelButton_Click(object sender, EventArgs e, CancellationTokenSource cancellationTokenSource)
        {
            cancellationTokenSource.Cancel(); // Trigger cancellation
            Button? button = sender as Button;
            if (button != null)
            {
                button.Enabled = false; // Disable the button
                cancellationTokenSource.Dispose();
            }
        }

        private void SetLocationButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                LocationInput.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void ResumeButton_Click(object sender, EventArgs e, CancellationTokenSource cancellationTokenSource)
        {
            //CreateDownloadPanel(information);
            //if (isPaused)
            //{
            //isPaused = false;
            //PauseButton.Enabled = true;
            //ResumeButton.Enabled = false;

            // Restart download
            DownloadButtonSingleThread(sender, e);
            //}

        }

        private void PauseButton_Click(object sender, EventArgs e, CancellationTokenSource cancellationTokenSource)
        {
            Button? PauseButton = sender as Button;
            if (_cancellationTokenSource != null && !_cancellationTokenSource.Token.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel(); // Pause the download
                //downloadpanel.isPaused = true;
                //ResumeButton.Enabled = true;
                //PauseButton.Enabled = false;
            }

        }

        private void label6_Click(object sender, EventArgs e)
        {
            //CreateDownloadPanel(information);

        }

        private void label8_Click(object sender, EventArgs e)
        {
            //CreateDownloadPanel(information);

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //CreateDownloadPanel(information);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //CreateDownloadPanel(information);

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void URLInputLabel(object sender, EventArgs e)
        {
            // Get the text from the TextBox
            //string url = txtUrl.Text.Trim(); // Trim to remove leading/trailing whitespace
            //string url = URLTextBox.Text.Trim();
        }

        private void Resume_Click(object sender, EventArgs e)
        {

        }

        private void MultiThreadDashboard_Load(object sender, EventArgs e)
        {

        }

        private void URLLabel_Click(object sender, EventArgs e)
        {

        }

        private void DownloadPath_Text(object sender, EventArgs e)
        {

        }
    }
}
