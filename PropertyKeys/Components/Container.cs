using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using DataArcs.Adapters;
using DataArcs.Adapters.Color;
using DataArcs.Adapters.Geometry;
using DataArcs.Graphic;
using DataArcs.Players;
using DataArcs.Samplers;
using DataArcs.SeriesData;
using DataArcs.Stores;

namespace DataArcs.Components
{
	public class Container : IContainer, IDrawable
	{
        private static int _idCounter = 1;

        private readonly Dictionary<PropertyId, IStore> _stores = new Dictionary<PropertyId, IStore>();
        private readonly List<IContainer> _children = new List<IContainer>();
        public int CompositeId { get; }
        public IContainer Parent { get; set; }
        public IRenderable Renderer { get; set; }

        private IStore _items;
        public IStore Items => _items ?? GetStore(PropertyId.Items);
        public int Capacity => GetStore(PropertyId.Location)?.Sampler?.Capacity ?? Items.Capacity;
        public string Name { get; set; }

        protected Container(IStore items)
        {
            if (items != null)
            {
                AddProperty(PropertyId.Items, items);
            }
            CompositeId = _idCounter++;
            Player.GetPlayerById(0).AddCompositeToLibrary(this);
        }
        public Container(IStore items = null, IContainer parent = null) : this(items)
        {
            Parent = parent;
        }

#region Elements
        public virtual int NestedItemCount
        {
            get
            {
                int result = 0;
                if (_children.Count > 0)
                {
                    for (int i = 0; i < Capacity; i++)
                    {
                        int index = Math.Min(_children.Count - 1, i);
                        result += _children[index].NestedItemCount;
                    }
                }
                else
                {
                    result = Items?.Capacity ?? 0;
                }
                return result;
            }
        }
        public virtual int[] ChildCounts
        {
            get
            {
                int[] result;
                if (_children.Count == 0) {
                    result = new int[] { Capacity };
                }
                else
                {
                    result = new int[Capacity];
                    for (int i = 0; i < Capacity; i++)
                    {
                        int index = Math.Max(0, Math.Min(_children.Count - 1, i));
                        result[i] = _children[index].NestedItemCount;
                    }
                }
                return result;
            }
        }
        public virtual int NestedItemCountAtT(float t)
        {
            return NestedItemCount;
        }

        public void AddProperty(PropertyId id, IStore store)
        {
            if(id == PropertyId.Items)
            {
                _items = store;
            }
            _stores[id] = store;
        }
        public void AppendProperty(PropertyId id, IStore store)
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
        public void RemoveProperty(PropertyId id, BlendStore store)
        {
            _stores.Remove(id);
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

            Parent?.GetDefinedStores(ids);
        }

        public void AddChild(IContainer child)
        {
            child.Parent = this;
            _children.Add(child);
        }
        public void RemoveChild(IContainer child)
        {
            _children.Remove(child);
        }
        public virtual IContainer CreateChild()
        {
            return new Container(null, this);
        }

#endregion

#region Updates
        public void Update(float currentTime, float deltaTime)
        {
            StartUpdate(currentTime, deltaTime);
            EndUpdate(currentTime, deltaTime);
        }

