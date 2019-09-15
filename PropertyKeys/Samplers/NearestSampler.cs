using System;
using DataArcs.SeriesData;

namespace DataArcs.Samplers
{
	public class NearestSampler : Sampler
	{
		public override Series GetValueAtIndex(Series series, int index, int virtualCount = -1)
		{
			index = Math.Max(0, Math.Min(series.DataSize - 1, index));
			return series.GetDataAtIndex(index);
		}

		public override Series GetValueAtT(Series series, float t, int virtualCount = -1)
		{
			var index = (int) Math.Round(t * series.DataSize);
			return series.GetDataAtIndex(index);
		}

		public override float GetTAtT(float t)
		{
			return t; // t is always nearest itself.
		}
	}
}