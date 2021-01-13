using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Motive.Components;
using Motive.SeriesData;
using Motive.Adapters.Color;
using Motive.Adapters.Geometry;

namespace Motive.Graphic
{
    public class UIBox : GraphicBase
    {
	    private float _defaultRoundness = 0f;
	    private float _defaultSize = 10f;

        public UIBox()
        {
        }

	    public BezierSeries GenerateUIBox(float width, float height, float roundedCorners)
	    {
		    int count = 4;
		    var values = new float[count * 2 + 2];
		    var moves = new BezierMove[count + 1];
		    float x = 0;
		    float y = 0;
            values[0] = x;
		    values[1] = y;
		    values[2] = width;
		    values[3] = y;
		    values[4] = width;
		    values[5] = height;
		    values[6] = x;
		    values[7] = height;
		    values[8] = x;
		    values[9] = y;
		    moves[0] = BezierMove.MoveTo;
		    moves[1] = BezierMove.LineTo;
		    moves[2] = BezierMove.LineTo;
		    moves[3] = BezierMove.LineTo;
		    moves[4] = BezierMove.LineTo;

		    return new BezierSeries(values, moves);
        }
        public override IDrawableSeries GetDrawable(Dictionary<PropertyId, Series> dict)
        {
            var roundness = dict.ContainsKey(PropertyId.Roundness) ? dict[PropertyId.Roundness].X : _defaultRoundness;

            var szX = dict.ContainsKey(PropertyId.Size) ? dict[PropertyId.Size].X : _defaultSize;
            var szY = dict.ContainsKey(PropertyId.Size) ? dict[PropertyId.Size].Y : _defaultSize;

            return GenerateUIBox(szX * 0.8f + 0.2f , szY * 0.8f + 0.2f, roundness);
        }

        public override void DrawWithProperties(Dictionary<PropertyId, Series> dict, Graphics g)
        {
            BezierSeries bezier = (BezierSeries)GetDrawable(dict);
            if (bezier != null)
            {
                GraphicsPath gp = bezier.Path();

                var loc = dict[PropertyId.Location];
                var originSeries = dict[PropertyId.Origin]?.GetVirtualValueAt(0) ?? new FloatSeries(1, 0f, 0f);
                var scaleSeries = dict[PropertyId.Scale]?.GetVirtualValueAt(0) ?? new FloatSeries(1, 1f, 1f);
                var state = g.Save();
                var scaleX = scaleSeries.X;
                var scaleY = scaleSeries.Y;
                g.ScaleTransform(scaleX, scaleY);
                g.TranslateTransform(loc.X + (originSeries.X / scaleX), loc.Y + (originSeries.Y / scaleY));

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
