using System;
using System.Collections.Generic;
using System.Drawing;
using Motive.Components;
using Motive.Components.Libraries;
using Motive.SeriesData;

namespace Motive.Graphic
{
    public interface IRenderable : IDefinition
    {
	    IDrawableSeries GetDrawable(Dictionary<PropertyId, Series> dict);
        void DrawWithProperties(Dictionary<PropertyId, Series> dict, Graphics g);
    }
}
