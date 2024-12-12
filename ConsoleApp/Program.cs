//using AngleSharp.Dom;
//using System;
//using System.IO;
//using System.Linq;
//using YoutubeExplode;
//using YoutubeExplode.Converter;
//using YoutubeExplode.Videos.Streams;
//namespace YouTubeDownloaderConsoleAppSynchronous
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            Console.WriteLine("YouTube Video Downloader - Console Application");
//            // Get URLs from the user
//            Console.Write("Enter URL for Video 1: ");
//            string? url1 = Console.ReadLine();
//            Console.Write("Enter URL for Video 2: ");
//            string? url2 = Console.ReadLine();
//            Console.Write("Enter URL for Video 3: ");
//            string? url3 = Console.ReadLine();
//            if (string.IsNullOrEmpty(url1) || string.IsNullOrEmpty(url2) || string.IsNullOrEmpty(url3))
//            {
//                Console.WriteLine("Please provide valid URLs for all videos.");
//                return;
//            }
//            // Download videos synchronously
//            var task1 = Task.Run(() => DownloadVideo(url1, 1));
//            task1.Wait();
//            var task2 = Task.Run(() => DownloadVideo(url2, 2));
//            task2.Wait();
//            var task3 = Task.Run(() => DownloadVideo(url3, 3));
//            task3.Wait();
//            Console.WriteLine("\nAll downloads are completed!");
//        }
//        static void DownloadVideo(string videoUrl, int videoNumber)
//        {
//            try
//            {
//                var youtube = new YoutubeClient();
//                // Get the video details synchronously
//                var video = youtube.Videos.GetAsync(videoUrl).Result; // Blocking call
//                // Get stream manifest synchronously
//                var streamManifest = youtube.Videos.Streams.GetManifestAsync(videoUrl).Result; // Blocking call
//                // Select best audio stream (highest bitrate)
//                var audioStreamInfo = streamManifest
//                    .GetAudioStreams()
//                    .Where(s => s.Container == Container.Mp4)
//                    .GetWithHighestBitrate();
//                // Select best video stream (1080p60 in this example)
//                var videoStreamInfo = streamManifest
//                    .GetVideoStreams()
//                    .Where(s => s.Container == Container.Mp4)
//                    .OrderByDescending(s => s.VideoQuality.Label)
//                    .FirstOrDefault();
//                // Set up output folder dynamically
//                string outputFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads\\You-tubeDownloader", "Videos");
//                // Ensure the output directory exists
//                Directory.CreateDirectory(outputFolder);
//                string outputFilePath = Path.Combine(outputFolder, $"{video.Title}.mp4");
//                // Download and mux streams into a single file synchronously
//                var streamInfos = new IStreamInfo[] { audioStreamInfo, videoStreamInfo };
//                youtube.Videos.DownloadAsync(streamInfos, new ConversionRequestBuilder(outputFilePath).Build())
//                                .AsTask()
//                                .Wait(); // Blocking call
//                Console.WriteLine($"Video {videoNumber} downloaded: {video.Title}");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error downloading {videoUrl}: {ex.Message}");
//            }
//        }
//    }
//}
//-----------------------------------------------------------------Download Play list-----------------------------------------------------------------
using System.Collections.Concurrent;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos.Streams;

namespace YouTubeDownloaderConsoleAppAsynchronous
{
    class Program
    {
        private static readonly ConcurrentDictionary<string, bool> DownloadedVideos = new ConcurrentDictionary<string, bool>();
        private static readonly BlockingCollection<string> DownloadQueue = new BlockingCollection<string>();
        private static readonly object ConsoleLock = new object();

        static async Task Main(string[] args)
        {
            Console.WriteLine("YouTube Video Downloader - Console Application");
            Console.WriteLine("Enter video or playlist URLs one by one. Type 'q' and press Enter to quit.\n");

            // Start a background task to collect user input
            Task inputTask = Task.Run(() => CollectUrls());

            // Process downloads synchronously
            await ProcessDownloadsSynchronously();

            // Wait for user input collection to complete
            await inputTask;

            Console.WriteLine("\nAll downloads are completed!");
        }

