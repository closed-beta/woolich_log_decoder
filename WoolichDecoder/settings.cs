using System;
using System.Windows.Forms;

namespace WoolichDecoder
{
    public partial class AppSettings : Form
    {
        public int MinRPM { get; private set; }
        public int MaxRPM { get; private set; }

        public AppSettings()
        {
            InitializeComponent();
        }

        // OK button click event handler
        private void btnOK_Click(object sender, EventArgs e)
        {
            MinRPM = int.Parse(textBox2.Text);
            MaxRPM = int.Parse(textBox3.Text);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // Cancel button click event handler
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // Open Folder Browser dialog directly
        private void btnOpenFolderSelector_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowser = new FolderBrowserDialog())
            {
                DialogResult result = folderBrowser.ShowDialog();
                if (result == DialogResult.OK)
                {
                    // Assuming you have a TextBox named textBoxPath
                    textBoxPath.Text = folderBrowser.SelectedPath;
                }
            }
        }
    }
}
