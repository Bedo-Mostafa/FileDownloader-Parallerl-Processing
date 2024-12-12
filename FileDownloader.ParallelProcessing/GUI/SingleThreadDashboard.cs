using System.Text.RegularExpressions;
using FileDownloader.ParallelProcessing.Models;
using FileDownloader.ParallelProcessing.Services;

namespace FileDownloader.ParallelProcessing
{
    public partial class SingleThreadDashboard : Form
    {
        public SingleThreadDashboard()
        {
            InitializeComponent();
        }
        YoutubeDownloadSingleThread youtubeDownloadSingleThread = new YoutubeDownloadSingleThread();
        FileDownloadSingleThread fileDownloadSingleThread = new FileDownloadSingleThread();
        CancellationTokenSource _cancellationTokenSource;

        private async void DownloadButtonSingleThread(object sender, EventArgs e)
        {
            // Validation for browse
            if (string.IsNullOrWhiteSpace(LocationInput.Text))
            {
                MessageBox.Show("Please select a destination folder.");
                return;
            }

            string url = URLTextBox.Text.Trim();
            string fileName = Path.GetFileName(new Uri(URLTextBox.Text).LocalPath);
            string destination = Path.Combine(LocationInput.Text, fileName);
            Models.FileInfo file = new Models.FileInfo(fileName);

            // clear the text box
            URLTextBox.Clear();
            _cancellationTokenSource = new CancellationTokenSource();
            var _token = _cancellationTokenSource.Token;

            var progress = CreateProgressReporter(file, url, destination);
            // Validate YouTube URL format
            if (IsValidYouTubeUrl(url))
            {
                //await Task.Run(async () =>
                //{
                  await youtubeDownloadSingleThread.DownloadVideo(url,progress,_cancellationTokenSource);
                //});
            }
            else
            {
                try
                {
                    await fileDownloadSingleThread.DownloadFilesSequentiallyAsync(url, destination, progress, _cancellationTokenSource).WaitAsync(_cancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    MessageBox.Show("Download was Cancelled or Stopped.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private IProgress<DownloadProgress> CreateProgressReporter(Models.FileInfo file, string url, string destination)
        {
            // Create a new download panel
            Downloadpanel downloadPanel = CreateDownloadPanel(file, _cancellationTokenSource, url, destination, null);
            var fileNameLabel = (downloadPanel.downloadpanel.Controls["FileNameValue"] as Label);
            var progressBar = (downloadPanel.downloadpanel.Controls["ProgressBar"] as ProgressBar);
            var downloadedBytesLabel = (downloadPanel.downloadpanel.Controls["DownloadedValue"] as Label);
            var speedValue = (downloadPanel.downloadpanel.Controls["SpeedValue"] as Label);

            // Create a progress reporter
            var progress = new Progress<DownloadProgress>(p =>
            {
                // Update the progress bar and other UI elements
                progressBar.Value = p.Percentage;
                fileNameLabel.Text = file.FileName;
                downloadedBytesLabel.Text = $"{p.BytesReceived / (1024 * 1024)} MB / {p.TotalBytesToReceive / (1024 * 1024)} MB";
                speedValue.Text = $"{(p.Speed / 1024.0):F2} MB/s"; // Display speed in MB/s with 2 decimal places
            });
            downloadPanel.progress = progress;
            return progress;
        }
        private bool IsValidYouTubeUrl(string url)
        {
            var youtubeUrlPattern = @"^(https?:\/\/)?(www\.)?(youtube\.com|youtu\.?be)\/.+$";
            return Regex.IsMatch(url, youtubeUrlPattern);
        }
        private void SetLocationButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                LocationInput.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void CancelButton_Click(object sender, EventArgs e, Downloadpanel result)
        {
            Button? button = sender as Button;
            try
            {
                if (button != null && result._cancellationTokenSource.Token != null)
                {
                    result._cancellationTokenSource?.Cancel(); // Trigger cancellation
                    button.Enabled = false; // Disable the button
                    result._cancellationTokenSource?.Dispose();
                }
            }
            catch (ObjectDisposedException DE)
            {
                MessageBox.Show($"The Download was Cancelled", "cancellation", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            finally
            {
                result.downloadpanel.Dispose();
                RearrangePanels();
            }
        }

        private async void ResumeButton_Click(object sender, EventArgs e, Downloadpanel downloadpanel)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            downloadpanel._cancellationTokenSource = cts;

            await fileDownloadSingleThread.DownloadFilesSequentiallyAsync(downloadpanel.URL, downloadpanel.Destination, downloadpanel.progress, downloadpanel._cancellationTokenSource);
        }

        private void PauseButton_Click(object sender, EventArgs e, Downloadpanel downloadpanel)
        {
            Button? PauseButton = sender as Button;
            try
            {
                if (downloadpanel._cancellationTokenSource.Token != null && !downloadpanel._cancellationTokenSource.Token.IsCancellationRequested)
                {
                    downloadpanel._cancellationTokenSource.Cancel(); // Pause the download
                    downloadpanel._cancellationTokenSource.Dispose();
                }
            }
            catch (ObjectDisposedException DE)
            {
                MessageBox.Show($"An error occurred: The download is Already Cancelled", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
