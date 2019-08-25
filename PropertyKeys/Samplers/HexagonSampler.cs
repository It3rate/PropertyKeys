using PropertyKeys.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyKeys.Samplers
{
    public class HexagonSampler : BaseSampler
    {
        public override float[] GetSample(BaseValueStore valueStore, int index)
        {
            float[] result = valueStore.GetZeroArray();
            float[] strideTs = GetStrideTsForIndex(valueStore, index);
            float[] temp = valueStore.GetZeroArray();
            for (int i = 0; i < result.Length; i++)
            {
                valueStore.GetUnsampledValueAt(strideTs[i], temp);
                int curRow = (int)((float)index / valueStore.Strides[0]);
                if(i == 0 && ((curRow & 1) == 1) && valueStore.Strides[0] > 0)
                {
                    result[i] = temp[i] + (valueStore.Size[0] / (valueStore.Strides[0] - 1f) * 0.5f);
                }
                else
                {
                    result[i] = temp[i];
                }
            }
            return result;
        }
        public override float[] GetSample(BaseValueStore valueStore, float t)
        {
            return valueStore.GetUnsampledValueAtT(t);
        }
    }
}
