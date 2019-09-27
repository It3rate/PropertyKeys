using System.Drawing;
using DataArcs.Components;
using DataArcs.SeriesData;
using DataArcs.Stores;

namespace DataArcs.Graphic
{
	public abstract class GraphicBase :IDrawable
	{
		public abstract BezierSeries GetDrawableAtT(IComposite composite, float t);

		public abstract void Draw(IComposite composite, Graphics g);
		public abstract void DrawAtIndex(int countIndex, IComposite composite, Graphics g);
	}
}