        public bool shouldShuffle; // temp basis for switching to events
        public virtual void StartUpdate(float currentTime, float deltaTime)
        {
            foreach (var store in _stores.Values)
            {
                store.Update(deltaTime);
            }

            float t = deltaTime % 1f;
            if (t <= 0.05f && shouldShuffle)
            {
                SeriesUtils.Shuffle(GetStore(PropertyId.Location).GetFullSeries());
            }
            if (t > 0.99 && shouldShuffle)
            {
                Series s = GetStore(PropertyId.Location).GetFullSeries();
                RandomSeries rs = (RandomSeries)s;
                rs.Seed = rs.Seed + 1;
            }
        }
        public virtual void EndUpdate(float currentTime, float deltaTime) { }
#endregion

#region Sampling
        public void AddLocalPropertiesAtT(Dictionary<PropertyId, Series> data, float t)
        {
	        foreach (var store in _stores)
	        {
		        if (!data.ContainsKey(store.Key))
		        {
			        data.Add(store.Key, null);
		        }
	        }
        }
        public virtual IRenderable QueryPropertiesAtT(Dictionary<PropertyId, Series> data, float t, bool addLocalProperties)
        {
	        IRenderable result = null;
	        if (addLocalProperties)
	        {
				AddLocalPropertiesAtT(data, t);
	        }

            var sample = SamplerUtils.GetSummedJaggedT(ChildCounts, (int)Math.Floor(t * (NestedItemCount - 1f) + 0.5f));
	        float indexT = sample.X;
	        float segmentT = sample.Y;
            if (_children.Count > 0)
            {
                int childIndex = (int)Math.Floor(indexT * (ChildCounts.Length - 0f) + 0.5f);
                float selfT = ChildCounts.Length > 1 ? childIndex / (ChildCounts.Length - 1f) : t;

                var keys = data.Keys.ToList();
                foreach (var key in keys)
                {
                    data[key] = GetSeriesAtT(key, selfT, data[key]);
                }
                childIndex = Math.Max(0, Math.Min(_children.Count - 1, childIndex));
                result = _children[childIndex].QueryPropertiesAtT(data, segmentT, addLocalProperties) ?? result;
            }
            else
            {
                var keys = data.Keys.ToList();
                foreach (var key in keys)
                {
                    data[key] = GetSeriesAtT(key, segmentT, data[key]);
                }
            }

            if (this is IDrawable drawable && drawable.Renderer != null)
            {
                result = drawable.Renderer;
            }
            return result;
        }

        public virtual Series GetSeriesAtT(PropertyId propertyId, float t, Series parentSeries)
        {
            var store = GetStore(propertyId);
            var result = store?.GetValuesAtT(t);
            if(parentSeries != null)
            {
                if (result != null)
                {
                    result.CombineInto(parentSeries, store.CombineFunction, t);
                }
                else
                {
                    result = parentSeries;
                }
            }
            return result;
        }

        public virtual Series GetSeriesAtIndex(PropertyId propertyId, int index, Series parentSeries)
        {
            var store = GetStore(propertyId);
            var result = store?.GetValuesAtIndex(index);
            if (parentSeries != null)
            {
                if (result != null)
                {
                    result.CombineInto(parentSeries, store.CombineFunction);
                }
                else
                {
                    result = parentSeries;
                }
            }
            return result;
        }

        public virtual Series GetNestedSeriesAtT(PropertyId propertyId, float t, Series parentSeries)
        {
	        return GetNestedSeriesAtIndex(propertyId, (int)(t * (NestedItemCount - 1f)), parentSeries);
        }
        public virtual Series GetNestedSeriesAtIndex(PropertyId propertyId, int index, Series parentSeries)
        {
	        // this uses t because many interpolations have no specific capacity information (eg a shared color store)
	        Series result;
	        var sample = SamplerUtils.GetSummedJaggedT(ChildCounts, index);
	        float indexT = sample.X;
	        float segmentT = sample.Y;
	        if (ChildCounts.Length <= 1)
	        {
		        result = GetSeriesAtT(propertyId, segmentT, parentSeries);
	        }
	        else
	        {
		        int childIndex = Math.Max(0, Math.Min(_children.Count - 1, (int)Math.Round(indexT * _children.Count)));
		        IContainer child = _children[childIndex];
				
		        float indexTNorm = indexT * (child.Capacity / (child.Capacity - 1f)); // normalize
		        Series val = GetSeriesAtT(propertyId, indexTNorm, parentSeries);
		        result = child.GetNestedSeriesAtT(propertyId, segmentT, val);
	        }
	        return result;
        }

        public virtual ParametricSeries GetSampledT(PropertyId propertyId, float t)
        {
            var store = GetStore(propertyId);
            return store != null ? store.GetSampledTs(t) : new ParametricSeries(1, t);
        }
#endregion

#region Draw
        public virtual void Draw(Graphics g, Dictionary<PropertyId, Series> dict)
        {
            var capacity = NestedItemCount;// NestedItemCountAtT(InterpolationT);
            if (capacity > 0)// != null)
            {
                for (int i = 0; i < capacity; i++)
                {
                    float indexT = i / (capacity - 1f);
                    dict.Clear();
                    IRenderable renderer = QueryPropertiesAtT(dict, indexT, true);
                    renderer?.DrawWithProperties(dict, g);
                }
            }
        }
#endregion
    }

}