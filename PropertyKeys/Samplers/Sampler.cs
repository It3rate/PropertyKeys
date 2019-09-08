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

    public abstract class Sampler
    {
        public abstract Series GetValueAtIndex(Series series, int index);
        public abstract Series GetValueAtT(Series series, float t);
        public abstract float GetTAtT(float t);


        public static int[] GetDimsForIndex(Series series, int[] strides, int index)
        {
            int count = index; // series.VirtualCount - 1;
            int slot = 0;
            int dSize = 1;
            for (int i = 0; i < strides.Length; i++)
            {
                if (strides[i] > 0)
                {
                    dSize *= strides[i];
                    slot++;
                }
            }
            int[] result = new int[slot + 1];
            for (int i = slot; i >= 0; i--)
            {
                result[i] = count / dSize;
                count -= result[i] * dSize;
                if (i > 0) { dSize /= strides[i - 1]; }
            }
            return result;
        }
        public static float[] GetStrideTsForIndex(Series series, int[] strides, int index)
        {
            int[] indexes = GetDimsForIndex(series, strides, index);
            int dSize = 1;
            int maxLen = series.VirtualCount - 1;
            float[] result = new float[indexes.Length];
            for (int i = 0; i < indexes.Length; i++)
            {
                if(i < strides.Length && strides[i] > 0)
                {
                    result[i] = indexes[i] / (float)(strides[i] - 1);
                    dSize *= strides[i];
                }
                else
                {
                    result[i] = (float)(indexes[i] / Math.Floor(maxLen / (float)dSize));
                    break;
                }
            }
            return result;
        }
        public static float[] GetStrideTsForT(Series series, int[] strides, float t)
        {
            int index = (int)Math.Round(t * (series.VirtualCount - 1)); // Need an index for a strided object, so discard remainder.
            return GetStrideTsForIndex(series, strides, index);
            //index = Math.Min(index, series.VirtualCount - 1);
            float[] result = series.GetZeroSeries().Floats;
            int curSize = 1;
            int prevSize = curSize; // prevSize allows rendering to edges of grid
            // Need to sample in each dimension of the vector and completely fill the remainder once zero is hit.
            for (int i = 0; i < result.Length; i++)
            {
                bool isLast = (strides.Length - 1 < i) || (strides[i] == 0);
                float dimT;
                if (isLast)
                {
                    int totalRows = series.VirtualCount / (curSize + prevSize);
                    dimT = (int)(index / curSize) / (float)totalRows;
                }
                else
                {
                    prevSize = curSize;
                    curSize *= strides[i];
                    dimT = (index % curSize) / (float)(curSize - prevSize);
                }

                //if (i < series.EasingTypes.Length)
                //{
                //    dimT = Easing.GetTAtT(dimT, series.EasingTypes[i]);
                //}
                result[i] = dimT;

                if (isLast)
                {
                    break;
                }
            }
            return result;
        }

        public static Sampler CreateSampler(SampleType sampleType, int[] strides = null)
        {
            Sampler result;
            switch (sampleType)
            {
                case SampleType.Line:
                    result = new LineSampler();
                    break;
                case SampleType.Grid:
                    result = new GridSampler(strides);
                    break;
                case SampleType.Ring:
                    result = new RingSampler();
                    break;
                case SampleType.Hexagon:
                    result = new HexagonSampler(strides);
                    break;
                default:
                    result = new LineSampler();
                    break;
            }
            return result;
        }
    }
}
