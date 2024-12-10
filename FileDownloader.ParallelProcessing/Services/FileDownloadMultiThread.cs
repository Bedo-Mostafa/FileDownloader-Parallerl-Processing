using FileDownloader.ParallelProcessing.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FileDownloader.ParallelProcessing.Services
{
    class FileDownloadMultiThread
    {
        public async Task DownloadFileAsync(string url, string destination, IProgress<DownloadProgress> progress, CancellationToken cancellationToken)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                    response.EnsureSuccessStatusCode();

                    long totalBytes = response.Content.Headers.ContentLength ?? 0;
                    long bytesDownloaded = 0;

                    using (var stream = await response.Content.ReadAsStreamAsync())
                    using (var fileStream = new FileStream(destination, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                    {
                        byte[] buffer = new byte[8192];
                        Stopwatch stopwatch = Stopwatch.StartNew();

                        int bytesRead;
                        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                            bytesDownloaded += bytesRead;

                            double speed = bytesDownloaded / stopwatch.Elapsed.TotalSeconds / 1024; // KB/s
                            int percentage = totalBytes > 0 ? (int)((bytesDownloaded * 100L) / totalBytes) : 0;

                            progress.Report(new DownloadProgress
                            {
                                Percentage = percentage,
                                BytesReceived = bytesDownloaded,
                                TotalBytesToReceive = totalBytes,
                                Speed = speed,
                            });
                        }

                        stopwatch.Stop();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Download was canceled.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
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
