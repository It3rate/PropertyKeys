using DataArcs.SeriesData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.Samplers
{
    public class IdentitySampler : Sampler
    {
        public IdentitySampler()
        {
            Capacity = 1;
        }
        public override Series GetValueAtIndex(Series series, int index)
        {
            return series;
        }

        public override Series GetValuesAtT(Series series, float t)
        {
            return series;
        }
    }
}
