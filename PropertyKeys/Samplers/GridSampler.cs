using DataArcs.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.Samplers
{
    public class GridSampler : BaseSampler
    {
        public override float[] GetFloatSample(Store valueStore, int index)
        {
            float[] result = valueStore.GetZeroArray();
            float[] strideTs = GetStrideTsForIndex(valueStore, index);
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = valueStore.GetInterpolatededValueAtT(strideTs[i])[i];
            }
            return result;
        }
        public override float[] GetFloatSample(Store valueStore, float t)
        {
            float[] result = valueStore.GetZeroArray();
            float[] strideTs = GetStrideTsForT(valueStore, t);
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = valueStore.GetInterpolatededValueAtT(strideTs[i])[i];
            }
            return result;
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
