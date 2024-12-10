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
}
