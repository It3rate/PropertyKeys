using DataArcs.Components;
using DataArcs.SeriesData;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace DataArcs.Graphic
{
    public interface IRenderable
    {
	    int RendererId { get; }
        BezierSeries GetDrawableAtT(IComposite composite, float t);

        BezierSeries GetDrawable(Dictionary<PropertyId, Series> dict);
        void DrawWithProperties(Dictionary<PropertyId, Series> dict, Graphics g);
    }
}
