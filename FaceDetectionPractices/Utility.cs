using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceDetectionPractices
{
    public enum AIOption { EyeTrack, HeadCoordinate, HeadPoseEstimate }

    public class WebcamCustomEventArgs : EventArgs
    {
        public FaceInfo faceInfo { get; set; }

        public WebcamCustomEventArgs(FaceInfo faceInfo)
        {
            this.faceInfo = faceInfo;
        }
    }

    public enum Direction { Up, Down, Left, Right, Center, Error };

    public class FaceInfo
    {
        public OpenCvSharp.Mat img;
        public List<Point> coordinates;
        public List<Direction> directions;

        public FaceInfo()
        {
            this.coordinates = new List<Point>();
            this.directions = new List<Direction>();
        }
    }
}
