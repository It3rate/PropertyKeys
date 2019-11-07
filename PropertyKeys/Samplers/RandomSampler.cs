using System;
using DataArcs.SeriesData;
using DataArcs.SeriesData.Utils;

namespace DataArcs.Samplers
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

		public override Series GetValueAtIndex(Series series, int index)
		{
			index = _random.Next(0, index);
			return series.GetSeriesAtIndex(index);
		}

		public override Series GetValuesAtT(Series series, float t)
		{
			t = (float) _random.NextDouble() * t;
			return series.GetValueAtT(t);
		}
	}
}