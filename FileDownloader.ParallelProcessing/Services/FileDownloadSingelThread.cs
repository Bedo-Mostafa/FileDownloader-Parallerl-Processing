using Microsoft.VisualBasic.Devices;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace FileDownloader.ParallelProcessing.Services
{
    public class FileDownloadSingelThread
    {
        private readonly object _lock = new object();

        private void DownloadFile(string url, string destination, IProgress<(int Progress, long Speed)> progress)
        {
            lock (_lock) // Ensure only one thread can download to the same file at a time
            {
                long totalBytes = 0;
                Stopwatch stopwatch = new Stopwatch();

                try
                {
                    using (WebClient webClient = new WebClient())
                    {
                        webClient.DownloadProgressChanged += (s, e) =>
                        {
                            //progress?.Report(e.ProgressPercentage);
                            totalBytes = e.BytesReceived;
                            stopwatch.Start(); // Start timing
                            progress?.Report((e.ProgressPercentage, CalculateSpeed(totalBytes, stopwatch)));
                        };

                        webClient.DownloadFileTaskAsync(new Uri(url), destination).Wait();
                    }
                }
                catch (IOException ex)
                {
                    Debug.WriteLine($"File conflict: {ex.Message}");
                    throw; // Re-throw to propagate to the caller
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to download {url}: {ex.Message}");
                    throw;
                }
            }
        }

        private long CalculateSpeed(long bytesReceived, Stopwatch stopwatch)
        {
            // Calculate speed in bytes per second
            if (stopwatch.Elapsed.TotalSeconds > 0)
            {
                return (long)(bytesReceived / stopwatch.Elapsed.TotalSeconds);
            }
            return 0;
        }

        public void DownloadFilesSequentially(string url, string destinationFolder, IProgress<(int Progress, long Speed)> progress)
        {
            try
            {
                string fileName = Path.GetFileName(new Uri(url).LocalPath);
                string destination = Path.Combine(destinationFolder, fileName);


                Task.Run(() => { DownloadFile(url, destination, progress); }).Wait();
            }
            catch (Exception)
            {
                // Handle exceptions if necessary
            }
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
