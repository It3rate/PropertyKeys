using System;
using DataArcs.SeriesData;

namespace DataArcs.Samplers
{
	public class HexagonSampler : Sampler
	{
		protected int[] Strides { get; }

        private int Capacity { get; }

		public HexagonSampler(int[] strides)
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

		public override Series GetValueAtIndex(Series series, int index, int virtualCount = -1)
		{
			virtualCount = virtualCount == -1 ? Capacity : virtualCount;
			index = Math.Max(0, Math.Min(virtualCount - 1, index));
			return GetSeriesSample(series, Strides, index);
		}

		public override Series GetValueAtT(Series series, float t, int virtualCount = -1)
		{
			virtualCount = virtualCount == -1 ? Capacity : virtualCount;
			t = Math.Max(0, Math.Min(1f, t));
			var index = (int) Math.Round(t * (virtualCount - 1f));
			return GetSeriesSample(series, Strides, index, virtualCount);
		}

        public override ParametricSeries GetSampledT(float t)
        {
            var index = (int)Math.Round(t * (Capacity - 1f));
            float[] strideTs = SamplerUtils.GetStrideTsForIndex(Capacity, Strides, index);
            float[] resultAr = new float[Strides.Length];
            Array.Copy(strideTs, resultAr, Strides.Length); // todo: GetStrideTsForIndex still returns extra param, need to fix
            return new ParametricSeries(Strides.Length, resultAr);
        }

        public static Series GetSeriesSample(Series series, int[] strides, int index, int virtualCount = -1)
		{
			virtualCount = virtualCount == -1 ? strides[0] * strides[1] : virtualCount;
			var result = SeriesUtils.GetFloatZeroArray(series.VectorSize);
			var size = series.Size.FloatData; // s0,s1...sn
			var strideTs = SamplerUtils.GetStrideTsForIndex(virtualCount, strides, index);

			for (var i = 0; i < result.Length; i++)
			{
				var temp = series.GetValueAtT(strideTs[i]).FloatData[i];
				var curRow = (int) ((float) index / strides[0]);
				if (i == 0 && (curRow & 1) == 1 && strides[0] > 0)
				{
					result[i] = temp + size[0] / (strides[0] - 1f) * 0.5f;
				}
				else
				{
					result[i] = temp;
				}
			}

			return SeriesUtils.Create(series, result);
		}
	}
}