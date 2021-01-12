using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MotiveCore.SeriesData;
using MotiveCore.SeriesData.Utils;

namespace MotiveCore.Samplers
{
    public class MappingSampler : Sampler
    {
	    private readonly FloatEquation _floatEquation;

	    public MappingSampler(FloatEquation floatEquation, int sampleCount = 1) : base(sampleCount: sampleCount)
	    {
		    _floatEquation = floatEquation;
	    }

	    public override Series GetValuesAtT(Series series, float t)
	    {
		    Series result = series.GetVirtualValueAt(t);
		    result.Map(_floatEquation);
		    return result;
	    }
    }
}
