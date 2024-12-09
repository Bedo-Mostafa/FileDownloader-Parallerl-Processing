using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDownloader.ParallelProcessing.Models
{
    public class FileInfo
    {
        public string FileName { get; set; }

        public Queue<string> urls = new Queue<string>();
    }
}
