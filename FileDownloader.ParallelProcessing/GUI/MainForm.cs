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
                if (ThreadsNumberInput.Text != "")
                {
                    MultiThreadFormInstance = new MultiThreadDashboard(int.Parse(ThreadsNumberInput.Text));
                    MultiThreadFormInstance.Show();
                }
                else
                {
                    MessageBox.Show("Enter Threads Valid Number","Warning",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                }
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
                e.Handled = true; // Block invalid input
            }
        }

        private void ThreadsNumberInput_Validating(object sender, CancelEventArgs e)
        {
            // Allow program closure without validation if the input is empty
            if (string.IsNullOrWhiteSpace(ThreadsNumberInput.Text)) return;

            // Validate input
            if ((!int.TryParse(ThreadsNumberInput.Text, out int threads) || threads < 1) && threads != 0)
            {
                MessageBox.Show("Please enter a valid number greater than or equal to 1.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true; // Keep focus on the input
                return;
            }

            // Terminate program if input is zero
            if (threads == 0)
            {
                MessageBox.Show("Zero is not a valid thread count. The program will close.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        private void ThreadsNumberInput_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
