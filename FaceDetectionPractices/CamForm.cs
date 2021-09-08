using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceDetectionPractices
{
    public partial class CamForm : Form
    {
        public event EventHandler coordinateChanged;

        private FaceDetection faceDetection;
        private VideoCapture videoCapture;

        private FaceInfo _faceInfo;
        public FaceInfo faceInfo
        {
            get { return this._faceInfo; }
            set
            {
                lock (this)
                {
                    this._faceInfo = value;
                }
                EventArgs args = new EventArgs();
                this.coordinateChanged(null, args);
            }
        }

        public CamForm()
        {
            InitializeComponent();
        }

        private void CamForm_Load(object sender, EventArgs e)
        {
            this.faceDetection = new FaceDetection();

            this.videoCapture = new VideoCapture();
            this.videoCapture.Open(0, VideoCaptureAPIs.ANY);
            if (!videoCapture.IsOpened())
            {
                Close();
                return;
            }

            pictureBox.Size = new System.Drawing.Size(this.videoCapture.FrameWidth, this.videoCapture.FrameHeight);
            ClientSize = new System.Drawing.Size(pictureBox.Width + 20, pictureBox.Height + 20);
            StartPosition = FormStartPosition.CenterScreen;
            backgroundWorker.RunWorkerAsync();
        }

        private void CamForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            backgroundWorker.CancelAsync();
            this.videoCapture?.Dispose();
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var bg_worker = (BackgroundWorker)sender;

            while (!bg_worker.CancellationPending)
            {
                using (var frame_mat = this.videoCapture.RetrieveMat())
                {
                    FaceInfo faceInfo = this.faceDetection.HeadPoseEstimate(frame_mat);
                    if (faceInfo.coordinates != null && faceInfo.directions != null)
                    {
                        this.faceInfo = faceInfo;
                    }
                    bg_worker.ReportProgress(0, BitmapConverter.ToBitmap(faceInfo.img));
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
