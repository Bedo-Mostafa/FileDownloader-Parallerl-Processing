using FileDownloader.ParallelProcessing.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FileInfo = FileDownloader.ParallelProcessing.Models.FileInfo;

namespace FileDownloader.ParallelProcessing
{
    public partial class MultiThreadDashboard : Form
    {
        public MultiThreadDashboard()
        {
            InitializeComponent();
        }

        FileInfo information = new FileInfo()
        {
            FileName= "Hazem"
        };
        private void button1_Click(object sender, EventArgs e)
        {
            CreateDownloadPanel(information);
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {
            CreateDownloadPanel(information);

        }

        private void label2_Click(object sender, EventArgs e)
        {
            CreateDownloadPanel(information);

        }

        private void button4_Click(object sender, EventArgs e)
        {
            CreateDownloadPanel(information);

        }



        private void button5_Click(object sender, EventArgs e)
        {
            CreateDownloadPanel(information);
        }

        private void label3_Click(object sender, EventArgs e)
        {
            CreateDownloadPanel(information);

        }

        private void label4_Click(object sender, EventArgs e)
        {
            CreateDownloadPanel(information);

        }

        private void label6_Click(object sender, EventArgs e)
        {
            CreateDownloadPanel(information);

        }

        private void label8_Click(object sender, EventArgs e)
        {
            CreateDownloadPanel(information);

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            CreateDownloadPanel(information);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            CreateDownloadPanel(information);

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Resume_Click(object sender, EventArgs e)
        {

        }
    }
}
