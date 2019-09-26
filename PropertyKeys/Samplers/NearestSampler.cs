using System;
using DataArcs.SeriesData;

namespace DataArcs.Samplers
{
	public class NearestSampler : Sampler
	{
        public override Series GetValueAtIndex(Series series, int index)
		{
			index = Math.Max(0, Math.Min(series.DataSize - 1, index));
			return series.GetSeriesAtIndex(index);
		}

		public override Series GetValueAtT(Series series, float t)
		{
			var index = (int) Math.Round(t * series.DataSize);
			return series.GetSeriesAtIndex(index);
		}
	}
}