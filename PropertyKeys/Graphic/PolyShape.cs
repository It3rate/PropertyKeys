﻿using System;
using System.Drawing;
using DataArcs.Samplers;
using DataArcs.SeriesData;
using DataArcs.Stores;

namespace DataArcs.Graphic
{
	public class PolyShape : GraphicBase
	{
		private BezierSeries Polygon { get; set; }

		public Store Orientation { get; set; }
		public Store PointCount { get; set; }
		public Store Starness { get; set; }
		public Store Roundness { get; set; }
		public Store Radius { get; set; }

		public PolyShape(float radius, float orientation = 0, int pointCount = 4, float roundness = 0, float starness = 0) : this (
			new Store(new[] { radius }),
			new Store(new[] { orientation }),
			new Store(new[] { pointCount }),
			new Store(new[] { roundness }),
			new Store(new[] { starness }) ) { }

		public PolyShape(float[] radius, float[] orientation = null, int[] pointCount = null, float[] roundness = null, float[] starness = null) : this(
			radius != null ? new Store(radius) : null,
			orientation != null ? new Store(orientation) : null,
			pointCount != null ? new Store(pointCount) : null,
			roundness != null ? new Store(roundness) : null,
			starness != null ? new Store(starness) : null) { }

		public PolyShape(Store radius, Store orientation = null, Store pointCount = null, Store roundness = null, Store starness = null)
		{
			Orientation = orientation ?? new Store(new[] { 0f });
			PointCount = pointCount ?? new Store(new[] { 4 });
			Roundness = roundness ?? new Store(new[] { 0f });
			Starness = starness ?? new Store(new[] { 1, 0f });
			Radius = radius ?? new Store(new FloatSeries(2, 10f, 10f));
		}

        //public FloatStore Orientation
        //{
        //    get => _orientation;
        //    set
        //    {
        //        // todo: calc isDirty flag using a separate 't' array, one for each property
        //        //if (Math.Abs(_orientation - value) > SeriesUtils.TOLERANCE)
        //        {
        //            _orientation = value;
        //            //GeneratePolyShape();
        //        }
        //    }
        //}

        //public IntStore PointCount
        //{
        //    get => _pointCount;
        //    set
        //    {
        //        //if (Math.Abs(_pointCount - value) > SeriesUtils.TOLERANCE)
        //        {
        //            _pointCount = value;
        //            //GeneratePolyShape();
        //        }
        //    }
        //}

        //public FloatStore Starness
        //{
        //    get => _starness;
        //    set
        //    {
        //        //if (Math.Abs(_starness - value) > SeriesUtils.TOLERANCE)
        //        {
        //            _starness = value;
        //            //GeneratePolyShape();
        //        }
        //    }
        //}

        //public FloatStore Roundness
        //{
        //    get => _roundness;
        //    set
        //    {
        //        //if (Math.Abs(_roundness - value) > SeriesUtils.TOLERANCE)
        //        {
        //            _roundness = value;
        //            //GeneratePolyShape();
        //        }
        //    }
        //}

        //public FloatStore Radius
        //{
        //    get => _radius;
        //    set
        //    {
        //        //if (Math.Abs(_radius - value) > SeriesUtils.TOLERANCE)
        //        {
        //            _radius = value;
        //            //GeneratePolyShape();
        //        }
        //    }
        //}


        public override void Draw(Graphics g, Brush brush, Pen pen, float t)
		{
			GeneratePolyShape(t, t, t, t, t);
			if (brush != null)
			{
				g.FillPath(brush, Polygon.Path());
			}

			if (pen != null)
			{
				g.DrawPath(pen, Polygon.Path());
			}
		}

		public void GeneratePolyShape(float radiusT, float pointCountT = 0, float orientationT = 0,
			float roundnessT = 0, float starnessT = 0)
		{
			var orientation = Orientation.GetSeriesAtT(orientationT).FloatDataAt(0);
			var roundness = Roundness.GetSeriesAtT(roundnessT).FloatDataAt(0);
			var radius = Radius.GetSeriesAtT(radiusT).FloatData;
			var radiusX = radius[0];
			var radiusY = radius.Length > 1 ? radius[1] : radius[0];
			var starness = Starness.GetSeriesAtT(starnessT).FloatDataAt(0);
			var pointCount = PointCount.GetSeriesAtT(pointCountT).IntDataAt(0);
			Polygon = GeneratePolyShape(orientation, pointCount, roundness, radiusX, radiusY, starness);
		}

		public static BezierSeries GeneratePolyShape(float orientation, int pointCount, float roundness, float radiusX,
			float radiusY, float starness)
		{
			var hasStarness = Math.Abs(starness) > 0.001f;
			var count = hasStarness ? pointCount * 2 : pointCount;
			var pointsPerStep = hasStarness ? 4 : 2;
			var movesPerStep = pointsPerStep / 2;
			var values = new float[count * 2];
			var moves = new BezierMove[count];

			var step = Utils.M_PIx2 / pointCount;
			for (var i = 0; i < pointCount; i++)
			{
				var theta = step * i + orientation * Utils.M_PIx2;
				values[i * pointsPerStep + 0] = (float) Math.Sin(theta) * radiusX;
				values[i * pointsPerStep + 1] = (float) Math.Cos(theta) * radiusY;
				moves[i * pointsPerStep / 2] = i == 0 ? BezierMove.MoveTo : BezierMove.LineTo;
				if (hasStarness)
				{
					theta = step * i + step / 2.0f + orientation * Utils.M_PIx2;
					var mpRadiusX = (float) Math.Cos(step / 2.0) * radiusX;
					var mpRadiusY = (float) Math.Cos(step / 2.0) * radiusY;
					values[i * pointsPerStep + 2] = (float) Math.Sin(theta) * (mpRadiusX + mpRadiusX * starness);
					values[i * pointsPerStep + 3] = (float) Math.Cos(theta) * (mpRadiusY + mpRadiusY * starness);
					moves[i * pointsPerStep / 2 + 1] = BezierMove.LineTo;
				}
			}

			return new BezierSeries(values, moves);
		}
	}
}