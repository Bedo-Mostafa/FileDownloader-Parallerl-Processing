//using FileDownloader.ParallelProcessing.Models;
//using System.Diagnostics;

//namespace FileDownloader.ParallelProcessing.Services
//{
//    public class FileDownloadSingleThread
//    {
//        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
//        public async Task DownloadFilesSequentiallyAsync(string url, string destinationPath, IProgress<DownloadProgress> progress, CancellationToken cancellationToken)
//        {
//            await _semaphore.WaitAsync(cancellationToken); // Acquire the semaphore
//            try
//            {
//                string fileName = Path.GetFileName(new Uri(url).LocalPath);

//                Stopwatch stopwatch = new Stopwatch();
//                stopwatch.Start(); // Start the stopwatch
//                long previousBytesReceived = 0;
//                DateTime previousTime = DateTime.Now;

//                try
//                {
//                    using (HttpClient httpClient = new HttpClient())
//                    {
//                        httpClient.Timeout = TimeSpan.FromMinutes(30); // Set a timeout value

//                        using (HttpResponseMessage response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
//                        {
//                            response.EnsureSuccessStatusCode(); // Throw an exception if the HTTP response is an error

//                            long totalBytesToReceive = response.Content.Headers.ContentLength ?? 0;

//                            using (Stream contentStream = await response.Content.ReadAsStreamAsync(cancellationToken),
//                                fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
//                            {
//                                byte[] buffer = new byte[8192];
//                                long totalBytesReceived = 0;
//                                int bytesRead;

//                                while ((bytesRead = await contentStream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken)) > 0)
//                                {
//                                    await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
//                                    totalBytesReceived += bytesRead;

//                                    TimeSpan timeElapsed = DateTime.Now - previousTime;

//                                    if (timeElapsed.TotalSeconds > 0)
//                                    {
//                                        float speedInKbps = CalculateSpeed(totalBytesReceived, stopwatch); // Speed in KB/s
//                                        progress?.Report(new DownloadProgress
//                                        {
//                                            Percentage = totalBytesToReceive > 0 ? (int)(totalBytesReceived * 100 / totalBytesToReceive) : 0,
//                                            BytesReceived = totalBytesReceived,
//                                            TotalBytesToReceive = totalBytesToReceive,
//                                            Speed = speedInKbps, // Update speed in KB/s
//                                        });

//                                        previousBytesReceived = totalBytesReceived;
//                                        previousTime = DateTime.Now; // Reset the timer
//                                    }
//                                }
//                            }
//                        }
//                    }
//                }
//                catch (OperationCanceledException)
//                {
//                    Console.WriteLine("Download canceled.");
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"An error occurred during download: {ex.Message}");
//                }
//            }
//            finally
//            {
//                _semaphore.Release(); // Release the semaphore
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
//            Uri uri = new Uri(url);
//            // Get the file name from the URL
//            fileName = Path.GetFileName(uri.LocalPath);
//            return fileName;
//        }
//    }
//}


using FileDownloader.ParallelProcessing.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FileDownloader.ParallelProcessing.Services
{
    public class FileDownloadSingleThread
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public async Task DownloadFilesSequentiallyAsync(string url, string destinationPath, IProgress<DownloadProgress> progress, CancellationToken cancellationToken)
        {
            await _semaphore.WaitAsync(cancellationToken); // Acquire the semaphore
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

                    using (HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
                    {
                        response.EnsureSuccessStatusCode(); // Throw an exception if the HTTP response is an error

                        if (response.Content.Headers.ContentLength.HasValue)
                        {
                            totalBytesToReceive = response.Content.Headers.ContentLength.Value + totalBytesDownloaded;
                        }

                        using (Stream contentStream = await response.Content.ReadAsStreamAsync(cancellationToken),
                            fileStream = new FileStream(destinationPath, FileMode.Append, FileAccess.Write, FileShare.None, 8192, true))
                        {
                            byte[] buffer = new byte[8192];
                            Stopwatch stopwatch = Stopwatch.StartNew();
                            long previousBytesReceived = totalBytesDownloaded;
                            DateTime previousTime = DateTime.Now;

                            int bytesRead;
                            while ((bytesRead = await contentStream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken)) > 0)
                            {
                                await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
                                totalBytesDownloaded += bytesRead;

                                TimeSpan timeElapsed = DateTime.Now - previousTime;

                                if (timeElapsed.TotalSeconds > 0)
                                {
                                    float speedInKbps = CalculateSpeed(totalBytesDownloaded - previousBytesReceived, stopwatch);
                                    progress?.Report(new DownloadProgress
                                    {
                                        Percentage = totalBytesToReceive > 0 ? (int)(totalBytesDownloaded * 100 / totalBytesToReceive) : 0,
                                        BytesReceived = totalBytesDownloaded,
                                        TotalBytesToReceive = totalBytesToReceive,
                                        Speed = speedInKbps, // Speed in KB/s
                                    });

                                    previousBytesReceived = totalBytesDownloaded;
                                    previousTime = DateTime.Now;
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

        private float CalculateSpeed(long bytesReceived, Stopwatch stopwatch)
        {
            // Calculate speed in KB/s
            if (stopwatch.Elapsed.TotalSeconds > 0)
            {
                return (float)(bytesReceived / 1024 / stopwatch.Elapsed.TotalSeconds); // Convert to KB/s
            }
            return 0;
        }
    }
}
