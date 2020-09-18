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
using DataArcs.SeriesData.Utils;
using DataArcs.Stores;

namespace DataArcs.Components
{
	public class Container : BaseComposite, IContainer
	{
		private readonly List<IContainer> _children = new List<IContainer>();
		
        public IContainer Parent { get; set; }
        public IRenderable Renderer { get; set; }
		
        // todo: move away from location being so important.
        public override int Capacity => Math.Max(GetStore(PropertyId.Location)?.Capacity ?? 0, GetStore(PropertyId.Items).Capacity);

        protected Container(IStore items)
        {
            if (items != null)
            {
                AddProperty(PropertyId.Items, items);
            }
        }
        public Container(IStore items = null, IContainer parent = null, IRenderable renderer = null) : this(items)
        {
            Parent = parent;
            Renderer = renderer;
        }

        #region Elements
		
        public override IStore GetStore(PropertyId propertyId)
		{
			var result = base.GetStore(propertyId);
			if (result == null && Parent != null)
			{
				result = Parent.GetStore(propertyId);
			}

			return result;
		}
		public override void GetDefinedStores(HashSet<PropertyId> ids)
		{
			base.GetDefinedStores(ids);
			Parent?.GetDefinedStores(ids);
		}

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
                    result = GetStore(PropertyId.Items)?.Capacity ?? 0;
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

        public bool shouldShuffle; // temp basis for switching to events
        public override void StartUpdate(double currentTime, double deltaTime)
        {
			base.StartUpdate(currentTime, deltaTime);
            float t = (float)(deltaTime % 1.0);
            if (t <= 0.05f && shouldShuffle)
            {
                SeriesUtils.ShuffleElements(GetStore(PropertyId.Location).GetSeriesRef());
            }
            if (t > 0.99 && shouldShuffle)
            {
                Series s = GetStore(PropertyId.Location).GetSeriesRef();
                RandomSeries rs = (RandomSeries)s;
                rs.Seed = rs.Seed + 1;
            }
        }
#endregion

#region Sampling
        public void AddLocalPropertiesAtT(Dictionary<PropertyId, Series> data, float t)
        {
	        foreach (var store in _properties)
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

	        int capacity = NestedItemCount;
	        var sample = SamplerUtils.GetSummedJaggedT(ChildCounts, SamplerUtils.IndexFromT(NestedItemCount, t));// (int)Math.Floor(t * (NestedItemCount - 1f) + 0.5f));
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

        public override Series GetSeriesAtT(PropertyId propertyId, float t, Series parentSeries)
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

        public override Series GetSeriesAtIndex(PropertyId propertyId, int index, Series parentSeries)
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
        public override ParametricSeries GetSampledTs(PropertyId propertyId, ParametricSeries seriesT)
        {
            var store = GetStore(propertyId);
            return store != null ? store.GetSampledTs(seriesT) : seriesT;
        }

        public virtual Series GetNestedSeriesAtT(PropertyId propertyId, float t, Series parentSeries)
        {
	        return GetNestedSeriesAtIndex(propertyId, SamplerUtils.IndexFromT(NestedItemCount, t), parentSeries); // int)(t * (NestedItemCount - 1f)), parentSeries);
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
		        int childIndex = SamplerUtils.IndexFromT(_children.Count, indexT); // Math.MaxSlots(0, Math.MinSlots(_children.Count - 1, (int)Math.Round(indexT * _children.Count)));
		        IContainer child = _children[childIndex];
				
		        float indexTNorm = indexT * (child.Capacity / (child.Capacity - 1f)); // normalize
		        Series val = GetSeriesAtT(propertyId, indexTNorm, parentSeries);
		        result = child.GetNestedSeriesAtT(propertyId, segmentT, val);
	        }
	        return result;
        }
#endregion

#region Draw
        public virtual void Draw(Graphics g, Dictionary<PropertyId, Series> dict)
        {
            var capacity = NestedItemCount;// NestedItemCountAtT(InterpolationT);
            if (capacity > 0)// != null)
            {
				SortedList<int, int> sl = new SortedList<int, int>();
				var items = GetLocalStore(PropertyId.Items);
                for (int i = 0; i < capacity; i++)
                {
	                int itemIndex = items?.GetValuesAtIndex(i).IntDataAt(0) ?? i;

                    float indexT = itemIndex / (capacity - 1f);
                    dict.Clear();
                    IRenderable renderer = QueryPropertiesAtT(dict, indexT, true);
                    renderer?.DrawWithProperties(dict, g);
                }
            }
        }
        #endregion
        
    }
}