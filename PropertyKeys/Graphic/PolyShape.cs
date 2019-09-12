using System;
using System.Drawing;
using DataArcs.Samplers;
using DataArcs.Series;
using DataArcs.Stores;

namespace DataArcs.Graphic
{
	public class PolyShape : GraphicBase
	{
		private BezierSeries Polygon { get; set; }
		private BezierSampler Sampler { get; }

		public Store Orientation { get; set; }
		public Store PointCount { get; set; }
		public Store Starness { get; set; }
		public Store Roundness { get; set; }
		public Store Radius { get; set; }

		public PolyShape(float radius, float orientation = 0, int pointCount = 4, float roundness = 0,
			float starness = 0)
		{
			Orientation = new Store(new[] {orientation});
			PointCount = new Store(new[] {pointCount});
			Roundness = new Store(new[] {roundness});
			Starness = new Store(new[] {starness});
			Radius = new Store(new FloatSeries(2, radius, radius));
		}

		public PolyShape(float[] radius, float[] orientation = null, int[] pointCount = null, float[] roundness = null,
			float[] starness = null)
		{
			Orientation = orientation == null ? new Store(new[] {0f}) : new Store(orientation);
			PointCount = pointCount == null ? new Store(new[] {4}) : new Store(pointCount);
			Roundness = roundness == null ? new Store(new[] {0f}) : new Store(roundness);
			Starness = starness == null ? new Store(new[] {1, 0f}) : new Store(starness);
			Radius = radius == null ? new Store(new FloatSeries(2, 10f, 10f)) : new Store(new FloatSeries(2, radius));
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
			var orientation = Orientation.GetSeriesAtT(orientationT)[0];
			var roundness = Roundness.GetSeriesAtT(roundnessT)[0];
			var radius = Radius.GetSeriesAtT(radiusT).FloatData;
			var radiusX = radius[0];
			var radiusY = radius.Length > 1 ? radius[1] : radius[0];
			var starness = Starness.GetSeriesAtT(starnessT)[0];
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