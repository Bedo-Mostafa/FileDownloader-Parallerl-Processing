using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace YouTubeDownloaderConsoleAppAsynchronous
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("YouTube Video Downloader - Console Application");

            // Get URLs from the user
            Console.Write("Enter URL for Video 1: ");
            string? url1 = Console.ReadLine();

            Console.Write("Enter URL for Video 2: ");
            string? url2 = Console.ReadLine();

            Console.Write("Enter URL for Video 3: ");
            string? url3 = Console.ReadLine();

            if (string.IsNullOrEmpty(url1) || string.IsNullOrEmpty(url2) || string.IsNullOrEmpty(url3))
            {
                Console.WriteLine("Please provide valid URLs for all videos.");
                return;
            }

            // Confirm details for each video before downloading
            if (await ConfirmVideoDetails(url1, 1) &&
                await ConfirmVideoDetails(url2, 2) &&
                await ConfirmVideoDetails(url3, 3))
            {
                // Start downloading concurrently, each in parallel on its own thread
                var task1 = Task.Run(() => DownloadVideoAsync(url1, 1));
                var task2 = Task.Run(() => DownloadVideoAsync(url2, 2));
                var task3 = Task.Run(() => DownloadVideoAsync(url3, 3));

                // Wait for all downloads to finish
                await Task.WhenAll(task1, task2, task3);

                Console.WriteLine("\nAll downloads are completed!");
            }
            else
            {
                Console.WriteLine("Download canceled.");
            }
        }

        static async Task<bool> ConfirmVideoDetails(string videoUrl, int videoNumber)
        {
            try
            {
                var youtube = new YoutubeClient();

                // Get the video details
                var video = await youtube.Videos.GetAsync(videoUrl);

                // Display details to the user
                Console.WriteLine($"\nVideo {videoNumber} Details:");
                Console.WriteLine($"Title: {video.Title}");
                Console.WriteLine($"Author: {video.Author.ChannelTitle}");
                Console.WriteLine($"Duration: {video.Duration?.ToString(@"hh\:mm\:ss") ?? "Unknown"}");

                // Ask for confirmation
                Console.Write($"Do you want to download Video {videoNumber}? (yes/no): ");
                string? input = Console.ReadLine()?.ToLower();

                return input == "yes";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching details for {videoUrl}: {ex.Message}");
                return false;
            }
        }

        static async Task DownloadVideoAsync(string videoUrl, int videoNumber)
        {
            try
            {
                var youtube = new YoutubeClient();

                // Get the video details
                var video = await youtube.Videos.GetAsync(videoUrl);

                // Get stream manifest
                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(videoUrl);

                // Select best audio stream (highest bitrate)
                var audioStreamInfo = streamManifest
                    .GetAudioStreams()
                    .Where(s => s.Container == Container.Mp4)
                    .GetWithHighestBitrate();

                // Select best video stream (highest quality)
                var videoStreamInfo = streamManifest
                    .GetVideoStreams()
                    .Where(s => s.Container == Container.Mp4)
                    .OrderByDescending(s => s.VideoQuality.Label)
                    .FirstOrDefault();

                // Set up output folder dynamically
                string outputFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads\\YouTubeDownloader", "Videos");
                // Ensure the output directory exists
                Directory.CreateDirectory(outputFolder);
                string outputFilePath = Path.Combine(outputFolder, $"{video.Title}.mp4");

                // Create a progress handler
                var progress = new Progress<double>(percent =>
                {
                    Console.WriteLine($"Video {videoNumber} Download Progress: {percent:P0}");
                });

                // Download and mux streams into a single file with progress reporting
                var streamInfos = new IStreamInfo[] { audioStreamInfo, videoStreamInfo };
                await youtube.Videos.DownloadAsync(streamInfos, new ConversionRequestBuilder(outputFilePath).Build(), progress);

                Console.WriteLine($"Video {videoNumber} downloaded successfully to: {outputFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading Video {videoNumber} ({videoUrl}): {ex.Message}");
            }
        }
    }
}
