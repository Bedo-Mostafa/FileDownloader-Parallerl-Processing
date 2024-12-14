namespace FileDownloader.ParallelProcessing.Models
{
    public class Downloadpanel
    {
        public Panel downloadpanel { get; set; }
        public string URL { get; set; }
        public string Destination { get; set; }
        public IProgress<DownloadProgress> progress { get; set; }
        public CancellationTokenSource _cancellationTokenSource { get; set; }
    }
}
