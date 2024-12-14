using FileDownloader.ParallelProcessing.Models;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos.Streams;

namespace FileDownloader.ParallelProcessing.Services
{
    internal class YoutubeDownloadSingleThread
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public async Task DownloadVideo(string videoUrl, string outputFolderPath, IProgress<DownloadProgress> progress, CancellationTokenSource cancellationTokenSource)
        {
            await _semaphore.WaitAsync();
            try
            {
                var youtube = new YoutubeClient();

                var video = await youtube.Videos.GetAsync(videoUrl);

                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(videoUrl);

                var audioStreamInfo = streamManifest
                    .GetAudioStreams()
                    .Where(s => s.Container == Container.Mp4)
                    .GetWithHighestBitrate();

                var videoStreamInfo = streamManifest
                    .GetVideoStreams()
                    .Where(s => s.Container == Container.Mp4)
                    .OrderByDescending(s => s.VideoQuality.Label)
                    .FirstOrDefault();

                string outputFolder = outputFolderPath;
                Directory.CreateDirectory(outputFolder);

                string outputFilePath = Path.Combine(outputFolder, $"{video.Title}.mp4");

                var streamInfos = new IStreamInfo[] { audioStreamInfo, videoStreamInfo };
                var conversionRequest = new ConversionRequestBuilder(outputFilePath).Build();

                long totalBytesToReceive = streamInfos
                                            .Where(info => info.Size != null)
                                            .Sum(info => info.Size.Bytes);

                long previousBytesDownloaded = 0;
                var stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();

                var progressDouble = new Progress<double>(percentage =>
                {
                    double bytesDownloaded = percentage * totalBytesToReceive;

                    double elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
                    double speedInKbps = elapsedSeconds > 0
                        ? (bytesDownloaded - previousBytesDownloaded) / elapsedSeconds / 1024
                        : 0;

                    progress?.Report(new DownloadProgress
                    {
                        Percentage = (int)(percentage * 100) + 1,
                        BytesReceived = (long)bytesDownloaded,
                        TotalBytesToReceive = totalBytesToReceive,
                        Speed = speedInKbps,
                    });

                    previousBytesDownloaded = (long)bytesDownloaded;
                    stopwatch.Restart();
                });

                await youtube.Videos.DownloadAsync(streamInfos, conversionRequest, progressDouble, cancellationToken: cancellationTokenSource.Token);
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
                _semaphore.Release();
            }
        }


        public async Task DownloadPlaylist(string playlistUrl, string outputFolderPath, IProgress<DownloadProgress> progress, CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                var youtube = new YoutubeClient();

                var playlist = await youtube.Playlists.GetAsync(playlistUrl);
                var videos = await youtube.Playlists.GetVideosAsync(playlistUrl);

                string outputFolder = Path.Combine(outputFolderPath, playlist.Title);
                Directory.CreateDirectory(outputFolder);

                foreach (var video in videos)
                {
                    try
                    {
                        var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Url);

                        var audioStreamInfo = streamManifest
                            .GetAudioStreams()
                            .Where(s => s.Container == Container.Mp4)
                            .GetWithHighestBitrate();

                        var videoStreamInfo = streamManifest
                            .GetVideoStreams()
                            .Where(s => s.Container == Container.Mp4)
                            .OrderByDescending(s => s.VideoQuality.Label)
                            .FirstOrDefault();

                        string outputFilePath = Path.Combine(outputFolder, $"{video.Title}.mp4");

                        var streamInfos = new IStreamInfo[] { audioStreamInfo, videoStreamInfo };
                        var conversionRequest = new ConversionRequestBuilder(outputFilePath).Build();

                        long totalBytesToReceive = streamInfos
                            .Where(info => info.Size != null)
                            .Sum(info => info.Size.Bytes);

                        long previousBytesDownloaded = 0;
                        var stopwatch = new System.Diagnostics.Stopwatch();
                        stopwatch.Start();

                        var progressDoubel = new Progress<double>(percentage =>
                        {
                            double bytesDownloaded = percentage * totalBytesToReceive;
                            double elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
                            double speedInKbps = elapsedSeconds > 0
                                ? (bytesDownloaded - previousBytesDownloaded) / elapsedSeconds / 1024
                                : 0;

                            progress?.Report(new DownloadProgress
                            {
                                Percentage = (int)(percentage * 100) + 1,
                                BytesReceived = (long)bytesDownloaded,
                                TotalBytesToReceive = totalBytesToReceive,
                                Speed = speedInKbps,
                            });

                            previousBytesDownloaded = (long)bytesDownloaded;
                            stopwatch.Restart();
                        });

                        await youtube.Videos.DownloadAsync(streamInfos, conversionRequest, progressDoubel, cancellationToken: cancellationTokenSource.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        Console.WriteLine($"Download of video {video.Title} paused or canceled.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred while downloading video {video.Title}: {ex.Message}");
                    }
                }

                Console.WriteLine("Playlist download complete.");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Playlist download paused or canceled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}