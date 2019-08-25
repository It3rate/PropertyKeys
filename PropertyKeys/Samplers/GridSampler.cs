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
            float[] result = new float[] { 0, 0, 0 };
            float[] strideTs = GetStrideIndexes(valueStore, index);
            float[] temp = new float[] { 0, 0, 0 };
            for (int i = 0; i < valueStore.VectorSize; i++)
            {
                valueStore.GetValueAt(strideTs[i], temp);
                result[i] = temp[i];
            }
            return result;
        }
        public override float[] GetSample(BaseValueStore valueStore, float t)
        {
            return valueStore.GetValueAt(t);
        }

    }
}
