using DataArcs.Components;
using DataArcs.Stores;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Players;
using DataArcs.SeriesData;

namespace DataArcs.Transitions
{
    public class BlendTransition : CompositeBase
    {
	    private Store _easing;
		
        private float _startTime; // todo: All time should be one class, maybe even a store.
        private Series _delay;
        private Series _duration;

        private readonly Dictionary<PropertyId, BlendStore> _blends = new Dictionary<PropertyId, BlendStore>();

        public CompositeBase Start { get; }
        public CompositeBase End { get; }

        public BlendTransition(CompositeBase start, CompositeBase end, float delay = 0, float startTime = -1, float duration = 0, Store easing = null)
        {
	        Start = start;
	        End = end;
	        _delay = new FloatSeries(1, delay);
	        _startTime = startTime < 0 ? (float)(DateTime.Now - Player.StartTime).TotalMilliseconds : startTime;
	        _duration = new FloatSeries(1, duration);
	        _easing = easing;
	        GenerateBlends(); // eventually immutable after creation, so no new blends
        }

        public BlendTransition(CompositeBase start, CompositeBase end, Series delay, float startTime, Series duration)
        {
	        Start = start;
	        End = end;
	        _delay = delay;
	        _startTime = startTime < 0 ? (float)(DateTime.Now - Player.StartTime).TotalMilliseconds : startTime;
	        _duration = duration;
	        GenerateBlends();
        }

        private void GenerateBlends()
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

        public override void Update(float currentTime, float deltaTime)
        {
	        float dur = _duration.FloatDataAt(0);
	        if (currentTime > _startTime + dur)
	        {
		        _startTime += dur;
	        }
            //float t = deltaTime < _startTime ? 0 : deltaTime > _startTime + _duration.FloatDataAt(0) ? 1f : (deltaTime - _startTime) / _duration.FloatDataAt(0);
            CurrentT = currentTime < _startTime ? 0 : 
		        currentTime > _startTime + dur ? 1f : 
		        (currentTime - _startTime) / dur;

            Start.Update(currentTime, deltaTime);
            End.Update(currentTime, deltaTime);
            foreach (var item in _blends.Values)
            {
                item.Update(CurrentT);
            }
        }

        public override Series GetSeriesAtIndex(PropertyId propertyId, int index)
        {
            Series result;
            if (_blends.ContainsKey(propertyId))
            {
                result = Start.GetSeriesAtIndex(propertyId, index);
                Series end = End.GetSeriesAtIndex(propertyId, index);

                float indexT = index / (Start.GetStore(propertyId).Capacity - 1f) + CurrentT; // delay per element.

                float easedT = _easing?.GetSeriesAtT(CurrentT * indexT).FloatDataAt(0) ?? CurrentT;
                result.InterpolateInto(end, easedT);
            }
            else
            {
                var store = Start.GetStore(propertyId) ?? End.GetStore(propertyId);
                result = store != null ? store.GetSeriesAtIndex(index) : SeriesUtils.GetZeroFloatSeries(1, 0);
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
                //float delT = _delay.GetValueAtT(t).FloatDataAt(0);
                //float durT = _duration.GetValueAtT(t).FloatDataAt(0);
                //float delRatio = delT / (delT + durT);
                //float blendT = delRatio < t || delRatio <= 0 ? 0 : (t - delRatio) * (1f / delRatio);
                float easedT = _easing?.GetSeriesAtT(CurrentT).FloatDataAt(0) ?? CurrentT;
                result.InterpolateInto(end, easedT);
            }
	        else
	        {
		        var store = Start.GetStore(propertyId) ?? End.GetStore(propertyId);
		        result = store != null ? store.GetSeriesAtT(t) : SeriesUtils.GetZeroFloatSeries(1, 0);
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
            Start.GetDefinedStores(ids);
            End.GetDefinedStores(ids);
        }

        public override CompositeBase CreateChild()
        {
            throw new NotImplementedException();
        }
    }
}
