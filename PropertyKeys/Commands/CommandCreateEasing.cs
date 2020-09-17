using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Samplers;
using DataArcs.SeriesData;
using DataArcs.SeriesData.Utils;

namespace DataArcs.Commands
{
    public class CommandCreateEasing : CommandBase
    {
	    private Easing Easing;

	    private readonly EasingType[] _easingTypes;
	    private readonly Slot[] _swizzleMap;
	    private readonly int _sampleCount;
	    private readonly bool _clamp;


        public CommandCreateEasing(EasingType[] easingTypes, Slot[] swizzleMap = null, int sampleCount = 1, bool clamp = false)
        {
	        _easingTypes = easingTypes;
	        _swizzleMap = swizzleMap;
	        _sampleCount = sampleCount;
	        _clamp = clamp;
        }

	    public override void Execute()
	    {
			Easing = new Easing(_easingTypes, _swizzleMap, _sampleCount, _clamp);
	    }

	    public override void UnExecute()
	    {
		    throw new NotImplementedException();
	    }

	    public override void Update(double time)
	    {
		    throw new NotImplementedException();
	    }

	    public override void Draw(Graphics graphics)
	    {
		    throw new NotImplementedException();
	    }
    }
}
