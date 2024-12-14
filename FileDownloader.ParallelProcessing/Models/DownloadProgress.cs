namespace FileDownloader.ParallelProcessing.Models
{

    public class DownloadProgress
    {
        public int Percentage { get; set; }
        public long BytesReceived { get; set; }
        public long TotalBytesToReceive { get; set; }
        public double Speed { get; set; }
    }
}
