using DataArcs.Components;
using DataArcs.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Players;
using DataArcs.SeriesData;

namespace DataArcs.Transitions
{
    public class BlendTransition : CompositeBase
    {
        private float _startTime; // todo: All time should be one class, maybe even a store.
        private Series _delay;
        private Series _duration;

        private Dictionary<PropertyId, BlendStore> _blends = new Dictionary<PropertyId, BlendStore>();

        public Composite Start { get; }
        public Composite End { get; }

        public BlendTransition(Composite start, Composite end, float delay = 0, float startTime = -1, float duration = 0)
        {
	        Start = start;
	        End = end;
	        _delay = new FloatSeries(1, delay);
	        _startTime = startTime < 0 ? (float)(DateTime.Now - Player.StartTime).TotalMilliseconds : startTime;
	        _duration = new FloatSeries(1, duration);
	        GenerateBlends(); // eventually immutable after creation, so no new blends
        }

        public BlendTransition(Composite start, Composite end, Series delay, float startTime, Series duration)
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
					_blends.Add(key, (BlendStore)GetStore(key));
	            }
            }
        }

        public override void Update(float currentTime, float deltaTime)
        {
            //float t = deltaTime < _startTime ? 0 : deltaTime > _startTime + _duration ? 1f : (deltaTime - _startTime) / _duration;
            foreach (var item in _blends.Values)
            {
                item.Update(deltaTime);
            }
        }

        public override Series GetSeriesAtT(PropertyId propertyId, float t, int virtualCount = -1)
        {
	        Series result;
	        if (_blends.ContainsKey(propertyId))
	        {
		        result = Start.GetSeriesAtT(propertyId, t, virtualCount);
		        Series end = End.GetSeriesAtT(propertyId, t, virtualCount);
		        float delT = _delay.GetValueAtT(t).FloatDataAt(0);
		        float durT = _delay.GetValueAtT(t).FloatDataAt(0);
		        float delRatio = delT / (delT + durT);
		        float blendT = delRatio < t ? 0 : (t - delRatio) * (1f / delRatio);
		        result.InterpolateInto(end, blendT);
            }
	        else
	        {
		        var store = Start.GetStore(propertyId) ?? End.GetStore(propertyId);
		        result = store != null ? store.GetSeriesAtT(t, virtualCount) : SeriesUtils.GetZeroFloatSeries(1, 0);
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
