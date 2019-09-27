using System.Drawing;
using DataArcs.Components;
using DataArcs.SeriesData;

namespace DataArcs.Graphic
{
	public abstract class GraphicBase
	{
		public abstract BezierSeries GetDrawableAtT(IComposite composite, float t);
	}
}