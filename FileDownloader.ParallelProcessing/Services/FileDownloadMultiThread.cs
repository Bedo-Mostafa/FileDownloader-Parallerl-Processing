using System.Diagnostics;
using FileDownloader.ParallelProcessing.Models;

namespace FileDownloader.ParallelProcessing.Services
{
    class FileDownloadMultiThread
    {
        public async Task DownloadFileAsync(string url, string destination, IProgress<DownloadProgress> progress, CancellationToken cancellationToken)
        {
            try
            {
                long totalBytesToReceive = 0;
                long totalBytesDownloaded = 0;

                if (File.Exists(destination))
                {
                    totalBytesDownloaded = new System.IO.FileInfo(destination).Length; // Get the size of the existing file
                }

                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromMinutes(30); // Set a timeout value

                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
                    if (totalBytesDownloaded > 0)
                    {
                        request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(totalBytesDownloaded, null);
                    }

                    using (HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
                    {
                        response.EnsureSuccessStatusCode(); // Throw an exception if the HTTP response is an error

                        if (response.Content.Headers.ContentLength.HasValue)
                        {
                            totalBytesToReceive = response.Content.Headers.ContentLength.Value + totalBytesDownloaded;
                        }

                        using (Stream contentStream = await response.Content.ReadAsStreamAsync(cancellationToken),
                            fileStream = new FileStream(destination, FileMode.Append, FileAccess.Write, FileShare.None, 8192, true))
                        {
                            byte[] buffer = new byte[8192];
                            Stopwatch stopwatch = new Stopwatch();
                            stopwatch.Start();

                            int bytesRead;

                            while ((bytesRead = await contentStream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken)) > 0)
                            {
                                await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
                                totalBytesDownloaded += bytesRead;

                                // Calculate elapsed time since download started
                                double secondsElapsed = stopwatch.Elapsed.TotalSeconds;
                                double speedInKbps = secondsElapsed > 0 ? totalBytesDownloaded / 1024.0 / secondsElapsed : 0;

                                progress?.Report(new DownloadProgress
                                {
                                    Percentage = totalBytesToReceive > 0 ? (int)(totalBytesDownloaded * 100 / totalBytesToReceive) : 0,
                                    BytesReceived = totalBytesDownloaded,
                                    TotalBytesToReceive = totalBytesToReceive,
                                    Speed = speedInKbps, // Speed in KB/s
                                });
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}:", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
            }
        }
    }
}
