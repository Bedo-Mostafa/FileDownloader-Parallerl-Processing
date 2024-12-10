using FileDownloader.ParallelProcessing.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FileDownloader.ParallelProcessing.Services
{
    class FileDownloadMultiThread
    {
        public async Task DownloadFile(string url, string destination, IProgress<DownloadProgress> progress)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    long totalBytes = 0;
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start(); // Start the stopwatch
                    long previousBytesReceived = 0;
                    DateTime previousTime = DateTime.Now;

                    webClient.DownloadProgressChanged += (s, e) =>
                    {
                        long currentBytesReceived = e.BytesReceived;
                        totalBytes = currentBytesReceived; // Update totalBytes

                        TimeSpan timeElapsed = DateTime.Now - previousTime;

                        if (timeElapsed.TotalSeconds > 0)
                        {
                            float speedInKbps = CalculateSpeed(currentBytesReceived, stopwatch); // Speed in KB/s
                            progress.Report(new DownloadProgress
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

                    await webClient.DownloadFileTaskAsync(new Uri(url), destination);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show("Access denied: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private float CalculateSpeed(long bytesReceived, Stopwatch stopwatch)
        {
            // Calculate speed in KB/s
            if (stopwatch.Elapsed.TotalSeconds > 0)
            {
                return (long)(bytesReceived / 1024 / stopwatch.Elapsed.TotalSeconds); // Convert to KB/s
            }
            return 0;
        }

    }
}
