using DataArcs.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.Samplers
{
    public class NearestSampler : Sampler
    {
        public override Series GetValueAtIndex(Series series, int index, int virtualCount = -1)
        {
            index = Math.Max(0, Math.Min(series.DataSize - 1, index));
            return series.GetDataAtIndex(index, virtualCount);
        }

        public override Series GetValueAtT(Series series, float t, int virtualCount = -1)
        {
            int index = (int)Math.Round(t * series.DataSize);
            return series.GetDataAtIndex(index, virtualCount);
        }

        public override float GetTAtT(float t)
        {
            return t; // t is always nearest itself.
        }
    }
}
