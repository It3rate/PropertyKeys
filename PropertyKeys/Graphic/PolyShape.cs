using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PropertyKeys.Graphic
{
    public class PolyShape : BaseGraphic
    {
        public float Orientation { get; set; }
        public int PointCount { get; set; }
        public float Starness { get; set; }
        public float Roundness { get; set; }
        public float Radius { get; set; }

        public PolyShape(float orientation = 0, int pointCount = 4, float roundness = 0, float radius = 10f)
        {
            Orientation = orientation;
            PointCount = pointCount;
            Roundness = roundness;
            Radius = radius;
        }
        public override void Draw(Graphics g, Brush brush, Pen pen, float t)
        {
            GraphicsPath p = new GraphicsPath();
            float step = M_PIx2 / PointCount;
            float startX = 0;
            float startY = 0;
            float prevX = 0;
            float prevY = 0;
            for (int i = 0; i < PointCount; i++)
            {
                float theta = step * i + Orientation * M_PIx2;
                float x = (float)Math.Sin(theta) * Radius;
                float y = (float)Math.Cos(theta) * Radius;
                if(i == 0)
                {
                    startX = x;
                    startY = y;
                }
                else {
                    p.AddLine(prevX, prevY, x, y);
                }
                prevX = x;
                prevY = y;
            }
            p.AddLine(prevX, prevY, startX, startY);

            if (brush != null)
            {
                g.FillPath(brush, p);
            }

            if (pen != null)
            {
                g.DrawPath(pen, p);
            }
        }
    }
}
