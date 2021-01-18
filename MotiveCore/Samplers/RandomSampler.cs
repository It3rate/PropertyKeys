using System;
using Motive.SeriesData;
using Motive.SeriesData.Utils;

namespace Motive.Samplers
{
	public class RandomSampler : Sampler
	{
		private readonly Random _random;
        private readonly int _seed;

        public RandomSampler(int seed = 0)
        {
	        _seed = seed == 0 ? SeriesUtils.Random.Next() : seed;
	        _random = new Random(seed);
        }

        public override ISeries GetValuesAtT(ISeries series, float t)
		{
			t = (float) _random.NextDouble() * t;
			return series.GetVirtualValueAt(t);
		}
	}
}