using DataArcs.Components;
using DataArcs.SeriesData;
using System;

namespace DataArcs.Graphic
{
    public interface IRenderable
    {
        BezierSeries GetDrawableAtT(IComposite composite, float t);
    }
}
