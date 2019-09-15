using DataArcs.SeriesData;

namespace DataArcs.Samplers
{
	public class LineSampler : Sampler
	{
		public override Series GetValueAtIndex(Series series, int index, int virtualCount = -1)
		{
			return series.GetDataAtIndex(index);
		}

		public override Series GetValueAtT(Series series, float t, int virtualCount = -1)
		{
			return series.GetValueAtT(t);
		}

		public override float GetTAtT(float t)
		{
			return t; // linear t doesn't change - could add invertable etc later.
		}
	}
}