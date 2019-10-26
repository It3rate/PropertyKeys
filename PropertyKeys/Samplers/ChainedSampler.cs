using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.SeriesData;

namespace DataArcs.Samplers
{
    public class ChainedSampler : Sampler
    {
        private readonly List<Sampler> _samplers;

        public ChainedSampler(Slot[] swizzleMap = null, int capacity = 1, params Sampler[] samplers) : 
            base(swizzleMap, 1)
        {
            _samplers = new List<Sampler>(samplers);
        }
        
        public override ParametricSeries GetSampledTs(ParametricSeries seriesT)
        {
            ParametricSeries result = seriesT;
            foreach (var sampler in _samplers)
            {
                result = sampler.GetSampledTs(result);
            }

            return Swizzle(result);
        }
    }
}
