using DataArcs.Components;
using DataArcs.Stores;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Players;
using DataArcs.SeriesData;
using DataArcs.Components.Transitions;
using DataArcs.Graphic;

namespace DataArcs.Components.Transitions
{
    public class BlendTransition : Animation
    {
        private readonly Dictionary<PropertyId, BlendStore> _blends = new Dictionary<PropertyId, BlendStore>();

        public IComposite Start { get; set; }
        public IComposite End { get; set; }

        public int[] StartStrides { get; }
        public int[] EndStrides { get; }

        public override int NestedItemCount
        {
            get
            {
                var startCount = Start.NestedItemCount;
                var endCount = End.NestedItemCount;
                return (int)Math.Max(startCount, endCount);
            }
        }
        public override int NestedItemCountAtT(float t)
        {
            var startCount = Start.NestedItemCount;
            var endCount = End.NestedItemCount;
            return (int)(startCount + (endCount - startCount) * t);
        }

        public BlendTransition(IComposite start, IComposite end, float delay = 0, float startTime = -1, float duration = 0, Store easing = null):
            base(delay, startTime, duration, easing)
        {
	        Start = start;
	        End = end;
            StartStrides = start.ChildCounts; // todo:  include own items
            EndStrides = end.ChildCounts;
            GenerateBlends(); // eventually immutable after creation, so no new blends
        }
        
        public void GenerateBlends()
        {
            // todo: render children maps from subcollection to subcollection as needed.
            // to do this, don't precompute blends as the composite target may change as t drifts over the child elements.
            Renderer = (Start is IDrawable) ? ((IDrawable)Start).Renderer : null;

            _blends.Clear();
            HashSet<PropertyId> commonKeys = new HashSet<PropertyId>();
            HashSet<PropertyId> endKeys = new HashSet<PropertyId>();
            Start.GetDefinedStores(commonKeys);
            End.GetDefinedStores(endKeys);
            commonKeys.IntersectWith(endKeys);
            foreach (var key in commonKeys)
            {
	            if (Start.GetStore(key).StoreId != End.GetStore(key).StoreId)
	            {
					_blends[key] = (BlendStore)GetStore(key);
	            }
            }
        }

        public override void StartUpdate(float currentTime, float deltaTime)
        {
            base.StartUpdate(currentTime, deltaTime);

            Start.Update(currentTime, deltaTime);
            End.Update(currentTime, deltaTime);
            foreach (var item in _blends.Values)
            {
                item.Update(InputT);
            }
        }

        public override IStore GetStore(PropertyId propertyId)
        {
	        IStore result;
	        if (_blends.ContainsKey(propertyId))
	        {
		        result = _blends[propertyId];
	        }
	        else
	        {
		        result = Start.GetStore(propertyId);
		        IStore end = End.GetStore(propertyId);
		        if (result == null)
		        {
			        result = end;
		        }
		        else if (end != null)
		        {
			        result = new BlendStore(result, end);
		        }
	        }
	        return result;
        }
        public override void GetDefinedStores(HashSet<PropertyId> ids)
        {
	        base.GetDefinedStores(ids);
	        Start.GetDefinedStores(ids);
	        End.GetDefinedStores(ids);
        }

        public override Series GetSeriesAtIndex(PropertyId propertyId, int index, Series parentSeries)
        {
			// Uses total item count, but may want to look up separate counts? Or put blend counts on a delegate function.
	        return GetSeriesAtT(propertyId, index / (float)NestedItemCount, parentSeries);
        }
        public override Series GetSeriesAtT(PropertyId propertyId, float t, Series parentSeries)
        {
            var startDict = new Dictionary<PropertyId, Series>() { { propertyId, null } };
            Start.QueryPropertiesAtT(startDict, t, false);
            Series result = startDict[propertyId];
            if (_blends.ContainsKey(propertyId))
            {
                var endDict = new Dictionary<PropertyId, Series>() { { propertyId, null } };
                End.QueryPropertiesAtT(endDict, t, false);

                float indexT = t + InputT; // delay per element.

                float easedT = Easing?.GetValuesAtT(InputT * indexT).X ?? InputT;
                result.InterpolateInto(endDict[propertyId], easedT);
            }
            else if(result == null)
            {
                result = End.GetNestedSeriesAtT(propertyId, t, parentSeries) ?? SeriesUtils.GetZeroFloatSeries(1, 0);
            }
            return result;
        }
        public override IRenderable QueryPropertiesAtT(Dictionary<PropertyId, Series> data, float t, bool addLocalProperties)
        {
            var endDict = new Dictionary<PropertyId, Series>(data);
            IRenderable result = Start.QueryPropertiesAtT(data, t, true);
            result = End.QueryPropertiesAtT(endDict, t, true) ?? result;

            float indexT = t + InputT; // delay per element.
            float easedT = Easing?.GetValuesAtT(InputT * indexT).X ?? InputT;
            foreach (var key in endDict.Keys)
            {
                if (data.TryGetValue(key, out Series value))
                {
                    value.InterpolateInto(endDict[key], easedT);
                }
                else
                {
                    data[key] = endDict[key];
                }
            }

            return result;
        }

    }
}
