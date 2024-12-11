using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDownloader.ParallelProcessing.Models
{

    public class DownloadProgress
    {
        public int Percentage { get; set; }
        public long BytesReceived { get; set; }
        public long TotalBytesToReceive { get; set; }
        public double Speed { get; set; }

    }

    public class Downloadpanel
    {
        public Panel dwonloadpanel { get; set; }
        public string URL{ get; set; }
        public string Destination{ get; set; }
        public IProgress<DownloadProgress> progress { get; set; }
        public CancellationTokenSource _cancellationTokenSource { get; set; }
    }
}
