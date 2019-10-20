using DataArcs.SeriesData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.Samplers
{
    public class CenterBlurSampler : GridSampler
    {
		public bool CenterOne { get; }
	    public CenterBlurSampler(int[] strides, bool centerOne = true) : base(strides)
	    {
		    CenterOne = centerOne;
	    }

	    public override ParametricSeries GetSampledTs(float t)
	    {
		    var result = base.GetSampledTs(t);
		    float z = (float)(Math.Sin(result.X * Math.PI) * Math.Sin(result.Y * Math.PI));
		    z = CenterOne ? z : 1f - z;
		    return new ParametricSeries(3, new[] { result.X, result.Y, z });
	    }

    }
}
