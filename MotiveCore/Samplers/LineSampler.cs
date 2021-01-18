using Motive.SeriesData;

namespace Motive.Samplers
{
	public class LineSampler : Sampler
	{
		public LineSampler(int sampleCount = 1) : base(sampleCount: sampleCount) { }

		public override ISeries GetValuesAtT(ISeries series, float t)
		{
			return series.GetVirtualValueAt(t);
		}
	}
}