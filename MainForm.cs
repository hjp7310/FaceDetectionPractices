using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceDetectionPractices
{
    public partial class MainForm : Form
    {
        CamForm CamForm;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void start_button_Click(object sender, EventArgs e)
        {
            CamForm = new CamForm();
            CamForm.coordinateChanged += new EventHandler(UpdateTextBox);
            CamForm.Show();
        }

        private void stop_button_Click(object sender, EventArgs e)
        {
            CamForm?.Close();
        }

        private void UpdateTextBox(object sender, EventArgs e)
        {
            textBox?.Invoke((MethodInvoker)delegate ()
            {
                textBox.AppendText(CamForm?.coordinate.Item1.ToString() + ", " + CamForm?.coordinate.Item2.ToString() + " \r\n");
            });
        }
    }
}
