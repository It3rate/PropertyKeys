using System;
using System.Linq;
using DataArcs.SeriesData;

namespace DataArcs.Samplers
{
	public enum SampleType
	{
		Default,
		Nearest,
		Line,
		Grid,
		Ring,
		Hexagon,
	}

	public abstract class Sampler
	{
		public abstract Series GetValueAtIndex(Series series, int index, int virtualCount = -1);
		public abstract Series GetValueAtT(Series series, float t, int virtualCount = -1);
        public virtual ParametricSeries GetSampledT(float t)
        {
            return new ParametricSeries(1, t);
        }
    }
}