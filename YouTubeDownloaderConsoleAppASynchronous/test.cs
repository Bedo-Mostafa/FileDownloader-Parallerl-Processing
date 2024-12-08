//using System;
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
//        private static readonly object ConsoleLock = new object();

//        static async Task Main(string[] args)
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

//            // Start downloading concurrently, each in parallel on its own thread
//            var task1 = Task.Run(() => DownloadVideoAsync(url1, 1, videoStreamInfo: videoStreamInfo));
//            var task2 = Task.Run(() => DownloadVideoAsync(url2, 2, videoStreamInfo: videoStreamInfo));
//            var task3 = Task.Run(() => DownloadVideoAsync(url3, 3, videoStreamInfo: videoStreamInfo));

//            // Wait for all downloads to finish
//            await Task.WhenAll(task1, task2, task3);

//            Console.WriteLine("\nAll downloads are completed!");
//        }

//        static async Task DownloadVideoAsync(string videoUrl, int videoNumber)
//        {
//            try
//            {
//                var youtube = new YoutubeClient();

//                // Get the video details
//                var video = await youtube.Videos.GetAsync(videoUrl);

//                // Get stream manifest
//                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(videoUrl);

//                // Select best audio and video streams
//                var audioStreamInfo = streamManifest
//                    .GetAudioStreams()
//                    .Where(s => s.Container == Container.Mp4)
//                    .GetWithHighestBitrate();

//                var videoStreamInfo = streamManifest
//                    .GetVideoStreams()
//                    .Where(s => s.Container == Container.Mp4)
//                    .OrderByDescending(s => s.VideoQuality.Label)
//                    .FirstOrDefault();

//                // Set up output folder
//                string outputFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads\\You-tubeDownloader", "Videos");
//                Directory.CreateDirectory(outputFolder);

//                // Ensure unique file name
//                string sanitizedTitle = string.Join("_", video.Title.Split(Path.GetInvalidFileNameChars()));
//                string uniqueFileName = $"{sanitizedTitle}_{DateTime.Now:yyyyMMddHHmmss}.mp4";
//                string outputFilePath = Path.Combine(outputFolder, uniqueFileName);

//                // Create a progress handler
//                var progress = new Progress<double>(percent =>
//                {
//                    lock (ConsoleLock)
//                    {
//                        Console.WriteLine($"Video {videoNumber} Download Progress: {percent:P0}");
//                    }
//                });

//                // Download and mux streams into a single file with progress reporting
//                var streamInfos = new IStreamInfo[] { audioStreamInfo, videoStreamInfo };
//                await youtube.Videos.DownloadAsync(streamInfos, new ConversionRequestBuilder(outputFilePath).Build(), progress);
//            }
//            catch (Exception ex)
//            {
//                lock (ConsoleLock)
//                {
//                    Console.WriteLine($"Error downloading video {videoNumber}: {ex.Message}");
//                }
//            }
//        }
//    }
//}
