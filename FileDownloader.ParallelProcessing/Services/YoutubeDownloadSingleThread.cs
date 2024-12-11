using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos.Streams;
using YoutubeExplode;
using FileDownloader.ParallelProcessing.Models;

namespace FileDownloader.ParallelProcessing.Services
{
    internal class YoutubeDownloadSingleThread
    {
        public async Task DownloadVideo(string videoUrl, IProgress<DownloadProgress> progress, CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                var youtube = new YoutubeClient();

                // Get video details
                var video = await youtube.Videos.GetAsync(videoUrl);

                // Get stream manifest
                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(videoUrl);

                // Select best audio and video streams
                var audioStreamInfo = streamManifest
                    .GetAudioStreams()
                    .Where(s => s.Container == Container.Mp4)
                    .GetWithHighestBitrate();

                var videoStreamInfo = streamManifest
                    .GetVideoStreams()
                    .Where(s => s.Container == Container.Mp4)
                    .OrderByDescending(s => s.VideoQuality.Label)
                    .FirstOrDefault();

                // Set up output folder
                string outputFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads\\You-tubeDownloader", "Videos");
                Directory.CreateDirectory(outputFolder);

                string outputFilePath = Path.Combine(outputFolder, $"{video.Title}.mp4");

                // Prepare to download streams
                var streamInfos = new IStreamInfo[] { audioStreamInfo, videoStreamInfo };
                var conversionRequest = new ConversionRequestBuilder(outputFilePath).Build();

                long totalBytesToReceive = streamInfos
                                            .Where(info => info.Size != null)
                                            .Sum(info => info.Size.Bytes);

                long previousBytesDownloaded = 0;
                var stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();

                // Perform the download
                await youtube.Videos.DownloadAsync(streamInfos, conversionRequest, progress: new Progress<double>(percentage =>
                {
                    // Calculate progress
                    double bytesDownloaded = percentage / 100.0 * totalBytesToReceive;
                    double speedInKbps = (bytesDownloaded - previousBytesDownloaded) / stopwatch.Elapsed.TotalSeconds / 1024;

                    progress?.Report(new DownloadProgress
                    {
                        Percentage = (int)percentage,
                        BytesReceived = (long)bytesDownloaded,
                        TotalBytesToReceive = totalBytesToReceive,
                        Speed = speedInKbps, // Speed in KB/s
                    });

                    previousBytesDownloaded = (long)bytesDownloaded;
                    stopwatch.Restart();
                }), cancellationToken: cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Download paused or canceled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
