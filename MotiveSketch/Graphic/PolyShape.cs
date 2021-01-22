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
    // These shapes should probably sample from (or be?) Stores. Rather than shapes from scratch, there should be 
    // a set of Series that hold pointCounts, radius etc. Then a sampler can transform and sample the points
    // and Stores multiply or blend them. Have mental goal of shape tweening to see where things should fit.
    // There should be one global renderer, and it does things like set color, stroke path etc. The shapes should all 
    // feed that.

    // points and relations
    // Series are tensors that can supply or be used to generate numerical information.
    // Samplers are parameterized functions that can generate numerical information from an arbitrary series.
    // Stores combine one or many samplers and series together to generate data.
    // 'Strokes' are contiguous data sampled from stores using an instruction set
    // 'Shapes' are collections of strokes and information on how to render them (color etc).
    // 'Animations' are a shape's life cycles as it changes over time.
    // 'Worlds' are Animations that interact with each other.

    // VisSeries: holds primitive sets of points for point, line, rect, circ, arc. Only structural. Could even be bezier series or poly shape.
    // VisNode: Like VisSeries, but always uses a ref to Series, Node or Stroke, and internal series is a pos/offset type FloatSeries (eg 4 Count of values for each node in a rect).
	//          Gets the type of element from the series ref (arc, line, etc)
    // VisSampler: Can sample VisSeries or VisStroke to create e.g. reversed line from rect. Can compute offsets. Produces VisNodes.
    //              VisSampler can act as the Query engine as well. (get top line etc). Returns Vis primitive nodes when queried with series.
    // VisStore: regular store
    // VisStroke: Uses instructions and context from VisShape to generate a stroke from 'skill' style instructions (may be encoded as series). Generates GraphicsPath.
    // VisShape: Is a VisPad. Gets context (letterbox etc), and renders multiple strokes using color, stroke width, strokes, etc. Feeds renderer.

    // BezierSeries: Series holds points and curve/move/line types.
    // BezierSampler: Samples points by index, or continuous path by t.
    // BezierStore: Maybe can just be regular store. By their nature they can blend and transform point sets, interpolate paths.
    // BezierStroke: Can sample store for points along t or by index. Generates GraphicsPath.
    // BezierShape: Multiple paths, holds color info, line weights etc. Feeds Renderer.

    // PolySeries: Has Count of XY positions, but usually is purely virtual values.
    // PolySampler: Able to sample points or along path. Can get arbitrary segments, cw or ccw.
    // PolyStore: regular store.
    // PolyStroke: Generates graphics path using VisStore.
    // PolyShape: Full color info, could do complex non connected shapes if that made sense, but not meant for repetition.

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

		//private ClampMode _radiusClampType = ClampMode.Mirror;
        public BezierSeries GeneratePolyShape(float orientation, int pointCount, float roundness, ISeries radii, float starness)
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
                var radius = radii.FloatValueAt(segmentIndex);
                values[i * pointsPerStep + 0] = (float) Math.Sin(theta) * radius;
				values[i * pointsPerStep + 1] = (float) Math.Cos(theta) * radius;
				moves[i * pointsPerStep / 2] = i == 0 ? BezierMove.MoveTo : BezierMove.LineTo;
				if (hasStarness)
				{
					var radius2 = radii.FloatValueAt(segmentIndex + 1);
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
		
        public override IDrawableSeries GetDrawable(Dictionary<PropertyId, ISeries> dict)
        {
            var orientation = dict.ContainsKey(PropertyId.Orientation) ? dict[PropertyId.Orientation].X : _defaultOrientation;
            var pointCount = dict.ContainsKey(PropertyId.PointCount) ? (int)dict[PropertyId.PointCount].X : _defaultPointCount;
            var starness = dict.ContainsKey(PropertyId.Starness) ? dict[PropertyId.Starness].X : _defaultStarness;
            var roundness = dict.ContainsKey(PropertyId.Roundness) ? dict[PropertyId.Roundness].X : _defaultRoundness;
            var radii = dict.ContainsKey(PropertyId.Radius) ? dict[PropertyId.Radius] : _defaultRadii;
            
            return GeneratePolyShape(orientation, pointCount, roundness, radii, starness);
        }

        public override void DrawWithProperties(Dictionary<PropertyId, ISeries> dict, Graphics g)
        {
            BezierSeries bezier = (BezierSeries)GetDrawable(dict);
            if (bezier != null)
            {
	            var gp = new GraphicsPath();
                bezier.AppendToGraphicsPath(gp);

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