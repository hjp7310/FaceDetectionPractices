using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceDetectionPractices
{
    public enum Direction { Up, Down, Left, Right, Center };

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
