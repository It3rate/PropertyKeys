using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using Motive.Components;
using Motive.SeriesData;
using Motive.Adapters.Color;
using Motive.SeriesData.Utils;

namespace Motive.Graphic
{
	public class PolyShape : GraphicBase
	{
        // Add directional radius series to allow rects, spirals etc. Or should that just be a periodic blend between two shapes?

		public bool FlatTop { get; set; }
		public bool PackHorizontal { get; set; }
        private float _defaultOrientation = 0;
		private int _defaultPointCount = 4;
		private float _defaultStarness = 0f;
		private float _defaultRoundness = 0f;
		private FloatSeries _defaultRadii = new FloatSeries(1, 1f);

        public PolyShape(bool flatTop = false, bool packHorizontal = false)
		{
			FlatTop = flatTop;
			PackHorizontal = packHorizontal;
		}

		public static PolyShape CreateQuad(float radiusX = 1f, float radiusY = 1f)
		{
            var result = new PolyShape(true)
            {
                _defaultRadii = new FloatSeries(2, radiusX, radiusY)
            };
            return result;
		}

		//private DiscreteClampMode _radiusClampType = DiscreteClampMode.Mirror;
        public BezierSeries GeneratePolyShape(float orientation, int pointCount, float roundness, Series radii, float starness)
        {
			var hasStarness = Math.Abs(starness) > 0.001f;
			var count = hasStarness ? pointCount * 2 : pointCount;
			var pointsPerStep = hasStarness ? 4 : 2;
			var movesPerStep = pointsPerStep / 2;
			var values = new float[count * 2 + 2];
			var moves = new BezierMove[count + 1];
			orientation += 0.5f; // start at top, odd points up
			orientation += (FlatTop || PackHorizontal) ? 1f / (pointCount * 2f) : 0;
			orientation += PackHorizontal ? 0.25f : 0;

			var step = Utils.M_PIx2 / pointCount;
			for (var i = 0; i < pointCount; i++)
			{
				var theta = step * i + orientation * Utils.M_PIx2;
				var segmentIndex = hasStarness ? i * 2 : i;
                //var radIndex = _radiusClampType.GetClampedValue(segmentIndex, radii.VectorSize);
				//radIndex = hasStarness ? radIndex / 2 * 2 : radIndex;
                var radius = radii.FloatDataAt(segmentIndex);
                values[i * pointsPerStep + 0] = (float) Math.Sin(theta) * radius;
				values[i * pointsPerStep + 1] = (float) Math.Cos(theta) * radius;
				moves[i * pointsPerStep / 2] = i == 0 ? BezierMove.MoveTo : BezierMove.LineTo;
				if (hasStarness)
				{
					var radius2 = radii.FloatDataAt(segmentIndex + 1);
                    theta = step * i + step / 2.0f + orientation * Utils.M_PIx2;
					var mpRadiusX = (float) Math.Cos(step / 2.0) * radius2;
					var mpRadiusY = (float) Math.Cos(step / 2.0) * radius2;
					values[i * pointsPerStep + 2] = (float) Math.Sin(theta) * (mpRadiusX + mpRadiusX * starness);
					values[i * pointsPerStep + 3] = (float) Math.Cos(theta) * (mpRadiusY + mpRadiusY * starness);
					moves[i * pointsPerStep / 2 + 1] = BezierMove.LineTo;
				}
			}

			moves[count] = BezierMove.LineTo;
			values[count * 2] = values[0];
			values[count * 2 + 1] = values[1];

			return new BezierSeries(values, moves);
		}
		
        public override IDrawableSeries GetDrawable(Dictionary<PropertyId, Series> dict)
        {
            var orientation = dict.ContainsKey(PropertyId.Orientation) ? dict[PropertyId.Orientation].X : _defaultOrientation;
            var pointCount = dict.ContainsKey(PropertyId.PointCount) ? (int)dict[PropertyId.PointCount].X : _defaultPointCount;
            var starness = dict.ContainsKey(PropertyId.Starness) ? dict[PropertyId.Starness].X : _defaultStarness;
            var roundness = dict.ContainsKey(PropertyId.Roundness) ? dict[PropertyId.Roundness].X : _defaultRoundness;
            var radii = dict.ContainsKey(PropertyId.Radius) ? dict[PropertyId.Radius] : _defaultRadii;
            
            return GeneratePolyShape(orientation, pointCount, roundness, radii, starness);
        }

        public override void DrawWithProperties(Dictionary<PropertyId, Series> dict, Graphics g)
        {
            BezierSeries bezier = (BezierSeries)GetDrawable(dict);
            if (bezier != null)
            {
                GraphicsPath gp = bezier.Path();

                var v = dict[PropertyId.Location];
                var state = g.Save();
                var scale = 1f;
                g.ScaleTransform(scale, scale);
                g.TranslateTransform(v.X / scale, v.Y / scale);

                if (dict.ContainsKey(PropertyId.FillColor))
                {
                    g.FillPath(new SolidBrush(dict[PropertyId.FillColor].RGB()), gp);
                }

                if (dict.ContainsKey(PropertyId.PenColor))
                {
                    var penWidth = dict.ContainsKey(PropertyId.PenWidth) ? dict[PropertyId.PenWidth].X : 1f;
                    Pen p = new Pen(dict[PropertyId.PenColor].RGB(), penWidth)
                    {
                        LineJoin = LineJoin.Round,
						StartCap = LineCap.Round,
						EndCap = LineCap.Round
                    };
                    g.DrawPath(p, gp);
                }
                g.Restore(state);
            }
        }
    }
}