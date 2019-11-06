using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Components.Transitions;
using DataArcs.SeriesData;

namespace DataArcs.Components
{
	public delegate void TimedEventHandler(object sender, EventArgs e);

    public interface ITimeable : IComposite
    {
	    float InterpolationT { get; set; }
        //float DeltaTime { get; }
        //float _currentTime { get; }
        //float PreviousTime { get; }

        float StartTime { get; set; }
        Series Duration { get; }
        Series Delay { get; }

        //bool IsReverse { get; }
        //bool IsComplete { get; }

        event TimedEventHandler StartTimedEvent;
        event TimedEventHandler StepTimedEvent;
        event TimedEventHandler EndTimedEvent;

        void Restart();
        void Reverse();
        //void Pause();
        //void Resume();
    }
}
