using FileDownloader.ParallelProcessing.Models;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Converter;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos.Streams;

namespace FileDownloader.ParallelProcessing.Services
{
    public class YoutubeDownloadMultiThread
    {
        private static readonly object ConsoleLock = new object();

        public async Task DownloadVideoAsync(string videoUrl, string outputFolderPath, IProgress<DownloadProgress> progress, CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                YoutubeClient youtube = new YoutubeClient();
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

                if (audioStreamInfo == null || videoStreamInfo == null)
                {
                    throw new Exception("Failed to find suitable audio or video streams.");
                }

                // Ensure the output directory exists
                Directory.CreateDirectory(outputFolderPath);

                // Ensure unique file name
                string sanitizedTitle = string.Join("_", video.Title.Split(Path.GetInvalidFileNameChars()));
                string uniqueId = Guid.NewGuid().ToString();
                string outputFilePath = Path.Combine(outputFolderPath, $"{sanitizedTitle}_{uniqueId}.mp4");


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
                        Percentage = (int)(percentage * 100) + 1, // Convert fractional percentage to 0-100 scale
                        BytesReceived = (long)bytesDownloaded,
                        TotalBytesToReceive = totalBytesToReceive,
                        Speed = speedInKbps,
                    });

                    // Update the previous bytes downloaded and restart the stopwatch
                    previousBytesDownloaded = (long)bytesDownloaded;
                    stopwatch.Restart();
                });

                await youtube.Videos.DownloadAsync(
                    streamInfos,
                    conversionRequest,
                    progressDouble,
                    cancellationToken: cancellationTokenSource.Token
                );
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show($"Download was canceled.");
            }
            catch (Exception)
            {
                MessageBox.Show($"Error downloading {videoUrl}:", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
            }
        }

        public async Task<List<PlaylistVideo>> GetPlaylistVideos(string playlistUrl)
        {
            var youtube = new YoutubeClient();
            var videoList = new List<PlaylistVideo>();

            try
            {
                // Fetch the playlist details
                var playlist = await youtube.Playlists.GetAsync(playlistUrl);
                var playlistVideos = await youtube.Playlists.GetVideosAsync(playlist.Id);

                foreach (var video in playlistVideos)
                {
                    videoList.Add(video);
                }
            }
            catch (Exception ex)
            {
                lock (ConsoleLock)
                {
                    MessageBox.Show($"Error processing playlist {playlistUrl}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return videoList;
        }
    }
}
