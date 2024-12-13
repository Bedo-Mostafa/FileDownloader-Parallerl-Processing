using FileDownloader.ParallelProcessing.Models;
using FileDownloader.ParallelProcessing.Services;
using System.Text.RegularExpressions;

namespace FileDownloader.ParallelProcessing
{
    public partial class MultiThreadDashboard : Form
    {

        FileDownloadMultiThread downloader = new FileDownloadMultiThread();
        YoutubeDownloadMultiThread downloaderyoutube = new YoutubeDownloadMultiThread();
        private readonly int MAX_NUMBER_OF_DOWNLOADS;
        private readonly SemaphoreSlim _semaphore;
        CancellationTokenSource _cancellationTokenSource;

        public MultiThreadDashboard(int threadsNumbers)
        {
            InitializeComponent();
            this.MAX_NUMBER_OF_DOWNLOADS = threadsNumbers;
            _semaphore = new SemaphoreSlim(MAX_NUMBER_OF_DOWNLOADS);
        }

        private readonly Dictionary<string, CancellationTokenSource> _cancellationTokens = new();
        private async void DownloadButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LocationInput.Text))
            {
                MessageBox.Show("Please select a destination folder.");
                return;
            }

            string url = URLTextBox.Text.Trim();
            string fileName = Path.GetFileName(new Uri(URLTextBox.Text).LocalPath);
            string destination = Path.Combine(LocationInput.Text, fileName);
            Models.FileInfo file = new Models.FileInfo(fileName);

            // Check if the file already exists and generate a new file name with a postfix
            if (File.Exists(destination))
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                string fileExtension = Path.GetExtension(fileName);
                int counter = 1;

                // Generate a unique file name with a postfix (e.g., fileName(1), fileName(2), ...)
                while (File.Exists(destination))
                {
                    string newFileName = $"{fileNameWithoutExtension}({counter++}){fileExtension}";
                    destination = Path.Combine(LocationInput.Text, newFileName);
                }
            }


            // Use semaphore to limit concurrent downloads
            await _semaphore.WaitAsync();
            if (IsValidYouTubeUrl(url))
            {
                if (url.Contains("playlist", StringComparison.OrdinalIgnoreCase))
                {
                    var playlist = await downloaderyoutube.GetPlaylistVideos(url);
                    try
                    {
                        foreach (var video in playlist)
                        {
                            var DownloadContext = CreateDownloadContext(file, url, destination);

                            // Start the download
                            await Task.Run(async () =>
                            {
                                try
                                {
                                    downloaderyoutube.DownloadVideoAsync(video.Url, destination, DownloadContext.progress, DownloadContext._cancellationTokenSource).WaitAsync(DownloadContext._token);
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
                    }
                    finally
                    {
                        _semaphore.Release();
                    }

                }
                else
                {
                    var DownloadContext = CreateDownloadContext(file, url, destination);

                    try
                    {
                        // Start the download
                        await Task.Run(async () =>
                        {
                            try
                            {
                                await downloaderyoutube.DownloadVideoAsync(url, destination, DownloadContext.progress, DownloadContext._cancellationTokenSource);
                            }
                            catch (OperationCanceledException)
                            {
                                MessageBox.Show($"Download for {fileName} was canceled.");
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"An error occurred: {ex.Message}");
                            }
                        }).WaitAsync(DownloadContext._token);
                    }
                    finally
                    {
                        // Release semaphore after download is complete
                        _semaphore.Release();
                    }
                }
            }
            else
            {
                var DownloadContext = CreateDownloadContext(file, url, destination);

                try
                {
                    // Start the download
                    await Task.Run(async () =>
                    {
                        try
                        {
                            await downloader.DownloadFileAsync(url, destination, DownloadContext.progress, DownloadContext._token);
                        }
                        catch (OperationCanceledException)
                        {
                            MessageBox.Show($"Download for {fileName} was canceled.");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"An error occurred: {ex.Message}");
                        }
                    }).WaitAsync(DownloadContext._token);
                }
                finally
                {
                    // Release semaphore after download is complete
                    _semaphore.Release();
                }
            }
        }
        private bool IsValidYouTubeUrl(string url)
        {
            var youtubeUrlPattern = @"^(https?:\/\/)?(www\.)?(youtube\.com|youtu\.?be)\/.+$";
            return Regex.IsMatch(url, youtubeUrlPattern);
        }

        public (CancellationTokenSource _cancellationTokenSource, CancellationToken _token, IProgress<DownloadProgress> progress) CreateDownloadContext(Models.FileInfo file, string url, string destination)
        {
            var _cancellationTokenSource = new CancellationTokenSource();
            var _token = _cancellationTokenSource.Token;
            var progress = CreateProgressReporter(file, url, destination); // Assuming CreateProgressReporter is a valid method that returns an IProgress<DownloadProgress>

            return (_cancellationTokenSource, _token, progress); // Return a tuple containing the necessary values
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

    }
}
