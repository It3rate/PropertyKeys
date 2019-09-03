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
        public override float[] GetFloatSample(Store valueStore, int index)
        {
            float indexT = (valueStore.ElementCount > 1) ?  index / (valueStore.ElementCount - 1f) : 0f;
            return GetFloatSample(valueStore, indexT);
        }
        public override float[] GetFloatSample(Store valueStore, float t)
        {
            return valueStore.GetInterpolatededValueAtT(t);
        }

        public override int[] GetIntSample(Store valueStore, int index)
        {
            return GetFloatSample(valueStore, index).ToInt();
        }

        public override int[] GetIntSample(Store valueStore, float t)
        {
            return GetFloatSample(valueStore, t).ToInt();
        }
    }
}
