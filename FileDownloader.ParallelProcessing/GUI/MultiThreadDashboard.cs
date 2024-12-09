using FileInfo = FileDownloader.ParallelProcessing.Models.FileInfo;
using FileDownloadSingelThread = FileDownloader.ParallelProcessing.Models.FileDownloadSingelThread;

namespace FileDownloader.ParallelProcessing
{
    public partial class MultiThreadDashboard : Form
    {
        public MultiThreadDashboard()
        {
            InitializeComponent();
        }

        FileDownloadSingelThread fileDownloadSingleThread = new FileDownloadSingelThread();
        FileInfo information = new FileInfo();

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

        private async void DownloadButtonSingleThread(object sender, EventArgs e)
        {
            string url = URLTextBox.Text.Trim();
            string pathDownload = LocationInput.Text.Trim();
            string fileName = fileDownloadSingleThread.GetInfo(url);
            information.FileName = fileName;

            // clear the inputs
            //URLTextBox.Clear();
            //LocationInput.Clear();

            var downloadPanel = CreateDownloadPanel(information);
            var progressBar = (downloadPanel.Controls["ProgressBar"] as ProgressBar);
            var SpeedValue = (downloadPanel.Controls["SpeedValue"] as Label);

            var progress = new Progress<(int Progress, long Speed)>(value =>
            {
                // Update the ProgressBar or Label with progress
                progressBar.Value = value.Progress;
                SpeedValue.Text = $"{value.Speed / 1024} KB/s";
            });

            await Task.Run(() => fileDownloadSingleThread.DownloadFilesSequentially(url, pathDownload, progress));
        }



        private void DownlaodPathButton(object sender, EventArgs e)
        {
            //CreateDownloadPanel(information);
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

        private void URLInputLabel(object sender, EventArgs e)
        {
            // Get the text from the TextBox
            //string url = txtUrl.Text.Trim(); // Trim to remove leading/trailing whitespace
            //string url = URLTextBox.Text.Trim();
        }

        private void Resume_Click(object sender, EventArgs e)
        {

        }

        private void MultiThreadDashboard_Load(object sender, EventArgs e)
        {

        }

        private void URLLabel_Click(object sender, EventArgs e)
        {

        }

        private void DownloadPath_Text(object sender, EventArgs e)
        {

        }
    }
}
