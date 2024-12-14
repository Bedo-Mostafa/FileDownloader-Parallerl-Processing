using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDownloader.ParallelProcessing.Models
{
    public class DownloadItem
    {
        public string FileName { get; set; }
        public string VideoUrl { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }
        public CancellationToken Token { get; set; }
        public IProgress<DownloadProgress> Progress { get; set; }

        public DownloadItem() { }

        public DownloadItem(string fileName)
        {

            FileName = fileName;
        }
    }
}
