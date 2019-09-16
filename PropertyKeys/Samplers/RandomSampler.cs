using System;
using DataArcs.SeriesData;

namespace DataArcs.Samplers
{
	public class RandomSampler : Sampler
	{
		private readonly Random _random;
		private readonly int _seed;

		public RandomSampler(int seed = 0)
		{
			seed = seed == 0 ? SeriesUtils.Random.Next() : seed;
			_seed = seed;
			_random = seed == 0 ? new Random() : new Random(seed);
		}

		public override Series GetValueAtIndex(Series series, int index, int virtualCount = -1)
		{
			index = _random.Next(0, index);
			return series.GetDataAtIndex(index);
		}

		public override Series GetValueAtT(Series series, float t, int virtualCount = -1)
		{
			t = (float) _random.NextDouble() * t;
			return series.GetValueAtT(t);
		}
	}
}