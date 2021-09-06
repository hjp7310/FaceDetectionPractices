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
    public class FaceDetection
    {
        private FrontalFaceDetector faceDetector;
        private ShapePredictor poseModel;

        public FaceDetection()
        {
            faceDetector = Dlib.GetFrontalFaceDetector();
            poseModel = ShapePredictor.Deserialize("shape_predictor_68_face_landmarks.dat");
        }

        public Mat HeadPosition(Mat img)
        {
            byte[] array = new byte[img.Width * img.Height * img.ElemSize()];
            Marshal.Copy(img.Data, array, 0, array.Length);
            Array2D<BgrPixel> cimg = Dlib.LoadImageData<BgrPixel>(array, (uint)img.Height, (uint)img.Width, (uint)(img.Width * img.ElemSize()));

            var faces = this.faceDetector.Operator(cimg);
            List<FullObjectDetection> shapes = new List<FullObjectDetection>();

            foreach (var face in faces)
            {
                shapes.Add(this.poseModel.Detect(cimg, face));
            }

            //foreach (var shape in shapes)
            //{
            //    for (uint i = 0; i < 68; i++)
            //    {
            //        Cv2.Circle(img, new OpenCvSharp.Point(shape.GetPart(i).X, shape.GetPart(i).Y), 2, new Scalar(0, 0, 255), -1);
            //    }
            //}

            if (faces.Length > 0)
            {
                List<Point2f> image_points = new List<Point2f>();
                image_points.Add(new Point2f(shapes[0].GetPart(30).X, shapes[0].GetPart(30).Y));    // Nose tip
                image_points.Add(new Point2f(shapes[0].GetPart(8).X, shapes[0].GetPart(8).Y));      // Chin
                image_points.Add(new Point2f(shapes[0].GetPart(36).X, shapes[0].GetPart(36).Y));    // Left eye left corner
                image_points.Add(new Point2f(shapes[0].GetPart(45).X, shapes[0].GetPart(45).Y));    // Right eye right corner
                image_points.Add(new Point2f(shapes[0].GetPart(48).X, shapes[0].GetPart(48).Y));    // Left Mouth corner
                image_points.Add(new Point2f(shapes[0].GetPart(54).X, shapes[0].GetPart(54).Y));    // Right mouth corner

                List<Point3f> model_points = new List<Point3f>();
                model_points.Add(new Point3f(0.0f, 0.0f, 0.0f));            // Nose tip
                model_points.Add(new Point3f(0.0f, -330.0f, -65.0f));       // Chin
                model_points.Add(new Point3f(-225.0f, 170.0f, -135.0f));    // Left eye left corner
                model_points.Add(new Point3f(225.0f, 170.0f, -135.0f));     // Right eye right corner
                model_points.Add(new Point3f(-150.0f, -150.0f, -125.0f));   // Left Mouth corner
                model_points.Add(new Point3f(150.0f, -150.0f, -125.0f));    // Right mouth corner

                double focal_length = img.Cols;
                Point2f center = new Point2f(img.Cols / 2, img.Rows / 2);
                double[,] camera_matrix = new double[3, 3] { { focal_length, 0, center.X }, { 0, focal_length, center.Y }, { 0, 0, 1 } };
                List<double> dist_coeffs = new List<double>() { 0, 0, 0, 0 };

                double[] rotation_vector = { 0, 0, 0 };
                double[] translation_vector = { 0, 0, 0 };

                Cv2.SolvePnP(model_points, image_points, camera_matrix, dist_coeffs, ref rotation_vector, ref translation_vector);

                List<Point3f> nose_end_point3D = new List<Point3f>();
                Point2f[] nose_end_point2D;

                nose_end_point3D.Add(new Point3f(0f, 0f, 1000.0f));
                double[,] jacobian;
                Cv2.ProjectPoints(nose_end_point3D, rotation_vector, translation_vector, camera_matrix, dist_coeffs.ToArray(), out nose_end_point2D, out jacobian);

                for (int i = 0; i < image_points.Count; i++)
                {
                    Cv2.Circle(img, new OpenCvSharp.Point(image_points[i].X, image_points[i].Y), 3, new Scalar(0, 0, 255), -1);
                }
                Cv2.Line(img, new OpenCvSharp.Point(image_points[0].X, image_points[0].Y), new OpenCvSharp.Point(nose_end_point2D[0].X, nose_end_point2D[0].Y), new Scalar(255, 0, 0), 2);
            }
            
            cimg.Dispose();
            return img;
        }

        public Mat FaceCoordinate(Mat img)
        {
            byte[] array = new byte[img.Width * img.Height * img.ElemSize()];
            Marshal.Copy(img.Data, array, 0, array.Length);
            Array2D<RgbPixel> cimg = Dlib.LoadImageData<RgbPixel>(array, (uint)img.Height, (uint)img.Width, (uint)(img.Width * img.ElemSize()));

            var faces = this.faceDetector.Operator(cimg);
            foreach (var face in faces)
            {
                Cv2.Rectangle(img, new OpenCvSharp.Point(face.TopLeft.X, face.TopLeft.Y), new OpenCvSharp.Point(face.BottomRight.X, face.BottomRight.Y), new Scalar(0, 0, 255), 1);
            }

            cimg.Dispose();
            return img;
        }
    }
}
