using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Remoting.Messaging;
using DataArcs.Adapters.Color;
using DataArcs.Components;
using DataArcs.Samplers;
using DataArcs.SeriesData;
using DataArcs.Stores;

namespace DataArcs.Graphic
{
	public class PolyShape : GraphicBase
	{
		private float _defaultOrientation = 0;
		private int _defaultPointCount = 4;
		private float _defaultStarness = 0f;
		private float _defaultRoundness = 0f;
		private float _defaultRadius = 10f;
		
        public BezierSeries GeneratePolyShape(float orientation, int pointCount, float roundness, float radiusX, float radiusY, float starness)
		{
			var hasStarness = Math.Abs(starness) > 0.001f;
			var count = hasStarness ? pointCount * 2 : pointCount;
			var pointsPerStep = hasStarness ? 4 : 2;
			var movesPerStep = pointsPerStep / 2;
			var values = new float[count * 2 + 2];
			var moves = new BezierMove[count + 1];
			orientation += 0.5f;

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

			moves[count] = BezierMove.LineTo;
			values[count * 2] = values[0];
			values[count * 2 + 1] = values[1];

			return new BezierSeries(values, moves);
		}
		
        public override BezierSeries GetDrawable(Dictionary<PropertyId, Series> dict)
        {
            var orientation = dict.ContainsKey(PropertyId.Orientation) ? dict[PropertyId.Orientation].X : _defaultOrientation;
            var pointCount = dict.ContainsKey(PropertyId.PointCount) ? (int)dict[PropertyId.PointCount].X : _defaultPointCount;
            var starness = dict.ContainsKey(PropertyId.Starness) ? dict[PropertyId.Starness].X : _defaultStarness;
            var roundness = dict.ContainsKey(PropertyId.Roundness) ? dict[PropertyId.Roundness].X : _defaultRoundness;
            var radiusX  = dict.ContainsKey(PropertyId.Radius) ? dict[PropertyId.Radius].X : _defaultRadius;
            var radiusY  = dict.ContainsKey(PropertyId.Radius) ? dict[PropertyId.Radius].Y : _defaultRadius;
            
            return GeneratePolyShape(orientation, pointCount, roundness, radiusX, radiusY, starness);
        }

        public override void DrawWithProperties(Dictionary<PropertyId, Series> dict, Graphics g)
        {
            BezierSeries bezier = GetDrawable(dict);
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
                        LineJoin = LineJoin.Round
                    };
                    g.DrawPath(p, gp);
                }
                g.Restore(state);
            }
        }
    }
}