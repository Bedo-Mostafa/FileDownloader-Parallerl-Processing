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

        public FileInfo()
        {

        }
        public FileInfo(string fileName, Queue<string> urls)
        {
            FileName = fileName;
            this.urls = urls;
        }
        public FileInfo(string fileName) { 
        
            FileName = fileName;
        }
    }
}
