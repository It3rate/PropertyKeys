using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyKeys.Samplers
{
    public class GridSampler : BaseSampler
    {
        public override float[] GetSample(ValueKey valueKey, int index)
        {
            float[] result = new float[] { 0, 0, 0 };
            float[] strideTs = GetStrideIndexes(valueKey, index);
            float[] temp = new float[] { 0, 0, 0 };
            for (int i = 0; i < valueKey.VectorSize; i++)
            {
                valueKey.GetVirtualValue(strideTs[i], temp);
                result[i] = temp[i];
            }
            return result;
        }

    }
}
