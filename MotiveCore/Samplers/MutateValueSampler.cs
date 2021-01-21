using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Motive.SeriesData;
using Motive.SeriesData.Utils;

namespace Motive.Samplers
{
    public class MutateValueSampler : Sampler
    {
	    private readonly FloatEquation _floatEquation;

	    public MutateValueSampler(FloatEquation floatEquation, int sampleCount = 1) : base(sampleCount: sampleCount)
	    {
		    _floatEquation = floatEquation;
	    }

	    public override ISeries GetValuesAtT(ISeries series, float t)
	    {
		    ISeries result = series.GetInterpolatedSeriesAt(t);
		    result.Map(_floatEquation);
		    return result;
	    }
    }
}
