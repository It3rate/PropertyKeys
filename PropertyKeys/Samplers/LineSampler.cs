using DataArcs.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.Samplers
{
    public class LineSampler : BaseSampler
    {
        public override float[] GetSample(IValueStore valueStore, int index)
        {
            float index_t = (valueStore.ElementCount > 1) ?  index / (valueStore.ElementCount - 1f) : 0f;
            return GetSample(valueStore, index_t);
        }
        public override float[] GetSample(IValueStore valueStore, float t)
        {
            return valueStore.GetUnsampledValueAtT(t);
        }
    }
}
