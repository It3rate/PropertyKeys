using System;
using System.Linq;
using DataArcs.SeriesData;

namespace DataArcs.Samplers
{
	public abstract class Sampler
	{
		public int Capacity { get; protected set; } = 1;
		public Slot[] Swizzle { get; set; }

		public Sampler(Slot[] swizzle = null, int capacity = 1)
		{
			Swizzle = swizzle;
			Capacity = capacity;
        }

        public abstract Series GetValueAtIndex(Series series, int index);
		public abstract Series GetValuesAtT(Series series, float t);
        public virtual ParametricSeries GetSampledTs(ParametricSeries seriesT)
        {
            return new ParametricSeries(1, seriesT.X);
        }
    }
}