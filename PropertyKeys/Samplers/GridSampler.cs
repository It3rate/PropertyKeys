using DataArcs.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Series;

namespace DataArcs.Samplers
{
    public class GridSampler : Sampler
    {
        protected int[] Strides { get; }

        public GridSampler(int[] strides)
        {
            Strides = strides;
        }

        public override Series.Series GetValueAtIndex(Series.Series series, int index, int virtualCount = -1)
        {
            virtualCount = (virtualCount == -1) ? series.VirtualCount : virtualCount;
            index = Math.Max(0, Math.Min(virtualCount - 1, index));
            return GetSeriesSample(series, Strides, index);
        }

        public override Series.Series GetValueAtT(Series.Series series, float t, int virtualCount = -1)
        {
            virtualCount = (virtualCount == -1) ? series.VirtualCount : virtualCount;
            t = Math.Max(0, Math.Min(1f, t));
            int index = (int)Math.Round(t * (virtualCount - 1f));
            return GetSeriesSample(series, Strides, index, virtualCount);

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

        public static Series.Series GetSeriesSample(Series.Series series, int[] strides, int index, int virtualCount = -1)
        {
            virtualCount = (virtualCount == -1) ? series.VirtualCount : virtualCount;
            float[] result = DataUtils.GetFloatZeroArray(series.VectorSize);
            float[] strideTs = GetStrideTsForIndex(virtualCount, strides, index);

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = series.GetValueAtT(strideTs[i])[i];
            }

            return SeriesUtils.Create(series, result);
        }

    }
}
