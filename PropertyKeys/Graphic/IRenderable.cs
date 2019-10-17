using DataArcs.Components;
using DataArcs.SeriesData;
using System;

namespace DataArcs.Graphic
{
    public interface IRenderable
    {
	    int RendererId { get; }
        BezierSeries GetDrawableAtT(IComposite composite, float t);
    }
}
