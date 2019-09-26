using System.Drawing;
using DataArcs.Components;

namespace DataArcs.Graphic
{
	public abstract class GraphicBase
	{
		// todo: return a drawable element, don't pass Graphics context.
		public abstract void Draw(CompositeBase composite, Graphics g, Brush brush, Pen pen, float t);

	}
}