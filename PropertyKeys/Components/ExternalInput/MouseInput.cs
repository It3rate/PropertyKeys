using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.SeriesData;

namespace DataArcs.Components.ExternalInput
{
    public class MouseInput : BaseComposite, ITimeable
    {
	    public float InterpolationT { get; set; }
	    public float StartTime { get; set; }
	    public Series Duration { get; }
	    public Series Delay { get; }
	    public event TimedEventHandler StartTimedEvent;
	    public event TimedEventHandler StepTimedEvent;
	    public event TimedEventHandler EndTimedEvent;

	    public void Restart()
	    {
		    throw new NotImplementedException();
	    }

	    public void Reverse()
	    {
		    throw new NotImplementedException();
	    }
    }
}
