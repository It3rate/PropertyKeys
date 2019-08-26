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
        public override float[] GetSample(FloatStore valueStore, int index)
        {
            float[] result = valueStore.GetZeroArray();
            float[] strideTs = GetStrideTsForIndex(valueStore, index);
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = valueStore.GetUnsampledValueAtT(strideTs[i])[i];
            }
            return result;
        }
        public override float[] GetSample(FloatStore valueStore, float t)
        {
            float[] result = valueStore.GetZeroArray();
            float[] strideTs = GetStrideTsForT(valueStore, t);
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = valueStore.GetUnsampledValueAtT(strideTs[i])[i];
            }
            return result;
        }

    }
}
