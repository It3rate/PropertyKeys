using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Adapters.Color;
using DataArcs.Graphic;
using DataArcs.SeriesData;
using DataArcs.Stores;

namespace DataArcs.Components
{
    public class DrawableComposite : Composite, IDrawable
    {
        public IRenderable Renderer { get; set; }

        public DrawableComposite(IStore items = null) : base(items)
        {

        }
        public void DrawAtT(float t, IComposite composite, Graphics g, Dictionary<PropertyId, Series> dict)
        {
            BezierSeries bezier = Renderer?.GetDrawableAtT(composite, t);
            if(bezier != null)
            {
                GraphicsPath gp = bezier.Path();
                var fillColor = composite.GetSeriesAtT(PropertyId.FillColor, t, null);
                if (fillColor != null)
                {
                    g.FillPath(new SolidBrush(fillColor.RGB()), gp);
                }

                var penColor = composite.GetSeriesAtT(PropertyId.PenColor, t, null); 
                if (penColor != null)
                {
                    var penWidth = composite.GetStore(PropertyId.PenWidth)?.GetValuesAtT(t);
                    float pw = penWidth?.X ?? 1f;

                    g.DrawPath(new Pen(penColor.RGB(), pw), gp);
                }
            }
        }
    }
}
