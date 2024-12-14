using FileDownloader.ParallelProcessing.Models;
using FileDownloader.ParallelProcessing.Services;
using System.Text.RegularExpressions;
using YoutubeExplode;

namespace FileDownloader.ParallelProcessing
{
    public partial class MultiThreadDashboard : Form
    {

        FileDownloadMultiThread downloader = new FileDownloadMultiThread();
        YoutubeDownloadMultiThread downloaderYoutube = new YoutubeDownloadMultiThread();
        private readonly SemaphoreSlim _semaphore;
        CancellationTokenSource _cancellationTokenSource;
        private static readonly object _fileNameLock = new object();
        public MultiThreadDashboard(int threadsNumbers)
        {
            InitializeComponent();
            _semaphore = new SemaphoreSlim(threadsNumbers);
        }

        private async void DownloadButton_Click(object sender, EventArgs e)
        {
            if (IsEmptyInput(LocationInput.Text, URLTextBox.Text)) { return; }

            string url = URLTextBox.Text.Trim();
            string fileName = Path.GetFileName(new Uri(URLTextBox.Text).LocalPath);
            string destination = Path.Combine(LocationInput.Text);


            string fileDownload = Path.Combine(LocationInput.Text, fileName);

            fileDownload = CheckfileNamePostFix(fileName, destination, fileDownload);

            // clear the text box
            URLTextBox.Clear();


            if (IsValidYouTubeUrl(url))
            {
                if (url.Contains("playlist", StringComparison.OrdinalIgnoreCase))
                {
                    await DownloadyoutubePlaylist(url, fileName, destination);
                }
                else
                {
                    await DownloadyoutubeVideo(url, fileName, destination);
                }
            }
            else
            {
                await DownloadFile(url, fileName, destination, fileDownload);
            }
        }

        private async Task DownloadFile(string url, string fileName, string destination, string fileDownload)
        {
            //destination = Path.Combine(LocationInput.Text, fileName);

            var DownloadContext = CreateDownloadContext(fileName, url, fileDownload);
            //DownloadContext.
            try
            {
                // Use semaphore to limit concurrent downloads
                await _semaphore.WaitAsync();
                // Start the download
                try
                {
                    await Task.Run(async () =>
                    {
                        await downloader.DownloadFileAsync(url, fileDownload, DownloadContext.downloadpanel.progress, DownloadContext._token).WaitAsync(DownloadContext._token);
                    });
                }
                catch (OperationCanceledException)
                {
                    MessageBox.Show($"Download for {fileName} was canceled.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
            finally
            {
                // Release semaphore after download is complete
                _semaphore.Release();
            }
        }

        private async Task DownloadyoutubeVideo(string url, string fileName, string destination)
        {
            var DownloadContext = CreateDownloadContext(fileName, url, destination);

            try
            {
                // Use semaphore to limit concurrent downloads
                await _semaphore.WaitAsync();
                // Start the download
                await Task.Run(async () =>
                {
                    try
                    {
                        await downloaderYoutube.DownloadVideoAsync(url, destination, DownloadContext.downloadpanel.progress, DownloadContext._cancellationTokenSource);
                    }
                    catch (OperationCanceledException)
                    {
                        MessageBox.Show($"Download for {fileName} was canceled.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}");
                    }
                });
            }
            finally
            {
                // Release semaphore after download is complete
                _semaphore.Release();
            }
        }

        private async Task DownloadyoutubePlaylist(string url, string fileName, string destination)
        {
            var playlist = await downloaderYoutube.GetPlaylistVideos(url);

            // Create a list to store file.FileName and DownloadContext
            var downloadList = new List<DownloadItem>();

            foreach (var video in playlist)
            {
                //file.FileName = ;
                var DownloadContext = CreateDownloadContext(fileName, url, destination);
                // Add a new DownloadItem to the list
                downloadList.Add(new DownloadItem
                {
                    FileName = video.Title,
                    VideoUrl = video.Url,
                    CancellationTokenSource = DownloadContext._cancellationTokenSource,
                    Token = DownloadContext._token,
                    Progress = DownloadContext.downloadpanel.progress
                });
            }

            for (int i = 0; i < downloadList.Count; i++)
            {
                // Capture the current index and item to avoid closure issues in the async block
                var downloadItem = downloadList[i];

                await _semaphore.WaitAsync();
                // Start the download
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await downloaderYoutube
                        .DownloadVideoAsync(downloadItem.VideoUrl, destination, downloadItem.Progress, downloadItem.CancellationTokenSource)
                        .WaitAsync(downloadItem.CancellationTokenSource.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        MessageBox.Show($"Download for {fileName} was canceled.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}");
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                });
            }
        }

        private string CheckfileNamePostFix(string fileName, string destination, string fileDownload)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string fileExtension = Path.GetExtension(fileName);

            // Generate a unique code using the current timestamp
            string uniqueCode = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"); // Format ensures high precision
            string newFileName = $"{fileNameWithoutExtension}_{uniqueCode}{fileExtension}";
            string newFileDownload = Path.Combine(destination, newFileName);

            // Ensure the file name is unique
            while (File.Exists(newFileDownload))
            {
                uniqueCode = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"); // Regenerate timestamp if necessary
                newFileName = $"{fileNameWithoutExtension}_{uniqueCode}{fileExtension}";
                newFileDownload = Path.Combine(destination, newFileName);
            }
            return newFileDownload;
        }

        private bool IsEmptyInput(string location, string url)
        {
            if (string.IsNullOrWhiteSpace(location) || string.IsNullOrWhiteSpace(url))
            {
                MessageBox.Show("Please select a destination folder or enter valid url.");
                return true;
            }
            return false;
        }

        private bool IsValidYouTubeUrl(string url)
        {
            var youtubeUrlPattern = @"^(https?:\/\/)?(www\.)?(youtube\.com|youtu\.?be)\/.+$";
            return Regex.IsMatch(url, youtubeUrlPattern);
        }

        public (CancellationTokenSource _cancellationTokenSource, CancellationToken _token, Downloadpanel downloadpanel) CreateDownloadContext(string fileName, string url, string destination)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var _token = _cancellationTokenSource.Token;
            var progress = CreateProgressReporter(fileName, url, destination); // Assuming CreateProgressReporter is a valid method that returns an IProgress<DownloadProgress>

            return (_cancellationTokenSource, _token, progress); // Return a tuple containing the necessary values
        }

