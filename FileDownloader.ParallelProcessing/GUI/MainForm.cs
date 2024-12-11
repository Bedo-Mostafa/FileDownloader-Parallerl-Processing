using System.ComponentModel;

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
                MultiThreadFormInstance = new MultiThreadDashboard(int.Parse(ThreadsNumberInput.Text));
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void ThreadsNumberInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow only digits and control keys (e.g., Backspace)
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Block the input
            }
        }

        private void ThreadsNumberInput_Validating(object sender, CancelEventArgs e)
        {
            if (!int.TryParse(ThreadsNumberInput.Text, out _))
            {
                MessageBox.Show("Please enter a valid number.");
                e.Cancel = true; // Keep focus on the input
            }
        }

        private void ThreadsNumberInput_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
