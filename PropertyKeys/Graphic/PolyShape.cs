using DataArcs.Stores;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace DataArcs.Graphic
{
    public class PolyShape : BaseGraphic
    {
        public float Orientation { get; set; }
        public int PointCount { get; set; }
        public float Starness { get; set; }
        public float Roundness { get; set; }
        float _radius;
        public float Radius { get => _radius; set { _radius = value; GeneratePolyShape();} }

        public readonly BezierStore[] BezierStores;
        private BezierStore polygon;

        public PolyShape(float orientation = 0, int pointCount = 4, float roundness = 0, float radius = 10f)
        {
            Orientation = orientation;
            PointCount = pointCount;
            Roundness = roundness;
            Radius = radius;
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
            polygon = GeneratePolyShape(Orientation, PointCount, Roundness, Radius);
        }
        public static BezierStore GeneratePolyShape(float orientation, int pointCount, float roundness, float radius)
        {
            float[] values = new float[pointCount * 2];
            BezierMove[] moves = new BezierMove[pointCount];

            float step = M_PIx2 / pointCount;
            for (int i = 0; i < pointCount; i++)
            {
                float theta = step * i + orientation * M_PIx2;
                values[i * 2] = (float)Math.Sin(theta) * radius;
                values[i * 2 + 1] = (float)Math.Cos(theta) * radius;
                moves[i] = (i == 0) ? BezierMove.MoveTo : BezierMove.LineTo;
            }
            return new BezierStore(values, moves);
        }
    }
}
