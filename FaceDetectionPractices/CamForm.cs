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

        private Tuple<int, int> _coordinate;
        public Tuple<int, int> coordinate
        {
            get { return this._coordinate; }
            set { lock (this) { 
                    this._coordinate = value;
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
            faceDetection = new FaceDetection();

            videoCapture = new VideoCapture();
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
                    //Bitmap frame_bitmap = BitmapConverter.ToBitmap(frame_mat);
                    //Graphics grp = Graphics.FromImage(frame_bitmap);
                    //
                    //var faces = faceDetection.FaceCoordinate(frame_mat);
                    //foreach (var face in faces)
                    //{
                    //    var rect = new Rectangle(face.Left, face.Top, face.Right - face.Left, face.Bottom - face.Top);
                    //    grp.DrawRectangle(new Pen(Color.Red), rect);
                    //    this.coordinate = new Tuple<int, int>(face.Center.X, face.Center.Y);
                    //}
                    //
                    //bg_worker.ReportProgress(0, frame_bitmap);

                    bg_worker.ReportProgress(0, BitmapConverter.ToBitmap(faceDetection.HeadPosition(frame_mat)));
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
