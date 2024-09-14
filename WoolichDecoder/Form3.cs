using System;
using System.Windows.Forms;

namespace WoolichDecoder
{
    public partial class Form3 : Form
    {
        public int MinRPM { get; private set; }
        public int MaxRPM { get; private set; }

        public Form3()
        {
            InitializeComponent();
        }

        // OK button click event handler
        public void btnOK_Click(object sender, EventArgs e)
        {
            // Ensure that textBox2 and textBox3 are valid and exist
            MinRPM = int.Parse(textBox2.Text); // Make sure textBox2 exists
            MaxRPM = int.Parse(textBox3.Text); // Make sure textBox3 exists

            // Set the DialogResult to OK and close the form
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // Cancel button click event handler
        private void btnCancel_Click(object sender, EventArgs e)
        {
            // Set the DialogResult to Cancel and close the form
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
