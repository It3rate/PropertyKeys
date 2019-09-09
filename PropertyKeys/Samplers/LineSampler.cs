using DataArcs.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.Samplers
{
    public class LineSampler : Sampler
    {
        public override Series GetValueAtIndex(Series series, int index, int virtualCount = -1)
        {
            return series.GetDataAtIndex(index, virtualCount);
        }

        public override Series GetValueAtT(Series series, float t, int virtualCount = -1)
        {
            return series.GetValueAtT(t, virtualCount);
        }

        public override float GetTAtT(float t)
        {
            return t; // linear t doesn't change - could add invertable etc later.
        }
    }
}
