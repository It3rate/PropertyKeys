using DataArcs.SeriesData;
using DataArcs.Stores;
using System;
using System.Collections.Generic;
using System.Drawing;
using DataArcs.Adapters.Color;
using DataArcs.Adapters.Geometry;
using DataArcs.Graphic;
using DataArcs.Samplers;
using System.Drawing.Drawing2D;

namespace DataArcs.Components
{
    public abstract class CompositeBase
    {
	    private static int _idCounter = 1;

	    public int CompositeId { get;}
	    protected float CurrentT { get; set; }
        public PolyShape Graphic { get; set; }

        protected CompositeBase()
        {
	        CompositeId = _idCounter++;
        }

        //public abstract void AddProperty(PropertyId id, IStore store);
        public abstract IStore GetStore(PropertyId propertyId);
        public abstract void GetDefinedStores(HashSet<PropertyId> ids);
        public abstract void Update(float currentTime, float deltaTime);
        public abstract CompositeBase CreateChild();

        public virtual Series GetSeriesAtT(PropertyId propertyId, float t)
        {
            var store = GetStore(propertyId);
            return store != null ? store.GetValuesAtT(t) : SeriesUtils.GetZeroFloatSeries(1, 0);
        }
        public virtual Series GetSeriesAtIndex(PropertyId propertyId, int index)
        {
            var store = GetStore(propertyId);
            return store != null ? store.GetValuesAtIndex(index) : SeriesUtils.GetZeroFloatSeries(1, 0);
        }
        public virtual ParametricSeries GetSampledT(PropertyId propertyId, float t)
        {
            var store = GetStore(propertyId);
            return store != null ? store.GetSampledTs(t) : new ParametricSeries(1, t);
        }

        public virtual void Draw(Graphics g)
        {
            IStore store = GetStore(PropertyId.Items) ?? GetStore(PropertyId.Location);
            if (store != null)
            {
	            for (int i = 0; i < store.Capacity; i++)
	            {
		            DrawAtIndex(i, store, g);
                }
            }
            //g.DrawRectangle(Pens.Blue, new Rectangle(150, 150, 500, 144));
        }

        public void DrawAtIndex(int countIndex, IStore itemStore, Graphics g)
        {
	        int count = itemStore.Capacity;
	        var it = count > 1 ? countIndex / (count - 1f) : 0;

	        int index = GetStore(PropertyId.Items)?.GetValuesAtIndex(countIndex).IntDataAt(0) ?? countIndex;
	        Series v = GetSeriesAtIndex(PropertyId.Location, index);
	        //Series v = GetSeriesAtT(PropertyId.Location, it);

	        ParametricSeries ps = GetSampledT(PropertyId.Location, it);

	        var state = g.Save();
	        var scale = 1f; // + it * 0.8f;
	        g.ScaleTransform(scale, scale);
	        g.TranslateTransform(v.X / scale, v.Y / scale);

	        BezierSeries bezier = Graphic.GetDrawableAtT(this, it*CurrentT);
            GraphicsPath gp = bezier.Path();

            var fillColor = GetStore(PropertyId.FillColor)?.GetValuesAtT(ps.X);
            if (fillColor != null)
            {
                g.FillPath(new SolidBrush(fillColor.RGB()), gp);
            }
            var penColor = GetStore(PropertyId.PenColor)?.GetValuesAtT(ps.X);
            if (penColor != null)
            {
                var penWidth = GetStore(PropertyId.PenWidth)?.GetValuesAtT(ps.X);
                float pw = penWidth?.X ?? 1f;

                g.DrawPath(new Pen(penColor.RGB(), pw), gp);
            }

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
        PenWidth,
        T,
	    StartTime,
	    Duration,
	    Easing,
	    SampleType,

	    Graphic,
		// polyShape
		Orientation,
		PointCount,
		Starness,
        Roundness,
	    Radius,
	    RandomMotion,

	    Custom = 0x1000,
    }
}
