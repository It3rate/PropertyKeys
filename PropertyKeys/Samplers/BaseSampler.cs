using DataArcs.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.Samplers
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
        public static BaseSampler CreateSampler(SampleType sampleType)
        {
            BaseSampler result;
            switch (sampleType)
            {
                case SampleType.Line:
                    result = new LineSampler();
                    break;
                case SampleType.Grid:
                    result = new GridSampler();
                    break;
                case SampleType.Ring:
                    result = new RingSampler();
                    break;
                case SampleType.Hexagon:
                    result = new HexagonSampler();
                    break;
                default:
                    result = new LineSampler();
                    break;
            }
            return result;
        }
        public abstract float[] GetSample(IValueStore valueStore, int index);
        public abstract float[] GetSample(IValueStore valueStore, float t);

        public float[] GetStrideTsForIndex(IValueStore valueStore, int index)
        {
            return GetStrideTsForT(valueStore, (float)index / valueStore.ElementCount);
        }
        public float[] GetStrideTsForT(IValueStore valueStore, float t)
        {
            int index = (int)(t * valueStore.ElementCount);
            float remainder = t * valueStore.ElementCount - index;
            remainder = (Math.Abs(remainder) < 0.0001) ? 0 : remainder;

            float[] result = valueStore.GetZeroArray();
            float dimT = 0;
            int curSize = 1;
            int prevSize = curSize; // prevSize allows rendering to edges of grid
            float[] temp = valueStore.GetZeroArray();
            // Need to sample in each dimension of the vector and completely fill the remainder once zero is hit.
            for (int i = 0; i < result.Length; i++)
            {
                // first zero results in fill to end
                bool isLast = (valueStore.Strides.Length - 1 < i) || (valueStore.Strides[i] == 0);
                if (isLast)
                {
                    dimT = (index / curSize) / (float)(valueStore.ElementCount / (curSize + prevSize)) + remainder;
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
