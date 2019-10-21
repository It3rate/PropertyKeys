using DataArcs.Players;
using DataArcs.SeriesData;
using DataArcs.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.Components.Transitions
{
	public class Animation : Container
    {
	    public float InterpolationT { get; set; }
        public IStore Easing { get; set; }
        public bool IsComplete { get; protected set; } = false;

        protected float _startTime; // todo: All time should be one class, maybe even a store.
        protected Series _delay;
        protected Series _duration;
        protected bool _isReverse = false;

        public event TransitionEventHandler StartTransitionEvent;
        public event TransitionEventHandler StepTransitionEvent;
        public event TransitionEventHandler EndTransitionEvent;

        public Animation(float delay = 0, float startTime = -1, float duration = 0, Store easing = null):base(null)
        {
            _delay = new FloatSeries(1, delay);
            _startTime = startTime < 0 ? (float)(DateTime.Now - Player.StartTime).TotalMilliseconds : startTime;
            _duration = new FloatSeries(1, duration);
            Easing = easing;
        }

        public void Restart()
        {
            _startTime = (float)(DateTime.Now - Player.StartTime).TotalMilliseconds;
            IsComplete = false;
        }
        public void Reverse()
        {
            _isReverse = !_isReverse;
        }

        public override void StartUpdate(float currentTime, float deltaTime)
        {
            float dur = _duration.X;
            if (currentTime > _startTime + dur)
            {
                IsComplete = true;
                InterpolationT = 1f;
            }
            else
            {
                //float t = deltaTime < _startTime ? 0 : deltaTime > _startTime + _duration.X ? 1f : (deltaTime - _startTime) / _duration.X;
                InterpolationT = currentTime < _startTime ? 0 :
                    currentTime > _startTime + dur ? 1f :
                    (currentTime - _startTime) / dur;
            }

            InterpolationT = _isReverse ? 1f - InterpolationT : InterpolationT;
        }
        public override void EndUpdate(float currentTime, float deltaTime)
        {
            if (IsComplete)
            {
                EndTransitionEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        public override IStore GetStore(PropertyId propertyId)
        {
            IStore result = null;
            switch (propertyId)
            {
                case PropertyId.EasedT:
                case PropertyId.EasedTCombined:
                    result = new FloatSeries(1, Easing?.GetValuesAtT(InterpolationT).X ?? InterpolationT).Store;
                    break;
                case PropertyId.SampleAtT:
                case PropertyId.SampleAtTCombined:
                    result = new FloatSeries(1, InterpolationT).Store;
                    break;
				default:
					result = base.GetStore(propertyId);
					break;
            }
            return result;
        }
        public override void GetDefinedStores(HashSet<PropertyId> ids)
        {
            ids.Add(PropertyId.EasedTCombined);
        }
    }
}
