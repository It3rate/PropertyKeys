using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.SeriesData.Utils;

namespace DataArcs.Samplers
{
    public class UILayoutSampler : Sampler
    {
	    public UILayoutSampler(int[] strides, Slot[] swizzleMap = null, GrowthType growthType = GrowthType.Product) : base(swizzleMap)
	    {
		    GrowthType = growthType;
		    Strides = strides;
		    SampleCount = StridesToSampleCount(Strides);

		    ClampTypes = new ClampType[strides.Length];
		    for (int i = 0; i < strides.Length - 1; i++)
		    {
			    ClampTypes[i] = ClampType.Wrap;
		    }
		    ClampTypes[strides.Length - 1] = ClampType.None;
		    AlignmentTypes = new AlignmentType[strides.Length];
	    }
    }
}
