using System;
using System.Drawing;
using DataArcs.Components;
using DataArcs.Samplers;
using DataArcs.SeriesData;
using DataArcs.Stores;

namespace DataArcs.Graphic
{
	public class PolyShape : GraphicBase
	{
		private BezierSeries Polygon { get; set; }

		public PolyShape()
		{
		}
		
        public override void Draw(CompositeBase composite, Graphics g, Brush brush, Pen pen, float t)
        {
	        GeneratePolyShape(composite, t);
	        if (brush != null)
	        {
		        g.FillPath(brush, Polygon.Path());
	        }

	        if (pen != null)
	        {
		        g.DrawPath(pen, Polygon.Path());
	        }
        }

        private float _defaultOrientation = 0;
        private int _defaultPointCount = 4;
        private float _defaultStarness = 0f;
        private float _defaultRoundness = 0f;
        private float _defaultRadius = 10f;
        public void GeneratePolyShape(CompositeBase composite, float t)
        {
	        var orientation = composite.GetStore(PropertyId.Orientation)?.GetValuesAtT(t).X ?? _defaultOrientation;
	        var pointCount = (int)(composite.GetStore(PropertyId.PointCount)?.GetValuesAtT(t).X ?? _defaultPointCount);
	        var starness = composite.GetStore(PropertyId.Starness)?.GetValuesAtT(t).X ?? _defaultStarness;
	        var roundness = composite.GetStore(PropertyId.Roundness)?.GetValuesAtT(t).X ?? _defaultRoundness;
	        var radiusX = composite.GetStore(PropertyId.Radius)?.GetValuesAtT(t).X ?? _defaultRadius;
	        var radiusY = composite.GetStore(PropertyId.Radius)?.GetValuesAtT(t).Y ?? _defaultRadius;
            Polygon = GeneratePolyShape(orientation, pointCount, roundness, radiusX, radiusY, starness);
        }
		
		public static BezierSeries GeneratePolyShape(float orientation, int pointCount, float roundness, float radiusX, float radiusY, float starness)
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