using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Motive.SeriesData;
using Motive.SeriesData.Utils;

namespace Motive.Samplers.Utils
{
    public class ChainedSampler : Sampler
    {
        private readonly List<int> _samplers;

        public ChainedSampler(Slot[] swizzleMap, params Sampler[] samplers) : 
            base(swizzleMap)
        {
	        _samplers = new List<int>(samplers.Length);
	        foreach (var sampler in samplers)
	        {
		        _samplers.Add(sampler.Id);
	        }
        }
        public ChainedSampler(params Sampler[] samplers)
        {
	        _samplers = new List<int>(samplers.Length);
	        foreach (var sampler in samplers)
	        {
		        _samplers.Add(sampler.Id);
	        }
        }

        public override ParametricSeries GetSampledTs(ParametricSeries seriesT)
        {
            ParametricSeries result = seriesT;
            foreach (var samplerId in _samplers)
            {
                result = SamplerUtils.GetSamplerById(samplerId).GetSampledTs(result);
            }

            return Swizzle(result, seriesT);
        }
    }
}
