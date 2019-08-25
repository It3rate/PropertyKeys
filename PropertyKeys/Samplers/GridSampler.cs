using PropertyKeys.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyKeys.Samplers
{
    public class GridSampler : BaseSampler
    {
        public override float[] GetSample(BaseValueStore valueStore, int index)
        {
            float[] result = valueStore.GetZeroArray();
            float[] strideTs = GetStrideIndexes(valueStore, index);
            float[] temp = valueStore.GetZeroArray();
            for (int i = 0; i < result.Length; i++)
            {
                valueStore.GetUnsampledValueAt(strideTs[i], temp);
                result[i] = temp[i];
            }
            return result;
        }
        public override float[] GetSample(BaseValueStore valueStore, float t)
        {
            return valueStore.GetUnsampledValueAtT(t);
        }

    }
}
