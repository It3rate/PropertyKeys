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
using DataArcs.Players;

namespace DataArcs.Components
{
	public abstract class CompositeBase : IComposite
    {
	    private static int _idCounter = 1;

	    public int CompositeId { get;}
	    public float CurrentT { get; set; }
        public IDrawable Graphic { get; set; }
        public IComposite Parent { get; set; }

        protected CompositeBase()
        {
	        CompositeId = _idCounter++;
			Player.GetPlayerById(0).AddCompositeToLibrary(this);
        }
		
        public abstract IStore GetStore(PropertyId propertyId);
        public abstract void GetDefinedStores(HashSet<PropertyId> ids);

        public void Update(float currentTime, float deltaTime)
        {
            StartUpdate(currentTime, deltaTime);
            EndUpdate(currentTime, deltaTime);
        }
        public abstract void StartUpdate(float currentTime, float deltaTime);
        public abstract void EndUpdate(float currentTime, float deltaTime);

        public abstract IComposite CreateChild();
        public abstract void Draw(IComposite composite, Graphics g);

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

    }
}
