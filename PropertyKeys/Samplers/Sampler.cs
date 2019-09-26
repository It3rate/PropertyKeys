using System;
using System.Linq;
using DataArcs.SeriesData;

namespace DataArcs.Samplers
{
	public abstract class Sampler
	{
		public int Capacity { get; protected set; } = 1;
        public abstract Series GetValueAtIndex(Series series, int index);
		public abstract Series GetValueAtT(Series series, float t);
        public virtual ParametricSeries GetSampledTs(float t)
        {
            return new ParametricSeries(1, t);
        }
        public virtual IntSeries GetSampledIndexes(float t)
        {
            return new IntSeries(1, (int)Math.Round(t * Capacity));
        }
    }
}