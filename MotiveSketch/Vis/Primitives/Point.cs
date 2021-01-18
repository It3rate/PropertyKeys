using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using Motive.Graphic;
using Motive.Samplers;
using Motive.SeriesData;
using Motive.Stores;

namespace Motive.Vis
{
	public class Point : FloatSeries, IPrimitive, IDrawableSeries
    {
		protected const float pi2 = (float)(Math.PI * 2.0);

        public Point(float x, float y) : base(2, new []{x,y})
		{
        }
        public Point(Point p) : base(2, new[] { p.X, p.Y })
        {
        }

        public Point(int vectorSize, params float[] values) : base(vectorSize, values) {}

       // public Point Sample(Gaussian g) => new Point(X + (float)g.Sample(), Y + (float)g.Sample());
        public virtual float Similarity(IPrimitive p) => 0;
        public Point Transpose() => new Point(Y, X);
        public Point Abs() => new Point(Math.Abs(X), Math.Abs(Y));
        public Point Add(Point pt) => new Point(X + pt.X, Y + pt.Y);
        public Point Subtract(Point pt) => new Point(X - pt.X, Y - pt.Y);
        public Point Multiply(Point pt) => new Point(X * pt.X, Y * pt.Y);
        public Point MidPointOf(Point pt) => new Point((pt.X - X) / 2f + X, (pt.Y - Y) / 2f + Y);
        public Point Multiply(float scalar) => new Point(X * scalar, Y * scalar);
        public Point DivideBy(float scalar) => new Point(X / scalar, Y / scalar);

        public float VectorLength() => (float)Math.Sqrt(X * X + Y * Y);
        public float VectorSquaredLength() => X * X + Y * Y;
        public float DistanceTo(Point pt) => (float)Math.Sqrt((pt.X - X) * (pt.X - X) + (pt.Y - Y) * (pt.Y - Y));
        public float SquaredDistanceTo(Point pt) => (pt.X - X) * (pt.X - X) + (pt.Y - Y) * (pt.Y - Y);
        public float DotProduct(Point pt) => (X * pt.X) + (Y * pt.Y); // negative because inverted Y
        public float Atan2(Point pt) => (float)Math.Atan2(pt.Y - Y, pt.X - X);

        public PointF PointF => new PointF(X, Y);

        public LinearDirection LinearDirection(Point pt)
        {
            // make this return probability as well
            LinearDirection result;
            var dir = Math.Atan2(pt.Y - Y, pt.X - X);
            var pi8 = Math.PI / 8f;
            if (dir < -(pi8 * 7))
            {
                result = Vis.LinearDirection.Horizontal;
            }
            else if (dir < -(pi8 * 5))
            {
                result = Vis.LinearDirection.TRDiagonal;
            }
            else if (dir < -(pi8 * 3))
            {
                result = Vis.LinearDirection.Vertical;
            }
            else if (dir < -(pi8 * 1))
            {
                result = Vis.LinearDirection.TLDiagonal;
            }
            else if (dir < (pi8 * 1))
            {
                result = Vis.LinearDirection.Horizontal;
            }
            else if (dir < (pi8 * 3))
            {
                result = Vis.LinearDirection.TRDiagonal;
            }
            else if (dir < (pi8 * 5))
            {
                result = Vis.LinearDirection.Vertical;
            }
            else if (dir < (pi8 * 7))
            {
                result = Vis.LinearDirection.TLDiagonal;
            }
            else
            {
                result = Vis.LinearDirection.Horizontal;
            }

            return result;
        }

        public CompassDirection DirectionFrom(Point pt)
        {
            // make this return probability as well
            CompassDirection result;
            var dir = Math.Atan2(Y - pt.Y, X - pt.X);
            var pi8 = Math.PI / 8f;
            if (dir < -(pi8 * 7))
            {
                result = CompassDirection.W;
            }
            else if (dir < -(pi8 * 5))
            {
                result = CompassDirection.SW;
            }
            else if (dir < -(pi8 * 3))
            {
                result = CompassDirection.S;
            }
            else if (dir < -(pi8 * 1))
            {
                result = CompassDirection.SE;
            }
            else if (dir < (pi8 * 1))
            {
                result = CompassDirection.E;
            }
            else if (dir < (pi8 * 3))
            {
                result = CompassDirection.NE;
            }
            else if (dir < (pi8 * 5))
            {
                result = CompassDirection.N;
            }
            else if (dir < (pi8 * 7))
            {
                result = CompassDirection.NW;
            }
            else
            {
                result = CompassDirection.W;
            }

            return result;
        }
        public Point ProjectedOntoLine(Line line)
        {
            return line.ProjectPointOnto(this);
        }

        public override string ToString()
        {
            return String.Format("Pt:{0:0.##},{1:0.##}", X, Y);
        }

        public static float DotSize { get; set; } = 0.0001f;
        public virtual void AppendToGraphicsPath(GraphicsPath path)
        {
	        path.AddEllipse(X, Y, DotSize, DotSize); // a point doesn't actually exist, but this is for scaffolding so rely on the line width.
        }
    }
}