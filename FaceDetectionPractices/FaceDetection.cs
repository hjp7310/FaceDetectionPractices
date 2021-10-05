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

        public FaceInfo HeadPoseEstimate(Mat img, bool is_multiple_faces = true)
        {
            Array2D<RgbPixel> cimg = MatToRgbArray2D(img);
            FaceInfo face_info = new FaceInfo();

            var faces = this.faceDetector.Operator(cimg);
            List<FullObjectDetection> shapes = new List<FullObjectDetection>();

            foreach (var face in faces)
            {
                shapes.Add(this.poseModel.Detect(cimg, face));
                face_info.coordinates.Add(new System.Drawing.Point(face.Center.X, face.Center.Y));
                if (is_multiple_faces == false)
                {
                    break;
                }
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

                    List<Point3f> nose_end_point3d = new List<Point3f>();
                    Point2f[] nose_end_point2d;

                    nose_end_point3d.Add(new Point3f(0f, 0f, 1000.0f));
                    double[,] jacobian;
                    Cv2.ProjectPoints(nose_end_point3d, rotation_vector, translation_vector, camera_matrix, dist_coeffs.ToArray(), out nose_end_point2d, out jacobian);

                    for (int i = 0; i < image_points.Count; i++)
                    {
                        Cv2.Circle(img, new OpenCvSharp.Point(image_points[i].X, image_points[i].Y), 3, new Scalar(0, 0, 255), -1);
                    }
                    Cv2.Line(img, new OpenCvSharp.Point(image_points[0].X, image_points[0].Y), new OpenCvSharp.Point(nose_end_point2d[0].X, nose_end_point2d[0].Y), new Scalar(255, 0, 0), 2);
                    face_info.directions.Add(PointToDirection(new System.Drawing.Point((int)image_points[0].X, (int)image_points[0].Y), new System.Drawing.Point((int)nose_end_point2d[0].X, (int)nose_end_point2d[0].Y)));
                }
            }

            face_info.img = img;
            cimg.Dispose();
            return face_info;
        }

        public FaceInfo FaceCoordinate(Mat img, bool is_multiple_faces = true)
        {
            Array2D<RgbPixel> cimg = MatToRgbArray2D(img);
            FaceInfo face_info = new FaceInfo();

            var faces = this.faceDetector.Operator(cimg);
            foreach (var face in faces)
            {
                Cv2.Rectangle(img, new OpenCvSharp.Point(face.TopLeft.X, face.TopLeft.Y), new OpenCvSharp.Point(face.BottomRight.X, face.BottomRight.Y), new Scalar(0, 0, 255), 1);
                face_info.coordinates.Add(new System.Drawing.Point(face.Center.X, face.Center.Y));
                face_info.directions.Add(PointToDirection(new System.Drawing.Point((int)(img.Cols / 2), (int)(img.Rows / 2)), new System.Drawing.Point(face.Center.X, face.Center.Y)));
                if (is_multiple_faces == false)
                {
                    break;
                }
            }

            face_info.img = img;
            cimg.Dispose();
            return face_info;
        }

        public FaceInfo EyeTrack(Mat img, bool is_multiple_faces = true)
        {
            Array2D<RgbPixel> cimg = MatToRgbArray2D(img);
            FaceInfo faceInfo = new FaceInfo();

            var faces = this.faceDetector.Operator(cimg);
            List<FullObjectDetection> shapes = new List<FullObjectDetection>();

            foreach (var face in faces)
            {
                shapes.Add(this.poseModel.Detect(cimg, face));
                faceInfo.coordinates.Add(new System.Drawing.Point(face.Center.X, face.Center.Y));
                if (is_multiple_faces == false)
                {
                    break;
                }
            }

            uint[] left_eye = new uint[] { 36, 37, 38, 39, 40, 41 };
            uint[] right_eye = new uint[] { 42, 43, 44, 45, 46, 47 };
            uint[] eyesIdx = new uint[] { 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47 };

            if (faces.Length > 0)
            {
                foreach (var shape in shapes)
                {
                    Mat mask = Mat.Zeros(img.Size(), MatType.CV_8UC1);

                    int[] end_points_left = EyeOnMask(mask, left_eye, shape);
                    int[] end_points_right = EyeOnMask(mask, right_eye, shape);

                    Cv2.Dilate(mask, mask, Mat.Ones(9, 9, MatType.CV_8UC1));

                    Mat eyes = Mat.Zeros(img.Size(), MatType.CV_8UC3);
                    Cv2.BitwiseAnd(img, img, eyes, mask);
                    IntPtr ptr = eyes.Data;
                    for (int row = 0; row < eyes.Rows; row++)
                    {
                        for (int col = 0; col < eyes.Cols; col++)
                        {
                            byte b = Marshal.ReadByte(ptr, row * eyes.Cols * 3 + col * 3 + 0);
                            byte g = Marshal.ReadByte(ptr, row * eyes.Cols * 3 + col * 3 + 1);
                            byte r = Marshal.ReadByte(ptr, row * eyes.Cols * 3 + col * 3 + 2);
                            if ((mask.At<UInt16>(row, col) == 0) && b == 0 && g == 0 && r == 0)
                            {
                                Marshal.WriteByte(ptr, row * eyes.Cols * 3 + col * 3 + 0, 255);
                                Marshal.WriteByte(ptr, row * eyes.Cols * 3 + col * 3 + 1, 255);
                                Marshal.WriteByte(ptr, row * eyes.Cols * 3 + col * 3 + 2, 255);
                            }
                        }
                    }
                    int mid = (int)((shape.GetPart(42).X + shape.GetPart(39).X) / 2);
                    Mat eyes_gray = new Mat();
                    Cv2.CvtColor(eyes, eyes_gray, ColorConversionCodes.BGR2GRAY);
                    int threshold = 75;
                    Mat thresh = new Mat();
                    Cv2.Threshold(eyes_gray, thresh, threshold, 255, ThresholdTypes.Binary);
                    ProcessThresh(thresh);

                    OpenCvSharp.Point eyeball_pos_left = Contouring(thresh.ColRange(0, mid), mid, img, end_points_left);
                    OpenCvSharp.Point eyeball_pos_right = Contouring(thresh.ColRange(mid, thresh.Cols), mid, img, end_points_right);

                    Direction direction_left = PointToDirection(eyeball_pos_left, end_points_left);
                    Direction direction_right = PointToDirection(eyeball_pos_right, end_points_right);
                    if (direction_left == direction_right)
                    {
                        faceInfo.directions.Add(direction_left);
                    }
                }
            }

            foreach (var shape in shapes)
            {
                foreach (uint eye in eyesIdx)
                {
                    Cv2.Circle(img, new OpenCvSharp.Point(shape.GetPart(eye).X, shape.GetPart(eye).Y), 2, new Scalar(0, 255, 0), -1);
                }
            }

            faceInfo.img = img;
            cimg.Dispose();
            return faceInfo;
        }

        private Direction PointToDirection(OpenCvSharp.Point eyeball_pos, int[] end_points)
        {
            double x_ratio = ((double)eyeball_pos.X - (double)end_points[0]) / ((double)end_points[2] - (double)end_points[0]);
            double y_ratio = ((double)eyeball_pos.Y - (double)end_points[1]) / ((double)end_points[3] - (double)end_points[1]);

            if (x_ratio > 0.66)
                return Direction.Left;
            else if (x_ratio < 0.33)
                return Direction.Right;
            else
                return Direction.Center;
        }

        private OpenCvSharp.Point Contouring(Mat thresh, int mid, Mat img, int[] end_points)
        {
            Mat[] contours;
            Mat none = new Mat();
            Cv2.FindContours(thresh, out contours, none, RetrievalModes.External, ContourApproximationModes.ApproxNone);
            int cx = (end_points[0] + end_points[2]) / 2;
            int cy = (end_points[1] + end_points[3]) / 2;

            if (contours.Length > 0)
            {
                double max = 0;
                int max_idx = 0;
                for (int i = 0; i < contours.Length; i++)
                {
                    double val = Cv2.ContourArea(contours[i]);
                    if (max < val)
                    {
                        max = val;
                        max_idx = i;
                    }
                }

                Moments moments = Cv2.Moments(contours[max_idx]);
                cx = (int)(moments.M10 / moments.M00);
                cy = (int)(moments.M01 / moments.M00);

                if (mid < end_points[0])
                {
                    cx += mid;
                }

                Cv2.Circle(img, new OpenCvSharp.Point(cx, cy), 4, new Scalar(0, 0, 255), 2);
            }
            return new OpenCvSharp.Point(cx, cy);
        }

        private void ProcessThresh(Mat thresh)
        {
            Cv2.Erode(thresh, thresh, null, iterations: 2);
            Cv2.Dilate(thresh, thresh, null, iterations: 4);
            Cv2.MedianBlur(thresh, thresh, 3);
            Cv2.BitwiseNot(thresh, thresh);
        }

        private int[] EyeOnMask(Mat mask, uint[] eye, FullObjectDetection shape)
        {
            List<OpenCvSharp.Point> eye_points = new List<OpenCvSharp.Point>();
            foreach (var point in eye)
            {
                eye_points.Add(new OpenCvSharp.Point(shape.GetPart(point).X, shape.GetPart(point).Y));
            }
            Cv2.FillConvexPoly(mask, eye_points, 255);
            return new int[] { eye_points[0].X, (int)((eye_points[1].Y + eye_points[2].Y) / 2), eye_points[3].X, (int)((eye_points[4].Y + eye_points[5].Y) / 2) };
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
