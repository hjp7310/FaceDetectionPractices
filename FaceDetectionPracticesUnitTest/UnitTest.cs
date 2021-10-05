using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using FaceDetectionPractices;
using OpenCvSharp;
using System.Drawing;

namespace FaceDetectionPracticesUnitTest
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void HeadPoseEstimateTest()
        {
            Mat img = Cv2.ImRead("img.jpg");
            var faceDetection = new FaceDetection();
            faceDetection.HeadPoseEstimate(img);
            img.SaveImage(@"output_HeadPoseEstimate.jpg");
        }

        [TestMethod]
        public void FaceCoordinateTest()
        {
            Mat img = Cv2.ImRead("img.jpg");
            var faceDetection = new FaceDetection();
            faceDetection.FaceCoordinate(img);
            img.SaveImage(@"output_FaceCoordinate.jpg");
        }

        [TestMethod]
        public void EyeTrackingTest()
        {
            Mat img = Cv2.ImRead("img.jpg");
            var faceDetection = new FaceDetection();
            faceDetection.EyeTrack(img);
            img.SaveImage(@"output_EyeTracking.jpg");
        }
    }
}
