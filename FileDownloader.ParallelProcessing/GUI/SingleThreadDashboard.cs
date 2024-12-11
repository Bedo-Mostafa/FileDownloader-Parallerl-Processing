using FileInfo = FileDownloader.ParallelProcessing.Models.FileInfo;
using FileDownloadSingelThread = FileDownloader.ParallelProcessing.Services.FileDownloadSingleThread;
using FileDownloader.ParallelProcessing.Models;
using System;
using System.Diagnostics;

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

            URLTextBox.Clear();

            _cancellationTokenSource = new CancellationTokenSource(); // Reset the token source
            var _token = _cancellationTokenSource.Token;

            var progress = new Progress<DownloadProgress>();
            // Create a new download panel
            Downloadpanel downloadPanel = CreateDownloadPanel(file, _cancellationTokenSource, url, destination, progress);
            var fileNameLabel = (downloadPanel.dwonloadpanel.Controls["FileNameValue"] as Label);
            var progressBar = (downloadPanel.dwonloadpanel.Controls["ProgressBar"] as ProgressBar);
            var downloadedBytesLabel = (downloadPanel.dwonloadpanel.Controls["DownloadedValue"] as Label);
            var speedValue = (downloadPanel.dwonloadpanel.Controls["SpeedValue"] as Label);

            // Create a progress reporter
            progress = new Progress<DownloadProgress>(p =>
            {
                // Update the progress bar and other UI elements
                progressBar.Value = p.Percentage;
                fileNameLabel.Text = file.FileName;
                downloadedBytesLabel.Text = $"{p.BytesReceived / (1024 * 1024)} MB / {p.TotalBytesToReceive / (1024 * 1024)} MB";
                speedValue.Text = $"{(p.Speed / 1024.0):F2} MB/s"; // Display speed in MB/s with 2 decimal places
            });

            try
            {
                await fileDownloadSingleThread.DownloadFilesSequentiallyAsync(url, destination, progress, _cancellationTokenSource).WaitAsync(_cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Download was canceled or Stooped.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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


        private async void ResumeButton_Click(object sender, EventArgs e, Downloadpanel downloadpanel)
        {
            string fileName = Path.GetFileName(new Uri(downloadpanel.URL).LocalPath);
            FileInfo file = new FileInfo(fileName);

            var fileNameLabel = (downloadpanel.dwonloadpanel.Controls["FileNameValue"] as Label);
            var progressBar = (downloadpanel.dwonloadpanel.Controls["ProgressBar"] as ProgressBar);
            var downloadedBytesLabel = (downloadpanel.dwonloadpanel.Controls["DownloadedValue"] as Label);
            var speedValue = (downloadpanel.dwonloadpanel.Controls["SpeedValue"] as Label);

            // Create a progress reporter
            var progress = new Progress<DownloadProgress>(p =>
            {
                // Update the progress bar and other UI elements
                progressBar.Value = p.Percentage;
                fileNameLabel.Text = file.FileName;
                downloadedBytesLabel.Text = $"{p.BytesReceived / (1024 * 1024)} MB / {p.TotalBytesToReceive / (1024 * 1024)} MB";
                speedValue.Text = $"{(p.Speed / 1024.0):F2} MB/s"; // Display speed in MB/s with 2 decimal places
            });



            CancellationTokenSource cts = new CancellationTokenSource();
            downloadpanel._cancellationTokenSource = cts;


            await fileDownloadSingleThread.DownloadFilesSequentiallyAsync(downloadpanel.URL, downloadpanel.Destination, progress, downloadpanel._cancellationTokenSource);

        }

        private void PauseButton_Click(object sender, EventArgs e, Downloadpanel downloadpanel)
        {
            Button? PauseButton = sender as Button;
            if (downloadpanel._cancellationTokenSource != null && !downloadpanel._cancellationTokenSource.Token.IsCancellationRequested)
            {
                downloadpanel._cancellationTokenSource.Cancel(); // Pause the download
                downloadpanel._cancellationTokenSource.Dispose();
            }

        }
    }
}
