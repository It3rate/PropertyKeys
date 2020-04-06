using DataArcs.SeriesData;

namespace DataArcs.Samplers
{
	public class LineSampler : Sampler
	{
		public LineSampler(int sliceCount = 1) : base(sliceCount: sliceCount) { }

		public override Series GetValueAtIndex(Series series, int index)
		{
			return series.GetVirtualValueAt(index, SliceCount);
		}

		public override Series GetValuesAtT(Series series, float t)
		{
			return series.GetVirtualValueAt(t);
		}
	}
}