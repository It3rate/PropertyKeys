using MotiveCore.SeriesData;

namespace MotiveCore.Samplers
{
	public class LineSampler : Sampler
	{
		public LineSampler(int sampleCount = 1) : base(sampleCount: sampleCount) { }

		public override Series GetValuesAtT(Series series, float t)
		{
			return series.GetVirtualValueAt(t);
		}
	}
}