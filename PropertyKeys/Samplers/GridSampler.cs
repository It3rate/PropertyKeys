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
		}

		public override Series GetValueAtIndex(Series series, int index, int virtualCount = -1)
		{
			virtualCount = virtualCount == -1 ? series.VirtualCount : virtualCount;
			index = Math.Max(0, Math.Min(virtualCount - 1, index));
			return GetSeriesSample(series, Strides, index);
		}

		public override Series GetValueAtT(Series series, float t, int virtualCount = -1)
		{
			virtualCount = virtualCount == -1 ? series.VirtualCount : virtualCount;
			t = Math.Max(0, Math.Min(1f, t));
			var index = (int) Math.Round(t * (virtualCount - 1f));
			return GetSeriesSample(series, Strides, index, virtualCount);
		}
		
		public static Series GetSeriesSample(Series series, int[] strides, int index,
			int virtualCount = -1)
		{
			virtualCount = virtualCount == -1 ? series.VirtualCount : virtualCount;
			var result = SeriesUtils.GetFloatZeroArray(series.VectorSize);
			var strideTs = GetStrideTsForIndex(virtualCount, strides, index);

			for (var i = 0; i < result.Length; i++)
			{
				result[i] = (i < strides.Length) ? series.GetValueAtT(strideTs[i]).FloatDataAt(i) : 0;
			}

			return SeriesUtils.Create(series, result);
		}
	}
}