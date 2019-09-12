using System.Drawing;

namespace DataArcs.Graphic
{
	public abstract class GraphicBase
	{
		public abstract void Draw(Graphics g, Brush brush, Pen pen, float t);
	}
}