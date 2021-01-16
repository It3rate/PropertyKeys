using System;
using System.Collections.Generic;
using System.Drawing;

namespace Motive.Vis
{
	public class Line : Point, IPath, IPrimitive
	{
		public Point StartPoint => this;
        public Point MidPoint => GetPoint(0.5f, 0);
        public Point EndPoint { get; private set; }

        public Point Center => GetPoint(0.5f, 0);

        private float _length;
        public float Length
        {
            get
            {
                if (_length == 0)
                {
                    _length = (float)Math.Sqrt((EndPoint.X - X) * (EndPoint.X - X) + (EndPoint.Y - Y) * (EndPoint.Y - Y));
                }
                return _length;
            }
        }

        private Line(float startX, float startY, float endX, float endY) : base(startX, startY)
        {
            EndPoint = new Point(endX, endY);
        }
        private Line(Point start, Point endPoint) : base(start.X, start.Y)
        {
            EndPoint = endPoint;
        }

        public static Line ByCenter(float centerX, float centerY, float originX, float originY)
        {
            return new Line(centerX, centerY, originX, originY);
        }
        public static Line ByEndpoints(Point start, Point end)
        {
            return new Line(start, end);
        }
        public static Line ByEndpoints(float startX, float startY, float endX, float endY)
        {
            var start = new Point(startX, startY);
            var end = new Point(endX, endY);
            return new Line(start, end);
        }

        public Point GetPoint(float position, float offset = 0)
        {
            var xOffset = 0f;
            var yOffset = 0f;
            var xDif = EndPoint.X - X;
            var yDif = EndPoint.Y - Y;
            if (offset != 0)
            {
                var ang = (float)(Math.Atan2(yDif, xDif));
                xOffset = (float)(-Math.Sin(ang) * Math.Abs(offset) * Math.Sign(-offset));
                yOffset = (float)(Math.Cos(ang) * Math.Abs(offset) * Math.Sign(-offset));
            }
            return new Point(X + xDif * position + xOffset, Y + yDif * position - yOffset);
        }

        public Point GetPointFromCenter(float centeredPosition, float offset = 0)
        {
            return GetPoint(centeredPosition * 2f - 1f, offset);
        }

        public Node NodeAt(float position) => new Node(this, position);
        public Node NodeAt(float position, float offset) => new TipNode(this, position, offset);
        public Node StartNode => new Node(this, 0f);
        public Node MidNode => new Node(this, 0.5f);
        public Node EndNode => new Node(this, 1f);

        public Point IntersectionPoint(Line line) => null;
        public Circle CircleFrom() => new Circle(this, EndPoint);
        public Quad RectangleFrom() => new Quad(this, EndPoint);


        public Point ProjectPointOnto(Point p)
        {
            var e1 = EndPoint.Subtract(this);
            var e2 = p.Subtract(this);
            var dp = e1.DotProduct(e2);
            var len2 = e1.VectorSquaredLength();
            return new Point(X + (dp * e1.X) / len2, Y + (dp * e1.Y) / len2);
            //   var e1 = EndPoint.Subtract(StartPoint);
            //var e2 = p.Subtract(StartPoint);
            //var dp = e1.DotProduct(e2);
            //var len2 = e1.VectorSquaredLength();
            //return new Point(StartPoint.X + (dp * e1.X) / len2, StartPoint.Y + (dp * e1.Y) / len2);
        }

        public Point[] GetPolylinePoints(int pointCount = 24)
        {
            var result = new List<Point>() { StartPoint, EndPoint };
            return result.ToArray();
        }

        public override string ToString()
        {
            return String.Format("Ln:{0:0.##},{1:0.##} {2:0.##},{3:0.##}", X, Y, EndPoint.X, EndPoint.Y);
        }
    }
}