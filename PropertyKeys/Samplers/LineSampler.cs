using DataArcs.SeriesData;

namespace DataArcs.Samplers
{
	public class LineSampler : Sampler
	{
		public LineSampler(int sampleCount = 1) : base(sampleCount: sampleCount) { }

		public override Series GetValueAtIndex(Series series, int index)
		{
			return series.GetVirtualValueAt(index, SampleCount);
		}

		public override Series GetValuesAtT(Series series, float t)
		{
			return series.GetVirtualValueAt(t);
		}
	}
}