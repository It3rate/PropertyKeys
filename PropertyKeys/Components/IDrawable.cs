using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Stores;

namespace DataArcs.Components
{
	public interface IDrawable
	{
		void Draw(IComposite composite, Graphics g);
		void DrawAtIndex(int countIndex, IComposite composite, Graphics g);
	}
}
