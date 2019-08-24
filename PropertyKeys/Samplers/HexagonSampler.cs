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
            float[] result = new float[] { 0, 0, 0 };
            float[] strideTs = GetStrideIndexes(valueStore, index);
            float[] temp = new float[] { 0, 0, 0 };
            for (int i = 0; i < valueStore.VectorSize; i++)
            {
                valueStore.GetValueAt(strideTs[i], temp);
                int curRow = (int)((float)index / valueStore.Strides[0]);
                if(i == 0 && ((curRow & 1) == 1) && valueStore.Strides[0] > 0)
                {
                    result[i] = temp[i] + (valueStore.Size[0] / (valueStore.Strides[0] - 1f) * 0.5f);// (float)Math.Sqrt(3) * 3f);
                }
                else
                {
                    result[i] = temp[i];
                }
            }
            return result;
        }
    }
}
