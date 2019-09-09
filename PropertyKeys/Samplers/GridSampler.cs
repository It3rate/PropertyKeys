using DataArcs.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataArcs.Samplers
{
    public class GridSampler : Sampler
    {
        protected int[] Strides { get; }

        public GridSampler(int[] strides)
        {
            Strides = strides;
        }

        public override Series GetValueAtIndex(Series series, int index)
        {
            index = Math.Max(0, Math.Min(series.VirtualCount - 1, index));
            return GetSeriesSample(series, Strides, index);
        }

        public override Series GetValueAtT(Series series, float t)
        {
            t = Math.Max(0, Math.Min(1f, t));
            return GetSeriesSample(series, Strides, (int)Math.Round(t * (series.VirtualCount - 1f)));
        }

        public override float GetTAtT(float t)
        {
            float result;
            if (Strides[0] > 0)
            {
                result = Strides[0] * t;
                result -= (int) result;
            }
            else
            {
                result = t;
            }
            return result;
        }

        public static Series GetSeriesSample(Series series, int[] strides, int index)
        {
            float[] result = DataUtils.GetFloatZeroArray(series.VectorSize);
            float[] strideTs = GetStrideTsForIndex(series, strides, index);

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = series.GetValueAtT(strideTs[i])[i];
            }

            return Series.Create(series, result);
        }

    }
}
