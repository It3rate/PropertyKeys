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
        public override Series GetValueAtIndex(Series series, int index)
        {
            return series.GetValueAtIndex(index);
        }

        public override Series GetValueAtT(Series series, float t)
        {
            return series.GetValueAtT(t);
        }

    }
}
