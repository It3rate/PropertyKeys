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
            float indexT = index / (series.VirtualCount - 1f); // full circle
            return GetSeriesSample(series, Strides, indexT);
        }

        public override Series GetValueAtT(Series series, float t)
        {
            return GetSeriesSample(series, Strides, t);
        }

        public static Series GetSeriesSample(Series series, int[] strides, float t)
        {
            float[] result = DataUtils.GetFloatZeroArray(series.VectorSize);
            float[] strideTs = GetStrideTsForT(series, strides, t);

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = series.GetValueAtT(strideTs[i]).FloatValuesCopy[i];
            }

            return Series.Create(series, result);
        }

    }
}
