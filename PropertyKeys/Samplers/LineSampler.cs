namespace DataArcs.Samplers
{
	public class LineSampler : Sampler
	{
		public override Series.Series GetValueAtIndex(Series.Series series, int index, int virtualCount = -1)
		{
			return series.GetSeriesAtIndex(index);
		}

		public override Series.Series GetValueAtT(Series.Series series, float t, int virtualCount = -1)
		{
			return series.GetValueAtT(t);
		}

		public override float GetTAtT(float t)
		{
			return t; // linear t doesn't change - could add invertable etc later.
		}
	}
}