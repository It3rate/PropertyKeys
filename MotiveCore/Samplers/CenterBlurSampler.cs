﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Motive.SeriesData;

namespace Motive.Samplers.Utils
{
    public class CenterBlurSampler : GridSampler
    {
		public bool CenterOne { get; }
        // This can probably be a MutateTSampler.
	    public CenterBlurSampler(int[] strides, bool centerOne = true) : base(strides)
	    {
		    CenterOne = centerOne;
	    }

	    public override ParametricSeries GetSampledTs(ParametricSeries seriesT)
	    {
		    var sample = base.GetSampledTs(seriesT);
			// only using half the sin wave, so multiply by PI (vs 2PI)
		    float z = (float)(Math.Sin(sample.X * Math.PI) * Math.Sin(sample.Y * Math.PI));
		    z = CenterOne ? z : 1f - z;
		    var result = new ParametricSeries(3, new[] { sample.X, sample.Y, z });
		    return Swizzle(result, seriesT);
        }

    }
}
