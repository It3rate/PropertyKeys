using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DataArcs.Adapters;
using DataArcs.Adapters.Color;
using DataArcs.Adapters.Geometry;
using DataArcs.Graphic;
using DataArcs.Players;
using DataArcs.SeriesData;
using DataArcs.Stores;

namespace DataArcs.Components
{
	public class Composite : IComposite
	{

        private static int _idCounter = 1;

        private readonly Dictionary<PropertyId, IStore> _stores = new Dictionary<PropertyId, IStore>();
        private readonly List<IComposite> _children = new List<IComposite>();
        public int CompositeId { get; }
        public float InputT { get; set; } // todo: this should probably be a store property on a collection for per element, and a single base timespan on a transition.
        public IComposite Parent { get; set; }
        private IStore _items;
        public IStore Items => _items ?? GetStore(PropertyId.Items);

        protected Composite(IStore items)
        {
            if (items != null)
            {
                AddProperty(PropertyId.Items, items);
            }
            CompositeId = _idCounter++;
            Player.GetPlayerById(0).AddCompositeToLibrary(this);
        }

        public Composite(IStore items, IComposite parent = null) : this(items)
        {
            Parent = parent;
        }

        public int TotalItemCount
        {
            get
            {
                int result = (_items?.Capacity ?? 0);
                foreach (var child in _children)
                {
                    result += child.TotalItemCount;
                }
                return result;
            }
        }

        public int[] ChildCounts
        {
            get
            {
                int[] result = new int[_children.Count];
                for (int i = 0; i < _children.Count; i++)
                {
                    result[i] = _children[i].TotalItemCount;
                }
                return result;
            }
        }

        public virtual void AddProperty(PropertyId id, IStore store)
        {
            if(id == PropertyId.Items)
            {
                _items = store;
            }
            _stores[id] = store;
        }
        public virtual void AppendProperty(PropertyId id, IStore store)
        {
            if (_stores.ContainsKey(id))
            {
                IStore curStore = _stores[id];
                if (curStore is FunctionalStore)
                {
                    ((FunctionalStore)curStore).Add(curStore);
                }
                else
                {
                    _stores[id] = new FunctionalStore(curStore, store);
                }
            }
            else
            {
                AddProperty(id, store);
            }
        }
        public virtual void RemoveProperty(PropertyId id, BlendStore store)
        {
            _stores.Remove(id);
        }

        public void AddChild(IComposite child)
        {
            _children.Add(child);
        }
        public void RemoveChild(IComposite child)
        {
            _children.Remove(child);
        }

        public virtual IStore GetStore(PropertyId propertyId)
        {
            _stores.TryGetValue(propertyId, out var result);
            if (result == null && Parent != null)
            {
                result = Parent.GetStore(propertyId);
            }

            return result;
        }
        public virtual void GetDefinedStores(HashSet<PropertyId> ids)
        {
            foreach (var item in _stores.Keys)
            {
                ids.Add(item);
            }
            if (Parent != null)
            {
                Parent.GetDefinedStores(ids);
            }
        }

        public void Update(float currentTime, float deltaTime)
        {
            StartUpdate(currentTime, deltaTime);
            EndUpdate(currentTime, deltaTime);
        }
        public bool shouldShuffle; // basis for switching to events
        public virtual void StartUpdate(float currentTime, float deltaTime)
        {
            foreach (var store in _stores.Values)
            {
                store.Update(deltaTime);
            }

            float t = InputT % 1f;
            if (t <= 0.05f && shouldShuffle)
            {
                SeriesUtils.Shuffle(GetStore(PropertyId.Location).GetFullSeries(0));
            }
            if (t > 0.99 && shouldShuffle)
            {
                Series s = GetStore(PropertyId.Location).GetFullSeries(0);
                RandomSeries rs = (RandomSeries)s;
                rs.Seed = rs.Seed + 1;
            }

            InputT = deltaTime;
        }
        public virtual void EndUpdate(float currentTime, float deltaTime) { }

        public virtual IComposite CreateChild()
        {
            return new Composite(null, this);
        }

        public virtual void Draw(IComposite composite, Graphics g)
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
               
			        if (this is IDrawable selfDrawable)
			        {
				        selfDrawable.DrawAtT(index / (capacity - 0f), this, g);
                    }

                    foreach (var child in _children)
                    {
                        child.Draw(this, g);
                    }
                    
			        g.Restore(state);
		        }
	        }
        }

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