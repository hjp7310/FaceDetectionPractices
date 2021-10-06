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

        public FaceInfo HeadPoseEstimate(Mat img, bool isMultipleFaces = true)
        {
            Array2D<RgbPixel> cimg = MatToRgbArray2D(img);
            FaceInfo faceInfo = new FaceInfo();

            var faces = this.faceDetector.Operator(cimg);
            List<FullObjectDetection> shapes = new List<FullObjectDetection>();

            foreach (var face in faces)
            {
                shapes.Add(this.poseModel.Detect(cimg, face));
                faceInfo.coordinates.Add(new System.Drawing.Point(face.Center.X, face.Center.Y));
                if (isMultipleFaces == false)
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
                    Point2f[] imagePoints = new Point2f[] {
                        new Point2f(shape.GetPart(30).X, shape.GetPart(30).Y),      // Nose tip
                        new Point2f(shape.GetPart(8).X, shape.GetPart(8).Y),        // Chin
                        new Point2f(shape.GetPart(36).X, shape.GetPart(36).Y),      // Left eye left corner
                        new Point2f(shape.GetPart(45).X, shape.GetPart(45).Y),      // Right eye right corner
                        new Point2f(shape.GetPart(48).X, shape.GetPart(48).Y),      // Left Mouth corner
                        new Point2f(shape.GetPart(54).X, shape.GetPart(54).Y),      // Right mouth corner
                    };

                    Point3f[] modelPoints = new Point3f[] {
                        new Point3f(0.0f, 0.0f, 0.0f),              // Nose tip
                        new Point3f(0.0f, -330.0f, -65.0f),         // Chin
                        new Point3f(-225.0f, 170.0f, -135.0f),      // Left eye left corner
                        new Point3f(225.0f, 170.0f, -135.0f),       // Right eye right corner
                        new Point3f(-150.0f, -150.0f, -125.0f),     // Left Mouth corner
                        new Point3f(150.0f, -150.0f, -125.0f)       // Right mouth corner
                    };

                    double focalLength = img.Cols;
                    Point2f center = new Point2f(img.Cols / 2, img.Rows / 2);
                    double[,] cameraMatrix = new double[3, 3] { { focalLength, 0, center.X }, { 0, focalLength, center.Y }, { 0, 0, 1 } };
                    double[] distCoeffs = new double[] { 0, 0, 0, 0 };

                    double[] rotationVector = { 0, 0, 0 };
                    double[] translationVector = { 0, 0, 0 };

                    Cv2.SolvePnP(modelPoints, imagePoints, cameraMatrix, distCoeffs, ref rotationVector, ref translationVector);

                    Point3f[] noseEndPoint3D = new Point3f[] { new Point3f(0f, 0f, 1000.0f) };
                    Point2f[] noseEndPoint2D;
                    double[,] jacobian;
                    Cv2.ProjectPoints(noseEndPoint3D, rotationVector, translationVector, cameraMatrix, distCoeffs.ToArray(), out noseEndPoint2D, out jacobian);

                    for (int i = 0; i < imagePoints.Length; i++)
                    {
                        Cv2.Circle(img, new OpenCvSharp.Point(imagePoints[i].X, imagePoints[i].Y), 3, new Scalar(0, 0, 255), -1);
                    }
                    Cv2.Line(img, new OpenCvSharp.Point(imagePoints[0].X, imagePoints[0].Y), new OpenCvSharp.Point(noseEndPoint2D[0].X, noseEndPoint2D[0].Y), new Scalar(255, 0, 0), 2);
                    faceInfo.directions.Add(PointToDirection(new OpenCvSharp.Point((int)imagePoints[0].X, (int)imagePoints[0].Y), new OpenCvSharp.Point((int)noseEndPoint2D[0].X, (int)noseEndPoint2D[0].Y)));
                }
            }

            faceInfo.img = img;
            cimg.Dispose();
            return faceInfo;
        }

        public FaceInfo FaceCoordinate(Mat img, bool isMultipleFaces = true)
        {
            Array2D<RgbPixel> cimg = MatToRgbArray2D(img);
            FaceInfo faceInfo = new FaceInfo();

            var faces = this.faceDetector.Operator(cimg);
            foreach (var face in faces)
            {
                Cv2.Rectangle(img, new OpenCvSharp.Point(face.TopLeft.X, face.TopLeft.Y), new OpenCvSharp.Point(face.BottomRight.X, face.BottomRight.Y), new Scalar(0, 0, 255), 1);
                faceInfo.coordinates.Add(new System.Drawing.Point(face.Center.X, face.Center.Y));
                faceInfo.directions.Add(PointToDirection(new OpenCvSharp.Point((int)(img.Cols / 2), (int)(img.Rows / 2)), new OpenCvSharp.Point(face.Center.X, face.Center.Y)));
                if (isMultipleFaces == false)
                {
                    break;
                }
            }

            faceInfo.img = img;
            cimg.Dispose();
            return faceInfo;
        }

        public FaceInfo EyeTrack(Mat img, bool isMultipleFaces = true)
        {
            Array2D<RgbPixel> cimg = MatToRgbArray2D(img);
            FaceInfo faceInfo = new FaceInfo();

            var faces = this.faceDetector.Operator(cimg);
            List<FullObjectDetection> shapes = new List<FullObjectDetection>();

            foreach (var face in faces)
            {
                shapes.Add(this.poseModel.Detect(cimg, face));
                faceInfo.coordinates.Add(new System.Drawing.Point(face.Center.X, face.Center.Y));
                if (isMultipleFaces == false)
                {
                    break;
                }
            }

            if (faces.Length > 0)
            {
                foreach (var shape in shapes)
                {
                    OpenCvSharp.Point[] eyePointsLeft = new OpenCvSharp.Point[] {
                        new OpenCvSharp.Point(shape.GetPart(36).X, shape.GetPart(36).Y),
                        new OpenCvSharp.Point(shape.GetPart(37).X, shape.GetPart(37).Y),
                        new OpenCvSharp.Point(shape.GetPart(38).X, shape.GetPart(38).Y),
                        new OpenCvSharp.Point(shape.GetPart(39).X, shape.GetPart(39).Y),
                        new OpenCvSharp.Point(shape.GetPart(40).X, shape.GetPart(40).Y),
                        new OpenCvSharp.Point(shape.GetPart(41).X, shape.GetPart(41).Y)
                    };

                    OpenCvSharp.Point[] eyePointsRight = new OpenCvSharp.Point[] {
                        new OpenCvSharp.Point(shape.GetPart(42).X, shape.GetPart(42).Y),
                        new OpenCvSharp.Point(shape.GetPart(43).X, shape.GetPart(43).Y),
                        new OpenCvSharp.Point(shape.GetPart(44).X, shape.GetPart(44).Y),
                        new OpenCvSharp.Point(shape.GetPart(45).X, shape.GetPart(45).Y),
                        new OpenCvSharp.Point(shape.GetPart(46).X, shape.GetPart(46).Y),
                        new OpenCvSharp.Point(shape.GetPart(47).X, shape.GetPart(47).Y)
                    };

                    Mat mask = Mat.Zeros(img.Size(), MatType.CV_8UC1);
                    OpenCvSharp.Point[] endPointsLeft = EyeOnMask(mask, eyePointsLeft, shape);    //(left, top), (right, bottom)
                    OpenCvSharp.Point[] endPointsRight = EyeOnMask(mask, eyePointsRight, shape);  //(left, top), (right, bottom)
                    Cv2.Dilate(mask, mask, Mat.Ones(9, 9, MatType.CV_8UC1));

                    Mat eyes = MaskImage(img, mask);
                    Cv2.CvtColor(eyes, eyes, ColorConversionCodes.BGR2GRAY);

                    int midCol = (int)((shape.GetPart(42).X + shape.GetPart(39).X) / 2);

                    Mat thresholdLeft = OptimalThresholdImage(eyes.ColRange(0, midCol), mask.ColRange(0, midCol));
                    Mat thresholdRight = OptimalThresholdImage(eyes.ColRange(midCol, eyes.Cols), mask.ColRange(midCol, mask.Cols));

                    OpenCvSharp.Point eyeballPointLeft = Contouring(thresholdLeft, midCol, endPointsLeft);
                    OpenCvSharp.Point eyeballPointRight = Contouring(thresholdRight, midCol, endPointsRight);

                    Cv2.Circle(img, new OpenCvSharp.Point(eyeballPointLeft.X, eyeballPointLeft.Y), 4, new Scalar(255, 0, 0), 2);
                    Cv2.Circle(img, new OpenCvSharp.Point(eyeballPointRight.X, eyeballPointRight.Y), 4, new Scalar(255, 0, 0), 2);
                    
                    Direction directionLeft = PointToDirection(eyeballPointLeft, endPointsLeft);
                    Direction directionRight = PointToDirection(eyeballPointRight, endPointsRight);
                    if (directionLeft == directionRight)
                    {
                        faceInfo.directions.Add(directionLeft);
                    }               
                }
            }

            foreach (var shape in shapes)
            {
                for (uint i = 0; i < 68; i++)
                {
                    Cv2.Circle(img, new OpenCvSharp.Point(shape.GetPart(i).X, shape.GetPart(i).Y), 2, new Scalar(0, 255, 0), -1);
                }
            }

            faceInfo.img = img;
            cimg.Dispose();
            return faceInfo;
        }

        private Array2D<RgbPixel> MatToRgbArray2D(Mat img)
        {
            byte[] array = new byte[img.Width * img.Height * img.ElemSize()];
            Marshal.Copy(img.Data, array, 0, array.Length);
            Array2D<RgbPixel> cimg = Dlib.LoadImageData<RgbPixel>(array, (uint)img.Height, (uint)img.Width, (uint)(img.Width * img.ElemSize()));
            return cimg;
        }

        private Direction PointToDirection(OpenCvSharp.Point point1, OpenCvSharp.Point point2)
        {
            Direction direction;
            double angle = Math.Atan2((-point2.Y) - (-point1.Y), point2.X - point1.X) * 180f / Math.PI;

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

        private OpenCvSharp.Point[] EyeOnMask(Mat mask, OpenCvSharp.Point[] eyePoints, FullObjectDetection shape)
        {
            Cv2.FillConvexPoly(mask, eyePoints, 255);

            OpenCvSharp.Point topLeft = new OpenCvSharp.Point(eyePoints[0].X, (int)((eyePoints[1].Y + eyePoints[2].Y) / 2));
            OpenCvSharp.Point bottomRight = new OpenCvSharp.Point(eyePoints[3].X, (int)((eyePoints[4].Y + eyePoints[5].Y) / 2));
            return new OpenCvSharp.Point[] { topLeft, bottomRight };
        }

        private Mat MaskImage(Mat img, Mat mask)
        {
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

            return eyes;
        }

        private Mat OptimalThresholdImage(Mat eye, Mat mask)
        {
            Mat threshold = ProcessThreshold(eye, 75);  //default
            int eyeAreaSize = Cv2.CountNonZero(mask);

            int thresholdValMax = 255;
            int thresholdValMin = 0;
            int thresholdVal;

            while (thresholdValMax < thresholdValMin)
            {
                thresholdVal = (thresholdValMax + thresholdValMin) / 2;

                threshold = ProcessThreshold(eye, thresholdVal);
                int eyeThresholdSize = Cv2.CountNonZero(threshold);

                double thresholdRatio = (double)eyeThresholdSize / (double)eyeAreaSize;
                if (thresholdRatio > 0.2 && thresholdRatio < 0.3)
                    break;
                else if (thresholdRatio <= 0.2)
                    thresholdValMin = thresholdVal + 1;
                else
                    thresholdValMax = thresholdVal - 1;
            }

            return threshold;
        }

        private Mat ProcessThreshold(Mat eyes, double thresholdVal)
        {
            Mat threshold = new Mat();
            Cv2.Threshold(eyes, threshold, thresholdVal, 255, ThresholdTypes.Binary);

            Cv2.Erode(threshold, threshold, null, iterations: 2);
            Cv2.Dilate(threshold, threshold, null, iterations: 4);
            Cv2.MedianBlur(threshold, threshold, 3);
            Cv2.BitwiseNot(threshold, threshold);

            return threshold;
        }

        private OpenCvSharp.Point Contouring(Mat threshold, int mid, OpenCvSharp.Point[] endPoints)
        {
            Mat[] contours;
            Mat none = new Mat();
            Cv2.FindContours(threshold, out contours, none, RetrievalModes.External, ContourApproximationModes.ApproxNone);
            int cx = (endPoints[0].X + endPoints[1].X) / 2;
            int cy = (endPoints[0].Y + endPoints[1].Y) / 2;

            if (contours.Length > 0)
            {
                Moments moments = Cv2.Moments(contours.Aggregate((c1, c2) => Cv2.ContourArea(c1) > Cv2.ContourArea(c2) ? c1 : c2));
                cx = (int)(moments.M10 / moments.M00);
                cy = (int)(moments.M01 / moments.M00);

                if (mid < endPoints[0].X)
                {
                    cx += mid;
                }
            }
            return new OpenCvSharp.Point(cx, cy);
        }

        private Direction PointToDirection(OpenCvSharp.Point eyeballPoint, OpenCvSharp.Point[] endPoints)
        {
            double xRatio = ((double)eyeballPoint.X - (double)endPoints[0].X) / ((double)endPoints[1].X - (double)endPoints[0].X);
            double yRatio = ((double)eyeballPoint.Y - (double)endPoints[0].Y) / ((double)endPoints[1].Y - (double)endPoints[0].Y);

            if (xRatio > 0.66)
                return Direction.Left;
            else if (xRatio < 0.33)
                return Direction.Right;
            else
                return Direction.Center;
        }
    }
}
