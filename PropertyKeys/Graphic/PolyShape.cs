﻿using DataArcs.Stores;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace DataArcs.Graphic
{
    public class PolyShape : BaseGraphic
    {
        private BezierStore polygon;

        private FloatStore _orientation;
        private int _pointCount;
        private FloatStore _starness;
        private FloatStore _roundness;
        private FloatStore _radius;

        public PolyShape(float radius, float orientation = 0, int pointCount = 4, float roundness = 0, float starness = 0)
        {
            Orientation = new FloatStore(1, orientation);
            PointCount = pointCount;
            Roundness = new FloatStore(1, roundness);
            Radius = new FloatStore(1, radius);
            Starness = new FloatStore(1, starness);
        }
        public PolyShape(float[] radius, float[] orientation = null, int pointCount = 6, float[] roundness = null, float[] starness = null)
        {
            Orientation = (orientation == null) ? new FloatStore(1, 0f) : new FloatStore(1, orientation);
            PointCount = pointCount;
            Roundness = (roundness == null) ? new FloatStore(1, 0f) : new FloatStore(1, roundness);
            Radius = (radius == null) ? new FloatStore(1, 10f) : new FloatStore(1, radius);
            Starness = (starness == null) ? new FloatStore(1, 0f) : new FloatStore(1, starness);
        }

        public FloatStore Orientation
        {
            get => _orientation;
            set
            {
                // todo: calc isDirty flag using a separate 't' array, one for each property
                //if (Math.Abs(_orientation - value) > DataUtils.TOLERANCE)
                {
                    _orientation = value;
                    //GeneratePolyShape();
                }
            }
        }

        public int PointCount
        {
            get => _pointCount;
            set
            {
                //if (Math.Abs(_pointCount - value) > DataUtils.TOLERANCE)
                {
                    _pointCount = value;
                    //GeneratePolyShape();
                }
            }
        }

        public FloatStore Starness
        {
            get => _starness;
            set
            {
                //if (Math.Abs(_starness - value) > DataUtils.TOLERANCE)
                {
                    _starness = value;
                    //GeneratePolyShape();
                }
            }
        }

        public FloatStore Roundness
        {
            get => _roundness;
            set
            {
                //if (Math.Abs(_roundness - value) > DataUtils.TOLERANCE)
                {
                    _roundness = value;
                    //GeneratePolyShape();
                }
            }
        }

        public FloatStore Radius
        {
            get => _radius;
            set
            {
                //if (Math.Abs(_radius - value) > DataUtils.TOLERANCE)
                {
                    _radius = value;
                    //GeneratePolyShape();
                }
            }
        }


        public override void Draw(Graphics g, Brush brush, Pen pen, float t)
        {
            GeneratePolyShape(Radius.CurrentT, Orientation.CurrentT, Roundness.CurrentT, Starness.CurrentT);
            if (brush != null)
            {
                g.FillPath(brush, polygon.Path);
            }

            if (pen != null)
            {
                g.DrawPath(pen, polygon.Path);
            }
        }
        public void GeneratePolyShape(float radiusT, float orientationT = 0, float roundnessT = 0, float starnessT = 0)
        {
            float orientation = Orientation.GetFloatArrayAtT(orientationT)[0];
            float roundness = Roundness.GetFloatArrayAtT(roundnessT)[0];
            float radius = Radius.GetFloatArrayAtT(radiusT)[0];
            float starness = Starness.GetFloatArrayAtT(starnessT)[0];
            polygon = GeneratePolyShape(orientation, PointCount, roundness, radius, starness);
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
