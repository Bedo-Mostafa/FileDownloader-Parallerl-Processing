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
using System.Threading.Tasks;
using System.Windows.Forms;
using FileDownloadMultiThread = FileDownloader.ParallelProcessing.Services.FileDownloadMultiThread;
using FileInfo = FileDownloader.ParallelProcessing.Models.FileInfo;

namespace FileDownloader.ParallelProcessing
{
    public partial class MultiThreadDashboard : Form
    {
        private HashSet<string> urls = new HashSet<string>();
        private SemaphoreSlim semaphore;
        FileDownloadMultiThread downloader = new FileDownloadMultiThread();
        public MultiThreadDashboard()
        {
            InitializeComponent();
        }

        //private async void DownloadButton_Click(object sender, EventArgs e)
        //{
        //    if (string.IsNullOrWhiteSpace(LocationInput.Text))
        //    {
        //        MessageBox.Show("Please select a destination folder.");
        //        return;
        //    }
        //    //if (urls.Count == 0)
        //    //{
        //    //    MessageBox.Show("No URLs provided. Please enter valid URLs.");
        //    //    return;
        //    //}
        //    else
        //    {
        //        string fileName = Path.GetFileName(new Uri(URLTextBox.Text).LocalPath);
        //        string destination = Path.Combine(LocationInput.Text, fileName);
        //        FileInfo file = new FileInfo(fileName);
        //        Panel Instance = CreateDownloadPanel(file);
        //        await downloader.DownloadFile(URLTextBox.Text, destination);


        //    }


        //}

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

            // Create a new download panel
            Panel downloadPanel = CreateDownloadPanel(file);
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
                speedValue.Text = $"{(p.Speed / 1024.0):F2} MB/s"; // Display speed in KB/s with 2 decimal places
            });

            // Start the download
            await downloader.DownloadFile(URLTextBox.Text, destination, progress);
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
