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
        public override Series GetValueAtIndex(Series series, int index)
        {
            index = Math.Max(0, Math.Min(series.DataSize - 1, index));
            return series.GetDataAtIndex(index);
        }

        public override Series GetValueAtT(Series series, float t)
        {
            int index = (int)Math.Round(t * series.DataSize);
            return series.GetDataAtIndex(index);
        }

        public override float GetTAtT(float t)
        {
            return t; // t is always nearest itself.
        }
    }
}
