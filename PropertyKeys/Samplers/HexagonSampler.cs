using System;
using DataArcs.Series;

namespace DataArcs.Samplers
{
	public class HexagonSampler : Sampler
	{
		protected int[] Strides { get; }

		public HexagonSampler(int[] strides)
		{
			Strides = strides;
		}

		public override Series.Series GetValueAtIndex(Series.Series series, int index, int virtualCount = -1)
		{
			virtualCount = virtualCount == -1 ? series.VirtualCount : virtualCount;
			index = Math.Max(0, Math.Min(virtualCount - 1, index));
			return GetSeriesSample(series, Strides, index);
		}

		public override Series.Series GetValueAtT(Series.Series series, float t, int virtualCount = -1)
		{
			virtualCount = virtualCount == -1 ? series.VirtualCount : virtualCount;
			t = Math.Max(0, Math.Min(1f, t));
			var index = (int) Math.Round(t * (virtualCount - 1f));
			return GetSeriesSample(series, Strides, index, virtualCount);
		}

		public override float GetTAtT(float t)
		{
			float result;
			if (Strides[0] > 0)
			{
				result = Strides[0] * t;
				var row = (int) result;
				if ((row & 1) == 1)
				{
					result += 1f / (Strides[0] - 1f) * 0.5f;
				}

				result -= row;
			}
			else
			{
				result = t;
			}

			return result;
		}

		public static Series.Series GetSeriesSample(Series.Series series, int[] strides, int index,
			int virtualCount = -1)
		{
			virtualCount = virtualCount == -1 ? series.VirtualCount : virtualCount;
			var result = SeriesUtils.GetFloatZeroArray(series.VectorSize);
			var size = series.Size.FloatData; // s0,s1...sn
			var strideTs = GetStrideTsForIndex(virtualCount, strides, index);

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