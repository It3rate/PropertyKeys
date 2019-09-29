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

        public IComposite Start { get; set; }
        public IComposite End { get; set; }

        public BlendTransition(IComposite start, IComposite end, float delay = 0, float startTime = -1, float duration = 0, Store easing = null):
            base(delay, startTime, duration, easing)
        {
	        Start = start;
	        End = end;
	        GenerateBlends(); // eventually immutable after creation, so no new blends
        }

        //public BlendTransition(IComposite start, IComposite end, Series delay, float startTime, Series duration) :
        //    base(delay, startTime, duration, easing)
        //{
	       // Start = start;
	       // End = end;
	       // GenerateBlends();
        //}

        public void GenerateBlends()
        {
	        Graphic = Start.Graphic;

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

        public override Series GetSeriesAtIndex(PropertyId propertyId, int index)
        {
            Series result;
            if (_blends.ContainsKey(propertyId))
            {
                result = Start.GetSeriesAtIndex(propertyId, index);
                Series end = End.GetSeriesAtIndex(propertyId, index);

                float indexT = index / (Start.GetStore(propertyId).Capacity - 1f) + InputT; // delay per element.

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
        public override Series GetSeriesAtT(PropertyId propertyId, float t)
        {
	        Series result;
	        if (_blends.ContainsKey(propertyId))
	        {
		        result = Start.GetSeriesAtT(propertyId, t);
		        Series end = End.GetSeriesAtT(propertyId, t);
                //float delT = _delay.GetValueAtT(t).X;
                //float durT = _duration.GetValueAtT(t).X;
                //float delRatio = delT / (delT + durT);
                //float blendT = delRatio < t || delRatio <= 0 ? 0 : (t - delRatio) * (1f / delRatio);
                float easedT = Easing?.GetValuesAtT(InputT).X ?? InputT;
                result.InterpolateInto(end, easedT);
            }
	        else
	        {
		        var store = Start.GetStore(propertyId) ?? End.GetStore(propertyId);
		        result = store != null ? store.GetValuesAtT(t) : SeriesUtils.GetZeroFloatSeries(1, 0);
            }
	        return result;
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