        private Downloadpanel CreateProgressReporter(string fileName, string url, string destination)
        {
            // Create a new download panel
            Downloadpanel downloadPanel = CreateDownloadPanel(fileName, _cancellationTokenSource, url, destination, null);
            var fileNameLabel = (downloadPanel.downloadpanel.Controls["FileNameValue"] as Label);
            var progressBar = (downloadPanel.downloadpanel.Controls["ProgressBar"] as ProgressBar);
            var downloadedBytesLabel = (downloadPanel.downloadpanel.Controls["DownloadedValue"] as Label);
            var speedValue = (downloadPanel.downloadpanel.Controls["SpeedValue"] as Label);
            string title = "";
            if (IsValidYouTubeUrl(url) && !url.Contains("playlist", StringComparison.OrdinalIgnoreCase))
            {
                Task.Run(() =>
                {
                    title = SetVideoTitleAsync(url, downloadPanel).Result;
                });
            }
            // Create a progress reporter
            var progress = new Progress<DownloadProgress>(p =>
            {
                // Update the progress bar and other UI elements
                progressBar.Value = p.Percentage;
                fileNameLabel.Text = title == "" ? fileName : title;
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
            downloadPanel.Destination = destination;

            return downloadPanel;
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

            // Start the download
            await Task.Run(async () =>
            {
                try
                {
                    await downloader.DownloadFileAsync(downloadpanel.URL, downloadpanel.Destination, downloadpanel.progress, cts.Token);
                }
                catch (OperationCanceledException)
                {
                    MessageBox.Show($"Download was canceled.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            });
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

        public async Task<string> SetVideoTitleAsync(string url, Downloadpanel downloadPanel)
        {
            // Initialize the YoutubeClient
            var youtubeClient = new YoutubeClient();

            // Get video details asynchronously
            var video = await youtubeClient.Videos.GetAsync(url);

            // Retrieve the video title
            string videoTitle = video.Title;

            return videoTitle;
        }
    }
}
