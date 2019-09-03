using DataArcs.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.Samplers
{
    public class HexagonSampler : BaseSampler
    {
        public override float[] GetFloatSample(Store valueStore, int index)
        {
            float[] result = valueStore.GetZeroArray();
            float[] strideTs = GetStrideTsForIndex(valueStore, index);
            for (int i = 0; i < result.Length; i++)
            {
                float temp = valueStore.GetUnsampledValueAtT(strideTs[i])[i];
                int curRow = (int)((float)index / valueStore.Strides[0]);
                if(i == 0 && ((curRow & 1) == 1) && valueStore.Strides[0] > 0)
                {
                    result[i] = temp + (valueStore.Size[0] / (valueStore.Strides[0] - 1f) * 0.5f);
                }
                else
                {
                    result[i] = temp;
                }
            }
            return result;
        }
        public override float[] GetFloatSample(Store valueStore, float t)
        {
            return valueStore.GetUnsampledValueAtT(t);
        }
    }
}
