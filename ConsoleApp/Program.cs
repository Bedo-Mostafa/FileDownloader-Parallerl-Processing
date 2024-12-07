using AngleSharp.Dom;
using System;
using System.IO;
using System.Linq;
using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos.Streams;

namespace YouTubeDownloaderConsoleAppSynchronous
{
    class Program
    {
        static void Main(string[] args)
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

            // Download videos synchronously
            var task1 = Task.Run(() => DownloadVideo(url1, 1));
            task1.Wait();
            var task2 = Task.Run(() => DownloadVideo(url1, 2));
            task2.Wait();
            var task3 = Task.Run(() => DownloadVideo(url1, 3));
            task3.Wait();


            Console.WriteLine("\nAll downloads are completed!");
        }

        static void DownloadVideo(string videoUrl, int videoNumber)
        {
            try
            {
                var youtube = new YoutubeClient();

                // Get the video details synchronously
                var video = youtube.Videos.GetAsync(videoUrl).Result; // Blocking call

                // Get stream manifest synchronously
                var streamManifest = youtube.Videos.Streams.GetManifestAsync(videoUrl).Result; // Blocking call

                // Select best audio stream (highest bitrate)
                var audioStreamInfo = streamManifest
                    .GetAudioStreams()
                    .Where(s => s.Container == Container.Mp4)
                    .GetWithHighestBitrate();

                // Select best video stream (1080p60 in this example)
                var videoStreamInfo = streamManifest
                    .GetVideoStreams()
                    .Where(s => s.Container == Container.Mp4)
                    .OrderByDescending(s => s.VideoQuality.Label)
                    .FirstOrDefault();

                // Set up output folder dynamically
                string outputFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads\\You-tubeDownloader", "Videos");
                // Ensure the output directory exists
                Directory.CreateDirectory(outputFolder);
                string outputFilePath = Path.Combine(outputFolder, $"{video.Title}.mp4");

                // Download and mux streams into a single file synchronously
                var streamInfos = new IStreamInfo[] { audioStreamInfo, videoStreamInfo };
                youtube.Videos.DownloadAsync(streamInfos, new ConversionRequestBuilder(outputFilePath).Build())
                                .AsTask()
                                .Wait(); // Blocking call


                Console.WriteLine($"Video {videoNumber} downloaded: {video.Title}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading {videoUrl}: {ex.Message}");
            }
        }
    }
}
