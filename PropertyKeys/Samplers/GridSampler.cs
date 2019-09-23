using System;
using DataArcs.SeriesData;

namespace DataArcs.Samplers
{
	public class GridSampler : Sampler
	{
		protected int[] Strides { get; }

        public GridSampler(int[] strides)
		{
			Strides = strides;
            Capacity = strides[0];
            for (int i = 1; i < strides.Length; i++)
            {
                if (strides[i] != 0)
                {
                    Capacity *= strides[i];
                }
                else
                {
                    break;
                }
            }
        }

		public override Series GetValueAtIndex(Series series, int index)
        {
			index = Math.Max(0, Math.Min(Capacity - 1, index));
			return GetSeriesSample(series, index);
		}

		public override Series GetValueAtT(Series series, float t)
        {
			t = Math.Max(0, Math.Min(1f, t));
			var index = (int) Math.Round(t * (Capacity - 1f));
			return GetSeriesSample(series, index);
		}

        public override ParametricSeries GetSampledT(float t)
        {
            var index = (int)Math.Round(t * (Capacity - 1f));
            float[] strideTs = SamplerUtils.GetStrideTsForIndex(Capacity, Strides, index);
            //float[] resultAr = new float[Strides.Length];
            //Array.Copy(strideTs, resultAr, Strides.Length); // todo: GetStrideTsForIndex still returns extra param, need to fix
            return new ParametricSeries(Strides.Length, strideTs[1], strideTs[0]);
        }

        private Series GetSeriesSample(Series series, int index)
        {
			var result = SeriesUtils.GetFloatZeroArray(series.VectorSize);
			var strideTs = SamplerUtils.GetStrideTsForIndex(Capacity, Strides, index);

			for (var i = 0; i < result.Length; i++)
			{
				result[i] = (i < Strides.Length) ? series.GetValueAtT(strideTs[i]).FloatDataAt(i) : 0;
			}

			return SeriesUtils.Create(series, result);
		}
	}
}