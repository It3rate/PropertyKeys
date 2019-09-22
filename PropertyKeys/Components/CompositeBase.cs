using DataArcs.SeriesData;
using DataArcs.Stores;
using System;
using System.Collections.Generic;
using System.Drawing;
using DataArcs.Adapters.Color;
using DataArcs.Adapters.Geometry;
using DataArcs.Graphic;
using DataArcs.Samplers;

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

        //public abstract void AddProperty(PropertyId id, IStore store);
        public abstract IStore GetStore(PropertyId propertyId);
        public abstract void GetDefinedStores(HashSet<PropertyId> ids);
        public abstract void Update(float currentTime, float deltaTime);
        public abstract CompositeBase CreateChild();
		
        public virtual Series GetSeriesAtT(PropertyId propertyId, float t, int virtualCount = -1)
        {
	        var store = GetStore(propertyId);
	        return store != null ? store.GetSeriesAtT(t, virtualCount) : SeriesUtils.GetZeroFloatSeries(1, 0);
        }
        public virtual ParametricSeries GetSampledT(PropertyId propertyId, float t)
        {
            var store = GetStore(propertyId);
            return store != null ? store.GetSampledT(t) : new ParametricSeries(1, t);
        }

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
            var it = index / (count - 1f);
            Series v = GetSeriesAtT(PropertyId.Location, it, count);
            
            ParametricSeries ps = GetSampledT(PropertyId.Location, it);

            var c = GetSeriesAtT(PropertyId.FillColor, ps.FloatDataAt(0)).RGB();

            Brush b = new SolidBrush(c);
            var state = g.Save();
            var scale = 1f;// + it * 0.8f;
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
