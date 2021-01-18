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
	    IDrawableSeries GetDrawable(Dictionary<PropertyId, ISeries> dict);
        void DrawWithProperties(Dictionary<PropertyId, ISeries> dict, Graphics g);
    }
}
