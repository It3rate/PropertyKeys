﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Adapters.Color;
using DataArcs.SeriesData;
using DataArcs.Stores;

namespace DataArcs.Components
{
    public abstract class DrawableComposite : CompositeBase, IDrawable
    {
	    public virtual void Draw(IComposite composite, Graphics g)
	    {
		    IStore items = GetStore(PropertyId.Items) ?? GetStore(PropertyId.Location);
		    if (items != null)
		    {
			    int capacity = items.Capacity;
			    for (int i = 0; i < capacity; i++)
			    {
				    DrawAtIndex(i, this, g);
			    }
		    }
		    //g.DrawRectangle(Pens.Blue, new Rectangle(150, 150, 500, 144));
	    }
	    public void DrawAtIndex(int countIndex, IComposite composite, Graphics g)
	    {
		    IStore items = GetStore(PropertyId.Items) ?? GetStore(PropertyId.Location);
		    int capacity = items.Capacity;
            var it = capacity > 1 ? countIndex / (capacity - 1f) : 0;

		    int index = GetStore(PropertyId.Items)?.GetValuesAtIndex(countIndex).IntDataAt(0) ?? countIndex;
		    Series v = GetSeriesAtIndex(PropertyId.Location, index);

		    //ParametricSeries ps = GetSampledT(PropertyId.Location, it);

		    var state = g.Save();
		    var scale = 1f; // + it * 0.8f;
		    g.ScaleTransform(scale, scale);
		    g.TranslateTransform(v.X / scale, v.Y / scale);

		    Graphic.DrawAtIndex(countIndex, composite, g);
		    //BezierSeries bezier = Graphic.GetDrawableAtT(this, it * CurrentT);
		    //GraphicsPath gp = bezier.Path();

		    //var fillColor = GetStore(PropertyId.FillColor)?.GetValuesAtT(ps.X);
		    //if (fillColor != null)
		    //{
			   // g.FillPath(new SolidBrush(fillColor.RGB()), gp);
		    //}
		    //var penColor = GetStore(PropertyId.PenColor)?.GetValuesAtT(ps.X);
		    //if (penColor != null)
		    //{
			   // var penWidth = GetStore(PropertyId.PenWidth)?.GetValuesAtT(ps.X);
			   // float pw = penWidth?.X ?? 1f;

			   // g.DrawPath(new Pen(penColor.RGB(), pw), gp);
		    //}

		    g.Restore(state);
	    }
    }
}
