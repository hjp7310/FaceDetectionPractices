using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceDetectionPractices
{
    public partial class CamForm : Form
    {
        private readonly VideoCapture videoCapture;
        public CamForm()
        {
            InitializeComponent();
            videoCapture = new VideoCapture();
        }

        private void CamForm_Load(object sender, EventArgs e)
        {
            videoCapture.Open(0, VideoCaptureAPIs.ANY);
            if(!videoCapture.IsOpened())
            {
                Close();
                return;
            }

            pictureBox.Size = new System.Drawing.Size(videoCapture.FrameWidth, videoCapture.FrameHeight);
            ClientSize = new System.Drawing.Size(pictureBox.Width + 20, pictureBox.Height + 20);
            StartPosition = FormStartPosition.CenterScreen;
            backgroundWorker.RunWorkerAsync();
        }

        private void CamForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            backgroundWorker.CancelAsync();
            videoCapture.Dispose();
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var bg_worker = (BackgroundWorker)sender;

            while(!bg_worker.CancellationPending)
            {
                using (var frame_mat = videoCapture.RetrieveMat())
                {
                    var frame_bitmap = BitmapConverter.ToBitmap(frame_mat);
                    bg_worker.ReportProgress(0, frame_bitmap);
                }
                Thread.Sleep(100);
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var frame_bitmap = (Bitmap)e.UserState;
            pictureBox.Image?.Dispose();
            pictureBox.Image = frame_bitmap;
        }
    }
}
