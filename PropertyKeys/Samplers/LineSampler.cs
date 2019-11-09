using DataArcs.SeriesData;

namespace DataArcs.Samplers
{
	public class LineSampler : Sampler
	{
		public LineSampler(int capacity = 1)
		{
			Capacity = capacity;
		}

        public override Series GetValueAtIndex(Series series, int index)
		{
			return series.GetVirtualValueAt(index, Capacity);
		}

		public override Series GetValuesAtT(Series series, float t)
		{
			return series.GetVirtualValueAt(t);
		}
	}
}