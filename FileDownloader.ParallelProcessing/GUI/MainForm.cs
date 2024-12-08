namespace FileDownloader.ParallelProcessing
{
    public partial class MainForm : Form
    {
        private MultiThreadDashboard multiThreadFormInstance = null;

        public MainForm()
        {
            InitializeComponent();
        }


        #region Buttons Events
        private void MultipleThreadButtonClick(object sender, EventArgs e)
        {
            // To Check Only 1 Instance Of the Downloader is Opened 

            if (multiThreadFormInstance == null || multiThreadFormInstance.IsDisposed)
            {
                multiThreadFormInstance = new MultiThreadDashboard();
                multiThreadFormInstance.Show();
            }
            else
            {
                multiThreadFormInstance.BringToFront();
            }
        }
        
        private void SingleThreadButtonClick(object sender, EventArgs e)
        {

        }

        #endregion

        #region Labels
        private void MultipleThreadLabelClick(object sender, EventArgs e)
        {

        }
        private void SingleThreadLabelClick(object sender, EventArgs e)
        {

        }

        private void MultiThreadNoteLabelClick(object sender, EventArgs e)
        {

        }

        private void ProjectTitleClick(object sender, EventArgs e)
        {

        }
        private void NoteLabelClick(object sender, EventArgs e)
        {

        }
        #endregion
    }
}
