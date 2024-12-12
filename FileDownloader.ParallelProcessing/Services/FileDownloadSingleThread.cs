using FileDownloader.ParallelProcessing.Models;
using System.Diagnostics;

namespace FileDownloader.ParallelProcessing.Services
{
    public class FileDownloadSingleThread
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public async Task DownloadFilesSequentiallyAsync(string url, string destinationPath, IProgress<DownloadProgress> progress, CancellationTokenSource cancellationTokenSource)
        {
            await _semaphore.WaitAsync(); // Acquire the semaphore
            try
            {
                long totalBytesToReceive = 0;
                long totalBytesDownloaded = 0;

                if (File.Exists(destinationPath))
                {
                    totalBytesDownloaded = new System.IO.FileInfo(destinationPath).Length; // Get the size of the existing file
                }

                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromMinutes(30); // Set a timeout value

                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
                    if (totalBytesDownloaded > 0)
                    {
                        request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(totalBytesDownloaded, null);
                    }

                    using (HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationTokenSource.Token))
                    {
                        response.EnsureSuccessStatusCode(); // Throw an exception if the HTTP response is an error

                        if (response.Content.Headers.ContentLength.HasValue)
                        {
                            totalBytesToReceive = response.Content.Headers.ContentLength.Value + totalBytesDownloaded;
                        }

                        using (Stream contentStream = await response.Content.ReadAsStreamAsync(cancellationTokenSource.Token),
                            fileStream = new FileStream(destinationPath, FileMode.Append, FileAccess.Write, FileShare.None, 8192, true))
                        {
                            byte[] buffer = new byte[8192];
                            Stopwatch stopwatch = new Stopwatch();
                            long previousBytesReceived = totalBytesDownloaded;

                            int bytesRead;
                            stopwatch.Start();


                            while ((bytesRead = await contentStream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationTokenSource.Token)) > 0)
                            {
                                await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationTokenSource.Token);

                                // Update totalBytesDownloaded immediately after reading
                                totalBytesDownloaded += bytesRead;

                                // Calculate the percentage for immediate progress bar update
                                int percentage = totalBytesToReceive > 0 ? (int)(totalBytesDownloaded * 100 / totalBytesToReceive) : 0;

                                // Measure elapsed time since the last speed update
                                double secondsElapsed = stopwatch.Elapsed.TotalSeconds;

                                // Calculate download speed in KB/s
                                double speedInKbps = secondsElapsed > 0 ? (totalBytesDownloaded - previousBytesReceived) / 1024.0 / secondsElapsed : 0;

                                // Report progress
                                progress?.Report(new DownloadProgress
                                {
                                    Percentage = percentage,
                                    BytesReceived = totalBytesDownloaded,
                                    TotalBytesToReceive = totalBytesToReceive,
                                    Speed = speedInKbps,
                                });

                                if (secondsElapsed >= 1) // Update speed and other properties every second
                                {
                                    // Reset stopwatch and update previous bytes
                                    previousBytesReceived = totalBytesDownloaded;
                                    stopwatch.Restart();
                                }
                            }

                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Download paused or canceled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                _semaphore.Release(); // Release the semaphore
            }
        }
    }
}
