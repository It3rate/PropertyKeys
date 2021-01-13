using System;
using System.Collections.Generic;
using System.Drawing;
using MotiveCore.Components;
using MotiveCore.Components.Libraries;
using MotiveCore.SeriesData;

namespace MotiveCore.Graphic
{
    public interface IRenderable : IDefinition
    {
	    IDrawableSeries GetDrawable(Dictionary<PropertyId, Series> dict);
        void DrawWithProperties(Dictionary<PropertyId, Series> dict, Graphics g);
    }
}
