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

namespace DataArcs.Transitions
{
	public delegate void TransitionEventHandler(object sender, EventArgs e);

    public class BlendTransition : CompositeBase
    {
	    public IStore Easing { get; set; }

	    private float _startTime; // todo: All time should be one class, maybe even a store.
        private Series _delay;
        private Series _duration;

        private bool _isComplete = false;
        private bool _isReverse = false;

        private readonly Dictionary<PropertyId, BlendStore> _blends = new Dictionary<PropertyId, BlendStore>();

        public IComposite Start { get; set; }
        public IComposite End { get; set; }

        public event TransitionEventHandler StartTransitionEvent;
        public event TransitionEventHandler StepTransitionEvent;
        public event TransitionEventHandler EndTransitionEvent;

        public BlendTransition(IComposite start, IComposite end, float delay = 0, float startTime = -1, float duration = 0, Store easing = null)
        {
	        Start = start;
	        End = end;
	        _delay = new FloatSeries(1, delay);
	        _startTime = startTime < 0 ? (float)(DateTime.Now - Player.StartTime).TotalMilliseconds : startTime;
	        _duration = new FloatSeries(1, duration);
	        Easing = easing;
	        GenerateBlends(); // eventually immutable after creation, so no new blends
        }

        public BlendTransition(IComposite start, IComposite end, Series delay, float startTime, Series duration)
        {
	        Start = start;
	        End = end;
	        _delay = delay;
	        _startTime = startTime < 0 ? (float)(DateTime.Now - Player.StartTime).TotalMilliseconds : startTime;
	        _duration = duration;
	        GenerateBlends();
        }

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

        public void Restart()
        {
	        _startTime = (float) (DateTime.Now - Player.StartTime).TotalMilliseconds;
	        _isComplete = false;
        }

        public void Reverse()
        {
	        _isReverse = !_isReverse;
        }

        public override void Update(float currentTime, float deltaTime)
        {
	        if (_isComplete) return;

	        float dur = _duration.X;
	        if (currentTime > _startTime + dur)
	        {
		        _isComplete = true;
		        CurrentT = 1f;
	        }
	        else
	        {
		        //float t = deltaTime < _startTime ? 0 : deltaTime > _startTime + _duration.X ? 1f : (deltaTime - _startTime) / _duration.X;
		        CurrentT = currentTime < _startTime ? 0 :
			        currentTime > _startTime + dur ? 1f :
			        (currentTime - _startTime) / dur;
	        }

	        CurrentT = _isReverse ? 1f - CurrentT : CurrentT;

            Start.Update(currentTime, deltaTime);
            End.Update(currentTime, deltaTime);
            foreach (var item in _blends.Values)
            {
                item.Update(CurrentT);
            }

            if (_isComplete)
            {
				EndTransitionEvent?.Invoke(this, EventArgs.Empty);
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

                float easedT = Easing?.GetValuesAtT(CurrentT * indexT).X ?? CurrentT;
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
                float easedT = Easing?.GetValuesAtT(CurrentT).X ?? CurrentT;
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
            Start.GetDefinedStores(ids);
            End.GetDefinedStores(ids);
        }

        public override IComposite CreateChild()
        {
            throw new NotImplementedException();
        }
    }
}
