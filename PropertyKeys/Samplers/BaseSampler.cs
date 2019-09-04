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
        Nearest,
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
        public abstract Series GetValueAtIndex(Series series, int index);
        public abstract Series GetValueAtT(Series series, float t);

        public abstract float[] GetFloatSample(Store valueStore, int index);
        public abstract float[] GetFloatSample(Store valueStore, float t);
        public abstract int[] GetIntSample(Store valueStore, int index);
        public abstract int[] GetIntSample(Store valueStore, float t);

        public float[] GetStrideTsForIndex(Store valueStore, int index)
        {
            return GetStrideTsForT(valueStore, (float)index / valueStore.ElementCount);
        }
        public float[] GetStrideTsForT(Store valueStore, float t)
        {
            int index = (int)Math.Round(t * valueStore.ElementCount);
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

        public static float[] GetStrideTsForIndex(Series series, int[] strides, int index)
        {
            return GetStrideTsForT(series, strides, (float)index / series.VirtualCount);
        }
        public static float[] GetStrideTsForT(Series series, int[] strides, float t)
        {
            int index = (int)Math.Round(t * series.VirtualCount);
            float remainder = t * series.VirtualCount - index;
            remainder = (Math.Abs(remainder) < 0.0001) ? 0 : remainder;

            float[] result = series.GetZeroSeries().FloatValuesCopy;
            float dimT = 0;
            int curSize = 1;
            int prevSize = curSize; // prevSize allows rendering to edges of grid
            float[] temp = series.GetZeroSeries().FloatValuesCopy;
            // Need to sample in each dimension of the vector and completely fill the remainder once zero is hit.
            for (int i = 0; i < result.Length; i++)
            {
                // first zero results in fill to end
                bool isLast = (strides.Length - 1 < i) || (strides[i] == 0);
                if (isLast)
                {
                    dimT = (index / curSize) / (float)(series.VirtualCount / (curSize + prevSize)) + remainder;
                }
                else
                {
                    prevSize = curSize;
                    curSize *= strides[i];
                    dimT = (index % curSize) / (float)(curSize - prevSize);
                }

                //if (i < series.EasingTypes.Length)
                //{
                //    dimT = Easing.GetValueAt(dimT, series.EasingTypes[i]);
                //}
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
