using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Components.Transitions;
using DataArcs.SeriesData;

namespace DataArcs.Components
{
	public delegate void TransitionEventHandler(object sender, EventArgs e);

    public interface IContinuous : IComposite
    {
	    float InterpolationT { get; set; }
        //float DeltaTime { get; }
        //float CurrentTime { get; }
        //float PreviousTime { get; }

        float StartTime { get; set; }
        Series Duration { get; }
        Series Delay { get; }

        //bool IsReverse { get; }
        //bool IsComplete { get; }

        event TransitionEventHandler StartTransitionEvent;
        event TransitionEventHandler StepTransitionEvent;
        event TransitionEventHandler EndTransitionEvent;

        void Restart();
        void Reverse();
        //void Pause();
        //void Resume();
    }
}
