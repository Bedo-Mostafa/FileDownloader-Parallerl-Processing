using FileDownloader.ParallelProcessing.Models;
using FileDownloader.ParallelProcessing.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FileDownloadMultiThread = FileDownloader.ParallelProcessing.Services.FileDownloadMultiThread;
using FileInfo = FileDownloader.ParallelProcessing.Models.FileInfo;

namespace FileDownloader.ParallelProcessing
{
    public partial class MultiThreadDashboard : Form
    {

        FileDownloadMultiThread downloader = new FileDownloadMultiThread();
        private readonly int MAX_NUMBER_OF_DOWNLOADS;
        private readonly SemaphoreSlim _semaphore;

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

            string fileName = Path.GetFileName(new Uri(URLTextBox.Text).LocalPath);
            string destination = Path.Combine(LocationInput.Text, fileName);
            FileInfo file = new FileInfo(fileName);

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

            // Create a new download panel
            var cts = new CancellationTokenSource();
            Panel downloadPanel = CreateDownloadPanel(file, cts);
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

            // Use semaphore to limit concurrent downloads
            await _semaphore.WaitAsync();
            try
            {
                // Start the download
                await Task.Run(async () =>
                {
                    try
                    {
                        await downloader.DownloadFileAsync(URLTextBox.Text, destination, progress, cts.Token);
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



        private void SetLocationButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                LocationInput.Text = folderBrowserDialog.SelectedPath;
            }

        }



        private void URLTextBox_TextChanged(object sender, EventArgs e)
        {

        }
            
        private void InstructionLabel_Click(object sender, EventArgs e)
        {

        }
    }
}
