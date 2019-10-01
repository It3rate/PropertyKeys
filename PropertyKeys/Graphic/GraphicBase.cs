using System.Drawing;
using DataArcs.Components;
using DataArcs.SeriesData;
using DataArcs.Stores;

namespace DataArcs.Graphic
{
	public abstract class GraphicBase : IRenderable
	{
		public abstract BezierSeries GetDrawableAtT(IComposite composite, float t);
	}
}