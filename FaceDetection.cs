using DlibDotNet;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FaceDetectionPractices
{
    static class FaceDetection
    {
        public static DlibDotNet.Rectangle[] FaceCoordinate(Mat img)
        {
            byte[] array = new byte[img.Width * img.Height * img.ElemSize()];
            Marshal.Copy(img.Data, array, 0, array.Length);
            Array2D<RgbPixel> cimg = Dlib.LoadImageData<RgbPixel>(array, (uint)img.Height, (uint)img.Width, (uint)(img.Width * img.ElemSize()));

            var detector = Dlib.GetFrontalFaceDetector();
            var faces = detector.Operator(cimg);

            cimg.Dispose();
            detector.Dispose();
            return faces;
        }

        public static Bitmap FaceCoordinateBitmap(Mat img)
        {
            Bitmap result = BitmapConverter.ToBitmap(img);
            Graphics grp = Graphics.FromImage(result);

            byte[] array = new byte[img.Width * img.Height * img.ElemSize()];
            Marshal.Copy(img.Data, array, 0, array.Length);
            Array2D<RgbPixel> cimg = Dlib.LoadImageData<RgbPixel>(array, (uint)img.Height, (uint)img.Width, (uint)(img.Width * img.ElemSize()));

            var detector = Dlib.GetFrontalFaceDetector();
            var faces = detector.Operator(cimg);
            foreach (var face in faces)
            {
                var rect = new System.Drawing.Rectangle(face.Left, face.Top, face.Right - face.Left, face.Bottom - face.Top);
                grp.DrawRectangle(new Pen(Color.Red), rect);
            }

            cimg.Dispose();
            detector.Dispose();
            return result;
        }
    }
}
