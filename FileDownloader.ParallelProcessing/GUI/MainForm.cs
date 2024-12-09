namespace FileDownloader.ParallelProcessing
{
    public partial class MainForm : Form
    {
        private MultiThreadDashboard MultiThreadFormInstance = null;
        private SingleThreadDashboard SingleThreadFormInstance = null;

        public MainForm()
        {
            InitializeComponent();
        }


        #region Buttons Events
        private void MultipleThreadButtonClick(object sender, EventArgs e)
        {
            // To Check Only 1 Instance Of the Downloader is Opened

            if (MultiThreadFormInstance == null || MultiThreadFormInstance.IsDisposed)
            {
                MultiThreadFormInstance = new MultiThreadDashboard();
                MultiThreadFormInstance.Show();
            }
            else
            {
                MultiThreadFormInstance.BringToFront();
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
            // To Check Only 1 Instance Of the Downloader is Opened

            if (SingleThreadFormInstance == null || SingleThreadFormInstance.IsDisposed)
            {
                SingleThreadFormInstance = new SingleThreadDashboard();
                SingleThreadFormInstance.Show();
            }
            else
            {
                SingleThreadFormInstance.BringToFront();
            }
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
