using System;
using System.Linq;
using DataArcs.SeriesData;

namespace DataArcs.Samplers
{
	public abstract class Sampler
	{
		public int Capacity { get; protected set; } = 1;
		public Slot[] SwizzleMap { get; set; }

		public Sampler(Slot[] swizzleMap = null, int capacity = 1)
		{
			SwizzleMap = swizzleMap;
			Capacity = capacity;
        }

        public abstract Series GetValueAtIndex(Series series, int index);
		public abstract Series GetValuesAtT(Series series, float t);
        public virtual ParametricSeries GetSampledTs(ParametricSeries seriesT)
        {
            return Swizzle(seriesT);
        }

        public ParametricSeries Swizzle(ParametricSeries series)
        {
	        ParametricSeries result;
	        if (SwizzleMap != null)
	        {
		        int len = SwizzleMap.Length;
		        result = new ParametricSeries(len, new float[len]);
		        for (int i = 0; i < len; i++)
		        {
			        int index = (int)SwizzleMap[i];
			        index = Math.Max(0, Math.Min(len - 1, index));
			        result[i] = series[index];
		        }
	        }
	        else
	        {
		        result = series;
	        }

	        return result;
        }

    }
}