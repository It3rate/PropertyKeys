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
        public void DrawAtT(float t, IComposite composite, Graphics g)
        {
            BezierSeries bezier = Renderer?.GetDrawableAtT(composite, t);// * composite.CurrentT);
            if(bezier != null)
            {
                GraphicsPath gp = bezier.Path();

                var fillColor = composite.GetStore(PropertyId.FillColor)?.GetValuesAtT(t);
                if (fillColor != null)
                {
                    g.FillPath(new SolidBrush(fillColor.RGB()), gp);
                }

                var penColor = composite.GetStore(PropertyId.PenColor)?.GetValuesAtT(t);
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
