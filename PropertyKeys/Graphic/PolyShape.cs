using DataArcs.Stores;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace DataArcs.Graphic
{
    public class PolyShape : BaseGraphic
    {
        public readonly BezierStore[] BezierStores;
        private BezierStore polygon;

        private float _orientation;
        private int _pointCount;
        private float _starness;
        private float _roundness;
        float _radius;

        public PolyShape(float orientation = 0, int pointCount = 4, float roundness = 0, float radius = 10f)
        {
            Orientation = orientation;
            PointCount = pointCount;
            Roundness = roundness;
            Radius = radius;
        }

        public float Orientation
        {
            get => _orientation;
            set
            {
                if (Math.Abs(_orientation - value) > DataUtils.TOLERANCE)
                {
                    _orientation = value;
                    GeneratePolyShape();
                }
            }
        }

        public int PointCount
        {
            get => _pointCount;
            set
            {
                if (Math.Abs(_pointCount - value) > DataUtils.TOLERANCE)
                {
                    _pointCount = value;
                    GeneratePolyShape();
                }
            }
        }

        public float Starness
        {
            get => _starness;
            set
            {
                if (Math.Abs(_starness - value) > DataUtils.TOLERANCE)
                {
                    _starness = value;
                    GeneratePolyShape();
                }
            }
        }

        public float Roundness
        {
            get => _roundness;
            set
            {
                if (Math.Abs(_roundness - value) > DataUtils.TOLERANCE)
                {
                    _roundness = value;
                    GeneratePolyShape();
                }
            }
        }

        public float Radius
        {
            get => _radius;
            set
            {
                if (Math.Abs(_radius - value) > DataUtils.TOLERANCE)
                {
                    _radius = value;
                    GeneratePolyShape();
                }
            }
        }


        public override void Draw(Graphics g, Brush brush, Pen pen, float t)
        {
            if (brush != null)
            {
                g.FillPath(brush, polygon.Path);
            }

            if (pen != null)
            {
                g.DrawPath(pen, polygon.Path);
            }
        }
        public void GeneratePolyShape()
        {
            polygon = GeneratePolyShape(Orientation, PointCount, Roundness, Radius, Starness);
        }
        public static BezierStore GeneratePolyShape(float orientation, int pointCount, float roundness, float radius, float starness)
        {
            bool hasStarness = Math.Abs(starness) > 0.001f;
            int count = hasStarness ? pointCount * 2 : pointCount;
            int pointsPerStep = hasStarness ? 4 : 2;
            int movesPerStep = pointsPerStep / 2;
            float[] values = new float[count * 2];
            BezierMove[] moves = new BezierMove[count];

            float step = M_PIx2 / pointCount;
            for (int i = 0; i < pointCount; i++)
            {
                float theta = step * i + orientation * M_PIx2;
                values[i * pointsPerStep + 0] = (float)Math.Sin(theta) * radius;
                values[i * pointsPerStep + 1] = (float)Math.Cos(theta) * radius;
                moves[i * pointsPerStep / 2] = (i == 0) ? BezierMove.MoveTo : BezierMove.LineTo;
                if (hasStarness)
                {
                    theta = (step * i + step / 2.0f) + orientation * M_PIx2;
                    float mpRadius = (float)Math.Cos(step / 2.0) * radius;
                    values[i * pointsPerStep + 2] = (float)Math.Sin(theta) * (mpRadius + mpRadius * starness);
                    values[i * pointsPerStep + 3] = (float)Math.Cos(theta) * (mpRadius + mpRadius * starness);
                    moves[i * pointsPerStep / 2 + 1] = BezierMove.LineTo;
                }
            }
            return new BezierStore(values, moves);
        }
    }
}
