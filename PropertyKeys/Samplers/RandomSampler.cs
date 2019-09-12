﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataArcs.Stores;

namespace DataArcs.Samplers
{
    public class RandomSampler : Sampler
    {
        private readonly Random _random;
        private readonly int _seed;

        public RandomSampler(int seed = 0)
        {
            seed = seed == 0 ? DataUtils.Random.Next() : seed;
            _seed = seed;
            _random = seed == 0 ? new Random() : new Random(seed);
        }
        public override Series GetValueAtIndex(Series series, int index, int virtualCount = -1)
        {
            index = _random.Next(0, index);
            return series.GetSeriesAtIndex(index);
        }

        public override Series GetValueAtT(Series series, float t, int virtualCount = -1)
        {
            t = (float)_random.NextDouble() * t;
            return series.GetValueAtT(t);
        }

        public override float GetTAtT(float t)
        {
            return (float)_random.NextDouble() * t;
        }
    }
}
