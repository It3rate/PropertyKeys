using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Graphic;
using DataArcs.Stores;

namespace DataArcs.Components
{
	public interface IDrawable
    {
        IRenderable Renderer { get; set; }
        void DrawAtT(float t, IComposite composite, Graphics g);
	}
}
