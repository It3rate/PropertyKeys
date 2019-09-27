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
		void DrawAtT(float t, IComposite composite, Graphics g);
	}
}
