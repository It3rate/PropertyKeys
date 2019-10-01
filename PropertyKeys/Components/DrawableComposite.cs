using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Adapters.Color;
using DataArcs.Graphic;
using DataArcs.SeriesData;
using DataArcs.Stores;

namespace DataArcs.Components
{
    public class DrawableComposite : Composite, IDrawable
    {
        public DrawableComposite(IStore items = null) : base(items)
        {

        }
	    public void DrawAtT(float t, IComposite composite, Graphics g)
	    {
		    Graphic?.DrawAtT(t, this, g);
        }
    }
}
