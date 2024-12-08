//using System;
//using System.Collections.Concurrent;
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;
//using YoutubeExplode;
//using YoutubeExplode.Converter;
//using YoutubeExplode.Videos.Streams;

//namespace YouTubeDownloaderConsoleAppAsynchronous
//{
//    class Program
//    {
//        private static readonly ConcurrentDictionary<string, bool> DownloadedVideos = new ConcurrentDictionary<string, bool>();
//        private static readonly BlockingCollection<string> DownloadQueue = new BlockingCollection<string>();
//        private static readonly object ConsoleLock = new object();

//        static async Task Main(string[] args)
//        {
//            Console.WriteLine("YouTube Video Downloader - Console Application");
//            Console.WriteLine("Enter video URLs one by one. Type 'q' and press Enter to quit.\n");

//            // Start a background task to collect user input
//            Task inputTask = Task.Run(() => CollectUrls());

//            // Process downloads in parallel
//            await ProcessDownloadsInParallel();

//            // Wait for user input collection to complete
//            await inputTask;

//            Console.WriteLine("\nAll downloads are completed!");
//        }

//        private static void CollectUrls()
//        {
//            while (true)
//            {
//                Console.Write("Enter a YouTube URL (or 'q' to quit): ");
//                string? input = Console.ReadLine();

//                if (string.IsNullOrWhiteSpace(input))
//                {
//                    Console.WriteLine("Please enter a valid URL or 'q' to quit.");
//                    continue;
//                }

//                if (input.Trim().ToLower() == "q")
//                {
//                    Console.WriteLine("\nStopping downloads...");
//                    DownloadQueue.CompleteAdding(); // Signal that no more items will be added
//                    break;
//                }
//                // Check for duplicate links
//                if (DownloadedVideos.ContainsKey(input))
//                {
//                    Console.WriteLine("This video is already in the download queue or has been downloaded.");
//                    continue;
//                }

//                // Add the URL to the download queue
//                DownloadedVideos.TryAdd(input, false); // Mark it as "processing"
//                DownloadQueue.Add(input);
//            }
//        }

//        private static async Task ProcessDownloadsInParallel()
//        {
//            var youtube = new YoutubeClient();
//            const int MaxDegreeOfParallelism = 5; // Adjust based on your system's capability

//            // Limit the number of concurrent tasks
//            var semaphore = new SemaphoreSlim(MaxDegreeOfParallelism);

//            var tasks = DownloadQueue.GetConsumingEnumerable().Select(async videoUrl =>
//            {
//                await semaphore.WaitAsync();
//                try
//                {
//                    await DownloadVideoAsync(youtube, videoUrl);
//                    //DownloadedVideos[videoUrl] = true; // Mark as "downloaded"
//                }
//                finally
//                {
//                    semaphore.Release();
//                }
//            });

//            await Task.WhenAll(tasks);
//        }

//        private static async Task DownloadVideoAsync(YoutubeClient youtube, string videoUrl)
//        {
//            try
//            {
//                // Get the video details
//                var video = await youtube.Videos.GetAsync(videoUrl);

//                // Get stream manifest
//                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(videoUrl);

//                // Select best audio stream (highest bitrate)
//                var audioStreamInfo = streamManifest
//                    .GetAudioStreams()
//                    .Where(s => s.Container == Container.Mp4)
//                    .GetWithHighestBitrate();

//                // Select best video stream
//                var videoStreamInfo = streamManifest
//                    .GetVideoStreams()
//                    .Where(s => s.Container == Container.Mp4)
//                    .OrderByDescending(s => s.VideoQuality.Label)
//                    .FirstOrDefault();

//                // Set up output folder dynamically
//                string outputFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads\\You-tubeDownloader", "Videos");

//                // Ensure the output directory exists
//                Directory.CreateDirectory(outputFolder);

//                // Ensure unique file name
//                string sanitizedTitle = string.Join("_", video.Title.Split(Path.GetInvalidFileNameChars()));
//                string uniqueFileName = $"{sanitizedTitle}_{DateTime.Now:yyyyMMddHHmmss}.mp4";
//                string outputFilePath = Path.Combine(outputFolder, uniqueFileName);

//                // Download and mux streams into a single file
//                IStreamInfo[] streamInfos = new IStreamInfo[] { audioStreamInfo, videoStreamInfo };
//                await youtube.Videos.DownloadAsync(streamInfos, new ConversionRequestBuilder(outputFilePath).Build());
//                DownloadedVideos.Remove(videoUrl, out bool value);// Mark as "downloaded"

//                lock (ConsoleLock)
//                {
//                    Console.WriteLine($"Download completed: {video.Title}");
//                }
//            }
//            catch (Exception ex)
//            {
//                lock (ConsoleLock)
//                {
//                    Console.WriteLine($"Error downloading {videoUrl}: {ex.Message}");
//                }
//            }
//        }
//    }
//}
//-----------------------------------------------------------Download Play list --------------------------------------------------------------------
using System;
using System.Collections.Concurrent;
using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos.Streams;
using YoutubeExplode.Common;

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

            // Process downloads in parallel
            await ProcessDownloadsInParallel();

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
                    // Process playlist
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

        private static async Task ProcessDownloadsInParallel()
        {
            var youtube = new YoutubeClient();
            const int MaxDegreeOfParallelism = 5; // Adjust based on your system's capability

            // Limit the number of concurrent tasks
            var semaphore = new SemaphoreSlim(MaxDegreeOfParallelism);

            var tasks = DownloadQueue.GetConsumingEnumerable().Select(async videoUrl =>
            {
                await semaphore.WaitAsync();
                try
                {
                    await DownloadVideoAsync(youtube, videoUrl);
                }
                finally
                {
                    semaphore.Release();
                }
            });

            await Task.WhenAll(tasks);
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
