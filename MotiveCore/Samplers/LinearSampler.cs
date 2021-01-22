using Motive.SeriesData;

namespace Motive.Samplers.Utils
{
	public class LinearSampler : Sampler
	{
		public LinearSampler(int sampleCount = 1) : base(sampleCount: sampleCount) { }

		public override ISeries GetValuesAtT(ISeries series, float t)
		{
			return series.GetInterpolatedSeriesAt(t);
		}
	}
}