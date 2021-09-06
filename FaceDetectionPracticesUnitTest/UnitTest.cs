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
        public void HeadPositionTest()
        {
            Mat img = Cv2.ImRead("img.jpg");
            var faceDetection = new FaceDetection();
            faceDetection.HeadPosition(img);
            img.SaveImage(@"output_HeadPosition.jpg");
        }

        [TestMethod]
        public void FaceCoordinateTest()
        {
            Mat img = Cv2.ImRead("img.jpg");
            var faceDetection = new FaceDetection();
            faceDetection.FaceCoordinate(img);
            img.SaveImage(@"output_FaceCoordinate.jpg");
        }
    }
}
