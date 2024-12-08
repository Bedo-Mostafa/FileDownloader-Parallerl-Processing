//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Net;
//using System.Threading;
//using System.Threading.Tasks;

//namespace File_Download
//{
//    internal class Program
//    {
//        static void DownloadFile(string url, string destination)
//        {
//            try
//            {
//                using (WebClient webClient = new WebClient())
//                {
//                    webClient.DownloadProgressChanged += (s, e) =>
//                    {
//                        Console.Write($"\rDownloading {Path.GetFileName(destination)}: {e.ProgressPercentage}%");
//                    };

//                    webClient.DownloadFileCompleted += (s, e) =>
//                    {
//                        Console.WriteLine($"\nCompleted: {Path.GetFileName(destination)}");
//                    };

//                    webClient.DownloadFileTaskAsync(new Uri(url), destination).Wait();
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"\nFailed to download {url}: {ex.Message}");
//            }
//        }

//        static void Main(string[] args)
//        {
//            Console.WriteLine("Welcome to the File Downloader!");
//            Console.WriteLine("Enter file URLs (one per line). Press Enter on an empty line to finish:");

//            var urls = new HashSet<string>(); // Use a HashSet to avoid duplicates
//            string input;

//            while (!string.IsNullOrWhiteSpace(input = Console.ReadLine()))
//            {
//                if (Uri.IsWellFormedUriString(input, UriKind.Absolute))
//                {
//                    urls.Add(input);
//                }
//                else
//                {
//                    Console.WriteLine("Invalid URL. Please enter a valid URL.");
//                }
//            }

//            if (urls.Count == 0)
//            {
//                Console.WriteLine("No URLs provided. Exiting.");
//                return;
//            }

//            Console.WriteLine("Enter the destination folder (leave blank for default 'C:\\Downloads'):");
//            string destinationFolder = Console.ReadLine();
//            destinationFolder = string.IsNullOrWhiteSpace(destinationFolder) ? "C:\\Downloads" : destinationFolder;

//            Directory.CreateDirectory(destinationFolder);

//            Console.WriteLine("How many downloads would you like to run simultaneously? (Default: 5)");
//            if (!int.TryParse(Console.ReadLine(), out int maxThreads) || maxThreads <= 0)
//            {
//                maxThreads = 5;
//            }

//            Console.WriteLine("\nStarting downloads...\n");

//            // Limit simultaneous downloads using a semaphore
//            using (SemaphoreSlim semaphore = new SemaphoreSlim(maxThreads))
//            {
//                List<Task> tasks = new List<Task>();

//                foreach (string url in urls)
//                {
//                    tasks.Add(Task.Run(async () =>
//                    {
//                        await semaphore.WaitAsync();

//                        try
//                        {
//                            string fileName = Path.GetFileName(new Uri(url).LocalPath);
//                            string destination = Path.Combine(destinationFolder, fileName);

//                            DownloadFile(url, destination);
//                        }
//                        finally
//                        {
//                            semaphore.Release();
//                        }
//                    }));
//                }

//                Task.WhenAll(tasks).Wait();
//            }

//            Console.WriteLine("\nAll downloads completed!");
//        }
//    }
//}
//------------------------------------------------------ download sequential -------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace File_Download
{
    internal class Program
    {
        static void DownloadFile(string url, string destination)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadProgressChanged += (s, e) =>
                    {
                        Console.Write($"\rDownloading {Path.GetFileName(destination)}: {e.ProgressPercentage}%");
                    };

                    webClient.DownloadFileCompleted += (s, e) =>
                    {
                        Console.WriteLine($"\nCompleted: {Path.GetFileName(destination)}");
                    };

                    webClient.DownloadFileTaskAsync(new Uri(url), destination).Wait();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nFailed to download {url}: {ex.Message}");
            }
        }

        static async Task DownloadFilesSequentiallyAsync(Queue<string> urls, string destinationFolder)
        {
            while (urls.Count > 0)
            {
                string url = urls.Dequeue();
                string fileName = Path.GetFileName(new Uri(url).LocalPath);
                string destination = Path.Combine(destinationFolder, fileName);

                // Call the download method for the current file
                await Task.Run(() => DownloadFile(url, destination));
            }
        }

        static async Task Main(string[] args)
        {
            Console.WriteLine("Welcome to the File Downloader!");
            Console.WriteLine("Enter file URLs (one per line). Press Enter on an empty line to finish:");

            var urls = new Queue<string>(); // Use a Queue to store URLs
            string input;

            while (!string.IsNullOrWhiteSpace(input = Console.ReadLine()))
            {
                if (Uri.IsWellFormedUriString(input, UriKind.Absolute))
                {
                    urls.Enqueue(input);  // Add the URL to the Queue
                }
                else
                {
                    Console.WriteLine("Invalid URL. Please enter a valid URL.");
                }
            }

            if (urls.Count == 0)
            {
                Console.WriteLine("No URLs provided. Exiting.");
                return;
            }

            Console.WriteLine("Enter the destination folder (leave blank for default 'C:\\Downloads'): ");
            string destinationFolder = Console.ReadLine();
            destinationFolder = string.IsNullOrWhiteSpace(destinationFolder) ? "C:\\Downloads" : destinationFolder;

            Directory.CreateDirectory(destinationFolder);

            Console.WriteLine("\nStarting downloads...\n");

            // Start the task to download files sequentially
            await DownloadFilesSequentiallyAsync(urls, destinationFolder);

            Console.WriteLine("\nAll downloads completed!");
        }
    }
}