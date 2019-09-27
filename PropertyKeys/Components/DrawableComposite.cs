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
    public abstract class DrawableComposite : CompositeBase, IDrawable
    {
	    public override void Draw(IComposite composite, Graphics g)
	    {
		    IStore items = GetStore(PropertyId.Items) ?? GetStore(PropertyId.Location);
		    if (items != null)
		    {
			    int capacity = items.Capacity;
			    for (int i = 0; i < capacity; i++)
			    {
				    int index = GetStore(PropertyId.Items)?.GetValuesAtIndex(i).IntDataAt(0) ?? i;
				    Series v = GetSeriesAtIndex(PropertyId.Location, index);

				    var state = g.Save();
				    var scale = 1f; // + it * 0.8f;
				    g.ScaleTransform(scale, scale);
				    g.TranslateTransform(v.X / scale, v.Y / scale);

                    if (Graphic is IComposite graphic)
				    {
					    graphic.Draw(this, g); //DrawAtIndex(index, capacity, graphic, g);
				    }
				    else
				    {
					    DrawAtT(index / (capacity - 1f), this, g);
                    }

                    g.Restore(state);
                }
		    }
	    }

	    public void DrawAtT(float t, IComposite composite, Graphics g)
	    {
		    Graphic.DrawAtT(t, this, g);
        }
    }
}
