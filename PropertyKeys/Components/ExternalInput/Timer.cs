using DataArcs.Players;
using DataArcs.SeriesData;
using DataArcs.Stores;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.Components.Transitions
{
    public class Timer : BaseComposite, ITimeable
    {
        public float InterpolationT { get; set; }
        public bool IsComplete { get; protected set; } = false;

        public float StartTime { get; set; }
        public Series Delay { get; }
        public Series Duration { get; }
        protected bool IsReverse { get; set; } = false;

        public event TimedEventHandler StartTimedEvent;
        public event TimedEventHandler StepTimedEvent;
        public event TimedEventHandler EndTimedEvent;

        public Timer(float delay = 0, float duration = 0, Store easing = null) : base()
        {
            Delay = new FloatSeries(1, delay);
            Duration = new FloatSeries(1, duration);
        }

        public void Restart()
        {
            StartTime = (float)(DateTime.Now - Player.StartTime).TotalMilliseconds;
            IsComplete = false;
        }
        public void Reverse()
        {
            IsReverse = !IsReverse;
        }
		
        public override void StartUpdate(float currentTime, float deltaTime)
        {
            float dur = Duration.X;
            if (currentTime > StartTime + dur)
            {
                IsComplete = true;
                InterpolationT = 1f;
            }
            else
            {
                //float t = deltaTime < _startTime ? 0 : deltaTime > _startTime + _duration.X ? 1f : (deltaTime - _startTime) / _duration.X;
                InterpolationT = currentTime < StartTime ? 0 :
                    currentTime > StartTime + dur ? 1f :
                    (currentTime - StartTime) / dur;
            }

            InterpolationT = IsReverse ? 1f - InterpolationT : InterpolationT;
        }
        public override void EndUpdate(float currentTime, float deltaTime)
        {
            if (IsComplete)
            {
                EndTimedEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        public override ParametricSeries GetSampledTs(PropertyId propertyId, ParametricSeries seriesT)
        {
	        ParametricSeries result;
	        if (propertyId == PropertyId.InterpolationT)
	        {
		        result = new ParametricSeries(1, InterpolationT);
	        }
	        else
	        {
		        result = base.GetSampledTs(propertyId, new ParametricSeries(1, InterpolationT)); // todo: include seriesT, probably needed when scrubbing.
	        }

	        return result;
        }

        public override Series GetSeriesAtIndex(PropertyId propertyId, int index, Series parentSeries)
        {
            return GetSeriesAtT(propertyId, index / Duration.X, parentSeries);
        }
        public override Series GetSeriesAtT(PropertyId propertyId, float t, Series parentSeries)
        {
	        Series result;
	        if (propertyId == PropertyId.InterpolationT)//PropertyIdSet.IsTSampling(propertyId))
	        {
				result = new ParametricSeries(1, InterpolationT); // this is straight timer lookup, so no ref to input t needed.
	        }
	        else
	        {
		        result = base.GetSampledTs(propertyId, new ParametricSeries(1, InterpolationT));
            }
	        return result;
        }
    }
}
