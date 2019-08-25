using PropertyKeys.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyKeys.Samplers
{
    public enum SampleType
    {
        Default,
        Line,
        Grid,
        Ring,
        Hexagon,
    }

    public abstract class BaseSampler
    {
        public abstract float[] GetSample(BaseValueStore valueStore, int index);
        public abstract float[] GetSample(BaseValueStore valueStore, float t);

        public float[] GetStrideIndexes(BaseValueStore valueStore, int index)
        {
            float[] result = new float[] { 0, 0, 0 };
            float dimT = 1f;
            int curSize = 1;
            int prevSize = curSize; // prevSize allows rendering to edges of grid
            float[] temp = new float[] { 0, 0, 0 };
            // Need to sample in each dimension of the vector and completely fill the remainder.
            for (int i = 0; i < valueStore.VectorSize; i++)
            {
                // first zero results in fill to end
                bool isLast = (valueStore.Strides.Length - 1 < i) || (valueStore.Strides[i] == 0);
                if (isLast)
                {
                    dimT = (index / curSize) / (float)(valueStore.ElementCount / (curSize + prevSize));
                }
                else
                {
                    prevSize = curSize;
                    curSize *= valueStore.Strides[i];
                    dimT = (index % curSize) / (float)(curSize - prevSize);
                }

                if (i < valueStore.EasingTypes.Length)
                {
                    dimT = Easing.GetValueAt(dimT, valueStore.EasingTypes[i]);
                }
                result[i] = dimT;

                if (isLast)
                {
                    break;
                }
            }
            return result;
        }
    }
}