        private static void CollectUrls()
        {
            while (true)
            {
                Console.Write("Enter a YouTube URL (or 'q' to quit): ");
                string? input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Please enter a valid URL or 'q' to quit.");
                    continue;
                }

                if (input.Trim().ToLower() == "q")
                {
                    Console.WriteLine("\nStopping downloads...");
                    DownloadQueue.CompleteAdding(); // Signal that no more items will be added
                    break;
                }

                // Check if the input is a playlist URL
                if (input.Contains("playlist"))
                {
                    Console.WriteLine("Detected a playlist. Fetching videos...");
                    DownloadPlaylist(input).Wait();
                    continue;
                }

                // Check for duplicate links (both video and playlist)
                if (DownloadedVideos.ContainsKey(input))
                {
                    Console.WriteLine("This video or playlist is already in the download queue or has been downloaded.");
                    continue;
                }

                // Add the URL to the download queue
                DownloadedVideos.TryAdd(input, false); // Mark it as "processing"
                DownloadQueue.Add(input);
            }
        }

        private static async Task DownloadPlaylist(string playlistUrl)
        {
            var youtube = new YoutubeClient();
            try
            {
                // Fetch the playlist details
                var playlist = await youtube.Playlists.GetAsync(playlistUrl);
                var playlistVideos = await youtube.Playlists.GetVideosAsync(playlist.Id);

                Console.WriteLine($"Playlist '{playlist.Title}' found. Fetching {playlist.Count} videos...");

                // Add each video URL in the playlist to the download queue
                foreach (var video in playlistVideos)
                {
                    string videoUrl = video.Url;
                    if (!DownloadedVideos.ContainsKey(videoUrl))
                    {
                        DownloadedVideos.TryAdd(videoUrl, false);
                        DownloadQueue.Add(videoUrl);
                    }
                }
            }
            catch (Exception ex)
            {
                lock (ConsoleLock)
                {
                    Console.WriteLine($"Error processing playlist {playlistUrl}: {ex.Message}");
                }
            }
        }

        private static async Task ProcessDownloadsSynchronously()
        {
            var youtube = new YoutubeClient();

            foreach (var videoUrl in DownloadQueue.GetConsumingEnumerable())
            {
                await DownloadVideoAsync(youtube, videoUrl);
            }
        }

        private static async Task DownloadVideoAsync(YoutubeClient youtube, string videoUrl)
        {
            try
            {
                // Get the video details
                var video = await youtube.Videos.GetAsync(videoUrl);

                // Get stream manifest
                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(videoUrl);

                // Select best audio stream (highest bitrate)
                var audioStreamInfo = streamManifest
                    .GetAudioStreams()
                    .Where(s => s.Container == Container.Mp4)
                    .GetWithHighestBitrate();

                // Select best video stream
                var videoStreamInfo = streamManifest
                    .GetVideoStreams()
                    .Where(s => s.Container == Container.Mp4)
                    .OrderByDescending(s => s.VideoQuality.Label)
                    .FirstOrDefault();

                // Set up output folder dynamically
                string outputFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads\\You-tubeDownloader", "Videos");

                // Ensure the output directory exists
                Directory.CreateDirectory(outputFolder);

                // Ensure unique file name
                string sanitizedTitle = string.Join("_", video.Title.Split(Path.GetInvalidFileNameChars()));
                string uniqueFileName = $"{sanitizedTitle}_{DateTime.Now:yyyyMMddHHmmss}.mp4";
                string outputFilePath = Path.Combine(outputFolder, uniqueFileName);

                // Download and mux streams into a single file
                IStreamInfo[] streamInfos = new IStreamInfo[] { audioStreamInfo, videoStreamInfo };
                await youtube.Videos.DownloadAsync(streamInfos, new ConversionRequestBuilder(outputFilePath).Build());

                DownloadedVideos.Remove(videoUrl, out bool value); // Mark as "downloaded"

                lock (ConsoleLock)
                {
                    Console.WriteLine($"Download completed: {video.Title}");
                }
            }
            catch (Exception ex)
            {
                lock (ConsoleLock)
                {
                    Console.WriteLine($"Error downloading {videoUrl}: {ex.Message}");
                }
            }
        }
    }
}
