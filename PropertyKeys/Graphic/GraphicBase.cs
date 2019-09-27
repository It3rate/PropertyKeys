using System.Drawing;
using DataArcs.Components;
using DataArcs.SeriesData;
using DataArcs.Stores;

namespace DataArcs.Graphic
{
	public abstract class GraphicBase : IDrawable
	{
		public abstract BezierSeries GetDrawableAtT(IComposite composite, float t);

		public abstract void DrawAtT(float t, IComposite composite, Graphics g);
	}
}