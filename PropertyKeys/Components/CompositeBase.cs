using DataArcs.SeriesData;
using DataArcs.Stores;
using System;
using System.Collections.Generic;
using System.Drawing;
using DataArcs.Adapters.Color;
using DataArcs.Adapters.Geometry;
using DataArcs.Graphic;

namespace DataArcs.Components
{
    public abstract class CompositeBase
    {
	    private static int _idCounter = 1;

	    public int CompositeId { get;}
	    protected float CurrentT { get; set; }
        public GraphicBase Graphic { get; set; } // will become store props or something

        protected CompositeBase()
        {
	        CompositeId = _idCounter++;
        }

        public abstract IStore GetStore(PropertyId propertyId);
        public abstract void GetDefinedStores(HashSet<PropertyId> ids);
        public abstract void Update(float time);
        public abstract CompositeBase CreateChild();

        public virtual void Draw(Graphics g)
        {
            var itemStore = GetStore(PropertyId.Items);
            if (itemStore != null)
            {
                foreach (Series series in itemStore)
                {
                    DrawAtIndex(series.IntDataAt(0), itemStore.VirtualCount, g);
                }
            }
            else
            {
                var count = 50;// GetStore(PropertyId.Location).GetElementCountAt(t);
                for (var i = 0; i < count; i++)
                {
                    DrawAtIndex(i, count, g);
                }
            }


            //g.DrawRectangle(Pens.Blue, new Rectangle(150, 150, 500, 144));
        }
        public void DrawAtIndex(int index, int count, Graphics g)
        {
            var loc = GetStore(PropertyId.Location);
            var col = GetStore(PropertyId.FillColor);
            var it = index / (count - 1f);
            Series v = loc.GetSeriesAtT(it, count);

            var c = col.GetSeriesAtT(it).RGB();
            Brush b = new SolidBrush(c);
            var state = g.Save();
            var scale = 1f; //  + t * 0.2f;
            g.ScaleTransform(scale, scale);
            g.TranslateTransform(v.X() / scale, v.Y() / scale);
            Graphic.Draw(g, b, null, CurrentT);
            g.Restore(state);
        }
    }

    public enum PropertyId : int // will change to local render defined, combo of type and property
    {
	    None = 0,
	    TModifier,

	    Items,
	    Shape,
	    Transform,
	    Location,
	    Size,
	    Scale,
	    Rotation,
	    FillColor,
	    PenColor,
	    T,
	    StartTime,
	    Duration,
	    Easing,
	    SampleType,

	    Graphic,
	    Starness,
	    Roundness,
	    Radius,
	    RandomMotion,

	    Custom = 0x1000,
    }
}
