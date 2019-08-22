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
            float dimT = 1f;
            int curSize = 1;
            int prevSize = curSize; // prevSize allows rendering to edges of grid
            float[] temp = new float[] { 0, 0, 0 };
            // Need to sample in each dimension of the vector and completely fill the remainder.
            for (int i = 0; i < valueKey.VectorSize; i++)
            {
                // first zero results in fill to end
                bool isLast = (valueKey.Strides.Length - 1 < i) || (valueKey.Strides[i] == 0);
                if (isLast)
                {
                    dimT = (index / curSize) / (float)(valueKey.ElementCount / (curSize + prevSize));
                }
                else
                {
                    prevSize = curSize;
                    curSize *= valueKey.Strides[i];
                    dimT = (index % curSize) / (float)(curSize - prevSize);
                }

                if (i < valueKey.EasingTypes.Length)
                {
                    dimT = Easing.GetValueAt(dimT, valueKey.EasingTypes[i]);
                }
                valueKey.GetVirtualValue(dimT, temp);
                result[i] = temp[i];

                if (isLast)
                {
                    break;
                }
            }
            return result;
        }

    }
}
