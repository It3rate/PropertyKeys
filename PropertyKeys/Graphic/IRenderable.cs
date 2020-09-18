using DataArcs.Components;
using DataArcs.SeriesData;
using System;
using System.Collections.Generic;
using System.Drawing;
using DataArcs.Components.Libraries;

namespace DataArcs.Graphic
{
    public interface IRenderable : IDefinition
    {
	    BezierSeries GetDrawable(Dictionary<PropertyId, Series> dict);
        void DrawWithProperties(Dictionary<PropertyId, Series> dict, Graphics g);
    }
}
