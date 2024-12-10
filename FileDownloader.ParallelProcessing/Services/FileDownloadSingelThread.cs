using FileDownloader.ParallelProcessing.Models;
using Microsoft.VisualBasic.Devices;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FileDownloader.ParallelProcessing.Services
{
    public class FileDownloadSingelThread
    {
        private readonly object _lock = new object();

        public void DownloadFilesSequentiallyAsync(string url, string destinationFolder, IProgress<DownloadProgress> progress, CancellationToken cancellationToken)
        {
            lock (_lock) // Ensure only one thread can download to the same file at a time
            {
                string fileName = Path.GetFileName(new Uri(url).LocalPath);

                long totalBytes = 0;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start(); // Start the stopwatch
                long previousBytesReceived = 0;
                DateTime previousTime = DateTime.Now;

                try
                {
                    using (WebClient webClient = new WebClient())
                    {
                        webClient.DownloadProgressChanged += (s, e) =>
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                webClient.CancelAsync();
                                return;
                            }

                            long currentBytesReceived = e.BytesReceived;
                            totalBytes = currentBytesReceived; // Update totalBytes

                            TimeSpan timeElapsed = DateTime.Now - previousTime;

                            if (timeElapsed.TotalSeconds > 0)
                            {
                                float speedInKbps = CalculateSpeed(currentBytesReceived, stopwatch); // Speed in KB/s
                                progress?.Report(new DownloadProgress
                                {
                                    Percentage = e.ProgressPercentage,
                                    BytesReceived = e.BytesReceived,
                                    TotalBytesToReceive = e.TotalBytesToReceive,
                                    Speed = speedInKbps, // Update speed in KB/s
                                });

                                previousBytesReceived = currentBytesReceived;
                                previousTime = DateTime.Now; // Reset the timer
                            }
                        };

                        //cancellationToken.ThrowIfCancellationRequested();

                        webClient.DownloadFileTaskAsync(new Uri(url), destinationFolder).Wait(cancellationToken); // Pass the token
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Download for task  canceled.", "Canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    //MessageBox.Show($"{e.Message}");
                    //throw;
                }
            }
        }

        private float CalculateSpeed(long bytesReceived, Stopwatch stopwatch)
        {
            // Calculate speed in KB/s
            if (stopwatch.Elapsed.TotalSeconds > 0)
            {
                return (float)(bytesReceived / 1024 / stopwatch.Elapsed.TotalSeconds); // Convert to KB/s
            }
            return 0;
        }

        public string GetInfo(string url)
        {
            string fileName;
            using (WebClient webClient = new WebClient())
            {
                Uri uri = new Uri(url);
                // Get the file name from the URL
                fileName = Path.GetFileName(uri.LocalPath);
            }
            return fileName;
        }
    }
}


//using FileDownloader.ParallelProcessing.Models;
//using System;
//using System.Diagnostics;
//using System.IO;
//using System.Net;
//using System.Threading;
//using System.Threading.Tasks;

//namespace FileDownloader.ParallelProcessing.Services
//{
//    public class FileDownloadSingelThread
//    {
//        private readonly object _lock = new object();

//        public void DownloadFilesSequentiallyAsync(string url, string destinationFolder, IProgress<DownloadProgress> progress, CancellationToken cancellationToken)
//        {
//            string fileName = Path.GetFileName(new Uri(url).LocalPath);
//            //string destinationPath = Path.Combine(destinationFolder, fileName);

//            lock (_lock) // Ensure only one thread accesses this block
//            {
//                long totalBytes = 0;
//                Stopwatch stopwatch = new Stopwatch();
//                stopwatch.Start();

//                long previousBytesReceived = 0;
//                DateTime previousTime = DateTime.Now;

//                using (WebClient webClient = new WebClient())
//                {
//                    // Ensure download can be canceled
//                    webClient.CancelAsync();

//                    try
//                    {
//                        webClient.DownloadProgressChanged += (s, e) =>
//                        {


//                            long currentBytesReceived = e.BytesReceived;
//                            totalBytes = currentBytesReceived;

//                            TimeSpan timeElapsed = DateTime.Now - previousTime;

//                            if (timeElapsed.TotalSeconds > 0)
//                            {
//                                float speedInKbps = CalculateSpeed(currentBytesReceived, stopwatch);
//                                progress?.Report(new DownloadProgress
//                                {
//                                    Percentage = e.ProgressPercentage,
//                                    BytesReceived = e.BytesReceived,
//                                    TotalBytesToReceive = e.TotalBytesToReceive,
//                                    Speed = speedInKbps,
//                                });

//                                previousBytesReceived = currentBytesReceived;
//                                previousTime = DateTime.Now;
//                            }

//                            if (cancellationToken.IsCancellationRequested)
//                            {
//                                webClient.CancelAsync(); // Cancel the download
//                                return;
//                                //cancellationToken.ThrowIfCancellationRequested();
//                            }
//                        };
//                        webClient.DownloadFileTaskAsync(new Uri(url), destinationFolder).Wait(cancellationToken);
//                    }
//                    catch (OperationCanceledException)
//                    {
//                        MessageBox.Show($"Download for task  canceled.", "Canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
//                        //throw;
//                    }
//                    finally
//                    {
//                        stopwatch.Stop();
//                    }
//                }
//            }
//        }



//        private float CalculateSpeed(long bytesReceived, Stopwatch stopwatch)
//        {
//            // Calculate speed in KB/s
//            if (stopwatch.Elapsed.TotalSeconds > 0)
//            {
//                return (float)(bytesReceived / 1024 / stopwatch.Elapsed.TotalSeconds); // Convert to KB/s
//            }
//            return 0;
//        }

//        public string GetInfo(string url)
//        {
//            string fileName;
//            using (WebClient webClient = new WebClient())
//            {
//                Uri uri = new Uri(url);
//                // Get the file name from the URL
//                fileName = Path.GetFileName(uri.LocalPath);
//            }
//            return fileName;
//        }
//    }
//}

