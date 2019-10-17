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
        public int Capacity => GetStore(PropertyId.Location)?.Sampler?.Capacity ?? Items.Capacity;
        public IComposite Background { get; set; }
        public string Name { get; set; }

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
                int result = 0;
                if (_children.Count > 0)
                {
                    for (int i = 0; i < Capacity; i++)
                    {
                        int index = Math.Min(_children.Count - 1, i);
                        result += _children[index].TotalItemCount;
                    }
                }
                else
                {
                    result = _items?.Capacity ?? 0;
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
                        result[i] = _children[index].TotalItemCount;
                    }
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
            child.Parent = this;
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

        public virtual void Draw(IComposite composite, Graphics g, Dictionary<PropertyId, Series> dict)
        {
	        IStore items = GetStore(PropertyId.Items) ?? GetStore(PropertyId.Location);
	        if (items != null)
	        {
		        int capacity = items.Capacity;
		        for (int i = 0; i < capacity; i++)
		        {
                    int index = i; // GetSeriesAtIndex(PropertyId.Items, i, null)?.IntDataAt(0) ?? i;
                    //Series v = GetSeriesAtIndex(PropertyId.Location, index, GetValueOrNull(dict, PropertyId.Location));
                    Series v = GetSeriesAtT(PropertyId.Location, index / (capacity - 1f), GetValueOrNull(dict, PropertyId.Location));
                    //Series v = GetChildSeriesAtIndex(PropertyId.Location, index, GetValueOrNull(dict, PropertyId.Location));
                    //Series v = GetChildSeriesAtT(PropertyId.Location, index / (capacity - 1f), GetValueOrNull(dict, PropertyId.Location));

                    // todo: swicth to matrix, child determines multiply add etc, get values from parents each call

                    Series temp = GetValueOrNull(dict, PropertyId.Location);
                    dict[PropertyId.Location] = v;
			        if (this is IDrawable selfDrawable)
			        {
                        var state = g.Save();
			            var scale = 1f; // + it * 0.8f;
			            //g.ScaleTransform(scale, scale);
			            g.TranslateTransform(v.X / scale, v.Y / scale);
				        selfDrawable.DrawAtT(index / (capacity - 1f), this, g, dict);
                        g.Restore(state);
                    }

                    foreach (var child in _children)
                    {
                        child.Draw(this, g, dict);
                    }
                    dict[PropertyId.Location] = temp;
                }
	        }
        }

        protected Series GetValueOrNull(Dictionary<PropertyId, Series> dict, PropertyId propertyId)
        {
            return dict.ContainsKey(propertyId) ? dict[propertyId] : null;
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

        public virtual Series GetChildSeriesAtT(PropertyId propertyId, float t, Series parentSeries)
        {
	        return GetChildSeriesAtIndex(propertyId, (int)(t * (TotalItemCount - 1f)), parentSeries);
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
        public virtual Series GetChildSeriesAtIndex(PropertyId propertyId, int index, Series parentSeries)
        {
	        // this uses t because many interpolations have no specific capacity information (eg a shared color store)
	        Series result;
	        SamplerUtils.GetSummedJaggedT(ChildCounts, index, out float indexT, out float segmentT);
	        if (ChildCounts.Length <= 1)
	        {
		        result = GetSeriesAtT(propertyId, segmentT, parentSeries);
	        }
	        else
	        {
		        int childIndex = Math.Max(0, Math.Min(_children.Count - 1, (int)Math.Round(indexT * _children.Count)));
		        IComposite composite = _children[childIndex];

		        // todo: adjust to make indexT 0-1
		        float indexTNorm = indexT * (composite.Capacity / (composite.Capacity - 1f)); // normalize

		        Series val = GetSeriesAtT(propertyId, indexTNorm, parentSeries);

		        if (propertyId == PropertyId.Location)
		        {
			        Debug.WriteLine(indexTNorm + " : " + segmentT + " :: " + propertyId);
		        }

		        result = composite.GetChildSeriesAtT(propertyId, segmentT, val);
	        }
	        return result;
        }

        public virtual ParametricSeries GetSampledT(PropertyId propertyId, float t)
        {
            var store = GetStore(propertyId);
            return store != null ? store.GetSampledTs(t) : new ParametricSeries(1, t);
        }

    }

}