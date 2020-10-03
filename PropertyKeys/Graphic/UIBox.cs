using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Adapters.Color;
using DataArcs.Components;
using DataArcs.SeriesData;

namespace DataArcs.Graphic
{
    public class UIBox : GraphicBase
    {
	    private float _defaultRoundness = 0f;
	    private float _defaultRadius = 10f;

        public UIBox()
	    {
	    }

	    public BezierSeries GenerateUIBox(float width, float height, float roundedCorners)
	    {
		    throw new NotImplementedException();
        }
        public override BezierSeries GetDrawable(Dictionary<PropertyId, Series> dict)
        {
            var roundness = dict.ContainsKey(PropertyId.Roundness) ? dict[PropertyId.Roundness].X : _defaultRoundness;
            var radiusX = dict.ContainsKey(PropertyId.Radius) ? dict[PropertyId.Radius].X : _defaultRadius;
            var radiusY = dict.ContainsKey(PropertyId.Radius) ? dict[PropertyId.Radius].Y : _defaultRadius;

            return GenerateUIBox(radiusX * 2, radiusY * 2, roundness);
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
