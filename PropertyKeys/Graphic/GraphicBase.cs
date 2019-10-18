using System.Collections.Generic;
using System.Drawing;
using DataArcs.Components;
using DataArcs.SeriesData;
using DataArcs.Stores;

namespace DataArcs.Graphic
{
	public abstract class GraphicBase : IRenderable
	{
		protected static int _rendererCount = 1;
		public int RendererId { get; }
		public abstract BezierSeries GetDrawableAtT(IComposite composite, float t);
        public abstract void DrawWithProperties(Dictionary<PropertyId, Series> dict, Graphics g);

        public abstract BezierSeries GetDrawable(Dictionary<PropertyId, Series> dict);

        public GraphicBase()
		{
			RendererId = _rendererCount++;
		}
	}
}