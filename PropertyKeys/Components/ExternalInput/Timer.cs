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

        public double StartTime { get; set; }
        private double _runningTime;
        private double _currentTime;
        private DateTime _pauseTime;
        private bool _isPaused;
        private float _delayTime = 0;
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
            _pauseTime = DateTime.Now;
        }

        public void Restart()
        {
            StartTime = (float)(DateTime.Now - Player.StartTime).TotalMilliseconds;
	        _currentTime = StartTime;
            _runningTime = 0;
            _delayTime = 0;
            IsComplete = false;
            StartTimedEvent?.Invoke(this, EventArgs.Empty);
        }
        public void Reverse()
        {
            IsReverse = !IsReverse;
        }
		
        public override void StartUpdate(double ct, double deltaTime)
        {
            if (!_isPaused)
            {
                _runningTime += deltaTime + _delayTime;
                _delayTime = 0;
                _currentTime = StartTime + _runningTime;
                float dur = Duration.X;
                if (_currentTime > StartTime + dur)
                {
                    IsComplete = true;
                    InterpolationT = 1f;
                }
                else
                {
                    InterpolationT = (float)(_currentTime < StartTime ? 0 :
                        _currentTime > StartTime + dur ? 1f :
                        (_currentTime - StartTime) / dur);
                }

                InterpolationT = IsReverse ? 1f - InterpolationT : InterpolationT;

                StepTimedEvent?.Invoke(this, EventArgs.Empty);
            }
        }
        public override void EndUpdate(double currentTime, double deltaTime)
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

        public void Pause()
        {
            _isPaused = true;
            _pauseTime = DateTime.Now;
        }

        public void Resume()
        {
            _isPaused = false;
            _delayTime = (float)(DateTime.Now - _pauseTime).TotalMilliseconds;
        }
    }
}
