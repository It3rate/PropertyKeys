using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MotiveCore.Components.Transitions;
using MotiveCore.SeriesData;

namespace MotiveCore.Components
{
	public delegate void TimedEventHandler(object sender, EventArgs e);

    public interface ITimeable : IComposite
    {
	    float InterpolationT { get; set; }
        //double DeltaTime { get; }
        //double _currentTime { get; }
        //double PreviousTime { get; }

        double StartTime { get; set; }
        Series Duration { get; }
        Series Delay { get; }

        //bool IsReverse { get; }
        //bool IsComplete { get; }

        event TimedEventHandler StartTimedEvent;
        event TimedEventHandler StepTimedEvent;
        event TimedEventHandler EndTimedEvent;

        void Restart();
        void Reverse();
        
        void Pause();
        void Resume();
    }
}
