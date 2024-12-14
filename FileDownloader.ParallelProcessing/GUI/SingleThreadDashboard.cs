using System.Text.RegularExpressions;
using FileDownloader.ParallelProcessing.Models;
using FileDownloader.ParallelProcessing.Services;
using YoutubeExplode;

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
            if (string.IsNullOrWhiteSpace(LocationInput.Text) || string.IsNullOrWhiteSpace(URLTextBox.Text))
            {
                MessageBox.Show("Please select a destination folder or enter valid url.");
                return;
            }

            string url = URLTextBox.Text.Trim();
            string fileName = Path.GetFileName(new Uri(URLTextBox.Text).LocalPath);
            string destination = Path.Combine(LocationInput.Text,fileName);
            Models.DownloadItem file = new Models.DownloadItem(fileName);

            // clear the text box
            URLTextBox.Clear();
            _cancellationTokenSource = new CancellationTokenSource();
            var _token = _cancellationTokenSource.Token;

            var downloadPanel = CreateProgressReporter(file, url, destination);
            // Validate YouTube URL format
            if (IsValidYouTubeUrl(url))
            {
                try
                {
                    if (url.Contains("playlist", StringComparison.OrdinalIgnoreCase))
                    {
                        YoutubeClient youtube = new YoutubeClient();

                        var playlist = await youtube.Playlists.GetAsync(url);
                        file.FileName = playlist.Title;
                         _ = SetVideoTitleAsync(url, downloadPanel, file);
                         await youtubeDownloadSingleThread.DownloadPlaylist(url, LocationInput.Text, downloadPanel.progress, _cancellationTokenSource).WaitAsync(_cancellationTokenSource.Token);
                    }
                    else
                    {
                        _ = SetVideoTitleAsync(url, downloadPanel,file);
                         await youtubeDownloadSingleThread.DownloadVideo(url, LocationInput.Text, downloadPanel.progress, _cancellationTokenSource).WaitAsync(_cancellationTokenSource.Token);

                    }
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
            else
            {
                //destination = Path.Combine(LocationInput.Text,fileName);
                try
                {
                    await fileDownloadSingleThread.DownloadFilesSequentiallyAsync(url, destination, downloadPanel.progress, _cancellationTokenSource).WaitAsync(_cancellationTokenSource.Token);
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
        private Downloadpanel CreateProgressReporter(Models.DownloadItem file, string url, string destination)
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
            // Disable Pause and resume buttons for youtubeVideos Download
            if (IsValidYouTubeUrl(url))
            {
                downloadPanel.downloadpanel.Controls["Pause"].Enabled = false;
                downloadPanel.downloadpanel.Controls["Resume"].Enabled = false;
            }
            downloadPanel.progress = progress;
            return downloadPanel;
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
            await fileDownloadSingleThread.DownloadFilesSequentiallyAsync(downloadpanel.URL, downloadpanel.Destination, downloadpanel.progress, downloadpanel._cancellationTokenSource).WaitAsync(downloadpanel._cancellationTokenSource.Token);
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

        public async Task SetVideoTitleAsync(string url, Downloadpanel downloadPanel,Models.DownloadItem file)
        {
            // Initialize the YoutubeClient
            var youtubeClient = new YoutubeClient();

            // Get video details asynchronously
            var video = await youtubeClient.Videos.GetAsync(url);

            // Retrieve the video title
            string videoTitle = video.Title;

            // Update the UI with the video title
            downloadPanel.downloadpanel.Controls["FileNameValue"].Text = videoTitle;

            file.FileName = videoTitle;

        }

    }
}
