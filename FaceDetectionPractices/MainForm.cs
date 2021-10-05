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
        WebCamForm webCamForm;

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

        private void EyeTrackStartButton_Click(object sender, EventArgs e)
        {
            if (this.webCamForm == null || this.webCamForm.IsDisposed)
            {
                this.webCamForm = new WebCamForm(AIOption.EyeTrack);
                this.webCamForm.coordinateChanged += new EventHandler(UpdateTextBox);
                this.webCamForm.Show();
            }
        }

        private void HeadCoordinateStartButton_Click(object sender, EventArgs e)
        {
            if (this.webCamForm == null || this.webCamForm.IsDisposed)
            {
                this.webCamForm = new WebCamForm(AIOption.HeadCoordinate);
                this.webCamForm.coordinateChanged += new EventHandler(UpdateTextBox);
                this.webCamForm.Show();
            }
        }

        private void HeadPoseEstimateStartButton_Click(object sender, EventArgs e)
        {
            if (this.webCamForm == null || this.webCamForm.IsDisposed)
            {
                this.webCamForm = new WebCamForm(AIOption.HeadPoseEstimate);
                this.webCamForm.coordinateChanged += new EventHandler(UpdateTextBox);
                this.webCamForm.Show();
            }
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            this.webCamForm?.Close();
        }

        private void UpdateTextBox(object sender, EventArgs e)
        {
            textBox?.Invoke((MethodInvoker)delegate ()
          {
              foreach (var coordinate in this.webCamForm?.faceInfo.coordinates)
              {
                  textBox.AppendText("(" + coordinate.X.ToString() + ", " + coordinate.Y.ToString() + ") ");
              }
              foreach (var direction in this.webCamForm?.faceInfo.directions)
              {
                  textBox.AppendText("Direction " + direction.ToString() + " \r\n");
              }
          });
        }
    }
}
