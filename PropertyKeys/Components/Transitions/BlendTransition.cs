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

namespace DataArcs.Components.Transitions
{
    public class BlendTransition : Animation
    {
        private readonly Dictionary<PropertyId, BlendStore> _blends = new Dictionary<PropertyId, BlendStore>();
        private readonly List<PropertyId> _blendProperties = new List<PropertyId>();

        public IComposite Start { get; set; }
        public IComposite End { get; set; }

        public int[] StartStrides { get; }
        public int[] EndStrides { get; }

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
        public override Series GetSeriesAtIndex(PropertyId propertyId, int index, Series parentSeries)
        {
            Series result;
            if (_blends.ContainsKey(propertyId))
            {
	            int startCount = Start.TotalItemCount;
	            int endCount = End.TotalItemCount;
                result = Start.GetChildSeriesAtT(propertyId, index / (startCount - 1f), parentSeries);
                Series end = End.GetChildSeriesAtT(propertyId, index / (endCount - 1f), parentSeries);

                int finalCount = startCount + (int)((endCount - startCount) * InputT);
                float indexT = index / (finalCount - 1f) + InputT; // delay per element.

                float easedT = Easing?.GetValuesAtT(InputT * indexT).X ?? InputT;
                result.InterpolateInto(end, easedT);
            }
            else
            {
                var store = Start.GetStore(propertyId) ?? End.GetStore(propertyId);
                result = store != null ? store.GetValuesAtIndex(index) : SeriesUtils.GetZeroFloatSeries(1, 0);
            }
            return result;
        }
        public override Series GetSeriesAtT(PropertyId propertyId, float t, Series parentSeries)
        {
            Series result = Start.GetChildSeriesAtT(propertyId, t, parentSeries);
            if (_blends.ContainsKey(propertyId))
            {
                Series end = End.GetChildSeriesAtT(propertyId, t, parentSeries);

                float indexT = t + InputT; // delay per element.

                float easedT = Easing?.GetValuesAtT(InputT * indexT).X ?? InputT;
                result.InterpolateInto(end, easedT);
            }
            else if(result == null)
            {
                result = End.GetChildSeriesAtT(propertyId, t, parentSeries) ?? SeriesUtils.GetZeroFloatSeries(1, 0);
            }
            return result;

         //   Series result;
	        //if (_blends.ContainsKey(propertyId))
	        //{
		       // result = Start.GetSeriesAtT(propertyId, t, parentSeries);
		       // Series end = End.GetSeriesAtT(propertyId, t, parentSeries);
         //       //float delT = _delay.GetValueAtT(t).X;
         //       //float durT = _duration.GetValueAtT(t).X;
         //       //float delRatio = delT / (delT + durT);
         //       //float blendT = delRatio < t || delRatio <= 0 ? 0 : (t - delRatio) * (1f / delRatio);
         //       float easedT = Easing?.GetValuesAtT(InputT).X ?? InputT;
         //       result.InterpolateInto(end, easedT);
         //   }
	        //else
	        //{
		       // var store = Start.GetStore(propertyId) ?? End.GetStore(propertyId);
		       // result = store != null ? store.GetValuesAtT(t) : SeriesUtils.GetZeroFloatSeries(1, 0);
         //   }
	        //return result;
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

        public override void AddProperty(PropertyId id, IStore store) { throw new NotImplementedException(); }
        public override void AppendProperty(PropertyId id, IStore store) { throw new NotImplementedException(); }
        public override void RemoveProperty(PropertyId id, BlendStore store) { throw new NotImplementedException(); }
        public override IComposite CreateChild()
        {
            throw new NotImplementedException();
        }
    }
}
