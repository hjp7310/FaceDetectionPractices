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

        public FaceInfo HeadPoseEstimate(Mat img)
        {
            Array2D<RgbPixel> cimg = MatToRgbArray2D(img);
            FaceInfo faceInfo = new FaceInfo();

            var faces = this.faceDetector.Operator(cimg);
            List<FullObjectDetection> shapes = new List<FullObjectDetection>();

            foreach (var face in faces)
            {
                shapes.Add(this.poseModel.Detect(cimg, face));
                faceInfo.coordinates.Add(new System.Drawing.Point(face.Center.X, face.Center.Y));
            }

            foreach (var shape in shapes)
            {
                for (uint i = 0; i < 68; i++)
                {
                    Cv2.Circle(img, new OpenCvSharp.Point(shape.GetPart(i).X, shape.GetPart(i).Y), 2, new Scalar(0, 255, 0), -1);
                }
            }

            if (faces.Length > 0)
            {
                foreach (var shape in shapes)
                {
                    List<Point2f> image_points = new List<Point2f>();
                    image_points.Add(new Point2f(shape.GetPart(30).X, shape.GetPart(30).Y));    // Nose tip
                    image_points.Add(new Point2f(shape.GetPart(8).X, shape.GetPart(8).Y));      // Chin
                    image_points.Add(new Point2f(shape.GetPart(36).X, shape.GetPart(36).Y));    // Left eye left corner
                    image_points.Add(new Point2f(shape.GetPart(45).X, shape.GetPart(45).Y));    // Right eye right corner
                    image_points.Add(new Point2f(shape.GetPart(48).X, shape.GetPart(48).Y));    // Left Mouth corner
                    image_points.Add(new Point2f(shape.GetPart(54).X, shape.GetPart(54).Y));    // Right mouth corner

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
                    faceInfo.directions.Add(PointToDirection(new System.Drawing.Point((int)image_points[0].X, (int)image_points[0].Y), new System.Drawing.Point((int)nose_end_point2D[0].X, (int)nose_end_point2D[0].Y)));
                }
            }

            faceInfo.img = img;
            cimg.Dispose();
            return faceInfo;
        }

        public Mat FaceCoordinate(Mat img)
        {
            Array2D<RgbPixel> cimg = MatToRgbArray2D(img);

            var faces = this.faceDetector.Operator(cimg);
            foreach (var face in faces)
            {
                Cv2.Rectangle(img, new OpenCvSharp.Point(face.TopLeft.X, face.TopLeft.Y), new OpenCvSharp.Point(face.BottomRight.X, face.BottomRight.Y), new Scalar(0, 0, 255), 1);
            }

            cimg.Dispose();
            return img;
        }

        private Array2D<RgbPixel> MatToRgbArray2D(Mat img)
        {
            byte[] array = new byte[img.Width * img.Height * img.ElemSize()];
            Marshal.Copy(img.Data, array, 0, array.Length);
            Array2D<RgbPixel> cimg = Dlib.LoadImageData<RgbPixel>(array, (uint)img.Height, (uint)img.Width, (uint)(img.Width * img.ElemSize()));
            return cimg;
        }

        private Direction PointToDirection(System.Drawing.Point p1, System.Drawing.Point p2)
        {
            Direction direction;
            double angle = Math.Atan2((-p2.Y) - (-p1.Y), p2.X - p1.X) * 180f / Math.PI;

            if (angle < 0)
            {
                angle = angle + 360f;
            }

            if (angle < 45f)
                direction = Direction.Left;
            else if (angle < 135f)
                direction = Direction.Up;
            else if (angle < 225f)
                direction = Direction.Right;
            else if (angle < 315f)
                direction = Direction.Down;
            else
                direction = Direction.Left;

            return direction;
        }
    }
}
