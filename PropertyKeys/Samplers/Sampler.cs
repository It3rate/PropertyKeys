using System;
using System.Linq;
using DataArcs.SeriesData;

namespace DataArcs.Samplers
{
	public abstract class Sampler
	{
		public int Capacity { get; protected set; } = 1;
        public abstract Series GetValueAtIndex(Series series, int index);
		public abstract Series GetValuesAtT(Series series, float t);
        public virtual ParametricSeries GetSampledTs(float t)
        {
            return new ParametricSeries(1, t);
        }
    }
}