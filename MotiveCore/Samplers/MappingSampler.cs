using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Motive.SeriesData;
using Motive.SeriesData.Utils;

namespace Motive.Samplers
{
    public class MappingSampler : Sampler
    {
	    private readonly FloatEquation _floatEquation;

	    public MappingSampler(FloatEquation floatEquation, int sampleCount = 1) : base(sampleCount: sampleCount)
	    {
		    _floatEquation = floatEquation;
	    }

	    public override ISeries GetValuesAtT(ISeries series, float t)
	    {
		    ISeries result = series.GetVirtualValueAt(t);
		    result.Map(_floatEquation);
		    return result;
	    }
    }
}
